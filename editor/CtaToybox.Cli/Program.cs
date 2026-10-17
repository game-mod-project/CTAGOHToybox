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
            "  ctatoybox units [--find <글자>] [--kind <종류>] [--layer <계층>] [--limit N] [--names <csv>] [--army ger|rus|usa|eng|fin|jap]\n" +
            "  ctatoybox trace <유닛 가상 경로> [--filter <글자>]\n" +
            "  ctatoybox show <가상 경로> [--filter <글자>]\n" +
            "  ctatoybox add --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] --new <값> [--scope shared|unit] [--unit <유닛 가상 경로>] [--kind macro-name]   (macro-name: 인자 없는 매크로 호출의 이름을 같은 파일에 정의된 다른 이름으로)\n" +
            "  ctatoybox add --project <파일> --kind file-copy --file <원본 가상 경로> --new <사본 가상 경로>   (같은 폴더에 새 이름의 사본. 유닛이 쓰게 하려면 이름 값을 사본 이름으로 바꾼다)\n" +
            "  ctatoybox add --project <파일> --kind add-line --file <가상 경로> --path <블록 경로, 예 /perks> [--occ N] --new \"<줄>\"   (블록의 닫는 괄호 앞에 줄 하나를 넣는다)\n" +
            "  ctatoybox remove --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] [--scope shared|unit] [--unit <유닛 가상 경로>]\n" +
            "  ctatoybox merge --project <합칠 프로젝트> --from <가져올 프로젝트>   (가져올 프로젝트의 수정을 더한다. 같은 자리에 다른 값이 있으면 합칠 프로젝트의 것을 둔다)\n" +
            "  ctatoybox rule preview|add --kind <종류> [--army ger|rus|usa|eng|fin|jap] [--path <글자,…>] [--exclude <글자,…>] [--any <글자,…>] [--items ammo|other] [--layer <계층>] --key <펼친 키> [--arg N|all] --set|--mul|--add <값> [--scope auto|shared|unit] [--project <파일>] [--name <이름>]\n" +
            "  ctatoybox rule list|reapply|remove|enable|disable --project <파일> [--id <id>]   rule edit --project <파일> --id <id> <바꿀 옵션>   (일괄 규칙)\n" +
            "  ctatoybox build --project <파일> [--force]   (게임이 실행 중이면 --force 없이는 빌드하지 않는다)\n" +
            "  ctatoybox faction-preset --army ger|rus|usa|eng|fin|jap --project <파일> [--base <베이스 프로젝트>] [--force] [--super <초중전차 목록, 기본 editor/presets/super-heavy.txt>] [--inventory 2] [--ammo 5] [--reload 0.5] [--health 2] [--armor-car 2] [--armor-light 2] [--armor-medium 2.5] [--armor-heavy 3] [--armor-super 5] [--speed 1.5] [--fuel 10] [--skirmish 50] [--report <건너뜀 전체를 쓸 파일>]   (그 군대 유닛만 강화하고 컨퀘스트 플레이어 자원을 최대로 하는 프로젝트를 만든다)\n" +
            "  ctatoybox vision-preset --variant wot|wt --project <파일> [--scale <수, 기본 4>] [--force] [--classes [--heavy-uncover N] [--medium-r X] [--medium-uncover N] [--light-r X] [--light-uncover N] [--vehicle-r X] [--vehicle-uncover N]]   (월드워 시야 프로젝트를 만든다. --classes는 wot에 차종별 시야·탐지를 더한다(E18). 있는 파일은 --force로만 덮어쓴다)\n" +
            "  ctatoybox l10n extract|check|build [--dir <번역 보관, 기본 l10n/ko>] [--file <글자>] [--force]   (한글화: 보관 갱신, 검사, 출력 모드에 .pot·글꼴 쓰기)\n" +
            "  ctatoybox spawn-menu preview|build [--out <폴더, 기본 mod/CTAToyboxSpawn>] [--force] [--keep-cost] [--basic-buttons] [--grid <열, 기본 7> | --list]   (Spawn Menu를 대체하는 스폰 모드를 만든다. preview는 쓰지 않고 수만 보인다)\n" +
            "공통: --settings <파일> (기본 editor/settings.json)\n" +
            "종류: tank, cannon, car, vehicle-other, breed, stuff, settings(게임 설정), 또는 vehicle(차량 전부). units는 앱 목록과 같이 수가 있는 파일만 보인다(게임 설정은 수가 없어도 보인다)";

        // 명령마다 받는 옵션. 여기에 없는 옵션은 사용법 오류다(오타를 조용히 버리지 않는다)
        static readonly Dictionary<string, string[]> KnownOptions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["init"] = new[] { "settings", "game", "workshop", "options", "out", "name" },
            ["sources"] = new[] { "settings" },
            ["units"] = new[] { "settings", "find", "kind", "layer", "limit", "names", "army" },
            ["trace"] = new[] { "settings", "filter" },
            ["show"] = new[] { "settings", "filter" },
            ["add"] = new[] { "settings", "project", "file", "path", "occ", "arg", "new", "scope", "unit", "kind" },
            ["remove"] = new[] { "settings", "project", "file", "path", "occ", "arg", "scope", "unit", "kind" },
            ["merge"] = new[] { "settings", "project", "from" },
            ["rule"] = new[] { "settings", "project", "kind", "path", "layer", "key", "arg", "set", "mul", "add", "scope", "name", "id", "army", "exclude", "any", "items" },
            ["build"] = new[] { "settings", "project", "force" },
            ["faction-preset"] = new[] { "settings", "army", "base", "project", "force", "super", "inventory", "ammo", "reload", "health", "armor-car", "armor-light", "armor-medium", "armor-heavy", "armor-super", "speed", "fuel", "skirmish", "report" },
            ["l10n"] = new[] { "settings", "dir", "file", "force" },
            ["spawn-menu"] = new[] { "settings", "out", "force", "keep-cost", "basic-buttons", "list", "grid" },
            ["vision-preset"] = new[] { "settings", "variant", "project", "scale", "force", "classes", "heavy-uncover", "medium-r", "medium-uncover", "light-r", "light-uncover", "vehicle-r", "vehicle-uncover" },
        };

        // 값을 받지 않는 옵션
        static readonly HashSet<string> Flags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "force", "classes", "keep-cost", "basic-buttons", "list" };

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
                    case "rule": return Rule(positional, opt, settingsPath, output);
                    case "build": return Build(opt, settingsPath, output);
                    case "vision-preset": return VisionPreset(opt, settingsPath, output);
                    case "faction-preset": return FactionPresetCli(opt, settingsPath, output);
                    case "spawn-menu": return SpawnMenu(positional, opt, settingsPath, output);
                    case "l10n": return L10n(positional, opt, settingsPath, output);
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

        // 팩션 강화 프로젝트(docs/superpowers/specs/2026-10-17-faction-boost-design.md)
        static int FactionPresetCli(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string army = Need(opt, "army").ToLowerInvariant(), projectPath = Need(opt, "project");
            if (!ArmyIndex.IsArmy(army)) throw new UsageException("--army는 다음 가운데 하나다: " + string.Join(", ", ArmyIndex.Armies));
            if (File.Exists(projectPath) && !opt.ContainsKey("force")) { output.WriteLine("이미 있다: " + projectPath + ". 덮어쓰려면 --force"); return 1; }
            var o = new FactionOptions { Army = army };
            o.Inventory = Positive(opt, "inventory", o.Inventory); o.Ammo = Positive(opt, "ammo", o.Ammo); o.Reload = Positive(opt, "reload", o.Reload); o.Health = Positive(opt, "health", o.Health);
            o.ArmorCar = Positive(opt, "armor-car", o.ArmorCar); o.ArmorLight = Positive(opt, "armor-light", o.ArmorLight); o.ArmorMedium = Positive(opt, "armor-medium", o.ArmorMedium);
            o.ArmorHeavy = Positive(opt, "armor-heavy", o.ArmorHeavy); o.ArmorSuper = Positive(opt, "armor-super", o.ArmorSuper); o.Speed = Positive(opt, "speed", o.Speed); o.Fuel = Positive(opt, "fuel", o.Fuel);
            string superPath = opt.ContainsKey("super") ? opt["super"] : Path.Combine("editor", "presets", "super-heavy.txt");
            if (!File.Exists(superPath)) { output.WriteLine("초중전차 목록이 없다: " + superPath); return 1; }
            o.SuperHeavy = FactionPreset.LoadSuperHeavy(superPath);
            EditProject baseProject = null;
            if (opt.ContainsKey("base"))
            {
                if (!File.Exists(opt["base"])) { output.WriteLine("베이스 프로젝트가 없다: " + opt["base"]); return 1; }
                baseProject = EditProject.Load(opt["base"]);
            }
            using (var session = EditorSession.Open(settingsPath, Directory.GetCurrentDirectory(), null))
            {
                var c = new FactionContext { Stack = session.Stack, Index = session.Index, Tracer = new UnitTracer(session.Stack), Armies = session.Armies, Items = session.Items };
                var missing = o.SuperHeavy.Where(p => !session.Index.Units.Any(u => u.Path.StartsWith(p, StringComparison.Ordinal))).ToList();
                var report = new FactionReport();
                var project = FactionPreset.Build(c, baseProject, o, new ConquestValues { Skirmish = Positive(opt, "skirmish", 50) }, report);
                project.Save(projectPath);
                output.WriteLine("군대 " + army + ": 유닛 " + report.Units + "(보병 " + report.Infantry + ", 차량 " + report.Vehicles + "), 수정 " + project.Edits.Count + "건, 규칙 " + (project.Rules == null ? 0 : project.Rules.Count) + "개: " + projectPath);
                foreach (var kv in report.Edits) output.WriteLine("  " + kv.Key + ": " + kv.Value);
                if (missing.Count > 0) output.WriteLine("초중전차 목록에서 지금 판에 없는 폴더 " + missing.Count + ": " + string.Join(", ", missing.Take(10)));
                output.WriteLine("두 군대가 같이 써서 건너뛴 유닛 " + report.Shared.Count + (report.Shared.Count == 0 ? "" : ": " + string.Join("; ", report.Shared.Take(20))));
                output.WriteLine("건너뜀 " + report.Skipped.Count + "건" + (report.Skipped.Count > 20 ? " (앞 20)" : ""));
                foreach (string s in report.Skipped.Take(20)) output.WriteLine("  " + s);
                foreach (string n in report.Notes) output.WriteLine("  " + n);
                if (opt.ContainsKey("report")) File.WriteAllLines(opt["report"], report.Edits.Select(kv => "수정 " + kv.Key + ": " + kv.Value).Concat(report.Shared.Select(x => "공유 " + x)).Concat(report.Skipped.Select(x => "건너뜀 " + x)).Concat(report.Notes));
                return 0;
            }
        }

        // 한글화(docs/superpowers/specs/2026-10-17-korean-localization-design.md): extract(번역 보관 갱신), check(검사), build(출력 모드에 쓰기)
        static int L10n(List<string> positional, Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string sub = positional.Count > 0 ? positional[0].ToLowerInvariant() : null;
            if (sub != "extract" && sub != "check" && sub != "build") throw new UsageException("l10n 다음에 extract, check, build 가운데 하나");
            string dir = opt.ContainsKey("dir") ? opt["dir"] : Path.Combine("l10n", "ko");
            EditorSettings settings; LayerPlan plan;
            using (var probe = Open(settingsPath, output, out settings, out plan)) { if (probe == null) return 1; }
            var warnings = new List<string>();
            if (sub == "extract")
            {
                var sources = Core.L10n.L10nSources.Collect(settings, plan, warnings);
                foreach (string w in warnings) output.WriteLine("  " + w);
                var r = Core.L10n.L10nCatalog.Extract(sources, dir, GameVersion(settings));
                output.WriteLine("파일 " + r.Files + ", 항목 " + r.Entries + ", 추가 " + r.Added + ", 바뀜 " + r.Changed + ", 지움 " + r.Removed + ", 미번역 " + r.Untranslated + ", fuzzy " + r.Fuzzy + ": " + dir);
                foreach (string f in r.RemovedFiles) output.WriteLine("  지운 파일 " + f);
                return 0;
            }
            var terms = Core.L10n.Glossary.Load(Path.Combine(dir, "glossary.psv"));
            string interfacePak = Path.Combine(settings.GameDir, "resource", "interface", "interface.pak");
            var fonts = File.Exists(interfacePak) ? Core.L10n.FontMap.Build(interfacePak) : new List<Core.L10n.FontOverride>();
            if (!File.Exists(interfacePak)) output.WriteLine("  interface.pak이 없어 글꼴을 바꾸지 않고 글자 검사를 건너뛴다: " + interfacePak);
            var glyphs = fonts.Count > 0 ? Core.L10n.L10nCheck.MainGlyphs(settings.GameDir, Core.L10n.FontMap.TargetFnts(fonts)) : null;
            var issues = Core.L10n.L10nCheck.Catalog(dir, terms, glyphs, opt.ContainsKey("file") ? opt["file"] : null);
            var errors = issues.Where(i => i.IsError).ToList();
            if (sub == "check")
            {
                foreach (var i in errors) output.WriteLine("오류 " + i.File + " " + i.Key + " " + i.Kind + ": " + i.Detail);
                output.WriteLine("오류 " + errors.Count + ", 미번역 " + issues.Count(i => i.Kind == "untranslated") + ", fuzzy " + issues.Count(i => i.Kind == "fuzzy"));
                return errors.Count > 0 ? 1 : 0;
            }
            if (GameRunning())
            {
                if (!opt.ContainsKey("force"))
                {
                    output.WriteLine("오류: 게임이나 에디터가 실행 중이다. 실행 중에 모드 파일을 바꾸거나 지우면, 게임이 그 파일을 다시 읽을 때(예: 전투를 새로 시작할 때) 꺼질 수 있다. 게임을 끄고 다시 빌드한다. 그래도 빌드하려면 --force를 붙인다.");
                    return 1;
                }
                output.WriteLine("경고: 게임이나 에디터가 실행 중이다(--force). 전투를 새로 시작하지 말고 게임을 다시 시작한다.");
            }
            if (errors.Count > 0)
            {
                foreach (var i in errors.Take(20)) output.WriteLine("오류 " + i.File + " " + i.Key + " " + i.Kind + ": " + i.Detail);
                output.WriteLine("검사 오류 " + errors.Count + "건. 고친 뒤 다시 빌드한다(l10n check)");
                return 1;
            }
            var all = Core.L10n.L10nSources.Collect(settings, plan, warnings);
            foreach (string w in warnings) output.WriteLine("  " + w);
            var br = Core.L10n.L10nBuilder.Build(all, dir, fonts, settings.OutputModDir);
            output.WriteLine("쓴 파일 " + br.Written.Count + "(원문 파일 " + br.Files + ", 번역 " + br.Translated + ", 영어로 둔 항목 " + br.FellBack + "), 글꼴 " + fonts.Count + ", 지운 파일 " + br.Removed.Count + ", 오류 " + br.Errors.Count + ": " + settings.OutputModDir);
            foreach (string e in br.Errors.Take(20)) output.WriteLine("  오류 " + e);
            return br.Errors.Count > 0 ? 1 : 0;
        }

        // binaries/x64/call_to_arms.exe의 제품 버전(예: 1.0.6.7-aaa36ed4d). 없으면 빈 글자
        static string GameVersion(EditorSettings settings)
        {
            string exe = Path.Combine(settings.GameDir, "binaries", "x64", "call_to_arms.exe");
            if (!File.Exists(exe)) return "";
            try { return System.Diagnostics.FileVersionInfo.GetVersionInfo(exe).ProductVersion ?? ""; }
            catch (IOException) { return ""; }
        }

        // 스폰 모드(docs/superpowers/specs/2026-10-17-spawn-menu-replacement-design.md). preview는 쓰지 않고 수만 보인다
        static int SpawnMenu(List<string> positional, Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string sub = positional.Count > 0 ? positional[0].ToLowerInvariant() : null;
            if (sub != "preview" && sub != "build") throw new UsageException("spawn-menu 다음에 preview 또는 build");
            if (sub == "build" && GameRunning())
            {
                if (!opt.ContainsKey("force"))
                {
                    output.WriteLine("오류: 게임이나 에디터가 실행 중이다. 실행 중에 모드 파일을 바꾸거나 지우면, 게임이 그 파일을 다시 읽을 때(예: 전투를 새로 시작할 때) 꺼질 수 있다. 게임을 끄고 다시 만든다. 그래도 만들려면 --force를 붙인다.");
                    return 1;
                }
                output.WriteLine("경고: 게임이나 에디터가 실행 중이다(--force). 전투를 새로 시작하지 말고 게임을 다시 시작한다.");
            }
            EditorSettings settings; LayerPlan plan;
            using (var probe = Open(settingsPath, output, out settings, out plan)) { if (probe == null) return 1; }
            string outDir = Path.GetFullPath(opt.ContainsKey("out") ? opt["out"] : Core.Spawn.SpawnSources.SpawnModDirOf(settings));
            settings.SpawnModDir = outDir;
            var warnings = new List<string>();
            using (var stack = Core.Spawn.SpawnSources.Open(settings, plan, warnings))
            {
                bool rebuilt;
                var index = UnitIndex.LoadOrBuild(stack, Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "cache", "spawn-unit-index.json"), null, out rebuilt);
                var armies = ArmyIndex.Build(stack, index.Units);
                var categories = Core.Spawn.SpawnCategories.Load(Path.Combine("editor", "spawn", "categories.psv"), warnings);
                var report = new Core.Spawn.SpawnReport();
                var o = new Core.Spawn.SpawnOptions { Roster = new Core.Spawn.RosterOptions { KeepCost = opt.ContainsKey("keep-cost"), BasicButtons = opt.ContainsKey("basic-buttons") } };
                o.Menu.GridCols = opt.ContainsKey("list") ? 0 : Whole(opt, "grid", 7);
                var files = Core.Spawn.SpawnGenerator.Generate(stack, index, armies, categories, o, report);
                foreach (string w in warnings) output.WriteLine("  " + w);
                output.WriteLine("맵 " + report.Maps + "(캠페인 " + report.CampaignMaps + ", 멀티 " + report.MultiMaps + "), 유닛 " + report.Units + ", 분대 " + report.Squads + ", 새 로스터 항목 " + report.NewRosterEntries + ", 단추 " + report.Buttons + ", 화면 " + report.Screens + ", 견본 " + report.Templates + ", 파일 " + files.Count + ", 건너뜀 " + report.Skipped.Count);
                foreach (string s in report.Skipped.Take(20)) output.WriteLine("  건너뜀 " + s);
                foreach (string n in report.Notes) output.WriteLine("  " + n);
                if (sub == "preview") return 0;

                var statBuilt = File.Exists(Path.Combine(settings.OutputModDir, ModBuilder.ManifestName)) ? File.GetLastWriteTimeUtc(Path.Combine(settings.OutputModDir, ModBuilder.ManifestName)) : (DateTime?)null;
                var spawnBuilt = Core.Spawn.SpawnWriter.BuiltAt(outDir);
                if (statBuilt != null && spawnBuilt != null && statBuilt > spawnBuilt) output.WriteLine("  알림: 스탯 출력 모드가 지난 스폰 생성보다 나중에 빌드됐다. 이번 생성이 그것을 반영한다");
                Directory.CreateDirectory(outDir);
                string info = Path.Combine(outDir, "mod.info");
                if (!File.Exists(info))
                    File.WriteAllText(info, "{mod\n\t{name \"CTAToybox Spawn\"}\n\t{tags \"Mod\"}\n\t{desc \"CTAToybox spawn menu: free skirmish roster with all units and a reinforcement menu in every map. Generated by the CTAToybox editor.\"}\n\t{minGameVersion \"any\"}\n\t{maxGameVersion \"any\"}\n}\n");
                var written = Core.Spawn.SpawnWriter.Write(outDir, files, settings.OutputModDir);
                output.WriteLine("썼다 " + written.Written.Count + ", 그대로 " + written.Unchanged.Count + ", 지웠다 " + written.Removed.Count + ", 오류 " + written.Errors.Count + ": " + outDir);
                foreach (string e in written.Errors.Take(20)) output.WriteLine("  오류 " + e);
                return written.Errors.Count > 0 ? 1 : 0;
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
                // 한글 이름: 저장소는 설정 파일 폴더(editor)의 부모
                if (!opt.ContainsKey("names")) names.AddTranslations(Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetFullPath(settingsPath))), "l10n", "ko"));
                foreach (string w in names.Warnings) output.WriteLine("경고: " + w);
                foreach (var kv in SettingFiles.LoadNames(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "settings", "files.psv")))
                    names.AddPathName(kv.Key, kv.Value.Name);
                var found = index.Search(opt.ContainsKey("find") ? opt["find"] : null, null, opt.ContainsKey("layer") ? opt["layer"] : null, names)
                    .Where(u => UnitKind.InEditor(u.EditorKind) && (u.Numbers > 0 || u.EditorKind == SettingFiles.Kind) && (kind == null || u.EditorKind == kind || u.Kind == kind)).ToList();
                if (opt.ContainsKey("army"))
                {
                    string army = opt["army"].ToLowerInvariant();
                    if (!ArmyIndex.IsArmy(army)) throw new UsageException("--army는 다음 가운데 하나다: " + string.Join(", ", ArmyIndex.Armies));
                    var armies = ArmyIndex.Build(stack, index.Units);
                    found = found.Where(u => armies.ArmyOf(u.Path) == army).ToList();
                }
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

        // 파일 사본(팩션 강화): add --kind file-copy --file <원본> --new <사본 가상 경로>
        static int AddFileCopy(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string projectPath = Need(opt, "project"), source = Need(opt, "file"), target = Need(opt, "new");
            using (var session = EditorSession.Open(settingsPath, Directory.GetCurrentDirectory(), null))
            {
                session.OpenProject(projectPath);
                try { session.AddFileCopy(opt.ContainsKey("unit") ? opt["unit"] : null, source, target); }
                catch (ArgumentException ex) { output.WriteLine(ex.Message); return 1; }
                output.WriteLine("사본을 넣었다: " + SourceStack.ToVirtual(source) + " → " + SourceStack.ToVirtual(target));
                return 0;
            }
        }

        // 줄 더하기(팩션 강화): add --kind add-line --file <파일> --path <블록 경로> [--occ N] --new "<줄>"
        static int AddLineCli(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string projectPath = Need(opt, "project"), file = Need(opt, "file"), block = Need(opt, "path"), line = Need(opt, "new");
            if (opt.ContainsKey("scope") && opt["scope"].ToLowerInvariant() != "shared") throw new UsageException("줄 더하기는 --scope shared만 된다");
            using (var session = EditorSession.Open(settingsPath, Directory.GetCurrentDirectory(), null))
            {
                session.OpenProject(projectPath);
                try { session.AddLineEdit(opt.ContainsKey("unit") ? opt["unit"] : null, file, block, opt.ContainsKey("occ") ? Number(opt, "occ") : 0, line); }
                catch (ArgumentException ex) { output.WriteLine(ex.Message); return 1; }
                output.WriteLine("줄을 넣었다: " + SourceStack.ToVirtual(file) + " " + block + " ← " + line);
                return 0;
            }
        }

        static int Add(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            if (opt.ContainsKey("kind") && opt["kind"] == FileCopy.Kind) return AddFileCopy(opt, settingsPath, output);
            if (opt.ContainsKey("kind") && opt["kind"] == AddLine.Kind) return AddLineCli(opt, settingsPath, output);
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

        // 일괄 규칙(F1): rule preview|add|list|edit|reapply|remove|enable|disable
        static int Rule(List<string> positional, Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string sub = positional.Count > 0 ? positional[0].ToLowerInvariant() : null;
            var known = new[] { "preview", "add", "list", "edit", "reapply", "remove", "enable", "disable" };
            if (sub == null || !known.Contains(sub)) throw new UsageException("rule 뒤에는 " + string.Join(", ", known) + " 가운데 하나를 쓴다");
            if (!File.Exists(settingsPath)) { output.WriteLine("설정 파일이 없다: " + settingsPath + ". 먼저 init을 실행한다."); return 1; }
            using (var session = EditorSession.Open(settingsPath, Directory.GetCurrentDirectory(), null))
            {
                if (sub != "preview" || opt.ContainsKey("project")) session.OpenProject(Need(opt, "project"));
                if (sub == "list")
                {
                    if (session.Rules.Count == 0) { output.WriteLine("규칙이 없다"); return 0; }
                    foreach (var r in session.Rules)
                        output.WriteLine(r.Id + "  " + (r.Enabled ? "켜짐" : "꺼짐") + "  수정 " + session.Project.Edits.Count(e => e.Rule == r.Id) + "건  " + r.Name + "  (" + r.Describe() + ")");
                    return 0;
                }
                if (sub == "preview" || sub == "add")
                {
                    var rule = RuleFrom(opt, null);
                    var preview = session.PreviewRule(rule);
                    WritePreview(preview, output);
                    if (sub == "preview") return 0;
                    var added = session.AddRule(rule);
                    output.WriteLine("규칙을 더했다: " + added.Id + " " + added.Name + " (수정 " + session.Project.Edits.Count(e => e.Rule == added.Id) + "건)");
                    return 0;
                }
                string id = Need(opt, "id");
                var existing = session.RuleOf(id);
                if (existing == null) { output.WriteLine("이 id의 규칙이 없다: " + id); return 1; }
                switch (sub)
                {
                    case "edit": session.EditRule(id, RuleFrom(opt, existing)); break;
                    case "reapply": session.ReapplyRule(id); break;
                    case "remove": session.RemoveRule(id); output.WriteLine("규칙을 지웠다: " + id); return 0;
                    case "enable": session.SetRuleEnabled(id, true); break;
                    case "disable": session.SetRuleEnabled(id, false); break;
                }
                output.WriteLine(sub + ": " + id + " " + session.RuleOf(id).Name + " (수정 " + session.Project.Edits.Count(e => e.Rule == id) + "건)");
                return 0;
            }
        }

        // 옵션으로 규칙을 만든다. baseRule이 있으면 주지 않은 칸은 그대로 둔다(rule edit)
        static BulkRule RuleFrom(Dictionary<string, string> opt, BulkRule baseRule)
        {
            var r = baseRule == null ? new BulkRule() : baseRule.Copy();
            if (opt.ContainsKey("name")) r.Name = opt["name"];
            if (opt.ContainsKey("kind")) r.Kind = opt["kind"];
            if (opt.ContainsKey("path")) r.Paths = opt["path"].Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
            if (opt.ContainsKey("layer")) r.Layer = opt["layer"];
            if (opt.ContainsKey("army")) r.Army = opt["army"].ToLowerInvariant();
            if (opt.ContainsKey("any")) r.Any = opt["any"].Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
            if (opt.ContainsKey("items")) r.Items = opt["items"].ToLowerInvariant();
            if (opt.ContainsKey("exclude")) r.Exclude = opt["exclude"].Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
            if (opt.ContainsKey("key")) r.Key = opt["key"];
            if (opt.ContainsKey("arg")) r.Arg = opt["arg"].ToLowerInvariant() == "all" ? -1 : Number(opt, "arg");
            if (opt.ContainsKey("scope")) r.Scope = opt["scope"].ToLowerInvariant();
            var ops = new[] { "set", "mul", "add" }.Where(opt.ContainsKey).ToList();
            if (ops.Count > 1) throw new UsageException("--set, --mul, --add 가운데 하나만 쓴다");
            if (ops.Count == 1) { r.Op = ops[0]; r.Value = opt[ops[0]]; }
            string bad = BulkRules.Check(r);
            if (bad != null) throw new UsageException(bad);
            return r;
        }

        static void WritePreview(RulePreview p, TextWriter output)
        {
            output.WriteLine("바뀜 " + p.Changes.Count + "건(유닛 " + p.Units + " / 대상 " + p.Targets + "), 수정 " + p.Edits.Count + "건, 건너뜀 " + p.Skipped.Count + ", 대상 밖 " + p.OutsideUnits);
            foreach (var c in p.Changes.Take(20)) output.WriteLine("  " + c.Unit + "  " + c.Old + " → " + c.New + "  (" + (c.Scope == "unit" ? "이 유닛만" : "전부") + ", " + c.File + ")");
            if (p.Changes.Count > 20) output.WriteLine("  … 외 " + (p.Changes.Count - 20) + "건");
            foreach (var g in p.Skipped.GroupBy(k => k.Reason).OrderByDescending(g => g.Count())) output.WriteLine("  건너뜀 " + g.Count() + ": " + g.Key + "  (예: " + string.Join(", ", g.Select(k => k.Unit).Distinct().Take(3)) + ")");
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
