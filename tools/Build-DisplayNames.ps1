param(
    [Parameter(Mandatory)][string]$LocPak,
    [Parameter(Mandatory)][string]$DataDir
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Unquote([string]$s) {
    $m = [regex]::Match($s, '"(.*)"\s*$')
    if (-not $m.Success) { return '' }
    $m.Groups[1].Value -replace '\\n', ' ' -replace '\\"', '"'
}

$rows = [System.Collections.Generic.List[object]]::new()
$zip = [IO.Compression.ZipFile]::OpenRead($LocPak)
try {
    foreach ($e in ($zip.Entries | Where-Object { $_.FullName -like '*.pot' })) {
        $sr = [IO.StreamReader]::new($e.Open(), [Text.Encoding]::UTF8)
        try { $lines = $sr.ReadToEnd() -split "`r?`n" } finally { $sr.Dispose() }
        $ctxt = $null; $id = $null; $mode = ''
        foreach ($l in ($lines + '')) {
            if ($l.StartsWith('msgctxt ')) { $ctxt = Unquote $l.Substring(8); $id = $null; $mode = 'ctxt' }
            elseif ($l.StartsWith('msgid ')) { $id = Unquote $l.Substring(6); $mode = 'id' }
            elseif ($l.StartsWith('msgstr ')) { $mode = 'str' }
            elseif ($l.StartsWith('"')) {
                if ($mode -eq 'id') { $id += Unquote $l } elseif ($mode -eq 'ctxt') { $ctxt += Unquote $l }
            }
            elseif ($l.Trim() -eq '') {
                if ($ctxt) { $rows.Add([pscustomobject]@{ Context = $ctxt; Text = $id; File = $e.FullName }) }
                $ctxt = $null; $id = $null; $mode = ''
            }
        }
    }
}
finally { $zip.Dispose() }
$rows | Export-Csv -LiteralPath (Join-Path $DataDir 'display-names.csv') -NoTypeInformation -Encoding utf8
Write-Host "contexts=$($rows.Count)"
$rows | Group-Object { (($_.Context -split '/') | Select-Object -First 2) -join '/' } | Sort-Object Count -Descending | Select-Object -First 12 | ForEach-Object { Write-Host ("{0,-28} {1}" -f $_.Name, $_.Count) }
