. "$PSScriptRoot/TestKit.ps1"
. "$PSScriptRoot/../lib/Common.ps1"
Add-Type -AssemblyName System.IO.Compression.FileSystem

function New-Pak([string]$PakPath, [hashtable]$Files) {
    $stage = New-TestDir
    foreach ($k in $Files.Keys) {
        $p = Join-Path $stage ($k -replace '/', '\')
        [IO.Directory]::CreateDirectory((Split-Path -Parent $p)) | Out-Null
        [IO.File]::WriteAllBytes($p, [byte[]]$Files[$k])
    }
    [IO.Directory]::CreateDirectory((Split-Path -Parent $PakPath)) | Out-Null
    [IO.Compression.ZipFile]::CreateFromDirectory($stage, $PakPath)
}
function B([string]$s) { ,[Text.Encoding]::ASCII.GetBytes($s) }

# --- 분류 함수
Assert-Equal 'set/breed/a.set' (ConvertTo-VirtualPath '\Set\Breed\A.set') 'virtual path'
Assert-Equal 'human' (Get-SdlFileGroup 'set/breed/mp/ger/x.set') 'group breed'
Assert-Equal 'vehicle' (Get-SdlFileGroup 'entity/-vehicle/germany/t/t.def') 'group vehicle'
Assert-Equal 'weapon-ammo' (Get-SdlFileGroup 'set/stuff/rifle/garand') 'group stuff'
Assert-Equal 'shared' (Get-SdlFileGroup 'properties/tank.ext') 'group shared'
Assert-Equal '(no-ext)' (Get-SdlFileKind 'set/stuff/rifle/garand') 'kind no ext'
Assert-Equal '.presets' (Get-SdlFileKind 'set/stuff/rifle/.presets') 'kind dotfile'
Assert-Equal '.ammo' (Get-SdlFileKind 'set/stuff/mortar/m.he.ammo') 'kind ext'
Assert-Equal 'stuff' (Get-SdlUnitKind 'set/stuff/rifle/garand') 'unit kind stuff'
Assert-Equal 'support' (Get-SdlUnitKind 'set/stuff/rifle/.presets') 'unit kind presets'
Assert-Equal 'pattern' (Get-SdlUnitKind 'set/stuff/rifle/rifle.pattern') 'unit kind pattern'
Assert-Equal 'breed' (Get-SdlUnitKind 'set/breed/mp/ger/x.set') 'unit kind breed'

# --- 추출
$src = New-TestDir; $out = New-TestDir; $data = New-TestDir
$nonAscii = [byte[]](0x7B, 0x61, 0x20, 0x31, 0x7D, 0x20, 0x3B, 0xC0, 0xE0, 0xFF)   # {a 1} ;<3 non-ascii bytes>
New-Pak (Join-Path $src 'a_gamelogic.pak') @{
    'set/a.set'      = (B '{a 1}')
    'set/b.ply'      = [byte[]](1, 2, 3)
    'set/nul.set'    = [byte[]](0x7B, 0x00, 0x7D)
    'set/empty.set'  = [byte[]]@()
    'set/same.set'   = (B '{s 1}')
    'script/x.lua'   = (B 'print(1)')
    'map/m.mi'       = (B '{mission}')
    'set/cyr.set'    = $nonAscii
}
New-Pak (Join-Path $src 'b_dup.pak') @{
    'set/a.set'    = (B '{a 2}')
    'set/same.set' = (B '{s 1}')
}
New-Pak (Join-Path $src 'entity\-vehicle.pak') @{ '-vehicle/T/T.def' = (B '{game_entity}') }
[IO.Directory]::CreateDirectory((Join-Path $src 'set\loose')) | Out-Null
[IO.File]::WriteAllBytes((Join-Path $src 'set\loose\c.set'), (B '{c 3}'))

pwsh -NoProfile -File "$PSScriptRoot/../Export-PakText.ps1" -SourceRoot $src -OutDir $out -DataDir $data | Out-Null
Assert-Equal 0 $LASTEXITCODE 'extractor exit code'

$idx = @(Import-Csv (Join-Path $data 'file-index.csv'))
function St([string]$vp, [string]$source) { @($idx | Where-Object { $_.VirtualPath -eq $vp -and $_.Source -like $source })[0].Status }

Assert-Equal 'text' (St 'set/a.set' 'a_gamelogic.pak') 'first copy is text'
Assert-Equal 'collision' (St 'set/a.set' 'b_dup.pak') 'different content collides'
Assert-Equal 'duplicate' (St 'set/same.set' 'b_dup.pak') 'identical content is duplicate'
Assert-Equal 'excluded-binary' (St 'set/b.ply' '*') 'binary extension'
Assert-Equal 'excluded-binary' (St 'set/nul.set' '*') 'NUL sniff'
Assert-Equal 'empty' (St 'set/empty.set' '*') 'empty file'
Assert-Equal 'reference-only' (St 'script/x.lua' '*') 'lua is reference only'
Assert-Equal 'excluded-scope' (St 'map/m.mi' '*') 'map is out of scope'
Assert-Equal 'text' (St 'entity/-vehicle/t/t.def' '*') 'pak folder becomes virtual prefix'
Assert-Equal 'text' (St 'set/loose/c.set' 'loose') 'loose file'
Assert-Equal 'vehicle' (@($idx | Where-Object VirtualPath -eq 'entity/-vehicle/t/t.def')[0].Group) 'group column'
Assert-Equal '-vehicle/T/T.def' (@($idx | Where-Object VirtualPath -eq 'entity/-vehicle/t/t.def')[0].OriginalPath.Split('/', 2)[1]) 'original case kept'

Assert-Equal '{a 1}' ([IO.File]::ReadAllText((Join-Path $out 'set\a.set'))) 'first copy wins on disk'
Assert-Equal $nonAscii ([IO.File]::ReadAllBytes((Join-Path $out 'set\cyr.set'))) 'non-ascii bytes preserved'
Assert-Equal $false (Test-Path -LiteralPath (Join-Path $out 'map\m.mi')) 'out-of-scope not extracted'
Assert-Equal 3 @(Import-Csv (Join-Path $data 'pak-summary.csv')).Count 'pak summary rows'
# 범위 안의 0바이트 파일은 include 대상일 수 있으므로 빈 파일로 추출한다 (상태는 empty 그대로)
Assert-Equal $true (Test-Path -LiteralPath (Join-Path $out 'set\empty.set')) 'in-scope empty file is extracted'
Assert-Equal 0 ([IO.FileInfo]::new((Join-Path $out 'set\empty.set'))).Length 'extracted empty file has zero length'

Complete-Tests
