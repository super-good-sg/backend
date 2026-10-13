# backend

Super Good backend.

## Docs (source of truth)
Specs, ADRs, contracts and the agent workflow live in [document-specs](https://github.com/super-good-sg/document-specs). Read [docs/agent-workflow.md](https://github.com/super-good-sg/document-specs/blob/main/docs/agent-workflow.md) before starting any ticket. Work lives in Linear (project SuperGood).

## Build / test / lint
TODO: fill in once the project is scaffolded.
- Build: TODO
- Test: TODO
- Lint: TODO

## Conventions
- Changes are OpenSpec changes that reference a merged spec in document-specs.
- One ticket, one branch (Linear `gitBranchName`), one PR. Never merge your own PR.
- Reuse approved patterns; stop and ask before adding a new library or convention.
- Contracts (OpenAPI, order schema) change in document-specs first, never here.
- No secrets in code or commits.
