# Tasks

## 1. Solution and contracts project

- [x] 1.1 Create `backend.sln` at repo root and `src/Shared/Shared.csproj`
      (class library, .NET 8), add it to the solution; verify `dotnet sln list`
      shows the project
- [x] 1.2 Add C# POCOs in `src/Shared/` mirroring
      `document-specs/docs/contracts/order-schema.v1.json`,
      `catalog-item-schema.v1.json`, `contact-schema.v1.json`,
      `product-schema.v1.json` (field names/types only, no persistence
      attributes); verify `dotnet build src/Shared` succeeds

## 2. Service projects

- [x] 2.1 Create `src/Orders.Service/Orders.Service.csproj` (ASP.NET Core
      minimal API, .NET 8), reference `Shared`, add `GET /health`
      returning 200; verify `dotnet run --project src/Orders.Service` then
      `curl localhost:<port>/health` returns 200
- [x] 2.2 Create `src/Catalog.Service/Catalog.Service.csproj` the same way;
      verify its `/health` endpoint returns 200
- [x] 2.3 Create `src/Jobs.Service/Jobs.Service.csproj` the same way; verify
      its `/health` endpoint returns 200
- [x] 2.4 Add all three service projects to `backend.sln`; verify
      `dotnet build` at repo root builds all four projects with no errors

## 3. Local development environment

- [x] 3.1 Add `docker-compose.yml` at repo root with a `postgres` service
      (official `postgres` image) and a `keycloak` service (official
      `quay.io/keycloak/keycloak` image), matching ADR 0005's one-shared-
      server model; verify `docker compose up -d` starts both containers
      and `docker compose ps` shows them healthy/running
- [x] 3.2 Document compose usage (how to start/stop, default ports/creds)
      in a short `docs/local-dev.md` note in this repo; verify the
      documented commands run as written

## 4. Verification

- [x] 4.1 Verify no service project references another service project
      (`Orders.Service` has no reference to `Catalog.Service`/
      `Jobs.Service`, etc.) by inspecting each `.csproj`'s
      `<ProjectReference>` entries
- [x] 4.2 Verify full solution build and all three health endpoints with
      no database/Keycloak running (liveness only, per spec)

## Workflow follow-up

- Archive this change once SIE-39's minimal scope above is merged and
  reviewed.
- Note in the PR description that this is a partial/minimal delivery of
  SIE-39, with remaining SIE-39 scope (CI integration, deeper health
  checks) left for later tickets (SIE-40, SIE-47).
