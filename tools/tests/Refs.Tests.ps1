. "$PSScriptRoot/TestKit.ps1"
. "$PSScriptRoot/../lib/SdlTokenizer.ps1"
. "$PSScriptRoot/../lib/SdlParser.ps1"
. "$PSScriptRoot/../lib/SdlKeys.ps1"
. "$PSScriptRoot/../lib/SdlRefs.ps1"

function Rows([string]$text) { , (Get-SdlKeyRows -Root (ConvertFrom-Sdl -Text $text).Root) }

$ix = New-SdlReferenceIndex
$ix.AddName('garand', 'stuff')
$ix.AddName('us_helmet', 'entity')
$ix.AddName('self_name', 'entity')
$ix.AddName('ab', 'stuff')      # 3글자 미만은 무시
$ix.AddName('123', 'stuff')     # 숫자뿐인 이름은 무시
Assert-Equal 3 $ix.NameCount 'short and numeric names are ignored'

$sw = [IO.StringWriter]::new()
$rows = Rows '{breed {inventory {item "garand" filling "rifle_usa garand ammo" 8} {item "GARAND"}} {armors {head us_helmet}} {skin "self_name"} {tag "unknown"}}'
$added = $ix.Scan('set/breed/x.set', 'self_name', $rows, $sw)
$lines = @($sw.ToString() -split "`r?`n" | Where-Object { $_ })
Assert-Equal @(
    '"set/breed/x.set","/breed/inventory/item",stuff,"garand",whole',
    '"set/breed/x.set","/breed/inventory/item",stuff,"garand",token',
    '"set/breed/x.set","/breed/armors/head",entity,"us_helmet",whole'
) $lines 'whole and token matches, case-insensitive dedupe, self and unknown names skipped'
Assert-Equal 3 $added 'scan returns number of new references'

$sw2 = [IO.StringWriter]::new()
Assert-Equal 0 ($ix.Scan('set/breed/x.set', 'self_name', $rows, $sw2)) 'same file scanned twice adds nothing'

Complete-Tests
