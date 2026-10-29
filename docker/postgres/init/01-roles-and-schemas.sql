-- Local-dev bootstrap for ADR 0005: one schema and one database user per service.
-- Runs once, on a fresh postgres-data volume, as the superuser. In deployed
-- environments the deploy pipeline does the same with the migration role.
-- Passwords here are local-dev only.

-- Migration role: creates and alters objects in the three schemas. Held by the
-- deploy pipeline in deployed environments; never used by a running service.
CREATE ROLE migrator LOGIN PASSWORD 'migrator';

-- Runtime roles: row-level access to their own schema only.
CREATE ROLE orders_svc  LOGIN PASSWORD 'orders_svc';
CREATE ROLE catalog_svc LOGIN PASSWORD 'catalog_svc';
CREATE ROLE jobs_svc    LOGIN PASSWORD 'jobs_svc';

REVOKE ALL ON DATABASE supergood FROM PUBLIC;
GRANT CONNECT ON DATABASE supergood TO migrator, orders_svc, catalog_svc, jobs_svc;
REVOKE CREATE ON SCHEMA public FROM PUBLIC;

CREATE SCHEMA orders  AUTHORIZATION migrator;
CREATE SCHEMA catalog AUTHORIZATION migrator;
CREATE SCHEMA jobs    AUTHORIZATION migrator;

GRANT USAGE ON SCHEMA orders  TO orders_svc;
GRANT USAGE ON SCHEMA catalog TO catalog_svc;
GRANT USAGE ON SCHEMA jobs    TO jobs_svc;

-- Tables are created later by migrations running as migrator; these defaults give
-- the matching runtime role DML rights on them, and nothing else.
ALTER DEFAULT PRIVILEGES FOR ROLE migrator IN SCHEMA orders
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO orders_svc;
ALTER DEFAULT PRIVILEGES FOR ROLE migrator IN SCHEMA catalog
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO catalog_svc;
ALTER DEFAULT PRIVILEGES FOR ROLE migrator IN SCHEMA jobs
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO jobs_svc;
