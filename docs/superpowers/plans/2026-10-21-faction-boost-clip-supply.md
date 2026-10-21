# 팩션 강화 보강(클립식 장전·보급·수정 내용 문서) 구현 계획

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 팩션 강화가 클립식 포의 발 사이 간격도 같은 배수로 줄이고, 그 군대 보급 유닛의 보급 매크로를 복제해 반경 ×3·총량 무제한으로 바꾸며, 강화 내용을 사람이 읽는 문서로 만든다.

> **2026-10-21 구현 뒤 바뀜**: 이 계획의 `limit 0`(파일 주석 "0이면 무제한")은 게임에서 보급이 동작하지 않아 총량 50000, 재생 주기 0.0002초(10초에 가득)로 바꿨다. 아래 단계의 `limit 0`은 처음 계획 그대로 둔 기록이다. 지금 동작은 설계 문서의 "구현 뒤 바뀐 것"을 본다.

**Architecture:** 새 수정 종류 `define-copy`(매크로 정의 복제)를 블록 복제(E18 `block-copy`)와 같은 길에 넣는다. 원본 본문에 복제를 먼저 넣고(덧씌우기 `SourceStack.Overlay`, 빌드의 `StageItem`), 값 수정과 매크로 호출 이름 바꾸기(E4 `macro-name`)는 그 본문에 적용한다. `FactionPreset`에 클립식 장전(무기 사본 단계)과 보급(새 단계)을 더하고, 명령줄이 결과를 `docs/14-faction-boost/<군대>.md`로 쓴다.

**Tech Stack:** C# .NET 8(외부 패키지 없음), 자체 테스트 `editor/CtaToybox.Core.Tests`(`T.Eq`·`T.True`, `Suites.All` 등록, xUnit이 묶음마다 돌린다), PowerShell 7 스크립트.

**Spec:** `docs/superpowers/specs/2026-10-21-faction-boost-clip-supply-design.md` (원래 설계 `docs/superpowers/specs/2026-10-17-faction-boost-design.md`)

## Global Constraints

- 게임 폴더·창작마당 폴더의 파일을 고치지 않는다. 바뀐 것은 출력 모드(`mod/CTAToybox`)와 저장소에만 쓴다.
- 게임 안 확인은 하지도 제안하지도 않는다. 게임으로만 알 수 있는 것은 문서에 "미확인"으로 적는다.
- 빌드(`build`, `spawn-menu build`)는 게임이 꺼져 있을 때만 한다(`Get-Process call_to_arms`로 확인).
- 다른 군대의 유닛 값과 보급 건물 6종의 값은 빌드 전과 같아야 한다.
- 사본·복제본 이름: 원래 이름 + `_ctb_<군대>`(예 `truck_ammo_heavy_ctb_ger`).
- 클립식 기준: `set/stuff/gun/` 아래 무기 사본, 펼친 탄창 발수 ≥ 2, 펼친 발 사이 ≥ 1초(기본 `--clip-min-gap 1`). 발 사이 배수 기본은 재장전 배수(`--reload`, 0.5).
- 클립식 포는 발 사이·탄창 장전 모두 소수 셋째 자리까지 남긴다(35 → 17.5). 다른 무기는 지금 규칙(정수는 정수로) 그대로.
- 보급: 반경 ×3(`--supply-radius 3`), 총량 `limit`을 0으로(`--supply-limit 0`, 파일 주석 "0이면 무제한"). 재생 주기·보급 주기·품목·`DecalView`·컨퀘스트 조건 줄은 그대로.
- 보급 건물 6종(`entity/service/-multiplayer/flag_point/flagpoint_ammo/flagpoint_ammo.def`, `entity/service/-multiplayer/ammo_point/ammo/ammo2.def`, `…/ammo3.def`, `entity/construction/_military/boxes/crates_set02_resupply/crates_set02_resupply.def`, `entity/construction/_military/environment/barrels/barrels_covered1/barrels_covered1_supply.def`, `entity/construction/_military/medic_rally/medic_rally.def`)은 바꾸지 않는다.
- 매크로 복제본은 원본 파일 끝에 붙인다(원본의 토큰 자리와 다른 줄의 순번이 밀리지 않게).
- 복제본 안의 값 수정과 복제본 이름으로 바꾸는 호출은 범위 `shared`다.
- 주석·메시지·문서는 한국어, 짧게. 새 테스트는 지금 꼴(`T.Eq`)로 쓰고 `Suites.All`에 등록한다.
- 커밋 메시지 끝에 `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.

## Review Focus

1. **모드 갱신으로 원본 정의가 사라짐**: 복제가 적용되지 않으면 그 복제본을 부르는 호출 이름 바꾸기와 복제본 안의 값 수정이 점검·빌드에서 막혀야 한다(호출이 없는 정의를 부르면 게임이 `define not found`로 꺼진다). → Task 2 테스트 `DisabledCopyBlocksDependents`.
2. **세션 없는 빌드(명령줄 `build`)**: 덧씌우기가 없는 `SourceStack`으로 빌드해도 이름 바꾸기 점검이 복제본을 찾아야 한다. → Task 2 테스트 `PlainStackBuild`.
3. **보급 매크로를 유닛 파일 밖(include 파일)에서 부르는 유닛**: 이름을 바꾸지 않고 이유와 함께 건너뛴다. → Task 4 테스트 `CallInIncludeIsSkipped`.
4. **클립식 발 사이가 원형에서 오는 무기**: 사본에 `{recoveryTime N}` 줄을 더한다. → Task 3 테스트 `GapFromPatternAddsLine`.
5. **`faction-preset`을 두 번 실행**: 복제·이름 바꾸기·클립 수정이 겹치지 않는다. → Task 3·Task 4 테스트의 "다시 실행" 단언.

---

### Task 0: 작업 브랜치

**Files:** 없음

- [ ] **Step 1: 브랜치를 딴다**

PR #9(`chore/fusion-2026-10-21`)가 develop에 머지됐는지 본다.

```bash
gh pr view 9 --repo game-mod-project/CTAGOHToybox --json state --jq .state
```

`MERGED`면 `git switch develop && git pull && git switch -c feat/faction-boost-clip-supply`. 아니면 `git switch chore/fusion-2026-10-21 && git switch -c feat/faction-boost-clip-supply`이고, 마지막 PR 설명에 "PR #9 위에 쌓았다"고 적는다.

- [ ] **Step 2: 설계 문서를 커밋한다**

```bash
git add docs/superpowers/specs/2026-10-21-faction-boost-clip-supply-design.md docs/superpowers/plans/2026-10-21-faction-boost-clip-supply.md
git commit -m "docs: design and plan for clip reload, supply and boost doc"
```

---

### Task 1: 매크로 정의 복제(`define-copy`)의 본문 처리

**Files:**
- Create: `editor/CtaToybox.Core/DefineCopy.cs`
- Create: `editor/CtaToybox.Core/TextCopies.cs`
- Create: `editor/CtaToybox.Core.Tests/DefineCopyTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (`Suites.All`에 `("DefineCopy", DefineCopyTests.Run)`을 `("BlockCopy", …)` 다음에)

**Interfaces:**
- Produces:
  - `DefineCopy.Kind = "define-copy"`
  - `string DefineCopy.PathOf(string name)` → `/(define "<name 소문자>")`
  - `string DefineCopy.CheckName(string newToken)`: 되면 null. 규칙은 `BlockCopy.CheckName`과 같다(따옴표로 감싼 이름 하나, 빈칸·따옴표·`/` 없음).
  - `Edit DefineCopy.New(string unit, string file, string name, string newName)`: `Kind=DefineCopy.Kind, Unit=unit, File=file, Path=PathOf(name), Occurrence=0, Arg=0, Old="\"name\"", New="\"newName\"", Scope="shared"`.
  - `string DefineCopy.Apply(string text, IEnumerable<Edit> copies, List<EditResult> results)`: 켜진 `define-copy`를 수정 순서대로 적용한 본문. 수정마다 결과 하나.
  - `string DefineCopy.Dependency(Edit e, IEnumerable<Edit> copies)`: e가 기대는 복제본 이름(따옴표 없이). 값 수정의 `Path`가 `PathOf(새 이름)`으로 시작하거나, `macro-name` 수정의 `New`가 `"새 이름"`이면 그 이름. 아니면 null.
  - `string DefineCopy.MissingNote(string name)` → `"매크로 " + name + "가 없다(매크로 복제가 꺼졌거나 적용되지 않았다)"`
  - `bool TextCopies.IsCopy(string kind)`: `block-copy` 또는 `define-copy`
  - `string TextCopies.Apply(string text, IEnumerable<Edit> fileEdits, List<EditResult> results)`: `BlockCopy.Apply` 다음 `DefineCopy.Apply`
  - `string TextCopies.ApplyAll(string text, IEnumerable<Edit> fileEdits)`: 결과를 버리는 판
  - `Func<string, string, string> TextCopies.Overlay(EditProject project)`: 켜진 복제가 없으면 null. 있으면 `(가상 경로, 본문)` → 그 파일의 복제를 넣은 본문
  - `string TextCopies.Dependency(Edit e, IEnumerable<Edit> all)`: `BlockCopy.Dependency ?? DefineCopy.Dependency`
  - `string TextCopies.MissingNote(string name, IEnumerable<Edit> all)`: `all`에 `New`가 `"name"`인 `define-copy`가 있으면 `DefineCopy.MissingNote(name)`, 아니면 `BlockCopy.MissingNote(name)`

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`DefineCopyTests.Run()`:

```csharp
const string F = "properties/resupply.inc";
const string Text = "(define \"a\"\n {x {radius 10} {limit %supply}}\n)\n(define \"b\" {y 1})\n";
var r = new List<EditResult>();
string one = DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "a", "a_ctb_ger") }, r);
T.Eq(EditStatus.Applied, r.Single().Status, "a define is copied");
T.True(one.StartsWith(Text), "the original text stays byte for byte in front: " + one);
T.True(one.EndsWith("(define \"a_ctb_ger\"\n {x {radius 10} {limit %supply}}\n)\n"), "the copy goes to the end with only the name changed: " + one);
string error;
var rows = SourceRows.Of(F, one, out error);
T.True(rows.Any(x => x.Path == "/(define \"a_ctb_ger\")/x/radius" && x.Leaves[0].Text == "10"), "rows inside the copy have their own path");

r.Clear(); DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "zz", "zz2") }, r);
T.Eq(EditStatus.TargetMissing, r.Single().Status, "a missing define is TargetMissing");
r.Clear(); DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "a", "b") }, r);
T.Eq(EditStatus.Invalid, r.Single().Status, "a name that already exists is refused");
r.Clear(); DefineCopy.Apply(Text, new[] { new Edit { Kind = DefineCopy.Kind, File = F, Path = DefineCopy.PathOf("a"), Old = "\"a\"", New = "\"a b\"", Enabled = true } }, r);
T.Eq(EditStatus.Invalid, r.Single().Status, "a bad new name is refused");
string two = DefineCopy.Apply(Text, new[] { DefineCopy.New(F, F, "a", "a2"), DefineCopy.New(F, F, "b", "b2") }, new List<EditResult>());
T.True(two.IndexOf("(define \"a2\"") < two.IndexOf("(define \"b2\""), "copies go in edit order");
var off = DefineCopy.New(F, F, "a", "a3"); off.Enabled = false;
T.Eq(Text, DefineCopy.Apply(Text, new[] { off }, new List<EditResult>()), "a disabled copy does nothing");

var copies = new List<Edit> { DefineCopy.New(F, F, "a", "a_ctb_ger") };
T.Eq("a_ctb_ger", DefineCopy.Dependency(new Edit { File = F, Path = "/(define \"a_ctb_ger\")/x/radius" }, copies), "a value inside the copy depends on it");
T.Eq("a_ctb_ger", DefineCopy.Dependency(new Edit { Kind = MacroSwap.Kind, File = "u.def", Path = "/e/(\"a\")", New = "\"a_ctb_ger\"" }, copies), "a call renamed to the copy depends on it");
T.Eq(null, DefineCopy.Dependency(new Edit { File = F, Path = "/(define \"a\")/x/radius" }, copies), "the original does not");

var project = new EditProject { Edits = copies };
var overlay = TextCopies.Overlay(project);
T.True(overlay(F, Text).Contains("(define \"a_ctb_ger\""), "the overlay copies in that file");
T.Eq(Text, overlay("other.inc", Text), "and leaves other files alone");
T.True(TextCopies.Overlay(new EditProject()) == null, "no copies, no overlay");
T.True(TextCopies.IsCopy(BlockCopy.Kind) && TextCopies.IsCopy(DefineCopy.Kind) && !TextCopies.IsCopy(MacroSwap.Kind), "both copy kinds");
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 오류 `DefineCopy`가 없다.

- [ ] **Step 3: `DefineCopy`와 `TextCopies`를 만든다**

`DefineCopy.Apply`는 `BlockCopy.Apply`와 같은 꼴(파싱 오류면 모두 `Invalid`, 수정마다 다시 파싱)이다. 원본 정의는 맨 위 깊이의 `(` 다음 낱말 `define`, 다음 문자열이 `Old`와 같은(대소문자 무시) 것이다. 그 `(`부터 짝 `)`까지를 복사해 이름 토큰만 `New`로 바꾸고, 본문 끝에 붙인다(끝이 줄바꿈이 아니면 줄바꿈 하나 먼저, 붙인 뒤 줄바꿈 하나. 파일의 줄바꿈 꼴 `\r\n`/`\n`을 따른다). 같은 이름의 정의가 이미 있으면 `Invalid "이미 있는 매크로 이름이다: …"`.

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== DefineCopy ok`, 마지막 줄 `failed=0`.

- [ ] **Step 5: 커밋**

```bash
git add editor/CtaToybox.Core/DefineCopy.cs editor/CtaToybox.Core/TextCopies.cs editor/CtaToybox.Core.Tests/DefineCopyTests.cs editor/CtaToybox.Core.Tests/Program.cs
git commit -m "feat(editor): define-copy edit kind (macro definition copy)"
```

---

### Task 2: 매크로 복제를 점검·빌드·세션·명령줄·앱에 잇고, 인자 있는 호출의 이름 바꾸기를 연다

**Files:**
- Modify: `editor/CtaToybox.Core/ModBuilder.cs` (91~115행 복제 이름·의존·이름 바꾸기 점검, 413~430행 `StageItem`)
- Modify: `editor/CtaToybox.Core/EditorSession.Edits.cs` (45·66·84·97·135행의 `BlockCopy.ApplyAll` → `TextCopies.ApplyAll`, 303~312행 `Check`, 359~389행 `SourceOf`·`CopiesOf`·`ActiveCopies`)
- Modify: `editor/CtaToybox.Core/EditorSession.cs` (368행 `Describe`, 599~606행 `RefreshOverlay`)
- Modify: `editor/CtaToybox.Core/Edit.cs` (28행 주석, 93행 `SameSlot`)
- Modify: `editor/CtaToybox.Core/MacroSwap.cs` (1~3행 머리 주석: 인자 있는 호출도)
- Modify: `editor/CtaToybox.Cli/Program.cs` (19행 사용법, 576~609행 `Add`)
- Modify: `editor/CtaToybox.App/Items.cs:104`, `editor/CtaToybox.App/MainWindow.Unit.cs:131`
- Create: `editor/CtaToybox.Core.Tests/DefineCopyBuildTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (`("DefineCopyBuild", DefineCopyBuildTests.Run)`)

**Interfaces:**
- Consumes: Task 1의 `DefineCopy.*`, `TextCopies.*`
- Produces:
  - `ModBuilder.Build(EditProject)`이 들어올 때 `stack.Overlay`를 `TextCopies.Overlay(project)`로 두고 나갈 때 원래 값으로 되돌린다(이름 바꾸기 점검의 추적기가 복제본을 본다).
  - `EditorSession.Describe`가 `define-copy`에 `ValueLabel = "매크로 복제", RowText = e.Old, ValueCount = 1, BlockNames = 빈 목록`을 돌려준다.
  - 명령줄 `add --kind define-copy --file <파일> --path '/(define "<이름>")' --new '"<새 이름>"'`
  - 명령줄 `add --kind macro-name`이 인자 있는 호출 줄(`found.Head != null && MacroSwap.NameOf(found.RawPath) != null`, `--arg 0`)도 받는다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`DefineCopyBuildTests`의 픽스처(`MacroSwapTests.Fixture`와 같은 꼴):
- `properties.pak` → `properties/resupply.inc` = `"(define \"truck_ammo\"\n {extender \"supply_zone\" {radius 20} {limit 3000}}\n)\n(define \"crate\"\n {extender \"supply_zone\" {radius 10} {limit %supply}}\n)\n"`
- `entity/-vehicle.pak` → `entity/-vehicle/germany/car/truck/truck.def` = `"{game_entity\n (include \"/properties/resupply.inc\")\n (\"truck_ammo\")\n}\n"`, `entity/-vehicle/germany/trailer/cart/cart.def` = `"{game_entity\n (include \"/properties/resupply.inc\")\n (\"crate\" supply(500))\n}\n"`

수정 여섯 개(모두 `Scope="shared"`): `DefineCopy.New(F, F, "truck_ammo", "truck_ammo_x")`, 값 `/(define "truck_ammo_x")/extender[supply_zone]/radius` `20 → 60`, `macro-name` `truck.def` `/game_entity/("truck_ammo")` `"truck_ammo" → "truck_ammo_x"`, `DefineCopy.New(F, F, "crate", "crate_x")`, 값 `/(define "crate_x")/extender[supply_zone]/limit` `%supply → 0`, `macro-name` `cart.def` `/game_entity/("crate")` `"crate" → "crate_x"`. 이 여섯 개를 `EditProject.Save`로 프로젝트 파일에 쓰고 `session.OpenProject(projectPath)`로 연다(여는 때 덧씌우기가 생긴다).

이름: `F = "properties/resupply.inc"`, `Truck = "entity/-vehicle/germany/car/truck/truck.def"`, `Cart = "entity/-vehicle/germany/trailer/cart/cart.def"`, `Original` = 위 `resupply.inc` 본문, `copyEdit` = `session.Project.Edits`의 첫 `define-copy`, `Out` = `FactionPresetTests.Out`, `cliProject` = 새 임시 프로젝트 경로, `w`·`w2` = `StringWriter`.

```csharp
// 세션에서 점검과 빌드
var bad = session.CheckEdits().Where(c => c.Status != EditStatus.Applied).ToList();
T.Eq(0, bad.Count, "every edit checks: " + string.Join("; ", bad.Select(c => c.Edit.Path + " " + c.Message)));
var report = session.Build();
T.Eq(6, report.Count(EditStatus.Applied), "the build applies all six");
string inc = Out(outDir, "properties/resupply.inc");
T.True(inc.StartsWith(Original), "the original defines stay as they were");
T.True(inc.Contains("(define \"truck_ammo_x\"\n {extender \"supply_zone\" {radius 60} {limit 3000}}\n)") && inc.Contains("{limit 0}"), "the copies with their values: " + inc);
T.True(Out(outDir, Truck).Contains("(\"truck_ammo_x\")"), "the truck calls the copy");
T.True(Out(outDir, Cart).Contains("(\"crate_x\" supply(500))"), "a call with arguments keeps its arguments");
var d = session.Describe(copyEdit);
T.True(d != null && d.ValueLabel == "매크로 복제", "the edit list names a define copy");

// DisabledCopyBlocksDependents
copyEdit.Enabled = false;
var checks = session.CheckEdits();
T.True(checks.Where(c => c.Edit.File == Truck || c.Edit.Path.Contains("truck_ammo_x")).All(c => c.Status == EditStatus.Invalid && c.Message.Contains("truck_ammo_x")), "the check blocks what needs the copy");
T.True(session.Build().Results.Where(r => r.Edit.File == Truck).All(r => r.Status == EditStatus.Invalid), "and so does the build");
copyEdit.Enabled = true;

// PlainStackBuild: 덧씌우기가 없는 새 스택(명령줄 build와 같은 조건)
using var plain = new SourceStack(session.Plan.Layers); plain.Scan();
T.True(plain.Overlay == null, "a fresh stack has no overlay");
var plainReport = new ModBuilder(plain, T.TempDir()).Build(session.Project);
T.Eq(6, plainReport.Count(EditStatus.Applied), "a build without a session overlay still finds the copies");
T.True(plain.Overlay == null, "and leaves the stack as it was");

// 명령줄
T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--kind", "define-copy", "--file", F, "--path", "/(define \"crate\")", "--new", "\"crate_y\"" }, w), "CLI add copies a define");
T.Eq(0, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--kind", "macro-name", "--file", Cart, "--path", "/game_entity/(\"crate\")", "--new", "\"crate_y\"" }, w), "CLI renames a call with arguments to the copy: " + w);
T.Eq(1, CliApp.Run(new[] { "add", "--settings", settings, "--project", cliProject, "--kind", "macro-name", "--file", Cart, "--path", "/game_entity/(\"crate\")", "--new", "\"nope\"" }, w2), "a name that is not defined is refused");
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `DefineCopyBuild FAILED`(복제본이 빌드에 들어가지 않고 이름 바꾸기 점검이 "같은 파일에 정의되어 있지 않다"로 막는다).

- [ ] **Step 3: 잇는다**

- `ModBuilder.Build`: 들어오며 `var oldOverlay = stack.Overlay; stack.Overlay = TextCopies.Overlay(project);`, `finally`에서 되돌린다. 91~107행의 복제 이름 모으기를 `block-copy`·`define-copy` 둘 다로(`TextCopies.Apply`), 의존은 `TextCopies.Dependency`, 메시지는 `TextCopies.MissingNote`. `StageItem` 414행의 복제 적용을 `TextCopies.IsCopy`인 수정 전부로, 430행 값 수정 목록에서 `define-copy`를 뺀다.
- `EditorSession`: `CopiesOf`가 두 종류를 돌려주고, `SourceOf`·`ValidateNew`·`BlockOfValue` 등은 `TextCopies.Apply(All)`을 쓴다. `Check`의 복제 결과 분기(303행)를 `TextCopies.IsCopy(e.Kind)`로, 의존(310행)을 `TextCopies.Dependency`로. `RefreshOverlay`는 `Stack.Overlay = TextCopies.Overlay(project)`.
- `Edit.SameSlot`: `define-copy`도 `New`가 달라야 다른 수정이다.
- 명령줄 `Add`: `--kind`에 `define-copy`를 더한다. 본문은 그 파일의 두 종류 복제를 넣은 것(`TextCopies.ApplyAll`)이다. `define-copy`면 `DefineCopy.Apply`로 넣어 보아 `Applied`일 때만 더한다. `macro-name` 점검 전에 `stack.Overlay = TextCopies.Overlay(project)`(새 수정이 복제면 그것까지 넣은 프로젝트로).
- 앱: `Items.EditTitle`에 `if (e.Kind == DefineCopy.Kind) return "매크로 복제: " + Value(e.Old) + " → " + Value(e.New);`, `CopyKey`는 `TextCopies.IsCopy(e.Kind)`.

- [ ] **Step 4: 통과를 확인한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: `all test files passed`, 앱 빌드 `경고 0개`.

- [ ] **Step 5: 커밋**

```bash
git add editor
git commit -m "feat(editor): build, check and CLI for define copies; rename calls with arguments"
```

---

### Task 3: 클립식 포의 발 사이 간격

**Files:**
- Modify: `editor/CtaToybox.Core/FactionPreset.cs` (`FactionOptions`, `FactionReport`)
- Modify: `editor/CtaToybox.Core/FactionPreset.Weapons.cs` (64~92행 3단계)
- Create: `editor/CtaToybox.Core.Tests/FactionClipTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (`("FactionClip", FactionClipTests.Run)`을 `("FactionPreset", …)` 다음에)
- Modify: `editor/CtaToybox.Core.Tests/FactionPresetTests.cs` (픽스처 확장. 지금 `Fixture(out repo, out outDir, params extraSet)` 호출은 그대로 둔다)

**Interfaces:**
- Produces:
  - 테스트 픽스처 `FactionPresetTests.FixtureWith(out string repo, out string outDir, (string Name, string Text)[] set, (string Name, string Text)[] vehicles, (string Name, string Text)[] properties)`: `set`은 `set.pak`에 더하고, `vehicles`는 새 pak `resource/entity/extra.pak`에 넣고(항목 이름은 가상 경로에서 `entity/`를 뗀 것. 그래서 `entity/service/…`도 넣을 수 있다), `properties`는 `properties.pak`에 더한다. 지금 `Fixture`는 `FixtureWith(extraSet, 빈 배열, 빈 배열)`을 부른다. Task 4가 다시 쓴다.
  - `FactionOptions.ClipMinGap` (double, 기본 1), `FactionOptions.ClipGap` (double, 기본 0 = `Reload`와 같게)
  - `public sealed class ClipChange { public string Weapon; public int Filling; public string GapOld; public string GapNew; public string ReloadOld; public string ReloadNew; }`
  - `FactionReport.Clips` (`List<ClipChange>`), `report.Edits[army + " 클립식 발 사이 ×" + Num(factor)]`
  - `static string FactionPreset.Scale(string oldText, double factor, bool keepDecimals, out string error)`: `keepDecimals`면 `Math.Round(old × factor, 3)`을 `"0.###"`로, 아니면 `BulkRules.Compute("mul", …)`과 같다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`FactionPresetTests.FixtureWith`의 `set`으로 넣는 무기. `vehicles`에는 무기마다 그것을 부르는 독일 차량 하나씩(`entity/-vehicle/germany/tank_heavy/<무기>/<무기>.def` = `"{game_entity\n {weaponry {place {weapon \"<무기 이름>\"}}}\n}\n"`):
- `set/stuff/gun/clip_own` = `"{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 6}\n {recoveryTime 1.5}\n {recoveryTime 5.00}\n {rechargeTime 35}\n}\n"`
- `set/stuff/gun/clip_rpm` = `"{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 3}\n (\"fire rate\" rpm(3))\n (\"reload time\" seconds(18))\n}\n"` 그리고 `set/stuff/gun/.presets` = `"(define \"fire rate\" {recoveryTime (/ 60 %rpm)})\n(define \"reload time\" {rechargeTime %seconds})\n"`
- `set/stuff/gun/clip_pattern` = `"{from \"clip.pattern\"\n {filling \"bulletger_88_l56\" 4}\n {rechargeTime 25}\n}\n"`, `set/stuff/gun/clip.pattern` = `"{weapon\n {recoveryTime 2.5}\n {rechargeTime 5}\n}\n"`
- `set/stuff/gun/autocannon` = `"{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 20}\n {recoveryTime 0.25}\n {rechargeTime 8}\n}\n"`
- `set/stuff/rocket/salvo.weapon` = `"{from \"gun.pattern\"\n {filling \"bulletger_88_l56\" 6}\n {recoveryTime 2}\n {rechargeTime 60}\n}\n"`(gun 폴더 밖)

```csharp
FactionPreset.AddWeaponsAndHealth(project, c, new FactionOptions { Army = "ger" }, report);
var build = new ModBuilder(s.Stack, outDir).Build(project);
T.Eq(0, build.Results.Count(r => r.Status != EditStatus.Applied), "everything applies: " + string.Join("; ", build.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Edit.File + " " + r.Message)));
string own = Out(outDir, "set/stuff/gun/clip_own_ctb_ger");
T.True(own.Contains("{recoveryTime 0.75}") && own.Contains("{recoveryTime 2.5}") && own.Contains("{rechargeTime 17.5}"), "both gap lines and the clip reload are halved with decimals: " + own);
string rpm = Out(outDir, "set/stuff/gun/clip_rpm_ctb_ger");
T.True(rpm.Contains("rpm(6)") && rpm.Contains("seconds(9)"), "the fire-rate rpm doubles (gap ×0.5): " + rpm);
// GapFromPatternAddsLine
string pat = Out(outDir, "set/stuff/gun/clip_pattern_ctb_ger");
T.True(pat.Contains("{recoveryTime 1.25}") && pat.Contains("{rechargeTime 12.5}"), "a gap from the pattern becomes a line in the copy: " + pat);
T.True(!Out(outDir, "set/stuff/gun/autocannon_ctb_ger").Contains("0.125") && Out(outDir, "set/stuff/gun/autocannon_ctb_ger").Contains("{rechargeTime 4}"), "a gap under one second is a fire rate and stays; its reload keeps integer rounding");
T.True(Out(outDir, "set/stuff/rocket/salvo_ctb_ger.weapon").Contains("{recoveryTime 2}"), "outside set/stuff/gun the gap stays");
T.Eq(new[] { "set/stuff/gun/clip_own_ctb_ger", "set/stuff/gun/clip_pattern_ctb_ger", "set/stuff/gun/clip_rpm_ctb_ger" }, report.Clips.Select(x => x.Weapon).OrderBy(x => x).ToArray(), "the report lists the clip guns");
var clipOwn = report.Clips.Single(x => x.Weapon.EndsWith("clip_own_ctb_ger"));
T.True(clipOwn.Filling == 6 && clipOwn.GapOld == "5.00" && clipOwn.GapNew == "2.5" && clipOwn.ReloadOld == "35" && clipOwn.ReloadNew == "17.5", "with old and new values");
int before = project.Edits.Count;
FactionPreset.AddWeaponsAndHealth(project, c, new FactionOptions { Army = "ger" }, report);
T.Eq(before, project.Edits.Count, "running it again adds nothing");
var p2 = new EditProject { Name = "g2" };
FactionPreset.AddWeaponsAndHealth(p2, c, new FactionOptions { Army = "ger", ClipMinGap = 10 }, new FactionReport());
T.True(!p2.Edits.Any(e => (e.Path ?? "").EndsWith("/recoverytime") || (e.New ?? "").StartsWith("{recoveryTime")), "--clip-min-gap raises the bar");
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `FactionClip FAILED`(발 사이가 그대로, `report.Clips`가 없다).

- [ ] **Step 3: 3단계를 고친다**

사본마다 `tracer.Trace(copy)`의 펼친 줄로 클립 여부를 먼저 정한다. 탄창 발수는 `^/[^/]+/filling$` 줄의 둘째 값 가운데 가장 큰 수(없으면 1), 발 사이는 `^/[^/]+/recoverytime$` 줄 가운데 `FromState != Overridden`인 마지막 줄의 값이다. 클립이면 장전 값은 `Scale(…, Reload, keepDecimals: true)`. 발 사이 배수는 `ClipGap > 0 ? ClipGap : Reload`다.
- 사본 자신의 `recoveryTime` 줄(`!v.Synthetic && v.Arg >= 0 && ToVirtual(v.File) == copy`): 줄마다 값 수정.
- 계산된 값(`v.Synthetic`): 사본의 `SourceRows`에서 `^/[^/]+/\("fire rate"\)$` 줄의 `rpm` 인자(`LeafNames.IndexOf("rpm")`)를 `Scale(rpm, 1 / 배수, true)`로. 그 줄이 사본에 없으면 줄 더하기.
- 그 밖(원형·다른 파일): 사본의 첫 블록에 `AddLine` `{recoveryTime <발 사이 × 배수>}`.
같은 자리 수정은 한 번만 넣는다(지금 장전 수정과 같은 중복 검사).

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== FactionClip ok`, `== FactionPreset ok`, `failed=0`.

- [ ] **Step 5: 커밋**

```bash
git add editor
git commit -m "feat(faction): clip guns get the shot gap scaled with the reload"
```

---

### Task 4: 보급 매크로 복제(반경 ×3, 총량 무제한)

**Files:**
- Create: `editor/CtaToybox.Core/FactionPreset.Supply.cs`
- Modify: `editor/CtaToybox.Core/FactionPreset.cs` (`FactionOptions`, `FactionReport`)
- Modify: `editor/CtaToybox.Core/FactionPreset.Build.cs` (`AddWeaponsAndHealth` 다음 `AddSupply`, `Build` 전체를 `try/finally`로 감싸 `c.Stack.Overlay`를 되돌린다)
- Create: `editor/CtaToybox.Core.Tests/FactionSupplyTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (`("FactionSupply", FactionSupplyTests.Run)`)

**Interfaces:**
- Consumes: Task 1·2의 `DefineCopy.New`, `TextCopies.Overlay`, `MacroSwap.Kind`, `MacroSwap.NameOf`, `MacroSwap.Check`
- Produces:
  - `FactionOptions.SupplyRadius` (double, 기본 3), `FactionOptions.SupplyLimit` (string, 기본 `"0"`)
  - `public sealed class SupplyChange { public string Macro; public string Copy; public string RadiusOld; public string RadiusNew; public string LimitOld; public string LimitNew; public List<string> Units = new List<string>(); }`
  - `FactionReport.Supplies` (`List<SupplyChange>`)
  - `public static void FactionPreset.AddSupply(EditProject project, FactionContext c, FactionOptions o, FactionReport report)`
  - 보고 항목: `army + " 보급 매크로 복제"`, `army + " 보급 반경 ×" + Num(o.SupplyRadius)`, `army + " 보급 총량 → " + o.SupplyLimit`, `army + " 보급 호출 → 복제본"`
  - `public const string FactionPreset.SupplyFile = "properties/resupply.inc"`

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`FactionPresetTests.FixtureWith`(Task 3)에 더하는 파일. `properties`와 `vehicles`로 넣는다:
- `properties/resupply.inc` = Task 2와 같은 두 정의 + `"(define \"flag_heavy\"\n {extender \"supply_zone\" {radius 15} {limit 50000}}\n)\n(define \"items\" {x 1})\n"`
- 독일: `entity/-vehicle/germany/car/truck/truck.def`(`("truck_ammo")`), `entity/-vehicle/germany/trailer/cart/cart.def`(`("crate" supply(500))`), `entity/-vehicle/germany/car/eng/eng.def` = `"{game_entity\n (include \"/properties/resupply.inc\")\n (include \"eng.inc\")\n}\n"`와 `entity/-vehicle/germany/car/eng/eng.inc` = `"(\"truck_ammo\")\n"`
- 소련: `entity/-vehicle/ussr/car/truck_r/truck_r.def`(`("truck_ammo")`)
- 건물: `entity/service/-multiplayer/flag_point/flagpoint_ammo/flagpoint_ammo.def` = `"{game_entity\n (include \"/properties/resupply.inc\")\n (\"flag_heavy\")\n}\n"`

```csharp
FactionPreset.AddSupply(project, c, new FactionOptions { Army = "ger" }, report);
T.Eq(new[] { "\"crate_ctb_ger\"", "\"truck_ammo_ctb_ger\"" }, project.Edits.Where(e => e.Kind == DefineCopy.Kind).Select(e => e.New).OrderBy(x => x).ToArray(), "one copy per macro the army calls");
var build = new ModBuilder(s.Stack, outDir).Build(project);
T.Eq(0, build.Results.Count(r => r.Status != EditStatus.Applied), "everything applies: " + string.Join("; ", build.Results.Where(r => r.Status != EditStatus.Applied).Select(r => r.Edit.File + " " + r.Message)));
string inc = Out(outDir, FactionPreset.SupplyFile);
T.True(inc.Contains("(define \"truck_ammo\"\n {extender \"supply_zone\" {radius 20} {limit 3000}}") && inc.Contains("(define \"flag_heavy\"\n {extender \"supply_zone\" {radius 15} {limit 50000}}"), "the originals and the building macro stay: " + inc);
T.True(inc.Contains("(define \"truck_ammo_ctb_ger\"\n {extender \"supply_zone\" {radius 60} {limit 0}}") && inc.Contains("(define \"crate_ctb_ger\"\n {extender \"supply_zone\" {radius 30} {limit 0}}"), "the copies: radius ×3, limit 0 (also over %supply): " + inc);
T.True(Out(outDir, "entity/-vehicle/germany/car/truck/truck.def").Contains("(\"truck_ammo_ctb_ger\")"), "the German truck calls the copy");
T.True(Out(outDir, "entity/-vehicle/germany/trailer/cart/cart.def").Contains("(\"crate_ctb_ger\" supply(500))"), "the cart keeps its argument");
T.Eq(null, Out(outDir, "entity/-vehicle/ussr/car/truck_r/truck_r.def"), "the Soviet truck is untouched");
T.Eq(null, Out(outDir, "entity/service/-multiplayer/flag_point/flagpoint_ammo/flagpoint_ammo.def"), "the building is untouched");
// CallInIncludeIsSkipped
T.Eq(null, Out(outDir, "entity/-vehicle/germany/car/eng/eng.inc"), "a call inside an include is not renamed");
T.True(report.Skipped.Any(x => x.StartsWith("entity/-vehicle/germany/car/eng/eng.def") && x.Contains("유닛 파일 밖")), "and is reported: " + string.Join("; ", report.Skipped));
var truck = report.Supplies.Single(x => x.Macro == "truck_ammo");
T.True(truck.Copy == "truck_ammo_ctb_ger" && truck.RadiusOld == "20" && truck.RadiusNew == "60" && truck.LimitOld == "3000" && truck.LimitNew == "0" && truck.Units.SequenceEqual(new[] { "entity/-vehicle/germany/car/truck/truck.def" }), "the report row");
var none = new EditProject { Name = "none" };
FactionPreset.AddSupply(none, FactionPresetTests.Context(plainSession), new FactionOptions { Army = "ger" }, new FactionReport());
T.Eq(0, none.Edits.Count, "no resupply.inc, nothing to do");   // plainSession: FactionPresetTests.Fixture로 연 세션(보급 파일 없음)
int before = project.Edits.Count;
FactionPreset.AddSupply(project, c, new FactionOptions { Army = "ger" }, report);
T.Eq(before, project.Edits.Count, "running it again adds nothing");
```

`ArmyIndex`는 로스터에 없는 차량을 폴더로 정하므로(`germany`→ger, `ussr`→rus) 로스터 파일은 더하지 않아도 된다. 건물 경로는 군대 유닛이 아니다.

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 오류 `AddSupply`가 없다.

- [ ] **Step 3: `AddSupply`를 만든다**

1. `SupplyFile`의 줄(`SourceRows.Of(SupplyFile, c.Stack.ReadText(SupplyFile))`)에서 `^/\(define "([^"]+)"\)/extender\[supply_zone\]$` 줄의 정의 이름을 모은다(보급 매크로). 파일이 없거나 보급 매크로가 없으면 아무것도 하지 않는다.
2. 군대 유닛 가운데 `entity/-vehicle/`이고 `c.Armies.Shared`에 없는 것마다: 유닛 파일 자신의 줄에서 `row.Head != null`이고 `MacroSwap.NameOf(row.RawPath)`가 보급 매크로인 호출 줄을 찾는다. 유닛 파일을 쓰는 유닛(`c.Index.UsersOf(unit)`)에 다른 군대가 있으면 `"<유닛>: 다른 군대도 이 유닛 파일을 쓴다"`로 건너뛴다. 호출 줄이 없는데 `c.Index.UsersOf(SupplyFile)`에 들어 있고 펼친 줄에 `/extender[supply_zone]`이 있으면 `"<유닛>: 보급 매크로를 유닛 파일 밖(include)에서 부른다"`로 건너뛴다.
3. 부르는 매크로마다 `DefineCopy.New(SupplyFile, SupplyFile, m, m + "_ctb_" + army)`(이미 있으면 그대로).
4. `c.Stack.Overlay = TextCopies.Overlay(project)`로 두고 `SupplyFile`을 다시 읽어, 복제본의 `…/extender[supply_zone]/radius` 줄은 `Scale(값, SupplyRadius, false)`, `…/limit` 줄은 `SupplyLimit`으로 값 수정(`Unit=File=SupplyFile`, `Scope="shared"`, `Arg=0`, 새 값이 원래와 같으면 넣지 않는다).
5. 호출 줄마다 `macro-name` 수정(`Unit=File=유닛`, `Path=row.Path` 소문자, `Occurrence=row.Occurrence`, `Arg=0`, `Old=row.Head.Text`, `New="\"복제본\""`, `Scope="shared"`). `new UnitTracer(c.Stack)`로 펼쳐 `MacroSwap.Check`가 null이 아니면 그 수정을 빼고 이유와 함께 건너뛴다.
6. `report.Supplies`와 보고 항목을 채운다.

`Build`는 시작에서 `c.Stack.Overlay`를 기억하고 끝에서 되돌린다(`try/finally`).

- [ ] **Step 4: 통과를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `== FactionSupply ok`, `failed=0`.

- [ ] **Step 5: 커밋**

```bash
git add editor
git commit -m "feat(faction): army supply units call a copied supply macro (radius x3, unlimited)"
```

---

### Task 5: 강화 내용 문서와 명령줄 인자

**Files:**
- Create: `editor/CtaToybox.Core/FactionDoc.cs`
- Modify: `editor/CtaToybox.Cli/Program.cs` (27행 사용법, 47행 `KnownOptions["faction-preset"]`, 187~222행 `FactionPresetCli`)
- Modify: `editor/CtaToybox.Core.Tests/FactionPresetCliTests.cs` (62·68·69행 호출에 `--doc <임시 파일>`을 더하고, 아래 단언을 더한다)
- Create: `editor/CtaToybox.Core.Tests/FactionDocTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (`("FactionDoc", FactionDocTests.Run)`)

**Interfaces:**
- Consumes: `FactionReport.Clips`, `FactionReport.Supplies`, `FactionOptions`, `EditProject.Rules`, `SourceStack.Layers`(`SourceLayer.Name`)
- Produces:
  - `public static string FactionDoc.Markdown(EditProject project, FactionOptions o, FactionReport report, IReadOnlyList<string> layers, string baseName, string date)`
  - 명령줄 인자: `--clip-min-gap <초, 기본 1>`, `--clip-gap <배수, 기본 --reload>`, `--supply-radius <배수, 기본 3>`, `--supply-limit <0 이상의 정수, 기본 0>`, `--doc <파일, 기본 docs/14-faction-boost/<군대>.md>`

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`FactionDocTests.Run()`은 손으로 만든 `FactionReport`(항목 두 개, `Clips` 하나, `Supplies` 하나, 건너뜀 세 개 중 둘은 같은 이유)로:

```csharp
string md = FactionDoc.Markdown(project, new FactionOptions { Army = "ger" }, report, new[] { "vanilla", "mod_2905667604", "mod_3732639687" }, "월드워-차종", "2026-10-21");
T.True(md.StartsWith("# 강화-독일 수정 내용\n"), "title");
T.True(md.Contains("- 원본 계층: vanilla → mod_2905667604 → mod_3732639687"), "layers in load order");
T.True(md.Contains("재장전 ×0.5") && md.Contains("보급 반경 ×3") && md.Contains("보급 총량 0(무제한)") && md.Contains("클립식 기준: 탄창 2발 이상, 발 사이 1초 이상"), "the options");
T.True(md.Contains("## 항목별 수정") && md.Contains("| ger 보급 반경 ×3 | 2 |"), "item counts as a table");
T.True(md.Contains("## 클립식 포") && md.Contains("| `set/stuff/gun/128oto_ctb_ger` | 3 | 20 → 10 | 18 → 9 |"), "clip table");
T.True(md.Contains("## 보급") && md.Contains("| `truck_ammo_heavy` | `truck_ammo_heavy_ctb_ger` | 20 → 60 | 3000 → 0 | `entity/-vehicle/germany/car/opel_maultier/opel_maultier.def` |"), "supply table");
T.True(md.Contains("## 건너뛴 것") && md.Contains("| 2 |"), "skips grouped by reason");
T.True(md.Contains("## 한계와 미확인") && md.Contains("E19") && md.Contains("E20") && md.Contains("limit 0"), "limits");
T.True(md.TrimEnd().EndsWith("이 문서는 `faction-preset`이 만든다. 손으로 고치지 않는다."), "generated note");
```

`FactionPresetCliTests`에 더할 단언:

```csharp
string doc = Path.Combine(repo, "boost-ger.md");
// 62행 호출에 "--doc", doc
T.True(File.ReadAllText(doc).StartsWith("# 강화-독일 수정 내용"), "faction-preset writes the doc");
T.Eq(2, Cli(out o, "faction-preset", "--settings", settings, "--army", "ger", "--project", projectPath, "--force", "--supply-limit", "-1", "--doc", doc), "a negative supply limit is a usage error");
```

- [ ] **Step 2: 실패를 확인한다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 오류 `FactionDoc`이 없다.

- [ ] **Step 3: 만든다**

문서 꼴(위 단언이 정하는 것 외에는 빌더 몫): 제목, 머리(만든 날·베이스·원본 계층·배수와 기준), `## 항목별 수정`(`report.Edits` 순서대로 `| 항목 | 수정 수 |`, 이어서 일괄 규칙 표 `| 규칙 | 대상 | 키 | 바꾸기 |`), `## 클립식 포`, `## 보급`, `## 건너뛴 것`(이유별 수, 많은 순 30개, "전체는 `--report`"), `## 두 군대가 같이 쓰는 유닛`, `## 한계와 미확인`(E19, E20, `limit 0` 무제한·보급 범위 그림·더한 `recoveryTime`/`rechargeTime`/`health` 줄이 이기는지 미확인), 맺음 줄. 건너뜀의 이유는 `"유닛: 이유"`에서 첫 `": "` 뒤다. 명령줄은 `Path.GetDirectoryName(doc)`을 만들고 쓴다. 사용법 문자열과 `KnownOptions`에 새 인자를 더한다.

- [ ] **Step 4: 통과를 확인한다**

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: `all test files passed`.

- [ ] **Step 5: 커밋**

```bash
git add editor
git commit -m "feat(faction): write the boost contents as a doc; clip and supply options"
```

---

### Task 6: 실제 파일 검사

**Files:**
- Modify: `editor/CtaToybox.Core.Tests/RealFilesCheck.cs` (227~291행 팩션 강화 절)

**Interfaces:**
- Consumes: `FactionReport.Clips`, `FactionReport.Supplies`

- [ ] **Step 1: 검사를 더한다**

```csharp
// 클립식: 지금 판의 다섯 개
var clipNames = boostReport.Clips.Select(x => x.Weapon).OrderBy(x => x).ToArray();
T.Eq(new[] { "set/stuff/gun/128mm_kwk44_auto_ctb_ger", "set/stuff/gun/128mm_l40_auto_ctb_ger", "set/stuff/gun/128oto_ctb_ger", "set/stuff/gun/88mm_flak41_2_ctb_ger", "set/stuff/gun/88mm_kwk43_d_ctb_ger" }, clipNames, "the five German clip guns (2026-10-21)");
foreach (var clip in boostReport.Clips)
{
    double gap = double.Parse(afterTracer.Trace(clip.Weapon).Rows.Last(r => Regex.IsMatch(r.Path, "^/[^/]+/recoverytime$")).Values[0].Text, CultureInfo.InvariantCulture);
    T.Eq(Math.Round(double.Parse(clip.GapOld, CultureInfo.InvariantCulture) * 0.5, 3), gap, "the built gap of " + clip.Weapon + " is half");
}
T.True(boost.Edits.Where(e => (e.Path ?? "").EndsWith("/recoverytime") || (e.Path ?? "").EndsWith("(\"fire rate\")") || (e.New ?? "").StartsWith("{recoveryTime")).All(e => boostReport.Clips.Any(x => x.Weapon == e.File)), "no other weapon's gap changes (machine guns, autocannons)");
// 보급: 독일 보급 유닛은 반경 ×3, 총량 0
foreach (var sc in boostReport.Supplies) foreach (string u in sc.Units)
{
    var rows = afterTracer.Trace(u).Rows;
    T.True(rows.Where(r => r.Path.EndsWith("/extender[supply_zone]/radius")).All(r => r.Values[0].Text == sc.RadiusNew), "radius of " + u);
    T.True(rows.Where(r => r.Path.EndsWith("/extender[supply_zone]/limit")).All(r => r.Values[0].Text == "0"), "limit of " + u);
}
T.True(boostReport.Supplies.SelectMany(x => x.Units).Count() >= 10, "the German supply units are found (11~12 on 2026-10-21)");
T.True(stack.Overlay == null, "FactionPreset.Build leaves the stack overlay as it was");
```

다른 군대 표본(`sample`)에 더한다: 보급 건물 6종(Global Constraints의 경로)과, 다른 군대 유닛 가운데 `stack.ReadText`에 `("supply_`·`("truck_ammo`·`("crate_ammo`가 든 것 전부. 기존 단언 `changed == 0`이 이것까지 본다. 출력 줄에 `faction-clips=` `faction-supply-copies=` `faction-supply-units=`를 더한다.

- [ ] **Step 2: 돌린다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`
Expected: `faction-clips=5`, `faction-other-army-changed=0`, 마지막 줄 `failed=0`.

- [ ] **Step 3: 커밋**

```bash
git add editor/CtaToybox.Core.Tests/RealFilesCheck.cs
git commit -m "test(real): clip guns, German supply units, other armies and buildings unchanged"
```

---

### Task 7: 강화-독일 다시 만들기, 출력·스폰 모드, 문서, PR

**Files:**
- Regenerate: `editor/projects/강화-독일.json`(git 밖), `mod/CTAToybox/**`, `mod/CTAToyboxSpawn*/**`
- Create (생성물): `docs/14-faction-boost/ger.md`
- Modify: `docs/07-stat-editor/README.md` (180~197행 "팩션별 강화 프로젝트": 클립식·보급·문서·새 인자, 명령줄 `add --kind define-copy`와 인자 있는 `macro-name`), `CLAUDE.md` (생성물 목록에 `docs/14-faction-boost/*.md`), `docs/10-backlog.md` (E20 메모: 매크로 복제가 생겼으니 장갑 매크로도 같은 방법으로 할 수 있다), `docs/superpowers/specs/2026-10-21-faction-boost-clip-supply-design.md` (끝에 "구현 결과" 절), `docs/superpowers/handoff-2026-10-21.md` (지금 상태)

- [ ] **Step 1: 게임이 꺼져 있는지 본다**

Run: `pwsh -NoProfile -Command "Get-Process call_to_arms -ErrorAction SilentlyContinue"`
Expected: 출력 없음. 있으면 여기서 멈추고 사용자에게 알린다.

- [ ] **Step 2: 프로젝트와 문서를 다시 만든다**

```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- faction-preset --army ger --base editor/projects/월드워-차종.json --project editor/projects/강화-독일.json --force
```

Expected: 보고에 `ger 클립식 발 사이 ×0.5: 6`(다섯 무기, `128mm_kwk44_auto`는 줄 둘이라 수정 둘, 나머지 넷은 rpm 하나씩), `ger 보급 매크로 복제: 6`(`truck_ammo_heavy`, `truck_ammo_heavy_2x`, `supply_ammo`, `supply_engineer`, `supply_engineer_light`, `supply_fuel`), `ger 보급 호출 → 복제본: 11~12`. 숫자가 다르면 그 차이의 이유를 문서와 PR에 적는다. `docs/14-faction-boost/ger.md`가 생긴다.

- [ ] **Step 3: 출력 모드와 스폰 모드를 빌드한다**

```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- build --project editor/projects/강화-독일.json
```

```bash
dotnet run --project editor/CtaToybox.Cli -c Release -- spawn-menu build
```

Expected: `원본이 바뀜 0건, 대상 없음 0건, 적용 불가 0건, 파일 오류 0건`, 스폰 `오류 0`. `git diff --stat mod/CTAToybox`에 `properties/resupply.inc`, 독일 보급 유닛 `.def`, 클립식 사본 다섯 개가 보인다.

- [ ] **Step 4: 전체 검사**

```bash
pwsh -NoProfile -File tools/tests/Run-Tests.ps1
```

```bash
dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real
```

```bash
dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real-spawn
```

Expected: 모두 `failed=0`.

- [ ] **Step 5: 손으로 고치는 문서를 고친다**

위 Files 목록대로. 설계 문서 "구현 결과"에는 실제 수(클립 무기, 복제 매크로, 바뀐 유닛, 수정 수)와 남은 미확인을 적는다.

- [ ] **Step 6: 커밋하고 PR을 올린다**

```bash
git add editor docs mod CLAUDE.md
git commit -m "build: German boost with clip gaps and supply copies; boost doc"
git push -u origin feat/faction-boost-clip-supply
gh pr create --repo game-mod-project/CTAGOHToybox --base develop --title "Faction boost: clip reload, supply copies, boost doc"
```

PR 본문: 무엇을(클립식·보급·문서), 확인(테스트·`real`·`real-spawn` 숫자), 미확인, (Task 0에서 PR #9 위에 쌓았으면) 그 사실. 끝에 `🤖 Generated with [Claude Code](https://claude.com/claude-code)`. 머지는 사용자가 한다.
