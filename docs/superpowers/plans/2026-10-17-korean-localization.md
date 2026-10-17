# 한글화 구현 계획

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 게임과 켜 둔 모드 4개의 현지화 `.pot`을 모두 한국어로 바꾸고, 한글이 나오게 글꼴을 맞추고, 결과를 출력 모드 `CTAToybox`에 넣는다. 편집기는 이름을 `원문 (한글)`로 보인다.

**Architecture:** `CtaToybox.Core/L10n/`에 `.po` 읽기·쓰기, 현지화 층 읽기, 갱신(extract), 검사(check), 빌드(build), 글꼴 대응을 둔다. 편집기 명령줄의 `l10n` 명령이 이것을 부른다. 번역은 `l10n/ko/<원래 경로>.po`(msgid 영어, msgstr 한국어)에 보관하고, 빌드가 `mod/CTAToybox/localizations/default/<원래 경로>.pot`(msgid 한국어)과 `mod/CTAToybox/resource/interface/font/*.font`를 만든다. 번역 자체는 AI가 파일 묶음 단위로 한다(Task 10~14).

**Tech Stack:** .NET 8, C# 12(`Nullable` 끔, `ImplicitUsings` 끔), 외부 패키지 없음. `System.IO.Compression.ZipFile`로 pak을 읽는다. 테스트는 `editor/CtaToybox.Core.Tests`의 `T.Eq` 꼴.

**Spec:** `docs/superpowers/specs/2026-10-17-korean-localization-design.md`

## Global Constraints

- 게임 폴더·창작마당 폴더의 파일을 고치지 않는다. 읽기만 한다.
- 게임을 띄우는 확인은 하지도 제안하지도 않는다. 모르는 것은 `미확인`으로 둔다.
- 브랜치: `develop`에서 `feat/korean-localization`을 따고 `git merge --no-ff`로 `develop`에 넣는다. 커밋 끝줄 `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.
- 출력 모드 `E:\CTAToybox\mod\CTAToybox`에는 지금 강화-독일 빌드가 들어 있다. 편집기 `build`로 덮어쓰지 않는다. `l10n build`는 `localizations/`와 `resource/interface/font/`만 쓴다.
- 번역 규칙(사용자 결정): 전차·장비 이름과 계급은 원문 그대로. 부대명은 음독. 독일어·러시아어 병과명 음독, 영어 분대 종류 번역, 진영·군대는 독일어 등 음독·영어 번역, 무기 이름·탄종 약어 원문, 캠페인 인물·지명 음독.
- 문체: 대사 구어체, 임무 목표 명령형(`~를 점령하라`), 메뉴·버튼 짧은 명사형, 유닛 설명 `~다` 평서문.
- 그대로 두는 것: `<…>` 태그 전체, `</c>`, `\n`, `\"`, `%s`·`%i`·`%d`·`%02d` 등 `%` 자리표시, `{…}`, URL.
- 출력 `.pot`: UTF-8(BOM 없음), 줄바꿈 LF. 바닐라와 같다.
- 편집기 이름 표시: `원문 (한글)`. 번역이 없으면 원문만.
- 새 테스트는 `editor/CtaToybox.Core.Tests/<이름>Tests.cs`에 `static class <이름>Tests { public static void Run() }`로 쓰고 `Program.cs`의 `Suites.All`에 `("<이름>", <이름>Tests.Run)`을 더한다.
- 테스트 명령: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`(결과 줄 `passed=N failed=0`). 마지막에 `pwsh -NoProfile -File tools/tests/Run-Tests.ps1`.

## Review Focus

1. 한 파일에 `msgctxt` 없는 항목이나 같은 `msgctxt`가 두 번 나오는 경우 → 항목이 섞이거나 사라지지 않고 순서대로 따로 번역된다(Task 1 테스트 `po_duplicate_and_no_context`).
2. 여러 줄로 이어진 `msgid`(`msgid ""` 다음 `"…"` 줄들, 바닐라에 438곳)와 `\"` → 이어 붙인 원문이 같고, 출력도 같은 뜻으로 다시 읽힌다(Task 1 `po_multiline_roundtrip`).
3. 원문이 게임 업데이트로 바뀐 항목 → 예전 번역을 버리지 않고 `#, fuzzy`와 `#| msgid`를 남기며, 빌드에는 영어 원문이 나간다(Task 3 `extract_changed_marks_fuzzy`, Task 6 `build_fuzzy_falls_back`).
4. 편집기 빌드의 목록 파일(`.ctatoybox-build.json`)에 있는 경로나 사용자가 둔 파일을 `l10n build`가 지우거나 덮어쓰지 않는다(Task 6 `build_never_touches_editor_files`).
5. 번역문에 바꾼 글꼴에 없는 글자(예: 이모지, 옛한글)가 들어감 → `check`가 그 글자와 위치를 오류로 낸다(Task 4 `check_missing_glyph`).

---

## File Structure

| 파일 | 맡는 일 |
|---|---|
| `editor/CtaToybox.Core/L10n/PoFile.cs` | `.po`/`.pot` 한 파일의 모형, 읽기, 쓰기 |
| `editor/CtaToybox.Core/L10n/L10nSources.cs` | 층(바닐라, 켜 둔 모드)에서 현지화 파일을 모아 경로마다 가장 뒤 층의 원문을 정한다 |
| `editor/CtaToybox.Core/L10n/L10nCatalog.cs` | `l10n/ko/*.po` 갱신(extract)과 요약 |
| `editor/CtaToybox.Core/L10n/L10nCheck.cs` | 검사: 태그·자리표시·줄바꿈·용어·글자 |
| `editor/CtaToybox.Core/L10n/BmFont.cs` | BMFont 바이너리 v3의 글자 목록 읽기 |
| `editor/CtaToybox.Core/L10n/FontMap.cs` | 한글 없는 글꼴 → `main_<크기>` 대응과 `.font` 글 만들기 |
| `editor/CtaToybox.Core/L10n/L10nBuilder.cs` | 출력 모드에 `.pot`과 `.font` 쓰기, 자기 목록 `.ctatoybox-l10n.json` |
| `editor/CtaToybox.Core/L10n/Glossary.cs` | `l10n/ko/glossary.psv` 읽기 |
| `editor/CtaToybox.Core/DisplayNames.cs` | 한글 이름을 받아 `원문 (한글)`으로 낸다 |
| `editor/CtaToybox.Core/EditorSession.cs` | 열 때 `l10n/ko`의 한글 이름을 읽는다 |
| `editor/CtaToybox.Cli/Program.cs` | `l10n extract|check|build` 명령, `units`도 한글 이름 |
| `editor/CtaToybox.Core.Tests/L10n*Tests.cs` | 테스트 |
| `l10n/ko/` | 번역 보관(손으로 고칠 수 있음) |
| `CLAUDE.md`, `docs/07-stat-editor/README.md`, `docs/10-backlog.md` | 문서 |

---

### Task 1: `.po` 모형과 읽기·쓰기

**Files:**
- Create: `editor/CtaToybox.Core/L10n/PoFile.cs`
- Create: `editor/CtaToybox.Core.Tests/L10nPoTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs` (`Suites.All`에 `("L10nPo", L10nPoTests.Run)`)

**Interfaces:**
- Produces:
  - `public sealed class PoEntry { public string Context; public string Id; public string Str; public bool Fuzzy; public string PreviousId; public List<string> Comments; public string Key { get; } }` — 문자열은 파일 속 **이스케이프된 그대로**(`\n`, `\"`를 풀지 않는다). `Context`가 없으면 null. `Key`는 `Context ?? ("\u0004" + Id)`. `Comments`는 `#, fuzzy`와 `#|` 말고 원래 있던 `#` 줄(예: `#, boost-format`, `#removed`, `#msgid "…"`)을 그대로 가진다.
  - `public sealed class PoFile { public string Header; public List<PoEntry> Entries; public static PoFile Parse(string text); public string Write(); }` — `Header`는 첫 `msgid ""` 항목의 msgstr 이어진 원문(여러 줄 그대로). 머리 앞 `#, fuzzy`는 `HeaderFuzzy` 불로 둔다(`public bool HeaderFuzzy`). 머리 앞의 그 밖의 `#` 줄은 `public List<string> HeaderComments`(쓸 때 맨 앞에 그대로).
  - `public static string PoFile.HeaderWithLanguage(string header, string lang)` — 머리의 `"Language: …\n"` 줄을 `lang`으로 바꾼다. 없으면 더한다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

```csharp
static class L10nPoTests
{
    public static void Run()
    {
        string src = "#, fuzzy\nmsgid \"\"\nmsgstr \"\"\n\"Language: en\\n\"\n\"Content-Type: text/plain; charset=UTF-8\\n\"\n\n" +
                     "msgctxt \"a\"\nmsgid \"Hello \\\"x\\\"\\nworld\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"b\"\nmsgid \"\"\n\"Long \"\n\"text\"\nmsgstr \"\"\n\n" +
                     "#, boost-format\nmsgid \"no context\"\nmsgstr \"\"\n\n" +
                     "msgctxt \"a\"\nmsgid \"again\"\nmsgstr \"\"\n";
        var po = PoFile.Parse(src);
        T.Eq(4, po.Entries.Count, "po_duplicate_and_no_context: all entries kept in order");
        T.Eq("Hello \\\"x\\\"\\nworld", po.Entries[0].Id, "escapes stay as written");
        T.Eq("Long text", po.Entries[1].Id, "po_multiline_roundtrip: continuation lines join");
        T.Eq(null, po.Entries[2].Context, "entry without msgctxt");
        T.Eq("\u0004no context", po.Entries[2].Key, "key falls back to msgid");
        T.Eq(new[] { "#, boost-format" }, po.Entries[2].Comments.ToArray(), "other comments kept");
        T.Eq("a", po.Entries[3].Context, "duplicate context kept as its own entry");
        T.True(po.HeaderFuzzy, "header fuzzy flag");
        var again = PoFile.Parse(po.Write());
        T.Eq(po.Entries.Select(e => e.Key + "=" + e.Id).ToArray(), again.Entries.Select(e => e.Key + "=" + e.Id).ToArray(), "po_multiline_roundtrip: write then parse is the same");
        T.True(PoFile.HeaderWithLanguage(po.Header, "ko").Contains("Language: ko\\n"), "language line replaced");

        string fz = "msgid \"\"\nmsgstr \"\"\n\n#, fuzzy\n#| msgid \"Old\"\nmsgctxt \"k\"\nmsgid \"New\"\nmsgstr \"번역\"\n";
        var f = PoFile.Parse(fz);
        T.True(f.Entries[0].Fuzzy, "fuzzy flag read");
        T.Eq("Old", f.Entries[0].PreviousId, "previous msgid read");
        T.Eq("번역", f.Entries[0].Str, "msgstr read");
        T.Eq(fz, f.Write(), "fuzzy entry writes back byte for byte");
    }
}
```

- [ ] **Step 2: 실행해 실패를 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: 빌드 오류 `PoFile`을 찾을 수 없음.

- [ ] **Step 3: `PoFile`·`PoEntry`를 `CtaToybox.Core.L10n` 이름공간에 구현한다**

Write 규칙: 항목 사이 빈 줄 하나, 파일 끝 줄바꿈 하나, LF. 항목 순서는 `Comments` → `#, fuzzy` → `#| msgid "…"` → `msgctxt` → `msgid` → `msgstr`. 값은 한 줄 `msgid "…"`로 쓴다(여러 줄로 나누지 않는다). 머리는 `msgid ""` / `msgstr ""` 다음 원래 줄들을 그대로 쓴다.

- [ ] **Step 4: 실행해 통과를 본다**

Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release`
Expected: `failed=0`

- [ ] **Step 5: 실제 파일 왕복을 확인한다**

`RealFilesCheck.cs`에 바닐라 `localizations/default.pak`의 모든 `.pot`을 `Parse`→`Write`→`Parse`해 항목 수와 `Key`·`Id` 목록이 같은지 보는 검사를 더한다(설정의 `GameDir` 사용).
Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real`
Expected: `failed=0`

- [ ] **Step 6: 커밋**

```bash
git add editor/CtaToybox.Core/L10n/PoFile.cs editor/CtaToybox.Core.Tests/L10nPoTests.cs editor/CtaToybox.Core.Tests/Program.cs editor/CtaToybox.Core.Tests/RealFilesCheck.cs
git commit -m "feat(l10n): po file model, parse and write"
```

---

### Task 2: 현지화 층 읽기

**Files:**
- Create: `editor/CtaToybox.Core/L10n/L10nSources.cs`
- Create: `editor/CtaToybox.Core.Tests/L10nSourcesTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `LayerPlan`, `SourceLayer`(`Name`, `ResourceDir`) from `GameProfile.PlanLayers`; `EditorSettings.GameDir`.
- Produces:
  - `public sealed class L10nSource { public string Path; public string Layer; public string Text; public List<string> Shadowed; }` — `Path`는 `interface/text/desc/desc_ammo_eng.pot` 꼴(앞의 `default/` 뗌, `/` 구분, 소문자로 바꾸지 않음. 비교는 대소문자 무시). `Layer`는 `vanilla` 또는 층 이름(`mod_2905667604`). `Shadowed`는 같은 경로를 가졌지만 진 층들.
  - `public static List<L10nSource> L10nSources.Collect(EditorSettings settings, LayerPlan plan, List<string> warnings)` — 순서: 바닐라 `<GameDir>/localizations/default.pak` → `plan.Layers`의 모드마다 `<모드>/localizations/default.pak`, 이어서 `<모드>/localizations/default/**/*.pot`(같은 모드 안에서는 낱개 파일이 pak을 이긴다). 뒤가 이긴다. 결과는 `Path` 순.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

`T.TempDir`·`T.Zip`·`T.Put`으로 게임 폴더(바닐라 pak에 `default/interface/text/tips.pot`, `default/interface/text/desc/a.pot`), 모드 A(`localizations/default.pak`에 `default/interface/text/tips.pot`), 모드 B(낱개 `localizations/default/interface/text/desc/b.pot`), 출력 모드(낱개 `localizations/default/interface/text/tips.pot`)를 만들고 `options.set`의 `{mods}`에 A, B, 출력 모드 순으로 넣는다.

```csharp
T.Eq(new[] { "interface/text/desc/a.pot", "interface/text/desc/b.pot", "interface/text/tips.pot" }, got.Select(s => s.Path).ToArray(), "all paths, sorted");
T.Eq("mod_A", got.Single(s => s.Path.EndsWith("tips.pot")).Layer, "later mod wins; output mod is not a source");
T.Eq(new[] { "vanilla" }, got.Single(s => s.Path.EndsWith("tips.pot")).Shadowed.ToArray(), "shadowed layers listed");
T.Eq("mod_B", got.Single(s => s.Path.EndsWith("b.pot")).Layer, "loose files read");
```
(모드 이름은 `GameProfile`이 짓는 꼴 그대로 쓴다. 창작마당 픽스처가 어려우면 로컬 모드 폴더 이름으로 한다.)

- [ ] **Step 2: 실패를 본다** — Run: `dotnet run --project editor/CtaToybox.Core.Tests -c Release` / Expected: 빌드 오류

- [ ] **Step 3: `L10nSources.Collect`를 구현한다** — 모드 폴더는 `Path.GetDirectoryName(layer.ResourceDir)`(`DisplayNames.FromGame`과 같은 법). 깨진 pak은 `warnings`에 더하고 건너뛴다.

- [ ] **Step 4: 통과를 본다** — Expected: `failed=0`

- [ ] **Step 5: 커밋** — `git commit -m "feat(l10n): collect localization sources by layer"`

---

### Task 3: 번역 보관 갱신 (extract)

**Files:**
- Create: `editor/CtaToybox.Core/L10n/L10nCatalog.cs`
- Create: `editor/CtaToybox.Core.Tests/L10nCatalogTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `PoFile`, `PoEntry`(Task 1), `L10nSource`(Task 2).
- Produces:
  - `public sealed class ExtractReport { public int Files, Entries, Added, Changed, Removed, Untranslated, Fuzzy; public List<string> RemovedFiles; }`
  - `public static ExtractReport L10nCatalog.Extract(List<L10nSource> sources, string catalogDir, string gameVersion)` — `catalogDir`는 `l10n/ko`. 경로마다 `<catalogDir>/<Path에서 .pot을 .po로>`를 만들거나 갱신한다.
  - `public static PoFile L10nCatalog.Merge(PoFile source, PoFile existing)` — 규칙: 원문 항목 순서대로 낸다. 같은 `Key`가 여러 번이면 몇 번째인지로 짝짓는다. 있던 항목의 `Id`가 원문과 같으면 `Str`·`Fuzzy`를 그대로 둔다. 다르면 `Fuzzy = true`, `PreviousId = 예전 Id`, `Id = 새 원문`, `Str`은 그대로 둔다. 새 항목은 `Str = ""`. 원문에 없는 항목은 버린다.
  - 머리: 원문 머리를 `HeaderWithLanguage(…, "ko")`로 바꾸고 맨 앞에 `# source: <층> <경로>`와 `# game-version: <gameVersion>` 주석 두 줄을 `HeaderComments`에 둔다(원문의 `HeaderComments`는 버린다).
  - 원문 경로가 사라진 `.po`는 지우고 `RemovedFiles`에 적는다. `glossary.psv`는 건드리지 않는다.
  - `gameVersion`은 CLI가 넘긴다(Task 7). 빈 글자면 `# game-version:` 줄을 쓰지 않는다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

```csharp
var src = PoFile.Parse("msgid \"\"\nmsgstr \"\"\n\"Language: en\\n\"\n\nmsgctxt \"k1\"\nmsgid \"Tank\"\nmsgstr \"\"\n\nmsgctxt \"k2\"\nmsgid \"Attack now\"\nmsgstr \"\"\n\nmsgctxt \"k4\"\nmsgid \"New line\"\nmsgstr \"\"\n");
var old = PoFile.Parse("msgid \"\"\nmsgstr \"\"\n\nmsgctxt \"k1\"\nmsgid \"Tank\"\nmsgstr \"전차\"\n\nmsgctxt \"k2\"\nmsgid \"Attack\"\nmsgstr \"공격\"\n\nmsgctxt \"k3\"\nmsgid \"Gone\"\nmsgstr \"사라짐\"\n");
var m = L10nCatalog.Merge(src, old);
T.Eq(new[] { "k1", "k2", "k4" }, m.Entries.Select(e => e.Context).ToArray(), "source order, removed key dropped");
T.Eq("전차", m.Entries[0].Str, "unchanged keeps translation");
T.True(!m.Entries[0].Fuzzy, "unchanged not fuzzy");
T.True(m.Entries[1].Fuzzy, "extract_changed_marks_fuzzy");
T.Eq("Attack", m.Entries[1].PreviousId, "previous source kept");
T.Eq("공격", m.Entries[1].Str, "old translation kept for reuse");
T.Eq("", m.Entries[2].Str, "new key untranslated");
T.True(m.Header.Contains("Language: ko\\n"), "header language ko");
```
그리고 `Extract`를 임시 폴더에 두 번 돌려 두 번째 보고가 `Added=0, Changed=0, Removed=0`이고 파일 바이트가 같음을 본다(`extract_is_idempotent`). 원문 경로 하나를 빼고 다시 돌리면 그 `.po`가 지워진다.

- [ ] **Step 2: 실패를 본다**
- [ ] **Step 3: 구현한다** — 출력 경로의 폴더를 만든다. 쓰기는 UTF-8 BOM 없음, LF.
- [ ] **Step 4: 통과를 본다** — Expected: `failed=0`
- [ ] **Step 5: 커밋** — `git commit -m "feat(l10n): extract and merge translation catalog"`

---

### Task 4: 용어집과 검사 (check)

**Files:**
- Create: `editor/CtaToybox.Core/L10n/Glossary.cs`, `editor/CtaToybox.Core/L10n/BmFont.cs`, `editor/CtaToybox.Core/L10n/L10nCheck.cs`
- Create: `editor/CtaToybox.Core.Tests/L10nCheckTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `PoFile`(Task 1).
- Produces:
  - `public sealed class GlossaryTerm { public string Source; public string Target; public string Note; public bool Keep => Source == Target; }`
  - `public static List<GlossaryTerm> Glossary.Load(string psvPath)` — 형식 `원문|번역|메모`, `#`로 시작하는 줄과 빈 줄은 건너뜀. 파일이 없으면 빈 목록.
  - `public static HashSet<int> BmFont.ReadChars(byte[] fnt)` — BMFont 바이너리 v3(`BMF` + 버전 3). 블록 4(chars)의 20바이트 항목마다 앞 4바이트 `uint` id. `BMF`가 아니면 `InvalidDataException`.
  - `public sealed class CheckIssue { public string File; public string Key; public string Kind; public string Detail; }` — `Kind`는 `tags`, `placeholders`, `newlines`, `braces`, `keep-term`, `glyph`, `untranslated`, `fuzzy` 가운데 하나. 앞의 여섯은 오류, 뒤의 둘은 알림.
  - `public static List<CheckIssue> L10nCheck.Entry(string file, PoEntry e, List<GlossaryTerm> terms, HashSet<int> glyphs)` — `Str`이 비면 `untranslated` 하나만 낸다. 비교 규칙:
    - `tags`: 정규식 `<[^>]*>`로 뽑은 목록(순서 무관, 개수 포함)이 같다.
    - `placeholders`: `%(\d+\$)?[-+ 0#]*\d*(\.\d+)?[sdifuxXc%]` 목록이 **순서까지** 같다.
    - `newlines`: `\n`(두 글자) 개수가 같다.
    - `braces`: `\{[^}]*\}` 목록이 같다.
    - `keep-term`: `Keep`인 용어가 원문에 단어 경계로 있으면 번역에도 있다.
    - `glyph`: 번역의 글자(태그 안 제외, 서러게이트 쌍은 코드포인트로)가 모두 `glyphs`에 있다. `Detail`에 빠진 글자와 U+ 코드.
  - `public static List<CheckIssue> L10nCheck.Catalog(string catalogDir, List<GlossaryTerm> terms, HashSet<int> glyphs)` — 모든 `.po`에 `Entry`를 돈다.
  - `public static HashSet<int> L10nCheck.MainGlyphs(string gameDir, IEnumerable<string> fntNames)` — `<gameDir>/resource/interface/interface.pak`의 `font/<이름>`들을 읽어 **교집합**을 낸다(모든 대상 크기에 있는 글자만 통과). 이름 목록은 Task 5 `FontMap.TargetFnts`가 준다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

```csharp
var glyphs = new HashSet<int>("전차를 점령하라 %s<>()cF0/".Select(c => (int)c)) { ' ' };
var terms = new List<GlossaryTerm> { new GlossaryTerm { Source = "Panzer IV", Target = "Panzer IV" } };
PoEntry E(string id, string str) => new PoEntry { Context = "k", Id = id, Str = str, Comments = new List<string>() };
T.Eq(0, L10nCheck.Entry("f", E("<c(F00)>Capture</c> %s", "<c(F00)>점령하라</c> %s"), terms, glyphs).Count, "clean entry");
T.Eq("tags", L10nCheck.Entry("f", E("<c(F00)>Capture</c>", "점령하라"), terms, glyphs)[0].Kind, "missing tag");
T.Eq("placeholders", L10nCheck.Entry("f", E("%s of %d", "%d 중 %s"), terms, null)[0].Kind, "placeholder order matters");
T.Eq("newlines", L10nCheck.Entry("f", E("a\\nb", "가나"), terms, null)[0].Kind, "newline count");
T.Eq("keep-term", L10nCheck.Entry("f", E("Panzer IV tank", "4호 전차"), terms, null)[0].Kind, "keep term must stay");
var g = L10nCheck.Entry("f", E("Tank", "전차🙂"), terms, glyphs);
T.Eq("glyph", g[0].Kind, "check_missing_glyph");
T.True(g[0].Detail.Contains("U+1F642"), "glyph detail names the code point");
T.Eq("untranslated", L10nCheck.Entry("f", E("Tank", ""), terms, glyphs)[0].Kind, "empty is untranslated notice");
```
`BmFont.ReadChars`는 손으로 만든 바이트(블록 1·4만, 글자 `A`와 `가`)로 `{65, 0xAC00}`을 확인한다.

- [ ] **Step 2: 실패를 본다**
- [ ] **Step 3: 구현한다** — `glyphs`가 null이면 `glyph` 검사를 건너뛴다.
- [ ] **Step 4: 통과를 본다**; real 검사에 `MainGlyphs`로 `main_12.fnt`를 읽어 `0xAC00`·`0xD7A3`이 있는지 더한다(Run `… -- real`).
- [ ] **Step 5: 커밋** — `git commit -m "feat(l10n): glossary, bmfont glyphs and catalog checks"`

---

### Task 5: 글꼴 대응

**Files:**
- Create: `editor/CtaToybox.Core/L10n/FontMap.cs`
- Create: `editor/CtaToybox.Core.Tests/L10nFontTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `BmFont.ReadChars`(Task 4).
- Produces:
  - `public sealed class FontOverride { public string FontPath; public string Text; public string TargetFnt; }` — `FontPath`는 `interface/font/roboto_con_15.font` 꼴(출력 `resource/` 아래 상대 경로).
  - `public static readonly int[] FontMap.MainSizes = { 8, 9, 10, 12, 15, 18, 21, 27, 28, 30 };` `public static readonly int[] FontMap.MainBoldSizes = { 8, 9, 10, 12 };` — 한글이 있는 `main` 크기(조사 결과, `main_42`는 한글 없음).
  - `public static string FontMap.TargetFor(int size, bool bold)` — 굵게이고 `MainBoldSizes`에 가장 가까운 크기가 같은 크기면 `main_<n>b.fnt`, 아니면 `MainSizes`에서 가장 가까운 크기 `main_<n>.fnt`. 거리가 같으면 작은 쪽.
  - `public static List<FontOverride> FontMap.Build(string interfacePak)` — pak의 `font/*.font`를 모두 읽는다. `{name "main"}`인 것은 뺀다. 각 `.font`의 `binary_data`가 가리키는 `.fnt`에 `0xAC00`이 있으면 뺀다(이미 한글 있음). 나머지는 원래 `.font`에서 `binary_data` 값만 `TargetFor(size, 이름이 b로 끝나는지)`로 바꾼 글을 `Text`로 둔다(`name`·`size`·`height`·`style`은 원래 값 유지: AI 판단, 스펙이 빌더에게 맡긴 몫).
  - `public static IEnumerable<string> FontMap.TargetFnts(List<FontOverride> list)` — 쓰인 `main_*.fnt` 이름들(중복 없음).

- [ ] **Step 1: 실패하는 테스트를 쓴다**

```csharp
T.Eq("main_12.fnt", FontMap.TargetFor(13, false), "13 -> 12");
T.Eq("main_15.fnt", FontMap.TargetFor(14, false), "14 -> 15");
T.Eq("main_30.fnt", FontMap.TargetFor(42, false), "42 -> 30");
T.Eq("main_30.fnt", FontMap.TargetFor(60, false), "60 -> 30");
T.Eq("main_12b.fnt", FontMap.TargetFor(12, true), "bold kept when main has it");
T.Eq("main_15.fnt", FontMap.TargetFor(15, true), "bold falls back to regular");
T.Eq("main_10.fnt", FontMap.TargetFor(11, false), "tie between 10 and 12 goes smaller");
```
픽스처 pak: `font/roboto_con_15.font`(`binary_data "roboto_con_15.fnt"`)와 한글 없는 `.fnt`, `font/main_15.font`와 한글 있는 `.fnt`, `font/odd_12.font`와 한글 있는 `.fnt`. 기대: 덮어쓸 것은 `interface/font/roboto_con_15.font` 하나, 그 `Text`에 `binary_data "main_15.fnt"`와 원래 `{name "roboto_con"}`.

- [ ] **Step 2: 실패를 본다**
- [ ] **Step 3: 구현한다** — `.font` 값 읽기는 `SdlCore.Parse`(이미 있는 파서)로 하고, 쓰기는 원문에서 `binary_data` 따옴표 값만 바꾼다.
- [ ] **Step 4: 통과를 본다**; real 검사에 바닐라 `interface.pak`으로 `Build`를 돌려 결과에 `main`이 없고 `roboto_con_15`가 있는지 더한다.
- [ ] **Step 5: 커밋** — `git commit -m "feat(l10n): map hangul-less fonts to main atlases"`

---

### Task 6: 출력 빌드 (build)

**Files:**
- Create: `editor/CtaToybox.Core/L10n/L10nBuilder.cs`
- Create: `editor/CtaToybox.Core.Tests/L10nBuildTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `L10nSource`(Task 2), `PoFile`(Task 1), `FontOverride`(Task 5).
- Produces:
  - `public sealed class L10nBuildReport { public int Files, Translated, FellBack; public List<string> Written, Removed, Errors; }`
  - `public static L10nBuildReport L10nBuilder.Build(List<L10nSource> sources, string catalogDir, List<FontOverride> fonts, string outputModDir)` —
    - 원문 `.pot`마다 출력 `<outputModDir>/localizations/default/<Path>`를 쓴다. 원문 구조(항목 순서, 주석, `msgctxt`)를 지키고 `msgid`만 바꾼다: 짝 항목의 `Str`이 비어 있지 않고 `Fuzzy`가 아니면 `Str`, 아니면 원문 `Id`(`FellBack`에 센다). 머리는 `HeaderWithLanguage(…, "ko")`. `.po`가 없는 경로도 원문 그대로 쓴다(덮어쓰기 규칙을 모든 경로에 같게: AI 판단).
    - 글꼴마다 `<outputModDir>/resource/<FontPath>`에 `Text`를 쓴다.
    - 자기 목록 `<outputModDir>/.ctatoybox-l10n.json`(`{"files":[{"path":"localizations/default/…"}]}`, 출력 모드 기준 상대 경로, `/`)을 쓴다. 지난 목록에 있고 이번에 없는 파일은 지운다. 목록 밖 파일은 지우지 않는다.
    - 쓰기 전에 `<outputModDir>/.ctatoybox-build.json`의 `files[].path`(이 파일은 `resource` 기준 상대 경로)와 겹치는 글꼴 경로가 있으면 그 파일은 쓰지 않고 `Errors`에 더한다. 지난 l10n 목록에 없는데 이미 있는 파일도 덮어쓰지 않고 `Errors`에 더한다.
    - 쓰기는 UTF-8 BOM 없음, LF.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

```csharp
// 원문 하나(k1 "Tank", k2 "Attack"), .po에 k1 번역 "전차", k2는 fuzzy 번역 "공격"
string pot = File.ReadAllText(Path.Combine(outDir, "localizations/default/interface/text/a.pot"));
T.True(pot.Contains("msgid \"전차\""), "translated msgid");
T.True(pot.Contains("msgid \"Attack\""), "build_fuzzy_falls_back");
T.True(pot.Contains("Language: ko\\n"), "language header ko");
T.True(!pot.Contains("\r"), "LF only");
T.Eq(1, report.FellBack, "fallback counted");
// 출력 모드에 .ctatoybox-build.json {"files":[{"path":"interface/font/x_12.font"}]} 과 사용자 파일 localizations/default/user.pot을 미리 둔다
T.True(report.Errors.Any(e => e.Contains("x_12.font")), "build_never_touches_editor_files: editor build file reported");
T.Eq("편집기 것", File.ReadAllText(Path.Combine(outDir, "resource/interface/font/x_12.font")), "editor file unchanged");
// 원문 경로를 하나 빼고 다시 빌드
T.True(!File.Exists(Path.Combine(outDir, "localizations/default/interface/text/a.pot")), "stale own file removed");
T.True(File.Exists(Path.Combine(outDir, "localizations/default/user.pot")), "file outside own list kept");
```

- [ ] **Step 2: 실패를 본다**
- [ ] **Step 3: 구현한다**
- [ ] **Step 4: 통과를 본다** — 출력 `.pot`을 `DisplayNames`가 읽는 법(`AddPot`)으로 다시 읽어 `desc/...` 이름이 한글로 나오는지도 한 줄 더한다(왕복).
- [ ] **Step 5: 커밋** — `git commit -m "feat(l10n): build korean pot files and font overrides into output mod"`

---

### Task 7: 명령줄 `l10n`

**Files:**
- Modify: `editor/CtaToybox.Cli/Program.cs` (`Usage`, `KnownOptions`, `Flags`, `switch`, 새 `L10n` 메서드)
- Create: `editor/CtaToybox.Core.Tests/L10nCliTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: Task 2~6의 `Collect`, `Extract`, `Glossary.Load`, `Catalog`, `MainGlyphs`, `FontMap.Build`, `L10nBuilder.Build`. 기존 `Open(settingsPath, output, out settings, out plan)`(층만 쓰고 `stack`은 쓰지 않아도 된다), `GameRunning`.
- Produces: 명령
  - `ctatoybox l10n extract [--dir <폴더, 기본 l10n/ko>]` — 요약 한 줄: `파일 N, 항목 N, 추가 N, 바뀜 N, 지움 N, 미번역 N, fuzzy N`. 지운 파일 목록.
  - `ctatoybox l10n check [--dir …] [--file <글자>]` — 오류마다 `오류 <파일> <키> <종류>: <내용>`, 끝에 `오류 N, 미번역 N, fuzzy N`. 오류가 있으면 종료 코드 1. `--file`은 경로에 그 글자가 든 `.po`만.
  - `ctatoybox l10n build [--dir …] [--force]` — 게임이 실행 중이면 `--force` 없이 멈춘다(`build`와 같은 문구). `check` 오류가 하나라도 있으면 쓰지 않고 1. 보고: `쓴 파일 N(번역 N, 영어로 둔 항목 N), 지운 파일 N` + `Errors`.
  - `KnownOptions["l10n"] = { "settings", "dir", "file", "force" }`. 하위 명령이 없거나 모르면 사용법 오류(2).
  - `extract`의 `gameVersion`: `<GameDir>` 아래에서 버전 글을 찾을 수 있는 파일이 있으면 그것, 없으면 빈 글자(구현자가 확인해 정한다; 못 찾으면 빈 글자).

- [ ] **Step 1: 실패하는 테스트를 쓴다** — `CliTests`처럼 `CliApp.Run(args, writer)`로 픽스처(Task 6과 같은 꼴)에 `l10n extract` → `.po`가 생김, `.po`에 번역을 넣고 `l10n check` → 0, 태그 깨진 번역 → 종료 1과 `오류 … tags`, `l10n build` → 출력 `.pot` 생김, 모르는 하위 명령 → 2.
- [ ] **Step 2: 실패를 본다**
- [ ] **Step 3: 구현한다**
- [ ] **Step 4: 통과를 본다**
- [ ] **Step 5: 실제 데이터로 extract를 돌린다**

Run: `dotnet run --project editor/CtaToybox.Cli -c Release -- l10n extract --settings E:\CTAToybox\editor\settings.json`
Expected: `파일`이 바닐라 362 + 모드 고유 파일 수(같은 경로 2개는 한 번), `항목`이 약 28,000. `l10n/ko/`에 `.po`가 생긴다. 미번역 = 항목 수.

- [ ] **Step 6: 커밋** — `l10n/ko/*.po`(전부 미번역)와 코드를 함께 커밋. `git commit -m "feat(l10n): l10n extract/check/build commands and initial catalog"`

---

### Task 8: 편집기 이름 `원문 (한글)`

**Files:**
- Modify: `editor/CtaToybox.Core/DisplayNames.cs`
- Modify: `editor/CtaToybox.Core/EditorSession.cs:130` 근처
- Modify: `editor/CtaToybox.Cli/Program.cs:293` 근처(`units`)
- Create: `editor/CtaToybox.Core.Tests/L10nNamesTests.cs`
- Modify: `editor/CtaToybox.Core.Tests/Program.cs`

**Interfaces:**
- Consumes: `PoFile`(Task 1).
- Produces:
  - `public void DisplayNames.AddTranslations(string catalogDir)` — 모든 `.po`에서 `Context`가 있고 `Str`이 비지 않고 `Fuzzy`가 아닌 항목을 `ctxt → Clean(Str)`로 모은다.
  - `OfUnit`이 `byContext`에서 찾은 이름에 한글이 있으면 `<원문> (<한글>)`을 낸다. 원문과 한글이 같으면 원문만. `byPath`(게임 설정 파일 이름)는 그대로. `_x` 꼬리 규칙의 `" (" + stem + ")"`는 한글을 붙인 뒤에 붙인다: `<원문> (<한글>) (<stem>)`.
  - `EditorSession.Open`은 `s.Names.AddTranslations(Path.Combine(repoRoot, "l10n", "ko"))`를 `FromGame` 바로 뒤에 부른다. CLI `units`는 `--names`가 없을 때 같은 일을 하되 저장소 루트는 설정 파일 폴더의 부모(`editor/settings.json` → 저장소)로 한다.
  - 검색: `UnitIndex.Search`가 `OfUnit` 결과에 대해 찾으므로 원문·한글 모두로 찾힌다. 테스트로 확인만 한다.

- [ ] **Step 1: 실패하는 테스트를 쓴다**

```csharp
var n = new DisplayNames();
n.AddPot("msgctxt \"desc/entity/kv85\"\nmsgid \"KV-85\"\nmsgstr \"\"\n\nmsgctxt \"desc/human/ger/rifleman\"\nmsgid \"Rifleman\"\nmsgstr \"\"\n");
// l10n 폴더에 desc/human/ger/rifleman → 소총수, desc/entity/kv85 → KV-85(같음)
n.AddTranslations(dir);
T.Eq("Rifleman (소총수)", n.OfUnit("set/breed/ger/rifleman.set"), "source (korean)");
T.Eq("KV-85", n.OfUnit("entity/vehicle/tank/kv85.def"), "same text shows once");
T.Eq("KV-85 (kv85_x)", n.OfUnit("entity/vehicle/tank/kv85_x.def"), "variant tail after names");
```
(`AddPot`은 이미 `public`이다.) 그리고 `UnitIndex.Search("소총", null, null, names)`가 그 유닛을 찾는지 한 줄.

- [ ] **Step 2: 실패를 본다**
- [ ] **Step 3: 구현한다**
- [ ] **Step 4: 통과를 본다**. 화면 확인은 Task 11 첫 빌드 뒤에 한 번만 한다: `CLAUDE.md`의 편집기 화면 옵션(`--unit <번역된 유닛> --tab summary --screenshot <스크래치 폴더>\names.png`)으로 이름이 `원문 (한글)`인지 본다.
- [ ] **Step 5: 커밋** — `git commit -m "feat(editor): show unit names as source (korean)"`

---

### Task 9: 문서

**Files:**
- Modify: `CLAUDE.md` — 폴더 표에 `l10n/` 줄(“한글화 번역 보관. 손으로 고친다”), "명령"에 `l10n extract|check|build` 한 줄, "손으로 고치는 파일"에 `l10n/ko/*.po`, `l10n/ko/glossary.psv`. 출력 모드 생성물에 `localizations/`와 `.ctatoybox-l10n.json`, `resource/interface/font/` 글꼴이 `l10n build` 생성물이라는 한 줄.
- Modify: `docs/07-stat-editor/README.md` — `l10n` 명령 사용법 절(갱신 → 번역 → 검사 → 빌드 순서, 미확인 5개를 스펙에서 링크).
- Modify: `docs/10-backlog.md` — P10을 "진행(구현 계획 `docs/superpowers/plans/2026-10-17-korean-localization.md`)"으로.
- [ ] **Step 1: 고친다**
- [ ] **Step 2: 전체 테스트** — Run: `pwsh -NoProfile -File tools/tests/Run-Tests.ps1` / Expected: 실패 0
- [ ] **Step 3: 커밋** — `git commit -m "docs: l10n commands and files"`

---

### Task 10: 용어집 첫 목록

**Files:**
- Create: `l10n/ko/glossary.psv`

- [ ] **Step 1: 후보를 뽑는다** — `desc` 원문에서 전차·장비 이름(`desc/entity/*`), 계급, 병과명, 분대 종류, 진영, 무기·탄종 약어를 모은다(스크립트는 스크래치 폴더에, 저장소에 넣지 않는다).
- [ ] **Step 2: 규칙대로 채운다** — Global Constraints의 번역 규칙. 원문 유지 말은 `Panzer IV Ausf. H|Panzer IV Ausf. H|장비`처럼 원문=번역. 음독은 `Grossdeutschland|그로스도이칠란트|부대`. 일반 군사 용어(Squad 분대, Platoon 소대, Reinforcements 증원 등)도 적는다. 머리 주석에 형식과 규칙을 적는다.
- [ ] **Step 3: 커밋** — `git commit -m "l10n: initial glossary"`

---

### Task 11~14: 번역 (단계마다 한 Task)

| Task | 범위(`l10n/ko/` 아래) | 대략 항목 수 |
|---|---|---|
| 11 | `interface/text/desc/**` | 12,048 + 모드의 `desc` |
| 12 | `interface/text/*.po`(폴더 바로 아래), `names/`, `announcements/` | 약 4,200 |
| 13 | `interface/text/mission/**` | 6,201 + 모드의 `mission` |
| 14 | Task 11~13에 들지 않은 모드 파일(원문 층이 `mod_*`인 나머지) | 나머지 |

각 Task의 단계(같다):

- [ ] **Step 1: 묶음을 나눈다** — 파일 하나 또는 항목 약 300개 단위.
- [ ] **Step 2: 묶음마다 번역한다** — 번역자(서브에이전트 가능)에게 `.po` 파일 경로, 용어집, Global Constraints의 번역 규칙·문체·그대로 둘 것을 주고, `msgstr`만 채우게 한다. `msgctxt`·`msgid`·주석은 건드리지 않는다. 위임 지시에는 "이 묶음만 번역하라. 끝나면 보고하고 정지하라. 모든 파일 명령에 절대경로를 쓰라"를 넣는다.
- [ ] **Step 3: 검사** — Run: `dotnet run --project editor/CtaToybox.Cli -c Release -- l10n check --settings E:\CTAToybox\editor\settings.json --file <묶음 경로>` / Expected: `오류 0`. 오류는 그 항목만 다시 번역.
- [ ] **Step 4: 표본 읽기** — 묶음마다 20항목을 컨트롤러가 직접 읽어 용어집·문체 규칙을 지켰는지 본다. 어긋나면 그 묶음을 다시 맡긴다.
- [ ] **Step 5: 커밋** — 묶음 몇 개마다 `git commit -m "l10n(ko): translate <범위>"`.
- [ ] **Step 6: 단계 끝 빌드** — 모든 묶음이 끝나면 `l10n check` 전체 `오류 0`, 이어 `dotnet run --project editor/CtaToybox.Cli -c Release -- l10n build --settings E:\CTAToybox\editor\settings.json`. 출력 `mod/CTAToybox/localizations/`, 글꼴, `.ctatoybox-l10n.json`을 커밋한다(`.ctatoybox-build.json`과 `resource`의 편집기 생성물은 건드리지 않았는지 `git status`로 본다). `git commit -m "build(l10n): korean localization <범위>"`.

---

### Task 15: 맵·미션 파일의 박힌 글 조사 (보고만)

**Files:** 없음(조사 결과는 공유 노트 폴더나 스크래치에. 저장소에 넣지 않는다)

- [ ] **Step 1:** 바닐라 `resource/map/*.pak`과 켜 둔 모드의 `resource` 안 맵·미션 파일(`.mi`, `.scn`, `.lua`, `.set` 등 텍스트)에서 따옴표 안 영어 문장(대문자로 시작, 낱말 4개 이상)을 찾는다. 게임 폴더는 읽기만.
- [ ] **Step 2:** 찾은 것을 파일·줄·글로 사용자에게 보이고 다룰지 묻는다. 이 계획에서는 고치지 않는다(스펙 "하지 않는 것").

---

### Task 16: 마무리

- [ ] **Step 1:** `pwsh -NoProfile -File tools/tests/Run-Tests.ps1` 실패 0, `dotnet run --project editor/CtaToybox.Core.Tests -c Release -- real` 실패 0.
- [ ] **Step 2:** `l10n check` 전체 `오류 0, 미번역 0, fuzzy 0`.
- [ ] **Step 3:** `docs/10-backlog.md`의 P10 줄을 지우고 "정리하며 지운 것"에 한 줄. 스펙 머리 상태를 "구현 끝(2026-MM-DD)"으로. 미확인 5개는 그대로 둔다.
- [ ] **Step 4:** `feat/korean-localization` → `develop` `git merge --no-ff`.
