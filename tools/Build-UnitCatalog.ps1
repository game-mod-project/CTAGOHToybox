param(
    [Parameter(Mandatory)][string]$WorkDir,
    [Parameter(Mandatory)][string]$DataDir,
    # include가 가리키는 파일을 추가로 찾을 폴더 (모드 분석 때 바닐라 작업 폴더)
    [string[]]$FallbackWorkDir = @(),
    # 이름 색인에 추가로 넣을 데이터 폴더 (모드 분석 때 바닐라 data)
    [string[]]$NameDataDir = @()
)
. "$PSScriptRoot/lib/Common.ps1"
. "$PSScriptRoot/lib/SdlTokenizer.ps1"
. "$PSScriptRoot/lib/SdlParser.ps1"
. "$PSScriptRoot/lib/SdlKeys.ps1"
. "$PSScriptRoot/lib/SdlResolve.ps1"
. "$PSScriptRoot/lib/SdlRefs.ps1"

$groups = 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared'
$resolvedKinds = 'breed', 'humanskin', 'vehicle', 'item-entity', 'stuff', 'pattern'
$nsOf = @{ breed = 'breed'; vehicle = 'entity'; humanskin = 'entity'; 'item-entity' = 'entity'; stuff = 'stuff'; pattern = 'pattern' }
$noRefKinds = '.mdl', '.mtl'   # 모델·재질 파일의 문자열은 뼈대·텍스처 이름이라 참조 색인에서 뺀다
$utf8 = [Text.UTF8Encoding]::new($false)

function New-GzWriter([string]$path) {
    [IO.StreamWriter]::new([IO.Compression.GZipStream]::new([IO.File]::Create($path), [IO.Compression.CompressionLevel]::Optimal), $utf8)
}
function J([string]$s) { [SdlCore]::JsonString($s) }
function JArray($items) { '[' + (@(foreach ($i in $items) { [SdlCore]::JsonString($i) }) -join ',') + ']' }

# 왕복 검사에 실패한 파일도 싣는다. 파서는 읽을 수 있는 데까지 트리를 만들므로 빼면 그 유닛이 통째로 사라진다.
# 대신 그 줄에 ParseErrors를 붙인다
$parseErrors = @{}
Import-Csv -LiteralPath (Join-Path $DataDir 'parse-results.csv') | Where-Object Ok -ne 'True' | ForEach-Object { $parseErrors[$_.VirtualPath] = @($_.Errors -split ' \| ') }
$index = @(Import-Csv -LiteralPath (Join-Path $DataDir 'file-index.csv') | Where-Object Status -eq 'text')

# 이름 색인: 파일명이 곧 아이템·엔티티·breed의 이름이다
$refs = New-SdlReferenceIndex
foreach ($d in (@($DataDir) + $NameDataDir)) {
    foreach ($f in (Import-Csv -LiteralPath (Join-Path $d 'file-index.csv') | Where-Object Status -eq 'text')) {
        $kind = Get-SdlUnitKind $f.VirtualPath
        if (-not $nsOf.ContainsKey($kind)) { continue }
        $leaf = Get-SdlLeafName $f.VirtualPath
        $refs.AddName($leaf, $nsOf[$kind])
        if ($kind -eq 'stuff' -and $leaf.Contains('.')) { $refs.AddName($leaf.Substring(0, $leaf.IndexOf('.')), 'stuff') }
    }
}
Write-Host "names=$($refs.NameCount)"

$rawWriters = @{}; $resWriters = @{}
foreach ($g in $groups) {
    $rawWriters[$g] = New-GzWriter (Join-Path $DataDir "units-$g.jsonl.gz")
    $resWriters[$g] = New-GzWriter (Join-Path $DataDir "units-$g.resolved.jsonl.gz")
}
$refWriter = New-GzWriter (Join-Path $DataDir 'references.csv.gz')
$refWriter.WriteLine('From,Path,Namespace,Target,MatchType')

$ctx = New-SdlResolveContext -WorkDirs (@($WorkDir) + $FallbackWorkDir)
$agg = New-SdlKeyAggregate
$summary = [System.Collections.Generic.List[object]]::new()
$n = 0
try {
    foreach ($f in $index) {
        $n++
        if ($n % 4000 -eq 0) { Write-Host "catalog $n / $($index.Count)" }
        $vp = $f.VirtualPath
        $kind = Get-SdlUnitKind $vp
        $name = Get-SdlLeafName $vp
        $root = Get-SdlParsedFile $ctx $vp $false
        $raw = Get-SdlKeyRows -Root $root
        $errJson = if ($parseErrors.ContainsKey($vp)) { ',"ParseErrors":' + (JArray $parseErrors[$vp]) } else { '' }
        $rawWriters[$f.Group].WriteLine('{"Id":' + (J $vp) + ',"Name":' + (J $name) + ',"Kind":' + (J $kind) + ',"Raw":' + (ConvertTo-SdlRowsJson -Rows $raw) + $errJson + '}')

        $refRows = $raw
        if ($kind -in $resolvedKinds) {
            $res = Resolve-SdlFile $ctx $vp
            $rows = Get-SdlKeyRows -Root ([pscustomobject]@{ Children = $res.Nodes })
            $resWriters[$f.Group].WriteLine('{"Id":' + (J $vp) + ',"Kind":' + (J $kind) + ',"Resolved":' + (ConvertTo-SdlRowsJson -Rows $rows) +
                ',"Unresolved":' + (JArray $res.Unresolved) + ',"Includes":' + (JArray $res.Includes) + ',"Macros":' + (JArray $res.Macros) + ',"Presets":' + (JArray $res.Presets) + $errJson + '}')
            Add-SdlKeyAggregateRows -Agg $agg -Scope "$($f.Group)|$kind" -Rows $rows -File $vp
            $summary.Add([pscustomobject]@{
                VirtualPath = $vp; Kind = $kind; RawRows = $raw.Count; ResolvedRows = $rows.Count
                UnresolvedCount = $res.Unresolved.Count; Unresolved = ($res.Unresolved -join ' ; ')
            })
            $refRows = $rows
        }
        if ((Get-SdlFileKind $vp) -notin $noRefKinds) { [void]$refs.Scan($vp, $name, $refRows, $refWriter) }
    }
}
finally {
    foreach ($w in $rawWriters.Values) { $w.Dispose() }
    foreach ($w in $resWriters.Values) { $w.Dispose() }
    $refWriter.Dispose()
}

$resolvedKeys = foreach ($a in (Export-SdlKeyAggregate -Agg $agg)) {
    $group, $kind = $a.Scope.Split('|', 2)
    [pscustomobject]@{
        Group = $group; Kind = $kind; Path = $a.Path; Form = $a.Form; Count = $a.Count; FileCount = $a.FileCount
        ArgShapes = $a.ArgShapes; NumMin = $a.NumMin; NumMax = $a.NumMax; Values = $a.Values
        ExampleFile = $a.ExampleFile; ExampleArgs = $a.ExampleArgs
    }
}
@($resolvedKeys | Sort-Object Group, Kind, Path, Form) | Export-Csv -LiteralPath (Join-Path $DataDir 'keys-resolved.csv') -NoTypeInformation -Encoding utf8
$summary | Export-Csv -LiteralPath (Join-Path $DataDir 'resolve-summary.csv') -NoTypeInformation -Encoding utf8
$withUnresolved = @($summary | Where-Object { [int]$_.UnresolvedCount -gt 0 }).Count
Write-Host "cataloged=$($index.Count) resolved=$($summary.Count) files-with-unresolved=$withUnresolved resolved-keys=$(@($resolvedKeys).Count) references=$($refs.ReferenceCount)"
