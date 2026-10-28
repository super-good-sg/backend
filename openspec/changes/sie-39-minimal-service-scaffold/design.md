# Design

## Context

The `backend` repo currently has no source code — only OpenSpec tooling and
Node-based repo tooling (commitlint/husky). ADR 0006
(document-specs) confirms the AI/jobs service is .NET, in the same backend
monorepo as orders and catalog — so all three services target .NET. See
proposal.md for why this scaffold is being pulled forward ahead of the full
SIE-39 design.

## Goals / Non-Goals

**Goals:**
- A buildable solution with one project per service plus a shared
  contracts project.
- Each service runnable locally and reporting liveness via `GET /health`.
- A docker-compose file giving each service the same Postgres/Keycloak
  dependencies it will have in later environments.
- A project layout SIE-26 can extend by adding one EF Core project/
  `DbContext` per service without restructuring anything here.

**Non-Goals:**
- CI pipeline wiring (SIE-40).
- Dependency-aware health checks (DB/queue/IDP reachability) — SIE-47.
- Any authentication/authorization wiring in the services themselves —
  Keycloak runs locally for later tickets (SIE-32/33) but nothing here
  calls it yet.
- EF Core `DbContext`s, connection strings, or migrations — SIE-26.
- Shared middleware, logging, or observability conventions beyond what
  ASP.NET Core provides by default — future scaffold hardening.

## Decisions

- **.NET 8 (LTS)**: no version is pinned anywhere in document-specs; .NET 8
  is the current LTS and keeps the team off a version needing an upgrade
  mid-project. Recorded here as an assumption — flag in the PR description
  for the spec owner to confirm or override.
- **Folder layout**: `src/Shared/`, `src/Orders.Service/`,
  `src/Catalog.Service/`, `src/Jobs.Service/`. Flat `src/` rather than
  nested by bounded-context folder, since there are only four projects and
  ADR 0005 already fixes the three service names — no benefit to extra
  nesting for v1.
- **Shared project is a plain class library**: holds C# POCOs mirroring
  `document-specs/docs/contracts/*.v1.json` (order, catalog-item, contact,
  product). No JSON Schema codegen tool is introduced — hand-written POCOs
  are simpler to review for four small schemas and avoid adding a new
  build dependency for v1.
  **Flagged during review**: this conflicts with ADR 0001 ("each code repo
  pins the contract version it follows" via a generated-code + pin-file
  convention, PR-checked for drift). That ADR has no recorded owner
  reverification and its implementing spec
  (`document-specs/specs/001-contract-drift-detection/`) is still Draft —
  no repo anywhere has a pin file or generator today. Building that
  pipeline is out of scope for this scaffold change; this needs an
  explicit owner decision (confirm the hand-written approach for v1, or
  require the generator+pin setup before any contract-mirroring code
  lands). Left open here rather than decided unilaterally.
  Type-fidelity gaps a future generator (or further hand-maintenance)
  would need to close, found in review and not fixed in this change
  because they're serialization-behavior concerns with no JSON call site
  yet in this diff: enums serialize as integers by default, not the
  schema's lowercase strings; optional-but-not-required string fields
  (e.g. `OrderContactV1.Phone`/`Address`) serialize an explicit `null`
  where the schema expects the key omitted. Schema-required-but-nullable
  fields (`ConfirmedAt`, `ContactId`, `CatalogItemId`) were fixed in this
  change (`required` added) since that's a type-definition correctness
  fix, not a serialization-behavior one.
- **Health endpoint via minimal API, not the `Microsoft.Extensions.Diagnostics.HealthChecks`
  package**: a single `app.MapGet("/health", () => Results.Ok())` meets
  the liveness-only requirement without pulling in a package whose main
  value (dependency health checks) is explicitly out of scope (SIE-47).
  Swapping to the health-checks package later is additive, not a breaking
  change.
- **docker-compose, not a dev-container or Aspire AppHost**: the repo has
  no existing compose or Aspire convention to follow, and ADR 0005 already
  describes the target topology (one shared Postgres server) in terms
  compose can express directly. Revisit if the later full SIE-39 design
  adopts .NET Aspire for service orchestration.

## Implementation Notes

- The `Contracts` project name from the original plan collides with this
  repo's permission rule protecting `**/contracts/**` (the real API
  contracts live only in document-specs). Renamed the shared class library
  to `Shared` throughout.
- `dotnet new sln` on this machine's SDK (10.x) defaults to the new `.slnx`
  format; generated with `-f sln` to keep the classic `.sln` referenced by
  the tasks.
- Keycloak's native `linux/arm64` image hits a JVM `SIGILL` crash under
  Docker Desktop's Apple Silicon virtualization; pinned
  `platform: linux/amd64` (Rosetta) in `docker-compose.yml` to work around
  it. Also moved its host port to `8081` (an unrelated local container was
  already bound to `8080`).

## Risks / Trade-offs

- [Scaffold decisions made here (layout, .NET version) get superseded once
  the full SIE-39 ticket is designed] → Mitigation: keep this change
  minimal and clearly scoped as a stopgap in the proposal; the full SIE-39
  work can restructure `src/` without touching SIE-26's schema work if it
  stays inside each service's own project.
- [Hand-written contract POCOs can drift from the JSON Schema source of
  truth in document-specs] → Mitigation: none automated in this change;
  flagged as a known gap, acceptable for v1 given only four small schemas.

## Open Questions

- Should the full SIE-39 ticket introduce shared health-check middleware,
  structured logging, or an Aspire AppHost across services? Does not
  affect this change's specs or tasks — deferred to that ticket.
