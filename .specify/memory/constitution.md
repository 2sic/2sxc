# 2sxc Spec Constitution

This constitution governs how specifications, plans, and code are authored for 2sxc across DNN (.NET Framework) and Oqtane (.NET). Specifications are the source of truth; code serves the spec. All contributors must align with the principles, gates, and constraints defined here.

Version: 1.0.0 | Ratified: 2025-09-19 | Last Amended: 2025-09-19

## Core Principles

### I. Specifications as Source of Truth (Spec-Driven)

- Specifications live under `.specify/` and related feature folders; they capture intent, user stories, acceptance criteria, constraints, and non-functionals.
- Implementation plans and tasks are derived from specs. When specs change, plans and code adapt—never the other way around.
- Specs must be precise and executable: complete enough to generate plans, tests, and scaffolding without ambiguity.

### II. Platform-Neutral Core with Strict Integration Boundaries

- Keep shared, platform-neutral logic in `Src/Sxc/ToSic.Sxc*`.
- DNN-specific code lives only in `Src/Dnn/ToSic.Sxc.Dnn*`; Oqtane-specific code lives only in `Src/Oqtane/ToSic.Sxc.Oqt.*`.
- The EAV data layer is external in `A:/2sxc/eav-server` – referenced by solutions, never copied into this repo.
- APIs in the core must remain neutral; platform adapters add the minimal glue required.

### III. Library-First, DI-Driven, and Side-Effect Aware

- Favor modular, reusable libraries with clear boundaries and minimal coupling.
- Heavy use of dependency injection; avoid static/global state and tight coupling.
- Prefer functional helpers and immutability where feasible; keep methods small, focused, and intention-revealing.

### IV. Test-First and Integration-First

- Write tests before implementation where practicable; at minimum, define contracts and acceptance tests up-front.
- Use xUnit test projects under `*Tests/`. Trace tests to spec acceptance criteria.
- Prefer realistic integration tests and contract tests over isolated mocks when verifying behavior.

### V. Simplicity and Anti-Abstraction

- Start simple; add complexity only when required by explicit requirements or measured constraints.
- Trust framework/platform features; avoid unnecessary wrappers or indirection.
- Minimize the number of new projects and layers for a feature; justify exceptions.

### VI. Observability and Deterministic I/O

- Use the internal logging library consistently; produce structured, actionable logs.
- Favor deterministic, side-effect-free functions and plain data contracts.
- Where tools/CLIs are used (e.g., build tasks), prefer text/stdin/stdout with optional JSON for structured outputs.

### VII. Security and Safety by Default

- Follow least-privilege and secure-by-default practices across code, configuration, and deployments.
- Validate inputs, handle errors explicitly, and avoid leaking sensitive data in logs or exceptions.
- Respect repository boundaries (do not modify or relocate external repos like EAV, DNN, Oqtane).

### VIII. Performance, Memory, and Concurrency Discipline

- Optimize for performance and memory efficiency; measure before optimizing.
- Use async/await for I/O-bound work and guard against race conditions.
- Be explicit about thread safety and synchronization where applicable.

### IX. Documentation and Versioning

- Public APIs include XML docs; comment the intention, rationale, and pitfalls.
- Keep specifications, plans, and tests in sync; version meaningful changes and record rationale.
- Maintain change history via commit descriptions and spec amendments.

## Process: From Spec to Code

1) Specification

- Capture problem, intent, user stories, acceptance criteria, constraints, and unknowns.
- Mark uncertainties as [NEEDS CLARIFICATION] and resolve before committing to build scope.

1) Implementation Plan

- Translate requirements into architecture, data models, contracts, and testing strategy.
- Map each requirement to decisions with rationale; keep the plan high-level and navigable.

1) Tasks

- Derive an executable task list from the plan (contracts → tests → implementation order).
- Identify parallelizable work and dependencies; keep tasks small and verifiable.

1) Continuous Validation

- As specs evolve, plans and tasks update; tests and code regenerate/adjust accordingly.
- Production insights feed back into specs as non-functional requirements or constraints.

## Compliance Gates (Pre-Implementation)

Simplicity Gate (Principle V)

- [ ] ≤ 3 new projects introduced by this feature (or justification recorded)
- [ ] No speculative features; no future-proofing without explicit requirements

Anti-Abstraction Gate (Principles II & V)

- [ ] Platform features used directly where feasible
- [ ] No unnecessary wrappers/indirection around framework constructs

Integration-First Gate (Principle IV)

- [ ] API/contracts defined and reviewed
- [ ] Contract/integration tests outlined before implementation

Boundary Gate (Principle II)

- [ ] Core-neutral code stays in `Src/Sxc/` or shared projects
- [ ] DNN/Oqtane specifics isolated to their respective folders
- [ ] No sources pulled from `A:/2sxc/eav-server` into this repo

Security Gate (Principle VII)

- [ ] Inputs validated; error handling explicit; secrets protected
- [ ] Logging avoids sensitive data; least-privilege enforced

## Quality Gates (Every PR)

- Build: Solutions compile with restored NuGet packages; respect `Dependencies/` binaries.
- Tests: All unit/integration tests under `*Tests/` pass; add/adjust tests for changed behavior.
- Lint/Types: C# nullable enabled; address warnings that affect correctness and safety.
- Smoke: Minimal manual or automated smoke test proving the primary path works.

## Project Constraints and Standards (2sxc)

- Build Targets
  - DNN: `Src/Dnn/ToSic.Sxc.Dnn/ToSic.Sxc.Dnn.csproj` (.NET Framework)
  - Oqtane: `Src/Oqtane/ToSic.Sxc.Oqt.Package/ToSic.Sxc.Oqt.Package.csproj` (.NET)
- Restore NuGet before build; some binaries ship in `Dependencies/`.
- Debug/Integration
  - Oqtane local: typically `http://localhost:44357` (IISExpress), DB `2sxc-oqtane*`
  - DNN local: `A:/2sxc/2sxc-dnn/Website` on IIS10, DB `2sxc-dnn`
- Coding Conventions
  - SoC + SOLID; heavy DI; nullable enabled; explicit null checks
  - Prefer latest C# features where sensible; keep methods small and focused
  - Logging via internal library; security best practices throughout
  - Cross-platform separation: shared in `Sxc/`; platform specifics in `Dnn/` or `Oqtane/`

## Governance

- This constitution supersedes ad-hoc practices for specs, plans, and code generation.
- All PRs verify compliance with principles, gates, and project constraints.
- Complexity must be justified in the plan with clear rationale and traceability to requirements.
- Amendments require:
  - Documented rationale and impact analysis
  - Maintainer review/approval
  - Backwards compatibility assessment or migration notes
- Record amendments in this file’s version header and in PR descriptions.

Docs and References

- 2sxc: https://docs.2sxc.org
- Oqtane: https://docs.oqtane.org
- DNN: https://docs.dnncommunity.org
- Spec-Driven Development: https://github.com/github/spec-kit/blob/main/spec-driven.md