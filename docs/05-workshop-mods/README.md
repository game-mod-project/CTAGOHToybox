# 창작마당 모드 분석

이 파일은 `tools/New-ModDoc.ps1`이 생성한다. 앞뒤의 설명은 `data/mods/readme-intro.md`와 `readme-outro.md`에서 고친다.

## 대상과 확보 방법

프로필의 `options.set`에 등록돼 있던 모드 네 개를 분석했다.

| ID | 이름 | 문서 |
|---|---|---|
| 2905667604 | MACE | [2905667604.md](2905667604.md) |
| 3732639687 | MACE Wunderwaffe Ultimate Fusion ENG | [3732639687.md](3732639687.md) |
| 3295913567 | Spawn Menu | [3295913567.md](3295913567.md) |
| 2829744930 | Auto Medic-AA-Artillery & Weather Switch | [2829744930.md](2829744930.md) |

- 파일은 2026-10-11에 Steam 구독으로 받았다(`steamapps\workshop\content\400750`). steamcmd 익명 다운로드는 거부됐다.
- 바닐라와 같은 도구로 분석했다. 모드의 `resource` 폴더를 추출하고, 파싱하고, 유닛을 펼친 뒤 바닐라의 결과와 차집합을 냈다.
- 네 모드 모두 `mod.info`의 버전 필드가 `"any"`이고 `require`가 없다. 파일만으로는 대상 게임 버전과 의존 관계를 알 수 없어서, 의존 관계는 창작마당 설명과 파일의 겹침으로 판단했다.
- 모드 원본 파일은 저장소에 넣지 않았다. 저장소에는 집계한 데이터(`data/mods/<ID>/`)만 있다.

### 읽는 법

- **새 파일 / 바뀐 파일**은 가상 경로가 바닐라에 있는지, 내용(SHA-1)이 같은지로 나눈 것이다.
- **새 유닛**은 바닐라에 없는 경로의 유닛 파일이다. 차량류는 `.def` 파일 수이며 파괴 모델용 `.def`가 포함돼 있어 실제 차량 수보다 많다.
- **바닐라에 없는 이름**이 "모드가 추가한 구성"에 가장 가까운 수치다. 블록 이름, 매크로 이름이 바닐라의 어떤 파일에도 나오지 않는 것만 센다.
- **바닐라에 없는 따옴표 이름**은 유닛 이름처럼 이름 자체가 데이터인 자리에 모드가 넣은 이름이다. 새 유닛을 편제표나 연구 트리에 등록한 흔적이 여기에 나온다.
- **바닐라에 없는 키**는 더 넓다. 이름은 바닐라에 있어도 그 경로와 파일 종류의 조합이 처음이면 포함된다. 새 매크로 안에 기존 키를 넣기만 해도 늘어난다.

## 한눈에 보기

| 모드 | 새 파일 | 내용이 바뀐 바닐라 파일 | 새 보병 | 새 차량류 .def | 새 아이템 | 바뀐 바닐라 유닛 파일 | 새 블록 이름 | 새 매크로 이름(정의+호출 행) | 바닐라에 없는 키 |
|---|---|---|---|---|---|---|---|---|---|
| [MACE](2905667604.md) | 113376 | 136 | 1100 | 1307 | 971 | 51 | 30 | 419 | 2343 |
| [MACE Wunderwaffe Ultimate Fusion ENG](3732639687.md) | 61632 | 6564 | 0 | 528 | 437 | 44 | 38 | 241 | 1626 |
| [Spawn Menu](3295913567.md) | 70 | 2 | 0 | 5 | 0 | 0 | 2 | 2 | 15 |
| [Auto Medic-AA-Artillery & Weather Switch](2829744930.md) | 168 | 51 | 0 | 0 | 0 | 28 | 6 | 0 | 25 |

## 모드 간 관계

둘 이상의 모드가 같은 가상 경로에 가진 텍스트 파일은 5528개이고 그중 5486개는 내용이 다르다. 전체는 `data/mods/cross-mod-conflicts.csv`에 있다.

| 모드 조합 | 내용이 다른 파일 | 내용이 같은 파일 |
|---|---|---|
| MACE + MACE Wunderwaffe Ultimate Fusion ENG | 5479 | 42 |
| Auto Medic-AA-Artillery & Weather Switch + MACE | 5 | 0 |
| Auto Medic-AA-Artillery & Weather Switch + MACE + Spawn Menu | 1 | 0 |
| Auto Medic-AA-Artillery & Weather Switch + MACE Wunderwaffe Ultimate Fusion ENG | 1 | 0 |

MACE와 Fusion 사이를 뺀 나머지 겹침:

- `set/panel_control/right_main.set`: Auto Medic-AA-Artillery & Weather Switch, MACE
- `set/panel_control/right_special.set`: Auto Medic-AA-Artillery & Weather Switch, MACE, Spawn Menu
- `set/stuff/shell/bulletger_105.heat.ammo`: Auto Medic-AA-Artillery & Weather Switch, MACE
- `set/stuff/special/searchlight`: Auto Medic-AA-Artillery & Weather Switch, MACE Wunderwaffe Ultimate Fusion ENG
- `set/target/ammo.set`: Auto Medic-AA-Artillery & Weather Switch, MACE
- `set/vision/settings.set`: Auto Medic-AA-Artillery & Weather Switch, MACE
- `set/vision/spotting.inc`: Auto Medic-AA-Artillery & Weather Switch, MACE

## 바닐라에 없는 블록 이름 전체

네 모드가 들여온 블록 이름을 합친 것이다. 매크로 이름은 모드별 문서에 있다.

| 이름 | 쓰인 경로(예) | 모드 | 등급 | 의미 |
|---|---|---|---|---|
| `active_time` | `/game_entity/extender[vision_mode]/timer_object/active_time` | MACE, MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 기능이 켜져 있는 시간 (추정: 이름. 단위 미확인. 값은 60과 1000) |
| `ai_auto_move` | `/panel_control/ai_auto_move` | Auto Medic-AA-Artillery & Weather Switch | 추정 | 조작 패널에 추가된 버튼 정의. AI 제어 (추정: 모드 설명의 AI 제어 버튼과 이름이 맞음) |
| `applyprojectile` | `/bullet/applyprojectile` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | bullet 아래의 인자 없는 블록. V-1 로켓 탄약에 쓰임 |
| `bitmap` | `/bitmap` | MACE | 미확인 | 아이템 엔티티 폴더의 재질 파일에 나오는 최상위 블록 |
| `cell` | `/actor/extender[inventory]/box/item/cell` | Spawn Menu | 추정 | 인벤토리 안에서 아이템이 놓이는 칸의 위치 (추정: box/item의 하위이고 값이 두 수) |
| `change_paint_scheme_toggle` | `/panel_control/change_paint_scheme_toggle` | Spawn Menu | 추정 | 조작 패널에 추가된 버튼 정의. 위장 전환 (추정: 모드 설명의 위장 전환 기능과 이름이 맞음) |
| `deploy_gas` | `/panel_control/deploy_gas` | MACE | 추정 | 조작 패널에 추가된 버튼 정의 (추정: set/panel_control의 파일에 있고 이름이 동작을 가리킴) |
| `detail2` | `/game_entity/extender[detail]/detail2` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | extender "detail"의 하위. 값 1 |
| `detail3` | `/game_entity/extender[detail]/detail3` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | extender "detail"의 하위. 값 1 |
| `detail4` | `/game_entity/extender[detail]/detail4` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | extender "detail"의 하위. 값 1 |
| `detail5` | `/game_entity/extender[detail]/detail5` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | extender "detail"의 하위. 값 1 |
| `detail6` | `/game_entity/extender[detail]/detail6` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | extender "detail"의 하위. 값 1 |
| `detonate_wanze_charge` | `/panel_control/detonate_wanze_charge` | MACE | 추정 | 조작 패널에 추가된 버튼 정의 (추정: 위와 같음) |
| `durability` | `/armor/setting/durability` | MACE, MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | armor/setting 아래의 인자 없는 블록. 하위 내용을 살펴보지 않았다 |
| `enable_for_human_user` | `/select_ammo/tank_heavy/"*"/enable_for_human_user` | MACE | 추정 | 탄 자동 선택 규칙을 사람이 조종하는 유닛에도 적용할지 여부 (추정: set/target/ammo.set의 select_ammo 아래에 있고 값이 0) |
| `extender[vision_mode]` | `/game_entity/extender[vision_mode]` | MACE, MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 바닐라에 없는 종류의 extender. 이름으로는 시야 모드 기능이다 (추정: 하위에 timer_object/active_time/recharge_rate가 있어 일정 시간 켜지고 재충전되는 기능으로 보임. 실제 동작은 미확인) |
| `filtering` | `/bitmap/filtering` | MACE | 미확인 | bitmap의 하위. 값 1 |
| `firing_binocular` | `/(define "firing")/firing_binocular` | Auto Medic-AA-Artillery & Weather Switch | 추정 | 쌍안경 사용 시의 발각·소음 항목 (추정: spotting.inc와 hearing.inc의 정의 안에 있고 settings.set의 terms에 firing "binocular"로 등록됨) |
| `firing_gev_m40_scope` | `/(define "firing")/firing_gev_m40_scope` | MACE | 추정 | 발사할 때의 발각 거리 항목 (추정: set/vision/spotting 계열 파일의 "firing" 정의 안에 있고 값이 100) |
| `firing_searchlight` | `/(define "firing")/firing_searchlight` | Auto Medic-AA-Artillery & Weather Switch | 추정 | 탐조등 사용 시의 발각 항목. 값 300 (추정: 위와 같은 자리. 모드 설명의 "탐조등 빛을 적이 알아챈다"와 맞음) |
| `firing_type97_scope` | `/(define "firing")/firing_type97_scope` | MACE | 추정 | 발사할 때의 발각 거리 항목 (추정: 위와 같음) |
| `firing_type97_scope_flash_hider` | `/(define "firing")/firing_type97_scope_flash_hider` | MACE | 추정 | 발사할 때의 발각 거리 항목 (추정: 위와 같음) |
| `firing_type97_scope_silencer` | `/(define "firing")/firing_type97_scope_silencer` | MACE | 추정 | 발사할 때의 발각 거리 항목 (추정: 위와 같음) |
| `firing_type99_early_scope` | `/(define "firing")/firing_type99_early_scope` | MACE | 추정 | 발사할 때의 발각 거리 항목 (추정: 위와 같음) |
| `firing_type99_long_scope` | `/(define "firing")/firing_type99_long_scope` | MACE | 추정 | 발사할 때의 발각 거리 항목 (추정: 위와 같음) |
| `firing_type99_mid_scope` | `/(define "firing")/firing_type99_mid_scope` | MACE | 추정 | 발사할 때의 발각 거리 항목 (추정: 위와 같음) |
| `fra_mid` | `/library/soldiers/fra_mid` | MACE | 추정 | 라이브러리의 병사 분류 등록. 프랑스 중기 (추정: set/registry/library.reg의 soldiers 아래) |
| `hide_hitmarker` | `/"*"/level/hide_hitmarker` | MACE, MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 3인칭 조종에서 명중 표시를 숨김 (추정: set/third_person의 파일에 있고 값 1) |
| `jap_late` | `/library/soldiers/jap_late` | MACE | 추정 | 라이브러리의 병사 분류 등록. 일본 후기 (추정: 위와 같음) |
| `jap_type21_buildable` | `/panel_control/jap_type21_buildable` | MACE | 추정 | 조작 패널의 건설 메뉴에 추가된 항목 (추정: right_emplacement.set에 있음) |
| `littledavid` | `/panel_control/littledavid` | MACE | 추정 | 조작 패널의 건설 메뉴에 추가된 항목 (추정: 위와 같음) |
| `look_around` | `/game_entity/brain/properties/look_around` | Auto Medic-AA-Artillery & Weather Switch | 미확인 | brain/properties 아래의 인자 없는 플래그point_1_6\|추정\|동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `medic_switch` | `/panel_control/medic_switch` | Auto Medic-AA-Artillery & Weather Switch | 추정 | 조작 패널에 추가된 버튼 정의. 자동 위생병 켜고 끄기 (추정: 모드 설명과 이름이 맞음) |
| `nolight` | `/material/nolight` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | 재질 파일의 인자 없는 플래그 |
| `ostfront_talvisota_west` | `/regions/ostfront_talvisota_west` | MACE, MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인의 지역 이름 (추정: values.set과 map_points.set의 regions 아래) |
| `plane_03` | `/(define "spot_vehicle")/plane_03` | MACE | 추정 | 차량 발각 거리 표의 항목. 값 3700 (추정: spotting.inc의 "spot_vehicle" 정의 안) |
| `point_1_6` | `/duration/unlimited/landscape/point_1_6` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_2_6` | `/duration/unlimited/landscape/point_2_6` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_4_6` | `/duration/verylong/landscape/point_4_6` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_4_7` | `/duration/verylong/landscape/point_4_7` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_5_5` | `/duration/verylong/landscape/point_5_5` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_5_6` | `/duration/verylong/landscape/point_5_6` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_5_7` | `/duration/verylong/landscape/point_5_7` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_6_4` | `/duration/verylong/landscape/point_6_4` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_6_5` | `/duration/verylong/landscape/point_6_5` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_6_6` | `/duration/verylong/landscape/point_6_6` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_6_7` | `/duration/verylong/landscape/point_6_7` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_7_1` | `/duration/verylong/landscape/point_7_1` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_7_2` | `/duration/verylong/landscape/point_7_2` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_7_3` | `/duration/verylong/landscape/point_7_3` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_7_4` | `/duration/verylong/landscape/point_7_4` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_7_5` | `/duration/verylong/landscape/point_7_5` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_8_1` | `/duration/verylong/landscape/point_8_1` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_8_2` | `/duration/verylong/landscape/point_8_2` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_8_3` | `/duration/verylong/landscape/point_8_3` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_8_4` | `/duration/verylong/landscape/point_8_4` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_9_1` | `/duration/verylong/landscape/point_9_1` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_9_2` | `/duration/verylong/landscape/point_9_2` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `point_9_3` | `/duration/verylong/landscape/point_9_3` | MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 동적 캠페인 지도의 지점. 지형 종류와 좌표 두 값 (추정: map_points.set의 landscape 아래. 바닐라에도 같은 형식의 point_ 항목이 있고 이 번호만 없음) |
| `projectiledamage` | `/from/projectiledamage` | MACE | 미확인 | 값 1300. 바닐라의 projectiledamagetable과 이름이 비슷하다 |
| `recharge_rate` | `/game_entity/extender[vision_mode]/timer_object/recharge_rate` | MACE, MACE Wunderwaffe Ultimate Fusion ENG | 추정 | 재충전 속도 (추정: 이름. 값은 1과 1000) |
| `recovery` | `/panel_control/recovery` | MACE | 추정 | 조작 패널에 추가된 버튼 정의 (추정: 위와 같음) |
| `smine_decoy` | `/panel_control/smine_decoy` | MACE | 추정 | 조작 패널에 추가된 버튼 정의 (추정: 위와 같음) |
| `speed2=28.8` | `/skeleton/bone/bone/bone/speed2=28.8` | MACE | 미확인 | 모델 파일에 `speed2=28.8`이 한 낱말로 적혀 있다. 바닐라는 `{speed2 28.8}` 형태라 오타로 보인다 |
| `swe_gun_buildable` | `/panel_control/swe_gun_buildable` | MACE | 추정 | 조작 패널의 건설 메뉴에 추가된 항목 (추정: 위와 같음) |
| `timer_object` | `/game_entity/extender[vision_mode]/timer_object` | MACE, MACE Wunderwaffe Ultimate Fusion ENG | 추정 | vision_mode의 타이머 설정 묶음 (추정: 하위 키) |
| `toggle_back_mg` | `/panel_control/toggle_back_mg` | MACE | 추정 | 조작 패널에 추가된 버튼 정의 (추정: 위와 같음) |
| `weather_switch` | `/panel_control/weather_switch` | Auto Medic-AA-Artillery & Weather Switch | 추정 | 조작 패널에 추가된 버튼 정의. 날씨 전환 (추정: 모드 설명과 이름이 맞음) |
| `whole_wound` | `/(define "armor_jagdpanzer")/armor/setting/whole_wound` | MACE Wunderwaffe Ultimate Fusion ENG | 미확인 | armor/setting 아래의 인자 없는 블록 |

## 요약

- **MACE**는 유닛을 대량으로 추가한다(보병 1,100, 차량류 `.def` 1,307, 아이템 971). 일본 진영이 가장 크다. 그러나 유닛을 이루는 키는 거의 전부 바닐라의 것이다. 바닐라에 없는 블록 이름은 30종이고 그 절반은 조작 패널 버튼과 발각 항목이다.
- **Fusion**은 MACE 위에 차량을 더한다(MACE에 없는 `.def` 511개). MACE의 파일 5,479개를 다른 내용으로 덮어쓴다. `extender "vision_mode"`를 34개 차량에 쓴다.
- **Spawn Menu**는 맵 파일을 덮어써 메뉴를 넣는다. 유닛 구성은 바꾸지 않는다.
- **Auto Medic**은 바닐라의 표적 규칙·무기·차량 파일 51개를 고치고 날씨 설정을 추가한다.
- 네 모드를 합쳐도 바닐라에 없는 블록 이름은 위 표가 전부다. 새 유닛을 만드는 데 필요한 구성은 `docs/04-units`의 바닐라 스키마로 거의 다 설명된다.

## 한계

- **바이너리 자산은 개수와 경로만 다뤘다.** 모델, 텍스처, 소리가 모드 용량의 대부분이지만 내용을 보지 않았다.
- **범위 밖 폴더의 정의 파일은 세지 않았다.** MACE는 `entity/service`, `entity/construction` 등에 83개, Fusion은 `entity/+inventory`에 66개의 정의 파일을 둔다. 모드별 문서의 "분석 범위 밖에 놓인 정의 파일"에 폴더별 수가 있다.
- **맵과 미션 파일은 범위 밖이다.** Spawn Menu와 Auto Medic의 핵심 기능이 여기에 있다.
- **Lua 스크립트는 목록만 있다.** 바뀐 스크립트의 내용은 비교하지 않았다.
- **펼치기 규칙은 추론이다.** `docs/04-units/README.md`의 "한계"가 그대로 적용된다. 모드 사이의 include 순서(Fusion → MACE → 바닐라)도 추정이다.
- **로드 순서에 따른 최종 결과는 계산하지 않았다.** 각 모드를 바닐라와 따로 비교했다. 게임이 어느 것을 쓰는지는 이후 확인됐다: 같은 경로면 `options.set`의 모드 목록에서 뒤에 있는 모드가 이기고, 한 모드 안에서는 낱개 파일이 pak을 이긴다(2026-10-13, `docs/07-stat-editor/ingame-check.md`). 이 규칙으로 최종 결과를 계산하는 기능은 아직 없다.
- **게임 버전.** 바닐라 기준은 1.0.6.7이다. 모드가 그보다 앞선 버전의 바닐라 파일을 복사해 고친 경우, "내용이 바뀐 파일"에는 모드의 의도와 무관한 게임 업데이트 차이가 섞여 있을 수 있다.
- **새 이름의 뜻은 대부분 추정이다.** 모드 제작자의 문서를 찾지 못했고 이름과 쓰인 자리로 판단했다. 매크로 이름은 살펴보지 않았다.
