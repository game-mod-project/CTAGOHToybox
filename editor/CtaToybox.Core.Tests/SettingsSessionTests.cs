// 게임 설정을 세션·명령줄에서 연다: 이름, 종류, 주석 묶음, '전부'만, 모드가 덮어쓴 파일
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

static class SettingsSessionTests
{
    // 한글이 든 저장소 파일은 UTF-8로 쓴다(T.Put은 Latin1이라 한글이 깨진다)
    static void Utf8(string root, string relativePath, string text)
    {
        string p = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(p));
        File.WriteAllText(p, text);
    }

    // 게임 설정 파일이 든 가짜 게임과 저장소. 모드 mod_222가 resources_standard.set을 덮어쓴다
    public static string Fixture(out string repo, out string outDir, out string game)
    {
        string workshop = T.TempDir(), work = T.TempDir();
        game = T.TempDir(); repo = T.TempDir(); outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), SettingFilesTests.Files());
        T.Put(Path.Combine(workshop, "222", "resource"), SettingFilesTests.Res,
            "{Standard\n\t{GlobalMaxCP 2000}\n\t{Resources\n\t\t{MP {StartVal 1500}}\n\t\t{RP {StartVal 7}}\n\t}\n}");
        Utf8(repo, "data/annotations/common.psv", "*/mass|block|확인|무게|C\n");
        Utf8(repo, "data/annotations/settings.psv", "**/globalmaxcp|block|확인|전체 CP 상한|C\n*/startval|block|확인|시작할 때의 양|C\n");
        Utf8(repo, "editor/settings/files.psv", SettingFilesTests.Res + "|컨퀘스트 자원|컨퀘스트 자원 — standard\n" + SettingFilesTests.Dl + "|난이도|난이도 — normal\n");
        Utf8(repo, "editor/summary/settings.psv", "**/globalmaxcp|전체 CP 상한\n*/startval|시작 자원\n");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"mod_222:0\" \"" + Path.GetFileName(outDir) + ":0\"}}");
        // 실제 저장소처럼 settings.json을 editor/에 둔다(CLI는 그 옆의 settings/files.psv에서 이름을 읽는다)
        string settings = Path.Combine(repo, "editor", "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    static int Cli(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    public static void Run()
    {
        string repo, outDir, game;
        string settings = Fixture(out repo, out outDir, out game);
        using (var session = EditorSession.Open(settings, repo, null))
        {
            var found = session.Search(null, "settings", null).Select(u => u.Path).ToList();
            T.True(found.Contains(SettingFilesTests.Res) && found.Contains(SettingFilesTests.Bz), "the settings kind lists setting files");
            T.True(found.Contains(SettingFilesTests.Dur), "a settings file with only string values is listed too");
            T.Eq("컨퀘스트 자원 — standard", session.Names.OfUnit(SettingFilesTests.Res), "the name from files.psv");
            T.Eq("컨퀘스트 자원", session.KindLabel(session.Index.Units.First(u => u.Path == SettingFilesTests.Res)), "the group as the kind label");
            T.Eq("게임 설정", session.KindLabel(session.Index.Units.First(u => u.Path == SettingFilesTests.Bz)), "a file without a name falls back to the kind");

            var res = session.OpenUnit(SettingFilesTests.Res);
            T.Eq("settings", res.Kind, "kind");
            T.Eq("settings", res.EditorKind, "editor kind");
            T.Eq("mod_222", res.Layer, "the winning layer is the mod that overrides the file");
            var max = res.Rows.First(r => r.Path == "/standard/globalmaxcp");
            T.Eq("2000", max.Values[0].Trace.Text, "the mod's value");
            T.Eq("전체 CP 상한", max.Note.Meaning, "notes come from settings.psv");
            T.Eq("전체 CP 상한", max.SummaryLabel, "the settings summary file");
            T.True(res.Rows.SelectMany(r => r.Values).Where(v => v.Editable).All(v => v.UnitScopeBlocker == SettingFiles.UnitScopeNote), "every value is shared-only");
            string warning;
            T.Eq(SettingFiles.UnitScopeNote, session.ValidateNew(res, max.Values[0], "2100", "unit", out warning), "unit scope is refused");

            var bz = session.OpenUnit(SettingFilesTests.Bz);
            var limits = bz.Rows.First(r => r.Path.EndsWith("/unitlimits"));
            T.Eq(SettingFilesTests.Limits, limits.Values[0].Trace.File, "an included preset traces to its file");

            // 조각과 단계표: 화면이 쓰는 값
            var parameters = bz.Rows.First(r => r.Path == "/game/parameters").Values[0];
            T.Eq(new[] { "flag_capture_time", "scoreStart" }, parameters.Pieces.Select(x => x.Name).ToArray(), "the session splits piece strings");
            T.Eq(2, parameters.PieceLabels.Count, "with one label slot per piece");
            var dur = session.OpenUnit(SettingFilesTests.Dur);
            T.Eq("2 → 5, 2 → 5, 3 → 5", dur.Rows.First(r => r.Path == "/normal/flagcount").Values[0].Steps, "step tables get a preview");
            T.True(parameters.Steps == null && dur.Rows.First(r => r.Path == "/normal/flagcount").Values[0].Pieces == null, "a piece string is not a step table and the other way round");

            // 같은 문자열의 조각을 차례로 고쳐도 앞의 것이 남는다(창은 기존 수정의 새 값에서 시작한다)
            session.OpenProject(Path.Combine(T.TempDir(), "pieces.json"));
            var row = bz.Rows.First(r => r.Path == "/game/parameters");
            session.ApplyRow(bz, row, new[] { SettingStrings.Join(row.Values[0].Trace.Text, new[] { "61", "250" }) }, "shared");
            session.RefreshEdits(bz);
            string current = row.Values[0].Edit.New;
            var start = SettingStrings.Split(current);
            session.ApplyRow(bz, row, new[] { SettingStrings.Join(row.Values[0].Trace.Text, new[] { start[0].Value, "777" }) }, "shared");
            T.Eq("\"flag_capture_time=61;scoreStart=777\"", session.Project.Edits.Single().New, "both piece changes are kept in one edit");
            T.Eq(new[] { "flag_capture_time: 60 → 61", "scoreStart: 250 → 777" }, session.Describe(session.Project.Edits.Single()).ChangedPieces.ToArray(), "the edit list names only the changed pieces");

            // 키 끝이 같고 상위 블록만 다른 줄은 상위 블록 이름을 부위로 보인다(시작 자원 MP·RP)
            var res2 = session.OpenUnit(SettingFilesTests.Res);
            T.Eq("MP", res2.Rows.First(r => r.Path == "/standard/resources/mp/startval").Context, "StartVal of MP");
            T.True(res2.Rows.First(r => r.Path == "/standard/globalmaxcp").Context == null, "a key with no namesake gets no context");
        }

        string o;
        T.Eq(0, Cli(out o, "units", "--settings", settings, "--kind", "settings"), "units --kind settings");
        T.True(o.Contains(SettingFilesTests.Res) && o.Contains("컨퀘스트 자원 — standard"), "lists settings with their names");
        T.True(o.Contains(SettingFilesTests.Dur), "and the string-only duration file");
        T.Eq(0, Cli(out o, "trace", SettingFilesTests.Res, "--settings", settings), "trace a settings file");
        T.True(o.Contains("(settings, 계층 mod_222)"), "trace names the settings kind");
        string project = Path.Combine(T.TempDir(), "p.json");
        T.Eq(1, Cli(out o, "add", "--settings", settings, "--project", project, "--file", SettingFilesTests.Res, "--path", "/standard/globalmaxcp", "--new", "2100", "--scope", "unit", "--unit", SettingFilesTests.Res), "add --scope unit is refused for settings");
        T.True(o.Contains(SettingFiles.UnitScopeNote), "with the reason");
        T.Eq(0, Cli(out o, "add", "--settings", settings, "--project", project, "--file", SettingFilesTests.Res, "--path", "/standard/globalmaxcp", "--new", "2100"), "add shared works");
    }
}
