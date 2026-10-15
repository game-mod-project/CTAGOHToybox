// Lua 값 바꾸기: 범위 교체, 종류 검사, 원본 바뀜, 다시 읽기, 빌드
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class LuaPatcherTests
{
    const string Lua = "script/multiplayer/modes/conquest/runtime_config.lua";

    static Edit E(string path, string old, string @new, int occ = 0)
    {
        return new Edit { Unit = Lua, File = Lua, Path = path, Occurrence = occ, Arg = 0, Old = old, New = @new };
    }

    public static void Run()
    {
        string warning;
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "3 * 60000", out warning) == null, "an expression for an expression");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "180000", out warning) == null, "a number for an expression");
        T.True(LuaPatcher.CheckNewValue("\"escalating\"", "\"flat\"", out warning) == null, "a string for a string");
        T.True(LuaPatcher.CheckNewValue("true", "false", out warning) == null, "a boolean for a boolean");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "2 * x", out warning) != null, "a name in an expression is refused");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "{}", out warning) != null, "a table is refused");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "print(1)", out warning) != null, "a call is refused");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "\"5\"", out warning) != null, "a string for a number is refused");
        T.True(LuaPatcher.CheckNewValue("\"a\"", "'a'", out warning) != null, "the quote kind stays");
        T.True(LuaPatcher.CheckNewValue("nil", "3", out warning) != null, "nil is not editable");
        T.True(LuaPatcher.CheckNewValue("3", "", out warning) != null, "an empty value is refused");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "5;", out warning) != null && LuaPatcher.CheckNewValue("1 * 1000", "5 -- x", out warning) != null, "a trailing ; or comment is refused");
        T.True(LuaPatcher.SameValue("1 * 1000", "1*1000") && !LuaPatcher.SameValue("\"a b\"", "\"ab\""), "SameValue ignores spaces only in numbers");

        string text = LuaTableTests.Sample;
        var results = new List<EditResult>();
        string patched = LuaPatcher.Apply(text, new[]
        {
            E("/conquestconfig.startspawntime/attackmin", "1 * 1000", "5 * 60000"),
            E("/conquestconfig.attackerwaves/pattern", "\"escalating\"", "\"flat\""),
            E("/conquestconfig.unitspawnwaittime", "1.5 * 60000", "90000"),
        }, results);
        T.True(results.All(r => r.Status == EditStatus.Applied), "all applied");
        T.True(patched.Contains("AttackMin = 5 * 60000,") && patched.Contains("Pattern = \"flat\",") && patched.Contains("UnitSpawnWaitTime = 90000 -- 1:30 min (ms)"), "only the value text changes");
        T.Eq(text.Length - "1 * 1000".Length + "5 * 60000".Length - "\"escalating\"".Length + "\"flat\"".Length - "1.5 * 60000".Length + "90000".Length, patched.Length, "nothing else changes");

        results.Clear();
        LuaPatcher.Apply(text, new[] { E("/conquestconfig.startspawntime/attackmin", "2 * 1000", "3") }, results);
        T.Eq(EditStatus.SourceChanged, results[0].Status, "a changed source value");
        results.Clear();
        LuaPatcher.Apply(text, new[] { E("/conquestconfig.nothing", "1", "2") }, results);
        T.Eq(EditStatus.TargetMissing, results[0].Status, "a missing slot");
        results.Clear();
        LuaPatcher.Apply(text, new[] { E("/conquestconfig.attackerwaves/bands/#2/max", "nil", "4") }, results);
        T.Eq(EditStatus.Invalid, results[0].Status, "nil stays");
        results.Clear();
        LuaPatcher.Apply("X = function() end", new[] { E("/x", "1", "2") }, results);
        T.Eq(EditStatus.Invalid, results[0].Status, "an unreadable file reports Invalid instead of writing");

        T.True(ValueRules.CheckNewValue(Lua, "1 * 1000", "2 * x", out warning) != null, "ValueRules uses the Lua rules for .lua");

        // 빌드: .lua 수정이 resource/script 아래에 쓰인다
        string game = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), (Lua, text));
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", Path.Combine(game, "resource")) });
        stack.Scan();
        var project = new EditProject { Name = "t", Edits = new List<Edit> { E("/conquestconfig.startspawntime/attackmin", "1 * 1000", "5 * 60000"), E("/conquestconfig.attackerwaves/flag", "true", "\"x\"") } };
        var report = new ModBuilder(stack, outDir).Build(project);
        T.Eq(1, report.Count(EditStatus.Applied), "the build applies the valid Lua edit");
        T.Eq(1, report.Count(EditStatus.Invalid), "and reports the wrong-kind edit");
        string written = File.ReadAllText(Path.Combine(outDir, "resource", "script", "multiplayer", "modes", "conquest", "runtime_config.lua"));
        T.True(written.Contains("AttackMin = 5 * 60000,") && written.Contains("Flag = true,"), "the written file");
    }
}
