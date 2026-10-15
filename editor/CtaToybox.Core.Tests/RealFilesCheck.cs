// 실제 게임 파일로 하는 검증. 게임이 설치된 PC에서 "-- real"로만 돈다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class RealFilesCheck
{
    static readonly string[] Prefixes = { "set/breed/", "set/stuff/", "properties/", "entity/-vehicle/", "entity/humanskin/", "entity/inventory/" };
    static readonly HashSet<string> Extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".def", ".set", ".ext", ".inc", ".weapon", ".ammo", ".grenade", ".mine", ".pattern", ".presets", ".hand", ".fsm", "" };

    public static void Run(string settingsPath)
    {
        var settings = EditorSettings.Load(settingsPath);
        var plan = GameProfile.PlanLayers(settings, GameProfile.ReadModEntries(File.ReadAllText(settings.OptionsSetPath)));
        using var stack = new SourceStack(plan.Layers);
        stack.Scan();

        int files = 0, unparsed = 0, noValues = 0, codecMismatch = 0, patchMismatch = 0;
        var examples = new List<string>();
        foreach (string vp in stack.Paths.OrderBy(p => p, StringComparer.Ordinal))
        {
            if (!Prefixes.Any(p => vp.StartsWith(p, StringComparison.Ordinal)) || !Extensions.Contains(Path.GetExtension(vp))) continue;
            files++;
            byte[] original = stack.Read(stack.Find(vp));
            string text = TextCodec.Decode(original, out TextKind kind);
            if (!original.AsSpan().SequenceEqual(TextCodec.Encode(text, kind))) { codecMismatch++; if (examples.Count < 10) examples.Add("codec: " + vp); continue; }

            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0) { unparsed++; continue; }
            var rows = KeyLocator.Rows(parsed.Root).Where(r => r.Leaves.Count > 0).ToList();
            if (rows.Count == 0) { noValues++; continue; }
            var edits = new List<Edit>();
            foreach (var row in new[] { rows[0], rows[rows.Count - 1] }.Distinct())
            {
                int arg = row.Leaves.Count - 1;
                edits.Add(new Edit { Unit = vp, File = vp, Path = row.Path, Occurrence = row.Occurrence, Arg = arg, Old = row.Leaves[arg].Text, New = row.Leaves[arg].Text });
            }
            var results = new List<EditResult>();
            string patched;
            try { patched = TokenPatcher.Apply(text, edits, results); }
            catch (Exception ex) { patchMismatch++; if (examples.Count < 10) examples.Add("throw: " + vp + " " + ex.Message); continue; }
            bool ok = results.All(r => r.Status == EditStatus.Applied) && original.AsSpan().SequenceEqual(TextCodec.Encode(patched, kind));
            if (!ok) { patchMismatch++; if (examples.Count < 10) examples.Add("patch: " + vp + " " + string.Join(",", results.Select(r => r.Status))); }
        }

        Console.WriteLine("layers=" + string.Join(",", plan.Layers.Select(l => l.Name)));
        Console.WriteLine("files=" + files + " unparsed=" + unparsed + " no-values=" + noValues + " codec-mismatch=" + codecMismatch + " patch-mismatch=" + patchMismatch);
        foreach (string e in examples) Console.WriteLine("  " + e);
        T.Eq(0, codecMismatch, "every file re-encodes to its original bytes");
        T.Eq(0, patchMismatch, "an identity edit leaves every file byte-identical");
        T.True(files > 1000, "the unit scope was actually scanned");

        // 유닛 색인: 실제 원본으로 만들어 지고, 걸리는 시간을 적는다
        var clock = System.Diagnostics.Stopwatch.StartNew();
        var index = UnitIndex.Build(stack, null);
        Console.WriteLine("units=" + index.Units.Count + " index-seconds=" + clock.Elapsed.TotalSeconds.ToString("0.0") +
                          " kinds=" + string.Join(",", index.Units.GroupBy(u => u.Kind).OrderBy(g => g.Key).Select(g => g.Key + ":" + g.Count())));
        T.True(index.Units.Count > 10000, "the index holds the game's units");

        // 출처 추적: 차량 전체에서, 편집 가능하다고 한 자리의 값이 실제로 그 파일 그 자리의 토큰과 같은지 본다
        var tracer = new UnitTracer(stack);
        var parsedFiles = new Dictionary<string, SdlNode>(StringComparer.Ordinal);
        int traced = 0, values = 0, editable = 0, wrongSlot = 0;
        foreach (var u in index.Units.Where(x => x.Kind == "vehicle"))
        {
            var t = tracer.Trace(u.Path);
            traced++;
            foreach (var row in t.Rows)
                foreach (var v in row.Values)
                {
                    values++;
                    if (v.Arg < 0) continue;
                    editable++;
                    SdlNode root;
                    if (!parsedFiles.TryGetValue(v.File, out root)) { root = SdlCore.Parse(stack.ReadText(v.File)).Root; parsedFiles[v.File] = root; }
                    var node = KeyLocator.Find(root, v.KeyPath, v.Occurrence, v.Arg);
                    if (node == null || node.Text != v.Text) { wrongSlot++; if (examples.Count < 20) examples.Add("slot: " + u.Path + " " + row.Path + " ← " + v.File + " " + v.KeyPath); }
                }
        }
        Console.WriteLine("traced-vehicles=" + traced + " values=" + values + " editable=" + editable + " wrong-slot=" + wrongSlot);
        foreach (string e in examples.Where(x => x.StartsWith("slot: "))) Console.WriteLine("  " + e);
        T.Eq(0, wrongSlot, "every editable trace value names the slot that holds that token");
        T.True(editable > 100000, "vehicles were actually traced");

        // 계획의 근거로 쓴 사실이 지금 원본에서도 맞는지 본다
        const string m3a1 = "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def", m3a1Inc = "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc";
        var halftrack = tracer.Trace(m3a1);
        var mine = halftrack == null ? null : halftrack.Rows.SelectMany(r => r.Values).FirstOrDefault(v => v.Arg >= 0 && v.File == m3a1Inc && v.KeyPath.EndsWith("/box/item") && v.Text == "24");
        Console.WriteLine("m3a1-mine=" + (mine == null ? "not found" : mine.File + " " + mine.KeyPath + " [" + mine.Occurrence + "] 값 " + mine.Arg) + " users=" + index.UsersOf(m3a1Inc).Count);
        T.True(mine != null, "the in-game check candidate (M3A1 halftrack, mine m1a1 24) is where the plan says");
        T.True(index.UsersOf(m3a1Inc).Count >= 2, "and its include file is shared by more than one unit");

        // 아이템 이름 규칙: 데이터 조사(docs/02-modding-research.md)와 같은 범위에서 몇 개가 파일 하나로 이어지는지
        var items = ItemLinks.Build(stack.Paths);
        var itemNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var u in index.Units.Where(x => x.Kind == "vehicle" || x.Kind == "breed"))
        {
            var parsedUnit = SdlCore.Parse(stack.ReadText(u.Path));
            if (parsedUnit.Errors.Count == 0) foreach (string n in ItemLinks.ReferencedNames(parsedUnit.Root.Children)) itemNames.Add(n);
        }
        var byRule = itemNames.Select(items.Find).GroupBy(l => l.Rule).ToDictionary(g => g.Key, g => g.Count());
        Console.WriteLine("item-names=" + itemNames.Count + " " + string.Join(" ", byRule.OrderBy(k => k.Key).Select(k => k.Key + "=" + k.Value)));
        int linked = new[] { ItemLinks.RuleName, ItemLinks.RuleWords, ItemLinks.RuleWordsNoExt }.Sum(r => byRule.TryGetValue(r, out int c) ? c : 0);
        T.True(linked * 100 >= itemNames.Count * 95, "at least 95% of item names link to exactly one file, as the survey found");
        T.Eq("set/stuff/smg/m3_grease", items.Find("m3_grease").Files.Single(), "the in-game-confirmed link");

        // 게임 설정(하위 프로젝트 B): 맨 위 파일이 모두 읽히고, 같은 값으로 고쳐도 바이트가 같고, 문자열 속 설정은 나눴다 이으면 같다
        int settingFiles = 0, settingUnread = 0, luaRows = 0, luaMismatch = 0, pieceStrings = 0, pieceMismatch = 0, settingWrongSlot = 0;
        var settingExamples = new List<string>();
        foreach (string vp in stack.Paths.Where(SettingFiles.IsTop).OrderBy(p => p, StringComparer.Ordinal))
        {
            settingFiles++;
            string text = stack.ReadText(vp);
            string error;
            var rows = SourceRows.Of(vp, text, out error);
            if (rows == null) { settingUnread++; settingExamples.Add("unread: " + vp + " " + error); continue; }
            if (!LuaTable.IsLua(vp)) continue;
            var edits = rows.Where(r => r.Leaves[0].Text != "nil")
                .Select(r => new Edit { Unit = vp, File = vp, Path = r.Path, Occurrence = r.Occurrence, Arg = 0, Old = r.Leaves[0].Text, New = r.Leaves[0].Text }).ToList();
            luaRows += edits.Count;
            var results = new List<EditResult>();
            string patched;
            try { patched = LuaPatcher.Apply(text, edits, results); }
            catch (Exception ex) { luaMismatch++; settingExamples.Add("lua throw: " + vp + " " + ex.Message); continue; }
            if (patched != text || results.Any(r => r.Status != EditStatus.Applied)) { luaMismatch++; settingExamples.Add("lua patch: " + vp); }
        }
        foreach (string vp in stack.Paths.Where(p => SettingFiles.InArea(p) && !LuaTable.IsLua(p) && (p.EndsWith(".set") || p.EndsWith(".inc") || p.EndsWith(".dl"))))
        {
            string error;
            var rows = SourceRows.Of(vp, stack.ReadText(vp), out error);
            if (rows == null) continue;
            foreach (var leaf in rows.SelectMany(r => r.Leaves))
            {
                var pieces = SettingStrings.Split(leaf.Text);
                if (pieces == null) continue;
                pieceStrings++;
                if (SettingStrings.Join(leaf.Text, pieces.Select(x => x.Value).ToList()) != leaf.Text) { pieceMismatch++; settingExamples.Add("pieces: " + vp + " " + leaf.Text); }
            }
        }

        // 요약 키가 실제 줄에 맞는지, 고칠 수 있다고 한 자리가 그 파일 그 자리의 글자인지(화면과 같은 세션으로 연다)
        var summaryKeys = SummaryKeys.Load(Path.Combine("editor", "summary"), SettingFiles.Kind, new KeyAnnotations());
        var matched = new HashSet<string>(StringComparer.Ordinal);
        using (var session = EditorSession.Open(settingsPath, Directory.GetCurrentDirectory(), null))
        {
            foreach (var u in session.Index.Units.Where(x => x.EditorKind == SettingFiles.Kind))
            {
                var view = session.OpenUnit(u.Path);
                foreach (var r in view.Rows)
                {
                    string k = summaryKeys.KeyOf(r.Path, r.RawPath);
                    if (k != null) matched.Add(k);
                    foreach (var v in r.Values.Where(x => x.Editable))
                    {
                        string error;
                        var fileRows = SourceRows.Of(v.Trace.File, stack.ReadText(v.Trace.File), out error);
                        var row = fileRows == null ? null : fileRows.FirstOrDefault(x => x.Occurrence == v.Trace.Occurrence && x.Path == v.Trace.KeyPath);
                        if (row == null || v.Trace.Arg >= row.Leaves.Count || row.Leaves[v.Trace.Arg].Text != v.Trace.Text)
                        { settingWrongSlot++; if (settingExamples.Count < 30) settingExamples.Add("slot: " + u.Path + " " + r.Path + " ← " + v.Trace.File + " " + v.Trace.KeyPath); }
                    }
                }
            }
        }
        var missingKeys = summaryKeys.Keys.Where(k => !matched.Contains(k)).ToList();
        Console.WriteLine("setting-files=" + settingFiles + " unread=" + settingUnread + " lua-rows=" + luaRows + " lua-mismatch=" + luaMismatch +
                          " piece-strings=" + pieceStrings + " piece-mismatch=" + pieceMismatch + " setting-wrong-slot=" + settingWrongSlot + " summary-keys-missing=" + missingKeys.Count);
        foreach (string e in settingExamples.Take(30)) Console.WriteLine("  " + e);
        foreach (string k in missingKeys) Console.WriteLine("  summary key with no row: " + k);
        T.True(settingFiles >= 22, "the game settings files were found");
        T.Eq(0, settingUnread, "every game settings file reads");
        T.True(luaRows > 300, "the conquest AI config was read");
        T.Eq(0, luaMismatch, "an identity Lua edit leaves the file byte-identical");
        T.True(pieceStrings >= 10, "the embedded setting strings were found");
        T.Eq(0, pieceMismatch, "every embedded setting string splits and joins back to its bytes");
        T.Eq(0, settingWrongSlot, "every editable settings value names the slot that holds that token");
        T.Eq(0, missingKeys.Count, "every settings summary key matches a real row");

        // 차량·보병·무기·탄약의 요약 키도 실제 줄에 맞는지(Q13). 열 개마다 하나씩 먼저 열고, 못 찾은 키가 남을 때만 나머지를 연다
        int unitsOpened = 0, unlabeled = 0;
        var unitKeysMissing = new List<string>();
        var unlabeledExamples = new List<string>();
        using (var session = EditorSession.Open(settingsPath, Directory.GetCurrentDirectory(), null))
        {
            foreach (string kind in new[] { "vehicle", "breed", "stuff" })
            {
                var keys = SummaryKeys.Load(Path.Combine("editor", "summary"), kind, new KeyAnnotations());
                var left = new HashSet<string>(keys.Keys, StringComparer.Ordinal);
                var units = session.Index.Units.Where(u => u.Kind == kind && UnitKind.InEditor(u.EditorKind) && u.Numbers > 0).OrderBy(u => u.Path, StringComparer.Ordinal).ToList();
                foreach (var u in units.Where((x, i) => i % 10 == 0).Concat(units.Where((x, i) => i % 10 != 0)))
                {
                    if (left.Count == 0) break;
                    var view = session.OpenUnit(u.Path);
                    unitsOpened++;
                    foreach (var r in view.Rows) { string k = keys.KeyOf(r.Path, r.RawPath); if (k != null) left.Remove(k); }
                }
                unitKeysMissing.AddRange(left.Select(k => kind + " " + k));
            }
            // 게임 설정의 값 이름(D3): 고장 부품 비용과 수비 단계의 열림 조건은 값마다 이름이 있다
            foreach (var u in session.Index.Units.Where(x => x.EditorKind == SettingFiles.Kind))
                foreach (var r in session.OpenUnit(u.Path).Rows.Where(x => x.Path.Contains("/brokencomponent/") || x.Path.EndsWith("/unlock")))
                    foreach (var v in r.Values.Where(x => x.Label == null)) { unlabeled++; if (unlabeledExamples.Count < 10) unlabeledExamples.Add(u.Path + " " + r.Path + " " + v.Trace.Text); }
        }
        Console.WriteLine("summary-units-opened=" + unitsOpened + " unit-summary-keys-missing=" + unitKeysMissing.Count + " setting-values-unlabeled=" + unlabeled);
        foreach (string k in unitKeysMissing) Console.WriteLine("  unit summary key with no row: " + k);
        foreach (string e in unlabeledExamples) Console.WriteLine("  unlabeled: " + e);
        T.Eq(0, unitKeysMissing.Count, "every vehicle, infantry and weapon summary key matches a real row (Q13)");
        T.Eq(0, unlabeled, "broken-component costs and defense-level unlocks have value names (D3)");
    }
}
