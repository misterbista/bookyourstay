# BookYourStay Docs

BookYourStay is a polyglot monorepo for a booking platform. The repository currently contains a .NET 10 backend and a Next.js frontend, with Bun orchestrating the JavaScript workspace tasks from the root.

## Repository Layout

```text
.
|-- apps/
|   |-- backend/    # ASP.NET Core API, database migrations, and tests
|   `-- frontend/   # Next.js app workspace
|-- doc/            # Repository documentation
|-- packages/       # Reserved for shared JS/TS packages
|-- compose.yml     # Local Postgres + MinIO services
|-- schema.sql      # Product-level schema draft
`-- package.json    # Root monorepo scripts
```

## Quick Start

1. Install dependencies:

```bash
bun install
```

2. Start local infrastructure:

```bash
bun run infra:up
```

3. Run the backend:

```bash
bun run dev:backend
```

4. Run the frontend:

```bash
bun run dev:frontend
```

## Root Commands

```bash
bun run build
bun run test
bun run lint
bun run typecheck
bun run check
```

Backend-specific:

```bash
bun run dev:backend
bun run build:backend
bun run test:backend
```

Frontend-specific:

```bash
bun run dev:frontend
bun run build:frontend
bun run lint:frontend
bun run typecheck:frontend
```

Infrastructure:

```bash
bun run infra:up
bun run infra:down
bun run infra:logs
```

## Docs Index

- Development Setup: [dev-setup.md](dev-setup.md)
- SRS: [srs.md](srs.md)
- Database: [database.md](database.md)
- Backend: [backend.md](backend.md)
- Frontend: [frontend.md](frontend.md)
- Packages: [packages.md](packages.md)

## Product Summary

The product is aimed at hotel, resort, event venue, activity, and package bookings with optional services like transport, decoration, and catering. The current backend foundation focuses on authentication and database migrations, while the frontend workspace is ready to grow into the customer-facing experience.

## Database

See [database.md](database.md) for the human-readable database overview. The production-oriented schema draft lives at [../schema.sql](../schema.sql), and runtime backend migrations live under [../apps/backend/Database/Migrations](../apps/backend/Database/Migrations).
