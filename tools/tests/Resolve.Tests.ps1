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
# E13: 조상 폴더의 .presets는 읽지 않는다(게임 확인 2026-10-16: 하위 폴더 파일에서 부르면 "define not found")
Assert-Equal '{ from "pattern gun" { recoveryTime 0.25 } { recoveryTime 0.5 } { tag "n" } ( "outer" ) ( "nope" ) { x %unbound } }' (Get-SdlNodesText $r.Nodes) 'named macro, nested macro with explicit args, arithmetic, own-folder presets'
Assert-Equal @('macro:nope', 'macro:outer', 'param:%unbound') @($r.Unresolved | Sort-Object) 'unknown macro, ancestor-only macro and unbound param reported'
Assert-Equal @('set/stuff/gun/.presets') @($r.Presets) 'only the own folder presets are read'

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

# E13: 안쪽 매크로는 바깥 호출의 인자를 받지 않는다(게임 확인 2026-10-16: 채워지지 않아 "Block terminator expected"로 꺼졌다)
Put $w 'set/inh.set' '(define "inner" {val %x %0}) (define "outer" ("inner")) ("outer" x(7) args 9)'
$r = Resolve-SdlFile $ctx 'set/inh.set'
Assert-Equal '{ val %x %0 }' (Get-SdlNodesText $r.Nodes) 'an inner macro does not see the outer call arguments'
Assert-Equal @('param:%0', 'param:%x') @($r.Unresolved | Sort-Object) 'and its params are reported unbound'
# 정의할 때 묶여 있던 값(include 인자)은 쓴다: set/multiplayer/games의 bots_standard.inc가 이 꼴이다
Put $w 'set/cap.inc' '(define "team" {cp %cp})'
Put $w 'set/cap.set' '(include "cap.inc" cp(3)) ("team")'
$r = Resolve-SdlFile $ctx 'set/cap.set'
Assert-Equal '{ cp 3 }' (Get-SdlNodesText $r.Nodes) 'a macro keeps the include arguments bound where it was defined'
Assert-Equal @('%cp') @($r.Inherited) 'use of a definition-time argument is recorded'
# 부른 쪽의 include 인자도 넘어가지 않는다
Put $w 'set/call.inc' '("free")'
Put $w 'set/call.set' '(define "free" {v %cp}) (include "call.inc" cp(4))'
$r = Resolve-SdlFile $ctx 'set/call.set'
Assert-Equal '{ v %cp }' (Get-SdlNodesText $r.Nodes) 'the caller include arguments do not reach a macro defined elsewhere'
$r = Resolve-SdlFile $ctx 'set/q.set'
Assert-Equal 0 @($r.Inherited).Count 'directly passed arguments are not inherited'

# M5: 산술 결과가 0으로 뭉개지지 않는다
Put $w 'set/num.set' '{a (/ 1 3000000)} {b (* -1 0)} {c (/ 60 240)}'
$r = Resolve-SdlFile $ctx 'set/num.set'
Assert-Equal '{ a 0.000000333333333 } { b 0 } { c 0.25 }' (Get-SdlNodesText $r.Nodes) 'small results keep precision and negative zero is plain zero'

# M6: include한 파일에 파싱 오류가 있으면 포함하는 쪽에 표시된다
Put $w 'set/brk.inc' '{a 1}}'
Put $w 'set/usesbrk.set' '(include "brk.inc")'
$r = Resolve-SdlFile $ctx 'set/usesbrk.set'
Assert-Equal @('include-parse-error:set/brk.inc') @($r.Unresolved) 'parse error in an included file is reported'
Assert-Equal '{ a 1 }' (Get-SdlNodesText $r.Nodes) 'readable part of the broken include is still used'

Complete-Tests
