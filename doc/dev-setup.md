# Development Setup

This guide covers local setup for the BookYourStay monorepo. Docker is only used for local infrastructure; Bun and .NET run directly on your host machine.

## Prerequisites

The default toolchain requirements are:

- Bun `1.3.4`
- .NET SDK `10`
- Docker Desktop or Docker Engine with `docker compose`
- Git

You can verify the local toolchain with:

```bash
bun --version
dotnet --version
docker --version
docker compose version
git --version
```

## Repository Setup

1. Clone the repository.
2. Move into the project root.
3. Create your local environment file:

```bash
cp .env.example .env
```

## Environment Setup

The repo includes a shared environment template at [`../.env.example`](../.env.example).

Default local values are set for:

- Postgres database name, user, password, and port
- MinIO root credentials, bucket, and ports
- `ASPNETCORE_ENVIRONMENT=Development`
- `APP_PORT=8080`
- `BACKEND_PORT=8080`
- `FRONTEND_PORT=3000`
- `NEXT_PUBLIC_API_BASE_URL=http://localhost:8080/api/v1`

The backend also has development defaults in [`../apps/backend/appsettings.Development.json`](../apps/backend/appsettings.Development.json), including the local Postgres connection string and JWT settings.

## Install Dependencies

Install JavaScript packages and restore the .NET solution:

```bash
bun run install
```

This runs:

- `bun install --frozen-lockfile`
- `dotnet restore bookyourstay.sln -p:NuGetAudit=false`

## Local Infrastructure

Bring up Postgres and MinIO:

```bash
sh ./dev up
```

Useful companion commands:

```bash
bun run infra:logs
bun run infra:ps
bun run infra:down
```

The default flow is:

1. Run `bun run install` once after cloning or whenever dependencies change.
2. Start local infrastructure with `bun run infra:up`.
3. Keep editing files locally from your normal IDE or editor.
4. Run `bun run dev:backend` and `bun run dev:frontend` in separate local terminals when you want the app servers running.

Useful direct execution commands:

```bash
bun install
dotnet restore bookyourstay.sln
```

Run the app servers from separate local terminals:

```bash
bun run dev:backend
bun run dev:frontend
```

Exposed local ports:

- Frontend: `http://localhost:3000`
- Backend: `http://localhost:8080`
- Postgres: `localhost:5432`
- MinIO API: `http://localhost:9000`
- MinIO Console: `http://localhost:9001`

Notes:

- Docker is only needed for Postgres and MinIO
- Bun and .NET must be installed on the host
- adjust ports or runtime settings in `.env` and app settings as needed

## Run The Backend

Start the backend in watch mode from a local terminal:

```bash
bun run dev:backend
```

What to expect:

- ASP.NET Core starts from [`../apps/backend`](../apps/backend)
- database migrations run on startup unless disabled
- the API root responds at `http://localhost:8080/`
- OpenAPI is exposed in development

If you want a one-off run instead of watch mode:

```bash
bun run start:backend --urls http://localhost:8080
```

## Run The Frontend

Start the Next.js app from a local terminal:

```bash
bun run dev:frontend
```

What to expect:

- the app runs from [`../apps/frontend`](../apps/frontend)
- Next.js uses Turbopack in development
- the default local app URL is typically `http://localhost:3000`

For a production-style local run:

```bash
bun run build:frontend
bun run start:frontend -- --port 3000
```

## Verification Commands

Use these commands to confirm the repo is healthy:

```bash
bun run test:backend
bun run lint
bun run typecheck
bun run check
```

What each one does:

- `bun run test:backend`: runs backend tests on the host
- `bun run lint`: runs frontend ESLint on the host
- `bun run typecheck`: runs frontend TypeScript checks on the host
- `bun run check`: runs the main verification suite end to end on the host

## Recommended Workflow

For normal day-to-day development:

1. Run `cp .env.example .env` once.
2. Run `bun run install`.
3. Start infrastructure with `bun run infra:up`.
4. Open one terminal for `bun run dev:backend` and another for `bun run dev:frontend`.
5. Before committing, run `bun run check`.

## Troubleshooting

### Port Already In Use

If Postgres, MinIO, backend, or frontend ports are already taken:

- stop the conflicting process
- or change the values in `.env`

### Backend Cannot Connect To Postgres

Check these items:

- `bun run infra:up` completed successfully
- Postgres is healthy in `bun run infra:logs`
- `.env` values still match the backend connection string expectations

### Frontend Is Not Updating

Try:

```bash
bun run infra:down
bun run infra:up
```

### Tests Fail Because Of Local State

Try:

```bash
bun run infra:down
bun run infra:up
bun run check
```

## Related Docs

- Repository docs index: [README.md](README.md)
- System requirements: [srs.md](srs.md)
- Database overview: [database.md](database.md)
- Backend notes: [backend.md](backend.md)
- Frontend notes: [frontend.md](frontend.md)
