param(
    [Parameter(Mandatory)][string]$DataDir,
    [Parameter(Mandatory)][string]$AnnotationDir,
    [Parameter(Mandatory)][string]$OutDir
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
[IO.Directory]::CreateDirectory($OutDir) | Out-Null
$utf8 = [Text.UTF8Encoding]::new($false)
$groups = 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared'
$resolved = @(Import-Csv -LiteralPath (Join-Path $DataDir 'keys-resolved.csv'))

function Cell([string]$s) { ($s -replace '\r?\n', ' ' -replace '\|', '\|').Trim() }
function PathCell([string]$p) { '`' + ($p -replace '`', "'" -replace '\|', '\|') + '`' }

function Get-Annotation([hashtable]$Ann, [string]$Kind, [string]$Path, [string]$Form, [string]$Comment) {
    $last = $Path.Substring($Path.LastIndexOf('/') + 1)
    foreach ($k in "$Kind`t$Path`t$Form", "*`t$Path`t$Form", "$Kind`t*/$last`t$Form", "*`t*/$last`t$Form") {
        if ($Ann.ContainsKey($k)) { return $Ann[$k] }
    }
    # 같은 이름의 키 옆에 달린 원문 주석을 이름만으로 붙인 것이다. 이 경로에도 그 뜻인지는 사람이 확인하지 않았으므로 추정이다
    if ($Comment) { return [pscustomobject]@{ Grade = '추정(파일 주석)'; Meaning = $Comment; Source = '게임 파일 내 주석(같은 이름의 키)' } }
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

$header = '| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |'
$sep = '|---|---|---|---|---|---|---|---|---|'

foreach ($g in $groups) {
    $keys = @(Import-Csv -LiteralPath (Join-Path $DataDir "keys-$g.csv"))
    $ann = @{}
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
    [void]$sb.AppendLine('이 파일은 `tools/New-SchemaDoc.ps1`이 생성한다. 손으로 고치지 않는다. 의미와 등급은 `data/annotations/' + $g + '.csv`에서 고친다.')
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
            $a = Get-Annotation $ann $r.FileKind $r.Path $r.Form $r.Comments
            [void]$sb.AppendLine("| $(PathCell $r.Path) | $($r.Form) | $(Cell $r.ArgShapes) | $($r.FileCount) | $($r.Count) | $(Cell (Range $r)) | $(Cell $a.Grade) | $(Cell $a.Meaning) | $(Cell $a.Source) |")
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
            $a = Get-Annotation $ann '*' $r.Path $r.Form $cm
            [void]$sb.AppendLine("| $(PathCell $r.Path) | $($r.Form) | $(Cell $r.ArgShapes) | $($r.FileCount) | $($r.Count) | $(Cell (Range $r)) | $(Cell $a.Grade) | $(Cell $a.Meaning) | $(Cell $a.Source) |")
        }
        [void]$sb.AppendLine()
    }
    [IO.File]::WriteAllText((Join-Path $OutDir "schema-$g.md"), $sb.ToString(), $utf8)
    Write-Host "schema-$g.md raw=$($keys.Count) resolved=$($res.Count)"
}
