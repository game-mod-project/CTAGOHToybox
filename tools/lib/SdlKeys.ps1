Set-StrictMode -Version 3.0

# 키 행 추출 본체는 SdlKeys.cs(C#)에 있다.
$script:SdlQualifiedBlocks = @('extender')
$script:SdlCommentRegex = [regex]::new('^[ \t]*\{[ \t]*([^\s{}()";]+)[^;\r\n]*;[;/ \t]*([^\r\n]*\S)', [Text.RegularExpressions.RegexOptions]::Multiline)

function Test-SdlExpr {
    param($Node)
    [SdlCore]::IsExpr($Node)
}

# Root는 .Children을 가진 객체면 된다(파서의 Root, 또는 펼친 노드 목록을 담은 객체)
function Get-SdlKeyRows {
    param([Parameter(Mandatory)]$Root)
    , [SdlCore]::KeyRows([object[]]@($Root.Children), $script:SdlQualifiedBlocks)
}

function ConvertTo-SdlRowsJson {
    param([Parameter(Mandatory)][AllowEmptyCollection()]$Rows)
    [SdlCore]::RowsJson($Rows)
}

function ConvertTo-SdlJsonString {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Text)
    [SdlCore]::JsonString($Text)
}

# 집계 본체는 SdlResolve.cs의 SdlKeyAggregator(C#)다
function New-SdlKeyAggregate {
    [SdlKeyAggregator]::new()
}

function Add-SdlKeyAggregate {
    param([Parameter(Mandatory)]$Agg, [Parameter(Mandatory)][string]$Scope, [Parameter(Mandatory)]$Row, [Parameter(Mandatory)][string]$File)
    $Agg.Add($Scope, $Row, $File)
}

function Add-SdlKeyAggregateRows {
    param([Parameter(Mandatory)]$Agg, [Parameter(Mandatory)][string]$Scope, [Parameter(Mandatory)][AllowEmptyCollection()]$Rows, [Parameter(Mandatory)][string]$File)
    $Agg.AddRows($Scope, $Rows, $File)
}

function Export-SdlKeyAggregate {
    param([Parameter(Mandatory)]$Agg)
    $Agg.Export()
}

function Get-SdlKeyComments {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Text)
    foreach ($m in $script:SdlCommentRegex.Matches($Text)) {
        [pscustomobject]@{ Name = $m.Groups[1].Value.ToLowerInvariant(); Comment = $m.Groups[2].Value.Trim() }
    }
}
