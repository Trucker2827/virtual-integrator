# Virtual Integrator

A modern, cross-platform (Windows, macOS, Linux) reimplementation of the Circon Visual Integrator — a LonWorks / building-automation integration tool originally released decades ago on Windows-only .NET + Echelon LNS.

## Status

Early planning. No code yet. See [docs/legacy-research/](docs/legacy-research/) for reverse-engineering notes on the original software that inform this rewrite.

## Goals

- **Faithful** reproduction of the original document model, workflows, and wire-level behaviour
- **Cross-platform** — Windows, macOS, Linux
- **Open** — no proprietary runtime dependencies; LonTalk stack reimplemented from the open ANSI/CEA-709.1 standard
- **Modern** — current .NET, current crypto (no MD5/Blowfish), current UI toolkit

## Planned stack

| Layer | Original | Planned |
|---|---|---|
| Runtime | .NET Framework 4.6.2 (Windows) | .NET 8 (cross-platform) |
| UI | WinForms + ActiveX (SandDock, TeeChart, ILOG) | Avalonia UI |
| Database | SQL Server + LINQ-to-SQL | PostgreSQL + EF Core |
| OPC bridge | OPC XML-DA 1.0 SOAP | ASP.NET Core SOAP endpoint |
| LonWorks | Echelon LNS COM (`Echelon.LNS.Interop`) | Open LonTalk stack (ANSI/CEA-709.1) |
| Password hashing | Unsalted MD5 | Argon2id |
| Field encryption | Blowfish | AES-GCM / libsodium |
| Licensing | wyDay TurboActivate | None (open source) |

## License

MIT — see [LICENSE](LICENSE).

## Non-affiliation

This project is an independent reimplementation. It is not affiliated with, endorsed by, or derived from the original Circon Systems Corp. software or its successors. Trademarks belong to their respective owners.
