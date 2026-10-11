param(
    [Parameter(Mandatory)][string]$VanillaDataDir,
    [Parameter(Mandatory)][string]$ModDataDir
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$groups = 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared'
$utf8 = [Text.UTF8Encoding]::new($false)

# --- 파일
$van = @{}
foreach ($f in (Import-Csv -LiteralPath (Join-Path $VanillaDataDir 'file-index.csv'))) {
    if ($f.Status -in 'duplicate', 'collision') { continue }
    $van[$f.VirtualPath] = $f
}
$fileRows = foreach ($f in (Import-Csv -LiteralPath (Join-Path $ModDataDir 'file-index.csv'))) {
    $v = $van[$f.VirtualPath]
    # 모드 안에서 같은 가상 경로가 두 번 나온 경우(pak과 낱개 파일)의 둘째 행은 파일 수에 넣지 않는다
    $rel = if ($f.Status -eq 'collision') { 'in-mod-collision' }
        elseif ($f.Status -eq 'duplicate') { 'in-mod-duplicate' }
        elseif ($null -eq $v) { 'new' }
        elseif ($f.Sha1 -and $v.Sha1) { if ($f.Sha1 -eq $v.Sha1) { 'override-identical' } else { 'override-changed' } }
        else { 'override-unhashed' }   # 바이너리나 범위 밖 파일: 내용 해시가 없어 같은지 다른지 판정하지 않는다
    [pscustomobject]@{ VirtualPath = $f.VirtualPath; Status = $f.Status; Relation = $rel; Length = $f.Length; VanillaLength = $(if ($null -ne $v) { $v.Length } else { '' }) }
}
$fileRows | Export-Csv -LiteralPath (Join-Path $ModDataDir 'diff-files.csv') -NoTypeInformation -Encoding utf8

# --- 키
$vanKeys = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($g in $groups) {
    foreach ($k in (Import-Csv -LiteralPath (Join-Path $VanillaDataDir "keys-$g.csv"))) { [void]$vanKeys.Add("$($k.Group)`t$($k.FileKind)`t$($k.Path)`t$($k.Form)") }
}
$newKeys = foreach ($g in $groups) {
    foreach ($k in (Import-Csv -LiteralPath (Join-Path $ModDataDir "keys-$g.csv"))) {
        if (-not $vanKeys.Contains("$($k.Group)`t$($k.FileKind)`t$($k.Path)`t$($k.Form)")) { $k }
    }
}
@($newKeys) | Export-Csv -LiteralPath (Join-Path $ModDataDir 'diff-keys.csv') -NoTypeInformation -Encoding utf8

# --- 유닛
$relation = @{}
foreach ($r in $fileRows) { if ($r.Status -eq 'text' -and $r.Relation -in 'new', 'override-changed') { $relation[$r.VirtualPath] = $r.Relation } }

# 카탈로그(gzip JSON Lines)에서 $only에 든 Id의 줄만 파싱한다. 모델 파일의 줄은 매우 길어서 전부 파싱하면 느리다
$idRegex = [regex]::new('^\{"Id":"([^"]*)"')
function Read-Units([string]$dir, [System.Collections.Generic.HashSet[string]]$only) {
    $map = @{}
    foreach ($g in $groups) {
        $p = Join-Path $dir "units-$g.jsonl.gz"
        if (-not (Test-Path -LiteralPath $p)) { continue }
        $sr = [IO.StreamReader]::new([IO.Compression.GZipStream]::new([IO.File]::OpenRead($p), [IO.Compression.CompressionMode]::Decompress), $utf8)
        try {
            while ($null -ne ($line = $sr.ReadLine())) {
                $m = $idRegex.Match($line)
                if (-not $m.Success -or -not $only.Contains($m.Groups[1].Value)) { continue }
                $u = ConvertFrom-Json $line
                $map[$u.Id] = $u
            }
        } finally { $sr.Dispose() }
    }
    $map
}
$relationIds = [System.Collections.Generic.HashSet[string]]::new([string[]]@($relation.Keys))
$modUnits = Read-Units $ModDataDir $relationIds
$changedIds = [System.Collections.Generic.HashSet[string]]::new([string[]]@($relation.Keys | Where-Object { $relation[$_] -eq 'override-changed' }))
$vanUnits = Read-Units $VanillaDataDir $changedIds
function Count-Rows($rows) {
    $c = @{}
    foreach ($r in $rows) { $k = "$($r[0])`t$($r[1])`t$($r[2])"; $c[$k] = 1 + [int]$c[$k] }
    $c
}
$unitRows = foreach ($id in ($modUnits.Keys | Sort-Object)) {
    if (-not $relation.ContainsKey($id)) { continue }
    $u = $modUnits[$id]
    if ($relation[$id] -eq 'new') {
        [pscustomobject]@{ Id = $id; Kind = $u.Kind; Relation = 'new'; RowsAdded = @($u.Raw).Count; RowsRemoved = 0 }
        continue
    }
    $a = Count-Rows $u.Raw
    $b = if ($vanUnits.ContainsKey($id)) { Count-Rows $vanUnits[$id].Raw } else { @{} }
    $added = 0; $removed = 0
    foreach ($k in $a.Keys) { $d = $a[$k] - [int]$b[$k]; if ($d -gt 0) { $added += $d } }
    foreach ($k in $b.Keys) { $d = $b[$k] - [int]$a[$k]; if ($d -gt 0) { $removed += $d } }
    [pscustomobject]@{ Id = $id; Kind = $u.Kind; Relation = 'changed'; RowsAdded = $added; RowsRemoved = $removed }
}
@($unitRows) | Export-Csv -LiteralPath (Join-Path $ModDataDir 'diff-units.csv') -NoTypeInformation -Encoding utf8

$fileRows | Group-Object Relation | ForEach-Object { Write-Host ("files {0,-20} {1}" -f $_.Name, $_.Count) }
Write-Host "new-keys=$(@($newKeys).Count)"
@($unitRows) | Group-Object Kind, Relation | ForEach-Object { Write-Host ("units {0,-28} {1}" -f $_.Name, $_.Count) }
