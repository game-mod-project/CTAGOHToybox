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

Complete-Tests
