. "$PSScriptRoot/TestKit.ps1"
. "$PSScriptRoot/../lib/SdlTokenizer.ps1"

$t = Get-SdlTokens -Text '{a "b c" 1} ; trailing'
Assert-Equal @('{', 'a', '"b c"', '1', '}') @($t.Text) 'basic tokens'
Assert-Equal @('open', 'atom', 'str', 'atom', 'close') @($t.Kind) 'basic kinds'

$t = Get-SdlTokens -Text '("fire rate" rpm(240))'
Assert-Equal @('(', '"fire rate"', 'rpm', '(', '240', ')', ')') @($t.Text) 'call tokens'
Assert-Equal @($false, $false, $false, $true, $false, $false, $false) @($t.Glued) 'glued paren'

$t = Get-SdlTokens -Text '{recoveryTime (/ 60 %rpm)}'
Assert-Equal @($false, $false, $false, $false, $false, $false, $false, $false) @($t.Glued) 'spaced paren is not glued'
Assert-Equal '%rpm' $t[5].Text 'param atom'

$t = Get-SdlTokens -Text '{"dirt/*" "ground"}'
Assert-Equal @('{', '"dirt/*"', '"ground"', '}') @($t.Text) 'slash-star stays inside string'

$t = Get-SdlTokens -Text "{##damage add} ;// it's ""odd`n{b}"
Assert-Equal @('{', '##damage', 'add', '}', '{', 'b', '}') @($t.Text) 'hash names and quote inside comment'

$t = Get-SdlTokens -Text '{a 1};x'
Assert-Equal @('{', 'a', '1', '}') @($t.Text) 'comment right after brace'

$t = Get-SdlTokens -Text '{a "bc'
Assert-Equal @('open', 'atom', 'badstr') @($t.Kind) 'unterminated string'

$t = Get-SdlTokens -Text ''
Assert-Equal 0 $t.Count 'empty text'

# M7: 공백은 ASCII 공백만이다. Latin-1로 읽은 0x85, 0xA0 바이트는 글자다(UTF-8 키릴 문자의 일부일 수 있다)
$t = Get-SdlTokens -Text ("{a" + [char]0x85 + "b c" + [char]0xA0 + "d}")
Assert-Equal 4 $t.Count 'non-ASCII bytes do not split tokens'
Assert-Equal ("a" + [char]0x85 + "b") $t[1].Text 'byte 0x85 stays inside the atom'

Complete-Tests
