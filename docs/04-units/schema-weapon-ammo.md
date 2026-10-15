# 스키마: weapon-ammo

이 파일은 `tools/New-SchemaDoc.ps1`이 생성한다. 손으로 고치지 않는다. 의미와 등급은 `data/annotations/common.psv`에서 고친다. 마지막 열은 같은 이름의 키 옆에 달린 게임 파일의 주석을 이름만으로 모은 참고 자료이며, 이 경로의 뜻과 다를 수 있다.

이 그룹은 `set/stuff` 아래의 아이템이다: 무기, 탄약, 수류탄, 지뢰, 헬멧과 장구.

| 파일 종류 | 내용 | 파일 수 |
|---|---|---|
| 확장자 없음 | 아이템 정의. 총기, 헬멧 등 | 2,094 |
| `.ammo` | 탄약 | 518 |
| `.weapon` | 무기 일부(로켓, 박격포 등) | 54 |
| `.grenade`, `.mine` | 수류탄, 지뢰 | 55 |
| `.pattern` | 원형. `{from "pattern 이름"}`이 가리키는 대상 | 64 |
| `.presets` | 폴더별 매크로 모음. 그 폴더의 파일이 include 없이 쓴다 | 14 |

아이템 대부분은 `{from "pattern 이름" ...}`으로 시작한다. 그 파일에는 원형과 다른 값만 적혀 있고, 원형의 값은 `.pattern` 파일에 있다. 두 값이 합쳐지는 규칙은 확인하지 못했으므로 이 문서는 둘을 따로 싣는다: 아래 "펼친 뒤의 키"에서 유닛 종류 `stuff`가 아이템 파일, `pattern`이 원형 파일이다.

탄속, 발사 간격, 장전 시간은 대개 `("velocity" mps(830))` 같은 매크로로 적혀 있고 `.presets`의 정의가 실제 키(`speed`, `recoveryTime`, `rechargeTime`)로 바꾼다.

## 원본 파일에 적힌 키

파일에 쓰인 그대로의 위치다. 고유 키 1785개.

### 파일 종류: .ammo

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/bullet` | block | []x120 | 120 | 120 | 예: | 추정 | 탄 정의의 최상위 블록 (추정: bullet.pattern) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/bullet/("he_heavy")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/bullet/("he_level_6")` | macro | [call:weight call:filler]x1 | 1 | 1 | weight ( 175 ) filler ( 49 ) | 미확인(자동) |  |  |  |
| `/bullet/##trace` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/bullet/##trace/length` | block | [num]x1 | 1 | 1 | 15 | 확인 | 애니메이션 한 번에 해당하는 길이, 미터 | 게임 파일 주석 |  |
| `/bullet/##trace/view` | block | [str]x1 | 1 | 1 | "/texture/fx/tracers/trace_empty" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 |  |
| `/bullet/##trace/view/light` | block | [num]x1 | 1 | 1 | 3 | 미확인 | 값 1 |  |  |
| `/bullet/##trace/view/scaley` | block | [num]x1 | 1 | 1 | 0.2 | 미확인(자동) |  |  |  |
| `/bullet/cumulative` | block | [num]x7 | 7 | 7 | 0.7 ~ 1 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/bullet/damage` | block | [atom]x14 [atom atom]x2 | 13 | 16 | blastwave ¦ add blastwave ¦ fire | 추정 | 피해 정의. 값은 피해 종류 | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/bullet/damage/area` | block | [num num]x15 | 13 | 15 | 0.2 3.5 ¦ 0.75 7.5 ¦ 3 6 ¦ 3.5 9.5 ¦ 0.5 4 ¦ 3 9 ¦ 1.53 2.55 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/bullet/damage/energy` | block | [num]x15 | 13 | 15 | 0.7 ~ 1.1 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/bullet/damage/ground_interaction_radius` | block | [num]x15 | 13 | 15 | 0 ~ 0.2 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/bullet/damage/heating` | block | [num]x1 | 1 | 1 | 10 | 미확인(자동) |  |  |  |
| `/bullet/damage/radius` | block | [num]x1 | 1 | 1 | 1.05 | 추정 | 피해가 미치는 반경, 미터 (추정: 이름. 값 1.05) | 파일 구조와 값에서 추론 |  |
| `/bullet/damage/time` | block | [num]x1 | 1 | 1 | 0.5 | 추정 | 피해가 이어지는 시간, 초 (추정: 이름과 원문 주석 "seconds") | 파일 구조와 값에서 추론 |  |
| `/bullet/detonationthickness` | block | [num]x6 | 6 | 6 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/bullet/entity` | block | [str]x120 | 120 | 120 | 예: "sturmtiger_ammo" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/bullet/inventory` | block | []x120 | 120 | 120 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/bullet/inventory/block` | block | [num]x103 | 103 | 103 | 5 ~ 300 | 미확인 | 탄약 파일의 inventory 안 값. 인벤토리 칸에 보이는 묶음 크기는 아니다: 250→37, 100→13으로 바꿔도 M26 Pershing 인벤토리에 4500발, 450발이 한 칸씩 그대로 보였다(2026-10-14 게임 확인). 무엇을 정하는지는 미확인 | 파일 구조와 값에서 추론 |  |
| `/bullet/inventory/fsm` | block | [str]x99 | 99 | 99 | "belt" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/bullet/inventory/size` | block | [num num]x120 | 120 | 120 | 1 1 ¦ 1 2 ¦ 2 1 ¦ 3 2 ¦ 2 2 ¦ 3 1 ¦ 3 3 ¦ 6 2 ¦ 8 4 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/bullet/inventory/weight` | block | [num]x120 | 120 | 120 | 0.333 ~ 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/bullet/mass` | block | [num]x120 | 120 | 120 | 0.006 ~ 175 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/bullet/tag` | block | [str]x189 [str str]x21 | 120 | 210 | 예: "flame" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/bullet/trace` | block | []x78 | 78 | 78 | 예: | 미확인(자동) |  |  |  |
| `/bullet/trace/length` | block | [num]x78 | 78 | 78 | 1.5 ~ 3 | 확인 | 애니메이션 한 번에 해당하는 길이, 미터 | 게임 파일 주석 |  |
| `/bullet/trace/view` | block | [str]x78 | 78 | 78 | "/texture/fx/tracers/trace_empty" ¦ "/texture/fx/tracer_new/trace_solid_red.mtl" ¦ "/texture/fx/tracer_new/trace_solid_orange.mtl" ¦ "/texture/fx/tracer_new/trace_solid_light_green.mtl" ¦ "/texture/fx/tracer_new/trace_solid_yellow.mtl" ¦ "/texture/fx/tracer_new/trace_scattered_green.mtl" ¦ "/texture/fx/tracer_new/trace_scattered_red.mtl" ¦ "/texture/fx/tracer_new/trace_solid_green.mtl" ¦ "/texture/fx/tracer_new/trace_scattered_white.mtl" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 |  |
| `/bullet/trace/view/light` | block | [num]x78 | 78 | 78 | 1 | 미확인 | 값 1 |  |  |
| `/bullet/trace/view/scalex` | block | [num]x78 | 78 | 78 | 1.5 ~ 2 | 미확인(자동) |  |  |  |
| `/bullet/trace/view/scaley` | block | [num]x78 | 78 | 78 | 0.1 ~ 0.2 | 미확인(자동) |  |  |  |
| `/bullet/view` | block | [str]x36 | 36 | 36 | 예: "300mm_fin_mortar_he" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 |  |
| `/bullet/view/tail` | block | [str str]x23 | 23 | 23 | "flare_tail_red" "fxshell" ¦ "flare_tail_green" "fxshell" ¦ "shell_tail" "tail" ¦ "hook_tail" "tail" | 추정 | 탄의 꼬리 이펙트. 이펙트 이름과 붙이는 뼈대 (추정: 값 "shell_tail" "tail") | 파일 구조와 값에서 추론 |  |
| `/from` | block | [str]x398 | 398 | 398 | 예: "pattern rpg_heat_ammo" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/("ap_blast")` | macro | [atom num]x78 | 78 | 78 | 예: args 1.1 | 미확인(자동) |  |  |  |
| `/from/("apcr_blast")` | macro | [atom num]x23 | 23 | 23 | args 0.78 ¦ args 0.4 ¦ args 0.35 ¦ args 0.9 ¦ args 0.27 ¦ args 0.5 ¦ args 0.8 ¦ args 0.86 | 미확인(자동) |  |  |  |
| `/from/("he_auto_blast")` | macro | [atom num]x15 | 15 | 15 | args 0.6 ¦ args 0.65 ¦ args 1.2 ¦ args 0.55 | 미확인(자동) |  |  |  |
| `/from/("he_heavy")` | macro | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/("he_level_1")` | macro | [call:weight call:filler]x37 | 37 | 37 | 예: weight ( 1.25 ) filler ( 0.270 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_2_offmap")` | macro | [call:weight call:filler]x2 | 2 | 2 | weight ( 11.33 ) filler ( 0.825 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_2")` | macro | [call:weight call:filler]x40 | 40 | 40 | 예: weight ( 7.2 ) filler ( 1.417 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_3")` | macro | [call:weight call:filler]x40 | 40 | 40 | 예: weight ( 15 ) filler ( 1.474 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_4")` | macro | [call:weight call:filler]x26 | 26 | 26 | 예: weight ( 45 ) filler ( 4.5 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_5")` | macro | [call:weight call:filler]x15 | 15 | 15 | weight ( 90.7 ) filler ( 16.66 ) ¦ weight ( 91.17 ) filler ( 12.64 ) ¦ weight ( 97.5 ) filler ( 10.00 ) ¦ weight ( 156.5 ) filler ( 24.52 ) ¦ weight ( 51.4 ) filler ( 10.3 ) ¦ weight ( 90.7 ) filler ( 13.4 ) ¦ weight ( 90.7 ) filler ( 13.2 ) ¦ weight ( 68 ) filler ( 7.33 ) ¦ weight ( 100 ) filler ( 12.17 ) ¦ weight ( 112.94 ) filler ( 17.35 ) ¦ weight ( 97.5 ) filler ( 8 ) ¦ weight ( 100 ) filler ( 15.36 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_6")` | macro | [call:weight call:filler]x8 | 8 | 8 | weight ( 103 ) filler ( 36 ) ¦ weight ( 103 ) filler ( 43.5 ) ¦ weight ( 50 ) filler ( 24.4 ) ¦ weight ( 60 ) filler ( 25 ) ¦ weight ( 45 ) filler ( 24.5 ) ¦ weight ( 90 ) filler ( 27 ) ¦ weight ( 286 ) filler ( 58.7 ) ¦ weight ( 380 ) filler ( 58 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_7")` | macro | [call:weight call:filler]x5 | 5 | 5 | weight ( 226 ) filler ( 97 ) ¦ weight ( 250 ) filler ( 130 ) ¦ weight ( 250 ) filler ( 79 ) ¦ weight ( 249.5 ) filler ( 117.4 ) ¦ weight ( 226 ) filler ( 162.81 ) | 미확인(자동) |  |  |  |
| `/from/("he_level_8")` | macro | [call:weight call:filler]x2 | 2 | 2 | weight ( 2170 ) filler ( 289 ) ¦ weight ( 1700 ) filler ( 220 ) | 미확인(자동) |  |  |  |
| `/from/("he_light")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("he_medium")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |  |
| `/from/("heat_blast")` | macro | [atom num]x3 | 3 | 3 | args 1.5 ¦ args 0.27 ¦ args 0.4 | 미확인(자동) |  |  |  |
| `/from/("heat_large_blast")` | macro | [atom num]x1 | 1 | 1 | args 6.5 | 미확인(자동) |  |  |  |
| `/from/("heat_medium_blast")` | macro | [atom num]x2 | 2 | 2 | args 5.5 | 미확인(자동) |  |  |  |
| `/from/("minimum_range_medium")` | macro | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/("rocket_level_2")` | macro | [call:weight call:filler]x2 | 2 | 2 | weight ( 6.8 ) filler ( 0.64 ) ¦ weight ( 6.8 ) filler ( 0.360 ) | 미확인(자동) |  |  |  |
| `/from/("rocket_level_3")` | macro | [call:weight call:filler]x5 | 5 | 5 | weight ( 18.143 ) filler ( 2.31 ) ¦ weight ( 21.91 ) filler ( 0.947 ) ¦ weight ( 20.8 ) filler ( 3.45 ) ¦ weight ( 31.8 ) filler ( 2.5 ) ¦ weight ( 15.53 ) filler ( 2.67 ) | 미확인(자동) |  |  |  |
| `/from/("rocket_level_4")` | macro | [call:weight call:filler]x2 | 2 | 2 | weight ( 43 ) filler ( 4.9 ) ¦ weight ( 27.4 ) filler ( 6 ) | 미확인(자동) |  |  |  |
| `/from/("rocket_level_5")` | macro | [call:weight call:filler]x1 | 1 | 1 | weight ( 109.6 ) filler ( 10.17 ) | 미확인(자동) |  |  |  |
| `/from/("rocket_level_6")` | macro | [call:weight call:filler]x3 | 3 | 3 | weight ( 82 ) filler ( 50 ) ¦ weight ( 94.8 ) filler ( 28.9 ) ¦ weight ( 127 ) filler ( 45 ) | 미확인(자동) |  |  |  |
| `/from/("rocket_level_7")` | macro | [call:weight call:filler]x1 | 1 | 1 | weight ( 345 ) filler ( 125 ) | 미확인(자동) |  |  |  |
| `/from/("smoke_blast")` | macro | [call:cal]x5 | 5 | 5 | cal ( 81 ) ¦ cal ( 100 ) ¦ cal ( 200 ) ¦ cal ( 75 ) | 미확인(자동) |  |  |  |
| `/from/("wp")` | macro | [call:cal]x20 | 20 | 20 | cal ( 122 ) ¦ cal ( 155 ) ¦ cal ( 107 ) ¦ cal ( 81 ) ¦ cal ( 114 ) ¦ cal ( 75 ) ¦ cal ( 50 ) ¦ cal ( 120 ) ¦ cal ( 82 ) ¦ cal ( 60 ) ¦ cal ( 94 ) ¦ cal ( 105 ) | 미확인(자동) |  |  |  |
| `/from/##damage` | block | [atom atom]x18 [atom]x12 | 12 | 30 | add blastwave ¦ blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 |  |
| `/from/##damage/area` | block | [num num]x30 | 12 | 30 | 예: 0.4 0.5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/##damage/energy` | block | [num]x30 | 12 | 30 | 1.195 ~ 10.93 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/##damage/ground_interaction_radius` | block | [num]x30 | 12 | 30 | 0 ~ 1 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/cumulative` | block | [num]x1 | 1 | 1 | 1 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/from/damage` | block | [atom]x17 [atom atom]x5 | 18 | 22 | blastwave ¦ fire ¦ add blastwave | 추정 | 피해 정의. 값은 피해 종류: blastwave, fire. add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/area` | block | [num num]x14 [expr expr]x1 | 13 | 15 | 예: 1.8 3 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/damage/energy` | block | [num]x13 [expr]x2 | 13 | 15 | 0.45 ~ 14 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/damage/ground_interaction_radius` | block | [num]x15 | 13 | 15 | 0 ~ 0.25 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/damage/heating` | block | [num]x7 | 7 | 7 | 0.5 ~ 20 | 미확인(자동) |  |  |  |
| `/from/damage/radius` | block | [num]x7 | 7 | 7 | 0.5 ~ 3.5 | 추정 | 피해가 미치는 반경, 미터 (추정: 이름. 값 1.05) | 파일 구조와 값에서 추론 |  |
| `/from/damage/time` | block | [num]x7 | 7 | 7 | 0.5 ~ 1 | 추정 | 피해가 이어지는 시간, 초 (추정: 이름과 원문 주석 "seconds") | 파일 구조와 값에서 추론 |  |
| `/from/detonationthickness` | block | [num]x1 | 1 | 1 | 1 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/from/entity` | block | [str]x397 | 397 | 397 | 예: "bazooka_heat_ammo" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/from/inventory` | block | []x374 | 374 | 374 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/block` | block | [num]x42 | 42 | 42 | 5 ~ 10 | 미확인(자동) |  |  |  |
| `/from/inventory/noview` | block | []x40 | 40 | 40 | 예: | 미확인(자동) |  |  |  |
| `/from/inventory/size` | block | [num num]x334 | 334 | 334 | 예: 4 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/from/inventory/weight` | block | [num]x69 | 69 | 69 | 10 ~ 200 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/from/mass` | block | [num]x357 | 357 | 357 | 0.17 ~ 2170 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/from/possibledamageradius` | block | [num]x9 | 9 | 9 | 30 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/tag` | block | [str]x446 [str str str str]x1 | 394 | 447 | 예: "heat_shell size1" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from/trace` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/trace/length` | block | [num]x4 | 4 | 4 | 3 ~ 20 | 확인 | 애니메이션 한 번에 해당하는 길이, 미터 | 게임 파일 주석 |  |
| `/from/trace/view` | block | [str]x4 | 4 | 4 | "/texture/fx/tracer_new/trace_solid_red.mtl" ¦ "/texture/fx/tracer_new/trace_solid_orange.mtl" ¦ "/texture/fx/tracer_new/trace_solid_white.mtl" ¦ "/texture/fx/tracers/trace_zenite" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 |  |
| `/from/trace/view/light` | block | [num]x3 | 3 | 3 | 1 | 미확인 | 값 1 |  |  |
| `/from/trace/view/scalex` | block | [num]x3 | 3 | 3 | 1.75 | 미확인(자동) |  |  |  |
| `/from/trace/view/scaley` | block | [num]x4 | 4 | 4 | 0.15 ~ 0.22 | 미확인(자동) |  |  |  |
| `/from/view` | block | [str]x343 | 343 | 343 | 예: "bazooka_heat_ammo" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 |  |
| `/from/view/tail` | block | [str str]x331 | 331 | 331 | 예: "bazooka_tail" "tail" | 추정 | 탄의 꼬리 이펙트. 이펙트 이름과 붙이는 뼈대 (추정: 값 "shell_tail" "tail") | 파일 구조와 값에서 추론 |  |

### 파일 종류: .fsm

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/state` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |  |
| `/state/forhold` | block | [str]x6 | 6 | 6 | "hold-item" ¦ "hold-bazooka" | 추정 | 이 종류의 아이템을 들고 있을 때의 요청 이름 (추정: 이름과 값 "hold-item") | 파일 구조와 값에서 추론 |  |
| `/state/formount` | block | [str]x6 | 6 | 6 | "bazooka-pick" ¦ "belt-pick" ¦ "inventory-pick-stand" ¦ "pistol-pick" ¦ "inventory-pick" ¦ "item-pick" | 추정 | 이 종류의 아이템을 꺼내 들 때 보내는 요청 이름 (추정: 이름과 값 "pistol-pick") | 파일 구조와 값에서 추론 |  |
| `/state/forpick` | block | [str]x6 | 6 | 6 | "pickup-hand" ¦ "pickup-stand" | 추정 | 이 종류의 아이템을 집을 때 보내는 요청 이름 (추정: 값 "pickup-hand", "pickup-stand"가 공식 문서의 요청 목록에 있음) | 파일 구조와 값에서 추론 |  |
| `/state/forthrow` | block | [str]x2 | 2 | 2 | "throw" | 추정 | 이 종류의 아이템을 던질 때 보내는 요청 이름 (추정: 이름과 값 "throw") | 파일 구조와 값에서 추론 |  |
| `/state/forunmount` | block | [str]x6 | 6 | 6 | "inventory-drop" ¦ "bazooka-drop" ¦ "belt-drop" ¦ "pistol-drop" ¦ "item-drop" | 추정 | 이 종류의 아이템을 집어넣거나 내려놓을 때 보내는 요청 이름 (추정: 이름과 값 "item-drop") | 파일 구조와 값에서 추론 |  |

### 파일 종류: .grenade

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/from` | block | [str]x39 | 39 | 39 | "pattern smoke" ¦ "pattern anti-tank" ¦ "pattern anti-personnel" ¦ "pattern throwable" ¦ "pattern anti-personnel_contact" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/damage` | block | [atom atom]x23 [atom]x19 | 26 | 42 | add blastwave ¦ blastwave ¦ cumulative | 추정 | 피해 정의. 값은 피해 종류: blastwave, fire. add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/area` | block | [num num]x38 | 26 | 38 | 4 10 ¦ 4.5 6 ¦ 3 5 ¦ 0.25 0.35 ¦ 0.01 0.25 ¦ 4 6 ¦ 1 2 ¦ 0.4 0.55 ¦ 0.6 2 ¦ 0.25 0.4 ¦ 3 6 ¦ 2.5 5.5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/damage/damage` | block | [num]x4 | 4 | 4 | 31 ~ 150 | 미확인 | 수류탄의 damage 하위. 값 31 ~ 150 |  | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/distance` | block | [num]x4 | 4 | 4 | 0.25 | 미확인(자동) |  |  | activation distance |
| `/from/damage/energy` | block | [num]x38 | 26 | 38 | 0 ~ 14 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/damage/ground_interaction_radius` | block | [num]x38 | 26 | 38 | 0 ~ 1 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/entity` | block | [str]x39 | 39 | 39 | 예: "f1" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/from/inventory` | block | []x33 | 33 | 33 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/size` | block | [num num]x33 | 33 | 33 | 2 1 ¦ 1 1 ¦ 3 1 ¦ 3 2 ¦ 1 2 ¦ 4 1 ¦ 2 2 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/from/mass` | block | [num]x25 | 25 | 25 | 0.383 ~ 3 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/from/possibledamageradius` | block | [num]x1 | 1 | 1 | 0.1 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/special` | block | []x11 | 11 | 11 | 예: | 미확인 | 하위에 mode, preparetime, preparecursor |  | {throw_off_always} |
| `/from/special/mode` | block | [atom]x4 | 4 | 4 | detonate | 미확인 | 값 detonate |  |  |
| `/from/special/preparecursor` | block | [str]x4 | 4 | 4 | "grenade" | 추정 | 준비 중 커서 (추정: 이름과 값 "grenade") | 파일 구조와 값에서 추론 |  |
| `/from/special/preparetime` | block | [num]x11 | 11 | 11 | 3.6 ~ 7 | 추정 | 사용 전 준비 시간 (추정: 이름과 값 2~8) | 파일 구조와 값에서 추론 |  |
| `/from/tag` | block | [str]x41 | 39 | 41 | "smoke" ¦ "size1" ¦ "size2" ¦ "size3" ¦ "ic ap fire at" ¦ "smoke launcher" ¦ "grenade dropboxed antitank mine" ¦ "size4" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from/throw` | block | []x13 | 13 | 13 | 예: | 추정 | 투척 설정 묶음 (추정: 하위 range, spreading, startangle) | 파일 구조와 값에서 추론 |  |
| `/from/throw/constantspeed` | block | [num]x5 | 5 | 5 | 12 ~ 14.5 | 미확인(자동) |  |  |  |
| `/from/throw/range` | block | [num]x13 | 13 | 13 | 8 ~ 28.7 | 추정 | 던지는 거리 (추정: throw의 하위) | 파일 구조와 값에서 추론 | 200m ¦ 285m max |
| `/from/throw/spreading` | block | [num]x4 | 4 | 4 | 0.1 ~ 1 | 추정 | 던질 때의 산포 (추정: throw의 하위) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/throw/spreading/spreadpower` | block | [num]x3 | 3 | 3 | 2 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/from/throw/startangle` | block | [num]x4 | 4 | 4 | 80 | 추정 | 던지는 각도 (추정: 이름과 값 10~80) | 파일 구조와 값에서 추론 |  |

### 파일 종류: .hand

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/weapon_throwing` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_throwing/aimingtolerance` | block | [num]x1 | 1 | 1 | 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/weapon_throwing/aimrange` | block | [num]x1 | 1 | 1 | 30 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/weapon_throwing/filling` | block | [str]x1 | 1 | 1 | "throwable" | 추정 | 장전하는 탄의 태그 (추정: 값 "throwable") | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_throwing/foresight` | block | [str]x1 | 1 | 1 | "Foresight2" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/weapon_throwing/gravity` | block | [num]x1 | 1 | 1 | 1 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/weapon_throwing/inventory` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_throwing/inventory/noview` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_throwing/states` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_throwing/states/aim` | block | [str]x1 | 1 | 1 | "aim-thrower" | 미확인(자동) |  |  |  |
| `/weapon_throwing/states/can_aim` | block | [str]x1 | 1 | 1 | "can-aim-thrower" | 미확인(자동) |  |  |  |
| `/weapon_throwing/states/fire` | block | [str]x1 | 1 | 1 | "fire" | 미확인(자동) |  |  |  |
| `/weapon_throwing/states/reload` | block | [str]x1 | 1 | 1 | "reload" | 미확인(자동) |  |  |  |
| `/weapon_throwing/states/unload` | block | [str]x1 | 1 | 1 | "unload" | 미확인(자동) |  |  |  |
| `/weapon_throwing/thirdpersonparams` | block | [atom str]x1 | 1 | 1 | inherit "human default" | 미확인(자동) |  |  |  |
| `/weapon_throwing/thirdpersonparams/clip` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_throwing/thirdpersonparams/clip/distancemax` | block | [num]x1 | 1 | 1 | 170 | 미확인(자동) |  |  |  |
| `/weapon_throwing/thirdpersonparams/level` | block | [num]x1 | 1 | 1 | 0 | 추정 | 3인칭 조준의 확대 단계 하나. 값은 단계 번호 (추정: 값 1, 2와 하위 camera) | 파일 구조와 값에서 추론 | 2 |
| `/weapon_throwing/thirdpersonparams/level/delta` | block | [num]x1 | 1 | 1 | 30 | 미확인(자동) |  |  |  |
| `/weapon_throwing/thirdpersonparams/level/faroffset` | block | [num num num]x1 | 1 | 1 | 0 11 65 | 미확인(자동) |  |  |  |

### 파일 종류: .mine

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/from` | block | [str]x16 | 16 | 16 | "pattern antitank" ¦ "pattern antipersonnel" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/damage` | block | [atom atom]x8 [atom]x5 | 5 | 13 | add blastwave ¦ blastwave | 추정 | 피해 정의. 값은 피해 종류: blastwave, fire. add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/area` | block | [num num]x13 | 5 | 13 | 0.6 2 ¦ 3 5 ¦ 6 8 ¦ 2.5 5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/damage/energy` | block | [num]x13 | 5 | 13 | 0.25 ~ 1 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/damage/ground_interaction_radius` | block | [num]x13 | 5 | 13 | 0 ~ 0.5 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/entity` | block | [str]x16 | 16 | 16 | 예: "mine_ap_shrapnel_mk1" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/from/inventory` | block | []x5 | 5 | 5 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/size` | block | [num num]x5 | 5 | 5 | 1 2 ¦ 2 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/from/mass` | block | [num]x11 | 11 | 11 | 1.5 ~ 8 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |

### 파일 종류: .pattern

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/bullet` | block | []x8 | 8 | 8 | 예: | 추정 | 탄 정의의 최상위 블록 (추정: bullet.pattern) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/bullet/cumulative` | block | [num]x3 | 3 | 3 | 0.7 ~ 3 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/bullet/damage` | block | [atom]x3 | 3 | 3 | blastwave | 추정 | 피해 정의. 값은 피해 종류 | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/bullet/damage/area` | block | [num num]x3 | 3 | 3 | 2 3.5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/bullet/damage/energy` | block | [num]x3 | 3 | 3 | 2.5 ~ 3 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/bullet/damage/ground_interaction_radius` | block | [num]x3 | 3 | 3 | 0.1 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/bullet/detonationthickness` | block | [num]x2 | 2 | 2 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/bullet/inventory` | block | []x7 | 7 | 7 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/bullet/inventory/fsm` | block | [str]x7 | 7 | 7 | "belt" ¦ "stuff" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/bullet/inventory/size` | block | [num num]x6 | 6 | 6 | 3 1 ¦ 5 1 ¦ 4 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/bullet/inventory/weight` | block | [num]x7 | 7 | 7 | 100 ~ 1000 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/bullet/mass` | block | [num]x1 | 1 | 1 | 2.75 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/bullet/possibledamageradius` | block | [num]x4 | 4 | 4 | 3 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/bullet/tag` | block | [str]x8 | 8 | 8 | "ammo rocketlauncher heat_shell heat dropboxed pierce no_ricochet" ¦ "ammo shell all_he fg dropboxed no_ricochet" ¦ "ammo rocketlauncher heat_shell dropboxed pierce no_ricochet" ¦ "ammo shell fg wp smoke wp_size1 dropboxed no_ricochet" ¦ "bomb fg itemin2hands build_in" ¦ "dropboxed distributable" ¦ "rocketselector rocketlauncher pierce no_ricochet" ¦ "ammo itemin2hands" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from` | block | [str]x43 | 43 | 43 | "pattern generic" ¦ "pattern throwable" ¦ "pattern bullet" ¦ "pattern rifle" ¦ "pattern mgun_generic" ¦ "pattern he" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/("accuracy_hmg")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_mg")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_pistol")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_rifle_at")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_rifle_auto_sniper")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_rifle_sniper")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_smg")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("ap_12_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("ap_7_62_slope_multiplier")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("ap_slope_multiplier")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("armor health")` | macro | [call:damage]x6 | 6 | 6 | damage ( 4 ) ¦ damage ( 13 ) ¦ damage ( 1 ) ¦ damage ( 45 ) ¦ damage ( 2 ) | 미확인(자동) |  |  |  |
| `/from/("at_rifle_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_medium")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("fire rate")` | macro | [call:rpm]x4 | 4 | 4 | rpm ( 300 ) ¦ rpm ( 29 ) ¦ rpm ( 175 ) ¦ rpm ( 24 ) | 미확인(자동) |  |  |  |
| `/from/("human health")` | macro | [call:damage]x8 | 8 | 8 | damage ( 180 ) ¦ damage ( 80 ) ¦ damage ( 60 ) ¦ damage ( 350 ) ¦ damage ( 160 ) ¦ damage ( 70 ) ¦ damage ( 100 ) ¦ damage ( 55 ) | 미확인(자동) |  |  |  |
| `/from/("penetration")` | macro | [atom num num num num num]x3 [atom num num]x3 [atom num]x1 | 7 | 7 | args 7 5 3 1 0 ¦ args 6.5 3.5 ¦ args 3 0 ¦ args 4 | 미확인(자동) |  |  |  |
| `/from/("recoil_hmg_medium")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_mg_medium")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_pistol")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_rifle_at")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_smg_medium")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_sniper_bolt")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_sniper_semi")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("reload time")` | macro | [call:seconds]x8 | 8 | 8 | seconds ( 5 ) ¦ seconds ( 6 ) ¦ seconds ( 3.25 ) ¦ seconds ( 10 ) | 미확인(자동) |  |  |  |
| `/from/("spreading_medium")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("sway_mg")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("sway_pistol")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("sway_rifle")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("sway_smg")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("sway_sniper")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/(mod)` | mod | [str]x1 | 1 | 1 | "mp" | 미확인(자동) |  |  |  |
| `/from/accuracybymotionk` | block | [num]x3 | 3 | 3 | 0.1 ~ 0.9 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/from/aimingtolerance` | block | [num]x6 | 6 | 6 | 1 ~ 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/from/automatic` | block | []x3 | 3 | 3 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/from/cumulative` | block | [num]x4 | 4 | 4 | 3 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/from/cursor` | block | [str]x8 | 8 | 8 | "ironsights/mg" ¦ "ironsights/smg" ¦ "ironsights/aa" ¦ "ironsights/bazooka" ¦ "ironsights/rifle" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/from/cursorreloading` | block | [str]x5 | 5 | 5 | "gun_ap" ¦ "carbine_reload" ¦ "gun_heat" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/curve` | block | [str]x8 | 8 | 8 | "bullet_sniper" ¦ "bullet_hmg" ¦ "bullet_mg" ¦ "bullet_pistol" ¦ "gun_std" ¦ "bullet" ¦ "bullet_smg" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/from/damage` | block | [atom]x3 [atom atom]x1 | 3 | 4 | blastwave ¦ add blastwave | 추정 | 피해 정의. 값은 피해 종류: blastwave, fire. add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/area` | block | [num num]x4 | 3 | 4 | 3 8 ¦ 8 12 ¦ 3 6 ¦ 3 5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/damage/energy` | block | [num]x4 | 3 | 4 | 1.1 ~ 15 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/damage/ground_interaction_radius` | block | [num]x4 | 3 | 4 | 0 ~ 0.15 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/detonationthickness` | block | [num]x4 | 4 | 4 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/from/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/firingtimeout` | block | [num num]x8 | 8 | 8 | 3.5 0.5 ¦ 2.0 0.5 ¦ 1.5 0.5 ¦ 1.75 0.5 ¦ 1.0 0.5 ¦ 2.5 0.5 | 확인 | 연사 사이의 지연 | 게임 파일 주석 | 3.5 0.5 ¦ delay between bursts |
| `/from/firingtimeoutdistancefactor` | block | []x6 | 6 | 6 | 예: | 미확인 | 하위에 두 수의 행 |  |  |
| `/from/firingtimeoutdistancefactor/#row` | block | [num num]x16 | 6 | 16 | 0.8 1.0 ¦ 1.0 1.0 ¦ 0.3 0.5 ¦ 0.25 0.0 ¦ 0.75 1.0 ¦ 0.25 0.6 ¦ 0.8 0.85 ¦ 0.2 0.55 ¦ 0.4 0.75 ¦ 0.2 0.5 ¦ 0.4 0.8 ¦ 0.6 0.9 | 미확인 | 두 수 |  |  |
| `/from/foresight` | block | [str]x7 | 7 | 7 | "foresight14" ¦ "foresight12" ¦ "foresight3" ¦ "foresight18" ¦ "Foresight11" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/from/gravity` | block | [num]x1 | 1 | 1 | 2 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/from/inventory` | block | []x11 | 11 | 11 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/fsm` | block | [str]x1 | 1 | 1 | "pistol" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/from/inventory/link_bone` | block | [str]x5 | 5 | 5 | "gun_back" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/from/inventory/size` | block | [num num]x11 | 11 | 11 | 3 1 ¦ 7 2 ¦ 3 2 ¦ 2 1 ¦ 8 2 ¦ 6 2 ¦ 5 2 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/from/inventory/weight` | block | [num]x7 | 7 | 7 | 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/from/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/from/mass` | block | [num]x5 | 5 | 5 | 0.3 ~ 3 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/from/overmatchslope` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/overmatchslope/#row` | block | [num num]x2 | 1 | 2 | 0 1 ¦ 90 1 | 미확인(자동) |  |  |  |
| `/from/parameters` | block | [str]x1 | 1 | 1 | "api" | 추정 | 탄종별 설정. 문자열이 탄종 이름 (추정: 값 "heat", "clip") | 파일 구조와 값에서 추론 | SPgGr. ¦ Sprgr. |
| `/from/parameters/cursorreloading` | block | [str]x1 | 1 | 1 | "gun_api" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/possibledamageradius` | block | [num]x5 | 5 | 5 | 0.1 ~ 1 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/from/range` | block | [num num]x7 | 7 | 7 | 120 120 ¦ 140 140 ¦ 70 70 ¦ 130 130 ¦ 80 80 | 추정 | 사거리 두 값 (추정: 원문 주석이 "200m", "285m max". 두 값의 구분은 미확인) | 게임 파일 주석 | 200m ¦ 285m max |
| `/from/relaxationtime` | block | [num]x5 | 5 | 5 | 0.25 ~ 2 | 미확인 | 값 0.25~2 |  |  |
| `/from/reloadanimbonus` | block | [num]x1 | 1 | 1 | 4 | 미확인(자동) |  |  |  |
| `/from/special` | block | []x6 | 6 | 6 | 예: | 미확인 | 하위에 mode, preparetime, preparecursor |  | {throw_off_always} |
| `/from/special/accuracy` | block | [num]x2 | 2 | 2 | 1.5 ~ 2.25 | 미확인(자동) |  |  |  |
| `/from/special/damage` | block | [num]x2 | 2 | 2 | 2 ~ 5 | 미확인 | 값 5. 특수 사격(special) 설정의 하위 |  | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/special/mode` | block | [atom]x4 | 4 | 4 | detonate ¦ aim | 미확인 | 값 detonate |  |  |
| `/from/special/preparecursor` | block | [str]x5 | 5 | 5 | "grenade" ¦ "aimed_shot" ¦ "grenade_sm" | 추정 | 준비 중 커서 (추정: 이름과 값 "grenade") | 파일 구조와 값에서 추론 |  |
| `/from/special/preparecursorscene` | block | [str]x2 | 2 | 2 | "aimed_shot_scene" | 미확인(자동) |  |  |  |
| `/from/special/preparetime` | block | [num]x4 | 4 | 4 | 2 ~ 8 | 추정 | 사용 전 준비 시간 (추정: 이름과 값 2~8) | 파일 구조와 값에서 추론 |  |
| `/from/special/throw_off` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/speed` | block | [num]x1 | 1 | 1 | 55 | 확인 | 탄속. "velocity" 매크로가 0.35 x m/s로 계산해 넣는다 | 게임 파일 주석 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/spreading` | block | []x6 [num]x1 | 7 | 7 | 2 | 추정 | 산포 설정 묶음 또는 산포 값 (추정: 하위 radiustable) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/spreading/burstaccuracy` | block | [num num num num num]x2 [num num num num num num]x1 [num num num num num num num num num]x1 | 4 | 4 | 100 95 90 85 80 ¦ 100 90 80 70 60 ¦ 100 90 80 70 60 50 ¦ 100 95 90 85 80 75 70 65 60 | 확인 | 연사 중 발마다의 정확도. 기본 100이고 첫 발이 100% | 게임 파일 주석 | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/from/spreading/spreadxyratio` | block | [num]x4 | 4 | 4 | 0.75 ~ 1.5 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/from/spreadtolerance` | block | [num]x7 | 7 | 7 | 0.05 ~ 0.3 | 확인 | 사격 전에 필요한 조준 수렴 정도 | 게임 파일 주석 | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/from/states` | block | []x3 | 3 | 3 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/from/states/aim` | block | [str]x3 | 3 | 3 | "aim" ¦ "aim-rgrenade" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/can_aim` | block | [str]x3 | 3 | 3 | "can-aim-bolt" ¦ "can-aim-rgrenade" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/fire` | block | [str]x3 | 3 | 3 | "fire" ¦ "fire-rgrenade" | 추정 | 발사 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/reload` | block | [str]x3 | 3 | 3 | "reload" | 추정 | 장전 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/unload` | block | [str]x3 | 3 | 3 | "unload" | 추정 | 탄 빼기 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/syncedprojectiles` | block | [num]x3 | 3 | 3 | 1 | 확인 | 멀티플레이에서 탄흔·탄공 생성을 시각적으로 동기화한다. 자동화기에는 쓰지 않는다 | 게임 파일 주석 | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/from/tag` | block | [str]x42 | 42 | 42 | 예: "smallitemin1hand grenade dynamite size4 distributable" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from/thirdpersonparams` | block | [atom str]x9 | 9 | 9 | inherit "human default" | 미확인 | 값 inherit "human default" |  |  |
| `/from/throw` | block | []x6 | 6 | 6 | 예: | 추정 | 투척 설정 묶음 (추정: 하위 range, spreading, startangle) | 파일 구조와 값에서 추론 |  |
| `/from/throw/constantspeed` | block | [num]x1 | 1 | 1 | 13.5 | 미확인(자동) |  |  |  |
| `/from/throw/range` | block | [num]x6 | 6 | 6 | 6.2 ~ 25 | 추정 | 던지는 거리 (추정: throw의 하위) | 파일 구조와 값에서 추론 | 200m ¦ 285m max |
| `/from/throw/spreading` | block | [num]x5 | 5 | 5 | 0.05 ~ 0.1 | 추정 | 던질 때의 산포 (추정: throw의 하위) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/throw/spreading/radiusfunction` | block | [num num num num num num num num num num]x1 | 1 | 1 | 0.0 0 0.7 0 1.0 0 1.5 2.0 2.0 5.0 | 미확인(자동) |  |  |  |
| `/from/throw/spreading/spreadpower` | block | [num]x1 | 1 | 1 | 2 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/from/throw/startangle` | block | [num]x6 | 6 | 6 | 10 ~ 80 | 추정 | 던지는 각도 (추정: 이름과 값 10~80) | 파일 구조와 값에서 추론 |  |
| `/from/tracefreq` | block | [num]x3 | 3 | 3 | 3 ~ 5 | 추정 | 예광탄 빈도 (추정: 이름과 값 3~5) | 파일 구조와 값에서 추론 |  |
| `/from/unlimitedrangetpc` | block | [num]x1 | 1 | 1 | 0 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/from/zeroingaccuracy` | block | [num num num]x2 [num num num num num]x1 | 3 | 3 | 0.8 0.7 0.6 ¦ 0.9 0.8 0.75 ¦ 0.9 0.8 0.7 0.6 0.5 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/item` | block | []x7 | 7 | 7 | 예: | 추정 | 아이템 정의의 최상위 블록 (추정: headgear.pattern 등이 이 블록으로 시작) | 파일 구조와 값에서 추론 | Jerry Can (20L) ¦ 55 gallon (~210L) drum |
| `/item/damage` | block | [atom atom]x2 [atom]x1 | 1 | 3 | add blastwave ¦ blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/item/damage/area` | block | [num num]x3 | 1 | 3 | 0.5 0.75 ¦ 2 3 ¦ 6 8 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/item/damage/energy` | block | [num]x3 | 1 | 3 | 1 ~ 9 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/item/damage/ground_interaction_radius` | block | [num]x3 | 1 | 3 | 0 ~ 0.25 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/item/entity` | block | [str]x1 | 1 | 1 | "knife" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/item/inventory` | block | []x8 | 7 | 8 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/item/inventory/fsm` | block | [str]x7 | 7 | 7 | "belt" ¦ "stuff" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/item/inventory/link_bone` | block | [str]x1 | 1 | 1 | "foot1r" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/item/inventory/size` | block | [num num]x4 | 4 | 4 | 2 2 ¦ 3 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/item/inventory/weight` | block | [num]x7 | 7 | 7 | 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/item/mass` | block | [num]x6 | 6 | 6 | 0.03 ~ 8 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/item/tag` | block | [str]x7 | 7 | 7 | "smallitemin1hand head distributable" ¦ "smallitemin1hand dropboxed antipersonnel" ¦ "itemin2hands dropboxed antitank at_mine" ¦ "smallitemin1hand head armor distributable" ¦ "knife sharp_edge smallitemin1hand distributable" ¦ "missile smallitemin1hand throwable" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/item/throw` | block | []x1 | 1 | 1 | 예: | 추정 | 투척 설정 묶음 (추정: 하위 range, spreading, startangle) | 파일 구조와 값에서 추론 |  |
| `/item/throw/spreading` | block | [num]x1 | 1 | 1 | 0.3 | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/weapon_bullet` | block | []x6 | 6 | 6 | 예: | 추정 | 탄을 쏘는 무기 정의의 최상위 블록 (추정: 패턴 파일의 구조) | 파일 구조와 값에서 추론 | Example / Default Settings ¦ required for AT-Grenades with cumaltive damage |
| `/weapon_bullet/("default gravity")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 6 ) | 미확인(자동) |  |  |  |
| `/weapon_bullet/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 2500 ) | 미확인(자동) |  |  |  |
| `/weapon_bullet/("recoil_rifle_bolt")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/("reload time")` | macro | [call:seconds]x2 | 2 | 2 | seconds ( 10 ) ¦ seconds ( 5 ) | 미확인(자동) |  |  |  |
| `/weapon_bullet/("velocity m/s")` | macro | [call:ap call:apcr call:he call:heat]x1 | 1 | 1 | ap ( 100 ) apcr ( 100 ) he ( 100 ) heat ( 100 ) | 미확인(자동) |  |  |  |
| `/weapon_bullet/(mod)` | mod | [str]x1 | 1 | 1 | "hardcore" | 미확인(자동) |  |  |  |
| `/weapon_bullet/accuracybymotionk` | block | [num]x1 | 1 | 1 | 0.1 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/aimingtolerance` | block | [num]x6 | 6 | 6 | 0.5 ~ 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/weapon_bullet/aimrange` | block | [num]x1 | 1 | 1 | 15 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/weapon_bullet/aimtype` | block | [atom]x1 | 1 | 1 | curved | 미확인(자동) |  |  |  |
| `/weapon_bullet/automatic` | block | []x1 | 1 | 1 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/autoreload` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |  |
| `/weapon_bullet/burst` | block | []x1 | 1 | 1 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/burst/long` | block | [num num]x1 | 1 | 1 | 5 1 | 확인 | 긴 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/weapon_bullet/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | rangeKoef x aimRange = 긴 점사를 쓰는 최대 거리 | 게임 파일 주석 | rangeKoef * aimRange = max long burst distance |
| `/weapon_bullet/burst/short` | block | [num num]x1 | 1 | 1 | 10 1 | 확인 | 짧은 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/weapon_bullet/calibre` | block | [num]x1 | 1 | 1 | 7.62 | 확인 | 구경, mm | 게임 파일 주석 | caliber of gun in millimeters ¦ AP shot caliber |
| `/weapon_bullet/clicksound` | block | [str]x2 | 2 | 2 | "trigger_click_01" ¦ "empty_gun_shot" | 미확인(자동) |  |  |  |
| `/weapon_bullet/crosshair` | block | [str]x2 | 2 | 2 | "crosshair_mgun" ¦ "crosshair_tank" | 확인 | 3인칭 조준 때의 조준선 이름 | 게임 파일 주석 | third person crosshair |
| `/weapon_bullet/crosshairreloading` | block | [str]x2 | 2 | 2 | "crosshair_mgun_reload" ¦ "crosshair_tank_reload" | 확인 | 3인칭 조준에서 재장전 중일 때의 조준선 이름 | 게임 파일 주석 | third person reload crosshair. |
| `/weapon_bullet/cursor` | block | [str]x6 | 6 | 6 | "ironsights/tank" ¦ "ironsights/bazooka" ¦ "ironsights/rifle" ¦ "ironsights/tank2" ¦ "ironsights/mortar" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/weapon_bullet/cursorreloading` | block | [str]x5 | 5 | 5 | "carbine_reload" ¦ "gun_heat" ¦ "gun_ic" ¦ "gun_he" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/weapon_bullet/curve` | block | [str]x4 | 4 | 4 | "gun_std" ¦ "bullet" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/weapon_bullet/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/filling` | block | [str]x1 | 1 | 1 | "mortar" | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_bullet/fillingvisible` | block | [str]x2 | 2 | 2 | "rocket" | 미확인(자동) |  |  |  |
| `/weapon_bullet/firesound` | block | [str]x2 | 2 | 2 | "flame/blank_flame" ¦ "mortar\mortar_shot" | 추정 | 발사음 경로 (추정: 이름과 값) | 파일 구조와 값에서 추론 | Placeholder |
| `/weapon_bullet/firesoundclose` | block | [str]x1 | 1 | 1 | "flame/blank_flame" | 추정 | 가까이서 들리는 발사음 경로 (추정: 이름과 값의 3p) | 파일 구조와 값에서 추론 | Placeholder |
| `/weapon_bullet/firingtimeout` | block | [num num]x4 | 4 | 4 | 11 3 ¦ 0.4 0.2 ¦ 0.8 0.2 ¦ 0.35 0.15 | 확인 | 연사 사이의 지연 | 게임 파일 주석 | 3.5 0.5 ¦ delay between bursts |
| `/weapon_bullet/foresight` | block | [str]x6 | 6 | 6 | "foresight3" ¦ "foresight" ¦ "Foresight1" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/weapon_bullet/gravity` | block | [num]x4 | 4 | 4 | 0.65 ~ 6 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/weapon_bullet/inventory` | block | []x4 | 4 | 4 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/inventory/fsm` | block | [str]x3 | 3 | 3 | "smg" ¦ "bazooka" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/weapon_bullet/inventory/handle_bone` | block | [str]x1 | 1 | 1 | "tank_back" | 확인 | 무기를 손에 들지 않고 메고 있을 때 몸에 연결되는 무기 쪽 뼈대 | 게임 파일 주석 | when weapon is shouldered (not equipped) this bone on the weapon is linked to the {link_bone} defined above |
| `/weapon_bullet/inventory/link_bone` | block | [str]x2 | 2 | 2 | "gun_back" ¦ "backpacks" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/inventory/size` | block | [num num]x3 | 3 | 3 | 8 2 ¦ 5 3 ¦ 4 2 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/weapon_bullet/inventory/weight` | block | [num]x3 | 3 | 3 | 100 ~ 1000 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/weapon_bullet/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/weapon_bullet/mass` | block | [num]x1 | 1 | 1 | 1000 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/weapon_bullet/maxrange` | block | [num]x1 | 1 | 1 | 25 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/weapon_bullet/minimumdamagemodifier` | block | [num]x2 | 1 | 2 | 3 | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/weapon_bullet/minrange` | block | [num]x1 | 1 | 1 | 3 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인 | 하위에 세 수의 행. 구경 대 장갑 두께 보정으로 보이나 확인하지 못함 |  |  |
| `/weapon_bullet/overmatchcalibre/#row` | block | [num num num]x19 | 1 | 19 | 예: 0 0.1 0.0637 | 미확인(자동) |  |  |  |
| `/weapon_bullet/overmatchdamping` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | [??] (angle - damping) |
| `/weapon_bullet/overmatchdamping/#row` | block | [num num]x6 | 1 | 6 | 0 1 ¦ 30 1.1 ¦ 55 1.15 ¦ 60 1.45 ¦ 70 4 ¦ 90 6 | 미확인(자동) |  |  |  |
| `/weapon_bullet/overmatchslope` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/overmatchslope/#row` | block | [num num]x2 | 1 | 2 | 0 1 ¦ 90 1 | 미확인(자동) |  |  |  |
| `/weapon_bullet/parameters` | block | [str]x30 | 2 | 30 | 예: "ap" | 추정 | 탄종별 설정. 문자열이 탄종 이름 (추정: 값 "heat", "clip") | 파일 구조와 값에서 추론 | SPgGr. ¦ Sprgr. |
| `/weapon_bullet/parameters/(mod)` | mod | [str]x6 | 1 | 6 | "mp" | 미확인(자동) |  |  |  |
| `/weapon_bullet/parameters/cursorreloading` | block | [str]x30 | 2 | 30 | 예: "gun_ap" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/weapon_bullet/parameters/unlimitedrangetpc` | block | [num]x6 | 1 | 6 | 0 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/weapon_bullet/preloaded` | block | []x6 | 6 | 6 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/weapon_bullet/projectiledamagethreshold` | block | [num]x2 | 2 | 2 | 1 | 확인 | 장갑 × 이 값보다 작은 피해는 무시한다 | 게임 파일 주석 | Damages lower than armor * threshold are ignored |
| `/weapon_bullet/range` | block | [num num]x1 | 1 | 1 | 70 70 | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/weapon_bullet/rechargetime` | block | [num]x1 | 1 | 1 | 0.1 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/weapon_bullet/recoverytime` | block | [num]x1 | 1 | 1 | 0.1 | 확인 | 발사 간격, 초. "fire rate" 매크로가 60 / rpm으로 계산해 넣는다 | 게임 파일 주석 | increase fuel consumption by 2x |
| `/weapon_bullet/reloadanimbonus` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/weapon_bullet/reloadsound` | block | [str]x1 | 1 | 1 | "reload.smg" | 확인 | 장전음 | 게임 파일 주석 | sound for weapon reload |
| `/weapon_bullet/ricochetenergydamping` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  | penetration reduction after ricochet (angle - damping multiplier) ¦ penetration energy damping after ricochet (angle - damping) |
| `/weapon_bullet/ricochetenergydamping/#row` | block | [num num]x10 | 2 | 10 | 0 1 ¦ 60 0 ¦ 20 1 ¦ 30 0.8 ¦ 45 0.4 ¦ 10 0.9 ¦ 25 0.7 ¦ 45 0.3 | 미확인(자동) |  |  |  |
| `/weapon_bullet/speed` | block | [num]x3 | 3 | 3 | 30 ~ 174 | 추정 | 탄속 (추정: 위와 같은 키) | 파일 구조와 값에서 추론 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/weapon_bullet/spreading` | block | [num]x3 []x2 | 5 | 5 | 0.9 ~ 1.75 | 추정 | 산포 설정 묶음 또는 산포 값 | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/weapon_bullet/spreading/burstaccuracy` | block | [num]x1 | 1 | 1 | 100 | 확인 | 연사 중 발마다의 정확도. 기본 100이고 첫 발이 100% | 게임 파일 주석 | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/weapon_bullet/spreading/burstrecoverytime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 사격 뒤 명중률이 평소로 돌아오는 데 걸리는 시간, 초 | 게임 파일 주석 | time in seconds for weapon accuracy to return to normal after firing. |
| `/weapon_bullet/spreading/radiusfunction` | block | [num num num num num num num num num num]x1 | 1 | 1 | 0.0 0 0.85 0.9 1.0 1.3 1.2 6 2 30 | 미확인(자동) |  |  |  |
| `/weapon_bullet/spreading/spreadpower` | block | [num]x4 | 4 | 4 | 1 ~ 2 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/weapon_bullet/spreading/spreadradius` | block | [num]x1 | 1 | 1 | 5 | 확인 | 무기의 산포 반경, 미터 | 게임 파일 주석 | radius of weapon accuracy in meters. Most weapon spread radius is defined in small.firearms.accuracy |
| `/weapon_bullet/spreading/spreadxyratio` | block | [num]x2 | 2 | 2 | 0.7 ~ 3 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/weapon_bullet/spreading/spreadyadjust` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/weapon_bullet/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.01 | 확인 | 사격 전에 필요한 조준 수렴 정도 | 게임 파일 주석 | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/weapon_bullet/states` | block | []x3 | 3 | 3 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/aim` | block | [str]x3 | 3 | 3 | "aim" ¦ "aim-bazooka" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/can_aim` | block | [str]x3 | 3 | 3 | "can-aim" ¦ "can-aim-bazooka" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/fire` | block | [str]x3 | 3 | 3 | "fire" ¦ "fire-bazooka" | 추정 | 발사 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/reload` | block | [str]x3 | 3 | 3 | "reload-bazooka" ¦ "reload" ¦ "" | 추정 | 장전 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/unload` | block | [str]x3 | 3 | 3 | "unload" ¦ "unload-bazooka" | 추정 | 탄 빼기 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/syncedprojectiles` | block | [num]x4 | 4 | 4 | 1 | 확인 | 멀티플레이에서 탄흔·탄공 생성을 시각적으로 동기화한다. 자동화기에는 쓰지 않는다 | 게임 파일 주석 | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/weapon_bullet/tag` | block | [str]x5 | 5 | 5 | "rocketlauncher to_hand ap cannot_lie" ¦ "weapon special_weapon_view flame_thrower fx_thrower bullet to_hand shoot_walking shoot_walking_ai shoot_walking_manual" ¦ "weapon build_in gun bullet tank_gun" ¦ "mrl gun build_in" ¦ "mrl" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/weapon_bullet/toplayervignette` | block | [str]x1 | 1 | 1 | "" | 미확인(자동) |  |  |  |
| `/weapon_bullet/unlimitedrangetpc` | block | [num]x3 | 3 | 3 | 0 ~ 1 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/weapon_bullet/zeroingaccuracy` | block | [num num num num]x1 [num num num num num]x1 | 2 | 2 | 0.7 0.6 0.5 0.4 ¦ 0.85 0.75 0.7 0.65 0.6 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |

### 파일 종류: .presets

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/(define "100_rus_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "100_rus_slope_multipliers")/parameters` | block | [str]x3 | 1 | 3 | "aphe" ¦ "aphebc" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "100_rus_slope_multipliers")/parameters/("ap_100_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "100_rus_slope_multipliers")/parameters/("apbc_100_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "100_rus_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "105_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "105_slope_multipliers")/parameters` | block | [str]x14 | 1 | 14 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "105_slope_multipliers")/parameters/("ap_105_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "105_slope_multipliers")/parameters/("apc_90_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "105_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "105_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "105_slope_multipliers")/parameters/("large_apcr_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "122_rus_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "122_rus_slope_multipliers")/parameters` | block | [str]x3 | 1 | 3 | "aphe" ¦ "aphebc" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "122_rus_slope_multipliers")/parameters/("ap_122_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "122_rus_slope_multipliers")/parameters/("apbc_122_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "122_rus_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "128_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "128_slope_multipliers")/parameters` | block | [str]x3 | 1 | 3 | "aphe" ¦ "apche" ¦ "apcbche" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "128_slope_multipliers")/parameters/("ap_122_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "128_slope_multipliers")/parameters/("apc_128_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "152_rus_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "152_rus_slope_multipliers")/parameters` | block | [str]x3 | 1 | 3 | "aphe" ¦ "aphebc" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "152_rus_slope_multipliers")/parameters/("ap_152_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "152_rus_slope_multipliers")/parameters/("apbc_152_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "152_rus_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "20_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "20_slope_multipliers")/parameters` | block | [str]x4 | 1 | 4 | "ap" ¦ "apbc" ¦ "aphe" ¦ "apcr" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "20_slope_multipliers")/parameters/("ap_20_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "20_slope_multipliers")/parameters/("apcr_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "20-76_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "20-76_slope_multipliers")/parameters` | block | [str]x16 | 1 | 16 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "20-76_slope_multipliers")/parameters/("ap_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "20-76_slope_multipliers")/parameters/("apbc_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "20-76_slope_multipliers")/parameters/("apc_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "20-76_slope_multipliers")/parameters/("apcr_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "20-76_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "20-76_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers_800ms")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers_800ms")/parameters` | block | [str]x12 | 1 | 12 | "ap" ¦ "apbc" ¦ "aphe" ¦ "apc" ¦ "apche" ¦ "apcbc" ¦ "apcbche" ¦ "apcr" ¦ "heat" ¦ "heata" ¦ "heatb" ¦ "heatc" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "37_slope_multipliers_800ms")/parameters/("ap_37_slope_multiplier_800ms")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers_800ms")/parameters/("apc_37_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers_800ms")/parameters/("apcr_50_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers_800ms")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers")/parameters` | block | [str]x12 | 1 | 12 | "ap" ¦ "apbc" ¦ "aphe" ¦ "apc" ¦ "apche" ¦ "apcbc" ¦ "apcbche" ¦ "apcr" ¦ "heat" ¦ "heata" ¦ "heatb" ¦ "heatc" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "37_slope_multipliers")/parameters/("ap_37_slope_multiplier_700ms")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers")/parameters/("apc_37_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers")/parameters/("apcr_50_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "37_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_eng_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_eng_slope_multipliers")/parameters` | block | [str]x7 | 1 | 7 | "ap" ¦ "ap2" ¦ "aphe" ¦ "apcbc" ¦ "apcr" ¦ "apcr2" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "40_eng_slope_multipliers")/parameters/("ap_40_eng_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_eng_slope_multipliers")/parameters/("apcbc_40_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_eng_slope_multipliers")/parameters/("apcr_50_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_eng_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_slope_multipliers")/parameters` | block | [str]x12 | 1 | 12 | "ap" ¦ "apbc" ¦ "aphe" ¦ "apc" ¦ "apche" ¦ "apcbc" ¦ "apcbche" ¦ "apcr" ¦ "heat" ¦ "heata" ¦ "heatb" ¦ "heatc" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "40_slope_multipliers")/parameters/("ap_40_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_slope_multipliers")/parameters/("apc_37_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_slope_multipliers")/parameters/("apcr_50_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "40_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "45_rus_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "45_rus_slope_multipliers")/parameters` | block | [str]x5 | 1 | 5 | "aphe" ¦ "apbc" ¦ "apcbc" ¦ "apcr" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "45_rus_slope_multipliers")/parameters/("ap_40_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "45_rus_slope_multipliers")/parameters/("apc_57_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "45_rus_slope_multipliers")/parameters/("apcr_50_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "45_rus_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")/parameters` | block | [str]x7 | 1 | 7 | "ap" ¦ "apc" ¦ "apcbc" ¦ "apcbche" ¦ "apcr" ¦ "apds" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "57_eng_slope_multipliers")/parameters/("ap_57_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")/parameters/("apc_57_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")/parameters/("apc_57_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")/parameters/("apcbc_57_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")/parameters/("apcr_50_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_eng_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_rus_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_rus_slope_multipliers")/parameters` | block | [str]x5 | 1 | 5 | "aphe" ¦ "apbc" ¦ "aphebc" ¦ "apcr" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "57_rus_slope_multipliers")/parameters/("ap_57_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_rus_slope_multipliers")/parameters/("apbc_57_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_rus_slope_multipliers")/parameters/("apcr_50_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_rus_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_slope_multipliers")/parameters` | block | [str]x13 | 1 | 13 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "57_slope_multipliers")/parameters/("ap_57_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_slope_multipliers")/parameters/("apc_57_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_slope_multipliers")/parameters/("apcr_50_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "57_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_ger_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_ger_slope_multipliers")/parameters` | block | [str]x7 | 1 | 7 | "apche" ¦ "apcbche" ¦ "apcr" ¦ "heat" ¦ "heata" ¦ "heatb" ¦ "heatc" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "75_ger_slope_multipliers")/parameters/("apc_75_ger_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_ger_slope_multipliers")/parameters/("apcr_75_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_ger_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_slope_multipliers")/parameters` | block | [str]x14 | 1 | 14 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "75_slope_multipliers")/parameters/("ap_75_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_slope_multipliers")/parameters/("apc_75_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_slope_multipliers")/parameters/("apcr_75_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "75_slope_multipliers")/parameters/("hvap_76_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_eng_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_eng_slope_multipliers")/parameters` | block | [str]x5 | 1 | 5 | "ap" ¦ "apc" ¦ "apcbc" ¦ "apds" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "76_eng_slope_multipliers")/parameters/("ap_76_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_eng_slope_multipliers")/parameters/("apc_76_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_eng_slope_multipliers")/parameters/("apcbc_76_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_eng_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_eng_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_rus_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_rus_slope_multipliers")/parameters` | block | [str]x10 | 1 | 10 | "aphe" ¦ "apbc" ¦ "apcbc" ¦ "aphebc" ¦ "aphebc2" ¦ "apcr" ¦ "heat" ¦ "heata" ¦ "heatb" ¦ "heatc" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "76_rus_slope_multipliers")/parameters/("ap_75_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_rus_slope_multipliers")/parameters/("apbc_76_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_rus_slope_multipliers")/parameters/("apc_75_ger_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_rus_slope_multipliers")/parameters/("apcr_75_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_rus_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_slope_multipliers")/parameters` | block | [str]x14 | 1 | 14 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "76_slope_multipliers")/parameters/("ap_76_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_slope_multipliers")/parameters/("apc_76_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_slope_multipliers")/parameters/("apcr_75_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "76_slope_multipliers")/parameters/("hvap_76_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85_rus_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85_rus_slope_multipliers")/parameters` | block | [str]x4 | 1 | 4 | "aphe" ¦ "aphebc" ¦ "apcr" ¦ "heat" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "85_rus_slope_multipliers")/parameters/("ap_85_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85_rus_slope_multipliers")/parameters/("apbc_85_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85_rus_slope_multipliers")/parameters/("apcr_88_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85_rus_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85-155_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85-155_slope_multipliers")/parameters` | block | [str]x17 | 1 | 17 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "85-155_slope_multipliers")/parameters/("ap_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "85-155_slope_multipliers")/parameters/("apbc_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "85-155_slope_multipliers")/parameters/("apc_slope_multiplier")` | macro | []x5 | 1 | 5 | 예: | 미확인(자동) |  |  |  |
| `/(define "85-155_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "85-155_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "85-155_slope_multipliers")/parameters/("large_apcr_slope_multiplier")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")/parameters` | block | [str]x15 | 1 | 15 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "88_ger_slope_multipliers")/parameters/("ap_90_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")/parameters/("apc_88_ger_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")/parameters/("apc_90_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")/parameters/("apcr_88_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "88_ger_slope_multipliers")/parameters/("hvap_90_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "90_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "90_slope_multipliers")/parameters` | block | [str]x15 | 1 | 15 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "90_slope_multipliers")/parameters/("ap_90_slope_multiplier")` | macro | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "90_slope_multipliers")/parameters/("apc_90_slope_multiplier")` | macro | []x5 | 1 | 5 | 예: | 미확인(자동) |  |  |  |
| `/(define "90_slope_multipliers")/parameters/("apcr_88_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "90_slope_multipliers")/parameters/("apds_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "90_slope_multipliers")/parameters/("heat_slope_multiplier")` | macro | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/(define "90_slope_multipliers")/parameters/("hvap_90_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "94_eng_slope_multipliers")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "94_eng_slope_multipliers")/parameters` | block | [str]x2 | 1 | 2 | "ap" ¦ "aphe" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "94_eng_slope_multipliers")/parameters/("ap_90_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "94_eng_slope_multipliers")/parameters/("ap_94_eng_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "accuracy_direct_arrowhead")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "accuracy_direct_arrowhead")/("curve_direct")` | macro | [call:r1 call:range_ap call:range_he call:range_heat call:falloff call:r0 call:apbc50 call:apbc100 call:apbc140 call:apbc200 call:apc50 call:apc100 call:apc140 call:apc200 call:ap50 call:ap100 call:ap140 call:ap200 call:he50 call:he100 call:he140 call:he200 call:heat50 call:heat100 call:heat140 call:heat200 call:apcr50 call:apcr100 call:apcr140 call:apcr200 call:hvap50 call:hvap100 call:hvap140 call:hvap200 call:apds50 call:apds100 call:apds140 call:apds200]x1 | 1 | 1 | r1 ( %r1 ) range_ap ( %range_ap ) range_he ( %range_he ) range_heat ( %range_heat ) falloff ( 0.15 ) r0 ( 0.50 ) apbc50 ( 1.50 ) apbc100 ( 2.50 ) apbc140 ( 3.50 ) apbc200 ( 6.00 ) apc50 ( 1.50 ) apc100 ( 2.75 ) apc140 ( 3.75 ) apc200 ( 6.75 ) ap50 ( 1.50 ) ap100 ( 2.75 ) ap140 ( 3.75 ) ap200 ( 6.50 ) he50 ( 1.50 ) he100 ( 2.75 ) he140 ( 6.00 ) he200 ( 12.00 ) heat50 ( 1.50 ) heat100 ( 3.00 ) heat140 ( 4.25 ) heat200 ( 8.00 ) apcr50 ( 1.50 ) apcr100 ( 2.50 ) apcr140 ( 3.75 ) apcr200 ( 6.75 ) hvap50 ( 1.50 ) hvap100 ( 2.00 ) hvap140 ( 3.00 ) hvap200 ( 5.00 ) apds50 ( 2.00 ) apds100 ( 4.00 ) apds140 ( 7.00 ) apds200 ( 10.00 ) | 미확인(자동) |  |  |  |
| `/(define "accuracy_direct_auto")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "accuracy_direct_auto")/("curve_direct")` | macro | [call:r1 call:range_ap call:range_he call:range_heat call:falloff call:r0 call:apbc50 call:apbc100 call:apbc140 call:apbc200 call:apc50 call:apc100 call:apc140 call:apc200 call:ap50 call:ap100 call:ap140 call:ap200 call:he50 call:he100 call:he140 call:he200 call:heat50 call:heat100 call:heat140 call:heat200 call:apcr50 call:apcr100 call:apcr140 call:apcr200 call:hvap50 call:hvap100 call:hvap140 call:hvap200 call:apds50 call:apds100 call:apds140 call:apds200]x1 | 1 | 1 | r1 ( %r1 ) range_ap ( %range_ap ) range_he ( %range_he ) range_heat ( %range_heat ) falloff ( 0.15 ) r0 ( 0.60 ) apbc50 ( 1.50 ) apbc100 ( 2.50 ) apbc140 ( 3.50 ) apbc200 ( 6.00 ) apc50 ( 1.50 ) apc100 ( 2.75 ) apc140 ( 3.75 ) apc200 ( 6.75 ) ap50 ( 1.50 ) ap100 ( 2.75 ) ap140 ( 3.75 ) ap200 ( 6.50 ) he50 ( 1.50 ) he100 ( 2.75 ) he140 ( 3.75 ) he200 ( 6.50 ) heat50 ( 1.50 ) heat100 ( 3.00 ) heat140 ( 4.25 ) heat200 ( 8.00 ) apcr50 ( 1.50 ) apcr100 ( 2.50 ) apcr140 ( 3.75 ) apcr200 ( 6.75 ) hvap50 ( 1.50 ) hvap100 ( 2.00 ) hvap140 ( 3.00 ) hvap200 ( 5.00 ) apds50 ( 2.00 ) apds100 ( 4.00 ) apds140 ( 7.00 ) apds200 ( 10.00 ) | 미확인(자동) |  |  |  |
| `/(define "accuracy_direct")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "accuracy_direct")/("curve_direct")` | macro | [call:r1 call:range_ap call:range_he call:range_heat call:falloff call:r0 call:apbc50 call:apbc100 call:apbc140 call:apbc200 call:apc50 call:apc100 call:apc140 call:apc200 call:ap50 call:ap100 call:ap140 call:ap200 call:he50 call:he100 call:he140 call:he200 call:heat50 call:heat100 call:heat140 call:heat200 call:apcr50 call:apcr100 call:apcr140 call:apcr200 call:hvap50 call:hvap100 call:hvap140 call:hvap200 call:apds50 call:apds100 call:apds140 call:apds200]x1 | 1 | 1 | r1 ( %r1 ) range_ap ( %range_ap ) range_he ( %range_he ) range_heat ( %range_heat ) falloff ( 0.15 ) r0 ( 0.50 ) apbc50 ( 1.50 ) apbc100 ( 2.50 ) apbc140 ( 3.50 ) apbc200 ( 6.00 ) apc50 ( 1.50 ) apc100 ( 2.75 ) apc140 ( 3.75 ) apc200 ( 6.75 ) ap50 ( 1.50 ) ap100 ( 2.75 ) ap140 ( 3.75 ) ap200 ( 6.50 ) he50 ( 1.50 ) he100 ( 2.75 ) he140 ( 6.00 ) he200 ( 12.00 ) heat50 ( 1.50 ) heat100 ( 3.00 ) heat140 ( 4.25 ) heat200 ( 8.00 ) apcr50 ( 1.50 ) apcr100 ( 2.25 ) apcr140 ( 3.25 ) apcr200 ( 5.50 ) hvap50 ( 1.50 ) hvap100 ( 2.00 ) hvap140 ( 3.00 ) hvap200 ( 5.00 ) apds50 ( 1.80 ) apds100 ( 3.50 ) apds140 ( 6.00 ) apds200 ( 8.25 ) | 미확인(자동) |  |  |  |
| `/(define "accuracy_indirect")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "accuracy_indirect")/("curve_indirect")` | macro | [call:r1 call:r2 call:rmax call:range_ap call:range_he call:range_heat call:falloff call:r0 call:r01 call:ap50 call:ap100 call:ap140 call:ap200 call:heat50 call:heat100 call:heat140 call:heat200 call:he125]x1 | 1 | 1 | r1 ( %r1 ) r2 ( %r2 ) rmax ( %rmax ) range_ap ( %range_ap ) range_he ( %range_he ) range_heat ( %range_heat ) falloff ( 0.05 ) r0 ( 1.450 ) r01 ( 0.5 ) ap50 ( 1.50 ) ap100 ( 2.75 ) ap140 ( 3.75 ) ap200 ( 6.50 ) heat50 ( 1.50 ) heat100 ( 3.00 ) heat140 ( 4.25 ) heat200 ( 8.00 ) he125 ( 0.4 ) | 미확인(자동) |  |  |  |
| `/(define "accuracy_spg")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "accuracy_spg")/("curve_direct")` | macro | [call:r1 call:range_ap call:range_he call:range_heat call:falloff call:r0 call:apbc50 call:apbc100 call:apbc140 call:apbc200 call:apc50 call:apc100 call:apc140 call:apc200 call:ap50 call:ap100 call:ap140 call:ap200 call:he50 call:he100 call:he140 call:he200 call:heat50 call:heat100 call:heat140 call:heat200 call:apcr50 call:apcr100 call:apcr140 call:apcr200 call:hvap50 call:hvap100 call:hvap140 call:hvap200 call:apds50 call:apds100 call:apds140 call:apds200]x1 | 1 | 1 | r1 ( %r1 ) range_ap ( %range_ap ) range_he ( %range_he ) range_heat ( %range_heat ) falloff ( 0.15 ) r0 ( 0.50 ) apbc50 ( 1.50 ) apbc100 ( 2.50 ) apbc140 ( 3.50 ) apbc200 ( 6.00 ) apc50 ( 1.50 ) apc100 ( 2.75 ) apc140 ( 3.75 ) apc200 ( 6.75 ) ap50 ( 1.50 ) ap100 ( 2.75 ) ap140 ( 3.75 ) ap200 ( 6.50 ) he50 ( 1.50 ) he100 ( 2.75 ) he140 ( 4.00 ) he200 ( 7.50 ) heat50 ( 1.50 ) heat100 ( 3.00 ) heat140 ( 4.25 ) heat200 ( 8.00 ) apcr50 ( 1.50 ) apcr100 ( 2.25 ) apcr140 ( 3.25 ) apcr200 ( 5.50 ) hvap50 ( 1.50 ) hvap100 ( 2.00 ) hvap140 ( 3.00 ) hvap200 ( 5.00 ) apds50 ( 2.00 ) apds100 ( 4.00 ) apds140 ( 7.00 ) apds200 ( 10.00 ) | 미확인(자동) |  |  |  |
| `/(define "aerial gravity")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "aerial gravity")/gravity` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | same as other direct-fire "cannon" type weapons |
| `/(define "airborne_mg")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "airborne_mg")/("accuracy_mg_plane")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "airborne_mg")/(mod)` | mod | [atom str]x1 | 1 | 1 | not "mp" | 미확인(자동) |  |  |  |
| `/(define "airborne_mg")/aimingtolerance` | block | [num]x1 | 1 | 1 | 20 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "airborne_mg")/range` | block | [num num]x2 | 1 | 2 | 140 140 ¦ 300 500 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "airborne_mg")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "airborne_mg")/spreading/burstaccuracy` | block | [num num num num num]x1 | 1 | 1 | 100 95 90 85 80 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "airborne_mg")/spreading/burstrecoverytime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 사격 뒤 명중률이 평소로 돌아오는 데 걸리는 시간, 초 | 게임 파일 주석 | time in seconds for weapon accuracy to return to normal after firing. |
| `/(define "airborne_mg")/spreading/spreadpower` | block | [num]x1 | 1 | 1 | 1.5 | 미확인(자동) |  |  | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/(define "airborne_mg")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "ap_100_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_100_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_100_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_105_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_105_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_105_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_122_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_122_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_122_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_152_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_152_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_152_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_155_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_155_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_155_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x12 | 1 | 12 | 0 1.0 0.00 ¦ 10 1.0225 0.01 ¦ 20 1.0915 0.042 ¦ 30 1.1885 0.15 ¦ 45 1.446 -0.38 ¦ 60 1.7785 -0.295 ¦ 65 2.0775 -0.17 ¦ 70 3.039 0.145 ¦ 75 6.61 0.743 ¦ 80 13.4 0.743 ¦ 85 44.85 0.743 ¦ 90 99.0000 0.00000 | 미확인(자동) |  |  |  |
| `/(define "ap_20_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_20_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_20_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x12 | 1 | 12 | 0 1.0 0.00 ¦ 20 1.381 0.15 ¦ 30 1.657 0.29 ¦ 40 1.95 0.295 ¦ 50 2.92 0.39 ¦ 60 3.965 0.325 ¦ 65 5.47 0.46 ¦ 70 7.58 0.46 ¦ 75 11.52 0.46 ¦ 80 20.85 0.46 ¦ 85 57.4 0.46 ¦ 90 99.0000 0.00000 | 미확인(자동) |  |  |  |
| `/(define "ap_37_slope_multiplier_700ms")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_37_slope_multiplier_700ms")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_37_slope_multiplier_700ms")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_37_slope_multiplier_800ms")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_37_slope_multiplier_800ms")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_37_slope_multiplier_800ms")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_40_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_40_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_40_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_40_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_40_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_40_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x12 | 1 | 12 | 0 1.0 0.00 ¦ 20 1.05 0.00 ¦ 30 1.167 0.01 ¦ 40 1.683 0.33 ¦ 50 1.958 0.30 ¦ 60 3.77 0.79 ¦ 65 5.22 0.79 ¦ 70 7.79 0.79 ¦ 75 13.03 0.79 ¦ 80 26.92 0.79 ¦ 85 93.05 0.79 ¦ 90 99.0000 0.00000 | 미확인(자동) |  |  |  |
| `/(define "ap_57_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_57_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_57_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_57_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_57_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_57_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_75_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_75_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_75_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_76_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_76_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_76_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_76_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_76_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_76_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_85_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_85_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_85_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_90_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_90_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_90_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_94_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_94_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_94_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ap_blast")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_blast")/damage` | block | [atom]x1 | 1 | 1 | blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/(define "ap_blast")/damage/area` | block | [expr expr]x1 | 1 | 1 | ( * %0 1.5 ) ( * %0 3 ) | 미확인(자동) |  |  |  |
| `/(define "ap_blast")/damage/energy` | block | [num]x1 | 1 | 1 | 1.2 | 미확인(자동) |  |  | = 25mm of penetration ¦ = 140mm of penetration |
| `/(define "ap_blast")/damage/ground_interaction_radius` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/(define "ap_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ap_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x19 | 1 | 19 | 예: 0 0.1 0.0637 | 미확인(자동) |  |  |  |
| `/(define "apbc_100_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_100_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_100_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apbc_122_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_122_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_122_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apbc_152_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_152_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_152_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apbc_45_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_45_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_45_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x12 | 1 | 12 | 0 1.0 0.00 ¦ 30 1.129 0.075 ¦ 45 1.38 0.175 ¦ 50 1.555 0.21 ¦ 55 1.807 0.302 ¦ 60 2.13 0.351 ¦ 65 2.726 0.352 ¦ 70 3.687 0.352 ¦ 75 5.44 0.352 ¦ 80 9.41 0.352 ¦ 85 24.02 0.352 ¦ 90 99.0000 0.0000 | 미확인(자동) |  |  |  |
| `/(define "apbc_57_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_57_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_57_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x12 | 1 | 12 | 0 1.0 0.00 ¦ 30 1.147 0.055 ¦ 45 1.387 0.144 ¦ 50 1.546 0.175 ¦ 55 1.764 0.225 ¦ 60 2.08 0.302 ¦ 65 2.634 0.299 ¦ 70 3.52 0.299 ¦ 75 5.115 0.299 ¦ 80 8.68 0.301 ¦ 85 21.38 0.301 ¦ 90 99.0000 0.0000 | 미확인(자동) |  |  |  |
| `/(define "apbc_76_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_76_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_76_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x12 | 1 | 12 | 0 1.0 0.00 ¦ 30 1.143 -0.01 ¦ 45 1.405 0.059 ¦ 50 1.564 0.075 ¦ 55 1.81 0.124 ¦ 60 2.218 0.224 ¦ 65 2.79 0.23 ¦ 70 3.67 0.23 ¦ 75 5.225 0.23 ¦ 80 8.605 0.23 ¦ 85 20.2 0.23 ¦ 90 99.0000 0.0000 | 미확인(자동) |  |  |  |
| `/(define "apbc_85_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_85_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_85_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apbc_90_t33_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_90_t33_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_90_t33_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x11 | 1 | 11 | 0 1.0 0.00 ¦ 20 1.0055 0.145 ¦ 30 1.2935 0.085 ¦ 55 1.78 -0.528 ¦ 60 1.918 -0.485 ¦ 65 2.113 -0.385 ¦ 70 2.382 -0.255 ¦ 75 3.058 -0.105 ¦ 80 4.465 -0.045 ¦ 85 10.265 -0.005 ¦ 90 99.0000 0.00000 | 미확인(자동) |  |  |  |
| `/(define "apbc_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apbc_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x19 | 1 | 19 | 예: 0 1.0 0.15 | 미확인(자동) |  |  |  |
| `/(define "apc_128_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_128_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_128_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_37_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_37_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_37_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_57_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_57_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_57_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_57_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_57_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_57_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x12 | 1 | 12 | 0 1.0 0.00 ¦ 20 1.045 0.039 ¦ 30 1.23 0.07 ¦ 45 1.928 0.063 ¦ 50 2.156 0.066 ¦ 60 3.276 0.357 ¦ 65 4.207 0.477 ¦ 70 6.337 0.675 ¦ 75 10.935 0.913 ¦ 80 23.727 0.913 ¦ 85 89.307 0.913 ¦ 90 99.0000 0.00000 | 미확인(자동) |  |  |  |
| `/(define "apc_75_ger_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_75_ger_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_75_ger_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x15 | 1 | 15 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_75_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_75_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_75_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_76_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_76_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_76_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x15 | 1 | 15 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_76_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_76_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_76_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_88_ger_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_88_ger_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_88_ger_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x15 | 1 | 15 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_90_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_90_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_90_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x15 | 1 | 15 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apc_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apc_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x19 | 1 | 19 | 예: 0 1.0 0.0225 | 미확인(자동) |  |  |  |
| `/(define "apcbc_40_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcbc_40_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcbc_40_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apcbc_57_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcbc_57_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcbc_57_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x14 | 1 | 14 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apcbc_76_eng_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcbc_76_eng_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcbc_76_eng_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x15 | 1 | 15 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apcr_50_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_50_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_50_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apcr_75_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_75_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_75_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apcr_88_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_88_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_88_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "apcr_blast")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_blast")/damage` | block | [atom]x1 | 1 | 1 | blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/(define "apcr_blast")/damage/area` | block | [expr expr]x1 | 1 | 1 | ( * %0 1 ) ( * %0 2 ) | 미확인(자동) |  |  |  |
| `/(define "apcr_blast")/damage/energy` | block | [num]x1 | 1 | 1 | 1.2 | 미확인(자동) |  |  | = 25mm of penetration ¦ = 140mm of penetration |
| `/(define "apcr_blast")/damage/ground_interaction_radius` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/(define "apcr_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_slope_multiplier")/overmatchfixed` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apcr_slope_multiplier")/overmatchfixed/#row` | block | [num num]x19 | 1 | 19 | 예: 0 1.00 | 미확인(자동) |  |  |  |
| `/(define "apds_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apds_slope_multiplier")/overmatchfixed` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "apds_slope_multiplier")/overmatchfixed/#row` | block | [num num]x19 | 1 | 19 | 예: 0 1.00 | 미확인(자동) |  |  |  |
| `/(define "armor health custom")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "armor health custom")/parameters` | block | [str]x1 | 1 | 1 | "%shell" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "armor health custom")/parameters/minimumdamagemodifier` | block | [expr]x1 | 1 | 1 | ( + ( * %damage ( / %velocity 2100 ) ) ( * 0.72 ( - ( + 29 ( + %damage ( + ( * 0.35 %filler ) ( * 0.0000000075 ( * %filler ( * %filler %filler ) ) ) ) ) ) ( * 0.00009 ( * %filler %filler ) ) ) ) ) | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/(define "armor health")` | define | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/(define "armor health")/minimumdamagemodifier` | block | [expr]x10 | 10 | 10 | ( * 1.0 %damage ) | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/(define "armor health")/parameters` | block | [str]x17 | 1 | 17 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "armor health")/parameters/minimumdamagemodifier` | block | [expr]x17 | 1 | 17 | ( * 1.25 %damage ) ¦ ( * 1.00 %damage ) | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/(define "artillery_firing_arc")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "artillery_firing_arc")/gravity` | block | [num]x1 | 1 | 1 | 6 | 미확인(자동) |  |  | same as other direct-fire "cannon" type weapons |
| `/(define "assault_rifle")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/("accuracy_assault_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/("ap_7_62_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 65 ) | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/("recoil_stg")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/accuracybymotionk` | block | [num]x1 | 1 | 1 | 0.9 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/(define "assault_rifle")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/burst/long` | block | [num num]x1 | 1 | 1 | 3.5 0.5 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "assault_rifle")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "assault_rifle")/burst/short` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "assault_rifle")/curve` | block | [str]x1 | 1 | 1 | "assaultrifle" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/(define "assault_rifle")/firingtimeout` | block | [num num]x1 | 1 | 1 | 1.5 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "assault_rifle")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.7 ¦ 0.499 0.95 ¦ 0.5 0.5 ¦ 1.0 0.7 | 미확인(자동) |  |  |  |
| `/(define "assault_rifle")/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/(define "assault_rifle")/range` | block | [num num]x1 | 1 | 1 | 100 100 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "assault_rifle")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "assault_rifle")/spreading/burstaccuracy` | block | [num num num num num num num num num]x1 | 1 | 1 | 100 95 90 85 80 75 70 60 50 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "assault_rifle")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "assault_rifle")/tag` | block | [str]x1 | 1 | 1 | "autorifle_sup auto_eject" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "auto_rifle")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/("accuracy_rifle_auto")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/("ap_7_62_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 70 ) | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/("recoil_rifle_semi")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/accuracybymotionk` | block | [num]x1 | 1 | 1 | 1.2 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/(define "auto_rifle")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/burst/long` | block | [num num]x1 | 1 | 1 | 2.25 0.5 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "auto_rifle")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "auto_rifle")/burst/short` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "auto_rifle")/curve` | block | [str]x1 | 1 | 1 | "autorifle" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/(define "auto_rifle")/firingtimeout` | block | [num num]x1 | 1 | 1 | 2.0 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "auto_rifle")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.65 ¦ 0.399 0.85 ¦ 0.4 0.75 ¦ 0.8 1.0 | 미확인(자동) |  |  |  |
| `/(define "auto_rifle")/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/(define "auto_rifle")/range` | block | [num num]x1 | 1 | 1 | 120 120 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "auto_rifle")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "auto_rifle")/spreading/burstaccuracy` | block | [num num num num]x1 | 1 | 1 | 100 80 60 40 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "auto_rifle")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "auto_rifle")/tag` | block | [str]x1 | 1 | 1 | "autorifle_sup auto_eject rifle_shell" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "battle_rifle_fg42")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/("accuracy_rifle_auto")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/("ap_7_62_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 70 ) | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/("recoil_fg42")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/accuracybymotionk` | block | [num]x1 | 1 | 1 | 1.2 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/(define "battle_rifle_fg42")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/burst/long` | block | [num num]x1 | 1 | 1 | 3 1 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "battle_rifle_fg42")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.65 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "battle_rifle_fg42")/burst/short` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "battle_rifle_fg42")/curve` | block | [str]x1 | 1 | 1 | "autorifle" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/(define "battle_rifle_fg42")/firingtimeout` | block | [num num]x1 | 1 | 1 | 1.75 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "battle_rifle_fg42")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.65 ¦ 0.649 1.0 ¦ 0.65 0.6 ¦ 0.8 0.7 | 미확인(자동) |  |  |  |
| `/(define "battle_rifle_fg42")/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/(define "battle_rifle_fg42")/range` | block | [num num]x1 | 1 | 1 | 120 120 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "battle_rifle_fg42")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "battle_rifle_fg42")/spreading/burstaccuracy` | block | [num num num num num num]x1 | 1 | 1 | 100 90 80 70 60 50 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "battle_rifle_fg42")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "battle_rifle_fg42")/tag` | block | [str]x1 | 1 | 1 | "autorifle_sup auto_eject rifle_shell" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "battle_rifle")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/("accuracy_battle_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/("ap_7_62_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 70 ) | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/("recoil_rifle_semi")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/accuracybymotionk` | block | [num]x1 | 1 | 1 | 1.2 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/(define "battle_rifle")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/burst/long` | block | [num num]x1 | 1 | 1 | 3 1 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "battle_rifle")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.33 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "battle_rifle")/burst/short` | block | [num num]x1 | 1 | 1 | 3 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "battle_rifle")/curve` | block | [str]x1 | 1 | 1 | "autorifle" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/(define "battle_rifle")/firingtimeout` | block | [num num]x1 | 1 | 1 | 2.0 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "battle_rifle")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/firingtimeoutdistancefactor/#row` | block | [num num]x2 | 1 | 2 | 0.2 0.65 ¦ 0.6 1.0 | 미확인(자동) |  |  |  |
| `/(define "battle_rifle")/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/(define "battle_rifle")/range` | block | [num num]x1 | 1 | 1 | 120 120 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "battle_rifle")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "battle_rifle")/spreading/burstaccuracy` | block | [num num num num num]x1 | 1 | 1 | 100 90 80 60 40 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "battle_rifle")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "battle_rifle")/tag` | block | [str]x1 | 1 | 1 | "autorifle_sup auto_eject rifle_shell" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "bolt_action")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/("ap_7_62_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 40 ) | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 70 ) | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/("recoil_rifle_bolt")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/("zeroing_boost")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/accuracybymotionk` | block | [num]x1 | 1 | 1 | 1.2 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/(define "bolt_action")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/burst/long` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "bolt_action")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "bolt_action")/burst/short` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "bolt_action")/range` | block | [num num]x1 | 1 | 1 | 120 120 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "bolt_action")/states` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/states/aim` | block | [str]x1 | 1 | 1 | "aim" | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/states/can_aim` | block | [str]x1 | 1 | 1 | "can-aim-bolt" | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/states/fire` | block | [str]x1 | 1 | 1 | "fire" | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/states/reload` | block | [str]x1 | 1 | 1 | "reload" | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/states/unload` | block | [str]x1 | 1 | 1 | "unload" | 미확인(자동) |  |  |  |
| `/(define "bolt_action")/tag` | block | [str]x1 | 1 | 1 | "bolt_action rifle_shell" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "burst_aa_quad")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_aa_quad")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_aa_quad")/burst/long` | block | [num num]x1 | 1 | 1 | 150 3 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "burst_aa_quad")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "burst_aa_quad")/burst/short` | block | [num num]x1 | 1 | 1 | 30 0 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "burst_aa")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_aa")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_aa")/burst/long` | block | [num num]x1 | 1 | 1 | 25 3 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "burst_aa")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "burst_aa")/burst/short` | block | [num num]x1 | 1 | 1 | 12 0 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "burst_long")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_long")/burst` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_long")/burst/long` | block | [num num]x2 | 2 | 2 | 7 2 ¦ 6 2 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "burst_long")/burst/rangekoef` | block | [num]x2 | 2 | 2 | 0.4 ~ 0.5 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "burst_long")/burst/short` | block | [num num]x2 | 2 | 2 | 6 1 ¦ 4 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "burst_long")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_long")/firingtimeoutdistancefactor/#row` | block | [num num]x3 | 1 | 3 | 0.25 0.55 ¦ 0.7 0.85 ¦ 1.0 0.95 | 미확인(자동) |  |  |  |
| `/(define "burst_medium")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_medium")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_medium")/burst/long` | block | [num num]x1 | 1 | 1 | 6 2 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "burst_medium")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "burst_medium")/burst/short` | block | [num num]x1 | 1 | 1 | 4 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "burst_short_slow")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_short_slow")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_short_slow")/burst/long` | block | [num num]x1 | 1 | 1 | 6 2 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "burst_short_slow")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "burst_short_slow")/burst/short` | block | [num num]x1 | 1 | 1 | 4 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "burst_short_slow")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_short_slow")/firingtimeoutdistancefactor/#row` | block | [num num]x3 | 1 | 3 | 0.25 0.55 ¦ 0.7 0.85 ¦ 1.0 0.95 | 미확인(자동) |  |  |  |
| `/(define "burst_short")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_short")/burst` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_short")/burst/long` | block | [num num]x2 | 2 | 2 | 5 1 ¦ 6 2 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "burst_short")/burst/rangekoef` | block | [num]x2 | 2 | 2 | 0.4 ~ 0.5 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "burst_short")/burst/short` | block | [num num]x2 | 2 | 2 | 3 1 ¦ 4 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "burst_short")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_short")/firingtimeoutdistancefactor/#row` | block | [num num]x3 | 1 | 3 | 0.25 0.5 ¦ 0.7 0.75 ¦ 1.0 0.85 | 미확인(자동) |  |  |  |
| `/(define "burst_stand")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_stand")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "burst_stand")/burst/long` | block | [num num]x1 | 1 | 1 | 20 3 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "burst_stand")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.75 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "burst_stand")/burst/short` | block | [num num]x1 | 1 | 1 | 10 3 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "carbine_rifle")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/("accuracy_assault_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/("ap_7_62_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 65 ) | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/("recoil_stg")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/burst/long` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "carbine_rifle")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "carbine_rifle")/burst/short` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "carbine_rifle")/curve` | block | [str]x1 | 1 | 1 | "assaultrifle" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/(define "carbine_rifle")/firingtimeout` | block | [num num]x1 | 1 | 1 | 1.5 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "carbine_rifle")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/firingtimeoutdistancefactor/#row` | block | [num num]x3 | 1 | 3 | 0.2 0.6 ¦ 0.4 0.7 ¦ 1.0 1.0 | 미확인(자동) |  |  |  |
| `/(define "carbine_rifle")/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/(define "carbine_rifle")/range` | block | [num num]x1 | 1 | 1 | 100 100 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "carbine_rifle")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "carbine_rifle")/spreading/burstaccuracy` | block | [num num num num num num num num num]x1 | 1 | 1 | 100 95 90 85 80 75 70 60 50 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "carbine_rifle")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "carbine_rifle")/tag` | block | [str]x1 | 1 | 1 | "autorifle_sup auto_eject autorifle" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "curve_direct")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "curve_direct")/parameters` | block | [str]x25 | 1 | 25 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "curve_direct")/parameters/falloffmeter` | block | [param]x9 | 1 | 9 | %range_he ¦ %range_heat | 미확인(자동) |  |  |  |
| `/(define "curve_direct")/parameters/falloffstrength` | block | [param]x9 | 1 | 9 | %falloff | 미확인(자동) |  |  |  |
| `/(define "curve_direct")/parameters/spreading` | block | []x25 | 1 | 25 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "curve_direct")/parameters/spreading/radiustable` | block | []x25 | 1 | 25 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "curve_direct")/parameters/spreading/radiustable/{}` | block | [expr expr]x25 [expr num]x16 | 1 | 41 | ( + %range_ap 50 ) 10 ¦ ( - %range_ap 0.01 ) ( * ( + ( * ( / ( - %range_ap 140 ) ( - 200 140 ) ) ( - %apbc200 %apbc140 ) ) %apbc140 ) ( * %r1 %r0 ) ) ¦ ( - %range_he 0.01 ) ( * ( + ( * ( / ( - %range_he 140 ) ( - 200 140 ) ) ( - %he200 %he140 ) ) %he140 ) ( * %r1 %r0 ) ) ¦ ( - %range_heat 0.01 ) ( * ( + ( * ( / ( - %range_heat 140 ) ( - 200 140 ) ) ( - %heat200 %heat140 ) ) %heat140 ) ( * %r1 %r0 ) ) ¦ ( - %range_ap 0.01 ) ( * ( + ( * ( / ( - %range_ap 140 ) ( - 200 140 ) ) ( - %ap200 %ap140 ) ) %ap140 ) ( * %r1 %r0 ) ) ¦ ( - %range_ap 0.01 ) ( * ( + ( * ( / ( - %range_ap 140 ) ( - 200 140 ) ) ( - %apc200 %apc140 ) ) %apc140 ) ( * %r1 %r0 ) ) ¦ ( - %range_ap 0.01 ) ( * ( + ( * ( / ( - %range_ap 140 ) ( - 200 140 ) ) ( - %apcr200 %apcr140 ) ) %apcr140 ) ( * %r1 %r0 ) ) ¦ ( - %range_ap 0.01 ) ( * ( + ( * ( / ( - %range_ap 140 ) ( - 200 140 ) ) ( - %hvap200 %hvap140 ) ) %hvap140 ) ( * %r1 %r0 ) ) ¦ ( - %range_ap 0.01 ) ( * ( + ( * ( / ( - %range_ap 140 ) ( - 200 140 ) ) ( - %apds200 %apds140 ) ) %apds140 ) ( * %r1 %r0 ) ) | 미확인(자동) |  |  |  |
| `/(define "curve_direct")/parameters/spreading/radiustable/#row` | block | [num expr]x75 [num num]x25 | 1 | 100 | 예: 0 0 | 미확인(자동) |  |  |  |
| `/(define "curve_direct")/parameters/spreading/radiustable/%range_he` | block | [num]x5 | 1 | 5 | 20 | 미확인(자동) |  |  |  |
| `/(define "curve_direct")/parameters/spreading/radiustable/%range_heat` | block | [num]x4 | 1 | 4 | 20 | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters` | block | [str]x25 | 1 | 25 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "curve_indirect")/parameters/("standard_firing_arc")` | macro | []x20 | 1 | 20 | 예: | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters/falloffmeter` | block | [param]x4 | 1 | 4 | %range_heat | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters/falloffstrength` | block | [param]x4 | 1 | 4 | %falloff | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters/spreading` | block | []x25 | 1 | 25 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "curve_indirect")/parameters/spreading/radiustable` | block | []x25 | 1 | 25 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "curve_indirect")/parameters/spreading/radiustable/{}` | block | [expr num]x25 [expr expr]x3 | 1 | 28 | ( + %range_ap 10 ) 10 ¦ ( + %range_heat 10 ) 10 ¦ ( - %range_he 0.1 ) ( * %rmax %r0 ) ¦ ( + %range_he 10 ) 60 ¦ ( * %range_he 10 ) 60 | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters/spreading/radiustable/#row` | block | [num expr]x70 [num num]x25 | 1 | 95 | 0 0 ¦ 50 ( * %ap50 ( * %r1 %r01 ) ) ¦ 100 ( * %ap100 ( * %r1 %r01 ) ) ¦ 140 ( * %ap140 ( * %r1 %r01 ) ) ¦ 125 ( * %he125 ( * 0.5 ( * %r2 %r0 ) ) ) ¦ 250 ( * 1.85 ( * %r2 %r0 ) ) ¦ 50 ( * %heat50 ( * %r1 %r01 ) ) ¦ 100 ( * %heat100 ( * %r1 %r01 ) ) ¦ 140 ( * %heat140 ( * %r1 %r01 ) ) | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters/spreading/radiustable/%range_ap` | block | [expr]x16 | 1 | 16 | ( * ( + ( * ( / ( - %range_ap 140 ) ( - 200 140 ) ) ( - %ap200 %ap140 ) ) %ap140 ) ( * %r1 %r01 ) ) | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters/spreading/radiustable/%range_he` | block | [expr]x2 | 1 | 2 | ( * %rmax %r0 ) | 미확인(자동) |  |  |  |
| `/(define "curve_indirect")/parameters/spreading/radiustable/%range_heat` | block | [expr]x4 | 1 | 4 | ( * ( + ( * ( / ( - %range_heat 140 ) ( - 200 140 ) ) ( - %heat200 %heat140 ) ) %heat140 ) ( * %r1 %r01 ) ) | 미확인(자동) |  |  |  |
| `/(define "curved_firing_arc")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "curved_firing_arc")/gravity` | block | [num]x1 | 1 | 1 | 5.5 | 미확인(자동) |  |  | same as other direct-fire "cannon" type weapons |
| `/(define "default gravity")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "default gravity")/gravity` | block | [num]x1 | 1 | 1 | 2 | 미확인(자동) |  |  | same as other direct-fire "cannon" type weapons |
| `/(define "direct_settings")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "direct_settings")/parameters` | block | [str]x3 | 1 | 3 | "he" ¦ "he2" ¦ "apds" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "direct_settings")/parameters/spreading` | block | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "direct_settings")/parameters/spreading/spreadxyratio` | block | [num]x3 | 1 | 3 | 1 ~ 1.5 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "direct_settings")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "direct_settings")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "direct_settings")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.6 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "direct_settings")/zeroingaccuracy` | block | [num num num num num]x1 | 1 | 1 | 0.8 0.65 0.55 0.5 0.45 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "enfield_no1_rate")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "enfield_no1_rate")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 50 ) | 미확인(자동) |  |  |  |
| `/(define "enfield_no1_rate")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 10 ) | 미확인(자동) |  |  |  |
| `/(define "enfield_no1_rate")/firingtimeout` | block | [num num]x1 | 1 | 1 | 3.1 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "enfield_no1_rate")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "enfield_no1_rate")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.5 ¦ 0.4 0.8 ¦ 0.6 0.9 ¦ 0.8 1.0 | 미확인(자동) |  |  |  |
| `/(define "enfield_no4_rate")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "enfield_no4_rate")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 60 ) | 미확인(자동) |  |  |  |
| `/(define "enfield_no4_rate")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 10 ) | 미확인(자동) |  |  |  |
| `/(define "enfield_no4_rate")/firingtimeout` | block | [num num]x1 | 1 | 1 | 2.9 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "enfield_no4_rate")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "enfield_no4_rate")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.5 ¦ 0.4 0.8 ¦ 0.6 0.9 ¦ 0.8 1.0 | 미확인(자동) |  |  |  |
| `/(define "filling_tank")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "filling_tank")/filling` | block | [str param]x1 | 1 | 1 | "ammo flame_thrower" %fuel | 미확인(자동) |  |  | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/(define "filling")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "filling")/filling` | block | [str param]x1 | 1 | 1 | "ammo flame_thrower" %fuel | 미확인(자동) |  |  | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/(define "finnish_rate")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "finnish_rate")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 50 ) | 미확인(자동) |  |  |  |
| `/(define "finnish_rate")/firingtimeout` | block | [num num]x1 | 1 | 1 | 3.5 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "finnish_rate")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "finnish_rate")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.5 ¦ 0.4 0.8 ¦ 0.6 0.9 ¦ 0.8 1.0 | 미확인(자동) |  |  |  |
| `/(define "fire rate")` | define | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "fire rate")/recoverytime` | block | [expr]x3 | 3 | 3 | ( / 60 %rpm ) | 미확인(자동) |  |  | increase fuel consumption by 2x |
| `/(define "flare_pistol")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/("armor health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 5 ) | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 60 ) | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/("penetration")` | macro | [atom num num]x1 | 1 | 1 | args 0.5 0.5 | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 120 ) | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/aimingtolerance` | block | [num]x1 | 1 | 1 | 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "flare_pistol")/calibre` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | caliber of gun in millimeters ¦ AP shot caliber |
| `/(define "flare_pistol")/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/firesound` | block | [str]x1 | 1 | 1 | "pistol/flare" | 미확인(자동) |  |  | Placeholder |
| `/(define "flare_pistol")/firesoundclose` | block | [str]x1 | 1 | 1 | "pistol/flare/3p/" | 미확인(자동) |  |  | Placeholder |
| `/(define "flare_pistol")/gravity` | block | [num]x1 | 1 | 1 | 0.3 | 미확인(자동) |  |  | same as other direct-fire "cannon" type weapons |
| `/(define "flare_pistol")/mass` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | with water ¦ empty weight |
| `/(define "flare_pistol")/range` | block | [num num]x1 | 1 | 1 | 65 70 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "flare_pistol")/reloadanimbonus` | block | [num]x1 | 1 | 1 | 117 | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/reloadsound` | block | [str]x1 | 1 | 1 | "flare" | 미확인(자동) |  |  | sound for weapon reload |
| `/(define "flare_pistol")/speed` | block | [num]x1 | 1 | 1 | 15 | 미확인(자동) |  |  | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/(define "flare_pistol")/spreading` | block | [num]x1 | 1 | 1 | 10.5 | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "flare_pistol")/spreading/radiusfunction` | block | [num num num num num num num num]x1 | 1 | 1 | 0.0 0 1.0 1.0 1.2 2.5 2.5 5.0 | 미확인(자동) |  |  |  |
| `/(define "flare_pistol")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 2 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "flare_pistol")/tag` | block | [str]x1 | 1 | 1 | "flare_gun flaregun" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "ger_ap_bullets")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ger_ap_bullets")/("penetration")` | macro | [atom num num num num num]x1 | 1 | 1 | args 11.5 5.5 3 2 0 | 미확인(자동) |  |  |  |
| `/(define "ger_ap_bullets")/parameters` | block | [str]x2 | 1 | 2 | "ap" ¦ "ap2" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "ger_ap_bullets")/parameters/("armor health")` | macro | [call:damage]x2 | 1 | 2 | damage ( 5 ) | 미확인(자동) |  |  |  |
| `/(define "ger_ap_bullets")/parameters/("penetration")` | macro | [atom num num num num num]x2 | 1 | 2 | args 18 8.8 5.3 2 0 ¦ args 25.9 12.7 7.7 3 0 | 미확인(자동) |  |  |  |
| `/(define "ger_ap_bullets")/parameters/cursorreloading` | block | [str]x2 | 1 | 2 | "gun_ap" | 미확인(자동) |  |  | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/(define "global velocity")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "global velocity")/speed` | block | [expr]x1 | 1 | 1 | ( * 0.75 %gv ) | 미확인(자동) |  |  | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/(define "he_auto_blast")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "he_auto_blast")/damage` | block | [atom]x1 [atom atom]x1 | 1 | 2 | blastwave ¦ add blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/(define "he_auto_blast")/damage/area` | block | [num num]x1 [expr expr]x1 | 1 | 2 | 0.3 0.6 ¦ ( * 0.3 ( * %0 5 ) ) ( * 1.5 ( * %0 5 ) ) | 미확인(자동) |  |  |  |
| `/(define "he_auto_blast")/damage/energy` | block | [expr]x2 | 1 | 2 | ( * %0 0.9 ) ¦ ( * %0 0.65 ) | 미확인(자동) |  |  | = 25mm of penetration ¦ = 140mm of penetration |
| `/(define "he_auto_blast")/damage/ground_interaction_radius` | block | [num]x2 | 1 | 2 | 0 ~ 0.2 | 미확인(자동) |  |  | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/(define "he_heavy")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "he_heavy")/("minimum_range_heavy")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "he_heavy")/detonationthickness` | block | [num]x1 | 1 | 1 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "he_light")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "he_light")/("minimum_range_short")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "he_light")/detonationthickness` | block | [num]x1 | 1 | 1 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "he_medium")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "he_medium")/("minimum_range_medium")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "he_medium")/detonationthickness` | block | [num]x1 | 1 | 1 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "heat_blast")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heat_blast")/damage` | block | [atom]x1 [atom atom]x1 | 1 | 2 | blastwave ¦ add blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/(define "heat_blast")/damage/area` | block | [num num]x2 | 1 | 2 | 2 3 ¦ 1 6 | 미확인(자동) |  |  |  |
| `/(define "heat_blast")/damage/energy` | block | [num]x2 | 1 | 2 | 1 ~ 3 | 미확인(자동) |  |  | = 25mm of penetration ¦ = 140mm of penetration |
| `/(define "heat_blast")/damage/ground_interaction_radius` | block | [num]x2 | 1 | 2 | 0 ~ 0.2 | 미확인(자동) |  |  | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/(define "heat_large_blast")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heat_large_blast")/damage` | block | [atom atom]x2 [atom]x1 | 1 | 3 | add blastwave ¦ blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/(define "heat_large_blast")/damage/area` | block | [expr expr]x2 [num num]x1 | 1 | 3 | 0.4 0.5 ¦ ( * %0 0.5 ) ( * %0 0.75 ) ¦ ( * 0.5 ( + %0 5 ) ) ( * 0.9 ( + %0 5 ) ) | 미확인(자동) |  |  |  |
| `/(define "heat_large_blast")/damage/energy` | block | [expr]x2 [num]x1 | 1 | 3 | 1.2 | 미확인(자동) |  |  | = 25mm of penetration ¦ = 140mm of penetration |
| `/(define "heat_large_blast")/damage/ground_interaction_radius` | block | [num]x3 | 1 | 3 | 0 ~ 0.5 | 미확인(자동) |  |  | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/(define "heat_medium_blast")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heat_medium_blast")/damage` | block | [atom]x1 [atom atom]x1 | 1 | 2 | blastwave ¦ add blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/(define "heat_medium_blast")/damage/area` | block | [num num]x1 [expr expr]x1 | 1 | 2 | 0.4 0.5 ¦ ( * 0.5 ( + %0 5 ) ) ( * 0.8 ( + %0 5 ) ) | 미확인(자동) |  |  |  |
| `/(define "heat_medium_blast")/damage/energy` | block | [expr]x1 [num]x1 | 1 | 2 | 1.2 | 미확인(자동) |  |  | = 25mm of penetration ¦ = 140mm of penetration |
| `/(define "heat_medium_blast")/damage/ground_interaction_radius` | block | [num]x2 | 1 | 2 | 0 ~ 0.5 | 미확인(자동) |  |  | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/(define "heat_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heat_slope_multiplier")/overmatchslope` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heat_slope_multiplier")/overmatchslope/#row` | block | [num num]x2 | 1 | 2 | 0 1 ¦ 90 1 | 미확인(자동) |  |  |  |
| `/(define "heavy_aa")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heavy_aa")/aimingtolerance` | block | [num]x1 | 1 | 1 | 5 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "heavy_aa")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "heavy_artillery")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heavy_artillery")/("artillery_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heavy_artillery")/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heavy_artillery")/parameters` | block | [str]x2 | 1 | 2 | "he" ¦ "he2" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "heavy_artillery")/parameters/spreading` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "heavy_artillery")/parameters/spreading/spreadpower` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/(define "heavy_artillery")/parameters/spreading/spreadxyratio` | block | [num]x2 | 1 | 2 | 0.75 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "heavy_artillery")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "heavy_artillery")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "heavy_artillery")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.85 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "heavy_artillery")/tracefreq` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/(define "heavy_artillery")/unlimitedrangetpc` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | restrict range in TPC/FPC |
| `/(define "heavy_artillery")/zeroingaccuracy` | block | [num num num num num]x1 | 1 | 1 | 0.80 0.65 0.55 0.45 0.40 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "heavy_mortar_range")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "heavy_mortar_range")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 10 ) | 미확인(자동) |  |  |  |
| `/(define "heavy_mortar_range")/aimrange` | block | [num]x1 | 1 | 1 | 230 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/(define "heavy_mortar_range")/maxrange` | block | [num]x1 | 1 | 1 | 230 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/(define "heavy_mortar_range")/minrange` | block | [num]x1 | 1 | 1 | 50 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/(define "heavy_mortar_range")/parameters` | block | [str]x4 | 1 | 4 | "he" ¦ "he2" ¦ "sm" ¦ "wp" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "heavy_mortar_range")/parameters/("reload time")` | macro | [call:seconds]x2 | 1 | 2 | seconds ( 3.5 ) | 미확인(자동) |  |  |  |
| `/(define "heavy_mortar_range")/parameters/projectiledamagetable` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "heavy_mortar_range")/parameters/projectiledamagetable/#row` | block | [num num]x2 | 1 | 2 | 0 30 | 미확인(자동) |  |  |  |
| `/(define "heavy_mortar_range")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "heavy_mortar_range")/spreading/radiustable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "heavy_mortar_range")/spreading/radiustable/#row` | block | [num num]x5 | 1 | 5 | 0 5 ¦ 120 25 ¦ 170 40 ¦ 229.99 50 ¦ 230 100 | 미확인(자동) |  |  |  |
| `/(define "human health")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "human health")/healthdamage` | block | [expr]x1 | 1 | 1 | ( * 1 %damage ) | 문서 | 사람과 동물에게 주는 체력 피해. 관통력과 별개의 값이며 단위는 임의다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/(define "hvap_76_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "hvap_76_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "hvap_76_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "hvap_90_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "hvap_90_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "hvap_90_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x13 | 1 | 13 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "ic")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "ic")/##damage` | block | [atom]x1 | 1 | 1 | fire | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 |  |
| `/(define "ic")/##damage/heating` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  |  |
| `/(define "ic")/##damage/radius` | block | [expr]x1 | 1 | 1 | ( * %0 3 ) | 추정 | 피해가 미치는 반경, 미터 (추정: 이름. 값 1.05) | 파일 구조와 값에서 추론 |  |
| `/(define "ic")/##damage/time` | block | [num]x1 | 1 | 1 | 1 | 추정 | 피해가 이어지는 시간, 초 (추정: 이름과 원문 주석 "seconds") | 파일 구조와 값에서 추론 |  |
| `/(define "indirect_settings")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "indirect_settings")/parameters` | block | [str]x2 | 1 | 2 | "he" ¦ "he2" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "indirect_settings")/parameters/spreading` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "indirect_settings")/parameters/spreading/spreadxyratio` | block | [num]x2 | 1 | 2 | 3 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "indirect_settings")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "indirect_settings")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "indirect_settings")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.75 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "indirect_settings")/zeroingaccuracy` | block | [num num num num num]x1 | 1 | 1 | 0.8 0.65 0.55 0.5 0.45 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "k98_rate")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "k98_rate")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 50 ) | 미확인(자동) |  |  |  |
| `/(define "k98_rate")/firingtimeout` | block | [num num]x1 | 1 | 1 | 3.5 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "k98_rate")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "k98_rate")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.5 ¦ 0.4 0.8 ¦ 0.6 0.9 ¦ 0.8 1.0 | 미확인(자동) |  |  |  |
| `/(define "large_apcr_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "large_apcr_slope_multiplier")/overmatchfixed` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "large_apcr_slope_multiplier")/overmatchfixed/#row` | block | [num num]x19 | 1 | 19 | 예: 0 1.00 | 미확인(자동) |  |  |  |
| `/(define "light_aa")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "light_aa")/aimingtolerance` | block | [num]x1 | 1 | 1 | 12 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "light_aa")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.1 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "light_mortar_range")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "light_mortar_range")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 2.7 ) | 미확인(자동) |  |  |  |
| `/(define "light_mortar_range")/aimrange` | block | [num]x1 | 1 | 1 | 140 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/(define "light_mortar_range")/maxrange` | block | [num]x1 | 1 | 1 | 140 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/(define "light_mortar_range")/minrange` | block | [num]x1 | 1 | 1 | 30 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/(define "light_mortar_range")/parameters` | block | [str]x2 | 1 | 2 | "he" ¦ "he2" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "light_mortar_range")/parameters/projectiledamagetable` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "light_mortar_range")/parameters/projectiledamagetable/#row` | block | [num num]x2 | 1 | 2 | 0 10 | 미확인(자동) |  |  |  |
| `/(define "light_mortar_range")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "light_mortar_range")/spreading/radiustable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "light_mortar_range")/spreading/radiustable/#row` | block | [num num]x4 | 1 | 4 | 0 5 ¦ 120 18 ¦ 139.99 25 ¦ 140 100 | 미확인(자동) |  |  |  |
| `/(define "marksman")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "marksman")/zeroingaccuracy` | block | [num num num num num num num]x1 | 1 | 1 | 0.9 0.85 0.8 0.75 0.7 0.65 0.6 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "medium_aa")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "medium_aa")/aimingtolerance` | block | [num]x1 | 1 | 1 | 12 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "medium_aa")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.1 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "medium_artillery")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "medium_artillery")/("artillery_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "medium_artillery")/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "medium_artillery")/parameters` | block | [str]x2 | 1 | 2 | "he" ¦ "he2" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "medium_artillery")/parameters/spreading` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "medium_artillery")/parameters/spreading/spreadpower` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/(define "medium_artillery")/parameters/spreading/spreadxyratio` | block | [num]x2 | 1 | 2 | 0.75 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "medium_artillery")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "medium_artillery")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "medium_artillery")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.85 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "medium_artillery")/tracefreq` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/(define "medium_artillery")/unlimitedrangetpc` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | restrict range in TPC/FPC |
| `/(define "medium_artillery")/zeroingaccuracy` | block | [num num num num num]x1 | 1 | 1 | 0.80 0.65 0.55 0.45 0.40 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "medium_heavy_mortar_range")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "medium_heavy_mortar_range")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 7 ) | 미확인(자동) |  |  |  |
| `/(define "medium_heavy_mortar_range")/aimrange` | block | [num]x1 | 1 | 1 | 180 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/(define "medium_heavy_mortar_range")/maxrange` | block | [num]x1 | 1 | 1 | 180 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/(define "medium_heavy_mortar_range")/minrange` | block | [num]x1 | 1 | 1 | 45 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/(define "medium_heavy_mortar_range")/parameters` | block | [str]x4 | 1 | 4 | "he" ¦ "he2" ¦ "sm" ¦ "wp" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "medium_heavy_mortar_range")/parameters/("reload time")` | macro | [call:seconds]x2 | 1 | 2 | seconds ( 3.4 ) | 미확인(자동) |  |  |  |
| `/(define "medium_heavy_mortar_range")/parameters/projectiledamagetable` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "medium_heavy_mortar_range")/parameters/projectiledamagetable/#row` | block | [num num]x2 | 1 | 2 | 0 20 | 미확인(자동) |  |  |  |
| `/(define "medium_heavy_mortar_range")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "medium_heavy_mortar_range")/spreading/radiustable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "medium_heavy_mortar_range")/spreading/radiustable/#row` | block | [num num]x4 | 1 | 4 | 0 5 ¦ 120 25 ¦ 179.99 45 ¦ 180 100 | 미확인(자동) |  |  |  |
| `/(define "medium_mortar_range")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "medium_mortar_range")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 5 ) | 미확인(자동) |  |  |  |
| `/(define "medium_mortar_range")/aimrange` | block | [num]x1 | 1 | 1 | 160 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/(define "medium_mortar_range")/maxrange` | block | [num]x1 | 1 | 1 | 160 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/(define "medium_mortar_range")/minrange` | block | [num]x1 | 1 | 1 | 40 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/(define "medium_mortar_range")/parameters` | block | [str]x4 | 1 | 4 | "he" ¦ "he2" ¦ "sm" ¦ "wp" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "medium_mortar_range")/parameters/("reload time")` | macro | [call:seconds]x2 | 1 | 2 | seconds ( 3.3 ) | 미확인(자동) |  |  |  |
| `/(define "medium_mortar_range")/parameters/projectiledamagetable` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "medium_mortar_range")/parameters/projectiledamagetable/#row` | block | [num num]x2 | 1 | 2 | 0 15 | 미확인(자동) |  |  |  |
| `/(define "medium_mortar_range")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "medium_mortar_range")/spreading/radiustable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "medium_mortar_range")/spreading/radiustable/#row` | block | [num num]x4 | 1 | 4 | 0 5 ¦ 120 25 ¦ 159.99 45 ¦ 160 100 | 미확인(자동) |  |  |  |
| `/(define "minimum_range_heavy")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "minimum_range_heavy")/possibledamageradius` | block | [num]x1 | 1 | 1 | 30 | 미확인(자동) |  |  |  |
| `/(define "minimum_range_medium")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "minimum_range_medium")/possibledamageradius` | block | [num]x1 | 1 | 1 | 25 | 미확인(자동) |  |  |  |
| `/(define "minimum_range_short")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "minimum_range_short")/possibledamageradius` | block | [num]x1 | 1 | 1 | 20 | 미확인(자동) |  |  |  |
| `/(define "mosin_rate")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "mosin_rate")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 40 ) | 미확인(자동) |  |  |  |
| `/(define "mosin_rate")/firingtimeout` | block | [num num]x1 | 1 | 1 | 4 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "mosin_rate")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "mosin_rate")/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 1 | 4 | 0.2 0.5 ¦ 0.4 0.8 ¦ 0.6 0.9 ¦ 0.8 1.0 | 미확인(자동) |  |  |  |
| `/(define "penetration_long")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "penetration_long")/parameters` | block | [str]x1 | 1 | 1 | "%shell" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "penetration_long")/parameters/projectiledamagetable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "penetration_long")/parameters/projectiledamagetable/{}` | block | [expr num]x1 | 1 | 1 | ( + %range 15 ) 0 | 문서 | 거리와 그 거리에서의 관통력 한 쌍. 점 사이는 선형으로 잇는다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/(define "penetration_long")/parameters/projectiledamagetable/#row` | block | [num param]x4 [num expr]x1 | 1 | 5 | 10 ( + %a ( + 1.3 ( / ( - %a %b ) 4.4 ) ) ) ¦ 30 %a ¦ 100 %b ¦ 155 %c ¦ 205 %d | 미확인(자동) |  |  |  |
| `/(define "penetration_long")/parameters/projectiledamagetable/%range` | block | [expr]x1 | 1 | 1 | ( + ( * ( / ( - 250 %range ) ( - 250 200 ) ) ( - %d %e ) ) %e ) | 문서 | 거리와 그 거리에서의 관통력 한 쌍. 점 사이는 선형으로 잇는다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/(define "penetration_long")/parameters/range` | block | [param param]x1 | 1 | 1 | %range %range | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/(define "penetration_medium")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "penetration_medium")/parameters` | block | [str]x1 | 1 | 1 | "%shell" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "penetration_medium")/parameters/projectiledamagetable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "penetration_medium")/parameters/projectiledamagetable/{}` | block | [expr num]x1 | 1 | 1 | ( + %range 15 ) 0 | 문서 | 거리와 그 거리에서의 관통력 한 쌍. 점 사이는 선형으로 잇는다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/(define "penetration_medium")/parameters/projectiledamagetable/#row` | block | [num param]x3 [num expr]x1 | 1 | 4 | 10 ( + %a ( + 1.3 ( / ( - %a %b ) 4.4 ) ) ) ¦ 30 %a ¦ 100 %b ¦ 155 %c | 미확인(자동) |  |  |  |
| `/(define "penetration_medium")/parameters/projectiledamagetable/%range` | block | [expr]x1 | 1 | 1 | ( + ( * ( / ( - 205 %range ) ( - 205 155 ) ) ( - %c %d ) ) %d ) | 문서 | 거리와 그 거리에서의 관통력 한 쌍. 점 사이는 선형으로 잇는다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/(define "penetration_medium")/parameters/range` | block | [param param]x1 | 1 | 1 | %range %range | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/(define "penetration_short")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "penetration_short")/parameters` | block | [str]x1 | 1 | 1 | "%shell" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "penetration_short")/parameters/projectiledamagetable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "penetration_short")/parameters/projectiledamagetable/{}` | block | [expr num]x1 | 1 | 1 | ( + %range 15 ) 0 | 문서 | 거리와 그 거리에서의 관통력 한 쌍. 점 사이는 선형으로 잇는다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/(define "penetration_short")/parameters/projectiledamagetable/#row` | block | [num param]x2 [num expr]x1 | 1 | 3 | 10 ( + %a ( + 1.3 ( / ( - %a %b ) 4.4 ) ) ) ¦ 30 %a ¦ 100 %b | 미확인(자동) |  |  |  |
| `/(define "penetration_short")/parameters/projectiledamagetable/%range` | block | [expr]x1 | 1 | 1 | ( + ( * ( / ( - 155 %range ) ( - 155 100 ) ) ( - %b %c ) ) %c ) | 문서 | 거리와 그 거리에서의 관통력 한 쌍. 점 사이는 선형으로 잇는다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/(define "penetration_short")/parameters/range` | block | [param param]x1 | 1 | 1 | %range %range | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/(define "penetration")` | define | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |  |
| `/(define "penetration")/projectiledamagetable` | block | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "penetration")/projectiledamagetable/#row` | block | [num param]x26 [num num]x4 [num expr]x2 | 9 | 32 | 예: 71 %0 | 미확인(자동) |  |  |  |
| `/(define "plane_bomb_weapon")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "plane_bomb_weapon")/aimrange` | block | [num]x1 | 1 | 1 | 200 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/(define "plane_bomb_weapon")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "plane_bomb_weapon")/cursorreloading` | block | [str]x1 | 1 | 1 | "gun_he" | 미확인(자동) |  |  | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/(define "plane_bomb_weapon")/fillingvisible` | block | [str]x1 | 1 | 1 | "bomb" | 미확인(자동) |  |  |  |
| `/(define "plane_bomb_weapon")/foresight` | block | [str]x1 | 1 | 1 | "pivot_front" | 미확인(자동) |  |  | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/(define "plane_bomb_weapon")/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "plane_bomb_weapon")/rechargetime` | block | [num]x1 | 1 | 1 | 40 | 미확인(자동) |  |  |  |
| `/(define "plane_bomb_weapon")/recoverytime` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | increase fuel consumption by 2x |
| `/(define "plane_bomb_weapon")/tag` | block | [str str]x1 | 1 | 1 | "weapon" "bomb" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "pointed_slope")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "pointed_slope")/overmatchslope` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "pointed_slope")/overmatchslope/#row` | block | [num num]x4 | 1 | 4 | 0 1 ¦ 50 1 ¦ 60 2 ¦ 90 99 | 미확인(자동) |  |  |  |
| `/(define "range_160_howitzer")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_160_howitzer")/("curved_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_160_howitzer")/("indirect_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_160_howitzer")/range` | block | [num num]x1 | 1 | 1 | 160 160 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_160")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_160")/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_160")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_160")/range` | block | [num num]x1 | 1 | 1 | 160 160 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_170_howitzer")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_170_howitzer")/("curved_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_170_howitzer")/("indirect_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_170_howitzer")/range` | block | [num num]x1 | 1 | 1 | 170 170 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_170")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_170")/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_170")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_170")/range` | block | [num num]x1 | 1 | 1 | 170 170 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_180_howitzer")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_180_howitzer")/("curved_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_180_howitzer")/("indirect_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_180_howitzer")/range` | block | [num num]x1 | 1 | 1 | 180 180 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_180")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_180")/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_180")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_180")/range` | block | [num num]x1 | 1 | 1 | 180 180 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_190_howitzer")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_190_howitzer")/("curved_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_190_howitzer")/("indirect_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_190_howitzer")/range` | block | [num num]x1 | 1 | 1 | 190 190 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_190")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_190")/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_190")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_190")/range` | block | [num num]x1 | 1 | 1 | 190 190 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_200_howitzer")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_200_howitzer")/("curved_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_200_howitzer")/("indirect_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_200_howitzer")/range` | block | [num num]x1 | 1 | 1 | 200 200 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_200")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_200")/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_200")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_200")/range` | block | [num num]x1 | 1 | 1 | 200 200 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_210_howitzer")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_210_howitzer")/("curved_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_210_howitzer")/("indirect_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_210_howitzer")/range` | block | [num num]x1 | 1 | 1 | 210 210 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_210")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_210")/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_210")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_210")/range` | block | [num num]x1 | 1 | 1 | 210 210 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_220")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_220")/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_220")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_220")/range` | block | [num num]x1 | 1 | 1 | 220 220 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_aircraft_gun")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_aircraft_gun")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_aircraft_gun")/(mod)` | mod | [atom str]x1 | 1 | 1 | not "mp" | 미확인(자동) |  |  |  |
| `/(define "range_aircraft_gun")/aimingtolerance` | block | [num]x2 | 1 | 2 | 10 ~ 15 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "range_aircraft_gun")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_aircraft_gun")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_aircraft_gun")/burst/long` | block | [num num]x1 | 1 | 1 | 50 10 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "range_aircraft_gun")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.75 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "range_aircraft_gun")/burst/short` | block | [num num]x1 | 1 | 1 | 20 5 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "range_aircraft_gun")/range` | block | [num num]x2 | 1 | 2 | 140 140 ¦ 275 500 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_aircraft_gun")/spreading` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "range_aircraft_gun")/spreading/burstaccuracy` | block | [num num num]x2 | 1 | 2 | 100 95 90 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "range_aircraft_gun")/spreading/burstrecoverytime` | block | [num]x2 | 1 | 2 | 0.5 | 확인 | 사격 뒤 명중률이 평소로 돌아오는 데 걸리는 시간, 초 | 게임 파일 주석 | time in seconds for weapon accuracy to return to normal after firing. |
| `/(define "range_aircraft_gun")/spreading/radiustable` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "range_aircraft_gun")/spreading/radiustable/#row` | block | [num num]x8 | 1 | 8 | 0 0.5 ¦ 200 2 ¦ 300 5 ¦ 350 12 ¦ 0 0 ¦ 80 2 ¦ 130 5 ¦ 200 10 | 미확인(자동) |  |  |  |
| `/(define "range_aircraft_gun")/spreading/spreadpower` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/(define "range_aircraft_gun")/spreading/spreadxyratio` | block | [num]x2 | 1 | 2 | 5 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "range_aircraft_gun")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "range_aircraft_gun")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "range_autocannon_12-20")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_12-20")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_12-20")/aimingtolerance` | block | [num]x1 | 1 | 1 | 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "range_autocannon_12-20")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_12-20")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_12-20")/burst/long` | block | [num num]x1 | 1 | 1 | 5 1 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "range_autocannon_12-20")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "range_autocannon_12-20")/burst/short` | block | [num num]x1 | 1 | 1 | 2 0.75 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "range_autocannon_12-20")/curve` | block | [str]x1 | 1 | 1 | "gun_autocannon" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/(define "range_autocannon_12-20")/parameters` | block | [str]x4 | 1 | 4 | "ap" ¦ "apbc" ¦ "apcr" ¦ "he" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "range_autocannon_12-20")/parameters/("human health")` | macro | [call:damage]x4 | 1 | 4 | damage ( 250 ) ¦ damage ( 500 ) | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_12-20")/range` | block | [num num]x1 | 1 | 1 | 150 150 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_autocannon_12-20")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "range_autocannon_12-20")/spreading/burstaccuracy` | block | [num num num num num num num num num num num num num num num]x1 | 1 | 1 | 100 98 95 90 85 80 75 70 65 60 55 50 45 40 35 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "range_autocannon_12-20")/spreading/burstrecoverytime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 사격 뒤 명중률이 평소로 돌아오는 데 걸리는 시간, 초 | 게임 파일 주석 | time in seconds for weapon accuracy to return to normal after firing. |
| `/(define "range_autocannon_12-20")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "range_autocannon_12-20")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "range_autocannon_12-20")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "range_autocannon_37-40")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_37-40")/("standard_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_37-40")/aimingtolerance` | block | [num]x1 | 1 | 1 | 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/(define "range_autocannon_37-40")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_37-40")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "range_autocannon_37-40")/burst/long` | block | [num num]x1 | 1 | 1 | 5 1 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "range_autocannon_37-40")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.85 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "range_autocannon_37-40")/burst/short` | block | [num num]x1 | 1 | 1 | 2 0.75 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "range_autocannon_37-40")/range` | block | [num num]x1 | 1 | 1 | 160 160 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "range_autocannon_37-40")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "range_autocannon_37-40")/spreading/burstaccuracy` | block | [num num num num num num num num]x1 | 1 | 1 | 100 95 88 82 75 68 55 32 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "range_autocannon_37-40")/spreading/burstrecoverytime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 사격 뒤 명중률이 평소로 돌아오는 데 걸리는 시간, 초 | 게임 파일 주석 | time in seconds for weapon accuracy to return to normal after firing. |
| `/(define "range_autocannon_37-40")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 0.5 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "range_autocannon_37-40")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "range_autocannon_37-40")/syncedprojectiles` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/(define "reload time")` | define | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "reload time")/rechargetime` | block | [param]x3 | 3 | 3 | %seconds | 미확인(자동) |  |  |  |
| `/(define "reload_belt")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "reload_belt")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 8.5 ) | 미확인(자동) |  |  |  |
| `/(define "rocket_artillery")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "rocket_artillery")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 60 ) | 미확인(자동) |  |  |  |
| `/(define "rocket_artillery")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 120 ) | 미확인(자동) |  |  |  |
| `/(define "rocket_artillery")/fillingvisible` | block | [str]x1 | 1 | 1 | "rocket" | 미확인(자동) |  |  |  |
| `/(define "rocket_artillery")/gravity` | block | [num]x1 | 1 | 1 | 4 | 미확인(자동) |  |  | same as other direct-fire "cannon" type weapons |
| `/(define "rocket_artillery")/inventory` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "rocket_artillery")/inventory/noview` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "rocket_artillery")/range` | block | [num num]x1 | 1 | 1 | 300 300 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "rocket_artillery")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "rocket_artillery")/spreading/radiustable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "rocket_artillery")/spreading/radiustable/#row` | block | [num num]x6 | 1 | 6 | 0 0 ¦ 100 10 ¦ 200 20 ¦ 299.99 50 ¦ 300 100 ¦ 320 200 | 미확인(자동) |  |  |  |
| `/(define "rus_ap_bullets")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "rus_ap_bullets")/parameters` | block | [str]x2 | 1 | 2 | "ap" ¦ "api" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "rus_ap_bullets")/parameters/("armor health")` | macro | [call:damage]x2 | 1 | 2 | damage ( 5 ) ¦ damage ( 6 ) | 미확인(자동) |  |  |  |
| `/(define "rus_ap_bullets")/parameters/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 100 ) | 미확인(자동) |  |  |  |
| `/(define "rus_ap_bullets")/parameters/("penetration")` | macro | [atom num num num num num]x2 | 1 | 2 | args 17.8 8.6 4.2 2 0 | 미확인(자동) |  |  |  |
| `/(define "rus_ap_bullets")/parameters/cursorreloading` | block | [str]x2 | 1 | 2 | "gun_ap" ¦ "gun_api" | 미확인(자동) |  |  | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/(define "sap_130_slope_multiplier")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "sap_130_slope_multiplier")/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "sap_130_slope_multiplier")/overmatchcalibre/#row` | block | [num num num]x15 | 1 | 15 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams` | block | [atom str]x1 | 1 | 1 | inherit "human default" | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level` | block | [num]x1 | 1 | 1 | 1 | 추정 | 3인칭 조준의 확대 단계 하나. 값은 단계 번호 (추정: 값 1, 2와 하위 camera) | 파일 구조와 값에서 추론 | 2 |
| `/(define "scopeview_2")/thirdpersonparams/level/aimer_type` | block | [str]x1 | 1 | 1 | "sniper" | 추정 | 조준 방식 이름 (추정: 값 "sniper") | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/blur_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/zoom_vignette_scope_01_mask" | 추정 | 가장자리 흐림 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/blur_strength` | block | [num]x1 | 1 | 1 | 10 | 추정 | 가장자리 흐림의 세기 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/bump_spec_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/%scope" | 추정 | 조준경 화면에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/camera` | block | [str]x1 | 1 | 1 | "human_zoom4" | 확인 | 이 단계에서 쓰는 카메라 프리셋. 배율을 정한다. human_zoom4는 4배 | 게임 파일 주석 | magnification x4 ¦ magnification x8 |
| `/(define "scopeview_2")/thirdpersonparams/level/delta` | block | [num]x1 | 1 | 1 | 20 | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/distance` | block | [num]x1 | 1 | 1 | 35 | 미확인(자동) |  |  | activation distance |
| `/(define "scopeview_2")/thirdpersonparams/level/distanceset` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/drop_zoom_after_shot` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/drop_zoom_on_reload` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/hide_crosshair` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/hide_weapon` | block | [num]x1 | 1 | 1 | 1 | 추정 | 조준 중에 무기 모델을 숨길지 여부 (추정: 이름과 값 1) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/lens_dirt_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/dirty_lens_scope_01" | 추정 | 렌즈 얼룩 효과에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/linkbone` | block | [str]x1 | 1 | 1 | "head" | 추정 | 조준 카메라가 붙는 뼈대 (추정: 값 "head") | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/manualaccuracyzoom` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/mouse_speed_factor` | block | [num]x1 | 1 | 1 | 0.6 | 추정 | 조준 중 마우스 감도 배수 (추정: 이름과 값 0.6 ~ 0.8) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/offset` | block | [num num num]x1 | 1 | 1 | 35 2.25 0.75 | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/refraction_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/zoom_vignette_scope_01_bp" | 추정 | 굴절 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/scope_vignette` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/%scope" | 추정 | 조준경 테두리로 쓰는 텍스처 (추정: 값이 lens_effects 폴더의 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview_2")/thirdpersonparams/level/vignette_fade_speed` | block | [num]x1 | 1 | 1 | 5 | 확인 | 조준경 테두리(비네트)가 나타나고 사라지는 속도 계수 | 게임 파일 주석 | factoring the speed at which the vignette fades in/out |
| `/(define "scopeview_2")/thirdpersonparams/level/vignette_gloss` | block | [num]x1 | 1 | 1 | 10 | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/vignette_recoil` | block | [num num]x1 | 1 | 1 | 2.75 1.1 | 확인 | 반동 때 조준경 테두리가 흔들리는 속도와 크기 | 게임 파일 주석 | speed, scale |
| `/(define "scopeview_2")/thirdpersonparams/level/vision_modes_active` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/weapon_sway_active` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/with_owner` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/with_owner/viewoff` | block | [str]x1 | 1 | 1 | "skin" | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/with_weapon` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/with_weapon/viewon` | block | [str]x1 | 1 | 1 | "foresight3" | 미확인(자동) |  |  |  |
| `/(define "scopeview_2")/thirdpersonparams/level/zoom` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams` | block | [atom str]x2 | 2 | 2 | inherit "human default" | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level` | block | [num]x2 | 2 | 2 | 1 | 추정 | 3인칭 조준의 확대 단계 하나. 값은 단계 번호 (추정: 값 1, 2와 하위 camera) | 파일 구조와 값에서 추론 | 2 |
| `/(define "scopeview")/thirdpersonparams/level/aimer_type` | block | [str]x2 | 2 | 2 | "sniper" | 추정 | 조준 방식 이름 (추정: 값 "sniper") | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/blur_mask` | block | [str]x2 | 2 | 2 | "/texture/common/environment/lens_effects/zoom_vignette_scope_01_mask" | 추정 | 가장자리 흐림 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/blur_strength` | block | [num]x2 | 2 | 2 | 10 | 추정 | 가장자리 흐림의 세기 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/bump_spec_mask` | block | [str]x2 | 2 | 2 | "/texture/common/environment/lens_effects/%scope" | 추정 | 조준경 화면에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/camera` | block | [str]x2 | 2 | 2 | "human_zoom4" | 확인 | 이 단계에서 쓰는 카메라 프리셋. 배율을 정한다. human_zoom4는 4배 | 게임 파일 주석 | magnification x4 ¦ magnification x8 |
| `/(define "scopeview")/thirdpersonparams/level/delta` | block | [num]x2 | 2 | 2 | 20 | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/distance` | block | [num]x2 | 2 | 2 | 35 | 미확인(자동) |  |  | activation distance |
| `/(define "scopeview")/thirdpersonparams/level/distanceset` | block | [num]x2 | 2 | 2 | 0 | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/drop_zoom_on_reload` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/hide_crosshair` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/hide_weapon` | block | [num]x2 | 2 | 2 | 1 | 추정 | 조준 중에 무기 모델을 숨길지 여부 (추정: 이름과 값 1) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/lens_dirt_mask` | block | [str]x2 | 2 | 2 | "/texture/common/environment/lens_effects/dirty_lens_scope_01" | 추정 | 렌즈 얼룩 효과에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/linkbone` | block | [str]x2 | 2 | 2 | "head" | 추정 | 조준 카메라가 붙는 뼈대 (추정: 값 "head") | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/manualaccuracyzoom` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/mouse_speed_factor` | block | [num]x2 | 2 | 2 | 0.6 | 추정 | 조준 중 마우스 감도 배수 (추정: 이름과 값 0.6 ~ 0.8) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/offset` | block | [num num num]x2 | 2 | 2 | 35 2.25 0.75 | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/refraction_mask` | block | [str]x2 | 2 | 2 | "/texture/common/environment/lens_effects/zoom_vignette_scope_01_bp" | 추정 | 굴절 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/scope_vignette` | block | [str]x2 | 2 | 2 | "/texture/common/environment/lens_effects/%scope" | 추정 | 조준경 테두리로 쓰는 텍스처 (추정: 값이 lens_effects 폴더의 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/(define "scopeview")/thirdpersonparams/level/vignette_fade_speed` | block | [num]x2 | 2 | 2 | 5 | 확인 | 조준경 테두리(비네트)가 나타나고 사라지는 속도 계수 | 게임 파일 주석 | factoring the speed at which the vignette fades in/out |
| `/(define "scopeview")/thirdpersonparams/level/vignette_gloss` | block | [num]x2 | 2 | 2 | 10 | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/vignette_recoil` | block | [num num]x2 | 2 | 2 | 2.75 1.1 | 확인 | 반동 때 조준경 테두리가 흔들리는 속도와 크기 | 게임 파일 주석 | speed, scale |
| `/(define "scopeview")/thirdpersonparams/level/vision_modes_active` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/weapon_sway_active` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/with_owner` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/with_owner/viewoff` | block | [str]x2 | 2 | 2 | "skin" | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/with_weapon` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/with_weapon/viewon` | block | [str]x2 | 2 | 2 | "foresight3" | 미확인(자동) |  |  |  |
| `/(define "scopeview")/thirdpersonparams/level/zoom` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/("accuracy_rifle_auto")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/("ap_7_62_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/("fire rate")` | macro | [call:rpm]x1 | 1 | 1 | rpm ( 150 ) | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 70 ) | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/("recoil_rifle_semi")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/accuracybymotionk` | block | [num]x1 | 1 | 1 | 1.2 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/(define "semi_rifle")/automatic` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/burst` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/burst/long` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/(define "semi_rifle")/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | rangeKoef * aimRange = max long burst distance |
| `/(define "semi_rifle")/burst/short` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/(define "semi_rifle")/curve` | block | [str]x1 | 1 | 1 | "autorifle" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/(define "semi_rifle")/firingtimeout` | block | [num num]x1 | 1 | 1 | 2.0 0.5 | 미확인(자동) |  |  | 3.5 0.5 ¦ delay between bursts |
| `/(define "semi_rifle")/firingtimeoutdistancefactor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/firingtimeoutdistancefactor/#row` | block | [num num]x3 | 1 | 3 | 0.2 0.55 ¦ 0.4 0.75 ¦ 0.8 1.0 | 미확인(자동) |  |  |  |
| `/(define "semi_rifle")/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/(define "semi_rifle")/range` | block | [num num]x1 | 1 | 1 | 120 120 | 미확인(자동) |  |  | 200m ¦ 285m max |
| `/(define "semi_rifle")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "semi_rifle")/spreading/burstaccuracy` | block | [num num num num]x1 | 1 | 1 | 100 80 60 40 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "semi_rifle")/tag` | block | [str]x1 | 1 | 1 | "auto_eject semi_auto autorifle" | 미확인(자동) |  |  | game code ¦ fx code |
| `/(define "spreading_high")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "spreading_high")/spreading` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "spreading_high")/spreading/burstaccuracy` | block | [num num num num num num num num num num num]x1 [num num num num num num num num num]x1 | 2 | 2 | 100 90 80 70 60 55 50 45 40 35 30 ¦ 100 92 84 76 69 62 56 50 45 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "spreading_low")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "spreading_low")/spreading` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "spreading_low")/spreading/burstaccuracy` | block | [num num num num num num num num num num num num num num num]x1 [num num num num num num num]x1 | 2 | 2 | 100 98 94 88 83 76 72 67 61 56 50 45 40 35 30 ¦ 100 95 90 85 80 75 70 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "spreading_max")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "spreading_max")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "spreading_max")/spreading/burstaccuracy` | block | [num num num num num num num num num]x1 | 1 | 1 | 100 91 82 73 65 57 51 45 40 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "spreading_medium")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "spreading_medium")/spreading` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "spreading_medium")/spreading/burstaccuracy` | block | [num num num num num num num num num num num num num num]x1 [num num num num num num num]x1 | 2 | 2 | 100 95 90 84 78 70 64 60 55 50 45 40 35 30 ¦ 100 93 86 79 72 66 60 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "spreading_mounted_low")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "spreading_mounted_low")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "spreading_mounted_low")/spreading/burstaccuracy` | block | [num num num num num num num num num]x1 | 1 | 1 | 100 98 95 93 90 88 85 83 80 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "spreading_mounted")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "spreading_mounted")/spreading` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "spreading_mounted")/spreading/burstaccuracy` | block | [num num num num num num num num]x1 [num num num num num num]x1 | 2 | 2 | 100 98 95 91 86 81 76 70 ¦ 100 95 90 85 80 75 | 미확인(자동) |  |  | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/(define "standard_firing_arc")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "standard_firing_arc")/gravity` | block | [num]x1 | 1 | 1 | 3.5 | 미확인(자동) |  |  | same as other direct-fire "cannon" type weapons |
| `/(define "standard_slope")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "standard_slope")/overmatchslope` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "standard_slope")/overmatchslope/#row` | block | [num num]x2 | 1 | 2 | 0 1 ¦ 90 1 | 미확인(자동) |  |  |  |
| `/(define "stick_reload")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "stick_reload")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 3.25 ) | 미확인(자동) |  |  |  |
| `/(define "super_heavy_artillery")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "super_heavy_artillery")/("artillery_firing_arc")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "super_heavy_artillery")/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "super_heavy_artillery")/parameters` | block | [str]x2 | 1 | 2 | "he" ¦ "he2" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "super_heavy_artillery")/parameters/spreading` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "super_heavy_artillery")/parameters/spreading/spreadpower` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/(define "super_heavy_artillery")/parameters/spreading/spreadxyratio` | block | [num]x2 | 1 | 2 | 0.75 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "super_heavy_artillery")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "super_heavy_artillery")/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 0.4 | 미확인(자동) |  |  | Shots land more longitudinally than transversely. |
| `/(define "super_heavy_artillery")/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.85 | 미확인(자동) |  |  | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/(define "super_heavy_artillery")/tracefreq` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/(define "super_heavy_artillery")/unlimitedrangetpc` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  | restrict range in TPC/FPC |
| `/(define "super_heavy_artillery")/zeroingaccuracy` | block | [num num num num num]x1 | 1 | 1 | 0.80 0.65 0.55 0.45 0.40 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(define "super_heavy_mortar_range")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "super_heavy_mortar_range")/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 10 ) | 미확인(자동) |  |  |  |
| `/(define "super_heavy_mortar_range")/aimrange` | block | [num]x1 | 1 | 1 | 150 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/(define "super_heavy_mortar_range")/maxrange` | block | [num]x1 | 1 | 1 | 150 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/(define "super_heavy_mortar_range")/minrange` | block | [num]x1 | 1 | 1 | 25 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/(define "super_heavy_mortar_range")/parameters` | block | [str]x4 | 1 | 4 | "he" ¦ "he2" ¦ "sm" ¦ "wp" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "super_heavy_mortar_range")/parameters/("reload time")` | macro | [call:seconds]x2 | 1 | 2 | seconds ( 3.5 ) | 미확인(자동) |  |  |  |
| `/(define "super_heavy_mortar_range")/parameters/projectiledamagetable` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/(define "super_heavy_mortar_range")/parameters/projectiledamagetable/#row` | block | [num num]x2 | 1 | 2 | 0 30 | 미확인(자동) |  |  |  |
| `/(define "super_heavy_mortar_range")/spreading` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/(define "super_heavy_mortar_range")/spreading/radiustable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/(define "super_heavy_mortar_range")/spreading/radiustable/#row` | block | [num num]x3 | 1 | 3 | 0 5 ¦ 149.99 20 ¦ 150 100 | 미확인(자동) |  |  |  |
| `/(define "velocity m/s")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/(define "velocity m/s")/("global velocity")` | macro | [call:gv]x1 | 1 | 1 | gv ( %he ) | 미확인(자동) |  |  |  |
| `/(define "velocity m/s")/parameters` | block | [str]x22 | 1 | 22 | 예: "ap" | 미확인(자동) |  |  | SPgGr. ¦ Sprgr. |
| `/(define "velocity m/s")/parameters/("global velocity")` | macro | [call:gv]x22 | 1 | 22 | gv ( %ap ) ¦ gv ( %he ) ¦ gv ( %heat ) ¦ gv ( %apcr ) | 미확인(자동) |  |  |  |
| `/(define "velocity")` | define | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/(define "velocity")/("global velocity")` | macro | [call:gv]x1 | 1 | 1 | gv ( %mps ) | 미확인(자동) |  |  |  |
| `/(define "velocity")/speed` | block | [expr]x2 | 2 | 2 | ( * 0.75 %mps ) ¦ ( * 0.35 %mps ) | 미확인(자동) |  |  | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/(define "zeroing_boost")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/(define "zeroing_boost")/zeroingaccuracy` | block | [num num num num num num num num num]x1 [num num num num num num num num]x1 | 2 | 2 | 0.95 0.9 0.85 0.825 0.8 0.775 0.75 0.725 0.7 ¦ 0.975 0.95 0.925 0.9 0.875 0.85 0.825 0.8 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/(include)` | include | [str]x13 | 12 | 13 | "\set\small_firearms_accuracy.inc" ¦ "\set\blast.inc" | 미확인(자동) |  |  |  |
| `/curve` | block | [str]x1 | 1 | 1 | "gun_std" | 미확인(자동) |  |  | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |

### 파일 종류: .weapon

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/from` | block | [str]x53 | 53 | 53 | "pattern reactive" ¦ "pattern mortar" ¦ "pattern rpg_weapon" ¦ "bazooka_m1" ¦ "weapon panzershreck_43" ¦ "127mm_hvar_4" ¦ "50mm_rocketprojector" ¦ "76mm_rp3_x2" ¦ "82mm_bm8_24" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/("aerial gravity")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("armor health")` | macro | [call:damage]x5 | 5 | 5 | damage ( 300 ) ¦ damage ( 250 ) | 미확인(자동) |  |  |  |
| `/from/("fire rate")` | macro | [call:rpm]x19 | 19 | 19 | rpm ( 36 ) ¦ rpm ( 80 ) ¦ rpm ( 600 ) ¦ rpm ( 60 ) ¦ rpm ( 288 ) ¦ rpm ( 120 ) ¦ rpm ( 106 ) ¦ rpm ( 33.3 ) ¦ rpm ( 37.5 ) ¦ rpm ( 240 ) | 미확인(자동) |  |  |  |
| `/from/("heavy_mortar_range")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("human health")` | macro | [call:damage]x8 | 8 | 8 | damage ( 500 ) | 미확인(자동) |  |  |  |
| `/from/("light_mortar_range")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("medium_heavy_mortar_range")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("medium_mortar_range")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/from/("penetration")` | macro | [atom num]x1 | 1 | 1 | args 185 | 미확인(자동) |  |  |  |
| `/from/("reload time")` | macro | [call:seconds]x26 | 26 | 26 | 예: seconds ( 30 ) | 미확인(자동) |  |  |  |
| `/from/("rocket_artillery")` | macro | []x14 | 14 | 14 | 예: | 미확인(자동) |  |  |  |
| `/from/("standard_slope")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |  |
| `/from/("super_heavy_mortar_range")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("velocity")` | macro | [call:mps]x24 | 24 | 24 | 예: mps ( 80.8 ) | 미확인(자동) |  |  |  |
| `/from/aimingtolerance` | block | [num]x1 | 1 | 1 | 1.5 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/from/aimrange` | block | [num]x15 | 15 | 15 | 140 ~ 325 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/from/aimtype` | block | [atom]x1 | 1 | 1 | curved | 미확인(자동) |  |  |  |
| `/from/ammolocked` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/automatic` | block | []x2 | 2 | 2 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/from/burst` | block | []x16 | 16 | 16 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 |  |
| `/from/burst/min` | block | [num]x16 | 16 | 16 | 4 ~ 30 | 미확인(자동) |  |  |  |
| `/from/burst/noaiming` | block | []x16 | 16 | 16 | 예: | 미확인(자동) |  |  |  |
| `/from/calibre` | block | [num]x45 | 45 | 45 | 50 ~ 380 | 확인 | 구경, mm | 게임 파일 주석 | caliber of gun in millimeters ¦ AP shot caliber |
| `/from/cursor` | block | [str]x1 | 1 | 1 | "ironsights/mortar" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/from/curve` | block | [str]x1 | 1 | 1 | "gun_std" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/from/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/entity` | block | [str]x12 | 12 | 12 | "bazooka_m1" ¦ "bazooka_m1a1" ¦ "bazooka_m9" ¦ "bazooka_m9a1" ¦ "faustpatrone_30" ¦ "panzerfaust_100" ¦ "panzerfaust_30" ¦ "panzerfaust_60" ¦ "panzershreck_43" ¦ "panzershreck_54" ¦ "piat" ¦ "recoilless_m18" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/from/falloffmeter` | block | [num]x2 | 2 | 2 | 180 | 미확인 | 값 1~220 |  |  |
| `/from/falloffstrength` | block | [num]x2 | 2 | 2 | 0.15 | 미확인 | 값 0~0.15 |  |  |
| `/from/filling` | block | [str num]x30 [str]x19 | 49 | 49 | 예: "ammo bazooka" 1 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/from/fillingvisible` | block | [str]x10 | 10 | 10 | "rocket" ¦ "faustrocket" ¦ "rocket00" | 미확인(자동) |  |  |  |
| `/from/firesound` | block | [str]x45 | 45 | 45 | 예: "reactive/d1/Shared/Rocket_Launcher/" | 추정 | 발사음 경로 (추정: 이름과 값) | 파일 구조와 값에서 추론 | Placeholder |
| `/from/firesoundclose` | block | [str]x23 | 23 | 23 | "cannon/s3/d1/Shared/Mortar/" ¦ "cannon/s2/d1/Shared/Mortar/" ¦ "reactive/3p/GER/Panzerfaust/" ¦ "cannon/s1/d1/Shared/Mortar/" ¦ "cannon/s4/3p/Shared/Cannon_105to130mm_LV/" ¦ "gun/gun_interior/size4" | 추정 | 가까이서 들리는 발사음 경로 (추정: 이름과 값의 3p) | 파일 구조와 값에서 추론 | Placeholder |
| `/from/gravity` | block | [num]x15 | 15 | 15 | 2 ~ 6 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/from/inventory` | block | []x8 | 8 | 8 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/noview` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/inventory/size` | block | [num num]x5 | 5 | 5 | 7 1 ¦ 7 2 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/from/mass` | block | [num]x12 | 12 | 12 | 6 ~ 20.1 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/from/maxrange` | block | [num]x16 | 16 | 16 | 140 ~ 325 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/from/minrange` | block | [num]x16 | 16 | 16 | 30 ~ 50 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/parameters` | block | [str]x17 | 10 | 17 | "he" ¦ "heat" ¦ "sm" ¦ "heata" ¦ "wp" ¦ "heatb" ¦ "he2 heavy" ¦ "aphe" | 추정 | 탄종별 설정. 문자열이 탄종 이름 (추정: 값 "heat", "clip") | 파일 구조와 값에서 추론 | SPgGr. ¦ Sprgr. |
| `/from/parameters/("armor health")` | macro | [call:damage]x7 | 3 | 7 | damage ( 300 ) ¦ damage ( 250 ) | 미확인(자동) |  |  |  |
| `/from/parameters/("penetration")` | macro | [atom num]x5 | 2 | 5 | args 82 ¦ args 88 ¦ args 102 ¦ args 80 ¦ args 100 | 미확인(자동) |  |  |  |
| `/from/parameters/("pointed_slope")` | macro | []x3 | 2 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/("reload time")` | macro | [call:seconds]x1 | 1 | 1 | seconds ( 8.0 ) | 미확인(자동) |  |  |  |
| `/from/parameters/("standard_slope")` | macro | []x3 | 2 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/aimrange` | block | [num]x1 | 1 | 1 | 140 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/from/parameters/cursorreloading` | block | [str]x11 | 5 | 11 | "gun_heat" ¦ "gun_wp" ¦ "gun_he" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/parameters/maxrange` | block | [num]x1 | 1 | 1 | 140 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/from/parameters/minimumdamagemodifier` | block | [num]x1 | 1 | 1 | 750 | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/from/parameters/projectiledamagetable` | block | []x5 | 5 | 5 | 예: | 추정 | 거리별 관통력 표 (추정: #row가 두 수의 쌍이고 주석이 탄종 이름) | 파일 구조와 값에서 추론 | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/from/parameters/projectiledamagetable/#row` | block | [num num]x7 | 5 | 7 | 0 30 ¦ 95 80 ¦ 100 0 ¦ 0 50 ¦ 0 76 ¦ 300 76 | 추정 | 거리와 관통력 한 쌍 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/from/projectiledamagetable` | block | []x4 | 4 | 4 | 예: | 추정 | 거리별 관통력 표 (추정: #row가 두 수의 쌍이고 주석이 탄종 이름) | 파일 구조와 값에서 추론 | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/from/projectiledamagetable/#row` | block | [num num]x8 | 4 | 8 | 36 0 ¦ 31 140 ¦ 51 200 ¦ 56 0 ¦ 31 200 ¦ 41 200 ¦ 46 0 | 추정 | 거리와 관통력 한 쌍 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/from/range` | block | [num num]x9 | 9 | 9 | 30 30 ¦ 1 300 ¦ 50 50 ¦ 40 40 ¦ 90 90 ¦ 270 270 ¦ 180 180 | 추정 | 사거리 두 값 (추정: 원문 주석이 "200m", "285m max". 두 값의 구분은 미확인) | 게임 파일 주석 | 200m ¦ 285m max |
| `/from/relaxationtime` | block | [num]x1 | 1 | 1 | 2.5 | 미확인 | 값 0.25~2 |  |  |
| `/from/reloadsound` | block | [str]x4 | 4 | 4 | "bazooka" ¦ "panzershreck" ¦ "PIAT_reload" | 확인 | 장전음 | 게임 파일 주석 | sound for weapon reload |
| `/from/speed` | block | [num]x2 [expr]x1 | 3 | 3 | 105 ~ 174 | 확인 | 탄속. "velocity" 매크로가 0.35 x m/s로 계산해 넣는다 | 게임 파일 주석 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/spreading` | block | []x20 [num]x7 | 27 | 27 | 1.25 ~ 12 | 추정 | 산포 설정 묶음 또는 산포 값 (추정: 하위 radiustable) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/spreading/radiusfunction` | block | [num num num num num num]x2 [num num num num num num num num num num num num]x1 | 3 | 3 | 1 0.1 300 1 400 2 ¦ 0 0.02 0.2 0.1 0.5 0.4 0.75 0.7 0.9995 1 1.00 50 | 미확인(자동) |  |  |  |
| `/from/spreading/radiustable` | block | []x18 | 18 | 18 | 예: | 확인 | 거리별 산포 반경 표. 최대 사거리를 넘으면 값이 급격히 커지게 해 원거리 사격을 막는다 | 게임 파일 주석 | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/from/spreading/radiustable/#row` | block | [num num]x99 | 18 | 99 | 예: 0 5 | 추정 | 거리와 산포 반경 한 쌍 (추정: .presets의 표가 {100 10} {200 20} 형태) | 파일 구조와 값에서 추론 |  |
| `/from/spreading/spreadradius` | block | [num]x2 | 2 | 2 | 4 | 확인 | 무기의 산포 반경, 미터 | 게임 파일 주석 | radius of weapon accuracy in meters. Most weapon spread radius is defined in small.firearms.accuracy |
| `/from/spreading/spreadxyratio` | block | [num]x5 | 5 | 5 | 1.8 ~ 5 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/from/spreading/spreadyadjust` | block | [num]x2 | 2 | 2 | 0.1 | 미확인(자동) |  |  |  |
| `/from/tag` | block | [str]x21 [str str]x1 | 22 | 22 | "gun" ¦ "nokeep rocketlauncher_fx" ¦ "rocketlauncher_fx" ¦ "-gun" ¦ "build_in" "gun" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from/unlimitedrangetpc` | block | [num]x1 | 1 | 1 | 0 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/from/zeroingaccuracy` | block | [num num num]x2 [num]x1 | 3 | 3 | 1 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/weapon_bullet` | block | []x1 | 1 | 1 | 예: | 추정 | 탄을 쏘는 무기 정의의 최상위 블록 (추정: 패턴 파일의 구조) | 파일 구조와 값에서 추론 | Example / Default Settings ¦ required for AT-Grenades with cumaltive damage |
| `/weapon_bullet/aimingtolerance` | block | [num]x1 | 1 | 1 | 10 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/weapon_bullet/clicksound` | block | [str]x1 | 1 | 1 | "" | 미확인(자동) |  |  |  |
| `/weapon_bullet/cursor` | block | [str]x1 | 1 | 1 | "ironsights/tank" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/weapon_bullet/filling` | block | [str num]x1 | 1 | 1 | "ammo goliath" 1 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_bullet/firesound` | block | [str]x1 | 1 | 1 | "" | 추정 | 발사음 경로 (추정: 이름과 값) | 파일 구조와 값에서 추론 | Placeholder |
| `/weapon_bullet/foresight` | block | [str]x1 | 1 | 1 | "foresight1" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/weapon_bullet/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/weapon_bullet/range` | block | [num num]x1 | 1 | 1 | 4 4.5 | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/weapon_bullet/speed` | block | [num]x1 | 1 | 1 | 500 | 추정 | 탄속 (추정: 위와 같은 키) | 파일 구조와 값에서 추론 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/weapon_bullet/tag` | block | [str]x1 | 1 | 1 | "gun" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |

### 파일 종류: (no-ext)

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/bullet` | block | []x1 | 1 | 1 | 예: | 추정 | 탄 정의의 최상위 블록 (추정: bullet.pattern) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/from` | block | [str]x2016 | 2016 | 2016 | 예: "pattern explosive" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/("100_rus_slope_multipliers")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("105_slope_multipliers")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |  |
| `/from/("122_rus_slope_multipliers")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("128_slope_multipliers")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("152_rus_slope_multipliers")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("20_slope_multipliers")` | macro | []x17 | 17 | 17 | 예: | 미확인(자동) |  |  |  |
| `/from/("37_slope_multipliers_800ms")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("37_slope_multipliers")` | macro | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/("40_eng_slope_multipliers")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("40_slope_multipliers")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("45_rus_slope_multipliers")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("57_eng_slope_multipliers")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("57_rus_slope_multipliers")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("57_slope_multipliers")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |  |
| `/from/("75_ger_slope_multipliers")` | macro | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/("75_slope_multipliers")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("76_eng_slope_multipliers")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("76_rus_slope_multipliers")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |  |
| `/from/("76_slope_multipliers")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("85_rus_slope_multipliers")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("85-155_slope_multipliers")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/from/("88_ger_slope_multipliers")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("90_slope_multipliers")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |  |
| `/from/("94_eng_slope_multipliers")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_direct_arrowhead")` | macro | [call:r1 call:range_ap call:range_he call:range_heat]x29 | 29 | 29 | 예: r1 ( 1.05 ) range_ap ( 160 ) range_he ( 160 ) range_heat ( 160 ) | 미확인(자동) |  |  |  |
| `/from/("accuracy_direct_auto")` | macro | [call:r1 call:range_ap call:range_he call:range_heat]x17 | 17 | 17 | r1 ( 0.8 ) range_ap ( 150 ) range_he ( 150 ) range_heat ( 150 ) ¦ r1 ( 0.55 ) range_ap ( 160 ) range_he ( 160 ) range_heat ( 160 ) ¦ r1 ( 0.7 ) range_ap ( 150 ) range_he ( 150 ) range_heat ( 150 ) ¦ r1 ( 0.80 ) range_ap ( 150 ) range_he ( 150 ) range_heat ( 150 ) ¦ r1 ( 0.58 ) range_ap ( 160 ) range_he ( 160 ) range_heat ( 160 ) ¦ r1 ( 0.50 ) range_ap ( 160 ) range_he ( 160 ) range_heat ( 160 ) | 미확인(자동) |  |  |  |
| `/from/("accuracy_direct")` | macro | [call:r1 call:range_ap call:range_he call:range_heat]x60 | 60 | 60 | 예: r1 ( 0.55 ) range_ap ( 220 ) range_he ( 220 ) range_heat ( 220 ) | 추정 | 직사 무기의 정확도. r1 정확도 반경, range_ap·range_he·range_heat 탄종별 거리 (추정: accuracy_indirect의 같은 이름 인자 주석) | 파일 구조와 값에서 추론 |  |
| `/from/("accuracy_hmg_tank")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_indirect")` | macro | [call:r1 call:r2 call:rmax call:range_ap call:range_heat call:range_he]x49 | 49 | 49 | 예: r1 ( 1.05 ) r2 ( 5.01 ) rmax ( 20.08 ) range_ap ( 190 ) range_heat ( 190 ) range_he ( 370 ) | 확인 | 200m 넘게 쏘는 곡사 무기의 정확도. r1 직사 탄 정확도 반경, r2 곡사 정확도 반경, rmax 최대 사거리의 곡사 반경, range_ap·range_heat·range_he 이 거리 뒤로 정확도가 떨어진다(먼 거리 사격을 막는다) | 게임 파일 주석 |  |
| `/from/("accuracy_rifle_auto")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_shotgun")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("accuracy_spg")` | macro | [call:r1 call:range_ap call:range_he call:range_heat]x21 | 20 | 21 | 예: r1 ( 1.00 ) range_ap ( 180 ) range_he ( 190 ) range_heat ( 180 ) | 미확인(자동) |  |  |  |
| `/from/("airborne_mg")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |  |
| `/from/("ap_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("armor health custom")` | macro | [call:shell call:damage call:velocity call:filler]x151 | 96 | 151 | 예: shell ( aphe ) damage ( 280 ) velocity ( 890 ) filler ( 88 ) | 미확인(자동) |  |  |  |
| `/from/("armor health")` | macro | [call:damage]x152 | 152 | 152 | 예: damage ( 280 ) | 미확인(자동) |  |  |  |
| `/from/("assault_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("auto_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("battle_rifle_fg42")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("battle_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("bolt_action")` | macro | []x15 | 15 | 15 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_aa_quad")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_aa")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_long")` | macro | []x26 | 26 | 26 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_medium")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_short_slow")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_short")` | macro | []x29 | 29 | 29 | 예: | 미확인(자동) |  |  |  |
| `/from/("burst_stand")` | macro | []x13 | 13 | 13 | 예: | 미확인(자동) |  |  |  |
| `/from/("carbine_rifle")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("direct_settings")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("enfield_no1_rate")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("enfield_no4_rate")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("filling_tank")` | macro | [call:fuel]x11 | 11 | 11 | fuel ( 10 ) ¦ fuel ( 200 ) ¦ fuel ( 510 ) ¦ fuel ( 20 ) ¦ fuel ( 437 ) | 미확인(자동) |  |  |  |
| `/from/("filling")` | macro | [call:fuel]x8 | 8 | 8 | fuel ( 100 ) ¦ fuel ( 70 ) ¦ fuel ( 90 ) | 미확인(자동) |  |  |  |
| `/from/("finnish_rate")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("fire rate")` | macro | [call:rpm]x136 | 136 | 136 | 예: rpm ( 1800 ) | 미확인(자동) |  |  |  |
| `/from/("flare_pistol")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("ger_ap_bullets")` | macro | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/("heavy_aa")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("heavy_artillery")` | macro | []x20 | 20 | 20 | 예: | 미확인(자동) |  |  |  |
| `/from/("human health")` | macro | [call:damage]x9 | 8 | 9 | damage ( 120 ) ¦ damage ( 250 ) ¦ damage ( 100 ) ¦ damage ( 70 ) ¦ damage ( 55 ) | 미확인(자동) |  |  |  |
| `/from/("k98_rate")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("light_aa")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/from/("marksman")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("medium_aa")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |  |
| `/from/("medium_artillery")` | macro | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |  |
| `/from/("mosin_rate")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/from/("penetration_long")` | macro | [call:shell call:range call:a call:b call:c call:d call:e]x46 | 22 | 46 | 예: shell ( aphe ) range ( 220 ) a ( 209 ) b ( 187 ) c ( 162 ) d ( 141 ) e ( 122 ) | 미확인(자동) |  |  |  |
| `/from/("penetration_medium")` | macro | [call:shell call:range call:a call:b call:c call:d call:e]x240 | 97 | 240 | 예: shell ( aphe ) range ( 180 ) a ( 73 ) b ( 67 ) c ( 62 ) d ( 59 ) e ( 53 ) | 미확인(자동) |  |  |  |
| `/from/("penetration_short")` | macro | [call:shell call:range call:a call:b call:c call:d call:e]x11 | 9 | 11 | shell ( ap ) range ( 150 ) a ( 45 ) b ( 33 ) c ( 23 ) d ( 15 ) e ( 11 ) ¦ shell ( apcr ) range ( 150 ) a ( 63 ) b ( 26 ) c ( 8 ) d ( 3 ) e ( 1 ) ¦ shell ( ap ) range ( 150 ) a ( 31 ) b ( 20 ) c ( 11 ) d ( 7 ) e ( 4 ) ¦ shell ( ap ) range ( 150 ) a ( 36 ) b ( 28 ) c ( 21 ) d ( 15 ) e ( 11 ) ¦ shell ( ap ) range ( 150 ) a ( 25 ) b ( 17 ) c ( 14 ) d ( 8 ) e ( 5 ) ¦ shell ( apbc ) range ( 150 ) a ( 44 ) b ( 35 ) c ( 26 ) d ( 19 ) e ( 15 ) ¦ shell ( heat ) range ( 110 ) a ( 180 ) b ( 180 ) c ( 180 ) d ( 180 ) e ( 180 ) ¦ shell ( heat ) range ( 120 ) a ( 180 ) b ( 180 ) c ( 180 ) d ( 180 ) e ( 180 ) | 미확인(자동) |  |  |  |
| `/from/("penetration")` | macro | [atom num num num num num]x10 [call:a call:b call:c call:d call:e]x5 [atom num num num]x3 [atom num]x2 | 20 | 20 | args 17.3 8 4 2 0 ¦ a ( 28 ) b ( 19 ) c ( 16 ) d ( 12 ) e ( 7 ) ¦ args 35 ¦ args 43 40 35 ¦ a ( 29 ) b ( 21 ) c ( 14 ) d ( 10 ) e ( 7 ) ¦ a ( 36 ) b ( 24 ) c ( 22 ) d ( 13 ) e ( 7 ) ¦ args 22 10 5 3 0 ¦ args 14 6.5 3.5 2 0 ¦ args 2 1 0.5 0 0 ¦ args 5 3 1 0.5 0 ¦ args 33 30 25 ¦ args 6 4 2 1 0 | 미확인(자동) |  |  |  |
| `/from/("range_160_howitzer")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_160")` | macro | []x17 | 17 | 17 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_170_howitzer")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_170")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_180_howitzer")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_180")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_190_howitzer")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_190")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_200_howitzer")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_200")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_210")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_220")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_aircraft_gun")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_autocannon_12-20")` | macro | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/("range_autocannon_37-40")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_shotgun")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("recoil_smg_low")` | macro | []x17 | 17 | 17 | 예: | 미확인(자동) |  |  |  |
| `/from/("reload time")` | macro | [call:seconds]x235 | 235 | 235 | 예: seconds ( 8 ) | 미확인(자동) |  |  |  |
| `/from/("reload_belt")` | macro | []x21 | 21 | 21 | 예: | 미확인(자동) |  |  |  |
| `/from/("rus_ap_bullets")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/from/("scopeview_2")` | macro | [call:scope]x6 | 6 | 6 | scope ( zoom_vignette_scope_eng_m1918 ) ¦ scope ( zoom_vignette_scope_ger_k98 ) ¦ scope ( zoom_vignette_scope_eng_no32 ) ¦ scope ( zoom_vignette_scope_rus_pe ) | 미확인(자동) |  |  |  |
| `/from/("scopeview")` | macro | [call:scope]x4 | 4 | 4 | scope ( zoom_vignette_scope_ger_k98 ) ¦ scope ( zoom_vignette_scope_rus_pe ) | 미확인(자동) |  |  |  |
| `/from/("semi_rifle")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/from/("spreading_high")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/("spreading_low")` | macro | []x39 | 39 | 39 | 예: | 미확인(자동) |  |  |  |
| `/from/("spreading_max")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/("spreading_medium")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |  |
| `/from/("spreading_mounted_low")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |  |
| `/from/("spreading_mounted")` | macro | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |  |
| `/from/("stick_reload")` | macro | []x26 | 26 | 26 | 예: | 미확인(자동) |  |  |  |
| `/from/("super_heavy_artillery")` | macro | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/("sway_rifle")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/("velocity m/s")` | macro | [call:ap call:apcr call:he call:heat]x133 | 133 | 133 | 예: ap ( 890 ) apcr ( 1 ) he ( 880 ) heat ( 1 ) | 미확인(자동) |  |  |  |
| `/from/("velocity")` | macro | [call:mps]x175 | 175 | 175 | 예: mps ( 220 ) | 미확인(자동) |  |  |  |
| `/from/("zeroing_boost")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/(mod)` | mod | [atom str]x1 | 1 | 1 | not "mp" | 미확인(자동) |  |  |  |
| `/from/accuracybymotionk` | block | [num]x3 | 3 | 3 | 0.5 ~ 0.9 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/from/aimingtolerance` | block | [num]x4 | 4 | 4 | 0.8 ~ 10 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/from/aimrange` | block | [num]x1 | 1 | 1 | 30 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/from/aimtype` | block | [atom]x1 | 1 | 1 | curved | 미확인(자동) |  |  |  |
| `/from/automatic` | block | []x1 | 1 | 1 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/from/burst` | block | []x9 | 9 | 9 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 |  |
| `/from/burst/long` | block | [num num]x8 | 8 | 8 | 20 1 ¦ 40 1 ¦ 6 2 ¦ 21 3 ¦ 6 0.75 ¦ 1 0.75 | 확인 | 긴 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/from/burst/min` | block | [num]x1 | 1 | 1 | 2 | 미확인(자동) |  |  |  |
| `/from/burst/rangekoef` | block | [num]x8 | 8 | 8 | 0.25 ~ 0.85 | 확인 | rangeKoef x aimRange = 긴 점사를 쓰는 최대 거리 | 게임 파일 주석 | rangeKoef * aimRange = max long burst distance |
| `/from/burst/short` | block | [num num]x8 | 8 | 8 | 10 0.75 ¦ 15 0.75 ¦ 1 0.1 ¦ 3 1 ¦ 6 3 | 확인 | 짧은 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/from/calibre` | block | [num]x308 | 308 | 308 | 5.6 ~ 600 | 확인 | 구경, mm | 게임 파일 주석 | caliber of gun in millimeters ¦ AP shot caliber |
| `/from/cursor` | block | [str]x27 | 27 | 27 | "ironsights/aa" ¦ "ironsights/rifle" ¦ "ironsights/grenade" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/from/cursorreloading` | block | [str]x9 | 9 | 9 | "gun_ap" ¦ "carbine_reload" ¦ "gun_he" ¦ "gun_apcr" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/curve` | block | [str]x4 | 4 | 4 | "shotgun" ¦ "bullet_smg" ¦ "bullet_mg" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/from/disablericochet` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/entity` | block | [str]x1760 | 1760 | 1760 | 예: "compositionb_x4" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/from/falloffmeter` | block | [num]x1 | 1 | 1 | 100 | 미확인 | 값 1~220 |  |  |
| `/from/falloffstrength` | block | [num]x1 | 1 | 1 | 0.15 | 미확인 | 값 0~0.15 |  |  |
| `/from/filling` | block | [str num]x207 [str]x135 | 342 | 342 | 예: "bulletrus_100" | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/from/fillingvisible` | block | [str]x8 | 8 | 8 | "grenade" ¦ "shell" ¦ "vpgs41" | 미확인(자동) |  |  |  |
| `/from/firesound` | block | [str]x325 | 324 | 325 | 예: "cannon/s4/d1/Shared/Cannon_85to100mm_HV/" | 추정 | 발사음 경로 (추정: 이름과 값) | 파일 구조와 값에서 추론 | Placeholder |
| `/from/firesoundclose` | block | [str]x346 | 346 | 346 | 예: "cannon/s4/3p/Shared/Cannon_85to100mm_HV/" | 추정 | 가까이서 들리는 발사음 경로 (추정: 이름과 값의 3p) | 파일 구조와 값에서 추론 | Placeholder |
| `/from/firingtimeout` | block | [num num]x3 | 3 | 3 | 2.5 0.5 ¦ 2.75 0.5 ¦ 1.8 0.4 | 확인 | 연사 사이의 지연 | 게임 파일 주석 | 3.5 0.5 ¦ delay between bursts |
| `/from/firingtimeoutdistancefactor` | block | []x2 | 2 | 2 | 예: | 미확인 | 하위에 두 수의 행 |  |  |
| `/from/firingtimeoutdistancefactor/#row` | block | [num num]x4 | 2 | 4 | 0.25 0.5 ¦ 1.0 1.0 ¦ 0.3 0.6 ¦ 0.8 1.0 | 미확인 | 두 수 |  |  |
| `/from/foresight` | block | [str]x14 | 14 | 14 | "foresight1" ¦ "foresight4" ¦ "foresight2" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/from/gravity` | block | [num]x3 | 3 | 3 | 0.75 ~ 1.5 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/from/inventory` | block | []x49 | 49 | 49 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/fsm` | block | [str]x5 | 5 | 5 | "belt" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/from/inventory/link_bone` | block | [str]x2 | 2 | 2 | "gun_back" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/from/inventory/size` | block | [num num]x49 | 49 | 49 | 7 2 ¦ 4 6 ¦ 3 1 ¦ 6 2 ¦ 3 3 ¦ 4 2 ¦ 5 2 ¦ 7 3 ¦ 2 2 ¦ 4 5 ¦ 6 3 ¦ 2 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/from/inventory/weight` | block | [num]x7 | 7 | 7 | 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/from/mass` | block | [num]x187 | 187 | 187 | 0.765 ~ 224 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/from/maxrange` | block | [num]x1 | 1 | 1 | 40 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/from/minrange` | block | [num]x2 | 2 | 2 | 5 ~ 150 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/parameters` | block | [str]x151 | 97 | 151 | 예: "aphebc" | 추정 | 탄종별 설정. 문자열이 탄종 이름 (추정: 값 "heat", "clip") | 파일 구조와 값에서 추론 | SPgGr. ¦ Sprgr. |
| `/from/parameters/("ap_155_slope_multiplier")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/("ap_57_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/("ap_slope_multiplier")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/("apbc_90_t33_slope_multiplier")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/("armor health")` | macro | [call:damage]x13 | 10 | 13 | damage ( 150 ) ¦ damage ( 5 ) ¦ damage ( 15 ) | 미확인(자동) |  |  |  |
| `/from/parameters/("global velocity")` | macro | [call:gv]x1 | 1 | 1 | gv ( 915 ) | 미확인(자동) |  |  |  |
| `/from/parameters/("human health")` | macro | [call:damage]x1 | 1 | 1 | damage ( 250 ) | 미확인(자동) |  |  |  |
| `/from/parameters/("large_apcr_slope_multiplier")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/("penetration")` | macro | [atom num]x8 [atom num num num num num]x4 [call:a call:b call:c call:d call:e]x2 | 11 | 14 | args 17.3 8 4 2 0 ¦ args 52 ¦ args 51 ¦ args 76 ¦ a ( 28 ) b ( 19 ) c ( 16 ) d ( 12 ) e ( 7 ) ¦ a ( 58 ) b ( 40 ) c ( 35 ) d ( 25 ) e ( 15 ) ¦ args 35 ¦ args 70 | 미확인(자동) |  |  |  |
| `/from/parameters/("reload time")` | macro | [call:seconds]x12 | 12 | 12 | seconds ( 10 ) ¦ seconds ( 30 ) ¦ seconds ( 20 ) | 미확인(자동) |  |  |  |
| `/from/parameters/("sap_130_slope_multiplier")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/("velocity")` | macro | [call:mps]x55 | 23 | 55 | 예: mps ( 213 ) | 미확인(자동) |  |  |  |
| `/from/parameters/(mod)` | mod | [str]x20 | 20 | 20 | "mp" | 미확인(자동) |  |  |  |
| `/from/parameters/cursor` | block | [str]x7 | 5 | 7 | "ironsights/grenade" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/from/parameters/cursorreloading` | block | [str]x19 | 7 | 19 | "gun_heat" ¦ "gun_he" ¦ "gun_wp" ¦ "gun_saphe" ¦ "gun_apcnr" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/parameters/falloffstrength` | block | [num]x18 | 18 | 18 | 0 | 미확인 | 값 0~0.15 |  |  |
| `/from/parameters/gravity` | block | [num]x6 | 6 | 6 | 0.08 ~ 3 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/from/parameters/minimumdamagemodifier` | block | [num]x2 | 2 | 2 | 450 | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/from/parameters/projectiledamagetable` | block | []x3 | 2 | 3 | 예: | 추정 | 거리별 관통력 표 (추정: #row가 두 수의 쌍이고 주석이 탄종 이름) | 파일 구조와 값에서 추론 | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/from/parameters/projectiledamagetable/#row` | block | [num num]x15 | 2 | 15 | 예: 20 24 | 추정 | 거리와 관통력 한 쌍 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/from/parameters/range` | block | [num num]x43 | 31 | 43 | 예: 220 220 | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/from/parameters/rechargetime` | block | [num]x2 | 2 | 2 | 8 ~ 10 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/from/parameters/reloadsound` | block | [str]x1 | 1 | 1 | "reload_clip_x2" | 확인 | 장전음 | 게임 파일 주석 | sound for weapon reload |
| `/from/parameters/speed` | block | [num]x4 | 4 | 4 | 1 ~ 100 | 확인 | 탄속. "velocity" 매크로가 0.35 x m/s로 계산해 넣는다 | 게임 파일 주석 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/parameters/spreading` | block | [num]x9 []x6 | 11 | 15 | 4.5 | 추정 | 산포 설정 묶음 또는 산포 값 (추정: 하위 radiustable) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/parameters/spreading/radiustable` | block | []x6 | 6 | 6 | 예: | 확인 | 거리별 산포 반경 표. 최대 사거리를 넘으면 값이 급격히 커지게 해 원거리 사격을 막는다 | 게임 파일 주석 | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/from/parameters/spreading/radiustable/#row` | block | [num num]x31 | 6 | 31 | 예: 0 0 | 추정 | 거리와 산포 반경 한 쌍 | 파일 구조와 값에서 추론 |  |
| `/from/parameters/spreading/spreadxyratio` | block | [num]x9 | 5 | 9 | 0.75 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/from/parameters/unlimitedrangetpc` | block | [num]x20 | 20 | 20 | 1 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/from/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/from/projectiledamagetable` | block | []x6 | 6 | 6 | 예: | 추정 | 거리별 관통력 표 (추정: #row가 두 수의 쌍이고 주석이 탄종 이름) | 파일 구조와 값에서 추론 | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/from/projectiledamagetable/#row` | block | [num num]x30 | 6 | 30 | 예: 0 35 | 추정 | 거리와 관통력 한 쌍 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/from/range` | block | [num num]x81 | 81 | 81 | 예: 60 60 | 추정 | 사거리 두 값 (추정: 원문 주석이 "200m", "285m max". 두 값의 구분은 미확인) | 게임 파일 주석 | 200m ¦ 285m max |
| `/from/rechargetime` | block | [num]x11 | 11 | 11 | 0.05 ~ 30 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/from/recoverytime` | block | [num]x1 | 1 | 1 | 0.05 | 확인 | 발사 간격, 초. "fire rate" 매크로가 60 / rpm으로 계산해 넣는다 | 게임 파일 주석 | increase fuel consumption by 2x |
| `/from/relaxationtime` | block | [num]x2 | 2 | 2 | 0.25 | 미확인 | 값 0.25~2 |  |  |
| `/from/reloadanimbonus` | block | [num]x2 | 2 | 2 | 57 | 미확인(자동) |  |  |  |
| `/from/reloadsound` | block | [str]x348 | 346 | 348 | 예: "utility/" | 확인 | 장전음 | 게임 파일 주석 | sound for weapon reload |
| `/from/special` | block | []x3 | 3 | 3 | 예: | 미확인 | 하위에 mode, preparetime, preparecursor |  | {throw_off_always} |
| `/from/special/accuracy` | block | [num]x1 | 1 | 1 | 3 | 미확인(자동) |  |  |  |
| `/from/special/damage` | block | [num]x1 | 1 | 1 | 5 | 미확인 | 값 5. 특수 사격(special) 설정의 하위 |  | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/special/mode` | block | [atom]x1 | 1 | 1 | aim | 미확인 | 값 detonate |  |  |
| `/from/special/preparecursor` | block | [str]x1 | 1 | 1 | "aimed_shot" | 추정 | 준비 중 커서 (추정: 이름과 값 "grenade") | 파일 구조와 값에서 추론 |  |
| `/from/special/preparecursorscene` | block | [str]x1 | 1 | 1 | "aimed_shot_scene" | 미확인(자동) |  |  |  |
| `/from/special/preparetime` | block | [num]x1 | 1 | 1 | 3.5 | 추정 | 사용 전 준비 시간 (추정: 이름과 값 2~8) | 파일 구조와 값에서 추론 |  |
| `/from/speed` | block | [num]x12 | 12 | 12 | 40 ~ 1000 | 확인 | 탄속. "velocity" 매크로가 0.35 x m/s로 계산해 넣는다 | 게임 파일 주석 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/spreading` | block | []x7 [num]x1 | 8 | 8 | 4.5 | 추정 | 산포 설정 묶음 또는 산포 값 (추정: 하위 radiustable) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/spreading/burstaccuracy` | block | [num num num num num num num num num]x3 [num]x3 | 6 | 6 | 100 | 확인 | 연사 중 발마다의 정확도. 기본 100이고 첫 발이 100% | 게임 파일 주석 | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/from/spreading/radiustable` | block | []x1 | 1 | 1 | 예: | 확인 | 거리별 산포 반경 표. 최대 사거리를 넘으면 값이 급격히 커지게 해 원거리 사격을 막는다 | 게임 파일 주석 | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/from/spreading/radiustable/#row` | block | [num num]x4 | 1 | 4 | 0 0 ¦ 50 1.5 ¦ 99.99 2.75 ¦ 100 20 | 추정 | 거리와 산포 반경 한 쌍 (추정: .presets의 표가 {100 10} {200 20} 형태) | 파일 구조와 값에서 추론 |  |
| `/from/spreading/spreadxyratio` | block | [num]x1 | 1 | 1 | 0.75 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/from/spreadtolerance` | block | [num]x4 | 4 | 4 | 0.15 ~ 0.5 | 확인 | 사격 전에 필요한 조준 수렴 정도 | 게임 파일 주석 | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/from/states` | block | []x2 | 2 | 2 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/from/states/aim` | block | [str]x2 | 2 | 2 | "aim" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/can_aim` | block | [str]x2 | 2 | 2 | "can-aim-bolt" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/fire` | block | [str]x2 | 2 | 2 | "fire" | 추정 | 발사 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/reload` | block | [str]x2 | 2 | 2 | "reload" | 추정 | 장전 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/unload` | block | [str]x2 | 2 | 2 | "unload" | 추정 | 탄 빼기 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/syncedprojectiles` | block | [num]x1 | 1 | 1 | 1 | 확인 | 멀티플레이에서 탄흔·탄공 생성을 시각적으로 동기화한다. 자동화기에는 쓰지 않는다 | 게임 파일 주석 | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/from/tag` | block | [str]x1211 [str str]x7 [str str str str]x1 | 1219 | 1219 | 예: "gun" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from/tracefreq` | block | [num]x10 | 10 | 10 | 0 ~ 5 | 추정 | 예광탄 빈도 (추정: 이름과 값 3~5) | 파일 구조와 값에서 추론 |  |
| `/from/unlimitedrangetpc` | block | [num]x9 | 9 | 9 | 0 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/from/zeroingaccuracy` | block | [num num num num]x1 | 1 | 1 | 0.9 0.8 0.7 0.65 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/item` | block | []x64 | 64 | 64 | 예: | 추정 | 아이템 정의의 최상위 블록 (추정: headgear.pattern 등이 이 블록으로 시작) | 파일 구조와 값에서 추론 | Jerry Can (20L) ¦ 55 gallon (~210L) drum |
| `/item/damage` | block | [atom]x1 [atom atom]x1 | 1 | 2 | blastwave ¦ add blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/item/damage/area` | block | [num num]x2 | 1 | 2 | 3 8 ¦ 8 12 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/item/damage/energy` | block | [num]x2 | 1 | 2 | 1.7 ~ 15 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/item/damage/ground_interaction_radius` | block | [num]x2 | 1 | 2 | 0 ~ 0.15 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/item/desc` | block | [str]x3 | 3 | 3 | "desc/stuff/diesel" ¦ "desc/stuff/petrol" ¦ "Stuff not found" | 미확인(자동) |  |  |  |
| `/item/entity` | block | [str]x57 | 57 | 57 | 예: "eng_radio" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/item/filling` | block | [str num]x3 | 3 | 3 | "liquid" 210 ¦ "liquid" 20 ¦ "drug" 100 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/item/inventory` | block | []x65 | 64 | 65 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/item/inventory/block` | block | [num]x3 | 3 | 3 | 5 | 미확인(자동) |  |  |  |
| `/item/inventory/fsm` | block | [str]x57 | 57 | 57 | "stuff" ¦ "belt" ¦ "hand-only" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/item/inventory/link_bone` | block | [str]x15 | 15 | 15 | "backpacks" ¦ "head" ¦ "IK_UpDown" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/item/inventory/noview` | block | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/item/inventory/size` | block | [num num]x51 | 51 | 51 | 예: 3 4 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/item/inventory/weight` | block | [num]x57 | 56 | 57 | 1 ~ 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/item/mass` | block | [num]x62 | 62 | 62 | 0.05 ~ 20 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/item/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/item/tag` | block | [str]x59 [str str]x1 | 60 | 60 | 예: "itemin1hand" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/weapon_bombing` | block | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bombing/("plane_bomb_weapon")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bombing/burst` | block | []x1 | 1 | 1 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 |  |
| `/weapon_bombing/burst/min` | block | [num]x1 | 1 | 1 | 2 | 미확인(자동) |  |  |  |
| `/weapon_bombing/filling` | block | [str num]x8 | 8 | 8 | "ammo air_bomb_eng_250lb" 2 ¦ "ammo air_bomb_eng_500lb" 2 ¦ "ammo air_bomb_ger_250kg" 1 ¦ "ammo air_bomb_ger_250kg" 2 ¦ "air_bomb_ger_50kg" 4 ¦ "air_bomb_100kg" 4 ¦ "ammo air_bomb_rus_250kg" 2 ¦ "ammo air_bomb_usa_500lb" 3 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_bombing/fillingvisible` | block | [str]x6 | 6 | 6 | "bomb_" ¦ "bomb" | 미확인(자동) |  |  |  |
| `/weapon_bullet` | block | []x4 | 4 | 4 | 예: | 추정 | 탄을 쏘는 무기 정의의 최상위 블록 (추정: 패턴 파일의 구조) | 파일 구조와 값에서 추론 | Example / Default Settings ¦ required for AT-Grenades with cumaltive damage |
| `/weapon_bullet/("binocularsview")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams` | block | [atom str]x1 | 1 | 1 | inherit "human default" | 미확인 | 값 inherit "human default" |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level` | block | [num]x1 []x1 | 1 | 2 | 1 | 추정 | 3인칭 조준의 확대 단계 하나. 값은 단계 번호 (추정: 값 1, 2와 하위 camera) | 파일 구조와 값에서 추론 | 2 |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/aimer_type` | block | [str]x1 | 1 | 1 | "sniper" | 추정 | 조준 방식 이름 (추정: 값 "sniper") | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/blur_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/zoom_vignette_binoculars_mask" | 추정 | 가장자리 흐림 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/blur_strength` | block | [num]x1 | 1 | 1 | 10 | 추정 | 가장자리 흐림의 세기 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/camera` | block | [str]x2 | 1 | 2 | "human_zoom4" ¦ "human_zoom8" | 확인 | 이 단계에서 쓰는 카메라 프리셋. 배율을 정한다. human_zoom4는 4배 | 게임 파일 주석 | magnification x4 ¦ magnification x8 |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/delta` | block | [num]x1 | 1 | 1 | 20 | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/distance` | block | [num]x1 | 1 | 1 | 35 | 미확인(자동) |  |  | activation distance |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/distanceset` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/drop_zoom_on_reload` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/hide_crosshair` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/hide_weapon` | block | [num]x1 | 1 | 1 | 1 | 추정 | 조준 중에 무기 모델을 숨길지 여부 (추정: 이름과 값 1) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/lens_dirt_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/dirty_lens_scope_01" | 추정 | 렌즈 얼룩 효과에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/linkbone` | block | [str]x1 | 1 | 1 | "head" | 추정 | 조준 카메라가 붙는 뼈대 (추정: 값 "head") | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/manualaccuracyzoom` | block | []x1 | 1 | 1 | 예: | 추정 | 직접 조종에서 확대했을 때 정확도 배율 (추정: 이름. 값 1.25) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/mouse_speed_factor` | block | [num]x2 | 1 | 2 | 0.6 ~ 0.8 | 추정 | 조준 중 마우스 감도 배수 (추정: 이름과 값 0.6 ~ 0.8) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/offset` | block | [num num num]x1 | 1 | 1 | 35 0 2.25 | 추정 | 표시 위치 오프셋 세 값 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/scope_vignette` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/zoom_vignette_binoculars" | 추정 | 조준경 테두리로 쓰는 텍스처 (추정: 값이 lens_effects 폴더의 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/vignette_fade_speed` | block | [num]x1 | 1 | 1 | 5 | 확인 | 조준경 테두리(비네트)가 나타나고 사라지는 속도 계수 | 게임 파일 주석 | factoring the speed at which the vignette fades in/out |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/vignette_gloss` | block | [num]x1 | 1 | 1 | 10 | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/vignette_recoil` | block | [num num]x1 | 1 | 1 | 2.75 1.1 | 확인 | 반동 때 조준경 테두리가 흔들리는 속도와 크기 | 게임 파일 주석 | speed, scale |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/vision_modes_active` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/weapon_sway_active` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/with_owner` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/with_owner/viewoff` | block | [str]x1 | 1 | 1 | "skin" | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/with_weapon` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/with_weapon/viewon` | block | [str]x1 | 1 | 1 | "foresight3" | 미확인(자동) |  |  |  |
| `/weapon_bullet/(define "binocularsview")/thirdpersonparams/level/zoom` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/aimingtolerance` | block | [num]x3 | 3 | 3 | 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/weapon_bullet/calibre` | block | [num]x1 | 1 | 1 | 120 | 확인 | 구경, mm | 게임 파일 주석 | caliber of gun in millimeters ¦ AP shot caliber |
| `/weapon_bullet/crosshair` | block | [str]x1 | 1 | 1 | "crosshair_mgun" | 확인 | 3인칭 조준 때의 조준선 이름 | 게임 파일 주석 | third person crosshair |
| `/weapon_bullet/crosshairreloading` | block | [str]x1 | 1 | 1 | "crosshair_mgun_reload" | 확인 | 3인칭 조준에서 재장전 중일 때의 조준선 이름 | 게임 파일 주석 | third person reload crosshair. |
| `/weapon_bullet/cursor` | block | [str]x2 | 2 | 2 | "ironsights/rifle" ¦ "ironsights/binoculars" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/weapon_bullet/curve` | block | [str]x1 | 1 | 1 | "gun_std" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/weapon_bullet/entity` | block | [str]x1 | 1 | 1 | "binocular" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/weapon_bullet/filling` | block | [str num]x2 [str]x1 | 3 | 3 | "light" 0 ¦ "dummy ap" | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_bullet/foresight` | block | [str]x3 | 3 | 3 | "foresight1" ¦ "Foresight3" ¦ "vision_rot" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/weapon_bullet/gravity` | block | [num]x2 | 2 | 2 | 0 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/weapon_bullet/inventory` | block | []x2 | 2 | 2 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/inventory/fsm` | block | [str]x1 | 1 | 1 | "belt" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/weapon_bullet/inventory/noview` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/inventory/size` | block | [num num]x1 | 1 | 1 | 2 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/weapon_bullet/inventory/weight` | block | [num]x1 | 1 | 1 | 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/weapon_bullet/mass` | block | [num]x2 | 2 | 2 | 0 ~ 1.5 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/weapon_bullet/range` | block | [num num]x2 | 2 | 2 | 200 200 | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/weapon_bullet/speed` | block | [num]x2 | 2 | 2 | 1000 | 추정 | 탄속 (추정: 위와 같은 키) | 파일 구조와 값에서 추론 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/weapon_bullet/states` | block | []x1 | 1 | 1 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/aim` | block | [str]x1 | 1 | 1 | "aim" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/can_aim` | block | [str]x1 | 1 | 1 | "can-aim" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/tag` | block | [str]x2 | 2 | 2 | "weapon smallitemin1hand field_glass visor distributable" ¦ "visor" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |

## 펼친 뒤의 키

include와 매크로를 펼친 뒤 유닛 안에서의 위치다. 펼치는 규칙은 추정이며 한계는 coverage.md에 있다. 고유 키 509개.

### 유닛 종류: pattern

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/bullet` | block | []x8 | 8 | 8 | 예: | 추정 | 탄 정의의 최상위 블록 (추정: bullet.pattern) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/bullet/cumulative` | block | [num]x3 | 3 | 3 | 0.7 ~ 3 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/bullet/damage` | block | [atom]x3 | 3 | 3 | blastwave | 추정 | 피해 정의. 값은 피해 종류 | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/bullet/damage/area` | block | [num num]x3 | 3 | 3 | 2 3.5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/bullet/damage/energy` | block | [num]x3 | 3 | 3 | 2.5 ~ 3 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/bullet/damage/ground_interaction_radius` | block | [num]x3 | 3 | 3 | 0.1 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/bullet/detonationthickness` | block | [num]x2 | 2 | 2 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/bullet/inventory` | block | []x7 | 7 | 7 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/bullet/inventory/fsm` | block | [str]x7 | 7 | 7 | "belt" ¦ "stuff" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/bullet/inventory/size` | block | [num num]x6 | 6 | 6 | 3 1 ¦ 5 1 ¦ 4 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/bullet/inventory/weight` | block | [num]x7 | 7 | 7 | 100 ~ 1000 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/bullet/mass` | block | [num]x1 | 1 | 1 | 2.75 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/bullet/possibledamageradius` | block | [num]x4 | 4 | 4 | 3 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/bullet/tag` | block | [str]x8 | 8 | 8 | "ammo rocketlauncher heat_shell heat dropboxed pierce no_ricochet" ¦ "ammo shell all_he fg dropboxed no_ricochet" ¦ "ammo rocketlauncher heat_shell dropboxed pierce no_ricochet" ¦ "ammo shell fg wp smoke wp_size1 dropboxed no_ricochet" ¦ "bomb fg itemin2hands build_in" ¦ "dropboxed distributable" ¦ "rocketselector rocketlauncher pierce no_ricochet" ¦ "ammo itemin2hands" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from` | block | [str]x43 | 43 | 43 | "pattern generic" ¦ "pattern throwable" ¦ "pattern bullet" ¦ "pattern rifle" ¦ "pattern mgun_generic" ¦ "pattern he" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/(mod)` | mod | [str]x1 | 1 | 1 | "mp" | 미확인(자동) |  |  |  |
| `/from/accuracybymotionk` | block | [num]x3 | 3 | 3 | 0.1 ~ 0.9 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/from/aimingtolerance` | block | [num]x6 | 6 | 6 | 1 ~ 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/from/automatic` | block | []x3 | 3 | 3 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/from/burst` | block | []x2 | 2 | 2 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 | multiplier to number of bullets in burst from weapon settings |
| `/from/burst/long` | block | [num num]x2 | 2 | 2 | 6 2 | 확인 | 긴 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/from/burst/rangekoef` | block | [num]x2 | 2 | 2 | 0.5 | 확인 | rangeKoef x aimRange = 긴 점사를 쓰는 최대 거리 | 게임 파일 주석 | rangeKoef * aimRange = max long burst distance |
| `/from/burst/short` | block | [num num]x2 | 2 | 2 | 4 1 | 확인 | 짧은 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/from/cumulative` | block | [num]x4 | 4 | 4 | 3 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/from/cursor` | block | [str]x8 | 8 | 8 | "ironsights/mg" ¦ "ironsights/smg" ¦ "ironsights/aa" ¦ "ironsights/bazooka" ¦ "ironsights/rifle" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/from/cursorreloading` | block | [str]x5 | 5 | 5 | "gun_ap" ¦ "carbine_reload" ¦ "gun_heat" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/curve` | block | [str]x8 | 8 | 8 | "bullet_sniper" ¦ "bullet_hmg" ¦ "bullet_mg" ¦ "bullet_pistol" ¦ "gun_std" ¦ "bullet" ¦ "bullet_smg" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/from/damage` | block | [atom]x3 [atom atom]x1 | 3 | 4 | blastwave ¦ add blastwave | 추정 | 피해 정의. 값은 피해 종류: blastwave, fire. add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/area` | block | [num num]x4 | 3 | 4 | 3 8 ¦ 8 12 ¦ 3 6 ¦ 3 5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/damage/energy` | block | [num]x4 | 3 | 4 | 1.1 ~ 15 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/damage/ground_interaction_radius` | block | [num]x4 | 3 | 4 | 0 ~ 0.15 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/detonationthickness` | block | [num]x4 | 4 | 4 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/from/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/firingtimeout` | block | [num num]x8 | 8 | 8 | 3.5 0.5 ¦ 2.0 0.5 ¦ 1.5 0.5 ¦ 1.75 0.5 ¦ 1.0 0.5 ¦ 2.5 0.5 | 확인 | 연사 사이의 지연 | 게임 파일 주석 | 3.5 0.5 ¦ delay between bursts |
| `/from/firingtimeoutdistancefactor` | block | []x6 | 6 | 6 | 예: | 미확인 | 하위에 두 수의 행 |  |  |
| `/from/firingtimeoutdistancefactor/#row` | block | [num num]x16 | 6 | 16 | 0.8 1.0 ¦ 1.0 1.0 ¦ 0.3 0.5 ¦ 0.25 0.0 ¦ 0.75 1.0 ¦ 0.25 0.6 ¦ 0.8 0.85 ¦ 0.2 0.55 ¦ 0.4 0.75 ¦ 0.2 0.5 ¦ 0.4 0.8 ¦ 0.6 0.9 | 미확인 | 두 수 |  |  |
| `/from/foresight` | block | [str]x7 | 7 | 7 | "foresight14" ¦ "foresight12" ¦ "foresight3" ¦ "foresight18" ¦ "Foresight11" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/from/gravity` | block | [num]x1 | 1 | 1 | 2 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/from/healthdamage` | block | [num]x8 | 8 | 8 | 55 ~ 350 | 문서 | 사람과 동물에게 주는 체력 피해. 관통력과 별개의 값이며 단위는 임의다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/from/inventory` | block | []x11 | 11 | 11 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/fsm` | block | [str]x1 | 1 | 1 | "pistol" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/from/inventory/link_bone` | block | [str]x5 | 5 | 5 | "gun_back" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/from/inventory/size` | block | [num num]x11 | 11 | 11 | 3 1 ¦ 7 2 ¦ 3 2 ¦ 2 1 ¦ 8 2 ¦ 6 2 ¦ 5 2 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/from/inventory/weight` | block | [num]x7 | 7 | 7 | 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/from/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/from/mass` | block | [num]x5 | 5 | 5 | 0.3 ~ 3 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/from/minimumdamagemodifier` | block | [num]x6 | 6 | 6 | 1 ~ 45 | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/from/overmatchcalibre` | block | []x7 | 7 | 7 | 예: | 미확인 | 하위에 세 수의 행. 구경 대 장갑 두께 보정으로 보이나 확인하지 못함 |  |  |
| `/from/overmatchcalibre/#row` | block | [num num num]x105 | 7 | 105 | 예: 0 1 0.00 | 미확인 | 세 수 |  |  |
| `/from/overmatchslope` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/overmatchslope/#row` | block | [num num]x2 | 1 | 2 | 0 1 ¦ 90 1 | 미확인(자동) |  |  |  |
| `/from/parameters` | block | [str]x1 | 1 | 1 | "api" | 추정 | 탄종별 설정. 문자열이 탄종 이름 (추정: 값 "heat", "clip") | 파일 구조와 값에서 추론 | SPgGr. ¦ Sprgr. |
| `/from/parameters/cursorreloading` | block | [str]x1 | 1 | 1 | "gun_api" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/possibledamageradius` | block | [num]x5 | 5 | 5 | 0.1 ~ 1 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/from/projectiledamagetable` | block | []x7 | 7 | 7 | 예: | 추정 | 거리별 관통력 표 (추정: #row가 두 수의 쌍이고 주석이 탄종 이름) | 파일 구조와 값에서 추론 | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/from/projectiledamagetable/#row` | block | [num num]x25 | 7 | 25 | 20 7 ¦ 100 5 ¦ 155 3 ¦ 250 1 ¦ 400 0 ¦ 30 6.5 ¦ 110 3.5 ¦ 250 0 ¦ 25 3 ¦ 150 0 ¦ 25 4 ¦ 100 0 | 추정 | 거리와 관통력 한 쌍 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/from/range` | block | [num num]x7 | 7 | 7 | 120 120 ¦ 140 140 ¦ 70 70 ¦ 130 130 ¦ 80 80 | 추정 | 사거리 두 값 (추정: 원문 주석이 "200m", "285m max". 두 값의 구분은 미확인) | 게임 파일 주석 | 200m ¦ 285m max |
| `/from/rechargetime` | block | [num]x8 | 8 | 8 | 3.25 ~ 10 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/from/recoil` | block | []x8 | 8 | 8 | 예: | 확인 | 반동 설정 묶음 | 게임 파일 주석 | weapon recoil, the higher the weaker ¦ recoil multiplier for all machine guns |
| `/from/recoil/maxangle` | block | [num num]x8 | 8 | 8 | 8 8 | 확인 | 최대 반동 각도(상하/좌우), 도 | 게임 파일 주석 | max recoil angle (pitch/yaw), degree |
| `/from/recoil/restoredelay` | block | [num]x8 | 8 | 8 | 0.2 | 확인 | 반동 회복이 시작되기 전의 지연, 초 | 게임 파일 주석 | delay before restore, sec |
| `/from/recoil/restoretime` | block | [num num]x8 | 8 | 8 | 0.25 0.005 ¦ 1.10 0.025 ¦ 1.00 0.025 ¦ 0.15 0.005 | 확인 | 반동 회복 시간. 첫 값이 최소 시간, 둘째 값이 추가 시간 | 게임 파일 주석 | recoil restore time, first -> minimum time, second -> additional time per shot, sec |
| `/from/recoil/shoot` | block | []x8 | 8 | 8 | 예: | 확인 | 발당 반동. 상하와 좌우 각각 값과 무작위 폭, 도 | 게임 파일 주석 | pitch / +- random and yaw / +- random, per shot, degree |
| `/from/recoil/shoot/#row` | block | [num num]x16 | 8 | 16 | 예: 0.50 0.25 | 추정 | 반동 값과 무작위 폭 (추정: 위 주석) | 게임 파일 주석 |  |
| `/from/recoverytime` | block | [num]x4 | 4 | 4 | 0.186915887850467 ~ 2.33644859813084 | 확인 | 발사 간격, 초. "fire rate" 매크로가 60 / rpm으로 계산해 넣는다 | 게임 파일 주석 | increase fuel consumption by 2x |
| `/from/relaxationtime` | block | [num]x5 | 5 | 5 | 0.25 ~ 2 | 미확인 | 값 0.25~2 |  |  |
| `/from/reloadanimbonus` | block | [num]x1 | 1 | 1 | 4 | 미확인(자동) |  |  |  |
| `/from/special` | block | []x6 | 6 | 6 | 예: | 미확인 | 하위에 mode, preparetime, preparecursor |  | {throw_off_always} |
| `/from/special/accuracy` | block | [num]x2 | 2 | 2 | 1.5 ~ 2.25 | 미확인(자동) |  |  | weapon accuracy, the higher the better ¦ accuracy multiplier from weapon settings |
| `/from/special/damage` | block | [num]x2 | 2 | 2 | 2 ~ 5 | 미확인 | 값 5. 특수 사격(special) 설정의 하위 |  | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/special/mode` | block | [atom]x4 | 4 | 4 | detonate ¦ aim | 미확인 | 값 detonate |  |  |
| `/from/special/preparecursor` | block | [str]x5 | 5 | 5 | "grenade" ¦ "aimed_shot" ¦ "grenade_sm" | 추정 | 준비 중 커서 (추정: 이름과 값 "grenade") | 파일 구조와 값에서 추론 |  |
| `/from/special/preparecursorscene` | block | [str]x2 | 2 | 2 | "aimed_shot_scene" | 미확인(자동) |  |  |  |
| `/from/special/preparetime` | block | [num]x4 | 4 | 4 | 2 ~ 8 | 추정 | 사용 전 준비 시간 (추정: 이름과 값 2~8) | 파일 구조와 값에서 추론 |  |
| `/from/special/throw_off` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/from/speed` | block | [num]x1 | 1 | 1 | 55 | 확인 | 탄속. "velocity" 매크로가 0.35 x m/s로 계산해 넣는다 | 게임 파일 주석 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/spreading` | block | []x16 [num]x1 | 9 | 17 | 2 | 추정 | 산포 설정 묶음 또는 산포 값 (추정: 하위 radiustable) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/spreading/burstaccuracy` | block | [num num num num num num num num num num num num num num]x2 [num num num num num]x2 [num num num num num num]x1 [num num num num num num num num num]x1 | 6 | 6 | 100 95 90 84 78 70 64 60 55 50 45 40 35 30 ¦ 100 95 90 85 80 ¦ 100 90 80 70 60 ¦ 100 90 80 70 60 50 ¦ 100 95 90 85 80 75 70 65 60 | 확인 | 연사 중 발마다의 정확도. 기본 100이고 첫 발이 100% | 게임 파일 주석 | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/from/spreading/radiustable` | block | []x8 | 8 | 8 | 예: | 확인 | 거리별 산포 반경 표. 최대 사거리를 넘으면 값이 급격히 커지게 해 원거리 사격을 막는다 | 게임 파일 주석 | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/from/spreading/radiustable/#row` | block | [num num]x42 | 8 | 42 | 예: 0 0.02 | 추정 | 거리와 산포 반경 한 쌍 (추정: .presets의 표가 {100 10} {200 20} 형태) | 파일 구조와 값에서 추론 |  |
| `/from/spreading/spreadxyratio` | block | [num]x4 | 4 | 4 | 0.75 ~ 1.5 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/from/spreadtolerance` | block | [num]x7 | 7 | 7 | 0.05 ~ 0.3 | 확인 | 사격 전에 필요한 조준 수렴 정도 | 게임 파일 주석 | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/from/states` | block | []x3 | 3 | 3 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/from/states/aim` | block | [str]x3 | 3 | 3 | "aim" ¦ "aim-rgrenade" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/can_aim` | block | [str]x3 | 3 | 3 | "can-aim-bolt" ¦ "can-aim-rgrenade" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/fire` | block | [str]x3 | 3 | 3 | "fire" ¦ "fire-rgrenade" | 추정 | 발사 상태 이름 | 파일 구조와 값에서 추론 | mg_maxim_gunner_fire |
| `/from/states/reload` | block | [str]x3 | 3 | 3 | "reload" | 추정 | 장전 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/unload` | block | [str]x3 | 3 | 3 | "unload" | 추정 | 탄 빼기 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/swayparams` | block | []x9 | 9 | 9 | 예: | 추정 | 조준 흔들림 설정 (추정: 하위 bounds의 주석) | 파일 구조와 값에서 추론 |  |
| `/from/swayparams/bounds` | block | [num num num num]x9 | 9 | 9 | -0.3 0.2 -0.4 0.3 ¦ -0.25 0.2 -0.35 0.3 ¦ -0.35 0.25 -0.45 0.35 | 확인 | 흔들림 한계. x 음의 좌우, y 양의 좌우, z 음의 상하, w 양의 상하 | 게임 파일 주석 | x -> negativeYaw, y -> positiveYaw, z -> negativePitch, w -> positive Pitch (in degree) |
| `/from/swayparams/speed` | block | [num]x9 | 9 | 9 | 0.1 ~ 0.3 | 미확인 | 값 0.1~0.3. 조준 흔들림 속도로 보이나 확인하지 못함 |  | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/syncedprojectiles` | block | [num]x3 | 3 | 3 | 1 | 확인 | 멀티플레이에서 탄흔·탄공 생성을 시각적으로 동기화한다. 자동화기에는 쓰지 않는다 | 게임 파일 주석 | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/from/tag` | block | [str]x42 | 42 | 42 | 예: "smallitemin1hand grenade dynamite size4 distributable" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from/thirdpersonparams` | block | [atom str]x9 | 9 | 9 | inherit "human default" | 미확인 | 값 inherit "human default" |  |  |
| `/from/throw` | block | []x6 | 6 | 6 | 예: | 추정 | 투척 설정 묶음 (추정: 하위 range, spreading, startangle) | 파일 구조와 값에서 추론 |  |
| `/from/throw/constantspeed` | block | [num]x1 | 1 | 1 | 13.5 | 미확인(자동) |  |  |  |
| `/from/throw/range` | block | [num]x6 | 6 | 6 | 6.2 ~ 25 | 추정 | 던지는 거리 (추정: throw의 하위) | 파일 구조와 값에서 추론 | 200m ¦ 285m max |
| `/from/throw/spreading` | block | [num]x5 | 5 | 5 | 0.05 ~ 0.1 | 추정 | 던질 때의 산포 (추정: throw의 하위) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/throw/spreading/radiusfunction` | block | [num num num num num num num num num num]x1 | 1 | 1 | 0.0 0 0.7 0 1.0 0 1.5 2.0 2.0 5.0 | 미확인(자동) |  |  |  |
| `/from/throw/spreading/spreadpower` | block | [num]x1 | 1 | 1 | 2 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/from/throw/startangle` | block | [num]x6 | 6 | 6 | 10 ~ 80 | 추정 | 던지는 각도 (추정: 이름과 값 10~80) | 파일 구조와 값에서 추론 |  |
| `/from/tracefreq` | block | [num]x3 | 3 | 3 | 3 ~ 5 | 추정 | 예광탄 빈도 (추정: 이름과 값 3~5) | 파일 구조와 값에서 추론 |  |
| `/from/unlimitedrangetpc` | block | [num]x1 | 1 | 1 | 0 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/from/zeroingaccuracy` | block | [num num num]x2 [num num num num num]x1 | 3 | 3 | 0.8 0.7 0.6 ¦ 0.9 0.8 0.75 ¦ 0.9 0.8 0.7 0.6 0.5 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/item` | block | []x7 | 7 | 7 | 예: | 추정 | 아이템 정의의 최상위 블록 (추정: headgear.pattern 등이 이 블록으로 시작) | 파일 구조와 값에서 추론 | Jerry Can (20L) ¦ 55 gallon (~210L) drum |
| `/item/damage` | block | [atom atom]x2 [atom]x1 | 1 | 3 | add blastwave ¦ blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/item/damage/area` | block | [num num]x3 | 1 | 3 | 0.5 0.75 ¦ 2 3 ¦ 6 8 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/item/damage/energy` | block | [num]x3 | 1 | 3 | 1 ~ 9 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/item/damage/ground_interaction_radius` | block | [num]x3 | 1 | 3 | 0 ~ 0.25 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/item/entity` | block | [str]x1 | 1 | 1 | "knife" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/item/inventory` | block | []x8 | 7 | 8 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/item/inventory/fsm` | block | [str]x7 | 7 | 7 | "belt" ¦ "stuff" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/item/inventory/link_bone` | block | [str]x1 | 1 | 1 | "foot1r" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/item/inventory/size` | block | [num num]x4 | 4 | 4 | 2 2 ¦ 3 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/item/inventory/weight` | block | [num]x7 | 7 | 7 | 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/item/mass` | block | [num]x6 | 6 | 6 | 0.03 ~ 8 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/item/tag` | block | [str]x7 | 7 | 7 | "smallitemin1hand head distributable" ¦ "smallitemin1hand dropboxed antipersonnel" ¦ "itemin2hands dropboxed antitank at_mine" ¦ "smallitemin1hand head armor distributable" ¦ "knife sharp_edge smallitemin1hand distributable" ¦ "missile smallitemin1hand throwable" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/item/throw` | block | []x1 | 1 | 1 | 예: | 추정 | 투척 설정 묶음 (추정: 하위 range, spreading, startangle) | 파일 구조와 값에서 추론 |  |
| `/item/throw/spreading` | block | [num]x1 | 1 | 1 | 0.3 | 미확인(자동) |  |  | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/weapon_bullet` | block | []x6 | 6 | 6 | 예: | 추정 | 탄을 쏘는 무기 정의의 최상위 블록 (추정: 패턴 파일의 구조) | 파일 구조와 값에서 추론 | Example / Default Settings ¦ required for AT-Grenades with cumaltive damage |
| `/weapon_bullet/(mod)` | mod | [str]x1 | 1 | 1 | "hardcore" | 미확인(자동) |  |  |  |
| `/weapon_bullet/accuracybymotionk` | block | [num]x1 | 1 | 1 | 0.1 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/aimingtolerance` | block | [num]x6 | 6 | 6 | 0.5 ~ 3 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/weapon_bullet/aimrange` | block | [num]x1 | 1 | 1 | 15 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/weapon_bullet/aimtype` | block | [atom]x1 | 1 | 1 | curved | 미확인(자동) |  |  |  |
| `/weapon_bullet/automatic` | block | []x1 | 1 | 1 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/autoreload` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |  |
| `/weapon_bullet/burst` | block | []x1 | 1 | 1 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 | multiplier to number of bullets in burst from weapon settings |
| `/weapon_bullet/burst/long` | block | [num num]x1 | 1 | 1 | 5 1 | 확인 | 긴 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/weapon_bullet/burst/rangekoef` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | rangeKoef x aimRange = 긴 점사를 쓰는 최대 거리 | 게임 파일 주석 | rangeKoef * aimRange = max long burst distance |
| `/weapon_bullet/burst/short` | block | [num num]x1 | 1 | 1 | 10 1 | 확인 | 짧은 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/weapon_bullet/calibre` | block | [num]x1 | 1 | 1 | 7.62 | 확인 | 구경, mm | 게임 파일 주석 | caliber of gun in millimeters ¦ AP shot caliber |
| `/weapon_bullet/clicksound` | block | [str]x2 | 2 | 2 | "trigger_click_01" ¦ "empty_gun_shot" | 미확인(자동) |  |  |  |
| `/weapon_bullet/crosshair` | block | [str]x2 | 2 | 2 | "crosshair_mgun" ¦ "crosshair_tank" | 확인 | 3인칭 조준 때의 조준선 이름 | 게임 파일 주석 | third person crosshair |
| `/weapon_bullet/crosshairreloading` | block | [str]x2 | 2 | 2 | "crosshair_mgun_reload" ¦ "crosshair_tank_reload" | 확인 | 3인칭 조준에서 재장전 중일 때의 조준선 이름 | 게임 파일 주석 | third person reload crosshair. |
| `/weapon_bullet/cursor` | block | [str]x6 | 6 | 6 | "ironsights/tank" ¦ "ironsights/bazooka" ¦ "ironsights/rifle" ¦ "ironsights/tank2" ¦ "ironsights/mortar" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/weapon_bullet/cursorreloading` | block | [str]x5 | 5 | 5 | "carbine_reload" ¦ "gun_heat" ¦ "gun_ic" ¦ "gun_he" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/weapon_bullet/curve` | block | [str]x4 | 4 | 4 | "gun_std" ¦ "bullet" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/weapon_bullet/disablericochet` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/filling` | block | [str]x1 | 1 | 1 | "mortar" | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_bullet/fillingvisible` | block | [str]x2 | 2 | 2 | "rocket" | 미확인(자동) |  |  |  |
| `/weapon_bullet/firesound` | block | [str]x2 | 2 | 2 | "flame/blank_flame" ¦ "mortar\mortar_shot" | 추정 | 발사음 경로 (추정: 이름과 값) | 파일 구조와 값에서 추론 | Placeholder |
| `/weapon_bullet/firesoundclose` | block | [str]x1 | 1 | 1 | "flame/blank_flame" | 추정 | 가까이서 들리는 발사음 경로 (추정: 이름과 값의 3p) | 파일 구조와 값에서 추론 | Placeholder |
| `/weapon_bullet/firingtimeout` | block | [num num]x4 | 4 | 4 | 11 3 ¦ 0.4 0.2 ¦ 0.8 0.2 ¦ 0.35 0.15 | 확인 | 연사 사이의 지연 | 게임 파일 주석 | 3.5 0.5 ¦ delay between bursts |
| `/weapon_bullet/foresight` | block | [str]x6 | 6 | 6 | "foresight3" ¦ "foresight" ¦ "Foresight1" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/weapon_bullet/gravity` | block | [num]x5 | 5 | 5 | 0.65 ~ 6 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/weapon_bullet/healthdamage` | block | [num]x1 | 1 | 1 | 2500 | 문서 | 사람과 동물에게 주는 체력 피해. 관통력과 별개의 값이며 단위는 임의다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/weapon_bullet/inventory` | block | []x4 | 4 | 4 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/inventory/fsm` | block | [str]x3 | 3 | 3 | "smg" ¦ "bazooka" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/weapon_bullet/inventory/handle_bone` | block | [str]x1 | 1 | 1 | "tank_back" | 확인 | 무기를 손에 들지 않고 메고 있을 때 몸에 연결되는 무기 쪽 뼈대 | 게임 파일 주석 | when weapon is shouldered (not equipped) this bone on the weapon is linked to the {link_bone} defined above |
| `/weapon_bullet/inventory/link_bone` | block | [str]x2 | 2 | 2 | "gun_back" ¦ "backpacks" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/inventory/size` | block | [num num]x3 | 3 | 3 | 8 2 ¦ 5 3 ¦ 4 2 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/weapon_bullet/inventory/weight` | block | [num]x3 | 3 | 3 | 100 ~ 1000 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/weapon_bullet/lockfiredistancetime` | block | [num]x1 | 1 | 1 | 0 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/weapon_bullet/mass` | block | [num]x1 | 1 | 1 | 1000 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/weapon_bullet/maxrange` | block | [num]x1 | 1 | 1 | 25 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/weapon_bullet/minimumdamagemodifier` | block | [num]x2 | 1 | 2 | 3 | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/weapon_bullet/minrange` | block | [num]x1 | 1 | 1 | 3 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/overmatchcalibre` | block | []x1 | 1 | 1 | 예: | 미확인 | 하위에 세 수의 행. 구경 대 장갑 두께 보정으로 보이나 확인하지 못함 |  |  |
| `/weapon_bullet/overmatchcalibre/#row` | block | [num num num]x19 | 1 | 19 | 예: 0 0.1 0.0637 | 미확인(자동) |  |  |  |
| `/weapon_bullet/overmatchdamping` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | [??] (angle - damping) |
| `/weapon_bullet/overmatchdamping/#row` | block | [num num]x6 | 1 | 6 | 0 1 ¦ 30 1.1 ¦ 55 1.15 ¦ 60 1.45 ¦ 70 4 ¦ 90 6 | 미확인(자동) |  |  |  |
| `/weapon_bullet/overmatchslope` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/overmatchslope/#row` | block | [num num]x2 | 1 | 2 | 0 1 ¦ 90 1 | 미확인(자동) |  |  |  |
| `/weapon_bullet/parameters` | block | [str]x52 | 2 | 52 | 예: "ap" | 추정 | 탄종별 설정. 문자열이 탄종 이름 (추정: 값 "heat", "clip") | 파일 구조와 값에서 추론 | SPgGr. ¦ Sprgr. |
| `/weapon_bullet/parameters/(mod)` | mod | [str]x6 | 1 | 6 | "mp" | 미확인(자동) |  |  |  |
| `/weapon_bullet/parameters/cursorreloading` | block | [str]x30 | 2 | 30 | 예: "gun_ap" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/weapon_bullet/parameters/speed` | block | [num]x22 | 1 | 22 | 75 | 미확인(자동) |  |  | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/weapon_bullet/parameters/unlimitedrangetpc` | block | [num]x6 | 1 | 6 | 0 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/weapon_bullet/preloaded` | block | []x6 | 6 | 6 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/weapon_bullet/projectiledamagethreshold` | block | [num]x2 | 2 | 2 | 1 | 확인 | 장갑 × 이 값보다 작은 피해는 무시한다 | 게임 파일 주석 | Damages lower than armor * threshold are ignored |
| `/weapon_bullet/range` | block | [num num]x1 | 1 | 1 | 70 70 | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/weapon_bullet/rechargetime` | block | [num]x3 | 3 | 3 | 0.1 ~ 10 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/weapon_bullet/recoil` | block | []x1 | 1 | 1 | 예: | 확인 | 반동 설정 묶음 | 게임 파일 주석 | weapon recoil, the higher the weaker ¦ recoil multiplier for all machine guns |
| `/weapon_bullet/recoil/maxangle` | block | [num num]x1 | 1 | 1 | 8 8 | 확인 | 최대 반동 각도(상하/좌우), 도 | 게임 파일 주석 | max recoil angle (pitch/yaw), degree |
| `/weapon_bullet/recoil/restoredelay` | block | [num]x1 | 1 | 1 | 0.2 | 확인 | 반동 회복이 시작되기 전의 지연, 초 | 게임 파일 주석 | delay before restore, sec |
| `/weapon_bullet/recoil/restoretime` | block | [num num]x1 | 1 | 1 | 0.5 0.025 | 확인 | 반동 회복 시간. 첫 값이 최소 시간, 둘째 값이 추가 시간 | 게임 파일 주석 | recoil restore time, first -> minimum time, second -> additional time per shot, sec |
| `/weapon_bullet/recoil/shoot` | block | []x1 | 1 | 1 | 예: | 확인 | 발당 반동. 상하와 좌우 각각 값과 무작위 폭, 도 | 게임 파일 주석 | pitch / +- random and yaw / +- random, per shot, degree |
| `/weapon_bullet/recoil/shoot/#row` | block | [num num]x2 | 1 | 2 | 0.90 0.25 ¦ 0 0.60 | 미확인(자동) |  |  |  |
| `/weapon_bullet/recoverytime` | block | [num]x2 | 2 | 2 | 0.1 ~ 9.34579439252336 | 확인 | 발사 간격, 초. "fire rate" 매크로가 60 / rpm으로 계산해 넣는다 | 게임 파일 주석 | increase fuel consumption by 2x |
| `/weapon_bullet/reloadanimbonus` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/weapon_bullet/reloadsound` | block | [str]x1 | 1 | 1 | "reload.smg" | 확인 | 장전음 | 게임 파일 주석 | sound for weapon reload |
| `/weapon_bullet/ricochetenergydamping` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  | penetration reduction after ricochet (angle - damping multiplier) ¦ penetration energy damping after ricochet (angle - damping) |
| `/weapon_bullet/ricochetenergydamping/#row` | block | [num num]x10 | 2 | 10 | 0 1 ¦ 60 0 ¦ 20 1 ¦ 30 0.8 ¦ 45 0.4 ¦ 10 0.9 ¦ 25 0.7 ¦ 45 0.3 | 미확인(자동) |  |  |  |
| `/weapon_bullet/speed` | block | [num]x4 | 4 | 4 | 30 ~ 174 | 추정 | 탄속 (추정: 위와 같은 키) | 파일 구조와 값에서 추론 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/weapon_bullet/spreading` | block | [num]x3 []x2 | 5 | 5 | 0.9 ~ 1.75 | 추정 | 산포 설정 묶음 또는 산포 값 | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/weapon_bullet/spreading/burstaccuracy` | block | [num]x1 | 1 | 1 | 100 | 확인 | 연사 중 발마다의 정확도. 기본 100이고 첫 발이 100% | 게임 파일 주석 | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/weapon_bullet/spreading/burstrecoverytime` | block | [num]x1 | 1 | 1 | 0.5 | 확인 | 사격 뒤 명중률이 평소로 돌아오는 데 걸리는 시간, 초 | 게임 파일 주석 | time in seconds for weapon accuracy to return to normal after firing. |
| `/weapon_bullet/spreading/radiusfunction` | block | [num num num num num num num num num num]x1 | 1 | 1 | 0.0 0 0.85 0.9 1.0 1.3 1.2 6 2 30 | 미확인(자동) |  |  |  |
| `/weapon_bullet/spreading/spreadpower` | block | [num]x4 | 4 | 4 | 1 ~ 2 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/weapon_bullet/spreading/spreadradius` | block | [num]x1 | 1 | 1 | 5 | 확인 | 무기의 산포 반경, 미터 | 게임 파일 주석 | radius of weapon accuracy in meters. Most weapon spread radius is defined in small.firearms.accuracy |
| `/weapon_bullet/spreading/spreadxyratio` | block | [num]x2 | 2 | 2 | 0.7 ~ 3 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/weapon_bullet/spreading/spreadyadjust` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/weapon_bullet/spreadtolerance` | block | [num]x1 | 1 | 1 | 0.01 | 확인 | 사격 전에 필요한 조준 수렴 정도 | 게임 파일 주석 | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/weapon_bullet/states` | block | []x3 | 3 | 3 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/aim` | block | [str]x3 | 3 | 3 | "aim" ¦ "aim-bazooka" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/can_aim` | block | [str]x3 | 3 | 3 | "can-aim" ¦ "can-aim-bazooka" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/fire` | block | [str]x3 | 3 | 3 | "fire" ¦ "fire-bazooka" | 추정 | 발사 상태 이름 | 파일 구조와 값에서 추론 | mg_maxim_gunner_fire |
| `/weapon_bullet/states/reload` | block | [str]x3 | 3 | 3 | "reload-bazooka" ¦ "reload" ¦ "" | 추정 | 장전 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/unload` | block | [str]x3 | 3 | 3 | "unload" ¦ "unload-bazooka" | 추정 | 탄 빼기 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/syncedprojectiles` | block | [num]x4 | 4 | 4 | 1 | 확인 | 멀티플레이에서 탄흔·탄공 생성을 시각적으로 동기화한다. 자동화기에는 쓰지 않는다 | 게임 파일 주석 | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/weapon_bullet/tag` | block | [str]x5 | 5 | 5 | "rocketlauncher to_hand ap cannot_lie" ¦ "weapon special_weapon_view flame_thrower fx_thrower bullet to_hand shoot_walking shoot_walking_ai shoot_walking_manual" ¦ "weapon build_in gun bullet tank_gun" ¦ "mrl gun build_in" ¦ "mrl" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/weapon_bullet/toplayervignette` | block | [str]x1 | 1 | 1 | "" | 미확인(자동) |  |  |  |
| `/weapon_bullet/unlimitedrangetpc` | block | [num]x3 | 3 | 3 | 0 ~ 1 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/weapon_bullet/zeroingaccuracy` | block | [num num num num]x1 [num num num num num]x1 | 2 | 2 | 0.7 0.6 0.5 0.4 ¦ 0.85 0.75 0.7 0.65 0.6 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |

### 유닛 종류: stuff

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/bullet` | block | []x121 | 121 | 121 | 예: | 추정 | 탄 정의의 최상위 블록 (추정: bullet.pattern) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/bullet/##trace` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/bullet/##trace/length` | block | [num]x1 | 1 | 1 | 15 | 확인 | 애니메이션 한 번에 해당하는 길이, 미터 | 게임 파일 주석 | meters per animation ¦ 0.6 meters per animation |
| `/bullet/##trace/view` | block | [str]x1 | 1 | 1 | "/texture/fx/tracers/trace_empty" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring |
| `/bullet/##trace/view/light` | block | [num]x1 | 1 | 1 | 3 | 미확인 | 값 1 |  |  |
| `/bullet/##trace/view/scaley` | block | [num]x1 | 1 | 1 | 0.2 | 미확인(자동) |  |  |  |
| `/bullet/cumulative` | block | [num]x7 | 7 | 7 | 0.7 ~ 1 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/bullet/damage` | block | [atom]x15 [atom atom]x4 | 14 | 19 | blastwave ¦ add blastwave ¦ fire | 추정 | 피해 정의. 값은 피해 종류 | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/bullet/damage/area` | block | [num num]x18 | 14 | 18 | 0.2 3.5 ¦ 0.75 7.5 ¦ 3 6 ¦ 2.49 4.98 ¦ 11.57 17.325 ¦ 18.796 28.09 ¦ 3.5 9.5 ¦ 0.5 4 ¦ 3 9 ¦ 1.53 2.55 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/bullet/damage/energy` | block | [num]x18 | 14 | 18 | 0.7 ~ 36.75 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/bullet/damage/ground_interaction_radius` | block | [num]x18 | 14 | 18 | 0 ~ 0.8 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/bullet/damage/heating` | block | [num]x1 | 1 | 1 | 10 | 미확인(자동) |  |  |  |
| `/bullet/damage/radius` | block | [num]x1 | 1 | 1 | 1.05 | 추정 | 피해가 미치는 반경, 미터 (추정: 이름. 값 1.05) | 파일 구조와 값에서 추론 | resupply radius in meter |
| `/bullet/damage/time` | block | [num]x1 | 1 | 1 | 0.5 | 추정 | 피해가 이어지는 시간, 초 (추정: 이름과 원문 주석 "seconds") | 파일 구조와 값에서 추론 | actual mining time equals the lenght of time for the human animation + this value if value is less than animation lenght. Otherwise this value is an absolute and animation will loop. ¦ actual mining time equals the length of time for the human animation + this value if value is less than animation length. Otherwise this value is an absolute and animation will loop. |
| `/bullet/detonationthickness` | block | [num]x7 | 7 | 7 | 0.5 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/bullet/entity` | block | [str]x120 | 120 | 120 | 예: "sturmtiger_ammo" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/bullet/inventory` | block | []x120 | 120 | 120 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/bullet/inventory/block` | block | [num]x103 | 103 | 103 | 5 ~ 300 | 미확인 | 탄약 파일의 inventory 안 값. 인벤토리 칸에 보이는 묶음 크기는 아니다: 250→37, 100→13으로 바꿔도 M26 Pershing 인벤토리에 4500발, 450발이 한 칸씩 그대로 보였다(2026-10-14 게임 확인). 무엇을 정하는지는 미확인 | 파일 구조와 값에서 추론 |  |
| `/bullet/inventory/fsm` | block | [str]x99 | 99 | 99 | "belt" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/bullet/inventory/size` | block | [num num]x120 | 120 | 120 | 1 1 ¦ 1 2 ¦ 2 1 ¦ 3 2 ¦ 2 2 ¦ 3 1 ¦ 3 3 ¦ 6 2 ¦ 8 4 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/bullet/inventory/weight` | block | [num]x120 | 120 | 120 | 0.333 ~ 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/bullet/mass` | block | [num]x120 | 120 | 120 | 0.006 ~ 175 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/bullet/possibledamageradius` | block | [num]x1 | 1 | 1 | 30 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/bullet/tag` | block | [str]x189 [str str]x21 | 120 | 210 | 예: "flame" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/bullet/trace` | block | []x78 | 78 | 78 | 예: | 미확인(자동) |  |  |  |
| `/bullet/trace/length` | block | [num]x78 | 78 | 78 | 1.5 ~ 3 | 확인 | 애니메이션 한 번에 해당하는 길이, 미터 | 게임 파일 주석 | meters per animation ¦ 0.6 meters per animation |
| `/bullet/trace/view` | block | [str]x78 | 78 | 78 | "/texture/fx/tracers/trace_empty" ¦ "/texture/fx/tracer_new/trace_solid_red.mtl" ¦ "/texture/fx/tracer_new/trace_solid_orange.mtl" ¦ "/texture/fx/tracer_new/trace_solid_light_green.mtl" ¦ "/texture/fx/tracer_new/trace_solid_yellow.mtl" ¦ "/texture/fx/tracer_new/trace_scattered_green.mtl" ¦ "/texture/fx/tracer_new/trace_scattered_red.mtl" ¦ "/texture/fx/tracer_new/trace_solid_green.mtl" ¦ "/texture/fx/tracer_new/trace_scattered_white.mtl" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring |
| `/bullet/trace/view/light` | block | [num]x78 | 78 | 78 | 1 | 미확인 | 값 1 |  |  |
| `/bullet/trace/view/scalex` | block | [num]x78 | 78 | 78 | 1.5 ~ 2 | 미확인(자동) |  |  |  |
| `/bullet/trace/view/scaley` | block | [num]x78 | 78 | 78 | 0.1 ~ 0.2 | 미확인(자동) |  |  |  |
| `/bullet/view` | block | [str]x36 | 36 | 36 | 예: "300mm_fin_mortar_he" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring |
| `/bullet/view/tail` | block | [str str]x23 | 23 | 23 | "flare_tail_red" "fxshell" ¦ "flare_tail_green" "fxshell" ¦ "shell_tail" "tail" ¦ "hook_tail" "tail" | 추정 | 탄의 꼬리 이펙트. 이펙트 이름과 붙이는 뼈대 (추정: 값 "shell_tail" "tail") | 파일 구조와 값에서 추론 |  |
| `/from` | block | [str]x2522 | 2522 | 2522 | 예: "pattern rpg_heat_ammo" | 추정 | 원형(pattern)에서 물려받는 아이템 정의. 첫 문자열 "pattern 이름"이 같은 이름의 .pattern 파일을 가리킨다 (추정: 참조 색인. 물려받은 값과 합쳐지는 규칙은 미확인) | 파일 구조와 값에서 추론 | used for Multi MG half tracks ¦ "" |
| `/from/(mod)` | mod | [atom str]x15 | 15 | 15 | not "mp" | 미확인(자동) |  |  |  |
| `/from/##damage` | block | [atom atom]x18 [atom]x12 | 12 | 30 | add blastwave ¦ blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 |  |
| `/from/##damage/area` | block | [num num]x30 | 12 | 30 | 예: 0.4 0.5 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/##damage/energy` | block | [num]x30 | 12 | 30 | 1.195 ~ 10.93 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/##damage/ground_interaction_radius` | block | [num]x30 | 12 | 30 | 0 ~ 1 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/accuracybymotionk` | block | [num]x29 | 29 | 29 | 0.5 ~ 1.2 | 추정 | 움직이는 동안의 명중률에 곱하는 계수 (추정: 이름. 값 0.1 ~ 0.9) | 파일 구조와 값에서 추론 |  |
| `/from/aimingtolerance` | block | [num]x67 | 46 | 67 | 0.8 ~ 20 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/from/aimrange` | block | [num]x33 | 33 | 33 | 30 ~ 325 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/from/aimtype` | block | [atom]x2 | 2 | 2 | curved | 미확인(자동) |  |  |  |
| `/from/ammolocked` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/automatic` | block | []x54 | 54 | 54 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/from/burst` | block | []x172 | 166 | 172 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 | multiplier to number of bullets in burst from weapon settings |
| `/from/burst/long` | block | [num num]x132 [num]x23 | 149 | 155 | 1 | 확인 | 긴 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on short distance, rand (+-)shots ¦ long burst shots on short distance, rand (+-)shots |
| `/from/burst/min` | block | [num]x17 | 17 | 17 | 2 ~ 30 | 미확인(자동) |  |  |  |
| `/from/burst/noaiming` | block | []x16 | 16 | 16 | 예: | 미확인(자동) |  |  |  |
| `/from/burst/rangekoef` | block | [num]x155 | 149 | 155 | 0.25 ~ 1 | 확인 | rangeKoef x aimRange = 긴 점사를 쓰는 최대 거리 | 게임 파일 주석 | rangeKoef * aimRange = max long burst distance |
| `/from/burst/short` | block | [num num]x129 [num]x26 | 149 | 155 | 1 | 확인 | 짧은 점사의 발 수와 무작위 폭 | 게임 파일 주석 | burst shots on long distance, rand (+-)shots ¦ short burst shots on long distance, rand (+-)shots |
| `/from/calibre` | block | [num]x358 | 358 | 358 | 1 ~ 600 | 확인 | 구경, mm | 게임 파일 주석 | caliber of gun in millimeters ¦ AP shot caliber |
| `/from/cumulative` | block | [num]x1 | 1 | 1 | 1 | 확인 | 볼륨을 맞힌 뒤 관통 효과가 이어지는 거리, 미터 (성형작약) | 게임 파일 주석 | after hitting a volume, distance (meters) that piercing projectile travels before terminating |
| `/from/cursor` | block | [str]x28 | 28 | 28 | "ironsights/aa" ¦ "ironsights/rifle" ¦ "ironsights/mortar" ¦ "ironsights/grenade" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/from/cursorreloading` | block | [str]x9 | 9 | 9 | "gun_ap" ¦ "carbine_reload" ¦ "gun_he" ¦ "gun_apcr" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/curve` | block | [str]x27 | 27 | 27 | "gun_autocannon" ¦ "autorifle" ¦ "shotgun" ¦ "assaultrifle" ¦ "bullet_mg" ¦ "gun_std" ¦ "bullet_smg" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/from/damage` | block | [atom]x397 [atom atom]x395 | 379 | 792 | add blastwave ¦ blastwave ¦ fire ¦ cumulative | 추정 | 피해 정의. 값은 피해 종류: blastwave, fire. add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/area` | block | [num num]x761 | 377 | 761 | 예: 1.8 3 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/from/damage/damage` | block | [num]x4 | 4 | 4 | 31 ~ 150 | 미확인 | 수류탄의 damage 하위. 값 31 ~ 150 |  | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/damage/distance` | block | [num]x4 | 4 | 4 | 0.25 | 미확인(자동) |  |  | activation distance |
| `/from/damage/energy` | block | [num]x761 | 377 | 761 | 0 ~ 130.05 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/from/damage/ground_interaction_radius` | block | [num]x761 | 377 | 761 | 0 ~ 2.5 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/from/damage/heating` | block | [num]x27 | 27 | 27 | 0.5 ~ 20 | 미확인(자동) |  |  |  |
| `/from/damage/radius` | block | [num]x27 | 27 | 27 | 0.5 ~ 6.25 | 추정 | 피해가 미치는 반경, 미터 (추정: 이름. 값 1.05) | 파일 구조와 값에서 추론 | resupply radius in meter |
| `/from/damage/time` | block | [num]x27 | 27 | 27 | 0.5 ~ 1 | 추정 | 피해가 이어지는 시간, 초 (추정: 이름과 원문 주석 "seconds") | 파일 구조와 값에서 추론 | actual mining time equals the lenght of time for the human animation + this value if value is less than animation lenght. Otherwise this value is an absolute and animation will loop. ¦ actual mining time equals the length of time for the human animation + this value if value is less than animation length. Otherwise this value is an absolute and animation will loop. |
| `/from/detonationthickness` | block | [num]x23 | 23 | 23 | 0.5 ~ 1 | 문서 | 맞은 물체의 두께와 견주는 기폭 조건으로 보인다. 개발자 기록은 이 값이 있으면 나무 울타리 같은 얇은 물체에서 포탄이 터지지 않았다고 적는다 | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/from/disablericochet` | block | []x56 | 56 | 56 | 예: | 미확인(자동) |  |  |  |
| `/from/entity` | block | [str]x2224 | 2224 | 2224 | 예: "bazooka_heat_ammo" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/from/falloffmeter` | block | [num]x3 | 3 | 3 | 100 ~ 180 | 미확인 | 값 1~220 |  |  |
| `/from/falloffstrength` | block | [num]x3 | 3 | 3 | 0.15 | 미확인 | 값 0~0.15 |  |  |
| `/from/filling` | block | [str num]x256 [str]x154 | 410 | 410 | 예: "ammo bazooka" 1 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/from/fillingvisible` | block | [str]x32 | 32 | 32 | "rocket" ¦ "grenade" ¦ "faustrocket" ¦ "shell" ¦ "rocket00" ¦ "vpgs41" | 미확인(자동) |  |  |  |
| `/from/firesound` | block | [str]x375 | 374 | 375 | 예: "reactive/d1/Shared/Rocket_Launcher/" | 추정 | 발사음 경로 (추정: 이름과 값) | 파일 구조와 값에서 추론 | Placeholder |
| `/from/firesoundclose` | block | [str]x374 | 374 | 374 | 예: "reactive/3p/GER/Panzerfaust/" | 추정 | 가까이서 들리는 발사음 경로 (추정: 이름과 값의 3p) | 파일 구조와 값에서 추론 | Placeholder |
| `/from/firingtimeout` | block | [num num]x30 | 30 | 30 | 2.0 0.5 ¦ 4 0.5 ¦ 3.5 0.5 ¦ 1.5 0.5 ¦ 2.5 0.5 ¦ 3.1 0.5 ¦ 2.9 0.5 ¦ 1.75 0.5 ¦ 2.75 0.5 ¦ 1.8 0.4 | 확인 | 연사 사이의 지연 | 게임 파일 주석 | 3.5 0.5 ¦ delay between bursts |
| `/from/firingtimeoutdistancefactor` | block | []x62 | 62 | 62 | 예: | 미확인 | 하위에 두 수의 행 |  |  |
| `/from/firingtimeoutdistancefactor/#row` | block | [num num]x201 | 62 | 201 | 예: 0.2 0.65 | 미확인 | 두 수 |  |  |
| `/from/foresight` | block | [str]x14 | 14 | 14 | "foresight1" ¦ "foresight4" ¦ "foresight2" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/from/gravity` | block | [num]x199 | 186 | 199 | 0.3 ~ 6 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/from/healthdamage` | block | [num]x49 | 48 | 49 | 55 ~ 500 | 문서 | 사람과 동물에게 주는 체력 피해. 관통력과 별개의 값이며 단위는 임의다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/from/inventory` | block | []x483 | 483 | 483 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/from/inventory/block` | block | [num]x42 | 42 | 42 | 5 ~ 10 | 미확인(자동) |  |  |  |
| `/from/inventory/fsm` | block | [str]x5 | 5 | 5 | "belt" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/from/inventory/link_bone` | block | [str]x2 | 2 | 2 | "gun_back" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/from/inventory/noview` | block | []x57 | 57 | 57 | 예: | 미확인(자동) |  |  |  |
| `/from/inventory/size` | block | [num num]x426 | 426 | 426 | 예: 7 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/from/inventory/weight` | block | [num]x76 | 76 | 76 | 10 ~ 200 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/from/lockfiredistancetime` | block | [num]x12 | 12 | 12 | 0.5 | 확인 | 조준점이 사거리 밖에 있어도 사격이 허용되는 시간 | 게임 파일 주석 | Time that AIM is allowed to be out of range for before firing is disabled |
| `/from/mass` | block | [num]x597 | 597 | 597 | 0.17 ~ 2170 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/from/maxrange` | block | [num]x34 | 34 | 34 | 40 ~ 325 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/from/minimumdamagemodifier` | block | [num]x162 | 162 | 162 | 2 ~ 450 | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/from/minrange` | block | [num]x35 | 35 | 35 | 5 ~ 150 | 추정 | 최소 사거리 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/overmatchcalibre` | block | []x28 | 28 | 28 | 예: | 미확인 | 하위에 세 수의 행. 구경 대 장갑 두께 보정으로 보이나 확인하지 못함 |  |  |
| `/from/overmatchcalibre/#row` | block | [num num num]x397 | 28 | 397 | 예: 0 0.1 0.0637 | 미확인 | 세 수 |  |  |
| `/from/overmatchslope` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |  |
| `/from/overmatchslope/#row` | block | [num num]x12 | 6 | 12 | 0 1 ¦ 90 1 | 미확인(자동) |  |  |  |
| `/from/parameters` | block | [str]x12070 | 246 | 12070 | 예: "heat" | 추정 | 탄종별 설정. 문자열이 탄종 이름 (추정: 값 "heat", "clip") | 파일 구조와 값에서 추론 | SPgGr. ¦ Sprgr. |
| `/from/parameters/(mod)` | mod | [str]x20 | 20 | 20 | "mp" | 미확인(자동) |  |  |  |
| `/from/parameters/aimrange` | block | [num]x1 | 1 | 1 | 140 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/from/parameters/cursor` | block | [str]x7 | 5 | 7 | "ironsights/grenade" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/from/parameters/cursorreloading` | block | [str]x64 | 29 | 64 | "gun_ap" ¦ "gun_heat" ¦ "gun_he" ¦ "gun_api" ¦ "gun_wp" ¦ "gun_saphe" ¦ "gun_apcnr" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/from/parameters/falloffmeter` | block | [num]x1339 | 175 | 1339 | 1 ~ 220 | 미확인 | 값 1~220 |  |  |
| `/from/parameters/falloffstrength` | block | [num]x1357 | 176 | 1357 | 0 ~ 0.15 | 미확인 | 값 0~0.15 |  |  |
| `/from/parameters/gravity` | block | [num]x986 | 54 | 986 | 0.08 ~ 3.5 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/from/parameters/healthdamage` | block | [num]x48 | 18 | 48 | 100 ~ 500 | 문서 | 사람과 동물에게 주는 체력 피해. 관통력과 별개의 값이며 단위는 임의다 | Call to Arms 위키의 용어집 calltoarms.fandom.com/wiki/Modding/Terms (2026-10-12 확인) |  |
| `/from/parameters/maxrange` | block | [num]x1 | 1 | 1 | 140 | 문서 | 쏠 수 있는 최대 거리 | mow-portal.ru의 SOEdit 해설 글 (Men of War 계열, 2026-10-12 확인) | 4,55km ¦ 4,3km |
| `/from/parameters/minimumdamagemodifier` | block | [num]x2605 | 179 | 2605 | 5 ~ 774.518828571428 | 문서 | 최소 피해. 계산식은 출처에 설명되어 있지 않다 | Call to Arms Steam 토론 steamcommunity.com/app/302670/discussions/0/3647273545681516166 (2026-10-12 확인) | In case only generic value is called ¦ Filler defined in grams of TNT |
| `/from/parameters/overmatchcalibre` | block | []x795 | 130 | 795 | 예: | 미확인 | 하위에 세 수의 행. 구경 대 장갑 두께 보정으로 보이나 확인하지 못함 |  |  |
| `/from/parameters/overmatchcalibre/#row` | block | [num num num]x11140 | 130 | 11140 | 예: 0 1.0 0.00 | 미확인(자동) |  |  |  |
| `/from/parameters/overmatchfixed` | block | []x109 | 70 | 109 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/overmatchfixed/#row` | block | [num num]x2071 | 70 | 2071 | 예: 0 1.00 | 미확인(자동) |  |  |  |
| `/from/parameters/overmatchslope` | block | []x371 | 109 | 371 | 예: | 미확인(자동) |  |  |  |
| `/from/parameters/overmatchslope/#row` | block | [num num]x748 | 109 | 748 | 0 1 ¦ 90 1 ¦ 50 1 ¦ 60 2 ¦ 90 99 | 미확인(자동) |  |  |  |
| `/from/parameters/projectiledamagetable` | block | []x392 | 178 | 392 | 예: | 추정 | 거리별 관통력 표 (추정: #row가 두 수의 쌍이고 주석이 탄종 이름) | 파일 구조와 값에서 추론 | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/from/parameters/projectiledamagetable/#row` | block | [num num]x2101 | 178 | 2101 | 예: 71 82 | 추정 | 거리와 관통력 한 쌍 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/from/parameters/range` | block | [num num]x340 | 134 | 340 | 예: 220 220 | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/from/parameters/rechargetime` | block | [num]x43 | 28 | 43 | 3.3 ~ 30 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/from/parameters/reloadsound` | block | [str]x1 | 1 | 1 | "reload_clip_x2" | 확인 | 장전음 | 게임 파일 주석 | sound for weapon reload |
| `/from/parameters/speed` | block | [num]x2986 | 139 | 2986 | 0.75 ~ 1050 | 확인 | 탄속. "velocity" 매크로가 0.35 x m/s로 계산해 넣는다 | 게임 파일 주석 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/parameters/spreading` | block | []x4651 [num]x105 | 181 | 4756 | 1 ~ 4.5 | 추정 | 산포 설정 묶음 또는 산포 값 (추정: 하위 radiustable) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/parameters/spreading/radiustable` | block | []x4406 | 175 | 4406 | 예: | 확인 | 거리별 산포 반경 표. 최대 사거리를 넘으면 값이 급격히 커지게 해 원거리 사격을 막는다 | 게임 파일 주석 | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/from/parameters/spreading/radiustable/#row` | block | [num num]x26186 | 175 | 26186 | 예: 0 0 | 추정 | 거리와 산포 반경 한 쌍 | 파일 구조와 값에서 추론 |  |
| `/from/parameters/spreading/spreadpower` | block | [num]x96 | 48 | 96 | 1 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/from/parameters/spreading/spreadxyratio` | block | [num]x350 | 142 | 350 | 0.75 ~ 3 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/from/parameters/unlimitedrangetpc` | block | [num]x20 | 20 | 20 | 1 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/from/possibledamageradius` | block | [num]x42 | 42 | 42 | 0.1 ~ 30 | 추정 | 피해가 미칠 수 있는 반경 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/from/projectiledamagetable` | block | []x46 | 46 | 46 | 예: | 추정 | 거리별 관통력 표 (추정: #row가 두 수의 쌍이고 주석이 탄종 이름) | 파일 구조와 값에서 추론 | adjusted to reduce far shoot ¦ M8 API + 350mph airspeed and reduced curve |
| `/from/projectiledamagetable/#row` | block | [num num]x201 | 46 | 201 | 예: 31 140 | 추정 | 거리와 관통력 한 쌍 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/from/range` | block | [num num]x271 | 252 | 271 | 예: 30 30 | 추정 | 사거리 두 값 (추정: 원문 주석이 "200m", "285m max". 두 값의 구분은 미확인) | 게임 파일 주석 | 200m ¦ 285m max |
| `/from/rechargetime` | block | [num]x357 | 338 | 357 | 0.05 ~ 300 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/from/recoil` | block | []x46 | 46 | 46 | 예: | 확인 | 반동 설정 묶음 | 게임 파일 주석 | weapon recoil, the higher the weaker ¦ recoil multiplier for all machine guns |
| `/from/recoil/maxangle` | block | [num num]x46 | 46 | 46 | 8 8 | 확인 | 최대 반동 각도(상하/좌우), 도 | 게임 파일 주석 | max recoil angle (pitch/yaw), degree |
| `/from/recoil/restoredelay` | block | [num]x46 | 46 | 46 | 0.2 | 확인 | 반동 회복이 시작되기 전의 지연, 초 | 게임 파일 주석 | delay before restore, sec |
| `/from/recoil/restoretime` | block | [num num]x46 | 46 | 46 | 0.5 0.025 ¦ 0.15 0.005 ¦ 1.00 0.025 ¦ 0.25 0.005 | 확인 | 반동 회복 시간. 첫 값이 최소 시간, 둘째 값이 추가 시간 | 게임 파일 주석 | recoil restore time, first -> minimum time, second -> additional time per shot, sec |
| `/from/recoil/shoot` | block | []x46 | 46 | 46 | 예: | 확인 | 발당 반동. 상하와 좌우 각각 값과 무작위 폭, 도 | 게임 파일 주석 | pitch / +- random and yaw / +- random, per shot, degree |
| `/from/recoil/shoot/#row` | block | [num num]x92 | 46 | 92 | 0 0.60 ¦ 0.325 0.10 ¦ 0 0.2 ¦ 0.90 0.25 ¦ 0.75 0.25 ¦ 2.50 0.75 ¦ 1 0.75 ¦ 0.45 0.20 ¦ 0 0.50 ¦ 0.55 0.20 ¦ 0.10 0.50 | 추정 | 반동 값과 무작위 폭 (추정: 위 주석) | 게임 파일 주석 |  |
| `/from/recoverytime` | block | [num]x207 | 177 | 207 | 0.008246289169874 ~ 3.73831775700935 | 확인 | 발사 간격, 초. "fire rate" 매크로가 60 / rpm으로 계산해 넣는다 | 게임 파일 주석 | increase fuel consumption by 2x |
| `/from/relaxationtime` | block | [num]x3 | 3 | 3 | 0.25 ~ 2.5 | 미확인 | 값 0.25~2 |  |  |
| `/from/reloadanimbonus` | block | [num]x7 | 7 | 7 | 57 ~ 117 | 미확인(자동) |  |  |  |
| `/from/reloadsound` | block | [str]x357 | 355 | 357 | 예: "bazooka" | 확인 | 장전음 | 게임 파일 주석 | sound for weapon reload |
| `/from/special` | block | []x14 | 14 | 14 | 예: | 미확인 | 하위에 mode, preparetime, preparecursor |  | {throw_off_always} |
| `/from/special/accuracy` | block | [num]x1 | 1 | 1 | 3 | 미확인(자동) |  |  | weapon accuracy, the higher the better ¦ accuracy multiplier from weapon settings |
| `/from/special/damage` | block | [num]x1 | 1 | 1 | 5 | 미확인 | 값 5. 특수 사격(special) 설정의 하위 |  | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/from/special/mode` | block | [atom]x5 | 5 | 5 | detonate ¦ aim | 미확인 | 값 detonate |  |  |
| `/from/special/preparecursor` | block | [str]x5 | 5 | 5 | "grenade" ¦ "aimed_shot" | 추정 | 준비 중 커서 (추정: 이름과 값 "grenade") | 파일 구조와 값에서 추론 |  |
| `/from/special/preparecursorscene` | block | [str]x1 | 1 | 1 | "aimed_shot_scene" | 미확인(자동) |  |  |  |
| `/from/special/preparetime` | block | [num]x12 | 12 | 12 | 3.5 ~ 7 | 추정 | 사용 전 준비 시간 (추정: 이름과 값 2~8) | 파일 구조와 값에서 추론 |  |
| `/from/speed` | block | [num]x352 | 352 | 352 | 0.75 ~ 1050 | 확인 | 탄속. "velocity" 매크로가 0.35 x m/s로 계산해 넣는다 | 게임 파일 주석 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/spreading` | block | []x357 [num]x13 | 322 | 370 | 1.25 ~ 12 | 추정 | 산포 설정 묶음 또는 산포 값 (추정: 하위 radiustable) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/spreading/burstaccuracy` | block | [num num num num num num num num num num num num num num num]x32 [num num num num num num num num]x26 [num num num num num num num]x25 [num num num num num num num num num]x21 [num num num]x10 | 131 | 140 | 100 | 확인 | 연사 중 발마다의 정확도. 기본 100이고 첫 발이 100% | 게임 파일 주석 | default value 100. Accuracy for 1st shot of burst is 100%. Lower values mean less accurate. |
| `/from/spreading/burstrecoverytime` | block | [num]x38 | 33 | 38 | 0.5 | 확인 | 사격 뒤 명중률이 평소로 돌아오는 데 걸리는 시간, 초 | 게임 파일 주석 | time in seconds for weapon accuracy to return to normal after firing. |
| `/from/spreading/radiusfunction` | block | [num num num num num num num num]x5 [num num num num num num]x2 [num num num num num num num num num num num num]x1 | 8 | 8 | 0.0 0 1.0 1.0 1.2 2.5 2.5 5.0 ¦ 1 0.1 300 1 400 2 ¦ 0 0.02 0.2 0.1 0.5 0.4 0.75 0.7 0.9995 1 1.00 50 | 미확인(자동) |  |  |  |
| `/from/spreading/radiustable` | block | []x88 | 68 | 88 | 예: | 확인 | 거리별 산포 반경 표. 최대 사거리를 넘으면 값이 급격히 커지게 해 원거리 사격을 막는다 | 게임 파일 주석 | Anti-farshoot, accuracy goes haywire past max range ¦ previous default apds accuracy |
| `/from/spreading/radiustable/#row` | block | [num num]x439 | 68 | 439 | 예: 0 0.5 | 추정 | 거리와 산포 반경 한 쌍 (추정: .presets의 표가 {100 10} {200 20} 형태) | 파일 구조와 값에서 추론 |  |
| `/from/spreading/spreadpower` | block | [num]x19 | 14 | 19 | 1 ~ 1.5 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/from/spreading/spreadradius` | block | [num]x2 | 2 | 2 | 4 | 확인 | 무기의 산포 반경, 미터 | 게임 파일 주석 | radius of weapon accuracy in meters. Most weapon spread radius is defined in small.firearms.accuracy |
| `/from/spreading/spreadxyratio` | block | [num]x186 | 181 | 186 | 0.4 ~ 5 | 확인 | 산포의 가로세로 비. 탄이 좌우보다 앞뒤로 더 퍼지게 한다 | 게임 파일 주석 | Shots land more longitudinally than transversely. |
| `/from/spreading/spreadyadjust` | block | [num]x2 | 2 | 2 | 0.1 | 미확인(자동) |  |  |  |
| `/from/spreadtolerance` | block | [num]x184 | 164 | 184 | 0 ~ 0.85 | 확인 | 사격 전에 필요한 조준 수렴 정도 | 게임 파일 주석 | Zeroing up to 85% needed before fire ¦ AI needs to aim 20% |
| `/from/states` | block | []x17 | 17 | 17 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/from/states/aim` | block | [str]x17 | 17 | 17 | "aim" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/can_aim` | block | [str]x17 | 17 | 17 | "can-aim-bolt" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/fire` | block | [str]x17 | 17 | 17 | "fire" | 추정 | 발사 상태 이름 | 파일 구조와 값에서 추론 | mg_maxim_gunner_fire |
| `/from/states/reload` | block | [str]x17 | 17 | 17 | "reload" | 추정 | 장전 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/states/unload` | block | [str]x17 | 17 | 17 | "unload" | 추정 | 탄 빼기 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/from/swayparams` | block | []x2 | 2 | 2 | 예: | 추정 | 조준 흔들림 설정 (추정: 하위 bounds의 주석) | 파일 구조와 값에서 추론 |  |
| `/from/swayparams/bounds` | block | [num num num num]x2 | 2 | 2 | -0.3 0.2 -0.4 0.3 | 확인 | 흔들림 한계. x 음의 좌우, y 양의 좌우, z 음의 상하, w 양의 상하 | 게임 파일 주석 | x -> negativeYaw, y -> positiveYaw, z -> negativePitch, w -> positive Pitch (in degree) |
| `/from/swayparams/speed` | block | [num]x2 | 2 | 2 | 0.2 | 미확인 | 값 0.1~0.3. 조준 흔들림 속도로 보이나 확인하지 못함 |  | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/from/syncedprojectiles` | block | [num]x30 | 29 | 30 | 0 ~ 1 | 확인 | 멀티플레이에서 탄흔·탄공 생성을 시각적으로 동기화한다. 자동화기에는 쓰지 않는다 | 게임 파일 주석 | not for automatic weapons ¦ visually syncs decal/crater spawn in multiplayer |
| `/from/tag` | block | [str]x1751 [str str]x8 [str str str str]x2 | 1681 | 1761 | 예: "heat_shell size1" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/from/thirdpersonparams` | block | [atom str]x10 | 10 | 10 | inherit "human default" | 미확인 | 값 inherit "human default" |  |  |
| `/from/thirdpersonparams/level` | block | [num]x10 | 10 | 10 | 1 | 추정 | 3인칭 조준의 확대 단계 하나. 값은 단계 번호 (추정: 값 1, 2와 하위 camera) | 파일 구조와 값에서 추론 | 2 |
| `/from/thirdpersonparams/level/aimer_type` | block | [str]x10 | 10 | 10 | "sniper" | 추정 | 조준 방식 이름 (추정: 값 "sniper") | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/blur_mask` | block | [str]x10 | 10 | 10 | "/texture/common/environment/lens_effects/zoom_vignette_scope_01_mask" | 추정 | 가장자리 흐림 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/blur_strength` | block | [num]x10 | 10 | 10 | 10 | 추정 | 가장자리 흐림의 세기 (추정: 이름) | 파일 구조와 값에서 추론 | blur strength modifier |
| `/from/thirdpersonparams/level/bump_spec_mask` | block | [str]x10 | 10 | 10 | "/texture/common/environment/lens_effects/zoom_vignette_scope_ger_k98" ¦ "/texture/common/environment/lens_effects/zoom_vignette_scope_eng_m1918" ¦ "/texture/common/environment/lens_effects/zoom_vignette_scope_rus_pe" ¦ "/texture/common/environment/lens_effects/zoom_vignette_scope_eng_no32" | 추정 | 조준경 화면에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/camera` | block | [str]x10 | 10 | 10 | "human_zoom4" | 확인 | 이 단계에서 쓰는 카메라 프리셋. 배율을 정한다. human_zoom4는 4배 | 게임 파일 주석 | magnification x4 ¦ magnification x8 |
| `/from/thirdpersonparams/level/delta` | block | [num]x10 | 10 | 10 | 20 | 미확인(자동) |  |  | distance delta, effects number of steps in between near and far ¦ distance delta |
| `/from/thirdpersonparams/level/distance` | block | [num]x10 | 10 | 10 | 35 | 미확인(자동) |  |  | activation distance |
| `/from/thirdpersonparams/level/distanceset` | block | [num]x10 | 10 | 10 | 0 | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/drop_zoom_after_shot` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/drop_zoom_on_reload` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/hide_crosshair` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/hide_weapon` | block | [num]x10 | 10 | 10 | 1 | 추정 | 조준 중에 무기 모델을 숨길지 여부 (추정: 이름과 값 1) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/lens_dirt_mask` | block | [str]x10 | 10 | 10 | "/texture/common/environment/lens_effects/dirty_lens_scope_01" | 추정 | 렌즈 얼룩 효과에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/linkbone` | block | [str]x10 | 10 | 10 | "head" | 추정 | 조준 카메라가 붙는 뼈대 (추정: 값 "head") | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/manualaccuracyzoom` | block | []x10 | 10 | 10 | 예: | 추정 | 직접 조종에서 확대했을 때 정확도 배율 (추정: 이름. 값 1.25) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/mouse_speed_factor` | block | [num]x10 | 10 | 10 | 0.6 | 추정 | 조준 중 마우스 감도 배수 (추정: 이름과 값 0.6 ~ 0.8) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/offset` | block | [num num num]x10 | 10 | 10 | 35 2.25 0.75 | 추정 | 표시 위치 오프셋 세 값 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/refraction_mask` | block | [str]x10 | 10 | 10 | "/texture/common/environment/lens_effects/zoom_vignette_scope_01_bp" | 추정 | 굴절 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/scope_vignette` | block | [str]x10 | 10 | 10 | "/texture/common/environment/lens_effects/zoom_vignette_scope_ger_k98" ¦ "/texture/common/environment/lens_effects/zoom_vignette_scope_eng_m1918" ¦ "/texture/common/environment/lens_effects/zoom_vignette_scope_rus_pe" ¦ "/texture/common/environment/lens_effects/zoom_vignette_scope_eng_no32" | 추정 | 조준경 테두리로 쓰는 텍스처 (추정: 값이 lens_effects 폴더의 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/from/thirdpersonparams/level/vignette_fade_speed` | block | [num]x10 | 10 | 10 | 5 | 확인 | 조준경 테두리(비네트)가 나타나고 사라지는 속도 계수 | 게임 파일 주석 | factoring the speed at which the vignette fades in/out |
| `/from/thirdpersonparams/level/vignette_gloss` | block | [num]x10 | 10 | 10 | 10 | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/vignette_recoil` | block | [num num]x10 | 10 | 10 | 2.75 1.1 | 확인 | 반동 때 조준경 테두리가 흔들리는 속도와 크기 | 게임 파일 주석 | speed, scale |
| `/from/thirdpersonparams/level/vision_modes_active` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/weapon_sway_active` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/with_owner` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  | removes shield to prevent clipping |
| `/from/thirdpersonparams/level/with_owner/viewoff` | block | [str]x10 | 10 | 10 | "skin" | 미확인(자동) |  |  | removes shield to prevent clipping ¦ removes to prevent clipping |
| `/from/thirdpersonparams/level/with_weapon` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/with_weapon/viewon` | block | [str]x10 | 10 | 10 | "foresight3" | 미확인(자동) |  |  |  |
| `/from/thirdpersonparams/level/zoom` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |  |
| `/from/throw` | block | []x13 | 13 | 13 | 예: | 추정 | 투척 설정 묶음 (추정: 하위 range, spreading, startangle) | 파일 구조와 값에서 추론 |  |
| `/from/throw/constantspeed` | block | [num]x5 | 5 | 5 | 12 ~ 14.5 | 미확인(자동) |  |  |  |
| `/from/throw/range` | block | [num]x13 | 13 | 13 | 8 ~ 28.7 | 추정 | 던지는 거리 (추정: throw의 하위) | 파일 구조와 값에서 추론 | 200m ¦ 285m max |
| `/from/throw/spreading` | block | [num]x4 | 4 | 4 | 0.1 ~ 1 | 추정 | 던질 때의 산포 (추정: throw의 하위) | 파일 구조와 값에서 추론 | improved accuracy due to sight ¦ Skews the distribution of shells more biased toward the centre (>1) or away (<1) |
| `/from/throw/spreading/spreadpower` | block | [num]x3 | 3 | 3 | 2 | 확인 | 조준원 안에서 탄이 떨어지는 분포를 정한다 | 게임 파일 주석 | Shot has en equal chance of landing anywhere on the aiming circle, no center-bias ¦ default value 1. Higher values increase chance of bullet to go into the center of spread radius. |
| `/from/throw/startangle` | block | [num]x4 | 4 | 4 | 80 | 추정 | 던지는 각도 (추정: 이름과 값 10~80) | 파일 구조와 값에서 추론 |  |
| `/from/trace` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |  |
| `/from/trace/length` | block | [num]x4 | 4 | 4 | 3 ~ 20 | 확인 | 애니메이션 한 번에 해당하는 길이, 미터 | 게임 파일 주석 | meters per animation ¦ 0.6 meters per animation |
| `/from/trace/view` | block | [str]x4 | 4 | 4 | "/texture/fx/tracer_new/trace_solid_red.mtl" ¦ "/texture/fx/tracer_new/trace_solid_orange.mtl" ¦ "/texture/fx/tracer_new/trace_solid_white.mtl" ¦ "/texture/fx/tracers/trace_zenite" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring |
| `/from/trace/view/light` | block | [num]x3 | 3 | 3 | 1 | 미확인 | 값 1 |  |  |
| `/from/trace/view/scalex` | block | [num]x3 | 3 | 3 | 1.75 | 미확인(자동) |  |  |  |
| `/from/trace/view/scaley` | block | [num]x4 | 4 | 4 | 0.15 ~ 0.22 | 미확인(자동) |  |  |  |
| `/from/tracefreq` | block | [num]x58 | 58 | 58 | 0 ~ 5 | 추정 | 예광탄 빈도 (추정: 이름과 값 3~5) | 파일 구조와 값에서 추론 |  |
| `/from/unlimitedrangetpc` | block | [num]x58 | 58 | 58 | 0 | 확인 | 3인칭·1인칭 조종에서 사거리 제한 여부 | 게임 파일 주석 | restrict range in TPC/FPC |
| `/from/view` | block | [str]x343 | 343 | 343 | 예: "bazooka_heat_ammo" | 추정 | 보이는 모델 엔티티 이름 (추정: 값이 entity와 같은 이름인 경우가 많음) | 파일 구조와 값에서 추론 | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring |
| `/from/view/tail` | block | [str str]x331 | 331 | 331 | 예: "bazooka_tail" "tail" | 추정 | 탄의 꼬리 이펙트. 이펙트 이름과 붙이는 뼈대 (추정: 값 "shell_tail" "tail") | 파일 구조와 값에서 추론 |  |
| `/from/zeroingaccuracy` | block | [num num num num num]x137 [num num num num num num num num num]x15 [num num num]x2 [num num num num num num num]x2 [num num num num]x1 | 159 | 159 | 1 | 추정 | 같은 표적에 연속으로 쏠 때의 발수별 명중 보정값으로 보인다 (추정: 개발자 변경 기록이 zeroing을 "연속으로 쏠수록 명중률이 오르는 보정"이라고 설명. 값이 차례로 줄어드는 수열) | Gates of Hell 개발자 변경 기록 steamcommunity.com/app/400750/discussions/0/4498514133983766024 (2026-10-12 확인) |  |
| `/item` | block | []x64 | 64 | 64 | 예: | 추정 | 아이템 정의의 최상위 블록 (추정: headgear.pattern 등이 이 블록으로 시작) | 파일 구조와 값에서 추론 | Jerry Can (20L) ¦ 55 gallon (~210L) drum |
| `/item/damage` | block | [atom]x1 [atom atom]x1 | 1 | 2 | blastwave ¦ add blastwave | 추정 | 피해 정의 한 건. 값은 피해 종류(blastwave, fire 등)이고 add가 붙으면 추가 피해 (추정: 탄약 파일의 {damage add blastwave ...}) | 파일 구조와 값에서 추론 | no blast damage. Used to ensure ground detonation fx are played ¦ = 41mm of penetration |
| `/item/damage/area` | block | [num num]x2 | 1 | 2 | 3 8 ¦ 8 12 | 추정 | 피해 반경 두 값. 안쪽과 바깥쪽으로 보인다 (추정: 값 0.4 0.5, 9 14) | 파일 구조와 값에서 추론 |  |
| `/item/damage/energy` | block | [num]x2 | 1 | 2 | 1.7 ~ 15 | 추정 | 폭발 에너지. blast.inc의 매크로가 탄 무게와 작약량으로 계산한다 (추정: set/blast.inc의 식) | 파일 구조와 값에서 추론 | = 25mm of penetration ¦ = 140mm of penetration |
| `/item/damage/ground_interaction_radius` | block | [num]x2 | 1 | 2 | 0 ~ 0.15 | 확인 | area의 둘째 값에 곱해, 폭발이 지면 이펙트를 낼 만큼 지면에 가까운지 판정한다 | 게임 파일 주석 | value is multiplied by 2nd value of area to deterimine is blast is close enough to ground to trigger terrain fx ¦ value is multiplied by 2nd value of area to determine is blast is close enough to ground to trigger terrain fx |
| `/item/desc` | block | [str]x3 | 3 | 3 | "desc/stuff/diesel" ¦ "desc/stuff/petrol" ¦ "Stuff not found" | 미확인(자동) |  |  |  |
| `/item/entity` | block | [str]x57 | 57 | 57 | 예: "eng_radio" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/item/filling` | block | [str num]x3 | 3 | 3 | "liquid" 210 ¦ "liquid" 20 ¦ "drug" 100 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/item/inventory` | block | []x65 | 64 | 65 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/item/inventory/block` | block | [num]x3 | 3 | 3 | 5 | 미확인(자동) |  |  |  |
| `/item/inventory/fsm` | block | [str]x57 | 57 | 57 | "stuff" ¦ "belt" ¦ "hand-only" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/item/inventory/link_bone` | block | [str]x15 | 15 | 15 | "backpacks" ¦ "head" ¦ "IK_UpDown" | 추정 | 착용했을 때 붙는 뼈대 이름 (추정: 값 "gun_back") | 파일 구조와 값에서 추론 |  |
| `/item/inventory/noview` | block | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |  |
| `/item/inventory/size` | block | [num num]x51 | 51 | 51 | 예: 3 4 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/item/inventory/weight` | block | [num]x57 | 56 | 57 | 1 ~ 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/item/mass` | block | [num]x62 | 62 | 62 | 0.05 ~ 20 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/item/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/item/tag` | block | [str]x59 [str str]x1 | 60 | 60 | 예: "itemin1hand" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/weapon_bombing` | block | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bombing/aimrange` | block | [num]x8 | 8 | 8 | 200 | 확인 | AI의 최대 사격 거리, 미터. {range 15 25}로 적을 때의 첫 값과 같다 | 게임 파일 주석 | meters ¦ Maximum fire range for AI. Expressed as {range 15 25} equals the same thing. |
| `/weapon_bombing/automatic` | block | []x8 | 8 | 8 | 예: | 추정 | 자동 사격 무기임을 표시 (추정: 이름. 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/weapon_bombing/burst` | block | []x1 | 1 | 1 | 예: | 추정 | 점사 설정 묶음 (추정: 하위 short, long) | 파일 구조와 값에서 추론 | multiplier to number of bullets in burst from weapon settings |
| `/weapon_bombing/burst/min` | block | [num]x1 | 1 | 1 | 2 | 미확인(자동) |  |  |  |
| `/weapon_bombing/cursorreloading` | block | [str]x8 | 8 | 8 | "gun_he" | 확인 | 직접 조종에서 장전 중일 때의 마우스 커서 | 게임 파일 주석 | rts reload cursor ¦ mouse cursor for weapon when reloading in direct control |
| `/weapon_bombing/filling` | block | [str num]x8 | 8 | 8 | "ammo air_bomb_eng_250lb" 2 ¦ "ammo air_bomb_eng_500lb" 2 ¦ "ammo air_bomb_ger_250kg" 1 ¦ "ammo air_bomb_ger_250kg" 2 ¦ "air_bomb_ger_50kg" 4 ¦ "air_bomb_100kg" 4 ¦ "ammo air_bomb_rus_250kg" 2 ¦ "ammo air_bomb_usa_500lb" 3 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_bombing/fillingvisible` | block | [str]x14 | 8 | 14 | "bomb" ¦ "bomb_" | 미확인(자동) |  |  |  |
| `/weapon_bombing/foresight` | block | [str]x8 | 8 | 8 | "pivot_front" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/weapon_bombing/preloaded` | block | []x8 | 8 | 8 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/weapon_bombing/rechargetime` | block | [num]x8 | 8 | 8 | 40 | 확인 | 탄창 또는 포탄을 장전하는 시간, 초. "reload time" 매크로가 seconds 인자로 넣는다 | 게임 파일 주석 |  |
| `/weapon_bombing/recoverytime` | block | [num]x8 | 8 | 8 | 0.4 | 확인 | 발사 간격, 초. "fire rate" 매크로가 60 / rpm으로 계산해 넣는다 | 게임 파일 주석 | increase fuel consumption by 2x |
| `/weapon_bombing/tag` | block | [str str]x8 | 8 | 8 | "weapon" "bomb" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/weapon_bullet` | block | []x5 | 5 | 5 | 예: | 추정 | 탄을 쏘는 무기 정의의 최상위 블록 (추정: 패턴 파일의 구조) | 파일 구조와 값에서 추론 | Example / Default Settings ¦ required for AT-Grenades with cumaltive damage |
| `/weapon_bullet/aimingtolerance` | block | [num]x4 | 4 | 4 | 3 ~ 10 | 확인 | AI가 사격을 허용받는 조준 오차, 도 | 게임 파일 주석 | Measure in degrees of IK error when AI is allowed to shoot ¦ degrees |
| `/weapon_bullet/calibre` | block | [num]x1 | 1 | 1 | 120 | 확인 | 구경, mm | 게임 파일 주석 | caliber of gun in millimeters ¦ AP shot caliber |
| `/weapon_bullet/clicksound` | block | [str]x1 | 1 | 1 | "" | 미확인(자동) |  |  |  |
| `/weapon_bullet/crosshair` | block | [str]x1 | 1 | 1 | "crosshair_mgun" | 확인 | 3인칭 조준 때의 조준선 이름 | 게임 파일 주석 | third person crosshair |
| `/weapon_bullet/crosshairreloading` | block | [str]x1 | 1 | 1 | "crosshair_mgun_reload" | 확인 | 3인칭 조준에서 재장전 중일 때의 조준선 이름 | 게임 파일 주석 | third person reload crosshair. |
| `/weapon_bullet/cursor` | block | [str]x3 | 3 | 3 | "ironsights/tank" ¦ "ironsights/rifle" ¦ "ironsights/binoculars" | 확인 | 직접 조종에서 조준할 때의 마우스 커서 | 게임 파일 주석 | rts cursor ¦ mouse cursor for weapon when aiming in direct control |
| `/weapon_bullet/curve` | block | [str]x1 | 1 | 1 | "gun_std" | 확인 | 거리별 피해 곡선 이름. set/ballistics.set을 가리킨다 | 게임 파일 주석 | Refers to ballistic.set ¦ profile for weapon damage over distance. Defined in ballistics.set |
| `/weapon_bullet/entity` | block | [str]x1 | 1 | 1 | "binocular" | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/weapon_bullet/filling` | block | [str num]x3 [str]x1 | 4 | 4 | "light" 0 ¦ "dummy ap" ¦ "ammo goliath" 1 | 추정 | 장전하는 탄의 태그와 탄창 용량 (추정: 값 "ammo rifle_rus" 10) | 파일 구조와 값에서 추론 | 2x 200 rounds ¦ Uses stick magazine on the model. |
| `/weapon_bullet/firesound` | block | [str]x1 | 1 | 1 | "" | 추정 | 발사음 경로 (추정: 이름과 값) | 파일 구조와 값에서 추론 | Placeholder |
| `/weapon_bullet/foresight` | block | [str]x4 | 4 | 4 | "foresight1" ¦ "Foresight3" ¦ "vision_rot" | 확인 | 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 | 게임 파일 주석 | bone for muzzleflash fx ¦ bone in weapon entity to define origin of fx and bullet |
| `/weapon_bullet/gravity` | block | [num]x2 | 2 | 2 | 0 | 추정 | 탄도에 적용하는 중력 값. 클수록 많이 휜다. 0이면 떨어지지 않는다 (추정: .presets의 "default gravity" 2, "aerial gravity" 1, gun/.presets의 standard_firing_arc 3.5·curved 5.5·artillery 6.0. 0은 쏘지 않는 binocular·commander_vision에만 있다) | 파일 구조와 값에서 추론 | same as other direct-fire "cannon" type weapons |
| `/weapon_bullet/inventory` | block | []x2 | 2 | 2 | 예: | 추정 | 인벤토리에서의 표시 설정 (추정: 하위 size, weight, fsm) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/inventory/fsm` | block | [str]x1 | 1 | 1 | "belt" | 문서 | 상태 기계 정의. 문자열은 이름 | GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인) |  |
| `/weapon_bullet/inventory/noview` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/inventory/size` | block | [num num]x1 | 1 | 1 | 2 1 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/weapon_bullet/inventory/weight` | block | [num]x1 | 1 | 1 | 100 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/weapon_bullet/mass` | block | [num]x2 | 2 | 2 | 0 ~ 1.5 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | with water ¦ empty weight |
| `/weapon_bullet/preloaded` | block | []x1 | 1 | 1 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/weapon_bullet/range` | block | [num num]x3 | 3 | 3 | 200 200 ¦ 4 4.5 | 문서 | 사거리 두 값, 미터. 첫 값은 AI가 스스로 교전하는 거리, 둘째 값은 실제 최대 사거리 | Steam 토론 steamcommunity.com/app/302670/discussions/0/135511259335716556, steamcommunity.com/app/244450/discussions/0/1743353164076238918 와 ModDB의 Men of War 모딩 FAQ가 같은 설명 (2026-10-12 확인) | 200m ¦ 285m max |
| `/weapon_bullet/speed` | block | [num]x3 | 3 | 3 | 500 ~ 1000 | 추정 | 탄속 (추정: 위와 같은 키) | 파일 구조와 값에서 추론 | velocity of bullet in meters per second ¦ Global muzzle velocity scale for all weapons in "gun" folder. |
| `/weapon_bullet/states` | block | []x1 | 1 | 1 | 예: | 추정 | 무기 동작별 보병 상태 이름 묶음 (추정: thrower.hand의 같은 구조) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/aim` | block | [str]x1 | 1 | 1 | "aim" | 추정 | 조준 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/states/can_aim` | block | [str]x1 | 1 | 1 | "can-aim" | 추정 | 조준 가능 판정 상태 이름 | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/tag` | block | [str]x3 | 3 | 3 | "gun" ¦ "weapon smallitemin1hand field_glass visor distributable" ¦ "visor" | 추정 | 분류 태그 문자열. 게임 코드와 이펙트 코드가 읽는다 (추정: 원문 주석 "game code", "fx code") | 게임 파일 주석 | game code ¦ fx code |
| `/weapon_bullet/thirdpersonparams` | block | [atom str]x1 | 1 | 1 | inherit "human default" | 미확인 | 값 inherit "human default" |  |  |
| `/weapon_bullet/thirdpersonparams/level` | block | [num]x1 []x1 | 1 | 2 | 1 | 추정 | 3인칭 조준의 확대 단계 하나. 값은 단계 번호 (추정: 값 1, 2와 하위 camera) | 파일 구조와 값에서 추론 | 2 |
| `/weapon_bullet/thirdpersonparams/level/aimer_type` | block | [str]x1 | 1 | 1 | "sniper" | 추정 | 조준 방식 이름 (추정: 값 "sniper") | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/blur_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/zoom_vignette_binoculars_mask" | 추정 | 가장자리 흐림 효과의 마스크 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/blur_strength` | block | [num]x1 | 1 | 1 | 10 | 추정 | 가장자리 흐림의 세기 (추정: 이름) | 파일 구조와 값에서 추론 | blur strength modifier |
| `/weapon_bullet/thirdpersonparams/level/camera` | block | [str]x2 | 1 | 2 | "human_zoom4" ¦ "human_zoom8" | 확인 | 이 단계에서 쓰는 카메라 프리셋. 배율을 정한다. human_zoom4는 4배 | 게임 파일 주석 | magnification x4 ¦ magnification x8 |
| `/weapon_bullet/thirdpersonparams/level/delta` | block | [num]x1 | 1 | 1 | 20 | 미확인(자동) |  |  | distance delta, effects number of steps in between near and far ¦ distance delta |
| `/weapon_bullet/thirdpersonparams/level/distance` | block | [num]x1 | 1 | 1 | 35 | 미확인(자동) |  |  | activation distance |
| `/weapon_bullet/thirdpersonparams/level/distanceset` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/drop_zoom_on_reload` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/hide_crosshair` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/hide_weapon` | block | [num]x1 | 1 | 1 | 1 | 추정 | 조준 중에 무기 모델을 숨길지 여부 (추정: 이름과 값 1) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/lens_dirt_mask` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/dirty_lens_scope_01" | 추정 | 렌즈 얼룩 효과에 쓰는 텍스처 (추정: 값이 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/linkbone` | block | [str]x1 | 1 | 1 | "head" | 추정 | 조준 카메라가 붙는 뼈대 (추정: 값 "head") | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/manualaccuracyzoom` | block | []x1 | 1 | 1 | 예: | 추정 | 직접 조종에서 확대했을 때 정확도 배율 (추정: 이름. 값 1.25) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/mouse_speed_factor` | block | [num]x2 | 1 | 2 | 0.6 ~ 0.8 | 추정 | 조준 중 마우스 감도 배수 (추정: 이름과 값 0.6 ~ 0.8) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/offset` | block | [num num num]x1 | 1 | 1 | 35 0 2.25 | 추정 | 표시 위치 오프셋 세 값 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/scope_vignette` | block | [str]x1 | 1 | 1 | "/texture/common/environment/lens_effects/zoom_vignette_binoculars" | 추정 | 조준경 테두리로 쓰는 텍스처 (추정: 값이 lens_effects 폴더의 텍스처 경로) | 파일 구조와 값에서 추론 |  |
| `/weapon_bullet/thirdpersonparams/level/vignette_fade_speed` | block | [num]x1 | 1 | 1 | 5 | 확인 | 조준경 테두리(비네트)가 나타나고 사라지는 속도 계수 | 게임 파일 주석 | factoring the speed at which the vignette fades in/out |
| `/weapon_bullet/thirdpersonparams/level/vignette_gloss` | block | [num]x1 | 1 | 1 | 10 | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/vignette_recoil` | block | [num num]x1 | 1 | 1 | 2.75 1.1 | 확인 | 반동 때 조준경 테두리가 흔들리는 속도와 크기 | 게임 파일 주석 | speed, scale |
| `/weapon_bullet/thirdpersonparams/level/vision_modes_active` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/weapon_sway_active` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/with_owner` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  | removes shield to prevent clipping |
| `/weapon_bullet/thirdpersonparams/level/with_owner/viewoff` | block | [str]x1 | 1 | 1 | "skin" | 미확인(자동) |  |  | removes shield to prevent clipping ¦ removes to prevent clipping |
| `/weapon_bullet/thirdpersonparams/level/with_weapon` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/with_weapon/viewon` | block | [str]x1 | 1 | 1 | "foresight3" | 미확인(자동) |  |  |  |
| `/weapon_bullet/thirdpersonparams/level/zoom` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |

