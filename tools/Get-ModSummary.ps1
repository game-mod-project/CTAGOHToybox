param(
    [Parameter(Mandatory)][string]$VanillaDataDir,
    [Parameter(Mandatory)][string]$ModsDataDir,
    [Parameter(Mandatory)][string[]]$ModIds
)
# 모드 분석 결과를 문서에 옮길 수 있게 요약한다.
#   <ModsDataDir>\<id>\new-names.csv        바닐라의 어떤 키에도 없는 이름(마지막 경로 조각)을 가진 키
#   <ModsDataDir>\<id>\summary.json         개수 요약
#   <ModsDataDir>\cross-mod-conflicts.csv   둘 이상의 모드가 가진 같은 가상 경로의 텍스트 파일
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$groups = 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared'

function Last([string]$path) { $path.Substring($path.LastIndexOf('/') + 1) }

# 바닐라에 나오는 모든 (형태, 이름)
$vanNames = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($g in $groups) { foreach ($k in (Import-Csv -LiteralPath (Join-Path $VanillaDataDir "keys-$g.csv"))) { [void]$vanNames.Add("$($k.Form)`t$(Last $k.Path)") } }
foreach ($k in (Import-Csv -LiteralPath (Join-Path $VanillaDataDir 'keys-resolved.csv'))) { [void]$vanNames.Add("$($k.Form)`t$(Last $k.Path)") }

$all = [System.Collections.Generic.List[object]]::new()
foreach ($id in $ModIds) {
    $d = Join-Path $ModsDataDir $id
    $index = @(Import-Csv -LiteralPath (Join-Path $d 'file-index.csv'))
    $files = @(Import-Csv -LiteralPath (Join-Path $d 'diff-files.csv'))
    $keys = @(Import-Csv -LiteralPath (Join-Path $d 'diff-keys.csv'))
    $units = @(Import-Csv -LiteralPath (Join-Path $d 'diff-units.csv'))
    $summary = @(Import-Csv -LiteralPath (Join-Path $d 'resolve-summary.csv'))
    $parse = @(Import-Csv -LiteralPath (Join-Path $d 'parse-results.csv'))

    # 펼친 키까지 포함해 바닐라에 없는 이름
    $modKeys = [System.Collections.Generic.List[object]]::new()
    foreach ($g in $groups) { foreach ($k in (Import-Csv -LiteralPath (Join-Path $d "keys-$g.csv"))) { $modKeys.Add([pscustomobject]@{ Where = "원본 $($k.Group) $($k.FileKind)"; Path = $k.Path; Form = $k.Form; FileCount = $k.FileCount; ArgShapes = $k.ArgShapes; ExampleFile = $k.ExampleFile; ExampleArgs = $k.ExampleArgs; Comments = $k.Comments }) } }
    foreach ($k in (Import-Csv -LiteralPath (Join-Path $d 'keys-resolved.csv'))) { $modKeys.Add([pscustomobject]@{ Where = "펼침 $($k.Group) $($k.Kind)"; Path = $k.Path; Form = $k.Form; FileCount = $k.FileCount; ArgShapes = $k.ArgShapes; ExampleFile = $k.ExampleFile; ExampleArgs = $k.ExampleArgs; Comments = '' }) }
    $newNames = @($modKeys | Where-Object { -not $vanNames.Contains("$($_.Form)`t$(Last $_.Path)") })
    $newNames | Sort-Object Form, { Last $_.Path }, Where, Path | Export-Csv -LiteralPath (Join-Path $d 'new-names.csv') -NoTypeInformation -Encoding utf8

    $unresolved = @($summary | Where-Object { [int]$_.UnresolvedCount -gt 0 })
    $s = [ordered]@{
        Id = $id
        Files = $index.Count
        FilesByStatus = [ordered]@{}
        FilesByRelation = [ordered]@{}
        TextByRelationAndTop = [ordered]@{}
        UnitsByKindRelation = [ordered]@{}
        NewVehicleDefsByNation = [ordered]@{}
        NewBreedsByFolder = [ordered]@{}
        KeysNotInVanilla = $keys.Count
        KeysNotInVanillaByForm = [ordered]@{}
        NamesNotInVanilla = $newNames.Count
        NamesNotInVanillaByForm = [ordered]@{}
        DistinctNewBlockNames = @($newNames | Where-Object Form -eq 'block' | ForEach-Object { Last $_.Path } | Sort-Object -Unique).Count
        RoundTripFailed = @($parse | Where-Object Ok -ne 'True' | ForEach-Object VirtualPath)
        UnresolvedFiles = $unresolved.Count
        UnresolvedTop = [ordered]@{}
        Collisions = @($index | Where-Object Status -eq 'collision').Count
    }
    foreach ($x in ($index | Group-Object Status | Sort-Object Name)) { $s.FilesByStatus[$x.Name] = $x.Count }
    foreach ($x in ($files | Group-Object Relation | Sort-Object Name)) { $s.FilesByRelation[$x.Name] = $x.Count }
    foreach ($x in ($files | Where-Object Status -in 'text', 'reference-only' | Group-Object { "$($_.Relation) $((($_.VirtualPath -split '/') | Select-Object -First 2) -join '/')" } | Sort-Object Count -Descending | Select-Object -First 25)) { $s.TextByRelationAndTop[$x.Name] = $x.Count }
    foreach ($x in ($units | Where-Object Kind -ne 'support' | Group-Object { "$($_.Kind) $($_.Relation)" } | Sort-Object Name)) { $s.UnitsByKindRelation[$x.Name] = $x.Count }
    foreach ($x in ($units | Where-Object { $_.Kind -eq 'vehicle' -and $_.Relation -eq 'new' } | Group-Object { ($_.Id -split '/')[2] } | Sort-Object Count -Descending)) { $s.NewVehicleDefsByNation[$x.Name] = $x.Count }
    foreach ($x in ($units | Where-Object { $_.Kind -eq 'breed' -and $_.Relation -eq 'new' } | Group-Object { (($_.Id -split '/') | Select-Object -Skip 2 -First 2) -join '/' } | Sort-Object Count -Descending | Select-Object -First 20)) { $s.NewBreedsByFolder[$x.Name] = $x.Count }
    foreach ($x in ($keys | Group-Object Form | Sort-Object Count -Descending)) { $s.KeysNotInVanillaByForm[$x.Name] = $x.Count }
    foreach ($x in ($newNames | Group-Object Form | Sort-Object Count -Descending)) { $s.NamesNotInVanillaByForm[$x.Name] = $x.Count }
    foreach ($x in ($unresolved | ForEach-Object { $_.Unresolved -split ' ; ' } | Group-Object | Sort-Object Count -Descending | Select-Object -First 15)) { $s.UnresolvedTop[$x.Name] = $x.Count }
    [IO.File]::WriteAllText((Join-Path $d 'summary.json'), (ConvertTo-Json -InputObject $s -Depth 4), [Text.UTF8Encoding]::new($false))
    Write-Host "$id names-not-in-vanilla=$($newNames.Count) distinct-new-block-names=$($s.DistinctNewBlockNames)"

    foreach ($f in ($index | Where-Object Status -in 'text', 'reference-only')) { $all.Add([pscustomobject]@{ Mod = $id; VirtualPath = $f.VirtualPath; Sha1 = $f.Sha1 }) }
}

$conf = $all | Group-Object VirtualPath | Where-Object { @($_.Group.Mod | Sort-Object -Unique).Count -gt 1 } | ForEach-Object {
    [pscustomobject]@{ VirtualPath = $_.Name; Mods = (@($_.Group.Mod | Sort-Object -Unique) -join ' '); SameContent = (@($_.Group.Sha1 | Sort-Object -Unique).Count -eq 1) }
}
@($conf) | Export-Csv -LiteralPath (Join-Path $ModsDataDir 'cross-mod-conflicts.csv') -NoTypeInformation -Encoding utf8
Write-Host "cross-mod text files=$(@($conf).Count) differing=$(@($conf | Where-Object { -not $_.SameContent }).Count)"
