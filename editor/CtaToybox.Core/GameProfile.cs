// 프로필의 options.set에서 켜 둔 모드 목록을 읽어 원본 계층을 정한다. options.set은 읽기만 한다.
// 창작마당 모드는 "mod_<숫자>:<숫자>"다. 그 밖의 항목은 ':' 앞을 로컬 모드 폴더 이름으로 본다(가정. 확인 기록은 docs/03-mod-folder-setup.md).
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public sealed class LayerPlan
    {
        public List<SourceLayer> Layers { get; } = new List<SourceLayer>();
        public List<string> Warnings { get; } = new List<string>();
        public bool OutputModActive { get; set; }
        public List<string> LayersAfterOutput { get; } = new List<string>();   // 출력 모드보다 뒤에 로드되는 모드
    }

    public static class GameProfile
    {
        static readonly Regex Workshop = new Regex(@"^mod_(\d+)(:|$)", RegexOptions.Compiled);

        public static List<string> ReadModEntries(string optionsSetText)
        {
            var result = new List<string>();
            var mods = FindBlock(SdlCore.Parse(optionsSetText).Root, "mods");
            if (mods == null) return result;
            for (int i = 1; i < mods.Children.Count; i++)
            {
                var n = SdlCore.Unwrap(mods.Children[i]);
                if (n.Kind == "str" || n.Kind == "atom") result.Add(n.Text.Trim('"'));
            }
            return result;
        }

        static SdlNode FindBlock(SdlNode node, string name)
        {
            if (node.Children == null) return null;
            if (node.Kind == "brace" && node.Children.Count > 0)
            {
                var head = SdlCore.Unwrap(node.Children[0]);
                if (head.Kind == "atom" && string.Equals(head.Text, name, StringComparison.OrdinalIgnoreCase)) return node;
            }
            foreach (object c in node.Children)
            {
                var found = FindBlock(SdlCore.Unwrap(c), name);
                if (found != null) return found;
            }
            return null;
        }

        public static LayerPlan PlanLayers(EditorSettings settings, IEnumerable<string> modEntries)
        {
            var plan = new LayerPlan();
            plan.Layers.Add(new SourceLayer("vanilla", Path.Combine(settings.GameDir, "resource")));
            foreach (string entry in modEntries)
            {
                var m = Workshop.Match(entry);
                string name, dir;
                if (m.Success)
                {
                    name = "mod_" + m.Groups[1].Value;
                    dir = Path.Combine(settings.WorkshopDir, m.Groups[1].Value, "resource");
                }
                else
                {
                    int colon = entry.IndexOf(':');
                    name = colon < 0 ? entry : entry.Substring(0, colon);
                    // 출력 모드는 이름이나 폴더로 알아본다. 이름이 다르면 그 폴더를 원본으로 읽어 이전 빌드의 생성물이 원본에 섞이므로 폴더도 본다
                    bool sameName = string.Equals(name, settings.OutputModName, StringComparison.OrdinalIgnoreCase);
                    if (sameName || IsOutputFolder(settings, Path.Combine(settings.GameDir, "mods", name)))
                    {
                        if (!sameName) plan.Warnings.Add("options.set의 " + name + " 폴더가 출력 모드 폴더인데 설정의 출력 모드 이름은 " + settings.OutputModName + "이다. 설정의 이름을 " + name + "로 맞춘다.");
                        plan.OutputModActive = true;
                        continue;
                    }
                    dir = Path.Combine(settings.GameDir, "mods", name, "resource");
                }
                if (!Directory.Exists(dir)) plan.Warnings.Add("모드 " + name + "의 폴더가 없다: " + dir);
                plan.Layers.Add(new SourceLayer(name, dir));
                if (plan.OutputModActive) plan.LayersAfterOutput.Add(name);
            }
            if (!plan.OutputModActive)
                plan.Warnings.Add("출력 모드 " + settings.OutputModName + "가 게임에서 켜져 있지 않다. 빌드해도 게임에 반영되지 않는다.");
            else if (plan.LayersAfterOutput.Count > 0)
                plan.Warnings.Add("출력 모드보다 뒤에 로드되는 모드가 있다: " + string.Join(", ", plan.LayersAfterOutput) + ". 이 모드들의 파일을 고친 수정은 덮어써진다. options.set의 {mods} 목록에서 " + settings.OutputModName + " 줄을 맨 뒤로 옮긴다.");
            return plan;
        }

        // 게임의 mods 아래 폴더가 출력 모드 폴더 자신이거나 그곳을 가리키는 정션인가
        static bool IsOutputFolder(EditorSettings settings, string folder)
        {
            if (string.IsNullOrEmpty(settings.OutputModDir) || !Directory.Exists(folder)) return false;
            string output = Full(settings.OutputModDir);
            if (string.Equals(Full(folder), output, StringComparison.OrdinalIgnoreCase)) return true;
            try
            {
                var target = new DirectoryInfo(folder).ResolveLinkTarget(true);
                return target != null && string.Equals(Full(target.FullName), output, StringComparison.OrdinalIgnoreCase);
            }
            catch (IOException) { return false; }
        }

        static string Full(string path) { return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar); }
    }
}
