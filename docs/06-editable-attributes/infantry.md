# 수정 가능한 속성: 보병

이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다. 설명은 `data/annotations/common.psv`에서 고친다. 표 읽는 법은 [README.md](README.md)에 있다.

보병 하나는 `set/breed` 아래의 `.set` 파일 하나다. 능력치는 본체에 직접 적히지 않고 `perks`와 `veterancy` 안에서 매크로로 불러오므로, 능력치를 바꾸려면 연관 파일의 매크로 본문을 고치거나 본체의 매크로 호출 인자를 고친다. 들고 있는 무기와 장구의 속성은 [weapon-ammo.md](weapon-ammo.md)에 있다.

| 구분 | 파일 | 위치 | 속성 수 | 뜻을 아는 속성 |
|---|---|---|---|---|
| 본체 | 보병 정의 | `set/breed/**/*.set` | 16 | 13 |
| 연관 | 능력치 매크로 | `set/breed/**/*.inc` | 91 | 0 |
| 연관 | 외형 엔티티 | `entity/humanskin/**/*.def` | 18 | 8 |

## 본체: 보병 정의

- 위치: `set/breed/**/*.set`
- 연결: 유닛 그 자체. 파일 하나가 보병 하나

### 뜻을 아는 속성 (13개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/breed/armors/head` | 예: civilian_dockyard_cap (13종 이상) | **추정** 머리 장구 아이템 이름. set/stuff/head의 파일명과 일치한다 (추정: 참조 색인) |  | 5007 | `set/breed/civ/civ_fra_1.set` |
| `/breed/behaviour` | soldier | **추정** 행동 유형. 값은 soldier 등 (추정: 이름과 값) |  | 5083 | `set/breed/civ/civ_fra_1.set` |
| `/breed/icon` | 예: "bazooka" (13종 이상) | **추정** 유닛 아이콘 이름. 주무장 종류로 붙어 있다: smg, autorifle, bazooka 등 (추정: 값) |  | 5044 | `set/breed/mp/can/late/antitank_piat.set` |
| `/breed/icon_priority` | 0 ~ 52 | **추정** 아이콘 표시 우선순위 숫자 (추정: 이름. 정렬 방향은 미확인) |  | 5044 | `set/breed/mp/can/late/antitank_piat.set` |
| `/breed/inventory/in_hands` | 0 ~ 4 | **추정** 스폰 때 손에 드는 아이템의 순번 (추정: 이름과 값 0~. 기준이 0부터인지는 미확인) |  | 4835 | `set/breed/mp/can/late/antitank_piat.set` |
| `/breed/inventory/item` | 예: "piat" filling "piat_bomb ammo heata" 1 (13종 이상) | **추정** 소지품 한 줄. 첫 문자열이 아이템 이름, filling 뒤는 장전된 탄과 수량, 끝의 수는 개수 (추정: 값 형태. 수가 둘일 때 둘째 수의 뜻은 미확인) | 128+8 ¦ 80+48+8 | 4970 | `set/breed/mp/can/late/antitank_piat.set` |
| `/breed/nationality` | 예: fra_civilian (13종 이상) | **추정** 소속 국가 코드. usa, ger, rus, eng, fin 등 (추정: 이름과 값) |  | 5083 | `set/breed/civ/civ_fra_1.set` |
| `/breed/perks/"*"` | 예: "rocketlauncher" lvl_1 (13종 이상) | **추정** 기술 이름과 수준. 예: "rifle" lvl_5. 이름은 인자로 남아 있다 (추정: set/breed/presets/perks/skills.inc의 매크로가 이 형태를 만든다) |  | 62 | `set/breed/mp/ned/late/resistance_medic.set` |
| `/breed/portrait` | 예: "civilian2" (13종 이상) | **추정** 인터페이스에 표시할 초상화 이름 (추정: 이름과 값이 "usa_1st_special" 같은 식별자) |  | 5083 | `set/breed/civ/civ_fra_1.set` |
| `/breed/skin` | 예: "civ_01_var1" (13종 이상) | **추정** 외형 엔티티 이름. entity/humanskin의 .def 파일명과 일치한다 (추정: 참조 색인에서 entity로 연결됨) | m41 uniform, haversack, rifle pouch ¦ m41 uniform, haversack, rifle pouch, general purpose bag | 5083 | `set/breed/civ/civ_fra_1.set` |
| `/breed/tags` | "soldier" ¦ "crew" ¦ "soldier_bazooka" ¦ "soldier_pzfaust" ¦ "soldier_atr" ¦ "soldier_atr_grenade" ¦ "soldier_pzscheck" ¦ "" | **추정** 유닛 분류 태그 문자열 (추정: 값 "soldier", "soldier_bazooka") |  | 4380 | `set/breed/mp/can/late/antitank_piat.set` |
| `/breed/veterancy/"*"` | 예: "lvl_1" 4 (13종 이상) | **추정** 숙련도 단계 이름과 그 단계에 필요한 값. 예: "lvl_1" 4 (추정: veterancy.inc의 주석에 단계별 숫자가 적혀 있으나 단위는 미확인) |  | 22 | `set/breed/mp/ned/late/resistance_medic.set` |
| `/breed/voice` | eng ¦ eng_pol ¦ usa_officer ¦ eng_officer ¦ usa_sniper ¦ usa_medic ¦ ger_officer ¦ eng_medic ¦ ger_sniper ¦ ger_medic ¦ usa_speirs ¦ usa_winters | **추정** 음성 세트 국가 코드 (추정: 값 eng 등) |  | 420 | `set/breed/mp/can/late/antitank_piat.set` |

### 뜻을 모르는 속성 (3개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/breed/field_icon` | "rus_miner_ap" ¦ "rus_miner_at" ¦ "usa_engineer_ap" ¦ "usa_engineer_at" ¦ "rus_pilot" ¦ "border_jag_ap" ¦ "border_jag_at" ¦ "ger_miner_ap" ¦ "ger_miner_at" ¦ "rus_artilleryman" | 미확인 |  | 21 | `set/breed/mp/fin/mid/border_jag_ap_mine.set` |
| `/breed/perks/"*"/factor` | 1 | 미확인 |  | 43 | `set/breed/mp/ned/late/resistance_medic.set` |
| `/breed/perks/"*"/fortifications` | "foxhole" "medic_rally" ¦ "foxhole" "medic_rally" "auto_medic_place_flag" | 미확인 |  | 43 | `set/breed/mp/ned/late/resistance_medic.set` |

## 연관: 능력치 매크로

- 위치: `set/breed/**/*.inc`
- 연결: 본체의 `perks`, `veterancy` 안에서 `(include)`와 매크로 호출로 불러온다. 폴더의 `ability.inc`가 `set/breed/presets/perks/`의 공용 정의를 include한다

### 뜻을 모르는 속성 (91개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/(define "at_rank_1")/"*"` | "rocketlauncher" lvl_1 ¦ "rifle" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "at_rank_2")/"*"` | "rocketlauncher" lvl_2 ¦ "rifle" lvl_2 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "at_rank_3")/"*"` | "rocketlauncher" lvl_3 ¦ "rifle" lvl_3 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "at_rank_4")/"*"` | "rocketlauncher" lvl_4 ¦ "rifle" lvl_4 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "at_rank_5")/"*"` | "rocketlauncher" lvl_5 ¦ "rifle" lvl_5 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "at_rank_6")/"*"` | "rocketlauncher" lvl_6 ¦ "rifle" lvl_6 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "basic_gun")/"*"` | "tank_gun" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "basic_repair")/mechanic` | factor 0.5 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "binocular_vision")/"*"` | "field_glass" ¦ "vision" human_elite | 미확인 |  | 1 | `set/breed/presets/perks/vision.inc` |
| `/(define "civilian")/"*"` | "health" "100%" ¦ "health_regeneration" "10%" ¦ "stamina" "75%" ¦ "stamina_regeneration" "75%" ¦ "speed" 0.90 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "elite_gun")/"*"` | "tank_gun" lvl_6 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "elite_vision")/"*"` | "vision" human_elite | 미확인 |  | 1 | `set/breed/presets/perks/vision.inc` |
| `/(define "engineer_heavy_fin")/"*"` | "resources" "1000%" ¦ "resources_regeneration" "0%" | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "engineer_heavy_fin")/barricade/"*"` | 예: "fortifications" "barricade_sq" (13종 이상) | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "engineer_heavy_fin")/mechanic` | factor 1.0 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "engineer")/"*"` | "resources" "1000%" ¦ "resources_regeneration" "0%" | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "engineer")/barricade/"*"` | "fortifications" "barricade_sq" ¦ "fortifications" "barricade" ¦ "fortifications" "barbwire" ¦ "fortifications" "hedgehog" ¦ "fortifications" "foxhole" ¦ "fortifications" "foxhole_big" ¦ "fortifications" "mortar_pit" ¦ "fortifications" "tank_cover" ¦ "fortifications" "heavy_mg_mg34" ¦ "fortifications" "heavy_mg_maxim" ¦ "fortifications" "heavy_mg_maxim_m1910_30" ¦ "fortifications" "heavy_mg_m1917_30cal" | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "engineer")/mechanic` | factor 1.0 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "expert_gun")/"*"` | "tank_gun" lvl_4 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "fortification")/"*"` | "resources" "10%" | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "fortification")/barricade/"*"` | "fortifications" "foxhole" ¦ "fortifications" "medic_rally" | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_rank_1")/"*"` | "pistol" lvl_2 ¦ "smg" lvl_1 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_2 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_rank_2")/"*"` | "pistol" lvl_3 ¦ "smg" lvl_2 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_3 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_rank_3")/"*"` | "pistol" lvl_4 ¦ "smg" lvl_3 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_4 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_rank_4")/"*"` | "pistol" lvl_5 ¦ "smg" lvl_4 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_5 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_rank_5")/"*"` | "pistol" lvl_6 ¦ "smg" lvl_5 ¦ "mgun" lvl_6 ¦ "mgun_light" lvl_6 ¦ "rifle" lvl_6 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_smg_rank_1")/"*"` | "pistol" lvl_2 ¦ "smg" lvl_2 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_smg_rank_2")/"*"` | "pistol" lvl_3 ¦ "smg" lvl_3 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_2 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_smg_rank_3")/"*"` | "pistol" lvl_4 ¦ "smg" lvl_4 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_3 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_smg_rank_4")/"*"` | "pistol" lvl_5 ¦ "smg" lvl_5 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_4 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "loader_skill_smg_rank_5")/"*"` | "pistol" lvl_6 ¦ "smg" lvl_6 ¦ "mgun" lvl_6 ¦ "mgun_light" lvl_6 ¦ "rifle" lvl_5 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "marksman")/"*"` | "rifle" lvl_8 | 미확인 |  | 1 | `set/breed/presets/perks/specialties.inc` |
| `/(define "medic")/"*"` | "medic" 10.0 ¦ "speed" 1.10 ¦ "stamina" "225%" | 미확인 |  | 1 | `set/breed/presets/perks/specialties.inc` |
| `/(define "medic")/barricade/"*"` | "fortifications" "auto_medic_place_flag" | 미확인 |  | 1 | `set/breed/presets/perks/specialties.inc` |
| `/(define "mg_skill_rank_1")/"*"` | "pistol" lvl_2 ¦ "smg" lvl_1 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "mg_skill_rank_2")/"*"` | "pistol" lvl_3 ¦ "smg" lvl_2 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_2 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "mg_skill_rank_3")/"*"` | "pistol" lvl_4 ¦ "smg" lvl_3 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_3 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "mg_skill_rank_4")/"*"` | "pistol" lvl_5 ¦ "smg" lvl_4 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_4 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "mg_skill_rank_5")/"*"` | "pistol" lvl_6 ¦ "smg" lvl_5 ¦ "mgun" lvl_6 ¦ "mgun_light" lvl_6 ¦ "rifle" lvl_5 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "recon_vision")/"*"` | "vision" human_recon | 미확인 |  | 1 | `set/breed/presets/perks/vision.inc` |
| `/(define "rifle_skill_rank_1")/"*"` | "pistol" lvl_2 ¦ "smg" lvl_1 ¦ "mgun" lvl_1 ¦ "mgun_light" lvl_1 ¦ "rifle" lvl_2 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "rifle_skill_rank_2")/"*"` | "pistol" lvl_3 ¦ "smg" lvl_2 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_3 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "rifle_skill_rank_3")/"*"` | "pistol" lvl_4 ¦ "smg" lvl_3 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_4 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "rifle_skill_rank_4")/"*"` | "pistol" lvl_5 ¦ "smg" lvl_4 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_5 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "rifle_skill_rank_5")/"*"` | "pistol" lvl_6 ¦ "smg" lvl_5 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_6 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "skill_rank_0")/"*"` | "pistol" lvl_1 ¦ "smg" lvl_1 ¦ "mgun" lvl_1 ¦ "mgun_light" lvl_1 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "skilled_gun")/"*"` | "tank_gun" lvl_3 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "skilled_repair")/mechanic` | factor 1.0 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "smg_skill_rank_1")/"*"` | "pistol" lvl_2 ¦ "smg" lvl_2 ¦ "mgun" lvl_1 ¦ "mgun_light" lvl_1 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "smg_skill_rank_2")/"*"` | "pistol" lvl_3 ¦ "smg" lvl_3 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_2 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "smg_skill_rank_3")/"*"` | "pistol" lvl_4 ¦ "smg" lvl_4 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_3 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "smg_skill_rank_4")/"*"` | "pistol" lvl_5 ¦ "smg" lvl_5 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_4 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "smg_skill_rank_5")/"*"` | "pistol" lvl_6 ¦ "smg" lvl_6 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_5 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인 |  | 1 | `set/breed/presets/perks/skills.inc` |
| `/(define "sniper_vision")/"*"` | "telescope" ¦ "aimedshot" ¦ "vision" human_recon | 미확인 |  | 1 | `set/breed/presets/perks/vision.inc` |
| `/(define "standard_vision")/"*"` | "vision" human | 미확인 |  | 1 | `set/breed/presets/perks/vision.inc` |
| `/(define "stealth")/"*"` | "stealth" ¦ "speed" 1.10 | 미확인 |  | 1 | `set/breed/presets/perks/specialties.inc` |
| `/(define "tank_commander")/"*"` | "health" "150%" | 미확인 |  | 1 | `set/breed/presets/perks/specialties.inc` |
| `/(define "tankman")/"*"` | "health" "150%" | 미확인 |  | 1 | `set/breed/presets/perks/specialties.inc` |
| `/(define "tier1_defense")/"*"` | "health" "150%" ¦ "health_regeneration" "300%" ¦ "stamina" "75%" ¦ "stamina_regeneration" "75%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier1_recon")/"*"` | "health" "100%" ¦ "health_regeneration" "10%" ¦ "stamina" "150%" ¦ "stamina_regeneration" "75%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier1_standard")/"*"` | "health" "150%" ¦ "health_regeneration" "10%" ¦ "stamina" "75%" ¦ "stamina_regeneration" "75%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier2_defense")/"*"` | "health" "200%" ¦ "health_regeneration" "250%" ¦ "stamina" "125%" ¦ "stamina_regeneration" "100%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier2_recon")/"*"` | "health" "150%" ¦ "health_regeneration" "10%" ¦ "stamina" "250%" ¦ "stamina_regeneration" "100%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier2_standard")/"*"` | "health" "200%" ¦ "health_regeneration" "10%" ¦ "stamina" "125%" ¦ "stamina_regeneration" "100%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier3_defense")/"*"` | "health" "300%" ¦ "health_regeneration" "200%" ¦ "stamina" "200%" ¦ "stamina_regeneration" "150%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier3_recon")/"*"` | "health" "250%" ¦ "health_regeneration" "10%" ¦ "stamina" "325%" ¦ "stamina_regeneration" "150%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier3_standard")/"*"` | "health" "300%" ¦ "health_regeneration" "10%" ¦ "stamina" "200%" ¦ "stamina_regeneration" "150%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier4_defense")/"*"` | "health" "375%" ¦ "health_regeneration" "150%" ¦ "stamina" "300%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier4_recon")/"*"` | "health" "325%" ¦ "health_regeneration" "10%" ¦ "stamina" "425%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier4_standard")/"*"` | "health" "375%" ¦ "health_regeneration" "10%" ¦ "stamina" "300%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "tier5_standard")/"*"` | "health" "500%" ¦ "health_regeneration" "20%" ¦ "stamina" "300%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인 |  | 1 | `set/breed/presets/perks/health.inc` |
| `/(define "vet_mp")/"*"` | "lvl_1" %0 ¦ "lvl_2" %1 ¦ "lvl_3" %2 ¦ "lvl_4" %3 ¦ "lvl_5" %4 ¦ "lvl_6" %5 ¦ "lvl_7" %6 ¦ "lvl_8" %7 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "vet")/"*"` | "lvl_1" %0 ¦ "lvl_2" %1 ¦ "lvl_3" %2 ¦ "lvl_4" %3 ¦ "lvl_5" %4 ¦ "lvl_6" %5 ¦ "lvl_7" %6 ¦ "lvl_8" %7 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_0")/("vet_mp")` | args 2 5 10 15 20 30 40 50 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_0")/("vet")` | args 4 10 20 30 40 60 80 100 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_1")/("vet_mp")` | args 0 3 8 13 18 28 38 48 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_1")/("vet")` | args 0 6 16 26 36 56 76 96 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_2")/("vet_mp")` | args -1 0 5 10 15 25 35 45 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_2")/("vet")` | args -1 0 10 20 30 50 70 90 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_3")/("vet_mp")` | args -1 -1 0 5 10 20 30 40 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_3")/("vet")` | args -1 -1 0 10 20 40 60 80 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_4")/("vet_mp")` | args -1 -1 -1 0 5 15 25 35 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_4")/("vet")` | args -1 -1 -1 0 10 30 50 70 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_5")/("vet_mp")` | args -1 -1 -1 -1 0 10 20 30 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_5")/("vet")` | args -1 -1 -1 -1 0 20 40 60 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_6")/("vet_mp")` | args -1 -1 -1 -1 -1 0 10 20 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_6")/("vet")` | args -1 -1 -1 -1 -1 0 20 40 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_7")/("vet_mp")` | args -1 -1 -1 -1 -1 -1 0 10 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_7")/("vet")` | args -1 -1 -1 -1 -1 -1 0 20 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_8")/("vet_mp")` | args -1 -1 -1 -1 -1 -1 -1 0 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |
| `/(define "veterancy_lvl_8")/("vet")` | args -1 -1 -1 -1 -1 -1 -1 0 | 미확인 |  | 1 | `set/breed/presets/perks/veterancy.inc` |

## 연관: 외형 엔티티

- 위치: `entity/humanskin/**/*.def`
- 연결: 본체의 `skin`이 이 파일의 이름을 가리킨다

### 뜻을 아는 속성 (8개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/game_entity/extension` | 예: "civ_01_var1.mdl" (13종 이상) | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) |  | 3666 | `entity/humanskin/[civilians]/civ_01/civ_01_var1.def` |
| `/game_entity/tag` | "rus soldier" | **추정** 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") |  | 1 | `entity/humanskin/human_for_cover/human_for_cover.def` |
| `/human/extension` | "human.mdl" | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) |  | 1 | `entity/humanskin/human/human.def` |
| `/human/obstacleid` | "human_impassable" "impassable" | **추정** 장애물 분류 이름 (추정: 값 "tank", "stuff") |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume` | "spacer" ¦ "Foot1R" ¦ "Foot2R" ¦ "Foot1L" ¦ "Foot2L" ¦ "ram" | **추정** 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/able/bullet` | 0 | **추정** 총탄에 맞는 볼륨임을 표시 (추정: able의 하위 플래그) |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/able/contact` | 0 | **추정** 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/able/contact_ground` | 0 | **추정** 지면 접촉 판정 대상 (추정: able의 하위 플래그) |  | 1 | `entity/humanskin/human/human.def` |

### 뜻을 모르는 속성 (10개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/human/nogroupmesh` | 1 | 미확인. 값은 1뿐. 용도 불명 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/obstacle` | ¦ "impassable" | 미확인 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/obstacle/circle2/radius` | 5 ~ 6 | 미확인 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/skeleton/viewvolume/center` | 0 0 18 | 미확인 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/skeleton/viewvolume/extent` | 30 30 18 | 미확인 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/able/obstacle` | 0 | 미확인 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/able/overlap` | 1 | 미확인 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/able/touch` | 0 | 미확인. able의 하위 플래그. 용도 불명 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/density` | 0.007 | 미확인. 값 0~2. 볼륨 밀도로 보이나 단위와 용도를 확인하지 못함 |  | 1 | `entity/humanskin/human/human.def` |
| `/human/volume/tags` | "no_target" | 미확인 |  | 1 | `entity/humanskin/human/human.def` |

