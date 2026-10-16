# 조사용 스크립트. 2026-10-14 docs/02-modding-research.md의 "아이템 이름과 파일" 표를 만든 것이다.
# 유닛 파일이 이름으로 가리키는 아이템(무기·탄약 등)이 set/stuff의 어느 파일로 이어지는지, 규칙별로 맞는 수를 센다
Add-Type -Path E:\CTAToybox\editor\CtaToybox.Core\bin\Release\net8.0\CtaToybox.Core.dll
$s = [CtaToybox.Core.EditorSettings]::Load('E:\CTAToybox\editor\settings.json')
$plan = [CtaToybox.Core.GameProfile]::PlanLayers($s, [CtaToybox.Core.GameProfile]::ReadModEntries([IO.File]::ReadAllText($s.OptionsSetPath)))
$stack = [CtaToybox.Core.SourceStack]::new($plan.Layers); $stack.Scan()
$paths = @($stack.Paths)
# stuff 파일: 잎 이름을 점으로 나눈 낱말들
$exact = @{}; $bag = @{}
foreach ($p in $paths) {
  if (-not $p.StartsWith('set/stuff/') -or $p.StartsWith('set/stuff/_settings/')) { continue }
  $leaf = $p.Substring($p.LastIndexOf('/') + 1)
  if ($leaf.StartsWith('.') -or $leaf -match '\.(pattern|inc|fsm|hand|presets)$') { continue }
  $words = $leaf.Split('.')
  $exact[$leaf] += @($p)
  $key = ($words | Sort-Object) -join ' '; $bag[$key] += @($p)
  if ($words.Count -gt 1) { $key2 = ($words[0..($words.Count-2)] | Sort-Object) -join ' '; $bag["~" + $key2] += @($p) }
}
"stuff files: $($exact.Count) leaf names"
$refs = @{}
foreach ($p in $paths) {
  $kind = [CtaToybox.Core.UnitKind]::Of($p); if ($kind -ne 'vehicle' -and $kind -ne 'breed') { continue }
  $parsed = [SdlCore]::Parse($stack.ReadText($p)); if ($parsed.Errors.Count) { continue }
  foreach ($row in [SdlCore]::KeyRows($parsed.Root.Children, [string[]]@('extender'))) {
    $seg = $row.Path.Substring($row.Path.LastIndexOf('/') + 1)
    $nodes = @($row.ArgNodes)
    for ($i = 0; $i -lt $nodes.Count; $i++) {
      $n = $nodes[$i]
      $take = ($n.Kind -eq 'str') -and (($i -eq 0 -and $seg -in 'item','weapon') -or ($i -gt 0 -and $nodes[$i-1].Kind -eq 'atom' -and $nodes[$i-1].Text -eq 'filling'))
      if ($take) { $name = $n.Text.Trim('"').ToLowerInvariant(); $refs["$seg|$name"] = 1 + $refs["$seg|$name"] }
    }
  }
}
$names = @{}; foreach ($k in $refs.Keys) { $names[$k.Split('|',2)[1]] = 1 }
"distinct referenced names: $($names.Count)  (item/weapon/filling references: $($refs.Values | Measure-Object -Sum | % Sum))"
$r = @{ exact = 0; bag = 0; bagNoExt = 0; none = 0; ambiguous = 0 }; $miss = @()
foreach ($nm in $names.Keys) {
  $w = $nm.Split(' ', [StringSplitOptions]::RemoveEmptyEntries); $key = ($w | Sort-Object) -join ' '
  if ($exact.ContainsKey($nm) -and $exact[$nm].Count -eq 1) { $r.exact++; continue }
  if ($bag.ContainsKey($key)) { if ($bag[$key].Count -gt 1) { $r.ambiguous++ } else { $r.bag++ }; continue }
  if ($bag.ContainsKey("~$key")) { if ($bag["~$key"].Count -gt 1) { $r.ambiguous++ } else { $r.bagNoExt++ }; continue }
  $r.none++; $miss += $nm
}
"exact-leaf=$($r.exact) same-words=$($r.bag) same-words-without-extension=$($r.bagNoExt) ambiguous=$($r.ambiguous) unmatched=$($r.none)"
"unmatched examples:"; $miss | Sort-Object | Select-Object -First 40 | % { "  $_" }
$stack.Dispose()
# 맞지 않은 이름: 이름의 낱말이 파일 낱말의 부분집합인 파일이 몇 개인지
$leafWords = @{}; foreach ($l in $exact.Keys) { $leafWords[$l] = [Collections.Generic.HashSet[string]]::new([string[]]$l.Split('.')) }
$sub = @{ one = 0; many = 0; zero = 0 }; $lines = @()
foreach ($nm in $miss) {
  $w = $nm.Split(' ', [StringSplitOptions]::RemoveEmptyEntries)
  $hits = @($leafWords.Keys | Where-Object { $set = $leafWords[$_]; -not ($w | Where-Object { -not $set.Contains($_) }) })
  if ($hits.Count -eq 1) { $sub.one++ } elseif ($hits.Count -gt 1) { $sub.many++ } else { $sub.zero++ }
  $lines += "  $nm -> $($hits.Count): $(($hits | Select-Object -First 3) -join ', ')"
}
"subset rule on unmatched: one=$($sub.one) many=$($sub.many) zero=$($sub.zero)"; $lines
