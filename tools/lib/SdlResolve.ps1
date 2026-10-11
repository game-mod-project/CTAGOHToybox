Set-StrictMode -Version 3.0

# 리졸버 본체는 SdlResolve.cs(C#)에 있다. 규칙과 한계는 그 파일 머리말에 적혀 있다.

function New-SdlResolveContext {
    param([Parameter(Mandatory)][string[]]$WorkDirs)
    New-Object SdlResolver -ArgumentList (, $WorkDirs)
}

function Get-SdlParsedFile {
    param($Ctx, [string]$VirtualPath, [bool]$Cache = $true)
    $Ctx.GetParsed($VirtualPath, $Cache)
}

function Resolve-SdlIncludePath {
    param([string]$Raw, [string]$FromDir)
    [SdlResolver]::IncludePath($Raw, $FromDir)
}

function Resolve-SdlFile {
    param($Ctx, [Parameter(Mandatory)][string]$VirtualPath)
    $Ctx.Resolve($VirtualPath)
}
