# Agent Guide for 2sxc

## Overview
2sxc is a modular CMS and app engine for DNN (DotNetNuke) and Oqtane, spanning .NET Framework and .NET Core. The repo hosts core runtime, DNN/Oqtane integrations, Razor rendering, WebApi layers, and build assets. For high-level context start with [readme.md](../readme.md).

## Repository Layout
- Absolute root: `A:/2sxc/2sxc`
- Solutions: `2sxc Multi-Target.sln` (primary) plus platform-specific `.sln` variants.
- Core runtime: `Src/Sxc/ToSic.Sxc*` with unit tests under `Src/Sxc/ToSic.Sxc*Tests/`.
- DNN integration: `Src/Dnn/ToSic.Sxc.Dnn*`; tests in `Src/Dnn.Tests/` and `Src/Dnn/ToSic.Sxc.Dnn*Tests/`.
- Oqtane integration: `Src/Oqtane/ToSic.Sxc.Oqt.*` with tests in `Src/Oqtane.Tests/`.
- WebApi endpoints: `Src/Dnn/ToSic.Sxc.WebApi/`, `Src/Dnn/ToSic.Sxc.Dnn.WebApi/`, `Src/Oqtane/ToSic.Sxc.Oqt.Server/WebApi/`.
- Razor engines: `Src/Razor/ToSic.Sxc.Razor/`, `Src/Dnn/ToSic.Sxc.Dnn.Razor/`.
- Data & assets: `Src/Data/`, `Src/Data-Dnn/`, `Src/assets-raw/`.
- Dependencies: NuGet cache under `Src/packages/`; other binaries under `Dependencies/`.

## External Repositories
- `../eav-server` (absolute `A:/2sxc/eav-server`): dynamic data layer (`EAV System.sln`); referenced by the main solution but lives outside this repo—do not relocate files.
- `../2sxc-sources`, `../2sxc-ui`, `../eav-ui`: provide compiled client assets and UIs.
- `../oqtane/oqtane.framework`: Oqtane platform (`Oqtane.sln`), re-used via solution references.
- Local DNN install resides in `A:/2sxc/2sxc-dnn/Website`; treated as external dependency.

## Coding Principles
- Maintain separation of concerns and SOLID design.
- Rely on dependency injection; avoid static coupling.
- Prefer functional, side-effect-free helpers.
- Prefer immutability where feasible.
- Enforce nullable reference types; check for nulls explicitly where needed.
- Keep cross-platform code neutral; isolate DNN- or Oqtane-specific logic to their folders.
- Keep methods small and focused; use descriptive names.
- Prefer latest C# features where sensible.
- Nullable enabled; explicit null checks are common.
- Security best practices.
- Protect against concurrency issues and racing conditions.
- Optimize for performance and memory efficiency.
- Leverage async/await for I/O-bound operations.
- Logging using internal library.
- Comment public APIs with XML docs. 
- Comment intention, why, common pitfalls.
- Write unit tests using xUnit; test projects are in `*Tests/` folders.

## Build & Test
- Primary build targets: `Src/Dnn/ToSic.Sxc.Dnn/ToSic.Sxc.Dnn.csproj` (DNN) and `Src/Oqtane/ToSic.Sxc.Oqt.Package/ToSic.Sxc.Oqt.Package.csproj` (Oqtane).
- Restore NuGet packages before building; some dependencies ship in `Dependencies/`.
- Run unit tests with `dotnet test` or Visual Studio Test Explorer against the `*Tests` projects.
- For integration debugging, attach to the platform instance (IISExpress for Oqtane, IIS10 for DNN).

## Runtime Environments
- Oqtane instance: `A:/2sxc/oqtane/oqtane.framework/Oqtane.Server`, using local SQL Server connection strings (`2sxc-oqtane`, `2sxc-oqtane-t1`) and served at `http://localhost:44357` via IISExpress.
- DNN instance: `A:/2sxc/2sxc-dnn/Website` with SQL database `2sxc-dnn`, hosted at `https://2sxc-dnn.dnndev.me` on IIS10.

## Documentation
- 2sxc docs: https://docs.2sxc.org
- Oqtane docs: https://docs.oqtane.org
- DNN docs: https://docs.dnncommunity.org

When conventions are unclear, consult `readme.md`, `contributing.md`, or ask the maintainers.
