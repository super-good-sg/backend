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
