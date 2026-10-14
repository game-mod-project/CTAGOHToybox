// 화면이 쓰는 편집 상태와 동작. 화면은 이 클래스만 부르고 원본을 직접 읽거나 파일을 쓰지 않는다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class ValueView
    {
        public TraceValue Trace { get; set; }
        public int SharedBy { get; set; }             // 그 파일을 읽는 유닛 수
        public string UnitScopeBlocker { get; set; }  // "이 유닛만"이 안 되는 이유. 되면 null
        public Edit Edit { get; set; }                // 이 자리에 걸린 수정
        public bool Editable { get { return Trace.Arg >= 0; } }
    }

    public sealed class RowView
    {
        public string Path { get; set; }
        public int Occurrence { get; set; }
        public string Form { get; set; }
        public KeyNote Note { get; set; }             // 키 설명. 없으면 null
        public string SummaryLabel { get; set; }      // 요약 탭의 이름. 요약에 없으면 null
        public List<ValueView> Values { get; } = new List<ValueView>();
    }

    public sealed class UnitView
    {
        public string Unit { get; set; }
        public string Kind { get; set; }
        public string Layer { get; set; }
        public string DisplayName { get; set; }
        public List<RowView> Rows { get; } = new List<RowView>();
        public List<string> Unresolved { get; set; }
        public List<ItemLink> Items { get; set; }
        public List<SdlIncludeEdge> IncludeEdges { get; set; }
    }

    public sealed partial class EditorSession : IDisposable
    {
        public EditorSettings Settings { get; private set; }
        public LayerPlan Plan { get; private set; }
        public SourceStack Stack { get; private set; }
        public UnitIndex Index { get; private set; }
        public DisplayNames Names { get; private set; }
        public ItemLinks Items { get; private set; }
        public EditProject Project { get; private set; }
        public string ProjectPath { get; private set; }

        string annotationDir, summaryDir;
        UnitTracer tracer;
        readonly Dictionary<string, KeyAnnotations> notes = new Dictionary<string, KeyAnnotations>(StringComparer.Ordinal);
        readonly Dictionary<string, SummaryKeys> summaries = new Dictionary<string, SummaryKeys>(StringComparer.Ordinal);

        // repoRoot는 data/와 editor/가 있는 저장소 루트다
        public static EditorSession Open(string settingsPath, string repoRoot, Action<string> progress)
        {
            Action<string> say = progress ?? (s => { });
            if (!File.Exists(settingsPath)) throw new FileNotFoundException("설정 파일이 없다: " + settingsPath, settingsPath);
            var s = new EditorSession { Settings = EditorSettings.Load(settingsPath) };
            if (!File.Exists(s.Settings.OptionsSetPath)) throw new FileNotFoundException("options.set이 없다: " + s.Settings.OptionsSetPath, s.Settings.OptionsSetPath);
            say("원본 계층을 읽는 중");
            s.Plan = GameProfile.PlanLayers(s.Settings, GameProfile.ReadModEntries(File.ReadAllText(s.Settings.OptionsSetPath)));
            s.Stack = new SourceStack(s.Plan.Layers);
            s.Stack.Scan();
            say("유닛 색인을 여는 중");
            string cache = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "cache", "unit-index.json");
            bool rebuilt;
            s.Index = UnitIndex.LoadOrBuild(s.Stack, cache, (done, total) => say("유닛 색인을 만드는 중: " + done + "/" + total), out rebuilt);
            if (rebuilt) say("유닛 색인을 새로 만들었다: " + s.Index.Units.Count + "개");
            s.Names = DisplayNames.Load(Path.Combine(repoRoot, "data", "display-names.csv"));
            s.Items = ItemLinks.Build(s.Stack.Paths);
            s.annotationDir = Path.Combine(repoRoot, "data", "annotations");
            s.summaryDir = Path.Combine(repoRoot, "editor", "summary");
            s.tracer = new UnitTracer(s.Stack);
            say("준비됐다");
            return s;
        }

        public IEnumerable<UnitEntry> Search(string text, string kind, string layer)
        {
            return Index.Search(text, kind, layer, Names);
        }

        KeyAnnotations NotesFor(string unit)
        {
            string group = KeyAnnotations.GroupOf(unit);
            KeyAnnotations n;
            if (!notes.TryGetValue(group, out n)) { n = KeyAnnotations.Load(annotationDir, group); notes[group] = n; }
            return n;
        }

        SummaryKeys SummaryFor(string kind, KeyAnnotations n)
        {
            string key = kind ?? "(none)";
            SummaryKeys s;
            if (!summaries.TryGetValue(key, out s)) { s = SummaryKeys.Load(summaryDir, key, n); summaries[key] = s; }
            return s;
        }

        // 없으면 null
        public UnitView OpenUnit(string unit)
        {
            var traced = tracer.Trace(unit);
            if (traced == null) return null;
            string kind = UnitKind.Of(traced.Unit);
            var n = NotesFor(traced.Unit);
            var summary = SummaryFor(kind, n);
            var view = new UnitView
            {
                Unit = traced.Unit, Kind = kind, Layer = Stack.Find(traced.Unit).Layer, DisplayName = Names.OfUnit(traced.Unit),
                Unresolved = traced.Unresolved, IncludeEdges = traced.IncludeEdges,
                Items = ItemLinks.ReferencedNames(traced.Nodes).Select(Items.Find).ToList()
            };
            foreach (var row in traced.Rows)
            {
                var r = new RowView { Path = row.Path, Occurrence = row.Occurrence, Form = row.Form, Note = n.Find(row.Path, row.Form, true), SummaryLabel = summary.LabelOf(row.Path) };
                foreach (var v in row.Values)
                {
                    var vv = new ValueView { Trace = v };
                    if (v.Arg >= 0)
                    {
                        vv.SharedBy = Index.UsersOf(v.File).Count;
                        vv.UnitScopeBlocker = IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, v.File, v.KeyPath);
                    }
                    r.Values.Add(vv);
                }
                view.Rows.Add(r);
            }
            RefreshEdits(view);
            return view;
        }

        // 화면의 유닛에 걸린 수정을 다시 잇는다(수정을 더하거나 지운 뒤)
        public void RefreshEdits(UnitView view)
        {
            foreach (var row in view.Rows)
                foreach (var v in row.Values) v.Edit = v.Editable ? EditFor(view, v) : null;
        }

        static bool SameSlot(Edit e, TraceValue v)
        {
            return string.Equals(SourceStack.ToVirtual(e.File ?? ""), v.File, StringComparison.Ordinal)
                && string.Equals(e.Path, v.KeyPath, StringComparison.OrdinalIgnoreCase) && e.Occurrence == v.Occurrence && e.Arg == v.Arg;
        }

        // 그 자리에 걸린 수정. '이 유닛만' 수정은 그 유닛의 것만 본다. 둘 다 있으면 '이 유닛만'이 이긴다(빌드와 같다)
        public Edit EditFor(UnitView unit, ValueView value)
        {
            if (Project == null || !value.Editable) return null;
            var mine = Project.Edits.FirstOrDefault(e => e.Scope == "unit" && SourceStack.ToVirtual(e.Unit ?? "") == unit.Unit && SameSlot(e, value.Trace));
            return mine ?? Project.Edits.FirstOrDefault(e => e.Scope != "unit" && SameSlot(e, value.Trace));
        }

        // 없는 파일은 빈 프로젝트로 시작한다(바뀌는 것이 있을 때 쓴다). 깨진 파일은 InvalidDataException이고 지금 프로젝트는 그대로다
        public void OpenProject(string path)
        {
            EditProject loaded;
            if (File.Exists(path))
            {
                try { loaded = EditProject.Load(path); }
                catch (JsonException ex) { throw new InvalidDataException("프로젝트 파일을 읽지 못했다: " + path + ": " + ex.Message, ex); }
                if (loaded == null) throw new InvalidDataException("프로젝트 파일이 비어 있다: " + path);
                if (loaded.Edits == null) loaded.Edits = new List<Edit>();
            }
            else loaded = new EditProject { Name = Path.GetFileNameWithoutExtension(path) };
            Project = loaded;
            ProjectPath = path;
        }

        public void Dispose()
        {
            if (Stack != null) Stack.Dispose();
        }
    }
}
