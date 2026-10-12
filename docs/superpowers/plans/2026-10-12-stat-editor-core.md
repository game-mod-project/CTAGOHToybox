# 스탯 편집기 코어 (구현 순서 1~2단계) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 화면 없이 명령줄로 "원본의 값 하나를 바꾸는 수정"을 프로젝트 파일에 담고, 빌드해서 `mod/CTAToybox/resource`에 써넣고, 게임에서 그 값이 바뀌었는지 확인할 수 있게 한다.

**Architecture:** 기존 C# 파서(`tools/lib/*.cs`)를 `editor/CtaToybox.Core`(net8.0 라이브러리)로 옮기고 PowerShell 도구는 빌드된 DLL을 불러 쓴다. 코어에 원본 계층(바닐라 pak + 켜 둔 모드), 수정 목록, 토큰 치환, 빌드를 더한다. `editor/CtaToybox.Cli`가 코어를 호출한다.

**Tech Stack:** .NET 8 SDK (C# 12), `System.IO.Compression`, `System.Text.Json`. PowerShell 7. NuGet 패키지를 쓰지 않는다. 테스트는 외부 프레임워크 없이 콘솔 프로젝트로 돌린다(이 저장소의 `tools/tests/TestKit.ps1`과 같은 방식).

**Spec:** `docs/superpowers/specs/2026-10-12-stat-editor-design.md`

**이 계획의 범위:** 스펙 "구현 순서"의 1단계(코어 이전)와 2단계(원본 계층, 토큰 치환, 빌드, 게임 안 확인)다. 3단계(출처 추적, "이 유닛만")와 4단계(WPF 화면)는 2단계의 게임 안 확인 결과를 본 뒤 별도 계획으로 쓴다. 스펙이 "2단계에서 게임이 수정을 받아들이지 않으면 화면을 만들기 전에 설계를 고친다"고 정했기 때문이다.

## Global Constraints

- 게임 폴더(`E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell`)와 창작마당 폴더는 읽기만 한다. 쓰는 곳은 `mod/CTAToybox/` 아래와 `editor/` 아래뿐이다.
- 프로필의 `options.set`은 읽기만 한다.
- 대상 프레임워크는 `net8.0`. NuGet 패키지 참조를 추가하지 않는다.
- 출력은 pak으로 묶지 않은 낱개 파일이다.
- 파일을 새로 조립하지 않는다. 원본 텍스트에서 값 토큰만 바꾼다. 바꾸지 않은 바이트는 원본과 같아야 한다.
- 빌드가 지우는 파일은 이전 빌드의 목록 파일 `mod/CTAToybox/.ctatoybox-build.json`에 적힌 것뿐이다.
- 이 계획에서 `scope`는 `shared`만 적용한다. `unit`은 "지원하지 않음"으로 보고한다.
- 브랜치: `develop`에서 `feat/stat-editor-core`를 분기해 작업하고 `git merge --no-ff`로 `develop`에 병합한다. `main`에 직접 커밋하지 않는다.
- 기존 PowerShell 테스트(`pwsh -NoProfile -File tools/tests/Run-Tests.ps1`)는 모든 Task 뒤에 통과해야 한다.
- 코드의 주석과 사용자에게 보이는 문구는 한국어, 식별자는 영어. 주변 코드의 주석 밀도를 따른다.
- 커밋 메시지 끝에 `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`를 붙인다.

## Review Focus

1. **ASCII가 아닌 바이트와 BOM이 든 파일.** 게임 파일은 Latin-1로 읽는다. 0x80 이상의 바이트, UTF-8 BOM, UTF-16 LE BOM이 있는 파일도 고친 토큰 밖의 바이트는 그대로여야 한다. → Task 2, Task 6의 테스트.
2. **새 값에 공백, 괄호, 따옴표, `;`가 든 경우.** `120 {evil}`이나 `1 ; x` 같은 값은 파일 구조를 깨뜨린다. 거부해야 한다. → Task 6.
3. **한 파일에 길이가 다른 수정 여러 건.** 앞의 치환이 뒤의 위치를 밀면 엉뚱한 글자를 덮어쓴다. → Task 6.
4. **목록 파일이 출력 폴더 밖을 가리키거나, 사람이 넣은 파일이 출력 폴더에 있는 경우.** 빌드는 목록에 있고 출력 폴더 안에 있는 파일만 지워야 한다. → Task 8.
5. **같은 가상 경로가 여러 계층, pak과 낱개 파일, 대소문자만 다른 이름으로 겹치는 경우.** 뒤 계층이 이기고, 한 계층 안에서는 낱개 파일이 이긴다(스펙의 가정). → Task 3.
6. **수정을 모두 끄고 다시 빌드한 경우.** 이전 빌드의 파일이 남으면 안 된다. → Task 8.

---

## File Structure

```
editor/
├─ CtaToybox.Core/
│   ├─ CtaToybox.Core.csproj
│   ├─ Sdl/Sdl.cs, SdlKeys.cs, SdlRefs.cs, SdlResolve.cs   tools/lib에서 옮김
│   ├─ TextCodec.cs        바이트 ↔ 텍스트. 인코딩을 기억해 그대로 되쓴다
│   ├─ SourceStack.cs      계층(바닐라, 모드)에서 가상 경로로 파일을 찾는다
│   ├─ EditorSettings.cs   경로 설정 (editor/settings.json)
│   ├─ GameProfile.cs      options.set의 모드 목록 → 계층 목록
│   ├─ KeyLocator.cs       (경로, 몇 번째, 몇 번째 값) → 원본 토큰
│   ├─ Edit.cs             수정 한 건과 프로젝트 파일
│   ├─ TokenPatcher.cs     텍스트 하나에 수정 여러 건을 적용
│   └─ ModBuilder.cs       프로젝트 → 출력 폴더, 목록 파일
├─ CtaToybox.Core.Tests/
│   ├─ CtaToybox.Core.Tests.csproj
│   ├─ Program.cs          테스트 도구(T)와 진입점
│   └─ *Tests.cs           부분별 테스트
└─ CtaToybox.Cli/
    ├─ CtaToybox.Cli.csproj
    └─ Program.cs          init, sources, show, add, build
```

옮긴 `Sdl*.cs`는 전역 네임스페이스에 그대로 둔다(PowerShell이 `[SdlCore]`로 부른다). 새 코드는 `CtaToybox.Core` 네임스페이스다.

명령은 모두 저장소 루트(`E:\CTAToybox`)에서 실행한다.

- 코어 테스트: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
- 기존 테스트: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`

---

### Task 1: 코어 프로젝트를 만들고 C# 코드를 옮긴다

**Files:**
- Create: `editor/CtaToybox.Core/CtaToybox.Core.csproj`
- Move: `tools/lib/Sdl.cs`, `SdlKeys.cs`, `SdlRefs.cs`, `SdlResolve.cs` → `editor/CtaToybox.Core/Sdl/`
- Modify: `editor/CtaToybox.Core/Sdl/Sdl.cs` (`Unwrap`, `IsNumber` 추가)
- Modify: `tools/lib/SdlTokenizer.ps1`
- Modify: `.gitignore`

**Interfaces:**
- Consumes: 없음
- Produces: `editor/CtaToybox.Core/bin/Release/net8.0/CtaToybox.Core.dll`. 전역 형식 `SdlCore`, `SdlNode`, `SdlToken`, `SdlParseResult`, `SdlRow`, `SdlResolver`, `SdlReferenceIndex`, `SdlKeyAggregator`는 이름과 멤버가 그대로다. 새 멤버 `public static bool SdlCore.IsNumber(string s)`.

- [ ] **Step 1: 브랜치를 만든다**

```bash
git -C E:/CTAToybox switch develop
git -C E:/CTAToybox switch -c feat/stat-editor-core
```

- [ ] **Step 2: 기존 테스트가 지금 통과하는지 확인한다 (기준선)**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 마지막 줄 `all test files passed`

- [ ] **Step 3: 파일을 옮긴다**

```bash
mkdir -p editor/CtaToybox.Core/Sdl
git mv tools/lib/Sdl.cs tools/lib/SdlKeys.cs tools/lib/SdlRefs.cs tools/lib/SdlResolve.cs editor/CtaToybox.Core/Sdl/
```

- [ ] **Step 4: 프로젝트 파일을 쓴다**

`editor/CtaToybox.Core/CtaToybox.Core.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <LangVersion>12</LangVersion>
    <AssemblyName>CtaToybox.Core</AssemblyName>
    <RootNamespace>CtaToybox.Core</RootNamespace>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

- [ ] **Step 5: `Sdl.cs`에서 PowerShell 어셈블리 의존을 없앤다**

코어는 `System.Management.Automation`을 참조하지 않는다. `editor/CtaToybox.Core/Sdl/Sdl.cs`의 `Unwrap`을 다음으로 바꾼다.

```csharp
    // PowerShell이 넘긴 PSObject 래퍼를 벗긴다. 코어가 PowerShell 어셈블리를 참조하지 않도록 형식 이름으로 알아본다
    public static SdlNode Unwrap(object x)
    {
        var n = x as SdlNode;
        if (n != null) return n;
        if (x != null && x.GetType().FullName == "System.Management.Automation.PSObject")
        {
            object inner = x.GetType().GetProperty("BaseObject").GetValue(x);
            n = inner as SdlNode;
            if (n != null) return n;
        }
        throw new ArgumentException("not an SdlNode: " + (x == null ? "null" : x.GetType().FullName));
    }
```

같은 파일의 `Tokenize` 바로 위에 추가한다.

```csharp
    public static bool IsNumber(string s) { return NumberRegex.IsMatch(s); }
```

- [ ] **Step 6: 빌드한다**

Run: `dotnet build editor/CtaToybox.Core -c Release --nologo -v q`
Expected: `오류 0개` (경고는 허용). `editor/CtaToybox.Core/bin/Release/net8.0/CtaToybox.Core.dll`이 생긴다.

- [ ] **Step 7: PowerShell이 DLL을 불러 쓰게 한다**

`tools/lib/SdlTokenizer.ps1`의 `if (-not ('SdlCore' -as [type])) { ... }` 블록 전체를 다음으로 바꾼다.

```powershell
# 토크나이저·파서 본체는 editor/CtaToybox.Core(C#)에 있다. .mdl 파일이 커서 PowerShell 객체로는 너무 느리다.
# 소스가 DLL보다 새로우면 다시 빌드한다. 파일을 잠그지 않도록 바이트로 읽어 올린다(잠그면 다음 빌드가 실패한다).
if (-not ('SdlCore' -as [type])) {
    $coreDir = Join-Path $PSScriptRoot '../../editor/CtaToybox.Core'
    $coreDll = Join-Path $coreDir 'bin/Release/net8.0/CtaToybox.Core.dll'
    $newestSource = Get-ChildItem -LiteralPath $coreDir -Recurse -File -Include '*.cs', '*.csproj' |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' } | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
    if (-not (Test-Path -LiteralPath $coreDll) -or (Get-Item -LiteralPath $coreDll).LastWriteTimeUtc -lt $newestSource.LastWriteTimeUtc) {
        $log = & dotnet build $coreDir -c Release --nologo -v q 2>&1
        if ($LASTEXITCODE -ne 0) { throw "CtaToybox.Core 빌드 실패:`n$($log -join "`n")" }
    }
    [void][Reflection.Assembly]::Load([IO.File]::ReadAllBytes((Resolve-Path -LiteralPath $coreDll).Path))
}
```

- [ ] **Step 8: `.gitignore`에 빌드 산출물과 개인 설정을 더한다**

`.gitignore` 끝에 추가:

```
bin/
obj/
editor/settings.json
```

- [ ] **Step 9: 기존 테스트가 그대로 통과하는지 확인한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 마지막 줄 `all test files passed`. 8개 파일의 `failed=0`.

- [ ] **Step 10: 데이터가 달라지지 않았는지 확인한다**

Run: `pwsh -NoProfile -File tools/Test-Coverage.ps1 -DataDir data -DocsDir docs\04-units`
Expected: 마지막 줄에 `keys-missing=0 keys-extra=0`. 그 뒤 `git restore docs/04-units/coverage.md`로 생성 시각 변경을 되돌린다.

- [ ] **Step 11: 커밋한다**

```bash
git add -A
git commit -m "refactor: move the SDL parser into editor/CtaToybox.Core" -m "PowerShell tools now load the built DLL instead of compiling the sources." -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 2: 테스트 프로젝트와 TextCodec

**Files:**
- Create: `editor/CtaToybox.Core.Tests/CtaToybox.Core.Tests.csproj`
- Create: `editor/CtaToybox.Core.Tests/Program.cs`
- Create: `editor/CtaToybox.Core.Tests/TextCodecTests.cs`
- Create: `editor/CtaToybox.Core/TextCodec.cs`

**Interfaces:**
- Consumes: Task 1의 코어 프로젝트
- Produces:
  - `enum CtaToybox.Core.TextKind { Latin1, Utf8Bom, Utf16LeBom }`
  - `static string TextCodec.Decode(byte[] bytes, out TextKind kind)`
  - `static byte[] TextCodec.Encode(string text, TextKind kind)`
  - 테스트 도구 `static class T`: `T.Eq<TValue>(TValue expected, TValue actual, string name)`, `T.True(bool condition, string name)`, `T.Throws<TException>(Action action, string name)`, `string T.TempDir()`, `void T.Zip(string pakPath, params (string Name, string Text)[] entries)`, `void T.Put(string root, string relativePath, string text)`. 텍스트는 Latin-1로 쓴다.

- [ ] **Step 1: 테스트 프로젝트와 도구를 쓴다**

`editor/CtaToybox.Core.Tests/CtaToybox.Core.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <LangVersion>12</LangVersion>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\CtaToybox.Core\CtaToybox.Core.csproj" />
  </ItemGroup>
</Project>
```

`editor/CtaToybox.Core.Tests/Program.cs`:

```csharp
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
        var suites = new List<(string Name, Action Run)>
        {
            ("TextCodec", TextCodecTests.Run),
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
```

- [ ] **Step 2: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/TextCodecTests.cs`:

```csharp
using System.Text;
using CtaToybox.Core;

static class TextCodecTests
{
    public static void Run()
    {
        // Latin-1: 0x80 이상의 바이트가 그대로 돌아온다
        byte[] latin = { (byte)'{', (byte)'a', 0xE9, 0x85, 0xA0, 0xFF, (byte)'}' };
        string text = TextCodec.Decode(latin, out TextKind kind);
        T.Eq(TextKind.Latin1, kind, "no BOM is Latin-1");
        T.Eq(7, text.Length, "Latin-1 keeps one char per byte");
        T.Eq(latin, TextCodec.Encode(text, kind), "Latin-1 round-trips byte for byte");

        byte[] utf8 = { 0xEF, 0xBB, 0xBF, (byte)'{', 0xED, 0x95, 0x9C, (byte)'}' };
        text = TextCodec.Decode(utf8, out kind);
        T.Eq(TextKind.Utf8Bom, kind, "UTF-8 BOM is detected");
        T.Eq("{한}", text, "UTF-8 text is decoded without the BOM");
        T.Eq(utf8, TextCodec.Encode(text, kind), "UTF-8 BOM round-trips");

        byte[] utf16 = Encoding.Unicode.GetPreamble();
        utf16 = Concat(utf16, Encoding.Unicode.GetBytes("{a 1}"));
        text = TextCodec.Decode(utf16, out kind);
        T.Eq(TextKind.Utf16LeBom, kind, "UTF-16 LE BOM is detected");
        T.Eq("{a 1}", text, "UTF-16 text is decoded without the BOM");
        T.Eq(utf16, TextCodec.Encode(text, kind), "UTF-16 round-trips");

        T.Eq(new byte[0], TextCodec.Encode(TextCodec.Decode(new byte[0], out kind), kind), "empty file round-trips");
    }

    static byte[] Concat(byte[] a, byte[] b)
    {
        var r = new byte[a.Length + b.Length];
        a.CopyTo(r, 0); b.CopyTo(r, a.Length);
        return r;
    }
}
```

- [ ] **Step 3: 테스트가 컴파일 오류로 실패하는지 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 실패, `TextCodec` 또는 `TextKind`을 찾을 수 없다는 오류(CS0103 또는 CS0246).

- [ ] **Step 4: 구현한다**

`editor/CtaToybox.Core/TextCodec.cs`:

```csharp
// 게임 텍스트 파일을 읽고 같은 인코딩으로 되쓴다.
// BOM이 없으면 Latin-1로 읽는다: 바이트 하나가 글자 하나라 고치지 않은 부분이 그대로 보존된다.
using System;
using System.Text;

namespace CtaToybox.Core
{
    public enum TextKind { Latin1, Utf8Bom, Utf16LeBom }

    public static class TextCodec
    {
        static readonly Encoding Latin1 = Encoding.Latin1;
        static readonly Encoding Utf8 = new UTF8Encoding(false);

        public static string Decode(byte[] bytes, out TextKind kind)
        {
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                kind = TextKind.Utf16LeBom;
                return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
            }
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                kind = TextKind.Utf8Bom;
                return Utf8.GetString(bytes, 3, bytes.Length - 3);
            }
            kind = TextKind.Latin1;
            return Latin1.GetString(bytes);
        }

        public static byte[] Encode(string text, TextKind kind)
        {
            switch (kind)
            {
                case TextKind.Utf16LeBom: return Join(new byte[] { 0xFF, 0xFE }, Encoding.Unicode.GetBytes(text));
                case TextKind.Utf8Bom: return Join(new byte[] { 0xEF, 0xBB, 0xBF }, Utf8.GetBytes(text));
                default: return Latin1.GetBytes(text);
            }
        }

        static byte[] Join(byte[] a, byte[] b)
        {
            var r = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, r, 0, a.Length);
            Buffer.BlockCopy(b, 0, r, a.Length, b.Length);
            return r;
        }
    }
}
```

- [ ] **Step 5: 테스트가 통과하는지 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== TextCodec ok`, 마지막 줄 `passed=10 failed=0`, 종료 코드 0.

- [ ] **Step 6: 커밋한다**

```bash
git add -A
git commit -m "feat(editor): add TextCodec and the core test runner" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 3: 원본 계층 (SourceStack)

**Files:**
- Create: `editor/CtaToybox.Core/SourceStack.cs`
- Create: `editor/CtaToybox.Core.Tests/SourceStackTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suites 목록)

**Interfaces:**
- Consumes: `T` (Task 2)
- Produces:
  - `sealed class SourceLayer { string Name; string ResourceDir; SourceLayer(string name, string resourceDir) }`
  - `sealed class SourceFile { string VirtualPath; string Layer; string Container; string Entry; }` — `Container`는 pak 파일이나 낱개 파일의 전체 경로, `Entry`는 pak 안의 원래 이름(낱개 파일이면 `null`)
  - `sealed class SourceStack : IDisposable`
    - `SourceStack(IEnumerable<SourceLayer> layersInLoadOrder)`
    - `void Scan()`
    - `SourceFile Find(string virtualPath)` — 없으면 `null`
    - `IReadOnlyList<SourceFile> Versions(string virtualPath)` — 로드 순서대로. 마지막이 이기는 쪽
    - `byte[] Read(SourceFile file)`
    - `IEnumerable<string> Paths`
    - `IReadOnlyList<SourceLayer> Layers`
    - `static string ToVirtual(string path)`

가상 경로 규칙(기존 `tools/Export-PakText.ps1`과 같다): pak이 `resource` 아래 하위 폴더에 있으면 그 폴더가 앞에 붙는다. `resource/entity/-vehicle.pak` 안의 `eng/tank/a.def`는 `entity/eng/tank/a.def`다. 가상 경로는 `/` 구분, 소문자다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/SourceStackTests.cs`:

```csharp
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
```

`Program.cs`의 `suites`에 한 줄을 더한다.

```csharp
            ("SourceStack", SourceStackTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 실패, `SourceStack`을 찾을 수 없다는 오류.

- [ ] **Step 3: 구현한다**

`editor/CtaToybox.Core/SourceStack.cs`:

```csharp
// 게임이 읽을 파일을 가상 경로로 찾는다. 계층은 로드 순서대로이고 뒤의 계층이 이긴다.
// 한 계층 안에서는 pak을 경로 이름순으로 읽은 뒤 낱개 파일을 읽으므로 낱개 파일이 이긴다.
// (같은 모드 안에서 pak과 낱개 파일이 겹칠 때 게임이 어느 쪽을 쓰는지는 확인되지 않았다. 스펙의 가정이다.)
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class SourceLayer
    {
        public string Name { get; }
        public string ResourceDir { get; }
        public SourceLayer(string name, string resourceDir) { Name = name; ResourceDir = resourceDir; }
    }

    public sealed class SourceFile
    {
        public string VirtualPath { get; set; }
        public string Layer { get; set; }
        public string Container { get; set; }
        public string Entry { get; set; }   // pak 안의 원래 이름. 낱개 파일이면 null
    }

    public sealed class SourceStack : IDisposable
    {
        readonly List<SourceLayer> layers;
        readonly Dictionary<string, List<SourceFile>> index = new Dictionary<string, List<SourceFile>>(StringComparer.Ordinal);
        readonly Dictionary<string, ZipArchive> open = new Dictionary<string, ZipArchive>(StringComparer.OrdinalIgnoreCase);
        static readonly IReadOnlyList<SourceFile> None = new SourceFile[0];

        public SourceStack(IEnumerable<SourceLayer> layersInLoadOrder) { layers = layersInLoadOrder.ToList(); }

        public IReadOnlyList<SourceLayer> Layers { get { return layers; } }
        public IEnumerable<string> Paths { get { return index.Keys; } }

        public static string ToVirtual(string path)
        {
            return path.Replace('\\', '/').TrimStart('/').ToLowerInvariant();
        }

        public void Scan()
        {
            index.Clear();
            foreach (var layer in layers)
            {
                if (!Directory.Exists(layer.ResourceDir)) continue;
                string root = Path.GetFullPath(layer.ResourceDir);
                var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                    .OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
                foreach (string pak in files.Where(f => f.EndsWith(".pak", StringComparison.OrdinalIgnoreCase)))
                {
                    string dir = Path.GetDirectoryName(Path.GetRelativePath(root, pak));
                    string prefix = string.IsNullOrEmpty(dir) ? "" : ToVirtual(dir) + "/";
                    foreach (var e in Archive(pak).Entries)
                    {
                        if (e.FullName.EndsWith("/") || e.FullName.EndsWith("\\")) continue;
                        Add(new SourceFile { VirtualPath = prefix + ToVirtual(e.FullName), Layer = layer.Name, Container = pak, Entry = e.FullName });
                    }
                }
                foreach (string f in files.Where(f => !f.EndsWith(".pak", StringComparison.OrdinalIgnoreCase)))
                    Add(new SourceFile { VirtualPath = ToVirtual(Path.GetRelativePath(root, f)), Layer = layer.Name, Container = f, Entry = null });
            }
        }

        void Add(SourceFile f)
        {
            List<SourceFile> list;
            if (!index.TryGetValue(f.VirtualPath, out list)) { list = new List<SourceFile>(); index[f.VirtualPath] = list; }
            list.Add(f);
        }

        ZipArchive Archive(string pak)
        {
            ZipArchive z;
            if (!open.TryGetValue(pak, out z)) { z = ZipFile.OpenRead(pak); open[pak] = z; }
            return z;
        }

        public IReadOnlyList<SourceFile> Versions(string virtualPath)
        {
            List<SourceFile> list;
            return index.TryGetValue(ToVirtual(virtualPath), out list) ? list : None;
        }

        public SourceFile Find(string virtualPath)
        {
            var v = Versions(virtualPath);
            return v.Count == 0 ? null : v[v.Count - 1];
        }

        public byte[] Read(SourceFile file)
        {
            if (file.Entry == null) return File.ReadAllBytes(file.Container);
            var entry = Archive(file.Container).GetEntry(file.Entry);
            using (var s = entry.Open())
            using (var m = new MemoryStream())
            {
                s.CopyTo(m);
                return m.ToArray();
            }
        }

        public void Dispose()
        {
            foreach (var z in open.Values) z.Dispose();
            open.Clear();
        }
    }
}
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== SourceStack ok`, `failed=0`.

- [ ] **Step 5: 커밋한다**

```bash
git add -A
git commit -m "feat(editor): resolve virtual paths across vanilla and mod layers" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 4: 설정과 모드 목록 (EditorSettings, GameProfile)

**Files:**
- Create: `editor/CtaToybox.Core/EditorSettings.cs`
- Create: `editor/CtaToybox.Core/GameProfile.cs`
- Create: `editor/CtaToybox.Core.Tests/GameProfileTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suites 목록)

**Interfaces:**
- Consumes: `SourceLayer` (Task 3), `SdlCore.Parse`, `SdlCore.Unwrap`
- Produces:
  - `sealed class EditorSettings { string GameDir; string WorkshopDir; string OptionsSetPath; string OutputModDir; string OutputModName; static EditorSettings Load(string path); void Save(string path); }` — `WorkshopDir`는 `...\steamapps\workshop\content\400750`, `OutputModDir`는 `mod.info`가 있는 폴더, `OutputModName`은 게임 `mods` 아래의 폴더 이름
  - `sealed class LayerPlan { List<SourceLayer> Layers; List<string> Warnings; bool OutputModActive; List<string> LayersAfterOutput; }`
  - `static List<string> GameProfile.ReadModEntries(string optionsSetText)`
  - `static LayerPlan GameProfile.PlanLayers(EditorSettings settings, IEnumerable<string> modEntries)`

`options.set`의 모드 목록 형식(2026-10-12 이 PC에서 확인):

```
{options
	...
	{mods
		"mod_3732639687:0"
		"mod_2905667604:0"
	}
}
```

창작마당 모드는 `mod_<숫자 ID>:<숫자>`다. **로컬 모드의 표기는 확인하지 못했다**(확인 시점에 로컬 모드가 꺼져 있었다). 이 Task는 "`mod_<숫자>`가 아니면 `:` 앞부분을 로컬 모드 폴더 이름으로 본다"고 가정하고, Task 10의 게임 안 확인에서 실제 표기를 기록한다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/GameProfileTests.cs`:

```csharp
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class GameProfileTests
{
    public static void Run()
    {
        string options = "{options\n\t{sound {volume 0.5}}\n\t{mods\n\t\t\"mod_111:0\"\n\t\t\"CTAToybox:0\"\n\t\t\"mod_222:2097245\"\n\t\t\"local_one\"\n\t}\n}\n";
        T.Eq(new[] { "mod_111:0", "CTAToybox:0", "mod_222:2097245", "local_one" }, GameProfile.ReadModEntries(options).ToArray(), "mod entries are read in file order");
        T.Eq(0, GameProfile.ReadModEntries("{options {sound {volume 1}}}").Count, "no mods block means no mods");
        T.Eq(0, GameProfile.ReadModEntries("{options {mods}}").Count, "an empty mods block means no mods");

        string game = T.TempDir(), workshop = T.TempDir(), repoMod = T.TempDir();
        Directory.CreateDirectory(Path.Combine(game, "resource"));
        Directory.CreateDirectory(Path.Combine(workshop, "111", "resource"));
        Directory.CreateDirectory(Path.Combine(game, "mods", "local_one", "resource"));
        var settings = new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = "unused", OutputModDir = repoMod, OutputModName = "ctatoybox" };

        var plan = GameProfile.PlanLayers(settings, GameProfile.ReadModEntries(options));
        T.Eq(new[] { "vanilla", "mod_111", "mod_222", "local_one" }, plan.Layers.Select(l => l.Name).ToArray(), "vanilla first, then mods in order, without the output mod");
        T.Eq(Path.Combine(game, "resource"), plan.Layers[0].ResourceDir, "vanilla resource folder");
        T.Eq(Path.Combine(workshop, "111", "resource"), plan.Layers[1].ResourceDir, "workshop mod resource folder");
        T.Eq(Path.Combine(game, "mods", "local_one", "resource"), plan.Layers[3].ResourceDir, "local mod resource folder");
        T.True(plan.OutputModActive, "the output mod is recognised case-insensitively");
        T.Eq(new[] { "mod_222", "local_one" }, plan.LayersAfterOutput.ToArray(), "layers loaded after the output mod are reported");
        T.True(plan.Warnings.Any(w => w.Contains("mod_222") && w.Contains("폴더")), "a mod whose folder is missing is warned about");
        T.True(plan.Warnings.Any(w => w.Contains("뒤에 로드")), "being loaded before other mods is warned about");

        var off = GameProfile.PlanLayers(settings, new[] { "mod_111:0" });
        T.True(!off.OutputModActive, "output mod absent from the list");
        T.True(off.Warnings.Any(w => w.Contains("켜져 있지 않")), "an inactive output mod is warned about");

        string path = Path.Combine(T.TempDir(), "settings.json");
        settings.Save(path);
        var loaded = EditorSettings.Load(path);
        T.Eq(game, loaded.GameDir, "settings round-trip");
        T.Eq("ctatoybox", loaded.OutputModName, "settings round-trip (name)");
    }
}
```

`Program.cs`의 `suites`에 더한다.

```csharp
            ("GameProfile", GameProfileTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 실패, `GameProfile` 또는 `EditorSettings`를 찾을 수 없다는 오류.

- [ ] **Step 3: 구현한다**

`editor/CtaToybox.Core/EditorSettings.cs`:

```csharp
// 이 PC의 경로 설정. editor/settings.json에 저장하며 git에 넣지 않는다.
using System.IO;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class EditorSettings
    {
        public string GameDir { get; set; }          // call_to_arms.exe가 있는 binaries의 부모. resource, mods 폴더가 있다
        public string WorkshopDir { get; set; }      // ...\steamapps\workshop\content\400750
        public string OptionsSetPath { get; set; }   // 프로필의 options.set. 읽기만 한다
        public string OutputModDir { get; set; }     // mod.info가 있는 폴더 (이 저장소의 mod\CTAToybox)
        public string OutputModName { get; set; }    // 게임의 mods 아래 폴더 이름

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static EditorSettings Load(string path)
        {
            return JsonSerializer.Deserialize<EditorSettings>(File.ReadAllText(path, Encoding.UTF8), Json);
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)));
            File.WriteAllText(path, JsonSerializer.Serialize(this, Json) + "\n", new UTF8Encoding(false));
        }
    }
}
```

`editor/CtaToybox.Core/GameProfile.cs`:

```csharp
// 프로필의 options.set에서 켜 둔 모드 목록을 읽어 원본 계층을 정한다. options.set은 읽기만 한다.
// 창작마당 모드는 "mod_<숫자>:<숫자>"다. 그 밖의 항목은 ':' 앞을 로컬 모드 폴더 이름으로 본다(가정. 확인 기록은 docs/03-mod-folder-setup.md).
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public sealed class LayerPlan
    {
        public List<SourceLayer> Layers { get; } = new List<SourceLayer>();
        public List<string> Warnings { get; } = new List<string>();
        public bool OutputModActive { get; set; }
        public List<string> LayersAfterOutput { get; } = new List<string>();   // 출력 모드보다 뒤에 로드되는 모드
    }

    public static class GameProfile
    {
        static readonly Regex Workshop = new Regex(@"^mod_(\d+)(:|$)", RegexOptions.Compiled);

        public static List<string> ReadModEntries(string optionsSetText)
        {
            var result = new List<string>();
            var mods = FindBlock(SdlCore.Parse(optionsSetText).Root, "mods");
            if (mods == null) return result;
            for (int i = 1; i < mods.Children.Count; i++)
            {
                var n = SdlCore.Unwrap(mods.Children[i]);
                if (n.Kind == "str" || n.Kind == "atom") result.Add(n.Text.Trim('"'));
            }
            return result;
        }

        static SdlNode FindBlock(SdlNode node, string name)
        {
            if (node.Children == null) return null;
            if (node.Kind == "brace" && node.Children.Count > 0)
            {
                var head = SdlCore.Unwrap(node.Children[0]);
                if (head.Kind == "atom" && string.Equals(head.Text, name, StringComparison.OrdinalIgnoreCase)) return node;
            }
            foreach (object c in node.Children)
            {
                var found = FindBlock(SdlCore.Unwrap(c), name);
                if (found != null) return found;
            }
            return null;
        }

        public static LayerPlan PlanLayers(EditorSettings settings, IEnumerable<string> modEntries)
        {
            var plan = new LayerPlan();
            plan.Layers.Add(new SourceLayer("vanilla", Path.Combine(settings.GameDir, "resource")));
            foreach (string entry in modEntries)
            {
                var m = Workshop.Match(entry);
                string name, dir;
                if (m.Success)
                {
                    name = "mod_" + m.Groups[1].Value;
                    dir = Path.Combine(settings.WorkshopDir, m.Groups[1].Value, "resource");
                }
                else
                {
                    int colon = entry.IndexOf(':');
                    name = colon < 0 ? entry : entry.Substring(0, colon);
                    if (string.Equals(name, settings.OutputModName, StringComparison.OrdinalIgnoreCase)) { plan.OutputModActive = true; continue; }
                    dir = Path.Combine(settings.GameDir, "mods", name, "resource");
                }
                if (!Directory.Exists(dir)) plan.Warnings.Add("모드 " + name + "의 폴더가 없다: " + dir);
                plan.Layers.Add(new SourceLayer(name, dir));
                if (plan.OutputModActive) plan.LayersAfterOutput.Add(name);
            }
            if (!plan.OutputModActive)
                plan.Warnings.Add("출력 모드 " + settings.OutputModName + "가 게임에서 켜져 있지 않다. 빌드해도 게임에 반영되지 않는다.");
            else if (plan.LayersAfterOutput.Count > 0)
                plan.Warnings.Add("출력 모드보다 뒤에 로드되는 모드가 있다: " + string.Join(", ", plan.LayersAfterOutput) + ". 이 모드들의 파일을 고친 수정은 덮어써진다. options.set의 {mods} 목록에서 " + settings.OutputModName + " 줄을 맨 뒤로 옮긴다.");
            return plan;
        }
    }
}
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== GameProfile ok`, `failed=0`.

- [ ] **Step 5: 커밋한다**

```bash
git add -A
git commit -m "feat(editor): read the active mod list and plan source layers" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 5: 값의 자리를 찾는다 (KeyLocator)

**Files:**
- Modify: `editor/CtaToybox.Core/Sdl/Sdl.cs` (`SdlRow`에 `ArgNodes`)
- Modify: `editor/CtaToybox.Core/Sdl/SdlKeys.cs` (`Row`가 `ArgNodes`를 채운다)
- Create: `editor/CtaToybox.Core/KeyLocator.cs`
- Create: `editor/CtaToybox.Core.Tests/KeyLocatorTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suites 목록)

**Interfaces:**
- Consumes: `SdlCore.KeyRows`, `SdlCore.IsExpr`, `SdlRow`
- Produces:
  - `SdlRow.ArgNodes` (`List<SdlNode>`): 그 행의 인자 노드들
  - `sealed class KeyRow { string Path; string Form; int Occurrence; List<SdlNode> Leaves; }`
  - `static List<KeyRow> KeyLocator.Rows(SdlNode root)` — 형태가 `block`이나 `macro`인 행만. `Occurrence`는 같은 `Path`를 가진 행 가운데 파일에 나온 순서(0부터)
  - `static SdlNode KeyLocator.Find(SdlNode root, string path, int occurrence, int arg)` — 없으면 `null`

**값의 번호(`arg`) 규칙.** 한 행의 "값"은 그 행의 인자에서 뽑은 잎 토큰(낱말과 문자열)이다. 순서대로 0부터 센다.

- 인자가 `name(…)` 꼴이면 괄호 안으로 들어간다. `name` 자체는 값이 아니다.
- 인자가 산술식 `(* 2 3)`이면 연산자 뒤의 피연산자만 값이다.
- 블록 `{…}`과 산술식이 아닌 괄호 구문은 별도의 행이므로 들어가지 않는다.

예: `("mobility" speed(48) reverse((* 2 10)) off({sensor 0}))`의 값은 `48`(0), `2`(1), `10`(2)이다. `{sensor 0}`의 `0`은 경로 `/("mobility")/off()/sensor` 행의 값 0번이다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/KeyLocatorTests.cs`:

```csharp
using System.Linq;
using CtaToybox.Core;

static class KeyLocatorTests
{
    static string[] Leaves(string text, string path, int occurrence)
    {
        var row = KeyLocator.Rows(SdlCore.Parse(text).Root).FirstOrDefault(r => r.Path == path && r.Occurrence == occurrence);
        return row == null ? null : row.Leaves.Select(l => l.Text).ToArray();
    }

    public static void Run()
    {
        string text = "{game_entity\n  {mass 46000}\n  {volume \"body\" {thickness 45 30}}\n  {volume \"turret\" {thickness 100}}\n" +
                      "  {Extender \"inventory\" {box {item \"ap\" 60}}}\n" +
                      "  (\"mobility\" speed(48) reverse((* 2 10)) off({sensor 0}))\n  (include \"a.ext\" scale(1.5))\n  (define \"m\" {k 7})\n}";
        var root = SdlCore.Parse(text).Root;

        T.Eq(new[] { "46000" }, Leaves(text, "/game_entity/mass", 0), "a plain block value");
        T.Eq(new[] { "\"body\"" }, Leaves(text, "/game_entity/volume", 0), "first occurrence of a repeated path");
        T.Eq(new[] { "\"turret\"" }, Leaves(text, "/game_entity/volume", 1), "second occurrence of a repeated path");
        T.Eq(new[] { "45", "30" }, Leaves(text, "/game_entity/volume/thickness", 0), "two values in one row");
        T.Eq(new[] { "100" }, Leaves(text, "/game_entity/volume/thickness", 1), "occurrences are counted per path across parents");
        T.Eq(new[] { "\"ap\"", "60" }, Leaves(text, "/game_entity/extender[inventory]/box/item", 0), "qualified extender path, lower-cased");
        T.Eq(new[] { "48", "2", "10" }, Leaves(text, "/game_entity/(\"mobility\")", 0), "macro call: named args and expression operands, not names or operators");
        T.Eq(new[] { "0" }, Leaves(text, "/game_entity/(\"mobility\")/off()/sensor", 0), "a block inside a macro argument is its own row");
        T.Eq(new[] { "7" }, Leaves(text, "/game_entity/(define \"m\")/k", 0), "a block inside a macro definition");
        T.True(Leaves(text, "/game_entity/(include)", 0) == null, "include rows are not editable rows");
        T.True(Leaves(text, "/game_entity/(define \"m\")", 0) == null, "define rows are not editable rows");

        var node = KeyLocator.Find(root, "/game_entity/volume/thickness", 0, 1);
        T.Eq("30", node.Text, "Find returns the token");
        T.Eq("30", text.Substring(node.Index, node.Text.Length), "the token's Index points into the source text");
        T.True(KeyLocator.Find(root, "/game_entity/volume/thickness", 0, 2) == null, "a value index past the end is null");
        T.True(KeyLocator.Find(root, "/game_entity/volume/thickness", 2, 0) == null, "an occurrence past the end is null");
        T.True(KeyLocator.Find(root, "/game_entity/nope", 0, 0) == null, "an unknown path is null");
        T.Eq("46000", KeyLocator.Find(root, "/GAME_ENTITY/Mass", 0, 0).Text, "paths compare case-insensitively");
    }
}
```

`Program.cs`의 `suites`에 더한다.

```csharp
            ("KeyLocator", KeyLocatorTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 실패, `KeyLocator`를 찾을 수 없다는 오류.

- [ ] **Step 3: 행이 인자 노드를 들고 다니게 한다**

`editor/CtaToybox.Core/Sdl/Sdl.cs`의 `SdlRow`에 속성을 더한다.

```csharp
public sealed class SdlRow
{
    public string Path { get; set; }
    public string Form { get; set; }   // block | include | define | mod | macro | paren-other | bare
    public string Shape { get; set; }
    public string Args { get; set; }
    public List<SdlNode> ArgNodes { get; set; }   // 이 행의 인자 노드. 편집기가 원본 토큰의 자리를 찾는 데 쓴다
}
```

`editor/CtaToybox.Core/Sdl/SdlKeys.cs`의 `Row`에서 반환문을 바꾼다.

```csharp
        return new SdlRow { Path = path, Form = form, Shape = string.Join(" ", shapes), Args = string.Join(" ", flat), ArgNodes = args };
```

- [ ] **Step 4: KeyLocator를 구현한다**

`editor/CtaToybox.Core/KeyLocator.cs`:

```csharp
// (경로, 몇 번째 행, 몇 번째 값)으로 원본 파일의 토큰을 찾는다.
// 경로는 SdlCore.KeyRows가 만드는 것과 같다(docs/04-units/README.md의 "경로 표기").
// 값은 행의 인자에서 뽑은 잎 토큰이다: name(...)은 안으로 들어가고, 산술식은 피연산자만 센다.
using System;
using System.Collections.Generic;

namespace CtaToybox.Core
{
    public sealed class KeyRow
    {
        public string Path { get; set; }
        public string Form { get; set; }
        public int Occurrence { get; set; }      // 같은 경로를 가진 행 가운데 파일에 나온 순서
        public List<SdlNode> Leaves { get; set; }
    }

    public static class KeyLocator
    {
        // tools/lib/SdlKeys.ps1의 $SdlQualifiedBlocks와 같아야 한다
        static readonly string[] Qualified = { "extender" };

        public static List<KeyRow> Rows(SdlNode root)
        {
            var result = new List<KeyRow>();
            var seen = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in SdlCore.KeyRows(root.Children, Qualified))
            {
                if (row.Form != "block" && row.Form != "macro") continue;
                int n;
                seen.TryGetValue(row.Path, out n);
                seen[row.Path] = n + 1;
                var leaves = new List<SdlNode>();
                foreach (var a in row.ArgNodes) AddLeaves(a, leaves);
                result.Add(new KeyRow { Path = row.Path, Form = row.Form, Occurrence = n, Leaves = leaves });
            }
            return result;
        }

        static void AddLeaves(SdlNode node, List<SdlNode> leaves)
        {
            if (node.Kind == "atom" || node.Kind == "str") { leaves.Add(node); return; }
            if (node.Kind == "brace") return;                       // 블록은 별도의 행이다
            bool expr = node.Kind == "paren" && SdlCore.IsExpr(node);
            if (node.Kind == "paren" && !expr) return;              // 산술식이 아닌 괄호 구문도 별도의 행이다
            for (int i = expr ? 1 : 0; i < node.Children.Count; i++) AddLeaves(SdlCore.Unwrap(node.Children[i]), leaves);
        }

        public static SdlNode Find(SdlNode root, string path, int occurrence, int arg)
        {
            foreach (var row in Rows(root))
            {
                if (row.Occurrence != occurrence || !string.Equals(row.Path, path, StringComparison.OrdinalIgnoreCase)) continue;
                return arg >= 0 && arg < row.Leaves.Count ? row.Leaves[arg] : null;
            }
            return null;
        }
    }
}
```

- [ ] **Step 5: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== KeyLocator ok`, `failed=0`.

- [ ] **Step 6: 기존 PowerShell 테스트가 통과하는지 확인한다 (`SdlRow`를 고쳤다)**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: `all test files passed`

- [ ] **Step 7: 커밋한다**

```bash
git add -A
git commit -m "feat(editor): locate a value token by key path, occurrence and index" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 6: 수정 모델과 토큰 치환 (Edit, TokenPatcher)

**Files:**
- Create: `editor/CtaToybox.Core/Edit.cs`
- Create: `editor/CtaToybox.Core/TokenPatcher.cs`
- Create: `editor/CtaToybox.Core.Tests/TokenPatcherTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suites 목록)

**Interfaces:**
- Consumes: `KeyLocator.Find` (Task 5), `SdlCore.Parse`, `SdlCore.Tokenize`, `SdlCore.RoundTrip`, `SdlCore.IsNumber`, `TextCodec` (Task 2)
- Produces:
  - `sealed class Edit { string Unit; string File; string Path; int Occurrence; int Arg; string Old; string New; string Scope; bool Enabled; }` — `Scope` 기본값 `"shared"`, `Enabled` 기본값 `true`. `Old`와 `New`는 따옴표를 포함한 토큰 원문이다(`"body"`, `120`)
  - `sealed class EditProject { string Name; List<Edit> Edits; static EditProject Load(string path); void Save(string path); }`
  - `enum EditStatus { Applied, SourceChanged, TargetMissing, Invalid }`
  - `sealed class EditResult { Edit Edit; EditStatus Status; string Message; }`
  - `static string TokenPatcher.CheckNewValue(string oldToken, string newToken, out string warning)` — 문제가 없으면 `null`, 있으면 이유. `warning`은 막지 않는 경고
  - `static string TokenPatcher.Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results)` — 고친 텍스트. 적용한 것이 없으면 `text` 그대로. 고친 결과가 다시 파싱되지 않으면 `InvalidOperationException`

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/TokenPatcherTests.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

static class TokenPatcherTests
{
    static Edit E(string path, int occ, int arg, string old, string @new)
    {
        return new Edit { Unit = "u.def", File = "u.def", Path = path, Occurrence = occ, Arg = arg, Old = old, New = @new };
    }

    static string Apply(string text, out List<EditResult> results, params Edit[] edits)
    {
        results = new List<EditResult>();
        return TokenPatcher.Apply(text, edits, results);
    }

    public static void Run()
    {
        List<EditResult> r;
        string text = "{game_entity ; top comment\r\n\t{mass   46000}\t; kg\r\n\t{volume \"body\" {thickness 45 30}}\r\n\t{volume \"turret\" {thickness 100}}\r\n}\r\n";

        string one = Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "50000"));
        T.Eq(text.Replace("46000", "50000"), one, "only the value token changes; comments, tabs and CRLF stay");
        T.Eq(EditStatus.Applied, r[0].Status, "applied status");

        // 길이가 다른 수정 여러 건: 앞의 치환이 뒤의 위치를 밀지 않는다
        string many = Apply(text, out r,
            E("/game_entity/mass", 0, 0, "46000", "5"),
            E("/game_entity/volume/thickness", 0, 1, "30", "3000000"),
            E("/game_entity/volume/thickness", 1, 0, "100", "1"),
            E("/game_entity/volume", 1, 0, "\"turret\"", "\"t\""));
        T.Eq(text.Replace("46000", "5").Replace("45 30", "45 3000000").Replace("{thickness 100}", "{thickness 1}").Replace("\"turret\"", "\"t\""), many, "several edits of different lengths in one file");
        T.Eq(4, r.Count(x => x.Status == EditStatus.Applied), "all four applied");

        string same = Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "46000"));
        T.Eq(text, same, "an identity edit leaves the text byte-identical");

        T.Eq(text, Apply(text, out r, E("/game_entity/mass", 0, 0, "40000", "50000")), "old value mismatch changes nothing");
        T.Eq(EditStatus.SourceChanged, r[0].Status, "old value mismatch is SourceChanged");
        T.True(r[0].Message.Contains("46000"), "the message shows the current source value");

        Apply(text, out r, E("/game_entity/nope", 0, 0, "1", "2"), E("/game_entity/mass", 3, 0, "46000", "1"), E("/game_entity/mass", 0, 5, "46000", "1"));
        T.Eq(new[] { EditStatus.TargetMissing, EditStatus.TargetMissing, EditStatus.TargetMissing }, r.Select(x => x.Status).ToArray(), "missing path, occurrence and value index");

        string twice = Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "1"), E("/game_entity/mass", 0, 0, "46000", "2"));
        T.Eq(text.Replace("46000", "1"), twice, "the first of two edits on one token wins");
        T.Eq(EditStatus.Invalid, r[1].Status, "the second edit on the same token is Invalid");

        // 새 값 검사: 구조를 깨는 값은 거부한다
        string w;
        T.True(TokenPatcher.CheckNewValue("46000", "50000", out w) == null && w == null, "a number for a number is fine");
        foreach (string bad in new[] { "", "1 2", "1}", "{1", "(1", "1)", "1 ; x", "1;x", "\"a", "a\"b", "1\n2", "\t1" })
            T.True(TokenPatcher.CheckNewValue("46000", bad, out w) != null, "rejected new value: [" + bad.Replace("\n", "\\n").Replace("\t", "\\t") + "]");
        T.True(TokenPatcher.CheckNewValue("\"body\"", "\"hull plate\"", out w) == null, "a quoted string may contain spaces");
        T.True(TokenPatcher.CheckNewValue("\"body\"", "\"a{b}\"", out w) == null, "a quoted string may contain braces");
        T.True(TokenPatcher.CheckNewValue("\"body\"", "body", out w) != null, "a quoted value cannot become unquoted");
        T.True(TokenPatcher.CheckNewValue("46000", "\"46000\"", out w) != null, "an unquoted value cannot become quoted");
        T.True(TokenPatcher.CheckNewValue("46000", "fast", out w) == null && w != null, "a non-number for a number is a warning, not an error");
        T.True(TokenPatcher.CheckNewValue("fast", "slow", out w) == null && w == null, "a word for a word is fine");

        Apply(text, out r, E("/game_entity/mass", 0, 0, "46000", "5 {x}"));
        T.Eq(EditStatus.Invalid, r[0].Status, "Apply refuses an invalid new value");

        var unitScope = E("/game_entity/mass", 0, 0, "46000", "1"); unitScope.Scope = "unit";
        T.Eq(text, Apply(text, out r, unitScope), "unit scope is not applied in this version");
        T.Eq(EditStatus.Invalid, r[0].Status, "unit scope is reported as Invalid");

        var off = E("/game_entity/mass", 0, 0, "46000", "1"); off.Enabled = false;
        T.Eq(text, Apply(text, out r, off), "a disabled edit is skipped");
        T.Eq(0, r.Count, "a disabled edit produces no result");

        Apply("{a {b 1}", out r, E("/a/b", 0, 0, "1", "2"));
        T.Eq(EditStatus.Invalid, r[0].Status, "a source file that does not parse cannot be edited");

        // 0x80 이상의 바이트가 있는 파일: 고친 토큰 밖의 바이트는 그대로다
        byte[] bytes = Encoding.Latin1.GetBytes("{a ; caf\u00E9 \u00A0\u0085\r\n {b 1}}");
        string decoded = TextCodec.Decode(bytes, out TextKind kind);
        byte[] patched = TextCodec.Encode(Apply(decoded, out r, E("/a/b", 0, 0, "1", "22")), kind);
        T.Eq(Encoding.Latin1.GetBytes("{a ; caf\u00E9 \u00A0\u0085\r\n {b 22}}"), patched, "high bytes outside the edited token survive");

        // 프로젝트 파일
        string path = Path.Combine(T.TempDir(), "p.json");
        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("/game_entity/volume", 1, 0, "\"turret\"", "\"t\""));
        project.Save(path);
        var loaded = EditProject.Load(path);
        T.Eq("\"turret\"", loaded.Edits[0].Old, "quotes survive the project file");
        T.Eq("shared", loaded.Edits[0].Scope, "scope defaults to shared");
        T.True(loaded.Edits[0].Enabled, "enabled defaults to true");
        T.True(File.ReadAllText(path).Contains("\"occurrence\": 1"), "the project file uses camelCase names");
    }
}
```

`Program.cs`의 `suites`에 더한다.

```csharp
            ("TokenPatcher", TokenPatcherTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 실패, `Edit` 또는 `TokenPatcher`를 찾을 수 없다는 오류.

- [ ] **Step 3: 수정 모델을 쓴다**

`editor/CtaToybox.Core/Edit.cs`:

```csharp
// 수정 한 건과 프로젝트 파일(editor/projects/<이름>.json).
// Old와 New는 따옴표를 포함한 토큰 원문이다: 문자열이면 "body", 낱말이나 수면 120.
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class Edit
    {
        public string Unit { get; set; }        // 어느 유닛을 열어 놓고 고쳤는지 (가상 경로)
        public string File { get; set; }        // 값이 실제로 있는 파일 (가상 경로)
        public string Path { get; set; }        // 그 파일 안의 키 경로
        public int Occurrence { get; set; }     // 같은 경로 가운데 몇 번째인지
        public int Arg { get; set; }            // 그 행의 몇 번째 값인지 (KeyLocator의 규칙)
        public string Old { get; set; }
        public string New { get; set; }
        public string Scope { get; set; } = "shared";   // shared | unit
        public bool Enabled { get; set; } = true;
    }

    public sealed class EditProject
    {
        public string Name { get; set; }
        public List<Edit> Edits { get; set; } = new List<Edit>();

        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static EditProject Load(string path)
        {
            return JsonSerializer.Deserialize<EditProject>(System.IO.File.ReadAllText(path, Encoding.UTF8), Json);
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)));
            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(this, Json) + "\n", new UTF8Encoding(false));
        }
    }
}
```

(`Edit`에 `File`과 `Path`라는 속성이 있어서 이 파일 안에서는 `System.IO.File`, `System.IO.Path`를 전체 이름으로 쓴다.)

- [ ] **Step 4: 토큰 치환을 구현한다**

`editor/CtaToybox.Core/TokenPatcher.cs`:

```csharp
// 텍스트 하나에 수정 여러 건을 적용한다. 파일을 다시 조립하지 않고 값 토큰의 글자만 바꾼다.
// 위치가 뒤인 것부터 바꾸므로 길이가 달라져도 앞의 위치가 밀리지 않는다.
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public enum EditStatus { Applied, SourceChanged, TargetMissing, Invalid }

    public sealed class EditResult
    {
        public Edit Edit { get; set; }
        public EditStatus Status { get; set; }
        public string Message { get; set; }
    }

    public static class TokenPatcher
    {
        // 새 값이 토큰 하나인지, 원래 값과 같은 종류(따옴표 문자열 또는 낱말)인지 본다.
        // 반환값이 null이 아니면 적용할 수 없는 이유다. warning은 막지 않는 경고다.
        public static string CheckNewValue(string oldToken, string newToken, out string warning)
        {
            warning = null;
            if (string.IsNullOrEmpty(newToken)) return "새 값이 비어 있다";
            var tokens = SdlCore.Tokenize(newToken);
            if (tokens.Count != 1 || tokens[0].Text != newToken || (tokens[0].Kind != "atom" && tokens[0].Kind != "str"))
                return "새 값은 낱말 하나이거나 따옴표로 닫힌 문자열 하나여야 한다 (공백, 괄호, 세미콜론 불가): " + newToken;
            bool oldQuoted = oldToken.StartsWith("\""), newQuoted = tokens[0].Kind == "str";
            if (oldQuoted != newQuoted) return oldQuoted ? "원래 값이 따옴표 문자열이다. 새 값도 따옴표로 감싼다" : "원래 값이 따옴표 없는 낱말이다. 새 값에 따옴표를 쓸 수 없다";
            if (SdlCore.IsNumber(oldToken) && !SdlCore.IsNumber(newToken)) warning = "원래 값은 숫자인데 새 값은 숫자가 아니다: " + newToken;
            return null;
        }

        sealed class Patch { public Edit Edit; public int Index; public int Length; }

        public static string Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results)
        {
            var active = edits.Where(e => e.Enabled).ToList();
            if (active.Count == 0) return text;
            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0)
            {
                foreach (var e in active) results.Add(Result(e, EditStatus.Invalid, "원본 파일이 파싱되지 않는다: " + parsed.Errors[0]));
                return text;
            }

            var patches = new List<Patch>();
            var taken = new HashSet<int>();
            foreach (var e in active)
            {
                if (e.Scope != "shared") { results.Add(Result(e, EditStatus.Invalid, "이 버전은 scope가 shared인 수정만 적용한다")); continue; }
                var node = KeyLocator.Find(parsed.Root, e.Path, e.Occurrence, e.Arg);
                if (node == null) { results.Add(Result(e, EditStatus.TargetMissing, "원본에 이 자리가 없다: " + e.Path + " [" + e.Occurrence + "] 값 " + e.Arg)); continue; }
                if (node.Text != e.Old) { results.Add(Result(e, EditStatus.SourceChanged, "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text)); continue; }
                string warning;
                string error = CheckNewValue(e.Old, e.New, out warning);
                if (error != null) { results.Add(Result(e, EditStatus.Invalid, error)); continue; }
                if (!taken.Add(node.Index)) { results.Add(Result(e, EditStatus.Invalid, "같은 값을 고치는 수정이 앞에 이미 있다")); continue; }
                patches.Add(new Patch { Edit = e, Index = node.Index, Length = node.Text.Length });
                results.Add(Result(e, EditStatus.Applied, warning));
            }
            if (patches.Count == 0) return text;

            var sb = new System.Text.StringBuilder(text);
            foreach (var p in patches.OrderByDescending(x => x.Index))
            {
                sb.Remove(p.Index, p.Length);
                sb.Insert(p.Index, p.Edit.New);
            }
            string patched = sb.ToString();

            // 고친 결과를 다시 읽어 구조가 그대로인지, 그 자리에 새 값이 들어갔는지 확인한다
            var check = SdlCore.Parse(patched);
            if (check.Errors.Count > 0 || !SdlCore.RoundTrip(check)) throw new InvalidOperationException("고친 파일이 다시 파싱되지 않는다");
            foreach (var p in patches)
            {
                var node = KeyLocator.Find(check.Root, p.Edit.Path, p.Edit.Occurrence, p.Edit.Arg);
                if (node == null || node.Text != p.Edit.New) throw new InvalidOperationException("고친 뒤 그 자리에 새 값이 없다: " + p.Edit.Path);
            }
            return patched;
        }

        static EditResult Result(Edit e, EditStatus status, string message)
        {
            return new EditResult { Edit = e, Status = status, Message = message };
        }
    }
}
```

- [ ] **Step 5: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== TokenPatcher ok`, `failed=0`.

- [ ] **Step 6: 커밋한다**

```bash
git add -A
git commit -m "feat(editor): apply value edits by replacing tokens in place" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 7: 빌드 (ModBuilder)

**Files:**
- Create: `editor/CtaToybox.Core/ModBuilder.cs`
- Create: `editor/CtaToybox.Core.Tests/ModBuilderTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suites 목록)

**Interfaces:**
- Consumes: `SourceStack` (Task 3), `TextCodec` (Task 2), `Edit`, `EditProject`, `EditResult`, `EditStatus`, `TokenPatcher.Apply` (Task 6)
- Produces:
  - `sealed class BuildReport { List<EditResult> Results; List<string> WrittenFiles; List<string> RemovedFiles; List<string> FileErrors; List<string> Warnings; int Count(EditStatus status); }` — `WrittenFiles`와 `RemovedFiles`는 출력 모드 폴더 기준 상대 경로(`resource/set/a.set`)
  - `sealed class ModBuilder { ModBuilder(SourceStack stack, string outputModDir); BuildReport Build(EditProject project); const string ManifestName = ".ctatoybox-build.json"; }`
  - `sealed class BuildManifest { string Project; string BuiltAt; List<BuildManifestFile> Files; }`, `sealed class BuildManifestFile { string Path; string Layer; int Edits; }`

목록 파일 `mod/CTAToybox/.ctatoybox-build.json`의 형식:

```json
{
  "project": "my-tweaks",
  "builtAt": "2026-10-12T21:30:00Z",
  "files": [
    { "path": "resource/set/a.set", "layer": "vanilla", "edits": 2 }
  ]
}
```

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/ModBuilderTests.cs`:

```csharp
using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

static class ModBuilderTests
{
    static Edit E(string file, string path, string old, string @new)
    {
        return new Edit { Unit = file, File = file, Path = path, Occurrence = 0, Arg = 0, Old = old, New = @new };
    }

    static string Read(string modDir, string relative) { return Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(modDir, relative.Replace('/', Path.DirectorySeparatorChar)))); }
    static bool Exists(string modDir, string relative) { return File.Exists(Path.Combine(modDir, relative.Replace('/', Path.DirectorySeparatorChar))); }

    public static void Run()
    {
        string vanilla = T.TempDir(), mod = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(vanilla, "gamelogic.pak"), ("set/a.set", "{a {x 1} {y 2}}\r\n"), ("set/b.set", "{b {x 1}}"), ("set/bad.set", "{c {x 1}"));
        T.Put(mod, "set/b.set", "{b {x 5}} ; modded");
        T.Put(outDir, "mod.info", "{mod}");
        T.Put(outDir, "resource/.gitkeep", "");
        T.Put(outDir, "resource/set/hand.set", "hand-made");

        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("modX", mod) });
        stack.Scan();
        var builder = new ModBuilder(stack, outDir);

        var project = new EditProject { Name = "t" };
        project.Edits.Add(E("set/a.set", "/a/x", "1", "10"));
        project.Edits.Add(E("set/a.set", "/a/y", "2", "20"));
        project.Edits.Add(E("set/b.set", "/b/x", "5", "50"));
        project.Edits.Add(E("set/b.set", "/b/x", "1", "99"));          // 바닐라 값 기준이라 모드 값과 다르다
        project.Edits.Add(E("set/gone.set", "/z", "1", "2"));
        project.Edits.Add(E("set/bad.set", "/c/x", "1", "2"));

        var report = builder.Build(project);
        T.Eq("{a {x 10} {y 20}}\r\n", Read(outDir, "resource/set/a.set"), "two edits land in one output file");
        T.Eq("{b {x 50}} ; modded", Read(outDir, "resource/set/b.set"), "the edit applies to the winning layer's file");
        T.Eq(new[] { "resource/set/a.set", "resource/set/b.set" }, report.WrittenFiles.OrderBy(x => x).ToArray(), "written files, relative to the mod folder");
        T.Eq(3, report.Count(EditStatus.Applied), "applied count");
        T.Eq(1, report.Count(EditStatus.SourceChanged), "an edit made against another base is skipped");
        T.Eq(1, report.Count(EditStatus.TargetMissing), "a file that no longer exists is TargetMissing");
        T.Eq(1, report.Count(EditStatus.Invalid), "a file that does not parse is Invalid");
        T.True(!Exists(outDir, "resource/set/bad.set") && !Exists(outDir, "resource/set/gone.set"), "files with no applied edit are not written");
        T.True(Exists(outDir, ModBuilder.ManifestName), "the manifest is written");
        T.True(File.ReadAllText(Path.Combine(outDir, ModBuilder.ManifestName)).Contains("\"path\": \"resource/set/a.set\""), "the manifest lists written files");
        T.Eq("hand-made", Read(outDir, "resource/set/hand.set"), "a hand-placed file is left alone");
        T.True(Exists(outDir, "mod.info") && Exists(outDir, "resource/.gitkeep"), "mod.info and .gitkeep are left alone");

        // 다시 빌드: 이전 빌드의 파일 가운데 이번에 안 쓰는 것은 지운다
        project.Edits.RemoveAll(e => e.File == "set/b.set");
        report = builder.Build(project);
        T.True(!Exists(outDir, "resource/set/b.set"), "a file from the previous build that is no longer produced is removed");
        T.Eq(new[] { "resource/set/b.set" }, report.RemovedFiles.ToArray(), "removed files are reported");
        T.True(Exists(outDir, "resource/set/a.set"), "a file still produced stays");
        T.Eq("hand-made", Read(outDir, "resource/set/hand.set"), "the hand-placed file survives a rebuild");

        // 수정을 모두 끄면 생성물이 남지 않는다
        foreach (var e in project.Edits) e.Enabled = false;
        report = builder.Build(project);
        T.True(!Exists(outDir, "resource/set/a.set"), "with every edit disabled no generated file remains");
        T.Eq(0, report.WrittenFiles.Count, "nothing written");
        T.True(Exists(outDir, "resource/.gitkeep") && Exists(outDir, "resource/set/hand.set"), "non-generated files remain");

        // 목록 파일이 출력 폴더 밖이나 손대면 안 되는 파일을 가리켜도 지우지 않는다
        string outside = Path.Combine(T.TempDir(), "victim.txt");
        File.WriteAllText(outside, "keep");
        string manifest = "{ \"project\": \"t\", \"builtAt\": \"x\", \"files\": [ { \"path\": \"../" + Path.GetFileName(Path.GetDirectoryName(outside)) + "/victim.txt\", \"layer\": \"v\", \"edits\": 1 }, " +
                          "{ \"path\": \"" + outside.Replace("\\", "\\\\") + "\", \"layer\": \"v\", \"edits\": 1 }, { \"path\": \"mod.info\", \"layer\": \"v\", \"edits\": 1 } ] }";
        File.WriteAllText(Path.Combine(outDir, ModBuilder.ManifestName), manifest);
        report = builder.Build(project);
        T.True(File.Exists(outside), "a manifest entry pointing outside the mod folder is not deleted");
        T.True(Exists(outDir, "mod.info"), "a manifest entry outside resource/ is not deleted");
        T.Eq(3, report.Warnings.Count(w => w.Contains("목록 파일")), "each refused manifest entry is warned about");

        // 깨진 목록 파일
        File.WriteAllText(Path.Combine(outDir, ModBuilder.ManifestName), "not json");
        foreach (var e in project.Edits) e.Enabled = true;
        report = builder.Build(project);
        T.True(report.Warnings.Any(w => w.Contains("목록 파일")), "an unreadable manifest is warned about, not fatal");
        T.Eq("{a {x 10} {y 20}}\r\n", Read(outDir, "resource/set/a.set"), "the build still completes");
    }
}
```

`Program.cs`의 `suites`에 더한다.

```csharp
            ("ModBuilder", ModBuilderTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 실패, `ModBuilder`를 찾을 수 없다는 오류.

- [ ] **Step 3: 구현한다**

`editor/CtaToybox.Core/ModBuilder.cs`:

```csharp
// 프로젝트의 수정을 원본에 적용해 출력 모드 폴더의 resource 아래에 낱개 파일로 쓴다.
// 지우는 것은 이전 빌드의 목록 파일에 적혀 있고 resource 아래에 있는 파일뿐이다.
// 새 파일은 임시 폴더에 다 만든 뒤 옮기므로, 적용 도중에 실패하면 이전 빌드 결과가 남는다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class BuildReport
    {
        public List<EditResult> Results { get; } = new List<EditResult>();
        public List<string> WrittenFiles { get; } = new List<string>();
        public List<string> RemovedFiles { get; } = new List<string>();
        public List<string> FileErrors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public int Count(EditStatus status) { return Results.Count(r => r.Status == status); }
    }

    // 목록 파일(.ctatoybox-build.json)의 형식
    public sealed class BuildManifestFile { public string Path { get; set; } public string Layer { get; set; } public int Edits { get; set; } }
    public sealed class BuildManifest { public string Project { get; set; } public string BuiltAt { get; set; } public List<BuildManifestFile> Files { get; set; } = new List<BuildManifestFile>(); }

    public sealed class ModBuilder
    {
        public const string ManifestName = ".ctatoybox-build.json";


        static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        readonly SourceStack stack;
        readonly string modDir, resourceDir;

        public ModBuilder(SourceStack stack, string outputModDir)
        {
            this.stack = stack;
            modDir = Path.GetFullPath(outputModDir);
            resourceDir = Path.Combine(modDir, "resource");
        }

        public BuildReport Build(EditProject project)
        {
            var report = new BuildReport();
            string stage = Path.Combine(Path.GetTempPath(), "ctatoybox-build-" + Guid.NewGuid().ToString("N"));
            var manifest = new BuildManifest { Project = project.Name, BuiltAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") };
            try
            {
                // 1. 임시 폴더에 고친 파일을 만든다
                foreach (var group in project.Edits.Where(e => e.Enabled).GroupBy(e => SourceStack.ToVirtual(e.File)))
                {
                    var source = stack.Find(group.Key);
                    if (source == null)
                    {
                        foreach (var e in group) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없다: " + group.Key });
                        continue;
                    }
                    var results = new List<EditResult>();
                    try
                    {
                        TextKind kind;
                        string text = TextCodec.Decode(stack.Read(source), out kind);
                        string patched = TokenPatcher.Apply(text, group.ToList(), results);
                        report.Results.AddRange(results);
                        int applied = results.Count(r => r.Status == EditStatus.Applied);
                        if (applied == 0) continue;
                        string relative = "resource/" + group.Key;
                        string target = Path.Combine(stage, relative.Replace('/', Path.DirectorySeparatorChar));
                        Directory.CreateDirectory(Path.GetDirectoryName(target));
                        File.WriteAllBytes(target, TextCodec.Encode(patched, kind));
                        manifest.Files.Add(new BuildManifestFile { Path = relative, Layer = source.Layer, Edits = applied });
                    }
                    catch (Exception ex)
                    {
                        report.FileErrors.Add(group.Key + ": " + ex.Message);
                        foreach (var e in group)
                            if (!results.Any(r => r.Edit == e && r.Status != EditStatus.Applied))
                                report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "파일을 만들지 못했다: " + ex.Message });
                    }
                }

                // 2. 이전 빌드의 파일을 지운다
                RemovePrevious(report);

                // 3. 새 파일을 옮기고 목록 파일을 쓴다
                foreach (var f in manifest.Files)
                {
                    string from = Path.Combine(stage, f.Path.Replace('/', Path.DirectorySeparatorChar));
                    string to = Path.Combine(modDir, f.Path.Replace('/', Path.DirectorySeparatorChar));
                    Directory.CreateDirectory(Path.GetDirectoryName(to));
                    File.Copy(from, to, true);
                    report.WrittenFiles.Add(f.Path);
                }
                Directory.CreateDirectory(modDir);
                File.WriteAllText(Path.Combine(modDir, ManifestName), JsonSerializer.Serialize(manifest, Json) + "\n", new UTF8Encoding(false));
            }
            finally
            {
                if (Directory.Exists(stage)) Directory.Delete(stage, true);
            }
            return report;
        }

        void RemovePrevious(BuildReport report)
        {
            string manifestPath = Path.Combine(modDir, ManifestName);
            if (!File.Exists(manifestPath)) return;
            BuildManifest old;
            try { old = JsonSerializer.Deserialize<BuildManifest>(File.ReadAllText(manifestPath, Encoding.UTF8), Json); }
            catch (JsonException)
            {
                report.Warnings.Add("이전 빌드의 목록 파일을 읽지 못했다. 이전 생성물을 지우지 않는다: " + manifestPath);
                return;
            }
            if (old == null || old.Files == null) return;
            string root = resourceDir + Path.DirectorySeparatorChar;
            foreach (var f in old.Files)
            {
                // 목록에 있어도 출력 모드의 resource 아래가 아니면 지우지 않는다
                string full = f.Path == null || Path.IsPathRooted(f.Path) ? null : Path.GetFullPath(Path.Combine(modDir, f.Path.Replace('/', Path.DirectorySeparatorChar)));
                if (full == null || !full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                {
                    report.Warnings.Add("목록 파일의 항목이 출력 폴더의 resource 밖을 가리킨다. 지우지 않는다: " + f.Path);
                    continue;
                }
                if (!File.Exists(full)) continue;
                File.Delete(full);
                report.RemovedFiles.Add(f.Path);
                // 비게 된 폴더를 resource 바로 아래까지 지운다
                string dir = Path.GetDirectoryName(full);
                while (dir.Length > resourceDir.Length && Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    Directory.Delete(dir);
                    dir = Path.GetDirectoryName(dir);
                }
            }
        }
    }
}
```

`RemovedFiles`에는 이번 빌드가 다시 쓰는 파일도 들어간다(지운 뒤 다시 쓴다). 테스트의 "removed files are reported"는 `set/b.set`만 기대하므로, 3단계에서 다시 쓰는 경로는 `RemovedFiles`에서 뺀다. `Build`의 3단계 `report.WrittenFiles.Add(f.Path);` 다음 줄에 추가한다.

```csharp
                    report.RemovedFiles.Remove(f.Path);
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== ModBuilder ok`, `failed=0`.

- [ ] **Step 5: 커밋한다**

```bash
git add -A
git commit -m "feat(editor): build edits into the mod folder with a manifest" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 8: 명령줄 도구 (CtaToybox.Cli)

**Files:**
- Create: `editor/CtaToybox.Cli/CtaToybox.Cli.csproj`
- Create: `editor/CtaToybox.Cli/Program.cs`
- Create: `editor/CtaToybox.Core.Tests/CliTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/CtaToybox.Core.Tests.csproj` (Cli 참조)
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suites 목록)

**Interfaces:**
- Consumes: `EditorSettings`, `GameProfile`, `SourceStack`, `KeyLocator.Rows`, `TokenPatcher.CheckNewValue`, `EditProject`, `ModBuilder`, `TextCodec`
- Produces: `public static class CtaToybox.Cli.CliApp { public static int Run(string[] args, TextWriter output) }`. 종료 코드 0은 성공, 1은 실패, 2는 사용법 오류.

명령:

| 명령 | 하는 일 |
|---|---|
| `init --game <폴더> --workshop <폴더> --options <파일> --out <폴더> [--name <이름>]` | `editor/settings.json`을 쓴다. `--name` 기본값은 `--out` 폴더의 이름 |
| `sources` | 계층 목록, 계층별 파일 수, 경고 |
| `show <가상 경로> [--filter <글자>]` | 그 파일의 편집 가능한 행. `경로 [몇 번째] 값0=… 값1=…` |
| `add --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] --new <값> [--unit <가상 경로>]` | 지금 원본의 값을 `old`로 읽어 수정 한 건을 프로젝트에 더한다. 프로젝트 파일이 없으면 만든다 |
| `build --project <파일>` | 빌드하고 결과를 요약한다 |

모든 명령은 `--settings <파일>`을 받는다. 기본값은 `editor/settings.json`이다.

- [ ] **Step 1: 프로젝트 파일을 쓴다**

`editor/CtaToybox.Cli/CtaToybox.Cli.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <LangVersion>12</LangVersion>
    <AssemblyName>ctatoybox</AssemblyName>
    <RootNamespace>CtaToybox.Cli</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\CtaToybox.Core\CtaToybox.Core.csproj" />
  </ItemGroup>
</Project>
```

`editor/CtaToybox.Core.Tests/CtaToybox.Core.Tests.csproj`의 `ItemGroup`에 참조를 더한다.

```xml
    <ProjectReference Include="..\CtaToybox.Cli\CtaToybox.Cli.csproj" />
```

- [ ] **Step 2: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/CliTests.cs`:

```csharp
using System.IO;
using System.Text;
using CtaToybox.Cli;
using CtaToybox.Core;

static class CliTests
{
    static int Run(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    public static void Run()
    {
        string game = T.TempDir(), workshop = T.TempDir(), outDir = T.TempDir(), work = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), ("set/a.set", "{a {x 1} {name \"n\"}}"));
        T.Put(Path.Combine(workshop, "111", "resource"), "set/a.set", "{a {x 5} {name \"n\"}}");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"mod_111:0\" \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json"), project = Path.Combine(work, "p.json");
        string o;

        T.Eq(2, Run(out o), "no command is a usage error");
        T.Eq(2, Run(out o, "frobnicate"), "an unknown command is a usage error");
        T.Eq(1, Run(out o, "sources", "--settings", settings), "a missing settings file fails");
        T.True(o.Contains("init"), "the failure tells the user to run init");

        T.Eq(0, Run(out o, "init", "--settings", settings, "--game", game, "--workshop", workshop, "--options", options, "--out", outDir), "init succeeds");
        T.Eq(Path.GetFileName(outDir), EditorSettings.Load(settings).OutputModName, "init derives the mod name from the output folder");
        T.Eq(2, Run(out o, "init", "--settings", settings, "--game", game), "init without required options is a usage error");

        T.Eq(0, Run(out o, "sources", "--settings", settings), "sources succeeds");
        T.True(o.Contains("vanilla") && o.Contains("mod_111"), "sources lists the layers");
        T.True(o.Contains("켜져 있다"), "sources reports the output mod as active");

        T.Eq(0, Run(out o, "show", "set/a.set", "--settings", settings), "show succeeds");
        T.True(o.Contains("mod_111"), "show names the winning layer");
        T.True(o.Contains("/a/x [0] 0=5"), "show prints path, occurrence and indexed values");
        T.True(o.Contains("/a/name [0] 0=\"n\""), "show prints quoted values with their quotes");
        T.Eq(0, Run(out o, "show", "set/a.set", "--filter", "name", "--settings", settings), "show with a filter succeeds");
        T.True(!o.Contains("/a/x"), "the filter hides other rows");
        T.Eq(1, Run(out o, "show", "set/zzz.set", "--settings", settings), "show of a missing file fails");

        T.Eq(0, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/x", "--new", "9"), "add succeeds");
        var p = EditProject.Load(project);
        T.Eq("5", p.Edits[0].Old, "add records the current source value as old");
        T.Eq("set/a.set", p.Edits[0].Unit, "unit defaults to the file");
        T.Eq(1, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/x", "--new", "1 2"), "add refuses an invalid value");
        T.Eq(1, Run(out o, "add", "--settings", settings, "--project", project, "--file", "set/a.set", "--path", "/a/nope", "--new", "1"), "add refuses an unknown key");
        T.Eq(1, EditProject.Load(project).Edits.Count, "refused edits are not saved");

        T.Eq(0, Run(out o, "build", "--settings", settings, "--project", project), "build succeeds");
        T.True(o.Contains("적용 1건"), "build prints the summary");
        T.Eq("{a {x 9} {name \"n\"}}", Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", "set", "a.set"))), "build writes the file");
    }
}
```

`Program.cs`의 `suites`에 더한다.

```csharp
            ("Cli", CliTests.Run),
```

- [ ] **Step 3: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 실패, `CtaToybox.Cli` 네임스페이스나 `CliApp`을 찾을 수 없다는 오류.

- [ ] **Step 4: 구현한다**

`editor/CtaToybox.Cli/Program.cs`:

```csharp
// 화면 없이 코어를 쓰는 명령줄 도구. 게임 폴더는 읽기만 하고, 쓰는 곳은 설정 파일, 프로젝트 파일, 출력 모드 폴더다.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CtaToybox.Core;

namespace CtaToybox.Cli
{
    public static class CliApp
    {
        const string Usage =
            "사용법:\n" +
            "  ctatoybox init --game <폴더> --workshop <폴더> --options <파일> --out <폴더> [--name <이름>]\n" +
            "  ctatoybox sources\n" +
            "  ctatoybox show <가상 경로> [--filter <글자>]\n" +
            "  ctatoybox add --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] --new <값> [--unit <가상 경로>]\n" +
            "  ctatoybox build --project <파일>\n" +
            "공통: --settings <파일> (기본 editor/settings.json)";

        sealed class UsageException : Exception { public UsageException(string m) : base(m) { } }

        static int Main(string[] args) { return Run(args, Console.Out); }

        public static int Run(string[] args, TextWriter output)
        {
            try
            {
                if (args.Length == 0) throw new UsageException("명령이 없다");
                var opt = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var positional = new List<string>();
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].StartsWith("--"))
                    {
                        if (i + 1 >= args.Length) throw new UsageException(args[i] + " 뒤에 값이 없다");
                        opt[args[i].Substring(2)] = args[++i];
                    }
                    else positional.Add(args[i]);
                }
                string settingsPath = opt.ContainsKey("settings") ? opt["settings"] : Path.Combine("editor", "settings.json");
                switch (args[0].ToLowerInvariant())
                {
                    case "init": return Init(opt, settingsPath, output);
                    case "sources": return Sources(settingsPath, output);
                    case "show": return Show(positional, opt, settingsPath, output);
                    case "add": return Add(opt, settingsPath, output);
                    case "build": return Build(opt, settingsPath, output);
                    default: throw new UsageException("모르는 명령: " + args[0]);
                }
            }
            catch (UsageException ex)
            {
                output.WriteLine(ex.Message);
                output.WriteLine(Usage);
                return 2;
            }
        }

        static string Need(Dictionary<string, string> opt, string name)
        {
            string v;
            if (!opt.TryGetValue(name, out v) || v.Length == 0) throw new UsageException("--" + name + "이 필요하다");
            return v;
        }

        static int Number(Dictionary<string, string> opt, string name)
        {
            string v;
            if (!opt.TryGetValue(name, out v)) return 0;
            int n;
            if (!int.TryParse(v, out n) || n < 0) throw new UsageException("--" + name + "은 0 이상의 정수여야 한다");
            return n;
        }

        static int Init(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string outDir = Path.GetFullPath(Need(opt, "out"));
            var s = new EditorSettings
            {
                GameDir = Path.GetFullPath(Need(opt, "game")),
                WorkshopDir = Path.GetFullPath(Need(opt, "workshop")),
                OptionsSetPath = Path.GetFullPath(Need(opt, "options")),
                OutputModDir = outDir,
                OutputModName = opt.ContainsKey("name") ? opt["name"] : Path.GetFileName(outDir.TrimEnd(Path.DirectorySeparatorChar))
            };
            s.Save(settingsPath);
            output.WriteLine("설정을 썼다: " + settingsPath);
            return 0;
        }

        // 설정을 읽어 원본 계층을 연다. 실패하면 null을 돌려주고 이유를 쓴다
        static SourceStack Open(string settingsPath, TextWriter output, out EditorSettings settings, out LayerPlan plan)
        {
            settings = null; plan = null;
            if (!File.Exists(settingsPath))
            {
                output.WriteLine("설정 파일이 없다: " + settingsPath + ". 먼저 init을 실행한다.");
                return null;
            }
            settings = EditorSettings.Load(settingsPath);
            if (!File.Exists(settings.OptionsSetPath))
            {
                output.WriteLine("options.set이 없다: " + settings.OptionsSetPath);
                return null;
            }
            plan = GameProfile.PlanLayers(settings, GameProfile.ReadModEntries(File.ReadAllText(settings.OptionsSetPath)));
            var stack = new SourceStack(plan.Layers);
            stack.Scan();
            return stack;
        }

        static int Sources(string settingsPath, TextWriter output)
        {
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var counts = new Dictionary<string, int>();
                foreach (string p in stack.Paths)
                    foreach (var v in stack.Versions(p)) counts[v.Layer] = (counts.TryGetValue(v.Layer, out int n) ? n : 0) + 1;
                output.WriteLine("원본 계층 (로드 순서):");
                foreach (var l in plan.Layers) output.WriteLine("  " + l.Name + "  파일 " + (counts.TryGetValue(l.Name, out int c) ? c : 0) + "  " + l.ResourceDir);
                output.WriteLine("출력 모드 " + settings.OutputModName + ": " + (plan.OutputModActive ? "게임에서 켜져 있다" : "게임에서 꺼져 있다"));
                foreach (string w in plan.Warnings) output.WriteLine("경고: " + w);
                return 0;
            }
        }

        static int Show(List<string> positional, Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            if (positional.Count != 1) throw new UsageException("show에는 가상 경로 하나가 필요하다");
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var file = stack.Find(positional[0]);
                if (file == null) { output.WriteLine("원본에 이 파일이 없다: " + positional[0]); return 1; }
                var versions = stack.Versions(positional[0]);
                output.WriteLine(file.VirtualPath + "  (계층 " + file.Layer + (versions.Count > 1 ? ", 겹침 " + string.Join(" → ", versions.Select(v => v.Layer)) : "") + ")");
                var parsed = SdlCore.Parse(TextCodec.Decode(stack.Read(file), out TextKind kind));
                if (parsed.Errors.Count > 0) { output.WriteLine("파싱되지 않는다: " + parsed.Errors[0]); return 1; }
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                foreach (var row in KeyLocator.Rows(parsed.Root))
                {
                    if (row.Leaves.Count == 0) continue;
                    if (filter != null && row.Path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    output.WriteLine(row.Path + " [" + row.Occurrence + "] " + string.Join(" ", row.Leaves.Select((l, i) => i + "=" + l.Text)));
                }
                return 0;
            }
        }

        static int Add(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string projectPath = Need(opt, "project"), fileArg = Need(opt, "file"), keyPath = Need(opt, "path"), newValue = Need(opt, "new");
            int occ = Number(opt, "occ"), arg = Number(opt, "arg");
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var file = stack.Find(fileArg);
                if (file == null) { output.WriteLine("원본에 이 파일이 없다: " + fileArg); return 1; }
                var parsed = SdlCore.Parse(TextCodec.Decode(stack.Read(file), out TextKind kind));
                if (parsed.Errors.Count > 0) { output.WriteLine("파싱되지 않는 파일은 고칠 수 없다: " + parsed.Errors[0]); return 1; }
                var node = KeyLocator.Find(parsed.Root, keyPath, occ, arg);
                if (node == null) { output.WriteLine("이 자리가 없다: " + keyPath + " [" + occ + "] 값 " + arg + ". show로 확인한다."); return 1; }
                string warning;
                string error = TokenPatcher.CheckNewValue(node.Text, newValue, out warning);
                if (error != null) { output.WriteLine(error); return 1; }
                if (warning != null) output.WriteLine("경고: " + warning);

                var project = File.Exists(projectPath) ? EditProject.Load(projectPath) : new EditProject { Name = Path.GetFileNameWithoutExtension(projectPath) };
                project.Edits.Add(new Edit
                {
                    Unit = SourceStack.ToVirtual(opt.ContainsKey("unit") ? opt["unit"] : fileArg),
                    File = file.VirtualPath, Path = keyPath.ToLowerInvariant(), Occurrence = occ, Arg = arg, Old = node.Text, New = newValue
                });
                project.Save(projectPath);
                output.WriteLine("수정을 더했다: " + file.VirtualPath + " " + keyPath + " " + node.Text + " → " + newValue + " (계층 " + file.Layer + ")");
                return 0;
            }
        }

        static int Build(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string projectPath = Need(opt, "project");
            if (!File.Exists(projectPath)) { output.WriteLine("프로젝트 파일이 없다: " + projectPath); return 1; }
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                foreach (string w in plan.Warnings) output.WriteLine("경고: " + w);
                if (Process.GetProcessesByName("call_to_arms").Length > 0 || Process.GetProcessesByName("call_to_arms_ed").Length > 0)
                    output.WriteLine("경고: 게임이나 에디터가 실행 중이다. 다시 시작해야 반영된다.");
                var report = new ModBuilder(stack, settings.OutputModDir).Build(EditProject.Load(projectPath));
                foreach (var r in report.Results.Where(x => x.Status != EditStatus.Applied || x.Message != null))
                    output.WriteLine(r.Status + ": " + r.Edit.File + " " + r.Edit.Path + " [" + r.Edit.Occurrence + "] 값 " + r.Edit.Arg + (r.Message == null ? "" : " : " + r.Message));
                foreach (string w in report.Warnings) output.WriteLine("경고: " + w);
                foreach (string e in report.FileErrors) output.WriteLine("오류: " + e);
                foreach (string f in report.WrittenFiles) output.WriteLine("씀: " + f);
                foreach (string f in report.RemovedFiles) output.WriteLine("지움: " + f);
                output.WriteLine("적용 " + report.Count(EditStatus.Applied) + "건, 원본이 바뀜 " + report.Count(EditStatus.SourceChanged) + "건, 대상 없음 " +
                                 report.Count(EditStatus.TargetMissing) + "건, 적용 불가 " + report.Count(EditStatus.Invalid) + "건, 파일 오류 " + report.FileErrors.Count + "건");
                return report.FileErrors.Count > 0 ? 1 : 0;
            }
        }
    }
}
```

- [ ] **Step 5: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== Cli ok`, `failed=0`.

- [ ] **Step 6: 커밋한다**

```bash
git add -A
git commit -m "feat(editor): add a command line front end for edits and builds" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 9: 실제 게임 파일 검증

**Files:**
- Create: `editor/CtaToybox.Core.Tests/RealFilesCheck.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (`real` 인자)

**Interfaces:**
- Consumes: `EditorSettings.Load`, `GameProfile`, `SourceStack`, `TextCodec`, `KeyLocator.Rows`, `TokenPatcher.Apply`, `Edit`
- Produces: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real [settings.json]`. 게임이 설치된 PC에서만 돈다. 인자가 없는 보통 실행에는 포함되지 않는다.

검사 내용: 유닛 범위의 모든 텍스트 파일에 대해 (1) 디코드한 뒤 다시 인코드하면 원본 바이트와 같은가, (2) 값이 있는 첫 행과 마지막 행에 "같은 값으로 바꾸는" 수정을 적용한 결과가 원본 바이트와 같은가.

- [ ] **Step 1: 검사를 쓴다**

`editor/CtaToybox.Core.Tests/RealFilesCheck.cs`:

```csharp
// 실제 게임 파일로 하는 검증. 게임이 설치된 PC에서 "-- real"로만 돈다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class RealFilesCheck
{
    static readonly string[] Prefixes = { "set/breed/", "set/stuff/", "properties/", "entity/-vehicle/", "entity/humanskin/", "entity/inventory/" };
    static readonly HashSet<string> Extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".def", ".set", ".ext", ".inc", ".weapon", ".ammo", ".grenade", ".mine", ".pattern", ".presets", ".hand", ".fsm", "" };

    public static void Run(string settingsPath)
    {
        var settings = EditorSettings.Load(settingsPath);
        var plan = GameProfile.PlanLayers(settings, GameProfile.ReadModEntries(File.ReadAllText(settings.OptionsSetPath)));
        using var stack = new SourceStack(plan.Layers);
        stack.Scan();

        int files = 0, unparsed = 0, noValues = 0, codecMismatch = 0, patchMismatch = 0;
        var examples = new List<string>();
        foreach (string vp in stack.Paths.OrderBy(p => p, StringComparer.Ordinal))
        {
            if (!Prefixes.Any(p => vp.StartsWith(p, StringComparison.Ordinal)) || !Extensions.Contains(Path.GetExtension(vp))) continue;
            files++;
            byte[] original = stack.Read(stack.Find(vp));
            string text = TextCodec.Decode(original, out TextKind kind);
            if (!original.AsSpan().SequenceEqual(TextCodec.Encode(text, kind))) { codecMismatch++; if (examples.Count < 10) examples.Add("codec: " + vp); continue; }

            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0) { unparsed++; continue; }
            var rows = KeyLocator.Rows(parsed.Root).Where(r => r.Leaves.Count > 0).ToList();
            if (rows.Count == 0) { noValues++; continue; }
            var edits = new List<Edit>();
            foreach (var row in new[] { rows[0], rows[rows.Count - 1] }.Distinct())
            {
                int arg = row.Leaves.Count - 1;
                edits.Add(new Edit { Unit = vp, File = vp, Path = row.Path, Occurrence = row.Occurrence, Arg = arg, Old = row.Leaves[arg].Text, New = row.Leaves[arg].Text });
            }
            var results = new List<EditResult>();
            string patched;
            try { patched = TokenPatcher.Apply(text, edits, results); }
            catch (Exception ex) { patchMismatch++; if (examples.Count < 10) examples.Add("throw: " + vp + " " + ex.Message); continue; }
            bool ok = results.All(r => r.Status == EditStatus.Applied) && original.AsSpan().SequenceEqual(TextCodec.Encode(patched, kind));
            if (!ok) { patchMismatch++; if (examples.Count < 10) examples.Add("patch: " + vp + " " + string.Join(",", results.Select(r => r.Status))); }
        }

        Console.WriteLine("layers=" + string.Join(",", plan.Layers.Select(l => l.Name)));
        Console.WriteLine("files=" + files + " unparsed=" + unparsed + " no-values=" + noValues + " codec-mismatch=" + codecMismatch + " patch-mismatch=" + patchMismatch);
        foreach (string e in examples) Console.WriteLine("  " + e);
        T.Eq(0, codecMismatch, "every file re-encodes to its original bytes");
        T.Eq(0, patchMismatch, "an identity edit leaves every file byte-identical");
        T.True(files > 1000, "the unit scope was actually scanned");
    }
}
```

`editor/CtaToybox.Core.Tests/Program.cs`의 `Main` 첫머리에 넣는다.

```csharp
        if (args.Length > 0 && args[0] == "real")
        {
            try { RealFilesCheck.Run(args.Length > 1 ? args[1] : System.IO.Path.Combine("editor", "settings.json")); }
            catch (Exception ex) { T.Fail++; Console.WriteLine("FAIL real files threw: " + ex); }
            Console.WriteLine("passed=" + T.Pass + " failed=" + T.Fail);
            return T.Fail > 0 ? 1 : 0;
        }
```

- [ ] **Step 2: 이 PC의 설정 파일을 만든다**

Run:

```powershell
$profile = Get-ChildItem "$env:LOCALAPPDATA\digitalmindsoft\gates of hell\profiles" -Directory | Where-Object Name -ne 'offline' | Select-Object -First 1
dotnet run --project editor/CtaToybox.Cli -c Release -- init --game "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell" --workshop "E:\SteamLibrary\steamapps\workshop\content\400750" --options "$($profile.FullName)\options.set" --out "E:\CTAToybox\mod\CTAToybox"
```

Expected: `설정을 썼다: editor\settings.json`. `git status`에 `editor/settings.json`이 나오지 않는다(무시 대상).

- [ ] **Step 3: 계층이 맞게 잡히는지 본다**

Run: `dotnet run --project editor/CtaToybox.Cli -c Release -- sources`
Expected: `vanilla`의 파일 수가 350,000개 안팎(`data/file-index.csv`는 350,397행). 켜 둔 모드가 `options.set`의 순서대로 나온다. 없는 폴더에 대한 경고가 없다. 출력 모드가 꺼져 있으면 "켜져 있지 않다" 경고가 나오는데, 이 단계에서는 정상이다.

- [ ] **Step 4: 실제 파일 검사를 돌린다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`
Expected: `codec-mismatch=0 patch-mismatch=0`, 마지막 줄 `failed=0`. `files`는 수천 개 이상. `unparsed`는 0이 아닐 수 있다(바닐라에 왕복 검사에 실패하는 파일이 1개 있다).

`patch-mismatch`가 0이 아니면 출력된 예시 파일을 `show`로 열어 원인을 찾는다. `KeyLocator`의 값 규칙이 그 파일의 구문을 놓친 것이므로 `KeyLocatorTests`에 그 구문의 테스트를 먼저 더하고 고친다.

- [ ] **Step 5: 보통 실행이 여전히 통과하는지 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 6개 묶음이 모두 `ok`, `failed=0`.

- [ ] **Step 6: 커밋한다**

```bash
git add -A
git commit -m "test(editor): check identity edits against the installed game files" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 10: 문서, 게임 안 확인 절차, 병합

**Files:**
- Create: `docs/07-stat-editor/README.md`
- Create: `docs/07-stat-editor/ingame-check.md`
- Modify: `CLAUDE.md`
- Modify: `README.md`
- Modify: `tools/tests/Run-Tests.ps1`

**Interfaces:**
- Consumes: Task 8의 명령줄 도구
- Produces: 사용법 문서와, 사람이 게임에서 따라 하는 확인 절차. 게임 실행은 사람이 한다(도구로 할 수 없다).

- [ ] **Step 1: 코어 테스트를 전체 테스트에 넣는다**

`tools/tests/Run-Tests.ps1`에서 `if ($failed -gt 0)` 줄 바로 위에 넣는다.

```powershell
Write-Host '== editor/CtaToybox.Core.Tests'
dotnet run --project (Join-Path $PSScriptRoot '../../editor/CtaToybox.Core.Tests') -c Release
if ($LASTEXITCODE -ne 0) { $failed++ }
```

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: `== editor/CtaToybox.Core.Tests` 아래에 `failed=0`, 마지막 줄 `all test files passed`.

- [ ] **Step 2: 사용법 문서를 쓴다**

`docs/07-stat-editor/README.md`:

````markdown
# 스탯 편집기

기준: 2026-10-12, 게임 1.0.6.7. 설계는 [../superpowers/specs/2026-10-12-stat-editor-design.md](../superpowers/specs/2026-10-12-stat-editor-design.md)에 있다.

지금 있는 것은 명령줄 도구다. 화면은 아직 없다.

## 할 수 있는 것과 없는 것

| 할 수 있다 | 아직 없다 |
|---|---|
| 원본 파일(바닐라와 켜 둔 모드)의 값 하나를 바꾸는 수정을 프로젝트 파일에 담기 | 유닛 검색, 화면 |
| 프로젝트를 빌드해 `mod/CTAToybox/resource`에 써넣기 | "이 유닛만" 고치기. 지금은 그 파일을 쓰는 모든 유닛이 바뀐다 |
| 게임이나 모드가 갱신돼 원래 값이 달라진 수정을 알아채기 | 키나 블록을 더하거나 빼기 |

게임 폴더와 창작마당 폴더, 프로필의 `options.set`은 읽기만 한다.

## 처음 한 번

저장소 루트에서 실행한다.

```powershell
$profile = Get-ChildItem "$env:LOCALAPPDATA\digitalmindsoft\gates of hell\profiles" -Directory | Where-Object Name -ne 'offline' | Select-Object -First 1
dotnet run --project editor/CtaToybox.Cli -c Release -- init --game "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell" --workshop "E:\SteamLibrary\steamapps\workshop\content\400750" --options "$($profile.FullName)\options.set" --out "E:\CTAToybox\mod\CTAToybox"
```

`editor/settings.json`이 생긴다. 이 파일은 git에 넣지 않는다.

## 값 하나를 고치기

아래에서 `ctatoybox`는 `dotnet run --project editor/CtaToybox.Cli -c Release --`를 줄여 쓴 것이다.

1. 원본 구성을 본다.

   ```
   ctatoybox sources
   ```

2. 고칠 파일의 값을 본다. 어느 파일에 어떤 값이 있는지는 [../06-editable-attributes/README.md](../06-editable-attributes/README.md)에서 찾는다.

   ```
   ctatoybox show entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --filter box/item
   ```

   한 줄이 한 행이다. `경로 [몇 번째] 0=값 1=값`.

3. 수정을 더한다. `--occ`는 대괄호 안의 수, `--arg`는 값 앞의 수다. 둘 다 생략하면 0이다. 문자열 값은 따옴표까지 적는다.

   ```
   ctatoybox add --project editor/projects/my-tweaks.json --file entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --path "/game_entity/extender[inventory]/box/item" --occ 0 --arg 1 --new 7
   ```

4. 빌드한다.

   ```
   ctatoybox build --project editor/projects/my-tweaks.json
   ```

5. 게임을 다시 시작한다. 모드 목록에서 `CTAToybox`가 켜져 있어야 한다.

## 빌드 결과의 뜻

| 표시 | 뜻 | 할 일 |
|---|---|---|
| 적용 | 출력 폴더에 반영했다 | |
| 원본이 바뀜 | 수정을 만들 때의 원래 값과 지금 원본의 값이 다르다. 적용하지 않았다 | 지금 값을 `show`로 보고 수정을 다시 만든다 |
| 대상 없음 | 그 파일이나 그 자리가 원본에 없다 | 모드를 껐거나 갱신으로 구조가 바뀐 것이다 |
| 적용 불가 | 새 값이 낱말 하나나 닫힌 문자열 하나가 아니거나, 원본 파일이 파싱되지 않는다 | |

## 빌드가 건드리는 파일

- 쓰는 곳은 `mod/CTAToybox/resource` 아래와 `mod/CTAToybox/.ctatoybox-build.json`뿐이다.
- 지우는 것은 이전 빌드가 `.ctatoybox-build.json`에 적어 둔 파일뿐이다. 손으로 넣은 파일은 그대로 둔다.
- 수정을 모두 끄고 빌드하면 생성물이 모두 사라진다.

## 알려진 한계

- **같은 경로의 파일은 나중에 로드된 모드가 이긴다는 것은 추정이다.** 그래서 `CTAToybox`가 `options.set`의 `{mods}` 목록에서 맨 뒤에 있어야 한다. `sources`와 `build`가 순서를 확인해 경고한다. 순서는 직접 고친다.
- **한 모드 안에서 pak과 낱개 파일이 겹치면 낱개 파일을 쓴다고 가정했다.** 게임의 실제 동작은 확인하지 않았다.
- **도구는 문법이 깨지지 않는 것까지만 보장한다.** 값이 게임에서 뜻한 대로 동작하는지는 게임에서 본다.
- 게임 안 확인 결과는 [ingame-check.md](ingame-check.md)에 있다.
````

- [ ] **Step 3: 게임 안 확인 절차를 쓴다**

`docs/07-stat-editor/ingame-check.md`:

````markdown
# 게임 안 확인

스펙의 구현 순서 2단계다. 이 확인이 끝나기 전에는 화면을 만들지 않는다. 결과가 "게임이 수정을 받아들이지 않는다"이면 설계를 고친다.

게임 실행은 사람이 한다. 아래에서 `ctatoybox`는 `dotnet run --project editor/CtaToybox.Cli -c Release --`를 줄여 쓴 것이다.

## 절차

1. 게임을 실행해 모드 목록에서 `CTAToybox`를 켜고 적용한 뒤 게임을 끈다.
2. `ctatoybox sources`를 실행한다. "출력 모드 CTAToybox: 게임에서 켜져 있다"가 나와야 한다.
   - "꺼져 있다"로 나오면 `options.set`의 `{mods}` 블록에서 CTAToybox에 해당하는 줄을 찾아 아래 "기록"에 그대로 옮겨 적는다. 도구가 로컬 모드의 표기를 잘못 가정한 것이므로 `GameProfile.PlanLayers`를 그 표기에 맞게 고친다.
   - "뒤에 로드되는 모드가 있다"는 경고가 나오면 `options.set`에서 CTAToybox 줄을 `{mods}` 목록의 맨 뒤로 옮긴다.
3. 바닐라 파일 하나를 고친다. KV-85의 적재 탄약 수를 쓴다(인벤토리 화면에서 바로 보인다).

   ```
   ctatoybox show entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --filter box/item
   ```

   첫 줄의 마지막 값(수량)을 고른다. 그 줄의 대괄호 안 수를 `--occ`에, 수량 앞의 번호를 `--arg`에 넣고 새 값은 7로 한다.

   ```
   ctatoybox add --project editor/projects/ingame-check.json --file entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --path "/game_entity/extender[inventory]/box/item" --occ <대괄호 안 수> --arg <수량 앞의 번호> --new 7
   ctatoybox build --project editor/projects/ingame-check.json
   ```

   `show`에 `box/item` 줄이 없으면 이 차량은 적재품을 include하는 파일에서 받는 것이다. 그때는 `show`를 `--filter` 없이 실행해 `(include)`가 아닌 숫자 값 하나를 고르고, 아래 "기록"에 무엇을 골랐는지 적는다.
4. 게임을 실행한다. 에디터(F3)나 스커미시에서 KV-85를 놓고 인벤토리를 열어 그 탄약의 수량이 7인지 본다.
5. 켜 둔 모드가 있으면 그 모드의 파일 하나에도 같은 확인을 한다. `sources`에 나온 모드 가운데 하나를 골라 그 모드가 덮어쓴 파일을 `show`로 열면 첫 줄에 "계층 mod_…"가 나온다. 3~4와 같은 방법으로 값 하나를 고쳐 본다.
6. 확인이 끝나면 수정을 끄고 다시 빌드해 생성물을 지운다.

   ```
   ctatoybox build --project editor/projects/ingame-check.json
   ```

   (`ingame-check.json`의 각 수정에서 `"enabled": true`를 `false`로 바꾼 뒤 실행한다.)

## 기록

| 항목 | 결과 |
|---|---|
| 확인한 날짜와 게임 버전 | 확인하지 않음 |
| `options.set`에서 CTAToybox의 표기 | 확인하지 않음 |
| 바닐라 파일의 수정이 게임에 반영됨 | 확인하지 않음 |
| 모드 파일의 수정이 게임에 반영됨 | 확인하지 않음 |
| 수정을 끄고 다시 빌드한 뒤 원래 값으로 돌아옴 | 확인하지 않음 |
| 게임 로그(`game.log`)에 이 모드와 관련된 오류 | 확인하지 않음 |
````

- [ ] **Step 4: `CLAUDE.md`와 `README.md`를 고친다**

`CLAUDE.md`의 "폴더" 표에서 `tools/` 행 다음에 한 행을 더한다.

```markdown
| `editor/` | 스탯 편집기 (.NET 8). 파서 본체(`CtaToybox.Core`)가 여기 있고 `tools/`의 스크립트가 이 DLL을 불러 쓴다 |
```

"명령" 목록에 더한다.

```markdown
- 편집기 테스트: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`. 전체 테스트(`Run-Tests.ps1`)에 포함돼 있다. 실제 게임 파일 검증은 끝에 `-- real`을 붙인다.
- 편집기 명령줄: `dotnet run --project editor/CtaToybox.Cli -c Release -- <명령>`. 사용법은 `docs/07-stat-editor/README.md`.
```

"구현 메모"의 첫 항목을 바꾼다.

```markdown
- 파서, 리졸버, 집계의 본체는 C#(`editor/CtaToybox.Core/Sdl/*.cs`)이고 PowerShell 함수가 감싼다. `.mdl` 파일이 커서 PowerShell로는 너무 느리다. `tools/lib/SdlTokenizer.ps1`이 소스가 DLL보다 새로우면 다시 빌드한다.
```

"손으로 고치는 파일" 목록에 더한다.

```markdown
- `editor/projects/*.json`: 편집기의 수정 목록. `mod/CTAToybox/resource`의 생성물과 `.ctatoybox-build.json`은 빌드가 만든다
```

`README.md`의 "문서" 표에 한 행을 더한다.

```markdown
| [docs/07-stat-editor/README.md](docs/07-stat-editor/README.md) | 스탯 편집기(명령줄)의 사용법과 한계 |
```

`README.md`의 "폴더" 표에서 `tools/` 행 다음에 한 행을 더한다.

```markdown
| `editor/` | 스탯 편집기. 지금은 명령줄 도구다 |
```

- [ ] **Step 5: 전체 테스트와 커버리지를 확인한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: `all test files passed`

Run: `pwsh -NoProfile -File tools/Test-Coverage.ps1 -DataDir data -DocsDir docs\04-units`
Expected: `keys-missing=0 keys-extra=0`. 그 뒤 `git restore docs/04-units/coverage.md`.

- [ ] **Step 6: 커밋하고 병합한다**

```bash
git add -A
git commit -m "docs(editor): usage, in-game check procedure, repo docs" -m "Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
git switch develop
git merge --no-ff feat/stat-editor-core -m "Merge branch 'feat/stat-editor-core' into develop"
git branch -d feat/stat-editor-core
```

- [ ] **Step 7: 사용자에게 게임 안 확인을 요청한다**

`docs/07-stat-editor/ingame-check.md`의 절차 1과 4는 게임 실행이라 사람이 해야 한다. 2, 3, 6의 명령은 에이전트가 실행할 수 있다. 사용자가 결과를 알려 주면 "기록" 표를 채우고, `options.set`에서 확인한 로컬 모드 표기를 `docs/03-mod-folder-setup.md`에도 적는다.

---

## 이 계획이 다루지 않는 것 (다음 계획)

| 스펙 항목 | 다음 계획 |
|---|---|
| Units (유닛 검색, 유닛을 이루는 파일 묶음) | 3단계 계획 |
| Provenance (펼친 값의 원본 위치, 공유하는 유닛 목록) | 3단계 계획 |
| `scope: unit` (include 사본, 매크로 인자) | 3단계 계획 |
| WPF 화면 (요약 탭, 전체 탭, 수정 목록) | 4단계 계획 |
