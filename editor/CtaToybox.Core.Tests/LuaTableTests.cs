// 컨퀘스트 AI 설정 Lua 읽기: 경로, 순서 목록, 따옴표 키, 주석, 수식, 모르는 문법 거부
using System.Linq;
using CtaToybox.Core;

static class LuaTableTests
{
    // runtime_config.lua의 꼴을 줄인 것
    public const string Sample =
        "-- RUNTIME CONFIG\n" +
        "ConquestConfig = ConquestConfig or {}\n" +
        "\n" +
        "-- Time from match start before the AI attempts its first purchase.\n" +
        "ConquestConfig.StartSpawnTime = {\n" +
        "\t-- Bot is attacker.\n" +
        "\tAttackMin = 1 * 1000,\n" +
        "\tAttackMax = 1 * 1000, -- upper bound\n" +
        "}\n" +
        "ConquestConfig.AttackerWaves = {\n" +
        "\tPattern = \"escalating\",\n" +
        "\tFlag = true,\n" +
        "\tDelta = -15,\n" +
        "\tescalating = {\n" +
        "\t\tFirst = { Scale = 0.85, Intent = \"probe\" },\n" +
        "\t},\n" +
        "\tBands = {\n" +
        "\t\t{ band = \"Stage1\", max = 3 },\n" +
        "\t\t{ band = \"Stage2\", max = nil },\n" +
        "\t},\n" +
        "\tRatio = {\n" +
        "\t\t[\"0.9\"] = 2.00,\n" +
        "\t},\n" +
        "}\n" +
        "\n" +
        "-- Maximum time the AI waits.\n" +
        "ConquestConfig.UnitSpawnWaitTime = 1.5 * 60000 -- 1:30 min (ms)\n" +
        "\n" +
        "ConquestConfig.OrderRotationPeriod = 2.5 * 60000\n";

    public static void Run()
    {
        var r = LuaTable.Read(Sample);
        T.Eq(0, r.Errors.Count, "the sample reads");
        T.Eq(new[]
        {
            "/conquestconfig.startspawntime/attackmin", "/conquestconfig.startspawntime/attackmax",
            "/conquestconfig.attackerwaves/pattern", "/conquestconfig.attackerwaves/flag", "/conquestconfig.attackerwaves/delta",
            "/conquestconfig.attackerwaves/escalating/first/scale", "/conquestconfig.attackerwaves/escalating/first/intent",
            "/conquestconfig.attackerwaves/bands/#1/band", "/conquestconfig.attackerwaves/bands/#1/max",
            "/conquestconfig.attackerwaves/bands/#2/band", "/conquestconfig.attackerwaves/bands/#2/max",
            "/conquestconfig.attackerwaves/ratio/\"*\"",
            "/conquestconfig.unitspawnwaittime", "/conquestconfig.orderrotationperiod",
        }, r.Rows.Select(x => x.Path).ToArray(), "paths: lowercase, positional #n, quoted keys as \"*\", X = X or {} makes no row");
        var min = r.Rows[0];
        T.Eq("/ConquestConfig.StartSpawnTime/AttackMin", min.RawPath, "the raw path keeps case");
        T.Eq("1 * 1000", min.Value, "the value text");
        T.Eq(Sample.IndexOf("1 * 1000"), min.Index, "the value position");
        T.Eq("expr", min.Kind, "an expression");
        T.Eq("Bot is attacker.", min.Comment, "the comment line above a field");
        T.Eq("Time from match start before the AI attempts its first purchase.", min.TableComment, "the comment above the table");
        T.Eq("upper bound", r.Rows[1].Comment, "a trailing comment wins");
        T.Eq("string", r.Rows[2].Kind, "a string");
        T.Eq("\"escalating\"", r.Rows[2].Value, "string values keep their quotes");
        T.Eq("bool", r.Rows[3].Kind, "a boolean");
        T.Eq("number", r.Rows[4].Kind, "a negative number is a number");
        T.Eq("nil", r.Rows[10].Kind, "nil");
        T.Eq("/ConquestConfig.AttackerWaves/Ratio/\"0.9\"", r.Rows[11].RawPath, "a quoted key keeps its name in the raw path");
        T.Eq("1:30 min (ms)", r.Rows[12].Comment, "a top-level scalar with a trailing comment");
        T.True(r.Rows[13].Comment == null, "no comment");

        double d;
        T.True(LuaTable.TryEvaluate("2.5 * 60000", out d) && d == 150000, "evaluate");
        T.True(LuaTable.TryEvaluate("-(2 + 3) * 4 / 2", out d) && d == -10, "evaluate with parentheses and unary minus");
        T.True(!LuaTable.TryEvaluate("2 * x", out d) && !LuaTable.TryEvaluate("1 / 0", out d), "names and infinity do not evaluate");

        T.Eq("expr", LuaTable.KindOf("3 * 60000"), "KindOf expression");
        T.Eq("number", LuaTable.KindOf(" 7 "), "KindOf number");
        T.Eq("string", LuaTable.KindOf("\"probe\""), "KindOf string");
        T.True(LuaTable.KindOf("2 * x") == null && LuaTable.KindOf("{}") == null && LuaTable.KindOf("print(1)") == null && LuaTable.KindOf("") == null, "KindOf refuses names, tables and calls");
        T.True(LuaTable.KindOf("5 -- x") == null && LuaTable.KindOf("5;") == null && LuaTable.KindOf("5 --[[x]]") == null && LuaTable.KindOf("5--1") == null,
            "KindOf refuses a trailing comment or ; (the value must be the whole input; in a table it would break the file)");

        foreach (string bad in new[] { "local x = 1", "X = function() end", "X = { a = 1 + y }", "X = { [1] = 2 }", "X = { a = 1", "X = 1 ..", "X = \"open" })
        {
            var b = LuaTable.Read(bad);
            T.True(b.Errors.Count > 0 && b.Rows.Count == 0, "unknown syntax is refused without rows: " + bad);
        }
        T.True(LuaTable.IsLua("script/multiplayer/modes/conquest/runtime_config.lua") && !LuaTable.IsLua("set/a.set"), "IsLua");
    }
}
