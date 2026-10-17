// 수정 한 건과 프로젝트 파일(editor/projects/<이름>.json).
// Old와 New는 따옴표를 포함한 토큰 원문이다: 문자열이면 "body", 낱말이나 수면 120.
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class Edit
    {
        public string Unit { get; set; }        // 어느 유닛을 열어 놓고 고쳤는지 (가상 경로)
        public string File { get; set; }        // 값이 실제로 있는 파일 (가상 경로)
        public string Path { get; set; }        // 그 파일 안의 키 경로
        public int Occurrence { get; set; }     // 같은 경로 가운데 몇 번째인지
        public int Arg { get; set; }            // 그 행의 몇 번째 값인지 (KeyLocator의 규칙)
        public string Old { get; set; }
        public string New { get; set; }
        public string Scope { get; set; } = "shared";   // shared | unit
        public bool Enabled { get; set; } = true;
        // 따옴표 이름 블록 안의 자리면 이름이 든 원문 경로(예 /"tiger"/cost)와 그 경로 가운데 몇 번째인지(Q7).
        // 모드가 앞에 항목을 끼워 넣어 Occurrence가 밀려도 프로젝트를 열 때 이름으로 다시 찾는다. 이름이 없는 자리면 null
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public string NamedPath { get; set; }
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public int NamedOccurrence { get; set; }
        // 무엇을 고치는지. 값이면 null(지금까지의 프로젝트 파일 그대로), 인자 없는 매크로 호출의 이름이면 MacroSwap.Kind(E4), 따옴표 이름 블록 복제면 BlockCopy.Kind(E18)
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public string Kind { get; set; }
        // 일괄 규칙(F1)이 만든 수정이면 그 규칙의 id. 사람이 만든 수정은 null이다(파일에 쓰지 않는다)
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public string Rule { get; set; }
    }

    // 프로젝트 합치기(EditProject.MergeFrom)의 결과
    public sealed class MergeReport
    {
        public int Added { get; set; }
        public int Same { get; set; }
        public List<string> Conflicts { get; } = new List<string>();
    }

    public sealed class EditProject
    {
        public string Name { get; set; }
        public List<Edit> Edits { get; set; } = new List<Edit>();
        // 일괄 규칙(F1). 목록 순서가 겹치는 자리의 우선순위다. 없으면 null이라 파일에 쓰지 않는다(예전 프로젝트 파일과 같다)
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public List<BulkRule> Rules { get; set; }

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // 깨진 JSON은 파일 이름과 이유를 담은 InvalidDataException으로 던진다
        public static EditProject Load(string path)
        {
            EditProject loaded;
            try { loaded = JsonSerializer.Deserialize<EditProject>(System.IO.File.ReadAllText(path, Encoding.UTF8), Json); }
            catch (JsonException ex) { throw new InvalidDataException("프로젝트 파일을 읽지 못했다: " + path + ": " + ex.Message, ex); }
            if (loaded == null) throw new InvalidDataException("프로젝트 파일을 읽지 못했다(내용이 비어 있다): " + path);
            if (loaded.Edits == null) loaded.Edits = new List<Edit>();
            return loaded;
        }

        // other의 수정을 이 프로젝트에 더한다(E17: 빌드는 프로젝트 하나만 쓴다). 같은 자리(범위, '이 유닛만'이면 유닛, 파일, 키 경로, 몇 번째, 몇 번째 값)에
        // 새 값까지 같으면 건너뛰고, 새 값이 다르면 이 프로젝트의 것을 두고 충돌로 알린다. 더하는 수정은 사본이다(켜짐·이름 경로 포함)
        public MergeReport MergeFrom(EditProject other)
        {
            var report = new MergeReport();
            // 규칙이 만든 수정은 옮기지 않는다. 규칙을 붙인 뒤 다시 계산해 만든다(F1, 세션의 MergeProject)
            foreach (var e in other.Edits.Where(x => x.Rule == null))
            {
                var here = Edits.FirstOrDefault(x => SameSlot(x, e));
                if (here == null)
                {
                    Edits.Add(new Edit { Unit = e.Unit, File = e.File, Path = e.Path, Occurrence = e.Occurrence, Arg = e.Arg, Old = e.Old, New = e.New, Scope = e.Scope, Enabled = e.Enabled, NamedPath = e.NamedPath, NamedOccurrence = e.NamedOccurrence, Kind = e.Kind });
                    report.Added++;
                }
                else if (here.New == e.New) report.Same++;
                else report.Conflicts.Add(e.File + " " + e.Path + " [" + e.Occurrence + "] 값 " + e.Arg + (e.Scope == "unit" ? " (이 유닛만: " + e.Unit + ")" : "") + ": 가져올 값 " + e.New + ", 이 프로젝트의 값 " + here.New + "(그대로 둔다)");
            }
            return report;
        }

        static bool SameSlot(Edit a, Edit b)
        {
            // 줄 더하기·파일 사본은 같은 자리에 여럿을 둘 수 있다. 넣는 줄·사본 경로가 같아야 같은 수정이다
            if ((a.Kind == AddLine.Kind || a.Kind == FileCopy.Kind) && a.New != b.New) return false;
            return a.Scope == b.Scope && a.Kind == b.Kind && a.Occurrence == b.Occurrence && a.Arg == b.Arg
                && string.Equals(SourceStack.ToVirtual(a.File ?? ""), SourceStack.ToVirtual(b.File ?? ""), System.StringComparison.OrdinalIgnoreCase)
                && string.Equals(a.Path, b.Path, System.StringComparison.OrdinalIgnoreCase)
                && (a.Scope != "unit" || string.Equals(SourceStack.ToVirtual(a.Unit ?? ""), SourceStack.ToVirtual(b.Unit ?? ""), System.StringComparison.OrdinalIgnoreCase));
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)));
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(this, Json) + "\n", new UTF8Encoding(false));
        }
    }
}
