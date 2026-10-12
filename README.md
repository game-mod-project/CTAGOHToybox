# CTAToybox

Call to Arms - Gates of Hell: Ostfront의 모드를 만들기 위한 레퍼런스 저장소다. 게임 파일을 분석한 문서, 그 문서를 만드는 도구, 빈 모드 폴더가 들어 있다.

기준: 게임 1.0.6.7 (Steam buildid 25492057), 조사 2026-10-11.

## 문서

읽는 순서대로다.

| 문서 | 내용 |
|---|---|
| [docs/01-game-structure.md](docs/01-game-structure.md) | 게임 폴더와 pak 아카이브의 구성, 파일 형식, 로딩 순서, 프로필·로그 위치 |
| [docs/02-modding-research.md](docs/02-modding-research.md) | 모드를 만드는 방법에 대한 조사. 출처와 함께, 웹 자료와 실제 파일의 차이까지 |
| [docs/03-mod-folder-setup.md](docs/03-mod-folder-setup.md) | 이 저장소의 모드 폴더가 게임에 어떻게 연결돼 있는지 |
| [docs/04-units/README.md](docs/04-units/README.md) | 유닛 구성 레퍼런스의 입구. 유닛을 이루는 파일의 참조 관계, 문법, 데이터 파일 안내 |
| [docs/04-units/coverage.md](docs/04-units/coverage.md) | 누락 검증 결과와, 그 검증이 보장하는 것·보장하지 않는 것 |
| [docs/05-workshop-mods/README.md](docs/05-workshop-mods/README.md) | 창작마당 모드 4종이 바닐라에 무엇을 더했는지 |
| [docs/06-editable-attributes/README.md](docs/06-editable-attributes/README.md) | 유닛의 값을 바꾸려면 어느 파일의 어떤 속성을 고쳐야 하는지. 파일별 속성명·속성값·설명 |

### 유닛 스키마

유닛을 이루는 모든 키를 유형별로 정리한 표다. 양이 많으므로 [docs/04-units/README.md](docs/04-units/README.md)의 "문서 구성"과 "경로 표기"를 먼저 읽는다.

| 문서 | 대상 |
|---|---|
| [schema-human.md](docs/04-units/schema-human.md) | 보병과 보병 외형 |
| [schema-vehicle.md](docs/04-units/schema-vehicle.md) | 차량, 화포, 항공기, 함정 |
| [schema-weapon-ammo.md](docs/04-units/schema-weapon-ammo.md) | 무기, 탄약, 수류탄, 지뢰, 장구 |
| [schema-inventory.md](docs/04-units/schema-inventory.md) | 아이템의 3D 엔티티 |
| [schema-shared.md](docs/04-units/schema-shared.md) | 공통 속성, 편제와 비용, 게임 규칙 |

### 모드별 문서

| 문서 | 모드 |
|---|---|
| [2905667604.md](docs/05-workshop-mods/2905667604.md) | MACE |
| [3732639687.md](docs/05-workshop-mods/3732639687.md) | MACE Wunderwaffe Ultimate Fusion ENG |
| [3295913567.md](docs/05-workshop-mods/3295913567.md) | Spawn Menu |
| [2829744930.md](docs/05-workshop-mods/2829744930.md) | Auto Medic-AA-Artillery & Weather Switch |

## 이 문서들을 믿어도 되는 정도

- **값과 구조는 게임 파일에서 기계적으로 뽑았다.** 어떤 키가 어느 파일에 몇 번, 어떤 값으로 나오는지는 파일 그대로다. 빠진 것이 없는지는 다섯 가지 검사로 확인하며 결과는 coverage.md에 있다.
- **키의 뜻은 대부분 확인되지 않았다.** 사람이 살펴 등급을 매긴 것은 자주 쓰이는 키 수백 개다. "확인"은 개발자가 파일에 남긴 주석에 근거하고, "추정"은 이름과 값에서 추론한 것이다. 나머지는 `미확인(자동)`이다.
- **게임에서 값을 바꿔 가며 검증한 것은 없다.** 엔진의 내부 동작도 조사하지 않았다.
- **include와 매크로를 펼치는 규칙은 파일에서 추론했다.** 근거와 한계는 docs/04-units/README.md의 "한계"에 있다.

## 폴더

| 경로 | 내용 |
|---|---|
| `docs/` | 문서 |
| `data/` | 문서의 바탕이 되는 데이터. 유닛별 전체 값은 여기에 있다 |
| `tools/` | 데이터와 문서를 만드는 스크립트 (PowerShell 7, 추가 설치 없음) |
| `mod/CTAToybox/` | 모드 본체. 지금은 `mod.info`만 있는 빈 뼈대다 |
| `_work/` | 게임에서 추출한 원본. 저장소에 넣지 않는다 |

어느 파일이 생성물이고 어느 파일을 손으로 고치는지, 다시 생성하는 명령은 [CLAUDE.md](CLAUDE.md)에 있다.

## 만든 과정의 기록

| 문서 | 내용 |
|---|---|
| [docs/superpowers/specs/2026-10-11-ctatoybox-reference-design.md](docs/superpowers/specs/2026-10-11-ctatoybox-reference-design.md) | 처음에 합의한 설계 |
| [docs/superpowers/plans/2026-10-11-ctatoybox-reference.md](docs/superpowers/plans/2026-10-11-ctatoybox-reference.md) | 구현 계획. 실제 구현은 여러 곳에서 달라졌으며 차이는 그 문서 머리에 적혀 있다 |
| [docs/superpowers/specs/2026-10-12-stat-editor-design.md](docs/superpowers/specs/2026-10-12-stat-editor-design.md) | 스탯 편집기의 설계. 아직 구현하지 않았다 |
| [docs/superpowers/plans/2026-10-12-stat-editor-core.md](docs/superpowers/plans/2026-10-12-stat-editor-core.md) | 스탯 편집기 구현 계획 1: 코어와 명령줄 도구. 아직 실행하지 않았다 |
