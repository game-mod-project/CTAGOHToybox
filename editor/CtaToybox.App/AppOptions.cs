// 명령줄 옵션. 저장소 루트를 찾고 기본 경로를 정한다.
using System;
using System.Collections.Generic;
using System.IO;

namespace CtaToybox.App
{
    public sealed class AppOptions
    {
        public string Repo, Settings, Project, Unit, Tab, Filter, Screenshot;

        public static AppOptions Parse(string[] args)
        {
            var opt = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("--")) throw new ArgumentException("모르는 인자: " + args[i]);
                if (i + 1 >= args.Length) throw new ArgumentException(args[i] + " 뒤에 값이 없다");
                opt[args[i].Substring(2)] = args[++i];
            }
            foreach (string k in opt.Keys)
                if (Array.IndexOf(new[] { "repo", "settings", "project", "unit", "tab", "filter", "screenshot" }, k.ToLowerInvariant()) < 0) throw new ArgumentException("모르는 옵션: --" + k);
            var o = new AppOptions();
            o.Repo = Path.GetFullPath(opt.ContainsKey("repo") ? opt["repo"] : FindRepo(Environment.CurrentDirectory) ?? FindRepo(AppContext.BaseDirectory) ?? Environment.CurrentDirectory);
            o.Settings = opt.ContainsKey("settings") ? opt["settings"] : Path.Combine(o.Repo, "editor", "settings.json");
            o.Project = opt.ContainsKey("project") ? opt["project"] : Path.Combine(o.Repo, "editor", "projects", "my-tweaks.json");
            o.Unit = opt.ContainsKey("unit") ? opt["unit"] : null;
            o.Tab = opt.ContainsKey("tab") ? opt["tab"] : null;
            o.Filter = opt.ContainsKey("filter") ? opt["filter"] : null;
            o.Screenshot = opt.ContainsKey("screenshot") ? Path.GetFullPath(opt["screenshot"]) : null;
            return o;
        }

        // editor와 data가 둘 다 있는 가장 가까운 상위 폴더
        static string FindRepo(string start)
        {
            for (var d = new DirectoryInfo(start); d != null; d = d.Parent)
                if (Directory.Exists(Path.Combine(d.FullName, "editor")) && Directory.Exists(Path.Combine(d.FullName, "data"))) return d.FullName;
            return null;
        }
    }
}
