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

- 테스트: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
- 바닐라 전체 재생성: `pwsh -NoProfile -File tools/Invoke-Pipeline.ps1 -SourceRoot "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\resource" -WorkDir _work\vanilla -DataDir data`
- 문서 재생성과 검증: `pwsh -NoProfile -File tools/New-SchemaDoc.ps1 -DataDir data -AnnotationDir data\annotations -OutDir docs\04-units` 다음 `pwsh -NoProfile -File tools/Test-Coverage.ps1 -DataDir data -DocsDir docs\04-units`

## 규칙

- 게임 폴더의 파일을 수정하지 않는다.
- 키의 의미를 모르면 `미확인`으로 둔다.
