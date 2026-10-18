// 스폰 모드 생성용 계층. 편집기 계층에서 Spawn Menu 두 모드와 스폰 모드 자신을 뺀다(그 모드의 맵 사본을 이긴 판으로 고르지 않게).
// 스탯 출력 모드는 넣는다: 게임에서 스폰 모드보다 앞에 로드된다고 보고, 그 모드가 바꾼 정의와 사본 유닛도 메뉴에 들어가게 한다(스펙 "입력").
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CtaToybox.Core.Spawn
{
    public static class SpawnSources
    {
        public static readonly string[] ExcludedLayers = { "mod_3617250510", "mod_3295913567" };

        public static string SpawnModDirOf(EditorSettings s)
        {
            if (!string.IsNullOrEmpty(s.SpawnModDir)) return s.SpawnModDir;
            return Path.Combine(Path.GetDirectoryName(Path.GetFullPath(s.OutputModDir)), "CTAToyboxSpawn");
        }

        public static List<SourceLayer> Layers(EditorSettings s, LayerPlan plan, List<string> warnings)
        {
            string spawnRes = Full(Path.Combine(SpawnModDirOf(s), "resource"));
            string statRes = string.IsNullOrEmpty(s.OutputModDir) ? null : Full(Path.Combine(s.OutputModDir, "resource"));
            var result = new List<SourceLayer>();
            foreach (var layer in plan.Layers)
            {
                if (ExcludedLayers.Contains(layer.Name, StringComparer.OrdinalIgnoreCase) || string.Equals(Full(layer.ResourceDir), spawnRes, StringComparison.OrdinalIgnoreCase))
                {
                    warnings?.Add("스폰 생성에서 뺀 계층: " + layer.Name);
                    continue;
                }
                result.Add(layer);
            }
            if (statRes != null && Directory.Exists(statRes) && !result.Any(l => string.Equals(Full(l.ResourceDir), statRes, StringComparison.OrdinalIgnoreCase)))
                result.Add(new SourceLayer("output", statRes));
            return result;
        }

        public static SourceStack Open(EditorSettings s, LayerPlan plan, List<string> warnings)
        {
            var stack = new SourceStack(Layers(s, plan, warnings));
            stack.Scan();
            return stack;
        }

        // 게임 mods 아래의 모드 폴더는 저장소 폴더를 가리키는 정션일 수 있다(mods\ctatoyboxspawn → mod\CTAToyboxSpawn).
        // 정션을 풀지 않으면 스폰 모드 자신의 지난 생성물을 원본으로 읽는다(2026-10-17: 로스터 146개가 지워졌다)
        public static string Full(string path)
        {
            string full = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            // 위로 올라가며 처음 만나는 정션을 그 대상으로 바꾼다(mods\ctatoyboxspawn\resource → mod\CTAToyboxSpawn\resource)
            string tail = "";
            for (string dir = full; !string.IsNullOrEmpty(dir); dir = Path.GetDirectoryName(dir))
            {
                try
                {
                    var info = new DirectoryInfo(dir);
                    if (info.Exists && info.LinkTarget != null)
                    {
                        var target = info.ResolveLinkTarget(true);
                        if (target != null) return Path.GetFullPath(Path.Combine(target.FullName, tail)).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { break; }
                string name = Path.GetFileName(dir);
                if (string.IsNullOrEmpty(name)) break;
                tail = tail.Length == 0 ? name : Path.Combine(name, tail);
            }
            return full;
        }
    }
}
