# 스탯 편집기

기준: 2026-10-12, 게임 1.0.6.7. 설계는 [../superpowers/specs/2026-10-12-stat-editor-design.md](../superpowers/specs/2026-10-12-stat-editor-design.md)에 있다.

지금 있는 것은 명령줄 도구다. 화면은 아직 없다.

## 할 수 있는 것과 없는 것

| 할 수 있다 | 아직 없다 |
|---|---|
| 원본 파일(바닐라와 켜 둔 모드)의 값 하나를 바꾸는 수정을 프로젝트 파일에 담기 | 유닛 검색, 화면 |
| 프로젝트를 빌드해 `mod/CTAToybox/resource`에 써넣기 | "이 유닛만" 고치기. 지금은 그 파일을 쓰는 모든 유닛이 바뀐다 |
| 게임이나 모드가 갱신돼 원래 값이 달라진 수정을 알아채기 | 키나 블록을 더하거나 빼기 |

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

## 빌드 결과의 뜻

| 표시 | 뜻 | 할 일 |
|---|---|---|
| 적용 | 출력 폴더에 반영했다 | |
| 원본이 바뀜 | 수정을 만들 때의 원래 값과 지금 원본의 값이 다르다. 적용하지 않았다 | 지금 값을 `show`로 보고 수정을 다시 만든다 |
| 대상 없음 | 그 파일이나 그 자리가 원본에 없다 | 모드를 껐거나 갱신으로 구조가 바뀐 것이다 |
| 적용 불가 | 새 값이 낱말 하나나 닫힌 문자열 하나가 아니거나, 원본 파일이 파싱되지 않는다 | |

## 빌드가 건드리는 파일

- 쓰는 곳은 `mod/CTAToybox/resource` 아래와 `mod/CTAToybox/.ctatoybox-build.json`뿐이다.
- 지우는 것은 이전 빌드가 `.ctatoybox-build.json`에 적어 둔 파일뿐이다. 손으로 넣은 파일은 그대로 둔다.
- 수정을 모두 끄고 빌드하면 생성물이 모두 사라진다.

## 알려진 한계

- **같은 경로의 파일은 나중에 로드된 모드가 이긴다는 것은 추정이다.** 그래서 `CTAToybox`가 `options.set`의 `{mods}` 목록에서 맨 뒤에 있어야 한다. `sources`와 `build`가 순서를 확인해 경고한다. 순서는 직접 고친다.
- **한 모드 안에서 pak과 낱개 파일이 겹치면 낱개 파일을 쓴다고 가정했다.** 게임의 실제 동작은 확인하지 않았다.
- **도구는 문법이 깨지지 않는 것까지만 보장한다.** 값이 게임에서 뜻한 대로 동작하는지는 게임에서 본다.
- 게임 안 확인 결과는 [ingame-check.md](ingame-check.md)에 있다.
