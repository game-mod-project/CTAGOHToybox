# E18 차종별 시야·탐지 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 게임 설정 파일의 따옴표 이름 블록을 복제하는 수정 종류(블록 복제)를 만들고, 그것으로 `vision-preset --classes`가 차종별 시야 필드 셋과 유닛별 필드 바꾸기를 담은 월드워-차종 프로젝트를 만든다.

**Architecture:** 블록 복제는 `Edit.Kind = "block-copy"`다. `BlockCopy`가 원문 텍스트에 복제 블록을 넣는다(같은 부모의 마지막 형제 뒤). 편집기는 `SourceStack`의 텍스트 덧씌우기(overlay)로 켜진 복제를 반영한 본문을 보이고, 점검·빌드·명령줄은 원본을 읽은 뒤 같은 `BlockCopy.Apply`로 본문을 만든다. 차종 프리셋은 `VisionPresets`에 더한다.

**Tech Stack:** C# .NET 8, 자체 테스트(`T.Eq`, `editor/CtaToybox.Core.Tests`), WPF 앱.

**Spec:** `docs/superpowers/specs/2026-10-16-e18-class-vision-design.md`

## Global Constraints

- 숫자 기본값: 중전차 시야 r 1.5(바꾸지 않음)·탐지 `uncover` 100, 중형 r 1.65·115, 경전차 r 1.8·130, 차량 r 1.8·145.
- 새 필드 이름: `ctb_tank_medium`, `ctb_tank_light`, `ctb_vehicle`. 원본은 `tank_range_200`. 복제 뒤 바꾸는 값은 `h_fov` 360, `uncover`, `("spotting2" r(…))`의 r뿐이다.
- 차종은 경로 `entity/-vehicle/<진영>/tank_light|tank_medium/`로만 정한다. `_x.def`, `_xx.def`는 건너뛴다. 차량은 `properties/car.ext`·`armoredcar.ext`·`moto.ext`의 `around` visor를 '전부'로 바꾼다.
- 기존 `editor/projects/월드워.json`은 덮어쓰지 않는다. 새 프로젝트는 `editor/projects/월드워-차종.json`.
- 게임 안 확인은 하지 않는다.
- 저장소 규칙: 작업 브랜치는 develop에서, `git merge --no-ff`로 develop에 병합한다. 커밋 끝에 `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.
- 블록 복제는 원본 블록 바로 뒤가 아니라 **같은 부모 블록의 마지막 형제 뒤**에 넣는다(스펙 고침, AI 판단). 같은 경로의 기존 줄 순번(Occurrence)이 밀리지 않게 하려는 것이다. 스펙도 같이 고친다.

## Review Focus

1. 복제를 끈 뒤 그 이름을 쓰는 수정(새 블록 안 값, `{vision "ctb_…"}`로 바꾸는 유닛 수정)이 켜져 있으면 점검·빌드가 "적용 불가: 필드 …가 없다"로 막는다 → Task 2 테스트.
2. 이미 있는 이름이나 같은 프로젝트의 다른 복제 이름으로 복제하면 막는다(대소문자 무시) → Task 1 테스트.
3. 복제 블록 안의 값을 고친 뒤 프로젝트를 다시 열어도(Reanchor) 그 수정이 같은 자리를 가리킨다 → Task 3 테스트.
4. 원본 블록 이름이 원본에서 바뀌거나 사라지면 복제는 대상 없음/원본이 바뀜이 되고 의존 수정도 막힌다 → Task 2 테스트.
5. 복제 블록이 들어간 출력 파일이 다시 파싱되고 기존 67건의 월드워 수정 순번이 그대로다 → Task 2 테스트(기존 필드 수정 하나를 함께 빌드).

---

### Task 1: `BlockCopy` 텍스트 규칙

**Files:**
- Create: `editor/CtaToybox.Core/BlockCopy.cs`
- Modify: `editor/CtaToybox.Core/Edit.cs`(Kind 주석에 block-copy)
- Test: `editor/CtaToybox.Core.Tests/BlockCopyTests.cs`, `Program.cs`에 등록

**Interfaces:**
- Produces:
  - `public const string BlockCopy.Kind = "block-copy"`
  - `string BlockCopy.CheckName(string newToken)` — 맞으면 null. 따옴표로 감싼 이름, 안에 빈칸·`"`·`/` 없음, 빈 이름 아님.
  - `string BlockCopy.Apply(string text, IEnumerable<Edit> copies, List<EditResult> results)` — 켜진 복제를 차례로 넣은 본문. 복제마다 결과 하나(Applied·TargetMissing·SourceChanged·Invalid). 원문이 파싱되지 않으면 모두 Invalid, 본문 그대로.
  - `string BlockCopy.ApplyAll(string text, IEnumerable<Edit> copies)` — 결과 없이 본문만(편집기 덧씌우기·명령줄용).
  - `string BlockCopy.Dependency(Edit e, IEnumerable<Edit> copies)` — e가 기대는 복제 이름(따옴표 없이). `e.NamedPath`에 `/"이름"` 조각이 있거나 값 수정(`Kind == null`)의 `New`가 `"이름"`이면 그 이름. 대소문자 무시. 아니면 null.
- 복제 수정의 꼴: `File` 설정 파일, `Path`·`Occurrence`는 원본 블록 이름 줄(`/"*"` 꼴), `NamedPath`·`NamedOccurrence`는 그 원문 경로(`/"tank_range_200"`), `Arg` 0, `Old` `"tank_range_200"`, `New` `"ctb_tank_light"`, `Scope` shared.

- [ ] **Step 1: Write the failing test** `BlockCopyTests.Run`:
  - 원문 `{"a"\n {uncover 20}\n ("spotting2" r(1.5))\n}\n{"b"\n {uncover 10}\n}` 에 `a`→`c` 복제: 결과 본문을 `SourceRows.Of("set/vision/vision_fields.inc", …)`로 읽으면 `RawPath == "/\"c\"/uncover"` 줄 값 `20`, `/"c"/("spotting2")` 줄 r `1.5`; `/"b"` 줄이 `/"c"` 줄보다 앞(마지막 형제 뒤); `/"*"/uncover`의 Occurrence 0·1(a·b)이 그대로; 결과 Applied 하나.
  - 부모 안의 블록: `{visions {"a" {x 1}} {"b" {x 2}}}`에서 `a` 복제는 `visions` 닫는 괄호 앞에 들어간다(`/visions/"c"/x` 값 1).
  - 이름 `"b"`·`"B"`로 복제 → Invalid("이미 있는 이름"). 같은 목록의 두 복제가 둘 다 `"c"` → 둘째 Invalid.
  - 원본 이름 `"zz"`(없음) → TargetMissing. `Old`가 `"a"`인데 그 자리 이름이 `"x"`(Path·NamedPath로 찾은 줄의 이름이 다름) → SourceChanged.
  - `CheckName`: `"ok_1"` null, `ok`(따옴표 없음)·`"a b"`·`"a/b"`·`""` 오류.
  - `Dependency`: NamedPath `/"c"/uncover` → `c`; New `"c"`(Kind null) → `c`; New `"c"`이지만 Kind macro-name → null; 관계없는 수정 → null.
  - 꺼진 복제(`Enabled=false`)는 `Apply`가 무시한다.
- [ ] **Step 2: Run** `dotnet run --project editor/CtaToybox.Core.Tests -c Release` → BlockCopy 컴파일 실패.
- [ ] **Step 3: Implement** `BlockCopy.cs`. 블록 범위는 토큰 목록(`SdlCore.Parse(...).Tokens`)에서 이름 토큰 앞의 `{`부터 짝 `}`까지. 넣을 곳은 그 `{`를 감싼 부모 블록의 닫는 `}` 앞(맨 위면 텍스트 끝). 넣는 글자는 줄바꿈(원문의 줄바꿈 꼴) + 원본 블록 원문에서 이름 토큰만 새 이름으로 바꾼 것. 여러 복제는 차례로 적용(앞 복제가 만든 본문에서 다음을 찾는다).
- [ ] **Step 4: Run** 테스트 → 통과.
- [ ] **Step 5: Commit** `feat(editor): E18 block copy text rule`.

### Task 2: 점검·빌드가 블록 복제를 쓴다

**Files:**
- Modify: `editor/CtaToybox.Core/ModBuilder.cs`(Build 첫머리, StageItem), `editor/CtaToybox.Core/EditorSession.Edits.cs`(CheckEdits·Check)
- Test: `editor/CtaToybox.Core.Tests/BlockCopyBuildTests.cs`

**Interfaces:**
- Consumes: Task 1 전부.
- Produces: `ModBuilder`가 같은 파일의 블록 복제를 먼저 적용한 본문에 나머지 수정을 적용한다. 의존 수정의 막힘 문장은 정확히 `"필드 " + 이름 + "가 없다(블록 복제가 꺼졌거나 적용되지 않았다)"`.

- [ ] **Step 1: Write the failing test** (`VisionSettingsTests.Files()` 꼴의 가짜 게임: `vision_fields.inc`에 `{"tank_range_200" {uncover 50} {h_fov 15} ("spotting2" r(1.5) r_human(0.66) veh_vis(50) can_vis(50) can_fire(70))}` 와 `{"human" {uncover 20} {h_fov 150}}`, `properties/tank_unarmed.ext`에 `{sensor {visor "around" {vision "tank_around"}}}`):
  - 프로젝트: 복제 `tank_range_200`→`ctb_tank_light`, 복제 블록 `uncover` 50→130(NamedPath `/"ctb_tank_light"/uncover`), 원본 `human` `h_fov` 150→360, `tank_unarmed.ext` around `"tank_around"`→`"ctb_tank_light"`('전부').
  - `CheckEdits` 모두 Applied. `Build` 적용 4건, 출력 `vision_fields.inc`가 파싱되고 `/"ctb_tank_light"/uncover` 130, `/"human"/h_fov` 360, `/"tank_range_200"/uncover` 50 그대로.
  - 복제를 끄면 복제 안 값 수정과 around 수정이 점검·빌드 모두 Invalid, 메시지에 `필드 ctb_tank_light가 없다`. 출력 `tank_unarmed.ext`가 생기지 않는다.
  - 원본에서 `tank_range_200`이 없는 판(다른 가짜 게임)에서는 복제 TargetMissing, 의존 수정 Invalid.
- [ ] **Step 2: Run** → 실패.
- [ ] **Step 3: Implement**
  - `ModBuilder.Build`: 켜진 수정 가운데 블록 복제를 파일별로 모아, 원본 본문에 `BlockCopy.Apply`로 적용해 적용된 이름 집합을 얻는다(결과는 버린다). 그다음 `BlockCopy.Dependency`가 그 집합에 없는 이름을 내는 켜진 수정을 Invalid로 보고하고 빼낸다(E4 macro-name 사전 검사와 같은 자리).
  - `StageItem`: `item.Edits` 가운데 블록 복제를 떼어 원문 본문에 `BlockCopy.Apply`(결과는 이번에는 보고)로 먼저 넣고, 나머지를 그 본문에 적용한다. 적용된 복제 수는 `count`에 더한다. 바이트 왕복 검사는 원본 그대로 한다.
  - `CheckEdits`: `SourceCheck.Text`를 만들 때 그 파일의 켜진 복제를 `BlockCopy.ApplyAll`로 넣은 본문을 쓴다. 블록 복제 수정 자체는 `BlockCopy.Apply`의 결과를 그 수정의 점검 결과로 쓴다. 의존 이름이 적용된 복제에 없으면 위 문장으로 Invalid.
- [ ] **Step 4: Run** 전체 편집기 테스트 → 통과.
- [ ] **Step 5: Commit** `feat(editor): E18 block copies in edit check and build`.

### Task 3: 세션·앱·명령줄에서 직접 복제

**Files:**
- Modify: `editor/CtaToybox.Core/SourceStack.cs`(덧씌우기), `editor/CtaToybox.Core/EditorSession.cs`·`EditorSession.Edits.cs`, `editor/CtaToybox.App/EditDialog.xaml(.cs)`, `editor/CtaToybox.App/Items.cs`(`Text.EditTitle`), `editor/CtaToybox.App/MainWindow.Scenario.cs`, `editor/CtaToybox.Cli/Program.cs`(add·remove `--kind block-copy`, 값 add가 복제 본문을 본다)
- Test: `editor/CtaToybox.Core.Tests/BlockCopySessionTests.cs`

**Interfaces:**
- Consumes: Task 1·2.
- Produces:
  - `SourceStack.Overlay : Func<string, string, string>` (가상 경로, 원문 → 보일 본문). `ReadText`만 거친다. `Read`(바이트)는 원본 그대로.
  - `Edit EditorSession.AddBlockCopy(UnitView unit, ValueView nameValue, string newName)` — `nameValue.Trace.Note == UnitTracer.BlockNameNote`이고 `SettingFiles.InArea(file)`일 때만. 이름 검사는 `BlockCopy.CheckName`과 "지금 본문에 없는 이름". 아니면 ArgumentException. 저장하고 덧씌우기를 다시 건다.
  - `EditorSession` 안 `void RefreshOverlay()` — 프로젝트의 켜진 복제로 `Stack.Overlay`를 정하고 `tracer`를 새로 만든다. `OpenProject`, `AddBlockCopy`, `RemoveEdit`, `SetEnabled`, `MergeProject` 뒤에 부른다. `ValidateNew`가 읽는 원본 본문도 복제를 넣은 본문이다.
  - `Describe`는 블록 복제에 `ValueLabel = "블록 복제"`, `RowText = 원본 이름`. 앱 `Text.EditTitle`은 블록 복제를 `"블록 복제: tank_range_200 → ctb_tank_light"`로 보인다.
  - 앱 값 고치기 창: 줄 값이 `BlockNameNote`이고 설정 파일이면 "새 이름" 칸과 "이 블록 복제" 단추. 시나리오 명령 `copyblock <새 이름>`(열린 창에서 칸에 쓰고 단추를 누른다).
  - 명령줄: `add --kind block-copy --file <파일> --path <이름 줄 경로> [--occ N] --new "<새 이름>"`, `remove --kind block-copy`(같은 자리). 값 `add`는 프로젝트 파일이 있으면 그 파일의 켜진 복제를 넣은 본문에서 자리를 찾는다.

- [ ] **Step 1: Write the failing test** (Task 2와 같은 가짜 게임):
  - `OpenUnit("set/vision/vision.set")`의 `tank_range_200` 이름 값으로 `AddBlockCopy(…, "\"ctb_tank_light\"")` → 수정 Kind block-copy, 프로젝트 파일에 `"kind": "block-copy"`. 다시 연 유닛에 `RawPath`가 `"ctb_tank_light"`를 담은 `uncover` 줄이 있고 값 고칠 수 있음.
  - 그 줄에 `AddEdit(…, "130", "shared")` → NamedPath `/"ctb_tank_light"/uncover`. `OpenProject` 다시 → 그 수정의 Path·Occurrence가 같은 줄을 가리킨다(Review Focus 3).
  - 같은 이름 다시 → ArgumentException. `"human"`(있는 이름) → ArgumentException. 유닛 파일(설정 파일 밖)의 따옴표 이름 → ArgumentException.
  - 복제를 `SetEnabled(false)` → 다시 연 유닛에 새 블록 줄이 없다.
  - 명령줄: `add --kind block-copy …` 0, 같은 이름 다시 1(이유에 "이미"), 그다음 `add --path '/"ctb_human"/uncover' --new 77`이 0, `remove --kind block-copy …` 0.
- [ ] **Step 2: Run** → 실패.
- [ ] **Step 3: Implement** 위 Interfaces대로. 앱은 `dotnet build editor/CtaToybox.App -c Release` 경고 0.
- [ ] **Step 4: Run** 전체 테스트(`pwsh -NoProfile -File tools/tests/Run-Tests.ps1`) → 통과.
- [ ] **Step 5: 화면 확인(한 번 실행)**: 시나리오 파일로 `unit set/vision/vision.set` → `tab full` → `filter tank_range_200` → `open tank_range_200` → `shot e18-dialog` → `copyblock ctb_tank_light` → `filter ctb` → `shot e18-copy` → `shot e18-list`. 그림을 읽고 이름·묶음·수정 목록 제목이 읽히는지 본다. 고칠 것을 모아 한 번에 고친다.
- [ ] **Step 6: Commit** `feat(editor): E18 block copy in the app and CLI`.

### Task 4: `vision-preset --classes`

**Files:**
- Modify: `editor/CtaToybox.Core/VisionPresets.cs`, `editor/CtaToybox.Cli/Program.cs`(옵션 `classes`·`heavy-uncover`·`medium-r`·`medium-uncover`·`light-r`·`light-uncover`·`vehicle-r`·`vehicle-uncover`)
- Test: `editor/CtaToybox.Core.Tests/VisionPresetsTests.cs`(추가)

**Interfaces:**
- Consumes: Task 1~3.
- Produces:
  - `public sealed class ClassVision { public double MediumR = 1.65, LightR = 1.8, VehicleR = 1.8; public int HeavyUncover = 100, MediumUncover = 115, LightUncover = 130, VehicleUncover = 145; }`
  - `void VisionPresets.AddClasses(EditProject project, SourceStack stack, IEnumerable<UnitEntry> units, UnitTracer tracer, Func<string, int> heirsOf, ClassVision values, List<string> notes)` — `Build(…, "wot", …)`이 만든 프로젝트에 더한다:
    1. `tank_`로 시작하는 `GroundFields`의 `uncover` 수정 값을 `HeavyUncover`로(이미 있는 50 수정을 바꾼다).
    2. `tank_range_200` 복제 셋과 각 복제의 `h_fov` 360, `uncover`, `("spotting2")` r 수정(복제를 넣은 본문의 자리).
    3. 경로가 `/tank_medium/`·`/tank_light/`인 차량 유닛(`_x`·`_xx` 제외)마다 `around` visor의 vision 값을 '이 유닛만'으로 `"ctb_tank_medium"`·`"ctb_tank_light"`로. 자리는 펼친 줄 가운데 경로가 `/sensor/visor/vision`으로 끝나는 값의 파일 줄에서 바로 앞 visor 머리 줄의 이름이 `around`(대소문자 무시)인 것. `UnitScope.Blocker`나 `heirsOf(unit) > 0`이면 건너뛰고 notes에 `"건너뜀: <유닛>: <이유>"`. around가 없으면 `"건너뜀: <유닛>: around visor 없음"`.
    4. `properties/car.ext`·`armoredcar.ext`·`moto.ext`의 around vision을 '전부'로 `"ctb_vehicle"`.
  - CLI `vision-preset --variant wot --classes --project <파일>`: 출력 첫 줄에 수정 수, 이어서 `"경전차 N, 중형전차 N, 차량 파일 N, 건너뜀 N"`과 건너뛴 목록.
- [ ] **Step 1: Write the failing test**: 가짜 게임에 `entity/-vehicle/x/tank_light/l1/l1.def`, `tank_medium/m1/m1.def`, `tank_heavy/h1/h1.def`(셋 다 `(include "/properties/tank_unarmed.ext")`), `tank_light/l1/l1_x.def`, `properties/car.ext`(around `human_peripheral`). `AddClasses` 뒤:
  - 복제 셋, `ctb_tank_light`의 uncover 130·r 1.8·h_fov 360, `ctb_tank_medium` 115·1.65, `ctb_vehicle` 145·1.8.
  - `l1`·`m1`의 '이 유닛만' around 수정 각 하나, `h1`·`l1_x`는 없음, car.ext '전부' 하나.
  - 전부 `CheckEdits` Applied, 빌드 뒤 `h1`의 출력 파일이 없고 `tank_unarmed.ext`의 '전부' 출력도 없다(중전차 그대로).
  - 옵션 값(`LightUncover = 140`)이 반영된다.
- [ ] **Step 2: Run** → 실패.
- [ ] **Step 3: Implement.**
- [ ] **Step 4: Run** 전체 테스트 → 통과.
- [ ] **Step 5: Commit** `feat(editor): E18 vision-preset --classes`.

### Task 5: 실제 파일 검사, 프로젝트 파일, 문서

**Files:**
- Modify: `editor/CtaToybox.Core.Tests/RealFilesCheck.cs`, `docs/07-stat-editor/README.md`, `docs/10-backlog.md`, `docs/superpowers/specs/2026-10-16-e18-class-vision-design.md`(넣는 자리 고침, 구현 결과), `docs/superpowers/handoff-2026-10-16.md`
- Create: `editor/projects/월드워-차종.json`(명령으로 만든다)

- [ ] **Step 1: `-- real` 검사 더하기**: 실제 판에서 `Build(…"wot"…)` + `AddClasses`로 프로젝트를 만들어 임시 출력 폴더에 `ModBuilder.Build`. 출력 `class-units-light=`·`class-units-medium=`·`class-skipped=`를 찍는다. 단언: 적용 불가 0(건너뛴 것은 프로젝트에 없으므로), 출력 `vision_fields.inc`에 `ctb_tank_light`·`ctb_tank_medium`·`ctb_vehicle`이 이름 줄로 한 번씩, 경전차 유닛 수 > 100.
- [ ] **Step 2: Run** `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real` → 통과.
- [ ] **Step 3: 프로젝트 만들기**: `dotnet run --project editor/CtaToybox.Cli -c Release -- vision-preset --variant wot --classes --project editor/projects/월드워-차종.json`. 출력의 수와 건너뛴 목록을 확인한다. 출력 모드에 빌드하지는 않는다(사용자가 확인한 뒤).
- [ ] **Step 4: 문서**: README(블록 복제, `--classes`), 백로그 E18 줄을 "정리하며 지운 것"으로, 스펙(넣는 자리: 같은 부모의 마지막 형제 뒤, 구현 결과 수), 핸드오프.
- [ ] **Step 5: 전체 테스트와 Commit** `feat(editor): E18 real check, WorldWar class project, docs`, develop에 병합.
