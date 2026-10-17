# 한글화 설계

2026-10-17. 상태: 설계(구현 전). 대화에서 정한 것을 적는다. **사용자 결정**은 그렇게 표시했고, 나머지는 AI 판단(표시: AI 판단)이다. 조사는 같은 날 실제 파일(게임 폴더, 창작마당 폴더, 게임 로그)과 웹으로 했다. 확인하지 못한 것은 `미확인`으로 둔다. 게임을 띄우는 확인은 하지도 제안하지도 않는다(사용자 결정 2026-10-16).

## 왜

사용자는 게임 안의 글을 한국어로 읽고 싶다. 공식 한국어는 없다. 범위는 **전부**다(사용자 결정): 캠페인 미션 설명, 캠페인 대사·목표, 유닛 설명, 메뉴·UI, 팁 등 게임이 보여 주는 글 모두. 켜 둔 모드(MACE, MACE Wunderwaffe, Auto Medic, Spawn Menu of MACE)가 내는 글도 포함한다(AI 판단, 사용자가 정리를 보고 고치지 않음).

## 목표

- 바닐라 현지화 `.pot` 362개(문자열 22,424개)와 켜 둔 모드 4개의 `.pot`(약 6,080개)를 한국어로 바꾼다.
- 한글이 화면에 나오게 글꼴을 맞춘다.
- 결과는 출력 모드 `CTAToybox` 안에 함께 넣는다(사용자 결정). 별도 모드를 만들지 않는다.
- 편집기는 이름을 `원문 (한글)`로 보여 준다(사용자 결정: 둘을 함께. 순서는 AI 판단).
- 게임 업데이트로 원문이 바뀌면 바뀐 줄만 다시 번역할 수 있다.

## 하지 않는 것

- 음성 더빙, 그림(텍스처) 속 글자.
- `.pot` 밖에 직접 박힌 글(사용자 결정 2026-10-17). 구현 중에 맵·미션 파일(`map/*.pak`)을 훑어 박힌 영어 글이 있으면 목록을 사용자에게 보이고 따로 다룬다.
- 모드의 `ch`(중국어) 폴더.
- Steam 언어를 한국어로 바꿔 쓰는 방식(아래 "조사"의 언어 선택. 한국어 depot이 없어 근거가 약하다).
- 게임 폴더·창작마당 폴더의 파일 수정.
- 게임 안 확인.

## 실패로 보는 것 (기술적으로 돌아가도)

- 색 태그·자리표시가 깨져 글이 엉뚱하게 보이거나 게임이 오류를 낸다.
- 전차·장비 이름이나 계급을 한국어로 옮겨 원문을 알아볼 수 없다.
- 같은 부대·장비 이름이 파일마다 다르게 번역된다.
- 편집기 빌드가 번역 파일을 지우거나, 번역 빌드가 편집기 생성물을 건드린다.

## 조사 (2026-10-17)

| 질문 | 확인한 것 | 근거 | 상태 |
|---|---|---|---|
| 언어를 고르는 법 | 게임은 Steam 언어를 따른다. 로그 첫머리에 `Steam Language: english`. `default.pak`의 `default/localization.info`는 `{code "default"} {name "English"} {steamLanguageCode "english"} {defaultChatChannel 0}`. 프로필 `options.set`에는 언어 항목이 없다 | `%LOCALAPPDATA%\digitalmindsoft\gates of hell\log\game.log` 8행, `profiles\59132039\options.set` | 확인 |
| 공식 언어 | Steam 상점: 영어, 프랑스어, 독일어, 스페인어, 러시아어, 간체 중국어, 이탈리아어, 폴란드어, 우크라이나어, 터키어. 언어별 depot(`config/language` french, italian, german, spanish, russian, schinese, ukrainian, polish)을 Steam이 따로 받는다. **한국어 depot은 없다** | Steam `appdetails?appids=400750`, steamcmd `app_info_print 400750`(`E:\CTAToybox\_work\steamcmd`) | 확인 |
| 새 언어 코드를 모드가 더할 수 있는지 | 번체 중국어 창작마당 모드(2982097092)가 있으나 구조를 보지 못했다(설명 없음, 파일 미구독) | [GoH CHT patch](https://steamcommunity.com/sharedfiles/filedetails/?id=2982097092) | 미확인 |
| 현지화 마운트 순서 | `localizations/default.pak`(바닐라) → 모드마다 `localizations/default.pak`. 로그 64·164행 | `game.log` | 확인 |
| 모드의 풀린 `localizations/default/` 폴더를 읽는지 | 읽는다. Wunderwaffe(3732639687)는 `.pak` 없이 `localizations/default/interface/text/desc/desc_ammo_eng2.pot` 등을 풀어 두었고, 로그의 key redefinition 줄이 바로 이 파일(`/interface/text/desc/desc_ammo_eng2.pot, 72`)을 가리킨다 | `game.log` 171행, 창작마당 폴더 | 확인 |
| `.pot` 밖에 박힌 글 | 압축을 푼 바닐라 `script`·`set`(유닛 범위, `_work/vanilla`)에서 따옴표 안 영어 문장을 찾지 못했다. 맵·미션 파일(`map/*.pak`)은 풀어 보지 않았다 | 검색 | 맵·미션은 미확인 |
| `.pot` 형식 | gettext, UTF-8. `msgctxt`가 키, **화면에 나오는 글은 `msgid`**. 바닐라 `msgstr`는 13개를 빼면 비어 있다. 모드의 `ch` 번역도 번역문을 `msgid`에 넣었다(`2829744930/localizations/ch/interface/text/dlg_ingame_addition.po`, 머리 `Language: zh_CN`). `msgid_plural` 없음 | `localizations/default.pak` 압축 해제 | 확인 |
| 문자열 분량 | 바닐라 22,424(`desc` 12,048, `mission` 6,201, 위 폴더 바로 아래 UI 4,097, `names` 55, `announcements` 23). 모드: MACE 4,494(`.pak` 안 52개 파일), Wunderwaffe 1,519, Spawn Menu of MACE 62, Auto Medic 7. 각 수에는 파일 머리 `msgid ""`가 들어 있다 | `grep -c '^msgid'` | 확인 |
| 형식 기호 | `<c(…)>` 4,004회, `<x(…)>` 76, `<s()e()b+c()>`·`<f()s()…>` 등 서식 태그, `%s` 39, `%i` 28, `%d` 10, `%02d` 8, 줄바꿈 `\n` | 같은 파일 | 확인 |
| 같은 경로 파일 | MACE `.pak`이 바닐라와 같은 경로의 파일 2개를 낸다: `desc/desc_squad_doctrines_eng.pot`, `tips.pot`. 다른 경로 파일이 같은 키를 다시 정의하면 로그에 `Localization key redefinition … previously declared in …`(474건, `TextLocalizationManager.cpp:312`). 어느 쪽이 쓰이는지는 로그로 알 수 없다 | `game.log` 171행~ | 경고는 확인, 이기는 쪽 미확인 |
| 같은 경로 파일을 모드가 이기는지 | `resource`에서는 뒤 모드의 파일이 이긴다(게임 확인 2026-10-13, `docs/01-game-structure.md` 258행). `localizations`도 같은 가상 파일 시스템에 합쳐진다고 알려져 있다(`docs/02-modding-research.md` 69행) | 문서 | `localizations`에서는 미확인 |
| 한글 글꼴 | `resource/interface/interface.pak`의 `font/main_*.fnt`(BMFont 바이너리 v3, 글꼴 "Arial Unicode MS")에 한글 음절 11,172자 전부와 한자 20,902자. 크기 8, 8b, 9, 9b, 10, 10b, 12, 12b, 15, 18, 21, 27, 28, 30. `main_42`에는 한글이 없다. 그 밖의 글꼴(anka, arial_hq, arial_narrow, bookman, impact, myriadpro, open_sans, oswald, pobeda, pt_sans, roboto_con, roboto_mono)에는 한글이 없다 | `.fnt`의 chars 블록을 세었다 | 확인 |
| `.font` 형식 | `{font_atlas {name "main"} {size 12} {height 21} {style 0} {binary_data "main_12.fnt"}}`. `.fnt` 안에 페이지 그림 이름(`main_12_0.tga`, `main_12_1.tga`)이 들어 있다 | `font/main_12.font` | 확인 |
| 어느 화면이 어느 글꼴을 쓰는지 | 압축을 푼 `interface.pak`의 설정·레이아웃에서 글꼴 이름을 찾지 못했다 | 검색 결과 없음 | 미확인 |
| 한국어 선례 | 한국어 모드·한글 패치를 찾지 못했다 | 웹 검색 | 없음 |

## 방법

### 1. 넣는 자리 (사용자 결정: `CTAToybox` 안에 함께)

- 번역 결과는 `mod/CTAToybox/localizations/default/interface/text/<원래 경로>.pot`에 둔다. 폴더(`localizations/.gitkeep`)가 이미 있다. 편집기 빌드는 `resource` 아래, 그중에서도 `.ctatoybox-build.json`에 적힌 파일만 지운다(`editor/CtaToybox.Core/ModBuilder.cs` 75행, `Remove`). 그래서 서로 건드리지 않는다.
- **파일 통째로 덮어쓴다(AI 판단).** 원래 파일과 같은 경로·같은 이름으로 내고, 그 안의 키를 모두 한국어로 채운다. 키만 다른 파일에 따로 넣으면 "key redefinition" 경고가 나고 어느 쪽이 이기는지 모른다. 같은 경로 파일이 이기는 규칙은 `resource`에서 확인됐고, `localizations`에서도 같다고 본다(미확인).
- 모드가 다른 이름의 파일로 같은 키를 다시 정의한 경우(예: Wunderwaffe `desc_ammo_eng2.pot`가 바닐라 `desc_ammo_eng.pot`의 키를 다시 정의)는 두 파일을 모두 번역한다. 어느 쪽이 이겨도 한국어가 나온다.
- 한 경로의 원문은 **로드 순서상 가장 뒤의 파일**로 한다. 바닐라 → `options.set`의 `{mods}` 순서. 예: `tips.pot`은 MACE의 것. 순서는 편집기의 `GameProfile`이 만드는 층(`editor/CtaToybox.Core/GameProfile.cs`)과 같게 하고, 출력 모드 자신은 원문에서 뺀다.
- 화면 글은 `msgid`에 쓴다. `msgctxt`와 파일 머리는 원문 그대로 두고, 머리의 `Language:`만 `ko`로 바꾼다(AI 판단).
- `CTAToybox`가 모드 목록 맨 뒤에 있어야 한다. 편집기는 이미 이 경고를 낸다(`GameProfile.cs` 87행).

### 2. 번역 보관

- 새 최상위 폴더 `l10n/ko/`에 둔다(AI 판단). 손으로 고칠 수 있는 입력 파일이다. `CLAUDE.md`의 폴더 표와 "손으로 고치는 파일"에 더한다.
- 형식은 표준 gettext `.po`다(AI 판단). 파일 하나가 원래 `.pot` 하나에 대응하고, 경로도 같다: `l10n/ko/<원래 경로>.po`. 항목마다 `msgctxt`(키), `msgid`(영어 원문), `msgstr`(한국어). 원문이 함께 있으니 게임이 바뀌면 바로 비교할 수 있다.
- 원문이 바뀐 항목은 `#, fuzzy`로 표시하고 예전 원문을 `#| msgid`로 남긴다(gettext 관례). 원문에서 사라진 키는 지우고, 새 키는 `msgstr ""`로 더한다.
- 출처 표시: 파일 머리에 원문을 가져온 층(바닐라·모드 ID)과 게임 버전을 적는다.

### 3. 용어집

- `l10n/ko/glossary.psv`(`원문|번역|메모`). 번역 전에 만든다.
- 규칙(사용자 결정): **전차·장비 이름과 계급은 원문 그대로**(예: `Panzer IV Ausf. H`, `Sd.Kfz. 251`, `StuG III`, `Oberleutnant`). **부대명은 소리 나는 대로 한글로**(예: `Grossdeutschland` → `그로스도이칠란트`).
- 그 밖의 이름(사용자 결정 2026-10-17, AI 제안을 받아들임):

  | 종류 | 예 | 규칙 |
  |---|---|---|
  | 독일어·러시아어 병과 이름 | Panzergrenadiere, Fallschirmjäger, Volksgrenadier, Shtrafbat | 음독(판처그레나디어, 팔쉬름예거) |
  | 영어로 된 분대 종류 | Assault Squad, Pioneers, Sniper Team | 번역(돌격 분대, 공병, 저격조) |
  | 진영·군대 이름 | Wehrmacht, Red Army, US Army | 독일어 등은 음독(베어마흐트), 영어는 번역(붉은 군대, 미 육군) |
  | 무기 이름·탄종 약어 | MP 40, Kar98k, APCR, HE | 원문 그대로 |
  | 캠페인 인물 이름·지명 | 사람 이름, 도시·마을 | 음독 |

- 원문 그대로 두는 말도 용어집에 적어 둔다(검사가 이 말이 번역문에 그대로 있는지 본다).
- 일반 군사 용어(예: Squad 분대, Platoon 소대, Reinforcements 증원)의 번역은 AI가 정하고 용어집에 적는다.

### 4. 문체 (AI 판단)

- 대사: 자연스러운 구어체. 말하는 사람의 계급·관계에 맞춘다.
- 임무 목표: 명령형(`~를 점령하라`).
- 메뉴·버튼: 짧은 명사형(`설정`, `저장`).
- 유닛 설명: `~다` 평서문.

### 5. 번역하는 법

- 번역은 AI가 한다(기계 번역 허용: AI 판단, 사용자가 정리를 보고 고치지 않음). 외부 번역 API는 쓰지 않는다.
- 파일 묶음 단위로 나눠 번역하고, 묶음마다 검사를 통과해야 결과에 넣는다.
- 순서(AI 판단): ① 유닛 이름·설명(`desc`) → ② 메뉴·UI → ③ 캠페인 미션·대사(`mission`) → ④ 모드 텍스트. 단계마다 생성·검사·커밋한다.
- 그대로 두는 것: `<…>` 태그와 그 안, `</c>`, `\n`, `%s`·`%i`·`%d`·`%02d` 등 자리표시, `{…}` 안, URL.

### 6. 글꼴

- 한글이 없는 글꼴마다 같은 이름의 `.font`를 내 `binary_data`가 한글이 있는 `main_<크기>.fnt`를 가리키게 한다(AI 판단). 예: `font/roboto_con_15.font` → `binary_data "main_15.fnt"`.
- 크기 대응: 같은 크기가 있으면 그 크기, 없으면 가장 가까운 크기(13→12, 14→15, 42·60→30). 굵은 글꼴(`…b`)은 `main_<크기>b`가 있으면 그것(8·9·10·12), 없으면 보통 굵기. `name`·`size`·`height`·`style`을 원래 값으로 둘지 main 값으로 둘지는 구현에서 정한다(빌더 몫).
- 둘 자리는 `mod/CTAToybox/resource/interface/font/`. 편집기 빌드는 자기 목록(`.ctatoybox-build.json`)에 없는 파일을 지우지 않으므로, 한글화 빌드가 직접 쓴다. 편집기 빌드가 같은 경로를 만들지 않는지 검사한다.
  - 대화에서는 "편집기 수정 목록의 파일 복사 항목으로 넣는다"고 말했으나, `file-copy`는 게임 파일을 새 이름으로 복사하는 기능이고 수정 목록이 여럿(`editor/projects/*.json`)이라 맞지 않는다. 그래서 바꿨다(AI 판단).
- 결과: 화면의 글씨 모양이 Arial 계열로 바뀐다. 글자 폭이 달라 칸을 넘칠 수 있다(미확인).
- `.fnt`가 페이지 그림을 같은 폴더에서 찾는지, 이름이 다른 `.font`가 다른 `.fnt`를 가리켜도 되는지는 미확인이다.

### 7. 도구

- 편집기 명령줄에 `l10n` 명령을 더한다(AI 판단. 본체는 `CtaToybox.Core`, 지금 구조와 같게).
  - `l10n extract`: 층을 읽어 경로마다 원문을 정하고 `l10n/ko/*.po`를 만들거나 갱신한다(새 키 추가, 바뀐 원문 fuzzy, 없어진 키 삭제). 요약(파일·항목·미번역·fuzzy 수)을 낸다.
  - `l10n check`: 아래 "검사"를 돌린다.
  - `l10n build`: `.po`에서 `mod/CTAToybox/localizations/default/…pot`과 글꼴 `.font`를 만든다. 번역이 비었거나 fuzzy인 항목은 원문(영어)으로 둔다. 자기가 만든 파일 목록을 따로 남겨 다음 빌드에서 지울 것을 안다(편집기 빌드 목록과 다른 파일).
- 생성물(`mod/CTAToybox/localizations/…`, 글꼴)은 편집기 생성물처럼 git에 넣는다(2026-10-17 사용자 결정과 같게).

### 8. 편집기 표시 (사용자 결정: 둘을 함께)

- 이름은 `원문 (한글)`(AI 판단: 정렬이 지금처럼 영어 순). 번역이 없으면 원문만.
- 검색은 원문과 한글 모두에 걸린다.
- 원문은 지금처럼 바닐라와 켜 둔 모드에서 읽는다. 출력 모드는 `GameProfile`이 층에서 이미 뺀다(`GameProfile.cs` 71~76행). 그래서 한국어가 원문 자리에 섞이지 않는다. 한글은 `l10n/ko/*.po`에서 읽는다.
- `data/display-names.csv`(`tools/Build-DisplayNames.ps1`)는 바꾸지 않는다(AI 판단).

## 검사 (게임 없이 파일로)

- 모든 원문 키가 결과에 있다(빠진 키 0). 결과에 원문에 없는 키가 없다.
- 항목마다 원문과 번역의 `<…>` 태그 목록, `%` 자리표시 목록(순서까지), `\n` 수, `{…}` 목록이 같다.
- 용어집의 "그대로 둘 말"이 원문에 있으면 번역에도 그대로 있다.
- 번역문의 모든 글자가 바꾼 글꼴(`main_*`)에 들어 있다.
- 결과 `.pot`이 다시 읽힌다(편집기의 `.pot` 읽기로 왕복).
- 단위 테스트는 `editor/CtaToybox.Core.Tests`에 지금 꼴(`T.Eq`)로 쓰고 `Suites.All`에 등록한다.

## 미확인 (게임 안 확인 없이 남는 것)

1. `localizations`에서 같은 경로 파일이 모드 쪽으로 이기는지.
2. 키를 다시 정의했을 때 어느 쪽이 쓰이는지.
3. 어느 화면이 어느 글꼴을 쓰는지, 그 글꼴을 `.font`로 바꾸면 실제로 바뀌는지.
4. 바꾼 글꼴로 글자가 칸을 넘치는지.
5. 모드가 새 언어 코드(`localizations/<코드>` + `localization.info`)를 더할 수 있는지(이번 설계는 쓰지 않는다).

사용자가 게임에서 보고 알려 주면 그때 고친다.

## 빌더에게 맡기는 것

- `.po` 읽기·쓰기 구현(외부 모듈 없음).
- `l10n` 명령의 세부 옵션과 출력 모양.
- 번역 묶음의 크기와 나누는 법, 번역 작업을 여러 세션·에이전트로 나누는 법.
- 글꼴 `.font`의 `name`·`size`·`height` 값.
- 용어집 첫 목록.
