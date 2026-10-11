param(
    [Parameter(Mandatory)][string]$DataDir,
    [Parameter(Mandatory)][string]$AnnotationDir,
    [Parameter(Mandatory)][string]$OutDir
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$DataDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($DataDir)
$AnnotationDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($AnnotationDir)
$OutDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutDir)
[IO.Directory]::CreateDirectory($OutDir) | Out-Null
$utf8 = [Text.UTF8Encoding]::new($false)
$groups = 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared'
$resolved = @(Import-Csv -LiteralPath (Join-Path $DataDir 'keys-resolved.csv'))

function Cell([string]$s) { ($s -replace '\r?\n', ' ' -replace '\|', '\|').Trim() }
function PathCell([string]$p) { '`' + ($p -replace '`', "'" -replace '\|', '\|') + '`' }

# 주석 조회. 정확한 경로가 우선하고, $AllowWildcard일 때만 */이름 주석을 쓴다.
# 와일드카드는 유닛 파일과 펼친 키에만 허용한다: 공용 설정이나 모델 파일에는 같은 이름이 다른 뜻으로 쓰인다.
function Get-Annotation([hashtable]$Ann, [string]$Kind, [string]$Path, [string]$Form, [bool]$AllowWildcard) {
    $last = $Path.Substring($Path.LastIndexOf('/') + 1)
    $keys = @("$Kind`t$Path`t$Form", "*`t$Path`t$Form")
    if ($AllowWildcard) { $keys += @("$Kind`t*/$last`t$Form", "*`t*/$last`t$Form") }
    foreach ($k in $keys) { if ($Ann.ContainsKey($k)) { return $Ann[$k] } }
    [pscustomobject]@{ Grade = '미확인(자동)'; Meaning = ''; Source = '' }
}
function Range($r) {
    if ($r.NumMin -ne '' -and $null -ne $r.NumMin) { if ($r.NumMin -eq $r.NumMax) { return "$($r.NumMin)" } else { return "$($r.NumMin) ~ $($r.NumMax)" } }
    if ($r.Values -and $r.Values -ne '(13종 이상)') { return $r.Values }
    "예: $($r.ExampleArgs)"
}

# 모든 그룹의 원문 주석: 펼친 키는 다른 그룹(properties 등)에서 온 경우가 많다
$globalComments = @{}
foreach ($gg in $groups) {
    foreach ($k in (Import-Csv -LiteralPath (Join-Path $DataDir "keys-$gg.csv"))) {
        if ($k.Comments) { $nm = $k.Path.Substring($k.Path.LastIndexOf('/') + 1); if (-not $globalComments.ContainsKey($nm)) { $globalComments[$nm] = $k.Comments } }
    }
}

$unitFileKinds = '.def', '.set', '(no-ext)', '.ammo', '.weapon', '.grenade', '.mine', '.pattern'
$sourceNames = @{ C = '게임 파일 주석'; S = '파일 구조와 값에서 추론' }
$header = '| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |'
$sep = '|---|---|---|---|---|---|---|---|---|---|'

foreach ($g in $groups) {
    $keys = @(Import-Csv -LiteralPath (Join-Path $DataDir "keys-$g.csv"))
    $ann = @{}
    # 모든 그룹에 공통인 주석 (경로|형태|등급|의미|출처)
    $commonPath = Join-Path $AnnotationDir 'common.psv'
    if (Test-Path -LiteralPath $commonPath) {
        foreach ($line in [IO.File]::ReadLines($commonPath)) {
            if (-not $line.Trim() -or $line.StartsWith('#')) { continue }
            $p = $line.Split('|', 5)
            if ($p.Count -lt 5) { throw "주석 줄 형식 오류: $line" }
            $src = if ($sourceNames.ContainsKey($p[4])) { $sourceNames[$p[4]] } else { $p[4] }
            $ann["*`t$($p[0])`t$($p[1])"] = [pscustomobject]@{ Grade = $p[2]; Meaning = $p[3]; Source = $src }
        }
    }
    $annPath = Join-Path $AnnotationDir "$g.csv"
    if (Test-Path -LiteralPath $annPath) {
        foreach ($a in (Import-Csv -LiteralPath $annPath)) { $ann["$($a.FileKind)`t$($a.Path)`t$($a.Form)"] = $a }
    }
    # 펼친 키에 붙일 원문 주석: 같은 그룹에서 마지막 조각이 같은 원본 키의 주석
    $commentByName = @{}
    foreach ($k in $keys) { if ($k.Comments) { $commentByName[$k.Path.Substring($k.Path.LastIndexOf('/') + 1)] = $k.Comments } }

    $sb = [Text.StringBuilder]::new()
    [void]$sb.AppendLine("# 스키마: $g")
    [void]$sb.AppendLine()
    [void]$sb.AppendLine('이 파일은 `tools/New-SchemaDoc.ps1`이 생성한다. 손으로 고치지 않는다. 의미와 등급은 `data/annotations/common.psv`에서 고친다. 마지막 열은 같은 이름의 키 옆에 달린 게임 파일의 주석을 이름만으로 모은 참고 자료이며, 이 경로의 뜻과 다를 수 있다.')
    [void]$sb.AppendLine()
    $introPath = Join-Path $AnnotationDir "$g.intro.md"
    if (Test-Path -LiteralPath $introPath) { [void]$sb.AppendLine((Get-Content -Raw -LiteralPath $introPath).TrimEnd()); [void]$sb.AppendLine() }

    [void]$sb.AppendLine('## 원본 파일에 적힌 키')
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("파일에 쓰인 그대로의 위치다. 고유 키 $($keys.Count)개.")
    [void]$sb.AppendLine()
    foreach ($kindGroup in ($keys | Group-Object FileKind | Sort-Object Name)) {
        [void]$sb.AppendLine("### 파일 종류: $($kindGroup.Name)")
        [void]$sb.AppendLine()
        [void]$sb.AppendLine($header); [void]$sb.AppendLine($sep)
        foreach ($r in ($kindGroup.Group | Sort-Object Path, Form)) {
            $a = Get-Annotation $ann $r.FileKind $r.Path $r.Form ($g -ne 'shared' -and $unitFileKinds -contains $r.FileKind)
            [void]$sb.AppendLine("| $(PathCell $r.Path) | $($r.Form) | $(Cell $r.ArgShapes) | $($r.FileCount) | $($r.Count) | $(Cell (Range $r)) | $(Cell $a.Grade) | $(Cell $a.Meaning) | $(Cell $a.Source) | $(Cell $r.Comments) |")
        }
        [void]$sb.AppendLine()
    }

    $res = @($resolved | Where-Object Group -eq $g)
    [void]$sb.AppendLine('## 펼친 뒤의 키')
    [void]$sb.AppendLine()
    [void]$sb.AppendLine("include와 매크로를 펼친 뒤 유닛 안에서의 위치다. 펼치는 규칙은 추정이며 한계는 coverage.md에 있다. 고유 키 $($res.Count)개.")
    [void]$sb.AppendLine()
    foreach ($kindGroup in ($res | Group-Object Kind | Sort-Object Name)) {
        [void]$sb.AppendLine("### 유닛 종류: $($kindGroup.Name)")
        [void]$sb.AppendLine()
        [void]$sb.AppendLine($header); [void]$sb.AppendLine($sep)
        foreach ($r in ($kindGroup.Group | Sort-Object Path, Form)) {
            $last = $r.Path.Substring($r.Path.LastIndexOf('/') + 1)
            $cm = ''
            if ($r.Form -eq 'block') { if ($commentByName.ContainsKey($last)) { $cm = $commentByName[$last] } elseif ($globalComments.ContainsKey($last)) { $cm = $globalComments[$last] } }
            $a = Get-Annotation $ann '*' $r.Path $r.Form $true
            [void]$sb.AppendLine("| $(PathCell $r.Path) | $($r.Form) | $(Cell $r.ArgShapes) | $($r.FileCount) | $($r.Count) | $(Cell (Range $r)) | $(Cell $a.Grade) | $(Cell $a.Meaning) | $(Cell $a.Source) | $(Cell $cm) |")
        }
        [void]$sb.AppendLine()
    }
    [IO.File]::WriteAllText((Join-Path $OutDir "schema-$g.md"), $sb.ToString(), $utf8)
    Write-Host "schema-$g.md raw=$($keys.Count) resolved=$($res.Count)"
}
