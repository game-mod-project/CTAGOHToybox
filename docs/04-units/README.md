# 유닛 구성 레퍼런스

## 범위와 기준

| 항목 | 값 |
|---|---|
| 게임 버전 | 1.0.6.7 (buildid 25492057) |
| 조사 날짜 | 2026-10-11 |
| 대상 | 바닐라 게임과 설치된 DLC 전부. 모드는 `docs/05-workshop-mods`에서 다룬다 |

여기서 "유닛"은 조작하거나 스폰할 수 있는 모든 대상(보병, 차량, 화포, 항공기, 함정)과 그것이 참조하는 구성 요소(무기, 탄약, 장구, 상속 속성, 편제, 표시 이름)를 말한다.

- 포함: `set/`, `script/`, `properties/`, `entity/-vehicle/`, `entity/humanskin/`, `entity/inventory/` 아래의 모든 텍스트 파일 33,732개
- 제외: 메시·애니메이션·텍스처·사운드의 내부 내용, 맵·미션 파일, 건물·지형·식생·이펙트 엔티티, 인터페이스

검증 결과와 그 검증이 무엇을 보장하고 무엇을 보장하지 않는지는 [coverage.md](coverage.md)에 있다.

## 문서 구성

| 문서 | 내용 |
|---|---|
| [schema-human.md](schema-human.md) | 보병: `set/breed`, `entity/humanskin` |
| [schema-vehicle.md](schema-vehicle.md) | 차량·화포·항공기·함정: `entity/-vehicle` |
| [schema-weapon-ammo.md](schema-weapon-ammo.md) | 무기·탄약·수류탄·지뢰·장구: `set/stuff` |
| [schema-inventory.md](schema-inventory.md) | 아이템 엔티티: `entity/inventory` |
| [schema-shared.md](schema-shared.md) | 그 밖의 전부: `properties`, 편제, 게임 규칙, 상호작용 |
| [coverage.md](coverage.md) | 누락 검증 결과와 한계 |

스키마 문서는 각각 두 부분으로 되어 있다.

- **원본 파일에 적힌 키**: 파일에 쓰인 그대로의 위치. 매크로 정의 안의 키는 `(define "이름")` 아래에 나온다.
- **펼친 뒤의 키**: include와 매크로를 풀어 유닛 하나 안에서의 실제 위치로 정리한 것. 유닛을 수정할 때는 이쪽을 먼저 본다.

표의 열:

| 열 | 뜻 |
|---|---|
| 경로 | 블록 이름을 `/`로 이은 것. 표기 규칙은 아래 "경로 표기" 참고 |
| 형태 | `block`, `include`, `define`, `mod`, `macro`, `paren-other`, `bare` |
| 값 형태 | 인자의 종류와 등장 횟수. `[num num]x120`은 수 두 개짜리가 120번 |
| 파일 수, 횟수 | 이 키가 나오는 파일 수와 전체 등장 횟수 |
| 값 범위·예 | 수 하나면 최솟값~최댓값, 종류가 12개 이하면 전체 목록, 그 밖에는 예시 하나 |
| 등급, 의미, 출처 | 아래 "의미 등급" 참고 |
| 같은 이름 키의 원문 주석(참고) | 게임 파일에서 같은 이름의 키 옆에 달린 주석을 이름만으로 모은 것. 이 경로의 뜻과 다를 수 있다 |

### 의미 등급

| 등급 | 기준 |
|---|---|
| 확인 | 게임 파일의 주석이 그 키를 직접 설명한다 |
| 문서 | 게임 밖의 문서에 설명이 있다. 대부분 같은 엔진을 쓰는 다른 게임의 문서다. 출처 칸에 어느 문서인지 적었다 |
| 추정 | 이름, 값의 분포, 쓰인 자리에서 추론했다. 의미 칸에 근거를 적었다 |
| 미확인 | 살펴봤지만 알 수 없다. 확인한 것만 적었다 |
| 미확인(자동) | 사람이 살펴보지 않았다 |

사람이 등급을 매긴 것은 펼친 뒤의 키 가운데 그 유닛 종류 파일의 5% 이상에 나오는 434개와, 2026-10-12에 경로 꼬리(`**/a/b`)로 추가한 것이다. 꼬리 주석은 include되는 파일과 매크로 정의 안의 같은 키에도 적용된다. 나머지는 `미확인(자동)`이며 원문 주석 열을 참고할 수 있다. 값을 고칠 목적이라면 파일별로 정리한 [../06-editable-attributes/README.md](../06-editable-attributes/README.md)가 더 편하다. 등급별 개수는 [coverage.md](coverage.md) 끝에 있다.

엔진 실행 파일의 내부 동작은 조사하지 않았다. "확인"도 개발자가 파일에 남긴 주석에 근거한 것이며 게임에서 값을 바꿔 가며 검증한 것은 아니다.

## 유닛 종류별 개수

| 종류 | 파일 수 | 위치 | 최상위 블록 |
|---|---|---|---|
| 보병 (breed) | 5,083 | `set/breed/**/*.set` | `{breed` |
| 보병 외형 (humanskin) | 3,667 | `entity/humanskin/**/*.def` | `{game_entity` |
| 차량류 (vehicle) | 2,443 | `entity/-vehicle/**/*.def` | `{game_entity` 2,384, `{actor` 58, `{entity` 1 |
| 아이템 엔티티 (item-entity) | 2,253 | `entity/inventory/**/*.def` | `{actor` 1,694, `{game_entity` 546, `{entity` 13 |
| 아이템 (stuff) | 2,720 | `set/stuff/**` (`.pattern`, `.presets`, `.inc`, 동작 설정 파일 제외) | `{from` 2,522, `{bullet` 121, `{item` 64, 그 밖 13 |
| 원형 (pattern) | 64 | `set/stuff/**/*.pattern` | `{from` 43, `{bullet` 8, `{item` 7, `{weapon_bullet` 6 |

`set/breed`의 텍스트 파일은 5,126개다. `.set` 5,083개가 보병이고 나머지 43개는 `ability.inc` 같은 `.inc` 파일이다.

### 보병

| 경로 | 수 |
|---|---|
| `set/breed/mp/usa` | 1,325 |
| `set/breed/mp/eng` | 936 |
| `set/breed/mp/ger` | 862 |
| `set/breed/mp/fin` | 538 |
| `set/breed/mp/rus` | 497 |
| `set/breed/single/rus` | 353 |
| `set/breed/single/ger` | 251 |
| `set/breed/mp/can` | 95 |
| `set/breed/mp/pol` | 83 |
| `set/breed/mp/fra` | 10 |
| `set/breed/mp/ned` | 9 |

`mp`는 멀티플레이와 컨퀘스트용, `single`은 캠페인용이다. 그 아래는 시기(`early`, `mid`, `late`)나 연도·계절(`1941_summer` 등)로 나뉜다.

### 차량류

| 진영 폴더 | `.def` 수 | | 분류 폴더 | `.def` 수 |
|---|---|---|---|---|
| `eng` | 653 | | `cannon` | 704 |
| `germany` | 557 | | `tank_medium` | 531 |
| `ussr` | 465 | | `btr` | 366 |
| `usa` | 388 | | `tank_light` | 291 |
| `finland` | 330 | | `car` | 234 |
| `generic` | 44 | | `tank_heavy` | 165 |
| `placeholder` | 6 | | `airborne` | 60 |
| | | | `train` 26, `marine` 22, `special` 21, `trailer` 17, `generic` 6 | |

`.def` 2,443개가 곧 서로 다른 차량 2,443대는 아니다. 한 차량 폴더에 본체 `.def` 말고도 `_x`, `_xx`가 붙은 파괴 모델용 `.def`가 함께 있다. `{chassis` 블록이 있는 것은 758개다.

### 아이템

`set/stuff`의 폴더별 파일 수: `head` 1,553, `shell` 313, `gun` 243, `mgun` 114, `rifle` 103, `mortar` 62, `bombing` 56, `pistol` 55, `smg` 52, `grenade` 43, `reactive` 41, `special` 32, `bazooka` 31, `explosive` 31, `flame` 23, `melee` 12, `body` 10, `med` 8.

`entity/inventory`의 폴더별 `.def` 수: `-gear_wearable` 1,475, `-ammo` 452, `-weapon` 281, `-stuff` 20, `-tools` 17, `-medical` 8.

## 유닛을 이루는 파일의 참조 사슬

화살표에 적힌 것이 연결하는 키다. 모두 [references.csv.gz](../../data/references.csv.gz)에서 실제 연결을 확인한 것이다.

### 보병

```mermaid
flowchart LR
    B["보병 정의<br/>set/breed/.../이름.set"]
    H["외형 엔티티<br/>entity/humanskin/.../이름.def"]
    M["모델<br/>.mdl, .ply, .mtl"]
    S["아이템<br/>set/stuff/.../이름"]
    P["원형<br/>set/stuff/.../이름.pattern"]
    I["아이템 엔티티<br/>entity/inventory/.../이름.def"]
    A["능력치 매크로<br/>ability.inc → set/breed/presets/perks/*.inc"]
    N["표시 이름<br/>desc/human/경로"]

    B -- "skin" --> H
    H -- "extension" --> M
    B -- "armors/head" --> S
    B -- "inventory/item" --> S
    B -- "perks, veterancy의 (include)" --> A
    S -- "from \"pattern 이름\"" --> P
    S -- "entity" --> I
    I -- "extender \"stuff\"/item (대개 엔티티와 같은 이름)" --> S
    B -. "파일 경로" .-> N
```

### 차량류

```mermaid
flowchart LR
    V["차량 엔티티<br/>entity/-vehicle/진영/분류/이름/이름.def"]
    X["공통 속성<br/>properties/*.ext, *.inc"]
    E["차량 전용 속성<br/>같은 폴더의 이름.ext"]
    M["모델<br/>.mdl, .ply, .vol"]
    W["무기<br/>set/stuff/.../이름"]
    AM["탄약<br/>set/stuff/.../이름.ammo"]
    N["표시 이름<br/>desc/entity/이름"]
    R["편제·비용<br/>set/multiplayer/units/**"]

    V -- "(include \"/properties/...\")" --> X
    V -- "(include \"이름.ext\")" --> E
    V -- "extension" --> M
    V -- "weaponry/place/weapon" --> W
    V -- "extender \"inventory\"/box/item" --> AM
    W -- "filling" --> AM
    V -. "파일명" .-> N
    R -- "유닛 이름" --> V
```

### 무기와 탄약

```mermaid
flowchart LR
    W["무기<br/>set/stuff/폴더/이름"]
    PR["폴더의 매크로 모음<br/>set/stuff/폴더/.presets"]
    P["원형<br/>.pattern"]
    AM["탄약<br/>.ammo"]
    BL["폭발 계산 매크로<br/>set/blast.inc"]
    I["아이템 엔티티<br/>entity/inventory/.../이름.def"]

    W -- "from \"pattern 이름\"" --> P
    W -- "(\"fire rate\"), (\"velocity\") 등" --> PR
    W -- "filling \"ammo 태그\"" --> AM
    AM -- "from \"pattern 이름\"" --> P
    AM -- "(\"he_level_3\") 등" --> BL
    PR -- "(include \"\\set\\blast.inc\")" --> BL
    W -- "entity" --> I
    AM -- "entity" --> I
```

## 문법

실제 파일 33,732개(토큰 1,177만 개)를 파싱해 확인한 것이다. 왕복 검사에 실패한 파일은 1개뿐이고 그 파일은 원본에 닫는 괄호가 남는다.

| 구문 | 예 | 설명 |
|---|---|---|
| 블록 | `{mass 16}` | 이름 뒤에 인자와 하위 블록이 온다. 이름의 대소문자는 구분하지 않는 것으로 보인다(`Extension`과 `extension`이 섞여 쓰임) |
| 주석 | `; 설명`, `;// 설명` | `;`부터 줄 끝까지 |
| 문자열 | `"rifle_usa garand_clip ap ammo"` | 이스케이프는 쓰이지 않는다 |
| include | `(include "/properties/cannon.ext" scale(1.0))` | `/`로 시작하면 리소스 루트 기준, 아니면 그 파일의 폴더 기준. `\set\blast.inc`처럼 역슬래시도 쓴다. 뒤의 `이름(값)`은 대상 파일 안의 `%이름`에 들어간다 |
| 매크로 정의 | `(define "fire rate" {recoveryTime (/ 60 %rpm)})` | 이름과 본문 |
| 매크로 호출 | `("fire rate" rpm(240))`, `("wheel" args "wheelright")` | 이름 있는 인자는 `%rpm`으로, `args` 뒤의 인자는 `%0`, `%1`로 본문에 들어간다 |
| 산술식 | `(/ 60 %rpm)`, `(+ 5 (* %filler 1))` | 연산자가 앞에 오는 전위 표기. `+ - * /` |
| 조건 | `(mod "mp" {limit 50})`, `(mod not "mp" {limit 200})` | 전역 게임 모드에 따라 넣거나 뺀다. `"mp"`는 멀티플레이에서만, `not "mp"`는 그 밖에서만(개발사 문서: Men of War II의 [mod 명령](https://docs.bestway.com.ua/foundational-knowledge/basic-knowledge-about-configuration-files/mod-instruction.md). 문서는 `not_mp`로 적는다). 바닐라에는 `"coop"`, `"hardcore"`, `"gameclass:..."`, `"gametype:..."`도 있다(뜻 미확인) |
| 원형 상속 | `{from "pattern rifle" ...}` | 이름의 낱말로 `set/stuff` 전체에서 찾은 파일(`rifle.pattern`)에서 물려받는다. 맞는 파일이 여럿이면 가까운 조상 폴더의 것으로 본다(추정, 아래) |
| 숫자 행 | `{100 10}` | 표의 한 줄 |
| `##` 접두 | `{##damage blastwave ...}` | 에디터가 꺼 둔 블록. 읽을 때 지우지 않지만 게임은 그 값을 쓰지 않는다(개발사 문서: Men of War II의 [설정 파일 기본](https://docs.bestway.com.ua/foundational-knowledge/basic-knowledge-about-configuration-files.md)). 바닐라 99개 파일에 189곳이고, 같은 항목의 옛 값·새 값 쌍은 없다: 매크로가 대신하는 손 계산 값(`he_level_N` 옆의 `##damage` 30곳), 쓰지 않는 장비(`commander_vision` 51곳 중 47곳) 등이다 |

### 암묵 규칙

- `set/stuff/<폴더>/.presets`는 그 폴더의 파일에서 include 없이 쓸 수 있다. 하위 폴더의 파일은 쓰지 못한다(2026-10-16 게임 확인: "define not found"). 이 규칙으로 펼쳤을 때 아이템 2,720개와 원형 64개에서 풀리지 않은 매크로가 0개였다.
- 0바이트 파일도 include 대상이다. `properties/shadow/*.inc` 20개가 비어 있고 27개 엔티티가 이를 include한다.

### 경로 표기

| 표기 | 뜻 |
|---|---|
| `/breed/inventory/item` | 블록 이름을 소문자로 이은 것 |
| `"*"` | 이름이 따옴표 문자열인 블록. 이름 자체는 데이터로 보고 인자에 남겼다. 예: `{"rifle" lvl_5}` → 경로 `/breed/perks/"*"`, 값 `"rifle" lvl_5` |
| `#row` | 숫자로 시작하는 블록(표의 한 줄) |
| `extender[cannon]` | `{extender "cannon" ...}`. 종류마다 하위 키가 달라 종류를 경로에 넣었다 |
| `(include)`, `(mod)` | 괄호 구문 |
| `("fire rate")` | 매크로 호출 |
| `(define "locomotion")/maxspeed` | 매크로 정의 안의 키 |
| `("crew_human")/off()/sensor` | 매크로 호출의 인자 `off(...)` 안에 든 블록 |
| `<bare>` | 블록 없이 놓인 낱말 |
| `{}` | 이름 없는 블록 |

## 데이터 파일 안내

모두 `data/` 아래에 있고 스크립트가 생성한다.

| 파일 | 내용 |
|---|---|
| `file-index.csv` | `resource` 아래 pak 44개의 모든 파일 350,397개: 가상 경로, 출처 pak, 크기, 그룹, 상태, SHA-1. `localizations\default.pak`은 들어 있지 않다 |
| `pak-summary.csv` | pak별 크기와 파일 수 |
| `parse-results.csv` | 파일별 왕복 검사와 글자 수 검사 결과, 토큰 수 |
| `keys-<그룹>.csv` | 원본 키 목록. `schema-*.md`의 앞부분이 여기서 나온다 |
| `keys-resolved.csv` | 펼친 뒤의 키 목록 |
| `resolve-summary.csv` | 유닛 파일별 펼친 행 수와 풀지 못한 구문 |
| `units-<그룹>.jsonl.gz` | 파일별 원본 키·값 전체. 한 줄에 파일 하나 |
| `units-<그룹>.resolved.jsonl.gz` | 유닛 파일별 펼친 키·값 전체 |
| `display-names.csv` | 현지화 키와 영문 표시 이름 22,033개 |
| `references.csv.gz` | 파일 사이의 이름 참조 212,557개 |
| `annotations/common.psv` | 사람이 쓴 키 의미. 유일하게 손으로 고치는 파일 |

유닛 하나의 모든 값은 `units-*.resolved.jsonl.gz`에 있다. 한 줄의 구조:

```json
{"Id":"entity/-vehicle/ussr/tank_heavy/kv85/kv85.def","Kind":"vehicle",
 "Resolved":[["/game_entity/mass","block","46000"], ["/game_entity/chassis/locomotion/maxspeed","block","40"], ...],
 "Unresolved":[],"Includes":["properties/tank.ext", ...],"Macros":["abm_dymamic", ...],"Presets":[],"Inherited":[]}
```

`Resolved`의 각 항목은 `[경로, 형태, 값]`이다. PowerShell로 한 유닛을 꺼내는 예:

```powershell
$gz = [IO.Compression.GZipStream]::new([IO.File]::OpenRead('data\units-vehicle.resolved.jsonl.gz'), [IO.Compression.CompressionMode]::Decompress)
$sr = [IO.StreamReader]::new($gz)
while ($null -ne ($line = $sr.ReadLine())) {
    if ($line.Contains('"Id":"entity/-vehicle/ussr/tank_heavy/kv85/kv85.def"')) { (ConvertFrom-Json $line).Resolved | ForEach-Object { $_ -join '  ' }; break }
}
$sr.Dispose()
```

## 표시 이름

`display-names.csv`의 `Context`가 유닛을 가리키는 키, `Text`가 영문 이름이다.

| 대상 | 키 형식 | 연결된 수 |
|---|---|---|
| 보병 | `desc/human/<set/breed 아래 경로, 확장자 제외>` | 5,083 중 5,058 |
| 차량류 | `desc/entity/<.def 파일명>` | 2,443 중 822 |
| 아이템 | `desc/stuff/...`, `desc/weapon/...`, `desc/ammo/...` | 이 문서를 쓸 때는 연결 규칙을 확인하지 않았다. 이후 파일 이름의 낱말과 파일 종류로 이름공간 순서를 정하는 규칙을 만들어, 2026-10-14 데이터로 무기·탄약 95%에 이름이 붙는다(`docs/07-stat-editor/README.md`의 "이름") |

차량류의 연결 수가 적은 것은 파괴 모델용 `.def`와 플레이어에게 이름이 보이지 않는 엔티티가 많기 때문으로 보인다. 아이템 엔티티는 `desc/entity`로는 하나도 연결되지 않는다. 아이템의 이름은 `desc/stuff`(1,624개), `desc/weapon`(547개), `desc/ammo`(565개)에 있으며 키가 `set/stuff`의 파일명과 어떻게 대응하는지는 조사하지 않았다.

## 편제와 비용

| 위치 | 내용 |
|---|---|
| `set/multiplayer/units/early`, `mid`, `late` | 시기별 멀티플레이 유닛 목록. 진영별 파일 |
| `set/multiplayer/units/conquest` | 컨퀘스트용 유닛 목록. `inf_<진영>.set`은 보병의 CP와 비용을 정하며 파일 머리에 가격 기준표가 주석으로 있다 |
| `set/multiplayer/units/roster_*.set`, `only_roster_*.set` | 편성표 |
| `set/multiplayer/games/presets` | 게임 모드 프리셋 35개 |
| `set/dynamic_campaign/unit_research_<진영>.set` | 동적 캠페인의 연구 트리 |
| `set/dynamic_campaign/values.set`, `resources_*.set`, `duration_*.set` | 캠페인 수치 |
| `script/multiplayer/units/unit_roster_readme.md` | 편성표 형식에 대한 개발자 문서 |

이 파일들은 유닛을 따옴표 이름 블록(`{"이름" ...}`)이나 매크로 호출(`("squad_with1types" ...)`, `("towed_gun_doctrine" ...)`)로 가리킨다. 참조 색인에서 `set/multiplayer`는 엔티티 이름을 1,700회 넘게, `set/dynamic_campaign`은 600회 넘게 가리킨다. 키별 내용은 [schema-shared.md](schema-shared.md)에 있다.

## 한계

- **펼치기 규칙은 추론이다.** include, 매크로, `.presets`의 처리 방식을 파일에서 추론해 구현했다. 유닛 파일 16,230개 중 풀리지 않은 구문이 남은 것은 16개이고 모두 채워지지 않은 매개변수(`%door2` 등)다. include와 매크로 이름은 전부 풀렸다. 이는 규칙이 맞다는 근거지만 엔진과 같은 결과라는 증명은 아니다.
- **이 문서는 원형 상속을 합치지 않았다.** `{from "pattern rifle" ...}`의 유닛에는 그 파일에 적힌 값만 있다. 원형의 값은 `.pattern` 파일(종류 `pattern`)에 따로 있다. 게임에서는 자식 파일의 같은 키가 이기고, 자식에 없는 키는 원형의 것을 쓴다(2026-10-16, `filling` 9인 부모를 `filling` 3인 자식이 물려받자 3/3으로 보였다). 편집기는 합쳐 보인다(E11, [docs/07-stat-editor/README.md](../07-stat-editor/README.md)).
- **`{from}` 이름을 파일로 잇는 규칙은 추정이다.** 아이템 이름과 같이 파일 이름을 점으로 나눈 낱말로 찾는다: 파일 이름과 같거나(`k98`), 낱말이 같거나(`pattern rifle` → `rifle.pattern`, `knife pattern` → `knife.pattern`), 확장자를 빼면 같다(`bazooka_m1` → `bazooka_m1.weapon`). 폴더로 좁히지 않는다: 모드는 다른 폴더의 원형을 가리킨다(MACE `body/cap1_mengjiang` → `head/light_headgear.pattern`). 바닐라 2,565개와 켜 둔 모드까지 3,761개가 모두 파일로 이어졌다. 맞는 파일이 둘인 이름은 `pattern mgun`(MACE `hmg/mgun.pattern`과 바닐라 `mgun/mgun.pattern`)과 `air_bomb`(같은 폴더의 `.ammo`와 `.pattern`)뿐이다. 편집기는 가까운 조상 폴더의 것, 그다음 확장자가 같은 것을 고른다. 게임이 고르는 파일은 미확인이다(`docs/10-backlog.md` G18).
- **`(mod ...)` 조건은 풀지 않았다.** 조건 안의 블록은 부모 경로에 그대로 실려 있다. 같은 키가 조건별로 두 번 나올 수 있다. 편집기는 그런 줄에 조건을 보인다("조건: 멀티플레이에서만" 등, 2026-10-16).
- **매크로 정의의 순서.** 정의가 쓰인 뒤에 나오는 경우는 풀지 못한다. 실제로는 발생하지 않았다.
- **매크로 본문은 정의할 때 묶여 있던 값과 그 호출의 인자로만 펼친다.** 매크로 안에서 부른 매크로는 바깥 호출의 인자를 받지 못한다(2026-10-16 게임 확인: 채워지지 않은 `%n` 때문에 "Block terminator expected"로 꺼졌다). 정의할 때 묶인 include 인자를 쓰는 것은 추정이고, include가 바깥 인자를 물려주는 것도 확인하지 않았다. 카탈로그의 `Inherited`는 그 호출에 직접 주어지지 않은 값을 쓴 곳이다. 바닐라 유닛 16,230개에는 없고, 유닛 범위 밖 `set/multiplayer/games`에 있다.
- **이름 기반 참조에는 잘못 이어진 것이 있다.** `references.csv.gz`는 인자 문자열이 다른 파일의 이름과 같으면 참조로 기록한다. `"stuff"`, `"rifle"`, `"body"`처럼 흔한 낱말은 실제 참조가 아닐 수 있다. `MatchType`이 `whole`인 것과 위의 참조 사슬에 나온 키를 우선 믿는다.
- **모델·재질 파일.** `.mdl`, `.mtl`은 키 목록과 카탈로그에는 있지만 참조 색인에서는 뺐다.
- **값을 바꿔 가며 검증하지 않았다.** 이 문서의 어떤 내용도 게임에서 실험한 결과가 아니다. 이후 게임에서 확인한 것(같은 경로는 나중 모드가 이김, 낱개 파일이 pak을 이김, `{from}` 상속이 모드 수정을 받음, include 사본, 매크로 인자 등)은 `docs/07-stat-editor/ingame-check.md`에 있다.

## 다시 생성하는 법

게임이 업데이트되면 저장소 루트에서 다음을 순서대로 실행한다. 전체에 약 6분이 걸린다.

```powershell
pwsh -NoProfile -File tools/Invoke-Pipeline.ps1 -SourceRoot "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\resource" -WorkDir _work\vanilla -DataDir data
pwsh -NoProfile -File tools/Build-DisplayNames.ps1 -LocPak "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\localizations\default.pak" -DataDir data
pwsh -NoProfile -File tools/New-SchemaDoc.ps1 -DataDir data -AnnotationDir data\annotations -OutDir docs\04-units
pwsh -NoProfile -File tools/Test-Coverage.ps1 -DataDir data -DocsDir docs\04-units
```

마지막 명령이 `keys-missing=0`을 출력해야 한다.
