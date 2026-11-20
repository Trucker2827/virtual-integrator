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

## What still needs doing

1. **ActiveX bitmap extraction** — the 44 OCXs each have embedded icons/bitmaps in their Windows resource section. Needs a native resource walker (`wrestool`, `Resource Hacker`, or a small C# tool using `PEReader` + `ManagedPEReader`).
2. **Managed .NET resource extraction** — `vilns32.exe` and the `Circon*.dll` assemblies have 57+ `.resources` streams. The ILSpy pass earlier dumped names only; to pull the actual bytes needs `ildasm` or a small `Mono.Cecil` tool.
3. **OLE serialization decoder** — needed to re-render floor plans faithfully in the Avalonia UI rather than just embedding the carved raster snapshots.
4. **Silverlight XAP XAML walk** — the two XAPs contain compiled XAML that defines the web-side page structure; pulling layout + referenced image URIs is a separate pass.

These are sequenced after the current loop (CLI wiring, LonTalk APDU, Postgres migration) — none of them block app-structure progress.
