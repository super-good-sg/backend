# Tasks

## 1. Persistence

- [x] 1.1 Add Npgsql EF Core and EF Core Design packages to each service
- [x] 1.2 Orders: entities, `OrdersDbContext` (schema `orders`), `InitialSchema` migration
- [x] 1.3 Catalog: entities, `CatalogDbContext` (schema `catalog`, includes `StockOnHand`), `InitialSchema` migration
- [x] 1.4 Jobs: entity, `JobsDbContext` (schema `jobs`), `InitialSchema` migration
- [x] 1.5 Register each context in `Program.cs` with its schema's migration history table; add Development connection strings

## 2. Roles and local environment

- [x] 2.1 Bootstrap SQL: migration role, runtime roles, schemas, grants; mount in compose
- [x] 2.2 Document schemas and migration conventions in `docs/local-dev.md`

## 3. Verification

- [x] 3.1 Apply migrations as `migrator` on a fresh Postgres; tables appear in the right schemas
- [x] 3.2 Runtime users: own-schema DML works; cross-schema select, DDL, create schema, drop, truncate are denied
- [x] 3.3 Orders EF round trip as `orders_svc` (enum, owned JSON, cascade)
- [x] 3.4 `GET /health` returns 200 with no database running

## Workflow follow-up

- Archive this change after merge.
