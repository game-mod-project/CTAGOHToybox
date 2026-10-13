# 스탯 편집기

기준: 2026-10-12, 게임 1.0.6.7. 설계는 [../superpowers/specs/2026-10-12-stat-editor-design.md](../superpowers/specs/2026-10-12-stat-editor-design.md)에 있다.

지금 있는 것은 명령줄 도구다. 화면은 아직 없다.

## 할 수 있는 것과 없는 것

| 할 수 있다 | 아직 없다 |
|---|---|
| 유닛을 경로나 표시 이름으로 찾기 (`units`) | 화면 |
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

4. 빌드한다.

   ```
   ctatoybox build --project editor/projects/my-tweaks.json
   ```

5. 게임을 다시 시작한다. 모드 목록에서 `CTAToybox`가 켜져 있어야 한다.

## 유닛에서 시작해 고치기

1. 유닛을 찾는다. 처음 한 번은 유닛 색인을 만든다(2026-10-13 이 PC에서 유닛 21,612개, 8.2초). 결과는 `editor/cache/`에 저장되고, 모드를 켜고 끄거나 모드가 갱신되면 다시 만든다.

   ```
   ctatoybox units --find m3a1_halftrack --kind vehicle
   ```

   한 줄이 유닛 하나다: `종류  계층  경로  표시 이름`. 표시 이름은 `data/display-names.csv`에 `desc/entity/<파일 이름>` 키가 있는 엔티티에만 붙는다.

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
- **한 모드 안에서 pak과 낱개 파일이 겹치면 낱개 파일을 쓴다고 가정했다.** 게임의 실제 동작은 확인하지 않았다.
- **도구는 문법이 깨지지 않는 것까지만 보장한다.** 값이 게임에서 뜻한 대로 동작하는지는 게임에서 본다.
- **게임이 사본 파일과 고쳐 쓴 include를 받아들이는지는 [ingame-check.md](ingame-check.md)의 3단계 확인으로 본다.** 확인 전에는 "이 유닛만"의 결과를 믿지 않는다.
- 게임 안 확인 결과는 [ingame-check.md](ingame-check.md)에 있다.
