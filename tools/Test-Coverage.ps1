param(
    [Parameter(Mandatory)][string]$DataDir,
    [Parameter(Mandatory)][string]$DocsDir
)
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$groups = 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared'
$allowed = 'text', 'reference-only', 'excluded-scope', 'excluded-binary', 'empty', 'duplicate'

# 검사 1: 파일
$index = @(Import-Csv -LiteralPath (Join-Path $DataDir 'file-index.csv'))
$byStatus = $index | Group-Object Status | Sort-Object Name
$badFiles = @($index | Where-Object { $_.Status -notin $allowed })

# 검사 2: 왕복
$parse = @(Import-Csv -LiteralPath (Join-Path $DataDir 'parse-results.csv'))
$failedParse = @($parse | Where-Object Ok -ne 'True')

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
L "| 2. 왕복 | $($failedParse.Count) | 파싱한 트리를 다시 쓰면 원본 토큰과 같음 |"
L "| 3. 키 | $($missing.Count) | 데이터의 모든 키가 스키마 문서에 행으로 있음 |"
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
Write-Host "files-unresolved=$($badFiles.Count) roundtrip-failed=$($failedParse.Count) keys-missing=$($missing.Count) keys-extra=$($extra.Count)"
if ($badFiles.Count -gt 0 -or $missing.Count -gt 0) { exit 1 }
