# Development Setup

This guide covers local setup for the BookYourStay monorepo.

## Prerequisites

Choose one of these local setup modes:

- Docker Desktop or Docker Engine with `docker compose`
- Git

For host-native development, also install:

- Bun `1.3.4` or newer
- .NET SDK `10.x`

You can verify the container-first toolchain with:

```bash
docker --version
docker compose version
git --version
```

For host-native development, you can additionally verify:

```bash
bun --version
dotnet --version
```

## Repository Setup

1. Clone the repository.
2. Move into the project root.
3. Install JavaScript workspace dependencies:

```bash
bun install
```

If you plan to work entirely through Docker, you can skip this step on the host and use `./dev install` later instead.

## Environment Setup

The repo includes a shared environment template at [`../.env.example`](../.env.example).

Create your local environment file:

```bash
cp .env.example .env
```

Default local values are set for:

- Postgres database name, user, password, and port
- MinIO root credentials, bucket, and ports
- `ASPNETCORE_ENVIRONMENT=Development`
- `APP_PORT=8080`
- `BACKEND_PORT=8080`
- `FRONTEND_PORT=3000`

Optional local overrides:

- `LOCAL_UID` and `LOCAL_GID` let the dev container match your host user on Linux

The backend also has development defaults in [`../apps/backend/appsettings.Development.json`](../apps/backend/appsettings.Development.json), including the local Postgres connection string and JWT settings.

## Start Local Infrastructure

Bring up Postgres and MinIO from the repo root:

```bash
bun run infra:up
```

Useful companion commands:

```bash
bun run infra:logs
bun run infra:down
```

Local persistence:

- Postgres data is bind-mounted to `./.data/postgres`
- removing the container will not remove your local database files
- deleting `./.data/postgres` will reset the local Postgres data set

Default local service endpoints:

- Postgres: `localhost:5432`
- MinIO API: `http://localhost:9000`
- MinIO Console: `http://localhost:9001`

## Containerized Developer Environment

The monorepo includes a `dev-env` container with:

- .NET SDK `10`
- Bun `1.3.4`
- Git, curl, and basic shell tools

Build and start the containerized environment:

```bash
sh ./dev up
```

Useful companion commands:

```bash
sh ./dev build
sh ./dev shell
sh ./dev down
```

After the container is up, install workspace dependencies inside it:

```bash
sh ./dev install
```

Run the apps from inside the containerized environment:

```bash
sh ./dev backend
sh ./dev frontend
```

Exposed local ports:

- Frontend: `http://localhost:3000`
- Backend: `http://localhost:8080`

Notes:

- source code is bind-mounted into `/workspace`
- Bun and NuGet caches are persisted in Docker volumes for faster rebuilds
- use `sh ./dev shell` if you want to work entirely inside the container
- this path does not require Bun or .NET to be installed on the host
- `sh ./dev up` starts the shell/tooling container without claiming app ports
- `sh ./dev backend` and `sh ./dev frontend` start dedicated app containers only when needed

## Run The Backend

Start the backend in watch mode:

```bash
bun run dev:backend
```

What to expect:

- ASP.NET Core starts from [`../apps/backend`](../apps/backend)
- database migrations run on startup unless disabled
- the API root responds at `http://localhost:5000/` or `https://localhost:7xxx/` depending on local launch settings and `dotnet watch`
- OpenAPI is exposed in development

If you want a one-off run instead of watch mode:

```bash
bun run start:backend
```

## Run The Frontend

Start the Next.js app from the monorepo root:

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
bun run start:frontend
```

## Run Both Apps

To run backend and frontend together:

```bash
bun run dev:all
```

This starts Postgres and MinIO first, then launches both watch processes from the monorepo root.

## Verification Commands

Use these commands to confirm the repo is healthy:

```bash
bun run build
bun run test
bun run lint
bun run typecheck
bun run check
```

What each one does:

- `bun run build`: builds backend and frontend
- `bun run test`: runs backend unit and integration tests
- `bun run lint`: runs frontend ESLint
- `bun run typecheck`: runs frontend TypeScript checks
- `bun run check`: runs the main verification suite end to end

## Recommended Workflow

For normal day-to-day development:

1. Run `bun install` after pulling dependency changes.
2. Start infra with `bun run infra:up`.
3. Start backend with `bun run dev:backend`.
4. Start frontend with `bun run dev:frontend`.
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
rm -rf apps/frontend/.next
bun run dev:frontend
```

### Tests Fail Because Of Local State

Try:

```bash
bun run infra:down
bun run infra:up
bun run test:backend
```

## Related Docs

- Repository docs index: [README.md](README.md)
- System requirements: [srs.md](srs.md)
- Database overview: [database.md](database.md)
- Backend notes: [backend.md](backend.md)
- Frontend notes: [frontend.md](frontend.md)
