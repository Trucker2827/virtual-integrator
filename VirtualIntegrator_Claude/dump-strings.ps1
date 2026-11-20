param(
    [string]$BinDir = 'C:\Program Files (x86)\Circon\BIN',
    [string]$OutFile = 'C:\Users\shackleford\Desktop\VisualIntegrator-Strings.md'
)

$ErrorActionPreference = 'Continue'

# Preload managed Circon DLLs so .resx-style resource sets can bind.
$files = Get-ChildItem $BinDir -File | Where-Object { $_.Extension -in '.dll','.exe' }

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

$writer = [System.IO.StreamWriter]::new($OutFile, $false, [System.Text.UTF8Encoding]::new($false))

function Scan-Assembly {
    param([string]$Path, [System.IO.TextWriter]$W)

    $W.WriteLine()
    $W.WriteLine("## $([System.IO.Path]::GetFileName($Path))")
    $W.WriteLine()
    $asm = [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($Path)

    # 1) String constants on literal fields
    try { $types = $asm.GetTypes() } catch [System.Reflection.ReflectionTypeLoadException] { $types = $_.Exception.Types | Where-Object { $_ -ne $null } }
    $literals = @()
    foreach ($t in $types) {
        if ($null -eq $t) { continue }
        try {
            $flags = [System.Reflection.BindingFlags]'Public,NonPublic,Static,DeclaredOnly'
            foreach ($fl in $t.GetFields($flags)) {
                if ($fl.IsLiteral -and $fl.FieldType -eq [string]) {
                    try {
                        $v = $fl.GetRawConstantValue()
                        if ($null -ne $v -and $v.Length -ge 4) { $literals += [pscustomobject]@{ Type = $t.FullName; Field = $fl.Name; Value = $v } }
                    } catch { }
                }
            }
        } catch { }
    }
    if ($literals.Count -gt 0) {
        $W.WriteLine("### String literal constants ($($literals.Count))")
        $W.WriteLine()
        $W.WriteLine('``````')
        foreach ($lit in ($literals | Sort-Object Type, Field)) {
            $v = $lit.Value -replace "`r", '\r' -replace "`n", '\n'
            if ($v.Length -gt 200) { $v = $v.Substring(0,200) + '...(truncated)' }
            $W.WriteLine("$($lit.Type).$($lit.Field) = $v")
        }
        $W.WriteLine('``````')
    }

    # 2) Raw MANIFEST BYTE scan for obvious strings (SQL / URL / path hints).
    try {
        $bytes = [System.IO.File]::ReadAllBytes($Path)
        $ascii = [System.Text.Encoding]::ASCII.GetString($bytes)
        $unicode = [System.Text.Encoding]::Unicode.GetString($bytes)
        $patterns = @(
            @{ Name = 'SQL (SELECT)';              Rx = '[A-Za-z0-9_@\[\].(), *=`''"-]{10,200}SELECT[\s\t]+[^\x00]{3,200}' },
            @{ Name = 'SQL (INSERT/UPDATE/DELETE)'; Rx = '\b(INSERT INTO|UPDATE |DELETE FROM )[^\x00]{3,200}' },
            @{ Name = 'Connection string';         Rx = '(Data Source|Server|Initial Catalog|Integrated Security|User ID|Password|Trusted_Connection)=[^\x00]{3,150}' },
            @{ Name = 'URL';                       Rx = '\bhttps?://[A-Za-z0-9._\-/:%?&=+]+' },
            @{ Name = 'File path';                 Rx = '[A-Z]:\\[A-Za-z0-9 _\-\\.]{4,200}\.(exe|dll|ocx|mdf|ldf|xml|sql|rpt|xap|xaml|cfg|ini|dat)' },
            @{ Name = 'Registry key';              Rx = 'SOFTWARE\\\\?[A-Za-z0-9_\\\-\s]{3,200}' },
            @{ Name = 'Email';                     Rx = '[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,10}' }
        )
        foreach ($p in $patterns) {
            $matches = @()
            foreach ($txt in @($ascii, $unicode)) {
                $rx = [regex]::new($p.Rx, 'IgnoreCase')
                foreach ($m in $rx.Matches($txt)) {
                    $v = $m.Value.Trim()
                    if ($v.Length -lt 6) { continue }
                    if ($matches -notcontains $v) { $matches += $v }
                }
            }
            if ($matches.Count -gt 0) {
                $W.WriteLine()
                $W.WriteLine("### $($p.Name) matches ($($matches.Count))")
                $W.WriteLine()
                $W.WriteLine('``````')
                foreach ($m in ($matches | Sort-Object -Unique)) {
                    $m2 = $m -replace "`r", '\r' -replace "`n", '\n'
                    if ($m2.Length -gt 300) { $m2 = $m2.Substring(0,300) + '...(truncated)' }
                    $W.WriteLine($m2)
                }
                $W.WriteLine('``````')
            }
        }
    } catch {
        $W.WriteLine("Byte scan failed: $($_.Exception.Message)")
    }
}

$writer.WriteLine("# Visual Integrator - String/Resource/Secret Audit")
$writer.WriteLine()
$writer.WriteLine("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$writer.WriteLine()
$writer.WriteLine("Scan over managed Circon assemblies in ``$BinDir``.")
$writer.WriteLine("Per assembly: 1) string literal constants from IL metadata, 2) regex scan of raw bytes for SQL / connection strings / URLs / paths / registry keys / emails.")

$targetList = @(
    'vilns32.exe',
    'nilns32.exe',
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
    'CirconOpcServer.dll',
    'CirconDeviceInfo.dll',
    'AlarmView.dll',
    'EventTranslate.dll',
    'CirconSiteService.exe',
    'CirconOpcService.exe',
    'CirconBrowser.exe',
    'CirconCatalogManager.exe',
    'CirconOpenLns.exe',
    'CirconWatchdog.exe'
)

foreach ($name in $targetList) {
    $path = Join-Path $BinDir $name
    if (-not (Test-Path $path)) { continue }
    try {
        Scan-Assembly -Path $path -W $writer
        Write-Host "Scanned $name"
    } catch {
        $writer.WriteLine("### $name - error")
        $writer.WriteLine()
        $writer.WriteLine('``````')
        $writer.WriteLine($_.Exception.Message)
        $writer.WriteLine('``````')
        Write-Host "Failed $name : $($_.Exception.Message)"
    }
}

$writer.Close()
$f = Get-Item $OutFile
"OUTFILE: $OutFile"
"SIZE_KB: $([math]::Round($f.Length/1KB,1))"
"LINES: " + ((Get-Content $OutFile | Measure-Object).Count)
