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

## Named .NET resource extraction (BinaryResourceReader)

Parsed each managed assembly for `0xBEEFCACE` resource streams, walked the name table, and dumped each entry's PNG/JPEG/GIF/BMP payload with its **logical resource name**:

| Assembly | Named extractions |
|---|---:|
| `vilns32.exe` | 105 (105 PNG) |
| `nilns32.exe` | 42 (41 PNG + 1 BMP) |
| `ailns32.exe` | 36 (20 PNG, 3 GIF, 13 BMP) |
| `AlarmView.dll` | 2 PNG |
| `CirconDataForms.dll` | 19 (18 PNG + 1 BMP) |
| `CirconDataLinqForms.dll` | 92 (88 PNG + 4 BMP) |
| `CirconForms.dll` | 7 (3 BMP + 4 PNG) |
| `CirconGfxForms.dll` | 15 PNG |
| `CirconLnsForms.dll` | 75 (63 PNG, 3 BMP, 9 GIF) |

Total: **393** named images (356 PNG + 25 BMP + 12 GIF). Names map directly to the Windows Forms designer identifiers — e.g. `pctDoor.Image.png`, `btnViewEvents.Image.png`, `$this.Icon.png`, `imgList.ImageStream.1.png`. Entry bounds are respected so an image found inside the wrong resource (bool/enum entries that happen to be near a PNG) does not steal another entry's name.

## Silverlight XAP image walk

Carved PNG/JPEG/GIF from each DLL inside the two Silverlight packages:

| Package | Hits |
|---|---:|
| `WebGraphics.xap` (operator web client) | 4 PNG across inner DLLs |
| `CirconGfxProject.xap` (web project template) | 1 PNG |

Structural XAML inside the XAPs is compiled to BAML and embedded in the DLLs; `AppManifest.xaml` is the only loose XAML file (lists assembly refs, no graphic resources).

## OLE serialization — object-graph catalog

Walked the `Graphics` stream of every `.vbd` / `.sub` / `.app` / `.fpt` in the Duke Realty site (79 files that have a Graphics stream; 3 don't). Scanned for all printable-ASCII runs 4–64 bytes, filtered to MFC-style class names (`C[A-Z][A-Za-z0-9]{2,}`).

**Full object-class catalog** (count = how many site files use the class):

| Class | Files | Role |
|---|---:|---|
| `CHyperFrameOb` | 79 | Hyperframe (page container) |
| `CHyperViewOb` | 79 | Hyperframe view |
| `CGenericOcx` | 78 | Embedded ActiveX / OCX instance |
| `CRectOb` | 74 | Rectangle / box primitive |
| `CBtnRteOb` | 2 | Button route |

Plus 7 low-count artifacts (`CDCT`, `CLuZ`, `CSU9`, `CX2y`, `CZ9Mb4X`, `CCJWmv`, `CUnh`) that appear 1–20 times — likely encrypted/encoded blobs or false-positive ASCII runs inside binary data, not real class names.

The real Circon graphic schema is therefore composed of just **5 primitive types**. A future object-graph decoder (sized for the new Avalonia UI port) only needs to handle these five + the inline bitmaps we already carved. The decoder work is tracked as app-side scope, not legacy-research scope.

## Grand totals

| Category | Count |
|---|---:|
| Static icons from `BIN\Images\` | 68 |
| Carved from live site OLE docs | 466 |
| PE resource walk of OCX files | 377 |
| Named `.NET resources` (with logical names) | 393 |
| Silverlight XAP inner-DLL carves | 5 |
| Extra OCX-body carves | 6 |
| **Total raster assets recovered** | **~1,315** |

## Deferred

- **Full OLE object-graph decoder** — recovers full floor-plan geometry (rectangles, OCX positions, bindings) so the new UI can re-render the page vector-exactly rather than embedding the carved raster snapshots. Scheduled under the Avalonia UI module, not legacy-research.
- **TeeChart RCDATA format** — only matters if we need pixel-exact trend-log rendering using the original palettes. Likely replaced by a modern charting library in the port, so this stays unreversed.

No remaining asset work blocks app-structure progress.
