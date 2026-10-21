# 강화-독일 수정 내용

- 만든 날: 2026-10-21
- 베이스: 월드워-차종
- 군대: ger(유닛 2118: 보병 1192, 차량 926)
- 원본 계층: vanilla → mod_2905667604 → mod_3732639687 → mod_2829744930 → ctatoyboxspawn → ctatoyboxspawn_ger → ctatoyboxspawn_rus → ctatoyboxspawn_usa
- 배수: 인벤토리 ×2, 탄약 ×5, 재장전 ×0.5, 보병 체력 ×2, 장갑 차량 ×2·경전차 ×2·중형전차 ×2.5·중전차 ×3·초중전차 ×5, 속도 ×1.5, 연료·항속 ×10, 보급 반경 ×3, 보급 총량 50000, 보급 재생 10초에 가득(재생 주기 0.0002초)
- 클립식 기준: 탄창 2발 이상, 발 사이 1초 이상인 `set/stuff/gun` 무기 사본. 발 사이 ×0.5, 소수 셋째 자리까지
- 수정 28960건, 일괄 규칙 11개

## 항목별 수정

| 항목 | 수정 수 |
|---|---|
| 베이스 수정 | 605 |
| ger 보병 탄약 ×5 | 2237 |
| ger 보병 소지품 ×2 | 7397 |
| ger 차량 탄약 ×5 | 962 |
| ger 차량 소지품 ×2 | 896 |
| ger 장갑 차량 ×2 | 2049 |
| ger 장갑 경전차 ×2 | 2021 |
| ger 장갑 중형전차 ×2.5 | 3983 |
| ger 장갑 중전차 ×3 | 2886 |
| ger 장갑 초중전차 ×5 | 702 |
| ger 속도 ×1.5 | 234 |
| ger 연료 ×10 | 584 |
| ger 항속(range) ×10 | 234 |
| ger 클립식 발 사이 ×0.5 | 6 |
| ger 무기 사본 | 245 |
| ger 재장전 값 ×0.5 | 167 |
| ger 재장전 줄 더하기 | 103 |
| ger 무기 이름 → 사본 | 2190 |
| ger 보병 체력 ×2 | 1192 |
| ger 보급 매크로 복제 | 6 |
| ger 보급 반경 ×3 | 6 |
| ger 보급 총량 → 50000 | 6 |
| ger 보급 재생 주기 → 0.0002초(10초에 가득) | 9 |
| ger 보급 호출 → 복제본 | 12 |
| 컨퀘스트 자원(플레이어) | 72 |
| 스커미시 플레이어 ×50 | 144 |
| 스커미시 봇 배율 ÷50 | 12 |

일괄 규칙(앱의 "일괄 규칙"에서 다시 적용할 수 있다):

| 규칙 | 대상 | 키 | 바꾸기 |
|---|---|---|---|
| ger 보병 탄약 ×5 | breed | `**/inventory/item, **/box/item` | ×5 |
| ger 보병 소지품 ×2 | breed | `**/inventory/item, **/box/item` | ×2 |
| ger 차량 탄약 ×5 | vehicle | `**/inventory/item, **/box/item` | ×5 |
| ger 차량 소지품 ×2 | vehicle | `**/inventory/item, **/box/item` | ×2 |
| ger 장갑 차량 ×2 | vehicle | `**/thickness, **/thickness/front, **/thickness/top, **/thickness/rear, **/thickness/bottom, **/thickness/left, **/thickness/right` | ×2 |
| ger 장갑 경전차 ×2 | vehicle | `**/thickness, **/thickness/front, **/thickness/top, **/thickness/rear, **/thickness/bottom, **/thickness/left, **/thickness/right` | ×2 |
| ger 장갑 중형전차 ×2.5 | vehicle | `**/thickness, **/thickness/front, **/thickness/top, **/thickness/rear, **/thickness/bottom, **/thickness/left, **/thickness/right` | ×2.5 |
| ger 장갑 중전차 ×3 | vehicle | `**/thickness, **/thickness/front, **/thickness/top, **/thickness/rear, **/thickness/bottom, **/thickness/left, **/thickness/right` | ×3 |
| ger 장갑 초중전차 ×5 | vehicle | `**/thickness, **/thickness/front, **/thickness/top, **/thickness/rear, **/thickness/bottom, **/thickness/left, **/thickness/right` | ×5 |
| ger 속도 ×1.5 | vehicle | `**/locomotion/maxspeed` | ×1.5 |
| ger 연료 ×10 | vehicle | `**/fuelbag/volume, **/fuelbag/remain` | ×10 |

## 클립식 포

탄창 안 발 사이 간격과 탄창 전체 장전을 같은 배수로 줄여 원래 짧은/긴 비율을 지킨다.

| 무기 사본 | 탄창 | 발 사이(초) | 탄창 장전(초) |
|---|---|---|---|
| `set/stuff/gun/128mm_kwk44_auto_ctb_ger` | 6 | 5.00 → 2.5 | 35 → 17.5 |
| `set/stuff/gun/128mm_l40_auto_ctb_ger` | 3 | 2.5 → 1.25 | 18 → 9 |
| `set/stuff/gun/128oto_ctb_ger` | 3 | 20 → 10 | 18 → 9 |
| `set/stuff/gun/88mm_flak41_2_ctb_ger` | 4 | 2.5 → 1.25 | 25 → 12.5 |
| `set/stuff/gun/88mm_kwk43_d_ctb_ger` | 2 | 1 → 0.5 | 10 → 5 |

## 보급

그 군대 보급 유닛이 부르는 `properties/resupply.inc` 매크로를 복제해 값을 바꾸고, 그 유닛의 호출 이름만 복제본으로 바꾼다. 원래 매크로와 보급 건물(깃발 탄약점, 탄약 진지 등)은 그대로다.

| 매크로 | 복제본 | 반경(m) | 총량 | 재생 주기(초) | 바꾼 유닛 |
|---|---|---|---|---|---|
| `supply_ammo` | `supply_ammo_ctb_ger` | 10 → 30 | %supply → 50000 | 10 → 0.0002 | `entity/-vehicle/+germany/btr/wanze_ammo/wanze_ammo.def`, `entity/-vehicle/generic/special/inf_crate_ger/inf_crate_ger.def`, `entity/-vehicle/germany/trailer/ammo_trailer/ammo_trailer.def`, `entity/-vehicle/germany/trailer/infantry_cart_if8/infantry_cart_if8.def`, `entity/-vehicle/germany/trailer/sdah_57_1/sdah_57_1.def` |
| `supply_engineer` | `supply_engineer_ctb_ger` | 15 → 45 | 5000 → 50000 | 0 → 0.0002 | `entity/-vehicle/+germany/tank_heavy/bergepanther/bergepanther.def`, `entity/-vehicle/germany/car/blitz3_6_engineering/blitz3_6_engineering.def` |
| `supply_engineer_light` | `supply_engineer_light_ctb_ger` | 15 → 45 | 2000 → 50000 | 0 → 0.0002 | `entity/-vehicle/germany/car/kettenkrad_eng/kettenkrad_eng.def` |
| `supply_fuel` | `supply_fuel_ctb_ger` | 15 → 45 | %liters → 50000 | 0 → 0.0002 | `entity/-vehicle/germany/car/blitz3_6_fuel/blitz3_6_fuel.def` |
| `truck_ammo_heavy` | `truck_ammo_heavy_ctb_ger` | 20 → 60 | 3000 → 50000 | 5 → 0.0002 | `entity/-vehicle/germany/car/blitz3_6_art_ammo/blitz3_6_art_ammo.def`, `entity/-vehicle/germany/car/opel_maultier/opel_maultier.def` |
| `truck_ammo_heavy_2x` | `truck_ammo_heavy_2x_ctb_ger` | 25 → 75 | 8000 → 50000 | 5 → 0.0002 | `entity/-vehicle/germany/btr/sdkfz6_cargo/sdkfz6_ammo.def` |

## 건너뛴 것

모두 3494건. 이유별 수(많은 순 20개). 전체 목록은 `faction-preset --report <파일>`이 쓴다.

| 이유 | 수 | 예 |
|---|---|---|
| ger 차량 탄약 ×5: 이 키가 없다 | 633 |  |
| ger 차량 소지품 ×2: 이 키가 없다 | 624 |  |
| ger 속도 ×1.5: 이 키가 없다 | 413 |  |
| ger 연료 ×10: 이 키가 없다 | 413 |  |
| ger 장갑 차량 ×2: 매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다 | 303 |  |
| ger 장갑 중형전차 ×2.5: 매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다 | 301 |  |
| ger 장갑 중전차 ×3: 매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다 | 178 |  |
| ger 장갑 경전차 ×2: 매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다 | 127 |  |
| ger 장갑 중전차 ×3: 이 키가 없다 | 80 |  |
| 매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다 | 80 | `entity/-vehicle/+germany/btr/wanze_ammo/wanze_ammo.def` |
| ger 장갑 초중전차 ×5: 매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다 | 76 |  |
| ger 장갑 중형전차 ×2.5: 이 키가 없다 | 74 |  |
| ger 장갑 차량 ×2: 이 키가 없다 | 74 |  |
| ger 연료 ×10: 매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다. '전부'만 된다 | 52 |  |
| ger 장갑 경전차 ×2: 이 키가 없다 | 38 |  |
| ger 보병 소지품 ×2: 이 키가 없다 | 15 |  |
| /from/parameters/rechargetime를 넣을 블록이 사본에 없다 | 8 | `set/stuff/gun/150mm_l11_ctb_ger` |
| ger 보병 탄약 ×5: 이 키가 없다 | 2 |  |
| ger 연료 ×10: 새 값이 원래 값과 같다(0) | 2 |  |
| ger 속도 ×1.5: 새 값이 원래 값과 같다(0) | 1 |  |

## 두 군대가 같이 쓰는 유닛

고치면 다른 군대도 바뀌므로 건너뛴다.

- `entity/-vehicle/+finland/cannon/37mm_m1915_trench_gun/37mm_m1915_trench_gun.def: rus, fin`
- `entity/-vehicle/germany/btr/sdkfz10_4/sdkfz10_4.def: ger, fin`
- `entity/-vehicle/germany/cannon/100mm_nbw35/100mm_nbw35.def: ger, fin`
- `entity/-vehicle/germany/cannon/210mm_nebelwerfer42/210mm_nebelwerfer42.def: ger, fin`
- `entity/-vehicle/germany/tank_light/pz38h_w40/pz38h_w40.def: ger, fin`

## 한계와 미확인

- 백로그 E19: 장전 시간이 원형의 탄종별 `{parameters …}` 블록 안에 있는 무기, 매크로 본문에서 무기 이름을 부르는 차량은 강화가 빠진다.
- 백로그 E20: 모든 군대가 같이 쓰는 매크로 본문 안의 장갑(`sandbags` 등)은 바꾸지 않았다.
- 보급 `limit 0`은 파일 주석상 무제한이지만 게임에서는 보급이 동작하지 않았다(사용자 게임 확인 2026-10-21). 그래서 총량은 게임 파일에 쓰인 가장 큰 값(50000) 안에서 정한다. 엔진이 받는 최대치는 미확인이다.
- 재생 주기 0.0002초처럼 1초보다 짧은 소수 값은 게임 파일에 쓰인 적이 없다(쓰인 값은 0·2·5·10). 게임이 이 값대로 채우는지는 미확인이다. 받지 못해도 총량은 그대로다.
- 반경을 키우면 보급 범위 그림이 같이 커지는지는 미확인이다. 컨퀘스트의 보급 차량 AP 비용(로스터의 `supply_ap`)은 그대로다.
- 보급 매크로 복제본은 `limit`을 직접 적으므로 호출에 남은 총량 인자(`supply(500)`, `liters(2500)`)는 쓰이지 않는다. 쓰이지 않는 인자를 게임이 문제없이 넘기는지는 미확인이다.
- 강화한 무기 사본·보급 차량을 적이 노획하면 그 적도 빠른 재장전·넓은 무제한 보급을 쓴다(받아들임).
- 사본에 더한 `rechargeTime`·`recoveryTime` 줄과 보병 perks에 더한 `health` 줄이 원형·매크로 값을 이기는지 게임 안에서 확인하지 않았다.
- 같은 블록에 `recoveryTime`이 여러 줄인 무기는 모두 곱했다. 게임이 어느 줄을 쓰는지 미확인이다.

이 문서는 `faction-preset`이 만든다. 손으로 고치지 않는다.
