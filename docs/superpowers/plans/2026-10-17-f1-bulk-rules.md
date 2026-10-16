# F1 일괄 규칙 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 종류·경로·키·연산으로 고른 값을 대상 유닛 전부에 한 번에 고치는 규칙을 편집기(코어·명령줄·앱)에 넣는다. 규칙이 만든 수정은 지금의 수정과 같이 점검·빌드된다.

**Architecture:** 계산은 `BulkRules`(코어)가 한다. 대상 유닛을 펼쳐 키가 맞는 값 자리를 모으고, 자리마다 범위와 겹침을 정해 `Edit`을 만든다(`Edit.Rule = id`). 규칙은 `EditProject.Rules`에 남는다. 세션이 넣기·고치기·끄기·지우기·다시 적용·합치기를 맡고, 명령줄 `rule`과 앱 규칙 창은 세션을 부른다.

**Tech Stack:** C# .NET 8, 자체 단언(`T.Eq`) + xUnit 묶음 실행, WPF.

**Spec:** `docs/superpowers/specs/2026-10-17-f1-bulk-rules-design.md`

## Global Constraints

- 연산: `set`(새 값 검사 `ValueRules.CheckNewValue`), `mul`, `add`. `mul`·`add`는 수에만. 원래 값이 정수면 반올림해 정수, 소수면 `0.###`. 결과가 원래 값과 같으면 수정을 만들지 않는다.
- 키 맞추기: 규칙의 키가 `KeyPattern.Candidates(row.Path, row.RawPath, false)` 가운데 하나와 같으면 맞다. `Arg` -1 = 줄의 수인 값 모두, 기본 0.
- 범위 `auto`: 자리 파일의 사용자(`UsersOf`)가 모두 대상이면 '전부' 하나, 아니면 대상 유닛마다 '이 유닛만'. '이 유닛만'은 `UnitScope.Blocker`와 `FromLevel.HeirsNote`(물려받는 유닛 있음)가 막으면 건너뛴다.
- 겹침: 같은 자리 = 파일·키 경로·몇 번째·몇 번째 값(범위 무관). 사람의 '전부' 수정 → 그 자리 통째 건너뜀. 사람의 '이 유닛만' 수정(유닛 U) → U만 건너뜀. 앞 규칙(목록 순서, 켜진 것)도 같은 꼴.
- 규칙을 넣기·고치기·켜고 끄기·다시 적용하면 그 규칙과 목록 뒤 규칙들을 차례로 다시 적용한다.
- 사람이 규칙 수정을 고치거나 끄면 `Rule`을 지운다(규칙에서 떨어짐).
- 합치기: 가져오는 규칙을 목록 끝에, `id` 겹치면 새로. 규칙 수정은 복사하지 않고 다시 적용으로 만든다.
- `id`는 8자 16진수. 규칙이 없으면 프로젝트 파일에 `rules`를 쓰지 않는다(예전 파일과 같다).
- 게임 안 확인 없음. 저장소 브랜치 규칙과 커밋 꼬리말은 CLAUDE.md대로.

## Review Focus

1. 대상 밖 유닛도 쓰는 매크로 정의 안 값: `auto`는 '이 유닛만'이 막혀 건너뛰고 이유가 보인다 → Task 1 테스트.
2. 예전 프로젝트 파일(규칙 없음)을 열고 저장해도 `rules`가 생기지 않는다 → Task 2 테스트.
3. 규칙 수정을 사람이 바꾼 뒤 다시 적용해도 사람 값이 남는다 → Task 2 테스트.
4. 규칙 A·B가 같은 자리를 노리면 앞 규칙만 그 자리를 가진다, A를 끄면 B가 가져간다 → Task 2 테스트.
5. 수가 아닌 값에 `mul` → 건너뜀(이유 "수가 아니다") → Task 1 테스트.

---

### Task 1: 규칙 모델과 계산 (`BulkRules`)

**Files:** Create `editor/CtaToybox.Core/BulkRules.cs`; Modify `editor/CtaToybox.Core/Edit.cs`(`Edit.Rule`, `EditProject.Rules`); Test `editor/CtaToybox.Core.Tests/BulkRulesTests.cs`(+ `Suites.All`).

**Interfaces (Produces):**
- `public sealed class BulkRule { string Id, Name, Kind; List<string> Paths; string Layer, Key; int Arg = 0; string Op, Value, Scope = "auto"; bool Enabled = true; }`
- `Edit.Rule : string`(null = 사람 수정, JSON에서 null이면 안 씀). `EditProject.Rules : List<BulkRule>`(null이면 안 씀).
- `public static string BulkRules.Compute(string op, string oldText, string value, out string error)` — 새 값 글자. 안 되면 null과 이유.
- `public sealed class RulePreview { List<Edit> Edits; List<RuleChange> Changes; List<RuleSkip> Skipped; int Units; int OutsideUnits; }`, `RuleChange { string Unit, File, Old, New, Scope; }`, `RuleSkip { string Unit, Reason; }`
- `public static RulePreview BulkRules.Preview(BulkRule rule, EditProject project, IEnumerable<UnitEntry> units, UnitTracer tracer, Func<string, IReadOnlyList<string>> usersOf, Func<string, int> heirsOf)` — `project`의 다른 수정(사람·앞 규칙)과 겹침을 본다. `Edits`의 `Rule`은 `rule.Id`.

- [ ] **Step 1: 실패하는 테스트** `BulkRulesTests.Run`: 가짜 게임(전차 세 대: `tank_light/l1`, `l2`, `tank_heavy/h1`, 각자 `("mobility_tank" speed(45))`처럼 자기 파일에 속도, 공용 `properties/tank_unarmed.ext`에 `{mass 10}`(세 대가 같이 씀), 매크로 정의 안 값 하나):
  - `Compute("mul","45","1.1")`=`"50"`, `("mul","0.5","1.1")`=`"0.55"`, `("add","45","-5")`=`"40"`, `("set","45","60")`=`"60"`, `("mul","\"x\"","2")` 오류("수가 아니다"), `("mul","45","x")` 오류.
  - 규칙(kind tank, path `/tank_light/`, key `**/locomotion/maxspeed`, mul 1.1): 수정 2건(l1·l2 자기 파일, '전부'— 사용자 1이 대상), 새 값 50.
  - 규칙(kind tank, path `/tank_light/`, key `**/mass`, mul 2): 공용 파일을 h1도 써서 `auto`는 l1·l2 '이 유닛만' 2건, `OutsideUnits` 0(만들지 않았으므로), 같은 규칙 `shared`면 '전부' 1건과 `OutsideUnits` 1.
  - 매크로 정의 안 값 → 건너뜀 2건(이유에 "매크로 본문").
  - 사람의 '전부' 수정이 l1 속도 자리에 있으면 l1 건너뜀("직접 고친"), l2는 그대로.
  - 결과가 원래 값과 같으면(mul 1) 수정 0.
- [ ] **Step 2: Run** `dotnet run --project editor/CtaToybox.Core.Tests -c Release` → 컴파일 실패.
- [ ] **Step 3: Implement** 위 Interfaces. 대상 = `UnitEntry.EditorKind == rule.Kind`(차량 원래 종류 `vehicle`이면 `Kind`), 경로 글자 모두 포함, `Layer` 일치. 펼친 줄은 `tracer.Trace`.
- [ ] **Step 4: Run** → 통과. **Step 5: Commit** `feat(editor): F1 bulk rule model and preview`.

### Task 2: 프로젝트·세션

**Files:** Modify `EditorSession.Edits.cs`(규칙 메서드, 사람 수정 때 `Rule` 떼기), `Edit.cs`(`MergeFrom`이 규칙을 붙이고 규칙 수정은 복사하지 않음); Test `BulkRulesSessionTests.cs`.

**Interfaces (Produces):** `RulePreview PreviewRule(BulkRule)`, `BulkRule AddRule(BulkRule)`(id를 정하고 저장), `void EditRule(string id, BulkRule changed)`, `void SetRuleEnabled(string id, bool)`, `void RemoveRule(string id)`, `RulePreview ReapplyRule(string id)`, `IReadOnlyList<BulkRule> Rules`. 모두 저장 실패 시 되돌린다(`SaveOr`).

- [ ] **Step 1: 실패하는 테스트**: 넣기 → 프로젝트 파일에 `rules`와 수정의 `rule`; 점검 모두 적용; 빌드 적용 수. 규칙 없는 프로젝트를 열고 저장하면 `"rules"` 글자가 없다. 규칙 수정 하나를 `AddEdit`로 사람이 고치면 `Rule`이 null, `ReapplyRule` 뒤에도 사람 값. 두 규칙이 같은 자리 → 앞 규칙만; 앞 규칙 끄면 뒤 규칙이 가져간다; 앞 규칙 지우면 그 수정이 사라진다. `EditRule`로 배수를 바꾸면 수정 값이 바뀌고 id는 같다. 합치기: 다른 프로젝트의 규칙이 붙고 수정이 다시 만들어진다, id 겹침이면 새 id.
- [ ] **Step 2~4**: 실패 확인 → 구현 → 통과. **Step 5: Commit** `feat(editor): F1 rules in projects and the session`.

### Task 3: 명령줄 `rule`

**Files:** Modify `editor/CtaToybox.Cli/Program.cs`; Test `BulkRulesCliTests.cs`.

- 하위 명령 `preview`, `add`, `list`, `edit`, `reapply`, `remove`, `enable`, `disable`. 옵션 `--project --kind --path(여러 개는 쉼표) --layer --key --arg(수|all) --set|--mul|--add --scope --name --id`. 세션은 `EditorSession.Open(settings, 현재 폴더)`. 미리 보기 출력: 첫 줄 `바뀜 N건(유닛 M), 건너뜀 K, 대상 밖 O`, 이어서 앞 20건과 건너뛴 이유별 수.
- [ ] 테스트(가짜 게임으로 preview·add·list·edit·disable·remove의 종료 코드와 출력 글자) → 구현 → 통과 → Commit `feat(cli): F1 rule commands`.

### Task 4: 앱

**Files:** Create `editor/CtaToybox.App/RuleDialog.xaml(.cs)`; Modify `MainWindow.xaml`(요약·전체 목록 줄 메뉴 "이 키로 일괄 규칙...", 수정 목록 아래 "규칙 고치기·규칙 다시 적용·규칙 끄기/켜기·규칙 지우기" 단추 — 고른 수정의 규칙에 적용), `MainWindow.Edits.cs`(규칙 수정 묶음 이름 `규칙 · <이름>`), `MainWindow.Scenario.cs`(`rule <종류>|<경로>|<키>|<값 자리>|<연산>|<값>`로 창을 채워 열기, `rulepreview`, `ruleok`).
- 창: 칸들, 미리 보기 단추, 바뀜 목록(앞 500), 건너뜀 목록, 합계, 미확인 키 경고, "수정 목록에 넣기".
- [ ] `dotnet build editor/CtaToybox.App` 경고 0 → 한 번 실행 시나리오로 창·미리 보기·수정 목록 찍어 읽고 고칠 것을 모아 고친다 → Commit `feat(app): F1 rule window and rule groups`.

### Task 5: 실제 파일 검사와 문서

- `RealFilesCheck`: 실제 판에서 규칙(kind tank, key `**/locomotion/maxspeed`, mul 1.1)을 미리 보고 임시 폴더에 빌드. `rule-changes= rule-units= rule-skipped= rule-not-applied=` 찍고 적용 불가 0, 바뀐 유닛 > 100 단언.
- 문서: README(일괄 규칙), CLAUDE.md 명령에 `rule`, 백로그(P2 F1 끝), 스펙 구현 결과, 핸드오프.
- [ ] 전체 테스트 + `-- real` → Commit → develop 병합.
