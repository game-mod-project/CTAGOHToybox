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
| 확인한 날짜와 게임 버전 | 확인하지 않음 |
| `options.set`에서 CTAToybox의 표기 | 확인하지 않음 |
| 바닐라 파일의 수정이 게임에 반영됨 | 확인하지 않음 |
| 모드 파일의 수정이 게임에 반영됨 | 확인하지 않음 |
| 수정을 끄고 다시 빌드한 뒤 원래 값으로 돌아옴 | 확인하지 않음 |
| 게임 로그(`game.log`)에 이 모드와 관련된 오류 | 확인하지 않음 |
