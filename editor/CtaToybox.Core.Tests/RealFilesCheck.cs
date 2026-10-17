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
        var parsedFiles = new Dictionary<string, KeyIndex>(StringComparer.Ordinal);   // 파일마다 행 색인 하나(값마다 행 목록을 만들면 40분 걸렸다)
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
                    KeyIndex root;
                    if (!parsedFiles.TryGetValue(v.File, out root)) { root = new KeyIndex(SdlCore.Parse(stack.ReadText(v.File)).Root); parsedFiles[v.File] = root; }
                    var node = root.Find(v.KeyPath, v.Occurrence, v.Arg);
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
        var family = items.Find("ammo mgun_ger");
        T.Eq(ItemLinks.RuleFirstByName + " set/stuff/mgun/mgun_ger.belt.ammo", family.Rule + " " + family.Files[0], "the family name goes to the file the game picked (G5)");

        // 원형 상속(E11): 아이템 파일의 {from} 이름이 모두 파일로 이어지고, 물려받은 값의 자리가 그 파일 그 자리의 글자인지
        int fromUnits = 0, fromProblems = 0, inherited = 0, fromWrongSlot = 0, appendable = 0, appendBad = 0;
        var fromStates = new Dictionary<string, int>(StringComparer.Ordinal);
        var fromExamples = new List<string>();
        foreach (var u in index.Units.Where(x => x.Kind == "stuff" || x.Kind == "pattern"))
        {
            var t = tracer.Trace(u.Path);
            var problems = t.Unresolved.Where(x => x.StartsWith("from-")).ToList();
            if (t.From.Count == 0 && problems.Count == 0) continue;
            fromUnits++;
            fromProblems += problems.Count;
            foreach (string p in problems) if (fromExamples.Count < 20) fromExamples.Add(u.Path + " " + p);
            foreach (var row in t.Rows.Where(r => r.From != null))
            {
                fromStates[row.FromState] = (fromStates.TryGetValue(row.FromState, out int n) ? n : 0) + 1;
                // E14: '이 유닛만'으로 덧붙일 수 있는 줄은 덧붙일 줄이 한 줄로 다시 읽히고 값이 같아야 한다
                if (UnitScope.InheritedBlocker(t, row) == null)
                {
                    appendable++;
                    string line = UnitScope.AppendLine(row, new Edit[0]);
                    var back = KeyLocator.Rows(SdlCore.Parse("{from " + line + "}").Root).Where(r => r.Path != "/from").ToList();
                    if (back.Count != 1 || back[0].Path != row.Path || !back[0].Leaves.Select(l => l.Text).SequenceEqual(row.Values.Select(v => v.Text)))
                    { appendBad++; if (fromExamples.Count < 20) fromExamples.Add("append: " + u.Path + " " + line); }
                }
                foreach (var v in row.Values.Where(x => x.Arg >= 0))
                {
                    inherited++;
                    KeyIndex root;
                    if (!parsedFiles.TryGetValue(v.File, out root)) { root = new KeyIndex(SdlCore.Parse(stack.ReadText(v.File)).Root); parsedFiles[v.File] = root; }
                    var node = root.Find(v.KeyPath, v.Occurrence, v.Arg);
                    if (node == null || node.Text != v.Text) { fromWrongSlot++; if (fromExamples.Count < 20) fromExamples.Add("slot: " + u.Path + " " + row.Path + " ← " + v.File + " " + v.KeyPath); }
                }
            }
        }
        // E4: 병종 파일의 인자 없는 매크로 호출마다 같은 갈래의 다른 이름으로 바꿔 보면 적용되고 다시 읽혀야 한다
        int swapCalls = 0, swapChecked = 0, swapBad = 0;
        var swapExamples = new List<string>();
        foreach (var u in index.Units.Where(x => x.Kind == "breed"))
        {
            string text = stack.ReadText(u.Path);
            if (text == null) continue;
            var parsedBreed = SdlCore.Parse(text);
            if (parsedBreed.Errors.Count > 0) continue;
            var calls = KeyLocator.Rows(parsedBreed.Root).Where(r => r.Form == "macro" && r.Leaves.Count == 0 && r.Head != null && !r.Path.Contains("(define ") && MacroSwap.NameOf(r.RawPath) != null).ToList();
            if (calls.Count == 0) continue;
            var defines = tracer.Trace(u.Path).Defines;
            foreach (var c in calls)
            {
                swapCalls++;
                string name = MacroSwap.NameOf(c.RawPath);
                string other = MacroSwap.Choices(defines, name).FirstOrDefault(x => MacroSwap.SameFamily(x, name) && !string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
                if (other == null) continue;
                swapChecked++;
                var e = new Edit { Unit = u.Path, File = u.Path, Path = c.Path, Occurrence = c.Occurrence, Arg = 0, Old = c.Head.Text, New = "\"" + other + "\"", Kind = MacroSwap.Kind };
                var results = new List<EditResult>();
                string error = null;
                try { TokenPatcher.Apply(text, new[] { e }, results); } catch (InvalidOperationException ex) { error = ex.Message; }
                if (error != null || results.Count != 1 || results[0].Status != EditStatus.Applied)
                { swapBad++; if (swapExamples.Count < 10) swapExamples.Add(u.Path + " " + c.Path + " → " + other + ": " + (error ?? (results.Count == 0 ? "no result" : results[0].Message))); }
            }
        }
        Console.WriteLine("macro-calls=" + swapCalls + " macro-swap-checked=" + swapChecked + " macro-swap-bad=" + swapBad);
        foreach (string e in swapExamples) Console.WriteLine("  " + e);
        T.True(swapChecked > 1000, "argument-less calls in breed files were swapped (E4)");
        T.Eq(0, swapBad, "every same-family swap applies and reads back (E4)");

        // E18: 월드워-차종 프로젝트를 실제 판에서 만들어 임시 출력 폴더에 빌드한다
        var classNotes = new List<string>();
        var classProject = VisionPresets.Build(stack, "wot", 4, classNotes);
        var classReport = VisionPresets.AddClasses(classProject, stack, index.Units, tracer, u => index.HeirsOf(u).Count, new ClassVision(), classNotes);
        string classOut = Path.Combine(Path.GetTempPath(), "ctatoybox-e18-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(classOut);
        try
        {
            var classBuild = new ModBuilder(stack, classOut).Build(classProject);
            var notApplied = classBuild.Results.Where(r => r.Status != EditStatus.Applied).ToList();
            string fieldsOut = File.ReadAllText(Path.Combine(classOut, "resource", "set", "vision", "vision_fields.inc"));
            string fieldsError;
            var fieldRows = SourceRows.Of(VisionPresets.FieldsFile, fieldsOut, out fieldsError);
            int copiedNames = new[] { VisionPresets.MediumField, VisionPresets.LightField, VisionPresets.VehicleField }.Count(n => fieldRows != null && fieldRows.Count(r => r.RawPath == "/\"" + n + "\"") == 1);
            int named = Directory.EnumerateFiles(Path.Combine(classOut, "resource"), "*", SearchOption.AllDirectories).Count(f => { string t = File.ReadAllText(f); return t.Contains("\"" + VisionPresets.LightField + "\"") || t.Contains("\"" + VisionPresets.MediumField + "\"") || t.Contains("\"" + VisionPresets.VehicleField + "\""); });
            Console.WriteLine("class-units-light=" + classReport.Light + " class-units-medium=" + classReport.Medium + " class-vehicle-files=" + classReport.VehicleFiles + " class-skipped=" + classReport.Skipped.Count +
                              " class-edits=" + classProject.Edits.Count + " class-not-applied=" + notApplied.Count + " class-file-errors=" + classBuild.FileErrors.Count + " class-files-naming-fields=" + named);
            foreach (var r in notApplied.Take(10)) Console.WriteLine("  class: " + r.Status + " " + r.Edit.Unit + " " + r.Edit.Path + " " + r.Message);
            foreach (string e in classBuild.FileErrors.Take(10)) Console.WriteLine("  class file: " + e);
            T.True(classReport.Light > 100 && classReport.Medium > 100, "light and medium tanks were found by folder (E18)");
            T.Eq(0, notApplied.Count, "every class edit applies (E18)");
            T.Eq(0, classBuild.FileErrors.Count, "and every file is written (E18)");
            T.Eq(3, copiedNames, "the three class fields are in the output once each (E18)");
        }
        finally { Directory.Delete(classOut, true); }

        // F1: 탱크 전부의 최고 속도 ×1.1 규칙을 실제 판에서 미리 보고 임시 출력 폴더에 빌드한다
        var speedRule = new BulkRule { Id = "realrule", Name = "tank speed", Kind = "tank", Key = "**/locomotion/maxspeed", Arg = 0, Op = "mul", Value = "1.1" };
        var ruleProject = new EditProject { Name = "f1-real", Rules = new List<BulkRule> { speedRule } };
        var rulePreview = BulkRules.Preview(speedRule, ruleProject, index.Units, tracer, f => index.UsersOf(f), u => index.HeirsOf(u).Count);
        ruleProject.Edits.AddRange(rulePreview.Edits);
        string ruleOut = Path.Combine(Path.GetTempPath(), "ctatoybox-f1-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(ruleOut);
        try
        {
            var ruleBuild = new ModBuilder(stack, ruleOut).Build(ruleProject);
            var ruleBad = ruleBuild.Results.Where(r => r.Status != EditStatus.Applied).ToList();
            var reasons = rulePreview.Skipped.GroupBy(k => k.Reason.Length > 40 ? k.Reason.Substring(0, 40) : k.Reason).OrderByDescending(g => g.Count()).Take(5);
            Console.WriteLine("rule-targets=" + rulePreview.Targets + " rule-units=" + rulePreview.Units + " rule-changes=" + rulePreview.Changes.Count + " rule-edits=" + rulePreview.Edits.Count +
                              " rule-outside=" + rulePreview.OutsideUnits + " rule-skipped=" + rulePreview.Skipped.Count + " rule-not-applied=" + ruleBad.Count + " rule-file-errors=" + ruleBuild.FileErrors.Count);
            foreach (var g in reasons) Console.WriteLine("  rule skip " + g.Count() + ": " + g.Key);
            foreach (var r in ruleBad.Take(10)) Console.WriteLine("  rule: " + r.Status + " " + r.Edit.Unit + " " + r.Edit.Path + " " + r.Message);
            T.True(rulePreview.Units > 100, "the speed rule changes more than a hundred tanks (F1)");
            T.Eq(0, ruleBad.Count, "every rule edit applies (F1)");
            T.Eq(0, ruleBuild.FileErrors.Count, "and every file is written (F1)");
            T.Eq(rulePreview.Targets, rulePreview.Units + rulePreview.Skipped.Select(k => k.Unit).Except(rulePreview.Changes.Select(c => c.Unit)).Distinct().Count(), "every target either changes or is skipped with a reason (F1)");
        }
        finally { Directory.Delete(ruleOut, true); }

        // 팩션 강화: 독일 프로젝트(월드워-차종 베이스)를 실제 판에서 만들어 임시 폴더에 빌드하고, 다른 군대 유닛의 펼친 값이 그대로인지 본다
        var armies = ArmyIndex.Build(stack, index.Units);
        var fc = new FactionContext { Stack = stack, Index = index, Tracer = new UnitTracer(stack), Armies = armies, Items = ItemLinks.Build(stack.Paths) };
        string basePath = Path.Combine(T.RepoRoot(), "editor", "projects", "월드워-차종.json");
        var boostReport = new FactionReport();
        var boost = FactionPreset.Build(fc, EditProject.Load(basePath), new FactionOptions { Army = "ger", SuperHeavy = FactionPreset.LoadSuperHeavy(Path.Combine(T.RepoRoot(), "editor", "presets", "super-heavy.txt")) }, new ConquestValues(), boostReport);
        string boostOut = Path.Combine(Path.GetTempPath(), "ctatoybox-boost-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(boostOut);
        try
        {
            var boostBuild = new ModBuilder(stack, boostOut).Build(boost);
            var boostBad = boostBuild.Results.Where(r => r.Status != EditStatus.Applied).ToList();
            // 빌드 출력을 맨 뒤 계층으로 붙인 판에서 다른 군대 유닛을 다시 펼친다. 비교 기준은 베이스(월드워-차종)만 빌드한 판이다(시야는 모든 군대에 바뀐다)
            stack.SetCopies(null);
            string baseOut = Path.Combine(Path.GetTempPath(), "ctatoybox-boostbase-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(baseOut);
            new ModBuilder(stack, baseOut).Build(EditProject.Load(basePath));
            stack.SetCopies(null);
            using var after = new SourceStack(plan.Layers.Concat(new[] { new SourceLayer("boost", Path.Combine(boostOut, "resource")) }));
            after.Scan();
            using var before = new SourceStack(plan.Layers.Concat(new[] { new SourceLayer("base", Path.Combine(baseOut, "resource")) }));
            before.Scan();
            var afterTracer = new UnitTracer(after);
            var beforeTracer = new UnitTracer(before);
            var copiedNames = new HashSet<string>(boost.Edits.Where(e => e.Kind == FileCopy.Kind).Select(e => e.File.Substring(e.File.LastIndexOf('/') + 1).Split('.')[0]), StringComparer.OrdinalIgnoreCase);
            var sample = new List<string>();
            foreach (string army in ArmyIndex.Armies.Where(a => a != "ger"))
            {
                var m = armies.Members(army);
                sample.AddRange(m.Where(p => p.StartsWith("set/breed/")).Take(20));
                sample.AddRange(m.Where(p => p.StartsWith("entity/-vehicle/")).Take(20));
                // 독일 사본의 원래 무기를 이름으로 부르는 다른 군대 유닛
                sample.AddRange(m.Where(p => p.StartsWith("set/breed/")).Where(p => { string t = stack.ReadText(p) ?? ""; return copiedNames.Any(n => t.IndexOf("\"" + n, StringComparison.OrdinalIgnoreCase) >= 0); }).Take(30));
            }
            sample.AddRange(armies.Shared.Select(x => x.Substring(0, x.IndexOf(':'))));
            sample = sample.Distinct().ToList();
            int changed = 0;
            var changedExamples = new List<string>();
            foreach (string u in sample)
            {
                var b = beforeTracer.Trace(u); var a2 = afterTracer.Trace(u);
                if (b == null || a2 == null) continue;
                string Flat(UnitTraceResult r) => string.Join("\n", r.Rows.Select(x => x.Path + "=" + string.Join(" ", x.Values.Select(v => v.Text))));
                string fb = Flat(b), fa = Flat(a2);
                if (fb != fa)
                {
                    changed++;
                    var lb = fb.Split('\n'); var la = fa.Split('\n');
                    string diff = lb.Except(la).FirstOrDefault() + " / " + la.Except(lb).FirstOrDefault();
                    if (changedExamples.Count < 10) changedExamples.Add(u + ": " + diff);
                }
            }
            Console.WriteLine("faction-units=" + boostReport.Units + " faction-edits=" + boost.Edits.Count + " faction-copies=" + boost.Edits.Count(e => e.Kind == FileCopy.Kind) + " faction-lines=" + boost.Edits.Count(e => e.Kind == AddLine.Kind) +
                              " faction-skipped=" + boostReport.Skipped.Count + " faction-shared=" + boostReport.Shared.Count + " faction-not-applied=" + boostBad.Count + " faction-file-errors=" + boostBuild.FileErrors.Count +
                              " faction-written=" + boostBuild.WrittenFiles.Count + " faction-other-army-sample=" + sample.Count + " faction-other-army-changed=" + changed);
            foreach (var r in boostBad.Take(10)) Console.WriteLine("  faction: " + r.Status + " " + r.Edit.File + " " + r.Edit.Path + " " + r.Message);
            foreach (string e in boostBuild.FileErrors.Take(10)) Console.WriteLine("  faction file: " + e);
            foreach (string e in changedExamples) Console.WriteLine("  faction other army changed: " + e);
            T.True(boostReport.Units > 1000, "the German army has its units (faction boost)");
            T.Eq(0, boostBad.Count, "every faction edit applies");
            T.Eq(0, boostBuild.FileErrors.Count, "and every file is written (faction boost)");
            T.Eq(0, changed, "other armies' units are unchanged (faction boost)");
            T.True(EditProject.Load(basePath).Edits.All(be => boost.Edits.Any(e => e.Kind == be.Kind && e.File == be.File && e.Path == be.Path && e.Occurrence == be.Occurrence && e.Arg == be.Arg && e.New == be.New)), "every base (월드워-차종) edit is in the faction project");
        }
        finally { Directory.Delete(boostOut, true); stack.SetCopies(null); foreach (string d in Directory.GetDirectories(Path.GetTempPath(), "ctatoybox-boostbase-*")) Directory.Delete(d, true); }

        const string k98 = "set/stuff/rifle/k98", riflePattern = "set/stuff/rifle/rifle.pattern";
        var k98Trace = tracer.Trace(k98);
        Console.WriteLine("from-units=" + fromUnits + " from-problems=" + fromProblems + " inherited-values=" + inherited + " from-wrong-slot=" + fromWrongSlot +
                          " states=" + string.Join(",", fromStates.OrderBy(k => k.Key).Select(k => k.Key + ":" + k.Value)) + " unit-appendable=" + appendable + " append-bad=" + appendBad +
                          " k98-chain=" + string.Join(" > ", k98Trace.From.Select(l => l.File)) + " rifle.pattern-users=" + index.UsersOf(riflePattern).Count);
        foreach (string e in fromExamples) Console.WriteLine("  " + e);
        T.True(fromUnits > 2000, "the item files that inherit were traced");
        T.Eq(0, fromProblems, "every {from} name in the game's item files leads to a file, without a cycle");
        T.Eq(0, appendBad, "every inherited row that can be unit-scoped makes a line that reads back the same (E14)");
        T.Eq(0, fromWrongSlot, "every editable inherited value names the slot that holds that token");
        T.Eq(riflePattern, k98Trace.From.Count > 0 ? k98Trace.From[0].File : null, "the K98 inherits rifle.pattern, as measured");
        T.True(index.UsersOf(riflePattern).Count > 50, "and rifle.pattern counts its inheritors as users");
        // .presets가 include한 파일(Q16): rifle/.presets의 \set\small_firearms_accuracy.inc를 읽는 유닛
        const string accuracyInc = "set/small_firearms_accuracy.inc";
        Console.WriteLine("accuracy-inc-users=" + index.UsersOf(accuracyInc).Count);
        T.True(index.UsersOf(accuracyInc).Count > 100, "a file a folder's .presets includes counts the units that read it (Q16)");

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
                        // 매크로 호출 이름 자리(E4)는 호출 줄의 이름 토큰이다
                        string at = row == null ? null : v.Trace.MacroName ? (row.Head == null ? null : row.Head.Text) : v.Trace.Arg < row.Leaves.Count ? row.Leaves[v.Trace.Arg].Text : null;
                        if (at != v.Trace.Text)
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
