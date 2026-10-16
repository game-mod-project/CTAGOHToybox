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

| 문서 | 구분 | 파일 | 위치 | 속성 수 | 확인 | 문서 | 추정 | 정의 | 미확인 |
|---|---|---|---|---|---|---|---|---|---|
| [infantry.md](infantry.md) | 본체 | 보병 정의 | `set/breed/**/*.set` | 16 | 0 | 0 | 15 | 0 | 1 |
| [infantry.md](infantry.md) | 연관 | 능력치 매크로 | `set/breed/**/*.inc` | 91 | 0 | 0 | 73 | 18 | 0 |
| [infantry.md](infantry.md) | 연관 | 외형 엔티티 | `entity/humanskin/**/*.def` | 18 | 0 | 6 | 7 | 0 | 5 |
| [vehicle.md](vehicle.md) | 본체 | 차량 엔티티 | `entity/-vehicle/<진영>/<분류>/<이름>/<이름>.def` | 282 | 24 | 14 | 130 | 75 | 39 |
| [vehicle.md](vehicle.md) | 연관 | 차량 전용 속성 (.ext) | `entity/-vehicle/**/*.ext` | 180 | 1 | 15 | 99 | 40 | 25 |
| [vehicle.md](vehicle.md) | 연관 | 차량 전용 속성 (.inc) | `entity/-vehicle/**/*.inc` | 113 | 0 | 9 | 57 | 39 | 8 |
| [vehicle.md](vehicle.md) | 연관 | 공통 속성 (.ext) | `properties/**/*.ext` | 888 | 42 | 110 | 487 | 170 | 79 |
| [vehicle.md](vehicle.md) | 연관 | 공통 속성 (.inc) | `properties/**/*.inc` | 605 | 136 | 102 | 247 | 30 | 90 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 무기와 장구 정의 (확장자 없음) | `set/stuff/<폴더>/<이름>` | 157 | 43 | 7 | 66 | 23 | 18 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 무기 정의 (.weapon) | `set/stuff/{reactive,mortar,bazooka,explosive}/*.weapon` | 58 | 16 | 4 | 22 | 9 | 7 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 탄약 (.ammo) | `set/stuff/**/*.ammo` | 79 | 10 | 3 | 30 | 23 | 13 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 수류탄 (.grenade) | `set/stuff/grenade/*.grenade` | 20 | 4 | 0 | 14 | 0 | 2 |
| [weapon-ammo.md](weapon-ammo.md) | 본체 | 지뢰와 폭발물 (.mine) | `set/stuff/explosive/*.mine` | 8 | 2 | 0 | 6 | 0 | 0 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 투척 원형 (.hand) | `set/stuff/thrower.hand` | 15 | 3 | 0 | 8 | 0 | 4 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 원형 (.pattern) | `set/stuff/**/*.pattern` | 155 | 54 | 9 | 65 | 8 | 19 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 폴더의 매크로 모음 (.presets) | `set/stuff/<폴더>/.presets` | 533 | 30 | 21 | 401 | 48 | 33 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 무기 상태 설정 (.fsm) | `set/stuff/_settings/*.fsm` | 5 | 0 | 0 | 5 | 0 | 0 |
| [weapon-ammo.md](weapon-ammo.md) | 연관 | 폭발 계산 매크로 | `set/blast.inc` | 71 | 0 | 0 | 53 | 0 | 18 |
| [item-entity.md](item-entity.md) | 본체 | 아이템 엔티티 | `entity/inventory/**/*.def` | 78 | 2 | 4 | 46 | 0 | 26 |
| 합계 | | | | 3372 | 367 | 304 | 1831 | 483 | 387 |

## 표 읽는 법

| 열 | 내용 |
|---|---|
| 속성명 | 파일 안에서의 키 경로. 표기 규칙은 [../04-units/README.md](../04-units/README.md)의 "경로 표기"와 같다. `("fire rate")`처럼 괄호와 따옴표로 된 이름은 매크로 호출이고 값은 그 인자다. `(define "이름")/...`은 매크로 본문 안의 값이다 |
| 속성값 | 바닐라 파일에 실제로 쓰인 값. 숫자 하나면 최솟값 ~ 최댓값, 종류가 12개 이하면 전부, 그보다 많으면 예 하나 |
| 설명 | 맨 앞의 굵은 낱말이 근거의 종류다. 아래 "설명의 등급"을 본다 |
| 게임 파일의 주석 | 같은 키 옆에 개발자가 남긴 주석을 모은 것. 그 줄의 값에 대한 메모인 경우가 많아 키의 뜻과 다를 수 있다 |
| 파일 수 | 이 속성이 적힌 파일의 수 |
| 예시 파일 | 이 속성이 적힌 파일 하나 |

구분의 **본체**는 유닛 그 자체인 파일이고, **연관**은 본체가 include·매크로·이름으로 끌어오는 파일이다. 연관 파일의 값은 여러 유닛이 함께 쓴다.

## 설명의 등급

| 등급 | 근거 |
|---|---|
| 확인 | 개발자가 게임 파일에 남긴 주석이 그 키를 설명한다 |
| 문서 | 게임 밖의 문서에 설명이 있다. 대부분 같은 엔진을 쓰는 다른 게임의 문서이므로 이 게임에서도 같은지는 확인하지 않았다 |
| 추정 | 이름, 값, 쓰인 자리에서 추론했다. "(같은 자리의 본체 설명을 가져옴)"이 붙은 것은 본체 파일의 같은 경로에 달린 설명을 include되는 파일과 매크로 정의 안의 키에 옮겨 적은 것이다 |
| 정의 | 매크로 호출이다. 그 매크로의 정의를 읽어 인자마다 어느 키에 어떤 식으로 들어가는지를 적었다. 뜻풀이가 아니라 파일에서 기계적으로 뽑은 사실이다 |
| 미확인 | 근거를 찾지 못했다 |

"정의"의 식은 게임 파일의 전위 표기 그대로다. `( * %speed 0.6 )`은 speed × 0.6, `( / 360 %traverse )`는 360 ÷ traverse다. 대괄호 안은 그 키의 설명이다.

## 설명의 출처

2026-10-12에 조사했다. 키를 망라한 공식 레퍼런스는 찾지 못했다. 창작마당은 열려 있지만 개발사가 공개한 키 설명서는 없고, 가이드 작성자들은 공식 Discord를 문의처로 든다(Discord는 읽지 못했다).

| 출처 | 성격 | 얻은 것 |
|---|---|---|
| 게임 파일의 주석 | 개발사. 이 게임 | 구동계, 보급 구역, 체력·스태미나, 조준 화면 등. 가장 믿을 만한 근거다 |
| 개발자 변경 기록 (Steam 토론 4498514133983766024) | 개발사. 이 게임 | zeroing, FiringTimeoutDistanceFactor, detonationThickness, no_target 태그, extender "edifice" |
| docs.bestway.com.ua | 엔진 개발사(Best Way)의 공식 문서. Men of War II 기준 | 설정 문법, 장갑 블록, 보병 이동 속도 표, 상태 기계(fsm), 센서 |
| wiki.bestway.com.ua (web.archive.org 보존본) | 엔진 개발사의 옛 공식 위키. В тылу врага 2 기준 | 볼륨의 able 플래그, land_patch, 구동계 키의 단위 |
| calltoarms.fandom.com의 Modding 문서 | 커뮤니티 위키. Call to Arms 기준. 스스로 추측이 섞였다고 밝힌다 | 사거리 두 값, 관통력 표, 체력 피해 |
| ModDB, mow-portal.ru, xgm.guru, Steam 토론과 가이드 | 커뮤니티 글. Men of War 계열과 이 게임 | 무장 자리, 뼈대 속도, 장갑 밀도, 대포 애니메이션 |

이 게임(Gates of Hell)은 Barbedwire Studios가 GEM 2 엔진을 갈라 내어 만든 것이라, 다른 게임의 문서가 설명하는 키가 이 게임에서 같은 뜻인지는 보장되지 않는다. 그런 설명은 "문서" 등급으로 따로 표시했다.

조사에서 모은 항목 전체(출처 주소 포함)는 `data/annotations/research/findings-*.psv`에 있다. 컨퀘스트와 게임 모드 설정 키에 대한 항목도 거기에 있다.

## 무엇을 "수정 가능한 속성"으로 셌나

- 원본 파일에 값(인자)이 하나라도 적힌 블록과 매크로 호출이다. 값 없이 이름만 있는 블록(하위 블록을 담는 그릇, 인자 없는 플래그)은 뺐다.
- `(include ...)`, `(define ...)`의 이름, `(mod ...)` 줄 자체는 뺐다. 그 안에 든 블록은 들어 있다.
- 모델 파일(`.mdl`, `.mtl`)은 뺐다.
- 속성은 (파일 종류, 경로)로 묶은 것이다. 같은 경로가 여러 파일에 있으면 한 행이다. 유닛 하나의 실제 값은 `data/units-*.jsonl.gz`에 있다.

## 한계

- **값을 바꿨을 때 게임이 어떻게 반응하는지는 확인하지 않았다.** 이 문서는 값이 어디에 있는지만 알려 준다.
- **"추정"과 "문서"는 틀릴 수 있다.** 추정은 이름과 값에서 추론한 것이고, 문서는 대부분 다른 게임의 자료다. 값을 고치기 전에 "확인"과 "정의"를 먼저 믿고, 나머지는 게임에서 시험해 본다.
- **`##`로 시작하는 이름**(예: `##bone`)은 편집기가 꺼 둔 블록이다. 개발사 문서(Men of War II, https://docs.bestway.com.ua/foundational-knowledge/basic-knowledge-about-configuration-files.md)에 따르면 읽을 때 지우지 않지만 게임은 그 안의 값을 쓰지 않는다. 스탯 편집기도 이런 값은 고치지 못하게 한다.
- **편제·비용·게임 규칙은 여기에 없다.** `set/multiplayer`, `set/dynamic_campaign` 등 유닛 파일이 아닌 설정은 [../04-units/schema-shared.md](../04-units/schema-shared.md)에 있다.
- 기준은 바닐라 1.0.6.7이다. 모드가 추가한 속성은 [../05-workshop-mods/README.md](../05-workshop-mods/README.md)에 있다.
