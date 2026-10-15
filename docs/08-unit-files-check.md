# 유닛 모딩 문서 검증 (2026-10-14)

사용자가 준 문서 "Gates of Hell 유닛 모딩 파일·속성 정리"(2026-10-14, 웹 조사 정리)의 주장을 이 PC의 바닐라 파일(`_work/vanilla`, 게임 `resource`에서 꺼낸 것)과 대조했다. 판정은 다음 넷이다.

- **맞음**: 게임 파일에 그대로 있다.
- **고침**: 있기는 하지만 경로·키·값이 다르다. 맞는 내용을 적었다.
- **틀림**: 지금 게임에는 그런 파일이나 키가 없다. 대부분 같은 엔진의 Call to Arms 자료를 옮긴 것이다.
- **미확인**: 파일로는 판단할 수 없다.

근거의 "주석"은 게임 파일 안의 개발사 주석이다. 이 문서는 손으로 고친다.

문서의 출처 사이트는 [09-modding-sources.md](09-modding-sources.md)에 정리했다. "틀림"으로 판정한 장갑·초속·파일 이름(`armor_heavy`, `speed2`, `damage_cumulative`, `perks_generic.inc`, `crew.ext`, `desc.lng`)은 Call to Arms 위키의 Modding/Files·Modding/Examples에서 온 것으로 확인했다(2026-10-14).

## 요약

- 문서의 큰 틀(엔티티 `.def` + `properties/*.ext` include, breed `.set`, `set/stuff`, `set/multiplayer/units`)은 맞다.
- **틀린 것이 많은 곳은 탱크 장갑과 포 성능이다.** `("armor_heavy" turret(...))`, `"damage_cumulative"`, `"speed2"`는 지금 바닐라에 한 곳도 없다. Gates of Hell은 볼륨마다 `{thickness 기본 {front} {top} {rear} {bottom}}`을 쓰고, 포는 `("velocity m/s" ...)`, `("armor health" damage(...))`, `("accuracy_direct"/"accuracy_indirect" ...)` 매크로를 쓴다.
- 문서가 "공개 자료에서 이름을 확인하지 못했다"고 한 **최고 속도·엔진·연료 키는 파일에 주석과 함께 있다**(`("mobility_tank" speed() power() fuel() ...)`).
- 시야 파일의 위치는 `set/stuff/vision`이 아니라 `set/vision`이다. 보병 정면 시야각은 지금 150°다(문서 값 100°는 2023년 값).
- 의무병 회복 플래그의 실제 키는 `"personage"`이고, 주석은 문서와 반대로 **0이 회복 가능**이라고 적는다.

## 파일과 경로

| 문서의 주장 | 판정 | 실제 |
|---|---|---|
| 보병 외형 `entity/humanskin/...` | 맞음 | |
| breed `set/breed/mp/<국가>/<시기>/<병과>.set`, 시기 early·mid·late | 맞음 | 예 `set/breed/mp/ger/mid/medic.set` |
| 능력치 `set/breed/perks_generic.inc` | 틀림 | 없다. `set/breed/presets/perks/`의 `health.inc`, `skills.inc`, `specialties.inc`, `veterancy.inc`, `vision.inc`이고, 각 국가·시기 폴더의 `ability.inc`가 이것들을 include한다 |
| `set/ability/ability_generic.inc` | 틀림 | 없다. 레벨 정의는 `set/ability/level_generic.inc`(`{level "5%" 0.05}` 꼴)와 `skill_rifle.set` 등 능력별 파일이다 |
| `properties/human.ext`, `vehicle.ext`, `tank.ext`, `car.ext`, `stuff.ext` | 맞음 | |
| `properties/crew.ext` | 틀림 | 없다. `cannon_crew.ext`, `car_crew.ext`가 있다 |
| `set/interaction_entity/human.inc`, `set/interaction_entity/tank/` | 맞음 | |
| `set/stuff/mortar/.presets` | 맞음 | 사거리·산포 프리셋(`"light_mortar_range"` 등)이 있다 |
| `set/stuff/vision` | 고침 | `set/vision/` (`vision.set`, `vision_fields.inc`, `spotting.inc`, `hearing.inc`, `settings.set`) |
| `set/ballistics...` | 맞음 | `set/ballistics.set`. 주석: 무기 종류별로 거리에 따라 사람 체력 피해를 조정하는 곡선 |
| `entity/inventory/-ammo/...` | 맞음 | |
| `localization/desc.lng` | 틀림 | 게임의 `localizations/*.pak` 안 `.pot` 파일이다(`desc/entity/<이름>` 등). 이 저장소의 `tools/Build-DisplayNames.ps1`이 읽는다 |
| 로스터 `units/conquest/units_<국가>.set`, `inf_<국가>.set`, `roster_<시기>.set`, `settings.set`, `settings_inf.set` | 맞음 | 시기 폴더(`early`·`mid`·`late`)와 `conquest`에 각각 있다 |
| `dynamic_campaign/unit_research_<국가>.set`, `resources_<난이도>.set` | 맞음 | 난이도는 `low`·`standard`·`high` |

## 보병

| 문서의 주장 | 판정 | 근거 |
|---|---|---|
| `{skin}` `{portrait}` `{icon}` `{icon_priority}` `{nationality}` | 맞음 | `medic.set`. 그 밖에 `{behaviour}` `{tags}` `{voice}` `{armors {head ...}}`가 있다 |
| `{item "luger" filled}`, `{item "이름" 수}` | 맞음 | `{item "luger" filled}`, `{item "pistol ammo" 24 8}` |
| 수가 둘이면 기본값 + 무작위 계수 | 고침 | 병사마다 첫 수 ± 둘째 수 사이에서 개수가 정해진다(게임 안 확인 2026-10-15: `medic.set`의 `{item "bandage_ger" 22.5 2.5}`를 `13 13`으로 바꾸자 의무병 6명이 21·2·24·11·25·4개). 바닐라 `set/breed`에서 수가 둘인 줄 17,954개 가운데 16,664개는 첫 수가 소수다 |
| perk 레벨을 breed가 고른다 | 고침 | `{perks (include "ability.inc") ("rifle_skill_rank_2")("tier2_standard") ("medic")}`처럼 매크로 이름으로 고른다 |
| 체력·스태미나·속도 값 | 고침 | `presets/perks/health.inc`의 `(define "tier1_standard" {"health" "150%"} {"health_regeneration" "10%"} {"stamina" "75%"} {"stamina_regeneration" "75%"} {"speed" 1.00} ...)` |
| 시작 숙련 | 맞음 | `{veterancy (include "ability.inc") ("veterancy_lvl_0")}` |
| 중상 플래그: 1이면 회복 가능, 0이면 즉사 | 고침 | 키는 `{"personage" 0}`(`health.inc`). 주석: "This means soldiers are revivable at veterancy 0". 값은 소생 가능 상태(Personage 퍽)가 켜지는 숙련도다(게임 안 확인 2026-10-15: `tier2_standard`를 3으로 바꾸자 Library에 "Personage (Vet. 3)"). `interaction_entity/human.inc`에 `"dead_personage"`, `"personage_check"` 처리가 있다 |
| `human.ext`의 `{Sensor}`, 시야 | 맞음 | `{vision "human"}`, `{vision "human_peripheral"}`, `{vision "noise_detection"}`, `{InformRange 80 100}`(주석: 첫 값 AI, 둘째 값 사용자) |

## 탱크와 차량

| 문서의 주장 | 판정 | 근거 |
|---|---|---|
| `("armor_heavy" turret(a b c d) body(...))`, `("armor_mantlet" t())`, `("armor_engine" t())` | 틀림 | 바닐라 `entity/-vehicle` 전체에 `"armor_heavy"`가 없다. 장갑은 유닛 `.ext`의 `{volume "body1" {thickness 30 {front 80} {top 20} {rear 10} {bottom 10}}}` 꼴이다(첫 수가 기본 두께, 하위 블록이 방향별 두께). 방순은 `{volume "mantlet1" {thickness 20 ...}}`. `"armor_heavy_cannon"` 같은 이름은 포 쪽 매크로다 |
| 네 값은 전면·측면·후면·상면 | 틀림 | 위의 형식이라 해당 없다. 방향은 이름(`front`, `top`, `rear`, `bottom`)으로 적는다 |
| 장갑 위치는 `.vol` 기준, 텍스트로는 두께만 | 맞음 | `volume` 이름이 모델의 볼륨을 가리킨다 |
| 최고 속도·엔진·연료 키를 확인하지 못했다 | 고침 | `{Chassis ("mobility_tank" speed(38) reverse(8) traverse(30) weight(25.0) power(265) track(4.5) fuel(680) type(fuel) range(210))}`. 주석: 도로 최고 속도 km/h, 후진 km/h, 궤도 선회 초당 도, 무게 t, 엔진 hp, 궤도 성능 (0,10], 연료 L, 연료 종류, 야지 항속 km. `properties/mobility.inc`가 이 인자로 `Locomotion`·`FuelBag` 값을 계산한다. 차량은 `"mobility_car"`/`"mobility_btr"`(`speedatmaxturn` 추가, `traverse`·`track` 없음), 포는 `"mobility_cannon" speed() weight()`이고 **포의 무게 단위는 kg**이다(주석) |
| 포탑 회전 속도 | 고침 | `{bone "turret" ("turret_medium" power_traverse(0))}`. `power_traverse(0)`의 주석은 "manual"이다. 포 상하 각은 `{bone "gun_rot" {limits -10 20} {speed2 5}}` |
| 승무원 | 맞음 | `("crew_5_human_extended")`, `("seat_human" place(seat1) ...)` |
| `tank.ext`의 수리 속도 | 맞음 | `("turret" repair(45))`, `("gun" repair(30) thickness(20))` |
| `{extender "inventory"}`, `{weapon "mgun" ...}` | 맞음 | |
| `{item "barrel" 20 filling "fuel" 420 {cell 6 7}}`의 `{cell x y}` = 격자 자리 | 추정 | 파일 주석은 없다. 바닐라 유닛 파일에는 없고 맵·미션 저장(`map/single.pak`의 `.mi`, `map`)에 있다. 값은 x 0~8, y 0~11로 사람 `{Size 8 12}`(`human.ext` 97행), 차량 `{Size 9 15}`(`vehicle.ext` 31행) 인벤토리 격자와 맞는다. 인벤토리 칸 크기는 `interface.set`의 `{cell_size 36 36}`. 2026-10-15 W-F 조사 |
| 시야 | 고침 | `.def`의 `{props ... "vision_lev04" ...}`가 `set/vision/settings.set`의 `{vehicle_04 actor "vision_lev04"}`(주석: medium tanks, rocket artillery)로 이어진다 |

## 포와 무기·탄약

| 문서의 주장 | 판정 | 근거 |
|---|---|---|
| `{parameters "<탄종>" ...}` | 맞음 | `set/stuff/gun`에 `"he"` 51곳, `"apcr"`·`"apcbc"` 33곳 등 |
| `("speed2" s())`, `(speed s())`로 초속 | 틀림 | `set/stuff`에 한 곳도 없다. 초속은 `("velocity m/s" ap(1) apcr(1) he(213) heat(1))` 매크로다 |
| `("damage_cumulative" a())`로 HEAT 관통력 | 틀림 | `set/stuff`에 없다 |
| `{gravity 0}` | 고침 | 키는 있다(35개 파일). `gun/.presets`의 `standard_firing_arc` 3.5, `curved_firing_arc` 5.5, `artillery_firing_arc` 6.0으로 보아 클수록 탄도가 많이 휜다. `{gravity 0}`은 쏘지 않는 `special/binocular`, `commander_vision` 두 곳뿐이다. 0이면 떨어지지 않는다는 것은 추정. 게임 안 확인은 G11 |
| armor_damage 값 | 틀림 | 그 이름이 없다. 포 파일에 `("armor health" damage(310))`가 있다 |
| 직사·곡사 정의 분리 | 맞음 | `("accuracy_direct" ...)` 61개 파일, `("accuracy_indirect" ...)` 50개 파일. 인자 주석: `r1` 직사 탄 정확도 반경, `r2` 곡사 정확도 반경, `rmax` 최대 사거리의 곡사 반경, `range_ap`·`range_heat`·`range_he` 정확도가 떨어지기 시작하는 거리 |
| 포탄 `set/stuff/shell/bullet<구경>.<탄종>.ammo`, `{entity}`, 패턴 | 맞음 | 예 `bullet_105x380.fin.he.ammo`: `{from "pattern he"}`, `{entity "105mm_fin_he"}`, `("he_level_3" weight(14.9) filler(1.4))`. 패턴은 `set/stuff/shell/*.pattern` |
| 박격포 산포 | 맞음 | `{spreading {radiusTable {0 5} {120 18} ...}}`, `{aimRange}`, `{minRange}`, `{maxRange}` |
| 같은 무기를 쓰는 모든 유닛이 바뀐다 | 맞음 | 편집기의 '같이 씀' 수가 이것이다 |

## 가격·로스터

| 문서의 주장 | 판정 | 근거 |
|---|---|---|
| `{"panzer5g" ("vehicle2" side() period() min_stage() max_stage() cw() cp() crew1(tankman:4) ...) {cost}}` | 맞음 | 형식은 같다. 값은 문서 예시와 다르다: 지금 `cw(6) cp(40)`, `{cost 1220}`, `min_stage(13)` |
| `cp` 지휘 포인트, `cw` 무게, `{cost}` 인력 | 고침 | `settings.set`의 `"single_unit"`이 `{cost %cost} {score (* %cost 0.05)} {cw %cw} {cp %cp}`로 넣는다. 같은 파일 머리 주석: `cw`는 "capture weight for flags"(깃발 점령 가중치, 무게가 아니다), `cp`는 "the value that units have that fill up how many troops can be fielded"(병력 한도를 채우는 CP), `{cost}`는 "MP cost of the unit" |
| `cd` 쿨다운, `ci` 호출 관련 | 고침 | `{charge {delay %cd} {interval %ci}}`. 주석: delay는 처음 지연만, interval은 간격만 조정한다 |
| 보병 `("inf_tier2" ... lvl() r() rm() cost())` | 맞음 | `conquest/inf_ger.set`. `r`·`rm`은 `conquest/settings_inf.set`의 `{research_stage %r}` `{research_stage_max %rm}`이고, `conquest/settings.set` 주석으로 봇이 이 유닛을 쓸 수 있는 연구 단계의 최소·최대다. `inf_tier2`는 `units/settings_inf.set`에서 `{cp 3} {cw 1.0}`, 컨퀘스트의 `units/conquest/settings_inf.set`에서 `{cp 2} {cw 1.0}`이다 |
| 분대 가격 = breed별 cost 합, K98 8, MP40 10 | 맞음 | `inf_ger.set` 줄 주석(`cost(10.0)} ; mp40`, `cost(8.0)} ; k98`)과 `units/prices.txt`(tier2 보병 기준 표) |
| `resources_<난이도>.set`의 `{StageCP}`, `{RP {StartVal}}` | 맞음 | `{StageCP 60 80 100 120 140}`, `{GlobalMaxCP 550}`, `{Resources {MP {StartVal 900}} ... {RP ...}}` |
| `unit_research_<국가>.set`의 연구 비용, `requires` | 맞음 | `{tech "defense_level_2" requires "..." costs 5 position 4 0}` |
| AI 구매 | 맞음 | `units/unit_roster_readme.md`에 개발사의 AI 구매 메타데이터 설명이 있다(availability, base, class, grade, roles 등) |

## 시야·탐지

| 문서의 주장 | 판정 | 근거 |
|---|---|---|
| 필드 단위, 한 유닛이 여러 필드 | 맞음 | `vision_fields.inc` 주석 |
| 최대 거리 = 기준 거리 × `("spotting" r())` | 맞음 | 주석: "Sets the vision range, defined in generic.inc". 전체 배수는 `vision.set`의 `(define "radius" ... {radius (* 1.0 %r)})`(주석: 게임 전체 시야 거리를 바꾼다) |
| `{uncover}` m, `{h_fov}`·`{v_fov}` 도 | 맞음 | 주석 |
| `human` 20 / 100° / 180° / 1.0 | 고침 | 지금 `h_fov 150` |
| `human_peripheral` 10 / 240 / 120 / 0.2 | 고침 | 지금 `spotting r(0.22)` |
| `vehicle_main` 25/50/100/1, `vehicle_top_mgun` 25/90/170/1, `vehicle_top_turret` 25/30/30/0.85, `vehicle_around` 15/360/170/0.6 | 맞음 | |
| Informrange로 정보 공유 범위 | 맞음 | `human.ext`, `vehicle.ext`의 `{InformRange 80 100}` |
| 유닛이 필드를 거는 문법은 공개 자료에 없다 | 고침 | 보병은 `human.ext`의 `{vision "human"}`, 차량은 `.def`의 `props`의 `"vision_levNN"`이 `set/vision/settings.set`의 actor로 이어진다 |
| 야간 시야 25% 감소, 전차·대전차포 특례, 야간 장비 extender | 고침 | 야간: 환경 파일(`set/environment/**/varnight*.set` 등)의 `{light}` 안에 `{night}`와 `{night_vision 0.75}`가 있다(2026-10-15 바닐라 0.75 150개, 주간 값 1 392개). 시야 배수인지는 주석이 없어 추정이고 게임 안 확인은 G13. 특례: `vision_fields.inc`의 `cannon_range_200`~`250`, `tank_range_200`~`250`이 `("spotting2" r(1.5) veh_vis(X))`이고 `spotting.inc`가 `{visible (+ 100 (/ %veh_vis %r))}`(주석 "once spotted, the distance the unit will remain spotted")와 "vision = radius * rules"이므로 한 번 발견한 차량은 1.5×(100+X/1.5) = 포 사거리(200~250m)까지 보인다(확인). 야간 장비 extender(`vision_mode`, `timer_object`)는 v1.067 바닐라에 없다 |
