. "$PSScriptRoot/TestKit.ps1"
Add-Type -AssemblyName System.IO.Compression.FileSystem

# 작은 트리로 스키마 문서 생성기와 누락 검증을 끝까지 돌린다
function Put([string]$root, [string]$rel, [string]$text) {
    $p = Join-Path $root ($rel -replace '/', '\')
    [IO.Directory]::CreateDirectory((Split-Path -Parent $p)) | Out-Null
    [IO.File]::WriteAllText($p, $text)
}
$stage = New-TestDir; $src = New-TestDir; $work = New-TestDir; $data = New-TestDir; $docs = New-TestDir; $ann = New-TestDir
Put $stage 'set/breed/mp/ger/a.set' "{breed {skin `"s1`"} (include `"ability.inc`") (`"hp`" v(5))`n{mass 3.5} ;// weight in kg`n}"
Put $stage 'set/breed/mp/ger/ability.inc' '(define "hp" {health %v})'
Put $stage 'set/breed/mp/ger/b.set' '{breed {skin "s2"}'            # 닫는 괄호가 없다
[IO.Compression.ZipFile]::CreateFromDirectory($stage, (Join-Path $src 'gamelogic.pak'))
[IO.File]::WriteAllText((Join-Path $ann 'common.psv'), "# comment`n*/skin|block|추정|외형 이름|S`n**/health|block|추정|체력|S`n")
& "$PSScriptRoot/../Invoke-Pipeline.ps1" -SourceRoot $src -WorkDir $work -DataDir $data *> $null

function Coverage { pwsh -NoProfile -File "$PSScriptRoot/../Test-Coverage.ps1" -DataDir $data -DocsDir $docs | Select-Object -Last 1 }
$line = Coverage
Assert-Equal $true ($LASTEXITCODE -ne 0) 'coverage fails before the schema docs exist'
Assert-Equal $true ($line -match 'keys-missing=[1-9]') 'missing keys are counted'

& "$PSScriptRoot/../New-SchemaDoc.ps1" -DataDir $data -AnnotationDir $ann -OutDir $docs *> $null
$line = Coverage
Assert-Equal 0 $LASTEXITCODE 'coverage passes once the docs are generated'
Assert-Equal $true ($line.Contains('keys-missing=0') -and $line.Contains('roundtrip-failed=1') -and $line.Contains('chars-failed=0') -and $line.Contains('pak-mismatch=0')) 'coverage summary line'
$schema = [IO.File]::ReadAllText((Join-Path $docs 'schema-human.md'))
Assert-Equal $true ($schema -match '\| `/breed/skin` \| block \|[^\n]*\| 추정 \| 외형 이름 \| 파일 구조와 값에서 추론 \|') 'annotation is applied with its source name'
Assert-Equal $true ($schema -match '\| `/breed/mass` \| block \|[^\n]*\| 미확인\(자동\) \|[^\n]*weight in kg') 'unannotated key is auto-unknown and shows the in-file comment'
$cov = [IO.File]::ReadAllText((Join-Path $docs 'coverage.md'))
Assert-Equal $true ($cov.Contains('`set/breed/mp/ger/b.set`')) 'round-trip failure is listed by file'

# 수정 가능한 속성 문서: 값이 있는 키만, 뜻을 아는 것과 모르는 것으로 나뉜다
$edit = New-TestDir
& "$PSScriptRoot/../New-EditableDoc.ps1" -DataDir $data -AnnotationDir $ann -OutDir $edit *> $null
$inf = [IO.File]::ReadAllText((Join-Path $edit 'infantry.md'))
Assert-Equal $true ($inf -match '\| `/breed/skin` \|[^\n]*"s1"[^\n]*\| \*\*추정\*\* 외형 이름 \|') 'annotated key with a value is listed as known'
Assert-Equal $true ($inf -match '### 뜻을 모르는 속성 \(1개\)\s+\|[^\n]*\n\|[-|]*\s+\| `/breed/mass` \| 3\.5 \| 미확인 \| weight in kg \|') 'unannotated key shows its value and the in-file comment'
Assert-Equal $false ($inf.Contains('| `/breed` |')) 'a block without a value is not an editable attribute'
Assert-Equal $true ($inf.Contains('| `/breed/("hp")` | v ( 5 ) | **정의** 매크로 호출. 인자가 들어가는 자리: `v` → `health` |')) 'macro call is described from its definition'
Assert-Equal $true ($inf.Contains('| `/(define "hp")/health` | %v | **추정** 체력 |')) 'a tail annotation reaches keys inside a macro definition'
Assert-Equal $true ([IO.File]::ReadAllText((Join-Path $edit 'README.md')) -match '\| 합계 \| \| \| \| 4 \| 0 \| 0 \| 2 \| 1 \| 1 \|') 'readme totals by grade'

# 문서에서 한 행을 지우면 잡힌다
$p = Join-Path $docs 'schema-human.md'
[IO.File]::WriteAllLines($p, @([IO.File]::ReadAllLines($p) | Where-Object { -not $_.StartsWith('| `/breed/skin`') }))
$line = Coverage
Assert-Equal $true ($LASTEXITCODE -ne 0 -and $line -match 'keys-missing=[1-9]') 'a deleted doc row is caught'
& "$PSScriptRoot/../New-SchemaDoc.ps1" -DataDir $data -AnnotationDir $ann -OutDir $docs *> $null

# 색인에서 한 행이 빠지면 pak 항목 수 검사가 잡는다
$ix = Join-Path $data 'file-index.csv'; $rows = @(Import-Csv $ix)
$rows | Select-Object -Skip 1 | Export-Csv $ix -NoTypeInformation -Encoding utf8
$line = Coverage
Assert-Equal $true ($LASTEXITCODE -ne 0 -and $line.Contains('pak-mismatch=1')) 'an index row missing for a pak is caught'
$rows | Export-Csv $ix -NoTypeInformation -Encoding utf8

# 글자 수 검사에 실패한 파일이 있으면 잡는다
$pr = Join-Path $data 'parse-results.csv'; $prRows = @(Import-Csv $pr); $prRows[0].CharsOk = 'False'
$prRows | Export-Csv $pr -NoTypeInformation -Encoding utf8
$line = Coverage
Assert-Equal $true ($LASTEXITCODE -ne 0 -and $line.Contains('chars-failed=1')) 'a failed character check is caught'

# 표시 이름: gettext .pot에서 msgctxt와 msgid를 뽑는다
$locStage = New-TestDir; $locPak = Join-Path (New-TestDir) 'default.pak'; $locData = New-TestDir
Put $locStage 'default/interface/text/desc/desc_x.pot' "msgid `"`"`nmsgstr `"`"`n`"Header: x\n`"`n`nmsgctxt `"desc/entity/tiger`"`nmsgid `"Tiger `"`n`"I`"`nmsgstr `"`"`n`nmsgctxt `"desc/human/mp/ger/a`"`nmsgid `"Rifleman`"`nmsgstr `"`"`n"
[IO.Compression.ZipFile]::CreateFromDirectory($locStage, $locPak)
& "$PSScriptRoot/../Build-DisplayNames.ps1" -LocPak $locPak -DataDir $locData *> $null
$names = @(Import-Csv (Join-Path $locData 'display-names.csv'))
Assert-Equal @('desc/entity/tiger', 'desc/human/mp/ger/a') @($names.Context) 'contexts are read; the header entry without a context is skipped'
Assert-Equal @('Tiger I', 'Rifleman') @($names.Text) 'multi-line msgid is joined'

Complete-Tests