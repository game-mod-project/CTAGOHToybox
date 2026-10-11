param(
    [Parameter(Mandatory)][string]$SourceRoot,
    [Parameter(Mandatory)][string]$OutDir,
    [Parameter(Mandatory)][string]$DataDir,
    [string[]]$ScopeInclude
)
. "$PSScriptRoot/lib/Common.ps1"
Add-Type -AssemblyName System.IO.Compression.FileSystem

if (-not $PSBoundParameters.ContainsKey('ScopeInclude')) { $ScopeInclude = $script:DefaultScopeInclude }
$SourceRoot = (Resolve-Path -LiteralPath $SourceRoot).Path.TrimEnd('\')
$OutDir = Resolve-FullPath $OutDir
$DataDir = Resolve-FullPath $DataDir
# 지난 실행의 파일이 남으면 사라진 파일이 계속 include 대상으로 잡힌다. 매번 비우고 시작한다.
# 이 스크립트가 만든 폴더(표식 파일이 있는 폴더)만 비운다
$marker = Join-Path $OutDir '.ctatoybox-work'
if ([IO.Directory]::Exists($OutDir) -and @([IO.Directory]::EnumerateFileSystemEntries($OutDir)).Count -gt 0) {
    if (-not [IO.File]::Exists($marker)) { throw "OutDir가 비어 있지 않고 이 스크립트가 만든 폴더가 아닙니다(표식 파일 .ctatoybox-work 없음): $OutDir" }
    foreach ($d in [IO.Directory]::GetDirectories($OutDir)) { [IO.Directory]::Delete($d, $true) }
    foreach ($f in [IO.Directory]::GetFiles($OutDir)) { if ($f -ne $marker) { [IO.File]::Delete($f) } }
}
[IO.Directory]::CreateDirectory($OutDir) | Out-Null
[IO.File]::WriteAllText($marker, 'Export-PakText.ps1 work folder. Cleared on every run.')
[IO.Directory]::CreateDirectory($DataDir) | Out-Null
$OutDir = (Resolve-Path -LiteralPath $OutDir).Path

$seen = @{}
$emptyRows = @{}   # 0바이트로 추출한 경로 -> 색인 행. 뒤에 같은 경로의 내용 있는 파일이 오면 충돌로 바꾼다
$index = [System.Collections.Generic.List[object]]::new()
$pakSummary = [System.Collections.Generic.List[object]]::new()
$sha = [Security.Cryptography.SHA1]::Create()

function Test-InScope([string]$vp) {
    if ($ScopeInclude.Count -eq 1 -and $ScopeInclude[0] -eq '') { return $true }
    foreach ($s in $ScopeInclude) { if ($vp.StartsWith($s)) { return $true } }
    $false
}

function Add-IndexRow([string]$vp, [string]$orig, [string]$source, [long]$length, [string]$status, [string]$reason, [string]$hash) {
    $row = [pscustomobject]@{
        VirtualPath = $vp; OriginalPath = $orig; Source = $source; Length = $length
        Ext = (Get-SdlFileKind $vp); Group = (Get-SdlFileGroup $vp); Status = $status; Reason = $reason; Sha1 = $hash
    }
    $index.Add($row)
    if ($status -eq 'empty' -and (Test-InScope $vp)) { $emptyRows[$vp] = $row }
}

# 바이트를 읽지 않고 결정되는 경우 @(status, reason), 아니면 $null
function Get-PathDisposition([string]$vp, [long]$length) {
    # pak의 항목 이름은 신뢰할 수 없다. 출력 폴더 밖으로 나가는 이름은 추출하지 않는다
    if (($vp -split '/') -contains '..' -or $vp.Contains(':')) { return @('excluded-unsafe', '출력 폴더 밖을 가리키는 경로') }
    if (-not (Test-InScope $vp)) { return @('excluded-scope', '유닛 정의 범위 밖 경로') }
    if ($length -eq 0) {
        # 0바이트 파일도 include 대상일 수 있다(예: properties/shadow/*.inc). 빈 파일로 추출해 둔다
        $dest = Join-Path $OutDir ($vp -replace '/', '\')
        [IO.Directory]::CreateDirectory((Split-Path -Parent $dest)) | Out-Null
        # 같은 경로에 이미 내용 있는 파일이 있으면 그 내용을 지우지 않고 충돌로 적는다
        if ($seen.ContainsKey($vp)) { return @('collision', "내용 있는 파일과 같은 경로의 0바이트 파일: $($seen[$vp].Source)") }
        if (-not [IO.File]::Exists($dest)) { [IO.File]::WriteAllBytes($dest, [byte[]]@()) }
        return @('empty', '0바이트')
    }
    $ext = Get-SdlFileKind $vp
    if ($script:BinaryExt -contains $ext) { return @('excluded-binary', "바이너리 확장자 $ext") }
    $null
}

function Register-Content([string]$vp, [string]$orig, [string]$source, [byte[]]$bytes) {
    $isUtf16 = ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE)
    if (-not $isUtf16 -and [Array]::IndexOf($bytes, [byte]0, 0, [Math]::Min(4096, $bytes.Length)) -ge 0) {
        Add-IndexRow $vp $orig $source $bytes.Length 'excluded-binary' 'NUL 바이트 포함' ''
        return
    }
    $hash = [Convert]::ToHexString($sha.ComputeHash($bytes))
    if ($seen.ContainsKey($vp)) {
        $first = $seen[$vp]
        if ($first.Sha1 -eq $hash) { Add-IndexRow $vp $orig $source $bytes.Length 'duplicate' "같은 내용: $($first.Source)" $hash }
        else { Add-IndexRow $vp $orig $source $bytes.Length 'collision' "다른 내용: $($first.Source)" $hash }
        return
    }
    $seen[$vp] = [pscustomobject]@{ Source = $source; Sha1 = $hash }
    # 앞서 같은 경로에 0바이트 파일이 있었다면 내용 있는 쪽을 쓰고, 0바이트 쪽을 충돌로 적는다
    if ($emptyRows.ContainsKey($vp)) {
        $emptyRows[$vp].Status = 'collision'
        $emptyRows[$vp].Reason = "같은 경로에 내용 있는 파일: $source"
        $emptyRows.Remove($vp)
    }
    $dest = Join-Path $OutDir ($vp -replace '/', '\')
    [IO.Directory]::CreateDirectory((Split-Path -Parent $dest)) | Out-Null
    [IO.File]::WriteAllBytes($dest, $bytes)
    $ext = Get-SdlFileKind $vp
    if ($script:ReferenceExt -contains $ext) { Add-IndexRow $vp $orig $source $bytes.Length 'reference-only' "SDL 아님($ext), 참조 목록" $hash }
    else { Add-IndexRow $vp $orig $source $bytes.Length 'text' '' $hash }
}

foreach ($pak in (Get-ChildItem -LiteralPath $SourceRoot -Recurse -File -Filter '*.pak' | Sort-Object FullName)) {
    $relPak = $pak.FullName.Substring($SourceRoot.Length + 1)
    $dir = Split-Path -Parent $relPak
    $prefixV = if ($dir) { (ConvertTo-VirtualPath $dir) + '/' } else { '' }
    $prefixO = if ($dir) { ($dir -replace '\\', '/') + '/' } else { '' }
    $extCount = @{}
    $files = 0
    $zip = [IO.Compression.ZipFile]::OpenRead($pak.FullName)
    try {
        foreach ($e in $zip.Entries) {
            if ($e.FullName.EndsWith('/')) { continue }
            $files++
            $vp = $prefixV + (ConvertTo-VirtualPath $e.FullName)
            $orig = $prefixO + $e.FullName
            $k = Get-SdlFileKind $vp
            $extCount[$k] = 1 + [int]$extCount[$k]
            $pre = Get-PathDisposition $vp $e.Length
            if ($null -ne $pre) { Add-IndexRow $vp $orig $relPak $e.Length $pre[0] $pre[1] ''; continue }
            $ms = [IO.MemoryStream]::new()
            $s = $e.Open()
            try { $s.CopyTo($ms) } finally { $s.Dispose() }
            Register-Content $vp $orig $relPak $ms.ToArray()
        }
    } finally { $zip.Dispose() }
    $top = ($extCount.GetEnumerator() | Sort-Object Value -Descending | Select-Object -First 8 | ForEach-Object { "$($_.Key)=$($_.Value)" }) -join ' '
    $pakSummary.Add([pscustomobject]@{ Pak = $relPak; SizeMB = [Math]::Round($pak.Length / 1MB, 1); Files = $files; TopExtensions = $top })
    Write-Host "pak $relPak files=$files"
}

foreach ($f in (Get-ChildItem -LiteralPath $SourceRoot -Recurse -File | Where-Object Extension -ne '.pak' | Sort-Object FullName)) {
    $rel = $f.FullName.Substring($SourceRoot.Length + 1)
    $vp = ConvertTo-VirtualPath $rel
    $orig = $rel -replace '\\', '/'
    $pre = Get-PathDisposition $vp $f.Length
    if ($null -ne $pre) { Add-IndexRow $vp $orig 'loose' $f.Length $pre[0] $pre[1] ''; continue }
    Register-Content $vp $orig 'loose' ([IO.File]::ReadAllBytes($f.FullName))
}

$index | Export-Csv -LiteralPath (Join-Path $DataDir 'file-index.csv') -NoTypeInformation -Encoding utf8
$pakSummary | Export-Csv -LiteralPath (Join-Path $DataDir 'pak-summary.csv') -NoTypeInformation -Encoding utf8
$index | Group-Object Status | Sort-Object Name | ForEach-Object { Write-Host ("{0,-16} {1}" -f $_.Name, $_.Count) }
