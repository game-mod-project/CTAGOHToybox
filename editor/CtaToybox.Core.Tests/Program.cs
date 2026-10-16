// 테스트 진입점. tools/tests/TestKit.ps1과 같은 방식의 자체 단언(T)이다. 같은 묶음을 xUnit(editor/CtaToybox.Core.XunitTests)으로도 돌린다(P7)
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

static class T
{
    public static int Pass, Fail;
    static readonly Encoding Latin1 = Encoding.Latin1;
    static readonly JsonSerializerOptions Json = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static void Eq<TValue>(TValue expected, TValue actual, string name)
    {
        string e = JsonSerializer.Serialize(expected, Json), a = JsonSerializer.Serialize(actual, Json);
        if (e == a) { Pass++; return; }
        Fail++;
        Console.WriteLine("FAIL " + name);
        Console.WriteLine("  expected: " + e);
        Console.WriteLine("  actual:   " + a);
    }

    public static void True(bool condition, string name) { Eq(true, condition, name); }

    public static void Throws<TException>(Action action, string name) where TException : Exception
    {
        try { action(); }
        catch (TException) { Pass++; return; }
        catch (Exception ex) { Fail++; Console.WriteLine("FAIL " + name + ": wrong exception " + ex.GetType().Name); return; }
        Fail++; Console.WriteLine("FAIL " + name + ": no exception");
    }

    // 저장소 루트(data/annotations가 있는 폴더). 실행 폴더에서 위로 찾는다. dotnet run(저장소 루트)과 dotnet test(출력 폴더) 모두에서 같다
    public static string RepoRoot()
    {
        for (var d = new DirectoryInfo(AppContext.BaseDirectory); d != null; d = d.Parent)
            if (Directory.Exists(Path.Combine(d.FullName, "data", "annotations"))) return d.FullName;
        return Directory.GetCurrentDirectory();
    }

    public static string TempDir()
    {
        string d = Path.Combine(Path.GetTempPath(), "ctatoybox-core-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(d);
        return d;
    }

    public static void Put(string root, string relativePath, string text)
    {
        string p = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(p));
        File.WriteAllBytes(p, Latin1.GetBytes(text));
    }

    public static void Zip(string pakPath, params (string Name, string Text)[] entries)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(pakPath));
        using var zip = ZipFile.Open(pakPath, ZipArchiveMode.Create);
        foreach (var e in entries)
        {
            using var s = zip.CreateEntry(e.Name).Open();
            byte[] b = Latin1.GetBytes(e.Text);
            s.Write(b, 0, b.Length);
        }
    }
}

static class Suites
{
    // 테스트 묶음 목록. 자체 진입점(Main)과 xUnit(CtaToybox.Core.XunitTests)이 같이 쓴다(P7)
    public static readonly List<(string Name, Action Run)> All = new List<(string Name, Action Run)>
    {
        ("TextCodec", TextCodecTests.Run),
        ("SourceStack", SourceStackTests.Run),
        ("GameProfile", GameProfileTests.Run),
        ("KeyLocator", KeyLocatorTests.Run),
        ("ResolverOrigin", ResolverOriginTests.Run),
        ("FromLinks", FromLinksTests.Run),
        ("FromInheritance", FromInheritanceTests.Run),
        ("FromDisplay", FromDisplayTests.Run),
        ("InheritedUnitScope", InheritedUnitScopeTests.Run),
        ("UnitIndex", UnitIndexTests.Run),
        ("IncludeChain", IncludeChainTests.Run),
        ("UnitTracer", UnitTracerTests.Run),
        ("KeyAnnotations", KeyAnnotationsTests.Run),
        ("AnnotationScope", AnnotationScopeTests.Run),
        ("ItemLinks", ItemLinksTests.Run),
        ("SummaryKeys", SummaryKeysTests.Run),
        ("EditorSession", EditorSessionTests.Run),
        ("EditorSessionEdit", EditorSessionEditTests.Run),
        ("CheckEdits", CheckEditsTests.Run),
        ("ProjectMerge", ProjectMergeTests.Run),
        ("MacroSwap", MacroSwapTests.Run),
        ("BlockCopy", BlockCopyTests.Run),
        ("BlockCopyBuild", BlockCopyBuildTests.Run),
        ("BlockCopySession", BlockCopySessionTests.Run),
        ("ReviewFix", ReviewFixTests.Run),
        ("Usability", UsabilityTests.Run),
        ("UsabilityReview", UsabilityReviewTests.Run),
        ("TokenPatcher", TokenPatcherTests.Run),
        ("ValueRules", ValueRulesTests.Run),
        ("ModBuilder", ModBuilderTests.Run),
        ("UnitScopeBuild", UnitScopeBuildTests.Run),
        ("UnitFilter", UnitFilterTests.Run),
        ("Roster", RosterTests.Run),
        ("RulesSession", RulesSessionTests.Run),
        ("ValueLabels", ValueLabelsTests.Run),
        ("Robustness", RobustnessTests.Run),
        ("SettingsPolish", SettingsPolishTests.Run),
        ("SettingFiles", SettingFilesTests.Run),
        ("SettingsSession", SettingsSessionTests.Run),
        ("VisionSettings", VisionSettingsTests.Run),
        ("VisionPresets", VisionPresetsTests.Run),
        ("ClassVision", ClassVisionTests.Run),
        ("BulkRules", BulkRulesTests.Run),
        ("BulkRulesSession", BulkRulesSessionTests.Run),
        ("SettingStrings", SettingStringsTests.Run),
        ("LuaTable", LuaTableTests.Run),
        ("LuaPatcher", LuaPatcherTests.Run),
        ("LuaSession", LuaSessionTests.Run),
        ("Cli", CliTests.Run),
    };
}

static class Program
{
    static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "real")
        {
            try { RealFilesCheck.Run(args.Length > 1 ? args[1] : System.IO.Path.Combine("editor", "settings.json")); }
            catch (Exception ex) { T.Fail++; Console.WriteLine("FAIL real files threw: " + ex); }
            Console.WriteLine("passed=" + T.Pass + " failed=" + T.Fail);
            return T.Fail > 0 ? 1 : 0;
        }
        var suites = Suites.All;
        foreach (var s in suites)
        {
            int before = T.Fail;
            try { s.Run(); }
            catch (Exception ex) { T.Fail++; Console.WriteLine("FAIL " + s.Name + " threw: " + ex); }
            Console.WriteLine("== " + s.Name + (T.Fail == before ? " ok" : " FAILED"));
        }
        Console.WriteLine("passed=" + T.Pass + " failed=" + T.Fail);
        return T.Fail > 0 ? 1 : 0;
    }
}
