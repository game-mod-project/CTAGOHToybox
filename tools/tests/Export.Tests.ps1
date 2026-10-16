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
# M13: set/stuff의 설정 파일은 아이템이 아니다
Assert-Equal 'support' (Get-SdlUnitKind 'set/stuff/_settings/hand-only.fsm') 'fsm settings are not items'
Assert-Equal 'support' (Get-SdlUnitKind 'set/stuff/thrower.hand') 'hand settings are not items'
Assert-Equal 'support' (Get-SdlUnitKind 'set/stuff/_settings/something') 'anything under _settings is not an item'

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
    'set/img.pdn'    = (B 'PDN3<pdnImage width="4"><layers>')   # 이미지 편집기 파일: 앞부분은 글자지만 텍스트 정의가 아니다
    'set/over.set'   = (B '{o 1}')                               # D16: 같은 경로의 낱개 파일이 이긴다
}
New-Pak (Join-Path $src 'b_dup.pak') @{
    'set/a.set'    = (B '{a 2}')
    'set/same.set' = (B '{s 1}')
    'set/empty.set' = (B '{late 1}')     # M1: 앞 pak에서는 0바이트였던 경로에 내용이 있는 파일
    'set/cyr.set'  = [byte[]]@()         # M1: 앞 pak에서는 내용이 있던 경로에 0바이트 파일
}
New-Pak (Join-Path $src 'entity\-vehicle.pak') @{ '-vehicle/T/T.def' = (B '{game_entity}') }
# 리뷰 I2: 상위 폴더로 빠져나가는 항목 이름을 가진 pak
$zs = [IO.File]::Create((Join-Path $src 'c_evil.pak'))
$za = [IO.Compression.ZipArchive]::new($zs, [IO.Compression.ZipArchiveMode]::Create)
$escapeName = 'escaped-' + [guid]::NewGuid().ToString('N') + '.set'   # 실행마다 다른 이름: 이전 실행의 흔적과 섞이지 않게
foreach ($pair in @(@("set/../../$escapeName", '{x 1}'), @('set/run.log', 'Error: at f (node:net:1:2)'))) {
    $st = $za.CreateEntry($pair[0]).Open(); $bb = [Text.Encoding]::ASCII.GetBytes($pair[1]); $st.Write($bb, 0, $bb.Length); $st.Dispose()
}
$za.Dispose(); $zs.Dispose()
[IO.Directory]::CreateDirectory((Join-Path $src 'set\loose')) | Out-Null
[IO.File]::WriteAllBytes((Join-Path $src 'set\loose\c.set'), (B '{c 3}'))
[IO.File]::WriteAllBytes((Join-Path $src 'set\over.set'), (B '{o 2}'))

pwsh -NoProfile -File "$PSScriptRoot/../Export-PakText.ps1" -SourceRoot $src -OutDir $out -DataDir $data | Out-Null
Assert-Equal 0 $LASTEXITCODE 'extractor exit code'

$idx = @(Import-Csv (Join-Path $data 'file-index.csv'))
function St([string]$vp, [string]$source) { @($idx | Where-Object { $_.VirtualPath -eq $vp -and $_.Source -like $source })[0].Status }

Assert-Equal 'text' (St 'set/a.set' 'a_gamelogic.pak') 'first copy is text'
Assert-Equal 'collision' (St 'set/a.set' 'b_dup.pak') 'different content collides'
Assert-Equal 'duplicate' (St 'set/same.set' 'b_dup.pak') 'identical content is duplicate'
Assert-Equal 'excluded-binary' (St 'set/b.ply' '*') 'binary extension'
Assert-Equal 'excluded-binary' (St 'set/nul.set' '*') 'NUL sniff'
# (0바이트 파일의 상태는 아래 M1 검사에서 확인한다)
Assert-Equal 'excluded-binary' (St 'set/img.pdn' '*') 'image editor file with a text-like header is binary'
Assert-Equal 'reference-only' (St 'script/x.lua' '*') 'lua is reference only'
Assert-Equal 'excluded-scope' (St 'map/m.mi' '*') 'map is out of scope'
Assert-Equal 'text' (St 'entity/-vehicle/t/t.def' '*') 'pak folder becomes virtual prefix'
Assert-Equal 'text' (St 'set/loose/c.set' 'loose') 'loose file'
Assert-Equal 'vehicle' (@($idx | Where-Object VirtualPath -eq 'entity/-vehicle/t/t.def')[0].Group) 'group column'
Assert-Equal '-vehicle/T/T.def' (@($idx | Where-Object VirtualPath -eq 'entity/-vehicle/t/t.def')[0].OriginalPath.Split('/', 2)[1]) 'original case kept'

Assert-Equal '{a 1}' ([IO.File]::ReadAllText((Join-Path $out 'set\a.set'))) 'first copy wins on disk'
# D16: 한 모드 안에서 pak과 낱개 파일이 겹치면 게임은 낱개 파일을 쓴다(ingame-check.md 일괄 확인 8번)
Assert-Equal 'text' (St 'set/over.set' 'loose') 'a loose file over a pak entry is the text that counts'
Assert-Equal 'collision' (St 'set/over.set' 'a_gamelogic.pak') 'the pak entry under a loose file is marked as colliding'
Assert-Equal '{o 2}' ([IO.File]::ReadAllText((Join-Path $out 'set\over.set'))) 'the loose file wins on disk'
Assert-Equal $nonAscii ([IO.File]::ReadAllBytes((Join-Path $out 'set\cyr.set'))) 'non-ascii bytes preserved'
Assert-Equal $false (Test-Path -LiteralPath (Join-Path $out 'map\m.mi')) 'out-of-scope not extracted'
Assert-Equal 4 @(Import-Csv (Join-Path $data 'pak-summary.csv')).Count 'pak summary rows'
Assert-Equal 'excluded-unsafe' (@($idx | Where-Object OriginalPath -like "*$escapeName")[0].Status) 'entry escaping the output folder is refused'
Assert-Equal $false (Test-Path -LiteralPath (Join-Path (Split-Path -Parent $out) $escapeName)) 'nothing is written outside the output folder'
Assert-Equal 'reference-only' (St 'set/run.log' '*') 'log files are not parsed as definitions'
# 범위 안의 0바이트 파일은 include 대상일 수 있으므로 빈 파일로 추출한다 (상태는 empty 그대로)
New-Pak (Join-Path $src 'd_only_empty.pak') @{ 'set/alone_empty.inc' = [byte[]]@() }

# M1: 0바이트 파일과 내용 있는 파일이 같은 경로에 있으면 조용히 넘어가지 않는다
Assert-Equal 'text' (St 'set/empty.set' 'b_dup.pak') 'content wins over an earlier empty file'
Assert-Equal 'collision' (St 'set/empty.set' 'a_gamelogic.pak') 'the earlier empty file is marked as colliding'
Assert-Equal '{late 1}' ([IO.File]::ReadAllText((Join-Path $out 'set\empty.set'))) 'content replaces the empty placeholder on disk'
Assert-Equal 'collision' (St 'set/cyr.set' 'b_dup.pak') 'a later empty file at a path with content is marked as colliding'
Assert-Equal $nonAscii ([IO.File]::ReadAllBytes((Join-Path $out 'set\cyr.set'))) 'a later empty file does not erase content'

# M11: 다시 추출하면 지난 실행의 파일이 남지 않는다. 범위 안의 0바이트 파일은 빈 파일로 추출된다
[IO.File]::WriteAllText((Join-Path $out 'set\stale.set'), '{stale 1}')
pwsh -NoProfile -File "$PSScriptRoot/../Export-PakText.ps1" -SourceRoot $src -OutDir $out -DataDir $data | Out-Null
Assert-Equal $false (Test-Path -LiteralPath (Join-Path $out 'set\stale.set')) 'files from an earlier run are gone'
Assert-Equal $true (Test-Path -LiteralPath (Join-Path $out 'set\a.set')) 'current files are extracted again'
$idx = @(Import-Csv (Join-Path $data 'file-index.csv'))
Assert-Equal 'empty' (St 'set/alone_empty.inc' '*') 'in-scope empty file keeps status empty'
Assert-Equal 0 ([IO.FileInfo]::new((Join-Path $out 'set\alone_empty.inc'))).Length 'in-scope empty file is extracted with zero length'
# 표식이 없는 폴더는 건드리지 않고 멈춘다
$foreign = New-TestDir; [IO.File]::WriteAllText((Join-Path $foreign 'keep.txt'), 'mine')
pwsh -NoProfile -File "$PSScriptRoot/../Export-PakText.ps1" -SourceRoot $src -OutDir $foreign -DataDir (New-TestDir) *> $null
Assert-Equal $true ($LASTEXITCODE -ne 0) 'refuses to clear a non-empty folder it did not create'
Assert-Equal 'mine' ([IO.File]::ReadAllText((Join-Path $foreign 'keep.txt'))) 'foreign folder is left untouched'

# M8: 상대 경로는 PowerShell의 현재 위치 기준이다 (프로세스 시작 폴더가 아니라)
$base = New-TestDir; Copy-Item -Recurse -LiteralPath $src -Destination (Join-Path $base 'src')
$exportScript = (Resolve-Path "$PSScriptRoot/../Export-PakText.ps1").Path
pwsh -NoProfile -Command "Set-Location -LiteralPath '$base'; & '$exportScript' -SourceRoot src -OutDir out -DataDir data" *> $null
Assert-Equal $true (Test-Path -LiteralPath (Join-Path $base 'data\file-index.csv')) 'relative DataDir resolves against the current location'
Assert-Equal $true (Test-Path -LiteralPath (Join-Path $base 'out\set\a.set')) 'relative OutDir resolves against the current location'

Complete-Tests
