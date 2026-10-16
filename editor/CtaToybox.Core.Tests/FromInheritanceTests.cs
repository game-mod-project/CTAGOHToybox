using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

// E11: {from} 원형 상속을 펼친다. k98 → rifle.pattern → bullet.pattern
static class FromInheritanceTests
{
    const string K98 = "set/stuff/rifle/k98", Rifle = "set/stuff/rifle/rifle.pattern", Bullet = "set/stuff/bullet.pattern";

    static string Fixture(out string repo)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        repo = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            (Bullet, "{bullet\n {mass 5}\n {inventory {size 1 1} {weight 10}}\n {tag \"base\"}\n {speed 800}\n {cal 9}\n {hit 1}\n {hit 2}\n {parameters \"ap\" {pen 90}}\n {parameters \"he\" {pen 10}}\n}"),
            ("set/stuff/rifle/.presets", "(define \"rof\" {fire_rate %rpm})"),
            (Rifle, "{from \"pattern bullet\"\n {mass 4}\n {cal 8}\n {range 300}\n (\"rof\" rpm(600))\n}"),
            (K98, "{from \"pattern rifle\"\n {range 500}\n {inventory {size 4 2}}\n {filling \"ammo x\" 5}\n {tag \"a\"}\n {tag \"b\"}\n {hit 3}\n {parameters \"ap\" {pen 95}}\n {cal 7}\n}"),
            ("set/stuff/odd/loop_a", "{from \"loop_b\" {x 1}}"),
            ("set/stuff/odd/loop_b", "{from \"loop_a\" {y 2}}"),
            ("set/stuff/odd/lost", "{from \"pattern nothing\" {z 3}}"),
            ("set/stuff/other/bullet.pattern", "{bullet {mass 7}}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        File.WriteAllText(Path.Combine(repo, "data", "annotations", "common.psv"), "**/mass|block|확인|무게. 킬로그램|C\n");
        Directory.CreateDirectory(Path.Combine(repo, "editor", "summary"));
        File.WriteAllText(Path.Combine(repo, "editor", "summary", "stuff.psv"), "/from/mass|무게\n/from/range|사거리\n/from/inventory/weight|들 때 무게\n");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        string repo;
        string settings = Fixture(out repo);
        using var session = EditorSession.Open(settings, repo, null);

        // 추적: 원형 파일의 줄이 맨 위 블록을 자식 것으로 바꿔 붙는다
        var t = new UnitTracer(session.Stack).Trace(K98);
        T.Eq(new[] { Rifle, Bullet }, t.From.Select(l => l.File).ToArray(), "the chain, nearest first");
        T.Eq(new[] { "pattern rifle", "pattern bullet" }, t.From.Select(l => l.Name).ToArray(), "with the names that pointed to them");
        var own = t.Rows.Single(r => r.Path == "/from/range" && r.From == null);
        T.Eq("500", own.Values[0].Text, "the child's own row has no From");
        var range = t.Rows.Single(r => r.Path == "/from/range" && r.From == Rifle);
        T.Eq("300", range.Values[0].Text, "the parent's row under the child's top block");
        T.Eq(FromLevel.Overridden, range.FromState, "a key the child also writes is overridden (G16)");
        T.Eq(Rifle, range.Values[0].File, "the value is located in the parent file");
        T.Eq("/from/range", range.Values[0].KeyPath, "at the parent's own key path");
        var rifleMass = t.Rows.Single(r => r.Path == "/from/mass" && r.From == Rifle);
        T.Eq(FromLevel.Inherited, rifleMass.FromState, "a key the child does not write is inherited");
        var bulletMass = t.Rows.Single(r => r.Path == "/from/mass" && r.From == Bullet);
        T.Eq(FromLevel.Overridden, bulletMass.FromState, "a nearer parent also overrides a farther one");
        T.Eq("/bullet/mass", bulletMass.Values[0].KeyPath, "the far parent's value keeps its own key path");
        var weight = t.Rows.Single(r => r.Path == "/from/inventory/weight");
        T.Eq(FromLevel.Unconfirmed, weight.FromState, "a key whose block the child also writes is unconfirmed");
        T.Eq("inventory", weight.FromBlock, "and names that block");
        T.Eq(FromLevel.Overridden, t.Rows.Single(r => r.Path == "/from/inventory/size" && r.From == Bullet).FromState, "a nested key the child writes is overridden");
        T.Eq(FromLevel.Inherited, t.Rows.Single(r => r.Path == "/from/speed").FromState, "a key nobody nearer writes is inherited");
        T.Eq(FromLevel.Overridden, t.Rows.Single(r => r.Path == "/from/cal" && r.From == Bullet).FromState, "a key the unit and a nearer parent each write once is overridden, not repeated");
        var tag = t.Rows.Single(r => r.Path == "/from/tag" && r.From == Bullet);
        T.Eq(FromLevel.Unconfirmed, tag.FromState, "a key the child writes more than once is unconfirmed (it may accumulate)");
        T.True(tag.FromBlock == null, "and names no block");
        T.True(t.Rows.Where(r => r.Path == "/from/hit" && r.From == Bullet).All(r => r.FromState == FromLevel.Unconfirmed), "a key the parent writes more than once is unconfirmed too");
        var pens = t.Rows.Where(r => r.Path == "/from/parameters/pen" && r.From == Bullet).ToList();
        T.Eq(new[] { "90", "10" }, pens.Select(r => r.Values[0].Text).ToArray(), "both of the parent's named parameter blocks");
        T.Eq(new[] { FromLevel.Overridden, FromLevel.Inherited }, pens.Select(r => r.FromState).ToArray(), "a named block is matched by its name: ap is overridden, he is inherited");
        var headers = t.Rows.Where(r => r.Path == "/from/parameters" && r.From == Bullet).ToList();
        T.Eq(new[] { FromLevel.Unconfirmed, FromLevel.Inherited }, headers.Select(r => r.FromState).ToArray(), "a named block's header is unconfirmed when the child has that block, else inherited");
        T.Eq("parameters\"ap\"", headers[0].FromBlock, "and names the block");
        T.True(FromLevel.LabelOf(Bullet, FromLevel.Unconfirmed, "parameters\"ap\"").Contains("parameters \"ap\" 블록"), "the label spaces the block name");
        var rof = t.Rows.Single(r => r.Path == "/from/fire_rate");
        T.Eq(Rifle, rof.From, "a parent's macro expansion is inherited too");
        T.Eq("/from/(\"rof\")", rof.Values[0].KeyPath, "located at the parent's macro call");
        T.True(!t.Rows.Any(r => r.From != null && r.Path == "/from"), "the parent's own top row is not a value");
        T.True(t.Unresolved.Count == 0, "nothing unresolved for a complete chain");
        T.Eq(1, t.From[0].Candidates.Count, "a unique name has one candidate");
        T.Eq(new[] { Bullet, "set/stuff/other/bullet.pattern" }, t.From[1].Candidates.ToArray(), "a name two files match keeps both candidates");
        string chain = FromLevel.ChainLine(t.From);
        T.True(chain.StartsWith("원형 사슬: rifle.pattern → bullet.pattern"), "the chain line: " + chain);
        T.True(chain.Contains("pattern bullet") && chain.Contains("2개") && chain.Contains("미확인"), "names the ambiguous link");
        T.True(FromLevel.ChainLine(new System.Collections.Generic.List<FromLevel>()) == null, "no chain, no line");

        var lost = new UnitTracer(session.Stack).Trace("set/stuff/odd/lost");
        T.True(lost.Unresolved.Any(u => u.StartsWith("from-not-found: pattern nothing")), "a missing parent is reported");
        var loop = new UnitTracer(session.Stack).Trace("set/stuff/odd/loop_a");
        T.Eq(new[] { "set/stuff/odd/loop_b" }, loop.From.Select(l => l.File).ToArray(), "a cycle stops before repeating a file");
        T.True(loop.Unresolved.Any(u => u.StartsWith("from-cycle: loop_a")), "and is reported");
        T.Eq(0, new UnitTracer(session.Stack).Trace(Bullet).From.Count, "a file whose top block is not from has no chain");

        // 유닛 색인: 원형 파일을 쓰는 유닛에 물려받는 유닛이 든다
        T.Eq(new[] { Bullet, K98, Rifle }, session.Index.UsersOf(Bullet).ToArray(), "users of the far parent");
        T.Eq(new[] { K98, Rifle }, session.Index.UsersOf(Rifle).ToArray(), "users of the near parent");
        T.Eq(new[] { K98, Rifle }, session.Index.UsersOf("set/stuff/rifle/.presets").ToArray(), "a parent's .presets is read by its inheritors");
        T.Eq(7, session.Index.Units.Single(u => u.Path == K98).Numbers, "the editor list counts only the unit file's own numbers (500 4 2 5 3 95 7, not the inherited ones)");
        T.Eq(new[] { K98, Rifle }, session.Index.HeirsOf(Bullet).ToArray(), "the units that inherit a pattern");
        T.Eq(0, session.Index.HeirsOf(K98).Count, "a unit nobody inherits has no heirs");

        // 화면
        var view = session.OpenUnit(K98);
        T.Eq(chain, view.FromChain, "the unit view carries the chain line");
        var vRifleMass = view.Rows.Single(r => r.Path == "/from/mass" && r.From == Rifle);
        T.Eq("무게", vRifleMass.SummaryLabel, "an inherited row is in the summary");
        T.Eq(2, vRifleMass.Values[0].SharedBy, "shared by the pattern and its inheritor");
        T.True(vRifleMass.Values[0].UnitScopeBlocker != null && vRifleMass.Values[0].UnitScopeBlocker.Contains("원형"), "an inherited value cannot be unit-scoped");
        T.Eq("원형 rifle.pattern에서 물려받는다", vRifleMass.FromLabel, "the inherited label");
        var vBulletMass = view.Rows.Single(r => r.Path == "/from/mass" && r.From == Bullet);
        T.True(vBulletMass.SummaryLabel == null, "an overridden row is not in the summary");
        T.True(vBulletMass.FromLabel.Contains("가려짐"), "the overridden label");
        var vWeight = view.Rows.Single(r => r.Path == "/from/inventory/weight");
        T.Eq("들 때 무게", vWeight.SummaryLabel, "an unconfirmed row stays in the summary");
        T.True(vWeight.FromLabel.Contains("미확인") && vWeight.FromLabel.Contains("inventory"), "the unconfirmed label names the block");
        T.True(view.Rows.Single(r => r.Path == "/from/tag" && r.From == Bullet).FromLabel.Contains("여러 번"), "the repeated-key label says why");
        var rifleView = session.OpenUnit(Rifle);
        var rifleOwn = rifleView.Rows.Single(r => r.Path == "/from/mass" && r.From == null).Values[0];
        T.True(rifleOwn.UnitScopeBlocker != null && rifleOwn.UnitScopeBlocker.Contains("1개"), "a unit another unit inherits cannot be unit-scoped (its heirs would change too): " + rifleOwn.UnitScopeBlocker);
        var vOwn = view.Rows.Single(r => r.Path == "/from/range" && r.From == null);
        T.True(vOwn.FromLabel == null && vOwn.Values[0].UnitScopeBlocker == null, "the unit's own row is unchanged");
        var call = view.Rows.Single(r => r.IsMacroCall && r.From == Rifle);
        T.Eq("/from/(\"rof\")", call.Path, "a parent's macro call row is shown");
        T.Eq(FromLevel.Inherited, call.FromState, "with its state");
        T.True(call.Values[0].UnitScopeBlocker != null, "and is shared-only");

        // 고치기: 물려받은 값의 수정은 원형 파일의 자리에 걸린다
        session.OpenProject(Path.Combine(T.TempDir(), "p.json"));
        var added = session.AddEdit(view, vRifleMass.Values[0], "4.5", "shared");
        T.Eq(Rifle, added.File, "an inherited value's edit targets the parent file");
        T.Eq("/from/mass", added.Path, "at the parent's own key path");
        session.RefreshEdits(view);
        T.True(vRifleMass.Values[0].Edit != null, "and shows on the inherited row");
        T.Throws<System.ArgumentException>(() => session.AddEdit(view, vRifleMass.Values[0], "4.6", "unit"), "unit scope is refused");

        // 명령줄도 같다: 원형으로 쓰이는 유닛의 '이 유닛만'은 막는다
        var cliOut = new StringWriter();
        int code = CliApp.Run(new[] { "add", "--settings", settings, "--project", Path.Combine(T.TempDir(), "c.json"), "--file", Rifle, "--path", "/from/mass", "--new", "4.1", "--scope", "unit", "--unit", Rifle }, cliOut);
        T.Eq(1, code, "cli add --scope unit on an inherited unit is refused");
        T.True(cliOut.ToString().Contains("물려받는 유닛이 1개"), "with the reason: " + cliOut);
        cliOut = new StringWriter();
        CliApp.Run(new[] { "trace", Rifle, "--settings", settings, "--filter", "mass" }, cliOut);
        T.True(cliOut.ToString().Contains("이 유닛만: 불가(이 유닛을 원형으로 물려받는 유닛이 1개"), "cli trace shows it");
        // 같은 줄 모으기는 다른 파일 단계의 줄을 모으지 않는다
        var a = new RowView { Path = "/from/x", Form = "block" };
        a.Values.Add(new ValueView { Trace = new TraceValue { Text = "1", Arg = -1 } });
        var b = new RowView { Path = "/from/x", Form = "block", From = Rifle };
        b.Values.Add(new ValueView { Trace = new TraceValue { Text = "1", Arg = -1 } });
        var list = new System.Collections.Generic.List<RowView> { a, b };
        EditorSession.CollapseRepeats(list);
        T.Eq(2, list.Count, "rows from different chain levels are not collapsed");
    }
}
