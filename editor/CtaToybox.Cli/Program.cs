// 화면 없이 코어를 쓰는 명령줄 도구. 게임 폴더는 읽기만 하고, 쓰는 곳은 설정 파일, 프로젝트 파일, 출력 모드 폴더다.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CtaToybox.Core;

namespace CtaToybox.Cli
{
    public static class CliApp
    {
        const string Usage =
            "사용법:\n" +
            "  ctatoybox init --game <폴더> --workshop <폴더> --options <파일> --out <폴더> [--name <이름>]\n" +
            "  ctatoybox sources\n" +
            "  ctatoybox units [--find <글자>] [--kind <종류>] [--layer <계층>] [--limit N] [--names <csv>]\n" +
            "  ctatoybox trace <유닛 가상 경로> [--filter <글자>]\n" +
            "  ctatoybox show <가상 경로> [--filter <글자>]\n" +
            "  ctatoybox add --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] --new <값> [--scope shared|unit] [--unit <유닛 가상 경로>]\n" +
            "  ctatoybox build --project <파일>\n" +
            "공통: --settings <파일> (기본 editor/settings.json)\n" +
            "종류: vehicle, breed, humanskin, item-entity, stuff, pattern";

        // 명령마다 받는 옵션. 여기에 없는 옵션은 사용법 오류다(오타를 조용히 버리지 않는다)
        static readonly Dictionary<string, string[]> KnownOptions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["init"] = new[] { "settings", "game", "workshop", "options", "out", "name" },
            ["sources"] = new[] { "settings" },
            ["units"] = new[] { "settings", "find", "kind", "layer", "limit", "names" },
            ["trace"] = new[] { "settings", "filter" },
            ["show"] = new[] { "settings", "filter" },
            ["add"] = new[] { "settings", "project", "file", "path", "occ", "arg", "new", "scope", "unit" },
            ["build"] = new[] { "settings", "project" },
        };

        sealed class UsageException : Exception { public UsageException(string m) : base(m) { } }

        static int Main(string[] args) { return Run(args, Console.Out); }

        public static int Run(string[] args, TextWriter output)
        {
            try
            {
                if (args.Length == 0) throw new UsageException("명령이 없다");
                var opt = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var positional = new List<string>();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].StartsWith("--"))
                    {
                        if (i + 1 >= args.Length) throw new UsageException(args[i] + " 뒤에 값이 없다");
                        opt[args[i].Substring(2)] = args[++i];
                    }
                    else positional.Add(args[i]);
                }
                string[] known;
                if (KnownOptions.TryGetValue(args[0], out known))
                    foreach (string name in opt.Keys)
                        if (!known.Contains(name, StringComparer.OrdinalIgnoreCase)) throw new UsageException("모르는 옵션: --" + name);
                string settingsPath = opt.ContainsKey("settings") ? opt["settings"] : Path.Combine("editor", "settings.json");
                switch (args[0].ToLowerInvariant())
                {
                    case "init": return Init(opt, settingsPath, output);
                    case "sources": return Sources(settingsPath, output);
                    case "units": return Units(opt, settingsPath, output);
                    case "trace": return Trace(positional, opt, settingsPath, output);
                    case "show": return Show(positional, opt, settingsPath, output);
                    case "add": return Add(opt, settingsPath, output);
                    case "build": return Build(opt, settingsPath, output);
                    default: throw new UsageException("모르는 명령: " + args[0]);
                }
            }
            catch (UsageException ex)
            {
                output.WriteLine(ex.Message);
                output.WriteLine(Usage);
                return 2;
            }
        }

        static string Need(Dictionary<string, string> opt, string name)
        {
            string v;
            if (!opt.TryGetValue(name, out v) || v.Length == 0) throw new UsageException("--" + name + "이 필요하다");
            return v;
        }

        static int Number(Dictionary<string, string> opt, string name)
        {
            string v;
            if (!opt.TryGetValue(name, out v)) return 0;
            int n;
            if (!int.TryParse(v, out n) || n < 0) throw new UsageException("--" + name + "은 0 이상의 정수여야 한다");
            return n;
        }

        static int Init(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string outDir = Path.GetFullPath(Need(opt, "out"));
            var s = new EditorSettings
            {
                GameDir = Path.GetFullPath(Need(opt, "game")),
                WorkshopDir = Path.GetFullPath(Need(opt, "workshop")),
                OptionsSetPath = Path.GetFullPath(Need(opt, "options")),
                OutputModDir = outDir,
                OutputModName = opt.ContainsKey("name") ? opt["name"] : Path.GetFileName(outDir.TrimEnd(Path.DirectorySeparatorChar))
            };
            s.Save(settingsPath);
            output.WriteLine("설정을 썼다: " + settingsPath);
            return 0;
        }

        // 설정을 읽어 원본 계층을 연다. 실패하면 null을 돌려주고 이유를 쓴다
        static SourceStack Open(string settingsPath, TextWriter output, out EditorSettings settings, out LayerPlan plan)
        {
            settings = null; plan = null;
            if (!File.Exists(settingsPath))
            {
                output.WriteLine("설정 파일이 없다: " + settingsPath + ". 먼저 init을 실행한다.");
                return null;
            }
            settings = EditorSettings.Load(settingsPath);
            if (!File.Exists(settings.OptionsSetPath))
            {
                output.WriteLine("options.set이 없다: " + settings.OptionsSetPath);
                return null;
            }
            plan = GameProfile.PlanLayers(settings, GameProfile.ReadModEntries(File.ReadAllText(settings.OptionsSetPath)));
            var stack = new SourceStack(plan.Layers);
            stack.Scan();
            return stack;
        }

        static int Sources(string settingsPath, TextWriter output)
        {
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var counts = new Dictionary<string, int>();
                foreach (string p in stack.Paths)
                    foreach (var v in stack.Versions(p)) counts[v.Layer] = (counts.TryGetValue(v.Layer, out int n) ? n : 0) + 1;
                output.WriteLine("원본 계층 (로드 순서):");
                foreach (var l in plan.Layers) output.WriteLine("  " + l.Name + "  파일 " + (counts.TryGetValue(l.Name, out int c) ? c : 0) + "  " + l.ResourceDir);
                output.WriteLine("출력 모드 " + settings.OutputModName + ": " + (plan.OutputModActive ? "게임에서 켜져 있다" : "게임에서 꺼져 있다"));
                foreach (string w in plan.Warnings) output.WriteLine("경고: " + w);
                return 0;
            }
        }

        // 유닛 색인을 캐시에서 읽거나 새로 만든다. 캐시는 설정 파일 옆의 cache 폴더에 둔다
        static UnitIndex OpenIndex(SourceStack stack, string settingsPath, TextWriter output)
        {
            string cache = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "cache", "unit-index.json");
            bool rebuilt;
            var index = UnitIndex.LoadOrBuild(stack, cache, (done, total) => output.WriteLine("유닛 색인을 만드는 중: " + done + "/" + total), out rebuilt);
            if (rebuilt) output.WriteLine("유닛 색인을 새로 만들었다: " + index.Units.Count + "개 (" + cache + ")");
            return index;
        }

        static int Units(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string kind = opt.ContainsKey("kind") ? opt["kind"].ToLowerInvariant() : null;
            if (kind != null && !UnitKind.All.Contains(kind)) throw new UsageException("--kind는 다음 가운데 하나다: " + string.Join(", ", UnitKind.All));
            int limit = opt.ContainsKey("limit") ? Number(opt, "limit") : 50;
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var index = OpenIndex(stack, settingsPath, output);
                // 이름은 게임과 켜 둔 모드의 현지화에서 읽는다(앱과 같다). --names를 주면 그 CSV를 쓴다
                var names = opt.ContainsKey("names") ? DisplayNames.Load(opt["names"]) : DisplayNames.FromGame(settings, plan);
                var found = index.Search(opt.ContainsKey("find") ? opt["find"] : null, kind, opt.ContainsKey("layer") ? opt["layer"] : null, names).ToList();
                foreach (var u in found.Take(limit))
                {
                    string name = names.OfUnit(u.Path);
                    output.WriteLine(u.Kind + "  " + u.Layer + "  " + u.Path + (name == null ? "" : "  " + name) + (u.Unresolved > 0 ? "  (풀리지 않은 구문 " + u.Unresolved + ")" : ""));
                }
                output.WriteLine(found.Count > limit ? found.Count + "개 가운데 " + limit + "개를 보였다. --limit로 늘린다." : found.Count + "개");
                return 0;
            }
        }

        static int Trace(List<string> positional, Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            if (positional.Count != 1) throw new UsageException("trace에는 유닛의 가상 경로 하나가 필요하다");
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var traced = new UnitTracer(stack).Trace(positional[0]);
                if (traced == null) { output.WriteLine("원본에 이 파일이 없다: " + positional[0]); return 1; }
                var index = OpenIndex(stack, settingsPath, output);
                output.WriteLine(traced.Unit + "  (" + (UnitKind.Of(traced.Unit) ?? "유닛 아님") + ", 계층 " + stack.Find(traced.Unit).Layer + ")");
                foreach (string u in traced.Unresolved) output.WriteLine("풀리지 않은 구문: " + u);
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                foreach (var row in traced.Rows)
                {
                    if (filter != null && row.Path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    output.WriteLine(row.Path + " [" + row.Occurrence + "]");
                    for (int i = 0; i < row.Values.Count; i++)
                    {
                        var v = row.Values[i];
                        if (v.Arg < 0) { output.WriteLine("  " + i + "=" + v.Text + "  (" + v.Note + ")"); continue; }
                        string blocker = IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, v.File, v.KeyPath);
                        output.WriteLine("  " + i + "=" + v.Text + "  ← " + v.File + " " + v.KeyPath + " [" + v.Occurrence + "] 값 " + v.Arg +
                                         "  공유 " + index.UsersOf(v.File).Count + "  이 유닛만: " + (blocker == null ? "가능" : "불가(" + blocker + ")"));
                    }
                }
                return 0;
            }
        }

        static int Show(List<string> positional, Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            if (positional.Count != 1) throw new UsageException("show에는 가상 경로 하나가 필요하다");
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var file = stack.Find(positional[0]);
                if (file == null) { output.WriteLine("원본에 이 파일이 없다: " + positional[0]); return 1; }
                var versions = stack.Versions(positional[0]);
                output.WriteLine(file.VirtualPath + "  (계층 " + file.Layer + (versions.Count > 1 ? ", 겹침 " + string.Join(" → ", versions.Select(v => v.Layer)) : "") + ")");
                var parsed = SdlCore.Parse(TextCodec.Decode(stack.Read(file), out TextKind kind));
                if (parsed.Errors.Count > 0) { output.WriteLine("파싱되지 않는다: " + parsed.Errors[0]); return 1; }
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                foreach (var row in KeyLocator.Rows(parsed.Root))
                {
                    if (row.Leaves.Count == 0) continue;
                    if (filter != null && row.Path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    output.WriteLine(row.Path + " [" + row.Occurrence + "] " + string.Join(" ", row.Leaves.Select((l, i) => i + "=" + l.Text)));
                }
                return 0;
            }
        }

        static int Add(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string projectPath = Need(opt, "project"), fileArg = Need(opt, "file"), keyPath = Need(opt, "path"), newValue = Need(opt, "new");
            int occ = Number(opt, "occ"), arg = Number(opt, "arg");
            string scope = opt.ContainsKey("scope") ? opt["scope"].ToLowerInvariant() : "shared";
            if (scope != "shared" && scope != "unit") throw new UsageException("--scope는 shared 또는 unit이다");
            if (scope == "unit" && !opt.ContainsKey("unit")) throw new UsageException("--scope unit에는 --unit이 필요하다");
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var file = stack.Find(fileArg);
                if (file == null) { output.WriteLine("원본에 이 파일이 없다: " + fileArg); return 1; }
                var parsed = SdlCore.Parse(TextCodec.Decode(stack.Read(file), out TextKind kind));
                if (parsed.Errors.Count > 0) { output.WriteLine("파싱되지 않는 파일은 고칠 수 없다: " + parsed.Errors[0]); return 1; }
                var node = KeyLocator.Find(parsed.Root, keyPath, occ, arg);
                if (node == null) { output.WriteLine("이 자리가 없다: " + keyPath + " [" + occ + "] 값 " + arg + ". show로 확인한다."); return 1; }
                string warning;
                string error = TokenPatcher.CheckNewValue(node.Text, newValue, out warning);
                if (error != null) { output.WriteLine(error); return 1; }
                if (warning != null) output.WriteLine("경고: " + warning);
                string unit = SourceStack.ToVirtual(opt.ContainsKey("unit") ? opt["unit"] : fileArg);
                if (scope == "unit")
                {
                    if (stack.Find(unit) == null) { output.WriteLine("원본에 이 유닛 파일이 없다: " + unit); return 1; }
                    string blocker = IncludeChain.UnitScopeBlocker(new SdlResolver(stack.ReadText).Resolve(unit).IncludeEdges, unit, file.VirtualPath, keyPath);
                    if (blocker != null) { output.WriteLine(blocker); return 1; }
                }

                var project = File.Exists(projectPath) ? EditProject.Load(projectPath) : new EditProject { Name = Path.GetFileNameWithoutExtension(projectPath) };
                project.Edits.Add(new Edit
                {
                    Unit = unit, File = file.VirtualPath, Path = keyPath.ToLowerInvariant(), Occurrence = occ, Arg = arg, Old = node.Text, New = newValue, Scope = scope
                });
                project.Save(projectPath);
                output.WriteLine("수정을 더했다: " + file.VirtualPath + " " + keyPath + " " + node.Text + " → " + newValue + " (계층 " + file.Layer + (scope == "unit" ? ", 이 유닛만: " + unit : "") + ")");
                return 0;
            }
        }

        static int Build(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string projectPath = Need(opt, "project");
            if (!File.Exists(projectPath)) { output.WriteLine("프로젝트 파일이 없다: " + projectPath); return 1; }
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                foreach (string w in plan.Warnings) output.WriteLine("경고: " + w);
                if (Process.GetProcessesByName("call_to_arms").Length > 0 || Process.GetProcessesByName("call_to_arms_ed").Length > 0)
                    output.WriteLine("경고: 게임이나 에디터가 실행 중이다. 다시 시작해야 반영된다.");
                var report = new ModBuilder(stack, settings.OutputModDir).Build(EditProject.Load(projectPath));
                foreach (var r in report.Results.Where(x => x.Status != EditStatus.Applied || x.Message != null))
                    output.WriteLine(r.Status + ": " + r.Edit.File + " " + r.Edit.Path + " [" + r.Edit.Occurrence + "] 값 " + r.Edit.Arg + (r.Message == null ? "" : " : " + r.Message));
                foreach (string w in report.Warnings) output.WriteLine("경고: " + w);
                foreach (string e in report.FileErrors) output.WriteLine("오류: " + e);
                foreach (string f in report.WrittenFiles) output.WriteLine("씀: " + f);
                foreach (string f in report.RemovedFiles) output.WriteLine("지움: " + f);
                output.WriteLine("적용 " + report.Count(EditStatus.Applied) + "건, 원본이 바뀜 " + report.Count(EditStatus.SourceChanged) + "건, 대상 없음 " +
                                 report.Count(EditStatus.TargetMissing) + "건, 적용 불가 " + report.Count(EditStatus.Invalid) + "건, 파일 오류 " + report.FileErrors.Count + "건");
                return report.FileErrors.Count > 0 ? 1 : 0;
            }
        }
    }
}
