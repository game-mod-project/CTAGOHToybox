Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'

$script:Latin1 = [Text.Encoding]::GetEncoding(28591)
$script:DefaultScopeInclude = @('set/', 'script/', 'properties/', 'entity/-vehicle/', 'entity/humanskin/', 'entity/inventory/')
$script:BinaryExt = @('.ply', '.vol', '.anm', '.dds', '.tga', '.ebm', '.png', '.jpg', '.bmp', '.wav', '.ogg', '.mp3', '.avi', '.bik', '.ttf', '.pso', '.vso',
    '.pdn', '.psd', '.xcf', '.max', '.blend', '.fbx', '.zip', '.7z', '.rar')   # 모드에 섞여 들어오는 작업 파일: 앞부분에 NUL이 없을 수 있다
$script:ReferenceExt = @('.lua', '.md', '.txt', '.log', '.ini')

function Read-SdlText {
    param([Parameter(Mandatory)][string]$Path)
    $b = [IO.File]::ReadAllBytes($Path)
    if ($b.Length -ge 2 -and $b[0] -eq 0xFF -and $b[1] -eq 0xFE) { return [Text.Encoding]::Unicode.GetString($b, 2, $b.Length - 2) }
    if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) { return [Text.Encoding]::UTF8.GetString($b, 3, $b.Length - 3) }
    $script:Latin1.GetString($b)
}

function ConvertTo-VirtualPath {
    param([Parameter(Mandatory)][AllowEmptyString()][string]$p)
    ($p -replace '\\', '/').TrimStart('/').ToLowerInvariant()
}

function Get-SdlFileGroup {
    param([Parameter(Mandatory)][string]$VirtualPath)
    if ($VirtualPath.StartsWith('set/breed/')) { return 'human' }
    if ($VirtualPath.StartsWith('entity/humanskin/')) { return 'human' }
    if ($VirtualPath.StartsWith('entity/-vehicle/')) { return 'vehicle' }
    if ($VirtualPath.StartsWith('set/stuff/')) { return 'weapon-ammo' }
    if ($VirtualPath.StartsWith('entity/inventory/')) { return 'inventory' }
    'shared'
}

function Get-SdlFileKind {
    param([Parameter(Mandatory)][string]$VirtualPath)
    $leaf = $VirtualPath.Substring($VirtualPath.LastIndexOf('/') + 1)
    if ($leaf.StartsWith('.')) { return $leaf }
    $dot = $leaf.LastIndexOf('.')
    if ($dot -lt 0) { return '(no-ext)' }
    $leaf.Substring($dot)
}

function Get-SdlUnitKind {
    param([Parameter(Mandatory)][string]$VirtualPath)
    $kind = Get-SdlFileKind $VirtualPath
    if ($VirtualPath.StartsWith('set/breed/') -and $kind -eq '.set') { return 'breed' }
    if ($VirtualPath.StartsWith('entity/humanskin/') -and $kind -eq '.def') { return 'humanskin' }
    if ($VirtualPath.StartsWith('entity/-vehicle/') -and $kind -eq '.def') { return 'vehicle' }
    if ($VirtualPath.StartsWith('entity/inventory/') -and $kind -eq '.def') { return 'item-entity' }
    if ($VirtualPath.StartsWith('set/stuff/')) {
        if ($kind -eq '.pattern') { return 'pattern' }
        if ($kind -notin '.presets', '.inc') { return 'stuff' }
    }
    'support'
}

function Get-SdlLeafName {
    # 파일명에서 마지막 확장자를 뗀 이름. 점으로 시작하는 파일은 그대로.
    param([Parameter(Mandatory)][string]$VirtualPath)
    $leaf = $VirtualPath.Substring($VirtualPath.LastIndexOf('/') + 1)
    if ($leaf.StartsWith('.')) { return $leaf }
    $dot = $leaf.LastIndexOf('.')
    if ($dot -lt 0) { return $leaf }
    $leaf.Substring(0, $dot)
}
