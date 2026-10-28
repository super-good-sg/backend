# Spec Delta

## Purpose

Defines the project layout and minimum runtime contract every backend
service must follow, so services are structured consistently and are
individually deployable and health-checkable from day one.

## ADDED Requirements

### Requirement: Solution structure
The repository SHALL contain a single .NET solution file at its root that
references a shared project and one project per service.

#### Scenario: Solution builds all services
- **WHEN** `dotnet build` is run against the root solution file
- **THEN** the shared project and every service project build
  successfully

### Requirement: Per-service project isolation
Each service SHALL live in its own project directory and SHALL NOT
reference another service's project directly. Services may reference the
shared project.

#### Scenario: Orders project has no dependency on Catalog or Jobs
- **WHEN** the `Orders.Service` project file is inspected
- **THEN** it lists no project reference to `Catalog.Service` or
  `Jobs.Service`

### Requirement: Liveness endpoint
Each service SHALL expose an HTTP `GET /health` endpoint that returns a
successful response when the service process is running, without checking
any external dependency (database, message bus, identity provider).

#### Scenario: Health check succeeds with no dependencies running
- **WHEN** a service is started with no database, Postgres, or Keycloak
  available
- **THEN** `GET /health` on that service still returns HTTP 200

### Requirement: Local development environment
The repository SHALL provide a Docker Compose definition that starts a
local PostgreSQL instance and a local Keycloak instance, matching the
shared-server model described in ADR 0005 (one PostgreSQL server shared
across services).

#### Scenario: Local stack starts
- **WHEN** a developer runs the repo's compose file
- **THEN** a PostgreSQL container and a Keycloak container both start and
  accept connections
