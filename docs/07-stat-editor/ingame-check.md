# 게임 안 확인

스펙의 구현 순서 2단계다. 이 확인이 끝나기 전에는 화면을 만들지 않는다. 결과가 "게임이 수정을 받아들이지 않는다"이면 설계를 고친다.

게임 실행은 사람이 한다. 아래에서 `ctatoybox`는 `dotnet run --project editor/CtaToybox.Cli -c Release --`를 줄여 쓴 것이다.

## 절차

1. 게임을 실행해 모드 목록에서 `CTAToybox`를 켜고 적용한 뒤 게임을 끈다.
2. `ctatoybox sources`를 실행한다. "출력 모드 CTAToybox: 게임에서 켜져 있다"가 나와야 한다.
   - "꺼져 있다"로 나오면 `options.set`의 `{mods}` 블록에서 CTAToybox에 해당하는 줄을 찾아 아래 "기록"에 그대로 옮겨 적는다. 도구가 로컬 모드의 표기를 잘못 가정한 것이므로 `GameProfile.PlanLayers`를 그 표기에 맞게 고친다.
   - "뒤에 로드되는 모드가 있다"는 경고가 나오면 `options.set`에서 CTAToybox 줄을 `{mods}` 목록의 맨 뒤로 옮긴다.
3. 바닐라 파일 하나를 고친다. KV-85의 적재 탄약 수를 쓴다(인벤토리 화면에서 바로 보인다).

   ```
   ctatoybox show entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --filter box/item
   ```

   첫 줄의 마지막 값(수량)을 고른다. 그 줄의 대괄호 안 수를 `--occ`에, 수량 앞의 번호를 `--arg`에 넣고 새 값은 7로 한다.

   ```
   ctatoybox add --project editor/projects/ingame-check.json --file entity/-vehicle/ussr/tank_heavy/kv85/kv85.def --path "/game_entity/extender[inventory]/box/item" --occ <대괄호 안 수> --arg <수량 앞의 번호> --new 7
   ctatoybox build --project editor/projects/ingame-check.json
   ```

   `show`에 `box/item` 줄이 없으면 이 차량은 적재품을 include하는 파일에서 받는 것이다. 그때는 `show`를 `--filter` 없이 실행해 `(include)`가 아닌 숫자 값 하나를 고르고, 아래 "기록"에 무엇을 골랐는지 적는다.
4. 에디터나 스커미시에서 KV-85를 놓고 인벤토리를 열어 그 탄약의 수량이 7인지 본다.
   - 에디터는 게임 안에서 여는 것이 아니라 별도 프로그램이다: `binaries\x64\call_to_arms_ed.exe` (Steam의 실행 옵션에도 있다). 게임 안에서 F3을 눌러도 아무 일도 일어나지 않는다(2026-10-13 확인).
   - 에디터 안의 조작(F2 미션 모드에서 엔티티를 놓고 F3으로 실행)은 확인하지 않았다. 게임 폴더의 `mods\tools\mission_editor(eng).pdf`에 설명서가 있다.
   - 에디터가 게임에서 켜 둔 모드를 그대로 읽는지도 확인하지 않았다.
5. 켜 둔 모드가 있으면 그 모드의 파일 하나에도 같은 확인을 한다. `sources`에 나온 모드 가운데 하나를 골라 그 모드가 덮어쓴 파일을 `show`로 열면 첫 줄에 "계층 mod_…"가 나온다. 3~4와 같은 방법으로 값 하나를 고쳐 본다.
6. 확인이 끝나면 수정을 끄고 다시 빌드해 생성물을 지운다.

   ```
   ctatoybox build --project editor/projects/ingame-check.json
   ```

   (`ingame-check.json`의 각 수정에서 `"enabled": true`를 `false`로 바꾼 뒤 실행한다.)

## 기록

| 항목 | 결과 |
|---|---|
| 확인한 날짜와 게임 버전 | 2026-10-13, v1.067.0 (빌드 2026.09.23 14:05) |
| `options.set`에서 CTAToybox의 표기 | `"ctatoybox:0"` (2026-10-13). 폴더 이름을 소문자로 쓰고 `:0`이 붙는다. 도구의 가정과 같다. 게임 로그에 `Mod: activate mod ctatoybox`가 찍힌다 |
| 바닐라 파일의 수정이 게임에 반영됨 | 확인함 (2026-10-13). KV-85의 `bulletrus_85 aphebc`가 7발로 나온다. `ctatoybox`가 `{mods}` 맨 앞이고 다른 모드 4개가 켜진 상태였다. 이 파일을 덮어쓰는 다른 모드는 없다 |
| 모드 파일의 수정이 게임에 반영됨 | 확인함 (2026-10-13). `ctatoybox`가 `{mods}` 맨 뒤일 때 채피가 5발로 나온다. 맨 앞일 때는 18발 그대로였다. 두 번 모두 같은 빌드, 같은 모드 구성이고 목록 순서만 달랐다. 로그의 `Mod: activate mod` 순서가 `options.set`의 순서와 같다. 확인용 수정을 준비했다: M24 채피(`entity/-vehicle/usa/tank_light/m24_chaffee/m24_chaffee.def`, 계층 `mod_2905667604`)의 `bulletusa_75x350 apcbche` 18 → 5. KV-85는 MACE에서 `doctrineonly`라 일반 구매 목록에 없어서 스커미시 후기 미국 진영에서 바로 살 수 있는 차량을 골랐다 |
| 수정을 끄고 다시 빌드한 뒤 원래 값으로 돌아옴 | 확인함 (2026-10-13). KV-85 수정을 끄고 빌드하자 `kv85.def`가 지워졌고 게임에서 19발로 돌아왔다 |
| 게임 로그(`game.log`)에 이 모드와 관련된 오류 | 없음 (2026-10-13). 스커미시 `multi/1v1_balzerweg:battle_zones` 진입까지의 로그에 `ctatoybox`나 `kv85.def`를 가리키는 오류가 없다. `discord.xml`의 XML 오류 6줄은 다른 모드의 것이다 |

## 3단계: "이 유닛만" 확인

스펙의 구현 순서 3단계다. 게임이 사본 파일 이름과 고쳐 쓴 include를 받아들이는지 본다. 이 확인이 끝나기 전에는 화면(4단계)을 만들지 않는다.

**준비한 것** (2026-10-13)

- 프로젝트: `editor/projects/ingame-check-unit.json`
- 수정: M3A1 하프트랙(`entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def`)만, `m3a1_halftrack.inc`의 `{item "mine m1a1" 24}`를 5로.
- 생성물(빌드해 뒀다. 원본과 바이트 단위로 비교해 아래 차이만 있는 것을 확인했다):
  - `resource/entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack.def`: include 경로만 `"m3a1_halftrack__ctb_m3a1_halftrack_8da609d3.inc"`로 바뀜
  - `resource/entity/-vehicle/usa/btr/m3a1_halftrack/m3a1_halftrack__ctb_m3a1_halftrack_8da609d3.inc`: `mine m1a1`의 수량만 5로 바뀜
- 이 차량을 고른 근거:
  - `m3a1_halftrack.inc`는 지금 구성에서 유닛 4개가 읽는다(`trace`의 `공유 4`): 바닐라의 `m3a1_halftrack`, `m3a1_halftrack_inf`, `m3a1_halftrack_inf_late`와 MACE의 `m3a1_halftrack_commander`. 파일의 계층은 `vanilla`다.
  - `m3a1_halftrack`은 MACE 스커미시 후기 미국 진영에서 일반 구매된다(`set/multiplayer/units/late/units_usa.set`의 `cd(150)`).

**절차**

1. Steam 라이브러리에서 게임을 켠다(Steam 없이 켜면 창작마당 모드가 로드되지 않는다. 2026-10-13에 겪음).
2. 스커미시 후기, 미국 진영에서 M3A1 Halftrack을 사서 인벤토리를 연다. `mine m1a1`(M1A1 대전차 지뢰)의 수량을 본다.
3. 같은 `.inc`를 쓰는 다른 차량을 꺼낼 수 있으면 그 차량의 지뢰 수량도 본다. 스커미시에서 살 수 없으면 "확인하지 못함"으로 적는다.
4. 게임을 끄고 `game.log`에서 `m3a1_halftrack`이나 `__ctb_`가 든 오류 줄을 찾는다.
5. 확인이 끝나면 수정을 끄고 다시 빌드해 생성물을 지운다.

**기록**

| 항목 | 결과 |
|---|---|
| 확인한 날짜와 게임 버전 | 확인하지 않음 |
| M3A1 하프트랙의 `mine m1a1` 수량 (기대 5) | 확인하지 않음 |
| 같은 `.inc`를 쓰는 다른 차량의 수량 (기대 24) | 확인하지 않음 |
| `game.log`에 사본 파일이나 include와 관련된 오류 | 확인하지 않음 |
| 수정을 끄고 다시 빌드한 뒤 24로 돌아옴 | 확인하지 않음 |

M3A1이 24 그대로면 게임이 고쳐 쓴 유닛 파일이나 사본을 쓰지 않은 것이다. 그때는 추측으로 원인을 정하지 않는다. `game.log`와 생성물을 먼저 보고, 사본 이름을 원본과 같은 글자 종류만 쓰는 짧은 이름으로 바꾼 빌드, 사본 없이 유닛 파일에 값을 직접 넣은 빌드를 각각 만들어 어느 쪽에서 갈리는지 본다.
