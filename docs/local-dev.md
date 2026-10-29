# Local development environment

## Start/stop the local stack

```bash
docker compose up -d    # start Postgres and Keycloak
docker compose ps       # check status
docker compose down     # stop and remove containers
```

## Defaults

| Service    | Port                     | Credentials                       |
|------------|--------------------------|------------------------------------|
| PostgreSQL | `5432`                   | user `supergood` / pass `supergood`, db `supergood` |
| Keycloak   | `8081` (mapped to container 8080) | admin `admin` / pass `admin` |

Keycloak's host port is `8081`, not the default `8080`, to avoid colliding
with other local services. On Apple Silicon, the Keycloak container runs
under Rosetta emulation (`platform: linux/amd64` in `docker-compose.yml`) —
the native `linux/arm64` Keycloak image hits a known JVM `SIGILL` crash
under Docker Desktop's virtualization on this host.

## Running a service

```bash
dotnet run --project src/Orders.Service
# or Catalog.Service / Jobs.Service
```

Each service exposes `GET /health` for liveness (no dependency checks yet —
see SIE-47 for deeper health checks).

## Database schemas and migrations

One database (`supergood`), one schema and one database user per service
(ADR 0005):

| Service | Schema | Runtime user (local) | Tables |
|---|---|---|---|
| Orders | `orders` | `orders_svc` | `orders`, `order_items` |
| Catalog | `catalog` | `catalog_svc` | `contacts`, `catalog_items`, `price_history` |
| Jobs | `jobs` | `jobs_svc` | `jobs` |

A runtime user has `USAGE` and row-level `SELECT/INSERT/UPDATE/DELETE` on its own
schema only. It cannot read another service's tables, run DDL, or create
schemas, so cross-service joins fail by permission. Data from another service is
read through that service's API. Cross-service ids (`contactId`,
`catalogItemId`) are plain columns with no foreign key.

`docker/postgres/init/01-roles-and-schemas.sql` creates the roles, schemas and
default privileges on the first start of a fresh `postgres-data` volume. After
changing it, run `docker compose down -v` to re-initialise. It also creates the
`migrator` role (password `migrator`): the migration role, held by the deploy
pipeline in deployed environments. Services never migrate at startup.

### Migration conventions

- Each service owns its `DbContext` and migrations under
  `src/<Service>.Service/Persistence/` (`Migrations/` holds the generated files).
  Never edit another service's migrations.
- Each `DbContext` sets `HasDefaultSchema` to its own schema, and its
  `__EFMigrationsHistory` table lives in that schema.
- Migration name `InitialSchema` first, then a descriptive PascalCase name
  (`AddOrderNotes`). One migration per change; never edit an applied migration.
- Tables are lowercase snake_case plurals; columns keep EF's PascalCase names.
  Enums are stored as their C# names in `varchar` with a `CHECK` constraint.
- Add a migration (design-time needs the Development connection string):

  ```bash
  ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add <Name> \
    --project src/Orders.Service --output-dir Persistence/Migrations
  ```

- Apply with the migration role, locally or as a script for the pipeline:

  ```bash
  ASPNETCORE_ENVIRONMENT=Development dotnet ef database update \
    --project src/Orders.Service \
    --connection "Host=localhost;Database=supergood;Username=migrator;Password=migrator"
  dotnet ef migrations script --idempotent --project src/Orders.Service
  ```

Install the tool once with `dotnet tool install -g dotnet-ef`.
