# Visual Integrator — Complete System Map

Read-only audit of the installation at `C:\Program Files (x86)\Circon\` on this machine.
Audited: 2026-11-17.

---

## 1. Product identity

| Field | Value |
|---|---|
| Product | **Visual Integrator** |
| Publisher | **Efficient Building Automation Corp.** (EBAC) — formerly / trades as **Circon Systems Corporation** |
| Copyright | 1998 – 2019 |
| Suite | *Visual Integrator* + *Network Integrator* + *Access Integrator* (same code base, three licensed personalities) |
| Domain | HVAC / building-automation front-end for **LonWorks** networks (Echelon LNS) |
| Installed version | **4.1.562.0** (final 2019 build) |
| Main exe build date | **2019-09-11** |
| Install root | `C:\Program Files (x86)\Circon\` |
| Main executable | `BIN\vilns32.exe` (660 KB, x86, managed) |
| Desktop shortcut | `C:\Users\shackleford\Desktop\Visual Integrator.lnk` → `BIN\vilns32.exe` |
| MSI product code | `{957A5AB4-FB1E-40E3-BCEB-51BF6C59902D}` |
| Assembly GUID | `4d7a8617-84fd-47c5-b9af-79d9185e745f` |
| License system | **TurboActivate** (`TurboActivate.dll` + `TurboActivate.dat`) + `EBACLicense.exe` + `VILicense\` folder |

Suite siblings in the same `BIN\` folder (all v4.1.562.0, built 2019‑09‑11):

| Exe | Role |
|---|---|
| `vilns32.exe` | Visual Integrator (this app) |
| `nilns32.exe` | Network Integrator |
| `ailns32.exe` | Access Integrator |

---

## 2. Technology stack

### 2.1 Main executable (`vilns32.exe`)

| Attribute | Value |
|---|---|
| Binary format | PE32, managed .NET |
| Architecture | **x86 (32-bit)** |
| Runtime | **.NET Framework 4.6.2** (declared in `vilns32.exe.config`, `useLegacyV2RuntimeActivationPolicy="true"`) |
| UI framework | **Windows Forms** (`System.Windows.Forms`) |
| Language | **C#** — proven by IL fingerprint: 20 `<>f__AnonymousType*` compiler-generated types (Roslyn/C# emission), zero `VB$AnonymousType_*` types, no `Microsoft.VisualBasic` reference |
| LINQ | Uses **LINQ to SQL** (`System.Data.Linq`, `System.Data.DataSetExtensions`) |
| Interop | Heavy **COM / ActiveX** interop (18 `AxInterop.*` / `Interop.*` assemblies) |
| Public types | 111 types across 4 namespaces: `Circon.VisualIntegrator`, `Circon.VisualIntegrator.Wizard`, `Circon.VisualIntegrator.Properties`, `Circon.ComInterop` |

### 2.2 Languages present across the whole install

| Language / runtime | Where used |
|---|---|
| **C# / .NET 4.6.2** | Main apps: `vilns32`, `nilns32`, `ailns32`, `CirconBrowser`, `CirconGfxEditor`, `CirconGfxProject`, `CirconSiteService`, `CirconOpcService`, `CirconCatalogManager`, `CirconServiceMgr`, `CirconWatchdog`, `CirconOpenLns`, `InstallDb`, Alarm/DataLog/LnsReport/Schedule tools |
| **Delphi / C++Builder (Borland)** | Plug-in subsystem: `CirconV01.bpl`, `PlugInUtil.bpl`, `HTMLHelp.bpl`, `Circon3xxPlugIn.exe`, `BASICPlugIn.exe` |
| **Native C/C++ (Win32 x86)** | OCX ActiveX controls (44 files), Echelon LNS libraries, device drivers (`xDriver.dll`, `sltacomm.dll`, `sltahook.dll`, `Hvwena32.dll`, `HobEDnt.dll`, `HvwEDnt.dll`), `Mynt4x.exe`, `Enable32.dll`, `NB32.exe` |
| **ASP.NET + WCF RIA Services** | `WebRoot\`, `CirconRiaWebService.Web.dll`, `WebGraphics.Web.dll`, `System.ServiceModel.DomainServices.*`, `Microsoft.ServiceModel.DomainServices.LinqToSql` |
| **Silverlight (XAML)** | Web UI client: `WebGraphics.xap`, `CirconGfxProject.xap`, `page.xaml`, `schedule.xaml`, `MainPage.html`, `Silverlight.js` |
| **Circon BASIC** | Embedded scripting for controllers (`Blank Device.app`, `Blank Functional Block.fpt`, `.lib` files, `Examples\BASIC\`, documented in `Circon Basic Language Reference.pdf`) |
| **VBScript** | Legacy automation hooks (`VBSCRIPT.HLP` shipped) |
| **T-SQL** | Schema + triggers + upgrades (`SHARED\CirconDb.sql`, `TriggersSql.sql`, `Upgrade.sql`) |
| **AVR assembly / firmware** | Controller firmware: `uhc300v02.a90`, `uhc320v02.a90` (Atmel `.a90` image format) |
| **XML config / device catalog** | `wizard.xml`, `CirconEvents.xml`, `UserEvents.xml`, `SHARED\DeviceXML\*.xml`, `applicationhost.config`, `Web.config` |

### 2.3 Third-party stack (shipped DLLs / OCXs)

| Vendor / library | Purpose |
|---|---|
| **Echelon LNS 3.25** (`Echelon.LNS.Interop`, `Echelon.LNS.Plugin.Framework`) | LonWorks Network Services core API |
| **Chilkat** (`ChilkatDotNet4`, `ChilkatMail2`, `ChilkatUtil`, `ChilkatCert`, `CkString`) | SMTP / email, certificates, crypto |
| **TeeChart 5** (`TeeChart5.ocx`, `AxInterop.TeeChart`, `Interop.TeeChart`) | Trend / log charting |
| **Crystal Reports** (`Interop.CRAXDRT.dll`, `CRVIEWERLib`, `AxInterop.CRVIEWERLib`) + `.rpt` templates | Report rendering |
| **ILOG.Diagrammer** | Diagram / schematic drawing control |
| **Ionic.Zip** (`Ionic.Zip.dll`) | Zip IO (archive / backup / export) |
| **Divelements SandDock 3.0 + WizardFramework** | Docking panels, wizards |
| **SourceGrid2**, **SourceLibrary** | Grid controls |
| **GoldParser** | Parser (likely for Circon BASIC) |
| **Edanmo.OleStorage** | OLE compound-document IO (reads/writes `.vbd`, `.app` structured storage) |
| **TurboActivate** | License activation |
| **Microsoft RIA Services** | WCF DomainServices + LinqToSql host |

---

## 3. Directory tree (what lives where)

```
C:\Program Files (x86)\Circon\
├── BIN\                 216 files, ~89 MB   ← all runtime binaries + docs
│   ├── vilns32.exe              Visual Integrator main GUI (C#, WinForms)
│   ├── nilns32.exe              Network Integrator (same code, NI persona)
│   ├── ailns32.exe              Access Integrator (same code, AI persona)
│   ├── CirconSiteService.exe    Windows service (background runtime)
│   ├── CirconOpcService.exe     OPC server service
│   ├── CirconServiceMgr.exe     Service manager UI
│   ├── CirconWatchdog.exe       Watchdog
│   ├── CirconBrowser.exe        Circon custom browser shell
│   ├── CirconGfxEditor.exe      Web graphics editor (edits .vbd pages)
│   ├── CirconGfxProject.exe     Silverlight web project manager
│   ├── CirconCatalogManager.exe Device catalog editor
│   ├── CirconOpenLns.exe        Global LNS Server
│   ├── CirconDbBackup.exe       DB backup
│   ├── CirconDbArchive.exe      Long-term archive
│   ├── CirconDbPurge.exe        Periodic purge of alarms/trends
│   ├── AlarmEmail.exe           Email outbound alarms
│   ├── AlarmLog.exe             Alarm log writer
│   ├── AlarmNotify.exe          Alarm notification dispatcher
│   ├── AlarmPrint.exe           Alarm printer service helper
│   ├── DataLogLocal.exe         Local data log collector
│   ├── DataLogRemote.exe        Remote data log collector
│   ├── LnsReport.exe            LNS report generator
│   ├── ScheduleSync.exe         Schedule sync to devices
│   ├── CirconPagePrint.exe      Page print utility
│   ├── CirconSiteBackup.exe     Site-level backup
│   ├── CirconSiteLns.exe        Site LNS connector
│   ├── CirconSiteLog.exe        Site log writer
│   ├── CirconSiteOpc.exe        Site OPC connector
│   ├── CirconSiteSync.exe       Site sync
│   ├── CirconStartService.exe   Service bootstrap
│   ├── CirconLogService.exe     Logging service
│   ├── CirconTask.exe           Task runner
│   ├── Circon.BrowserPlugin.exe Browser plugin host
│   ├── EBACLicense.exe          EBAC license tool
│   ├── InstallDb.exe            Database installer
│   ├── InstallWebServices.exe   Web services installer
│   ├── RasDial.exe              Dial-up RAS helper
│   ├── RestoreDb.exe            Database restore
│   ├── ServiceManager.exe       Legacy service manager
│   ├── Sync.exe                 Generic sync utility
│   ├── Viewer.exe               Standalone viewer
│   ├── WatchdogSettings.exe     Watchdog config UI
│   ├── Mynt4x.exe               Loytec / 4x FT-10 comms helper
│   ├── NB32.exe                 Neuron Browser 32 (controller protocol)
│   ├── *.ocx (44)               ActiveX UI controls (see §7)
│   ├── *.dll (88)               Managed + native libraries
│   ├── Interop.* / AxInterop.*  COM interop bridges to the OCX stack
│   ├── Blank Device.app         OLE-structured empty device template
│   ├── Shadow Device.app        Shadow device template
│   ├── Blank Functional Block.fpt / Shadow Functional Block.fpt
│   ├── Blank Subsystem.sub / Shadow Subsystem.sub
│   ├── Blank Graphic.vbd / Shadow Graphic.vbd / BlankDoc.vbd
│   ├── page.xaml, schedule.xaml, page.ivn
│   ├── MainPage.html, Silverlight.js (in WebRoot)
│   ├── wizard.xml               Document tree layout for New Project wizard
│   ├── snvtunits.txt, units.txt Engineering-unit tables
│   ├── applicationhost.config   IIS Express site config (see §6.3)
│   ├── Web.config               WCF RIA config (see §6.3)
│   ├── aspnet.config
│   ├── temp.mdf                 Scratch SQL Server data file
│   ├── TurboActivate.dll/.dat   License runtime
│   ├── Images\                  68 HVAC gif/png/bmp icons (fans, dampers, VAVs…)
│   ├── Reports\                 15 Crystal .rpt report templates
│   ├── NewProject\              CirconGfxProject.xap template
│   ├── WebRoot\                 ASP.NET site root (Default.aspx + ClientBin\WebGraphics.xap + bin\)
│   ├── Docs\                    (empty stub)
│   ├── VILicense\               (license storage)
│   ├── AI.chm, ni3.chm          Compiled HTML help for AI & NI
│   ├── Visual Integrator 3.pdf  9.4 MB user guide (2018‑12‑19)
│   ├── Release Notes.pdf        VI release notes
│   ├── NI Release Notes.pdf     NI release notes
│   ├── AI Release Notes.pdf     AI release notes
│   ├── Network Integrator 3 User Guide.pdf
│   └── VBSCRIPT.HLP             Legacy Microsoft VBScript help
│
├── DATA\                5.5 GB total ← runtime data
│   ├── DB\
│   │   ├── circon.mdf          Active SQL Server data file
│   │   └── circon.ldf          Active SQL Server log file
│   ├── BACKUP\         26 files ← quarterly auto-backups
│   │   ├── 20191001-020000.bak (first)  … through
│   │   └── 20261001-020000.bak (newest, ~76 MB)
│   ├── ARCHIVE\                Long-term archives
│   └── EMAIL\
│       └── ERRORS\             Outbound email failure spool
│
├── SHARED\              Cross-app assets
│   ├── CirconDb.sql            Full schema script, DB version 2.87 (see §5)
│   ├── Upgrade.sql             DB upgrade migrations (2.60 → 2.87)
│   ├── TriggersSql.sql         Trigger definitions
│   ├── CirconEvents.xml        Built-in event / alarm translation table (rev 49)
│   ├── UserEvents.xml          User-defined event translations
│   ├── DeviceXML\              24 XML device profiles (see §8)
│   ├── SetACL.exe              Permissions tool used by installer
│   └── resreg.exe              Resource registration tool
│
├── PlugIn\              63 files (incl. Extensions\)
│   ├── Circon3xxPlugIn.exe     Delphi plug-in host for 3xx controllers
│   ├── BASICPlugIn.exe         Circon BASIC editor / downloader
│   ├── APCFlasher\             Firmware flasher for APC-300
│   ├── CirconV01.bpl           Delphi runtime package
│   ├── PlugInUtil.bpl          Delphi utilities
│   ├── HTMLHelp.bpl            Delphi HTMLHelp wrapper
│   ├── NB32.exe                Neuron browser
│   ├── BASIC-*.lib             Circon BASIC libraries per controller model
│   ├── BASCSNVT.LST            BASIC SNVT list
│   ├── TARGETS.INI, Calib.ini  Plug-in config
│   ├── libDevInfoMap.dll, libEventTranslation.dll, libLNSWrappers.dll,
│   │   libResourceFileAPI.dll, libUtil.dll   Native plug-in libs
│   ├── Download.avi            Download tutorial video
│   ├── Circon Basic Language Reference.pdf
│   ├── CirconPlugins.chm
│   └── Extensions\             35 .pext plug-ins (see §9)
│
├── IOSubProcessor\
│   ├── uhc300v02.a90           Atmel AVR firmware for UHC-300 I/O subprocessor
│   └── uhc320v02.a90           Atmel AVR firmware for UHC-320
│
├── Examples\
│   └── BASIC\                  Circon BASIC sample programs
│
└── Installation Guides\        12 controller installation PDFs:
    ├── 80-0128 UHC-300           Universal HVAC Controller 300
    ├── 80-0129 UHC-302           + 80-0300 UHC-302-XMV
    ├── 80-0131 UHC-320
    ├── 80-0132 SCC-300           Space Comfort Controller
    ├── 80-0305 SMC-300           Small Mechanical Controller
    ├── 80-0310 APC-300           Advanced Programmable Controller
    ├── 80-0315 APC-300-EAC
    ├── 80-0332 VAV-332-IMV_PRG   Variable Air Volume
    ├── 80-0335 VAV-332-XMV_XPR
    ├── 80-0401 UHC-400
    └── 80-0406 MAC-300           Monitoring and Alarming Controller
```

---

## 4. Windows services & processes

Services registered by the suite (observed via `sc.exe`):

| Service name | Display name | State | Binary | Depends on |
|---|---|---|---|---|
| `CirconSite` | Circon Site Service | **RUNNING** (auto-start, delayed) | `BIN\CirconSiteService.exe` | `MSSQL$CIRCON` |
| `Circon Alarm Printer` | Circon Alarm Printer | Stopped | (alarm print) | — |
| `Circon RAS Dial-up Service` | Circon RAS Dial-up Service | Stopped | `RasDial.exe` | — |
| `MSSQL$CIRCON` | SQL Server (CIRCON) | **RUNNING** | `C:\Program Files (x86)\Microsoft SQL Server\MSSQL12.CIRCON\MSSQL\Binn\sqlservr.exe -sCIRCON` | — |
| `SQLAgent$CIRCON` | SQL Server Agent (CIRCON) | Stopped | SQL Agent | — |
| `LnsMtsSvc` | Echelon Support Service for Microsoft Terminal Services (MTS) | **RUNNING** | Echelon LNS | — |
| `xDriver Connection Broker` | Echelon xDriver Connection Broker | Stopped | Echelon xDriver | — |

**Key fact:** `MSSQL12.*` is **SQL Server 2014**, named instance `CIRCON`.
`CirconSiteService` runs as LocalSystem, starts after SQL, and is the background heart of the system (schedule sync, alarm pumping, data log, OPC bridging).

---

## 5. Database layer

### 5.1 Hosting

| Item | Value |
|---|---|
| SQL instance | `(local)\CIRCON` (SQL Server 2014, Express-class based on path) |
| Database name | `circon` |
| Files | `DATA\DB\circon.mdf` + `circon.ldf` |
| Scratch file | `BIN\temp.mdf` |
| Connection string | `Data Source=(local)\Circon;Initial Catalog=circon;Integrated Security=True` (from `Web.config`, Windows auth) |
| ORM | **LINQ to SQL** (`CirconDataLinqForms.dll`, `Microsoft.ServiceModel.DomainServices.LinqToSql.dll`) |
| Schema version | **2.87** (declared in `CirconDb.sql`) |
| Schema size | **140 tables** |

### 5.2 Table groups (from `SHARED\CirconDb.sql`)

The 140 tables decompose into these functional domains:

- **Access control (~22 tables)** — access_users, access_cards, access_card_formats, access_format_types, access_points, access_zones, access_apb_zones, access_groups, access_groups_points, access_users_groups, access_actions, access_tests, access_images, access_users_images, access_users_exts, access_users_custom_exts, access_group_actions, access_group_enable_tests, access_group_lockout_tests, access_group_schedule_objects, access_device_cards, access_device_card_changes, access_device_groups, access_device_group_actions, access_device_group_changes, access_device_group_enable_tests, access_device_group_lockout_tests.
- **Alarms (~7)** — alarms, alarm_categories, alarm_codes, alarm_colors, alarm_priorities, alarm_states, SNVT_alarm_priorities.
- **Events / profiles** — device_events, event_profiles, event_series_profiles, event_access_groups_profiles, event_access_points_profiles, event_access_users_profiles, system_events, system_event_categories, system_event_types.
- **Trend / log** — trend_models, trend_profiles, trend_series_profiles, trend_values, runtime_values, log_profile, site_logtimes.
- **Scheduling** — schedule_events, schedule_nvs, schedule_objects, schedule_states, schedule_zones.
- **Sites / groups / partitions** — sites, groups, groups_sites, groups_partitions, groups_alarm_codes, groups_alarm_ack_codes, partitions, partitions_fields.
- **LNS inventory** — lns_devices, lns_appdevice_templates, lns_classids, lns_lonmark_objects.
- **SNVTs / units** — snvts, units, unit_systems.
- **Users / sessions / prefs** — users, users_groups, sessions, user_prefs.
- **Notifications** — email_notifications, print_notifications.
- **Misc** — applications, customers, global, upgrade.

### 5.3 Upgrade / triggers

- `Upgrade.sql` is a chained `IF ((SELECT version from global) = 'X.Y') BEGIN … UPDATE global SET version = 'X.Y+1' END` migration from v2.60 upwards.
- `TriggersSql.sql` adds purge columns to `global` (alarm_purge_days, trend_purge_days, event_purge_days, runtime_purge_days) — consumed by `CirconDbPurge.exe`.

### 5.4 Backups

- Automatic quarterly `.bak` to `DATA\BACKUP\` at 02:00 UTC (filename pattern `YYYYMMDD-HHMMSS.bak`).
- 26 backups on disk, first 2019‑10‑01, last **2026‑10‑01** (~76 MB). Several are 300 MB+ around 2020, suggesting active history retention.

---

## 6. Web / integration tier

### 6.1 WCF RIA Services (ASP.NET)

Located under `BIN\WebRoot\`:

```
WebRoot\
├── Default.aspx
├── Silverlight.js
├── Web.config
├── ClientBin\
│   └── WebGraphics.xap        (Silverlight app payload)
└── bin\
    ├── CirconRiaWebService.Web.dll
    ├── WebGraphics.Web.dll
    ├── System.ServiceModel.DomainServices.Hosting.dll
    ├── System.ServiceModel.DomainServices.Server.dll
    ├── Microsoft.ServiceModel.DomainServices.LinqToSql.dll (+ .xml doc)
    ├── CirconLns.dll, CirconLonWorks.dll, CirconNvt.dll,
    ├── CirconLdrf.dll, CirconLinq.dll, CirconService.dll, CirconUtil.dll
    ├── Echelon.LNS.Interop.dll
    ├── Ionic.Zip.dll
    └── GoldParser.dll
```

`Web.config` exposes a WCF endpoint:

- Service: `WebGraphics.Web.Service1`
- Binding: custom, `binaryMessageEncoding` over `httpTransport`
- Limits: `maxReceivedMessageSize = maxBufferSize = maxBufferPoolSize = 2147483647`
- Auth: ASP.NET **Forms**
- Compilation target: .NET 4.0 (host side), `aspNetCompatibilityEnabled="true"`

### 6.2 Silverlight client (`WebGraphics.xap` + `CirconGfxProject.xap`)

- Pages authored as XAML (`page.xaml`, `schedule.xaml`).
- Root control is `circon:Page` from `clr-namespace:Circon.Web.Controls;assembly=WebGraphics`.
- Live data comes from `opc:OpcPointGroup` bound to `/opcserver/lns` with `PollInterval="1"` second — i.e. the Silverlight UI polls the local **OPC server service** every second over HTTP.
- Includes Silverlight Toolkit (DataVisualization charts, DataVisualization.Toolkit), Expression.Interactivity, Expression.Drawing.
- `CirconGfxProject.exe` is the WinForms side that **authors** these XAPs.

### 6.3 IIS Express host (`applicationhost.config`)

- One site registered:
  - name: `Circon Visual Integrator`, id 1
  - app pool: `Clr4IntegratedAppPool`
  - binding: `http *:80:localhost`
  - physicalPath: **`c:\work\testopcservice`** ← dev leftover path still in config
- Site defaults log in W3C format to `%IIS_USER_HOME%\Logs`, failed-request tracing enabled.

### 6.4 Custom browser

`CirconBrowser.exe` wraps IE (`SHDocVw`, `Browser.ocx`) and persists `WindowPosition`, `WindowState`, `LastFile`, `VisibleCols` into the user settings file. Launches the Silverlight UI inline for operators who don't have IE configured.

---

## 7. ActiveX / OCX control catalog (44 controls in BIN)

These are the native x86 UI widgets the WinForms app embeds via `AxInterop.*`:

| OCX | Typical role |
|---|---|
| `CirconAlarm.ocx` | Alarm grid |
| `CirconAnimatedPoint.ocx` | Animated HVAC point icon |
| `CirconButtonPoint.ocx` | Button-bound point |
| `CirconDataPoint.ocx` | Raw data point |
| `CirconDbUI.ocx` | Database browser |
| `CirconDeviceStatus.ocx` | Device status indicator |
| `CirconEnumPoint.ocx` | Enumerated-value point |
| `CirconGaugePoint.ocx` | Analog gauge |
| `CirconImage.ocx` | Linked image |
| `CirconNavigate.ocx` | Site-tree navigator |
| `CirconObjectLabel.ocx` | Object text label |
| `CirconSliderPoint.ocx` | Slider input |
| `CirconSwitchPoint.ocx` | Switch input |
| `CirconTempSetPoint.ocx` | Temperature setpoint |
| `CirconTimePoint.ocx` | Time point |
| `CirconTimer.ocx` | Timer logic |
| `Browser.ocx` | Embedded IE control |
| `Common.ocx`, `DataObject.ocx`, `EnumObject.ocx`, `obselect.ocx` | Generic COM utilities |
| `EventLog.ocx`, `LogConfig.ocx` | Event logging UI + config |
| `NVDigital.ocx`, `NVGauge.ocx`, `NVSetpoint.ocx`, `NVSlider.ocx`, `NVSwitch.ocx`, `NVTime.ocx`, `NVanimat.ocx` | Legacy Network Variable widgets |
| `PenPlot.ocx`, `TeeChart5.ocx`, `TrendLog.ocx` | Charting / trend plot |
| `PlugIn.ocx` | Plug-in host surface |
| `RelayCmd.ocx`, `RelayStatus.ocx`, `SwsCmd.ocx` | Relay / switch controls |
| `Reports.ocx` | Report viewer |
| `ScheduleUI.ocx` | Schedule editor |
| `Timer.ocx` | Timer |
| `ccgraph.ocx`, `datalabel.ocx`, `label.ocx` | Generic labels + graph |
| `lnsui.ocx` | LNS UI |

Interop wrappers: `AxInterop.{BROWSERLib, CIRCONDBUILib, CIRCONNAVIGATELib, CirconLNS_Navigate, CirconLNS_UI, CRVIEWERLib, EventLogLib, GRAPHICLib, LOGCONFIGLib, REPORTSLib, SCHEDULEUILib, SHDocVw, TRENDLOGLib, TeeChart}` plus matching non-Ax `Interop.*`.

---

## 8. LonWorks / LNS integration (the "integrator" part)

### 8.1 Native + managed LNS libraries

- `Echelon.LNS.Interop.dll` v3.25 — direct Echelon LNS API bindings
- `Echelon.LNS.Plugin.Framework.dll` — official Echelon plug-in framework
- `CirconLns.dll`, `CirconLnsDevices.dll`, `CirconLnsForms.dll`, `CirconLnsDevicesForms.dll` — Circon's managed wrappers
- `CirconLonWorks.dll`, `CirconNvt.dll` (SNVTs), `CirconLdrf.dll` (LonMark Device Resource Files)
- `LnsChange.dll`, `LnsChangeNet.dll` — change-tracking against LNS DB
- `sltacomm.dll`, `sltahook.dll` — SLTA (Serial LonTalk Adapter) comms + hook
- `HobEDnt.dll`, `HvwEDnt.dll`, `Hvwena32.dll`, `Enable32.dll` — HobbitBus / HVW Enable device drivers
- `Mynt4x.exe`, `NB32.exe` — low-level neuron/FT-10 helpers
- `xDriver.dll` — Echelon xDriver (TCP-over-IP tunneling into LonWorks)
- `LonMarkLdrf.dll` — LonMark LDRF handling
- `BASCSNVT.LST`, `snvtunits.txt`, `units.txt` — Standard Network Variable Type + unit tables

### 8.2 Device profiles (`SHARED\DeviceXML\`, 24 files)

One XML descriptor per supported controller firmware variant:

```
mac300STAm01 / m02
scc300AHCm05, FCCm05, GPCm02, HPCm05, PRGm02, RTCm05, UVCm05
scc410Bm18, PRGm14
scc420PRGm30, m31
scc430PRGm30
uhc300m07, uhc320m07
uhc400m12, m14
uhp340m01
vav332PRGm11
```

---

## 9. Plug-in extensions (`PlugIn\Extensions\`, 35 `.pext`)

Delphi-based plug-ins delivered as `.pext` files — loaded dynamically by `Circon3xxPlugIn.exe`.

Grouped by purpose:

- **Controller plug-ins** (open the device's config / monitoring UI):
  `APC300.pext`, `APC300EAC.pext`, `APC300Viewer.pext`, `APC300EACViewer.pext`, `SMC300.pext`, `MAC300STAm01.pext`, `UHC302m03.pext`, `UHC302m04.pext`, `UHC302m05.pext`, `UHC302Calibration.pext`, `VAV332m10.pext`, `SCC300GPCm01.pext`, `SCC300GPCm02.pext`, `SCC300xxCm04.pext`, `SCC300xxCm05.pext`, `ProgrammableDeviceViewer.pext`, `ProgrammableTUCs.pext`, `ProgrammableVAVs.pext`.
- **Firmware / provisioning**:
  `FirmwareDownloader.pext`, `AVRDownLoader.pext`, `OSTConfig.pext`, `RealTimeKeeper.pext`.
- **Logic / configuration tools**:
  `AlarmGenerator.pext`, `Balancer.pext`, `DeviceScheduler.pext`, `PIDConfig.pext`, `SharedVariables.pext`, `SideLoop.pext`, `TrendConfigView.pext`, `UniversalInput.pext`, `UniversalInputOutput.pext`, `UniversalOutput.pext`.
- **Diagnostics**:
  `DebugUtil.pext`, `EventLogViewer.pext`, `EventQueueViewer.pext`.

Alongside: `APCFlasher\` (firmware flash tool), `BASIC-scc300.lib / BASIC-uhc302*.lib / BASIC-uhc3x0.lib / BASIC-vav332*.lib` are Circon BASIC runtime libraries for each controller target.

---

## 10. Reports (`BIN\Reports\`)

Crystal Reports templates (`.rpt`), both runtime and translated (`_t.rpt`) variants:

| Report | Purpose |
|---|---|
| `alarms.rpt` / `alarms_t.rpt` | Alarm history |
| `activity.rpt` / `activity_t.rpt` | User / system activity log |
| `runtime.rpt` / `runtime_t.rpt` | Runtime hours per point |
| `scheduler.rpt` / `scheduler_t.rpt` | Scheduler export |
| `users.rpt` / `users_t.rpt` | User list |
| `groups.rpt` / `groups_t.rpt` | Group / partition list |
| `AccessPoints_t.rpt` | Access control points |
| `AccessUsers_Brief_t.rpt`, `AccessUsers_Full_t.rpt` | Access users |

Rendered by `Reports.ocx` + `Interop.CRAXDRT.dll` + `AxInterop.CRVIEWERLib.dll`. `LnsReport.exe` drives batch generation.

---

## 11. Config files

| File | Purpose |
|---|---|
| `BIN\vilns32.exe.config` | .NET 4.6.2 startup tag only |
| `BIN\nilns32.exe.config` | .NET 4.6.2 startup tag only |
| `BIN\CirconGfxProject.exe.config` | .NET 4.6.2 startup tag only |
| `BIN\CirconBrowser.exe.config` | User settings: `WindowPosition=0,0,800,600`, `WindowState=Normal`, `LastFile`, `VisibleCols=1,1,1,1,1,1,1,1` |
| `BIN\InstallDb.exe.config` | DB installer settings |
| `BIN\Circon.BrowserPlugin.exe.config` | Browser plugin host |
| `BIN\nilns32.exe.manifest` | COM / side-by-side isolation manifest for NI |
| `BIN\Web.config` | WCF RIA Services + connection string (see §6) |
| `BIN\applicationhost.config` | IIS Express site config (see §6.3) |
| `BIN\aspnet.config` | ASP.NET host tuning |
| `BIN\wizard.xml` | Document-tree seeding for the New Project wizard |
| `SHARED\CirconEvents.xml` | Built-in event/alarm translation (format rules in file header) |
| `SHARED\UserEvents.xml` | User-added event translations (revison=4) |
| `PlugIn\TARGETS.INI`, `PlugIn\Calib.ini` | Plug-in target + calibration tables |
| `BIN\AccessIntegrator.dat`, `NetworkIntegrator.dat`, `TurboActivate.dat` | Binary data blobs (feature/permission/license bitmaps) |

---

## 12. End-to-end system map (how it hangs together)

```
                        ┌──────────────────────────────────────────┐
                        │  Operator desktop (Windows)              │
                        │                                          │
                        │  vilns32.exe  (C# WinForms + ActiveX)    │
                        │  ├─ hosts 44 OCX controls (alarms,       │
                        │  │  schedules, trends, HVAC widgets)     │
                        │  ├─ Crystal Reports viewer                │
                        │  ├─ Delphi plug-in bridge (.pext)        │
                        │  └─ Circon BASIC editor                   │
                        │                                          │
                        │  Alt UIs: CirconBrowser.exe  → Silverlight│
                        │           CirconGfxEditor.exe             │
                        │           CirconGfxProject.exe            │
                        └──────────────┬───────────────────────────┘
                                       │ LINQ-to-SQL (Windows auth)
                                       │ COM in-process
                                       ▼
                        ┌──────────────────────────────────────────┐
                        │  SQL Server 2014  (MSSQL$CIRCON)         │
                        │  DB "circon" — schema v2.87, 140 tables  │
                        │  circon.mdf / circon.ldf in DATA\DB      │
                        │  Quarterly .bak → DATA\BACKUP            │
                        └──────────────┬───────────────────────────┘
                                       ▲
                                       │
┌──────────────────────┐   WCF binary  │          ┌───────────────────────────┐
│ Silverlight browser  │  (RIA)        │          │ CirconSiteService (svc)   │
│  WebGraphics.xap     │◄────────┐     │          │ ├─ schedule sync          │
│  page.xaml / schedule│         │     │          │ ├─ alarm pump             │
│  polls /opcserver/lns│         │     │          │ ├─ data-log collect       │
│  every 1 s           │         │     │          │ ├─ purge jobs             │
└──────────┬───────────┘         │     │          │ └─ runs AlarmEmail,       │
           │                     │     │          │    AlarmNotify, …         │
           │ HTTP                │     │          └──────────┬────────────────┘
           ▼                     │     │                     │
┌──────────────────────────────┐ │     │                     │
│ IIS Express (localhost:80)   │ │     │                     │
│  site "Circon Visual         │ │     │                     │
│  Integrator"                 │ │     │                     │
│  app pool Clr4Integrated     │─┘     │                     │
│  → WebRoot\ (ASP.NET 4.0)    │       │                     │
│    WebGraphics.Web.dll       │       │                     │
│    CirconRiaWebService.Web   │       │                     │
│    LinqToSql DomainService   │───────┘                     │
└──────────────────────────────┘                             │
                                                             ▼
                                                 ┌───────────────────────────┐
                                                 │ CirconOpcService (OPC)    │
                                                 │ CirconOpenLns (LNS srv)   │
                                                 │ Echelon.LNS.Interop 3.25  │
                                                 │ LnsMtsSvc (Echelon svc)   │
                                                 │ xDriver Connection Broker │
                                                 └──────────┬────────────────┘
                                                            │  LonTalk
                                                            │  (FT-10 TP/FT-10,
                                                            │   SLTA serial,
                                                            │   or xDriver/IP)
                                                            ▼
                                                 ┌───────────────────────────┐
                                                 │ LonWorks field devices    │
                                                 │ UHC-300/302/320/400       │
                                                 │ SCC-300/410/420/430       │
                                                 │ SMC-300, APC-300, APC-EAC │
                                                 │ VAV-332, UVC, HPC, RTC…   │
                                                 │ MAC-300 (alarming)        │
                                                 │ UHP-340                   │
                                                 │ (firmware = Atmel AVR)    │
                                                 └───────────────────────────┘
```

### Typical data flow for a point value

1. A **UHC-302** on the LonWorks trunk exposes an SNVT network variable (e.g. `nvoSpaceTemp`).
2. `CirconOpcService` polls LNS via `Echelon.LNS.Interop` at its configured poll rate.
3. New value lands in `runtime_values` / `trend_values` in SQL via `DataLogLocal.exe`.
4. Alarm engine (`CirconSiteService`) compares against `alarm_codes` / `alarm_priorities` and writes to `alarms` + `device_events`; translation is pulled from `CirconEvents.xml` / `UserEvents.xml`.
5. If an email template fires, `AlarmEmail.exe` renders via Chilkat and (on failure) drops to `DATA\EMAIL\ERRORS\`.
6. `vilns32.exe` operator UI picks up changes via LINQ-to-SQL polling and refreshes an OCX (e.g. `CirconAnimatedPoint.ocx`) tied to that point.
7. Silverlight client gets the same value via the WCF RIA endpoint at `/opcserver/lns`, at 1 s poll.
8. Nightly: `CirconDbPurge.exe` honors `alarm_purge_days / trend_purge_days / event_purge_days / runtime_purge_days` from the `global` table.
9. Quarterly 02:00: `CirconDbBackup.exe` writes a timestamped `.bak` to `DATA\BACKUP\`.

### Typical commissioning / engineering flow

1. User opens `vilns32.exe`, creates a Project; `wizard.xml` seeds the document tree (Documents / Subsystems / Circon Web Site).
2. User drops **devices** from the catalog — the catalog is XML in `SHARED\DeviceXML\` and templates in `BIN\*.app / *.fpt / *.sub / *.vbd`.
3. For programmable controllers, the user launches a `.pext` plug-in from `PlugIn\Extensions\` (e.g. `ProgrammableVAVs.pext`) via `PlugIn.ocx` → `Circon3xxPlugIn.exe` (Delphi).
4. User writes/edits logic in **Circon BASIC** (`BASICPlugIn.exe`, libs in `PlugIn\BASIC-*.lib`).
5. `FirmwareDownloader.pext` / `AVRDownLoader.pext` flashes firmware (`uhc300v02.a90`, `uhc320v02.a90`) through `NB32.exe` and the SLTA/xDriver.
6. Graphic pages are authored in `CirconGfxEditor.exe`, saved as `.vbd` (OLE structured storage), and published as `.xap` to `BIN\WebRoot\ClientBin\` via `CirconGfxProject.exe`.

---

## 13. Documentation on disk

| Doc | Location | Date |
|---|---|---|
| Visual Integrator 3.pdf (9.4 MB) | `BIN\` | 2018‑12‑19 |
| Release Notes.pdf | `BIN\` | 2018‑12‑17 |
| NI Release Notes.pdf | `BIN\` | 2018‑12‑17 |
| AI Release Notes.pdf | `BIN\` | — |
| AI.chm | `BIN\` | 2017‑03‑24 |
| ni3.chm | `BIN\` | 2017‑03‑24 |
| Network Integrator 3 User Guide.pdf | `BIN\` | 2017‑03‑24 |
| Circon Basic Language Reference.pdf | `PlugIn\` | — |
| CirconPlugins.chm | `PlugIn\` | — |
| VBSCRIPT.HLP | `BIN\` | — (legacy) |
| 12 × controller Installation Guide PDFs | `Installation Guides\` | — |

---

## 14. Quick facts cheat-sheet

- **What it is:** Operator + engineering workstation for Echelon **LonWorks** building-automation networks.
- **Who writes it:** Efficient Building Automation Corp. (EBAC) under the **Circon** brand.
- **Primary language:** **C#** on .NET Framework 4.6.2 (Windows Forms).
- **Secondary languages:** Delphi (plug-in subsystem), native C/C++ (OCX + LNS drivers), Silverlight XAML (web UI), T-SQL (database), Circon BASIC (controller logic), AVR assembly (device firmware).
- **Bitness:** 32-bit only.
- **DB:** SQL Server 2014 named instance `CIRCON`, database `circon`, 140 tables, schema v2.87, Windows auth.
- **Background service:** `CirconSite` → `CirconSiteService.exe`, auto-start delayed, depends on `MSSQL$CIRCON`.
- **Web UI:** IIS Express site "Circon Visual Integrator" on `http://localhost:80/`, hosting WCF RIA Services + Silverlight XAP.
- **Build:** v4.1.562.0, compiled 2019‑09‑11 — effectively the final release (18 months of EOL legacy use since 2019).
- **Licensing:** TurboActivate + EBAC license tool + `VILicense\` + `AccessIntegrator.dat` / `NetworkIntegrator.dat` feature maps.
- **Backups still running:** last `.bak` on 2026‑10‑01.

---

*End of system map — generated from on-disk audit, no files modified.*
