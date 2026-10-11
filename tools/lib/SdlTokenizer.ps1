Set-StrictMode -Version 3.0

# 토크나이저·파서 본체는 Sdl.cs(C#)에 있다. .mdl 파일이 커서 PowerShell 객체로는 너무 느리다.
if (-not ('SdlCore' -as [type])) {
    Add-Type -Path @(Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.cs' | Sort-Object Name | ForEach-Object FullName)
}

function Get-SdlTokens {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Text)
    , [SdlCore]::Tokenize($Text)
}
