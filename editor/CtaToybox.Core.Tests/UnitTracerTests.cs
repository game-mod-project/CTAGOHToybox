using System.IO;
using System.Linq;
using CtaToybox.Core;

static class UnitTracerTests
{
    static TraceValue Value(UnitTraceResult t, string path, int occurrence, int index)
    {
        return t.Rows.First(r => r.Path == path && r.Occurrence == occurrence).Values[index];
    }

    public static void Run()
    {
        string vanilla = T.TempDir();
        T.Put(vanilla, "entity/v/.presets", "(define \"mob\" {speed %speed} {mass 10})");
        T.Put(vanilla, "entity/v/a/a.def", "{game_entity\n (include \"a.inc\" scale(1.5))\n (\"mob\" speed(40))\n {half (/ 10 4)}\n {volume \"body\" {thickness 45}}\n {volume \"turret\" {thickness 100}}\n}");
        T.Put(vanilla, "entity/v/a/a.inc", "{volume \"hull\" {thickness 20}}\n{box {item \"mine\" 24}}\n{size %scale}\n(include \"bad.inc\")");
        T.Put(vanilla, "entity/v/a/bad.inc", "{broken 1");

        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var tracer = new UnitTracer(stack);
        T.True(tracer.Trace("entity/v/a/none.def") == null, "a missing unit is null");

        var t = tracer.Trace("ENTITY/v/a/a.def");
        T.Eq("entity/v/a/a.def", t.Unit, "the unit path is normalised");
        T.Eq(2, t.IncludeEdges.Count, "include edges are passed through");
        T.True(t.Unresolved.Any(u => u.StartsWith("include-parse-error:")), "unresolved notes are passed through");

        var mine = Value(t, "/game_entity/box/item", 0, 1);
        T.Eq("24", mine.Text, "the expanded value");
        T.Eq("entity/v/a/a.inc", mine.File, "its file");
        T.Eq("/box/item", mine.KeyPath, "its key path inside that file, not the expanded path");
        T.Eq(0, mine.Occurrence, "its occurrence inside that file");
        T.Eq(1, mine.Arg, "its value index inside that row");

        // 펼친 뒤의 순번과 파일 안의 순번은 다르다: 유닛의 volume은 펼친 트리에서 2번째, 3번째지만 파일에서는 0번째, 1번째다
        var hull = Value(t, "/game_entity/volume/thickness", 0, 0);
        T.Eq("entity/v/a/a.inc", hull.File, "the first expanded thickness comes from the include");
        var turret = Value(t, "/game_entity/volume/thickness", 2, 0);
        T.Eq("100", turret.Text, "the third expanded thickness");
        T.Eq("entity/v/a/a.def", turret.File, "comes from the unit file");
        T.Eq(1, turret.Occurrence, "where it is the second thickness");
        T.Eq("/game_entity/volume/thickness", turret.KeyPath, "with the unit file's own key path");

        var speed = Value(t, "/game_entity/speed", 0, 0);
        T.Eq("entity/v/a/a.def", speed.File, "a macro argument is located in the calling file");
        T.Eq("/game_entity/(\"mob\")", speed.KeyPath, "at the macro call row");
        T.Eq(0, speed.Arg, "as that row's first value");

        var mass = Value(t, "/game_entity/mass", 0, 0);
        T.Eq("entity/v/.presets", mass.File, "a macro body value is located in the defining file");
        T.Eq("/(define \"mob\")/mass", mass.KeyPath, "inside the define");

        var size = Value(t, "/game_entity/size", 0, 0);
        T.Eq(-1, size.Arg, "an include argument is not an editable slot");
        T.True(size.Note != null, "and says why");
        var half = Value(t, "/game_entity/half", 0, 0);
        T.Eq(true, half.Synthetic, "a computed value is flagged");
        T.Eq(-1, half.Arg, "and is not an editable slot");
        var broken = Value(t, "/game_entity/broken", 0, 0);
        T.Eq(-1, broken.Arg, "a value from a file that does not parse is not editable");

        // 추적 결과로 만든 수정이 그 자리를 고친다
        var results = new System.Collections.Generic.List<EditResult>();
        var edit = new Edit { Unit = t.Unit, File = turret.File, Path = turret.KeyPath, Occurrence = turret.Occurrence, Arg = turret.Arg, Old = turret.Text, New = "110" };
        string patched = TokenPatcher.Apply(stack.ReadText(turret.File), new[] { edit }, results);
        T.True(patched.Contains("{volume \"turret\" {thickness 110}}") && patched.Contains("{thickness 45}"), "an edit built from a trace value changes exactly that token");
    }
}
