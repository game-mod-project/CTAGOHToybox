# 스탯 편집기

기준: 2026-10-12, 게임 1.0.6.7. 설계는 [../superpowers/specs/2026-10-12-stat-editor-design.md](../superpowers/specs/2026-10-12-stat-editor-design.md)에 있다.

지금 있는 것은 명령줄 도구다. 화면은 아직 없다.

## 할 수 있는 것과 없는 것

| 할 수 있다 | 아직 없다 |
|---|---|
| 유닛을 경로나 게임 속 이름으로 찾기 (`units`, 화면) | |
| 유닛의 값이 어느 파일의 어느 자리에서 왔는지, 몇 개 유닛이 그 파일을 같이 쓰는지 보기 (`trace`) | 유닛이 쓰는 무기·탄약 파일을 유닛과 함께 보기 |
| 원본 파일(바닐라와 켜 둔 모드)의 값 하나를 바꾸는 수정을 프로젝트 파일에 담기 | 매크로 본문 안의 값을 "이 유닛만" 고치기 |
| include로 공유된 값을 "이 유닛만" 고치기 (`add --scope unit`) | 키나 블록을 더하거나 빼기 |
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

- **왼쪽 유닛 찾기**: 게임 속 이름이나 파일 이름으로 찾고, 종류(탱크, 포, 차량, 기타 탈것, 보병, 무기·탄약·장비)와 출처(바닐라, 모드 이름)로 거른다. 차량 `.def`는 include하는 `properties/*.ext`로 나눈다: `cannon*.ext`면 포, `tank.ext`·`tank_assaultgun.ext`·`tank_unarmed.ext`면 탱크(자주포도 여기에 든다), `car.ext`·`armoredcar.ext`·`halftrack.ext`·`moto.ext`·`bicycle.ext`면 차량, 나머지(항공기·배·열차, `_x` 파생 파일)는 기타 탈것이다. 이름순으로 500개까지 보인다. 보병 외형(`entity/humanskin`), 아이템 모델(`entity/inventory`), 아이템 틀(`.pattern`)은 목록에 없다. 펼친 값에 수가 하나도 없는 파일(예: `{from "75mm_pak39"}` 한 줄짜리 별칭, `from`·`tag`·`entity`만 있는 헬멧)도 고칠 스탯이 없어 뺀다. 2026-10-14 데이터로 무기·탄약·장비 4,074개 가운데 1,723개(헬멧 1,662개)가 빠졌다. 같이 쓰는 유닛 수는 빠진 파일까지 센다. x 폴더의 `_x`, `_xx` 파일은 기본으로 숨긴다(무엇인지는 확인하지 않았다).
- **이름**: 게임과 켜 둔 모드의 현지화(`localizations/default`)에서 읽는다. 차량은 `desc/entity/<파일 이름>`, 보병은 `desc/human/<set/breed 아래 경로>`, 무기·탄약은 파일 종류에 따라 `desc/weapon|ammo|stuff/<파일 이름의 낱말>`이다. 2026-10-14 데이터로 확인했고, 이름이 붙는 비율은 차량 98%, 보병 100%, 무기·탄약 95%다. 출처의 모드 이름은 각 모드의 `mod.info`에서 읽는다.
- **요약 탭**: 종류별로 고른 항목을 짧은 이름으로 묶어 보인다. 고른 항목과 이름은 `editor/summary/<종류>.psv`(`경로|이름`)에 있다(차량, 보병, 무기·탄약). 같은 항목이 여러 부위에 있으면 줄마다 부위(예: `volume body1`, `place gun`)를 붙이고, 6줄이 넘는 묶음은 접어 둔다. 키의 설명과 등급(확인·문서·추정)은 줄마다 보인다. 따옴표 이름 줄(`{"health" "200%"}`)은 펼친 경로가 `/breed/perks/"*"` 하나로 묶이므로, 요약 키·키 주석·값 이름에 이름을 넣은 경로(`/breed/perks/"health"`)를 쓰면 이름별로 나뉜다. 이름 경로가 `"*"` 경로보다 먼저다. 보병 요약은 체력, 체력 회복, 스태미나, 스태미나 회복, 이동 속도, 무기 기술, 회피, 소생 설정, 시야 순으로 보인다. 이 값들은 대부분 `set/breed/presets/perks/*.inc`의 tier 매크로 안에 있어 '전부'로만 고쳐지고, 그 매크로를 쓰는 병종이 모두 바뀐다(2026-10-15 바닐라에서 6,111개).
- **전체 탭**: 펼친 값을 원본 파일별로 묶어 보인다. 유닛 파일, 나머지 파일, 매크로 호출, 고칠 수 없는 값 순서다. 키는 읽히는 표기(`inventory › box › item`)와, 그 아래 원본에 쓰인 그대로의 키 경로(`키 /from/parameters/spreading/radiusTable/#row`, 대소문자 포함)로 보인다. 같은 원본 자리를 가리키는 줄이 여러 번 펼쳐지면(예: 매크로 인자 하나가 탄종마다 같은 사거리 표에 들어감) 한 줄로 모으고 `같은 줄 N곳`을 적는다. 무기의 `parameters "ap2"` 같은 탄종 묶음은 부위처럼 줄마다 적는다. "매크로 호출" 묶음에서 계산에 쓰이는 인자(예: 연사 속도 `rpm`)를 고친다. `%이름` 매개변수와 쓰는지 알 수 없는 매크로 정의 안의 호출은 보이지 않거나 고칠 수 없다. 위의 "거르기"로 줄인다.
- **가격·로스터 탭**: 콘퀘스트·배틀존 등록 파일(`set/multiplayer/units/**/*.set`)과 연구 트리(`set/dynamic_campaign/unit_research_*.set`)에서 이 유닛의 항목을 보인다. 항목은 따옴표 이름 블록이고, 이름이 차량 `.def`의 파일 이름(예: `panzer5g`)이나 breed의 `set/breed` 아래 경로(예: `mp/ger/mid/nco`)와 같으면, 또는 항목 안에 `vehicle(이름)`이 있으면 이 유닛의 것이다. 값은 원문처럼 `cp(40)`, `lvl(lvl1 lvl2)`로 보인다. 고치면 그 항목만 바뀌므로 "이 유닛만"은 없다. 항목 이름과 `requires`·`costs` 같은 열쇠말은 고치지 않는다. 난이도별 자원(`resources_<난이도>.set`)처럼 유닛에 딸리지 않은 설정은 아직 편집기에 없다.
- **무기·탄약 탭**: 유닛이 이름으로 가리키는 무기·탄약과 이어진 파일, 이은 규칙이다. 파일 하나로 이어진 것은 두 번 눌러 연다. "후보"와 "파일 없음"은 게임이 어느 파일을 쓰는지 확인하지 않은 이름이다(규칙의 근거는 `docs/02-modding-research.md`의 "아이템 이름과 파일").
- **값 고치기**: 창의 제목은 항목의 한글 이름이고, 값마다 뜻(예: `최고 속도 (km/h)`, `아이템 이름`, `수량`)과 원문 이름·자리·원래 값을 보인다. 값의 뜻은 `data/annotations/values.psv`에서 읽고, 없으면 "뜻 미확인"으로 적는다. 표의 "지금 값" 칸도 뜻을 아는 값은 한 줄에 하나씩 "뜻: 값"으로 보인다. 전체 탭은 줄마다 키 주석에서 뽑은 한글 이름을 굵게, 그 아래 원문 키를 보인다(뜻을 모르면 "이름 미확인"). 값을 두 번 누른다. 공유되는 값이면 영향받는 유닛 목록과 "전부 / 이 유닛만"이 나온다. "이 유닛만"이 안 되는 자리는 선택지가 꺼지고 이유가 보인다. 같은 자리를 다시 고치면 수정이 바뀐다(쌓이지 않는다).
- **오른쪽 수정 목록**: 유닛별로 묶인다. 체크로 끄고 켠다. "삭제"하면 값이 원본으로 돌아간다. "원본이 바뀜"이 뜬 수정은 "지금 값에 맞추기"로 원래 값을 지금 원본 값으로 바꾸거나 삭제한다.
- **빌드**: 게임이나 에디터가 실행 중이면 꺼질 수 있다고 경고하고 묻는다(기본 선택은 "아니요"). 결과를 창으로 보인다.

화면 확인용 옵션: `--unit <가상 경로> --tab summary|full|items --filter <글자> --edit-row <항목 이름> --screenshot <png>`는 그 화면(또는 그 줄의 값 고치기 창)을 PNG로 저장하고 끝낸다. `--shots <목록 파일>`은 한 번 실행으로 여러 화면을 찍는다(한 줄: `파일 이름|유닛|탭|거르기|값 고치기 창 항목`). `--project <파일>`로 프로젝트를 고른다. `--scenario <파일>`은 값 고치기 창에 값을 쓰고, 범위를 고르고, 넣기를 누르고, 수정 목록의 체크를 바꾸거나 지우는 조작을 차례로 하며 화면을 찍는다(명령 목록은 `MainWindow.Scenario.cs` 머리 주석). 반드시 `--project`로 임시 프로젝트를 준다.

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
   - `공유 N`은 그 파일을 읽는 유닛의 수다. `shared`로 고치면 N개 유닛이 모두 바뀐다.
   - `이 유닛만: 가능`이면 `--scope unit`을 쓸 수 있다. `불가`면 괄호 안에 이유가 있다.
   - `←` 없이 괄호만 붙은 줄은 고칠 수 없는 값이다(치환이나 계산으로 만든 값, include 인자).

3. "이 유닛만" 고친다.

   ```
   ctatoybox add --project editor/projects/my-tweaks.json --scope unit --unit entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def --file entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.inc --path "/extender[inventory]/box/item" --occ 1 --arg 1 --new 5
   ctatoybox build --project editor/projects/my-tweaks.json
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
- 지우는 것은 이전 빌드가 `.ctatoybox-build.json`에 적어 둔 파일뿐이다. 손으로 넣은 파일은 그대로 둔다.
- 수정을 모두 끄고 빌드하면 생성물이 모두 사라진다.

## 알려진 한계

- **같은 경로의 파일은 `{mods}` 목록에서 뒤에 있는 모드의 것이 쓰인다.** 게임에서 확인했다(2026-10-13, [ingame-check.md](ingame-check.md)): 같은 빌드로 `CTAToybox`가 맨 앞일 때는 `mod_2905667604`의 값이, 맨 뒤일 때는 `CTAToybox`의 값이 나왔다. 그래서 `CTAToybox`가 `options.set`의 `{mods}` 목록에서 맨 뒤에 있어야 한다. `sources`와 `build`가 순서를 확인해 경고한다. 순서는 직접 고친다.
- **한 모드 안에서 pak과 낱개 파일이 겹치면 낱개 파일이 쓰인다.** 게임에서 확인했다(2026-10-13, [ingame-check.md](ingame-check.md)의 "일괄 확인").
- **`{from "..."}`로 다른 파일의 값을 물려받는 것은 편집기가 펼치지 않고, 게임에서도 확인하지 않았다.** 물려준 쪽 파일을 고쳤을 때 물려받는 쪽이 바뀌는지는 미확인이다.
- **도구는 문법이 깨지지 않는 것까지만 보장한다.** 값이 게임에서 뜻한 대로 동작하는지는 게임에서 본다.
- **게임은 사본 파일과 고쳐 쓴 include를 받아들인다.** 한 단계와 두 단계 사슬, 같은 폴더와 `/properties/...` 형태의 경로를 게임에서 확인했다(2026-10-13, [ingame-check.md](ingame-check.md)).
- 게임 안 확인 결과는 [ingame-check.md](ingame-check.md)에 있다.
