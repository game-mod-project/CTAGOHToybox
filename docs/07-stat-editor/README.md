# 스탯 편집기

기준: 2026-10-15, 게임 v1.067.0. 설계는 [../superpowers/specs/2026-10-12-stat-editor-design.md](../superpowers/specs/2026-10-12-stat-editor-design.md)(유닛 스탯)와 [../superpowers/specs/2026-10-15-settings-editor-design.md](../superpowers/specs/2026-10-15-settings-editor-design.md)(게임 설정)에 있다.

명령줄 도구(`ctatoybox`)와 화면(`CtaToybox.exe`, 아래 "화면" 절)이 있다. 유닛 스탯과 함께 게임 설정(컨퀘스트·스커미시·난이도, 아래 "게임 설정" 절)도 고친다.

## 할 수 있는 것과 없는 것

| 할 수 있다 | 아직 없다 |
|---|---|
| 유닛을 경로나 게임 속 이름으로 찾기 (`units`, 화면) | |
| 유닛의 값이 어느 파일의 어느 자리에서 왔는지, 몇 개 유닛이 그 파일을 같이 쓰는지 보기 (`trace`) | |
| 유닛이 쓰는 무기·탄약 파일을 유닛과 함께 보기 (화면의 무기·탄약 탭) | |
| 게임 설정(컨퀘스트 자원·길이·전투 규칙·AI, 스커미시 모드, 난이도) 고치기 | 맵 목록, 연구 트리 구조, 세이브 파일 고치기 |
| 원본 파일(바닐라와 켜 둔 모드)의 값 하나를 바꾸는 수정을 프로젝트 파일에 담기 | 매크로 본문 안의 값을 "이 유닛만" 고치기 |
| include로 공유된 값을 "이 유닛만" 고치기 (`add --scope unit`) | 키나 블록을 빼기 |
| 블록 끝에 줄 하나 더하기, 아이템·무기 파일을 새 이름으로 사본 만들기 (`add --kind add-line`, `add --kind file-copy`, 값 고치기 창) | |
| 프로젝트를 빌드해 `mod/CTAToybox/resource`에 써넣기 | |
| 게임이나 모드가 갱신돼 원래 값이 달라진 수정을 알아채기 | |

게임 폴더와 창작마당 폴더, 프로필의 `options.set`은 읽기만 한다.

## 처음 한 번

저장소 루트에서 실행한다.

```powershell
$profile = Get-ChildItem "$env:LOCALAPPDATA\digitalmindsoft\gates of hell\profiles" -Directory | Where-Object Name -ne 'offline' | Select-Object -First 1
dotnet run --project editor/CtaToybox.Cli -c Release -- init --game "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell" --workshop "E:\SteamLibrary\steamapps\workshop\content\400750" --options "$($profile.FullName)\options.set" --out "E:\CTAToybox\mod\CTAToybox"
```

`editor/settings.json`이 생긴다. 이 파일은 git에 넣지 않는다.

## 값 하나를 고치기

아래에서 `ctatoybox`는 `dotnet run --project editor/CtaToybox.Cli -c Release --`를 줄여 쓴 것이다.

1. 원본 구성을 본다.

   ```
   ctatoybox sources
   ```

2. 고칠 파일의 값을 본다. 어느 파일에 어떤 값이 있는지는 [../06-editable-attributes/README.md](../06-editable-attributes/README.md)에서 찾는다.

   ```
   ctatoybox show entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --filter box/item
   ```

   한 줄이 한 행이다. `경로 [몇 번째] 0=값 1=값`.

3. 수정을 더한다. `--occ`는 대괄호 안의 수, `--arg`는 값 앞의 수다. 둘 다 생략하면 0이다. 문자열 값은 따옴표까지 적는다.

   ```
   ctatoybox add --project editor/projects/my-tweaks.json --file entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --path "/game_entity/extender[inventory]/box/item" --occ 0 --arg 1 --new 7
   ```

   `add`는 원본에 미리 적용해 보고 안 되는 값을 거부한다. 블록 이름 뒤의 한정자(`extender[inventory]`의 `"inventory"`)나 숫자로 시작하는 행의 첫 값을 낱말로 바꾸는 것처럼 키 경로가 바뀌는 값이 그렇다. 더한 수정을 지울 때는 같은 자리로 `remove`를 쓴다. 빌드는 프로젝트 하나만 쓰므로, 다른 프로젝트(예: 월드워)의 수정을 함께 넣으려면 `merge --project <합칠 프로젝트> --from <가져올 프로젝트>`로 합친다(앱은 "다른 프로젝트 합치기..."). 같은 자리에 다른 값이 있으면 합칠 프로젝트의 값을 두고 충돌로 알린다. 그 자리에 수정이 여럿이면 `--scope shared`('전부')나 `--unit <유닛 가상 경로>`(그 유닛의 '이 유닛만')로 고른다.

   ```
   ctatoybox remove --project editor/projects/my-tweaks.json --file entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --path "/game_entity/extender[inventory]/box/item" --arg 1
   ```

4. 게임을 끄고 빌드한다. 게임이나 에디터가 실행 중이면 `build`는 빌드하지 않고 오류로 끝난다. 그래도 빌드하려면 `--force`를 붙인다.

   ```
   ctatoybox build --project editor/projects/my-tweaks.json
   ```

5. 게임을 다시 시작한다. 모드 목록에서 `CTAToybox`가 켜져 있어야 한다.

게임이 실행 중일 때 빌드하면 안 되는 이유: 실행 중인 게임은 켤 때 본 모드 파일을 나중에 다시 읽는다. 2026-10-14에 게임이 켜진 채로 빌드해 확인용 수정 파일(`set/stuff/mgun/hmgun_usa.ammo`)이 지워졌다. 그 뒤 스커미시를 새로 시작하자 게임이 그 파일을 읽다가 `Block start expected! (eSDLReader.cpp:644)`로 꺼졌다(같은 맵의 첫 판은 정상, `game.log`).

## 화면 (CtaToybox.exe)

```
dotnet run --project editor/CtaToybox.App -c Release
```

처음 실행하면 경로를 묻는 설정 창이 뜬다(명령줄의 `init`과 같다). 프로젝트는 기본으로 `editor/projects/my-tweaks.json`이고 위쪽 "열기..."로 바꾼다. 유닛 색인이 없으면 처음 한 번 만든다(2026-10-14 이 PC에서 유닛 21,612개).

- **왼쪽 유닛 찾기**: 게임 속 이름이나 파일 이름으로 찾고, 종류(탱크, 포, 차량, 기타 탈것, 보병, 무기·탄약·장비)와 출처(바닐라, 모드 이름)로 거른다. 차량 `.def`는 include하는 `properties/*.ext`로 나눈다: `cannon*.ext`면 포, `tank.ext`·`tank_assaultgun.ext`·`tank_unarmed.ext`면 탱크(자주포도 여기에 든다), `car.ext`·`armoredcar.ext`·`halftrack.ext`·`moto.ext`·`bicycle.ext`면 차량, 나머지(항공기·배·열차, `_x` 파생 파일)는 기타 탈것이다. 이름순으로 500개까지 보인다. 보병 외형(`entity/humanskin`), 아이템 모델(`entity/inventory`), 아이템 틀(`.pattern`)은 목록에 없다. 펼친 값에 수가 하나도 없는 파일(예: `{from "75mm_pak39"}` 한 줄짜리 별칭, `from`·`tag`·`entity`만 있는 헬멧)도 고칠 스탯이 없어 뺀다. 2026-10-14 데이터로 무기·탄약·장비 4,074개 가운데 1,723개(헬멧 1,662개)가 빠졌다. 같이 쓰는 유닛 수는 빠진 파일까지 센다. x 폴더의 `_x`, `_xx` 파일은 기본으로 숨긴다. `_xx`는 폭발로 완전히 부서진 잔해다(바닐라 확인: `set/interaction_entity/vehicle/vehicle-main.inc`의 `on destroy`가 차체 파괴 때 `explosion`을 부르고, 그 안에서 `{set_skeleton "_xx" sync}`. 엔진 파괴로 불탈 때는 바꾸지 않는다). `_x`는 부품이 부서졌을 때 그 부품 모양을 빌려 오는 손상 모델로 보인다(GoH 스크립트에는 바꾸는 줄이 없고, Men of War II 개발사 문서와 `_x.mdl`의 `break` 애니메이션이 근거. 미확인). `_x.def` 806개에는 장갑·스탯이 하나도 없다. `_xx.def`에는 장갑 볼륨이 있지만 본체와 따로 적힌 값이라, 본체 장갑을 고쳐도 잔해에는 반영되지 않는다. 자세한 근거는 [../09-modding-sources.md](../09-modding-sources.md)의 R19.
- **이름**: 게임과 켜 둔 모드의 현지화(`localizations/default`)에서 읽는다. 차량은 `desc/entity/<파일 이름>`, 보병은 `desc/human/<set/breed 아래 경로>`, 무기·탄약은 파일 종류에 따라 `desc/weapon|ammo|stuff/<파일 이름의 낱말>`이다. 2026-10-14 데이터로 확인했고, 이름이 붙는 비율은 차량 98%, 보병 100%, 무기·탄약 95%다. 출처의 모드 이름은 각 모드의 `mod.info`에서 읽는다.
- **요약 탭**: 종류별로 고른 항목을 짧은 이름으로 묶어 보인다. 고른 항목과 이름은 `editor/summary/<종류>.psv`(`경로|이름`)에 있다(차량, 보병, 무기·탄약). 같은 항목이 여러 부위에 있으면 줄마다 부위(예: `volume body1`, `place gun`)를 붙이고, 6줄이 넘는 묶음은 접어 둔다. 묶음 머리를 눌러 펼치거나 접은 상태와 스크롤 위치는 값을 고쳐도 그대로 남고(요약·전체·가격·로스터 탭), 다른 유닛을 열면 처음으로 돌아간다. 키의 설명과 등급(확인·문서·추정)은 줄마다 보인다. 따옴표 이름 줄(`{"health" "200%"}`)은 펼친 경로가 `/breed/perks/"*"` 하나로 묶이므로, 요약 키·키 주석·값 이름에 이름을 넣은 경로(`/breed/perks/"health"`)를 쓰면 이름별로 나뉜다. 이름 경로가 `"*"` 경로보다 먼저다. 보병 요약은 체력, 체력 회복, 스태미나, 스태미나 회복, 이동 속도, 무기 기술, 회피, 소생 설정, 시야 순으로 보인다. 이 값들은 대부분 `set/breed/presets/perks/*.inc`의 tier 매크로 안에 있어 '전부'로만 고쳐지고, 그 매크로를 쓰는 병종이 모두 바뀐다(2026-10-15 바닐라에서 6,111개).
- **전체 탭**: 펼친 값을 원본 파일별로 묶어 보인다. 유닛 파일, 나머지 파일, 매크로 호출, 고칠 수 없는 값 순서다. 키는 읽히는 표기(`inventory › box › item`)와, 그 아래 원본에 쓰인 그대로의 키 경로(`키 /from/parameters/spreading/radiusTable/#row`, 대소문자 포함)로 보인다. 같은 원본 자리를 가리키는 줄이 여러 번 펼쳐지면(예: 매크로 인자 하나가 탄종마다 같은 사거리 표에 들어감) 한 줄로 모으고 `같은 줄 N곳`을 적는다. 무기의 `parameters "ap2"` 같은 탄종 묶음은 부위처럼 줄마다 적는다. "매크로 호출" 묶음에서 계산에 쓰이는 인자(예: 연사 속도 `rpm`)를 고친다. `(mod "mp" …)` 같은 조건 안의 줄에는 "조건: 멀티플레이에서만"처럼 조건을 보인다. 이름 앞에 `##`가 붙은 꺼진 블록의 값은 게임이 쓰지 않으므로(개발사 문서) "고칠 수 없는 값" 묶음에 들어간다. `%이름` 매개변수와 쓰는지 알 수 없는 매크로 정의 안의 호출은 보이지 않거나 고칠 수 없다. 따옴표 이름 블록의 이름 자체(`{"human" …}`의 `human`, `{"head" 1.3}`의 `head`)도 다른 곳이 그 이름으로 부르므로 고칠 수 없다. 인자 없는 매크로 호출(병종의 `("tier2_standard")`, `("veterancy_lvl_0")` 등)은 "매크로 호출" 묶음에 이름 하나를 값으로 보인다. 그 이름은 같은 파일에 정의된 다른 매크로로만 바꿀 수 있다(창에 고를 수 있는 이름이 보인다. 보이지 않는 이름을 쓰면 게임이 시작할 때 꺼진다). 숫자만 다른 같은 갈래가 아니면 경고한다(E4, 명령줄은 `add --kind macro-name`). 위의 "거르기"로 줄인다.
- **가격·로스터 탭**: 콘퀘스트·배틀존 등록 파일(`set/multiplayer/units/**/*.set`)과 연구 트리(`set/dynamic_campaign/unit_research_*.set`)에서 이 유닛의 항목을 보인다. 항목은 따옴표 이름 블록이고, 이름이 차량 `.def`의 파일 이름(예: `panzer5g`)이나 breed의 `set/breed` 아래 경로(예: `mp/ger/mid/nco`)와 같으면, 또는 항목 안에 `vehicle(이름)`이 있으면 이 유닛의 것이다. 분대 매크로의 인자도 본다: 분대 틀이 병종을 `content "mp/%side/%period/%c1 …"`로 만들므로 `c1(ersatz_sl:1)`·`crew1(tankman:3)`는 `side()`·`period()`와 합쳐 병종 `mp/ger/mid/ersatz_sl`로, `v1(이름)`은 차량으로 잇는다. 따옴표 이름 블록 없이 파일 맨 위에 쓴 분대 호출(`("squad_with3types_doc" side(ger) name(squad_ersatz_rifle_mid_all) …)`)도 항목이고, 이름은 틀이 만드는 것처럼 `squad_ersatz_rifle_mid_all(ger)`이다. 값은 원문처럼 `cp(40)`, `lvl(lvl1 lvl2)`로 보인다. 고치면 그 항목만 바뀌므로 "이 유닛만"은 없다. 항목 이름(분대 호출은 `name()`)과 `requires`·`costs` 같은 열쇠말은 고치지 않는다. 따옴표 이름 블록(로스터 항목 등) 안의 수정은 그 이름이 든 경로도 저장한다. 모드가 앞에 항목을 끼워 넣어 "몇 번째"가 밀리면 프로젝트를 열 때 이름으로 다시 맞추고, 그 이름이 원본에서 사라지면 수정 목록에 "원본에 이 이름의 항목이 없다"로 보이며 빌드하지 않는다. 난이도별 자원(`resources_<난이도>.set`)처럼 유닛에 딸리지 않은 설정은 종류 "게임 설정"에서 고친다(아래 "게임 설정" 절).
- **무기·탄약 탭**: 유닛이 이름으로 가리키는 무기·탄약과 이어진 파일, 이은 규칙이다. 파일로 이어진 것은 두 번 눌러 연다. 이름의 낱말을 모두 가진 후보가 한 폴더에 여럿이면 게임은 이름순(점이 글자보다 앞) 맨 앞 파일을 쓰므로 그 파일로 잇는다(2026-10-16 게임 확인). "후보"와 "파일 없음"은 게임이 어느 파일을 쓰는지 확인하지 않은 이름이다(규칙의 근거는 `docs/02-modding-research.md`의 "아이템 이름과 파일").
- **값 고치기**: 창의 제목은 항목의 한글 이름이고, 값마다 뜻(예: `최고 속도 (km/h)`, `아이템 이름`, `수량`)과 원문 이름·자리·원래 값을 보인다. 값의 뜻은 `data/annotations/values.psv`에서 읽고, 없으면 "뜻 미확인"으로 적는다. 표의 "지금 값" 칸도 뜻을 아는 값은 한 줄에 하나씩 "뜻: 값"으로 보인다. 전체 탭은 줄마다 키 주석에서 뽑은 한글 이름을 굵게, 그 아래 원문 키를 보인다(뜻을 모르면 "이름 미확인"). 값을 두 번 누른다. 공유되는 값이면 영향받는 유닛 목록과 "전부 / 이 유닛만"이 나온다. "이 유닛만"이 안 되는 자리는 선택지가 꺼지고 이유가 보인다. 같은 자리를 다시 고치면 수정이 바뀐다(쌓이지 않는다). 숫자였던 자리에 숫자가 아닌 값을 넣거나 0보다 큰 값을 음수로 바꾸면 창 아래에 "경고:"가 보이지만 막지는 않는다(명령줄 `add`도 같다).
- **오른쪽 수정 목록**: 유닛별로 묶인다. 체크로 끄고 켠다. "삭제"하면 값이 원본으로 돌아간다. "원본이 바뀜"이 뜬 수정은 "지금 값에 맞추기"로 원래 값을 지금 원본 값으로 바꾸거나 삭제한다. 제목은 줄 이름과 값의 뜻이고, 따옴표 이름 블록 안의 값이면 블록 이름을 괄호로 붙인다(예: `수평 시야각 (human_elite): 150 → 360`). 목록은 빌드와 같은 규칙으로 미리 점검한다: 새 값 규칙, 파일 인코딩, 키 경로를 바꾸는 예전 수정(블록 이름 뒤의 한정자나 숫자 행의 첫 값), 출력 폴더에 이 도구가 만들지 않은 같은 경로의 파일("이 유닛만"이면 유닛 파일과 사본도 본다). 걸리면 "적용 불가"와 이유가 보인다.
- **빌드**: 게임이나 에디터가 실행 중이면 꺼질 수 있다고 경고하고 묻는다(기본 선택은 "아니요"). 결과를 창으로 보인다.

화면 확인용 옵션: `--unit <가상 경로> --tab summary|full|items --filter <글자> --edit-row <항목 이름> --screenshot <png>`는 그 화면(또는 그 줄의 값 고치기 창)을 PNG로 저장하고 끝낸다. `--shots <목록 파일>`은 한 번 실행으로 여러 화면을 찍는다(한 줄: `파일 이름|유닛|탭|거르기|값 고치기 창 항목`). `--project <파일>`로 프로젝트를 고른다. `--scenario <파일>`은 값 고치기 창에 값을 쓰고, 범위를 고르고, 넣기를 누르고, 수정 목록의 체크를 바꾸거나 지우는 조작을 차례로 하며 화면을 찍는다(명령 목록은 `MainWindow.Scenario.cs` 머리 주석). 반드시 `--project`로 임시 프로젝트를 준다.

### 게임 설정 (하위 프로젝트 B)

종류에서 "게임 설정"을 고르면 컨퀘스트 자원(`resources_*.set`), 컨퀘스트 길이(`duration_*.set`), 컨퀘스트 전투 규칙(`campaign_capture_the_flag.set`), 컨퀘스트 AI(`runtime_config.lua`), 스커미시 모드(`set/multiplayer/games/*.set`), 난이도(`set/difficulty/*.dl`), 시야(`set/vision/vision.set`과 그 include. 필드의 각도·근접 발견 거리와 거리 매크로 인자 r을 고친다. 전체 배수 `radius`는 식 안의 값이라 화면에서 고치지 못한다)가 나온다. 게임 설정 파일의 따옴표 이름 블록(예: 시야 필드 `tank_range_200`)은 값 고치기 창의 "이 블록 복제"로 새 이름의 사본을 만들 수 있다(E18). 사본은 같은 부모의 마지막 형제 뒤에 들어가고, 전체 탭에 바로 보이며 그 안의 값을 다른 값처럼 고친다. 다른 파일이 그 새 이름을 부르게 하면(예: 유닛의 `{vision "ctb_tank_light"}`) 그 사본이 쓰인다. 복제를 끄거나 지우면 그 이름을 쓰는 수정은 점검·빌드가 "필드 …가 없다"로 막는다. 명령줄은 `add --kind block-copy --path <이름 줄 경로> --new "<새 이름>"`. 월드워 차종 프로젝트는 `vision-preset --variant wot --classes --project editor/projects/월드워-차종.json`이 만든다(경전차·중형전차·차량의 시야와 탐지를 중전차보다 길게, 설계 `docs/superpowers/specs/2026-10-16-e18-class-vision-design.md`). 이름은 `editor/settings/files.psv`, 키의 뜻은 `data/annotations/settings.psv`(그리고 `common.psv`의 정확한 경로와 `**/꼬리` 줄. `common.psv`의 `*/이름` 줄은 유닛 파일용이라 게임 설정에는 붙지 않는다), 요약 키는 `editor/summary/settings.psv`에 있다. 설계는 [../superpowers/specs/2026-10-15-settings-editor-design.md](../superpowers/specs/2026-10-15-settings-editor-design.md).

- 수정은 항상 "전부"다. 파일 하나가 곧 설정이다. 가격·로스터와 무기·탄약 탭은 숨긴다. 값이 모두 문자열 단계표인 파일(`duration_*.set`)도 목록에 나온다.
- `"이름=값;이름=값"` 문자열(게임 모드의 `parameters`, `unitLimits`)은 조각마다 칸이 있다. 조각 이름·개수·순서는 바꿀 수 없다. 조각의 뜻은 `values.psv`의 자리 `@이름`. 표의 "수정" 칸과 수정 목록에는 바뀐 조각만 보인다.
- `"1:200 4:250"` 같은 판 수 단계표는 값 고치기 창 아래에 풀이를 보인다(`1 → 200, 4 → 250`). 칸마다 수의 개수가 원래와 같아야 한다. `FlagCount`는 한 칸이 한 판이므로(`duration_*.set` 주석 "each entry is 1 game played") `1~9판 2/5, 10~24판 3/5`처럼 판 순서로 풀고 같은 값이 이어지는 판은 묶는다.
- 키 끝이 같은 줄(시작 자원의 MP·RP, 맵 보상의 `MapRewards/Airfield` 등)은 서로 구분되는 데까지 상위 블록 이름을 붙인다(`MP`, `MP/MapRewards`, `CPLimitStages/#1`).
- 컨퀘스트 AI(Lua)는 이 파일에 나오는 꼴만 읽는다(2026-10-15 바닐라에서 값 324개, 읽지 못한 것 0). 수식은 계산값을 함께 보이고, 개발사 영어 주석을 "원문 주석"으로 보인다(위 줄 주석과 줄 끝 주석이 다 있으면 " / "로 잇는다). 새 값은 원래와 같은 종류(수·수식, 문자열, 참·거짓)여야 하고 `nil`은 고칠 수 없다. 모르는 문법이 있으면 "이 Lua 파일은 읽지 못해 고칠 수 없다: 이유"로 보이고 고치지 못한다.
- 멀티플레이는 모든 사람이 같은 모드를 써야 한다. 설정 파일을 바꾼 모드는 다른 사람과 어긋날 수 있다.
- 게임에서 확인했다(2026-10-15, [ingame-check.md](ingame-check.md)): 컨퀘스트 자원 시작값은 새 캠페인에 들어간다(MP 9137, RP 37). 모드의 낱개 Lua 파일은 바닐라와 앞선 모드(Wunderwaffe)의 Lua를 덮어쓴다(`targetWaveOffSeconds= 1210`). 스커미시의 `parameters`·`unitLimits` 조각은 아직 게임에서 보지 않았다. 진행 중인 캠페인에 반영되는지도 확인하지 않았다.

## 유닛에서 시작해 고치기

1. 유닛을 찾는다. 처음 한 번은 유닛 색인을 만든다(2026-10-13 이 PC에서 유닛 21,612개, 8.2초). 결과는 `editor/cache/`에 저장되고, 모드를 켜고 끄거나 모드가 갱신되면 다시 만든다.

   ```
   ctatoybox units --find m3a1_halftrack --kind vehicle
   ```

   한 줄이 유닛 하나다: `종류  계층  경로  표시 이름`. 앱의 목록과 같이 `vehicle`, `breed`, `stuff` 가운데 값에 수가 있는 파일만 보인다. 표시 이름은 `data/display-names.csv`에 `desc/entity/<파일 이름>` 키가 있는 엔티티에만 붙는다.

2. 유닛의 값을 본다.

   ```
   ctatoybox trace entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --filter box/item
   ```

   ```
   /game_entity/extender[inventory]/box/item [0]
     0="can"  ← properties/armoredcar.ext /extender[inventory]/box/item [0] 값 0  공유 127  이 유닛만: 가능
     1=1  ← properties/armoredcar.ext /extender[inventory]/box/item [0] 값 1  공유 127  이 유닛만: 가능
   /game_entity/extender[inventory]/box/item [3]
     0="mine m1a1"  ← entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc /extender[inventory]/box/item [1] 값 0  공유 4  이 유닛만: 가능
     1=24  ← entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc /extender[inventory]/box/item [1] 값 1  공유 4  이 유닛만: 가능
   ```

   (2026-10-13의 실제 출력에서 네 줄을 뺀 것이다.)

   - 윗줄은 펼친 뒤의 키 경로와 순번이다.
   - `←` 뒤는 그 값이 실제로 있는 파일, 그 파일 안의 키 경로, `[몇 번째]`, `값 N`이다. 이 넷을 `add`의 `--file`, `--path`, `--occ`, `--arg`에 넣는다. 펼친 뒤의 순번(`[3]`)과 파일 안의 순번(`[1]`)은 다르다.
   - `공유 N`은 그 파일을 읽는 유닛의 수다. 유닛 파일, include한 파일, 폴더의 `.presets`와 그 `.presets`가 include한 파일(예: `rifle/.presets`의 `\set\small_firearms_accuracy.inc`, 바닐라와 켜 둔 모드에서 618개), `{from}`으로 물려받는 원형과 그 원형이 읽는 파일을 센다. `shared`로 고치면 N개 유닛이 모두 바뀐다.
   - `이 유닛만: 가능`이면 `--scope unit`을 쓸 수 있다. `불가`면 괄호 안에 이유가 있다.
   - `←` 없이 괄호만 붙은 줄은 고칠 수 없는 값이다(치환이나 계산으로 만든 값, include 인자).

3. "이 유닛만" 고친다.

   ```
   ctatoybox add --project editor/projects/my-tweaks.json --scope unit --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --file entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc --path "/extender[inventory]/box/item" --occ 1 --arg 1 --new 5
   ctatoybox build --project editor/projects/my-tweaks.json
   ```

### 월드워 시야 프로젝트 (`vision-preset`)

`vision-preset`은 지금 이긴 판의 `set/vision/vision_fields.inc`에서 지상 시야 필드 34개의 수정 목록을 만든다(설계: [../superpowers/specs/2026-10-16-worldwar-vision-design.md](../superpowers/specs/2026-10-16-worldwar-vision-design.md)).

- `--variant wot`(월드워): 수평 시야각을 360으로, 숨은 병사를 찾는 거리를 50m로 바꾼다.
- `--variant wt`(월드워-워썬더): 수평 시야각을 360으로 바꾸고, 거리 배수 r을 `--scale`배(기본 4)로 바꾼다.
- `vision.set`의 전체 배수는 바꾸지 않는다. 항공·해상·청각·쌍안경까지 바뀌기 때문이다.
- 이미 있는 프로젝트 파일은 `--force`를 줄 때만 덮어쓴다.

```
ctatoybox vision-preset --variant wot --project editor/projects/월드워.json
ctatoybox build --project editor/projects/월드워.json
```

### 일괄 규칙 (`rule`, F1)

"이 종류의 유닛 모두에서 이 키의 값을 이렇게"를 규칙 하나로 프로젝트에 넣는다(설계: [../superpowers/specs/2026-10-17-f1-bulk-rules-design.md](../superpowers/specs/2026-10-17-f1-bulk-rules-design.md)).

- 칸: 대상 종류(`--kind tank` 등), 유닛 경로에 모두 들어야 하는 글자(`--path /tank_light/`. 여럿이면 쉼표로 나눈다), 원본 계층(`--layer`), 펼친 키(정확한 경로 또는 `**/꼬리`), 값 자리(`--arg 0`, `all`이면 수인 값 모두), 바꾸는 방법(`--mul`·`--add`·`--set` 가운데 하나), 범위(`--scope auto|shared|unit`).
- 정수였던 값은 반올림해 정수로 남고, 소수는 소수 셋째 자리까지 쓴다. 수가 아닌 값은 곱하거나 더하지 않는다.
- 범위 `auto`: 그 파일을 쓰는 유닛이 모두 대상이면 '전부' 수정 하나, 아니면 대상 유닛마다 '이 유닛만'. '이 유닛만'이 안 되는 값(매크로 본문 등)은 건너뛰고 이유를 보인다.
- 사람이 고친 수정과 목록 앞 규칙의 수정이 있는 자리는 건너뛴다. 규칙이 만든 수정을 사람이 고치거나 끄면 그 수정은 규칙에서 떨어지고, 다시 적용해도 남는다.
- 규칙을 고치거나 끄거나 지우면 그 규칙과 목록 뒤 규칙들의 수정을 다시 계산한다. 원본(모드)이 바뀌면 "다시 적용"한다.
- 화면: 요약·전체 탭의 줄을 오른쪽 단추로 눌러 "이 키로 일괄 규칙...". 지금 유닛의 종류, 그 줄의 키, 처음 고칠 수 있는 값의 자리로 창이 채워진다. "꼬리만"은 키를 마지막 두 이름(`**/locomotion/maxspeed`)으로 줄인다. "미리 보기"로 바뀌는 값(앞 500건)과 건너뛰는 유닛, 합계를 본 뒤 "수정 목록에 넣기". 미리 본 뒤 칸을 바꾸면 다시 미리 봐야 넣을 수 있다. 뜻이 미확인인 키는 창이 경고한다(막지는 않는다).
- 수정 목록에서 규칙이 만든 수정은 `규칙 · <이름>` 묶음에 모인다. 아래 "일괄 규칙" 칸에서 규칙을 고르고(꺼진 규칙도 여기 있다) 고치기·다시 적용·끄기/켜기·지우기를 누른다.

```
ctatoybox rule preview --kind tank --path /tank_light/ --key "**/locomotion/maxspeed" --mul 1.1
ctatoybox rule add --project editor/projects/my-tweaks.json --name "경전차 속도 10%" --kind tank --path /tank_light/ --key "**/locomotion/maxspeed" --mul 1.1
ctatoybox rule list --project editor/projects/my-tweaks.json
ctatoybox rule edit --project editor/projects/my-tweaks.json --id <id> --mul 1.2
ctatoybox rule reapply|enable|disable|remove --project editor/projects/my-tweaks.json --id <id>
```

### 팩션별 강화 프로젝트 (`faction-preset`)

군대(ger·rus·usa·eng·fin·jap) 하나의 유닛만 강화하는 프로젝트를 만든다(설계: [../superpowers/specs/2026-10-17-faction-boost-design.md](../superpowers/specs/2026-10-17-faction-boost-design.md), 파일 정리: [../12-faction-files.md](../12-faction-files.md)). 다른 군대 유닛과 상대 AI는 지금 판 그대로다.

- 베이스(`--base`, 보통 `editor/projects/월드워-차종.json`)의 수정을 그대로 가져온다. 시야는 모든 군대에 같다.
- 유닛 강화(그 군대 유닛만): 인벤토리 개수 ×2(탄약 파일 `.ammo`로 풀리는 아이템은 ×5, 개수와 ± 편차를 같이), 재장전 시간 ×0.5, 보병 체력 ×2, 장갑 차량 ×2·경전차 ×2·중형 ×2.5·중전차 ×3·초중전차 ×5, 속도 ×1.5, 연료와 항속 ×10. 배수는 인자로 바꾼다.
- 인벤토리·장갑·속도·연료는 일괄 규칙(군대 거르기)으로 들어가 앱의 "일괄 규칙"에서 보고 다시 적용할 수 있다. 항속(range)은 연료 규칙이 고친 mobility 호출마다 직접 고친다.
- 재장전: 무기는 여러 군대가 이름으로 같이 부르므로, 그 군대 유닛이 부르는 무기마다 파일 사본(`<이름>_ctb_<군대>`)을 만들어 장전 시간을 고치고 그 군대 유닛의 이름 값만 사본 이름으로 바꾼다. 원형·매크로에서 오는 장전 시간은 사본에 `{rechargeTime N}` 줄을 더한다.
- 보병 체력: perks의 `"health"` 값 2배를 그 보병 파일의 perks 블록 끝에 `{"health" "N%"}`로 더한다. 더한 줄과 사본 쪽 줄이 매크로·원형 값을 이기는지는 게임 안 확인을 하지 않아 미확인이다.
- 초중전차는 `editor/presets/super-heavy.txt`(손으로 고친다)의 유닛 폴더다. 파생형(대공·자주포·구축전차)도 넣었다.
- 컨퀘스트(플레이어 몫만, 모든 군대 프로젝트에 같이): 시작 자원 MP·AP 99999, SP·RP 999, 단계 I~V 한도와 예비 병력 한도 9999, 승패 보상 MP·AP `"1:99999"`, SP·RP 999. AI 몫(`Bots`)은 그대로다.
- 스커미시: 배틀존·탄약전 프리셋의 MP 시작·끝과 CP ×50(`--skirmish`), 봇 배율(`bots_generic.inc`의 mpStart·mpFinish·cp) ÷50이라 AI의 결과 값은 그대로다. 피난은 팀 a(플레이어 수비)만 ×50. DP와 프런트라인은 그대로다.
- 두 군대 로스터에 같이 등록된 유닛(바닐라 5대)은 건너뛰고 보고한다. `--report <파일>`은 건너뛴 것을 모두 적는다.

```
ctatoybox faction-preset --army ger --base editor/projects/월드워-차종.json --project editor/projects/강화-독일.json
ctatoybox build --project editor/projects/강화-독일.json
```

### 스폰 모드 (`spawn-menu`)

Spawn Menu of MACE를 대체하는 모드 `mod/CTAToyboxSpawn`(이름 `CTAToybox Spawn`)을 만든다(설계: [../superpowers/specs/2026-10-17-spawn-menu-replacement-design.md](../superpowers/specs/2026-10-17-spawn-menu-replacement-design.md), 조사: [../13-spawn-menu-spike.md](../13-spawn-menu-spike.md)). 스탯 출력 모드와 다른 폴더이고 같은 경로를 쓰지 않는다(겹치면 쓰지 않고 오류).

- 입력: 지금 계층에서 Spawn Menu 두 모드(3617250510, 3295913567)와 스폰 모드 자신을 뺀 것, 그리고 스탯 출력 모드. 그래서 맵은 Auto Medic·MACE가 고친 판이 이긴다.
- 스커미시 구매 창: 기본으로 바꾸지 않는다(2026-10-18 사용자 결정: 스폰 메뉴만 둔다). `--roster`를 주면 시기 로스터(`set/multiplayer/units/<시기>/{units,inf}_<국가>*.set`)의 cd·ci·cp·cost·cw·cool을 0으로, 병종 틀(`settings_inf*.set`)의 CP를 0으로, `unitlimits*.inc`의 한도를 999로 하고, 단추는 `budget*.inc`의 `{buttons}`를 `inf1 inf2 empl vehicles light medium heavy td air`로 바꾸고 tanks 그림(`button_5_<국가>`)을 6~9번으로 복사한다(`--basic-buttons`면 단추 그대로, `--keep-cost`면 비용 그대로). `--roster-add`는 여기에 로스터에 없던 전차·포·차량을 국가·시기마다 새 항목으로 더한다(승무원은 국가 기본 → 같은 파일에서 가장 많이 쓰는 구성). 게임은 판을 열 때 그 판 국가·시기의 로스터 차량을 모두 미리 읽어서(`eScenePreload`), 더한 차량만큼 로딩이 길어진다(2026-10-17 21:59 game.log: 210초 넘게, 미리 읽은 차량 43종 중 38종이 더한 차량). 컨퀘스트 로스터는 그대로다.
- 맵: 캠페인 `map/single/**/0.mi`와 멀티 `map/multi/**/*.mi`마다 이긴 판 + `{mission}` 닫는 괄호 앞 include 줄(캠페인은 `nation_<국가>.mi`, 견본 `templates_<국가>.mi`, `menu.mi` 순서, 멀티는 견본과 `menu.mi`).
- 견본(`templates_<국가>.mi`): 메뉴가 복제하는 숨긴 사람·비행기를 국가마다 따로 둔다. 멀티 맵은 판의 국가를 맵을 만들 때 알 수 없어서 모두 넣고, 캠페인 맵은 그 맵 글에 나오는 병종 국가(`"mp/<국가>/…"`)와 캠페인 국가만 넣는다(2026-10-18 실측 평균 1.9개 국가, 못 찾으면 모두). 견본 파일은 `ctb_has_<국가>`를 1로 켜고, 국가 화면은 견본이 없는 국가 단추를 숨긴다. `--all-nations`면 캠페인 맵에도 모두 넣는다. 판을 열 때 견본 2,965명을 만드는 데 약 100초 걸렸다(같은 로그, 판에 없는 일본 장비까지 읽음).
- 메뉴(`resource/map/ctatoybox_spawn/menu.mi`): 증원 창 단추로 만든 단계 메뉴. 맨 위 `Spawn` → 국가(바로 고름)·소속(Auto, Player 0~9 바로 고름)·보병(분대·낱개, 시기별)·포·차량·전차(경·중형·중·구축)·항공기·지원(포격 소·중·대, 증원 점수)·도구(무적, 등급 최대, 생성 시 최고 등급, 찍은 유닛 삭제, 비행기 삭제, 잔해 정리). 한 화면 21개를 넘으면 계열 묶음 화면과 Next/Prev. 분대는 로스터 구성 그대로 숨긴 견본을 복제한다. 캠페인은 국가·소속(Player 0)이 미리 정해지고, 멀티는 시작할 때 소속 화면을 연다. 글자는 `localizations/default/interface/text/mission/ctatoybox_spawn.pot`(영어).
- 화면: `SceneGameView.xml`의 세 패널(멀티·싱글·컨퀘스트) 모두에 증원 창을 조건 없이 둔다.
- 다시 만들 때는 지난 생성물(`.ctatoybox-spawn.json`)만 지우고 바꾼다. 게임 패치·모드 갱신·스탯 출력 모드 재빌드 뒤에는 다시 만든다.
- git: 맵 사본(약 210MB)은 넣지 않는다(`.gitignore`). 메뉴·견본·글자·화면(그리고 `--roster`로 만든 로스터·그림)은 넣는다. 지난 생성물을 지우면 빈 폴더도 지운다.
- 게임에 연결: 게임 `mods` 폴더에 정션을 만들고(`New-Item -ItemType Junction -Path "<게임>\mods\CTAToyboxSpawn" -Target "E:\CTAToybox\mod\CTAToyboxSpawn"`), 모드 목록에서 모든 모드 뒤에 켜고 Spawn Menu of MACE는 끈다.
- 미확인(게임 안에서만 알 수 있다): 견본 파일의 트리거가 `ctb_has_<국가>`를 켜는지(따로 include한 `{Helpers}`가 합쳐지는지. 안 되면 `--all-nations`), 새 종류 단추를 게임이 받는지(`--roster`일 때. 안 되면 `--basic-buttons`), 증원 창이 멀티·컨퀘스트에서 보이는지와 사라지는 문제가 고쳐지는지, `{type side}`가 멀티에서 국가를 주는지, 봇이 비용 0 로스터를 쓰는지, 견본 id(0xC100~)가 맵의 id와 부딪히는지(Spawn Menu도 같은 범위를 쓴다).

```
ctatoybox spawn-menu preview
ctatoybox spawn-menu build [--all-nations] [--grid <열> | --list] [--roster [--roster-add] [--keep-cost] [--basic-buttons]]
```

### 한글화 (`l10n`)

게임과 켜 둔 모드의 현지화 `.pot`을 한국어로 바꿔 출력 모드 `CTAToybox`에 넣는다(설계: [../superpowers/specs/2026-10-17-korean-localization-design.md](../superpowers/specs/2026-10-17-korean-localization-design.md), 계획: [../superpowers/plans/2026-10-17-korean-localization.md](../superpowers/plans/2026-10-17-korean-localization.md)).

1. 갱신 `l10n extract`: 바닐라와 켜 둔 모드(로드 순서상 가장 뒤의 파일이 원문)에서 `l10n/ko/<원래 경로>.po`를 만들거나 고친다. 원문이 바뀐 항목은 `#, fuzzy`와 `#| msgid`(예전 원문)를 남기고, 사라진 키는 지우고, 새 키는 비워 둔다. 머리에 원문 층과 게임 버전(`call_to_arms.exe` 제품 버전)을 적는다.
2. 번역: `.po`의 `msgstr`를 채운다. 용어집은 `l10n/ko/glossary.psv`(`원문|번역|메모`, 원문과 번역이 같으면 "그대로 둘 말").
3. 검사 `l10n check [--file <글자>]`: 태그 `<…>`, `%` 자리표시(순서까지), `\n` 수, `{…}`, 그대로 둘 말, 글꼴에 없는 글자. 오류가 있으면 종료 1. 미번역·fuzzy는 수만 센다.
4. 빌드 `l10n build`: 검사 오류가 있거나 게임이 실행 중이면(`--force` 없이) 멈춘다. `mod/CTAToybox/localizations/default/<경로>.pot`(msgid가 한국어, 비었거나 fuzzy면 영어)과 한글 없는 글꼴 81개의 `.font`(`resource/interface/font/`, `binary_data`만 가장 가까운 `main_<크기>.fnt`)를 쓴다. 자기 목록 `.ctatoybox-l10n.json`의 지난 생성물만 지우고, 편집기 빌드 목록에 있는 경로와 목록 밖의 있는 파일은 덮지 않는다.

편집기의 유닛 이름은 `원문 (한글)`으로 보인다(번역이 있고 fuzzy가 아닐 때). 검색은 둘 다에 걸린다. 미확인 다섯 가지는 설계 문서 "미확인"에 있다.

```
ctatoybox l10n extract
ctatoybox l10n check
ctatoybox l10n build
```

### "이 유닛만"이 하는 일

공유 파일은 건드리지 않는다. 유닛에서 그 파일까지의 include 사슬을 유닛 전용 사본으로 만들고, 유닛 파일과 사본 안의 include 경로를 사본으로 고쳐 쓴다.

- 사본은 원본과 같은 폴더에 `<원본 이름>__ctb_<유닛 파일 이름>_<8자 해시><확장자>`로 생긴다. 위 예에서는 `m3a1_halftrack__ctb_m3a1_halftrack_8da609d3.inc`다.
- 유닛 파일도 출력 폴더에 생긴다. 바뀌는 것은 include 경로뿐이다.
- 같은 파일에 `shared` 수정도 걸려 있으면 사본에도 들어간다. 같은 자리를 둘 다 고치면 그 유닛에는 "이 유닛만"의 값이 쓰인다.
- 값이 유닛 자신의 파일에 있으면 사본 없이 그 파일을 고친다.

### "이 유닛만"이 안 되는 경우

| 경우 | 이유 |
|---|---|
| 매크로 본문(`(define ...)` 안)의 값 | 그 매크로를 부르는 모든 곳이 바뀐다. `shared`만 된다 |
| `.presets`에 있는 값 | include로 읽는 파일이 아니라서 사본으로 바꿔 끼울 수 없다 |
| 치환이나 계산으로 만든 값 | 원본의 인자나 피연산자를 고친다 |

## 빌드 결과의 뜻

| 표시 | 뜻 | 할 일 |
|---|---|---|
| 적용 | 출력 폴더에 반영했다 | |
| 원본이 바뀜 | 수정을 만들 때의 원래 값과 지금 원본의 값이 다르다. 적용하지 않았다 | 지금 값을 `show`로 보고 수정을 다시 만든다 |
| 대상 없음 | 그 파일이나 그 자리가 원본에 없다 | 모드를 껐거나 갱신으로 구조가 바뀐 것이다 |
| 적용 불가 | 새 값이 낱말 하나나 닫힌 문자열 하나가 아니거나, 원본 파일이 파싱되지 않는다 | |

## 빌드가 건드리는 파일

- 쓰는 곳은 `mod/CTAToybox/resource` 아래와 `mod/CTAToybox/.ctatoybox-build.json`뿐이다.
- "이 유닛만" 수정은 원본에 없는 이름의 사본 파일도 만든다. 사본도 `.ctatoybox-build.json`에 적히고 다음 빌드에서 지워진다.
- 쓴 파일과 같은 폴더의 `.presets`(이긴 판)와, 쓴 아이템 파일이 `{from}`으로 물려받는 원형 사슬(이긴 판)을 바이트 그대로 같이 쓴다. 출력 모드에 있는 파일이 다른 모드 폴더에만 있는 정의·원형을 찾지 못해 게임이 꺼졌기 때문이다(2026-10-17 `define not found`, `pattern not found`). 내용이 같아 다른 파일이 읽는 값은 바뀌지 않는다.
- 지우는 것은 이전 빌드가 `.ctatoybox-build.json`에 적어 둔 파일뿐이다. 손으로 넣은 파일은 그대로 둔다.
- `.ctatoybox-build.json`이 깨져 읽히지 않으면 빌드는 아무것도 쓰거나 지우지 않고 멈춘다(그대로 빌드하면 이전 생성물의 기록을 잃는다). 출력 폴더의 `resource` 아래에서 이전 생성물을 확인해 지우고 목록 파일도 지운 뒤 다시 빌드한다.
- 수정을 모두 끄고 빌드하면 생성물이 모두 사라진다.
- 생성물과 `.ctatoybox-build.json`은 git에 넣는다(2026-10-17). 다시 빌드하면 함께 커밋한다. `builtAt` 때문에 목록 파일은 빌드마다 바뀐다. `.gitattributes`가 생성물의 줄바꿈을 바이트 그대로 둔다.

## 알려진 한계

- **같은 경로의 파일은 `{mods}` 목록에서 뒤에 있는 모드의 것이 쓰인다.** 게임에서 확인했다(2026-10-13, [ingame-check.md](ingame-check.md)): 같은 빌드로 `CTAToybox`가 맨 앞일 때는 `mod_2905667604`의 값이, 맨 뒤일 때는 `CTAToybox`의 값이 나왔다. 그래서 `CTAToybox`가 `options.set`의 `{mods}` 목록에서 맨 뒤에 있어야 한다. `sources`와 `build`가 순서를 확인해 경고한다. 순서는 직접 고친다.
- **한 모드 안에서 pak과 낱개 파일이 겹치면 낱개 파일이 쓰인다.** 게임에서 확인했다(2026-10-13, [ingame-check.md](ingame-check.md)의 "일괄 확인").
- **`{from "..."}` 원형 상속은 펼쳐 보이지만 셋은 미확인이다(E11, 2026-10-16).** 전체 탭의 `원형 · <파일>` 묶음에 원형 파일의 줄이 보이고, 줄 아래에 상태가 적힌다. "물려받는다", "가려짐"(자식 쪽 파일에 같은 키가 있어 게임은 그 값을 쓴다, 요약 탭에서 뺀다), "미확인"(감싼 블록만 자식에도 있거나 여러 번 적는 키), "조건부"(자식 쪽 파일이 같은 키를 `(mod …)` 조건 안에서만 적는다. 그 조건에서는 자식 값, 아니면 원형 값을 쓴다). 따옴표 이름 블록(`{"head" 1.3}`)은 이름마다 따로 견준다. 원형의 매크로 호출 줄은 호출이 펼치는 키를 자식이 모두 적으면 "가려짐"이다. 원형이 include한 파일의 호출 줄은 그 내용이 펼쳐진 자리의 경로로 보인다. `set/stuff` 밖 파일의 `{from}`은 원형 상속으로 펼치지 않는다. 유닛 머리에 원형 사슬이 보이고, 이름에 맞는 파일이 둘이면 고른 파일과 후보를 적는다. 물려받은 값을 '전부'로 고치면 원형 파일이 바뀌어 그 원형을 물려받는 유닛이 모두 바뀐다(2026-10-14, [ingame-check.md](ingame-check.md): `thompson_m1928a1_50`의 연사를 고치자 `thompson_m1928a1_30`도 느려졌다). '이 유닛만'은 원형의 맨 위 블록 바로 아래 키이고, 상태가 '물려받는다'이며, 원형에 한 번만 적혀 있고, 조건 밖이며, 원형 파일의 그 줄에 직접 적힌 값일 때만 된다(E14, 2026-10-16). 빌드가 유닛 파일의 맨 위 블록 끝에 원형의 키 표기 그대로 그 줄 전체를 덧붙인다(예: `{Speed 800 7}`). 게임은 자식 파일의 같은 키를 쓴다(G16). 감싼 블록 안의 키, 여러 번 적는 키, 조건 안의 줄, 매크로 인자로 만든 값은 '전부'만 된다. '같이 씀'에 물려받는 유닛이 든다. 미확인 셋(감싼 블록, 여러 번 적는 키, 맞는 파일이 둘인 이름)은 게임에서 확인하지 않았고, 이름 규칙은 추정이다([docs/04-units/README.md](../04-units/README.md) "한계"). 왼쪽 목록의 "값에 수가 있는 유닛"은 유닛 파일 자신의 값으로 센다(헬멧 같은 한 줄 별칭 파일은 목록에 없다). 다른 유닛이 원형으로 물려받는 유닛(바닐라 `bazooka_m1.weapon`, `air_bomb_100kg.ammo` 등)은 '이 유닛만'이 안 된다: 그 유닛의 파일을 고치면 물려받는 유닛도 바뀐다.
- **도구는 문법이 깨지지 않는 것까지만 보장한다.** 값이 게임에서 뜻한 대로 동작하는지는 게임에서 본다.
- **게임은 사본 파일과 고쳐 쓴 include를 받아들인다.** 한 단계와 두 단계 사슬, 같은 폴더와 `/properties/...` 형태의 경로를 게임에서 확인했다(2026-10-13, [ingame-check.md](ingame-check.md)).
- 게임 안 확인 결과는 [ingame-check.md](ingame-check.md)에 있다. 시야·사거리를 잴 때 화면의 표시(안개, 미니맵, 직접 조종의 거리 숫자 등)가 무엇인지는 [ingame-displays.md](ingame-displays.md)에 있다.
