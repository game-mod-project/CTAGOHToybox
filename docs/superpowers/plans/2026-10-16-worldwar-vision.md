# 월드워 360° 시야 Implementation Plan

> **상태(2026-10-17)**: 끝났다(`feat/worldwar-vision`). 차종별 시야는 `2026-10-16-e18-class-vision.md`. 아래 체크 상자는 진행 중에 표시하지 않았다. 무엇을 했는지는 git 기록과 `docs/10-backlog.md` "정리하며 지운 것"에 있다.

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 편집기 프로젝트 "월드워"(WoT식)와 "월드워-워썬더"(비교용)를 만들어 전 지상 유닛의 시야를 360°로 넓히고, 두 방식을 게임에서 확인한다.

**Architecture:** 시야는 `set/vision/vision_fields.inc`의 필드 정의(이름으로 유닛에 걸림)와 `set/vision/vision.set`의 전체 배수로 정해진다. 편집기가 `set/vision`을 게임 설정으로 열게 하고, Core의 `VisionPresets`가 지금 이긴 판(모드 포함)의 그 파일에서 두 변형의 수정 목록을 만든다. 명령줄 `vision-preset`이 그 목록을 프로젝트 파일로 쓰고, 기존 `build`로 출력 모드에 넣은 뒤 AI가 게임을 띄워 확인한다.

**Tech Stack:** C# .NET 8(`editor/CtaToybox.Core`, `CtaToybox.Cli`, 자체 테스트 `CtaToybox.Core.Tests`), PowerShell 7 테스트 묶음(`tools/tests/Run-Tests.ps1`).

**Spec:** `docs/superpowers/specs/2026-10-16-worldwar-vision-design.md`

## Global Constraints

- 브랜치: `develop`에서 `feat/worldwar-vision`을 만들고 끝나면 `git merge --no-ff`로 `develop`에 넣는다. `main`은 건드리지 않는다.
- 게임 폴더(`E:\SteamLibrary\...\Call to Arms - Gates of Hell`)의 파일을 고치지 않는다. 바꾸는 것은 출력 모드(`mod/CTAToybox`)뿐이다.
- 지상 필드 목록(스펙 "두 변형"): `human`, `human_elite`, `human_recon`, `human_peripheral`, `cannon_main`, `cannon_mortar`, `cannon_range_200`, `cannon_range_210`, `cannon_range_220`, `cannon_range_230`, `cannon_range_240`, `cannon_range_250`, `vehicle_main`, `vehicle_top_mgun`, `vehicle_top_turret`, `recon_main`, `recon_open`, `tank_main_mg`, `tank_main-l`, `tank_main-m`, `tank_main-h`, `tank_main-aa`, `tank_driver`, `tank_hull-mg`, `tank_rear-mg`, `tank_remote-mg`, `tank_shielded-mg`, `tank_range_200`, `tank_range_210`, `tank_range_220`, `tank_range_230`, `tank_range_240`, `tank_range_250`, `watchtower` (34개).
- 월드워(WoT식): 위 필드의 `h_fov` → `360`, `uncover` → `50`. 거리 배수는 그대로.
- 월드워-워썬더: 위 필드의 `h_fov` → `360`, `uncover`는 그대로, 위 필드의 `("spotting" …)`·`("spotting2" …)` 호출의 인자 `r` → 지금 값 × 4(`--scale`, 기본 4). `vision.set`은 바꾸지 않는다(전체 배수는 항공·해상·청각·쌍안경까지 바꾼다).
- `v_fov`, 항공(`airborne_*`), 해상(`marine_*`), `soldier_focused`, `searchlight`, `cover-detector`, `animal`, `ultra_vision`은 바꾸지 않는다.
- 수정은 모두 '전부'(shared)다. 게임 설정은 '이 유닛만'이 없다(`SettingFiles.UnitScopeNote`).
- 출력 모드 `CTAToybox`는 `options.set`의 `{mods}` 맨 뒤여야 한다. 게임 안 확인 뒤 `options.set`을 백업으로 되돌린다.
- 키의 뜻을 모르면 `미확인`으로 적는다. 주석의 근거가 있으면 `확인`과 출처 `C`.

## Review Focus

1. 모드 판 `vision_fields.inc`에 필드가 끼어들어 순번이 밀려도 같은 이름의 필드를 고친다 → 수정에 `NamedPath`(`/"human"/h_fov`)를 넣는다. Task 2 테스트 `an inserted field does not shift the target`.
2. 이미 목표 값인 자리(예: `h_fov 360`)는 수정을 만들지 않는다 → Task 2 테스트 `a field already at the target makes no edit`.
3. `uncover`가 없는 필드(`watchtower`)나 파일에 없는 필드는 건너뛰고 안내에 적는다 → Task 2 테스트 `missing keys and fields are noted, not invented`.
4. 사용자가 앱에서 고친 프로젝트 파일을 덮어쓰지 않는다 → Task 2 CLI 테스트 `an existing project is not overwritten without --force`.
5. 한 변형을 빌드한 뒤 다른 변형을 빌드하면 앞 변형의 값이 남지 않는다 → Task 3 Step 4의 빌드 결과 검사.

---

### Task 1: 편집기가 `set/vision`을 게임 설정으로 연다

**Files:**
- Modify: `editor/CtaToybox.Core/SettingFiles.cs` (`Top` 정규식, `Areas`)
- Modify: `editor/CtaToybox.Core/UnitIndex.cs` (`FormatVersion` "8" → "9", 주석에 "9: 시야 파일을 게임 설정으로")
- Modify: `editor/settings/files.psv`, `data/annotations/settings.psv`, `data/annotations/values.psv`, `editor/summary/settings.psv`
- Create: `editor/CtaToybox.Core.Tests/VisionSettingsTests.cs`; Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suite `("VisionSettings", VisionSettingsTests.Run)`를 `SettingsSession` 뒤에)
- Modify: `docs/07-stat-editor/README.md` (게임 설정 목록에 시야 한 줄)

**Interfaces:**
- Produces: `SettingFiles.IsTop("set/vision/vision.set") == true`, `SettingFiles.InArea("set/vision/...") == true`. 앱·명령줄의 유닛 목록에 `set/vision/vision.set`이 종류 `settings`로 나온다.

- [ ] **Step 1: 실패하는 테스트를 쓴다** (`VisionSettingsTests.Run`, 고정물은 `FromInheritanceTests.Fixture`처럼 게임 pak 하나와 저장소 폴더)

고정물의 `gamelogic.pak`:
- `set/vision/vision.set` = `{visions\n (define "radius" (mod "mp" {radius (* 1.0 %r)}) (mod not "mp" {radius (* 1.0 %r)}))\n (include "vision_fields.inc")\n}`
- `set/vision/vision_fields.inc` = `{"human"\n {uncover 20}\n {h_fov 150}\n {v_fov 180}\n}`

저장소의 `data/annotations/settings.psv`에는 아래 Step 3의 `**/h_fov` 줄 하나를 넣는다.

```csharp
T.True(SettingFiles.IsTop("set/vision/vision.set"), "vision.set is a settings top file");
T.True(!SettingFiles.IsTop("set/vision/vision_fields.inc"), "its include is not a top file");
T.True(SettingFiles.InArea("set/vision/spotting.inc"), "the vision folder is a settings area");
T.Eq("settings", session.Index.Units.Single(u => u.Path == "set/vision/vision.set").EditorKind, "listed as game settings");
var view = session.OpenUnit("set/vision/vision.set");
var fov = view.Rows.Single(r => r.Path == "/visions/\"*\"/h_fov");
T.Eq("set/vision/vision_fields.inc", fov.Values[0].Trace.File, "the value is located in vision_fields.inc");
T.True(fov.Values[0].Editable, "and is editable");
T.Eq(SettingFiles.UnitScopeNote, fov.Values[0].UnitScopeBlocker, "shared only");
T.Eq("확인", fov.Note.Grade, "settings.psv annotates vision keys");
```

- [ ] **Step 2: 실패를 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `FAIL vision.set is a settings top file` 등 VisionSettings 실패(그 밖은 통과).

- [ ] **Step 3: 구현한다**

- `SettingFiles.Top`에 `set/vision/vision\.set`을 대안으로, `Areas`에 `"set/vision/"`을 더한다. 머리 주석의 목록에 "시야"를 더한다.
- `UnitIndex.FormatVersion`을 "9"로 올린다(유닛 목록이 바뀐다).
- `editor/settings/files.psv`: `set/vision/vision.set|시야|시야 (필드·발견 거리)`
- `data/annotations/settings.psv`에 머리 `# ---------- 시야 (set/vision/vision.set, vision_fields.inc)`와 다음 줄:
  - `**/uncover|block|확인|이 거리(m) 안의 숨은 병사(덤불 속 등)를 찾는다 (확인: vision_fields.inc 주석 "Will spot hidden soldiers in bushes within 15m")|C`
  - `**/h_fov|block|확인|수평 시야각(도). 360이면 모든 방향을 본다 (확인: vision_fields.inc 주석 "Horizontal field of view arc")|C`
  - `**/v_fov|block|확인|수직 시야각(도). 180이면 바로 위·아래까지 본다 (확인: vision_fields.inc 주석 "Vertical field of view arc")|C`
- `data/annotations/values.psv`에 머리 `# ---------- 시야 필드의 거리 매크로 (set/vision/spotting.inc)`와 다음 줄:
  - `*/("spotting")|r|거리 배수. 대상별 기준 거리(사람 100m, 움직이는 차량 100m+50/r 등)에 곱한다|C`
  - `*/("spotting2")|r|거리 배수(차량·포 대상)|C`
  - `*/("spotting2")|r_human|보병 대상 거리 배수|S`
  - `*/("spotting2")|veh_vis|발견한 차량이 시선이 끊길 때까지 계속 보이는 거리에 더하는 값(m)|C`
  - `*/("spotting2")|can_vis|발견한 포가 계속 보이는 거리에 더하는 값(m)|C`
  - `*/("spotting2")|can_fire|포·차량이 쏘면 드러나는 거리에 더하는 값(m)|C`
  - `*/("glass")|r|거리 배수(조준경·쌍안경)|S`
- `editor/summary/settings.psv`: `**/h_fov|시야각 (도)`, `**/uncover|숨은 병사를 찾는 거리 (m)`
- `docs/07-stat-editor/README.md`의 게임 설정 설명에 "시야(`set/vision/vision.set`과 그 include. 필드의 각도·근접 발견 거리와 거리 매크로 인자 r을 '전부'로 고친다. 전체 배수 `radius`는 식 안의 값이라 화면에서 고치지 못한다)"를 더한다.

- [ ] **Step 4: 통과를 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `passed=N failed=0`.

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`
Expected: `setting-files=`가 23 이상, `setting-wrong-slot=0`, `summary-keys-missing=0`, `failed=0`.

- [ ] **Step 5: 커밋한다**

```bash
git add editor/CtaToybox.Core/SettingFiles.cs editor/CtaToybox.Core/UnitIndex.cs editor/settings/files.psv data/annotations/settings.psv data/annotations/values.psv editor/summary/settings.psv editor/CtaToybox.Core.Tests/VisionSettingsTests.cs editor/CtaToybox.Core.Tests/Program.cs docs/07-stat-editor/README.md
git commit -m "feat(editor): open set/vision as game settings"
```

---

### Task 2: `VisionPresets`와 명령줄 `vision-preset`

**Files:**
- Create: `editor/CtaToybox.Core/VisionPresets.cs`
- Modify: `editor/CtaToybox.Cli/Program.cs` (명령 `vision-preset`, 알려진 옵션, 사용법 글)
- Create: `editor/CtaToybox.Core.Tests/VisionPresetsTests.cs`; Modify: `editor/CtaToybox.Core.Tests/Program.cs` (suite `("VisionPresets", VisionPresetsTests.Run)`)
- Modify: `docs/07-stat-editor/README.md` (명령 목록에 `vision-preset`)

**Interfaces:**
- Consumes: Task 1의 게임 설정 영역(프로젝트의 `Unit`이 `set/vision/vision.set`이므로 앱에서 그 항목에 수정이 붙는다).
- Produces:
  - `public static class VisionPresets`
    - `public const string FieldsFile = "set/vision/vision_fields.inc", VisionSet = "set/vision/vision.set";`
    - `public static readonly string[] GroundFields` (Global Constraints의 34개, 그 순서)
    - `public static EditProject Build(SourceStack stack, string variant, double scale, List<string> notes)`: `variant`는 `"wot"`(이름 `월드워`) 또는 `"wt"`(이름 `월드워-워썬더`). 다른 값이면 `ArgumentException`. `scale`은 `"wt"`의 r 배수다.
  - 명령: `vision-preset --variant wot|wt --project <파일> [--scale <수, 기본 4>] [--force] [--settings <파일>]`. 성공 0, 사용법 오류 2, 덮어쓰기 거절·파일 없음 1.

- [ ] **Step 1: 실패하는 테스트를 쓴다** (`VisionPresetsTests.Run`)

고정물(SourceStack에 바닐라 계층 하나):
- `set/vision/vision_fields.inc` =
  `{"extra" {h_fov 10} ("spotting" r(1.0))}\n{"human" {uncover 20} {h_fov 150} {v_fov 180} ("spotting" r(1.0))}\n{"vehicle_around" {uncover 15} {h_fov 360} ("spotting" r(0.6))}\n{"tank_range_200" {uncover 20} {h_fov 15} ("spotting2" r(1.5) r_human(0.66) veh_vis(50) can_vis(50) can_fire(70))}\n{"watchtower" {h_fov 90} ("spotting" r(1))}\n{"airborne_main" {uncover 0} {h_fov 110} ("spotting" r(3.5))}`
- `set/vision/vision.set` = Task 1 고정물과 같다

```csharp
var notes = new List<string>();
var wot = VisionPresets.Build(stack, "wot", 4, notes);
T.Eq("월드워", wot.Name, "the WoT project name");
var human = wot.Edits.Single(e => e.NamedPath == "/\"human\"/h_fov");
T.Eq("150", human.Old, "an inserted field does not shift the target");
T.Eq("360", human.New, "h_fov becomes 360");
T.Eq("set/vision/vision_fields.inc", human.File, "edits the fields file");
T.Eq("set/vision/vision.set", human.Unit, "under the vision settings item");
T.Eq("shared", human.Scope, "shared");
T.Eq("50", wot.Edits.Single(e => e.NamedPath == "/\"human\"/uncover").New, "uncover becomes 50");
T.True(!wot.Edits.Any(e => e.NamedPath == "/\"vehicle_around\"/h_fov"), "a field already at the target makes no edit");
T.True(!wot.Edits.Any(e => e.NamedPath.Contains("airborne") || e.NamedPath.Contains("extra")), "fields outside the ground list are untouched");
T.True(!wot.Edits.Any(e => e.NamedPath == "/\"watchtower\"/uncover") && notes.Any(n => n.Contains("watchtower") && n.Contains("uncover")), "missing keys and fields are noted, not invented");
T.True(notes.Any(n => n.Contains("human_elite")), "a ground field absent from the file is noted");
T.True(!wot.Edits.Any(e => e.NamedPath.Contains("(\"spotting")), "WoT keeps the distance multipliers");
T.True(wot.Edits.All(e => e.File == VisionPresets.FieldsFile), "vision.set is never edited");

notes.Clear();
var wt = VisionPresets.Build(stack, "wt", 4, notes);
T.Eq("월드워-워썬더", wt.Name, "the War Thunder project name");
T.True(!wt.Edits.Any(e => e.NamedPath.EndsWith("/uncover")), "WT keeps uncover");
var r1 = wt.Edits.Single(e => e.NamedPath == "/\"human\"/(\"spotting\")");
T.Eq(new[] { "1.0", "4" }, new[] { r1.Old, r1.New }, "spotting r x4");
var r2 = wt.Edits.Single(e => e.NamedPath == "/\"tank_range_200\"/(\"spotting2\")");
T.Eq(new[] { "1.5", "6", "0" }, new[] { r2.Old, r2.New, r2.Arg.ToString() }, "spotting2 r x4, the r argument only");
T.True(!wt.Edits.Any(e => e.NamedPath.Contains("airborne") || e.NamedPath.Contains("vehicle_around")), "only the ground list is scaled");
T.True(wt.Edits.All(e => e.File == VisionPresets.FieldsFile), "vision.set is never edited");
T.Throws<ArgumentException>(() => VisionPresets.Build(stack, "bf", 4, notes), "an unknown variant is refused");

// 만든 수정이 그 파일에 그대로 들어간다
foreach (var p in new[] { wot, wt })
{
    var results = new List<EditResult>();
    string patched = TokenPatcher.Apply(stack.ReadText(VisionPresets.FieldsFile), p.Edits, results);
    T.True(results.All(r => r.Status == EditStatus.Applied), p.Name + ": every edit applies");
    T.True(patched.Contains("{\"airborne_main\" {uncover 0} {h_fov 110} (\"spotting\" r(3.5))}"), p.Name + ": the air field is unchanged");
}
```

r의 새 글자는 `(old × scale).ToString("0.###", CultureInfo.InvariantCulture)`다(1.0×4 → `"4"`, 1.5×4 → `"6"`, 0.85×4 → `"3.4"`).

CLI 테스트(같은 파일, `CliApp.Run`으로, 고정물은 `EditorSettings`와 `options.set`을 가진 판):

```csharp
T.Eq(2, Run("vision-preset", "--settings", settings, "--variant", "bf", "--project", p), "an unknown variant is a usage error");
T.Eq(2, Run("vision-preset", "--settings", settings, "--variant", "wt", "--scale", "0", "--project", p), "a scale must be above zero");
T.Eq(0, Run("vision-preset", "--settings", settings, "--variant", "wot", "--project", p), "writes the WoT project");
T.Eq("월드워", EditProject.Load(p).Name, "with its name");
T.Eq(1, Run("vision-preset", "--settings", settings, "--variant", "wt", "--project", p), "an existing project is not overwritten without --force");
T.Eq(0, Run("vision-preset", "--settings", settings, "--variant", "wt", "--scale", "2.5", "--project", p, "--force"), "--force overwrites");
T.True(EditProject.Load(p).Edits.Any(e => e.NamedPath == "/\"human\"/(\"spotting\")" && e.New == "2.5"), "--scale sets the r multiplier");
```

- [ ] **Step 2: 실패를 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 컴파일 오류 `VisionPresets` 없음.

- [ ] **Step 3: 구현한다**

- `VisionPresets.Build`: `stack.ReadText`와 `SourceRows.Of`로 `FieldsFile`의 행을 읽는다. 필드는 원문 경로 `/"<이름>"/h_fov`, `/"<이름>"/uncover`, `/"<이름>"/("spotting")` 또는 `/"<이름>"/("spotting2")`(대소문자 무시)로 찾는다. 수정은 `Path=row.Path`, `Occurrence=row.Occurrence`, `Arg`(h_fov·uncover는 0, 매크로 줄은 `LeafNames`가 `r`인 자리), `Old=` 지금 글자, `NamedPath=row.RawPath`, `NamedOccurrence=row.RawOccurrence`, `Unit=VisionSet`, `Scope="shared"`로 만든다. 목표 값과 같으면 만들지 않는다. 필드나 키가 없으면 `notes`에 "<필드>: <키> 없음"을 더한다.
- 명령 `vision-preset`: `Open`으로 원본 계층을 열고 `Build`를 부른다. 파일이 있고 `--force`가 없으면 "이미 있다. 덮어쓰려면 --force" 출력 후 1. 저장 뒤 "수정 N건(h_fov a, uncover b, r c)"와 `notes`를 출력한다. `--scale`은 0보다 큰 수만 받는다(아니면 사용법 오류 2).
- `docs/07-stat-editor/README.md` 명령 목록에 `vision-preset`의 한 줄 설명과 예를 더한다.

- [ ] **Step 4: 통과를 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `failed=0`.

Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
Expected: `all test files passed`.

- [ ] **Step 5: 커밋한다**

```bash
git add editor/CtaToybox.Core/VisionPresets.cs editor/CtaToybox.Cli/Program.cs editor/CtaToybox.Core.Tests/VisionPresetsTests.cs editor/CtaToybox.Core.Tests/Program.cs docs/07-stat-editor/README.md
git commit -m "feat(editor): vision-preset builds the WorldWar vision projects"
```

---

### Task 3: 두 프로젝트를 만들고 빌드 결과를 본다

**Files:**
- Create: `editor/projects/월드워.json`, `editor/projects/월드워-워썬더.json` (이 두 파일만 커밋한다. `editor/projects/`의 다른 파일은 사용자의 것이라 건드리지 않는다)

- [ ] **Step 1: 프로젝트를 만든다**

Run: `dotnet run --project editor/CtaToybox.Cli -c Release -- vision-preset --variant wot --project editor/projects/월드워.json`
Expected: `수정 67건(h_fov 34, uncover 33, r 0)`, 안내에 `watchtower: uncover 없음`. 숫자가 다르면 안내 줄로 이유를 확인한다(모드 판의 필드가 다를 수 있다).

Run: `dotnet run --project editor/CtaToybox.Cli -c Release -- vision-preset --variant wt --project editor/projects/월드워-워썬더.json`
Expected: `수정 68건(h_fov 34, uncover 0, r 34)`.

- [ ] **Step 2: 워 썬더식을 빌드하고 결과 파일을 본다**

Run: `dotnet run --project editor/CtaToybox.Cli -c Release -- build --project editor/projects/월드워-워썬더.json`
Expected: 빌드 성공. `mod/CTAToybox/resource/set/vision/vision_fields.inc`에 `("spotting" r(4))`와 `("spotting2" r(6)`이 있고 `{uncover 50}`은 없다. `mod/CTAToybox/resource/set/vision/vision.set`은 없다.

- [ ] **Step 3: 월드워를 빌드하고 결과 파일을 본다**

Run: `dotnet run --project editor/CtaToybox.Cli -c Release -- build --project editor/projects/월드워.json`
Expected: `mod/CTAToybox/resource/set/vision/vision_fields.inc`에 `{h_fov 360}`이 이긴 판보다 34개 많고 `{uncover 50}`이 33개다. 앞 빌드의 `r(6)`은 남지 않았다(Review Focus 5).

- [ ] **Step 4: 앱에서 본다** (한 번 실행, 시나리오로)

`--project editor/projects/월드워.json --scenario <시나리오>`. 시나리오: `unit set/vision/vision.set`, `tab summary`, `shot ww-summary`, `tab full`, `filter h_fov`, `shot ww-full`.
Expected: 요약에 시야각 줄이 필드 이름과 함께 보이고, 수정 목록에 67건, 줄의 "수정" 칸에 `→ 360 (전부)`.

- [ ] **Step 5: 커밋한다**

```bash
git add "editor/projects/월드워.json" "editor/projects/월드워-워썬더.json"
git commit -m "feat: WorldWar vision projects (WoT and War Thunder variants)"
```

---

### Task 4: 게임 안 확인 (AI가 게임을 띄워 직접 본다)

메모리 규칙: 주 모니터 맨 앞, 조작은 한 명령에 묶고 결과를 읽을 곳만 찍는다, 항목을 빠뜨리지 않는다, Spawn Menu는 스크롤해 올린 뒤 누른다, `options.set`은 시작 전에 백업하고 끝나면 되돌린다.

**Files:**
- Modify: `docs/07-stat-editor/ingame-check.md` (새 절 "월드워 시야 (2026-10-1x)")
- Modify: `docs/superpowers/specs/2026-10-16-worldwar-vision-design.md` ("확인 결과" 절)

- [ ] **Step 1: 준비**: `options.set` 백업. 출력 모드가 `{mods}` 맨 뒤인지 `sources`로 본다.
- [ ] **Step 2: 시험 배치를 만든다**: 미션 에디터(F2·F3)에서 평지 맵을 열고 아군 전차 1대(기갑 정면이 북쪽)와 아군 보병 분대 1개를 둔다. 적 전차와 적 소총 분대를 정면(북)·옆(동)·뒤(남)에 50 / 100 / 150 / 200m, 워 썬더식 판에는 400 / 600 / 800m를 더해 둔다. 적은 움직이지 않게(멈춘 전차 기준) 둔다. 배치를 저장해 세 판에 같이 쓴다.
- [ ] **Step 3: 세 판을 본다** (판마다 빌드 → 게임 시작 → 배치 열기 → START → 위에서 내려다본 화면 한 장)
  - 바닐라(출력 모드 비움: 스크래치패드의 `{"name": "empty", "edits": []}` 프로젝트를 `build`), 월드워-워썬더, 월드워 순서. 마지막을 월드워로 해서 확인이 끝나면 월드워가 켜진 채로 둔다(AI 판단: 사용자가 원한 시야다).
  - 기록: 방향 × 거리마다 적 전차·보병이 그려지는지(보임/안 보임).
  - 기대(스펙 표, 멈춘 대상): 바닐라 전차는 정면 150m까지 전차, 옆·뒤 50m. 월드워는 모든 방향 150m 전차·99m 보병. 워 썬더식은 모든 방향 600m 전차·약 400m 보병(시선이 이어질 때).
  - 합격 기준(스펙): 세 판 모두 시작할 때 꺼지지 않는다. 월드워의 옆·뒤 멈춘 전차가 150m에서 보인다. 워 썬더식에서 400m 이상의 적이 시선이 이어지면 보인다. 못 넘어도 값은 다시 맞추지 않고 기록한다.
  - 판마다 `game.log`에서 `eSDLReader` 오류를 찾는다. 워 썬더식에서 화면 끊김을 본다.
  - 50m 근접 발견: 월드워 판에서 적 보병 하나를 엎드려 덤불 안 40m에 두고 보이는지 본다.
- [ ] **Step 4: 정리**: `options.set` 되돌림. 결과를 `ingame-check.md` 새 절에 표로 적는다. 스펙의 "위험과 미확인" 가운데 풀린 것(필드가 여럿일 때 긴 거리가 이기는지 등)을 `docs/08` "시야·탐지"와 `settings.psv` 주석에 반영한다.
- [ ] **Step 5: 커밋한다**

```bash
git add docs/07-stat-editor/ingame-check.md docs/superpowers/specs/2026-10-16-worldwar-vision-design.md docs/08-unit-files-check.md data/annotations/settings.psv
git commit -m "docs: WorldWar vision in-game check results"
```

---

### Task 5: 마무리

- [ ] **Step 1**: `docs/10-backlog.md`에 남은 것: 렌더 거리(안개) 조정, 차종별 시야 수치(WoT식 재조정), 사용자 프로젝트와 월드워를 한 빌드에 합치기(빌드는 한 프로젝트만 쓴다). 확인에서 드러난 문제도 더한다.
- [ ] **Step 2**: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`과 `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`. Expected: 모두 통과.
- [ ] **Step 3**: 커밋 후 `git checkout develop` → `git merge --no-ff feat/worldwar-vision`.
