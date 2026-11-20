# Virtual Integrator

A modern, cross-platform (Windows, macOS, Linux) reimplementation of the Circon Visual Integrator — a LonWorks / building-automation integration tool originally released decades ago on Windows-only .NET + Echelon LNS.

## Status

Skeleton in place. `dotnet build && dotnet test` passes on Linux/macOS/Windows CI. See [docs/legacy-research/](docs/legacy-research/) for reverse-engineering notes on the original software that inform this rewrite.

## Layout

```
src/
  VirtualIntegrator.Core/         — domain model + abstractions (ILonChannel, LonAddress, ...)
  VirtualIntegrator.Data/         — EF Core + Postgres (domains, subnets, nodes, NVs, bindings)
  VirtualIntegrator.Lon.OpenLdv/  — Windows OpenLDV (U10/U20/U60) ILonChannel implementation
  VirtualIntegrator.Ui/           — Avalonia desktop shell (Windows/macOS/Linux)
  VirtualIntegrator.Cli/          — `vi` CLI (System.CommandLine)
tests/
  VirtualIntegrator.Core.Tests/
docs/legacy-research/             — reverse-engineering notes (LonTalk plan, Phase 0 findings, etc.)
```

Pending modules: `VirtualIntegrator.Lon.Ip852` (CEA-852 cross-platform transport), `VirtualIntegrator.OpcServer` (OPC XML-DA SOAP endpoint).

## Build

```
dotnet build
dotnet test
```

Requires the .NET 8 SDK. The dev workstation holding the original Circon install is Windows 7, which cannot host the .NET 8 SDK — development happens on Windows 10+, macOS, or Linux. CI (GitHub Actions) builds on all three.

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
