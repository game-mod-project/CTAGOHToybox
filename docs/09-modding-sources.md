# 모딩 참고·출처 사이트 정리 (2026-10-14 확인)

사용자가 준 "Gates of Hell 유닛 모딩 파일·속성 정리"의 출처와 추가 참고 사이트를 모두 열어 보고 정리했다. 이 문서는 손으로 고친다.

**작업 규칙**: 게임·모드 동작을 다루는 작업은 시작 전에 이 문서와 [08-unit-files-check.md](08-unit-files-check.md)를 확인한다. 관련 사이트가 있으면 그 페이지를 다시 열어 바뀐 것이 있는지 본다. 사이트의 내용은 바닐라 파일(`_work/vanilla`)로 확인한 뒤에만 사실로 쓴다. 확인하지 못한 것은 "미확인"으로 둔다.

## 먼저 알아 둘 것

- **같은 엔진의 다른 게임 자료가 많다.** Call to Arms 위키(현대전, M1A2·T-62)와 Men of War: Assault Squad 2 자료는 문법은 비슷하지만 파일과 키가 다르다. 붙여 받은 문서의 `("armor_heavy" turret(...))`, `"speed2"`, `"damage_cumulative"`, `perks_generic.inc`, `crew.ext`, `desc.lng`는 모두 Call to Arms 위키(아래 S1·S2)에서 왔다. 지금 Gates of Hell에는 없다(08 문서).
- **Steam 창작마당 글은 많은 수가 "삭제됨·게임과 호환되지 않음"으로 표시돼 있다.** 2026-10-14에 그 표시가 붙은 것: S4, S9, S10, S11, 시야 절의 V6~V10, R8, R11. 내용은 읽히지만 오래된 버전 기준일 수 있다.
- **개발자 발언은 공식 공지(S8, V3, R12)와 CHANGE-LOG(V4)뿐이다.** 나머지는 모더 경험이다.
- **게임 버전이 자주 파일을 바꾼다.** 2026-09-24 기준 최신은 V 1.067.0이다. 공지의 "Note to Modders"가 깨지는 파일을 알려 준다(아래 "패치가 바꾼 파일").

## 출처 (붙여 받은 문서의 출처 목록)

| # | 사이트 | 읽기 | 기준 게임·근거 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|---|
| S1 | [Modding/Files – Call to Arms Wiki](https://calltoarms.fandom.com/wiki/Modding/Files) | 읽음(내장 브라우저. WebFetch는 402) | Call to Arms, 위키 편집자 | 파일 목록과 폴더 구조. `human.ext`(이동·장갑·Sensor·brain), 차량 `.def`, `.mdl`의 뼈대(포 상하 `{limits}`는 `.mdl`에 있다고 함), `perks_generic.inc`, `vehicles_XXX.set`, `bot.data.lua`, `library.reg`, include 경로 규칙(같은 폴더는 `/` 없이, 다른 폴더는 `/`로 시작) | 파일 목록은 CtA 것이라 GoH와 다르다. include 경로 규칙은 GoH 파일에서도 같다(`(include "/properties/tank.ext")`, `(include "s.inc")`). GoH `.def`에는 `{bone "gun_rot" {limits -10 20}}`가 있다 |
| S2 | [Modding/Examples – Call to Arms Wiki](https://calltoarms.fandom.com/wiki/Modding/Examples) | 읽음(내장 브라우저) | Call to Arms, 위키 편집자 | M1A2 장갑 `("armor_heavy" turret(600 600 600 300) ...)`(앞·옆·뒤·위 순), T-80 탄종별 `("speed2" s())`, T-62에 ATGM 추가(무기·탄 엔티티·`.ammo`·`desc.lng` 네 곳을 함께 고침), 옛 MoW 전차 이식 절차, `vehicles_XXX.set`의 `s(XXX)`로 진영 이동 | 장갑·초속 형식은 GoH에 없다(08). "여러 파일을 함께 고친다"는 절차의 모양은 GoH도 같다 |
| S3 | [Mod Content – GoH Wiki](https://gatesofhell.fandom.com/wiki/Mod_Content) | 읽음(내장 브라우저) | GoH, 위키 편집자 | 폴더 설명 페이지로 가는 목차뿐이다. 하위: [Set_Folder](https://gatesofhell.fandom.com/wiki/Set_Folder)(파일 이름 목록만, 설명 없음), Stuff_Folder, Resource_Folder, Localizations_Folder, Hotmod1968, MACE | 해당 없음 |
| S4 | [Conquest Modding guide by Schwienyy](https://steamcommunity.com/sharedfiles/filedetails/?id=3629209372) | 읽음 | GoH, 모더(2025-12) | 콘퀘스트에 새 분대 넣기: breed(`skin`은 `.mdl` 뺀 이름), `inf_ger.set`(`cost()` 예 K98 8, MP40 10, MG42 35~40), `units_ger.set`(`squad_with3types_conquest`의 숫자 = 병종 종류 수, `name()`은 연구 항목 이름과 같아야 함, `c1(breed:수)`), `unit_research_ger.set` 자리는 두 수(오른쪽 칸, 아래 줄), 번역 `.pot`의 `msgctxt "desc/human/mp/..."`, `desc/squad/<이름>(ger)`, 초상화 `portrait_squad/<분대>_00~03.png`(없으면 클릭할 때 게임이 꺼진다고 함) | 가격 값은 `inf_ger.set` 줄 주석과 맞다. `squad_with3types_conquest`, `name()`, `c1()`은 대조하지 않았다(미확인) |
| S5 | [Make Breed – Normand 답변](https://steamcommunity.com/workshop/filedetails/discussion/455424236/3114771735692558832) | 읽음 | **Men of War: AS2**, 모드 제작자(2021) | AS2의 breed 예: `{perks "health" "250%" "speed" 1.0 "rifle" lvl_3}`, `{veterancy (include "veterancy_generic.inc") ("veterancy_1")}`, `{inventory {item "luger" filled} {in_hands 0}}` | GoH는 `perks`를 매크로로 고른다(`("tier2_standard")`). `{item "luger" filled}`는 같다 |
| S6 | [No fuel cans in any vehicle](https://steamcommunity.com/app/400750/discussions/0/3055114073625644646/) | 읽음 | GoH, 모더(2021) + 개발자는 설계 의도만 | 차량 `.def`의 `{extender "inventory" {box ... {item "can" 3}}}`로 빈 연료통. 세이브 수정은 `AppData\Local\digitalmindsoft\gates of hell\profiles\<ID>`의 세이브를 7-Zip으로 열어 `mission.scn` | `{extender "inventory"}`와 `{box}`는 GoH 파일에 있다. `{cell x y}`의 뜻은 미확인 |
| S7 | [Changing repair speed for vehicles](https://steamcommunity.com/app/400750/discussions/0/3824158244553504675/) | 읽음 | GoH, 사용자(2023) | 수리 설정이 `properties/tank.ext`에 있다는 말뿐이다 | `tank.ext`에 `("turret" repair(45))`, `("gun" repair(30) thickness(20))`가 있다(08) |
| S8 | [Steam 공지 (~2022-02)](https://store.steampowered.com/news/posts/?feed=steam_community_announcements&appids=400750&enddate=1645213578) | 읽음 | GoH, **개발자** | Update 19(V1.017, 2021-12-21) Note to Modders: `set/stuff/gun`을 새 직사·곡사 정의에 맞추고 `set/stuff/vision`을 고친 모드는 갱신하라. Update 21(V1.018, 2022-02-09): `properties/cannon_armor.ext`·`vehicle.ext`가 body에 볼륨을 더 받는다, 포탄 인벤토리 크기·충전물 계산 변경 | 지금 파일에 `accuracy_direct`/`accuracy_indirect`가 있다. 시야는 그 뒤 `set/vision`으로 옮겨졌다 |
| S9 | [Guide for changing Conquest Enhanced v2](https://steamcommunity.com/sharedfiles/filedetails/?id=2567221405) | 읽음 | GoH(모드 기준), 모더(2021~2023) | `resources_xxx`의 AI 군대 크기 `Base`(초:CP), `StageCP`(단계별 CP. 고치면 Budget start도), `units_ger`의 `cp()`=CP 비용·`cost`=MP 비용·`min_stage(99)`면 못 씀, `set/multiplayer/games/campaign_capture_the_flag`의 `points_table_player`/`points_table_ai`, `resource/conquest_configuration`의 `Attackmin`/`Attackmax`, 맵 끄기 `map/multi/<맵>/campaign_capture_the_flag.info`, 로그 `Documents\My Games\gates of hell\log` | `{StageCP 60 80 100 120 140}`, `{Bots {CP {Base "0:500"}}}`는 바닐라 `resources_standard.set`에 있다. cp가 CP 비용이라는 것은 모더 설명이다(파일 주석 없음) |
| S10 | [Increased Unit Cap](https://steamcommunity.com/sharedfiles/filedetails/?id=2928929379) | 읽음 | GoH, 댓글 모더(2024, V1.045) | `resources_<난이도>.set`의 `{StageCP ...}`가 단계 I~V의 최대 유닛, `{RP {StartVal 6}}` 시작 연구 포인트 | `StageCP`는 있다. "최대 유닛"이라는 뜻은 미확인 |
| S11 | [Simple Damage increase](https://steamcommunity.com/sharedfiles/filedetails/?id=2792852728) | 읽음 | GoH(V1.036~1.067), 모더 | `set/ballistics.set` 곡선 `{0.05 3.60}`: 첫 수는 거리 계수(고치지 말 것), 둘째는 피해 계수. 로드 순서는 `options.set`의 `mod_<ID>:0` 위치. 함께 플레이하면 resource desync 보고 | 바닐라 주석과 맞다: 첫 값은 무기 사거리 배수, 둘째 값은 피해 배수(08) |
| S12 | [Hotmod 1968 버그 리포트](https://steamcommunity.com/workshop/filedetails/discussion/2614199156/4814903332730255742) | 못 읽음 | | 글이 지워졌다("That item does not exist") | |

## 출처 (시야·탐지 절)

| # | 사이트 | 읽기 | 기준·근거 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|---|
| V1 | [Separate vehicle vision](https://steamcommunity.com/app/400750/discussions/0/3818543707180733231/) | 읽음 | GoH, 사용자(2023-08) | 시야 필드 6개의 값, `uncover`·`h_fov`·`v_fov`·`"spotting" r()`의 뜻, 기준 거리는 `generic.inc` | 지금 값은 `human`의 `h_fov`가 150, `human_peripheral`의 spotting이 0.22로 바뀌었다. 나머지는 같다(08) |
| V2 | [Disable unit vision sharing viable?](https://steamcommunity.com/app/400750/discussions/0/4033600364201752775/) | 읽음 | GoH, 모더 1인 실험(2023-12) | `properties/human.ext`의 `InformRange`를 1로 하면 분대 안에서도 표적 정보를 나누지 않았다 | 키는 있다: `{InformRange 80 100}`, 주석 "첫 값 AI, 둘째 값 사용자"(08) |
| V3 | [Steam 공지 (~2022-03)](https://store.steampowered.com/news/posts/?feed=steam_community_announcements&appids=400750&enddate=1647185112) | 읽음 | GoH, **개발자** | Update 19: 새 시야 로직, 정찰 차량 visor와 소음 감지 visor 추가, 전차 조종수 FOV 30→45°. Update 21: 발견한 표적은 사거리 안에서 시선이 이어지는 한 계속 보인다(은신 유닛 제외) | 소음 감지는 `{vision "noise_detection"}`로 `human.ext`·`vehicle.ext`에 있다. 지금 `vehicle_main`의 `h_fov`는 50이다 |
| V4 | [CHANGE-LOG](https://steamcommunity.com/app/400750/discussions/0/4498514133983766024/) | 일부(1·3·6쪽) | GoH, **개발자**(Culee, Ty) | U9: PvP 야간 시야 25% 감소, 분대 산개는 `set/squads.set`. U11: 정찰병 시야각·거리 증가. U35 Note to Modders: 맵에 둔 잔해·동물은 playerid 0, 콘퀘스트 설정 파일 이름이 바뀌고 나뉘었다. U36: `set/stuff/shell/*.ammo` 태그와 `target/ammo.set` 변경. U68: `pather.set`·`obstacle.reg`. U69: 지원 유닛 비용이 로스터 `.set`으로 옮겨짐 | 이 저장소의 `findings-goh.psv`가 이 스레드를 근거로 쓴다. 2·4·5쪽은 읽지 않았다 |
| V5 | [About night vision devices](https://steamcommunity.com/app/400750/discussions/0/3943524562544037387/) | 읽음 | GoH, 사용자(2023-11) | `{extender "vision_mode" {timer_object {tag "night"} {active_time 60} {recharge_rate 1} {type "recharge"}}}`. 값의 뜻과 관리 파일은 답이 없다 | 대조하지 않았다(미확인) |
| V6 | [MACE Range increase and Supply 댓글](https://steamcommunity.com/sharedfiles/filedetails/comments/2926190357) | 읽음 | GoH, 모더(~2025-12) | 사거리는 모드 `stuff/`의 `smg`·`rifle`·`gun`, 시야는 `gamelogic.pak`의 vision 파일 하나. 다른 모드보다 나중에 로드. `.presets`에 없는 define을 쓰면 `define not found (eSDLReader.cpp:455)`로 시작 중 멈춘다 | 시야 파일은 `set/vision/vision_fields.inc`다 |
| V7 | [Increased Viewrange](https://steamcommunity.com/sharedfiles/filedetails/?id=2517130206) | 읽음 | GoH, 모더(2021~2022) | 그래픽 시야 거리 4배. 모드의 `environment_config.ext`에서 배율. 그래픽 설정이 중 이상이어야 한다 | 렌더링 거리이고 유닛 탐지와는 다른 것으로 보인다(미확인) |

## 추가 참고 사이트

| # | 사이트 | 읽기 | 쓸 만한 내용 |
|---|---|---|---|
| R1 | [CtA – Gates of Hell 공식 디스코드](https://discord.com/servers/cta-gates-of-hell-596302805635956777) | 소개 페이지만 | 약 4만 명. 모딩 채널에서 모더와 가끔 개발자가 답한다. 채널 내용은 로그인해야 보인다 |
| R2 | [MACE 디스코드](https://discord.com/servers/mace-970356070327406672) | 소개 페이지만 | 약 8,900명. 유닛·국가 추가 모드 MACE의 서버. 이 PC에 MACE(2905667604)가 구독돼 있다 |
| R3 | [GEM 2 Modding Community (ModDB)](https://www.moddb.com/groups/gem-2-modding-community) | 읽음(내장 브라우저. WebFetch는 403) | 엔진 공통 Q&A. 댓글: breed `.set`에서 체력·속도·스태미나를 고친다(MoW 기준). 그룹이 [GEM 2 Modding Community gitbook](https://gem-2-modding-community.gitbook.io/gem-2-modding-community)을 연결한다: 메시, 볼륨과 존(ables·parameters·tags), 재질, 3ds Max 플러그인(MoW·CtA 기준) |
| R4 | [GoH Wiki 대문](https://gatesofhell.fandom.com/wiki/Gates_of_Hell:_Ostfront_Wiki) | 읽음(내장 브라우저) | 대문뿐이다 |
| R5 | [German units](https://gatesofhell.fandom.com/wiki/German_units) | 읽음(내장 브라우저) | 유닛별 주포, 관통력(거리별), 장갑, 속도, 장전, 승무원. 판터 G: 속도 46, 장전 8초 → 바닐라 `speed(46)`, `("reload time" seconds(8))`와 맞다. 장갑 "차체 전면 110mm"는 파일의 볼륨 값(`body1` 기본 40, `top 82` 등)과 1:1로 맞지 않는다. 위키 장갑 값은 비교 기준으로 쓰지 않는다 |
| R6 | [USSR Conquest Unit Cost](https://gatesofhell.fandom.com/wiki/USSR_Conquest_Unit_Cost) | 읽음(내장 브라우저) | 개요 몇 줄뿐이다. 수치표가 없다. 연구로 CP가 오르고, 경제 설정이 보상과 AI 규모를 바꾼다는 설명 |
| R7 | [나무위키 – Gates of Hell](https://en.namu.wiki/w/Gates%20of%20Hell) | 읽음 | 모드 소개 목록(한글 패치, VALOUR, HOTMOD 1968, MACE 등)뿐이다. 기술 정보 없음 |
| R8 | [Guide to Gate of Hell Mod Manager](https://steamcommunity.com/sharedfiles/filedetails/?id=3488308840) | 읽음(삭제 표시) | MrCookie's Mod Manager(창작마당 2986408240). `Documents\my games\gates of hell\profiles\<ID>\options.set`을 골라 순서를 끌어 바꾼다. 위에서 아래 순서로 로드 |
| R9 | [Add mod not from steam?](https://steamcommunity.com/app/400750/discussions/0/599645602069875221) | 읽음 | 개발자(Tele): 게임의 `mods` 폴더에 넣으면 로컬 모드로 읽는다. 모더: `mods\<이름>\mod.info`, 키 `Name`(폴더 이름과 같게)·`Desc`·`MinGameVersion`·`MaxGameVersion`(`any` 가능), 압축을 한 단계 더 풀면 인식 안 됨, 이름의 공백은 피한다 |
| R10 | [Editor Help](https://steamcommunity.com/app/400750/discussions/0/3825289117352705259) | 읽음 | 에디터는 MoW AS2/GEM 2 계열. AS2의 `mods/tools`에 맵·미션 제작 PDF. 지형: `+`/`-`로 영역 크기, Shift+드래그로 평탄화 |
| R11 | [BM Mod 파일 편집 튜토리얼](https://steamcommunity.com/workshop/filedetails/discussion/2922646932/4033601393695434118) | 읽음(숨김 표시) | 피 지속시간 `entity/fx/hit/bm_*`의 `{timer {duration 600}}`, 피 양 `set/interaction_entity/human.inc`의 `blood_small`/`blood_big`·`rand`, 시체·팔다리·포탄 구멍 한도 `set/entitymanager.set`(BM Mod 값 `dead` 200, `limbs` 500, `shellhole` 500) |
| R12 | [GoH 개발 업데이트 (Steam 뉴스)](https://store.steampowered.com/news/app/400750) | 일부(News API로 최근 40건) | 아래 "패치가 바꾼 파일" |
| R13 | [ModDB – GoH 모드](https://www.moddb.com/games/gates-of-hell/mods) | 읽음(내장 브라우저) | Hotmod 1968(Gates of Hell), Weltkrieg 1914-1918, ROA 등 목록 |
| R14 | [ModDB – GEM 2 다운로드](https://www.moddb.com/engines/gem-2/downloads) | 읽음(내장 브라우저) | "gem2 blender plugin (GOH/MOWAS2)"(2026-09-21, GEM2 모델 가져오기·내보내기), SPC - SinglePlayer Creator, Noresus Project |

모드 사례(R15~R18):

| # | 사이트 | 읽기 | 쓸 만한 내용 |
|---|---|---|---|
| R15 | [Conquest Enhanced V2 변경 기록 3쪽](https://steamcommunity.com/sharedfiles/filedetails/changelog/2899823515?p=3) | 읽음 | 정찰 차량 시야·발견 강화, spotting을 바닐라로 되돌림, 저격수를 hearing·spotting 파일에 추가. 경로 없음 |
| R16 | [Modern Conflict 변경 기록 4쪽](https://steamcommunity.com/sharedfiles/filedetails/changelog/3014476200?p=4) | 읽음 | 모딩에 쓸 내용 없음 |
| R17 | [That207Guy's Realism](https://steamcommunity.com/workshop/filedetails/?id=3142379562) | 읽음 | 시야 설계 수치(사람 180~200m, 경전차·APC 800m, 중형 900m, 중전차 1000m). 경로 없음 |
| R18 | [Long Range Combat](https://steamcommunity.com/workshop/filedetails/?id=2514972920) | 읽음 | 사거리 약 1.75배. 댓글: 멀티에서 쓰려면 `mod.info`의 게임 버전을 `any`로 |

차량 파생 모델 `_x`·`_xx`(R19, 2026-10-15):

| # | 사이트 | 읽기 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|
| R19 | Best Way GEM RTS 문서(Men of War II, **개발사**): [X 모델](https://docs.bestway.com.ua/gem-rts-v1-ru/modeli/plan-nastroiki-modelei-tekhniki/kh-model-tekhniki.md), [XX 모델](https://docs.bestway.com.ua/gem-rts-v1-ru/modeli/plan-nastroiki-modelei-tekhniki/khkh-model-modeli-tekhniki.md), [모델 설정 순서](https://docs.bestway.com.ua/modeling/vehicle-model-setup-pipeline.md), [맵 장식용 파괴 차량](https://docs.bestway.com.ua/editor/map-editor/specifics-of-map-editing/creating-destroyed-vehicle-entities-for-map-entourage) | 읽음(서브에이전트, WebFetch) | X는 부분 손상 모델이다. 부품이 부서지면(폭발 파괴 제외) 그 부품을 X 모델의 것으로 바꾼다. XX는 수리할 수 없는 완전 파괴 모델이고 폭발 때 날아가는 부품이 있다. 둘 다 엔진이 바꾸는 기술용 모델이고 맵에 직접 두지 않는다 | `_xx`는 맞다: `vehicle-main.inc:249` `on destroy` → 차체면 `explosion` → `:579` `{set_skeleton "_xx" sync}`(`tank/tank-main.inc:446`도 같다). 엔진 파괴(`_burn`)는 바꾸지 않는다. `_x`는 GoH 스크립트에 차량을 바꾸는 줄이 없고 `_x.mdl`에 부품별 `{sequence "break" ...}`(807개 파일, 2,747곳)만 있다. 미확인. `_x.def` 806개는 include·장갑이 없다. `_xx.def`는 대부분 `properties/vehicle_xx.ext`를 읽고 본체와 따로 적힌 장갑 볼륨을 가진다. GoH 전용 Steam·모더 글은 찾지 못했다 |

창작마당의 대형 모드(Hotmod 1968, Conquest Enhanced v2, Modern Conflict, Realism, Long Range Combat, Valour)는 이 PC에 구독돼 있지 않다(2026-10-14, `steamapps\workshop\content\400750`). 구독된 것은 MACE, MACE Wunderwaffe Ultimate Fusion ENG, Spawn Menu, Auto Medic-AA-Artillery & Weather Switch와 3617250510이다. 파일을 비교하려면 사용자가 구독해야 한다.

## 패치가 바꾼 파일 (개발자 공지)

모드가 바닐라 파일을 덮어쓰면 패치 뒤 깨질 수 있다. 공지에 나온 것만 적는다.

| 날짜·버전 | 바뀐 것 |
|---|---|
| 2021-12-21 V1.017 (Update 19) | `set/stuff/gun`의 직사·곡사 정의, `set/stuff/vision`(지금은 `set/vision`), `grass.set` 모듈화, 스크립트 변수 `players:composition@` |
| 2022-02-09 V1.018 (Update 21) | `properties/cannon_armor.ext`, `vehicle.ext`(body 볼륨), 예광탄 길이 define, 포탄 인벤토리 크기·충전물·무게 계산 |
| CHANGE-LOG U35·U36 | 콘퀘스트 설정 파일 이름 변경과 분할, 맵 배치 엔티티는 playerid 0, `set/stuff/shell/*.ammo` 태그, `target/ammo.set`, `human.mdl` |
| 2026-05-07 V1.063 (Update 68) | `pather.set`, `obstacle.reg`(넘기 동작) |
| 2026-07-24 V1.064.0 (Update 69) | 로스터 `.set`의 지원 유닛 비용, 콘퀘스트 `.mi`의 `{functions (include "../dcg_functions.inc")}`, `interface/action/teleport.set` 추가, LUA `scene_query_readme.md`, `BotApi.Instance.CampaignFirstEnemyId` → `BotApi.Conquest.FirstEnemyId` 등 이름 변경 |
| 2026-09-21 V1.066.0 (Update 70) | 콘퀘스트 AI 로스터 스키마 readme(`set/multiplayer/units/unit_roster_readme.md`, 바닐라에 있다), `BotApi.Instance.teamSize` → `ownTeamSize`, `entity.set` 안의 `vehicle/animation-defines.inc` 로드 순서 |
| 2026-09-24 V1.067.0 (Hotfix 1) | 맵을 업그레이드하는 콘퀘스트 템플릿이 `mods` 아래 TEMPLATE 폴더에 있다 |
| 2026-06-25 Devblog #164 | 쓰지 않는 기본 에셋을 공식 창작마당 모드로 옮겨 모드가 의존 항목으로 쓰게 한다는 예고 |

## 읽지 못한 것

- S12: 글이 지워졌다.
- V4: CHANGE-LOG 2·4·5쪽을 읽지 않았다.
- R1·R2: 디스코드 채널은 로그인해야 보인다. 로그인하지 않았다.
- R12: 2026-05 이전 공지 본문 일부를 읽지 않았다.
- fandom(S1~S3, R4~R6)과 ModDB(R3, R13, R14)는 WebFetch가 막혀(402·403) 내장 브라우저로 읽었다. 봇 확인 화면은 나오지 않았다.
