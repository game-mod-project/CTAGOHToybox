# 모드 폴더 설정

기준: 2026-10-11, 게임 실행 파일 버전 1.0.6.7.

## 구성

| 항목 | 경로 |
|---|---|
| 모드 본체 | `E:\CTAToybox\mod\CTAToybox` |
| 게임이 보는 위치 | `E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\mods\CTAToybox` |

게임이 보는 위치는 모드 본체를 가리키는 디렉터리 정션이다. 모드 파일을 이 저장소에서 git으로 관리하면서, 게임은 자기 `mods` 폴더 안에 있는 것으로 인식한다. 파일을 복사하지 않으므로 두 곳의 내용이 어긋날 일이 없다.

현재 내용:

```
mod\CTAToybox\
├─ mod.info
├─ resource\         (비어 있음)
└─ localizations\    (비어 있음)
```

`resource`와 `localizations`에는 git이 빈 폴더를 유지하도록 `.gitkeep` 파일만 있다. 게임 동작을 바꾸는 파일은 없다.

## 로컬 모드 규칙

게임 폴더의 `mods\template\readme.txt`에 적힌 규칙이다.

- 로컬 모드는 `게임폴더\mods` 아래에 둔다. 창작마당 모드는 Steam이 따로 관리한다.
- 폴더명은 정규식 `[a-zA-Z_]+`를 따라야 한다.
- `mod.info`의 `name`은 폴더명과 같게 한다. 대소문자는 구분하지 않는다.
- 로컬 모드는 업데이트와 삭제를 직접 한다.

## mod.info

```
{mod
	{name "CTAToybox"}
	{tags "Mod"}
	{desc "CTAToybox development mod. Empty skeleton: changes nothing in the game."}
	{minGameVersion "any"}
	{maxGameVersion "any"}
}
```

| 필드 | 값 | 설명 |
|---|---|---|
| `name` | `CTAToybox` | 폴더명과 같다 |
| `tags` | `Mod` | 모드 태그 |
| `desc` | 영문 한 줄 | 모드 목록에 표시되는 설명 |
| `minGameVersion` | `any` | 지원하는 가장 오래된 게임 버전 |
| `maxGameVersion` | `any` | 지원하는 가장 최신 게임 버전 |

### 버전 필드에 `any`를 쓴 근거와 남은 의문

두 정보가 어긋난다.

- 템플릿 readme는 두 버전 필드가 비어 있으면 안 되고 `X.YYY` 형식이어야 하며, 아니면 모드가 호환되지 않는 것으로 표시된다고 적고 있다. 템플릿의 예시는 `1.500`과 `1.510`이다.
- 같은 게임 폴더의 `mods\SpawnMenu_MACE_Fusion_Ground_Patch\mod.info`는 두 필드에 `"any"`를 쓴다. 2026-07-09 작업 기록에 이 모드가 프로필의 활성 모드 목록에 있었고 게임에서 동작했다고 적혀 있다. 당시 게임 버전은 1.063이었다.

- 2026-10-11에 받은 창작마당 모드 네 개(MACE, MACE Wunderwaffe Ultimate Fusion ENG, Spawn Menu, Auto Medic-AA-Artillery & Weather Switch)의 `mod.info`가 모두 두 필드에 `"any"`를 쓴다. 가장 최근 것은 조사 당일에 갱신된 판이다.

실제로 동작한 사례가 있고 현재 배포되는 모드들이 같은 값을 쓰므로 `any`를 택했다.

2026-10-11에 게임 1.0.6.7의 모드 목록 화면에서 확인했다. `CTAToybox`가 목록에 나오고 활성화돼 있으며(버튼이 `DEACTIVATE`), 호환되지 않는다는 표시는 보이지 않는다. 다만 호환되지 않는 모드가 이 화면에 어떻게 표시되는지는 본 적이 없으므로, "표시가 없다"는 것은 화면에서 다른 모드와 다르게 보이는 점이 없다는 뜻이다. 나중에 이 모드가 "호환되지 않음"으로 표시되면 두 필드를 readme 형식의 숫자로 바꿔야 한다.

## 정션 다시 만들기

```powershell
$link = 'E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\mods\CTAToybox'
New-Item -ItemType Junction -Path $link -Target 'E:\CTAToybox\mod\CTAToybox'
```

정션만 없애려면 다음을 쓴다. 대상 폴더(`E:\CTAToybox\mod\CTAToybox`)는 지워지지 않는다.

```powershell
(Get-Item -LiteralPath $link).Delete()
```

저장소 폴더를 다른 곳으로 옮기면 정션이 끊어지므로 다시 만들어야 한다.

## 확인 상태

| 항목 | 상태 |
|---|---|
| 정션이 모드 본체를 가리킴 | 확인함 (`LinkType`이 `Junction`, `Target`이 `E:\CTAToybox\mod\CTAToybox`) |
| 정션을 통해 `mod.info`가 읽힘 | 확인함 |
| 게임의 모드 목록에 `CTAToybox`가 표시됨 | 확인함 (2026-10-11, 게임 1.0.6.7). 이름은 `CTATOYBOX`, 설명은 `mod.info`의 `desc` 그대로 나온다 |
| 모드 목록에서 활성화됨 | 확인함 (2026-10-11). 버튼이 `DEACTIVATE`로 표시된다 |
| 모드를 켠 상태에서 게임이 정상 동작함 | 모드 목록 화면까지만 확인함. 전투 진입은 확인하지 않음 |

## 문제가 생기면

템플릿 readme의 안내다. 모드를 켠 뒤 게임이 시작되지 않으면 다음 중 하나로 한 번 실행한다.

- "Safe mode (No mods)" 또는 실행 인자 `-no_mods`: 모드 초기화를 건너뛴다. 그 상태에서 옵션에서 모드를 끈다.
- "Safe mode" 또는 실행 인자 `-safe`: 모든 설정을 안전 프로필로 되돌린다.
