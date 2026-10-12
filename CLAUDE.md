# CTAToybox

Call to Arms - Gates of Hell: Ostfront 모드 개발용 레퍼런스 저장소.

## 브랜치 전략

- `main`은 보호 브랜치다. 직접 커밋·푸시하지 않는다.
- `develop`에서 `feat/*`, `fix/*`, `chore/*`, `docs/*` 브랜치를 분기하고 `develop`으로 병합한다.
- `main`에는 릴리스 시점에 `develop` → `main`으로만 들어간다.
- 원격 저장소가 생기기 전까지 PR은 로컬 `git merge --no-ff`로 대신한다.

## 폴더

| 경로 | 역할 |
|---|---|
| `docs/` | 사람이 읽는 문서 |
| `data/` | 스크립트가 생성한 데이터. 손으로 고치지 않는다. 예외: `data/annotations/` |
| `tools/` | 분석 스크립트 (PowerShell 7, 외부 모듈 없음) |
| `mod/CTAToybox/` | 모드 본체. 게임의 `mods\CTAToybox`가 이 폴더를 가리키는 정션이다 |
| `_work/` | 게임에서 추출한 원본. 커밋하지 않는다 |

## 명령

모두 저장소 루트에서 실행한다.

- 테스트: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
- 바닐라 전체 재생성(약 6분): `pwsh -NoProfile -File tools/Invoke-Pipeline.ps1 -SourceRoot "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\resource" -WorkDir _work\vanilla -DataDir data`
- 표시 이름: `pwsh -NoProfile -File tools/Build-DisplayNames.ps1 -LocPak "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\localizations\default.pak" -DataDir data`
- 유닛 문서 재생성과 검증: `pwsh -NoProfile -File tools/New-SchemaDoc.ps1 -DataDir data -AnnotationDir data\annotations -OutDir docs\04-units` 다음 `pwsh -NoProfile -File tools/Test-Coverage.ps1 -DataDir data -DocsDir docs\04-units`. `keys-missing=0`이 나와야 한다.
- 수정 가능한 속성 문서 재생성: `pwsh -NoProfile -File tools/New-EditableDoc.ps1 -DataDir data -AnnotationDir data\annotations -OutDir docs\06-editable-attributes`. `common.psv`를 고치면 유닛 문서와 함께 다시 만든다.
- 모드 분석(pwsh 세션 안에서, 배열 인자 때문에 `&`로 호출): `& tools/Invoke-Pipeline.ps1 -SourceRoot "<모드 폴더>\resource" -WorkDir _work\mods\<id> -DataDir data\mods\<id> -FallbackWorkDir @('_work\vanilla') -NameDataDir @('data')` 다음 `& tools/Compare-ModToVanilla.ps1 -VanillaDataDir data -ModDataDir data\mods\<id>`
- 모드 문서 재생성: `& tools/Get-ModSummary.ps1 -VanillaDataDir data -ModsDataDir data\mods -ModIds <id들>` 다음 `& tools/New-ModDoc.ps1 -ModsDataDir data\mods -OutDir docs\05-workshop-mods`

## 손으로 고치는 파일

- `data/annotations/common.psv`: 키의 의미와 등급
- `data/annotations/<그룹>.intro.md`: 스키마 문서의 소개문
- `data/mods/mods.psv`, `data/mods/annotations.psv`, `data/mods/<id>/notes.md`, `data/mods/readme-*.md`: 모드 문서의 입력
- `docs/01-game-structure.md`, `docs/02-modding-research.md`, `docs/03-mod-folder-setup.md`, `docs/04-units/README.md`, `docs/04-units/coverage-notes.md`

`docs/04-units/schema-*.md`, `coverage.md`, `docs/05-workshop-mods/*.md`, `docs/06-editable-attributes/*.md`는 생성물이다.

## 구현 메모

- 파서, 리졸버, 집계의 본체는 C#(`tools/lib/*.cs`)이고 PowerShell 함수가 감싼다. `.mdl` 파일이 커서 PowerShell로는 너무 느리다.
- 스크립트에 배열 인자를 넘길 때는 `pwsh -File`이 아니라 `&`로 호출한다.
## 규칙

- 게임 폴더의 파일을 수정하지 않는다.
- 키의 의미를 모르면 `미확인`으로 둔다.
