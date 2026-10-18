# 팩션별 강화 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**상태:** 구현 끝(2026-10-17). 결과는 설계 문서 "구현 결과".

**Goal:** 월드워-차종을 베이스로 군대(ger·rus·usa·eng·fin·jap)마다 강화 프로젝트를 만든다. 그 군대 유닛만 강해지고 다른 군대는 지금 판 그대로다. 컨퀘스트·스커미시의 플레이어 몫 자원도 최대로 한다.

**Architecture:** 편집기 코어에 세 가지를 더한다. (1) 군대 소속 색인(`ArmyIndex`) — 로스터의 `side()`와 파일 이름으로 유닛을 군대에 묶는다. (2) 파일 사본 수정(`Kind "file-copy"`) — `SourceStack`이 사본 경로를 원본 내용으로 보이게 해서 기존의 펼치기·고치기·빌드가 그대로 돈다. (3) 줄 더하기 수정(`Kind "add-line"`) — 블록의 닫는 괄호 앞에 줄 하나를 넣는다. 그 위에 `FactionPreset`이 수정 목록을 만들고, 명령줄 `faction-preset`이 프로젝트 파일을 쓴다.

**Tech Stack:** C# .NET 8, 기존 `CtaToybox.Core`·`Cli`·`App`, 자체 테스트(`T.Eq`, `Suites.All`).

**Spec:** `docs/superpowers/specs/2026-10-17-faction-boost-design.md`

## Global Constraints

- 게임 폴더·창작마당 폴더는 읽기만 한다. 쓰는 곳은 출력 모드와 `editor/projects/`뿐이다.
- 게임 안 확인을 하지도 제안하지도 않는다. 게임으로만 알 수 있는 것은 문서에 `미확인`.
- 팩션 = 군대 6개(`set/multiplayer/armies/*.set`: ger, rus, usa, eng, fin, jap). 군대의 `side()` 묶음은 그 군대 로스터 파일(`inf_<군대>*.set`, `units_<군대>*.set`)에 가장 많이 나오는 군대로 정한다(eng 묶음: eng, can, pol, fra, aus, nzl).
- 두 군대가 같이 쓰는 유닛(로스터 두 군대에 등록)은 건너뛰고 목록으로 보인다.
- 배수: 인벤토리 개수 ×2(탄약 ×5, 개수와 편차를 같이), 재장전 시간 ×0.5, 보병 체력 ×2, 장갑 차량 ×2·경 ×2·중형 ×2.5·중 ×3·초중 ×5(파생형 포함), 속도 ×1.5, 연료 `fuel`·`range` ×10.
- 컨퀘스트 `resources_{low,standard,high,very_high}.set`: `Resources/{MP,AP}/StartVal` 99999, `{SP,RP}` 999, `StageCP` 다섯 값 9999, `GlobalMaxCP` 9999, `{MP,AP}` `WinGain`·`LoseGain` `"1:99999"`, `{SP,RP}` 999. `Bots`는 그대로.
- 스커미시: 배틀존·탄약전 MP 시작·끝 99999, CP 9999, 봇 `balancingFactorBonus`의 `mpStart`·`mpFinish`·`cp`를 `원래 배율 × 원래 값 / 새 값`. DP 그대로. 피난(라스트스탠드)은 팀 a만. 프런트라인은 바꾸지 않는다.
- 사본 이름: 원래 파일 이름 + `_ctb_<군대>`(같은 폴더).
- 새 수정 종류는 예전 프로젝트 파일을 그대로 연다(새 필드는 null이면 쓰지 않는다).

## Review Focus

1. **다른 군대가 바뀌는 것**: 독일 빌드 뒤 rus·usa·eng·fin·jap 유닛과 독일과 무기를 같이 쓰는 유닛의 펼친 값이 빌드 전과 같아야 한다 → Task 7 실제 검사.
2. **사본 이름의 이름 해석 충돌**: 사본 `k98_ctb_ger`가 생겨도 `"k98"`은 원래 `k98`로 풀려야 한다 → Task 2 테스트.
3. **이미 사본·줄이 있는 프로젝트에 프리셋을 다시 돌리는 것**: 같은 사본·줄이 두 번 생기지 않아야 한다 → Task 6 테스트(같은 프로젝트에 두 번 → 수정 수 같음).
4. **줄 더하기 대상 블록이 원본에서 사라진 경우**: 점검·빌드가 `TargetMissing`으로 막고 다른 수정은 적용된다 → Task 3 테스트.
5. **개수가 소수인 인벤토리**(`22.5 2.5`): 소수는 소수로 남는다(F1 `Compute`) → Task 4 테스트.

---

### Task 1: 군대 소속 색인

**Files:** Create `editor/CtaToybox.Core/ArmyIndex.cs`; Modify `BulkRules.cs`(`BulkRule.Army`, `IsTarget`), `EditorSession.cs`(`Armies` 속성, 지연 생성), `CtaToybox.Cli/Program.cs`(`rule --army`, `units --army`); Test `editor/CtaToybox.Core.Tests/ArmyIndexTests.cs`.

**Interfaces — Produces:**
- `public sealed class ArmyIndex { public static readonly string[] Armies = { "ger","rus","usa","eng","fin","jap" }; public static ArmyIndex Build(SourceStack stack, IEnumerable<UnitEntry> units); public string ArmyOf(string unitPath); /* null이면 어느 군대도 아니거나 공유 */ public IReadOnlyList<string> Members(string army); public IReadOnlyList<string> Shared { get; } /* "유닛: ger, fin" */ public IReadOnlyList<string> SidesOf(string army); }`
- 규칙: 보병 = `set/breed/mp/<side>/**`, `set/breed/single/<side>/**`에서 side가 그 군대 묶음인 파일. 차량 = 그 군대 로스터 파일의 항목 이름 → 엔티티 `.def`(UnitIndex의 파일 이름으로). 로스터에 없는 차량은 폴더(`germany|+germany`→ger, `ussr|+ussr`→rus, `usa|+usa`→usa, `eng`→eng, `finland|+finland`→fin, `+japan`→jap). 둘 이상의 군대 로스터에 등록된 유닛은 `Shared`로, `ArmyOf`는 null.
- `BulkRule.Army`(string, null이면 거르지 않음). `BulkRules.Preview`에 `Func<string,string> armyOf` 인자를 더하고(null 허용) `IsTarget`이 `rule.Army != null`이면 `armyOf(u.Path) == rule.Army`도 본다. `BulkRules.Check`는 `Army`가 목록 밖이면 거절.

- [ ] Step 1: 테스트 `ArmyIndexTests.Run()` — 임시 저장소 픽스처(F1 `BulkRulesTests.Fixture` 꼴): `set/breed/mp/ger/mid/a.set`, `mp/rus/mid/b.set`, `mp/can/mid/c.set`, `set/multiplayer/units/conquest/inf_ger.set`(`side(ger)`), `inf_eng.set`(`side(eng)`, `side(can)`), `units_ger.set`·`units_fin.set` 둘 다에 `"shared_car"`, `entity/-vehicle/germany/car/shared_car/shared_car.def`, `entity/-vehicle/germany/car/solo/solo.def`(로스터 없음). 단언: `ArmyOf(a)=="ger"`, `ArmyOf(c)=="eng"`, `ArmyOf(solo)=="ger"`, `ArmyOf(shared_car)==null`이고 `Shared`에 `shared_car`, 규칙 `Army="ger"` 미리 보기 대상이 ger 유닛뿐.
- [ ] Step 2: 실패 확인 `dotnet run --project editor/CtaToybox.Core.Tests -c Release` → 컴파일 오류.
- [ ] Step 3: 구현(위 Interfaces). 로스터 파일은 `RosterLinks.IsRosterFile`과 같은 범위를 읽고, `side(x)`는 정규식으로 모은다.
- [ ] Step 4: 전체 테스트 통과.
- [ ] Step 5: Commit `feat(editor): army index and rule army filter`.

### Task 2: 파일 사본 수정

**Files:** Create `editor/CtaToybox.Core/FileCopy.cs`; Modify `SourceStack.cs`(사본 등록), `EditorSession.cs`·`EditorSession.Edits.cs`(프로젝트를 열 때·사본을 더할 때 등록, 점검), `ModBuilder.cs`(사본 파일을 수정이 없어도 쓴다), `ItemLinks`를 만드는 곳(사본 경로 포함), `Cli/Program.cs`(`add --kind file-copy --file <원본> --new <새 가상 경로>`); Test `FileCopyTests.cs`.

**Interfaces — Produces:**
- `Edit.Kind == FileCopy.Kind ("file-copy")`, `File` 원본 가상 경로, `New` 새 가상 경로(따옴표 없음), `Path=""`, `Scope="shared"`.
- `public static class FileCopy { public const string Kind = "file-copy"; public static string CheckTarget(SourceStack stack, string source, string target); /* 원본 없음, 대상이 이미 있음, 폴더가 다름, 이름 해석 충돌이면 이유 */ public static string NameFor(string source, string army); /* "set/stuff/rifle/k98" → "set/stuff/rifle/k98_ctb_ger" (확장자 앞에 붙인다) */ }`
- `SourceStack.SetCopies(IEnumerable<KeyValuePair<string,string>> targetToSource)`: `Find(target)`가 원본 `SourceFile`의 복제본(`VirtualPath=target`, `Layer="project"`)을 돌려준다. `Paths`에 사본 경로가 들어간다.
- 사본 파일을 대상으로 한 다른 수정(`File = 사본 경로`)은 일반 수정처럼 펼치고 점검·빌드된다.

- [ ] Step 1: 테스트 `FileCopyTests.Run()` — 픽스처 `set/stuff/rifle/k98`(`{from "rifle.pattern"}`), `set/stuff/rifle/rifle.pattern`(`{rechargeTime 6}`). 단언: `NameFor`, 사본을 더하면 `stack.Find("set/stuff/rifle/k98_ctb_ger")`가 원본 내용, `ItemLinks.Find("k98")`는 여전히 `k98`, `Find("k98_ctb_ger")`는 사본, 사본 파일의 값 수정이 빌드에서 사본에 적용되고 원본 `k98`은 출력에 없음, 사본만 있고 수정이 없어도 출력에 사본 파일이 쓰임, 이미 있는 경로로 사본 → `CheckTarget` 거절, 사본 수정을 끄면 사본 파일을 가리키는 수정이 `TargetMissing`.
- [ ] Step 2: 실패 확인.
- [ ] Step 3: 구현.
- [ ] Step 4: 통과.
- [ ] Step 5: Commit `feat(editor): file-copy edits`.

### Task 3: 줄 더하기 수정

**Files:** Create `editor/CtaToybox.Core/AddLine.cs`; Modify `ModBuilder.cs`(블록 복제 다음, 값 고치기 전에 넣는다), `EditorSession.Edits.cs`(점검, `AddLine` 메서드), `Cli/Program.cs`(`add --kind add-line --file --path <블록 경로> --occ --new "<줄>"`); Test `AddLineTests.cs`.

**Interfaces — Produces:**
- `Edit.Kind == AddLine.Kind ("add-line")`, `File`, `Path` 블록의 키 경로(KeyIndex 꼴, 예 `/perks`), `Occurrence`, `New` 넣을 줄 원문(괄호 포함 한 줄), `Old = null`, `Scope="shared"`.
- `public static class AddLine { public const string Kind = "add-line"; public static string CheckLine(string line); /* 괄호 짝이 맞고 파싱되는 한 덩이 */ public static string Apply(string text, IEnumerable<Edit> adds, List<EditResult> results); public static IReadOnlyList<BlockSpan> Blocks(string text); }`, `public sealed class BlockSpan { public string Path; public int Occurrence; public int Open; public int Close; }`(Open·Close는 본문의 글자 자리).
- 넣는 자리: 블록의 닫는 괄호 바로 앞, 같은 블록에 여럿이면 수정 순서대로. 블록이 없으면 `TargetMissing`. 같은 줄이 이미 블록 안에 있으면 `Applied`이고 다시 넣지 않는다.

- [ ] Step 1: 테스트 — `{perks (include "ability.inc") ("tier3_standard")}`에 `{"health" "600%"}` 넣기 → 결과가 파싱되고 줄이 닫는 괄호 앞, 블록 없음 → `TargetMissing`이고 같은 파일의 값 수정은 적용, 깨진 줄(`{"health"`) → `CheckLine` 거절, 같은 줄 두 번 → 한 번만, 빌드 결과 파일에 줄이 있다.
- [ ] Step 2: 실패 확인. Step 3: 구현. Step 4: 통과.
- [ ] Step 5: Commit `feat(editor): add-line edits`.

### Task 4: 프리셋 — 인벤토리·장갑·속도·연료

**Files:** Create `editor/CtaToybox.Core/FactionPreset.cs`, `editor/presets/super-heavy.txt`(스펙의 후보 목록, 가상 경로 한 줄에 하나, `#` 주석); Test `FactionPresetTests.cs`.

**Interfaces — Consumes:** `ArmyIndex`(Task 1), `BulkRules.Preview`/`Compute`(F1), `ItemLinks.Find`.
**Produces:**
- `public sealed class FactionOptions { public string Army; public double Inventory = 2, Ammo = 5, Reload = 0.5, Health = 2, ArmorCar = 2, ArmorLight = 2, ArmorMedium = 2.5, ArmorHeavy = 3, ArmorSuper = 5, Speed = 1.5, Fuel = 10; public List<string> SuperHeavy; }`
- `public sealed class FactionReport { public int Units, Infantry, Vehicles; public Dictionary<string,int> Edits; /* 항목 → 수정 수 */ public List<string> Skipped; public List<string> Shared; }`
- `public static class FactionPreset { public static void AddUnitStats(EditProject project, SourceStack stack, UnitIndex index, UnitTracer tracer, ArmyIndex armies, FactionOptions o, FactionReport report); public static string ClassOf(string unitPath, ICollection<string> superHeavy); /* car|light|medium|heavy|super|null */ public static bool IsAmmo(ItemLinks links, string itemName); /* 풀린 파일 확장자가 .ammo */ }`
- 인벤토리: 군대 유닛의 `{item "이름" N [R]}` 줄(보병 `/inventory/item`, 차량 `/extender/box/item` 등 `**/item`)에서 개수 인자 1과 편차 인자 2를 `IsAmmo`면 ×5, 아니면 ×2. 개수 없는 줄·`filled` 줄은 건너뜀. 범위는 F1 `auto`(파일을 쓰는 유닛이 모두 이 군대면 '전부', 아니면 '이 유닛만').
- 장갑: `ClassOf`가 null이 아닌 차량의 `**/thickness` 줄 모든 수 값, `("tracks"|"gun" thickness())`, `("car_thickness" body() engine())` 인자. `internals` 매크로 본문은 건너뛰어진다(F1 막힘 그대로).
- 속도: `**/locomotion/maxspeed`가 아니라 원래 인자(`mobility_*`의 `speed`)를 고친다(F1 규칙이 이미 인자 자리로 간다). 연료: `fuel`·`range` 인자, `**/fuelbag/volume`.
- 모두 F1 규칙(`BulkRule`, `Army`, 경로 글자로 차종 폴더)으로 만들어 `project.Rules`에 넣는다(사용자가 앱에서 규칙으로 보고 다시 적용할 수 있게). 초중전차는 규칙 대상 경로 목록, 중전차 규칙은 초중 목록을 뺀다(`BulkRule`에 `Exclude` 경로 목록을 더한다).

- [ ] Step 1: 테스트 — 픽스처(독일 보병 1, 소련 보병 1, 독일 경·중·초중 전차 각 1, 독일 차량 1, 탄약 `set/stuff/rifle/rifle_ger.clip.ammo`, 포탄 `set/stuff/shell/bulletger_88_l56.he.ammo`, 도구 `set/stuff/melee/shovel_ger`). 단언: 독일 보병 `{item "rifle_ger clip ammo" 60 5}` → `300 25`, `{item "shovel_ger"}` 그대로, `{item "bandage_ger" 22.5 2.5}` → `45 5`, 전차 box `{item "bulletger_88_l56 he" 46}` → `230`, 경 `thickness 30` → 60, 중형 → 75, 중 → 90, 초중 → 150, `speed(38)` → 57, `fuel(540)` → 5400·`range(210)` → 2100, 소련 보병 값 그대로, `ClassOf`.
- [ ] Step 2~4: 실패 확인, 구현, 통과.
- [ ] Step 5: Commit `feat(editor): faction preset unit stats`.

### Task 5: 프리셋 — 재장전·체력

**Files:** Modify `FactionPreset.cs`; Test `FactionPresetTests.cs`에 더함.

**Interfaces — Produces:** `public static void AddWeaponsAndHealth(EditProject project, SourceStack stack, UnitIndex index, UnitTracer tracer, ArmyIndex armies, ItemLinks links, FactionOptions o, FactionReport report);`
- 재장전: 군대 유닛이 부르는 무기 이름(`**/weapon` 값, `{item "x" filled}`, `filling` 무기)을 `ItemLinks`로 풀고, 펼쳤을 때 `rechargeTime`이 있는 파일마다 사본 하나(`FileCopy.NameFor`). 사본의 `rechargeTime` 값마다: 사본 자신의 줄이면 값 수정 ×0.5, 원형에서 물려받으면 E14 덧붙이기(값 수정, 빌드가 맨 위 블록에 붙인다), 매크로 본문이면 그 호출이 있는 블록에 `AddLine` `{rechargeTime <×0.5>}`. 그 뒤 군대 유닛의 이름 값을 사본 이름으로 바꾸는 수정(문자열, 계열 이름이면 사본의 정확한 이름). 범위 `auto`.
- 체력: 군대 보병 파일마다 펼친 perks의 `"health"` 마지막 값(`"300%"`) ×2를 `AddLine`으로 그 파일 `/perks` 블록에 `{"health" "600%"}`. perks나 health가 없으면 `Skipped`.
- 무기를 다른 군대만 쓰면 사본을 만들지 않는다. 같은 무기는 군대당 사본 하나.

- [ ] Step 1: 테스트 — 픽스처에 `set/stuff/rifle/k98`(`{from "rifle.pattern"}`), `rifle/rifle.pattern`(`("reload time" seconds(6))` → `{rechargeTime 6}`), `set/stuff/gun/88mm_kwk36`(자기 줄 8), `mgun/.presets`의 `reload_belt` 매크로와 그것을 부르는 `mgun/mg34`. 독일 보병이 `k98`·`mg34`, 소련 보병도 `k98`, 독일 전차가 `88mm_kwk36`. 단언: 사본 세 개, 사본 펼침 `rechargeTime` 3·4·4.25, 독일 보병 `{item "k98_ctb_ger" filled}`, 소련 보병은 `"k98"`, 원본 `k98` 출력 없음, 독일 보병 perks에 `{"health" "600%"}`, 소련 보병 perks 그대로.
- [ ] Step 2~4. Step 5: Commit `feat(editor): faction preset reload and health`.

### Task 6: 프리셋 — 컨퀘스트·스커미시, 명령줄

**Files:** Modify `FactionPreset.cs`(`AddGameSettings`), `Cli/Program.cs`(`faction-preset`); Test `FactionPresetTests.cs`, `FactionPresetCliTests.cs`.

**Interfaces — Produces:**
- `public static void AddGameSettings(EditProject project, SourceStack stack, FactionReport report);` Global Constraints의 컨퀘스트·스커미시 값. 키가 없는 파일은 `Skipped`. 봇 배율 = 지금 이긴 판의 배율 × 원래 값 / 새 값(소수 셋째 자리, `BulkRules.Compute`의 꼴).
- `public static EditProject Build(SourceStack stack, UnitIndex index, UnitTracer tracer, EditProject baseProject, FactionOptions o, FactionReport report);` 베이스 수정을 `MergeFrom`으로 가져오고 위 세 메서드를 차례로.
- 명령줄: `faction-preset --army ger --base editor/projects/월드워-차종.json --project editor/projects/강화-독일.json [--force] [--inventory 2 --ammo 5 --reload 0.5 --health 2 --armor-car 2 --armor-light 2 --armor-medium 2.5 --armor-heavy 3 --armor-super 5 --speed 1.5 --fuel 10]`. 보고: 군대 유닛 수, 항목별 수정 수, 건너뜀·공유 상위 20. 있는 파일은 `--force`로만 덮어쓴다.

- [ ] Step 1: 테스트 — 픽스처 `resources_standard.set`(Resources·Bots·StageCP·GlobalMaxCP), `presets/preset_late.inc`(standard start 200 finish 4500 doctrine 200 cp), `bots` 배율. 단언: StartVal 99999·999, StageCP 9999×5, Bots 그대로, doctrine 그대로, 봇 mpStart 배율 = 1.10×200/99999을 반올림한 값. 명령줄: 프로젝트 파일이 생기고 베이스 수정 수 + 강화 수정, 같은 명령 두 번(`--force`)이면 수정 수 같음(Review Focus 3), 없는 군대 → 종료 코드 2.
- [ ] Step 2~4. Step 5: Commit `feat(cli): faction-preset`.

### Task 7: 앱, 실제 파일 검사, 문서, 병합

**Files:** Modify `editor/CtaToybox.App/EditDialog.xaml(.cs)`(문자열 값이 아이템·무기 이름이면 "이 파일 사본 만들기", 블록 줄이면 "이 블록에 줄 더하기"), `MainWindow.Edits.cs`(사본·줄 수정의 제목), `MainWindow.Scenario.cs`(`filecopy <새 이름>`, `addline <줄>`); `editor/CtaToybox.Core.Tests/RealFilesCheck.cs`; Create `docs/12-faction-files.md`; Modify `docs/07-stat-editor/README.md`, `CLAUDE.md`(명령·손으로 고치는 파일 `editor/presets/super-heavy.txt`, `docs/12`), `docs/10-backlog.md`, 스펙 "구현 결과", `docs/superpowers/handoff-2026-10-17.md`.

- 실제 검사: ger 프리셋을 실제 판에서 만들어 임시 폴더에 빌드. 찍기: `faction-units= faction-edits= faction-copies= faction-lines= faction-skipped= faction-shared= faction-not-applied= faction-other-army-changed=`. 단언: 적용 불가 0, 파일 오류 0, 다른 군대 표본(군대마다 보병 20·차량 20, 그리고 ger와 무기 이름을 같이 쓰는 다른 군대 유닛 전부)을 빌드 출력 폴더를 맨 뒤 계층으로 붙인 스택에서 펼친 값이 빌드 전과 같다(0건), 월드워-차종 수정 601건이 들어 있다.
- 프로젝트 6개를 만든다: `editor/projects/강화-{독일,소련,미국,영국,핀란드,일본}.json`(명령줄로). 출력 모드에는 빌드하지 않는다(지금 출력 모드는 월드워-차종 그대로. 빌드는 사용자에게 묻는다).
- [ ] 앱 빌드 경고 0 → 시나리오 한 번으로 사본·줄 더하기 창과 수정 목록을 찍어 고칠 것을 모아 고친다.
- [ ] 전체 테스트 + `-- real` → Commit `feat: faction boost projects` → `develop`에 `--no-ff` 병합.
