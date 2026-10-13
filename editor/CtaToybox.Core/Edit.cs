// 수정 한 건과 프로젝트 파일(editor/projects/<이름>.json).
// Old와 New는 따옴표를 포함한 토큰 원문이다: 문자열이면 "body", 낱말이나 수면 120.
using System.Collections.Generic;
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
    }

    public sealed class EditProject
    {
        public string Name { get; set; }
        public List<Edit> Edits { get; set; } = new List<Edit>();

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static EditProject Load(string path)
        {
            return JsonSerializer.Deserialize<EditProject>(System.IO.File.ReadAllText(path, Encoding.UTF8), Json);
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)));
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(this, Json) + "\n", new UTF8Encoding(false));
        }
    }
}
