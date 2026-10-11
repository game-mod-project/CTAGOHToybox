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

function Add-SdlKeyAggregate {
    param([Parameter(Mandatory)][hashtable]$Agg, [Parameter(Mandatory)][string]$Scope, [Parameter(Mandatory)]$Row, [Parameter(Mandatory)][string]$File)
    $k = "$Scope`t$($Row.Path)`t$($Row.Form)"
    $a = $Agg[$k]
    if ($null -eq $a) {
        $a = [pscustomobject]@{
            Scope = $Scope; Path = $Row.Path; Form = $Row.Form; Count = 0; FileCount = 0; LastFile = ''
            Shapes = @{}; Values = @{}; NumMin = $null; NumMax = $null; ExampleFile = $File; ExampleArgs = $Row.Args
        }
        $Agg[$k] = $a
    }
    $a.Count++
    if ($a.LastFile -ne $File) { $a.FileCount++; $a.LastFile = $File }
    $a.Shapes[$Row.Shape] = 1 + [int]$a.Shapes[$Row.Shape]
    if ($a.Values.Count -le 12) { $a.Values[$Row.Args] = 1 + [int]$a.Values[$Row.Args] }
    if ($Row.Shape -eq 'num') {
        $d = [double]::Parse($Row.Args, [Globalization.NumberStyles]::Float, [cultureinfo]::InvariantCulture)
        if ($null -eq $a.NumMin -or $d -lt $a.NumMin) { $a.NumMin = $d }
        if ($null -eq $a.NumMax -or $d -gt $a.NumMax) { $a.NumMax = $d }
    }
}

function Export-SdlKeyAggregate {
    param([Parameter(Mandatory)][hashtable]$Agg)
    foreach ($a in $Agg.Values) {
        $shapes = ($a.Shapes.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 5 | ForEach-Object { "[$($_.Key)]x$($_.Value)" }) -join ' '
        $values = if ($a.Values.Count -gt 12) { '(13종 이상)' } else { ($a.Values.GetEnumerator() | Sort-Object Value -Descending | ForEach-Object { $_.Key }) -join ' ¦ ' }
        [pscustomobject]@{
            Scope = $a.Scope; Path = $a.Path; Form = $a.Form; Count = $a.Count; FileCount = $a.FileCount
            ArgShapes = $shapes; NumMin = $a.NumMin; NumMax = $a.NumMax; Values = $values
            ExampleFile = $a.ExampleFile; ExampleArgs = $a.ExampleArgs
        }
    }
}

function Get-SdlKeyComments {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$Text)
    foreach ($m in $script:SdlCommentRegex.Matches($Text)) {
        [pscustomobject]@{ Name = $m.Groups[1].Value.ToLowerInvariant(); Comment = $m.Groups[2].Value.Trim() }
    }
}
