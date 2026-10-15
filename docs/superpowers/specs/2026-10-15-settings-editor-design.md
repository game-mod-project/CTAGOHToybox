# 게임 설정 편집 설계 (하위 프로젝트 B)

작성: 2026-10-15. 기준: 게임 v1.067.0. 상위 문서: [2026-10-12-stat-editor-design.md](2026-10-12-stat-editor-design.md)(하위 프로젝트 A).

## 배경

원 요청의 3번 "컨퀘스트·스커미시 설정의 상세 변경. 1의 실행 파일에 포함"이다. A의 편집기(코어, 화면, 수정 목록, 빌드) 위에 얹는다.

사용자가 고른 범위(2026-10-15)는 넷 모두다.

1. 컨퀘스트 경제·진행
2. 컨퀘스트 전투 규칙·AI
3. 스커미시(배틀존 등)
4. 난이도 배수

## 합의한 전제

| 항목 | 결정 |
|---|---|
| 방식 | 설정 파일을 편집기의 새 종류 "게임 설정"으로 넣는다. 전용 입력 화면이나 묶음 프리셋은 만들지 않는다 |
| Lua | 컨퀘스트 AI 설정(`runtime_config.lua`)을 이번에 넣는다. 작은 Lua 표 읽기기와 패처를 새로 만든다 |
| 수정 범위 | 항상 "전부". 파일 하나가 곧 설정이라 "이 유닛만"이 없다 |
| 수정 기록 | A의 형식(파일, 경로, 몇 번째, 몇 번째 값, 원래 값, 새 값)을 그대로 쓴다 |
| 모드가 덮어쓴 파일 | A와 같이 이기는 계층의 파일을 고친다. 2026-10-15 이 PC에서는 `resources_standard.set`을 MACE Wunderwaffe(`mod_3732639687`)가 덮어쓴다(`GlobalMaxCP` 바닐라 550 → 2000) |
| 게임 폴더 | 읽기만 한다. 출력은 `mod/CTAToybox/resource`에만 쓴다 |
| 게임 실행 중 | 빌드하지 않는다(2026-10-15 `fix/build-game-running`) |

## 대상 파일

왼쪽 목록에는 맨 위 파일 하나가 한 항목이다. 다른 파일이 include하는 파일(`presets/*.inc`, `presets/global_parameters/**/*.inc`, `difficulty_generic.inc`)은 목록에 따로 올리지 않는다. 그 파일을 읽는 항목의 전체 탭에 원본 파일별로 묶여 나온다. "같이 씀"은 A와 같이 그 파일을 읽는 항목 수다.

| 묶음 | 파일 | 항목 수 |
|---|---|---|
| 컨퀘스트 자원 (난이도별) | `set/dynamic_campaign/resources_{low,standard,high,very_high}.set` | 4 |
| 컨퀘스트 길이 | `set/dynamic_campaign/duration_{short,normal,long,very_long,unlimited}.set` | 5 |
| 컨퀘스트 전투 규칙 | `set/multiplayer/games/campaign_capture_the_flag.set` | 1 |
| 컨퀘스트 AI | `script/multiplayer/modes/conquest/runtime_config.lua` | 1 |
| 스커미시 모드 | `set/multiplayer/games/{battle_zones,frontlines,cooperative,evacuation,ammunition}.set`. 프리셋 `.inc`는 이 안에 펼쳐진다 | 5 |
| 난이도 | `set/difficulty/{training,easy,normal,hard,heroic,multiplayer}.dl` | 6 |

넣지 않는 것: 맵 목록(`set/dynamic_campaign/map_points.set`), 개발사 설명 파일(`set/dynamic_campaign/values.set`. 주석의 근거로만 쓴다), 연구 트리(`unit_research_*`. 이미 A의 가격·로스터 탭에 있다), 다른 모드 스크립트(`script/multiplayer/modes/*.lua`. 설정 표가 없다).

모드가 위 경로에 파일을 더하면(예: 새 `resources_*.set`) 같은 규칙으로 목록에 들어간다. 이름이 `files.psv`에 없으면 파일 이름을 보인다.

## 구성

A의 구조에 더하는 것만 적는다.

```
editor/
  settings/files.psv                  게임 설정 항목의 경로|묶음|화면 이름 (손으로 고친다)
  summary/settings.psv                게임 설정 요약 탭의 키|이름 (손으로 고친다)
  CtaToybox.Core/
    UnitKind.cs                       종류 "settings" 판정 추가
    SettingStrings.cs                 "이름=값;…" 나누기·잇기, 단계표 검사 (새 파일)
    Lua/LuaTable.cs                   Lua 표 읽기기 (새 파일)
    Lua/LuaPatcher.cs                 Lua 값 바꾸기 (새 파일)
    ModBuilder.cs                     확장자로 패처 고르기
data/annotations/
  settings.psv                        게임 설정 키의 뜻·단위·등급 (손으로 고친다)
  values.psv                          게임 설정 줄의 값 이름 추가
```

- **종류**: `UnitKind.Of`가 위 표의 경로에 "settings"를 돌려준다. 편집기 종류도 "settings", 화면 이름은 "게임 설정"이다. 유닛 색인(`UnitIndex`)에 들어가므로 색인 형식 번호를 올린다.
- **화면 이름**: 게임 현지화에 이름이 없어서 `editor/settings/files.psv`(`경로|묶음|화면 이름`)에서 읽는다. 예: `set/dynamic_campaign/resources_standard.set|컨퀘스트 자원|컨퀘스트 자원 — 표준`.
- **주석 묶음**: `KeyAnnotations.GroupOf`가 게임 설정 경로에 "settings"를 돌려준다. `data/annotations/settings.psv`는 `common.psv`보다 먼저 찾는다(A의 묶음 규칙과 같다).
- **탭**: 요약·전체 탭을 쓴다. 가격·로스터 탭과 무기·탄약 탭은 게임 설정에서 숨긴다. 값 고치기 창의 "이 유닛만"은 꺼 두고 이유("게임 설정은 파일 전체에 적용된다")를 보인다.

## 값을 읽기 쉽게 보이기

### 뜻과 단위

- `data/annotations/settings.psv`에 키마다 한글 뜻·단위·등급을 적는다. 형식은 `common.psv`와 같다.
- 근거와 등급:
  - **확인**: 개발사 주석. `values.set`의 설명 블록(예: `GlobalMaxCP`, `StageCP`, `ResupplyCost`, `Bots`, `PaybackFactor`), 각 파일의 줄 끝 `;//` 주석, `runtime_config.lua`의 `--` 주석
  - **문서**·**추정**: `data/annotations/research/findings-goh.psv`의 모더·사용자 글
  - 근거가 없으면 등급 "미확인"으로 둔다
- 값이 여럿인 줄은 `values.psv`에 값마다 이름을 붙인다. 예: `{StartMP 1.00 min 99999 attackerMultiplier 1.00}`, `{BrokenComponent ...}`.
- 파일마다 맨 위 블록 이름이 다르다(`{Standard`, `{Low`, `{Normal`, `{difficulty`, `{game`). 그래서 경로는 `**/resources/mp/startval`처럼 꼬리로 적는다.
- SDL 파서는 주석을 버린다. SDL 쪽의 영어 원문 주석은 화면에 자동으로 띄우지 않고, 근거 인용으로 뜻 설명 안에 넣는다.

### 요약 키 (`editor/summary/settings.psv`)

| 묶음 | 요약에 올리는 것 |
|---|---|
| 컨퀘스트 자원 | 시작 MP·AP·RP·SP(`StartVal`), 승리·패배 보상(`WinGain`, `LoseGain`), 맵 종류별 보상(`MapRewards`), 전투 시작 CP(`Budget/Start`), 초당 CP(`PointsPerSecond`), CP 상한(`Limit`), 단계별 CP(`StageCP`), 전체 CP 상한(`GlobalMaxCP`), 손실·퇴역 환급(`PaybackFactor`, `SellFactor`), AI 병력(`Bots/CP/Base`, `StartMP`, `FinishMP`), 위험도별 보상 배수(`RiskFactor`) |
| 컨퀘스트 길이 | 판마다 깃발 수(`FlagCount`), 봇 연구 단계(`ResearchStages`) |
| 컨퀘스트 전투 규칙 | 종료 점수(`scoreFinal`), 준비 시간(`preparationTime`), 스폰 보호 시간(`impregnableTimeout`), 깃발 점령·해제 시간과 점수표(`parameters` 조각) |
| 컨퀘스트 AI | 첫 구매 시점(`StartSpawnTime`), 구매 간격(`SpawnCooldownTime`), 파도 수·크기·모양(`AttackerWaves`), 파도 사이 대기(`AttackerDynamicWaveOff`), 수비 시작(`DefenderOpening`) 등 약 30~40개 |
| 스커미시 | 시작·최종 MP와 시간별 CP(프리셋 매크로 `start() finish() cp()`), 유닛 수 제한(`unitLimits` 조각), 종료 점수, 준비 시간 |
| 난이도 | 적·아군·플레이어 체력·명중 배수, 출혈 사망 시간(`death_delay_min/max`), 엄폐 효율(`cover_efficiency`), 점사 탄수(`burst`) |

### 문자열 속 설정 `"이름=값;이름=값"`

- 바닐라에 10곳(파일 9개)이 있다(2026-10-15): 게임 모드 5개(`campaign_capture_the_flag`, `battle_zones`, `frontlines`, `evacuation`, `ammunition`)의 `{parameters "..."}`, 유닛 수 제한 `{unitLimits "officer=2;flamer=3;…"}` 3곳(`unitlimits.inc`, 프런트라인 공격·수비), 프런트라인 타이머 `timers_frontlines.inc` 2곳(`"infantry=0.66;engineer=1;…"`).
- 표와 값 고치기 창에서 `이름=값` 조각마다 한 칸씩 보인다. 조각의 이름과 뜻은 `values.psv`에 적는다. 자리 표기는 `parameters` 조각용 새 표기 `@이름`(예: `**/game/parameters|@scoreStart|시작 점수|C`)을 쓴다.
- 파일에는 지금처럼 문자열 하나를 통째로 바꿔 쓴다. 수정 기록의 원래 값·새 값은 문자열 전체다.
- 창은 조각을 다시 이어 붙인다. 조각 이름·개수·순서·구분자(`;`, `=`)는 그대로 두고 값만 바꿀 수 있다. 어긋나면 넣지 않는다.
- 한 문자열에 같은 수정이 이미 있으면 창은 그 수정의 새 값에서 시작한다. 그래서 조각 둘을 차례로 고쳐도 앞의 것이 남는다.
- 조각 안에 또 목록이 있으면(`points_table_player=0/0.000,0.333/0.277,…`) 한 칸으로 두고 형식을 설명으로 보인다. 근거는 같은 파일의 개발사 주석(`0.277 = 60min` 등의 표)이다.

### 판 수 단계표 `"1:200 4:250 6:300"`

- `WinGain`, `LoseGain`, `FlagCount`, `ResearchStages`, `Bots/CP/Base`, `RoundMultiplier`, `Budget/Start`, `MapSelection`, `MapBorder` 48곳이다(2026-10-15, `"수:수 …"` 꼴만 센 것).
- 값 하나로 둔다. 이름은 개발사 주석의 형식을 쓴다. 예: "판 수 : MP 보상", "켜진 깃발 수 : 맵 전체 깃발 수(한 칸이 한 판)", "지난 초 : CP".
- 값 고치기 창 아래에 읽기용 풀이를 보인다. 예: `1 → 200`, `4 → 250`, `6 → 300`. "그 판부터 적용"인지는 주석에 없으므로 풀이에 쓰지 않는다.
- 넣을 때 원래 값과 같은 꼴(`수:수`가 공백으로 이어짐, `FlagCount`처럼 칸마다 두 수)인지 검사한다.

## 컨퀘스트 AI 설정 (Lua)

대상은 `script/multiplayer/modes/conquest/runtime_config.lua` 하나다(669줄). `conquest.lua`가 `require`로 읽는다.

- 맨 위 표 18개: `CombatPressureWeights`, `AssaultWeights`, `PlayerDefenseWeights`, `StartSpawnTime`, `SpawnCooldownTime`, `AttackerWaves`, `AttackerWavePatterns`, `AttackerDynamicWaveOff`, `CompletedPurchaseIdleTime`, `DefenderOpening`, `DefenderCounterattack`, `PurchasePressure`, `PurchaseForces`, `PurchaseProfiles`, `PurchaseScoring`, `UnitSpawnWaitTime`, `OrderRotationPeriod`, `AttackerFailure`
- 값 약 400개, `--` 주석 82줄. 함수는 없다.

### 읽기 (`LuaTable`)

Lua 해석기가 아니라, 이 파일에 실제로 나오는 형태만 읽는다.

- 문장: `이름(.이름)* = 식`. 맨 위의 `ConquestConfig = ConquestConfig or {}`는 값 줄로 만들지 않는다.
- 표: `{ 필드, 필드, … }`. 필드는 `이름 = 식`, `["문자열"] = 식`, 순서 값 `식`이다. 구분자는 `,`와 `;`.
- 값: 수, 수식(수와 `* / + - ( )`만, 예: `2 * 60000`, `-15`), 문자열(`"…"`, `'…'`), `true`, `false`, `nil`, 안쪽 표.
- 주석: 줄 주석 `--`와 블록 주석 `--[[ … ]]`. 필드 줄 끝 주석은 그 줄에, 표 앞의 주석 줄은 그 표에 붙인다.
- 이 밖의 문법(함수, 다른 연산자, 변수 참조 등)을 만나면 파일 전체를 "읽지 못함"으로 표시하고 고치지 못하게 한다. 추측해서 읽지 않는다.

줄 경로는 SDL과 같은 꼴로 만든다. 그래서 요약 키·주석·값 이름·수정 기록의 규칙을 그대로 쓴다.

| 원문 | 경로 |
|---|---|
| `ConquestConfig.StartSpawnTime = { AttackMin = 1 * 1000 }` | `/ConquestConfig.StartSpawnTime/AttackMin` (값 0 = `1 * 1000`) |
| `DeltaClamp = { Default = { min = -2, max = 8 } }` | `/ConquestConfig.PurchasePressure/DeltaClamp/Default/min` |
| `["1"] = 0.5` | `/…/"*"` (A의 따옴표 이름 규칙. 이름 경로 `/…/"1"`로 주석을 나눌 수 있다) |
| 순서 목록 `{ band = "Stage1", max = 3 }, …` | `/…/#1/band`, `/…/#1/max`, `/…/#2/…` |

경로 비교는 A와 같이 대소문자를 가리지 않는다. 보이는 이름은 원문 그대로다.

### 보이기

- 수식은 원문과 계산한 값을 함께 보인다. 예: `2 * 60000` → `120000`.
- 단위는 주석(`settings.psv`, `values.psv`)에서 온다. 예: "ms, 60000 = 1분".
- 읽기기가 붙인 개발사 영어 주석을 줄마다 그대로 보인다(설명 칸 아래 "원문 주석").
- 한글 뜻은 요약 키부터 `settings.psv`에 적는다. 나머지 키는 영어 주석과 "한글 설명 없음"으로 보이고, 이후 채운다.

### 고치기 (`LuaPatcher`)

- 빌드가 확장자로 패처를 고른다. `.lua`는 `LuaPatcher`, 나머지는 A의 `TokenPatcher`다.
- 값의 원문 범위만 새 글자로 바꾼다. 원래 값이 지금 원본과 다르면(공백 차이는 무시) "원본이 바뀜"이다.
- 새 값은 원래 값과 같은 종류여야 한다.
  - 수·수식: 수와 `* / + - ( )`만 쓰고, 계산이 되어야 한다
  - 문자열: 따옴표 안의 글자. 따옴표 종류는 원문을 따른다
  - 참·거짓: `true`, `false`
  - `nil`은 이번에 고칠 수 없다
- 고친 뒤 파일 전체를 다시 읽어 깨지지 않았는지 본다. 깨지면 그 수정은 "적용 불가"다.
- 인코딩은 A와 같이 `TextCodec`으로 읽고 다시 써서 바이트가 같은지 먼저 본다.

## 빌드

A의 `ModBuilder`를 그대로 쓴다. 출력은 `mod/CTAToybox/resource/set/...`와 `resource/script/...`의 낱개 파일이다. 바뀌는 것은 확장자로 패처를 고르는 것뿐이다. 목록 파일(`.ctatoybox-build.json`), 이전 생성물 지우기, 손으로 넣은 파일 보호는 A와 같다.

## 게임 안 확인

한 빌드에 몰아서 부탁한다. 값은 다른 데서 나오기 어려운 수를 쓴다.

| # | 고칠 것 | 볼 곳 | 기대 |
|---|---|---|---|
| 1 | `resources_standard.set`(이기는 계층)의 시작 MP 900 → 9137, 시작 RP 6 → 37 | 새 컨퀘스트를 자원 "표준"으로 시작한 첫 화면 | MP 9137, RP 37 |
| 2 | `battle_zones.set`의 `parameters` 조각 `scoreStart=250` → 777 | 스커미시(배틀존) 시작 때 점수 | 777 |
| 3 | 스커미시 프리셋 `unitLimits`의 `officer=2` → 7 | 스커미시 구매 화면의 장교 수 제한 | 7 |
| 4 | Lua `AttackerDynamicWaveOff.WaveOffSecondsByAttackerState`의 135/165/210 → 1135/1165/1210 | 컨퀘스트에서 방어할 때 AI 파도가 끝난 뒤 `game.log`의 `[LUA] Print: attacker wave-off pressure … targetWaveOffSeconds=` | 1000이 넘는 값. 모드의 Lua 파일이 바닐라를 덮어쓴다는 확인 |
| 5 | 난이도 배수 | 숫자로 볼 곳을 플랜에서 찾는다 | 없으면 "게임이 정상으로 뜬다"만 본다 |

- 2·3은 플랜 단계에서 게임 화면의 위치와 이름을 먼저 집어서 부탁한다. 3의 "장교" 제한이 어느 병종에 걸리는지도 데이터에서 먼저 센다.
- 4의 출력은 `attacker_wave_off.lua:209`의 조건 없는 `print`다. 이 로그가 `game.log`에 남는 것은 2026-10-14 로그의 `[LUA] Print:` 줄로 확인했다.
- 확인이 끝나면 수정을 모두 끄고, 게임을 끈 상태에서 다시 빌드한다.

## 테스트

- **단위 테스트**
  - 종류 판정과 `files.psv` 이름
  - `SettingStrings`: 나눴다 이으면 바이트가 같다. 조각 이름·개수·순서를 바꾸면 거부한다. 단계표 꼴 검사
  - `LuaTable`: 경로, 순서 목록, 따옴표 키, 주석 붙이기, 수식 계산, 모르는 문법 거부
  - `LuaPatcher`: 범위 교체, 종류가 다른 새 값 거부, 원본 바뀜, 고친 뒤 다시 읽기
  - `.lua` 수정이 든 빌드와 이전 생성물 지우기
  - CLI `show`·`trace`·`add`·`build`를 게임 설정 파일과 Lua 파일로
- **실제 파일 검증(`-- real`)**
  - 게임 설정 파일이 모두 읽힌다
  - `runtime_config.lua`의 모르는 문법이 0이다
  - 문자열 속 설정이 모두 나눠졌다가 바이트 그대로 이어진다
  - `editor/summary/settings.psv`의 키가 모두 실제 줄에 맞는다. 핸드오프의 Minor "요약 키의 `-- real` 검사 없음"을 게임 설정부터 푼다
- **화면**: `--scenario` 한 번 실행으로 자원 값 고치기, `parameters` 조각 고치기, Lua 값 고치기, 끄고 켜기, 삭제를 하며 찍는다. 찍은 화면을 사용자 입장에서 읽는다(뜻과 단위가 보이는가).

## 구현 순서

1. 게임 설정 종류, `files.psv`, 탭 숨기기와 "이 유닛만" 끄기, 설정 주석·요약 키(자원, 길이, 전투 규칙, 난이도)
2. 문자열 속 설정 나누기와 단계표(표시, 값 고치기 창, 검사)
3. Lua 읽기기·패처, 빌드의 패처 고르기, AI 주석·요약 키
4. `-- real` 검사, 화면 확인, 게임 안 확인 빌드

단계마다 `develop`에서 브랜치를 나눠 병합한다.

## 범위

들어가는 것: 위 대상 파일의 모든 값 보기와 고치기, 요약과 주석, 문자열 속 설정, 단계표, Lua AI 설정.

들어가지 않는 것: 맵 목록 편집, 연구 트리 구조 바꾸기(이미 A의 로스터 탭에서 값만 고친다), `nil` 값 고치기, 새 키나 새 블록 더하기, 컨퀘스트 세이브(`.sav`) 고치기, SDL 영어 원문 주석 자동 표시, 다른 모드 스크립트.

## 알려진 한계와 위험

- **멀티플레이**: 모든 사람이 같은 모드를 써야 한다. 설정 파일을 바꾼 모드는 다른 사람과 어긋날 수 있다(docs/09 S11의 desync 보고). 문서에 적는다.
- **진행 중인 캠페인**: 자원 시작값(`StartVal`) 같은 컨퀘스트 설정이 진행 중인 캠페인에 반영되는지는 미확인이다. 게임 안 확인 1은 새 캠페인으로 본다.
- **덮어쓴 모드**: Wunderwaffe를 끄면 그 판을 기준으로 한 수정은 "원본이 바뀜"으로 뜬다. A와 같은 동작이다.
- **Lua 덮어쓰기**: 모드의 `resource/script/...` 낱개 파일이 바닐라 Lua를 덮어쓰는지는 게임 안 확인 4 전까지 미확인이다. 근거는 Conquest Tweaks 모드 가이드가 `conquest.lua`를 고친다는 글뿐이다(`findings-goh.psv`). → 2026-10-15 게임 안 확인 4로 확인됐다(앞선 모드의 낱개 Lua도 덮어쓴다. `docs/07-stat-editor/ingame-check.md`).
- **Lua 읽기기의 범위**: 패치로 파일에 새 문법이 들어오면 그 파일은 "읽지 못함"이 된다. 고칠 수 없다는 표시가 나올 뿐 잘못 고치지는 않는다.
- **단계표의 뜻**: "판 수 : 값"이 그 판부터 적용되는지, 구간 사이를 잇는지는 주석에 없다(미확인).
