// 프로젝트의 수정을 원본에 적용해 출력 모드 폴더의 resource 아래에 낱개 파일로 쓴다.
// 지우는 것은 이전 빌드의 목록 파일에 적혀 있고 resource 아래에 있는 파일뿐이다. 손으로 넣은 파일은 덮어쓰지도 않는다.
// 새 파일은 임시 폴더에 다 만든 뒤 옮긴다. 옮기기 전에 목록 파일에 이전 생성물과 이번 생성물을 모두 적어 두므로,
// 옮기다 실패해도 다음 빌드가 남은 파일을 지울 수 있다.
// scope가 unit인 수정: 값이 유닛 자신의 파일에 있으면 그 파일을 고친다. include한 파일에 있으면 공유 파일은 두고,
// 유닛에서 그 파일까지의 include 사슬을 유닛 전용 사본으로 만든 뒤 include 경로를 사본으로 고쳐 쓴다(IncludeChain).
// 사본을 하나라도 만들지 못하면 그 유닛의 include는 고쳐 쓰지 않는다: 없는 파일을 include하는 유닛을 내보내지 않는다.
// .lua 파일(컨퀘스트 AI 설정)은 LuaPatcher로, 나머지는 TokenPatcher로 고친다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class BuildReport
    {
        public List<EditResult> Results { get; } = new List<EditResult>();
        public List<string> WrittenFiles { get; } = new List<string>();
        public List<string> RemovedFiles { get; } = new List<string>();
        public List<string> FileErrors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public int Count(EditStatus status) { return Results.Count(r => r.Status == status); }
    }

    // 목록 파일(.ctatoybox-build.json)의 형식
    public sealed class BuildManifestFile { public string Path { get; set; } public string Layer { get; set; } public int Edits { get; set; } }
    public sealed class BuildManifest { public string Project { get; set; } public string BuiltAt { get; set; } public List<BuildManifestFile> Files { get; set; } = new List<BuildManifestFile>(); }

    public sealed class ModBuilder
    {
        public const string ManifestName = ".ctatoybox-build.json";

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        readonly SourceStack stack;
        readonly string modDir, resourceDir;
        // 출력 상대 경로 → 그 파일에 담긴 수정. 파일을 쓰지 못했을 때 어느 수정이 적용되지 않았는지 찾는 데 쓴다
        readonly Dictionary<string, List<Edit>> owners = new Dictionary<string, List<Edit>>(StringComparer.OrdinalIgnoreCase);
        UnitTracer tracer;   // 원형에서 물려받은 값을 찾을 때만 만든다(E14)

        // 출력 파일 하나를 만드는 데 필요한 것
        sealed class WorkItem
        {
            public string Output;                                      // 출력 가상 경로
            public SourceFile Source;
            public List<Edit> Edits = new List<Edit>();                // 결과를 보고하는 수정
            public List<Edit> Silent = new List<Edit>();               // 사본에 함께 넣는 '전부' 수정. 결과는 원래 파일 쪽에서 보고한다
            public List<RawPatch> Raw = new List<RawPatch>();          // include 경로 고쳐 쓰기
            public bool Always;                                        // 적용된 수정이 없어도 쓴다(사슬 중간의 사본)
            public List<KeyValuePair<Edit, TraceRow>> Appends = new List<KeyValuePair<Edit, TraceRow>>();   // 원형에서 물려받은 값의 '이 유닛만': 맨 위 블록에 덧붙일 줄(E14)
        }

        // 한 유닛의 '이 유닛만' 수정이 만드는 사본들과 유닛 파일의 include 고쳐 쓰기
        sealed class UnitPlan
        {
            public string Unit;
            public List<WorkItem> Copies = new List<WorkItem>();
            public List<RawPatch> UnitRaw = new List<RawPatch>();
            public List<BuildManifestFile> Produced = new List<BuildManifestFile>();
            public List<EditResult> Results = new List<EditResult>();
        }

        public ModBuilder(SourceStack stack, string outputModDir)
        {
            this.stack = stack;
            modDir = Path.GetFullPath(outputModDir);
            resourceDir = Path.Combine(modDir, "resource");
        }

        public BuildReport Build(EditProject project)
        {
            var report = new BuildReport();
            owners.Clear();
            string stage = Path.Combine(Path.GetTempPath(), "ctatoybox-build-" + Guid.NewGuid().ToString("N"));
            var manifest = new BuildManifest { Project = project.Name, BuiltAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") };
            var previous = ReadPrevious(report);
            if (previous == null) return report;
            // 추적기(이름 바꾸기 점검, 원형 사슬)가 복제를 넣은 본문을 읽게 한다. 세션 없이(명령줄) 빌드해도 같다. 끝나면 되돌린다
            var oldOverlay = stack.Overlay;
            stack.Overlay = TextCopies.Overlay(project);
            try
            {
                // 1. 임시 폴더에 고친 파일을 만든다
                var enabled = project.Edits.Where(e => e.Enabled).ToList();
                // 블록 복제(E18)·매크로 복제: 원본에 넣어 보아 적용되는 새 이름을 모으고, 그 이름에 기대는 수정 가운데 복제가 없는 것은 막는다
                var copyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var g in enabled.Where(e => TextCopies.IsCopy(e.Kind)).GroupBy(e => SourceStack.ToVirtual(e.File ?? "")))
                {
                    var src = stack.Find(g.Key);
                    if (src == null) continue;
                    TextKind copyKind;
                    var copyResults = new List<EditResult>();
                    TextCopies.Apply(TextCodec.Decode(stack.Read(src), out copyKind), g, copyResults);
                    foreach (var r in copyResults.Where(r => r.Status == EditStatus.Applied)) copyNames.Add(r.Edit.New.Trim('"'));
                }
                foreach (var e in enabled.ToList())
                {
                    string dependency = TextCopies.Dependency(e, project.Edits);
                    if (dependency == null || copyNames.Contains(dependency)) continue;
                    report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = TextCopies.MissingNote(dependency, project.Edits) });
                    enabled.Remove(e);
                }
                // 매크로 호출 이름 바꾸기(E4): 새 이름이 그 유닛을 펼칠 때 같은 파일에 정의된 이름이어야 한다. 아니면 게임이 시작할 때 꺼진다
                foreach (var e in enabled.Where(x => x.Kind == MacroSwap.Kind).ToList())
                {
                    string swap = MacroSwap.Check((tracer ?? (tracer = new UnitTracer(stack))).Trace(SourceStack.ToVirtual(e.Unit ?? e.File ?? "")), e);
                    if (swap == null) continue;
                    report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = swap });
                    enabled.Remove(e);
                }
                // 파일 사본(팩션 강화): 만들 수 있는 사본만 원본 위에 등록한다. 그래야 사본을 가리키는 수정이 사본 본문을 읽고, 빠진 사본은 "파일이 없다"로 막힌다
                var items = new Dictionary<string, WorkItem>(StringComparer.Ordinal);
                var goodCopies = new List<Edit>();
                stack.SetCopies(null);
                foreach (var e in enabled.Where(x => x.Kind == FileCopy.Kind).ToList())
                {
                    string bad = FileCopy.CheckTarget(stack, e.File, e.New) ?? (goodCopies.Any(c => SourceStack.ToVirtual(c.New) == SourceStack.ToVirtual(e.New)) ? "같은 사본이 이미 있다: " + e.New : null);
                    enabled.Remove(e);
                    if (bad != null) { report.Results.Add(new EditResult { Edit = e, Status = stack.FindReal(e.File ?? "") == null ? EditStatus.TargetMissing : EditStatus.Invalid, Message = bad }); continue; }
                    goodCopies.Add(e);
                }
                stack.SetCopies(goodCopies.Select(e => new KeyValuePair<string, string>(e.New, e.File)));
                foreach (var e in goodCopies)
                {
                    var item = ItemFor(items, SourceStack.ToVirtual(e.New), stack.Find(e.New));
                    item.Always = true;
                    item.Edits.Add(e);
                }
                var shared = enabled.Where(e => e.Scope != "unit").ToList();
                foreach (var group in shared.GroupBy(e => SourceStack.ToVirtual(e.File ?? "")))
                {
                    var source = stack.Find(group.Key);
                    if (source == null)
                    {
                        foreach (var e in group) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없다: " + group.Key });
                        continue;
                    }
                    ItemFor(items, group.Key, source).Edits.AddRange(group);
                }

                // 1-1. '이 유닛만' 수정: 사본을 먼저 만든다. 성공한 유닛만 유닛 파일의 include를 고쳐 쓴다
                var plans = new List<UnitPlan>();
                foreach (var group in enabled.Where(e => e.Scope == "unit").GroupBy(e => SourceStack.ToVirtual(e.Unit ?? "")))
                {
                    var plan = PlanUnit(group.Key, group.ToList(), shared, items, report);
                    if (plan == null) continue;
                    string failure = null;
                    foreach (var copy in plan.Copies)
                    {
                        string error;
                        try { error = StageItem(copy, stage, previous, plan.Results, plan.Produced); }
                        catch (Exception ex) { error = ex.Message; }
                        if (error != null) { failure = copy.Output + ": " + error; break; }
                    }
                    if (failure != null)
                    {
                        report.FileErrors.Add(failure);
                        FailPlan(plan, "이 유닛의 사본을 만들지 못했다: " + failure);
                    }
                    else
                    {
                        ItemFor(items, plan.Unit, stack.Find(plan.Unit)).Raw.AddRange(plan.UnitRaw);
                        plans.Add(plan);
                    }
                    report.Results.AddRange(plan.Results);
                }

                // 1-2. 나머지 파일(원본과 같은 경로에 쓰는 것)
                foreach (var item in items.Values)
                {
                    var results = new List<EditResult>();
                    var produced = new List<BuildManifestFile>();
                    string error;
                    try { error = StageItem(item, stage, previous, results, produced); }
                    catch (Exception ex) { error = ex.Message; }
                    if (error != null)
                    {
                        // 파일을 만들지 못했으면 적용으로 세지 않는다. 다른 상태의 결과는 그대로 둔다
                        report.FileErrors.Add(item.Output + ": " + error);
                        foreach (var r in results.Where(x => x.Status == EditStatus.Applied)) { r.Status = EditStatus.Invalid; r.Message = "파일을 만들지 못했다: " + error; }
                        foreach (var e in item.Edits.Where(x => !results.Any(r => r.Edit == x)))
                            results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "파일을 만들지 못했다: " + error });
                        // 유닛 파일을 못 썼으면 그 유닛의 사본은 쓰이지 않는다. 남기지 않는다
                        foreach (var plan in plans.Where(p => p.Unit == item.Output)) FailPlan(plan, "유닛 파일을 만들지 못했다: " + error);
                    }
                    else manifest.Files.AddRange(produced);
                    report.Results.AddRange(results);
                }
                foreach (var plan in plans) manifest.Files.AddRange(plan.Produced);

                // 1-3a. 쓴 아이템 파일이 {from}으로 물려받는 파일(원형 사슬)을 이긴 판 그대로 같이 쓴다(2026-10-17 게임이 꺼졌다: 출력 모드의 사본
                // set/stuff/gun/20mm_flak38_x4_2_ctb_ger의 {from "20mm_flak38_2"}가 "pattern not found". 원형은 MACE Wunderwaffe 폴더에만 있었다)
                var writtenPaths = new HashSet<string>(manifest.Files.Select(f => f.Path.Substring("resource/".Length)), StringComparer.OrdinalIgnoreCase);
                foreach (string written in writtenPaths.Where(p => p.StartsWith("set/stuff/", StringComparison.Ordinal)).OrderBy(p => p, StringComparer.Ordinal).ToList())
                {
                    var traced = (tracer ?? (tracer = new UnitTracer(stack))).Trace(written);
                    if (traced == null) continue;
                    foreach (var level in traced.From)
                    {
                        string parent = SourceStack.ToVirtual(level.File ?? "");
                        if (parent.Length == 0 || writtenPaths.Contains(parent)) continue;
                        var parentSource = stack.FindReal(parent);
                        if (parentSource == null) continue;
                        var produced = new List<BuildManifestFile>();
                        string error;
                        try { error = StageItem(new WorkItem { Output = parent, Source = parentSource, Always = true }, stage, previous, new List<EditResult>(), produced); }
                        catch (Exception ex) { error = ex.Message; }
                        if (error != null) report.FileErrors.Add(parent + ": " + error);
                        else { manifest.Files.AddRange(produced); writtenPaths.Add(parent); }
                    }
                }

                // 1-3. 쓴 파일의 폴더에 .presets가 있으면 이긴 판을 그대로 같이 쓴다(2026-10-17 게임이 꺼졌다: 출력 모드의 사본
                // set/stuff/bazooka/raketenwerfer43_ctb_ger가 MACE pak의 bazooka/.presets에만 있는 "penetration_short"를 "define not found").
                // 같은 내용이라 다른 파일이 읽는 정의는 그대로다
                var presetDirs = manifest.Files.Select(f => f.Path.Substring("resource/".Length)).Where(p => p.Contains('/'))
                    .Select(p => p.Substring(0, p.LastIndexOf('/'))).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(d => d, StringComparer.Ordinal).ToList();
                foreach (string dir in presetDirs)
                {
                    string presetPath = dir + "/.presets";
                    if (manifest.Files.Any(f => string.Equals(f.Path, "resource/" + presetPath, StringComparison.OrdinalIgnoreCase))) continue;
                    var presetSource = stack.FindReal(presetPath);
                    if (presetSource == null) continue;
                    var produced = new List<BuildManifestFile>();
                    string error;
                    try { error = StageItem(new WorkItem { Output = presetPath, Source = presetSource, Always = true }, stage, previous, new List<EditResult>(), produced); }
                    catch (Exception ex) { error = ex.Message; }
                    if (error != null) report.FileErrors.Add(presetPath + ": " + error);
                    else manifest.Files.AddRange(produced);
                }

                // 2. 이전 생성물과 이번 생성물을 모두 적은 목록 파일을 먼저 쓴다
                var fresh = new HashSet<string>(manifest.Files.Select(x => x.Path), StringComparer.OrdinalIgnoreCase);
                var stale = previous.Values.Where(x => !fresh.Contains(x.Path)).ToList();
                if (!WriteManifest(manifest, manifest.Files.Concat(stale), report)) return report;

                // 3. 새 파일을 옮긴다
                foreach (var f in manifest.Files)
                {
                    try
                    {
                        string to = FullPath(f.Path);
                        Directory.CreateDirectory(Path.GetDirectoryName(to));
                        File.Copy(Path.Combine(stage, f.Path.Replace('/', Path.DirectorySeparatorChar)), to, true);
                        report.WrittenFiles.Add(f.Path);
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        string virtualPath = f.Path.Substring("resource/".Length);
                        report.FileErrors.Add(virtualPath + ": " + ex.Message);
                        List<Edit> owned;
                        if (owners.TryGetValue(f.Path, out owned))
                            foreach (var r in report.Results.Where(x => x.Status == EditStatus.Applied && owned.Contains(x.Edit)))
                            { r.Status = EditStatus.Invalid; r.Message = "파일을 쓰지 못했다: " + ex.Message; }
                    }
                }

                // 4. 이번에 만들지 않은 이전 생성물을 지우고 목록 파일을 마무리한다. 지우지 못한 것은 목록에 남긴다
                var kept = new List<BuildManifestFile>(manifest.Files);
                foreach (var f in stale)
                {
                    try { Remove(f.Path, report); }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        report.FileErrors.Add(f.Path + ": 지우지 못했다: " + ex.Message);
                        kept.Add(f);
                    }
                }
                WriteManifest(manifest, kept, report);
            }
            finally
            {
                stack.Overlay = oldOverlay;
                if (Directory.Exists(stage)) Directory.Delete(stage, true);
            }
            return report;
        }

        static WorkItem ItemFor(Dictionary<string, WorkItem> items, string virtualPath, SourceFile source)
        {
            WorkItem item;
            if (!items.TryGetValue(virtualPath, out item)) { item = new WorkItem { Output = virtualPath, Source = source }; items[virtualPath] = item; }
            return item;
        }

        static bool SameSlot(Edit a, Edit b)
        {
            return a.Occurrence == b.Occurrence && a.Arg == b.Arg && string.Equals(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
        }

        // from 파일 안에서 사본으로 바뀐 파일을 가리키는 include 경로들. 같은 지시문이 펼치기에서 여러 번 지나가도 한 번만 낸다
        static List<RawPatch> Rewrites(List<SdlIncludeEdge> edges, string from, HashSet<string> copySet, string unit)
        {
            var result = new List<RawPatch>();
            var seen = new HashSet<int>();
            foreach (var e in edges)
                if (e.FromFile == from && copySet.Contains(e.ToFile) && seen.Add(e.Index))
                    result.Add(new RawPatch { Index = e.Index, Old = e.Raw, New = IncludeChain.RewriteRaw(e.Raw, IncludeChain.CopyName(e.ToFile, unit)) });
            return result;
        }

        void FailPlan(UnitPlan plan, string message)
        {
            foreach (var r in plan.Results.Where(x => x.Status == EditStatus.Applied)) { r.Status = EditStatus.Invalid; r.Message = message; }
            foreach (var e in plan.Copies.SelectMany(c => c.Edits).Where(x => !plan.Results.Any(r => r.Edit == x)))
                plan.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = message });
            foreach (var f in plan.Produced) owners.Remove(f.Path);
            plan.Produced.Clear();
        }

        // 한 유닛의 '이 유닛만' 수정을 계획한다. 사본이 필요 없으면 null.
        // 유닛 자신의 파일에 있는 값은 items에 바로 넣는다. 적용할 수 없는 수정은 여기서 보고한다
        UnitPlan PlanUnit(string unit, List<Edit> edits, List<Edit> shared, Dictionary<string, WorkItem> items, BuildReport report)
        {
            var unitSource = stack.Find(unit);
            if (unitSource == null)
            {
                foreach (var e in edits) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 유닛 파일이 없다: " + unit });
                return null;
            }
            var edges = new SdlResolver(stack.ReadText).Resolve(unit).IncludeEdges;
            UnitTraceResult traced = null;
            var live = new Dictionary<string, List<Edit>>(StringComparer.Ordinal);
            var copySet = new HashSet<string>(StringComparer.Ordinal);
            foreach (var group in edits.GroupBy(e => SourceStack.ToVirtual(e.File ?? "")))
            {
                string file = group.Key;
                // 유닛 자신의 파일: 같은 자리를 '전부' 수정도 고치면 '이 유닛만'이 이긴다(앞에 둔 수정이 이긴다). 화면의 EditorSession.EditFor와 같다
                if (file == unit) { ItemFor(items, unit, unitSource).Edits.InsertRange(0, group); continue; }
                // 원형에서 물려받은 값: 사본 대신 유닛 파일의 맨 위 블록에 같은 키를 덧붙인다(E14). 규칙은 화면·점검과 같은 UnitScope다
                if (traced == null) traced = (tracer ?? (tracer = new UnitTracer(stack))).Trace(unit);
                var rest = new List<Edit>();
                foreach (var e in group)
                {
                    var row = UnitScope.InheritedRow(traced, file, e.Path, e.Occurrence, e.Arg);
                    if (row == null) { rest.Add(e); continue; }
                    string inheritedBlocker = UnitScope.InheritedBlocker(traced, row);
                    if (inheritedBlocker != null) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = inheritedBlocker });
                    else ItemFor(items, unit, unitSource).Appends.Add(new KeyValuePair<Edit, TraceRow>(e, row));
                }
                if (rest.Count == 0) continue;
                var source = stack.Find(file);
                if (source == null)
                {
                    foreach (var e in rest) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없다: " + file });
                    continue;
                }
                var allowed = new List<Edit>();
                foreach (var e in rest)
                {
                    string blocker = IncludeChain.UnitScopeBlocker(edges, unit, file, e.Path);
                    if (blocker != null) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = blocker });
                    else allowed.Add(e);
                }
                if (allowed.Count == 0) continue;
                // 미리 적용해 본다. 하나도 적용되지 않으면 사본을 만들지 않는다
                var probe = new List<EditResult>();
                TextKind kind;
                string text = TextCodec.Decode(stack.Read(source), out kind);
                try { TokenPatcher.Apply(text, allowed, probe); }
                catch (InvalidOperationException)
                {
                    // 키 경로를 바꾸는 수정(예전 프로젝트의 블록 한정자 등)이 하나라도 있으면 전체가 던진다. 하나씩 해 보고 던지는 것만 뺀다
                    var usable = new List<Edit>();
                    foreach (var e in allowed)
                    {
                        try { TokenPatcher.Apply(text, new[] { e }, new List<EditResult>()); usable.Add(e); }
                        catch (InvalidOperationException ex) { report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = ex.Message }); }
                    }
                    allowed = usable;
                    probe.Clear();
                    if (allowed.Count == 0) continue;
                    try { TokenPatcher.Apply(text, allowed, probe); }
                    catch (InvalidOperationException ex)
                    {
                        foreach (var e in allowed) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = ex.Message });
                        continue;
                    }
                }
                if (!probe.Any(r => r.Status == EditStatus.Applied)) { report.Results.AddRange(probe); continue; }
                live[file] = allowed;
                copySet.UnionWith(IncludeChain.CopySet(edges, unit, file));
            }
            if (copySet.Count == 0) return null;

            var plan = new UnitPlan { Unit = unit };
            foreach (string file in copySet.OrderBy(x => x, StringComparer.Ordinal))
            {
                var item = new WorkItem { Output = IncludeChain.CopyName(file, unit), Source = stack.Find(file), Always = true };
                List<Edit> own;
                if (live.TryGetValue(file, out own)) item.Edits.AddRange(own);
                item.Silent.AddRange(shared.Where(s => SourceStack.ToVirtual(s.File ?? "") == file && !item.Edits.Any(u => SameSlot(u, s))));
                item.Raw.AddRange(Rewrites(edges, file, copySet, unit));
                plan.Copies.Add(item);
            }
            plan.UnitRaw.AddRange(Rewrites(edges, unit, copySet, unit));
            return plan;
        }

        // 원본 하나에 수정과 include 고쳐 쓰기를 적용해 임시 폴더에 쓴다. 파일을 만들 수 없으면 이유를 돌려준다
        string StageItem(WorkItem item, string stage, Dictionary<string, BuildManifestFile> previous, List<EditResult> results, List<BuildManifestFile> produced)
        {
            byte[] original = stack.Read(item.Source);
            TextKind kind;
            string text = TextCodec.Decode(original, out kind);
            if (!original.AsSpan().SequenceEqual(TextCodec.Encode(text, kind)))
                return "원본을 읽고 다시 쓰면 바이트가 달라진다(인코딩이 깨진 파일). 고치지 않는다";

            // 블록 복제(E18)·매크로 복제를 먼저 넣고, 나머지 수정은 그 본문에 적용한다(복제는 같은 부모의 마지막 형제 뒤나 파일 끝이라 다른 자리의 순번이 그대로다)
            var copyEdits = item.Edits.Where(e => TextCopies.IsCopy(e.Kind)).ToList();
            int copiedCount = 0;
            if (copyEdits.Count > 0)
            {
                var copyResults = new List<EditResult>();
                text = TextCopies.Apply(text, copyEdits, copyResults);
                results.AddRange(copyResults);
                copiedCount = copyResults.Count(r => r.Status == EditStatus.Applied);
            }
            // 파일 사본은 이 파일을 쓰는 것 자체다(item.Always)
            foreach (var e in item.Edits.Where(x => x.Kind == FileCopy.Kind))
            {
                results.Add(new EditResult { Edit = e, Status = EditStatus.Applied });
                copiedCount++;
            }
            var usable = new List<Edit>();
            foreach (var e in item.Edits.Where(x => !TextCopies.IsCopy(x.Kind) && x.Kind != FileCopy.Kind && x.Kind != AddLine.Kind))
            {
                string ruleWarning;
                string ruleError = e.New == null ? null : ValueRules.CheckNewValue(item.Source.VirtualPath, e.Old, e.New, out ruleWarning);
                if (e.New != null && !TextCodec.CanEncode(e.New, kind))
                    results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "새 값에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + e.New });
                else if (ruleError != null)
                    results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = ruleError });
                else usable.Add(e);
            }
            foreach (var p in item.Raw)
                if (!TextCodec.CanEncode(p.New, kind)) return "include 경로를 이 파일의 인코딩(" + kind + ")으로 쓸 수 없다: " + p.New;

            // '이 유닛만' 수정을 앞에 둔다: 같은 자리를 '전부' 수정도 고치면 앞의 것이 이긴다
            var all = usable.Concat(item.Silent.Where(s => s.New != null && TextCodec.CanEncode(s.New, kind))).ToList();
            var applied = new List<EditResult>();
            // 적용이 검증에서 던져도 그때까지의 결과는 보고한다(호출한 쪽이 적용을 적용 불가로 바꾼다)
            var reported = new List<EditResult>();
            string patched;
            try { patched = LuaTable.IsLua(item.Source.VirtualPath) ? LuaPatcher.Apply(text, all, applied) : TokenPatcher.Apply(text, all, applied, item.Raw); }
            finally
            {
                reported.AddRange(applied.Where(r => item.Edits.Contains(r.Edit)));
                results.AddRange(reported);
            }
            int count = reported.Count(r => r.Status == EditStatus.Applied) + copiedCount;
            var appended = item.Appends.Count == 0 ? new List<Edit>() : Append(ref patched, item, kind, results);
            count += appended.Count;
            // 줄 더하기(팩션 강화)는 값 고치기를 다 한 본문에 넣는다. 넣은 줄이 같은 경로의 다른 줄 순번을 밀지 않게
            var lineEdits = item.Edits.Where(e => e.Kind == AddLine.Kind).ToList();
            if (lineEdits.Count > 0)
            {
                var lineResults = new List<EditResult>();
                if (LuaTable.IsLua(item.Source.VirtualPath)) lineResults.AddRange(lineEdits.Select(e => new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "Lua 파일에는 줄을 더하지 않는다" }));
                else
                {
                    foreach (var e in lineEdits.Where(e => !TextCodec.CanEncode(e.New ?? "", kind)))
                        lineResults.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "넣을 줄에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + e.New });
                    patched = AddLine.Apply(patched, lineEdits.Where(e => TextCodec.CanEncode(e.New ?? "", kind)), lineResults);
                }
                results.AddRange(lineResults);
                count += lineResults.Count(r => r.Status == EditStatus.Applied);
            }
            if (count == 0 && item.Raw.Count == 0 && !item.Always) return null;

            string relative = "resource/" + item.Output;
            if (FullPath(relative) == null) return "출력 폴더의 resource 밖을 가리키는 경로다";
            if (File.Exists(FullPath(relative)) && !previous.ContainsKey(relative)) return "출력 폴더에 이 도구가 만들지 않은 같은 경로의 파일이 있다. 덮어쓰지 않는다: " + relative;
            string target = Path.Combine(stage, relative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.WriteAllBytes(target, TextCodec.Encode(patched, kind));
            produced.Add(new BuildManifestFile { Path = relative, Layer = item.Source.Layer, Edits = count });
            owners[relative] = item.Edits.Concat(appended).ToList();
            return null;
        }

        // 원형에서 물려받은 값의 '이 유닛만'(E14): 원형의 지금 값이 수정 당시와 같고 새 값 규칙에 맞는 것만 줄 단위로 모아 맨 위 블록 끝에 덧붙인다.
        // 덧붙인 뒤 다시 읽어 그 키가 맨 위 블록에 새 값으로 있는지 본다. 아니면 던진다(호출한 쪽이 이 파일의 수정을 적용 불가로 바꾼다)
        static List<Edit> Append(ref string text, WorkItem item, TextKind kind, List<EditResult> results)
        {
            var ok = new List<KeyValuePair<Edit, TraceRow>>();
            foreach (var a in item.Appends)
            {
                var e = a.Key;
                string now = e.Arg >= 0 && e.Arg < a.Value.Values.Count ? a.Value.Values[e.Arg].Text : null;
                string warning;
                string rule = e.New == null ? "새 값이 없다" : ValueRules.CheckNewValue(e.File, e.Old, e.New, out warning);
                if (now != e.Old) results.Add(new EditResult { Edit = e, Status = EditStatus.SourceChanged, Message = "원형의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + now });
                else if (rule != null) results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = rule });
                else if (!TextCodec.CanEncode(e.New, kind)) results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "새 값에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + e.New });
                else ok.Add(a);
            }
            if (ok.Count == 0) return new List<Edit>();
            var lines = ok.GroupBy(a => a.Value).Select(g => new { Row = g.Key, Line = UnitScope.AppendLine(g.Key, g.Select(x => x.Key)) }).ToList();
            var parsed = SdlCore.Parse(text);
            int close = TopClose(parsed);
            if (close < 0) throw new InvalidOperationException("유닛 파일의 맨 위 {from …} 블록을 찾지 못해 원형의 값을 덧붙일 수 없다");
            // 파일의 줄바꿈을 따른다. 닫는 } 가 줄 끝에 붙어 있으면 줄을 먼저 바꾼다
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            string lead = close > 0 && text[close - 1] != '\n' ? nl : "";
            string patched = text.Substring(0, close) + lead + string.Concat(lines.Select(l => "\t" + l.Line + nl)) + text.Substring(close);
            var check = SdlCore.Parse(patched);
            if (check.Errors.Count > 0 || !SdlCore.RoundTrip(check)) throw new InvalidOperationException("원형의 값을 덧붙인 파일이 다시 파싱되지 않는다");
            var rows = KeyLocator.Rows(check.Root);
            foreach (var l in lines)
            {
                var last = rows.LastOrDefault(r => string.Equals(r.Path, l.Row.Path, StringComparison.OrdinalIgnoreCase));
                var expected = UnitScope.AppendLine(l.Row, ok.Where(x => x.Value == l.Row).Select(x => x.Key));
                if (last == null || "{" + KeyLabel.Split(last.RawPath).Last() + " " + string.Join(" ", last.Leaves.Select(x => x.Text)) + "}" != expected)
                    throw new InvalidOperationException("덧붙인 뒤 맨 위 블록에 그 키가 새 값으로 없다: " + l.Row.Path);
            }
            text = patched;
            foreach (var a in ok) results.Add(new EditResult { Edit = a.Key, Status = EditStatus.Applied });
            return ok.Select(a => a.Key).ToList();
        }

        // 맨 위 {from …} 블록을 닫는 } 의 글자 자리. 없으면 -1
        static int TopClose(SdlParseResult parsed)
        {
            var tokens = parsed.Tokens;
            for (int i = 0; i + 1 < tokens.Count; i++)
            {
                if (tokens[i].Kind != "open" || tokens[i].Text != "{" || !string.Equals(tokens[i + 1].Text, "from", StringComparison.OrdinalIgnoreCase)) continue;
                int depth = 0;
                for (int j = i; j < tokens.Count; j++)
                {
                    if (tokens[j].Kind == "open") depth++;
                    else if (tokens[j].Kind == "close" && --depth == 0) return tokens[j].Index;
                }
                return -1;
            }
            return -1;
        }

        // 출력 모드 폴더 기준 상대 경로의 전체 경로. resource 아래가 아니면 null
        string FullPath(string relative)
        {
            if (string.IsNullOrEmpty(relative) || Path.IsPathRooted(relative)) return null;
            string full;
            try { full = Path.GetFullPath(Path.Combine(modDir, relative.Replace('/', Path.DirectorySeparatorChar))); }
            catch (ArgumentException) { return null; }
            return full.StartsWith(resourceDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ? full : null;
        }

        bool WriteManifest(BuildManifest manifest, IEnumerable<BuildManifestFile> files, BuildReport report)
        {
            var copy = new BuildManifest { Project = manifest.Project, BuiltAt = manifest.BuiltAt, Files = files.ToList() };
            try
            {
                Directory.CreateDirectory(modDir);
                File.WriteAllText(Path.Combine(modDir, ManifestName), JsonSerializer.Serialize(copy, Json) + "\n", new UTF8Encoding(false));
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                report.FileErrors.Add(ManifestName + ": 목록 파일을 쓰지 못했다: " + ex.Message);
                return false;
            }
        }

        // 수정 점검(EditorSession.CheckEdits)이 빌드와 같은 규칙으로 보는 출력 충돌: 출력 가상 경로마다
        // 이 도구가 만들지 않은 같은 경로의 파일이 출력 폴더에 있으면 StageItem과 같은 문장을, 아니면 null을 돌려준다.
        // 목록 파일이 깨졌으면 아무것도 막지 않는다(빌드가 목록 파일 오류로 멈추고 알린다)
        public Func<string, string> OutputConflicts()
        {
            var previous = ReadPrevious(new BuildReport());
            return virtualPath =>
            {
                if (previous == null || string.IsNullOrEmpty(virtualPath)) return null;
                string relative = "resource/" + virtualPath;
                string full = FullPath(relative);
                if (full == null || !File.Exists(full) || previous.ContainsKey(relative)) return null;
                return "출력 폴더에 이 도구가 만들지 않은 같은 경로의 파일이 있다. 덮어쓰지 않는다: " + relative;
            };
        }

        // 이전 빌드의 목록 파일에서 지워도 되는 항목(출력 모드의 resource 아래)만 읽는다.
        // 목록 파일이 깨졌으면 null: 그대로 빌드하면 이전 생성물의 기록을 덮어써 다시는 지우지 못하므로 빌드를 멈춘다
        Dictionary<string, BuildManifestFile> ReadPrevious(BuildReport report)
        {
            var result = new Dictionary<string, BuildManifestFile>(StringComparer.OrdinalIgnoreCase);
            string manifestPath = Path.Combine(modDir, ManifestName);
            if (!File.Exists(manifestPath)) return result;
            BuildManifest old;
            bool hasList;
            try
            {
                string json = File.ReadAllText(manifestPath, Encoding.UTF8);
                // 이 도구는 늘 파일 목록(files 배열)을 쓴다. 목록이 없으면(null, {}) 깨진 것과 같이 다룬다. {}는 역직렬화하면 빈 목록이 되므로 원문에서 본다
                using (var doc = JsonDocument.Parse(json))
                {
                    JsonElement files;
                    hasList = doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("files", out files) && files.ValueKind == JsonValueKind.Array;
                }
                old = JsonSerializer.Deserialize<BuildManifest>(json, Json);
            }
            catch (JsonException)
            {
                report.FileErrors.Add(ManifestName + ": 이전 빌드의 목록 파일을 읽지 못해 빌드를 멈췄다. 목록에 있던 생성물을 확인해 지우고 목록 파일도 지운 뒤 다시 빌드한다: " + manifestPath);
                return null;
            }
            if (!hasList || old == null || old.Files == null)
            {
                report.FileErrors.Add(ManifestName + ": 이전 빌드의 목록 파일에 파일 목록이 없어 빌드를 멈췄다. 목록에 있던 생성물을 확인해 지우고 목록 파일도 지운 뒤 다시 빌드한다: " + manifestPath);
                return null;
            }
            foreach (var f in old.Files)
            {
                if (f == null || FullPath(f.Path) == null)
                {
                    report.Warnings.Add("목록 파일의 항목이 출력 폴더의 resource 밖을 가리킨다. 지우지 않는다: " + (f == null ? "null" : f.Path));
                    continue;
                }
                f.Path = f.Path.Replace('\\', '/');
                result[f.Path] = f;
            }
            return result;
        }

        void Remove(string relative, BuildReport report)
        {
            string full = FullPath(relative);
            if (full == null || !File.Exists(full)) return;
            File.Delete(full);
            report.RemovedFiles.Add(relative);
            // 비게 된 폴더를 resource 바로 아래까지 지운다
            string dir = Path.GetDirectoryName(full);
            while (dir.Length > resourceDir.Length && Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
            {
                Directory.Delete(dir);
                dir = Path.GetDirectoryName(dir);
            }
        }
    }
}
