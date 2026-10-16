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
| S4 | [Conquest Modding guide by Schwienyy](https://steamcommunity.com/sharedfiles/filedetails/?id=3629209372) | 읽음 | GoH, 모더(2025-12) | 콘퀘스트에 새 분대 넣기: breed(`skin`은 `.mdl` 뺀 이름), `inf_ger.set`(`cost()` 예 K98 8, MP40 10, MG42 35~40), `units_ger.set`(`squad_with3types_conquest`의 숫자 = 병종 종류 수, `name()`은 연구 항목 이름과 같아야 함, `c1(breed:수)`), `unit_research_ger.set` 자리는 두 수(오른쪽 칸, 아래 줄), 번역 `.pot`의 `msgctxt "desc/human/mp/..."`, `desc/squad/<이름>(ger)`, 초상화 `portrait_squad/<분대>_00~03.png`(없으면 클릭할 때 게임이 꺼진다고 함) | 가격 값은 `inf_ger.set` 줄 주석과 맞다. 2026-10-15 바닐라 대조: `conquest/settings.set`의 `squad_with3types_conquest`가 `{"%name(%side)" … {content "mp/%side/%period/%c1 … %c3"}}`라 숫자는 병종 종류 수, 연구 항목은 `"squad_officer_con(ger)"`처럼 `name()`에 진영을 붙인 이름을 가리킨다(`unit_research_ger.set`). `c(breed:N)`의 N은 인원으로 보인다(`set/multiplayer/units/mid/units_eng.set` 416행의 분대 합 7명이 줄 주석 "1 smg, 5 rifle, 1 mg"과 같다). 추정 |
| S5 | [Make Breed – Normand 답변](https://steamcommunity.com/workshop/filedetails/discussion/455424236/3114771735692558832) | 읽음 | **Men of War: AS2**, 모드 제작자(2021) | AS2의 breed 예: `{perks "health" "250%" "speed" 1.0 "rifle" lvl_3}`, `{veterancy (include "veterancy_generic.inc") ("veterancy_1")}`, `{inventory {item "luger" filled} {in_hands 0}}` | GoH는 `perks`를 매크로로 고른다(`("tier2_standard")`). `{item "luger" filled}`는 같다 |
| S6 | [No fuel cans in any vehicle](https://steamcommunity.com/app/400750/discussions/0/3055114073625644646/) | 읽음 | GoH, 모더(2021) + 개발자는 설계 의도만 | 차량 `.def`의 `{extender "inventory" {box ... {item "can" 3}}}`로 빈 연료통. 세이브 수정은 `AppData\Local\digitalmindsoft\gates of hell\profiles\<ID>`의 세이브를 7-Zip으로 열어 `mission.scn` | `{extender "inventory"}`와 `{box}`는 GoH 파일에 있다. `{cell x y}`의 뜻은 미확인 |
| S7 | [Changing repair speed for vehicles](https://steamcommunity.com/app/400750/discussions/0/3824158244553504675/) | 읽음 | GoH, 사용자(2023) | 수리 설정이 `properties/tank.ext`에 있다는 말뿐이다 | `tank.ext`에 `("turret" repair(45))`, `("gun" repair(30) thickness(20))`가 있다(08) |
| S8 | [Steam 공지 (~2022-02)](https://store.steampowered.com/news/posts/?feed=steam_community_announcements&appids=400750&enddate=1645213578) | 읽음 | GoH, **개발자** | Update 19(V1.017, 2021-12-21) Note to Modders: `set/stuff/gun`을 새 직사·곡사 정의에 맞추고 `set/stuff/vision`을 고친 모드는 갱신하라. Update 21(V1.018, 2022-02-09): `properties/cannon_armor.ext`·`vehicle.ext`가 body에 볼륨을 더 받는다, 포탄 인벤토리 크기·충전물 계산 변경 | 지금 파일에 `accuracy_direct`/`accuracy_indirect`가 있다. 시야는 그 뒤 `set/vision`으로 옮겨졌다 |
| S9 | [Guide for changing Conquest Enhanced v2](https://steamcommunity.com/sharedfiles/filedetails/?id=2567221405) | 읽음 | GoH(모드 기준), 모더(2021~2023) | `resources_xxx`의 AI 군대 크기 `Base`(초:CP), `StageCP`(단계별 CP. 고치면 Budget start도), `units_ger`의 `cp()`=CP 비용·`cost`=MP 비용·`min_stage(99)`면 못 씀, `set/multiplayer/games/campaign_capture_the_flag`의 `points_table_player`/`points_table_ai`, `resource/conquest_configuration`의 `Attackmin`/`Attackmax`, 맵 끄기 `map/multi/<맵>/campaign_capture_the_flag.info`, 로그 `Documents\My Games\gates of hell\log` | `{StageCP 60 80 100 120 140}`, `{Bots {CP {Base "0:500"}}}`는 바닐라 `resources_standard.set`에 있다. cp가 CP 비용이라는 것은 모더 설명이다(파일 주석 없음) |
| S10 | [Increased Unit Cap](https://steamcommunity.com/sharedfiles/filedetails/?id=2928929379) | 읽음 | GoH, 댓글 모더(2024, V1.045) | `resources_<난이도>.set`의 `{StageCP ...}`가 단계 I~V의 최대 유닛, `{RP {StartVal 6}}` 시작 연구 포인트 | `StageCP`는 단계마다 쓸 수 있는 CP다(확인: `values.set` "Amount of CP available for each stage"). 유닛 수가 아니다. `runtime_config.lua`의 CPLimitStages 60/140/240/360이 StageCP 60 80 100 120의 누적합이다 |
| S11 | [Simple Damage increase](https://steamcommunity.com/sharedfiles/filedetails/?id=2792852728) | 읽음 | GoH(V1.036~1.067), 모더 | `set/ballistics.set` 곡선 `{0.05 3.60}`: 첫 수는 거리 계수(고치지 말 것), 둘째는 피해 계수. 로드 순서는 `options.set`의 `mod_<ID>:0` 위치. 함께 플레이하면 resource desync 보고 | 바닐라 주석과 맞다: 첫 값은 무기 사거리 배수, 둘째 값은 피해 배수(08) |
| S12 | [Hotmod 1968 버그 리포트](https://steamcommunity.com/workshop/filedetails/discussion/2614199156/4814903332730255742) | 못 읽음 | | 글이 지워졌다("That item does not exist") | |

## 출처 (시야·탐지 절)

| # | 사이트 | 읽기 | 기준·근거 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|---|
| V1 | [Separate vehicle vision](https://steamcommunity.com/app/400750/discussions/0/3818543707180733231/) | 읽음 | GoH, 사용자(2023-08) | 시야 필드 6개의 값, `uncover`·`h_fov`·`v_fov`·`"spotting" r()`의 뜻, 기준 거리는 `generic.inc` | 지금 값은 `human`의 `h_fov`가 150, `human_peripheral`의 spotting이 0.22로 바뀌었다. 나머지는 같다(08) |
| V2 | [Disable unit vision sharing viable?](https://steamcommunity.com/app/400750/discussions/0/4033600364201752775/) | 읽음 | GoH, 모더 1인 실험(2023-12) | `properties/human.ext`의 `InformRange`를 1로 하면 분대 안에서도 표적 정보를 나누지 않았다 | 키는 있다: `{InformRange 80 100}`, 주석 "첫 값 AI, 둘째 값 사용자"(08) |
| V3 | [Steam 공지 (~2022-03)](https://store.steampowered.com/news/posts/?feed=steam_community_announcements&appids=400750&enddate=1647185112) | 읽음 | GoH, **개발자** | Update 19: 새 시야 로직, 정찰 차량 visor와 소음 감지 visor 추가, 전차 조종수 FOV 30→45°. Update 21: 발견한 표적은 사거리 안에서 시선이 이어지는 한 계속 보인다(은신 유닛 제외) | 소음 감지는 `{vision "noise_detection"}`로 `human.ext`·`vehicle.ext`에 있다. 지금 `vehicle_main`의 `h_fov`는 50이다 |
| V4 | [CHANGE-LOG](https://steamcommunity.com/app/400750/discussions/0/4498514133983766024/) | 일부(1·3·6쪽) | GoH, **개발자**(Culee, Ty) | U9: PvP 야간 시야 25% 감소, 분대 산개는 `set/squads.set`. U11: 정찰병 시야각·거리 증가. U35 Note to Modders: 맵에 둔 잔해·동물은 playerid 0, 콘퀘스트 설정 파일 이름이 바뀌고 나뉘었다. U36: `set/stuff/shell/*.ammo` 태그와 `target/ammo.set` 변경. U68: `pather.set`·`obstacle.reg`. U69: 지원 유닛 비용이 로스터 `.set`으로 옮겨짐 | 이 저장소의 `findings-goh.psv`가 이 스레드를 근거로 쓴다. 2·4·5쪽은 읽지 않았다 |
| V5 | [About night vision devices](https://steamcommunity.com/app/400750/discussions/0/3943524562544037387/) | 읽음 | GoH, 사용자(2023-11) | `{extender "vision_mode" {timer_object {tag "night"} {active_time 60} {recharge_rate 1} {type "recharge"}}}`. 값의 뜻과 관리 파일은 답이 없다 | 2026-10-15 대조: `timer_object`, `extender "vision_mode"`, `active_time`, `recharge_rate`는 v1.067 바닐라 어디에도 없다. 바닐라에 있는 것은 `{vision_modes_active}`와 환경 파일의 `{vision_mode {mode none}}`뿐이다. 글의 값은 옛 버전이거나 모드의 것으로 보인다(미확인) |
| V6 | [MACE Range increase and Supply 댓글](https://steamcommunity.com/sharedfiles/filedetails/comments/2926190357) | 읽음 | GoH, 모더(~2025-12) | 사거리는 모드 `stuff/`의 `smg`·`rifle`·`gun`, 시야는 `gamelogic.pak`의 vision 파일 하나. 다른 모드보다 나중에 로드. `.presets`에 없는 define을 쓰면 `define not found (eSDLReader.cpp:455)`로 시작 중 멈춘다 | 시야 파일은 `set/vision/vision_fields.inc`다 |
| V7 | [Increased Viewrange](https://steamcommunity.com/sharedfiles/filedetails/?id=2517130206) | 읽음 | GoH, 모더(2021~2022) | 그래픽 시야 거리 4배. 모드의 `environment_config.ext`에서 배율. 그래픽 설정이 중 이상이어야 한다 | 2026-10-16 대조(R3): `environment_config.ext`는 v1.067 바닐라에 없다(모드 자체 파일, 이 PC에 모드가 없어 내용 미확인). 바닐라 환경 파일의 화면 거리 값은 거리 안개 `{haze {start} {end}}`(지도 환경 파일 끝 대부분 4000~6000m)와 피사계 심도 `{dof {far_start} {far_end}}`다. `full_opaque_view_distance`는 물 표현(`enhanced_water_look`) 값이다. 유닛 발견 거리는 `set/vision` 파일이 정하고 그 파일들은 환경 값을 읽지 않는다(확인: `set/vision`에 haze·environment 참조 없음). 그래서 렌더 거리를 늘려도 발견 거리는 바뀌지 않는 쪽과 맞다. 게임 확인은 없다 |
| V8 | [공식 매뉴얼 Rev 1.0 (2022-09, PDF)](https://cdn.akamai.steamstatic.com/steam/apps/400750/manuals/CtA_-_GoH_Game_manual_1.0.pdf) | 읽음(사용자가 받은 파일, `pdftotext`. 이 주소의 파일과 크기가 같다) | GoH, **개발자** | 직접 조종 조준 표시: 초록 점은 사격선이 트임, 빨간 점은 가림 또는 사거리 밖. 고른 유닛 발밑의 색 원(분대장은 굵게). 미니맵 범례와 색. 판 설정 "Fog of War off는 모두가 모든 것을 본다". Ctrl+Alt+Shift+D는 인터페이스 숨기기. 안개 경계·시야 범위 표시는 나오지 않는다 | 키는 지금 `profiles/default/keys.set`과 같다(E, T, J, Shift+F). 표시 정리는 `docs/07-stat-editor/ingame-displays.md` |
| V9 | [Update 53 (V1.048.0, 2024-12-19)](https://store.steampowered.com/news/app/400750/view/1786409468882493) | 읽음(Steam 뉴스 API 본문) | GoH, **개발자** | 적을 누르면 그 적의 FOV가 보인다. 안개가 켜졌을 때만 되고, 안개를 끈 판으로 넓힐 계획. 사람의 지뢰 탐지 반경 표시 | 게임에서 보지 않았다. `settings.set` 주석의 "line of sight visualization"(`cannon_00`을 찾는 거리)이 이 선으로 보인다(추정) |
| V10 | [Cone vision](https://steamcommunity.com/app/400750/discussions/0/599656262950476810/) | 읽음 | GoH, **개발자**(Ty)와 사용자(2025-06) | Ty: 적을 누르면 시야의 대략 윤곽이 보인다. 유닛마다 발견 거리가 달라 정확하지 않다. Saxhorn: 오른쪽 단추로 적 하나씩, 아군은 안 해 봄. 쐐기는 선 병사를 찾는 최대 거리 같다, 차량은 쐐기 밖에서도 찾는다, 승무원이 드러난 포는 무기의 시야만 그린다, 안개를 켜면 미니맵에 승무원마다의 시야가 그려진다(Flak 30 장전수는 옆 90°) | 파일 주석은 선을 `cannon_00`을 찾는 거리라고 한다. 사용자 해석과 다르다 |
| V11 | [Enemy Vision](https://steamcommunity.com/app/400750/discussions/0/599650454957265734/) | 읽음 | GoH, 사용자(2025-04) | Saxhorn(시험 안 했다고 적음): 앞 쐐기는 지형을 따지지 않은 시야각, 작은 안쪽 원은 벽 너머라도 알아채는 거리(그 안이면 적 미니맵에 나옴), 뒤가 잘린 큰 원은 덤불·엎드린 병사를 찾는 곳. 보통 병사 약 100m, 정찰병 약 120m | 바닐라 `human` r 1.0(100m), `human_recon` r 1.25(125m) |
| V12 | [Is it possible to get the range from the camera to cursor?](https://steamcommunity.com/app/400750/discussions/0/599648005874838915/), [Cant get Artillery to work in Conquest](https://steamcommunity.com/app/400750/discussions/0/599661451928575553/) | 읽음 | GoH, 사용자(2025-03, 2025-08) | 직접 조종 거리 숫자: 빨강은 사거리 밖, 흰색은 안. 지면 공격(F6)을 고르면 전술 화면에서도 거리가 나온다. Quick Direct Control에는 기본 키가 없다. T-34로 약 596m가 빨갛게 나왔다. 무기 사거리 원은 없다 | Quick Direct Control은 `keys.set`의 `control_manual/modify`, 값 `""`(확인). 2026-10-16 이 PC에서는 약 100m 너머 숫자가 사라졌다. 왜 다른지 미확인 |
| V13 | [mg42s not firing](https://steamcommunity.com/app/400750/discussions/0/4291440545832700484/) | 읽음 | GoH, 사용자(2024-03) | 사선이 막히면 초록 조준점이 장애물 위에 놓였다. 사선은 총구 끝에서 잰다 | 대조할 파일 없음 |
| V14 | [Steam 뉴스 API](https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid=400750&count=400&maxlength=0&format=json) (공지 400건, 2015-11~2026-05) | 키워드 검색 | GoH, **개발자** | Update 51(2024-10): 안개를 걷는 유닛을 "적이 아닌 유닛"에서 "아군"으로, 에디터 콘솔 명령 `ui_armorDisplay`. Dev update #107(2021-11): 움직이는 차량은 엔진 소리로 visor 거리보다 50m 더 멀리서 드러난다. Update 21(2022-02): 전차·대전차포가 찾은 표적은 최대 사거리 안에서 시선이 이어지는 동안 보인다. Update 24(2022-06): 안개를 그리는 새 미니맵. Update 27(2022-08): 전차·포 주포의 시야를 무기 사거리에 맞춤. Update 66(2026-02): 주무기 설명 아래 최대 사거리. Update 68(2026-05): MP 정보 창에 탄종별 최대 사거리. Beta phase 1(2021-03): 3인칭은 무기 사거리 제한을 없앤다. Update 13(2021-09): 3인칭 최대 사거리 제한은 위에서 보는 포에만. Update 14(2021-09): 보급 반경 원 그래픽 | #107은 `spotting.inc`의 움직일 때 식 `r × (100 + 50/r)`과 맞다(확인). 툴팁 "Range (current ammo)"는 현지화 문자열에 있다 |
| V15 | [Sensor (docs.bestway.com.ua)](https://docs.bestway.com.ua/game-settings/sensor) | 읽음 | **다른 게임**(Men of War 계열 엔진의 개발사 문서) | `v_visor 1`은 유닛마다 볼 수 있는 곳, `v_visorInfo 1`은 그 세부, `v_sensorBasic`은 찾은 표적을 색(적 빨강, 아군 초록, 중립 노랑, 미확인 청록)과 기호로. 콘솔 여는 법은 없음 | GoH 실행 파일에도 같은 변수 이름이 있다(확인). 동작은 콘솔을 열지 못해 미확인 |
| V16 | [Simple Battle in Editor](https://steamcommunity.com/sharedfiles/filedetails/?id=2522001504) | 읽음(내장 브라우저) | GoH, 사용자(M4F10S0, 2021-06, 2025-05 갱신) | 에디터는 실행할 때 고르거나(실행 옵션 "Ask when starting game") `-edit -level <난이도>`. F1 모드에서 Left Alt+F로 안개를 켠다. F7로 난이도 | 게임에서 보지 않았다 |

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
| R10 | [Editor Help](https://steamcommunity.com/app/400750/discussions/0/3825289117352705259) | 읽음 | 에디터는 MoW AS2/GEM 2 계열. AS2의 `mods/tools`에 맵·미션 제작 PDF. 지형: `+`/`-`로 영역 크기, Shift+드래그로 평탄화. 2026-10-16 R5: GoH 게임 폴더의 같은 PDF 둘을 읽었다. 2013년 러시아어 원본의 영어판이라 AS2(2014)보다 앞선다. 내용은 `docs/02-modding-research.md` "에디터 설명서" |
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

차량 파생 모델 `_x`·`_xx`(R19, 2026-10-15). 2026-10-16 게임 안 확인: `panzer4g_x.mdl` 사본에서 엔진 뼈의 모델을 포탑 모델로 바꿔 두고 Panzer IV G의 엔진을 쏴 부수자("Engine Broken") 엔진 덱 자리에 포탑 모양 두 개가 나타났다. 포탑 등 부서지지 않은 부품은 그대로였다. 부품이 부서지면 그 부품을 `_x.mdl`의 같은 뼈 모델로 바꾼다는 개발사 설명과 맞다:

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
| 2026-04-08 V1.062.0 (Update 67) | `pather.set`, `obstacle.reg`, 맵 오브젝트 `.def` 다수(사람이 넘어가기, vault). 처음 바뀐 것은 이 업데이트다(2026-10-16 R1) |
| 2026-05-07 V1.063 (Update 68) | `pather.set`, `obstacle.reg`(기는 동안 넘기 끄기) |
| 2026-07-24 V1.064.0 (Update 69) | 로스터 `.set`의 지원 유닛 비용, 콘퀘스트 `.mi`의 `{functions (include "../dcg_functions.inc")}`, `interface/action/teleport.set` 추가, LUA `scene_query_readme.md`, `BotApi.Instance.CampaignFirstEnemyId` → `BotApi.Conquest.FirstEnemyId` 등 이름 변경 |
| 2026-09-21 V1.066.0 (Update 70) | 콘퀘스트 AI 로스터 스키마 readme(`set/multiplayer/units/unit_roster_readme.md`, 바닐라에 있다), `BotApi.Instance.teamSize` → `ownTeamSize`, `entity.set` 안의 `vehicle/animation-defines.inc` 로드 순서 |
| 2026-09-24 V1.067.0 (Hotfix 1) | 맵을 업그레이드하는 콘퀘스트 템플릿이 `mods` 아래 TEMPLATE 폴더에 있다 |
| 2026-06-25 Devblog #164 | 쓰지 않는 기본 에셋을 공식 창작마당 모드로 옮겨 모드가 의존 항목으로 쓰게 한다는 예고 |

## 추가 조사 (2026-10-16, 백로그 R1·R2)

CHANGE-LOG 2·4·5쪽, 2026-05 이전 공지, Nexus, Steam 가이드 52건을 서브에이전트가 읽고 바닐라에서 파일·키가 있는지 Grep으로 대조했다. 대조는 `_work/vanilla`(entity·properties·script·set)에 풀린 파일만 했다. 표 안의 주장은 개발자 글이라도 게임에서 확인한 것이 아니다.

### R1-a CHANGE-LOG 2·4·5쪽

스레드: https://steamcommunity.com/app/400750/discussions/0/4498514133983766024/ . 2쪽(`?ctp=2`)은 글 #16~#30(Update 16~30, 2021-09~2022-11), 4쪽은 #46~#60(Update 44~55, 2024-03~2025-04), 5쪽은 #61~#75(Update 55 이어짐~67, 2025-04~2026-04)다. 세 쪽의 글 45개는 모두 개발자 표시(commentthread_author_developer)가 붙은 Culee·Ty·BWS의 글이다. curl로 받아 본문만 뽑아 읽었다.

| 사이트(주소) | 읽기 | 기준·근거 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|
| CHANGE-LOG 2쪽 #17 Update 17 V1.015 | 읽음 | GoH, **개발자**(Culee, 2021-10-23) | "Mention for modders": 겨울 메시는 `---#winter.ply`, 기본 모델에 `---#winter.dds`가 있어야 texmod를 알아챈다(더미는 `.ebm`로). LOD는 `---#winter_lod-.ply`. 사람 스킨 이름이 바뀌어 모드 breed를 고쳐야 할 수 있다 | `#winter`가 든 파일 17개(예: `entity/humanskin/[germans]/ger_heer_43_var1/*.mdl`). 있음 |
| CHANGE-LOG 2쪽 #18 Update 18 V1.016 | 읽음 | **개발자**(2021-10-31) | 전차 조종수 시야각 +5°, 비행기는 유닛 시야에 잡히면 450→600m에서 그린다 | 대조 안 함(값 변경) |
| CHANGE-LOG 2쪽 #19 Update 19 V1.017 | 읽음 | **개발자**(2021-12-25) | 이미 docs/09에 있는 Note to Modders(`resource/set/stuff/gun` 직사·곡사 정의, `set/stuff/vision`) 외에: 소음 감지 visor, 정찰 차량 visor, 쌍안경·저격수 uncover/spot 반경 증가, 전차 주포 시야 증가, "드러난 뒤 계속 보이는 시간" 증가, 콘퀘스트 로스터에 SG43·76mm M1902 | `set/stuff/vision`은 없다. 지금은 `set/vision/`(hearing.inc, settings.set, spotting.inc, vision.set, vision_fields.inc). 다름(이동) |
| CHANGE-LOG 2쪽 #20 Update 20 V1.017.1 | 읽음 | **개발자**(2021-12-25) | 깨진 세이브는 `documents/my games/gates of hell/profiles/<번호>/save/`의 `.scn`을 메모장으로 고친다 | 게임 파일 대조 대상 아님 |
| CHANGE-LOG 2쪽 #22 Update 22 V1.019 | 읽음 | **개발자**(2022-03-13) | Note for modders: interaction 코드를 새 기능에 맞춰 고칠 것, gun 파일의 define 이름 일부 변경, 모드에 `set/pather.set`이 있으면 새 지형 종류로 갱신. 직사 전차포 최대 사거리 차이를 줄임 | `set/pather.set` 있음 |
| CHANGE-LOG 2쪽 #24 Update 24 V1.021 | 읽음 | **개발자**(2022-06-08) | 탄 쏠 때 들키는 거리 감소(사람 시야 기준 200→150m, 저격총 200→100m), 은신 발견 거리 감소, 항공기 시야 감소, 불탄·헐벗은 덤불·나무는 시야를 막지 않게 하는 interaction 코드 추가, 포병 사거리를 선형(250~600m = 2000~24000m)으로 | 대조 안 함(값). `set/interaction_entity/flora.inc` 있음 |
| CHANGE-LOG 2쪽 #27 Update 27 V1.024 | 읽음 | **개발자**(2022-08-30) | 전차·포 주포 시야를 무기 사거리에 맞춤(docs/09 V14와 같음), 시야를 막지 않는 `concrete_block` 변형 추가 | 대조 안 함 |
| CHANGE-LOG 2쪽 #28 Update 28 V1.025 | 읽음 | **개발자**(2022-10-03) | 차량 hearing visor 높이를 차체 위까지 올림, 발견된 포가 보이는 시간 증가, 탄약 트레일러 탐지 거리 감소, Panzer II L에 recon 시야, x 차량·장갑차의 x 모델 properties 갱신 | 대조 안 함 |
| CHANGE-LOG 2쪽 #29 Scorched Earth 출시(V1.026) | 읽음 | **개발자**(2022-11-23) | 콘퀘스트 연구 파일에 "유닛 요구 조건과 칸 위치를 읽기 쉬운 새 형식" 도입, 산포 포 detect 속성 수정, 공격 중 차량이 드러나는 시간 증가 | 연구 파일은 `set/dynamic_campaign/unit_research_<진영>.set`(docs/09 S4). 형식 대조는 안 함 |
| CHANGE-LOG 4쪽 #46~#47 Update 44 V1.038.0 | 읽음 | **개발자**(Ty, 2024-03-13) | 은신 분대 한도 2~3, 은신 분대 해금 tier 2. doctrine 항목 이름과 DP 비용·`limit` 변경 수십 건(예: `doctrine_panzer1f` 한도 2→3), 콘퀘스트 CP 비용 변경, Goliath는 시야 없음 | 대조 안 함(값) |
| CHANGE-LOG 4쪽 #49 Update 46 V1.040.0 | 읽음 | **개발자**(Ty, 2024-05-08) | "Modders": 차체가 부서지면 `vehicle-main.inc`의 interaction 스크립트로 좌석을 끄고 수리하면 켠다. `hearing.inc`에 빠진 저격총 참조 수정 | `set/interaction_entity/vehicle/vehicle-main.inc` 있음, `set/vision/hearing.inc` 있음. 그 안에 "sniper" 글자는 없다. 다름(이름이 다르거나 바뀜, 미확인) |
| CHANGE-LOG 4쪽 #51 Update 48 V1.042.0 | 읽음 | **개발자**(Culee, 2024-06-26) | 콘퀘스트 세이브를 열 때 모드가 다르면 버전 숫자가 빨갛게 나온다. 일부 미국 셔먼을 `max stage 99`로 | `max_stage(99)`는 `set/multiplayer/units/conquest/units_*.set`에 있다 |
| CHANGE-LOG 4쪽 #53 Update 50 V1.045.0 | 읽음 | **개발자**(2024-07-30) | StuG III G·Kht-130의 `max_stage`를 AI용으로 조정 | 있음(위와 같음) |
| CHANGE-LOG 4쪽 #54 Update 51 V1.046.0 | 읽음 | **개발자**(2024-10-14) | `teamsize_cp_factor`를 MP 판에 추가(코옵·콘퀘스트 제외). 팀 최대 인원으로 1v1 1.3, 2v2 1.2, 3v3 1.1, 4v4 1.0. 시작 CP와 15·25분 증가분에 함께 곱한다. `{extender "edifice"}`는 지붕·위층을 숨길 건물 조각에만(새 properties `.inc`를 가리킴). `mods/tools`에 3ds Max 2024·2025 플러그인. 안개를 걷는 유닛을 "not-enemy"에서 "ally"로 | 있음: `set/multiplayer/games/battle_zones.set` 3행 `teamsize_cp_factor=1:1.3,2:1.2,3:1.1,4:1.0`, 주석이 공지와 같다(`ammunition.set`, `frontlines.set`에도). `{extender "edifice"}`는 `properties/building.inc`, `building_roof.inc`에 있다 |
| CHANGE-LOG 4쪽 #57~#58 Update 53 V1.048.0 | 읽음 | **개발자**(2024-12-16) | 적을 눌러 FOV 보기(docs/09 V9와 같음), 지뢰 탐지 반경 표시, 지뢰 탐지기 반경 5→10m | 대조 안 함 |
| CHANGE-LOG 5쪽 #63 Update 59 V1.054.0 | 읽음 | **개발자**(2025-08-20) | Note to modders: 맵 파일에 없는 엔티티를 콘퀘스트에서 남기려면 `.def`에 `{DynCmpPersistent}`(예: `resource/properties/crater.inc`). 차량 잔해를 남기려면 interaction 스크립트에 `{mark_for_persistence}`(예: `vehicle-main.inc`의 `{on "_burn"}`). 정찰·은신·위생병 이동 속도 보정 +15·20%→+10% | 있음: `properties/crater.inc` 4행 `{DynCmpPersistent}`. `{mark_for_persistence}`는 `vehicle-main.inc` 583·635행(주석 "Conquest map persistence")과 `tank-main.inc`, `tank-damage.inc`, `cannon.inc`, `car-main.inc`, `moto.inc`에 있다 |
| CHANGE-LOG 5쪽 #64 Update 60 V1.055.0 | 읽음 | **개발자**(BWS, 2025-09-26) | 맵 `.info`의 `{texMode 'none'}`은 없는 texmod이니 `;{texMode}`로 주석 처리. 'destroyed' 태그는 소스 코드가 자동으로 붙이므로 `airborne.inc`·`marine.inc`의 검사 스크립트를 교체 | 맵 `.info`는 풀린 폴더에 없어 대조 못 함 |
| CHANGE-LOG 5쪽 #67 Update 64 V1.059.0 | 읽음 | **개발자**(2025-12-05) | 영국 구축함 대공포 부서짐을 위해 gun define `gun5`·`gun6` 추가 | 대조 안 함 |
| CHANGE-LOG 5쪽 #68 Update 65 V1.060.0 | 읽음 | **개발자**(2025-12-16) | `interaction_entity/flora.inc`가 burned·bare가 아닌 덤불의 시야 막기를 자동으로 켠다. 모더가 `bf109_e4b`를 MP에 넣으면 없는 뼈를 가리키는 define 때문에 꺼지던 문제 수정 | `set/interaction_entity/flora.inc` 있음 |
| CHANGE-LOG 5쪽 #69 Update 66 V1.061.0 | 읽음 | **개발자**(2026-02-09) | 봇 최대 연구 단계 13→15. `inf_tier2_lite` define의 CP를 `inf_tier2`와 같게 | 대조 안 함 |
| CHANGE-LOG 5쪽 #71 Update 67 V1.062.0 | 읽음 | **개발자**(2026-04-08) | Note to Modders: 사람이 넘어가기(vault) 위해 `pather.set`, `obstacle.reg`, 많은 맵 오브젝트 `.def`가 바뀌었다 | `set/pather.set` 있음. `obstacle.reg`는 풀린 폴더에 없다(미확인) |

### R1-b 2026-05 이전 공지

Steam 뉴스 API(공지 400건) 가운데 2026-05-01 이전 글에서 키워드가 든 것 54건을 찾았다. 세일·인사 공지 11건은 상점 주소의 "set/"가 걸린 것이라 뺐다. 나머지 43건의 해당 문단을 읽었다. 아래는 docs/09 "패치가 바꾼 파일" 표에 없는 파일 변경만이다. 모두 **개발자** 공지다.

| 사이트(주소) | 읽기 | 기준·근거 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|
| [Update 3](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/4056035920049393019) 2021-06-16 V1.001.0 | 읽음 | 개발자 | `generic.inc`에 짚(hay) fx 재질 추가 | 대조 안 함 |
| [Update 9](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/4038024692286758151) 2021-07-22 V1.007.0 | 읽음 | 개발자 | 분대 산개 로직을 `set/squads.set`에서 고칠 수 있다(docs/09 V4에 있으나 "패치가 바꾼 파일" 표에는 없다) | `set/squads.set` 있음 |
| [Update 10](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/4038025325470668295) 2021-07-28 | 읽음 | 개발자 | spawn 명령에 지면 구속 규칙 추가. 모더는 interaction 명령 `constrain_after_dir`로 쓴다 | 있음: `set/interaction_entity/flora.inc`, `vehicle/vehicle-main.inc` |
| [Dev update #102](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/4022264629318962035) 2021-08-21 | 읽음 | 개발자 | 모더용 새 interaction 함수 `{if season ".."}`(맵 계절로 값 바꾸기) | `{if season` 그대로는 없다. `set/interaction_entity/generic.inc` 244행 등에 `else season "winter"` 형태가 있다. 다름(형식) |
| [Update 17](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/4103335143113281995) 2021-10-23 V1.015 | 읽음 | 개발자 | 겨울 메시 이름 규칙 `#winter`와 사람 스킨 이름 변경(R1-a와 같음) | 있음(R1-a) |
| [Update 22](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/5631194088570774961) 2022-03-14 V1.019 | 읽음 | 개발자 | interaction 코드, gun 파일 define 이름, `set/pather.set` 지형 종류(R1-a와 같음) | `set/pather.set` 있음 |
| [Update 36](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/5141475722341661124) 2023-08-28 V1.030.0 | 읽음 | 개발자 | docs/09 U36 행 외에: `target/ammo.set`에서 `{ground}`를 대부분 지움(자동포가 표적이 죽거나 움직인 뒤에도 쏘던 문제), `{if effector_velocity}` 기반 이벤트를 `call_synced`로 | `call_synced`가 든 파일 23개(예: `set/interaction_entity/cannon.inc`) |
| [Update 41](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/5395938616802003671) 2023-12-09 V1.035.0 | 읽음 | 개발자 | Note for modders: 콘퀘스트 맵의 (0,0) 위치는 수면보다 낮으면 안 된다(공습기가 '익사'함). U59도 `vehicle-main.inc`의 수위 검사 지연으로 같은 문제를 다룸 | 맵 파일이라 대조 못 함 |
| [Update 46](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/5768625435392148614) 2024-05-08 V1.040.0 | 읽음 | 개발자 | `vehicle-main.inc`(부서진 차체 좌석 끄기), `hearing.inc`(저격총 참조) | R1-a와 같음 |
| [Update 50](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/5963413094366378325) 2024-07-30 V1.045.0 | 읽음 | 개발자 | interaction_entity 스크립트의 무작위 지연 안 호출을 `call_synced`로. 모더도 온라인·코옵 sync를 위해 고치라고 권함 | `call_synced` 있음(위) |
| [Devblog #151](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/6211118320230812800) 2024-10-12 | 읽음 | 개발자 | "French barn 01"을 다시 내보내고 하위 파일 이름을 바꿈. 맵 제작자는 그 헛간을 자기 자신으로 바꿔 놓고 맵을 다시 저장해야 한다. 예전에 태그를 넣었을 때도 모드를 다시 내보내라고 했다고 언급 | 대조 안 함 |
| [Update 51](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/6212244583176433075) 2024-10-17 V1.046.0 | 읽음 | 개발자 | `{extender "edifice"}`를 필요한 건물 조각에만, 새 properties `.inc`를 가리킴. Liberation 건물 22개의 하위 조각을 20개 안팎으로 합침 | 있음(R1-a) |
| [Update 56](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1800357164472175) 2025-05-24 V1.051.0 | 읽음 | 개발자 | prop `no_mouse_hint`로 모더가 특정 유닛의 마우스 힌트(장갑 두께 힌트)를 끌 수 있다 | 있음: `set/interaction_entity/dummy.inc`, `properties/animal.ext` |
| [Update 59](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1808601382512322) 2025-08-23 V1.054.0 | 읽음 | 개발자 | `{DynCmpPersistent}`, `{mark_for_persistence}`(R1-a와 같음). `vehicle-main.inc` 수위 검사 지연. 새 핀란드 사람 모델 400여 개가 모듈식 | 있음(R1-a) |
| [Update 60](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1811772772409136) 2025-09-30 V1.055.0 | 읽음 | 개발자 | `airborne.inc`·`marine.inc`의 'destroyed' 태그 검사 교체 | `set/interaction_entity/marine.inc` 있음 |
| [Update 62](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1817483467048827) 2025-12-02 V1.057.0 | 읽음 | 개발자 | 화염 입자에 맞은 사람의 "contact" 쿨다운(`human.inc`), 사람 `inflammation_time` 약 50% 증가 | 대조 안 함 |
| [Update 65](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1818752592135405) 2025-12-16 V1.060.0 | 읽음 | 개발자 | `interaction_entity/flora.inc` 자동 시야 막기, `bf109_e4b` define(R1-a와 같음) | 있음 |
| [Update 67](https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1829528821303544) 2026-04-08 V1.062.0 | 읽음 | 개발자 | **`pather.set`·`obstacle.reg`·맵 오브젝트 `.def` 변경은 U67이 처음이다.** docs/09는 이것을 U68(2026-05-07)에만 적었다. U68(V1.063)의 노트는 "기는 동안 넘기를 끄려고 `pather.set`·`obstacle.reg`를 또 고쳤다"이다 | `set/pather.set` 있음 |

이미 docs/09에 있어 뺀 것: U19, U21, U35(playerid 0, 콘퀘스트 설정 파일 이름 변경·분할. 공지에 파일 이름은 없다), U36의 `*.ammo` 태그·`target/ammo.set`·`human.mdl`.

모딩 정보가 없는 키워드 일치(제목만): Dev update #90·#106(모딩 SDK를 "언젠가" 내겠다는 말, 날짜 없음), Mod competition 결과, Open Beta 공지, Devblog #78·#143·#144·#161(모듈식 사람 모델, 에디터 이스터 에그), Dev update #126·#127(봇 구매 파일 구조 개편 예고)·#131·#136(콘퀘스트 지역 선택, 모더가 지역을 더 만들 수 있다), Update 23·24·28(채팅 "Note for modders"는 문의처 안내뿐, U28은 에디터 크로마키 엔티티), Update 35 open beta·Open beta update 2023-11-20(버전 숫자가 같아 리소스 desync가 나면 어느 한쪽이 업데이트 안 한 것), Update 61 open beta(에디터 카메라 키프레임), Content Update 6 공지(이전 버전으로 돌리는 betas 탭).

### R1-c Nexus

| 사이트(주소) | 읽기 | 기준·근거 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|
| [Nexus – Gates of Hell: Ostfront 상위 모드](https://www.nexusmods.com/calltoarmsgatesofhellostfront/mods/top) | 읽음(내장 브라우저. 쿠키 창은 누르지 않고 본문 글만 읽었다. WebFetch는 `/calltoarms/mods/`에 403) | 사용자 업로드(2024-07~2026-10) | 이 게임 항목은 따로 있고(`calltoarmsgatesofhellostfront`) 공개 모드는 14개(번호 1~17, 7·10·13은 비어 있다)뿐이다. 점프, ReShade, Vespia at War, Conquest Survival, ALL Maps in Conquest Mode, 105mm 탄 치트, Noco(MP·쿨다운 0), CHKL Long Range Combat Hardcore, Custom Made mod, CoD2 스킨, Sudden Strike 4 음악, PT-BR·태국어 번역, 4k UI 크기. 설명은 "`mods` 폴더에 풀고 Extras에서 켠다" 정도. 파일·키 설명이나 모딩 가이드는 없다. 게임별 articles 페이지는 404 | 대조할 파일·키 없음 |

정리: Nexus에는 모딩 정보가 없다.

### R2 Steam 가이드

검색 https://steamcommunity.com/app/400750/guides/?searchText=modding 을 trend·toprated·mostrecent 세 정렬로 1~6쪽씩 받았다. 어느 정렬이든 1쪽 30건, 2쪽 24건, 3쪽부터 0건이라 모두 같은 54건이다.

docs/02가 출처로 남긴 가이드는 2건(3629209372 Schwienyy, 3472176545 Denis)뿐이다. docs/02 본문은 "3건을 읽었다"고 하지만 세 번째는 이름이 없다. 아래는 이 2건을 뺀 52건이다. docs/09에서 이미 읽은 2건(2567221405 = S9, 3488308840 = R8)은 표시만 하고 다시 읽지 않았다.

### 제목 목록 (52건)

| id | 제목 | 작성자 | 분류 |
|---|---|---|---|
| 3617051153 | Ostfront. Секретные задания / Secret Objectives | Elva | 공략 |
| 3617051705 | Scorched Earth. Секретные задания / Secret Objectives | Elva | 공략 |
| 3801835773 | Gates of Hell Campaign Editor - How to use guide | Pvt. Matusz | 콘퀘스트 → 본문 읽음 |
| 3775767399 | Indomitus — Interactive Tech Tree (IG / TG) | Manslayer | 특정 모드 |
| 3732761563 | CTA: Gates of Hell / Dawn of the Dead: Mod Mechanics | Tyler | 특정 모드(받았으나 쓸 내용 없음) |
| 3709396415 | [PT-BR] Tradução para Call to Arms - Gates of Hell: Ostfront | NewsFire | 번역 |
| 3690976106 | Indomitus Mechanics Guide | Penguin Of Doom | 특정 모드 |
| 3617050755 | Liberation. Секретные задания / Secret Objectives | Elva | 공략 |
| 3617048862 | Talvisota. Секретные задания / Secret Objectives | Elva | 공략 |
| 3665610432 | Coarmius Madness - Homebrew Gameplay | CoarmiuS | 놀이 규칙 |
| 3661752845 | Несоответствие ресурсов и как с ним бороться / Resource Desynchronization | ArtyomRUS | 코옵 desync·로드 순서 → 본문 읽음 |
| 3626530438 | mod制作 | aabb | 콘퀘스트 AI → 본문 읽음 |
| 3618662453 | Doctrine units in Commonwealth skirmish battles | kampfer91 | 유닛 목록 |
| 3598848056 | Finest Hour. Секретные задания / Secret Objectives | Elva | 공략 |
| 3574060214 | Dawn of the Dead «Survival - Motel» | Tyler | 특정 모드 |
| 3547919258 | (MOD) German liberation skirmishes unit guide | kampfer91 | 특정 모드 |
| 3564691426 | Модификация стрелкового и артиллерийского вооружения | Einfach Funker | 무기 → 본문 읽음 |
| 3526306178 | Changing "Conquest Tweaks" to your own preference | TirolerKaiserjäger | 콘퀘스트 → 본문 읽음 |
| 3503751734 | Jubilee. Секретные задания / Secret Objectives | Elva | 공략 |
| 3488308840 | Guide to Gate of Hell Mod Manager | Noboru | 로드 순서. docs/09 R8에서 읽음 |
| 3460900318 | Хорошие Моды для Гох | Бѣлая Гвардія | 모드 추천 |
| 3358924065 | Horten's Frontline: FL Doctrines [2.8] | GDI Trooper | 특정 모드 |
| 3351785100 | Airborne. Секретные задания / Secret Objectives | Elva | 공략 |
| 3250836311 | Fix for west 81 my version | MEGA_COLT | 특정 모드 |
| 3187234227 | How to restrict units for player/AI in dynamic campaign | JOHN GAMER | 콘퀘스트 → 본문 읽음 |
| 3172070197 | 更新日志/Change Log | Heart of Stone | 특정 모드 |
| 3142522001 | How to cheese infantry in Conquest | nephilimnexus | 콘퀘스트(게임 안 조작) → 본문 읽음 |
| 3133824064 | Editing your conquest save file (Fix red version number or add/remove mods) | ТоЬЫэЯ | 콘퀘스트·모드 → 본문 읽음 |
| 3125222309 | Coop resource desynchronization even with same mods | SkebbZ | 코옵 desync → 본문 읽음 |
| 3105261913 | A Guide to Masochist Mode (Infantry Only) | Jeb T. Firefly | 놀이 방식 |
| 3100107161 | Doctrines units in USA skirmishes | kampfer91 | 유닛 목록 |
| 3045584624 | 征服参数修改 | 133 | 콘퀘스트·유닛·무기 → 본문 읽음 |
| 3032135008 | Some Desperate Glory new update guide | Vasikle | 특정 모드 |
| 3021640568 | Noob guide for PVP multiplayer | JonasIV | 공략 |
| 3021979249 | NORESUS Project - Strategic Map - Local Skirmish | vecchiounno | 외부 도구 |
| 3018184092 | Does it tow? | S-Array 03 | 차량 → 본문 읽음 |
| 3000098433 | How To Screenshots | np7 | 공략 |
| 2919551462 | Doctrines Valour mod | BITcOIN | 특정 모드 |
| 2895735438 | Doctrine units in Finland Skirmishes | kampfer91 | 유닛 목록 |
| 2892683384 | Doctrines units in Germany Skirmishes | kampfer91 | 유닛 목록 |
| 2889608570 | Doctrines units in USSR Skirmishes | kampfer91 | 유닛 목록 |
| 2889691325 | ความรู้เบื้องต้น | Nox | 입문 |
| 2824014751 | FAQ for SPC 3.0, 4.x & 5.x | vecchiounno | 외부 도구 |
| 2796755670 | Manual for SPC (SinglePlayer Creator) 2.0 | vecchiounno | 외부 도구 |
| 2779700692 | Skirmish Combined Arms MOD | Cryptic | 봇 구매(유닛) → 본문 읽음 |
| 2741400123 | Artillery - The Infantry's Best Friend | The Hyena | 공략 |
| 2695557901 | A general guide for beginners in multiplayer | [BGS] Nekoro | 공략 |
| 2691154695 | How to activate cinematic mod in "Call To Arms - Gate of Hell" | Lezard The Only One | 단축키(Shift+Ctrl+Alt+D, docs/09 V8과 같음) |
| 2590733273 | Reinforcements Modders Mod Guide | Tjdagger | 미션 제작(동영상) → 본문 읽음 |
| 2567221405 | Guide for changing Conquest Enhanced v2 to your taste | MrCookie | 콘퀘스트. docs/09 S9에서 읽음 |
| 2524767376 | Gates of Hell Reinforcement Menu - SEE ALL in 1 | O()O | 미션 UI |
| 2515262536 | CtA Gates of Hell: Ostfront Troubleshooting | Delta | 로드·모드 문제 → 본문 읽음 |

### 본문을 읽은 가이드

| 사이트(주소) | 읽기 | 기준·근거 | 쓸 만한 내용 | 바닐라 대조 |
|---|---|---|---|---|
| [Resource Desynchronization (러시아어)](https://steamcommunity.com/sharedfiles/filedetails/?id=3661752845) | 읽음 | GoH, 사용자(2026-02-07) | "리소스 불일치"의 원인: 모드 버전이 다름(마지막 갱신 시각을 초 단위까지 비교), 로드 순서가 다름(크래시도 남), 게임 버전이 다름, 드물게 게임 `resource` 아래 pak이 아닌 폴더가 남아 있으면 폴더만 지우고 무결성 검사. MrCookie's Mod Manager(2986408240)는 구독만 하고 게임에서 켜지 않는다. `options.set` 위치는 `Documents\My games\Gates of hell\profiles\<번호>` 또는 `AppData\Local\digitalmindsoft\gates of hell\profiles\<번호>`. 로드 순서 예: Hotmod 1968 → West 81 → Code:X → 그 하위 모드 | 게임 파일 대조 대상 아님. 프로필 위치 두 곳은 docs/02의 불일치 기록과 맞다 |
| [Coop resource desynchronization even with same mods](https://steamcommunity.com/sharedfiles/filedetails/?id=3125222309) | 읽음 | GoH, 사용자(2023-12-28) | "Last updated"가 "Subscribed on"보다 늦은 모드는 구독을 끊고 다시 구독해 강제로 갱신. 안 되면 모두 끊고 다시 구독 | 해당 없음 |
| [mod制作 (중국어)](https://steamcommunity.com/sharedfiles/filedetails/?id=3626530438) | 읽음 | GoH, 사용자(2025-12-18) | `gamelogic.pak`을 `mods\<이름>\resource\`에 복사해 고친다. `script\units\<진영>\conquest.eng.lua`의 `priority`(0.00~1.00, 0이면 AI가 안 산다). 바꾼 파일만 남기면 된다(같은 경로의 파일이 덮어쓴다) | 다름: 지금은 `script/multiplayer/units/<진영>/conquest.<진영>.lua`이고 `priority`가 0개다(2026-10-16 확인: `conquest.<진영>.lua` 다섯은 0개, 같은 폴더의 `early`·`mid`·`late.<진영>.lua`에는 파일마다 100개 넘게 남아 있다). 항목이 `availability`·`base`·`grade`·`posture`·`roles`로 바뀌었다(U70 로스터 스키마, docs/09). `early/mid/late.<진영>.lua`(MP 봇용)에는 `priority`가 그대로 있다 |
| [Модификация стрелкового и артиллерийского вооружения (러시아어)](https://steamcommunity.com/sharedfiles/filedetails/?id=3564691426) | 읽음 | GoH, 모더(2025-09-08) | `set/vision/vision.set`의 `(define "radius" (mod "mp" {radius (* 1.0 %r)}) ...)` 숫자가 게임 전체 시야 배율이다. 로그는 `Documents\my games\gates of hell\log\game.log`, `eSDLReader` 오류는 대부분 문법(괄호). 에디터에서 F3 → Mission → Reload로 압축 안 한 모드 파일을 다시 읽는다고 함. **하지만 예시 키 상당수가 지어낸 것으로 보인다** | 있음: `vision.set` 2~4행이 글과 같다(바닐라는 1.0, 주석 "This value changes the global vision ranges of the game"). `accuracy_spg`, `range_180_howitzer`, `sway_rifle`, `recoil_rifle_semi`, `"accuracy_rifle"`, `projectileDamageTable`, `small_firearms_accuracy.inc`도 있다. 없음: `armor_health`, `spotting_range`, `unit_detection_range`, `infantry_modifier`, `rifle_shot_range`, `"max_speed"`, `engine_power`, `"main_gun"`, `caliber_7_62_soviet`, `mod.json`(모드 설명 파일은 `mod.info`, docs/09 R9). 이 가이드의 spotting·hearing·차량 예시는 쓰지 않는다. F3 Reload는 미확인 |
| [Changing "Conquest Tweaks" to your own preference](https://steamcommunity.com/sharedfiles/filedetails/?id=3526306178) | 읽음 | GoH(Conquest Tweaks 모드 3456859082 기준), 사용자(2025-07-15) | `set/dynamic_campaign/resources_<low·standard·high·very_high>.set`: `GlobalMaxCP`(예비 병력 한도), `StageCP`, `SpecialCP`(공습·포격), `{Bots {CP {Base "0:150"}}}`(AI 병력), `PaybackFactor`, `SellFactor`, `ScavengedVehicleSellFactor`, `RiskFactor`의 `BotResources`·`BotVeterancy`·`Rewards`, `MP`·`SP`·`AP`·`RP`의 `StartVal`·`WinGain`·`LoseGain`·`MapRewards`. `set/difficulty/*`의 `{any {health} {accuracy}}`·`{ally ...}`. `script/multiplayer/modes/conquest.lua`의 `AttackMin`·`AttackMax`(분×60×1000), `DCGWaveOffMin/Max`, `DCGMin/Max`, `WaveUnit {Min Max}` | 있음: `values.set`이 `GlobalMaxCP`·`SpecialCP`·`SellFactor`·`ScavengedVehicleSellFactor`·`BotResources`를 설명한다. `resources_standard.set`은 `GlobalMaxCP 550`, `SpecialCP 50`, `SellFactor 0.6 0 0 0`. `set/difficulty/heroic.dl`에 `{any`·`{health 0.7}`·`{accuracy 1.0}`·`{ally`. 다름: `AttackMin`·`DCGWaveOffMin`·`DCGMin`은 지금 `script/multiplayer/modes/conquest/runtime_config.lua` 75~86행에 있고(`AttackMin = 1 * 1000`), `WaveUnit`은 없고 `MinWaveUnits = 1`이 있다 |
| [征服参数修改 (중국어)](https://steamcommunity.com/sharedfiles/filedetails/?id=3045584624) | 읽음 | GoH, 사용자(2023-10, 2024-02 수정) | 위와 같은 `resources_*.set` 키에 더해 `Budget {Start} {PointsPerSecond} {Limit}`, `RoundMultiplier`(AI CP 배수), `{Bots {ResearchStages}}`. 차량 `.def`의 `{box {item ...}}` 탄약, `{place "gun" {weapon ...}}`. `set/stuff/gun`의 `("accuracy_indirect" r1 r2 rmax range_ap range_heat range_he)`. `units_<진영>.set`의 `min_stage`·`max_stage`·`cw`·`cp`·`{cost}`. breed `{perks ("rifle_skill_rank_1")}`·`{veterancy ("veterancy_lvl_0")}`·`{inventory {item "k98" filled}}`. `properties/vehicle.ext`의 `ManualAccuracy`, `set/breed/presets/perks/skills.inc`의 `skilled_repair`·`basic_repair`, `properties/resupply.inc`의 `radius`·`resupplyPeriod`·`regenerationPeriod`·`limit`(0 = 무한), `interface/action/fortify.set`의 `entrenchment_rotate ... time(15)`. 콘퀘스트 세이브 `status`의 `mapPoints` | 있음: `RoundMultiplier`·`PointsPerSecond`·`{Limit`(`resources_*.set` 5개), `ResearchStages`(`values.set` 설명), `ManualAccuracy 1.00`(`vehicle.ext` 20행), `skilled_repair`·`basic_repair`(`skills.inc` 73·77행), `resupply.inc` 803~806행(`{limit 50}` 주석 "0 = unlimited"). `min_stage`·`max_stage`·`cp(`는 `units_ger.set`에 있다. `fortify.set`은 풀린 폴더에 없어 미확인. 항목 4(`script/multiplayer/rus`의 `priority`)는 위와 같은 이유로 다름 |
| [How to restrict units for player/AI in dynamic campaign](https://steamcommunity.com/sharedfiles/filedetails/?id=3187234227) | 읽음 | GoH, 사용자(2024-03-16) | AI: `conquest.<진영>.lua`에서 줄을 지운다. 플레이어: `units_<진영>.set`에서 보병 분대 줄을 지우거나 차량에 `{not_for_player_sale 1}`. 연구 트리 `unit_research_<진영>.set`에서 지우면 `units_` 쪽도 함께 지워야 하고, 그 유닛을 요구 조건으로 쓰는 다른 유닛을 바꾸지 않으면 연구 창을 열 때 꺼진다. 경로: `resource\script\multiplayer\<진영>`, `resource\set\dynamic_campaign`, `resource\set\multiplayer\units\conquest` | 있음: `{not_for_player_sale 1}`은 `set/multiplayer/units/conquest/units_ger.set` 200·399·543행 등. 다름: lua 경로는 지금 `script/multiplayer/units/<진영>/`. 크래시 주장은 미확인 |
| [Editing your conquest save file](https://steamcommunity.com/sharedfiles/filedetails/?id=3133824064) | 읽음 | GoH, 사용자(2024-01-07) | Steam 클라우드를 끄고 `profiles\<번호>\campaign\*.sav`를 7-Zip으로 열어 `status`의 게임 버전과 `mod_<ID>:0` 목록을 고친다. 유닛·연구 트리를 바꾸는 모드는 중간에 넣거나 빼면 안 된다. 고친 뒤 게임 Extras에서도 같은 모드를 켜야 로드된다 | 세이브라 대조 못 함. `mod_<ID>:0` 형식은 docs/02의 `options.set` 기록과 같다 |
| [How to cheese infantry in Conquest](https://steamcommunity.com/sharedfiles/filedetails/?id=3142522001) | 읽음 | GoH, 사용자(2024-01-18) | 전투 끝 "계속"에서 X로 장비를 바꿔 주면 본부로 돌아가도 남는다. 파일 수정 없음 | 해당 없음 |
| [Gates of Hell Campaign Editor](https://steamcommunity.com/sharedfiles/filedetails/?id=3801835773) | 읽음 | GoH, 모더(2026-09-14, 09-25 수정) | 콘퀘스트 `.sav` 편집 도구(창작마당 exe, GitHub `GoH-Campaign-Editor`). 분대원 옮기기, 실제 든 무기에 맞춰 보급, 차량 번호·피탄 자국. 죽은 분대원은 `0xffffffff`. 세이브 위치 두 곳(Documents·AppData) | 대조 못 함(세이브) |
| [Does it tow?](https://steamcommunity.com/sharedfiles/filedetails/?id=3018184092) | 읽음 | GoH, 사용자(2023-08, 2024-05 수정) | 진영별로 견인할 수 있는 차량 목록(예: kfz 15와 Kettenkrad는 탄약 트레일러·3.7cm 대전차포만). 파일·키 없음 | 대조 안 함 |
| [Skirmish Combined Arms MOD](https://steamcommunity.com/sharedfiles/filedetails/?id=2779700692) | 읽음 | GoH, 사용자(2022-03, 2025-09 수정) | `script/multiplayer/bot.data.purchase.standard.lua`에서 주석 처리된 HMG·AA 구매 줄을 살린다. `bot.lua` 185~187행(아군 전차 여부 검사)을 주석 처리. `bot.data.lua` 4·5행이 봇 유닛 생성 간격 최소 10초·최대 30초 | 다름: 풀린 `script/multiplayer/`에는 `bot.main.lua`만 있고 이 세 파일이 없다. 지금 버전에서 이름이 바뀌었는지 미확인 |
| [Reinforcements Modders Mod Guide](https://steamcommunity.com/sharedfiles/filedetails/?id=2590733273) | 읽음 | GoH, 모드 제작자(2021-09) | 동영상 링크뿐(미션 파일을 맵에 놓기, 증원 단추 변수, 미션 파일 합치기) | 해당 없음 |
| [CtA Gates of Hell: Ostfront Troubleshooting](https://steamcommunity.com/sharedfiles/filedetails/?id=2515262536) | 읽음 | GoH, 작성자 Delta(2021-06, 2025-02 수정). 개발사 메일과 문제 해결 도구 링크를 안내하므로 개발사 쪽 사람일 수 있으나 확인 못 함 | `[NO SDL]` 오류는 실행 옵션 `-reset_cloud_profiles`와 `profiles` 폴더 이름 바꾸기로. 프로필이 `%localappdata%`에 있을 수 있다. 대부분의 오류는 낡거나 서로 충돌하는 모드에서 온다 | 게임 파일 대조 대상 아님 |
| [Dawn of the Dead: Mod Mechanics](https://steamcommunity.com/sharedfiles/filedetails/?id=3732761563) | 받음(훑어봄) | 특정 모드 사용자(2026-06) | 모드 게임 방식 설명. 파일·키 없음 | 해당 없음 |

### 읽지 못한 것

- R2: 쓸 범주(유닛·무기·차량·콘퀘스트·시야·로드 순서·창작마당 게시·desync)에 들지 않아 제목만 본 가이드 35건(위 목록에서 "본문 읽음"이 아닌 것). 창작마당 게시(업로드 절차)만 다룬 가이드는 54건 안에 없다.
- R2: 가이드 페이지에 "삭제·호환 안 됨" 표시가 실제로 붙었는지는 판단하지 못했다. 페이지 소스에 그 문구가 들어 있으나 모든 페이지에 같은 틀로 들어 있어 표시 여부를 가를 수 없었다.
- R2: docs/02가 읽었다는 가이드 3건 중 세 번째는 출처 목록에 없어 무엇인지 모른다.
- R1-c: WebFetch는 403. 내장 브라우저로 읽었고 쿠키 동의 창은 누르지 않았다. 로그인이 필요한 Nexus 포럼은 보지 않았다.
- 바닐라 대조: `obstacle.reg`, `interface/action/fortify.set`, `bot.data*.lua`, 맵 `.info`는 `_work\vanilla`에 풀려 있지 않아 대조하지 못했다.

## 읽지 못한 것

- S12: 글이 지워졌다.
- V4: CHANGE-LOG 2·4·5쪽은 2026-10-16에 읽었다("추가 조사"). 1~6쪽 모두 읽었다.
- R1·R2: 디스코드 채널은 로그인해야 보인다. 로그인하지 않았다.
- R12: 2026-05 이전 공지는 2026-10-16에 키워드(modder, .set, properties/ 등)가 든 43건의 해당 문단을 읽었다. 키워드가 없는 공지 본문은 읽지 않았다.
- fandom(S1~S3, R4~R6)과 ModDB(R3, R13, R14)는 WebFetch가 막혀(402·403) 내장 브라우저로 읽었다. 봇 확인 화면은 나오지 않았다.
