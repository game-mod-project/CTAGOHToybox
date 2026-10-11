Set-StrictMode -Version 3.0

function New-SdlNode {
    param([string]$Kind, [string]$Text, [int]$Index)
    [SdlNode]::new($Kind, $Text, $Index, $true)
}

function New-SdlLeaf {
    param([string]$Kind, [string]$Text, [int]$Index)
    [SdlNode]::new($Kind, $Text, $Index, $false)
}

function ConvertFrom-Sdl {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Text)
    [SdlCore]::Parse($Text)
}

function Get-SdlFlatText {
    param($Node, [System.Collections.Generic.List[string]]$Out)
    [SdlCore]::Flat($Node, $Out)
}

function Get-SdlNodesText {
    param($Nodes)
    [SdlCore]::NodesText([object[]]@($Nodes))
}

function Test-SdlRoundTrip {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Text)
    $p = [SdlCore]::Parse($Text)
    [pscustomobject]@{ Ok = [SdlCore]::RoundTrip($p); Errors = $p.Errors; TokenCount = $p.Tokens.Count; Parsed = $p }
}
