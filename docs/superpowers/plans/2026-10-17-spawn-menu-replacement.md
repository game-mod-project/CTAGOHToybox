# 스폰 메뉴 대체 구현 계획

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 편집기 명령 `spawn-menu build`가 새 모드 `mod/CTAToyboxSpawn`을 만든다. 스커미시 구매 창은 전 유닛·비용 0 로스터로, 캠페인·컨퀘스트·멀티 맵은 증원 창 메뉴(단계 메뉴·옵션)로 Spawn Menu of MACE를 대체한다.

**Architecture:** `CtaToybox.Core/Spawn/`에 생성기를 둔다. 입력은 편집기의 `SourceStack`(Spawn Menu 두 모드와 스폰 모드 자신을 뺀 계층)과 `UnitIndex`·`ArmyIndex`. 생성기는 파일 목록(경로 → 바이트)을 만들고 `SpawnWriter`가 자기 목록 `.ctatoybox-spawn.json`으로 출력 폴더를 갱신한다. 맵 `.mi`는 이긴 판 바이트 + include 줄, 로스터 `.set`은 이긴 판에서 값 인자만 바꾼 글, 메뉴는 `map/ctatoybox_spawn/*.mi` 생성물이다.

**Tech Stack:** .NET 8, C#(`Nullable`·`ImplicitUsings` 끔), 외부 패키지 없음. 테스트는 `editor/CtaToybox.Core.Tests`의 `T.Eq` 꼴.

**Spec:** `docs/superpowers/specs/2026-10-17-spawn-menu-replacement-design.md` (조사: `docs/13-spawn-menu-spike.md`)

## Global Constraints

- 게임 폴더·창작마당 폴더의 파일을 고치지 않는다. 읽기만 한다.
- 게임을 띄우는 확인은 하지도 제안하지도 않는다. 파일로 확인하지 못한 것은 `미확인`으로 적는다.
- 브랜치: `develop`에서 `feat/spawn-menu`를 따고 `git merge --no-ff`로 `develop`에 넣는다. 커밋 끝줄 `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.
- 출력 폴더 `mod/CTAToyboxSpawn`(이름 `CTAToybox Spawn`). 스탯 출력 모드 `mod/CTAToybox`에는 쓰지 않는다. `.ctatoybox-build.json`의 경로와 겹치면 그 파일은 쓰지 않고 오류로 보고한다.
- 입력 계층에서 뺀다: `mod_3617250510`(Spawn Menu of MACE), `mod_3295913567`(Spawn Menu), 스폰 모드 폴더 자신. 스탯 출력 모드는 넣는다.
- Lua를 쓰지 않는다. 메뉴 글자는 영어. 위장 바꾸기·`debug.inc`·보급 상자는 만들지 않는다.
- 맵 사본: 이긴 판 바이트 그대로 + include 줄만. 다른 줄은 바꾸지 않는다. 맵의 다른 파일은 복사하지 않는다.
- 분대 구성은 로스터 항목 그대로(사용자 결정). 새로 정하지 않는다.
- 스커미시 값(사용자 결정): 비용 0 MP, CP 0, 대기 0, 구매 횟수 제한 없음. `--keep-cost`로 원래 비용 유지.
- 종류 단추(사용자 결정): `inf1`, `inf2`, `empl`, `vehicles`, `light`, `medium`, `heavy`, `td`, `air`(지원은 `doctrine` 그대로). `--basic-buttons`면 기본 6개만(경·중형·중전차·구축은 `tanks`, 항공기는 `vehicles`).
- 정의로 풀리지 않는 이름(차량 정의, 병종 `mp/<국가>/<시기>/<병종>`)은 메뉴·로스터에 넣지 않고 보고한다(`define not found` 방지).
- 생성 파일 인코딩: `.mi`·`.set`·`.inc`는 이긴 판의 바이트를 바탕으로 하고, 새로 만드는 파일은 UTF-8(BOM 없음), CRLF(바닐라 미션 꼴. 구현자가 바닐라 `.mi` 한 개의 줄바꿈을 보고 맞춘다).
- 새 테스트는 `editor/CtaToybox.Core.Tests/<이름>Tests.cs`에 `static class <이름>Tests { public static void Run() }`, `Program.cs`의 `Suites.All`에 등록.
- 테스트 명령: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`(결과 `failed=0`). 마지막에 `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`.

## 조사로 정한 것 (2026-10-17, 파일 근거)

- 로스터: 바닐라 `gamelogic.pak!set/multiplayer/units/`. 항목 꼴 `{"panzer1b" ("vehicle2" period(mid) cd(150) ci(60) t(light) side(ger) crew1(tankman:1) crew2(tank_commander:1) g(light1) b(tanks) cw(4) cp(8) cost(105))}`(`mid/units_ger.set` 789행). 보병 분대 `("squad_with3types_doc" side(ger) name(...) ... c1(ersatz_sl:1) c2(ersatz_sl_asst:1) c3(ersatz_rifle:8) b(inf1) t())`(403행). `cd`·`ci`·`cp`·`cost`·`cw`는 매크로 기본값이 없어 항목마다 바꾼다. 차량 비용에는 승무원 단가가 `squad_cost_factor`로 더해지고, 보병 비용은 `inf_<국가>.set` 병종 `cost()` × `scf()`다. 횟수 제한은 `set/multiplayer/games/presets/global_parameters/unitlimits.inc`(와 `frontlines/unitlimits_attacker.inc`·`_defender.inc`)의 `{unitLimits "officer=2;…"}`.
- MACE가 `late/units_*.set`·`roster_late.set`을 덮고 `late/units_jap.set`을 더한다. Wunderwaffe는 MACE와 같은 경로를 덮는다. 이긴 판은 `SourceStack`이 고른다.
- 컨퀘스트 로스터(`set/multiplayer/units/conquest/`)는 바꾸지 않는다(스펙).
- 미션: 단추 `{"arty" {state hidden} {text "$/artystrike"} {image "…"} {hint "$/artystrike"} {count -1} {score 0} {delay 120} {eventStart "re-arty"} {waypoint "95"} {previewEntity crosshair}}`(bootcamp `04_reinforcements-offmap-support/0.mi` 14579). 표시는 `{"reinforcement" {operation show|hide} {name <단추, * 허용>}}`. 소속 `{"player" {selector …} {operation set} {player "N"}}`. 보병은 숨긴 견본을 `{"waypoint" {who …} {action {type start} {waypoint "W"} {clone} {make_squad}}}`로 복제한다(같은 파일 19146). 병종 경로로 사람을 바로 만드는 동작은 없다.
- 동작 원문: 포격 `{"loop" N {"spawn" {entity "artillery_single_150_dummy"} {waypoint "W"}} {"delay" {time 1.5}}}`(kollaa 43455), 무적 `{"entity_state" {selector …} {impregnability full}}`, 등급 `{"actor_state" {selector …} {human {veterancy 8}}}`, 삭제 `{"delete" {selector …}}`, 점수 `{"reinforcement" {operation score_add} {score N}}`, 항공 `{"air_attack" {selector {tag …}} {enemies …} {count N} …}`. 반복은 트리거 `{autoreset}` + `"delay"`.
- 컨퀘스트 국가: MACE `dcg_script.inc` 11138 `"dcg/player_nation"`의 `{"switch" {"case" {condition {type side} {side rus}} …}}`(rus 1, ger 2, fin 3, usa 4, eng 5, jap 11).
- 화면: 바닐라 `SceneGameView.xml` 211행 `<DialogRef refid="gui_gc_reinforcement"/>` 하나. Spawn Menu 판은 세 `FlowPanel`(멀티·싱글·컨퀘스트)에 모두 넣는다.

## Review Focus

1. 계층에 `{mission`이 없거나 닫는 괄호를 못 찾는 `.mi`(빈 파일, 다른 구조) → 그 맵은 건너뛰고 보고한다. 바이트를 망가뜨려 쓰지 않는다(Task 4 `map_unparsable_skipped`).
2. 같은 차량 이름이 두 국가 로스터에 있거나(`ArmyIndex.Shared`) 국가를 정하지 못하는 정의 → 새 항목을 만들지 않고 보고(Task 3 `roster_shared_not_added`).
3. 로스터 항목이 쓰는 승무원 병종이 그 시기의 `inf_<국가>.set`에 없다(새 항목의 기본 승무원) → 그 항목을 넣지 않고 보고(Task 3 `roster_crew_missing_skipped`).
4. 스폰 모드를 다시 생성할 때 사용자가 그 폴더에 둔 파일(목록 밖)은 지우지 않고, 지난 생성물만 지운다(Task 1 `writer_keeps_foreign_files`).
5. 한 화면 단추가 한도(21)를 넘는 종류(예: 독일 중형전차 수백 개) → 계열 묶음, 그래도 넘으면 `Next`/`Prev` 페이지로 나뉘고 모든 유닛이 어느 페이지에든 한 번 나온다(Task 5 `menu_pages_cover_all`).

---

## File Structure

| 파일 | 맡는 일 |
|---|---|
| `editor/CtaToybox.Core/Spawn/SpawnSources.cs` | 스폰용 계층(뺄 모드 제외)으로 `SourceStack`을 연다 |
| `editor/CtaToybox.Core/Spawn/SpawnWriter.cs` | 출력 폴더 갱신, `.ctatoybox-spawn.json`, 스탯 목록과의 겹침 검사 |
| `editor/CtaToybox.Core/Spawn/SpawnCatalog.cs` | 소환 유닛 목록: 국가·종류·계열·로스터 항목 |
| `editor/CtaToybox.Core/Spawn/SpawnCategories.cs` | `editor/spawn/categories.psv` 읽기와 종류 규칙 |
| `editor/CtaToybox.Core/Spawn/RosterRewrite.cs` | 스커미시 로스터 값 0, 새 차량 항목, 단추, unitlimits |
| `editor/CtaToybox.Core/Spawn/MapPatch.cs` | 맵 `.mi` 사본 + include 줄 |
| `editor/CtaToybox.Core/Spawn/MissionText.cs` | 미션 SDL 조각 쓰기 도우미(단추·트리거·동작) |
| `editor/CtaToybox.Core/Spawn/MenuBuilder.cs` | `menu.mi`: 견본·대기 구역·단추·트리거·옵션 |
| `editor/CtaToybox.Core/Spawn/SceneViewPatch.cs` | `SceneGameView.xml` 증원 창 켜기 |
| `editor/CtaToybox.Core/Spawn/SpawnGenerator.cs` | 위를 묶어 파일 목록과 보고를 만든다 |
| `editor/CtaToybox.Core/EditorSettings.cs` | `SpawnModDir` 키 |
| `editor/CtaToybox.Cli/Program.cs` | `spawn-menu preview|build` |
| `editor/spawn/categories.psv` | 종류 규칙(손으로 고침) |
| `mod/CTAToyboxSpawn/` | 생성물(`mod.info`는 손으로 둔다) |

---

### Task 1: 계층과 출력 쓰기

**Files:**
- Create: `editor/CtaToybox.Core/Spawn/SpawnSources.cs`, `editor/CtaToybox.Core/Spawn/SpawnWriter.cs`, `editor/CtaToybox.Core.Tests/SpawnWriterTests.cs`
- Modify: `editor/CtaToybox.Core/EditorSettings.cs`(`public string SpawnModDir { get; set; }`, 비면 `<OutputModDir의 부모>/CTAToyboxSpawn`), `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Produces:
  - `public static readonly string[] SpawnSources.ExcludedLayers = { "mod_3617250510", "mod_3295913567" };`
  - `public static SourceStack SpawnSources.Open(EditorSettings s, LayerPlan plan, List<string> warnings)` — `plan.Layers`에서 `ExcludedLayers`와 `ResourceDir`이 `<SpawnModDir>/resource`인 층을 빼고, 스탯 출력 모드(`<OutputModDir>/resource`)가 `plan`에 없으면 맨 뒤에 `"output"` 층으로 더한다. `Scan()`까지 한다. 뺀 층 이름을 `warnings`에 적는다.
  - `public sealed class SpawnFile { public string Path; public byte[] Bytes; public string Source; }` — `Path`는 모드 폴더 기준(`resource/map/…`, `/`).
  - `public sealed class SpawnWriteReport { public List<string> Written, Removed, Errors; }`
  - `public static SpawnWriteReport SpawnWriter.Write(string modDir, IReadOnlyList<SpawnFile> files, string statModDir)` — 지난 `.ctatoybox-spawn.json`(`{"files":[{"path":…}]}`)에 있고 이번에 없는 파일은 지운다. 목록 밖에 이미 있는 파일은 덮지 않고 `Errors`. `statModDir/.ctatoybox-build.json`의 `files[].path`(resource 기준)와 겹치는 `resource/…` 경로는 쓰지 않고 `Errors`. 바이트가 같은 파일은 다시 쓰지 않는다. 마지막에 목록 파일을 쓴다.

- [ ] **Step 1: 실패하는 테스트**

```csharp
// 임시 modDir에 사용자 파일 resource/user.txt, 스탯 폴더 .ctatoybox-build.json {"files":[{"path":"map/x.mi"}]}
var r = SpawnWriter.Write(dir, new[] { F("resource/map/a.mi", "A"), F("resource/map/x.mi", "X") }, stat);
T.True(File.Exists(Path.Combine(dir, "resource/map/a.mi")), "written");
T.True(r.Errors.Any(e => e.Contains("map/x.mi")) && !File.Exists(Path.Combine(dir, "resource/map/x.mi")), "stat overlap refused");
r = SpawnWriter.Write(dir, new[] { F("resource/map/b.mi", "B") }, stat);
T.True(!File.Exists(Path.Combine(dir, "resource/map/a.mi")), "stale own file removed");
T.True(File.Exists(Path.Combine(dir, "resource/user.txt")), "writer_keeps_foreign_files");
// 계층: FactionPresetTests.Fixture 꼴로 options.set에 mod_3617250510을 넣고 SpawnSources.Open 결과 Layers에 없음
```
- [ ] **Step 2: 실패를 본다** — Run: 테스트 명령 / Expected: 빌드 오류
- [ ] **Step 3: 구현한다**
- [ ] **Step 4: 통과를 본다** — Expected: `failed=0`
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): spawn layers and output writer"`

---

### Task 2: 소환 유닛 목록과 종류

**Files:**
- Create: `editor/CtaToybox.Core/Spawn/SpawnCategories.cs`, `editor/CtaToybox.Core/Spawn/SpawnCatalog.cs`, `editor/spawn/categories.psv`, `editor/CtaToybox.Core.Tests/SpawnCatalogTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `UnitIndex.Build`(Units: Path, Kind, EditorKind), `ArmyIndex.Build(stack, units)`(`ArmyOf`, `Shared`), `RosterLinks`.
- Produces:
  - `categories.psv` 형식 `종류|규칙|값`(`#` 주석). 종류 `light|medium|heavy|td|air|car|empl|other`. 규칙 `folder`(경로 조각 포함), `name`(파일 이름에 단어 포함), `button`(로스터 `b()` 값). 첫 내용:
    `light|folder|/tank_light/`, `medium|folder|/tank_medium/`, `heavy|folder|/tank_heavy/`, `td|name|td,stug,jagd,gmc,spg,sau,su_,marder,hetzer,nashorn,elefant,ferdinand,isu,achilles,wolverine,hellcat`, `empl|button|empl`, `car|button|vehicles`, `air|folder|/air/,/aviation/,/plane/`.
    순서(스펙): 로스터 `b()`가 `tanks`가 아니면 그 값의 종류 → 폴더 → 이름 → `other`. `b(tanks)`면 폴더·이름 규칙으로 경·중·중전차·구축을 가르고, 못 가르면 `medium`.
  - `public sealed class SpawnUnit { public string Def; public string Name; public string Army; public string Category; public string Family; public RosterEntry Roster; }` — `Name`은 정의 파일 이름(확장자 뺀 것), `Family`는 정의 폴더 이름에서 끝의 `_<글자>` 꼬리와 숫자를 뗀 앞부분(예: `tiger1e`·`tiger1h` 폴더 → `tiger`. 규칙: 폴더 이름을 `[a-z]+`로 처음 맞는 조각).
  - `public sealed class SpawnSquad { public string Army; public string Period; public string Name; public string Button; public List<(string Breed, int Count)> Members; public string File; }` — 로스터 `b(inf1)`·`b(inf2)` 항목에서. `Members`는 `c1(x:n)`…를 `mp/<side>/<period>/x`로.
  - `public static SpawnCatalog SpawnCatalog.Build(SourceStack stack, UnitIndex index, ArmyIndex armies, List<string> skipped)` — `Units`: `EditorKind`가 `tank|cannon|car`인 정의, 그리고 `vehicle-other` 가운데 로스터에 이름이 있는 정의(항공기 등). `Army` 없음·`Shared`는 `skipped`에 이유와 함께. `Squads`: 스커미시 로스터(`set/multiplayer/units/{early,mid,late}/units_*.set`, conquest 제외) 항목 전부, 병종 파일(`set/breed/mp/<side>/<period>/<x>.set` 등 `stack.Find`로 찾는 실제 경로. 구현자가 바닐라에서 `mp/ger/mid/tankman`이 어느 파일로 풀리는지 보고 정한다)로 풀리지 않는 구성원이 있으면 그 분대를 `skipped`에.
  - `public List<SpawnUnit> Units; public List<SpawnSquad> Squads;`

- [ ] **Step 1: 실패하는 테스트** — `FactionPresetTests.Fixture` 꼴 픽스처에 `tank_light/lt`, `tank_heavy/ht`, `ussr/tank_medium/rt`, 이름 `stug3g`, 로스터 `mid/units_ger.set`에 `{"lt" ("vehicle2" … b(tanks) …)}`와 분대 `("squad_with2types_doc" side(ger) name(sq_a) period(mid) … c1(nco:1) c2(rifle:3) b(inf1) t())`, 병종 `mp/ger/mid/nco`·`rifle`.

```csharp
T.Eq("light", cat.Units.Single(u => u.Name == "lt").Category, "folder rule");
T.Eq("td", cat.Units.Single(u => u.Name == "stug3g").Category, "name rule");
T.Eq("rus", cat.Units.Single(u => u.Name == "rt").Army, "army by folder");
var sq = cat.Squads.Single(s => s.Name == "sq_a");
T.Eq(new[] { ("mp/ger/mid/nco", 1), ("mp/ger/mid/rifle", 3) }, sq.Members.ToArray(), "squad members from the roster");
// 구성원 병종이 없는 분대 → skipped
```
- [ ] **Step 2~4:** 실패 → 구현 → 통과. real 검사(`RealFilesCheck.cs`)에 지금 계층으로 `Build`해 `Units` 수, 종류별 수, `Squads` 수(약 1,151, 국가·시기·이름 기준), `skipped` 수를 출력하고 `Units`에 Wunderwaffe 정의가 1개 이상 있음을 단언.
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): spawn unit catalog and categories"`

---

### Task 3: 스커미시 로스터

**Files:**
- Create: `editor/CtaToybox.Core/Spawn/RosterRewrite.cs`, `editor/CtaToybox.Core.Tests/SpawnRosterTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `SpawnCatalog`(Task 2), `SpawnFile`(Task 1).
- Produces:
  - `public sealed class RosterOptions { public bool KeepCost; public bool BasicButtons; }`
  - `public static List<SpawnFile> RosterRewrite.Build(SourceStack stack, SpawnCatalog cat, RosterOptions o, List<string> skipped)` —
    1. 이긴 판 `set/multiplayer/units/{early,mid,late}/{units,inf}_*.set` 마다(conquest 아님): `KeepCost`가 아니면 매크로 호출 인자 `cd(…)`·`ci(…)`·`cp(…)`·`cost(…)`·`cw(…)`·`cool(…)`의 값을 `0`으로, 블록 `{cost N}`·`{cp N}`의 N을 `0`으로. 단추: `BasicButtons`가 아니면 `b(tanks)` 항목의 `b()`를 그 차량의 `Category`(light/medium/heavy/td)로. 다른 글자는 바이트 그대로(토큰 위치만 바꾼다: `TokenPatcher`를 쓴다).
    2. 그 국가의 어느 시기 로스터에도 없는 `Units`(`Roster == null`, `Category`가 `other` 아님)를 그 국가의 각 시기 `units_<국가>.set` 끝(맨 위 블록 밖, 파일 끝)에 새 항목으로 더한다: `{"<Name>" ("vehicle2" period(<p>) cd(0) ci(0) t() side(<army>) crew1(<승무원>:1) crew2(<전차장>:1) g(ctb_spawn) b(<단추>) cw(0) cp(0) cost(0))}`. 승무원은 국가별 표 `ger tankman/tank_commander, rus tankman/tank_commander, usa tankman_m3/tank_com_m3, eng tankman/tank_commander_m1928, fin tankman/tankman_vet, jap tankman/tank_commander`(조사 집계에서 고른 것. AI 판단); 거치·포(`empl`)는 `artilleryman:2`. 그 시기에 승무원 병종이 없으면 그 시기는 건너뛰고 `skipped`. 국가 로스터 파일이 없는 시기(usa early, jap early·mid)는 건너뛴다.
    3. `unitlimits.inc`, `frontlines/unitlimits_attacker.inc`, `unitlimits_defender.inc`의 이긴 판: `{unitLimits "…"}` 안 `이름=N`의 N을 모두 `999`로(`KeepCost`여도 한다: 횟수 제한 없음은 따로 끌 수 없음. AI 판단).
    4. `BasicButtons`가 아니면 단추 그림: 바닐라 `interface/scene/unit_icon/`에서 `tanks` 단추가 쓰는 그림 이름 꼴을 구현자가 `interface` 파일(`GameUnitSpawnView.xml`, `set/panel_control/*`, `unit_icon` 목록)에서 찾아 같은 꼴로 `light|medium|heavy|td|air` 그림을 `tanks` 그림 바이트로 만든다. 꼴을 찾지 못하면 그림은 만들지 않고 보고에 `미확인: 단추 그림 이름`을 적는다(단추는 그래도 만든다).
- [ ] **Step 1: 실패하는 테스트** — Task 2 픽스처에 `mid/inf_ger.set`(`{"mp/ger/mid/tankman" ("inf_tankman" … cost(5.0))}`, `tank_commander`), `global_parameters/unitlimits.inc` `{unitLimits "officer=2;limit4=4"}`.

```csharp
string units = Text(files, "resource/set/multiplayer/units/mid/units_ger.set");
T.True(units.Contains("cd(0) ci(0)") && units.Contains("cost(0)") && units.Contains("cp(0)"), "values zero");
T.True(units.Contains("b(light)"), "light tank button");
T.True(units.Contains("{\"ht\" (\"vehicle2\" period(mid)") && units.Contains("crew1(tankman:1) crew2(tank_commander:1)"), "missing vehicle added with default crew");
T.Eq("{unitLimits \"officer=999;limit4=999\"}", Text(files, "resource/set/multiplayer/games/presets/global_parameters/unitlimits.inc").Trim(), "limits lifted");
// roster_shared_not_added: 두 국가에 걸친 정의는 항목 없음 + skipped
// roster_crew_missing_skipped: inf_ger.set에 tank_commander가 없으면 ht 항목 없음 + skipped
// BasicButtons: b(tanks) 그대로, 새 항목도 b(tanks)
// KeepCost: cost(105) 그대로
```
- [ ] **Step 2~4:** 실패 → 구현 → 통과. 결과 `.set`을 `SdlCore.Parse`로 읽어 `Errors.Count == 0`도 단언. real 검사: 지금 계층으로 만들어 파싱 오류 0, 새 항목 수 출력.
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): free skirmish roster with all units"`

---

### Task 4: 맵 사본

**Files:**
- Create: `editor/CtaToybox.Core/Spawn/MapPatch.cs`, `editor/CtaToybox.Core.Tests/SpawnMapTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Produces:
  - `public const string MapPatch.MenuInclude = "(include \"/map/ctatoybox_spawn/menu.mi\")";`
  - `public static string MapPatch.NationOf(string virtualPath)` — 캠페인 폴더 `map/single/<NN-국가…>/`의 국가 조각: `rus→rus, ger→ger, fin→fin, usa→usa, eng→eng`(`041-usa_airborne`, `051-ger_finesthour`, `02-ger-west` 등 `-` 뒤 첫 단어). 모르면 null.
  - `public static byte[] MapPatch.Patch(byte[] original, string nation, out string error)` — 마지막 `}`(맨 위 `{mission` 블록의 닫는 괄호. 구현자가 `SdlCore.Parse`로 맨 위 블록이 `mission`인지와 그 닫는 괄호 위치를 확인한다) 바로 앞에 줄을 넣는다: 국가가 있으면 `(include "/map/ctatoybox_spawn/nation_<n>.mi")` 줄 다음 `MenuInclude` 줄, 없으면 `MenuInclude`만. 줄바꿈은 원문의 것(CRLF/LF)을 따르고 들여쓰기는 탭 하나. 이미 `MenuInclude`가 있으면 원문 그대로 돌려준다. `{mission`이 아니면 null과 `error`.
  - `public static List<SpawnFile> MapPatch.Build(SourceStack stack, List<string> skipped)` — `stack.Paths`의 `map/single/**/0.mi`와 `map/multi/**/*.mi`마다 이긴 판(`stack.Find`)을 읽어 `Patch`. 원문 바이트는 `stack`의 읽기(바이트) 수단을 쓴다(없으면 `SourceStack`에 `ReadBytes(virtualPath)`를 더한다).
- [ ] **Step 1: 실패하는 테스트**

```csharp
byte[] src = Encoding.UTF8.GetBytes("{mission\r\n\t{a 1}\r\n}\r\n");
T.Eq("{mission\r\n\t{a 1}\r\n\t(include \"/map/ctatoybox_spawn/nation_ger.mi\")\r\n\t(include \"/map/ctatoybox_spawn/menu.mi\")\r\n}\r\n", Encoding.UTF8.GetString(MapPatch.Patch(src, "ger", out _)), "two lines before the close");
byte[] once = MapPatch.Patch(src, null, out _);
T.Eq(once, MapPatch.Patch(once, null, out _), "an include already there is not added twice");
T.Eq(null, MapPatch.Patch(Encoding.UTF8.GetBytes("{scene}"), null, out string err), "map_unparsable_skipped");
T.Eq("ger", MapPatch.NationOf("map/single/051-ger_finesthour/x/0.mi"), "nation from folder");
// Build: 픽스처 바닐라 map/single.pak의 0.mi와 mod_3617250510 낱개 사본 → 결과가 바닐라 판 + 줄(Spawn Menu 판이 아님)
```
- [ ] **Step 2~4:** 실패 → 구현 → 통과. real 검사: 맵 수(캠페인·멀티), 건너뛴 수, 모든 결과가 원문 바이트 + 넣은 줄만 다른지(넣은 줄을 지우면 원문과 같다) 단언.
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): map copies with menu include"`

---

### Task 5: 메뉴 (`menu.mi`)

**Files:**
- Create: `editor/CtaToybox.Core/Spawn/MissionText.cs`, `editor/CtaToybox.Core/Spawn/MenuBuilder.cs`, `editor/CtaToybox.Core.Tests/SpawnMenuTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `SpawnCatalog`(Task 2).
- Produces:
  - `public sealed class MenuOptions { public int PageSize = 21; }`
  - `public sealed class MenuScreen { public string Id; public string Parent; public List<MenuButton> Buttons; }`, `public sealed class MenuButton { public string Id; public string Text; public string Image; public string Event; public string Waypoint; public string Opens; }` — `Id`·`Event`는 `ctb_` 앞붙이 영어·숫자·`_`만, 겹치지 않음. 화면 표시는 이름 와일드카드 `ctb_<화면>_*`로.
  - `public static List<MenuScreen> MenuBuilder.Screens(SpawnCatalog cat, string army, MenuOptions o)` — 스펙 "메뉴 단계": 첫 화면(Nation, Owner, Infantry, Guns, Vehicles, Tanks, Aircraft, Support, Tools) → 종류 화면(Tanks: Light/Medium/Heavy/TD; Infantry: Squads/Single) → 유닛 화면. 한 화면 유닛이 `PageSize` 넘으면 `Family` 묶음 화면을 한 단계 두고, 묶음 하나가 넘으면 `Next`/`Prev`로 나눈다. 모든 화면에 `Back`·`Home`(첫 화면 제외). 유닛 단추: `Text`·`hint` `desc/entity/<Name>`, `Image` `<Name>`, 분대는 `desc/squad/<Name>(<army>)`(스펙). 
  - `public static string MenuBuilder.Build(SpawnCatalog cat, MenuOptions o, out List<string> skipped)` — `;{mission`으로 시작하는 조각. 구성(구현자는 `cheat.mi`의 같은 블록 원문 꼴을 따른다. 행은 spike 표):
    - 견본 `{Human "<병종>" …}`: 분대 구성원의 병종마다 그 병종이 한 분대에 나오는 최대 수만큼(국가·시기별), 승무원 병종(국가별 Task 3 표, 시기는 `late` 우선 있는 것) 차량 하나 몫. 위치·숨김·태그는 `cheat.mi` 견본 꼴(맵 밖 `{Position -9639.63 9712.23}` 근처, `{Volume "ram" {able {visible 0}…}{disabled}}`). 태그 `ctb_h_<번호>`(병종·순번마다 하나).
    - `{Helpers`: `{event …}` 단추마다, 대기 구역 2개(보병·차량), `{reinforcements` 맨 위 `"ctb_spawn"`(`{count -1}`, 보임) + 나머지 `{state hidden}`, `{vars` (`ctb_nation`, `ctb_owner`, `ctb_god`, `ctb_maxvet`, `ctb_page…`), `{functions`, `{triggers`, `{waypoints` (대기 둘, 놓을 자리 `ctb_place`).
    - 국가 단추 6개(ger, rus, usa, eng, fin, jap; 모드가 더한 국가가 `cat`에 있으면 더함)는 `ctb_nation`을 정하고 그 국가 화면 묶음을 보인다(토글 아님). 국가마다 화면 묶음이 따로다.
    - 소속 화면: `Me`, `Player 0` … `Player 9`(바로 고름). `Me`의 값: 캠페인(`nation_<n>.mi`가 정한 변수 `ctb_campaign`=1)은 `player "0"`(바닐라 bootcamp 19278 근거), 그 밖은 처음 놓을 때 고른 번호를 기억한다(스펙의 대비책). 고르기 전 첫 배치는 소속 화면을 먼저 연다.
    - 차량 트리거: 승무원 견본 `{clone}` → `{"spawn" {entity "<Name>"} {waypoint 대기}}` → 함수 `ctb_place_vehicle`(cheat.mi `"vk c"` 59338–59613의 순서: 태그 붙이기 → `"player" … {operation set} {player "<ctb_owner>"}` → `"placement" … {target_waypoint ctb_place}` → `"linker"`) → `{mode reset}`. 소속 값은 변수 비교 `"switch"`로 `player "0"`~`"9"` 열 갈래.
    - 분대 트리거: 그 분대 구성원 견본 태그를 모아 `{"waypoint" {who …} {action {type start} {waypoint ctb_place} {clone} {make_squad}}}`(bootcamp 19146 꼴) → 소속 → 최고 등급 변수면 `"actor_state" … {veterancy 8}`.
    - 국가 기본값: 멀티는 시작 트리거가 `dcg/player_nation`과 같은 `{"switch" {"case" {condition {type side} {side <n>}} …}}`로 `ctb_nation`을 정하고, 못 정하면 ger(스펙). 캠페인은 `nation_<n>.mi`가 먼저 정한다.
    - 메뉴 유지: `{autoreset}` 트리거가 30초마다(`"delay" {time 30}`) `{"reinforcement" {operation show} {name ctb_spawn}}`.
  - `public static string MenuBuilder.NationFile(string army)` — `nation_<army>.mi`: `;{mission` 조각, 시작 트리거가 `ctb_nation`·`ctb_campaign`을 정한다.
- [ ] **Step 1: 실패하는 테스트** — Task 2 픽스처에 독일 중형전차 50개(이름 `m00`~`m49`, 폴더 계열 `pz` 30개·`stu` 20개)를 더한다.

```csharp
var screens = MenuBuilder.Screens(cat, "ger", new MenuOptions());
var units = screens.SelectMany(s => s.Buttons).Where(b => b.Event != null && b.Event.StartsWith("ctb_u_")).Select(b => b.Text).ToList();
T.Eq(cat.Units.Count(u => u.Army == "ger"), units.Distinct().Count(), "menu_pages_cover_all");
T.Eq(units.Count, units.Distinct().Count(), "each unit once");
T.True(screens.All(s => s.Buttons.Count <= 21 + 4), "page size plus navigation");
string menu = MenuBuilder.Build(cat, new MenuOptions(), out var skipped);
var parsed = SdlCore.Parse(menu.Substring(menu.IndexOf('\n') + 1));
T.Eq(0, parsed.Errors.Count, "menu parses");
// 단추의 eventStart마다 같은 이름 트리거의 event 조건이 있다, 트리거마다 {event} 선언이 있다(이름 집합 비교)
// 모든 {entity "…"}가 cat.Units의 Name, 모든 {Human "…"}가 풀리는 병종
```
- [ ] **Step 2~4:** 실패 → 구현 → 통과. real 검사: 지금 계층으로 `Build`, 파싱 오류 0, 단추 수·트리거 수·견본 수·파일 크기 출력.
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): reinforcement menu with nation, owner and unit pages"`

---

### Task 6: 옵션

**Files:**
- Modify: `editor/CtaToybox.Core/Spawn/MenuBuilder.cs`, `editor/CtaToybox.Core.Tests/SpawnMenuTests.cs`

**Interfaces:**
- Produces: 첫 화면 `Support`·`Tools` 아래 단추와 트리거(스펙 "옵션" 표):
  - `God mode On/Off`: 변수 `ctb_god`; `{autoreset}` 트리거가 5초마다 `ctb_god`=1이면 `{"entity_state" {selector {source advanced} {ignore_captured_by_user 0} {group {select {player {player "<ctb_owner>"}}}}} {impregnability full}}`, Off는 한 번 `{impregnability disabled}`(값 이름은 bootcamp 21460 `disabled` 근거).
  - `Max veterancy (all mine)`, `Max veterancy (area)`: 앞은 내 소속 전체에 `{"actor_state" … {human {veterancy 8}}}`, 뒤는 `{waypoint ctb_place}` 단추로 찍은 자리 근처(선택자 `near`. 구현자가 바닐라 `near` 조건 원문 꼴을 찾아 쓴다. 못 찾으면 이 단추는 빼고 보고).
  - `Spawn at max veterancy On/Off`: 변수 `ctb_maxvet`.
  - `Artillery small/medium/large`: 단추 `{waypoint ctb_target} {previewEntity crosshair}` → `{"loop" 1|3|6 {"spawn" {entity "artillery_single_150_dummy"} {waypoint ctb_target}} {"delay" {time 1.5}}}`. 곡사포·자주포 같은 더미(스펙).
  - `Air strike small/medium/large`: 항공기 정의(`cat.Units`의 `air`, 국가 = `ctb_nation`) 1·2·4대를 대기 구역에 `"spawn"`하고 태그를 붙여 `{"air_attack" {selector {tag ctb_air}} {enemies {ignore_captured_by_user 0} …} {count N}}`(dyle 45807 꼴). 그 국가에 항공기가 없으면 단추를 만들지 않는다.
  - `Reinforcement points +1000`: `{"reinforcement" {operation score_add} {score 1000}}`.
  - `Delete (area)`, `Delete all aircraft`, `Clear wrecks`: `"delete"`(bootcamp 14897 꼴). 잔해 선택자는 `cheat.mi` 删除 분류 트리거의 원문 꼴을 따른다.
- [ ] **Step 1: 실패하는 테스트** — 메뉴 글에 `impregnability full`, `veterancy 8`, `artillery_single_150_dummy`가 들어 있고 `{"loop" 6`이 있음, 항공기 없는 국가의 메뉴에 `air_attack`이 없음, 파싱 오류 0, 이벤트·트리거 짝 검사 통과.
- [ ] **Step 2~4:** 실패 → 구현 → 통과
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): menu options (god mode, veterancy, strikes, points, delete)"`

---

### Task 7: 화면 파일과 생성기 묶기

**Files:**
- Create: `editor/CtaToybox.Core/Spawn/SceneViewPatch.cs`, `editor/CtaToybox.Core/Spawn/SpawnGenerator.cs`, `editor/CtaToybox.Core.Tests/SpawnGeneratorTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: Task 1~6.
- Produces:
  - `public static string SceneViewPatch.Patch(string xml, out string error)` — `unitspawn_layer` 아래 세 `FlowPanel`(`enableif`가 `game.multiplayer and …`, `gui.editor or game.singleplayer …`, `game.campaign and …`) 각각의 `<Children>`에 `gui_gc_reinforcement` `DialogRef`가 조건 없이 하나 있게 한다(있는데 `enableif`가 있으면 그 속성을 지운다, 없으면 첫 자식으로 넣는다). 세 패널 중 못 찾는 것이 있으면 `error`. 나머지 바이트는 그대로.
  - `public sealed class SpawnOptions { public RosterOptions Roster; public MenuOptions Menu; }`
  - `public sealed class SpawnReport { public int Maps, CampaignMaps, MultiMaps, Units, Squads, NewRosterEntries, Buttons; public List<string> Skipped, Warnings; }`
  - `public static List<SpawnFile> SpawnGenerator.Generate(SourceStack stack, UnitIndex index, ArmyIndex armies, SpawnOptions o, SpawnReport report)` — 로스터 + 맵 + `resource/map/ctatoybox_spawn/menu.mi` + `nation_<n>.mi`들 + `resource/interface/layout/views/scene/scenegameview.xml`(이긴 판에 `Patch`. 경로 대소문자는 이긴 판 원래 이름) + 단추 그림.
- [ ] **Step 1: 실패하는 테스트** — 픽스처 전체로 `Generate`: 파일 경로 집합에 menu.mi, nation_ger.mi, 맵, 로스터, scenegameview.xml이 있음; `SceneViewPatch.Patch(바닐라 꼴 3패널 xml)`의 결과에 `gui_gc_reinforcement`가 3번, `enableif="gameclient.master"`가 그 줄에 없음; 패널 하나 없는 xml → `error`.
- [ ] **Step 2~4:** 실패 → 구현 → 통과
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): scene view patch and generator"`

---

### Task 8: 명령줄 `spawn-menu`

**Files:**
- Modify: `editor/CtaToybox.Cli/Program.cs`(`Usage`, `KnownOptions["spawn-menu"] = { "settings", "out", "force", "keep-cost", "basic-buttons" }`, `Flags`, `switch`, 새 `SpawnMenu` 메서드)
- Create: `editor/CtaToybox.Core.Tests/SpawnCliTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `SpawnSources.Open`, `UnitIndex.LoadOrBuild`(캐시는 스폰 계층 서명으로 `editor/cache/`에), `ArmyIndex.Build`, `SpawnGenerator.Generate`, `SpawnWriter.Write`, `EditorSession.GameRunning()`.
- Produces:
  - `ctatoybox spawn-menu preview` — 쓰지 않고 `맵 N(캠페인 N, 멀티 N), 유닛 N, 분대 N, 새 로스터 항목 N, 단추 N, 건너뜀 N` 한 줄 + 건너뛴 것 앞 20개(이유).
  - `ctatoybox spawn-menu build [--out <폴더>] [--force] [--keep-cost] [--basic-buttons]` — 게임 실행 중이면 `--force` 없이 멈춘다(`build`와 같은 문구). `<out>/mod.info`가 없으면 `{mod {name "CTAToybox Spawn"} {tags "Mod"} {desc "…"} {minGameVersion "any"} {maxGameVersion "any"}}`를 만든다(목록에 넣지 않음). 스탯 목록 `.ctatoybox-build.json`의 시각이 스폰 목록보다 새로우면 경고 한 줄. 보고 뒤 `Errors`가 있으면 종료 1.
  - 하위 명령이 없거나 모르면 사용법 오류(2).
- [ ] **Step 1: 실패하는 테스트** — `CliApp.Run`으로 픽스처에 `spawn-menu preview` → 0과 `맵 ` 줄, `spawn-menu build --out <임시>` → `mod.info`, `.ctatoybox-spawn.json`, menu.mi 생김, `spawn-menu nope` → 2.
- [ ] **Step 2~4:** 실패 → 구현 → 통과
- [ ] **Step 5: 커밋** — `git commit -m "feat(spawn): spawn-menu preview and build commands"`

---

### Task 9: 실제 생성, 문서, 마무리

**Files:**
- Create: `mod/CTAToyboxSpawn/`(생성물)
- Modify: `.gitattributes`(`mod/CTAToyboxSpawn/resource/** -text`, `mod/CTAToyboxSpawn/.ctatoybox-spawn.json -text`), `CLAUDE.md`(폴더 표, 명령 `spawn-menu`, 손으로 고치는 파일 `editor/spawn/categories.psv`), `docs/07-stat-editor/README.md`(`spawn-menu` 절: 정션 `mods\CTAToyboxSpawn` 연결과 로드 순서 권장 "모든 모드 뒤, Spawn Menu of MACE 끔", 게임·모드 갱신 뒤 다시 생성, 미확인 5개 표는 스펙 링크), `docs/10-backlog.md`(P1 줄 정리), 스펙 머리 상태.

- [ ] **Step 1:** `dotnet run --project editor/CtaToybox.Cli -c Release -- spawn-menu preview --settings E:\CTAToybox\editor\settings.json` — 수를 기록. 맵 수가 Spawn Menu의 캠페인 91·멀티 389 이상인지 본다(적으면 원인을 찾는다).
- [ ] **Step 2:** `… spawn-menu build --settings E:\CTAToybox\editor\settings.json` — `Errors` 0.
- [ ] **Step 3:** 생성물 정적 검사(스크래치 스크립트): 모든 `.mi`·`.set`이 `SdlCore.Parse` 오류 0, 맵 사본은 넣은 줄을 지우면 이긴 판 바이트와 같음, 스폰 모드와 스탯 모드의 `resource` 경로 겹침 0.
- [ ] **Step 4:** 전체 테스트 — `pwsh -NoProfile -File tools/tests/Run-Tests.ps1` 실패 0, `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real` 실패 0.
- [ ] **Step 5:** 커밋 — 코드와 생성물·문서. `git commit -m "build(spawn): generate CTAToybox Spawn mod"`; `feat/spawn-menu` → `develop` `git merge --no-ff`.
- [ ] **Step 6:** 사용자에게 보고: 정션 연결 명령(`New-Item -ItemType Junction`)과 `options.set` 순서는 사용자가 한다(스펙), Spawn Menu of MACE 끄기, 미확인 목록.
