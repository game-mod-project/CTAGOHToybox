param(
    [Parameter(Mandatory)][string]$ModsDataDir,
    [Parameter(Mandatory)][string]$OutDir
)
# data/mods의 분석 결과로 docs/05-workshop-mods의 문서를 만든다.
# 사람이 쓰는 입력 (모두 $ModsDataDir 아래):
#   mods.psv              id|이름|제작자|갱신일|크기MB|mod.info 요약|창작마당 설명 요약
#   annotations.psv       이름|등급|의미          (바닐라에 없는 블록 이름의 뜻)
#   <id>\notes.md         모드별 수기 메모
#   readme-intro.md, readme-outro.md
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$ModsDataDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ModsDataDir)
$OutDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutDir)
[IO.Directory]::CreateDirectory($OutDir) | Out-Null
$utf8 = [Text.UTF8Encoding]::new($false)
$listLimit = 300

function Cell([string]$s) { ($s -replace '\r?\n', ' ' -replace '\|', '\|').Trim() }
function Code([string]$s) { '`' + ($s -replace '`', "'" -replace '\|', '\|') + '`' }
function Last([string]$path) { $path.Substring($path.LastIndexOf('/') + 1) }
function Short([string]$s, [int]$n) { if ($s.Length -gt $n) { $s.Substring(0, $n) + '…' } else { $s } }
function Read-Psv([string]$path, [int]$cols) {
    foreach ($line in [IO.File]::ReadLines($path)) {
        if (-not $line.Trim() -or $line.StartsWith('#')) { continue }
        $p = $line.Split('|', $cols)
        if ($p.Count -lt $cols) { throw "형식 오류($path): $line" }
        , $p
    }
}
function Props($obj) { if ($null -eq $obj) { @() } else { @($obj.PSObject.Properties) } }

$mods = @(Read-Psv (Join-Path $ModsDataDir 'mods.psv') 7)
$ann = @{}
# @()로 감싼다: 줄이 하나뿐이면 PowerShell이 그 줄의 배열을 풀어 버린다
foreach ($a in @(Read-Psv (Join-Path $ModsDataDir 'annotations.psv') 3)) { $ann[$a[0]] = $a }
$conflicts = @(Import-Csv -LiteralPath (Join-Path $ModsDataDir 'cross-mod-conflicts.csv'))
$nameOf = @{}; foreach ($m in $mods) { $nameOf[$m[0]] = $m[1] }

$overview = [System.Collections.Generic.List[string]]::new()
$unionNames = @{}

foreach ($m in $mods) {
    $id = $m[0]
    $d = Join-Path $ModsDataDir $id
    $s = Get-Content -Raw -LiteralPath (Join-Path $d 'summary.json') | ConvertFrom-Json
    $files = @(Import-Csv -LiteralPath (Join-Path $d 'diff-files.csv'))
    $keys = @(Import-Csv -LiteralPath (Join-Path $d 'diff-keys.csv'))
    $units = @(Import-Csv -LiteralPath (Join-Path $d 'diff-units.csv') | Where-Object Kind -ne 'support')
    $names = @(Import-Csv -LiteralPath (Join-Path $d 'new-names.csv'))
    $paks = @(Import-Csv -LiteralPath (Join-Path $d 'pak-summary.csv'))
    $index = @(Import-Csv -LiteralPath (Join-Path $d 'file-index.csv'))

    $sb = [Text.StringBuilder]::new()
    function L([string]$x = '') { [void]$sb.AppendLine($x) }
    L "# $($m[1])"
    L
    L '이 파일은 `tools/New-ModDoc.ps1`이 생성한다. 손으로 고치지 않는다. 메모는 `data/mods/' + $id + '/notes.md`, 새 이름의 뜻은 `data/mods/annotations.psv`에서 고친다.'
    L
    L '## 개요'
    L
    L '| 항목 | 값 |'
    L '|---|---|'
    L "| 창작마당 ID | $id |"
    L "| 페이지 | https://steamcommunity.com/sharedfiles/filedetails/?id=$id |"
    L "| 제작자 | $(Cell $m[2]) |"
    L "| 분석한 판 | $(Cell $m[3]) 갱신본, $(Cell $m[4])MB |"
    L "| mod.info | $(Cell $m[5]) |"
    L "| 파일 수 | $($s.Files) (pak 안의 파일 포함) |"
    L '| 기준 | 게임 1.0.6.7의 바닐라와 비교. 조사 2026-10-11 |'
    L
    L '## 폴더 구성'
    L
    if ($paks.Count -gt 0) {
        L '`resource` 아래의 pak:'
        L
        L '| pak | 크기(MB) | 파일 수 | 많은 확장자 |'
        L '|---|---|---|---|'
        foreach ($p in $paks) { L "| $(Code ($p.Pak -replace '\\', '/')) | $($p.SizeMB) | $($p.Files) | $(Cell $p.TopExtensions) |" }
        L
    }
    $loose = @($index | Where-Object Source -eq 'loose').Count
    L "pak에 넣지 않은 낱개 파일: $($loose)개."
    L
    L '파일 상태:'
    L
    L '| 상태 | 파일 수 |'
    L '|---|---|'
    foreach ($p in (Props $s.FilesByStatus)) { L "| $($p.Name) | $($p.Value) |" }
    L
    L '## 바닐라 대비 파일'
    L
    L '| 관계 | 파일 수 | 뜻 |'
    L '|---|---|---|'
    $relMeaning = @{ 'new' = '바닐라에 없는 경로'; 'override-changed' = '바닐라와 같은 경로, 내용이 다름'; 'override-identical' = '바닐라와 같은 경로, 내용이 같음'; 'override-unhashed' = '바닐라와 같은 경로. 바이너리나 범위 밖이라 내용을 비교하지 않음'; 'in-mod-collision' = '이 모드 안에서 같은 경로가 두 번 나온 것의 둘째(내용 다름). 파일 수에 넣지 않음'; 'in-mod-duplicate' = '이 모드 안에서 같은 경로가 두 번 나온 것의 둘째(내용 같음)' }
    foreach ($p in (Props $s.FilesByRelation)) { L "| $($p.Name) | $($p.Value) | $($relMeaning[$p.Name]) |" }
    L
    L '텍스트 파일을 관계와 폴더로 나눈 것(많은 순):'
    L
    L '| 관계와 폴더 | 파일 수 |'
    L '|---|---|'
    foreach ($p in (Props $s.TextByRelationAndTop)) { L "| $(Cell $p.Name) | $($p.Value) |" }
    L
    $changed = @($files | Where-Object { $_.Relation -eq 'override-changed' -and $_.Status -in 'text', 'reference-only' } | Sort-Object VirtualPath)
    L "### 내용이 바뀐 바닐라 텍스트 파일 ($($changed.Count)개)"
    L
    if ($changed.Count -le $listLimit) { foreach ($f in $changed) { L "- $(Code $f.VirtualPath)" } }
    else {
        L "목록이 길어 폴더별 개수만 싣는다. 전체는 ``data/mods/$id/diff-files.csv``에서 ``Relation``이 ``override-changed``인 행이다."
        L
        L '| 폴더 | 파일 수 |'
        L '|---|---|'
        foreach ($g in ($changed | Group-Object { (($_.VirtualPath -split '/') | Select-Object -First 3) -join '/' } | Sort-Object Count -Descending | Select-Object -First 40)) { L "| $(Code $g.Name) | $($g.Count) |" }
    }
    L
    L '## 추가된 유닛'
    L
    L '| 종류 | 새로 추가 | 바닐라의 것을 변경 |'
    L '|---|---|---|'
    foreach ($k in 'breed', 'humanskin', 'vehicle', 'item-entity', 'stuff', 'pattern') {
        $n = @($units | Where-Object { $_.Kind -eq $k -and $_.Relation -eq 'new' }).Count
        $c = @($units | Where-Object { $_.Kind -eq $k -and $_.Relation -eq 'changed' }).Count
        L "| $k | $n | $c |"
    }
    L
    L '종류의 뜻: breed 보병, humanskin 보병 외형, vehicle 차량류 `.def`(파괴 모델용 포함), item-entity 아이템 엔티티, stuff 무기·탄약·장구, pattern 원형.'
    L
    if (@(Props $s.NewVehicleDefsByNation).Count -gt 0) {
        L '새 차량류 `.def`의 진영 폴더별 수:'
        L
        L '| `entity/-vehicle/` 아래 폴더 | `.def` 수 |'
        L '|---|---|'
        foreach ($p in (Props $s.NewVehicleDefsByNation)) { L "| $(Code $p.Name) | $($p.Value) |" }
        L
    }
    if (@(Props $s.NewBreedsByFolder).Count -gt 0) {
        L '새 보병의 폴더별 수:'
        L
        L '| `set/breed/` 아래 폴더 | 보병 수 |'
        L '|---|---|'
        foreach ($p in (Props $s.NewBreedsByFolder)) { L "| $(Code $p.Name) | $($p.Value) |" }
        L
    }
    $newUnits = @($units | Where-Object Relation -eq 'new' | Sort-Object Kind, Id)
    if ($newUnits.Count -eq 0) { L '새 유닛 파일은 없다.' }
    elseif ($newUnits.Count -le $listLimit) { L '새 유닛 파일 전체:'; L; foreach ($u in $newUnits) { L "- $($u.Kind): $(Code $u.Id)" } }
    else { L "새 유닛 파일 $($newUnits.Count)개의 전체 목록은 ``data/mods/$id/diff-units.csv``에 있다(``Relation``이 ``new``). 각 유닛의 모든 값은 ``data/mods/$id/units-*.resolved.jsonl.gz``에 있다." }
    L
    L '## 바뀐 기존 유닛'
    L
    $chg = @($units | Where-Object Relation -eq 'changed' | Sort-Object Kind, Id)
    if ($chg.Count -eq 0) { L '바닐라 유닛 파일을 바꾼 것은 없다.' }
    else {
        L '행 증감은 파일에 적힌 키·값 행을 바닐라와 비교한 것이다(펼치기 전 기준).'
        L
        L '| 종류 | 파일 | 늘어난 행 | 줄어든 행 |'
        L '|---|---|---|---|'
        foreach ($u in ($chg | Select-Object -First $listLimit)) { L "| $($u.Kind) | $(Code $u.Id) | $($u.RowsAdded) | $($u.RowsRemoved) |" }
        if ($chg.Count -gt $listLimit) { L; L "나머지 $($chg.Count - $listLimit)개는 ``data/mods/$id/diff-units.csv``에 있다." }
    }
    L
    L '## 추가된 구성'
    L
    L '두 단계로 나눠 본다.'
    L
    L "1. **바닐라 어디에도 없는 이름** ($($names.Count)행): 블록 이름, 매크로 이름, 정의 이름이 바닐라의 어떤 파일에도 나오지 않는 것. 모드가 새로 들여온 구성이다."
    L "2. **이름은 바닐라에 있지만 그 자리에 쓰인 적이 없는 키** ($($keys.Count)행에서 1번을 뺀 나머지): 기존 키를 새 위치나 새 파일 종류에서 쓴 것."
    L
    L '### 1. 바닐라에 없는 이름'
    L
    $blockNames = @($names | Where-Object Form -eq 'block')
    L "#### 블록 이름 ($(@($blockNames | ForEach-Object { Last $_.Path } | Sort-Object -Unique).Count)종)"
    L
    if ($blockNames.Count -eq 0) { L '없다.' }
    else {
        L '| 이름 | 쓰인 경로(예) | 값(예) | 파일(예) | 나온 곳 수 | 등급 | 의미 |'
        L '|---|---|---|---|---|---|---|'
        foreach ($g in ($blockNames | Group-Object { Last $_.Path } | Sort-Object Name)) {
            $x = $g.Group[0]
            $a = if ($ann.ContainsKey($g.Name)) { $ann[$g.Name] } else { @($g.Name, '미확인(자동)', '') }
            L "| $(Code $g.Name) | $(Code $x.Path) | $(Cell (Short $x.ExampleArgs 50)) | $(Code $x.ExampleFile) | $($g.Count) | $(Cell $a[1]) | $(Cell $a[2]) |"
            if (-not $unionNames.ContainsKey($g.Name)) { $unionNames[$g.Name] = [pscustomobject]@{ Mods = [System.Collections.Generic.List[string]]::new(); Path = $x.Path; Grade = $a[1]; Meaning = $a[2] } }
            $unionNames[$g.Name].Mods.Add($id)
        }
    }
    L
    foreach ($form in 'define', 'macro', 'paren-other') {
        $rows = @($names | Where-Object Form -eq $form)
        if ($rows.Count -eq 0) { continue }
        $title = @{ define = '매크로 정의'; macro = '매크로 호출'; 'paren-other' = '그 밖의 괄호 구문' }[$form]
        $distinct = @($rows | ForEach-Object { Last $_.Path } | Sort-Object -Unique)
        L "#### $title ($($distinct.Count)종)"
        L
        L '뜻은 살펴보지 않았다(미확인(자동)). 이름과 처음 나온 파일만 싣는다.'
        L
        L '| 이름 | 파일(예) |'
        L '|---|---|'
        foreach ($g in ($rows | Group-Object { Last $_.Path } | Sort-Object Name)) { L "| $(Code $g.Name) | $(Code $g.Group[0].ExampleFile) |" }
        L
    }
    L '### 2. 바닐라에 없는 키 전체'
    L
    L "원본 파일 기준으로, (그룹, 파일 종류, 경로, 형태)가 바닐라에 없는 키 $($keys.Count)개다. 1번의 이름들도 여기에 들어 있다."
    L
    if ($keys.Count -eq 0) { L '없다.' }
    else {
        L '| 그룹 | 파일 종류 | 경로 | 형태 | 값 형태 | 파일 수 | 파일(예) |'
        L '|---|---|---|---|---|---|---|'
        foreach ($k in ($keys | Sort-Object Group, FileKind, Path, Form)) { L "| $($k.Group) | $(Cell $k.FileKind) | $(Code $k.Path) | $($k.Form) | $(Cell (Short $k.ArgShapes 40)) | $($k.FileCount) | $(Code $k.ExampleFile) |" }
    }
    L
    L '### 3. 바닐라에 없는 따옴표 이름'
    L
    $qnPath = Join-Path $d 'diff-quoted-names.csv'
    # 대입문으로 나눈다: if 식의 결과로 받으면 빈 배열이 $null로 풀린다
    $qn = @()
    if (Test-Path -LiteralPath $qnPath) { $qn = @(Import-Csv -LiteralPath $qnPath) }
    L '이름이 따옴표 문자열인 블록(`{"이름" ...}`)은 경로에서 `"*"`로 묶여 있어 위의 키 목록에는 새 이름이 드러나지 않는다. 편제표의 유닛 이름, 기술 이름, 텍스처 경로처럼 이름 자체가 데이터인 자리다. 그런 자리에 이 모드가 넣은, 바닐라에 없는 이름을 경로별로 센 것이다.'
    L
    if ($qn.Count -eq 0) { L '없다.' }
    else {
        L "모두 $($qn.Count)개. 전체는 ``data/mods/$id/diff-quoted-names.csv``에 있다."
        L
        L '| 경로 | 새 이름 수 | 이름(예) | 파일(예) |'
        L '|---|---|---|---|'
        foreach ($g in ($qn | Group-Object Path | Sort-Object Count -Descending | Select-Object -First 60)) {
            $examples = (@($g.Group | Select-Object -First 6 | ForEach-Object { Code (Short $_.Name 40) }) -join ', ')
            L "| $(Code $g.Name) | $($g.Count) | $examples | $(Code $g.Group[0].ExampleFile) |"
        }
    }
    L
    L '## 파싱과 펼치기에서 걸린 것'
    L
    L "- 왕복 검사 실패: $(@($s.RoundTripFailed).Count)개" + $(if (@($s.RoundTripFailed).Count) { ' (' + ((@($s.RoundTripFailed) | ForEach-Object { Code $_ }) -join ', ') + ')' } else { '' })
    L "- 펼친 유닛 파일 중 풀리지 않은 구문이 있는 파일: $($s.UnresolvedFiles)개"
    foreach ($p in (Props $s.UnresolvedTop)) { L "  - $(Code $p.Name): $($p.Value)개 파일" }
    L "- 모드 안에서 같은 가상 경로에 내용이 다른 파일이 둘 있는 경우: $($s.Collisions)건"
    L
    L '## 분석 범위 밖에 놓인 정의 파일'
    L
    $oos = @(Props $s.OutOfScopeDefinitions)
    if ($oos.Count -eq 0) { L '없다.' }
    else {
        L '이 문서의 유닛 수와 키 목록은 `set`, `script`, `properties`, `entity/-vehicle`, `entity/humanskin`, `entity/inventory` 아래만 센다. 이 모드는 그 밖의 폴더에도 정의 파일(`.def`, `.set`, `.ext`, `.inc` 등)을 두고 있다. 아래 파일들은 분석되지 않았고 위의 어떤 수치에도 들어 있지 않다.'
        L
        L '| 폴더 | 정의 파일 수 |'
        L '|---|---|'
        foreach ($p in $oos) { L "| $(Code $p.Name) | $($p.Value) |" }
        L
        L "전체 목록은 ``data/mods/$id/file-index.csv``에서 ``Status``가 ``excluded-scope``인 행이다."
    }
    L
    L '## 창작마당 페이지의 설명'
    L
    L (Cell $m[6])
    L
    L "출처: https://steamcommunity.com/sharedfiles/filedetails/?id=$id (2026-10-11 확인)"
    L
    $notes = Join-Path $d 'notes.md'
    if (Test-Path -LiteralPath $notes) { L (Get-Content -Raw -LiteralPath $notes).TrimEnd() }
    [IO.File]::WriteAllText((Join-Path $OutDir "$id.md"), $sb.ToString(), $utf8)

    $u = { param($kind, $rel) @($units | Where-Object { $_.Kind -eq $kind -and $_.Relation -eq $rel }).Count }
    $rel = @{}; foreach ($p in (Props $s.FilesByRelation)) { $rel[$p.Name] = $p.Value }
    $overview.Add("| [$($m[1])]($id.md) | $($rel['new']) | $($rel['override-changed']) | $(& $u 'breed' 'new') | $(& $u 'vehicle' 'new') | $(& $u 'stuff' 'new') | $(@($units | Where-Object Relation -eq 'changed').Count) | $(@($blockNames | ForEach-Object { Last $_.Path } | Sort-Object -Unique).Count) | $(@($names | Where-Object Form -in 'define', 'macro').Count) | $($keys.Count) |")
    Write-Host "$id.md written"
}

# README
$sb = [Text.StringBuilder]::new()
function Out([string]$x = '') { [void]$sb.AppendLine($x) }
Out '# 창작마당 모드 분석'
Out
Out '이 파일은 `tools/New-ModDoc.ps1`이 생성한다. 앞뒤의 설명은 `data/mods/readme-intro.md`와 `readme-outro.md`에서 고친다.'
Out
Out (Get-Content -Raw -LiteralPath (Join-Path $ModsDataDir 'readme-intro.md')).TrimEnd()
Out
Out '## 한눈에 보기'
Out
Out '| 모드 | 새 파일 | 내용이 바뀐 바닐라 파일 | 새 보병 | 새 차량류 .def | 새 아이템 | 바뀐 바닐라 유닛 파일 | 새 블록 이름 | 새 매크로 이름(정의+호출 행) | 바닐라에 없는 키 |'
Out '|---|---|---|---|---|---|---|---|---|---|'
foreach ($line in $overview) { Out $line }
Out
Out '## 모드 간 관계'
Out
Out "둘 이상의 모드가 같은 가상 경로에 가진 텍스트 파일은 $($conflicts.Count)개이고 그중 $(@($conflicts | Where-Object SameContent -ne 'True').Count)개는 내용이 다르다. 전체는 ``data/mods/cross-mod-conflicts.csv``에 있다."
Out
Out '| 모드 조합 | 내용이 다른 파일 | 내용이 같은 파일 |'
Out '|---|---|---|'
foreach ($g in ($conflicts | Group-Object Mods | Sort-Object Count -Descending)) {
    $label = (($g.Name -split ' ') | ForEach-Object { $nameOf[$_] }) -join ' + '
    Out "| $label | $(@($g.Group | Where-Object SameContent -ne 'True').Count) | $(@($g.Group | Where-Object SameContent -eq 'True').Count) |"
}
Out
$small = @($conflicts | Where-Object { $_.Mods -ne '2905667604 3732639687' })
if ($small.Count -gt 0) {
    Out 'MACE와 Fusion 사이를 뺀 나머지 겹침:'
    Out
    foreach ($c in $small) { Out "- $(Code $c.VirtualPath): $((($c.Mods -split ' ') | ForEach-Object { $nameOf[$_] }) -join ', ')" }
    Out
}
Out '## 바닐라에 없는 블록 이름 전체'
Out
Out '네 모드가 들여온 블록 이름을 합친 것이다. 매크로 이름은 모드별 문서에 있다.'
Out
Out '| 이름 | 쓰인 경로(예) | 모드 | 등급 | 의미 |'
Out '|---|---|---|---|---|'
foreach ($k in ($unionNames.Keys | Sort-Object)) { $v = $unionNames[$k]; Out "| $(Code $k) | $(Code $v.Path) | $((@($v.Mods) | ForEach-Object { $nameOf[$_] }) -join ', ') | $(Cell $v.Grade) | $(Cell $v.Meaning) |" }
Out
Out (Get-Content -Raw -LiteralPath (Join-Path $ModsDataDir 'readme-outro.md')).TrimEnd()
[IO.File]::WriteAllText((Join-Path $OutDir 'README.md'), $sb.ToString(), $utf8)
Write-Host 'README.md written'
