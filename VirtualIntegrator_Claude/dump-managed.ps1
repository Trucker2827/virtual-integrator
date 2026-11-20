param(
    [string]$BinDir = 'C:\Program Files (x86)\Circon\BIN',
    [string]$OutFile = 'C:\Users\shackleford\Desktop\VisualIntegrator-ILSpy.md'
)

$ErrorActionPreference = 'Continue'

# Preload all Circon managed DLLs into ReflectionOnly context so cross-references resolve.
$files = Get-ChildItem $BinDir -File | Where-Object { $_.Extension -in '.dll','.exe' }
$managedFiles = @()
foreach ($f in $files) {
    try {
        [void][System.Reflection.AssemblyName]::GetAssemblyName($f.FullName)
        $managedFiles += $f
    } catch { }
}

# Pre-resolve handler
$resolver = {
    param($s, $e)
    try { return [System.Reflection.Assembly]::ReflectionOnlyLoad($e.Name) } catch { }
    $simple = ($e.Name -split ',')[0]
    $candidate = Join-Path $BinDir "$simple.dll"
    if (Test-Path $candidate) { return [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($candidate) }
    $candidate = Join-Path $BinDir "$simple.exe"
    if (Test-Path $candidate) { return [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($candidate) }
    return $null
}
[System.AppDomain]::CurrentDomain.add_ReflectionOnlyAssemblyResolve($resolver)

function Format-Type {
    param([System.Type]$t)
    if ($null -eq $t) { return '?' }
    if ($t.IsGenericType) {
        $base = ($t.Name -split '`')[0]
        $args = $t.GetGenericArguments() | ForEach-Object { Format-Type $_ }
        return "$base<$($args -join ', ')>"
    }
    return $t.Name
}

function Get-TypeKind {
    param([System.Type]$t)
    if ($t.IsInterface) { return 'interface' }
    if ($t.IsEnum) { return 'enum' }
    if ($t.IsValueType) { return 'struct' }
    if ($t.IsAbstract -and $t.IsSealed) { return 'static class' }
    if ($t.IsAbstract) { return 'abstract class' }
    if ($t.IsSealed) { return 'sealed class' }
    return 'class'
}

function Format-Method {
    param([System.Reflection.MethodBase]$m)
    $acc = if ($m.IsPublic) { 'public' } elseif ($m.IsFamily) { 'protected' } elseif ($m.IsAssembly) { 'internal' } else { 'private' }
    $static = if ($m.IsStatic) { ' static' } else { '' }
    $virtual = if ($m.IsVirtual -and -not $m.IsFinal) { ' virtual' } else { '' }
    $ps = $m.GetParameters() | ForEach-Object { "$(Format-Type $_.ParameterType) $($_.Name)" }
    if ($m -is [System.Reflection.ConstructorInfo]) {
        return "{0}{1} .ctor({2})" -f $acc, $static, ($ps -join ', ')
    }
    $ret = Format-Type $m.ReturnType
    return "{0}{1}{2} {3} {4}({5})" -f $acc, $static, $virtual, $ret, $m.Name, ($ps -join ', ')
}

function Dump-Assembly {
    param([string]$Path, [System.IO.TextWriter]$W)

    $W.WriteLine()
    $W.WriteLine('---')
    $W.WriteLine()
    $W.WriteLine("## $([System.IO.Path]::GetFileName($Path))")
    $W.WriteLine()
    $asm = [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($Path)
    $aname = $asm.GetName()
    $W.WriteLine("- Full name: ``$($aname.FullName)``")
    $W.WriteLine("- Processor architecture: $($aname.ProcessorArchitecture)")

    try {
        $attrs = [System.Reflection.CustomAttributeData]::GetCustomAttributes($asm)
        $tfm = $attrs | Where-Object { $_.AttributeType.Name -eq 'TargetFrameworkAttribute' } | Select-Object -First 1
        if ($tfm) { $W.WriteLine("- Target framework: $($tfm.ConstructorArguments[0].Value)") }
        $comp = $attrs | Where-Object { $_.AttributeType.Name -eq 'AssemblyCompanyAttribute' } | Select-Object -First 1
        if ($comp) { $W.WriteLine("- Company: $($comp.ConstructorArguments[0].Value)") }
        $prod = $attrs | Where-Object { $_.AttributeType.Name -eq 'AssemblyProductAttribute' } | Select-Object -First 1
        if ($prod) { $W.WriteLine("- Product: $($prod.ConstructorArguments[0].Value)") }
        $cvis = $attrs | Where-Object { $_.AttributeType.Name -eq 'ComVisibleAttribute' } | Select-Object -First 1
        if ($cvis) { $W.WriteLine("- ComVisible: $($cvis.ConstructorArguments[0].Value)") }
        $guid = $attrs | Where-Object { $_.AttributeType.Name -eq 'GuidAttribute' } | Select-Object -First 1
        if ($guid) { $W.WriteLine("- Assembly GUID: $($guid.ConstructorArguments[0].Value)") }
    } catch { }

    $W.WriteLine()
    $W.WriteLine("### Referenced assemblies")
    $W.WriteLine()
    $asm.GetReferencedAssemblies() | Sort-Object Name | ForEach-Object {
        $W.WriteLine("- $($_.Name) $($_.Version)")
    }

    $resNames = $asm.GetManifestResourceNames()
    if ($resNames.Count -gt 0) {
        $W.WriteLine()
        $W.WriteLine("### Embedded resources ($($resNames.Count))")
        $W.WriteLine()
        foreach ($rn in $resNames) {
            try {
                $info = $asm.GetManifestResourceInfo($rn)
                $W.WriteLine("- ``$rn`` [$($info.ResourceLocation)]")
            } catch {
                $W.WriteLine("- ``$rn``")
            }
        }
    }

    try {
        $types = $asm.GetTypes()
    } catch [System.Reflection.ReflectionTypeLoadException] {
        $types = $_.Exception.Types | Where-Object { $_ -ne $null }
    }

    $nsGroups = $types | Where-Object { $_ -ne $null -and -not $_.FullName.StartsWith('<') } | Group-Object Namespace | Sort-Object Name

    $W.WriteLine()
    $W.WriteLine("### Types by namespace ($($types.Count) loaded)")
    $W.WriteLine()
    foreach ($g in $nsGroups) {
        $ns = if ([string]::IsNullOrEmpty($g.Name)) { '(global)' } else { $g.Name }
        $W.WriteLine("- **$ns** - $($g.Count) types")
    }

    $W.WriteLine()
    $W.WriteLine("### Type detail")
    $W.WriteLine()
    foreach ($g in $nsGroups) {
        $ns = if ([string]::IsNullOrEmpty($g.Name)) { '(global)' } else { $g.Name }
        $W.WriteLine("#### namespace $ns")
        $W.WriteLine()
        foreach ($t in ($g.Group | Sort-Object Name)) {
            if ($t.Name -match '^<') { continue }
            $kind = Get-TypeKind $t
            $base = ''
            if ($t.BaseType -and $t.BaseType.Name -ne 'Object' -and -not $t.IsInterface) { $base = " : $(Format-Type $t.BaseType)" }
            $ifaces = @()
            try { $ifaces = $t.GetInterfaces() | ForEach-Object { Format-Type $_ } } catch { }
            if ($ifaces.Count -gt 0) {
                $sep = if ($base) { ', ' } else { ' : ' }
                $base = "$base$sep$($ifaces -join ', ')"
            }
            $W.WriteLine("- **$kind $($t.Name)$base**")
            if ($t.IsEnum) {
                foreach ($f in $t.GetFields([System.Reflection.BindingFlags]'Public,Static')) {
                    try { $v = $f.GetRawConstantValue(); $W.WriteLine("  - $($f.Name) = $v") } catch { $W.WriteLine("  - $($f.Name)") }
                }
                continue
            }
            $flags = [System.Reflection.BindingFlags]'Public,NonPublic,Instance,Static,DeclaredOnly'
            $ctors = $t.GetConstructors($flags) | Sort-Object { $_.GetParameters().Length }
            foreach ($c in $ctors) { $W.WriteLine("  - $(Format-Method $c)") }
            $methods = $t.GetMethods($flags) | Where-Object { -not $_.IsSpecialName -and -not $_.Name.StartsWith('<') } | Sort-Object Name
            foreach ($m in $methods) { $W.WriteLine("  - $(Format-Method $m)") }
            $props = $t.GetProperties($flags) | Sort-Object Name
            foreach ($p in $props) {
                $acc = @()
                if ($p.CanRead) { $acc += 'get' }
                if ($p.CanWrite) { $acc += 'set' }
                $W.WriteLine("  - property $(Format-Type $p.PropertyType) $($p.Name) { $($acc -join '; ') }")
            }
            $fields = $t.GetFields($flags) | Where-Object { -not $_.Name.StartsWith('<') } | Sort-Object Name
            foreach ($fl in $fields) {
                $acc = if ($fl.IsPublic) { 'public' } elseif ($fl.IsFamily) { 'protected' } elseif ($fl.IsAssembly) { 'internal' } else { 'private' }
                $stat = if ($fl.IsStatic) { ' static' } else { '' }
                $W.WriteLine("  - $acc$stat field $(Format-Type $fl.FieldType) $($fl.Name)")
            }
            $events = $t.GetEvents($flags) | Sort-Object Name
            foreach ($e in $events) { $W.WriteLine("  - event $(Format-Type $e.EventHandlerType) $($e.Name)") }
            $W.WriteLine()
        }
    }
}

$targetList = @(
    'vilns32.exe',
    'nilns32.exe',
    'ailns32.exe',
    'CirconUtil.dll',
    'CirconService.dll',
    'CirconData.dll',
    'CirconDataForms.dll',
    'CirconDataLinqForms.dll',
    'CirconForms.dll',
    'CirconLns.dll',
    'CirconLnsDevices.dll',
    'CirconLnsDevicesForms.dll',
    'CirconLnsForms.dll',
    'CirconLonWorks.dll',
    'CirconNvt.dll',
    'CirconLdrf.dll',
    'CirconLinq.dll',
    'CirconGfxForms.dll',
    'CirconOpcServer.dll',
    'CirconDeviceInfo.dll',
    'AlarmView.dll',
    'EventTranslate.dll',
    'LnsChangeNet.dll',
    'CirconSiteService.exe',
    'CirconOpcService.exe',
    'CirconGfxEditor.exe',
    'CirconGfxProject.exe',
    'CirconBrowser.exe',
    'CirconCatalogManager.exe',
    'CirconOpenLns.exe',
    'CirconWatchdog.exe'
)

$writer = [System.IO.StreamWriter]::new($OutFile, $false, [System.Text.UTF8Encoding]::new($false))
try {
    $writer.WriteLine("# Visual Integrator - Managed Assembly Decompile (Reflection-Based)")
    $writer.WriteLine()
    $writer.WriteLine("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
    $writer.WriteLine("Source: ``$BinDir``")
    $writer.WriteLine()
    $writer.WriteLine("ILSpy-style surface: namespaces, types, members, attributes, embedded resources, references.")
    $writer.WriteLine("Generated by System.Reflection ReflectionOnly load - no code is executed.")

    foreach ($name in $targetList) {
        $path = Join-Path $BinDir $name
        if (-not (Test-Path $path)) { continue }
        try {
            Dump-Assembly -Path $path -W $writer
            Write-Host "Dumped $name"
        } catch {
            $writer.WriteLine("### $name - error")
            $writer.WriteLine()
            $writer.WriteLine('``````')
            $writer.WriteLine($_.Exception.Message)
            $writer.WriteLine('``````')
            Write-Host "Failed $name : $($_.Exception.Message)"
        }
    }
} finally {
    $writer.Close()
}

$final = Get-Item $OutFile
"OUTFILE: $OutFile"
"SIZE_BYTES: $($final.Length)"
"LINES: " + ((Get-Content $OutFile | Measure-Object).Count)
