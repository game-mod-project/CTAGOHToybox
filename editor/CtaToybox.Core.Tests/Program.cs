// 외부 테스트 프레임워크 없이 도는 테스트 진입점. tools/tests/TestKit.ps1과 같은 방식이다.
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
        var suites = new List<(string Name, Action Run)>
        {
            ("TextCodec", TextCodecTests.Run),
            ("SourceStack", SourceStackTests.Run),
            ("GameProfile", GameProfileTests.Run),
            ("KeyLocator", KeyLocatorTests.Run),
            ("ResolverOrigin", ResolverOriginTests.Run),
            ("UnitIndex", UnitIndexTests.Run),
            ("IncludeChain", IncludeChainTests.Run),
            ("UnitTracer", UnitTracerTests.Run),
            ("KeyAnnotations", KeyAnnotationsTests.Run),
            ("ItemLinks", ItemLinksTests.Run),
            ("SummaryKeys", SummaryKeysTests.Run),
            ("EditorSession", EditorSessionTests.Run),
            ("TokenPatcher", TokenPatcherTests.Run),
            ("ModBuilder", ModBuilderTests.Run),
            ("UnitScopeBuild", UnitScopeBuildTests.Run),
            ("Cli", CliTests.Run),
        };
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
