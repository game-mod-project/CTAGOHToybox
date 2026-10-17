using System.IO;
using CtaToybox.Cli;
using CtaToybox.Core;
using CtaToybox.Core.L10n;

// 한글화 Task 7: 명령줄 l10n
static class L10nCliTests
{
    public static void Run()
    {
        LayerPlan plan; string outDir;
        var s = L10nSourcesTests.Fixture(out plan, out outDir);
        string settings = Path.Combine(T.TempDir(), "settings.json");
        s.Save(settings);
        string dir = Path.Combine(T.TempDir(), "ko");
        var w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "l10n", "extract", "--settings", settings, "--dir", dir }, w), "extract: " + w);
        T.True(w.ToString().Contains("파일 3, 항목 3"), "extract summary: " + w);
        string po = Path.Combine(dir, "interface", "text", "tips.po");
        T.True(File.Exists(po), "po made");
        File.WriteAllText(po, File.ReadAllText(po).Replace("msgid \"Tank A\"\nmsgstr \"\"", "msgid \"Tank A\"\nmsgstr \"전차 A\""), new System.Text.UTF8Encoding(false));
        w = new StringWriter();
        T.Eq(0, CliApp.Run(new[] { "l10n", "check", "--settings", settings, "--dir", dir }, w), "check passes: " + w);
        T.True(w.ToString().Contains("오류 0, 미번역 2"), "check summary: " + w);

        var running = CliApp.GameRunning;
        try
        {
            CliApp.GameRunning = () => false;
            // 픽스처가 출력 모드에 둔 파일(원문에서 빠지는지 보려던 것)은 목록 밖이라 덮지 않는다. 지우고 빌드한다
            File.Delete(Path.Combine(outDir, "localizations", "default", "interface", "text", "tips.pot"));
            w = new StringWriter();
            T.Eq(0, CliApp.Run(new[] { "l10n", "build", "--settings", settings, "--dir", dir }, w), "build: " + w);
            string pot = File.ReadAllText(Path.Combine(outDir, "localizations", "default", "interface", "text", "tips.pot"));
            T.True(pot.Contains("msgid \"전차 A\""), "build writes korean");

            File.WriteAllText(po, File.ReadAllText(po).Replace("msgstr \"전차 A\"", "msgstr \"전차 A %s\""), new System.Text.UTF8Encoding(false));
            w = new StringWriter();
            T.Eq(1, CliApp.Run(new[] { "l10n", "check", "--settings", settings, "--dir", dir }, w), "a broken placeholder fails the check");
            T.True(w.ToString().Contains("오류 interface/text/tips.po k1 placeholders"), "error line: " + w);
            w = new StringWriter();
            T.Eq(1, CliApp.Run(new[] { "l10n", "build", "--settings", settings, "--dir", dir }, w), "build refuses with check errors");
            CliApp.GameRunning = () => true;
            T.Eq(1, CliApp.Run(new[] { "l10n", "build", "--settings", settings, "--dir", dir }, new StringWriter()), "build refuses while the game runs");
        }
        finally { CliApp.GameRunning = running; }
        T.Eq(2, CliApp.Run(new[] { "l10n", "nope", "--settings", settings }, new StringWriter()), "unknown sub-command");
        T.Eq(2, CliApp.Run(new[] { "l10n" }, new StringWriter()), "no sub-command");
    }
}
