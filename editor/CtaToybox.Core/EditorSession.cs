// 화면이 쓰는 편집 상태와 동작. 화면은 이 클래스만 부르고 원본을 직접 읽거나 파일을 쓰지 않는다.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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
        public List<SettingPiece> Pieces { get; set; }   // 문자열 속 설정("이름=값;…")의 조각. 게임 설정 폴더의 파일만. 아니면 null
        public List<string> PieceLabels { get; set; }    // 조각마다 뜻(values.psv의 @이름). 모르면 그 자리 null
        public string Steps { get; set; }                // 판 수 단계표의 읽기용 풀이. 아니면 null
        public string Computed { get; set; }             // Lua 수식(예: 2 * 60000)의 계산값 글자. 아니면 null
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
        public string SourceComment { get; set; }     // Lua 설정의 개발사 주석(영어 원문). 없으면 null
        public string TableComment { get; set; }      // Lua 설정의 표 앞 주석. 없으면 null
        public bool ContextIsNamesake { get; set; }   // Context가 이름이 같은 줄을 가르는 상위 블록 이름이다(게임 설정, NameNamesakes)
        public string Cond { get; set; }              // 감싼 (mod ...) 조건 원문. 조건 밖이면 null
        public string CondLabel { get { return LabelOf(Cond); } }
        public string From { get; set; }              // 원형 상속으로 물려받은 줄이면 그 원형 파일. 유닛 자신의 줄이면 null
        public string FromState { get; set; }         // FromLevel.Inherited | Overridden | Unconfirmed
        public string FromBlock { get; set; }         // Unconfirmed일 때 자식 쪽에도 있는 감싼 블록
        public string FromLabel { get { return FromLevel.LabelOf(From, FromState, FromBlock); } }
        public List<ValueView> Values { get; } = new List<ValueView>();

        // (mod ...) 조건의 읽히는 이름. 개발사 문서(Men of War II)는 전역 모드를 멀티플레이(mp)와 그 밖으로만 나눈다.
        // Gates of Hell의 다른 조건 이름(coop, hardcore, gameclass:..., gametype:...)은 뜻을 확인하지 않아 원문 그대로 보인다
        public static string LabelOf(string cond)
        {
            if (string.IsNullOrEmpty(cond)) return null;
            var parts = System.Text.RegularExpressions.Regex.Matches(cond, @"\(mod [^)]*\)").Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value).ToList();
            return string.Join(" · ", parts.Select(p =>
            {
                string inner = p.Substring(5, p.Length - 6).Trim();
                if (inner == "\"mp\"" || inner == "mp") return "멀티플레이에서만";
                if (inner == "not \"mp\"" || inner == "not_mp") return "멀티플레이가 아닐 때만";
                return p + "일 때만 (뜻 미확인)";
            }));
        }

        // 요약 탭의 줄 이름. 부위(volume body1 등)는 묶음 이름을 대신하는 줄 이름이 되고,
        // 이름이 같은 줄의 상위 블록은 이름 뒤에 붙인다(대신하면 Low·Low·Low처럼 뜻이 사라진다)
        public string SummaryName(string rowName)
        {
            if (Context == null) return rowName;
            return ContextIsNamesake ? rowName + " · " + Context : Context;
        }
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
        public string ReadError { get; set; }         // 파일을 읽지 못해 고칠 수 없을 때의 안내. 읽었으면 null
        public List<ItemLink> Items { get; set; }
        public List<SdlIncludeEdge> IncludeEdges { get; set; }
        public List<RowView> Roster { get; } = new List<RowView>();   // 가격·로스터 파일에서 이 유닛의 항목 줄
        public string FromChain { get; set; }         // {from}으로 물려받는 원형 사슬의 안내(FromLevel.ChainLine). 물려받지 않으면 null
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
        Dictionary<string, SettingFiles.Entry> settingFiles = new Dictionary<string, SettingFiles.Entry>(StringComparer.OrdinalIgnoreCase);
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
            s.Plan.Warnings.AddRange(s.Names.Warnings);
            // 게임 설정 파일 이름표는 명령줄과 같이 설정 파일 옆의 settings/files.psv에서 읽는다
            s.settingFiles = SettingFiles.LoadNames(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "settings", "files.psv"));
            foreach (var kv in s.settingFiles) s.Names.AddPathName(kv.Key, kv.Value.Name);
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

        // 편집기 목록: 편집기 종류(UnitKind.Editor)이고 값에 수가 하나라도 있는 유닛만. from·tag·entity 같은 이름만 있는 파일은 고칠 스탯이 없다. 게임 설정은 수가 없어도 넣는다(문자열 단계표만 있는 파일).
        // kind는 편집기 종류(tank 등)나 원래 종류(vehicle이면 차량 전부)다
        public IEnumerable<UnitEntry> Search(string text, string kind, string layer)
        {
            // 이름 있는 유닛을 먼저, 이름순으로. 이름이 없으면 경로순
            return Index.Search(text, null, layer, Names)
                .Where(u => UnitKind.InEditor(u.EditorKind) && (u.Numbers > 0 || u.EditorKind == SettingFiles.Kind))
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

        // 목록에 보이는 종류 글자: 게임 설정은 files.psv의 묶음(컨퀘스트 자원 등), 나머지는 종류 이름
        public string KindLabel(UnitEntry u)
        {
            SettingFiles.Entry e;
            if (u.EditorKind == SettingFiles.Kind && settingFiles.TryGetValue(u.Path, out e)) return e.Group;
            return UnitKind.Label(u.EditorKind);
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
            // 게임 설정은 편집기 전용 묶음 settings.psv를 쓴다. GroupOf는 Common.ps1과 같은 규칙으로 둔다
            string group = GroupFor(unit);
            KeyAnnotations n;
            // 게임 설정에는 common.psv의 */이름이 붙지 않는다(Q8). settings.psv의 */이름만 쓴다
            if (!notes.TryGetValue(group, out n)) { n = KeyAnnotations.Load(annotationDir, group, group != SettingFiles.Kind); notes[group] = n; }
            return n;
        }

        static string GroupFor(string unit)
        {
            return UnitKind.OfEditor(unit) == SettingFiles.Kind ? SettingFiles.Kind : KeyAnnotations.GroupOf(unit);
        }

        // */이름 주석을 붙여도 되는 줄인지: file은 줄이 있는 파일(펼친 줄이면 유닛), unit은 열어 놓은 유닛.
        // Get-Annotation과 같이 유닛 파일의 줄과 펼친 키에만 붙인다. 게임 설정은 NotesFor가 common.psv의 */이름을 빼 두었다(Q8)
        static bool WildcardFor(string file, string unit)
        {
            return UnitKind.Of(file) != null || UnitKind.OfEditor(unit) == SettingFiles.Kind;
        }

        // 화면이 요약 묶음의 순서를 정할 때 쓴다
        public SummaryKeys SummaryOf(UnitView unit) { return SummaryFor(unit.Kind, NotesFor(unit.Unit), GroupFor(unit.Unit)); }

        // 요약 키 파일이 없으면 주석 묶음의 "확인" 키로 대신하므로, 종류와 묶음을 함께 열쇠로 둔다(Q8)
        SummaryKeys SummaryFor(string kind, KeyAnnotations n, string group)
        {
            string key = (kind ?? "(none)") + "\t" + group;
            SummaryKeys s;
            if (!summaries.TryGetValue(key, out s)) { s = SummaryKeys.Load(summaryDir, kind ?? "(none)", n); summaries[key] = s; }
            return s;
        }

        // 없으면 null
        public UnitView OpenUnit(string unit)
        {
            var traced = tracer.Trace(unit);
            if (traced == null) return null;
            string kind = UnitKind.OfEditor(traced.Unit);
            var n = NotesFor(traced.Unit);
            var summary = SummaryFor(kind, n, GroupFor(traced.Unit));
            bool wild = WildcardFor(traced.Unit, traced.Unit);
            var view = new UnitView
            {
                Unit = traced.Unit, Kind = kind, EditorKind = UnitKind.EditorKindOf(kind, traced.IncludeEdges.Select(e => e.ToFile)), Layer = Stack.Find(traced.Unit).Layer, DisplayName = Names.OfUnit(traced.Unit),
                Unresolved = traced.Unresolved, ReadError = traced.ReadError, IncludeEdges = traced.IncludeEdges,
                Items = ItemLinks.ReferencedNames(traced.Nodes).Select(Items.Find).ToList(),
                FromChain = FromLevel.ChainLine(traced.From),
            };
            foreach (var row in traced.Rows)
            {
                var r = new RowView { Path = row.Path, RawPath = row.RawPath, Occurrence = row.Occurrence, Form = row.Form, Context = row.Context, Cond = row.Cond, Note = n.Find(row.Path, row.Form, wild, row.RawPath), SummaryLabel = summary.LabelOf(row.Path, row.RawPath),
                    From = row.From, FromState = row.FromState, FromBlock = row.FromBlock };
                // 요약은 게임이 쓰는 값을 보이는 곳이라 자식 쪽 값에 가려진 원형의 줄은 넣지 않는다
                if (row.FromState == FromLevel.Overridden) r.SummaryLabel = null;
                r.SourceComment = row.Comment;
                r.TableComment = row.TableComment;
                foreach (var v in row.Values)
                {
                    var vv = new ValueView { Trace = v, Name = row.LeafNames == null ? null : row.LeafNames[r.Values.Count] };
                    double computed;
                    if (LuaTable.IsLua(v.File) && !SdlCore.IsNumber(v.Text) && LuaTable.TryEvaluate(v.Text, out computed))
                        vv.Computed = computed.ToString("0.###", CultureInfo.InvariantCulture);
                    if (v.Arg >= 0)
                    {
                        vv.SharedBy = Index.UsersOf(v.File).Count;
                        vv.UnitScopeBlocker = row.From != null ? FromLevel.UnitScopeNote : IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, v.File, v.KeyPath);
                    }
                    r.Values.Add(vv);
                }
                view.Rows.Add(r);
            }
            AddMacroCalls(view, traced, n, summary);
            int heirs = Index.HeirsOf(traced.Unit).Count;
            if (heirs > 0)
                foreach (var r in view.Rows)
                    foreach (var v in r.Values) if (v.Editable && v.UnitScopeBlocker == null) v.UnitScopeBlocker = FromLevel.HeirsNote(heirs);
            // 게임 설정은 파일 하나가 곧 설정이라 '이 유닛만'이 없다
            if (kind == SettingFiles.Kind)
            {
                foreach (var r in view.Rows)
                    foreach (var v in r.Values) if (v.Editable) v.UnitScopeBlocker = SettingFiles.UnitScopeNote;
                NameNamesakes(view.Rows);
            }
            CollapseRepeats(view.Rows);
            AddRoster(view);
            foreach (var row in view.Rows.Concat(view.Roster))
            {
                var labels = valueLabels.Of(row.Path, row.Values.Select(v => v.Trace.Text).ToList(), row.Values.Select(v => v.Name).ToList(), row.RawPath);
                for (int i = 0; i < row.Values.Count; i++)
                {
                    var v = row.Values[i];
                    v.Label = labels[i];
                    if (!v.Editable || !SettingFiles.InArea(v.Trace.File)) continue;
                    v.Pieces = SettingStrings.Split(v.Trace.Text);
                    if (v.Pieces != null) v.PieceLabels = v.Pieces.Select(p => valueLabels.PieceLabel(row.Path, row.RawPath, p.Name)).ToList();
                    v.Steps = SettingStrings.StepPreview(v.Trace.Text, row.Path);
                }
            }
            RefreshEdits(view);
            return view;
        }

        static readonly KeyNote RosterMacroNote = new KeyNote
        {
            Grade = "확인",
            Meaning = "등록 줄의 매크로 인자. set/multiplayer/units/settings.set이 cost는 {cost}(인력 MP 비용), cp는 {cp}, cw는 {cw}, cd는 {charge {delay}}(처음 지연), ci는 {charge {interval}}(간격)으로 넣는다. crewN(이름:수)는 탑승 breed와 인원이다. 보병의 cost·CP는 분대에 든 사람만큼 더해 분대 값이 된다(inf_*.set 주석). cw는 깃발 점령 가중치, cp는 배치할 수 있는 병력 한도를 채우는 CP다(같은 파일 주석 \"cw() corresponds to capture weight for flags\", \"cp() corresponds to CP, that is, the value that units have that fill up how many troops can be fielded\"). 컨퀘스트 보병의 r·rm은 봇이 이 유닛을 쓸 수 있는 연구 단계의 최소·최대다(conquest/settings.set 주석, settings_inf.set의 {research_stage %r} {research_stage_max %rm})",
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
                    var note = n.Find(row.Path, row.Form, false, row.RawPath) ?? (row.Form == "macro" ? RosterMacroNote : r == 0 ? RosterNameNote : row.Path.EndsWith("/cost", StringComparison.Ordinal) ? RosterCostNote : null);
                    var rv = new RowView { Path = row.Path, RawPath = row.RawPath, Occurrence = row.Occurrence, Form = row.Form, Note = note, RosterEntry = group, IsMacroCall = row.Form == "macro", Cond = row.Cond };
                    for (int i = 0; i < row.Leaves.Count; i++)
                    {
                        var leaf = row.Leaves[i];
                        var t = new TraceValue { Text = leaf.Text, File = entry.File, KeyPath = row.Path, Occurrence = row.Occurrence, Arg = i };
                        if (KeyLocator.IsNamed(row.Path)) { t.NamedPath = row.RawPath; t.NamedOccurrence = row.RawOccurrence; }
                        // 항목 이름을 바꾸면 유닛과 이어지지 않는다. 호출 밖의 따옴표 없는 낱말(requires, costs 같은 열쇠말)도 고치지 않는다
                        // 분대 호출 항목은 name() 인자가 항목 이름이다
                        bool entryName = entry.IsCall ? r == 0 && string.Equals(row.LeafNames[i], "name", StringComparison.OrdinalIgnoreCase) : r == 0 && i == 0;
                        if (KeyLocator.IsDisabled(row.Path)) { t.Arg = -1; t.Note = KeyLocator.DisabledNote; }
                        else if (entryName) { t.Arg = -1; t.Note = "항목 이름이다. 바꾸면 유닛과 이어지지 않는다"; }
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
            public List<string> ChangedPieces { get; set; }   // 문자열 속 설정이면 바뀐 조각만 "뜻(없으면 이름): 원래 → 새 값". 아니면 null
            public List<string> BlockNames { get; set; }      // 줄이 든 따옴표 이름 블록의 이름(바깥부터). 없으면 빈 목록(E16)
        }

        // 수정 목록이 보일 설명: 원본 파일에서 그 줄을 다시 찾아 이름을 붙인다. 줄을 찾지 못하면 null
        public EditDescription Describe(Edit e)
        {
            string file = SourceStack.ToVirtual(e.File ?? "");
            string text = Stack.ReadText(file);
            if (text == null) return null;
            string error;
            var rows = SourceRows.Of(file, text, out error);
            if (rows == null) return null;
            var row = rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
            if (row == null || e.Arg < 0 || e.Arg >= row.Leaves.Count) return null;
            var texts = row.Leaves.Select(l => l.Text).ToList();
            var labels = valueLabels.Of(row.Path, texts, row.LeafNames, row.RawPath);
            var n = NotesFor(SourceStack.ToVirtual(e.Unit ?? file));
            return new EditDescription
            {
                Note = n.Find(row.Path, row.Form, WildcardFor(file, SourceStack.ToVirtual(e.Unit ?? file)), row.RawPath) ?? NotesFor(file).Find(row.Path, row.Form, WildcardFor(file, file), row.RawPath),
                IsMacroCall = row.Form == "macro", ValueLabel = labels[e.Arg], ValueName = row.LeafNames[e.Arg],
                RowText = string.Join(" ", texts), ValueCount = texts.Count,
                ChangedPieces = ChangedPieces(row.Path, row.RawPath, e),
                BlockNames = KeyLabel.QuotedNames(row.Path, row.RawPath),
            };
        }

        // 문자열 속 설정의 수정에서 바뀐 조각들 "뜻(없으면 이름): 원래 → 새 값". 조각 이름이 원래와 같을 때만(아니면 null).
        // 수정 목록(Describe)과 표의 "새 값" 칸이 같이 쓴다
        public List<string> ChangedPieces(string path, string rawPath, Edit e)
        {
            if (!SettingFiles.InArea(SourceStack.ToVirtual(e.File ?? ""))) return null;
            var before = SettingStrings.Split(e.Old);
            var after = SettingStrings.Split(e.New);
            if (before == null || after == null || before.Count != after.Count) return null;
            var result = new List<string>();
            for (int i = 0; i < before.Count; i++)
            {
                if (before[i].Name != after[i].Name) return null;
                if (before[i].Value == after[i].Value) continue;
                result.Add((valueLabels.PieceLabel(path, rawPath, before[i].Name) ?? before[i].Name) + ": " + before[i].Value + " → " + after[i].Value);
            }
            return result;
        }

        // 게임 설정에서 키 끝이 같고 상위 블록만 다른 줄(시작 자원의 MP·SP·AP·RP, 난이도의 enemy·ally·player 등)에
        // 상위 블록의 원문 이름을 부위로 붙인다. 이름이 같은 줄이 없으면 붙이지 않는다
        static void NameNamesakes(List<RowView> rows)
        {
            foreach (var group in rows.Where(r => r.Context == null).GroupBy(r => r.Path.Substring(r.Path.LastIndexOf('/') + 1)))
            {
                var parents = group.Select(r => Parents(r.RawPath ?? r.Path)).ToList();
                int distinct = parents.Select(p => string.Join("/", p)).Distinct(StringComparer.OrdinalIgnoreCase).Count();
                if (distinct < 2) continue;
                // 서로 구분되는 데까지 거슬러 올라간다: MapRewards가 MP·RP에 다 있으면 MP/MapRewards, 두 표의 목록 항목이면 CPLimitStages/#1
                int deepest = parents.Max(p => p.Count);
                for (int depth = 1; depth <= deepest; depth++)
                {
                    var names = parents.Select(p => string.Join("/", p.Skip(Math.Max(0, p.Count - depth)))).ToList();
                    if (depth < deepest && names.Distinct(StringComparer.OrdinalIgnoreCase).Count() < distinct) continue;
                    int i = 0;
                    foreach (var r in group) { r.Context = names[i++]; r.ContextIsNamesake = true; }
                    break;
                }
            }
        }

        // 경로의 마지막 조각을 뺀 상위 조각들(따옴표는 뗀다). /a/b/c → [a, b]
        static List<string> Parents(string path)
        {
            var parts = path.Split('/').Where(s => s.Length > 0).Select(s => s.Trim('"')).ToList();
            if (parts.Count > 0) parts.RemoveAt(parts.Count - 1);
            return parts;
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
            sb.Append(row.Path).Append('\t').Append(row.Form).Append('\t').Append(row.IsMacroCall).Append('\t').Append(row.From);
            foreach (var v in row.Values)
            {
                var t = v.Trace;
                if (v.Editable) sb.Append("\t@").Append(t.File).Append('|').Append(t.KeyPath).Append('|').Append(t.Occurrence).Append('|').Append(t.Arg);
                else sb.Append("\t=").Append(t.Text);
            }
            return sb.ToString();
        }

        // 유닛 파일과 include한 파일의 매크로 호출 줄. 펼친 트리에서는 호출이 본문으로 바뀌어 사라지므로 따로 보인다.
        // 물려받는 원형 파일과 그 include의 호출 줄도 원형의 상태와 함께 보인다(E11). 원형 파일 자신의 줄은 맨 위 블록을 from으로 바꾼다
        void AddMacroCalls(UnitView view, UnitTraceResult traced, KeyAnnotations n, SummaryKeys summary)
        {
            var files = new List<string> { traced.Unit };
            foreach (var e in traced.IncludeEdges) if (!files.Contains(e.ToFile)) files.Add(e.ToFile);
            AddMacroCalls(view, traced, n, summary, files, null);
            foreach (var level in traced.From) AddMacroCalls(view, traced, n, summary, level.Files, level);
        }

        void AddMacroCalls(UnitView view, UnitTraceResult traced, KeyAnnotations n, SummaryKeys summary, List<string> files, FromLevel level)
        {
            foreach (string file in files)
            {
                if (LuaTable.IsLua(file)) continue;   // Lua 설정에는 매크로 호출이 없다
                string text = Stack.ReadText(file);
                if (text == null) continue;
                var parsed = SdlCore.Parse(text);
                if (parsed.Errors.Count > 0) continue;
                var fileRows = KeyLocator.Rows(parsed.Root);
                var keyed = level == null ? null : UnitTracer.KeyedPathsOf(fileRows);
                for (int k = 0; k < fileRows.Count; k++)
                {
                    var row = fileRows[k];
                    // 매크로 정의 안의 호출은 이 유닛이 그 매크로를 쓸 때만 펼쳐지므로 보이지 않는다(쓰는지 알 수 없다)
                    if (row.Form != "macro" || row.Leaves.Count == 0 || row.Path.Contains("(define ")) continue;
                    string path = row.Path, rawPath = row.RawPath, keyedPath = keyed == null ? null : keyed[k];
                    if (level != null && file == level.File)
                    {
                        path = FromLinks.Reroot(row.Path, "/from");
                        rawPath = FromLinks.Reroot(row.RawPath, "/from");
                        keyedPath = FromLinks.Reroot(keyedPath, "/from");
                        if (path == null) continue;
                    }
                    var r = new RowView { Path = path, RawPath = rawPath, Occurrence = row.Occurrence, Form = row.Form, IsMacroCall = true, Cond = row.Cond, Note = n.Find(path, row.Form, WildcardFor(file, view.Unit), rawPath), SummaryLabel = summary.LabelOf(path, rawPath) };
                    if (level != null)
                    {
                        string block;
                        r.From = level.File;
                        r.FromState = level.StateOf(keyedPath, out block);
                        r.FromBlock = block;
                        if (r.FromState == FromLevel.Overridden) r.SummaryLabel = null;
                    }
                    for (int i = 0; i < row.Leaves.Count; i++)
                    {
                        var t = new TraceValue { Text = row.Leaves[i].Text, File = file, KeyPath = row.Path, Occurrence = row.Occurrence, Arg = i };
                        if (KeyLocator.IsNamed(row.Path)) { t.NamedPath = row.RawPath; t.NamedOccurrence = row.RawOccurrence; }
                        // %이름은 바깥에서 받는 매개변수다. 고치면 매크로가 깨진다
                        if (KeyLocator.IsDisabled(row.Path)) { t.Arg = -1; t.Note = KeyLocator.DisabledNote; }
                        else if (t.Text.StartsWith("%")) { t.Arg = -1; t.Note = "매개변수(" + t.Text + ")라 고칠 수 없다. 이 값을 넘기는 쪽에서 고친다"; }
                        r.Values.Add(new ValueView { Trace = t, Name = row.LeafNames[i], SharedBy = Index.UsersOf(file).Count, UnitScopeBlocker = level != null ? FromLevel.UnitScopeNote : IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, file, row.Path) });
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
            if (File.Exists(path)) loaded = EditProject.Load(path);
            else loaded = new EditProject { Name = Path.GetFileNameWithoutExtension(path) };
            Reanchor(loaded.Edits);
            Project = loaded;
            ProjectPath = path;
        }

        // 따옴표 이름 블록의 수정(Q7): 원본에 항목이 끼어들거나 빠져 "몇 번째"가 밀렸으면 이름으로 같은 항목을 다시 찾아 Path·Occurrence를 맞춘다.
        // 그 이름이 원본에 없으면 그대로 두고, 점검·빌드가 "이 이름의 항목이 없다"로 막는다
        void Reanchor(IEnumerable<Edit> edits)
        {
            var indexes = new Dictionary<string, KeyIndex>(StringComparer.Ordinal);
            foreach (var e in edits.Where(x => x.NamedPath != null && x.File != null && !LuaTable.IsLua(x.File)))
            {
                string file = SourceStack.ToVirtual(e.File);
                KeyIndex index;
                if (!indexes.TryGetValue(file, out index))
                {
                    string text = Stack.ReadText(file);
                    var parsed = text == null ? null : SdlCore.Parse(text);
                    index = parsed == null || parsed.Errors.Count > 0 ? null : new KeyIndex(parsed.Root);
                    indexes[file] = index;
                }
                var row = index == null ? null : index.NamedRow(e.NamedPath, e.NamedOccurrence);
                if (row == null) continue;
                e.Path = row.Path;
                e.Occurrence = row.Occurrence;
            }
        }

        public void Dispose()
        {
            if (Stack != null) Stack.Dispose();
        }
    }
}
