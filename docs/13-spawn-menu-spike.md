# Spawn Menu 구조 조사 (spike, 2026-10-17)

하위 프로젝트 C(스폰 모드 대체, 백로그 P1)의 사전 조사. 지금 켜 둔 **Spawn Menu of MACE**(창작마당 3617250510, 제작 Septwnine)와 원래 **Spawn Menu**(3295913567, 제작 Rosemi)를 파일로만 조사했다. 게임은 띄우지 않았다. 이 문서는 손으로 고친다.

경로 약어:

- `SMM` = `E:\SteamLibrary\steamapps\workshop\content\400750\3617250510`
- `SM` = `...\400750\3295913567`
- `MACE` = `...\400750\2905667604`, `AM` = `...\400750\2829744930`(Auto Medic)
- pak 안의 파일은 `pak 경로!안쪽 경로`로 적는다.

## 요약

1. **스폰 메뉴는 미션 스크립트다.** 엔진의 미션 "증원(reinforcements)" 단추를 메뉴처럼 쓰고, 트리거가 단추마다 유닛을 만든다. Lua나 새 인터페이스 창은 없다.
2. **모든 맵의 `.mi`를 통째로 복사해 끝에 한 줄을 붙인다**: `(include "/map/cheat/cheat.mi")`. 캠페인 91개, 멀티 맵(컨퀘스트·스커미시 등) 389개.
3. **유닛 목록은 손으로 적은 것이다.** 차량은 정의 이름을 직접 부르고(`{"spawn" {entity "panzer3e"}}`), 보병은 맵 밖에 미리 놓아 둔 사람 755명을 복제한다. 차량 이름은 `cheat.mi` 665개 + MACE용 `mace.mi` 337개.
4. **MACE 지원은 파일 하나(`mace.mi`)를 더 include한 것**이다. Spawn Menu of MACE의 스크립트·화면 파일은 원래 Spawn Menu와 바이트까지 같다.
5. **지금 계층에서 충돌이 있다**: Spawn Menu of MACE가 맨 뒤에 로드되어 Auto Medic의 캠페인 미션 수정 70개와, MACE의 더 새 컨퀘스트 스크립트를 옛 사본으로 덮는다(아래 "지금 계층과의 충돌").

## 파일 구성

| 파일 | SMM | SM | 같은가 |
|---|---|---|---|
| `resource/set/interaction_entity/debug.inc` (위장 바꾸기) | 있음 | 있음 | 같음 |
| `resource/interface/layout/views/scene/SceneGameView.xml` | 있음 | 있음 | 같음 |
| `resource/interface/action/change_paint_scheme_toggle.set` + 단추 그림 4장, `sound/vehicle/paint/spray_paint.wav` | 있음 | 있음 | 같음 |
| `resource/entity/-vehicle/generic/special/inf_crate_<진영>/inf_crate_<진영>_weapon.def` 5개 | 있음 | 있음 | 같음 |
| `localizations/default/interface/text/mission/cheat.pot`, `ch/.../cheat.po` | 있음 | 있음 | 같음 |
| `resource/interface/scene/unit_icon/*.tga` (메뉴 단추 그림) | 62장 | 58장 | SMM에 4장 더 |
| `resource/set/panel_control/right_special.set` (위장 단추 배치) | **없음** | 있음 | — |
| `resource/map/cheat/cheat.mi` | 낱개 | `map.pak!map/cheat/cheat.mi` | 다름(SMM에 단추 20줄 등 430줄 차이, 47855행부터) |
| `resource/map/cheat/mace.mi` | 낱개 | 없음 | — |
| 맵 `.mi` 사본 | 낱개 `map/single/**/0.mi` 91개 + `map/multi.pak` 389개 | `map.pak` 안에 single 91 + multi 388 | single은 같음(표본 `01-rus/1941_11_typhoon/0.mi`) |
| `map/multi/*.inc` (컨퀘스트 스크립트 등) | `multi.pak` 안에 12개 | 없음 | — |

## 어떻게 붙는가: 맵 사본 + include 한 줄

- 캠페인 미션 사본의 맨 끝, `{mission ...}`의 닫는 괄호 바로 앞: `SMM\resource\map\single\00-bootcamp\01_inf-control\0.mi` 9727행 `(include "/map/cheat/cheat.mi")`.
- 바닐라와 비교: `map/single.pak`에 있는 17개 미션 가운데 16개는 바닐라 + 이 한 줄과 같다. `02-ger/1943_07_petrovka/0.mi`는 편집기로 다시 저장한 흔적(`{SpawnedInFog}`, 상태값)이 더 있다. DLC pak의 나머지 74개는 비교하지 않았다(미확인).
- 멀티 맵: `SMM\resource\map\multi.pak` 안의 `.mi` 389개 모두 같은 줄이 있다. 파일 이름별로 `battle_zones.mi` 118, `campaign_capture_the_flag.mi` 114(컨퀘스트 `dcg_*` 맵), `ammunition.mi` 111, `evacuation.mi` 29, `frontlines.mi` 16, `test.mi` 1.
- MACE가 고친 맵은 MACE 판 위에 한 줄을 붙였다: `multi.pak!multi/0_frontlines/1v1_barbarossa/frontlines.mi`는 `MACE\resource\map\multi.pak`의 같은 파일과 8715행 include 한 줄만 다르다.
- `cheat.mi` 1행이 `;{mission`(주석)이다. 즉 `cheat.mi`는 미션 블록 안에 끼워 넣는 조각이다. 127205행에서 `(include "/map/cheat/mace.mi")`로 MACE 목록을 이어 붙인다.

모드별로 메뉴가 생기는 곳(파일 구조로 본 것):

| 모드 | 메뉴가 붙는 파일 |
|---|---|
| 캠페인 | `map/single/**/0.mi` 91개 |
| 컨퀘스트 | `map/multi/dcg_*/campaign_capture_the_flag.mi` 114개 |
| 스커미시·기타 멀티 | `map/multi/**/battle_zones.mi`, `ammunition.mi`, `evacuation.mi`, `frontlines.mi`, `single/*_skirmish` 미션 |

사본이 없는 맵(새 DLC 맵, 다른 모드의 맵)에는 메뉴가 없다. 게임 패치가 맵 `.mi`를 바꾸면 사본이 옛 판으로 덮는다.

## `cheat.mi`의 구조

| 행 | 블록 | 내용 |
|---|---|---|
| 2–46864 | `{Human ...}` 755개, `{Entity ...}` 798개 | 견본. 보병은 `{Position -9639.63 9712.23}` 근처(맵 밖)에 `{Volume "ram" {able {visible 0}...}{disabled}}`로 숨겨 둔다. 사람마다 진영·역할 태그(`tkm`, `tkcom`, `"s1.2 1"`, `ger` 등)와 인벤토리가 있다 |
| 46865 | `{Helpers` | 미션 도우미 묶음 시작 |
| 46866–47787 | `{event "중형전차/eng/1.1"}` 등 922개 | 단추마다 이벤트 이름 |
| 47788–47803 | `{zone "poly" ...}` 2개 | `infantryzone`(위치 77777), `tankzone`(88888). 맵 밖의 대기 구역 |
| 47804–59325 | `{reinforcements` | 메뉴 단추. 맨 위 `"spawn menu"`(`{state "-"}`, 그림 `主页`)가 메인 메뉴를 연다. 나머지는 `{state hidden}`으로 숨겨 두고 페이지를 바꿀 때 보인다 |
| 59326–59336 | `{vars` | `set player`, `set ai`, `以最高等级生成单位`(최고 등급으로 생성), 위장 변수 등 |
| 59337–63409 | `{functions` | `"vk c"`(59338, 생성 뒤 처리), `"set player"`(59614), `"ai"`(60380) 등 |
| 63410–127152 | `{triggers` | 단추마다 트리거 하나 |
| 127153–127204 | `{waypoints "cheat_waypoint"` 등 | `6997`(77777, 보병 대기), `6998`(88888, 차량 대기), `6999`(놓을 자리), `7000`, `plane/8000` |

### 단추 하나의 흐름 (예: 독일 3호 전차 E형)

1. 단추 `cheat.mi` 51909–51920행:
   `{"ger_中型坦克1_6.1" {state hidden} {text "desc/entity/panzer3e"} {image panzer3e} {hint "desc/entity/panzer3e"} {count -1} {eventStart "中型坦克/ger/6.1"} {waypoint "6999"} {previewEntity pointer_green}}`.
   이름·그림은 게임의 유닛 설명(`desc/entity/<이름>`)과 아이콘을 그대로 쓴다. `{waypoint "6999"}`와 `{previewEntity ...}`로 놓을 자리를 고르게 한다(추정: 바닐라 증원 단추와 같은 방식).
2. 트리거 `cheat.mi` 72434–72514행: 이벤트 `中型坦克/ger/6.1`이 오면
   - 견본 승무원(태그 `tkm` 4명, `tkcom` 1명, 진영 `ger`, `un` 제외)을 `6997`로 `{clone}` `{approach teleport}`,
   - `{"spawn" {entity "panzer3e"} {waypoint "6998"}}`로 전차를 대기 구역에 만들고,
   - 함수 `"vk c"`를 부른 뒤 이벤트를 되돌린다(`{mode reset}`).
3. 함수 `"vk c"`(59338–): 대기 구역의 차량에 `vk`, 사람에 `sc` 태그 → 변수 `set player`/`set ai`에 따라 소속(`autoassign`)·AI 조종(`actor_state {control user}`)을 정함 → `{"placement" ... {target_waypoint "6999"}}`로 고른 자리에 옮김 → `"linker"`로 승무원을 태운다.
4. 웨이포인트 `6999`(127162–127193행)의 명령: 변수 `以最高等级生成单位`가 1이면 `{"actor_state" {human {veterancy 8}}}`.

### 보병 분대

- 단추 예 48680–48692행: `{text "desc/squad/squad_ersatz_gren_late_all(ger)"}`, `{eventStart "步兵小队/ger/1.1"}`.
- 트리거 예 63623–63772행(`步兵小队/ger/1.2`): 태그 `"s1.2 1"` 1명, `"s1.2 2"` 4명, `"s1.2 3"` 3명, `"s1.2 4"` 2명 … 을 복제한다. 분대 구성(인원·무기)은 제작자가 견본 사람으로 직접 만든 것이다.
- 사람 견본의 병종은 `mp/<진영>/<시기>/<역할>` 꼴 693종(`{Human "mp/ger/late/mgun_1"}` 등).
- 미션 트리거의 `"spawn"`으로 병종 경로를 바로 만들 수 있는지는 **미확인**. 바닐라와 세 모드의 `.mi`·`.inc`에서 `"spawn"`의 `{entity}` 값은 모두 차량·효과 이름이었다(1,344회). 보병은 `"squad_create"`가 이미 있는 사람을 묶을 뿐이다.

### 메뉴 분류 (단추의 `eventStart` 앞부분)

| 분류 | `cheat.mi` | `mace.mi` |
|---|---|---|
| 菜单(메뉴 이동) | 63 | — |
| 步兵小队(보병 분대) | 94 | — |
| 步兵(보병 낱개, 화면 이름 "Career Infantry") | 91 | — |
| 架设(거치 화기·포) | 186 | 69 |
| 载具(차량) | 164 | 54 |
| 轻型坦克(경전차) | 79 | 43 |
| 中型坦克(중형전차) | 123 | 86 |
| 重型坦克(중전차) | 42 | 56 |
| 歼击坦克(구축전차) | 56 | 31 |
| 飞机(항공기) | 52 | 16 |
| 删除(삭제·최대 강화·최고 등급 켜기·잔해 정리) | 7 | — |
| 迷彩(위장) | 2 | — |

### 차량 목록이 덮는 범위

편집기 색인(`units --kind tank|cannon|car`, 이긴 판 기준)과 `"spawn"` 이름 1,000개(대소문자 무시)를 맞춰 본 것:

| 계층 | 전차·포·차량 정의 수 | 메뉴에 있는 수 |
|---|---|---|
| 바닐라 | 758 | 625 |
| MACE(2905667604) | 403 | 341 |
| MACE Wunderwaffe(3732639687) | 204 | **0** |
| Auto Medic(2829744930) | 10 | 8 |
| Spawn Menu of MACE 자신(보급 상자) | 5 | 5 |

메뉴 이름 21개는 색인에 없다(포격 더미 `conquest_artillery_barrage_*` 15개, `comet`, `cflammpanzer_b2_fra`, `type_94_tk`, `new_unit`, `paratrooper_attach` 등). 보병은 견본 사람으로만 만들므로 모드 보병(MACE·Wunderwaffe 병종)은 거의 없다(`mace.mi`의 사람 27명은 승무원·조종사).

## 화면 쪽 변경

- `SceneGameView.xml`: 바닐라(`resource/interface/interface.pak!layout/views/scene/SceneGameView.xml`)와 비교하면 증원 대화상자 `gui_gc_reinforcement`를 세 곳에서 더 켠다. 204행 `enableif="gameclient.master"` 추가, 212행 `enableif="gui.editor or game.singleplayer or gameclient.master"`, 219행 조건 없이 하나 더. 추정: 컨퀘스트·스커미시(멀티 구조로 도는 판)에서도 증원 단추 줄이 보이게 하려는 것.
- 위장 바꾸기: `debug.inc` 63–226행이 차량·사람의 `change_paint_scheme_toggle` 이벤트에 텍스처 이름 순환(`tex_mod`)을 건다. 단추 배치는 SM의 `right_special.set` 78–81행(사람)·106–109행(차량)에만 있다. SMM에는 이 파일이 없고 지금 계층에서 이기는 `right_special.set`(Auto Medic 판)에는 `change_paint` 줄이 0개다. 파일로 보면 지금 위장 단추는 조작 패널에 배치되지 않는다. 화면에 보이는지는 **미확인**.
- 보급 상자 `inf_crate_<진영>_weapon.def` 5개: 그 진영 무기를 `{item "k98" 99 filled {cell 0 0}}` 꼴로 채운 상자. 메뉴에서 `inf_crate_ger`, `inf_crate_ger_weapon` 등 10개를 만든다.

## 지금 계층과의 충돌

로드 순서는 바닐라 → Wunderwaffe → MACE → Auto Medic → Spawn Menu of MACE → CTAToybox 출력 모드. 같은 경로는 나중 모드가 이긴다(`docs/07-stat-editor/ingame-check.md` 47·79·99행).

| 겹치는 파일 | 누가 이기나 | 잃는 것 |
|---|---|---|
| 캠페인 미션 `map/single/**/0.mi` 70개 (AM `map.pak`과 SMM 낱개) | SMM | Auto Medic 판에만 있는 `(include "/map/nikral's trigger.mi")` 줄(예: `AM map.pak!map/single/01-rus/1941_11_typhoon/0.mi` 9814행). SMM 사본에는 없다 |
| `map/multi/dcg_script.inc` (컨퀘스트 스크립트) | SMM | SMM 판 439,491바이트(파일 날짜 2026-07-23)가 MACE 판 475,479바이트(2026-09-26)와 AM 판 476,906바이트를 덮는다. MACE 판 앞부분의 `dcg/supply_building_register` 트리거 등이 SMM 판에 없다 |
| `map/multi/mace_scripts.inc` | SMM | SMM 판 66,710바이트(2026-07-29)가 MACE 판 150,949바이트(2026-09-26)를 덮는다 |
| `map/multi/dcg_vars.inc` | SMM | MACE 판과 다르다 |
| `map/multi/0_frontlines/*_script.inc` | SMM | MACE 판과 같다. AM 판(`1v1_script.inc` 등)은 덮인다 |

이 충돌이 게임에서 무엇을 깨는지는 **미확인**(게임 안 확인은 하지 않는다).

## 미확인으로 남긴 것

- 미션 `"spawn"`으로 보병 병종을 바로 만들 수 있는지.
- `{waypoint "6999"}` + `{previewEntity}` 단추가 놓을 자리를 고르게 하는 정확한 방식(바닐라 증원 단추와 같은지).
- 컨퀘스트·스커미시에서 `SceneGameView.xml` 변경이 꼭 필요한지.
- DLC pak에 있는 캠페인 미션 74개가 바닐라 + 한 줄인지.
- 위 충돌이 게임에서 일으키는 일.
