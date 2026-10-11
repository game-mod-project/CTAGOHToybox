. "$PSScriptRoot/TestKit.ps1"
. "$PSScriptRoot/../lib/SdlTokenizer.ps1"
. "$PSScriptRoot/../lib/SdlParser.ps1"
. "$PSScriptRoot/../lib/SdlKeys.ps1"

function Rows([string]$text) { , (Get-SdlKeyRows -Root (ConvertFrom-Sdl -Text $text).Root) }

$rows = Rows @'
{game_entity
	(include "/properties/cannon.ext" scale(1.0))
	{Extension "x.mdl"}
	{props "a" "b"}
	("wheel" args "wheelright")
	{extender "cannon" {animation {switch "close"}}}
	{spreading {radiusTable {0 0} {100 10}}}
	{speed (* 0.35 %mps)}
	(mod not "mp" {limit 50})
	{volume "s1" {able {holed}}}
}
'@
Assert-Equal @(
    '/game_entity',
    '/game_entity/(include)',
    '/game_entity/extension',
    '/game_entity/props',
    '/game_entity/("wheel")',
    '/game_entity/extender[cannon]',
    '/game_entity/extender[cannon]/animation',
    '/game_entity/extender[cannon]/animation/switch',
    '/game_entity/spreading',
    '/game_entity/spreading/radiustable',
    '/game_entity/spreading/radiustable/#row',
    '/game_entity/spreading/radiustable/#row',
    '/game_entity/speed',
    '/game_entity/(mod)',
    '/game_entity/limit',
    '/game_entity/volume',
    '/game_entity/volume/able',
    '/game_entity/volume/able/holed'
) @($rows.Path) 'paths in document order'

function Row([string]$path) { @($rows | Where-Object Path -eq $path)[0] }
Assert-Equal 'include' (Row '/game_entity/(include)').Form 'include form'
Assert-Equal 'str call:scale' (Row '/game_entity/(include)').Shape 'include shape'
Assert-Equal 'str str' (Row '/game_entity/props').Shape 'two strings'
Assert-Equal '"a" "b"' (Row '/game_entity/props').Args 'args text'
Assert-Equal 'macro' (Row '/game_entity/("wheel")').Form 'macro form'
Assert-Equal 'atom str' (Row '/game_entity/("wheel")').Shape 'macro shape'
Assert-Equal 'str' (Row '/game_entity/extender[cannon]').Shape 'qualified block keeps its argument'
Assert-Equal 'num num' (Row '/game_entity/spreading/radiustable/#row').Shape 'numeric row'
Assert-Equal '0 0' (Row '/game_entity/spreading/radiustable/#row').Args 'numeric row keeps first number'
Assert-Equal 'expr' (Row '/game_entity/speed').Shape 'expression argument'
Assert-Equal '( * 0.35 %mps )' (Row '/game_entity/speed').Args 'expression text'
Assert-Equal 'mod' (Row '/game_entity/(mod)').Form 'mod form'
Assert-Equal 'not "mp"' (Row '/game_entity/(mod)').Args 'mod condition'
Assert-Equal 'num' (Row '/game_entity/limit').Shape 'block inside mod is attached to parent'

$rows = Rows '(define "m" {a %x} ("n") stray)'
Assert-Equal @('/(define "m")', '/(define "m")/a', '/(define "m")/("n")', '/(define "m")/<bare>') @($rows.Path) 'define body paths'
Assert-Equal 'param' $rows[1].Shape 'param shape'

$rows = Rows "actor`n{}`n{{x 1}}"
Assert-Equal @('/<bare>', '/{}', '/{}', '/{}/x') @($rows.Path) 'bare atom and unnamed blocks'

$rows = Rows '{"pistol" lvl_5}'
# 따옴표로 된 블록 이름은 키가 아니라 데이터다: 경로는 "*"로 묶고 이름은 첫 인자로 남긴다
Assert-Equal '/"*"' $rows[0].Path 'quoted block name collapses to wildcard segment'
Assert-Equal 'str atom' $rows[0].Shape 'quoted block name becomes first argument shape'
Assert-Equal '"pistol" lvl_5' $rows[0].Args 'quoted block name is kept in args'
$rows = Rows '{perks {"pistol" lvl_5} {"smg" lvl_4} {rifle 1}}'
Assert-Equal @('/perks', '/perks/"*"', '/perks/"*"', '/perks/rifle') @($rows.Path) 'sibling quoted names share one path'

# Review Focus 3: 대소문자만 다른 키는 하나로 집계
$agg = New-SdlKeyAggregate
foreach ($r in (Rows '{Extension "a.mdl"} {extension "b.mdl"} {EXTENSION "a.mdl"}')) { Add-SdlKeyAggregate -Agg $agg -Scope 'vehicle|.def' -Row $r -File 'f1' }
foreach ($r in (Rows '{mass 3.85} {mass 16}')) { Add-SdlKeyAggregate -Agg $agg -Scope 'vehicle|.def' -Row $r -File 'f2' }
$out = @(Export-SdlKeyAggregate -Agg $agg | Sort-Object Path)
Assert-Equal @('/extension', '/mass') @($out.Path) 'case-insensitive aggregation'
Assert-Equal 3 $out[0].Count 'extension count'
Assert-Equal 1 $out[0].FileCount 'extension file count'
Assert-Equal 3.85 $out[1].NumMin 'numeric min'
Assert-Equal ([double]16) $out[1].NumMax 'numeric max'
Assert-Equal 'vehicle|.def' $out[1].Scope 'scope kept'
Add-SdlKeyAggregateRows -Agg $agg -Scope 'vehicle|.def' -Rows (Rows '{mass 20} {mass 1}') -File 'f3'
$out = @(Export-SdlKeyAggregate -Agg $agg | Sort-Object Path)
Assert-Equal @(4, 2, [double]1, [double]20) @($out[1].Count, $out[1].FileCount, $out[1].NumMin, $out[1].NumMax) 'bulk add updates count, files, min, max'

$c = @(Get-SdlKeyComments -Text "`t{MaxSpeed`t40`t}`t;// max speed in kph`n{x 1}`n; {y 2} ; not a key line")
Assert-Equal 1 $c.Count 'one comment'
Assert-Equal 'maxspeed' $c[0].Name 'comment key name'
Assert-Equal 'max speed in kph' $c[0].Comment 'comment text'

# 카탈로그 한 줄에 쓰는 JSON: 행 배열 [[Path,Form,Args],...]
$json = ConvertTo-SdlRowsJson -Rows (Rows "{a `"x\y`t1`" 2} {b}")
Assert-Equal '[["/a","block","\"x\\y\t1\" 2"],["/b","block",""]]' $json 'rows json escapes quotes, backslash and control chars'
$back = ConvertFrom-Json $json
Assert-Equal "`"x\y`t1`" 2" $back[0][2] 'rows json round-trips through ConvertFrom-Json'
Assert-Equal '[]' (ConvertTo-SdlRowsJson -Rows (Rows '')) 'empty rows json'
Assert-Equal '"set/a\"b"' (ConvertTo-SdlJsonString 'set/a"b') 'json string'

Complete-Tests