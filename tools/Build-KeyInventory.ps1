param(
    [Parameter(Mandatory)][string]$WorkDir,
    [Parameter(Mandatory)][string]$DataDir
)
. "$PSScriptRoot/lib/Common.ps1"
. "$PSScriptRoot/lib/SdlTokenizer.ps1"
. "$PSScriptRoot/lib/SdlParser.ps1"
. "$PSScriptRoot/lib/SdlKeys.ps1"

$index = @(Import-Csv -LiteralPath (Join-Path $DataDir 'file-index.csv') | Where-Object Status -eq 'text')
$agg = @{}
$comments = @{}
$results = [System.Collections.Generic.List[object]]::new()
$n = 0
foreach ($f in $index) {
    $n++
    if ($n % 2000 -eq 0) { Write-Host "parsed $n / $($index.Count)" }
    $text = Read-SdlText (Join-Path $WorkDir ($f.VirtualPath -replace '/', '\'))
    $rt = Test-SdlRoundTrip -Text $text
    $results.Add([pscustomobject]@{ VirtualPath = $f.VirtualPath; Ok = $rt.Ok; TokenCount = $rt.TokenCount; Errors = ($rt.Errors -join ' | ') })
    $scope = "$($f.Group)|$(Get-SdlFileKind $f.VirtualPath)"
    foreach ($r in (Get-SdlKeyRows -Root $rt.Parsed.Root)) { Add-SdlKeyAggregate -Agg $agg -Scope $scope -Row $r -File $f.VirtualPath }
    foreach ($c in (Get-SdlKeyComments -Text $text)) {
        $ck = "$($f.Group)`t$($c.Name)"
        if (-not $comments.ContainsKey($ck)) { $comments[$ck] = @{} }
        $comments[$ck][$c.Comment] = 1 + [int]$comments[$ck][$c.Comment]
    }
}

$results | Export-Csv -LiteralPath (Join-Path $DataDir 'parse-results.csv') -NoTypeInformation -Encoding utf8

$rows = foreach ($a in (Export-SdlKeyAggregate -Agg $agg)) {
    $group, $kind = $a.Scope.Split('|', 2)
    $last = $a.Path.Substring($a.Path.LastIndexOf('/') + 1)
    $ck = "$group`t$last"
    $cm = if ($a.Form -eq 'block' -and $comments.ContainsKey($ck)) {
        ($comments[$ck].GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 2 | ForEach-Object { $_.Key }) -join ' ¦ '
    } else { '' }
    [pscustomobject]@{
        Group = $group; FileKind = $kind; Path = $a.Path; Form = $a.Form; Count = $a.Count; FileCount = $a.FileCount
        ArgShapes = $a.ArgShapes; NumMin = $a.NumMin; NumMax = $a.NumMax; Values = $a.Values
        ExampleFile = $a.ExampleFile; ExampleArgs = $a.ExampleArgs; Comments = $cm
    }
}
foreach ($g in 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared') {
    @($rows | Where-Object Group -eq $g | Sort-Object FileKind, Path, Form) |
        Export-Csv -LiteralPath (Join-Path $DataDir "keys-$g.csv") -NoTypeInformation -Encoding utf8
}
$failed = @($results | Where-Object { -not $_.Ok }).Count
Write-Host "files=$($index.Count) roundtrip-failed=$failed distinct-keys=$(@($rows).Count)"
