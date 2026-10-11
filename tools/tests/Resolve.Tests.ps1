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
Assert-Equal $true (@($r.Unresolved | Where-Object { $_ -like 'include-depth:*' }).Count -ge 1) 'cycle reported'
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

Complete-Tests
