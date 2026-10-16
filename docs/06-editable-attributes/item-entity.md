# 수정 가능한 속성: 아이템 엔티티

이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다. 설명은 `data/annotations/common.psv`에서 고친다. 표 읽는 법은 [README.md](README.md)에 있다.

아이템이 땅에 놓이거나 손에 들렸을 때의 3D 엔티티다. `set/stuff`의 아이템이 `entity`로 이 파일을 가리킨다. 전투 수치는 여기가 아니라 [weapon-ammo.md](weapon-ammo.md)의 파일에 있다.

| 구분 | 파일 | 위치 | 속성 수 | 확인 | 문서 | 추정 | 정의 | 미확인 |
|---|---|---|---|---|---|---|---|---|
| 본체 | 아이템 엔티티 | `entity/inventory/**/*.def` | 78 | 2 | 4 | 46 | 0 | 26 |

## 본체: 아이템 엔티티

- 위치: `entity/inventory/**/*.def`
- 연결: `set/stuff` 아이템의 `entity`

### 뜻을 아는 속성 (52개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/{}/extension` | "barrel1.mdl" | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) |  | 1 | `entity/inventory/-stuff/barrel1/barrel1.def` |
| `/{}/props` | "collectable" | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 1 | `entity/inventory/-stuff/barrel1/barrel1.def` |
| `/actor/bone/limits` | -0 15 | **추정** 회전 한계 각도 두 개 (추정: 주석 "elevation range = 0 +70") |  | 1 | `entity/inventory/-weapon/-small_arms/lee_enfield_no1_mk3_grenade/2in_mortar_test.def` |
| `/actor/bone/speed2` | 10 | **문서** 뼈대의 최대 속도. 회전은 도/초, 직선 이동은 미터/초. speed의 새 단위판이다 |  | 1 | `entity/inventory/-weapon/-small_arms/lee_enfield_no1_mk3_grenade/2in_mortar_test.def` |
| `/actor/collider` | "mine.at" ¦ "mine.ap" ¦ "stuff" | **추정** 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") |  | 17 | `entity/inventory/-weapon/-mines/mine_ap_shrapnel_mk1/mine_ap_shrapnel_mk1.def` |
| `/actor/extender[mine]` | "mine" | **추정** 지뢰 기능 (추정: 이름) |  | 18 | `entity/inventory/-weapon/-grenades/anti_tank/m7/m7.def` |
| `/actor/extender[mine]/time` | 3 | **확인** 매설에 더해지는 시간. 실제 매설 시간은 보병의 매설 애니메이션 길이에 이 값을 더한 것 | actual mining time equals the lenght of time for the human animation + this value if value is less than animation lenght. Otherwise this value is an absolute and animation will loop. ¦ actual mining time equals the length of time for the human animation + this value if value is less than animation length. Otherwise this value is an absolute and animation will loop. | 18 | `entity/inventory/-weapon/-grenades/anti_tank/m7/m7.def` |
| `/actor/extender[stuff]` | "stuff" | **추정** 아이템 기능. 이 엔티티를 set/stuff의 아이템과 잇는다 (추정: 하위 item) |  | 1694 | `entity/inventory/-gear_wearable/_unused/sov_cap/sov_cap.def` |
| `/actor/extender[stuff]/item` | 예: "sov_cap" (13종 이상) | **추정** 연결된 아이템 이름. set/stuff의 파일명과 일치한다 (추정: 참조 색인) |  | 1694 | `entity/inventory/-gear_wearable/_unused/sov_cap/sov_cap.def` |
| `/actor/extension` | 예: "sov_cap.mdl" (13종 이상) | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) |  | 1694 | `entity/inventory/-gear_wearable/_unused/sov_cap/sov_cap.def` |
| `/actor/lodclass` | "stuff_weapon" | **추정** 상세도(LOD) 전환 거리 등급의 이름 (추정: 값 "vehicle_plane", "human") |  | 1 | `entity/inventory/-weapon/-small_arms/k98/k98_entourage.def` |
| `/actor/obstacleid` | "nothing" | **추정** 장애물 분류 이름 (추정: 값 "tank", "stuff") |  | 5 | `entity/inventory/-weapon/-grenades/smoke/m24_smoke/m24_smoke.def` |
| `/actor/props` | 예: "headwear_rnd" (13종 이상) | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 176 | `entity/inventory/-gear_wearable/head/[germany]/ger_waff_fieldcap_camo/ger_wf_fieldcap_camo.def` |
| `/actor/texmod` | "black" ¦ "green" ¦ "camo" | **추정** 기본으로 쓰는 텍스처 변형 이름 (추정: 값 "black", "camo") |  | 3 | `entity/inventory/-gear_wearable/head/civilian_fedora_black/civilian_fedora_black.def` |
| `/actor/volume` | "body" ¦ "body1" ¦ "select" ¦ "body2" ¦ "casing1" ¦ "casing2" ¦ "shield" ¦ "shell1" ¦ "shell2" | **추정** 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) |  | 63 | `entity/inventory/-gear_wearable/head/[soviet_union]/soviet_tankist_cap/soviet_tankist_cap.def` |
| `/actor/volume/able` | clear ¦ | **추정** 볼륨이 반응하는 상호작용 묶음 (추정: 하위 플래그) |  | 20 | `entity/inventory/-tools/mine_detector/mine_detector.def` |
| `/actor/volume/able/visible` | 0 | **추정** 0이면 이 볼륨이 시야를 가리지 않는다 (추정: /game_entity의 같은 자리와 같다. properties/vehicle.ext 502줄 유리, 516줄 램프 볼륨이 0) |  | 5 | `entity/inventory/-tools/pickaxe/pickaxe.def` |
| `/actor/volume/density` | 0 | **문서** 장갑의 밀도(품질) 계수. 기본 1 |  | 19 | `entity/inventory/-weapon/-grenades/anti_tank/m7/m7.def` |
| `/actor/volume/material` | "wood" ¦ "glass" | **추정** 볼륨의 재질 이름. set/material.set 참조로 보인다 (추정: 값 "wood") |  | 10 | `entity/inventory/-tools/pickaxe/pickaxe.def` |
| `/actor/volume/tags` | "body" | **문서** 볼륨에 붙이는 태그. "no_target"은 AI가 그 볼륨을 표적으로 삼지 않게 한다 |  | 16 | `entity/inventory/-weapon/-grenades/anti_personnel/m24/m24.def` |
| `/actor/volume/thickness` | 1 ~ 10 | **추정** 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) |  | 36 | `entity/inventory/-medical/medkit/medkit.def` |
| `/entity/collider` | "grenade" | **추정** 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") |  | 1 | `entity/inventory/-tools/wirecutters/wirecutters.def` |
| `/entity/extender[stuff]/item` | "bandage_eng" ¦ "bandage_fin" ¦ "bandage_ger" ¦ "bandage_rus" ¦ "bandage_usa" ¦ "canned_meat" ¦ "human" ¦ "wirecutters" ¦ "grenade no73" ¦ "boys_mk1" | **추정** 연결된 아이템 이름. set/stuff의 파일명과 일치한다 (같은 자리의 본체 설명을 가져옴) |  | 10 | `entity/inventory/-medical/bandage_eng/bandage_eng.def` |
| `/entity/extension` | "bandage_ger.mdl" ¦ "bandage_eng.mdl" ¦ "bandage_rus.mdl" ¦ "bandage_usa.mdl" ¦ "canned_meat.mdl" ¦ "corpse.mdl" ¦ "wirecutters.mdl" ¦ "no73_at_grenade.mdl" ¦ "boys_mk1.mdl" | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) |  | 10 | `entity/inventory/-medical/bandage_eng/bandage_eng.def` |
| `/entity/props` | "physics_damping" ¦ "contact" ¦ "bipod" | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 3 | `entity/inventory/-tools/wirecutters/wirecutters.def` |
| `/entity/spriteview/sprite` | "flag1.ebm" ¦ "flag2.ebm" | **추정** 스프라이트로 쓰는 그림 파일(.ebm) (추정: 값 "arrow.ebm") |  | 3 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/spriteview/sprite/preturn` | 90 | **추정** 미리 돌려 놓는 각도, 도 (추정: 값 90) |  | 3 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/spriteview/sprite/scale` | 0.3 | **추정** 스프라이트 배율 (추정: 이름) |  | 3 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/volume` | "body" | **추정** 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) |  | 2 | `entity/inventory/-medical/canned_meat/canned_meat.def` |
| `/game_entity/collider` | "ammo" ¦ "stuff.construction" ¦ "construction" | **추정** 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") |  | 393 | `entity/inventory/-ammo/-mortar/170mm_fin_mortar_he/170mm_fin_mortar_he.def` |
| `/game_entity/extender[inventory]` | "inventory" | **추정** 차량 화물칸 기능 (추정: 하위 box) |  | 12 | `entity/inventory/-gear_wearable/body/plash_palatka_a/plash_palatka_a.def` |
| `/game_entity/extender[inventory]/box/item` | 예: "bandage_rus" 2 (13종 이상) | **추정** 실려 있는 아이템과 수량 (추정: 값 "ammo mgun_usa belt" 1000) |  | 6 | `entity/inventory/-gear_wearable/body/plash_palatka_a/plash_palatka_a.def` |
| `/game_entity/extender[inventory]/box/size` | 8 6 ¦ 4 4 ¦ 4 6 ¦ 6 6 | **확인** 인벤토리 창의 크기. 가로 세로 칸 수 |  | 12 | `entity/inventory/-gear_wearable/body/plash_palatka_a/plash_palatka_a.def` |
| `/game_entity/extender[inventory]/box/weight` | 600 ~ 5000 | **추정** 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인). 게임 안 확인 2026-10-16: 탄약 파일 mp40.ammo의 inventory weight를 3.1→10으로 바꾸자 병사 인벤토리의 MP40 탄이 한 칸에 32발에서 10발이 됐다(100/weight). 차량·거치 무기 인벤토리에서는 탄이 한 칸에 다 들어갔다(4500발, 1000발) |  | 6 | `entity/inventory/-stuff/ammo_box_inf/ammo_box_inf_ger.def` |
| `/game_entity/extender[stuff]` | "stuff" | **추정** 아이템 기능. 이 엔티티를 set/stuff의 아이템과 잇는다 (추정: 하위 item) |  | 504 | `entity/inventory/-ammo/-bomb/250lb_gp_mk4/250lb_gp_mk4.def` |
| `/game_entity/extender[stuff]/item` | 예: "air_bomb_eng_250lb" (13종 이상) | **추정** 연결된 아이템 이름. set/stuff의 파일명과 일치한다 (추정: 참조 색인) |  | 504 | `entity/inventory/-ammo/-bomb/250lb_gp_mk4/250lb_gp_mk4.def` |
| `/game_entity/extension` | 예: "250lb_gp_mk4.mdl" (13종 이상) | **추정** 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) |  | 545 | `entity/inventory/-ammo/-bomb/250lb_gp_mk4/250lb_gp_mk4.def` |
| `/game_entity/lodclass` | "tracer" ¦ "casing" ¦ "decor_small" | **추정** 상세도(LOD) 전환 거리 등급의 이름 (추정: 값 "vehicle_plane", "human") |  | 20 | `entity/inventory/-ammo/-shells/generic/howitzer_shell_105-150/howitzer_shell_105-150.def` |
| `/game_entity/mass` | 30 | **추정** 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명). 탄약 파일(bullet)에서는 한 발의 무게(kg)이고 인벤토리 툴팁의 무게 = 수량 × mass다 (게임 안 확인 2026-10-16: mgun_ger.belt.ammo를 0.011로 바꾸자 7.92mm 2850발이 31.4kg, 1000발이 11.0kg) |  | 3 | `entity/inventory/-stuff/box_metal/box_metal.def` |
| `/game_entity/obstacleid` | "stuff" | **추정** 장애물 분류 이름 (추정: 값 "tank", "stuff") |  | 2 | `entity/inventory/-stuff/box_metal/box_metal.def` |
| `/game_entity/props` | 예: "steel" "explosive_stuff" (13종 이상) | **추정** 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) |  | 332 | `entity/inventory/-ammo/-mortar/107mm_usa_mortar_he/107mm_usa_mortar_he.def` |
| `/game_entity/spriteview/layer` | grounddynamic | **추정** 스프라이트를 그리는 층 (추정: 값 grounddynamic) |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/offset` | 0 0 5 ¦ 0 0 25 ¦ 0 0 3 | **추정** 위치 오프셋 x y z (추정: 값) |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite` | "pointer_halo" ¦ "mine_f.ebm" ¦ "pointer_clr.ebm" | **추정** 스프라이트로 쓰는 그림 파일(.ebm) (추정: 값 "arrow.ebm") |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/scale` | 0.1 ~ 0.8 | **추정** 스프라이트 배율 (추정: 이름) |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/timer/mode` | play loop ¦ play | **추정** special 안에서는 특수 사용의 종류. aim = 저격 조준 사격, detonate = 기폭 (추정: sniper_rifle.pattern 5줄 {mode aim}, explosive.pattern 19줄 {mode detonate}). spriteview timer 안의 값 play, play loop는 재생 방식으로 보인다 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/texmod` | "ger" | **추정** 기본으로 쓰는 텍스처 변형 이름 (추정: 값 "ger") |  | 1 | `entity/inventory/-stuff/ammo_box_inf/ammo_box_inf_ger.def` |
| `/game_entity/volume` | "body" ¦ "fuel" ¦ "body1" ¦ "body2" ¦ "select" | **추정** 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) |  | 410 | `entity/inventory/-ammo/-mortar/170mm_fin_mortar_he/170mm_fin_mortar_he.def` |
| `/game_entity/volume/able` | ¦ clear | **추정** 볼륨이 반응하는 상호작용 묶음. clear는 상속된 설정을 지우는 것으로 보인다 (추정: 하위가 bullet, blast, touch 등 인자 없는 플래그) |  | 294 | `entity/inventory/-ammo/-mortar/170mm_fin_mortar_he/170mm_fin_mortar_he.def` |
| `/game_entity/volume/density` | 2 | **문서** 장갑의 밀도(품질) 계수. 기본 1 |  | 2 | `entity/inventory/-ammo/-shells/generic/shell_cannon/shell_cannon.def` |
| `/game_entity/volume/tags` | "body" ¦ "pour_over" | **추정** 볼륨 태그. 재질이나 부위를 나타낸다 (추정: 값 "wood", "wheel", "body") |  | 9 | `entity/inventory/-ammo/-shells/generic/shell_cannon/shell_cannon.def` |
| `/game_entity/volume/thickness` | 1 ~ 50 | **추정** 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) |  | 392 | `entity/inventory/-ammo/-mortar/170mm_fin_mortar_he/170mm_fin_mortar_he.def` |

### 뜻을 모르는 속성 (26개)

| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |
|---|---|---|---|---|---|
| `/actor/bone` | "foresight3" | 미확인 |  | 1 | `entity/inventory/-weapon/-small_arms/lee_enfield_no1_mk3_grenade/2in_mortar_test.def` |
| `/actor/nogroupmesh` | 1 | 미확인. 값은 1뿐. 용도 불명 |  | 188 | `entity/inventory/-tools/bagpipes/bagpipes.def` |
| `/actor/obstacle` | "body" | 미확인 |  | 18 | `entity/inventory/-weapon/-grenades/anti_tank/m7/m7.def` |
| `/actor/obstacle/circle2/radius` | 7.5 ~ 20 | 미확인 |  | 18 | `entity/inventory/-weapon/-grenades/anti_tank/m7/m7.def` |
| `/actor/volume/able/obstacle` | 0 | 미확인. able의 하위. 값 0 |  | 1 | `entity/inventory/-tools/pickaxe/pickaxe.def` |
| `/entity/extender[stuff]` | "stuff" | 미확인 |  | 10 | `entity/inventory/-medical/bandage_eng/bandage_eng.def` |
| `/entity/nogroupmesh` | 1 | 미확인. 값은 1뿐. 용도 불명 |  | 4 | `entity/inventory/-stuff/corpse/corpse.def` |
| `/entity/spriteview/orientation` | trace | 미확인 |  | 3 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/spriteview/sprite/light/linear/max` | 0.2 | 미확인 |  | 1 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/spriteview/sprite/light/linear/min` | 1 | 미확인 |  | 1 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/spriteview/sprite/scale/linear/max` | 0.35 | 미확인 |  | 1 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/spriteview/sprite/scale/linear/min` | 0.6 | 미확인 |  | 1 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/entity/spriteview/sprite/timer/duration` | 1.2 | 미확인 |  | 1 | `entity/inventory/-weapon/-mines/mined_view/mined_view_defused.def` |
| `/game_entity/body/proper` | body sensible | 미확인 |  | 5 | `entity/inventory/-weapon/-demolition/dynamite_ger_x3/dynamite_ger_x3.def` |
| `/game_entity/constrain` | none | 미확인. 값은 ground, none 등. 배치 제약으로 보이나 확인하지 못함 |  | 28 | `entity/inventory/-ammo/-mortar/200mm_leldgw_ammo/200mm_leldgw_ammo.def` |
| `/game_entity/fogable` | never ¦ always | 미확인 |  | 30 | `entity/inventory/-ammo/-bomb/250lb_gp_mk4/250lb_gp_mk4.def` |
| `/game_entity/nogroupmesh` | 1 | 미확인. 값은 1뿐. 용도 불명 |  | 79 | `entity/inventory/-ammo/-small_arms/-place_holder/eng_revolver_45_ammo/eng_revolver_38_ammo.def` |
| `/game_entity/scale` | 0.85 ~ 1.5 | 미확인 |  | 4 | `entity/inventory/-ammo/-small_arms/-casings/shell_762x25mm/shell_762x25mm.def` |
| `/game_entity/spriteview/sprite/highlight` | 0xFF5000 | 미확인 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/light/peak/max` | 0.4 ~ 0.5 | 미확인 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/light/peak/min` | 0 | 미확인 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/scale/linear/max` | 0.03 ~ 0.1 | 미확인 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/scale/linear/min` | 0.15 ~ 0.3 | 미확인 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/timer/duration` | 1 ~ 2 | 미확인 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/spriteview/sprite/visibility` | 0.3 ~ 0.5 | 미확인 |  | 20 | `entity/inventory/-weapon/-mines/mine_antipersonnel_vis/mine_antipersonnel_vis.def` |
| `/game_entity/volume/able/obstacle` | 0 | 미확인. able의 하위. 값 0 |  | 1 | `entity/inventory/-stuff/box_metal/box_metal.def` |

