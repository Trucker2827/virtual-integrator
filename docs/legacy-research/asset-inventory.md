# Asset inventory — Visual Integrator originals

What exists on the Windows 7 reference workstation under `C:\Program Files (x86)\Circon\` and `C:\NI\`, and what was extracted for reference. Nothing from the original install is modified.

All extracted copies live locally at `C:\Users\shackleford\Desktop\VirtualIntegrator_Claude\extracted\` and are **not** committed to this repo. See the local `MANIFEST.md` in that folder for absolute paths.

## Counts

| Category | Count | Source |
|---|---:|---|
| HVAC icons (static GIF/PNG/BMP)                 | 68  | `BIN\Images\` |
| Animated-state GIFs (carved from OLE devices)   | 415 | `.app` / `.sub` inside live site |
| Static PNGs carved from OLE                     | 47  | `.app` / `.vbd` |
| JPEG page / camera images carved from OLE       | 4   | `.vbd` |
| OLE template files (`Blank*`, `Shadow*`)        | 9   | `BIN\` |
| Live project OLE files (`.vbd` / `.app` / `.sub`) | 96 | `C:\NI\Files\Duke Realty\Building 1835\vi\` |
| ActiveX OCX controls                            | 44  | `BIN\` |
| Crystal Reports templates (`.rpt`)              | 15  | `BIN\Reports\` |
| Silverlight XAP packages                        | 2   | `BIN\WebRoot\ClientBin\`, `BIN\NewProject\` |
| Quarterly site backup ZIPs                      | 28  | `C:\NI\Files\Duke Realty\Building 1835\backup\` |
| Media (tutorial)                                | 1   | `PlugIn\Download.avi` |

## Live project found: *Duke Realty — Building 1835*

Three floors, 85 HVAC devices, camera dashboard, backups retained 2019-10 → 2026-10.

```
vi/
├── _default.vbd         site landing page (2 JPEGs embedded)
├── Training.vbd
├── Link Document.vbd
├── Cameras/             camera dashboard subsystem (2 JPEGs embedded)
├── Floor 1 (Unit 1)/    _default.sub + PIU-1-xx + RTU + VAV-1-xx .app devices
├── Floor 2 (Unit 2)/    same shape as Floor 1
├── Floor 3 (Unit 3)/    same shape
├── HVAC Units/          Rooftop Unit per floor
└── Miscellaneous/       SMC300 / UHC300 / System Status
```

## OLE compound-doc format

`.vbd` / `.app` / `.sub` / `.fpt` files are CFBF compound documents (`D0 CF 11 E0 A1 B1 1A E1`). Two primary streams per file:

- `CONTENTS` — metadata (name, version, parent IDs)
- `Graphics` — a length-prefixed sequence of serialized objects (`CHyperFrameObject`, `CDeviceObject`, text, lines, embedded bitmaps)

Embedded raster images (PNG/JPEG/GIF) are stored inline in the object stream and extractable by scanning for magic bytes. The floor plan itself is **not** a stored bitmap — it is composed from rectangles, labels, and positioned OCX control instances. Reproducing it in the new app requires parsing the serialized object graph (future work).

## Database BLOB inventory

Enumerated via `sys.columns` on the running SQL Server `(local)\CIRCON`. **No floor-plan or graphic storage in the DB** — site graphics live on disk under `C:\NI\` only. The only image BLOBs are user photos / signatures in the access-control tables (`access_users_images.photo`, `access_users_images.signature`).

## PE resource extraction (ActiveX OCX, 44 files)

Walked via `resedit` (Node). Totals across all 44 OCXs:

| Resource type | Count |
|---|---:|
| RT_BITMAP → `.bmp` (DIB + synthesized file header) | 217 |
| RT_GROUP_ICON → `.ico` (reassembled from RT_ICON entries) | 147 |
| RT_GROUP_CURSOR → `.cur` | 13 |
| RT_RCDATA (TeeChart palettes, misc) | 100 |
| RT_STRING tables (catalogued, not extracted as files) | — |

Standouts: `TeeChart5.ocx` ships 134 bitmaps + 98 RCDATA, `ccgraph.ocx` 20 bitmaps + 4 cursors, `CirconDbUI.ocx` 6 bitmaps + 29 icons, `CirconNavigate.ocx` 17 icons, `lnsui.ocx` 38 icons, `ScheduleUI.ocx` 7 icons + 6 bitmaps, `obselect.ocx` 17 icons.

## Managed .NET resource extraction (vilns32/nilns32/ailns32 + Circon*.dll)

Scanned the 24 copied managed assemblies for PNG/JPEG/GIF magic bytes within their embedded `.resources` streams (full BinaryResourceReader parsing deferred — magic-carve is a pragmatic MVP that recovers the pixel data):

| File | Hits |
|---|---:|
| `vilns32.exe` | 72 PNGs (main app form toolbars, menu icons, splash) |
| `nilns32.exe` | 39 PNGs |
| `ailns32.exe` | 21 PNGs |
| `AlarmView.dll` | 2 PNGs |
| `CirconDataForms.dll` | 14 PNGs + 3 JPEGs |
| `CirconDataLinqForms.dll` | 78 PNGs |
| `CirconForms.dll` | 3 PNGs |
| `CirconGfxForms.dll` | 13 PNGs |
| `CirconLnsForms.dll` | 45 PNGs + 6 GIFs |
| ... (rest) | ... |

Total across managed: **~296 PNG + 7 GIF + 3 JPEG**.

## Grand totals

| Category | Count |
|---|---:|
| Static icons from `BIN\Images\` | 68 |
| Carved from live site OLE docs (`.vbd`/`.app`/`.sub`) | 466 |
| PE resource walk of OCX files (bitmaps/icons/cursors) | 377 |
| Managed assembly `.resources` carve (PNG/GIF/JPEG) | 306 |
| Extra OCX-level carved JPEGs | 6 |
| **Total raster assets recovered** | **~1,223** |

Plus 100 TeeChart RCDATA binaries (chart palettes — native format, kept for reference).

## What still needs doing

1. **BinaryResourceReader parser** — recovers resource *names* alongside the raw bytes so we can map each extracted PNG back to its logical identifier (e.g. `toolbar_save.png` vs anonymous offset). Format is well-documented (System.Resources.ResourceReader).
2. **OLE serialization decoder** — needed to re-render floor plans faithfully in the Avalonia UI rather than just embedding the carved raster snapshots.
3. **Silverlight XAP XAML walk** — the two XAPs contain compiled XAML that defines the web-side page structure; pulling layout + referenced image URIs is a separate pass.
4. **TeeChart RCDATA format** — if we need the chart cursor/palette data for faithful trend-log rendering, reverse the proprietary RCDATA format or replace with a modern charting library.

None of these block app-structure progress; they inform the UI port.
