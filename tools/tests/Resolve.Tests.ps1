. "$PSScriptRoot/TestKit.ps1"
. "$PSScriptRoot/../lib/Common.ps1"
. "$PSScriptRoot/../lib/SdlTokenizer.ps1"
. "$PSScriptRoot/../lib/SdlParser.ps1"
. "$PSScriptRoot/../lib/SdlKeys.ps1"
. "$PSScriptRoot/../lib/SdlResolve.ps1"

function Put([string]$root, [string]$vp, [string]$text) {
    $p = Join-Path $root ($vp -replace '/', '\')
    [IO.Directory]::CreateDirectory((Split-Path -Parent $p)) | Out-Null
    [IO.File]::WriteAllText($p, $text)
}

Assert-Equal 'properties/cannon.ext' (Resolve-SdlIncludePath '"/properties/cannon.ext"' 'entity/-vehicle/t') 'absolute include'
Assert-Equal 'set/blast.inc' (Resolve-SdlIncludePath '"\set\blast.inc"' 'set/stuff/gun') 'backslash include'
Assert-Equal 'properties/vehicle.ext' (Resolve-SdlIncludePath '"vehicle.ext"' 'properties') 'relative include'
Assert-Equal 'set/x.inc' (Resolve-SdlIncludePath '"../x.inc"' 'set/stuff') 'parent include'

$w = New-TestDir
Put $w 'properties/base.ext' '{props "p"} (define "wheel" {volume %0 {thickness 0.5} {component %0}})'
Put $w 'entity/-vehicle/t/t.def' '{game_entity (include "/properties/base.ext") ("wheel" args "w1") (include "missing.inc") (mod "mp" {limit 5})}'
Put $w 'set/stuff/.presets' '(define "outer" {o 1})'
Put $w 'set/stuff/gun/.presets' '(define "fire rate" {recoveryTime (/ 60 %rpm)}) (define "nested" ("fire rate" rpm(%r)) {tag "n"})'
Put $w 'set/stuff/gun/g1' '{from "pattern gun" ("fire rate" rpm(240)) ("nested" r(120)) ("outer") ("nope") {x %unbound}}'
Put $w 'set/a.inc' '(include "b.inc")'
Put $w 'set/b.inc' '(include "a.inc") {x 1}'
Put $w 'set/scaled.inc' '{size %scale}'
Put $w 'set/uses.set' '(include "scaled.inc" scale(2.5))'

$ctx = New-SdlResolveContext -WorkDirs @($w)

$r = Resolve-SdlFile $ctx 'entity/-vehicle/t/t.def'
Assert-Equal '{ game_entity { props "p" } { volume "w1" { thickness 0.5 } { component "w1" } } ( include "missing.inc" ) ( mod "mp" { limit 5 } ) }' (Get-SdlNodesText $r.Nodes) 'include and positional macro'
Assert-Equal @('include:entity/-vehicle/t/missing.inc') @($r.Unresolved) 'missing include reported'
Assert-Equal @('properties/base.ext') @($r.Includes) 'include list'
Assert-Equal @('wheel') @($r.Macros) 'macro list'

$r = Resolve-SdlFile $ctx 'set/stuff/gun/g1'
Assert-Equal '{ from "pattern gun" { recoveryTime 0.25 } { recoveryTime 0.5 } { tag "n" } { o 1 } ( "nope" ) { x %unbound } }' (Get-SdlNodesText $r.Nodes) 'named macro, nested macro, arithmetic, presets'
Assert-Equal @('macro:nope', 'param:%unbound') @($r.Unresolved | Sort-Object) 'unknown macro and unbound param reported'
Assert-Equal @('set/stuff/.presets', 'set/stuff/gun/.presets') @($r.Presets) 'presets outer first'

$r = Resolve-SdlFile $ctx 'set/uses.set'
Assert-Equal '{ size 2.5 }' (Get-SdlNodesText $r.Nodes) 'include arguments bind params'

# Review Focus 4: 순환 include는 끝나고 보고된다
$r = Resolve-SdlFile $ctx 'set/a.inc'
Assert-Equal $true (@($r.Unresolved | Where-Object { $_ -like 'include-cycle:*' }).Count -ge 1) 'cycle reported'
Assert-Equal $true ((Get-SdlNodesText $r.Nodes).Contains('{ x 1 }')) 'cycle still yields content'

# 정의는 파일 사이에 새지 않는다
$r = Resolve-SdlFile $ctx 'set/uses.set'
Assert-Equal 0 @($r.Macros).Count 'context is reset per file'

# 여러 작업 폴더: 앞이 우선
$w2 = New-TestDir
Put $w2 'properties/base.ext' '{props "override"}'
$ctx2 = New-SdlResolveContext -WorkDirs @($w2, $w)
$r = Resolve-SdlFile $ctx2 'entity/-vehicle/t/t.def'
Assert-Equal $true ((Get-SdlNodesText $r.Nodes).Contains('{ props "override" }')) 'first work dir wins'
Assert-Equal $true (@($r.Unresolved) -contains 'macro:wheel') 'override without define leaves macro unresolved'

# 리뷰 I1: 문자열과 낱말 안의 %매개변수도 치환하고, 채워지지 않으면 보고한다
Put $w 'set/p.set' '(define "m" {tex "skin_%color"} {name "%0_left"} {v pre%0} {u "%nope_x"} {pct "100%"}) ("m" color(red) args foo)'
$r = Resolve-SdlFile $ctx 'set/p.set'
Assert-Equal '{ tex "skin_red" } { name "foo_left" } { v prefoo } { u "%nope_x" } { pct "100%" }' (Get-SdlNodesText $r.Nodes) 'params inside strings and words use the longest bound name'
Assert-Equal @('param:%nope_x') @($r.Unresolved) 'unbound param inside a string is reported'

# 이름 있는 매개변수는 낱말 전체가 같아야 한다: %door가 묶여 있어도 %door2는 다른 매개변수다.
# 숫자로 시작하는 위치 인자만 뒤에 글자가 붙을 수 있다("%0_left")
Put $w 'set/q.set' '(define "m" {d %door2} {s "%door2"} {ok "%door"}) ("m" door("left"))'
$r = Resolve-SdlFile $ctx 'set/q.set'
Assert-Equal '{ d %door2 } { s "%door2" } { ok "left" }' (Get-SdlNodesText $r.Nodes) 'a named param is not matched by its prefix'
Assert-Equal @('param:%door2') @($r.Unresolved) 'prefix-only match is reported as unbound'

# 리뷰 I2: 작업 폴더 밖을 가리키는 include는 읽지 않는다
Put $w 'set/abs.set' '(include "/c:/windows/win.ini") {k 1}'
$r = Resolve-SdlFile $ctx 'set/abs.set'
Assert-Equal @('include-unsafe:c:/windows/win.ini') @($r.Unresolved) 'include with a drive path is refused'
Assert-Equal '( include "/c:/windows/win.ini" ) { k 1 }' (Get-SdlNodesText $r.Nodes) 'refused include stays as written'

# 리뷰 I6: 서로를 두 번씩 include해도 바로 끝난다
Put $w 'set/x.inc' '(include "y.inc") (include "y.inc")'
Put $w 'set/y.inc' '(include "x.inc") (include "x.inc") {y 1}'
$sw = [Diagnostics.Stopwatch]::StartNew()
$r = Resolve-SdlFile $ctx 'set/x.inc'
Assert-Equal $true ($sw.Elapsed.TotalSeconds -lt 5) 'fan-out include cycle terminates quickly'
Assert-Equal @('include-cycle:set/x.inc') @($r.Unresolved) 'fan-out include cycle is reported once'
Assert-Equal '{ y 1 } { y 1 }' ((Get-SdlNodesText $r.Nodes) -replace '\( include "x\.inc" \) ', '') 'content outside the cycle is kept'
Put $w 'set/rec.set' '(define "a" ("a") ("a") {z 1}) ("a")'
$r = Resolve-SdlFile $ctx 'set/rec.set'
Assert-Equal @('macro-cycle:a') @($r.Unresolved) 'self-recursive macro is reported and terminates'

Complete-Tests
