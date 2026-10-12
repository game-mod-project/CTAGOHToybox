# 수정 가능한 속성: 차량류 (차량, 전차, 화포, 항공기, 함정)

이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다. 설명은 `data/annotations/common.psv`에서 고친다. 표 읽는 법은 [README.md](README.md)에 있다.

차량류 하나는 `entity/-vehicle` 아래의 `.def` 파일 하나다. 많은 값이 본체가 아니라 include하는 파일에 있다. 장갑·기동 같은 차종 공통 값은 `properties/`의 공통 속성에, 그 차량만의 값은 같은 폴더의 `.ext`와 `.inc`에 있다. 탑재 무기와 포탄의 속성은 [weapon-ammo.md](weapon-ammo.md)에 있다.

| 구분 | 파일 | 위치 | 속성 수 | 뜻을 아는 속성 |
|---|---|---|---|---|
| 본체 | 차량 엔티티 | `entity/-vehicle/<진영>/<분류>/<이름>/<이름>.def` | 282 | 113 |
| 연관 | 차량 전용 속성 (.ext) | `entity/-vehicle/**/*.ext` | 180 | 0 |
| 연관 | 차량 전용 속성 (.inc) | `entity/-vehicle/**/*.inc` | 113 | 1 |
| 연관 | 공통 속성 (.ext) | `properties/**/*.ext` | 888 | 1 |
| 연관 | 공통 속성 (.inc) | `properties/**/*.inc` | 605 | 0 |

## 본체: 차량 엔티티

- 위치: `entity/-vehicle/<진영>/<분류>/<이름>/<이름>.def`
- 연결: 유닛 그 자체. 파괴 모델용 `.def`도 같은 형식이다

### 뜻을 아는 속성 (113개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/{}/extender[inventory]/box/item` | 예: "bulleteng_40 ap2" 60 (13종 이상) | **추정** 실려 있는 아이템과 수량 (추정: /game_entity 아래의 같은 자리와 값 형태가 같음) | 600 rounds ¦ 50 rounds | 49 | `entity/-vehicle/eng/cannon/40mm_qf2_early/40mm_qf2_early.def` |
| `/{}/props` | "cannon_sandbag_standard" ¦ "mg34_lafette" "cannon_sandbag_standard_ai" "no_scavenge" | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 3 | `entity/-vehicle/germany/cannon/mg42_lafette/heavy_mg_mg34.def` |
| `/actor/extension` | "inf_crate.mdl" ¦ "usa_stretcher.mdl" | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | needs full QA | 6 | `entity/-vehicle/generic/special/inf_crate_eng/inf_crate_eng.def` |
| `/actor/mass` | 100 | **추정** 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 6 | `entity/-vehicle/generic/special/inf_crate_eng/inf_crate_eng.def` |
| `/actor/patherid` | "cannon.strecher" | **추정** 길찾기 분류 이름 (추정: 값 "vehicle.large". set/PATHER.SET 참조로 보임) |  | 1 | `entity/-vehicle/usa/special/usa_stretcher/usa_stretcher.def` |
| `/actor/props` | "vision_lev00_can" "ammo_crate" "no_recrew_logic" "sq_ammo_supply" ¦ "vision_lev00_can" "no_recrew_logic" | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 6 | `entity/-vehicle/generic/special/inf_crate_eng/inf_crate_eng.def` |
| `/actor/volume` | "body1" | **추정** 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | Hull ¦ Superstructure | 1 | `entity/-vehicle/usa/special/usa_stretcher/usa_stretcher.def` |
| `/actor/volume/material` | "flesh" | **추정** 볼륨의 재질 이름. set/material.set 참조로 보인다 (추정: 값 "wood") |  | 1 | `entity/-vehicle/usa/special/usa_stretcher/usa_stretcher.def` |
| `/entity/extension` | "universal.mdl" | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | needs full QA | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/mass` | 2100 | **추정** 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/props` | "truck" "tractor" "tractor_heavy" | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/volume` | "fuel" ¦ "transmission" | **추정** 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | Hull ¦ Superstructure | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/volume/able/contact` | 1 | **추정** 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/forward/begin` | "board_thor_front" ¦ "board_thor_rear" | **추정** 동작을 시작할 때의 애니메이션 이름 (추정: 값 "board_torpedo_boat") |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/forward/end` | "pose_thor_gunner_animation" 10 ¦ "pose_thor_gunner2_animation" 10 ¦ "pose_thor_charger_animation" 10 ¦ "pose_thor_charger2_animation" 10 ¦ "pose_watch_binocular" 10 ¦ "seat_thor_driver1" ¦ "seat_thor_driver2" ¦ "seat_thor_driver3" ¦ "seat_thor_driver4" ¦ "pose_watch_binocular" | **추정** 동작이 끝난 뒤의 자세 애니메이션 이름 (추정: 값 "seat_driver_car") |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm/reverse/end` | "emit_tank_2_hold" | **추정** 동작이 끝난 뒤의 자세 애니메이션 이름 (추정: 값 "seat_driver_car") |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/door` | "emit1" ¦ "emit2" ¦ "emit3" | **추정** 출입 지점 이름 (추정: 값 "emit1") |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/group` | "crew" | **추정** 자리의 분류. 값 "crew" 등 (추정: 값) |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human")/boarder/door` | "emit1" ¦ "emit2" ¦ "%door" | **추정** 출입 지점 이름 (추정: 값 "emit1") |  | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/placer/place/group` | "crew" ¦ "%group" | **추정** 자리의 분류. 값 "crew" 등 (추정: 값) |  | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew")/boarder/door` | "emit1" ¦ "emit2" | **추정** 출입 지점 이름 (추정: 값 "emit1") |  | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "crew")/placer/place/group` | "%group" | **추정** 자리의 분류. 값 "crew" 등 (추정: 값) |  | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/##bone/speed2` | 180 | **추정** 회전 속도 (추정: 주석 "manual handcrank, extremely slow") | manual handcrack, extremely slow ¦ remote controlled from inside | 43 | `entity/-vehicle/eng/tank_medium/sherman3_late/sherman3_late.def` |
| `/game_entity/##extender/cooldown` | 5 | **추정** 재사용 대기 시간 (추정: 이름과 값 5~20) | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 1 | `entity/-vehicle/finland/tank_medium/panzer4j_fin/panzer4j_fin.def` |
| `/game_entity/##extender/itemtags` | "grenade nbks" | **추정** 사용하는 탄의 태그 (추정: 값 "mortareng_50 sm") |  | 1 | `entity/-vehicle/finland/tank_medium/panzer4j_fin/panzer4j_fin.def` |
| `/game_entity/##sensor/visor/vision` | "" ¦ "vehicle_around" | **추정** 시야·청각 프로필 이름. set/vision 참조로 보인다 (추정: 값 "noise_detection") | better vision than a in-closed turret | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/##volume/thickness` | 10 | **추정** 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 15 + 4 ¦ 89 + 89 | 2 | `entity/-vehicle/finland/tank_medium/x/t28e_fin_xx/t28e_fin_late_xx.def` |
| `/game_entity/able` | "emit_on_attack" ¦ "emit_passangers_on_enemy" ¦ "emit_on_attack" "emit_crew_on_injure" | **추정** 엔티티가 할 수 있는 동작 태그 목록. 예: "emit_passangers_on_enemy", "repair" (추정: 값) |  | 130 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/boarder/anm` | 예: "driver" (13종 이상) | **추정** 좌석별 탑승 애니메이션. 문자열이 좌석 이름 (추정: 값 "driver", "gunner") | gunner ¦ commander | 595 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/boarder/anm/forward/begin` | 예: "board_plane_left" (13종 이상) | **추정** 동작을 시작할 때의 애니메이션 이름 (추정: 값 "board_torpedo_boat") |  | 409 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/boarder/anm/forward/end` | 예: "seat_driver_car" (13종 이상) | **추정** 동작이 끝난 뒤의 자세 애니메이션 이름 (추정: 값 "seat_driver_car") |  | 593 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/boarder/anm/reverse/end` | 예: "board_plane_left" -1 (13종 이상) | **추정** 동작이 끝난 뒤의 자세 애니메이션 이름 (추정: 값 "seat_driver_car") |  | 409 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/boarder/door` | "emit" ¦ "emit1" ¦ "emit0" | **추정** 출입 지점 이름 (추정: 값 "emit1") |  | 3 | `entity/-vehicle/generic/train/drg_class75/drg_class75.def` |
| `/game_entity/boarder/link` | "driver" "gunner" ¦ "emit" "driver" ¦ "emit1" "driver" ¦ "emit0" "driver" | **확인** 출입 지점과 좌석의 연결. { link <from> <to> } |  | 6 | `entity/-vehicle/generic/train/drg_class75/drg_class75.def` |
| `/game_entity/boarder/link/anm` | "driver_left" ¦ "driver_right" ¦ "driver" | **추정** 이 연결에 쓰는 좌석 애니메이션 이름 (추정: 값) | gunner ¦ commander | 3 | `entity/-vehicle/generic/train/drg_class75/drg_class75.def` |
| `/game_entity/bone` | 예: "gun_rot" (13종 이상) | **추정** 모델 뼈대 하나의 동작 설정. 포탑·총가 회전에 쓴다 (추정: 값 "mgun_rot1", 주석 turret) | Boys rot ¦ turret | 597 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/bone/limits` | 예: -15 15 (13종 이상) | **추정** 회전 한계 각도 두 개 (추정: 주석 "elevation range = 0 +70") | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 563 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/bone/speed2` | 1.5 ~ 360 | **추정** 회전 속도 (추정: 주석 "manual handcrank, extremely slow") | manual handcrack, extremely slow ¦ remote controlled from inside | 592 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/chassis` | ¦ "track" ¦ "wheel" ¦ "airborne" ¦ "water" ¦ "human_carrier" ¦ "railway" | **추정** 구동계 정의. 문자열은 종류: "wheel", "track" 등 (추정: properties/airborne.ext의 {Chassis "wheel" ...}) | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 684 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/chassis/fuelbag/fuel` | "fuel" | **확인** 연료 종류. "fuel"은 휘발유, "diesel"은 경유 |  | 1 | `entity/-vehicle/germany/tank_light/h39_turret/h39_turret.def` |
| `/game_entity/chassis/fuelbag/rate` | 0 | **확인** 이동 중 연료 소모율. 차량의 비포장 주행 거리를 기준으로 정했다 |  | 1 | `entity/-vehicle/germany/tank_light/h39_turret/h39_turret.def` |
| `/game_entity/chassis/fuelbag/remain` | 0 | **확인** 스폰될 때 들어 있는 연료량 |  | 1 | `entity/-vehicle/germany/tank_light/h39_turret/h39_turret.def` |
| `/game_entity/chassis/fuelbag/volume` | 0 | **확인** 연료 탱크 용량, 리터 | Hull ¦ Superstructure | 1 | `entity/-vehicle/germany/tank_light/h39_turret/h39_turret.def` |
| `/game_entity/chassis/locomotion/braketime` | 3 ~ 15 | **확인** 브레이크를 써서 최고 속도에서 정지하는 시간, 초 |  | 7 | `entity/-vehicle/eng/marine/lca/lca.def` |
| `/game_entity/chassis/locomotion/fast` | 0 | **확인** 빠른 이동 속도. 이동 명령을 더블클릭했을 때 |  | 1 | `entity/-vehicle/germany/tank_light/h39_turret/h39_turret.def` |
| `/game_entity/chassis/locomotion/maxspeed` | 0 ~ 70 | **확인** 최고 속도, km/h |  | 9 | `entity/-vehicle/eng/marine/lca/lca.def` |
| `/game_entity/chassis/locomotion/normal` | 0 | **확인** 보통 이동 속도. 이동 명령을 한 번 클릭했을 때 |  | 1 | `entity/-vehicle/germany/tank_light/h39_turret/h39_turret.def` |
| `/game_entity/chassis/locomotion/slow` | 0 | **확인** 느린 이동 속도. 주로 미션 스크립트에서 쓴다. km/h |  | 1 | `entity/-vehicle/germany/tank_light/h39_turret/h39_turret.def` |
| `/game_entity/chassis/locomotion/starttime` | 3 ~ 18 | **확인** 정지에서 최고 속도까지 가속하는 시간, 초 |  | 7 | `entity/-vehicle/eng/marine/lca/lca.def` |
| `/game_entity/chassis/locomotion/turnradius` | 4 ~ 20 | **확인** AI의 회전 반경, 미터. 작을수록 멈추지 않고 돈다 |  | 4 | `entity/-vehicle/eng/marine/lca/lca.def` |
| `/game_entity/chassis/locomotion/turntime` | 45 | **확인** 직접 조종 시 최대 회전 속도로 360도 도는 시간 |  | 3 | `entity/-vehicle/eng/marine/lca/lca.def` |
| `/game_entity/collider` | "tank.light" ¦ "tank.heavy" ¦ "truck" ¦ "cannon_small" ¦ "car" ¦ "train" | **추정** 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") |  | 254 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/component` | "body" ¦ "wheels_left" ¦ "wheels_right" | **추정** 부품 하나의 설정. 문자열이 부품 이름 (추정: vehicle.ext의 매크로) | Bucket | 7 | `entity/-vehicle/generic/train/tram_eu/tram_eu.def` |
| `/game_entity/component/tags` | "mover" | **추정** 부품 태그 (추정: 값 "wheel", "no_target") |  | 2 | `entity/-vehicle/generic/train/tram_eu/tram_eu.def` |
| `/game_entity/extender[cannon]` | "cannon" | **추정** 화포 운용 기능. 밀기·사격·견인 상태를 정의한다 (추정: 하위 블록의 주석) |  | 188 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/animation/aim` | 예: "mortar_82bm37_gunner_aim" "mortar_82bm37_commander_aim" (13종 이상) | **추정** 조준 애니메이션. 자리 순서대로 나열 (추정: pak9738.def의 주석이 자리 순서를 적음) |  | 179 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/animation/fire` | 예: "mortar_82bm37_gunner_fire" "mortar_82bm37_commander_fire" (13종 이상) | **추정** 발사 애니메이션. 자리 순서대로 (추정: 위와 같음) | mg_maxim_gunner_fire | 172 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/animation/move` | 예: "mortar_driver_r" "mortar_driver_l" (13종 이상) | **확인** 이동 상태: 화포를 밀 때 각 자리가 재생하는 애니메이션 | move state: cannon being pushed and which positions play animations | 187 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/animation/reload` | 예: "" "mortar_82bm37_commander_reload" (13종 이상) | **추정** 장전 애니메이션. 자리 순서대로 (추정: 위와 같음) |  | 183 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/attack/workers` | "gunner" "charger" "charger2" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "commander" ¦ "gunner" "charger" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "charger3" "commander" ¦ "gunner" "commander" ¦ "gunner" ¦ "" "" "" "" "" ¦ "" "" "" ¦ clear | **추정** 그 상태에서 일하는 자리 이름 목록. minWorkers 뒤는 최소 인원 (추정: 값) |  | 134 | `entity/-vehicle/eng/cannon/114mm_bl_4_5/114mm_bl_4_5.def` |
| `/game_entity/extender[cannon]/move/workers` | "driver1" "driver2" "driver3" "driver4" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 2 ¦ "driver1" "driver2" minWorkers 1 ¦ clear ¦ "driver1" "driver2" "driver3" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" "driver6" minWorkers 2 ¦ "driver1" "driver2" minWorkers 2 ¦ "driver" minWorkers 1 ¦ "driver1" "driver2" "driver3" "driver4" minWorkers 3 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 1 | **추정** 그 상태에서 일하는 자리 이름 목록. minWorkers 뒤는 최소 인원 (추정: 값) |  | 149 | `entity/-vehicle/eng/cannon/114mm_bl_4_5/114mm_bl_4_5.def` |
| `/game_entity/extender[cannon]/trailing/workers` | "seat1" "seat2" "seat3" "seat4" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" ¦ "seat1" "seat2" "seat3" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" "seat6" ¦ clear ¦ "seat1" "seat2" | **추정** 그 상태에서 일하는 자리 이름 목록. minWorkers 뒤는 최소 인원 (추정: 값) |  | 126 | `entity/-vehicle/eng/cannon/114mm_bl_4_5/114mm_bl_4_5.def` |
| `/game_entity/extender[enumerator]` | "enumerator" | **추정** 차량 번호 표시 기능 (추정: 하위 digit_folder) |  | 328 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/extender[enumerator]/digit_folder` | "us2" ¦ "ru1" ¦ "ger" ¦ "np" ¦ "ger2" ¦ "moe" ¦ "np_w" ¦ "us3" ¦ "us1" ¦ "ru3" ¦ "c47" ¦ "ru2" | **추정** 숫자 텍스처 폴더 이름 (추정: 이름과 값 "us2") |  | 328 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/extender[inventory]` | "inventory" | **추정** 차량 화물칸 기능 (추정: 하위 box) |  | 747 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/extender[inventory]/box/item` | 예: "ammo mgun_usa belt" 1000 (13종 이상) | **추정** 실려 있는 아이템과 수량 (추정: 값 "ammo mgun_usa belt" 1000) | 600 rounds ¦ 50 rounds | 691 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/extender[inventory]/box/size` | 9 12 ¦ 4 2 ¦ 0 0 ¦ 6 4 ¦ 9 6 ¦ 9 9 ¦ 6 6 ¦ 9 11 | **확인** 인벤토리 창의 크기. 가로 세로 칸 수 |  | 14 | `entity/-vehicle/eng/marine/goatley_boat/goatley_boat.def` |
| `/game_entity/extender[inventory]/box/weight` | 2100 ~ 100000 | **추정** 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) |  | 16 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.def` |
| `/game_entity/extender[inventory]/weapon` | "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | **추정** 인벤토리에 장착할 수 있는 무기 종류 (추정: 값 "mgun") | Placeholder weapon ¦ !!! PLACEHOLDER | 102 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/extender[shield]` | "shield" | **추정** 방패 기능 (추정: 이름. 화포에 쓰임) |  | 206 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/extender[smokescreen]` | "smokescreen" | **추정** 연막 발사 기능 (추정: 하위 키) |  | 136 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/extender[smokescreen]/cooldown` | 5 ~ 20 | **추정** 재사용 대기 시간 (추정: 이름과 값 5~20) | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 136 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/extender[smokescreen]/itemtags` | "mortareng_50 sm" ¦ "mortareng_50 wp" ¦ "grenade no8_smoke" ¦ "grenade nbks" | **추정** 사용하는 탄의 태그 (추정: 값 "mortareng_50 sm") |  | 136 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/extension` | 예: "blenheim_mk1.mdl" (13종 이상) | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | needs full QA | 2324 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/mass` | 5.5 ~ 1380000 | **추정** 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 698 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/obstacleid` | "unbreakable" | **추정** 장애물 분류 이름 (추정: 값 "tank", "stuff") |  | 2 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/patherid` | 예: "blenheim" (13종 이상) | **추정** 길찾기 분류 이름 (추정: 값 "vehicle.large". set/PATHER.SET 참조로 보임) |  | 563 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/placer/place` | 예: "driver" (13종 이상) | **추정** 승무원 자리 하나. 문자열이 자리 이름 (추정: 값 "driver") | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 77 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/placer/place/entity` | "lee_enfield_no1_mk3" ¦ "76mm_rus_he" ¦ "76mm_rus_apcr" ¦ "ammo_boxes_01" ¦ "ammo_boxes_04" ¦ "crate4" ¦ "ammo_boxes_03" | **추정** 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) |  | 10 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/placer/place/group` | "crew" | **추정** 자리의 분류. 값 "crew" 등 (추정: 값) |  | 10 | `entity/-vehicle/generic/train/drg_class75/drg_class75.def` |
| `/game_entity/placer/place/linkbone` | 예: "weapon1" (13종 이상) | **추정** 승무원을 붙이는 뼈대 이름 (추정: 값 "driver") |  | 15 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/placer/place/visor` | "main" ¦ "none" ¦ "around" ¦ "rear" ¦ "gunner" ¦ "main_2" ¦ "mgun1" ¦ "driver" | **추정** 이 자리가 쓰는 시야 이름 (추정: 값 "main") |  | 33 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/props` | 예: "blenheim_mk1" "wheeled" "vision_lev02_air" (13종 이상) | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 727 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/sensor/visor` | 예: "rear" (13종 이상) | **추정** 감지기 하나. 문자열이 이름 (추정: 값 "hearing") |  | 201 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/sensor/visor/bone` | 예: "foresight6" (13종 이상) | **추정** 감지기의 기준 뼈대 (추정: 값 "hearing_bone") | Boys rot ¦ turret | 121 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/sensor/visor/vision` | 예: "airborne_main" (13종 이상) | **추정** 시야·청각 프로필 이름. set/vision 참조로 보인다 (추정: 값 "noise_detection") | better vision than a in-closed turret | 187 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/targetclass` | "tank_medium" ¦ "howitzer" ¦ "tank_light" ¦ "armoredcar" ¦ "tank_heavy" ¦ "cannon_big" ¦ "cannon_small" ¦ "car" ¦ "cannon_zenite" ¦ "armoredcar" "bullet" ¦ "cannon_bullet" ¦ "armoredcar_l" | **추정** 표적 분류. 다른 유닛이 이 엔티티를 어떤 종류의 표적으로 보는지 (추정: 값 "airborne") |  | 574 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/targetselector` | 예: "tank_light" (13종 이상) | **추정** 표적 선택 규칙 이름 (추정: 값 "airborne_bullet". set/target 참조로 보임) |  | 579 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/volume` | 예: "gear_l" (13종 이상) | **추정** 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | Hull ¦ Superstructure | 1421 | `entity/-vehicle/eng/airborne/horsa_mk1/horsa_mk1.def` |
| `/game_entity/volume/able` | ¦ clear | **추정** 볼륨이 반응하는 상호작용 묶음. clear는 상속된 설정을 지우는 것으로 보인다 (추정: 하위가 bullet, blast, touch 등 인자 없는 플래그) |  | 1090 | `entity/-vehicle/eng/airborne/horsa_mk1/horsa_mk1.def` |
| `/game_entity/volume/able/contact` | 0 ~ 1 | **추정** 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) |  | 57 | `entity/-vehicle/eng/btr/x/daimler_mk1_xx/daimler_mk1_lj_xx.def` |
| `/game_entity/volume/able/contact_ground` | 0 | **추정** 지면 접촉 판정 대상 (추정: able의 하위 플래그) |  | 3 | `entity/-vehicle/germany/btr/sdkfz10_4/sdkfz10_4.def` |
| `/game_entity/volume/able/nowounddecal` | 0 | **추정** 피격 자국을 그리지 않음 (추정: 이름. 바퀴 볼륨에 쓰임) |  | 53 | `entity/-vehicle/eng/btr/x/wasp_mk2c_xx/wasp_mk2c_xx.def` |
| `/game_entity/volume/caststeel` | 0.8063 | **확인** 주조 장갑 보정. 같은 두께의 압연판에 비해 유효 장갑이 줄어드는 비율 | effective armor reduction compared to solid plate ¦ effective armor reduction compared to solid plate; 25 + 75 spaced armor array ~86mm effective | 144 | `entity/-vehicle/eng/btr/x/aec_mk1_xx/aec_mk1_xx.def` |
| `/game_entity/volume/component` | "body" ¦ "turret" ¦ "gun" ¦ "engine" ¦ "bodyback" ¦ "gun1" ¦ "gun2" | **추정** 이 볼륨이 속한 부품 이름 (추정: vehicle.ext의 wheel 매크로가 {volume %0 {component %0}}) | Bucket | 44 | `entity/-vehicle/eng/tank_heavy/x/churchill_mk1_xx/churchill_mk1_xx.def` |
| `/game_entity/volume/material` | "wood" ¦ "steel" ¦ "fabric" ¦ "glass" | **추정** 볼륨의 재질 이름. set/material.set 참조로 보인다 (추정: 값 "wood") |  | 95 | `entity/-vehicle/eng/airborne/horsa_mk1/horsa_mk1.def` |
| `/game_entity/volume/tags` | 예: "invisible" (13종 이상) | **추정** 볼륨 태그. 재질이나 부위를 나타낸다 (추정: 값 "wood", "wheel", "body") |  | 159 | `entity/-vehicle/eng/cannon/152mm_bl_6/152mm_bl_6.def` |
| `/game_entity/volume/thickness` | 0.5 ~ 406 | **추정** 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 15 + 4 ¦ 89 + 89 | 891 | `entity/-vehicle/eng/airborne/x/blenheim_mk1_eng_xx/blenheim_mk1_eng_recon_xx.def` |
| `/game_entity/volume/thickness/bottom` | 1 ~ 200 | **추정** 하면 두께 (추정: thickness의 하위. 이름) | Overhang (above tracks) ¦ 4mm + .25" | 330 | `entity/-vehicle/eng/btr/x/aec_mk1_xx/aec_mk1_xx.def` |
| `/game_entity/volume/thickness/front` | 3 ~ 300 | **추정** 전면 두께 (추정: thickness의 하위이고 주석이 Glacis, Lower front) | Glacis ¦ Lower front | 345 | `entity/-vehicle/eng/btr/x/aec_mk1_xx/aec_mk1_xx.def` |
| `/game_entity/volume/thickness/left` | 6 ~ 95 | **추정** 좌측면 두께 (추정: thickness의 하위. 주석 Top sides, side) | Top sides ¦ side | 114 | `entity/-vehicle/eng/btr/x/daimler_dingo_xx/daimler_dingo_boys_xx.def` |
| `/game_entity/volume/thickness/rear` | 1 ~ 110 | **추정** 후면 두께 (추정: thickness의 하위. 이름) | Front Roof Turret Armor ¦ Bottom Front Nose | 325 | `entity/-vehicle/eng/btr/x/aec_mk1_xx/aec_mk1_xx.def` |
| `/game_entity/volume/thickness/right` | 10 ~ 600 | **추정** 우측면 두께 (추정: thickness의 하위. 주석 armor plate sides) | armor plate sides ¦ Top sides | 92 | `entity/-vehicle/eng/btr/x/aec_mk1_xx/aec_mk1_xx.def` |
| `/game_entity/volume/thickness/top` | 1 ~ 200 | **추정** 상면 두께 (추정: thickness의 하위. 이름) | Glacis ¦ front upper part | 372 | `entity/-vehicle/eng/btr/x/aec_mk1_xx/aec_mk1_xx.def` |
| `/game_entity/weaponry/##place/charger` | "driver" ¦ "gunner4" ¦ "commander" ¦ "gunner2" ¦ "gunner" | **추정** 이 무장을 장전하는 자리 이름 (추정: 값이 placer의 자리 이름) | done to keep gunner3 from emitting | 7 | `entity/-vehicle/eng/airborne/typhoon_mk1b_recon/typhoon_mk1b_recon.def` |
| `/game_entity/weaponry/##place/foresight` | "vision_rot" ¦ "foresight2" | **확인** 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 |  | 45 | `entity/-vehicle/eng/tank_medium/sherman3_late/sherman3_late.def` |
| `/game_entity/weaponry/##place/gunner` | "commander" ¦ "driver" ¦ "gunner4" ¦ "gunner2" ¦ "gunner" | **추정** 이 무장을 쏘는 자리 이름 (추정: 값이 placer의 자리 이름) |  | 51 | `entity/-vehicle/eng/airborne/typhoon_mk1b_recon/typhoon_mk1b_recon.def` |
| `/game_entity/weaponry/place` | 예: "mgun1" (13종 이상) | **추정** 무장 장착 위치 하나. 문자열이 위치 이름 (추정: 값 "mgun1", "gun") | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 654 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/weaponry/place/charger` | 예: "driver" (13종 이상) | **추정** 이 무장을 장전하는 자리 이름 (추정: 값이 placer의 자리 이름) | done to keep gunner3 from emitting | 618 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/weaponry/place/foresight` | 예: "foresight3" (13종 이상) | **확인** 총구 화염과 탄이 나가는 원점이 되는 뼈대 이름 |  | 326 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/weaponry/place/gunner` | 예: "driver" (13종 이상) | **추정** 이 무장을 쏘는 자리 이름 (추정: 값이 placer의 자리 이름) |  | 511 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/weaponry/place/linkbone` | "mgun" ¦ "mgun2" ¦ "mgun_rot" ¦ "mgun1" ¦ "mgun3" ¦ "mgun_bone" ¦ "mgun4" ¦ "dshk_mgunF" ¦ "dshk_mgunB" | **추정** 무장을 붙이는 뼈대 이름 (추정: 값 "mgun") |  | 160 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/weaponry/place/weapon` | 예: "browning_303_mk2" filling "ammo mgun_usa belt" 500 (13종 이상) | **추정** 장착 무기. 첫 문자열이 무기 이름, filling 뒤가 장전된 탄과 수량 (추정: 값. set/stuff의 파일명과 일치) | Placeholder weapon ¦ !!! PLACEHOLDER | 616 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |

### 뜻을 모르는 속성 (169개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/{}/extender[inventory]` | "inventory" | 미확인 |  | 49 | `entity/-vehicle/eng/cannon/40mm_qf2_early/40mm_qf2_early.def` |
| `/{}/extender[inventory]/box/item/user` | "mgun" | 미확인 |  | 3 | `entity/-vehicle/germany/cannon/mg42_lafette/heavy_mg_mg34.def` |
| `/{}/weaponry/place` | "gun" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 27 | `entity/-vehicle/eng/cannon/40mm_qf2_early/40mm_qf2_early.def` |
| `/{}/weaponry/place/weapon` | 예: "40mm_qf2" filling "bulleteng_40 ap2" 1 (13종 이상) | 미확인 | Placeholder weapon ¦ !!! PLACEHOLDER | 27 | `entity/-vehicle/eng/cannon/40mm_qf2_early/40mm_qf2_early.def` |
| `/actor/("supply_ammo")` | supply ( 250 ) | 미확인 |  | 5 | `entity/-vehicle/generic/special/inf_crate_eng/inf_crate_eng.def` |
| `/actor/extender[inventory]` | "inventory" | 미확인 |  | 6 | `entity/-vehicle/generic/special/inf_crate_eng/inf_crate_eng.def` |
| `/entity/("board_animation")` | place ( driver ) board_begin ( board_body ) board_end ( seat_universal_driver ) | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/("car_thickness")` | body ( 2 ) engine ( 2 ) | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/("crew_human_v")` | place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) group ( crew ) | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/("fuel")` | args "fuel" | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/("solid_wheel")` | args "wheelright1" ¦ args "wheelleft1" ¦ args "wheelright2" ¦ args "wheelleft2" | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/("transmission")` | args "transmission" | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/able` | "emit_on_attack" | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/chassis/("car_trace")` | fx ( "tracks_sm" ) step ( 0.3 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/entity/chassis/("mobility_car")` | speed ( 10 ) speedatmaxturn ( 8 ) reverse ( 8 ) weight ( 2.1 ) power ( 22 ) fuel ( 50 ) type ( fuel ) range ( 100 ) | 미확인 |  | 1 | `entity/-vehicle/generic/car/universal/universal.def` |
| `/game_entity/("ammo")` | args "ammo" ¦ args "ammo1" ¦ args "ammo2" ¦ args "ammo3" | 미확인 |  | 4 | `entity/-vehicle/eng/car/bedford_oyd_art_ammo/bedford_oyd_art_ammo.def` |
| `/game_entity/("board_animation")` | 예: place ( driver ) board_begin ( board_big-midcar ) board_end ( seat_driver_car ) base ( doorright ) (13종 이상) | 미확인 |  | 38 | `entity/-vehicle/eng/car/bedford_oyc_fuel/bedford_oyc_fuel.def` |
| `/game_entity/("body")` | repair ( 45 ) | 미확인 |  | 3 | `entity/-vehicle/germany/special/sdkfz302/sdkfz302.def` |
| `/game_entity/("bodyback")` | repair ( 30 ) | 미확인 |  | 43 | `entity/-vehicle/eng/car/bedford_oyc_fuel/bedford_oyc_fuel.def` |
| `/game_entity/("cannon_durability")` | body ( 150 ) gun ( 40 ) wheel ( 100 ) shield ( 200 ) | 미확인 |  | 2 | `entity/-vehicle/germany/special/150mm_sw34/150mm_sw34.def` |
| `/game_entity/("cannon_thickness")` | args 25 | 미확인 |  | 3 | `entity/-vehicle/finland/cannon/210mm_h17/210mm_h17.def` |
| `/game_entity/("car_thickness")` | body ( 1.5 ) engine ( 1.5 ) ¦ body ( 2.5 ) engine ( 1.5 ) ¦ body ( 3 ) engine ( 2 ) | 미확인 |  | 18 | `entity/-vehicle/eng/car/willys_mb_eng/willys_mb_eng.def` |
| `/game_entity/("component")` | volume ( "body" ) component ( "body" ) repair ( 10 ) | 미확인 |  | 1 | `entity/-vehicle/eng/marine/goatley_boat/goatley_boat.def` |
| `/game_entity/("crew_co_driver")` | place ( gunner3 ) door ( emit3 ) door2 ( emit1 ) door3 ( emit2 ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인 |  | 20 | `entity/-vehicle/eng/tank_heavy/churchill_mk1/churchill_mk1.def` |
| `/game_entity/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) ¦ door ( emit0 ) door2 ( emit2 ) door3 ( emit3 ) ¦ door ( emit0 ) anm_left ( driver ) anm_right ( driver ) visor ( driver ) ¦ door ( emit0 ) door2 ( emit2 ) door3 ( emit2 ) | 미확인 |  | 21 | `entity/-vehicle/eng/tank_light/cruiser_mk1/cruiser_mk1.def` |
| `/game_entity/("crew_human_2_v")` | place ( driver ) bone ( driver ) door ( emit1 ) door2 ( emit2 ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) door ( emit4 ) | 미확인 |  | 3 | `entity/-vehicle/germany/car/kettenkrad/kettenkrad.def` |
| `/game_entity/("crew_human_2")` | place ( gunner ) bone ( gunner ) door ( emit3 ) door2 ( emit4 ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) door ( emit4 ) | 미확인 |  | 4 | `entity/-vehicle/germany/car/blitz3_6_radio/blitz3_6_radio.def` |
| `/game_entity/("crew_human_v")` | place ( commander ) bone ( commander ) door ( emit2 ) animation ( commander ) group ( crew ) ¦ place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( passenger1 ) group ( passenger ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( passenger2 ) group ( passenger ) ¦ place ( seat1 ) bone ( seat1 ) door ( emit3 ) animation ( passenger1 ) group ( passenger ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) animation ( gunner ) group ( crew ) | 미확인 |  | 5 | `entity/-vehicle/germany/car/kettenkrad/kettenkrad.def` |
| `/game_entity/("crew_human_visible_anm")` | 예: place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( main ) anm_left ( driver ) anm_right ( driver ) (13종 이상) | 미확인 |  | 29 | `entity/-vehicle/eng/tank_light/lvt4_eng/lvt4_eng.def` |
| `/game_entity/("crew_human_visible_anm2")` | 예: place ( driver ) door ( emit2 ) door2 ( emit1 ) anm_left ( right ) anm_right ( right ) (13종 이상) | 미확인 |  | 24 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/("crew_human_visible")` | place ( gunner ) door ( emit2 ) visor ( main ) ¦ place ( commander ) door ( emit3 ) visor ( around ) | 미확인 |  | 1 | `entity/-vehicle/ussr/tank_light/zis30/zis30.def` |
| `/game_entity/("crew_human")` | 예: place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) group ( crew ) (13종 이상) | 미확인 |  | 91 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/("crew_open")` | place ( driver ) group ( crew ) bone ( driver ) door ( emit1 ) animation ( driver ) extra ( ) ¦ place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) | 미확인 |  | 3 | `entity/-vehicle/ussr/cannon/broneschitok_1/broneschitok_1.def` |
| `/game_entity/("crew")` | 예: place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) visor ( driver ) off ( { sensor } ) (13종 이상) | 미확인 |  | 32 | `entity/-vehicle/germany/btr/sdkfz250a_1/sdkfz250a_1.def` |
| `/game_entity/("engine")` | repair ( 30 ) | 미확인 |  | 2 | `entity/-vehicle/germany/special/sdkfz302/sdkfz302.def` |
| `/game_entity/("fuel")` | args "fuel" ¦ args "fuel1" ¦ args "fuel2" ¦ args "transmission" | 미확인 |  | 73 | `entity/-vehicle/eng/car/bedford_oyc_fuel/bedford_oyc_fuel.def` |
| `/game_entity/("general_durability")` | body ( 150 ) engine ( 85 ) turret ( 1 ) mantlet ( 1 ) gun ( 1 ) track ( 100 ) ammo ( 120 ) | 미확인 |  | 2 | `entity/-vehicle/germany/special/sdkfz302/sdkfz302.def` |
| `/game_entity/("glass")` | 예: args "glass1" (13종 이상) | 미확인 |  | 72 | `entity/-vehicle/eng/car/bedford_oyc_fuel/bedford_oyc_fuel.def` |
| `/game_entity/("gun")` | repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ repair ( 30 ) thickness ( 6 ) ¦ repair ( 30 ) thickness ( 2 ) ¦ repair ( 30 ) thickness ( 50 ) ¦ repair ( 30 ) thickness ( 10 ) ¦ repair ( 45 ) thickness ( 50 ) | 미확인 |  | 149 | `entity/-vehicle/eng/cannon/114mm_bl_4_5/114mm_bl_4_5.def` |
| `/game_entity/("gun1")` | repair ( 60 ) thickness ( 120 ) | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/("gun2")` | repair ( 60 ) thickness ( 120 ) | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/("gunner_open")` | place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit4 ) door2 ( emit5 ) anm_left ( gunner2 ) anm_right ( gunner2 ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit3 ) door2 ( emit4 ) anm_left ( gunner2 ) anm_right ( gunner2 ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit5 ) door2 ( emit7 ) anm_left ( left ) anm_right ( right ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit4 ) door2 ( emit7 ) anm_left ( gunner2 ) anm_right ( gunner2 ) group ( passenger ) ¦ place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit3 ) door2 ( emit4 ) anm_left ( left ) anm_right ( right ) group ( passenger ) | 미확인 |  | 27 | `entity/-vehicle/eng/tank_light/stuart1/stuart1.def` |
| `/game_entity/("lamp")` | args "lamp" | 미확인 |  | 2 | `entity/-vehicle/germany/special/150mm_sw34/150mm_sw34.def` |
| `/game_entity/("marine_durability")` | body ( 100 ) engine ( 60 ) gun ( 100 ) | 미확인 |  | 1 | `entity/-vehicle/eng/marine/goatley_boat/goatley_boat.def` |
| `/game_entity/("passenger_human")` | 예: place ( seat1 ) bone ( seat01 ) door ( emit1 ) animation ( passenger1 ) group ( passenger ) (13종 이상) | 미확인 |  | 4 | `entity/-vehicle/eng/airborne/horsa_mk1/horsa_mk1.def` |
| `/game_entity/("passenger")` | 예: place ( seat1 ) bone ( seat01 ) door ( emit1 ) animation ( passenger1 ) off ( ) (13종 이상) | 미확인 |  | 16 | `entity/-vehicle/germany/btr/sdkfz250a_1/sdkfz250a_1.def` |
| `/game_entity/("seat_human")` | 예: place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( armor1 ) (13종 이상) | 미확인 |  | 300 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/("shield_group")` | 예: args "shield1" 5 "shield" (13종 이상) | 미확인 |  | 51 | `entity/-vehicle/eng/cannon/37mm_qf_mk1/37mm_qf_mk1.def` |
| `/game_entity/("shield")` | 예: args "shield" 8 "shield" (13종 이상) | 미확인 |  | 69 | `entity/-vehicle/eng/cannon/114mm_qf_how/114mm_qf_how.def` |
| `/game_entity/("solid_wheel")` | args "wheelright" ¦ args "wheelleft" ¦ args "wheelright1" ¦ args "wheelleft1" ¦ args "wheelright2" ¦ args "wheelleft2" | 미확인 |  | 56 | `entity/-vehicle/eng/cannon/114mm_bl_4_5_mk1/114mm_bl_4_5_mk1.def` |
| `/game_entity/("supply_ammo")` | supply ( 500 ) ¦ supply ( 250 ) | 미확인 |  | 5 | `entity/-vehicle/eng/trailer/ammo_trailer_eng/ammo_trailer_eng.def` |
| `/game_entity/("supply_fuel")` | liters ( 2500 ) ¦ liters ( 2840 ) ¦ liters ( 2050 ) | 미확인 |  | 6 | `entity/-vehicle/eng/car/bedford_oyc_fuel/bedford_oyc_fuel.def` |
| `/game_entity/("tracks")` | thickness ( 20 ) ¦ thickness ( 15 ) ¦ thickness ( 30 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ thickness ( 10 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 15 ) thickness ( 5 ) ¦ repair ( 45 ) thickness ( 30 ) ¦ thickness ( 21 ) | 미확인 |  | 376 | `entity/-vehicle/eng/btr/x/bren_carrier_xx/bren_carrier_boys_xx.def` |
| `/game_entity/("train_durability")` | body ( 500 ) engine ( 250 ) turret ( 300 ) mantlet ( 225 ) gun ( 225 ) | 미확인 |  | 3 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.def` |
| `/game_entity/("transmission")` | args "transmission" | 미확인 |  | 64 | `entity/-vehicle/eng/car/bedford_oyc_fuel/bedford_oyc_fuel.def` |
| `/game_entity/("turret")` | repair ( 45 ) | 미확인 |  | 8 | `entity/-vehicle/eng/car/morris_c9_aa/morris_c9_aa.def` |
| `/game_entity/("wheel")` | 예: args "wheelr" (13종 이상) | 미확인 |  | 191 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/("wing")` | number ( r2 ) component ( right2 ) tag ( wing ) ¦ number ( l2 ) component ( left2 ) tag ( wing ) ¦ number ( rt1 ) component ( right_top ) tag ( wing ) ¦ number ( rt2 ) component ( right2_top ) tag ( wing ) ¦ number ( lt1 ) component ( left_top ) tag ( wing ) ¦ number ( lt2 ) component ( left2_top ) tag ( wing ) | 미확인 |  | 1 | `entity/-vehicle/ussr/airborne/po2/po2.def` |
| `/game_entity/("wood")` | 예: args "bodyback1" (13종 이상) | 미확인 |  | 29 | `entity/-vehicle/eng/car/bedford_oyd_art_ammo/bedford_oyd_art_ammo.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/anm` | 예: "1_gunner" (13종 이상) | 미확인 | gunner ¦ commander | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link` | "emit1" "%place" ¦ "emit2" "%place" ¦ "emit3" "%place" ¦ "driver1" "gunner" ¦ "driver2" "gunner2" ¦ "driver3" "charger" ¦ "driver4" "charger2" ¦ "driver5" "commander" | 미확인 |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link/anm` | "2_%place" ¦ "1_%place" | 미확인 | gunner ¦ commander | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/boarder/link/reverse` | puton | 미확인 |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/placer/place` | "%place" %extra | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/linkbone` | "%place" | 미확인 |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/turnoff` | %off | 미확인. 하위에 sensor |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human_visible_anm")/placer/place/visor` | "%visor" | 미확인 |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/(define "crew_human")/boarder/link` | "emit1" "%place" ¦ "emit2" "%place" ¦ "%door" "%place" | 미확인 |  | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/boarder/link/anm` | "%animation" ¦ "%place" ¦ "left%animation" ¦ "right%animation" | 미확인 | gunner ¦ commander | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/boarder/link/reverse` | puton | 미확인 |  | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/placer/place/linkbone` | "%bone" ¦ "%place" | 미확인 |  | 4 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/placer/place/turnoff` | ¦ %off | 미확인. 하위에 sensor |  | 6 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew_human")/placer/place/visor` | "%visor" | 미확인 |  | 4 | `entity/-vehicle/eng/tank_medium/archer/archer.def` |
| `/game_entity/(define "crew")/boarder/link` | "emit1" "%place" ¦ "emit2" "%place" | 미확인 |  | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "crew")/boarder/link/anm` | "%animation" | 미확인 | gunner ¦ commander | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "crew")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "crew")/boarder/link/reverse` | puton | 미확인 |  | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "crew")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "crew")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "crew")/placer/place/turnoff` | %off | 미확인. 하위에 sensor |  | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/(define "restore_ik_time")/restoreiktime` | 4 | 미확인. 값 4~10 |  | 1 | `entity/-vehicle/germany/cannon/600mm_thor/600mm_thor.def` |
| `/game_entity/##bone` | "commander_rot" | 미확인 |  | 43 | `entity/-vehicle/eng/tank_medium/sherman3_late/sherman3_late.def` |
| `/game_entity/##extender` | "smokescreen" | 미확인 | sources say that these were not installed in the vehicles delivered to the Finns. | 1 | `entity/-vehicle/finland/tank_medium/panzer4j_fin/panzer4j_fin.def` |
| `/game_entity/##sensor/visor` | "main" ¦ "around" | 미확인 |  | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/##sensor/visor/bone` | "" | 미확인 | Boys rot ¦ turret | 1 | `entity/-vehicle/usa/btr/m20/m20.def` |
| `/game_entity/##volume` | "engine3" | 미확인 | Engine Cover | 2 | `entity/-vehicle/finland/tank_medium/x/t28e_fin_xx/t28e_fin_late_xx.def` |
| `/game_entity/autooperatable` | 1 | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/boarder/anm/forward/base` | 예: "open" (13종 이상) | 미확인. 값 "open" 또는 "open" -1 |  | 326 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/boarder/anm/reverse/base` | 예: "open" -1 (13종 이상) | 미확인. 값 "open" 또는 "open" -1 |  | 318 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/boarder/link/forward` | putoff | 미확인. 값 putoff | placeholder pose ¦ 4 | 3 | `entity/-vehicle/generic/train/drg_class75/drg_class75.def` |
| `/game_entity/boarder/link/reverse` | puton | 미확인. 값 puton |  | 3 | `entity/-vehicle/generic/train/drg_class75/drg_class75.def` |
| `/game_entity/bone/("turret_heavy")` | power_traverse ( 6 ) ¦ power_traverse ( 12 ) ¦ power_traverse ( 23.7 ) ¦ power_traverse ( 24 ) ¦ power_traverse ( 16 ) ¦ power_traverse ( 33 ) ¦ power_traverse ( 18 ) ¦ power_traverse ( 0 ) | 미확인 |  | 27 | `entity/-vehicle/eng/tank_medium/challenger_a30/challenger_a30.def` |
| `/game_entity/bone/("turret_light")` | power_traverse ( 0 ) ¦ power_traverse ( 24 ) ¦ power_traverse ( 14 ) ¦ power_traverse ( 18 ) ¦ power_traverse ( 55 ) | 미확인 |  | 94 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/bone/("turret_medium")` | power_traverse ( 24 ) ¦ power_traverse ( 19 ) ¦ power_traverse ( 0 ) ¦ power_traverse ( 25 ) ¦ power_traverse ( 16 ) ¦ power_traverse ( 6 ) ¦ power_traverse ( 20 ) ¦ power_traverse ( 36 ) ¦ power_traverse ( 14 ) | 미확인 |  | 132 | `entity/-vehicle/eng/btr/aec_mk2/aec_mk2.def` |
| `/game_entity/brain/properties/fire_mode` | hold | 미확인 |  | 50 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng_recon/blenheim_mk1_eng_recon.def` |
| `/game_entity/chassis/("car_trace")` | 예: fx ( "tracewheel05" ) step ( 0.07 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) (13종 이상) | 미확인 |  | 305 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/chassis/("flight_model")` | 예: speed ( 375 ) climb_speed ( 6 ) turn ( 200 ) turnMP ( 33 ) aerodynamics ( 1.20 ) weight ( 6000 ) power ( 1840 ) fuel ( 1060 ) range ( 2350 ) roll_rate ( 50 ) roll_angle ( 65 ) (13종 이상) | 미확인 |  | 20 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/chassis/("mobility_btr")` | 예: speed ( 58 ) speedatmaxturn ( 25 ) reverse ( 12 ) weight ( 11.0 ) power ( 105 ) fuel ( 95 ) type ( diesel ) range ( 400 ) (13종 이상) | 미확인 |  | 41 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/chassis/("mobility_cannon")` | 예: speed ( 10.3 ) weight ( 147 ) (13종 이상) | 미확인 |  | 181 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/chassis/("mobility_car")` | 예: speed ( 65 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 6.968 ) power ( 72 ) fuel ( 145 ) type ( fuel ) range ( 450 ) (13종 이상) | 미확인 |  | 108 | `entity/-vehicle/eng/car/bedford_oyc_fuel/bedford_oyc_fuel.def` |
| `/game_entity/chassis/("mobility_goliath")` | speed ( 10 ) reverse ( 7 ) traverse ( 80 ) weight ( 0.370 ) power ( 6.85 ) track ( 10 ) fuel ( 2 ) type ( fuel ) range ( 32.0 ) ¦ speed ( 11.5 ) reverse ( 8 ) traverse ( 80 ) weight ( 0.430 ) power ( 12.5 ) track ( 10 ) fuel ( 6 ) type ( fuel ) range ( 60 ) | 미확인 |  | 2 | `entity/-vehicle/germany/special/sdkfz302/sdkfz302.def` |
| `/game_entity/chassis/("mobility_medium_boat")` | speed ( 17 ) speedatmaxturn ( 8 ) reverse ( 6 ) weight ( 50 ) power ( 400 ) fuel ( 6630 ) type ( fuel ) range ( 547 ) ¦ speed ( 65 ) speedatmaxturn ( 20 ) reverse ( 20 ) weight ( 1380 ) power ( 34000 ) fuel ( 6630 ) type ( fuel ) range ( 8900 ) ¦ speed ( 48 ) speedatmaxturn ( 20 ) reverse ( 20 ) weight ( 50.6 ) power ( 2520 ) fuel ( 6630 ) type ( fuel ) range ( 547 ) | 미확인 |  | 3 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.def` |
| `/game_entity/chassis/("mobility_small_boat")` | speed ( 6 ) speedatmaxturn ( 6 ) reverse ( 6 ) weight ( 0.15 ) power ( 1 ) fuel ( 0.1 ) type ( fuel ) range ( 100000 ) ¦ speed ( 19 ) speedatmaxturn ( 13 ) reverse ( 10 ) weight ( 13.0 ) power ( 120 ) fuel ( 5000 ) type ( fuel ) range ( 500 ) | 미확인 |  | 2 | `entity/-vehicle/eng/marine/goatley_boat/goatley_boat.def` |
| `/game_entity/chassis/("mobility_tank_slow")` | speed ( 10 ) reverse ( 6 ) traverse ( 20 ) weight ( 33.0 ) power ( 425 ) track ( 4 ) fuel ( 606 ) type ( fuel ) range ( 160 ) ¦ speed ( 7.8 ) reverse ( 4 ) traverse ( 20 ) weight ( 6.7 ) power ( 39 ) track ( 25.0 ) fuel ( 95 ) type ( fuel ) range ( 35 ) ¦ speed ( 7.8 ) reverse ( 4 ) traverse ( 20 ) weight ( 6.5 ) power ( 39 ) track ( 25.0 ) fuel ( 95 ) type ( fuel ) range ( 35 ) ¦ speed ( 10 ) reverse ( 5 ) traverse ( 10 ) weight ( 57.26 ) power ( 350 ) track ( 4 ) fuel ( 660 ) type ( fuel ) range ( 100 ) | 미확인 |  | 4 | `entity/-vehicle/eng/tank_medium/sherman5_crab/sherman5_crab.def` |
| `/game_entity/chassis/("mobility_tank")` | 예: speed ( 48 ) reverse ( 20 ) traverse ( 35 ) weight ( 3.55 ) power ( 85 ) track ( 5 ) fuel ( 91 ) type ( fuel ) range ( 150 ) (13종 이상) | 미확인 |  | 312 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/chassis/("mobility_train")` | speed ( 72 ) speedatmaxturn ( 17 ) reverse ( 30 ) weight ( 8.2 ) power ( 105 ) fuel ( 145 ) type ( fuel ) range ( 200 ) | 미확인 |  | 1 | `entity/-vehicle/germany/btr/p204_f_rail/p204_f_rail.def` |
| `/game_entity/chassis/("tank_trace_ex")` | fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.58 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.49 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.29 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.46 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.50 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.82 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.7 ) boneleft ( "wheell4" ) boneright ( "wheelr4" ) ¦ fx ( "tracks_sm2" ) step ( 0.1 ) len ( 0.6 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.8 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( -0.49 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.8 ) boneleft ( "wheell4" ) boneright ( "wheelr4" ) | 미확인 |  | 40 | `entity/-vehicle/germany/btr/panzerwerfer42/panzerwerfer42.def` |
| `/game_entity/chassis/("tank_trace")` | 예: fx ( "tracks_mid" ) step ( 0.32 ) len ( 0.5 ) (13종 이상) | 미확인 |  | 320 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/chassis/switch/terrain` | not "water" "deep_water" ¦ "water" "deep_water" | 미확인 |  | 11 | `entity/-vehicle/eng/tank_light/lvt4_eng/lvt4_eng.def` |
| `/game_entity/chassis/waterline` | 0 ~ 1.6 | 미확인 |  | 18 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.def` |
| `/game_entity/constrain` | none | 미확인. 값은 ground, none 등. 배치 제약으로 보이나 확인하지 못함 |  | 1 | `entity/-vehicle/germany/cannon/mgun_ring/mgun_ring.def` |
| `/game_entity/extender[cannon]/##` | animation | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/extender[cannon]/##/aim` | "" "" | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/extender[cannon]/##/fire` | "" "" | 미확인 | mg_maxim_gunner_fire | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/extender[cannon]/##/move` | "" "" | 미확인 | move state: cannon being pushed and which positions play animations | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/extender[cannon]/##/reload` | "" "" | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/extender[cannon]/##/switch` | "close" | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/305mm_mb_2_12/305mm_mb_2_12.def` |
| `/game_entity/extender[cannon]/animation/gunners` | "" "" "" "" | 미확인 |  | 7 | `entity/-vehicle/eng/cannon/76mm_land_mattress/76mm_land_mattress.def` |
| `/game_entity/extender[cannon]/animation/switch` | "close" | 미확인. 값 "close" |  | 163 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/attack/obstacle` | "open" | 미확인. 값 "open" |  | 166 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/move/obstacle` | "close" | 미확인. 값 "close" |  | 173 | `entity/-vehicle/eng/cannon/107mm_ml4_2/107mm_ml4_2.def` |
| `/game_entity/extender[cannon]/rotate_ik_limit` | 0 ~ 35 | 미확인 |  | 34 | `entity/-vehicle/eng/cannon/20mm_oerlikon/20mm_oerlikon.def` |
| `/game_entity/extender[detail]` | "detail" | 미확인 |  | 3 | `entity/-vehicle/eng/tank_medium/valentine_mk2/valentine_mk2.def` |
| `/game_entity/extender[detail]/detail1` | 0 ~ 1 | 미확인 |  | 3 | `entity/-vehicle/eng/tank_medium/valentine_mk2/valentine_mk2.def` |
| `/game_entity/extender[detail]/random` | 0 | 미확인. ## 블록의 하위. 용도 불명 |  | 2 | `entity/-vehicle/germany/car/kubelwagen/kubelwagen.def` |
| `/game_entity/extender[edifice]` | "edifice" | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/x/305mm_mb_2_12_xx/305mm_mb_2_12_xx.def` |
| `/game_entity/extender[inventory]/box/item/user` | "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | 미확인 |  | 99 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/extender[inventory]/weapon/mask` | "mountable" | 미확인. 값 "mountable" |  | 102 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/extender[land_patch]` | "land_patch" | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/x/305mm_mb_2_12_xx/305mm_mb_2_12_xx.def` |
| `/game_entity/extender[land_patch]/terraintype` | "road" "bridge_steel" | 미확인 |  | 1 | `entity/-vehicle/ussr/cannon/x/305mm_mb_2_12_xx/305mm_mb_2_12_xx.def` |
| `/game_entity/placer/crew` | 0 | 미확인. 값 0 또는 인자 없음 |  | 3 | `entity/-vehicle/germany/special/sdkfz302/sdkfz302.def` |
| `/game_entity/placer/place/handlebone` | "body" ¦ "basis" | 미확인 |  | 13 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/placer/place/turnoff/sensor` | 0 | 미확인. 값 0 또는 인자 없음 |  | 4 | `entity/-vehicle/generic/train/drg_class75/drg_class75.def` |
| `/game_entity/placer/place/turnoff/targetable` | 0 | 미확인 |  | 27 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/placer/place/usevision` | 0 | 미확인 |  | 1 | `entity/-vehicle/germany/car/kettenkrad/kettenkrad.def` |
| `/game_entity/simulation/able` | 0 ~ 1 | 미확인. 값 0 또는 1 |  | 4 | `entity/-vehicle/germany/cannon/panzernest_krab/panzernest_krab.def` |
| `/game_entity/simulation/disablelinked` | 0 | 미확인. 값 1뿐. 용도 불명 |  | 3 | `entity/-vehicle/germany/cannon/panzernest_krab/panzernest_krab.def` |
| `/game_entity/texmod` | "late" ¦ "rus" ¦ "camo" ¦ "camo2" ¦ "grey" ¦ "2" ¦ "early" ¦ "red_ball" | 미확인 |  | 17 | `entity/-vehicle/eng/tank_heavy/churchill_mk3_l50_late/churchill_mk3_l50_late.def` |
| `/game_entity/volume/able/nolook` | 1 | 미확인. able의 하위. 값 1 |  | 3 | `entity/-vehicle/finland/cannon/x/150mm_h40_xx/150mm_h40_xx.def` |
| `/game_entity/volume/able/obstacle` | 0 | 미확인. able의 하위. 값 0 |  | 493 | `entity/-vehicle/eng/airborne/x/blenheim_mk1_eng_x/blenheim_mk1_eng_recon_x.def` |
| `/game_entity/volume/able/visible` | 0 | 미확인. able의 하위. 값 0 |  | 46 | `entity/-vehicle/eng/airborne/horsa_mk1/horsa_mk1.def` |
| `/game_entity/volume/density` | 0 ~ 0.1 | 미확인. 값 0~2. 볼륨 밀도로 보이나 단위와 용도를 확인하지 못함 |  | 7 | `entity/-vehicle/eng/marine/goatley_boat/goatley_boat.def` |
| `/game_entity/volume/quality` | "large_flaw" | 미확인 |  | 8 | `entity/-vehicle/eng/tank_medium/x/achilles_mk2_xx/achilles_mk2_xx.def` |
| `/game_entity/weaponry/##place` | "mgun" ¦ "mgun2" ¦ "bomb1" | 미확인 | maybe future feature | 51 | `entity/-vehicle/eng/airborne/typhoon_mk1b_recon/typhoon_mk1b_recon.def` |
| `/game_entity/weaponry/##place/barrels` | 2 ~ 3 | 미확인 |  | 2 | `entity/-vehicle/eng/airborne/typhoon_mk1b_recon/typhoon_mk1b_recon.def` |
| `/game_entity/weaponry/##place/linkanimation` | "off" | 미확인. 값 "off" |  | 4 | `entity/-vehicle/finland/tank_medium/panzer4j_fin/panzer4j_fin.def` |
| `/game_entity/weaponry/##place/linkbone` | "mgun2" ¦ "mgun_rot" | 미확인 |  | 4 | `entity/-vehicle/finland/tank_medium/panzer4j_fin/panzer4j_fin.def` |
| `/game_entity/weaponry/##place/shooter` | "shooter_bb" | 미확인 |  | 2 | `entity/-vehicle/eng/airborne/typhoon_mk1b_recon/typhoon_mk1b_recon.def` |
| `/game_entity/weaponry/##place/type` | "commander" ¦ "rear" ¦ "aa" | 미확인. 값 "rear" 등 |  | 48 | `entity/-vehicle/eng/tank_medium/sherman3_late/sherman3_late.def` |
| `/game_entity/weaponry/##place/weapon` | "commander_vision" ¦ "bomb_dropper_eng_250x2" filled ¦ "bomb_dropper_usa_250x3" filled ¦ "thrower_v" filling "f1 grenade" 1 | 미확인 | Placeholder weapon ¦ !!! PLACEHOLDER | 47 | `entity/-vehicle/eng/airborne/typhoon_mk1b_recon/typhoon_mk1b_recon.def` |
| `/game_entity/weaponry/place/("abm_dymamic_aa")` | zeroing ( 2.25 ) dispersion ( 0.50 ) ¦ zeroing ( 1.95 ) dispersion ( 0.24 ) ¦ zeroing ( 2.25 ) dispersion ( 0.3 ) ¦ zeroing ( 2.5 ) dispersion ( 0.50 ) | 미확인 |  | 10 | `entity/-vehicle/finland/cannon/75mm_itk37/75mm_itk37.def` |
| `/game_entity/weaponry/place/("abm_dymamic_aaa")` | zeroing ( 1.2 ) dispersion ( 0.15 ) ¦ zeroing ( 1.95 ) dispersion ( 0.24 ) ¦ zeroing ( 2.9 ) dispersion ( 0.08 ) ¦ zeroing ( 2.5 ) dispersion ( 0.15 ) | 미확인 |  | 28 | `entity/-vehicle/eng/cannon/20mm_oerlikon/20mm_oerlikon.def` |
| `/game_entity/weaponry/place/("abm_dymamic_autocannon")` | zeroing ( 2 ) dispersion ( 0.15 ) ¦ zeroing ( 2.0 ) dispersion ( 0.15 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인 |  | 10 | `entity/-vehicle/germany/btr/sdkfz222a/sdkfz222a.def` |
| `/game_entity/weaponry/place/("abm_dymamic_gun_boat")` | zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인 |  | 4 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.def` |
| `/game_entity/weaponry/place/("abm_dymamic_gun_poor")` | zeroing ( 5.0 ) dispersion ( 0.35 ) | 미확인 |  | 1 | `entity/-vehicle/finland/cannon/76mm_k00/76mm_k00.def` |
| `/game_entity/weaponry/place/("abm_dymamic_gun")` | 예: zeroing ( 5.0 ) dispersion ( 1 ) (13종 이상) | 미확인 |  | 120 | `entity/-vehicle/eng/cannon/114mm_bl_4_5/114mm_bl_4_5.def` |
| `/game_entity/weaponry/place/("abm_dymamic_mortar")` | zeroing ( 3.0 ) dispersion ( 1 ) ¦ zeroing ( 2.5 ) dispersion ( 1 ) ¦ zeroing ( 0.0 ) dispersion ( 0 ) ¦ zeroing ( 10.0 ) dispersion ( 1 ) | 미확인 |  | 27 | `entity/-vehicle/eng/btr/mortar_carrier_mk1/mortar_carrier_mk1.def` |
| `/game_entity/weaponry/place/("abm_dymamic_poor")` | zeroing ( 2.0 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) ¦ zeroing ( 4.5 ) dispersion ( 0.20 ) | 미확인 |  | 5 | `entity/-vehicle/germany/tank_light/pz35t/pz35t.def` |
| `/game_entity/weaponry/place/("abm_dymamic_rocket")` | zeroing ( 10.0 ) dispersion ( 1 ) ¦ zeroing ( 5.0 ) dispersion ( 1 ) | 미확인 |  | 18 | `entity/-vehicle/eng/cannon/76mm_land_mattress/76mm_land_mattress.def` |
| `/game_entity/weaponry/place/("abm_dymamic_stabilised")` | zeroing ( 3 ) dispersion ( 0.3 ) ¦ zeroing ( 3.0 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) ¦ zeroing ( 2.8 ) dispersion ( 0.20 ) ¦ zeroing ( 3.1 ) dispersion ( 0.5 ) ¦ zeroing ( 1.75 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.25 ) ¦ zeroing ( 2.0 ) dispersion ( 0.20 ) | 미확인 |  | 74 | `entity/-vehicle/eng/tank_medium/sherman1/sherman1.def` |
| `/game_entity/weaponry/place/("abm_dymamic_td")` | zeroing ( 2.5 ) dispersion ( 0.25 ) ¦ zeroing ( 1.5 ) dispersion ( 0.25 ) ¦ zeroing ( 3.5 ) dispersion ( 0.25 ) ¦ zeroing ( 4.0 ) dispersion ( 0.25 ) ¦ zeroing ( 3.0 ) dispersion ( 0.25 ) ¦ zeroing ( 2.25 ) dispersion ( 0.25 ) ¦ zeroing ( 3.50 ) dispersion ( 0.25 ) ¦ zeroing ( 2.5 ) dispersion ( 0.2 ) ¦ zeroing ( 2.0 ) dispersion ( 0.25 ) | 미확인 |  | 44 | `entity/-vehicle/eng/tank_medium/achilles_mk2/achilles_mk2.def` |
| `/game_entity/weaponry/place/("abm_dymamic")` | 예: zeroing ( 3.0 ) dispersion ( 0.20 ) (13종 이상) | 미확인 |  | 177 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.def` |
| `/game_entity/weaponry/place/barrels` | 1 ~ 60 | 미확인 |  | 72 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/weaponry/place/linkanimation` | "off" | 미확인. 값 "off" |  | 155 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.def` |
| `/game_entity/weaponry/place/shooter` | "shooter" ¦ "shooter_mg" ¦ "shooter_bb" ¦ "shooter_b" ¦ "shooter_r" | 미확인 |  | 21 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |
| `/game_entity/weaponry/place/type` | 예: "rear" (13종 이상) | 미확인. 값 "rear" 등 |  | 414 | `entity/-vehicle/eng/airborne/blenheim_mk1_eng/blenheim_mk1_eng.def` |

## 연관: 차량 전용 속성 (.ext)

- 위치: `entity/-vehicle/**/*.ext`
- 연결: 본체의 `(include "이름.ext")`

### 뜻을 모르는 속성 (180개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/("ammo")` | 예: args "ammo1" (13종 이상) | 미확인 |  | 313 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/("armor")` | 예: args "armor1" 4 (13종 이상) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.ext` |
| `/("aux_turret")` | repair ( 45 ) ¦ repair ( 30 ) | 미확인 |  | 18 | `entity/-vehicle/eng/tank_light/cruiser_mk1/cruiser_mk1.ext` |
| `/("body")` | repair ( 30 ) thickness ( 4 ) ¦ repair ( 30 ) | 미확인 |  | 2 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.ext` |
| `/("cabin")` | repair ( 30 ) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.ext` |
| `/("component")` | 예: volume ( "body_front1" ) component ( "body_front" ) repair ( 10 ) (13종 이상) | 미확인 |  | 3 | `entity/-vehicle/eng/marine/hms_boadicea/hms_boadicea.ext` |
| `/("crew_commander")` | place ( commander ) bone ( commander ) visor ( commander ) off ( { sensor } ) | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/("crew_human_visible_anm")` | place ( gunner ) bone ( gunner ) visor ( main ) ¦ place ( driver ) linkbone ( ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) visor ( driver ) pose ( gunner ) off ( { sensor } ) ¦ place ( gunner ) linkbone ( gunner ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( gunner ) visor ( main ) off ( { sensor } ) ¦ place ( charger ) linkbone ( charger ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( charger ) visor ( none ) off ( { sensor } ) ¦ place ( charger2 ) linkbone ( charger2 ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( charger ) visor ( none ) off ( ) ¦ place ( commander ) linkbone ( commander ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) pose ( commander ) visor ( none ) off ( ) | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/("crew_human_visible_anm2")` | place ( driver ) door ( emit1 ) door2 ( emit2 ) anm_left ( left_driver ) anm_right ( right_driver ) ¦ place ( gunner ) door ( emit1 ) door2 ( emit2 ) anm_left ( left_gunner ) anm_right ( right_gunner ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) anm_left ( left_commander ) anm_right ( right_commander ) ¦ place ( driver ) door ( emit1 ) door2 ( emit1 ) anm_left ( left ) anm_right ( left ) ¦ place ( commander ) door ( emit1 ) door2 ( emit1 ) anm_left ( right ) anm_right ( right ) | 미확인 |  | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/("crew_human")` | 예: place ( driver ) visor ( driver ) (13종 이상) | 미확인 |  | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/("crew_human")/off()/targetable` | 0 | 미확인 |  | 9 | `entity/-vehicle/eng/btr/morris_cs9/crew.ext` |
| `/("crew_open")` | place ( commander ) visor ( top ) | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/("crew")` | 예: place ( driver ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) door4 ( emit4 ) door5 ( emit5 ) door6 ( emit6 ) visor ( main ) anm ( driver ) (13종 이상) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/("engine")` | repair ( 30 ) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.ext` |
| `/("fuel")` | args "fuel1" ¦ args "fuel2" ¦ args "fuel" ¦ args "fuel3" ¦ args "fuel4" | 미확인 |  | 386 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/("glass")` | 예: args "window1" (13종 이상) | 미확인 |  | 22 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.ext` |
| `/("gun_shield")` | 예: args "tshield1" 20 (13종 이상) | 미확인 |  | 11 | `entity/-vehicle/eng/tank_medium/archer/archer.ext` |
| `/("gun")` | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ repair ( 30 ) thickness ( 8 ) ¦ repair ( 30 ) thickness ( 6 ) ¦ repair ( 60 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 10 ) ¦ repair ( 30 ) thickness ( 35 ) ¦ repair ( 30 ) thickness ( 80 ) ¦ repair ( 30 ) thickness ( 2.5 ) | 미확인 |  | 310 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/("gun1")` | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ repair ( 45 ) thickness ( 20 ) | 미확인 |  | 11 | `entity/-vehicle/eng/marine/hms_boadicea/hms_boadicea.ext` |
| `/("gun2")` | repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 25 ) | 미확인 |  | 4 | `entity/-vehicle/eng/marine/hms_boadicea/hms_boadicea.ext` |
| `/("gun3")` | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 8 ) | 미확인 |  | 2 | `entity/-vehicle/eng/marine/hms_boadicea/hms_boadicea.ext` |
| `/("gun4")` | repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 8 ) | 미확인 |  | 2 | `entity/-vehicle/eng/marine/hms_boadicea/hms_boadicea.ext` |
| `/("gun5")` | repair ( 30 ) thickness ( 15 ) | 미확인 |  | 1 | `entity/-vehicle/eng/marine/hms_boadicea/hms_boadicea.ext` |
| `/("gun6")` | repair ( 30 ) thickness ( 15 ) | 미확인 |  | 1 | `entity/-vehicle/eng/marine/hms_boadicea/hms_boadicea.ext` |
| `/("lamp")` | args "lamp" | 미확인 |  | 2 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.ext` |
| `/("make_component")` | args "aux_turret1" "aux_turret1" ¦ args "aux_turret11" "aux_turret1" ¦ args "aux_turret12" "aux_turret1" ¦ args "aux_turret21" "aux_turret2" ¦ args "aux_turret22" "aux_turret2" ¦ args "aux_turret2" "aux_turret2" ¦ args "aux_turret23" "aux_turret2" ¦ args "aux_turret13" "aux_turret1" ¦ args "aux_turret31" "aux_turret3" ¦ args "aux_turret41" "aux_turret4" ¦ args "aux_turret32" "aux_turret3" ¦ args "aux_turret42" "aux_turret4" | 미확인 |  | 14 | `entity/-vehicle/eng/tank_light/cruiser_mk1/cruiser_mk1.ext` |
| `/("mantlet")` | repair ( 30 ) | 미확인 |  | 7 | `entity/-vehicle/finland/btr/ba10_fin/ba10_fin.ext` |
| `/("marine_durability")` | body ( 600 ) engine ( 450 ) gun ( 150 ) ¦ body ( 600 ) engine ( 300 ) gun ( 150 ) ¦ body ( 600 ) engine ( 450 ) gun ( 1 ) | 미확인 |  | 4 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.ext` |
| `/("passenger")` | 예: place ( seat1 ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) door4 ( emit4 ) door5 ( emit5 ) door6 ( emit6 ) anm ( seat1 ) (13종 이상) | 미확인 |  | 4 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/("rocket")` | args "rocket00" ¦ args "rocket01" ¦ args "rocket02" ¦ args "rocket03" ¦ args "shell" ¦ args "rocket04" ¦ args "rocket05" | 미확인 |  | 3 | `entity/-vehicle/eng/tank_heavy/churchill_mk4_avre/churchill_mk4_avre.ext` |
| `/("shield_group")` | 예: args "shield1" 19 "body" (13종 이상) | 미확인 |  | 25 | `entity/-vehicle/eng/marine/lca/lca.ext` |
| `/("shield_part")` | 예: args "shield3" 105 "turret" (13종 이상) | 미확인 |  | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40.ext` |
| `/("shield_tear")` | 예: args "shield3" 7 (13종 이상) | 미확인 |  | 30 | `entity/-vehicle/eng/tank_heavy/churchill_mk1/churchill_mk1.ext` |
| `/("shield_track")` | 예: args "detail2" 30 (13종 이상) | 미확인 |  | 113 | `entity/-vehicle/eng/tank_heavy/churchill_mk1/churchill_mk1.ext` |
| `/("shield")` | 예: args "shield1" 4 (13종 이상) | 미확인 |  | 166 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.ext` |
| `/("solid_wheel")` | args "wheelright1" ¦ args "wheelleft1" ¦ args "wheelright2" ¦ args "wheelleft2" ¦ args "wheelright3" ¦ args "wheelleft3" ¦ args "wheelleft4" ¦ args "wheelright4" | 미확인 |  | 21 | `entity/-vehicle/finland/btr/ba10_fin/ba10_fin.ext` |
| `/("tracks")` | repair ( 30 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 15 ) ¦ repair ( 40 ) thickness ( 30 ) ¦ repair ( 40 ) thickness ( 20 ) ¦ repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 10 ) ¦ repair ( 30 ) thickness ( 25 ) | 미확인 |  | 340 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.ext` |
| `/("train_durability")` | body ( 750 ) engine ( 250 ) turret ( 300 ) mantlet ( 225 ) gun ( 225 ) | 미확인 |  | 1 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.ext` |
| `/("transmission")` | args "transmission" ¦ args "transmission1" ¦ args "transmission2" | 미확인 |  | 387 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/("turret")` | repair ( 45 ) ¦ repair ( 50 ) ¦ repair ( 30 ) thickness ( 10 ) | 미확인 |  | 44 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/("wheel")` | 예: args "wheelleft1" (13종 이상) | 미확인 |  | 60 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/(define "crew_commander")/boarder/door` | "emit1" ¦ "emit2" | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/boarder/link` | "emit1" "%place" ¦ "emit2" "%place" | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/boarder/link/anm` | "commander_l" ¦ "commander_r" | 미확인 | gunner ¦ commander | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/boarder/link/reverse` | puton | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/placer/place/group` | "crew" | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/placer/place/turnoff` | %off | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_commander")/placer/place/visor` | "%visor" | 미확인 |  | 1 | `entity/-vehicle/germany/btr/kfz14/crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/anm` | "1_gunner" ¦ "2_gunner" ¦ "3_gunner" ¦ "1_charger" ¦ "2_charger" ¦ "3_charger" ¦ "1_commander" ¦ "2_commander" ¦ "3_commander" | 미확인 | gunner ¦ commander | 1 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.ext` |
| `/(define "crew_human_visible_anm")/boarder/anm/forward/base` | "doorleft1" ¦ "doorright2" ¦ "doorright1" | 미확인 |  | 1 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.ext` |
| `/(define "crew_human_visible_anm")/boarder/anm/forward/begin` | "board_tank_right" ¦ "board_kv" | 미확인 |  | 1 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.ext` |
| `/(define "crew_human_visible_anm")/boarder/anm/forward/end` | "pose_m5_gunlayer" ¦ "seat_tm1_charger" ¦ "pose_watch_binocular" | 미확인 |  | 1 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.ext` |
| `/(define "crew_human_visible_anm")/boarder/anm/reverse/base` | "doorleft1" -1 ¦ "doorright2" -1 ¦ "doorright1" -1 | 미확인 |  | 1 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.ext` |
| `/(define "crew_human_visible_anm")/boarder/anm/reverse/end` | "emit_tank_1_hold" | 미확인 |  | 1 | `entity/-vehicle/generic/train/180mm_tm1/180mm_tm1.ext` |
| `/(define "crew_human_visible_anm")/boarder/door` | "emit1" ¦ "emit2" ¦ "%door" ¦ "%door2" ¦ "%door3" | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link` | "emit1" "%place" ¦ "emit2" "%place" ¦ "%door" "%place" ¦ "%door2" "%place" ¦ "%door3" "%place" | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link/anm` | "left" ¦ "right" ¦ "1_%pose" ¦ "2_%pose" ¦ "3_%pose" | 미확인 | gunner ¦ commander | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link/reverse` | puton | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place/group` | "crew" | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place/linkbone` | "%place" ¦ "%linkbone" | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place/turnoff` | ¦ %off | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place/visor` | "%visor" | 미확인 |  | 2 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 |  | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link/anm` | "%anm_left" ¦ "%anm_right" | 미확인 | gunner ¦ commander | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link/reverse` | puton | 미확인 |  | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human_visible_anm2")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human_visible_anm2")/placer/place/group` | "crew" | 미확인 |  | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human_visible_anm2")/placer/place/linkbone` | "%place" | 미확인 |  | 3 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/(define "crew_human")/boarder/door` | "emit1" ¦ "%door" ¦ "emit2" ¦ "%door2" | 미확인 |  | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_human")/boarder/link` | "emit1" "%place" ¦ "%door" "%place" ¦ "emit2" "%place" ¦ "%door2" "%place" | 미확인 |  | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_human")/boarder/link/anm` | "left" ¦ "right" ¦ "%animation" ¦ "%place" ¦ "left%animation" ¦ "right%animation" ¦ "%anm_left" ¦ "%anm_right" | 미확인 | gunner ¦ commander | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_human")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_human")/boarder/link/reverse` | puton | 미확인 |  | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_human")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_human")/placer/place/group` | "crew" ¦ "%group" | 미확인 |  | 56 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_human")/placer/place/linkbone` | "%bone" ¦ "%place" | 미확인 |  | 31 | `entity/-vehicle/eng/btr/morris_cs9/crew.ext` |
| `/(define "crew_human")/placer/place/turnoff` | ¦ %off | 미확인 |  | 31 | `entity/-vehicle/eng/btr/morris_cs9/crew.ext` |
| `/(define "crew_human")/placer/place/visor` | "%visor" | 미확인 |  | 34 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/(define "crew_open")/boarder/door` | "emit1" ¦ "emit2" | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/boarder/link` | "emit1" "%place" ¦ "emit2" "%place" | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/boarder/link/anm` | "left" ¦ "right" | 미확인 | gunner ¦ commander | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/boarder/link/reverse` | puton | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/placer/place/group` | "crew" | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/placer/place/linkbone` | "gunner2" | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/placer/place/visor` | "%visor" | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/sensor/visor` | "top" | 미확인 |  | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/sensor/visor/bone` | "vision_rot" | 미확인 | Boys rot ¦ turret | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew_open")/sensor/visor/vision` | "recon_open" | 미확인 | better vision than a in-closed turret | 1 | `entity/-vehicle/ussr/btr/ba20_command/crew.ext` |
| `/(define "crew")/boarder/door` | 예: "emit1" (13종 이상) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/boarder/link` | 예: "emit1" "%place" (13종 이상) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/boarder/link/anm` | "%anm" | 미확인 | gunner ¦ commander | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/boarder/link/reverse` | puton | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/placer/place/group` | "crew" | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/placer/place/linkbone` | "%place" | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "crew")/placer/place/visor` | "%visor" | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "define_place_jump")/boarder/door` | "emit_l1" | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/boarder/link` | "emit_l1" "%i" | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/boarder/link/anm` | "[fsm]" | 미확인 | gunner ¦ commander | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/boarder/link/reverse` | puton | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/boarder/node` | "node_l1" ¦ "node_l2" | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/placer/place` | "%i" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/placer/place/linkbone` | "attach%i" | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/placer/place/obstacle` | disable | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/volume` | "place%i" | 미확인 | Hull ¦ Superstructure | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/volume/able` | clear | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/volume/able/touch` | 1 | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place_jump")/volume/tags` | "attach" "attach_%i" | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place")/placer/place` | "attach%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place")/placer/place/linkbone` | "%place" | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "define_place")/placer/place/obstacle` | disable | 미확인 |  | 1 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/(define "passenger")/boarder/door` | 예: "emit1" (13종 이상) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "passenger")/boarder/link` | 예: "emit1" "%place" (13종 이상) | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "passenger")/boarder/link/anm` | "%anm" ¦ "passenger" | 미확인 | gunner ¦ commander | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "passenger")/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "passenger")/boarder/link/reverse` | puton | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "passenger")/placer/place` | "%place" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "passenger")/placer/place/group` | "passenger" | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/(define "passenger")/placer/place/linkbone` | "%place" | 미확인 |  | 5 | `entity/-vehicle/eng/marine/drifter_eng/crew.ext` |
| `/##sensor/visor` | "around" ¦ "human" | 미확인 |  | 2 | `entity/-vehicle/germany/btr/sdkfz10/crew.ext` |
| `/##sensor/visor/bone` | "foresight14" | 미확인 | Boys rot ¦ turret | 1 | `entity/-vehicle/germany/btr/sdkfz7/crew.ext` |
| `/##sensor/visor/vision` | "tank_around" | 미확인 | better vision than a in-closed turret | 2 | `entity/-vehicle/germany/btr/sdkfz10/crew.ext` |
| `/##volume` | "engine3" | 미확인 | Engine Cover | 5 | `entity/-vehicle/finland/tank_medium/t28_38_fin/t28_38_fin.ext` |
| `/##volume/thickness` | 10 | 미확인 | 15 + 4 ¦ 89 + 89 | 5 | `entity/-vehicle/finland/tank_medium/t28_38_fin/t28_38_fin.ext` |
| `/boarder/anm` | 예: "left" (13종 이상) | 미확인 | gunner ¦ commander | 66 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/boarder/anm/forward/base` | "doorleft" ¦ "doorright" ¦ "board" ¦ "open_left" ¦ "open_right" ¦ "open_driver" ¦ "open" ¦ "" ¦ "doorright" 1 ¦ "doorleft" 1 ¦ "open_gunner" ¦ "open_commander" | 미확인 |  | 44 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/boarder/anm/forward/begin` | 예: "board_kv" (13종 이상) | 미확인 |  | 65 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/boarder/anm/forward/end` | 예: "pose_daimler_dingo_driver" (13종 이상) | 미확인 |  | 43 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/boarder/anm/reverse/base` | 예: "open_left" -1 (13종 이상) | 미확인 |  | 46 | `entity/-vehicle/eng/btr/daimler_dingo/crew.ext` |
| `/boarder/anm/reverse/begin` | "emit_Bk1124" | 미확인 |  | 1 | `entity/-vehicle/placeholder/generic/marine/ya5_k/crew.ext` |
| `/boarder/anm/reverse/end` | "emit_sd222_driver" ¦ "emit_body" 1 ¦ "emit_torpedo_boat" ¦ "emit_tank_1" ¦ "emit_tank_1_hold" ¦ "emit_seat_nkl_passenger" ¦ "lca_ty_pass_exit_1" ¦ "lca_ty_pass_exit_2" ¦ "board_lowarmor" -1 ¦ "goatley_crew_l_emit" ¦ "goatley_crew_r_emit" ¦ "board_rubber_boat" -1 | 미확인 |  | 64 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/boarder/door` | "emit1" ¦ "emit2" ¦ "emit01" ¦ "emit02" | 미확인 |  | 2 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/boarder/link` | 예: "emit1" "driver" (13종 이상) | 미확인 |  | 2 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/boarder/link/anm` | "seat" ¦ "seat1" ¦ "seat2" ¦ "seat3" ¦ "driver" ¦ "gunner" | 미확인 | gunner ¦ commander | 2 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/boarder/link/forward` | putoff | 미확인 | placeholder pose ¦ 4 | 2 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/boarder/link/reverse` | puton | 미확인 |  | 2 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/component` | 예: "shield1" (13종 이상) | 미확인 | Bucket | 123 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.ext` |
| `/component/tags` | "cut_part" | 미확인 |  | 120 | `entity/-vehicle/eng/btr/bren_carrier/bren_carrier.ext` |
| `/extender[shield]` | "shield" | 미확인 |  | 6 | `entity/-vehicle/eng/tank_light/lvt4_eng/lvt4_eng.ext` |
| `/perks/component` | "shield1" ¦ "shield2" ¦ "shield3" ¦ "turret_armor1" ¦ "turret_armor2" ¦ "turret_armor3" ¦ "turret_armor4" ¦ "turret_armor5" | 미확인 | Bucket | 5 | `entity/-vehicle/eng/tank_medium/sherman1/sherman1.ext` |
| `/perks/component/armor_damage_modifier` | 0 | 미확인 |  | 5 | `entity/-vehicle/eng/tank_medium/sherman1/sherman1.ext` |
| `/perks/component/health/hp` | 300 | 미확인 |  | 5 | `entity/-vehicle/eng/tank_medium/sherman1/sherman1.ext` |
| `/placer/place` | 예: "driver" (13종 이상) | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 22 | `entity/-vehicle/eng/btr/morris_cs9/crew.ext` |
| `/placer/place/linkbone` | 예: "driver" (13종 이상) | 미확인 |  | 2 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/placer/place/obstacle` | disable | 미확인 |  | 2 | `entity/-vehicle/eng/marine/lca/crew.ext` |
| `/placer/place/turnoff/targetable` | 0 | 미확인 |  | 6 | `entity/-vehicle/germany/btr/sdkfz10_4/crew.ext` |
| `/placer/place/visor` | "none" ¦ "main" ¦ "gun" ¦ "around" ¦ "driver" ¦ "human" | 미확인 |  | 19 | `entity/-vehicle/eng/btr/morris_cs9/crew.ext` |
| `/sensor/visor` | 예: "main" (13종 이상) | 미확인 |  | 33 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/sensor/visor/bone` | "foresight1" ¦ "visor1" ¦ "visor2" ¦ "foresight1a" ¦ "foresight5" ¦ "foresight1b" ¦ "foresight1c" ¦ "foresight4" ¦ "visor" ¦ "vision_rot" ¦ "foresight6" | 미확인 | Boys rot ¦ turret | 20 | `entity/-vehicle/eng/btr/staghound_mkaa/crew.ext` |
| `/sensor/visor/vision` | 예: "recon_main" (13종 이상) | 미확인 | better vision than a in-closed turret | 33 | `entity/-vehicle/eng/btr/aec_mk1/crew.ext` |
| `/volume` | 예: "transmission" (13종 이상) | 미확인 | Hull ¦ Superstructure | 398 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/able` | ¦ clear | 미확인 |  | 251 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/able/contact` | 0 ~ 1 | 미확인 |  | 15 | `entity/-vehicle/eng/btr/wasp_mk2c/wasp_mk2c.ext` |
| `/volume/able/contact_ground` | 0 | 미확인 |  | 32 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.ext` |
| `/volume/able/nowounddecal` | 0 | 미확인 |  | 65 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/able/obstacle` | 0 | 미확인 |  | 21 | `entity/-vehicle/eng/tank_heavy/churchill_mk1/churchill_mk1.ext` |
| `/volume/able/visible` | 0 | 미확인 |  | 6 | `entity/-vehicle/finland/tank_light/ft17_fin/ft17_fin.ext` |
| `/volume/caststeel` | 0.3 ~ 0.88 | 미확인 | effective armor reduction compared to solid plate ¦ effective armor reduction compared to solid plate; 25 + 75 spaced armor array ~86mm effective | 174 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/component` | "turret" ¦ "body" ¦ "gun" | 미확인 | Bucket | 131 | `entity/-vehicle/eng/btr/staghound_mk1/staghound_mk1.ext` |
| `/volume/material` | "wood" ¦ "fabric" ¦ "sand" ¦ "concrete" | 미확인 |  | 17 | `entity/-vehicle/eng/marine/drifter_eng/drifter_eng.ext` |
| `/volume/quality` | "large_flaw" | 미확인 |  | 5 | `entity/-vehicle/germany/tank_heavy/jagdtiger/jagdtiger.ext` |
| `/volume/tags` | 예: "mover" "track" (13종 이상) | 미확인 |  | 204 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.ext` |
| `/volume/thickness` | 0.5 ~ 250 | 미확인 | 15 + 4 ¦ 89 + 89 | 398 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/thickness/bottom` | 1 ~ 200 | 미확인 | Overhang (above tracks) ¦ 4mm + .25" | 332 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/thickness/front` | 3 ~ 250 | 미확인 | Glacis ¦ Lower front | 339 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/thickness/left` | 5 ~ 95.25 | 미확인 | Top sides ¦ side | 96 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.ext` |
| `/volume/thickness/rear` | 1 ~ 89 | 미확인 | Front Roof Turret Armor ¦ Bottom Front Nose | 322 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/thickness/right` | 5 ~ 600 | 미확인 | armor plate sides ¦ Top sides | 89 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |
| `/volume/thickness/top` | 1 ~ 200 | 미확인 | Glacis ¦ front upper part | 360 | `entity/-vehicle/eng/btr/aec_mk1/aec_mk1.ext` |

## 연관: 차량 전용 속성 (.inc)

- 위치: `entity/-vehicle/**/*.inc`
- 연결: 본체나 다른 차량의 `(include "../이름/이름.inc")`. 파생형이 기본형의 값을 물려받을 때 쓴다

### 뜻을 아는 속성 (1개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/extender[inventory]/box/item` | 예: "sandbag_vehicle" 1 (13종 이상) | **추정** 실려 있는 아이템과 수량 (추정: /game_entity 아래의 같은 자리와 값 형태가 같음) | 600 rounds ¦ 50 rounds | 14 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |

### 뜻을 모르는 속성 (112개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/("board_animation")` | place ( driver ) board_begin ( board_big-midcar_driver ) board_end ( seat_driver_car ) base ( doorleft ) ¦ place ( commander ) board_begin ( board_big-midcar ) board_end ( seat_passenger ) base ( doorright ) ¦ place ( driver ) board_begin ( board_big-midcar ) board_end ( seat_driver_car ) base ( doorright ) ¦ place ( commander ) board_begin ( board_big-midcar_driver ) board_end ( seat_passenger ) base ( doorleft ) ¦ place ( driver ) board_begin ( board_SdKfz251-1 ) board_end ( pose_qld_driver ) base ( doorright ) ¦ place ( commander ) board_begin ( board_SdKfz251-1 ) board_end ( pose_qld_commander ) base ( doorleft ) | 미확인 |  | 4 | `entity/-vehicle/eng/car/bedford_oyd/bedford_oyd.inc` |
| `/("bodyback")` | repair ( 30 ) | 미확인 |  | 4 | `entity/-vehicle/eng/car/bedford_oyd/bedford_oyd.inc` |
| `/("car_thickness")` | body ( 1.5 ) engine ( 1.5 ) | 미확인 |  | 1 | `entity/-vehicle/usa/car/willys_mb/willys_mg.inc` |
| `/("crew_co_driver")` | place ( commander ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인 |  | 4 | `entity/-vehicle/usa/tank_medium/m10_gmc/m10_gmc.inc` |
| `/("crew_driver")` | door ( emit0 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인 |  | 4 | `entity/-vehicle/usa/tank_medium/m10_gmc/m10_gmc.inc` |
| `/("crew_human_2_v")` | place ( gunner ) bone ( gunner ) door ( emit3 ) door ( emit4 ) | 미확인 |  | 1 | `entity/-vehicle/usa/car/willys_mb/willys_mg.inc` |
| `/("crew_human_v")` | place ( commander ) bone ( commander ) door ( emit2 ) animation ( commander ) group ( crew ) | 미확인 |  | 2 | `entity/-vehicle/usa/car/willys_mb/willys_mg.inc` |
| `/("crew_human_visible_anm")` | place ( gunner ) door ( emit2 ) door2 ( emit3 ) visor ( main ) anm_left ( gunner_l ) anm_right ( gunner_r ) ¦ place ( charger ) door ( emit2 ) door2 ( emit3 ) visor ( around ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger ) door ( emit2 ) door2 ( emit3 ) visor ( none ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger2 ) door ( emit2 ) door2 ( emit3 ) visor ( around ) anm_left ( charger2_l ) anm_right ( charger2_r ) ¦ place ( gunner2 ) door ( emit2 ) door2 ( emit3 ) visor ( main ) anm_left ( gunner2_l ) anm_right ( gunner2_r ) ¦ place ( charger ) door ( emit4 ) door2 ( emit5 ) visor ( around ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger2 ) door ( emit4 ) door2 ( emit5 ) visor ( around ) anm_left ( charger2_l ) anm_right ( charger2_r ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) anm_left ( driver_l ) anm_right ( driver_r ) ¦ place ( gunner ) door ( emit3 ) door2 ( emit4 ) visor ( main ) anm_left ( gunner_l ) anm_right ( gunner_r ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) anm_left ( commander_l ) anm_right ( commander_r ) | 미확인 |  | 5 | `entity/-vehicle/usa/tank_medium/m10_gmc/m10_gmc.inc` |
| `/("crew_human_visible_anm2")` | place ( charger2 ) door ( emit2 ) door2 ( emit3 ) anm_left ( charger2_l ) anm_right ( charger2_r ) ¦ place ( commander ) door ( emit2 ) door2 ( emit3 ) anm_left ( commander_l ) anm_right ( commander_r ) ¦ place ( gunner2 ) door ( emit1 ) door2 ( emit2 ) visor ( ) anm_left ( gunner2_l ) anm_right ( gunner2_r ) ¦ place ( charger ) door ( emit3 ) door2 ( emit4 ) visor ( ) anm_left ( charger_l ) anm_right ( charger_r ) ¦ place ( charger2 ) door ( emit3 ) door2 ( emit4 ) visor ( ) anm_left ( charger2_l ) anm_right ( charger2_r ) | 미확인 |  | 3 | `entity/-vehicle/usa/tank_medium/m36_gmc/m36_gmc.inc` |
| `/("crew_human")` | 예: place ( seat1 ) bone ( seat01 ) door ( emit2 ) animation ( commander ) group ( passenger ) (13종 이상) | 미확인 |  | 11 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/("crew")` | place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) visor ( driver ) off ( { sensor } ) ¦ place ( gunner ) bone ( gunner ) door ( emit3 ) animation ( gunner ) visor ( none ) off ( ) ¦ place ( gunner2 ) bone ( gunner2 ) door ( emit4 ) animation ( gunner2 ) visor ( none ) off ( ) ¦ place ( gunner3 ) bone ( gunner3 ) door ( emit3 ) animation ( gunner2 ) visor ( none ) off ( ) ¦ place ( gunner ) bone ( gunner ) door ( emit2 ) animation ( gunner ) visor ( none ) off ( ) | 미확인 |  | 5 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/("fuel")` | args "fuel" ¦ args "fuel2" ¦ args "fuel1" | 미확인 |  | 5 | `entity/-vehicle/eng/car/bedford_oyd/bedford_oyd.inc` |
| `/("glass")` | args "window1" ¦ args "window2" ¦ args "window3" ¦ args "window4" ¦ args "glass1" ¦ args "glass2" ¦ args "glass3" ¦ args "glass4" ¦ args "glass5" | 미확인 |  | 5 | `entity/-vehicle/eng/car/bedford_oyd/bedford_oyd.inc` |
| `/("gun")` | repair ( 30 ) thickness ( 30 ) ¦ repair ( 30 ) thickness ( 25 ) ¦ repair ( 30 ) thickness ( 20 ) | 미확인 |  | 20 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/("gunner_open")` | place ( gunner2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit5 ) door2 ( emit7 ) anm_left ( left ) anm_right ( right ) group ( passenger ) | 미확인 |  | 1 | `entity/-vehicle/usa/tank_medium/m10_gmc/m10_gmc.inc` |
| `/("seat_human")` | 예: place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( armor1 ) (13종 이상) | 미확인 |  | 11 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/("shield_group")` | 예: args "shield1" 5 "shield" (13종 이상) | 미확인 |  | 10 | `entity/-vehicle/eng/cannon/40mm_qf2/40mm_qf2.inc` |
| `/("shield")` | args "shield_lower" 8 ¦ args "shield_lower" 4 ¦ args "shield" 8 "shield" ¦ args "shield" 4 ¦ args "shield1" 4 ¦ args "shield_lower" 8 "shield" ¦ args "shield_upper" 8 "shield" ¦ args "shield" 5 "shield" ¦ args "upper_shield" 8 "shield" ¦ args "shield_lower" 4.5 ¦ args "shield2" 4 | 미확인 |  | 10 | `entity/-vehicle/eng/cannon/57mm_qf6/57mm_qf6_mk2.inc` |
| `/("solid_wheel")` | args "wheelright" ¦ args "wheelleft" | 미확인 |  | 4 | `entity/-vehicle/finland/cannon/114mm_h18_how/114mm_h18_how.inc` |
| `/("transmission")` | args "transmission" | 미확인 |  | 5 | `entity/-vehicle/eng/car/bedford_oyd/bedford_oyd.inc` |
| `/("wheel")` | args "wheelright" ¦ args "wheelleft" ¦ args "wheelright1" ¦ args "wheelright2" ¦ args "wheelleft1" ¦ args "wheelleft2" ¦ args "wheelright3" ¦ args "wheelleft3" | 미확인 |  | 20 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/("wood")` | args "bodyback1" ¦ args "bodyback2" ¦ args "bodyback3" ¦ args "bodyback4" ¦ args "bodyback5" ¦ args "bodyback6" ¦ args "bodyback7" | 미확인 |  | 1 | `entity/-vehicle/eng/car/bedford_oyd/bedford_oyd.inc` |
| `/##bone` | "commander_rot" | 미확인 |  | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/##bone/speed2` | 180 | 미확인 | manual handcrack, extremely slow ¦ remote controlled from inside | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/able` | "emit_on_attack" ¦ "emit_passangers_on_enemy" ¦ "emit_passengers_on_enemy" | 미확인 |  | 11 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/boarder/anm` | 예: "driver" (13종 이상) | 미확인 | gunner ¦ commander | 45 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/boarder/anm/forward/base` | "board" ¦ "open_driver" ¦ "doorleft" ¦ "doorright" ¦ "open_co_driver" | 미확인 |  | 19 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/boarder/anm/forward/begin` | 예: "board_big-midcar_driver" (13종 이상) | 미확인 |  | 22 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/boarder/anm/forward/end` | 예: "seat_kubel_driver" (13종 이상) | 미확인 |  | 45 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/boarder/anm/reverse/base` | "board" ¦ "doorleft" -1 ¦ "doorright" -1 ¦ "open_driver" ¦ "open_co_driver" ¦ "open_driver" -1 ¦ "close_driver" | 미확인 |  | 17 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/boarder/anm/reverse/end` | "emit_tank_1_hold" ¦ "emit_tank_2_hold" ¦ "emit_body" 1 ¦ "emit_tank_1" ¦ "emit_tank_1_hold" 1 ¦ "emit_tank_2" ¦ "emit_body" ¦ "board_big-midcar_driver" -1 ¦ "board_big-midcar" -1 ¦ "board_t26_driver" -1 ¦ "emit_t34_hold" | 미확인 |  | 22 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/bone` | 예: "mgun_h_rot1" (13종 이상) | 미확인 | Boys rot ¦ turret | 40 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/bone/("turret_heavy")` | power_traverse ( 6 ) ¦ power_traverse ( 24 ) ¦ power_traverse ( 16 ) | 미확인 |  | 5 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/bone/("turret_light")` | power_traverse ( 0 ) | 미확인 |  | 4 | `entity/-vehicle/finland/btr/ba10_fin/ba10_fin.inc` |
| `/bone/("turret_medium")` | power_traverse ( 19 ) | 미확인 |  | 1 | `entity/-vehicle/finland/tank_medium/t3476_41_fin/t3476_41_fin.inc` |
| `/bone/limits` | 예: -5 45 (13종 이상) | 미확인 | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° | 36 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/bone/speed2` | 1.75 ~ 60 | 미확인 | manual handcrack, extremely slow ¦ remote controlled from inside | 40 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/brain/properties/fire_mode` | hold | 미확인 |  | 4 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/chassis` | "track" ¦  ¦ "wheel" | 미확인 | cannot move. this Maxim version is for fixed position use only. ¦ cannot move. | 49 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/chassis/("car_trace")` | fx ( "tracewheel05a" ) step ( 0.14 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03" ) step ( 0.08 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03a" ) step ( 0.09 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03d" ) step ( 0.08 ) wheell ( "wlr" ) wheelr ( "wrr" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel06" ) step ( 0.08 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel03" ) step ( 0.08 ) wheell ( "wlr" ) wheelr ( "wrr" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel06" ) step ( 0.07 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel02" ) step ( 0.1 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel02" ) step ( 0.1 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel02a" ) step ( 0.08 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL1" ) boner ( "FXtraceR1" ) ¦ fx ( "tracewheel02a" ) step ( 0.15 ) wheell ( "wlr" ) wheelr ( "wrr" ) bonel ( "FXtraceL2" ) boner ( "FXtraceR2" ) ¦ fx ( "tracewheel02" ) step ( 0.1 ) wheell ( "wlf" ) wheelr ( "wrf" ) bonel ( "FXtraceL3" ) boner ( "FXtraceR3" ) | 미확인 |  | 31 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/chassis/("mobility_btr")` | speed ( 96 ) speedatmaxturn ( 25 ) reverse ( 82 ) weight ( 3.2 ) power ( 55 ) fuel ( 81 ) type ( fuel ) range ( 320 ) ¦ speed ( 53 ) speedatmaxturn ( 24 ) reverse ( 17 ) weight ( 5.1 ) power ( 52 ) fuel ( 118 ) type ( fuel ) range ( 200 ) | 미확인 |  | 2 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/chassis/("mobility_cannon")` | 예: speed ( 4.3 ) weight ( 6190 ) (13종 이상) | 미확인 |  | 23 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/chassis/("mobility_car")` | speed ( 72 ) speedatmaxturn ( 14 ) reverse ( 14 ) weight ( 9.1 ) power ( 147 ) fuel ( 225 ) type ( fuel ) range ( 320 ) ¦ speed ( 72 ) speedatmaxturn ( 14 ) reverse ( 14 ) weight ( 9.3 ) power ( 147 ) fuel ( 225 ) type ( fuel ) range ( 320 ) ¦ speed ( 65 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 4.631 ) power ( 72 ) fuel ( 145 ) type ( fuel ) range ( 450 ) ¦ speed ( 65 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 5.131 ) power ( 72 ) fuel ( 145 ) type ( fuel ) range ( 250 ) ¦ speed ( 72 ) speedatmaxturn ( 14 ) reverse ( 14 ) weight ( 8.707 ) power ( 147 ) fuel ( 227 ) type ( fuel ) range ( 320 ) ¦ speed ( 96 ) speedatmaxturn ( 12 ) reverse ( 18 ) weight ( 1.11 ) power ( 60 ) fuel ( 57 ) type ( fuel ) range ( 480 ) ¦ speed ( 72 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 4.00 ) power ( 104 ) fuel ( 150 ) type ( fuel ) range ( 480 ) ¦ speed ( 72 ) speedatmaxturn ( 15 ) reverse ( 22 ) weight ( 4.16 ) power ( 104 ) fuel ( 150 ) type ( fuel ) range ( 480 ) | 미확인 |  | 10 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/chassis/("mobility_tank")` | 예: speed ( 48 ) reverse ( 20 ) traverse ( 35 ) weight ( 3.75 ) power ( 85 ) track ( 5 ) fuel ( 91 ) type ( fuel ) range ( 150 ) (13종 이상) | 미확인 |  | 14 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/chassis/("tank_trace_ex")` | fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.58 ) boneleft ( "FXtraceL3" ) boneright ( "FXtraceR3" ) | 미확인 |  | 5 | `entity/-vehicle/eng/btr/m5_halftrack/m5_halftrack.inc` |
| `/chassis/("tank_trace")` | fx ( "tracks_mid" ) step ( 0.32 ) len ( 0.5 ) ¦ fx ( "tracks_big" ) step ( 0.47 ) len ( 0.79 ) ¦ fx ( "tracks_sm2" ) step ( 0.2 ) len ( 0.37 ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.7 ) ¦ fx ( "tracks_mid" ) step ( 0.3 ) len ( 1.05 ) ¦ fx ( "tracks_sm" ) step ( 0.3 ) len ( 1.15 ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.95 ) ¦ fx ( "tracks_mid" ) step ( 0.52 ) len ( 0.9 ) ¦ fx ( "tracks_mid" ) step ( 0.3 ) len ( 0.7 ) | 미확인 |  | 14 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/collider` | "tank.light" ¦ "cannon_small" ¦ "truck" ¦ "tank.heavy" | 미확인 |  | 15 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/extender[cannon]` | "cannon" | 미확인 |  | 23 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/animation/aim` | 예: "cannon_zis3_gunlayer" "" "" "" "" (13종 이상) | 미확인 |  | 23 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/animation/fire` | "cannon_small_gunner_fire" "cannon_small_charger_fire" "cannon_small_charger2_fire" "cannon_small_commander_fire" ¦ "new_cannon_gunner_fire" "new_cannon_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "cannon_gunlayer_fire-alert" "flak_gunner2_fire" "flak_charger_fire" "" "flak_commander_fire" ¦ "" "cannon_small_charger_fire" "cannon_small_charger2_fire" "cannon_small_commander_fire" ¦ "AAgun_61k_gunner_fire" "AAgun_61k_charger_fire" "" ¦ "" "new_cannon_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "" "cannon_small_charger_fire" "cannon_small_charger2_fire" "" ¦ "aagun_flak37_commander_fire" "aagun_72k_gunner_fire" "flak_charger_fire" "flak_gunner2_fire" "aagun_flak37_gunner_fire" "flak_commander_fire" ¦ "cannon_gunlayer_fire-alert" "cannon_pdr25_charger_fire" "new_cannon_charger2_fire" "new_cannon_commander_fire" ¦ "AAgun_61k_gunner_fire" "AAgun_61k_charger_fire" "flak_charger_fire" "flak_gunner2_fire" "flak_commander_fire" ¦ "mortar_82bm37_gunner_fire" "mortar_82bm37_commander_fire" ¦ "" "mg_lafette_commander_fire" | 미확인 | mg_maxim_gunner_fire | 22 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/animation/move` | "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mortar_driver_l" "mgun_browning_driver_r" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mgun_browning_driver_l" "mgun_browning_driver_r" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "mgun_browning_driver_l" "mgun_browning_driver_l" "mgun_browning_driver_r" ¦ "mgun_browning_driver_r" "mortar_driver_l" ¦ "cannon_pak40_pusher_right" "cannon_pak40_pusher_left" "mortar_driver_l" ¦ "cannon_pak40_pusher_left" "cannon_pak40_pusher_left" "cannon_pak40_pusher_left" "cannon_pak40_pusher_right" "cannon_pak40_pusher_right" "cannon_pak40_pusher_right" ¦ "" "" "" "" "" ¦ "mortar_driver_r" "mortar_driver_l" | 미확인 | move state: cannon being pushed and which positions play animations | 23 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/animation/reload` | 예: "" "flak_gunner2_reload" "flak_charger_reload" "" "flak_commander_reload" (13종 이상) | 미확인 |  | 23 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/animation/switch` | "close" | 미확인 |  | 21 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/attack/obstacle` | "open" | 미확인 |  | 21 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/attack/workers` | "gunner" "charger" "charger2" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "commander" ¦ "gunner" "charger" "commander" ¦ "gunner" "gunner2" "charger" "charger2" "charger3" "commander" | 미확인 |  | 20 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/move/obstacle` | "close" | 미확인 |  | 21 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/move/workers` | "driver1" "driver2" "driver3" "driver4" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" minWorkers 2 ¦ "driver1" "driver2" "driver3" minWorkers 2 ¦ "driver1" "driver2" "driver3" "driver4" "driver5" "driver6" minWorkers 2 ¦ clear | 미확인 |  | 20 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[cannon]/rotate_ik_limit` | 0 | 미확인 |  | 3 | `entity/-vehicle/eng/cannon/40mm_qf40/40mm_qf40.inc` |
| `/extender[cannon]/trailing/workers` | "seat1" "seat2" "seat3" "seat4" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" ¦ "seat1" "seat2" "seat3" ¦ "seat1" "seat2" "seat3" "seat4" "seat5" "seat6" ¦ clear | 미확인 |  | 20 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[enumerator]` | "enumerator" | 미확인 |  | 12 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/extender[enumerator]/digit_folder` | "us2" ¦ "ru1" | 미확인 |  | 12 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/extender[inventory]` | "inventory" | 미확인 |  | 16 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/extender[inventory]/box/item/user` | "mgun2" ¦ "mgun3" ¦ "mgun" | 미확인 |  | 2 | `entity/-vehicle/usa/btr/m2_halftrack/m2_halftrack.inc` |
| `/extender[inventory]/weapon` | "mgun" ¦ "mgun2" ¦ "mgun3" | 미확인 | Placeholder weapon ¦ !!! PLACEHOLDER | 3 | `entity/-vehicle/germany/cannon/mg42_lafette/lafette.inc` |
| `/extender[inventory]/weapon/mask` | "mountable" | 미확인 |  | 3 | `entity/-vehicle/germany/cannon/mg42_lafette/lafette.inc` |
| `/extender[shield]` | "shield" | 미확인 |  | 9 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/extender[smokescreen]` | "smokescreen" | 미확인 |  | 2 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/extender[smokescreen]/cooldown` | 5 | 미확인 | smoke launcher easily accessible from turret ¦ smoke launcher outside vehicle | 2 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/extender[smokescreen]/itemtags` | "grenade no8_smoke" | 미확인 |  | 2 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/extension` | 예: "daimler_dingo.mdl" (13종 이상) | 미확인 | needs full QA | 47 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/mass` | 32 ~ 47500 | 미확인 | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 | 49 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/patherid` | 예: "sdkfz232" (13종 이상) | 미확인 |  | 32 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/placer/place` | 예: "weapon1" (13종 이상) | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 3 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/placer/place/entity` | "lee_enfield_no1_mk3" | 미확인 |  | 1 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/placer/place/handlebone` | "body" | 미확인 |  | 1 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/placer/place/linkbone` | "weapon1" ¦ "weapon2" ¦ "weapon3" | 미확인 |  | 1 | `entity/-vehicle/eng/btr/universal_carrier_mk1/universal_carrier_mk1.inc` |
| `/placer/place/visor` | "main" | 미확인 |  | 1 | `entity/-vehicle/usa/cannon/81mm_m1/m1_mortar.inc` |
| `/props` | 예: "tank" "wheeled" "-turret" "explosion_throw_crew" "vision_lev02" "detect_tank_mg" "open_death_fx" (13종 이상) | 미확인 |  | 49 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/sensor/visor` | "main" ¦ "back" ¦ "around" ¦ "gunner3" ¦ "gunner4" | 미확인 |  | 10 | `entity/-vehicle/eng/cannon/76mm_qf17/76mm_qf17.inc` |
| `/sensor/visor/bone` | "foresight6" ¦ "foresight4" ¦ "foresight5" ¦ "visor" | 미확인 | Boys rot ¦ turret | 4 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/sensor/visor/vision` | "tank_rear-mg" ¦ "vehicle_around" ¦ "cannon_range_210" ¦ "recon_main" ¦ "cannon_mortar" | 미확인 | better vision than a in-closed turret | 10 | `entity/-vehicle/eng/cannon/76mm_qf17/76mm_qf17.inc` |
| `/targetclass` | "howitzer" ¦ "cannon_big" ¦ "armoredcar" ¦ "tank_medium" ¦ "cannon_small" ¦ "tank_heavy" ¦ "tank_light" ¦ "armoredcar" "bullet" ¦ "armoredcar_l" ¦ "cannon_zenite" | 미확인 |  | 39 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/targetselector` | "cannon_howitzer" ¦ "tank_medium" ¦ "tank_bullet" ¦ "cannon_big" ¦ "tank_light" ¦ "car_bullet" ¦ "cannon_small" ¦ "cannon_aa" ¦ "cannon_zenite" | 미확인 |  | 39 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/volume` | 예: "body3" (13종 이상) | 미확인 | Hull ¦ Superstructure | 24 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/volume/able` | ¦ clear | 미확인 |  | 22 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/volume/able/contact` | 1 | 미확인 |  | 1 | `entity/-vehicle/eng/cannon/40mm_qf40/40mm_qf40.inc` |
| `/volume/able/nowounddecal` | 0 | 미확인 |  | 1 | `entity/-vehicle/usa/car/cckw/cckw.inc` |
| `/volume/able/visible` | 0 | 미확인 |  | 2 | `entity/-vehicle/eng/cannon/40mm_qf2/40mm_qf2.inc` |
| `/volume/material` | "wood" | 미확인 |  | 5 | `entity/-vehicle/eng/car/bedford_oyd/bedford_oyd.inc` |
| `/volume/tags` | "wood" ¦ "invisible" | 미확인 |  | 7 | `entity/-vehicle/eng/cannon/40mm_qf2/40mm_qf2.inc` |
| `/volume/thickness` | 0.05 ~ 30 | 미확인 | 15 + 4 ¦ 89 + 89 | 6 | `entity/-vehicle/eng/cannon/94mm_qf_3_7_aa/94mm_qf_3_7_aa.inc` |
| `/weaponry/##place` | "mgun" | 미확인 | maybe future feature | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/weaponry/##place/foresight` | "vision_rot" | 미확인 |  | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/weaponry/##place/gunner` | "commander" | 미확인 |  | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/weaponry/##place/type` | "commander" | 미확인 |  | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/weaponry/##place/weapon` | "commander_vision" | 미확인 | Placeholder weapon ¦ !!! PLACEHOLDER | 3 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/weaponry/place` | "gun" ¦ "mgun" ¦ "mgun2" ¦ "mgun1" ¦ "mgun3" | 미확인 | sits out of view from top of turret ¦ fake job so crew stay in vehicle, cant fire cus of DD canvas | 40 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/weaponry/place/("abm_dymamic_aa")` | zeroing ( 2.25 ) dispersion ( 0.50 ) | 미확인 |  | 2 | `entity/-vehicle/eng/cannon/94mm_qf_3_7_aa/94mm_qf_3_7_aa.inc` |
| `/weaponry/place/("abm_dymamic_aaa")` | zeroing ( 1.95 ) dispersion ( 0.24 ) | 미확인 |  | 1 | `entity/-vehicle/eng/cannon/40mm_qf40/40mm_qf40.inc` |
| `/weaponry/place/("abm_dymamic_gun")` | zeroing ( 8.0 ) dispersion ( 1 ) ¦ zeroing ( 1.50 ) dispersion ( 0.35 ) ¦ zeroing ( 1.00 ) dispersion ( 0.35 ) ¦ zeroing ( 2.00 ) dispersion ( 0.35 ) ¦ zeroing ( 4.0 ) dispersion ( 1 ) ¦ zeroing ( 4.00 ) dispersion ( 0.25 ) ¦ zeroing ( 5.0 ) dispersion ( 1 ) ¦ zeroing ( 3.00 ) dispersion ( 0.35 ) ¦ zeroing ( 2.50 ) dispersion ( 0.35 ) | 미확인 |  | 18 | `entity/-vehicle/eng/cannon/140mm_bl_5_5/140mm_bl_5_5.inc` |
| `/weaponry/place/("abm_dymamic_mortar")` | zeroing ( 3.0 ) dispersion ( 1 ) | 미확인 |  | 1 | `entity/-vehicle/usa/cannon/81mm_m1/m1_mortar.inc` |
| `/weaponry/place/("abm_dymamic_stabilised")` | zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인 |  | 1 | `entity/-vehicle/finland/btr/ba10_fin/ba10_fin.inc` |
| `/weaponry/place/("abm_dymamic_td")` | zeroing ( 2.5 ) dispersion ( 0.25 ) | 미확인 |  | 2 | `entity/-vehicle/usa/tank_medium/m36_gmc/m36_gmc.inc` |
| `/weaponry/place/("abm_dymamic")` | zeroing ( 3.0 ) dispersion ( 0.20 ) ¦ zeroing ( 2.5 ) dispersion ( 0.20 ) | 미확인 |  | 7 | `entity/-vehicle/finland/tank_heavy/kv1_40e_fin/kv1_40e_fin.inc` |
| `/weaponry/place/charger` | "charger charger2" minWorkers 1 ¦ "commander" ¦ "charger" ¦ "gunner" ¦ "gunner3" ¦ "charger2" ¦ "gunner2" ¦ "charger charger2 charger3" minWorkers 1 | 미확인 | done to keep gunner3 from emitting | 39 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/weaponry/place/foresight` | "foresight5" ¦ "foresight4" ¦ "foresight6" ¦ "foresight3" | 미확인 |  | 9 | `entity/-vehicle/finland/btr/ba10_fin/ba10_fin.inc` |
| `/weaponry/place/gunner` | "gunner" ¦ "gunner3" ¦ "gunner gunner2" minWorkers 1 ¦ "commander" ¦ "charger" ¦ "charger2" ¦ "gunner2" | 미확인 |  | 25 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/weaponry/place/item` | "browning_mbrowning_m19a417a1" filling "ammo mgun_usa belt" 250 | 미확인 | 600 rounds ¦ 50 rounds | 1 | `entity/-vehicle/usa/cannon/m2_30cal_tripod/m2_30cal.inc` |
| `/weaponry/place/linkanimation` | "off" | 미확인 |  | 10 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/weaponry/place/linkbone` | "mgun" ¦ "mgun2" ¦ "mgun3" | 미확인 |  | 10 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/weaponry/place/type` | "main" ¦ "coaxial" ¦ "hull" ¦ "rear" ¦ "aa" ¦ "mgun_turret_left" ¦ "mgun_turret_right" ¦ "left" ¦ "right" | 미확인 |  | 19 | `entity/-vehicle/eng/btr/daimler_dingo/daimler_dingo.inc` |
| `/weaponry/place/weapon` | 예: "browning_m2" filled (13종 이상) | 미확인 | Placeholder weapon ¦ !!! PLACEHOLDER | 22 | `entity/-vehicle/eng/btr/m5a1_halftrack/m5a1_halftrack.inc` |

## 연관: 공통 속성 (.ext)

- 위치: `properties/**/*.ext`
- 연결: 본체의 `(include "/properties/...")`. 같은 차종의 모든 유닛이 공유한다

### 뜻을 아는 속성 (1개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/extender[inventory]/box/item` | "can" 1 ¦ "repair_kit" 5 ¦ "repair_kit" 3 ¦ "repair_kit" 1 | **추정** 실려 있는 아이템과 수량 (추정: /game_entity 아래의 같은 자리와 값 형태가 같음) | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 7 | `properties/armoredcar.ext` |

### 뜻을 모르는 속성 (887개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/("armor_tag")` | 예: args "armor_plate" (13종 이상) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/("ballast")` | args 0 ¦ args 1 ¦ args 2 ¦ args 3 ¦ args 4 ¦ args 5 ¦ args 6 ¦ args 7 ¦ args 8 ¦ args 9 ¦ args 10 | 미확인 |  | 1 | `properties/marine.ext` |
| `/("basic_armor")` | args 99 ¦ args 10 ¦ args 15 ¦ args 20 | 미확인 |  | 9 | `properties/airborne.ext` |
| `/("bike_durability")` | body ( 100 ) | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/("bike_thickness")` | thickness ( 1 ) | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/("body")` | repair ( 100 ) ¦ repair ( 15 ) ¦ repair ( 30 ) ¦ repair ( 45 ) ¦ repair ( 20 ) ¦ repair ( 35 ) ¦ repair ( 130 ) | 미확인 |  | 12 | `properties/airborne.ext` |
| `/("cannon_durability")` | body ( 200 ) gun ( 150 ) wheel ( 100 ) shield ( 200 ) | 미확인 |  | 1 | `properties/cannon_armor.ext` |
| `/("cannon_thickness")` | args 8 | 미확인 |  | 1 | `properties/cannon_armor.ext` |
| `/("car_thickness")` | body ( 2.0 ) engine ( 1.5 ) | 미확인 |  | 1 | `properties/car.ext` |
| `/("crew")` | place ( driver ) bone ( driver ) visor ( none ) door ( emit1 ) animation ( driver ) group ( crew ) off ( ) | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/("engine")` | repair ( 45 ) ¦ repair ( 30 ) ¦ repair ( 20 ) | 미확인 |  | 5 | `properties/armoredcar.ext` |
| `/("general_durability")` | body ( 200 ) engine ( 125 ) turret ( 100 ) mantlet ( 125 ) gun ( 100 ) track ( 100 ) ammo ( 120 ) ¦ body ( 150 ) engine ( 85 ) turret ( 125 ) mantlet ( 100 ) gun ( 150 ) track ( 100 ) ammo ( 120 ) ¦ body ( 250 ) engine ( 150 ) turret ( 150 ) mantlet ( 150 ) gun ( 150 ) track ( 150 ) ammo ( 120 ) | 미확인 |  | 3 | `properties/armoredcar.ext` |
| `/("gun")` | repair ( 30 ) thickness ( 10 ) ¦ repair ( 30 ) thickness ( 20 ) ¦ thickness ( 20 ) repair ( 30 ) | 미확인 |  | 4 | `properties/cannon.ext` |
| `/("holes")` | 예: volume ( hole ) number ( ) (13종 이상) | 미확인 |  | 2 | `properties/vehicle.ext` |
| `/("main_weapon")` | 예: args "gun" (13종 이상) | 미확인 |  | 1 | `properties/vehicle_xx.ext` |
| `/("marine_durability")` | body ( 400 ) engine ( 60 ) gun ( 100 ) | 미확인 |  | 1 | `properties/marine.ext` |
| `/("moto_durability")` | body ( 110 ) engine ( 60 ) | 미확인 |  | 1 | `properties/moto.ext` |
| `/("moto_thickness")` | body ( 1.5 ) engine ( 1.5 ) | 미확인 |  | 1 | `properties/moto.ext` |
| `/("plane_durability")` | body ( 100 ) engine ( 50 ) propeller ( 50 ) wing ( 75 ) wheel ( 100 ) part ( 100 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/("sandbags")` | volume ( sandbag ) number ( ) ¦ volume ( sandbag ) number ( 1 ) ¦ volume ( sandbag ) number ( 2 ) ¦ volume ( sandbag ) number ( 3 ) ¦ volume ( sandbag ) number ( 4 ) ¦ volume ( sandbag ) number ( 5 ) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/("tearable")` | 예: volume ( flap ) number ( 1 ) (13종 이상) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/("tracks")` | repair ( 30 ) thickness ( 20 ) | 미확인 |  | 2 | `properties/halftrack.ext` |
| `/("train_durability")` | body ( 300 ) engine ( 200 ) turret ( 200 ) mantlet ( 175 ) gun ( 200 ) | 미확인 |  | 1 | `properties/railway.ext` |
| `/("turret")` | repair ( 45 ) | 미확인 |  | 1 | `properties/tank.ext` |
| `/("visible")` | 예: volume ( body ) number ( ) tags ( ) extra ( ) (13종 이상) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/("visible")/extra()/component` | "body" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/("visible")/extra()/material` | "glass" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/("wing")` | number ( r1 ) component ( right ) tag ( wing ) ¦ number ( r2 ) component ( right ) tag ( wing ) ¦ number ( r3 ) component ( right ) tag ( wing ) ¦ number ( l1 ) component ( left ) tag ( wing ) ¦ number ( l2 ) component ( left ) tag ( wing ) ¦ number ( l3 ) component ( left ) tag ( wing ) ¦ number ( rt1 ) component ( right_top ) tag ( wing ) ¦ number ( rt2 ) component ( right_top ) tag ( wing ) ¦ number ( lt1 ) component ( left_top ) tag ( wing ) ¦ number ( lt2 ) component ( left_top ) tag ( wing ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/("wood_shield")` | number ( ) ¦ number ( 1 ) ¦ number ( 2 ) ¦ number ( 3 ) ¦ number ( 4 ) ¦ number ( 5 ) ¦ number ( 6 ) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "ammo")/("internals")` | args %0 ammo 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "armor_carried_cannon")/("cannon_durability")` | body ( 150 ) gun ( 150 ) wheel ( 100 ) shield ( 150 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_carried_cannon")/("cannon_thickness")` | args 6 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_heavy_cannon")/("body")` | repair ( 90 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_heavy_cannon")/("cannon_durability")` | body ( 200 ) gun ( 150 ) wheel ( 100 ) shield ( 300 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_heavy_cannon")/("cannon_thickness")` | args 8 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_light_cannon")/("cannon_durability")` | body ( 150 ) gun ( 150 ) wheel ( 100 ) shield ( 200 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_light_cannon")/("cannon_thickness")` | args 6 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_medium_cannon")/("cannon_durability")` | body ( 175 ) gun ( 150 ) wheel ( 100 ) shield ( 250 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_medium_cannon")/("cannon_thickness")` | args 6 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armor_tag")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "armor_tag")/volume/tags` | "armor" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "armor")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "armor")/volume/tags` | "armor" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "armor")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "armoredcar_tier1")/("basic_armor")` | args 15 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armoredcar_tier1")/("general_durability")` | body ( 175 ) engine ( 150 ) turret ( 150 ) mantlet ( 100 ) gun ( 150 ) track ( 150 ) ammo ( 120 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armoredcar_tier2")/("basic_armor")` | args 15 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "armoredcar_tier2")/("general_durability")` | body ( 200 ) engine ( 150 ) turret ( 150 ) mantlet ( 125 ) gun ( 150 ) track ( 150 ) ammo ( 120 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "aux_turret")/("make_component")` | args "aux_turret1" "aux_turret1" ¦ args "aux_turret11" "aux_turret1" ¦ args "aux_turret12" "aux_turret1" ¦ args "aux_turret2" "aux_turret2" ¦ args "aux_turret21" "aux_turret2" ¦ args "aux_turret22" "aux_turret2" ¦ args "aux_turret3" "aux_turret3" ¦ args "aux_turret31" "aux_turret3" ¦ args "aux_turret32" "aux_turret3" ¦ args "aux_turret4" "aux_turret4" ¦ args "aux_turret41" "aux_turret4" ¦ args "aux_turret42" "aux_turret4" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "aux_turret")/component` | "aux_turret1" ¦ "aux_turret2" ¦ "aux_turret3" ¦ "aux_turret4" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "aux_turret")/component/repairorder` | 6 ~ 7 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "aux_turret")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "ballast")/volume` | "ballast%0" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/marine.ext` |
| `/(define "ballast")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 1 | `properties/marine.ext` |
| `/(define "ballast")/volume/tags` | "float" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/marine.ext` |
| `/(define "basic_armor")/armor/("set_ammo")` | args %0 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "basic_armor")/armor/("set_armor")` | 예: args body (13종 이상) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "bike_durability")/perks/component` | "body" ¦ tag "wheel" | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "bike_durability")/perks/component/armor_damage_modifier` | 2 ~ 5 | 미확인 | multiplier adjusts ratio of which part of penetration does damage to component. (shell penetration - armor thickness) verses minimumDamageModifier. values < 1 = less damage done by over penetration. | 1 | `properties/armor.ext` |
| `/(define "bike_durability")/perks/component/health/hp` | %body ¦ 125 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "bike_durability")/perks/component/health/hp_broken` | ( + 1500 ( * %body 5 ) ) | 미확인 | destroys component and cannot be repaired | 1 | `properties/armor.ext` |
| `/(define "bike_parts")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/bicycle.ext` |
| `/(define "bike_parts")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/bicycle.ext` |
| `/(define "bike_parts")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/(define "bike_thickness")/("bike_parts")` | args "body1" %thickness ¦ args "body2" %thickness ¦ args "body3" %thickness ¦ args "body4" %thickness ¦ args "body5" %thickness ¦ args "body6" %thickness ¦ args "body7" %thickness ¦ args "body8" %thickness ¦ args "body9" %thickness ¦ args "body10" %thickness ¦ args "wheelleft1" 0.5 ¦ args "wheelleft2" 0.5 | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/(define "board_animation")/boarder/anm` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "board_animation")/boarder/anm/forward/base` | "%base" | 미확인 | seconds : CP | 1 | `properties/car_crew.ext` |
| `/(define "board_animation")/boarder/anm/forward/begin` | "%board_begin" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "board_animation")/boarder/anm/forward/end` | "%board_end" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "board_animation")/boarder/anm/reverse/base` | "%base" | 미확인 | seconds : CP | 1 | `properties/car_crew.ext` |
| `/(define "board_animation")/boarder/anm/reverse/end` | "%board_begin" -1 | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "body")/("make_component")` | 예: args "body" "body" (13종 이상) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "body")/component` | "body" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "body")/component/repairorder` | 0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "body")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "bodyback")/("make_component")` | 예: args "bodyback" "bodyback" (13종 이상) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "bodyback")/component` | "bodyback" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "bodyback")/component/repairorder` | 6 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "bodyback")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "brain_vehicle")/brain` | "vehicle" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "brain_vehicle")/brain/const` | "crush/min_danger_zone_time" 1 ¦ "crush/min_danger_zone_length" 1 | 미확인 | hide if danger is more than "x" meters away, otherwise run away ¦ disable ai weapon switch for 5 seconds | 1 | `properties/vehicle.ext` |
| `/(define "brain_vehicle")/brain/state` | "vehicle" | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 1 | `properties/vehicle.ext` |
| `/(define "cabin")/("make_component")` | args "cabin" "cabin" ¦ args "cabin1" "cabin" ¦ args "cabin2" "cabin" ¦ args "cabin3" "cabin" ¦ args "cabin4" "cabin" ¦ args "cabin5" "cabin" ¦ args "cabin6" "cabin" ¦ args "cabin7" "cabin" ¦ args "cabin8" "cabin" ¦ args "cabin9" "cabin" ¦ args "cabin10" "cabin" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "cabin")/component` | "cabin" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "cabin")/component/repairorder` | 2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "cabin")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "cannon_durability")/perks/component` | "body" ¦ "gun" ¦ tag "wheel" ¦ tag "shield" | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "cannon_durability")/perks/component/health/hp` | %body ¦ %gun ¦ %wheel ¦ %shield | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "cannon_durability")/perks/component/health/hp_broken` | ( * %body 0.5 ) | 미확인 | destroys component and cannot be repaired | 1 | `properties/armor.ext` |
| `/(define "cannon_thickness")/volume` | 예: "body" (13종 이상) | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/cannon_armor.ext` |
| `/(define "cannon_thickness")/volume/able/nolook` | 1 | 미확인 |  | 1 | `properties/cannon_armor.ext` |
| `/(define "cannon_thickness")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/cannon_armor.ext` |
| `/(define "cannon_thickness")/volume/thickness` | %0 | 미확인 |  | 1 | `properties/cannon_armor.ext` |
| `/(define "car_thickness")/volume` | 예: "body" (13종 이상) | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/car.ext` |
| `/(define "car_thickness")/volume/thickness` | %body ¦ %engine | 미확인 |  | 1 | `properties/car.ext` |
| `/(define "car_trace")/track` | %wheell ¦ %wheelr | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "car_trace")/track/tracer/bone` | %bonel ¦ %boner | 미확인 | Land;"}} ¦ "}} | 1 | `properties/vehicle.ext` |
| `/(define "car_trace")/track/tracer/step` | %step | 미확인 | in meters ¦ in degrees; possible values: 1, 3, 5, 9, 15, 45 [ 360 % (step * 8) = 0 ] | 1 | `properties/vehicle.ext` |
| `/(define "car_trace")/track/tracer/view` | %fx | 미확인 | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring | 1 | `properties/vehicle.ext` |
| `/(define "charger_visible")/("crew_open")` | place ( charger ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit1 ) door2 ( emit2 ) anm_left ( left ) anm_right ( right ) group ( crew ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "charger_visible2")/("crew_open")` | place ( charger ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit2 ) door2 ( emit3 ) anm_left ( left ) anm_right ( right ) group ( crew ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "charger2_visible")/("crew_open")` | place ( charger2 ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit1 ) door2 ( emit2 ) anm_left ( left ) anm_right ( right ) group ( crew ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "chassis_defaults")/fuelbag/fuel` | "fuel" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/vehicle.ext` |
| `/(define "chassis_defaults")/fuelbag/rate` | 40 | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/vehicle.ext` |
| `/(define "chassis_defaults")/fuelbag/remain` | 200 | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/vehicle.ext` |
| `/(define "chassis_defaults")/fuelbag/volume` | 200 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "chassis_defaults")/speed/fast` | 30 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/vehicle.ext` |
| `/(define "chassis_defaults")/speed/normal` | 17 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/vehicle.ext` |
| `/(define "chassis_defaults")/speed/slow` | 10 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/vehicle.ext` |
| `/(define "commander_visible")/("crew_open")` | place ( commander ) bone ( gunner2 ) visorbone ( vision_rot ) door ( emit1 ) door2 ( emit2 ) anm_left ( left ) anm_right ( right ) group ( crew ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "component")/component` | %component | 미확인 |  | 1 | `properties/marine.ext` |
| `/(define "component")/component/repairorder` | 0 | 미확인 |  | 1 | `properties/marine.ext` |
| `/(define "component")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/marine.ext` |
| `/(define "component")/component/tags` | %component "hull" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/marine.ext` |
| `/(define "component")/volume` | %volume | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/marine.ext` |
| `/(define "component")/volume/component` | %component | 미확인 |  | 1 | `properties/marine.ext` |
| `/(define "component")/volume/tags` | %component "hull" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/marine.ext` |
| `/(define "crew_1_human")/("crew_human")` | place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_10_human")/("crew_human")` | place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( main1 ) ¦ place ( charger3 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner4 ) door ( emit1 ) door2 ( emit2 ) visor ( main2 ) ¦ place ( charger4 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner5 ) door ( emit1 ) door2 ( emit2 ) visor ( mg1 ) ¦ place ( gunner6 ) door ( emit1 ) door2 ( emit2 ) visor ( mg2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_2_human_2")/("crew_human_v")` | place ( commander ) bone ( commander ) door ( emit2 ) animation ( commander ) group ( crew ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_2_human_2")/("crew_human")` | place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) group ( crew ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( main ) | 미확인 |  | 2 | `properties/car_crew.ext` |
| `/(define "crew_2_human_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_2_human_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_2_human")/("crew_human_v")` | place ( commander ) bone ( commander ) door ( emit2 ) animation ( commander ) group ( passenger ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_2_human")/("crew_human")` | place ( driver ) bone ( driver ) door ( emit1 ) animation ( driver ) group ( crew ) ¦ place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) | 미확인 |  | 2 | `properties/car_crew.ext` |
| `/(define "crew_3_human_2_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_3_human_2_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_3_human_2")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_3_human_3")/("crew_human")` | place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_3_human_4")/("crew_human")` | place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( main ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_3_human_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_3_human_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_3_human")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_2_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_2_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_2_extended2")/("crew_driver")` | door ( emit0 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_2_extended2")/("crew_gunner3")` | door ( emit1 ) door2 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_2_extended2")/("crew_human")` | place ( gunner ) door ( emit2 ) door2 ( emit3 ) visor ( main ) ¦ place ( commander ) door ( emit2 ) door2 ( emit3 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_2")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_3")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_4_human")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_2_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_2_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( charger2 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_2")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( charger2 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_extended2")/("crew_driver")` | door ( emit0 ) door2 ( emit2 ) door3 ( emit3 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_extended2")/("crew_gunner3")` | door ( emit1 ) door2 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human_extended2")/("crew_human")` | place ( gunner ) door ( emit2 ) door2 ( emit3 ) visor ( main ) ¦ place ( charger ) door ( emit2 ) door2 ( emit3 ) visor ( none ) ¦ place ( commander ) door ( emit2 ) door2 ( emit3 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_5_human")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_6_human_2_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_6_human_2_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( charger2 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_6_human_extended")/("crew_driver")` | door ( emit0 ) door2 ( emit1 ) door3 ( emit2 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_6_human_extended")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( charger2 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_6_human")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( charger2 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_7_human_2")/("crew_human")` | place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( back ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger3 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner4 ) door ( emit1 ) door2 ( emit2 ) visor ( main1 ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_7_human")/("crew_human")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) visor ( main ) ¦ place ( driver ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( charger2 ) door ( emit1 ) door2 ( emit2 ) visor ( none ) ¦ place ( gunner3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( charger3 ) door ( emit1 ) door2 ( emit2 ) visor ( driver ) ¦ place ( commander ) door ( emit1 ) door2 ( emit2 ) visor ( around ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_carried_ammo")/("crew_open_2")` | place ( gunner ) door ( emit1 ) door2 ( emit2 ) anm_left ( gunner ) anm_right ( commander ) extra ( ) ¦ place ( commander ) door ( emit2 ) door2 ( emit1 ) anm_left ( gunner ) anm_right ( commander ) extra ( ) ¦ place ( driver1 ) door ( emit1 ) door2 ( emit2 ) anm_left ( driver1 ) anm_right ( driver2 ) extra ( ) ¦ place ( driver2 ) door ( emit2 ) door2 ( emit1 ) anm_left ( driver1 ) anm_right ( driver2 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_ammo")/boarder/link` | "driver1" "gunner" ¦ "driver2" "commander" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_ammo")/placer/place` | "gunner" ¦ "commander" ¦ "driver1" ¦ "driver2" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_cannon_4")/("crew_open")` | place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) ¦ place ( charger ) group ( crew ) bone ( charger ) door ( emit2 ) animation ( charger ) extra ( { openForCrush } ) ¦ place ( charger2 ) group ( crew ) bone ( charger2 ) door ( emit3 ) animation ( charger2 ) extra ( { openForCrush } ) ¦ place ( commander ) group ( crew ) bone ( commander ) door ( emit4 ) animation ( commander ) extra ( { openForCrush } ) ¦ place ( driver1 ) group ( crew ) bone ( driver1 ) door ( emit1 ) animation ( driver1 ) extra ( ) ¦ place ( driver2 ) group ( crew ) bone ( driver2 ) door ( emit2 ) animation ( driver2 ) extra ( ) ¦ place ( driver3 ) group ( crew ) bone ( driver3 ) door ( emit3 ) animation ( driver3 ) extra ( ) ¦ place ( driver4 ) group ( crew ) bone ( driver4 ) door ( emit4 ) animation ( driver4 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_cannon_4")/boarder/link` | "driver1" "gunner" ¦ "driver2" "charger" ¦ "driver3" "charger2" ¦ "driver4" "commander" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_cannon_4")/placer/place` | "gunner" ¦ "charger" ¦ "charger2" ¦ "commander" ¦ "driver1" ¦ "driver2" ¦ "driver3" ¦ "driver4" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_cannon_4")/placer/place/visor` | "main" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_cannon")/("crew_open")` | place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) ¦ place ( commander ) group ( crew ) bone ( commander ) door ( emit2 ) animation ( commander ) extra ( { openForCrush } ) ¦ place ( driver1 ) group ( crew ) bone ( driver1 ) door ( emit1 ) animation ( driver1 ) extra ( ) ¦ place ( driver2 ) group ( crew ) bone ( driver2 ) door ( emit2 ) animation ( driver2 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_cannon")/boarder/link` | "driver1" "gunner" ¦ "driver2" "commander" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_carried_cannon")/placer/place` | "gunner" ¦ "commander" ¦ "driver1" ¦ "driver2" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_co_driver")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_co_driver")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/tank_crew.ext` |
| `/(define "crew_co_driver")/boarder/link/anm` | "co_driver" ¦ "left" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_co_driver")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_co_driver")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_co_driver")/placer/place` | "%place" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_co_driver")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/tank_crew.ext` |
| `/(define "crew_co_driver")/placer/place/visor` | "driver" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/boarder/door` | "%door" ¦ "%door2" ¦ "%door3" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/boarder/link` | "%door" "driver" ¦ "%door2" "driver" ¦ "%door3" "driver" | 미확인 | { link <from> <to> } | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/boarder/link/anm` | "driver" ¦ "left" ¦ "right" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/placer/place` | "driver" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/tank_crew.ext` |
| `/(define "crew_driver")/placer/place/visor` | "driver" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/boarder/link/anm` | "left" ¦ "right" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/placer/place` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/placer/place/group` | "%group" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/placer/place/turnoff/targetable` | 0 | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner")/placer/place/visor` | "%visor" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_gunner3")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner3")/boarder/link` | "%door" "gunner3" ¦ "%door2" "gunner3" | 미확인 | { link <from> <to> } | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner3")/boarder/link/anm` | "co_driver" ¦ "left" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner3")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner3")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner3")/placer/place` | "gunner3" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner3")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/tank_crew.ext` |
| `/(define "crew_gunner3")/placer/place/visor` | "driver" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_2_v")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/boarder/link/anm` | "left" ¦ "right" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/placer/place` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2_v")/placer/place/turnoff/targetable` | 0 | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/boarder/link/anm` | "left" ¦ "right" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/placer/place` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_2")/placer/place/turnoff/targetable` | 0 | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/boarder/door` | "%door" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/boarder/link` | "%door" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/boarder/link/anm` | "%animation" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/placer/place` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/placer/place/group` | "%group" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_v")/placer/place/turnoff/targetable` | 0 | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link/anm` | "%anm_left" ¦ "%anm_right" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place` | "%place" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place/linkbone` | "%place" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm")/placer/place/visor` | "%visor" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link/anm` | "%anm_left" ¦ "%anm_right" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/placer/place` | "%place" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible_anm2")/placer/place/linkbone` | "%place" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible")/("crew_human_visible_anm")` | place ( %place ) door ( %door ) door2 ( %door2 ) visor ( %visor ) anm_left ( left ) anm_right ( right ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human_visible2")/("crew_human_visible_anm2")` | place ( %place ) door ( %door ) door2 ( %door2 ) anm_left ( left ) anm_right ( right ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_human")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 2 | `properties/car_crew.ext` |
| `/(define "crew_human")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 2 | `properties/car_crew.ext` |
| `/(define "crew_human")/boarder/link/anm` | "%animation" ¦ "left" ¦ "right" | 미확인 |  | 2 | `properties/car_crew.ext` |
| `/(define "crew_human")/boarder/link/forward` | putoff | 미확인 |  | 2 | `properties/car_crew.ext` |
| `/(define "crew_human")/boarder/link/reverse` | puton | 미확인 |  | 2 | `properties/car_crew.ext` |
| `/(define "crew_human")/placer/place` | "%place" | 미확인 |  | 2 | `properties/car_crew.ext` |
| `/(define "crew_human")/placer/place/group` | "%group" ¦ "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 2 | `properties/car_crew.ext` |
| `/(define "crew_human")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human")/placer/place/turnoff/targetable` | 0 | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_human")/placer/place/visor` | "%visor" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_inside")/("board_animation")` | place ( driver ) board_begin ( board_bigcar ) board_end ( seat_driver_car ) base ( doordriver ) ¦ place ( passenger ) board_begin ( board_bigcar ) board_end ( seat_passenger ) base ( doorpassenger ) ¦ place ( commander ) board_begin ( board_bigcar ) board_end ( seat_passenger ) base ( doorcommander ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew_inside")/boarder/anm` | "left" ¦ "right" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_inside")/boarder/anm/forward/begin` | "board_tank" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_inside")/boarder/anm/reverse/end` | "emit_tank_1_hold" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_open_2")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open_2")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open_2")/boarder/link/anm` | "%anm_left" ¦ "%anm_right" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open_2")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open_2")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open_2")/placer/place` | "%place" %extra | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open_2")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open_2")/placer/place/linkbone` | "%place" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/boarder/link/anm` | "%animation" ¦ "%anm_left" ¦ "%anm_right" | 미확인 |  | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/boarder/link/forward` | putoff | 미확인 |  | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/boarder/link/reverse` | puton | 미확인 |  | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/placer/place` | "%place" %extra ¦ "%place" | 미확인 |  | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/placer/place/group` | "%group" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/placer/place/linkbone` | "%bone" | 미확인 |  | 2 | `properties/cannon_crew.ext` |
| `/(define "crew_open")/placer/place/visor` | "top" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_open")/sensor/visor` | "top" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_open")/sensor/visor/bone` | "%visorbone" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/tank_crew.ext` |
| `/(define "crew_open")/sensor/visor/vision` | "human" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "crew_wheeled_cannon_2")/("crew_open")` | place ( seat1 ) group ( crew ) bone ( seat1 ) door ( emit1 ) animation ( seat1 ) extra ( ) ¦ place ( seat2 ) group ( crew ) bone ( seat2 ) door ( emit2 ) animation ( seat2 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_2")/boarder/link` | "seat1" "driver1" ¦ "seat2" "driver2" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_2")/placer/place` | "seat1" ¦ "seat2" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_3")/("crew_open")` | place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) ¦ place ( charger ) group ( crew ) bone ( charger ) door ( emit2 ) animation ( charger ) extra ( { openForCrush } ) ¦ place ( commander ) group ( crew ) bone ( commander ) door ( emit3 ) animation ( commander ) extra ( { openForCrush } ) ¦ place ( driver1 ) group ( crew ) bone ( driver1 ) door ( emit1 ) animation ( driver1 ) extra ( ) ¦ place ( driver2 ) group ( crew ) bone ( driver2 ) door ( emit2 ) animation ( driver2 ) extra ( ) ¦ place ( driver3 ) group ( crew ) bone ( driver3 ) door ( emit3 ) animation ( driver3 ) extra ( ) ¦ place ( seat1 ) group ( crew ) bone ( seat1 ) door ( emit1 ) animation ( seat1 ) extra ( ) ¦ place ( seat2 ) group ( crew ) bone ( seat2 ) door ( emit2 ) animation ( seat2 ) extra ( ) ¦ place ( seat3 ) group ( crew ) bone ( seat3 ) door ( emit3 ) animation ( seat3 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_3")/boarder/link` | "seat1" "driver1" ¦ "seat2" "driver2" ¦ "seat3" "driver3" ¦ "driver1" "gunner" ¦ "driver2" "charger" ¦ "driver3" "commander" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_3")/placer/place` | "gunner" ¦ "charger" ¦ "commander" ¦ "driver1" ¦ "driver2" ¦ "driver3" ¦ "seat1" ¦ "seat2" ¦ "seat3" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_3")/placer/place/visor` | "main" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_4")/("crew_open")` | place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) ¦ place ( charger ) group ( crew ) bone ( charger ) door ( emit2 ) animation ( charger ) extra ( { openForCrush } ) ¦ place ( charger2 ) group ( crew ) bone ( charger2 ) door ( emit3 ) animation ( charger2 ) extra ( { openForCrush } ) ¦ place ( commander ) group ( crew ) bone ( commander ) door ( emit4 ) animation ( commander ) extra ( { openForCrush } ) ¦ place ( driver1 ) group ( crew ) bone ( driver1 ) door ( emit1 ) animation ( driver1 ) extra ( ) ¦ place ( driver2 ) group ( crew ) bone ( driver2 ) door ( emit2 ) animation ( driver2 ) extra ( ) ¦ place ( driver3 ) group ( crew ) bone ( driver3 ) door ( emit3 ) animation ( driver3 ) extra ( ) ¦ place ( driver4 ) group ( crew ) bone ( driver4 ) door ( emit4 ) animation ( driver4 ) extra ( ) ¦ place ( seat1 ) group ( crew ) bone ( seat1 ) door ( emit1 ) animation ( seat1 ) extra ( ) ¦ place ( seat2 ) group ( crew ) bone ( seat2 ) door ( emit2 ) animation ( seat2 ) extra ( ) ¦ place ( seat3 ) group ( crew ) bone ( seat3 ) door ( emit3 ) animation ( seat3 ) extra ( ) ¦ place ( seat4 ) group ( crew ) bone ( seat4 ) door ( emit4 ) animation ( seat4 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_4")/boarder/link` | "seat1" "driver1" ¦ "seat2" "driver2" ¦ "seat3" "driver3" ¦ "seat4" "driver4" ¦ "driver1" "gunner" ¦ "driver2" "charger" ¦ "driver3" "charger2" ¦ "driver4" "commander" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_4")/placer/place` | "gunner" ¦ "charger" ¦ "charger2" ¦ "commander" ¦ "driver1" ¦ "driver2" ¦ "driver3" ¦ "driver4" ¦ "seat1" ¦ "seat2" ¦ "seat3" ¦ "seat4" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_4")/placer/place/visor` | "main" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_5")/("crew_open")` | 예: place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) (13종 이상) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_5")/boarder/link` | "seat1" "driver1" ¦ "seat2" "driver2" ¦ "seat3" "driver3" ¦ "seat4" "driver4" ¦ "seat5" "driver5" ¦ "driver1" "gunner" ¦ "driver2" "gunner2" ¦ "driver3" "charger" ¦ "driver4" "charger2" ¦ "driver5" "commander" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_5")/placer/place` | 예: "gunner" (13종 이상) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_5")/placer/place/visor` | "main" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_6")/("crew_open")` | 예: place ( gunner ) group ( crew ) bone ( gunner ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) (13종 이상) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_6")/boarder/link` | "seat1" "driver1" ¦ "seat2" "driver2" ¦ "seat3" "driver3" ¦ "seat4" "driver4" ¦ "seat5" "driver5" ¦ "seat6" "driver6" ¦ "driver1" "gunner" ¦ "driver2" "gunner2" ¦ "driver3" "charger" ¦ "driver4" "charger2" ¦ "driver5" "charger3" ¦ "driver6" "commander" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_6")/placer/place` | 예: "gunner" (13종 이상) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_6")/placer/place/visor` | "main" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_closed")/("crew_open")` | place ( gunner ) group ( crew ) bone ( ) door ( emit1 ) animation ( gunner ) extra ( { openForCrush } ) ¦ place ( commander ) group ( crew ) bone ( ) door ( emit2 ) animation ( commander ) extra ( { openForCrush } ) ¦ place ( seat1 ) group ( crew ) bone ( seat1 ) door ( emit1 ) animation ( seat1 ) extra ( ) ¦ place ( seat2 ) group ( crew ) bone ( seat2 ) door ( emit2 ) animation ( seat2 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_closed")/boarder/link` | "seat1" "driver1" ¦ "seat2" "driver2" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_closed")/placer/place` | "gunner" ¦ "commander" ¦ "seat1" ¦ "seat2" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon_closed")/placer/place/visor` | "main" ¦ "commander" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon")/("crew_open")` | place ( seat1 ) group ( crew ) bone ( seat1 ) door ( emit1 ) animation ( seat1 ) extra ( ) ¦ place ( seat2 ) group ( crew ) bone ( seat2 ) door ( emit2 ) animation ( seat2 ) extra ( ) | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon")/boarder/link` | "seat1" "driver1" ¦ "seat2" "driver2" | 미확인 | { link <from> <to> } | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon")/placer/place` | "gunner" ¦ "seat1" ¦ "seat2" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew_wheeled_cannon")/placer/place/visor` | "main" | 미확인 |  | 1 | `properties/cannon_crew.ext` |
| `/(define "crew")/boarder/door` | "%door" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew")/boarder/link` | "%door" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/car_crew.ext` |
| `/(define "crew")/boarder/link/anm` | "%animation" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew")/placer/place` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew")/placer/place/group` | "crew" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/car_crew.ext` |
| `/(define "crew")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew")/placer/place/turnoff` | %off | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "crew")/placer/place/visor` | "%visor" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "declare_health")/extender[vitality]` | "vitality" | 미확인 |  | 1 | `properties/animal.ext` |
| `/(define "declare_health")/extender[vitality]/health/current` | %0 | 미확인 | current stamina ¦ current resources | 1 | `properties/animal.ext` |
| `/(define "declare_health")/extender[vitality]/health/inc` | 0.005 | 미확인 | health regeneration per second ¦ stamina regeneration/usage per second (depending on movement mode and load) | 1 | `properties/animal.ext` |
| `/(define "declare_health")/extender[vitality]/health/max` | %0 | 미확인 | max health ¦ max resources | 1 | `properties/animal.ext` |
| `/(define "double_engine")/("engine")` | volume ( engine ) number ( 2 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "double_engine")/("prop_blade")` | volume ( blade1 ) number ( 2 ) ¦ volume ( blade2 ) number ( 2 ) ¦ volume ( blade3 ) number ( 2 ) ¦ volume ( blade4 ) number ( 2 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "double_engine")/("prop_spin")` | volume ( spin ) number ( 2 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/component` | "%volume%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/component/repairorder` | 3 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/component/repairtime` | 20 | 미확인 | 45 ¦ 30 | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/component/tags` | "engine%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/volume` | "%volume%number%part" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/volume/component` | "%volume%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/volume/tags` | "engine%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "engine_part")/volume/thickness` | 2 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "engine")/("make_component")` | args "engine" "engine" ¦ args "engine1" "engine" ¦ args "engine2" "engine" ¦ args "engine3" "engine" ¦ args "engine4" "engine" ¦ args "engine5" "engine" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "engine")/component` | "%volume%number" ¦ "engine" | 미확인 |  | 2 | `properties/airborne.ext` |
| `/(define "engine")/component/repairorder` | 3 | 미확인 |  | 2 | `properties/airborne.ext` |
| `/(define "engine")/component/repairtime` | 20 ¦ %repair | 미확인 | 45 ¦ 30 | 2 | `properties/airborne.ext` |
| `/(define "engine")/component/tags` | "engine%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "engine")/volume` | "%volume%number" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/airborne.ext` |
| `/(define "engine")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/airborne.ext` |
| `/(define "engine")/volume/component` | "%volume%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "engine")/volume/tags` | "engine%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "engine")/volume/thickness` | 2 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "fighter_tier1")/("plane_durability")` | body ( 100 ) engine ( 50 ) propeller ( 50 ) wing ( 80 ) wheel ( 100 ) part ( 100 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "fighter_tier2")/("plane_durability")` | body ( 175 ) engine ( 75 ) propeller ( 50 ) wing ( 125 ) wheel ( 100 ) part ( 100 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "fuel")/("internals")` | args %0 fuel 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "general_durability")/perks/component` | 예: "body" (13종 이상) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "general_durability")/perks/component/armor_damage_modifier` | 5 | 미확인 | multiplier adjusts ratio of which part of penetration does damage to component. (shell penetration - armor thickness) verses minimumDamageModifier. values < 1 = less damage done by over penetration. | 1 | `properties/armor.ext` |
| `/(define "general_durability")/perks/component/health/hp` | %body ¦ %engine ¦ %mantlet ¦ %turret ¦ %gun ¦ %track ¦ 400 ¦ 125 ¦ 60 ¦ 100 ¦ %ammo ¦ 50 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "general_durability")/perks/component/health/hp_broken` | ( + 1500 ( * %body 5 ) ) ¦ %body ¦ ( * %engine 1.5 ) ¦ 1 | 미확인 | destroys component and cannot be repaired | 1 | `properties/armor.ext` |
| `/(define "glass")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "glass")/component/repairorder` | 13 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "glass")/component/tags` | "glass" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "glass")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "glass")/volume/able/obstacle` | 0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "glass")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/vehicle.ext` |
| `/(define "glass")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "glass")/volume/material` | "glass" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "glass")/volume/tags` | "glass" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "glass")/volume/thickness` | 0.1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "global_damage_mod")/armor_damage_modifier` | 0.1 | 미확인 | multiplier adjusts ratio of which part of penetration does damage to component. (shell penetration - armor thickness) verses minimumDamageModifier. values < 1 = less damage done by over penetration. | 1 | `properties/armor.ext` |
| `/(define "gun_shield")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "gun_shield")/volume/tags` | "gun_shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "gun_shield")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun")/("main_weapon")` | 예: args "gun" %thickness gun (13종 이상) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun")/component` | "gun" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "gun1")/("main_weapon")` | args "barrel11" %thickness gun1 ¦ args "barrel12" %thickness gun1 ¦ args "barrel13" %thickness gun1 ¦ args "barrel14" %thickness gun1 ¦ args "barrel15" %thickness gun1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun1")/component` | "gun1" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun1")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun1")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "gun2")/("main_weapon")` | args "barrel21" %thickness gun2 ¦ args "barrel22" %thickness gun2 ¦ args "barrel23" %thickness gun2 ¦ args "barrel24" %thickness gun2 ¦ args "barrel25" %thickness gun2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun2")/component` | "gun2" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun2")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun2")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "gun3")/("main_weapon")` | args "barrel31" %thickness gun3 ¦ args "barrel32" %thickness gun3 ¦ args "barrel33" %thickness gun3 ¦ args "barrel34" %thickness gun3 ¦ args "barrel35" %thickness gun3 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun3")/component` | "gun3" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun3")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun3")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "gun4")/("main_weapon")` | args "barrel41" %thickness gun4 ¦ args "barrel42" %thickness gun4 ¦ args "barrel43" %thickness gun4 ¦ args "barrel44" %thickness gun4 ¦ args "barrel45" %thickness gun4 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun4")/component` | "gun4" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun4")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun4")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "gun5")/("main_weapon")` | args "barrel51" %thickness gun5 ¦ args "barrel52" %thickness gun5 ¦ args "barrel53" %thickness gun5 ¦ args "barrel54" %thickness gun5 ¦ args "barrel55" %thickness gun5 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun5")/component` | "gun5" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun5")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun5")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "gun6")/("main_weapon")` | args "barrel61" %thickness gun6 ¦ args "barrel62" %thickness gun6 ¦ args "barrel63" %thickness gun6 ¦ args "barrel64" %thickness gun6 ¦ args "barrel65" %thickness gun6 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun6")/component` | "gun6" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun6")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "gun6")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "gunner_open")/boarder/door` | "%door" ¦ "%door2" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/boarder/link` | "%door" "%place" ¦ "%door2" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/boarder/link/anm` | "%anm_left" ¦ "%anm_right" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/placer/place` | "%place" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/placer/place/group` | "%group" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/placer/place/visor` | "top" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/sensor/visor` | "top" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/sensor/visor/bone` | "%visorbone" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_open")/sensor/visor/vision` | "human" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_visible")/("crew_open")` | place ( commander ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit1 ) door2 ( emit2 ) anm_left ( left ) anm_right ( right ) group ( crew ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "gunner_visible2")/("crew_open")` | place ( commander ) bone ( gunner2 ) visorbone ( mgun_rot ) door ( emit2 ) door2 ( emit3 ) anm_left ( left ) anm_right ( right ) group ( crew ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "holes")/volume` | "%volume%number" | 미확인 | Main Body ¦ Truck Bed | 2 | `properties/vehicle.ext` |
| `/(define "holes")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 2 | `properties/vehicle.ext` |
| `/(define "holes")/volume/density` | 0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "internals")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "internals")/component/repairorder` | 5 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "internals")/component/repairtime` | %2 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "internals")/component/tags` | "%1" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "internals")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "internals")/volume/able/contact` | 0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "internals")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "internals")/volume/tags` | "%1" "no_target" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "internals")/volume/thickness` | 2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/component/repairorder` | 13 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/component/tags` | "lamp" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/volume/able/obstacle` | 0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/volume/material` | "glass" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/volume/tags` | "lamp" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "lamp")/volume/thickness` | 0.1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "locomotion")/braketime` | 2 ~ 5 | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 2 | `properties/marine.ext` |
| `/(define "locomotion")/fast` | 30 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/railway.ext` |
| `/(define "locomotion")/gears` | 0.5 0.7 1.0 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/marine.ext` |
| `/(define "locomotion")/maxspeed` | 25 ~ 30 | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 2 | `properties/marine.ext` |
| `/(define "locomotion")/normal` | 17 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/railway.ext` |
| `/(define "locomotion")/reargears` | 0.15 0.3 | 미확인 | Reverse max speed. | 1 | `properties/marine.ext` |
| `/(define "locomotion")/slow` | 10 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/railway.ext` |
| `/(define "locomotion")/splineradius` | 3 | 미확인 | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 1 | `properties/marine.ext` |
| `/(define "locomotion")/starttime` | 10 | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 2 | `properties/marine.ext` |
| `/(define "locomotion")/steeroff` | 40 | 미확인 | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 1 | `properties/marine.ext` |
| `/(define "locomotion")/steeron` | 20 | 미확인 | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 1 | `properties/marine.ext` |
| `/(define "locomotion")/stoptime` | 4 ~ 10 | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 2 | `properties/marine.ext` |
| `/(define "locomotion")/turnradius` | 8 | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/marine.ext` |
| `/(define "main_weapon")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 2 | `properties/vehicle.ext` |
| `/(define "main_weapon")/volume/able/contact` | 0 | 미확인 |  | 2 | `properties/vehicle.ext` |
| `/(define "main_weapon")/volume/able/nolook` | 1 | 미확인 |  | 2 | `properties/vehicle.ext` |
| `/(define "main_weapon")/volume/able/obstacle` | 0 | 미확인 |  | 2 | `properties/vehicle.ext` |
| `/(define "main_weapon")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 2 | `properties/vehicle.ext` |
| `/(define "main_weapon")/volume/component` | "%2" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "main_weapon")/volume/tags` | "no_target" "%2" ¦ "no_target" "gun" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 2 | `properties/vehicle.ext` |
| `/(define "main_weapon")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "make_component")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "make_component")/volume/component` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "make_component")/volume/tags` | %1 | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "mantlet")/("make_component")` | args "mantlet" "mantlet" ¦ args "mantlet1" "mantlet" ¦ args "mantlet2" "mantlet" ¦ args "mantlet3" "mantlet" ¦ args "mantlet4" "mantlet" ¦ args "mantlet5" "mantlet" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "mantlet")/component` | "mantlet" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "mantlet")/component/repairorder` | 3 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "mantlet")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "marine_durability")/perks/component` | "body" ¦ tag "hull" ¦ "cabin" ¦ "engine" ¦ tag "gun" ¦ tag "shield" ¦ tag "cut_part" ¦ tag "fuel" ¦ tag "ammo" | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "marine_durability")/perks/component/health/hp` | %body ¦ ( * %body 0.75 ) ¦ %engine ¦ %gun ¦ 100 ¦ 120 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "marine_durability")/perks/component/health/hp_broken` | ( * %body 1 ) ¦ ( * %body 0.75 ) ¦ ( * %engine 3 ) ¦ 1 | 미확인 | destroys component and cannot be repaired | 1 | `properties/armor.ext` |
| `/(define "moto_durability")/perks/component` | "body" ¦ "engine" ¦ tag "wheel" ¦ tag "glass" ¦ tag "shield" ¦ tag "cut_part" ¦ tag "fuel" ¦ tag "ammo" | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "moto_durability")/perks/component/armor_damage_modifier` | 2 ~ 5 | 미확인 | multiplier adjusts ratio of which part of penetration does damage to component. (shell penetration - armor thickness) verses minimumDamageModifier. values < 1 = less damage done by over penetration. | 1 | `properties/armor.ext` |
| `/(define "moto_durability")/perks/component/health/hp` | %body ¦ %engine ¦ 125 ¦ 60 ¦ 100 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "moto_durability")/perks/component/health/hp_broken` | ( * %body 3 ) ¦ %engine ¦ 1 | 미확인 | destroys component and cannot be repaired | 1 | `properties/armor.ext` |
| `/(define "moto_thickness")/volume` | 예: "body" (13종 이상) | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/moto.ext` |
| `/(define "moto_thickness")/volume/tags` | "explosive" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/moto.ext` |
| `/(define "moto_thickness")/volume/thickness` | %body ¦ %engine ¦ 1 | 미확인 |  | 1 | `properties/moto.ext` |
| `/(define "mp_gun_limits")/bone` | "gun_rot1" ¦ "gun_rot2" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/airborne.ext` |
| `/(define "mp_gun_limits")/bone/limits` | -1 1 ¦ -25 15 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "mp_gun_limits")/bone/speed2` | 60 ~ 360 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "mp_mgun_limits")/bone` | "mgun_rot1" ¦ "mgun_rot2" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/airborne.ext` |
| `/(define "mp_mgun_limits")/bone/limits` | -1 1 ¦ -25 15 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "mp_mgun_limits")/bone/speed2` | 60 ~ 360 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "passenger_10_human")/("passenger_human")` | place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat3 ) bone ( seat03 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat4 ) bone ( seat04 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat5 ) bone ( seat05 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat6 ) bone ( seat06 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat7 ) bone ( seat07 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat8 ) bone ( seat08 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat9 ) bone ( seat09 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat10 ) bone ( seat10 ) door ( emit4 ) animation ( passenger ) group ( passenger ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_12_human")/("passenger_human")` | place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat3 ) bone ( seat03 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat4 ) bone ( seat04 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat5 ) bone ( seat05 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat6 ) bone ( seat06 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat7 ) bone ( seat07 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat8 ) bone ( seat08 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat9 ) bone ( seat09 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat10 ) bone ( seat10 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat11 ) bone ( seat11 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat12 ) bone ( seat12 ) door ( emit4 ) animation ( passenger ) group ( passenger ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_4_human")/("passenger_human")` | place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat3 ) bone ( seat03 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat4 ) bone ( seat04 ) door ( emit4 ) animation ( passenger ) group ( passenger ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_6_human")/("passenger_human")` | place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat3 ) bone ( seat03 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat4 ) bone ( seat04 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat5 ) bone ( seat05 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat6 ) bone ( seat06 ) door ( emit4 ) animation ( passenger ) group ( passenger ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_8_human")/("passenger_human")` | place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat3 ) bone ( seat03 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat4 ) bone ( seat04 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat5 ) bone ( seat05 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat6 ) bone ( seat06 ) door ( emit4 ) animation ( passenger ) group ( passenger ) ¦ place ( seat7 ) bone ( seat07 ) door ( emit3 ) animation ( passenger ) group ( passenger ) ¦ place ( seat8 ) bone ( seat08 ) door ( emit4 ) animation ( passenger ) group ( passenger ) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_c47")/("passenger_human")` | 예: place ( seat1 ) bone ( seat01 ) door ( emit1 ) animation ( passenger ) group ( passenger ) (13종 이상) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_horsa")/("passenger_human")` | 예: place ( seat1 ) bone ( seat01 ) door ( emit1 ) animation ( passenger ) group ( passenger ) (13종 이상) | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/boarder/door` | "%door" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/boarder/link` | "%door" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/boarder/link/anm` | "%animation" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/placer/place` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/placer/place/group` | "%group" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/car_crew.ext` |
| `/(define "passenger_human")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/boarder/door` | "%door" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/boarder/link` | "%door" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/boarder/link/anm` | "%animation" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/placer/place` | "%place" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/placer/place/group` | "passenger" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "passenger")/placer/place/turnoff` | %off | 미확인 |  | 1 | `properties/car_crew.ext` |
| `/(define "plane_durability")/perks/component` | "body" ¦ "engine1" ¦ "engine2" ¦ tag "propeller" ¦ tag "wing" ¦ tag "wheel" ¦ tag "part" | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "plane_durability")/perks/component/health/hp` | %body ¦ %engine ¦ %propeller ¦ %wing ¦ %wheel ¦ %part | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "plane_durability")/perks/component/health/hp_broken` | ( * %body 0.5 ) ¦ ( * %engine 1.33 ) ¦ ( * %propeller 1.0 ) ¦ ( * %wing 0.66 ) ¦ 1 | 미확인 | destroys component and cannot be repaired | 1 | `properties/armor.ext` |
| `/(define "prop_blade")/component` | "propeller%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/component/repairorder` | 10 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/component/repairtime` | 20 | 미확인 | 45 ¦ 30 | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/component/tags` | "propeller" "still" "still%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/volume` | "%number%volume" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/volume/component` | "propeller%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/volume/tags` | "propeller" "still" "still%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "prop_blade")/volume/thickness` | 5 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/component` | "%volume%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/component/repairorder` | 10 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/component/repairtime` | 20 | 미확인 | 45 ¦ 30 | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/component/tags` | "propeller" "spin" "spin%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/volume` | "%volume%number" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/volume/component` | "propeller%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/volume/tags` | "propeller" "spin" "spin%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "prop_spin")/volume/thickness` | 5 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "quad_engine")/("engine")` | volume ( engine ) number ( 4 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "quad_engine")/("prop_blade")` | volume ( blade1 ) number ( 4 ) ¦ volume ( blade2 ) number ( 4 ) ¦ volume ( blade3 ) number ( 4 ) ¦ volume ( blade4 ) number ( 4 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "quad_engine")/("prop_spin")` | volume ( spin ) number ( 4 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "recon_tier1")/("plane_durability")` | body ( 75 ) engine ( 50 ) propeller ( 50 ) wing ( 50 ) wheel ( 75 ) part ( 75 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "recon_tier2")/("plane_durability")` | body ( 75 ) engine ( 50 ) propeller ( 50 ) wing ( 70 ) wheel ( 75 ) part ( 75 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "restore_ik_time")/restoreiktime` | 4 ~ 10 | 미확인 |  | 10 | `properties/armoredcar.ext` |
| `/(define "rocket")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/component/repairorder` | 11 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/component/repairtime` | 1 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/component/tags` | "rocket" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/volume/able/obstacle` | 0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/volume/tags` | "rocket" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "rocket")/volume/thickness` | 2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "sandbags")/volume` | "%volume%number" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "sandbags")/volume/material` | "sand" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "sandbags")/volume/thickness` | 10 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "seat_2_human")/("seat_human")` | place ( seat1 ) bone ( seat01 ) door ( emit3 ) animation ( armor ) ¦ place ( seat2 ) bone ( seat02 ) door ( emit4 ) animation ( armor ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_3_human")/("seat_human")` | place ( seat3 ) bone ( seat03 ) door ( emit5 ) animation ( armor ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_4_human")/("seat_human")` | place ( seat4 ) bone ( seat04 ) door ( emit6 ) animation ( armor ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_6_human")/("seat_human")` | place ( seat5 ) bone ( seat05 ) door ( emit7 ) animation ( armor ) ¦ place ( seat6 ) bone ( seat06 ) door ( emit8 ) animation ( armor ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_9_human")/("seat_human")` | place ( seat7 ) bone ( seat07 ) door ( emit9 ) animation ( armor ) ¦ place ( seat8 ) bone ( seat08 ) door ( emit10 ) animation ( armor ) ¦ place ( seat9 ) bone ( seat09 ) door ( emit11 ) animation ( armor ) | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/boarder/door` | "%door" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/boarder/link` | "%door" "%place" | 미확인 | { link <from> <to> } | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/boarder/link/anm` | "%animation" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/boarder/link/forward` | putoff | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/boarder/link/reverse` | puton | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/placer/place` | "%place" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/placer/place/group` | "passenger" | 미확인 | priority of vehicles to attack assuming more than one is in weapon's range ¦ humans | 1 | `properties/tank_crew.ext` |
| `/(define "seat_human")/placer/place/linkbone` | "%bone" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_on_armor")/boarder/anm` | "armor" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_on_armor")/boarder/anm/forward/begin` | "board_bigcar" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_on_armor")/boarder/anm/forward/end` | "seat_armor" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "seat_on_armor")/boarder/anm/reverse/end` | "emit_tank_1_hold" | 미확인 |  | 1 | `properties/tank_crew.ext` |
| `/(define "set_ammo")/setting` | "ammo" ¦ "rocket" | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "set_ammo")/setting/threshold/blast/koef` | %0 ¦ ( * %0 0.25 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "set_ammo")/setting/threshold/pierce/koef` | 1 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "set_ammo")/setting/volumes` | "ammo*" ¦ "rocket*" ¦ "shell*" | 미확인 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 1 | `properties/armor.ext` |
| `/(define "set_armor")/setting` | "%0" | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "set_armor")/setting/threshold/blast/koef` | 0.5 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "set_armor")/setting/threshold/pierce/koef` | 0.8 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "set_armor")/setting/volumes` | "%0*" | 미확인 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 1 | `properties/armor.ext` |
| `/(define "shield_cover")/component` | "shield_cover" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_cover")/component/repairorder` | 12 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_cover")/component/repairtime` | 10 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "shield_cover")/volume` | "shield_cover" ¦ "shield_cover1" ¦ "shield_cover2" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "shield_cover")/volume/component` | "shield_cover" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_cover")/volume/material` | "fabric" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_cover")/volume/thickness` | 0.5 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/component` | %2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/component/repairorder` | 11 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/component/repairtime` | 10 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/component/tags` | "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/volume/component` | %2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/volume/tags` | "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield_group")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_part")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "shield_part")/volume/component` | %2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_part")/volume/tags` | %2 | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield_part")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/component/repairorder` | 11 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/component/repairtime` | 1 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/component/tags` | "cut_part" "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/volume/tags` | "cut_part" "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield_tear")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/component/repairorder` | 11 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/component/repairtime` | 10 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/component/tags` | "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/volume/caststeel` | 0.5 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/volume/tags` | "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield_track")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield")/component/repairorder` | 11 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield")/component/repairtime` | 10 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "shield")/component/tags` | "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "shield")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "shield")/volume/tags` | "shield" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "shield")/volume/thickness` | %1 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "single_engine")/("engine_part")` | volume ( engine ) number ( 1 ) part ( 1 ) ¦ volume ( engine ) number ( 1 ) part ( 2 ) ¦ volume ( engine ) number ( 1 ) part ( 3 ) ¦ volume ( engine ) number ( 1 ) part ( 4 ) ¦ volume ( engine ) number ( 1 ) part ( 5 ) ¦ volume ( engine ) number ( 1 ) part ( 6 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "single_engine")/("engine")` | volume ( engine ) number ( 1 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "single_engine")/("prop_blade")` | volume ( blade1 ) number ( 1 ) ¦ volume ( blade2 ) number ( 1 ) ¦ volume ( blade3 ) number ( 1 ) ¦ volume ( blade4 ) number ( 1 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "single_engine")/("prop_spin")` | volume ( spin ) number ( 1 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "solid_wheel")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "solid_wheel")/component/repairorder` | 10 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "solid_wheel")/component/repairtime` | 10 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "solid_wheel")/component/tags` | "wheel" "no_target" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "solid_wheel")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "solid_wheel")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "solid_wheel")/volume/tags` | "wheel" "no_target" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "solid_wheel")/volume/thickness` | 10 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "tank_heavy_tier1")/("basic_armor")` | args 30 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_heavy_tier1")/("general_durability")` | body ( 450 ) engine ( 250 ) turret ( 275 ) mantlet ( 225 ) gun ( 225 ) track ( 300 ) ammo ( 225 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_heavy_tier2")/("basic_armor")` | args 30 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_heavy_tier2")/("general_durability")` | body ( 500 ) engine ( 250 ) turret ( 300 ) mantlet ( 225 ) gun ( 225 ) track ( 300 ) ammo ( 250 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_light_tier1")/("basic_armor")` | args 15 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_light_tier1")/("general_durability")` | body ( 200 ) engine ( 150 ) turret ( 150 ) mantlet ( 125 ) gun ( 150 ) track ( 150 ) ammo ( 120 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_light_tier2")/("basic_armor")` | args 15 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_light_tier2")/("general_durability")` | body ( 250 ) engine ( 175 ) turret ( 175 ) mantlet ( 150 ) gun ( 175 ) track ( 175 ) ammo ( 125 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_medium_tier1")/("basic_armor")` | args 20 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_medium_tier1")/("general_durability")` | body ( 325 ) engine ( 200 ) turret ( 200 ) mantlet ( 175 ) gun ( 200 ) track ( 200 ) ammo ( 160 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_medium_tier2")/("basic_armor")` | args 20 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_medium_tier2")/("general_durability")` | body ( 350 ) engine ( 200 ) turret ( 225 ) mantlet ( 175 ) gun ( 200 ) track ( 200 ) ammo ( 175 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_medium_tier3")/("basic_armor")` | args 25 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_medium_tier3")/("general_durability")` | body ( 425 ) engine ( 225 ) turret ( 250 ) mantlet ( 175 ) gun ( 200 ) track ( 200 ) ammo ( 210 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tank_trace_ex")/track` | "left" ¦ "right" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "tank_trace_ex")/track/texturescroller/bone` | "TrackL" ¦ "TrackR" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/vehicle.ext` |
| `/(define "tank_trace_ex")/track/texturescroller/length` | %len | 미확인 | meters per animation ¦ 0.6 meters per animation | 1 | `properties/vehicle.ext` |
| `/(define "tank_trace_ex")/track/tracer/bone` | %boneleft ¦ %boneright | 미확인 | Land;"}} ¦ "}} | 1 | `properties/vehicle.ext` |
| `/(define "tank_trace_ex")/track/tracer/step` | %step | 미확인 | in meters ¦ in degrees; possible values: 1, 3, 5, 9, 15, 45 [ 360 % (step * 8) = 0 ] | 1 | `properties/vehicle.ext` |
| `/(define "tank_trace_ex")/track/tracer/view` | %fx | 미확인 | fx has a glow that will show unless hidden until later called ¦ stop cooking FX, turret ring | 1 | `properties/vehicle.ext` |
| `/(define "tank_trace")/("tank_trace_ex")` | fx ( %fx ) step ( %step ) len ( %len ) boneleft ( "FXtraceL1" ) boneright ( "FXtraceR1" ) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "tankette_tier1")/("basic_armor")` | args 15 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tankette_tier1")/("general_durability")` | body ( 175 ) engine ( 150 ) turret ( 150 ) mantlet ( 100 ) gun ( 150 ) track ( 150 ) ammo ( 120 ) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "tearable")/component` | "%volume%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "tearable")/component/repairorder` | 11 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "tearable")/component/repairtime` | 2 | 미확인 | 45 ¦ 30 | 1 | `properties/airborne.ext` |
| `/(define "tearable")/component/tags` | "%volume%number" "part" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "tearable")/volume` | "%volume%number" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/airborne.ext` |
| `/(define "tearable")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/airborne.ext` |
| `/(define "tearable")/volume/component` | "%volume%number" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "tearable")/volume/tags` | "%volume%number" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "tearable")/volume/thickness` | 1 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "track_wheels")/volume` | "track%0%1" | 미확인 | Main Body ¦ Truck Bed | 2 | `properties/vehicle.ext` |
| `/(define "track_wheels")/volume/component` | "track%0" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "track_wheels")/volume/tags` | "mover" "track" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "track_wheels")/volume/thickness` | %2 | 미확인 |  | 2 | `properties/vehicle.ext` |
| `/(define "tracks")/("track_wheels")` | 예: args left 1 %thickness (13종 이상) | 미확인 |  | 2 | `properties/vehicle.ext` |
| `/(define "tracks")/component` | "trackleft" ¦ "trackright" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "tracks")/component/repairorder` | 4 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "tracks")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "tracks")/component/tags` | "mover" "track" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "tracks")/volume` | "trackleft" ¦ "trackright" | 미확인 | Main Body ¦ Truck Bed | 2 | `properties/vehicle.ext` |
| `/(define "tracks")/volume/able/contact_ground` | 0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "tracks")/volume/tags` | "mover" "track" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "tracks")/volume/thickness` | %thickness | 미확인 |  | 2 | `properties/vehicle.ext` |
| `/(define "train_durability")/perks/component` | 예: "body" (13종 이상) | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "train_durability")/perks/component/health/hp` | %body ¦ %engine ¦ %mantlet ¦ %turret ¦ %gun ¦ 200 ¦ 60 ¦ 100 ¦ 170 | 미확인 |  | 1 | `properties/armor.ext` |
| `/(define "train_durability")/perks/component/health/hp_broken` | ( * %body 0.75 ) ¦ %body ¦ %engine ¦ 1 | 미확인 | destroys component and cannot be repaired | 1 | `properties/armor.ext` |
| `/(define "transmission")/("internals")` | args %0 transmission 20 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "triple_engine")/("engine")` | volume ( engine ) number ( 3 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "triple_engine")/("prop_blade")` | volume ( blade1 ) number ( 3 ) ¦ volume ( blade2 ) number ( 3 ) ¦ volume ( blade3 ) number ( 3 ) ¦ volume ( blade4 ) number ( 3 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "triple_engine")/("prop_spin")` | volume ( spin ) number ( 3 ) | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "turret")/("make_component")` | 예: args "turret" "turret" (13종 이상) | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "turret")/component` | "turret" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "turret")/component/repairorder` | 2 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "turret")/component/repairtime` | %repair | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "visible")/volume` | "%volume%number" %extra | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/airborne.ext` |
| `/(define "visible")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/airborne.ext` |
| `/(define "visible")/volume/tags` | %tags | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "visible")/volume/thickness` | 1 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "wheel")/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "wheel")/component/repairorder` | 10 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "wheel")/component/repairtime` | 10 | 미확인 | 45 ¦ 30 | 1 | `properties/vehicle.ext` |
| `/(define "wheel")/component/tags` | "wheel" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "wheel")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "wheel")/volume/component` | %0 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "wheel")/volume/tags` | "wheel" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "wheel")/volume/thickness` | 0.5 | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "wing")/component` | "wing%component" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "wing")/component/repairorder` | 1 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "wing")/component/repairtime` | 20 | 미확인 | 45 ¦ 30 | 1 | `properties/airborne.ext` |
| `/(define "wing")/component/tags` | "wing%component" "%tag" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "wing")/volume` | "wing%number" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/airborne.ext` |
| `/(define "wing")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/airborne.ext` |
| `/(define "wing")/volume/component` | "wing%component" | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "wing")/volume/tags` | "wing%component" "%tag" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/airborne.ext` |
| `/(define "wing")/volume/thickness` | 1 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/(define "wood_shield")/volume` | "shieldw%number" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/vehicle.ext` |
| `/(define "wood_shield")/volume/material` | "wood" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/(define "wood_shield")/volume/tags` | "wood" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/vehicle.ext` |
| `/(define "wood")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/car.ext` |
| `/(define "wood")/volume/material` | "wood" | 미확인 |  | 1 | `properties/car.ext` |
| `/(define "wood")/volume/tags` | "wood" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/car.ext` |
| `/##` | extender "detail" | 미확인 | No longer used? | 1 | `properties/vehicle.ext` |
| `/##weaponry/place` | "mgun" | 미확인 |  | 1 | `properties/cannon_mgun.ext` |
| `/##weaponry/place/charger` | "gunner" | 미확인 |  | 1 | `properties/cannon_mgun.ext` |
| `/##weaponry/place/gunner` | "gunner" | 미확인 |  | 1 | `properties/cannon_mgun.ext` |
| `/##weaponry/place/linkanimation` | "off" | 미확인 |  | 1 | `properties/cannon_mgun.ext` |
| `/##weaponry/place/linkbone` | "mgun" | 미확인 |  | 1 | `properties/cannon_mgun.ext` |
| `/##weaponry/place/type` | "main" | 미확인 | "unit tag" or "unit type" + "current main weapon tag" | 1 | `properties/cannon_mgun.ext` |
| `/able` | "emit_crew_on_injure" ¦ "-select" "neutral" ¦ "talk" ¦ "emit_on_attack" ¦ "talk" "-emit_passangers_on_enemy" ¦ "crew" "fight" "minimap" "personage" ¦ "-emit_passangers_on_enemy" ¦ "emit_crew_on_injure" "talk" "-emit_passangers_on_enemy" ¦ "emit_passangers_on_enemy" "repaired" "minimap" "ally_crew" | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 14 | `properties/ammo_box.ext` |
| `/animation/smooth` | 0.5 | 미확인 |  | 1 | `properties/animation/human/human_anm.ext` |
| `/armor/setting` | "body" | 미확인 |  | 3 | `properties/straw.ext` |
| `/armor/setting/thickness` | 20 | 미확인 |  | 2 | `properties/stuff_grenade_mine.ext` |
| `/armor/setting/threshold/blast/value` | 10 | 미확인 |  | 1 | `properties/straw.ext` |
| `/armor/setting/volumes` | "body*" | 미확인 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 2 | `properties/stuff_grenade_mine.ext` |
| `/autooperatable` | 0 | 미확인 |  | 1 | `properties/railway.ext` |
| `/boarder/anm` | "gunner" ¦ "commander" ¦ "driver1" ¦ "driver2" ¦ "driver" | 미확인 |  | 4 | `properties/ammo_box.ext` |
| `/boarder/anm/forward/base` | "up" | 미확인 | seconds : CP | 1 | `properties/bicycle.ext` |
| `/boarder/anm/forward/begin` | "board_moto_driver" | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/boarder/anm/forward/end` | "ammo_box_gunner" ¦ "ammo_box_commander" ¦ "ammo_box_commander_push" ¦ "ammo_box_gunner_push" ¦ "seat_bicycle_driver" ¦ "pose_mgun_stan" ¦ "pose_mg_stan_charger" ¦ "mgun_browning_driver_r" ¦ "mgun_browning_driver_l" | 미확인 |  | 4 | `properties/ammo_box.ext` |
| `/boarder/anm/reverse/end` | "board_moto_driver" -1 | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/boarder/link` | "driver1" "gunner" ¦ "driver2" "commander" | 미확인 | { link <from> <to> } | 3 | `properties/ammo_box.ext` |
| `/body/permit` | body | 미확인 |  | 2 | `properties/stuff_dynamite.ext` |
| `/body/proper` | body sensible | 미확인 |  | 2 | `properties/stuff_bomb.ext` |
| `/bone` | "foresight2" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/human.ext` |
| `/bone/speed` | 5 | 미확인 | sway speed coef ¦ kmph | 1 | `properties/human.ext` |
| `/brain` | "vehicle" ¦ "actor" ¦ "human" | 미확인 |  | 3 | `properties/airborne.ext` |
| `/brain/const` | "bomb/detect_explosives" 7 ¦ "bomb/detect_grenades" 5 ¦ "bomb/danger_zone" 7 ¦ "fight/distance" 2 ¦ "weapon/change_time" 5 | 미확인 | hide if danger is more than "x" meters away, otherwise run away ¦ disable ai weapon switch for 5 seconds | 1 | `properties/human.ext` |
| `/brain/properties/advance_ratio` | 0.5 | 미확인 |  | 1 | `properties/human.ext` |
| `/brain/properties/retreat_ratio` | 4 | 미확인 |  | 1 | `properties/human.ext` |
| `/brain/state` | "airborne" ¦ "animal" | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 2 | `properties/airborne.ext` |
| `/chassis` | "wheel" ¦ "airborne" ¦ "human_carrier" ¦ "foot" ¦ "water" ¦ "railway" ¦ "track" | 미확인 |  | 12 | `properties/airborne.ext` |
| `/chassis/("mobility_cannon")` | speed ( 14 ) weight ( 32 ) ¦ speed ( 12 ) weight ( 32 ) | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/chassis/("mobility_car")` | speed ( 40 ) speedatmaxturn ( 20 ) reverse ( 5 ) weight ( 0.075 ) power ( 1.3 ) fuel ( 0.1 ) type ( fuel ) range ( 1000 ) | 미확인 |  | 1 | `properties/bicycle.ext` |
| `/chassis/crawl/fast` | 6 9 180 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/human.ext` |
| `/chassis/crawl/normal` | 4.5 7 180 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/human.ext` |
| `/chassis/crawl/slow` | 3.5 6 180 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/human.ext` |
| `/chassis/elevationacceleration` | 8 | 미확인 | how quickly aircraft can change from level flight to full climb or dive. (m/s^2) | 1 | `properties/airborne.ext` |
| `/chassis/fuelbag/fuel` | "fuel" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 4 | `properties/airborne.ext` |
| `/chassis/fuelbag/rate` | 0 ~ 10 | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 4 | `properties/airborne.ext` |
| `/chassis/fuelbag/remain` | 1 ~ 500 | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 4 | `properties/airborne.ext` |
| `/chassis/fuelbag/volume` | 1 ~ 500 | 미확인 | Main Body ¦ Truck Bed | 4 | `properties/airborne.ext` |
| `/chassis/injured` | 0.85 | 미확인 | speed multiplier when health is below 30% of maximum | 1 | `properties/human.ext` |
| `/chassis/locomotion/braketime` | 2.5 ~ 3 | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/maxspeed` | 40 ~ 75 | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/maxspeedatmaxturn` | 15 | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/splineradius` | 10 | 미확인 | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/starttime` | 8 ~ 20 | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/steeroff` | 40 | 미확인 | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/steeron` | 15 | 미확인 | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/stoptime` | 4 ~ 5 | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/airborne.ext` |
| `/chassis/locomotion/turnradius` | 10 ~ 50 | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/airborne.ext` |
| `/chassis/maxairbornealtitude` | 100 | 미확인 |  | 1 | `properties/airborne.ext` |
| `/chassis/maxelevationspeed` | 8 | 미확인 | climb rate, how many meters per second aircraft changes altitude when in full climb or dive | 1 | `properties/airborne.ext` |
| `/chassis/maxsideslope` | 15 | 미확인 | limit in degrees that aircraft can roll while in a turn | 1 | `properties/airborne.ext` |
| `/chassis/run/bow` | 16 5.0 600 | 미확인 | mission script movement mode | 1 | `properties/human.ext` |
| `/chassis/run/fast` | 25 25 500 ¦ 18 5.8 600 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 2 | `properties/animal.ext` |
| `/chassis/run/normal` | 7 7 200 ¦ 13 6.2 600 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 2 | `properties/animal.ext` |
| `/chassis/run/slow` | 3 3 100 ¦ 8 3.2 360 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 2 | `properties/animal.ext` |
| `/chassis/run/sneak` | 5 2 360 | 미확인 | mission script movement mode | 1 | `properties/human.ext` |
| `/chassis/run/spot` | 6 2 420 | 미확인 | Manual TPC Zoom | 1 | `properties/human.ext` |
| `/chassis/run/walk` | 4 1.8 600 | 미확인 | single flick, walk mode | 1 | `properties/human.ext` |
| `/chassis/sideslopespeed` | 45 | 미확인 | how quickly in degrees per second that aircraft rolls | 1 | `properties/airborne.ext` |
| `/chassis/speed/fast` | 20 ~ 100 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/airborne.ext` |
| `/chassis/speed/normal` | 15 ~ 80 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/airborne.ext` |
| `/chassis/speed/slow` | 10 ~ 60 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/airborne.ext` |
| `/chassis/strafe/back` | 8 0 0 10.5 1.22 "run_back_spot_gun_up" "run_back_spot_gun_down" | 미확인 |  | 1 | `properties/human.ext` |
| `/chassis/strafe/forth` | 3 2 0 6.5 3 "walk_spot_gun_up_1" "walk_spot_gun_down_1" ¦ 3 2 1 11.0 1 "run_spot_gun_up" "run_spot_gun_down" ¦ 8 0 0 13.2 1.2 "run_spot_gun_up" "run_spot_gun_down" | 미확인 | 6.5 km/h move 2 km/h animation 0.37 scale --> in Run | 1 | `properties/human.ext` |
| `/chassis/strafe/left` | 4 0 0 10.5 1.22 "run_spot_back-strafe_gun_up" "run_spot_back-strafe_gun_down" | 미확인 |  | 1 | `properties/human.ext` |
| `/chassis/strafe/right` | 8 0 0 13.0 1.1 "run_spot_left-strafe_gun_up" "run_spot_left-strafe_gun_down" | 미확인 |  | 1 | `properties/human.ext` |
| `/chassis/swim/fast` | 5 5 180 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/human.ext` |
| `/chassis/swim/normal` | 3 5 180 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/human.ext` |
| `/chassis/swim/slow` | 2 5 180 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/human.ext` |
| `/chassismanager/current` | "wheel" | 미확인 | current stamina ¦ current resources | 1 | `properties/airborne.ext` |
| `/collider` | 예: "airborne" (13종 이상) | 미확인 |  | 28 | `properties/airborne.ext` |
| `/constrain` | ground ¦ none | 미확인 |  | 11 | `properties/ammo_box.ext` |
| `/extender[cannon]` | "cannon" | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/extender[cannon]/animation/aim` | "" "" | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/extender[cannon]/animation/fire` | "" "" | 미확인 | adds heating to unit (potential to set other items on fire due to proximity) | 3 | `properties/ammo_box.ext` |
| `/extender[cannon]/animation/move` | "ammo_box_commander_push" "ammo_box_gunner_push" ¦ "mgun_browning_driver_r" "mgun_browning_driver_l" | 미확인 | move state: cannon being pushed and which positions play animations | 3 | `properties/ammo_box.ext` |
| `/extender[cannon]/animation/reload` | "" "" ¦ "mgun_stan_charger" "" | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/extender[cannon]/animation/switch` | "close" | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/extender[cannon]/attack/obstacle` | "open" | 미확인 |  | 1 | `properties/cannon_mgun.ext` |
| `/extender[cannon]/attack/workers` | "gunner" "commander" | 미확인 |  | 2 | `properties/ammo_box.ext` |
| `/extender[cannon]/move/obstacle` | "close" | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/extender[cannon]/move/workers` | "driver1" "driver2" minWorkers 2 | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/extender[inventory]` | "inventory" | 미확인 |  | 15 | `properties/airborne.ext` |
| `/extender[inventory]/armor` | "head" ¦ "body" ¦ "accessories" | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[inventory]/armor/bone` | "head" ¦ "IK_UpDown" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/human.ext` |
| `/extender[inventory]/armor/heatdamping` | 0.6 | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[inventory]/armor/mask` | "head" ¦ "body" ¦ "head2" | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[inventory]/armor/pierceabsorb` | 20 ~ 25 | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[inventory]/armor/volumes` | "head" ¦ "body" | 미확인 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 1 | `properties/human.ext` |
| `/extender[inventory]/box/size` | 9 10 ¦ 6 10 ¦ 6 2 ¦ 9 6 ¦ 8 8 ¦ 8 12 ¦ 9 15 ¦ 9 8 ¦ 4 4 ¦ 9 12 | 미확인 | size of the window | 15 | `properties/airborne.ext` |
| `/extender[inventory]/box/weight` | 100 ~ 20000 | 미확인 |  | 14 | `properties/airborne.ext` |
| `/extender[inventory]/weapon` | "mgun" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/extender[inventory]/weapon/mask` | "mountable" | 미확인 |  | 1 | `properties/vehicle.ext` |
| `/extender[mine]` | "mine" | 미확인 |  | 2 | `properties/stuff_grenade_mine.ext` |
| `/extender[resources]` | "resources" | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[resources]/resources/current` | 10 | 미확인 | current stamina ¦ current resources | 1 | `properties/human.ext` |
| `/extender[resources]/resources/income` | 0.1 | 미확인 | resources regeneration per second | 1 | `properties/human.ext` |
| `/extender[resources]/resources/max` | 10 | 미확인 | max health ¦ max resources | 1 | `properties/human.ext` |
| `/extender[stuff]` | "stuff" | 미확인 |  | 5 | `properties/barrel.ext` |
| `/extender[stuff]/item` | "barrel" ¦ "human" | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 2 | `properties/barrel.ext` |
| `/extender[talk]` | "talk" | 미확인 |  | 10 | `properties/ammo_box.ext` |
| `/extender[talk]/talker` | "cannon" ¦ "tank" ¦ "human" | 미확인 |  | 10 | `properties/ammo_box.ext` |
| `/extender[vitality]` | "vitality" | 미확인 |  | 2 | `properties/animal.ext` |
| `/extender[vitality]/damage/"*"` | 예: "head" 1.3 (13종 이상) | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[vitality]/damageblast` | 1.00 ~ 1.2 | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[vitality]/damagetpc/"*"` | 예: "head" 1.75 (13종 이상) | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[vitality]/health/current` | 100 | 미확인 | current stamina ¦ current resources | 1 | `properties/human.ext` |
| `/extender[vitality]/health/inc` | 1.0 | 미확인 | health regeneration per second ¦ stamina regeneration/usage per second (depending on movement mode and load) | 1 | `properties/human.ext` |
| `/extender[vitality]/health/max` | 100 | 미확인 | max health ¦ max resources | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/current` | 100 | 미확인 | current stamina ¦ current resources | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/inc/fast` | -3 -3 -3 -3 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/inc/normal` | +2 +2 +1.5 +1 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/inc/slow` | +3 +3 +2.5 +2 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/inc/stay` | +3 +3 +2.5 +2 | 미확인 |  | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/inc/walk` | +3 +3 +2.5 +2 | 미확인 | single flick, walk mode | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/load` | 0 25 50 75 | 미확인 | equipment weight (in kg) | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/max` | 100 | 미확인 | max health ¦ max resources | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/movement_limits/fast` | 1 10 | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/movement_limits/normal` | 0 0 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/human.ext` |
| `/extender[vitality]/stamina/movement_limits/slow` | -1 -1 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/human.ext` |
| `/extension` | "human_fsm.ext" | 미확인 |  | 1 | `properties/human.ext` |
| `/fsm` | "animal" ¦ "human" | 미확인 |  | 2 | `properties/animal.ext` |
| `/fsm/request` | "dead" ¦ "idle" ¦ "move" ¦ "rotate" ¦ "stop-move" ¦ "switch-move" ¦ "swim" | 미확인 |  | 1 | `properties/animal.ext` |
| `/fsm/request/duration` | 2 | 미확인 |  | 1 | `properties/animal.ext` |
| `/fsm/request/exit` | "stop-move" | 미확인 |  | 1 | `properties/animal.ext` |
| `/fsm/request/priority` | 0 ~ 50 | 미확인 |  | 1 | `properties/animal.ext` |
| `/fsm/start` | "stand" | 미확인 | games played : starting CP of each match | 1 | `properties/animal.ext` |
| `/fsm/state` | "stand" ¦ "walk" ¦ "switch_move" ¦ "idle" ¦ "dying" ¦ "dead" | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 1 | `properties/animal.ext` |
| `/fsm/state/from` | "stand" ¦ "walk" ¦ "switch_move" ¦ "idle" | 미확인 |  | 1 | `properties/animal.ext` |
| `/fsm/state/tag` | "stop-move" ¦ "rotate" ¦ "stand" ¦ "swim" ¦ "move" ¦ "switch-move" ¦ "idle" ¦ "dying" ¦ "dead" ¦ "stored" | 미확인 | hybrid AT grenade / mines ¦ thrower hand weapon | 1 | `properties/animal.ext` |
| `/fsm/state/to` | "walk" ¦ "idle" ¦ "stand" ¦ "switch_move" | 미확인 |  | 1 | `properties/animal.ext` |
| `/fsm/state/transit` | "dead" | 미확인 |  | 1 | `properties/animal.ext` |
| `/fsm/subset` | "default" | 미확인 |  | 1 | `properties/animal.ext` |
| `/hearingbias` | 0 ~ 250.0 | 미확인 | reduces units ability to hear sound signals. value = meters ¦ in meters | 4 | `properties/airborne.ext` |
| `/lodclass` | "vehicle_plane" ¦ "cannon" ¦ "human" ¦ "decor_medium" ¦ "part_small" ¦ "stuff_small" ¦ "decor_landmark" ¦ "stuff_weapon" ¦ "stuff_grenade" ¦ "stuff_mine" ¦ "vehicle" | 미확인 |  | 25 | `properties/airborne.ext` |
| `/mass` | 1 ~ 500 | 미확인 | 65kg human + 5kg bike | 7 | `properties/ammo_box.ext` |
| `/nogroupmesh` | 1 | 미확인 |  | 5 | `properties/human.ext` |
| `/obstacleid` | "tank" ¦ "human" ¦ "construction" ¦ "stuff" ¦ "marine" ¦ "nothing" | 미확인 |  | 14 | `properties/ammo_box.ext` |
| `/patherid` | "cannon.carried" ¦ "animal" ¦ "armored" ¦ "bicycle" ¦ "cannon.small" ¦ "human" ¦ "water" ¦ "railway" ¦ "tank" ¦ "vehicle.large" | 미확인 |  | 13 | `properties/ammo_box.ext` |
| `/perks/blastwave_modifier` | 5.0 ~ 20.0 | 미확인 |  | 13 | `properties/airborne.ext` |
| `/perks/component` | "body" | 미확인 |  | 2 | `properties/ammo_box.ext` |
| `/perks/component/health/hp` | 60 ~ 120 | 미확인 |  | 2 | `properties/ammo_box.ext` |
| `/perks/component/health/hp_broken` | 1 | 미확인 | destroys component and cannot be repaired | 2 | `properties/ammo_box.ext` |
| `/placer/place` | "driver" | 미확인 |  | 1 | `properties/car.ext` |
| `/placer/place/usevision` | 0 | 미확인 |  | 1 | `properties/car.ext` |
| `/placer/place/visor` | "main" | 미확인 |  | 1 | `properties/car.ext` |
| `/props` | 예: "airborne" "engine" (13종 이상) | 미확인 |  | 34 | `properties/airborne.ext` |
| `/sensor/attackzone` | 2.5 | 미확인 |  | 1 | `properties/human.ext` |
| `/sensor/informrange` | 80 100 ¦ 50 75 | 미확인 | first = AI , second = USER | 2 | `properties/human.ext` |
| `/sensor/minrecognizetime` | 0.01 | 미확인 |  | 1 | `properties/human.ext` |
| `/sensor/recognizetime` | 0.01 | 미확인 |  | 2 | `properties/human.ext` |
| `/sensor/visor` | "main" ¦ "around" ¦ "eyes" ¦ "driver" ¦ "human" ¦ "none" ¦ "hearing" ¦ "peripheral" ¦ "telescope" | 미확인 |  | 12 | `properties/airborne.ext` |
| `/sensor/visor/bone` | "visor" ¦ "basis" ¦ "foresight1" ¦ "visor2" ¦ "visor1" ¦ "visor3" ¦ "hearing_bone" ¦ "foresight3" | 미확인 | Land;"}} ¦ "}} | 11 | `properties/airborne.ext` |
| `/sensor/visor/vision` | 예: "airborne_main_mp" (13종 이상) | 미확인 |  | 12 | `properties/airborne.ext` |
| `/simulation/able` | 0 ~ 1 | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 14 | `properties/ammo_box.ext` |
| `/simulation/disablelinked` | 1 | 미확인 |  | 6 | `properties/ammo_box.ext` |
| `/simulation/state` | 0 ~ 1 | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 3 | `properties/human.ext` |
| `/simulatorid` | "airborne" ¦ "human" ¦ "water" ¦ "grenade" | 미확인 |  | 6 | `properties/airborne.ext` |
| `/skeleton/bone` | "hearing_bone" ¦ "basis" ¦ "mined_view" | 미확인 | Land;"}} ¦ "}} | 7 | `properties/armoredcar.ext` |
| `/skeleton/bone/matrix34` | 1 0 0 0 -1 0 0 0 1 0 0 0 ¦ 0 0 1 0 -1 0 1 0 0 0 0 0 | 미확인 |  | 3 | `properties/piece.ext` |
| `/skeleton/bone/position` | 0 0 50 ¦ 0 0 40 ¦ 0 0 70 | 미확인 |  | 4 | `properties/armoredcar.ext` |
| `/targetclass` | "airborne" ¦ "cannon_bullet" ¦ "armoredcar" "bullet" ¦ "car" ¦ "cannon_small" "ap fg" ¦ "human" ¦ "tank_medium" "ap fg bullet" ¦ "howitzer" | 미확인 |  | 14 | `properties/airborne.ext` |
| `/targetselector` | "airborne_bullet" ¦ "cannon_bullet" ¦ "tank_zenite" ¦ "car_bullet" ¦ "cannon_small" ¦ "human" ¦ "cannon_zenite" ¦ "cannon_aa" ¦ "tank_medium" | 미확인 |  | 14 | `properties/airborne.ext` |
| `/volume` | 예: "body" (13종 이상) | 미확인 | Main Body ¦ Truck Bed | 20 | `properties/ammo_box.ext` |
| `/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 16 | `properties/barrel.ext` |
| `/volume/able/contact` | 0 | 미확인 |  | 4 | `properties/decor.ext` |
| `/volume/able/nolook` | 1 | 미확인 |  | 1 | `properties/human.ext` |
| `/volume/able/obstacle` | 0 | 미확인 |  | 5 | `properties/decor.ext` |
| `/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 4 | `properties/decor.ext` |
| `/volume/density` | 0 | 미확인 |  | 8 | `properties/barrel.ext` |
| `/volume/material` | "fabric" | 미확인 |  | 1 | `properties/stuff_small_bag.ext` |
| `/volume/tags` | "fire" ¦ "no_target" "fire" ¦ "body" "fire" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 2 | `properties/human.ext` |
| `/volume/thickness` | 0 ~ 5 | 미확인 |  | 11 | `properties/ammo_box.ext` |
| `/weaponry/manualaccuracy` | 1.00 | 미확인 |  | 2 | `properties/human.ext` |
| `/weaponry/manualaccuracyzoom` | 1.25 ~ 1.50 | 미확인 |  | 2 | `properties/human.ext` |
| `/weaponry/place` | "mgun" ¦ "gun" ¦ "hand_right" | 미확인 |  | 6 | `properties/ammo_box.ext` |
| `/weaponry/place/aimbone` | "aim_ref" | 미확인 |  | 1 | `properties/human.ext` |
| `/weaponry/place/charger` | "gunner" | 미확인 |  | 2 | `properties/ammo_box.ext` |
| `/weaponry/place/gunner` | "gunner" | 미확인 |  | 2 | `properties/cannon.ext` |
| `/weaponry/place/linkanimation` | "off" ¦ "on" | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/weaponry/place/linkbone` | "mgun" ¦ "right_hand" | 미확인 |  | 3 | `properties/ammo_box.ext` |
| `/weaponry/place/sequentialaim` | 0 | 미확인 | was at 1 | 1 | `properties/human.ext` |

## 연관: 공통 속성 (.inc)

- 위치: `properties/**/*.inc`
- 연결: 본체와 공통 속성 `.ext`의 `(include "/properties/...")`. 같은 차종의 모든 유닛이 공유한다

### 뜻을 모르는 속성 (605개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/("base_vol")` | args 1 ¦ args 2 ¦ args 3 ¦ args 4 ¦ args 5 ¦ args 6 ¦ args 7 ¦ args 8 ¦ args 9 | 미확인 |  | 1 | `properties/tree.inc` |
| `/("bole_vol")` | args 1 ¦ args 2 ¦ args 3 ¦ args 4 ¦ args 5 ¦ args 6 ¦ args 7 ¦ args 8 ¦ args 9 | 미확인 |  | 1 | `properties/tree.inc` |
| `/("cliff_vol")` | 예: args "body1" (13종 이상) | 미확인 |  | 1 | `properties/cliff.inc` |
| `/("do_healing_lie")` | args head ¦ args leg ¦ args hand | 미확인 |  | 1 | `properties/human_fsm/special.inc` |
| `/("do_healing")` | args head ¦ args leg ¦ args hand | 미확인 |  | 1 | `properties/human_fsm/special.inc` |
| `/("look_chain")` | args left ¦ args right ¦ args sleft ¦ args sright | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/("peep_chain")` | args left ¦ args right | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "abm_dymamic_aa")/("accuracy_by_motion")` | max_linear ( 10 ) accuracy_by_linear ( 0.4 ) max_angular ( 12 ) accuracy_by_angular ( 0.4 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_aaa")/("accuracy_by_motion")` | max_linear ( 10 ) accuracy_by_linear ( 0.25 ) max_angular ( 10 ) accuracy_by_angular ( 0.25 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_autocannon")/("accuracy_by_motion")` | max_linear ( 10 ) accuracy_by_linear ( 0.30 ) max_angular ( 12 ) accuracy_by_angular ( 0.35 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_gun_boat")/("accuracy_by_motion")` | max_linear ( 30 ) accuracy_by_linear ( 0.5 ) max_angular ( 15 ) accuracy_by_angular ( 0.5 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_gun_poor")/("accuracy_by_motion")` | max_linear ( 1 ) accuracy_by_linear ( 0.25 ) max_angular ( 1 ) accuracy_by_angular ( 0.25 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_gun")/("accuracy_by_motion")` | max_linear ( 5 ) accuracy_by_linear ( 0.5 ) max_angular ( 8 ) accuracy_by_angular ( 0.5 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_mortar")/("accuracy_by_motion")` | max_linear ( 5 ) accuracy_by_linear ( 0.5 ) max_angular ( 8 ) accuracy_by_angular ( 0.5 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_poor")/("accuracy_by_motion")` | max_linear ( 10 ) accuracy_by_linear ( 0.25 ) max_angular ( 5 ) accuracy_by_angular ( 0.25 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_rocket")/("accuracy_by_motion")` | max_linear ( 5 ) accuracy_by_linear ( 0.4 ) max_angular ( 8 ) accuracy_by_angular ( 0.4 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_stabilised")/("accuracy_by_motion")` | max_linear ( 20 ) accuracy_by_linear ( 0.55 ) max_angular ( 10 ) accuracy_by_angular ( 0.55 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic_td")/("accuracy_by_motion")` | max_linear ( 10 ) accuracy_by_linear ( 0.4 ) max_angular ( 8 ) accuracy_by_angular ( 0.50 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_dymamic")/("accuracy_by_motion")` | max_linear ( 10 ) accuracy_by_linear ( 0.25 ) max_angular ( 8 ) accuracy_by_angular ( 0.35 ) disorder_time ( %dispersion ) order_time ( %zeroing ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_human")/("accuracy_by_motion")` | max_linear ( 15 ) accuracy_by_linear ( 0.4 ) max_angular ( 25 ) accuracy_by_angular ( 0.4 ) disorder_time ( 0.5 ) order_time ( 1.0 ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_mgun_boat")/("accuracy_by_motion")` | max_linear ( 30 ) accuracy_by_linear ( 0.4 ) max_angular ( 30 ) accuracy_by_angular ( 0.5 ) disorder_time ( 0.6 ) order_time ( 2 ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_mgun")/("accuracy_by_motion")` | max_linear ( 8 ) accuracy_by_linear ( 0.4 ) max_angular ( 30 ) accuracy_by_angular ( 0.5 ) disorder_time ( 0.6 ) order_time ( 2 ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "abm_rear_mg")/("accuracy_by_motion")` | max_linear ( 50 ) accuracy_by_linear ( 0.8 ) max_angular ( 5 ) accuracy_by_angular ( 0.4 ) disorder_time ( 1 ) order_time ( 2 ) | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "accuracy_by_motion")/accuracybymotion/byangularspeed` | %accuracy_by_angular | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "accuracy_by_motion")/accuracybymotion/bylinearspeed` | %accuracy_by_linear | 미확인 |  | 1 | `properties/abm.inc` |
| `/(define "accuracy_by_motion")/accuracybymotion/disordertime` | %disorder_time | 미확인 | s | 1 | `properties/abm.inc` |
| `/(define "accuracy_by_motion")/accuracybymotion/maxangularspeed` | %max_angular | 미확인 | deg/s | 1 | `properties/abm.inc` |
| `/(define "accuracy_by_motion")/accuracybymotion/maxlinearspeed` | %max_linear | 미확인 | km/h | 1 | `properties/abm.inc` |
| `/(define "accuracy_by_motion")/accuracybymotion/ordertime` | %order_time | 미확인 | s | 1 | `properties/abm.inc` |
| `/(define "add_climb_bones")/bone` | "%0" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/construction.inc` |
| `/(define "add_climb_bones")/bone/bone` | "left_%0" ¦ "right_%0" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/construction.inc` |
| `/(define "add_climb_bones")/bone/bone/matrix34/pos` | 8 8 0 ¦ 8 -8 0 | 미확인 |  | 1 | `properties/construction.inc` |
| `/(define "add_climb_bones")/bone/bone/matrix34/xform` | 0 -0.79 0 ¦ 0 0.79 0 | 미확인 |  | 1 | `properties/construction.inc` |
| `/(define "armor_75mm")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_75mm")/armor/setting/threshold/blast/value` | 15 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_adjustable")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_adjustable")/armor/setting/threshold/blast/value` | %thresh_fg ¦ 10 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_adjustable")/armor/setting/threshold/pierce/calibre` | %calibre | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_adjustable")/armor/setting/threshold/pierce/value` | %thresh_ap | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_adjustable")/armor/setting/volumes` | "body*" ¦ "box*" ¦ "wire*" ¦ "wires*" ¦ "wood*" ¦ "door*" ¦ "roof*" ¦ "floor*" | 미확인 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 1 | `properties/construction_generic.inc` |
| `/(define "armor_bunker_medium")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_bunker_medium")/armor/setting/threshold/blast/value` | 35 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_construction")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_construction")/armor/setting/threshold/pierce/koef` | 1 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_construction")/armor/setting/volumes` | "body*" ¦ "box*" ¦ "wire*" ¦ "wires*" ¦ "wood*" ¦ "door*" ¦ "roof*" ¦ "floor*" | 미확인 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 1 | `properties/construction_generic.inc` |
| `/(define "armor_disable_damage_acc")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_dragontooth")/("armor_adjustable")` | thresh_fg ( 13 ) thresh_ap ( 60 ) calibre ( 37 ) | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_fence_wood")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_fence_wood")/armor/setting/threshold/blast/value` | 4.75 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_fragile")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_fragile")/armor/setting/threshold/blast/value` | 2 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_glass")/("armor_adjustable")` | thresh_fg ( 6 ) thresh_ap ( 15 ) calibre ( 0 ) | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_heavy")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_heavy")/armor/setting/threshold/blast/value` | 22 ~ 24 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_hedgehog")/("armor_adjustable")` | thresh_fg ( 35 ) thresh_ap ( 80 ) calibre ( 57 ) | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_light")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_light")/armor/setting/threshold/blast/value` | 10 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_medium")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_medium")/armor/setting/threshold/blast/value` | 12 ~ 13 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_post")/armor/setting` | "body" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_post")/armor/setting/threshold/blast/value` | 10 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_sandbag_woodbeam")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_sandbag_woodbeam")/armor/setting/threshold/blast/value` | 10 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_sandbag")/("armor_adjustable")` | thresh_fg ( 10 ) thresh_ap ( 15 ) calibre ( 30 ) | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_stack")/("armor_adjustable")` | thresh_fg ( 10 ) thresh_ap ( 15 ) calibre ( 15 ) | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_use_volume_thickness")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "armor_use_volume_thickness")/armor/setting/ray_cast_thickness` | 0 | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "base_vol")/volume` | "base%0" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/tree.inc` |
| `/(define "base_vol")/volume/able/touch` | 0 | 미확인 |  | 1 | `properties/tree.inc` |
| `/(define "base_vol")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/tree.inc` |
| `/(define "base_vol")/volume/tags` | "bole" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/tree.inc` |
| `/(define "base_vol")/volume/thickness` | 10 | 미확인 |  | 1 | `properties/tree.inc` |
| `/(define "blast_protection")/armor/setting` | "body" ¦ "roof" | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "blast_protection")/armor/setting/threshold/blast/value` | %dmg | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "body_vol")/volume` | "%0" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/construction_generic.inc` |
| `/(define "body_vol")/volume/able/visible` | %1 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/construction_generic.inc` |
| `/(define "body_vol")/volume/tags` | "body" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/construction_generic.inc` |
| `/(define "bole_vol")/volume` | "bole%0" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/tree.inc` |
| `/(define "bole_vol")/volume/able/obstacle` | 0 | 미확인 |  | 1 | `properties/tree.inc` |
| `/(define "bole_vol")/volume/able/touch` | 0 | 미확인 |  | 1 | `properties/tree.inc` |
| `/(define "bole_vol")/volume/able/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/tree.inc` |
| `/(define "bole_vol")/volume/tags` | "bole" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/tree.inc` |
| `/(define "bole_vol")/volume/thickness` | 10 | 미확인 |  | 1 | `properties/tree.inc` |
| `/(define "bonealign")/bone` | %0 | 미확인 | Land;"}} ¦ "}} | 1 | `properties/trench.inc` |
| `/(define "bonealign")/bone/parameters` | "Align;" | 미확인 | base_release_time=20;ammo_table=400;flag_capture_time=60;flag_release_time=20;flag_capture_factor=1.0;supply_waves=1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35,39;"} ¦ points_table=0/0.000,0.05/0.75,0.50/1.00,0.68/1.25,0.81/1.50,1.00/3;flag_capture_time=60;flag_release_time=10;flag_capture_factor=0.9;invertedUnitScores=true;scoreStart=250;teamsize_cp_factor=1:1.3,2:1.2,3:1.1,4:1.0"} | 1 | `properties/trench.inc` |
| `/(define "boneland")/bone` | %0 | 미확인 | Land;"}} ¦ "}} | 2 | `properties/lump.inc` |
| `/(define "boneland")/bone/parameters` | "Ground;Land;" ¦ "Ground;Land;Align;" | 미확인 | base_release_time=20;ammo_table=400;flag_capture_time=60;flag_release_time=20;flag_capture_factor=1.0;supply_waves=1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35,39;"} ¦ points_table=0/0.000,0.05/0.75,0.50/1.00,0.68/1.25,0.81/1.50,1.00/3;flag_capture_time=60;flag_release_time=10;flag_capture_factor=0.9;invertedUnitScores=true;scoreStart=250;teamsize_cp_factor=1:1.3,2:1.2,3:1.1,4:1.0"} | 2 | `properties/lump.inc` |
| `/(define "boneland2")/bone` | %0 | 미확인 | Land;"}} ¦ "}} | 1 | `properties/trench.inc` |
| `/(define "boneland2")/bone/parameters` | "Ground;Land;" | 미확인 | base_release_time=20;ammo_table=400;flag_capture_time=60;flag_release_time=20;flag_capture_factor=1.0;supply_waves=1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35,39;"} ¦ points_table=0/0.000,0.05/0.75,0.50/1.00,0.68/1.25,0.81/1.50,1.00/3;flag_capture_time=60;flag_release_time=10;flag_capture_factor=0.9;invertedUnitScores=true;scoreStart=250;teamsize_cp_factor=1:1.3,2:1.2,3:1.1,4:1.0"} | 1 | `properties/trench.inc` |
| `/(define "cliff_vol")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/cliff.inc` |
| `/(define "cliff_vol")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 1 | `properties/cliff.inc` |
| `/(define "crate_ammo_heavy")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/extender[supply_zone]/limit` | 100 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/extender[supply_zone]/radius` | 15 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/extender[supply_zone]/regenerationperiod` | 0 ~ 10 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "crate_ammo_heavy")/extender[supply_zone]/resupplyperiod` | 10 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "declare_fire_volume")/volume` | "fire%0" ¦ "smoke%0" | 미확인 | Main Body ¦ Truck Bed | 2 | `properties/burning.inc` |
| `/(define "declare_fire_volume")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 2 | `properties/burning.inc` |
| `/(define "declare_fire_volume")/volume/tags` | "fire" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 2 | `properties/burning.inc` |
| `/(define "declare_fire_volumes")/("declare_fire_volume")` | args 0 ¦ args 1 ¦ args 2 ¦ args 3 ¦ args 4 ¦ args 5 ¦ args 6 ¦ args 7 ¦ args 8 ¦ args 9 | 미확인 |  | 2 | `properties/burning.inc` |
| `/(define "declare_fire_volumes")/props` | "burning_old" | 미확인 |  | 2 | `properties/burning.inc` |
| `/(define "do_healing_lie")/state` | "lie_healing_%0_begin" ¦ "lie_healing_%0" ¦ "lie_healing_%0_end" | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 1 | `properties/human_fsm/special.inc` |
| `/(define "do_healing_lie")/state/from` | "lie" | 미확인 |  | 1 | `properties/human_fsm/special.inc` |
| `/(define "do_healing_lie")/state/tag` | "down" ¦ "healing" ¦ "heal-%0" | 미확인 | hybrid AT grenade / mines ¦ thrower hand weapon | 1 | `properties/human_fsm/special.inc` |
| `/(define "do_healing_lie")/state/to` | "lie_healing_%0" ¦ "lie_healing_%0_end" ¦ "lie" | 미확인 |  | 1 | `properties/human_fsm/special.inc` |
| `/(define "do_healing")/state` | "healing_%0_begin" ¦ "healing_%0" ¦ "healing_%0_end" | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 1 | `properties/human_fsm/special.inc` |
| `/(define "do_healing")/state/from` | "squat" | 미확인 |  | 1 | `properties/human_fsm/special.inc` |
| `/(define "do_healing")/state/tag` | "squat" ¦ "healing" ¦ "heal-%0" | 미확인 | hybrid AT grenade / mines ¦ thrower hand weapon | 1 | `properties/human_fsm/special.inc` |
| `/(define "do_healing")/state/to` | "healing_%0" ¦ "healing_%0_end" ¦ "squat" | 미확인 |  | 1 | `properties/human_fsm/special.inc` |
| `/(define "flag_ammo_heavy")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/extender[supply_zone]/limit` | 500 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/extender[supply_zone]/radius` | 15 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/extender[supply_zone]/regenerationperiod` | 0 ~ 10 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo_heavy")/extender[supply_zone]/resupplyperiod` | 10 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/extender[supply_zone]/limit` | 250 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/extender[supply_zone]/radius` | 15 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/extender[supply_zone]/regenerationperiod` | 0 ~ 10 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "flag_ammo")/extender[supply_zone]/resupplyperiod` | 5 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "flight_model")/elevationacceleration` | ( + ( / %climb_speed 4 ) 2 ) | 미확인 | how quickly aircraft can change from level flight to full climb or dive. (m/s^2) | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/fuelbag/fuel` | "fuel" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/fuelbag/rate` | ( * 20 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/locomotion/fast` | ( * 1.0 ( / ( * %speed %power ) ( * %weight %aerodynamics ) ) ) ¦ ( * 1.1 ( / ( * %speed %power ) ( * %weight %aerodynamics ) ) ) | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/locomotion/maxspeed` | ( * 1.0 ( / ( * %speed %power ) ( * %weight %aerodynamics ) ) ) ¦ ( * 1.1 ( / ( * %speed %power ) ( * %weight %aerodynamics ) ) ) | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/locomotion/normal` | ( * 1.0 ( / ( * %speed %power ) ( * %weight %aerodynamics ) ) ) ¦ ( * 1.1 ( / ( * %speed %power ) ( * %weight %aerodynamics ) ) ) | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/locomotion/slow` | ( * 0.7 ( / ( * %speed %power ) ( * %weight %aerodynamics ) ) ) | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/locomotion/turnradius` | ( * 0.3 %turn ) ¦ ( * 0.3 %turnMP ) | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/maxelevationspeed` | ( + ( / %climb_speed 6 ) 2 ) | 미확인 | climb rate, how many meters per second aircraft changes altitude when in full climb or dive | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/maxsideslope` | %roll_angle | 미확인 | limit in degrees that aircraft can roll while in a turn | 1 | `properties/mobility.inc` |
| `/(define "flight_model")/sideslopespeed` | %roll_rate ¦ ( * 2 %roll_rate ) | 미확인 | how quickly in degrees per second that aircraft rolls | 1 | `properties/mobility.inc` |
| `/(define "foxhole")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/trench.inc` |
| `/(define "foxhole")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 1 | `properties/trench.inc` |
| `/(define "foxhole")/volume/able/blast` | 1 | 미확인 | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break | 1 | `properties/trench.inc` |
| `/(define "foxhole")/volume/able/obstacle` | 1 | 미확인 |  | 1 | `properties/trench.inc` |
| `/(define "foxhole")/volume/tags` | "body" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/trench.inc` |
| `/(define "foxhole")/volume/thickness` | 60 | 미확인 |  | 1 | `properties/trench.inc` |
| `/(define "frag_grenade_protection")/("blast_protection")` | dmg ( 6 ) | 미확인 |  | 1 | `properties/construction_generic.inc` |
| `/(define "items_engineer")/item` | "shovel_ger" 1 ¦ "shovel_rus" 1 ¦ "shovel_fin" 1 ¦ "shovel_usa" 1 ¦ "shovel_eng" 1 ¦ "pickaxe" 1 ¦ "tent_kit" 1 ¦ "sandbag_vehicle" 1 ¦ "mine_detector" 1 ¦ "repair_kit" 1 ¦ "flame_thrower" "ammo" 10 | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_engineer")/item/value` | 5 ~ 20 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_explosives")/item` | 예: "dynamite_ger_x7" 1 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_explosives")/item/value` | 5 ~ 15 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_eng")/item` | 예: "bulleteng_15_auto" "ap" "ammo" 25 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_eng")/item/value` | 5 ~ 100 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_fin")/item` | 예: "mortarfin_170" "he" "ammo" 1 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_fin")/item/value` | 5 ~ 100 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_fra")/item` | "bulletfra_25" "ap" "ammo" 2 | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_fra")/item/value` | 5 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_generic")/item` | "sandbag_vehicle" 1 | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_generic")/item/value` | 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_ger")/item` | 예: "bullet_20_auto" "aphe" "ammo" 20 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_ger")/item/value` | 5 ~ 350 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_rus")/item` | 예: "bullet_20_auto" "aphe" "ammo" 20 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_rus")/item/value` | 5 ~ 175 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_usa")/item` | 예: "bulletusa_37" "ap" "ammo" 2 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_heavy_usa")/item/value` | 5 ~ 100 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_light_eng")/item` | 예: "7_65mm_pistol" "ammo" 8 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_light_eng")/item/value` | 1 ~ 15 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_light_fin")/item` | 예: "m1907_mag" "ammo" 14 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_light_fin")/item/value` | 1 ~ 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_light_fra")/item` | "rifle_fra" "clip" "ammo" 10 ¦ "fm_24_29_mag" "ammo" 25 ¦ "grenade" "mk2" 1 | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_light_fra")/item/value` | 1 ~ 5 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_light_generic")/item` | "flame_thrower" "ammo" 50 ¦ "pistol" "ammo" 20 ¦ "bandage_ger" 1 ¦ "bandage_rus" 1 ¦ "bandage_fin" 1 ¦ "bandage_usa" 1 ¦ "bandage_eng" 1 ¦ "knife_ger" 1 ¦ "knife_rus" 1 ¦ "knife_fin" 1 ¦ "knife_usa" 1 ¦ "knife_eng" 1 | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_light_generic")/item/value` | 1 ~ 100 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_light_ger")/item` | 예: "mp40" "ammo" 32 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_light_ger")/item/value` | 1 ~ 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_light_rus")/item` | 예: "tt33" "ammo" 16 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_light_rus")/item/value` | 1 ~ 15 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_light_usa")/item` | 예: "m1911_mag" "ammo" 14 (13종 이상) | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_light_usa")/item/value` | 1 ~ 15 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "items_medic")/item` | "bandage_ger" 1 ¦ "bandage_rus" 1 ¦ "bandage_fin" 1 ¦ "bandage_usa" 1 ¦ "bandage_eng" 1 | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "items_medic")/item/value` | 1 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "look_chain")/state` | "look_%0_in" ¦ "look_%0" ¦ "look_%0_throw_begin" ¦ "look_%0_throw_end" ¦ "look_%0_fire" ¦ "look_%0_recoil" ¦ "look_%0_out" | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 1 | `properties/human_fsm/look.inc` |
| `/(define "look_chain")/state/condition` | "look-%0-in" | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "look_chain")/state/from` | "stand" | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "look_chain")/state/tag` | 예: "up" (13종 이상) | 미확인 | hybrid AT grenade / mines ¦ thrower hand weapon | 1 | `properties/human_fsm/look.inc` |
| `/(define "look_chain")/state/to` | "look_%0" ¦ "look_%0_fire" ¦ "look_%0_throw_begin" ¦ "look_%0_out" ¦ "look_%0_recoil" ¦ "stand" | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "look_chain")/state/transit` | "look_%0_throw_end" ¦ "look_%0" | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "mobility_btr")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/braketime` | ( + 1 ( * %weight ( * %speed 0.004 ) ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/fallslope` | 75 | 미확인 | Maximum slope before physics kick in | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/fast` | ( * %speed 0.9 ) | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/gears` | 0.15 1 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/maxslope` | 75 | 미확인 | Maximum slope to drive up hill | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/maxspeedatmaxturn` | %speedatmaxturn | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/normal` | 25 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/reargears` | ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/slow` | 10 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/splineradius` | 0.75 | 미확인 | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/starttime` | ( * ( / %speed ( / %power %weight ) ) 4 ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/steeroff` | 25 | 미확인 | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/steeron` | 10 | 미확인 | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/stoptime` | ( + 2 ( * %weight ( * %speed 0.004 ) ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_btr")/locomotion/turnradius` | 5 | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/braketime` | ( + 0.25 ( / %weight 10000 ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/fallslope` | 70 | 미확인 | Maximum slope before physics kick in | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/fast` | %speed | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/gears` | 0.35 0.5 0.7 1.0 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/maxslope` | 60 | 미확인 | Maximum slope to drive up hill | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/normal` | %speed | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/reargears` | 0.33 0.75 | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/slow` | ( * %speed 0.33 ) | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/starttime` | ( + 1.25 ( / %weight 4000 ) ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/stoptime` | ( + 0.35 ( / %weight 4000 ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/turnradius` | ( + 2 ( / %weight 5000 ) ) | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/turnstart` | ( + 0.65 ( / %weight 5500 ) ) | 미확인 | Direct Control, Time for vehicle to from 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/turnstop` | ( + 0.65 ( / %weight 5500 ) ) | 미확인 | Direct Control, Time for vehicle to from max rotation speed to 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_cannon")/locomotion/turntime` | ( - ( + ( / %weight 500 ) ( / 50 %speed ) ) 0 ) | 미확인 | Direct Control, Time it takes to do a full 360 degree at max turn speed, fetching custom define | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/braketime` | ( + 1 ( * %weight ( * %speed 0.004 ) ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/fallslope` | 75 | 미확인 | Maximum slope before physics kick in | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/fast` | ( * %speed 0.9 ) | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/gears` | 0.15 1 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/maxslope` | 75 | 미확인 | Maximum slope to drive up hill | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/maxspeedatmaxturn` | %speedatmaxturn | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/normal` | 25 | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/reargears` | ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/slow` | 10 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/splineradius` | 0.75 | 미확인 | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/starttime` | ( * ( / %speed ( / %power %weight ) ) 7 ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/steeroff` | 30 | 미확인 | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/steeron` | 20 | 미확인 | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/stoptime` | ( + 3 ( * %weight ( * %speed 0.020 ) ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_car")/locomotion/turnradius` | 5 | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/braketime` | ( + 0.30 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/fallslope` | 75 | 미확인 | Maximum slope before physics kick in | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/fast` | %speed | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/gears` | 0.5 0.65 0.8 0.9 1.0 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/maxslope` | 75 | 미확인 | Maximum slope to drive up hill | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/maxspeedatmaxturn` | ( * %speed ( * ( / %track 30 ) ( / %power ( * %weight 5 ) ) ) ) | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/normal` | %speed | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/reargears` | 0.05 ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/slow` | 4 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/starttime` | ( * 4 ( / %speed ( / %power %weight ) ) ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/stoptime` | ( + 0.60 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/turnradius` | 0.5 | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/turnstart` | ( + 0.35 ( * ( / %weight 6 ) 0.04 ) ) | 미확인 | Direct Control, Time for vehicle to from 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/turnstop` | ( + 0.3 ( * ( / %weight 6 ) 0.04 ) ) | 미확인 | Direct Control, Time for vehicle to from max rotation speed to 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_goliath")/locomotion/turntime` | ( * ( / 360 %traverse ) 1.1 ) | 미확인 | Direct Control, Time it takes to do a full 360 degree at max turn speed, fetching custom define | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/braketime` | ( + 1.5 ( * %weight ( * %speed 0.002 ) ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/fast` | ( * %speed 0.8 ) | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/gears` | 0.5 0.7 1.0 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/maxspeedatmaxturn` | %speedatmaxturn | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/normal` | ( * %speed 0.6 ) | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/reargears` | ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/slow` | ( * %speed 0.3 ) | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/splineradius` | 3 | 미확인 | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/starttime` | ( * 30 ( / %speed ( / %power %weight ) ) ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/steeroff` | 15 | 미확인 | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/steeron` | 7.5 | 미확인 | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/stoptime` | ( + 3.5 ( * %weight ( * %speed 0.004 ) ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_medium_boat")/locomotion/turnradius` | 5 | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/braketime` | ( + 1.5 ( * %weight ( * %speed 0.004 ) ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/fast` | %speed | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/gears` | 0.15 1 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/maxspeedatmaxturn` | %speedatmaxturn | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/normal` | %speed | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/reargears` | ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/slow` | ( * %speed 0.3 ) | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/splineradius` | 0.75 | 미확인 | limit turning radius on the splines (path planner parameter), meters ¦ limit turning radius on the splines, meters | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/starttime` | ( * ( / %speed ( / %power %weight ) ) 7 ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/steeroff` | 40 | 미확인 | Steering wheel's relaxed (no control applied) neutral position return speed, degrees per second. ¦ wheel stop turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/steeron` | 20 | 미확인 | Steering wheel's turn speed, degrees per second. ¦ wheel turn speed in degrees per second | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/stoptime` | ( + 3.5 ( * %weight ( * %speed 0.020 ) ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_small_boat")/locomotion/turnradius` | 5 | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/braketime` | ( + 0.80 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/fallslope` | 75 | 미확인 | Maximum slope before physics kick in | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/fast` | ( * %speed 0.9 ) | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/gears` | 0.5 0.65 0.8 0.9 1.0 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/maxslope` | 75 | 미확인 | Maximum slope to drive up hill | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/maxspeedatmaxturn` | ( * %speed ( * ( / %track 30 ) ( / %power ( * %weight 5 ) ) ) ) | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/normal` | ( * %speed 0.6 ) | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/reargears` | 0.05 ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/slow` | 4 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/starttime` | ( * 5 ( / %speed ( / %power %weight ) ) ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/stoptime` | ( + 1.50 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/turnradius` | ( + 1 ( + ( / %speed 12 ) ( / %weight 60 ) ) ) | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/turnstart` | ( + 0.6 ( * ( / %weight 6 ) 0.05 ) ) | 미확인 | Direct Control, Time for vehicle to from 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/turnstop` | ( + 0.6 ( * ( / %weight 6 ) 0.05 ) ) | 미확인 | Direct Control, Time for vehicle to from max rotation speed to 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_slow")/locomotion/turntime` | ( / 360 %traverse ) | 미확인 | Direct Control, Time it takes to do a full 360 degree at max turn speed, fetching custom define | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/braketime` | ( + 0.80 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/fallslope` | 75 | 미확인 | Maximum slope before physics kick in | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/fast` | %fast | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/gears` | 0.5 0.65 0.8 0.9 1.0 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/maxslope` | 75 | 미확인 | Maximum slope to drive up hill | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/maxspeedatmaxturn` | ( * %speed ( * ( / %track 30 ) ( / %power ( * %weight 5 ) ) ) ) | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/normal` | %normal | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/reargears` | 0.05 ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/slow` | %slow | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/starttime` | ( * 5 ( / %speed ( / %power %weight ) ) ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/stoptime` | ( + 1.50 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/turnradius` | ( + 1 ( + ( / %speed 12 ) ( / %weight 60 ) ) ) | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/turnstart` | ( + 0.6 ( * ( / %weight 6 ) 0.05 ) ) | 미확인 | Direct Control, Time for vehicle to from 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/turnstop` | ( + 0.6 ( * ( / %weight 6 ) 0.05 ) ) | 미확인 | Direct Control, Time for vehicle to from max rotation speed to 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank_special")/locomotion/turntime` | ( / 360 %traverse ) | 미확인 | Direct Control, Time it takes to do a full 360 degree at max turn speed, fetching custom define | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/braketime` | ( + 0.80 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/fallslope` | 75 | 미확인 | Maximum slope before physics kick in | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/fast` | ( * %speed 0.9 ) | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/gears` | 0.5 0.65 0.8 0.9 1.0 | 미확인 | Gearing affects acceleration curve. ¦ Forward gear ratios | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/maxslope` | 75 | 미확인 | Maximum slope to drive up hill | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/maxspeedatmaxturn` | ( * %speed ( * ( / %track 30 ) ( / %power ( * %weight 5 ) ) ) ) | 미확인 | Max speed the vehicle is limited to when executing a tight turn. ¦ Direct Control, Max speed the vehicle is limited to when executing a tight turn. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/normal` | ( * %speed 0.6 ) | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/reargears` | 0.05 ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/slow` | 10 | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/starttime` | ( * 5 ( / %speed ( / %power %weight ) ) ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/stoptime` | ( + 1.50 ( * %weight 0.04 ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/turnradius` | ( + 1 ( + ( / %speed 12 ) ( / %weight 60 ) ) ) | 미확인 | AI turning radius, lower = able to turn more without stopping. ¦ New turning radius to help reduce "stop and neutral" turns for tanks | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/turnstart` | ( + 0.6 ( * ( / %weight 6 ) 0.05 ) ) | 미확인 | Direct Control, Time for vehicle to from 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/turnstop` | ( + 0.6 ( * ( / %weight 6 ) 0.05 ) ) | 미확인 | Direct Control, Time for vehicle to from max rotation speed to 0Â°/s rotation speed to max rotation speed. ¦ / | 1 | `properties/mobility.inc` |
| `/(define "mobility_tank")/locomotion/turntime` | ( / 360 %traverse ) | 미확인 | Direct Control, Time it takes to do a full 360 degree at max turn speed, fetching custom define | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/fuelbag/fuel` | "%type" | 미확인 | Type of fuel used. "fuel" = petrol/gasoline. "diesel" = diesel. ¦ Fuel type. | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/fuelbag/rate` | ( * 10 ( / %fuel %range ) ) | 미확인 | Fuel consumption rate. Based on off-road range of vehicle. ¦ liters per kilometer | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/fuelbag/remain` | %fuel | 미확인 | Amount of fuel in vehicle when spawned. ¦ how much fuel is in fuel tank at spawn | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/fuelbag/volume` | %fuel | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/braketime` | ( + 2 ( * %weight ( * %speed 0.01 ) ) ) | 미확인 | Time to come to a complete stop from max speed with brakes applied. ¦ time to decelerate from max speed to 0 kph in seconds when using brakes (hard stop) | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/fast` | %speed | 미확인 | Sprint speed. Double click movement order ¦ speed for fast movement state (double mouse click move order) | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/gearswitchdelay` | 0.5 | 미확인 | Time to switch between forward and reverse. | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/maxspeed` | %speed | 미확인 | Max speed. ¦ max speed while in flight mode in kph | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/normal` | ( * %speed 0.66 ) | 미확인 | Standard speed. Single click movement order ¦ single flick, running mode | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/reargears` | ( / %reverse %speed ) | 미확인 | Reverse max speed. | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/slow` | ( * %speed 0.33 ) | 미확인 | Slow speed. Mainly used in mission scripting ¦ km/h | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/starttime` | ( * ( / %speed ( / %power %weight ) ) 9 ) | 미확인 | Time for vehicle to accelerate from 0kph to max speed. ¦ time to accelerate to max speed from 0 kph in seconds | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotion/stoptime` | ( + 3 ( * %weight ( * %speed 0.025 ) ) ) | 미확인 | Time to come to a complete stop from max speed with out applying brakes. "Coasting". ¦ time to decelerate from max speed to 0 kph in seconds when coasting to a stop (not applying forward motion) | 1 | `properties/mobility.inc` |
| `/(define "mobility_train")/locomotive` | 1 | 미확인 |  | 1 | `properties/mobility.inc` |
| `/(define "peep_chain")/state` | "peep_%0_in" ¦ "peep_%0" ¦ "peep_%0_out" | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 1 | `properties/human_fsm/look.inc` |
| `/(define "peep_chain")/state/condition` | "peep-%0-in" | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "peep_chain")/state/from` | "stand" | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "peep_chain")/state/tag` | "up" ¦ "looking" ¦ "peep-%0-in" | 미확인 | hybrid AT grenade / mines ¦ thrower hand weapon | 1 | `properties/human_fsm/look.inc` |
| `/(define "peep_chain")/state/to` | "peep_%0" ¦ "peep_%0_out" ¦ "stand" | 미확인 |  | 1 | `properties/human_fsm/look.inc` |
| `/(define "place")/place` | "%0" | 미확인 |  | 1 | `properties/hedgerow.inc` |
| `/(define "place")/place/entity` | "%1" | 미확인 | parts ¦ airborne parts | 1 | `properties/hedgerow.inc` |
| `/(define "place")/place/handlebone` | "basis" | 미확인 |  | 1 | `properties/hedgerow.inc` |
| `/(define "place")/place/linkbone` | "%0" | 미확인 |  | 1 | `properties/hedgerow.inc` |
| `/(define "roof_vol")/volume` | "%0" | 미확인 | Main Body ¦ Truck Bed | 1 | `properties/construction_generic.inc` |
| `/(define "roof_vol")/volume/able/visible` | %1 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/construction_generic.inc` |
| `/(define "roof_vol")/volume/tags` | "roof" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 1 | `properties/construction_generic.inc` |
| `/(define "supply_ammo_fixed")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/extender[supply_zone]/limit` | %supply | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/extender[supply_zone]/radius` | 6 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/extender[supply_zone]/regenerationperiod` | 0 ~ 10 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo_fixed")/extender[supply_zone]/resupplyperiod` | 2 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/extender[supply_zone]/limit` | %supply | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/extender[supply_zone]/radius` | 10 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/extender[supply_zone]/regenerationperiod` | 0 ~ 10 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "supply_ammo")/extender[supply_zone]/resupplyperiod` | 2 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/extender[supply_zone]/limit` | 200 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/extender[supply_zone]/mechanicresources/amount` | 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/extender[supply_zone]/mechanicresources/value` | 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/extender[supply_zone]/radius` | 12 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/extender[supply_zone]/regenerationperiod` | 0 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer_light")/extender[supply_zone]/resupplyperiod` | 2 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/extender[supply_zone]/limit` | 500 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/extender[supply_zone]/mechanicresources/amount` | 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/extender[supply_zone]/mechanicresources/value` | 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/extender[supply_zone]/radius` | 12 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/extender[supply_zone]/regenerationperiod` | 0 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "supply_engineer")/extender[supply_zone]/resupplyperiod` | 2 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/fuel/amount` | 1 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/fuel/value` | 1 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/limit` | %liters | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/radius` | 12 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/regenerationperiod` | 0 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/resupply/item` | "flame_thrower" "ammo" 50 | 미확인 | Finnish shell just has to have rus entity name for gun compatibility ¦ 6 rockets costs 360 per salvo | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/resupply/item/value` | 1 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_fuel")/extender[supply_zone]/resupplyperiod` | 1 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/extender[supply_zone]/limit` | 50 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/extender[supply_zone]/radius` | 5 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/extender[supply_zone]/regenerationperiod` | 0 ~ 2 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "supply_medic")/extender[supply_zone]/resupplyperiod` | 1 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/extender[supply_zone]/limit` | 2000 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/extender[supply_zone]/radius` | 12 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/extender[supply_zone]/regenerationperiod` | 0 ~ 5 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy_2x")/extender[supply_zone]/resupplyperiod` | 2 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/extender[supply_zone]/limit` | 1000 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/extender[supply_zone]/radius` | 12 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/extender[supply_zone]/regenerationperiod` | 0 ~ 5 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo_heavy")/extender[supply_zone]/resupplyperiod` | 2 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/decalview` | supply_zone | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/decalview/layer` | grounddynamic | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/decalview/sprite` | "supply_zone.ebm" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/decalview/sprite/visibility` | 0.25 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/extender[supply_zone]` | "supply_zone" | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/extender[supply_zone]/limit` | 500 | 미확인 | max number of resupplies (0 = unlimited) ¦ Max CP | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/extender[supply_zone]/mechanicresources/amount` | 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/extender[supply_zone]/mechanicresources/value` | 10 | 미확인 |  | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/extender[supply_zone]/radius` | 12 | 미확인 | resupply radius in meter | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/extender[supply_zone]/regenerationperiod` | 0 ~ 10 | 미확인 | regeneration period in seconds, +1 supply every x seconds (0 = no regeneration) | 1 | `properties/resupply.inc` |
| `/(define "truck_ammo")/extender[supply_zone]/resupplyperiod` | 2 | 미확인 | resupply period in seconds | 1 | `properties/resupply.inc` |
| `/(define "turret_heavy")/speed2` | ( + 4 ( / %power_traverse 3 ) ) | 미확인 |  | 1 | `properties/mobility.inc` |
| `/(define "turret_light")/speed2` | ( + 10 ( / %power_traverse 2.5 ) ) | 미확인 |  | 1 | `properties/mobility.inc` |
| `/(define "turret_medium")/speed2` | ( + 6 ( / %power_traverse 2.5 ) ) | 미확인 |  | 1 | `properties/mobility.inc` |
| `/(define "volume")/volume` | %0 | 미확인 | Main Body ¦ Truck Bed | 2 | `properties/lump.inc` |
| `/(define "volume")/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 2 | `properties/lump.inc` |
| `/(define "volume")/volume/able/contact` | 0 | 미확인 |  | 1 | `properties/trench.inc` |
| `/(define "volume")/volume/able/obstacle` | 1 | 미확인 |  | 2 | `properties/lump.inc` |
| `/(define "volume")/volume/tags` | "body" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 2 | `properties/lump.inc` |
| `/(define "volume")/volume/thickness` | 1 ~ 60 | 미확인 |  | 2 | `properties/lump.inc` |
| `/armor/setting` | "body" | 미확인 |  | 5 | `properties/big_stone.inc` |
| `/armor/setting/threshold/blast/koef` | 1 | 미확인 |  | 3 | `properties/big_stone.inc` |
| `/armor/setting/threshold/blast/value` | 15 ~ 64 | 미확인 |  | 2 | `properties/hedgerow.inc` |
| `/armor/setting/threshold/pierce/calibre` | 12 | 미확인 |  | 3 | `properties/big_stone.inc` |
| `/armor/setting/volumes` | "body*" | 미확인 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading | 5 | `properties/big_stone.inc` |
| `/blend` | 예: "pose_lie_empty" (13종 이상) | 미확인 |  | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/blend/pose` | 예: "weapon\anim_item_lie_take_end_01" end (13종 이상) | 미확인 |  | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/bone` | "basis" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/selection/human.inc` |
| `/collider` | "particle.small" ¦ "construction" ¦ "bush" ¦ "tree" | 미확인 |  | 17 | `properties/big_part.inc` |
| `/constrain` | none ¦ zero | 미확인 |  | 4 | `properties/big_part.inc` |
| `/decalview` | %name ¦ "selection_active" ¦ "mineDetectionDecal" ¦ "selection_selectable" ¦ "selection_selected" | 미확인 |  | 27 | `properties/selection/cannon_active.inc` |
| `/decalview/layer` | alpha ¦ grounddynamic | 미확인 |  | 27 | `properties/selection/cannon_active.inc` |
| `/decalview/offset` | 0 0 10 ¦ 0 0 1 | 미확인 |  | 26 | `properties/selection/cannon_active.inc` |
| `/decalview/sprite` | 예: "cannon_selected.ebm" (13종 이상) | 미확인 |  | 27 | `properties/selection/cannon_active.inc` |
| `/decalview/sprite/light` | 0.2 ~ 1 | 미확인 |  | 27 | `properties/selection/cannon_active.inc` |
| `/decalview/sprite/prescalex` | ( * 0.30 %scale ) ¦ 0.12 ¦ 0.07 ¦ 0.4 ¦ 0.093 ¦ 0.085 ¦ ( * 0.50 %scale ) | 미확인 |  | 27 | `properties/selection/cannon_active.inc` |
| `/decalview/sprite/prescaley` | ( * 0.30 %scale ) ¦ 0.12 ¦ 0.07 ¦ 0.4 ¦ 0.093 ¦ 0.085 ¦ ( * 0.50 %scale ) ¦ ( * 0.25 %scale ) ¦ ( * 0.35 %scale ) | 미확인 |  | 27 | `properties/selection/cannon_active.inc` |
| `/decalview/sprite/preturn` | 90 | 미확인 |  | 4 | `properties/selection/human_active.inc` |
| `/decalview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 27 | `properties/selection/cannon_active.inc` |
| `/extender[edifice]` | "edifice" | 미확인 |  | 1 | `properties/building_roof.inc` |
| `/extender[land_patch]` | "land_patch" | 미확인 |  | 6 | `properties/cliff.inc` |
| `/extender[land_patch]/alignenabled` | 1 | 미확인 |  | 5 | `properties/crater.inc` |
| `/extender[land_patch]/alignstacked` | 0 ~ 1 | 미확인 |  | 5 | `properties/crater.inc` |
| `/extender[land_patch]/alignvolumes` | 0 ~ 1 | 미확인 |  | 2 | `properties/lump.inc` |
| `/extender[land_patch]/randomrotate` | 1 | 미확인 |  | 2 | `properties/crater.inc` |
| `/extender[land_patch]/removeentities` | 1 | 미확인 |  | 1 | `properties/trench.inc` |
| `/extender[land_patch]/terraintype` | "ground" "rock" ¦ "ground" "ground" ¦ "ground" "snow" | 미확인 |  | 4 | `properties/cliff.inc` |
| `/extender[land_patch]/updateentities` | 0 | 미확인 |  | 1 | `properties/hedgerow.inc` |
| `/extender[light]` | "light" | 미확인 |  | 1 | `properties/light.inc` |
| `/extender[light]/preset` | "default" ¦ "lamp_spot" ¦ "lamp_point" ¦ "lamp_search" | 미확인 |  | 1 | `properties/light.inc` |
| `/extender[light]/preset/break_bone` | "lamp_glass1" | 미확인 |  | 1 | `properties/light.inc` |
| `/extender[light]/preset/break_fx` | "lamp_hit" | 미확인 |  | 1 | `properties/light.inc` |
| `/extender[light]/preset/light_bone` | "lamp_light_point1" ¦ "lamp_light_spot1" | 미확인 |  | 1 | `properties/light.inc` |
| `/extender[light]/preset/light_fx` | "light_post" ¦ "light_post_dir" ¦ "light_post_plafond" ¦ "light_searchlight" | 미확인 |  | 1 | `properties/light.inc` |
| `/lodclass` | "part_big" ¦ "huge" ¦ "decor_landmark" ¦ "decor_building" ¦ "decor_bush" ¦ "decor_big" ¦ "decor_shellhole" ¦ "decor_medium" ¦ "part_small" ¦ "decor_tree" ¦ "decor_trench" | 미확인 |  | 18 | `properties/big_part.inc` |
| `/mass` | 1 ~ 50000 | 미확인 | 65kg human + 5kg bike | 10 | `properties/big_part.inc` |
| `/nogroupmesh` | 1 | 미확인 |  | 2 | `properties/bush.inc` |
| `/obstacleid` | "construction" ¦ "unbreakable" ¦ "building" ¦ "nothing" ¦ "construction_vaultable" ¦ "trench_wall" ¦ "post" ¦ "tree" | 미확인 |  | 21 | `properties/big_part.inc` |
| `/props` | 예: "big" "part" (13종 이상) | 미확인 |  | 19 | `properties/big_part.inc` |
| `/request` | 예: "dead" (13종 이상) | 미확인 |  | 1 | `properties/human_fsm/request.inc` |
| `/request/duration` | 1 ~ 2 | 미확인 |  | 1 | `properties/human_fsm/request.inc` |
| `/request/exit` | "stop-move" ¦ "stop-rotate" ¦ "stop-repair" ¦ "stop-hold-back" ¦ "stop-animation" | 미확인 |  | 1 | `properties/human_fsm/request.inc` |
| `/request/priority` | 0 ~ 50 | 미확인 |  | 1 | `properties/human_fsm/request.inc` |
| `/request/switch` | "walk" 0 ¦ "walk" 1 | 미확인 |  | 1 | `properties/human_fsm/request.inc` |
| `/sequence` | 예: "cinematics\lst_attn_01" (13종 이상) | 미확인 | if too slow, semi autos won't play animation everytime ¦ {file "weapon\anim_mg_stand_reload_up.anm"} @note: commented out because of diff wep speeds. | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/sequence/dynamicspeed/#row` | 예: 102 103 1000 (13종 이상) | 미확인 |  | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/sequence/events/#row` | 예: 11 "leg_right" (13종 이상) | 미확인 |  | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/sequence/file` | 예: "flinch/flinch_head_01.anm" (13종 이상) | 미확인 |  | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/sequence/pinback` | 2 | 미확인 |  | 1 | `properties/animation/human/_reg_human_movement.inc` |
| `/sequence/rotate` | "body" 0 0 1 -90 | 미확인 |  | 1 | `properties/animation/human/_reg_human_movement.inc` |
| `/sequence/smooth` | 0 ~ 0.7 | 미확인 |  | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/sequence/speed` | 0.15 ~ 4 | 미확인 | sway speed coef ¦ kmph | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/simulation/able` | 1 | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 3 | `properties/big_part.inc` |
| `/simulation/state` | 1 | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 3 | `properties/big_part.inc` |
| `/simulatorid` | "pillar" | 미확인 |  | 3 | `properties/post.inc` |
| `/skeleton/bone` | "basis" | 미확인 | Land;"}} ¦ "}} | 1 | `properties/small_rock.inc` |
| `/skeleton/bone/matrix34` | 0 0 1 0 -1 0 1 0 0 0 0 -0.5 | 미확인 |  | 1 | `properties/small_rock.inc` |
| `/skeleton/texmods/"*"` | 예: "Autumn" "aut" (13종 이상) | 미확인 |  | 2 | `properties/seasons.inc` |
| `/smooth` | 0 ~ 0.5 | 미확인 |  | 2 | `properties/animation/human/_reg_human_movement.inc` |
| `/split` | 2 | 미확인 | { split <index> <from> <to> } | 1 | `properties/human_fsm/request.inc` |
| `/spriteview` | "selection_linked" | 미확인 |  | 1 | `properties/selection/linked.inc` |
| `/spriteview/layer` | debug | 미확인 |  | 1 | `properties/selection/linked.inc` |
| `/spriteview/offset` | 0 0 %off | 미확인 |  | 1 | `properties/selection/linked.inc` |
| `/spriteview/orientation` | billboard | 미확인 |  | 1 | `properties/selection/linked.inc` |
| `/spriteview/sprite` | "arrow.ebm" | 미확인 |  | 1 | `properties/selection/linked.inc` |
| `/spriteview/sprite/prescalex` | ( * 0.4 %scale ) | 미확인 |  | 1 | `properties/selection/linked.inc` |
| `/spriteview/sprite/prescaley` | ( * 0.4 %scale ) | 미확인 |  | 1 | `properties/selection/linked.inc` |
| `/spriteview/visible` | 0 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 1 | `properties/selection/linked.inc` |
| `/start` | "stand" | 미확인 | games played : starting CP of each match | 1 | `properties/human_fsm/request.inc` |
| `/state` | 예: "stand_bazooka_pick" (13종 이상) | 미확인 | 2hand & 1hand items ¦ to enable item ops in snipe cover | 11 | `properties/human_fsm/bazooka_stand.inc` |
| `/state/condition` | 예: "board" (13종 이상) | 미확인 |  | 6 | `properties/human_fsm/board.inc` |
| `/state/from` | "stand" ¦ "board" ¦ "lie" ¦ "lie_aim" ¦ "snipe" ¦ "squat" ¦ "walk" ¦ "squat_aim" ¦ "stand_noaim" ¦ "throwoff_end" | 미확인 |  | 10 | `properties/human_fsm/bazooka_stand.inc` |
| `/state/fromtagged` | "up" ¦ "snipe" ¦ "squat" ¦ "onboard" ¦ "down" ¦ "swim" ¦ "cover" ¦ "healing" | 미확인 |  | 2 | `properties/human_fsm/die.inc` |
| `/state/split` | 예: "snipe_aim" "recoil" (13종 이상) | 미확인 | { split <index> <from> <to> } | 3 | `properties/human_fsm/snipe.inc` |
| `/state/tag` | 예: "up" (13종 이상) | 미확인 | hybrid AT grenade / mines ¦ thrower hand weapon | 11 | `properties/human_fsm/bazooka_stand.inc` |
| `/state/to` | 예: "stand" (13종 이상) | 미확인 |  | 11 | `properties/human_fsm/bazooka_stand.inc` |
| `/state/transit` | 예: "stand" (13종 이상) | 미확인 |  | 8 | `properties/human_fsm/bazooka_stand.inc` |
| `/subset` | "default" ¦ "with_item" from "default" ¦ "with_weapon" from "default" ¦ "with_bazooka" from "with_weapon" ¦ "on_board" | 미확인 |  | 1 | `properties/human_fsm/subset.inc` |
| `/subset/disable` | "weapon" ¦ "open-with-item" ¦ "down" ¦ "squat" ¦ "looking" ¦ "repair" ¦ "revive" ¦ "step-aside" ¦ "up" | 미확인 |  | 1 | `properties/human_fsm/subset.inc` |
| `/subset/enable` | "open-with-item" ¦ "weapon" ¦ "board" | 미확인 | to enable item ops in snipe cover | 1 | `properties/human_fsm/subset.inc` |
| `/switch` | "walk" | 미확인 |  | 1 | `properties/human_fsm/switch.inc` |
| `/switch/link` | "stand" "walk" ¦ "stand" "walk_switch_move" ¦ "stand_recoil" "walk_recoil" ¦ "stand_bolt" "walk_bolt" | 미확인 | { link <from> <to> } | 1 | `properties/human_fsm/switch.inc` |
| `/switch/split` | 0 "stand" "walk" ¦ 0 "stand_aim" "walk" | 미확인 | { split <index> <from> <to> } | 1 | `properties/human_fsm/switch.inc` |
| `/tpc_zclip` | 1 | 미확인 |  | 2 | `properties/bush.inc` |
| `/volume` | 예: "rock" (13종 이상) | 미확인 | Main Body ¦ Truck Bed | 11 | `properties/big_stone.inc` |
| `/volume/able` | clear | 미확인 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) | 7 | `properties/bush.inc` |
| `/volume/able/blast` | 1 | 미확인 | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break | 3 | `properties/bush.inc` |
| `/volume/able/bullet` | 0 | 미확인 |  | 2 | `properties/hedgerow.inc` |
| `/volume/able/obstacle` | 0 | 미확인 |  | 5 | `properties/post.inc` |
| `/volume/able/overlap` | 1 | 미확인 |  | 2 | `properties/bush.inc` |
| `/volume/able/touch` | 0 | 미확인 |  | 1 | `properties/tree.inc` |
| `/volume/able/visible` | 0 ~ 1 | 미확인 | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. | 4 | `properties/bush.inc` |
| `/volume/density` | 0.00 ~ 0.007 | 미확인 |  | 2 | `properties/tree.inc` |
| `/volume/material` | "ground" | 미확인 |  | 1 | `properties/hedgerow.inc` |
| `/volume/tags` | "body" ¦ "lamp" ¦ "bole" ¦ "body" "fire" | 미확인 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set | 7 | `properties/big_stone.inc` |
| `/volume/thickness` | 0.2 ~ 1000 | 미확인 |  | 8 | `properties/big_stone.inc` |
| `/zclipvisibility` | 0 ~ 1 | 미확인 |  | 4 | `properties/bush.inc` |

