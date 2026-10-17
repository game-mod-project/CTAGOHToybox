using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;
using CtaToybox.Core.L10n;

// 한글화 Task 6: 출력 빌드(build)
static class L10nBuildTests
{
    public static void Run()
    {
        string pot = "msgid \"\"\nmsgstr \"\"\n\"Language: en\\n\"\n\n#. note\nmsgctxt \"desc/entity/kv85\"\nmsgid \"Tank\"\nmsgstr \"\"\n\nmsgctxt \"k2\"\nmsgid \"Attack\"\nmsgstr \"\"\n";
        string catalog = T.TempDir(), outDir = T.TempDir();
        T.PutUtf8(catalog, "interface/text/a.po", "msgid \"\"\nmsgstr \"\"\n\nmsgctxt \"desc/entity/kv85\"\nmsgid \"Tank\"\nmsgstr \"전차\"\n\n#, fuzzy\nmsgctxt \"k2\"\nmsgid \"Attack\"\nmsgstr \"공격\"\n");
        File.WriteAllText(Path.Combine(outDir, ModBuilder.ManifestName), "{\"files\":[{\"path\":\"interface/font/x_12.font\"}]}");
        T.PutUtf8(outDir, "resource/interface/font/x_12.font", "편집기 것");
        T.PutUtf8(outDir, "localizations/default/user.pot", "mine");
        var sources = new List<L10nSource> { new L10nSource { Path = "interface/text/a.pot", Layer = "vanilla", Text = pot }, new L10nSource { Path = "interface/text/b.pot", Layer = "vanilla", Text = pot } };
        var fonts = new List<FontOverride> { new FontOverride { FontPath = "interface/font/x_12.font", Text = "{x}", TargetFnt = "main_12.fnt" }, new FontOverride { FontPath = "interface/font/y_15.font", Text = "{y}", TargetFnt = "main_15.fnt" } };
        var report = L10nBuilder.Build(sources, catalog, fonts, outDir);
        string a = File.ReadAllText(Path.Combine(outDir, "localizations", "default", "interface", "text", "a.pot"));
        T.True(a.Contains("msgctxt \"desc/entity/kv85\"\nmsgid \"전차\""), "translated msgid");
        T.True(a.Contains("msgctxt \"k2\"\nmsgid \"Attack\""), "build_fuzzy_falls_back");
        T.True(a.Contains("Language: ko\\n") && a.Contains("#. note"), "language header ko, comments kept");
        T.True(!a.Contains("\r"), "LF only");
        T.True(File.ReadAllText(Path.Combine(outDir, "localizations", "default", "interface", "text", "b.pot")).Contains("msgid \"Tank\""), "a path without a po is written as source");
        T.Eq(1, report.Translated, "translated counted");
        T.Eq(3, report.FellBack, "fallback counted");
        T.True(report.Errors.Any(e => e.Contains("x_12.font")), "build_never_touches_editor_files: editor build file reported");
        T.Eq("편집기 것", File.ReadAllText(Path.Combine(outDir, "resource", "interface", "font", "x_12.font")), "editor file unchanged");
        T.Eq("{y}", File.ReadAllText(Path.Combine(outDir, "resource", "interface", "font", "y_15.font")), "font override written");
        var names = new DisplayNames();
        names.AddPot(a);
        T.Eq("전차", names.OfUnit("entity/-vehicle/ussr/tank_heavy/kv85/kv85.def"), "the built pot reads back with the editor's pot reader");

        report = L10nBuilder.Build(sources.Take(1).ToList(), catalog, fonts, outDir);
        T.True(!File.Exists(Path.Combine(outDir, "localizations", "default", "interface", "text", "b.pot")) && report.Removed.Contains("localizations/default/interface/text/b.pot"), "stale own file removed");
        T.True(File.Exists(Path.Combine(outDir, "localizations", "default", "user.pot")), "file outside own list kept");
        T.True(File.Exists(Path.Combine(outDir, L10nBuilder.ManifestName)), "own list written");
    }
}
