using System.IO;
using System.Linq;
using CtaToybox.Core;

// E15: 원형 상속 표시의 작은 어긋남(E11 최종 리뷰)
static class FromDisplayTests
{
    const string Base = "set/stuff/e15/base.pattern", Inc = "set/stuff/e15/e15inc.inc", Gun = "set/stuff/e15/gun", Outside = "set/breed/mp/e15/x.set";

    static string Fixture(out string repo)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        repo = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/stuff/e15/.presets", "(define \"rof\" {fire_rate %rpm} {burst 3})\n(define \"rof2\" {cycle %rpm})"),
            (Base, "{bullet\n {\"head\" 1.3}\n {\"body\" 1.0}\n {speed 800}\n {range 300}\n (\"rof\" rpm(600))\n (include \"e15inc.inc\")\n}"),
            (Inc, "{weight 5}\n(\"rof2\" rpm(9))"),
            (Gun, "{From \"pattern base\"\n {\"head\" 2.0}\n (mod \"mp\" {speed 900})\n {fire_rate 700}\n {burst 4}\n}"),
            (Outside, "{from \"pattern base\" {mass 1}}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        // Reroot: 맨 위 매크로 이름 안의 / 는 가르지 않는다
        T.Eq("/from/x", FromLinks.Reroot("/(\"a/b\")/x", "/from"), "a slash inside the top macro name is not a separator");
        T.True(FromLinks.Reroot("/(\"a/b\")", "/from") == null, "the top row itself is still null");
        T.Eq("/from/mass", FromLinks.Reroot("/bullet/mass", "/from"), "an ordinary path");

        // None은 공유 목록이 아니다
        var links = new FromLinks(new[] { "set/stuff/a/a" });
        var none1 = links.Candidates("set/stuff/a/x", "nothing");
        none1.Add("x");
        T.Eq(0, links.Candidates("set/stuff/a/y", "nothing").Count, "an empty result is not a shared list");

        string repo;
        string settings = Fixture(out repo);
        using var session = EditorSession.Open(settings, repo, null);
        var t = new UnitTracer(session.Stack).Trace(Gun);
        T.Eq(new[] { Base }, t.From.Select(l => l.File).ToArray(), "a capitalised {From} still inherits");

        // 따옴표 이름 블록은 이름으로 가른다
        var head = t.Rows.Single(r => r.From == Base && r.RawPath != null && r.RawPath.EndsWith("\"head\""));
        var body = t.Rows.Single(r => r.From == Base && r.RawPath != null && r.RawPath.EndsWith("\"body\""));
        T.Eq(FromLevel.Overridden, head.FromState, "a quoted block the child writes is overridden");
        T.Eq(FromLevel.Inherited, body.FromState, "another quoted name is inherited");

        // (mod …) 안에서만 자식이 적은 키는 가려짐이 아니다
        var speed = t.Rows.Single(r => r.From == Base && r.Path == "/from/speed");
        T.Eq(FromLevel.Conditional, speed.FromState, "a key the child writes only inside (mod ...) is conditional");
        T.True(FromLevel.LabelOf(Base, FromLevel.Conditional, null).Contains("조건"), "the conditional label says so");
        T.Eq(FromLevel.Inherited, t.Rows.Single(r => r.From == Base && r.Path == "/from/range").FromState, "an untouched key is inherited");

        var view = session.OpenUnit(Gun);
        // 원형의 매크로 호출 줄: 펼친 키(fire_rate, burst)를 자식이 모두 적으면 가려짐
        var calls = view.Rows.Where(r => r.IsMacroCall && r.From == Base && r.Path == "/from/(\"rof\")").ToList();
        T.Eq(1, calls.Count, "the call is shown");
        T.True(calls.All(c => c.FromState == FromLevel.Overridden), "a call whose expanded keys the child all writes is overridden");
        T.True(calls.All(c => c.RawPath.StartsWith("/From/")), "the call row's raw path uses the child's own top block spelling");
        // 원형이 include한 파일의 호출 줄은 펼친 자리의 경로로 보인다
        var inc = view.Rows.Single(r => r.IsMacroCall && r.From == Base && r.Values.Any(v => v.Trace.File == Inc));
        T.Eq("/from/(\"rof2\")", inc.Path, "an include's call row sits under the pattern's top block");
        T.Eq(FromLevel.Inherited, inc.FromState, "and is inherited");

        // set/stuff 밖 유닛의 {from}은 원형 상속으로 펼치지 않고, 풀리지 않은 구문으로도 알리지 않는다
        var outside = new UnitTracer(session.Stack).Trace(Outside);
        T.Eq(0, outside.From.Count, "no chain outside set/stuff");
        T.True(!outside.Unresolved.Any(u => u.StartsWith("from-")), "and no from-not-found");
    }
}
