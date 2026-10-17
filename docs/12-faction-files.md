# 팩션별 유닛 정보가 있는 파일 (2026-10-17)

팩션별 강화(설계 `docs/superpowers/specs/2026-10-17-faction-boost-design.md`)를 위해 바닐라와 지금 켜 둔 모드의 파일을 다시 열어 정리했다. 손으로 고치는 문서다.

- 원본 계층(로드 순서, 뒤가 이긴다): 바닐라 → MACE Wunderwaffe Ultimate Fusion(3732639687) → MACE(2905667604) → Auto Medic-AA-Artillery & Weather Switch(2829744930) → Spawn Menu of MACE(3617250510).
- 근거는 파일 경로와 줄이다. 게임 안 확인은 하지 않았다. 파일로 정할 수 없는 것은 "미확인"이다.

## 팩션(군대)

- 플레이할 수 있는 군대는 `set/multiplayer/armies/*.set`의 ger, rus, usa, eng, fin(바닐라)과 jap(MACE `armies/jap.set`)이다.
- 유닛의 군대는 **로스터 항목의 `side()`**로 정한다. 로스터는 `set/multiplayer/units/**/inf_<군대>*.set`(보병)과 `units_<군대>*.set`(분대·차량)다. 예: `{"mp/ger/mid/medic" ("inf_medic" side(ger) cost(40.0))}`(`conquest/inf_ger.set` 66행).
- 한 군대 파일에 여러 side가 섞인다. 바닐라 `conquest/inf_eng.set`은 can 26, eng 393, pol 37이다. MACE의 영국 파일에는 fra·aus·nzl도 있다. 그래서 side를 가장 많이 나오는 군대에 묶는다(eng 묶음: eng, can, pol, fra, aus, nzl).
- 보병 폴더(`set/breed/mp/<side>/`)와 `{nationality}`는 진영 코드가 아니다. 바닐라 mp에서 폴더와 nationality가 다른 파일이 137개다(eng:sco 85, fin:swe 27, ger:spa 14, ned:fra 9, ger:rus 2). 엔진이 nationality를 어디에 쓰는지는 미확인이다.
- 차량 폴더(`entity/-vehicle/<국가>/`, 모드는 `+<국가>`)도 근거가 아니다. 노획 차량은 다른 이름의 사본이다(`finland/tank_medium/panzer4j_fin`). 로스터에 없는 차량만 폴더로 정한다(germany→ger, ussr→rus, usa→usa, eng→eng, finland→fin, japan→jap).
- 두 군대 로스터에 같이 등록된 유닛(지금 판 5대): `germany/btr/sdkfz10_4`, `germany/cannon/100mm_nbw35`, `germany/cannon/210mm_nebelwerfer42`, `germany/tank_light/pz38h_w40`(ger·fin), `+finland/cannon/37mm_m1915_trench_gun`(rus·fin). 하나를 고치면 다른 군대도 바뀐다.

편집기 `units --army <군대>`로 센 지금 판의 유닛 수:

| 군대 | 탱크 | 보병 파일 |
|---|---|---|
| ger | 178 | 1,192 |
| rus | 161 | 875 |
| usa | 120 | 1,439 |
| eng | 175 | 1,148 |
| fin | 34 | 628 |
| jap | 88 | 768 |

## 항목별 파일과 키

| 항목 | 값이 있는 곳 | 팩션 사이 공유 |
|---|---|---|
| 보병 인벤토리 | 보병 파일 안 `{inventory {item "이름" N R} …}`. N 개수, R ± 편차(게임 확인 2026-10-15). `{item "k98" filled}`는 무기, `filling "탄" N`은 장전된 탄 | 파일마다 따로 |
| 차량 인벤토리 | 유닛 `.def`/`.inc`의 `{extender "inventory" {box {item "이름" N} …}}`. 공통 `properties/car.ext`의 `{item "repair_kit" 5}` 등 | 유닛 파일은 따로, `car.ext`류는 공유 |
| 탄약 구별 | 아이템 이름 → `set/stuff/**` 파일(점으로 나눈 낱말 집합, 계열 이름은 이름순 맨 앞, `docs/02` 82~96행). 확장자 `.ammo`가 탄약. 포탄은 이름에 "ammo"가 없다(`bulletger_88_l56 he` → `shell/bulletger_88_l56.he.ammo`) | |
| 재장전 | `rechargeTime`(초, 클수록 느림). `("reload time" seconds(N))` 매크로가 넣는다(`gun/.presets` 53행 등). 포는 무기 파일 자신의 줄(`gun/88mm_kwk36` 8초), 소총은 원형(`rifle/rifle.pattern` 6초, k98을 127개가 씀), 기관총은 `mgun/.presets`의 `reload_belt`(8.5초, 206개) | 무기는 이름으로 불린다. k98을 소련 파르티잔도, mg34_tank를 핀란드 전차도 쓴다 |
| 보병 체력 | 기본 `properties/human.ext` 5~10행 `Health max 100`(모든 보병). 배율은 perks의 `("tier3_standard")` 등(`set/breed/presets/perks/health.inc`, `{"health" "300%"}`, 6,111곳 공유) | 모든 팩션 |
| 장갑 | 유닛 `.ext`의 `{volume "…" {thickness N {front} {top} {rear} {bottom} {left}}}`(mm), 호출 인자 `("tracks" thickness(30))`·`("gun" thickness(30))`, 승용차 `("car_thickness" body() engine())`. 매크로 본문 `vehicle.ext`의 `sandbags`·`internals`(공유 1,494)는 '이 유닛만'이 안 된다 | 유닛 파일은 따로 |
| 차종 | 폴더 `car`·`btr`(차량), `tank_light`(MACE `fra/light`·`+swe/light`·`+finland/light`), `tank_medium`, `tank_heavy`(MACE `fra/heavy`). 초중전차 표시는 없다(`editor/presets/super-heavy.txt`로 손으로 고른다). `/x/` 폴더는 파괴된 차체 파일이라 기동 값이 없다 | |
| 속도 | `("mobility_tank"|"mobility_car"|"mobility_btr" speed(N) …)`의 `speed` → `/game_entity/chassis/locomotion/maxspeed`(`properties/mobility.inc` 32행) | 유닛 `.def` 호출 인자 |
| 연료 | 같은 호출의 `fuel(N)`(L)·`range(N)`(km). `rate = 10 × fuel / range`(mobility.inc). `tank_unarmed.ext`의 `fuelbag volume 250`(공유 782)이 같은 유닛에 함께 펼쳐지는데 어느 쪽이 쓰이는지는 미확인 | 유닛 `.def` 호출 인자 |
| 컨퀘스트 자원 | `set/dynamic_campaign/resources_{low,standard,high,very_high}.set`. 플레이어 몫 `Resources/{MP,SP,AP,RP}/StartVal`·`WinGain`·`LoseGain`, 단계 한도 `StageCP`(화면의 단계 I~V `0/200`), 예비 병력 한도 `GlobalMaxCP`(`0/2000`). AI 몫은 `Bots`. 지금 이기는 판은 MACE Wunderwaffe(MP 5000 등). Wunderwaffe의 `high`·`very_high`는 맨 위 이름이 서로 바뀌어 있다(영향 미확인) | 진영 구분 없음. 플레이어·AI는 나뉜다 |
| 컨퀘스트 분대 인원 | `units_<군대>.set`의 `c1(nco:1) c3(rifleman_1:5)` 꼴(분대 구성). 사용자가 말한 "분대 최대값"은 단계 한도 `StageCP`다(2026-10-17 화면으로 확인) | 군대별 |
| 스커미시 MP·DP·CP | 배틀존·탄약전 `set/multiplayer/games/presets/preset_{early,mid,late}.inc`의 `("standard_all"|"custom_all" start() finish() doctrine() cp())`(MACE가 late를 덮어쓴다). 모든 프리셋의 MP 시작은 200, 끝은 2,150~7,500. 봇은 `global_parameters/bots_generic.inc`의 `balancingFactorBonus {cp mpStart mpFinish payback}` 배율 하나를 모든 프리셋이 같이 쓴다. 피난 `laststand_*.inc`는 팀 a(플레이어 수비)·b(AI 공격)가 따로, 프런트라인 `frontlines_*.inc`는 팀 a·b가 따로인데 어느 팀이 플레이어인지는 파일로 알 수 없다 | 배틀존은 모든 플레이어 공통 |

## 캠페인

- 캠페인 보병은 `set/breed/single/ger`, `single/rus`에도 있다. 같은 폴더 이름의 side로 군대에 넣는다.
- 미션 맵에 미리 놓인 유닛은 인벤토리를 맵 파일에 따로 가질 수 있다. 맵 파일은 고치지 않는다(사용자 결정 2026-10-17). 유닛 파일에서 오는 값은 강화된다.
