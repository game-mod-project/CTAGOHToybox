# 게임 설정 편집 (하위 프로젝트 B) Implementation Plan

> **상태(2026-10-17)**: 끝났다(게임 설정, 하위 프로젝트 B). 아래 체크 상자는 진행 중에 표시하지 않았다. 무엇을 했는지는 git 기록과 `docs/10-backlog.md` "정리하며 지운 것"에 있다.

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 컨퀘스트 자원·길이·전투 규칙·AI(Lua), 스커미시 모드, 난이도 설정 파일을 편집기의 새 종류 "게임 설정"으로 열어, 뜻과 단위를 보며 고치고 빌드한다.

**Architecture:** 설정 파일을 A(스탯 편집기)의 유닛 색인·화면·수정 목록·빌드에 그대로 태운다. 편집기 전용 판정(`SettingFiles`, `UnitKind.OfEditor`)을 더하고, 문자열 속 설정과 단계표(`SettingStrings`), 새 값 검사의 한 입구(`ValueRules`), 작은 Lua 표 읽기기·패처(`LuaTable`, `LuaPatcher`), 파일 종류를 가리지 않고 값 줄을 내는 `SourceRows`를 새로 만든다. 수정 기록 형식은 바꾸지 않는다.

**Tech Stack:** .NET 8 SDK (C# 12), WPF, `System.Text.Json`. 외부 패키지 없음. 테스트는 `editor/CtaToybox.Core.Tests` 콘솔 테스트(`T.Eq`, `T.True`, `T.Throws`).

**Spec:** `docs/superpowers/specs/2026-10-15-settings-editor-design.md`

## Global Constraints

- 게임 폴더와 창작마당 폴더는 읽기만 한다. 출력은 `mod/CTAToybox/resource`와 `mod/CTAToybox/.ctatoybox-build.json`에만 쓴다.
- 수정 기록(`Edit`: Unit, File, Path, Occurrence, Arg, Old, New, Scope, Enabled) 형식을 바꾸지 않는다.
- 게임 설정의 수정 범위는 항상 "전부"(`shared`)다. "이 유닛만"의 이유 글자: `게임 설정은 파일 전체에 적용된다. '전부'만 된다`
- `UnitKind.Of`와 `KeyAnnotations.GroupOf`는 `tools/lib/Common.ps1`과 같은 규칙으로 둔다(바꾸지 않는다). 게임 설정은 편집기 전용 판정으로 더한다.
- 키의 뜻을 모르면 등급 `미확인`으로 둔다. 근거 없이 뜻을 쓰지 않는다. 주석 줄 형식: `경로|형태|등급|의미|출처`, 등급은 `확인`(게임 파일 주석), `문서`(웹 글), `추정`(파일 구조·이름), `미확인`.
- 브랜치: `develop`에서 단계마다 `feat/*` 또는 `chore/*`를 만들고 `git merge --no-ff`로 `develop`에 병합한다(원격 없음). `main`에 직접 커밋하지 않는다.
- 파일의 줄 끝을 바꾸지 않는다. `.cs`는 파일마다 LF 또는 CRLF가 섞여 있고, `data/annotations/*.psv`, `editor/summary/*.psv`, `docs/**/*.md`는 CRLF다. 여러 줄은 Edit 도구로 넣고, bash `sed -i`를 쓴 뒤에는 `file`로 줄 끝을 확인해 `sed -i 's/\r*$/\r/'`로 되돌린다.
- `git add editor`를 쓰지 않는다(추적하지 않는 `editor/projects/*.json`이 들어간다). 경로를 골라 add한다.
- 게임이 켜져 있으면 빌드하지 않는다(CLI `build`는 `--force` 없이 멈춘다). 게임 안 확인 빌드도 게임을 끈 상태에서 한다.
- 앱을 반복해 띄우지 않는다. 화면 확인은 한 번 실행(`--shots` 또는 `--scenario`)으로 모아 찍고, 사용자의 프로젝트를 쓰지 않는다(`--project <임시 파일>`).
- 명령은 모두 저장소 루트 `E:\CTAToybox`에서 실행한다. 코어 테스트: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`. 전체 테스트: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`.

## Review Focus

- **모드가 덮어쓴 설정 파일**: 이 PC의 `resources_standard.set`은 MACE Wunderwaffe가 덮어쓴다. 목록·화면·수정은 이기는 계층의 파일을 보여야 하고, 요약 키는 `**/꼬리`라서 맞아야 한다. → Task 2의 모드 계층 테스트
- **조각 값에 `;`나 `=`를 친 경우**: `scoreStart=7;x=1`처럼 치면 조각 수가 바뀐다. 넣지 않고 이유를 보여야 한다. → Task 4의 `CheckPieces` 테스트
- **같은 문자열의 조각을 차례로 두 번 고친 경우**: 두 번째 수정이 첫 번째 조각 값을 지우면 안 된다(창은 기존 수정의 새 값에서 시작한다). → Task 5의 `ApplyRow` 두 번 테스트
- **Lua 값에 이름·표·함수를 친 경우**: `2 * x`, `{}`, `print(1)`은 넣지 않아야 한다. → Task 7의 `CheckNewValue` 테스트
- **모드가 Lua 파일을 읽기기가 모르는 문법으로 바꾼 경우**: 줄 없이 "읽지 못함"이 보이고, 이전 수정은 빌드에서 적용 불가로 보고돼야 한다(파일을 깨뜨리지 않는다). → Task 8의 모르는 문법 테스트

## 이 계획에서 정한 것 (스펙과 다르거나 스펙이 열어 둔 부분)

- **주석 묶음 고르기**: 스펙은 `KeyAnnotations.GroupOf`가 "settings"를 돌려준다고 적었다. 그런데 `GroupOf`는 `tools/lib/Common.ps1`의 `Get-SdlFileGroup`과 같은 규칙이어야 한다(코드 주석, `tools/tests/Export.Tests.ps1`). 그래서 `GroupOf`는 그대로 두고, `EditorSession.NotesFor`가 게임 설정이면 "settings"를 고른다. 결과(설정 파일의 주석이 `settings.psv`에서 온다)는 같다.
- **수가 없는 게임 설정 파일**: 편집기 목록은 수가 하나도 없는 파일을 뺀다(`Numbers > 0`). 바닐라 `duration_*.set`은 값이 모두 문자열 단계표라 빠진다. 게임 설정은 이 조건에서 뺀다(앱 `Search`, CLI `units`).
- **값 줄을 찾는 한 입구 `SourceRows`**: 수정의 자리를 원본에서 다시 찾는 곳(점검, 설명, CLI `show`·`add`)이 SDL 파서를 직접 불렀다. Lua를 끼우려고 한 함수로 모은다.
- **게임 안 확인 5(난이도)**: 배수가 숫자로 보이는 화면이나 로그를 찾지 못했다(2026-10-14 `game.log`는 `player#1, has a difficulty of heroic`처럼 이름만 찍는다). 이번 확인 빌드에는 난이도 수정을 넣지 않고 "확인하지 않음"으로 적는다.

---

## 단계 1: 게임 설정 종류 (브랜치 `feat/settings-kind`)

### Task 1: 게임 설정 파일 판정과 색인

**Files:**
- Create: `editor/CtaToybox.Core/SettingFiles.cs`
- Modify: `editor/CtaToybox.Core/UnitKind.cs`
- Modify: `editor/CtaToybox.Core/UnitIndex.cs` (Build의 경로 고르기와 종류, `FormatVersion`)
- Modify: `editor/CtaToybox.Core/DisplayNames.cs` (경로 이름)
- Create: `editor/CtaToybox.Core.Tests/SettingFilesTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (스위트 등록)

**Interfaces:**
- Produces:
  - `SettingFiles.Kind` = `"settings"`, `SettingFiles.UnitScopeNote` (string)
  - `bool SettingFiles.IsTop(string virtualPath)`, `bool SettingFiles.InArea(string virtualPath)`
  - `sealed class SettingFiles.Entry { string Group; string Name; }`, `Dictionary<string, SettingFiles.Entry> SettingFiles.LoadNames(string psvPath)` (키는 대소문자 무시)
  - `string UnitKind.OfEditor(string virtualPath)`: `Of(...)`에 게임 설정을 더한 것
  - `UnitKind.Editor`에 `"settings"`, `UnitKind.Label("settings")` = `"게임 설정"`, `UnitKind.EditorKindOf("settings", …)` = `"settings"`
  - `void DisplayNames.AddPathName(string virtualPath, string name)`

- [ ] **Step 1: 브랜치를 만든다**

```bash
git checkout develop
git checkout -b feat/settings-kind
```

- [ ] **Step 2: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/SettingFilesTests.cs`:

```csharp
// 게임 설정 파일(하위 프로젝트 B)의 판정, 이름 표, 유닛 색인
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class SettingFilesTests
{
    public const string Res = "set/dynamic_campaign/resources_standard.set", Dur = "set/dynamic_campaign/duration_normal.set",
        Ctf = "set/multiplayer/games/campaign_capture_the_flag.set", Bz = "set/multiplayer/games/battle_zones.set",
        Limits = "set/multiplayer/games/presets/global_parameters/unitlimits.inc", Dl = "set/difficulty/normal.dl", DlInc = "set/difficulty/difficulty_generic.inc";

    // 바닐라 꼴을 줄인 게임 설정 파일들
    public static (string Name, string Text)[] Files()
    {
        return new[]
        {
            (Res, "{Standard\n\t{GlobalMaxCP 550}\n\t{Bots {StartMP 1.00 min 99999 attackerMultiplier 1.00}}\n\t{Resources\n\t\t{MP {StartVal 900} {WinGain \"1:200 4:250\"}}\n\t\t{RP {StartVal 6}}\n\t}\n}"),
            (Dur, "{Normal\n\t{FlagCount \"2:5 2:5 3:5\"}\n}"),
            (Ctf, "{game\n\t{parameters \"flag_capture_time=75;scoreStart=250\"}\n\t{settings {scoreFinal 1000}}\n}"),
            (Bz, "{game\n\t{parameters \"flag_capture_time=60;scoreStart=250\"}\n\t(include \"presets/global_parameters/unitlimits.inc\")\n}"),
            (Limits, "{unitLimits \"officer=2;sniper=5\"}"),
            (Dl, "{difficulty\n\t(include \"difficulty_generic.inc\")\n\t{enemy {health 0.50}}\n}"),
            (DlInc, "\t{any {burst 1}}"),
            ("set/dynamic_campaign/values.set", "{GameModes \"campaign_capture_the_flag\"}"),
            ("set/dynamic_campaign/unit_research_ger.set", "{tech \"a\" costs 5}"),
        };
    }

    public static void Run()
    {
        T.True(SettingFiles.IsTop(Res) && SettingFiles.IsTop(Dur) && SettingFiles.IsTop(Ctf) && SettingFiles.IsTop(Bz) && SettingFiles.IsTop(Dl), "top setting files");
        T.True(SettingFiles.IsTop("SET/Dynamic_Campaign/Resources_High.set"), "case does not matter");
        T.True(!SettingFiles.IsTop(Limits) && !SettingFiles.IsTop(DlInc), "included files are not list entries");
        T.True(!SettingFiles.IsTop("set/dynamic_campaign/values.set") && !SettingFiles.IsTop("set/dynamic_campaign/unit_research_ger.set") && !SettingFiles.IsTop("set/dynamic_campaign/map_points.set"), "values, research and map list are out of scope");
        T.True(!SettingFiles.IsTop("set/multiplayer/games/gamemode.template"), "only .set game modes");
        T.True(SettingFiles.InArea(Limits) && SettingFiles.InArea(DlInc) && SettingFiles.InArea("script/multiplayer/modes/conquest/waves.lua"), "the settings area holds included files too");
        T.True(!SettingFiles.InArea("set/breed/mp/ger/mid/medic.set"), "units are not in the settings area");

        T.Eq("settings", UnitKind.OfEditor(Res), "OfEditor adds the settings kind");
        T.Eq("breed", UnitKind.OfEditor("set/breed/mp/ger/mid/medic.set"), "OfEditor keeps the unit kinds");
        T.True(UnitKind.Of(Res) == null, "Of stays the same as Common.ps1");
        T.Eq("settings", UnitKind.EditorKindOf("settings", new string[0]), "editor kind");
        T.True(UnitKind.InEditor("settings"), "settings is shown in the editor");
        T.Eq("게임 설정", UnitKind.Label("settings"), "label");

        string dir = T.TempDir(), psv = Path.Combine(dir, "files.psv");
        File.WriteAllText(psv, "# 주석\n" + Res + "|컨퀘스트 자원|컨퀘스트 자원 — standard\n\n" + Dl + "|난이도|난이도 — normal\n");
        var names = SettingFiles.LoadNames(psv);
        T.Eq(2, names.Count, "names");
        T.Eq("컨퀘스트 자원", names[Res.ToUpperInvariant()].Group, "group, case-insensitive key");
        T.Eq("난이도 — normal", names[Dl].Name, "name");
        T.Eq(0, SettingFiles.LoadNames(Path.Combine(dir, "none.psv")).Count, "a missing file is empty");
        File.WriteAllText(psv, Res + "|두 칸\n");
        T.Throws<System.FormatException>(() => SettingFiles.LoadNames(psv), "a short line is a format error");

        var display = DisplayNames.Load(Path.Combine(dir, "none.csv"));
        display.AddPathName(Res, "컨퀘스트 자원 — standard");
        T.Eq("컨퀘스트 자원 — standard", display.OfUnit(Res), "a path name");

        string game = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), Files());
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", Path.Combine(game, "resource")) });
        stack.Scan();
        var index = UnitIndex.Build(stack, null);
        var settings = index.Units.Where(u => u.Kind == "settings").Select(u => u.Path).OrderBy(p => p).ToArray();
        T.Eq(new[] { Dl, Dur, Res, Bz, Ctf }.OrderBy(p => p).ToArray(), settings, "the index lists the top setting files");
        T.True(index.Units.Where(u => u.Kind == "settings").All(u => u.EditorKind == "settings"), "with the settings editor kind");
        T.True(index.Units.Single(u => u.Path == Res).Numbers > 0, "numbers are counted");
        T.Eq(0, index.Units.Single(u => u.Path == Dur).Numbers, "a duration file holds only step-table strings (the list must not hide it, Task 2)");
        T.Eq(new[] { Bz }, index.UsersOf(Limits).ToArray(), "an included preset counts its users");
    }
}
```

`editor/CtaToybox.Core.Tests/Program.cs`의 스위트 목록에서 `("Cli", CliTests.Run),` 앞에 한 줄을 넣는다:

```csharp
            ("SettingFiles", SettingFilesTests.Run),
```

- [ ] **Step 3: 테스트가 실패하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'SettingFiles'` 이름이 없다(CS0103), `AddPathName`·`OfEditor` 정의 없음.

- [ ] **Step 4: `SettingFiles.cs`를 쓴다**

`editor/CtaToybox.Core/SettingFiles.cs`:

```csharp
// 게임 설정 파일(하위 프로젝트 B): 컨퀘스트 자원·길이·전투 규칙·AI, 스커미시 모드, 난이도.
// 편집기에만 있는 종류다. tools/lib/Common.ps1의 유닛 종류에는 넣지 않는다(UnitKind.Of는 그대로 둔다).
// 스펙: docs/superpowers/specs/2026-10-15-settings-editor-design.md
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public static class SettingFiles
    {
        public const string Kind = "settings";
        public const string UnitScopeNote = "게임 설정은 파일 전체에 적용된다. '전부'만 된다";

        // 목록에 한 항목으로 올리는 맨 위 파일. 이 파일들이 include하는 .inc는 그 항목의 전체 탭에 나온다
        static readonly Regex Top = new Regex(
            @"^(set/dynamic_campaign/(resources|duration)_[^/]+\.set|set/multiplayer/games/[^/]+\.set|set/difficulty/[^/]+\.dl)$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // 게임 설정이 있는 폴더. 문자열 속 설정과 단계표 검사는 이 아래 파일에만 한다
        static readonly string[] Areas = { "set/dynamic_campaign/", "set/multiplayer/games/", "set/difficulty/", "script/multiplayer/modes/conquest/" };

        public static bool IsTop(string virtualPath) { return virtualPath != null && Top.IsMatch(virtualPath); }

        public static bool InArea(string virtualPath)
        {
            if (virtualPath == null) return false;
            foreach (string a in Areas) if (virtualPath.StartsWith(a, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public sealed class Entry
        {
            public string Group { get; set; }
            public string Name { get; set; }
        }

        // editor/settings/files.psv: 경로|묶음|화면 이름. #으로 시작하는 줄과 빈 줄은 건너뛴다. 파일이 없으면 빈 표
        public static Dictionary<string, Entry> LoadNames(string psvPath)
        {
            var result = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(psvPath)) return result;
            foreach (string line in File.ReadLines(psvPath, Encoding.UTF8))
            {
                if (line.Trim().Length == 0 || line.StartsWith("#")) continue;
                string[] p = line.Split('|');
                if (p.Length < 3) throw new FormatException("게임 설정 이름 줄 형식 오류(경로|묶음|화면 이름): " + line);
                result[p[0].Trim()] = new Entry { Group = p[1].Trim(), Name = p[2].Trim() };
            }
            return result;
        }
    }
}
```

- [ ] **Step 5: `UnitKind`에 편집기 판정을 더한다**

`editor/CtaToybox.Core/UnitKind.cs`에서:

```csharp
        public static readonly string[] Editor = { "tank", "cannon", "car", "vehicle-other", "breed", "stuff" };
```
를
```csharp
        public static readonly string[] Editor = { "tank", "cannon", "car", "vehicle-other", "breed", "stuff", SettingFiles.Kind };
```
로,

```csharp
            if (kind == "breed" || kind == "stuff") return kind;
```
를
```csharp
            if (kind == "breed" || kind == "stuff" || kind == SettingFiles.Kind) return kind;
```
로 바꾸고, `Label`의 `case "stuff": return "무기·탄약·장비";` 다음 줄에 넣는다:

```csharp
                case SettingFiles.Kind: return "게임 설정";
```

`Of` 메서드 앞에 넣는다:

```csharp
        // 편집기의 원래 종류: Of에 게임 설정(SettingFiles)을 더한 것. Of는 tools/lib/Common.ps1과 같은 규칙으로 둔다
        public static string OfEditor(string virtualPath)
        {
            return Of(virtualPath) ?? (SettingFiles.IsTop(virtualPath) ? SettingFiles.Kind : null);
        }
```

파일 머리 주석 두 번째 줄 뒤에 한 줄을 넣는다:

```csharp
// 게임 설정(settings)은 편집기에만 있는 종류다(OfEditor). Common.ps1의 Get-SdlUnitKind에는 없다.
```

- [ ] **Step 6: 색인이 게임 설정을 담게 한다**

`editor/CtaToybox.Core/UnitIndex.cs`의 `Build`에서:

```csharp
            var paths = stack.Paths.Where(p => UnitKind.Of(p) != null).OrderBy(p => p, StringComparer.Ordinal).ToList();
```
를
```csharp
            var paths = stack.Paths.Where(p => UnitKind.OfEditor(p) != null).OrderBy(p => p, StringComparer.Ordinal).ToList();
```
로, 루프 안의

```csharp
                    Path = paths[i], Kind = UnitKind.Of(paths[i]), Layer = stack.Find(paths[i]).Layer,
                    Includes = r.Includes, Presets = r.Presets, Unresolved = r.Unresolved.Count, Numbers = CountNumbers(r.Nodes),
                    EditorKind = UnitKind.EditorKindOf(UnitKind.Of(paths[i]), r.Includes)
```
를
```csharp
                    Path = paths[i], Kind = UnitKind.OfEditor(paths[i]), Layer = stack.Find(paths[i]).Layer,
                    Includes = r.Includes, Presets = r.Presets, Unresolved = r.Unresolved.Count, Numbers = CountNumbers(r.Nodes),
                    EditorKind = UnitKind.EditorKindOf(UnitKind.OfEditor(paths[i]), r.Includes)
```
로 바꾸고, `const string FormatVersion = "3";`을 `"4"`로 바꾼다.

- [ ] **Step 7: 경로 이름을 더한다**

`editor/CtaToybox.Core/DisplayNames.cs`에서 `readonly Dictionary<string, string> byContext = …;` 다음 줄에 넣는다:

```csharp
        // 현지화에 없는 이름(게임 설정 파일 등). 현지화보다 먼저 찾는다
        readonly Dictionary<string, string> byPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public void AddPathName(string virtualPath, string name) { byPath[virtualPath] = name; }
```

`OfUnit`의 `string name;` 줄 바로 다음에 넣는다:

```csharp
            if (byPath.TryGetValue(unitPath, out name)) return name;
```

- [ ] **Step 8: 테스트가 통과하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed=|SettingFiles"`
Expected: `== SettingFiles ok`, `failed=0`

- [ ] **Step 9: 커밋한다**

```bash
git add editor/CtaToybox.Core/SettingFiles.cs editor/CtaToybox.Core/UnitKind.cs editor/CtaToybox.Core/UnitIndex.cs editor/CtaToybox.Core/DisplayNames.cs editor/CtaToybox.Core.Tests/SettingFilesTests.cs editor/CtaToybox.Core.Tests/Program.cs
git commit -m "feat(editor): game settings files as an editor-only kind in the unit index"
```

### Task 2: 세션·화면·명령줄에서 게임 설정 열기

**Files:**
- Modify: `editor/CtaToybox.Core/EditorSession.cs` (`Open`, `NotesFor`, `OpenUnit`, `KindLabel`)
- Modify: `editor/CtaToybox.App/MainWindow.Unit.cs` (`ShowUnit`의 탭 숨기기)
- Modify: `editor/CtaToybox.App/MainWindow.xaml.cs` (`RunSearch`의 종류 글자)
- Modify: `editor/CtaToybox.Cli/Program.cs` (`Usage`, `Units`, `Trace`, `Add`)
- Create: `editor/CtaToybox.Core.Tests/SettingsSessionTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: Task 1의 `SettingFiles`, `UnitKind.OfEditor`, `DisplayNames.AddPathName`
- Produces:
  - `string EditorSession.KindLabel(UnitEntry u)`: 게임 설정이면 `files.psv`의 묶음, 아니면 `UnitKind.Label`
  - 게임 설정 유닛의 모든 고칠 수 있는 값은 `ValueView.UnitScopeBlocker == SettingFiles.UnitScopeNote`
  - `SettingsSessionTests.Fixture(out string repo, out string outDir, out string game)`: 게임 설정 파일(Task 1의 `SettingFilesTests.Files()`)이 든 가짜 게임·저장소·설정. 이후 Task가 쓴다
  - 게임 설정의 주석 묶음은 `data/annotations/settings.psv`(`KeyAnnotations.Load(dir, "settings")`)

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/SettingsSessionTests.cs`:

```csharp
// 게임 설정을 세션·명령줄에서 연다: 이름, 종류, 주석 묶음, '전부'만, 모드가 덮어쓴 파일
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

static class SettingsSessionTests
{
    // 한글이 든 저장소 파일은 UTF-8로 쓴다(T.Put은 Latin1이라 한글이 깨진다)
    static void Utf8(string root, string relativePath, string text)
    {
        string p = Path.Combine(root, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(p));
        File.WriteAllText(p, text);
    }

    // 게임 설정 파일이 든 가짜 게임과 저장소. 모드 mod_222가 resources_standard.set을 덮어쓴다
    public static string Fixture(out string repo, out string outDir, out string game)
    {
        string workshop = T.TempDir(), work = T.TempDir();
        game = T.TempDir(); repo = T.TempDir(); outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), SettingFilesTests.Files());
        T.Put(Path.Combine(workshop, "222", "resource"), SettingFilesTests.Res,
            "{Standard\n\t{GlobalMaxCP 2000}\n\t{Resources\n\t\t{MP {StartVal 1500}}\n\t}\n}");
        Utf8(repo, "data/annotations/common.psv", "*/mass|block|확인|무게|C\n");
        Utf8(repo, "data/annotations/settings.psv", "**/globalmaxcp|block|확인|전체 CP 상한|C\n*/startval|block|확인|시작할 때의 양|C\n");
        Utf8(repo, "editor/settings/files.psv", SettingFilesTests.Res + "|컨퀘스트 자원|컨퀘스트 자원 — standard\n" + SettingFilesTests.Dl + "|난이도|난이도 — normal\n");
        Utf8(repo, "editor/summary/settings.psv", "**/globalmaxcp|전체 CP 상한\n*/startval|시작 자원\n");
        string options = Path.Combine(work, "options.set");
        File.WriteAllText(options, "{options {mods \"mod_222:0\" \"" + Path.GetFileName(outDir) + ":0\"}}");
        string settings = Path.Combine(work, "settings.json");
        new EditorSettings { GameDir = game, WorkshopDir = workshop, OptionsSetPath = options, OutputModDir = outDir, OutputModName = Path.GetFileName(outDir) }.Save(settings);
        return settings;
    }

    static int Cli(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    public static void Run()
    {
        string repo, outDir, game;
        string settings = Fixture(out repo, out outDir, out game);
        using (var session = EditorSession.Open(settings, repo, null))
        {
            var found = session.Search(null, "settings", null).Select(u => u.Path).ToList();
            T.True(found.Contains(SettingFilesTests.Res) && found.Contains(SettingFilesTests.Bz), "the settings kind lists setting files");
            T.True(found.Contains(SettingFilesTests.Dur), "a settings file with only string values is listed too");
            T.Eq("컨퀘스트 자원 — standard", session.Names.OfUnit(SettingFilesTests.Res), "the name from files.psv");
            T.Eq("컨퀘스트 자원", session.KindLabel(session.Index.Units.First(u => u.Path == SettingFilesTests.Res)), "the group as the kind label");
            T.Eq("게임 설정", session.KindLabel(session.Index.Units.First(u => u.Path == SettingFilesTests.Bz)), "a file without a name falls back to the kind");

            var res = session.OpenUnit(SettingFilesTests.Res);
            T.Eq("settings", res.Kind, "kind");
            T.Eq("settings", res.EditorKind, "editor kind");
            T.Eq("mod_222", res.Layer, "the winning layer is the mod that overrides the file");
            var max = res.Rows.First(r => r.Path == "/standard/globalmaxcp");
            T.Eq("2000", max.Values[0].Trace.Text, "the mod's value");
            T.Eq("전체 CP 상한", max.Note.Meaning, "notes come from settings.psv");
            T.Eq("전체 CP 상한", max.SummaryLabel, "the settings summary file");
            T.True(res.Rows.SelectMany(r => r.Values).Where(v => v.Editable).All(v => v.UnitScopeBlocker == SettingFiles.UnitScopeNote), "every value is shared-only");
            string warning;
            T.Eq(SettingFiles.UnitScopeNote, session.ValidateNew(res, max.Values[0], "2100", "unit", out warning), "unit scope is refused");

            var bz = session.OpenUnit(SettingFilesTests.Bz);
            var limits = bz.Rows.First(r => r.Path.EndsWith("/unitlimits"));
            T.Eq(SettingFilesTests.Limits, limits.Values[0].Trace.File, "an included preset traces to its file");
        }

        string o;
        T.Eq(0, Cli(out o, "units", "--settings", settings, "--kind", "settings"), "units --kind settings");
        T.True(o.Contains(SettingFilesTests.Res) && o.Contains("컨퀘스트 자원 — standard"), "lists settings with their names");
        T.True(o.Contains(SettingFilesTests.Dur), "and the string-only duration file");
        T.Eq(0, Cli(out o, "trace", SettingFilesTests.Res, "--settings", settings), "trace a settings file");
        T.True(o.Contains("(settings, 계층 mod_222)"), "trace names the settings kind");
        string project = Path.Combine(T.TempDir(), "p.json");
        T.Eq(1, Cli(out o, "add", "--settings", settings, "--project", project, "--file", SettingFilesTests.Res, "--path", "/standard/globalmaxcp", "--new", "2100", "--scope", "unit", "--unit", SettingFilesTests.Res), "add --scope unit is refused for settings");
        T.True(o.Contains(SettingFiles.UnitScopeNote), "with the reason");
        T.Eq(0, Cli(out o, "add", "--settings", settings, "--project", project, "--file", SettingFilesTests.Res, "--path", "/standard/globalmaxcp", "--new", "2100"), "add shared works");
    }
}
```

`Program.cs`의 스위트 목록에서 `("SettingFiles", SettingFilesTests.Run),` 다음 줄에 넣는다:

```csharp
            ("SettingsSession", SettingsSessionTests.Run),
```

- [ ] **Step 2: 테스트가 실패하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'EditorSession'에 'KindLabel'에 대한 정의가 없다`.

- [ ] **Step 3: 세션을 고친다**

`editor/CtaToybox.Core/EditorSession.cs`:

필드 `ValueLabels valueLabels = new ValueLabels();` 다음 줄에 넣는다:

```csharp
        Dictionary<string, SettingFiles.Entry> settingFiles = new Dictionary<string, SettingFiles.Entry>(StringComparer.OrdinalIgnoreCase);
```

`Open`에서 `s.Names = DisplayNames.FromGame(s.Settings, s.Plan);` 다음 줄에 넣는다:

```csharp
            s.settingFiles = SettingFiles.LoadNames(Path.Combine(repoRoot, "editor", "settings", "files.psv"));
            foreach (var kv in s.settingFiles) s.Names.AddPathName(kv.Key, kv.Value.Name);
```

`LayerLabel` 메서드 다음에 넣는다:

```csharp
        // 목록에 보이는 종류 글자: 게임 설정은 files.psv의 묶음(컨퀘스트 자원 등), 나머지는 종류 이름
        public string KindLabel(UnitEntry u)
        {
            SettingFiles.Entry e;
            if (u.EditorKind == SettingFiles.Kind && settingFiles.TryGetValue(u.Path, out e)) return e.Group;
            return UnitKind.Label(u.EditorKind);
        }
```

`NotesFor`의 `string group = KeyAnnotations.GroupOf(unit);`를 바꾼다:

```csharp
            // 게임 설정은 편집기 전용 묶음 settings.psv를 쓴다. GroupOf는 Common.ps1과 같은 규칙으로 둔다
            string group = UnitKind.OfEditor(unit) == SettingFiles.Kind ? SettingFiles.Kind : KeyAnnotations.GroupOf(unit);
```

`Search`의 `.Where(u => UnitKind.InEditor(u.EditorKind) && u.Numbers > 0)`를 바꾼다(게임 설정은 값이 모두 문자열 단계표인 파일이 있다. 예: 바닐라 `duration_*.set`):

```csharp
                .Where(u => UnitKind.InEditor(u.EditorKind) && (u.Numbers > 0 || u.EditorKind == SettingFiles.Kind))
```

그 위의 주석 `// 편집기 목록: 편집기 종류(UnitKind.Editor)이고 값에 수가 하나라도 있는 유닛만. …` 끝에 ` 게임 설정은 수가 없어도 넣는다(문자열 단계표만 있는 파일).`을 덧붙인다.

`OpenUnit`의 `string kind = UnitKind.Of(traced.Unit);`를 `string kind = UnitKind.OfEditor(traced.Unit);`로 바꾸고, `AddMacroCalls(view, traced, n, summary);` 다음 줄에 넣는다:

```csharp
            // 게임 설정은 파일 하나가 곧 설정이라 '이 유닛만'이 없다
            if (kind == SettingFiles.Kind)
                foreach (var r in view.Rows)
                    foreach (var v in r.Values) if (v.Editable) v.UnitScopeBlocker = SettingFiles.UnitScopeNote;
```

- [ ] **Step 4: 명령줄을 고친다**

`editor/CtaToybox.Cli/Program.cs`:

`Usage`의 마지막 줄 `"종류: tank, cannon, car, vehicle-other, breed, stuff, 또는 vehicle(차량 전부). …"`의 `stuff,`를 `stuff, settings(게임 설정),`로 바꾼다.

`Units`에서 `var names = opt.ContainsKey("names") ? … ;` 다음 줄에 넣는다:

```csharp
                foreach (var kv in SettingFiles.LoadNames(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(settingsPath)), "settings", "files.psv")))
                    names.AddPathName(kv.Key, kv.Value.Name);
```

`Units`의 `.Where(u => UnitKind.InEditor(u.EditorKind) && u.Numbers > 0 && …)`에서 `u.Numbers > 0`을 `(u.Numbers > 0 || u.EditorKind == SettingFiles.Kind)`로 바꾼다(앱과 같다).

`Trace`의 `(UnitKind.Of(traced.Unit) ?? "유닛 아님")`을 `(UnitKind.OfEditor(traced.Unit) ?? "유닛 아님")`으로 바꾼다.

`Add`에서 `if (file == null) { output.WriteLine("원본에 이 파일이 없다: " + fileArg); return 1; }` 다음 줄에 넣는다:

```csharp
                if (scope == "unit" && (SettingFiles.InArea(file.VirtualPath) || UnitKind.OfEditor(SourceStack.ToVirtual(opt["unit"])) == SettingFiles.Kind))
                { output.WriteLine(SettingFiles.UnitScopeNote); return 1; }
```

- [ ] **Step 5: 화면을 고친다**

`editor/CtaToybox.App/MainWindow.Unit.cs`의 `ShowUnit`에서 `ItemsTab.Header = "무기·탄약 (" + unit.Items.Count + ")";` 다음 줄에 넣는다:

```csharp
            // 게임 설정에는 가격·로스터와 무기·탄약이 없다
            var other = unit.Kind == SettingFiles.Kind ? Visibility.Collapsed : Visibility.Visible;
            RosterTab.Visibility = ItemsTab.Visibility = other;
            if (other == Visibility.Collapsed && (Tabs.SelectedItem == RosterTab || Tabs.SelectedItem == ItemsTab)) Tabs.SelectedItem = SummaryTab;
```

`editor/CtaToybox.App/MainWindow.xaml.cs`의 `RunSearch`에서 `UnitKind.Label(u.EditorKind)`를 `session.KindLabel(u)`로 바꾼다.

- [ ] **Step 6: 테스트가 통과하는지 본다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1 2>&1 | tail -3`
Expected: `all test files passed` (코어 테스트에 `== SettingsSession ok`, 앱 빌드 오류 0)

- [ ] **Step 7: 커밋한다**

```bash
git add editor/CtaToybox.Core/EditorSession.cs editor/CtaToybox.App/MainWindow.Unit.cs editor/CtaToybox.App/MainWindow.xaml.cs editor/CtaToybox.Cli/Program.cs editor/CtaToybox.Core.Tests/SettingsSessionTests.cs editor/CtaToybox.Core.Tests/Program.cs
git commit -m "feat(editor): open game settings in the session, app and CLI - names, settings notes, shared-only"
```

### Task 3: 게임 설정 이름·주석·요약 키 (SDL 파일)

**Files:**
- Create: `editor/settings/files.psv`
- Create: `editor/summary/settings.psv`
- Create: `data/annotations/settings.psv`
- Modify: `data/annotations/values.psv` (게임 설정 묶음 추가)

**Interfaces:**
- Consumes: Task 2의 묶음 이름 `settings`, `files.psv` 형식
- Produces: 이후 화면 확인(Task 11)과 `-- real` 요약 키 검사(Task 10)가 쓰는 키 목록. 조각 자리 `@이름`은 Task 5가 읽는다(이 Task에서 미리 적어 둔다)

모든 뜻은 아래 근거에서 옮긴 것이다. `values.set`은 `set/dynamic_campaign/values.set`의 머리 설명(1~55줄)이고, "주석"은 그 줄 끝의 `;//` 주석이다(`_work/vanilla`로 확인).

- [ ] **Step 1: `editor/settings/files.psv`를 쓴다 (CRLF)**

```text
# 게임 설정 항목의 화면 이름. 형식: 경로|묶음|화면 이름. 손으로 고친다.
# 스커미시 모드와 난이도의 게임 안 이름은 확인하지 않아 파일 이름을 함께 적는다.
set/dynamic_campaign/resources_low.set|컨퀘스트 자원|컨퀘스트 자원 — low
set/dynamic_campaign/resources_standard.set|컨퀘스트 자원|컨퀘스트 자원 — standard
set/dynamic_campaign/resources_high.set|컨퀘스트 자원|컨퀘스트 자원 — high
set/dynamic_campaign/resources_very_high.set|컨퀘스트 자원|컨퀘스트 자원 — very_high
set/dynamic_campaign/duration_short.set|컨퀘스트 길이|컨퀘스트 길이 — short
set/dynamic_campaign/duration_normal.set|컨퀘스트 길이|컨퀘스트 길이 — normal
set/dynamic_campaign/duration_long.set|컨퀘스트 길이|컨퀘스트 길이 — long
set/dynamic_campaign/duration_very_long.set|컨퀘스트 길이|컨퀘스트 길이 — very_long
set/dynamic_campaign/duration_unlimited.set|컨퀘스트 길이|컨퀘스트 길이 — unlimited
set/multiplayer/games/campaign_capture_the_flag.set|컨퀘스트 전투 규칙|컨퀘스트 전투 규칙 (campaign_capture_the_flag)
script/multiplayer/modes/conquest/runtime_config.lua|컨퀘스트 AI|컨퀘스트 AI 설정 (runtime_config.lua)
set/multiplayer/games/battle_zones.set|스커미시 모드|스커미시 — 배틀존 (battle_zones)
set/multiplayer/games/frontlines.set|스커미시 모드|스커미시 — 프런트라인 (frontlines)
set/multiplayer/games/cooperative.set|스커미시 모드|스커미시 — cooperative
set/multiplayer/games/evacuation.set|스커미시 모드|스커미시 — evacuation
set/multiplayer/games/ammunition.set|스커미시 모드|스커미시 — ammunition
set/difficulty/training.dl|난이도|난이도 — training
set/difficulty/easy.dl|난이도|난이도 — easy
set/difficulty/normal.dl|난이도|난이도 — normal
set/difficulty/hard.dl|난이도|난이도 — hard
set/difficulty/heroic.dl|난이도|난이도 — heroic
set/difficulty/multiplayer.dl|난이도|난이도 — multiplayer
```

- [ ] **Step 2: `data/annotations/settings.psv`를 쓴다 (CRLF)**

```text
# 게임 설정 파일(하위 프로젝트 B) 전용 키 주석. 형식은 common.psv와 같다(경로|형태|등급|의미|출처). 손으로 고친다.
# values.set = set/dynamic_campaign/values.set의 머리 설명. 주석 = 같은 파일 그 줄 끝의 ;// 주석.
# ---------- 컨퀘스트 자원 (set/dynamic_campaign/resources_*.set)
**/globalmaxcp|block|확인|전체 CP 상한. 모든 단계의 유닛 CP를 더한 값이 이를 넘지 못한다 (확인: values.set "Global CP limit; accumulated CP of all units in all stages can't be higher than this")|C
**/stagecp|block|확인|단계별로 쓸 수 있는 CP. 값의 개수가 단계 수다(5개면 5단계) (확인: values.set "number of entries determine the number of stages")|C
**/specialcp|block|확인|특수 단계(공습 탭)의 CP (확인: values.set, 주석 "airstrikes special tab")|C
**/emplacementcp|block|확인|공병이 짓는 진지 단계의 CP (확인: values.set, 주석 "buildable with engineer")|C
**/bots/startmp|block|확인|AI의 시작 MP 배율. FinishMP보다 커지지 않는다. min·max로 최종값을 자를 수 있다 (확인: values.set StartMP)|C
**/bots/finishmp|block|확인|AI MP가 20분 동안 시작값에서 늘어나는 끝값의 배율. 플레이어 군대 MP에 곱한다 (확인: values.set FinishMP "start to finish in 20 minutes", "Multiplier * player army (MP value) + add")|C
**/bots/cp/base|block|확인|AI의 CP. "지난 초:CP" 단계표이고 게임 모드의 CP 설정을 덮어쓴다 (확인: values.set "Overwrites CP settings in gamemode. seconds : amount CP")|C
**/bots/cp/roundmultiplier|block|확인|판 수에 따라 AI CP에 곱하는 배수. "판 수:배수" 단계표 (확인: values.set, 주석 "games played : multiplier for CP")|C
**/bots/cp/attackmultiplier|block|미확인|뜻 미확인(이름은 공격 배수). 주석이 없다|S
**/defensebudgetplayer|block|확인|플레이어 쪽에 미리 두는 수비 유닛의 예산 배율. 봇 StartMP(공격 배율 제외)에 곱하고, 깃발 수를 곱한다 (확인: values.set DefenseBudgetPlayer, 주석 "budget it multiplied by number of flags")|C
**/defensebudgetai|block|확인|AI 쪽에 미리 두는 수비 유닛의 예산 배율. 봇 StartMP(공격 배율 제외)에 곱한다 (확인: values.set DefenseBudgetAI)|C
*/unlock|block|확인|이 수비 단계가 열리는 조건. games 뒤가 판 수, progress 뒤가 캠페인 진행률(0~1)이고 먼저 맞는 것으로 열린다 (확인: values.set unlock)|C
**/paybackfactor|block|확인|유닛(병사·분대·차량)을 잃었을 때 돌려받는 비율. 값 넷은 MP·SP·AP·RP 순이고 적지 않은 것은 1이다 (확인: values.set PaybackFactor, 주석 "{factor MP / SP / AP / RP} Assumes value of 1 if not defined")|C
**/sellfactor|block|확인|유닛을 퇴역시킬 때 돌려받는 비율. 값은 MP·SP·AP·RP 순 (확인: values.set SellFactor)|C
**/scavengedvehiclesellfactor|block|확인|전투 중 노획한 차량을 퇴역시킬 때 돌려받는 비율. 값은 MP·SP·AP·RP 순 (확인: values.set ScavengedVehicleSellFactor)|C
**/riskfactor|block|확인|임무마다 무작위로 붙는 추가 난이도(Low·Standard·High) 묶음 (확인: values.set RiskFactor)|C
**/botresources|block|확인|이 위험도에서 봇의 StartMP·FinishMP·CP에 곱하는 배율 (확인: values.set BotResources)|C
**/botveterancy|block|확인|이 위험도에서 적 유닛의 숙련도 (확인: values.set BotVeterancy)|C
**/rewards|block|확인|이 위험도에서 이겼을 때 얻는 모든 자원에 곱하는 배율 (확인: values.set Rewards)|C
**/resources/mp|block|확인|MP(인력. 유닛을 사는 돈) 묶음 (확인: 주석 "Manpower (generic money)")|C
**/resources/sp|block|확인|SP(공습에 쓰는 특수 포인트) 묶음 (확인: 주석 "Special Points (airstrikes)")|C
**/resources/ap|block|확인|AP(탄약 포인트) 묶음 (확인: 주석 "Ammo points")|C
**/resources/rp|block|확인|RP(연구 포인트) 묶음 (확인: 주석 "Research points")|C
*/startval|block|확인|컨퀘스트를 시작할 때 이 자원의 양 (확인: values.set StartVal)|C
*/wingain|block|확인|이긴 판마다 얻는 이 자원의 양. "판 수:양" 단계표이거나 수 하나 (확인: values.set WinGain "depending on how many games were played")|C
*/losegain|block|확인|진 판마다 얻는 이 자원의 양. "판 수:양" 단계표이거나 수 하나 (확인: values.set LoseGain)|C
*/maprewards|block|확인|맵 종류별 추가 보상. 한 판은 무작위로 고른 한 종류다 (확인: values.set MapRewards)|C
**/maprewards/airfield|block|확인|맵 종류가 비행장(Airfield)일 때 추가 보상 (확인: values.set MapRewards)|C
**/maprewards/ammodepot|block|확인|맵 종류가 탄약고(Ammodepot)일 때 추가 보상 (확인: values.set MapRewards)|C
**/maprewards/bonus|block|확인|맵 종류가 보너스(Bonus)일 때 추가 보상 (확인: values.set MapRewards)|C
**/maprewards/factory|block|확인|맵 종류가 공장(Factory)일 때 추가 보상 (확인: values.set MapRewards)|C
**/maprewards/research|block|확인|맵 종류가 연구소(Research)일 때 추가 보상 (확인: values.set MapRewards)|C
**/budget/start|block|확인|전투를 시작할 때 주는 CP. "판 수:CP" 단계표 (확인: 주석 "games played : starting CP of each match")|C
**/budget/pointspersecond|block|확인|전투 중 초당 CP 수입 (확인: 주석 "CP income")|C
**/budget/limit|block|확인|전투 중 모을 수 있는 CP 최댓값 (확인: 주석 "Max CP")|C
**/resupplycost|block|확인|보급 비용 묶음. 비용이 0이어도 모두 적용된다 (확인: values.set ResupplyCost)|C
**/resupplycost/human/newitem|block|확인|다시 채우는 아이템(떨어뜨린 무기, 쓴 수류탄 등) 하나의 비용 (확인: values.set NewItem)|C
**/resupplycost/human/weight|block|확인|보병에게 채우는 아이템 1kg마다 비용 (확인: values.set Weight)|C
**/resupplycost/human/mechanic|block|확인|보병만. 쓴 공병 자재(건설 자재) 1점마다 비용 (확인: values.set Mechanic)|C
**/resupplycost/vehicles/weight|block|확인|차량에 채우는 아이템 1kg마다 비용 (확인: values.set Weight)|C
**/resupplycost/vehicles/fuel|block|확인|차량만. 연료 1L마다 비용 (확인: values.set Fuel "Per liter")|C
**/resupplycost/vehicles/resupply|block|확인|차량만. 쓴 보급 1점마다 비용 (확인: values.set Resupply)|C
**/brokencomponent/body|block|확인|차체가 부서졌을 때 수리 비용. "비율 % 자원"을 하나 이상 적는다 (확인: values.set BrokenComponent)|C
**/brokencomponent/turret|block|확인|포탑이 부서졌을 때 수리 비용 (확인: values.set BrokenComponent)|C
**/brokencomponent/engine|block|확인|엔진이 부서졌을 때 수리 비용 (확인: values.set BrokenComponent)|C
**/brokencomponent/track|block|확인|궤도가 부서졌을 때 수리 비용 (확인: values.set BrokenComponent)|C
**/brokencomponent/shield|block|확인|방패가 부서졌을 때 수리 비용 (확인: values.set BrokenComponent)|C
**/brokencomponent/other|block|확인|위 어느 부품도 아닐 때의 수리 비용. 꼭 있어야 한다 (확인: 주석 "If none of the above. Required.")|C
**/support/airstrikes|block|확인|공습 비용 (확인: values.set ResupplyCost 형식)|C
# ---------- 컨퀘스트 길이 (set/dynamic_campaign/duration_*.set)
**/flagcount|block|확인|판마다 깃발 수. 한 칸이 한 판이고 "켜진 깃발 수:고를 범위"(예 1:5 = 앞의 5개 가운데 1개) (확인: values.set FlagCount, 주석 "each entry is 1 game played")|C
**/bots/researchstages|block|확인|봇이 쓸 수 있는 연구 단계. "판 수:단계"(예 3:2 = 3판을 하면 2단계가 열린다). 순서가 중요하다 (확인: values.set ResearchStages)|C
**/mapselection|block|확인|판 수에 따라 쓸 맵. "판 수:mapSelection 값"(예 3:2-4 = 3판 뒤 mapSelection 2~4인 맵) (확인: values.set MapSelection)|C
**/mapborder|block|확인|판 수에 따라 쓰는 맵 경계. "판 수:경계 번호"(예 3:2 = 3판 뒤 경계 2) (확인: values.set MapBorder)|C
# ---------- 게임 모드 (set/multiplayer/games/*.set)
/game/order|block|미확인|뜻 미확인(게임 모드 목록의 순서로 보인다). 주석이 없다|S
/game/parameters|block|확인|게임 모드 매개변수. "이름=값;…"의 조각마다 한 설정이다 (확인: 게임 모드 .set의 parameters 문자열과 그 주석)|C
/game/settings/scorefinal|block|문서|전투가 끝나는 점수. 기본 1000이고 올리면 전투가 길어진다고 한다 (문서: 가이드. campaign_capture_the_flag.set 주석은 1000점에 이르는 시간 표를 적는다)|steamcommunity.com/sharedfiles/filedetails/?id=3610240102 (findings-goh.psv)
/game/settings/scoremin|block|추정|게임 설정 화면에서 고를 수 있는 종료 점수의 최솟값 (추정: 이름과 options의 scoreFinal)|S
/game/settings/scoremax|block|추정|게임 설정 화면에서 고를 수 있는 종료 점수의 최댓값 (추정: 이름과 options의 scoreFinal)|S
/game/settings/preparationtime|block|확인|준비 시간(초). 플레이어가 수비일 때 봇이 공격을 시작하기 전까지의 시간 (확인: campaign_capture_the_flag.set 주석 "seconds. is sets how long before bot begins an attack when player is the defender")|C
/game/settings/preparationtimemin|block|추정|고를 수 있는 준비 시간의 최솟값(초) (추정: 이름)|S
/game/settings/preparationtimemax|block|문서|고를 수 있는 준비 시간의 최댓값(초). 너무 길면 수비가 지나치게 유리하다 (문서: 가이드가 frontlines.set 주석을 인용)|steamcommunity.com/sharedfiles/filedetails/?id=3610248314 (findings-goh.psv)
/game/settings/pauseduringpreparation|block|확인|준비 시간 동안 시뮬레이션을 멈출지 (확인: 주석 "controls if simulation is paused during preparation time")|C
/game/settings/impregnabletimeout|block|확인|스폰 보호 시간(초) (확인: 주석 "Spawn protection time")|C
/game/settings/difficulty|block|추정|이 모드가 쓰는 난이도 파일 이름(set/difficulty/<이름>.dl) (추정: 값 multiplayer와 파일 이름이 같다)|S
**/unitlimits|block|추정|유닛 분류별 최대 수. "분류=수;…"의 조각마다 한 분류 (추정: 이름과 값 officer=2 sniper=5. 게임 안 확인 3에서 본다)|S
**/common/commandpoints|block|추정|시간별 CP. 프리셋 매크로의 cp() 값이 "*=…"로 들어간다 (추정: preset_*.inc의 {commandPoints "*=%cp"})|S
**/budgets/vehicle/resource/start|block|추정|스커미시 시작 MP. 프리셋 매크로의 start()가 들어간다 (추정: budget.inc {start %s})|S
**/budgets/vehicle/resource/finish|block|추정|스커미시 MP가 growTime 동안 늘어나는 끝값. 프리셋 매크로의 finish()가 들어간다 (추정: budget.inc {finish %f})|S
**/budgets/vehicle/resource/growtime|block|확인|MP가 시작값에서 끝값으로 늘어나는 시간(초) (확인: budget.inc 주석 "in seconds")|C
**/budgets/vehicle/resource/growtable|block|확인|growTime을 나눈 구간마다의 증가 비율 목록. 항목 수 = growTime / 구간 (확인: 주석 "each entry divides divides grow time")|C
**/budgets/vehicle/resource/payback/cp|block|확인|잃은 유닛 환급의 CP 구간 6개(소스 코드가 6개만 받는다) (확인: budget.inc 주석 "source code only supports 6 values")|C
**/budgets/vehicle/resource/payback/restore|block|확인|CP 구간마다 돌려받는 비율 6개 (확인: budget.inc 주석 "source code only supports 6 values")|C
*/("standard_all")|macro|추정|스커미시 표준 프리셋. start = 시작 MP, finish = 끝 MP, doctrine = 교리 자원 시작값, cp = 시간별 CP (추정: preset_*.inc의 매크로 정의가 budget.inc와 commandPoints로 넘긴다)|S
*/("custom_all")|macro|추정|스커미시 사용자 설정 프리셋. 인자는 standard_all과 같다 (추정: preset_*.inc의 매크로 정의)|S
# ---------- 난이도 (set/difficulty/*.dl, difficulty_generic.inc)
/difficulty/any/translate_damage|block|확인|폭풍 피해가 보병에게 들어갈 때 곱하는 배수 (확인: difficulty_generic.inc 주석 "damage multiplier for blast-wave damage to humans")|C
/difficulty/any/cover_hitchance|block|확인|엄폐한 병사를 맞힐 확률 배수 (확인: 주석 "the probability of hitting a soldier behind cover")|C
/difficulty/any/cover_damage|block|확인|엄폐로 보호될 때의 피해 배수 (확인: 주석 "damage when protected by cover")|C
/difficulty/any/crawl_hitchance|block|확인|엎드린 병사를 맞힐 확률 배수 (확인: 주석)|C
/difficulty/any/crawl_damage|block|확인|엎드렸을 때 피해 배수 (확인: 주석 "damage when in prone position")|C
/difficulty/any/vehicle_burst|block|확인|모든 차량의 점사 1회 탄수 배수. 1보다 작으면 줄어든다 (확인: 주석)|C
/difficulty/any/cover_mgun_burst|block|확인|수평 엄폐 중인 기관총 사수의 점사 탄수 배수 (확인: 주석)|C
/difficulty/any/cover_mgun_delay|block|확인|수평 엄폐 중인 기관총 사수의 점사 사이 대기 배수 (확인: 주석 "decrease timeout between bursts")|C
/difficulty/any/recoil|block|확인|모든 기관총의 반동 배수 (확인: 주석)|C
/difficulty/any/recoil_zoom|block|확인|조준(줌) 때 반동 배수 (확인: 주석)|C
/difficulty/any/vehicle_recoil|block|확인|차량에 단 기관총의 반동 배수 (확인: 주석)|C
/difficulty/any/cover_efficiency|block|확인|거리에 따른 엄폐 효율 표. 줄마다 "거리(m) 효율" (확인: 주석 "how cover efficiency will drop with distance")|C
/difficulty/any/blast_crawl|block|확인|폭발 에너지를 셀 때 엎드린 병사의 거리 배수(2.0 = 두 배 멀리 있는 것으로 친다) (확인: 주석)|C
/difficulty/any/blast_cover|block|확인|보호되는 쪽 엄폐 중의 폭발 거리 배수 (확인: 주석)|C
/difficulty/any/blast_crawl_prepared|block|확인|엎드려 폭발을 기다릴 때의 폭발 거리 배수 (확인: 주석)|C
/difficulty/any/blast_cover_prepared|block|확인|보호되지 않는 쪽 엄폐에서 폭발을 기다릴 때의 폭발 거리 배수 (확인: 주석)|C
/difficulty/any/blast_squat|block|확인|무릎 쏴 자세의 폭발 거리 배수 (확인: 주석)|C
/difficulty/any/death_delay_min|block|확인|쓰러진 병사가 출혈로 죽기까지 최소 시간(초) (확인: 주석 "min bleeding out time in seconds")|C
/difficulty/any/death_delay_max|block|확인|쓰러진 병사가 출혈로 죽기까지 최대 시간(초) (확인: 주석)|C
/difficulty/any/firing_timeout|block|확인|점사 사이 대기 배수(무기의 FiringTimeout에 곱한다) (확인: 주석)|C
/difficulty/any/grenade_timeout|block|확인|수류탄 사이 대기 배수 (확인: 주석)|C
/difficulty/any/ai_damage|block|확인|AI 병사끼리 주는 피해 배수. 플레이어가 조종하는 유닛에는 적용되지 않는다 (확인: 주석)|C
/difficulty/any/burst|block|확인|무기 설정의 점사 탄수에 곱하는 배수 (확인: 주석 "multiplier to number of bullets in burst from weapon settings")|C
/difficulty/any/accuracy|block|확인|무기 설정의 명중에 곱하는 배수 (확인: 주석 "accuracy multiplier from weapon settings")|C
/difficulty/any/health|block|확인|병사 설정의 체력에 곱하는 배수 (확인: 주석 "health multiplier from soldier settings")|C
/difficulty/enemy/health|block|확인|적 유닛의 체력 배수. 0.5면 적이 피해를 두 배로 받는 것과 같다. 코옵에서는 동작하지 않는다 (확인: normal.dl 주석)|C
/difficulty/enemy/accuracy|block|추정|적 유닛의 명중 배수 (추정: any의 accuracy와 같은 꼴. normal.dl 주석 "not working in coop")|S
/difficulty/ally/health|block|추정|아군 AI의 체력 배수 (추정: 블록 이름 ally와 any의 health)|S
/difficulty/ally/accuracy|block|추정|아군 AI의 명중 배수 (추정: 블록 이름 ally)|S
/difficulty/player/health|block|추정|플레이어 유닛의 체력 배수 (추정: 블록 이름 player)|S
/difficulty/player/accuracy|block|추정|플레이어 유닛의 명중 배수 (추정: 블록 이름 player)|S
```

- [ ] **Step 3: `editor/summary/settings.psv`를 쓴다 (CRLF)**

```text
# 게임 설정 요약 탭에 올리는 키. 형식과 규칙은 vehicle.psv와 같다. Lua 키는 단계 3(Task 9)에서 더한다.
*/startval|시작 자원 (MP·SP·AP·RP)
*/wingain|이긴 판마다 얻는 자원
*/losegain|진 판마다 얻는 자원
**/maprewards/airfield|맵 종류별 추가 보상
**/maprewards/ammodepot|맵 종류별 추가 보상
**/maprewards/bonus|맵 종류별 추가 보상
**/maprewards/factory|맵 종류별 추가 보상
**/maprewards/research|맵 종류별 추가 보상
**/budget/start|전투 시작 CP
**/budget/pointspersecond|초당 CP
**/budget/limit|전투 중 CP 상한
**/stagecp|단계별 CP
**/globalmaxcp|전체 CP 상한
**/paybackfactor|손실 환급 비율
**/sellfactor|퇴역 환급 비율
**/bots/startmp|AI 병력 (MP)
**/bots/finishmp|AI 병력 (MP)
**/bots/cp/base|AI 병력 (CP)
**/botresources|위험도별 배율
**/botveterancy|위험도별 배율
**/rewards|위험도별 배율
**/flagcount|판마다 깃발 수
**/bots/researchstages|봇 연구 단계
/game/parameters|모드 매개변수 (점수표·깃발 시간 등)
/game/settings/scorefinal|종료 점수
/game/settings/preparationtime|준비 시간 (초)
/game/settings/impregnabletimeout|스폰 보호 시간 (초)
**/unitlimits|유닛 수 제한
*/("standard_all")|스커미시 자원 (시작·끝 MP, CP)
*/("custom_all")|스커미시 자원 (시작·끝 MP, CP)
/difficulty/enemy/health|적·아군·플레이어 체력·명중
/difficulty/enemy/accuracy|적·아군·플레이어 체력·명중
/difficulty/ally/health|적·아군·플레이어 체력·명중
/difficulty/ally/accuracy|적·아군·플레이어 체력·명중
/difficulty/player/health|적·아군·플레이어 체력·명중
/difficulty/player/accuracy|적·아군·플레이어 체력·명중
/difficulty/any/health|모든 병사 체력·명중·점사
/difficulty/any/accuracy|모든 병사 체력·명중·점사
/difficulty/any/burst|모든 병사 체력·명중·점사
/difficulty/any/death_delay_min|출혈 사망 시간 (초)
/difficulty/any/death_delay_max|출혈 사망 시간 (초)
/difficulty/any/cover_efficiency|엄폐 효율
```

- [ ] **Step 4: `data/annotations/values.psv` 끝에 게임 설정 묶음을 더한다 (CRLF 유지)**

```text

# ---------- 게임 설정 (하위 프로젝트 B. 근거는 data/annotations/settings.psv와 같다)
**/bots/startmp|#1|시작 MP 배율 (플레이어 군대 MP에 곱한다)|C
**/bots/startmp|min#1|최종값의 하한|C
**/bots/startmp|attackerMultiplier#1|공격 배율 (뜻 미확인)|S
**/bots/finishmp|#1|끝 MP 배율|C
**/bots/finishmp|min#1|최종값의 하한|C
**/bots/finishmp|attackerMultiplier#1|공격 배율 (뜻 미확인)|S
*/unlock|games#1|열리는 판 수|C
*/unlock|progress#1|열리는 캠페인 진행률 (0~1)|C
**/paybackfactor|#1|MP 비율|C
**/paybackfactor|#2|SP 비율|C
**/paybackfactor|#3|AP 비율|C
**/paybackfactor|#4|RP 비율|C
**/sellfactor|#1|MP 비율|C
**/sellfactor|#2|SP 비율|C
**/sellfactor|#3|AP 비율|C
**/sellfactor|#4|RP 비율|C
**/scavengedvehiclesellfactor|#1|MP 비율|C
**/scavengedvehiclesellfactor|#2|SP 비율|C
**/scavengedvehiclesellfactor|#3|AP 비율|C
**/scavengedvehiclesellfactor|#4|RP 비율|C
**/resupplycost/human/newitem|#1|비용|C
**/resupplycost/human/newitem|#2|자원 (MP·AP 등)|C
**/resupplycost/human/weight|#1|1kg당 비용|C
**/resupplycost/human/weight|#2|자원 (MP·AP 등)|C
**/resupplycost/human/mechanic|#1|자재 1점당 비용|C
**/resupplycost/human/mechanic|#2|자원 (MP·AP 등)|C
**/resupplycost/vehicles/weight|#1|1kg당 비용|C
**/resupplycost/vehicles/weight|#2|자원 (MP·AP 등)|C
**/resupplycost/vehicles/fuel|#1|1L당 비용|C
**/resupplycost/vehicles/fuel|#2|자원 (MP·AP 등)|C
**/resupplycost/vehicles/resupply|#1|보급 1점당 비용|C
**/resupplycost/vehicles/resupply|#2|자원 (MP·AP 등)|C
*/body|#1|비율 (%)|C
*/body|#2|% 표시|C
*/body|#3|자원 (MP·AP 등)|C
*/body|#4|두 번째 비율 (%)|C
*/body|#5|% 표시|C
*/body|#6|두 번째 자원|C
**/support/airstrikes|#1|비율 (%)|C
**/support/airstrikes|#2|% 표시|C
**/support/airstrikes|#3|자원 (SP)|C
/game/parameters|@points_table_player|플레이어 점수표 "점령 비율/틱당 점수" 목록. 틱당 0.277이면 1000점까지 60분, 0.555면 30분 (파일 주석의 표)|C
/game/parameters|@points_table_ai|AI 점수표 "점령 비율/틱당 점수" 목록 (꼴은 플레이어 점수표와 같다)|C
/game/parameters|@points_table|점수표 "점령 비율/점수" 목록 (뜻 미확인)|S
/game/parameters|@flag_capture_time|깃발 점령 시간 (단위 미확인)|S
/game/parameters|@flag_release_time|깃발 해제 시간 (단위 미확인)|S
/game/parameters|@flag_capture_factor|깃발 점령 계수 (뜻 미확인)|S
/game/parameters|@kill_score_multiplier|처치 점수 배수 (뜻 미확인)|S
/game/parameters|@flag_count|깃발 수 표 (꼴 미확인)|S
/game/parameters|@scoreStart|시작 점수 (미확인. 게임 안 확인 2에서 본다)|S
/game/parameters|@teamsize_cp_factor|팀 크기별 CP 배율 "최대 팀 크기:배율" (1:1.3 = 1대1이면 CP +30%)|C
**/unitlimits|@officer|officer 분류의 최대 수 (미확인. 게임 안 확인 3에서 본다)|S
**/unitlimits|@flamer|flamer 분류의 최대 수 (미확인)|S
**/unitlimits|@sniper|sniper 분류의 최대 수 (미확인)|S
*/("standard_all")|start|시작 MP|S
*/("standard_all")|finish|끝 MP (growTime 동안 늘어난다)|S
*/("standard_all")|doctrine|교리 자원 시작값|S
*/("standard_all")|cp|시간별 CP "시간:CP,…" (꼴 미확인)|S
*/("custom_all")|start|시작 MP|S
*/("custom_all")|finish|끝 MP (growTime 동안 늘어난다)|S
*/("custom_all")|doctrine|교리 자원 시작값|S
*/("custom_all")|cp|시간별 CP "시간:CP,…" (꼴 미확인)|S
```

`teamsize_cp_factor`의 근거는 `battle_zones.set` 4줄의 주석("Means a 1v1 game match will have global CP multiplier of 1.3")이다.

- [ ] **Step 5: 줄 끝을 확인한다**

Run: `file editor/settings/files.psv editor/summary/settings.psv data/annotations/settings.psv data/annotations/values.psv`
Expected: 넷 모두 `with CRLF line terminators`. 아니면 `sed -i 's/\r*$/\r/' <파일>`.

- [ ] **Step 6: 실제 파일로 주석이 붙는지 본다**

Run:
```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- units --kind settings --limit 40
```
Expected: 게임 설정 22개가 `files.psv`의 이름과 함께 나온다(첫 실행은 색인을 다시 만든다). 이름이 없는 파일이 있으면 그 경로를 `files.psv`에 더한다.

- [ ] **Step 7: 전체 테스트와 커밋**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1 2>&1 | tail -1` → `all test files passed`

```bash
git add editor/settings/files.psv editor/summary/settings.psv data/annotations/settings.psv data/annotations/values.psv
git commit -m "docs(editor): names, notes and summary keys for the game settings files"
```

- [ ] **Step 8: 단계 1을 병합한다**

```bash
git checkout develop
git merge --no-ff feat/settings-kind -m "Merge branch 'feat/settings-kind' into develop"
```

---

## 단계 2: 문자열 속 설정과 단계표 (브랜치 `feat/settings-strings`)

### Task 4: `SettingStrings`와 새 값 검사의 한 입구 `ValueRules`

**Files:**
- Create: `editor/CtaToybox.Core/SettingStrings.cs`
- Create: `editor/CtaToybox.Core/ValueRules.cs`
- Modify: `editor/CtaToybox.Core/EditorSession.Edits.cs` (`ValidateNew`)
- Modify: `editor/CtaToybox.Cli/Program.cs` (`Add`의 검사)
- Modify: `editor/CtaToybox.Core/ModBuilder.cs` (`StageItem`의 검사)
- Create: `editor/CtaToybox.Core.Tests/SettingStringsTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: Task 1의 `SettingFiles.InArea`
- Produces:
  - `sealed class SettingPiece { string Name; string Value; }`
  - `List<SettingPiece> SettingStrings.Split(string token)` (아니면 null), `string SettingStrings.Join(string originalToken, IList<string> values)`, `string SettingStrings.CheckPieces(string oldToken, string newToken)` (맞으면 null)
  - `bool SettingStrings.IsStepTable(string token)`, `string SettingStrings.CheckSteps(string oldToken, string newToken)` (맞으면 null), `string SettingStrings.StepPreview(string token)` (아니면 null)
  - `string ValueRules.CheckNewValue(string file, string oldText, string newText, out string warning)` (맞으면 null). Task 7이 Lua 갈래를 더한다

- [ ] **Step 1: 브랜치를 만든다**

```bash
git checkout develop
git checkout -b feat/settings-strings
```

- [ ] **Step 2: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/SettingStringsTests.cs`:

```csharp
// 문자열 속 설정("이름=값;…")과 판 수 단계표("1:200 4:250"), 새 값 검사의 한 입구
using System.Linq;
using CtaToybox.Core;

static class SettingStringsTests
{
    public static void Run()
    {
        const string p = "\"flag_capture_time=60;scoreStart=250\"";
        var pieces = SettingStrings.Split(p);
        T.Eq(new[] { "flag_capture_time", "scoreStart" }, pieces.Select(x => x.Name).ToArray(), "piece names");
        T.Eq(new[] { "60", "250" }, pieces.Select(x => x.Value).ToArray(), "piece values");
        T.Eq(p, SettingStrings.Join(p, new[] { "60", "250" }), "split and join give the same bytes");
        T.Eq("\"flag_capture_time=60;scoreStart=777\"", SettingStrings.Join(p, new[] { "60", "777" }), "join changes only values");
        T.Eq("\"a=1;b=2;\"", SettingStrings.Join("\"a=1;b=2;\"", new[] { "1", "2" }), "a trailing ; is kept");
        T.Eq(2, SettingStrings.Split("\"points_table_player=0/0.000,0.333/0.277;flag_capture_time=75\"").Count, "a piece value may hold a list");
        T.True(SettingStrings.Split("\"*=1.00\"") == null, "one piece or a non-word name is not a piece string");
        T.True(SettingStrings.Split("\"0:500\"") == null && SettingStrings.Split("250") == null, "other values are not piece strings");

        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=777\"") == null, "a changed value is fine");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scorestart=777\"") != null, "a renamed piece is refused");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=7;x=1\"") != null, "an extra piece (a ; typed into a value) is refused");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=7=1\"") != null, "an = typed into a value is refused");
        T.True(SettingStrings.CheckPieces(p, "\"flag_capture_time=60;scoreStart=\"") != null, "an empty value is refused");
        T.True(SettingStrings.CheckPieces("\"a=1;b=2;\"", "\"a=1;b=3\"") != null, "the trailing ; stays");
        T.True(SettingStrings.CheckPieces("250", "300") == null, "not a piece string: nothing to check");

        T.True(SettingStrings.IsStepTable("\"1:200 4:250 6:300\"") && SettingStrings.IsStepTable("\"0:500\"") && SettingStrings.IsStepTable("\"2:5 2:5 3:5\""), "step tables");
        T.True(!SettingStrings.IsStepTable("\"0:1-4\"") && !SettingStrings.IsStepTable("\"a:1\"") && !SettingStrings.IsStepTable("200"), "not step tables");
        T.True(SettingStrings.CheckSteps("\"1:200 4:250\"", "\"1:300 4:350 8:400\"") == null, "more entries of the same shape are fine");
        T.True(SettingStrings.CheckSteps("\"1:200 4:250\"", "\"1:300,4:350\"") != null, "commas are refused");
        T.True(SettingStrings.CheckSteps("\"2:5 2:5\"", "\"2:5:1 2:5\"") != null, "each entry keeps its count of numbers");
        T.Eq("1 → 200, 4 → 250", SettingStrings.StepPreview("\"1:200 4:250\""), "preview");
        T.True(SettingStrings.StepPreview("250") == null, "no preview for other values");

        const string area = "set/multiplayer/games/battle_zones.set", unit = "set/breed/mp/ger/mid/medic.set";
        string warning;
        T.True(ValueRules.CheckNewValue(area, p, "\"flag_capture_time=60;scoreStart=7;x=1\"", out warning) != null, "ValueRules checks pieces in the settings area");
        T.True(ValueRules.CheckNewValue(area, "\"1:200\"", "\"1:200,4:5\"", out warning) != null, "and step tables");
        T.True(ValueRules.CheckNewValue(unit, "\"a=1;b=2\"", "\"a=1;b=2;c=3\"", out warning) == null, "not outside the settings area");
        T.True(ValueRules.CheckNewValue(area, "250", "2 0", out warning) != null, "the token rules still apply");
    }
}
```

`Program.cs` 스위트 목록의 `("SettingsSession", SettingsSessionTests.Run),` 다음에 넣는다:

```csharp
            ("SettingStrings", SettingStringsTests.Run),
```

- [ ] **Step 3: 테스트가 실패하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'SettingStrings'` 이름이 없다.

- [ ] **Step 4: `SettingStrings.cs`를 쓴다**

```csharp
// 게임 설정 문자열의 두 꼴. 스펙: docs/superpowers/specs/2026-10-15-settings-editor-design.md
//  - 문자열 속 설정 "이름=값;이름=값": 게임 모드의 parameters, unitLimits 등. 조각의 이름·개수·순서는 두고 값만 고친다
//  - 판 수 단계표 "1:200 4:250": WinGain, FlagCount 등. 칸은 공백으로 나누고 칸마다 ':'로 이은 수의 개수가 같다
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtaToybox.Core
{
    public sealed class SettingPiece
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public static class SettingStrings
    {
        static readonly Regex PieceName = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.Compiled);
        static readonly Regex StepEntry = new Regex(@"^\d+(\.\d+)?(:\d+(\.\d+)?)+$", RegexOptions.Compiled);

        static string Body(string token)
        {
            if (token == null || token.Length < 2 || token[0] != '"' || token[token.Length - 1] != '"') return null;
            return token.Substring(1, token.Length - 2);
        }

        // 따옴표 문자열이 "이름=값;…"(조각 둘 이상, 이름은 낱말) 꼴이면 조각들. 아니면 null. 끝의 ;는 조각으로 치지 않는다
        public static List<SettingPiece> Split(string token)
        {
            string body = Body(token);
            if (body == null) return null;
            if (body.EndsWith(";")) body = body.Substring(0, body.Length - 1);
            var result = new List<SettingPiece>();
            foreach (string part in body.Split(';'))
            {
                int eq = part.IndexOf('=');
                if (eq <= 0 || !PieceName.IsMatch(part.Substring(0, eq))) return null;
                result.Add(new SettingPiece { Name = part.Substring(0, eq), Value = part.Substring(eq + 1) });
            }
            return result.Count >= 2 ? result : null;
        }

        // 원래 문자열의 조각 이름·순서·끝의 ;를 두고 값만 바꿔 잇는다. values는 조각 순서대로다
        public static string Join(string originalToken, IList<string> values)
        {
            var pieces = Split(originalToken);
            if (pieces == null) throw new ArgumentException("문자열 속 설정이 아니다: " + originalToken);
            if (values.Count != pieces.Count) throw new ArgumentException("조각 수가 다르다: " + values.Count + " / " + pieces.Count);
            bool trailing = Body(originalToken).EndsWith(";");
            return "\"" + string.Join(";", pieces.Select((p, i) => p.Name + "=" + values[i])) + (trailing ? ";" : "") + "\"";
        }

        // 문자열 속 설정의 새 값 검사. 원래 값이 문자열 속 설정이 아니면 검사할 것이 없다(null). 맞으면 null
        public static string CheckPieces(string oldToken, string newToken)
        {
            var before = Split(oldToken);
            if (before == null) return null;
            var after = Split(newToken);
            if (after == null || after.Count != before.Count)
                return "조각(이름=값)의 개수와 꼴은 원래와 같아야 한다. 값에 ; 를 쓸 수 없다: " + newToken;
            for (int i = 0; i < before.Count; i++)
            {
                if (after[i].Name != before[i].Name) return (i + 1) + "번째 조각의 이름은 " + before[i].Name + "이어야 한다(지금 " + after[i].Name + ")";
                if (after[i].Value.Length == 0) return before[i].Name + "의 값이 비어 있다";
                if (after[i].Value.Contains('=') && !before[i].Value.Contains('=')) return before[i].Name + "의 값에 = 를 쓸 수 없다";
            }
            if (Body(oldToken).EndsWith(";") != Body(newToken).EndsWith(";")) return "끝의 ; 는 원래대로 둔다";
            return null;
        }

        // 칸마다 ':'로 이은 수의 개수. 단계표가 아니거나 칸마다 개수가 다르면 null
        static int[] StepShape(string token)
        {
            string body = Body(token);
            if (body == null || body.Trim().Length == 0) return null;
            var entries = Regex.Split(body.Trim(), @"\s+");
            if (!entries.All(e => StepEntry.IsMatch(e))) return null;
            var shape = entries.Select(e => e.Split(':').Length).ToArray();
            return shape.Distinct().Count() == 1 ? shape : null;
        }

        public static bool IsStepTable(string token) { return StepShape(token) != null; }

        // 단계표의 새 값 검사: 같은 꼴(칸마다 수의 개수가 같다)이어야 한다. 칸 수는 달라도 된다. 맞으면 null
        public static string CheckSteps(string oldToken, string newToken)
        {
            var before = StepShape(oldToken);
            if (before == null) return null;
            var after = StepShape(newToken);
            if (after == null) return "\"수:수 수:수\" 꼴이어야 한다(칸은 공백으로 나눈다): " + newToken;
            if (after[0] != before[0]) return "칸마다 ':'로 이은 수가 " + before[0] + "개여야 한다";
            return null;
        }

        // 읽기용 풀이: "1:200 4:250" → "1 → 200, 4 → 250". 단계표가 아니면 null
        public static string StepPreview(string token)
        {
            if (StepShape(token) == null) return null;
            return string.Join(", ", Regex.Split(Body(token).Trim(), @"\s+").Select(e =>
            {
                var p = e.Split(':');
                return p[0] + " → " + string.Join(" : ", p.Skip(1));
            }));
        }
    }
}
```

- [ ] **Step 5: `ValueRules.cs`를 쓴다**

```csharp
// 새 값 검사를 한곳에 모은다. 화면(EditorSession.ValidateNew), 명령줄(add), 빌드(ModBuilder)가 같은 규칙을 쓴다.
// 게임 설정이 있는 폴더(SettingFiles.InArea)의 파일이면 문자열 속 설정과 단계표의 꼴도 본다
namespace CtaToybox.Core
{
    public static class ValueRules
    {
        // 맞으면 null. warning은 막지 않는 경고다
        public static string CheckNewValue(string file, string oldText, string newText, out string warning)
        {
            string error = TokenPatcher.CheckNewValue(oldText, newText, out warning);
            if (error != null || !SettingFiles.InArea(file)) return error;
            return SettingStrings.CheckPieces(oldText, newText) ?? SettingStrings.CheckSteps(oldText, newText);
        }
    }
}
```

- [ ] **Step 6: 세 입구가 `ValueRules`를 쓰게 한다**

`editor/CtaToybox.Core/EditorSession.Edits.cs`의 `ValidateNew`에서
```csharp
            return TokenPatcher.CheckNewValue(value.Trace.Text, newText, out warning);
```
를
```csharp
            return ValueRules.CheckNewValue(value.Trace.File, value.Trace.Text, newText, out warning);
```
로 바꾼다.

`editor/CtaToybox.Cli/Program.cs`의 `Add`에서
```csharp
                string error = TokenPatcher.CheckNewValue(node.Text, newValue, out warning);
```
를
```csharp
                string error = ValueRules.CheckNewValue(file.VirtualPath, node.Text, newValue, out warning);
```
로 바꾼다.

`editor/CtaToybox.Core/ModBuilder.cs`의 `StageItem`에서
```csharp
                if (e.New != null && !TextCodec.CanEncode(e.New, kind))
                    results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "새 값에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + e.New });
                else usable.Add(e);
```
를
```csharp
                string ruleWarning;
                string ruleError = e.New == null ? null : ValueRules.CheckNewValue(item.Source.VirtualPath, e.Old, e.New, out ruleWarning);
                if (e.New != null && !TextCodec.CanEncode(e.New, kind))
                    results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = "새 값에 이 파일의 인코딩(" + kind + ")으로 쓸 수 없는 글자가 있다: " + e.New });
                else if (ruleError != null)
                    results.Add(new EditResult { Edit = e, Status = EditStatus.Invalid, Message = ruleError });
                else usable.Add(e);
```
로 바꾼다.

- [ ] **Step 7: 테스트가 통과하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: `failed=0`

- [ ] **Step 8: 커밋한다**

```bash
git add editor/CtaToybox.Core/SettingStrings.cs editor/CtaToybox.Core/ValueRules.cs editor/CtaToybox.Core/EditorSession.Edits.cs editor/CtaToybox.Cli/Program.cs editor/CtaToybox.Core/ModBuilder.cs editor/CtaToybox.Core.Tests/SettingStringsTests.cs editor/CtaToybox.Core.Tests/Program.cs
git commit -m "feat(editor): embedded name=value settings and step tables - split, join and check in one place"
```

### Task 5: 조각과 단계표를 화면에 보이기

**Files:**
- Modify: `editor/CtaToybox.Core/EditorSession.cs` (`ValueView` 속성, `OpenUnit`의 값 이름 루프)
- Modify: `editor/CtaToybox.Core/ValueLabels.cs` (`PieceLabel`)
- Modify: `editor/CtaToybox.App/Items.cs` (`Text.ValuesLine`)
- Modify: `editor/CtaToybox.App/EditDialog.xaml.cs` (조각 칸, 단계표 풀이, `TypePiece`)
- Modify: `editor/CtaToybox.App/MainWindow.Scenario.cs` (`piece` 명령)
- Modify: `editor/CtaToybox.Core.Tests/SettingsSessionTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/ValueLabelsTests.cs`

**Interfaces:**
- Consumes: Task 4의 `SettingStrings`, Task 3의 `values.psv` `@이름` 자리
- Produces:
  - `ValueView.Pieces` (`List<SettingPiece>`, 아니면 null), `ValueView.PieceLabels` (`List<string>`, 조각 순서, 모르면 그 자리 null), `ValueView.Steps` (string, 아니면 null)
  - `string ValueLabels.PieceLabel(string path, string rawPath, string name)`
  - `EditDialog.TypePiece(int valueIndex, string pieceName, string text)`, 시나리오 명령 `piece <n> <조각 이름> <글자>`

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/ValueLabelsTests.cs`의 `Run` 끝에 넣는다:

```csharp
        // 문자열 속 설정 조각의 뜻(자리 @이름)
        string file3 = Path.Combine(dir, "values3.psv");
        File.WriteAllText(file3, "/game/parameters|@scoreStart|시작 점수|S\n**/unitlimits|@officer|장교 수|S\n");
        var l3 = ValueLabels.Load(file3);
        T.Eq("시작 점수", l3.PieceLabel("/game/parameters", null, "scoreStart"), "a piece label by exact path");
        T.Eq("장교 수", l3.PieceLabel("/game/presets/\"*\"/common/unitlimits", "/game/presets/\"s:x\"/common/unitLimits", "OFFICER"), "by tail, piece names ignore case");
        T.True(l3.PieceLabel("/game/parameters", null, "flag_count") == null, "an unknown piece has no label");
```

`editor/CtaToybox.Core.Tests/SettingsSessionTests.cs`의 `using (var session = …)` 블록 끝(`T.Eq(SettingFilesTests.Limits, …)` 다음)에 넣는다:

```csharp
            // 조각과 단계표: 화면이 쓰는 값
            var parameters = bz.Rows.First(r => r.Path == "/game/parameters").Values[0];
            T.Eq(new[] { "flag_capture_time", "scoreStart" }, parameters.Pieces.Select(x => x.Name).ToArray(), "the session splits piece strings");
            T.Eq(2, parameters.PieceLabels.Count, "with one label slot per piece");
            var dur = session.OpenUnit(SettingFilesTests.Dur);
            T.Eq("2 → 5, 2 → 5, 3 → 5", dur.Rows.First(r => r.Path == "/normal/flagcount").Values[0].Steps, "step tables get a preview");
            T.True(parameters.Steps == null && dur.Rows.First(r => r.Path == "/normal/flagcount").Values[0].Pieces == null, "a piece string is not a step table and the other way round");

            // 같은 문자열의 조각을 차례로 고쳐도 앞의 것이 남는다(창은 기존 수정의 새 값에서 시작한다)
            session.OpenProject(Path.Combine(T.TempDir(), "pieces.json"));
            var row = bz.Rows.First(r => r.Path == "/game/parameters");
            session.ApplyRow(bz, row, new[] { SettingStrings.Join(row.Values[0].Trace.Text, new[] { "61", "250" }) }, "shared");
            session.RefreshEdits(bz);
            string current = row.Values[0].Edit.New;
            var start = SettingStrings.Split(current);
            session.ApplyRow(bz, row, new[] { SettingStrings.Join(row.Values[0].Trace.Text, new[] { start[0].Value, "777" }) }, "shared");
            T.Eq("\"flag_capture_time=61;scoreStart=777\"", session.Project.Edits.Single().New, "both piece changes are kept in one edit");
```

- [ ] **Step 2: 테스트가 실패하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'ValueLabels'에 'PieceLabel'…`, `'ValueView'에 'Pieces'…`.

- [ ] **Step 3: `ValueLabels.PieceLabel`을 더한다**

`editor/CtaToybox.Core/ValueLabels.cs`의 `Of` 메서드 앞에 넣는다:

```csharp
        // 문자열 속 설정 조각의 뜻(values.psv의 자리 @이름). 후보 경로를 모두 본다. 모르면 null
        public string PieceLabel(string path, string rawPath, string name)
        {
            foreach (string key in KeyPattern.Candidates(path, rawPath, true))
            {
                Dictionary<string, string> slots;
                string label;
                if (map.TryGetValue(key, out slots) && slots.TryGetValue("@" + name, out label)) return label;
            }
            return null;
        }
```

- [ ] **Step 4: `ValueView`에 조각·단계표를 넣는다**

`editor/CtaToybox.Core/EditorSession.cs`의 `ValueView`에서 `public string Label { get; set; }` 다음에 넣는다:

```csharp
        public List<SettingPiece> Pieces { get; set; }   // 문자열 속 설정("이름=값;…")의 조각. 게임 설정 폴더의 파일만. 아니면 null
        public List<string> PieceLabels { get; set; }    // 조각마다 뜻(values.psv의 @이름). 모르면 그 자리 null
        public string Steps { get; set; }                // 판 수 단계표의 읽기용 풀이. 아니면 null
```

`OpenUnit`의 값 이름 루프를 바꾼다:

```csharp
            foreach (var row in view.Rows.Concat(view.Roster))
            {
                var labels = valueLabels.Of(row.Path, row.Values.Select(v => v.Trace.Text).ToList(), row.Values.Select(v => v.Name).ToList(), row.RawPath);
                for (int i = 0; i < row.Values.Count; i++) row.Values[i].Label = labels[i];
            }
```
를
```csharp
            foreach (var row in view.Rows.Concat(view.Roster))
            {
                var labels = valueLabels.Of(row.Path, row.Values.Select(v => v.Trace.Text).ToList(), row.Values.Select(v => v.Name).ToList(), row.RawPath);
                for (int i = 0; i < row.Values.Count; i++)
                {
                    var v = row.Values[i];
                    v.Label = labels[i];
                    if (!v.Editable || !SettingFiles.InArea(v.Trace.File)) continue;
                    v.Pieces = SettingStrings.Split(v.Trace.Text);
                    if (v.Pieces != null) v.PieceLabels = v.Pieces.Select(p => valueLabels.PieceLabel(row.Path, row.RawPath, p.Name)).ToList();
                    v.Steps = SettingStrings.StepPreview(v.Trace.Text);
                }
            }
```

- [ ] **Step 5: 테스트가 통과하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: `failed=0`

- [ ] **Step 6: 표에 조각을 보인다**

`editor/CtaToybox.App/Items.cs`의 `ValuesLine` 맨 앞(`// 값이 여럿이고 뜻을 아는 값이 있으면 …` 앞)에 넣는다:

```csharp
            // 문자열 속 설정: 조각마다 한 줄 "뜻: 값"
            if (row.Values.Count == 1 && row.Values[0].Pieces != null)
            {
                var v0 = row.Values[0];
                return string.Join("\n", v0.Pieces.Select((p, i) => (v0.PieceLabels[i] ?? p.Name + " (뜻 미확인)") + ": " + p.Value));
            }
```

- [ ] **Step 7: 값 고치기 창에 조각 칸과 단계표 풀이를 넣는다**

`editor/CtaToybox.App/EditDialog.xaml.cs`:

필드 `readonly Dictionary<int, TextBox> boxes = …;` 다음에 넣는다:

```csharp
        readonly Dictionary<int, List<TextBox>> pieceBoxes = new Dictionary<int, List<TextBox>>();   // 문자열 속 설정: 값 자리 → 조각 칸
        readonly Dictionary<int, TextBlock> stepTexts = new Dictionary<int, TextBlock>();             // 단계표: 값 자리 → 풀이
```

생성자의 값 루프에서 `DockPanel.SetDock(caption, Dock.Left);`부터 `ValuesPanel.Children.Add(line);`까지를 아래로 바꾼다:

```csharp
                if (v.Editable && v.Pieces != null)
                {
                    // 문자열 속 설정: 조각마다 칸. 이미 수정이 있으면 그 새 값에서 시작한다(앞에 고친 조각이 남는다)
                    var start = SettingStrings.Split(v.Edit != null ? v.Edit.New : v.Trace.Text) ?? v.Pieces;
                    var block = new StackPanel { Margin = new Thickness(0, 3, 0, 3) };
                    block.Children.Add(caption);
                    var list = new List<TextBox>();
                    for (int j = 0; j < v.Pieces.Count; j++)
                    {
                        var pieceLine = new DockPanel { Margin = new Thickness(12, 2, 0, 2) };
                        var pieceCaption = new TextBlock { Width = 288, TextWrapping = TextWrapping.Wrap, FontSize = 12,
                            Text = (v.PieceLabels[j] ?? v.Pieces[j].Name + " (뜻 미확인)") + "\n원문 " + v.Pieces[j].Name + " · 원래 " + v.Pieces[j].Value };
                        DockPanel.SetDock(pieceCaption, Dock.Left);
                        pieceLine.Children.Add(pieceCaption);
                        var pieceBox = new TextBox { Text = start[j].Value, VerticalAlignment = VerticalAlignment.Top };
                        pieceBox.TextChanged += (s, e) => Validate();
                        pieceLine.Children.Add(pieceBox);
                        block.Children.Add(pieceLine);
                        list.Add(pieceBox);
                    }
                    pieceBoxes[i] = list;
                    ValuesPanel.Children.Add(block);
                    continue;
                }
                DockPanel.SetDock(caption, Dock.Left);
                line.Children.Add(caption);
                if (v.Editable)
                {
                    var box = new TextBox { Text = Text.Value(v.Edit != null ? v.Edit.New : v.Trace.Text), VerticalAlignment = VerticalAlignment.Top };
                    box.TextChanged += (s, e) => Validate();
                    boxes[i] = box;
                    if (v.Steps != null)
                    {
                        // 판 수 단계표: 칸 아래에 새 값의 풀이를 보인다
                        var column = new StackPanel();
                        column.Children.Add(box);
                        var preview = new TextBlock { Foreground = Brushes.DimGray, FontSize = 12, TextWrapping = TextWrapping.Wrap };
                        column.Children.Add(preview);
                        stepTexts[i] = preview;
                        line.Children.Add(column);
                    }
                    else line.Children.Add(box);
                }
                else line.Children.Add(new TextBlock { Text = "고칠 수 없다: " + v.Trace.Note, Foreground = Brushes.Gray, TextWrapping = TextWrapping.Wrap });
                ValuesPanel.Children.Add(line);
```

`Tokens()`의 루프 본문을 바꾼다:

```csharp
            for (int i = 0; i < row.Values.Count; i++)
                result.Add(boxes.ContainsKey(i) ? Token(row.Values[i].Trace.Text, boxes[i].Text) : null);
```
를
```csharp
            for (int i = 0; i < row.Values.Count; i++)
            {
                if (pieceBoxes.ContainsKey(i)) result.Add(SettingStrings.Join(row.Values[i].Trace.Text, pieceBoxes[i].Select(b => b.Text.Trim()).ToList()));
                else result.Add(boxes.ContainsKey(i) ? Token(row.Values[i].Trace.Text, boxes[i].Text) : null);
            }
```

`Validate()`에서 `MessageText.Text = errors.Count > 0 ? …;` 앞에 넣는다:

```csharp
            foreach (var kv in stepTexts) kv.Value.Text = "풀이: " + (SettingStrings.StepPreview(tokens[kv.Key]) ?? "꼴이 맞지 않는다");
```

`TypeValue` 다음에 넣는다:

```csharp
        public void TypePiece(int index, string pieceName, string text)
        {
            int j = row.Values[index].Pieces.FindIndex(p => p.Name == pieceName);
            pieceBoxes[index][j].Text = text;
        }
```

- [ ] **Step 8: 시나리오에 `piece` 명령을 더한다**

`editor/CtaToybox.App/MainWindow.Scenario.cs` 머리 주석의 `//   type <n> <글자>` 줄 다음에 넣는다:

```csharp
//   piece <n> <조각 이름> <글자>  창의 n번째 값이 문자열 속 설정이면 그 조각 칸에 쓴다
```

`case "type": …` 줄 다음에 넣는다:

```csharp
                    case "piece": { var parts = arg.Split(' ', 3); dialog.TypePiece(int.Parse(parts[0]) - 1, parts[1], parts[2]); break; }
```

- [ ] **Step 9: 전체 테스트와 커밋**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1 2>&1 | tail -3` → `all test files passed`(앱 빌드 오류 0)

```bash
git add editor/CtaToybox.Core/EditorSession.cs editor/CtaToybox.Core/ValueLabels.cs editor/CtaToybox.App/Items.cs editor/CtaToybox.App/EditDialog.xaml.cs editor/CtaToybox.App/MainWindow.Scenario.cs editor/CtaToybox.Core.Tests/SettingsSessionTests.cs editor/CtaToybox.Core.Tests/ValueLabelsTests.cs
git commit -m "feat(editor): show setting pieces and step tables - one box per piece, step preview"
```

- [ ] **Step 10: 단계 2를 병합한다**

```bash
git checkout develop
git merge --no-ff feat/settings-strings -m "Merge branch 'feat/settings-strings' into develop"
```

---

## 단계 3: 컨퀘스트 AI 설정 Lua (브랜치 `feat/settings-lua`)

### Task 6: Lua 표 읽기기 `LuaTable`

**Files:**
- Create: `editor/CtaToybox.Core/Lua/LuaTable.cs`
- Create: `editor/CtaToybox.Core.Tests/LuaTableTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Produces:
  - `sealed class LuaRow { string Path; string RawPath; int Occurrence; string Value; int Index; string Kind; string Comment; string TableComment; }` (Kind: `number` | `expr` | `string` | `bool` | `nil`)
  - `sealed class LuaParseResult { List<LuaRow> Rows; List<string> Errors; }`
  - `bool LuaTable.IsLua(string virtualPath)`, `LuaParseResult LuaTable.Read(string text)`, `bool LuaTable.TryEvaluate(string expr, out double value)`, `string LuaTable.KindOf(string valueText)` (읽을 수 없으면 null)

- [ ] **Step 1: 브랜치를 만든다**

```bash
git checkout develop
git checkout -b feat/settings-lua
```

- [ ] **Step 2: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/LuaTableTests.cs`:

```csharp
// 컨퀘스트 AI 설정 Lua 읽기: 경로, 순서 목록, 따옴표 키, 주석, 수식, 모르는 문법 거부
using System.Linq;
using CtaToybox.Core;

static class LuaTableTests
{
    // runtime_config.lua의 꼴을 줄인 것
    public const string Sample =
        "-- RUNTIME CONFIG\n" +
        "ConquestConfig = ConquestConfig or {}\n" +
        "\n" +
        "-- Time from match start before the AI attempts its first purchase.\n" +
        "ConquestConfig.StartSpawnTime = {\n" +
        "\t-- Bot is attacker.\n" +
        "\tAttackMin = 1 * 1000,\n" +
        "\tAttackMax = 1 * 1000, -- upper bound\n" +
        "}\n" +
        "ConquestConfig.AttackerWaves = {\n" +
        "\tPattern = \"escalating\",\n" +
        "\tFlag = true,\n" +
        "\tDelta = -15,\n" +
        "\tescalating = {\n" +
        "\t\tFirst = { Scale = 0.85, Intent = \"probe\" },\n" +
        "\t},\n" +
        "\tBands = {\n" +
        "\t\t{ band = \"Stage1\", max = 3 },\n" +
        "\t\t{ band = \"Stage2\", max = nil },\n" +
        "\t},\n" +
        "\tRatio = {\n" +
        "\t\t[\"0.9\"] = 2.00,\n" +
        "\t},\n" +
        "}\n" +
        "\n" +
        "-- Maximum time the AI waits.\n" +
        "ConquestConfig.UnitSpawnWaitTime = 1.5 * 60000 -- 1:30 min (ms)\n" +
        "\n" +
        "ConquestConfig.OrderRotationPeriod = 2.5 * 60000\n";

    public static void Run()
    {
        var r = LuaTable.Read(Sample);
        T.Eq(0, r.Errors.Count, "the sample reads");
        T.Eq(new[]
        {
            "/conquestconfig.startspawntime/attackmin", "/conquestconfig.startspawntime/attackmax",
            "/conquestconfig.attackerwaves/pattern", "/conquestconfig.attackerwaves/flag", "/conquestconfig.attackerwaves/delta",
            "/conquestconfig.attackerwaves/escalating/first/scale", "/conquestconfig.attackerwaves/escalating/first/intent",
            "/conquestconfig.attackerwaves/bands/#1/band", "/conquestconfig.attackerwaves/bands/#1/max",
            "/conquestconfig.attackerwaves/bands/#2/band", "/conquestconfig.attackerwaves/bands/#2/max",
            "/conquestconfig.attackerwaves/ratio/\"*\"",
            "/conquestconfig.unitspawnwaittime", "/conquestconfig.orderrotationperiod",
        }, r.Rows.Select(x => x.Path).ToArray(), "paths: lowercase, positional #n, quoted keys as \"*\", X = X or {} makes no row");
        var min = r.Rows[0];
        T.Eq("/ConquestConfig.StartSpawnTime/AttackMin", min.RawPath, "the raw path keeps case");
        T.Eq("1 * 1000", min.Value, "the value text");
        T.Eq(Sample.IndexOf("1 * 1000"), min.Index, "the value position");
        T.Eq("expr", min.Kind, "an expression");
        T.Eq("Bot is attacker.", min.Comment, "the comment line above a field");
        T.Eq("Time from match start before the AI attempts its first purchase.", min.TableComment, "the comment above the table");
        T.Eq("upper bound", r.Rows[1].Comment, "a trailing comment wins");
        T.Eq("string", r.Rows[2].Kind, "a string");
        T.Eq("\"escalating\"", r.Rows[2].Value, "string values keep their quotes");
        T.Eq("bool", r.Rows[3].Kind, "a boolean");
        T.Eq("number", r.Rows[4].Kind, "a negative number is a number");
        T.Eq("nil", r.Rows[10].Kind, "nil");
        T.Eq("/ConquestConfig.AttackerWaves/Ratio/\"0.9\"", r.Rows[11].RawPath, "a quoted key keeps its name in the raw path");
        T.Eq("1:30 min (ms)", r.Rows[12].Comment, "a top-level scalar with a trailing comment");
        T.True(r.Rows[13].Comment == null, "no comment");

        double d;
        T.True(LuaTable.TryEvaluate("2.5 * 60000", out d) && d == 150000, "evaluate");
        T.True(LuaTable.TryEvaluate("-(2 + 3) * 4 / 2", out d) && d == -10, "evaluate with parentheses and unary minus");
        T.True(!LuaTable.TryEvaluate("2 * x", out d) && !LuaTable.TryEvaluate("1 / 0", out d), "names and infinity do not evaluate");

        T.Eq("expr", LuaTable.KindOf("3 * 60000"), "KindOf expression");
        T.Eq("number", LuaTable.KindOf(" 7 "), "KindOf number");
        T.Eq("string", LuaTable.KindOf("\"probe\""), "KindOf string");
        T.True(LuaTable.KindOf("2 * x") == null && LuaTable.KindOf("{}") == null && LuaTable.KindOf("print(1)") == null && LuaTable.KindOf("") == null, "KindOf refuses names, tables and calls");

        foreach (string bad in new[] { "local x = 1", "X = function() end", "X = { a = 1 + y }", "X = { [1] = 2 }", "X = { a = 1", "X = 1 ..", "X = \"open" })
        {
            var b = LuaTable.Read(bad);
            T.True(b.Errors.Count > 0 && b.Rows.Count == 0, "unknown syntax is refused without rows: " + bad);
        }
        T.True(LuaTable.IsLua("script/multiplayer/modes/conquest/runtime_config.lua") && !LuaTable.IsLua("set/a.set"), "IsLua");
    }
}
```

`Program.cs` 스위트 목록의 `("SettingStrings", SettingStringsTests.Run),` 다음에 넣는다:

```csharp
            ("LuaTable", LuaTableTests.Run),
```

- [ ] **Step 3: 테스트가 실패하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'LuaTable'` 이름이 없다.

- [ ] **Step 4: `LuaTable.cs`를 쓴다**

`editor/CtaToybox.Core/Lua/LuaTable.cs`:

```csharp
// 컨퀘스트 AI 설정(script/multiplayer/modes/conquest/runtime_config.lua)을 읽는다. Lua 해석기가 아니라 이 파일에 나오는 꼴만 읽는다:
//   문장: 이름(.이름)* = 값. 맨 위의 X = X or {}는 줄을 만들지 않는다
//   값: 수, 수식(수와 * / + - ( )), 문자열, true, false, nil, 표 { 이름 = 값, ["문자열"] = 값, 순서 값 }
//   주석: -- 줄 주석, --[[ ]] 블록 주석. 필드 줄 끝 주석(없으면 바로 위 주석 줄)은 그 줄에, 표 앞의 주석 줄은 그 표의 줄들에 붙인다
// 이 밖의 문법을 만나면 Errors에 적고 줄을 하나도 내지 않는다(파일 전체를 읽지 못함으로 본다). 추측해서 읽지 않는다.
// 줄 경로는 KeyLocator와 같은 꼴이다: /conquestconfig.startspawntime/attackmin, 따옴표 키는 "*", 순서 값은 #1, #2
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CtaToybox.Core
{
    public sealed class LuaRow
    {
        public string Path { get; set; }          // 소문자 경로. 비교에 쓴다
        public string RawPath { get; set; }       // 원문 그대로의 경로. 따옴표 키는 "0.9"처럼 원문 이름
        public int Occurrence { get; set; }       // 같은 경로 가운데 몇 번째
        public string Value { get; set; }         // 값의 원문. 예: 2 * 60000, "escalating", true
        public int Index { get; set; }            // 값이 시작하는 글자 위치
        public string Kind { get; set; }          // number | expr | string | bool | nil
        public string Comment { get; set; }       // 그 줄 끝 주석. 없으면 바로 위 주석 줄들. 없으면 null
        public string TableComment { get; set; }  // 값을 감싼 가장 가까운 표 앞의 주석. 없으면 null
    }

    public sealed class LuaParseResult
    {
        public List<LuaRow> Rows { get; } = new List<LuaRow>();
        public List<string> Errors { get; } = new List<string>();
    }

    public static class LuaTable
    {
        public static bool IsLua(string virtualPath) { return virtualPath != null && virtualPath.EndsWith(".lua", StringComparison.OrdinalIgnoreCase); }

        public static LuaParseResult Read(string text)
        {
            var result = new LuaParseResult();
            try { new Parser(text ?? "", result).Chunk(); }
            catch (FormatException ex) { result.Rows.Clear(); result.Errors.Add(ex.Message); }
            return result;
        }

        // 값 하나의 종류. 값으로 읽을 수 없으면(이름, 표, 호출 등) null
        public static string KindOf(string valueText)
        {
            if (string.IsNullOrWhiteSpace(valueText)) return null;
            var r = Read("x = " + valueText.Trim());
            return r.Errors.Count == 0 && r.Rows.Count == 1 && r.Rows[0].Path == "/x" ? r.Rows[0].Kind : null;
        }

        // 수식 계산: 수, + - * /, 괄호, 단항 -. 계산이 안 되거나 무한·NaN이면 false
        public static bool TryEvaluate(string expr, out double value)
        {
            value = 0;
            if (expr == null) return false;
            try
            {
                int i = 0;
                value = Sum(expr, ref i);
                Skip(expr, ref i);
                return i == expr.Length && !double.IsNaN(value) && !double.IsInfinity(value);
            }
            catch (FormatException) { return false; }
        }

        static void Skip(string s, ref int i) { while (i < s.Length && char.IsWhiteSpace(s[i])) i++; }

        static double Sum(string s, ref int i)
        {
            double v = Product(s, ref i);
            for (;;)
            {
                Skip(s, ref i);
                if (i < s.Length && (s[i] == '+' || s[i] == '-')) { char op = s[i++]; double r = Product(s, ref i); v = op == '+' ? v + r : v - r; }
                else return v;
            }
        }

        static double Product(string s, ref int i)
        {
            double v = Unary(s, ref i);
            for (;;)
            {
                Skip(s, ref i);
                if (i < s.Length && (s[i] == '*' || s[i] == '/')) { char op = s[i++]; double r = Unary(s, ref i); v = op == '*' ? v * r : v / r; }
                else return v;
            }
        }

        static double Unary(string s, ref int i)
        {
            Skip(s, ref i);
            if (i < s.Length && s[i] == '-') { i++; return -Unary(s, ref i); }
            if (i < s.Length && s[i] == '(')
            {
                i++;
                double v = Sum(s, ref i);
                Skip(s, ref i);
                if (i >= s.Length || s[i] != ')') throw new FormatException("')'가 없다");
                i++;
                return v;
            }
            int start = i;
            while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
            if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
            {
                i++;
                if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
                while (i < s.Length && char.IsDigit(s[i])) i++;
            }
            double d;
            if (i == start || !double.TryParse(s.Substring(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture, out d)) throw new FormatException("수가 아니다");
            return d;
        }

        sealed class Tok
        {
            public string Kind;   // name | number | string | sym | comment
            public string Text;
            public int Index;
            public int Line;
        }

        static List<Tok> Tokenize(string s)
        {
            var list = new List<Tok>();
            int i = 0, line = 1;
            while (i < s.Length)
            {
                char c = s[i];
                if (c == '\n') { line++; i++; continue; }
                if (char.IsWhiteSpace(c)) { i++; continue; }
                int start = i;
                if (c == '-' && i + 1 < s.Length && s[i + 1] == '-')
                {
                    if (i + 3 < s.Length && s[i + 2] == '[' && s[i + 3] == '[')
                    {
                        int close = s.IndexOf("]]", i + 4, StringComparison.Ordinal);
                        if (close < 0) throw new FormatException(line + "줄: 닫히지 않은 블록 주석");
                        string body = s.Substring(i + 4, close - i - 4);
                        list.Add(new Tok { Kind = "comment", Text = body.Trim(), Index = start, Line = line });
                        line += body.Count(ch => ch == '\n');
                        i = close + 2;
                        continue;
                    }
                    int end = s.IndexOf('\n', i);
                    if (end < 0) end = s.Length;
                    list.Add(new Tok { Kind = "comment", Text = s.Substring(i + 2, end - i - 2).Trim(), Index = start, Line = line });
                    i = end;
                    continue;
                }
                if (char.IsLetter(c) || c == '_')
                {
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i] == '_')) i++;
                    list.Add(new Tok { Kind = "name", Text = s.Substring(start, i - start), Index = start, Line = line });
                    continue;
                }
                if (char.IsDigit(c) || (c == '.' && i + 1 < s.Length && char.IsDigit(s[i + 1])))
                {
                    while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.')) i++;
                    if (i < s.Length && (s[i] == 'e' || s[i] == 'E'))
                    {
                        i++;
                        if (i < s.Length && (s[i] == '+' || s[i] == '-')) i++;
                        while (i < s.Length && char.IsDigit(s[i])) i++;
                    }
                    string number = s.Substring(start, i - start);
                    double d;
                    if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out d)) throw new FormatException(line + "줄: 수가 아니다 (" + number + ")");
                    list.Add(new Tok { Kind = "number", Text = number, Index = start, Line = line });
                    continue;
                }
                if (c == '"' || c == '\'')
                {
                    i++;
                    while (i < s.Length && s[i] != c)
                    {
                        if (s[i] == '\n') throw new FormatException(line + "줄: 줄이 바뀌는 문자열");
                        if (s[i] == '\\') i++;
                        i++;
                    }
                    if (i >= s.Length) throw new FormatException(line + "줄: 닫히지 않은 문자열");
                    i++;
                    list.Add(new Tok { Kind = "string", Text = s.Substring(start, i - start), Index = start, Line = line });
                    continue;
                }
                if ("{}[]=,;.*/+-()".IndexOf(c) >= 0)
                {
                    i++;
                    list.Add(new Tok { Kind = "sym", Text = c.ToString(), Index = start, Line = line });
                    continue;
                }
                throw new FormatException(line + "줄: 읽을 수 없는 글자 '" + c + "'");
            }
            return list;
        }

        sealed class Parser
        {
            readonly LuaParseResult result;
            readonly List<Tok> t = new List<Tok>();
            readonly Dictionary<int, Tok> comments = new Dictionary<int, Tok>();
            readonly HashSet<int> codeLines = new HashSet<int>();
            readonly Dictionary<string, int> seen = new Dictionary<string, int>(StringComparer.Ordinal);
            readonly string text;
            int p;

            public Parser(string text, LuaParseResult result)
            {
                this.text = text;
                this.result = result;
                foreach (var k in Tokenize(text))
                {
                    if (k.Kind == "comment") comments[k.Line] = k;
                    else { t.Add(k); codeLines.Add(k.Line); }
                }
            }

            Tok Peek(int ahead = 0) { return p + ahead < t.Count ? t[p + ahead] : null; }
            static bool IsSym(Tok k, string s) { return k != null && k.Kind == "sym" && k.Text == s; }
            static FormatException Error(Tok k, string message)
            {
                return new FormatException((k == null ? "파일 끝" : k.Line + "줄") + ": " + message + (k == null ? "" : " (" + k.Text + ")"));
            }
            void Expect(string sym) { if (!IsSym(Peek(), sym)) throw Error(Peek(), "'" + sym + "'가 와야 한다"); p++; }

            // 바로 위의 주석 줄들(코드가 없는 줄만, 빈 줄에서 멈춘다). 없으면 null
            string Preceding(int line)
            {
                var parts = new List<string>();
                for (int l = line - 1; l > 0; l--)
                {
                    Tok c;
                    if (!comments.TryGetValue(l, out c) || codeLines.Contains(l)) break;
                    parts.Insert(0, c.Text);
                }
                return parts.Count == 0 ? null : string.Join(" ", parts);
            }

            public void Chunk()
            {
                while (Peek() != null)
                {
                    var first = Peek();
                    if (first.Kind != "name") throw Error(first, "문장은 이름으로 시작해야 한다");
                    var names = new List<string> { first.Text };
                    p++;
                    while (IsSym(Peek(), "."))
                    {
                        p++;
                        if (Peek() == null || Peek().Kind != "name") throw Error(Peek(), "'.' 뒤에 이름이 와야 한다");
                        names.Add(Peek().Text);
                        p++;
                    }
                    Expect("=");
                    string target = string.Join(".", names);
                    // X = X or {}: 표를 만들 뿐 값이 없다
                    if (Peek() != null && Peek().Kind == "name" && Peek().Text == target && Peek(1) != null && Peek(1).Kind == "name" && Peek(1).Text == "or"
                        && IsSym(Peek(2), "{") && IsSym(Peek(3), "}"))
                        p += 4;
                    else Value("/" + target.ToLowerInvariant(), "/" + target, Preceding(first.Line), null);
                    if (IsSym(Peek(), ";")) p++;
                }
            }

            void Value(string path, string raw, string fieldComment, string tableComment)
            {
                if (IsSym(Peek(), "{")) { Table(path, raw, fieldComment ?? tableComment); return; }
                int start = p, depth = 0;
                bool expectOperand = true;
                while (Peek() != null)
                {
                    var k = Peek();
                    if (depth == 0 && (IsSym(k, ",") || IsSym(k, ";") || IsSym(k, "}"))) break;
                    if (k.Kind == "number" || k.Kind == "string" || k.Kind == "name")
                    {
                        if (!expectOperand) break;   // 다음 문장의 시작(맨 위의 값은 구분자 없이 줄이 바뀐다)
                        expectOperand = false; p++; continue;
                    }
                    if (IsSym(k, "(")) { if (!expectOperand) throw Error(k, "호출은 읽지 않는다"); depth++; p++; continue; }
                    if (IsSym(k, ")")) { if (expectOperand || depth == 0) throw Error(k, "짝이 맞지 않는 ')'"); depth--; p++; continue; }
                    if (IsSym(k, "-")) { if (!expectOperand) expectOperand = true; p++; continue; }
                    if (IsSym(k, "*") || IsSym(k, "/") || IsSym(k, "+")) { if (expectOperand) throw Error(k, "연산자 앞에 값이 없다"); expectOperand = true; p++; continue; }
                    throw Error(k, "값 자리에 올 수 없다");
                }
                if (p == start) throw Error(Peek(), "값이 없다");
                if (expectOperand || depth != 0) throw Error(Peek(), "식이 끝나지 않았다");

                var first = t[start];
                var last = t[p - 1];
                int count = p - start;
                string kind;
                if (count == 1 && first.Kind == "string") kind = "string";
                else if (count == 1 && first.Kind == "name")
                {
                    if (first.Text == "true" || first.Text == "false") kind = "bool";
                    else if (first.Text == "nil") kind = "nil";
                    else throw Error(first, "다른 값을 가리키는 이름은 읽지 않는다");
                }
                else
                {
                    for (int i = start; i < p; i++)
                        if (t[i].Kind == "string" || t[i].Kind == "name") throw Error(t[i], "문자열·이름이 든 식은 읽지 않는다");
                    kind = count == 1 || (count == 2 && IsSym(first, "-")) ? "number" : "expr";
                }
                int end = last.Index + last.Text.Length;
                string valueText = text.Substring(first.Index, end - first.Index);
                double ignored;
                if (kind == "expr" && !TryEvaluate(valueText, out ignored)) throw Error(first, "계산되지 않는 식");

                Tok trailing;
                string comment = comments.TryGetValue(last.Line, out trailing) && trailing.Index > last.Index ? trailing.Text : fieldComment;
                int occ;
                seen.TryGetValue(path, out occ);
                seen[path] = occ + 1;
                result.Rows.Add(new LuaRow { Path = path, RawPath = raw, Occurrence = occ, Value = valueText, Index = first.Index, Kind = kind, Comment = comment, TableComment = tableComment });
            }

            void Table(string path, string raw, string tableComment)
            {
                Expect("{");
                int position = 0;
                while (!IsSym(Peek(), "}"))
                {
                    var k = Peek();
                    if (k == null) throw Error(null, "닫히지 않은 표");
                    string fieldComment = Preceding(k.Line);
                    if (k.Kind == "name" && IsSym(Peek(1), "="))
                    {
                        p += 2;
                        Value(path + "/" + k.Text.ToLowerInvariant(), raw + "/" + k.Text, fieldComment, tableComment);
                    }
                    else if (IsSym(k, "["))
                    {
                        p++;
                        var key = Peek();
                        if (key == null || key.Kind != "string") throw Error(key, "[ ] 안에는 문자열 키만 읽는다");
                        p++;
                        Expect("]");
                        Expect("=");
                        Value(path + "/\"*\"", raw + "/" + key.Text, fieldComment, tableComment);
                    }
                    else
                    {
                        position++;
                        Value(path + "/#" + position, raw + "/#" + position, fieldComment, tableComment);
                    }
                    if (IsSym(Peek(), ",") || IsSym(Peek(), ";")) p++;
                    else if (!IsSym(Peek(), "}")) throw Error(Peek(), "',' 또는 '}'가 와야 한다");
                }
                p++;
            }
        }
    }
}
```

- [ ] **Step 5: 테스트가 통과하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: `failed=0`. 실패한 기대값이 있으면 테스트가 아니라 읽기기를 고친다(테스트의 경로·종류·주석은 스펙의 표와 같다).

- [ ] **Step 6: 바닐라 파일을 읽어 본다**

CLI `show`는 Task 8까지 Lua를 SDL로 읽으므로 쓰지 않는다. 테스트가 빌드한 코어 DLL로 바닐라 파일을 읽는다:

```bash
pwsh -NoProfile -Command "Add-Type -Path editor/CtaToybox.Core/bin/Release/net8.0/CtaToybox.Core.dll; $r = [CtaToybox.Core.LuaTable]::Read([IO.File]::ReadAllText('_work/vanilla/script/multiplayer/modes/conquest/runtime_config.lua')); 'rows=' + $r.Rows.Count + ' errors=' + $r.Errors.Count; $r.Errors"
```
Expected: `errors=0`, `rows=`가 380 이상. 오류가 있으면 그 줄의 꼴을 Task 6 테스트에 더하고 읽기기를 고친다.

- [ ] **Step 7: 커밋한다**

```bash
git add editor/CtaToybox.Core/Lua/LuaTable.cs editor/CtaToybox.Core.Tests/LuaTableTests.cs editor/CtaToybox.Core.Tests/Program.cs
git commit -m "feat(editor): a small Lua table reader for the conquest AI config"
```

### Task 7: Lua 패처와 빌드

**Files:**
- Create: `editor/CtaToybox.Core/Lua/LuaPatcher.cs`
- Modify: `editor/CtaToybox.Core/ValueRules.cs` (Lua 갈래)
- Modify: `editor/CtaToybox.Core/ModBuilder.cs` (`StageItem`의 패처 고르기)
- Create: `editor/CtaToybox.Core.Tests/LuaPatcherTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: Task 6의 `LuaTable.Read`, `LuaTable.KindOf`, `LuaRow`
- Produces:
  - `string LuaPatcher.CheckNewValue(string oldValue, string newValue, out string warning)` (맞으면 null)
  - `bool LuaPatcher.SameValue(string a, string b)`: 수·수식은 공백을 빼고, 나머지는 앞뒤 공백만 빼고 비교
  - `string LuaPatcher.Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results)`: `TokenPatcher.Apply`와 같은 약속(고친 뒤 다시 읽히지 않으면 `InvalidOperationException`)

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/LuaPatcherTests.cs`:

```csharp
// Lua 값 바꾸기: 범위 교체, 종류 검사, 원본 바뀜, 다시 읽기, 빌드
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CtaToybox.Core;

static class LuaPatcherTests
{
    const string Lua = "script/multiplayer/modes/conquest/runtime_config.lua";

    static Edit E(string path, string old, string @new, int occ = 0)
    {
        return new Edit { Unit = Lua, File = Lua, Path = path, Occurrence = occ, Arg = 0, Old = old, New = @new };
    }

    public static void Run()
    {
        string warning;
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "3 * 60000", out warning) == null, "an expression for an expression");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "180000", out warning) == null, "a number for an expression");
        T.True(LuaPatcher.CheckNewValue("\"escalating\"", "\"flat\"", out warning) == null, "a string for a string");
        T.True(LuaPatcher.CheckNewValue("true", "false", out warning) == null, "a boolean for a boolean");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "2 * x", out warning) != null, "a name in an expression is refused");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "{}", out warning) != null, "a table is refused");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "print(1)", out warning) != null, "a call is refused");
        T.True(LuaPatcher.CheckNewValue("1 * 1000", "\"5\"", out warning) != null, "a string for a number is refused");
        T.True(LuaPatcher.CheckNewValue("\"a\"", "'a'", out warning) != null, "the quote kind stays");
        T.True(LuaPatcher.CheckNewValue("nil", "3", out warning) != null, "nil is not editable");
        T.True(LuaPatcher.CheckNewValue("3", "", out warning) != null, "an empty value is refused");
        T.True(LuaPatcher.SameValue("1 * 1000", "1*1000") && !LuaPatcher.SameValue("\"a b\"", "\"ab\""), "SameValue ignores spaces only in numbers");

        string text = LuaTableTests.Sample;
        var results = new List<EditResult>();
        string patched = LuaPatcher.Apply(text, new[]
        {
            E("/conquestconfig.startspawntime/attackmin", "1 * 1000", "5 * 60000"),
            E("/conquestconfig.attackerwaves/pattern", "\"escalating\"", "\"flat\""),
            E("/conquestconfig.unitspawnwaittime", "1.5 * 60000", "90000"),
        }, results);
        T.True(results.All(r => r.Status == EditStatus.Applied), "all applied");
        T.True(patched.Contains("AttackMin = 5 * 60000,") && patched.Contains("Pattern = \"flat\",") && patched.Contains("UnitSpawnWaitTime = 90000 -- 1:30 min (ms)"), "only the value text changes");
        T.Eq(text.Length - "1 * 1000".Length + "5 * 60000".Length - "\"escalating\"".Length + "\"flat\"".Length - "1.5 * 60000".Length + "90000".Length, patched.Length, "nothing else changes");

        results.Clear();
        LuaPatcher.Apply(text, new[] { E("/conquestconfig.startspawntime/attackmin", "2 * 1000", "3") }, results);
        T.Eq(EditStatus.SourceChanged, results[0].Status, "a changed source value");
        results.Clear();
        LuaPatcher.Apply(text, new[] { E("/conquestconfig.nothing", "1", "2") }, results);
        T.Eq(EditStatus.TargetMissing, results[0].Status, "a missing slot");
        results.Clear();
        LuaPatcher.Apply(text, new[] { E("/conquestconfig.attackerwaves/bands/#2/max", "nil", "4") }, results);
        T.Eq(EditStatus.Invalid, results[0].Status, "nil stays");
        results.Clear();
        LuaPatcher.Apply("X = function() end", new[] { E("/x", "1", "2") }, results);
        T.Eq(EditStatus.Invalid, results[0].Status, "an unreadable file reports Invalid instead of writing");

        T.True(ValueRules.CheckNewValue(Lua, "1 * 1000", "2 * x", out warning) != null, "ValueRules uses the Lua rules for .lua");

        // 빌드: .lua 수정이 resource/script 아래에 쓰인다
        string game = T.TempDir(), outDir = T.TempDir();
        T.Zip(Path.Combine(game, "resource", "gamelogic.pak"), (Lua, text));
        using var stack = new SourceStack(new[] { new SourceLayer("vanilla", Path.Combine(game, "resource")) });
        stack.Scan();
        var project = new EditProject { Name = "t", Edits = new List<Edit> { E("/conquestconfig.startspawntime/attackmin", "1 * 1000", "5 * 60000"), E("/conquestconfig.attackerwaves/flag", "true", "\"x\"") } };
        var report = new ModBuilder(stack, outDir).Build(project);
        T.Eq(1, report.Count(EditStatus.Applied), "the build applies the valid Lua edit");
        T.Eq(1, report.Count(EditStatus.Invalid), "and reports the wrong-kind edit");
        string written = File.ReadAllText(Path.Combine(outDir, "resource", "script", "multiplayer", "modes", "conquest", "runtime_config.lua"));
        T.True(written.Contains("AttackMin = 5 * 60000,") && written.Contains("Flag = true,"), "the written file");
    }
}
```

`Program.cs` 스위트 목록의 `("LuaTable", LuaTableTests.Run),` 다음에 넣는다:

```csharp
            ("LuaPatcher", LuaPatcherTests.Run),
```

- [ ] **Step 2: 테스트가 실패하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'LuaPatcher'` 이름이 없다.

- [ ] **Step 3: `LuaPatcher.cs`를 쓴다**

`editor/CtaToybox.Core/Lua/LuaPatcher.cs`:

```csharp
// Lua 설정 값 바꾸기. 값의 원문 범위만 새 글자로 바꾸고, 고친 뒤 파일 전체를 다시 읽어 그 자리에 새 값이 있는지 본다.
// TokenPatcher와 같은 약속이다: 자리가 없으면 TargetMissing, 원래 값이 다르면 SourceChanged, 규칙에 어긋나면 Invalid,
// 고친 파일이 다시 읽히지 않으면 InvalidOperationException(빌드가 그 파일의 수정을 적용 불가로 바꾼다)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtaToybox.Core
{
    public static class LuaPatcher
    {
        static bool Numeric(string kind) { return kind == "number" || kind == "expr"; }

        static string KindName(string kind)
        {
            return Numeric(kind) ? "수" : kind == "string" ? "문자열" : kind == "bool" ? "참·거짓" : kind == "nil" ? "nil" : "읽을 수 없음";
        }

        // 새 값이 원래 값과 같은 종류인지 본다. 맞으면 null
        public static string CheckNewValue(string oldValue, string newValue, out string warning)
        {
            warning = null;
            if (string.IsNullOrWhiteSpace(newValue)) return "새 값이 비어 있다";
            string before = LuaTable.KindOf(oldValue), after = LuaTable.KindOf(newValue);
            if (before == "nil") return "nil(값 없음)은 이번에 고칠 수 없다";
            if (after == null) return "읽을 수 없는 값이다. 수·수식(수와 * / + - ( )), 따옴표 문자열, true, false만 쓴다: " + newValue;
            if (Numeric(before) != Numeric(after) || (!Numeric(before) && before != after))
                return "원래 값과 종류가 다르다(원래 " + KindName(before) + ", 새 값 " + KindName(after) + ")";
            if (before == "string" && oldValue.Trim()[0] != newValue.Trim()[0]) return "따옴표는 원래처럼 " + oldValue.Trim()[0] + " 를 쓴다";
            return null;
        }

        // 같은 값인가: 수·수식은 공백을 빼고, 나머지는 앞뒤 공백만 빼고 비교한다
        public static bool SameValue(string a, string b)
        {
            if (a == null || b == null) return a == b;
            if (a.Trim() == b.Trim()) return true;
            if (!Numeric(LuaTable.KindOf(a)) || !Numeric(LuaTable.KindOf(b))) return false;
            return new string(a.Where(c => !char.IsWhiteSpace(c)).ToArray()) == new string(b.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static string Apply(string text, IReadOnlyList<Edit> edits, List<EditResult> results)
        {
            var active = edits.Where(e => e.Enabled).ToList();
            if (active.Count == 0) return text;
            var parsed = LuaTable.Read(text);
            if (parsed.Errors.Count > 0)
            {
                foreach (var e in active) results.Add(Result(e, EditStatus.Invalid, "원본 Lua 파일을 읽지 못한다: " + parsed.Errors[0]));
                return text;
            }
            var patches = new List<KeyValuePair<Edit, LuaRow>>();
            var taken = new HashSet<int>();
            foreach (var e in active)
            {
                var row = e.Arg != 0 ? null : parsed.Rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
                if (row == null) { results.Add(Result(e, EditStatus.TargetMissing, "원본에 이 자리가 없다: " + e.Path + " [" + e.Occurrence + "] 값 " + e.Arg)); continue; }
                if (!SameValue(row.Value, e.Old)) { results.Add(Result(e, EditStatus.SourceChanged, "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + row.Value)); continue; }
                string warning;
                string error = CheckNewValue(row.Value, e.New, out warning);
                if (error != null) { results.Add(Result(e, EditStatus.Invalid, error)); continue; }
                if (!taken.Add(row.Index)) { results.Add(Result(e, EditStatus.Invalid, "같은 값을 고치는 수정이 앞에 이미 있다")); continue; }
                patches.Add(new KeyValuePair<Edit, LuaRow>(e, row));
                results.Add(Result(e, EditStatus.Applied, warning));
            }
            if (patches.Count == 0) return text;

            var sb = new StringBuilder(text);
            foreach (var x in patches.OrderByDescending(x => x.Value.Index))
            {
                sb.Remove(x.Value.Index, x.Value.Value.Length);
                sb.Insert(x.Value.Index, x.Key.New.Trim());
            }
            string patched = sb.ToString();

            var check = LuaTable.Read(patched);
            if (check.Errors.Count > 0) throw new InvalidOperationException("고친 Lua 파일이 다시 읽히지 않는다: " + check.Errors[0]);
            foreach (var x in patches)
            {
                var row = check.Rows.FirstOrDefault(r => r.Occurrence == x.Key.Occurrence && string.Equals(r.Path, x.Key.Path, StringComparison.OrdinalIgnoreCase));
                if (row == null || row.Value != x.Key.New.Trim()) throw new InvalidOperationException("고친 뒤 그 자리에 새 값이 없다: " + x.Key.Path);
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

- [ ] **Step 4: `ValueRules`와 빌드가 Lua를 고르게 한다**

`editor/CtaToybox.Core/ValueRules.cs`의 `CheckNewValue` 첫 줄로 넣는다:

```csharp
            if (LuaTable.IsLua(file)) return LuaPatcher.CheckNewValue(oldText, newText, out warning);
```

`editor/CtaToybox.Core/ModBuilder.cs`의 `StageItem`에서

```csharp
            try { patched = TokenPatcher.Apply(text, all, applied, item.Raw); }
```
를
```csharp
            try { patched = LuaTable.IsLua(item.Source.VirtualPath) ? LuaPatcher.Apply(text, all, applied) : TokenPatcher.Apply(text, all, applied, item.Raw); }
```
로 바꾼다. 파일 머리 주석 끝에 한 줄을 더한다:

```csharp
// .lua 파일(컨퀘스트 AI 설정)은 LuaPatcher로, 나머지는 TokenPatcher로 고친다.
```

- [ ] **Step 5: 테스트가 통과하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: `failed=0`

- [ ] **Step 6: 커밋한다**

```bash
git add editor/CtaToybox.Core/Lua/LuaPatcher.cs editor/CtaToybox.Core/ValueRules.cs editor/CtaToybox.Core/ModBuilder.cs editor/CtaToybox.Core.Tests/LuaPatcherTests.cs editor/CtaToybox.Core.Tests/Program.cs
git commit -m "feat(editor): Lua value patcher - same-kind values, source-changed check, re-read after patching"
```

### Task 8: Lua 설정을 색인·추적·세션·명령줄·화면에 잇기

**Files:**
- Create: `editor/CtaToybox.Core/SourceRows.cs`
- Modify: `editor/CtaToybox.Core/SettingFiles.cs` (`Top`에 Lua 파일)
- Modify: `editor/CtaToybox.Core/UnitIndex.cs` (Lua 갈래, `FormatVersion` "5")
- Modify: `editor/CtaToybox.Core/UnitTracer.cs` (`TraceRow`의 주석, `TraceLua`)
- Modify: `editor/CtaToybox.Core/EditorSession.cs` (`RowView`·`ValueView` 속성, `OpenUnit`, `Describe`, `AddMacroCalls`)
- Modify: `editor/CtaToybox.Core/EditorSession.Edits.cs` (`Check`, `Rebase`, `CheckEdits`)
- Modify: `editor/CtaToybox.Cli/Program.cs` (`Show`, `Add`)
- Modify: `editor/CtaToybox.App/Items.cs` (`RowNote`, `ValuesLine`)
- Modify: `editor/CtaToybox.App/EditDialog.xaml.cs` (원래 값의 계산값)
- Create: `editor/CtaToybox.Core.Tests/LuaSessionTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: Task 6·7의 `LuaTable`, `LuaPatcher`, Task 2의 `SettingsSessionTests.Fixture`
- Produces:
  - `List<KeyRow> SourceRows.Of(string virtualPath, string text, out string error)`: SDL은 `KeyLocator.Rows`, Lua는 `LuaTable` 줄을 `KeyRow`로(값 하나, `Form` = `block`). 읽지 못하면 null
  - `TraceRow.Comment`, `TraceRow.TableComment`, `RowView.SourceComment`, `RowView.TableComment`, `ValueView.Computed`(수식의 계산값 글자, 아니면 null)

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/LuaSessionTests.cs`:

```csharp
// Lua 설정을 세션과 명령줄에서: 색인, 추적, 주석, 계산값, 수정 점검, 모르는 문법
using System.IO;
using System.Linq;
using CtaToybox.Cli;
using CtaToybox.Core;

static class LuaSessionTests
{
    const string Lua = "script/multiplayer/modes/conquest/runtime_config.lua";

    static int Cli(out string output, params string[] args)
    {
        var w = new StringWriter();
        int code = CliApp.Run(args, w);
        output = w.ToString();
        return code;
    }

    public static void Run()
    {
        T.True(SettingFiles.IsTop(Lua), "the conquest AI config is a top setting file");

        string repo, outDir, game;
        string settings = SettingsSessionTests.Fixture(out repo, out outDir, out game);
        T.Zip(Path.Combine(game, "resource", "script.pak"), (Lua, LuaTableTests.Sample));
        using (var session = EditorSession.Open(settings, repo, null))
        {
            var entry = session.Index.Units.Single(u => u.Path == Lua);
            T.Eq("settings", entry.EditorKind, "the Lua file is in the index");
            T.True(entry.Numbers >= 5 && entry.Unresolved == 0, "with numbers and no errors");

            var view = session.OpenUnit(Lua);
            T.Eq(14, view.Rows.Count, "one row per Lua value");
            var min = view.Rows.First(r => r.Path == "/conquestconfig.startspawntime/attackmin");
            T.Eq("Bot is attacker.", min.SourceComment, "the developer comment");
            T.Eq("Time from match start before the AI attempts its first purchase.", min.TableComment, "the table comment");
            T.Eq("1000", min.Values[0].Computed, "an expression shows its value");
            T.True(view.Rows.First(r => r.Path == "/conquestconfig.attackerwaves/delta").Values[0].Computed == null, "a plain number has no computed value");
            var nil = view.Rows.First(r => r.Path == "/conquestconfig.attackerwaves/bands/#2/max").Values[0];
            T.True(!nil.Editable, "nil is not editable");
            T.Eq(SettingFiles.UnitScopeNote, min.Values[0].UnitScopeBlocker, "shared only");

            session.OpenProject(Path.Combine(T.TempDir(), "lua.json"));
            session.ApplyRow(view, min, new[] { "5 * 60000" }, "shared");
            T.Eq(EditStatus.Applied, session.CheckEdits().Single().Status, "the check finds the Lua slot");
            var d = session.Describe(session.Project.Edits.Single());
            T.True(d != null && d.RowText == "1 * 1000", "Describe reads the Lua row");
            T.Throws<System.ArgumentException>(() => session.ApplyRow(view, min, new[] { "2 * x" }, "shared"), "a bad Lua value is refused by the session");
            var report = session.Build();
            T.Eq(1, report.Count(EditStatus.Applied), "the build writes the Lua edit");
        }

        string o;
        T.Eq(0, Cli(out o, "show", Lua, "--settings", settings), "show reads Lua");
        T.True(o.Contains("/conquestconfig.startspawntime/attackmin [0] 0=1 * 1000"), "show prints Lua rows");
        string project = Path.Combine(T.TempDir(), "p.json");
        T.Eq(0, Cli(out o, "add", "--settings", settings, "--project", project, "--file", Lua, "--path", "/conquestconfig.attackerwaves/pattern", "--new", "\"flat\""), "add a Lua value");
        T.Eq(1, Cli(out o, "add", "--settings", settings, "--project", project, "--file", Lua, "--path", "/conquestconfig.attackerwaves/pattern", "--new", "flat"), "add refuses a wrong-kind Lua value");

        // 모드가 읽기기가 모르는 문법으로 바꾼 Lua: 줄 없이 이유만, 이전 수정은 적용 불가
        string repo2, out2, game2;
        string settings2 = SettingsSessionTests.Fixture(out repo2, out out2, out game2);
        T.Zip(Path.Combine(game2, "resource", "script.pak"), (Lua, LuaTableTests.Sample));
        var s2 = EditorSettings.Load(settings2);
        T.Put(Path.Combine(s2.WorkshopDir, "222", "resource"), Lua, "ConquestConfig.X = function() end\n");
        using (var session = EditorSession.Open(settings2, repo2, null))
        {
            var view = session.OpenUnit(Lua);
            T.True(view.Rows.Count == 0 && view.Unresolved.Count == 1 && view.Unresolved[0].StartsWith("Lua: "), "an unreadable Lua file shows why, with no rows");
            session.OpenProject(Path.Combine(T.TempDir(), "old.json"));
            session.Project.Edits.Add(new Edit { Unit = Lua, File = Lua, Path = "/conquestconfig.startspawntime/attackmin", Occurrence = 0, Arg = 0, Old = "1 * 1000", New = "3" });
            T.Eq(EditStatus.TargetMissing, session.CheckEdits().Single().Status, "the old edit no longer finds its slot");
            var report = session.Build();
            T.Eq(1, report.Count(EditStatus.Invalid), "the build reports it instead of writing a broken file");
            T.True(!File.Exists(Path.Combine(out2, "resource", "script", "multiplayer", "modes", "conquest", "runtime_config.lua")), "nothing is written");
        }
    }
}
```

`Program.cs` 스위트 목록의 `("LuaPatcher", LuaPatcherTests.Run),` 다음에 넣는다:

```csharp
            ("LuaSession", LuaSessionTests.Run),
```

- [ ] **Step 2: 테스트가 실패하는지 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'RowView'에 'SourceComment'…`, `'ValueView'에 'Computed'…`.

- [ ] **Step 3: `SourceRows.cs`를 쓰고 Lua 파일을 게임 설정에 넣는다**

`editor/CtaToybox.Core/SourceRows.cs`:

```csharp
// 파일 하나의 값 줄. SDL 파일은 KeyLocator.Rows, Lua 파일은 LuaTable의 줄(값 하나짜리 KeyRow)이다.
// 수정의 자리(경로, 몇 번째, 몇 번째 값)를 원본에서 다시 찾는 곳(점검, 설명, 명령줄)이 모두 이것을 쓴다
using System.Collections.Generic;
using System.Linq;

namespace CtaToybox.Core
{
    public static class SourceRows
    {
        // 읽지 못하면 null이고 error에 이유
        public static List<KeyRow> Of(string virtualPath, string text, out string error)
        {
            error = null;
            if (text == null) { error = "원본에 이 파일이 없다"; return null; }
            if (LuaTable.IsLua(virtualPath))
            {
                var lua = LuaTable.Read(text);
                if (lua.Errors.Count > 0) { error = lua.Errors[0]; return null; }
                return lua.Rows.Select(r => new KeyRow
                {
                    Path = r.Path, RawPath = r.RawPath, Form = "block", Occurrence = r.Occurrence,
                    Leaves = new List<SdlNode> { new SdlNode(r.Kind == "string" ? "str" : "atom", r.Value, r.Index, false) },
                    LeafNames = new List<string> { null },
                }).ToList();
            }
            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0) { error = parsed.Errors[0]; return null; }
            return KeyLocator.Rows(parsed.Root);
        }
    }
}
```

`editor/CtaToybox.Core/SettingFiles.cs`의 `Top` 정규식을 바꾼다:

```csharp
            @"^(set/dynamic_campaign/(resources|duration)_[^/]+\.set|set/multiplayer/games/[^/]+\.set|set/difficulty/[^/]+\.dl|script/multiplayer/modes/conquest/runtime_config\.lua)$",
```

- [ ] **Step 4: 색인이 Lua를 읽게 한다**

`editor/CtaToybox.Core/UnitIndex.cs`의 `Build` 루프 본문 맨 앞(`var r = resolver.Resolve(paths[i]);` 앞)에 넣는다:

```csharp
                if (LuaTable.IsLua(paths[i]))
                {
                    // Lua 설정은 include가 없다. 값 가운데 수·수식의 개수를 센다
                    var lua = LuaTable.Read(stack.ReadText(paths[i]));
                    index.Units.Add(new UnitEntry
                    {
                        Path = paths[i], Kind = UnitKind.OfEditor(paths[i]), EditorKind = SettingFiles.Kind, Layer = stack.Find(paths[i]).Layer,
                        Unresolved = lua.Errors.Count, Numbers = lua.Rows.Count(x => x.Kind == "number" || x.Kind == "expr"),
                    });
                    if (progress != null && (i + 1) % 1000 == 0) progress(i + 1, paths.Count);
                    continue;
                }
```

`const string FormatVersion = "4";`를 `"5"`로 바꾼다.

- [ ] **Step 5: 추적기에 Lua 갈래를 넣는다**

`editor/CtaToybox.Core/UnitTracer.cs`의 `TraceRow`에 `public List<string> LeafNames { get; set; }` 다음 두 줄을 넣는다:

```csharp
        public string Comment { get; set; }        // Lua 설정의 개발사 주석(그 줄). 없으면 null
        public string TableComment { get; set; }   // Lua 설정의 표 앞 주석. 없으면 null
```

`Trace`의 `if (stack.Find(unit) == null) return null;` 다음 줄에 넣는다:

```csharp
            if (LuaTable.IsLua(unit)) return TraceLua(unit);
```

`Trace` 메서드 다음에 넣는다:

```csharp
        // Lua 설정 파일: include·매크로가 없으므로 파일의 줄이 곧 펼친 줄이다. 읽지 못하면 줄 없이 이유만 낸다
        UnitTraceResult TraceLua(string unit)
        {
            var lua = LuaTable.Read(stack.ReadText(unit));
            var result = new UnitTraceResult { Unit = unit, Unresolved = lua.Errors.Select(e => "Lua: " + e).ToList(), IncludeEdges = new List<SdlIncludeEdge>(), Nodes = new List<object>() };
            foreach (var r in lua.Rows)
            {
                var row = new TraceRow { Path = r.Path, RawPath = r.RawPath, Occurrence = r.Occurrence, Form = "block", LeafNames = new List<string> { null }, Comment = r.Comment, TableComment = r.TableComment };
                var v = new TraceValue { Text = r.Value, File = unit, KeyPath = r.Path, Occurrence = r.Occurrence, Arg = 0 };
                if (r.Kind == "nil") { v.Arg = -1; v.Note = "nil(값 없음)은 이번에 고칠 수 없다"; }
                row.Values.Add(v);
                result.Rows.Add(row);
            }
            return result;
        }
```

- [ ] **Step 6: 세션을 고친다**

`editor/CtaToybox.Core/EditorSession.cs`:

`RowView`의 `public int Repeats { get; set; } = 1;` 다음에 넣는다:

```csharp
        public string SourceComment { get; set; }     // Lua 설정의 개발사 주석(영어 원문). 없으면 null
        public string TableComment { get; set; }      // Lua 설정의 표 앞 주석. 없으면 null
```

`ValueView`의 `public string Steps { get; set; }` 다음에 넣는다:

```csharp
        public string Computed { get; set; }             // Lua 수식(예: 2 * 60000)의 계산값 글자. 아니면 null
```

파일 머리 `using` 목록에 `using System.Globalization;`을 넣는다.

`OpenUnit`의 `var r = new RowView { Path = row.Path, … };` 다음 줄에 넣는다:

```csharp
                r.SourceComment = row.Comment;
                r.TableComment = row.TableComment;
```

같은 루프의 `var vv = new ValueView { … };` 다음 줄에 넣는다:

```csharp
                    double computed;
                    if (LuaTable.IsLua(v.File) && !SdlCore.IsNumber(v.Text) && LuaTable.TryEvaluate(v.Text, out computed))
                        vv.Computed = computed.ToString("0.###", CultureInfo.InvariantCulture);
```

`AddMacroCalls`의 `foreach (string file in files)` 루프 본문 첫 줄(`string text = Stack.ReadText(file);` 앞)에 넣는다:

```csharp
                if (LuaTable.IsLua(file)) continue;   // Lua 설정에는 매크로 호출이 없다
```

`Describe`의 아래 세 줄을

```csharp
            var parsed = SdlCore.Parse(text);
            if (parsed.Errors.Count > 0) return null;
            var row = KeyLocator.Rows(parsed.Root).FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
```
아래로 바꾼다:

```csharp
            string error;
            var rows = SourceRows.Of(file, text, out error);
            if (rows == null) return null;
            var row = rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
```

`editor/CtaToybox.Core/EditorSession.Edits.cs`:

`Rebase`의 `var check = Check(edit, new Dictionary<string, SdlNode>(StringComparer.Ordinal));`를 `var check = Check(edit, new Dictionary<string, List<KeyRow>>(StringComparer.Ordinal));`로, `CheckEdits`의 `var parsed = new Dictionary<string, SdlNode>(StringComparer.Ordinal);`를 `var parsed = new Dictionary<string, List<KeyRow>>(StringComparer.Ordinal);`로 바꾼다.

`Check` 메서드의 머리부터 `var row = …` 줄까지를 바꾼다:

```csharp
        EditCheck Check(Edit e, Dictionary<string, List<KeyRow>> parsed)
        {
            string file = SourceStack.ToVirtual(e.File ?? "");
            List<KeyRow> rows;
            if (!parsed.TryGetValue(file, out rows))
            {
                string error;
                rows = SourceRows.Of(file, Stack.ReadText(file), out error);
                parsed[file] = rows;
            }
            if (rows == null) return new EditCheck { Edit = e, Status = EditStatus.TargetMissing, Message = "원본에 이 파일이 없거나 읽히지 않는다: " + file };
            var row = rows.FirstOrDefault(r => r.Occurrence == e.Occurrence && string.Equals(r.Path, e.Path, StringComparison.OrdinalIgnoreCase));
```

같은 메서드의 `if (node.Text != e.Old) return …`의 조건을 바꾼다:

```csharp
            bool same = LuaTable.IsLua(file) ? LuaPatcher.SameValue(node.Text, e.Old) : node.Text == e.Old;
            if (!same) return new EditCheck { Edit = e, Status = EditStatus.SourceChanged, Current = node.Text, RowText = rowText, Message = "원본의 값이 바뀌었다. 수정 당시 " + e.Old + ", 지금 " + node.Text };
```

- [ ] **Step 7: 명령줄을 고친다**

`editor/CtaToybox.Cli/Program.cs`의 `Show`에서

```csharp
                var parsed = SdlCore.Parse(TextCodec.Decode(stack.Read(file), out TextKind kind));
                if (parsed.Errors.Count > 0) { output.WriteLine("파싱되지 않는다: " + parsed.Errors[0]); return 1; }
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                foreach (var row in KeyLocator.Rows(parsed.Root))
```
를
```csharp
                string error;
                var rows = SourceRows.Of(file.VirtualPath, TextCodec.Decode(stack.Read(file), out TextKind kind), out error);
                if (rows == null) { output.WriteLine("읽히지 않는다: " + error); return 1; }
                string filter = opt.ContainsKey("filter") ? opt["filter"] : null;
                foreach (var row in rows)
```
로 바꾼다.

`Add`에서

```csharp
                var parsed = SdlCore.Parse(TextCodec.Decode(stack.Read(file), out TextKind kind));
                if (parsed.Errors.Count > 0) { output.WriteLine("파싱되지 않는 파일은 고칠 수 없다: " + parsed.Errors[0]); return 1; }
                var node = KeyLocator.Find(parsed.Root, keyPath, occ, arg);
```
를
```csharp
                string readError;
                var rows = SourceRows.Of(file.VirtualPath, TextCodec.Decode(stack.Read(file), out TextKind kind), out readError);
                if (rows == null) { output.WriteLine("읽히지 않는 파일은 고칠 수 없다: " + readError); return 1; }
                var found = rows.FirstOrDefault(r => r.Occurrence == occ && string.Equals(r.Path, keyPath, StringComparison.OrdinalIgnoreCase));
                var node = found != null && arg < found.Leaves.Count ? found.Leaves[arg] : null;
```
로 바꾼다.

- [ ] **Step 8: 화면에 주석과 계산값을 보인다**

`editor/CtaToybox.App/Items.cs`의 `RowNote`를 바꾼다:

```csharp
        public static string RowNote(RowView row)
        {
            string comment = (row.SourceComment == null ? "" : "\n원문 주석: " + row.SourceComment) + (row.TableComment == null ? "" : "\n표 설명: " + row.TableComment);
            if (row.Note == null && row.IsMacroCall) return "[매크로 호출] 이 인자를 쓰는 값들이 함께 바뀐다. 인자로 계산한 값은 전체 탭의 '고칠 수 없는 값'에 보인다";
            if (row.Note == null && comment.Length > 0) return "(한글 설명 없음)" + comment;
            return Note(row.Note) + comment;
        }
```

`ValuesLine`의 조각 갈래(`// 문자열 속 설정: …`) 앞에 넣는다:

```csharp
            if (row.Values.Count == 1 && row.Values[0].Computed != null) return Value(row.Values[0].Trace.Text) + "  (= " + row.Values[0].Computed + ")";
```

`editor/CtaToybox.App/EditDialog.xaml.cs`의 값 설명 둘째 줄에서 `" · 원래 " + Text.Value(v.Trace.Text) +`를 `" · 원래 " + Text.Value(v.Trace.Text) + (v.Computed == null ? "" : " (= " + v.Computed + ")") +`로 바꾼다.

- [ ] **Step 9: 전체 테스트와 커밋**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1 2>&1 | tail -3` → `all test files passed`

```bash
git add editor/CtaToybox.Core/SourceRows.cs editor/CtaToybox.Core/SettingFiles.cs editor/CtaToybox.Core/UnitIndex.cs editor/CtaToybox.Core/UnitTracer.cs editor/CtaToybox.Core/EditorSession.cs editor/CtaToybox.Core/EditorSession.Edits.cs editor/CtaToybox.Cli/Program.cs editor/CtaToybox.App/Items.cs editor/CtaToybox.App/EditDialog.xaml.cs editor/CtaToybox.Core.Tests/LuaSessionTests.cs editor/CtaToybox.Core.Tests/Program.cs
git commit -m "feat(editor): the conquest AI Lua config in the index, tracer, session, CLI and app"
```

### Task 9: 컨퀘스트 AI 주석과 요약 키

**Files:**
- Modify: `data/annotations/settings.psv` (컨퀘스트 AI 묶음)
- Modify: `editor/summary/settings.psv` (Lua 키)

**Interfaces:**
- Consumes: Task 6의 경로 꼴(`/conquestconfig.<표>/<키>`)
- Produces: Task 10의 요약 키 검사 대상

근거는 `_work/vanilla/script/multiplayer/modes/conquest/runtime_config.lua`의 `--` 주석이다(줄 번호는 2026-10-15 바닐라). 단위 ms는 `UnitSpawnWaitTime`의 주석 "(ms)"와 같은 `* 1000`·`* 60000` 꼴이라 "추정"으로 적는다.

- [ ] **Step 1: `data/annotations/settings.psv` 끝에 넣는다 (CRLF)**

```text
# ---------- 컨퀘스트 AI (script/multiplayer/modes/conquest/runtime_config.lua의 -- 주석)
/conquestconfig.startspawntime/defensemin|block|확인|봇이 수비일 때 전투 시작부터 첫 구매까지의 최소 시간. ms로 보인다(1000 = 1초, 추정) (확인: 69~74줄 주석 "Time from match start before the AI attempts its first purchase", "Bot is defender")|C
/conquestconfig.startspawntime/defensemax|block|확인|봇이 수비일 때 첫 구매까지의 최대 시간. ms로 보인다(추정) (확인: 69~74줄 주석)|C
/conquestconfig.startspawntime/attackmin|block|확인|봇이 공격일 때 전투 시작부터 첫 구매까지의 최소 시간. ms로 보인다(추정) (확인: 69~77줄 주석 "Bot is attacker")|C
/conquestconfig.startspawntime/attackmax|block|확인|봇이 공격일 때 첫 구매까지의 최대 시간. ms로 보인다(추정) (확인: 69~77줄 주석)|C
/conquestconfig.spawncooldowntime/dcgwaveoffmin|block|확인|공격 파도가 끝난 뒤 다음 파도까지의 최소 시간. ms로 보인다(60000 = 1분, 추정) (확인: 80~83줄 주석 "Time between completed attacker waves")|C
/conquestconfig.spawncooldowntime/dcgwaveoffmax|block|확인|공격 파도 사이의 최대 시간. ms로 보인다(추정) (확인: 80~83줄 주석)|C
/conquestconfig.spawncooldowntime/dcgmin|block|확인|파도·첫 배치 안에서 구매 사이의 최소 시간. ms로 보인다(추정) (확인: 85~86줄 주석 "Time between purchases inside an active wave/opening")|C
/conquestconfig.spawncooldowntime/dcgmax|block|확인|파도·첫 배치 안에서 구매 사이의 최대 시간. ms로 보인다(추정) (확인: 85~87줄 주석)|C
/conquestconfig.attackerwaves/maxwavesmin|block|확인|공격 파도 수의 최소(기준값. 이후 압력 보정이 바꿀 수 있다) (확인: 90~91줄 주석 "Baseline attacker wave count and size")|C
/conquestconfig.attackerwaves/maxwavesmax|block|확인|공격 파도 수의 최대(기준값) (확인: 90~91줄 주석)|C
/conquestconfig.attackerwaves/minunits|block|확인|파도 하나의 유닛 수 최소(기준값) (확인: 90~91줄 주석)|C
/conquestconfig.attackerwaves/maxunits|block|확인|파도 하나의 유닛 수 최대(기준값) (확인: 90~91줄 주석)|C
/conquestconfig.attackerwaves/pattern|block|확인|파도 모양. AttackerWavePatterns의 flat, escalating, frontloaded, last_push 가운데 하나 (확인: 103줄 주석 "Wave-shape multipliers used by AttackerWaves.Pattern")|C
/conquestconfig.attackerwaves/minwaveunits|block|추정|보정 뒤 파도 하나의 유닛 수 하한 (추정: 이름과 waves.lua의 math.max(scaledTarget, plan.MinWaveUnits))|S
/conquestconfig.attackerdynamicwaveoff/firstcheckseconds|block|확인|파도가 끝난 뒤 다음 파도를 낼지 처음 살피기까지의 시간(초) (확인: 133줄 주석)|C
/conquestconfig.attackerdynamicwaveoff/repeatpollintervalseconds|block|확인|다음 파도를 낼지 다시 살피는 간격(초) (확인: 134줄 주석)|C
/conquestconfig.attackerdynamicwaveoff/forcereleaseseconds|block|확인|전장 상황과 상관없이 다음 파도를 내기까지의 최대 대기(초) (확인: 135줄 주석)|C
/conquestconfig.attackerdynamicwaveoff/randomvarianceseconds|block|확인|다음 파도 시점에 더하는 무작위 ± 시간(초) (확인: 137줄 주석)|C
/conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/degraded|block|확인|앞 공격 파도가 크게 줄었을 때 다음 파도까지 목표 시간(초). 작을수록 빨리 온다 (확인: 145~147줄 주석)|C
/conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/reduced|block|확인|앞 파도가 일부 줄었을 때 다음 파도까지 목표 시간(초) (확인: 148줄 주석)|C
/conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/retained|block|확인|앞 파도가 거의 남아 있을 때 다음 파도까지 목표 시간(초) (확인: 149줄 주석)|C
/conquestconfig.defenderopening/minunitsperflag|block|확인|AI 수비 첫 배치: 깃발 하나당 구매 유닛 수 최소. 깃발 수를 곱한다 (확인: 167~168줄 주석)|C
/conquestconfig.defenderopening/maxunitsperflag|block|확인|AI 수비 첫 배치: 깃발 하나당 구매 유닛 수 최대 (확인: 167~168줄 주석)|C
/conquestconfig.defenderopening/minunitsperflagfloor|block|확인|깃발 하나당 유닛 수의 하한. 쉬운 난이도·낮은 위험도에서 수비가 사라지지 않게 한다 (확인: 172줄 주석)|C
/conquestconfig.defenderopening/unitsholdmovement|block|확인|첫 배치 유닛이 스폰·임무 위치를 지킬지(true면 Lua 이동 교대에 들어가지 않는다) (확인: 175줄 주석)|C
/conquestconfig.defenderopening/maxduration|block|확인|첫 배치 구매의 제한 시간. 지나면 남은 구매는 버린다. ms로 보인다(추정) (확인: 178줄 주석)|C
/conquestconfig.defendercounterattack/minunits|block|추정|AI 반격 한 번의 유닛 수 최소 (추정: 이름)|S
/conquestconfig.defendercounterattack/maxunits|block|추정|AI 반격 한 번의 유닛 수 최대 (추정: 이름)|S
/conquestconfig.defendercounterattack/confirmdelay|block|확인|깃발 주인이 바뀐 채로 이만큼 지나야 수비가 반응한다. ms로 보인다(추정) (확인: 186줄 주석)|C
/conquestconfig.defendercounterattack/responsechance|block|확인|깃발을 잃었을 때 반격할 확률. 1.0 = 항상 (확인: 197줄 주석)|C
/conquestconfig.unitspawnwaittime|block|확인|AI가 자원이나 유닛 타이머를 기다리다 다시 고르기까지의 최대 시간(ms. 90000 = 1분 30초) (확인: 629~630줄 주석 "1:30 min (ms)")|C
/conquestconfig.orderrotationperiod|block|확인|분대가 새 명령을 받기까지의 간격(ms. 150000 = 2분 30초) (확인: 632~633줄 주석 "2:30 min (ms)")|C
```

- [ ] **Step 2: 줄 번호를 확인한다**

Run: `grep -n "Time from match start\|Bot is defender\|Time between completed\|Time between purchases\|Baseline attacker wave\|Wave-shape multipliers\|FirstCheckSeconds\|RepeatPollIntervalSeconds\|ForceReleaseSeconds\|RandomVarianceSeconds\|degraded =\|reduced =\|retained =\|Defender opening purchase\|Floor on the per-flag\|hold their spawn\|Time limit on the garrison\|Flag ownership must remain\|Chance to respond\|Maximum time the AI waits\|refreshed order" _work/vanilla/script/multiplayer/modes/conquest/runtime_config.lua`
Expected: 위 주석에 적은 줄 번호와 같다. 다르면 `settings.psv`의 줄 번호를 실제 번호로 고친다.

- [ ] **Step 3: `editor/summary/settings.psv` 끝에 넣는다 (CRLF)**

```text
/conquestconfig.startspawntime/defensemin|AI 첫 구매 시점 (ms)
/conquestconfig.startspawntime/defensemax|AI 첫 구매 시점 (ms)
/conquestconfig.startspawntime/attackmin|AI 첫 구매 시점 (ms)
/conquestconfig.startspawntime/attackmax|AI 첫 구매 시점 (ms)
/conquestconfig.spawncooldowntime/dcgwaveoffmin|AI 구매 간격 (ms)
/conquestconfig.spawncooldowntime/dcgwaveoffmax|AI 구매 간격 (ms)
/conquestconfig.spawncooldowntime/dcgmin|AI 구매 간격 (ms)
/conquestconfig.spawncooldowntime/dcgmax|AI 구매 간격 (ms)
/conquestconfig.attackerwaves/maxwavesmin|AI 공격 파도 수·크기·모양
/conquestconfig.attackerwaves/maxwavesmax|AI 공격 파도 수·크기·모양
/conquestconfig.attackerwaves/minunits|AI 공격 파도 수·크기·모양
/conquestconfig.attackerwaves/maxunits|AI 공격 파도 수·크기·모양
/conquestconfig.attackerwaves/pattern|AI 공격 파도 수·크기·모양
/conquestconfig.attackerdynamicwaveoff/firstcheckseconds|AI 파도 사이 대기 (초)
/conquestconfig.attackerdynamicwaveoff/forcereleaseseconds|AI 파도 사이 대기 (초)
/conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/degraded|AI 파도 사이 대기 (초)
/conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/reduced|AI 파도 사이 대기 (초)
/conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/retained|AI 파도 사이 대기 (초)
/conquestconfig.defenderopening/minunitsperflag|AI 수비 첫 배치
/conquestconfig.defenderopening/maxunitsperflag|AI 수비 첫 배치
/conquestconfig.defenderopening/minunitsperflagfloor|AI 수비 첫 배치
/conquestconfig.defenderopening/maxduration|AI 수비 첫 배치
/conquestconfig.defendercounterattack/minunits|AI 반격
/conquestconfig.defendercounterattack/maxunits|AI 반격
/conquestconfig.defendercounterattack/responsechance|AI 반격
/conquestconfig.unitspawnwaittime|AI 대기·명령 주기 (ms)
/conquestconfig.orderrotationperiod|AI 대기·명령 주기 (ms)
```

같은 파일 머리 주석의 `Lua 키는 단계 3(Task 9)에서 더한다.`를 지운다.

- [ ] **Step 4: 줄 끝을 확인하고 커밋한다**

Run: `file data/annotations/settings.psv editor/summary/settings.psv` → 둘 다 CRLF.

```bash
git add data/annotations/settings.psv editor/summary/settings.psv
git commit -m "docs(editor): notes and summary keys for the conquest AI Lua config"
```

- [ ] **Step 5: 단계 3을 병합한다**

```bash
git checkout develop
git merge --no-ff feat/settings-lua -m "Merge branch 'feat/settings-lua' into develop"
```

---

## 단계 4: 검증과 게임 안 확인 (브랜치 `chore/settings-check`)

### Task 10: 실제 파일 검증(`-- real`)에 게임 설정을 더한다

**Files:**
- Modify: `editor/CtaToybox.Core/SummaryKeys.cs` (`Keys`, `KeyOf`)
- Modify: `editor/CtaToybox.Core.Tests/RealFilesCheck.cs`
- Modify: `editor/CtaToybox.Core.Tests/SummaryKeysTests.cs`

**Interfaces:**
- Consumes: Task 1~9 전부
- Produces: `IEnumerable<string> SummaryKeys.Keys`, `string SummaryKeys.KeyOf(string expandedPath, string rawPath = null)`

- [ ] **Step 1: 브랜치를 만든다**

```bash
git checkout develop
git checkout -b chore/settings-check
```

- [ ] **Step 2: `SummaryKeys`에 키 조회를 더하는 테스트를 쓴다**

`editor/CtaToybox.Core.Tests/SummaryKeysTests.cs`의 `Run` 끝에 넣는다:

```csharp
        T.Eq(new[] { "/breed/perks/\"health\"", "**/perks/\"speed\"", "/breed/perks/\"*\"" }, named.Keys.ToArray(), "Keys in file order");
        T.Eq("/breed/perks/\"health\"", named.KeyOf("/breed/perks/\"*\"", "/breed/perks/\"health\""), "KeyOf names the matching key");
        T.True(named.KeyOf("/x/y") == null, "KeyOf is null outside the summary");
```

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="`
Expected: 컴파일 오류 `'SummaryKeys'에 'Keys'…`.

- [ ] **Step 3: `SummaryKeys`를 고친다**

`editor/CtaToybox.Core/SummaryKeys.cs`에서 `readonly Dictionary<string, string> labels = …;`를

```csharp
        readonly Dictionary<string, string> labels = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly List<string> keys = new List<string>();   // 파일에 적힌 순서
```
로 바꾸고, `Add`의 `labels[path] = label;` 앞에 `if (!labels.ContainsKey(path)) keys.Add(path);`를 넣는다. `Count` 다음에 넣는다:

```csharp
        public IEnumerable<string> Keys { get { return keys; } }
```

`LabelOf`를 바꾼다:

```csharp
        // 맞는 요약 키. 요약에 없으면 null
        public string KeyOf(string expandedPath, string rawPath = null)
        {
            foreach (string key in KeyPattern.Candidates(expandedPath, rawPath, true))
                if (labels.ContainsKey(key)) return key;
            return null;
        }

        // 요약에 없으면 null. rawPath(원문 경로)를 주면 따옴표 이름 줄은 이름을 넣은 경로(/breed/perks/"health")를 먼저 찾는다
        public string LabelOf(string expandedPath, string rawPath = null)
        {
            string key = KeyOf(expandedPath, rawPath);
            return key == null ? null : labels[key];
        }
```

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release 2>&1 | grep -E "error|FAIL|passed="` → `failed=0`

- [ ] **Step 4: `RealFilesCheck`에 게임 설정 검사를 더한다**

`editor/CtaToybox.Core.Tests/RealFilesCheck.cs`의 `Run` 끝(`T.Eq("set/stuff/smg/m3_grease", …);` 다음)에 넣는다:

```csharp
        // 게임 설정(하위 프로젝트 B): 맨 위 파일이 모두 읽히고, 같은 값으로 고쳐도 바이트가 같고, 문자열 속 설정은 나눴다 이으면 같다
        int settingFiles = 0, settingUnread = 0, luaRows = 0, luaMismatch = 0, pieceStrings = 0, pieceMismatch = 0, settingWrongSlot = 0;
        var settingExamples = new List<string>();
        foreach (string vp in stack.Paths.Where(SettingFiles.IsTop).OrderBy(p => p, StringComparer.Ordinal))
        {
            settingFiles++;
            string text = stack.ReadText(vp);
            string error;
            var rows = SourceRows.Of(vp, text, out error);
            if (rows == null) { settingUnread++; settingExamples.Add("unread: " + vp + " " + error); continue; }
            if (!LuaTable.IsLua(vp)) continue;
            var edits = rows.Where(r => r.Leaves[0].Text != "nil")
                .Select(r => new Edit { Unit = vp, File = vp, Path = r.Path, Occurrence = r.Occurrence, Arg = 0, Old = r.Leaves[0].Text, New = r.Leaves[0].Text }).ToList();
            luaRows += edits.Count;
            var results = new List<EditResult>();
            string patched;
            try { patched = LuaPatcher.Apply(text, edits, results); }
            catch (Exception ex) { luaMismatch++; settingExamples.Add("lua throw: " + vp + " " + ex.Message); continue; }
            if (patched != text || results.Any(r => r.Status != EditStatus.Applied)) { luaMismatch++; settingExamples.Add("lua patch: " + vp); }
        }
        foreach (string vp in stack.Paths.Where(p => SettingFiles.InArea(p) && !LuaTable.IsLua(p) && (p.EndsWith(".set") || p.EndsWith(".inc") || p.EndsWith(".dl"))))
        {
            string error;
            var rows = SourceRows.Of(vp, stack.ReadText(vp), out error);
            if (rows == null) continue;
            foreach (var leaf in rows.SelectMany(r => r.Leaves))
            {
                var pieces = SettingStrings.Split(leaf.Text);
                if (pieces == null) continue;
                pieceStrings++;
                if (SettingStrings.Join(leaf.Text, pieces.Select(x => x.Value).ToList()) != leaf.Text) { pieceMismatch++; settingExamples.Add("pieces: " + vp + " " + leaf.Text); }
            }
        }

        // 요약 키가 실제 줄에 맞는지, 고칠 수 있다고 한 자리가 그 파일 그 자리의 글자인지(화면과 같은 세션으로 연다)
        var summaryKeys = SummaryKeys.Load(Path.Combine("editor", "summary"), SettingFiles.Kind, new KeyAnnotations());
        var matched = new HashSet<string>(StringComparer.Ordinal);
        using (var session = EditorSession.Open(settingsPath, Directory.GetCurrentDirectory(), null))
        {
            foreach (var u in session.Index.Units.Where(x => x.EditorKind == SettingFiles.Kind))
            {
                var view = session.OpenUnit(u.Path);
                foreach (var r in view.Rows)
                {
                    string k = summaryKeys.KeyOf(r.Path, r.RawPath);
                    if (k != null) matched.Add(k);
                    foreach (var v in r.Values.Where(x => x.Editable))
                    {
                        string error;
                        var fileRows = SourceRows.Of(v.Trace.File, stack.ReadText(v.Trace.File), out error);
                        var row = fileRows == null ? null : fileRows.FirstOrDefault(x => x.Occurrence == v.Trace.Occurrence && x.Path == v.Trace.KeyPath);
                        if (row == null || v.Trace.Arg >= row.Leaves.Count || row.Leaves[v.Trace.Arg].Text != v.Trace.Text)
                        { settingWrongSlot++; if (settingExamples.Count < 30) settingExamples.Add("slot: " + u.Path + " " + r.Path + " ← " + v.Trace.File + " " + v.Trace.KeyPath); }
                    }
                }
            }
        }
        var missingKeys = summaryKeys.Keys.Where(k => !matched.Contains(k)).ToList();
        Console.WriteLine("setting-files=" + settingFiles + " unread=" + settingUnread + " lua-rows=" + luaRows + " lua-mismatch=" + luaMismatch +
                          " piece-strings=" + pieceStrings + " piece-mismatch=" + pieceMismatch + " setting-wrong-slot=" + settingWrongSlot + " summary-keys-missing=" + missingKeys.Count);
        foreach (string e in settingExamples.Take(30)) Console.WriteLine("  " + e);
        foreach (string k in missingKeys) Console.WriteLine("  summary key with no row: " + k);
        T.True(settingFiles >= 22, "the game settings files were found");
        T.Eq(0, settingUnread, "every game settings file reads");
        T.True(luaRows > 300, "the conquest AI config was read");
        T.Eq(0, luaMismatch, "an identity Lua edit leaves the file byte-identical");
        T.True(pieceStrings >= 10, "the embedded setting strings were found");
        T.Eq(0, pieceMismatch, "every embedded setting string splits and joins back to its bytes");
        T.Eq(0, settingWrongSlot, "every editable settings value names the slot that holds that token");
        T.Eq(0, missingKeys.Count, "every settings summary key matches a real row");
```

파일 머리 `Prefixes`·`Extensions`는 그대로 둔다(유닛 범위 검사).

- [ ] **Step 5: 실제 파일 검증을 돌린다 (15분 남짓, 백그라운드)**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real 2>&1 | tail -30`
Expected: `setting-files=22 이상 unread=0 … lua-mismatch=0 piece-mismatch=0 setting-wrong-slot=0 summary-keys-missing=0`, 마지막 줄 `failed=0`. 앞부분의 `codec-mismatch=0 patch-mismatch=0`, `wrong-slot=0`도 본다(색인 형식 3 이후 처음 돌리는 것이다).

`summary key with no row`가 나오면: 그 키의 경로를 `dotnet run --project editor/CtaToybox.Cli -c Release -- trace <파일> --filter <키 끝>`으로 확인해 `editor/summary/settings.psv`를 실제 경로로 고친다. 실제로 없는 키면 지운다. `slot:`이 나오면 추적 버그다. 그 경로로 실패하는 단위 테스트를 먼저 쓰고 고친다.

- [ ] **Step 6: 커밋한다**

```bash
git add editor/CtaToybox.Core/SummaryKeys.cs editor/CtaToybox.Core.Tests/RealFilesCheck.cs editor/CtaToybox.Core.Tests/SummaryKeysTests.cs editor/summary/settings.psv
git commit -m "test(editor): real-file checks for game settings - reads, identity Lua patch, piece strings, slots, summary keys"
```

### Task 11: 화면 확인과 문서

**Files:**
- Modify: `docs/07-stat-editor/README.md` (게임 설정 절)
- Modify: `CLAUDE.md` (손으로 고치는 파일)

- [ ] **Step 1: 시나리오를 쓴다**

스크래치패드에 `settings.scenario`를 쓴다(`<S>`는 스크래치패드 경로):

```text
unit set/dynamic_campaign/resources_standard.set
tab summary
shot 01-resources-summary
open StartVal
shot 02-startval-dialog
type 1 9137
ok
unit set/multiplayer/games/battle_zones.set
tab full
filter parameters
shot 03-parameters-full
open parameters
shot 04-parameters-dialog
piece 1 scoreStart 777
shot 05-parameters-typed
ok
unit set/dynamic_campaign/duration_normal.set
tab summary
open 판마다 깃발 수
type 1 2:5 3:5
shot 06-steps-dialog
cancel
unit script/multiplayer/modes/conquest/runtime_config.lua
tab summary
shot 07-lua-summary
open AttackMin
type 1 5 * 60000
shot 08-lua-dialog
ok
toggle 1
shot 09-toggled
remove 2
shot 10-removed
```

- [ ] **Step 2: 앱을 한 번 실행해 찍는다**

Run:
```bash
dotnet run --project editor/CtaToybox.App -c Release -- --project "<S>/settings-check-tmp.json" --scenario "<S>/settings.scenario" --out "<S>/settings-shots"
```
Expected: `<S>/settings-shots/01-…png`부터 `10-…png`. 시나리오 기록에 `open: 줄을 찾지 못했다`가 있으면 그 명령의 글자를 화면의 실제 이름으로 고쳐 다시 실행한다(한 번에 모아 고친다).

- [ ] **Step 3: 찍은 화면을 사용자 입장에서 읽는다**

PNG를 Read 도구로 하나씩 본다. 볼 것:
- 요약 묶음 이름과 줄 이름이 한글인가. 값마다 뜻이 보이는가("뜻 미확인"만 있는 값이 요약에 있으면 실패다)
- 조각 칸마다 뜻과 원문 이름, 원래 값이 보이는가. 단계표 풀이가 새 값을 따라가는가
- Lua 줄에 한글 뜻, 원문 주석, 계산값(`= 1000`)이 보이는가. "이 유닛만"이 꺼져 있고 이유가 보이는가
- 가격·로스터, 무기·탄약 탭이 게임 설정에서 숨겨졌는가

문제가 있으면 모아서 한 번에 고친 뒤 Step 2를 한 번 더 한다.

- [ ] **Step 4: 문서를 고친다**

`docs/07-stat-editor/README.md`의 "## 화면 (CtaToybox.exe)" 절 끝(화면 확인용 옵션 문단 다음)에 넣는다(CRLF 유지, Edit 도구로):

```markdown
### 게임 설정 (하위 프로젝트 B)

종류에서 "게임 설정"을 고르면 컨퀘스트 자원(`resources_*.set`), 컨퀘스트 길이(`duration_*.set`), 컨퀘스트 전투 규칙(`campaign_capture_the_flag.set`), 컨퀘스트 AI(`runtime_config.lua`), 스커미시 모드(`set/multiplayer/games/*.set`), 난이도(`set/difficulty/*.dl`)가 나온다. 이름은 `editor/settings/files.psv`, 키의 뜻은 `data/annotations/settings.psv`, 요약 키는 `editor/summary/settings.psv`에 있다. 설계는 [../superpowers/specs/2026-10-15-settings-editor-design.md](../superpowers/specs/2026-10-15-settings-editor-design.md).

- 수정은 항상 "전부"다. 파일 하나가 곧 설정이다. 가격·로스터와 무기·탄약 탭은 숨긴다.
- `"이름=값;이름=값"` 문자열(게임 모드의 `parameters`, `unitLimits`)은 조각마다 칸이 있다. 조각 이름·개수·순서는 바꿀 수 없다. 조각의 뜻은 `values.psv`의 자리 `@이름`.
- `"1:200 4:250"` 같은 판 수 단계표는 값 고치기 창 아래에 풀이를 보인다. 칸마다 수의 개수가 원래와 같아야 한다.
- 컨퀘스트 AI(Lua)는 이 파일에 나오는 꼴만 읽는다. 수식은 계산값을 함께 보이고, 개발사 영어 주석을 "원문 주석"으로 보인다. 새 값은 원래와 같은 종류(수·수식, 문자열, 참·거짓)여야 하고 `nil`은 고칠 수 없다. 모르는 문법이 있으면 파일 전체를 읽지 못함으로 보이고 고치지 못한다.
- 멀티플레이는 모든 사람이 같은 모드를 써야 한다. 설정 파일을 바꾼 모드는 다른 사람과 어긋날 수 있다.
- 진행 중인 컨퀘스트 캠페인에 반영되는지는 확인하지 않았다. 게임 안 확인은 [ingame-check.md](ingame-check.md).
```

`CLAUDE.md`의 "## 손으로 고치는 파일" 목록에서 `- \`editor/summary/<종류>.psv\`: …` 줄 다음에 넣는다(Edit 도구로):

```markdown
- `editor/settings/files.psv`: 게임 설정 항목의 화면 이름(경로|묶음|화면 이름)
- `data/annotations/settings.psv`: 게임 설정 파일 전용 키 주석. 형식은 `common.psv`와 같다
```

- [ ] **Step 5: 커밋한다**

```bash
git add docs/07-stat-editor/README.md CLAUDE.md
git commit -m "docs: game settings in the editor README and the hand-edited file list"
```

이 Task에서 화면을 고쳤으면 그 파일도 경로를 골라 add해 같은 커밋 앞에 따로 커밋한다(`fix(editor): …`).

### Task 12: 게임 안 확인 (한 빌드에 몰아서)

**Files:**
- Create: `editor/projects/settings-check.json` (커밋하지 않는다)
- Modify: `docs/07-stat-editor/ingame-check.md` (결과)

- [ ] **Step 1: 확인 대상을 데이터에서 먼저 집는다**

Run:
```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- show set/dynamic_campaign/resources_standard.set --filter startval
dotnet run --project editor/CtaToybox.Cli -c Release -- show set/multiplayer/games/battle_zones.set --filter parameters
dotnet run --project editor/CtaToybox.Cli -c Release -- show set/multiplayer/games/presets/global_parameters/unitlimits.inc
grep -rln "officer" _work/vanilla/set/multiplayer/units | head
grep -rn "\"officer\"\|unitlimit\|limit officer\|class.*officer" _work/vanilla/set/multiplayer/units/*.set | head
```
Expected: 시작 MP·RP의 지금 값과 계층(이 PC에서는 `mod_3732639687`), `parameters` 문자열 전체, `unitLimits` 문자열 전체. "officer" 제한이 걸리는 유닛 분류를 로스터에서 찾고, 그 병종의 게임 속 표시 이름을 `data/display-names.csv`에서 센다(이름이 한 병종에만 쓰이는지). 못 찾으면 확인 3은 "구매 화면에 7이 보이는지"만 묻고 대상은 "미확인"으로 적는다.

- [ ] **Step 2: 확인용 수정을 넣는다**

`<PARAM>`은 Step 1의 `parameters` 원래 문자열에서 `scoreStart=250`만 `scoreStart=777`로 바꾼 것, `<LIMITS>`는 `unitLimits` 문자열에서 `officer=2`만 `officer=7`로 바꾼 것이다(따옴표 포함):

```bash
P=editor/projects/settings-check.json
C="dotnet run --project editor/CtaToybox.Cli -c Release --"
$C add --project $P --file set/dynamic_campaign/resources_standard.set --path /standard/resources/mp/startval --new 9137
$C add --project $P --file set/dynamic_campaign/resources_standard.set --path /standard/resources/rp/startval --new 37
$C add --project $P --file set/multiplayer/games/battle_zones.set --path /game/parameters --new '<PARAM>'
$C add --project $P --file set/multiplayer/games/presets/global_parameters/unitlimits.inc --path /unitlimits --new '<LIMITS>'
$C add --project $P --file script/multiplayer/modes/conquest/runtime_config.lua --path /conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/degraded --new 1135
$C add --project $P --file script/multiplayer/modes/conquest/runtime_config.lua --path /conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/reduced --new 1165
$C add --project $P --file script/multiplayer/modes/conquest/runtime_config.lua --path /conquestconfig.attackerdynamicwaveoff/waveoffsecondsbyattackerstate/retained --new 1210
```
Expected: 일곱 줄 모두 `수정을 더했다`. 경로가 다르다고 나오면 Step 1의 `show` 출력에 있는 경로를 그대로 쓴다(Wunderwaffe 판의 맨 위 블록 이름이 다를 수 있다).

- [ ] **Step 3: 게임이 꺼진 것을 확인하고 빌드한다**

Run:
```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- build --project editor/projects/settings-check.json
```
Expected: `적용 7건 … 파일 오류 0건`, `씀:` 네 파일(`resource/set/dynamic_campaign/resources_standard.set`, `…/battle_zones.set`, `…/unitlimits.inc`, `resource/script/multiplayer/modes/conquest/runtime_config.lua`). "게임이나 에디터가 실행 중이다"로 멈추면 사용자에게 게임을 꺼 달라고 하고 다시 한다(`--force`를 쓰지 않는다).

- [ ] **Step 4: 사용자에게 한 번에 부탁한다**

아래를 그대로 보낸다(Step 1에서 찾은 이름으로 3을 채운다):

> 게임을 켜서 다음 넷을 봐 주세요. 모드 목록에서 CTAToybox가 맨 뒤에 켜져 있어야 합니다.
> 1. 새 컨퀘스트를 자원 "standard"로 시작했을 때 첫 화면의 MP와 RP (기대: MP 9137, RP 37)
> 2. 스커미시 배틀존을 시작했을 때 양쪽 점수 막대의 시작 점수 (기대: 777)
> 3. 스커미시 구매 화면에서 <병종 이름>의 수 제한 표시 (기대: 7)
> 4. 컨퀘스트 전투 하나를 수비로 해서, AI 공격 파도가 한 번 끝날 때까지 진행한 뒤 게임을 꺼 주세요. 이건 제가 로그로 봅니다.
>
> 난이도 배수는 숫자로 볼 곳이 없어 이번에는 확인하지 않습니다.

- [ ] **Step 5: 로그를 읽는다**

Run: `grep -n "attacker wave-off pressure\|Exception\|LUA.*error" "/c/Users/deepe/AppData/Local/digitalmindsoft/gates of hell/log/game.log" | tail -20`
Expected: `targetWaveOffSeconds=` 뒤 값이 1000을 넘는다(1135·1165·1210에 보정과 ±10초). 1000 아래면 모드의 Lua 파일이 바닐라를 덮어쓰지 않은 것이다. 로그 줄이 없으면 파도가 끝나지 않은 것이다(결과는 "판단 못 함").

- [ ] **Step 6: 확인용 수정을 끄고 다시 빌드한다 (게임이 꺼진 상태)**

```bash
pwsh -NoProfile -Command "$p='editor/projects/settings-check.json'; $j=Get-Content $p -Raw | ConvertFrom-Json; $j.edits | ForEach-Object { $_.enabled = $false }; $j | ConvertTo-Json -Depth 5 | Set-Content $p -Encoding utf8"
dotnet run --project editor/CtaToybox.Cli -c Release -- build --project editor/projects/settings-check.json
```
Expected: `지움:` 네 파일, `mod/CTAToybox/resource`에 `.gitkeep`만 남는다(`find mod/CTAToybox -type f`).

- [ ] **Step 7: 결과를 적고 병합한다**

`docs/07-stat-editor/ingame-check.md` 끝에 "게임 설정 (하위 프로젝트 B, 2026-10-15~)" 절을 더한다(Edit 도구, CRLF). 표: `| # | 고친 것 | 볼 곳 | 기대 | 결과 |`, 사용자가 본 값과 로그 줄을 그대로 적는다. 판단하지 못한 것은 "판단 못 함"과 이유를 적는다. 스펙의 "알려진 한계" 가운데 확인된 것(Lua 덮어쓰기 등)은 `docs/07-stat-editor/README.md`의 게임 설정 절에도 반영한다.

```bash
git add docs/07-stat-editor/ingame-check.md docs/07-stat-editor/README.md
git commit -m "docs: record in-game results for the game settings editor"
git checkout develop
git merge --no-ff chore/settings-check -m "Merge branch 'chore/settings-check' into develop"
```
