# Design

## Decisions

- **DbContext in each service project, not a separate project.** The pattern
  says "EF Core project/`DbContext`"; a context inside the service project
  keeps one project per service (ADR 0005) and no new project references.
- **Persistence entities are separate from `Shared` records.** `Shared` mirrors
  the JSON contracts and carries no persistence attributes (SIE-39). Entities
  reuse `Shared` enums only.
- **No cross-service foreign keys.** `ContactId` and `CatalogItemId` on order
  rows are plain nullable columns; orders hold point-in-time snapshots anyway.
- **Owner scoping.** Every top-level table has `OwnerId` (Keycloak `sub`), as
  the OpenAPI contract scopes every query to the authenticated user.
- **Stock.** `catalog_items.StockOnHand` defaults to 0 and has no non-negative
  check (the contract sets no minimum). Only the order-confirmed handler will
  write it.
- **Price history.** `price_history` rows are appended on price change;
  `catalog_items.Price` holds the current price.
- **Jobs.** `Result` and `Error` are `jsonb`; check constraints enforce
  "result only when succeeded, error only when failed".
- **Types.** Money `numeric(18,2)`, quantity and stock `numeric(18,4)`, tax
  rate `numeric(7,4)`, timestamps `timestamptz`, enums as `varchar` C# names
  with `CHECK`. Confidence is stored as JSON (owned type `ToJson`).
- **Roles.** Schemas are pre-created by the bootstrap script, owned by
  `migrator`; `ALTER DEFAULT PRIVILEGES FOR ROLE migrator` gives each runtime
  role DML on tables the migration creates. Services never run migrations.
- **Naming.** Lowercase snake_case plural tables; EF default PascalCase columns
  (avoids a naming-convention library).
