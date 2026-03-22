# BookYourStay Docs

BookYourStay is a polyglot monorepo for a booking platform. The repository currently contains a .NET 10 backend and a Next.js frontend, with Docker used only for local infrastructure services.

## Repository Layout

```text
.
|-- apps/
|   |-- backend/    # ASP.NET Core API, database migrations, and tests
|   `-- frontend/   # Next.js app workspace
|-- doc/            # Repository documentation
|-- packages/       # Reserved for shared JS/TS packages
|-- compose.yml     # Local infrastructure
|-- schema.sql      # Product-level schema draft
`-- package.json    # Root monorepo scripts
```

## Quick Start

1. Copy the local environment template:

```bash
cp .env.example .env
```

2. Install local dependencies:

```bash
bun run install
```

3. Start local infrastructure:

```bash
bun run infra:up
```

4. Start the app processes from separate terminals when you need them:

```bash
bun run dev:backend
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

Local development helpers:

```bash
bun run install
bun run infra:up
bun run dev:backend
bun run dev:frontend
bun run test:backend
bun run infra:down
```

## Docs Index

- Development Setup: [dev-setup.md](dev-setup.md)
- SRS: [srs.md](srs.md)
- Database: [database.md](database.md)
- Backend: [backend.md](backend.md)
- Frontend: [frontend.md](frontend.md)
- Packages: [packages.md](packages.md)

## Product Summary

The product is aimed at hotel, resort, event venue, activity, and package bookings with optional services like transport, decoration, and catering. The current backend foundation focuses on authentication and database migrations, while the frontend app is ready to grow into the customer-facing experience.

## Database

See [database.md](database.md) for the human-readable database overview. The production-oriented schema draft lives at [../schema.sql](../schema.sql), and runtime backend migrations live under [../apps/backend/Database/Migrations](../apps/backend/Database/Migrations).
