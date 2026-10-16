// 화면 없이 코어를 쓰는 명령줄 도구. 게임 폴더는 읽기만 하고, 쓰는 곳은 설정 파일, 프로젝트 파일, 출력 모드 폴더다.
using System;
using System.Collections.Generic;
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
            "  ctatoybox add --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] --new <값> [--scope shared|unit] [--unit <유닛 가상 경로>] [--kind macro-name]   (macro-name: 인자 없는 매크로 호출의 이름을 같은 파일에 정의된 다른 이름으로)\n" +
            "  ctatoybox remove --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] [--scope shared|unit] [--unit <유닛 가상 경로>]\n" +
            "  ctatoybox merge --project <합칠 프로젝트> --from <가져올 프로젝트>   (가져올 프로젝트의 수정을 더한다. 같은 자리에 다른 값이 있으면 합칠 프로젝트의 것을 둔다)\n" +
            "  ctatoybox build --project <파일> [--force]   (게임이 실행 중이면 --force 없이는 빌드하지 않는다)\n" +
            "  ctatoybox vision-preset --variant wot|wt --project <파일> [--scale <수, 기본 4>] [--force] [--classes [--heavy-uncover N] [--medium-r X] [--medium-uncover N] [--light-r X] [--light-uncover N] [--vehicle-r X] [--vehicle-uncover N]]   (월드워 시야 프로젝트를 만든다. --classes는 wot에 차종별 시야·탐지를 더한다(E18). 있는 파일은 --force로만 덮어쓴다)\n" +
            "공통: --settings <파일> (기본 editor/settings.json)\n" +
            "종류: tank, cannon, car, vehicle-other, breed, stuff, settings(게임 설정), 또는 vehicle(차량 전부). units는 앱 목록과 같이 수가 있는 파일만 보인다(게임 설정은 수가 없어도 보인다)";

        // 명령마다 받는 옵션. 여기에 없는 옵션은 사용법 오류다(오타를 조용히 버리지 않는다)
        static readonly Dictionary<string, string[]> KnownOptions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["init"] = new[] { "settings", "game", "workshop", "options", "out", "name" },
            ["sources"] = new[] { "settings" },
            ["units"] = new[] { "settings", "find", "kind", "layer", "limit", "names" },
            ["trace"] = new[] { "settings", "filter" },
            ["show"] = new[] { "settings", "filter" },
            ["add"] = new[] { "settings", "project", "file", "path", "occ", "arg", "new", "scope", "unit", "kind" },
            ["remove"] = new[] { "settings", "project", "file", "path", "occ", "arg", "scope", "unit", "kind" },
            ["merge"] = new[] { "settings", "project", "from" },
            ["build"] = new[] { "settings", "project", "force" },
            ["vision-preset"] = new[] { "settings", "variant", "project", "scale", "force", "classes", "heavy-uncover", "medium-r", "medium-uncover", "light-r", "light-uncover", "vehicle-r", "vehicle-uncover" },
        };

        // 값을 받지 않는 옵션
        static readonly HashSet<string> Flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "force", "classes" };

        // 게임이나 에디터가 실행 중인지. 테스트가 바꿔 끼운다
        public static Func<bool> GameRunning = EditorSession.GameRunning;

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
                    if (args[i].StartsWith("--") && Flags.Contains(args[i].Substring(2))) opt[args[i].Substring(2)] = "";
                    else if (args[i].StartsWith("--"))
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
                    case "remove": return Remove(opt, output);
                    case "merge": return Merge(opt, output);
                    case "build": return Build(opt, settingsPath, output);
                    case "vision-preset": return VisionPreset(opt, settingsPath, output);
                    default: throw new UsageException("모르는 명령: " + args[0]);
                }
            }
            catch (UsageException ex)
            {
                output.WriteLine(ex.Message);
                output.WriteLine(Usage);
                return 2;
            }
            // 깨진 설정·프로젝트·pak: 코어가 파일 이름과 이유를 넣어 던진다. 스택 트레이스 대신 그것만 보인다
            catch (InvalidDataException ex)
            {
                output.WriteLine("오류: " + ex.Message);
                return 1;
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

        // 월드워 시야 프로젝트(VisionPresets)를 지금 원본 계층에서 만들어 쓴다
        static int VisionPreset(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string variant = Need(opt, "variant").ToLowerInvariant(), projectPath = Need(opt, "project");
            if (variant != "wot" && variant != "wt") throw new UsageException("--variant는 wot 또는 wt다");
            if (opt.ContainsKey("classes") && variant != "wot") throw new UsageException("--classes는 --variant wot와 쓴다");
            double scale = 4;
            if (opt.ContainsKey("scale") && (!double.TryParse(opt["scale"], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out scale) || scale <= 0))
                throw new UsageException("--scale은 0보다 큰 수여야 한다");
            if (File.Exists(projectPath) && !opt.ContainsKey("force")) { output.WriteLine("이미 있다: " + projectPath + ". 덮어쓰려면 --force"); return 1; }
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var notes = new List<string>();
                var project = VisionPresets.Build(stack, variant, scale, notes);
                ClassReport classes = null;
                if (opt.ContainsKey("classes"))
                {
                    var v = new ClassVision();
                    v.HeavyUncover = Whole(opt, "heavy-uncover", v.HeavyUncover); v.MediumUncover = Whole(opt, "medium-uncover", v.MediumUncover);
                    v.LightUncover = Whole(opt, "light-uncover", v.LightUncover); v.VehicleUncover = Whole(opt, "vehicle-uncover", v.VehicleUncover);
                    v.MediumR = Positive(opt, "medium-r", v.MediumR); v.LightR = Positive(opt, "light-r", v.LightR); v.VehicleR = Positive(opt, "vehicle-r", v.VehicleR);
                    var index = OpenIndex(stack, settingsPath, output);
                    classes = VisionPresets.AddClasses(project, stack, index.Units, new UnitTracer(stack), u => index.HeirsOf(u).Count, v, notes);
                }
                project.Save(projectPath);
                var fields = project.Edits.Where(e => e.Kind == null && e.NamedPath != null && e.File == VisionPresets.FieldsFile).ToList();
                int fov = fields.Count(e => e.NamedPath.EndsWith("/h_fov")), uncover = fields.Count(e => e.NamedPath.EndsWith("/uncover"));
                output.WriteLine("수정 " + project.Edits.Count + "건(h_fov " + fov + ", uncover " + uncover + ", r " + (fields.Count - fov - uncover) + (classes == null ? "" : ", 블록 복제 " + project.Edits.Count(e => e.Kind == BlockCopy.Kind) + ", 유닛·차량 파일 " + (project.Edits.Count - fields.Count - project.Edits.Count(e => e.Kind == BlockCopy.Kind))) + "): " + projectPath);
                if (classes != null) output.WriteLine("경전차 " + classes.Light + ", 중형전차 " + classes.Medium + ", 차량 파일 " + classes.VehicleFiles + ", 건너뜀 " + classes.Skipped.Count);
                foreach (string n in notes) output.WriteLine("  " + n);
                return 0;
            }
        }

        static int Whole(Dictionary<string, string> opt, string name, int fallback)
        {
            if (!opt.ContainsKey(name)) return fallback;
            int v;
            if (!int.TryParse(opt[name], out v) || v < 0) throw new UsageException("--" + name + "는 0 이상의 정수다");
            return v;
        }

        static double Positive(Dictionary<string, string> opt, string name, double fallback)
        {
            if (!opt.ContainsKey(name)) return fallback;
            double v;
            if (!double.TryParse(opt[name], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v) || v <= 0) throw new UsageException("--" + name + "는 0보다 큰 수다");
            return v;
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
            if (kind != null && !UnitKind.InEditor(kind) && kind != "vehicle") throw new UsageException("--kind는 다음 가운데 하나다: " + string.Join(", ", UnitKind.Editor) + ", vehicle");
            int limit = opt.ContainsKey("limit") ? Number(opt, "limit") : 50;
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var index = OpenIndex(stack, settingsPath, output);
                // 이름은 게임과 켜 둔 모드의 현지화에서 읽는다(앱과 같다). --names를 주면 그 CSV를 쓴다
                var names = opt.ContainsKey("names") ? DisplayNames.Load(opt["names"]) : DisplayNames.FromGame(settings, plan);
                foreach (string w in names.Warnings) output.WriteLine("경고: " + w);
                foreach (var kv in SettingFiles.LoadNames(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "settings", "files.psv")))
                    names.AddPathName(kv.Key, kv.Value.Name);
                var found = index.Search(opt.ContainsKey("find") ? opt["find"] : null, null, opt.ContainsKey("layer") ? opt["layer"] : null, names)
                    .Where(u => UnitKind.InEditor(u.EditorKind) && (u.Numbers > 0 || u.EditorKind == SettingFiles.Kind) && (kind == null || u.EditorKind == kind || u.Kind == kind)).ToList();
                foreach (var u in found.Take(limit))
                {
                    string name = names.OfUnit(u.Path);
                    output.WriteLine(u.EditorKind + "  " + u.Layer + "  " + u.Path + (name == null ? "" : "  " + name) + (u.Unresolved > 0 ? "  (풀리지 않은 구문 " + u.Unresolved + ")" : ""));
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
                output.WriteLine(traced.Unit + "  (" + (UnitKind.OfEditor(traced.Unit) ?? "유닛 아님") + ", 계층 " + stack.Find(traced.Unit).Layer + ")");
                foreach (string u in traced.Unresolved) output.WriteLine("풀리지 않은 구문: " + u);
                if (traced.From.Count > 0) output.WriteLine(FromLevel.ChainLine(traced.From));
                if (traced.ReadError != null) output.WriteLine(traced.ReadError);
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                // 게임 설정은 파일 하나가 곧 설정이라 '이 유닛만'이 없다(앱의 OpenUnit과 같다)
                bool settingsUnit = UnitKind.OfEditor(traced.Unit) == SettingFiles.Kind;
                foreach (var row in traced.Rows)
                {
                    if (filter != null && row.Path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    output.WriteLine(row.Path + " [" + row.Occurrence + "]" + (row.From == null ? "" : "  (" + FromLevel.LabelOf(row.From, row.FromState, row.FromBlock) + ")"));
                    for (int i = 0; i < row.Values.Count; i++)
                    {
                        var v = row.Values[i];
                        if (v.Arg < 0) { output.WriteLine("  " + i + "=" + v.Text + "  (" + v.Note + ")"); continue; }
                        string blocker = settingsUnit || SettingFiles.InArea(v.File) ? SettingFiles.UnitScopeNote
                            : row.From != null ? UnitScope.InheritedBlocker(traced, row) : IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, v.File, v.KeyPath);
                        if (blocker == null && index.HeirsOf(traced.Unit).Count > 0) blocker = FromLevel.HeirsNote(index.HeirsOf(traced.Unit).Count);
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
                string error;
                var rows = SourceRows.Of(file.VirtualPath, TextCodec.Decode(stack.Read(file), out TextKind kind), out error);
                if (rows == null) { output.WriteLine("읽히지 않는다: " + error); return 1; }
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                foreach (var row in rows)
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
            string editKind = opt.ContainsKey("kind") ? opt["kind"] : null;
            if (editKind != null && editKind != MacroSwap.Kind && editKind != BlockCopy.Kind) throw new UsageException("--kind는 " + MacroSwap.Kind + " 또는 " + BlockCopy.Kind + "이다");
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var file = stack.Find(fileArg);
                if (file == null) { output.WriteLine("원본에 이 파일이 없다: " + fileArg); return 1; }
                if (scope == "unit" && (SettingFiles.InArea(file.VirtualPath) || UnitKind.OfEditor(SourceStack.ToVirtual(opt["unit"])) == SettingFiles.Kind))
                { output.WriteLine(SettingFiles.UnitScopeNote); return 1; }
                var project = File.Exists(projectPath) ? EditProject.Load(projectPath) : new EditProject { Name = Path.GetFileNameWithoutExtension(projectPath) };
                // 이 프로젝트의 켜진 블록 복제를 넣은 본문에서 자리를 찾는다(빌드와 같은 본문, E18)
                var fileCopies = project.Edits.Where(e => e.Kind == BlockCopy.Kind && e.Enabled && string.Equals(SourceStack.ToVirtual(e.File ?? ""), file.VirtualPath, StringComparison.Ordinal)).ToList();
                string readError;
                string original = TextCodec.Decode(stack.Read(file), out TextKind kind);
                string text = BlockCopy.ApplyAll(original, fileCopies);
                var rows = SourceRows.Of(file.VirtualPath, text, out readError);
                if (rows == null) { output.WriteLine("읽히지 않는 파일은 고칠 수 없다: " + readError); return 1; }
                var found = rows.FirstOrDefault(r => r.Occurrence == occ && string.Equals(r.Path, keyPath, StringComparison.OrdinalIgnoreCase));
                // --kind macro-name은 인자 없는 호출 줄의 이름 토큰을 고친다(E4)
                // --kind block-copy는 따옴표 이름 블록의 이름 줄을 고른다(E18)
                var node = editKind == MacroSwap.Kind ? (found != null && found.Leaves.Count == 0 && arg == 0 ? found.Head : null)
                    : editKind == BlockCopy.Kind ? (found != null && found.Path.EndsWith("/\"*\"", StringComparison.Ordinal) && arg == 0 && found.Leaves.Count > 0 ? found.Leaves[0] : null)
                    : found != null && arg < found.Leaves.Count ? found.Leaves[arg] : null;
                if (node == null) { output.WriteLine("이 자리가 없다: " + keyPath + " [" + occ + "] 값 " + arg + ". show로 확인한다."); return 1; }
                string warning = null;
                string named = KeyLocator.IsNamed(keyPath) ? found.RawPath : null;
                int namedOcc = named == null ? 0 : found.RawOccurrence;
                string error = editKind == BlockCopy.Kind ? CopyError(original, fileCopies, file.VirtualPath, keyPath, occ, named, namedOcc, node.Text, newValue) : ValueRules.CheckNewValue(file.VirtualPath, node.Text, newValue, out warning)
                               ?? (TextCodec.CanEncode(newValue, kind) ? null : "새 값에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + newValue)
                               ?? ValueRules.CheckApplies(file.VirtualPath, text, new Edit { File = file.VirtualPath, Path = keyPath, Occurrence = occ, Arg = arg, Old = node.Text, New = newValue, Kind = editKind });
                string unitForSwap = SourceStack.ToVirtual(opt.ContainsKey("unit") ? opt["unit"] : fileArg);
                if (error == null && editKind == MacroSwap.Kind)
                    error = MacroSwap.Check(new UnitTracer(stack).Trace(unitForSwap), new Edit { Old = node.Text, New = newValue, Kind = editKind });
                if (error != null) { output.WriteLine(error); return 1; }
                if (warning != null) output.WriteLine("경고: " + warning);
                string unit = SourceStack.ToVirtual(opt.ContainsKey("unit") ? opt["unit"] : fileArg);
                if (scope == "unit")
                {
                    if (stack.Find(unit) == null) { output.WriteLine("원본에 이 유닛 파일이 없다: " + unit); return 1; }
                    // 앱·빌드와 같은 규칙(UnitScope). 원형에서 물려받은 값은 덧붙일 수 있는 줄이면 된다(E14)
                    string blocker = UnitScope.Blocker(new UnitTracer(stack).Trace(unit), file.VirtualPath, keyPath, occ, arg);
                    if (blocker == null)
                    {
                        int heirs = OpenIndex(stack, settingsPath, output).HeirsOf(unit).Count;
                        if (heirs > 0) blocker = FromLevel.HeirsNote(heirs);
                    }
                    if (blocker != null) { output.WriteLine(blocker); return 1; }
                }

                // 같은 자리·같은 범위의 수정이 있으면 값만 바꾼다(앱의 EditorSession.AddEdit과 같다). '이 유닛만'은 유닛까지 같아야 같은 수정이다
                var existing = project.Edits.FirstOrDefault(e => e.Scope == scope && e.Kind == editKind
                    && string.Equals(SourceStack.ToVirtual(e.File ?? ""), file.VirtualPath, StringComparison.Ordinal)
                    && string.Equals(e.Path, keyPath, StringComparison.OrdinalIgnoreCase) && e.Occurrence == occ && e.Arg == arg
                    && (scope != "unit" || SourceStack.ToVirtual(e.Unit ?? "") == unit));
                if (existing != null)
                {
                    string before = existing.New;
                    existing.New = newValue; existing.Enabled = true;
                    project.Save(projectPath);
                    output.WriteLine("수정을 바꿨다: " + file.VirtualPath + " " + keyPath + " " + node.Text + " → " + newValue + " (전에는 → " + before + ")");
                    return 0;
                }
                project.Edits.Add(new Edit
                {
                    Unit = unit, File = file.VirtualPath, Path = keyPath.ToLowerInvariant(), Occurrence = occ, Arg = arg, Old = node.Text, New = newValue, Scope = scope, Kind = editKind, NamedPath = named, NamedOccurrence = namedOcc
                });
                project.Save(projectPath);
                output.WriteLine("수정을 더했다: " + file.VirtualPath + " " + keyPath + " " + node.Text + " → " + newValue + " (계층 " + file.Layer + (scope == "unit" ? ", 이 유닛만: " + unit : "") + ")");
                return 0;
            }
        }

        // 블록 복제(E18)가 안 되는 이유. 되면 null. 이름 규칙, 원본 블록, 겹치는 이름을 빌드와 같은 규칙(BlockCopy.Apply)으로 본다
        static string CopyError(string original, List<Edit> fileCopies, string file, string keyPath, int occ, string named, int namedOcc, string old, string newValue)
        {
            if (!SettingFiles.InArea(file)) return "게임 설정 파일의 따옴표 이름 블록만 복제할 수 있다";
            string bad = BlockCopy.CheckName(newValue);
            if (bad != null) return bad;
            var results = new List<EditResult>();
            BlockCopy.Apply(original, fileCopies.Concat(new[] { new Edit { Kind = BlockCopy.Kind, File = file, Path = keyPath, Occurrence = occ, Arg = 0, NamedPath = named, NamedOccurrence = namedOcc, Old = old, New = newValue } }), results);
            var mine = results.Last();
            return mine.Status == EditStatus.Applied ? null : mine.Message;
        }

        // 프로젝트에서 한 자리의 수정을 지운다. 게임 파일은 읽지 않는다. 범위나 유닛을 주지 않았는데 그 자리의 수정이 여럿이면 고르게 한다
        static int Remove(Dictionary<string, string> opt, TextWriter output)
        {
            string projectPath = Need(opt, "project"), file = SourceStack.ToVirtual(Need(opt, "file")), keyPath = Need(opt, "path");
            int occ = Number(opt, "occ"), arg = Number(opt, "arg");
            string scope = opt.ContainsKey("scope") ? opt["scope"].ToLowerInvariant() : null;
            if (scope != null && scope != "shared" && scope != "unit") throw new UsageException("--scope는 shared 또는 unit이다");
            // --unit은 '이 유닛만' 수정을 고른다. '전부' 수정의 Unit은 만든 화면의 유닛일 뿐이라 거르지 않는다
            string unit = opt.ContainsKey("unit") ? SourceStack.ToVirtual(opt["unit"]) : null;
            if (unit != null && scope == "shared") throw new UsageException("--unit은 '이 유닛만' 수정을 고를 때 쓴다. --scope shared와 함께 쓰지 않는다");
            if (unit != null) scope = "unit";
            if (!File.Exists(projectPath)) { output.WriteLine("프로젝트 파일이 없다: " + projectPath); return 1; }
            var project = EditProject.Load(projectPath);
            var matches = project.Edits.Where(e => string.Equals(SourceStack.ToVirtual(e.File ?? ""), file, StringComparison.OrdinalIgnoreCase)
                && string.Equals(e.Path, keyPath, StringComparison.OrdinalIgnoreCase) && e.Occurrence == occ && e.Arg == arg
                && (scope == null || (scope == "unit") == (e.Scope == "unit"))
                && (!opt.ContainsKey("kind") || e.Kind == opt["kind"])
                && (unit == null || string.Equals(SourceStack.ToVirtual(e.Unit ?? ""), unit, StringComparison.OrdinalIgnoreCase))).ToList();
            if (matches.Count == 0) { output.WriteLine("그 자리의 수정이 없다: " + file + " " + keyPath + " [" + occ + "] 값 " + arg); return 1; }
            if (matches.Count > 1)
            {
                output.WriteLine("그 자리의 수정이 " + matches.Count + "개다. --scope나 --unit으로 고른다:");
                foreach (var e in matches) output.WriteLine("  " + e.Scope + "  " + e.Unit + "  " + e.Old + " → " + e.New);
                return 1;
            }
            project.Edits.Remove(matches[0]);
            project.Save(projectPath);
            output.WriteLine("수정을 지웠다: " + file + " " + keyPath + " " + matches[0].Old + " → " + matches[0].New + " (" + matches[0].Scope + ")");
            return 0;
        }

        // E17: 빌드는 프로젝트 하나만 쓴다. 다른 프로젝트의 수정을 이 프로젝트로 가져온다
        static int Merge(Dictionary<string, string> opt, TextWriter output)
        {
            string projectPath = Need(opt, "project"), fromPath = Need(opt, "from");
            if (!File.Exists(fromPath)) { output.WriteLine("가져올 프로젝트 파일이 없다: " + fromPath); return 1; }
            if (string.Equals(Path.GetFullPath(projectPath), Path.GetFullPath(fromPath), StringComparison.OrdinalIgnoreCase)) { output.WriteLine("같은 프로젝트끼리는 합치지 않는다: " + projectPath); return 1; }
            var project = File.Exists(projectPath) ? EditProject.Load(projectPath) : new EditProject { Name = Path.GetFileNameWithoutExtension(projectPath) };
            var report = project.MergeFrom(EditProject.Load(fromPath));
            if (report.Added > 0) project.Save(projectPath);
            output.WriteLine("합쳤다: 더함 " + report.Added + "건, 이미 같음 " + report.Same + "건, 충돌 " + report.Conflicts.Count + "건 → " + projectPath);
            foreach (string c in report.Conflicts) output.WriteLine("  충돌: " + c);
            return 0;
        }

        static int Build(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string projectPath = Need(opt, "project");
            if (!File.Exists(projectPath)) { output.WriteLine("프로젝트 파일이 없다: " + projectPath); return 1; }
            // 실행 중인 게임은 켤 때 본 모드 파일을 나중에 다시 읽는다. 그 사이에 파일을 바꾸거나 지우면 꺼질 수 있다
            // (2026-10-14: 빌드가 지운 hmgun_usa.ammo를 다음 전투 시작 때 읽다가 "Block start expected!"로 꺼졌다)
            if (GameRunning())
            {
                if (!opt.ContainsKey("force"))
                {
                    output.WriteLine("오류: 게임이나 에디터가 실행 중이다. 실행 중에 모드 파일을 바꾸거나 지우면, 게임이 그 파일을 다시 읽을 때(예: 전투를 새로 시작할 때) 꺼질 수 있다. 게임을 끄고 다시 빌드한다. 그래도 빌드하려면 --force를 붙인다.");
                    return 1;
                }
                output.WriteLine("경고: 게임이나 에디터가 실행 중이다(--force). 전투를 새로 시작하지 말고 게임을 다시 시작한다. 실행 중인 게임은 바뀐 파일을 읽다가 꺼질 수 있다.");
            }
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                foreach (string w in plan.Warnings) output.WriteLine("경고: " + w);
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
