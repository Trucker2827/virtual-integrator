# LonTalk reimplementation plan

## 1. The problem

The original Visual Integrator depends on Echelon's **LNS** (LonWorks Network Services) runtime via `Echelon.LNS.Interop 3.25` (COM, Windows-only). LNS in turn sits on **OpenLDV** (the driver/transport layer) and the **LonTalk** protocol stack (ANSI/CEA-709.1).

Echelon no longer exists as an independent company (acquired by Adesto → Dialog → Renesas); new LNS SDK licenses are not obtainable, and LNS was never ported to macOS or Linux.

To meet the cross-platform goal, the Echelon dependency has to be replaced. This document plans how.

## 2. What the original actually uses

From the ILSpy pass of `vilns32.exe` and the `CirconLns*.dll` assemblies, the surface area the application touches is narrower than "all of LNS". It calls into LNS for:

| Capability | Where used | Replacement effort |
|---|---|---|
| Open an LNS database / network | `Circon.Lns.*` | New — DB schema in Postgres |
| Enumerate channels, subnets, devices | Browser / open-site dialogs | New — DB query |
| Download / commission a device (install) | Device install workflow | LonTalk NM commands |
| Read/write SNVT/UCPT values | Runtime monitoring, `CirconNvt.dll` | LonTalk NV update/fetch |
| Subscribe to NV changes (monitor sets) | Alarm/event pipeline | LonTalk NV reports + local pub/sub |
| Parse LonMark Device Resource Files (DRF/XIF) | `CirconLdrf.dll` | New — XIF/DRF parser |
| Manage bindings (NV connections) | Visual Integrator editor | LonTalk NM message binding + local DB |
| xDriver (remote LNS over IP) | `X.Circon.*` prefix | CEA-852 (LonTalk/IP) |
| LNS plug-in registry (`SOFTWARE\LonWorks\LCA\Plug-Ins`) | `Circon.Lns.LnsPluginRegistryKeys` | Out of scope for v1 |

The app does **not** use the full LNS plug-in ecosystem internally — it enumerates plug-ins but delegates invocation to Windows COM shells. For a faithful v1, dropping plug-in hosting is acceptable (it can be reintroduced as an extension API later).

## 3. Strategic options

### Option A — Full LNS equivalent, full protocol stack, all physical media
Reimplement ANSI/CEA-709.1 (LonTalk), 709.2 (TP/FT-10), 709.3 (PL), 852 (IP), the Neuron management layer, and an LNS-equivalent database. **Years of work, deep protocol expertise, paywalled specs.** Rejected as scope.

### Option B — LonTalk + lean LNS equivalent, IP transport only
Reimplement enough of LonTalk to encode/decode APDUs and address packets. Use **CEA-852 (LonTalk/IP)** as the only transport. Build a Postgres-backed database to replace LNS's role (device inventory, bindings, monitor sets). Drop plug-in hosting from v1. **Tractable; 6–18 months of focused work.** This is the recommendation.

### Option C — IP-only, no reimplementation, use an existing bridge
Depend on an iLON SmartServer (or equivalent) as the only interface to the LON network. Talk to it over its documented REST/SOAP API. **Fast; requires users to have an iLON on the network.** Not "faithful" — Circon users without an iLON can't use it. Keep as a fallback if B stalls.

### Option D — Platform-split
LNS COM on Windows, CEA-852 reimplementation on macOS/Linux. **Doubles maintenance surface, splits testing, violates the "same software everywhere" goal.** Rejected.

## 4. Recommendation: Option B

**Rationale:**
- Modern LonWorks deployments are overwhelmingly IP-bridged. TP/FT-10 and PL still exist in legacy sites, but new devices and new sites almost always have an iLON/i.LON/SmartServer at the edge translating to CEA-852.
- CEA-852 is fully userspace: UDP packets to a known port (1628). No kernel driver, no USB chipset work, no per-OS pain. **Cross-platform for free.**
- The LonTalk APDU layer above the transport is the same regardless of medium — writing it once pays off even if TP/FT-10 is added later.
- Dropping TP/FT-10 for v1 removes the single largest driver-side risk (U10/U20 USB adapters, Echelon's native dongles).

**Non-goals for v1:**
- Native TP/FT-10 or PL transports
- USB adapter support (U10, U20, PCC-10)
- Full LNS plug-in hosting
- Neuron C compiler / app download of custom firmware images (we commission existing devices only, not reprogram them)

## 5. Phased plan

### Phase 0 — Research & setup (2–4 weeks)
1. Acquire specs:
   - **ANSI/CEA-709.1-D** (LonTalk) — paywalled, ~$200
   - **ANSI/CEA-852-D** (LonTalk/IP tunnel) — paywalled, ~$150
   - Echelon's **Neuron C Reference Guide** and **LonMark SNVT/SCPT Master List** (free, mirrors exist)
2. Survey prior art:
   - `OpenLCB` / NMRA LCC — CAN-based cousin; architectural inspiration only
   - `lontalkstack` and any GitHub LonTalk projects (audit licensing)
   - LonMark International resource set (XIF files, DRF format spec)
3. Pick a target device to test against:
   - **iLON 100 / SmartServer** — cheap used on eBay (~$50–$200); acts as our CEA-852 peer
   - Or: Loytec L-IP or similar IP router
4. Set up a bench: one IP router + one LonTalk device (e.g., a thermostat or a dev-kit Neuron evaluation board).
5. **Deliverable:** a one-page spec summary (packet layout, addressing model, service types) checked into this folder.

### Phase 1 — LonTalk/IP MVP (3–6 months)
Repo module: `src/LonTalk/` (.NET 8, pure managed, no P/Invoke).

1. CEA-852 framing
   - UDP listener on port 1628
   - Encapsulation header (CN, SN, device type, session)
   - Config-server registration handshake
2. LonTalk PDU encoding (CEA-709.1 §7)
   - Addressing: domain / subnet / node / group / neuron-ID
   - Service types: UNACKD, ACKD, REQUEST, REPEATED
3. Network-management messages
   - `wink`, `query status`, `query NV config`, `update NV config`
4. Network-variable traffic
   - `NV_UPDATE` (poll-response, event-driven)
   - `NV_FETCH`
5. Minimal in-memory device/NV model
6. CLI tool: `vi lon scan`, `vi lon read <addr>`, `vi lon write <addr> <value>`

**Success criterion:** From a macOS/Linux box with no Echelon software installed, read and write an SNVT on a real LonTalk device via an iLON router.

### Phase 2 — Database & binding model (2–3 months)
Repo module: `src/Lon.Store/` + Postgres schema.

1. Port the relevant slice of the LNS data model to Postgres:
   - `domains`, `subnets`, `nodes`
   - `channels`, `routers`
   - `functional_blocks`, `network_variables`, `config_properties`
   - `bindings` (source NV, destination NV(s), service type, retry, etc.)
   - `monitor_sets`, `monitor_subscriptions`
2. EF Core migrations; seed with an empty "default" domain for new projects
3. Binding engine: when a binding is created, issue the NM messages to both endpoints to update their NV config tables
4. Monitor engine: maintain subscriptions; surface NV updates via an in-process event bus

**Success criterion:** Create a domain in the UI (even a CLI stub), import an XIF, commission a device onto it, bind two NVs, observe values flow in a live monitor view.

### Phase 3 — Resource files & SNVT library (2–3 months, overlaps Phase 2)
Repo module: `src/Lon.Resources/`.

1. XIF (external interface file) parser — replaces the vendor's device catalog
2. DRF (device resource file) parser — replaces `CirconLdrf.dll`
3. SNVT master-list ingestion — all ~200 standard types with scale/offset/units
4. UCPT / UNVT user type support (declared in the XIF)
5. Value marshalling: raw bytes ↔ typed values ↔ display strings

### Phase 4 — Interop hardening & additional transports (deferred, 6+ months)
- TP/FT-10 via a USB adapter — requires picking a Linux-capable chipset or an IP-bridge
- PL (power line) — deferred indefinitely
- LNS plug-in extension API (own format, not Echelon's `.pext`)

## 6. Known unknowns / risks

| # | Risk | Mitigation |
|---|---|---|
| 1 | CEA-709.1 / 852 specs are paywalled and dense | Budget for them in Phase 0; cross-reference with Echelon's old public tech notes |
| 2 | Real-world devices diverge from spec in small ways | Build against ≥ 2 vendors' devices in Phase 1 to catch quirks early |
| 3 | SNVT master list evolves; some SNVTs are deprecated | Pin a snapshot; version the SNVT library inside the repo |
| 4 | Authentication (LonTalk §9) — most sites run with auth disabled, but some don't | Phase 2: implement auth key handling behind a feature flag |
| 5 | iLON config-server protocol has vendor-specific wrinkles | Test against Echelon iLON AND a Loytec router to flush out assumptions |
| 6 | Legal: Echelon trademarks, LonMark certification | Call it "LonTalk/IP-compatible" not "LonWorks™"; do not pursue LonMark certification initially |
| 7 | Patents on LonTalk have mostly expired (filed late 1980s, 20-yr term) | Verify with a patent search in Phase 0 before writing code |

## 7. Out-of-scope but worth noting

- **Faithful UI** of Visual Integrator remains the *application* goal; this document is only about replacing the LonWorks substrate underneath.
- **Database schema v2.88** of the original Circon app is the subject of a separate doc (`database-schema.md`, to be written).
- **OPC XML-DA bridge** can be built against our new in-process NV model without depending on LonTalk internals — covered in `opc-xml-da.md` (to be written).

## 8. Open questions for the project owner

1. Do we have budget for the CEA specs (~$350 total) and a used iLON (~$100) in Phase 0?
2. Is an iLON-required deployment model acceptable for v1, or must we support direct TP/FT-10 at v1?
3. Target site profile: how many devices per network? (affects DB design, discovery latency targets)
4. Any specific LonTalk device vendors we need day-one compat with?
