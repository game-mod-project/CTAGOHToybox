Set-StrictMode -Version 3.0

$script:SdlMaxDepth = 32
$script:SdlParamInString = [regex]::new('%(\w+)')

function New-SdlResolveContext {
    param([Parameter(Mandatory)][string[]]$WorkDirs)
    [pscustomobject]@{
        WorkDirs = $WorkDirs; Cache = @{}; Defines = @{}
        Unresolved = [System.Collections.Generic.List[string]]::new()
        Includes = [System.Collections.Generic.List[string]]::new()
        Macros = [System.Collections.Generic.List[string]]::new()
    }
}

function Find-SdlFile {
    param($Ctx, [string]$VirtualPath)
    foreach ($d in $Ctx.WorkDirs) {
        $full = Join-Path $d ($VirtualPath -replace '/', '\')
        if ([IO.File]::Exists($full)) { return $full }
    }
    $null
}

function Get-SdlParsedFile {
    param($Ctx, [string]$VirtualPath, [bool]$Cache = $true)
    if ($Ctx.Cache.ContainsKey($VirtualPath)) { return $Ctx.Cache[$VirtualPath] }
    $full = Find-SdlFile $Ctx $VirtualPath
    $root = if ($null -ne $full) { (ConvertFrom-Sdl -Text (Read-SdlText $full)).Root } else { $null }
    if ($Cache) { $Ctx.Cache[$VirtualPath] = $root }
    $root
}

function Resolve-SdlIncludePath {
    param([string]$Raw, [string]$FromDir)
    $p = $Raw.Trim('"') -replace '\\', '/'
    $joined = if ($p.StartsWith('/')) { $p.TrimStart('/') } elseif ($FromDir) { "$FromDir/$p" } else { $p }
    $parts = [System.Collections.Generic.List[string]]::new()
    foreach ($s in $joined.Split('/')) {
        if ($s -eq '..') { if ($parts.Count -gt 0) { $parts.RemoveAt($parts.Count - 1) } }
        elseif ($s -and $s -ne '.') { $parts.Add($s) }
    }
    ($parts -join '/').ToLowerInvariant()
}

function Get-SdlDir {
    param([string]$VirtualPath)
    $i = $VirtualPath.LastIndexOf('/')
    if ($i -lt 0) { '' } else { $VirtualPath.Substring(0, $i) }
}

# 두 피연산자의 전위 산술식을 계산한다. 계산할 수 없으면 $null
function Invoke-SdlExpr {
    param($Node)
    $k = $Node.Children
    if ($k.Count -ne 3) { return $null }
    $vals = [double[]]::new(2)
    for ($i = 0; $i -lt 2; $i++) {
        $o = $k[$i + 1]
        if ($o.Kind -eq 'atom') {
            $d = 0.0
            if (-not [double]::TryParse($o.Text, [Globalization.NumberStyles]::Float, [cultureinfo]::InvariantCulture, [ref]$d)) { return $null }
            $vals[$i] = $d
        }
        elseif (Test-SdlExpr $o) {
            $r = Invoke-SdlExpr $o
            if ($null -eq $r) { return $null }
            $vals[$i] = $r
        }
        else { return $null }
    }
    $op = $k[0].Text
    if ($op -eq '+') { return $vals[0] + $vals[1] }
    if ($op -eq '-') { return $vals[0] - $vals[1] }
    if ($op -eq '*') { return $vals[0] * $vals[1] }
    if ($vals[1] -eq 0) { return $null }
    $vals[0] / $vals[1]
}

function Expand-SdlNodes {
    param($Nodes, [string]$Dir, [hashtable]$Bind, $Ctx, [int]$Depth)
    $out = [System.Collections.Generic.List[object]]::new()
    foreach ($n in $Nodes) {
        if ($n.Kind -eq 'atom') {
            if ($n.Text.StartsWith('%') -and $n.Text.Length -gt 1) {
                $key = $n.Text.Substring(1)
                if ($Bind.ContainsKey($key)) { foreach ($b in $Bind[$key]) { $out.Add($b) } }
                else { $Ctx.Unresolved.Add("param:$($n.Text)"); $out.Add($n) }
            }
            else { $out.Add($n) }
            continue
        }
        if ($n.Kind -eq 'str') {
            if ($n.Text.Contains('%')) {
                $text = $script:SdlParamInString.Replace($n.Text, {
                    param($m)
                    $key = $m.Groups[1].Value
                    if ($Bind.ContainsKey($key)) { (Get-SdlNodesText $Bind[$key]).Trim('"') } else { $m.Value }
                })
                $out.Add((New-SdlLeaf 'str' $text $n.Index))
            }
            else { $out.Add($n) }
            continue
        }
        if ($n.Kind -eq 'brace' -or $n.Kind -eq 'call') {
            $c = New-SdlNode $n.Kind $n.Text $n.Index
            $c.Children.AddRange((Expand-SdlNodes $n.Children $Dir $Bind $Ctx $Depth))
            $out.Add($c)
            continue
        }

        # paren
        $kids = $n.Children
        $head = if ($kids.Count -gt 0) { $kids[0] } else { $null }
        $word = if ($null -ne $head -and $head.Kind -eq 'atom') { $head.Text.ToLowerInvariant() } else { '' }

        if ($word -eq 'include' -and $kids.Count -ge 2 -and $kids[1].Kind -eq 'str') {
            $vp = Resolve-SdlIncludePath $kids[1].Text $Dir
            if ($Depth -ge $script:SdlMaxDepth) { $Ctx.Unresolved.Add("include-depth:$vp"); $out.Add($n); continue }
            $root = Get-SdlParsedFile $Ctx $vp
            if ($null -eq $root) { $Ctx.Unresolved.Add("include:$vp"); $out.Add($n); continue }
            $Ctx.Includes.Add($vp)
            $b = $Bind.Clone()
            for ($i = 2; $i -lt $kids.Count; $i++) {
                if ($kids[$i].Kind -eq 'call') { $b[$kids[$i].Text] = (Expand-SdlNodes $kids[$i].Children $Dir $Bind $Ctx $Depth) }
            }
            $out.AddRange((Expand-SdlNodes $root.Children (Get-SdlDir $vp) $b $Ctx ($Depth + 1)))
            continue
        }
        if ($word -eq 'define' -and $kids.Count -ge 2) {
            $Ctx.Defines[$kids[1].Text.Trim('"')] = [pscustomobject]@{ Body = $kids.GetRange(2, $kids.Count - 2); Dir = $Dir }
            continue
        }
        if ($null -ne $head -and $head.Kind -eq 'str') {
            $name = $head.Text.Trim('"')
            if ($Depth -ge $script:SdlMaxDepth -or -not $Ctx.Defines.ContainsKey($name)) { $Ctx.Unresolved.Add("macro:$name"); $out.Add($n); continue }
            $Ctx.Macros.Add($name)
            $b = $Bind.Clone()
            $pos = -1
            for ($i = 1; $i -lt $kids.Count; $i++) {
                $k = $kids[$i]
                if ($k.Kind -eq 'call') { $b[$k.Text] = (Expand-SdlNodes $k.Children $Dir $Bind $Ctx $Depth) }
                elseif ($pos -lt 0 -and $k.Kind -eq 'atom' -and $k.Text -ieq 'args') { $pos = 0 }
                elseif ($pos -ge 0) { $b["$pos"] = (Expand-SdlNodes @($k) $Dir $Bind $Ctx $Depth); $pos++ }
            }
            $def = $Ctx.Defines[$name]
            $out.AddRange((Expand-SdlNodes $def.Body $def.Dir $b $Ctx ($Depth + 1)))
            continue
        }

        $c = New-SdlNode 'paren' '' $n.Index
        $c.Children.AddRange((Expand-SdlNodes $kids $Dir $Bind $Ctx $Depth))
        if (Test-SdlExpr $c) {
            $v = Invoke-SdlExpr $c
            if ($null -ne $v) { $out.Add((New-SdlLeaf 'atom' ($v.ToString('0.######', [cultureinfo]::InvariantCulture)) $n.Index)); continue }
        }
        $out.Add($c)
    }
    , $out
}

function Resolve-SdlFile {
    param($Ctx, [Parameter(Mandatory)][string]$VirtualPath)
    $Ctx.Defines = @{}
    $Ctx.Unresolved = [System.Collections.Generic.List[string]]::new()
    $Ctx.Includes = [System.Collections.Generic.List[string]]::new()
    $Ctx.Macros = [System.Collections.Generic.List[string]]::new()
    $presets = [System.Collections.Generic.List[string]]::new()

    $dir = Get-SdlDir $VirtualPath
    $acc = ''
    foreach ($seg in $dir.Split('/')) {
        if (-not $seg) { continue }
        $acc = if ($acc) { "$acc/$seg" } else { $seg }
        $pv = "$acc/.presets"
        if ($pv -eq $VirtualPath) { continue }
        $proot = Get-SdlParsedFile $Ctx $pv
        if ($null -ne $proot) {
            $presets.Add($pv)
            [void](Expand-SdlNodes $proot.Children $acc @{} $Ctx 1)
        }
    }
    # .presets를 읽으며 쌓인 include/매크로 기록은 파일 자체의 것이 아니므로 비운다
    $Ctx.Includes.Clear(); $Ctx.Macros.Clear()

    $root = Get-SdlParsedFile $Ctx $VirtualPath $false
    # 빈 목록이 파이프라인에서 $null로 풀리지 않도록 대입문으로 나눈다
    $nodes = [System.Collections.Generic.List[object]]::new()
    if ($null -ne $root) { $nodes = Expand-SdlNodes $root.Children $dir @{} $Ctx 0 }
    [pscustomobject]@{
        Nodes = $nodes
        Unresolved = @($Ctx.Unresolved | Sort-Object -Unique)
        Includes = @($Ctx.Includes | Sort-Object -Unique)
        Macros = @($Ctx.Macros | Sort-Object -Unique)
        Presets = @($presets)
    }
}
