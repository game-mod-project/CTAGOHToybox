# 스탯 편집기 화면 (구현 순서 4단계) Implementation Plan

> **상태(2026-10-17)**: 끝났다(편집기 화면). 아래 체크 상자는 진행 중에 표시하지 않았다. 무엇을 했는지는 git 기록과 `docs/10-backlog.md` "정리하며 지운 것"에 있다.

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 명령줄 없이 창 하나에서 유닛을 찾고, 요약·전체 탭에서 값과 출처를 보고, "전부 / 이 유닛만"을 골라 고치고, 수정 목록을 관리하고 빌드할 수 있게 한다.

**Architecture:** 화면이 쓰는 상태와 동작을 코어의 `EditorSession`에 모으고(자동 테스트 대상), WPF 앱 `editor/CtaToybox.App`은 그것을 부르는 얇은 층으로 둔다. 키 설명은 `data/annotations`를 C#으로 읽고, 유닛이 가리키는 무기·탄약은 2026-10-14의 데이터 조사로 확인한 이름 규칙으로 잇는다. 화면 확인은 앱의 `--screenshot` 옵션이 창을 PNG로 저장해 사람이 아닌 실행자가 직접 본다.

**Tech Stack:** .NET 8 SDK (C# 12), WPF(`net8.0-windows`, `UseWPF`), `System.Text.Json`. NuGet 패키지를 쓰지 않는다(2026-10-14에 이 PC에서 `dotnet new wpf`가 패키지 없이 빌드되는 것을 확인했다). 테스트는 기존 콘솔 테스트 프로젝트.

**Spec:** `docs/superpowers/specs/2026-10-12-stat-editor-design.md` ("화면" 절과 "구현 순서" 4단계)

## 설계의 근거 (데이터로 확인한 것)

| 사실 | 근거 |
|---|---|
| WPF 앱은 이 PC에서 외부 패키지 없이 빌드된다. SDK는 8.0.425 | 2026-10-14 `dotnet new wpf` + `dotnet build` |
| 키 주석은 `data/annotations/common.psv`(경로\|형태\|등급\|의미\|출처)와 그룹 전용 `<그룹>.psv`다. 등급 분포: 추정 294, 확인 128, 문서 78, 미확인 58, 빈칸 72 | 파일 집계 |
| 주석 조회 규칙: 정확한 경로 > `**/꼬리`(긴 것부터) > `*/이름`(유닛 파일과 펼친 키에만). 그룹은 경로 앞부분으로 정한다(`set/breed`, `entity/humanskin` → human, `entity/-vehicle` → vehicle, `set/stuff` → weapon-ammo, `entity/inventory` → inventory, 나머지 shared) | `tools/lib/Annotations.ps1`, `tools/lib/Common.ps1`의 `Get-SdlFileGroup` |
| 유닛이 가리키는 아이템 이름 2,411개 가운데 2,341개(97%)가 세 규칙(파일 이름과 같음 / 낱말 집합이 같음 / 확장자를 뺀 낱말 집합이 같음)으로 파일 하나에 이어진다. 나머지는 후보가 여럿이거나 없다 | `docs/02-modding-research.md`의 "아이템 이름과 파일" |
| 첫 규칙으로 이어진 `m3_grease`는 게임에서 그 파일의 값이 반영됐다 | `docs/07-stat-editor/ingame-check.md`의 "일괄 확인" 4번 |
| 3단계까지의 "이 유닛만", 매크로 인자, `.presets` 매크로 본문, pak/낱개 우선순위는 게임에서 확인됐다 | 같은 문서 |

**아직 모르는 것:** `{from "..."}` 상속의 게임 동작. 이름 규칙의 나머지 3%(후보가 여럿이거나 없는 이름)를 게임이 어떻게 고르는지. 이 계획은 둘 다 "미확인"으로 화면에 표시하고, Task 10에서 한 빌드로 함께 확인한다.

## 이 계획에서 정한 것 (스펙이 열어 둔 부분)

- **"되돌리기"와 "삭제"**: 스펙의 수정 목록은 "끄기·되돌리기·삭제"를 말한다. 수정을 지우면 값이 원본으로 돌아가므로 "삭제"가 곧 되돌리기다. 화면에는 "끄기"(체크 상자)와 "삭제" 둘만 둔다.
- **"원본이 바뀜"의 "유지"**: "지금 값에 맞추기"로 둔다. 수정의 `old`를 원본의 지금 값으로 바꿔 다시 적용되게 한다. "삭제"도 함께 고를 수 있다.
- **요약 탭의 키**: `editor/summary/<종류>.psv`(형식 `경로|이름`)가 있으면 그것을 쓰고, 없으면 주석에서 등급이 "확인"인 키를 쓴다. 이름은 주석 의미의 첫 마디다. 이 계획에서는 `editor/summary/`를 만들지 않는다(파일이 생기면 그것이 이긴다).
- **전체 탭의 모양**: 스펙 그림은 파일별로 접히는 트리다. 펼친 값을 원본 파일별로 묶어 접을 수 있는 목록(ListView 그룹)으로 만든다. 값 한 줄이 한 행이다.
- **무기·탄약**: 전체 탭 아래에 "참조하는 아이템" 목록을 둔다. 이름 규칙으로 파일 하나에 이어지면 두 번 눌러 그 파일을 유닛처럼 연다. 후보가 여럿이거나 없으면 "후보(미확인)"·"없음"으로 표시하고 열지 않는다.

## Global Constraints

- 게임 폴더와 창작마당 폴더, 프로필의 `options.set`은 읽기만 한다. 쓰는 곳은 `mod/CTAToybox/` 아래와 `editor/` 아래뿐이다.
- 대상 프레임워크는 코어 `net8.0`, 앱 `net8.0-windows`. NuGet 패키지 참조를 추가하지 않는다.
- 화면은 `EditorSession`만 부른다. 원본을 읽거나 파일을 쓰는 코드를 앱에 두지 않는다.
- 화면의 문구는 한국어. 값의 출처, 공유 수, "이 유닛만" 가능 여부, 키 설명의 등급을 숨기지 않는다. 등급이 "확인"이 아닌 설명은 등급을 같이 보인다.
- 게임·엔진의 동작을 추정해 화면이나 문서에 사실처럼 적지 않는다. 확인하지 않은 것은 "미확인"으로 적는다.
- 브랜치: `develop`에서 `feat/stat-editor-app`을 분기해 작업하고 `git merge --no-ff`로 `develop`에 병합한다.
- 모든 Task 뒤에 `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`이 통과해야 한다. Task 6부터 이 스크립트가 앱도 빌드한다.
- 게임 안 확인이 필요한 항목은 Task 10에서 한 빌드로 모은다. Task마다 따로 부탁하지 않는다.
- 코드의 주석과 화면 문구는 한국어, 식별자는 영어. 주변 코드의 주석 밀도를 따른다.
- 커밋 메시지 끝에 `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`를 붙인다.

## Review Focus

1. **같은 자리에 수정을 두 번 넣는 경우.** 화면에서 같은 값을 다시 고치면 새 수정이 쌓이지 않고 바뀌어야 한다. 쌓이면 빌드가 두 번째를 "같은 값을 고치는 수정이 앞에 이미 있다"로 버린다. → Task 5의 테스트.
2. **"이 유닛만"이 안 되는 자리에서 그것을 고르는 경우.** 화면이 선택지를 끄더라도 `EditorSession.AddEdit`이 다시 막아야 한다. → Task 5의 테스트.
3. **프로젝트 파일이 없거나 깨진 경우.** 없는 파일은 빈 프로젝트로 시작하고, 깨진 JSON은 앱을 죽이지 않고 이유를 보여야 한다. → Task 5의 테스트와 Task 8.
4. **유닛 하나가 1,000행을 넘는 경우.** 전체 탭이 가상화 없이 그리면 창이 멈춘다. 그룹이 있어도 가상화가 켜져 있어야 한다. → Task 7(`VirtualizingPanel.IsVirtualizingWhenGrouping`).
5. **색인을 처음 만드는 동안.** 8초 남짓 걸린다. 화면 스레드에서 하면 창이 응답하지 않는다. → Task 6(백그라운드에서 연다).

---

## File Structure

| 파일 | 하는 일 | Task |
|---|---|---|
| `editor/CtaToybox.Core/KeyPattern.cs` (신규) | 주석·요약 키의 경로 조회 순서(정확한 경로, `**/꼬리`, `*/이름`) | 1 |
| `editor/CtaToybox.Core/KeyAnnotations.cs` (신규) | `data/annotations`의 키 설명 읽기와 찾기 | 1 |
| `editor/CtaToybox.Core/ItemLinks.cs` (신규) | 아이템 이름 → `set/stuff` 파일 | 2 |
| `editor/CtaToybox.Core/SummaryKeys.cs` (신규) | 요약 탭에 올릴 키와 이름 | 3 |
| `editor/CtaToybox.Core/UnitTracer.cs` (수정) | `TraceRow.Form`, `UnitTraceResult.Nodes` | 4 |
| `editor/CtaToybox.Core/EditorSession.cs` (신규) | 화면이 쓰는 상태와 동작 | 4, 5 |
| `editor/CtaToybox.App/*` (신규) | WPF 앱 | 6~9 |
| `tools/tests/Run-Tests.ps1` (수정) | 앱 빌드를 테스트에 포함 | 6 |
| `editor/CtaToybox.Core.Tests/*Tests.cs` | Task마다 테스트 파일 하나 | 1~5 |
| `docs/07-stat-editor/README.md`, `ingame-check.md`, `CLAUDE.md` (수정) | 앱 사용법, 4단계 확인 | 10 |

---

### Task 1: 키 설명

**Files:**
- Create: `editor/CtaToybox.Core/KeyPattern.cs`
- Create: `editor/CtaToybox.Core/KeyAnnotations.cs`
- Create: `editor/CtaToybox.Core.Tests/KeyAnnotationsTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Produces (네임스페이스 `CtaToybox.Core`):
  - `static class KeyPattern`: `List<string> PlainKeyPath(string path)`, `IEnumerable<string> Candidates(string path, bool allowWildcard)`(조회할 키를 우선순위대로: 경로 자체, `**/꼬리`들(긴 것부터), `allowWildcard`면 `*/마지막 조각`).
  - `sealed class KeyNote { string Grade; string Meaning; string Source; }` (속성).
  - `sealed class KeyAnnotations`: `static KeyAnnotations Load(string annotationDir, string group)`, `static string GroupOf(string virtualPath)`, `KeyNote Find(string path, string form, bool allowWildcard)`(없으면 `null`), `IEnumerable<KeyValuePair<string, KeyNote>> Entries`(키는 `경로\t형태`), `int Count`.

- [ ] **Step 1: 브랜치를 만든다**

```bash
git -C E:/CTAToybox checkout develop
git -C E:/CTAToybox checkout -b feat/stat-editor-app
```

- [ ] **Step 2: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/KeyAnnotationsTests.cs`:

```csharp
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class KeyAnnotationsTests
{
    public static void Run()
    {
        T.Eq(new[] { "game_entity", "extender[inventory]", "box", "item" }, KeyPattern.PlainKeyPath("/game_entity/extender[inventory]/box/item").ToArray(), "plain path keeps block names");
        T.Eq(new[] { "from", "speed" }, KeyPattern.PlainKeyPath("/from/(\"mobility\")/speed").ToArray(), "macro call segments are dropped");
        T.Eq(new[] { "a", "sensor" }, KeyPattern.PlainKeyPath("/a/off()/#sensor").ToArray(), "call-argument segments and the # marker are dropped");
        T.Eq(new[] { "/a/b/c", "**/a/b/c", "**/b/c", "**/c", "*/c" }, KeyPattern.Candidates("/a/b/c", true).ToArray(), "lookup order with wildcard");
        T.Eq(new[] { "/a/b/c", "**/a/b/c", "**/b/c", "**/c" }, KeyPattern.Candidates("/a/b/c", false).ToArray(), "lookup order without wildcard");

        T.Eq("human", KeyAnnotations.GroupOf("set/breed/mp/usa/late/x.set"), "breed group");
        T.Eq("human", KeyAnnotations.GroupOf("entity/humanskin/a/a.def"), "humanskin group");
        T.Eq("vehicle", KeyAnnotations.GroupOf("entity/-vehicle/a/a.def"), "vehicle group");
        T.Eq("weapon-ammo", KeyAnnotations.GroupOf("set/stuff/smg/m3_grease"), "stuff group");
        T.Eq("inventory", KeyAnnotations.GroupOf("entity/inventory/a/a.def"), "inventory group");
        T.Eq("shared", KeyAnnotations.GroupOf("properties/tank.ext"), "everything else is shared");

        string dir = T.TempDir();
        File.WriteAllText(Path.Combine(dir, "common.psv"),
            "# 주석\n\n" +
            "/game_entity/mass|block|확인|무게. 킬로그램|C\n" +
            "**/box/item|block|추정|적재품 한 칸|S\n" +
            "*/mass|block|추정|어디든 mass|S\n" +
            "*/speed|block|문서|속도|D\n" +
            "**/damage|block|추정|공통 damage|S\n");
        File.WriteAllText(Path.Combine(dir, "weapon-ammo.psv"), "**/damage|block|추정|무기의 damage|S\n");

        var notes = KeyAnnotations.Load(dir, "vehicle");
        T.Eq(5, notes.Count, "five entries");
        var mass = notes.Find("/game_entity/mass", "block", true);
        T.Eq("확인", mass.Grade, "an exact path wins");
        T.Eq("게임 파일 주석", mass.Source, "source abbreviations are expanded");
        T.Eq("추정", notes.Find("/x/mass", "block", true).Grade, "*/name applies with wildcard");
        T.True(notes.Find("/x/mass", "block", false) == null, "*/name does not apply without wildcard");
        T.Eq("적재품 한 칸", notes.Find("/game_entity/extender[inventory]/box/item", "block", false).Meaning, "**/tail applies anywhere");
        T.Eq("파일 구조와 값에서 추론", notes.Find("/game_entity/extender[inventory]/box/item", "block", false).Source, "S is expanded");
        T.True(notes.Find("/game_entity/mass", "macro", true) == null, "the form must match");
        T.Eq("공통 damage", notes.Find("/a/damage", "block", false).Meaning, "the common entry without the group file");
        T.Eq("무기의 damage", KeyAnnotations.Load(dir, "weapon-ammo").Find("/a/damage", "block", false).Meaning, "a group file overrides common");
        T.Eq(5, KeyAnnotations.Load(dir, null).Count, "no group reads only common");
        T.Eq(0, KeyAnnotations.Load(Path.Combine(dir, "none"), "vehicle").Count, "a missing folder is empty");

        File.WriteAllText(Path.Combine(dir, "bad.psv"), "/a|block|추정\n");
        T.Throws<System.FormatException>(() => KeyAnnotations.Load(dir, "bad"), "a line with fewer than five fields is an error");
    }
}
```

`Program.cs`의 suites 목록에서 `("UnitTracer", UnitTracerTests.Run),` 다음 줄에 더한다:

```csharp
            ("KeyAnnotations", KeyAnnotationsTests.Run),
```

- [ ] **Step 3: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `KeyPattern`, `KeyAnnotations`가 없다.

- [ ] **Step 4: `KeyPattern.cs`를 쓴다**

```csharp
// 키 경로로 주석이나 요약 키를 찾을 때의 조회 순서. tools/lib/Annotations.ps1의 Get-Annotation과 같은 규칙이어야 한다.
//   정확한 경로 > **/꼬리 (긴 것부터) > */마지막 조각 (allowWildcard일 때만)
// **/꼬리는 값의 자리를 가리키지 않는 조각(매크로 정의·호출, 호출 인자 이름(), 꺼 둔 블록의 #)을 뺀 경로로 만든다.
using System.Collections.Generic;

namespace CtaToybox.Core
{
    public static class KeyPattern
    {
        public static List<string> PlainKeyPath(string path)
        {
            var parts = new List<string>();
            foreach (string s in path.Split('/'))
            {
                if (s.StartsWith("(") || s.EndsWith("()")) continue;
                string t = s.TrimStart('#');
                if (t.Length > 0) parts.Add(t);
            }
            return parts;
        }

        public static IEnumerable<string> Candidates(string path, bool allowWildcard)
        {
            yield return path;
            var plain = PlainKeyPath(path);
            for (int i = 0; i < plain.Count; i++) yield return "**/" + string.Join("/", plain.GetRange(i, plain.Count - i));
            if (allowWildcard) yield return "*/" + path.Substring(path.LastIndexOf('/') + 1);
        }
    }
}
```

- [ ] **Step 5: `KeyAnnotations.cs`를 쓴다**

```csharp
// 키 의미 주석(data/annotations)을 읽고 찾는다. tools/lib/Annotations.ps1의 Read-Annotations와 같은 형식이다.
// common.psv(경로|형태|등급|의미|출처)와 그룹 전용 <그룹>.psv를 읽고, 같은 키면 그룹 쪽이 이긴다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class KeyNote
    {
        public string Grade { get; set; }
        public string Meaning { get; set; }
        public string Source { get; set; }
    }

    public sealed class KeyAnnotations
    {
        static readonly Dictionary<string, string> SourceNames = new Dictionary<string, string>
        {
            ["C"] = "게임 파일 주석",
            ["S"] = "파일 구조와 값에서 추론",
            ["D"] = "GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인)",
            ["W"] = "Best Way 옛 공식 위키 wiki.bestway.com.ua의 web.archive.org 보존본 (В тылу врага 2 기준, 2026-10-12 확인)",
            ["M"] = "mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인)",
            ["G"] = "Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인)",
        };

        readonly Dictionary<string, KeyNote> map = new Dictionary<string, KeyNote>(StringComparer.Ordinal);

        public int Count { get { return map.Count; } }
        public IEnumerable<KeyValuePair<string, KeyNote>> Entries { get { return map; } }

        // tools/lib/Common.ps1의 Get-SdlFileGroup과 같은 규칙
        public static string GroupOf(string virtualPath)
        {
            if (virtualPath.StartsWith("set/breed/", StringComparison.Ordinal)) return "human";
            if (virtualPath.StartsWith("entity/humanskin/", StringComparison.Ordinal)) return "human";
            if (virtualPath.StartsWith("entity/-vehicle/", StringComparison.Ordinal)) return "vehicle";
            if (virtualPath.StartsWith("set/stuff/", StringComparison.Ordinal)) return "weapon-ammo";
            if (virtualPath.StartsWith("entity/inventory/", StringComparison.Ordinal)) return "inventory";
            return "shared";
        }

        public static KeyAnnotations Load(string annotationDir, string group)
        {
            var result = new KeyAnnotations();
            var files = new List<string> { Path.Combine(annotationDir, "common.psv") };
            if (group != null) files.Add(Path.Combine(annotationDir, group + ".psv"));
            foreach (string file in files)
            {
                if (!File.Exists(file)) continue;
                foreach (string line in File.ReadLines(file, Encoding.UTF8))
                {
                    if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                    string[] p = line.Split('|', 5);
                    if (p.Length < 5) throw new FormatException("주석 줄 형식 오류: " + line);
                    string source;
                    result.map[p[0] + "\t" + p[1]] = new KeyNote { Grade = p[2], Meaning = p[3], Source = SourceNames.TryGetValue(p[4], out source) ? source : p[4] };
                }
            }
            return result;
        }

        // 없으면 null
        public KeyNote Find(string path, string form, bool allowWildcard)
        {
            foreach (string key in KeyPattern.Candidates(path, allowWildcard))
            {
                KeyNote note;
                if (map.TryGetValue(key + "\t" + form, out note)) return note;
            }
            return null;
        }
    }
}
```

- [ ] **Step 6: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== KeyAnnotations ok`, 마지막 줄 `failed=0`.

- [ ] **Step 7: 실제 주석 파일을 읽어 본다**

Run (pwsh):

```powershell
Add-Type -Path E:\CTAToybox\editor\CtaToybox.Core\bin\Release\net8.0\CtaToybox.Core.dll
foreach ($g in 'human','vehicle','weapon-ammo','inventory','shared') { "$g " + [CtaToybox.Core.KeyAnnotations]::Load('E:\CTAToybox\data\annotations', $g).Count }
[CtaToybox.Core.KeyAnnotations]::Load('E:\CTAToybox\data\annotations', 'vehicle').Find('/game_entity/chassis/("mobility_tank")', 'macro', $true)
```

Expected: 그룹마다 600개 안팎의 수가 찍히고 예외가 없다. 마지막 줄은 `$null`이거나 주석 하나다. 예외가 나면 그 줄을 `NEEDS_CONTEXT`로 보고한다(주석 파일을 고치지 않는다).

- [ ] **Step 8: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): read key annotations in C# with the same lookup rules as the docs

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 2: 아이템 이름 → 파일

**Files:**
- Create: `editor/CtaToybox.Core/ItemLinks.cs`
- Create: `editor/CtaToybox.Core.Tests/ItemLinksTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`
- Modify: `editor/CtaToybox.Core.Tests/RealFilesCheck.cs`

**Interfaces:**
- Consumes: `SdlCore.KeyRows`, `SdlNode` (기존).
- Produces (네임스페이스 `CtaToybox.Core`):
  - `sealed class ItemLink { string Name; string Rule; List<string> Files; }` (속성). `Rule`은 상수 하나.
  - `sealed class ItemLinks`: 상수 `RuleName = "이름"`, `RuleWords = "낱말"`, `RuleWordsNoExt = "확장자 뺀 낱말"`, `RuleCandidates = "후보(미확인)"`, `RuleNone = "없음"`. `static bool IsItemFile(string virtualPath)`, `static ItemLinks Build(IEnumerable<string> virtualPaths)`, `ItemLink Find(string name)`, `static List<string> ReferencedNames(IEnumerable<object> expandedNodes)`.
  - 앞의 세 규칙은 파일이 정확히 하나일 때만 쓴다. 둘 이상이면 그 파일들을 `RuleCandidates`로 낸다. 세 규칙이 모두 없으면 이름의 낱말이 파일 낱말의 부분집합인 파일들을 `RuleCandidates`로, 그것도 없으면 `RuleNone`.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/ItemLinksTests.cs`:

```csharp
using System.Linq;
using CtaToybox.Core;

static class ItemLinksTests
{
    public static void Run()
    {
        T.True(ItemLinks.IsItemFile("set/stuff/smg/m3_grease"), "a stuff file without extension");
        T.True(ItemLinks.IsItemFile("set/stuff/shell/bulletrus_85.aphebc.ammo"), "an ammo file");
        T.True(!ItemLinks.IsItemFile("set/stuff/smg/.presets"), ".presets is not an item");
        T.True(!ItemLinks.IsItemFile("set/stuff/smg/smg.pattern"), "a pattern is not an item");
        T.True(!ItemLinks.IsItemFile("set/stuff/smg/x.inc"), "an .inc is not an item");
        T.True(!ItemLinks.IsItemFile("set/stuff/_settings/a"), "_settings is not items");
        T.True(!ItemLinks.IsItemFile("entity/-vehicle/a/a.def"), "outside set/stuff is not an item file");

        var links = ItemLinks.Build(new[]
        {
            "set/stuff/smg/m3_grease", "set/stuff/smg/m3_grease_mag.ammo", "set/stuff/shell/bulletrus_85.aphebc.ammo",
            "set/stuff/mgun/mgun_usa.belt.ammo", "set/stuff/mgun/mgun_ger.belt.ammo", "set/stuff/mgun/mgun_ger.drum.ammo",
            "set/stuff/a/dup", "set/stuff/b/dup", "set/stuff/smg/.presets", "entity/-vehicle/m3_grease/m3_grease.def",
        });

        var l = links.Find("m3_grease");
        T.Eq(ItemLinks.RuleName, l.Rule, "a name equal to a file name");
        T.Eq(new[] { "set/stuff/smg/m3_grease" }, l.Files.ToArray(), "links to that file, not to the entity def");
        T.Eq(ItemLinks.RuleWords, links.Find("m3_grease_mag ammo").Rule, "same words in another order");
        T.Eq(ItemLinks.RuleWords, links.Find("ammo m3_grease_mag").Rule, "same words, other order");
        T.Eq(new[] { "set/stuff/smg/m3_grease_mag.ammo" }, links.Find("ammo m3_grease_mag").Files.ToArray(), "to the ammo file");
        T.Eq(ItemLinks.RuleWordsNoExt, links.Find("bulletrus_85 aphebc").Rule, "same words without the extension");
        T.Eq(ItemLinks.RuleWordsNoExt, links.Find("BULLETRUS_85  APHEBC").Rule, "case and extra spaces do not matter");
        var family = links.Find("ammo mgun_ger");
        T.Eq(ItemLinks.RuleCandidates, family.Rule, "a family name has candidates");
        T.Eq(new[] { "set/stuff/mgun/mgun_ger.belt.ammo", "set/stuff/mgun/mgun_ger.drum.ammo" }, family.Files.ToArray(), "every file whose words contain the name's words, sorted");
        T.Eq(ItemLinks.RuleCandidates, links.Find("ammo mgun_usa").Rule, "one subset candidate is still only a candidate");
        T.Eq(ItemLinks.RuleCandidates, links.Find("dup").Rule, "the same file name in two folders is a candidate set");
        T.Eq(2, links.Find("dup").Files.Count, "with both files");
        var none = links.Find("kordr");
        T.Eq(ItemLinks.RuleNone, none.Rule, "no file at all");
        T.Eq(0, none.Files.Count, "and no files");

        var nodes = SdlCore.Parse(
            "{game_entity {Weaponry {place \"gun\" {weapon \"85mm_d5t\" filling \"bulletrus_85 aphebc\" 1}}}" +
            " {extender \"inventory\" {box {item \"m3_grease\" filled} {item \"m3_grease_mag ammo\" 90} {item \"m3_grease\" filled}}}" +
            " {weapon \"mgun\" {mask \"mountable\"}} {name \"not an item\"}}").Root.Children;
        T.Eq(new[] { "85mm_d5t", "bulletrus_85 aphebc", "m3_grease", "m3_grease_mag ammo", "mgun" }, ItemLinks.ReferencedNames(nodes).ToArray(),
            "names from item and weapon rows and after filling, lower-cased, distinct, in order");
    }
}
```

`Program.cs`의 suites 목록에서 `("KeyAnnotations", KeyAnnotationsTests.Run),` 다음 줄에 더한다:

```csharp
            ("ItemLinks", ItemLinksTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `ItemLinks`가 없다.

- [ ] **Step 3: `ItemLinks.cs`를 쓴다**

```csharp
// 유닛이 이름으로 가리키는 아이템(무기, 탄약 등)을 set/stuff의 파일로 잇는다.
// 규칙은 2026-10-14의 데이터 조사에서 나왔다(docs/02-modding-research.md의 "아이템 이름과 파일"):
//   1. 파일 이름과 같다  2. 이름의 낱말과 파일 이름을 점으로 나눈 낱말이 같다  3. 파일의 마지막 조각(확장자)을 빼면 같다
// 세 규칙으로 파일이 정확히 하나일 때만 이어졌다고 본다. 나머지는 후보로만 보인다(게임이 어떻게 고르는지 미확인).
using System;
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class ItemLink
    {
        public string Name { get; set; }
        public string Rule { get; set; }
        public List<string> Files { get; set; } = new List<string>();
    }

    public sealed class ItemLinks
    {
        public const string RuleName = "이름", RuleWords = "낱말", RuleWordsNoExt = "확장자 뺀 낱말", RuleCandidates = "후보(미확인)", RuleNone = "없음";

        readonly Dictionary<string, List<string>> byLeaf = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> byWords = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly Dictionary<string, List<string>> byWordsNoExt = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        readonly List<KeyValuePair<string, HashSet<string>>> all = new List<KeyValuePair<string, HashSet<string>>>();

        // tools/research/Measure-ItemNameLinks.ps1과 같은 범위: set/stuff 아래, _settings와 설정·틀 파일 제외
        public static bool IsItemFile(string virtualPath)
        {
            if (!virtualPath.StartsWith("set/stuff/", StringComparison.Ordinal) || virtualPath.StartsWith("set/stuff/_settings/", StringComparison.Ordinal)) return false;
            string leaf = virtualPath.Substring(virtualPath.LastIndexOf('/') + 1);
            if (leaf.StartsWith(".")) return false;
            foreach (string ext in new[] { ".pattern", ".inc", ".fsm", ".hand", ".presets" })
                if (leaf.EndsWith(ext, StringComparison.Ordinal)) return false;
            return true;
        }

        static string Key(IEnumerable<string> words) { return string.Join(" ", words.OrderBy(w => w, StringComparer.Ordinal)); }

        static void Add(Dictionary<string, List<string>> map, string key, string file)
        {
            List<string> list;
            if (!map.TryGetValue(key, out list)) { list = new List<string>(); map[key] = list; }
            list.Add(file);
        }

        public static ItemLinks Build(IEnumerable<string> virtualPaths)
        {
            var links = new ItemLinks();
            foreach (string p in virtualPaths.Where(IsItemFile).OrderBy(x => x, StringComparer.Ordinal))
            {
                string leaf = p.Substring(p.LastIndexOf('/') + 1);
                string[] words = leaf.Split('.');
                Add(links.byLeaf, leaf, p);
                Add(links.byWords, Key(words), p);
                if (words.Length > 1) Add(links.byWordsNoExt, Key(words.Take(words.Length - 1)), p);
                links.all.Add(new KeyValuePair<string, HashSet<string>>(p, new HashSet<string>(words, StringComparer.Ordinal)));
            }
            return links;
        }

        public ItemLink Find(string name)
        {
            string lower = name.Trim().ToLowerInvariant();
            string[] words = lower.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string key = Key(words);
            var rules = new[]
            {
                new KeyValuePair<string, Dictionary<string, List<string>>>(RuleName, byLeaf),
                new KeyValuePair<string, Dictionary<string, List<string>>>(RuleWords, byWords),
                new KeyValuePair<string, Dictionary<string, List<string>>>(RuleWordsNoExt, byWordsNoExt),
            };
            foreach (var rule in rules)
            {
                List<string> files;
                if (!rule.Value.TryGetValue(rule.Key == RuleName ? lower : key, out files)) continue;
                return new ItemLink { Name = lower, Rule = files.Count == 1 ? rule.Key : RuleCandidates, Files = new List<string>(files) };
            }
            var candidates = all.Where(f => words.All(f.Value.Contains)).Select(f => f.Key).ToList();
            return new ItemLink { Name = lower, Rule = candidates.Count > 0 ? RuleCandidates : RuleNone, Files = candidates };
        }

        // 펼친 노드에서 아이템 이름을 모은다: item·weapon 행의 첫 문자열과 filling 뒤의 문자열
        public static List<string> ReferencedNames(IEnumerable<object> expandedNodes)
        {
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var row in SdlCore.KeyRows(expandedNodes.ToList(), new[] { "extender" }))
            {
                string seg = row.Path.Substring(row.Path.LastIndexOf('/') + 1);
                var args = row.ArgNodes;
                for (int i = 0; i < args.Count; i++)
                {
                    bool take = args[i].Kind == "str" &&
                        ((i == 0 && (seg == "item" || seg == "weapon")) || (i > 0 && args[i - 1].Kind == "atom" && args[i - 1].Text == "filling"));
                    if (!take) continue;
                    string name = args[i].Text.Trim('"').ToLowerInvariant();
                    if (seen.Add(name)) result.Add(name);
                }
            }
            return result;
        }
    }
}
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== ItemLinks ok`, 마지막 줄 `failed=0`.

- [ ] **Step 5: 실제 원본으로 조사 결과와 맞는지 본다**

`editor/CtaToybox.Core.Tests/RealFilesCheck.cs`의 `Run` 끝(`m3a1` 검사 다음)에 더한다:

```csharp
        // 아이템 이름 규칙: 데이터 조사(docs/02-modding-research.md)와 같은 범위에서 몇 개가 파일 하나로 이어지는지
        var items = ItemLinks.Build(stack.Paths);
        var itemNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var u in index.Units.Where(x => x.Kind == "vehicle" || x.Kind == "breed"))
        {
            var parsedUnit = SdlCore.Parse(stack.ReadText(u.Path));
            if (parsedUnit.Errors.Count == 0) foreach (string n in ItemLinks.ReferencedNames(parsedUnit.Root.Children)) itemNames.Add(n);
        }
        var byRule = itemNames.Select(items.Find).GroupBy(l => l.Rule).ToDictionary(g => g.Key, g => g.Count());
        Console.WriteLine("item-names=" + itemNames.Count + " " + string.Join(" ", byRule.OrderBy(k => k.Key).Select(k => k.Key + "=" + k.Value)));
        int linked = new[] { ItemLinks.RuleName, ItemLinks.RuleWords, ItemLinks.RuleWordsNoExt }.Sum(r => byRule.TryGetValue(r, out int c) ? c : 0);
        T.True(linked * 100 >= itemNames.Count * 95, "at least 95% of item names link to exactly one file, as the survey found");
        T.Eq("set/stuff/smg/m3_grease", items.Find("m3_grease").Files.Single(), "the in-game-confirmed link");
```

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`
Expected: `item-names=` 줄이 찍히고 `failed=0`. 이 검사는 15분 남짓 걸린다(차량 전체 추적이 앞에 있다). 찍힌 줄을 그대로 보고에 옮긴다. 조사 문서의 수(2,411개 중 2,341개)와 조금 다를 수 있다: 조사 스크립트는 파일 이름이 두 폴더에 같으면 다음 규칙으로 넘어갔고, 이 코드는 후보로 낸다.

- [ ] **Step 6: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): link item names in units to set/stuff files by the surveyed rules

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 3: 요약 탭의 키

**Files:**
- Create: `editor/CtaToybox.Core/SummaryKeys.cs`
- Create: `editor/CtaToybox.Core.Tests/SummaryKeysTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `KeyPattern.Candidates`, `KeyAnnotations.Entries` (Task 1).
- Produces: `sealed class SummaryKeys`: `static SummaryKeys Load(string summaryDir, string kind, KeyAnnotations notes)`, `string SourceFile`(파일에서 읽었으면 그 경로, 주석에서 만들었으면 `null`), `int Count`, `string LabelOf(string expandedPath)`(요약에 없으면 `null`), `static string ShortLabel(string meaning)`.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/SummaryKeysTests.cs`:

```csharp
using System.IO;
using CtaToybox.Core;

static class SummaryKeysTests
{
    public static void Run()
    {
        T.Eq("무게", SummaryKeys.ShortLabel("무게. 킬로그램"), "the label stops at the first period");
        T.Eq("최고 속도", SummaryKeys.ShortLabel("최고 속도, km/h"), "or at a comma");
        T.Eq("관통 피해 기준", SummaryKeys.ShortLabel("관통 피해 기준 (값은 mm)"), "or at a parenthesis");
        T.Eq("가나다라마바사아자차카타파하가나다라마바사아자차카타파하가나…", SummaryKeys.ShortLabel("가나다라마바사아자차카타파하가나다라마바사아자차카타파하가나다라마바사"), "long labels are cut at 30 characters");

        string ann = T.TempDir(), summary = T.TempDir();
        File.WriteAllText(Path.Combine(ann, "common.psv"),
            "*/maxspeed|block|확인|최고 속도, km/h|C\n" +
            "**/armor/front|block|확인|전면 장갑. mm|C\n" +
            "*/mass|block|추정|무게|S\n");
        var notes = KeyAnnotations.Load(ann, "vehicle");

        var fromNotes = SummaryKeys.Load(summary, "vehicle", notes);
        T.True(fromNotes.SourceFile == null, "without a summary file the keys come from annotations");
        T.Eq(2, fromNotes.Count, "only 확인 entries");
        T.Eq("최고 속도", fromNotes.LabelOf("/game_entity/chassis/(\"mobility_tank\")/maxspeed"), "*/name matches expanded paths");
        T.Eq("전면 장갑", fromNotes.LabelOf("/game_entity/props/armor/front"), "**/tail matches");
        T.True(fromNotes.LabelOf("/game_entity/mass") == null, "a 추정 key is not in the summary");

        File.WriteAllText(Path.Combine(summary, "vehicle.psv"), "# 요약 키\n**/box/item|적재품\n/game_entity/mass|무게(kg)\n");
        var fromFile = SummaryKeys.Load(summary, "vehicle", notes);
        T.Eq(Path.Combine(summary, "vehicle.psv"), fromFile.SourceFile, "a summary file wins");
        T.Eq(2, fromFile.Count, "its entries");
        T.Eq("무게(kg)", fromFile.LabelOf("/game_entity/mass"), "an exact path");
        T.Eq("적재품", fromFile.LabelOf("/game_entity/extender[inventory]/box/item"), "a tail");
        T.True(fromFile.LabelOf("/game_entity/chassis/maxspeed") == null, "annotation keys are not added when the file exists");
        T.Eq(2, SummaryKeys.Load(summary, "breed", notes).Count, "another kind without a file falls back to annotations");
    }
}
```

`Program.cs`의 suites 목록에서 `("ItemLinks", ItemLinksTests.Run),` 다음 줄에 더한다:

```csharp
            ("SummaryKeys", SummaryKeysTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `SummaryKeys`가 없다.

- [ ] **Step 3: `SummaryKeys.cs`를 쓴다**

```csharp
// 요약 탭에 올릴 키와 그 이름. editor/summary/<종류>.psv(경로|이름)가 있으면 그것을 쓰고,
// 없으면 키 주석에서 등급이 "확인"인 키를 쓴다(스펙: 첫 버전은 common.psv의 "확인" 키로 시작한다).
// 경로는 주석과 같은 방식으로 맞춘다(정확한 경로, **/꼬리, */이름).
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CtaToybox.Core
{
    public sealed class SummaryKeys
    {
        const int MaxLabel = 30;
        readonly Dictionary<string, string> labels = new Dictionary<string, string>(StringComparer.Ordinal);

        public string SourceFile { get; private set; }
        public int Count { get { return labels.Count; } }

        public static string ShortLabel(string meaning)
        {
            string s = meaning ?? "";
            foreach (string stop in new[] { ". ", ", ", " (", "(" })
            {
                int i = s.IndexOf(stop, StringComparison.Ordinal);
                if (i > 0) s = s.Substring(0, i);
            }
            s = s.TrimEnd('.', ' ');
            return s.Length > MaxLabel ? s.Substring(0, MaxLabel) + "…" : s;
        }

        public static SummaryKeys Load(string summaryDir, string kind, KeyAnnotations notes)
        {
            var keys = new SummaryKeys();
            string file = Path.Combine(summaryDir, kind + ".psv");
            if (File.Exists(file))
            {
                keys.SourceFile = file;
                foreach (string line in File.ReadLines(file, Encoding.UTF8))
                {
                    if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                    string[] p = line.Split('|', 2);
                    if (p.Length < 2) throw new FormatException("요약 키 줄 형식 오류: " + line);
                    keys.labels[p[0]] = p[1];
                }
                return keys;
            }
            foreach (var e in notes.Entries)
            {
                if (e.Value.Grade != "확인") continue;
                string path = e.Key.Substring(0, e.Key.IndexOf('\t'));
                if (!keys.labels.ContainsKey(path)) keys.labels[path] = ShortLabel(e.Value.Meaning);
            }
            return keys;
        }

        // 요약에 없으면 null
        public string LabelOf(string expandedPath)
        {
            foreach (string key in KeyPattern.Candidates(expandedPath, true))
            {
                string label;
                if (labels.TryGetValue(key, out label)) return label;
            }
            return null;
        }
    }
}
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== SummaryKeys ok`, 마지막 줄 `failed=0`.

- [ ] **Step 5: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): summary keys from editor/summary or confirmed annotations

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 4: 편집 세션 — 열기와 유닛 보기

**Files:**
- Modify: `editor/CtaToybox.Core/UnitTracer.cs`
- Create: `editor/CtaToybox.Core/EditorSession.cs`
- Create: `editor/CtaToybox.Core.Tests/EditorSessionTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `UnitTracer`, `UnitIndex.LoadOrBuild`, `.UsersOf`, `.Search`, `DisplayNames`, `GameProfile`, `IncludeChain.UnitScopeBlocker` (3단계). `KeyAnnotations` (Task 1), `ItemLinks` (Task 2), `SummaryKeys` (Task 3).
- Produces (네임스페이스 `CtaToybox.Core`):
  - `TraceRow.Form` (`string`), `UnitTraceResult.Nodes` (`List<object>`).
  - `sealed class ValueView { TraceValue Trace; int SharedBy; string UnitScopeBlocker; Edit Edit; bool Editable (=> Trace.Arg >= 0); }`
  - `sealed class RowView { string Path; int Occurrence; string Form; KeyNote Note; string SummaryLabel; List<ValueView> Values; }`
  - `sealed class UnitView { string Unit; string Kind; string Layer; string DisplayName; List<RowView> Rows; List<string> Unresolved; List<ItemLink> Items; List<SdlIncludeEdge> IncludeEdges; }`
  - `sealed class EditorSession : IDisposable`:
    - `static EditorSession Open(string settingsPath, string repoRoot, Action<string> progress)`: 설정이나 `options.set`이 없으면 `FileNotFoundException`(메시지에 경로).
    - 속성: `EditorSettings Settings`, `LayerPlan Plan`, `SourceStack Stack`, `UnitIndex Index`, `DisplayNames Names`, `ItemLinks Items`, `EditProject Project`, `string ProjectPath`.
    - `IEnumerable<UnitEntry> Search(string text, string kind, string layer)`.
    - `UnitView OpenUnit(string unit)`: 없으면 `null`.
    - `void OpenProject(string path)`: 없으면 빈 프로젝트(이름은 파일 이름). 아직 쓰지 않는다. 깨진 JSON은 `InvalidDataException`.
    - `Edit EditFor(UnitView unit, ValueView value)`: 그 자리에 걸린 수정. `unit` 범위면 그 유닛의 것만.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/EditorSessionTests.cs`:

```csharp
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class EditorSessionTests
{
    public const string A = "entity/-vehicle/x/a/a.def", B = "entity/-vehicle/x/a/b.def", S = "entity/-vehicle/x/a/s.inc";

    // 게임 폴더, 창작마당, options.set, 설정 파일, 저장소 루트(data/annotations, data/display-names.csv)를 가짜로 만든다
    public static string Fixture(out string repo, out string outDir)
    {
        string game = T.TempDir(), workshop = T.TempDir(), work = T.TempDir();
        repo = T.TempDir(); outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "entity", "-vehicle.pak"),
            ("-vehicle/x/a/a.def", "{game_entity\n (include \"s.inc\")\n {mass 10}\n {weapon \"gun_a\" filling \"gun_a_mag ammo\" 5}\n}"),
            ("-vehicle/x/a/b.def", "{game_entity (include \"s.inc\")}"),
            ("-vehicle/x/a/s.inc", "{box {item \"mine\" 24}}"));
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"),
            ("set/stuff/smg/gun_a", "{filling \"ammo gun_a_mag\" 30}"),
            ("set/stuff/smg/gun_a_mag.ammo", "{bullet {mass 0.02}}"));
        Directory.CreateDirectory(Path.Combine(repo, "data", "annotations"));
        File.WriteAllText(Path.Combine(repo, "data", "annotations", "common.psv"),
            "*/mass|block|확인|무게. 킬로그램|C\n**/box/item|block|추정|적재품 한 칸|S\n");
        File.WriteAllText(Path.Combine(repo, "data", "display-names.csv"), "\"Context\",\"Text\",\"File\"\n\"desc/entity/a\",\"Tank A\",\"x.pot\"\n");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    public static void Run()
    {
        string repo, outDir;
        string settings = Fixture(out repo, out outDir);

        T.Throws<FileNotFoundException>(() => EditorSession.Open(Path.Combine(repo, "none.json"), repo, null), "a missing settings file");

        var messages = new System.Collections.Generic.List<string>();
        using var session = EditorSession.Open(settings, repo, messages.Add);
        T.True(messages.Count > 0, "progress messages are reported");
        T.True(session.Plan.OutputModActive, "the output mod is active in the fixture");
        T.Eq(new[] { A, B }, session.Search(null, "vehicle", null).Select(u => u.Path).ToArray(), "vehicles");
        T.Eq(new[] { A }, session.Search("tank a", null, null).Select(u => u.Path).ToArray(), "search by display name");
        T.True(session.OpenUnit("entity/-vehicle/x/none.def") == null, "a missing unit is null");

        var a = session.OpenUnit(A);
        T.Eq("Tank A", a.DisplayName, "display name");
        T.Eq("vehicle", a.Kind, "kind");
        T.Eq("vanilla", a.Layer, "layer");

        var mass = a.Rows.Single(r => r.Path == "/game_entity/mass");
        T.Eq("block", mass.Form, "row form");
        T.Eq("확인", mass.Note.Grade, "annotation of the row");
        T.Eq("무게", mass.SummaryLabel, "a 확인 key is in the summary");
        T.Eq(1, mass.Values[0].SharedBy, "a value in the unit's own file is used by one unit");
        T.True(mass.Values[0].UnitScopeBlocker == null, "and can be unit-scoped");

        var item = a.Rows.Single(r => r.Path == "/game_entity/box/item");
        T.Eq("추정", item.Note.Grade, "a tail annotation");
        T.True(item.SummaryLabel == null, "a 추정 key is not in the summary");
        var mine = item.Values[1];
        T.Eq("24", mine.Trace.Text, "the value");
        T.Eq(S, mine.Trace.File, "comes from the include");
        T.Eq(2, mine.SharedBy, "shared by both units");
        T.True(mine.UnitScopeBlocker == null, "unit scope is possible through the include");
        T.True(mine.Edit == null, "no edit yet");

        T.Eq(new[] { "mine", "gun_a", "gun_a_mag ammo" }, a.Items.Select(i => i.Name).ToArray(), "referenced item names, in expansion order (the include comes first)");
        T.Eq(ItemLinks.RuleNone, a.Items[0].Rule, "mine has no file in the fixture");
        T.Eq(ItemLinks.RuleName, a.Items[1].Rule, "gun_a links by name");
        T.Eq("set/stuff/smg/gun_a", a.Items[1].Files.Single(), "to the stuff file");
        T.Eq(ItemLinks.RuleWords, a.Items[2].Rule, "the ammo links by words");

        var gun = session.OpenUnit("set/stuff/smg/gun_a");
        T.Eq("stuff", gun.Kind, "a linked stuff file opens as a unit");

        string projectPath = Path.Combine(repo, "editor", "projects", "p.json");
        session.OpenProject(projectPath);
        T.Eq("p", session.Project.Name, "a missing project starts empty, named after the file");
        T.True(!File.Exists(projectPath), "and is not written until something changes");
        File.WriteAllText(Path.Combine(repo, "broken.json"), "{ not json");
        T.Throws<InvalidDataException>(() => session.OpenProject(Path.Combine(repo, "broken.json")), "a broken project is reported, not thrown as JsonException");
        T.Eq(projectPath, session.ProjectPath, "a failed open keeps the previous project");
    }
}
```

`Program.cs`의 suites 목록에서 `("SummaryKeys", SummaryKeysTests.Run),` 다음 줄에 더한다:

```csharp
            ("EditorSession", EditorSessionTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `EditorSession`이 없다.

- [ ] **Step 3: `UnitTracer`에 두 속성을 더한다**

`editor/CtaToybox.Core/UnitTracer.cs`에서:

`TraceRow`의 `public int Occurrence { get; set; }     // 펼친 뒤의 순번` 다음 줄에 더한다:

```csharp
        public string Form { get; set; }        // block | macro (KeyLocator의 행 형태). 주석을 찾을 때 쓴다
```

`UnitTraceResult`의 `public List<SdlIncludeEdge> IncludeEdges { get; set; }` 다음 줄에 더한다:

```csharp
        public List<object> Nodes { get; set; }   // 펼친 노드. 아이템 이름을 모을 때 쓴다
```

`Trace`에서 두 줄을 바꾼다:

```csharp
            var result = new UnitTraceResult { Unit = unit, Unresolved = resolved.Unresolved, IncludeEdges = resolved.IncludeEdges, Nodes = resolved.Nodes };
```

```csharp
                var traced = new TraceRow { Path = row.Path, Occurrence = row.Occurrence, Form = row.Form };
```

- [ ] **Step 4: `EditorSession.cs`를 쓴다**

```csharp
// 화면이 쓰는 편집 상태와 동작. 화면은 이 클래스만 부르고 원본을 직접 읽거나 파일을 쓰지 않는다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CtaToybox.Core
{
    public sealed class ValueView
    {
        public TraceValue Trace { get; set; }
        public int SharedBy { get; set; }             // 그 파일을 읽는 유닛 수
        public string UnitScopeBlocker { get; set; }  // "이 유닛만"이 안 되는 이유. 되면 null
        public Edit Edit { get; set; }                // 이 자리에 걸린 수정
        public bool Editable { get { return Trace.Arg >= 0; } }
    }

    public sealed class RowView
    {
        public string Path { get; set; }
        public int Occurrence { get; set; }
        public string Form { get; set; }
        public KeyNote Note { get; set; }             // 키 설명. 없으면 null
        public string SummaryLabel { get; set; }      // 요약 탭의 이름. 요약에 없으면 null
        public List<ValueView> Values { get; } = new List<ValueView>();
    }

    public sealed class UnitView
    {
        public string Unit { get; set; }
        public string Kind { get; set; }
        public string Layer { get; set; }
        public string DisplayName { get; set; }
        public List<RowView> Rows { get; } = new List<RowView>();
        public List<string> Unresolved { get; set; }
        public List<ItemLink> Items { get; set; }
        public List<SdlIncludeEdge> IncludeEdges { get; set; }
    }

    public sealed partial class EditorSession : IDisposable
    {
        public EditorSettings Settings { get; private set; }
        public LayerPlan Plan { get; private set; }
        public SourceStack Stack { get; private set; }
        public UnitIndex Index { get; private set; }
        public DisplayNames Names { get; private set; }
        public ItemLinks Items { get; private set; }
        public EditProject Project { get; private set; }
        public string ProjectPath { get; private set; }

        string annotationDir, summaryDir;
        UnitTracer tracer;
        readonly Dictionary<string, KeyAnnotations> notes = new Dictionary<string, KeyAnnotations>(StringComparer.Ordinal);
        readonly Dictionary<string, SummaryKeys> summaries = new Dictionary<string, SummaryKeys>(StringComparer.Ordinal);

        // repoRoot는 data/와 editor/가 있는 저장소 루트다
        public static EditorSession Open(string settingsPath, string repoRoot, Action<string> progress)
        {
            Action<string> say = progress ?? (s => { });
            if (!File.Exists(settingsPath)) throw new FileNotFoundException("설정 파일이 없다: " + settingsPath, settingsPath);
            var s = new EditorSession { Settings = EditorSettings.Load(settingsPath) };
            if (!File.Exists(s.Settings.OptionsSetPath)) throw new FileNotFoundException("options.set이 없다: " + s.Settings.OptionsSetPath, s.Settings.OptionsSetPath);
            say("원본 계층을 읽는 중");
            s.Plan = GameProfile.PlanLayers(s.Settings, GameProfile.ReadModEntries(File.ReadAllText(s.Settings.OptionsSetPath)));
            s.Stack = new SourceStack(s.Plan.Layers);
            s.Stack.Scan();
            say("유닛 색인을 여는 중");
            string cache = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "cache", "unit-index.json");
            bool rebuilt;
            s.Index = UnitIndex.LoadOrBuild(s.Stack, cache, (done, total) => say("유닛 색인을 만드는 중: " + done + "/" + total), out rebuilt);
            if (rebuilt) say("유닛 색인을 새로 만들었다: " + s.Index.Units.Count + "개");
            s.Names = DisplayNames.Load(Path.Combine(repoRoot, "data", "display-names.csv"));
            s.Items = ItemLinks.Build(s.Stack.Paths);
            s.annotationDir = Path.Combine(repoRoot, "data", "annotations");
            s.summaryDir = Path.Combine(repoRoot, "editor", "summary");
            s.tracer = new UnitTracer(s.Stack);
            say("준비됐다");
            return s;
        }

        public IEnumerable<UnitEntry> Search(string text, string kind, string layer)
        {
            return Index.Search(text, kind, layer, Names);
        }

        KeyAnnotations NotesFor(string unit)
        {
            string group = KeyAnnotations.GroupOf(unit);
            KeyAnnotations n;
            if (!notes.TryGetValue(group, out n)) { n = KeyAnnotations.Load(annotationDir, group); notes[group] = n; }
            return n;
        }

        SummaryKeys SummaryFor(string kind, KeyAnnotations n)
        {
            string key = kind ?? "(none)";
            SummaryKeys s;
            if (!summaries.TryGetValue(key, out s)) { s = SummaryKeys.Load(summaryDir, key, n); summaries[key] = s; }
            return s;
        }

        // 없으면 null
        public UnitView OpenUnit(string unit)
        {
            var traced = tracer.Trace(unit);
            if (traced == null) return null;
            string kind = UnitKind.Of(traced.Unit);
            var n = NotesFor(traced.Unit);
            var summary = SummaryFor(kind, n);
            var view = new UnitView
            {
                Unit = traced.Unit, Kind = kind, Layer = Stack.Find(traced.Unit).Layer, DisplayName = Names.OfUnit(traced.Unit),
                Unresolved = traced.Unresolved, IncludeEdges = traced.IncludeEdges,
                Items = ItemLinks.ReferencedNames(traced.Nodes).Select(Items.Find).ToList()
            };
            foreach (var row in traced.Rows)
            {
                var r = new RowView { Path = row.Path, Occurrence = row.Occurrence, Form = row.Form, Note = n.Find(row.Path, row.Form, true), SummaryLabel = summary.LabelOf(row.Path) };
                foreach (var v in row.Values)
                {
                    var vv = new ValueView { Trace = v };
                    if (v.Arg >= 0)
                    {
                        vv.SharedBy = Index.UsersOf(v.File).Count;
                        vv.UnitScopeBlocker = IncludeChain.UnitScopeBlocker(traced.IncludeEdges, traced.Unit, v.File, v.KeyPath);
                    }
                    r.Values.Add(vv);
                }
                view.Rows.Add(r);
            }
            RefreshEdits(view);
            return view;
        }

        // 화면의 유닛에 걸린 수정을 다시 잇는다(수정을 더하거나 지운 뒤)
        public void RefreshEdits(UnitView view)
        {
            foreach (var row in view.Rows)
                foreach (var v in row.Values) v.Edit = v.Editable ? EditFor(view, v) : null;
        }

        static bool SameSlot(Edit e, TraceValue v)
        {
            return string.Equals(SourceStack.ToVirtual(e.File ?? ""), v.File, StringComparison.Ordinal)
                && string.Equals(e.Path, v.KeyPath, StringComparison.OrdinalIgnoreCase) && e.Occurrence == v.Occurrence && e.Arg == v.Arg;
        }

        // 그 자리에 걸린 수정. '이 유닛만' 수정은 그 유닛의 것만 본다. 둘 다 있으면 '이 유닛만'이 이긴다(빌드와 같다)
        public Edit EditFor(UnitView unit, ValueView value)
        {
            if (Project == null || !value.Editable) return null;
            var mine = Project.Edits.FirstOrDefault(e => e.Scope == "unit" && SourceStack.ToVirtual(e.Unit ?? "") == unit.Unit && SameSlot(e, value.Trace));
            return mine ?? Project.Edits.FirstOrDefault(e => e.Scope != "unit" && SameSlot(e, value.Trace));
        }

        // 없는 파일은 빈 프로젝트로 시작한다(바뀌는 것이 있을 때 쓴다). 깨진 파일은 InvalidDataException이고 지금 프로젝트는 그대로다
        public void OpenProject(string path)
        {
            EditProject loaded;
            if (File.Exists(path))
            {
                try { loaded = EditProject.Load(path); }
                catch (JsonException ex) { throw new InvalidDataException("프로젝트 파일을 읽지 못했다: " + path + ": " + ex.Message, ex); }
                if (loaded == null) throw new InvalidDataException("프로젝트 파일이 비어 있다: " + path);
                if (loaded.Edits == null) loaded.Edits = new List<Edit>();
            }
            else loaded = new EditProject { Name = Path.GetFileNameWithoutExtension(path) };
            Project = loaded;
            ProjectPath = path;
        }

        public void Dispose()
        {
            if (Stack != null) Stack.Dispose();
        }
    }
}
```

`partial`인 이유: Task 5가 수정·빌드 동작을 `EditorSession.Edits.cs`에 나눠 둔다.

- [ ] **Step 5: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== EditorSession ok`, 마지막 줄 `failed=0`. 기존 `UnitTracer`, `Cli` 묶음도 그대로 통과한다.

- [ ] **Step 6: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): editor session that opens sources, index and units for the app

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 5: 편집 세션 — 수정, 검사, 빌드

**Files:**
- Create: `editor/CtaToybox.Core/EditorSession.Edits.cs`
- Create: `editor/CtaToybox.Core.Tests/EditorSessionEditTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: Task 4의 `EditorSession`, `UnitView`, `ValueView`, `EditFor`, `RefreshEdits`. 기존 `TokenPatcher.CheckNewValue`, `ModBuilder`, `KeyLocator`, `IncludeChain`.
- Produces (`EditorSession`의 메서드):
  - `string ValidateNew(UnitView unit, ValueView value, string newText, string scope, out string warning)`: 안 되면 이유, 되면 `null`.
  - `Edit AddEdit(UnitView unit, ValueView value, string newText, string scope)`: 안 되면 `ArgumentException`(메시지가 이유). 같은 자리·같은 범위(·같은 유닛)의 수정이 있으면 그것을 바꾼다. 프로젝트를 저장한다.
  - `void RemoveEdit(Edit edit)`, `void SetEnabled(Edit edit, bool enabled)`, `void Rebase(Edit edit)`(old를 원본의 지금 값으로): 모두 저장한다.
  - `List<EditCheck> CheckEdits()`: 켜진 수정마다 지금 원본 기준의 상태. `sealed class EditCheck { Edit Edit; EditStatus Status; string Current; string Message; }`.
  - `BuildReport Build()`, `static bool GameRunning()`.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/EditorSessionEditTests.cs`:

```csharp
using System;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class EditorSessionEditTests
{
    public static void Run()
    {
        string repo, outDir;
        string settings = EditorSessionTests.Fixture(out repo, out outDir);
        using var session = EditorSession.Open(settings, repo, null);
        string projectPath = Path.Combine(repo, "editor", "projects", "p.json");
        session.OpenProject(projectPath);

        var a = session.OpenUnit(EditorSessionTests.A);
        var mass = a.Rows.Single(r => r.Path == "/game_entity/mass").Values[0];
        var mine = a.Rows.Single(r => r.Path == "/game_entity/box/item").Values[1];
        string warning;

        T.True(session.ValidateNew(a, mass, "12", "shared", out warning) == null && warning == null, "a number for a number is fine");
        T.True(session.ValidateNew(a, mass, "fast", "shared", out warning) == null && warning != null, "a word for a number is a warning");
        T.True(session.ValidateNew(a, mass, "1 2", "shared", out warning) != null, "two tokens are refused");
        T.True(session.ValidateNew(a, mass, "12", "everyone", out warning) != null, "an unknown scope is refused");
        var gunName = a.Rows.Single(r => r.Path == "/game_entity/weapon").Values[0];
        T.True(session.ValidateNew(a, gunName, "\"gun_b\"", "shared", out warning) == null, "a quoted value for a quoted value");

        T.Throws<ArgumentException>(() => session.AddEdit(a, mass, "1 2", "shared"), "AddEdit refuses what ValidateNew refuses");
        T.True(!File.Exists(projectPath), "a refused edit does not write the project");

        var e1 = session.AddEdit(a, mass, "12", "shared");
        T.True(File.Exists(projectPath), "adding an edit saves the project");
        T.Eq("10", e1.Old, "old is the current source value");
        T.Eq(EditorSessionTests.A, e1.Unit, "the unit is recorded");
        session.RefreshEdits(a);
        T.True(mass.Edit == e1, "the value now shows its edit");

        var e2 = session.AddEdit(a, mine, "5", "unit");
        T.Eq("unit", e2.Scope, "unit scope");
        T.Eq(EditorSessionTests.S, e2.File, "the edit points at the include file");
        var e3 = session.AddEdit(a, mine, "6", "unit");
        T.True(e3 == e2, "the same slot and scope replaces, not adds");
        T.Eq("6", e2.New, "with the new value");
        T.Eq(2, session.Project.Edits.Count, "two edits");
        T.Eq(2, EditProject.Load(projectPath).Edits.Count, "two edits on disk");

        var b = session.OpenUnit(EditorSessionTests.B);
        var bMine = b.Rows.Single(r => r.Path == "/game_entity/box/item").Values[1];
        T.True(bMine.Edit == null, "another unit does not see a unit-scoped edit of a");
        var shared = session.AddEdit(b, bMine, "30", "shared");
        session.RefreshEdits(a);
        T.True(mine.Edit == e2, "for a, its unit-scoped edit wins over the shared one on the same slot");
        session.RefreshEdits(b);
        T.True(bMine.Edit == shared, "for b, the shared edit shows");

        // "이 유닛만"이 안 되는 자리: 화면이 막지 못해도 세션이 막는다
        var blocked = new ValueView { Trace = mine.Trace, UnitScopeBlocker = "이유" };
        T.Throws<ArgumentException>(() => session.AddEdit(a, blocked, "7", "unit"), "a blocked slot cannot be unit-scoped");

        var checks = session.CheckEdits();
        T.Eq(3, checks.Count, "one check per enabled edit");
        T.True(checks.All(c => c.Status == EditStatus.Applied), "all apply to the current sources");
        T.Eq("10", checks.Single(c => c.Edit == e1).Current, "current source value is reported");

        e1.Old = "99";
        var stale = session.CheckEdits().Single(c => c.Edit == e1);
        T.Eq(EditStatus.SourceChanged, stale.Status, "a stale old value is SourceChanged");
        T.Eq("10", stale.Current, "with the current value");
        session.Rebase(e1);
        T.Eq("10", e1.Old, "rebase sets old to the current value");
        T.Eq("10", EditProject.Load(projectPath).Edits.Single(e => e.Path == "/game_entity/mass").Old, "and saves");

        var report = session.Build();
        T.Eq(3, report.Count(EditStatus.Applied), "build applies all three");
        T.True(File.Exists(Path.Combine(outDir, "resource", "entity", "-vehicle", "x", "a", "a.def")), "the unit file is written");

        session.SetEnabled(e1, false);
        T.Eq(false, EditProject.Load(projectPath).Edits.Single(e => e.Path == "/game_entity/mass").Enabled, "disabling saves");
        T.Eq(2, session.CheckEdits().Count, "a disabled edit is not checked");
        session.RemoveEdit(shared);
        T.Eq(2, EditProject.Load(projectPath).Edits.Count, "removing saves");
        session.RemoveEdit(e2);
        session.RemoveEdit(e1);
        report = session.Build();
        T.Eq(0, report.Count(EditStatus.Applied), "nothing to apply");
        T.True(!Directory.EnumerateFiles(Path.Combine(outDir, "resource"), "*", SearchOption.AllDirectories).Any(), "and the outputs are removed");

        T.True(EditorSession.GameRunning() == (System.Diagnostics.Process.GetProcessesByName("call_to_arms").Length > 0 || System.Diagnostics.Process.GetProcessesByName("call_to_arms_ed").Length > 0), "GameRunning looks at the game and the editor");
    }
}
```

`Program.cs`의 suites 목록에서 `("EditorSession", EditorSessionTests.Run),` 다음 줄에 더한다:

```csharp
            ("EditorSessionEdit", EditorSessionEditTests.Run),
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류. `ValidateNew`, `AddEdit` 등이 없다.

- [ ] **Step 3: `EditorSession.Edits.cs`를 쓴다**

```csharp
// 편집 세션의 수정 목록 동작: 더하기, 바꾸기, 끄기, 지우기, 지금 원본으로 검사하기, 빌드. 바뀌면 프로젝트 파일을 바로 쓴다.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class EditCheck
    {
        public Edit Edit { get; set; }
        public EditStatus Status { get; set; }   // Applied는 "지금 원본에 적용할 수 있다"는 뜻이다
        public string Current { get; set; }      // 원본의 지금 값. 자리가 없으면 null
        public string Message { get; set; }
    }

    public sealed partial class EditorSession
    {
        public string ValidateNew(UnitView unit, ValueView value, string newText, string scope, out string warning)
        {
            warning = null;
            if (!value.Editable) return value.Trace.Note ?? "편집할 수 없는 자리다";
            if (scope != "shared" && scope != "unit") return "범위는 shared 또는 unit이다: " + scope;
            if (scope == "unit" && value.UnitScopeBlocker != null) return value.UnitScopeBlocker;
            return TokenPatcher.CheckNewValue(value.Trace.Text, newText, out warning);
        }

        public Edit AddEdit(UnitView unit, ValueView value, string newText, string scope)
        {
            string warning;
            string error = ValidateNew(unit, value, newText, scope, out warning);
            if (error != null) throw new ArgumentException(error);
            var t = value.Trace;
            var existing = Project.Edits.FirstOrDefault(e => e.Scope == scope && SameSlot(e, t) && (scope != "unit" || SourceStack.ToVirtual(e.Unit ?? "") == unit.Unit));
            if (existing != null)
            {
                existing.New = newText;
                existing.Enabled = true;
                Save();
                return existing;
            }
            var edit = new Edit { Unit = unit.Unit, File = t.File, Path = t.KeyPath, Occurrence = t.Occurrence, Arg = t.Arg, Old = t.Text, New = newText, Scope = scope };
            Project.Edits.Add(edit);
            Save();
            return edit;
        }

        public void RemoveEdit(Edit edit) { Project.Edits.Remove(edit); Save(); }

        public void SetEnabled(Edit edit, bool enabled) { edit.Enabled = enabled; Save(); }

        // old를 원본의 지금 값으로 바꾼다. 자리가 없어졌으면 그대로 둔다
        public void Rebase(Edit edit)
        {
            var check = Check(edit, new Dictionary<string, SdlNode>(StringComparer.Ordinal));
            if (check.Current == null) return;
            edit.Old = check.Current;
            Save();
        }

        public List<EditCheck> CheckEdits()
        {
            var parsed = new Dictionary<string, SdlNode>(StringComparer.Ordinal);
            return Project.Edits.Where(e => e.Enabled).Select(e => Check(e, parsed)).ToList();
        }

        EditCheck Check(Edit e, Dictionary<string, SdlNode> parsed)
        {
            string file = SourceStack.ToVirtual(e.File ?? "");
            SdlNode root;
            if (!parsed.TryGetValue(file, out root))
            {
                string text = Stack.ReadText(file);
                var p = text == null ? null : SdlCore.Parse(text);
                root = p == null || p.Errors.Count > 0 ? null : p.Root;
                parsed[file] = root;
            }
            if (root == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 파싱되지 않는다: " + file };
            var node = KeyLocator.Find(root, e.Path, e.Occurrence, e.Arg);
            if (node == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 자리가 없다" };
            if (node.Text != e.Old) return new EditCheck { Edit = e, Status = EditStatus.SourceChanged, Current = node.Text, Message = "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text };
            if (e.Scope == "unit")
            {
                string unit = SourceStack.ToVirtual(e.Unit ?? "");
                if (Stack.Find(unit) == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Current = node.Text, Message = "원본에 이 유닛이 없다: " + unit };
                string blocker = IncludeChain.UnitScopeBlocker(new SdlResolver(Stack.ReadText).Resolve(unit).IncludeEdges, unit, file, e.Path);
                if (blocker != null) return new EditCheck { Edit = e, Status = EditStatus.Invalid, Current = node.Text, Message = blocker };
            }
            return new EditCheck { Edit = e, Status = EditStatus.Applied, Current = node.Text };
        }

        public BuildReport Build()
        {
            return new ModBuilder(Stack, Settings.OutputModDir).Build(Project);
        }

        public static bool GameRunning()
        {
            return Process.GetProcessesByName("call_to_arms").Length > 0 || Process.GetProcessesByName("call_to_arms_ed").Length > 0;
        }

        void Save() { Project.Save(ProjectPath); }
    }
}
```

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== EditorSessionEdit ok`, 마지막 줄 `failed=0`.

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 모두 통과.

- [ ] **Step 5: 커밋한다**

```bash
git -C E:/CTAToybox add editor/CtaToybox.Core editor/CtaToybox.Core.Tests
git -C E:/CTAToybox commit -m "feat(editor): session edits, checks against current sources, rebase and build

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 6: 앱 뼈대 — 설정, 불러오기, 유닛 찾기, 스크린샷

**Files:**
- Create: `editor/CtaToybox.App/CtaToybox.App.csproj`
- Create: `editor/CtaToybox.App/App.xaml`, `App.xaml.cs`
- Create: `editor/CtaToybox.App/AppOptions.cs`
- Create: `editor/CtaToybox.App/SettingsDialog.xaml`, `SettingsDialog.xaml.cs`
- Create: `editor/CtaToybox.App/MainWindow.xaml`, `MainWindow.xaml.cs`
- Create: `editor/CtaToybox.App/Items.cs`
- Modify: `tools/tests/Run-Tests.ps1`

**Interfaces:**
- Consumes: `EditorSession` (Task 4, 5).
- Produces:
  - 실행 파일 `CtaToybox.exe`. 옵션: `--repo <폴더>`(기본: 현재 폴더에서 위로 올라가며 `editor`와 `data`가 둘 다 있는 폴더), `--settings <파일>`(기본 `<repo>/editor/settings.json`), `--project <파일>`(기본 `<repo>/editor/projects/my-tweaks.json`), `--unit <가상 경로>`(열고 시작할 유닛), `--tab summary|full`, `--screenshot <png>`(불러오고 `--unit`을 연 뒤 창을 PNG로 저장하고 끝낸다).
  - `MainWindow`의 이름 붙은 요소(이후 Task가 쓴다): `ProjectText`, `SourcesText`, `WarningText`, `StatusText`, `EditCountText`, `BuildButton`, `SearchBox`, `KindBox`, `LayerBox`, `ResultCountText`, `UnitList`, `UnitTitle`, `UnitInfo`, `Tabs`, `SummaryTab`, `FullTab`, `SummaryList`, `FullFilter`, `FullList`, `ItemList`, `EditList`.
  - `MainWindow`의 메서드: `void ShowUnit(string unit)`, `void RefreshUnitViews()`(Task 7, 9가 채운다), `void RefreshEdits()`(Task 8이 채운다).

- [ ] **Step 1: 프로젝트 파일을 만든다**

`editor/CtaToybox.App/CtaToybox.App.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <LangVersion>12</LangVersion>
    <AssemblyName>CtaToybox</AssemblyName>
    <RootNamespace>CtaToybox.App</RootNamespace>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\CtaToybox.Core\CtaToybox.Core.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: 옵션과 앱 진입점을 쓴다**

`editor/CtaToybox.App/AppOptions.cs`:

```csharp
// 명령줄 옵션. 저장소 루트를 찾고 기본 경로를 정한다.
using System;
using System.Collections.Generic;
using System.IO;

namespace CtaToybox.App
{
    public sealed class AppOptions
    {
        public string Repo, Settings, Project, Unit, Tab, Screenshot;

        public static AppOptions Parse(string[] args)
        {
            var opt = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("--")) throw new ArgumentException("모르는 인자: " + args[i]);
                if (i + 1 >= args.Length) throw new ArgumentException(args[i] + " 뒤에 값이 없다");
                opt[args[i].Substring(2)] = args[++i];
            }
            foreach (string k in opt.Keys)
                if (Array.IndexOf(new[] { "repo", "settings", "project", "unit", "tab", "screenshot" }, k.ToLowerInvariant()) < 0) throw new ArgumentException("모르는 옵션: --" + k);
            var o = new AppOptions();
            o.Repo = Path.GetFullPath(opt.ContainsKey("repo") ? opt["repo"] : FindRepo(Environment.CurrentDirectory) ?? FindRepo(AppContext.BaseDirectory) ?? Environment.CurrentDirectory);
            o.Settings = opt.ContainsKey("settings") ? opt["settings"] : Path.Combine(o.Repo, "editor", "settings.json");
            o.Project = opt.ContainsKey("project") ? opt["project"] : Path.Combine(o.Repo, "editor", "projects", "my-tweaks.json");
            o.Unit = opt.ContainsKey("unit") ? opt["unit"] : null;
            o.Tab = opt.ContainsKey("tab") ? opt["tab"] : null;
            o.Screenshot = opt.ContainsKey("screenshot") ? Path.GetFullPath(opt["screenshot"]) : null;
            return o;
        }

        // editor와 data가 둘 다 있는 가장 가까운 상위 폴더
        static string FindRepo(string start)
        {
            for (var d = new DirectoryInfo(start); d != null; d = d.Parent)
                if (Directory.Exists(Path.Combine(d.FullName, "editor")) && Directory.Exists(Path.Combine(d.FullName, "data"))) return d.FullName;
            return null;
        }
    }
}
```

`editor/CtaToybox.App/App.xaml`:

```xml
<Application x:Class="CtaToybox.App.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             ShutdownMode="OnMainWindowClose">
</Application>
```

`editor/CtaToybox.App/App.xaml.cs`:

```csharp
// 앱 진입점. 설정 파일이 없으면 설정 창을 먼저 띄운다.
using System;
using System.IO;
using System.Windows;

namespace CtaToybox.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppOptions options;
            try { options = AppOptions.Parse(e.Args); }
            catch (ArgumentException ex) { MessageBox.Show(ex.Message, "CTAToybox"); Shutdown(2); return; }

            if (!File.Exists(options.Settings))
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var dialog = new SettingsDialog(options.Settings, options.Repo);
                if (dialog.ShowDialog() != true) { Shutdown(1); return; }
                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            var window = new MainWindow(options);
            MainWindow = window;
            window.Show();
        }
    }
}
```

- [ ] **Step 3: 설정 창을 쓴다**

`editor/CtaToybox.App/SettingsDialog.xaml`:

```xml
<Window x:Class="CtaToybox.App.SettingsDialog"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="CTAToybox 설정" Width="760" SizeToContent="Height" WindowStartupLocation="CenterScreen" ResizeMode="NoResize">
  <StackPanel Margin="12">
    <TextBlock TextWrapping="Wrap" Margin="0,0,0,8">경로를 정한다. 게임 폴더, 창작마당 폴더, options.set은 읽기만 한다. 이 설정은 editor/settings.json에 저장되고 git에 넣지 않는다.</TextBlock>
    <TextBlock Text="게임 폴더 (resource와 mods가 있는 곳)"/>
    <TextBox x:Name="GameBox" Margin="0,2,0,8"/>
    <TextBlock Text="창작마당 폴더 (...\steamapps\workshop\content\400750)"/>
    <TextBox x:Name="WorkshopBox" Margin="0,2,0,8"/>
    <TextBlock Text="프로필의 options.set"/>
    <TextBox x:Name="OptionsBox" Margin="0,2,0,8"/>
    <TextBlock Text="출력 모드 폴더 (mod.info가 있는 곳)"/>
    <TextBox x:Name="OutBox" Margin="0,2,0,8"/>
    <TextBlock x:Name="ErrorText" Foreground="#B00020" TextWrapping="Wrap"/>
    <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,8,0,0">
      <Button Content="저장" Width="80" IsDefault="True" Click="Save_Click"/>
      <Button Content="취소" Width="80" Margin="8,0,0,0" IsCancel="True"/>
    </StackPanel>
  </StackPanel>
</Window>
```

`editor/CtaToybox.App/SettingsDialog.xaml.cs`:

```csharp
// 설정 파일이 없을 때 경로를 받는다. 프로필의 options.set은 찾을 수 있으면 미리 채운다(offline 프로필은 뺀다).
using System;
using System.IO;
using System.Linq;
using System.Windows;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class SettingsDialog : Window
    {
        readonly string settingsPath;

        public SettingsDialog(string settingsPath, string repo)
        {
            InitializeComponent();
            this.settingsPath = settingsPath;
            string profiles = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "digitalmindsoft", "gates of hell", "profiles");
            if (Directory.Exists(profiles))
            {
                var found = Directory.GetDirectories(profiles).Where(d => !string.Equals(Path.GetFileName(d), "offline", StringComparison.OrdinalIgnoreCase))
                    .Select(d => Path.Combine(d, "options.set")).FirstOrDefault(File.Exists);
                if (found != null) OptionsBox.Text = found;
            }
            OutBox.Text = Path.Combine(repo, "mod", "CTAToybox");
        }

        void Save_Click(object sender, RoutedEventArgs e)
        {
            string game = GameBox.Text.Trim(), workshop = WorkshopBox.Text.Trim(), options = OptionsBox.Text.Trim(), output = OutBox.Text.Trim();
            if (!Directory.Exists(Path.Combine(game, "resource"))) { ErrorText.Text = "게임 폴더에 resource 폴더가 없다: " + game; return; }
            if (!Directory.Exists(workshop)) { ErrorText.Text = "창작마당 폴더가 없다: " + workshop; return; }
            if (!File.Exists(options)) { ErrorText.Text = "options.set이 없다: " + options; return; }
            if (!File.Exists(Path.Combine(output, "mod.info"))) { ErrorText.Text = "출력 모드 폴더에 mod.info가 없다: " + output; return; }
            string full = Path.GetFullPath(output).TrimEnd(Path.DirectorySeparatorChar);
            new EditorSettings { GameDir = Path.GetFullPath(game), WorkshopDir = Path.GetFullPath(workshop), OptionsSetPath = Path.GetFullPath(options), OutputModDir = full, OutputModName = Path.GetFileName(full) }.Save(settingsPath);
            DialogResult = true;
        }
    }
}
```

- [ ] **Step 4: 목록 항목 클래스를 쓴다**

`editor/CtaToybox.App/Items.cs`:

```csharp
// 화면 목록이 묶는 항목. 표시 문자열만 만들고 판단은 하지 않는다(판단은 EditorSession이 한다).
using CtaToybox.Core;

namespace CtaToybox.App
{
    public sealed class UnitItem
    {
        public UnitEntry Entry { get; set; }
        public string Label { get; set; }
    }

    public sealed class ValueItem
    {
        public RowView Row { get; set; }
        public ValueView Value { get; set; }
        public string Group { get; set; }      // 전체 탭의 묶음: 값이 있는 파일
        public string Key { get; set; }        // 펼친 키 경로 [순번] 또는 요약 이름
        public string Text { get; set; }      // 원본 값
        public string NewText { get; set; }    // 걸린 수정의 새 값과 범위. 없으면 빈칸
        public string Origin { get; set; }     // 파일 안의 자리
        public string Shared { get; set; }     // 공유 수
        public string Scope { get; set; }      // "이 유닛만" 가능 여부
        public string Note { get; set; }       // 키 설명과 등급
    }

    public sealed class ItemLinkItem
    {
        public ItemLink Link { get; set; }
        public string Label { get; set; }
    }

    public sealed class EditItem
    {
        public Edit Edit { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }
    }
}
```

- [ ] **Step 5: 주 창을 쓴다**

`editor/CtaToybox.App/MainWindow.xaml`:

```xml
<Window x:Class="CtaToybox.App.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="CTAToybox 스탯 편집기" Width="1500" Height="900" WindowStartupLocation="CenterScreen"
        Background="White" FontFamily="Malgun Gothic" FontSize="13">
  <DockPanel>
    <Border DockPanel.Dock="Top" Padding="8" BorderBrush="#DDDDDD" BorderThickness="0,0,0,1">
      <DockPanel>
        <StackPanel DockPanel.Dock="Right" Orientation="Horizontal" VerticalAlignment="Center">
          <TextBlock x:Name="EditCountText" VerticalAlignment="Center" Margin="0,0,12,0"/>
          <Button x:Name="BuildButton" Content="빌드" Padding="18,4" IsEnabled="False" Click="Build_Click"/>
        </StackPanel>
        <StackPanel>
          <StackPanel Orientation="Horizontal">
            <TextBlock Text="프로젝트: "/>
            <TextBlock x:Name="ProjectText" FontWeight="Bold"/>
            <Button Content="열기..." Margin="8,0,0,0" Padding="6,0" Click="OpenProject_Click"/>
            <TextBlock Text="    원본: "/>
            <TextBlock x:Name="SourcesText"/>
          </StackPanel>
          <TextBlock x:Name="WarningText" Foreground="#B00020" TextWrapping="Wrap"/>
          <TextBlock x:Name="StatusText" Foreground="#555555"/>
        </StackPanel>
      </DockPanel>
    </Border>
    <Grid>
      <Grid.ColumnDefinitions>
        <ColumnDefinition Width="320"/>
        <ColumnDefinition Width="5"/>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="5"/>
        <ColumnDefinition Width="380"/>
      </Grid.ColumnDefinitions>

      <DockPanel Grid.Column="0" Margin="6">
        <StackPanel DockPanel.Dock="Top">
          <TextBlock Text="유닛 찾기" FontWeight="Bold"/>
          <TextBox x:Name="SearchBox" Margin="0,4" TextChanged="Search_Changed" IsEnabled="False"/>
          <DockPanel Margin="0,2"><TextBlock Text="종류" Width="40" VerticalAlignment="Center"/><ComboBox x:Name="KindBox" SelectionChanged="Search_Changed" IsEnabled="False"/></DockPanel>
          <DockPanel Margin="0,2"><TextBlock Text="출처" Width="40" VerticalAlignment="Center"/><ComboBox x:Name="LayerBox" SelectionChanged="Search_Changed" IsEnabled="False"/></DockPanel>
          <TextBlock x:Name="ResultCountText" Foreground="#555555" Margin="0,2"/>
        </StackPanel>
        <ListBox x:Name="UnitList" DisplayMemberPath="Label" SelectionChanged="UnitList_SelectionChanged"
                 VirtualizingPanel.IsVirtualizing="True" VirtualizingPanel.VirtualizationMode="Recycling"/>
      </DockPanel>

      <GridSplitter Grid.Column="1" HorizontalAlignment="Stretch" Background="#EEEEEE"/>

      <DockPanel Grid.Column="2" Margin="6">
        <StackPanel DockPanel.Dock="Top">
          <TextBlock x:Name="UnitTitle" FontSize="16" FontWeight="Bold" Text="유닛을 고른다"/>
          <TextBlock x:Name="UnitInfo" Foreground="#555555" TextWrapping="Wrap"/>
        </StackPanel>
        <TabControl x:Name="Tabs" Margin="0,6,0,0">
          <TabItem x:Name="SummaryTab" Header="요약">
            <ListView x:Name="SummaryList" MouseDoubleClick="Value_DoubleClick"/>
          </TabItem>
          <TabItem x:Name="FullTab" Header="전체">
            <DockPanel>
              <DockPanel DockPanel.Dock="Top" Margin="0,4">
                <TextBlock Text="거르기 " VerticalAlignment="Center"/>
                <TextBox x:Name="FullFilter" TextChanged="FullFilter_Changed"/>
              </DockPanel>
              <Expander DockPanel.Dock="Bottom" Header="참조하는 아이템 (무기·탄약 등)" IsExpanded="True" Margin="0,4,0,0">
                <ListBox x:Name="ItemList" DisplayMemberPath="Label" Height="140" MouseDoubleClick="ItemList_DoubleClick"/>
              </Expander>
              <ListView x:Name="FullList" MouseDoubleClick="Value_DoubleClick"/>
            </DockPanel>
          </TabItem>
        </TabControl>
      </DockPanel>

      <GridSplitter Grid.Column="3" HorizontalAlignment="Stretch" Background="#EEEEEE"/>

      <DockPanel Grid.Column="4" Margin="6">
        <TextBlock DockPanel.Dock="Top" Text="수정 목록" FontWeight="Bold"/>
        <WrapPanel DockPanel.Dock="Bottom" Margin="0,4,0,0">
          <Button Content="삭제" Padding="8,2" Margin="0,0,6,4" Click="RemoveEdit_Click"/>
          <Button Content="지금 값에 맞추기" Padding="8,2" Margin="0,0,6,4" Click="RebaseEdit_Click"/>
          <Button Content="유닛 열기" Padding="8,2" Margin="0,0,6,4" Click="OpenEditUnit_Click"/>
        </WrapPanel>
        <ListView x:Name="EditList"/>
      </DockPanel>
    </Grid>
  </DockPanel>
</Window>
```

`editor/CtaToybox.App/MainWindow.xaml.cs`:

```csharp
// 주 창. 세션을 백그라운드에서 열고, 유닛 찾기를 맡는다. 유닛 보기는 MainWindow.Unit.cs, 수정 목록은 MainWindow.Edits.cs에 있다.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class MainWindow : Window
    {
        const int MaxResults = 500;
        const string AllChoice = "(전체)";

        readonly AppOptions options;
        EditorSession session;
        UnitView unit;

        public MainWindow(AppOptions options)
        {
            InitializeComponent();
            this.options = options;
            Loaded += async (s, e) => await LoadSession();
            Closed += (s, e) => { if (session != null) session.Dispose(); };
        }

        void Say(string message) { Dispatcher.Invoke(() => StatusText.Text = message); }

        async Task LoadSession()
        {
            try
            {
                session = await Task.Run(() => EditorSession.Open(options.Settings, options.Repo, Say));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Text.Json.JsonException || ex is InvalidDataException)
            {
                WarningText.Text = "원본을 열지 못했다: " + ex.Message;
                if (options.Screenshot != null) { SaveScreenshot(options.Screenshot); Close(); }
                return;
            }
            SourcesText.Text = string.Join(" → ", session.Plan.Layers.Select(l => l.Name)) + "  (출력 모드 " + session.Settings.OutputModName + (session.Plan.OutputModActive ? ", 켜져 있음" : ", 꺼져 있음") + ")";
            WarningText.Text = string.Join("\n", session.Plan.Warnings);
            KindBox.ItemsSource = new[] { AllChoice }.Concat(UnitKind.All).ToList();
            KindBox.SelectedIndex = 0;
            LayerBox.ItemsSource = new[] { AllChoice }.Concat(session.Plan.Layers.Select(l => l.Name)).ToList();
            LayerBox.SelectedIndex = 0;
            SearchBox.IsEnabled = KindBox.IsEnabled = LayerBox.IsEnabled = BuildButton.IsEnabled = true;
            OpenProjectFile(options.Project);
            RunSearch();
            if (options.Unit != null) ShowUnit(options.Unit);
            if (options.Tab == "full") Tabs.SelectedItem = FullTab;
            if (options.Screenshot != null)
            {
                await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle);
                SaveScreenshot(options.Screenshot);
                Close();
            }
        }

        void Search_Changed(object sender, EventArgs e) { if (session != null) RunSearch(); }

        void RunSearch()
        {
            string text = SearchBox.Text.Trim();
            string kind = KindBox.SelectedItem as string, layer = LayerBox.SelectedItem as string;
            var found = session.Search(text.Length == 0 ? null : text, kind == AllChoice ? null : kind, layer == AllChoice ? null : layer).ToList();
            UnitList.ItemsSource = found.Take(MaxResults).Select(u =>
            {
                string name = session.Names.OfUnit(u.Path);
                return new UnitItem { Entry = u, Label = (name == null ? "" : name + "  ") + u.Path.Substring(u.Path.LastIndexOf('/') + 1) + "  [" + u.Kind + ", " + u.Layer + "]" };
            }).ToList();
            ResultCountText.Text = found.Count > MaxResults ? found.Count + "개 가운데 " + MaxResults + "개. 검색어를 좁힌다." : found.Count + "개";
        }

        void UnitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = UnitList.SelectedItem as UnitItem;
            if (item != null) ShowUnit(item.Entry.Path);
        }

        // PNG로 창 내용을 저장한다. 화면 확인을 사람 대신 할 때 쓴다(--screenshot)
        void SaveScreenshot(string path)
        {
            UpdateLayout();
            var content = (FrameworkElement)Content;
            var dpi = VisualTreeHelper.GetDpi(this);
            var bitmap = new RenderTargetBitmap((int)(content.ActualWidth * dpi.DpiScaleX), (int)(content.ActualHeight * dpi.DpiScaleY), 96 * dpi.DpiScaleX, 96 * dpi.DpiScaleY, PixelFormats.Pbgra32);
            bitmap.Render(content);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var f = File.Create(path)) encoder.Save(f);
        }
    }
}
```

`MainWindow.Unit.cs`와 `MainWindow.Edits.cs`는 Task 7, 8이 채운다. 이 Task에서는 빌드가 되도록 빈 부분을 둔다.

`editor/CtaToybox.App/MainWindow.Unit.cs`:

```csharp
// 유닛 보기: 요약 탭, 전체 탭, 참조하는 아이템, 값 고치기
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        public void ShowUnit(string path)
        {
            unit = session.OpenUnit(path);
            UnitTitle.Text = unit == null ? "원본에 이 파일이 없다: " + path : unit.Unit;
        }

        public void RefreshUnitViews() { }
        void FullFilter_Changed(object sender, TextChangedEventArgs e) { }
        void ItemList_DoubleClick(object sender, MouseButtonEventArgs e) { }
        void Value_DoubleClick(object sender, MouseButtonEventArgs e) { }
    }
}
```

`editor/CtaToybox.App/MainWindow.Edits.cs`:

```csharp
// 수정 목록, 프로젝트 열기, 빌드
using System;
using System.IO;
using System.Windows;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        void OpenProjectFile(string path)
        {
            try { session.OpenProject(path); }
            catch (InvalidDataException ex) { MessageBox.Show(ex.Message, "프로젝트"); }
            ProjectText.Text = session.Project == null ? "(없음)" : session.Project.Name;
        }

        public void RefreshEdits() { }
        void OpenProject_Click(object sender, RoutedEventArgs e) { }
        void Build_Click(object sender, RoutedEventArgs e) { }
        void RemoveEdit_Click(object sender, RoutedEventArgs e) { }
        void RebaseEdit_Click(object sender, RoutedEventArgs e) { }
        void OpenEditUnit_Click(object sender, RoutedEventArgs e) { }
    }
}
```

- [ ] **Step 6: 테스트 스크립트에 앱 빌드를 넣는다**

`tools/tests/Run-Tests.ps1`의 `if ($failed -gt 0) ...` 줄 바로 앞에 더한다:

```powershell
Write-Host '== editor/CtaToybox.App (build)'
dotnet build (Join-Path $PSScriptRoot '../../editor/CtaToybox.App') -c Release --nologo -v quiet
if ($LASTEXITCODE -ne 0) { $failed++ }
```

- [ ] **Step 7: 빌드하고 스크린샷으로 확인한다**

Run: `dotnet build editor/CtaToybox.App -c Release`
Expected: 경고 0개, 오류 0개.

Run(저장소 루트에서, 게임이 꺼져 있지 않아도 된다. 읽기만 한다):

```bash
editor/CtaToybox.App/bin/Release/net8.0-windows/CtaToybox.exe --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --screenshot .superpowers/screens/task6.png
```

Expected: 몇 초 뒤 앱이 스스로 닫히고 PNG가 생긴다. Read 도구로 PNG를 연다. 위쪽에 프로젝트 이름(`my-tweaks`)과 원본 구성(`vanilla → mod_3732639687 → ... → mod_3617250510`, `출력 모드 CTAToybox, 켜져 있음`)이, 왼쪽에 유닛 목록과 개수가, 가운데 제목에 `entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def`가 보여야 한다. 다르면 그림에서 본 것을 그대로 적고 원인을 찾는다. `.superpowers/`는 git에서 빠져 있다.

- [ ] **Step 8: 전체 테스트를 돌고 커밋한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: `== editor/CtaToybox.App (build)` 포함 모두 통과.

```bash
git -C E:/CTAToybox add editor/CtaToybox.App tools/tests/Run-Tests.ps1
git -C E:/CTAToybox commit -m "feat(editor): WPF app skeleton with settings, background loading, unit search and screenshots

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 7: 전체 탭과 값 고치기 창

**Files:**
- Modify: `editor/CtaToybox.App/MainWindow.Unit.cs` (전체 내용 교체)
- Modify: `editor/CtaToybox.App/MainWindow.xaml` (`FullList`와 `SummaryList`에 열 정의)
- Create: `editor/CtaToybox.App/EditDialog.xaml`, `EditDialog.xaml.cs`

**Interfaces:**
- Consumes: `EditorSession.OpenUnit`, `ValidateNew`, `AddEdit`, `RemoveEdit`, `RefreshEdits`, `Index.UsersOf` (Task 4, 5). `Items.cs`의 `ValueItem`, `ItemLinkItem` (Task 6).
- Produces: `EditDialog(EditorSession session, UnitView unit, RowView row, ValueView value)`. 닫힐 때 `DialogResult == true`면 프로젝트가 바뀌었다.

- [ ] **Step 1: 목록의 열을 정의한다**

`MainWindow.xaml`에서 `<ListView x:Name="SummaryList" MouseDoubleClick="Value_DoubleClick"/>`를 아래로 바꾼다:

```xml
            <ListView x:Name="SummaryList" MouseDoubleClick="Value_DoubleClick">
              <ListView.View>
                <GridView>
                  <GridViewColumn Header="이름" Width="220" DisplayMemberBinding="{Binding Key}"/>
                  <GridViewColumn Header="값" Width="90" DisplayMemberBinding="{Binding Text}"/>
                  <GridViewColumn Header="수정" Width="120" DisplayMemberBinding="{Binding NewText}"/>
                  <GridViewColumn Header="공유" Width="60" DisplayMemberBinding="{Binding Shared}"/>
                  <GridViewColumn Header="이 유닛만" Width="80" DisplayMemberBinding="{Binding Scope}"/>
                  <GridViewColumn Header="원본" Width="420" DisplayMemberBinding="{Binding Origin}"/>
                </GridView>
              </ListView.View>
            </ListView>
```

`<ListView x:Name="FullList" MouseDoubleClick="Value_DoubleClick"/>`를 아래로 바꾼다:

```xml
              <ListView x:Name="FullList" MouseDoubleClick="Value_DoubleClick"
                        VirtualizingPanel.IsVirtualizing="True" VirtualizingPanel.IsVirtualizingWhenGrouping="True" VirtualizingPanel.VirtualizationMode="Recycling">
                <ListView.GroupStyle>
                  <GroupStyle>
                    <GroupStyle.ContainerStyle>
                      <Style TargetType="GroupItem">
                        <Setter Property="Template">
                          <Setter.Value>
                            <ControlTemplate TargetType="GroupItem">
                              <Expander IsExpanded="True">
                                <Expander.Header>
                                  <TextBlock FontWeight="Bold"><Run Text="{Binding Name, Mode=OneWay}"/><Run Text="  "/><Run Text="{Binding ItemCount, Mode=OneWay}"/><Run Text="개"/></TextBlock>
                                </Expander.Header>
                                <ItemsPresenter/>
                              </Expander>
                            </ControlTemplate>
                          </Setter.Value>
                        </Setter>
                      </Style>
                    </GroupStyle.ContainerStyle>
                  </GroupStyle>
                </ListView.GroupStyle>
                <ListView.View>
                  <GridView>
                    <GridViewColumn Header="키 (펼친 뒤)" Width="300" DisplayMemberBinding="{Binding Key}"/>
                    <GridViewColumn Header="값" Width="90" DisplayMemberBinding="{Binding Text}"/>
                    <GridViewColumn Header="수정" Width="120" DisplayMemberBinding="{Binding NewText}"/>
                    <GridViewColumn Header="공유" Width="60" DisplayMemberBinding="{Binding Shared}"/>
                    <GridViewColumn Header="이 유닛만" Width="80" DisplayMemberBinding="{Binding Scope}"/>
                    <GridViewColumn Header="파일 안의 자리" Width="260" DisplayMemberBinding="{Binding Origin}"/>
                    <GridViewColumn Header="설명" Width="360" DisplayMemberBinding="{Binding Note}"/>
                  </GridView>
                </ListView.View>
              </ListView>
```

- [ ] **Step 2: 값 고치기 창을 쓴다**

`editor/CtaToybox.App/EditDialog.xaml`:

```xml
<Window x:Class="CtaToybox.App.EditDialog"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="값 고치기" Width="640" SizeToContent="Height" WindowStartupLocation="CenterOwner" ResizeMode="NoResize"
        FontFamily="Malgun Gothic" FontSize="13">
  <StackPanel Margin="12">
    <TextBlock x:Name="KeyText" FontWeight="Bold" TextWrapping="Wrap"/>
    <TextBlock x:Name="NoteText" Foreground="#555555" TextWrapping="Wrap" Margin="0,2,0,8"/>
    <TextBlock x:Name="OriginText" TextWrapping="Wrap"/>
    <TextBlock x:Name="CurrentText" Margin="0,2,0,8"/>
    <TextBlock x:Name="SharedText" TextWrapping="Wrap"/>
    <ListBox x:Name="UsersList" Height="110" Margin="0,2,0,8"/>
    <StackPanel x:Name="ScopePanel" Margin="0,0,0,8">
      <RadioButton x:Name="SharedRadio" Content="전부 (이 파일을 쓰는 모든 유닛)" IsChecked="True" Checked="Scope_Changed"/>
      <RadioButton x:Name="UnitRadio" Content="이 유닛만" Checked="Scope_Changed"/>
      <TextBlock x:Name="BlockerText" Foreground="#B00020" TextWrapping="Wrap" Margin="18,0,0,0"/>
    </StackPanel>
    <DockPanel>
      <TextBlock Text="새 값 " VerticalAlignment="Center"/>
      <TextBox x:Name="NewBox" TextChanged="NewBox_Changed"/>
    </DockPanel>
    <TextBlock x:Name="MessageText" TextWrapping="Wrap" Margin="0,4,0,0"/>
    <DockPanel Margin="0,10,0,0">
      <Button x:Name="RemoveButton" DockPanel.Dock="Left" Content="이 자리의 수정 지우기" Padding="8,2" Click="Remove_Click"/>
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
        <Button x:Name="OkButton" Content="수정 목록에 넣기" Padding="12,2" IsDefault="True" Click="Ok_Click"/>
        <Button Content="취소" Width="70" Margin="8,0,0,0" IsCancel="True"/>
      </StackPanel>
    </DockPanel>
  </StackPanel>
</Window>
```

`editor/CtaToybox.App/EditDialog.xaml.cs`:

```csharp
// 값 하나를 고친다. 공유되는 값이면 영향받는 유닛을 보이고 "전부 / 이 유닛만"을 고르게 한다.
// "이 유닛만"이 안 되는 자리는 선택지를 끄고 이유를 적는다. 판단은 모두 EditorSession이 한다.
using System;
using System.Linq;
using System.Windows;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class EditDialog : Window
    {
        const int MaxUsers = 300;
        readonly EditorSession session;
        readonly UnitView unit;
        readonly ValueView value;

        public EditDialog(EditorSession session, UnitView unit, RowView row, ValueView value)
        {
            InitializeComponent();
            this.session = session; this.unit = unit; this.value = value;
            var t = value.Trace;
            KeyText.Text = row.Path + " [" + row.Occurrence + "]" + (row.SummaryLabel == null ? "" : "  " + row.SummaryLabel);
            NoteText.Text = row.Note == null ? "설명 없음 (미확인)" : row.Note.Meaning + "  [" + row.Note.Grade + ", " + row.Note.Source + "]";
            OriginText.Text = "원본: " + t.File + "  " + t.KeyPath + " [" + t.Occurrence + "] 값 " + t.Arg;
            CurrentText.Text = "지금 값: " + t.Text;
            var users = session.Index.UsersOf(t.File);
            SharedText.Text = "이 파일을 쓰는 유닛 " + users.Count + "개" + (users.Count > MaxUsers ? " (앞의 " + MaxUsers + "개를 보인다)" : "");
            UsersList.ItemsSource = users.Take(MaxUsers).ToList();
            bool shared = users.Count > 1;
            ScopePanel.Visibility = shared ? Visibility.Visible : Visibility.Collapsed;
            UnitRadio.IsEnabled = value.UnitScopeBlocker == null;
            BlockerText.Text = value.UnitScopeBlocker == null ? "" : "이 유닛만: " + value.UnitScopeBlocker;
            if (value.Edit != null)
            {
                NewBox.Text = value.Edit.New;
                if (value.Edit.Scope == "unit" && UnitRadio.IsEnabled) UnitRadio.IsChecked = true;
            }
            else NewBox.Text = t.Text;
            RemoveButton.Visibility = value.Edit == null ? Visibility.Collapsed : Visibility.Visible;
            Validate();
        }

        string Scope { get { return UnitRadio.IsChecked == true ? "unit" : "shared"; } }

        void Validate()
        {
            if (session == null || MessageText == null || OkButton == null) return;   // InitializeComponent 중에 라디오 단추의 Checked가 먼저 온다
            string warning;
            string error = session.ValidateNew(unit, value, NewBox.Text.Trim(), Scope, out warning);
            MessageText.Text = error ?? (warning == null ? "" : "경고: " + warning);
            MessageText.Foreground = error != null ? System.Windows.Media.Brushes.Firebrick : System.Windows.Media.Brushes.DarkGoldenrod;
            OkButton.IsEnabled = error == null && NewBox.Text.Trim() != value.Trace.Text;
        }

        void NewBox_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e) { Validate(); }
        void Scope_Changed(object sender, RoutedEventArgs e) { Validate(); }

        void Ok_Click(object sender, RoutedEventArgs e)
        {
            try { session.AddEdit(unit, value, NewBox.Text.Trim(), Scope); }
            catch (ArgumentException ex) { MessageText.Text = ex.Message; return; }
            DialogResult = true;
        }

        void Remove_Click(object sender, RoutedEventArgs e)
        {
            session.RemoveEdit(value.Edit);
            DialogResult = true;
        }
    }
}
```

- [ ] **Step 3: 유닛 보기를 채운다**

`editor/CtaToybox.App/MainWindow.Unit.cs`를 아래로 바꾼다:

```csharp
// 유닛 보기: 요약 탭, 전체 탭, 참조하는 아이템, 값 고치기
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CtaToybox.Core;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        const string NotEditableGroup = "(고칠 수 없는 값: 치환·계산으로 만든 값, include 인자)";
        List<ValueItem> fullItems = new List<ValueItem>();

        public void ShowUnit(string path)
        {
            unit = session.OpenUnit(path);
            if (unit == null)
            {
                UnitTitle.Text = "원본에 이 파일이 없다: " + path;
                UnitInfo.Text = "";
                SummaryList.ItemsSource = FullList.ItemsSource = ItemList.ItemsSource = null;
                return;
            }
            UnitTitle.Text = (unit.DisplayName == null ? "" : unit.DisplayName + "  ") + unit.Unit;
            UnitInfo.Text = "종류 " + (unit.Kind ?? "유닛 아님") + ", 계층 " + unit.Layer + ", 값 " + unit.Rows.Sum(r => r.Values.Count) + "개" +
                (unit.Unresolved.Count == 0 ? "" : ", 풀리지 않은 구문 " + unit.Unresolved.Count + "개: " + string.Join(", ", unit.Unresolved.Take(5)));
            ItemList.ItemsSource = unit.Items.Select(i => new ItemLinkItem
            {
                Link = i,
                Label = i.Name + "  →  " + (i.Files.Count == 0 ? "" : string.Join(", ", i.Files.Take(3)) + (i.Files.Count > 3 ? " 외 " + (i.Files.Count - 3) + "개" : "")) + "  (" + i.Rule + ")"
            }).ToList();
            RefreshUnitViews();
        }

        static ValueItem Item(RowView row, ValueView v, int index, bool summary)
        {
            var t = v.Trace;
            string key = summary ? row.SummaryLabel : row.Path + " [" + row.Occurrence + "]";
            if (row.Values.Count > 1) key += " #" + index;
            return new ValueItem
            {
                Row = row, Value = v, Key = key, Text = t.Text,
                Group = v.Editable ? t.File : NotEditableGroup,
                NewText = v.Edit == null ? "" : "→ " + v.Edit.New + (v.Edit.Scope == "unit" ? " (이 유닛만)" : " (전부)") + (v.Edit.Enabled ? "" : " 꺼짐"),
                Origin = v.Editable ? t.KeyPath + " [" + t.Occurrence + "] 값 " + t.Arg : t.Note,
                Shared = v.Editable ? v.SharedBy.ToString() : "",
                Scope = !v.Editable ? "" : v.UnitScopeBlocker == null ? "가능" : "불가",
                Note = row.Note == null ? "" : row.Note.Meaning + " [" + row.Note.Grade + "]",
            };
        }

        public void RefreshUnitViews()
        {
            if (unit == null) return;
            session.RefreshEdits(unit);
            SummaryList.ItemsSource = unit.Rows.Where(r => r.SummaryLabel != null)
                .SelectMany(r => r.Values.Select((v, i) => Item(r, v, i, true))).ToList();
            fullItems = unit.Rows.SelectMany(r => r.Values.Select((v, i) => Item(r, v, i, false))).ToList();
            ApplyFullFilter();
        }

        void ApplyFullFilter()
        {
            string f = FullFilter.Text.Trim();
            var shown = f.Length == 0 ? fullItems : fullItems.Where(x =>
                x.Key.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0 || x.Text.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0 ||
                x.Group.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0 || x.Note.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            var view = new ListCollectionView(shown);
            view.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            FullList.ItemsSource = view;
        }

        void FullFilter_Changed(object sender, TextChangedEventArgs e) { if (unit != null) ApplyFullFilter(); }

        void ItemList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ItemList.SelectedItem as ItemLinkItem;
            if (item == null) return;
            if (item.Link.Files.Count != 1 || item.Link.Rule == ItemLinks.RuleCandidates)
            {
                MessageBox.Show("이 이름은 파일 하나로 이어지지 않는다(" + item.Link.Rule + "). 게임이 어느 파일을 쓰는지 확인하지 않았다.\n\n" + string.Join("\n", item.Link.Files.Take(20)), "아이템");
                return;
            }
            ShowUnit(item.Link.Files[0]);
        }

        void Value_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((ListView)sender).SelectedItem as ValueItem;
            if (item == null || unit == null) return;
            if (!item.Value.Editable) { MessageBox.Show(item.Value.Trace.Note ?? "고칠 수 없는 값이다", "값 고치기"); return; }
            var dialog = new EditDialog(session, unit, item.Row, item.Value) { Owner = this };
            if (dialog.ShowDialog() == true) { RefreshUnitViews(); RefreshEdits(); }
        }
    }
}
```

- [ ] **Step 4: 빌드하고 스크린샷으로 확인한다**

Run: `dotnet build editor/CtaToybox.App -c Release`
Expected: 경고 0개, 오류 0개.

```bash
editor/CtaToybox.App/bin/Release/net8.0-windows/CtaToybox.exe --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --tab full --screenshot .superpowers/screens/task7-full.png
editor/CtaToybox.App/bin/Release/net8.0-windows/CtaToybox.exe --unit entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --tab full --screenshot .superpowers/screens/task7-kv85.png
```

Expected: 두 PNG를 Read로 연다.
- `task7-full.png`: 전체 탭에 파일별 묶음(`entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc`, `properties/armoredcar.ext` 등)이 보이고, `mine m1a1`의 `24` 행에 공유 `4`, 이 유닛만 `가능`이 보인다. 아래 "참조하는 아이템"에 이름과 `(이름)`·`(낱말)` 같은 규칙이 보인다.
- `task7-kv85.png`: 행이 1,000개를 넘는 유닛에서도 창이 그려지고 앱이 스스로 닫혔다(멈추지 않았다). 앱이 30초 안에 닫히지 않으면 멈춘 것이다: 작업 관리자로 끄지 말고 `taskkill /IM CtaToybox.exe`로 끈 뒤 원인을 찾는다.

값 고치기 창은 스크린샷 옵션이 열지 않는다. 창의 동작(검증, 범위 선택, 저장)은 `EditorSession` 테스트(Task 5)가 보장하고, 창 자체는 Task 10의 사람 확인 목록에 넣는다.

- [ ] **Step 5: 전체 테스트를 돌고 커밋한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 모두 통과.

```bash
git -C E:/CTAToybox add editor/CtaToybox.App
git -C E:/CTAToybox commit -m "feat(editor): full tab grouped by source file, item links and the edit dialog

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 8: 수정 목록과 빌드

**Files:**
- Modify: `editor/CtaToybox.App/MainWindow.Edits.cs` (전체 내용 교체)
- Modify: `editor/CtaToybox.App/MainWindow.xaml` (`EditList`에 항목 모양)

**Interfaces:**
- Consumes: `EditorSession.CheckEdits`, `SetEnabled`, `RemoveEdit`, `Rebase`, `Build`, `GameRunning`, `OpenProject` (Task 4, 5). `EditItem` (Task 6).
- Produces: 수정 목록(유닛별 묶음, 체크로 끄고 켜기, 상태 표시), 프로젝트 열기, 빌드 결과 창.

- [ ] **Step 1: 수정 목록의 항목 모양을 정한다**

`MainWindow.xaml`에서 `<ListView x:Name="EditList"/>`를 아래로 바꾼다:

```xml
        <ListView x:Name="EditList" HorizontalContentAlignment="Stretch">
          <ListView.GroupStyle>
            <GroupStyle>
              <GroupStyle.HeaderTemplate>
                <DataTemplate><TextBlock FontWeight="Bold" Margin="0,6,0,2" Text="{Binding Name}" TextWrapping="Wrap"/></DataTemplate>
              </GroupStyle.HeaderTemplate>
            </GroupStyle>
          </ListView.GroupStyle>
          <ListView.ItemTemplate>
            <DataTemplate>
              <DockPanel Margin="0,2">
                <CheckBox DockPanel.Dock="Left" IsChecked="{Binding Enabled, Mode=OneWay}" Click="EditEnabled_Click" Tag="{Binding}" VerticalAlignment="Top" Margin="0,2,6,0"/>
                <StackPanel>
                  <TextBlock Text="{Binding Title}" TextWrapping="Wrap"/>
                  <TextBlock Text="{Binding Detail}" Foreground="#555555" TextWrapping="Wrap"/>
                  <TextBlock Text="{Binding Status}" Foreground="#B00020" TextWrapping="Wrap"/>
                </StackPanel>
              </DockPanel>
            </DataTemplate>
          </ListView.ItemTemplate>
        </ListView>
```

- [ ] **Step 2: 수정 목록 동작을 채운다**

`editor/CtaToybox.App/MainWindow.Edits.cs`를 아래로 바꾼다:

```csharp
// 수정 목록, 프로젝트 열기, 빌드
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CtaToybox.Core;
using Microsoft.Win32;

namespace CtaToybox.App
{
    public partial class MainWindow
    {
        void OpenProjectFile(string path)
        {
            try { session.OpenProject(path); }
            catch (InvalidDataException ex) { MessageBox.Show(ex.Message, "프로젝트"); }
            ProjectText.Text = session.Project == null ? "(없음)" : session.Project.Name + "  (" + session.ProjectPath + ")";
            RefreshEdits();
            RefreshUnitViews();
        }

        public void RefreshEdits()
        {
            if (session == null || session.Project == null) return;
            var checks = session.CheckEdits().ToDictionary(c => c.Edit);
            var items = session.Project.Edits.Select(e =>
            {
                EditCheck c;
                checks.TryGetValue(e, out c);
                string status = !e.Enabled ? "" : c == null ? "" : c.Status == EditStatus.Applied ? "" :
                    (c.Status == EditStatus.SourceChanged ? "원본이 바뀜: 지금 " + c.Current : c.Status == EditStatus.TargetMissing ? "대상 없음: " + c.Message : "적용 불가: " + c.Message);
                return new EditItem
                {
                    Edit = e, Enabled = e.Enabled,
                    Title = e.Path + " [" + e.Occurrence + "] 값 " + e.Arg + ":  " + e.Old + " → " + e.New,
                    Detail = (e.Scope == "unit" ? "이 유닛만" : "전부(유닛 " + session.Index.UsersOf(e.File).Count + "개)") + "  ·  " + e.File,
                    Status = status,
                };
            }).ToList();
            var view = new ListCollectionView(items);
            view.GroupDescriptions.Add(new PropertyGroupDescription(null, new UnitGroupConverter(session)));
            EditList.ItemsSource = view;
            int problems = items.Count(i => i.Status.Length > 0);
            EditCountText.Text = "수정 " + items.Count + "건" + (problems > 0 ? ", 문제 " + problems + "건" : "");
        }

        // 수정 목록을 유닛별로 묶는다
        sealed class UnitGroupConverter : IValueConverter
        {
            readonly EditorSession session;
            public UnitGroupConverter(EditorSession session) { this.session = session; }
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var e = ((EditItem)value).Edit;
                string unit = SourceStack.ToVirtual(e.Unit ?? "");
                string name = session.Names.OfUnit(unit);
                return (name == null ? "" : name + "  ") + unit;
            }
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) { throw new NotSupportedException(); }
        }

        EditItem SelectedEdit { get { return EditList.SelectedItem as EditItem; } }

        void EditEnabled_Click(object sender, RoutedEventArgs e)
        {
            var item = (EditItem)((CheckBox)sender).Tag;
            session.SetEnabled(item.Edit, ((CheckBox)sender).IsChecked == true);
            RefreshEdits();
            RefreshUnitViews();
        }

        void RemoveEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = SelectedEdit;
            if (item == null) return;
            session.RemoveEdit(item.Edit);
            RefreshEdits();
            RefreshUnitViews();
        }

        void RebaseEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = SelectedEdit;
            if (item == null) return;
            session.Rebase(item.Edit);
            RefreshEdits();
        }

        void OpenEditUnit_Click(object sender, RoutedEventArgs e)
        {
            var item = SelectedEdit;
            if (item != null) ShowUnit(SourceStack.ToVirtual(item.Edit.Unit ?? item.Edit.File));
        }

        void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "프로젝트 (*.json)|*.json", CheckFileExists = false, InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(session.ProjectPath)) };
            if (dialog.ShowDialog(this) == true) OpenProjectFile(dialog.FileName);
        }

        void Build_Click(object sender, RoutedEventArgs e)
        {
            if (EditorSession.GameRunning() &&
                MessageBox.Show("게임이나 에디터가 실행 중이다. 빌드해도 다시 시작해야 반영된다. 계속할까?", "빌드", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            var report = session.Build();
            var lines = new System.Collections.Generic.List<string>
            {
                "적용 " + report.Count(EditStatus.Applied) + "건, 원본이 바뀜 " + report.Count(EditStatus.SourceChanged) + "건, 대상 없음 " +
                report.Count(EditStatus.TargetMissing) + "건, 적용 불가 " + report.Count(EditStatus.Invalid) + "건, 파일 오류 " + report.FileErrors.Count + "건",
            };
            lines.AddRange(session.Plan.Warnings.Select(w => "경고: " + w));
            lines.AddRange(report.Warnings.Select(w => "경고: " + w));
            lines.AddRange(report.FileErrors.Select(x => "오류: " + x));
            lines.AddRange(report.Results.Where(r => r.Status != EditStatus.Applied).Take(20).Select(r => r.Status + ": " + r.Edit.File + " " + r.Edit.Path + (r.Message == null ? "" : " : " + r.Message)));
            lines.Add("씀 " + report.WrittenFiles.Count + "개, 지움 " + report.RemovedFiles.Count + "개");
            MessageBox.Show(string.Join("\n", lines), "빌드 결과");
            RefreshEdits();
        }
    }
}
```

- [ ] **Step 3: 빌드하고 스크린샷으로 확인한다**

확인용 프로젝트를 만든다(저장소 루트, 게임이 꺼져 있을 필요 없다. 빌드는 하지 않는다):

```bash
MSYS_NO_PATHCONV=1 dotnet run --project editor/CtaToybox.Cli -c Release -- add --project .superpowers/screens/edits.json --file properties/armoredcar.ext --path "/extender[inventory]/box/item" --occ 0 --arg 1 --new 3
MSYS_NO_PATHCONV=1 dotnet run --project editor/CtaToybox.Cli -c Release -- add --project .superpowers/screens/edits.json --scope unit --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --file entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc --path "/extender[inventory]/box/item" --occ 1 --arg 1 --new 5
dotnet build editor/CtaToybox.App -c Release
editor/CtaToybox.App/bin/Release/net8.0-windows/CtaToybox.exe --project .superpowers/screens/edits.json --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --tab full --screenshot .superpowers/screens/task8.png
```

Expected: `task8.png`에서 오른쪽 수정 목록에 유닛별 묶음 두 개와 각 수정(`1 → 3` 전부(유닛 127개), `24 → 5` 이 유닛만)이 보이고, 위쪽에 `수정 2건`이 보인다. 전체 탭의 `24` 행의 수정 칸에 `→ 5 (이 유닛만)`, 연료통 `1` 행에 `→ 3 (전부)`가 보인다. Git Bash에서 `MSYS_NO_PATHCONV=1`이 없으면 `/extender...` 경로가 Windows 경로로 바뀐다(2026-10-13에 겪음).

- [ ] **Step 4: 전체 테스트를 돌고 커밋한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 모두 통과.

```bash
git -C E:/CTAToybox add editor/CtaToybox.App
git -C E:/CTAToybox commit -m "feat(editor): edit list grouped by unit with status, toggle, remove, rebase and build

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
```

---

### Task 9: 요약 탭 확인

요약 탭의 목록은 Task 7에서 이미 채운다(`SummaryList`). 이 Task는 실제 유닛에서 요약 탭이 비어 있지 않은지, 이름이 읽을 만한지를 본다.

**Files:**
- 바꿀 파일 없음(확인에서 문제가 나오면 그때 정한다)

- [ ] **Step 1: 스크린샷 세 장을 찍는다**

```bash
editor/CtaToybox.App/bin/Release/net8.0-windows/CtaToybox.exe --unit entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --tab summary --screenshot .superpowers/screens/task9-vehicle.png
editor/CtaToybox.App/bin/Release/net8.0-windows/CtaToybox.exe --unit set/breed/mp/usa/late/tank_com_m3.set --tab summary --screenshot .superpowers/screens/task9-breed.png
editor/CtaToybox.App/bin/Release/net8.0-windows/CtaToybox.exe --unit set/stuff/smg/m3_grease --tab summary --screenshot .superpowers/screens/task9-stuff.png
```

- [ ] **Step 2: 그림을 보고 기록한다**

Read로 세 PNG를 연다. 종류마다 요약 탭의 행 수, 이름이 비어 있거나 잘린 행, 같은 이름이 여러 번 나오는 행을 적는다.

- 행이 0개인 종류가 있으면 그 종류의 "확인" 주석이 펼친 키와 맞지 않는 것이다. 추측으로 주석을 고치지 않는다. 그 종류의 펼친 키 몇 개와 `common.psv`의 "확인" 키를 나란히 적어 보고하고, `editor/summary/<종류>.psv`를 만들지는 사용자에게 묻는다(키의 의미를 정하는 일이라 근거 없이 정하지 않는다).
- 행이 있으면 이 Task는 기록만 하고 끝낸다.

기록은 Task 10의 README "요약 탭" 절에 넣는다.

---

### Task 10: 문서와 확인 목록

**Files:**
- Modify: `docs/07-stat-editor/README.md`
- Modify: `docs/07-stat-editor/ingame-check.md`
- Modify: `CLAUDE.md`

**Interfaces:**
- Consumes: Task 6~9의 앱과 스크린샷 기록.

- [ ] **Step 1: README에 앱 절을 더한다**

`docs/07-stat-editor/README.md`의 "할 수 있는 것과 없는 것" 표에서 `| 유닛을 경로나 표시 이름으로 찾기 (`units`) | 화면 |`의 `화면`을 지우고(빈칸), 표 바로 아래에 절을 더한다:

````markdown
## 화면 (CtaToybox.exe)

```
dotnet run --project editor/CtaToybox.App -c Release
```

처음 실행하면 경로를 묻는 설정 창이 뜬다(명령줄의 `init`과 같다). 프로젝트는 기본으로 `editor/projects/my-tweaks.json`이고 위쪽 "열기..."로 바꾼다. 처음 한 번은 유닛 색인을 만드느라 몇 초 걸린다.

- **왼쪽 유닛 찾기**: 표시 이름이나 경로로 찾고 종류와 출처(계층)로 거른다. 결과는 500개까지 보인다.
- **요약 탭**: 뜻이 확인된 키만 이름을 붙여 보인다. 어떤 키를 올릴지는 `editor/summary/<종류>.psv`(`경로|이름`)로 정한다. 파일이 없으면 `data/annotations`에서 등급이 "확인"인 키를 쓴다.
- **전체 탭**: 펼친 값을 원본 파일별로 묶어 보인다. 열마다 값, 걸린 수정, 공유 수, "이 유닛만" 가능 여부, 파일 안의 자리, 키 설명과 등급이 있다. 아래 "참조하는 아이템"은 유닛이 이름으로 가리키는 무기·탄약이다. 파일 하나로 이어진 것은 두 번 눌러 연다. `후보(미확인)`, `없음`은 게임이 어느 파일을 쓰는지 확인하지 않은 이름이다.
- **값 고치기**: 값을 두 번 누른다. 공유되는 값이면 영향받는 유닛 목록과 "전부 / 이 유닛만"이 나온다. "이 유닛만"이 안 되는 자리는 선택지가 꺼지고 이유가 보인다. 같은 자리를 다시 고치면 수정이 바뀐다(쌓이지 않는다).
- **오른쪽 수정 목록**: 유닛별로 묶인다. 체크로 끄고 켠다. "삭제"하면 값이 원본으로 돌아간다. "원본이 바뀜"이 뜬 수정은 "지금 값에 맞추기"로 원래 값을 지금 원본 값으로 바꾸거나 삭제한다.
- **빌드**: 게임이나 에디터가 실행 중이면 묻는다. 결과를 창으로 보인다.

화면 확인용 옵션: `--unit <가상 경로> --tab summary|full --screenshot <png>`는 그 유닛을 연 창을 PNG로 저장하고 끝낸다.

### 요약 탭 (2026-10-14)

<Task 9의 기록: 종류별 행 수와 눈에 띈 문제를 그대로 적는다>
````

- [ ] **Step 2: 사람 확인 목록과 게임 확인 목록을 한 절에 모은다**

`docs/07-stat-editor/ingame-check.md` 끝에 더한다:

```markdown
## 4단계: 화면과 남은 게임 확인

화면의 창 동작과 남은 게임 확인을 한 번에 본다. 확인용 프로젝트는 `editor/projects/ingame-check-app.json`이다(Step 3에서 만든다).

**화면에서 볼 것** (앱으로 직접 한다. 스크린샷 옵션은 대화 창을 열지 않는다)

| # | 할 것 | 기대 |
|---|---|---|
| 1 | M3A1 Halftrack(`m3a1_halftrack.def`)을 열고 전체 탭에서 `mine m1a1`의 `24`를 두 번 누른다 | 영향받는 유닛 4개 목록, "전부 / 이 유닛만" 둘 다 켜짐 |
| 2 | "이 유닛만", 새 값 `1 2` | 오류 문구, 넣기 단추 꺼짐 |
| 3 | 새 값 `5`로 넣기 | 수정 목록에 "이 유닛만"으로 한 건, 그 행의 수정 칸 `→ 5 (이 유닛만)` |
| 4 | 같은 값을 다시 눌러 `6`으로 넣기 | 수정 목록은 여전히 한 건, 값만 `6` |
| 5 | 그리스건(`set/stuff/smg/m3_grease`)을 열고 재장전 시간(`.presets`의 매크로 본문)을 두 번 누른다 | "이 유닛만"이 꺼져 있고 이유가 보인다 |
| 6 | 수정 목록에서 체크를 끄고 켠다, 삭제한다 | 목록과 유닛 보기가 바로 바뀐다 |

**게임에서 볼 것** (위 화면 확인과 같은 빌드에 넣는다)

| # | 넣은 것 | 볼 것 | 알게 되는 것 |
|---|---|---|---|
| A | `thompson_m1928a1_50`의 `rpm(830)` → 60 | 톰슨 30발 탄창(`thompson_m1928a1_30`, `{from "thompson_m1928a1_50"}`)의 연사 | `from` 상속: 물려준 파일의 수정이 물려받는 파일에 반영되는지 |
| B | `set/stuff/mgun/mgun_usa.belt.ammo`의 값 하나(Step 3에서 고른 것) | M26 Pershing(`{item "ammo mgun_usa" 4500}`)의 기관총 탄 | 이름 규칙의 "후보" 이름(`ammo mgun_usa`)이 그 파일로 이어지는지 |

**기록**

| 항목 | 결과 |
|---|---|
| 화면 1~6 | 확인하지 않음 |
| A. `from` 상속 | 확인하지 않음 |
| B. 후보 이름 | 확인하지 않음 |
```

- [ ] **Step 3: B의 값을 데이터로 고르고 확인용 프로젝트를 만든다**

`mgun_usa.belt.ammo`에서 게임에서 눈에 보이는 값을 고른다. 추측하지 않는다: `ctatoybox show set/stuff/mgun/mgun_usa.belt.ammo`의 행과 `data/annotations`의 등급을 보고, 등급이 "확인"이거나 게임에서 이미 본 적이 있는 키(예: 인벤토리의 `block` 같은 묶음 크기)를 고른다. 고를 근거가 없으면 B를 빼고 기록에 "고를 근거 없음"으로 적는다.

A와 B를 `editor/projects/ingame-check-app.json`에 넣는다(명령은 3단계 문서의 `add`와 같다. Git Bash면 `MSYS_NO_PATHCONV=1`을 붙인다). 빌드는 게임이 꺼진 뒤 사용자 확인 직전에 한다.

- [ ] **Step 4: `CLAUDE.md`를 고친다**

"명령" 절의 편집기 명령줄 줄 다음에 한 줄을 더한다:

```markdown
- 편집기 화면: `dotnet run --project editor/CtaToybox.App -c Release`. 화면 확인은 `--unit <가상 경로> --tab summary|full --screenshot <png>`로 PNG를 찍어 본다.
```

- [ ] **Step 5: 전체 테스트를 돌고 커밋·병합한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: 모두 통과.

```bash
git -C E:/CTAToybox add CLAUDE.md docs/07-stat-editor
git -C E:/CTAToybox commit -m "docs(editor): app usage and the stage 4 check list

Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>"
git -C E:/CTAToybox checkout develop
git -C E:/CTAToybox merge --no-ff feat/stat-editor-app -m "Merge branch 'feat/stat-editor-app' into develop"
```

- [ ] **Step 6: 사용자에게 한 번에 부탁한다**

게임이 꺼져 있으면 `ingame-check-app.json`을 빌드하고, 4단계 절의 "화면에서 볼 것" 1~6과 "게임에서 볼 것" A·B를 표 하나로 전한다. 결과를 받으면 기록 표를 채우고, 수정을 끄고 다시 빌드해 생성물을 지운다.
