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
| `tools/` | 분석 스크립트 (PowerShell 7.4 이상, 외부 모듈 없음. 파서 DLL을 빌드하므로 .NET 8 SDK가 필요하다) |
| `editor/` | 스탯 편집기 (.NET 8). 파서 본체(`CtaToybox.Core`)가 여기 있고 `tools/`의 스크립트가 이 DLL을 불러 쓴다 |
| `mod/CTAToybox/` | 모드 본체. 게임의 `mods\CTAToybox`가 이 폴더를 가리키는 정션이다 |
| `_work/` | 게임에서 추출한 원본. 커밋하지 않는다 |

## 명령

모두 저장소 루트에서 실행한다.

- 테스트: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`
- 편집기 테스트: `dotnet test editor/CtaToybox.Core.XunitTests -c Release`(xUnit, 묶음마다 테스트 하나. 전체 테스트 `Run-Tests.ps1`에 포함돼 있다). 단언 수까지 보려면 `dotnet run --project editor/CtaToybox.Core.Tests -c Release`. 실제 게임 파일 검증은 `dotnet run`에 `-- real`을 붙인다. 새 테스트는 `editor/CtaToybox.Core.Tests`에 지금 꼴(`T.Eq`)로 쓰고 `Suites.All`에 등록하면 xUnit에서도 돈다
- 편집기 명령줄: `dotnet run --project editor/CtaToybox.Cli -c Release -- <명령>`. 명령은 `sources`, `units`, `trace`, `show`, `add`(`--kind file-copy`·`add-line` 포함), `remove`, `build`, `rule`(일괄 규칙: `preview`·`add`·`list`·`edit`·`reapply`·`enable`·`disable`·`remove`), `vision-preset`, `faction-preset`(팩션별 강화 프로젝트). 사용법은 `docs/07-stat-editor/README.md`. 유닛 색인 캐시는 `editor/cache/`(git 제외)에 생긴다.
- 편집기 화면: `dotnet run --project editor/CtaToybox.App -c Release`. 화면 확인은 `--unit <가상 경로> --tab summary|full --filter <글자> --screenshot <png>`로 PNG를 찍어 본다. 조작까지 확인할 때는 `--project <임시 파일> --scenario <시나리오 파일>`(명령은 `editor/CtaToybox.App/MainWindow.Scenario.cs` 머리 주석)로 값 넣기·범위·넣기·수정 켜고 끄기·삭제·일괄 규칙을 사람 대신 하고 그 사이 화면을 찍는다. 사용자의 프로젝트를 쓰지 않는다.
- 바닐라 전체 재생성(약 6분): `pwsh -NoProfile -File tools/Invoke-Pipeline.ps1 -SourceRoot "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\resource" -WorkDir _work\vanilla -DataDir data`
- 표시 이름: `pwsh -NoProfile -File tools/Build-DisplayNames.ps1 -LocPak "E:\SteamLibrary\steamapps\common\Call to Arms - Gates of Hell\localizations\default.pak" -DataDir data`
- 유닛 문서 재생성과 검증: `pwsh -NoProfile -File tools/New-SchemaDoc.ps1 -DataDir data -AnnotationDir data\annotations -OutDir docs\04-units` 다음 `pwsh -NoProfile -File tools/Test-Coverage.ps1 -DataDir data -DocsDir docs\04-units`. `keys-missing=0`이 나와야 한다.
- 수정 가능한 속성 문서 재생성: `pwsh -NoProfile -File tools/New-EditableDoc.ps1 -DataDir data -AnnotationDir data\annotations -OutDir docs\06-editable-attributes`. `common.psv`를 고치면 유닛 문서와 함께 다시 만든다.
- 모드 분석(pwsh 세션 안에서, 배열 인자 때문에 `&`로 호출): `& tools/Invoke-Pipeline.ps1 -SourceRoot "<모드 폴더>\resource" -WorkDir _work\mods\<id> -DataDir data\mods\<id> -FallbackWorkDir @('_work\vanilla') -NameDataDir @('data')` 다음 `& tools/Compare-ModToVanilla.ps1 -VanillaDataDir data -ModDataDir data\mods\<id>`
- 모드 문서 재생성: `& tools/Get-ModSummary.ps1 -VanillaDataDir data -ModsDataDir data\mods -ModIds <id들>` 다음 `& tools/New-ModDoc.ps1 -ModsDataDir data\mods -OutDir docs\05-workshop-mods`

## 손으로 고치는 파일

- `data/annotations/common.psv`: 키의 의미와 등급. `**/꼬리`는 include되는 파일과 매크로 정의 안의 키에도 적용된다
- `data/annotations/<그룹>.psv`: 한 그룹에만 적용하는 주석. 형식은 `common.psv`와 같다
- `data/annotations/values.psv`: 한 줄에 값이 여럿일 때 값마다 붙이는 이름(편집기의 값 고치기 창, 표, 수정 목록). 형식은 파일 머리 주석
- `data/annotations/research/findings-*.psv`: 웹 조사로 모은 키 설명과 출처. 주석을 쓸 때의 근거 자료다
- `data/annotations/<그룹>.intro.md`: 스키마 문서의 소개문
- `data/mods/mods.psv`, `data/mods/annotations.psv`, `data/mods/<id>/notes.md`, `data/mods/readme-*.md`: 모드 문서의 입력
- `editor/projects/*.json`: 편집기의 수정 목록. `mod/CTAToybox/resource`의 생성물과 `.ctatoybox-build.json`은 빌드가 만든다. 둘 다 git에 넣는다(2026-10-17 사용자 결정). 출력 모드를 다시 빌드하면 함께 커밋한다. `.gitattributes`가 이 파일들의 줄바꿈을 바이트 그대로 둔다
- `editor/summary/<종류>.psv`: 편집기 요약 탭에 올리는 키와 화면 이름(경로|이름). 경로 규칙은 `data/annotations`와 같다
- `editor/settings/files.psv`: 게임 설정 항목의 화면 이름(경로|묶음|화면 이름)
- `editor/presets/super-heavy.txt`: 팩션 강화의 초중전차 유닛 폴더 목록(장갑 ×5). 한 줄에 하나, `#` 뒤는 주석
- `data/annotations/settings.psv`: 게임 설정 파일 전용 키 주석. 형식은 `common.psv`와 같다
- `docs/01-game-structure.md`, `docs/02-modding-research.md`, `docs/03-mod-folder-setup.md`, `docs/04-units/README.md`, `docs/04-units/coverage-notes.md`, `docs/08-unit-files-check.md`, `docs/09-modding-sources.md`, `docs/10-backlog.md`(미룬 작업 정리. 일을 끝내면 그 줄을 지운다), `docs/11-feature-review.md`(추가 기능 후보), `docs/12-faction-files.md`(팩션별 강화에 쓰는 파일 정리)

`docs/04-units/schema-*.md`, `coverage.md`, `docs/05-workshop-mods/*.md`, `docs/06-editable-attributes/*.md`는 생성물이다.

## 구현 메모

- 파서, 리졸버, 집계의 본체는 C#(`editor/CtaToybox.Core/Sdl/*.cs`)이고 PowerShell 함수가 감싼다. `.mdl` 파일이 커서 PowerShell로는 너무 느리다. `tools/lib/SdlTokenizer.ps1`이 소스가 DLL보다 새로우면 다시 빌드한다.
- 스크립트에 배열 인자를 넘길 때는 `pwsh -File`이 아니라 `&`로 호출한다.
## 규칙

- 게임 폴더의 파일을 수정하지 않는다.
- 키의 의미를 모르면 `미확인`으로 둔다.
- 게임·모드 동작을 다루는 작업은 시작 전에 `docs/09-modding-sources.md`(참고·출처 사이트 정리)와 `docs/08-unit-files-check.md`(문서 주장 검증)를 읽고, 관련 사이트를 다시 열어 바뀐 것이 있는지 본다. 사이트 내용은 바닐라 파일로 확인한 뒤에만 사실로 쓴다. Call to Arms·Men of War 자료는 키가 다르다. fandom·ModDB는 WebFetch가 막히니 내장 브라우저로 연다
