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
        public string Name { get; set; }              // 값을 감싼 호출의 이름(cp(40)이면 cp). 화면에 값 이름으로 보인다. 없으면 null
        public string Label { get; set; }             // 값의 뜻(data/annotations/values.psv). 모르면 null
        public bool Editable { get { return Trace.Arg >= 0; } }
    }

    public sealed class RowView
    {
        public string Path { get; set; }
        public string RawPath { get; set; }          // 원문의 대소문자 그대로 쓴 경로. 화면에 "정확한 키"로 보인다
        public int Occurrence { get; set; }
        public string Form { get; set; }
        public KeyNote Note { get; set; }             // 키 설명. 없으면 null
        public string SummaryLabel { get; set; }      // 요약 탭의 이름. 요약에 없으면 null
        public string Context { get; set; }           // 같은 키가 여러 번 나올 때 어느 것인지: 가장 가까운 이름 있는 상위 블록(볼륨, 부품, 무장 자리 등)의 이름
        public bool IsMacroCall { get; set; }         // 파일 안의 매크로 호출 줄. 계산에 쓰여 펼친 값으로는 못 고치는 인자를 여기서 고친다
        public string RosterEntry { get; set; }       // 가격·로스터 줄이면 '파일 · 항목 이름'. 유닛의 줄이면 null
        public int Repeats { get; set; } = 1;         // 같은 줄이 펼친 트리에 나온 횟수(CollapseRepeats가 하나로 모은 수)
        public List<ValueView> Values { get; } = new List<ValueView>();
    }

    public sealed class UnitView
    {
        public string Unit { get; set; }
        public string Kind { get; set; }
        public string EditorKind { get; set; }        // 편집기 종류(탱크, 포 등). 화면 이름은 UnitKind.Label
        public string Layer { get; set; }
        public string DisplayName { get; set; }
        public List<RowView> Rows { get; } = new List<RowView>();
        public List<string> Unresolved { get; set; }
        public List<ItemLink> Items { get; set; }
        public List<SdlIncludeEdge> IncludeEdges { get; set; }
        public List<RowView> Roster { get; } = new List<RowView>();   // 가격·로스터 파일에서 이 유닛의 항목 줄
    }

    public sealed partial class EditorSession : IDisposable
    {
        public EditorSettings Settings { get; private set; }
        public LayerPlan Plan { get; private set; }
        public SourceStack Stack { get; private set; }
        public UnitIndex Index { get; private set; }
        public DisplayNames Names { get; private set; }
        public ItemLinks Items { get; private set; }
        public RosterLinks Roster { get; private set; }
        public EditProject Project { get; private set; }
        public string ProjectPath { get; private set; }

        string annotationDir, summaryDir;
        ValueLabels valueLabels = new ValueLabels();
        readonly Dictionary<string, string> layerLabels = new Dictionary<string, string>(StringComparer.Ordinal);
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
            say("게임 현지화에서 이름을 읽는 중");
            s.Names = DisplayNames.FromGame(s.Settings, s.Plan);
            foreach (var layer in s.Plan.Layers) s.layerLabels[layer.Name] = layer.Name == "vanilla" ? "바닐라" : ModName(layer) ?? layer.Name;
            s.Items = ItemLinks.Build(s.Stack.Paths);
            say("가격·로스터 파일을 읽는 중");
            s.Roster = RosterLinks.Build(s.Stack.Paths, s.Stack.ReadText);
            s.annotationDir = Path.Combine(repoRoot, "data", "annotations");
            s.valueLabels = ValueLabels.Load(Path.Combine(s.annotationDir, "values.psv"));
            s.summaryDir = Path.Combine(repoRoot, "editor", "summary");
            s.tracer = new UnitTracer(s.Stack);
            say("준비됐다");
            return s;
        }

        // 편집기 목록: 편집기 종류(UnitKind.Editor)이고 값에 수가 하나라도 있는 유닛만. from·tag·entity 같은 이름만 있는 파일은 고칠 스탯이 없다.
        // kind는 편집기 종류(tank 등)나 원래 종류(vehicle이면 차량 전부)다
        public IEnumerable<UnitEntry> Search(string text, string kind, string layer)
        {
            // 이름 있는 유닛을 먼저, 이름순으로. 이름이 없으면 경로순
            return Index.Search(text, null, layer, Names)
                .Where(u => UnitKind.InEditor(u.EditorKind) && u.Numbers > 0)
                .Where(u => kind == null || string.Equals(u.EditorKind, kind, StringComparison.OrdinalIgnoreCase) || string.Equals(u.Kind, kind, StringComparison.OrdinalIgnoreCase))
                .Select(u => new { u, name = Names.OfUnit(u.Path) })
                .OrderBy(x => x.name == null ? 1 : 0).ThenBy(x => x.name, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.u.Path, StringComparer.Ordinal)
                .Select(x => x.u);
        }

        // 계층의 화면 이름: 바닐라, 또는 모드 폴더의 mod.info에 적힌 이름
        public string LayerLabel(string layer)
        {
            string label;
            return layerLabels.TryGetValue(layer, out label) ? label : layer;
        }

        static string ModName(SourceLayer layer)
        {
            string info = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(layer.ResourceDir)), "mod.info");
            if (!File.Exists(info)) return null;
            var rows = SdlCore.KeyRows(SdlCore.Parse(File.ReadAllText(info)).Root.Children, new string[0]);
            var name = rows.FirstOrDefault(r => r.Path == "/mod/name" && r.ArgNodes.Count > 0);
            return name == null ? null : name.ArgNodes[0].Text.Trim('"');
        }

        KeyAnnotations NotesFor(string unit)
        {
            string group = KeyAnnotations.GroupOf(unit);
            KeyAnnotations n;
            if (!notes.TryGetValue(group, out n)) { n = KeyAnnotations.Load(annotationDir, group); notes[group] = n; }
            return n;
        }

        // 화면이 요약 묶음의 순서를 정할 때 쓴다
        public SummaryKeys SummaryOf(UnitView unit) { return SummaryFor(unit.Kind, NotesFor(unit.Unit)); }

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
                Unit = traced.Unit, Kind = kind, EditorKind = UnitKind.EditorKindOf(kind, traced.IncludeEdges.Select(e => e.ToFile)), Layer = Stack.Find(traced.Unit).Layer, DisplayName = Names.OfUnit(traced.Unit),
                Unresolved = traced.Unresolved, IncludeEdges = traced.IncludeEdges,
                Items = ItemLinks.ReferencedNames(traced.Nodes).Select(Items.Find).ToList()
            };
            foreach (var row in traced.Rows)
            {
                var r = new RowView { Path = row.Path, RawPath = row.RawPath, Occurrence = row.Occurrence, Form = row.Form, Context = row.Context, Note = n.Find(row.Path, row.Form, true), SummaryLabel = summary.LabelOf(row.Path) };
                foreach (var v in row.Values)
                {
                    var vv = new ValueView { Trace = v, Name = row.LeafNames == null ? null : row.LeafNames[r.Values.Count] };
                    if (v.Arg >= 0)
                    {
                        vv.SharedBy = Index.UsersOf(v.File).Count;
                        vv.UnitScopeBlocker = IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, v.File, v.KeyPath);
                    }
                    r.Values.Add(vv);
                }
                view.Rows.Add(r);
            }
            AddMacroCalls(view, traced, n, summary);
            CollapseRepeats(view.Rows);
            AddRoster(view);
            foreach (var row in view.Rows.Concat(view.Roster))
            {
                var labels = valueLabels.Of(row.Path, row.Values.Select(v => v.Trace.Text).ToList(), row.Values.Select(v => v.Name).ToList());
                for (int i = 0; i < row.Values.Count; i++) row.Values[i].Label = labels[i];
            }
            RefreshEdits(view);
            return view;
        }

        static readonly KeyNote RosterMacroNote = new KeyNote
        {
            Grade = "확인",
            Meaning = "등록 줄의 매크로 인자. set/multiplayer/units/settings.set이 cost는 {cost}(인력 MP 비용), cp는 {cp}, cw는 {cw}, cd는 {charge {delay}}(처음 지연), ci는 {charge {interval}}(간격)으로 넣는다. crewN(이름:수)는 탑승 breed와 인원이다. 보병의 cost·CP는 분대에 든 사람만큼 더해 분대 값이 된다(inf_*.set 주석). cp·cw의 뜻을 적은 주석은 없다(미확인)",
            Source = "게임 파일 주석",
        };
        static readonly KeyNote RosterCostNote = new KeyNote
        {
            Grade = "확인",
            Meaning = "인력 비용 (MP). 분대는 MP 비용이 0이고 든 병사 cost의 합으로 값이 정해진다(units_*.set 주석)",
            Source = "게임 파일 주석",
        };
        static readonly KeyNote RosterNameNote = new KeyNote
        {
            Grade = "확인",
            Meaning = "로스터 항목. 첫 값이 이 유닛의 이름이다. 연구 트리(unit_research_*)에서는 requires 뒤가 먼저 연구할 항목, costs 뒤가 연구 비용, position 뒤가 트리의 자리다",
            Source = "게임 파일 구조",
        };

        // 가격·로스터 파일에서 이 유닛의 항목 줄. 원본 파일 그대로의 줄이라 '전부'로만 고친다(그 파일은 유닛이 읽는 파일이 아니다)
        void AddRoster(UnitView view)
        {
            var n = NotesFor("set/multiplayer/units/x.set");
            foreach (var entry in Roster.EntriesOf(view.Unit))
            {
                string group = entry.File + " · " + entry.Name;
                for (int r = 0; r < entry.Rows.Count; r++)
                {
                    var row = entry.Rows[r];
                    if (row.Leaves.Count == 0) continue;
                    var note = n.Find(row.Path, row.Form, true) ?? (row.Form == "macro" ? RosterMacroNote : r == 0 ? RosterNameNote : row.Path.EndsWith("/cost", StringComparison.Ordinal) ? RosterCostNote : null);
                    var rv = new RowView { Path = row.Path, RawPath = row.RawPath, Occurrence = row.Occurrence, Form = row.Form, Note = note, RosterEntry = group, IsMacroCall = row.Form == "macro" };
                    for (int i = 0; i < row.Leaves.Count; i++)
                    {
                        var leaf = row.Leaves[i];
                        var t = new TraceValue { Text = leaf.Text, File = entry.File, KeyPath = row.Path, Occurrence = row.Occurrence, Arg = i };
                        // 항목 이름을 바꾸면 유닛과 이어지지 않는다. 호출 밖의 따옴표 없는 낱말(requires, costs 같은 열쇠말)도 고치지 않는다
                        if (r == 0 && i == 0) { t.Arg = -1; t.Note = "항목 이름이다. 바꾸면 유닛과 이어지지 않는다"; }
                        else if (row.LeafNames[i] == null && leaf.Kind == "atom" && !SdlCore.IsNumber(leaf.Text)) { t.Arg = -1; t.Note = "열쇠말이다(" + leaf.Text + ")"; }
                        rv.Values.Add(new ValueView { Trace = t, Name = row.LeafNames[i], SharedBy = 1,
                            UnitScopeBlocker = "가격·로스터 파일은 유닛이 읽는 파일이 아니다. 이 항목만 바뀐다" });
                    }
                    view.Roster.Add(rv);
                }
            }
        }

        public sealed class EditDescription
        {
            public KeyNote Note { get; set; }      // 줄의 키 주석. 없으면 null
            public bool IsMacroCall { get; set; }
            public string ValueLabel { get; set; } // 고친 값의 뜻(values.psv). 모르면 null
            public string ValueName { get; set; }  // 고친 값을 감싼 호출의 이름. 없으면 null
            public string RowText { get; set; }    // 원본 줄의 값 전체
            public int ValueCount { get; set; }
        }

        // 수정 목록이 보일 설명: 원본 파일에서 그 줄을 다시 찾아 이름을 붙인다. 줄을 찾지 못하면 null
        public EditDescription Describe(Edit e)
        {
            string file = SourceStack.ToVirtual(e.File ?? "");
            string text = Stack.ReadText(file);
            if (text == null) return null;
            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0) return null;
            var row = KeyLocator.Rows(parsed.Root).FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
            if (row == null || e.Arg < 0 || e.Arg >= row.Leaves.Count) return null;
            var texts = row.Leaves.Select(l => l.Text).ToList();
            var labels = valueLabels.Of(row.Path, texts, row.LeafNames);
            var n = NotesFor(SourceStack.ToVirtual(e.Unit ?? file));
            return new EditDescription
            {
                Note = n.Find(row.Path, row.Form, true) ?? NotesFor(file).Find(row.Path, row.Form, true),
                IsMacroCall = row.Form == "macro", ValueLabel = labels[e.Arg], ValueName = row.LeafNames[e.Arg],
                RowText = string.Join(" ", texts), ValueCount = texts.Count,
            };
        }

        // 같은 줄을 하나로 모은다. 매크로 인자 하나가 여러 곳에 펼쳐지면(예: 탄종마다 같은 사거리 표) 같은 원본 자리를 가리키는 줄이 여러 번 나온다.
        // 같은 키 경로에, 고칠 수 있는 값은 같은 원본 자리, 고칠 수 없는 값은 같은 글자면 같은 줄이다. 어느 줄을 고쳐도 같은 자리가 바뀌므로 하나만 보인다
        public static void CollapseRepeats(List<RowView> rows)
        {
            const int MaxContexts = 3;
            var first = new Dictionary<string, RowView>(StringComparer.Ordinal);
            var contexts = new Dictionary<RowView, List<string>>();
            var kept = new List<RowView>(rows.Count);
            foreach (var row in rows)
            {
                string key = RepeatKey(row);
                RowView same;
                if (!first.TryGetValue(key, out same))
                {
                    first[key] = row; kept.Add(row);
                    contexts[row] = row.Context == null ? new List<string>() : new List<string> { row.Context };
                    continue;
                }
                same.Repeats++;
                if (row.Context != null && !contexts[same].Contains(row.Context)) contexts[same].Add(row.Context);
            }
            // 모은 줄의 부위는 앞의 몇 개만 적는다
            foreach (var row in kept)
            {
                var list = contexts[row];
                if (list.Count > 1)
                    row.Context = string.Join(", ", list.Take(MaxContexts)) + (list.Count > MaxContexts ? " 외 " + (list.Count - MaxContexts) + "곳" : "");
            }
            rows.Clear();
            rows.AddRange(kept);
        }

        static string RepeatKey(RowView row)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(row.Path).Append('\t').Append(row.Form).Append('\t').Append(row.IsMacroCall);
            foreach (var v in row.Values)
            {
                var t = v.Trace;
                if (v.Editable) sb.Append("\t@").Append(t.File).Append('|').Append(t.KeyPath).Append('|').Append(t.Occurrence).Append('|').Append(t.Arg);
                else sb.Append("\t=").Append(t.Text);
            }
            return sb.ToString();
        }

        // 유닛 파일과 include한 파일의 매크로 호출 줄. 펼친 트리에서는 호출이 본문으로 바뀌어 사라지므로 따로 보인다
        void AddMacroCalls(UnitView view, UnitTraceResult traced, KeyAnnotations n, SummaryKeys summary)
        {
            var files = new List<string> { traced.Unit };
            foreach (var e in traced.IncludeEdges) if (!files.Contains(e.ToFile)) files.Add(e.ToFile);
            foreach (string file in files)
            {
                string text = Stack.ReadText(file);
                if (text == null) continue;
                var parsed = SdlCore.Parse(text);
                if (parsed.Errors.Count > 0) continue;
                foreach (var row in KeyLocator.Rows(parsed.Root))
                {
                    // 매크로 정의 안의 호출은 이 유닛이 그 매크로를 쓸 때만 펼쳐지므로 보이지 않는다(쓰는지 알 수 없다)
                    if (row.Form != "macro" || row.Leaves.Count == 0 || row.Path.Contains("(define ")) continue;
                    var r = new RowView { Path = row.Path, RawPath = row.RawPath, Occurrence = row.Occurrence, Form = row.Form, IsMacroCall = true, Note = n.Find(row.Path, row.Form, true), SummaryLabel = summary.LabelOf(row.Path) };
                    for (int i = 0; i < row.Leaves.Count; i++)
                    {
                        var t = new TraceValue { Text = row.Leaves[i].Text, File = file, KeyPath = row.Path, Occurrence = row.Occurrence, Arg = i };
                        // %이름은 바깥에서 받는 매개변수다. 고치면 매크로가 깨진다
                        if (t.Text.StartsWith("%")) { t.Arg = -1; t.Note = "매개변수(" + t.Text + ")라 고칠 수 없다. 이 값을 넘기는 쪽에서 고친다"; }
                        r.Values.Add(new ValueView { Trace = t, Name = row.LeafNames[i], SharedBy = Index.UsersOf(file).Count, UnitScopeBlocker = IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, file, row.Path) });
                    }
                    view.Rows.Add(r);
                }
            }
        }

        // 화면의 유닛에 걸린 수정을 다시 잇는다(수정을 더하거나 지운 뒤)
        public void RefreshEdits(UnitView view)
        {
            foreach (var row in view.Rows.Concat(view.Roster))
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
