param(
    [Parameter(Mandatory)][string]$DataDir,
    [Parameter(Mandatory)][string]$DocsDir
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$groups = 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared'
$allowed = 'text', 'reference-only', 'excluded-scope', 'excluded-binary', 'excluded-unsafe', 'empty', 'duplicate'

# 검사 1: 파일
$index = @(Import-Csv -LiteralPath (Join-Path $DataDir 'file-index.csv'))
$byStatus = $index | Group-Object Status | Sort-Object Name
$badFiles = @($index | Where-Object { $_.Status -notin $allowed })

# 검사 2: 왕복
$parse = @(Import-Csv -LiteralPath (Join-Path $DataDir 'parse-results.csv'))
$failedParse = @($parse | Where-Object Ok -ne 'True')
# 토크나이저와 독립된 글자 수 검사에 실패한 파일: 토크나이저가 글자를 버렸다는 뜻이다
$failedChars = @($parse | Where-Object { $_.PSObject.Properties['CharsOk'] -and $_.CharsOk -ne 'True' })
# pak의 실제 항목 수와 색인의 행 수 대조
$rowsBySource = @{}; foreach ($g in ($index | Group-Object Source)) { $rowsBySource[$g.Name] = $g.Count }
$pakMismatch = @(Import-Csv -LiteralPath (Join-Path $DataDir 'pak-summary.csv') | Where-Object { [int]$_.Files -ne [int]$rowsBySource[$_.Pak] })

# 검사 3: 키
$expected = [System.Collections.Generic.HashSet[string]]::new()
foreach ($g in $groups) {
    foreach ($k in (Import-Csv -LiteralPath (Join-Path $DataDir "keys-$g.csv"))) { [void]$expected.Add("$g`tK`t$($k.FileKind)`t$($k.Path)`t$($k.Form)") }
}
foreach ($k in (Import-Csv -LiteralPath (Join-Path $DataDir 'keys-resolved.csv'))) { [void]$expected.Add("$($k.Group)`tR`t$($k.Kind)`t$($k.Path)`t$($k.Form)") }

$actual = [System.Collections.Generic.HashSet[string]]::new()
$grades = @{}
$rowRegex = [regex]::new('^\| `(.+?)` \| ([\w-]+) \|')
$splitRegex = [regex]::new('(?<!\\)\|')
foreach ($g in $groups) {
    $doc = Join-Path $DocsDir "schema-$g.md"
    if (-not (Test-Path -LiteralPath $doc)) { continue }
    $section = ''
    foreach ($line in [IO.File]::ReadLines($doc)) {
        if ($line.StartsWith('### 파일 종류: ')) { $section = "K`t" + $line.Substring(11); continue }
        if ($line.StartsWith('### 유닛 종류: ')) { $section = "R`t" + $line.Substring(11); continue }
        $m = $rowRegex.Match($line)
        if (-not $m.Success -or -not $section) { continue }
        $path = $m.Groups[1].Value -replace '\\\|', '|'
        [void]$actual.Add("$g`t$section`t$path`t$($m.Groups[2].Value)")
        $cells = $splitRegex.Split($line)
        if ($cells.Count -ge 9) { $grade = $cells[7].Trim(); $grades[$grade] = 1 + [int]$grades[$grade] }
    }
}
$missing = @($expected | Where-Object { -not $actual.Contains($_) })
$extra = @($actual | Where-Object { -not $expected.Contains($_) })

# 미해결 요약
$summary = @(Import-Csv -LiteralPath (Join-Path $DataDir 'resolve-summary.csv'))
$unresolvedFiles = @($summary | Where-Object { [int]$_.UnresolvedCount -gt 0 })
$unresolvedTop = $unresolvedFiles | ForEach-Object { $_.Unresolved -split ' ; ' } | Group-Object | Sort-Object Count -Descending

$sb = [Text.StringBuilder]::new()
function L([string]$s = '') { [void]$sb.AppendLine($s) }
L '# 누락 검증 결과'
L
L "생성: $(Get-Date -Format 'yyyy-MM-dd HH:mm'). 이 파일은 ``tools/Test-Coverage.ps1``이 생성한다."
L
L '## 요약'
L
L '| 검사 | 미해결 | 기준 |'
L '|---|---|---|'
L "| 1. 파일 | $($badFiles.Count) | 모든 파일이 파싱됨 또는 제외됨(사유 있음) |"
L "| 1b. pak 항목 수 | $($pakMismatch.Count) | pak마다 색인의 행 수가 pak의 실제 파일 수와 같음 |"
L "| 2. 왕복 | $($failedParse.Count) | 파싱한 트리를 다시 쓰면 원본 토큰과 같음. 괄호와 따옴표의 짝이 맞는지를 잡는다 |"
L "| 2b. 글자 수 | $($failedChars.Count) | 주석과 공백을 뺀 원본의 글자 수가 토큰 길이의 합과 같음. 토크나이저가 버린 글자가 있는지를 잡는다 |"
L "| 3. 키 | $($missing.Count) | 데이터의 모든 키가 스키마 문서에 행으로 있음. 문서가 데이터보다 낡았거나 손으로 고쳐졌는지를 잡는다 |"
L
L '검사 1과 3은 같은 도구가 만든 산출물끼리의 대조다. 도구 자체가 무언가를 빠뜨렸는지는 1b와 2b가 따로 확인한다.'
L
L '## 1. 파일 검사'
L
L '| 상태 | 파일 수 |'
L '|---|---|'
foreach ($s in $byStatus) { L "| $($s.Name) | $($s.Count) |" }
L
if ($badFiles.Count -gt 0) {
    L '분류되지 않았거나 충돌한 파일:'
    L
    L '| 가상 경로 | 출처 | 상태 | 사유 |'
    L '|---|---|---|---|'
    foreach ($f in $badFiles) { L "| ``$($f.VirtualPath)`` | $($f.Source) | $($f.Status) | $($f.Reason) |" }
    L
}
L '## 2. 왕복 검사'
L
L "파싱 대상 $($parse.Count)개 중 실패 $($failedParse.Count)개."
L
if ($failedParse.Count -gt 0) {
    L '| 파일 | 오류 |'
    L '|---|---|'
    foreach ($f in $failedParse) { L "| ``$($f.VirtualPath)`` | $($f.Errors -replace '\|', '/') |" }
    L
}
L '## 3. 키 검사'
L
L "데이터의 키 $($expected.Count)개, 문서의 키 $($actual.Count)개, 문서에 없는 키 $($missing.Count)개, 데이터에 없는데 문서에만 있는 키 $($extra.Count)개."
L
foreach ($m in ($missing | Select-Object -First 200)) { L "- 문서에 없음: ``$($m -replace "`t", ' / ')``" }
foreach ($m in ($extra | Select-Object -First 200)) { L "- 문서에만 있음: ``$($m -replace "`t", ' / ')``" }
L
L '## 펼치지 못한 구문'
L
L "펼친 유닛 파일 $($summary.Count)개 중 미해결 항목이 있는 파일 $($unresolvedFiles.Count)개. 이 파일들은 해당 구문이 원문 그대로 남아 있다."
L
L '| 미해결 항목 | 파일 수 |'
L '|---|---|'
foreach ($u in ($unresolvedTop | Select-Object -First 100)) { L "| ``$($u.Name)`` | $($u.Count) |" }
L
L '## 의미 등급 분포'
L
L '| 등급 | 키 수 |'
L '|---|---|'
foreach ($k in ($grades.Keys | Sort-Object)) { L "| $k | $($grades[$k]) |" }
L

$out = Join-Path $DocsDir 'coverage.md'
$body = $sb.ToString()
$manualPath = Join-Path $DocsDir 'coverage-notes.md'
if (Test-Path -LiteralPath $manualPath) { $body += "`n" + (Get-Content -Raw -LiteralPath $manualPath) }
[IO.File]::WriteAllText($out, $body, [Text.UTF8Encoding]::new($false))
foreach ($f in $failedChars) { Write-Host "char-check failed: $($f.VirtualPath)" }
foreach ($p in $pakMismatch) { Write-Host "pak count mismatch: $($p.Pak) pak=$($p.Files) index=$($rowsBySource[$p.Pak])" }
Write-Host "files-unresolved=$($badFiles.Count) pak-mismatch=$($pakMismatch.Count) roundtrip-failed=$($failedParse.Count) chars-failed=$($failedChars.Count) keys-missing=$($missing.Count) keys-extra=$($extra.Count)"
if ($badFiles.Count -gt 0 -or $missing.Count -gt 0 -or $failedChars.Count -gt 0 -or $pakMismatch.Count -gt 0) { exit 1 }
