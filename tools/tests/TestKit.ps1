Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$script:TestPass = 0
$script:TestFail = 0

function Assert-Equal {
    param($Expected, $Actual, [Parameter(Mandatory)][string]$Name)
    $e = ConvertTo-Json -InputObject $Expected -Depth 8 -Compress
    $a = ConvertTo-Json -InputObject $Actual -Depth 8 -Compress
    if ($e -ceq $a) { $script:TestPass++ }
    else {
        $script:TestFail++
        Write-Host "FAIL $Name" -ForegroundColor Red
        Write-Host "  expected: $e"
        Write-Host "  actual:   $a"
    }
}

function New-TestDir {
    $d = Join-Path ([IO.Path]::GetTempPath()) ("ctatoybox-test-" + [guid]::NewGuid().ToString('N'))
    [IO.Directory]::CreateDirectory($d) | Out-Null
    $d
}

function Complete-Tests {
    Write-Host "passed=$script:TestPass failed=$script:TestFail"
    if ($script:TestFail -gt 0) { exit 1 }
}
