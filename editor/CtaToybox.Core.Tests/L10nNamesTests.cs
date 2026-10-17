using System.Linq;
using CtaToybox.Core;

// 한글화 Task 8: 편집기 이름 "원문 (한글)"
static class L10nNamesTests
{
    public static void Run()
    {
        var n = new DisplayNames();
        n.AddPot("msgctxt \"desc/entity/kv85\"\nmsgid \"KV-85\"\nmsgstr \"\"\n\nmsgctxt \"desc/human/ger/rifleman\"\nmsgid \"Rifleman\"\nmsgstr \"\"\n\nmsgctxt \"desc/entity/t34\"\nmsgid \"T-34\"\nmsgstr \"\"\n");
        string dir = T.TempDir();
        T.PutUtf8(dir, "interface/text/desc/a.po", "msgctxt \"desc/human/ger/rifleman\"\nmsgid \"Rifleman\"\nmsgstr \"소총수\"\n\nmsgctxt \"desc/entity/kv85\"\nmsgid \"KV-85\"\nmsgstr \"KV-85\"\n\n#, fuzzy\nmsgctxt \"desc/entity/t34\"\nmsgid \"T-34\"\nmsgstr \"티\"\n");
        n.AddTranslations(dir);
        T.Eq("Rifleman (소총수)", n.OfUnit("set/breed/ger/rifleman.set"), "source (korean)");
        T.Eq("KV-85", n.OfUnit("entity/vehicle/tank/kv85.def"), "same text shows once");
        T.Eq("KV-85 (kv85_x)", n.OfUnit("entity/vehicle/tank/kv85_x.def"), "variant tail after names");
        T.Eq("T-34", n.OfUnit("entity/vehicle/tank/t34.def"), "fuzzy translations are not shown");
        var index = new UnitIndex();
        index.Units.Add(new UnitEntry { Path = "set/breed/ger/rifleman.set", Kind = "breed", EditorKind = "breed", Layer = "vanilla", Numbers = 1 });
        T.Eq(1, index.Search("소총", null, null, n).Count(), "search finds the korean name");
        T.Eq(1, index.Search("Rifle", null, null, n).Count(), "and the source name");
    }
}
