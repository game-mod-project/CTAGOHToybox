// 자체 테스트 묶음(Suites.All)을 묶음마다 xUnit 테스트 하나로 돌린다(P7).
// 묶음들은 정적 상태(T.Pass·T.Fail, CliApp.GameRunning 등)를 같이 쓰므로 차례로 돈다(xunit.runner.json).
// 실패하면 그 묶음이 콘솔에 쓴 FAIL 줄을 메시지로 보인다. 실제 게임 파일 검사(-- real)는 여기서 돌리지 않는다
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

public sealed class SuiteTests
{
    readonly ITestOutputHelper output;

    public SuiteTests(ITestOutputHelper output) { this.output = output; }

    public static IEnumerable<object[]> Names() { return Suites.All.Select(s => new object[] { s.Name }); }

    [Theory]
    [MemberData(nameof(Names))]
    public void Suite(string name)
    {
        var suite = Suites.All.Single(s => s.Name == name);
        var console = Console.Out;
        var captured = new StringWriter();
        int passBefore = T.Pass, failBefore = T.Fail;
        Console.SetOut(captured);
        try { suite.Run(); }
        catch (Exception ex) { T.Fail++; captured.WriteLine("FAIL " + name + " threw: " + ex); }
        finally { Console.SetOut(console); }
        string text = captured.ToString();
        output.WriteLine(name + ": passed " + (T.Pass - passBefore) + ", failed " + (T.Fail - failBefore));
        if (text.Length > 0) output.WriteLine(text);
        Assert.True(T.Fail == failBefore, string.Join(Environment.NewLine, text.Split('\n').Where(l => l.StartsWith("FAIL") || l.StartsWith("  ")).Take(40)));
        Assert.True(T.Pass > passBefore, name + " checked nothing");
    }
}
