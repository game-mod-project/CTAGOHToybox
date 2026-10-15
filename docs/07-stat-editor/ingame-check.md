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
| 확인한 날짜와 게임 버전 | 2026-10-13, v1.067.0 |
| M3A1 하프트랙의 `mine m1a1` 수량 (기대 5) | 5 (2026-10-13, v1.067.0). 게임이 사본 파일 `m3a1_halftrack__ctb_m3a1_halftrack_8da609d3.inc`와 고쳐 쓴 include를 받아들인다 |
| 같은 `.inc`를 쓰는 다른 차량의 수량 (기대 24) | 24 (2026-10-13, `m3a1_halftrack_inf`와 `m3a1_halftrack_inf_late`. 아래 "일괄 확인") |
| `game.log`에 사본 파일이나 include와 관련된 오류 | 없음 (2026-10-13). `__ctb_`나 `m3a1_halftrack`이 든 줄이 없다 |
| 수정을 끄고 다시 빌드한 뒤 24로 돌아옴 | 게임에서 보지 않음. 같은 방식의 원복은 2단계에서 KV-85로 확인했다 |

M3A1이 24 그대로면 게임이 고쳐 쓴 유닛 파일이나 사본을 쓰지 않은 것이다. 그때는 추측으로 원인을 정하지 않는다. `game.log`와 생성물을 먼저 보고, 사본 이름을 원본과 같은 글자 종류만 쓰는 짧은 이름으로 바꾼 빌드, 사본 없이 유닛 파일에 값을 직접 넣은 빌드를 각각 만들어 어느 쪽에서 갈리는지 본다.

## 일괄 확인 (2026-10-13, v1.067.0)

남은 미확인 항목을 한 빌드(`editor/projects/ingame-check-batch.json`과 손으로 만든 `resource/ctb_check.pak`)에 모아 게임 한 번으로 봤다. `CTAToybox`는 `{mods}` 맨 뒤였고 다른 모드 4개가 켜져 있었다. 로그에 `Mount package .../mods/ctatoybox/resource/ctb_check.pak with 2 entries`가 찍혔고, 이 빌드의 파일을 가리키는 오류 줄은 없다.

| # | 넣은 것 | 게임에서 본 값 | 확인된 것 |
|---|---|---|---|
| 1 | `m3a1_halftrack.def`만 `m3a1_halftrack.inc`의 지뢰 24 → 5 | 기본형 5, `m3a1_halftrack_inf` 24 | "이 유닛만" 수정은 같은 파일을 쓰는 다른 유닛에 새지 않는다 |
| 2 | `m3a1_halftrack_inf_late.def`만 `properties/armoredcar.ext`의 수리 키트 5 → 9 | 후기 보병 탑승형 9, 기본형 5 | 두 단계 사슬(`.def` → `.inc` → `/properties/*.ext`)과 루트 기준 경로도 사본으로 바뀐다 |
| 3 | `properties/armoredcar.ext`의 연료통 1 → 3 (전부) | 기본형 3, 후기 보병 탑승형 3 | 공유 파일의 "전부" 수정이 반영되고, 사본에도 함께 들어간다 |
| 4 | `set/stuff/smg/m3_grease`의 `filling` 30 → 17 | 승무원의 그리스건 탄창 17발 | 유닛의 `{item "m3_grease" filled}`는 `set/stuff` 아래 같은 이름의 파일로 이어진다(이 무기 하나로 본 것이다) |
| 5 | 같은 파일의 `("fire rate" rpm(450))` → 60 | 느림 | 매크로 인자를 고치면 반영된다 |
| 6 | `set/stuff/smg/.presets`의 `(define "stick_reload")` 안 `seconds(3.25)` → 12 | 재장전 12초 | `.presets`의 매크로 본문을 고치면 그 매크로를 부르는 파일에 반영된다 |
| 7 | `thompson_m1928a1_50`의 `rpm(830)` → 60 | 보지 않음 | `{from "thompson_m1928a1_50"}`로 물려받는 30발 파일이 바뀌는지는 **미확인** |
| 8 | 한 모드 안에 KV-85 철갑탄을 pak에는 11, 낱개 파일에는 7 | 7 | 한 모드 안에서 pak과 낱개 파일이 겹치면 낱개 파일이 쓰인다 |
| 9 | 채피 철갑탄을 pak에만 9 (MACE의 pak은 18) | 9 | pak끼리 겹쳐도 `{mods}` 뒤쪽 모드의 것이 쓰인다 |

확인 뒤 수정을 모두 끄고 다시 빌드해 생성물을 지우고 `ctb_check.pak`도 지운다. 되돌린 뒤의 값을 게임에서 다시 보지는 않았다.

## 4단계: 화면과 남은 게임 확인

화면의 창 동작과 남은 게임 확인을 한 번에 본다. 확인용 프로젝트는 `editor/projects/ingame-check-app.json`이다.

**화면에서 볼 것** (앱으로 직접 한다. 스크린샷 옵션은 대화 창을 열지 않는다)

| # | 할 것 | 기대 |
|---|---|---|
| 1 | M3A1 Halftrack(`m3a1_halftrack.def`)을 열고 전체 탭에서 `mine m1a1`의 `24`를 두 번 누른다 | 영향받는 유닛 4개 목록, "전부 / 이 유닛만" 둘 다 켜짐 |
| 2 | "이 유닛만", 새 값 `1 2` | 오류 문구, 넣기 단추 꺼짐 |
| 3 | 새 값 `5`로 넣기 | 수정 목록에 "이 유닛만"으로 한 건, 그 행의 수정 칸 `→ 5 (이 유닛만)` |
| 4 | 같은 값을 다시 눌러 `6`으로 넣기 | 수정 목록은 여전히 한 건, 값만 `6` |
| 5 | 그리스건(`set/stuff/smg/m3_grease`)을 열고 요약 탭의 "탄창 또는 포탄을 장전하는 시간"을 두 번 누른다 | "이 유닛만"이 꺼져 있고 이유가 보인다 |
| 6 | 수정 목록에서 체크를 끄고 켠다, 삭제한다 | 목록과 유닛 보기가 바로 바뀐다 |

화면 1~6은 `my-tweaks` 같은 다른 프로젝트에서 해도 된다. 끝나면 그 수정은 지운다.

**게임에서 볼 것** (`ingame-check-app.json`을 빌드해 둔다)

| # | 넣은 것 | 볼 것 | 알게 되는 것 |
|---|---|---|---|
| A | `set/stuff/smg/thompson_m1928a1_50`의 `("fire rate" rpm(830))` → 60 | 톰슨 30발 탄창(`thompson_m1928a1_30`. 파일이 `{from "thompson_m1928a1_50"}`로 시작한다)의 연사. 느려졌는지 | `from` 상속: 물려준 파일의 수정이 물려받는 파일에 반영되는지 |
| B | `set/stuff/mgun/mgun_usa.belt.ammo`의 `inventory/block` 250 → 100 | M26 Pershing(`{item "ammo mgun_usa" 4500}`) 인벤토리의 기관총 탄이 한 묶음에 몇 발인지 | 후보 이름 `ammo mgun_usa`가 이 파일로 이어지는지 |

B의 값을 고른 근거: 같은 형식의 탄약 파일에서 `inventory/block`이 탄창 크기와 같다(`m3_grease_mag.ammo` 30, `lanchester_mag.eng_smg.ammo` 50). 그래서 기관총 탄 한 묶음의 발 수로 보고 골랐다. 키의 뜻은 주석에 없다(미확인). `ammo mgun_usa belt`로 이 파일을 가리키는 차량(예: M3 하프트랙 계열)도 함께 바뀐다.

**기록**

| 항목 | 결과 |
|---|---|
| 화면 1~6 | 기대대로 (2026-10-14, Claude가 `--scenario`와 임시 프로젝트로 직접 함). 1: 영향받는 유닛 4개, 두 범위 모두 켜짐. 2: "값 2: 새 값은 낱말 하나이거나 따옴표로 닫힌 문자열 하나여야 한다" 오류, 넣기 단추 꺼짐. 3·4: 수정 목록 한 건, `24 → 6 (이 유닛만)`. 5: "이 유닛만" 꺼짐, 이유 "매크로 본문 안의 값은 '이 유닛만' 고칠 수 없다". 6: 체크를 끄면 수정 칸에 `[꺼짐]`, 켜면 사라짐, 삭제하면 목록과 수정 칸이 비워짐 |
| A. `from` 상속 | 판단 못 함 (2026-10-14). 사용자 보고 "그대로". 다만 확인 질문이 무기를 하나로 집지 못했다: 30발 톰슨은 `thompson_m1928a1_30`(50발 파일을 물려받음, 병종 31개) 말고도 `thompson_m1a1_30`(별개 파일, 미군 병종 84개), `thompson_m1_30`(12개)이 있다. 미군 병사가 든 것은 대개 M1A1이라 바뀌지 않는 것이 맞다. 빌드와 로드는 정상이었다(`game.log`: ctatoybox가 마지막에 활성화, 오류 없음. 이 파일들을 덮는 다른 모드 없음) |
| B. 후보 이름 | 판단 못 함 (2026-10-14). 사용자 보고 "450". M26 Pershing에는 `{item "ammo hmgun_usa" 450}`(.50 cal AP M2 Rounds, 묶음 `block 100`)과 `{item "ammo mgun_usa" 4500}`(.30 cal, 고친 파일)이 함께 있다. 450은 .50구경의 수량과 같다. 어느 탄을 본 것인지 질문이 집지 못했다 |

### 4단계 다시 확인 (2026-10-14 준비)

위 A·B를 무기와 탄의 게임 속 이름으로 집어 다시 본다. 값은 다른 데서 나오기 어려운 수로 바꿨다. 빌드는 모드 폴더에 들어가 있다(생성물 세 파일, 원본과 그 값만 다름을 diff로 확인).

| # | 넣은 것 | 게임에서 할 것 | 볼 것 |
|---|---|---|---|
| A | `thompson_m1928a1_50`의 `("fire rate" rpm(830))` → 60 (그대로) | 스커미시 후기 미군, 보병 단독(inf2) 버튼의 "Tank Crew"(`single_tankman`)를 산다. 그 안의 "Tank Commander [USAGF]"(`mp/usa/late/tank_commander`)가 `thompson_m1928a1_30`을 든다. 그 병사로 연사한다 | 1초에 한 발쯤으로 느린가 |
| A' (선택) | 같음 | 스커미시 중기 소련, "SMG inf. [KA]"(`mp/rus/mid/smg_m1928`)가 든 `thompson_m1928a1_50` 자체 | 느린가. A가 그대로이고 A'가 느리면 `from`이 고친 파일을 물려받지 않는 것이다 |
| B | `mgun_usa.belt.ammo` `block` 250 → 37, `hmgun_usa.ammo` `block` 100 → 13 | M26 Pershing 인벤토리 | ".30 cal" 탄 칸들과 ".50 cal AP M2 Rounds" 칸들의 숫자. 각각 37, 13씩 나뉘어 있는지, 아니면 4500, 450처럼 한 칸인지 |

**기록** (2026-10-14, v1.067.0, 사용자 확인과 스크린샷)

| 항목 | 결과 |
|---|---|
| A. `thompson_m1928a1_30` 연사 | 느려졌다. **`{from "다른 파일"}`로 물려받는 파일은 물려준 파일의 모드 수정을 그대로 받는다.** 물려준 파일 하나를 고치면 물려받는 파일들도 바뀐다 |
| A'. `thompson_m1928a1_50` 연사 | 빠름(바뀌지 않음)으로 보고됨. 그러나 게임 속 이름 "SMG inf. [KA]"는 `mp/rus/mid/smg_drum`(PPSh-41 드럼)과 `mp/rus/late/smg_stick`도 쓴다. A가 느려졌으므로 고친 부모 파일은 로드됐다. A'에서 본 병사는 PPSh 병사였을 가능성이 높다(판단 못 함. 다시 볼 필요 없음) |
| B. 탄 묶음 | .30 cal 4500, .50 cal 450이 한 칸씩 그대로 보였다. **`inventory/block`은 인벤토리 칸을 나누는 묶음 크기가 아니다.** 뜻은 미확인. 툴팁 ".30 cal AP M2 Rounds (4500) (135.0 kg)"의 이름은 `desc/ammo/mgun_usa belt`의 표시 이름이고 무게는 4500 × `mass 0.03`과 같다. 그래서 후보 이름 `ammo mgun_usa`는 `mgun_usa.belt.ammo`로 이어진다 |

배운 것: 게임 안 대상을 안내할 때는 그 게임 속 이름이 한 병종·한 무기에만 쓰이는지 `data/display-names.csv`에서 먼저 센다.

확인이 끝나 `ingame-check-app.json`의 수정을 모두 끄고 다시 빌드해 생성물을 지웠다.

## 게임 설정 (하위 프로젝트 B, 2026-10-15, v1.067.0)

프로젝트 `editor/projects/settings-check.json`(커밋하지 않음). 한 빌드에 일곱 건을 넣었다. 이 PC에서는 `resources_standard.set`과 `runtime_config.lua`를 MACE Wunderwaffe(`mod_3732639687`)가, `unitlimits.inc`를 MACE(`mod_2905667604`)가 덮어쓰므로, 수정은 그 판에 적용됐다. CTAToybox는 모드 목록 맨 뒤였다.

| # | 고친 것 | 볼 곳 | 기대 | 결과 |
|---|---|---|---|---|
| 1 | `resources_standard.set` 시작 MP 5000 → 9137, 시작 RP 14 → 37 | 새 컨퀘스트(자원 standard) 첫 화면 | MP 9137, RP 37 | **확인**(사용자): MP 9137, RP 37. 새 캠페인 "Conquest 3"(`game.log` 00:02:36 Creating Campaign) |
| 2 | `battle_zones.set` `parameters`의 `scoreStart=250` → 777 | 배틀존 시작 점수 | 777 | 판단 못 함. 스커미시를 하지 않았다 |
| 3 | `unitlimits.inc` `officer=2` → 7 | 스커미시 구매 화면의 Leutnant [Heer](`mp/ger/*/Officer`, `inf_officer` 꼴로 `officer` 태그) 수 제한 | 7 | 판단 못 함. 스커미시를 하지 않았다 |
| 4 | `runtime_config.lua` `WaveOffSecondsByAttackerState` degraded·reduced·retained 60·75·95 → 1135·1165·1210 | AI가 공격하는 컨퀘스트 전투의 `game.log` `[LUA] Print: attacker wave-off pressure … targetWaveOffSeconds=` | 1000 넘음 | **확인**(로그): 00:32:23~00:33:08 `attackerPressure= retained … targetWaveOffSeconds= 1210`, 00:33:23 `attackerPressure= reduced … targetWaveOffSeconds= 1165` |
| 5 | 난이도 배수 | 숫자로 볼 곳 없음 | | 확인하지 않음 |

알게 된 것:

- **모드의 낱개 Lua 파일(`resource/script/...`)은 바닐라와 앞선 모드의 Lua를 덮어쓴다.** 4의 값이 Wunderwaffe의 낱개 파일 값(95·75)이 아니라 CTAToybox의 값이었다.
- **컨퀘스트 자원 시작값(`StartVal`)은 새 캠페인에 들어간다.** 진행 중인 캠페인에 반영되는지는 이번에도 확인하지 않았다.
- 같은 실행에서 우리 파일을 읽다가 생긴 오류는 `game.log`에 없다.

확인이 끝나 수정을 모두 끄고, 게임을 끈 상태에서 다시 빌드해 생성물 네 파일을 지웠다. 확인 중에 만든 캠페인 "Conquest 3"은 시작 자원이 수정된 값(MP 9137)으로 만들어졌다.

## W-G 묶음 (2026-10-15 준비, v1.067.0)

프로젝트 `editor/projects/ingame-check-wg.json`(커밋하지 않음). 한 빌드에 열 건을 넣었다(`docs/10-backlog.md`의 G1·G2·G9~G13). 이 PC에서는 `resources_standard.set`을 MACE Wunderwaffe(`mod_3732639687`)가, `unitlimits.inc`를 MACE(`mod_2905667604`)가 덮어쓰므로 그 판에 적용했다. 나머지는 바닐라 판이다. 빌드한 파일은 원본과 아래 값만 다르다(2026-10-15 diff로 확인).

| # | 백로그 | 고친 것 | 볼 곳 | 기대 | 결과 |
|---|---|---|---|---|---|
| 1 | G1 | `battle_zones.set` `parameters`의 `scoreStart=250` → 777 | 스커미시 배틀존의 시작 점수 | 777 | **확인**(2026-10-15): 양쪽 시작 점수 777 / 1000, 결과 화면 승리 점수 777 |
| 2 | G1 | `unitlimits.inc` `officer=2` → 7 | 스커미시 구매 화면에서 Leutnant [Heer](`officer` 태그)의 수 제한 | 7 | **확인**: 구매 목록에 Officer 7/7(같은 목록의 Flamer 3/3, Sniper 5/5도 `unitlimits`와 같다) |
| 3 | G2 | `resources_standard.set` MP `LoseGain "1:130"` → `"2:111 6:555"` | 이어 하는 컨퀘스트 캠페인(자원 Standard)에서 한 판 지고 늘어난 MP | 130이 아니면 진행 중인 캠페인에도 설정이 들어간다 | **확인**: 사용자 캠페인(conquest 1·2)은 경제 Very High라 이 수정이 닿지 않아 쓰지 않았다. 대신 시험 캠페인 "ctatoybox check2"를 `"2:222 6:555"`일 때 만들어 저장하고, 게임을 끄고 `"1:333"`으로 다시 빌드한 뒤 이어 해 첫 판을 지자 MP 333(옛 값이었다면 0) |
| 4 | G12 | 3과 같은 수정 | 새 컨퀘스트(자원 Standard)에서 네 판 연속 지고 판마다 늘어난 MP | 계단이면 둘째 판부터 111이 이어진다. 칸 사이를 잇는다면 211·311처럼 커진다. 첫 판(첫 칸 2보다 앞)의 값도 적는다 | **확인**: 새 컨퀘스트 "ctatoybox check"에서 1~4패의 MP가 0·111·111·111(같은 판 AP 390·RP 4는 MACE 값 그대로). 계단이고 첫 칸 앞은 0이다 |
| 5 | G9 | `mp/ger/mid/medic.set` `{item "bandage_ger" 22.5 2.5}` → `13 13` | 새 컨퀘스트(독일)에서 Medic(한 번에 2명, `units_ger.set`의 `single_medic`)을 3~4번 사서 병사마다 German Bandage 수 | 0~26 사이로 흩어지면 둘째 수는 ± 폭이다. 모두 13이면 다른 뜻이다 | **확인**: 의무병 6명의 German Bandage가 21·2·24·11·25·4개(인벤토리 I). 둘째 수는 ± 폭 |
| 6 | G10 | `health.inc` `tier2_standard`의 `{"personage" 0}` → 3 | 메인 메뉴 → Library → Soldiers에서 Medic [Heer](tier2_standard)의 퍽과, tier1 병종의 같은 퍽 | tier2는 "Personage (Vet. 3)", tier1은 "(Vet. 0)" | **확인**: AP-Mine Engineer [Heer](`miner-ap.set`, tier2_standard)가 "Personage (Vet. 3)", 다른 tier의 병종은 "Personage"만 |
| 7 | G11 | `set/stuff/gun/37mm_pak35`의 `{parameters "heat" {gravity 2}}` → 0 | (보지 않는다) | | **뺐다**(2026-10-15): 2와 0의 차이를 화면에서 가릴 수 있다는 근거가 없고, HEAT 탄은 후기형 PaK 36(`37mm_pak36_late.def`)에만 3발 있다. 수정은 빌드에 남아 있으나 그 탄에만 걸린다 |
| 8 | G13 | 1v1 Kalinin `varnight1~3.set`의 `{night_vision 0.75}` → 0.4 | 스커미시 1v1 Kalinin을 낮과 밤으로 열어 같은 유닛의 시야 표시선 반경을 견준다 | 밤이 낮의 절반 아래로 줄면 시야 배수다(바닐라 밤은 0.75) | **확인하지 못함**: 밤 전투는 열렸지만(로그 "With options: night_weather") 일반 게임 화면에 시야 범위를 그리는 표시가 없다. 기본 키(`interface.pak`의 `profiles/default/keys.set`)에 그런 키가 없고 `v_visor`는 바닐라 주석에만 나오는 개발용 표시다 |

이번에 넣지 않은 것: G3(난이도 배수를 숫자로 볼 곳을 아직 찾지 못했다), G4(새 가설이 먼저다), G5(고르는 규칙을 시험할 설계가 먼저다), G6(`.mdl`은 편집기로 고치지 않는다), G7·G8(낮은 우선).

확인 순서 제안: 6(메뉴) → 1·2·8(스커미시) → 5(새 컨퀘스트) → 4(같은 새 컨퀘스트에서 지기) → 3(이어 하는 캠페인). 7은 뺐다.

2026-10-15에 AI가 PowerShell로 게임을 직접 조작해 확인했다(게임 창만 찍고, 게임을 맨 앞에 두고 마우스·키 입력을 보냄). 게임은 마우스를 Raw Input으로 읽어 맨 앞 창일 때만 입력을 받는다. 확인하는 동안 `options.set`을 창 모드(1360×768)로 바꿨다가 끝에 백업으로 되돌렸다. 사용자 세이브 conquest 1·2는 시작 전에 백업했고 끝에 해시가 같았다. 시험 캠페인 "ctatoybox check", "ctatoybox check2"는 프로필에 남겨 두었다(지워도 된다). 확인이 끝나 수정을 모두 끄고 게임을 끈 채 다시 빌드해 생성물을 지웠다.

알게 된 것:

- **breed 아이템의 둘째 수는 ± 폭이다.** 병사마다 첫 수 ± 둘째 수 사이에서 개수가 정해진다.
- **`{"personage" N}`은 소생이 켜지는 숙련도다.** Library의 퍽에 "(Vet. N)"으로 보인다.
- **단계표 "판:값"은 계단이다.** 그 판부터 다음 칸 전까지 같은 값이고, 첫 칸보다 앞의 판은 0이다.
- **진행 중인 캠페인도 지금 파일의 설정을 쓴다.** 캠페인을 만들 때 값을 저장해 두지 않는다(적어도 LoseGain).
- 컨퀘스트에서 지는 가장 빠른 길은 전투 메뉴의 RETREAT다. 확인이 끝나면 게임을 끈 상태에서 수정을 모두 끄고 다시 빌드해 생성물을 지운다.
