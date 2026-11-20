# Legacy research notes

Reverse-engineering notes on the original Circon Visual Integrator, used to inform the faithful rewrite.

The raw reflection dumps, strings scan, and system map live outside this repo (local-only, not committed) at `C:\Users\shackleford\Desktop\VirtualIntegrator_Claude\` on the dev workstation. This directory in the repo holds only derived, hand-written analysis — never original vendor binaries or decompiled source.

Planned contents:

- `document-model.md` — mapping of original `IXxxDocument` types to new domain model
- `database-schema.md` — original schema (v2.88) → planned PostgreSQL schema
- `opc-xml-da.md` — SOAP endpoint contract to preserve
- `lontalk-protocol.md` — notes on the ANSI/CEA-709.1 reimplementation path
