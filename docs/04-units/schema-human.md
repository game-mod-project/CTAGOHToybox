# 스키마: human

이 파일은 `tools/New-SchemaDoc.ps1`이 생성한다. 손으로 고치지 않는다. 의미와 등급은 `data/annotations/human.csv`에서 고친다.

## 원본 파일에 적힌 키

파일에 쓰인 그대로의 위치다. 고유 키 494개.

### 파일 종류: .def

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/game_entity` | block | []x3666 | 3666 | 3666 | 예: | 미확인(자동) |  |  |
| `/game_entity/extension` | block | [str]x3666 | 3666 | 3666 | 예: "civ_01_var1.mdl" | 미확인(자동) |  |  |
| `/game_entity/nocastshadows` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/nogetshadows` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/rimlight` | block | []x3664 | 3664 | 3664 | 예: | 미확인(자동) |  |  |
| `/game_entity/tag` | block | [str]x1 | 1 | 1 | "rus soldier" | 미확인(자동) |  |  |
| `/human` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/(include)` | include | [str]x2 | 1 | 2 | "/properties/human.ext" ¦ "/properties/selection/human.inc" | 미확인(자동) |  |  |
| `/human/extension` | block | [str]x1 | 1 | 1 | "human.mdl" | 미확인(자동) |  |  |
| `/human/nogroupmesh` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/human/novisibilitypatch` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/obstacle` | block | []x1 [str]x1 | 1 | 2 | ¦ "impassable" | 미확인(자동) |  |  |
| `/human/obstacle/circle2` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/human/obstacle/circle2/radius` | block | [num]x2 | 1 | 2 | 5 ~ 6 | 미확인(자동) |  |  |
| `/human/obstacleid` | block | [str str]x1 | 1 | 1 | "human_impassable" "impassable" | 미확인(자동) |  |  |
| `/human/rimlight` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/skeleton` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/skeleton/viewvolume` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/skeleton/viewvolume/center` | block | [num num num]x1 | 1 | 1 | 0 0 18 | 미확인(자동) |  |  |
| `/human/skeleton/viewvolume/extent` | block | [num num num]x1 | 1 | 1 | 30 30 18 | 미확인(자동) |  |  |
| `/human/volume` | block | [str]x6 | 1 | 6 | "spacer" ¦ "Foot1R" ¦ "Foot2R" ¦ "Foot1L" ¦ "Foot2L" ¦ "ram" | 미확인(자동) |  |  |
| `/human/volume/able` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/volume/able/bullet` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/able/contact` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/able/contact_ground` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/able/obstacle` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/able/overlap` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/human/volume/able/touch` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/density` | block | [num]x1 | 1 | 1 | 0.007 | 미확인(자동) |  |  |
| `/human/volume/tags` | block | [str]x5 | 1 | 5 | "no_target" | 미확인(자동) |  |  |

### 파일 종류: .inc

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/(define "at_rank_1")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "at_rank_1")/"*"` | block | [str atom]x2 | 1 | 2 | "rocketlauncher" lvl_1 ¦ "rifle" lvl_1 | 미확인(자동) |  |  |
| `/(define "at_rank_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "at_rank_2")/"*"` | block | [str atom]x2 | 1 | 2 | "rocketlauncher" lvl_2 ¦ "rifle" lvl_2 | 미확인(자동) |  |  |
| `/(define "at_rank_3")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "at_rank_3")/"*"` | block | [str atom]x2 | 1 | 2 | "rocketlauncher" lvl_3 ¦ "rifle" lvl_3 | 미확인(자동) |  |  |
| `/(define "at_rank_4")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "at_rank_4")/"*"` | block | [str atom]x2 | 1 | 2 | "rocketlauncher" lvl_4 ¦ "rifle" lvl_4 | 미확인(자동) |  |  |
| `/(define "at_rank_5")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "at_rank_5")/"*"` | block | [str atom]x2 | 1 | 2 | "rocketlauncher" lvl_5 ¦ "rifle" lvl_5 | 미확인(자동) |  |  |
| `/(define "at_rank_6")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "at_rank_6")/"*"` | block | [str atom]x2 | 1 | 2 | "rocketlauncher" lvl_6 ¦ "rifle" lvl_6 | 미확인(자동) |  |  |
| `/(define "basic_gun")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "basic_gun")/"*"` | block | [str atom]x1 | 1 | 1 | "tank_gun" lvl_1 | 미확인(자동) |  |  |
| `/(define "basic_repair")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "basic_repair")/mechanic` | block | [atom num]x1 | 1 | 1 | factor 0.5 | 미확인(자동) |  |  |
| `/(define "binocular_vision")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "binocular_vision")/"*"` | block | [str]x1 [str atom]x1 | 1 | 2 | "field_glass" ¦ "vision" human_elite | 미확인(자동) |  |  |
| `/(define "civilian")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "civilian")/"*"` | block | [str str]x4 [str num]x1 | 1 | 5 | "health" "100%" ¦ "health_regeneration" "10%" ¦ "stamina" "75%" ¦ "stamina_regeneration" "75%" ¦ "speed" 0.90 | 미확인(자동) |  |  |
| `/(define "civilian")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "elite_gun")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "elite_gun")/"*"` | block | [str atom]x1 | 1 | 1 | "tank_gun" lvl_6 | 미확인(자동) |  |  |
| `/(define "elite_vision")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "elite_vision")/"*"` | block | [str atom]x1 | 1 | 1 | "vision" human_elite | 미확인(자동) |  |  |
| `/(define "engineer_heavy_fin")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "engineer_heavy_fin")/"*"` | block | [str str]x2 | 1 | 2 | "resources" "1000%" ¦ "resources_regeneration" "0%" | 미확인(자동) |  |  |
| `/(define "engineer_heavy_fin")/barricade` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "engineer_heavy_fin")/barricade/"*"` | block | [str str]x15 | 1 | 15 | 예: "fortifications" "barricade_sq" | 미확인(자동) |  |  |
| `/(define "engineer_heavy_fin")/fortification` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "engineer_heavy_fin")/mechanic` | block | [atom num]x1 | 1 | 1 | factor 1.0 | 미확인(자동) |  |  |
| `/(define "engineer")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "engineer")/"*"` | block | [str str]x2 | 1 | 2 | "resources" "1000%" ¦ "resources_regeneration" "0%" | 미확인(자동) |  |  |
| `/(define "engineer")/barricade` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "engineer")/barricade/"*"` | block | [str str]x12 | 1 | 12 | "fortifications" "barricade_sq" ¦ "fortifications" "barricade" ¦ "fortifications" "barbwire" ¦ "fortifications" "hedgehog" ¦ "fortifications" "foxhole" ¦ "fortifications" "foxhole_big" ¦ "fortifications" "mortar_pit" ¦ "fortifications" "tank_cover" ¦ "fortifications" "heavy_mg_mg34" ¦ "fortifications" "heavy_mg_maxim" ¦ "fortifications" "heavy_mg_maxim_m1910_30" ¦ "fortifications" "heavy_mg_m1917_30cal" | 미확인(자동) |  |  |
| `/(define "engineer")/fortification` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "engineer")/mechanic` | block | [atom num]x1 | 1 | 1 | factor 1.0 | 미확인(자동) |  |  |
| `/(define "expert_gun")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "expert_gun")/"*"` | block | [str atom]x1 | 1 | 1 | "tank_gun" lvl_4 | 미확인(자동) |  |  |
| `/(define "fortification")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "fortification")/"*"` | block | [str str]x1 | 1 | 1 | "resources" "10%" | 미확인(자동) |  |  |
| `/(define "fortification")/barricade` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "fortification")/barricade/"*"` | block | [str str]x2 | 1 | 2 | "fortifications" "foxhole" ¦ "fortifications" "medic_rally" | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_1")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_1")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_2 ¦ "smg" lvl_1 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_2 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_1")/("at_rank_1")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_1")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_1")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_1")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_2")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_3 ¦ "smg" lvl_2 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_3 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_2")/("at_rank_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_2")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_2")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_2")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_3")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_3")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_4 ¦ "smg" lvl_3 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_4 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_3")/("at_rank_3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_3")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_3")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_3")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_4")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_4")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_5 ¦ "smg" lvl_4 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_5 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_4")/("at_rank_4")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_4")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_4")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_4")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_5")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_5")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_6 ¦ "smg" lvl_5 ¦ "mgun" lvl_6 ¦ "mgun_light" lvl_6 ¦ "rifle" lvl_6 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_5")/("at_rank_5")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_5")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_5")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_rank_5")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_1")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_1")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_2 ¦ "smg" lvl_2 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_1")/("at_rank_1")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_1")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_1")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_1")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_2")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_3 ¦ "smg" lvl_3 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_2 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_2")/("at_rank_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_2")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_2")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_2")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_3")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_3")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_4 ¦ "smg" lvl_4 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_3 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_3")/("at_rank_3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_3")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_3")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_3")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_4")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_4")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_5 ¦ "smg" lvl_5 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_4 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_4")/("at_rank_4")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_4")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_4")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_4")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_5")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_5")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_6 ¦ "smg" lvl_6 ¦ "mgun" lvl_6 ¦ "mgun_light" lvl_6 ¦ "rifle" lvl_5 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_5")/("at_rank_5")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_5")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_5")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "loader_skill_smg_rank_5")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "marksman")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "marksman")/"*"` | block | [str atom]x1 | 1 | 1 | "rifle" lvl_8 | 미확인(자동) |  |  |
| `/(define "medic")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "medic")/"*"` | block | [str num]x2 [str str]x1 | 1 | 3 | "medic" 10.0 ¦ "speed" 1.10 ¦ "stamina" "225%" | 미확인(자동) |  |  |
| `/(define "medic")/barricade` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "medic")/barricade/"*"` | block | [str str]x1 | 1 | 1 | "fortifications" "auto_medic_place_flag" | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_1")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_1")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_2 ¦ "smg" lvl_1 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_1")/("at_rank_1")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_1")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_1")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_1")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_2")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_3 ¦ "smg" lvl_2 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_2 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_2")/("at_rank_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_2")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_2")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_2")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_3")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_3")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_4 ¦ "smg" lvl_3 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_3 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_3")/("at_rank_3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_3")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_3")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_3")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_4")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_4")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_5 ¦ "smg" lvl_4 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_4 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_4")/("at_rank_4")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_4")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_4")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_4")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_5")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_5")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_6 ¦ "smg" lvl_5 ¦ "mgun" lvl_6 ¦ "mgun_light" lvl_6 ¦ "rifle" lvl_5 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_5")/("at_rank_5")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_5")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_5")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "mg_skill_rank_5")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "recon_vision")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "recon_vision")/"*"` | block | [str atom]x1 | 1 | 1 | "vision" human_recon | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_1")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_1")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_2 ¦ "smg" lvl_1 ¦ "mgun" lvl_1 ¦ "mgun_light" lvl_1 ¦ "rifle" lvl_2 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_1")/("at_rank_1")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_1")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_1")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_1")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_2")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_3 ¦ "smg" lvl_2 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_3 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_2")/("at_rank_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_2")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_2")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_2")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_3")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_3")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_4 ¦ "smg" lvl_3 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_4 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_3")/("at_rank_3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_3")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_3")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_3")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_4")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_4")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_5 ¦ "smg" lvl_4 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_5 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_4")/("at_rank_4")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_4")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_4")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_4")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_5")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_5")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_6 ¦ "smg" lvl_5 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_6 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_5")/("at_rank_5")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_5")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_5")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "rifle_skill_rank_5")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skill_rank_0")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skill_rank_0")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_1 ¦ "smg" lvl_1 ¦ "mgun" lvl_1 ¦ "mgun_light" lvl_1 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인(자동) |  |  |
| `/(define "skill_rank_0")/("at_rank_1")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skill_rank_0")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skill_rank_0")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skill_rank_0")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skilled_gun")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skilled_gun")/"*"` | block | [str atom]x1 | 1 | 1 | "tank_gun" lvl_3 | 미확인(자동) |  |  |
| `/(define "skilled_repair")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "skilled_repair")/mechanic` | block | [atom num]x1 | 1 | 1 | factor 1.0 | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_1")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_1")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_2 ¦ "smg" lvl_2 ¦ "mgun" lvl_1 ¦ "mgun_light" lvl_1 ¦ "rifle" lvl_1 ¦ "melee" lvl_1 ¦ "avoidance" lvl_1 | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_1")/("at_rank_1")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_1")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_1")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_1")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_2")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_3 ¦ "smg" lvl_3 ¦ "mgun" lvl_2 ¦ "mgun_light" lvl_2 ¦ "rifle" lvl_2 ¦ "melee" lvl_2 ¦ "avoidance" lvl_2 | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_2")/("at_rank_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_2")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_2")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_2")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_3")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_3")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_4 ¦ "smg" lvl_4 ¦ "mgun" lvl_3 ¦ "mgun_light" lvl_3 ¦ "rifle" lvl_3 ¦ "melee" lvl_3 ¦ "avoidance" lvl_3 | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_3")/("at_rank_3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_3")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_3")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_3")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_4")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_4")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_5 ¦ "smg" lvl_5 ¦ "mgun" lvl_4 ¦ "mgun_light" lvl_4 ¦ "rifle" lvl_4 ¦ "melee" lvl_4 ¦ "avoidance" lvl_4 | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_4")/("at_rank_4")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_4")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_4")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_4")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_5")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_5")/"*"` | block | [str atom]x7 | 1 | 7 | "pistol" lvl_6 ¦ "smg" lvl_6 ¦ "mgun" lvl_5 ¦ "mgun_light" lvl_5 ¦ "rifle" lvl_5 ¦ "melee" lvl_5 ¦ "avoidance" lvl_5 | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_5")/("at_rank_5")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_5")/("basic_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_5")/("basic_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "smg_skill_rank_5")/("fortification")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "sniper_vision")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "sniper_vision")/"*"` | block | [str]x2 [str atom]x1 | 1 | 3 | "telescope" ¦ "aimedshot" ¦ "vision" human_recon | 미확인(자동) |  |  |
| `/(define "standard_vision")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "standard_vision")/"*"` | block | [str atom]x1 | 1 | 1 | "vision" human | 미확인(자동) |  |  |
| `/(define "stealth")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "stealth")/"*"` | block | [str]x1 [str num]x1 | 1 | 2 | "stealth" ¦ "speed" 1.10 | 미확인(자동) |  |  |
| `/(define "stealth")/("recon_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tank_commander")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tank_commander")/"*"` | block | [str str]x1 | 1 | 1 | "health" "150%" | 미확인(자동) |  |  |
| `/(define "tank_commander")/("expert_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tank_commander")/("loader_skill_smg_rank_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tank_commander")/("skilled_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tank_commander")/("tier2_standard")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tankman")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tankman")/"*"` | block | [str str]x1 | 1 | 1 | "health" "150%" | 미확인(자동) |  |  |
| `/(define "tankman")/("loader_skill_smg_rank_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tankman")/("skilled_gun")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tankman")/("skilled_repair")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tankman")/("tier2_standard")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier1_defense")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier1_defense")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "150%" ¦ "health_regeneration" "300%" ¦ "stamina" "75%" ¦ "stamina_regeneration" "75%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인(자동) |  |  |
| `/(define "tier1_defense")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier1_recon")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier1_recon")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "100%" ¦ "health_regeneration" "10%" ¦ "stamina" "150%" ¦ "stamina_regeneration" "75%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인(자동) |  |  |
| `/(define "tier1_recon")/("recon_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier1_standard")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier1_standard")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "150%" ¦ "health_regeneration" "10%" ¦ "stamina" "75%" ¦ "stamina_regeneration" "75%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인(자동) |  |  |
| `/(define "tier1_standard")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier2_defense")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier2_defense")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "200%" ¦ "health_regeneration" "250%" ¦ "stamina" "125%" ¦ "stamina_regeneration" "100%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인(자동) |  |  |
| `/(define "tier2_defense")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier2_recon")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier2_recon")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "150%" ¦ "health_regeneration" "10%" ¦ "stamina" "250%" ¦ "stamina_regeneration" "100%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인(자동) |  |  |
| `/(define "tier2_recon")/("recon_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier2_standard")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier2_standard")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "200%" ¦ "health_regeneration" "10%" ¦ "stamina" "125%" ¦ "stamina_regeneration" "100%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인(자동) |  |  |
| `/(define "tier2_standard")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier3_defense")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier3_defense")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "300%" ¦ "health_regeneration" "200%" ¦ "stamina" "200%" ¦ "stamina_regeneration" "150%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인(자동) |  |  |
| `/(define "tier3_defense")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier3_recon")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier3_recon")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "250%" ¦ "health_regeneration" "10%" ¦ "stamina" "325%" ¦ "stamina_regeneration" "150%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인(자동) |  |  |
| `/(define "tier3_recon")/("recon_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier3_standard")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier3_standard")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "300%" ¦ "health_regeneration" "10%" ¦ "stamina" "200%" ¦ "stamina_regeneration" "150%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인(자동) |  |  |
| `/(define "tier3_standard")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier4_defense")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier4_defense")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "375%" ¦ "health_regeneration" "150%" ¦ "stamina" "300%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 0.90 | 미확인(자동) |  |  |
| `/(define "tier4_defense")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier4_recon")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier4_recon")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "325%" ¦ "health_regeneration" "10%" ¦ "stamina" "425%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 1.10 | 미확인(자동) |  |  |
| `/(define "tier4_recon")/("recon_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier4_standard")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier4_standard")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "375%" ¦ "health_regeneration" "10%" ¦ "stamina" "300%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인(자동) |  |  |
| `/(define "tier4_standard")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier5_standard")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "tier5_standard")/"*"` | block | [str str]x4 [str num]x2 | 1 | 6 | "health" "500%" ¦ "health_regeneration" "20%" ¦ "stamina" "300%" ¦ "stamina_regeneration" "250%" ¦ "personage" 0 ¦ "speed" 1.00 | 미확인(자동) |  |  |
| `/(define "tier5_standard")/("standard_vision")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "vet_mp")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "vet_mp")/"*"` | block | [str param]x8 | 1 | 8 | "lvl_1" %0 ¦ "lvl_2" %1 ¦ "lvl_3" %2 ¦ "lvl_4" %3 ¦ "lvl_5" %4 ¦ "lvl_6" %5 ¦ "lvl_7" %6 ¦ "lvl_8" %7 | 미확인(자동) |  |  |
| `/(define "vet_mp")/(mod)` | mod | [str]x1 | 1 | 1 | "mp" | 미확인(자동) |  |  |
| `/(define "vet")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "vet")/"*"` | block | [str param]x8 | 1 | 8 | "lvl_1" %0 ¦ "lvl_2" %1 ¦ "lvl_3" %2 ¦ "lvl_4" %3 ¦ "lvl_5" %4 ¦ "lvl_6" %5 ¦ "lvl_7" %6 ¦ "lvl_8" %7 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_0")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_0")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args 2 5 10 15 20 30 40 50 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_0")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args 4 10 20 30 40 60 80 100 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_1")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_1")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args 0 3 8 13 18 28 38 48 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_1")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args 0 6 16 26 36 56 76 96 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_2")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 0 5 10 15 25 35 45 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_2")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 0 10 20 30 50 70 90 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_3")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_3")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 0 5 10 20 30 40 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_3")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 0 10 20 40 60 80 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_4")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_4")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 0 5 15 25 35 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_4")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 0 10 30 50 70 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_5")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_5")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 0 10 20 30 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_5")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 0 20 40 60 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_6")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_6")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 -1 0 10 20 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_6")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 -1 0 20 40 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_7")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_7")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 -1 -1 0 10 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_7")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 -1 -1 0 20 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_8")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_8")/("vet_mp")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 -1 -1 -1 0 | 미확인(자동) |  |  |
| `/(define "veterancy_lvl_8")/("vet")` | macro | [atom num num num num num num num num]x1 | 1 | 1 | args -1 -1 -1 -1 -1 -1 -1 0 | 미확인(자동) |  |  |
| `/(include)` | include | [str]x79 | 41 | 79 | "/set/breed/presets/perks/specialties.inc" ¦ "/set/breed/presets/perks/veterancy.inc" ¦ "/set/breed/presets/perks/vision.inc" ¦ "/set/breed/presets/perks/health.inc" ¦ "/set/breed/presets/perks/skills.inc" | 미확인(자동) |  |  |

### 파일 종류: .mdl

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/skeleton` | block | []x3672 | 3672 | 3672 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone` | block | [atom str]x3672 | 3672 | 3672 | revolute "basis" | 미확인(자동) |  |  |
| `/skeleton/bone/bone` | block | [str]x36703 [atom str]x3672 | 3672 | 40375 | 예: "body" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone` | block | [str]x14682 [atom str]x7346 | 3672 | 22028 | "foot1l" ¦ "foot1r" ¦ revolute "ik_leftright" ¦ revolute "placement" ¦ "palm_ik_holder_right01" ¦ "palm_ik_holder_left01" ¦ revolute "aim_ref_h" ¦ revolute "foresight2" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone` | block | [str]x14683 [atom str]x3672 | 3672 | 18355 | "foot2l" ¦ "foot2r" ¦ revolute "ik_updown" ¦ "palm_ik_holder_right" ¦ "palm_ik_holder_left" ¦ "ik_socket" ¦ revolute "aim_ref_v" ¦ planarsocket "ik_socket" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone` | block | [str]x36707 | 3672 | 36707 | "foot3l" ¦ "foot3r" ¦ "bone06" ¦ "bone03" ¦ "head" ¦ "gun_back" ¦ "clavicle_right" ¦ "clavicle_left" ¦ "ik_chain07" ¦ "ik_chain08" ¦ "compensate" ¦ "aim_ref" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone` | block | [str]x18352 [atom str]x2 | 3672 | 18354 | "bone07" ¦ "bone05" ¦ "visor" ¦ "hand1r" ¦ "hand1l" ¦ revolute "ik_updown" ¦ revolute "ik_lat" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone` | block | [str]x7344 [atom str]x1 | 3672 | 7345 | "hand2r" ¦ "hand2l" ¦ "head" ¦ "gun_back" ¦ "clavicle_right" ¦ "clavicle_left" ¦ revolute "ik_lng" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x22025 | 3672 | 22025 | "hand3r" ¦ "right_hand" ¦ "hand_rot1r" ¦ "hand3l" ¦ "left_hand" ¦ "hand_rot1l" ¦ "visor" ¦ "helmet_link" ¦ "hand1r" ¦ "hand1l" ¦ "compensate2" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x7344 | 3672 | 7344 | "palm1r" ¦ "palm1l" ¦ "hand2r" ¦ "hand2l" ¦ "ik_updown" ¦ "em3" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x14689 | 3672 | 14689 | 예: "palm2r_hide" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x14683 | 3672 | 14683 | "palm3r" ¦ "palm3l" ¦ "palm3r_hide" ¦ "palm3l_hide" ¦ "palm1r" ¦ "palm1l" ¦ "visor" ¦ "hand1r" ¦ "hand1l" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x7352 | 3671 | 7352 | 예: "palm4r_hide" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x16 | 2 | 16 | 예: "palm3r" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x12 | 2 | 12 | "righthandindex3" ¦ "righthandmiddle3" ¦ "righthandring3" ¦ "righthandpinky3" ¦ "righthandthumb3" ¦ "lefthandindex3" ¦ "lefthandmiddle3" ¦ "lefthandring3" ¦ "lefthandpinky3" ¦ "lefthandthumb3" ¦ "palm1r" ¦ "palm1l" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x2 | 1 | 2 | "palm2r" ¦ "palm2l" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x2 | 1 | 2 | "palm3r" ¦ "palm3l" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x2 | 1 | 2 | 0.576833 0.816863 0 -0.816862 0.576832 0 0 0 1 0.58188 -0.000115261 0 ¦ 0.578449 -0.815719 0 0.815719 0.578449 0 0 0 1 0.578554 0.00016544 0 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x2 | 1 | 2 | -0.00756741 0.50705 -0.861883 0.00245775 0.861916 0.507046 0.999968 0.00171632 -0.00776881 0.146144 0.242285 -1.25751 ¦ -0.00583927 0.491345 -0.870945 -0.00450489 -0.870965 -0.491325 -0.999973 0.0010554 0.00729822 -0.0738367 0.242492 -1.26342 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num atom num atom num num num num]x4 [num num num num num num num num num num num num]x3 [num num atom num num num atom num num num num num]x3 [num atom num num num num num num num num num num]x1 [num atom atom atom num num atom num num num num num]x1 | 2 | 12 | 1 0 0 0 1 0 0 0 1 0.64282 0 0 ¦ 1 0 -1.90111e-05 0 1 0 2.02004e-05 0 1 0.741715 0 0 ¦ 1 0 0 0 1 -6.9052e-05 0 7.10785e-05 1 0.642819 0 0 ¦ 1 0 0 0 1 -4.83394e-05 0 4.98891e-05 1 0.535685 0 0 ¦ 1 0 -1.32173e-05 0 1 0 1.38469e-05 0 1 0.73094 0 0 ¦ 1 -1.0252e-05 0 0 1 0 0 0 1 0.642821 0 0 ¦ 1 0 -1.96639e-05 0 1 0 2.05347e-05 0 1 0.741717 0 0 ¦ 1 0 0 0 1 7.06613e-05 0 -7.13766e-05 1 0.642824 0 0 ¦ 1 0 0 0 1 4.93824e-05 0 -4.98295e-05 1 0.535683 0 0 ¦ 1 -1.08331e-05 -1.44243e-05 1.017e-05 1 0 1.43796e-05 0 1 0.730939 0 0 ¦ -0.987462 -0.121604 -0.100652 -0.0703691 -0.231655 0.97025 -0.141302 0.965167 0.220194 0.12139 -0.163001 0.149547 ¦ 0.965404 0.249251 -0.0766089 -0.0261254 -0.199861 -0.979476 -0.259446 0.947591 -0.186436 0.0886898 -0.337659 -0.0916094 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x11 [num num atom num num num atom num num num num num]x2 [num atom atom atom num num atom num num num num num]x2 [num num num atom num num num num num num num num]x1 | 2 | 16 | 예: 0.304929 0.952376 0 -0.952376 0.304927 0 0 0 1 0.581881 -0.000116348 0 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x1 | 1 | 1 | "LastPose = undefined;" | 추정(파일 주석) | "} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x7352 | 3671 | 7352 | 예: 1 0 0 0 1 0 0 0 1 0.5345 0 0 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x4 | 1 | 4 | "LastPose = undefined;" | 추정(파일 주석) | "} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x14683 | 3672 | 14683 | 0.5768 0.8169 0 -0.8169 0.5768 0 0 0 1 0.5819 -0.0001 0 ¦ 0.5784 -0.8157 0 0.8157 0.5784 0 0 0 1 0.5786 0.0002 0 ¦ 0.5784 -0.8157 0 0.8157 0.5784 0 0 0 1 0.5785 0.0002 0 ¦ -0.963565 -0.249481 -0.0964401 -0.0459582 -0.20077 0.97856 -0.263496 0.947338 0.181991 0.0853548 -0.397722 0.0915813 ¦ 0.965404 0.24925 -0.0766089 -0.0261243 -0.199865 -0.979475 -0.259447 0.947591 -0.186439 0.0886898 -0.337658 -0.09161 ¦ 1 0 0 0 1 0 0 0 1 2.31916 0 2.33291 ¦ -0.139908 -0.964538 0.223815 0.8542 -0.23189 -0.465371 0.500768 0.126073 0.856351 0.0503405 -0.225974 2.95337 ¦ -0.17574 -0.948524 -0.263473 0.850453 -0.281083 0.444659 -0.495827 -0.145928 0.856073 0.0503408 -0.225974 -2.95273 ¦ 0.576833 0.816863 0 -0.816862 0.576832 0 0 0 1 0.58188 -0.000115261 0 ¦ 0.578449 -0.815719 0 0.815719 0.578449 0 0 0 1 0.578554 0.00016544 0 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x14687 | 3672 | 14687 | -0.0078 0.5069 -0.862 0.0025 0.862 0.5069 1 0.0018 -0.008 0.1461 0.2423 -1.2575 ¦ -0.0049 0.4905 -0.8714 -0.0048 -0.8714 -0.4905 -1 0.0018 0.0066 -0.0738 0.2425 -1.2634 ¦ 0.982611 -0.185677 0 0.185676 0.982611 0 0 0 1 5.19218 0 0 ¦ 0.00647827 0.996053 0.0885295 -0.106389 0.0887156 -0.990359 -0.994303 -0.00300278 0.106544 -4.16774 -4.2536 2.42593 ¦ 0.968615 -0.225094 0.105442 0.191903 0.946804 0.258332 -0.157981 -0.22999 0.960285 6.33401 0.913939 -0.654926 ¦ 0.00333649 -0.999958 0.00855827 -0.999992 -0.00331752 0.00210692 -0.00207908 -0.00856459 -0.999961 5.19224 0 0 ¦ 0.275672 0.823504 -0.495828 0.00418913 -0.516839 -0.856072 -0.961242 0.23392 -0.145927 5.99778 -0.667306 0.409924 ¦ 0.0013026 -0.999932 0.0116372 -0.999999 -0.00131376 -0.00103799 0.00105401 -0.0116362 -0.999932 5.25743 -0.000658989 -0.00162315 ¦ 1 0 0 0 0 -1 0 1 0 0 7.21458 0 ¦ -0.00756741 0.50705 -0.861883 0.00245775 0.861916 0.507046 0.999968 0.00171632 -0.00776881 0.146144 0.242285 -1.25751 ¦ -0.00583927 0.491345 -0.870945 -0.00450489 -0.870965 -0.491325 -0.999973 0.0010554 0.00729822 -0.0738367 0.242492 -1.26342 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x2 | 1 | 2 | -0.13918 6.12487 1.17253 ¦ -0.13918 6.12487 -1.18112 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x7343 | 3672 | 7343 | -0.9959 -0.0821 0.0371 0.0595 -0.2902 0.9551 -0.0677 0.9534 0.2939 0.1214 -0.163 0.1495 ¦ 0.9363 0.1938 0.2929 0.3211 -0.1348 -0.9374 -0.1422 0.9717 -0.1885 0.0887 -0.3377 -0.0916 ¦ 0.460802 0.887503 0 -0.887503 0.460802 0 0 0 1 6.26813 0 0 ¦ 0.458669 0.888607 0 -0.888607 0.458669 0 0 0 1 6.26813 0 0 ¦ 1 0 0 0 0 1 0 -1 0 0 0 1.66801 ¦ -0.987462 -0.121604 -0.100652 -0.0703691 -0.231655 0.97025 -0.141302 0.965167 0.220194 0.12139 -0.163001 0.149547 ¦ 0.965404 0.249251 -0.0766089 -0.0261254 -0.199861 -0.979476 -0.259446 0.947591 -0.186436 0.0886898 -0.337659 -0.0916094 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x1 | 1 | 1 | 0 0 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x1 | 1 | 1 | "em3.ply" | 추정(파일 주석) | (Retained for LOD1 for fidelity.) | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x22025 | 3672 | 22025 | 예: 0.9826 -0.1857 0 0.1857 0.9826 0 0 0 1 5.1922 0 0 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/limits` | block | [num num]x1 | 1 | 1 | -40 40 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x7343 | 3672 | 7343 | 0.9663 0.2573 0 -0.2573 0.9663 0 0 0 1 6.2681 0 0 ¦ 0.9367 0.3501 0 -0.3501 0.9367 0 0 0 1 6.2681 0 0 ¦ 0.999952 0.00413703 -0.00886165 -0.00885884 -0.000694277 -0.99996 -0.00414242 0.999991 -0.000657678 0 7.21457 0 ¦ 0.00647848 0.996053 0.0885296 -0.106389 0.0887155 -0.990359 -0.994303 -0.00300312 0.106544 -4.16774 -4.2536 2.42593 ¦ 1 0 0 0 0 1 0 -1 0 0 0 0 ¦ 0.941579 0.336792 0 -0.336792 0.941579 0 0 0 1 6.26813 0 0 ¦ 0.864419 0.502773 0 -0.502773 0.864419 0 0 0 1 6.26813 0 0 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x2 | 1 | 2 | -0.139178 6.12487 1.17253 ¦ -0.139178 6.12487 -1.18112 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/speed2` | block | [num]x1 | 1 | 1 | 720 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/limits` | block | [num num]x2 | 2 | 2 | -40 20 ¦ -40 40 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x14681 | 3672 | 14681 | 1 0 0 0 1 0 0 0 1 2.6001 0 0 ¦ 1 0 0 0 1 0 0 0 1 2.8146 0 2.3329 ¦ 0.4351 -0.7063 0.5584 0.8932 0.2604 -0.3665 0.1135 0.6583 0.7442 0.0503 -0.226 2.9534 ¦ 0.2385 -0.8203 -0.5198 0.9296 0.038 0.3665 -0.281 -0.5706 0.7717 0.0503 -0.226 -2.9527 ¦ 1 0 0 0 0 1 0 -1 0 0 0 1.66801 ¦ 0 0 1 0 1 0 -1 0 0 0 0 0 ¦ 1 0 0 0 1 0 0 0 1 2.31916 0 2.33291 ¦ -0.139908 -0.964538 0.223815 0.8542 -0.23189 -0.465371 0.500768 0.126073 0.856351 0.0503405 -0.225974 2.95337 ¦ -0.17574 -0.948524 -0.263473 0.850453 -0.281083 0.444659 -0.495827 -0.145928 0.856073 0.0503408 -0.225974 -2.95273 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x3673 | 3671 | 3673 | 2.6001 0 0 ¦ 2.60015 0 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/speed` | block | [num]x1 | 1 | 1 | 0.1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/speed2` | block | [num]x1 | 1 | 1 | 720 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x36705 | 3672 | 36705 | 예: 0.2214 0.974 0.0488 -0.9723 0.2166 0.0881 0.0752 -0.067 0.9949 8.7402 0.1133 0.0057 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/position` | block | [num num num]x2 | 1 | 2 | -0.13918 6.12487 1.17253 ¦ -0.13918 6.12487 -1.18112 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/limits` | block | [num num]x3671 [num]x1 | 3671 | 3672 | 65 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x18349 [num num num num num atom num num num num num num]x6 | 3672 | 18355 | 0.9974 -0.0721 -0.0005 0.0721 0.9974 -0.0002 0.0006 0.0002 1 9.3784 -0.0472 0 ¦ 0.9982 -0.0601 -0.0006 0.0601 0.9982 -0.0002 0.0006 0.0002 1 9.3783 -0.0472 0 ¦ 1 0 0 0 0 1 0 -1 0 0 0 1.668 ¦ -0.9959 -0.0821 0.0371 0.0595 -0.2902 0.9551 -0.0677 0.9534 0.2939 0.1199 -0.1649 0.1487 ¦ 0.9363 0.1938 0.2929 0.3211 -0.1348 -0.9374 -0.1422 0.9717 -0.1885 0.0856 -0.3388 -0.0895 ¦ 0 1 0 1 0 0 0 0 1 0 0 0 ¦ 0.952385 -0.304899 -0.000108002 0.304899 0.952385 -4.08264e-005 0.000115287 0 1 9.37838 -0.0471724 0 ¦ 0.951792 -0.306743 -0.000104386 0.306743 0.951792 -3.94955e-005 0.000111691 0 1 9.37831 -0.0472316 0 ¦ 0.952382 -0.304908 -0.000107976 0.304908 0.952382 -4.07626e-05 0.000115262 0 1 9.37838 -0.0471723 0 ¦ 0.951795 -0.306736 -0.000104569 0.306736 0.951795 -3.94722e-05 0.000111624 0 1 9.37831 -0.0472317 0 ¦ 1 0 0 0 0 1 0 -1 0 0 0 0 ¦ 1 0 0 0 0 1 0 -1 0 0 0 1.66801 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/speed` | block | [num]x3670 | 3670 | 3670 | 0.1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/speed2` | block | [num]x2 | 1 | 2 | 720 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/limits` | block | [num num]x3674 | 3672 | 3674 | -20 20 ¦ -50 40 ¦ -45 45 ¦ -10 80 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x22028 | 3672 | 22028 | 예: -0.9996 0.017 -0.0247 -0.0247 -0.0005 0.9997 -0.017 -0.9999 -0.0009 -0.1435 -2.131 -0.489 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/speed` | block | [num]x3671 | 3671 | 3671 | 0.1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/speed2` | block | [num]x3 | 1 | 3 | 300 ~ 720 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/limits` | block | [num num]x3672 | 3672 | 3672 | -20 20 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/lodview` | block | []x32698 | 3669 | 32698 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/lodview/volumeview` | block | [str]x124512 | 3669 | 124512 | 예: "heads/civ_head_1.ply" | 추정(파일 주석) | (Retained for LOD1 for fidelity.) | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x18353 | 3672 | 18353 | 0 0 1 0 -1 0 1 0 0 0.6118 0.0345 19.9368 ¦ 1 0 0 0 -1 0 0 0 1 -0.8883 1.8586 1.7109 ¦ 1 0 0 0 -1 0 0 0 1 3.0299 2.478 0.0515 ¦ 0.3452 -0.5526 -0.7586 0.9324 0.1096 0.3444 -0.1072 -0.8262 0.5531 0.1477 -7.3837 23.9651 ¦ 0.3909 0.513 -0.7642 0.9202 -0.2361 0.3122 -0.0203 -0.8253 -0.5644 0.0473 7.5158 24.2278 ¦ 0 0 1 0 -1 0 1 0 0 0 0.0344553 19.9224 ¦ 1 0 0 0 -1 0 0 0 1 -1.09561 2.53715 1.86587 ¦ 1 0 0 0 -1 0 0 0 1 3.29347 3.4301 0.0514984 ¦ 1 0 0 0 0.999873 0.0159503 0 -0.0159503 0.999873 4.66535 12.6378 28.3042 ¦ 1 0 0 0 -1 0 0 0 1 -0.580139 -0.0706339 23.3558 ¦ 0.90617 -0.0158173 -0.422618 -0.0174523 -0.999848 0 0.422554 -0.00737534 0.906308 -0.0932226 -0.0173895 27.0013 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/position` | block | [num num num]x18350 | 3672 | 18350 | 1.9399 -10.2529 20.0263 ¦ 3.2935 -2.6165 0.0515 ¦ -0.9987 -1.8803 1.7145 ¦ 2.0768 10.1794 20.2598 ¦ 13.7429 0.0247 27.5947 ¦ 13.7429 0.0246876 27.5947 ¦ 3.29347 -3.36689 0.0514994 ¦ -1.09561 -2.34785 1.86587 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/speed` | block | [num]x3671 | 3671 | 3671 | 0.1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/speed2` | block | [num]x1 | 1 | 1 | 720 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/visibility` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/volumeview` | block | [str]x3 | 3 | 3 | "skin.ply" ¦ "1stp.ply" | 추정(파일 주석) | (Retained for LOD1 for fidelity.) | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/limits` | block | [num num]x3672 | 3672 | 3672 | -30 30 | 미확인(자동) |  |  |
| `/skeleton/bone/orientation` | block | [num num num num num num num num num]x3672 | 3672 | 3672 | 1 0 0 0 -1 0 0 0 1 | 미확인(자동) |  |  |
| `/skeleton/bone/speed` | block | [num]x3671 | 3671 | 3671 | 0.1 | 미확인(자동) |  |  |
| `/skeleton/bone/speed2` | block | [num]x1 | 1 | 1 | 720 | 미확인(자동) |  |  |
| `/skeleton/extension` | block | [str]x2 | 2 | 2 | "/properties/animation/human/human_anm.ext" | 미확인(자동) |  |  |
| `/volume` | block | [str]x12 | 1 | 12 | "body" ¦ "head" ¦ "face" ¦ "hand1l" ¦ "hand2l" ¦ "hand1r" ¦ "hand2r" ¦ "foot1r" ¦ "foot2r" ¦ "foot1l" ¦ "foot2l" ¦ "ram" | 미확인(자동) |  |  |
| `/volume/bone` | block | [str]x12 | 1 | 12 | "head" ¦ "ik_updown" ¦ "hand1l" ¦ "hand2l" ¦ "hand1r" ¦ "hand2r" ¦ "foot1r" ¦ "foot2r" ¦ "foot1l" ¦ "foot2l" ¦ "ram" | 미확인(자동) |  |  |
| `/volume/box` | block | [num num num]x1 | 1 | 1 | 4 6.75 9.5 | 미확인(자동) |  |  |
| `/volume/cylinder` | block | [num num]x11 | 1 | 11 | 1 6.5 ¦ 1 5.5 ¦ 1.8 10.5 ¦ 1.65 10 ¦ 1.5 1.75 ¦ 1.5 3.5 ¦ 8.0 33.5 | 미확인(자동) |  |  |
| `/volume/disabled` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/volume/matrix34` | block | [num num num num num num num num num num num num]x9 | 1 | 9 | 0 0 1 0 1 0 -1 0 0 3 0 0 ¦ 1 0 0 0 0 -1 0 1 0 0 1.6 0 ¦ 0.173648 0 0.984808 -0.254887 0.965926 0.044943 -0.951251 -0.258819 0.167731 3.5 -0.5 0.15 ¦ 0.087156 0 -0.996195 -0.257834 0.965926 -0.022558 0.96225 0.258819 0.084186 3.5 -0.5 -0.15 ¦ -0.000547874 0.999998 0.00169417 -0.0113967 0.00168782 -0.999934 -0.999935 -0.000567006 0.0113954 4.59069 -0.0514774 -0.0236297 ¦ -0.0340668 0.999419 -0.00146551 -0.0112599 -0.00184951 -0.999935 -0.999356 -0.0340486 0.0113172 5.35503 -0.0714047 -0.0133916 ¦ -0.00145271 0.999994 0.00300585 0.0202548 -0.0029758 0.99979 -0.999794 -0.00151317 0.0202504 4.5898 -0.0468472 -0.0931758 ¦ -0.0330716 0.999449 -0.00263528 0.0200897 0.00330076 0.999793 -0.999251 -0.0330119 0.0201878 5.3503 -0.068715 -0.207656 | 추정(파일 주석) | 0 90 0 ¦ 15 85 0 | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/position` | block | [num num num]x3 | 1 | 3 | 0 0 3.75 ¦ 0 0 1 ¦ 0 0 16.75 | 미확인(자동) |  |  |

### 파일 종류: .mtl

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/material` | block | [atom]x1127 | 1127 | 1127 | bump ¦ simple | 미확인(자동) |  |  |
| `/material/blend` | block | [atom]x1127 | 1127 | 1127 | test ¦ none ¦ blend ¦ add | 미확인(자동) |  |  |
| `/material/bump` | block | [str]x1061 | 1061 | 1061 | 예: "$/_hum/civ_clothing/civ_bp" | 미확인(자동) |  |  |
| `/material/color` | block | [str]x676 | 676 | 676 | "230 230 230 25" ¦ "255 255 255 255" | 미확인(자동) |  |  |
| `/material/diffuse` | block | [str]x1127 | 1127 | 1127 | 예: "$/_hum/civ_clothing/civ_01" | 미확인(자동) |  |  |
| `/material/emitsheat` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/material/full_specular` | block | []x449 | 449 | 449 | 예: | 미확인(자동) |  |  |
| `/material/specular` | block | [str]x1061 | 1061 | 1061 | 예: "$/_hum/civ_clothing/civ_sp" | 미확인(자동) |  |  |

### 파일 종류: .set

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/breed` | block | []x5083 | 5083 | 5083 | 예: | 미확인(자동) |  |  |
| `/breed/armors` | block | []x5025 | 5025 | 5025 | 예: | 미확인(자동) |  |  |
| `/breed/armors/head` | block | [atom]x5007 | 5007 | 5007 | 예: civilian_dockyard_cap | 미확인(자동) |  |  |
| `/breed/behaviour` | block | [atom]x5083 | 5083 | 5083 | soldier | 미확인(자동) |  |  |
| `/breed/field_icon` | block | [str]x21 | 21 | 21 | "rus_miner_ap" ¦ "rus_miner_at" ¦ "usa_engineer_ap" ¦ "usa_engineer_at" ¦ "rus_pilot" ¦ "border_jag_ap" ¦ "border_jag_at" ¦ "ger_miner_ap" ¦ "ger_miner_at" ¦ "rus_artilleryman" | 미확인(자동) |  |  |
| `/breed/hero` | block | []x84 | 84 | 84 | 예: | 미확인(자동) |  |  |
| `/breed/icon` | block | [str]x4970 [atom]x74 | 5044 | 5044 | 예: "bazooka" | 미확인(자동) |  |  |
| `/breed/icon_priority` | block | [num]x5044 | 5044 | 5044 | 0 ~ 52 | 미확인(자동) |  |  |
| `/breed/inventory` | block | []x4970 | 4970 | 4970 | 예: | 미확인(자동) |  |  |
| `/breed/inventory/in_hands` | block | [num]x4835 | 4835 | 4835 | 0 ~ 4 | 미확인(자동) |  |  |
| `/breed/inventory/item` | block | [str num num]x17805 [str num]x7022 [str]x5376 [str atom]x2916 [str atom str num]x2871 | 4970 | 36135 | 예: "piat" filling "piat_bomb ammo heata" 1 | 추정(파일 주석) | 128+8 ¦ 80+48+8 | 게임 파일 내 주석(같은 이름의 키) |
| `/breed/nationality` | block | [atom]x5083 | 5083 | 5083 | 예: fra_civilian | 미확인(자동) |  |  |
| `/breed/perks` | block | []x5054 | 5054 | 5054 | 예: | 미확인(자동) |  |  |
| `/breed/perks/"*"` | block | [str atom]x430 [str str]x215 [str]x113 [str num]x86 | 62 | 844 | 예: "rocketlauncher" lvl_1 | 미확인(자동) |  |  |
| `/breed/perks/"*"/factor` | block | [num]x43 | 43 | 43 | 1 | 미확인(자동) |  |  |
| `/breed/perks/"*"/fortifications` | block | [str str]x35 [str str str]x8 | 43 | 43 | "foxhole" "medic_rally" ¦ "foxhole" "medic_rally" "auto_medic_place_flag" | 미확인(자동) |  |  |
| `/breed/perks/("at_rank_2")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("at_rank_3")` | macro | []x231 | 231 | 231 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("at_rank_4")` | macro | []x66 | 66 | 66 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("at_rank_5")` | macro | []x79 | 79 | 79 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("at_rank_6")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("binocular_vision")` | macro | []x217 | 217 | 217 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("civilian")` | macro | []x39 | 39 | 39 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("elite_gun")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("engineer_heavy_fin")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("engineer")` | macro | []x90 | 90 | 90 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("expert_gun")` | macro | []x84 | 84 | 84 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_rank_1")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_rank_2")` | macro | []x155 | 155 | 155 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_rank_3")` | macro | []x117 | 117 | 117 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_rank_4")` | macro | []x89 | 89 | 89 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_rank_5")` | macro | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_smg_rank_2")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_smg_rank_3")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("loader_skill_smg_rank_4")` | macro | []x13 | 13 | 13 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("marksman")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("medic")` | macro | []x66 | 66 | 66 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("mg_skill_rank_1")` | macro | []x21 | 21 | 21 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("mg_skill_rank_2")` | macro | []x108 | 108 | 108 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("mg_skill_rank_3")` | macro | []x88 | 88 | 88 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("mg_skill_rank_4")` | macro | []x102 | 102 | 102 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("mg_skill_rank_5")` | macro | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("rifle_skill_rank_1")` | macro | []x155 | 155 | 155 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("rifle_skill_rank_2")` | macro | []x1167 | 1167 | 1167 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("rifle_skill_rank_3")` | macro | []x946 | 946 | 946 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("rifle_skill_rank_4")` | macro | []x615 | 615 | 615 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("rifle_skill_rank_5")` | macro | []x96 | 96 | 96 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("skill_rank_0")` | macro | []x39 | 39 | 39 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("skilled_gun")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("skilled_repair")` | macro | []x156 | 156 | 156 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("smg_skill_rank_1")` | macro | []x47 | 47 | 47 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("smg_skill_rank_2")` | macro | []x298 | 298 | 298 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("smg_skill_rank_3")` | macro | []x266 | 266 | 266 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("smg_skill_rank_4")` | macro | []x337 | 337 | 337 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("smg_skill_rank_5")` | macro | []x52 | 52 | 52 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("sniper_vision")` | macro | []x134 | 134 | 134 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("stealth")` | macro | []x184 | 184 | 184 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tank_commander")` | macro | []x127 | 127 | 127 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tankman")` | macro | []x109 | 109 | 109 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier1_defense")` | macro | []x112 | 112 | 112 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier1_standard")` | macro | []x158 | 158 | 158 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier2_defense")` | macro | []x326 | 326 | 326 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier2_recon")` | macro | []x182 | 182 | 182 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier2_standard")` | macro | []x1338 | 1338 | 1338 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier3_defense")` | macro | []x233 | 233 | 233 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier3_recon")` | macro | []x99 | 99 | 99 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier3_standard")` | macro | []x1100 | 1100 | 1100 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier4_defense")` | macro | []x242 | 242 | 242 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier4_recon")` | macro | []x102 | 102 | 102 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier4_standard")` | macro | []x841 | 841 | 841 | 예: | 미확인(자동) |  |  |
| `/breed/perks/("tier5_standard")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/breed/perks/(include)` | include | [str]x5011 | 5011 | 5011 | "ability.inc" | 미확인(자동) |  |  |
| `/breed/portrait` | block | [str]x5003 [atom]x80 | 5083 | 5083 | 예: "civilian2" | 미확인(자동) |  |  |
| `/breed/skin` | block | [str]x5083 | 5083 | 5083 | 예: "civ_01_var1" | 추정(파일 주석) | m41 uniform, haversack, rifle pouch ¦ m41 uniform, haversack, rifle pouch, general purpose bag | 게임 파일 내 주석(같은 이름의 키) |
| `/breed/tags` | block | [str]x4380 | 4380 | 4380 | "soldier" ¦ "crew" ¦ "soldier_bazooka" ¦ "soldier_pzfaust" ¦ "soldier_atr" ¦ "soldier_atr_grenade" ¦ "soldier_pzscheck" ¦ "" | 미확인(자동) |  |  |
| `/breed/veterancy` | block | []x5022 | 5022 | 5022 | 예: | 미확인(자동) |  |  |
| `/breed/veterancy/"*"` | block | [str num]x176 | 22 | 176 | 예: "lvl_1" 4 | 미확인(자동) |  |  |
| `/breed/veterancy/("veterancy_lvl_0")` | macro | []x4037 | 4037 | 4037 | 예: | 미확인(자동) |  |  |
| `/breed/veterancy/("veterancy_lvl_3")` | macro | []x323 | 323 | 323 | 예: | 미확인(자동) |  |  |
| `/breed/veterancy/("veterancy_lvl_5")` | macro | []x627 | 627 | 627 | 예: | 미확인(자동) |  |  |
| `/breed/veterancy/("veterancy_lvl_8")` | macro | []x13 | 13 | 13 | 예: | 미확인(자동) |  |  |
| `/breed/veterancy/(include)` | include | [str]x5000 | 5000 | 5000 | "ability.inc" | 미확인(자동) |  |  |
| `/breed/voice` | block | [atom]x423 | 420 | 423 | eng ¦ eng_pol ¦ usa_officer ¦ eng_officer ¦ usa_sniper ¦ usa_medic ¦ ger_officer ¦ eng_medic ¦ ger_sniper ¦ ger_medic ¦ usa_speirs ¦ usa_winters | 미확인(자동) |  |  |

## 펼친 뒤의 키

include와 매크로를 펼친 뒤 유닛 안에서의 위치다. 펼치는 규칙은 추정이며 한계는 coverage.md에 있다. 고유 키 184개.

### 유닛 종류: breed

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/breed` | block | []x5083 | 5083 | 5083 | 예: | 미확인(자동) |  |  |
| `/breed/armors` | block | []x5025 | 5025 | 5025 | 예: | 미확인(자동) |  |  |
| `/breed/armors/head` | block | [atom]x5007 | 5007 | 5007 | 예: civilian_dockyard_cap | 미확인(자동) |  |  |
| `/breed/behaviour` | block | [atom]x5083 | 5083 | 5083 | soldier | 미확인(자동) |  |  |
| `/breed/field_icon` | block | [str]x21 | 21 | 21 | "rus_miner_ap" ¦ "rus_miner_at" ¦ "usa_engineer_ap" ¦ "usa_engineer_at" ¦ "rus_pilot" ¦ "border_jag_ap" ¦ "border_jag_at" ¦ "ger_miner_ap" ¦ "ger_miner_at" ¦ "rus_artilleryman" | 미확인(자동) |  |  |
| `/breed/hero` | block | []x84 | 84 | 84 | 예: | 미확인(자동) |  |  |
| `/breed/icon` | block | [str]x4970 [atom]x74 | 5044 | 5044 | 예: "bazooka" | 미확인(자동) |  |  |
| `/breed/icon_priority` | block | [num]x5044 | 5044 | 5044 | 0 ~ 52 | 미확인(자동) |  |  |
| `/breed/inventory` | block | []x4970 | 4970 | 4970 | 예: | 미확인(자동) |  |  |
| `/breed/inventory/in_hands` | block | [num]x4835 | 4835 | 4835 | 0 ~ 4 | 미확인(자동) |  |  |
| `/breed/inventory/item` | block | [str num num]x17805 [str num]x7022 [str]x5376 [str atom]x2916 [str atom str num]x2871 | 4970 | 36135 | 예: "piat" filling "piat_bomb ammo heata" 1 | 추정(파일 주석) | 128+8 ¦ 80+48+8 | 게임 파일 내 주석(같은 이름의 키) |
| `/breed/nationality` | block | [atom]x5083 | 5083 | 5083 | 예: fra_civilian | 미확인(자동) |  |  |
| `/breed/perks` | block | []x5054 | 5054 | 5054 | 예: | 미확인(자동) |  |  |
| `/breed/perks/"*"` | block | [str atom]x57183 [str str]x25758 [str num]x10385 [str]x782 | 5054 | 94108 | 예: "rocketlauncher" lvl_1 | 미확인(자동) |  |  |
| `/breed/perks/"*"/factor` | block | [num]x43 | 43 | 43 | 1 | 미확인(자동) |  |  |
| `/breed/perks/"*"/fortifications` | block | [str str]x35 [str str str]x8 | 43 | 43 | "foxhole" "medic_rally" ¦ "foxhole" "medic_rally" "auto_medic_place_flag" | 미확인(자동) |  |  |
| `/breed/perks/barricade` | block | []x5170 | 5011 | 5170 | 예: | 미확인(자동) |  |  |
| `/breed/perks/barricade/"*"` | block | [str str]x11213 | 5011 | 11213 | 예: "fortifications" "foxhole" | 미확인(자동) |  |  |
| `/breed/perks/fortification` | block | []x93 | 93 | 93 | 예: | 미확인(자동) |  |  |
| `/breed/perks/mechanic` | block | [atom num]x5496 | 5011 | 5496 | factor 0.5 ¦ factor 1.0 | 미확인(자동) |  |  |
| `/breed/portrait` | block | [str]x5003 [atom]x80 | 5083 | 5083 | 예: "civilian2" | 미확인(자동) |  |  |
| `/breed/skin` | block | [str]x5083 | 5083 | 5083 | 예: "civ_01_var1" | 추정(파일 주석) | m41 uniform, haversack, rifle pouch ¦ m41 uniform, haversack, rifle pouch, general purpose bag | 게임 파일 내 주석(같은 이름의 키) |
| `/breed/tags` | block | [str]x4380 | 4380 | 4380 | "soldier" ¦ "crew" ¦ "soldier_bazooka" ¦ "soldier_pzfaust" ¦ "soldier_atr" ¦ "soldier_atr_grenade" ¦ "soldier_pzscheck" ¦ "" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/breed/veterancy` | block | []x5022 | 5022 | 5022 | 예: | 미확인(자동) |  |  |
| `/breed/veterancy/"*"` | block | [str num]x80176 | 5022 | 80176 | 예: "lvl_1" 4 | 미확인(자동) |  |  |
| `/breed/veterancy/(mod)` | mod | [str]x5000 | 5000 | 5000 | "mp" | 미확인(자동) |  |  |
| `/breed/voice` | block | [atom]x423 | 420 | 423 | eng ¦ eng_pol ¦ usa_officer ¦ eng_officer ¦ usa_sniper ¦ usa_medic ¦ ger_officer ¦ eng_medic ¦ ger_sniper ¦ ger_medic ¦ usa_speirs ¦ usa_winters | 미확인(자동) |  |  |

### 유닛 종류: humanskin

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/game_entity` | block | []x3666 | 3666 | 3666 | 예: | 추정(파일 주석) | june '41 ¦ sept 44 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extension` | block | [str]x3666 | 3666 | 3666 | 예: "civ_01_var1.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/nocastshadows` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/nogetshadows` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/rimlight` | block | []x3664 | 3664 | 3664 | 예: | 미확인(자동) |  |  |
| `/game_entity/tag` | block | [str]x1 | 1 | 1 | "rus soldier" | 추정(파일 주석) | game code ¦ fx code | 게임 파일 내 주석(같은 이름의 키) |
| `/human` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | fallback value if no other subordinated case is applicable ¦ -TARGET CLASS- | 게임 파일 내 주석(같은 이름의 키) |
| `/human/able` | block | [str str str str]x1 | 1 | 1 | "crew" "fight" "minimap" "personage" | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/human/bone` | block | [str]x2 | 1 | 2 | "foresight2" ¦ "basis" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/human/bone/decalview` | block | [str]x4 | 1 | 4 | "selection_selected" ¦ "selection_selectable" ¦ "selection_active" ¦ "mineDetectionDecal" | 미확인(자동) |  |  |
| `/human/bone/decalview/dynamic` | block | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |
| `/human/bone/decalview/ground` | block | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |
| `/human/bone/decalview/layer` | block | [atom]x4 | 1 | 4 | alpha ¦ grounddynamic | 미확인(자동) |  |  |
| `/human/bone/decalview/offset` | block | [num num num]x3 | 1 | 3 | 0 0 10 | 미확인(자동) |  |  |
| `/human/bone/decalview/sprite` | block | [str]x4 | 1 | 4 | "selected.ebm" ¦ "selectable.ebm" ¦ "active.ebm" ¦ "demine.ebm" | 미확인(자동) |  |  |
| `/human/bone/decalview/sprite/light` | block | [num]x4 | 1 | 4 | 0.2 ~ 1 | 미확인(자동) |  |  |
| `/human/bone/decalview/sprite/prescalex` | block | [num]x4 | 1 | 4 | 0.085 ~ 0.4 | 미확인(자동) |  |  |
| `/human/bone/decalview/sprite/prescaley` | block | [num]x4 | 1 | 4 | 0.085 ~ 0.4 | 미확인(자동) |  |  |
| `/human/bone/decalview/sprite/preturn` | block | [num]x2 | 1 | 2 | 90 | 미확인(자동) |  |  |
| `/human/bone/decalview/usedirection` | block | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |
| `/human/bone/decalview/visible` | block | [num]x4 | 1 | 4 | 0 | 추정(파일 주석) | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 게임 파일 내 주석(같은 이름의 키) |
| `/human/bone/speed` | block | [num]x1 | 1 | 1 | 5 | 추정(파일 주석) | velocity of bullet in meters per second ¦ m/s | 게임 파일 내 주석(같은 이름의 키) |
| `/human/brain` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/brain/const` | block | [str num]x5 | 1 | 5 | "bomb/detect_explosives" 7 ¦ "bomb/detect_grenades" 5 ¦ "bomb/danger_zone" 7 ¦ "fight/distance" 2 ¦ "weapon/change_time" 5 | 추정(파일 주석) | detect explosives inside 7 meter radius ¦ hide if danger is more than "x" meters away, otherwise run away | 게임 파일 내 주석(같은 이름의 키) |
| `/human/brain/properties` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/brain/properties/advance_ratio` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  |
| `/human/brain/properties/retreat_ratio` | block | [num]x1 | 1 | 1 | 4 | 미확인(자동) |  |  |
| `/human/chassis` | block | [str]x1 | 1 | 1 | "foot" | 추정(파일 주석) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/crawl` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | "crawl" stance - in cover ¦ "crawl" stance - moving. Also serves as default if other conditions are not defined in this block. | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/crawl/fast` | block | [num num num]x1 | 1 | 1 | 6 9 180 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/crawl/normal` | block | [num num num]x1 | 1 | 1 | 4.5 7 180 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/crawl/slow` | block | [num num num]x1 | 1 | 1 | 3.5 6 180 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/injured` | block | [num]x1 | 1 | 1 | 0.85 | 추정(파일 주석) | speed multiplier when health is below 30% of maximum | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/run` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/chassis/run/bow` | block | [num num num]x1 | 1 | 1 | 16 5.0 600 | 추정(파일 주석) | mission script movement mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/run/fast` | block | [num num num]x1 | 1 | 1 | 18 5.8 600 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/run/normal` | block | [num num num]x1 | 1 | 1 | 13 6.2 600 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/run/slow` | block | [num num num]x1 | 1 | 1 | 8 3.2 360 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/run/sneak` | block | [num num num]x1 | 1 | 1 | 5 2 360 | 추정(파일 주석) | mission script movement mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/run/spot` | block | [num num num]x1 | 1 | 1 | 6 2 420 | 추정(파일 주석) | Manual TPC Zoom | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/run/walk` | block | [num num num]x1 | 1 | 1 | 4 1.8 600 | 추정(파일 주석) | single flick, walk mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/strafe` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/chassis/strafe/back` | block | [num num num num num str str]x1 | 1 | 1 | 8 0 0 10.5 1.22 "run_back_spot_gun_up" "run_back_spot_gun_down" | 미확인(자동) |  |  |
| `/human/chassis/strafe/forth` | block | [num num num num num str str]x3 | 1 | 3 | 3 2 0 6.5 3 "walk_spot_gun_up_1" "walk_spot_gun_down_1" ¦ 3 2 1 11.0 1 "run_spot_gun_up" "run_spot_gun_down" ¦ 8 0 0 13.2 1.2 "run_spot_gun_up" "run_spot_gun_down" | 추정(파일 주석) | 6.5 km/h move 2 km/h animation 0.37 scale --> in Run | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/strafe/left` | block | [num num num num num str str]x1 | 1 | 1 | 4 0 0 10.5 1.22 "run_spot_back-strafe_gun_up" "run_spot_back-strafe_gun_down" | 추정(파일 주석) | Top sides ¦ side | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/strafe/right` | block | [num num num num num str str]x1 | 1 | 1 | 8 0 0 13.0 1.1 "run_spot_left-strafe_gun_up" "run_spot_left-strafe_gun_down" | 추정(파일 주석) | armor plate sides ¦ Top sides | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/swim` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/chassis/swim/fast` | block | [num num num]x1 | 1 | 1 | 5 5 180 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/swim/normal` | block | [num num num]x1 | 1 | 1 | 3 5 180 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/chassis/swim/slow` | block | [num num num]x1 | 1 | 1 | 2 5 180 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/human/collider` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/extender[inventory]` | block | [str]x1 | 1 | 1 | "inventory" | 미확인(자동) |  |  |
| `/human/extender[inventory]/armor` | block | [str]x3 | 1 | 3 | "head" ¦ "body" ¦ "accessories" | 미확인(자동) |  |  |
| `/human/extender[inventory]/armor/bone` | block | [str]x3 | 1 | 3 | "head" ¦ "IK_UpDown" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[inventory]/armor/heatdamping` | block | [num]x1 | 1 | 1 | 0.6 | 미확인(자동) |  |  |
| `/human/extender[inventory]/armor/mask` | block | [str]x3 | 1 | 3 | "head" ¦ "body" ¦ "head2" | 미확인(자동) |  |  |
| `/human/extender[inventory]/armor/pierceabsorb` | block | [num]x3 | 1 | 3 | 20 ~ 25 | 미확인(자동) |  |  |
| `/human/extender[inventory]/armor/volumes` | block | [str]x3 | 1 | 3 | "head" ¦ "body" | 추정(파일 주석) | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[inventory]/box` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/extender[inventory]/box/size` | block | [num num]x1 | 1 | 1 | 8 12 | 추정(파일 주석) | size of the window | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[inventory]/box/weight` | block | [num]x1 | 1 | 1 | 100 | 추정(파일 주석) | will stack up in 100/2 = 2 on humans | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[inventory]/hand_left` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/extender[inventory]/hand_right` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/extender[resources]` | block | [str]x1 | 1 | 1 | "resources" | 미확인(자동) |  |  |
| `/human/extender[resources]/resources` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/extender[resources]/resources/current` | block | [num]x1 | 1 | 1 | 10 | 추정(파일 주석) | current stamina ¦ current health | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[resources]/resources/income` | block | [num]x1 | 1 | 1 | 0.1 | 추정(파일 주석) | resources regeneration per second | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[resources]/resources/max` | block | [num]x1 | 1 | 1 | 10 | 추정(파일 주석) | max health ¦ max resources | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[stuff]` | block | [str]x1 | 1 | 1 | "stuff" | 미확인(자동) |  |  |
| `/human/extender[stuff]/item` | block | [str]x1 | 1 | 1 | "human" | 추정(파일 주석) | 128+8 ¦ 80+48+8 | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[talk]` | block | [str]x1 | 1 | 1 | "talk" | 미확인(자동) |  |  |
| `/human/extender[talk]/talker` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/extender[vitality]` | block | [str]x1 | 1 | 1 | "vitality" | 미확인(자동) |  |  |
| `/human/extender[vitality]/(mod)` | mod | [str]x1 | 1 | 1 | "hardcore" | 미확인(자동) |  |  |
| `/human/extender[vitality]/damage` | block | []x2 | 1 | 2 | 예: | 추정(파일 주석) | no blast damage. Used to ensure ground detonation fx are played ¦ = 150mm of penetration | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/damage/"*"` | block | [str num]x22 | 1 | 22 | 예: "head" 1.3 | 미확인(자동) |  |  |
| `/human/extender[vitality]/damageblast` | block | [num]x2 | 1 | 2 | 1 ~ 1.2 | 미확인(자동) |  |  |
| `/human/extender[vitality]/damagetpc` | block | []x2 | 1 | 2 | 예: | 추정(파일 주석) | Third person and direct control | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/damagetpc/"*"` | block | [str num]x22 | 1 | 22 | 예: "head" 1.75 | 미확인(자동) |  |  |
| `/human/extender[vitality]/health` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | A better way to translate this is that it will deal 2x damage to the enemy. In the editor it is hard to understand, as if you're switching between players it will always display the hitpoints of "player". But the enemy units on that particular instance will have half the total health. ¦ health multiplier from soldier settings | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/health/current` | block | [num]x1 | 1 | 1 | 100 | 추정(파일 주석) | current stamina ¦ current health | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/health/inc` | block | [num]x1 | 1 | 1 | 1 | 추정(파일 주석) | stamina regeneration/usage per second (depending on movement mode and load) ¦ health regeneration per second | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/health/max` | block | [num]x1 | 1 | 1 | 100 | 추정(파일 주석) | max health ¦ max resources | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/extender[vitality]/stamina/current` | block | [num]x1 | 1 | 1 | 100 | 추정(파일 주석) | current stamina ¦ current health | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/inc` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | stamina regeneration/usage per second (depending on movement mode and load) ¦ health regeneration per second | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/inc/fast` | block | [num num num num]x1 | 1 | 1 | -3 -3 -3 -3 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/inc/normal` | block | [num num num num]x1 | 1 | 1 | +2 +2 +1.5 +1 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/inc/slow` | block | [num num num num]x1 | 1 | 1 | +3 +3 +2.5 +2 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/inc/stay` | block | [num num num num]x1 | 1 | 1 | +3 +3 +2.5 +2 | 미확인(자동) |  |  |
| `/human/extender[vitality]/stamina/inc/walk` | block | [num num num num]x1 | 1 | 1 | +3 +3 +2.5 +2 | 추정(파일 주석) | single flick, walk mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/load` | block | [num num num num]x1 | 1 | 1 | 0 25 50 75 | 추정(파일 주석) | equipment weight (in kg) | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/max` | block | [num]x1 | 1 | 1 | 100 | 추정(파일 주석) | max health ¦ max resources | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/movement_limits` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/extender[vitality]/stamina/movement_limits/fast` | block | [num num]x1 | 1 | 1 | 1 10 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/movement_limits/normal` | block | [num num]x1 | 1 | 1 | 0 0 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extender[vitality]/stamina/movement_limits/slow` | block | [num num]x1 | 1 | 1 | -1 -1 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/human/extension` | block | [str]x2 | 1 | 2 | "human.mdl" ¦ "human_fsm.ext" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/human/lodclass` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/mass` | block | [num]x1 | 1 | 1 | 70 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/human/nogroupmesh` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  |
| `/human/novisibilitypatch` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/obstacle` | block | []x1 [str]x1 | 1 | 2 | ¦ "impassable" | 미확인(자동) |  |  |
| `/human/obstacle/circle2` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/human/obstacle/circle2/radius` | block | [num]x2 | 1 | 2 | 5 ~ 6 | 추정(파일 주석) | resupply radius in meter | 게임 파일 내 주석(같은 이름의 키) |
| `/human/obstacleid` | block | [str]x1 [str str]x1 | 1 | 2 | "human" ¦ "human_impassable" "impassable" | 미확인(자동) |  |  |
| `/human/patherid` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/props` | block | [str str str str]x1 | 1 | 1 | "flesh" "human" "nocover" "collectable" | 미확인(자동) |  |  |
| `/human/rimlight` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/sensor/attackzone` | block | [num]x1 | 1 | 1 | 2.5 | 미확인(자동) |  |  |
| `/human/sensor/informrange` | block | [num num]x1 | 1 | 1 | 80 100 | 추정(파일 주석) | first = AI , second = USER | 게임 파일 내 주석(같은 이름의 키) |
| `/human/sensor/minrecognizetime` | block | [num]x1 | 1 | 1 | 0.01 | 미확인(자동) |  |  |
| `/human/sensor/recognizetime` | block | [num]x1 | 1 | 1 | 0.01 | 미확인(자동) |  |  |
| `/human/sensor/visor` | block | [str]x4 | 1 | 4 | "eyes" ¦ "peripheral" ¦ "hearing" ¦ "telescope" | 미확인(자동) |  |  |
| `/human/sensor/visor/bone` | block | [str]x3 | 1 | 3 | "visor" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/human/sensor/visor/vision` | block | [str]x3 | 1 | 3 | "human" ¦ "human_peripheral" ¦ "noise_detection" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/human/simulation` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/simulation/able` | block | [num]x1 | 1 | 1 | 1 | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/human/simulation/state` | block | [num]x1 | 1 | 1 | 0 | 추정(파일 주석) | 2hand & 1hand items ¦ to enable item ops in snipe cover | 게임 파일 내 주석(같은 이름의 키) |
| `/human/simulatorid` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/skeleton` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/skeleton/viewvolume` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/skeleton/viewvolume/center` | block | [num num num]x1 | 1 | 1 | 0 0 18 | 미확인(자동) |  |  |
| `/human/skeleton/viewvolume/extent` | block | [num num num]x1 | 1 | 1 | 30 30 18 | 미확인(자동) |  |  |
| `/human/targetclass` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/targetselector` | block | [str]x1 | 1 | 1 | "human" | 미확인(자동) |  |  |
| `/human/volume` | block | [str]x17 | 1 | 17 | 예: "body" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/human/volume/able` | block | []x10 | 1 | 10 | 예: | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/human/volume/able/bullet` | block | [num]x1 | 1 | 1 | 0 | 추정(파일 주석) | required for AT-Grenades with cumaltive damage | 게임 파일 내 주석(같은 이름의 키) |
| `/human/volume/able/contact` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/able/contact_ground` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/able/nolook` | block | [num]x9 | 1 | 9 | 1 | 미확인(자동) |  |  |
| `/human/volume/able/obstacle` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/able/overlap` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/human/volume/able/touch` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/human/volume/density` | block | [num]x1 | 1 | 1 | 0.007 | 미확인(자동) |  |  |
| `/human/volume/tags` | block | [str]x9 [str str]x7 | 1 | 16 | "no_target" "fire" ¦ "no_target" ¦ "fire" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/human/volume/thickness` | block | [num]x11 | 1 | 11 | 2 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/human/weaponry` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/weaponry/manualaccuracy` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/human/weaponry/manualaccuracyzoom` | block | [num]x1 | 1 | 1 | 1.5 | 미확인(자동) |  |  |
| `/human/weaponry/place` | block | [str]x1 | 1 | 1 | "hand_right" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/human/weaponry/place/accuracybymotion` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/weaponry/place/accuracybymotion/byangularspeed` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  |
| `/human/weaponry/place/accuracybymotion/bylinearspeed` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  |
| `/human/weaponry/place/accuracybymotion/disordertime` | block | [num]x1 | 1 | 1 | 0.5 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/human/weaponry/place/accuracybymotion/maxangularspeed` | block | [num]x1 | 1 | 1 | 25 | 추정(파일 주석) | deg/s | 게임 파일 내 주석(같은 이름의 키) |
| `/human/weaponry/place/accuracybymotion/maxlinearspeed` | block | [num]x1 | 1 | 1 | 15 | 추정(파일 주석) | km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/human/weaponry/place/accuracybymotion/ordertime` | block | [num]x1 | 1 | 1 | 1 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/human/weaponry/place/aimbone` | block | [str]x1 | 1 | 1 | "aim_ref" | 미확인(자동) |  |  |
| `/human/weaponry/place/linkanimation` | block | [str]x1 | 1 | 1 | "on" | 미확인(자동) |  |  |
| `/human/weaponry/place/linkbone` | block | [str]x1 | 1 | 1 | "right_hand" | 미확인(자동) |  |  |
| `/human/weaponry/place/rotate` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/weaponry/place/sequentialaim` | block | [num]x1 | 1 | 1 | 0 | 추정(파일 주석) | was at 1 | 게임 파일 내 주석(같은 이름의 키) |
| `/human/weaponry/tpcinstantaiming` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/human/weaponry/uselookataiming` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |

