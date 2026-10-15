// 게임 설정 화면 다듬기(docs/10-backlog.md W-C): 이름이 같은 줄 가르기, 바뀐 조각만 보이기
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class SettingsPolishTests
{
    const string Res = SettingFilesTests.Res, Lua = "script/multiplayer/modes/conquest/runtime_config.lua";

    public static void Run()
    {
        string workshop = T.TempDir(), work = T.TempDir(), game = T.TempDir(), repo = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            (Res, "{Standard\n\t{Resources\n\t\t{MP {StartVal 900} {MapRewards {Airfield 200} {Bonus 400}}}\n\t\t{RP {StartVal 6} {MapRewards {Airfield 6}}}\n\t}\n}"),
            (Lua, "ConquestConfig.Purchase = {\n\tCPLimitStages = {\n\t\t{ band = \"Stage1\", max = 60 },\n\t\t{ band = \"Stage2\", max = 140 },\n\t},\n\tCommitment = {\n\t\t{ band = \"Low\", max = 0.49 },\n\t},\n}\n"),
            (SettingFilesTests.Bz, "{game\n\t{parameters \"flag_capture_time=60;scoreStart=250\"}\n}"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(repo, "editor", "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);

        using (var session = EditorSession.Open(settings, repo, null))
        {
            // E6: 이름이 같은 줄은 서로 구분되는 데까지 상위 블록을 거슬러 올라간다
            var res = session.OpenUnit(Res);
            T.Eq("MP/MapRewards", res.Rows.First(r => r.Path == "/standard/resources/mp/maprewards/airfield").Context, "MapRewards of MP: one level up is the same for all, so two levels");
            T.Eq("RP/MapRewards", res.Rows.First(r => r.Path == "/standard/resources/rp/maprewards/airfield").Context, "MapRewards of RP");
            T.Eq("MP", res.Rows.First(r => r.Path == "/standard/resources/mp/startval").Context, "one level when that already tells them apart");
            T.True(res.Rows.First(r => r.Path == "/standard/resources/mp/maprewards/bonus").Context == null, "a key with no namesake gets no context");
            var lua = session.OpenUnit(Lua);
            T.Eq("CPLimitStages/#1", lua.Rows.First(r => r.Path == "/conquestconfig.purchase/cplimitstages/#1/band").Context, "list items of two tables: the table name and the position");
            T.Eq("CPLimitStages/#2", lua.Rows.First(r => r.Path == "/conquestconfig.purchase/cplimitstages/#2/band").Context, "the second item");
            T.Eq("Commitment/#1", lua.Rows.First(r => r.Path == "/conquestconfig.purchase/commitment/#1/band").Context, "the other table's first item");

            // E8: 표의 "새 값" 칸도 수정 목록처럼 바뀐 조각만 보인다(화면과 수정 목록이 같은 함수를 쓴다)
            session.OpenProject(Path.Combine(work, "pieces.json"));
            var bz = session.OpenUnit(SettingFilesTests.Bz);
            var row = bz.Rows.First(r => r.Path == "/game/parameters");
            session.ApplyRow(bz, row, new[] { "\"flag_capture_time=61;scoreStart=250\"" }, "shared");
            var edit = session.Project.Edits.Single();
            T.Eq(new[] { "flag_capture_time: 60 → 61" }, session.ChangedPieces(row.Path, row.RawPath, edit).ToArray(), "only the changed piece");
            T.Eq(session.Describe(edit).ChangedPieces.ToArray(), session.ChangedPieces(row.Path, row.RawPath, edit).ToArray(), "the same text as the edit list");
            T.True(session.ChangedPieces("/standard/globalmaxcp", null, new Edit { File = Res, Old = "550", New = "600" }) == null, "not a piece string: null");
        }
    }
}
