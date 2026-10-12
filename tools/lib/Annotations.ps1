# 키 의미 주석(data/annotations)의 읽기와 조회. 스키마 문서와 속성 문서가 같은 규칙을 쓴다.

$script:AnnotationSourceNames = @{ C = '게임 파일 주석'; S = '파일 구조와 값에서 추론' }

# common.psv(경로|형태|등급|의미|출처)와, 있으면 그룹 전용 <그룹>.csv를 읽는다
function Read-Annotations {
    param([Parameter(Mandatory)][string]$AnnotationDir, [string]$Group)
    $ann = @{}
    $commonPath = Join-Path $AnnotationDir 'common.psv'
    if (Test-Path -LiteralPath $commonPath) {
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

# 주석 조회. 정확한 경로가 우선하고, $AllowWildcard일 때만 */이름 주석을 쓴다.
# 와일드카드는 유닛 파일과 펼친 키에만 허용한다: 공용 설정이나 모델 파일에는 같은 이름이 다른 뜻으로 쓰인다.
function Get-Annotation([hashtable]$Ann, [string]$Kind, [string]$Path, [string]$Form, [bool]$AllowWildcard) {
    $last = $Path.Substring($Path.LastIndexOf('/') + 1)
    $keys = @("$Kind`t$Path`t$Form", "*`t$Path`t$Form")
    if ($AllowWildcard) { $keys += @("$Kind`t*/$last`t$Form", "*`t*/$last`t$Form") }
    foreach ($k in $keys) { if ($Ann.ContainsKey($k)) { return $Ann[$k] } }
    [pscustomobject]@{ Grade = '미확인(자동)'; Meaning = ''; Source = '' }
}

# */이름 주석을 허용하는 파일 종류(유닛 파일)
$script:UnitFileKinds = '.def', '.set', '(no-ext)', '.ammo', '.weapon', '.grenade', '.mine', '.pattern'
