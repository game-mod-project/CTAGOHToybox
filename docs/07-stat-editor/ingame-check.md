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

## W-G 둘째 묶음 (2026-10-16, v1.067.0)

남은 게임 안 확인(G3–G8, G11, G13)을 AI가 게임을 직접 조작해 확인했다. 프로젝트 `editor/projects/ingame-check-wg2.json`(커밋하지 않음, 수정 16건)과 손으로 넣은 `panzer4g_x.mdl` 사본 하나를 썼다. 스커미시는 "Test Mode"(자원 무제한)로 열고 적 칸을 닫았으며, 유닛은 구매 카드와 Spawn Menu 모드(`mod_3617250510`)로 불렀다. 확인이 끝나 수정을 모두 끄고 다시 빌드해 생성물과 사본을 지웠고 `options.set`·`mod.info`를 되돌렸다.

| 백로그 | 고친 것 | 본 것 | 결과 |
|---|---|---|---|
| G3 | `difficulty/multiplayer.dl` `{any {health 1.0}}` → 3.0 | 스커미시(로그 `difficulty=multiplayer`)의 Ersatz-Grenadier 분대장(tier1, 체력 150%)과 핀란드 Reserve 분대장의 체력 | **확인**: 450/450(= 100 × 150% × 3.0). 난이도 파일의 `health`가 병사 최대 체력에 곱해진다 |
| G4 | `smg/mp40.ammo` `{block 32}` → 7 | 분대장의 MP40 탄창(breed `"mp40 ammo" 192`, 무기 `filling "ammo mp40" 32`) | 탄창은 32발씩 6개 그대로. **`block`은 탄창 크기도 아니다**(뜻은 미확인) |
| G5 | `mgun_ger.*.ammo` 9개의 `mass`를 0.011~0.019로 서로 다르게 | Panzer IV G(`"ammo mgun_ger belt" 2850`)와 핀란드 Maxim MG 08(`"ammo mgun_ger" 1000`) 인벤토리 툴팁 무게 | **확인**: 31.4kg·11.0kg = 수량 × 0.011, 둘 다 `mgun_ger.belt.ammo`. 계열 이름은 후보 9개 가운데 이 파일로 간다. 탄 `mass`는 한 발 무게(kg)다 |
| G6 | `panzer4g_x.mdl` 사본에서 엔진·엔진 덮개·궤도·포 등의 뼈 모델을 `turret.ply`로, 포탑을 `body.ply`로 | Panzer IV G의 엔진을 37mm 철갑탄으로 쏴 "Engine Broken" | **확인**: 엔진 덱 자리에 포탑 모양 두 개가 나타났고 부서지지 않은 부품은 그대로였다. 부품이 부서지면 그 부품을 `_x.mdl`의 같은 뼈 모델로 바꾼다 |
| G7 | 없음 | 에디터(`call_to_arms_ed.exe`)를 띄워 `game.log` | **일부 확인**: "Scanning mods in …/mods/ — Found 2 available mods", "Mod: activate mod ctatoybox". 에디터는 게임 `mods` 폴더의 로컬 모드만 찾아 켜고 창작마당 모드(MACE 등)는 켜지 않는다. 게임은 같은 `options.set` 순서로 창작마당 모드까지 켠다(위 47행). 에디터 안의 조작(F2·F3)은 보지 않았다 |
| G8 | `mod.info` 버전 `"any"` → `"1.000"`·`"1.010"` | EXTRAS의 모드 목록 | **확인**: 회색 `CTATOYBOX [1.0-1.10]`, 빨간 "This mod is not up to date. Please wait for the mod author to update it.", 켜기·끄기 단추 없음(DELETE만). `options.set`의 줄은 남는다 |
| G11 | `37mm_pak35` `{parameters "heat" {gravity 2}}` → 60, 후기형 PaK 36의 HEAT 3 → 30발 | 직접 조종(E)으로 HEAT를 조준하고 쏨 | **확인**: 조준점이 커서를 따라가지 않고 포 앞 2~4m에 묶였고, 쏜 탄이 그 자리(2.7m)에 떨어졌다. 철갑탄으로 바꾸면 조준점이 커서를 따라간다. `gravity`가 크면 탄이 빨리 떨어져 닿는 거리가 준다 |
| G13 | Kalinin `varnight1~3.set`의 `night_vision` 0.75 → 0.4, 다음 실행에서 0.05 | 밤 판에서 적 분대가 보이는 거리 | **판단 못 함**: 0.4에서는 35.0m의 적 분대가 보였다. 0.05에서는 Spawn Menu로 놓은 적 분대가 8~30m 어디서도 보이지 않았지만, 결과 화면의 처치 수가 0이라 적이 정말 놓였는지 증명하지 못했다. 바닐라 0.75 대조도 없다 |

알게 된 것(조작):

- 게임은 마우스를 Raw Input으로 읽어 맨 앞 창일 때만 입력을 받는다. 창에 보낸 메시지는 무시한다.
- 스커미시 로비에서 "Game settings → Test Mode"는 자원 무제한, AI 칸 오른쪽 클릭 → "Close slot"으로 적을 비우고 "Start with empty enemy team?"에 YES.
- Spawn Menu의 국기 단추는 누를 때마다 소속을 돈다(독·소·영·미·핀·일·TEST). 배정 단추는 Auto Assign → Player 0 → … → Player 9로 돈다. 이번 판에서 나는 Player 1이었다. 항목을 누르면 놓을 자리에 초록 원이 커서를 따라오고, 한 번 더 누르면 놓는다.
- 인벤토리 I, 직접 조종 E, 다음 탄 Space, 카메라를 고른 유닛에 Shift+F, 카메라 이동 W.

## W-G 셋째 묶음 (2026-10-16, v1.067.0)

남은 G4·G5·G7·G13을 AI가 게임과 에디터를 직접 조작해 확인했다. 프로젝트 `editor/projects/ingame-check-wg3.json`(커밋하지 않음, 수정 9건)과 손으로 넣은 파일 네 개(후보 사본 `mgun_ger.zz.ammo`·`mgun_ger.aaa.ammo`·`mgun_ger.aa.zz.ammo`, `breed/mp/ger/late/ersatz_gren_sl.set` 사본)를 썼다. 스커미시는 Test Mode, 밤은 "Mission Options → Nighttime"(로그 `With options: night_weather`)으로 열었다. 확인이 끝나 수정을 끄고 다시 빌드해 생성물과 사본을 지웠고 `options.set`을 되돌렸다.

| 백로그 | 고친 것 | 본 것 | 결과 |
|---|---|---|---|
| G4 | `smg/mp40.ammo` `{block 32}` → 7, `inventory/weight` 3.1 → 10 | 분대장 인벤토리 툴팁, 직접 조종으로 탄창을 비운 뒤의 탄 수 | 칸마다 "9mm MP38/40 Magazine (10) (0.2 kg)". 한 칸의 발 수는 `weight`가 정한다(100/10 = 10, 바닐라 100/3.1 → 32). 쏘기 전 32/192, 다 쏜 뒤 0/192, 재장전 뒤 32/160. 재장전 양은 무기의 `filling` 32다. **`block`은 재장전 양도 아니다** |
| G4 | `ersatz_gren_sl.set` 사본에서 `{item "mp40 ammo" 192}` → `{item "mp40 ammo"}`, `{item "ammo mgun_ger belt"}` 추가 | 구매 카드로 산 Ersatz-Grenadier 분대장 인벤토리 | 두 탄 모두 수량 없이 "(0.0 kg)" 한 칸(1발). **개수 없이 적은 아이템은 1개이고 `block`(7, 300)이 아니다** |
| G5 | 후보 사본 `mgun_ger.zz.ammo`(mass 0.022), `mgun_ger.aaa.ammo`(0.021), `mgun_ger.aa.zz.ammo`(0.023)를 차례로 더함. `mgun_ger.belt.ammo`는 바닐라(0.0367) | 핀란드 Maxim MG 08(`"ammo mgun_ger" 1000`)의 탄 툴팁 | zz만 더했을 때 "7.92mm Belt-Fed Rounds (1000) (36.7 kg)" = belt. aaa를 더하자 "desc/ammo/mgun_ger aaa (1000) (21.0 kg)". aa.zz를 더하자 "desc/ammo/mgun_ger aa zz (1000) (23.0 kg)". **확인**: 후보 가운데 파일 이름순(점이 글자보다 앞) 맨 앞 파일을 고른다. 낱말 수(aa.zz는 4조각)와 계층(모드·바닐라)은 상관없다 |
| G7 | 없음(빌드는 이 묶음 그대로) | 에디터(`call_to_arms_ed.exe`)에서 엔티티를 놓고 실행 | **일부 확인**: F2를 누르자 오른쪽 아래에 "RESET ALL INVENTORIES" 단추가 생겼다. 엔티티 목록(`-vehicle/germany/tank_medium/hummel`)을 고르고 맵을 누르면 놓인다. 새 맵에서 왼쪽 위 START를 누르면 "You have to store the map before!"가 나온다. Esc 메뉴 → Load…로 `map/multi/1v1_farmland`을 연 뒤 놓고 START를 누르자 시뮬레이션이 돌았다(로그 "Starting simulation on difficulty [normal]", 단추가 FINISH로 바뀜). 놓은 차량은 승무원이 없어 고를 수 없었고 인벤토리는 보지 못했다. 맵은 저장하지 않았다. 이번 에디터는 "Found 7 available mods"로 창작마당 모드까지 `options.set` 순서대로 켰다(둘째 묶음에서는 2개만 찾았다. 조건은 미확인) |
| G13 | Kalinin과 Farmland의 `varnight1~3.set` `night_vision` 0.75 → 0.05 | 밤 판에서 적 AI 소속으로 놓은 분대를 내 분대 가까이에서 봄 | Kalinin 0.05: 직접 조종 거리 7.8m에 놓은 적 분대가 쏘는데(총구 불빛만 보임) 병사는 그려지지 않았다. Farmland: 같은 카메라·같은 자리(내 소총 분대에서 6.5m)에 적 분대를 놓자 0.75 판에서는 내 소총 분대가 3초 안에 8명을 쓰러뜨렸고 0.05 판에서는 내 소총 분대의 처치가 0이었다(그 적은 불빛 아래 다른 분대가 쓰러뜨렸다). **값이 낮을수록 밤에 적을 알아보는 거리가 준다는 쪽과 맞다.** 맵의 불빛과 AI 움직임이 섞여 배수가 어디에 곱해지는지는 미확인 |

알게 된 것(조작):

- 이 실행에서는 `options.set`에 있는데도 Spawn Menu 모드가 켜지지 않았다(로그에 `Mod: activate mod mod_3617250510`이 없음). EXTRAS → 모드 화면에서 켜자 게임이 모드 목록을 다시 쓰며 `ctatoybox`를 맨 앞으로 옮겼다. 시험 전후로 `options.set`을 백업에서 되돌린다.
- 적 AI가 있는 판에서는 Spawn Menu의 번호가 소속을 가른다. 이번에는 Player 1이 적 AI, Player 2가 나였다(처치 기록의 이름과 고를 수 있는지로 가렸다).
- 개수 없이 적은 탄은 툴팁에 수량이 나오지 않는다. 한 발이다.

## W-G 넷째 묶음 (2026-10-16, v1.067.0)

펼치기 규칙(G15)과 원형 상속(G16), `##` 블록(G17)을 AI가 게임을 직접 조작해 확인했다. 빌드 없이 시험 파일을 모드 폴더에 손으로 넣었고, 끝나고 지웠다(모드 폴더에는 `.gitkeep`만 남았다). `options.set`은 백업으로 되돌렸다.

| 백로그 | 넣은 것 | 본 것 | 결과 |
|---|---|---|---|
| G15 정의 순서 | `smg/mp40_unfolded`의 `filling`을 `(define "ctb_d1" … 7) (define "ctb_d1" … 3) ("ctb_d1")`로 | Volksgrenadier 분대의 MP40 | "9mm MP38/40 Magazine (3/3)". **같은 이름을 두 번 정의하면 나중 정의가 쓰인다** |
| G15 `.presets` 적용 | `rifle/.presets`(MACE판 사본) 끝에 `(define "ctb_up" {filling "ammo stg44" 9})`, `rifle/stg44`의 `filling`을 `("ctb_up")`로 | Volksgrenadier 분대장의 StG 44 | "7.92mm-K. MP-44 Magazine (9/9)". 폴더의 `.presets` 정의는 include 없이 그 폴더 파일에서 쓰인다 |
| G15 바깥 인자 물려받기 | `rifle/k98`에 `(define "ctb_in" {filling … %n}) (define "ctb_out" ("ctb_in")) ("ctb_out" n(13))` | 게임 시작 | **시작할 때 꺼졌다**: "Block terminator expected! (eSDLReader.cpp:614) > file "/set/stuff/rifle/k98" >> file "define 'ctb_out'" > file "define 'ctb_in'" `{filling "ammo rifle_ger clip" %n}`". 안쪽 매크로는 바깥 호출의 `n(13)`을 받지 못했다. 그래서 `%n`이 채워지지 않은 채 읽히다 멈췄다. **엔진은 매크로 안에서 부른 매크로에 바깥 인자를 물려주지 않는다** |
| G15 조상 `.presets` | `rifle/sniper/mosin_m27_scope`의 `filling`을 `("ctb_up")`로, `ctb_up`은 `rifle/.presets`에만 | 게임 시작 | **시작할 때 꺼졌다**: "define not found (eSDLReader.cpp:455) > file "/set/stuff/rifle/sniper/mosin_m27_scope" `("ctb_up")`". **조상 폴더의 `.presets`는 하위 폴더의 파일에 쓰이지 않는다** |
| G16 | `rifle/k98_ctbparent`(바닐라 K98 사본, `filling` 9), `rifle/k98`을 `{from "k98_ctbparent" {filling "ammo rifle_ger clip" 3}}`로 | Ersatz-Grenadier 소총수의 Karabiner 98k | "7.92mm Mauser Rounds (3/3)". **원형 상속에서 자식 파일의 같은 키가 이긴다**. 자식에 적지 않은 키(무기 이름·모델 등)는 부모 것을 쓴다(Karabiner 98k로 보였다) |
| G17 | 없음(바닐라 그대로) | Spawn Menu로 놓은 핀란드 PzKw IVJ(`{##extender "smokescreen"`)와 독일 Pz.Kpfw IV Ausf.J(켜져 있음) | 두 전차의 단추 줄과 명령판을 견줬다. 다른 것은 독일 쪽의 "Close Hatch" 단추 하나뿐이었다(연막과 무관). 연막 장치는 인벤토리의 `nbks grenade`(핀란드판은 `;`로 꺼짐)로 쓰므로 단추로는 가리지 못했다. **판단 못 함** |

알게 된 것:

- 하위 폴더 `rifle/sniper/.presets`를 모드에 넣었을 때도 MACE의 `hanyang_rifle`이 "define not found"로 멈췄다(`("bolt_action_m88")`). 그 정의는 MACE판 `rifle/.presets`에 있다. 모드의 `rifle/.presets`를 바닐라 사본으로 덮어 MACE 정의가 사라졌기 때문이다. **모드가 `.presets`를 덮으면 앞 모드가 그 파일에 더한 정의가 모두 사라진다.** 편집기는 이긴 계층의 파일을 원본으로 쓰므로 빌드로 만든 사본에는 이 문제가 없다.
- 위 두 시험이 증명하듯, 엔진은 SDL에 문제가 있으면 그 파일을 읽는 순간 게임을 끈다. 시험 파일은 하나씩 나눠 넣는 것이 안전하다.
- Spawn Menu의 국기 순서는 독→소→미→영→핀→일→TEST다(앞 기록과 다름). 목록 아래쪽 항목은 잘 눌리지 않는다. 휠로 올린 뒤 누른다.
- 오프라인 프로필(`profiles/offline`)로 뜨면 창작마당 모드가 하나도 켜지지 않는다(로그 "Found 2 available mods"). Steam이 서버와 다시 연결된 뒤 다시 띄우면 7개가 켜졌다.

## 월드워 시야 (2026-10-16, v1.067.0)

월드워 시야 프로젝트(설계 `docs/superpowers/specs/2026-10-16-worldwar-vision-design.md`)를 AI가 게임을 직접 조작해 확인했다. 빌드 세 판을 차례로 넣었다: 빈 프로젝트(바닐라 시야), `editor/projects/월드워-워썬더.json`(수정 68건), `editor/projects/월드워.json`(67건). 판마다 Farmland 스커미시를 Test Mode로 열고 적 칸을 닫았다. Spawn Menu로 내 M4A2 (Early)(소련 국기, Player 1)를 같은 길에 놓고, 그 전차를 골라 Shift+F로 카메라를 맞춘 뒤 가장 멀리 당겨 찍었다. 끝나고 `options.set`을 백업으로 되돌렸다. 출력 모드는 월드워(WoT식) 빌드로 남겼다.

| 판 | 본 것 | 결과 |
|---|---|---|
| 셋 모두 | 시작과 전투 중 `game.log` | SDL 오류(`eSDLReader`) 없음. 세 판 모두 정상으로 시작했다 |
| 바닐라 | 전차 둘레의 안개(아무 아군도 보지 못하는 곳은 흐리고 회색으로 그려진다)와 미니맵 | 옆·뒤는 약 40~50m부터 흐리다. 미니맵에 밝혀진 곳이 거의 없다 |
| 월드워 (WoT식) | 같은 구도 | 화면(가로 약 100m) 안이 모든 방향으로 맑다. **미니맵에 전차를 중심으로 한 둥근 원이 밝혀졌다.** 지상 필드의 `h_fov` 360이 게임에 쓰여 시야가 유닛 중심의 원이 된다 |
| 월드워-워썬더 | 같은 구도 | 화면 안이 맑고(왼쪽 아래 끝만 조금 흐림), 미니맵에서 아래쪽 절반 가까이가 밝혀졌다. r ×4가 시야 거리를 늘린다 |

비교 그림: [worldwar-vision-compare.jpg](worldwar-vision-compare.jpg)(위부터 바닐라, 월드워, 월드워-워썬더. 오른쪽은 미니맵).

확인하지 못한 것(아래 "G19"에서 일부 이어서 봤다):

- 거리별 적 발견. 스펙의 합격 기준인 "옆·뒤의 멈춘 적 전차를 150m에서 발견", "400m 넘는 적을 발견"은 이 판에서 재지 않았다.
  - 적 AI가 있는 판: AI가 산 부대와 Spawn Menu로 놓은 AI 소속 전차가 시험 자리에서 싸움을 벌였다.
  - 적 칸을 닫은 판: Player 2로 놓은 독일 전차가 적이 아니었다(서로 쏘지 않았다).
  - 이번 판단의 근거는 안개 경계, 곧 시야의 모양과 넓이다.
- 근접 발견 50m(`uncover` 50): 덤불 안 40m에 엎드린 적 보병이 보이는지. 놓지 않았다.
- 안개 거리(`full_opaque_view_distance` 1500m)보다 먼 적이 그려지는지(워 썬더식). 그 변형을 쓰지 않기로 해 보지 않는다.
- 프레임. 짧은 시험에서 끊김은 느끼지 못했지만 재지 않았다.

### G19: 월드워의 거리별 발견 (2026-10-16, 월드워(WoT식)를 쓰기로 정한 뒤)

출력 모드는 월드워 빌드 그대로다. 내 전차는 M4A2 (Early)다. 주 시야는 `tank_range_200`(포수, r 1.5)이고, `spotting.inc`의 식으로는 멈춘 중형 전차(`vehicle_04`, 중전차 `vehicle_05`도 같은 값)를 1.5 × 100 = 150m, 움직이면 1.5 × (100 + 50/1.5) = 200m에서 찾는다. 같은 전차의 다른 시야는 조종수(`tank_driver` r 0.85), 둘레(`tank_around` r 0.5), 사람(`human` r 1.0)이다. 거리는 직접 조종(E)의 조준점 숫자로 쟀다.

| 판 | 놓은 것 | 본 것 |
|---|---|---|
| Farmland, AI: Rookie, 낮(비) | 적 AI(이 판에서 Player 2) Pz.Kpfw III F를 내 전차 옆(북동) 163.5m에 멈춘 채로 | 놓은 뒤 2·6·12초 동안 그려지지 않았다 |
| 같은 판 | 같은 방향 126.3m에 하나 더 | 2·6초 동안 그려지지 않았다. 내 전차와 그 사이에 나무와 울타리 줄이 있다 |
| 같은 판 | 없음 | 두 적 전차는 40초쯤 뒤 내 전차 옆까지 와서 보였다(놓인 것은 맞다) |
| 앞 판(AI 있음) | 적을 내 전차에서 159m, 내 둘째 전차에서 약 60m에 | 바로 보이고 싸움이 났다(가까운 둘째 전차가 찾았다) |
| Farmland, 적 칸 닫음, 낮(맑음) | 내 전차 하나 | 뒤쪽(남동) 안개 경계: 70.0m는 색, 82.4~94.2m는 섞임, 103.8m부터 회색. 그쪽은 땅이 오르막이다 |

판단:

- 옆·뒤 시야가 바닐라(둘레 필드 50m)보다 멀다(뒤쪽 약 90~100m까지 색). 360° 시야가 게임에 쓰인 것과 맞다.
- **계산한 150m는 확인하지 못했다.** 나무 뒤 126m의 멈춘 적은 보이지 않았고, 뒤쪽 안개 경계는 약 100m였다. 163.5m의 적이 안 보인 것은 150m 한계와 맞지만, 126m도 안 보였으니 거리 때문인지 가림 때문인지 가를 수 없다.
- 원인 후보(확인하지 않음):
  - 지형·나무가 시선을 가렸다.
  - 안개 경계가 포수 필드가 아니라 다른 필드(사람 필드 r 1.0 → 100m)로 정해진다.
  - 포수 필드가 다른 이유로 둘레에 쓰이지 않는다.
- 다음 확인(백로그 G19): 평지에서 앞(포수 정면)과 뒤의 경계를 같은 판에서 견준다. 사람 필드만 r 0.5로 낮춘 빌드에서 뒤쪽 경계가 줄어드는지 본다.

알게 된 것(조작):

- 적 AI가 있는 판의 Spawn Menu 번호는 판마다 다르다. 이번에는 Player 1이 나, Player 2가 적 AI였다(앞 판은 반대). 놓은 전차가 내 선택 목록에 들어오면 내 것이다.
- 로비의 Mission Options에서 "Daytime"을 고르면 밤이 나오지 않는다.
- 카메라는 WASD로 움직인다(화살표 키와 화면 가장자리는 이 창 모드에서 움직이지 않았다). Shift+F는 고른 유닛으로 카메라를 옮긴다.
- 적 AI가 있으면 Battle Zones 점수로 6분 안팎에 판이 끝난다(AI가 1001점).

알게 된 것(조작):

- 아군이 아무도 보지 못하는 곳은 화면에서 흐린 회색으로 그려진다. 유닛 하나를 놓으면 그 시야만 색이 돌아온다. 시야 모양을 보는 가장 빠른 방법이다.
- Spawn Menu의 배정 단추:
  - 적 AI가 있는 판: Player 1이 적 AI, Player 2가 나였다(앞 기록과 같다). Auto Assign과 독일 국기로 놓은 전차는 적 AI 것이 되었다.
  - 적 칸을 닫은 판: Player 1이 나였다. 소련 국기와 Auto Assign으로는 아무것도 나오지 않았다.
- 커서를 창 가장자리에 두면 카메라가 밀려간다. 찍기 전에 커서를 화면 위쪽 가운데(점수판 근처)에 둔다.
- 로비의 Game settings 목록은 펼침 단추(▼)를 한 번 눌러야 열린다. 두 번 누르면 닫힌다.
### G19 이어서 (2026-10-16, 두 번째)

빌드는 그대로(월드워 WoT식)다. 판은 Farmland 스커미시, Test Mode, 소련이다. 화면 표시가 무엇을 뜻하는지 먼저 정리하라는 사용자 지시로 중간에 멈췄다. 표시의 뜻은 [ingame-displays.md](ingame-displays.md)에 정리했다.

| 판 | 본 것 | 결과 |
|---|---|---|
| 적 AI 있음(핀란드), 비(`var2`) | Spawn Menu를 열기 전 | 내 소속 독일 Tiger II 세 대가 이미 나와 싸우고 있었다. `game.log`에 `Multiple threads spawned by trigger 重型坦克/ger/7.6`(3번), `…/7.4`와 내 Tiger II의 전과가 있다. 사용자가 Spawn Menu로 소환한 것이다(사용자 확인). 이 판은 닫았다 |
| 적 칸 닫음, 비(`var2`, `night_vision` 1) | M4A2 (Early) 하나를 길에 놓고 미니맵을 확대 | 미니맵에 전차를 중심으로 한 원. 전차가 동쪽을 볼 때 동서 반경이 거의 같다(4배 확대에서 125px, 130px. 확대 전 약 42px, 미니맵 한 변 약 156px). 포수 정면만 길고 뒤가 짧은 모양이 아니다 |
| 같은 판 | 전차를 서쪽 들판 가운데로 보내 직접 조종(E)으로 거리를 읽음 | 숫자는 29.9~100.6m까지만 나오고 그보다 먼 곳에서는 나오지 않았다. 사용자가 사거리라고 짚었다. 그래서 이 숫자로 100m 너머의 안개 경계를 잴 수 없다 |
| 같은 판 | 서쪽을 본 화면 | 전차 쪽이 색이 있고 화면 왼쪽 위(먼 쪽)가 회색이었다. 경계까지의 거리는 위 이유로 읽지 못했다 |
| 메인 메뉴와 전투 중 | 콘솔 키 후보 다섯(`` ` ``와 Ctrl·Shift·Ctrl+Shift·Alt 조합) | 열리지 않았다. 디버그 표시(`v_visor`)는 보지 못했다 |

판단:

- 앞과 뒤의 경계는 미니맵에서 거의 같다. 미터 값은 아직 없다.
- 첫 G19의 거리 숫자 117.7m, 163.5m와 이번 100.6m 한계가 왜 다른지는 미확인이다.
- 첫 G19의 "뒤쪽 경계 약 100m"는 날씨 변형 탓일 수 있다(후보, 확인하지 않음). Farmland의 `var3`는 `night_vision` 0.6이다. 이 값이 시야에 곱해진다면 150m는 90m가 된다. 첫 G19 판의 날씨 변형은 적지 않았다.
- 하지 못한 것: 사람 필드만 r 0.5로 낮춘 빌드, 덤불 안 40m에 엎드린 적 보병.
- 그 뒤 조사에서 적을 누르면 그 적의 시야 윤곽이 그려진다는 것을 알았다(개발자 공지 Update 53, 안개를 켠 판만). 이번에는 쓰지 않았다. 다음 G19는 이것부터 본다.
- 게임은 17:33에 로비를 나가 닫혔다. 그 뒤 `options.set`을 백업으로 되돌렸다(해시 일치).

알게 된 것(조작):

- 왼쪽 유닛 카드를 두 번 누르면 카메라가 그 유닛으로 간다. 직접 조종 중에는 카메라가 따라가지 않는다.
- 적 칸을 닫은 판의 내 번호는 로그 `Game: Add player 2 Velka`와 상관없이 Spawn Menu에서 Player 1이었다.

### G19 셋째 (2026-10-16, 세 번째)

사용자 지시로 남은 미확인 셋을 찾았다: 거리 숫자가 약 100m에서 사라지는 이유(사용자: 주포 사거리가 100m를 넘는지 보라), 안개 경계가 무엇의 거리인지, `night_vision`이 시야에 곱해지는지.

- 빌드는 그대로다(월드워 WoT식).
- 판은 Farmland, Test Mode, 소련이고, 로비 ADVANCED의 Victory Points to win을 5000(최대)으로 올렸다.
- 미니맵은 원본 크기로 잘라(240×220px) 밝기가 바뀌는 곳으로 원의 폭을 쟀다.
- 해 질 녘 판이 `var3`인 근거: 그림자가 동쪽으로 지고 그늘이 푸르며 빛이 주황이었다. `var3`의 빛 방향(x +0.48), 주변광 `0xff1a232d`, 확산광 `0xff884523`과 맞는다. `var1`은 빛 방향 x −0.322에 회색 빛이다.

| 판 | 놓은 것·한 것 | 본 것 |
|---|---|---|
| (파일) | M4A2 (Early) 주포 `75mm_m3`, 탄 `bulletusa_75x350 apcbche`·`he` | 사거리 190m: MACE `set/stuff/gun/.presets`의 `range_190` = `{range 190 190}`, 탄종마다 `("penetration_medium" … range(190))`. `gun.pattern` 주석: `{maxRange}`는 "직접 조종에서 플레이어의 최대 사격 거리". 동축 기관총 `browning_m19a4_tank` 120m, 전차장 `browning_m2` 140m |
| 낮(비 `var2`), 적 AI 있음 | 내 M4A2 | 툴팁 "Range (current ammo): 190 m" |
| 같은 판 | AI 소속 M4A2를 내 전차 앞 길에 놓고, 빈 땅을 눌러 고른 것을 푼 뒤 그 적을 누름 | 적 전차 둘레에 빨간 원판이 그려졌다(반지름은 전차 길이로 어림해 약 5~6m). 곧 내 전차 승무원이 죽어 안개가 덮였고 더 먼 선은 보지 못했다 |
| 해 질 녘(`var3`, `night_vision` 0.6), 적 칸 닫음 | 남쪽 길에 M4A2(전차 1), 북쪽 들판에 Conscript rifle squad | 미니맵 원 폭: 전차 83~84px, 보병 분대 56px. 비 1.48. 전차 원은 비 판(`var2`, 두 번째 확인)의 84px와 같다 |
| 같은 판 | 북쪽 들판에 M4A2 하나 더(전차 2). 전차 1을 직접 조종해 전차 2를 겨눔 | 384.1m(빨강). 미니맵에서 두 전차 표시가 105.5px 떨어져 있어 1px = 3.64m |
| 같은 판 | 전차 1 직접 조종, 카메라를 북쪽으로 옮겨 여러 곳을 겨눔 | 188.7m는 흰 숫자(초록 점), 196.5m부터 빨강. 217.7, 296.2m, 460.1~498.9m까지 빨간 숫자가 나왔다 |
| 밤(Nighttime, `varnight*`, `night_vision` 0.75, `{night}` 있음), 적 칸 닫음 | 같은 자리에 M4A2 | 미니맵 원 폭 63px. 낮 83.5px의 0.755배 |

판단:

- **주포 사거리는 190m다.** 파일, 툴팁, 그리고 거리 숫자 색이 약 190m에서 흰색에서 빨강으로 바뀌는 것이 모두 맞는다. 100m를 넘는다.
  - 거리 숫자는 사거리 밖에서도 빨갛게 나왔다(500m 가까이). 두 번째 확인에서 약 100m 너머 숫자가 사라진 것은 사거리 때문이 아니다.
  - 그때의 원인은 재현하지 못해 미확인이다. 커서를 더 멀리 옮겨도 숫자가 92~97m에 머문 것은 조준점이 울타리·나무에 걸린 경우(docs/09 V13)와 맞는다(추정).
- **안개 경계는 `cannon_00`을 찾는 거리(필드 r × 100m)다.**
  - 두 가설의 예상: 보병(사람 필드 r 1.0)은 서 있는 병사도 `cannon_00`도 100m다. M4A2 포수 필드는 `cannon_00` 150m, 서 있는 병사 99m(100 × `r_human` 0.66 × r 1.5)다.
  - 원의 비는 1.48이었다. `cannon_00`이면 150/100 = 1.5, 서 있는 병사면 1.0이다.
  - 미터로도 맞는다: 전차 원 반지름 41.75px × 3.64 = 152m, 보병 28px × 3.64 = 102m(분대가 퍼진 몇 m 포함).
- **월드워 빌드의 M4A2 시야는 모든 방향 150m다.** 첫 G19의 "뒤쪽 경계 약 100m"와는 맞지 않는다. 그때는 경계를 거리 숫자로 읽었는데, 숫자가 장애물에 걸렸을 수 있다(추정).
- **밤에는 시야 거리가 약 25% 준다(0.755배).**
  - 개발자 공지 Update 9 "Night-time reduces vision ranges by 25%"와 밤 환경의 `night_vision` 0.75에 맞는다.
  - 낮 `var3`의 `night_vision` 0.6은 원을 줄이지 않았다.
  - 밤 환경에만 있는 `{night}` 표시가 있을 때만 `night_vision`이 곱해지는 것으로 보인다(추정). 이번 두 판과 G13의 밤 판 결과로 낸 판단이고, 값을 바꿔 본 빌드는 아직 없다.
- 사람 필드만 r 0.5로 낮추는 시험은 하지 않아도 된다. 안개 경계가 `cannon_00` 거리임을 위에서 확인했다.
- 적의 시야 윤곽:
  - 적을 눌렀을 때 보인 작은 빨간 원판은 이 PC `hearing.inc`의 `cannon_00` 5m(듣는 거리)와 크기가 비슷하다(추정).
  - 쐐기와 큰 원은 내 관측 유닛이 먼저 죽어 보지 못했다.

알게 된 것(조작):

- 로비에서 적 칸을 닫고 START를 누르면 "Start with empty enemy team?" 확인 창이 뜬다. YES를 눌러야 시작한다. 이번에 이 창을 보지 않고 로그만 기다려 사용자가 짚었다. 단추를 누른 뒤에는 화면을 찍어 확인 창이 있는지 본다.
- 로비 ADVANCED에서 Victory Points to win(최대 5000)과 Fog of War(Realistic)를 고칠 수 있다. 처음에 확인 창이 뜨면 YES를 누른다.
- AI 칸을 오른쪽 단추로 누르면 Open slot, Close slot, AI 난이도가 나온다.
- Spawn Menu의 소속 단추:
  - 누를 때마다 Player 3 → … → Player 9 → Auto Assign → Player 0 → Player 1 순으로 돈다.
  - 표시가 늦게 바뀔 수 있으니 한 번씩 누르고 본다.
- Spawn Menu에서 전차는 Armored Support → Medium Tank에 있다. Vehicle에는 없다.
- 왼쪽 유닛 카드는 나중에 놓은 유닛이 왼쪽에 온다.
- 직접 조종: 카드를 한 번 눌러 고른 뒤 E를 누른다. 카메라를 먼저 옮겨 두면 먼 곳을 겨눌 수 있다. 직접 조종 중 WASD는 전차를 움직인다.
- 게임 메뉴(Esc)의 CLOSE → YES는 판을 끝내고 결과 창을 거쳐 로비로 간다. 메인 메뉴의 EXIT → YES는 게임을 닫는다.
- 끝나고 `options.set`을 백업으로 되돌렸다(해시 일치).
