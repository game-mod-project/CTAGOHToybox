using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

static class SourceStackTests
{
    static string Text(SourceStack s, string vp) { return Encoding.Latin1.GetString(s.Read(s.Find(vp))); }

    public static void Run()
    {
        string vanilla = T.TempDir(), modA = T.TempDir(), modB = T.TempDir();
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/a.set", "vanilla-a"), ("set/b.set", "vanilla-b"), ("set/dir/", ""));
        T.Zip(Path.Combine(vanilla, "entity", "-vehicle.pak"), ("ENG/Tank/T.def", "vanilla-t"));
        T.Zip(Path.Combine(modA, "a.pak"), ("set/a.set", "modA-a-pak"), ("set/c.set", "modA-c-pak"));
        T.Put(modA, "set/c.set", "modA-c-loose");
        T.Put(modB, "SET/A.SET", "modB-a-loose");

        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("modA", modA), new SourceLayer("modB", modB) });
        stack.Scan();

        T.Eq("vanilla-b", Text(stack, "set/b.set"), "a file only in vanilla comes from vanilla");
        T.Eq("vanilla-t", Text(stack, "entity/eng/tank/t.def"), "a pak in a subfolder gets the folder as prefix, lower-cased");
        T.Eq("modB-a-loose", Text(stack, "set/a.set"), "the last layer wins, case-insensitively");
        T.Eq("modA-c-loose", Text(stack, "set/c.set"), "inside one layer a loose file wins over a pak entry");
        T.Eq(new[] { "vanilla", "modA", "modB" }, stack.Versions("set/a.set").Select(v => v.Layer).ToArray(), "versions are listed in load order");
        T.Eq(2, stack.Versions("set/c.set").Count, "pak and loose versions of one layer are both listed");
        T.Eq("modB-a-loose", Text(stack, "SET\\A.set"), "lookup accepts backslashes and any case");
        T.True(stack.Find("set/missing.set") == null, "a missing path returns null");
        T.True(stack.Find("set/dir") == null && stack.Find("set/dir/") == null, "directory entries are not files");
        T.Eq(4, stack.Paths.Count(), "distinct virtual paths");
        T.Eq("set/a.set", SourceStack.ToVirtual("\\Set\\A.set"), "ToVirtual normalises");

        // 없는 계층 폴더는 빈 계층이다
        using var empty = new SourceStack(new[] { new SourceLayer("gone", Path.Combine(vanilla, "nope")) });
        empty.Scan();
        T.Eq(0, empty.Paths.Count(), "a missing resource folder is an empty layer");
    }
}
