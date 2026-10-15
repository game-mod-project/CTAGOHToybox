// 게임 설정 파일(하위 프로젝트 B): 컨퀘스트 자원·길이·전투 규칙·AI, 스커미시 모드, 난이도.
// 편집기에만 있는 종류다. tools/lib/Common.ps1의 유닛 종류에는 넣지 않는다(UnitKind.Of는 그대로 둔다).
// 스펙: docs/superpowers/specs/2026-10-15-settings-editor-design.md
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static class SettingFiles
    {
        public const string Kind = "settings";
        public const string UnitScopeNote = "게임 설정은 파일 전체에 적용된다. '전부'만 된다";

        // 목록에 한 항목으로 올리는 맨 위 파일. 이 파일들이 include하는 .inc는 그 항목의 전체 탭에 나온다
        static readonly Regex Top = new Regex(
            @"^(set/dynamic_campaign/(resources|duration)_[^/]+\.set|set/multiplayer/games/[^/]+\.set|set/difficulty/[^/]+\.dl|script/multiplayer/modes/conquest/runtime_config\.lua)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 게임 설정이 있는 폴더. 문자열 속 설정과 단계표 검사는 이 아래 파일에만 한다
        static readonly string[] Areas = { "set/dynamic_campaign/", "set/multiplayer/games/", "set/difficulty/", "script/multiplayer/modes/conquest/" };

        public static bool IsTop(string virtualPath) { return virtualPath != null && Top.IsMatch(virtualPath); }

        public static bool InArea(string virtualPath)
        {
            if (virtualPath == null) return false;
            foreach (string a in Areas) if (virtualPath.StartsWith(a, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public sealed class Entry
        {
            public string Group { get; set; }
            public string Name { get; set; }
        }

        // editor/settings/files.psv: 경로|묶음|화면 이름. #으로 시작하는 줄과 빈 줄은 건너뛴다. 파일이 없으면 빈 표
        public static Dictionary<string, Entry> LoadNames(string psvPath)
        {
            var result = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(psvPath)) return result;
            foreach (string line in File.ReadLines(psvPath, Encoding.UTF8))
            {
                if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                string[] p = line.Split('|');
                if (p.Length < 3) throw new FormatException("게임 설정 이름 줄 형식 오류(경로|묶음|화면 이름): " + line);
                result[p[0].Trim()] = new Entry { Group = p[1].Trim(), Name = p[2].Trim() };
            }
            return result;
        }
    }
}
