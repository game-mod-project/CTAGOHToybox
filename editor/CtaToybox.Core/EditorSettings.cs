// 이 PC의 경로 설정. editor/settings.json에 저장하며 git에 넣지 않는다.
using System.IO;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class EditorSettings
    {
        public string GameDir { get; set; }          // call_to_arms.exe가 있는 binaries의 부모. resource, mods 폴더가 있다
        public string WorkshopDir { get; set; }      // ...\steamapps\workshop\content\400750
        public string OptionsSetPath { get; set; }   // 프로필의 options.set. 읽기만 한다
        public string OutputModDir { get; set; }     // mod.info가 있는 폴더 (이 저장소의 mod\CTAToybox)
        public string OutputModName { get; set; }    // 게임의 mods 아래 폴더 이름

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // 깨진 JSON은 파일 이름과 이유를 담은 InvalidDataException으로 던진다(화면과 명령줄이 스택 트레이스 대신 이유를 보인다)
        public static EditorSettings Load(string path)
        {
            EditorSettings loaded;
            try { loaded = JsonSerializer.Deserialize<EditorSettings>(File.ReadAllText(path, Encoding.UTF8), Json); }
            catch (JsonException ex) { throw new InvalidDataException("설정 파일을 읽지 못했다: " + path + ": " + ex.Message, ex); }
            if (loaded == null) throw new InvalidDataException("설정 파일을 읽지 못했다(내용이 비어 있다): " + path);
            return loaded;
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            File.WriteAllText(path, JsonSerializer.Serialize(this, Json) + "\n", new UTF8Encoding(false));
        }
    }
}
