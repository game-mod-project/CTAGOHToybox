# 모드 개발 리서치

## 조사 범위와 날짜

조사 날짜는 2026-10-11이다. 게임 버전 1.0.6.7 기준으로 디스크의 파일과 대조했다. 문장 끝의 `[번호]`는 맨 아래 출처 목록의 번호다.

| 범주 | 확인한 것 | 결과 |
|---|---|---|
| 게임에 든 문서 | 템플릿 readme, 편성표 설명서, 게임 모드 템플릿, 설정 파일의 주석 | 읽음 [1][2][3] |
| 공식 | Steam 토론의 개발자 답변, 같은 엔진(GEM)의 공식 문서 사이트 | 읽음 [4][5] |
| Steam 창작마당·가이드 | 모딩 가이드 3건, 대상 모드 4개의 창작마당 페이지 | 읽음 [6][7][8][9][10][11][12] |
| 모드 사이트 | Nexus Mods, ModDB | 페이지 존재만 확인. 내용은 읽지 못함 [13][14] |

### 접근하지 못했거나 찾지 못한 것

- **Gates of Hell 전용 공식 모딩 위키나 문서 사이트.** 두 번 검색했지만 나오지 않았다. 없다고 단정할 수는 없다. 가이드 작성자들은 공식 Discord의 모딩 채널을 주된 문의처로 든다 [6]. Discord는 읽지 못했다.
- **에디터 PDF 2종** (`mods\tools\map_editor(eng).pdf` 12쪽, `mission_editor(eng).pdf` 37쪽). 2026-10-16에 읽었다(R5, 아래 "에디터 설명서").
- **ModDB.** GEM 2 엔진의 튜토리얼 목록 페이지가 검색에는 나오지만 접근이 거부됐다(HTTP 403) [14]. 게임 이름으로 추정한 주소에는 페이지가 없었다.
- **Nexus Mods.** Call to Arms 항목 아래에 Gates of Hell용 모드 페이지가 있는 것은 확인했다 [13]. 쿠키 동의 화면에 가려 본문은 읽지 않았다.
- **Steam 가이드 목록의 나머지.** "modding"으로 찾은 가이드 54건 중 첫 30건의 제목만 보았고 그중 3건을 읽었다.

웹 출처는 대부분 커뮤니티 가이드다. 개발사가 직접 쓴 것은 템플릿 readme와 게임 파일의 주석, 그리고 토론 글의 개발자 답변 하나뿐이다.

## 모드의 종류와 위치

| 종류 | 위치 | 관리 |
|---|---|---|
| 창작마당 모드 | `<Steam 라이브러리>\steamapps\workshop\content\400750\<창작마당 ID>` | Steam이 받고 갱신하고 지운다. 내용을 고치면 자동 갱신과 삭제가 되지 않는다 [1] |
| 로컬 모드 | `<게임 폴더>\mods\<이름>` | 직접 넣고 갱신하고 지운다 [1] |

- 개발자 답변: 창작마당이 아닌 모드는 게임의 `mods` 폴더에 넣으면 로컬로 로드된다 [4].
- 모드 폴더는 `mods` 바로 아래에 있어야 하고 그 안에 `mod.info`가 있어야 한다. 한 단계 더 깊이 풀면 목록에 나타나지 않는다 [4].
- 폴더명에 공백이 있으면 게임 안의 모드 관리 화면이 받아들이지 않는다는 사용자 보고가 있다 [4]. readme의 규칙 `[a-zA-Z_]+`와 맞는다 [1].
- 켠 모드는 프로필의 `options.set`에 `{mods ...}` 목록으로 기록된다 [7]. 디스크에서 확인한 형식은 `"mod_<창작마당 ID>:0"`이다.

## mod.info

템플릿과 readme에 나온 필드 [1]:

| 필드 | 설명 |
|---|---|
| `name` | 모드 이름. 폴더명과 같게 한다. 라틴 문자, 숫자, `_`, `-`, 공백만 쓰고 120자 이하. 대소문자 구분 없음 |
| `tags` | 태그 목록. 선택 |
| `desc` | 설명 |
| `minGameVersion` | 지원하는 가장 오래된 게임 버전 |
| `maxGameVersion` | 지원하는 가장 최신 게임 버전. `minGameVersion` 이상이어야 한다 |
| `require` | 의존하는 모드 이름. 선택. 템플릿에 주석 처리된 예시가 있다 |

### 버전 필드

출처마다 말이 다르다.

| 출처 | 내용 |
|---|---|
| 템플릿 readme [1] | 두 필드 모두 비어 있으면 안 되고 `X.YYY` 형식이어야 한다. 아니면 호환되지 않는 모드로 표시된다. `minGameVersion`은 현재 버전으로, `maxGameVersion`은 몇 버전 앞으로 두라고 권한다. 단순한 모드는 4버전 뒤까지, 내부 변경에 민감한 모드는 같은 값으로 |
| Steam 토론의 사용자 [4] | 두 필드에 `"any"`를 쓸 수 있다. 다만 제작자가 맞는 버전을 적는 편이 낫다 |
| 디스크의 실제 모드 | `mods\SpawnMenu_MACE_Fusion_Ground_Patch\mod.info`가 `"any"`를 쓰고 2026-07에 로드되어 동작했다 |
| 검색에 나온 사용자 글 | 게임이 업데이트되면 모드가 "out of date"로 표시되고, `mod.info`의 버전 값을 현재 버전으로 고쳐 다시 켜는 사람들이 있다 |

정리하면 readme의 형식 규칙은 실제보다 엄격하게 적혀 있고 `"any"`가 통한다는 정황이 둘 있다. 이 저장소의 모드는 `"any"`를 쓴다. 근거와 남은 의문은 `docs/03-mod-folder-setup.md`에 있다.

## 리소스 덮어쓰기

### 확인된 것

- 모드의 `resource` 폴더는 게임의 `resource`와 같은 구조를 쓴다. 바닐라 pak에서 파일을 꺼내 모드의 같은 상대 경로에 두고 고치는 것이 가이드들의 공통된 방법이다 [6]. 예: `gamelogic.pak`의 `set/breed/mp/ger/early/nco.set` → `mods\<이름>\resource\set\breed\mp\ger\early\nco.set`.
- 모드는 pak으로 묶지 않은 낱개 파일을 써도 된다 [6]. 대상 모드 4개 중 MACE는 pak을, 로컬 패치 모드는 낱개 파일과 pak을 함께 썼다(2026-07 게임 로그의 마운트 목록).
- 게임은 바닐라 pak, 현지화 pak, 켠 모드의 pak 순서로 마운트한다. 모드는 `options.set`의 목록 순서를 따른다(2026-07 게임 로그. `docs/01-game-structure.md`).
- 현지화는 모드의 `localizations\<언어 코드>\` 아래에 두면 엔진의 가상 파일 시스템에 합쳐진다 [1].

### 추정

- **같은 경로의 파일은 나중에 로드된 쪽이 이긴다.** 2026-10-13에 게임에서 확인해 더는 추정이 아니다(`07-stat-editor/ingame-check.md`): 로컬 모드의 낱개 파일이 바닐라 pak의 파일을 이겼고, `{mods}` 목록 맨 뒤에 둔 모드의 낱개 파일이 앞 모드의 pak 안 파일을 이겼으며, 맨 앞에 뒀을 때는 졌다. pak끼리 겹친 경우는 따로 보지 않았다. 직접 그렇게 적은 출처는 찾지 못했다. 간접 근거: 가이드가 "기반 모드를 먼저, 그것을 고치는 모드를 나중에" 두라고 하고 [7], 창작마당의 하위 모드들이 "MACE 다음에 이 모드"처럼 로드 순서를 지정한다 [9]. 이 모든 것이 뒤의 것이 앞의 것을 덮어쓴다는 전제와 맞는다.
- **같은 모드 안에서 낱개 파일과 pak이 겹치면 낱개 파일이 쓰인다.** 2026-10-13에 게임에서 확인했다: 한 모드에 pak 안의 값 11과 낱개 파일의 값 7을 함께 두자 7이 나왔다. 같은 날 pak끼리도 확인했다: 목록 맨 뒤 모드의 pak에만 둔 값 9가 MACE pak의 18을 이겼다(`07-stat-editor/ingame-check.md`의 "일괄 확인"). 출처는 여전히 찾지 못했다.

### 로드 순서를 정하는 법

게임 안에서는 모드 순서를 직접 정할 수 없고 켜는 순서대로도 되지 않는다는 보고가 있다. 프로필의 `options.set`에서 `{mods ...}` 목록의 줄 순서를 직접 고치는 방법이 알려져 있다 [7]. 같은 가이드는 ID 뒤의 숫자(`:2097245` 등)를 `:0`으로 바꾸니 오류가 없어졌다고 적었는데, 그 숫자가 무엇인지는 설명하지 않는다 [7]. 2026-10-16 확인(R6): 이 PC의 `options.set`은 다섯 항목 모두 `:0`이고, 창작마당 목록 파일(`appworkshop_400750.acf`)의 크기·갱신 시각·매니페스트 값과도 맞는 것이 없다. 게임 실행 파일에서 이 꼴을 만드는 문자열도 찾지 못했다. 뜻은 미확인이다.

## 아이템 이름과 파일

유닛은 무기·탄약·장비를 이름으로 가리킨다: `{item "m3_grease" filled}`, `{weapon "85mm_d5t" filling "bulletrus_85 aphebc" 1}`, `{item "m3_grease_mag ammo" 90}`. 이 이름이 `set/stuff` 아래 어느 파일로 이어지는지를 2026-10-14에 데이터로 셌다(`tools/research/Measure-ItemNameLinks.ps1`. 원본은 바닐라와 켜 둔 모드 4개, 차량과 보병 파일의 `item`·`weapon`의 첫 문자열과 `filling` 뒤 문자열).

| 규칙 | 이름 수 |
|---|---|
| 파일 이름과 같다 (`m3_grease` → `set/stuff/smg/m3_grease`) | 1,047 |
| 이름의 낱말과 파일 이름을 점으로 나눈 낱말이 같다. 순서는 상관없다 (`m3_grease_mag ammo`, `ammo m3_grease_mag` → `m3_grease_mag.ammo`) | 683 |
| 파일 이름의 마지막 조각(확장자)을 빼면 낱말이 같다 (`bulletrus_85 aphebc` → `bulletrus_85.aphebc.ammo`) | 611 |
| 위 규칙으로 파일이 둘 이상 | 14 |
| 위 규칙으로 맞는 파일이 없음 | 56 |
| 합계 (서로 다른 이름) | 2,411 |

- 앞의 세 규칙으로 하나의 파일에 이어지는 이름이 2,341개(97%)다.
- 맞지 않은 56개 가운데 46개는 이름의 낱말이 파일 낱말의 일부인 파일이 있다. 예: `ammo mgun_usa` → `mgun_usa.belt.ammo` 하나, `ammo mgun_ger` → `mgun_ger.*.ammo` 10개, `bazooka` → `bazooka.*.ammo` 4개. 탄약 "계열"을 가리키는 이름이다. 게임은 이름의 낱말을 모두 가진 파일 가운데 **파일 이름순(ASCII, 점이 글자보다 앞) 맨 앞 파일**을 고른다(2026-10-16 게임 확인. 같은 폴더의 후보로만 봤다).
- 나머지 10개(`mgun`, `mgun1`, `kordr`, `grenade` 계열 일부 등)는 이름이 아니라 무기 자리나 다른 것을 가리키는 것으로 보인다. **미확인**.
- 게임에서 확인한 것: `m3_grease`(첫 규칙. `docs/07-stat-editor/ingame-check.md`의 "일괄 확인" 4번), `ammo mgun_ger belt` → `mgun_ger.belt.ammo`(둘째 규칙, Panzer IV G), 계열 이름 `ammo mgun_ger` → `mgun_ger.belt.ammo`(핀란드 Maxim MG 08. 바닐라 후보 9개 가운데 이름순 맨 앞). 2026-10-16 W-G에서 후보 파일마다 탄 무게를 다르게 바꿔 툴팁 무게로 가렸다. 모드에 후보 사본을 더하자 `mgun_ger.zz.ammo`는 뽑히지 않고, `mgun_ger.aaa.ammo`, 이어서 낱말이 더 많은 `mgun_ger.aa.zz.ammo`가 뽑혔다. 그러므로 낱말 수나 계층이 아니라 이름순이다(`docs/07-stat-editor/ingame-check.md`의 "W-G 셋째 묶음").

## 유닛을 추가·수정하는 절차

컨퀘스트용 보병 분대를 만드는 가이드 [6]의 절차를 정리하고, 각 단계가 다루는 파일을 이 저장소의 문서와 이었다. 가이드에 나온 파일 경로와 매크로 이름은 모두 디스크에 있는 것을 확인했다.

| 단계 | 하는 일 | 파일 | 이 저장소의 문서 |
|---|---|---|---|
| 1 | 모드 폴더, `resource`, `localizations`, `mod.info`를 만든다 | `mods\<이름>\` | `03-mod-folder-setup.md` |
| 2 | 바닐라 보병을 복사해 고친다. `skin`, `armors`, `perks`, `inventory` | `set/breed/mp/<진영>/<시기>/<이름>.set` | `04-units/schema-human.md` |
| 3 | (새 외형이 필요하면) 외형 엔티티와 모델을 만든다 | `entity/humanskin/[폴더]/...` `.def`, `.mdl`, `.mtl` | `04-units/schema-human.md` |
| 4 | 보병의 등급과 비용을 등록한다 | `set/multiplayer/units/conquest/inf_<진영>.set` | `04-units/schema-shared.md` |
| 5 | 분대를 정의한다 | `set/multiplayer/units/conquest/units_<진영>.set` | `04-units/schema-shared.md` |
| 6 | 연구 트리에 넣는다 | `set/dynamic_campaign/unit_research_<진영>.set` | `04-units/schema-shared.md` |
| 7 | 에디터에서 유닛을 확인하고, 컨퀘스트에서 구매·연구가 되는지 본다 | | |
| 8 | 표시 이름을 넣는다 | `localizations\default\interface\text\desc\*.pot` | `04-units/README.md`의 "표시 이름" |
| 9 | 분대 초상화를 넣는다 | `interface/scene/portrait_squad/<분대 이름>_00.png` ~ `_03.png` | 범위 밖 |

단계별 세부 [6]:

- **2단계.** `skin`은 외형 엔티티 이름에서 `.mdl`을 뗀 것, `portrait`는 선택했을 때 화면 왼쪽 아래의 그림, `icon`은 유닛 위의 아이콘, `armors`는 머리 장구, `veterancy`는 시작 숙련도다. `inventory`의 아이템 뒤 숫자는 첫째가 기본 수량, 둘째가 수량을 무작위로 바꾸는 계수라고 한다. 작성자 스스로 확실하지 않다고 적었다.
- **3단계.** 외형에 쓰는 메시(`.ply`)는 같은 범주 폴더의 source 폴더에 있어야 한다. 가이드는 바닐라의 `[ger_source]`를 통째로 복사한 뒤 안 쓰는 것을 지우라고 한다. `.mtl`은 `diffuse`(실제 텍스처), `bump`(노멀 맵), `specular`(광택)를 지정한다. 외형 조립에는 커뮤니티 도구를 권한다.
- **4단계.** 한 줄의 형식은 `{"mp/ger/mid/nco" ("inf_tier2" side(ger) lvl(...) r(1) rm(8) cost(10.0))}`다. 첫 문자열이 보병 파일의 경로, `inf_tier2`는 `settings_inf.set`에 정의된 등급 매크로, `side`는 진영, `cost`는 한 명의 비용이다. 10짜리 열 명이면 분대 비용이 100이다.
- **5단계.** `("squad_with3types_conquest" side(ger) period(mid) min_stage(1) max_stage(5) name(squad_x) c1(nco:1) c2(a:4) c3(b:4))`. 매크로 이름의 숫자가 보병 종류 수이고 `c1`~`cN`이 "보병 이름:인원"이다. `name`이 6단계와 8단계에서 쓰는 분대 이름이다.
- **6단계.** 분대 이름, 연구 비용, 선행 조건, 화면에서의 위치를 적는다. 끝의 두 수는 오른쪽으로 몇 칸, 아래로 몇 줄인지다.
- **8단계.** `msgctxt "desc/human/<보병 경로>"`와 `msgid "<표시 이름>"`. 분대는 `msgctxt "desc/squad/<분대 이름>(<진영>)"`. 바닐라의 `desc_breed_mp_ger.pot`, `desc_squad_ger.pot`, `desc_squad_ger_dcg_late.pot`를 복사해 고친다.
- **9단계.** 분대마다 `_00`부터 `_03`까지 네 장이 있어야 하고, 하나라도 없으면 게임이 죽는다고 한다.

차량을 추가하는 절차를 다룬 가이드는 읽지 못했다. `04-units/schema-vehicle.md`와 참조 사슬(`04-units/README.md`)이 그 출발점이다.

### 편성표의 봇 구매 정보

`script/multiplayer/units/unit_roster_readme.md`는 컨퀘스트 AI가 무엇을 살지 정하는 Lua 편성표의 태그 체계를 설명하는 개발자 문서다 [2]. 유닛마다 플랫폼, 등급, 전투 역할, 효과, 기동성 등의 태그를 달고, 등급(grade)과 등장 빈도(availability)는 따로 조절한다고 적혀 있다. 새 유닛을 AI가 쓰게 하려면 이 문서를 먼저 읽어야 한다.

## 도구

| 도구 | 용도 | 출처 |
|---|---|---|
| 7-Zip, WinRAR | pak을 열어 파일을 꺼낸다. pak은 ZIP이다 | [1][6], 디스크에서 확인 |
| Notepad++ 등 텍스트 편집기 | 설정 파일 편집 | [6] |
| 게임 내장 에디터 (`call_to_arms_ed.exe`) | 맵·미션 제작, 유닛 확인. 유닛은 에디터에서 `breed/mp/ger/early/<이름>` 경로로 찾는다 | [6] |
| 3ds Max 내보내기 플러그인 (`mods\tools\plug-ins`) | 모델 제작. 2011~2016, 2020, 2024, 2025용 | 디스크 |
| GohMdlExpert (Klusandr, GitHub) | 보병 외형(`.mdl`) 조립과 텍스처 확인. 가이드는 경로와 리깅이 틀리게 저장될 때가 있다고 적었다 | [6] |
| 이미지 편집기 (Paint.NET, GIMP 등) | 텍스처와 초상화 | [6] |
| 이 저장소의 `tools/` | 모든 정의 파일을 파싱해 키와 값을 표로 만든다 | `04-units/README.md` |

## 디버깅

| 방법 | 내용 | 출처 |
|---|---|---|
| 로그 | `%LOCALAPPDATA%\digitalmindsoft\gates of hell\log\game.log`. 마운트한 pak, 모드 적용 과정이 찍힌다 | 디스크 |
| 크래시 덤프 | 같은 폴더의 `minidumps\`. `.mdmp.log`에 예외 종류와 위치가 있다 | 디스크 |
| 모드 없이 실행 | "Safe mode (No mods)" 또는 실행 인자 `-no_mods`. 모드 초기화를 건너뛴다 | [1] |
| 설정 초기화 | "Safe mode" 또는 실행 인자 `-safe` | [1] |
| 에디터에서 먼저 확인 | 유닛을 에디터에 놓아 모델, 텍스처, 장구가 맞는지 본 뒤 게임 모드에서 시험한다 | [6] |

가이드와 댓글에 나온 증상 [6]:

- 외형의 리깅이 깨져 보이면 정상인 `.mdl`의 리깅 부분을 복사해 넣는다.
- 모델이 파랗게 나오거나 앉은 자세로 나오면 `.mtl`의 텍스처 경로나 `.mdl`을 의심한다.
- 분대 초상화 네 장 중 하나라도 이름이 틀리면 그 유닛을 눌렀을 때 죽는다.
- 분대를 미리 볼 때 `EXCEPTION_ACCESS_VIOLATION`이 난다는 보고가 있고 원인은 답이 달리지 않았다.

협동 플레이에서 같은 모드를 쓰는데도 리소스가 어긋난다는 가이드 제목이 있었으나 읽지 않았다.

## 에디터 설명서 (게임 폴더의 PDF, 2026-10-16 R5)

두 문서 모두 게임 이름이 없다. 실행 파일을 `theGame.exe`로 적고, 그림 속 글자 일부가 러시아어이며, PDF 속성은 2013-10-08 Word 2010(제목 "Редактор карт", "Описание редактора миссий")이다. Men of War 계열 에디터의 옛 설명서이고 GoH용으로 고쳐 쓴 것이 아니다. 아래는 문서의 주장이며 GoH 에디터에서 확인한 것은 따로 적는다.

- 맵 에디터는 F2로 켜고 F1로 끈다(map p.1). F3은 두 문서에 없다. 미션은 패널 오른쪽 아래 `Start (Stop)` 단추로 시작·정지한다(mission p.3). 이 저장소의 GoH 에디터 확인(2026-10-16 W-G 셋째)에서는 왼쪽 위 START였다.
- 미션 에디터는 숫자 1~9로 바꾸는 아홉 하위 편집기(보기, 엔티티, 그룹, 웨이포인트, 존, 트리거, 카메라, 엄폐, 환경)다(mission p.3~4). 목록에서 고르고 지도를 누르면 놓고, 오른쪽 끌기로 옮기고, Ctrl+오른쪽 끌기로 돌리고, Del로 지운다(mission p.7).
- 소속은 놓은 엔티티의 `player` 속성(0~7)이다. 동맹·적은 Diplomacy에서 정한다(mission p.5, 8). 승무원은 트리거 명령 `board`(자리: gunner, driver, commander, charger)로 태우고 `wait_crew`, `emit`이 있다(mission p.30~31).
- 사람의 인벤토리는 병사마다가 아니라 병종(breed) 편집기에서 고친다(Inventory Add/Remove, In hands). 차량 인벤토리를 고치는 법은 없다(mission p.8).
- 단축키: Ctrl+T 태그, Alt+I mid 번호, Alt+P 이동 경로, Alt+H 높이 표시, Alt+X 에디터 나가기, Ctrl+F mid로 찾기. 시험용 G 폭탄, Y 화염병, B 폭발, 백틱 콘솔(mission p.36~37). 보기 편집기에서 게임 속도 10배~0.1배(mission p.5).
- 맵·미션 파일의 폴더·이름·저장 형식, 모드 폴더, 모드 시험 방법은 나오지 않는다.
- 이 저장소가 GoH 에디터에서 본 것 가운데 "RESET ALL INVENTORIES" 단추, "You have to store the map before!", V16의 Alt+F 안개·F7 난이도는 문서에 없다.

## 창작마당 게시

이 저장소에서는 게시하지 않는다. 참고로 확인한 것만 적는다.

- 패치 파일(`1.pat`, `2.pat`)을 순서대로 쌓는 방식은 창작마당 항목에는 필요 없다 [1].
- 게임 폴더의 `binaries\x64\call_to_arms_workshop.exe`는 창작마당 업로드 도구 "Digitalmindsoft Workshop Upload"(Copyright 2024)다. 실행하지 않고 실행 파일에 든 화면 문자열로 확인했다(2026-10-16 R6):
  - 주 화면: 내가 올린 항목 목록과 Create·Edit·Remove·Refresh, Steam Cloud 공간과 "Reset Cloud Storage".
  - 게시 창: Upload Directory(올릴 모드 폴더), Preview Image(JPEG, 512×512), Tags, Title, Description, Visibility, Steam Workshop Contribution Agreement 동의 체크, Publish.
  - mod.info 창: Name*, Min game version*, Max game version*, Description*, Require. 파일에는 `{name}`, `{tags}`, `{desc}`, `{require}`로 쓴다. 이 PC의 창작마당 모드 다섯의 `mod.info`도 `{mod {name} {tags} {desc} {minGameVersion} {maxGameVersion}}` 꼴이다.
  - Steam API로 항목을 만들고(`CreateItem`) 갱신한다(`SubmitItemUpdate`). 처음 올린 항목의 공개 범위 기본값은 문자열로는 알 수 없다(미확인).
- 같은 엔진 계열인 Men of War II의 공식 문서는 게임 설정의 패키지 화면에서 게시 버튼으로 올리고, 새로 올린 항목은 기본적으로 숨김 상태라고 설명한다 [5]. Gates of Hell의 절차가 같은지는 확인하지 못했다.
- 다른 모드의 모델이나 자산을 쓰려면 원작자의 허락을 구하는 것이 관행이다. MACE는 설명에서 이를 명시적으로 요구한다 [8].

## 웹 자료와 디스크의 차이

| 웹의 주장 | 출처 | 디스크에서 본 것 | 판정 |
|---|---|---|---|
| `options.set`은 `C:\Users\<이름>\Documents\My Games\gates of hell\profiles\<번호>`에 있다 | [7] | 이 PC에서는 `%LOCALAPPDATA%\digitalmindsoft\gates of hell\profiles\<번호>`에 있고 `Documents\My Games\gates of hell`은 없다 | 불일치. 가이드는 2025년 것이고 위치가 바뀌었을 수 있다 |
| `mod.info`의 버전 필드는 `X.YYY` 형식이어야 한다 | [1] | `"any"`를 쓴 로컬 모드가 로드되어 동작했다 | 불일치 |
| 버전 필드 이름이 `max_version` | 검색 요약에 나온 사용자 댓글 | 템플릿과 실제 모드 모두 `maxGameVersion` | 불일치. 댓글의 표기가 부정확한 것으로 보인다 |
| 컨퀘스트 파일 `settings.set`, `settings_inf.set`, `inf_ger.set`, `units_ger.set`이 `set/multiplayer/units/conquest`에 있다 | [6] | 네 파일 모두 있다 | 일치 |
| 보병 `nco.set`, `rifleman_1.set`, `mgun_1.set`이 `set/breed/mp/ger/early`에 있다 | [6] | 세 파일 모두 있다 | 일치 |
| `inf_tier2`는 `settings_inf.set`에 정의된 등급이다 | [6] | `settings_inf.set`에 `(define "inf_tier2" ...)`가 있다 | 일치 |
| `squad_with3types_conquest` 매크로로 분대를 만든다 | [6] | `conquest/settings.set`에 정의가 있고 진영 파일 5개가 쓴다 | 일치 |
| 연구 트리는 `set/dynamic_campaign/unit_research_<진영>.set` | [6] | 진영 5개의 파일이 있다 | 일치 |
| 표시 이름은 `desc_breed_mp_ger.pot` 등 `.pot` 파일의 `msgctxt "desc/human/..."` | [6] | 세 파일이 `localizations\default.pak`에 있고 형식이 같다. 보병 5,083개 중 5,058개가 이 규칙으로 이름과 이어진다 | 일치 |
| 분대 초상화는 `_00`~`_03` 네 장 | [6] | `interface/scene/portrait_squad`의 파일 5,052개 중 1,412개가 `_00`~`_03.png`로 끝난다 | 일치하는 예가 있음. 네 장이 필수인지는 디스크로 확인 불가 |
| 아이템 뒤의 두 수는 기본 수량과 무작위 계수 | [6] | 값 형태가 `[str num num]`인 `item`이 17,805번 나온다. 뜻은 파일만으로 알 수 없다 | 디스크로 확인 불가 |
| 모드 폴더는 `mods` 바로 아래, `name`은 폴더명과 같게 | [4] | readme의 규칙과 같다 | 일치 |

## 대상 모드 4개의 창작마당 설명

파일 분석은 `docs/05-workshop-mods`에 있다. 여기에는 제작자가 페이지에 적은 내용만 요약한다.

| ID | 이름 | 제작자 | 페이지의 설명 요약 |
|---|---|---|---|
| 2905667604 | MACE | Andy02_ITA | "Multiplayer and Conquest Expanded". 콘텐츠를 늘려 게임플레이를 다양화하는 모드. DLC 필요. 댓글을 닫고 Discord로 피드백을 받는다. 자산을 쓰려면 허락을 구하라고 요구한다 [8] |
| 3732639687 | MACE Wunderwaffe Ultimate Fusion ENG standalone Update | MON1KA | Wunderwaffe 계열 모드의 차량 중 MACE에 없는 것을 MACE에 넣고 컨퀘스트 연구 트리에 배치한다. 독일, 소련·중국, 미국, 영국, 일본의 시제·가상 전차 목록이 길게 있다. 로드 순서는 MACE → (중국어 번역) → 이 모드. "standalone"은 Wunderwaffe 없이 동작한다는 뜻이며 MACE는 필요하다(댓글의 설명). DLC 필요 [9] |
| 3295913567 | Spawn Menu | Rosemi | 미션 중에 보병, 차량, 전차, 항공기를 스폰하는 메뉴. AI 전환, 위장 전환, 유닛 삭제, 잔해 정리 기능. 바닐라 미션과 게임 모드에 맞춘 것이며 다른 모드의 유닛을 쓰려면 그 모드에 맞춘 판이 필요하다고 적혀 있다. DLC 필요 [10] |
| 2829744930 | Auto Medic-AA-Artillery & Weather Switch | Nikral晓杉同学 | 스크립트 기능: 자동 포격(360m 안의 적), 자동 위생병(100m 안), 대공포 강화(250m), 날씨 전환, AI 제어 버튼. 비스크립트 기능: 포가 WP·IC·SM 탄을 스스로 장전, 대공 아닌 유닛의 대공 사격, 총류탄 종류 자동 전환 등. 스크립트 기능은 공식 맵에서만 지원한다 [11] |

## 출처 목록

| 번호 | 제목 | 위치 | 확인 날짜 | 시점·버전 |
|---|---|---|---|---|
| 1 | 템플릿 모드의 `readme.txt`, `mod.info` | `<게임 폴더>\mods\template\` | 2026-10-11 | 게임 1.0.6.7에 포함. 예시 버전은 1.500, 1.510 |
| 2 | Unit Roster Schema Notes | `gamelogic.pak`의 `script/multiplayer/units/unit_roster_readme.md` | 2026-10-11 | 게임 1.0.6.7에 포함 |
| 3 | 컨퀘스트 설정 파일의 머리 주석 | `gamelogic.pak`의 `set/multiplayer/units/conquest/inf_<진영>.set` | 2026-10-11 | 게임 1.0.6.7에 포함 |
| 4 | Add mod not from steam? (Steam 토론, 개발자 Tele 답변 포함) | https://steamcommunity.com/app/400750/discussions/0/599645602069875221/ | 2026-10-11 | 2025-02-25 ~ 26, 이후 댓글 |
| 5 | Steam Workshop and Publishing your Mods (GEM 엔진 공식 문서, Men of War II 예시) | https://docs.bestway.com.ua/mod-development/modding-guides/steam-workshop-and-publishing-your-mods | 2026-10-11 | 날짜 표기 없음. 다른 게임 |
| 6 | Gates of Hell - Conquest Modding guide by Schwienyy (Steam 가이드) | https://steamcommunity.com/sharedfiles/filedetails/?id=3629209372 | 2026-10-11 | 2025-12-23 게시, 2025-12-24 수정 |
| 7 | GOH Mod Order Setting Guide (Steam 가이드, Denis) | https://steamcommunity.com/sharedfiles/filedetails/?id=3472176545 | 2026-10-11 | 게시일은 읽지 못함. 댓글이 2025-12 |
| 8 | MACE (창작마당) | https://steamcommunity.com/sharedfiles/filedetails/?id=2905667604 | 2026-10-11 | |
| 9 | MACE Wunderwaffe Ultimate Fusion ENG standalone Update (창작마당) | https://steamcommunity.com/sharedfiles/filedetails/?id=3732639687 | 2026-10-11 | 댓글이 조사 당일까지 이어짐 |
| 10 | Spawn Menu (창작마당) | https://steamcommunity.com/sharedfiles/filedetails/?id=3295913567 | 2026-10-11 | |
| 11 | Auto Medic-AA-Artillery & Weather Switch (창작마당) | https://steamcommunity.com/sharedfiles/filedetails/?id=2829744930 | 2026-10-11 | |
| 12 | Steam 가이드 목록, 검색어 "modding" | https://steamcommunity.com/app/400750/guides/?searchText=modding | 2026-10-11 | 54건 중 30건의 제목만 봄 |
| 13 | Jump (Gates of Hell Ostfront), Call to Arms Nexus | https://www.nexusmods.com/calltoarms/mods/4 | 2026-10-11 | 본문은 읽지 못함 |
| 14 | GEM 2 엔진 튜토리얼 목록 (ModDB) | https://www.moddb.com/engines/gem-2/tutorials | 2026-10-11 | 접근 거부(403). 검색 결과의 제목만 확인 |

읽을 만하지만 이번에 읽지 않은 Steam 가이드: "Guide to Gate of Hell Mod Manager"(id 3488308840), "CtA Gates of Hell: Ostfront Troubleshooting"(id 2515262536), "Editing your conquest save file"(id 3133824064), "How to restrict units for player/AI in dynamic campaign"(id 3187234227), "Changing Conquest Tweaks to your own preference"(id 3526306178).
