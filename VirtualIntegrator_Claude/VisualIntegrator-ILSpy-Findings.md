# Visual Integrator — ILSpy-Style Deep Inspection Findings

Companion summary for the two raw dumps on the Desktop:

- `VisualIntegrator-ILSpy.md` — full type / member / attribute / resource dump of every managed Circon assembly (1.8 MB, ~37k lines).
- `VisualIntegrator-Strings.md` — IL string-literal constants and raw-byte regex scan for SQL / connection strings / URLs / paths / emails.

Everything below was obtained via `System.Reflection.ReflectionOnlyLoad` + byte scanning — **no managed code was executed**.

---

## 1. Managed assembly inventory (43 confirmed as managed in `BIN\`)

| Assembly | Target | Arch | Notes |
|---|---|---|---|
| `vilns32.exe` | .NET 4.6.2 | x86 | Main Visual Integrator (6 namespaces, 111 types, 57 resx) |
| `nilns32.exe` | .NET 4.6.2 | x86 | Network Integrator persona |
| `ailns32.exe` | .NET 4.6.2 | x86 | Access Integrator persona (needs `WizardControl.dll` to fully bind) |
| `CirconUtil.dll` | — | MSIL (AnyCPU) | Core helpers, `Circon.Diagnostics`, `Circon.Util.RegistryLogon` |
| `CirconService.dll` | — | MSIL | Base service plumbing (`Circon.Service`, `.Service.Log`, `.Service.Opc`) |
| `CirconData.dll` | — | MSIL | **LINQ datasets, 26 raw SELECT queries, 19 INSERT/UPDATE/DELETE, embedded Blowfish** |
| `CirconDataForms.dll` | — | MSIL | Data-bound WinForms |
| `CirconDataLinqForms.dll` | — | MSIL | LINQ-to-SQL form bindings + report SQL |
| `CirconForms.dll` | — | MSIL | Shared WinForms controls + **TurboActivate wrapper (`wyDay.TurboActivate`)** |
| `CirconLns.dll` | — | MSIL | LNS façade (`Circon.Lns`) |
| `CirconLnsDevices.dll` | — | MSIL | Device templates + schedule LMO |
| `CirconLnsDevicesForms.dll` | — | MSIL | Device forms |
| `CirconLnsForms.dll` | — | MSIL | LNS browser / open-site dialogs |
| `CirconLonWorks.dll` | — | MSIL | LonWorks primitives |
| `CirconNvt.dll` | — | MSIL | **SNVT / UCPT / UNVT** types + event queue |
| `CirconLdrf.dll` | — | MSIL | LonMark Device Resource File parser |
| `CirconLinq.dll` | — | MSIL | **LINQ-to-SQL context + Blowfish + DB version pin** |
| `CirconGfxForms.dll` | — | MSIL | Graphics forms |
| `CirconOpcServer.dll` | — | MSIL | **OPC XML-DA server implementation (WSDL inline)** |
| `CirconDeviceInfo.dll` | — | x86 | Device info / BASIC runtime wrappers |
| `AlarmView.dll` | — | MSIL | Alarm grid control |
| `EventTranslate.dll` | — | MSIL | Alarm/event text translation |
| `LnsChangeNet.dll` | — | x86 | **C++/CLI mixed-mode** (ATL+STL namespaces present in metadata) |
| `CirconSiteService.exe` | — | x86 | Windows service host (`Circon.Service.Site`) |
| `CirconOpcService.exe` | — | x86 | OPC service (uses TurboActivate) |
| `CirconGfxEditor.exe` | — | x86 | Web Editor (bound against newer ILOG.Diagrammer.Design than shipped — load fails on cross-ref) |
| `CirconGfxProject.exe` | — | x86 | Web Project Manager (same ILOG mismatch) |
| `CirconBrowser.exe` | — | x86 | Shell browser |
| `CirconCatalogManager.exe` | — | x86 | Catalog manager |
| `CirconOpenLns.exe` | — | x86 | Global LNS Server |
| `CirconWatchdog.exe` | — | x86 | Watchdog |
| … plus 12 more `CirconXxxx.exe` tools | all .NET 4.6.2 | x86 | backup / sync / print / log helpers |

---

## 2. `vilns32.exe` — complete type map

Namespaces (6):
- `Circon` (3 types — root glue)
- `Circon.ComInterop` (helpers around the OCX layer)
- `Circon.VisualIntegrator` — **the application** (document model, views, property sheets, navigate tree)
- `Circon.VisualIntegrator.Forms` (UI forms)
- `Circon.VisualIntegrator.Properties` (settings/resources)
- `Circon.VisualIntegrator.Wizard` (new-project wizard)

Document model (core type family in `Circon.VisualIntegrator`):

```
DocumentType  (enum)
IAppDeviceDocument              -> AppDeviceDocument
IAppDeviceGraphicShadowDocument -> AppDeviceGraphicShadowDocument
AppDeviceGraphicDocument
GraphicDocument
    SubsystemGraphicDocument
    LonMarkObjectGraphicDocument
IEventProfileDocument           -> EventProfileDocument
IScheduleDocument               -> ScheduleDocument
ILinkDocument                   -> LinkDocument
IGraphicShadowDocument          -> GraphicShadowDocument
                                -> SubsystemGraphicShadowDocument
                                -> LonMarkObjectGraphicShadowDocument
ISubsystemGraphicShadowDocument
ILonMarkObjectGraphicShadowDocument
ISubsystemLonMarkObjectStatusDocument -> SubsystemLonMarkObjectStatusDocument
ILonMarkObjectDocument          -> LonMarkObjectDocument
ISubsystemDocument              -> SubsystemDocument
```

Each `IXxxDocument` is a persistable business object; the matching `XxxDocumentView` is a WinForms UI (57 `.resources` entries in the assembly — one per view / property sheet). The pairing of document and view types is the ILSpy giveaway that this is a classic doc/view MDI app.

Embedded resources include views for: `AlarmReport`, `AppDeviceBrowser`, `AppDeviceGraphic`, `AppDeviceGraphicShadowDocumentView`, `AppDeviceSelectPropertySheet`, `BlankDocument`, `ContentPanel`, `SubsystemFirmwareStatus`, `SubsystemGraphicShadow`, `LonMarkObjectGraphicShadow`, `GraphicShadowDocumentSelectPropertySheet`, plus the Wizard forms.

### Key string constants
```
NavigateTree.Default            = _default
NavigateTree.DefaultGraphic     = _default.vbd
NavigateTree.DefaultGraphicSearch = _default.*
Wizard.appDeviceTemplateBlank   = Blank Device
Wizard.lonMarkObjectTemplateBlank = Blank Functional Block
Wizard.unassignedTemplate       = <unassigned>
```
Every new project is seeded by searching for a `_default.*` graphic — matches the `BlankDoc.vbd` / `Blank *.app / .fpt / .sub / .vbd` templates we saw in `BIN\`.

### `vilns32.exe` depends on (direct references)
```
AlarmView, CirconData, CirconDataForms, CirconDataLinqForms,
CirconDeviceInfo, CirconForms, CirconLdrf, CirconLinq,
CirconLns, CirconLnsForms, CirconLonWorks, CirconNvt,
CirconService, CirconUtil, EventTranslate
Echelon.LNS.Interop 3.25
SandDock 3.0.6.1, Edanmo.OleStorage 1.0
AxInterop + Interop wrappers for Browser, CIRCONDBUI, CIRCONNAVIGATE,
    CirconLNS_Data/Navigate/UI, EventLog, GRAPHIC, LOGCONFIG, REPORTS,
    SCHEDULEUI, SHDocVw, TRENDLOG, TeeChart, CirconSession
System, System.Core, System.Data.DataSetExtensions, System.Data.Linq,
System.Drawing, System.ServiceProcess, System.Windows.Forms, System.Xml
```

---

## 3. Data layer — `CirconData.dll` + `CirconLinq.dll`

### 3.1 Database version pin
```
Circon.Data.Linq.CirconDatabase.CurrentVersion = 2.88
```
Note: `SHARED\CirconDb.sql` is schema v2.87. The **active app runs against 2.88** — i.e. `Upgrade.sql` has been applied past the ship script. Any backup/restore has to run the upgrade chain.

### 3.2 Hard-coded connection strings
Found *in* the binaries themselves (not just the Web.config):
```
Data Source=(local)\CIRCON;Initial Catalog=circon;Integrated Security=True   -- CirconLinq.dll (prod template)
Data Source=TAS64\CIRCON;Initial Catalog=circon;Integrated Security=True     -- CirconData.dll (stale dev-box fossil)
```
The `TAS64\CIRCON` string is a build-machine leftover — not a runtime connection, but it tells you an original dev host was named `TAS64`.

### 3.3 Embedded Blowfish
Both `CirconData.dll` and `CirconLinq.dll` ship a `Blowfish_NET` namespace. In combination with the schema having a `password` column on `users` holding an `@md5hash_password` **plus** a separate encrypted field pattern in the queries, we can infer the layered scheme used in classic Circon:

- User login passwords hashed with **MD5** at authentication time (query: `password=@md5hash_password`) — ***weak by today's standards; MD5 is broken for password storage***.
- Sensitive fields (likely LNS credentials, email account passwords for `AlarmEmail`, etc.) encrypted with Blowfish using a key compiled in.

The MD5 auth fossil is a real security finding — any unauthenticated SQL read of `users.password` leaks hashes that are trivially crackable with modern GPU rigs.

### 3.4 26 named SELECT queries + 19 INSERT/UPDATE/DELETE in `CirconData.dll`

Representative (full list in `VisualIntegrator-Strings.md`):

- `SELECT alarms.*, device_events.*, ... FROM alarm_colors, groups_alarm_ack_codes, sessions, groups_sites, users_groups, alarm_priorities, alarm_states, alarm_categories, alarm_codes, alarms WHERE ...`
- `UPDATE alarms SET ack_user_id=sessions.user_id, ack_comment=@comment, state=1, ack_time_stamp=GETDATE() FROM alarms, sessions, device_events, users_groups, groups_sites, lns_devices ...`
- `UPDATE alarms SET clear_user_id=sessions.user_id, clear_comment=@comment, state=2, clear_time_stamp=GETDATE() ...`
- `IF EXISTS (SELECT id FROM tempdb.dbo.sysobjects WHERE id = OBJECT_ID('Tempdb..#tempalarms') AND type='U') DROP TABLE #tempalarms` — uses **tempdb global temp tables** for alarm aggregation.
- Complex access-group DELETE/INSERT transactions keyed on `@access_group_id` + `@device_id`.
- `UPDATE schedule_events SET expired=1 WHERE end_year!=0 AND end_month!=0 AND end_day!=0 ...` — nightly scheduler housekeeping, in `CirconLinq.dll`.

Many queries show the familiar `EXEC setsession @key` prefix — a stored procedure that pins a session identity into `CONTEXT_INFO` for row-level filtering through views. Every modification is scoped by user session.

---

## 4. LonWorks / LNS identity

### 4.1 Circon's LonMark manufacturer ID
```
Circon.Lns.LnsDeviceTemplate.CirconManufacturerId = 000018
```
= LonMark manufacturer **24 (0x18)** — Circon Systems Corp.

### 4.2 xDriver (Echelon remote LNS over IP) naming prefix
```
Circon.Lns.Forms.*.xDriverPrefix = X.Circon.
```
All remote LNS network names Circon creates are `X.Circon.<site>` — if you see those in `HKLM\SOFTWARE\LonWorks\...` or `ipconfig /displaydns`, that's this app.

### 4.3 LNS plug-in registry key
```
Circon.Lns.LnsPluginRegistryKeys.pluginRegistryKeyName = SOFTWARE\LonWorks\LCA\Plug-Ins
```
Where the Delphi `.pext` plug-ins register themselves. 35 plug-ins on this box.

### 4.4 Magic values
```
LnsSystem.AuthenticationDisabledKey = FFFFFFFFFFFF   -- all-F key = auth off
LnsSystem.NeuronIdNull              = 000000000000   -- empty Neuron ID
LnsObjectServer.InvalidNameChars    = *?.\:/"        -- rejected characters
```

### 4.5 Config variable types the app understands
`SCPT` (Standard Configuration Property Type), `SNVT` (Standard Network Variable Type), `UCPT` (User Configuration Property Type), `UNVT` (User Network Variable Type). Each has its own CLR namespace inside `CirconNvt.dll`.

---

## 5. OPC XML-DA bridge — `CirconOpcServer.dll` + `CirconOpcService.exe`

Ships a **full OPC XML-DA 1.0** SOAP endpoint, exposing all 7 standard operations:
```
http://opcfoundation.org/webservices/XMLDA/1.0/Browse
http://opcfoundation.org/webservices/XMLDA/1.0/GetProperties
http://opcfoundation.org/webservices/XMLDA/1.0/GetStatus
http://opcfoundation.org/webservices/XMLDA/1.0/Read
http://opcfoundation.org/webservices/XMLDA/1.0/Subscribe
http://opcfoundation.org/webservices/XMLDA/1.0/SubscriptionCancel
http://opcfoundation.org/webservices/XMLDA/1.0/Write
```
plus the standard WSDL fragments (`/a`, `/d`, `/h`, `/i`, `/p`, `/S`, `/T`, `/U`, `/X`).

### 5.1 Silverlight cross-domain policy baked in
`Circon.OpcXmlDa.OpcServer.File` and `CirconOpcService.OpcService` both embed the full `<?xml?><access-policy>…</access-policy>` document inline. That's what unblocks the Silverlight client (`WebGraphics.xap`) when it hits `/opcserver/lns` cross-origin.

### 5.2 Event log identity
```
eventLogName = Application
eventSource  = Circon Point Service
```
If anything fails in the OPC bridge, look in Event Viewer under *Application → Circon Point Service*.

---

## 6. Licensing — `wyDay.TurboActivate` + Circon feature maps

`CirconForms.dll` includes the `wyDay.TurboActivate` namespace and these file-path constants:

```
Circon.Forms.CirconActivate.VILicenseFile = TurboActivate.dat
Circon.Forms.CirconActivate.NILicenseFile = NetworkIntegrator.dat
Circon.Forms.CirconActivate.AILicenseFile = AccessIntegrator.dat
Circon.Forms.CirconActivate.BILicenseFile = BACnetIntegrator.dat   ← note!
```

**`BACnetIntegrator.dat` references a BACnet Integrator product.** There's no `bilns32.exe` on this box, but the licensing layer anticipates one — meaning a **BACnet-flavoured Integrator** exists (or did) in the EBAC catalog, unlockable via a separate `.dat`.

Browser license-check error string (recovered from `CirconBrowser.exe` resource):
> "No valid Efficient Building Automation license is detected. Please activate either Visual Integrator or Network Integrator."

---

## 7. IDE / shell integration constants (`CirconForms.dll`)

The app ships with full CLSID/IID tables for the modern Win32 common file dialogs (`IFileDialog`, `IFileOpenDialog`, `IFileSaveDialog`, `IShellItem`, `IKnownFolderManager`, `IProgressDialog`, etc.). These are standard Windows CLSIDs/IIDs — nothing bespoke — but the app does P/Invoke them directly instead of using the WinForms `OpenFileDialog` wrapper. Explains why file dialogs feel modern-ish even on a .NET 4.6.2 app.

```
CLSID_FileOpenDialog      = DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7
CLSID_FileSaveDialog      = C0B4E2F3-BA21-4773-8DBA-335EC946EB8B
CLSID_KnownFolderManager  = 4df0c730-df9d-4ae3-9153-aa6b82e9795a
```

---

## 8. Mixed-mode C++/CLI — `LnsChangeNet.dll`

Reflection surfaces `ATL`, `ATL.<AtlImplementationDetails>`, `ATL._ATL_SAFE_ALLOCA_IMPL`, `std`, and `CirconLns` namespaces inside the same assembly — the dead giveaway for a **C++/CLI mixed-mode** DLL. It's the glue between the managed `CirconLns` API and the native LNS change-notification plumbing (`LnsChange.dll` native sibling in the same folder).

Version is `1.0.7193.22746` — different from the rest of the 4.1.562 product line — meaning this component is built separately and rolled forward on its own cadence.

---

## 9. Service / log sinks

- **OPC service** logs to Windows Event Log → *Application / "Circon Point Service"*.
- **Site service** namespace `Circon.Service.Site` (fully visible in `VisualIntegrator-ILSpy.md`) implements its own Service Control Manager wiring.
- Logging interfaces live in `Circon.Service.Log` with WCF URIs `http://Circon.Service.Log` and `http://Circon.Service.Opc` — that's the namespace WCF prefix, not a reachable URL.

---

## 10. Notable security / hygiene observations

| # | Finding | Severity | Why it matters |
|---|---|---|---|
| 1 | User passwords stored as **unsalted MD5** (query: `password=@md5hash_password`) | High | MD5 is broken; any exfil of `users` row set yields crackable hashes. |
| 2 | Hard-coded dev connection string `Data Source=TAS64\CIRCON` in `CirconData.dll` | Low | Just a fossil; not used at runtime. Still a small info-leak about the build environment. |
| 3 | Silverlight cross-domain policy baked into the OPC service allows cross-site SOAP calls (default `*` policy inline) | Medium | Any page loaded in the embedded browser can call the OPC endpoint. OK on an isolated plant network; risky if the host is dual-homed. |
| 4 | OPC XML-DA endpoint exposes **Write** alongside **Read/Subscribe** | Medium | Writes propagate to live LonWorks field devices (VAVs, relays, setpoints). Authentication on this endpoint matters. |
| 5 | LNS auth disable sentinel hardcoded (`FFFFFFFFFFFF`) | Info | Known Echelon convention; expected. Just worth flagging that disabling LNS auth is a code-recognized state. |
| 6 | 44 ActiveX controls (OCX) register system-wide via `regsvr32` during install (shipped `Interop.*` wrappers imply this) | Info | On uninstall, orphaned CLSIDs may remain in HKCR. |
| 7 | `CirconGfxEditor` / `CirconGfxProject` reference an `ILOG.Diagrammer.Design` method that doesn't exist in the shipped DLL (reflection load fails) | Info | Suggests the on-disk `ILOG.Diagrammer.Design.dll` is older than what these two EXEs were compiled against — they'd throw `MissingMethodException` when you open context menus in the editor. Not exploitable, but a real latent bug. |
| 8 | Backups run as `LocalSystem` and write to `DATA\BACKUP\` without rotation | Info | 26 files from 2019 → 2026 still present (5.5 GB). No pruning. |

---

## 11. What a full ILSpy install would add, that this pass does **not** cover

- **Method bodies** — I can see every signature, but not the IL/C# body. For that you'd want the real ILSpy, dnSpy, dotPeek, or Mono.Cecil running on a Node-22-capable box.
- **.resx resource contents** — resource *names* are listed, but the actual `.resources` streams were not extracted.
- **Satellite assemblies** (localization) — none detected.
- **COM type library (.tlb)** contents of the `Interop.*` / `AxInterop.*` wrappers — the wrappers themselves are enumerated, but the underlying OCX interface definitions would need `OleView` / `tlbexp`.

---

## 12. File index

Three artifacts on your Desktop from this pass:

| File | Purpose |
|---|---|
| `VisualIntegrator-SystemMap.md` | Install-level map (directories, services, DB, services, dataflow). |
| `VisualIntegrator-ILSpy.md` | 37k-line raw reflection dump — every namespace, type, constructor, method, property, field, event, attribute, resource name, and referenced assembly across 28 assemblies. |
| `VisualIntegrator-Strings.md` | IL string literals + regex scan for SQL, connection strings, URLs, paths, registry keys, emails. |
| `VisualIntegrator-ILSpy-Findings.md` | **This file** — synthesized findings and highlights. |

Plus the two reusable scripts: `dump-managed.ps1` and `dump-strings.ps1` — re-runnable against any future Circon update to diff the surface.
