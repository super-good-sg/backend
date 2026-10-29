# Proposal

## Why

SIE-26 ("[2.3] Database schema v1 per service") defines the PostgreSQL schema
for each service, with no cross-service joins and EF Core migration
conventions (ADR 0005, pattern `ef-core-migrations-per-service`). The service
scaffold from SIE-39 has no persistence yet. The catalog schema must include an
on-hand stock count per item (ticket update 2026-10-21).

## What Changes

- Add an EF Core `DbContext` and an `InitialSchema` migration to each service,
  owned by that service:
  - `orders` schema: `orders`, `order_items` (order-schema.v1.json).
  - `catalog` schema: `contacts`, `catalog_items` (including `StockOnHand`),
    `price_history` (contact-schema.v1.json, catalog-item-schema.v1.json,
    OpenAPI price history).
  - `jobs` schema: `jobs` (OpenAPI `Job`, v1 baseline).
- Add the Npgsql EF Core provider and EF Core design package to each service.
- Add a local-dev bootstrap script that creates the migration role, one runtime
  role per service, the three schemas and the per-schema grants; mount it in
  `docker-compose.yml`.
- Document the migration conventions in `docs/local-dev.md`.

Out of scope:
- Repositories, endpoints, validation and the order-confirmed stock handler.
- Retry and dead-letter columns on jobs (SIE-27).
- The deploy pipeline that applies migrations and grants in deployed
  environments (SIE-40).
- Keycloak's own database.

## Capabilities

### New Capabilities
- `database-schema`: per-service schema, runtime/migration role split, and
  migration conventions.

### Modified Capabilities
(none)

## Impact

- Affected code: the three service projects (new `Persistence/` folders,
  `Program.cs`, `appsettings.Development.json`, package references),
  `docker-compose.yml`, `docker/postgres/init/`, `docs/local-dev.md`.
- New dependencies: `Npgsql.EntityFrameworkCore.PostgreSQL` and
  `Microsoft.EntityFrameworkCore.Design` 8.0.11, implied by ADR 0005's EF Core
  decision.
