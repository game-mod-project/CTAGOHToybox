# 키 의미 주석(data/annotations)의 읽기와 조회. 스키마 문서와 속성 문서가 같은 규칙을 쓴다.

$script:AnnotationSourceNames = @{ C = '게임 파일 주석'; S = '파일 구조와 값에서 추론'
    D = 'GEM RTS 엔진 공식 문서 docs.bestway.com.ua (Men of War II 기준, 2026-10-12 확인)' }

# common.psv(경로|형태|등급|의미|출처)와, 있으면 그룹 전용 <그룹>.psv, <그룹>.csv를 읽는다
function Read-Annotations {
    param([Parameter(Mandatory)][string]$AnnotationDir, [string]$Group)
    $ann = @{}
    # <그룹>.psv는 common.psv와 같은 형식이고 그 그룹에만 적용된다. 같은 키면 그룹 쪽이 이긴다
    $psvPaths = @(Join-Path $AnnotationDir 'common.psv')
    if ($Group) { $psvPaths += Join-Path $AnnotationDir "$Group.psv" }
    foreach ($commonPath in $psvPaths) {
        if (-not (Test-Path -LiteralPath $commonPath)) { continue }
        foreach ($line in [IO.File]::ReadLines($commonPath)) {
            if (-not $line.Trim() -or $line.StartsWith('#')) { continue }
            $p = $line.Split('|', 5)
            if ($p.Count -lt 5) { throw "주석 줄 형식 오류: $line" }
            $src = if ($script:AnnotationSourceNames.ContainsKey($p[4])) { $script:AnnotationSourceNames[$p[4]] } else { $p[4] }
            $ann["*`t$($p[0])`t$($p[1])"] = [pscustomobject]@{ Grade = $p[2]; Meaning = $p[3]; Source = $src }
        }
    }
    if ($Group) {
        $annPath = Join-Path $AnnotationDir "$Group.csv"
        if (Test-Path -LiteralPath $annPath) {
            foreach ($a in (Import-Csv -LiteralPath $annPath)) { $ann["$($a.FileKind)`t$($a.Path)`t$($a.Form)"] = $a }
        }
    }
    $ann
}

# 경로에서 값의 자리를 가리키지 않는 조각을 뺀다: 매크로 정의 (define "이름"), 매크로 호출 ("이름"),
# 호출 인자 이름(), 꺼 둔 블록 표시 ##. 남는 것은 블록 이름의 나열이다.
function ConvertTo-PlainKeyPath([string]$Path) {
    $parts = foreach ($s in $Path.Split('/')) {
        if ($s.StartsWith('(') -or $s.EndsWith('()')) { continue }
        $s = $s.TrimStart('#')
        if ($s) { $s }
    }
    @($parts)
}

# 주석 조회. 우선순위: 정확한 경로 > **/꼬리 (긴 것부터) > */이름.
#   **/a/b  : 경로의 끝이 a/b인 모든 키. include되는 파일과 매크로 정의 안의 키에도 적용된다.
#   */이름  : 마지막 조각이 그 이름인 키. $AllowWildcard일 때만 쓴다.
# */이름은 유닛 파일과 펼친 키에만 허용한다: 공용 설정이나 모델 파일에는 같은 이름이 다른 뜻으로 쓰인다.
function Get-Annotation([hashtable]$Ann, [string]$Kind, [string]$Path, [string]$Form, [bool]$AllowWildcard) {
    $last = $Path.Substring($Path.LastIndexOf('/') + 1)
    $keys = @("$Kind`t$Path`t$Form", "*`t$Path`t$Form")
    foreach ($k in $keys) { if ($Ann.ContainsKey($k)) { return $Ann[$k] } }
    $plain = @(ConvertTo-PlainKeyPath $Path)
    for ($i = 0; $i -lt $plain.Count; $i++) {
        $k = "*`t**/$($plain[$i..($plain.Count - 1)] -join '/')`t$Form"
        if ($Ann.ContainsKey($k)) { return $Ann[$k] }
    }
    if ($AllowWildcard) {
        foreach ($k in @("$Kind`t*/$last`t$Form", "*`t*/$last`t$Form")) { if ($Ann.ContainsKey($k)) { return $Ann[$k] } }
    }
    [pscustomobject]@{ Grade = '미확인(자동)'; Meaning = ''; Source = '' }
}

# */이름 주석을 허용하는 파일 종류(유닛 파일)
$script:UnitFileKinds = '.def', '.set', '(no-ext)', '.ammo', '.weapon', '.grenade', '.mine', '.pattern'
