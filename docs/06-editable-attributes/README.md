# 유닛의 수정 가능한 속성

이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다.

유닛의 값을 바꾸려 할 때 "그 값이 어느 파일에 있는가"를 찾기 위한 문서다. 유닛 종류마다, 값이 적힌 파일을 본체와 연관 파일로 나눠 속성명·속성값·설명을 실었다.

## 문서

| 문서 | 대상 |
|---|---|
| [infantry.md](infantry.md) | 보병 |
| [vehicle.md](vehicle.md) | 차량, 전차, 화포, 항공기, 함정 |
| [weapon-ammo.md](weapon-ammo.md) | 무기, 탄약, 수류탄, 지뢰, 장구 |
| [item-entity.md](item-entity.md) | 아이템의 3D 엔티티 |

## 파일별 속성 수

| 문서 | 구분 | 파일 | 위치 | 속성 수 | 뜻을 아는 속성 |
|---|---|---|---|---|---|
| [infantry.md](infantry.md) | 본체 | 보병 정의 | `set/breed/**/*.set` | 16 | 13 |
| [infantry.md](infantry.md) | 연관 | 능력치 매크로 | `set/breed/**/*.inc` | 91 | 0 |
| [infantry.md](infantry.md) | 연관 | 외형 엔티티 | `entity/humanskin/**/*.def` | 18 | 8 |
| [vehicle.md](vehicle.md) | 본체 | 차량 엔티티 | `entity/-vehicle/<진영>/<분류>/<이름>/<이름>.def` | 282 | 113 |
| [vehicle.md](vehicle.md) | 연관 | 차량 전용 속성 (.ext) | `entity/-vehicle/**/*.ext` | 180 | 0 |
| [vehicle.md](vehicle.md) | 연관 | 차량 전용 속성 (.inc) | `entity/-vehicle/**/*.inc` | 113 | 1 |
| [vehicle.md](vehicle.md) | 연관 | 공통 속성 (.ext) | `properties/**/*.ext` | 888 | 1 |
| [vehicle.md](vehicle.md) | 연관 | 공통 속성 (.inc) | `properties/**/*.inc` | 605 | 0 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 무기와 장구 정의 (확장자 없음) | `set/stuff/<폴더>/<이름>` | 157 | 84 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 무기 정의 (.weapon) | `set/stuff/{reactive,mortar,bazooka,explosive}/*.weapon` | 58 | 31 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 탄약 (.ammo) | `set/stuff/**/*.ammo` | 79 | 36 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 수류탄 (.grenade) | `set/stuff/grenade/*.grenade` | 20 | 16 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 지뢰와 폭발물 (.mine) | `set/stuff/explosive/*.mine` | 8 | 8 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 투척 (.hand) | `set/stuff/thrower.hand` | 15 | 0 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 원형 (.pattern) | `set/stuff/**/*.pattern` | 155 | 102 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 폴더의 매크로 모음 (.presets) | `set/stuff/<폴더>/.presets` | 533 | 0 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 무기 상태 설정 (.fsm) | `set/stuff/_settings/*.fsm` | 5 | 0 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 폭발 계산 매크로 | `set/blast.inc` | 71 | 0 |
| [item-entity.md](item-entity.md) | 본체 | 아이템 엔티티 | `entity/inventory/**/*.def` | 78 | 38 |
| 합계 | | | | 3372 | 451 |

## 표 읽는 법

| 열 | 내용 |
|---|---|
| 속성명 | 파일 안에서의 키 경로. 표기 규칙은 [../04-units/README.md](../04-units/README.md)의 "경로 표기"와 같다. `("fire rate")`처럼 괄호와 따옴표로 된 이름은 매크로 호출이고 값은 그 인자다. `(define "이름")/...`은 매크로 본문 안의 값이다 |
| 속성값 | 바닐라 파일에 실제로 쓰인 값. 숫자 하나면 최솟값 ~ 최댓값, 종류가 12개 이하면 전부, 그보다 많으면 예 하나 |
| 설명 | **확인**은 게임 파일의 주석이 그 키를 직접 설명한 것, **추정**은 이름·값·쓰인 자리에서 추론한 것, 미확인은 뜻을 모르는 것이다 |
| 게임 파일의 주석 | 같은 키 옆에 개발자가 남긴 주석을 모은 것. 그 줄의 값에 대한 메모인 경우가 많아 키의 뜻과 다를 수 있다 |
| 파일 수 | 이 속성이 적힌 파일의 수 |
| 예시 파일 | 이 속성이 적힌 파일 하나 |

구분의 **본체**는 유닛 그 자체인 파일이고, **연관**은 본체가 include·매크로·이름으로 끌어오는 파일이다. 연관 파일의 값은 여러 유닛이 함께 쓴다.

## 무엇을 "수정 가능한 속성"으로 셌나

- 원본 파일에 값(인자)이 하나라도 적힌 블록과 매크로 호출이다. 값 없이 이름만 있는 블록(하위 블록을 담는 그릇, 인자 없는 플래그)은 뺐다.
- `(include ...)`, `(define ...)`의 이름, `(mod ...)` 줄 자체는 뺐다. 그 안에 든 블록은 들어 있다.
- 모델 파일(`.mdl`, `.mtl`)은 뺐다.
- 속성은 (파일 종류, 경로)로 묶은 것이다. 같은 경로가 여러 파일에 있으면 한 행이다. 유닛 하나의 실제 값은 `data/units-*.jsonl.gz`에 있다.

## 한계

- **값을 바꿨을 때 게임이 어떻게 반응하는지는 확인하지 않았다.** 이 문서는 값이 어디에 있는지만 알려 준다.
- **뜻을 아는 속성이 적다.** 연관 파일(`.inc`, `.ext`, `.presets`)에는 이름만으로 뜻을 붙이지 않았다. 같은 이름이 파일마다 다른 뜻으로 쓰이기 때문이다.
- **`##`로 시작하는 이름**(예: `##bone`)은 꺼 둔 블록으로 보인다. 엔진의 처리 방식은 확인하지 못했다.
- **편제·비용·게임 규칙은 여기에 없다.** `set/multiplayer`, `set/dynamic_campaign` 등 유닛 파일이 아닌 설정은 [../04-units/schema-shared.md](../04-units/schema-shared.md)에 있다.
- 기준은 바닐라 1.0.6.7이다. 모드가 추가한 속성은 [../05-workshop-mods/README.md](../05-workshop-mods/README.md)에 있다.
