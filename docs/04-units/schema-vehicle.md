# 스키마: vehicle

이 파일은 `tools/New-SchemaDoc.ps1`이 생성한다. 손으로 고치지 않는다. 의미와 등급은 `data/annotations/vehicle.csv`에서 고친다.

## 원본 파일에 적힌 키

파일에 쓰인 그대로의 위치다. 고유 키 1055개.

### 파일 종류: .def

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/{}` | block | []x52 | 52 | 52 | 예: | 미확인(자동) |  |  |
| `/{}/(include)` | include | [str]x52 | 52 | 52 | 예: "../40mm_qf2/40mm_qf2.inc" | 미확인(자동) |  |  |
| `/{}/extender[inventory]` | block | [str]x49 | 49 | 49 | "inventory" | 미확인(자동) |  |  |
| `/{}/extender[inventory]/box` | block | []x49 | 49 | 49 | 예: | 미확인(자동) |  |  |
| `/{}/extender[inventory]/box/item` | block | [str num]x150 [str atom str num]x3 | 49 | 153 | 예: "bulleteng_40 ap2" 60 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/{}/extender[inventory]/box/item/user` | block | [str]x3 | 3 | 3 | "mgun" | 미확인(자동) |  |  |
| `/{}/props` | block | [str]x2 [str str str]x1 | 3 | 3 | "cannon_sandbag_standard" ¦ "mg34_lafette" "cannon_sandbag_standard_ai" "no_scavenge" | 미확인(자동) |  |  |
| `/{}/weaponry` | block | []x27 | 27 | 27 | 예: | 미확인(자동) |  |  |
| `/{}/weaponry/place` | block | [str]x27 | 27 | 27 | "gun" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/{}/weaponry/place/weapon` | block | [str atom str num]x27 | 27 | 27 | 예: "40mm_qf2" filling "bulleteng_40 ap2" 1 | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/actor` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/actor/("supply_ammo")` | macro | [call:supply]x5 | 5 | 5 | supply ( 250 ) | 미확인(자동) |  |  |
| `/actor/(include)` | include | [str]x11 | 6 | 11 | "/properties/ammo_box.ext" ¦ "/properties/resupply.inc" ¦ "/properties/stretcher.ext" | 미확인(자동) |  |  |
| `/actor/extender[inventory]` | block | [str]x6 | 6 | 6 | "inventory" | 미확인(자동) |  |  |
| `/actor/extender[inventory]/box` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/actor/extension` | block | [str]x6 | 6 | 6 | "inf_crate.mdl" ¦ "usa_stretcher.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/mass` | block | [num]x6 | 6 | 6 | 100 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/patherid` | block | [str]x1 | 1 | 1 | "cannon.strecher" | 미확인(자동) |  |  |
| `/actor/props` | block | [str str str str]x5 [str str]x1 | 6 | 6 | "vision_lev00_can" "ammo_crate" "no_recrew_logic" "sq_ammo_supply" ¦ "vision_lev00_can" "no_recrew_logic" | 미확인(자동) |  |  |
| `/actor/volume` | block | [str]x1 | 1 | 1 | "body1" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/material` | block | [str]x1 | 1 | 1 | "flesh" | 미확인(자동) |  |  |
| `/entity` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/("board_animation")` | macro | [call:place call:board_begin call:board_end]x1 | 1 | 1 | place ( driver ) board_begin ( board_body ) board_end ( seat_universal_driver ) | 미확인(자동) |  |  |
| `/entity/("car_thickness")` | macro | [call:body call:engine]x1 | 1 | 1 | body ( 2 ) engine ( 2 ) | 미확인(자동) |  |  |
| `/entity/("crew_human_v")` | macro | [call:place call:bone call:door call:animation call:group]x1 | 1 | 1 | place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) group ( crew ) | 미확인(자동) |  |  |
| `/entity/("crew_inside")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/("fuel")` | macro | [atom str]x1 | 1 | 1 | args "fuel" | 미확인(자동) |  |  |
| `/entity/("solid_wheel")` | macro | [atom str]x4 | 1 | 4 | args "wheelright1" ¦ args "wheelleft1" ¦ args "wheelright2" ¦ args "wheelleft2" | 미확인(자동) |  |  |
| `/entity/("transmission")` | macro | [atom str]x1 | 1 | 1 | args "transmission" | 미확인(자동) |  |  |
| `/entity/(include)` | include | [str]x1 [str call:scale]x1 | 1 | 2 | "/properties/car.ext" ¦ "/properties/selection/vehicle.inc" scale ( 0.8 ) | 미확인(자동) |  |  |
| `/entity/able` | block | [str]x1 | 1 | 1 | "emit_on_attack" | 미확인(자동) |  |  |
| `/entity/chassis` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/("car_trace")` | macro | [call:fx call:step call:wheell call:wheelr call:bonel call:boner]x1 | 1 | 1 | fx ( "tracks_sm" ) step ( 0.3 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) | 미확인(자동) |  |  |
| `/entity/chassis/("mobility_car")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x1 | 1 | 1 | speed ( 10 ) speedatmaxturn ( 8 ) reverse ( 8 ) weight ( 2.1 ) power ( 22 ) fuel ( 50 ) type ( fuel ) range ( 100 ) | 미확인(자동) |  |  |
| `/entity/extension` | block | [str]x1 | 1 | 1 | "universal.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/mass` | block | [num]x1 | 1 | 1 | 2100 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/props` | block | [str str str]x1 | 1 | 1 | "truck" "tractor" "tractor_heavy" | 미확인(자동) |  |  |
| `/entity/volume` | block | [str]x2 | 1 | 2 | "fuel" ¦ "transmission" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/volume/able` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/entity/volume/able/contact` | block | [num]x2 | 1 | 2 | 1 | 미확인(자동) |  |  |
| `/game_entity` | block | []x2384 | 2384 | 2384 | 예: | 추정(파일 주석) | june '41 ¦ sept 44 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/("ammo")` | macro | [atom str]x7 | 4 | 7 | args "ammo" ¦ args "ammo1" ¦ args "ammo2" ¦ args "ammo3" | 미확인(자동) |  |  |
| `/game_entity/("armor_carried_cannon")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/("armor_heavy_cannon")` | macro | []x50 | 50 | 50 | 예: | 미확인(자동) |  |  |
| `/game_entity/("armor_light_cannon")` | macro | []x41 | 41 | 41 | 예: | 미확인(자동) |  |  |
| `/game_entity/("armor_medium_cannon")` | macro | []x45 | 45 | 45 | 예: | 미확인(자동) |  |  |
| `/game_entity/("armoredcar_tier1")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/game_entity/("board_animation")` | macro | [call:place call:board_begin call:board_end call:base]x71 [call:place call:board_begin call:board_end]x4 | 38 | 75 | 예: place ( driver ) board_begin ( board_big-midcar ) board_end ( seat_driver_car ) base ( doorright ) | 미확인(자동) |  |  |
| `/game_entity/("body")` | macro | [call:repair]x3 | 3 | 3 | repair ( 45 ) | 미확인(자동) |  |  |
| `/game_entity/("bodyback")` | macro | [call:repair]x43 | 43 | 43 | repair ( 30 ) | 미확인(자동) |  |  |
| `/game_entity/("cannon_durability")` | macro | [call:body call:gun call:wheel call:shield]x2 | 2 | 2 | body ( 150 ) gun ( 40 ) wheel ( 100 ) shield ( 200 ) | 미확인(자동) |  |  |
| `/game_entity/("cannon_thickness")` | macro | [atom num]x3 | 3 | 3 | args 25 | 미확인(자동) |  |  |
| `/game_entity/("car_thickness")` | macro | [call:body call:engine]x18 | 18 | 18 | body ( 1.5 ) engine ( 1.5 ) ¦ body ( 2.5 ) engine ( 1.5 ) ¦ body ( 3 ) engine ( 2 ) | 미확인(자동) |  |  |
| `/game_entity/("charger_visible")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |
| `/game_entity/("charger_visible2")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/("charger2_visible")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("commander_visible")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/("component")` | macro | [call:volume call:component call:repair]x1 | 1 | 1 | volume ( "body" ) component ( "body" ) repair ( 10 ) | 미확인(자동) |  |  |
| `/game_entity/("crew_10_human")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_2_human_2")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_2_human_extended")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_2_human")` | macro | []x53 | 53 | 53 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_3_human_2_extended")` | macro | []x26 | 26 | 26 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_3_human_2")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_3_human_4")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_3_human_extended")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_4_human_2_extended")` | macro | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_4_human_2_extended2")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_4_human_2")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_4_human_3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_4_human_extended")` | macro | []x21 | 21 | 21 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_4_human")` | macro | []x21 | 21 | 21 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_5_human_2_extended")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_5_human_2")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_5_human_extended")` | macro | []x44 | 44 | 44 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_5_human_extended2")` | macro | []x62 | 62 | 62 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_5_human")` | macro | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_6_human_extended")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_7_human_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_7_human")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_carried_cannon_4")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_co_driver")` | macro | [call:place call:door call:door2 call:door3]x20 | 20 | 20 | place ( gunner3 ) door ( emit3 ) door2 ( emit1 ) door3 ( emit2 ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인(자동) |  |  |
| `/game_entity/("crew_driver")` | macro | [call:door call:door2 call:door3]x20 [call:door call:anm_left call:anm_right call:visor]x1 | 21 | 21 | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) ¦ door ( emit0 ) door2 ( emit2 ) door3 ( emit3 ) ¦ door ( emit0 ) anm_left ( driver ) anm_right ( driver ) visor ( driver ) ¦ door ( emit0 ) door2 ( emit2 ) door3 ( emit2 ) | 미확인(자동) |  |  |
| `/game_entity/("crew_human_2_v")` | macro | [call:place call:bone call:door call:door2]x2 [call:place call:bone call:door call:door]x1 | 3 | 3 | place ( driver ) bone ( driver ) door ( emit1 ) door2 ( emit2 ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) door ( emit4 ) | 미확인(자동) |  |  |
| `/game_entity/("crew_human_2")` | macro | [call:place call:bone call:door call:door2]x3 [call:place call:bone call:door call:door]x1 | 4 | 4 | place ( gunner ) bone ( gunner ) door ( emit3 ) door2 ( emit4 ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) door ( emit4 ) | 미확인(자동) |  |  |
| `/game_entity/("crew_human_v")` | macro | [call:place call:bone call:door call:animation call:group]x7 | 5 | 7 | place ( commander ) bone ( commander ) door ( emit2 ) animation ( commander ) group ( crew ) ¦ place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( passenger1 ) group ( passenger ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( passenger2 ) group ( passenger ) ¦ place ( seat1 ) bone ( seat1 ) door ( emit3 ) animation ( passenger1 ) group ( passenger ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) animation ( gunner ) group ( crew ) | 미확인(자동) |  |  |
| `/game_entity/("crew_human_visible_anm")` | macro | [call:place call:door call:door2 call:visor call:anm_left call:anm_right]x62 [call:place call:door call:visor call:anm_left call:anm_right]x15 [call:place call:visor call:extra call:off]x10 [call:place call:door call:door2 call:anm_left call:anm_right call:visor]x1 | 29 | 88 | 예: place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( main ) anm_left ( driver ) anm_right ( driver ) | 미확인(자동) |  |  |
| `/game_entity/("crew_human_visible_anm2")` | macro | [call:place call:door call:door2 call:anm_left call:anm_right]x49 [call:place call:door call:door2 call:visor call:anm_left call:anm_right]x15 | 24 | 64 | 예: place ( driver ) door ( emit2 ) door2 ( emit1 ) anm_left ( right ) anm_right ( right ) | 미확인(자동) |  |  |
| `/game_entity/("crew_human_visible")` | macro | [call:place call:door call:visor]x2 | 1 | 2 | place ( gunner ) door ( emit2 ) visor ( main ) ¦ place ( commander ) door ( emit3 ) visor ( around ) | 미확인(자동) |  |  |
| `/game_entity/("crew_human")` | macro | [call:place call:bone call:door call:animation call:group]x124 [call:place call:door call:door2 call:visor]x48 [call:place call:door call:bone call:animation call:group call:off]x13 [call:place call:visor]x4 [call:place call:visor call:animation]x4 | 91 | 201 | 예: place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) group ( crew ) | 미확인(자동) |  |  |
| `/game_entity/("crew_inside")` | macro | []x12 | 12 | 12 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_open")` | macro | [call:place call:group call:bone call:door call:animation call:extra]x6 | 3 | 6 | place ( driver ) group ( crew ) bone ( driver ) door ( emit1 ) animation ( driver ) extra ( ) ¦ place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) | 미확인(자동) |  |  |
| `/game_entity/("crew_wheeled_cannon_2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_wheeled_cannon_3")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_wheeled_cannon_4")` | macro | []x79 | 79 | 79 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_wheeled_cannon_5")` | macro | []x30 | 30 | 30 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_wheeled_cannon_6")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_wheeled_cannon_closed")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew_wheeled_cannon")` | macro | []x19 | 19 | 19 | 예: | 미확인(자동) |  |  |
| `/game_entity/("crew")` | macro | [call:place call:bone call:door call:animation call:visor call:off]x103 [call:place call:door call:bone call:animation call:group call:off]x3 | 32 | 106 | 예: place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) visor ( driver ) off ( { sensor } ) | 미확인(자동) |  |  |
| `/game_entity/("double_engine")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/("engine")` | macro | [call:repair]x2 | 2 | 2 | repair ( 30 ) | 미확인(자동) |  |  |
| `/game_entity/("fighter_tier1")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/("fighter_tier2")` | macro | []x14 | 14 | 14 | 예: | 미확인(자동) |  |  |
| `/game_entity/("fuel")` | macro | [atom str]x85 | 73 | 85 | args "fuel" ¦ args "fuel1" ¦ args "fuel2" ¦ args "transmission" | 미확인(자동) |  |  |
| `/game_entity/("general_durability")` | macro | [call:body call:engine call:turret call:mantlet call:gun call:track call:ammo]x2 | 2 | 2 | body ( 150 ) engine ( 85 ) turret ( 1 ) mantlet ( 1 ) gun ( 1 ) track ( 100 ) ammo ( 120 ) | 미확인(자동) |  |  |
| `/game_entity/("glass")` | macro | [atom str]x337 | 72 | 337 | 예: args "glass1" | 미확인(자동) |  |  |
| `/game_entity/("gun")` | macro | [call:repair call:thickness]x149 | 149 | 149 | repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ repair ( 30 ) thickness ( 6 ) ¦ repair ( 30 ) thickness ( 2 ) ¦ repair ( 30 ) thickness ( 50 ) ¦ repair ( 30 ) thickness ( 10 ) ¦ repair ( 45 ) thickness ( 50 ) | 미확인(자동) |  |  |
| `/game_entity/("gun1")` | macro | [call:repair call:thickness]x1 | 1 | 1 | repair ( 60 ) thickness ( 120 ) | 미확인(자동) |  |  |
| `/game_entity/("gun2")` | macro | [call:repair call:thickness]x1 | 1 | 1 | repair ( 60 ) thickness ( 120 ) | 미확인(자동) |  |  |
| `/game_entity/("gunner_open")` | macro | [call:place call:bone call:visorbone call:door call:door2 call:anm_left call:anm_right call:group]x27 | 27 | 27 | place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit4 ) door2 ( emit5 ) anm_left ( gunner2 ) anm_right ( gunner2 ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit3 ) door2 ( emit4 ) anm_left ( gunner2 ) anm_right ( gunner2 ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit5 ) door2 ( emit7 ) anm_left ( left ) anm_right ( right ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit4 ) door2 ( emit7 ) anm_left ( gunner2 ) anm_right ( gunner2 ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit3 ) door2 ( emit4 ) anm_left ( left ) anm_right ( right ) group ( passenger ) | 미확인(자동) |  |  |
| `/game_entity/("gunner_visible")` | macro | []x17 | 17 | 17 | 예: | 미확인(자동) |  |  |
| `/game_entity/("gunner_visible2")` | macro | []x12 | 12 | 12 | 예: | 미확인(자동) |  |  |
| `/game_entity/("lamp")` | macro | [atom str]x2 | 2 | 2 | args "lamp" | 미확인(자동) |  |  |
| `/game_entity/("marine_durability")` | macro | [call:body call:engine call:gun]x1 | 1 | 1 | body ( 100 ) engine ( 60 ) gun ( 100 ) | 미확인(자동) |  |  |
| `/game_entity/("mp_gun_limits")` | macro | []x12 | 12 | 12 | 예: | 미확인(자동) |  |  |
| `/game_entity/("mp_mgun_limits")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |
| `/game_entity/("passenger_10_human")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/("passenger_12_human")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/("passenger_6_human")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/("passenger_8_human")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("passenger_c47")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("passenger_horsa")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("passenger_human")` | macro | [call:place call:bone call:door call:animation call:group]x22 | 4 | 22 | 예: place ( seat1 ) bone ( seat01 ) door ( emit1 ) animation ( passenger1 ) group ( passenger ) | 미확인(자동) |  |  |
| `/game_entity/("passenger")` | macro | [call:place call:bone call:door call:animation call:off]x72 | 16 | 72 | 예: place ( seat1 ) bone ( seat01 ) door ( emit1 ) animation ( passenger1 ) off ( ) | 미확인(자동) |  |  |
| `/game_entity/("recon_tier1")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("recon_tier2")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("seat_human")` | macro | [call:place call:bone call:door call:animation]x1296 [call:place call:bone call:door call:animation call:group]x24 | 300 | 1320 | 예: place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( armor1 ) | 미확인(자동) |  |  |
| `/game_entity/("shield_cover")` | macro | []x34 | 34 | 34 | 예: | 미확인(자동) |  |  |
| `/game_entity/("shield_group")` | macro | [atom str num str]x182 | 51 | 182 | 예: args "shield1" 5 "shield" | 미확인(자동) |  |  |
| `/game_entity/("shield")` | macro | [atom str num]x101 [atom str num str]x10 | 69 | 111 | 예: args "shield" 8 "shield" | 미확인(자동) |  |  |
| `/game_entity/("single_engine")` | macro | []x14 | 14 | 14 | 예: | 미확인(자동) |  |  |
| `/game_entity/("solid_wheel")` | macro | [atom str]x116 | 56 | 116 | args "wheelright" ¦ args "wheelleft" ¦ args "wheelright1" ¦ args "wheelleft1" ¦ args "wheelright2" ¦ args "wheelleft2" | 미확인(자동) |  |  |
| `/game_entity/("supply_ammo")` | macro | [call:supply]x5 | 5 | 5 | supply ( 500 ) ¦ supply ( 250 ) | 미확인(자동) |  |  |
| `/game_entity/("supply_engineer_light")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("supply_engineer")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/("supply_fuel")` | macro | [call:liters]x6 | 6 | 6 | liters ( 2500 ) ¦ liters ( 2840 ) ¦ liters ( 2050 ) | 미확인(자동) |  |  |
| `/game_entity/("tank_light_tier1")` | macro | []x7 | 7 | 7 | 예: | 미확인(자동) |  |  |
| `/game_entity/("tank_medium_tier2")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |
| `/game_entity/("tank_medium_tier3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/("tracks")` | macro | [call:thickness]x357 [call:repair call:thickness]x19 | 376 | 376 | thickness ( 20 ) ¦ thickness ( 15 ) ¦ thickness ( 30 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ thickness ( 10 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 15 ) thickness ( 5 ) ¦ repair ( 45 ) thickness ( 30 ) ¦ thickness ( 21 ) | 미확인(자동) |  |  |
| `/game_entity/("train_durability")` | macro | [call:body call:engine call:turret call:mantlet call:gun]x3 | 3 | 3 | body ( 500 ) engine ( 250 ) turret ( 300 ) mantlet ( 225 ) gun ( 225 ) | 미확인(자동) |  |  |
| `/game_entity/("transmission")` | macro | [atom str]x64 | 64 | 64 | args "transmission" | 미확인(자동) |  |  |
| `/game_entity/("truck_ammo_heavy_2x")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/("truck_ammo_heavy")` | macro | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |
| `/game_entity/("turret")` | macro | [call:repair]x8 | 8 | 8 | repair ( 45 ) | 미확인(자동) |  |  |
| `/game_entity/("wheel")` | macro | [atom str]x610 | 191 | 610 | 예: args "wheelr" | 미확인(자동) |  |  |
| `/game_entity/("wing")` | macro | [call:number call:component call:tag]x6 | 1 | 6 | number ( r2 ) component ( right2 ) tag ( wing ) ¦ number ( l2 ) component ( left2 ) tag ( wing ) ¦ number ( rt1 ) component ( right_top ) tag ( wing ) ¦ number ( rt2 ) component ( right2_top ) tag ( wing ) ¦ number ( lt1 ) component ( left_top ) tag ( wing ) ¦ number ( lt2 ) component ( left2_top ) tag ( wing ) | 미확인(자동) |  |  |
| `/game_entity/("wood")` | macro | [atom str]x158 | 29 | 158 | 예: args "bodyback1" | 미확인(자동) |  |  |
| `/game_entity/(define "armor_light")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "armor_light")/<bare>` | bare | [atom]x1 [num]x1 | 1 | 2 | 0.01 | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm` | block | [str]x20 | 1 | 20 | 예: "1_gunner" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/forward` | block | []x20 | 1 | 20 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/forward/begin` | block | [str]x10 | 1 | 10 | "board_thor_front" ¦ "board_thor_rear" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/forward/end` | block | [str num]x10 [str]x10 | 1 | 20 | "pose_thor_gunner_animation" 10 ¦ "pose_thor_gunner2_animation" 10 ¦ "pose_thor_charger_animation" 10 ¦ "pose_thor_charger2_animation" 10 ¦ "pose_watch_binocular" 10 ¦ "seat_thor_driver1" ¦ "seat_thor_driver2" ¦ "seat_thor_driver3" ¦ "seat_thor_driver4" ¦ "pose_watch_binocular" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/reverse` | block | []x20 | 1 | 20 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/reverse/end` | block | [str]x20 | 1 | 20 | "emit_tank_2_hold" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder/door` | block | [str]x3 | 1 | 3 | "emit1" ¦ "emit2" ¦ "emit3" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link` | block | [str str]x8 | 1 | 8 | "emit1" "%place" ¦ "emit2" "%place" ¦ "emit3" "%place" ¦ "driver1" "gunner" ¦ "driver2" "gunner2" ¦ "driver3" "charger" ¦ "driver4" "charger2" ¦ "driver5" "commander" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link/anm` | block | [str]x3 | 1 | 3 | "2_%place" ¦ "1_%place" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link/forward` | block | [atom]x3 | 1 | 3 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link/reverse` | block | [atom]x3 | 1 | 3 | puton | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/placer` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human_visible_anm")/placer/place` | block | [str param]x1 | 1 | 1 | "%place" %extra | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/group` | block | [str]x1 | 1 | 1 | "crew" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/linkbone` | block | [str]x1 | 1 | 1 | "%place" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/turnoff` | block | [param]x1 | 1 | 1 | %off | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/visor` | block | [str]x1 | 1 | 1 | "%visor" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")` | define | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/boarder` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/boarder/door` | block | [str]x11 | 6 | 11 | "emit1" ¦ "emit2" ¦ "%door" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/boarder/link` | block | [str str]x11 | 6 | 11 | "emit1" "%place" ¦ "emit2" "%place" ¦ "%door" "%place" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/boarder/link/anm` | block | [str]x11 | 6 | 11 | "%animation" ¦ "%place" ¦ "left%animation" ¦ "right%animation" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human")/boarder/link/forward` | block | [atom]x11 | 6 | 11 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human")/boarder/link/reverse` | block | [atom]x11 | 6 | 11 | puton | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/placer` | block | []x6 | 6 | 6 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human")/placer/place` | block | [str]x6 | 6 | 6 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew_human")/placer/place/group` | block | [str]x6 | 6 | 6 | "crew" ¦ "%group" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/placer/place/linkbone` | block | [str]x4 | 4 | 4 | "%bone" ¦ "%place" | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/placer/place/turnoff` | block | []x3 [param]x3 | 6 | 6 | ¦ %off | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/placer/place/turnoff/sensor` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/placer/place/turnoff/shadow` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew_human")/placer/place/visor` | block | [str]x4 | 4 | 4 | "%visor" | 미확인(자동) |  |  |
| `/game_entity/(define "crew")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/boarder` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/boarder/door` | block | [str]x2 | 1 | 2 | "emit1" ¦ "emit2" | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/boarder/link` | block | [str str]x2 | 1 | 2 | "emit1" "%place" ¦ "emit2" "%place" | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/boarder/link/anm` | block | [str]x2 | 1 | 2 | "%animation" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew")/boarder/link/forward` | block | [atom]x2 | 1 | 2 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew")/boarder/link/reverse` | block | [atom]x2 | 1 | 2 | puton | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/placer` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew")/placer/place` | block | [str]x1 | 1 | 1 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(define "crew")/placer/place/group` | block | [str]x1 | 1 | 1 | "%group" | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/placer/place/linkbone` | block | [str]x1 | 1 | 1 | "%bone" | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/placer/place/turnoff` | block | [param]x1 | 1 | 1 | %off | 미확인(자동) |  |  |
| `/game_entity/(define "crew")/placer/place/turnoff/shadow` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "restore_ik_time")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/(define "restore_ik_time")/restoreiktime` | block | [num]x1 | 1 | 1 | 4 | 미확인(자동) |  |  |
| `/game_entity/(include)` | include | [str]x2013 [str call:scale]x650 | 1572 | 2663 | 예: "/properties/airborne.ext" | 미확인(자동) |  |  |
| `/game_entity/(mod)` | mod | [str]x46 [atom str]x20 | 66 | 66 | "mp" ¦ not "mp" | 미확인(자동) |  |  |
| `/game_entity/##bone` | block | [str]x43 | 43 | 43 | "commander_rot" | 미확인(자동) |  |  |
| `/game_entity/##bone/speed2` | block | [num]x43 | 43 | 43 | 180 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##extender` | block | [str]x1 | 1 | 1 | "smokescreen" | 추정(파일 주석) | sources say that these were not installed in the vehicles delivered to the Finns. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##extender/cooldown` | block | [num]x1 | 1 | 1 | 5 | 추정(파일 주석) | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##extender/itemtags` | block | [str]x1 | 1 | 1 | "grenade nbks" | 미확인(자동) |  |  |
| `/game_entity/##sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/##sensor/visor` | block | [str]x2 | 1 | 2 | "main" ¦ "around" | 미확인(자동) |  |  |
| `/game_entity/##sensor/visor/bone` | block | [str]x1 | 1 | 1 | "" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##sensor/visor/vision` | block | [str]x2 | 1 | 2 | "" ¦ "vehicle_around" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##volume` | block | [str]x2 | 2 | 2 | "engine3" | 추정(파일 주석) | Engine Cover | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##volume/thickness` | block | [num]x2 | 2 | 2 | 10 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/able` | block | [str]x129 [str str]x1 | 130 | 130 | "emit_on_attack" ¦ "emit_passangers_on_enemy" ¦ "emit_on_attack" "emit_crew_on_injure" | 미확인(자동) |  |  |
| `/game_entity/autooperatable` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/game_entity/boarder` | block | []x597 | 595 | 597 | 예: | 미확인(자동) |  |  |
| `/game_entity/boarder/anm` | block | [str]x3896 | 595 | 3896 | 예: "driver" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/anm/forward` | block | []x3896 | 595 | 3896 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/anm/forward/base` | block | [str]x647 [str num]x1 | 326 | 648 | 예: "open" | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/forward/begin` | block | [str]x2080 | 409 | 2080 | 예: "board_plane_left" | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/forward/end` | block | [str]x1899 [str num]x1811 | 593 | 3710 | 예: "seat_driver_car" | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/reverse` | block | []x2081 | 410 | 2081 | 예: | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/reverse/base` | block | [str]x412 [str num]x235 | 318 | 647 | 예: "open" -1 | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/reverse/end` | block | [str]x1719 [str num]x361 | 409 | 2080 | 예: "board_plane_left" -1 | 미확인(자동) |  |  |
| `/game_entity/boarder/door` | block | [str]x5 | 3 | 5 | "emit" ¦ "emit1" ¦ "emit0" | 미확인(자동) |  |  |
| `/game_entity/boarder/link` | block | [str str]x8 | 6 | 8 | "driver" "gunner" ¦ "emit" "driver" ¦ "emit1" "driver" ¦ "emit0" "driver" | 미확인(자동) |  |  |
| `/game_entity/boarder/link/anm` | block | [str]x5 | 3 | 5 | "driver_left" ¦ "driver_right" ¦ "driver" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/link/forward` | block | [atom]x5 | 3 | 5 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/link/reverse` | block | [atom]x5 | 3 | 5 | puton | 미확인(자동) |  |  |
| `/game_entity/bone` | block | [str]x1660 | 597 | 1660 | 예: "gun_rot" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/bone/("turret_heavy")` | macro | [call:power_traverse]x27 | 27 | 27 | power_traverse ( 6 ) ¦ power_traverse ( 12 ) ¦ power_traverse ( 23.7 ) ¦ power_traverse ( 24 ) ¦ power_traverse ( 16 ) ¦ power_traverse ( 33 ) ¦ power_traverse ( 18 ) ¦ power_traverse ( 0 ) | 미확인(자동) |  |  |
| `/game_entity/bone/("turret_light")` | macro | [call:power_traverse]x94 | 94 | 94 | power_traverse ( 0 ) ¦ power_traverse ( 24 ) ¦ power_traverse ( 14 ) ¦ power_traverse ( 18 ) ¦ power_traverse ( 55 ) | 미확인(자동) |  |  |
| `/game_entity/bone/("turret_medium")` | macro | [call:power_traverse]x132 | 132 | 132 | power_traverse ( 24 ) ¦ power_traverse ( 19 ) ¦ power_traverse ( 0 ) ¦ power_traverse ( 25 ) ¦ power_traverse ( 16 ) ¦ power_traverse ( 6 ) ¦ power_traverse ( 20 ) ¦ power_traverse ( 36 ) ¦ power_traverse ( 14 ) | 미확인(자동) |  |  |
| `/game_entity/bone/limits` | block | [num num]x973 | 563 | 973 | 예: -15 15 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/bone/speed2` | block | [num]x1400 | 592 | 1400 | 1.5 ~ 360 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/brain` | block | []x50 | 50 | 50 | 예: | 미확인(자동) |  |  |
| `/game_entity/brain/properties` | block | []x50 | 50 | 50 | 예: | 미확인(자동) |  |  |
| `/game_entity/brain/properties/fire_mode` | block | [atom]x50 | 50 | 50 | hold | 미확인(자동) |  |  |
| `/game_entity/chassis` | block | []x412 [str]x303 | 684 | 715 | ¦ "track" ¦ "wheel" ¦ "airborne" ¦ "water" ¦ "human_carrier" ¦ "railway" | 추정(파일 주석) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/("car_trace")` | macro | [call:fx call:step call:wheell call:wheelr call:bonel call:boner]x438 | 305 | 438 | 예: fx ( "tracewheel05" ) step ( 0.07 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("chassis_defaults")` | macro | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/("flight_model")` | macro | [call:speed call:climb_speed call:turn call:turnmp call:aerodynamics call:weight call:power call:fuel call:range call:roll_rate call:roll_angle]x20 | 20 | 20 | 예: speed ( 375 ) climb_speed ( 6 ) turn ( 200 ) turnMP ( 33 ) aerodynamics ( 1.20 ) weight ( 6000 ) power ( 1840 ) fuel ( 1060 ) range ( 2350 ) roll_rate ( 50 ) roll_angle ( 65 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_btr")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x41 | 41 | 41 | 예: speed ( 58 ) speedatmaxturn ( 25 ) reverse ( 12 ) weight ( 11.0 ) power ( 105 ) fuel ( 95 ) type ( diesel ) range ( 400 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_cannon")` | macro | [call:speed call:weight]x181 | 181 | 181 | 예: speed ( 10.3 ) weight ( 147 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_car")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x108 | 108 | 108 | 예: speed ( 65 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 6.968 ) power ( 72 ) fuel ( 145 ) type ( fuel ) range ( 450 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_goliath")` | macro | [call:speed call:reverse call:traverse call:weight call:power call:track call:fuel call:type call:range]x2 | 2 | 2 | speed ( 10 ) reverse ( 7 ) traverse ( 80 ) weight ( 0.370 ) power ( 6.85 ) track ( 10 ) fuel ( 2 ) type ( fuel ) range ( 32.0 ) ¦ speed ( 11.5 ) reverse ( 8 ) traverse ( 80 ) weight ( 0.430 ) power ( 12.5 ) track ( 10 ) fuel ( 6 ) type ( fuel ) range ( 60 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_medium_boat")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x3 | 3 | 3 | speed ( 17 ) speedatmaxturn ( 8 ) reverse ( 6 ) weight ( 50 ) power ( 400 ) fuel ( 6630 ) type ( fuel ) range ( 547 ) ¦ speed ( 65 ) speedatmaxturn ( 20 ) reverse ( 20 ) weight ( 1380 ) power ( 34000 ) fuel ( 6630 ) type ( fuel ) range ( 8900 ) ¦ speed ( 48 ) speedatmaxturn ( 20 ) reverse ( 20 ) weight ( 50.6 ) power ( 2520 ) fuel ( 6630 ) type ( fuel ) range ( 547 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_small_boat")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x2 | 2 | 2 | speed ( 6 ) speedatmaxturn ( 6 ) reverse ( 6 ) weight ( 0.15 ) power ( 1 ) fuel ( 0.1 ) type ( fuel ) range ( 100000 ) ¦ speed ( 19 ) speedatmaxturn ( 13 ) reverse ( 10 ) weight ( 13.0 ) power ( 120 ) fuel ( 5000 ) type ( fuel ) range ( 500 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_tank_slow")` | macro | [call:speed call:reverse call:traverse call:weight call:power call:track call:fuel call:type call:range]x4 | 4 | 4 | speed ( 10 ) reverse ( 6 ) traverse ( 20 ) weight ( 33.0 ) power ( 425 ) track ( 4 ) fuel ( 606 ) type ( fuel ) range ( 160 ) ¦ speed ( 7.8 ) reverse ( 4 ) traverse ( 20 ) weight ( 6.7 ) power ( 39 ) track ( 25.0 ) fuel ( 95 ) type ( fuel ) range ( 35 ) ¦ speed ( 7.8 ) reverse ( 4 ) traverse ( 20 ) weight ( 6.5 ) power ( 39 ) track ( 25.0 ) fuel ( 95 ) type ( fuel ) range ( 35 ) ¦ speed ( 10 ) reverse ( 5 ) traverse ( 10 ) weight ( 57.26 ) power ( 350 ) track ( 4 ) fuel ( 660 ) type ( fuel ) range ( 100 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_tank")` | macro | [call:speed call:reverse call:traverse call:weight call:power call:track call:fuel call:type call:range]x312 | 312 | 312 | 예: speed ( 48 ) reverse ( 20 ) traverse ( 35 ) weight ( 3.55 ) power ( 85 ) track ( 5 ) fuel ( 91 ) type ( fuel ) range ( 150 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("mobility_train")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x1 | 1 | 1 | speed ( 72 ) speedatmaxturn ( 17 ) reverse ( 30 ) weight ( 8.2 ) power ( 105 ) fuel ( 145 ) type ( fuel ) range ( 200 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("tank_trace_ex")` | macro | [call:fx call:step call:len call:boneleft call:boneright]x40 | 40 | 40 | fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.58 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.49 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.29 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.46 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.50 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.82 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.7 ) boneleft ( "wheell4" ) boneright ( "wheelr4" ) ¦ fx ( "tracks_sm2" ) step ( 0.1 ) len ( 0.6 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.8 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( -0.49 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.8 ) boneleft ( "wheell4" ) boneright ( "wheelr4" ) | 미확인(자동) |  |  |
| `/game_entity/chassis/("tank_trace")` | macro | [call:fx call:step call:len]x317 [call:fx call:step call:len call:boneleft call:boneright]x3 | 320 | 320 | 예: fx ( "tracks_mid" ) step ( 0.32 ) len ( 0.5 ) | 미확인(자동) |  |  |
| `/game_entity/chassis/(mod)` | mod | [atom str]x12 | 12 | 12 | not "mp" | 미확인(자동) |  |  |
| `/game_entity/chassis/disablemovewhilefiring` | block | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/fuelbag` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/fuelbag/fuel` | block | [str]x1 | 1 | 1 | "fuel" | 미확인(자동) |  |  |
| `/game_entity/chassis/fuelbag/rate` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/game_entity/chassis/fuelbag/remain` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/game_entity/chassis/fuelbag/volume` | block | [num]x1 | 1 | 1 | 0 | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/ignorepatherbounds` | block | []x12 | 12 | 12 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion` | block | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/("locomotion")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/braketime` | block | [num]x7 | 7 | 7 | 3 ~ 15 | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/fast` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/maxspeed` | block | [num]x9 | 9 | 9 | 0 ~ 70 | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/normal` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/slow` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/starttime` | block | [num]x7 | 7 | 7 | 3 ~ 18 | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/turnradius` | block | [num]x4 | 4 | 4 | 4 ~ 20 | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/turntime` | block | [num]x3 | 3 | 3 | 45 | 미확인(자동) |  |  |
| `/game_entity/chassis/switch` | block | []x22 | 11 | 22 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/switch/exitfromwater` | block | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/switch/terrain` | block | [atom str str]x11 [str str]x11 | 11 | 22 | not "water" "deep_water" ¦ "water" "deep_water" | 미확인(자동) |  |  |
| `/game_entity/chassis/waterline` | block | [num]x18 | 18 | 18 | 0 ~ 1.6 | 미확인(자동) |  |  |
| `/game_entity/collider` | block | [str]x254 | 254 | 254 | "tank.light" ¦ "tank.heavy" ¦ "truck" ¦ "cannon_small" ¦ "car" ¦ "train" | 미확인(자동) |  |  |
| `/game_entity/component` | block | [str]x11 | 7 | 11 | "body" ¦ "wheels_left" ¦ "wheels_right" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/component/tags` | block | [str]x6 | 2 | 6 | "mover" | 미확인(자동) |  |  |
| `/game_entity/compoundobstacles` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/constrain` | block | [atom]x1 | 1 | 1 | none | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]` | block | [str]x188 | 188 | 188 | "cannon" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/##` | block | [atom]x1 | 1 | 1 | animation | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/##/aim` | block | [str str]x1 | 1 | 1 | "" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/##/fire` | block | [str str]x1 | 1 | 1 | "" "" | 추정(파일 주석) | mg_maxim_gunner_fire | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/##/move` | block | [str str]x1 | 1 | 1 | "" "" | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/##/reload` | block | [str str]x1 | 1 | 1 | "" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/##/switch` | block | [str]x1 | 1 | 1 | "close" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation` | block | []x187 | 187 | 187 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/aim` | block | [str str str str]x80 [str str]x49 [str str str str str]x38 [str str str]x9 [str str str str str str]x3 | 179 | 179 | 예: "mortar_82bm37_gunner_aim" "mortar_82bm37_commander_aim" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/fire` | block | [str str str str]x80 [str str]x41 [str str str str str]x37 [str str str]x9 [str str str str str str]x4 | 172 | 172 | 예: "mortar_82bm37_gunner_fire" "mortar_82bm37_commander_fire" | 추정(파일 주석) | mg_maxim_gunner_fire | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/animation/gunners` | block | [str str str str]x7 | 7 | 7 | "" "" "" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/move` | block | [str str str str]x81 [str str]x58 [str str str str str]x31 [str str str]x9 [str str str str str str]x4 | 187 | 187 | 예: "mortar_driver_r" "mortar_driver_l" | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/animation/reload` | block | [str str str str]x80 [str str]x49 [str str str str str]x37 [str str str]x9 [str str str str str str]x4 | 183 | 183 | 예: "" "mortar_82bm37_commander_reload" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/switch` | block | [str]x163 | 163 | 163 | "close" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/attack` | block | []x169 | 167 | 169 | 예: | 추정(파일 주석) | fire/attack state: cannon aim, fire, and reload events | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/attack/obstacle` | block | [str]x168 | 166 | 168 | "open" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/attack/workers` | block | [str str str str]x81 [str str str str str]x31 [str str str]x9 [str str str str str str]x4 [str str]x4 | 134 | 134 | "gunner" "charger" "charger2" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "commander" ¦ "gunner" "charger" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "charger3" "commander" ¦ "gunner" "commander" ¦ "gunner" ¦ "" "" "" "" "" ¦ "" "" "" ¦ clear | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/move` | block | []x176 | 174 | 176 | 예: | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/move/obstacle` | block | [str]x175 | 173 | 175 | "close" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/move/workers` | block | [str str str str atom num]x81 [str str str str str atom num]x29 [str str atom num]x17 [atom]x8 [str str str atom num]x7 | 149 | 149 | "driver1" "driver2" "driver3" "driver4" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 2 ¦ "driver1" "driver2" minWorkers 1 ¦ clear ¦ "driver1" "driver2" "driver3" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" "driver6" minWorkers 2 ¦ "driver1" "driver2" minWorkers 2 ¦ "driver" minWorkers 1 ¦ "driver1" "driver2" "driver3" "driver4" minWorkers 3 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 1 | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/rotate_ik_limit` | block | [num]x34 | 34 | 34 | 0 ~ 35 | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/trailing` | block | []x126 | 126 | 126 | 예: | 추정(파일 주석) | trailing state: cannon is being towed {obstacle} not needed. Uses "close" | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/trailing/workers` | block | [str str str str]x79 [str str str str str]x28 [str str str]x7 [str str str str str str]x4 [atom]x4 | 126 | 126 | "seat1" "seat2" "seat3" "seat4" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" ¦ "seat1" "seat2" "seat3" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" "seat6" ¦ clear ¦ "seat1" "seat2" | 미확인(자동) |  |  |
| `/game_entity/extender[detail]` | block | [str]x3 | 3 | 3 | "detail" | 미확인(자동) |  |  |
| `/game_entity/extender[detail]/detail1` | block | [num]x3 | 3 | 3 | 0 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/extender[detail]/random` | block | [num]x2 | 2 | 2 | 0 | 미확인(자동) |  |  |
| `/game_entity/extender[edifice]` | block | [str]x1 | 1 | 1 | "edifice" | 미확인(자동) |  |  |
| `/game_entity/extender[enumerator]` | block | [str]x328 | 328 | 328 | "enumerator" | 미확인(자동) |  |  |
| `/game_entity/extender[enumerator]/digit_folder` | block | [str]x328 | 328 | 328 | "us2" ¦ "ru1" ¦ "ger" ¦ "np" ¦ "ger2" ¦ "moe" ¦ "np_w" ¦ "us3" ¦ "us1" ¦ "ru3" ¦ "c47" ¦ "ru2" | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]` | block | [str]x747 | 747 | 747 | "inventory" | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box` | block | []x747 | 747 | 747 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box/clear` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box/item` | block | [str num]x2510 [str atom]x86 [str atom str num]x85 [str str num]x18 [str atom str]x1 | 691 | 2700 | 예: "ammo mgun_usa belt" 1000 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[inventory]/box/item/user` | block | [str]x111 | 99 | 111 | "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box/size` | block | [num num]x14 | 14 | 14 | 9 12 ¦ 4 2 ¦ 0 0 ¦ 6 4 ¦ 9 6 ¦ 9 9 ¦ 6 6 ¦ 9 11 | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box/weight` | block | [num]x16 | 16 | 16 | 2100 ~ 100000 | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/weapon` | block | [str]x115 | 102 | 115 | "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[inventory]/weapon/mask` | block | [str]x115 | 102 | 115 | "mountable" | 미확인(자동) |  |  |
| `/game_entity/extender[land_patch]` | block | [str]x1 | 1 | 1 | "land_patch" | 미확인(자동) |  |  |
| `/game_entity/extender[land_patch]/terraintype` | block | [str str]x1 | 1 | 1 | "road" "bridge_steel" | 미확인(자동) |  |  |
| `/game_entity/extender[shield]` | block | [str]x208 | 206 | 208 | "shield" | 미확인(자동) |  |  |
| `/game_entity/extender[shield]/off` | block | []x13 | 13 | 13 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[shield]/on` | block | []x195 | 194 | 195 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[smokescreen]` | block | [str]x136 | 136 | 136 | "smokescreen" | 미확인(자동) |  |  |
| `/game_entity/extender[smokescreen]/cooldown` | block | [num]x136 | 136 | 136 | 5 ~ 20 | 추정(파일 주석) | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[smokescreen]/itemtags` | block | [str]x136 | 136 | 136 | "mortareng_50 sm" ¦ "mortareng_50 wp" ¦ "grenade no8_smoke" ¦ "grenade nbks" | 미확인(자동) |  |  |
| `/game_entity/extension` | block | [str]x2393 | 2324 | 2393 | 예: "blenheim_mk1.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/mass` | block | [num]x698 | 698 | 698 | 5.5 ~ 1380000 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/obstacleid` | block | [str]x2 | 2 | 2 | "unbreakable" | 미확인(자동) |  |  |
| `/game_entity/patherid` | block | [str]x563 | 563 | 563 | 예: "blenheim" | 미확인(자동) |  |  |
| `/game_entity/placer` | block | []x80 | 77 | 80 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/placer/crew` | block | [num]x3 | 3 | 3 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place` | block | [str]x124 [str atom]x52 | 77 | 176 | 예: "driver" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/placer/place/attach` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/entity` | block | [str]x40 | 10 | 40 | "lee_enfield_no1_mk3" ¦ "76mm_rus_he" ¦ "76mm_rus_apcr" ¦ "ammo_boxes_01" ¦ "ammo_boxes_04" ¦ "crate4" ¦ "ammo_boxes_03" | 미확인(자동) |  |  |
| `/game_entity/placer/place/group` | block | [str]x10 | 10 | 10 | "crew" | 미확인(자동) |  |  |
| `/game_entity/placer/place/handlebone` | block | [str]x46 | 13 | 46 | "body" ¦ "basis" | 미확인(자동) |  |  |
| `/game_entity/placer/place/linkbone` | block | [str]x48 | 15 | 48 | 예: "weapon1" | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff` | block | []x42 | 29 | 42 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff/sensor` | block | [num]x6 []x2 | 4 | 8 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff/shadow` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff/targetable` | block | []x30 [num]x10 | 27 | 40 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place/usevision` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place/visor` | block | [str]x56 | 33 | 56 | "main" ¦ "none" ¦ "around" ¦ "rear" ¦ "gunner" ¦ "main_2" ¦ "mgun1" ¦ "driver" | 미확인(자동) |  |  |
| `/game_entity/props` | block | [str str str str str str]x107 [str str str str str str str str str]x107 [str str str str str str str str]x100 [str str str str str]x87 [str str str str str str str]x80 | 727 | 727 | 예: "blenheim_mk1" "wheeled" "vision_lev02_air" | 미확인(자동) |  |  |
| `/game_entity/rimlight` | block | []x756 | 756 | 756 | 예: | 미확인(자동) |  |  |
| `/game_entity/sensor` | block | []x201 | 201 | 201 | 예: | 미확인(자동) |  |  |
| `/game_entity/sensor/visor` | block | [str]x301 | 201 | 301 | 예: "rear" | 미확인(자동) |  |  |
| `/game_entity/sensor/visor/bone` | block | [str]x160 | 121 | 160 | 예: "foresight6" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/sensor/visor/vision` | block | [str]x272 | 187 | 272 | 예: "airborne_main" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/simulation` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/simulation/able` | block | [num]x4 | 4 | 4 | 0 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/simulation/disablelinked` | block | [num]x3 | 3 | 3 | 0 | 미확인(자동) |  |  |
| `/game_entity/targetclass` | block | [str]x562 [str str]x12 | 574 | 574 | "tank_medium" ¦ "howitzer" ¦ "tank_light" ¦ "armoredcar" ¦ "tank_heavy" ¦ "cannon_big" ¦ "cannon_small" ¦ "car" ¦ "cannon_zenite" ¦ "armoredcar" "bullet" ¦ "cannon_bullet" ¦ "armoredcar_l" | 미확인(자동) |  |  |
| `/game_entity/targetselector` | block | [str]x579 | 579 | 579 | 예: "tank_light" | 미확인(자동) |  |  |
| `/game_entity/texmod` | block | [str]x17 | 17 | 17 | "late" ¦ "rus" ¦ "camo" ¦ "camo2" ¦ "grey" ¦ "2" ¦ "early" ¦ "red_ball" | 미확인(자동) |  |  |
| `/game_entity/volume` | block | [str]x16328 | 1421 | 16328 | 예: "gear_l" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/able` | block | []x2823 [atom]x33 | 1090 | 2856 | ¦ clear | 미확인(자동) |  |  |
| `/game_entity/volume/able/contact` | block | [num]x139 []x4 | 57 | 143 | 0 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/volume/able/contact_ground` | block | [num]x6 | 3 | 6 | 0 | 미확인(자동) |  |  |
| `/game_entity/volume/able/float` | block | []x17 | 3 | 17 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/hole` | block | []x3 | 2 | 3 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/holed` | block | []x1266 | 659 | 1266 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/nolook` | block | [num]x4 | 3 | 4 | 1 | 미확인(자동) |  |  |
| `/game_entity/volume/able/nowounddecal` | block | [num]x57 []x18 | 53 | 75 | 0 | 미확인(자동) |  |  |
| `/game_entity/volume/able/obstacle` | block | [num]x1435 | 493 | 1435 | 0 | 미확인(자동) |  |  |
| `/game_entity/volume/able/visible` | block | [num]x87 | 46 | 87 | 0 | 미확인(자동) |  |  |
| `/game_entity/volume/caststeel` | block | [num]x913 | 144 | 913 | 0.8063 | 추정(파일 주석) | effective armor reduction compared to solid plate ¦ effective armor reduction compared to solid plate; 25 + 75 spaced armor array ~86mm effective | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/component` | block | [str]x64 | 44 | 64 | "body" ¦ "turret" ¦ "gun" ¦ "engine" ¦ "bodyback" ¦ "gun1" ¦ "gun2" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/density` | block | [num]x9 | 7 | 9 | 0 ~ 0.1 | 미확인(자동) |  |  |
| `/game_entity/volume/highhardness` | block | []x209 | 37 | 209 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/material` | block | [str]x405 | 95 | 405 | "wood" ¦ "steel" ¦ "fabric" ¦ "glass" | 미확인(자동) |  |  |
| `/game_entity/volume/quality` | block | [str]x38 | 8 | 38 | "large_flaw" | 미확인(자동) |  |  |
| `/game_entity/volume/tags` | block | [str]x511 [str str]x6 | 159 | 517 | 예: "invisible" | 미확인(자동) |  |  |
| `/game_entity/volume/thickness` | block | [num]x14220 []x148 | 891 | 14368 | 0.5 ~ 406 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/bottom` | block | [num]x1710 | 330 | 1710 | 1 ~ 200 | 추정(파일 주석) | Overhang (above tracks) ¦ 4mm + .25" | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/front` | block | [num]x1849 | 345 | 1849 | 3 ~ 300 | 추정(파일 주석) | Glacis ¦ Lower front | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/left` | block | [num]x397 | 114 | 397 | 6 ~ 95 | 추정(파일 주석) | Top sides ¦ side | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/rear` | block | [num]x1172 | 325 | 1172 | 1 ~ 110 | 추정(파일 주석) | Front Roof Turret Armor ¦ Bottom Front Nose | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/right` | block | [num]x286 | 92 | 286 | 10 ~ 600 | 추정(파일 주석) | armor plate sides ¦ Top sides | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/top` | block | [num]x2120 | 372 | 2120 | 1 ~ 200 | 추정(파일 주석) | Glacis ¦ front upper part | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry` | block | []x655 | 655 | 655 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/("restore_ik_time")` | macro | []x497 | 497 | 497 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place` | block | [str]x51 | 51 | 51 | "mgun" ¦ "mgun2" ¦ "bomb1" | 추정(파일 주석) | maybe future feature | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/("abm_mgun")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/ai_in_manual` | block | []x48 | 48 | 48 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/barrels` | block | [num]x2 | 2 | 2 | 2 ~ 3 | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/basic` | block | []x51 | 51 | 51 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/charger` | block | [str]x7 | 7 | 7 | "driver" ¦ "gunner4" ¦ "commander" ¦ "gunner2" ¦ "gunner" | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/foresight` | block | [str]x45 | 45 | 45 | "vision_rot" ¦ "foresight2" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/gunner` | block | [str]x51 | 51 | 51 | "commander" ¦ "driver" ¦ "gunner4" ¦ "gunner2" ¦ "gunner" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/linkanimation` | block | [str]x4 | 4 | 4 | "off" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/linkbone` | block | [str]x4 | 4 | 4 | "mgun2" ¦ "mgun_rot" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/shooter` | block | [str]x2 | 2 | 2 | "shooter_bb" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/type` | block | [str]x48 | 48 | 48 | "commander" ¦ "rear" ¦ "aa" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/weapon` | block | [str]x44 [str atom]x2 [str atom str num]x1 | 47 | 47 | "commander_vision" ¦ "bomb_dropper_eng_250x2" filled ¦ "bomb_dropper_usa_250x3" filled ¦ "thrower_v" filling "f1 grenade" 1 | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place` | block | [str]x1313 | 654 | 1313 | 예: "mgun1" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/("abm_dymamic_aa")` | macro | [call:zeroing call:dispersion]x10 | 10 | 10 | zeroing ( 2.25 ) dispersion ( 0.50 ) ¦ zeroing ( 1.95 ) dispersion ( 0.24 ) ¦ zeroing ( 2.25 ) dispersion ( 0.3 ) ¦ zeroing ( 2.5 ) dispersion ( 0.50 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_aaa")` | macro | [call:zeroing call:dispersion]x29 | 28 | 29 | zeroing ( 1.2 ) dispersion ( 0.15 ) ¦ zeroing ( 1.95 ) dispersion ( 0.24 ) ¦ zeroing ( 2.9 ) dispersion ( 0.08 ) ¦ zeroing ( 2.5 ) dispersion ( 0.15 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_autocannon")` | macro | [call:zeroing call:dispersion]x10 | 10 | 10 | zeroing ( 2 ) dispersion ( 0.15 ) ¦ zeroing ( 2.0 ) dispersion ( 0.15 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_gun_boat")` | macro | [call:zeroing call:dispersion]x8 | 4 | 8 | zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_gun_poor")` | macro | [call:zeroing call:dispersion]x1 | 1 | 1 | zeroing ( 5.0 ) dispersion ( 0.35 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_gun")` | macro | [call:zeroing call:dispersion]x121 | 120 | 121 | 예: zeroing ( 5.0 ) dispersion ( 1 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_mortar")` | macro | [call:zeroing call:dispersion]x27 | 27 | 27 | zeroing ( 3.0 ) dispersion ( 1 ) ¦ zeroing ( 2.5 ) dispersion ( 1 ) ¦ zeroing ( 0.0 ) dispersion ( 0 ) ¦ zeroing ( 10.0 ) dispersion ( 1 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_poor")` | macro | [call:zeroing call:dispersion]x5 | 5 | 5 | zeroing ( 2.0 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) ¦ zeroing ( 4.5 ) dispersion ( 0.20 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_rocket")` | macro | [call:zeroing call:dispersion]x18 | 18 | 18 | zeroing ( 10.0 ) dispersion ( 1 ) ¦ zeroing ( 5.0 ) dispersion ( 1 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_stabilised")` | macro | [call:zeroing call:dispersion]x76 | 74 | 76 | zeroing ( 3 ) dispersion ( 0.3 ) ¦ zeroing ( 3.0 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) ¦ zeroing ( 2.8 ) dispersion ( 0.20 ) ¦ zeroing ( 3.1 ) dispersion ( 0.5 ) ¦ zeroing ( 1.75 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.25 ) ¦ zeroing ( 2.0 ) dispersion ( 0.20 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic_td")` | macro | [call:zeroing call:dispersion]x44 | 44 | 44 | zeroing ( 2.5 ) dispersion ( 0.25 ) ¦ zeroing ( 1.5 ) dispersion ( 0.25 ) ¦ zeroing ( 3.5 ) dispersion ( 0.25 ) ¦ zeroing ( 4.0 ) dispersion ( 0.25 ) ¦ zeroing ( 3.0 ) dispersion ( 0.25 ) ¦ zeroing ( 2.25 ) dispersion ( 0.25 ) ¦ zeroing ( 3.50 ) dispersion ( 0.25 ) ¦ zeroing ( 2.5 ) dispersion ( 0.2 ) ¦ zeroing ( 2.0 ) dispersion ( 0.25 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_dymamic")` | macro | [call:zeroing call:dispersion]x184 | 177 | 184 | 예: zeroing ( 3.0 ) dispersion ( 0.20 ) | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_mgun_boat")` | macro | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_mgun")` | macro | []x702 | 407 | 702 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/("abm_rear_mg")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/ai_in_manual` | block | []x443 | 290 | 443 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/barrels` | block | [num]x83 | 72 | 83 | 1 ~ 60 | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/basic` | block | []x798 | 441 | 798 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/charger` | block | [str]x1113 [str atom num]x153 | 618 | 1266 | 예: "driver" | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/foresight` | block | [str]x580 | 326 | 580 | 예: "foresight3" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/gunner` | block | [str]x1126 [str atom num]x38 | 511 | 1164 | 예: "driver" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/linkanimation` | block | [str]x176 | 155 | 176 | "off" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/linkbone` | block | [str]x186 | 160 | 186 | "mgun" ¦ "mgun2" ¦ "mgun_rot" ¦ "mgun1" ¦ "mgun3" ¦ "mgun_bone" ¦ "mgun4" ¦ "dshk_mgunF" ¦ "dshk_mgunB" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/restoreikafteraim` | block | []x506 | 485 | 506 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/rotate` | block | []x91 | 84 | 91 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/shooter` | block | [str]x36 | 21 | 36 | "shooter" ¦ "shooter_mg" ¦ "shooter_bb" ¦ "shooter_b" ¦ "shooter_r" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/stationaryfiring` | block | []x16 | 16 | 16 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/type` | block | [str]x726 | 414 | 726 | 예: "rear" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/weapon` | block | [str atom str num]x787 [str atom]x401 [str]x11 [str atom str]x3 | 616 | 1202 | 예: "browning_303_mk2" filling "ammo mgun_usa belt" 500 | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |

### 파일 종류: .ext

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/("ammo")` | macro | [atom str]x1077 [atom str num]x2 | 313 | 1079 | 예: args "ammo1" | 미확인(자동) |  |  |
| `/("armor")` | macro | [atom str num]x54 | 5 | 54 | 예: args "armor1" 4 | 미확인(자동) |  |  |
| `/("armoredcar_tier1")` | macro | []x54 | 54 | 54 | 예: | 미확인(자동) |  |  |
| `/("armoredcar_tier2")` | macro | []x40 | 40 | 40 | 예: | 미확인(자동) |  |  |
| `/("aux_turret")` | macro | [call:repair]x18 | 18 | 18 | repair ( 45 ) ¦ repair ( 30 ) | 미확인(자동) |  |  |
| `/("body")` | macro | [call:repair call:thickness]x1 [call:repair]x1 | 2 | 2 | repair ( 30 ) thickness ( 4 ) ¦ repair ( 30 ) | 미확인(자동) |  |  |
| `/("cabin")` | macro | [call:repair]x5 | 5 | 5 | repair ( 30 ) | 미확인(자동) |  |  |
| `/("component")` | macro | [call:volume call:component call:repair]x20 | 3 | 20 | 예: volume ( "body_front1" ) component ( "body_front" ) repair ( 10 ) | 미확인(자동) |  |  |
| `/("crew_commander")` | macro | [call:place call:bone call:visor call:off]x1 | 1 | 1 | place ( commander ) bone ( commander ) visor ( commander ) off ( { sensor } ) | 미확인(자동) |  |  |
| `/("crew_human_visible_anm")` | macro | [call:place call:linkbone call:door call:door2 call:door3 call:pose call:visor call:off]x4 [call:place call:linkbone call:door call:door2 call:door3 call:visor call:pose call:off]x1 [call:place call:bone call:visor]x1 | 2 | 6 | place ( gunner ) bone ( gunner ) visor ( main ) ¦ place ( driver ) linkbone ( ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) visor ( driver ) pose ( gunner ) off ( { sensor } ) ¦ place ( gunner ) linkbone ( gunner ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( gunner ) visor ( main ) off ( { sensor } ) ¦ place ( charger ) linkbone ( charger ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( charger ) visor ( none ) off ( { sensor } ) ¦ place ( charger2 ) linkbone ( charger2 ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( charger ) visor ( none ) off ( ) ¦ place ( commander ) linkbone ( commander ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( commander ) visor ( none ) off ( ) | 미확인(자동) |  |  |
| `/("crew_human_visible_anm2")` | macro | [call:place call:door call:door2 call:anm_left call:anm_right]x8 | 3 | 8 | place ( driver ) door ( emit1 ) door2 ( emit2 ) anm_left ( left_driver ) anm_right ( right_driver ) ¦ place ( gunner ) door ( emit1 ) door2 ( emit2 ) anm_left ( left_gunner ) anm_right ( right_gunner ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) anm_left ( left_commander ) anm_right ( right_commander ) ¦ place ( driver ) door ( emit1 ) door2 ( emit1 ) anm_left ( left ) anm_right ( left ) ¦ place ( commander ) door ( emit1 ) door2 ( emit1 ) anm_left ( right ) anm_right ( right ) | 미확인(자동) |  |  |
| `/("crew_human")` | macro | [call:place call:visor]x81 [call:place call:door call:bone call:animation call:group call:off]x47 [call:place call:door call:bone call:animation call:group]x41 [call:place call:bone call:door call:animation]x13 [call:place call:group call:door call:door2 call:visor]x8 | 56 | 224 | 예: place ( driver ) visor ( driver ) | 미확인(자동) |  |  |
| `/("crew_open")` | macro | [call:place call:visor]x1 | 1 | 1 | place ( commander ) visor ( top ) | 미확인(자동) |  |  |
| `/("crew")` | macro | [call:place call:door call:door2 call:door3 call:door4 call:door5 call:door6 call:visor call:anm]x47 | 5 | 47 | 예: place ( driver ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) door4 ( emit4 ) door5 ( emit5 ) door6 ( emit6 ) visor ( main ) anm ( driver ) | 미확인(자동) |  |  |
| `/("engine")` | macro | [call:repair]x5 | 5 | 5 | repair ( 30 ) | 미확인(자동) |  |  |
| `/("fuel")` | macro | [atom str]x807 | 386 | 807 | args "fuel1" ¦ args "fuel2" ¦ args "fuel" ¦ args "fuel3" ¦ args "fuel4" | 미확인(자동) |  |  |
| `/("glass")` | macro | [atom str]x92 | 22 | 92 | 예: args "window1" | 미확인(자동) |  |  |
| `/("gun_shield")` | macro | [atom str num]x48 | 11 | 48 | 예: args "tshield1" 20 | 미확인(자동) |  |  |
| `/("gun")` | macro | [call:repair call:thickness]x310 | 310 | 310 | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ repair ( 30 ) thickness ( 8 ) ¦ repair ( 30 ) thickness ( 6 ) ¦ repair ( 60 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 10 ) ¦ repair ( 30 ) thickness ( 35 ) ¦ repair ( 30 ) thickness ( 80 ) ¦ repair ( 30 ) thickness ( 2.5 ) | 미확인(자동) |  |  |
| `/("gun1")` | macro | [call:repair call:thickness]x11 | 11 | 11 | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 45 ) thickness ( 20 ) | 미확인(자동) |  |  |
| `/("gun2")` | macro | [call:repair call:thickness]x4 | 4 | 4 | repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 25 ) | 미확인(자동) |  |  |
| `/("gun3")` | macro | [call:repair call:thickness]x2 | 2 | 2 | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 8 ) | 미확인(자동) |  |  |
| `/("gun4")` | macro | [call:repair call:thickness]x2 | 2 | 2 | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 8 ) | 미확인(자동) |  |  |
| `/("gun5")` | macro | [call:repair call:thickness]x1 | 1 | 1 | repair ( 30 ) thickness ( 15 ) | 미확인(자동) |  |  |
| `/("gun6")` | macro | [call:repair call:thickness]x1 | 1 | 1 | repair ( 30 ) thickness ( 15 ) | 미확인(자동) |  |  |
| `/("lamp")` | macro | [atom str]x2 | 2 | 2 | args "lamp" | 미확인(자동) |  |  |
| `/("make_component")` | macro | [atom str str]x44 | 14 | 44 | args "aux_turret1" "aux_turret1" ¦ args "aux_turret11" "aux_turret1" ¦ args "aux_turret12" "aux_turret1" ¦ args "aux_turret21" "aux_turret2" ¦ args "aux_turret22" "aux_turret2" ¦ args "aux_turret2" "aux_turret2" ¦ args "aux_turret23" "aux_turret2" ¦ args "aux_turret13" "aux_turret1" ¦ args "aux_turret31" "aux_turret3" ¦ args "aux_turret41" "aux_turret4" ¦ args "aux_turret32" "aux_turret3" ¦ args "aux_turret42" "aux_turret4" | 미확인(자동) |  |  |
| `/("mantlet")` | macro | [call:repair]x7 | 7 | 7 | repair ( 30 ) | 미확인(자동) |  |  |
| `/("marine_durability")` | macro | [call:body call:engine call:gun]x4 | 4 | 4 | body ( 600 ) engine ( 450 ) gun ( 150 ) ¦ body ( 600 ) engine ( 300 ) gun ( 150 ) ¦ body ( 600 ) engine ( 450 ) gun ( 1 ) | 미확인(자동) |  |  |
| `/("passenger")` | macro | [call:place call:door call:door2 call:door3 call:door4 call:door5 call:door6 call:anm]x43 [call:place call:door call:door2 call:door3 call:door4 call:door5 call:door6]x10 | 4 | 53 | 예: place ( seat1 ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) door4 ( emit4 ) door5 ( emit5 ) door6 ( emit6 ) anm ( seat1 ) | 미확인(자동) |  |  |
| `/("rocket")` | macro | [atom str]x11 | 3 | 11 | args "rocket00" ¦ args "rocket01" ¦ args "rocket02" ¦ args "rocket03" ¦ args "shell" ¦ args "rocket04" ¦ args "rocket05" | 미확인(자동) |  |  |
| `/("shield_cover")` | macro | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/("shield_group")` | macro | [atom str num str]x166 | 25 | 166 | 예: args "shield1" 19 "body" | 미확인(자동) |  |  |
| `/("shield_part")` | macro | [atom str num str]x38 | 3 | 38 | 예: args "shield3" 105 "turret" | 미확인(자동) |  |  |
| `/("shield_tear")` | macro | [atom str num]x188 | 30 | 188 | 예: args "shield3" 7 | 미확인(자동) |  |  |
| `/("shield_track")` | macro | [atom str num]x380 | 113 | 380 | 예: args "detail2" 30 | 미확인(자동) |  |  |
| `/("shield")` | macro | [atom str num]x868 | 166 | 868 | 예: args "shield1" 4 | 미확인(자동) |  |  |
| `/("solid_wheel")` | macro | [atom str]x112 | 21 | 112 | args "wheelright1" ¦ args "wheelleft1" ¦ args "wheelright2" ¦ args "wheelleft2" ¦ args "wheelright3" ¦ args "wheelleft3" ¦ args "wheelleft4" ¦ args "wheelright4" | 미확인(자동) |  |  |
| `/("tank_heavy_tier1")` | macro | []x49 | 49 | 49 | 예: | 미확인(자동) |  |  |
| `/("tank_heavy_tier2")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/("tank_light_tier1")` | macro | []x52 | 52 | 52 | 예: | 미확인(자동) |  |  |
| `/("tank_light_tier2")` | macro | []x26 | 26 | 26 | 예: | 미확인(자동) |  |  |
| `/("tank_medium_tier1")` | macro | []x33 | 33 | 33 | 예: | 미확인(자동) |  |  |
| `/("tank_medium_tier2")` | macro | []x109 | 109 | 109 | 예: | 미확인(자동) |  |  |
| `/("tank_medium_tier3")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/("tankette_tier1")` | macro | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/("tracks")` | macro | [call:repair call:thickness]x340 | 340 | 340 | repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ repair ( 40 ) thickness ( 30 ) ¦ repair ( 40 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 10 ) ¦ repair ( 30 ) thickness ( 25 ) | 미확인(자동) |  |  |
| `/("train_durability")` | macro | [call:body call:engine call:turret call:mantlet call:gun]x1 | 1 | 1 | body ( 750 ) engine ( 250 ) turret ( 300 ) mantlet ( 225 ) gun ( 225 ) | 미확인(자동) |  |  |
| `/("transmission")` | macro | [atom str]x389 | 387 | 389 | args "transmission" ¦ args "transmission1" ¦ args "transmission2" | 미확인(자동) |  |  |
| `/("turret")` | macro | [call:repair]x43 [call:repair call:thickness]x1 | 44 | 44 | repair ( 45 ) ¦ repair ( 50 ) ¦ repair ( 30 ) thickness ( 10 ) | 미확인(자동) |  |  |
| `/("wheel")` | macro | [atom str]x194 | 60 | 194 | 예: args "wheelleft1" | 미확인(자동) |  |  |
| `/(define "crew_commander")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_commander")/boarder` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_commander")/boarder/door` | block | [str]x2 | 1 | 2 | "emit1" ¦ "emit2" | 미확인(자동) |  |  |
| `/(define "crew_commander")/boarder/link` | block | [str str]x2 | 1 | 2 | "emit1" "%place" ¦ "emit2" "%place" | 미확인(자동) |  |  |
| `/(define "crew_commander")/boarder/link/anm` | block | [str]x2 | 1 | 2 | "commander_l" ¦ "commander_r" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_commander")/boarder/link/forward` | block | [atom]x2 | 1 | 2 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_commander")/boarder/link/reverse` | block | [atom]x2 | 1 | 2 | puton | 미확인(자동) |  |  |
| `/(define "crew_commander")/placer` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_commander")/placer/place` | block | [str]x1 | 1 | 1 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_commander")/placer/place/group` | block | [str]x1 | 1 | 1 | "crew" | 미확인(자동) |  |  |
| `/(define "crew_commander")/placer/place/linkbone` | block | [str]x1 | 1 | 1 | "%bone" | 미확인(자동) |  |  |
| `/(define "crew_commander")/placer/place/turnoff` | block | [param]x1 | 1 | 1 | %off | 미확인(자동) |  |  |
| `/(define "crew_commander")/placer/place/visor` | block | [str]x1 | 1 | 1 | "%visor" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")` | define | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/anm` | block | [str]x9 | 1 | 9 | "1_gunner" ¦ "2_gunner" ¦ "3_gunner" ¦ "1_charger" ¦ "2_charger" ¦ "3_charger" ¦ "1_commander" ¦ "2_commander" ¦ "3_commander" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm")/boarder/anm/forward` | block | []x9 | 1 | 9 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm")/boarder/anm/forward/base` | block | [str]x9 | 1 | 9 | "doorleft1" ¦ "doorright2" ¦ "doorright1" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/anm/forward/begin` | block | [str]x9 | 1 | 9 | "board_tank_right" ¦ "board_kv" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/anm/forward/end` | block | [str]x9 | 1 | 9 | "pose_m5_gunlayer" ¦ "seat_tm1_charger" ¦ "pose_watch_binocular" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/anm/reverse` | block | []x9 | 1 | 9 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/anm/reverse/base` | block | [str num]x9 | 1 | 9 | "doorleft1" -1 ¦ "doorright2" -1 ¦ "doorright1" -1 | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/anm/reverse/end` | block | [str]x9 | 1 | 9 | "emit_tank_1_hold" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/door` | block | [str]x5 | 2 | 5 | "emit1" ¦ "emit2" ¦ "%door" ¦ "%door2" ¦ "%door3" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/link` | block | [str str]x5 | 2 | 5 | "emit1" "%place" ¦ "emit2" "%place" ¦ "%door" "%place" ¦ "%door2" "%place" ¦ "%door3" "%place" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/boarder/link/anm` | block | [str]x5 | 2 | 5 | "left" ¦ "right" ¦ "1_%pose" ¦ "2_%pose" ¦ "3_%pose" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm")/boarder/link/forward` | block | [atom]x5 | 2 | 5 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm")/boarder/link/reverse` | block | [atom]x5 | 2 | 5 | puton | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/placer` | block | []x2 | 2 | 2 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm")/placer/place` | block | [str]x2 | 2 | 2 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm")/placer/place/group` | block | [str]x2 | 2 | 2 | "crew" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/placer/place/linkbone` | block | [str]x2 | 2 | 2 | "%place" ¦ "%linkbone" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/placer/place/turnoff` | block | []x1 [param]x1 | 2 | 2 | ¦ %off | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/placer/place/turnoff/sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm")/placer/place/visor` | block | [str]x2 | 2 | 2 | "%visor" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")` | define | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")/boarder` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")/boarder/door` | block | [str]x6 | 3 | 6 | "%door" ¦ "%door2" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")/boarder/link` | block | [str str]x6 | 3 | 6 | "%door" "%place" ¦ "%door2" "%place" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")/boarder/link/anm` | block | [str]x6 | 3 | 6 | "%anm_left" ¦ "%anm_right" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm2")/boarder/link/forward` | block | [atom]x6 | 3 | 6 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm2")/boarder/link/reverse` | block | [atom]x6 | 3 | 6 | puton | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")/placer` | block | []x3 | 3 | 3 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm2")/placer/place` | block | [str]x3 | 3 | 3 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human_visible_anm2")/placer/place/group` | block | [str]x3 | 3 | 3 | "crew" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")/placer/place/linkbone` | block | [str]x3 | 3 | 3 | "%place" | 미확인(자동) |  |  |
| `/(define "crew_human_visible_anm2")/placer/place/turnoff` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human")` | define | []x56 | 56 | 56 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human")/boarder` | block | []x56 | 56 | 56 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human")/boarder/door` | block | [str]x89 | 56 | 89 | "emit1" ¦ "%door" ¦ "emit2" ¦ "%door2" | 미확인(자동) |  |  |
| `/(define "crew_human")/boarder/link` | block | [str str]x89 | 56 | 89 | "emit1" "%place" ¦ "%door" "%place" ¦ "emit2" "%place" ¦ "%door2" "%place" | 미확인(자동) |  |  |
| `/(define "crew_human")/boarder/link/anm` | block | [str]x89 | 56 | 89 | "left" ¦ "right" ¦ "%animation" ¦ "%place" ¦ "left%animation" ¦ "right%animation" ¦ "%anm_left" ¦ "%anm_right" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human")/boarder/link/forward` | block | [atom]x89 | 56 | 89 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human")/boarder/link/reverse` | block | [atom]x89 | 56 | 89 | puton | 미확인(자동) |  |  |
| `/(define "crew_human")/placer` | block | []x56 | 56 | 56 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human")/placer/place` | block | [str]x56 | 56 | 56 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_human")/placer/place/group` | block | [str]x56 | 56 | 56 | "crew" ¦ "%group" | 미확인(자동) |  |  |
| `/(define "crew_human")/placer/place/linkbone` | block | [str]x31 | 31 | 31 | "%bone" ¦ "%place" | 미확인(자동) |  |  |
| `/(define "crew_human")/placer/place/turnoff` | block | []x17 [param]x14 | 31 | 31 | ¦ %off | 미확인(자동) |  |  |
| `/(define "crew_human")/placer/place/turnoff/sensor` | block | []x13 | 13 | 13 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human")/placer/place/turnoff/shadow` | block | []x24 | 24 | 24 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human")/placer/place/turnoff/targetable` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/(define "crew_human")/placer/place/visor` | block | [str]x34 | 34 | 34 | "%visor" | 미확인(자동) |  |  |
| `/(define "crew_open")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_open")/boarder` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_open")/boarder/door` | block | [str]x2 | 1 | 2 | "emit1" ¦ "emit2" | 미확인(자동) |  |  |
| `/(define "crew_open")/boarder/link` | block | [str str]x2 | 1 | 2 | "emit1" "%place" ¦ "emit2" "%place" | 미확인(자동) |  |  |
| `/(define "crew_open")/boarder/link/anm` | block | [str]x2 | 1 | 2 | "left" ¦ "right" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_open")/boarder/link/forward` | block | [atom]x2 | 1 | 2 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_open")/boarder/link/reverse` | block | [atom]x2 | 1 | 2 | puton | 미확인(자동) |  |  |
| `/(define "crew_open")/placer` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_open")/placer/place` | block | [str]x1 | 1 | 1 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_open")/placer/place/group` | block | [str]x1 | 1 | 1 | "crew" | 미확인(자동) |  |  |
| `/(define "crew_open")/placer/place/linkbone` | block | [str]x1 | 1 | 1 | "gunner2" | 미확인(자동) |  |  |
| `/(define "crew_open")/placer/place/sealable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_open")/placer/place/turnoff` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_open")/placer/place/turnoff/sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_open")/placer/place/visor` | block | [str]x1 | 1 | 1 | "%visor" | 미확인(자동) |  |  |
| `/(define "crew_open")/sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew_open")/sensor/visor` | block | [str]x1 | 1 | 1 | "top" | 미확인(자동) |  |  |
| `/(define "crew_open")/sensor/visor/bone` | block | [str]x1 | 1 | 1 | "vision_rot" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew_open")/sensor/visor/vision` | block | [str]x1 | 1 | 1 | "recon_open" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew")` | define | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/(define "crew")/boarder` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/(define "crew")/boarder/door` | block | [str]x36 | 5 | 36 | 예: "emit1" | 미확인(자동) |  |  |
| `/(define "crew")/boarder/link` | block | [str str]x36 | 5 | 36 | 예: "emit1" "%place" | 미확인(자동) |  |  |
| `/(define "crew")/boarder/link/anm` | block | [str]x36 | 5 | 36 | "%anm" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew")/boarder/link/forward` | block | [atom]x36 | 5 | 36 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew")/boarder/link/reverse` | block | [atom]x36 | 5 | 36 | puton | 미확인(자동) |  |  |
| `/(define "crew")/placer` | block | []x5 | 5 | 5 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew")/placer/place` | block | [str]x5 | 5 | 5 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "crew")/placer/place/group` | block | [str]x5 | 5 | 5 | "crew" | 미확인(자동) |  |  |
| `/(define "crew")/placer/place/linkbone` | block | [str]x5 | 5 | 5 | "%place" | 미확인(자동) |  |  |
| `/(define "crew")/placer/place/turnoff` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/(define "crew")/placer/place/turnoff/sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "crew")/placer/place/turnoff/shadow` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/(define "crew")/placer/place/visor` | block | [str]x5 | 5 | 5 | "%visor" | 미확인(자동) |  |  |
| `/(define "define_place_jump")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "define_place_jump")/boarder` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "define_place_jump")/boarder/door` | block | [str]x1 | 1 | 1 | "emit_l1" | 미확인(자동) |  |  |
| `/(define "define_place_jump")/boarder/link` | block | [str str]x1 | 1 | 1 | "emit_l1" "%i" | 미확인(자동) |  |  |
| `/(define "define_place_jump")/boarder/link/anm` | block | [str]x1 | 1 | 1 | "[fsm]" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "define_place_jump")/boarder/link/forward` | block | [atom]x1 | 1 | 1 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "define_place_jump")/boarder/link/reverse` | block | [atom]x1 | 1 | 1 | puton | 미확인(자동) |  |  |
| `/(define "define_place_jump")/boarder/node` | block | [str]x2 | 1 | 2 | "node_l1" ¦ "node_l2" | 미확인(자동) |  |  |
| `/(define "define_place_jump")/placer` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "define_place_jump")/placer/place` | block | [str]x1 | 1 | 1 | "%i" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "define_place_jump")/placer/place/linkbone` | block | [str]x1 | 1 | 1 | "attach%i" | 미확인(자동) |  |  |
| `/(define "define_place_jump")/placer/place/obstacle` | block | [atom]x1 | 1 | 1 | disable | 미확인(자동) |  |  |
| `/(define "define_place_jump")/placer/place/turnoff` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "define_place_jump")/placer/place/turnoff/shadow` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "define_place_jump")/volume` | block | [str]x1 | 1 | 1 | "place%i" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "define_place_jump")/volume/able` | block | [atom]x1 | 1 | 1 | clear | 미확인(자동) |  |  |
| `/(define "define_place_jump")/volume/able/touch` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/(define "define_place_jump")/volume/tags` | block | [str str]x1 | 1 | 1 | "attach" "attach_%i" | 미확인(자동) |  |  |
| `/(define "define_place")` | define | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "define_place")/placer` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "define_place")/placer/place` | block | [str]x1 | 1 | 1 | "attach%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "define_place")/placer/place/linkbone` | block | [str]x1 | 1 | 1 | "%place" | 미확인(자동) |  |  |
| `/(define "define_place")/placer/place/obstacle` | block | [atom]x1 | 1 | 1 | disable | 미확인(자동) |  |  |
| `/(define "define_place")/placer/place/turnoff` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "define_place")/placer/place/turnoff/sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "define_place")/placer/place/turnoff/shadow` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/(define "passenger")` | define | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/(define "passenger")/boarder` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/(define "passenger")/boarder/door` | block | [str]x36 | 5 | 36 | 예: "emit1" | 미확인(자동) |  |  |
| `/(define "passenger")/boarder/link` | block | [str str]x36 | 5 | 36 | 예: "emit1" "%place" | 미확인(자동) |  |  |
| `/(define "passenger")/boarder/link/anm` | block | [str]x36 | 5 | 36 | "%anm" ¦ "passenger" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "passenger")/boarder/link/forward` | block | [atom]x36 | 5 | 36 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "passenger")/boarder/link/reverse` | block | [atom]x36 | 5 | 36 | puton | 미확인(자동) |  |  |
| `/(define "passenger")/placer` | block | []x5 | 5 | 5 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "passenger")/placer/place` | block | [str]x5 | 5 | 5 | "%place" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/(define "passenger")/placer/place/group` | block | [str]x5 | 5 | 5 | "passenger" | 미확인(자동) |  |  |
| `/(define "passenger")/placer/place/linkbone` | block | [str]x5 | 5 | 5 | "%place" | 미확인(자동) |  |  |
| `/(define "passenger")/placer/place/turnoff` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/(define "passenger")/placer/place/turnoff/shadow` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/##sensor` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/##sensor/visor` | block | [str]x3 | 2 | 3 | "around" ¦ "human" | 미확인(자동) |  |  |
| `/##sensor/visor/bone` | block | [str]x1 | 1 | 1 | "foresight14" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/##sensor/visor/vision` | block | [str]x2 | 2 | 2 | "tank_around" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/##volume` | block | [str]x5 | 5 | 5 | "engine3" | 추정(파일 주석) | Engine Cover | 게임 파일 내 주석(같은 이름의 키) |
| `/##volume/thickness` | block | [num]x5 | 5 | 5 | 10 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/boarder` | block | []x66 | 66 | 66 | 예: | 미확인(자동) |  |  |
| `/boarder/anm` | block | [str]x261 | 66 | 261 | 예: "left" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/boarder/anm/forward` | block | []x261 | 66 | 261 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/boarder/anm/forward/base` | block | [str]x120 [str num]x4 | 44 | 124 | "doorleft" ¦ "doorright" ¦ "board" ¦ "open_left" ¦ "open_right" ¦ "open_driver" ¦ "open" ¦ "" ¦ "doorright" 1 ¦ "doorleft" 1 ¦ "open_gunner" ¦ "open_commander" | 미확인(자동) |  |  |
| `/boarder/anm/forward/begin` | block | [str]x226 [str num]x33 | 65 | 259 | 예: "board_kv" | 미확인(자동) |  |  |
| `/boarder/anm/forward/end` | block | [str]x209 [str num]x9 | 43 | 218 | 예: "pose_daimler_dingo_driver" | 미확인(자동) |  |  |
| `/boarder/anm/reverse` | block | []x259 | 65 | 259 | 예: | 미확인(자동) |  |  |
| `/boarder/anm/reverse/base` | block | [str num]x96 [str]x30 | 46 | 126 | 예: "open_left" -1 | 미확인(자동) |  |  |
| `/boarder/anm/reverse/begin` | block | [str]x3 | 1 | 3 | "emit_Bk1124" | 미확인(자동) |  |  |
| `/boarder/anm/reverse/end` | block | [str]x182 [str num]x74 | 64 | 256 | "emit_sd222_driver" ¦ "emit_body" 1 ¦ "emit_torpedo_boat" ¦ "emit_tank_1" ¦ "emit_tank_1_hold" ¦ "emit_seat_nkl_passenger" ¦ "lca_ty_pass_exit_1" ¦ "lca_ty_pass_exit_2" ¦ "board_lowarmor" -1 ¦ "goatley_crew_l_emit" ¦ "goatley_crew_r_emit" ¦ "board_rubber_boat" -1 | 미확인(자동) |  |  |
| `/boarder/door` | block | [str]x4 | 2 | 4 | "emit1" ¦ "emit2" ¦ "emit01" ¦ "emit02" | 미확인(자동) |  |  |
| `/boarder/link` | block | [str str]x54 | 2 | 54 | 예: "emit1" "driver" | 미확인(자동) |  |  |
| `/boarder/link/anm` | block | [str]x54 | 2 | 54 | "seat" ¦ "seat1" ¦ "seat2" ¦ "seat3" ¦ "driver" ¦ "gunner" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/boarder/link/forward` | block | [atom]x54 | 2 | 54 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/boarder/link/reverse` | block | [atom]x54 | 2 | 54 | puton | 미확인(자동) |  |  |
| `/component` | block | [str]x752 | 123 | 752 | 예: "shield1" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/component/tags` | block | [str]x747 | 120 | 747 | "cut_part" | 미확인(자동) |  |  |
| `/extender[shield]` | block | [str]x6 | 6 | 6 | "shield" | 미확인(자동) |  |  |
| `/extender[shield]/on` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/perks` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/perks/component` | block | [str]x10 | 5 | 10 | "shield1" ¦ "shield2" ¦ "shield3" ¦ "turret_armor1" ¦ "turret_armor2" ¦ "turret_armor3" ¦ "turret_armor4" ¦ "turret_armor5" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/perks/component/armor_damage_modifier` | block | [num]x10 | 5 | 10 | 0 | 미확인(자동) |  |  |
| `/perks/component/health` | block | []x10 | 5 | 10 | 예: | 미확인(자동) |  |  |
| `/perks/component/health/hp` | block | [num]x10 | 5 | 10 | 300 | 미확인(자동) |  |  |
| `/placer` | block | []x22 | 22 | 22 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/placer/place` | block | [str]x94 | 22 | 94 | 예: "driver" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/placer/place/linkbone` | block | [str]x43 | 2 | 43 | 예: "driver" | 미확인(자동) |  |  |
| `/placer/place/obstacle` | block | [atom]x43 | 2 | 43 | disable | 미확인(자동) |  |  |
| `/placer/place/sealable` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/placer/place/turnoff` | block | []x50 | 8 | 50 | 예: | 미확인(자동) |  |  |
| `/placer/place/turnoff/sensor` | block | []x43 | 2 | 43 | 예: | 미확인(자동) |  |  |
| `/placer/place/turnoff/shadow` | block | []x43 | 2 | 43 | 예: | 미확인(자동) |  |  |
| `/placer/place/turnoff/targetable` | block | []x5 [num]x2 | 6 | 7 | 0 | 미확인(자동) |  |  |
| `/placer/place/visor` | block | [str]x46 | 19 | 46 | "none" ¦ "main" ¦ "gun" ¦ "around" ¦ "driver" ¦ "human" | 미확인(자동) |  |  |
| `/sensor` | block | []x33 | 33 | 33 | 예: | 미확인(자동) |  |  |
| `/sensor/visor` | block | [str]x53 | 33 | 53 | 예: "main" | 미확인(자동) |  |  |
| `/sensor/visor/bone` | block | [str]x33 | 20 | 33 | "foresight1" ¦ "visor1" ¦ "visor2" ¦ "foresight1a" ¦ "foresight5" ¦ "foresight1b" ¦ "foresight1c" ¦ "foresight4" ¦ "visor" ¦ "vision_rot" ¦ "foresight6" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/sensor/visor/vision` | block | [str]x53 | 33 | 53 | 예: "recon_main" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/volume` | block | [str]x5982 | 398 | 5982 | 예: "transmission" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/able` | block | []x678 [atom]x11 | 251 | 689 | ¦ clear | 미확인(자동) |  |  |
| `/volume/able/blast` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/volume/able/bullet` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/volume/able/contact` | block | [num]x16 []x6 | 15 | 22 | 0 ~ 1 | 미확인(자동) |  |  |
| `/volume/able/contact_ground` | block | [num]x64 | 32 | 64 | 0 | 미확인(자동) |  |  |
| `/volume/able/holed` | block | []x566 | 236 | 566 | 예: | 미확인(자동) |  |  |
| `/volume/able/nowounddecal` | block | []x89 [num]x26 | 65 | 115 | 0 | 미확인(자동) |  |  |
| `/volume/able/obstacle` | block | [num]x82 []x1 | 21 | 83 | 0 | 미확인(자동) |  |  |
| `/volume/able/touch` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/volume/able/visible` | block | [num]x19 | 6 | 19 | 0 | 미확인(자동) |  |  |
| `/volume/caststeel` | block | [num]x1163 | 174 | 1163 | 0.3 ~ 0.88 | 추정(파일 주석) | effective armor reduction compared to solid plate ¦ effective armor reduction compared to solid plate; 25 + 75 spaced armor array ~86mm effective | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/component` | block | [str]x166 | 131 | 166 | "turret" ¦ "body" ¦ "gun" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/highhardness` | block | []x280 | 34 | 280 | 예: | 미확인(자동) |  |  |
| `/volume/material` | block | [str]x101 | 17 | 101 | "wood" ¦ "fabric" ¦ "sand" ¦ "concrete" | 미확인(자동) |  |  |
| `/volume/quality` | block | [str]x33 | 5 | 33 | "large_flaw" | 미확인(자동) |  |  |
| `/volume/tags` | block | [str]x432 [str str]x74 | 204 | 506 | 예: "mover" "track" | 미확인(자동) |  |  |
| `/volume/thickness` | block | [num]x5554 []x108 | 398 | 5662 | 0.5 ~ 250 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/thickness/bottom` | block | [num]x1679 | 332 | 1679 | 1 ~ 200 | 추정(파일 주석) | Overhang (above tracks) ¦ 4mm + .25" | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/thickness/front` | block | [num]x1969 | 339 | 1969 | 3 ~ 250 | 추정(파일 주석) | Glacis ¦ Lower front | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/thickness/left` | block | [num]x311 | 96 | 311 | 5 ~ 95.25 | 추정(파일 주석) | Top sides ¦ side | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/thickness/rear` | block | [num]x1193 | 322 | 1193 | 1 ~ 89 | 추정(파일 주석) | Front Roof Turret Armor ¦ Bottom Front Nose | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/thickness/right` | block | [num]x245 | 89 | 245 | 5 ~ 600 | 추정(파일 주석) | armor plate sides ¦ Top sides | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/thickness/top` | block | [num]x2170 | 360 | 2170 | 1 ~ 200 | 추정(파일 주석) | Glacis ¦ front upper part | 게임 파일 내 주석(같은 이름의 키) |

### 파일 종류: .inc

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/("armor_heavy_cannon")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/("armor_light_cannon")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/("armor_medium_cannon")` | macro | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/("board_animation")` | macro | [call:place call:board_begin call:board_end call:base]x8 | 4 | 8 | place ( driver ) board_begin ( board_big-midcar_driver ) board_end ( seat_driver_car ) base ( doorleft ) ¦ place ( commander ) board_begin ( board_big-midcar ) board_end ( seat_passenger ) base ( doorright ) ¦ place ( driver ) board_begin ( board_big-midcar ) board_end ( seat_driver_car ) base ( doorright ) ¦ place ( commander ) board_begin ( board_big-midcar_driver ) board_end ( seat_passenger ) base ( doorleft ) ¦ place ( driver ) board_begin ( board_SdKfz251-1 ) board_end ( pose_qld_driver ) base ( doorright ) ¦ place ( commander ) board_begin ( board_SdKfz251-1 ) board_end ( pose_qld_commander ) base ( doorleft ) | 미확인(자동) |  |  |
| `/("bodyback")` | macro | [call:repair]x4 | 4 | 4 | repair ( 30 ) | 미확인(자동) |  |  |
| `/("car_thickness")` | macro | [call:body call:engine]x1 | 1 | 1 | body ( 1.5 ) engine ( 1.5 ) | 미확인(자동) |  |  |
| `/("crew_2_human")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/("crew_3_human_2_extended")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/("crew_4_human_2_extended")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/("crew_5_human")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/("crew_6_human_extended")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/("crew_co_driver")` | macro | [call:place call:door call:door2 call:door3]x4 | 4 | 4 | place ( commander ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인(자동) |  |  |
| `/("crew_driver")` | macro | [call:door call:door2 call:door3]x4 | 4 | 4 | door ( emit0 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인(자동) |  |  |
| `/("crew_human_2_v")` | macro | [call:place call:bone call:door call:door]x1 | 1 | 1 | place ( gunner ) bone ( gunner ) door ( emit3 ) door ( emit4 ) | 미확인(자동) |  |  |
| `/("crew_human_v")` | macro | [call:place call:bone call:door call:animation call:group]x2 | 2 | 2 | place ( commander ) bone ( commander ) door ( emit2 ) animation ( commander ) group ( crew ) | 미확인(자동) |  |  |
| `/("crew_human_visible_anm")` | macro | [call:place call:door call:door2 call:visor call:anm_left call:anm_right]x14 | 5 | 14 | place ( gunner ) door ( emit2 ) door2 ( emit3 ) visor ( main ) anm_left ( gunner_l ) anm_right ( gunner_r ) ¦ place ( charger ) door ( emit2 ) door2 ( emit3 ) visor ( around ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger ) door ( emit2 ) door2 ( emit3 ) visor ( none ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger2 ) door ( emit2 ) door2 ( emit3 ) visor ( around ) anm_left ( charger2_l ) anm_right ( charger2_r ) ¦ place ( gunner2 ) door ( emit2 ) door2 ( emit3 ) visor ( main ) anm_left ( gunner2_l ) anm_right ( gunner2_r ) ¦ place ( charger ) door ( emit4 ) door2 ( emit5 ) visor ( around ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger2 ) door ( emit4 ) door2 ( emit5 ) visor ( around ) anm_left ( charger2_l ) anm_right ( charger2_r ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) anm_left ( driver_l ) anm_right ( driver_r ) ¦ place ( gunner ) door ( emit3 ) door2 ( emit4 ) visor ( main ) anm_left ( gunner_l ) anm_right ( gunner_r ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) anm_left ( commander_l ) anm_right ( commander_r ) | 미확인(자동) |  |  |
| `/("crew_human_visible_anm2")` | macro | [call:place call:door call:door2 call:visor call:anm_left call:anm_right]x3 [call:place call:door call:door2 call:anm_left call:anm_right]x2 | 3 | 5 | place ( charger2 ) door ( emit2 ) door2 ( emit3 ) anm_left ( charger2_l ) anm_right ( charger2_r ) ¦ place ( commander ) door ( emit2 ) door2 ( emit3 ) anm_left ( commander_l ) anm_right ( commander_r ) ¦ place ( gunner2 ) door ( emit1 ) door2 ( emit2 ) visor ( ) anm_left ( gunner2_l ) anm_right ( gunner2_r ) ¦ place ( charger ) door ( emit3 ) door2 ( emit4 ) visor ( ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger2 ) door ( emit3 ) door2 ( emit4 ) visor ( ) anm_left ( charger2_l ) anm_right ( charger2_r ) | 미확인(자동) |  |  |
| `/("crew_human")` | macro | [call:place call:bone call:door call:animation call:group]x77 [call:place call:door call:door2 call:visor]x5 | 11 | 82 | 예: place ( seat1 ) bone ( seat01 ) door ( emit2 ) animation ( commander ) group ( passenger ) | 미확인(자동) |  |  |
| `/("crew_wheeled_cannon_3")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/("crew_wheeled_cannon_4")` | macro | []x15 | 15 | 15 | 예: | 미확인(자동) |  |  |
| `/("crew_wheeled_cannon_5")` | macro | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/("crew_wheeled_cannon_6")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/("crew")` | macro | [call:place call:bone call:door call:animation call:visor call:off]x12 | 5 | 12 | place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) visor ( driver ) off ( { sensor } ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) animation ( gunner ) visor ( none ) off ( ) ¦ place ( gunner2 ) bone ( gunner2 ) door ( emit4 ) animation ( gunner2 ) visor ( none ) off ( ) ¦ place ( gunner3 ) bone ( gunner3 ) door ( emit3 ) animation ( gunner2 ) visor ( none ) off ( ) ¦ place ( gunner ) bone ( gunner ) door ( emit2 ) animation ( gunner ) visor ( none ) off ( ) | 미확인(자동) |  |  |
| `/("fuel")` | macro | [atom str]x7 | 5 | 7 | args "fuel" ¦ args "fuel2" ¦ args "fuel1" | 미확인(자동) |  |  |
| `/("glass")` | macro | [atom str]x19 | 5 | 19 | args "window1" ¦ args "window2" ¦ args "window3" ¦ args "window4" ¦ args "glass1" ¦ args "glass2" ¦ args "glass3" ¦ args "glass4" ¦ args "glass5" | 미확인(자동) |  |  |
| `/("gun")` | macro | [call:repair call:thickness]x20 | 20 | 20 | repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 20 ) | 미확인(자동) |  |  |
| `/("gunner_open")` | macro | [call:place call:bone call:visorbone call:door call:door2 call:anm_left call:anm_right call:group]x1 | 1 | 1 | place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit5 ) door2 ( emit7 ) anm_left ( left ) anm_right ( right ) group ( passenger ) | 미확인(자동) |  |  |
| `/("passenger_10_human")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/("passenger_12_human")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/("seat_human")` | macro | [call:place call:bone call:door call:animation]x46 | 11 | 46 | 예: place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( armor1 ) | 미확인(자동) |  |  |
| `/("shield_cover")` | macro | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/("shield_group")` | macro | [atom str num str]x45 | 10 | 45 | 예: args "shield1" 5 "shield" | 미확인(자동) |  |  |
| `/("shield")` | macro | [atom str num]x12 [atom str num str]x6 | 10 | 18 | args "shield_lower" 8 ¦ args "shield_lower" 4 ¦ args "shield" 8 "shield" ¦ args "shield" 4 ¦ args "shield1" 4 ¦ args "shield_lower" 8 "shield" ¦ args "shield_upper" 8 "shield" ¦ args "shield" 5 "shield" ¦ args "upper_shield" 8 "shield" ¦ args "shield_lower" 4.5 ¦ args "shield2" 4 | 미확인(자동) |  |  |
| `/("solid_wheel")` | macro | [atom str]x8 | 4 | 8 | args "wheelright" ¦ args "wheelleft" | 미확인(자동) |  |  |
| `/("supply_engineer")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/("tank_medium_tier2")` | macro | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/("transmission")` | macro | [atom str]x5 | 5 | 5 | args "transmission" | 미확인(자동) |  |  |
| `/("wheel")` | macro | [atom str]x60 | 20 | 60 | args "wheelright" ¦ args "wheelleft" ¦ args "wheelright1" ¦ args "wheelright2" ¦ args "wheelleft1" ¦ args "wheelleft2" ¦ args "wheelright3" ¦ args "wheelleft3" | 미확인(자동) |  |  |
| `/("wood")` | macro | [atom str]x7 | 1 | 7 | args "bodyback1" ¦ args "bodyback2" ¦ args "bodyback3" ¦ args "bodyback4" ¦ args "bodyback5" ¦ args "bodyback6" ¦ args "bodyback7" | 미확인(자동) |  |  |
| `/(include)` | include | [str]x72 [str call:scale]x46 | 50 | 118 | 예: "/properties/armoredcar.ext" | 미확인(자동) |  |  |
| `/(mod)` | mod | [str]x4 [atom str]x1 | 5 | 5 | "mp" ¦ not "mp" | 미확인(자동) |  |  |
| `/##bone` | block | [str]x3 | 3 | 3 | "commander_rot" | 미확인(자동) |  |  |
| `/##bone/speed2` | block | [num]x3 | 3 | 3 | 180 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/able` | block | [str]x11 | 11 | 11 | "emit_on_attack" ¦ "emit_passangers_on_enemy" ¦ "emit_passengers_on_enemy" | 미확인(자동) |  |  |
| `/boarder` | block | []x45 | 45 | 45 | 예: | 미확인(자동) |  |  |
| `/boarder/anm` | block | [str]x400 | 45 | 400 | 예: "driver" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/boarder/anm/forward` | block | []x400 | 45 | 400 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/boarder/anm/forward/base` | block | [str]x41 | 19 | 41 | "board" ¦ "open_driver" ¦ "doorleft" ¦ "doorright" ¦ "open_co_driver" | 미확인(자동) |  |  |
| `/boarder/anm/forward/begin` | block | [str]x145 | 22 | 145 | 예: "board_big-midcar_driver" | 미확인(자동) |  |  |
| `/boarder/anm/forward/end` | block | [str num]x251 [str]x129 | 45 | 380 | 예: "seat_kubel_driver" | 미확인(자동) |  |  |
| `/boarder/anm/reverse` | block | []x145 | 22 | 145 | 예: | 미확인(자동) |  |  |
| `/boarder/anm/reverse/base` | block | [str]x26 [str num]x13 | 17 | 39 | "board" ¦ "doorleft" -1 ¦ "doorright" -1 ¦ "open_driver" ¦ "open_co_driver" ¦ "open_driver" -1 ¦ "close_driver" | 미확인(자동) |  |  |
| `/boarder/anm/reverse/end` | block | [str]x111 [str num]x34 | 22 | 145 | "emit_tank_1_hold" ¦ "emit_tank_2_hold" ¦ "emit_body" 1 ¦ "emit_tank_1" ¦ "emit_tank_1_hold" 1 ¦ "emit_tank_2" ¦ "emit_body" ¦ "board_big-midcar_driver" -1 ¦ "board_big-midcar" -1 ¦ "board_t26_driver" -1 ¦ "emit_t34_hold" | 미확인(자동) |  |  |
| `/bone` | block | [str]x113 | 40 | 113 | 예: "mgun_h_rot1" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/bone/("turret_heavy")` | macro | [call:power_traverse]x5 | 5 | 5 | power_traverse ( 6 ) ¦ power_traverse ( 24 ) ¦ power_traverse ( 16 ) | 미확인(자동) |  |  |
| `/bone/("turret_light")` | macro | [call:power_traverse]x4 | 4 | 4 | power_traverse ( 0 ) | 미확인(자동) |  |  |
| `/bone/("turret_medium")` | macro | [call:power_traverse]x1 | 1 | 1 | power_traverse ( 19 ) | 미확인(자동) |  |  |
| `/bone/limits` | block | [num num]x67 | 36 | 67 | 예: -5 45 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/bone/speed2` | block | [num]x103 | 40 | 103 | 1.75 ~ 60 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/brain` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/brain/properties` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/brain/properties/fire_mode` | block | [atom]x4 | 4 | 4 | hold | 미확인(자동) |  |  |
| `/chassis` | block | [str]x29 []x20 | 49 | 49 | "track" ¦  ¦ "wheel" | 추정(파일 주석) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 게임 파일 내 주석(같은 이름의 키) |
| `/chassis/("car_trace")` | macro | [call:fx call:step call:wheell call:wheelr call:bonel call:boner]x42 | 31 | 42 | fx ( "tracewheel05a" ) step ( 0.14 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03" ) step ( 0.08 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03a" ) step ( 0.09 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03d" ) step ( 0.08 ) wheell ( "wlr" ) wheelr ( "wrr" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel06" ) step ( 0.08 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03" ) step ( 0.08 ) wheell ( "wlr" ) wheelr ( "wrr" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel06" ) step ( 0.07 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel02" ) step ( 0.1 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel02" ) step ( 0.1 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel02a" ) step ( 0.08 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel02a" ) step ( 0.15 ) wheell ( "wlr" ) wheelr ( "wrr" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel02" ) step ( 0.1 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL3" ) boner ( "FXtraceR3" ) | 미확인(자동) |  |  |
| `/chassis/("mobility_btr")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x2 | 2 | 2 | speed ( 96 ) speedatmaxturn ( 25 ) reverse ( 82 ) weight ( 3.2 ) power ( 55 ) fuel ( 81 ) type ( fuel ) range ( 320 ) ¦ speed ( 53 ) speedatmaxturn ( 24 ) reverse ( 17 ) weight ( 5.1 ) power ( 52 ) fuel ( 118 ) type ( fuel ) range ( 200 ) | 미확인(자동) |  |  |
| `/chassis/("mobility_cannon")` | macro | [call:speed call:weight]x23 | 23 | 23 | 예: speed ( 4.3 ) weight ( 6190 ) | 미확인(자동) |  |  |
| `/chassis/("mobility_car")` | macro | [call:speed call:speedatmaxturn call:reverse call:weight call:power call:fuel call:type call:range]x10 | 10 | 10 | speed ( 72 ) speedatmaxturn ( 14 ) reverse ( 14 ) weight ( 9.1 ) power ( 147 ) fuel ( 225 ) type ( fuel ) range ( 320 ) ¦ speed ( 72 ) speedatmaxturn ( 14 ) reverse ( 14 ) weight ( 9.3 ) power ( 147 ) fuel ( 225 ) type ( fuel ) range ( 320 ) ¦ speed ( 65 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 4.631 ) power ( 72 ) fuel ( 145 ) type ( fuel ) range ( 450 ) ¦ speed ( 65 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 5.131 ) power ( 72 ) fuel ( 145 ) type ( fuel ) range ( 250 ) ¦ speed ( 72 ) speedatmaxturn ( 14 ) reverse ( 14 ) weight ( 8.707 ) power ( 147 ) fuel ( 227 ) type ( fuel ) range ( 320 ) ¦ speed ( 96 ) speedatmaxturn ( 12 ) reverse ( 18 ) weight ( 1.11 ) power ( 60 ) fuel ( 57 ) type ( fuel ) range ( 480 ) ¦ speed ( 72 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 4.00 ) power ( 104 ) fuel ( 150 ) type ( fuel ) range ( 480 ) ¦ speed ( 72 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 4.16 ) power ( 104 ) fuel ( 150 ) type ( fuel ) range ( 480 ) | 미확인(자동) |  |  |
| `/chassis/("mobility_tank")` | macro | [call:speed call:reverse call:traverse call:weight call:power call:track call:fuel call:type call:range]x14 | 14 | 14 | 예: speed ( 48 ) reverse ( 20 ) traverse ( 35 ) weight ( 3.75 ) power ( 85 ) track ( 5 ) fuel ( 91 ) type ( fuel ) range ( 150 ) | 미확인(자동) |  |  |
| `/chassis/("tank_trace_ex")` | macro | [call:fx call:step call:len call:boneleft call:boneright]x5 | 5 | 5 | fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.58 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) | 미확인(자동) |  |  |
| `/chassis/("tank_trace")` | macro | [call:fx call:step call:len]x14 | 14 | 14 | fx ( "tracks_mid" ) step ( 0.32 ) len ( 0.5 ) ¦ fx ( "tracks_big" ) step ( 0.47 ) len ( 0.79 ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.37 ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.7 ) ¦ fx ( "tracks_mid" ) step ( 0.3 ) len ( 1.05 ) ¦ fx ( "tracks_sm" ) step ( 0.3 ) len ( 1.15 ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.95 ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.9 ) ¦ fx ( "tracks_mid" ) step ( 0.3 ) len ( 0.7 ) | 미확인(자동) |  |  |
| `/collider` | block | [str]x15 | 15 | 15 | "tank.light" ¦ "cannon_small" ¦ "truck" ¦ "tank.heavy" | 미확인(자동) |  |  |
| `/extender[cannon]` | block | [str]x23 | 23 | 23 | "cannon" | 미확인(자동) |  |  |
| `/extender[cannon]/animation` | block | []x23 | 23 | 23 | 예: | 미확인(자동) |  |  |
| `/extender[cannon]/animation/aim` | block | [str str str str]x15 [str str str str str]x3 [str str]x3 [str str str]x1 [str str str str str str]x1 | 23 | 23 | 예: "cannon_zis3_gunlayer" "" "" "" "" | 미확인(자동) |  |  |
| `/extender[cannon]/animation/fire` | block | [str str str str]x15 [str str str str str]x3 [str str]x2 [str str str]x1 [str str str str str str]x1 | 22 | 22 | "cannon_small_gunner_fire" "cannon_small_charger_fire" "cannon_small_charger2_fire" "cannon_small_commander_fire" ¦ "new_cannon_gunner_fire" "new_cannon_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "cannon_gunlayer_fire-alert" "flak_gunner2_fire" "flak_charger_fire" "" "flak_commander_fire" ¦ "" "cannon_small_charger_fire" "cannon_small_charger2_fire" "cannon_small_commander_fire" ¦ "AAgun_61k_gunner_fire" "AAgun_61k_charger_fire" "" ¦ "" "new_cannon_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "" "cannon_small_charger_fire" "cannon_small_charger2_fire" "" ¦ "aagun_flak37_commander_fire" "aagun_72k_gunner_fire" "flak_charger_fire" "flak_gunner2_fire" "aagun_flak37_gunner_fire" "flak_commander_fire" ¦ "cannon_gunlayer_fire-alert" "cannon_pdr25_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "AAgun_61k_gunner_fire" "AAgun_61k_charger_fire" "flak_charger_fire" "flak_gunner2_fire" "flak_commander_fire" ¦ "mortar_82bm37_gunner_fire" "mortar_82bm37_commander_fire" ¦ "" "mg_lafette_commander_fire" | 추정(파일 주석) | mg_maxim_gunner_fire | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[cannon]/animation/move` | block | [str str str str]x15 [str str str str str]x3 [str str]x3 [str str str]x1 [str str str str str str]x1 | 23 | 23 | "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mortar_driver_l" "mgun_browning_driver_r" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mgun_browning_driver_l" "mgun_browning_driver_r" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mgun_browning_driver_l" "mgun_browning_driver_l" "mgun_browning_driver_r" ¦ "mgun_browning_driver_r" "mortar_driver_l" ¦ "cannon_pak40_pusher_right" "cannon_pak40_pusher_left" "mortar_driver_l" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_left" "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "cannon_pak40_pusher_right" "cannon_pak40_pusher_right" ¦ "" "" "" "" "" ¦ "mortar_driver_r" "mortar_driver_l" | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[cannon]/animation/reload` | block | [str str str str]x15 [str str str str str]x3 [str str]x3 [str str str]x1 [str str str str str str]x1 | 23 | 23 | 예: "" "flak_gunner2_reload" "flak_charger_reload" "" "flak_commander_reload" | 미확인(자동) |  |  |
| `/extender[cannon]/animation/switch` | block | [str]x21 | 21 | 21 | "close" | 미확인(자동) |  |  |
| `/extender[cannon]/attack` | block | []x21 | 21 | 21 | 예: | 추정(파일 주석) | fire/attack state: cannon aim, fire, and reload events | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[cannon]/attack/obstacle` | block | [str]x21 | 21 | 21 | "open" | 미확인(자동) |  |  |
| `/extender[cannon]/attack/workers` | block | [str str str str]x15 [str str str str str]x3 [str str str]x1 [str str str str str str]x1 | 20 | 20 | "gunner" "charger" "charger2" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "commander" ¦ "gunner" "charger" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "charger3" "commander" | 미확인(자동) |  |  |
| `/extender[cannon]/move` | block | []x21 | 21 | 21 | 예: | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[cannon]/move/obstacle` | block | [str]x21 | 21 | 21 | "close" | 미확인(자동) |  |  |
| `/extender[cannon]/move/workers` | block | [str str str str atom num]x15 [str str str str str atom num]x2 [str str str atom num]x1 [str str str str str str atom num]x1 [atom]x1 | 20 | 20 | "driver1" "driver2" "driver3" "driver4" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 2 ¦ "driver1" "driver2" "driver3" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" "driver6" minWorkers 2 ¦ clear | 미확인(자동) |  |  |
| `/extender[cannon]/rotate_ik_limit` | block | [num]x3 | 3 | 3 | 0 | 미확인(자동) |  |  |
| `/extender[cannon]/trailing` | block | []x20 | 20 | 20 | 예: | 추정(파일 주석) | trailing state: cannon is being towed {obstacle} not needed. Uses "close" | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[cannon]/trailing/workers` | block | [str str str str]x15 [str str str str str]x2 [str str str]x1 [str str str str str str]x1 [atom]x1 | 20 | 20 | "seat1" "seat2" "seat3" "seat4" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" ¦ "seat1" "seat2" "seat3" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" "seat6" ¦ clear | 미확인(자동) |  |  |
| `/extender[enumerator]` | block | [str]x12 | 12 | 12 | "enumerator" | 미확인(자동) |  |  |
| `/extender[enumerator]/digit_folder` | block | [str]x12 | 12 | 12 | "us2" ¦ "ru1" | 미확인(자동) |  |  |
| `/extender[inventory]` | block | [str]x16 | 16 | 16 | "inventory" | 미확인(자동) |  |  |
| `/extender[inventory]/box` | block | []x16 | 16 | 16 | 예: | 미확인(자동) |  |  |
| `/extender[inventory]/box/item` | block | [str num]x27 [str atom str num]x3 | 14 | 30 | 예: "sandbag_vehicle" 1 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[inventory]/box/item/user` | block | [str]x3 | 2 | 3 | "mgun2" ¦ "mgun3" ¦ "mgun" | 미확인(자동) |  |  |
| `/extender[inventory]/weapon` | block | [str]x4 | 3 | 4 | "mgun" ¦ "mgun2" ¦ "mgun3" | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[inventory]/weapon/mask` | block | [str]x4 | 3 | 4 | "mountable" | 미확인(자동) |  |  |
| `/extender[shield]` | block | [str]x9 | 9 | 9 | "shield" | 미확인(자동) |  |  |
| `/extender[shield]/on` | block | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |
| `/extender[smokescreen]` | block | [str]x2 | 2 | 2 | "smokescreen" | 미확인(자동) |  |  |
| `/extender[smokescreen]/cooldown` | block | [num]x2 | 2 | 2 | 5 | 추정(파일 주석) | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 게임 파일 내 주석(같은 이름의 키) |
| `/extender[smokescreen]/itemtags` | block | [str]x2 | 2 | 2 | "grenade no8_smoke" | 미확인(자동) |  |  |
| `/extension` | block | [str]x49 | 47 | 49 | 예: "daimler_dingo.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/mass` | block | [num]x49 | 49 | 49 | 32 ~ 47500 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/patherid` | block | [str]x32 | 32 | 32 | 예: "sdkfz232" | 미확인(자동) |  |  |
| `/placer` | block | []x3 | 3 | 3 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/placer/place` | block | [str atom]x10 [str]x4 | 3 | 14 | 예: "weapon1" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/placer/place/entity` | block | [str]x3 | 1 | 3 | "lee_enfield_no1_mk3" | 미확인(자동) |  |  |
| `/placer/place/handlebone` | block | [str]x3 | 1 | 3 | "body" | 미확인(자동) |  |  |
| `/placer/place/linkbone` | block | [str]x3 | 1 | 3 | "weapon1" ¦ "weapon2" ¦ "weapon3" | 미확인(자동) |  |  |
| `/placer/place/visor` | block | [str]x1 | 1 | 1 | "main" | 미확인(자동) |  |  |
| `/props` | block | [str str str str str str str str str]x11 [str str str str str str str]x7 [str str str str str str str str str str str]x6 [str str str str str str]x5 [str str str str str str str str str str str str str]x4 | 49 | 49 | 예: "tank" "wheeled" "-turret" "explosion_throw_crew" "vision_lev02" "detect_tank_mg" "open_death_fx" | 미확인(자동) |  |  |
| `/sensor` | block | []x10 | 10 | 10 | 예: | 미확인(자동) |  |  |
| `/sensor/visor` | block | [str]x12 | 10 | 12 | "main" ¦ "back" ¦ "around" ¦ "gunner3" ¦ "gunner4" | 미확인(자동) |  |  |
| `/sensor/visor/bone` | block | [str]x6 | 4 | 6 | "foresight6" ¦ "foresight4" ¦ "foresight5" ¦ "visor" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/sensor/visor/vision` | block | [str]x12 | 10 | 12 | "tank_rear-mg" ¦ "vehicle_around" ¦ "cannon_range_210" ¦ "recon_main" ¦ "cannon_mortar" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/targetclass` | block | [str]x37 [str str]x2 | 39 | 39 | "howitzer" ¦ "cannon_big" ¦ "armoredcar" ¦ "tank_medium" ¦ "cannon_small" ¦ "tank_heavy" ¦ "tank_light" ¦ "armoredcar" "bullet" ¦ "armoredcar_l" ¦ "cannon_zenite" | 미확인(자동) |  |  |
| `/targetselector` | block | [str]x39 | 39 | 39 | "cannon_howitzer" ¦ "tank_medium" ¦ "tank_bullet" ¦ "cannon_big" ¦ "tank_light" ¦ "car_bullet" ¦ "cannon_small" ¦ "cannon_aa" ¦ "cannon_zenite" | 미확인(자동) |  |  |
| `/volume` | block | [str]x92 | 24 | 92 | 예: "body3" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/able` | block | []x53 [atom]x5 | 22 | 58 | ¦ clear | 미확인(자동) |  |  |
| `/volume/able/contact` | block | [num]x5 | 1 | 5 | 1 | 미확인(자동) |  |  |
| `/volume/able/holed` | block | []x47 | 21 | 47 | 예: | 미확인(자동) |  |  |
| `/volume/able/nowounddecal` | block | [num]x2 | 1 | 2 | 0 | 미확인(자동) |  |  |
| `/volume/able/visible` | block | [num]x4 | 2 | 4 | 0 | 미확인(자동) |  |  |
| `/volume/material` | block | [str]x15 | 5 | 15 | "wood" | 미확인(자동) |  |  |
| `/volume/tags` | block | [str]x24 | 7 | 24 | "wood" ¦ "invisible" | 미확인(자동) |  |  |
| `/volume/thickness` | block | [num]x14 | 6 | 14 | 0.05 ~ 30 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/weaponry` | block | []x40 | 40 | 40 | 예: | 미확인(자동) |  |  |
| `/weaponry/("restore_ik_time")` | macro | []x31 | 31 | 31 | 예: | 미확인(자동) |  |  |
| `/weaponry/##place` | block | [str]x3 | 3 | 3 | "mgun" | 추정(파일 주석) | maybe future feature | 게임 파일 내 주석(같은 이름의 키) |
| `/weaponry/##place/ai_in_manual` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/weaponry/##place/basic` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/weaponry/##place/foresight` | block | [str]x3 | 3 | 3 | "vision_rot" | 미확인(자동) |  |  |
| `/weaponry/##place/gunner` | block | [str]x3 | 3 | 3 | "commander" | 미확인(자동) |  |  |
| `/weaponry/##place/type` | block | [str]x3 | 3 | 3 | "commander" | 미확인(자동) |  |  |
| `/weaponry/##place/weapon` | block | [str]x3 | 3 | 3 | "commander_vision" | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/weaponry/place` | block | [str]x64 | 40 | 64 | "gun" ¦ "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/weaponry/place/("abm_dymamic_aa")` | macro | [call:zeroing call:dispersion]x2 | 2 | 2 | zeroing ( 2.25 ) dispersion ( 0.50 ) | 미확인(자동) |  |  |
| `/weaponry/place/("abm_dymamic_aaa")` | macro | [call:zeroing call:dispersion]x1 | 1 | 1 | zeroing ( 1.95 ) dispersion ( 0.24 ) | 미확인(자동) |  |  |
| `/weaponry/place/("abm_dymamic_gun")` | macro | [call:zeroing call:dispersion]x18 | 18 | 18 | zeroing ( 8.0 ) dispersion ( 1 ) ¦ zeroing ( 1.50 ) dispersion ( 0.35 ) ¦ zeroing ( 1.00 ) dispersion ( 0.35 ) ¦ zeroing ( 2.00 ) dispersion ( 0.35 ) ¦ zeroing ( 4.0 ) dispersion ( 1 ) ¦ zeroing ( 4.00 ) dispersion ( 0.25 ) ¦ zeroing ( 5.0 ) dispersion ( 1 ) ¦ zeroing ( 3.00 ) dispersion ( 0.35 ) ¦ zeroing ( 2.50 ) dispersion ( 0.35 ) | 미확인(자동) |  |  |
| `/weaponry/place/("abm_dymamic_mortar")` | macro | [call:zeroing call:dispersion]x1 | 1 | 1 | zeroing ( 3.0 ) dispersion ( 1 ) | 미확인(자동) |  |  |
| `/weaponry/place/("abm_dymamic_stabilised")` | macro | [call:zeroing call:dispersion]x1 | 1 | 1 | zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인(자동) |  |  |
| `/weaponry/place/("abm_dymamic_td")` | macro | [call:zeroing call:dispersion]x2 | 2 | 2 | zeroing ( 2.5 ) dispersion ( 0.25 ) | 미확인(자동) |  |  |
| `/weaponry/place/("abm_dymamic")` | macro | [call:zeroing call:dispersion]x7 | 7 | 7 | zeroing ( 3.0 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인(자동) |  |  |
| `/weaponry/place/("abm_mgun")` | macro | []x32 | 19 | 32 | 예: | 미확인(자동) |  |  |
| `/weaponry/place/ai_in_manual` | block | []x23 | 15 | 23 | 예: | 미확인(자동) |  |  |
| `/weaponry/place/basic` | block | []x31 | 18 | 31 | 예: | 미확인(자동) |  |  |
| `/weaponry/place/charger` | block | [str]x43 [str atom num]x20 | 39 | 63 | "charger charger2" minWorkers 1 ¦ "commander" ¦ "charger" ¦ "gunner" ¦ "gunner3" ¦ "charger2" ¦ "gunner2" ¦ "charger charger2 charger3" minWorkers 1 | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/weaponry/place/foresight` | block | [str]x19 | 9 | 19 | "foresight5" ¦ "foresight4" ¦ "foresight6" ¦ "foresight3" | 미확인(자동) |  |  |
| `/weaponry/place/gunner` | block | [str]x44 [str atom num]x5 | 25 | 49 | "gunner" ¦ "gunner3" ¦ "gunner gunner2" minWorkers 1 ¦ "commander" ¦ "charger" ¦ "charger2" ¦ "gunner2" | 미확인(자동) |  |  |
| `/weaponry/place/item` | block | [str atom str num]x1 | 1 | 1 | "browning_mbrowning_m19a417a1" filling "ammo mgun_usa belt" 250 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/weaponry/place/linkanimation` | block | [str]x12 | 10 | 12 | "off" | 미확인(자동) |  |  |
| `/weaponry/place/linkbone` | block | [str]x12 | 10 | 12 | "mgun" ¦ "mgun2" ¦ "mgun3" | 미확인(자동) |  |  |
| `/weaponry/place/restoreikafteraim` | block | []x32 | 32 | 32 | 예: | 미확인(자동) |  |  |
| `/weaponry/place/rotate` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/weaponry/place/type` | block | [str]x32 | 19 | 32 | "main" ¦ "coaxial" ¦ "hull" ¦ "rear" ¦ "aa" ¦ "mgun_turret_left" ¦ "mgun_turret_right" ¦ "left" ¦ "right" | 미확인(자동) |  |  |
| `/weaponry/place/weapon` | block | [str atom str num]x32 [str atom]x8 | 22 | 40 | 예: "browning_m2" filled | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |

### 파일 종류: .mdl

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/area` | block | [str]x36 | 16 | 36 | 예: "obstacle" | 미확인(자동) |  |  |
| `/area/circle2` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/area/circle2/center` | block | [num num]x2 [atom atom]x1 [num atom]x1 | 4 | 4 | 0 0 ¦ -3.8147e-006 2.04847e-009 ¦ 0 2.04847e-009 | 미확인(자동) |  |  |
| `/area/circle2/radius` | block | [num]x4 | 4 | 4 | 8.3967 ~ 124.771 | 미확인(자동) |  |  |
| `/area/obb2` | block | []x29 | 11 | 29 | 예: | 미확인(자동) |  |  |
| `/area/obb2/axis` | block | [num num]x29 | 11 | 29 | 1 0 ¦ -1 0 ¦ 0.927257 0.374426 ¦ 0.992554 0.121808 | 미확인(자동) |  |  |
| `/area/obb2/center` | block | [num num]x29 | 11 | 29 | 예: 4.53811 0.0132437 | 미확인(자동) |  |  |
| `/area/obb2/extent` | block | [num num]x29 | 11 | 29 | 예: 21.2878 1.01774 | 미확인(자동) |  |  |
| `/area/polygon2` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/area/polygon2/radius` | block | [num]x3 | 3 | 3 | 0 | 미확인(자동) |  |  |
| `/area/polygon2/vertex` | block | [num num]x29 [num atom]x2 | 3 | 31 | 예: 1.00224 10.9324 | 미확인(자동) |  |  |
| `/area/rotate` | block | []x30 | 11 | 30 | 예: | 미확인(자동) |  |  |
| `/area/tags` | block | [str]x1 | 1 | 1 | "open" | 미확인(자동) |  |  |
| `/obstacle` | block | [str]x940 | 366 | 940 | 예: "rectangle1" | 미확인(자동) |  |  |
| `/obstacle/obb2` | block | []x840 | 338 | 840 | 예: | 미확인(자동) |  |  |
| `/obstacle/obb2/axis` | block | [num num]x821 [num atom]x19 | 338 | 840 | 예: 1 0 | 미확인(자동) |  |  |
| `/obstacle/obb2/center` | block | [num num]x773 [num atom]x47 [atom num]x18 [atom atom]x2 | 338 | 840 | 예: -127.125 -0.21965 | 미확인(자동) |  |  |
| `/obstacle/obb2/extent` | block | [num num]x840 | 338 | 840 | 예: 14.0452 43.2815 | 미확인(자동) |  |  |
| `/obstacle/polygon2` | block | []x100 | 73 | 100 | 예: | 미확인(자동) |  |  |
| `/obstacle/polygon2/radius` | block | [num]x100 | 73 | 100 | 0 | 미확인(자동) |  |  |
| `/obstacle/polygon2/vertex` | block | [num num]x550 [num atom]x1 | 73 | 551 | 예: -2.45032 -255.99 | 미확인(자동) |  |  |
| `/obstacle/rotate` | block | []x1090 | 362 | 1090 | 예: | 미확인(자동) |  |  |
| `/obstacle/tags` | block | [str]x382 | 142 | 382 | "open" ¦ "close" ¦ "body" | 미확인(자동) |  |  |
| `/skeleton` | block | []x2141 | 2141 | 2141 | 예: | 미확인(자동) |  |  |
| `/skeleton/animation` | block | []x718 | 718 | 718 | 예: | 미확인(자동) |  |  |
| `/skeleton/animation/sequence` | block | [str]x4059 | 718 | 4059 | 예: "engine_start" | 미확인(자동) |  |  |
| `/skeleton/animation/sequence/events` | block | []x1083 | 435 | 1083 | 예: | 미확인(자동) |  |  |
| `/skeleton/animation/sequence/events/#row` | block | [num str]x1533 | 435 | 1533 | 예: 2 "hatch_light" | 미확인(자동) |  |  |
| `/skeleton/animation/sequence/resume` | block | []x1261 | 609 | 1261 | 예: | 미확인(자동) |  |  |
| `/skeleton/animation/sequence/speed` | block | [num]x4036 | 716 | 4036 | 0.1 ~ 9 | 미확인(자동) |  |  |
| `/skeleton/bone` | block | [str]x1492 [atom str]x649 | 2141 | 2141 | "basis" ¦ revolute "basis" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone` | block | [str]x11088 [atom str]x269 | 2139 | 11357 | 예: "body" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/##bone` | block | [str]x1 | 1 | 1 | "scissor" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/##bone/position` | block | [num num num]x1 | 1 | 1 | -0.2175 0 -0.7228 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/##bone/volumeview` | block | [str]x1 | 1 | 1 | "scissor.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/##bone/volumeview/layer` | block | [atom]x1 | 1 | 1 | waterscissor | 미확인(자동) |  |  |
| `/skeleton/bone/bone/animation` | block | []x1605 | 1303 | 1605 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/animation/sequence` | block | [str]x1605 | 1303 | 1605 | "break" ¦ "repair" ¦ "body" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/animation/sequence/file` | block | [str]x1605 | 1303 | 1605 | "body_r.anm" ¦ "body_b.anm" ¦ "trackl_b.anm" ¦ "trackr_b.anm" ¦ "trackl_r.anm" ¦ "trackr_r.anm" ¦ "trackl.anm" ¦ "trackr.anm" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/animation/sequence/speed` | block | [num]x1549 | 1295 | 1549 | 0.2 ~ 1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone` | block | [str]x29299 [atom str]x18600 | 2120 | 47899 | 예: revolute "turret" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/##bone` | block | [str]x1 | 1 | 1 | "detail7" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/##bone/lodview` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/##bone/lodview/volumeview` | block | [str]x2 | 1 | 2 | "detail7.ply" ¦ "detail7_lod1.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/##bone/matrix34` | block | [num num num num num num num num num num num num]x1 | 1 | 1 | 0 1 0 0 0 1 1 0 0 19.3585 -5.5815 1.3099 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/##bone/parameters` | block | [str]x1 | 1 | 1 | "id=turret;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/animation` | block | []x2191 | 1018 | 2191 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/animation/sequence` | block | [str]x2191 | 1018 | 2191 | "break" ¦ "repair" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/animation/sequence/file` | block | [str]x2191 | 1018 | 2191 | 예: "turret_r.anm" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/animation/sequence/speed` | block | [num]x2120 | 1004 | 2120 | 0.15 ~ 1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone` | block | [str]x21571 [atom str]x17796 | 2026 | 39367 | 예: revolute "mgun_rot" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/animation` | block | []x815 | 777 | 815 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/animation/sequence` | block | [str]x815 | 777 | 815 | "repair" ¦ "break" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/animation/sequence/file` | block | [str]x815 | 777 | 815 | 예: "gun_r.anm" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/animation/sequence/speed` | block | [num]x766 | 728 | 766 | 0.25 ~ 1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone` | block | [atom str]x9771 [str]x7431 | 1870 | 17202 | 예: "vickers_k" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/animation` | block | []x138 | 131 | 138 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/animation/sequence` | block | [str]x138 | 131 | 138 | "break" ¦ "repair" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/animation/sequence/file` | block | [str]x138 | 131 | 138 | "gun_r.anm" ¦ "gun_b.anm" ¦ "gun1_r.anm" ¦ "trackl_b.anm" ¦ "trackr_b.anm" ¦ "turret_r.anm" ¦ "turret_b.anm" ¦ "gun2_r.anm" ¦ "gun1_b.anm" ¦ "gun2_b.anm" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/animation/sequence/speed` | block | [num]x133 | 126 | 133 | 0.25 ~ 1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone` | block | [str]x3663 [atom str]x644 | 1238 | 4307 | 예: "foresight6" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/animation` | block | []x38 | 38 | 38 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/animation/sequence` | block | [str]x38 | 38 | 38 | "repair" ¦ "break" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/animation/sequence/file` | block | [str]x37 | 37 | 37 | "gun_r.anm" ¦ "gun_b.anm" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/animation/sequence/speed` | block | [num]x38 | 38 | 38 | 0.5 ~ 1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone` | block | [str]x1336 [atom str]x36 | 536 | 1372 | 예: revolute "mgun_rot2" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/animation` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/animation/sequence` | block | [str]x4 | 4 | 4 | "repair" ¦ "break" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/animation/sequence/file` | block | [str]x4 | 4 | 4 | "gun_r.anm" ¦ "gun_b.anm" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/animation/sequence/speed` | block | [num]x4 | 4 | 4 | 0.7 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x435 [atom str]x6 | 118 | 441 | 예: "foresight3" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x71 | 35 | 71 | 예: "at2_6" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x19 | 13 | 19 | "at1_7" ¦ "at2_7" ¦ "object004" ¦ "object005" ¦ "ammo_belt038" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x17 | 11 | 17 | "at1_8" ¦ "at2_8" ¦ "object005" ¦ "object006" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x5 | 5 | 5 | "object006" ¦ "object007" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x5 | 5 | 5 | "object007" ¦ "object008" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x5 | 5 | 5 | "object008" ¦ "object009" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x5 | 5 | 5 | "object009" ¦ "object010" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x5 | 5 | 5 | "object010" ¦ "object011" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x5 | 5 | 5 | "object011" ¦ "object012" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone` | block | [str]x3 | 3 | 3 | "object012" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x3 | 3 | 3 | "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x3 | 3 | 3 | 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x3 | 3 | 3 | "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x5 | 5 | 5 | "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x5 | 5 | 5 | 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x5 | 5 | 5 | "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x5 | 5 | 5 | "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x5 | 5 | 5 | 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x5 | 5 | 5 | "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x5 | 5 | 5 | "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x5 | 5 | 5 | 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x5 | 5 | 5 | "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x3 | 3 | 3 | 1 0 0 0 0.9744 -0.225 0 0.225 0.9744 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x5 | 5 | 5 | "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x2 | 2 | 2 | 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x5 | 5 | 5 | "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x5 | 5 | 5 | 1 0 0 0 0.788 -0.6157 0 0.6157 0.788 0 -0.5381 0 ¦ 1 0 0 0 0.9744 -0.225 0 0.225 0.9744 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x5 | 5 | 5 | "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x5 | 5 | 5 | "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x5 | 5 | 5 | 1 0 0 0 0.8192 -0.5736 0 0.5736 0.8192 0 -0.5381 0 ¦ 1 0 0 0 0.788 -0.6157 0 0.6157 0.788 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x5 | 5 | 5 | "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x5 | 5 | 5 | "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x9 | 7 | 9 | 1 0 0 0 0.9781 -0.2079 0 0.2079 0.9781 0 -0.5381 0 ¦ 1 0 0 0 0.8192 -0.5736 0 0.5736 0.8192 0 -0.5381 0 ¦ 0.9789 -0.1962 -0.0569 0.1801 0.9604 -0.2126 0.0964 0.1979 0.9755 -0.0394 0 3.9567 ¦ 0.8058 -0.592 0.018 0.5787 0.7934 0.1886 -0.1259 -0.1416 0.9819 -0.0591 0.1772 5.1969 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x17 | 11 | 17 | "id=turret;" ¦ "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x8 | 4 | 8 | -0.0394 0 3.9567 ¦ -0.0591 0.1772 5.1969 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x17 | 11 | 17 | "at1_8.ply" ¦ "at2_8.ply" ¦ "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/lodview` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/lodview/volumeview` | block | [str]x8 | 2 | 8 | "ammo_belt010.ply" ¦ "ammo_belt038_lod1.ply" ¦ "ammo_belt038_lod2.ply" ¦ "ammo_belt038_lod3.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x9 | 7 | 9 | 1 0 0 0 0.9703 0.2419 0 -0.2419 0.9703 0 -0.5381 0 ¦ 1 0 0 0 0.9781 -0.2079 0 0.2079 0.9781 0 -0.5381 0 ¦ 0.9789 -0.1962 -0.0569 0.1801 0.9604 -0.2126 0.0964 0.1979 0.9755 -0.0197 -0.0197 2.7362 ¦ 0.8058 -0.592 0.018 0.5787 0.7934 0.1886 -0.1259 -0.1416 0.9819 0 0.0787 2.7165 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x17 | 11 | 17 | "id=turret;" ¦ "id=gun;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x10 | 6 | 10 | -0.0197 -0.0197 2.7362 ¦ 0 0.0787 2.7165 ¦ -0.5391 0 -0.0025 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x17 | 11 | 17 | "at1_7.ply" ¦ "at2_7.ply" ¦ "object001.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/lodview` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/lodview/volumeview` | block | [str]x8 | 2 | 8 | "ammo_belt010.ply" ¦ "ammo_belt038_lod1.ply" ¦ "ammo_belt038_lod2.ply" ¦ "ammo_belt038_lod3.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x31 | 18 | 31 | 예: 1 0 0 0 0.8572 0.515 0 -0.515 0.8572 0 -0.5381 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x53 | 29 | 53 | "id=turret;" ¦ "id=gun;" ¦ "id=body;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x40 | 22 | 40 | 예: -0.0591 0.1772 5.2165 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x53 | 29 | 53 | 예: "at2_6.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/lodview` | block | []x6 | 4 | 6 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/lodview/volumeview` | block | [str]x16 | 4 | 16 | "ammo_belt010.ply" ¦ "ammo_belt038_lod1.ply" ¦ "ammo_belt038_lod2.ply" ¦ "ammo_belt038_lod3.ply" ¦ "step_left4.ply" ¦ "step_left4_lod1.ply" ¦ "step_right4.ply" ¦ "step_right4_lod1.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x278 | 64 | 278 | 예: 1 0 0 0 0 -1 0 1 0 0.6131 0 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x263 | 101 | 263 | 예: "id=turret;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x163 | 84 | 163 | 예: -0.0198 0.1575 5.091 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x363 | 97 | 363 | 예: "at2_5.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/limits` | block | [num num]x16 | 9 | 16 | -15 15 ¦ -10 10 ¦ -5 5 ¦ -20 35 ¦ -10 30 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/lodview` | block | []x226 | 31 | 226 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/lodview/volumeview` | block | [str]x492 | 31 | 492 | 예: "ammo_belt010.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x786 | 264 | 786 | 예: 1 0 0 0 0 1 0 -1 0 0 0 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/orientation` | block | [num num num num num num num num num]x2 | 2 | 2 | 1 0.0063 0.0011 -0.0062 0.9997 -0.0218 -0.0013 0.0218 0.9998 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x731 | 372 | 731 | 예: "id=turret;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x577 | 347 | 577 | 예: 0 0.1378 5.0862 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/speed` | block | [num]x1 | 1 | 1 | 0.01 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/visibility` | block | [num]x4 | 1 | 4 | 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x751 | 381 | 751 | 예: "at2_4.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/limits` | block | [num num]x101 | 92 | 101 | 예: -7 7 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/lodview` | block | []x236 | 59 | 236 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/lodview/volumeview` | block | [str]x623 | 59 | 623 | 예: "bomb_00.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x2397 | 840 | 2397 | 예: 1 0.0095 -0.0018 0.0016 0.0147 0.9999 0.0095 -0.9998 0.0146 11.6345 0.0439 0.4402 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/orientation` | block | [num num num num num num num num num]x43 | 27 | 43 | 1 0 0 0 0 -1 0 1 0 ¦ 0 -1 0 0 0 -1 1 0 0 ¦ -1 0 0 0 0 -1 0 -1 0 ¦ 0.9962 0 -0.0872 0 1 0 0.0872 0 0.9962 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/parameters` | block | [str]x2796 | 1036 | 2796 | 예: "id=body;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/bone/position` | block | [num num num]x1840 | 848 | 1840 | 예: 1.9845 -0.0038 0.0125 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/speed` | block | [num]x70 | 67 | 70 | 0.003 ~ 0.1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/visibility` | block | [num]x17 | 11 | 17 | 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/bone/volumeview` | block | [str]x2708 | 1041 | 2708 | 예: "geararmr2.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/limits` | block | [num num]x867 [num]x2 | 505 | 869 | 20 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/lodview` | block | []x1389 | 273 | 1389 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/lodview/volumeview` | block | [str]x2793 | 273 | 2793 | 예: "bomb_01.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x11890 | 1783 | 11890 | 예: 1 0 0 0 0 -1 0 1 0 -1.2365 0.0535 -0.0843 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/orientation` | block | [num num num num num num num num num]x1996 | 506 | 1996 | 예: 1 0 0 0 0 -1 0 1 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/parameters` | block | [str]x15021 | 1842 | 15021 | 예: "id=body;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/bone/position` | block | [num num num]x3176 | 1090 | 3176 | 예: 39.321 -0.1032 0.4816 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/speed` | block | [num]x600 | 305 | 600 | 0.003 ~ 0.14 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/terminator` | block | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/visibility` | block | [num]x77 | 48 | 77 | 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/bone/volumeview` | block | [str]x13587 | 1755 | 13587 | 예: "vickers_k.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/limits` | block | [num num]x9811 [num]x3 | 1442 | 9814 | 15 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/lodview` | block | []x2510 | 270 | 2510 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/lodview/volumeview` | block | [str]x4999 | 270 | 4999 | 예: "gun_rot.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x16834 [num num num num num num num num num atom num num]x3 | 1900 | 16837 | 예: -1 0 0 0 0 1 0 1 0 -5.7122 -0.0057 9.3934 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/orientation` | block | [num num num num num num num num num]x2118 | 598 | 2118 | 예: 1 0 0 0 -1 0 0 0 -1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/parameters` | block | [str]x24970 | 2000 | 24970 | 예: "id=body;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/bone/position` | block | [num num num]x20095 | 1744 | 20095 | 예: 16.7243 -33.9688 -2.7634 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/speed` | block | [num]x8720 | 1311 | 8720 | 0.001 ~ 1.6 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/terminator` | block | []x125 | 119 | 125 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/visibility` | block | [num]x177 | 75 | 177 | 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/volumeview` | block | [str]x21365 | 1939 | 21365 | 예: "mgun_rot.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/bone/volumeview/layer` | block | [atom]x2 | 2 | 2 | waterscissor | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/limits` | block | [num num]x7948 [num]x11 | 1390 | 7959 | 1 ~ 40 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/lodview` | block | []x2836 | 339 | 2836 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/lodview/volumeview` | block | [str]x5652 | 339 | 5652 | 예: "bomb_01.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x21516 | 2010 | 21516 | 예: 1 0 0 0 0 1 0 -1 0 -139.249 25.7262 19.496 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/orientation` | block | [num num num num num num num num num]x32 | 24 | 32 | 1 0 0 0 0 -1 0 1 0 ¦ 1 0 0 0 0 1 0 -1 0 ¦ -1 0 0 0 -1 0 0 0 1 ¦ 0.682 0 0.7314 0 1 0 -0.7314 0 0.682 ¦ 1 0 0 0 0.9962 0.0872 0 -0.0872 0.9962 ¦ -0.9336 0 -0.3584 0 -1 0 -0.3584 0 0.9336 ¦ 0.9336 0 -0.3584 0 1 0 0.3584 0 0.9336 ¦ -0.9336 -0.0125 -0.3581 0 -0.9994 0.0349 -0.3584 0.0326 0.933 ¦ 0.9337 0.0038 -0.3581 -0.0174 0.9992 -0.0349 0.3577 0.0388 0.933 ¦ 1 0 0 0 -1 0 0 0 1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/parameters` | block | [str]x30426 | 2075 | 30426 | 예: "id=body;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/position` | block | [num num num]x25913 | 1996 | 25913 | 예: -46.8368 0.0922 15.2546 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/speed` | block | [num]x6895 | 1314 | 6895 | 0.001 ~ 2 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/speed2` | block | [num]x38 | 8 | 38 | 2 ~ 40 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/bone/terminator` | block | []x41 | 41 | 41 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/visibility` | block | [num]x177 | 57 | 177 | 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/volumeview` | block | [str]x26271 | 2028 | 26271 | 예: "turret.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/volumeview/ground` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/bone/volumeview/layer` | block | [atom]x14 | 14 | 14 | waterscissor | 미확인(자동) |  |  |
| `/skeleton/bone/bone/limits` | block | [num num]x77 | 17 | 77 | -8 8 ¦ -10 10 ¦ -5 5 ¦ -15 15 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/lodview` | block | []x408 | 241 | 408 | 예: | 미확인(자동) |  |  |
| `/skeleton/bone/bone/lodview/volumeview` | block | [str]x827 | 241 | 827 | 예: "body.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x5030 [num num num num num num num num num atom num num]x2 [num num num atom num num num num num num num num]x1 | 1314 | 5033 | 예: 0 1 0 -1 0 0 0 0 1 107.084 -0.0493 43.1862 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/orientation` | block | [num num num num num num num num num]x6 | 6 | 6 | 1 0 0 0 0 1 0 -1 0 ¦ -1 0 0 0 0 1 0 1 0 ¦ 1 0 0 0 1 0 0 0 1 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/parameters` | block | [str]x5045 | 2128 | 5045 | 예: "id=body;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/position` | block | [num num num]x6274 | 2030 | 6274 | 예: 8.7001 -0.0084 28.9026 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/speed` | block | [num]x3 | 3 | 3 | 0.003 ~ 0.01 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/speed2` | block | [num]x6 | 6 | 6 | 40 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/bone/visibility` | block | [num]x531 | 107 | 531 | 0 | 미확인(자동) |  |  |
| `/skeleton/bone/bone/volumeview` | block | [str]x2450 | 1934 | 2450 | 예: "body.ply" | 미확인(자동) |  |  |
| `/skeleton/bone/limits` | block | [num num]x135 | 135 | 135 | 0 0 ¦ -0 0 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/orientation` | block | [num num num num num num num num num]x2141 | 2141 | 2141 | 1 0 0 0 -1 0 0 0 1 | 미확인(자동) |  |  |
| `/skeleton/bone/parameters` | block | [str]x1616 | 1616 | 1616 | 예: "steermax=15;" | 추정(파일 주석) | "} ¦ radius=0.17;"} | 게임 파일 내 주석(같은 이름의 키) |
| `/skeleton/bone/speed` | block | [num]x97 | 97 | 97 | 0.0005 ~ 0.01 | 미확인(자동) |  |  |
| `/skeleton/bone/speed2` | block | [num]x6 | 6 | 6 | 5 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/volume` | block | [str]x44932 | 1738 | 44932 | 예: "1blade1" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/bone` | block | [str]x44932 | 1738 | 44932 | 예: "propeller1" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/volume/box` | block | [num num num]x10147 | 1327 | 10147 | 예: 6.35309 33.7371 3.28608 | 미확인(자동) |  |  |
| `/volume/cylinder` | block | [num num]x16323 | 1698 | 16323 | 예: 6.63515 11.221 | 미확인(자동) |  |  |
| `/volume/matrix34` | block | [num num num num num num num num num num num num]x27715 [num num num num num num num num atom num num num]x1 | 1710 | 27716 | 예: 0 -1 0 0 0 1 -1 0 0 0.1603 0.1964 -17.0191 | 미확인(자동) |  |  |
| `/volume/orientation` | block | [num num num num num num num num num]x387 | 192 | 387 | 예: 1 0 0 0 0 1 0 -1 0 | 미확인(자동) |  |  |
| `/volume/polyhedron` | block | [str]x18460 | 1356 | 18460 | 예: "body1.vol" | 미확인(자동) |  |  |
| `/volume/position` | block | [num num num]x16484 | 1367 | 16484 | 예: -34.5316 -0.004 7.0221 | 미확인(자동) |  |  |
| `/volume/sphere` | block | [num]x2 | 2 | 2 | 2.2376 | 미확인(자동) |  |  |

### 파일 종류: .mtl

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/material` | block | [atom]x5358 | 5358 | 5358 | bump ¦ simple ¦ envmap | 미확인(자동) |  |  |
| `/material/blend` | block | [atom]x5357 | 5357 | 5357 | test ¦ blend ¦ none | 미확인(자동) |  |  |
| `/material/bump` | block | [str]x4176 | 4176 | 4176 | 예: "$/_vehicle/blenheim_mk1_bp" | 미확인(자동) |  |  |
| `/material/color` | block | [str]x4175 | 4175 | 4175 | 예: "230 230 230 80" | 미확인(자동) |  |  |
| `/material/diffuse` | block | [str]x5358 | 5358 | 5358 | 예: "$/_vehicle/eng/blenheim_mk1_eng" | 미확인(자동) |  |  |
| `/material/diffuse/alphachannel` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/material/envmap` | block | [str]x204 | 204 | 204 | "$/_vehicle/envmap_gold01" ¦ "$/_vehicle/general/envmap_gold01" | 미확인(자동) |  |  |
| `/material/full_specular` | block | []x12 | 12 | 12 | 예: | 미확인(자동) |  |  |
| `/material/lightmap` | block | [str]x8 | 8 | 8 | "$/_lm/wood_crate1_lm" ¦ "$/_lm/wood_crate1b_lm" | 미확인(자동) |  |  |
| `/material/mask` | block | [str]x222 | 222 | 222 | 예: "$/_vehicle/eng/blenheim_mk1_eng_sp" | 미확인(자동) |  |  |
| `/material/specular` | block | [str]x4185 | 4185 | 4185 | 예: "$/_vehicle/eng/blenheim_mk1_eng_sp" | 미확인(자동) |  |  |
| `/material/tile` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |

## 펼친 뒤의 키

include와 매크로를 펼친 뒤 유닛 안에서의 위치다. 펼치는 규칙은 추정이며 한계는 coverage.md에 있다. 고유 키 704개.

### 유닛 종류: vehicle

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 |
|---|---|---|---|---|---|---|---|---|
| `/actor` | block | []x52 [atom]x6 | 58 | 58 | ¦ actor | 미확인(자동) |  |  |
| `/actor/(mod)` | mod | [str]x2 [atom str]x2 | 4 | 4 | "mp" ¦ not "mp" | 미확인(자동) |  |  |
| `/actor/##` | block | [atom str]x58 | 58 | 58 | extender "detail" | 추정(파일 주석) | No longer used? | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##/random` | block | []x58 | 58 | 58 | 예: | 미확인(자동) |  |  |
| `/actor/##bone` | block | [str]x6 | 6 | 6 | "commander_rot" | 미확인(자동) |  |  |
| `/actor/##bone/speed2` | block | [num]x6 | 6 | 6 | 180 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##volume` | block | [str]x2 | 2 | 2 | "engine3" | 추정(파일 주석) | Engine Cover | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##volume/thickness` | block | [num]x2 | 2 | 2 | 10 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##weaponry` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/actor/##weaponry/place` | block | [str]x5 | 5 | 5 | "mgun" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##weaponry/place/accuracybymotion` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/actor/##weaponry/place/accuracybymotion/byangularspeed` | block | [num]x5 | 5 | 5 | 0.5 | 미확인(자동) |  |  |
| `/actor/##weaponry/place/accuracybymotion/bylinearspeed` | block | [num]x5 | 5 | 5 | 0.4 | 미확인(자동) |  |  |
| `/actor/##weaponry/place/accuracybymotion/disordertime` | block | [num]x5 | 5 | 5 | 0.6 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##weaponry/place/accuracybymotion/maxangularspeed` | block | [num]x5 | 5 | 5 | 30 | 추정(파일 주석) | deg/s | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##weaponry/place/accuracybymotion/maxlinearspeed` | block | [num]x5 | 5 | 5 | 8 | 추정(파일 주석) | km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##weaponry/place/accuracybymotion/ordertime` | block | [num]x5 | 5 | 5 | 2 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##weaponry/place/charger` | block | [str]x5 | 5 | 5 | "gunner" | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/##weaponry/place/gunner` | block | [str]x5 | 5 | 5 | "gunner" | 미확인(자동) |  |  |
| `/actor/##weaponry/place/linkanimation` | block | [str]x5 | 5 | 5 | "off" | 미확인(자동) |  |  |
| `/actor/##weaponry/place/linkbone` | block | [str]x5 | 5 | 5 | "mgun" | 미확인(자동) |  |  |
| `/actor/##weaponry/place/rotate` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/actor/##weaponry/place/type` | block | [str]x5 | 5 | 5 | "main" | 추정(파일 주석) | "unit tag" or "unit type" + "current main weapon tag" | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/able` | block | [str str str str]x58 [str str]x36 [str]x24 | 58 | 118 | "emit_passangers_on_enemy" "repaired" "minimap" "ally_crew" ¦ "talk" "-emit_passangers_on_enemy" ¦ "talk" ¦ "emit_crew_on_injure" ¦ "emit_on_attack" | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/armor` | block | []x74 | 58 | 74 | 예: | 미확인(자동) |  |  |
| `/actor/armor/setting` | block | [str]x3626 | 58 | 3626 | 예: "body" | 미확인(자동) |  |  |
| `/actor/armor/setting/threshold` | block | []x3626 | 58 | 3626 | 예: | 미확인(자동) |  |  |
| `/actor/armor/setting/threshold/blast` | block | []x3626 | 58 | 3626 | 예: | 추정(파일 주석) | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/armor/setting/threshold/blast/koef` | block | [num]x3626 | 58 | 3626 | 0.5 ~ 30 | 미확인(자동) |  |  |
| `/actor/armor/setting/threshold/pierce` | block | []x3626 | 58 | 3626 | 예: | 추정(파일 주석) | Minimum penetration necessary to leave a ricochet/pen mark on the armour | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/armor/setting/threshold/pierce/koef` | block | [num]x3626 | 58 | 3626 | 0.8 ~ 1 | 미확인(자동) |  |  |
| `/actor/armor/setting/volumes` | block | [str]x3700 | 58 | 3700 | 예: "body*" | 추정(파일 주석) | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder` | block | []x730 | 58 | 730 | 예: | 미확인(자동) |  |  |
| `/actor/boarder/anm` | block | [str]x513 | 56 | 513 | 예: "gunner" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder/anm/forward` | block | []x513 | 56 | 513 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder/anm/forward/base` | block | [str]x14 | 14 | 14 | "open_driver" | 추정(파일 주석) | seconds : CP | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder/anm/forward/begin` | block | [str]x96 | 14 | 96 | "board_tank" ¦ "board_HighArmor" ¦ "board_LowArmor" ¦ "board_Kv-85" ¦ "board_Is-2" ¦ "board_t26_driver" ¦ "board_tank_right" ¦ "board_kv2_driver" ¦ "board_MiddleArmor" ¦ "board_tank_left" ¦ "board_t34_driver" ¦ "board_kv" | 미확인(자동) |  |  |
| `/actor/boarder/anm/forward/end` | block | [str num]x367 [str]x106 | 56 | 473 | 예: "pose_qf2_gunner" 10 | 미확인(자동) |  |  |
| `/actor/boarder/anm/reverse` | block | []x96 | 14 | 96 | 예: | 미확인(자동) |  |  |
| `/actor/boarder/anm/reverse/base` | block | [str num]x6 [str]x4 | 10 | 10 | "open_driver" -1 ¦ "close_driver" | 추정(파일 주석) | seconds : CP | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder/anm/reverse/end` | block | [str]x90 [str num]x6 | 14 | 96 | "emit_tank_1_hold" ¦ "emit_body" ¦ "emit_tank_2_hold" ¦ "emit_tank_2" ¦ "board_t26_driver" -1 ¦ "emit_tank_1" ¦ "emit_t34_hold" | 미확인(자동) |  |  |
| `/actor/boarder/door` | block | [str]x659 | 56 | 659 | "emit1" ¦ "emit2" ¦ "emit3" ¦ "emit4" ¦ "emit5" ¦ "emit6" ¦ "emit0" ¦ "emit7" ¦ "emit8" ¦ "emit9" | 미확인(자동) |  |  |
| `/actor/boarder/link` | block | [str str]x957 | 56 | 957 | 예: "emit1" "gunner" | 추정(파일 주석) | { link <from> <to> } | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder/link/anm` | block | [str]x659 | 56 | 659 | 예: "gunner" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder/link/forward` | block | [atom]x659 | 56 | 659 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/boarder/link/reverse` | block | [atom]x659 | 56 | 659 | puton | 미확인(자동) |  |  |
| `/actor/bone` | block | [str]x134 | 52 | 134 | "turret" ¦ "gun_rot" ¦ "basis" ¦ "aux_turret1" ¦ "aux_turret2" ¦ "foresight" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/bone/limits` | block | [num num]x78 | 52 | 78 | 예: -13 15 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/bone/speed2` | block | [num]x134 | 52 | 134 | 1.75 ~ 60 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/brain` | block | [str]x58 []x2 | 58 | 60 | "vehicle" ¦ | 미확인(자동) |  |  |
| `/actor/brain/const` | block | [str num]x116 | 58 | 116 | "crush/min_danger_zone_time" 1 ¦ "crush/min_danger_zone_length" 1 | 추정(파일 주석) | detect explosives inside 7 meter radius ¦ hide if danger is more than "x" meters away, otherwise run away | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/brain/properties` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |
| `/actor/brain/properties/fire_mode` | block | [atom]x2 | 2 | 2 | hold | 미확인(자동) |  |  |
| `/actor/brain/state` | block | [str]x58 | 58 | 58 | "vehicle" | 추정(파일 주석) | 2hand & 1hand items ¦ to enable item ops in snipe cover | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis` | block | [str]x58 []x19 | 58 | 77 | "track" ¦  ¦ "human_carrier" ¦ "wheel" | 추정(파일 주석) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/fuelbag` | block | []x36 | 22 | 36 | 예: | 미확인(자동) |  |  |
| `/actor/chassis/fuelbag/fuel` | block | [str]x36 | 22 | 36 | "fuel" ¦ "diesel" | 추정(파일 주석) | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/fuelbag/rate` | block | [num]x36 | 22 | 36 | 0 ~ 72.22222 | 추정(파일 주석) | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/fuelbag/remain` | block | [num]x36 | 22 | 36 | 1 ~ 650 | 추정(파일 주석) | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/fuelbag/volume` | block | [num]x36 | 22 | 36 | 1 ~ 650 | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion` | block | []x63 | 58 | 63 | 예: | 미확인(자동) |  |  |
| `/actor/chassis/locomotion/braketime` | block | [num]x63 | 58 | 63 | 0.2532 ~ 2.7 | 추정(파일 주석) | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/fallslope` | block | [num]x63 | 58 | 63 | 70 ~ 75 | 추정(파일 주석) | Maximum slope before physics kick in | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/fast` | block | [num]x63 | 58 | 63 | 2.8 ~ 49.5 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/gears` | block | [num num num num]x47 [num num num num num]x14 [num num]x2 | 58 | 63 | 0.35 0.5 0.7 1.0 ¦ 0.5 0.65 0.8 0.9 1.0 ¦ 0.15 1 | 추정(파일 주석) | Gearing affects acceleration curve. ¦ Forward gear ratios | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/gearswitchdelay` | block | [num]x16 | 16 | 16 | 0.5 | 추정(파일 주석) | Time to switch between forward and reverse. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/maxslope` | block | [num]x63 | 58 | 63 | 60 ~ 75 | 추정(파일 주석) | Maximum slope to drive up hill | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/maxspeed` | block | [num]x63 | 58 | 63 | 2.8 ~ 55 | 추정(파일 주석) | Max speed. ¦ max speed while in flight mode in kph | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/maxspeedatmaxturn` | block | [num]x16 | 16 | 16 | 5.26314 ~ 52.436956 | 추정(파일 주석) | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/normal` | block | [num]x63 | 58 | 63 | 2.8 ~ 33 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/reargears` | block | [num num]x61 [num]x2 | 58 | 63 | 0.320755 | 추정(파일 주석) | Reverse max speed. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/slow` | block | [num]x63 | 58 | 63 | 0.924 ~ 10 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/splineradius` | block | [num]x2 | 2 | 2 | 0.75 | 추정(파일 주석) | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/starttime` | block | [num]x63 | 58 | 63 | 1.258 ~ 20.792308 | 추정(파일 주석) | Time for vehicle to accelerate from 0kph to max speed. ¦ acceleration time from 0 to max. speed, seconds | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/steeroff` | block | [num]x2 | 2 | 2 | 25 | 추정(파일 주석) | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/steeron` | block | [num]x2 | 2 | 2 | 10 | 추정(파일 주석) | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/stoptime` | block | [num]x63 | 58 | 63 | 0.358 ~ 3.82 | 추정(파일 주석) | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/turnradius` | block | [num]x63 | 58 | 63 | 2.0064 ~ 6.05 | 추정(파일 주석) | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/turnstart` | block | [num]x61 | 56 | 61 | 0.655818 ~ 3.173636 | 추정(파일 주석) | Direct Control, Time for vehicle to from 0Â°/s rotation speed to max rotation speed. ¦ / | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/turnstop` | block | [num]x61 | 56 | 61 | 0.655818 ~ 3.173636 | 추정(파일 주석) | Direct Control, Time for vehicle to from max rotation speed to 0Â°/s rotation speed to max rotation speed. ¦ / | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/locomotion/turntime` | block | [num]x61 | 56 | 61 | 3.635429 ~ 45.617143 | 추정(파일 주석) | Direct Control, Time it takes to do a full 360 degree at max turn speed, fetching custom define | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/track` | block | [str]x100 | 44 | 100 | "wlf" ¦ "wrf" ¦ "left" ¦ "right" ¦ "wlr" ¦ "wrr" | 미확인(자동) |  |  |
| `/actor/chassis/track/texturescroller` | block | []x28 | 14 | 28 | 예: | 미확인(자동) |  |  |
| `/actor/chassis/track/texturescroller/bone` | block | [str]x28 | 14 | 28 | "TrackL" ¦ "TrackR" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/track/texturescroller/length` | block | [num]x28 | 14 | 28 | 0.37 ~ 1.15 | 추정(파일 주석) | 0.6 meters per animation ¦ meters per animation | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/track/tracer` | block | []x100 | 44 | 100 | 예: | 미확인(자동) |  |  |
| `/actor/chassis/track/tracer/bone` | block | [str]x100 | 44 | 100 | "FXtraceL1" ¦ "FXtraceR1" ¦ "FXtraceL2" ¦ "FXtraceR2" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/track/tracer/step` | block | [num]x100 | 44 | 100 | 0.08 ~ 0.52 | 추정(파일 주석) | in meters ¦ 0.7 meters | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/chassis/track/tracer/view` | block | [str]x100 | 44 | 100 | "tracewheel05a" ¦ "tracewheel06" ¦ "tracewheel02a" ¦ "tracewheel03" ¦ "tracks_big" ¦ "tracks_sm2" ¦ "tracks_mid" ¦ "tracewheel02" ¦ "tracks_sm" | 추정(파일 주석) | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/collider` | block | [str]x136 | 58 | 136 | "vehicle" ¦ "cannon" ¦ "cannon_small" ¦ "tank.medium" ¦ "tank.light" ¦ "tank.heavy" ¦ "armored" | 미확인(자동) |  |  |
| `/actor/component` | block | [str]x545 | 58 | 545 | 예: "body" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/component/repairorder` | block | [num]x537 | 58 | 537 | 0 ~ 11 | 미확인(자동) |  |  |
| `/actor/component/repairtime` | block | [num]x537 | 58 | 537 | 1 ~ 130 | 추정(파일 주석) | 45 ¦ 30 | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/component/tags` | block | [str]x256 [str str]x90 | 44 | 346 | "shield" ¦ "mover" "track" ¦ "ammo" ¦ "wheel" ¦ "wheel" "no_target" ¦ "fuel" ¦ "transmission" ¦ "cut_part" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/constrain` | block | [atom]x11 | 11 | 11 | ground | 미확인(자동) |  |  |
| `/actor/decalview` | block | [str]x168 [atom]x5 | 58 | 173 | "selection_active" ¦ "selection_selected" ¦ "selection_selectable" ¦ supply_zone | 미확인(자동) |  |  |
| `/actor/decalview/dynamic` | block | []x168 | 58 | 168 | 예: | 미확인(자동) |  |  |
| `/actor/decalview/ground` | block | []x168 | 58 | 168 | 예: | 미확인(자동) |  |  |
| `/actor/decalview/layer` | block | [atom]x173 | 58 | 173 | alpha ¦ grounddynamic | 미확인(자동) |  |  |
| `/actor/decalview/offset` | block | [num num num]x168 | 58 | 168 | 0 0 10 ¦ 0 0 1 | 미확인(자동) |  |  |
| `/actor/decalview/sprite` | block | [str]x173 | 58 | 173 | "vehicle_selected.ebm" ¦ "cannon_selected.ebm" ¦ "vehicle_small_selected.ebm" ¦ "cannon_elongated_selected.ebm" ¦ "vehicle_selectable.ebm" ¦ "special.ebm" ¦ "cannon_selectable.ebm" ¦ "vehicle_small_selectable.ebm" ¦ "cannon_elongated_selectable.ebm" ¦ "vehicle_elongated_selected.ebm" ¦ "supply_zone.ebm" ¦ "vehicle_elongated_selectable.ebm" | 미확인(자동) |  |  |
| `/actor/decalview/sprite/light` | block | [num]x168 | 58 | 168 | 1 | 미확인(자동) |  |  |
| `/actor/decalview/sprite/prescalex` | block | [num]x168 | 58 | 168 | 0.21 ~ 0.42 | 미확인(자동) |  |  |
| `/actor/decalview/sprite/prescaley` | block | [num]x168 | 58 | 168 | 0.21 ~ 0.39 | 미확인(자동) |  |  |
| `/actor/decalview/sprite/preturn` | block | [num]x12 | 6 | 12 | 90 | 미확인(자동) |  |  |
| `/actor/decalview/sprite/visibility` | block | [num]x5 | 5 | 5 | 0.25 | 미확인(자동) |  |  |
| `/actor/decalview/usedirection` | block | []x168 | 58 | 168 | 예: | 미확인(자동) |  |  |
| `/actor/decalview/visible` | block | [num]x173 | 58 | 173 | 0 | 추정(파일 주석) | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[cannon]` | block | [str]x47 | 42 | 47 | "cannon" | 미확인(자동) |  |  |
| `/actor/extender[cannon]/animation` | block | []x47 | 42 | 47 | 예: | 미확인(자동) |  |  |
| `/actor/extender[cannon]/animation/aim` | block | [str str str str]x24 [str str]x16 [str str str str str]x5 [str str str]x2 | 42 | 47 | "" "" ¦ "cannon_small_gunner_aim" "cannon_small_charger_aim" "cannon_small_charger2_aim" "cannon_small_commander_aim" ¦ "" "cannon_small_charger_aim" "cannon_small_charger2_aim" "cannon_small_commander_aim" ¦ "cannon_m42_gunlayer" "" "" "" ¦ "" "" "" "" ¦ "cannon_m5_gunlayer" "" "" "" ¦ "AAgun_61k_gunner_aim" "AAgun_61k_charger_aim" "flak_charger_aim" "flak_gunner2_aim" "flak_commander_aim" ¦ "mg_lafette_gunner_aim" "mg_lafette_commander_aim" ¦ "AAgun_61k_gunner_aim" "AAgun_61k_charger_aim" "" ¦ "cannon_zis3_gunlayer" "" "" "" "" ¦ "mortar_82bm37_gunner_aim" "mortar_82bm37_commander_aim" | 미확인(자동) |  |  |
| `/actor/extender[cannon]/animation/fire` | block | [str str str str]x24 [str str]x13 [str str str str str]x5 [str str str]x2 | 42 | 44 | "" "" ¦ "cannon_small_gunner_fire" "cannon_small_charger_fire" "cannon_small_charger2_fire" "cannon_small_commander_fire" ¦ "" "cannon_small_charger_fire" "cannon_small_charger2_fire" "cannon_small_commander_fire" ¦ "new_cannon_gunner_fire" "new_cannon_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "" "new_cannon_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "cannon_gunlayer_fire-alert" "cannon_pdr25_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "AAgun_61k_gunner_fire" "AAgun_61k_charger_fire" "flak_charger_fire" "flak_gunner2_fire" "flak_commander_fire" ¦ "AAgun_61k_gunner_fire" "AAgun_61k_charger_fire" "" ¦ "cannon_gunlayer_fire-alert" "flak_gunner2_fire" "flak_charger_fire" "" "flak_commander_fire" ¦ "mortar_82bm37_gunner_fire" "mortar_82bm37_commander_fire" | 추정(파일 주석) | mg_maxim_gunner_fire | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[cannon]/animation/move` | block | [str str str str]x24 [str str]x16 [str str str str str]x5 [str str str]x2 | 42 | 47 | "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mgun_browning_driver_l" "mgun_browning_driver_r" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mortar_driver_l" "mgun_browning_driver_r" ¦ "ammo_box_commander_push" "ammo_box_gunner_push" ¦ "mgun_browning_driver_r" "mgun_browning_driver_l" ¦ "" "" "" "" "" ¦ "mgun_browning_driver_r" "mortar_driver_l" ¦ "cannon_pak40_pusher_right" "cannon_pak40_pusher_left" "mortar_driver_l" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mgun_browning_driver_l" "mgun_browning_driver_l" "mgun_browning_driver_r" ¦ "mortar_driver_r" "mortar_driver_l" | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[cannon]/animation/reload` | block | [str str str str]x24 [str str]x16 [str str str str str]x5 [str str str]x2 | 42 | 47 | "cannon_small_gunner_reload" "cannon_small_charger_reload" "cannon_small_charger2_reload" "cannon_small_commander_reload" ¦ "" "" ¦ "" "cannon_small_charger_reload" "cannon_small_charger2_reload" "cannon_small_commander_reload" ¦ "new_cannon_gunner_reload" "new_cannon_charger_reload" "new_cannon_charger2_reload" "new_cannon_commander_reload" ¦ "mgun_stan_charger" "" ¦ "" "new_cannon_charger_reload" "new_cannon_charger2_reload" "new_cannon_commander_reload" ¦ "" "cannon_pdr25_charger" "new_cannon_charger2_reload" "new_cannon_commander_reload" ¦ "AAgun_61k_gunner_reload" "AAgun_61k_charger_reload" "flak_charger_reload" "flak_gunner2_reload" "flak_commander_reload" ¦ "mg_lafette_gunner_reload" "mg_lafette_commander_reload" ¦ "AAgun_61k_gunner_reload" "AAgun_61k_charger_reload" "" ¦ "" "flak_gunner2_reload" "flak_charger_reload" "" "flak_commander_reload" ¦ "" "mortar_82bm37_commander_reload" | 미확인(자동) |  |  |
| `/actor/extender[cannon]/animation/switch` | block | [str]x44 | 42 | 44 | "close" | 미확인(자동) |  |  |
| `/actor/extender[cannon]/attack` | block | []x44 | 42 | 44 | 예: | 추정(파일 주석) | fire/attack state: cannon aim, fire, and reload events | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[cannon]/attack/obstacle` | block | [str]x38 | 36 | 38 | "open" | 미확인(자동) |  |  |
| `/actor/extender[cannon]/attack/workers` | block | [str str str str]x24 [str str]x6 [str str str str str]x5 [str str str]x2 | 37 | 37 | "gunner" "charger" "charger2" "commander" ¦ "gunner" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "commander" ¦ "gunner" "charger" "commander" | 미확인(자동) |  |  |
| `/actor/extender[cannon]/move` | block | []x44 | 42 | 44 | 예: | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[cannon]/move/obstacle` | block | [str]x44 | 42 | 44 | "close" | 미확인(자동) |  |  |
| `/actor/extender[cannon]/move/workers` | block | [str str str str atom num]x24 [str str atom num]x11 [atom]x3 [str str str atom num]x2 [str str str str str atom num]x2 | 42 | 42 | "driver1" "driver2" "driver3" "driver4" minWorkers 2 ¦ "driver1" "driver2" minWorkers 2 ¦ clear ¦ "driver1" "driver2" "driver3" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 2 | 미확인(자동) |  |  |
| `/actor/extender[cannon]/rotate_ik_limit` | block | [num]x5 | 5 | 5 | 0 | 미확인(자동) |  |  |
| `/actor/extender[cannon]/trailing` | block | []x31 | 31 | 31 | 예: | 추정(파일 주석) | trailing state: cannon is being towed {obstacle} not needed. Uses "close" | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[cannon]/trailing/workers` | block | [str str str str]x24 [atom]x3 [str str str]x2 [str str str str str]x2 | 31 | 31 | "seat1" "seat2" "seat3" "seat4" ¦ clear ¦ "seat1" "seat2" "seat3" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" | 미확인(자동) |  |  |
| `/actor/extender[enumerator]` | block | [str]x10 | 10 | 10 | "enumerator" | 미확인(자동) |  |  |
| `/actor/extender[enumerator]/digit_folder` | block | [str]x10 | 10 | 10 | "ru1" | 미확인(자동) |  |  |
| `/actor/extender[inventory]` | block | [str]x187 | 58 | 187 | "inventory" | 미확인(자동) |  |  |
| `/actor/extender[inventory]/box` | block | []x187 | 58 | 187 | 예: | 미확인(자동) |  |  |
| `/actor/extender[inventory]/box/item` | block | [str num]x242 [str atom str num]x3 | 52 | 245 | 예: "repair_kit" 3 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[inventory]/box/item/user` | block | [str]x3 | 3 | 3 | "mgun" | 미확인(자동) |  |  |
| `/actor/extender[inventory]/box/size` | block | [num num]x116 | 58 | 116 | 9 15 ¦ 9 6 ¦ 9 12 ¦ 6 10 ¦ 8 8 ¦ 9 10 ¦ 4 4 | 추정(파일 주석) | size of the window | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[inventory]/box/weight` | block | [num]x58 | 58 | 58 | 1000 ~ 15000 | 추정(파일 주석) | will stack up in 100/2 = 2 on humans | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[inventory]/weapon` | block | [str]x61 | 58 | 61 | "mgun" | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[inventory]/weapon/mask` | block | [str]x61 | 58 | 61 | "mountable" | 미확인(자동) |  |  |
| `/actor/extender[shield]` | block | [str]x6 | 6 | 6 | "shield" | 미확인(자동) |  |  |
| `/actor/extender[shield]/on` | block | []x6 | 6 | 6 | 예: | 추정(파일 주석) | event created by animation ¦ overwrites and stops dust fx on ground from firing main gun | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[supply_zone]` | block | [str]x5 | 5 | 5 | "supply_zone" | 미확인(자동) |  |  |
| `/actor/extender[supply_zone]/(mod)` | mod | [str]x5 | 5 | 5 | "gameclass:campaign_capture_the_flag" | 미확인(자동) |  |  |
| `/actor/extender[supply_zone]/limit` | block | [num]x5 | 5 | 5 | 250 | 추정(파일 주석) | max number of resupplies (0 = unlimited) ¦ Max CP | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[supply_zone]/radius` | block | [num]x5 | 5 | 5 | 10 | 추정(파일 주석) | resupply radius in meter | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[supply_zone]/regenerationperiod` | block | [num]x10 | 5 | 10 | 0 ~ 10 | 추정(파일 주석) | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[supply_zone]/resupply` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |
| `/actor/extender[supply_zone]/resupply/(mod)` | mod | [atom str]x10 [str]x5 | 5 | 15 | not "mp" ¦ "mp" | 미확인(자동) |  |  |
| `/actor/extender[supply_zone]/resupply/item` | block | [str str str num]x1440 [str str num]x485 [str str str str num]x425 [str num]x55 | 5 | 2405 | 예: "flame_thrower" "ammo" 50 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[supply_zone]/resupply/item/value` | block | [num]x2405 | 5 | 2405 | 1 ~ 350 | 미확인(자동) |  |  |
| `/actor/extender[supply_zone]/resupplyperiod` | block | [num]x5 | 5 | 5 | 2 | 추정(파일 주석) | resupply period in seconds | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/extender[talk]` | block | [str]x58 | 58 | 58 | "talk" | 미확인(자동) |  |  |
| `/actor/extender[talk]/talker` | block | [str]x58 | 58 | 58 | "cannon" ¦ "tank" | 미확인(자동) |  |  |
| `/actor/extension` | block | [str]x60 | 58 | 60 | 예: "40mm_qf2.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/hearingbias` | block | [num]x16 | 16 | 16 | 0 ~ 2 | 추정(파일 주석) | reduces units ability to hear sound signals. value = meters ¦ in meters | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/lodclass` | block | [str]x100 | 58 | 100 | "vehicle" ¦ "cannon" | 미확인(자동) |  |  |
| `/actor/mass` | block | [num]x64 | 58 | 64 | 32 ~ 47500 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/nogroupmesh` | block | [num]x58 | 58 | 58 | 1 | 미확인(자동) |  |  |
| `/actor/obstacleid` | block | [str]x64 | 58 | 64 | "tank" | 미확인(자동) |  |  |
| `/actor/patherid` | block | [str]x140 | 58 | 140 | 예: "vehicle.large" | 미확인(자동) |  |  |
| `/actor/perks` | block | []x168 | 58 | 168 | 예: | 미확인(자동) |  |  |
| `/actor/perks/blastwave_modifier` | block | [num]x58 | 58 | 58 | 6 ~ 20 | 미확인(자동) |  |  |
| `/actor/perks/component` | block | [str]x546 [atom str]x444 | 58 | 990 | 예: "body" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/perks/component/armor_damage_modifier` | block | [num]x926 | 58 | 926 | 0.1 ~ 5 | 추정(파일 주석) | multiplier adjusts ratio of which part of penetration does damage to component. (shell penetration - armor thickness) verses minimumDamageModifier. values < 1 = less damage done by over penetration. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/perks/component/health` | block | []x990 | 58 | 990 | 예: | 추정(파일 주석) | A better way to translate this is that it will deal 2x damage to the enemy. In the editor it is hard to understand, as if you're switching between players it will always display the hitpoints of "player". But the enemy units on that particular instance will have half the total health. ¦ health multiplier from soldier settings | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/perks/component/health/hp` | block | [num]x990 | 58 | 990 | 50 ~ 450 | 미확인(자동) |  |  |
| `/actor/perks/component/health/hp_broken` | block | [num]x372 [expr]x32 | 58 | 404 | 1 ~ 450 | 추정(파일 주석) | destroys component and cannot be repaired | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/placer` | block | []x660 | 58 | 660 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/placer/crew` | block | []x58 | 58 | 58 | 예: | 미확인(자동) |  |  |
| `/actor/placer/place` | block | [str]x982 [str atom]x30 | 56 | 1012 | 예: "gunner" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/placer/place/group` | block | [str]x555 | 56 | 555 | "crew" ¦ "passenger" | 추정(파일 주석) | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/placer/place/linkbone` | block | [str]x485 | 56 | 485 | 예: "gunner" | 미확인(자동) |  |  |
| `/actor/placer/place/openforcrush` | block | []x137 | 36 | 137 | 예: | 미확인(자동) |  |  |
| `/actor/placer/place/visor` | block | [str]x103 | 47 | 103 | "main" ¦ "driver" ¦ "around" ¦ "none" ¦ "back" ¦ "gunner3" ¦ "gunner4" | 미확인(자동) |  |  |
| `/actor/props` | block | [str str str str]x65 [str str]x32 [str str str]x32 [str str str str str str str str str]x14 [str str str str str str str str str str str]x12 | 58 | 191 | 예: "steel" "vehicle" "obstacle_ignore_height" "emit_on_sink" | 미확인(자동) |  |  |
| `/actor/rimlight` | block | []x58 | 58 | 58 | 예: | 미확인(자동) |  |  |
| `/actor/sensor` | block | []x118 | 58 | 118 | 예: | 미확인(자동) |  |  |
| `/actor/sensor/informrange` | block | [num num]x58 | 58 | 58 | 50 75 | 추정(파일 주석) | first = AI , second = USER | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/sensor/recognizetime` | block | [num]x58 | 58 | 58 | 0.01 | 미확인(자동) |  |  |
| `/actor/sensor/visor` | block | [str]x202 | 58 | 202 | "hearing" ¦ "main" ¦ "driver" ¦ "around" ¦ "human" ¦ "none" ¦ "back" ¦ "gunner3" ¦ "gunner4" | 미확인(자동) |  |  |
| `/actor/sensor/visor/bone` | block | [str]x181 | 58 | 181 | "hearing_bone" ¦ "visor" ¦ "foresight1" ¦ "visor2" ¦ "visor1" ¦ "visor3" ¦ "foresight6" ¦ "foresight4" ¦ "foresight5" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/sensor/visor/vision` | block | [str]x202 | 58 | 202 | "noise_detection" ¦ "cannon_main" ¦ "tank_range_200" ¦ "tank_driver" ¦ "human" ¦ "none" ¦ "tank_around" ¦ "tank_rear-mg" ¦ "cannon_range_210" ¦ "vehicle_around" ¦ "recon_main" ¦ "cannon_mortar" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/simulation` | block | []x69 | 58 | 69 | 예: | 미확인(자동) |  |  |
| `/actor/simulation/able` | block | [num]x69 | 58 | 69 | 1 | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/simulation/disablelinked` | block | [num]x11 | 11 | 11 | 1 | 미확인(자동) |  |  |
| `/actor/skeleton` | block | []x47 | 47 | 47 | 예: | 미확인(자동) |  |  |
| `/actor/skeleton/bone` | block | [str]x47 | 47 | 47 | "hearing_bone" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/skeleton/bone/position` | block | [num num num]x47 | 47 | 47 | 0 0 40 ¦ 0 0 70 ¦ 0 0 50 | 추정(파일 주석) | X (Forward/Backward) Y (Left/Right) Z (Up/Down) | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/targetclass` | block | [str]x60 [str str]x47 | 58 | 107 | "cannon_small" "ap fg" ¦ "cannon_big" ¦ "tank_medium" "ap fg bullet" ¦ "cannon_bullet" ¦ "cannon_small" ¦ "howitzer" ¦ "tank_light" ¦ "tank_heavy" ¦ "tank_medium" ¦ "cannon_zenite" ¦ "armoredcar" "bullet" ¦ "armoredcar" | 미확인(자동) |  |  |
| `/actor/targetselector` | block | [str]x107 | 58 | 107 | "cannon_small" ¦ "tank_medium" ¦ "cannon_big" ¦ "cannon_bullet" ¦ "tank_light" ¦ "cannon_howitzer" ¦ "cannon_aa" ¦ "cannon_zenite" ¦ "tank_zenite" | 미확인(자동) |  |  |
| `/actor/volume` | block | [str]x13505 | 58 | 13505 | 예: "select" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/able` | block | []x3402 [atom]x1634 | 58 | 5036 | ¦ clear | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/able/contact` | block | [num]x2220 | 47 | 2220 | 0 ~ 1 | 미확인(자동) |  |  |
| `/actor/volume/able/contact_ground` | block | [num]x56 | 14 | 56 | 0 | 미확인(자동) |  |  |
| `/actor/volume/able/hole` | block | []x1508 | 58 | 1508 | 예: | 미확인(자동) |  |  |
| `/actor/volume/able/holed` | block | []x96 | 37 | 96 | 예: | 미확인(자동) |  |  |
| `/actor/volume/able/nolook` | block | [num]x2658 | 58 | 2658 | 1 | 미확인(자동) |  |  |
| `/actor/volume/able/nowounddecal` | block | []x694 | 58 | 694 | 예: | 미확인(자동) |  |  |
| `/actor/volume/able/obstacle` | block | [num]x2112 | 47 | 2112 | 0 | 미확인(자동) |  |  |
| `/actor/volume/able/select` | block | []x58 | 58 | 58 | 예: | 미확인(자동) |  |  |
| `/actor/volume/able/touch` | block | []x58 | 58 | 58 | 예: | 미확인(자동) |  |  |
| `/actor/volume/able/visible` | block | [num]x2668 | 58 | 2668 | 0 | 추정(파일 주석) | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/caststeel` | block | [num]x78 | 8 | 78 | 0.6 ~ 0.8063 | 추정(파일 주석) | effective armor reduction compared to solid plate ¦ effective armor reduction compared to solid plate; 25 + 75 spaced armor array ~86mm effective | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/component` | block | [str]x5724 | 58 | 5724 | 예: "body" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/density` | block | [num]x1624 | 58 | 1624 | 0 | 미확인(자동) |  |  |
| `/actor/volume/highhardness` | block | []x22 | 2 | 22 | 예: | 미확인(자동) |  |  |
| `/actor/volume/material` | block | [str]x771 | 58 | 771 | "wood" ¦ "sand" ¦ "flesh" | 미확인(자동) |  |  |
| `/actor/volume/tags` | block | [str]x5656 [str str]x3308 | 58 | 8964 | 예: "wood" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/thickness` | block | [num]x6252 | 58 | 6252 | 0.5 ~ 105 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/thickness/bottom` | block | [num]x72 | 16 | 72 | 4 ~ 40 | 추정(파일 주석) | Overhang (above tracks) ¦ 4mm + .25" | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/thickness/front` | block | [num]x72 | 14 | 72 | 5 ~ 105 | 추정(파일 주석) | Glacis ¦ Lower front | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/thickness/left` | block | [num]x24 | 8 | 24 | 13 ~ 90 | 추정(파일 주석) | Top sides ¦ side | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/thickness/rear` | block | [num]x52 | 16 | 52 | 4 ~ 70 | 추정(파일 주석) | Front Roof Turret Armor ¦ Bottom Front Nose | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/thickness/right` | block | [num]x18 | 6 | 18 | 17 ~ 90 | 추정(파일 주석) | armor plate sides ¦ Top sides | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/volume/thickness/top` | block | [num]x90 | 16 | 90 | 5 ~ 65 | 추정(파일 주석) | Glacis ¦ front upper part | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry` | block | []x174 | 58 | 174 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/##place` | block | [str]x6 | 6 | 6 | "mgun" | 추정(파일 주석) | maybe future feature | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/##place/ai_in_manual` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/##place/basic` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/##place/foresight` | block | [str]x6 | 6 | 6 | "vision_rot" | 추정(파일 주석) | bone in weapon entity to define origin of fx and bullet ¦ bone for muzzleflash fx | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/##place/gunner` | block | [str]x6 | 6 | 6 | "commander" | 미확인(자동) |  |  |
| `/actor/weaponry/##place/type` | block | [str]x6 | 6 | 6 | "commander" | 추정(파일 주석) | "unit tag" or "unit type" + "current main weapon tag" | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/##place/weapon` | block | [str]x6 | 6 | 6 | "commander_vision" | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/manualaccuracy` | block | [num]x58 | 58 | 58 | 1 | 미확인(자동) |  |  |
| `/actor/weaponry/manualaccuracyzoom` | block | [num]x58 | 58 | 58 | 1.25 | 미확인(자동) |  |  |
| `/actor/weaponry/place` | block | [str]x152 | 58 | 152 | "gun" ¦ "mgun2" ¦ "mgun" ¦ "mgun1" ¦ "mgun3" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/accuracybymotion` | block | []x88 | 52 | 88 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/place/accuracybymotion/byangularspeed` | block | [num]x88 | 52 | 88 | 0.25 ~ 0.55 | 미확인(자동) |  |  |
| `/actor/weaponry/place/accuracybymotion/bylinearspeed` | block | [num]x88 | 52 | 88 | 0.25 ~ 0.55 | 미확인(자동) |  |  |
| `/actor/weaponry/place/accuracybymotion/disordertime` | block | [num]x88 | 52 | 88 | 0.2 ~ 1 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/accuracybymotion/maxangularspeed` | block | [num]x88 | 52 | 88 | 8 ~ 30 | 추정(파일 주석) | deg/s | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/accuracybymotion/maxlinearspeed` | block | [num]x88 | 52 | 88 | 5 ~ 20 | 추정(파일 주석) | km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/accuracybymotion/ordertime` | block | [num]x88 | 52 | 88 | 1 ~ 8 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/ai_in_manual` | block | []x22 | 12 | 22 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/place/basic` | block | []x38 | 18 | 38 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/place/charger` | block | [str]x63 [str atom num]x29 | 56 | 92 | "charger charger2" minWorkers 1 ¦ "commander" ¦ "charger" ¦ "gunner3" ¦ "gunner" ¦ "charger2" | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/foresight` | block | [str]x36 | 16 | 36 | "foresight5" ¦ "foresight4" ¦ "foresight6" ¦ "foresight3" | 추정(파일 주석) | bone in weapon entity to define origin of fx and bullet ¦ bone for muzzleflash fx | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/gunner` | block | [str]x88 [str atom num]x5 | 52 | 93 | "gunner" ¦ "gunner3" ¦ "charger" ¦ "gunner gunner2" minWorkers 1 ¦ "charger2" ¦ "commander" | 미확인(자동) |  |  |
| `/actor/weaponry/place/linkanimation` | block | [str]x9 | 9 | 9 | "off" | 미확인(자동) |  |  |
| `/actor/weaponry/place/linkbone` | block | [str]x9 | 9 | 9 | "mgun" | 미확인(자동) |  |  |
| `/actor/weaponry/place/restoreikafteraim` | block | []x49 | 49 | 49 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/place/rotate` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |
| `/actor/weaponry/place/type` | block | [str]x39 | 19 | 39 | "coaxial" ¦ "hull" ¦ "rear" ¦ "main" ¦ "mgun_turret_left" ¦ "mgun_turret_right" | 추정(파일 주석) | "unit tag" or "unit type" + "current main weapon tag" | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/place/weapon` | block | [str atom str num]x83 [str atom]x2 | 49 | 85 | 예: "40mm_qf2" filling "bulleteng_40 ap2" 1 | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/actor/weaponry/restoreiktime` | block | [num]x47 | 47 | 47 | 4 ~ 10 | 미확인(자동) |  |  |
| `/entity` | block | [atom]x1 | 1 | 1 | actor | 추정(파일 주석) | place holder | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/##` | block | [atom str]x1 | 1 | 1 | extender "detail" | 추정(파일 주석) | No longer used? | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/##/random` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/able` | block | [str]x2 [str str str str]x1 | 1 | 3 | "emit_passangers_on_enemy" "repaired" "minimap" "ally_crew" ¦ "talk" ¦ "emit_on_attack" | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/armor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/armor/setting` | block | [str]x49 | 1 | 49 | 예: "body" | 미확인(자동) |  |  |
| `/entity/armor/setting/threshold` | block | []x49 | 1 | 49 | 예: | 미확인(자동) |  |  |
| `/entity/armor/setting/threshold/blast` | block | []x49 | 1 | 49 | 예: | 추정(파일 주석) | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/armor/setting/threshold/blast/koef` | block | [num]x49 | 1 | 49 | 0.5 ~ 10 | 미확인(자동) |  |  |
| `/entity/armor/setting/threshold/pierce` | block | []x49 | 1 | 49 | 예: | 추정(파일 주석) | Minimum penetration necessary to leave a ricochet/pen mark on the armour | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/armor/setting/threshold/pierce/koef` | block | [num]x49 | 1 | 49 | 0.8 ~ 1 | 미확인(자동) |  |  |
| `/entity/armor/setting/volumes` | block | [str]x50 | 1 | 50 | 예: "body*" | 추정(파일 주석) | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder` | block | []x6 | 1 | 6 | 예: | 미확인(자동) |  |  |
| `/entity/boarder/anm` | block | [str]x4 | 1 | 4 | "driver" ¦ "passenger" ¦ "commander" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder/anm/forward` | block | []x4 | 1 | 4 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder/anm/forward/base` | block | [str]x4 | 1 | 4 | "doordriver" ¦ "doorpassenger" ¦ "doorcommander" ¦ "%base" | 추정(파일 주석) | seconds : CP | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder/anm/forward/begin` | block | [str]x4 | 1 | 4 | "board_bigcar" ¦ "board_body" | 미확인(자동) |  |  |
| `/entity/boarder/anm/forward/end` | block | [str]x4 | 1 | 4 | "seat_passenger" ¦ "seat_driver_car" ¦ "seat_universal_driver" | 미확인(자동) |  |  |
| `/entity/boarder/anm/reverse` | block | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |
| `/entity/boarder/anm/reverse/base` | block | [str]x4 | 1 | 4 | "doordriver" ¦ "doorpassenger" ¦ "doorcommander" ¦ "%base" | 추정(파일 주석) | seconds : CP | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder/anm/reverse/end` | block | [str num]x4 | 1 | 4 | "board_bigcar" -1 ¦ "board_body" -1 | 미확인(자동) |  |  |
| `/entity/boarder/door` | block | [str]x1 | 1 | 1 | "emit1" | 미확인(자동) |  |  |
| `/entity/boarder/link` | block | [str str]x1 | 1 | 1 | "emit1" "driver" | 추정(파일 주석) | { link <from> <to> } | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder/link/anm` | block | [str]x1 | 1 | 1 | "driver" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder/link/forward` | block | [atom]x1 | 1 | 1 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/boarder/link/reverse` | block | [atom]x1 | 1 | 1 | puton | 미확인(자동) |  |  |
| `/entity/brain` | block | [str]x1 | 1 | 1 | "vehicle" | 미확인(자동) |  |  |
| `/entity/brain/const` | block | [str num]x2 | 1 | 2 | "crush/min_danger_zone_time" 1 ¦ "crush/min_danger_zone_length" 1 | 추정(파일 주석) | detect explosives inside 7 meter radius ¦ hide if danger is more than "x" meters away, otherwise run away | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/brain/state` | block | [str]x1 | 1 | 1 | "vehicle" | 추정(파일 주석) | 2hand & 1hand items ¦ to enable item ops in snipe cover | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis` | block | [str]x1 []x1 | 1 | 2 | "wheel" ¦ | 추정(파일 주석) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/fuelbag` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/entity/chassis/fuelbag/fuel` | block | [str]x2 | 1 | 2 | "fuel" | 추정(파일 주석) | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/fuelbag/rate` | block | [num]x2 | 1 | 2 | 5 ~ 40 | 추정(파일 주석) | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/fuelbag/remain` | block | [num]x2 | 1 | 2 | 50 ~ 200 | 추정(파일 주석) | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/fuelbag/volume` | block | [num]x2 | 1 | 2 | 50 ~ 200 | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/chassis/locomotion/braketime` | block | [num]x1 | 1 | 1 | 1.084 | 추정(파일 주석) | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/fallslope` | block | [num]x1 | 1 | 1 | 75 | 추정(파일 주석) | Maximum slope before physics kick in | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/fast` | block | [num]x1 | 1 | 1 | 9 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/gears` | block | [num num]x1 | 1 | 1 | 0.15 1 | 추정(파일 주석) | Gearing affects acceleration curve. ¦ Forward gear ratios | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/gearswitchdelay` | block | [num]x1 | 1 | 1 | 0.5 | 추정(파일 주석) | Time to switch between forward and reverse. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/maxslope` | block | [num]x1 | 1 | 1 | 75 | 추정(파일 주석) | Maximum slope to drive up hill | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/maxspeed` | block | [num]x1 | 1 | 1 | 10 | 추정(파일 주석) | Max speed. ¦ max speed while in flight mode in kph | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/maxspeedatmaxturn` | block | [num]x1 | 1 | 1 | 8 | 추정(파일 주석) | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/normal` | block | [num]x1 | 1 | 1 | 25 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/reargears` | block | [num]x1 | 1 | 1 | 0.8 | 추정(파일 주석) | Reverse max speed. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/slow` | block | [num]x1 | 1 | 1 | 10 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/splineradius` | block | [num]x1 | 1 | 1 | 0.75 | 추정(파일 주석) | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/starttime` | block | [num]x1 | 1 | 1 | 6.681815 | 추정(파일 주석) | Time for vehicle to accelerate from 0kph to max speed. ¦ acceleration time from 0 to max. speed, seconds | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/steeroff` | block | [num]x1 | 1 | 1 | 30 | 추정(파일 주석) | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/steeron` | block | [num]x1 | 1 | 1 | 20 | 추정(파일 주석) | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/stoptime` | block | [num]x1 | 1 | 1 | 3.42 | 추정(파일 주석) | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/locomotion/turnradius` | block | [num]x1 | 1 | 1 | 5 | 추정(파일 주석) | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/speed` | block | []x1 | 1 | 1 | 예: | 추정(파일 주석) | velocity of bullet in meters per second ¦ m/s | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/speed/fast` | block | [num]x1 | 1 | 1 | 30 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/speed/normal` | block | [num]x1 | 1 | 1 | 17 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/speed/slow` | block | [num]x1 | 1 | 1 | 10 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/track` | block | [str]x2 | 1 | 2 | "wlf" ¦ "wrf" | 미확인(자동) |  |  |
| `/entity/chassis/track/tracer` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/entity/chassis/track/tracer/bone` | block | [str]x2 | 1 | 2 | "FXtraceL2" ¦ "FXtraceR2" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/track/tracer/step` | block | [num]x2 | 1 | 2 | 0.3 | 추정(파일 주석) | in meters ¦ 0.7 meters | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/chassis/track/tracer/view` | block | [str]x2 | 1 | 2 | "tracks_sm" | 추정(파일 주석) | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/collider` | block | [str]x2 | 1 | 2 | "vehicle" ¦ "car" | 미확인(자동) |  |  |
| `/entity/component` | block | [str]x8 | 1 | 8 | "body" ¦ "engine" ¦ "wheelright1" ¦ "wheelleft1" ¦ "wheelright2" ¦ "wheelleft2" ¦ "fuel" ¦ "transmission" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/component/repairorder` | block | [num]x8 | 1 | 8 | 0 ~ 10 | 미확인(자동) |  |  |
| `/entity/component/repairtime` | block | [num]x8 | 1 | 8 | 1 ~ 45 | 추정(파일 주석) | 45 ¦ 30 | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/component/tags` | block | [str str]x4 [str]x2 | 1 | 6 | "wheel" "no_target" ¦ "fuel" ¦ "transmission" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/decalview` | block | [str]x3 | 1 | 3 | "selection_active" ¦ "selection_selected" ¦ "selection_selectable" | 미확인(자동) |  |  |
| `/entity/decalview/dynamic` | block | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |
| `/entity/decalview/ground` | block | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |
| `/entity/decalview/layer` | block | [atom]x3 | 1 | 3 | alpha | 미확인(자동) |  |  |
| `/entity/decalview/offset` | block | [num num num]x3 | 1 | 3 | 0 0 10 | 미확인(자동) |  |  |
| `/entity/decalview/sprite` | block | [str]x3 | 1 | 3 | "vehicle_selected.ebm" ¦ "vehicle_selectable.ebm" | 미확인(자동) |  |  |
| `/entity/decalview/sprite/light` | block | [num]x3 | 1 | 3 | 1 | 미확인(자동) |  |  |
| `/entity/decalview/sprite/prescalex` | block | [num]x3 | 1 | 3 | 0.24 | 미확인(자동) |  |  |
| `/entity/decalview/sprite/prescaley` | block | [num]x3 | 1 | 3 | 0.2 | 미확인(자동) |  |  |
| `/entity/decalview/usedirection` | block | []x3 | 1 | 3 | 예: | 미확인(자동) |  |  |
| `/entity/decalview/visible` | block | [num]x3 | 1 | 3 | 0 | 추정(파일 주석) | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/extender[inventory]` | block | [str]x2 | 1 | 2 | "inventory" | 미확인(자동) |  |  |
| `/entity/extender[inventory]/box` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/entity/extender[inventory]/box/item` | block | [str num]x2 | 1 | 2 | "can" 1 ¦ "repair_kit" 5 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/extender[inventory]/box/size` | block | [num num]x2 | 1 | 2 | 9 15 ¦ 9 10 | 추정(파일 주석) | size of the window | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/extender[inventory]/box/weight` | block | [num]x1 | 1 | 1 | 2000 | 추정(파일 주석) | will stack up in 100/2 = 2 on humans | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/extender[inventory]/weapon` | block | [str]x1 | 1 | 1 | "mgun" | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/extender[inventory]/weapon/mask` | block | [str]x1 | 1 | 1 | "mountable" | 미확인(자동) |  |  |
| `/entity/extender[talk]` | block | [str]x1 | 1 | 1 | "talk" | 미확인(자동) |  |  |
| `/entity/extender[talk]/talker` | block | [str]x1 | 1 | 1 | "cannon" | 미확인(자동) |  |  |
| `/entity/extension` | block | [str]x1 | 1 | 1 | "universal.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/hearingbias` | block | [num]x1 | 1 | 1 | 4.5 | 추정(파일 주석) | reduces units ability to hear sound signals. value = meters ¦ in meters | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/lodclass` | block | [str]x1 | 1 | 1 | "vehicle" | 미확인(자동) |  |  |
| `/entity/mass` | block | [num]x1 | 1 | 1 | 2100 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/nogroupmesh` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/entity/obstacleid` | block | [str]x1 | 1 | 1 | "tank" | 미확인(자동) |  |  |
| `/entity/patherid` | block | [str]x1 | 1 | 1 | "vehicle.large" | 미확인(자동) |  |  |
| `/entity/perks` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/entity/perks/blastwave_modifier` | block | [num]x1 | 1 | 1 | 10 | 미확인(자동) |  |  |
| `/entity/perks/component` | block | [str]x12 [atom str]x9 | 1 | 21 | 예: "body" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/perks/component/armor_damage_modifier` | block | [num]x19 | 1 | 19 | 0.1 ~ 5 | 추정(파일 주석) | multiplier adjusts ratio of which part of penetration does damage to component. (shell penetration - armor thickness) verses minimumDamageModifier. values < 1 = less damage done by over penetration. | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/perks/component/health` | block | []x21 | 1 | 21 | 예: | 추정(파일 주석) | A better way to translate this is that it will deal 2x damage to the enemy. In the editor it is hard to understand, as if you're switching between players it will always display the hitpoints of "player". But the enemy units on that particular instance will have half the total health. ¦ health multiplier from soldier settings | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/perks/component/health/hp` | block | [num]x21 | 1 | 21 | 50 ~ 400 | 미확인(자동) |  |  |
| `/entity/perks/component/health/hp_broken` | block | [num]x9 [expr]x1 | 1 | 10 | 1 ~ 150 | 추정(파일 주석) | destroys component and cannot be repaired | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/placer` | block | []x3 | 1 | 3 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/placer/crew` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/placer/place` | block | [str]x2 | 1 | 2 | "driver" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/placer/place/group` | block | [str]x1 | 1 | 1 | "crew" | 추정(파일 주석) | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/placer/place/linkbone` | block | [str]x1 | 1 | 1 | "driver" | 미확인(자동) |  |  |
| `/entity/placer/place/turnoff` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/placer/place/turnoff/targetable` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/entity/placer/place/usevision` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |
| `/entity/placer/place/visor` | block | [str]x1 | 1 | 1 | "main" | 미확인(자동) |  |  |
| `/entity/props` | block | [str str str str]x2 [str str str]x1 | 1 | 3 | "steel" "vehicle" "obstacle_ignore_height" "emit_on_sink" ¦ "car" "engine" "wheeled" "turn_over" ¦ "truck" "tractor" "tractor_heavy" | 미확인(자동) |  |  |
| `/entity/rimlight` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/sensor` | block | []x2 | 1 | 2 | 예: | 미확인(자동) |  |  |
| `/entity/sensor/informrange` | block | [num num]x1 | 1 | 1 | 50 75 | 추정(파일 주석) | first = AI , second = USER | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/sensor/recognizetime` | block | [num]x1 | 1 | 1 | 0.01 | 미확인(자동) |  |  |
| `/entity/sensor/visor` | block | [str]x4 | 1 | 4 | "hearing" ¦ "main" ¦ "around" ¦ "none" | 미확인(자동) |  |  |
| `/entity/sensor/visor/bone` | block | [str]x3 | 1 | 3 | "hearing_bone" ¦ "visor2" ¦ "visor1" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/sensor/visor/vision` | block | [str]x4 | 1 | 4 | "noise_detection" ¦ "vehicle_main" ¦ "human_peripheral" ¦ "none" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/simulation` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/simulation/able` | block | [num]x1 | 1 | 1 | 1 | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/targetclass` | block | [str]x1 | 1 | 1 | "car" | 미확인(자동) |  |  |
| `/entity/targetselector` | block | [str]x1 | 1 | 1 | "car_bullet" | 미확인(자동) |  |  |
| `/entity/volume` | block | [str]x216 | 1 | 216 | 예: "select" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/volume/able` | block | [atom]x28 []x16 | 1 | 44 | clear ¦ | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/volume/able/contact` | block | [num]x4 | 1 | 4 | 0 ~ 1 | 미확인(자동) |  |  |
| `/entity/volume/able/hole` | block | []x26 | 1 | 26 | 예: | 미확인(자동) |  |  |
| `/entity/volume/able/nowounddecal` | block | []x14 | 1 | 14 | 예: | 미확인(자동) |  |  |
| `/entity/volume/able/select` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/volume/able/touch` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/volume/component` | block | [str]x40 | 1 | 40 | "body" ¦ "engine" ¦ "wheelright1" ¦ "wheelleft1" ¦ "wheelright2" ¦ "wheelleft2" ¦ "fuel" ¦ "transmission" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/volume/density` | block | [num]x28 | 1 | 28 | 0 | 미확인(자동) |  |  |
| `/entity/volume/material` | block | [str]x13 | 1 | 13 | "wood" ¦ "sand" | 미확인(자동) |  |  |
| `/entity/volume/tags` | block | [str]x88 [str str]x6 | 1 | 94 | "armor" ¦ "body" ¦ "wood" ¦ "engine" ¦ "wheel" "no_target" ¦ "fuel" "no_target" ¦ "transmission" "no_target" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/volume/thickness` | block | [num]x96 | 1 | 96 | 1.5 ~ 10 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/entity/weaponry` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/entity/weaponry/manualaccuracy` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/entity/weaponry/manualaccuracyzoom` | block | [num]x1 | 1 | 1 | 1.25 | 미확인(자동) |  |  |
| `/game_entity` | block | []x1621 [atom]x763 | 2384 | 2384 | ¦ actor | 추정(파일 주석) | june '41 ¦ sept 44 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/(mod)` | mod | [str]x71 [atom str]x20 | 88 | 91 | "mp" ¦ not "mp" | 미확인(자동) |  |  |
| `/game_entity/##` | block | [atom str]x763 | 763 | 763 | extender "detail" | 추정(파일 주석) | No longer used? | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##/random` | block | []x763 | 763 | 763 | 예: | 미확인(자동) |  |  |
| `/game_entity/##bone` | block | [str]x43 | 43 | 43 | "commander_rot" | 미확인(자동) |  |  |
| `/game_entity/##bone/speed2` | block | [num]x43 | 43 | 43 | 180 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##extender` | block | [str]x1 | 1 | 1 | "smokescreen" | 추정(파일 주석) | sources say that these were not installed in the vehicles delivered to the Finns. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##extender/cooldown` | block | [num]x1 | 1 | 1 | 5 | 추정(파일 주석) | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##extender/itemtags` | block | [str]x1 | 1 | 1 | "grenade nbks" | 미확인(자동) |  |  |
| `/game_entity/##sensor` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |
| `/game_entity/##sensor/visor` | block | [str]x2 | 1 | 2 | "main" ¦ "around" | 미확인(자동) |  |  |
| `/game_entity/##sensor/visor/bone` | block | [str]x1 | 1 | 1 | "" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##sensor/visor/vision` | block | [str]x2 | 1 | 2 | "" ¦ "vehicle_around" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##volume` | block | [str]x6 | 6 | 6 | "engine3" | 추정(파일 주석) | Engine Cover | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##volume/thickness` | block | [num]x6 | 6 | 6 | 10 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##weaponry` | block | []x47 | 47 | 47 | 예: | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place` | block | [str]x47 | 47 | 47 | "mgun" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##weaponry/place/accuracybymotion` | block | []x47 | 47 | 47 | 예: | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place/accuracybymotion/byangularspeed` | block | [num]x47 | 47 | 47 | 0.5 | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place/accuracybymotion/bylinearspeed` | block | [num]x47 | 47 | 47 | 0.4 | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place/accuracybymotion/disordertime` | block | [num]x47 | 47 | 47 | 0.6 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##weaponry/place/accuracybymotion/maxangularspeed` | block | [num]x47 | 47 | 47 | 30 | 추정(파일 주석) | deg/s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##weaponry/place/accuracybymotion/maxlinearspeed` | block | [num]x47 | 47 | 47 | 8 | 추정(파일 주석) | km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##weaponry/place/accuracybymotion/ordertime` | block | [num]x47 | 47 | 47 | 2 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##weaponry/place/charger` | block | [str]x47 | 47 | 47 | "gunner" | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/##weaponry/place/gunner` | block | [str]x47 | 47 | 47 | "gunner" | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place/linkanimation` | block | [str]x47 | 47 | 47 | "off" | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place/linkbone` | block | [str]x47 | 47 | 47 | "mgun" | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place/rotate` | block | []x47 | 47 | 47 | 예: | 미확인(자동) |  |  |
| `/game_entity/##weaponry/place/type` | block | [str]x47 | 47 | 47 | "main" | 추정(파일 주석) | "unit tag" or "unit type" + "current main weapon tag" | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/able` | block | [str str str str]x763 [str]x679 [str str]x206 | 763 | 1648 | "emit_passangers_on_enemy" "repaired" "minimap" "ally_crew" ¦ "talk" ¦ "talk" "-emit_passangers_on_enemy" ¦ "emit_on_attack" ¦ "emit_passangers_on_enemy" ¦ "-emit_passangers_on_enemy" ¦ "emit_passengers_on_enemy" ¦ "emit_on_attack" "emit_crew_on_injure" | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/armor` | block | []x2007 | 1571 | 2007 | 예: | 미확인(자동) |  |  |
| `/game_entity/armor/setting` | block | [str]x98343 | 1571 | 98343 | 예: "body" | 미확인(자동) |  |  |
| `/game_entity/armor/setting/threshold` | block | []x98343 | 1571 | 98343 | 예: | 미확인(자동) |  |  |
| `/game_entity/armor/setting/threshold/blast` | block | []x98343 | 1571 | 98343 | 예: | 추정(파일 주석) | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/armor/setting/threshold/blast/koef` | block | [num]x98343 | 1571 | 98343 | 0.5 ~ 99 | 미확인(자동) |  |  |
| `/game_entity/armor/setting/threshold/pierce` | block | []x98343 | 1571 | 98343 | 예: | 추정(파일 주석) | Minimum penetration necessary to leave a ricochet/pen mark on the armour | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/armor/setting/threshold/pierce/koef` | block | [num]x98343 | 1571 | 98343 | 0.8 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/armor/setting/volumes` | block | [str]x100350 | 1571 | 100350 | 예: "body*" | 추정(파일 주석) | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/autooperatable` | block | [num]x13 | 13 | 13 | 0 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/boarder` | block | []x8192 | 763 | 8192 | 예: | 미확인(자동) |  |  |
| `/game_entity/boarder/anm` | block | [str]x5676 | 682 | 5676 | 예: "driver" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/anm/forward` | block | []x5676 | 682 | 5676 | 예: | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/anm/forward/base` | block | [str]x1045 [str num]x1 | 385 | 1046 | 예: "open" | 추정(파일 주석) | seconds : CP | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/anm/forward/begin` | block | [str]x3332 | 478 | 3332 | 예: "board_plane_left" | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/forward/end` | block | [str]x2753 [str num]x2151 | 680 | 4904 | 예: "seat_driver_car" | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/reverse` | block | []x3433 | 479 | 3433 | 예: | 미확인(자동) |  |  |
| `/game_entity/boarder/anm/reverse/base` | block | [str]x739 [str num]x305 | 376 | 1044 | 예: "open" -1 | 추정(파일 주석) | seconds : CP | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/anm/reverse/end` | block | [str]x2730 [str num]x702 | 478 | 3432 | 예: "board_plane_left" -1 | 미확인(자동) |  |  |
| `/game_entity/boarder/door` | block | [str]x7794 | 682 | 7794 | 예: "emit1" | 미확인(자동) |  |  |
| `/game_entity/boarder/link` | block | [str str]x9325 | 682 | 9325 | 예: "emit1" "driver" | 추정(파일 주석) | { link <from> <to> } | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/link/anm` | block | [str]x7794 | 682 | 7794 | 예: "driver" | 추정(파일 주석) | gunner ¦ commander | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/link/forward` | block | [atom]x7794 | 682 | 7794 | putoff | 추정(파일 주석) | placeholder pose ¦ 3 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/boarder/link/reverse` | block | [atom]x7794 | 682 | 7794 | puton | 미확인(자동) |  |  |
| `/game_entity/bone` | block | [str]x1823 | 653 | 1823 | 예: "mgun_rot1" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/bone/limits` | block | [num num]x1086 | 609 | 1086 | 예: -1 1 | 추정(파일 주석) | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/bone/speed2` | block | [num]x1816 | 650 | 1816 | 1.5 ~ 360 | 추정(파일 주석) | manual handcrack, extremely slow ¦ manual using gunner's shoulder | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/brain` | block | [str]x763 []x56 | 763 | 819 | "vehicle" ¦ | 미확인(자동) |  |  |
| `/game_entity/brain/const` | block | [str num]x1486 | 743 | 1486 | "crush/min_danger_zone_time" 1 ¦ "crush/min_danger_zone_length" 1 | 추정(파일 주석) | detect explosives inside 7 meter radius ¦ hide if danger is more than "x" meters away, otherwise run away | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/brain/properties` | block | []x56 | 56 | 56 | 예: | 미확인(자동) |  |  |
| `/game_entity/brain/properties/fire_mode` | block | [atom]x56 | 56 | 56 | hold | 미확인(자동) |  |  |
| `/game_entity/brain/state` | block | [str]x763 | 763 | 763 | "vehicle" ¦ "airborne" | 추정(파일 주석) | 2hand & 1hand items ¦ to enable item ops in snipe cover | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis` | block | [str]x875 []x436 | 758 | 1311 | "track" ¦  ¦ "wheel" ¦ "human_carrier" ¦ "airborne" ¦ "water" ¦ "railway" | 추정(파일 주석) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/(mod)` | mod | [str]x40 [atom str]x12 | 32 | 52 | "mp" ¦ not "mp" | 미확인(자동) |  |  |
| `/game_entity/chassis/disablebackward` | block | []x40 | 20 | 40 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/disablemovewhilefiring` | block | []x8 | 8 | 8 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/dontuseslope` | block | []x40 | 20 | 40 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/elevationacceleration` | block | [num]x40 | 20 | 40 | 3.25 ~ 8 | 추정(파일 주석) | how quickly aircraft can change from level flight to full climb or dive. (m/s^2) | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/fuelbag` | block | []x1024 | 546 | 1024 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/fuelbag/fuel` | block | [str]x1024 | 546 | 1024 | "fuel" ¦ "diesel" | 추정(파일 주석) | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/fuelbag/rate` | block | [num]x1024 | 546 | 1024 | 0 ~ 200 | 추정(파일 주석) | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/fuelbag/remain` | block | [num]x1024 | 546 | 1024 | 0 ~ 6630 | 추정(파일 주석) | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/fuelbag/volume` | block | [num]x1024 | 546 | 1024 | 0 ~ 6630 | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/ignorepatherbounds` | block | []x24 | 24 | 24 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/ignorepatherboundsai` | block | []x40 | 20 | 40 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion` | block | []x835 | 749 | 835 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/(mod)` | mod | [str]x20 | 20 | 20 | "mp" | 미확인(자동) |  |  |
| `/game_entity/chassis/locomotion/braketime` | block | [num]x821 | 748 | 821 | 0.25121 ~ 180.9 | 추정(파일 주석) | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/fallslope` | block | [num]x760 | 714 | 760 | 70 ~ 75 | 추정(파일 주석) | Maximum slope before physics kick in | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/fast` | block | [num]x811 | 745 | 811 | 0 ~ 132.062293 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/gears` | block | [num num num num num]x338 [num num num num]x246 [num num]x178 [num num num]x7 | 723 | 769 | 0.5 0.65 0.8 0.9 1.0 ¦ 0.35 0.5 0.7 1.0 ¦ 0.15 1 ¦ 0.5 0.7 1.0 | 추정(파일 주석) | Gearing affects acceleration curve. ¦ Forward gear ratios | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/gearswitchdelay` | block | [num]x520 | 520 | 520 | 0.5 | 추정(파일 주석) | Time to switch between forward and reverse. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/maxslope` | block | [num]x760 | 714 | 760 | 60 ~ 75 | 추정(파일 주석) | Maximum slope to drive up hill | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/maxspeed` | block | [num]x863 | 749 | 863 | 0 ~ 132.062293 | 추정(파일 주석) | Max speed. ¦ max speed while in flight mode in kph | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/maxspeedatmaxturn` | block | [num]x539 | 539 | 539 | 0.65484 ~ 84.41672 | 추정(파일 주석) | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/normal` | block | [num]x811 | 745 | 811 | 0 ~ 132.062293 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/reargears` | block | [num num]x588 [num]x182 | 724 | 770 | 0.042857 ~ 1.0625 | 추정(파일 주석) | Reverse max speed. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/slow` | block | [num]x811 | 745 | 811 | 0 ~ 84.039641 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/splineradius` | block | [num]x225 | 205 | 225 | 0.75 ~ 10 | 추정(파일 주석) | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/starttime` | block | [num]x821 | 748 | 821 | 1.253025 ~ 79.14705 | 추정(파일 주석) | Time for vehicle to accelerate from 0kph to max speed. ¦ acceleration time from 0 to max. speed, seconds | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/steeroff` | block | [num]x205 | 205 | 205 | 15 ~ 40 | 추정(파일 주석) | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/steeron` | block | [num]x205 | 205 | 205 | 7.5 ~ 20 | 추정(파일 주석) | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/stoptime` | block | [num]x814 | 748 | 814 | 0.353025 ~ 362.3 | 추정(파일 주석) | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/turnradius` | block | [num]x853 | 743 | 853 | 0.5 ~ 75 | 추정(파일 주석) | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/turnstart` | block | [num]x584 | 538 | 584 | 0.352467 ~ 3.995455 | 추정(파일 주석) | Direct Control, Time for vehicle to from 0Â°/s rotation speed to max rotation speed. ¦ / | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/turnstop` | block | [num]x584 | 538 | 584 | 0.302467 ~ 3.995455 | 추정(파일 주석) | Direct Control, Time for vehicle to from max rotation speed to 0Â°/s rotation speed to max rotation speed. ¦ / | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotion/turntime` | block | [num]x587 | 541 | 587 | 3.914154 ~ 57.767273 | 추정(파일 주석) | Direct Control, Time it takes to do a full 360 degree at max turn speed, fetching custom define | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/locomotive` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |
| `/game_entity/chassis/maxairbornealtitude` | block | [num]x20 | 20 | 20 | 100 | 미확인(자동) |  |  |
| `/game_entity/chassis/maxelevationspeed` | block | [num]x40 | 20 | 40 | 2.833333 ~ 8 | 추정(파일 주석) | climb rate, how many meters per second aircraft changes altitude when in full climb or dive | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/maxsideslope` | block | [num]x40 | 20 | 40 | 15 ~ 65 | 추정(파일 주석) | limit in degrees that aircraft can roll while in a turn | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/sideslopespeed` | block | [num]x60 | 20 | 60 | 20 ~ 120 | 추정(파일 주석) | how quickly in degrees per second that aircraft rolls | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/speed` | block | []x145 | 123 | 145 | 예: | 추정(파일 주석) | velocity of bullet in meters per second ¦ m/s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/speed/fast` | block | [num]x145 | 123 | 145 | 20 ~ 100 | 추정(파일 주석) | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/speed/normal` | block | [num]x145 | 123 | 145 | 15 ~ 80 | 추정(파일 주석) | Standard speed. Single click movement order ¦ single flick, running mode | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/speed/slow` | block | [num]x145 | 123 | 145 | 10 ~ 60 | 추정(파일 주석) | Slow speed. Mainly used in mission scripting ¦ km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/switch` | block | []x22 | 11 | 22 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/switch/exitfromwater` | block | []x11 | 11 | 11 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/switch/terrain` | block | [atom str str]x11 [str str]x11 | 11 | 22 | not "water" "deep_water" ¦ "water" "deep_water" | 미확인(자동) |  |  |
| `/game_entity/chassis/track` | block | [str]x1788 | 690 | 1788 | "left" ¦ "right" ¦ "wlf" ¦ "wrf" ¦ "wlr" ¦ "wrr" | 미확인(자동) |  |  |
| `/game_entity/chassis/track/texturescroller` | block | []x784 | 392 | 784 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/track/texturescroller/bone` | block | [str]x784 | 392 | 784 | "TrackL" ¦ "TrackR" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/track/texturescroller/length` | block | [num]x784 | 392 | 784 | -1.39 ~ 1.48 | 추정(파일 주석) | 0.6 meters per animation ¦ meters per animation | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/track/tracer` | block | []x1788 | 690 | 1788 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassis/track/tracer/bone` | block | [str]x1788 | 690 | 1788 | "FXtraceL1" ¦ "FXtraceR1" ¦ "FXtraceL2" ¦ "FXtraceR2" ¦ "FXtraceL3" ¦ "FXtraceR3" ¦ "wheell4" ¦ "wheelr4" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/track/tracer/step` | block | [num]x1788 | 690 | 1788 | 0.07 ~ 0.62 | 추정(파일 주석) | in meters ¦ 0.7 meters | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/track/tracer/view` | block | [str]x1788 | 690 | 1788 | 예: "tracewheel05" | 추정(파일 주석) | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/chassis/waterline` | block | [num]x18 | 18 | 18 | 0 ~ 1.6 | 미확인(자동) |  |  |
| `/game_entity/chassismanager` | block | []x20 | 20 | 20 | 예: | 미확인(자동) |  |  |
| `/game_entity/chassismanager/current` | block | [str]x20 | 20 | 20 | "wheel" | 추정(파일 주석) | current stamina ¦ current health | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/collider` | block | [str]x2605 | 1572 | 2605 | 예: "vehicle" | 미확인(자동) |  |  |
| `/game_entity/component` | block | [str]x12435 | 762 | 12435 | 예: "wingright" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/component/repairorder` | block | [num]x11601 | 761 | 11601 | 0 ~ 13 | 미확인(자동) |  |  |
| `/game_entity/component/repairtime` | block | [num]x11112 | 761 | 11112 | 1 ~ 130 | 추정(파일 주석) | 45 ¦ 30 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/component/tags` | block | [str]x7002 [str str]x2441 [str str str]x130 | 724 | 9573 | 예: "wingright" "wing" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/compoundobstacles` | block | []x21 | 21 | 21 | 예: | 미확인(자동) |  |  |
| `/game_entity/constrain` | block | [atom]x48 | 47 | 48 | ground ¦ none | 미확인(자동) |  |  |
| `/game_entity/decalview` | block | [str]x2277 [atom]x31 | 754 | 2308 | "selection_active" ¦ "selection_selected" ¦ "selection_selectable" ¦ supply_zone | 미확인(자동) |  |  |
| `/game_entity/decalview/dynamic` | block | []x2277 | 754 | 2277 | 예: | 미확인(자동) |  |  |
| `/game_entity/decalview/ground` | block | []x2277 | 754 | 2277 | 예: | 미확인(자동) |  |  |
| `/game_entity/decalview/layer` | block | [atom]x2308 | 754 | 2308 | alpha ¦ grounddynamic | 미확인(자동) |  |  |
| `/game_entity/decalview/offset` | block | [num num num]x2277 | 754 | 2277 | 0 0 10 | 미확인(자동) |  |  |
| `/game_entity/decalview/sprite` | block | [str]x2308 | 754 | 2308 | 예: "vehicle_selected.ebm" | 미확인(자동) |  |  |
| `/game_entity/decalview/sprite/light` | block | [num]x2277 | 754 | 2277 | 1 | 미확인(자동) |  |  |
| `/game_entity/decalview/sprite/prescalex` | block | [num]x2277 | 754 | 2277 | 0.18 ~ 0.9 | 미확인(자동) |  |  |
| `/game_entity/decalview/sprite/prescaley` | block | [num]x2277 | 754 | 2277 | 0.175 ~ 0.9 | 미확인(자동) |  |  |
| `/game_entity/decalview/sprite/visibility` | block | [num]x31 | 31 | 31 | 0.25 | 미확인(자동) |  |  |
| `/game_entity/decalview/usedirection` | block | []x2277 | 754 | 2277 | 예: | 미확인(자동) |  |  |
| `/game_entity/decalview/visible` | block | [num]x2308 | 754 | 2308 | 0 | 추정(파일 주석) | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]` | block | [str]x253 | 206 | 253 | "cannon" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/##` | block | [atom]x1 | 1 | 1 | animation | 추정(파일 주석) | No longer used? | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/##/aim` | block | [str str]x1 | 1 | 1 | "" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/##/fire` | block | [str str]x1 | 1 | 1 | "" "" | 추정(파일 주석) | mg_maxim_gunner_fire | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/##/move` | block | [str str]x1 | 1 | 1 | "" "" | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/##/reload` | block | [str str]x1 | 1 | 1 | "" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/##/switch` | block | [str]x1 | 1 | 1 | "close" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation` | block | []x252 | 205 | 252 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/aim` | block | [str str]x96 [str str str str]x93 [str str str str str]x40 [str str str]x9 [str str str str str str]x6 | 204 | 244 | 예: "" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/fire` | block | [str str str str]x93 [str str]x88 [str str str str str]x39 [str str str]x9 [str str str str str str]x7 | 197 | 237 | 예: "" "" | 추정(파일 주석) | mg_maxim_gunner_fire | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/animation/gunners` | block | [str str str str]x7 | 7 | 7 | "" "" "" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/move` | block | [str str]x105 [str str str str]x94 [str str str str str]x33 [str str str]x9 [str str str str str str]x7 | 205 | 252 | 예: "mgun_browning_driver_r" "mgun_browning_driver_l" | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/animation/reload` | block | [str str]x96 [str str str str]x93 [str str str str str]x39 [str str str]x9 [str str str str str str]x7 | 203 | 248 | 예: "mgun_stan_charger" "" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/animation/switch` | block | [str]x228 | 205 | 228 | "close" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/attack` | block | []x234 | 206 | 234 | 예: | 추정(파일 주석) | fire/attack state: cannon aim, fire, and reload events | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/attack/obstacle` | block | [str]x233 | 205 | 233 | "open" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/attack/workers` | block | [str str str str]x94 [str str str str str]x33 [str str str]x9 [str str str str str str]x7 [str str]x4 | 152 | 152 | "gunner" "charger" "charger2" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "charger3" "commander" ¦ "gunner" "charger" "commander" ¦ "gunner" "commander" ¦ "gunner" ¦ "" "" "" "" "" ¦ "" "" "" ¦ clear | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/move` | block | []x241 | 206 | 241 | 예: | 추정(파일 주석) | move state: cannon being pushed and which positions play animations | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/move/obstacle` | block | [str]x240 | 205 | 240 | "close" | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/move/workers` | block | [str str str str atom num]x94 [str str atom num]x64 [str str str str str atom num]x31 [atom]x8 [str str str str str str atom num]x7 | 197 | 214 | "driver1" "driver2" "driver3" "driver4" minWorkers 2 ¦ "driver1" "driver2" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 2 ¦ "driver1" "driver2" minWorkers 1 ¦ clear ¦ "driver1" "driver2" "driver3" "driver4" "driver5" "driver6" minWorkers 2 ¦ "driver1" "driver2" "driver3" minWorkers 2 ¦ "driver" minWorkers 1 ¦ "driver1" "driver2" "driver3" "driver4" minWorkers 3 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 1 | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/rotate_ik_limit` | block | [num]x37 | 37 | 37 | 0 ~ 35 | 미확인(자동) |  |  |
| `/game_entity/extender[cannon]/trailing` | block | []x144 | 144 | 144 | 예: | 추정(파일 주석) | trailing state: cannon is being towed {obstacle} not needed. Uses "close" | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[cannon]/trailing/workers` | block | [str str str str]x92 [str str str str str]x30 [str str str str str str]x7 [str str str]x7 [atom]x4 | 144 | 144 | "seat1" "seat2" "seat3" "seat4" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" "seat6" ¦ "seat1" "seat2" "seat3" ¦ clear ¦ "seat1" "seat2" | 미확인(자동) |  |  |
| `/game_entity/extender[detail]` | block | [str]x3 | 3 | 3 | "detail" | 미확인(자동) |  |  |
| `/game_entity/extender[detail]/detail1` | block | [num]x3 | 3 | 3 | 0 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/extender[detail]/random` | block | [num]x2 | 2 | 2 | 0 | 미확인(자동) |  |  |
| `/game_entity/extender[edifice]` | block | [str]x1 | 1 | 1 | "edifice" | 미확인(자동) |  |  |
| `/game_entity/extender[enumerator]` | block | [str]x347 | 347 | 347 | "enumerator" | 미확인(자동) |  |  |
| `/game_entity/extender[enumerator]/digit_folder` | block | [str]x347 | 347 | 347 | "us2" ¦ "ru1" ¦ "ger" ¦ "np" ¦ "ger2" ¦ "moe" ¦ "np_w" ¦ "us3" ¦ "us1" ¦ "ru3" ¦ "c47" ¦ "ru2" | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]` | block | [str]x2282 | 763 | 2282 | "inventory" | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box` | block | []x2282 | 763 | 2282 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box/clear` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box/item` | block | [str num]x3781 [str atom str num]x92 [str atom]x86 [str str num]x18 [str atom str]x1 | 740 | 3978 | 예: "ammo mgun_usa belt" 1000 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[inventory]/box/item/user` | block | [str]x118 | 104 | 118 | "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | 미확인(자동) |  |  |
| `/game_entity/extender[inventory]/box/size` | block | [num num]x1528 | 763 | 1528 | 9 15 ¦ 9 12 ¦ 9 10 ¦ 9 6 ¦ 8 8 ¦ 4 2 ¦ 0 0 ¦ 6 4 ¦ 9 9 ¦ 6 2 ¦ 6 6 ¦ 9 11 | 추정(파일 주석) | size of the window | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[inventory]/box/weight` | block | [num]x767 | 754 | 767 | 100 ~ 100000 | 추정(파일 주석) | will stack up in 100/2 = 2 on humans | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[inventory]/weapon` | block | [str]x885 | 763 | 885 | "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[inventory]/weapon/mask` | block | [str]x885 | 763 | 885 | "mountable" | 미확인(자동) |  |  |
| `/game_entity/extender[land_patch]` | block | [str]x1 | 1 | 1 | "land_patch" | 미확인(자동) |  |  |
| `/game_entity/extender[land_patch]/terraintype` | block | [str str]x1 | 1 | 1 | "road" "bridge_steel" | 미확인(자동) |  |  |
| `/game_entity/extender[shield]` | block | [str]x230 | 228 | 230 | "shield" | 미확인(자동) |  |  |
| `/game_entity/extender[shield]/off` | block | []x13 | 13 | 13 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[shield]/on` | block | []x217 | 216 | 217 | 예: | 추정(파일 주석) | event created by animation ¦ overwrites and stops dust fx on ground from firing main gun | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[smokescreen]` | block | [str]x143 | 143 | 143 | "smokescreen" | 미확인(자동) |  |  |
| `/game_entity/extender[smokescreen]/cooldown` | block | [num]x143 | 143 | 143 | 5 ~ 20 | 추정(파일 주석) | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[smokescreen]/itemtags` | block | [str]x143 | 143 | 143 | "mortareng_50 sm" ¦ "grenade no8_smoke" ¦ "mortareng_50 wp" ¦ "grenade nbks" | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]` | block | [str]x31 | 31 | 31 | "supply_zone" | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/(mod)` | mod | [str]x16 | 16 | 16 | "gameclass:campaign_capture_the_flag" | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/fuel` | block | []x6 | 6 | 6 | 예: | 추정(파일 주석) | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[supply_zone]/fuel/amount` | block | [num]x6 | 6 | 6 | 1 | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/fuel/value` | block | [num]x6 | 6 | 6 | 1 | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/limit` | block | [num]x31 | 31 | 31 | 200 ~ 2840 | 추정(파일 주석) | max number of resupplies (0 = unlimited) ¦ Max CP | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[supply_zone]/mechanicresources` | block | []x9 | 9 | 9 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/mechanicresources/amount` | block | [num]x9 | 9 | 9 | 10 | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/mechanicresources/value` | block | [num]x9 | 9 | 9 | 10 | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/radius` | block | [num]x31 | 31 | 31 | 10 ~ 12 | 추정(파일 주석) | resupply radius in meter | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[supply_zone]/regenerationperiod` | block | [num]x47 | 31 | 47 | 0 ~ 10 | 추정(파일 주석) | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[supply_zone]/resupply` | block | []x31 | 31 | 31 | 예: | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/resupply/(mod)` | mod | [atom str]x32 [str]x16 | 16 | 48 | not "mp" ¦ "mp" | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/resupply/item` | block | [str str str num]x4608 [str str num]x1729 [str str str str num]x1360 [str num]x318 | 31 | 8015 | 예: "flame_thrower" "ammo" 50 | 추정(파일 주석) | 600 rounds ¦ 50 rounds | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[supply_zone]/resupply/item/value` | block | [num]x8015 | 31 | 8015 | 1 ~ 350 | 미확인(자동) |  |  |
| `/game_entity/extender[supply_zone]/resupplyperiod` | block | [num]x31 | 31 | 31 | 1 ~ 2 | 추정(파일 주석) | resupply period in seconds | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/extender[talk]` | block | [str]x727 | 718 | 727 | "talk" | 미확인(자동) |  |  |
| `/game_entity/extender[talk]/talker` | block | [str]x727 | 718 | 727 | "tank" ¦ "cannon" | 미확인(자동) |  |  |
| `/game_entity/extension` | block | [str]x2456 | 2384 | 2456 | 예: "blenheim_mk1.mdl" | 추정(파일 주석) | needs full QA | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/hearingbias` | block | [num]x533 | 533 | 533 | 0 ~ 250 | 추정(파일 주석) | reduces units ability to hear sound signals. value = meters ¦ in meters | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/lodclass` | block | [str]x988 | 763 | 988 | "vehicle" ¦ "cannon" ¦ "vehicle_plane" | 미확인(자동) |  |  |
| `/game_entity/mass` | block | [num]x763 | 763 | 763 | 5.5 ~ 1380000 | 추정(파일 주석) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/nogroupmesh` | block | [num]x763 | 763 | 763 | 1 | 미확인(자동) |  |  |
| `/game_entity/obstacleid` | block | [str]x1584 | 1572 | 1584 | "tank" ¦ "marine" ¦ "unbreakable" | 미확인(자동) |  |  |
| `/game_entity/patherid` | block | [str]x2038 | 763 | 2038 | 예: "vehicle.large" | 미확인(자동) |  |  |
| `/game_entity/perks` | block | []x2206 | 763 | 2206 | 예: | 미확인(자동) |  |  |
| `/game_entity/perks/blastwave_modifier` | block | [num]x763 | 763 | 763 | 5 ~ 20 | 미확인(자동) |  |  |
| `/game_entity/perks/component` | block | [str]x12549 [atom str]x9742 | 763 | 22291 | 예: "body" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/perks/component/armor_damage_modifier` | block | [num]x20364 | 763 | 20364 | 0 ~ 5 | 추정(파일 주석) | multiplier adjusts ratio of which part of penetration does damage to component. (shell penetration - armor thickness) verses minimumDamageModifier. values < 1 = less damage done by over penetration. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/perks/component/health` | block | []x22291 | 763 | 22291 | 예: | 추정(파일 주석) | A better way to translate this is that it will deal 2x damage to the enemy. In the editor it is hard to understand, as if you're switching between players it will always display the hitpoints of "player". But the enemy units on that particular instance will have half the total health. ¦ health multiplier from soldier settings | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/perks/component/health/hp` | block | [num]x22291 | 763 | 22291 | 1 ~ 750 | 미확인(자동) |  |  |
| `/game_entity/perks/component/health/hp_broken` | block | [num]x9473 [expr]x952 | 763 | 10425 | 1 ~ 1350 | 추정(파일 주석) | destroys component and cannot be repaired | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/placer` | block | []x7117 | 763 | 7117 | 예: | 추정(파일 주석) | sits out of view from top of turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/placer/crew` | block | []x763 [num]x3 | 763 | 766 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place` | block | [str]x8323 [str atom]x52 | 706 | 8375 | 예: "driver" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/placer/place/attach` | block | []x6 | 6 | 6 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/entity` | block | [str]x49 | 13 | 49 | "lee_enfield_no1_mk3" ¦ "76mm_rus_he" ¦ "76mm_rus_apcr" ¦ "ammo_boxes_01" ¦ "ammo_boxes_04" ¦ "crate4" ¦ "ammo_boxes_03" | 추정(파일 주석) | place holder | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/placer/place/group` | block | [str]x5965 | 682 | 5965 | "crew" ¦ "passenger" ¦ "%group" | 추정(파일 주석) | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/placer/place/handlebone` | block | [str]x55 | 16 | 55 | "body" ¦ "basis" | 미확인(자동) |  |  |
| `/game_entity/placer/place/linkbone` | block | [str]x4724 | 680 | 4724 | 예: "driver" | 미확인(자동) |  |  |
| `/game_entity/placer/place/openforcrush` | block | []x750 | 204 | 750 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/sealable` | block | []x43 | 43 | 43 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff` | block | []x1143 | 207 | 1143 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff/sensor` | block | []x877 [num]x6 | 187 | 883 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff/shadow` | block | []x18 | 6 | 18 | 예: | 미확인(자동) |  |  |
| `/game_entity/placer/place/turnoff/targetable` | block | [num]x445 []x262 | 119 | 707 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place/usevision` | block | [num]x83 | 82 | 83 | 0 | 미확인(자동) |  |  |
| `/game_entity/placer/place/visor` | block | [str]x1936 | 628 | 1936 | 예: "main" | 미확인(자동) |  |  |
| `/game_entity/props` | block | [str str str str]x978 [str]x845 [str str str]x633 [str str]x247 [str str str str str]x134 | 1572 | 3442 | 예: "steel" "vehicle" "obstacle_ignore_height" "emit_on_sink" | 미확인(자동) |  |  |
| `/game_entity/rimlight` | block | []x1519 | 1519 | 1519 | 예: | 미확인(자동) |  |  |
| `/game_entity/sensor` | block | []x1754 | 763 | 1754 | 예: | 미확인(자동) |  |  |
| `/game_entity/sensor/informrange` | block | [num num]x763 | 763 | 763 | 50 75 | 추정(파일 주석) | first = AI , second = USER | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/sensor/recognizetime` | block | [num]x763 | 763 | 763 | 0.01 | 미확인(자동) |  |  |
| `/game_entity/sensor/visor` | block | [str]x4209 | 763 | 4209 | 예: "hearing" | 미확인(자동) |  |  |
| `/game_entity/sensor/visor/(mod)` | mod | [str]x40 [atom str]x40 | 20 | 80 | "mp" ¦ not "mp" | 미확인(자동) |  |  |
| `/game_entity/sensor/visor/bone` | block | [str]x3529 | 763 | 3529 | 예: "hearing_bone" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/sensor/visor/vision` | block | [str]x4220 | 763 | 4220 | 예: "noise_detection" | 추정(파일 주석) | better vision than a in-closed turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/simulation` | block | []x814 | 763 | 814 | 예: | 미확인(자동) |  |  |
| `/game_entity/simulation/able` | block | [num]x814 | 763 | 814 | 0 ~ 1 | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/simulation/disablelinked` | block | [num]x50 | 47 | 50 | 0 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/simulatorid` | block | [str]x30 | 30 | 30 | "airborne" ¦ "water" | 미확인(자동) |  |  |
| `/game_entity/skeleton` | block | []x599 | 599 | 599 | 예: | 미확인(자동) |  |  |
| `/game_entity/skeleton/bone` | block | [str]x599 | 599 | 599 | "hearing_bone" | 추정(파일 주석) | Boys turret ¦ turret | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/skeleton/bone/position` | block | [num num num]x599 | 599 | 599 | 0 0 70 ¦ 0 0 40 ¦ 0 0 50 | 추정(파일 주석) | X (Forward/Backward) Y (Left/Right) Z (Up/Down) | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/targetclass` | block | [str]x753 [str str]x597 | 752 | 1350 | 예: "airborne" | 미확인(자동) |  |  |
| `/game_entity/targetselector` | block | [str]x1355 | 752 | 1355 | 예: "airborne_bullet" | 미확인(자동) |  |  |
| `/game_entity/texmod` | block | [str]x17 | 17 | 17 | "late" ¦ "rus" ¦ "camo" ¦ "camo2" ¦ "grey" ¦ "2" ¦ "early" ¦ "red_ball" | 미확인(자동) |  |  |
| `/game_entity/volume` | block | [str]x244828 | 1945 | 244828 | 예: "select" | 추정(파일 주석) | Hull ¦ Superstructure | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/able` | block | []x68087 [atom]x42682 | 1939 | 110769 | ¦ clear | 추정(파일 주석) | disables vehicle from being crewed until body is repaired ¦ stops uncontrollable actions | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/able/blast` | block | []x106 | 22 | 106 | 예: | 추정(파일 주석) | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/able/bullet` | block | []x106 | 22 | 106 | 예: | 추정(파일 주석) | required for AT-Grenades with cumaltive damage | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/able/contact` | block | [num]x37613 []x114 | 1513 | 37727 | 0 ~ 1 | 미확인(자동) |  |  |
| `/game_entity/volume/able/contact_ground` | block | [num]x1556 | 394 | 1556 | 0 | 미확인(자동) |  |  |
| `/game_entity/volume/able/float` | block | []x127 | 12 | 127 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/hole` | block | []x40875 | 1572 | 40875 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/holed` | block | []x4162 | 1238 | 4162 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/nolook` | block | [num]x37712 | 1404 | 37712 | 1 | 미확인(자동) |  |  |
| `/game_entity/volume/able/nowounddecal` | block | []x25412 [num]x92 | 1123 | 25504 | 0 | 미확인(자동) |  |  |
| `/game_entity/volume/able/obstacle` | block | [num]x36858 []x1 | 1813 | 36859 | 0 | 미확인(자동) |  |  |
| `/game_entity/volume/able/select` | block | []x763 | 763 | 763 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/touch` | block | []x869 | 763 | 869 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/able/visible` | block | [num]x39358 | 1518 | 39358 | 0 | 추정(파일 주석) | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/able/window` | block | []x485 | 112 | 485 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/caststeel` | block | [num]x2543 | 379 | 2543 | 0.3 ~ 0.88 | 추정(파일 주석) | effective armor reduction compared to solid plate ¦ effective armor reduction compared to solid plate; 25 + 75 spaced armor array ~86mm effective | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/component` | block | [str]x89210 | 798 | 89210 | 예: "wingright" | 추정(파일 주석) | Bucket | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/density` | block | [num]x21373 | 765 | 21373 | 0 ~ 0.1 | 미확인(자동) |  |  |
| `/game_entity/volume/highhardness` | block | []x543 | 77 | 543 | 예: | 미확인(자동) |  |  |
| `/game_entity/volume/material` | block | [str]x11376 | 823 | 11376 | "wood" ¦ "sand" ¦ "glass" ¦ "fabric" ¦ "steel" ¦ "concrete" | 미확인(자동) |  |  |
| `/game_entity/volume/quality` | block | [str]x75 | 15 | 75 | "large_flaw" | 미확인(자동) |  |  |
| `/game_entity/volume/tags` | block | [str]x77996 [str str]x66189 []x300 [str str str]x130 | 1572 | 144615 | 예: "wood" | 추정(파일 주석) | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness` | block | [num]x113865 []x292 | 1569 | 114157 | 0.05 ~ 406 | 추정(파일 주석) | 15 + 4 ¦ 89 + 89 | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/bottom` | block | [num]x3522 | 680 | 3522 | 1 ~ 200 | 추정(파일 주석) | Overhang (above tracks) ¦ 4mm + .25" | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/front` | block | [num]x4000 | 707 | 4000 | 3 ~ 300 | 추정(파일 주석) | Glacis ¦ Lower front | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/left` | block | [num]x784 | 222 | 784 | 5 ~ 95.25 | 추정(파일 주석) | Top sides ¦ side | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/rear` | block | [num]x2475 | 667 | 2475 | 1 ~ 110 | 추정(파일 주석) | Front Roof Turret Armor ¦ Bottom Front Nose | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/right` | block | [num]x565 | 188 | 565 | 5 ~ 600 | 추정(파일 주석) | armor plate sides ¦ Top sides | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/volume/thickness/top` | block | [num]x4440 | 762 | 4440 | 1 ~ 200 | 추정(파일 주석) | Glacis ¦ front upper part | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry` | block | []x1663 | 763 | 1663 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place` | block | [str]x51 | 51 | 51 | "mgun" ¦ "mgun2" ¦ "bomb1" | 추정(파일 주석) | maybe future feature | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/accuracybymotion` | block | []x4 | 4 | 4 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/accuracybymotion/byangularspeed` | block | [num]x4 | 4 | 4 | 0.5 | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/accuracybymotion/bylinearspeed` | block | [num]x4 | 4 | 4 | 0.4 | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/accuracybymotion/disordertime` | block | [num]x4 | 4 | 4 | 0.6 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/accuracybymotion/maxangularspeed` | block | [num]x4 | 4 | 4 | 30 | 추정(파일 주석) | deg/s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/accuracybymotion/maxlinearspeed` | block | [num]x4 | 4 | 4 | 8 | 추정(파일 주석) | km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/accuracybymotion/ordertime` | block | [num]x4 | 4 | 4 | 2 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/ai_in_manual` | block | []x48 | 48 | 48 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/barrels` | block | [num]x2 | 2 | 2 | 2 ~ 3 | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/basic` | block | []x51 | 51 | 51 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/charger` | block | [str]x7 | 7 | 7 | "driver" ¦ "gunner4" ¦ "commander" ¦ "gunner2" ¦ "gunner" | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/foresight` | block | [str]x45 | 45 | 45 | "vision_rot" ¦ "foresight2" | 추정(파일 주석) | bone in weapon entity to define origin of fx and bullet ¦ bone for muzzleflash fx | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/gunner` | block | [str]x51 | 51 | 51 | "commander" ¦ "driver" ¦ "gunner4" ¦ "gunner2" ¦ "gunner" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/linkanimation` | block | [str]x4 | 4 | 4 | "off" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/linkbone` | block | [str]x4 | 4 | 4 | "mgun2" ¦ "mgun_rot" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/shooter` | block | [str]x2 | 2 | 2 | "shooter_bb" | 미확인(자동) |  |  |
| `/game_entity/weaponry/##place/type` | block | [str]x48 | 48 | 48 | "commander" ¦ "rear" ¦ "aa" | 추정(파일 주석) | "unit tag" or "unit type" + "current main weapon tag" | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/##place/weapon` | block | [str]x44 [str atom]x2 [str atom str num]x1 | 47 | 47 | "commander_vision" ¦ "bomb_dropper_eng_250x2" filled ¦ "bomb_dropper_usa_250x3" filled ¦ "thrower_v" filling "f1 grenade" 1 | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/manualaccuracy` | block | [num]x763 | 763 | 763 | 1 | 미확인(자동) |  |  |
| `/game_entity/weaponry/manualaccuracyzoom` | block | [num]x763 | 763 | 763 | 1.25 | 미확인(자동) |  |  |
| `/game_entity/weaponry/place` | block | [str]x1573 | 687 | 1573 | 예: "mgun1" | 추정(파일 주석) | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/accuracybymotion` | block | []x1298 | 665 | 1298 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/accuracybymotion/byangularspeed` | block | [num]x1298 | 665 | 1298 | 0.25 ~ 0.55 | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/accuracybymotion/bylinearspeed` | block | [num]x1298 | 665 | 1298 | 0.25 ~ 0.8 | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/accuracybymotion/disordertime` | block | [num]x1298 | 665 | 1298 | 0 ~ 1 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/accuracybymotion/maxangularspeed` | block | [num]x1298 | 665 | 1298 | 1 ~ 30 | 추정(파일 주석) | deg/s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/accuracybymotion/maxlinearspeed` | block | [num]x1298 | 665 | 1298 | 1 ~ 50 | 추정(파일 주석) | km/h | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/accuracybymotion/ordertime` | block | [num]x1298 | 665 | 1298 | 0 ~ 10 | 추정(파일 주석) | s | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/ai_in_manual` | block | []x473 | 313 | 473 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/barrels` | block | [num]x83 | 72 | 83 | 1 ~ 60 | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/basic` | block | []x828 | 464 | 828 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/charger` | block | [str]x1149 [str atom num]x172 | 658 | 1321 | 예: "driver" | 추정(파일 주석) | done to keep gunner3 from emitting | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/foresight` | block | [str]x583 | 329 | 583 | 예: "foresight3" | 추정(파일 주석) | bone in weapon entity to define origin of fx and bullet ¦ bone for muzzleflash fx | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/gunner` | block | [str]x1322 [str atom num]x45 | 657 | 1367 | 예: "driver" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/linkanimation` | block | [str]x203 | 178 | 203 | "off" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/linkbone` | block | [str]x213 | 183 | 213 | "mgun" ¦ "mgun2" ¦ "mgun_rot" ¦ "mgun1" ¦ "mgun3" ¦ "mgun_bone" ¦ "mgun4" ¦ "dshk_mgunF" ¦ "dshk_mgunB" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/restoreikafteraim` | block | []x531 | 510 | 531 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/rotate` | block | []x142 | 116 | 142 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/shooter` | block | [str]x36 | 21 | 36 | "shooter" ¦ "shooter_mg" ¦ "shooter_bb" ¦ "shooter_b" ¦ "shooter_r" | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/stationaryfiring` | block | []x16 | 16 | 16 | 예: | 미확인(자동) |  |  |
| `/game_entity/weaponry/place/type` | block | [str]x756 | 437 | 756 | 예: "rear" | 추정(파일 주석) | "unit tag" or "unit type" + "current main weapon tag" | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/place/weapon` | block | [str atom str num]x799 [str atom]x419 [str]x11 [str atom str]x3 | 635 | 1232 | 예: "browning_303_mk2" filling "ammo mgun_usa belt" 500 | 추정(파일 주석) | Placeholder weapon ¦ !!! PLACEHOLDER | 게임 파일 내 주석(같은 이름의 키) |
| `/game_entity/weaponry/restoreiktime` | block | [num]x522 | 522 | 522 | 4 ~ 10 | 미확인(자동) |  |  |

