Set-StrictMode -Version 3.0

# 토크나이저·파서 본체는 editor/CtaToybox.Core(C#)에 있다. .mdl 파일이 커서 PowerShell 객체로는 너무 느리다.
# 소스가 DLL보다 새로우면 다시 빌드한다. 파일을 잠그지 않도록 바이트로 읽어 올린다(잠그면 다음 빌드가 실패한다).
if (-not ('SdlCore' -as [type])) {
    $coreDir = Join-Path $PSScriptRoot '../../editor/CtaToybox.Core'
    $coreDll = Join-Path $coreDir 'bin/Release/net8.0/CtaToybox.Core.dll'
    $newestSource = Get-ChildItem -LiteralPath $coreDir -Recurse -File -Include '*.cs', '*.csproj' |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
    if (-not (Test-Path -LiteralPath $coreDll) -or (Get-Item -LiteralPath $coreDll).LastWriteTimeUtc -lt $newestSource.LastWriteTimeUtc) {
        $log = & dotnet build $coreDir -c Release --nologo -v q 2>&1
        if ($LASTEXITCODE -ne 0) { throw "CtaToybox.Core 빌드 실패:`n$($log -join "`n")" }
    }
    [void][Reflection.Assembly]::Load([IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $coreDll).Path))
}

function Get-SdlTokens {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Text)
    , [SdlCore]::Tokenize($Text)
}
