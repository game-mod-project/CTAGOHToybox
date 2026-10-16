# 스탯 편집기 출처 추적과 "이 유닛만" (구현 순서 3단계) Implementation Plan

> **상태(2026-10-17)**: 끝났다(값의 출처 추적). 아래 체크 상자는 진행 중에 표시하지 않았다. 무엇을 했는지는 git 기록과 `docs/10-backlog.md` "정리하며 지운 것"에 있다.

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 명령줄에서 유닛을 찾고, 유닛의 펼친 값이 어느 파일의 어느 자리에서 왔는지와 몇 개 유닛이 그 파일을 같이 쓰는지 보고, include로 공유된 값을 "이 유닛만" 고쳐 빌드할 수 있게 한다.

**Architecture:** 리졸버가 펼친 노드마다 원본 파일을 적고 include 지시문의 자리를 기록하게 한다. 그 위에 유닛 색인(유닛 → 읽는 파일), 출처 추적(펼친 값 → 파일·키 경로·몇 번째 값), include 사슬 계산을 얹는다. 빌더는 `scope: unit` 수정을 만나면 유닛에서 그 파일까지의 include 사슬을 유닛 전용 사본으로 만들고 include 경로를 사본으로 고쳐 쓴다.

**Tech Stack:** .NET 8 SDK (C# 12), `System.IO.Compression`, `System.Text.Json`, `System.Security.Cryptography`. PowerShell 7. NuGet 패키지를 쓰지 않는다. 테스트는 외부 프레임워크 없이 콘솔 프로젝트(`editor/CtaToybox.Core.Tests`)로 돌린다.

**Spec:** `docs/superpowers/specs/2026-10-12-stat-editor-design.md`

**이 계획의 범위:** 스펙 "구현 순서"의 3단계다. 4단계(WPF 화면)는 이 계획의 게임 안 확인(Task 7) 뒤에 별도 계획으로 쓴다.

**이 계획에서 빼는 것 (스펙에는 있다):**

- 유닛이 참조하는 무기·탄약 파일을 유닛의 파일 묶음에 넣는 것. 이름으로 파일을 잇는 규칙(`{weapon "85mm_d5t"}` → 어느 파일)을 엔진에서 확인한 근거가 아직 없다. 근거를 조사한 뒤 4단계 계획에서 다룬다. 무기·탄약 파일 자체는 지금도 유닛(`stuff`)으로 검색되고 `shared`로 고칠 수 있다.
- 표시 이름은 `data/display-names.csv`에 `desc/entity/<파일 이름>` 키가 있는 엔티티만 붙인다(예: `desc/entity/kv85` → `KV-85`). 보병(`set/breed`)과 아이템(`set/stuff`)의 이름 키 규칙은 확인하지 않았으므로 붙이지 않는다.

## 설계의 근거 (2026-10-13에 데이터로 확인한 것)

추정이 아니라 관측한 사실만 적는다. 구현 중 이와 다른 것을 보면 멈추고 보고한다.

| 사실 | 근거 |
|---|---|
| 같은 경로의 파일은 `options.set`의 `{mods}`에서 뒤에 있는 모드의 것이 쓰인다. 모드의 낱개 파일이 바닐라 pak의 파일을 이긴다 | 게임 안 확인. `docs/07-stat-editor/ingame-check.md` |
| 유닛은 16,230개다(바닐라). 종류는 `tools/lib/Common.ps1`의 `Get-SdlUnitKind` 규칙으로 정한다 | `_work/vanilla`를 그 규칙으로 센 결과 |
| 바닐라 유닛 전체를 펼치는 데 73초가 걸린다(낱개 파일 기준) | 기존 `SdlResolver`를 PowerShell 루프로 돌려 측정. 그래서 색인을 캐시하기로 했다. 구현 뒤 C#에서 잰 값은 유닛 21,612개에 8.2초다(Task 6의 실제 파일 검증) |
| 차량과 보병 파일의 include 대상은 `.inc`(10,942건)와 `.ext`(1,996건)뿐이다. `.def`를 include하는 `.def`는 없다 | `data/units-vehicle.jsonl.gz`, `data/units-human.jsonl.gz` |
| include 대상 파일 이름에는 숫자와 밑줄이 쓰인다(`aec_mk1.ext` 등 411개가 `[a-z_]` 밖의 글자를 가진다) | `_work/vanilla/entity/-vehicle`의 `.ext`·`.inc` 이름 |
| include 경로는 세 형태다: `"kv85.ext"`(같은 폴더), `"/properties/tank.ext"`(루트 기준), 인자가 붙은 `"/properties/selection/vehicle.inc" scale(1.2)` | `entity/-vehicle/ussr/tank_heavy/kv85/kv85.def` |
| 매크로 인자는 유닛 파일이나 include 파일 안의 토큰이다: `("mobility_tank" speed(40) ... fuel(600) ...)` | 같은 파일 119행 |
| `m3a1_halftrack.inc`는 `.def` 3개(`m3a1_halftrack`, `m3a1_halftrack_inf`, `m3a1_halftrack_inf_late`)가 include하고, 그 안에 `{item "mine m1a1" 24}`가 있다. 지금 구성에서 이 파일의 계층은 `vanilla`다. `m3a1_halftrack`은 MACE 스커미시 후기 미국 진영에서 일반 구매된다(`cd(150)`) | `_work/vanilla`, `ctatoybox show`, MACE의 `set/multiplayer/units/late/units_usa.set` 914행 |

**아직 모르는 것 (Task 7의 게임 안 확인으로 본다):** 게임이 사본 파일 이름(`<이름>__ctb_<유닛>_<해시>.inc`)을 받아들이는지, include 경로를 고쳐 쓴 유닛 파일을 정상으로 읽는지.

## Global Constraints

- 게임 폴더(`E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell`)와 창작마당 폴더는 읽기만 한다. 쓰는 곳은 `mod/CTAToybox/` 아래와 `editor/` 아래뿐이다.
- 프로필의 `options.set`은 읽기만 한다.
- 대상 프레임워크는 `net8.0`. NuGet 패키지 참조를 추가하지 않는다.
- 출력은 pak으로 묶지 않은 낱개 파일이다.
- 파일을 새로 조립하지 않는다. 원본 텍스트에서 값 토큰과 include 경로 토큰만 바꾼다. 바꾸지 않은 바이트는 원본과 같아야 한다.
- 빌드가 지우는 파일은 이전 빌드의 목록 파일 `mod/CTAToybox/.ctatoybox-build.json`에 적힌 것뿐이다.
- 매크로 본문(`(define ...)` 안)의 값은 "이 유닛만"으로 고칠 수 없다. `shared`만 된다(스펙 "적용 범위별 동작").
- 키나 블록을 더하거나 빼지 않는다. 있는 값을 바꾸는 것만 한다.
- 게임·엔진의 동작을 추정해 코드나 문서에 사실처럼 적지 않는다. 확인하지 않은 것은 "미확인"으로 적는다.
- 브랜치: `develop`에서 `feat/stat-editor-provenance`를 분기해 작업하고 `git merge --no-ff`로 `develop`에 병합한다. `main`에 직접 커밋하지 않는다.
- 모든 Task 뒤에 `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`이 통과해야 한다(PowerShell 테스트 + 코어 테스트).
- 기존 코어 파일은 `editor/CtaToybox.Core/` 바로 아래에 평평하게 있다. 새 파일도 같은 자리에 둔다.
- 코드의 주석과 사용자에게 보이는 문구는 한국어, 식별자는 영어. 주변 코드의 주석 밀도를 따른다.
- 커밋 메시지 끝에 `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`를 붙인다.

## Review Focus

1. **같은 파일이 유닛에서 두 경로로 include되는 경우**(유닛 → A → F, 유닛 → F). 사본은 한 벌이어야 하고 F를 가리키는 include는 모두 사본으로 바뀌어야 한다. 하나라도 원본을 가리키면 "이 유닛만" 고친 값과 원래 값이 섞인다. → Task 3, Task 5의 테스트.
2. **같은 include 지시문이 펼치기에서 여러 번 지나가는 경우**(매크로가 두 번 호출되거나 파일이 두 번 include됨). 같은 자리를 두 번 고쳐 쓰면 파일이 깨진다. → Task 5의 `Rewrites`가 자리로 중복을 거른다. Task 5의 테스트.
3. **사본을 만들지 못했는데 유닛 파일의 include만 고쳐 쓴 경우.** 없는 파일을 include하는 유닛이 게임에 들어간다. 사본이 하나라도 실패하면 그 유닛의 include는 그대로 둬야 한다. → Task 5의 테스트.
4. **`shared` 수정과 `unit` 수정이 같은 파일에 걸린 경우.** 사본에도 `shared` 수정이 들어가야 한다(아니면 그 유닛만 `shared` 수정을 잃는다). 같은 자리를 둘 다 고치면 `unit`이 이긴다. → Task 5의 테스트.
5. **수정을 끄고 다시 빌드한 경우.** 사본과 고쳐 쓴 유닛 파일이 모두 지워져야 한다. → Task 5의 테스트.
6. **원본이 바뀐 뒤 색인 캐시가 낡은 경우.** 모드를 켜고 끄거나 모드가 갱신되면 색인을 다시 만들어야 한다. → Task 2의 테스트.
7. **펼친 값이 원본 토큰 그대로가 아닌 경우**(`"x_%scale"` 같은 부분 치환, `(/ 10 4)` 같은 계산). 이런 값을 편집 가능한 자리로 보여 주면 틀을 고치게 된다. 편집 불가로 표시해야 한다. → Task 1, Task 4의 테스트.

---

## File Structure

| 파일 | 하는 일 | Task |
|---|---|---|
| `editor/CtaToybox.Core/Sdl/Sdl.cs` (수정) | `SdlNode`에 `File`, `Synthetic` 추가 | 1 |
| `editor/CtaToybox.Core/Sdl/SdlResolve.cs` (수정) | 읽기 함수를 받는 생성자, 노드에 파일 적기, include 자리 기록 | 1 |
| `editor/CtaToybox.Core/SourceStack.cs` (수정) | `ReadText` | 2 |
| `editor/CtaToybox.Core/UnitKind.cs` (신규) | 가상 경로 → 유닛 종류 | 2 |
| `editor/CtaToybox.Core/UnitIndex.cs` (신규) | 유닛 목록, 유닛이 읽는 파일, 파일을 쓰는 유닛, 캐시 | 2 |
| `editor/CtaToybox.Core/DisplayNames.cs` (신규) | `data/display-names.csv`에서 엔티티 표시 이름 | 2 |
| `editor/CtaToybox.Core/IncludeChain.cs` (신규) | include 사슬, 사본 집합, 사본 이름, include 경로 고쳐 쓰기, "이 유닛만" 가능 여부 | 3 |
| `editor/CtaToybox.Core/UnitTracer.cs` (신규) | 유닛의 펼친 값 → 파일·키 경로·몇 번째 값 | 4 |
| `editor/CtaToybox.Core/TokenPatcher.cs` (수정) | `RawPatch`(include 경로 치환), `scope` 검사 제거 | 5 |
| `editor/CtaToybox.Core/ModBuilder.cs` (수정) | `scope: unit` 적용 | 5 |
| `editor/CtaToybox.Cli/Program.cs` (수정) | `units`, `trace`, `add --scope unit`, 모르는 옵션 거부 | 6 |
| `editor/CtaToybox.Core.Tests/*.cs` | Task마다 테스트 파일 하나 | 1~6 |
| `docs/07-stat-editor/README.md`, `ingame-check.md` (수정) | 사용법, 3단계 게임 안 확인 절차 | 7 |

---

### Task 1: 리졸버가 출처를 적는다

**Files:**
- Modify: `editor/CtaToybox.Core/Sdl/Sdl.cs` (`SdlNode` 클래스)
- Modify: `editor/CtaToybox.Core/Sdl/SdlResolve.cs`
- Create: `editor/CtaToybox.Core.Tests/ResolverOriginTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suites 목록)

**Interfaces:**
- Consumes: 기존 `SdlResolver`, `SdlCore.Parse`, `KeyLocator.Rows(SdlNode root)`.
- Produces:
  - `SdlNode.File` (`string`): 이 토큰이 있는 파일의 가상 경로. 리졸버가 읽은 트리에만 채워진다.
  - `SdlNode.Synthetic` (`bool`): `%` 치환이나 산술 계산으로 만든 값이면 `true`.
  - `public sealed class SdlIncludeEdge { string FromFile; int Index; string Raw; string ToFile; }` (모두 `{ get; set; }` 속성). `Index`와 `Raw`는 `FromFile` 안에 있는 include 경로 문자열 토큰의 자리와 원문(따옴표 포함).
  - `SdlResolveResult.IncludeEdges` (`List<SdlIncludeEdge>`).
  - `public SdlResolver(Func<string, string> readText)`: 가상 경로를 주면 텍스트를, 없으면 `null`을 돌려주는 함수로 파일을 읽는다. 기존 `SdlResolver(string[] workDirs)`는 그대로 둔다(PowerShell 도구가 쓴다).

- [ ] **Step 1: 브랜치를 만든다**

```bash
git -C E:/CTAToybox checkout develop
git -C E:/CTAToybox checkout -b feat/stat-editor-provenance
```

- [ ] **Step 2: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/ResolverOriginTests.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class ResolverOriginTests
{
    static readonly Dictionary<string, string> Files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["entity/v/.presets"] = "(define \"mob\" {speed %speed} {mass 10})",
        ["entity/v/a/a.def"] = "{game_entity\n (include \"a.inc\" scale(1.5))\n (include \"/properties/tank.ext\")\n (\"mob\" speed(40))\n {half (/ 10 4)}\n}",
        ["entity/v/a/a.inc"] = "{box {item \"mine\" 24}}\n{size %scale}\n{name \"x_%scale\"}\n(include \"deep.inc\")",
        ["entity/v/a/deep.inc"] = "{deep 1}",
        ["properties/tank.ext"] = "{armor 30}",
    };

    static SdlNode Leaf(SdlResolveResult r, string path, int arg)
    {
        var root = new SdlNode("root", "", 0, true);
        root.Children.AddRange(r.Nodes);
        return KeyLocator.Rows(root).First(x => x.Path == path).Leaves[arg];
    }

    static void Origin(SdlResolveResult r, string path, int arg, string file, string text, string name)
    {
        var leaf = Leaf(r, path, arg);
        T.Eq(file, leaf.File, name + ": file");
        T.Eq(false, leaf.Synthetic, name + ": not synthetic");
        T.Eq(text, Files[file].Substring(leaf.Index, text.Length), name + ": Index points at the token in that file");
    }

    public static void Run()
    {
        var r = new SdlResolver(vp => Files.TryGetValue(vp, out string t) ? t : null).Resolve("entity/v/a/a.def");
        T.Eq(0, r.Unresolved.Count, "the fixture resolves completely");

        Origin(r, "/game_entity/box/item", 1, "entity/v/a/a.inc", "24", "a value in an included file");
        Origin(r, "/game_entity/deep", 0, "entity/v/a/deep.inc", "1", "a value two includes deep");
        Origin(r, "/game_entity/armor", 0, "properties/tank.ext", "30", "a value from a root-relative include");
        Origin(r, "/game_entity/size", 0, "entity/v/a/a.def", "1.5", "an include argument comes from the including file");
        Origin(r, "/game_entity/speed", 0, "entity/v/a/a.def", "40", "a macro argument comes from the calling file");
        Origin(r, "/game_entity/mass", 0, "entity/v/.presets", "10", "a value fixed in a macro body comes from the defining file");

        var half = Leaf(r, "/game_entity/half", 0);
        T.Eq("2.5", half.Text, "arithmetic is evaluated");
        T.Eq(true, half.Synthetic, "a computed value is synthetic");
        var name = Leaf(r, "/game_entity/name", 0);
        T.Eq("\"x_1.5\"", name.Text, "a partial substitution is expanded");
        T.Eq(true, name.Synthetic, "a partially substituted value is synthetic");

        var edges = r.IncludeEdges.Select(e => e.FromFile + " -> " + e.ToFile + " " + e.Raw).ToArray();
        T.Eq(new[]
        {
            "entity/v/a/a.def -> entity/v/a/a.inc \"a.inc\"",
            "entity/v/a/a.inc -> entity/v/a/deep.inc \"deep.inc\"",
            "entity/v/a/a.def -> properties/tank.ext \"/properties/tank.ext\"",
        }, edges, "include edges in expansion order, with the raw path token");
        foreach (var e in r.IncludeEdges)
            T.Eq(e.Raw, Files[e.FromFile].Substring(e.Index, e.Raw.Length), "an edge's Index points at its path token: " + e.Raw);

        // 폴더에서 읽는 기존 생성자도 같은 결과를 낸다
        string dir = T.TempDir();
        foreach (var f in Files) T.Put(dir, f.Key, f.Value);
        var fromDisk = new SdlResolver(new[] { dir }).Resolve("entity/v/a/a.def");
        T.Eq(SdlCore.NodesText(r.Nodes), SdlCore.NodesText(fromDisk.Nodes), "the folder-based constructor expands to the same text");
        T.Eq("entity/v/a/a.inc", Leaf(fromDisk, "/game_entity/box/item", 1).File, "the folder-based constructor stamps files too");
    }
}
```

`editor/CtaToybox.Core.Tests/Program.cs`의 suites 목록에서 `("KeyLocator", KeyLocatorTests.Run),` 다음 줄에 더한다:

```csharp
            ("ResolverOrigin", ResolverOriginTests.Run),
```

- [ ] **Step 3: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `SdlResolver`에 `Func<string, string>`을 받는 생성자가 없고 `SdlNode`에 `File`이 없다.

- [ ] **Step 4: `SdlNode`에 속성을 더한다**

`editor/CtaToybox.Core/Sdl/Sdl.cs`의 `SdlNode`에서 `public List<object> Children { get; set; }   // 잎은 null` 다음 줄에 더한다:

```csharp
    public string File { get; set; }       // 이 토큰이 있는 파일의 가상 경로. 리졸버가 읽은 트리에만 채워진다
    public bool Synthetic { get; set; }    // 원본 토큰 그대로가 아니다: %치환이나 산술 계산으로 만든 값
```

- [ ] **Step 5: 리졸버를 고친다**

`editor/CtaToybox.Core/Sdl/SdlResolve.cs`에서 아래를 차례로 고친다.

(a) `SdlResolveResult` 위에 클래스를 더하고, `SdlResolveResult`에 속성을 더한다:

```csharp
// include 지시문 하나. Index와 Raw는 FromFile 안에 있는 경로 문자열 토큰의 자리와 원문(따옴표 포함)이다
public sealed class SdlIncludeEdge
{
    public string FromFile { get; set; }
    public int Index { get; set; }
    public string Raw { get; set; }
    public string ToFile { get; set; }
}
```

```csharp
    public List<SdlIncludeEdge> IncludeEdges { get; set; }
```

(b) 필드와 생성자. `readonly string[] workDirs;   // 앞에 있는 폴더가 우선한다` 다음에 `readonly Func<string, string> reader;`를 더하고, `List<string> unresolved, includes, macros, inherited;` 다음 줄에 `List<SdlIncludeEdge> edges;`를 더한다. 생성자를 아래 둘로 바꾼다:

```csharp
    public SdlResolver(string[] workDirs) { this.workDirs = workDirs; reader = ReadFromDirs; }

    // 가상 경로를 주면 텍스트를, 없으면 null을 돌려주는 함수로 읽는다 (편집기의 원본 계층)
    public SdlResolver(Func<string, string> readText) { workDirs = new string[0]; reader = readText; }

    string ReadFromDirs(string virtualPath)
    {
        string full = Find(virtualPath);
        return full == null ? null : ReadText(full);
    }

    static void Stamp(SdlNode n, string file)
    {
        n.File = file;
        if (n.Children != null) foreach (object c in n.Children) Stamp(SdlCore.Unwrap(c), file);
    }
```

(c) `Load`의 본문에서 파일을 읽는 부분을 바꾼다. 원래:

```csharp
        string full = Find(virtualPath);
        if (full == null) p = null;
        else
        {
            var r = SdlCore.Parse(ReadText(full));
            p = new Parsed { Root = r.Root, HasErrors = r.Errors.Count > 0 };
        }
```

바꾼 것:

```csharp
        string text = reader(virtualPath);
        if (text == null) p = null;
        else
        {
            var r = SdlCore.Parse(text);
            Stamp(r.Root, virtualPath);
            p = new Parsed { Root = r.Root, HasErrors = r.Errors.Count > 0 };
        }
```

(d) `Expand`에서 새 노드를 만드는 다섯 곳에 출처를 넘긴다.

```csharp
                        o.Add(s == n.Text ? n : new SdlNode("atom", s, n.Index, false) { File = n.File, Synthetic = true });
```

```csharp
                    o.Add(text == n.Text ? n : new SdlNode("str", text, n.Index, false) { File = n.File, Synthetic = true });
```

```csharp
                var c = new SdlNode(n.Kind, n.Text, n.Index, true) { File = n.File };
```

```csharp
            var p = new SdlNode("paren", "", n.Index, true) { File = n.File };
```

```csharp
                if (v.HasValue) { o.Add(new SdlNode("atom", FormatNumber(v.Value), n.Index, false) { File = n.File, Synthetic = true }); continue; }
```

(e) include 분기에서 `includes.Add(vp);` 다음 줄에 더한다:

```csharp
                var pathNode = SdlCore.Unwrap(kids[1]);
                edges.Add(new SdlIncludeEdge { FromFile = n.File, Index = pathNode.Index, Raw = pathNode.Text, ToFile = vp });
```

(f) `Resolve`에서 목록을 만드는 줄에 `edges = new List<SdlIncludeEdge>();`를 더하고, `.presets` 기록을 비우는 줄을 `includes.Clear(); macros.Clear(); inherited.Clear(); edges.Clear();`로 바꾸고, 반환 객체에 `IncludeEdges = edges`를 더한다:

```csharp
        return new SdlResolveResult
        {
            Nodes = nodes, Unresolved = SortedDistinct(unresolved), Includes = SortedDistinct(includes),
            Macros = SortedDistinct(macros), Presets = presets, Inherited = SortedDistinct(inherited), IncludeEdges = edges
        };
```

파일 맨 위의 설명 주석 끝에 한 줄을 더한다:

```csharp
// 출처: 읽은 노드마다 파일의 가상 경로(SdlNode.File)를 적는다. 치환·계산으로 만든 노드는 Synthetic이다.
//       include 지시문의 자리는 IncludeEdges에 적는다. .presets를 읽으며 지나간 include는 적지 않는다.
```

- [ ] **Step 6: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== ResolverOrigin ok`, 마지막 줄 `failed=0`.

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 모든 묶음 통과. PowerShell 도구는 `SdlResolver(string[])`를 그대로 쓴다.

- [ ] **Step 7: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core/Sdl editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): resolver records the origin file of each node and include sites

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 2: 유닛 색인

**Files:**
- Modify: `editor/CtaToybox.Core/SourceStack.cs`
- Create: `editor/CtaToybox.Core/UnitKind.cs`
- Create: `editor/CtaToybox.Core/UnitIndex.cs`
- Create: `editor/CtaToybox.Core/DisplayNames.cs`
- Create: `editor/CtaToybox.Core.Tests/UnitIndexTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`
- Modify: `.gitignore`

**Interfaces:**
- Consumes: `SdlResolver(Func<string, string>)`와 `SdlResolveResult.Includes`, `.Presets`, `.Unresolved` (Task 1). `SourceStack.Paths`, `.Versions`, `.Find`, `.Read`, `.Layers`. `TextCodec.Decode`.
- Produces:
  - `string SourceStack.ReadText(string virtualPath)`: 없으면 `null`.
  - `static string UnitKind.Of(string virtualPath)`: `"vehicle" | "breed" | "humanskin" | "item-entity" | "stuff" | "pattern"`, 유닛이 아니면 `null`. `static readonly string[] UnitKind.All`.
  - `UnitEntry { string Path; string Kind; string Layer; List<string> Includes; List<string> Presets; int Unresolved; }`
  - `UnitIndex { string Signature; List<UnitEntry> Units; }`와
    - `static UnitIndex Build(SourceStack stack, Action<int, int> progress)`
    - `static UnitIndex LoadOrBuild(SourceStack stack, string cachePath, Action<int, int> progress, out bool rebuilt)`
    - `static string SignatureOf(SourceStack stack)`
    - `void Save(string path)`
    - `IReadOnlyList<string> UsersOf(string file)`: 그 파일을 읽는 유닛(자기 자신, include, `.presets`)의 경로, 정렬됨.
    - `IEnumerable<UnitEntry> Search(string text, string kind, string layer, DisplayNames names)`: 인자가 `null`이면 그 조건은 보지 않는다.
  - `DisplayNames`: `static DisplayNames Load(string csvPath)`(파일이 없으면 빈 것), `string OfUnit(string unitPath)`(없으면 `null`), `static List<string[]> ParseCsv(string text)`.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/UnitIndexTests.cs`:

```csharp
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class UnitIndexTests
{
    public static void Run()
    {
        T.Eq("vehicle", UnitKind.Of("entity/-vehicle/ussr/tank_heavy/kv85/kv85.def"), "a vehicle def");
        T.True(UnitKind.Of("entity/-vehicle/ussr/tank_heavy/kv85/kv85.ext") == null, "a vehicle .ext is not a unit");
        T.Eq("breed", UnitKind.Of("set/breed/mp/ger/early/nco.set"), "a breed");
        T.Eq("humanskin", UnitKind.Of("entity/humanskin/ger/x/x.def"), "a humanskin def");
        T.Eq("item-entity", UnitKind.Of("entity/inventory/helmet/h.def"), "an inventory entity");
        T.Eq("stuff", UnitKind.Of("set/stuff/gun/85mm_d5t"), "a stuff file without extension");
        T.Eq("stuff", UnitKind.Of("set/stuff/shell/bulletger_105.heat.ammo"), "a stuff file with an extension");
        T.Eq("pattern", UnitKind.Of("set/stuff/gun/gun.pattern"), "a pattern");
        T.True(UnitKind.Of("set/stuff/gun/.presets") == null, "a .presets file is not a unit");
        T.True(UnitKind.Of("set/stuff/gun/common.inc") == null, "a stuff .inc is not a unit");
        T.True(UnitKind.Of("set/stuff/_settings/x.set") == null, "the _settings folder is not units");
        T.True(UnitKind.Of("set/multiplayer/units/late/units_rus.set") == null, "other set files are not units");

        string vanilla = T.TempDir(), mod = T.TempDir(), work = T.TempDir();
        T.Zip(Path.Combine(vanilla, "entity", "-vehicle.pak"),
            ("-vehicle/x/.presets", "(define \"m\" {k 1})"),
            ("-vehicle/x/a/a.def", "{game_entity (include \"s.inc\") (include \"/properties/tank.ext\")}"),
            ("-vehicle/x/a/b.def", "{game_entity (include \"s.inc\")}"),
            ("-vehicle/x/a/s.inc", "{box {item \"mine\" 24}}"),
            ("-vehicle/x/c/c.def", "{game_entity (include \"missing.inc\")}"));
        T.Zip(Path.Combine(vanilla, "properties.pak"), ("properties/tank.ext", "{armor 30}"));
        T.Put(mod, "entity/-vehicle/x/a/b.def", "{game_entity {mass 1}}");
        T.Put(mod, "set/breed/mp/n.set", "{breed}");

        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("modX", mod) });
        stack.Scan();
        T.Eq("{armor 30}", stack.ReadText("properties/tank.ext"), "ReadText decodes the winning file");
        T.True(stack.ReadText("properties/none.ext") == null, "ReadText of a missing file is null");

        var index = UnitIndex.Build(stack, null);
        T.Eq(new[] { "entity/-vehicle/x/a/a.def", "entity/-vehicle/x/a/b.def", "entity/-vehicle/x/c/c.def", "set/breed/mp/n.set" },
            index.Units.Select(u => u.Path).ToArray(), "units only, sorted by path");
        var a = index.Units[0];
        T.Eq("vehicle", a.Kind, "kind");
        T.Eq("vanilla", a.Layer, "layer of the winning file");
        T.Eq(new[] { "entity/-vehicle/x/a/s.inc", "properties/tank.ext" }, a.Includes.ToArray(), "includes");
        T.Eq(new[] { "entity/-vehicle/x/.presets" }, a.Presets.ToArray(), "presets of ancestor folders");
        T.Eq("modX", index.Units[1].Layer, "an overridden unit reports the overriding layer");
        T.Eq(0, index.Units[1].Includes.Count, "the overriding file's includes are used, not the original's");
        T.Eq(1, index.Units[2].Unresolved, "unresolved includes are counted");

        T.Eq(new[] { "entity/-vehicle/x/a/a.def" }, index.UsersOf("entity/-vehicle/x/a/s.inc").ToArray(), "users of an include: b.def was overridden and no longer includes it");
        T.Eq(3, index.UsersOf("entity/-vehicle/x/.presets").Count, "users of a .presets file");
        T.Eq(new[] { "entity/-vehicle/x/a/a.def" }, index.UsersOf("ENTITY/-vehicle/x/a/a.def").ToArray(), "a unit uses itself; lookup ignores case");
        T.Eq(0, index.UsersOf("properties/unused.ext").Count, "a file nobody reads has no users");

        string csv = Path.Combine(work, "names.csv");
        File.WriteAllText(csv, "\"Context\",\"Text\",\"File\"\r\n\"desc/entity/a\",\"Tank \"\"A\"\"\",\"x.pot\"\r\n\"info/entity/a\",\"line one\nline two\",\"x.pot\"\r\n");
        var names = DisplayNames.Load(csv);
        T.Eq("Tank \"A\"", names.OfUnit("entity/-vehicle/x/a/a.def"), "an entity's name comes from desc/entity/<file name>");
        T.True(names.OfUnit("entity/-vehicle/x/a/b.def") == null, "no key, no name");
        T.True(names.OfUnit("set/breed/mp/n.set") == null, "breeds get no name");
        T.True(DisplayNames.Load(Path.Combine(work, "none.csv")).OfUnit("entity/-vehicle/x/a/a.def") == null, "a missing csv is empty");
        T.Eq(3, DisplayNames.ParseCsv("a,b\r\n\"c\nd\",e\r\nf,\"g\"\"h\"").Count, "csv rows: quoted newlines stay in one row");

        T.Eq(4, index.Search(null, null, null, null).Count(), "no criteria returns everything");
        T.Eq(new[] { "entity/-vehicle/x/c/c.def" }, index.Search("C.DEF", null, null, null).Select(u => u.Path).ToArray(), "text matches the path, ignoring case");
        T.Eq(new[] { "entity/-vehicle/x/a/a.def" }, index.Search("tank", null, null, names).Select(u => u.Path).ToArray(), "text matches the display name");
        T.Eq(new[] { "set/breed/mp/n.set" }, index.Search(null, "breed", null, null).Select(u => u.Path).ToArray(), "kind filter");
        T.Eq(2, index.Search(null, null, "modX", null).Count(), "layer filter");

        string cache = Path.Combine(work, "cache", "unit-index.json");
        bool rebuilt;
        UnitIndex.LoadOrBuild(stack, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "the first call builds");
        T.True(File.Exists(cache), "and writes the cache");
        var again = UnitIndex.LoadOrBuild(stack, cache, null, out rebuilt);
        T.Eq(false, rebuilt, "the second call loads the cache");
        T.Eq(4, again.Units.Count, "the cached index has the same units");
        T.Eq(1, again.UsersOf("entity/-vehicle/x/a/s.inc").Count, "the cached index answers UsersOf");

        T.Put(mod, "set/breed/mp/n.set", "{breed {changed 1}}");
        using var stack2 = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("modX", mod) });
        stack2.Scan();
        UnitIndex.LoadOrBuild(stack2, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "a changed source file invalidates the cache");
        using var stack3 = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack3.Scan();
        UnitIndex.LoadOrBuild(stack3, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "a different set of layers invalidates the cache");
        File.WriteAllText(cache, "{ not json");
        UnitIndex.LoadOrBuild(stack3, cache, null, out rebuilt);
        T.Eq(true, rebuilt, "a corrupt cache is rebuilt, not fatal");
    }
}
```

`Program.cs`의 suites 목록에서 `("ResolverOrigin", ResolverOriginTests.Run),` 다음 줄에 더한다:

```csharp
            ("UnitIndex", UnitIndexTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `UnitKind`, `UnitIndex`, `DisplayNames`가 없다.

- [ ] **Step 3: `SourceStack.ReadText`를 더한다**

`editor/CtaToybox.Core/SourceStack.cs`의 `Read` 메서드 다음에 더한다:

```csharp
        // 지금 게임이 읽을 그 파일의 텍스트. 없으면 null
        public string ReadText(string virtualPath)
        {
            var file = Find(virtualPath);
            if (file == null) return null;
            TextKind kind;
            return TextCodec.Decode(Read(file), out kind);
        }
```

- [ ] **Step 4: `UnitKind.cs`를 쓴다**

```csharp
// 가상 경로로 유닛의 종류를 정한다. tools/lib/Common.ps1의 Get-SdlUnitKind와 같은 규칙이어야 한다.
using System;

namespace CtaToybox.Core
{
    public static class UnitKind
    {
        public static readonly string[] All = { "vehicle", "breed", "humanskin", "item-entity", "stuff", "pattern" };

        // 점으로 시작하는 파일은 이름 전체, 확장자가 없으면 "(no-ext)", 그 밖에는 마지막 확장자
        static string FileKind(string virtualPath)
        {
            string leaf = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1);
            if (leaf.StartsWith(".")) return leaf;
            int dot = leaf.LastIndexOf('.');
            return dot < 0 ? "(no-ext)" : leaf.Substring(dot);
        }

        // 유닛이 아니면 null
        public static string Of(string virtualPath)
        {
            string kind = FileKind(virtualPath);
            if (virtualPath.StartsWith("set/breed/", StringComparison.Ordinal) && kind == ".set") return "breed";
            if (virtualPath.StartsWith("entity/humanskin/", StringComparison.Ordinal) && kind == ".def") return "humanskin";
            if (virtualPath.StartsWith("entity/-vehicle/", StringComparison.Ordinal) && kind == ".def") return "vehicle";
            if (virtualPath.StartsWith("entity/inventory/", StringComparison.Ordinal) && kind == ".def") return "item-entity";
            if (virtualPath.StartsWith("set/stuff/", StringComparison.Ordinal))
            {
                if (kind == ".pattern") return "pattern";
                // .fsm, .hand와 _settings 폴더는 아이템이 아니라 동작 설정이다
                bool support = kind == ".presets" || kind == ".inc" || kind == ".fsm" || kind == ".hand";
                if (!support && !virtualPath.StartsWith("set/stuff/_settings/", StringComparison.Ordinal)) return "stuff";
            }
            return null;
        }
    }
}
```

- [ ] **Step 5: `DisplayNames.cs`를 쓴다**

```csharp
// data/display-names.csv에서 엔티티의 표시 이름을 찾는다.
// 확인된 키 규칙은 desc/entity/<파일 이름> 하나다(예: desc/entity/kv85 → KV-85). 보병과 아이템의 규칙은 확인하지 않아 붙이지 않는다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class DisplayNames
    {
        readonly Dictionary<string, string> byContext = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static DisplayNames Load(string csvPath)
        {
            var names = new DisplayNames();
            if (!File.Exists(csvPath)) return names;
            var rows = ParseCsv(File.ReadAllText(csvPath, Encoding.UTF8));
            for (int i = 1; i < rows.Count; i++)
                if (rows[i].Length >= 2 && !names.byContext.ContainsKey(rows[i][0])) names.byContext[rows[i][0]] = rows[i][1];
            return names;
        }

        public string OfUnit(string unitPath)
        {
            if (!unitPath.StartsWith("entity/", StringComparison.Ordinal)) return null;
            string leaf = unitPath.Substring(unitPath.LastIndexOf('/') + 1);
            int dot = leaf.LastIndexOf('.');
            string name;
            return byContext.TryGetValue("desc/entity/" + (dot <= 0 ? leaf : leaf.Substring(0, dot)), out name) ? name : null;
        }

        // 따옴표 필드 안의 쉼표, 줄바꿈, ""를 처리한다
        public static List<string[]> ParseCsv(string text)
        {
            var rows = new List<string[]>();
            var row = new List<string>();
            var field = new StringBuilder();
            bool quoted = false, any = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (quoted)
                {
                    if (c == '"' && i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                    else if (c == '"') quoted = false;
                    else field.Append(c);
                    continue;
                }
                if (c == '"') { quoted = true; any = true; }
                else if (c == ',') { row.Add(field.ToString()); field.Clear(); any = true; }
                else if (c == '\n' || c == '\r')
                {
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                    if (any || field.Length > 0) { row.Add(field.ToString()); rows.Add(row.ToArray()); }
                    row.Clear(); field.Clear(); any = false;
                }
                else field.Append(c);
            }
            if (any || field.Length > 0) { row.Add(field.ToString()); rows.Add(row.ToArray()); }
            return rows;
        }
    }
}
```

- [ ] **Step 6: `UnitIndex.cs`를 쓴다**

```csharp
// 유닛 목록과 유닛마다 읽는 파일(include와 .presets). 값이 몇 개 유닛에 공유되는지 세는 데 쓴다.
// 유닛 전체를 펼치는 데 1분 넘게 걸리므로 결과를 파일로 캐시한다. 원본 구성이 바뀌면 다시 만든다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class UnitEntry
    {
        public string Path { get; set; }
        public string Kind { get; set; }
        public string Layer { get; set; }
        public List<string> Includes { get; set; } = new List<string>();
        public List<string> Presets { get; set; } = new List<string>();
        public int Unresolved { get; set; }
    }

    public sealed class UnitIndex
    {
        public string Signature { get; set; }
        public List<UnitEntry> Units { get; set; } = new List<UnitEntry>();

        static readonly IReadOnlyList<string> None = new string[0];
        Dictionary<string, List<string>> users;

        public static UnitIndex Build(SourceStack stack, Action<int, int> progress)
        {
            var paths = stack.Paths.Where(p => UnitKind.Of(p) != null).OrderBy(p => p, StringComparer.Ordinal).ToList();
            var resolver = new SdlResolver(stack.ReadText);
            var index = new UnitIndex { Signature = SignatureOf(stack) };
            for (int i = 0; i < paths.Count; i++)
            {
                var r = resolver.Resolve(paths[i]);
                index.Units.Add(new UnitEntry
                {
                    Path = paths[i], Kind = UnitKind.Of(paths[i]), Layer = stack.Find(paths[i]).Layer,
                    Includes = r.Includes, Presets = r.Presets, Unresolved = r.Unresolved.Count
                });
                if (progress != null && (i + 1) % 1000 == 0) progress(i + 1, paths.Count);
            }
            return index;
        }

        // 계층의 구성과 원본 파일(pak, 낱개 파일)의 크기·수정 시각으로 만든 지문
        public static string SignatureOf(SourceStack stack)
        {
            var containers = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string p in stack.Paths)
                foreach (var v in stack.Versions(p)) containers.Add(v.Container);
            var sb = new StringBuilder();
            foreach (var l in stack.Layers) sb.Append(l.Name).Append('|').Append(l.ResourceDir).Append('\n');
            foreach (string c in containers)
            {
                var info = new FileInfo(c);
                sb.Append(c).Append('|').Append(info.Length).Append('|').Append(info.LastWriteTimeUtc.Ticks).Append('\n');
            }
            using (var sha = SHA1.Create()) return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())));
        }

        public static UnitIndex LoadOrBuild(SourceStack stack, string cachePath, Action<int, int> progress, out bool rebuilt)
        {
            string signature = SignatureOf(stack);
            if (File.Exists(cachePath))
            {
                try
                {
                    var cached = JsonSerializer.Deserialize<UnitIndex>(File.ReadAllText(cachePath, Encoding.UTF8));
                    if (cached != null && cached.Units != null && cached.Signature == signature) { rebuilt = false; return cached; }
                }
                catch (JsonException) { }   // 깨진 캐시는 다시 만든다
            }
            var built = Build(stack, progress);
            built.Save(cachePath);
            rebuilt = true;
            return built;
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path)));
            File.WriteAllText(path, JsonSerializer.Serialize(this), new UTF8Encoding(false));
        }

        // 그 파일을 읽는 유닛: 자기 자신, include, .presets
        public IReadOnlyList<string> UsersOf(string file)
        {
            if (users == null)
            {
                var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);
                foreach (var u in Units)
                    foreach (string f in new[] { u.Path }.Concat(u.Includes).Concat(u.Presets).Distinct())
                    {
                        List<string> list;
                        if (!map.TryGetValue(f, out list)) { list = new List<string>(); map[f] = list; }
                        list.Add(u.Path);
                    }
                users = map;
            }
            List<string> found;
            return users.TryGetValue(SourceStack.ToVirtual(file), out found) ? found : None;
        }

        public IEnumerable<UnitEntry> Search(string text, string kind, string layer, DisplayNames names)
        {
            foreach (var u in Units)
            {
                if (kind != null && !string.Equals(u.Kind, kind, StringComparison.OrdinalIgnoreCase)) continue;
                if (layer != null && !string.Equals(u.Layer, layer, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrEmpty(text))
                {
                    string name = names == null ? null : names.OfUnit(u.Path);
                    bool hit = u.Path.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                        || (name != null && name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!hit) continue;
                }
                yield return u;
            }
        }
    }
}
```

`Units`는 경로순으로 만들어지므로 `UsersOf`의 결과도 경로순이다.

- [ ] **Step 7: 캐시 폴더를 git에서 뺀다**

`.gitignore`의 `editor/settings.json` 다음 줄에 더한다:

```
editor/cache/
```

- [ ] **Step 8: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== UnitIndex ok`, 마지막 줄 `failed=0`.

- [ ] **Step 9: 커밋한다**

```bash
git -C E:/CTAToybox add .gitignore editor/CtaToybox.Core editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): unit index with file users, display names and a cache

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 3: include 사슬

**Files:**
- Create: `editor/CtaToybox.Core/IncludeChain.cs`
- Create: `editor/CtaToybox.Core.Tests/IncludeChainTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `SdlIncludeEdge` (Task 1).
- Produces (`public static class IncludeChain`, 네임스페이스 `CtaToybox.Core`):
  - `HashSet<string> Reachable(IEnumerable<SdlIncludeEdge> edges, string from)`: `from`에서 include를 따라 닿는 파일(`from` 자신은 순환일 때만 들어간다).
  - `HashSet<string> CopySet(IEnumerable<SdlIncludeEdge> edges, string unit, string target)`: `unit`에서 닿고 `target`에 닿는 파일과 `target`. `unit` 자신은 뺀다. `target`에 닿지 않으면 빈 집합.
  - `string CopyName(string file, string unit)`: 사본의 가상 경로. 원본과 같은 폴더, 같은 확장자.
  - `string RewriteRaw(string raw, string copyFile)`: include 경로 토큰(따옴표 포함)에서 파일 이름 부분만 사본의 이름으로 바꾼 토큰.
  - `string UnitScopeBlocker(IEnumerable<SdlIncludeEdge> edges, string unit, string file, string keyPath)`: "이 유닛만"이 안 되는 이유. 되면 `null`.

사본을 원본과 같은 폴더에 두는 이유: 사본 안의 상대 경로 include(`"deep.inc"`)가 고치지 않아도 같은 파일을 가리킨다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/IncludeChainTests.cs`:

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CtaToybox.Core;

static class IncludeChainTests
{
    static SdlIncludeEdge Edge(string from, string to) { return new SdlIncludeEdge { FromFile = from, ToFile = to, Index = 0, Raw = "\"x\"" }; }

    public static void Run()
    {
        // u → a → f,  u → f (두 경로),  u → g,  a → h,  p(.presets)에서 나온 include → q
        var edges = new List<SdlIncludeEdge>
        {
            Edge("u.def", "d/a.inc"), Edge("d/a.inc", "d/f.inc"), Edge("u.def", "d/f.inc"),
            Edge("u.def", "d/g.inc"), Edge("d/a.inc", "d/h.inc"), Edge("d/.presets", "d/q.inc"),
        };

        T.Eq(new[] { "d/a.inc", "d/f.inc", "d/g.inc", "d/h.inc" }, IncludeChain.Reachable(edges, "u.def").OrderBy(x => x).ToArray(), "reachable from the unit");
        T.Eq(0, IncludeChain.Reachable(edges, "d/g.inc").Count, "a leaf reaches nothing");

        T.Eq(new[] { "d/a.inc", "d/f.inc" }, IncludeChain.CopySet(edges, "u.def", "d/f.inc").OrderBy(x => x).ToArray(), "every file on any path to the target, and the target");
        T.Eq(new[] { "d/g.inc" }, IncludeChain.CopySet(edges, "u.def", "d/g.inc").ToArray(), "a directly included target is copied alone");
        T.Eq(new[] { "d/a.inc", "d/h.inc" }, IncludeChain.CopySet(edges, "u.def", "d/h.inc").OrderBy(x => x).ToArray(), "siblings off the path are not copied");
        T.Eq(0, IncludeChain.CopySet(edges, "u.def", "d/q.inc").Count, "a file reached only through .presets is not in the chain");
        T.Eq(0, IncludeChain.CopySet(edges, "u.def", "d/none.inc").Count, "an unknown file is not in the chain");

        var cycle = new List<SdlIncludeEdge> { Edge("u.def", "d/a.inc"), Edge("d/a.inc", "u.def") };
        T.Eq(new[] { "d/a.inc" }, IncludeChain.CopySet(cycle, "u.def", "d/a.inc").ToArray(), "the unit itself is never copied, even in a cycle");

        string copy = IncludeChain.CopyName("entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc", "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def");
        T.True(Regex.IsMatch(copy, @"^entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack__ctb_m3a1_halftrack_[0-9a-f]{8}\.inc$"), "copy name: same folder, same extension, unit name and hash: " + copy);
        T.Eq(copy, IncludeChain.CopyName("entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc", "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def"), "copy names are stable");
        T.True(IncludeChain.CopyName("properties/tank.ext", "entity/a/x.def") != IncludeChain.CopyName("properties/tank.ext", "entity/b/x.def"), "units with the same file name in different folders get different copies");
        T.True(Regex.IsMatch(IncludeChain.CopyName("properties/tank.ext", "set/breed/mp/ger-a b.set"), @"^properties/tank__ctb_ger_a_b_[0-9a-f]{8}\.ext$"), "characters outside a-z, 0-9 and _ become _");
        T.True(Regex.IsMatch(IncludeChain.CopyName("set/stuff/gun/common", "set/stuff/gun/85mm_d5t"), @"^set/stuff/gun/common__ctb_85mm_d5t_[0-9a-f]{8}$"), "files without an extension stay without one");

        T.Eq("\"kv85__ctb_x_0a1b2c3d.ext\"", IncludeChain.RewriteRaw("\"kv85.ext\"", "entity/v/kv85/kv85__ctb_x_0a1b2c3d.ext"), "a same-folder include keeps its form");
        T.Eq("\"/properties/tank__ctb_x_0a1b2c3d.ext\"", IncludeChain.RewriteRaw("\"/properties/tank.ext\"", "properties/tank__ctb_x_0a1b2c3d.ext"), "a root-relative include keeps its folder part");
        T.Eq("\"../base/base__ctb_x_0a1b2c3d.inc\"", IncludeChain.RewriteRaw("\"../base/base.inc\"", "entity/v/base/base__ctb_x_0a1b2c3d.inc"), "a parent-relative include keeps its folder part");
        T.Eq("\"..\\base\\base__ctb_x_0a1b2c3d.inc\"", IncludeChain.RewriteRaw("\"..\\base\\base.inc\"", "entity/v/base/base__ctb_x_0a1b2c3d.inc"), "backslashes are kept");

        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "u.def", "/a/(define \"m\")/k") == null, "the unit's own file is always possible");
        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "d/f.inc", "/box/item") == null, "an included file is possible");
        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "d/f.inc", "/(define \"m\")/k") != null, "a value inside a macro body is not");
        T.True(IncludeChain.UnitScopeBlocker(edges, "u.def", "d/q.inc", "/box/item") != null, "a file outside the include chain is not");
    }
}
```

`Program.cs`의 suites 목록에서 `("UnitIndex", UnitIndexTests.Run),` 다음 줄에 더한다:

```csharp
            ("IncludeChain", IncludeChainTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `IncludeChain`이 없다.

- [ ] **Step 3: `IncludeChain.cs`를 쓴다**

```csharp
// "이 유닛만" 수정에 쓰는 include 사슬 계산.
// 공유 파일을 건드리지 않고 유닛에서 그 파일까지의 사슬만 유닛 전용 사본으로 만든다.
// 사본은 원본과 같은 폴더에 둔다: 사본 안의 상대 경로 include가 그대로 같은 파일을 가리킨다.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CtaToybox.Core
{
    public static class IncludeChain
    {
        // from에서 include를 따라 닿는 파일
        public static HashSet<string> Reachable(IEnumerable<SdlIncludeEdge> edges, string from)
        {
            var next = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var e in edges)
            {
                if (e.FromFile == null) continue;
                List<string> list;
                if (!next.TryGetValue(e.FromFile, out list)) { list = new List<string>(); next[e.FromFile] = list; }
                list.Add(e.ToFile);
            }
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var queue = new Queue<string>();
            queue.Enqueue(from);
            while (queue.Count > 0)
            {
                List<string> outs;
                if (!next.TryGetValue(queue.Dequeue(), out outs)) continue;
                foreach (string t in outs) if (seen.Add(t)) queue.Enqueue(t);
            }
            return seen;
        }

        // 사본으로 만들 파일: unit에서 닿고 target에 닿는 파일과 target. unit 자신은 뺀다
        public static HashSet<string> CopySet(IEnumerable<SdlIncludeEdge> edges, string unit, string target)
        {
            var list = edges.ToList();
            var result = new HashSet<string>(StringComparer.Ordinal);
            var fromUnit = Reachable(list, unit);
            if (!fromUnit.Contains(target)) return result;
            foreach (string f in fromUnit)
                if (f != unit && (f == target || Reachable(list, f).Contains(target))) result.Add(f);
            return result;
        }

        static string Stem(string leaf, out string ext)
        {
            int dot = leaf.LastIndexOf('.');
            ext = dot <= 0 ? "" : leaf.Substring(dot);
            return dot <= 0 ? leaf : leaf.Substring(0, dot);
        }

        // <원본 이름>__ctb_<유닛 파일 이름>_<유닛 경로의 해시 8자><확장자>
        public static string CopyName(string file, string unit)
        {
            int slash = file.LastIndexOf('/');
            string ext, ignored;
            string stem = Stem(file.Substring(slash + 1), out ext);
            string unitStem = Stem(unit.Substring(unit.LastIndexOf('/') + 1), out ignored);
            var clean = new StringBuilder();
            foreach (char c in unitStem) clean.Append((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' ? c : '_');
            string hash;
            using (var sha = SHA1.Create()) hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(unit))).Substring(0, 8).ToLowerInvariant();
            return file.Substring(0, slash + 1) + stem + "__ctb_" + clean + "_" + hash + ext;
        }

        // include 경로 토큰에서 파일 이름 부분만 사본의 이름으로 바꾼다. 폴더 부분과 구분자는 그대로 둔다
        public static string RewriteRaw(string raw, string copyFile)
        {
            string body = raw.Trim('"');
            int cut = Math.Max(body.LastIndexOf('/'), body.LastIndexOf('\\'));
            return "\"" + body.Substring(0, cut + 1) + copyFile.Substring(copyFile.LastIndexOf('/') + 1) + "\"";
        }

        // "이 유닛만"이 안 되는 이유. 되면 null
        public static string UnitScopeBlocker(IEnumerable<SdlIncludeEdge> edges, string unit, string file, string keyPath)
        {
            if (file == unit) return null;
            if (keyPath != null && keyPath.IndexOf("/(define ", StringComparison.OrdinalIgnoreCase) >= 0)
                return "매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다";
            if (CopySet(edges, unit, file).Count == 0)
                return "이 유닛이 include로 읽는 파일이 아니다(.presets이거나 다른 유닛의 파일). '전부'만 된다";
            return null;
        }
    }
}
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== IncludeChain ok`, 마지막 줄 `failed=0`.

- [ ] **Step 5: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core/IncludeChain.cs editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): include chain, copy set and copy naming for unit-scoped edits

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 4: 출처 추적

**Files:**
- Create: `editor/CtaToybox.Core/UnitTracer.cs`
- Create: `editor/CtaToybox.Core.Tests/UnitTracerTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `SdlResolver(Func<string, string>)`, `SdlNode.File`, `SdlNode.Synthetic`, `SdlResolveResult.IncludeEdges` (Task 1). `SourceStack.ReadText` (Task 2). `KeyLocator.Rows`.
- Produces (네임스페이스 `CtaToybox.Core`):
  - `TraceValue { string Text; string File; string KeyPath; int Occurrence; int Arg; bool Synthetic; string Note; }`. `Arg`가 `-1`이면 편집할 수 없는 자리이고 `Note`에 이유가 있다. `File`, `KeyPath`, `Occurrence`, `Arg`는 그대로 `Edit.File`, `Edit.Path`, `Edit.Occurrence`, `Edit.Arg`에 넣을 수 있다.
  - `TraceRow { string Path; int Occurrence; List<TraceValue> Values; }`: 펼친 뒤의 키 경로와 순번.
  - `UnitTraceResult { string Unit; List<TraceRow> Rows; List<string> Unresolved; List<SdlIncludeEdge> IncludeEdges; }`
  - `UnitTracer(SourceStack stack)`, `UnitTraceResult Trace(string unit)`: 유닛 파일이 없으면 `null`.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/UnitTracerTests.cs`:

```csharp
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
```

`Program.cs`의 suites 목록에서 `("IncludeChain", IncludeChainTests.Run),` 다음 줄에 더한다:

```csharp
            ("UnitTracer", UnitTracerTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `UnitTracer`가 없다.

- [ ] **Step 3: `UnitTracer.cs`를 쓴다**

```csharp
// 유닛을 펼친 값마다 그 값이 원본의 어느 파일, 어느 키 경로, 몇 번째 값인지 찾는다.
// 리졸버가 노드에 적어 둔 파일과 글자 위치로 그 파일의 KeyLocator 행을 거꾸로 찾는다.
using System;
using System.Collections.Generic;

namespace CtaToybox.Core
{
    public sealed class TraceValue
    {
        public string Text { get; set; }
        public string File { get; set; }        // 값이 있는 파일
        public string KeyPath { get; set; }     // 그 파일 안의 키 경로 (Edit.Path)
        public int Occurrence { get; set; }     // 그 파일 안에서 같은 경로 가운데 몇 번째인지 (Edit.Occurrence)
        public int Arg { get; set; }            // 그 행의 몇 번째 값인지 (Edit.Arg). -1이면 편집할 수 없는 자리다
        public bool Synthetic { get; set; }
        public string Note { get; set; }        // 편집할 수 없는 이유
    }

    public sealed class TraceRow
    {
        public string Path { get; set; }        // 펼친 뒤의 키 경로
        public int Occurrence { get; set; }     // 펼친 뒤의 순번
        public List<TraceValue> Values { get; } = new List<TraceValue>();
    }

    public sealed class UnitTraceResult
    {
        public string Unit { get; set; }
        public List<TraceRow> Rows { get; } = new List<TraceRow>();
        public List<string> Unresolved { get; set; }
        public List<SdlIncludeEdge> IncludeEdges { get; set; }
    }

    public sealed class UnitTracer
    {
        sealed class Slot { public string Path; public int Occurrence; public int Arg; }

        readonly SourceStack stack;
        // 파일 → (토큰의 글자 위치 → 자리). 파싱되지 않는 파일은 빈 표다
        readonly Dictionary<string, Dictionary<int, Slot>> slots = new Dictionary<string, Dictionary<int, Slot>>(StringComparer.Ordinal);

        public UnitTracer(SourceStack stack) { this.stack = stack; }

        Dictionary<int, Slot> SlotsOf(string file)
        {
            Dictionary<int, Slot> map;
            if (slots.TryGetValue(file, out map)) return map;
            map = new Dictionary<int, Slot>();
            string text = stack.ReadText(file);
            if (text != null)
            {
                var parsed = SdlCore.Parse(text);
                if (parsed.Errors.Count == 0)
                    foreach (var row in KeyLocator.Rows(parsed.Root))
                        for (int i = 0; i < row.Leaves.Count; i++)
                            map[row.Leaves[i].Index] = new Slot { Path = row.Path, Occurrence = row.Occurrence, Arg = i };
            }
            slots[file] = map;
            return map;
        }

        TraceValue ValueOf(SdlNode leaf)
        {
            var v = new TraceValue { Text = leaf.Text, File = leaf.File, Arg = -1, Synthetic = leaf.Synthetic };
            if (leaf.Synthetic) { v.Note = "치환이나 계산으로 만든 값이다. 원본의 인자나 피연산자를 고친다"; return v; }
            if (leaf.File == null) { v.Note = "출처를 알 수 없다"; return v; }
            Slot slot;
            if (!SlotsOf(leaf.File).TryGetValue(leaf.Index, out slot)) { v.Note = "편집할 수 있는 자리가 아니다(include 인자이거나 파싱되지 않는 파일)"; return v; }
            v.KeyPath = slot.Path; v.Occurrence = slot.Occurrence; v.Arg = slot.Arg;
            return v;
        }

        // 유닛 파일이 없으면 null
        public UnitTraceResult Trace(string unit)
        {
            unit = SourceStack.ToVirtual(unit);
            if (stack.Find(unit) == null) return null;
            var resolved = new SdlResolver(stack.ReadText).Resolve(unit);
            var root = new SdlNode("root", "", 0, true);
            root.Children.AddRange(resolved.Nodes);
            var result = new UnitTraceResult { Unit = unit, Unresolved = resolved.Unresolved, IncludeEdges = resolved.IncludeEdges };
            foreach (var row in KeyLocator.Rows(root))
            {
                if (row.Leaves.Count == 0) continue;
                var traced = new TraceRow { Path = row.Path, Occurrence = row.Occurrence };
                foreach (var leaf in row.Leaves) traced.Values.Add(ValueOf(leaf));
                result.Rows.Add(traced);
            }
            return result;
        }
    }
}
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== UnitTracer ok`, 마지막 줄 `failed=0`.

테스트의 `bad.inc`(`{broken 1`)는 닫히지 않은 블록이다. 리졸버는 읽을 수 있는 데까지 쓰므로 `/game_entity/broken` 행이 나온다. 이 행이 나오지 않아 테스트가 `First`에서 던지면 추측으로 고치지 말고 `NEEDS_CONTEXT`로 보고한다.

- [ ] **Step 5: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core/UnitTracer.cs editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): trace each expanded value of a unit back to its file slot

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 5: 빌더가 `scope: unit`을 적용한다

**Files:**
- Modify: `editor/CtaToybox.Core/TokenPatcher.cs`
- Modify: `editor/CtaToybox.Core/ModBuilder.cs`
- Modify: `editor/CtaToybox.Core.Tests/TokenPatcherTests.cs:68-70`
- Create: `editor/CtaToybox.Core.Tests/UnitScopeBuildTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `IncludeChain.CopySet`, `.CopyName`, `.RewriteRaw`, `.UnitScopeBlocker` (Task 3). `SdlResolveResult.IncludeEdges` (Task 1). `SourceStack.ReadText` (Task 2).
- Produces:
  - `public sealed class RawPatch { public int Index; public string Old; public string New; }` (`TokenPatcher.cs`, 네임스페이스 `CtaToybox.Core`).
  - `static string TokenPatcher.Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results, IReadOnlyList<RawPatch> raw)`. 기존 3인자 `Apply`는 `raw`를 `null`로 넘긴다. `raw`의 자리가 원본과 다르거나 원본이 파싱되지 않으면 `InvalidOperationException`.
  - `TokenPatcher.Apply`는 더는 `Scope`를 보지 않는다. 범위는 `ModBuilder`가 처리한다.
  - `ModBuilder.Build(EditProject)`의 서명과 `BuildReport`는 그대로다. `scope: unit` 수정의 동작:
    - `Edit.File`이 `Edit.Unit`과 같으면 그 파일을 그대로 고친다.
    - 다르면 `IncludeChain.CopySet`의 파일마다 사본을 만들고, 유닛 파일과 사본 안의 include 경로를 사본으로 고쳐 쓴다.
    - 사본에는 같은 파일에 걸린 `shared` 수정도 들어간다. 같은 자리면 `unit` 수정이 이긴다.
    - `UnitScopeBlocker`가 이유를 돌려주면 `EditStatus.Invalid`와 그 이유를 보고한다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/UnitScopeBuildTests.cs`:

```csharp
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CtaToybox.Core;

static class UnitScopeBuildTests
{
    const string A = "entity/v/a/a.def", B = "entity/v/a/b.def", S = "entity/v/a/shared.inc", D = "properties/deep.ext";

    static Edit U(string unit, string file, string path, int arg, string old, string @new)
    {
        return new Edit { Unit = unit, File = file, Path = path, Occurrence = 0, Arg = arg, Old = old, New = @new, Scope = "unit" };
    }

    static Edit Sh(string file, string path, int occ, int arg, string old, string @new)
    {
        return new Edit { Unit = file, File = file, Path = path, Occurrence = occ, Arg = arg, Old = old, New = @new };
    }

    static string Vanilla()
    {
        string dir = T.TempDir();
        T.Put(dir, A, "{game_entity\r\n\t(include \"shared.inc\")\r\n\t(include \"shared.inc\")\r\n\t{mass 10}\r\n}\r\n");
        T.Put(dir, B, "{game_entity\r\n\t(include \"shared.inc\")\r\n}\r\n");
        T.Put(dir, S, "{box {item \"mine\" 24} {item \"can\" 1}}\r\n(include \"/properties/deep.ext\")\r\n");
        T.Put(dir, D, "{armor 30}\r\n(define \"m\" {k 7})\r\n");
        T.Put(dir, "properties/other.ext", "{other 1}\r\n");
        return dir;
    }

    // 빌드 결과를 게임처럼 원본 위에 얹어 유닛을 펼친 뒤, 펼친 값 하나를 읽는다
    static string Expanded(string vanilla, string outDir, string unit, string path, int occurrence, int arg)
    {
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla), new SourceLayer("out", Path.Combine(outDir, "resource")) });
        stack.Scan();
        var t = new UnitTracer(stack).Trace(unit);
        return t.Rows.First(r => r.Path == path && r.Occurrence == occurrence).Values[arg].Text;
    }

    static List<string> Outputs(string outDir)
    {
        string root = Path.Combine(outDir, "resource");
        if (!Directory.Exists(root)) return new List<string>();
        return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(root, f).Replace('\\', '/')).OrderBy(x => x, System.StringComparer.Ordinal).ToList();
    }

    static BuildReport Build(string vanilla, string outDir, params Edit[] edits)
    {
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", vanilla) });
        stack.Scan();
        var project = new EditProject { Name = "t" };
        project.Edits.AddRange(edits);
        return new ModBuilder(stack, outDir).Build(project);
    }

    public static void Run()
    {
        string sCopy = IncludeChain.CopyName(S, A), dCopy = IncludeChain.CopyName(D, A);

        // 1. include로 공유된 값을 이 유닛만 고친다
        string vanilla = Vanilla(), outDir = T.TempDir();
        var report = Build(vanilla, outDir, U(A, S, "/box/item", 1, "24", "5"));
        T.Eq(1, report.Count(EditStatus.Applied), "the unit-scoped edit is applied");
        T.Eq(new[] { A, sCopy }.OrderBy(x => x, System.StringComparer.Ordinal).ToArray(), Outputs(outDir).ToArray(), "outputs: the unit file and one copy; the shared file and the other unit are not written");
        T.Eq("5", Expanded(vanilla, outDir, A, "/game_entity/box/item", 0, 1), "the unit sees the new value");
        T.Eq("5", Expanded(vanilla, outDir, A, "/game_entity/box/item", 2, 1), "through every include of that file (it is included twice)");
        T.Eq("24", Expanded(vanilla, outDir, B, "/game_entity/box/item", 0, 1), "the other unit keeps the original value");
        string aText = Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", A.Replace('/', Path.DirectorySeparatorChar))));
        string leaf = sCopy.Substring(sCopy.LastIndexOf('/') + 1);
        T.Eq("{game_entity\r\n\t(include \"" + leaf + "\")\r\n\t(include \"" + leaf + "\")\r\n\t{mass 10}\r\n}\r\n", aText, "only the include path tokens changed in the unit file");

        // 2. 수정을 끄고 다시 빌드하면 사본과 유닛 파일이 모두 지워진다
        var off = U(A, S, "/box/item", 1, "24", "5"); off.Enabled = false;
        report = Build(vanilla, outDir, off);
        T.Eq(0, Outputs(outDir).Count, "disabling the edit removes the copy and the rewritten unit file");
        T.Eq(2, report.RemovedFiles.Count, "both are reported as removed");

        // 3. 두 단계 사슬: 유닛 → shared.inc → deep.ext
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir, U(A, D, "/armor", 0, "30", "40"));
        T.Eq(new[] { A, sCopy, dCopy }.OrderBy(x => x, System.StringComparer.Ordinal).ToArray(), Outputs(outDir).ToArray(), "the whole chain is copied");
        T.Eq("40", Expanded(vanilla, outDir, A, "/game_entity/armor", 0, 0), "the unit sees the new value two includes deep");
        T.Eq("30", Expanded(vanilla, outDir, B, "/game_entity/armor", 0, 0), "the other unit does not");
        string sText = Encoding.Latin1.GetString(File.ReadAllBytes(Path.Combine(outDir, "resource", sCopy.Replace('/', Path.DirectorySeparatorChar))));
        T.True(sText.Contains("(include \"/properties/" + dCopy.Substring(dCopy.LastIndexOf('/') + 1) + "\")"), "the intermediate copy includes the deep copy, keeping the root-relative form");

        // 4. 같은 파일에 '전부' 수정과 '이 유닛만' 수정이 함께 걸린다
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir,
            Sh(S, "/box/item", 1, 1, "1", "2"),            // can 1 → 2, 전부
            Sh(S, "/box/item", 0, 1, "24", "30"),          // mine 24 → 30, 전부
            U(A, S, "/box/item", 1, "24", "5"));           // mine 24 → 5, 이 유닛만
        T.Eq(3, report.Count(EditStatus.Applied), "each edit is reported once, as applied");
        T.Eq("5", Expanded(vanilla, outDir, A, "/game_entity/box/item", 0, 1), "on the same slot the unit-scoped edit wins for that unit");
        T.Eq("2", Expanded(vanilla, outDir, A, "/game_entity/box/item", 1, 1), "the unit's copy also carries the shared edit");
        T.Eq("30", Expanded(vanilla, outDir, B, "/game_entity/box/item", 0, 1), "other units get the shared edit");
        T.Eq("2", Expanded(vanilla, outDir, B, "/game_entity/box/item", 1, 1), "both of them");

        // 5. 유닛 자신의 파일에 있는 값은 그 파일을 그대로 고친다
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir, U(A, A, "/game_entity/mass", 0, "10", "12"));
        T.Eq(new[] { A }, Outputs(outDir).ToArray(), "no copy is needed for the unit's own file");
        T.Eq("12", Expanded(vanilla, outDir, A, "/game_entity/mass", 0, 0), "the value changed");

        // 6. 안 되는 경우는 아무것도 쓰지 않고 이유를 보고한다
        vanilla = Vanilla(); outDir = T.TempDir();
        report = Build(vanilla, outDir,
            U(A, D, "/(define \"m\")/k", 0, "7", "8"),             // 매크로 본문
            U(B, "properties/other.ext", "/other", 0, "1", "2"),   // include 사슬 밖
            U(A, S, "/box/item", 1, "99", "5"),                    // 원본이 바뀜
            U("entity/v/a/gone.def", S, "/box/item", 1, "24", "5"),// 유닛이 없음
            U(A, "properties/gone.ext", "/x", 0, "1", "2"));       // 파일이 없음
        T.Eq(0, Outputs(outDir).Count, "nothing is written when no unit-scoped edit applies");
        T.Eq(2, report.Count(EditStatus.Invalid), "macro body and out-of-chain edits are Invalid");
        T.Eq(1, report.Count(EditStatus.SourceChanged), "a stale old value is SourceChanged");
        T.Eq(2, report.Count(EditStatus.TargetMissing), "a missing unit or file is TargetMissing");
        T.True(report.Results.Where(r => r.Status == EditStatus.Invalid).All(r => !string.IsNullOrEmpty(r.Message)), "Invalid results carry the reason");

        // 7. 사본을 만들 수 없으면 유닛 파일의 include도 고쳐 쓰지 않는다
        vanilla = Vanilla(); outDir = T.TempDir();
        T.Put(outDir, "resource/" + sCopy, "hand-made");          // 이 도구가 만들지 않은 같은 이름의 파일
        report = Build(vanilla, outDir, U(A, S, "/box/item", 1, "24", "5"));
        T.Eq(new[] { sCopy }, Outputs(outDir).ToArray(), "the unit file is not rewritten when its copy could not be made");
        T.Eq("hand-made", File.ReadAllText(Path.Combine(outDir, "resource", sCopy.Replace('/', Path.DirectorySeparatorChar))), "the hand-made file is untouched");
        T.Eq(0, report.Count(EditStatus.Applied), "the edit is not reported as applied");
        T.Eq(1, report.FileErrors.Count, "the failure is reported");

        // 8. 유닛 파일을 쓸 수 없으면 사본도 남기지 않는다
        vanilla = Vanilla(); outDir = T.TempDir();
        T.Put(outDir, "resource/" + A, "hand-made");
        report = Build(vanilla, outDir, U(A, S, "/box/item", 1, "24", "5"));
        T.Eq(new[] { A }, Outputs(outDir).ToArray(), "no orphan copy is left when the unit file could not be written");
        T.Eq(0, report.Count(EditStatus.Applied), "the edit is not reported as applied");
    }
}
```

`Program.cs`의 suites 목록에서 `("ModBuilder", ModBuilderTests.Run),` 다음 줄에 더한다:

```csharp
            ("UnitScopeBuild", UnitScopeBuildTests.Run),
```

`editor/CtaToybox.Core.Tests/TokenPatcherTests.cs`의 68~70행(패처가 `unit` 범위를 거부한다는 검사)을 아래로 바꾼다. 범위는 이제 빌더가 처리한다:

```csharp
        var unitScope = E("/game_entity/mass", 0, 0, "46000", "1"); unitScope.Scope = "unit";
        T.True(Apply(text, out r, unitScope) != text && r[0].Status == EditStatus.Applied, "scope is the builder's business; the patcher applies the edit it is given");

        var rawResults = new List<EditResult>();
        string rawText = "{a (include \"x.inc\") {k 1}}";
        var rawPatch = new RawPatch { Index = rawText.IndexOf("\"x.inc\""), Old = "\"x.inc\"", New = "\"x__copy.inc\"" };
        T.Eq("{a (include \"x__copy.inc\") {k 2}}",
            TokenPatcher.Apply(rawText, new[] { new Edit { File = "f", Path = "/a/k", Old = "1", New = "2" } }, rawResults, new[] { rawPatch }),
            "a raw patch and an edit apply together, even though the raw patch shifts the edit's position");
        T.Eq("{a (include \"x__copy.inc\") {k 1}}", TokenPatcher.Apply(rawText, new Edit[0], rawResults, new[] { rawPatch }), "a raw patch applies with no edits");
        T.Throws<InvalidOperationException>(() => TokenPatcher.Apply(rawText, new Edit[0], rawResults, new[] { new RawPatch { Index = 3, Old = "\"x.inc\"", New = "\"y.inc\"" } }), "a raw patch whose Old is not at Index throws");
        T.Throws<InvalidOperationException>(() => TokenPatcher.Apply(rawText, new Edit[0], rawResults, new[] { new RawPatch { Index = rawPatch.Index, Old = rawPatch.Old, New = "\"a\" {b}" } }), "a raw patch whose New is not one string token throws");
        T.Throws<InvalidOperationException>(() => TokenPatcher.Apply("{a (include \"x.inc\")", new Edit[0], rawResults, new[] { new RawPatch { Index = 12, Old = "\"x.inc\"", New = "\"y.inc\"" } }), "a raw patch on a file that does not parse throws");
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `RawPatch`가 없고 `TokenPatcher.Apply`에 4인자 형태가 없다.

- [ ] **Step 3: `TokenPatcher`를 고친다**

`editor/CtaToybox.Core/TokenPatcher.cs`에서 `EditResult` 클래스 다음에 더한다:

```csharp
    // 수정 목록에 없는 치환: include 경로를 사본으로 고쳐 쓰는 데 쓴다. Old와 New는 따옴표를 포함한 토큰 원문이다
    public sealed class RawPatch
    {
        public int Index;
        public string Old;
        public string New;
    }
```

`sealed class Patch`와 `Apply`를 아래로 바꾼다(`CheckNewValue`와 `Result`는 그대로 둔다):

```csharp
        sealed class Patch { public Edit Edit; public int Index; public int Length; public string New; }

        public static string Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results)
        {
            return Apply(text, edits, results, null);
        }

        // raw의 자리가 원본과 다르거나 원본이 파싱되지 않으면 던진다: include 경로를 반만 고친 파일을 쓰면 안 된다
        public static string Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results, IReadOnlyList<RawPatch> raw)
        {
            var active = edits.Where(e => e.Enabled).ToList();
            bool hasRaw = raw != null && raw.Count > 0;
            if (active.Count == 0 && !hasRaw) return text;
            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0)
            {
                foreach (var e in active) results.Add(Result(e, EditStatus.Invalid, "원본 파일이 파싱되지 않는다: " + parsed.Errors[0]));
                if (hasRaw) throw new InvalidOperationException("원본 파일이 파싱되지 않아 include 경로를 고쳐 쓸 수 없다: " + parsed.Errors[0]);
                return text;
            }

            var patches = new List<Patch>();
            var taken = new HashSet<int>();
            if (hasRaw)
            {
                foreach (var r in raw)
                {
                    if (r.Index < 0 || r.Index + r.Old.Length > text.Length || string.CompareOrdinal(text, r.Index, r.Old, 0, r.Old.Length) != 0)
                        throw new InvalidOperationException("include 경로의 자리가 원본과 다르다: " + r.Old);
                    var tokens = SdlCore.Tokenize(r.New);
                    if (tokens.Count != 1 || tokens[0].Kind != "str" || tokens[0].Text != r.New)
                        throw new InvalidOperationException("include 경로는 따옴표로 닫힌 문자열 하나여야 한다: " + r.New);
                    if (!taken.Add(r.Index)) throw new InvalidOperationException("같은 include 경로를 두 번 고쳐 쓰려 했다: " + r.Old);
                    patches.Add(new Patch { Index = r.Index, Length = r.Old.Length, New = r.New });
                }
            }
            foreach (var e in active)
            {
                var node = KeyLocator.Find(parsed.Root, e.Path, e.Occurrence, e.Arg);
                if (node == null) { results.Add(Result(e, EditStatus.TargetMissing, "원본에 이 자리가 없다: " + e.Path + " [" + e.Occurrence + "] 값 " + e.Arg)); continue; }
                if (node.Text != e.Old) { results.Add(Result(e, EditStatus.SourceChanged, "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text)); continue; }
                string warning;
                string error = CheckNewValue(e.Old, e.New, out warning);
                if (error != null) { results.Add(Result(e, EditStatus.Invalid, error)); continue; }
                if (!taken.Add(node.Index)) { results.Add(Result(e, EditStatus.Invalid, "같은 값을 고치는 수정이 앞에 이미 있다")); continue; }
                patches.Add(new Patch { Edit = e, Index = node.Index, Length = node.Text.Length, New = e.New });
                results.Add(Result(e, EditStatus.Applied, warning));
            }
            if (patches.Count == 0) return text;

            var sb = new System.Text.StringBuilder(text);
            foreach (var p in patches.OrderByDescending(x => x.Index))
            {
                sb.Remove(p.Index, p.Length);
                sb.Insert(p.Index, p.New);
            }
            string patched = sb.ToString();

            // 고친 결과를 다시 읽어 구조가 그대로인지, 그 자리에 새 값이 들어갔는지 확인한다
            var check = SdlCore.Parse(patched);
            if (check.Errors.Count > 0 || !SdlCore.RoundTrip(check)) throw new InvalidOperationException("고친 파일이 다시 파싱되지 않는다");
            foreach (var p in patches)
            {
                if (p.Edit == null) continue;
                var node = KeyLocator.Find(check.Root, p.Edit.Path, p.Edit.Occurrence, p.Edit.Arg);
                if (node == null || node.Text != p.Edit.New) throw new InvalidOperationException("고친 뒤 그 자리에 새 값이 없다: " + p.Edit.Path);
            }
            return patched;
        }
```

- [ ] **Step 4: `ModBuilder`를 고친다**

`editor/CtaToybox.Core/ModBuilder.cs`에서 아래를 고친다.

(a) 파일 맨 위 설명 주석 끝에 더한다:

```csharp
// scope가 unit인 수정: 값이 유닛 자신의 파일에 있으면 그 파일을 고친다. include한 파일에 있으면 공유 파일은 두고,
// 유닛에서 그 파일까지의 include 사슬을 유닛 전용 사본으로 만든 뒤 include 경로를 사본으로 고쳐 쓴다(IncludeChain).
// 사본을 하나라도 만들지 못하면 그 유닛의 include는 고쳐 쓰지 않는다: 없는 파일을 include하는 유닛을 내보내지 않는다.
```

(b) `ModBuilder` 클래스 안, `readonly string modDir, resourceDir;` 다음에 더한다:

```csharp
        // 출력 파일 하나를 만드는 데 필요한 것
        sealed class WorkItem
        {
            public string Output;                                      // 출력 가상 경로
            public SourceFile Source;
            public List<Edit> Edits = new List<Edit>();                // 결과를 보고하는 수정
            public List<Edit> Silent = new List<Edit>();               // 사본에 함께 넣는 '전부' 수정. 결과는 원래 파일 쪽에서 보고한다
            public List<RawPatch> Raw = new List<RawPatch>();          // include 경로 고쳐 쓰기
            public bool Always;                                        // 적용된 수정이 없어도 쓴다(사슬 중간의 사본)
        }

        // 한 유닛의 '이 유닛만' 수정이 만드는 사본들과 유닛 파일의 include 고쳐 쓰기
        sealed class UnitPlan
        {
            public string Unit;
            public List<WorkItem> Copies = new List<WorkItem>();
            public List<RawPatch> UnitRaw = new List<RawPatch>();
            public List<BuildManifestFile> Produced = new List<BuildManifestFile>();
            public List<EditResult> Results = new List<EditResult>();
        }
```

(c) `Build`에서 `// 1. 임시 폴더에 고친 파일을 만든다` 주석부터 그 `foreach` 블록의 끝(`report.Results.AddRange(results);` 다음의 `}`)까지를 아래로 바꾼다:

```csharp
                // 1. 임시 폴더에 고친 파일을 만든다
                var enabled = project.Edits.Where(e => e.Enabled).ToList();
                var shared = enabled.Where(e => e.Scope != "unit").ToList();
                var items = new Dictionary<string, WorkItem>(StringComparer.Ordinal);
                foreach (var group in shared.GroupBy(e => SourceStack.ToVirtual(e.File ?? "")))
                {
                    var source = stack.Find(group.Key);
                    if (source == null)
                    {
                        foreach (var e in group) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없다: " + group.Key });
                        continue;
                    }
                    ItemFor(items, group.Key, source).Edits.AddRange(group);
                }

                // 1-1. '이 유닛만' 수정: 사본을 먼저 만든다. 성공한 유닛만 유닛 파일의 include를 고쳐 쓴다
                var plans = new List<UnitPlan>();
                foreach (var group in enabled.Where(e => e.Scope == "unit").GroupBy(e => SourceStack.ToVirtual(e.Unit ?? "")))
                {
                    var plan = PlanUnit(group.Key, group.ToList(), shared, items, report);
                    if (plan == null) continue;
                    string failure = null;
                    foreach (var copy in plan.Copies)
                    {
                        string error;
                        try { error = StageItem(copy, stage, previous, plan.Results, plan.Produced); }
                        catch (Exception ex) { error = ex.Message; }
                        if (error != null) { failure = copy.Output + ": " + error; break; }
                    }
                    if (failure != null)
                    {
                        report.FileErrors.Add(failure);
                        FailPlan(plan, "이 유닛의 사본을 만들지 못했다: " + failure);
                    }
                    else
                    {
                        ItemFor(items, plan.Unit, stack.Find(plan.Unit)).Raw.AddRange(plan.UnitRaw);
                        plans.Add(plan);
                    }
                    report.Results.AddRange(plan.Results);
                }

                // 1-2. 나머지 파일(원본과 같은 경로에 쓰는 것)
                foreach (var item in items.Values)
                {
                    var results = new List<EditResult>();
                    var produced = new List<BuildManifestFile>();
                    string error;
                    try { error = StageItem(item, stage, previous, results, produced); }
                    catch (Exception ex) { error = ex.Message; }
                    if (error != null)
                    {
                        // 파일을 만들지 못했으면 적용으로 세지 않는다. 다른 상태의 결과는 그대로 둔다
                        report.FileErrors.Add(item.Output + ": " + error);
                        foreach (var r in results.Where(x => x.Status == EditStatus.Applied)) { r.Status = EditStatus.Invalid; r.Message = "파일을 만들지 못했다: " + error; }
                        foreach (var e in item.Edits.Where(x => !results.Any(r => r.Edit == x)))
                            results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "파일을 만들지 못했다: " + error });
                        // 유닛 파일을 못 썼으면 그 유닛의 사본은 쓰이지 않는다. 남기지 않는다
                        foreach (var plan in plans.Where(p => p.Unit == item.Output)) FailPlan(plan, "유닛 파일을 만들지 못했다: " + error);
                    }
                    else manifest.Files.AddRange(produced);
                    report.Results.AddRange(results);
                }
                foreach (var plan in plans) manifest.Files.AddRange(plan.Produced);
```

`FailPlan`이 `plan.Produced`를 비우므로 실패한 유닛의 사본은 목록 파일에 들어가지 않고, 임시 폴더에서 출력 폴더로 옮겨지지도 않는다.

(d) `Build`의 3단계(`// 3. 새 파일을 옮긴다`)에서 파일을 쓰지 못했을 때 결과를 고치는 부분을 바꾼다. 사본은 출력 경로와 `Edit.File`이 다르므로 경로 비교로는 찾을 수 없다. 원래:

```csharp
                        string virtualPath = f.Path.Substring("resource/".Length);
                        report.FileErrors.Add(virtualPath + ": " + ex.Message);
                        foreach (var r in report.Results.Where(x => x.Status == EditStatus.Applied && SourceStack.ToVirtual(x.Edit.File) == virtualPath))
                        { r.Status = EditStatus.Invalid; r.Message = "파일을 쓰지 못했다: " + ex.Message; }
```

바꾼 것:

```csharp
                        string virtualPath = f.Path.Substring("resource/".Length);
                        report.FileErrors.Add(virtualPath + ": " + ex.Message);
                        List<Edit> owned;
                        if (owners.TryGetValue(f.Path, out owned))
                            foreach (var r in report.Results.Where(x => x.Status == EditStatus.Applied && owned.Contains(x.Edit)))
                            { r.Status = EditStatus.Invalid; r.Message = "파일을 쓰지 못했다: " + ex.Message; }
```

`owners`는 클래스 필드로 둔다. `readonly string modDir, resourceDir;` 다음에 더하고, `Build`의 첫 줄(`var report = new BuildReport();`) 다음에 `owners.Clear();`를 넣는다:

```csharp
        // 출력 상대 경로 → 그 파일에 담긴 수정. 파일을 쓰지 못했을 때 어느 수정이 적용되지 않았는지 찾는 데 쓴다
        readonly Dictionary<string, List<Edit>> owners = new Dictionary<string, List<Edit>>(StringComparer.OrdinalIgnoreCase);
```

(e) `StageFile` 메서드를 지우고 그 자리에 아래 메서드들을 넣는다:

```csharp
        static WorkItem ItemFor(Dictionary<string, WorkItem> items, string virtualPath, SourceFile source)
        {
            WorkItem item;
            if (!items.TryGetValue(virtualPath, out item)) { item = new WorkItem { Output = virtualPath, Source = source }; items[virtualPath] = item; }
            return item;
        }

        static bool SameSlot(Edit a, Edit b)
        {
            return a.Occurrence == b.Occurrence && a.Arg == b.Arg && string.Equals(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
        }

        // from 파일 안에서 사본으로 바뀐 파일을 가리키는 include 경로들. 같은 지시문이 펼치기에서 여러 번 지나가도 한 번만 낸다
        static List<RawPatch> Rewrites(List<SdlIncludeEdge> edges, string from, HashSet<string> copySet, string unit)
        {
            var result = new List<RawPatch>();
            var seen = new HashSet<int>();
            foreach (var e in edges)
                if (e.FromFile == from && copySet.Contains(e.ToFile) && seen.Add(e.Index))
                    result.Add(new RawPatch { Index = e.Index, Old = e.Raw, New = IncludeChain.RewriteRaw(e.Raw, IncludeChain.CopyName(e.ToFile, unit)) });
            return result;
        }

        void FailPlan(UnitPlan plan, string message)
        {
            foreach (var r in plan.Results.Where(x => x.Status == EditStatus.Applied)) { r.Status = EditStatus.Invalid; r.Message = message; }
            foreach (var e in plan.Copies.SelectMany(c => c.Edits).Where(x => !plan.Results.Any(r => r.Edit == x)))
                plan.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = message });
            foreach (var f in plan.Produced) owners.Remove(f.Path);
            plan.Produced.Clear();
        }

        // 한 유닛의 '이 유닛만' 수정을 계획한다. 사본이 필요 없으면 null.
        // 유닛 자신의 파일에 있는 값은 items에 바로 넣는다. 적용할 수 없는 수정은 여기서 보고한다
        UnitPlan PlanUnit(string unit, List<Edit> edits, List<Edit> shared, Dictionary<string, WorkItem> items, BuildReport report)
        {
            var unitSource = stack.Find(unit);
            if (unitSource == null)
            {
                foreach (var e in edits) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 유닛 파일이 없다: " + unit });
                return null;
            }
            var edges = new SdlResolver(stack.ReadText).Resolve(unit).IncludeEdges;
            var live = new Dictionary<string, List<Edit>>(StringComparer.Ordinal);
            var copySet = new HashSet<string>(StringComparer.Ordinal);
            foreach (var group in edits.GroupBy(e => SourceStack.ToVirtual(e.File ?? "")))
            {
                string file = group.Key;
                if (file == unit) { ItemFor(items, unit, unitSource).Edits.AddRange(group); continue; }
                var source = stack.Find(file);
                if (source == null)
                {
                    foreach (var e in group) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없다: " + file });
                    continue;
                }
                var allowed = new List<Edit>();
                foreach (var e in group)
                {
                    string blocker = IncludeChain.UnitScopeBlocker(edges, unit, file, e.Path);
                    if (blocker != null) report.Results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = blocker });
                    else allowed.Add(e);
                }
                if (allowed.Count == 0) continue;
                // 미리 적용해 본다. 하나도 적용되지 않으면 사본을 만들지 않는다
                var probe = new List<EditResult>();
                TextKind kind;
                TokenPatcher.Apply(TextCodec.Decode(stack.Read(source), out kind), allowed, probe);
                if (!probe.Any(r => r.Status == EditStatus.Applied)) { report.Results.AddRange(probe); continue; }
                live[file] = allowed;
                copySet.UnionWith(IncludeChain.CopySet(edges, unit, file));
            }
            if (copySet.Count == 0) return null;

            var plan = new UnitPlan { Unit = unit };
            foreach (string file in copySet.OrderBy(x => x, StringComparer.Ordinal))
            {
                var item = new WorkItem { Output = IncludeChain.CopyName(file, unit), Source = stack.Find(file), Always = true };
                List<Edit> own;
                if (live.TryGetValue(file, out own)) item.Edits.AddRange(own);
                item.Silent.AddRange(shared.Where(s => SourceStack.ToVirtual(s.File ?? "") == file && !item.Edits.Any(u => SameSlot(u, s))));
                item.Raw.AddRange(Rewrites(edges, file, copySet, unit));
                plan.Copies.Add(item);
            }
            plan.UnitRaw.AddRange(Rewrites(edges, unit, copySet, unit));
            return plan;
        }

        // 원본 하나에 수정과 include 고쳐 쓰기를 적용해 임시 폴더에 쓴다. 파일을 만들 수 없으면 이유를 돌려준다
        string StageItem(WorkItem item, string stage, Dictionary<string, BuildManifestFile> previous, List<EditResult> results, List<BuildManifestFile> produced)
        {
            byte[] original = stack.Read(item.Source);
            TextKind kind;
            string text = TextCodec.Decode(original, out kind);
            if (!original.AsSpan().SequenceEqual(TextCodec.Encode(text, kind)))
                return "원본을 읽고 다시 쓰면 바이트가 달라진다(인코딩이 깨진 파일). 고치지 않는다";

            var usable = new List<Edit>();
            foreach (var e in item.Edits)
            {
                if (e.New != null && !TextCodec.CanEncode(e.New, kind))
                    results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "새 값에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + e.New });
                else usable.Add(e);
            }
            foreach (var p in item.Raw)
                if (!TextCodec.CanEncode(p.New, kind)) return "include 경로를 이 파일의 인코딩(" + kind + ")으로 쓸 수 없다: " + p.New;

            // '이 유닛만' 수정을 앞에 둔다: 같은 자리를 '전부' 수정도 고치면 앞의 것이 이긴다
            var all = usable.Concat(item.Silent.Where(s => s.New != null && TextCodec.CanEncode(s.New, kind))).ToList();
            var applied = new List<EditResult>();
            // 적용이 검증에서 던져도 그때까지의 결과는 보고한다(호출한 쪽이 적용을 적용 불가로 바꾼다)
            var reported = new List<EditResult>();
            string patched;
            try { patched = TokenPatcher.Apply(text, all, applied, item.Raw); }
            finally
            {
                reported.AddRange(applied.Where(r => item.Edits.Contains(r.Edit)));
                results.AddRange(reported);
            }
            int count = reported.Count(r => r.Status == EditStatus.Applied);
            if (count == 0 && item.Raw.Count == 0 && !item.Always) return null;

            string relative = "resource/" + item.Output;
            if (FullPath(relative) == null) return "출력 폴더의 resource 밖을 가리키는 경로다";
            if (File.Exists(FullPath(relative)) && !previous.ContainsKey(relative)) return "출력 폴더에 이 도구가 만들지 않은 같은 경로의 파일이 있다. 덮어쓰지 않는다: " + relative;
            string target = Path.Combine(stage, relative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(target));
            File.WriteAllBytes(target, TextCodec.Encode(patched, kind));
            produced.Add(new BuildManifestFile { Path = relative, Layer = item.Source.Layer, Edits = count });
            owners[relative] = item.Edits;
            return null;
        }
```

`using System;`이 이미 있으므로 `StringComparer`를 쓸 수 있다. 파일 위쪽의 `using` 목록은 고치지 않는다.

- [ ] **Step 5: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== TokenPatcher ok`, `== ModBuilder ok`, `== UnitScopeBuild ok`, 마지막 줄 `failed=0`. 기존 `ModBuilder` 테스트는 한 줄도 고치지 않고 통과해야 한다. 기존 테스트가 실패하면 테스트를 고치지 말고 구현을 본다.

- [ ] **Step 6: 실제 파일로 항등 검사를 다시 돈다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`
Expected: `codec-mismatch=0 patch-mismatch=0`, `failed=0`. `TokenPatcher.Apply`를 고쳤으므로 실제 파일 전체에서 같은 값으로 바꾸는 수정이 여전히 바이트 단위로 같은 결과를 내는지 본다.

- [ ] **Step 7: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): build unit-scoped edits as per-unit copies of the include chain

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 6: 명령줄 `units`, `trace`, `add --scope unit`

**Files:**
- Modify: `editor/CtaToybox.Cli/Program.cs`
- Modify: `editor/CtaToybox.Core.Tests/CliTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/RealFilesCheck.cs`

**Interfaces:**
- Consumes: `UnitIndex.LoadOrBuild`, `.Search`, `.UsersOf`, `DisplayNames.Load`, `.OfUnit`, `UnitKind.All` (Task 2). `IncludeChain.UnitScopeBlocker` (Task 3). `UnitTracer.Trace`, `TraceValue` (Task 4). `SdlResolver(Func<string, string>)` (Task 1).
- Produces (명령줄):
  - `ctatoybox units [--find <글자>] [--kind <종류>] [--layer <계층>] [--limit N] [--names <csv>]`
  - `ctatoybox trace <유닛 가상 경로> [--filter <글자>]`
  - `ctatoybox add ... [--scope shared|unit] [--unit <유닛 가상 경로>]`
  - 모르는 옵션은 사용법 오류(종료 코드 2)다.
  - 색인 캐시는 설정 파일과 같은 폴더의 `cache/unit-index.json`이다(기본 설정이면 `editor/cache/unit-index.json`).

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/CliTests.cs`에서 fixture를 만드는 줄(`T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), ...)`) 다음에 더한다:

```csharp
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity (include \"s.inc\") {mass 10}}"),
            ("-vehicle/x/a/b.def", "{game_entity (include \"s.inc\")}"),
            ("-vehicle/x/a/s.inc", "{box {item \"mine\" 24}}"));
```

`Run()`의 맨 끝(`build writes the file` 검사 다음)에 더한다:

```csharp
        // 모르는 옵션
        T.Eq(2, Run(out o, "show", "set/a.set", "--settings", settings, "--fliter", "x"), "an unknown option is a usage error");
        T.True(o.Contains("--fliter"), "and names the option");

        // units
        const string unitA = "entity/-vehicle/x/a/a.def", unitB = "entity/-vehicle/x/a/b.def", inc = "entity/-vehicle/x/a/s.inc";
        T.Eq(0, Run(out o, "units", "--settings", settings), "units succeeds");
        T.True(o.Contains("새로 만들었다"), "the first run builds the index");
        T.True(File.Exists(Path.Combine(work, "cache", "unit-index.json")), "the cache sits next to the settings file");
        T.True(o.Contains("vehicle") && o.Contains(unitA) && o.Contains(unitB), "units lists kind and path");
        T.True(!o.Contains(inc), "an include file is not a unit");
        T.Eq(0, Run(out o, "units", "--settings", settings, "--find", "B.DEF"), "units --find succeeds");
        T.True(!o.Contains("새로 만들었다"), "the second run uses the cache");
        T.True(o.Contains(unitB) && !o.Contains(unitA), "find filters by path, ignoring case");
        T.Eq(0, Run(out o, "units", "--settings", settings, "--kind", "breed"), "units --kind succeeds");
        T.True(!o.Contains(unitA) && o.Contains("0개"), "no breeds in the fixture");
        T.Eq(2, Run(out o, "units", "--settings", settings, "--kind", "tank"), "an unknown kind is a usage error");
        T.Eq(0, Run(out o, "units", "--settings", settings, "--limit", "1"), "units --limit succeeds");
        T.True(o.Contains(unitA) && !o.Contains(unitB) && o.Contains("2개 가운데 1개"), "limit cuts the list and says so");

        // trace
        T.Eq(0, Run(out o, "trace", unitA, "--settings", settings), "trace succeeds");
        T.True(o.Contains("/game_entity/box/item [0]"), "trace prints expanded rows");
        T.True(o.Contains("1=24  ← " + inc + " /box/item [0] 값 1  공유 2  이 유닛만: 가능"), "an included value: file slot, share count, unit scope possible");
        T.True(o.Contains("0=10  ← " + unitA + " /game_entity/mass [0] 값 0  공유 1"), "the unit's own value is shared by one");
        T.Eq(0, Run(out o, "trace", unitA, "--settings", settings, "--filter", "mass"), "trace --filter succeeds");
        T.True(!o.Contains("box/item"), "the filter hides other rows");
        T.Eq(1, Run(out o, "trace", "entity/-vehicle/x/a/none.def", "--settings", settings), "trace of a missing unit fails");
        T.Eq(2, Run(out o, "trace", "--settings", settings), "trace without a unit is a usage error");

        // add --scope unit, build
        string project2 = Path.Combine(work, "p2.json");
        T.Eq(2, Run(out o, "add", "--settings", settings, "--project", project2, "--file", inc, "--path", "/box/item", "--arg", "1", "--new", "5", "--scope", "everyone"), "an unknown scope is a usage error");
        T.Eq(2, Run(out o, "add", "--settings", settings, "--project", project2, "--file", inc, "--path", "/box/item", "--arg", "1", "--new", "5", "--scope", "unit"), "scope unit needs --unit");
        T.Eq(1, Run(out o, "add", "--settings", settings, "--project", project2, "--file", "set/a.set", "--path", "/a/x", "--new", "5", "--scope", "unit", "--unit", unitA), "a file outside the unit's include chain is refused");
        T.True(o.Contains("include"), "with the reason");
        T.True(!File.Exists(project2), "refused edits are not saved");
        T.Eq(0, Run(out o, "add", "--settings", settings, "--project", project2, "--file", inc, "--path", "/box/item", "--arg", "1", "--new", "5", "--scope", "unit", "--unit", unitA), "add --scope unit succeeds");
        T.Eq("unit", EditProject.Load(project2).Edits[0].Scope, "the scope is saved");
        T.Eq(unitA, EditProject.Load(project2).Edits[0].Unit, "the unit is saved");
        T.Eq(0, Run(out o, "build", "--settings", settings, "--project", project2), "build with a unit-scoped edit succeeds");
        string copy = IncludeChain.CopyName(inc, unitA);
        T.True(File.Exists(Path.Combine(outDir, "resource", copy.Replace('/', Path.DirectorySeparatorChar))), "build writes the copy");
        T.True(File.Exists(Path.Combine(outDir, "resource", unitA.Replace('/', Path.DirectorySeparatorChar))), "and the rewritten unit file");
        T.True(!File.Exists(Path.Combine(outDir, "resource", inc.Replace('/', Path.DirectorySeparatorChar))), "the shared file is not written");
        T.True(!File.Exists(Path.Combine(outDir, "resource", "set", "a.set")), "the previous project's output was removed");
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `FAIL an unknown option is a usage error` 등 새 검사가 실패한다. `== Cli FAILED`.

- [ ] **Step 3: `Program.cs`를 고친다**

(a) `Usage`를 바꾼다:

```csharp
        const string Usage =
            "사용법:\n" +
            "  ctatoybox init --game <폴더> --workshop <폴더> --options <파일> --out <폴더> [--name <이름>]\n" +
            "  ctatoybox sources\n" +
            "  ctatoybox units [--find <글자>] [--kind <종류>] [--layer <계층>] [--limit N] [--names <csv>]\n" +
            "  ctatoybox trace <유닛 가상 경로> [--filter <글자>]\n" +
            "  ctatoybox show <가상 경로> [--filter <글자>]\n" +
            "  ctatoybox add --project <파일> --file <가상 경로> --path <키 경로> [--occ N] [--arg N] --new <값> [--scope shared|unit] [--unit <유닛 가상 경로>]\n" +
            "  ctatoybox build --project <파일>\n" +
            "공통: --settings <파일> (기본 editor/settings.json)\n" +
            "종류: vehicle, breed, humanskin, item-entity, stuff, pattern";

        // 명령마다 받는 옵션. 여기에 없는 옵션은 사용법 오류다(오타를 조용히 버리지 않는다)
        static readonly Dictionary<string, string[]> KnownOptions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["init"] = new[] { "settings", "game", "workshop", "options", "out", "name" },
            ["sources"] = new[] { "settings" },
            ["units"] = new[] { "settings", "find", "kind", "layer", "limit", "names" },
            ["trace"] = new[] { "settings", "filter" },
            ["show"] = new[] { "settings", "filter" },
            ["add"] = new[] { "settings", "project", "file", "path", "occ", "arg", "new", "scope", "unit" },
            ["build"] = new[] { "settings", "project" },
        };
```

(b) `Run`에서 `string settingsPath = ...` 줄 바로 앞에 더한다:

```csharp
                string[] known;
                if (KnownOptions.TryGetValue(args[0], out known))
                    foreach (string name in opt.Keys)
                        if (!known.Contains(name, StringComparer.OrdinalIgnoreCase)) throw new UsageException("모르는 옵션: --" + name);
```

`switch`에 두 줄을 더한다:

```csharp
                    case "units": return Units(opt, settingsPath, output);
                    case "trace": return Trace(positional, opt, settingsPath, output);
```

(c) `Sources` 메서드 다음에 더한다:

```csharp
        // 유닛 색인을 캐시에서 읽거나 새로 만든다. 캐시는 설정 파일 옆의 cache 폴더에 둔다
        static UnitIndex OpenIndex(SourceStack stack, string settingsPath, TextWriter output)
        {
            string cache = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "cache", "unit-index.json");
            bool rebuilt;
            var index = UnitIndex.LoadOrBuild(stack, cache, (done, total) => output.WriteLine("유닛 색인을 만드는 중: " + done + "/" + total), out rebuilt);
            if (rebuilt) output.WriteLine("유닛 색인을 새로 만들었다: " + index.Units.Count + "개 (" + cache + ")");
            return index;
        }

        static int Units(Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            string kind = opt.ContainsKey("kind") ? opt["kind"].ToLowerInvariant() : null;
            if (kind != null && !UnitKind.All.Contains(kind)) throw new UsageException("--kind는 다음 가운데 하나다: " + string.Join(", ", UnitKind.All));
            int limit = opt.ContainsKey("limit") ? Number(opt, "limit") : 50;
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var index = OpenIndex(stack, settingsPath, output);
                var names = DisplayNames.Load(opt.ContainsKey("names") ? opt["names"] : Path.Combine("data", "display-names.csv"));
                var found = index.Search(opt.ContainsKey("find") ? opt["find"] : null, kind, opt.ContainsKey("layer") ? opt["layer"] : null, names).ToList();
                foreach (var u in found.Take(limit))
                {
                    string name = names.OfUnit(u.Path);
                    output.WriteLine(u.Kind + "  " + u.Layer + "  " + u.Path + (name == null ? "" : "  " + name) + (u.Unresolved > 0 ? "  (풀리지 않은 구문 " + u.Unresolved + ")" : ""));
                }
                output.WriteLine(found.Count > limit ? found.Count + "개 가운데 " + limit + "개를 보였다. --limit로 늘린다." : found.Count + "개");
                return 0;
            }
        }

        static int Trace(List<string> positional, Dictionary<string, string> opt, string settingsPath, TextWriter output)
        {
            if (positional.Count != 1) throw new UsageException("trace에는 유닛의 가상 경로 하나가 필요하다");
            EditorSettings settings; LayerPlan plan;
            using (var stack = Open(settingsPath, output, out settings, out plan))
            {
                if (stack == null) return 1;
                var traced = new UnitTracer(stack).Trace(positional[0]);
                if (traced == null) { output.WriteLine("원본에 이 파일이 없다: " + positional[0]); return 1; }
                var index = OpenIndex(stack, settingsPath, output);
                output.WriteLine(traced.Unit + "  (" + (UnitKind.Of(traced.Unit) ?? "유닛 아님") + ", 계층 " + stack.Find(traced.Unit).Layer + ")");
                foreach (string u in traced.Unresolved) output.WriteLine("풀리지 않은 구문: " + u);
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                foreach (var row in traced.Rows)
                {
                    if (filter != null && row.Path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    output.WriteLine(row.Path + " [" + row.Occurrence + "]");
                    for (int i = 0; i < row.Values.Count; i++)
                    {
                        var v = row.Values[i];
                        if (v.Arg < 0) { output.WriteLine("  " + i + "=" + v.Text + "  (" + v.Note + ")"); continue; }
                        string blocker = IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, v.File, v.KeyPath);
                        output.WriteLine("  " + i + "=" + v.Text + "  ← " + v.File + " " + v.KeyPath + " [" + v.Occurrence + "] 값 " + v.Arg +
                                         "  공유 " + index.UsersOf(v.File).Count + "  이 유닛만: " + (blocker == null ? "가능" : "불가(" + blocker + ")"));
                    }
                }
                return 0;
            }
        }
```

(d) `Add`를 고친다. 첫 줄(`string projectPath = Need(opt, "project"), ...`) 다음에 더한다:

```csharp
            string scope = opt.ContainsKey("scope") ? opt["scope"].ToLowerInvariant() : "shared";
            if (scope != "shared" && scope != "unit") throw new UsageException("--scope는 shared 또는 unit이다");
            if (scope == "unit" && !opt.ContainsKey("unit")) throw new UsageException("--scope unit에는 --unit이 필요하다");
```

`if (warning != null) output.WriteLine("경고: " + warning);` 다음에 더한다:

```csharp
                string unit = SourceStack.ToVirtual(opt.ContainsKey("unit") ? opt["unit"] : fileArg);
                if (scope == "unit")
                {
                    if (stack.Find(unit) == null) { output.WriteLine("원본에 이 유닛 파일이 없다: " + unit); return 1; }
                    string blocker = IncludeChain.UnitScopeBlocker(new SdlResolver(stack.ReadText).Resolve(unit).IncludeEdges, unit, file.VirtualPath, keyPath);
                    if (blocker != null) { output.WriteLine(blocker); return 1; }
                }
```

`project.Edits.Add(new Edit { ... })`를 바꾼다:

```csharp
                project.Edits.Add(new Edit
                {
                    Unit = unit, File = file.VirtualPath, Path = keyPath.ToLowerInvariant(), Occurrence = occ, Arg = arg, Old = node.Text, New = newValue, Scope = scope
                });
```

마지막 출력 줄을 바꾼다:

```csharp
                output.WriteLine("수정을 더했다: " + file.VirtualPath + " " + keyPath + " " + node.Text + " → " + newValue + " (계층 " + file.Layer + (scope == "unit" ? ", 이 유닛만: " + unit : "") + ")");
```

`Program.cs` 위쪽의 `using` 목록은 그대로다(`System.Linq`가 이미 있다). `SdlResolver`와 `SdlNode`는 전역 네임스페이스에 있다.

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== Cli ok`, 마지막 줄 `failed=0`.

- [ ] **Step 5: 실제 파일 검증에 색인과 추적을 더한다**

`editor/CtaToybox.Core.Tests/RealFilesCheck.cs`의 `Run` 끝(`T.True(files > 1000, ...)` 다음)에 더한다:

```csharp
        // 유닛 색인: 실제 원본으로 만들어 지고, 걸리는 시간을 적는다
        var clock = System.Diagnostics.Stopwatch.StartNew();
        var index = UnitIndex.Build(stack, null);
        Console.WriteLine("units=" + index.Units.Count + " index-seconds=" + clock.Elapsed.TotalSeconds.ToString("0.0") +
                          " kinds=" + string.Join(",", index.Units.GroupBy(u => u.Kind).OrderBy(g => g.Key).Select(g => g.Key + ":" + g.Count())));
        T.True(index.Units.Count > 10000, "the index holds the game's units");

        // 출처 추적: 차량 전체에서, 편집 가능하다고 한 자리의 값이 실제로 그 파일 그 자리의 토큰과 같은지 본다
        var tracer = new UnitTracer(stack);
        var parsedFiles = new Dictionary<string, SdlNode>(StringComparer.Ordinal);
        int traced = 0, values = 0, editable = 0, wrongSlot = 0;
        foreach (var u in index.Units.Where(x => x.Kind == "vehicle"))
        {
            var t = tracer.Trace(u.Path);
            traced++;
            foreach (var row in t.Rows)
                foreach (var v in row.Values)
                {
                    values++;
                    if (v.Arg < 0) continue;
                    editable++;
                    SdlNode root;
                    if (!parsedFiles.TryGetValue(v.File, out root)) { root = SdlCore.Parse(stack.ReadText(v.File)).Root; parsedFiles[v.File] = root; }
                    var node = KeyLocator.Find(root, v.KeyPath, v.Occurrence, v.Arg);
                    if (node == null || node.Text != v.Text) { wrongSlot++; if (examples.Count < 20) examples.Add("slot: " + u.Path + " " + row.Path + " ← " + v.File + " " + v.KeyPath); }
                }
        }
        Console.WriteLine("traced-vehicles=" + traced + " values=" + values + " editable=" + editable + " wrong-slot=" + wrongSlot);
        foreach (string e in examples.Where(x => x.StartsWith("slot: "))) Console.WriteLine("  " + e);
        T.Eq(0, wrongSlot, "every editable trace value names the slot that holds that token");
        T.True(editable > 100000, "vehicles were actually traced");

        // 계획의 근거로 쓴 사실이 지금 원본에서도 맞는지 본다
        const string m3a1 = "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def", m3a1Inc = "entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc";
        var halftrack = tracer.Trace(m3a1);
        var mine = halftrack == null ? null : halftrack.Rows.SelectMany(r => r.Values).FirstOrDefault(v => v.Arg >= 0 && v.File == m3a1Inc && v.KeyPath.EndsWith("/box/item") && v.Text == "24");
        Console.WriteLine("m3a1-mine=" + (mine == null ? "not found" : mine.File + " " + mine.KeyPath + " [" + mine.Occurrence + "] 값 " + mine.Arg) + " users=" + index.UsersOf(m3a1Inc).Count);
        T.True(mine != null, "the in-game check candidate (M3A1 halftrack, mine m1a1 24) is where the plan says");
        T.True(index.UsersOf(m3a1Inc).Count >= 2, "and its include file is shared by more than one unit");
```

`examples`는 이 메서드 위쪽에 이미 있는 목록이다.

- [ ] **Step 6: 실제 파일 검증을 돈다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`
Expected: `wrong-slot=0`, `failed=0`. `units=`, `index-seconds=`, `m3a1-mine=` 줄이 찍힌다. 이 세 줄의 값을 그대로 보고서에 옮긴다(Task 7의 문서에 쓴다).

`wrong-slot`이 0이 아니거나 `m3a1-mine=not found`면 추측으로 고치지 말고, 찍힌 예시와 함께 `NEEDS_CONTEXT`로 보고한다.

- [ ] **Step 7: 전체 테스트를 돈다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 모든 묶음 통과.

- [ ] **Step 8: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Cli editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): units and trace commands, add --scope unit, reject unknown options

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 7: 문서와 게임 안 확인 준비

**Files:**
- Modify: `docs/07-stat-editor/README.md`
- Modify: `docs/07-stat-editor/ingame-check.md`
- Modify: `CLAUDE.md` ("명령" 절의 편집기 줄)

**Interfaces:**
- Consumes: Task 6의 명령과 Task 6 Step 6에서 찍힌 실제 값.
- Produces: 사용법 문서, 3단계 게임 안 확인 절차, 확인용 프로젝트 파일 `editor/projects/ingame-check-unit.json`(커밋하지 않는다. `editor/projects/`의 확인용 파일은 지금까지도 작업 트리에만 두었다).

- [ ] **Step 1: `README.md`를 고친다**

`docs/07-stat-editor/README.md`에서:

(a) "할 수 있는 것과 없는 것" 표를 아래로 바꾼다:

```markdown
| 할 수 있다 | 아직 없다 |
|---|---|
| 유닛을 경로나 표시 이름으로 찾기 (`units`) | 화면 |
| 유닛의 값이 어느 파일의 어느 자리에서 왔는지, 몇 개 유닛이 그 파일을 같이 쓰는지 보기 (`trace`) | 유닛이 쓰는 무기·탄약 파일을 유닛과 함께 보기 |
| 원본 파일(바닐라와 켜 둔 모드)의 값 하나를 바꾸는 수정을 프로젝트 파일에 담기 | 매크로 본문 안의 값을 "이 유닛만" 고치기 |
| include로 공유된 값을 "이 유닛만" 고치기 (`add --scope unit`) | 키나 블록을 더하거나 빼기 |
| 프로젝트를 빌드해 `mod/CTAToybox/resource`에 써넣기 | |
| 게임이나 모드가 갱신돼 원래 값이 달라진 수정을 알아채기 | |
```

(b) "값 하나를 고치기" 절 다음, "빌드 결과의 뜻" 절 앞에 절을 더한다. `<...>` 안의 값은 Task 6 Step 6과 아래 Step 3에서 실제로 찍힌 것을 그대로 옮긴다:

````markdown
## 유닛에서 시작해 고치기

1. 유닛을 찾는다. 처음 한 번은 유닛 색인을 만드느라 시간이 걸린다(이 PC에서 <index-seconds>초, 유닛 <units>개). 결과는 `editor/cache/`에 저장되고, 모드를 켜고 끄거나 모드가 갱신되면 다시 만든다.

   ```
   ctatoybox units --find halftrack --kind vehicle
   ```

   한 줄이 유닛 하나다: `종류  계층  경로  표시 이름`. 표시 이름은 `data/display-names.csv`에 `desc/entity/<파일 이름>` 키가 있는 엔티티에만 붙는다.

2. 유닛의 값을 본다.

   ```
   ctatoybox trace entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --filter box/item
   ```

   ```
   <Step 3에서 찍힌 trace 출력을 그대로 붙인다>
   ```

   - 윗줄은 펼친 뒤의 키 경로와 순번이다.
   - `←` 뒤는 그 값이 실제로 있는 파일, 그 파일 안의 키 경로, `[몇 번째]`, `값 N`이다. 이 넷을 `add`의 `--file`, `--path`, `--occ`, `--arg`에 넣는다.
   - `공유 N`은 그 파일을 읽는 유닛의 수다. `shared`로 고치면 N개 유닛이 모두 바뀐다.
   - `이 유닛만: 가능`이면 `--scope unit`을 쓸 수 있다. `불가`면 괄호 안에 이유가 있다.
   - 괄호만 있는 줄은 고칠 수 없는 값이다(치환이나 계산으로 만든 값, include 인자).

3. "이 유닛만" 고친다.

   ```
   ctatoybox add --project editor/projects/my-tweaks.json --scope unit --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --file entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc --path "<trace의 키 경로>" --occ <N> --arg <N> --new 5
   ctatoybox build --project editor/projects/my-tweaks.json
   ```

### "이 유닛만"이 하는 일

공유 파일은 건드리지 않는다. 유닛에서 그 파일까지의 include 사슬을 유닛 전용 사본으로 만들고, 유닛 파일과 사본 안의 include 경로를 사본으로 고쳐 쓴다.

- 사본은 원본과 같은 폴더에 `<원본 이름>__ctb_<유닛 파일 이름>_<8자 해시><확장자>`로 생긴다.
- 유닛 파일도 출력 폴더에 생긴다. 바뀌는 것은 include 경로뿐이다.
- 같은 파일에 `shared` 수정도 걸려 있으면 사본에도 들어간다. 같은 자리를 둘 다 고치면 그 유닛에는 "이 유닛만"의 값이 쓰인다.
- 값이 유닛 자신의 파일에 있으면 사본 없이 그 파일을 고친다.

### "이 유닛만"이 안 되는 경우

| 경우 | 이유 |
|---|---|
| 매크로 본문(`(define ...)` 안)의 값 | 그 매크로를 부르는 모든 곳이 바뀐다. `shared`만 된다 |
| `.presets`에 있는 값 | include로 읽는 파일이 아니라서 사본으로 바꿔 끼울 수 없다 |
| 치환이나 계산으로 만든 값 | 원본의 인자나 피연산자를 고친다 |
````

(c) "빌드가 건드리는 파일" 절의 첫 항목 다음에 한 줄을 더한다:

```markdown
- "이 유닛만" 수정은 원본에 없는 이름의 사본 파일도 만든다. 사본도 `.ctatoybox-build.json`에 적히고 다음 빌드에서 지워진다.
```

(d) "알려진 한계" 절에 한 줄을 더한다:

```markdown
- **게임이 사본 파일과 고쳐 쓴 include를 받아들이는지는 [ingame-check.md](ingame-check.md)의 3단계 확인으로 본다.** 확인 전에는 "이 유닛만"의 결과를 믿지 않는다.
```

- [ ] **Step 2: `CLAUDE.md`의 편집기 줄을 고친다**

`CLAUDE.md` "명령" 절의 `- 편집기 명령줄: ...` 줄을 바꾼다:

```markdown
- 편집기 명령줄: `dotnet run --project editor/CtaToybox.Cli -c Release -- <명령>`. 명령은 `sources`, `units`, `trace`, `show`, `add`, `build`. 사용법은 `docs/07-stat-editor/README.md`. 유닛 색인 캐시는 `editor/cache/`(git 제외)에 생긴다.
```

- [ ] **Step 3: 확인용 수정을 만들고, 문서에 넣을 실제 출력을 얻는다**

게임이 꺼져 있을 때 한다(빌드가 `mod/CTAToybox/resource`에 쓴다). 저장소 루트에서:

```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- sources
dotnet run --project editor/CtaToybox.Cli -c Release -- trace entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --filter box/item
```

Expected: `sources`에 "뒤에 로드되는 모드가 있다" 경고가 없다(`CTAToybox`가 `{mods}` 맨 뒤다). `trace`에 `="mine m1a1"`과 `=24`가 있는 행이 나오고 `← entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc`, `공유 3`, `이 유닛만: 가능`이 붙는다.

이 출력을 Step 1 (b)의 `<...>` 자리에 그대로 붙인다. `공유`의 수나 계층이 위와 다르면(모드 구성이 바뀌었으면) 찍힌 대로 적는다.

`24`가 있는 줄의 `[몇 번째]`와 `값 N`을 써서 수정을 더하고 빌드한다:

```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- add --project editor/projects/ingame-check-unit.json --scope unit --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --file entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc --path "<trace의 키 경로>" --occ <trace의 대괄호 안 수> --arg <trace의 값 번호> --new 5
dotnet run --project editor/CtaToybox.Cli -c Release -- build --project editor/projects/ingame-check-unit.json
```

Expected: `씀:` 두 줄(`resource/entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def`와 `..._halftrack__ctb_m3a1_halftrack_<해시>.inc`), `적용 1건`, 나머지 0건.

빌드 결과를 다시 펼쳐 확인한다. 출력 폴더는 원본 계층이 아니므로 `trace`로는 볼 수 없다. 생성물을 직접 본다:

```bash
grep -n "include" mod/CTAToybox/resource/entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def
grep -n "mine m1a1" mod/CTAToybox/resource/entity/-vehicle/usa/btr/m3a1_halftrack/*__ctb_*.inc
```

Expected: 첫 명령은 `(include "m3a1_halftrack__ctb_m3a1_halftrack_<해시>.inc")` 한 줄. 둘째 명령은 `{item "mine m1a1" 5}`가 든 줄.

- [ ] **Step 4: `ingame-check.md`에 3단계 절차를 더한다**

`docs/07-stat-editor/ingame-check.md`의 끝에 절을 더한다. `<해시>`와 `<...>`는 Step 3에서 실제로 나온 값으로 채운다:

```markdown
## 3단계: "이 유닛만" 확인

스펙의 구현 순서 3단계다. 게임이 사본 파일 이름과 고쳐 쓴 include를 받아들이는지 본다. 이 확인이 끝나기 전에는 화면(4단계)을 만들지 않는다.

**준비한 것** (2026-10-13)

- 프로젝트: `editor/projects/ingame-check-unit.json`
- 수정: M3A1 하프트랙(`entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def`)만, `m3a1_halftrack.inc`의 `{item "mine m1a1" 24}`를 5로.
- 생성물:
  - `resource/entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def` (include 경로만 바뀜)
  - `resource/entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack__ctb_m3a1_halftrack_<해시>.inc` (`mine m1a1`이 5)
- 이 차량을 고른 근거: `m3a1_halftrack.inc`는 `.def` 3개가 include한다(`m3a1_halftrack`, `m3a1_halftrack_inf`, `m3a1_halftrack_inf_late`). 지금 구성에서 계층은 `vanilla`다. `m3a1_halftrack`은 MACE 스커미시 후기 미국 진영에서 일반 구매된다(`set/multiplayer/units/late/units_usa.set`의 `cd(150)`).

**절차**

1. Steam 라이브러리에서 게임을 켠다(Steam 없이 켜면 창작마당 모드가 로드되지 않는다. 2026-10-13에 겪음).
2. 스커미시 후기, 미국 진영에서 M3A1 Halftrack을 사서 인벤토리를 연다. `mine m1a1`(M1A1 대전차 지뢰)의 수량을 본다.
3. 같은 `.inc`를 쓰는 다른 차량(`m3a1_halftrack_inf`, `m3a1_halftrack_inf_late`)을 꺼낼 수 있으면 그 차량의 지뢰 수량도 본다. 스커미시에서 살 수 없으면 "확인하지 못함"으로 적는다.
4. 게임을 끄고 `game.log`에서 `m3a1_halftrack`이나 `__ctb_`가 든 오류 줄을 찾는다.
5. 확인이 끝나면 수정을 끄고 다시 빌드해 생성물을 지운다.

**기록**

| 항목 | 결과 |
|---|---|
| 확인한 날짜와 게임 버전 | 확인하지 않음 |
| M3A1 하프트랙의 `mine m1a1` 수량 (기대 5) | 확인하지 않음 |
| 같은 `.inc`를 쓰는 다른 차량의 수량 (기대 24) | 확인하지 않음 |
| `game.log`에 사본 파일이나 include와 관련된 오류 | 확인하지 않음 |
| 수정을 끄고 다시 빌드한 뒤 24로 돌아옴 | 확인하지 않음 |

M3A1이 24 그대로면 게임이 고쳐 쓴 유닛 파일이나 사본을 쓰지 않은 것이다. 그때는 추측으로 원인을 정하지 않는다. `game.log`와 생성물을 먼저 보고, 사본 이름을 원본과 같은 글자 종류만 쓰는 짧은 이름으로 바꾼 빌드, 사본 없이 유닛 파일에 값을 직접 넣은 빌드를 각각 만들어 어느 쪽에서 갈리는지 본다.
```

- [ ] **Step 5: 전체 테스트를 돈다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 모든 묶음 통과.

Run: `git -C E:/CTAToybox status --short`
Expected: 추적되는 변경은 `CLAUDE.md`, `docs/07-stat-editor/README.md`, `docs/07-stat-editor/ingame-check.md`뿐이다. `editor/projects/`, `mod/CTAToybox/.ctatoybox-build.json`, `mod/CTAToybox/resource/entity/`는 추적되지 않는 채로 둔다. `editor/cache/`는 보이지 않아야 한다(git 제외).

- [ ] **Step 6: 커밋하고 병합한다**

```bash
git -C E:/CTAToybox add CLAUDE.md docs/07-stat-editor
git -C E:/CTAToybox commit -m "docs(editor): units, trace and unit scope usage; stage 3 in-game checklist

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
git -C E:/CTAToybox checkout develop
git -C E:/CTAToybox merge --no-ff feat/stat-editor-provenance -m "Merge branch 'feat/stat-editor-provenance' into develop"
```

- [ ] **Step 7: 사용자에게 게임 안 확인을 부탁한다**

`docs/07-stat-editor/ingame-check.md`의 "3단계" 절차 1~3을 그대로 전한다. 볼 것은 두 가지다: M3A1 하프트랙의 `mine m1a1` 수량(기대 5), 꺼낼 수 있으면 같은 `.inc`를 쓰는 다른 차량의 수량(기대 24). 결과를 받으면 "기록" 표를 채우고 `game.log`를 직접 확인한다.
