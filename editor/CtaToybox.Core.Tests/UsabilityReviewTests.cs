using System;
using System.IO;
using System.Linq;
using CtaToybox.Core;

// 사용성 수정의 리뷰 지적(Important)을 재현하는 테스트
static class UsabilityReviewTests
{
    public static void Run()
    {
        // I-1: 이름공간은 파일 종류로 고른다. 탄창에 같은 이름의 총 이름이 붙지 않는다. 틀·설정 파일에는 이름을 붙이지 않는다
        var names = new DisplayNames();
        names.AddPot("msgctxt \"desc/weapon/chauchat\"\nmsgid \"Chauchat M/15\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"desc/ammo/chauchat\"\nmsgid \"8mm Chauchat Magazine\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"desc/ammo/pistol\"\nmsgid \"9mm Parabellum\"\nmsgstr \"\"\n");
        T.Eq("Chauchat M/15", names.OfUnit("set/stuff/mgun/chauchat"), "I-1: the gun file gets the weapon name");
        T.Eq("8mm Chauchat Magazine", names.OfUnit("set/stuff/mgun/chauchat.ammo"), "I-1: the .ammo file gets the ammo name");
        T.True(names.OfUnit("set/stuff/pistol/pistol.pattern") == null, "I-1: a pattern gets no name");
        T.True(names.OfUnit("set/stuff/_settings/pistol.fsm") == null, "I-1: a settings file gets no name");

        // I-6: 문맥은 정해 둔 부위 블록(volume, component, place 등)의 이름만. 종류 문자열(chassis "wheel")은 아니다.
        //      인자 없는 형제 블록은 앞 형제의 이름을 물려받지 않는다
        var root = SdlCore.Parse("{e {volume \"body1\" {thickness 45}} {chassis \"wheel\" {maxspeed 40}}" +
                                 " {place \"turret\" {weapon \"a\"}} {place {weapon \"b\"}} {extender \"inventory\" {box {item \"can\" 1}}}}").Root;
        var rows = KeyLocator.Rows(root);
        var contexts = UnitTracer.ContextsOf(rows);
        string Ctx(string path, int occ) { return contexts[rows.FindIndex(r => r.Path == path && r.Occurrence == occ)]; }
        T.Eq("volume body1", Ctx("/e/volume/thickness", 0), "I-6: a volume name is a context");
        T.True(Ctx("/e/chassis/maxspeed", 0) == null, "I-6: a chassis type string is not a context");
        T.Eq("place turret", Ctx("/e/place/weapon", 0), "I-6: a place name is a context");
        T.True(Ctx("/e/place/weapon", 1) == null, "I-6: an unnamed sibling does not inherit the previous sibling's name");
        T.True(Ctx("/e/extender[inventory]/box/item", 0) == null, "I-6: an extender name is not a context");
        var top = KeyLocator.Rows(SdlCore.Parse("{from \"pattern smg\" {calibre 1}}").Root);
        T.True(UnitTracer.ContextsOf(top)[1] == null, "I-6: the top-level block's name is not a context");

        // 세션 고정물: 유닛 a, b가 같은 include s.inc를 쓴다. s.inc에 매크로 정의와 그 안의 호출, 매개변수 인자 호출이 있다
        string game = T.TempDir(), work = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity (include \"s.inc\" thick(30)) {mass 10}}"),
            ("-vehicle/x/b/b.def", "{game_entity (include \"../a/s.inc\" thick(20))}"),
            ("-vehicle/x/a/s.inc", "(define \"lvl\" (\"vet\" 5)) (\"part\" \"body\" %thick) {box {item \"mine\" 24}}"));
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = T.TempDir(), OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        using var session = EditorSession.Open(settings, T.TempDir(), null);

        // I-5: 매크로 정의 안의 호출은 보이지 않는다. % 매개변수 인자는 고칠 수 없다
        var a = session.OpenUnit("entity/-vehicle/x/a/a.def");
        T.True(!a.Rows.Any(r => r.IsMacroCall && r.Path.Contains("(define")), "I-5: calls inside a macro definition are not shown");
        var part = a.Rows.Single(r => r.IsMacroCall && r.Path.EndsWith("(\"part\")"));
        T.Eq(true, part.Values[0].Editable, "I-5: a literal argument is editable");
        T.Eq(false, part.Values[1].Editable, "I-5: a %parameter argument is not editable");

        // I-2: b에서 '이 유닛만'으로 고치거나 되돌려도 a에서 만든 '전부' 수정은 남는다
        session.OpenProject(Path.Combine(work, "p.json"));
        var aRow = a.Rows.Single(r => r.Path == "/game_entity/box/item");
        session.ApplyRow(a, aRow, new[] { "\"mine\"", "30" }, "shared");
        var shared = session.Project.Edits.Single();
        var b = session.OpenUnit("entity/-vehicle/x/b/b.def");
        var bRow = b.Rows.Single(r => r.Path == "/game_entity/box/item");
        session.ApplyRow(b, bRow, new[] { "\"mine\"", "5" }, "unit");
        T.Eq(2, session.Project.Edits.Count, "I-2: a unit edit is added beside the shared one");
        T.True(session.Project.Edits.Contains(shared) && shared.New == "30", "I-2: the shared edit made from another unit survives");
        b = session.OpenUnit("entity/-vehicle/x/b/b.def");
        bRow = b.Rows.Single(r => r.Path == "/game_entity/box/item");
        session.ApplyRow(b, bRow, new[] { "\"mine\"", "24" }, "unit");
        T.Eq(1, session.Project.Edits.Count, "I-2: reverting in unit scope removes only this unit's edit");
        T.True(session.Project.Edits.Single() == shared, "I-2: the shared edit is still there");

        // 이 유닛만 → 전부로 바꾸면 이 유닛의 '이 유닛만' 수정은 지운다(남으면 이 유닛에서 '전부' 값이 안 보인다)
        session.ApplyRow(b, bRow, new[] { "\"mine\"", "7" }, "unit");
        b = session.OpenUnit("entity/-vehicle/x/b/b.def");
        bRow = b.Rows.Single(r => r.Path == "/game_entity/box/item");
        session.ApplyRow(b, bRow, new[] { "\"mine\"", "40" }, "shared");
        T.Eq(1, session.Project.Edits.Count, "switching to shared drops this unit's own unit edit and updates the shared one");
        T.Eq("40", session.Project.Edits.Single().New, "the shared edit has the new value");

        T.Throws<ArgumentException>(() => session.ApplyRow(b, bRow, new[] { "\"mine\"", "1 2" }, "shared"), "an invalid value refuses the whole row");
        T.Eq("40", session.Project.Edits.Single().New, "and changes nothing");

        // I-3: 저장이 실패하면 그 줄의 변경은 모두 되돌린다(한 번에 저장)
        string projectPath = session.ProjectPath;
        File.SetAttributes(projectPath, FileAttributes.ReadOnly);
        try
        {
            T.Throws<UnauthorizedAccessException>(() => session.ApplyRow(b, bRow, new[] { "\"mine\"", "9" }, "unit"), "I-3: a failed save is reported");
            T.Eq(1, session.Project.Edits.Count, "I-3: nothing was added");
            T.Eq("40", session.Project.Edits.Single().New, "I-3: nothing was changed");
        }
        finally { File.SetAttributes(projectPath, FileAttributes.Normal); }
    }
}
