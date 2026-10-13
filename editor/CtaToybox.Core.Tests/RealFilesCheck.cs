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
    }
}
