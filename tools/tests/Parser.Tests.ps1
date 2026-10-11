. "$PSScriptRoot/TestKit.ps1"
. "$PSScriptRoot/../lib/SdlTokenizer.ps1"
. "$PSScriptRoot/../lib/SdlParser.ps1"

$sample = @'
;SDL
actor
{game_entity
	(include "/properties/cannon.ext")
	{Extension "x.mdl"}
	("gun" repair(30) thickness(25))
	{volume "shield1" {able {holed}}}
	{speed (* 0.35 %mps)}
	(mod not "mp" {limit 200})
}
'@
$r = Test-SdlRoundTrip -Text $sample
Assert-Equal $true $r.Ok 'sample round trip'
Assert-Equal 0 $r.Errors.Count 'sample has no errors'

$p = ConvertFrom-Sdl -Text '("m" w(1.5) args "a")'
$paren = $p.Root.Children[0]
Assert-Equal 'paren' $paren.Kind 'macro is paren'
Assert-Equal 'call' $paren.Children[1].Kind 'glued becomes call'
Assert-Equal 'w' $paren.Children[1].Text 'call name'
Assert-Equal '1.5' $paren.Children[1].Children[0].Text 'call argument'
Assert-Equal 4 $paren.Children.Count 'call consumed the atom'
Assert-Equal '( "m" w ( 1.5 ) args "a" )' (Get-SdlNodesText $p.Root.Children) 'flat text'

$p = ConvertFrom-Sdl -Text '{a {b 1} c}'
Assert-Equal 'brace' $p.Root.Children[0].Children[1].Kind 'nested brace'
Assert-Equal $null $p.Root.Children[0].Children[0].Children 'leaf has no children list'

# Review Focus 1: 깨진 입력은 예외 없이 실패로 기록
foreach ($bad in '{a {b}', '{a}}', '{a)', '(a}', '{a "bc', ')') {
    $r = Test-SdlRoundTrip -Text $bad
    Assert-Equal $false $r.Ok "broken input is not ok: $bad"
    Assert-Equal $true ($r.Errors.Count -ge 1) "broken input reports an error: $bad"
}

$r = Test-SdlRoundTrip -Text ''
Assert-Equal $true $r.Ok 'empty is ok'
Assert-Equal 0 $r.TokenCount 'empty has no tokens'

$r = Test-SdlRoundTrip -Text "; only a comment`n"
Assert-Equal $true $r.Ok 'comment only is ok'

# 리뷰 I5: 토크나이저와 독립된 문자 수 검사. 주석과 공백을 뺀 글자 수가 토큰 길이의 합과 같아야 한다
Assert-Equal 8 ([SdlCore]::CountSignificantChars('{a "b c"} ; x')) 'significant chars skip comments and outer whitespace, keep spaces inside strings'
Assert-Equal 8 ([SdlCore]::TokenChars((Get-SdlTokens -Text '{a "b c"} ; x'))) 'token chars'
Assert-Equal 3 ([SdlCore]::CountSignificantChars("a;c`n`"x")) 'comment ends at line end, unterminated string counted'
$r = Test-SdlRoundTrip -Text $sample
Assert-Equal $true $r.CharsOk 'sample passes the independent char check'

Complete-Tests
