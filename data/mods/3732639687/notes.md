## 메모

- **MACE 위에 얹는 모드다.** `mod.info`에 `require`는 없지만, 창작마당 설명이 MACE 다음에 로드하라고 하고, 파일로도 확인된다: MACE와 같은 가상 경로의 텍스트 파일이 5,521개이고 그중 5,458개는 내용이 다르다. 대부분 `entity/-vehicle`(4,637개)과 `entity/humanskin`(763개)의 파일이다. 이 문서의 펼치기는 Fusion의 파일, MACE의 파일, 바닐라 순으로 include를 찾았다.
- **"바닐라의 것을 변경" 수치의 해석.** 위 표의 "내용이 바뀐 바닐라 텍스트 파일 6,564개"는 바닐라와 비교한 것이다. 유닛 파일로 좁히면 아이템 43개와 아이템 엔티티 1개뿐이고 나머지는 모델·재질 같은 보조 파일이다.
- **MACE에 없는 새 유닛.** 새 차량류 `.def` 537개 중 520개, 새 아이템 440개 중 385개가 MACE에 없는 경로다. 구동계(`chassis`)가 있는 `.def`는 214개다.
- **모드 안의 충돌 2,481건.** 위 표의 `in-mod-collision` 2,481행이 이것이다. `resource` 아래 낱개 `.mtl` 파일 2,481개가 `resource\entity\emitsheat.pak` 안의 같은 경로 파일과 내용이 다르다. 게임은 같은 모드 안에서 낱개 파일을 쓴다(2026-10-13 게임 확인, `docs/02-modding-research.md`의 "리소스 덮어쓰기"). 2026-10-16부터 이 분석도 낱개 파일을 쓰고 pak 쪽을 충돌로 적는다. `.mtl`은 재질 파일이라 유닛 수치에는 영향이 없다.
- **새 따옴표 이름 3,310개.** MACE와 겹치는 것을 포함해 바닐라와 비교한 수다.
- **바닐라에 없는 extender.** `extender "vision_mode"`가 차량 `.def` 35개에 쓰인다. MACE에는 한 곳뿐이다.
- **새 블록 이름 38종 중 23종은 지도 지점이다.** `set/dynamic_campaign/map_points.set`에 `point_7_3` 같은 지점이 추가됐다. 바닐라에도 같은 형식의 지점이 있다.
- **섞여 들어온 파일.** `entity/-vehicle/+ussr/tank_heavy/x/0bj_268_xx/error.log`는 다른 프로그램의 오류 로그다. 텍스트라서 키 목록에 "그 밖의 괄호 구문" 7종으로 잡혔다. 모드의 구성이 아니다.
- **풀리지 않은 구문이 44개 파일에 있다.** 대부분 채워지지 않은 매개변수다: `%door2` 14개 파일, `%door` 14개, `%group` 12개 등. `properties/projectile_impulse.ext`를 include하는 파일(`entity/inventory/-ammo/mine/s_mine_proj.def` 2행)이 하나 있는데 그 파일이 Fusion, MACE, 바닐라, 다른 구독 모드, 게임 pak 어디에도 없다(2026-10-16). 이 `.def`를 이름으로 부르는 곳도 없어(S-마인은 `wt.inc`에서 `mine_s`를 만든다) 읽히지 않는 파일로 보인다(추정).
- **분석 범위 밖의 유닛.** `entity/+inventory` 아래에 `.def` 66개가 있다. 이 도구는 `entity/inventory`만 아이템 엔티티로 보므로 이 파일들은 위의 수치에 들어 있지 않다(아래 "분석 범위 밖에 놓인 정의 파일").
- **2026-10-20 갱신에서 바뀐 것.** 창작마당 변경 기록에는 설명이 없어 2026-10-16 분석의 `file-index.csv`와 대조했다. `resource` 안에서 내용이 바뀐 파일 16개(텍스트 12, 참조용 4), 새 파일 387개(텍스트 47, 나머지는 모델·텍스처), 없어진 파일 0개다. 그 밖에 `localizations`의 설명 글 3개가 바뀌었다.
  - 새 소련 차량: `tank_medium/obj430u`(`obj430u`, `obj430u_1965`)와 `obj450`, 손상 모델 `x/*_x`·`*_xx`. 연구(`unit_research_rus.set`: `obj430u`는 `t-54_49` 다음, `obj450`은 `object279` 다음이고 `object_490`이 이제 `obj450` 다음), 컨퀘스트 로스터(`units_rus_mace_cq.set`), 시기 로스터와 교리(`units/late/units_rus.set`, `script/units/rus/*.lua`), `unit.reg`, `tp_control.set`, 사운드(`tank-specifics_mace.inc`), 설명 글(`desc_vehicles_rus2.pot`, `desc_doctrine_vehicles.pot`)이 함께 들어왔다.
  - 포: 새 `set/stuff/gun/100mm_AT`·`122mm_m62_t3`(설명 글 `desc_weapon_rus2.pot`). `130mm_m65`는 사거리 `("range_220")` → `{range 270 270}`과 탄종별 관통 거리, `150mm_kwkl38`은 `{parameters "he2"}` 블록을 더했다.
  - V1: `gun/v1rocket_gun`의 장전 900 → 750초, `gun/v1rocket_gun.weapon` 40 → 35초, `reactive/rocket_v1.weapon`의 `rechargeTime`·`recoveryTime` 900 → 750.
  - `vz68.def`의 동축 기관총 `kpvt` → `kpvt_2`.
  - 컨퀘스트 AI: `script/multiplayer/modes/conquest/orders.lua`(`ACTIVE_DEFENSE_CHANCE` 0.75 → 0.8, 빈 목록 검사), `runtime_config.lua`(줄 주석이 늘고 구조와 값이 바뀌었다: 난이도별 easy·normal·hard·heroic 표, 공격 파도·수비 첫 배치·수비 반격 값 등). 편집기가 이 판에서 읽는 키 317개는 모두 `data/annotations/settings.psv`의 주석이 붙는다(2026-10-21 확인).
- **게임 버전.** `mod.info`가 `"any"`다. 창작마당 갱신일은 2026-10-20이다(처음 조사는 2026-10-11, 2026-10-16·2026-10-21에 다시 분석했다).
