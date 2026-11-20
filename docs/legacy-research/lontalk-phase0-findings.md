# LonTalk Phase 0 — research findings

Companion to [`lontalk-protocol.md`](lontalk-protocol.md). Compressed facts, no planning.

## Budget revision

Original estimate was ~$450 (specs + hardware). Actual need is **~$85** (one used i.LON 100 e3). Specs are available free via legitimate public mirrors; ANSI purchase is unnecessary unless legal review insists.

## Reference implementations (study, don't copy)

| Repo | Lang | License | Scope | Notes |
|---|---|---|---|---|
| `izot/lon-stack-ex` | C/C++/Python | MIT | Full stack + IP-852 | EnOcean's own open-sourced Echelon stack. Primary reference. |
| `izot/lon-stack-dx` | C | MIT | MCU-targeted stack | Updated Mar 2026. |
| `izot/shortstack` | C | MIT | Host-side API + Micro Server | Shows host/Neuron split. |
| `LonMarkTech/lon-stack-dx-lmi` | C | MIT | LonMark fork | Lightly maintained. |
| `karlhiramoto/lon4linux` | C | — | Linux kernel framing (2000–2001) | Historical only. |
| Wireshark CEA-852 dissector + Gesytec GPL extension | C | GPL | Wire-format decoding | Invaluable for debugging. |
| `martonmiklos/dialog_u60_linux_driver` | C | GPL | U60 USB kernel driver | Only relevant if TP/FT-10 via USB is added later. |

OpenLCB/NMRA LCC is a CAN-based cousin — useful only as API-design inspiration, not protocol reference.

## Specifications

| Document | Status | Source |
|---|---|---|
| **LonTalk Protocol Specification v3.0 (Echelon)** — predecessor to CEA-709.1, functionally complete | **Free** | scadahacker.com, icscsi.org |
| ANSI/CTA-709.1-D (2014) | $376 | ANSI/Accuris webstore |
| ANSI/CTA-852-C (2014) | ~$250 | ANSI webstore |
| ISO/IEC 14908 parts 1–4 | ~€283 + $143 + $127 + $258 | SFS / Accuris |
| SNVT & SCPT Master List v15 | **Free** | lonmark.org |
| Enumeration Master List | **Free** | lonmark.org |
| Neuron C Reference Guide | **Free** | downloads.echelon.com, renesas.com |
| LonMark XIF File Reference v4.501 | **Free** | lonmark.org |
| LonMark Application-Layer Interop v3.4 | **Free** | lonmark.org |
| OpenLDV Programmer's Guide | **Free** | downloads.echelon.com |

**Decision:** use scadahacker v3.0 + lonmark.org free docs as the primary references. No spec purchase in Phase 0.

## Patent status (verified Nov 2026)

- Core LonTalk/Neuron patents (filed 1988–1991): **expired**. Representative: US4918690 ("Network and intelligent cell"), marked "Expired – Lifetime" on Google Patents.
- US5297143 (reliable multicast), US6654750 (network management DB): all beyond 20-year term.
- CEA-852 (2002) era filings: at or past 20 years.
- Current assignee: Echelon → Adesto (2018) → Dialog (2020) → Renesas (2021). No known enforcement activity.
- **Trademarks** (LonWorks, LonTalk, Neuron, SNVT, LonMark) remain live under Renesas / LonMark International.

**Action:** describe the stack as "CEA-709 / ISO/IEC 14908 compatible", never as "LonWorks™". Already noted in root README non-affiliation clause.

## Hardware peer — eBay snapshot (late 2026)

| Device | Price | Brick risk | Recommendation |
|---|---|---|---|
| **i.LON 100 e3 / e3 Plus (72101R)** | $25–$85 | Low — no cloud auth | ✅ Primary pick |
| i.LON 600 (CNIP router) | $200–$500 | Low | Great if found near $200 |
| i.LON SmartServer 1/2 | $90–$300 | Medium — activation-gated | Only if IoT features needed |
| SmartServer IoT (SS3) | varies | **High** — cloud-dependent | ❌ Avoid |
| Loytec L-IP | $500–$1000 | None | Over budget |
| Loytec NIC852 (software-based) | $140–€230 | None | Decent software peer |
| Gesytec Easylon USB Interface+ | ~$95 | None | TP/FT-10 only — pair with router |

**Also:** compile `izot/lon-stack-ex` on a Linux VM as a zero-cost second CEA-852 peer.

## Pre-Phase-1 gotchas (sourced to spec v3 and Claroty Team82 research)

1. Unconfigured state wipes auth keys — state machine must handle this explicitly.
2. Failed-auth messages still reach the application with a flag — don't assume drop.
3. ACKed service group size capped at 64 (6-bit member ID); larger groups must use UnackedRpt.
4. A node that sent in cycle N must re-contend (not reuse priority slot) in cycle N+1.
5. **CEA-852 "HMAC" is a non-standard MD5 construction, not RFC 2104 HMAC-MD5.** Standard library won't interop.
6. CNIP protocol codes: 0x01 = IP-852 (public), 0x03 = RNI, 0x04 = LPA. RNI/LPA need Claroty writeups to implement.
7. Config-server handshake required before peers accept packets — CS client is Phase 1 scope, not Phase 2.
8. UDP 1628 (normal) vs 1629 (urgent) are separate traffic classes, not duplicates.
9. `UnackedRpt` is a distinct wire message with transaction-ID dedup, not an app-layer loop.
10. SmartServer 2 persists IP-852 state in flash; bench requires explicit config reset.
11. Address-matching algorithm is "implementation-specific" — test against izot AND a real i.LON.

## Modern technical write-ups worth reading

- Claroty Team82, "Examining the Legacy BMS LonTalk Protocol"
- Claroty Team82, "The Risky Road: Exploring the CEA-852 Standard"
- Claroty Team82, "Exploiting EnOcean SmartServer"
- LonMark International, "Open-Source LON Stacks" (2025 whitepaper)
- Naval Postgraduate School MS thesis, DTIC ADA342308 (LonWorks for networked robots)
