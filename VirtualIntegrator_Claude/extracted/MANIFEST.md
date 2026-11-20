# Visual Integrator asset extraction — local manifest

Extracted on this Windows 7 workstation at `C:\Users\shackleford\Desktop\VirtualIntegrator_Claude\extracted\`.

All files are **copies**. The originals under `C:\Program Files (x86)\Circon\` and `C:\NI\` were not touched.

## Folders

```
extracted/
├── images/                 68 icons from BIN\Images\        (fans, dampers, VAVs, LEDs, Circon Building)
├── templates/               9 OLE compound-doc templates     (Blank/Shadow .app .fpt .sub .vbd)
├── silverlight/             WebGraphics.xap + CirconGfxProject.xap + unpacked DLLs
├── reports/                15 Crystal Reports .rpt templates
├── ocx/                    44 ActiveX OCX controls           (embedded bitmaps extractable via resource tooling)
├── media/                   Download.avi tutorial video
├── site_data/              439 MB — Duke Realty / Building 1835 live project
│   ├── Duke Realty/Building 1835/vi/      7 .vbd + 85 .app + 4 .sub (floor plans, HVAC, cameras, misc)
│   ├── Duke Realty/Building 1835/backup/  28 quarterly ZIP backups 2019-10 → 2026-10
│   └── backup_latest_unpacked/            2026-10-01 backup unpacked for reference
├── carved_images/          466 bitmaps carved out of the OLE site files
│                           415 GIF (animated HVAC widgets) + 47 PNG + 4 JPG
├── ole_extracted_sample/    streams from a sample .vbd (format research)
├── ole_extracted_floor1/    streams from Floor 1 _default.sub
├── managed/                 24 copied managed assemblies (vilns32/nilns32/ailns32 + Circon*.dll + AlarmView + EventTranslate)
├── managed_carved/          306 images carved from .NET .resources streams (PNG/GIF/JPEG) — anonymous offsets
├── managed_named/           393 images from .NET .resources with their logical names (e.g. `pctDoor.Image.png`)
├── pe_resources/            521 artifacts from OCX resource walk: 217 BMP + 147 ICO + 13 CUR + 100 RCDATA + 44 INDEX.json
├── ocx_carved/              6 extra JPEGs carved from OCX body (not in .rsrc)
├── silverlight_carved/      5 PNGs carved from inner DLLs of the two XAPs
├── object_graphs/           79 per-file JSON catalogs of object class names + labels in .vbd/.sub/.app Graphics streams
├── rcdata_carved/           (empty — RCDATA bins have no embedded PNG/JPEG)
└── manifest/
    ├── extract_ole.js              Node: dump OLE compound-doc streams
    ├── scan_images.js              Node: carve PNG/JPEG/GIF by magic bytes
    ├── extract_pe_resources.js     Node: walk PE .rsrc (bitmaps/icons/cursors/RCDATA)
    ├── extract_resources_named.js  Node: parse .NET ResourceReader v2 — names + bytes
    ├── walk_ole_objects.js         Node: walk OLE Graphics stream, catalog object class names
    └── find_blobs.sql              sqlcmd query: catalog varbinary/image columns
```

## Summary by asset type

| Category | Count | Where |
|---|---:|---|
| HVAC icons (static)                        | 68  | `images/` (copied verbatim from `BIN\Images\`) |
| Animated / state GIFs (device widgets)     | 415 | `carved_images/` (carved from `.app` / `.sub`) |
| Static PNG assets inside OLE docs          | 47  | `carved_images/` |
| JPEG hero / page images                    | 4   | `carved_images/` |
| OLE template files                         | 9   | `templates/` |
| Live project OLE files (.vbd/.app/.sub)    | 96  | `site_data/Duke Realty/Building 1835/vi/` |
| Crystal Reports templates                  | 15  | `reports/` |
| ActiveX OCX controls                       | 44  | `ocx/` |
| Silverlight XAP (compiled web UI)          | 2   | `silverlight/` |
| Site backup ZIPs                           | 28  | `site_data/.../backup/` |
| Media files                                | 1   | `media/Download.avi` |

## Floor plan / live site content

Only live site found on this workstation: **Duke Realty — Building 1835** (3 floors, 85 devices).

```
vi/
├── _default.vbd                    site landing page         (2 embedded JPEGs carved)
├── Training.vbd                    training page
├── Link Document.vbd               cross-site link
├── Cameras/
│   ├── _default.vbd                camera dashboard          (2 embedded JPEGs carved)
│   └── Cameras Subpage.vbd
├── Floor 1 (Unit 1)/               _default.sub + 28 device .app files (PIU-1-xx, RTU, VAV-1-xx)
├── Floor 2 (Unit 2)/               floor 2 subsystem + devices
├── Floor 3 (Unit 3)/               floor 3 subsystem + devices
├── HVAC Units/
│   ├── _default.vbd                HVAC roll-up page
│   └── Rooftop Unit [1-3] (Floor [1-3]).app  + matching subfolders
└── Miscellaneous/                  SMC300, UHC300, system status
```

## Database BLOB inventory (SQL Server `circon`)

Enumerated via `manifest/find_blobs.sql`. **No floor-plan / graphic columns found** — site graphics live on disk under `C:\NI\` only. Image BLOBs in the DB are user-related (access control):

| Column                                         | Type       | Size |
|------------------------------------------------|------------|------|
| `access_users_images.photo`                    | image      | BLOB |
| `access_users_images.signature`                | image      | BLOB |
| `access_cards.card`, `access_device_cards.card`| binary(16) | card data (not image) |
| `trend_values.data`, `device_events.data`      | binary     | telemetry blobs |
| `users.md5hash_password`                       | binary(16) | legacy MD5 hash |
| `sites.slta_identifier`, `sites.ilon_md5_key`  | binary     | hardware IDs |

## OLE compound-doc format notes (.vbd / .app / .sub)

- File header is the standard CFBF magic (`D0 CF 11 E0 A1 B1 1A E1`).
- Two primary streams: `CONTENTS` (metadata) and `Graphics` (serialized object graph).
- The `Graphics` stream is NOT a raw image; it is a sequence of length-prefixed object records (`"Static Text"`, `"CHyperFrameObject"`, `"CDeviceObject"`, etc.).
- Embedded bitmap resources (PNG / JPEG / GIF) are encoded inline within the object stream and were extracted by carving on magic bytes — see `manifest/scan_images.js`.
- The floor subsystem `.sub` files have no carved images because the floor plan itself is composed from rectangles, text, and positioned OCX control instances — rendered live, not stored as a pre-baked bitmap. Reproducing the floor plan render requires parsing the serialized object graph.

## Reproducing this extraction

```
cd extracted
# 1. OLE stream dump (per-file)
mkdir -p ole_out
node manifest/extract_ole.js "<input.vbd>" ole_out/

# 2. Carve PNG/JPEG/GIF
mkdir -p carved_out
node manifest/scan_images.js "<input.vbd>" carved_out/
```

Requires `npm install cfb` locally for `extract_ole.js`.

## Repo cross-reference

A summary lives in the project repo at `docs/legacy-research/asset-inventory.md`. The extracted files themselves stay **local-only** — never committed (vendor binaries / proprietary content).
