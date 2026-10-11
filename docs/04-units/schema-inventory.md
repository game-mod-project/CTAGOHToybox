# 스키마: inventory

이 파일은 `tools/New-SchemaDoc.ps1`이 생성한다. 손으로 고치지 않는다. 의미와 등급은 `data/annotations/common.psv`에서 고친다. 마지막 열은 같은 이름의 키 옆에 달린 게임 파일의 주석을 이름만으로 모은 참고 자료이며, 이 경로의 뜻과 다를 수 있다.

이 그룹은 아이템의 3D 엔티티다.

| 위치 | 내용 | `.def` 수 |
|---|---|---|
| `entity/inventory/-gear_wearable` | 헬멧, 군장 | 1,475 |
| `entity/inventory/-ammo` | 탄약 | 452 |
| `entity/inventory/-weapon` | 무기 | 281 |
| `entity/inventory/-stuff`, `-tools`, `-medical` | 기타 | 45 |

`.def`는 `properties/stuff_*.ext`를 include하고, `{extender "stuff" {item "이름"}}`으로 `set/stuff`의 아이템과 이어진다. 수치는 거의 없고 충돌 볼륨과 모델 지정이 대부분이다.

## 원본 파일에 적힌 키

파일에 쓰인 그대로의 위치다. 고유 키 201개.

### 파일 종류: .def

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/{}` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/{}/(include)` | include | [str]x1 | 1 | 1 | "/properties/barrel.ext" | 미확인(자동) |  |  |  |
| `/{}/extension` | block | [str]x1 | 1 | 1 | "barrel1.mdl" | 추정 | 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | 파일 구조와 값에서 추론 |  |
| `/{}/props` | block | [str]x1 | 1 | 1 | "collectable" | 추정 | 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/actor` | block | []x1694 | 1694 | 1694 | 예: | 추정 | 엔티티 정의의 최상위 블록. 주로 아이템이 쓴다 (추정: 파일 구조. game_entity와의 차이는 미확인) | 파일 구조와 값에서 추론 |  |
| `/actor/("armor_fragile")` | macro | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/actor/(include)` | include | [str]x1696 | 1694 | 1696 | "/properties/stuff_small.ext" ¦ "/properties/stuff_weapon.ext" ¦ "/properties/stuff_grenade.ext" ¦ "/properties/stuff_mine.ext" ¦ "/properties/stuff_grenade_mine.ext" ¦ "/properties/shadow/round.inc" ¦ "/properties/construction.inc" | 미확인(자동) |  |  |  |
| `/actor/bone` | block | [str]x1 | 1 | 1 | "foresight3" | 미확인(자동) |  |  |  |
| `/actor/bone/limits` | block | [num num]x1 | 1 | 1 | -0 15 | 추정 | 회전 한계 각도 두 개 (추정: 주석 "elevation range = 0 +70") | 게임 파일 주석 |  |
| `/actor/bone/speed2` | block | [num]x1 | 1 | 1 | 10 | 추정 | 회전 속도 (추정: 주석 "manual handcrank, extremely slow") | 게임 파일 주석 |  |
| `/actor/collider` | block | [str]x17 | 17 | 17 | "mine.at" ¦ "mine.ap" ¦ "stuff" | 추정 | 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") | 파일 구조와 값에서 추론 |  |
| `/actor/extender[mine]` | block | [str]x18 | 18 | 18 | "mine" | 미확인(자동) |  |  |  |
| `/actor/extender[mine]/time` | block | [num]x18 | 18 | 18 | 3 | 미확인(자동) |  |  | actual mining time equals the lenght of time for the human animation + this value if value is less than animation lenght. Otherwise this value is an absolute and animation will loop. ¦ actual mining time equals the length of time for the human animation + this value if value is less than animation length. Otherwise this value is an absolute and animation will loop. |
| `/actor/extender[stuff]` | block | [str]x1694 | 1694 | 1694 | "stuff" | 추정 | 아이템 기능. 이 엔티티를 set/stuff의 아이템과 잇는다 (추정: 하위 item) | 파일 구조와 값에서 추론 |  |
| `/actor/extender[stuff]/item` | block | [str]x1689 [str atom]x5 | 1694 | 1694 | 예: "sov_cap" | 추정 | 연결된 아이템 이름. set/stuff의 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 |  |
| `/actor/extension` | block | [str]x1694 | 1694 | 1694 | 예: "sov_cap.mdl" | 추정 | 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | 파일 구조와 값에서 추론 |  |
| `/actor/lodclass` | block | [str]x1 | 1 | 1 | "stuff_weapon" | 추정 | 디테일 단계(LOD) 분류 이름 (추정: 이름과 값 "vehicle", "stuff_small". set/level_of_detail.set 참조로 보임) | 파일 구조와 값에서 추론 |  |
| `/actor/nogroupmesh` | block | [num]x188 | 188 | 188 | 1 | 미확인 | 값은 1뿐. 용도 불명 |  |  |
| `/actor/obstacle` | block | [str]x18 | 18 | 18 | "body" | 미확인(자동) |  |  |  |
| `/actor/obstacle/circle2` | block | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |  |
| `/actor/obstacle/circle2/radius` | block | [num]x18 | 18 | 18 | 7.5 ~ 20 | 미확인(자동) |  |  |  |
| `/actor/obstacleid` | block | [str]x5 | 5 | 5 | "nothing" | 추정 | 장애물 분류 이름 (추정: 값 "tank", "stuff") | 파일 구조와 값에서 추론 |  |
| `/actor/props` | block | [str]x147 [str str]x26 [str str str str]x3 [str str str]x2 | 176 | 178 | 예: "headwear_rnd" | 추정 | 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/actor/texmod` | block | [str]x3 | 3 | 3 | "black" ¦ "green" ¦ "camo" | 미확인(자동) |  |  |  |
| `/actor/volume` | block | [str]x82 | 63 | 82 | "body" ¦ "body1" ¦ "select" ¦ "body2" ¦ "casing1" ¦ "casing2" ¦ "shield" ¦ "shell1" ¦ "shell2" | 추정 | 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able` | block | [atom]x23 []x2 | 20 | 25 | clear ¦ | 추정 | 볼륨이 반응하는 상호작용 묶음 (추정: 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/blast` | block | []x18 | 17 | 18 | 예: | 추정 | 폭발에 반응하는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/bullet` | block | []x20 | 18 | 20 | 예: | 추정 | 총탄에 맞는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/contact` | block | []x22 | 18 | 22 | 예: | 추정 | 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/contact_ground` | block | []x22 | 18 | 22 | 예: | 추정 | 지면 접촉 판정 대상 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/obstacle` | block | [num]x1 | 1 | 1 | 0 | 미확인 | able의 하위. 값 0 |  |  |
| `/actor/volume/able/select` | block | []x23 | 19 | 23 | 예: | 추정 | 마우스 선택 대상 (추정: able의 하위 플래그, 볼륨 이름 "select") | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/touch` | block | []x24 | 20 | 24 | 예: | 미확인 | able의 하위 플래그. 용도 불명 |  |  |
| `/actor/volume/able/visible` | block | []x3 [num]x2 | 5 | 5 | 0 | 미확인 | able의 하위. 값 0 |  |  |
| `/actor/volume/density` | block | [num]x19 | 19 | 19 | 0 | 미확인 | 값 0~2. 볼륨 밀도로 보이나 단위와 용도를 확인하지 못함 |  |  |
| `/actor/volume/material` | block | [str]x10 | 10 | 10 | "wood" ¦ "glass" | 추정 | 볼륨의 재질 이름. set/material.set 참조로 보인다 (추정: 값 "wood") | 파일 구조와 값에서 추론 |  |
| `/actor/volume/tags` | block | [str]x19 | 16 | 19 | "body" | 미확인(자동) |  |  |  |
| `/actor/volume/thickness` | block | [num]x40 | 36 | 40 | 1 ~ 10 | 추정 | 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 게임 파일 주석 |  |
| `/entity` | block | []x13 | 13 | 13 | 예: | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 |  |
| `/entity/(include)` | include | [str]x10 | 10 | 10 | "/properties/stuff_small.ext" ¦ "/properties/stuff_grenade.ext" ¦ "/properties/stuff_weapon.ext" | 미확인(자동) |  |  |  |
| `/entity/collider` | block | [str]x1 | 1 | 1 | "grenade" | 추정 | 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") | 파일 구조와 값에서 추론 |  |
| `/entity/extender[stuff]` | block | [str]x10 | 10 | 10 | "stuff" | 미확인(자동) |  |  |  |
| `/entity/extender[stuff]/item` | block | [str]x10 | 10 | 10 | "bandage_eng" ¦ "bandage_fin" ¦ "bandage_ger" ¦ "bandage_rus" ¦ "bandage_usa" ¦ "canned_meat" ¦ "human" ¦ "wirecutters" ¦ "grenade no73" ¦ "boys_mk1" | 추정 | 아이템 정의의 최상위 블록 (추정: headgear.pattern 등이 이 블록으로 시작) | 파일 구조와 값에서 추론 |  |
| `/entity/extension` | block | [str]x10 | 10 | 10 | "bandage_ger.mdl" ¦ "bandage_eng.mdl" ¦ "bandage_rus.mdl" ¦ "bandage_usa.mdl" ¦ "canned_meat.mdl" ¦ "corpse.mdl" ¦ "wirecutters.mdl" ¦ "no73_at_grenade.mdl" ¦ "boys_mk1.mdl" | 추정 | 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | 파일 구조와 값에서 추론 |  |
| `/entity/nogroupmesh` | block | [num]x4 | 4 | 4 | 1 | 미확인 | 값은 1뿐. 용도 불명 |  |  |
| `/entity/props` | block | [str]x3 | 3 | 3 | "physics_damping" ¦ "contact" ¦ "bipod" | 추정 | 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/entity/spriteview` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/orientation` | block | [atom]x3 | 3 | 3 | trace | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite` | block | [str]x3 | 3 | 3 | "flag1.ebm" ¦ "flag2.ebm" | 추정 | 표시에 쓰는 이미지 파일 (추정: 값이 .ebm) | 파일 구조와 값에서 추론 |  |
| `/entity/spriteview/sprite/light` | block | []x1 | 1 | 1 | 예: | 미확인 | 값 1 |  |  |
| `/entity/spriteview/sprite/light/linear` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/light/linear/max` | block | [num]x1 | 1 | 1 | 0.2 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/light/linear/min` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/preturn` | block | [num]x3 | 3 | 3 | 90 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/scale` | block | [num]x2 []x1 | 3 | 3 | 0.3 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/scale/linear` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/scale/linear/max` | block | [num]x1 | 1 | 1 | 0.35 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/scale/linear/min` | block | [num]x1 | 1 | 1 | 0.6 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/timer` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/timer/duration` | block | [num]x1 | 1 | 1 | 1.2 | 미확인(자동) |  |  |  |
| `/entity/volume` | block | [str]x2 | 2 | 2 | "body" | 추정 | 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | 파일 구조와 값에서 추론 |  |
| `/entity/volume/able` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/volume/able/contact` | block | []x1 | 1 | 1 | 예: | 추정 | 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) | 파일 구조와 값에서 추론 |  |
| `/game_entity` | block | []x545 | 545 | 545 | 예: | 추정 | 엔티티 정의의 최상위 블록. 차량·화포·보병 외형·일부 아이템이 쓴다 (추정: 파일 구조) | 파일 구조와 값에서 추론 |  |
| `/game_entity/(include)` | include | [str]x535 | 512 | 535 | "/properties/stuff_small.ext" ¦ "/properties/stuff_weapon.ext" ¦ "/properties/stuff.ext" ¦ "/properties/shadow/round.inc" ¦ "/properties/stuff_bomb.ext" ¦ "/properties/stuff_dynamite.ext" ¦ "/properties/small_part_harmless.inc" ¦ "/properties/stuff_small_bag.ext" ¦ "/properties/knife.ext" ¦ "/properties/construction.inc" | 미확인(자동) |  |  |  |
| `/game_entity/body` | block | []x5 | 5 | 5 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/body/proper` | block | [atom atom]x5 | 5 | 5 | body sensible | 미확인(자동) |  |  |  |
| `/game_entity/collider` | block | [str]x393 | 393 | 393 | "ammo" ¦ "stuff.construction" ¦ "construction" | 추정 | 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") | 파일 구조와 값에서 추론 |  |
| `/game_entity/constrain` | block | [atom]x28 | 28 | 28 | none | 미확인 | 값은 ground, none 등. 배치 제약으로 보이나 확인하지 못함 |  |  |
| `/game_entity/extender[inventory]` | block | [str]x12 | 12 | 12 | "inventory" | 추정 | 차량 화물칸 기능 (추정: 하위 box) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extender[inventory]/box` | block | []x12 | 12 | 12 | 예: | 추정 | 화물칸 하나 (추정: 하위 size, weight, item) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extender[inventory]/box/item` | block | [str num]x25 | 6 | 25 | 예: "bandage_rus" 2 | 추정 | 실려 있는 아이템과 수량 (추정: 값 "ammo mgun_usa belt" 1000) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extender[inventory]/box/size` | block | [num num]x12 | 12 | 12 | 8 6 ¦ 4 4 ¦ 4 6 ¦ 6 6 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 |  |
| `/game_entity/extender[inventory]/box/weight` | block | [num]x6 | 6 | 6 | 600 ~ 5000 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 |  |
| `/game_entity/extender[stuff]` | block | [str]x504 | 504 | 504 | "stuff" | 추정 | 아이템 기능. 이 엔티티를 set/stuff의 아이템과 잇는다 (추정: 하위 item) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extender[stuff]/item` | block | [str]x504 | 504 | 504 | 예: "air_bomb_eng_250lb" | 추정 | 연결된 아이템 이름. set/stuff의 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extension` | block | [str]x545 | 545 | 545 | 예: "250lb_gp_mk4.mdl" | 추정 | 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | 파일 구조와 값에서 추론 |  |
| `/game_entity/fogable` | block | [atom]x30 | 30 | 30 | never ¦ always | 미확인(자동) |  |  |  |
| `/game_entity/lodclass` | block | [str]x20 | 20 | 20 | "tracer" ¦ "casing" ¦ "decor_small" | 추정 | 디테일 단계(LOD) 분류 이름 (추정: 이름과 값 "vehicle", "stuff_small". set/level_of_detail.set 참조로 보임) | 파일 구조와 값에서 추론 |  |
| `/game_entity/mass` | block | [num]x3 | 3 | 3 | 30 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 |  |
| `/game_entity/nogetshadows` | block | []x20 | 20 | 20 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/nogroupmesh` | block | [num]x79 | 79 | 79 | 1 | 미확인 | 값은 1뿐. 용도 불명 |  |  |
| `/game_entity/obstacleid` | block | [str]x2 | 2 | 2 | "stuff" | 추정 | 장애물 분류 이름 (추정: 값 "tank", "stuff") | 파일 구조와 값에서 추론 |  |
| `/game_entity/props` | block | [str str]x285 [str]x33 [str str str]x9 [str str str str str str]x4 [str str str str str]x2 | 332 | 333 | 예: "steel" "explosive_stuff" | 추정 | 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/game_entity/scale` | block | [num]x4 | 4 | 4 | 0.85 ~ 1.5 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview` | block | []x60 | 20 | 60 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/ground` | block | []x40 | 20 | 40 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/game_entity/spriteview/layer` | block | [atom]x40 | 20 | 40 | grounddynamic | 미확인 | 값 alpha |  |  |
| `/game_entity/spriteview/offset` | block | [num num num]x60 | 20 | 60 | 0 0 5 ¦ 0 0 25 ¦ 0 0 3 | 추정 | 표시 위치 오프셋 세 값 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/game_entity/spriteview/sprite` | block | [str]x60 | 20 | 60 | "pointer_halo" ¦ "mine_f.ebm" ¦ "pointer_clr.ebm" | 추정 | 표시에 쓰는 이미지 파일 (추정: 값이 .ebm) | 파일 구조와 값에서 추론 |  |
| `/game_entity/spriteview/sprite/highlight` | block | [atom]x60 | 20 | 60 | 0xFF5000 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/light` | block | []x60 | 20 | 60 | 예: | 미확인 | 값 1 |  |  |
| `/game_entity/spriteview/sprite/light/peak` | block | []x60 | 20 | 60 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/light/peak/max` | block | [num]x60 | 20 | 60 | 0.4 ~ 0.5 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/light/peak/min` | block | [num]x60 | 20 | 60 | 0 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/scale` | block | [num]x60 | 20 | 60 | 0.1 ~ 0.8 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/scale/linear` | block | []x40 | 20 | 40 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/scale/linear/max` | block | [num]x40 | 20 | 40 | 0.03 ~ 0.1 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/scale/linear/min` | block | [num]x40 | 20 | 40 | 0.15 ~ 0.3 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/timer` | block | []x60 | 20 | 60 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/timer/duration` | block | [num]x60 | 20 | 60 | 1 ~ 2 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/timer/mode` | block | [atom atom]x40 [atom]x20 | 20 | 60 | play loop ¦ play | 미확인 | 값 detonate |  |  |
| `/game_entity/spriteview/sprite/visibility` | block | [num]x60 | 20 | 60 | 0.3 ~ 0.5 | 미확인(자동) |  |  |  |
| `/game_entity/texmod` | block | [str]x1 | 1 | 1 | "ger" | 미확인(자동) |  |  |  |
| `/game_entity/volume` | block | [str]x412 | 410 | 412 | "body" ¦ "fuel" ¦ "body1" ¦ "body2" ¦ "select" | 추정 | 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able` | block | []x285 [atom]x9 | 294 | 294 | ¦ clear | 추정 | 볼륨이 반응하는 상호작용 묶음. clear는 상속된 설정을 지우는 것으로 보인다 (추정: 하위가 bullet, blast, touch 등 인자 없는 플래그) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/blast` | block | []x288 | 288 | 288 | 예: | 추정 | 폭발에 반응하는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/bullet` | block | []x284 | 284 | 284 | 예: | 추정 | 총탄에 맞는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/contact` | block | []x13 | 13 | 13 | 예: | 추정 | 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/contact_ground` | block | []x12 | 12 | 12 | 예: | 추정 | 지면 접촉 판정 대상 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/obstacle` | block | [num]x1 | 1 | 1 | 0 | 미확인 | able의 하위. 값 0 |  |  |
| `/game_entity/volume/able/select` | block | []x12 | 12 | 12 | 예: | 추정 | 마우스 선택 대상 (추정: able의 하위 플래그, 볼륨 이름 "select") | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/touch` | block | []x293 | 293 | 293 | 예: | 미확인 | able의 하위 플래그. 용도 불명 |  |  |
| `/game_entity/volume/density` | block | [num]x2 | 2 | 2 | 2 | 미확인 | 값 0~2. 볼륨 밀도로 보이나 단위와 용도를 확인하지 못함 |  |  |
| `/game_entity/volume/tags` | block | [str]x9 | 9 | 9 | "body" ¦ "pour_over" | 추정 | 볼륨 태그. 재질이나 부위를 나타낸다 (추정: 값 "wood", "wheel", "body") | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/thickness` | block | [num]x393 | 392 | 393 | 1 ~ 50 | 추정 | 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 게임 파일 주석 |  |

### 파일 종류: .mdl

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/extender[preview]` | block | [str]x2019 | 2019 | 2019 | "preview" | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"` | block | [str]x3809 | 2019 | 3809 | "stuff" ¦ "stuff_preview" ¦ "stuff_primary" ¦ "stuff_secondary" | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/camera` | block | []x3728 | 2019 | 3728 | 예: | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/camera/fov` | block | [num]x3728 | 2019 | 3728 | 1.5 ~ 75.2894 | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/camera/horizon` | block | [num]x1072 | 682 | 1072 | -180 ~ 180 | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/camera/lookat` | block | [num num num]x3728 | 2019 | 3728 | 예: -2.4781 -0.4306 0.113 | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/camera/origin` | block | [num num num]x3728 | 2019 | 3728 | 예: -2.2191 -45.3434 7.8296 | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights` | block | []x3809 | 2019 | 3809 | 예: | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights/"*"` | block | [str]x11323 | 2019 | 11323 | "0" ¦ "1" ¦ "2" ¦ "3" | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights/"*"/ambient` | block | [atom]x2221 | 1118 | 2221 | 0x5f5f5f ¦ 0xff5f5f5f ¦ 0x818181 ¦ 0x757575 ¦ 0x1d1d1d ¦ 0x858585 ¦ 0x6c6c6c ¦ 0x141414 ¦ 0x7f7f7f | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights/"*"/diffuse` | block | [atom]x925 | 241 | 925 | 예: 0xffe2ecff | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights/"*"/direction` | block | [num num num]x4693 [atom num num]x174 [num atom num]x4 | 2019 | 4871 | 예: -0.01 0.4999 0.0036 | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights/"*"/phi` | block | [num]x3 | 1 | 3 | 45 ~ 55.1 | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights/"*"/position` | block | [num num num]x6455 | 1723 | 6455 | 예: 8.6568 -3.1885 51.1825 | 미확인(자동) |  |  | X (Forward/Backward) Y (Left/Right) Z (Up/Down) |
| `/extender[preview]/"*"/lights/"*"/theta` | block | [num]x3 | 1 | 3 | 43 ~ 53.1 | 미확인(자동) |  |  |  |
| `/extender[preview]/"*"/lights/"*"/type` | block | [atom]x11323 | 2019 | 11323 | point ¦ directional ¦ spot | 미확인(자동) |  |  |  |
| `/obstacle` | block | [str]x10 | 3 | 10 | "box_metal" ¦ "rectangle01" ¦ "body1" ¦ "body2" ¦ "body3" ¦ "body4" ¦ "body5" ¦ "body6" ¦ "body7" ¦ "body" | 미확인(자동) |  |  |  |
| `/obstacle/obb2` | block | []x10 | 3 | 10 | 예: | 미확인(자동) |  |  |  |
| `/obstacle/obb2/axis` | block | [num num]x10 | 3 | 10 | 1 0 ¦ -1 0 | 미확인(자동) |  |  |  |
| `/obstacle/obb2/center` | block | [num num]x10 | 3 | 10 | 0.0101357 -0.010411 ¦ 0.132092 -0.152341 ¦ 75.9552 30.5962 ¦ 38.2049 -39.1249 ¦ 38.2049 39.8831 ¦ -38.6588 -38.9574 ¦ -38.6588 40.391 ¦ -74.4975 -28.1558 ¦ -74.4975 30.565 ¦ 75.9552 -28.2809 | 미확인(자동) |  |  |  |
| `/obstacle/obb2/extent` | block | [num num]x10 | 3 | 10 | 0.984253 0.984253 ¦ 5.98969 4.06327 ¦ 10.437 6.03591 | 미확인(자동) |  |  |  |
| `/obstacle/rotate` | block | []x10 | 3 | 10 | 예: | 미확인(자동) |  |  |  |
| `/skeleton` | block | []x2047 | 2047 | 2047 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/animation` | block | []x151 | 151 | 151 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/animation/sequence` | block | [str]x274 | 151 | 274 | 예: "start" | 미확인(자동) |  |  |  |
| `/skeleton/animation/sequence/events` | block | []x19 | 7 | 19 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/animation/sequence/events/#row` | block | [num str]x19 | 7 | 19 | 15 "eject" | 미확인(자동) |  |  |  |
| `/skeleton/animation/sequence/speed` | block | [num]x274 | 151 | 274 | 0.05 ~ 4 | 미확인(자동) |  |  |  |
| `/skeleton/bone` | block | [str]x2044 [atom str]x5 | 2047 | 2049 | 예: "bomb" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone` | block | [str]x2481 [atom str]x1 | 1557 | 2482 | 예: "tail" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/animation` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/animation/sequence` | block | [str]x1 | 1 | 1 | "recoil" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/animation/sequence/speed` | block | [num]x1 | 1 | 1 | 2.5 | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone` | block | [str]x94 | 55 | 94 | 예: "rope" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/bone` | block | [str]x27 | 12 | 27 | "fuel_tank" ¦ "bullet_4" ¦ "bullet_5" ¦ "bullet_3" ¦ "bullet_6" ¦ "bullet_1" ¦ "bullet_2" ¦ "bolt2" ¦ "stock_2" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/bone/lodview` | block | []x27 | 12 | 27 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/bone/lodview/volumeview` | block | [str]x121 | 12 | 121 | 예: "bullet_4.ply" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x8 | 3 | 8 | 1 0 0 0 1 0 0 0 1 -4.2101 0.0288 0.5672 ¦ 0.4384 0 -0.8988 0 1 0 0.8988 0 0.4384 -2.6776 0 -1.687 ¦ 0 -1 0 1 0 0 0 0 1 -0.2266 0.275 0 ¦ 0 -1 0 1 0 0 0 0 1 -0.2266 0.1376 0.2407 ¦ 0 -1 0 1 0 0 0 0 1 -0.2266 -0.141 0.2407 ¦ 0 -1 0 1 0 0 0 0 1 -0.2266 -0.2782 0.0012 ¦ 0 -1 0 1 0 0 0 0 1 -0.2266 -0.1407 -0.2406 ¦ 0 -1 0 1 0 0 0 0 1 -0.2266 0.1393 -0.2406 | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/bone/position` | block | [num num num]x19 | 9 | 19 | 예: -0.1925 -0.2457 0.002 | 미확인(자동) |  |  | X (Forward/Backward) Y (Left/Right) Z (Up/Down) |
| `/skeleton/bone/bone/bone/lodview` | block | []x76 | 42 | 76 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/lodview/volumeview` | block | [str]x286 | 42 | 286 | 예: "bipod_leg_l.ply" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x46 | 28 | 46 | 예: -0.502634 0.859419 -0.0935912 0.85984 0.485759 -0.157204 -0.089642 -0.15949 -0.983121 -5.264 -4.17848 1.0062 | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/bone/parameters` | block | [str]x5 | 4 | 5 | "id=gun;" ¦ "id=mgun;" ¦ "id=body;" | 미확인(자동) |  |  | "} ¦ Ground;"} |
| `/skeleton/bone/bone/bone/position` | block | [num num num]x46 | 30 | 46 | 예: -0.4819 -0.0512 -1.866 | 미확인(자동) |  |  | X (Forward/Backward) Y (Left/Right) Z (Up/Down) |
| `/skeleton/bone/bone/bone/volumeview` | block | [str]x6 | 6 | 6 | "rope.ply" ¦ "belt.ply" ¦ "front.ply" ¦ "glass.ply" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/lodview` | block | []x2602 | 1341 | 2602 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/lodview/volumeview` | block | [str]x9113 | 1341 | 9113 | 예: "20mm_eng_auto_ap.ply" | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/matrix34` | block | [num num num num num num num num num num num num]x852 [num num num num num atom num atom num num num num]x1 | 346 | 853 | 예: -1 0 0 0 0 -1 0 -1 0 -5.2015 0 0 | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/orientation` | block | [num num num num num num num num num]x4 | 3 | 4 | -1 0 0 0 1 0 0 0 -1 ¦ 1 0 0 0 1 0 0 0 1 ¦ 0 0 1 0 1 0 -1 0 0 | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/parameters` | block | [str]x53 | 21 | 53 | "ID=gun;" ¦ "id=mgun;" ¦ "ID=body;" ¦ "LastPose = undefined;" ¦ "commonmesh;" ¦ "ID=mgun;LastPose = undefined;" ¦ "commonmesh;id=body;" ¦ "id=lamp;" | 미확인(자동) |  |  | "} ¦ Ground;"} |
| `/skeleton/bone/bone/position` | block | [num num num]x367 | 158 | 367 | 예: -1 0 0 | 미확인(자동) |  |  | X (Forward/Backward) Y (Left/Right) Z (Up/Down) |
| `/skeleton/bone/bone/visibility` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/skeleton/bone/bone/volumeview` | block | [str]x67 | 46 | 67 | 예: "rod.ply" | 미확인(자동) |  |  |  |
| `/skeleton/bone/lodview` | block | []x1070 | 754 | 1070 | 예: | 미확인(자동) |  |  |  |
| `/skeleton/bone/lodview/volumeview` | block | [str]x3377 | 754 | 3377 | 예: "bomb.ply" | 미확인(자동) |  |  |  |
| `/skeleton/bone/orientation` | block | [num num num num num num num num num]x2047 | 2047 | 2047 | 1 0 0 0 -1 0 0 0 1 ¦ 0.95 0 0 0 -0.95 0 0 0 0.95 ¦ 1.21 0 0 0 -1.21 0 0 0 1.21 ¦ 0.75 0 0 0 -0.75 0 0 0 0.75 ¦ 1.15 0 0 0 -1.15 0 0 0 1.15 | 미확인(자동) |  |  |  |
| `/skeleton/bone/parameters` | block | [str]x79 | 79 | 79 | "id=mgun;" ¦ "id=gun;" ¦ "LastPose = undefined;" ¦ "CommonMesh;" ¦ "CommonMesh;Ground;" | 미확인(자동) |  |  | "} ¦ Ground;"} |
| `/skeleton/bone/position` | block | [num num num]x3 | 3 | 3 | -0.25 0 0 ¦ -0.2 0 0.1 ¦ 0 0 -10 | 미확인(자동) |  |  | X (Forward/Backward) Y (Left/Right) Z (Up/Down) |
| `/skeleton/bone/volumeview` | block | [str]x83 | 83 | 83 | 예: "body.ply" | 미확인(자동) |  |  |  |
| `/volume` | block | [str]x4220 | 2043 | 4220 | 예: "body" | 미확인(자동) |  |  |  |
| `/volume/bone` | block | [str]x4220 | 2043 | 4220 | 예: "bomb" | 미확인(자동) |  |  |  |
| `/volume/box` | block | [num num num]x3102 | 1554 | 3102 | 예: 9.81412 1.30072 1.32108 | 미확인(자동) |  |  |  |
| `/volume/cylinder` | block | [num num]x669 | 366 | 669 | 예: 2.66361 30.2001 | 미확인(자동) |  |  |  |
| `/volume/matrix34` | block | [num atom num num num num atom num atom num num num]x2388 [num num num num num num num num num num num num]x1375 [num num num num num num num num num num num atom]x27 [num num num num num num num num num num atom num]x2 [num num num num num num atom num atom num num num]x2 | 1938 | 3798 | 예: 0 -1 0 0 0 1 -1 0 0 0.9594 -0.2288 0.2391 | 미확인(자동) |  |  |  |
| `/volume/orientation` | block | [num num num num num num num num num]x5 | 4 | 5 | 1 0 0 0 1 0 0 0 1 ¦ -0.9986 0 -0.0523 0 1 0 0.0523 0 -0.9986 ¦ 0 0 -1 0 1 0 1 0 0 ¦ 0 0 -1 -0.0698 0.9976 0 0.9976 0.0698 0 | 미확인(자동) |  |  |  |
| `/volume/polyhedron` | block | [str]x444 | 246 | 444 | "body.vol" ¦ "select.vol" ¦ "magazine.vol" ¦ "body2.vol" ¦ "body1.vol" ¦ "netting.vol" | 미확인(자동) |  |  |  |
| `/volume/position` | block | [num num num]x404 | 241 | 404 | 예: 0.4341 0.7165 0.0028 | 미확인(자동) |  |  | X (Forward/Backward) Y (Left/Right) Z (Up/Down) |
| `/volume/sphere` | block | [num]x5 | 3 | 5 | 1.20823 ~ 7.11538 | 미확인(자동) |  |  |  |

### 파일 종류: .mtl

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/material` | block | [atom]x1459 | 1459 | 1459 | bump ¦ simple ¦ envmap | 미확인(자동) |  |  |  |
| `/material/alpharef` | block | [num]x72 | 72 | 72 | 120 | 미확인(자동) |  |  |  |
| `/material/alphatocoverage` | block | []x29 | 29 | 29 | 예: | 미확인(자동) |  |  |  |
| `/material/blend` | block | [atom]x1458 | 1458 | 1458 | none ¦ test ¦ blend | 미확인(자동) |  |  |  |
| `/material/bump` | block | [str]x1329 | 1329 | 1329 | 예: "$/_ammo/500lb_mc_bomb_bp" | 미확인(자동) |  |  |  |
| `/material/color` | block | [str]x1093 | 1093 | 1093 | 예: "230 230 230 25" | 미확인(자동) |  |  |  |
| `/material/diffuse` | block | [str]x1459 | 1459 | 1459 | 예: "$/_ammo/250lb_gp_bomb" | 미확인(자동) |  |  |  |
| `/material/diffuse/alphachannel` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/material/diffuse/mipmap` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/material/envmap` | block | [str]x1 | 1 | 1 | "$/_vehicle/general/envmap_gold01" | 미확인(자동) |  |  |  |
| `/material/full_specular` | block | []x388 | 388 | 388 | 예: | 미확인(자동) |  |  |  |
| `/material/gloss_scale` | block | [num]x225 | 225 | 225 | 0.35 ~ 0.9 | 미확인(자동) |  |  |  |
| `/material/lightmap` | block | [str]x2 | 2 | 2 | "$/_lm/b33r_lm" | 미확인(자동) |  |  |  |
| `/material/mask` | block | [str]x2 | 2 | 2 | "$/_weapon/lamp_sp" ¦ "$/_vehicle/rus/mo4_alpha_sp" | 미확인(자동) |  |  |  |
| `/material/specular` | block | [str]x1354 | 1354 | 1354 | 예: "$/_ammo/500lb_mc_bomb_sp" | 미확인(자동) |  |  |  |
| `/material/specular_intensity` | block | [num]x234 | 234 | 234 | 0.75 ~ 1.55 | 미확인(자동) |  |  |  |
| `/material/tile` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |

## 펼친 뒤의 키

include와 매크로를 펼친 뒤 유닛 안에서의 위치다. 펼치는 규칙은 추정이며 한계는 coverage.md에 있다. 고유 키 180개.

### 유닛 종류: item-entity

| 경로 | 형태 | 값 형태 | 파일 수 | 횟수 | 값 범위·예 | 등급 | 의미 | 출처 | 같은 이름 키의 원문 주석(참고) |
|---|---|---|---|---|---|---|---|---|---|
| `/actor` | block | [atom]x1694 | 1694 | 1694 | game_entity | 추정 | 엔티티 정의의 최상위 블록. 주로 아이템이 쓴다 (추정: 파일 구조. game_entity와의 차이는 미확인) | 파일 구조와 값에서 추론 |  |
| `/actor/armor` | block | []x22 | 19 | 22 | 예: | 추정 | 장갑 판정 설정 묶음 (추정: 하위가 setting/threshold) | 파일 구조와 값에서 추론 |  |
| `/actor/armor/setting` | block | [str]x26 | 19 | 26 | "body" ¦ "roof" | 추정 | 장갑 설정 한 벌. 문자열은 설정 이름 (추정: 값 "body") | 파일 구조와 값에서 추론 |  |
| `/actor/armor/setting/ray_cast_thickness` | block | []x4 | 1 | 4 | 예: | 미확인(자동) |  |  |  |
| `/actor/armor/setting/thickness` | block | [num]x18 | 18 | 18 | 20 | 추정 | 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 게임 파일 주석 | 15 + 4 ¦ 89 + 89 |
| `/actor/armor/setting/threshold` | block | []x8 | 1 | 8 | 예: | 추정 | 피해가 들어가기 시작하는 기준값 묶음 (추정: 하위 blast/pierce의 주석) | 파일 구조와 값에서 추론 |  |
| `/actor/armor/setting/threshold/blast` | block | []x4 | 1 | 4 | 예: | 추정 | 폭발에 반응하는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break |
| `/actor/armor/setting/threshold/blast/value` | block | [num]x4 | 1 | 4 | 2 ~ 13 | 미확인(자동) |  |  |  |
| `/actor/armor/setting/threshold/pierce` | block | []x4 | 1 | 4 | 예: | 확인 | 관통 피해 기준. 장갑에 도탄·관통 자국을 남기는 데 필요한 최소 관통력 | 게임 파일 주석 | Minimum penetration necessary to leave a ricochet/pen mark on the armour |
| `/actor/armor/setting/threshold/pierce/koef` | block | [num]x4 | 1 | 4 | 1 | 확인 | 계수(coefficient). 기준값 = koef x 볼륨의 유효 두께 | 게임 파일 주석 |  |
| `/actor/armor/setting/volumes` | block | [str]x34 | 19 | 34 | "body*" ¦ "box*" ¦ "wire*" ¦ "wires*" ¦ "wood*" ¦ "door*" ¦ "roof*" ¦ "floor*" | 추정 | 이 설정이 적용되는 볼륨 이름 패턴. 예: "body*" (추정: 값) | 파일 구조와 값에서 추론 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading |
| `/actor/body` | block | []x36 | 36 | 36 | 예: | 미확인(자동) |  |  |  |
| `/actor/body/permit` | block | [atom]x36 | 36 | 36 | body | 미확인(자동) |  |  |  |
| `/actor/body/proper` | block | [atom atom]x36 | 36 | 36 | body sensible | 미확인(자동) |  |  |  |
| `/actor/bone` | block | [str]x1 | 1 | 1 | "foresight3" | 미확인(자동) |  |  | Boys rot ¦ turret |
| `/actor/bone/limits` | block | [num num]x1 | 1 | 1 | -0 15 | 추정 | 회전 한계 각도 두 개 (추정: 주석 "elevation range = 0 +70") | 게임 파일 주석 | AA mode limits ¦ starting point is +20Â° elevation range = 0Â° +70Â° |
| `/actor/bone/speed2` | block | [num]x1 | 1 | 1 | 10 | 추정 | 회전 속도 (추정: 주석 "manual handcrank, extremely slow") | 게임 파일 주석 | manual handcrack, extremely slow ¦ remote controlled from inside |
| `/actor/collider` | block | [str]x1747 | 1694 | 1747 | "stuff" ¦ "grenade" ¦ "nothing" ¦ "mine.at" ¦ "mine.ap" ¦ "mine.ap.grenade" ¦ "construction" | 추정 | 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") | 파일 구조와 값에서 추론 |  |
| `/actor/constrain` | block | [atom]x1693 | 1693 | 1693 | ground | 미확인 | 값은 ground, none 등. 배치 제약으로 보이나 확인하지 못함 |  |  |
| `/actor/extender[mine]` | block | [str]x36 | 18 | 36 | "mine" | 미확인(자동) |  |  |  |
| `/actor/extender[mine]/time` | block | [num]x18 | 18 | 18 | 3 | 미확인(자동) |  |  | actual mining time equals the lenght of time for the human animation + this value if value is less than animation lenght. Otherwise this value is an absolute and animation will loop. ¦ actual mining time equals the length of time for the human animation + this value if value is less than animation length. Otherwise this value is an absolute and animation will loop. |
| `/actor/extender[stuff]` | block | [str]x3369 | 1694 | 3369 | "stuff" | 추정 | 아이템 기능. 이 엔티티를 set/stuff의 아이템과 잇는다 (추정: 하위 item) | 파일 구조와 값에서 추론 |  |
| `/actor/extender[stuff]/item` | block | [str]x1689 [str atom]x5 | 1694 | 1694 | 예: "sov_cap" | 추정 | 연결된 아이템 이름. set/stuff의 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | 128+8 ¦ 80+48+8 |
| `/actor/extension` | block | [str]x1694 | 1694 | 1694 | 예: "sov_cap.mdl" | 추정 | 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | 파일 구조와 값에서 추론 | needs full QA |
| `/actor/lodclass` | block | [str]x3545 | 1694 | 3545 | "stuff_small" ¦ "stuff_weapon" ¦ "stuff_grenade" ¦ "stuff_mine" ¦ "decor_big" | 추정 | 디테일 단계(LOD) 분류 이름 (추정: 이름과 값 "vehicle", "stuff_small". set/level_of_detail.set 참조로 보임) | 파일 구조와 값에서 추론 |  |
| `/actor/mass` | block | [num]x1 | 1 | 1 | 300 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 |
| `/actor/nogroupmesh` | block | [num]x1865 | 1678 | 1865 | 1 | 미확인 | 값은 1뿐. 용도 불명 |  |  |
| `/actor/novisibilitypatch` | block | []x1675 | 1675 | 1675 | 예: | 미확인 | 인자 없는 플래그. 용도 불명 |  |  |
| `/actor/obstacle` | block | [str]x18 | 18 | 18 | "body" | 미확인(자동) |  |  |  |
| `/actor/obstacle/circle2` | block | []x18 | 18 | 18 | 예: | 미확인(자동) |  |  |  |
| `/actor/obstacle/circle2/radius` | block | [num]x18 | 18 | 18 | 7.5 ~ 20 | 미확인(자동) |  |  | resupply radius in meter |
| `/actor/obstacleid` | block | [str]x1699 | 1694 | 1699 | "stuff" ¦ "nothing" ¦ "construction" | 추정 | 장애물 분류 이름 (추정: 값 "tank", "stuff") | 파일 구조와 값에서 추론 |  |
| `/actor/props` | block | [str]x1822 [str str str]x1677 [str str]x62 [str str str str str str]x16 [str str str str]x3 | 1694 | 3582 | 예: "steel" "stuff" "collectable" | 추정 | 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/actor/rimlight` | block | []x1677 | 1677 | 1677 | 예: | 미확인 | 인자 없는 플래그. 이름으로는 윤곽 조명으로 보이나 확인하지 못함 |  |  |
| `/actor/simulation` | block | []x1693 | 1693 | 1693 | 예: | 미확인 | 물리 시뮬레이션 설정 묶음으로 보이나 확인하지 못함 |  |  |
| `/actor/simulation/able` | block | [num]x1693 | 1693 | 1693 | 1 | 미확인 | 값 1 |  | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) |
| `/actor/simulation/disablelinked` | block | [num]x1677 | 1677 | 1677 | 1 | 미확인 | 값 1뿐. 용도 불명 |  |  |
| `/actor/simulatorid` | block | [str]x38 | 38 | 38 | "grenade" | 미확인(자동) |  |  |  |
| `/actor/skeleton` | block | []x19 | 19 | 19 | 예: | 추정 | 엔티티에 추가하는 뼈대 정의 (추정: 하위 bone/position) | 파일 구조와 값에서 추론 |  |
| `/actor/skeleton/bone` | block | [str]x18 | 18 | 18 | "mined_view" | 미확인(자동) |  |  | Boys rot ¦ turret |
| `/actor/skeleton/bone/matrix34` | block | [num num num num num num num num num num num num]x18 | 18 | 18 | 0 0 1 0 -1 0 1 0 0 0 0 0 | 미확인(자동) |  |  | 0 90 0 ¦ 15 85 0 |
| `/actor/skeleton/texmods` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/actor/skeleton/texmods/"*"` | block | [str str]x48 | 1 | 48 | 예: "_x" "_x" | 미확인(자동) |  |  |  |
| `/actor/texmod` | block | [str]x3 | 3 | 3 | "black" ¦ "green" ¦ "camo" | 미확인(자동) |  |  |  |
| `/actor/volume` | block | [str]x9486 | 1694 | 9486 | "body" ¦ "select" ¦ "body1" ¦ "body2" ¦ "shield" ¦ "fuel" ¦ "magazine" ¦ "casing1" ¦ "casing2" ¦ "shell1" ¦ "shell2" | 추정 | 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | 파일 구조와 값에서 추론 | Hull ¦ Superstructure |
| `/actor/volume/able` | block | []x6000 [atom]x3566 | 1694 | 9566 | ¦ clear | 추정 | 볼륨이 반응하는 상호작용 묶음 (추정: 하위 플래그) | 파일 구조와 값에서 추론 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) |
| `/actor/volume/able/blast` | block | []x1693 | 1675 | 1693 | 예: | 추정 | 폭발에 반응하는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break |
| `/actor/volume/able/bullet` | block | []x1695 | 1675 | 1695 | 예: | 추정 | 총탄에 맞는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/actor/volume/able/contact` | block | []x1697 | 1675 | 1697 | 예: | 추정 | 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/contact_ground` | block | []x1836 | 1675 | 1836 | 예: | 추정 | 지면 접촉 판정 대상 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/obstacle` | block | [num]x974 | 140 | 974 | 0 | 미확인 | able의 하위. 값 0 |  |  |
| `/actor/volume/able/select` | block | []x3373 | 1676 | 3373 | 예: | 추정 | 마우스 선택 대상 (추정: able의 하위 플래그, 볼륨 이름 "select") | 파일 구조와 값에서 추론 |  |
| `/actor/volume/able/touch` | block | []x3567 | 1694 | 3567 | 예: | 미확인 | able의 하위 플래그. 용도 불명 |  |  |
| `/actor/volume/able/visible` | block | [num]x5027 []x1678 | 1676 | 6705 | 0 | 미확인 | able의 하위. 값 0 |  | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. |
| `/actor/volume/density` | block | [num]x1750 | 1694 | 1750 | 0 | 미확인 | 값 0~2. 볼륨 밀도로 보이나 단위와 용도를 확인하지 못함 |  |  |
| `/actor/volume/material` | block | [str]x10 | 10 | 10 | "wood" ¦ "glass" | 추정 | 볼륨의 재질 이름. set/material.set 참조로 보인다 (추정: 값 "wood") | 파일 구조와 값에서 추론 |  |
| `/actor/volume/tags` | block | [str]x19 | 16 | 19 | "body" | 미확인(자동) |  |  | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set |
| `/actor/volume/thickness` | block | [num]x5204 | 1676 | 5204 | 1 ~ 10 | 추정 | 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 게임 파일 주석 | 15 + 4 ¦ 89 + 89 |
| `/actor/zclipvisibility` | block | [num]x1 | 1 | 1 | 0 | 미확인(자동) |  |  |  |
| `/entity` | block | [atom]x10 []x3 | 13 | 13 | game_entity ¦ | 추정 | 이 아이템의 엔티티 이름. entity/inventory의 .def 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | place holder |
| `/entity/body` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/body/permit` | block | [atom]x1 | 1 | 1 | body | 미확인(자동) |  |  |  |
| `/entity/body/proper` | block | [atom atom]x1 | 1 | 1 | body sensible | 미확인(자동) |  |  |  |
| `/entity/collider` | block | [str]x12 | 10 | 12 | "stuff" ¦ "grenade" | 추정 | 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") | 파일 구조와 값에서 추론 |  |
| `/entity/constrain` | block | [atom]x10 | 10 | 10 | ground | 미확인 | 값은 ground, none 등. 배치 제약으로 보이나 확인하지 못함 |  |  |
| `/entity/extender[stuff]` | block | [str]x20 | 10 | 20 | "stuff" | 미확인(자동) |  |  |  |
| `/entity/extender[stuff]/item` | block | [str]x10 | 10 | 10 | "bandage_eng" ¦ "bandage_fin" ¦ "bandage_ger" ¦ "bandage_rus" ¦ "bandage_usa" ¦ "canned_meat" ¦ "human" ¦ "wirecutters" ¦ "grenade no73" ¦ "boys_mk1" | 추정 | 아이템 정의의 최상위 블록 (추정: headgear.pattern 등이 이 블록으로 시작) | 파일 구조와 값에서 추론 | 128+8 ¦ 80+48+8 |
| `/entity/extension` | block | [str]x10 | 10 | 10 | "bandage_ger.mdl" ¦ "bandage_eng.mdl" ¦ "bandage_rus.mdl" ¦ "bandage_usa.mdl" ¦ "canned_meat.mdl" ¦ "corpse.mdl" ¦ "wirecutters.mdl" ¦ "no73_at_grenade.mdl" ¦ "boys_mk1.mdl" | 추정 | 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | 파일 구조와 값에서 추론 | needs full QA |
| `/entity/lodclass` | block | [str]x22 | 10 | 22 | "stuff_small" ¦ "stuff_grenade" ¦ "stuff_weapon" | 추정 | 디테일 단계(LOD) 분류 이름 (추정: 이름과 값 "vehicle", "stuff_small". set/level_of_detail.set 참조로 보임) | 파일 구조와 값에서 추론 |  |
| `/entity/nogroupmesh` | block | [num]x14 | 10 | 14 | 1 | 미확인 | 값은 1뿐. 용도 불명 |  |  |
| `/entity/novisibilitypatch` | block | []x10 | 10 | 10 | 예: | 미확인 | 인자 없는 플래그. 용도 불명 |  |  |
| `/entity/obstacleid` | block | [str]x10 | 10 | 10 | "stuff" | 추정 | 장애물 분류 이름 (추정: 값 "tank", "stuff") | 파일 구조와 값에서 추론 |  |
| `/entity/props` | block | [str]x13 [str str str]x10 [str str]x1 | 10 | 24 | "steel" "stuff" "collectable" ¦ "nocover" ¦ "physics_damping" ¦ "grenade" "steel" ¦ "contact" ¦ "bipod" | 추정 | 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/entity/rimlight` | block | []x10 | 10 | 10 | 예: | 미확인 | 인자 없는 플래그. 이름으로는 윤곽 조명으로 보이나 확인하지 못함 |  |  |
| `/entity/simulation` | block | []x10 | 10 | 10 | 예: | 미확인 | 물리 시뮬레이션 설정 묶음으로 보이나 확인하지 못함 |  |  |
| `/entity/simulation/able` | block | [num]x10 | 10 | 10 | 1 | 미확인(자동) |  |  | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) |
| `/entity/simulation/disablelinked` | block | [num]x10 | 10 | 10 | 1 | 미확인 | 값 1뿐. 용도 불명 |  |  |
| `/entity/simulatorid` | block | [str]x1 | 1 | 1 | "grenade" | 미확인(자동) |  |  |  |
| `/entity/spriteview` | block | []x3 | 3 | 3 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/orientation` | block | [atom]x3 | 3 | 3 | trace | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite` | block | [str]x3 | 3 | 3 | "flag1.ebm" ¦ "flag2.ebm" | 추정 | 표시에 쓰는 이미지 파일 (추정: 값이 .ebm) | 파일 구조와 값에서 추론 |  |
| `/entity/spriteview/sprite/light` | block | []x1 | 1 | 1 | 예: | 미확인 | 값 1 |  |  |
| `/entity/spriteview/sprite/light/linear` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/light/linear/max` | block | [num]x1 | 1 | 1 | 0.2 | 미확인(자동) |  |  | max health ¦ max resources |
| `/entity/spriteview/sprite/light/linear/min` | block | [num]x1 | 1 | 1 | 1 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/preturn` | block | [num]x3 | 3 | 3 | 90 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/scale` | block | [num]x2 []x1 | 3 | 3 | 0.3 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/scale/linear` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/scale/linear/max` | block | [num]x1 | 1 | 1 | 0.35 | 미확인(자동) |  |  | max health ¦ max resources |
| `/entity/spriteview/sprite/scale/linear/min` | block | [num]x1 | 1 | 1 | 0.6 | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/timer` | block | []x1 | 1 | 1 | 예: | 미확인(자동) |  |  |  |
| `/entity/spriteview/sprite/timer/duration` | block | [num]x1 | 1 | 1 | 1.2 | 미확인(자동) |  |  |  |
| `/entity/volume` | block | [str]x60 | 10 | 60 | "body" ¦ "select" ¦ "body1" ¦ "body2" ¦ "fuel" ¦ "shield" ¦ "magazine" | 추정 | 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | 파일 구조와 값에서 추론 | Hull ¦ Superstructure |
| `/entity/volume/able` | block | []x38 [atom]x22 | 10 | 60 | ¦ clear | 미확인(자동) |  |  | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) |
| `/entity/volume/able/blast` | block | []x10 | 10 | 10 | 예: | 추정 | 폭발에 반응하는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break |
| `/entity/volume/able/bullet` | block | []x10 | 10 | 10 | 예: | 추정 | 총탄에 맞는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/entity/volume/able/contact` | block | []x11 | 10 | 11 | 예: | 추정 | 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) | 파일 구조와 값에서 추론 |  |
| `/entity/volume/able/contact_ground` | block | []x11 | 10 | 11 | 예: | 추정 | 지면 접촉 판정 대상 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/entity/volume/able/obstacle` | block | [num]x7 | 1 | 7 | 0 | 미확인(자동) |  |  |  |
| `/entity/volume/able/select` | block | []x20 | 10 | 20 | 예: | 추정 | 마우스 선택 대상 (추정: able의 하위 플래그, 볼륨 이름 "select") | 파일 구조와 값에서 추론 |  |
| `/entity/volume/able/touch` | block | []x22 | 10 | 22 | 예: | 미확인 | able의 하위 플래그. 용도 불명 |  |  |
| `/entity/volume/able/visible` | block | [num]x30 []x10 | 10 | 40 | 0 | 미확인(자동) |  |  | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. |
| `/entity/volume/density` | block | [num]x11 | 10 | 11 | 0 | 미확인 | 값 0~2. 볼륨 밀도로 보이나 단위와 용도를 확인하지 못함 |  |  |
| `/entity/volume/thickness` | block | [num]x31 | 10 | 31 | 1 | 추정 | 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 게임 파일 주석 | 15 + 4 ¦ 89 + 89 |
| `/game_entity` | block | [atom]x512 []x34 | 546 | 546 | game_entity ¦ | 추정 | 엔티티 정의의 최상위 블록. 차량·화포·보병 외형·일부 아이템이 쓴다 (추정: 파일 구조) | 파일 구조와 값에서 추론 | june '41 ¦ jan 45 |
| `/game_entity/armor` | block | []x4 | 2 | 4 | 예: | 추정 | 장갑 판정 설정 묶음 (추정: 하위가 setting/threshold) | 파일 구조와 값에서 추론 |  |
| `/game_entity/armor/setting` | block | [str]x8 | 2 | 8 | "body" ¦ "roof" | 추정 | 장갑 설정 한 벌. 문자열은 설정 이름 (추정: 값 "body") | 파일 구조와 값에서 추론 |  |
| `/game_entity/armor/setting/ray_cast_thickness` | block | []x4 | 2 | 4 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/armor/setting/threshold` | block | []x8 | 2 | 8 | 예: | 추정 | 피해가 들어가기 시작하는 기준값 묶음 (추정: 하위 blast/pierce의 주석) | 파일 구조와 값에서 추론 |  |
| `/game_entity/armor/setting/threshold/blast` | block | []x4 | 2 | 4 | 예: | 확인 | 폭발 피해 기준. 기준값 = koef x 볼륨의 유효 두께 | 게임 파일 주석 | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break |
| `/game_entity/armor/setting/threshold/blast/value` | block | [num]x4 | 2 | 4 | 12 ~ 13 | 미확인(자동) |  |  |  |
| `/game_entity/armor/setting/threshold/pierce` | block | []x4 | 2 | 4 | 예: | 확인 | 관통 피해 기준. 장갑에 도탄·관통 자국을 남기는 데 필요한 최소 관통력 | 게임 파일 주석 | Minimum penetration necessary to leave a ricochet/pen mark on the armour |
| `/game_entity/armor/setting/threshold/pierce/koef` | block | [num]x4 | 2 | 4 | 1 | 확인 | 계수(coefficient). 기준값 = koef x 볼륨의 유효 두께 | 게임 파일 주석 |  |
| `/game_entity/armor/setting/volumes` | block | [str]x16 | 2 | 16 | "body*" ¦ "box*" ¦ "wire*" ¦ "wires*" ¦ "wood*" ¦ "door*" ¦ "roof*" ¦ "floor*" | 추정 | 이 설정이 적용되는 볼륨 이름 패턴. 예: "body*" (추정: 값) | 파일 구조와 값에서 추론 | prevent destroying at spawn moment ¦ Disables rocket volume after firing while reloading |
| `/game_entity/body` | block | []x30 | 25 | 30 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/body/permit` | block | [atom]x14 | 14 | 14 | body | 미확인(자동) |  |  |  |
| `/game_entity/body/proper` | block | [atom atom]x21 | 21 | 21 | body sensible | 미확인(자동) |  |  |  |
| `/game_entity/collider` | block | [str]x920 | 513 | 920 | "stuff" ¦ "ammo" ¦ "grenade" ¦ "particle.small" ¦ "stuff.construction" ¦ "construction" | 추정 | 충돌 분류 이름. set/colliders.set의 항목을 가리키는 것으로 보인다 (추정: 값 "vehicle", "stuff") | 파일 구조와 값에서 추론 |  |
| `/game_entity/constrain` | block | [atom]x539 | 524 | 539 | ground ¦ none | 미확인 | 값은 ground, none 등. 배치 제약으로 보이나 확인하지 못함 |  |  |
| `/game_entity/extender[inventory]` | block | [str]x18 | 12 | 18 | "inventory" | 추정 | 차량 화물칸 기능 (추정: 하위 box) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extender[inventory]/box` | block | []x18 | 12 | 18 | 예: | 추정 | 화물칸 하나 (추정: 하위 size, weight, item) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extender[inventory]/box/item` | block | [str num]x25 | 6 | 25 | 예: "bandage_rus" 2 | 추정 | 실려 있는 아이템과 수량 (추정: 값 "ammo mgun_usa belt" 1000) | 파일 구조와 값에서 추론 | 128+8 ¦ 80+48+8 |
| `/game_entity/extender[inventory]/box/size` | block | [num num]x18 | 12 | 18 | 4 4 ¦ 8 6 ¦ 4 6 ¦ 6 6 | 확인 | 인벤토리 창의 크기. 가로 세로 칸 수 | 게임 파일 주석 | size of the window |
| `/game_entity/extender[inventory]/box/weight` | block | [num]x12 | 12 | 12 | 200 ~ 5000 | 추정 | 적재 한도 (추정: 원문 주석 "will stack up in 100/2 = 2 on humans"으로 보아 쌓이는 수를 정하는 값. 단위 미확인) | 게임 파일 주석 | will stack up in 100/2 = 2 on humans |
| `/game_entity/extender[stuff]` | block | [str]x1009 | 505 | 1009 | "stuff" | 추정 | 아이템 기능. 이 엔티티를 set/stuff의 아이템과 잇는다 (추정: 하위 item) | 파일 구조와 값에서 추론 |  |
| `/game_entity/extender[stuff]/item` | block | [str]x505 | 505 | 505 | 예: "air_bomb_eng_250lb" | 추정 | 연결된 아이템 이름. set/stuff의 파일명과 일치한다 (추정: 참조 색인) | 파일 구조와 값에서 추론 | 128+8 ¦ 80+48+8 |
| `/game_entity/extension` | block | [str]x546 | 546 | 546 | 예: "250lb_gp_mk4.mdl" | 추정 | 이 엔티티의 모델 파일(.mdl) 이름 (추정: 값이 항상 .mdl) | 파일 구조와 값에서 추론 | needs full QA |
| `/game_entity/fogable` | block | [atom]x30 | 30 | 30 | never ¦ always | 미확인(자동) |  |  |  |
| `/game_entity/lodclass` | block | [str]x1039 | 526 | 1039 | "stuff_small" ¦ "stuff_weapon" ¦ "tracer" ¦ "decor_landmark" ¦ "part_small" ¦ "casing" ¦ "stuff_grenade" ¦ "decor_big" ¦ "decor_medium" ¦ "decor_small" | 추정 | 디테일 단계(LOD) 분류 이름 (추정: 이름과 값 "vehicle", "stuff_small". set/level_of_detail.set 참조로 보임) | 파일 구조와 값에서 추론 |  |
| `/game_entity/mass` | block | [num]x23 | 21 | 23 | 1 ~ 500 | 추정 | 질량. 차량·무기의 원문 주석이 kg 단위로 적고 있다 (추정: 주석이 값의 내역을 kg으로 설명) | 게임 파일 주석 | 18.1kg gun and water + 23kg tripod ¦ empty weight , gross 6532 |
| `/game_entity/nogetshadows` | block | []x20 | 20 | 20 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/nogroupmesh` | block | [num]x566 | 487 | 566 | 1 | 미확인 | 값은 1뿐. 용도 불명 |  |  |
| `/game_entity/novisibilitypatch` | block | []x487 | 487 | 487 | 예: | 미확인 | 인자 없는 플래그. 용도 불명 |  |  |
| `/game_entity/obstacleid` | block | [str]x504 | 502 | 504 | "stuff" ¦ "nothing" ¦ "construction" | 추정 | 장애물 분류 이름 (추정: 값 "tank", "stuff") | 파일 구조와 값에서 추론 |  |
| `/game_entity/props` | block | [str str str]x518 [str]x498 [str str]x290 [str str str str]x6 [str str str str str str]x5 | 526 | 1319 | 예: "air_bomb" "explosive_stuff" "steel" | 추정 | 엔티티 속성 태그 목록. 재질("steel"), 종류("vehicle"), 동작 조건 등을 나열한다 (추정: 값) | 파일 구조와 값에서 추론 |  |
| `/game_entity/rimlight` | block | []x455 | 455 | 455 | 예: | 미확인 | 인자 없는 플래그. 이름으로는 윤곽 조명으로 보이나 확인하지 못함 |  |  |
| `/game_entity/scale` | block | [num]x4 | 4 | 4 | 0.85 ~ 1.5 | 미확인(자동) |  |  |  |
| `/game_entity/simulation` | block | []x511 | 511 | 511 | 예: | 미확인 | 물리 시뮬레이션 설정 묶음으로 보이나 확인하지 못함 |  |  |
| `/game_entity/simulation/able` | block | [num]x511 | 511 | 511 | 1 | 미확인 | 값 0 또는 1 |  | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) |
| `/game_entity/simulation/disablelinked` | block | [num]x493 | 493 | 493 | 1 | 미확인 | 값 1뿐. 용도 불명 |  |  |
| `/game_entity/simulation/state` | block | [num]x17 | 17 | 17 | 1 | 미확인(자동) |  |  | 2hand & 1hand items ¦ to enable item ops in snipe cover |
| `/game_entity/simulatorid` | block | [str]x14 | 14 | 14 | "grenade" | 미확인(자동) |  |  |  |
| `/game_entity/skeleton` | block | []x2 | 2 | 2 | 예: | 추정 | 엔티티에 추가하는 뼈대 정의 (추정: 하위 bone/position) | 파일 구조와 값에서 추론 |  |
| `/game_entity/skeleton/texmods` | block | []x2 | 2 | 2 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/skeleton/texmods/"*"` | block | [str str]x96 | 2 | 96 | 예: "_x" "_x" | 미확인(자동) |  |  |  |
| `/game_entity/spriteview` | block | []x60 | 20 | 60 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/ground` | block | []x40 | 20 | 40 | 예: | 미확인 | 인자 없는 플래그 |  |  |
| `/game_entity/spriteview/layer` | block | [atom]x40 | 20 | 40 | grounddynamic | 미확인 | 값 alpha |  |  |
| `/game_entity/spriteview/offset` | block | [num num num]x60 | 20 | 60 | 0 0 5 ¦ 0 0 25 ¦ 0 0 3 | 추정 | 표시 위치 오프셋 세 값 (추정: 이름) | 파일 구조와 값에서 추론 |  |
| `/game_entity/spriteview/sprite` | block | [str]x60 | 20 | 60 | "pointer_halo" ¦ "mine_f.ebm" ¦ "pointer_clr.ebm" | 추정 | 표시에 쓰는 이미지 파일 (추정: 값이 .ebm) | 파일 구조와 값에서 추론 |  |
| `/game_entity/spriteview/sprite/highlight` | block | [atom]x60 | 20 | 60 | 0xFF5000 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/light` | block | []x60 | 20 | 60 | 예: | 미확인 | 값 1 |  |  |
| `/game_entity/spriteview/sprite/light/peak` | block | []x60 | 20 | 60 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/light/peak/max` | block | [num]x60 | 20 | 60 | 0.4 ~ 0.5 | 미확인(자동) |  |  | max health ¦ max resources |
| `/game_entity/spriteview/sprite/light/peak/min` | block | [num]x60 | 20 | 60 | 0 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/scale` | block | [num]x60 | 20 | 60 | 0.1 ~ 0.8 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/scale/linear` | block | []x40 | 20 | 40 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/scale/linear/max` | block | [num]x40 | 20 | 40 | 0.03 ~ 0.1 | 미확인(자동) |  |  | max health ¦ max resources |
| `/game_entity/spriteview/sprite/scale/linear/min` | block | [num]x40 | 20 | 40 | 0.15 ~ 0.3 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/timer` | block | []x60 | 20 | 60 | 예: | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/timer/duration` | block | [num]x60 | 20 | 60 | 1 ~ 2 | 미확인(자동) |  |  |  |
| `/game_entity/spriteview/sprite/timer/mode` | block | [atom atom]x40 [atom]x20 | 20 | 60 | play loop ¦ play | 미확인 | 값 detonate |  |  |
| `/game_entity/spriteview/sprite/visibility` | block | [num]x60 | 20 | 60 | 0.3 ~ 0.5 | 미확인(자동) |  |  |  |
| `/game_entity/texmod` | block | [str]x1 | 1 | 1 | "ger" | 미확인(자동) |  |  |  |
| `/game_entity/volume` | block | [str]x3060 | 525 | 3060 | "body" ¦ "select" ¦ "body1" ¦ "body2" ¦ "fuel" ¦ "shield" ¦ "magazine" | 추정 | 모델의 충돌 볼륨 하나에 대한 설정. 첫 문자열이 볼륨 이름 (추정: properties/vehicle.ext의 매크로가 {volume %0 ...} 형태) | 파일 구조와 값에서 추론 | Hull ¦ Superstructure |
| `/game_entity/volume/able` | block | []x1937 [atom]x1041 | 513 | 2978 | ¦ clear | 추정 | 볼륨이 반응하는 상호작용 묶음. clear는 상속된 설정을 지우는 것으로 보인다 (추정: 하위가 bullet, blast, touch 등 인자 없는 플래그) | 파일 구조와 값에서 추론 | disables vehicle from being crewed until body is repaired ¦ stops uncontrolable actions (keep incase friendly is set on fire and is not damaged/killed) |
| `/game_entity/volume/able/blast` | block | []x775 | 487 | 775 | 예: | 추정 | 폭발에 반응하는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 | koef = coefficient. Threshold = koef value x volume effective thickness ¦ 75mm+ HE will break |
| `/game_entity/volume/able/bullet` | block | []x771 [num]x6 | 493 | 777 | 0 | 추정 | 총탄에 맞는 볼륨임을 표시 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 | required for AT-Grenades with cumaltive damage |
| `/game_entity/volume/able/contact` | block | []x505 [num]x11 | 498 | 516 | 0 | 추정 | 접촉 판정 대상 (추정: able의 하위 플래그. 0이면 끔) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/contact_ground` | block | []x536 | 487 | 536 | 예: | 추정 | 지면 접촉 판정 대상 (추정: able의 하위 플래그) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/nowounddecal` | block | []x6 | 6 | 6 | 예: | 추정 | 피격 자국을 그리지 않음 (추정: 이름. 바퀴 볼륨에 쓰임) | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/obstacle` | block | [num]x283 | 61 | 283 | 0 | 미확인 | able의 하위. 값 0 |  |  |
| `/game_entity/volume/able/select` | block | []x993 | 494 | 993 | 예: | 추정 | 마우스 선택 대상 (추정: able의 하위 플래그, 볼륨 이름 "select") | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/able/touch` | block | []x1324 | 495 | 1324 | 예: | 미확인 | able의 하위 플래그. 용도 불명 |  |  |
| `/game_entity/volume/able/visible` | block | [num]x1371 []x487 | 493 | 1858 | 0 | 미확인 | able의 하위. 값 0 |  | once spotted, the distance the unit will remain spotted until Line Of Sight is broken. ¦ once human is spotted, the distance the human will remain spotted until Line Of Sight is broken. When placed outside of a specific stance block, it becomes a master value for all stances. |
| `/game_entity/volume/density` | block | [num]x510 | 494 | 510 | 0 ~ 2 | 미확인 | 값 0~2. 볼륨 밀도로 보이나 단위와 용도를 확인하지 못함 |  |  |
| `/game_entity/volume/material` | block | [str]x6 | 6 | 6 | "fabric" | 추정 | 볼륨의 재질 이름. set/material.set 참조로 보인다 (추정: 값 "wood") | 파일 구조와 값에서 추론 |  |
| `/game_entity/volume/tags` | block | [str]x9 | 9 | 9 | "body" ¦ "pour_over" | 추정 | 볼륨 태그. 재질이나 부위를 나타낸다 (추정: 값 "wood", "wheel", "body") | 파일 구조와 값에서 추론 | tag for conquest so that vehicle cannot be repaired/recovered ¦ strikes types see at {strokes} in set\ability\melee.set |
| `/game_entity/volume/thickness` | block | [num]x1819 | 485 | 1819 | 0.01 ~ 50 | 추정 | 볼륨의 장갑 두께. 단위는 mm로 보인다 (추정: 원문 주석이 "15 + 4", "4mm + .25"" 같이 판 두께를 적음) | 게임 파일 주석 | 15 + 4 ¦ 89 + 89 |
| `/game_entity/zclipvisibility` | block | [num]x2 | 2 | 2 | 0 | 미확인(자동) |  |  |  |

