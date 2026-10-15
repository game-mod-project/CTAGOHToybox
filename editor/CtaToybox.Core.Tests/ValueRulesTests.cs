using CtaToybox.Core;

// 새 값의 범위 경고(E10): 막지 않고 경고만 한다
static class ValueRulesTests
{
    static string Warn(string file, string oldText, string newText)
    {
        string warning;
        string error = ValueRules.CheckNewValue(file, oldText, newText, out warning);
        T.True(error == null, "a range problem is never an error: " + oldText + " -> " + newText);
        return warning;
    }

    public static void Run()
    {
        const string Ammo = "set/stuff/smg/mp40.ammo", Lua = "set/dynamic_campaign/ai/x.lua";

        string w = Warn(Ammo, "3.1", "-1");
        T.True(w != null && w.Contains("음수"), "a positive value made negative is warned");
        T.True(Warn(Ammo, "192", "1.5") == null, "an integer may become fractional: counts like {item \"m24 grenade\" 1.25 0.5} are normal");
        T.True(Warn(Ammo, "-2", "-5") == null, "a negative value may stay negative");
        T.True(Warn(Ammo, "0", "-1") == null, "zero may become negative (offsets)");
        T.True(Warn(Ammo, "3.1", "10") == null, "a bigger positive value is fine");
        T.True(Warn(Ammo, "192", "100") == null, "an integer for an integer is fine");
        T.True(Warn(Ammo, "1.0", "2.5") == null, "a fraction for a fraction is fine");
        T.True(Warn(Ammo, "2", "60") == null, "a large factor is not warned (gravity 2 -> 60 was real)");
        w = Warn(Ammo, "5", "abc");
        T.True(w != null && w.Contains("숫자가 아니다"), "the number-to-word warning stays");

        w = Warn(Lua, "3", "-1");
        T.True(w != null && w.Contains("음수"), "Lua numbers are checked the same way");
        T.True(Warn(Lua, "60", "0.5") == null, "a Lua integer may become fractional");
        T.True(Warn(Lua, "1 * 1000", "-5") == null, "an expression is not range-checked");
    }
}
