# 게임 구조

## 기준

| 항목 | 값 |
|---|---|
| 게임 | Call to Arms - Gates of Hell: Ostfront |
| Steam appid | `400750` |
| Steam buildid | `25492057` |
| 실행 파일 버전 | `1.0.6.7-aaa36ed4d` |
| 설치 경로 | `E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell` |
| 조사 날짜 | 2026-10-11 |

숫자는 모두 `data/file-index.csv`와 `data/pak-summary.csv`에서 가져왔다. 두 파일은 `tools/Export-PakText.ps1`이 생성한다.

## 최상위 폴더

| 폴더 | 내용 |
|---|---|
| `binaries\x64` | 실행 파일과 DLL |
| `resource` | 게임 데이터. pak 아카이브 44개 |
| `localizations` | 현지화. `default.pak` 하나 |
| `mods` | 로컬 모드와 모드 제작 도구 |

### 실행 파일

| 파일 | 버전 | 역할 |
|---|---|---|
| `call_to_arms.exe` | 1.0.6.7 | 게임 |
| `call_to_arms_ed.exe` | 1.0.6.7 | 에디터 (맵·미션) |
| `call_to_arms_server.exe` | 1.0.6.7 | 전용 서버 |
| `call_to_arms_workshop.exe` | 표기 없음 | 창작마당 업로드 도구 "Digitalmindsoft Workshop Upload"(2024). 실행하지 않고 실행 파일의 화면 문자열로 확인했다(2026-10-16 R6). 자세한 것은 `docs/02-modding-research.md` "창작마당 게시" |

같은 폴더에 `steam_api64.dll`, 사운드 라이브러리 `mss64.dll`(Miles), 크래시 리포터 `BugSplat64.dll`·`BsSndRpt64.exe`, `d3dcompiler_47.dll` 등이 있다.

## pak 아카이브

pak은 일반 ZIP 파일이다. 첫 4바이트가 `50 4B 03 04`이고 .NET의 `ZipFile`로 그대로 열린다. 7-Zip 같은 압축 도구로도 열 수 있다.

`resource` 아래 44개, 합계 약 100GB다.

| pak | 크기(MB) | 파일 수 | 내용 | 많은 확장자 |
|---|---|---|---|---|
| `_dlc7.pak` | 4,372.2 | 482 | DLC 콘텐츠: 맵, 영상, 음악 | .dds=104 .mi=41 .net=41 (no-ext)=41 .info=41 .grs=41 .ply=41 .materials=41 |
| `dlc1.pak` | 3,801.0 | 482 | DLC 콘텐츠: 맵, 영상 | .dds=119 .mi=44 .net=44 (no-ext)=44 .ply=44 .materials=44 .grs=40 .omask=27 |
| `dlc2.pak` | 1,166.1 | 109 | DLC 콘텐츠: 맵, 영상 | .dds=24 .mi=8 .set=8 .net=8 (no-ext)=8 .avi=8 .info=8 .grs=8 |
| `dlc3.pak` | 4,453.1 | 504 | DLC 콘텐츠: 맵, 영상 | .dds=120 .mi=49 .net=49 (no-ext)=49 .ply=49 .materials=49 .grs=40 .info=34 |
| `dlc4.pak` | 2,122.6 | 492 | DLC 콘텐츠: 음성, 영상, 맵 | .wav=296 .avi=41 .dds=39 .set=13 .mi=12 .net=12 (no-ext)=12 .info=12 |
| `dlc5.pak` | 134.8 | 42 | DLC 콘텐츠: 맵 | .dds=12 .set=4 .png=3 (no-ext)=3 .mi=3 .net=3 .ply=3 .materials=3 |
| `entity/-vehicle.pak` | 5,300.7 | 113,317 | 차량·화포·항공기·함정 엔티티 | .ply=76635 .vol=17612 .anm=8468 .mtl=5358 .def=2443 .mdl=2141 .ext=464 .dds=110 |
| `entity/construction.pak` | 1,965.5 | 93,523 | 건물 엔티티 | .ply=41536 .mtl=25037 .vol=12367 .def=6905 .mdl=6884 .dds=664 .anm=103 .txt=10 |
| `entity/entity.pak` | 126.4 | 2,919 | 이펙트, 피해 모델, 서비스 엔티티 | .def=1757 .ply=273 .mdl=236 .mtl=210 .dds=188 .anm=103 .ebm=49 .vol=40 |
| `entity/flora.pak` | 170.3 | 1,758 | 식생 엔티티 | .ply=640 .mtl=444 .def=256 .mdl=255 .anm=62 .dds=43 .vol=30 .ebm=28 |
| `entity/humanskin.pak` | 1,656.4 | 23,485 | 보병 외형 엔티티 | .ply=14952 .mdl=3672 .def=3667 .mtl=1127 .txt=61 .dds=6 |
| `entity/inventory.pak` | 331.2 | 12,391 | 아이템 엔티티(무기, 탄약, 장구) | .ply=5592 .def=2253 .mdl=2047 .mtl=1459 .vol=558 .anm=276 .dds=196 .ebm=8 |
| `entity/landscape.pak` | 4,743.0 | 6,196 | 지형 엔티티 | .dds=3192 .mtl=1188 .def=993 .ply=459 .mdl=219 .vol=98 .ebm=28 .anm=8 |
| `gamelogic.pak` | 12.8 | 8,969 | 게임 규칙: 보병 정의, 무기·탄약, 편제, Lua 스크립트 | .set=5873 (no-ext)=2094 .ammo=518 .inc=192 .pattern=64 .weapon=54 .lua=43 .grenade=39 |
| `interface/interface.pak` | 270.2 | 2,078 | 인터페이스 그래픽, 글꼴, 레이아웃 | .tga=1449 .xml=116 .ebm=113 .fnt=97 .font=96 .dds=82 .set=77 .png=21 |
| `interface/pages.pak` | 267.0 | 1,467 | 인터페이스 화면 이미지 | .tga=504 .dds=485 .png=416 .gif=35 .ebm=24 .fx=1 .avi=1 .set=1 |
| `interface/scene.pak` | 1,604.9 | 33,334 | 인터페이스 아이콘·초상화 | .tga=23595 .ebm=8207 .png=1470 .dds=49 .fx=7 .mtl=3 .set=2 .ply=1 |
| `map/multi.pak` | 9,491.4 | 2,013 | 멀티플레이 맵 | .dds=470 .info=306 .mi=305 .net=166 (no-ext)=166 .ply=166 .materials=166 .grs=157 |
| `map/single.pak` | 744.0 | 229 | 싱글플레이 맵 | .dds=63 .set=20 .mi=17 .net=17 (no-ext)=17 .ply=17 .materials=17 .png=17 |
| `music.pak` | 1,925.7 | 113 | 음악 | .wav=96 .mp3=17 |
| `properties.pak` | 70.5 | 1,792 | 엔티티 공통 속성, 애니메이션 | .anm=1606 .inc=106 .ext=44 .ebm=21 .dds=13 .tga=2 |
| `shader.pak` | 49.3 | 5,795 | 셰이더 | .pso=4152 .vso=1510 .inc=54 .hlsl=34 .dso=27 .hso=10 .cso=4 .fxh=3 |
| `sound/detonation.pak` | 551.1 | 699 | 폭발음 | .wav=635 .set=64 |
| `sound/mission.pak` | 1,418.3 | 2,025 | 미션 음성 | .wav=1994 .set=29 (no-ext)=1 .txt=1 |
| `sound/sound.pak` | 248.5 | 1,402 | 일반 효과음 | .wav=1295 .set=103 .inc=2 .txt=1 (no-ext)=1 |
| `sound/talk.pak` | 1,494.1 | 12,500 | 유닛 음성 | .wav=12470 .txt=16 .set=14 |
| `sound/vehicle.pak` | 817.0 | 978 | 차량 소리 | .wav=764 .set=213 .txt=1 |
| `sound/weapon.pak` | 1,223.9 | 3,699 | 무기 소리 | .wav=3589 .set=110 |
| `sound/world.pak` | 804.9 | 153 | 환경음 | .wav=136 .set=17 |
| `texture/common/_building_euro.pak` | 354.5 | 731 | 텍스처 | .dds=551 .ebm=180 |
| `texture/common/_building.pak` | 675.6 | 302 | 텍스처 | .dds=300 .tex=2 |
| `texture/common/_euro.pak` | 1,187.8 | 444 | 텍스처 | .dds=443 .ebm=1 |
| `texture/common/_flora.pak` | 710.9 | 384 | 텍스처 | .dds=381 .ebm=3 |
| `texture/common/_hum.pak` | 2,157.2 | 1,351 | 텍스처 | .dds=1064 .ebm=287 |
| `texture/common/_lm.pak` | 345.6 | 371 | 텍스처 | .dds=371 |
| `texture/common/_military.pak` | 2,103.3 | 524 | 텍스처 | .dds=524 |
| `texture/common/_vehicle.pak` | 25,921.1 | 7,133 | 텍스처 | .dds=7112 .ebm=18 .def=3 |
| `texture/common/_weapon.pak` | 252.1 | 461 | 텍스처 | .dds=461 |
| `texture/common/_wood.pak` | 244.8 | 220 | 텍스처 | .dds=215 .tga=3 .ebm=1 .mtl=1 |
| `texture/common/common.pak` | 2,135.1 | 2,287 | 텍스처 | .dds=2204 .ebm=55 .tex=14 .tga=13 .jpg=1 |
| `texture/common/environment.pak` | 1,624.1 | 313 | 텍스처 | .dds=313 |
| `texture/land.pak` | 2,408.8 | 1,102 | 지형 텍스처 | .dds=1077 .ebm=18 .tga=6 .tif=1 |
| `texture/texture.pak` | 207.9 | 1,767 | 기타 텍스처 | .tga=1307 .ebm=318 .dds=96 .mtl=36 (no-ext)=4 .inc=2 .ctm=2 .etf=1 |
| `video.pak` | 2,402.1 | 53 | 영상 | .avi=52 .txt=1 |

## 가상 경로

게임은 pak 안의 파일을 하나의 폴더 트리로 본다. 파일의 가상 경로는 다음과 같다.

> `resource` 기준 pak이 있는 폴더 + pak 안의 경로

예: `resource\entity\-vehicle.pak` 안의 `-vehicle/germany/cannon/75mm_pak9738/75mm_pak9738.def`는 `entity/-vehicle/germany/cannon/75mm_pak9738/75mm_pak9738.def`가 된다. `resource` 바로 아래의 pak(`gamelogic.pak`, `properties.pak`)은 접두사가 없다.

근거: 차량 `.def`가 `(include "/properties/cannon.ext")`를 쓰는데, 위 규칙으로 추출하면 `properties.pak` 안의 `properties/cannon.ext`가 정확히 `properties/cannon.ext`에 놓인다. 이 규칙으로 include가 얼마나 풀리는지는 `docs/04-units/coverage.md`의 "펼치지 못한 구문"에서 확인할 수 있다.

최상위 가상 폴더별 파일 수:

| 가상 폴더 | 파일 수 | 유닛 범위 |
|---|---|---|
| `entity/` | 253,589 | 일부(-vehicle, humanskin, inventory) |
| `interface/` | 36,883 | 밖 |
| `sound/` | 21,752 | 밖 |
| `texture/` | 17,390 | 밖 |
| `set/` | 8,924 | 전체 |
| `shader/` | 5,801 | 밖 |
| `map/` | 3,960 | 밖 |
| `properties/` | 1,792 | 전체 |
| `video/` | 146 | 밖 |
| `music/` | 115 | 밖 |
| `script/` | 45 | 전체 |

## 파일 형식

텍스트 형식은 대부분 같은 블록 문법(`{이름 값 ...}`)을 쓴다. 문법은 `docs/04-units/README.md`에 정리한다. "표본"은 직접 열어 본 파일이다. 표본을 열지 않은 형식은 역할을 적지 않았다.

### 유닛 범위 안의 텍스트 형식

| 확장자 | 파일 수 | 역할 | 표본 |
|---|---|---|---|
| `.def` | 8,363 | 엔티티 정의. 차량은 `{game_entity`, 아이템은 `{actor`로 시작하고 공통 속성을 include한다 | `entity/inventory/-gear_wearable/head/[finland]/fin_helmet_cze/fin_helm_cze.def` |
| `.mtl` | 7,944 | 재질. 텍스처 경로와 블렌딩 | `entity/inventory/.../sov_cap_peaked/sov_gog.mtl` |
| `.mdl` | 7,860 | 모델 구조. 3ds Max에서 내보낸 뼈대(`{skeleton {bone ...`)와 볼륨 | `entity/humanskin/[united_states]/us_m41_1st/us_m41_1st_hs2_rfl_bag.mdl` |
| `.set` | 5,873 | 설정. `set/breed` 아래는 보병 정의(`{breed`), 그 밖은 게임 규칙·편제 | `set/breed/single/rus/1941_summer/spetsnaz_2.set` |
| 확장자 없음 | 2,094 | `set/stuff` 아래의 아이템 정의. 헬멧, 총기 등. `{from "pattern ..."`으로 시작 | `set/stuff/head/us_m1_29th_co_b_net_var1` |
| `.ammo` | 518 | 탄약 | `set/stuff/shell/bulletrus_85.aphebc.ammo` |
| `.ext` | 508 | 엔티티가 include하는 속성 묶음. `properties/` 아래는 공통, 차량 폴더 아래는 그 차량 전용 | `entity/-vehicle/ussr/tank_heavy/kv85/kv85.ext` |
| `.inc` | 328 | include용 조각. 매크로 정의(`(define ...)`)가 많다 | `set/breed/presets/perks/veterancy.inc` |
| `.pattern` | 64 | 아이템의 원형. `{from "pattern 이름"}`이 가리키는 대상 | `set/stuff/head/headgear.pattern` |
| `.weapon` | 54 | 무기 (로켓, 박격포 등 일부. 총기는 확장자 없는 파일) | `set/stuff/bazooka/panzerfaust_60.weapon` |
| `.grenade` | 39 | 수류탄 | `set/stuff/grenade/anti-personnel grenade/m36.grenade` |
| `.bzc` | 25 | 베지어 곡선 값 목록. 블록 문법이 아니다 | `set/curves/tonemapping/dark3.bzc` |
| `.reg` | 18 | 인터페이스 스킨 등록 | `set/registry/main_menu_skin.reg` |
| `.mine` | 16 | 지뢰 | `set/stuff/explosive/m1a1.mine` |
| `.presets` | 14 | `set/stuff`의 폴더별 매크로 모음. 파일명이 `.presets`다 | `set/stuff/smg/.presets` |
| `.fsm` | 6 | 아이템을 손에 드는 상태 정의 | `set/stuff/_settings/hand-only.fsm` |
| `.dl` | 6 | 난이도 설정 | `set/difficulty/heroic.dl` |
| `.hand` | 1 | 투척 동작 정의 | `set/stuff/thrower.hand` |
| `.template` | 1 | 게임 모드 템플릿. 주석으로 각 항목을 설명한다 | `set/multiplayer/games/gamemode.template` |
| `.lua`, `.md`, `.txt` | 112 | 스크립트와 문서. 블록 문법이 아니므로 파싱하지 않고 목록만 둔다 | `script/multiplayer/units/unit_roster_readme.md` |

### 전체 확장자 분포

모든 pak의 파일을 확장자로 센 것이다. 유닛 범위 밖(맵, 인터페이스, 사운드, 텍스처 등)을 포함한다.

| 확장자 | 파일 수 |
|---|---|
| `.ply` | 140,428 |
| `.mtl` | 34,863 |
| `.vol` | 30,705 |
| `.tga` | 26,906 |
| `.wav` | 21,277 |
| `.dds` | 21,091 |
| `.def` | 18,277 |
| `.mdl` | 15,454 |
| `.anm` | 10,626 |
| `.ebm` | 9,396 |
| `.set` | 6,607 |
| `.pso` | 4,152 |
| `(no-ext)` | 2,441 |
| `.png` | 1,999 |
| `.vso` | 1,510 |
| `.ammo` | 518 |
| `.ext` | 508 |
| `.mi` | 479 |
| `.inc` | 444 |
| `.info` | 443 |
| `.net` | 341 |
| `.materials` | 340 |
| `.grs` | 317 |
| `.omask` | 190 |
| `.avi` | 146 |
| `.xml` | 116 |
| `.txt` | 104 |
| `.fnt` | 97 |
| `.font` | 96 |
| `.pattern` | 64 |
| `.weapon` | 54 |
| `.tex` | 46 |
| `.lua` | 43 |
| `.grenade` | 39 |
| `.gif` | 35 |
| `.hlsl` | 34 |
| `.dso` | 32 |
| `.bzc` | 25 |
| `.reg` | 18 |
| `.mp3` | 17 |
| `.mine` | 16 |
| `.presets` | 14 |
| `.fx` | 12 |
| `.hso` | 11 |
| `.log` | 6 |
| `.bmp` | 6 |
| `.dl` | 6 |
| `.fsm` | 6 |
| `.mod` | 6 |
| `.psd` | 5 |
| `.cso` | 4 |
| `.fxh` | 3 |
| `.html` | 3 |
| `.md` | 3 |
| `.tpl` | 3 |
| `.ctm` | 2 |
| `.js` | 2 |
| `.h` | 1 |
| `.hand` | 1 |
| `.css` | 1 |
| `.template` | 1 |
| `.iml` | 1 |
| `.jpg` | 1 |
| `.xsd` | 1 |
| `.etf` | 1 |
| `.lnk` | 1 |
| `.tif` | 1 |
| `.json` | 1 |

`.ply`(메시), `.vol`(충돌 볼륨), `.anm`(애니메이션), `.dds`·`.tga`(텍스처), `.wav`(소리)가 대부분이다. 이 바이너리 형식의 내부는 조사하지 않았다.

## 유닛 데이터가 있는 곳

| 가상 경로 | 내용 | 전체 파일 | 텍스트 파일 | `.def` |
|---|---|---|---|---|
| `set/breed/` | 보병 유닛 정의 | 5,127 | 5,126 | - |
| `set/stuff/` | 무기·탄약·수류탄·지뢰·장구 | 2,805 | 2,805 | - |
| `set/multiplayer/` | 멀티플레이 편제·비용·게임 모드 | 103 | 101 | - |
| `set/dynamic_campaign/` | 동적 캠페인 설정 | 16 | 16 | - |
| `entity/-vehicle/` | 차량·화포·항공기·함정 엔티티 | 113,317 | 10,456 | 2,443 |
| `entity/humanskin/` | 보병 외형 엔티티 | 23,485 | 8,466 | 3,667 |
| `entity/inventory/` | 아이템 엔티티 | 12,391 | 5,759 | 2,253 |
| `properties/` | 엔티티 공통 속성 | 1,792 | 130 | - |

`set/breed/`의 파일 하나는 0바이트라 텍스트 수가 하나 적다.

유닛 범위에 넣지 않은 엔티티: `entity/construction`(건물, 93,523개), `entity/landscape`(지형, 6,196개), `entity/flora`(식생, 1,758개), `entity/fx`·`entity/particle`·`entity/sfx`(이펙트), `entity/service`, `entity/-fauna`(동물, 260개), `entity/mapping_assets`, `entity/misc`.

## 리소스 로딩 순서

### 디스크에서 확인한 것

- 바닐라의 pak들 사이에 같은 가상 경로를 가진 텍스트 파일은 없다(유닛 범위 기준, `file-index.csv`에 `duplicate`와 `collision`이 0건).
- 2026-07-09의 게임 로그(`game.log`, 당시 버전 1.063)에 pak을 마운트한 순서가 남아 있다.

```
1. resource 아래의 pak (dlc1 ~ dlc5, entity/*, gamelogic, interface/*, map/*, ... , video, _dlc7)
2. localizations/default.pak
3. 활성 모드의 pak, 모드 순서대로
   mod 2905667604 → mod 3732639687 → mod 3295913567 → 로컬 모드 → mod 2829744930
```

모드 안에서는 `resource` 아래 pak 다음에 그 모드의 `localizations/default.pak`이 마운트된다. 모드 순서는 프로필 `options.set`의 `{mods ...}` 목록 순서와 같다.

로그 앞부분에는 `Scanning mods in .../mods/`, `Found 11 available mods`, `Mod: applying mods`, `Mod: disable mod ...` 줄이 있다. 게임은 시작할 때 로컬 모드 폴더를 훑고, 모드를 적용할 때 켜지 않은 모드를 끈다.

### 리서치로 보완한 것

- 같은 경로의 파일이 모드끼리 겹치면 `options.set`의 `{mods}` 목록에서 뒤에 있는 모드의 것이 쓰이고, 모드의 파일은 바닐라의 것을 이긴다. 게임에서 **확인**했다(2026-10-13, `docs/07-stat-editor/ingame-check.md`). 본 것은 뒤 모드의 낱개 파일이 앞 모드의 pak 안 파일과 바닐라 pak 안 파일을 이긴 경우다. pak끼리 겹친 경우는 따로 보지 않았다.
- 같은 모드 안에서 낱개 파일과 pak이 겹치면 낱개 파일이 쓰인다. pak끼리 겹쳐도 목록 뒤쪽 모드의 것이 쓰인다. 게임에서 **확인**했다(2026-10-13, `docs/07-stat-editor/ingame-check.md`의 "일괄 확인").
- 모드 순서는 게임 안에서 직접 정할 수 없다는 보고가 있고, `options.set`의 `{mods ...}` 목록을 직접 고치는 방법이 알려져 있다.

출처와 자세한 내용은 `docs/02-modding-research.md`의 "리소스 덮어쓰기" 절에 있다.

## 프로필·로그·세이브

위치: `%LOCALAPPDATA%\digitalmindsoft\gates of hell`

| 경로 | 내용 |
|---|---|
| `log\game.log` | 마지막 실행의 로그 |
| `log\minidumps\` | 크래시 덤프(`.mdmp`)와 그 로그(`.mdmp.log`). 파일명에 날짜와 게임 버전이 들어간다 |
| `profiles\<Steam 계정 번호>\options.set` | 설정. 활성 모드 목록 포함 |
| `profiles\<번호>\settings.set`, `keys.set`, `game_progress.set` | 게임 설정, 키 설정, 진행 상황 |
| `profiles\<번호>\save\`, `campaign\`, `replays\` | 세이브, 캠페인, 리플레이 |
| `shader_cache\` | 컴파일된 셰이더 |

활성 모드는 `options.set`에 다음처럼 기록된다. 창작마당 모드는 `mod_<창작마당 ID>:0` 형태다.

```
{mods
	"mod_2905667604:0"
	"mod_3732639687:0"
	"mod_3295913567:0"
	"mod_2829744930:0"
}
```

이 목록의 모드 4개는 현재 디스크에 파일이 없다(`steamapps\workshop\content\400750` 폴더가 없음).

## 현지화

`localizations\default.pak`은 영어 기본 텍스트다. gettext `.pot` 파일 362개와 `localization.info` 하나가 들어 있다. 인코딩은 UTF-8이다.

유닛의 표시 이름은 `default/interface/text/desc/` 아래에 있다.

```
msgctxt "desc/entity/20mm_flakvierling38"
msgid "2cm Flakvierling 38"
```

`msgctxt`가 유닛을 가리키는 키이고 `msgid`가 영문 표시 이름이다. 엔티티는 `desc/entity/<엔티티 이름>`, 보병은 `desc/human/<breed 경로>` 형식이다. 유닛과의 연결 결과는 `docs/04-units/README.md`에 있다.

## 게임에 들어 있는 문서와 도구

| 위치 | 내용 |
|---|---|
| `mods\template\` | 공식 모드 템플릿. `mod.info`, `readme.txt`, 예시 폴더 구조 |
| `mods\tools\map_editor(eng).pdf` | 맵 에디터 설명서 |
| `mods\tools\mission_editor(eng).pdf` | 미션 에디터 설명서 |
| `mods\tools\plug-ins\` | 3ds Max 내보내기 플러그인. x64는 2011~2016, 2020, 2024, 2025용, x86은 2011~2013용 |
| `mods\tools\ExampleFiles3dsMax\` | 예시 `.max` 파일 (T-62, 테크니컬, 건물, T2) |
| `gamelogic.pak`의 `script/multiplayer/units/unit_roster_readme.md` | 유닛 편성표 형식 설명 |
| `gamelogic.pak`의 `set/multiplayer/games/gamemode.template` | 게임 모드 정의 템플릿 |
