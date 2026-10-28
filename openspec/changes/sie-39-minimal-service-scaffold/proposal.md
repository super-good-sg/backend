# Proposal

## Why

SIE-26 ("[2.3] Database schema v1 per service") requires EF Core migrations
owned by each service, but the `backend` repo currently has no .NET solution
at all — no `.sln`, no service projects, nowhere to put a `DbContext`. SIE-26
cannot proceed until a minimal solution/service skeleton exists. The full
service-template design is ticket SIE-39 ("[4.1] Solution scaffold and
service template"), still in Backlog; this change delivers just enough of
SIE-39's scope to unblock SIE-26, deferring the remaining SIE-39 acceptance
criteria (full CI integration, deeper health checks) to later work.

## What Changes

- Add a .NET solution (`backend.sln`) at the repo root.
- Add a shared `Shared` class library project (for types shared across
  services — e.g. the canonical order/catalog/contact record shapes from
  `document-specs/docs/contracts/`).
- Add three minimal ASP.NET Core Web API projects, one per service named in
  ADR 0005: `Orders.Service`, `Catalog.Service`, `Jobs.Service`. Each exposes
  only a `GET /health` liveness endpoint (no external dependency checks —
  deeper health checks are SIE-47, explicitly out of scope here).
- Add a `docker-compose.yml` for local development: PostgreSQL and Keycloak
  containers, matching ADR 0005's "one PostgreSQL Flexible Server, shared
  across services" model.
- Add a per-service folder convention (`src/<ServiceName>.Service/`,
  `src/Shared/`) that SIE-26 will extend with an EF Core project per
  service.

Out of scope (left for their own tickets):
- CI build/test/image-push (SIE-40).
- Health checks beyond basic liveness (SIE-47).
- EF Core `DbContext`s, migrations, and schema/grant setup (SIE-26 — the
  next change, built on top of this one).
- The migration-role / deploy-pipeline grant automation described in ADR
  0005's consequences section.

## Capabilities

### New Capabilities
- `service-scaffold`: the solution structure, per-service project
  skeleton, and local dev compose setup that every backend service is
  built from. Durable behavior: "a new service follows this project
  layout and exposes a liveness endpoint."

### Modified Capabilities
(none — no existing specs to modify)

## Impact

- Affected code: repo root (new `backend.sln`, `docker-compose.yml`,
  `src/` directory tree). No existing application code is touched (none
  exists yet).
- Affected systems: local developer environment (docker-compose), future
  CI (SIE-40) will build against this solution.
- Dependencies: ASP.NET Core / .NET SDK (version to be pinned in
  design.md), Postgres and Keycloak container images for compose.
- Downstream: SIE-26 (DB schema v1 per service) depends directly on the
  per-service project layout this change creates.
