param(
    [Parameter(Mandatory)][string]$DataDir,
    [Parameter(Mandatory)][string]$AnnotationDir,
    [Parameter(Mandatory)][string]$OutDir
)
# 유닛의 수정 가능한 속성을 "값이 적힌 파일"별로 정리한 문서를 만든다.
# 수정 가능한 속성 = 원본 파일에 값(인자)이 하나라도 적힌 블록과 매크로 호출. 모델 파일(.mdl, .mtl)은 뺀다.
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$DataDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($DataDir)
$AnnotationDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($AnnotationDir)
$OutDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutDir)
[IO.Directory]::CreateDirectory($OutDir) | Out-Null
$utf8 = [Text.UTF8Encoding]::new($false)
. "$PSScriptRoot/lib/Annotations.ps1"

function Cell([string]$s) { ($s -replace '\r?\n', ' ' -replace '\|', '\|').Trim() }
function Code([string]$s) { '`' + ($s -replace '`', "'" -replace '\|', '\|') + '`' }

# keys-<그룹>.csv의 한 파일 종류에서 값이 있는 행을 고른다
function Get-CsvRows([string]$Group, [string]$FileKind) {
    foreach ($r in $script:keys[$Group]) {
        if ($r.FileKind -ne $FileKind -or $r.Form -notin 'block', 'macro') { continue }
        if (-not @($r.ArgShapes -split ' ' | Where-Object { $_ -and $_ -notmatch '^\[\]x\d+$' })) { continue }
        $value = if ($r.NumMin -ne '') { if ($r.NumMin -eq $r.NumMax) { "$($r.NumMin)" } else { "$($r.NumMin) ~ $($r.NumMax)" } }
        elseif ($r.Values -and $r.Values -ne '(13종 이상)') { $r.Values }
        else { "예: $($r.ExampleArgs) (13종 이상)" }
        [pscustomobject]@{ FileKind = $r.FileKind; Path = $r.Path; Form = $r.Form; FileCount = [int]$r.FileCount; Value = $value; Comments = $r.Comments; Example = $r.ExampleFile }
    }
}

# units-shared.jsonl.gz에서 조건에 맞는 파일만 골라 같은 형태로 집계한다.
# keys-shared.csv는 공용 그룹 전체를 합친 것이라 폴더별로 나눌 수 없다
$numberRegex = [regex]'^[+-]?(\d+\.?\d*|\.\d+)$'
function Get-SharedRows([scriptblock]$Match) {
    $path = Join-Path $DataDir 'units-shared.jsonl.gz'
    if (-not (Test-Path -LiteralPath $path) -or (Get-Item -LiteralPath $path).Length -eq 0) { return }
    $agg = [ordered]@{}
    $gz = [IO.Compression.GZipStream]::new([IO.File]::OpenRead($path), [IO.Compression.CompressionMode]::Decompress)
    $sr = [IO.StreamReader]::new($gz)
    try {
        while ($null -ne ($line = $sr.ReadLine())) {
            if ($line -notmatch '^\{"Id":"([^"]+)"' -or -not (& $Match $Matches[1])) { continue }
            $id = $Matches[1]
            $kind = [IO.Path]::GetExtension($id); if (-not $kind) { $kind = '(no-ext)' }
            foreach ($raw in (ConvertFrom-Json $line).Raw) {
                if ($raw[1] -notin 'block', 'macro') { continue }
                $k = "$kind`t$($raw[0])`t$($raw[1])"
                $a = $agg[$k]
                if (-not $a) {
                    $a = [pscustomobject]@{ FileKind = $kind; Path = $raw[0]; Form = $raw[1]; FileCount = 0; Last = ''; Values = [ordered]@{}; AllNum = $true; Min = $null; Max = $null; MinText = ''; MaxText = ''; Example = $id }
                    $agg[$k] = $a
                }
                if ($a.Last -ne $id) { $a.FileCount++; $a.Last = $id }
                $v = [string]$raw[2]
                if (-not $v) { continue }
                if ($a.AllNum -and $numberRegex.IsMatch($v)) {
                    $d = [double]::Parse($v, [cultureinfo]::InvariantCulture)
                    if ($null -eq $a.Min -or $d -lt $a.Min) { $a.Min = $d; $a.MinText = $v }
                    if ($null -eq $a.Max -or $d -gt $a.Max) { $a.Max = $d; $a.MaxText = $v }
                }
                else { $a.AllNum = $false }
                if ($a.Values.Count -le 12) { $a.Values[$v] = 1 }
            }
        }
    }
    finally { $sr.Dispose() }
    foreach ($a in $agg.Values) {
        if ($a.Values.Count -eq 0) { continue }
        $value = if ($a.AllNum) { if ($a.Min -eq $a.Max) { $a.MinText } else { "$($a.MinText) ~ $($a.MaxText)" } }
        elseif ($a.Values.Count -le 12) { @($a.Values.Keys) -join ' ¦ ' }
        else { "예: $(@($a.Values.Keys)[0]) (13종 이상)" }
        $c = $script:sharedComments["$($a.FileKind)`t$($a.Path)`t$($a.Form)"]
        [pscustomobject]@{ FileKind = $a.FileKind; Path = $a.Path; Form = $a.Form; FileCount = $a.FileCount; Value = $value; Comments = [string]$c; Example = $a.Example }
    }
}

$script:keys = @{}
foreach ($g in 'human', 'vehicle', 'weapon-ammo', 'inventory') { $script:keys[$g] = @(Import-Csv -LiteralPath (Join-Path $DataDir "keys-$g.csv")) }
$script:sharedComments = @{}
foreach ($k in (Import-Csv -LiteralPath (Join-Path $DataDir 'keys-shared.csv'))) { if ($k.Comments) { $script:sharedComments["$($k.FileKind)`t$($k.Path)`t$($k.Form)"] = $k.Comments } }
$script:ann = @{}
foreach ($g in 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared') { $script:ann[$g] = Read-Annotations -AnnotationDir $AnnotationDir -Group $g }

# 매크로 정의를 읽어 "인자 → 그 인자가 들어가는 키와 식"을 모은다.
# (define "mobility_tank" {locomotion {maxspeed %speed} {normal (* %speed 0.6)}}) 이면
#   mobility_tank / speed → locomotion/maxspeed = %speed, locomotion/normal = ( * %speed 0.6 )
$defineRegex = [regex]'\(define "([^"]+)"\)(?!.*\(define ")/?(.*)$'
$paramRegex = [regex]'%(\w+)'
function Read-MacroIndex {
    $index = @{}
    foreach ($g in 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared') {
        $path = Join-Path $DataDir "units-$g.jsonl.gz"
        if (-not (Test-Path -LiteralPath $path) -or (Get-Item -LiteralPath $path).Length -eq 0) { continue }
        $sr = [IO.StreamReader]::new([IO.Compression.GZipStream]::new([IO.File]::OpenRead($path), [IO.Compression.CompressionMode]::Decompress))
        try {
            while ($null -ne ($line = $sr.ReadLine())) {
                if (-not $line.Contains('(define \"')) { continue }
                foreach ($raw in (ConvertFrom-Json $line).Raw) {
                    $value = [string]$raw[2]
                    if ($value.IndexOf('%') -lt 0) { continue }
                    $m = $defineRegex.Match($raw[0])
                    if (-not $m.Success -or -not $m.Groups[2].Value) { continue }
                    $name = $m.Groups[1].Value.ToLowerInvariant(); $tail = $m.Groups[2].Value
                    if (-not $index.ContainsKey($name)) { $index[$name] = [ordered]@{} }
                    foreach ($pm in $paramRegex.Matches($value)) {
                        $arg = $pm.Groups[1].Value
                        # 숫자로 시작하면 위치 인자다: %0_left 에서 0
                        if ($arg -match '^(\d+)\D') { $arg = $Matches[1] }
                        if (-not $index[$name].Contains($arg)) { $index[$name][$arg] = [ordered]@{} }
                        $index[$name][$arg]["$tail`t$value"] = 1
                    }
                }
            }
        }
        finally { $sr.Dispose() }
    }
    $index
}
$script:macros = Read-MacroIndex

# $Parent: 이 파일의 내용이 펼쳐지는 블록 이름을 돌려주는 스크립트 블록. 없으면 $null
function CsvSection([string]$Role, [string]$Name, [string]$Where, [string]$Link, [string]$Group, [string]$FileKind, [scriptblock]$Parent) {
    $s = @{ Role = $Role; Name = $Name; Where = $Where; Link = $Link; Group = $Group; Rows = @(Get-CsvRows $Group $FileKind) }
    if ($Parent) { $s.Parent = $Parent }
    $s
}
function SharedSection([string]$Role, [string]$Name, [string]$Where, [string]$Link, [scriptblock]$Match, [scriptblock]$Parent) {
    $s = @{ Role = $Role; Name = $Name; Where = $Where; Link = $Link; Group = 'shared'; Rows = @(Get-SharedRows $Match) }
    if ($Parent) { $s.Parent = $Parent }
    $s
}

$pages = @(
    @{
        File = 'infantry.md'; Title = '보병'
        Intro = '보병 하나는 `set/breed` 아래의 `.set` 파일 하나다. 능력치는 본체에 직접 적히지 않고 `perks`와 `veterancy` 안에서 매크로로 불러오므로, 능력치를 바꾸려면 연관 파일의 매크로 본문을 고치거나 본체의 매크로 호출 인자를 고친다. 들고 있는 무기와 장구의 속성은 [weapon-ammo.md](weapon-ammo.md)에 있다.'
        Sections = @(
            (CsvSection '본체' '보병 정의' 'set/breed/**/*.set' '유닛 그 자체. 파일 하나가 보병 하나' 'human' '.set'),
            (CsvSection '연관' '능력치 매크로' 'set/breed/**/*.inc' '본체의 `perks`, `veterancy` 안에서 `(include)`와 매크로 호출로 불러온다. 폴더의 `ability.inc`가 `set/breed/presets/perks/`의 공용 정의를 include한다' 'human' '.inc' { param($r) if ($r.Example -like '*veterancy*') { 'veterancy' } else { 'perks' } }),
            (CsvSection '연관' '외형 엔티티' 'entity/humanskin/**/*.def' '본체의 `skin`이 이 파일의 이름을 가리킨다' 'human' '.def')
        )
    },
    @{
        File = 'vehicle.md'; Title = '차량류 (차량, 전차, 화포, 항공기, 함정)'
        Intro = '차량류 하나는 `entity/-vehicle` 아래의 `.def` 파일 하나다. 많은 값이 본체가 아니라 include하는 파일에 있다. 장갑·기동 같은 차종 공통 값은 `properties/`의 공통 속성에, 그 차량만의 값은 같은 폴더의 `.ext`와 `.inc`에 있다. 탑재 무기와 포탄의 속성은 [weapon-ammo.md](weapon-ammo.md)에 있다.'
        Sections = @(
            (CsvSection '본체' '차량 엔티티' 'entity/-vehicle/<진영>/<분류>/<이름>/<이름>.def' '유닛 그 자체. 파괴 모델용 `.def`도 같은 형식이다' 'vehicle' '.def'),
            (CsvSection '연관' '차량 전용 속성 (.ext)' 'entity/-vehicle/**/*.ext' '본체의 `(include "이름.ext")`' 'vehicle' '.ext' { 'game_entity' }),
            (CsvSection '연관' '차량 전용 속성 (.inc)' 'entity/-vehicle/**/*.inc' '본체나 다른 차량의 `(include "../이름/이름.inc")`. 파생형이 기본형의 값을 물려받을 때 쓴다' 'vehicle' '.inc' { 'game_entity' }),
            (SharedSection '연관' '공통 속성 (.ext)' 'properties/**/*.ext' '본체의 `(include "/properties/...")`. 같은 차종의 모든 유닛이 공유한다' { param($id) $id.StartsWith('properties/') -and $id.EndsWith('.ext') } { 'game_entity' }),
            (SharedSection '연관' '공통 속성 (.inc)' 'properties/**/*.inc' '본체와 공통 속성 `.ext`의 `(include "/properties/...")`. 같은 차종의 모든 유닛이 공유한다' { param($id) $id.StartsWith('properties/') -and $id.EndsWith('.inc') } { 'game_entity' })
        )
    },
    @{
        File = 'weapon-ammo.md'; Title = '무기, 탄약, 장구'
        Intro = '무기·탄약·수류탄·지뢰·장구는 모두 `set/stuff` 아래에 있다. 보병은 `inventory/item`과 `armors`로, 차량은 `weaponry/place/weapon`과 `extender "inventory"/box/item`으로 이 파일들을 가리킨다. 한 파일을 여러 유닛이 함께 쓰므로 여기의 값을 고치면 그 무기나 탄약을 쓰는 모든 유닛이 바뀐다.'
        Sections = @(
            (CsvSection '본체' '무기와 장구 정의 (확장자 없음)' 'set/stuff/<폴더>/<이름>' '유닛의 `inventory/item`, `armors/*`, `weaponry/place/weapon`이 이 파일의 이름을 가리킨다. `gun`, `mgun`, `rifle`, `smg`, `pistol`, `head` 등의 폴더' 'weapon-ammo' '(no-ext)'),
            (CsvSection '본체' '무기 정의 (.weapon)' 'set/stuff/{reactive,mortar,bazooka,explosive}/*.weapon' '위와 같다. 로켓, 박격포, 대전차 화기, 폭발물' 'weapon-ammo' '.weapon'),
            (CsvSection '본체' '탄약 (.ammo)' 'set/stuff/**/*.ammo' '무기의 `filling`과 차량의 `extender "inventory"/box/item`이 탄약 태그로 가리킨다' 'weapon-ammo' '.ammo'),
            (CsvSection '본체' '수류탄 (.grenade)' 'set/stuff/grenade/*.grenade' '보병의 `inventory/item`' 'weapon-ammo' '.grenade'),
            (CsvSection '본체' '지뢰와 폭발물 (.mine)' 'set/stuff/explosive/*.mine' '보병의 `inventory/item`' 'weapon-ammo' '.mine'),
            (CsvSection '연관' '투척 원형 (.hand)' 'set/stuff/thrower.hand' '`{from "hand thrower"}`로 물려받는 원형이다(`set/stuff/thrower_v`). 그 `thrower_v`를 가리키는 줄은 바닐라에서 `##`로 꺼진 `gaz98.def`의 무장 자리 하나뿐이라 실제로는 쓰이지 않는 것으로 보인다' 'weapon-ammo' '.hand'),
            (CsvSection '연관' '원형 (.pattern)' 'set/stuff/**/*.pattern' '무기와 탄약의 `{from "pattern 이름"}`. 같은 원형을 쓰는 모든 아이템이 값을 물려받는다' 'weapon-ammo' '.pattern'),
            (CsvSection '연관' '폴더의 매크로 모음 (.presets)' 'set/stuff/<폴더>/.presets' '그 폴더와 하위 폴더의 파일이 include 없이 쓰는 매크로 정의. `("fire rate" rpm(240))` 같은 호출의 본문이 여기에 있다' 'weapon-ammo' '.presets' { 'from' }),
            (CsvSection '연관' '무기 상태 설정 (.fsm)' 'set/stuff/_settings/*.fsm' '아이템의 `inventory` 안 `{fsm "X"}`가 `set/stuff/_settings/X.fsm`을 가리킨다(아이템의 폴더와 상관없다). 바닐라의 값 6종과 파일 6개가 1:1이다(belt 132, stuff 35, hand-only 9, smg 2, bazooka·pistol 1). 내용은 줍기·들기·싣기 상태 이름이다' 'weapon-ammo' '.fsm'),
            (SharedSection '연관' '폭발 계산 매크로' 'set/blast.inc' '탄약의 `("he_level_3")` 같은 매크로 호출과 `.presets`의 `(include "\set\blast.inc")`' { param($id) $id -eq 'set/blast.inc' })
        )
    },
    @{
        File = 'item-entity.md'; Title = '아이템 엔티티'
        Intro = '아이템이 땅에 놓이거나 손에 들렸을 때의 3D 엔티티다. `set/stuff`의 아이템이 `entity`로 이 파일을 가리킨다. 전투 수치는 여기가 아니라 [weapon-ammo.md](weapon-ammo.md)의 파일에 있다.'
        Sections = @(
            (CsvSection '본체' '아이템 엔티티' 'entity/inventory/**/*.def' '`set/stuff` 아이템의 `entity`' 'inventory' '.def')
        )
    }
)

$header = '| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |'
$sep = '|---|---|---|---|---|---|'
$gradeOrder = '확인', '문서', '추정', '정의'
$summary = [Collections.Generic.List[object]]::new()

# 설명 끝의 근거 괄호를 뗀다: "최고 속도, km/h (추정: 이름)" → "최고 속도, km/h"
function Get-ShortMeaning([string]$m) { ($m -replace '\s*\((추정|같은 자리)[^)]*\)\s*$', '').Trim() }

# 매크로 호출의 설명: 인자마다 그 인자가 들어가는 키와 식
function Get-MacroMeaning([string]$Path, [hashtable]$Tails) {
    $name = $Path.Substring($Path.LastIndexOf('/') + 1)
    if ($name -notmatch '^\("(.+)"\)$') { return $null }
    $def = $script:macros[$Matches[1].ToLowerInvariant()]
    if (-not $def) { return $null }
    $parts = foreach ($arg in $def.Keys) {
        $targets = @($def[$arg].Keys)
        $shown = foreach ($t in ($targets | Select-Object -First 4)) {
            $tail, $expr = $t.Split("`t", 2)
            if ($expr -eq "%$arg" -or $expr -eq "`"%$arg`"") {
                $plain = @(ConvertTo-PlainKeyPath "/$tail")
                $known = if ($plain.Count -ge 2) { $Tails["block`t$($plain[-2..-1] -join '/')"] }
                if ($known -and $known.Count -eq 1) { "``$tail`` [$(Get-ShortMeaning @($known.Keys)[0])]" } else { "``$tail``" }
            }
            else { "``$tail`` = ``$expr``" }
        }
        $more = if ($targets.Count -gt 4) { " 외 $($targets.Count - 4)곳" } else { '' }
        $label = if ($arg -match '^\d+$') { "args의 $([int]$arg + 1)번째" } else { "``$arg``" }
        "$label → $($shown -join ', ')$more"
    }
    "매크로 호출. 인자가 들어가는 자리: $($parts -join '; ')"
}

foreach ($page in $pages) {
    # 1차: 주석 파일에서 직접 찾는다
    $tails = @{}; $roots = @{}
    # 주석 파일에 정확한 경로로 적힌 설명도 같은 자리의 근거가 된다 (펼친 뒤에만 나타나는 경로 포함)
    foreach ($k in $script:ann[$page.Sections[0].Group].Keys) {
        $kind, $apath, $aform = $k.Split("`t")
        $a = $script:ann[$page.Sections[0].Group][$k]
        if ($kind -ne '*' -or -not $apath.StartsWith('/') -or $a.Grade -notin '확인', '문서', '추정') { continue }
        $plain = @(ConvertTo-PlainKeyPath $apath)
        for ($i = 0; $i -le $plain.Count - 2; $i++) {
            $tk = "$aform`t$($plain[$i..($plain.Count - 1)] -join '/')"
            if (-not $tails.ContainsKey($tk)) { $tails[$tk] = @{} }
            $tails[$tk][$a.Meaning] = 1
        }
    }
    foreach ($s in $page.Sections) {
        foreach ($r in $s.Rows) {
            $a = Get-Annotation $script:ann[$s.Group] $r.FileKind $r.Path $r.Form ($s.Group -ne 'shared' -and $UnitFileKinds -contains $r.FileKind)
            $r | Add-Member -NotePropertyName Grade -NotePropertyValue $a.Grade -Force
            $r | Add-Member -NotePropertyName Meaning -NotePropertyValue $a.Meaning -Force
            if ($a.Grade -notin '확인', '문서', '추정') { continue }
            # 같은 자리의 다른 파일이 물려받을 수 있게 경로 꼬리로 등록한다
            $plain = @(ConvertTo-PlainKeyPath $r.Path)
            for ($i = 0; $i -le $plain.Count - 2; $i++) {
                $k = "$($r.Form)`t$($plain[$i..($plain.Count - 1)] -join '/')"
                if (-not $tails.ContainsKey($k)) { $tails[$k] = @{} }
                $tails[$k][$a.Meaning] = 1
            }
            if ($s.Role -eq '본체' -and $plain.Count -eq 2) {
                $k = "$($r.Form)`t$($plain[1])"
                if (-not $roots.ContainsKey($k)) { $roots[$k] = @{} }
                $roots[$k][$a.Meaning] = 1
            }
        }
    }
    # 2차: 매크로 호출은 정의에서, 나머지는 본체의 같은 자리에서 가져온다
    foreach ($s in $page.Sections) {
        foreach ($r in $s.Rows) {
            if ($r.Grade -in '확인', '문서', '추정') { continue }
            if ($r.Form -eq 'macro') {
                $m = Get-MacroMeaning $r.Path $tails
                if ($m) { $r.Grade = '정의'; $r.Meaning = $m }
                continue
            }
            $plain = @(ConvertTo-PlainKeyPath $r.Path)
            # 같은 자리를 가리키는 경로 후보: 그대로, 이 파일이 펼쳐지는 블록을 앞에 붙인 것,
            # 매크로 이름을 부모로 본 것((define "locomotion") {maxspeed ..} → locomotion/maxspeed)
            $candidates = [Collections.Generic.List[object]]::new()
            $candidates.Add($plain)
            if ($s.ContainsKey('Parent')) { $candidates.Add(@(& $s.Parent $r) + $plain) }
            $dm = $defineRegex.Match($r.Path)
            if ($dm.Success) { $candidates.Add(@($dm.Groups[1].Value.ToLowerInvariant()) + @(ConvertTo-PlainKeyPath "/$($dm.Groups[2].Value)")) }
            $found = $null
            foreach ($cand in $candidates) {
                for ($i = 0; $i -le $cand.Count - 2 -and -not $found; $i++) {
                    $hit = $tails["$($r.Form)`t$($cand[$i..($cand.Count - 1)] -join '/')"]
                    if ($hit -and $hit.Count -eq 1) { $found = @($hit.Keys)[0] }
                }
            }
            if (-not $found -and $plain.Count -eq 1) {
                $hit = $roots["$($r.Form)`t$($plain[0])"]
                if ($hit -and $hit.Count -eq 1) { $found = @($hit.Keys)[0] }
            }
            if ($found) { $r.Grade = '추정'; $r.Meaning = "$(Get-ShortMeaning $found) (같은 자리의 본체 설명을 가져옴)" }
        }
    }

    $sb = [Text.StringBuilder]::new()
    [void]$sb.AppendLine("# 수정 가능한 속성: $($page.Title)")
    [void]$sb.AppendLine()
    [void]$sb.AppendLine('이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다. 설명은 `data/annotations/common.psv`에서 고친다. 표 읽는 법은 [README.md](README.md)에 있다.')
    [void]$sb.AppendLine()
    [void]$sb.AppendLine($page.Intro)
    [void]$sb.AppendLine()
    [void]$sb.AppendLine('| 구분 | 파일 | 위치 | 속성 수 | 확인 | 문서 | 추정 | 정의 | 미확인 |')
    [void]$sb.AppendLine('|---|---|---|---|---|---|---|---|---|')
    $body = [Text.StringBuilder]::new()
    foreach ($s in $page.Sections) {
        $known = [Collections.Generic.List[string]]::new(); $unknown = [Collections.Generic.List[string]]::new()
        $count = @{}; foreach ($g in $gradeOrder) { $count[$g] = 0 }
        foreach ($r in ($s.Rows | Sort-Object Path, Form)) {
            $isKnown = $r.Grade -in $gradeOrder
            $desc = if ($isKnown) { "**$($r.Grade)** $($r.Meaning)" } elseif ($r.Meaning) { "미확인. $($r.Meaning)" } else { '미확인' }
            $row = "| $(Code $r.Path) | $(Cell $r.Value) | $(Cell $desc) | $(Cell $r.Comments) | $($r.FileCount) | $(Code $r.Example) |"
            if ($isKnown) { $known.Add($row); $count[$r.Grade]++ } else { $unknown.Add($row) }
        }
        $total = $known.Count + $unknown.Count
        [void]$sb.AppendLine("| $($s.Role) | $($s.Name) | $(Code $s.Where) | $total | $($count['확인']) | $($count['문서']) | $($count['추정']) | $($count['정의']) | $($unknown.Count) |")
        $summary.Add([pscustomobject]@{ Page = $page.File; Role = $s.Role; Name = $s.Name; Where = $s.Where; Total = $total
                Confirmed = $count['확인']; Documented = $count['문서']; Inferred = $count['추정']; Defined = $count['정의']; Unknown = $unknown.Count })

        [void]$body.AppendLine("## $($s.Role): $($s.Name)")
        [void]$body.AppendLine()
        [void]$body.AppendLine("- 위치: $(Code $s.Where)")
        [void]$body.AppendLine("- 연결: $($s.Link)")
        [void]$body.AppendLine()
        if ($total -eq 0) { [void]$body.AppendLine('값이 적힌 속성이 없다.'); [void]$body.AppendLine(); continue }
        if ($known.Count -gt 0) {
            [void]$body.AppendLine("### 뜻을 아는 속성 ($($known.Count)개)")
            [void]$body.AppendLine()
            [void]$body.AppendLine($header); [void]$body.AppendLine($sep)
            foreach ($row in $known) { [void]$body.AppendLine($row) }
            [void]$body.AppendLine()
        }
        if ($unknown.Count -gt 0) {
            [void]$body.AppendLine("### 뜻을 모르는 속성 ($($unknown.Count)개)")
            [void]$body.AppendLine()
            [void]$body.AppendLine($header); [void]$body.AppendLine($sep)
            foreach ($row in $unknown) { [void]$body.AppendLine($row) }
            [void]$body.AppendLine()
        }
    }
    [void]$sb.AppendLine()
    [void]$sb.Append($body.ToString())
    [IO.File]::WriteAllText((Join-Path $OutDir $page.File), $sb.ToString(), $utf8)
    $mine = @($summary | Where-Object Page -eq $page.File)
    Write-Host "$($page.File) attributes=$(($mine | Measure-Object Total -Sum).Sum) unknown=$(($mine | Measure-Object Unknown -Sum).Sum)"
}
$sb = [Text.StringBuilder]::new()
[void]$sb.AppendLine(@'
# 유닛의 수정 가능한 속성

이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다.

유닛의 값을 바꾸려 할 때 "그 값이 어느 파일에 있는가"를 찾기 위한 문서다. 유닛 종류마다, 값이 적힌 파일을 본체와 연관 파일로 나눠 속성명·속성값·설명을 실었다.

## 문서

| 문서 | 대상 |
|---|---|
| [infantry.md](infantry.md) | 보병 |
| [vehicle.md](vehicle.md) | 차량, 전차, 화포, 항공기, 함정 |
| [weapon-ammo.md](weapon-ammo.md) | 무기, 탄약, 수류탄, 지뢰, 장구 |
| [item-entity.md](item-entity.md) | 아이템의 3D 엔티티 |

## 파일별 속성 수

'@)
[void]$sb.AppendLine('| 문서 | 구분 | 파일 | 위치 | 속성 수 | 확인 | 문서 | 추정 | 정의 | 미확인 |')
[void]$sb.AppendLine('|---|---|---|---|---|---|---|---|---|---|')
foreach ($s in $summary) { [void]$sb.AppendLine("| [$($s.Page)]($($s.Page)) | $($s.Role) | $($s.Name) | $(Code $s.Where) | $($s.Total) | $($s.Confirmed) | $($s.Documented) | $($s.Inferred) | $($s.Defined) | $($s.Unknown) |") }
$sum = { param($n) ($summary | Measure-Object $n -Sum).Sum }
[void]$sb.AppendLine("| 합계 | | | | $(& $sum Total) | $(& $sum Confirmed) | $(& $sum Documented) | $(& $sum Inferred) | $(& $sum Defined) | $(& $sum Unknown) |")
[void]$sb.AppendLine(@'

## 표 읽는 법

| 열 | 내용 |
|---|---|
| 속성명 | 파일 안에서의 키 경로. 표기 규칙은 [../04-units/README.md](../04-units/README.md)의 "경로 표기"와 같다. `("fire rate")`처럼 괄호와 따옴표로 된 이름은 매크로 호출이고 값은 그 인자다. `(define "이름")/...`은 매크로 본문 안의 값이다 |
| 속성값 | 바닐라 파일에 실제로 쓰인 값. 숫자 하나면 최솟값 ~ 최댓값, 종류가 12개 이하면 전부, 그보다 많으면 예 하나 |
| 설명 | 맨 앞의 굵은 낱말이 근거의 종류다. 아래 "설명의 등급"을 본다 |
| 게임 파일의 주석 | 같은 키 옆에 개발자가 남긴 주석을 모은 것. 그 줄의 값에 대한 메모인 경우가 많아 키의 뜻과 다를 수 있다 |
| 파일 수 | 이 속성이 적힌 파일의 수 |
| 예시 파일 | 이 속성이 적힌 파일 하나 |

구분의 **본체**는 유닛 그 자체인 파일이고, **연관**은 본체가 include·매크로·이름으로 끌어오는 파일이다. 연관 파일의 값은 여러 유닛이 함께 쓴다.

## 설명의 등급

| 등급 | 근거 |
|---|---|
| 확인 | 개발자가 게임 파일에 남긴 주석이 그 키를 설명한다 |
| 문서 | 게임 밖의 문서에 설명이 있다. 대부분 같은 엔진을 쓰는 다른 게임의 문서이므로 이 게임에서도 같은지는 확인하지 않았다 |
| 추정 | 이름, 값, 쓰인 자리에서 추론했다. "(같은 자리의 본체 설명을 가져옴)"이 붙은 것은 본체 파일의 같은 경로에 달린 설명을 include되는 파일과 매크로 정의 안의 키에 옮겨 적은 것이다 |
| 정의 | 매크로 호출이다. 그 매크로의 정의를 읽어 인자마다 어느 키에 어떤 식으로 들어가는지를 적었다. 뜻풀이가 아니라 파일에서 기계적으로 뽑은 사실이다 |
| 미확인 | 근거를 찾지 못했다 |

"정의"의 식은 게임 파일의 전위 표기 그대로다. `( * %speed 0.6 )`은 speed × 0.6, `( / 360 %traverse )`는 360 ÷ traverse다. 대괄호 안은 그 키의 설명이다.

## 설명의 출처

2026-10-12에 조사했다. 키를 망라한 공식 레퍼런스는 찾지 못했다. 창작마당은 열려 있지만 개발사가 공개한 키 설명서는 없고, 가이드 작성자들은 공식 Discord를 문의처로 든다(Discord는 읽지 못했다).

| 출처 | 성격 | 얻은 것 |
|---|---|---|
| 게임 파일의 주석 | 개발사. 이 게임 | 구동계, 보급 구역, 체력·스태미나, 조준 화면 등. 가장 믿을 만한 근거다 |
| 개발자 변경 기록 (Steam 토론 4498514133983766024) | 개발사. 이 게임 | zeroing, FiringTimeoutDistanceFactor, detonationThickness, no_target 태그, extender "edifice" |
| docs.bestway.com.ua | 엔진 개발사(Best Way)의 공식 문서. Men of War II 기준 | 설정 문법, 장갑 블록, 보병 이동 속도 표, 상태 기계(fsm), 센서 |
| wiki.bestway.com.ua (web.archive.org 보존본) | 엔진 개발사의 옛 공식 위키. В тылу врага 2 기준 | 볼륨의 able 플래그, land_patch, 구동계 키의 단위 |
| calltoarms.fandom.com의 Modding 문서 | 커뮤니티 위키. Call to Arms 기준. 스스로 추측이 섞였다고 밝힌다 | 사거리 두 값, 관통력 표, 체력 피해 |
| ModDB, mow-portal.ru, xgm.guru, Steam 토론과 가이드 | 커뮤니티 글. Men of War 계열과 이 게임 | 무장 자리, 뼈대 속도, 장갑 밀도, 대포 애니메이션 |

이 게임(Gates of Hell)은 Barbedwire Studios가 GEM 2 엔진을 갈라 내어 만든 것이라, 다른 게임의 문서가 설명하는 키가 이 게임에서 같은 뜻인지는 보장되지 않는다. 그런 설명은 "문서" 등급으로 따로 표시했다.

조사에서 모은 항목 전체(출처 주소 포함)는 `data/annotations/research/findings-*.psv`에 있다. 컨퀘스트와 게임 모드 설정 키에 대한 항목도 거기에 있다.

## 무엇을 "수정 가능한 속성"으로 셌나

- 원본 파일에 값(인자)이 하나라도 적힌 블록과 매크로 호출이다. 값 없이 이름만 있는 블록(하위 블록을 담는 그릇, 인자 없는 플래그)은 뺐다.
- `(include ...)`, `(define ...)`의 이름, `(mod ...)` 줄 자체는 뺐다. 그 안에 든 블록은 들어 있다.
- 모델 파일(`.mdl`, `.mtl`)은 뺐다.
- 속성은 (파일 종류, 경로)로 묶은 것이다. 같은 경로가 여러 파일에 있으면 한 행이다. 유닛 하나의 실제 값은 `data/units-*.jsonl.gz`에 있다.

## 한계

- **값을 바꿨을 때 게임이 어떻게 반응하는지는 확인하지 않았다.** 이 문서는 값이 어디에 있는지만 알려 준다.
- **"추정"과 "문서"는 틀릴 수 있다.** 추정은 이름과 값에서 추론한 것이고, 문서는 대부분 다른 게임의 자료다. 값을 고치기 전에 "확인"과 "정의"를 먼저 믿고, 나머지는 게임에서 시험해 본다.
- **`##`로 시작하는 이름**(예: `##bone`)은 편집기가 꺼 둔 블록이다. 개발사 문서(Men of War II, https://docs.bestway.com.ua/foundational-knowledge/basic-knowledge-about-configuration-files.md)에 따르면 읽을 때 지우지 않지만 게임은 그 안의 값을 쓰지 않는다. 스탯 편집기도 이런 값은 고치지 못하게 한다.
- **편제·비용·게임 규칙은 여기에 없다.** `set/multiplayer`, `set/dynamic_campaign` 등 유닛 파일이 아닌 설정은 [../04-units/schema-shared.md](../04-units/schema-shared.md)에 있다.
- 기준은 바닐라 1.0.6.7이다. 모드가 추가한 속성은 [../05-workshop-mods/README.md](../05-workshop-mods/README.md)에 있다.
'@)
[IO.File]::WriteAllText((Join-Path $OutDir 'README.md'), $sb.ToString(), $utf8)
Write-Host "README.md sections=$($summary.Count)"
