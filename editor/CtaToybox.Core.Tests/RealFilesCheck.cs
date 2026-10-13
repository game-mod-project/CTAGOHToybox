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
    }
}
