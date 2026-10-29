# Spec Delta

## Purpose

Defines where each service's data lives and how its schema is migrated, so
cross-service joins are impossible by permission (ADR 0005).

## ADDED Requirements

### Requirement: Schema per service
Orders, Catalog and Jobs SHALL each store data only in their own PostgreSQL
schema (`orders`, `catalog`, `jobs`). The catalog schema SHALL hold contacts,
catalog items with an on-hand stock count, and price history.

#### Scenario: Migrations create each service's tables in its schema
- **WHEN** a service's migrations are applied to an empty database prepared by
  the bootstrap script
- **THEN** its tables exist in its own schema and in no other

### Requirement: Runtime user limited to its own schema
Each service's runtime database user SHALL have row-level access on its own
schema only, with no DDL and no rights on any other schema.

#### Scenario: Cross-service read is denied
- **WHEN** the `orders_svc` user selects from `catalog.contacts`
- **THEN** PostgreSQL rejects it with a permission error

#### Scenario: Runtime user cannot alter schema
- **WHEN** `orders_svc` runs `CREATE TABLE` or `DROP TABLE` in the `orders` schema
- **THEN** PostgreSQL rejects it

### Requirement: Migrations owned per service and applied by the migration role
Each service SHALL hold its own EF Core migrations and migration history table
in its own schema. Services SHALL NOT apply migrations at startup.

#### Scenario: Service starts without a database
- **WHEN** a service starts with no database available
- **THEN** `GET /health` still returns HTTP 200

### Requirement: Stock count constraints
`catalog_items.StockOnHand` SHALL default to 0.

#### Scenario: New item has zero stock
- **WHEN** a catalog item row is inserted without a stock value
- **THEN** `StockOnHand` is 0
