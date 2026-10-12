param(
    [Parameter(Mandatory)][string]$DataDir,
    [Parameter(Mandatory)][string]$AnnotationDir,
    [Parameter(Mandatory)][string]$OutDir
)
# 유닛의 수정 가능한 속성을 "값이 적힌 파일"별로 정리한 문서를 만든다.
# 수정 가능한 속성 = 원본 파일에 값(인자)이 하나라도 적힌 블록과 매크로 호출. 모델 파일(.mdl, .mtl)은 뺀다.
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$DataDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($DataDir)
$AnnotationDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($AnnotationDir)
$OutDir = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutDir)
[IO.Directory]::CreateDirectory($OutDir) | Out-Null
$utf8 = [Text.UTF8Encoding]::new($false)
. "$PSScriptRoot/lib/Annotations.ps1"

function Cell([string]$s) { ($s -replace '\r?\n', ' ' -replace '\|', '\|').Trim() }
function Code([string]$s) { '`' + ($s -replace '`', "'" -replace '\|', '\|') + '`' }

# keys-<그룹>.csv의 한 파일 종류에서 값이 있는 행을 고른다
function Get-CsvRows([string]$Group, [string]$FileKind) {
    foreach ($r in $script:keys[$Group]) {
        if ($r.FileKind -ne $FileKind -or $r.Form -notin 'block', 'macro') { continue }
        if (-not @($r.ArgShapes -split ' ' | Where-Object { $_ -and $_ -notmatch '^\[\]x\d+$' })) { continue }
        $value = if ($r.NumMin -ne '') { if ($r.NumMin -eq $r.NumMax) { "$($r.NumMin)" } else { "$($r.NumMin) ~ $($r.NumMax)" } }
        elseif ($r.Values -and $r.Values -ne '(13종 이상)') { $r.Values }
        else { "예: $($r.ExampleArgs) (13종 이상)" }
        [pscustomobject]@{ FileKind = $r.FileKind; Path = $r.Path; Form = $r.Form; FileCount = [int]$r.FileCount; Value = $value; Comments = $r.Comments; Example = $r.ExampleFile }
    }
}

# units-shared.jsonl.gz에서 조건에 맞는 파일만 골라 같은 형태로 집계한다.
# keys-shared.csv는 공용 그룹 전체를 합친 것이라 폴더별로 나눌 수 없다
$numberRegex = [regex]'^[+-]?(\d+\.?\d*|\.\d+)$'
function Get-SharedRows([scriptblock]$Match) {
    $path = Join-Path $DataDir 'units-shared.jsonl.gz'
    if (-not (Test-Path -LiteralPath $path) -or (Get-Item -LiteralPath $path).Length -eq 0) { return }
    $agg = [ordered]@{}
    $gz = [IO.Compression.GZipStream]::new([IO.File]::OpenRead($path), [IO.Compression.CompressionMode]::Decompress)
    $sr = [IO.StreamReader]::new($gz)
    try {
        while ($null -ne ($line = $sr.ReadLine())) {
            if ($line -notmatch '^\{"Id":"([^"]+)"' -or -not (& $Match $Matches[1])) { continue }
            $id = $Matches[1]
            $kind = [IO.Path]::GetExtension($id); if (-not $kind) { $kind = '(no-ext)' }
            foreach ($raw in (ConvertFrom-Json $line).Raw) {
                if ($raw[1] -notin 'block', 'macro') { continue }
                $k = "$kind`t$($raw[0])`t$($raw[1])"
                $a = $agg[$k]
                if (-not $a) {
                    $a = [pscustomobject]@{ FileKind = $kind; Path = $raw[0]; Form = $raw[1]; FileCount = 0; Last = ''; Values = [ordered]@{}; AllNum = $true; Min = $null; Max = $null; MinText = ''; MaxText = ''; Example = $id }
                    $agg[$k] = $a
                }
                if ($a.Last -ne $id) { $a.FileCount++; $a.Last = $id }
                $v = [string]$raw[2]
                if (-not $v) { continue }
                if ($a.AllNum -and $numberRegex.IsMatch($v)) {
                    $d = [double]::Parse($v, [cultureinfo]::InvariantCulture)
                    if ($null -eq $a.Min -or $d -lt $a.Min) { $a.Min = $d; $a.MinText = $v }
                    if ($null -eq $a.Max -or $d -gt $a.Max) { $a.Max = $d; $a.MaxText = $v }
                }
                else { $a.AllNum = $false }
                if ($a.Values.Count -le 12) { $a.Values[$v] = 1 }
            }
        }
    }
    finally { $sr.Dispose() }
    foreach ($a in $agg.Values) {
        if ($a.Values.Count -eq 0) { continue }
        $value = if ($a.AllNum) { if ($a.Min -eq $a.Max) { $a.MinText } else { "$($a.MinText) ~ $($a.MaxText)" } }
        elseif ($a.Values.Count -le 12) { @($a.Values.Keys) -join ' ¦ ' }
        else { "예: $(@($a.Values.Keys)[0]) (13종 이상)" }
        $c = $script:sharedComments["$($a.FileKind)`t$($a.Path)`t$($a.Form)"]
        [pscustomobject]@{ FileKind = $a.FileKind; Path = $a.Path; Form = $a.Form; FileCount = $a.FileCount; Value = $value; Comments = [string]$c; Example = $a.Example }
    }
}

$script:keys = @{}
foreach ($g in 'human', 'vehicle', 'weapon-ammo', 'inventory') { $script:keys[$g] = @(Import-Csv -LiteralPath (Join-Path $DataDir "keys-$g.csv")) }
$script:sharedComments = @{}
foreach ($k in (Import-Csv -LiteralPath (Join-Path $DataDir 'keys-shared.csv'))) { if ($k.Comments) { $script:sharedComments["$($k.FileKind)`t$($k.Path)`t$($k.Form)"] = $k.Comments } }
$script:ann = @{}
foreach ($g in 'human', 'vehicle', 'weapon-ammo', 'inventory', 'shared') { $script:ann[$g] = Read-Annotations -AnnotationDir $AnnotationDir -Group $g }

function CsvSection([string]$Role, [string]$Name, [string]$Where, [string]$Link, [string]$Group, [string]$FileKind) {
    @{ Role = $Role; Name = $Name; Where = $Where; Link = $Link; Group = $Group; Rows = @(Get-CsvRows $Group $FileKind) }
}
function SharedSection([string]$Role, [string]$Name, [string]$Where, [string]$Link, [scriptblock]$Match) {
    @{ Role = $Role; Name = $Name; Where = $Where; Link = $Link; Group = 'shared'; Rows = @(Get-SharedRows $Match) }
}

$pages = @(
    @{
        File = 'infantry.md'; Title = '보병'
        Intro = '보병 하나는 `set/breed` 아래의 `.set` 파일 하나다. 능력치는 본체에 직접 적히지 않고 `perks`와 `veterancy` 안에서 매크로로 불러오므로, 능력치를 바꾸려면 연관 파일의 매크로 본문을 고치거나 본체의 매크로 호출 인자를 고친다. 들고 있는 무기와 장구의 속성은 [weapon-ammo.md](weapon-ammo.md)에 있다.'
        Sections = @(
            (CsvSection '본체' '보병 정의' 'set/breed/**/*.set' '유닛 그 자체. 파일 하나가 보병 하나' 'human' '.set'),
            (CsvSection '연관' '능력치 매크로' 'set/breed/**/*.inc' '본체의 `perks`, `veterancy` 안에서 `(include)`와 매크로 호출로 불러온다. 폴더의 `ability.inc`가 `set/breed/presets/perks/`의 공용 정의를 include한다' 'human' '.inc'),
            (CsvSection '연관' '외형 엔티티' 'entity/humanskin/**/*.def' '본체의 `skin`이 이 파일의 이름을 가리킨다' 'human' '.def')
        )
    },
    @{
        File = 'vehicle.md'; Title = '차량류 (차량, 전차, 화포, 항공기, 함정)'
        Intro = '차량류 하나는 `entity/-vehicle` 아래의 `.def` 파일 하나다. 많은 값이 본체가 아니라 include하는 파일에 있다. 장갑·기동 같은 차종 공통 값은 `properties/`의 공통 속성에, 그 차량만의 값은 같은 폴더의 `.ext`와 `.inc`에 있다. 탑재 무기와 포탄의 속성은 [weapon-ammo.md](weapon-ammo.md)에 있다.'
        Sections = @(
            (CsvSection '본체' '차량 엔티티' 'entity/-vehicle/<진영>/<분류>/<이름>/<이름>.def' '유닛 그 자체. 파괴 모델용 `.def`도 같은 형식이다' 'vehicle' '.def'),
            (CsvSection '연관' '차량 전용 속성 (.ext)' 'entity/-vehicle/**/*.ext' '본체의 `(include "이름.ext")`' 'vehicle' '.ext'),
            (CsvSection '연관' '차량 전용 속성 (.inc)' 'entity/-vehicle/**/*.inc' '본체나 다른 차량의 `(include "../이름/이름.inc")`. 파생형이 기본형의 값을 물려받을 때 쓴다' 'vehicle' '.inc'),
            (SharedSection '연관' '공통 속성 (.ext)' 'properties/**/*.ext' '본체의 `(include "/properties/...")`. 같은 차종의 모든 유닛이 공유한다' { param($id) $id.StartsWith('properties/') -and $id.EndsWith('.ext') }),
            (SharedSection '연관' '공통 속성 (.inc)' 'properties/**/*.inc' '본체와 공통 속성 `.ext`의 `(include "/properties/...")`. 같은 차종의 모든 유닛이 공유한다' { param($id) $id.StartsWith('properties/') -and $id.EndsWith('.inc') })
        )
    },
    @{
        File = 'weapon-ammo.md'; Title = '무기, 탄약, 장구'
        Intro = '무기·탄약·수류탄·지뢰·장구는 모두 `set/stuff` 아래에 있다. 보병은 `inventory/item`과 `armors`로, 차량은 `weaponry/place/weapon`과 `extender "inventory"/box/item`으로 이 파일들을 가리킨다. 한 파일을 여러 유닛이 함께 쓰므로 여기의 값을 고치면 그 무기나 탄약을 쓰는 모든 유닛이 바뀐다.'
        Sections = @(
            (CsvSection '본체' '무기와 장구 정의 (확장자 없음)' 'set/stuff/<폴더>/<이름>' '유닛의 `inventory/item`, `armors/*`, `weaponry/place/weapon`이 이 파일의 이름을 가리킨다. `gun`, `mgun`, `rifle`, `smg`, `pistol`, `head` 등의 폴더' 'weapon-ammo' '(no-ext)'),
            (CsvSection '본체' '무기 정의 (.weapon)' 'set/stuff/{reactive,mortar,bazooka,explosive}/*.weapon' '위와 같다. 로켓, 박격포, 대전차 화기, 폭발물' 'weapon-ammo' '.weapon'),
            (CsvSection '본체' '탄약 (.ammo)' 'set/stuff/**/*.ammo' '무기의 `filling`과 차량의 `extender "inventory"/box/item`이 탄약 태그로 가리킨다' 'weapon-ammo' '.ammo'),
            (CsvSection '본체' '수류탄 (.grenade)' 'set/stuff/grenade/*.grenade' '보병의 `inventory/item`' 'weapon-ammo' '.grenade'),
            (CsvSection '본체' '지뢰와 폭발물 (.mine)' 'set/stuff/explosive/*.mine' '보병의 `inventory/item`' 'weapon-ammo' '.mine'),
            (CsvSection '본체' '투척 (.hand)' 'set/stuff/thrower.hand' '파일이 하나뿐이다. 어디서 가리키는지는 확인하지 않았다' 'weapon-ammo' '.hand'),
            (CsvSection '연관' '원형 (.pattern)' 'set/stuff/**/*.pattern' '무기와 탄약의 `{from "pattern 이름"}`. 같은 원형을 쓰는 모든 아이템이 값을 물려받는다' 'weapon-ammo' '.pattern'),
            (CsvSection '연관' '폴더의 매크로 모음 (.presets)' 'set/stuff/<폴더>/.presets' '그 폴더와 하위 폴더의 파일이 include 없이 쓰는 매크로 정의. `("fire rate" rpm(240))` 같은 호출의 본문이 여기에 있다' 'weapon-ammo' '.presets'),
            (CsvSection '연관' '무기 상태 설정 (.fsm)' 'set/stuff/_settings/*.fsm' '어디서 가리키는지는 확인하지 않았다' 'weapon-ammo' '.fsm'),
            (SharedSection '연관' '폭발 계산 매크로' 'set/blast.inc' '탄약의 `("he_level_3")` 같은 매크로 호출과 `.presets`의 `(include "\set\blast.inc")`' { param($id) $id -eq 'set/blast.inc' })
        )
    },
    @{
        File = 'item-entity.md'; Title = '아이템 엔티티'
        Intro = '아이템이 땅에 놓이거나 손에 들렸을 때의 3D 엔티티다. `set/stuff`의 아이템이 `entity`로 이 파일을 가리킨다. 전투 수치는 여기가 아니라 [weapon-ammo.md](weapon-ammo.md)의 파일에 있다.'
        Sections = @(
            (CsvSection '본체' '아이템 엔티티' 'entity/inventory/**/*.def' '`set/stuff` 아이템의 `entity`' 'inventory' '.def')
        )
    }
)

$header = '| 속성명 | 속성값 (범위·예) | 설명 | 게임 파일의 주석 (참고) | 파일 수 | 예시 파일 |'
$sep = '|---|---|---|---|---|---|'
$summary = [Collections.Generic.List[object]]::new()

foreach ($page in $pages) {
    $sb = [Text.StringBuilder]::new()
    [void]$sb.AppendLine("# 수정 가능한 속성: $($page.Title)")
    [void]$sb.AppendLine()
    [void]$sb.AppendLine('이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다. 설명은 `data/annotations/common.psv`에서 고친다. 표 읽는 법은 [README.md](README.md)에 있다.')
    [void]$sb.AppendLine()
    [void]$sb.AppendLine($page.Intro)
    [void]$sb.AppendLine()
    [void]$sb.AppendLine('| 구분 | 파일 | 위치 | 속성 수 | 뜻을 아는 속성 |')
    [void]$sb.AppendLine('|---|---|---|---|---|')
    $body = [Text.StringBuilder]::new()
    foreach ($s in $page.Sections) {
        $known = [Collections.Generic.List[string]]::new(); $unknown = [Collections.Generic.List[string]]::new()
        foreach ($r in ($s.Rows | Sort-Object Path, Form)) {
            $a = Get-Annotation $script:ann[$s.Group] $r.FileKind $r.Path $r.Form ($s.Group -ne 'shared' -and $UnitFileKinds -contains $r.FileKind)
            $isKnown = $a.Grade -in '확인', '추정'
            $desc = if ($isKnown) { "**$($a.Grade)** $($a.Meaning)" } elseif ($a.Meaning) { "미확인. $($a.Meaning)" } else { '미확인' }
            $row = "| $(Code $r.Path) | $(Cell $r.Value) | $(Cell $desc) | $(Cell $r.Comments) | $($r.FileCount) | $(Code $r.Example) |"
            if ($isKnown) { $known.Add($row) } else { $unknown.Add($row) }
        }
        $total = $known.Count + $unknown.Count
        [void]$sb.AppendLine("| $($s.Role) | $($s.Name) | $(Code $s.Where) | $total | $($known.Count) |")
        $summary.Add([pscustomobject]@{ Page = $page.File; Title = $page.Title; Role = $s.Role; Name = $s.Name; Where = $s.Where; Total = $total; Known = $known.Count })

        [void]$body.AppendLine("## $($s.Role): $($s.Name)")
        [void]$body.AppendLine()
        [void]$body.AppendLine("- 위치: $(Code $s.Where)")
        [void]$body.AppendLine("- 연결: $($s.Link)")
        [void]$body.AppendLine()
        if ($total -eq 0) { [void]$body.AppendLine('값이 적힌 속성이 없다.'); [void]$body.AppendLine(); continue }
        if ($known.Count -gt 0) {
            [void]$body.AppendLine("### 뜻을 아는 속성 ($($known.Count)개)")
            [void]$body.AppendLine()
            [void]$body.AppendLine($header); [void]$body.AppendLine($sep)
            foreach ($row in $known) { [void]$body.AppendLine($row) }
            [void]$body.AppendLine()
        }
        if ($unknown.Count -gt 0) {
            [void]$body.AppendLine("### 뜻을 모르는 속성 ($($unknown.Count)개)")
            [void]$body.AppendLine()
            [void]$body.AppendLine($header); [void]$body.AppendLine($sep)
            foreach ($row in $unknown) { [void]$body.AppendLine($row) }
            [void]$body.AppendLine()
        }
    }
    [void]$sb.AppendLine()
    [void]$sb.Append($body.ToString())
    [IO.File]::WriteAllText((Join-Path $OutDir $page.File), $sb.ToString(), $utf8)
    Write-Host "$($page.File) attributes=$(($summary | Where-Object Page -eq $page.File | Measure-Object Total -Sum).Sum)"
}

$sb = [Text.StringBuilder]::new()
[void]$sb.AppendLine(@'
# 유닛의 수정 가능한 속성

이 파일은 `tools/New-EditableDoc.ps1`이 생성한다. 손으로 고치지 않는다.

유닛의 값을 바꾸려 할 때 "그 값이 어느 파일에 있는가"를 찾기 위한 문서다. 유닛 종류마다, 값이 적힌 파일을 본체와 연관 파일로 나눠 속성명·속성값·설명을 실었다.

## 문서

| 문서 | 대상 |
|---|---|
| [infantry.md](infantry.md) | 보병 |
| [vehicle.md](vehicle.md) | 차량, 전차, 화포, 항공기, 함정 |
| [weapon-ammo.md](weapon-ammo.md) | 무기, 탄약, 수류탄, 지뢰, 장구 |
| [item-entity.md](item-entity.md) | 아이템의 3D 엔티티 |

## 파일별 속성 수

'@)
[void]$sb.AppendLine('| 문서 | 구분 | 파일 | 위치 | 속성 수 | 뜻을 아는 속성 |')
[void]$sb.AppendLine('|---|---|---|---|---|---|')
foreach ($s in $summary) { [void]$sb.AppendLine("| [$($s.Page)]($($s.Page)) | $($s.Role) | $($s.Name) | $(Code $s.Where) | $($s.Total) | $($s.Known) |") }
[void]$sb.AppendLine("| 합계 | | | | $(($summary | Measure-Object Total -Sum).Sum) | $(($summary | Measure-Object Known -Sum).Sum) |")
[void]$sb.AppendLine(@'

## 표 읽는 법

| 열 | 내용 |
|---|---|
| 속성명 | 파일 안에서의 키 경로. 표기 규칙은 [../04-units/README.md](../04-units/README.md)의 "경로 표기"와 같다. `("fire rate")`처럼 괄호와 따옴표로 된 이름은 매크로 호출이고 값은 그 인자다. `(define "이름")/...`은 매크로 본문 안의 값이다 |
| 속성값 | 바닐라 파일에 실제로 쓰인 값. 숫자 하나면 최솟값 ~ 최댓값, 종류가 12개 이하면 전부, 그보다 많으면 예 하나 |
| 설명 | **확인**은 게임 파일의 주석이 그 키를 직접 설명한 것, **추정**은 이름·값·쓰인 자리에서 추론한 것, 미확인은 뜻을 모르는 것이다 |
| 게임 파일의 주석 | 같은 키 옆에 개발자가 남긴 주석을 모은 것. 그 줄의 값에 대한 메모인 경우가 많아 키의 뜻과 다를 수 있다 |
| 파일 수 | 이 속성이 적힌 파일의 수 |
| 예시 파일 | 이 속성이 적힌 파일 하나 |

구분의 **본체**는 유닛 그 자체인 파일이고, **연관**은 본체가 include·매크로·이름으로 끌어오는 파일이다. 연관 파일의 값은 여러 유닛이 함께 쓴다.

## 무엇을 "수정 가능한 속성"으로 셌나

- 원본 파일에 값(인자)이 하나라도 적힌 블록과 매크로 호출이다. 값 없이 이름만 있는 블록(하위 블록을 담는 그릇, 인자 없는 플래그)은 뺐다.
- `(include ...)`, `(define ...)`의 이름, `(mod ...)` 줄 자체는 뺐다. 그 안에 든 블록은 들어 있다.
- 모델 파일(`.mdl`, `.mtl`)은 뺐다.
- 속성은 (파일 종류, 경로)로 묶은 것이다. 같은 경로가 여러 파일에 있으면 한 행이다. 유닛 하나의 실제 값은 `data/units-*.jsonl.gz`에 있다.

## 한계

- **값을 바꿨을 때 게임이 어떻게 반응하는지는 확인하지 않았다.** 이 문서는 값이 어디에 있는지만 알려 준다.
- **뜻을 아는 속성이 적다.** 연관 파일(`.inc`, `.ext`, `.presets`)에는 이름만으로 뜻을 붙이지 않았다. 같은 이름이 파일마다 다른 뜻으로 쓰이기 때문이다.
- **`##`로 시작하는 이름**(예: `##bone`)은 꺼 둔 블록으로 보인다. 엔진의 처리 방식은 확인하지 못했다.
- **편제·비용·게임 규칙은 여기에 없다.** `set/multiplayer`, `set/dynamic_campaign` 등 유닛 파일이 아닌 설정은 [../04-units/schema-shared.md](../04-units/schema-shared.md)에 있다.
- 기준은 바닐라 1.0.6.7이다. 모드가 추가한 속성은 [../05-workshop-mods/README.md](../05-workshop-mods/README.md)에 있다.
'@)
[IO.File]::WriteAllText((Join-Path $OutDir 'README.md'), $sb.ToString(), $utf8)
Write-Host "README.md sections=$($summary.Count)"
