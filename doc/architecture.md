# Architecture

This repo is a small polyglot monorepo. Keep it boring, explicit, and feature-first.

## Repository Boundaries

```text
apps/backend      ASP.NET Core API and backend tests
apps/frontend     Next.js app
packages/shared   TypeScript contracts and tiny framework-agnostic utilities
```

Do not put app-specific code in `packages/shared`. Shared code should be stable contracts, constants, or pure utilities used by more than one workspace.

## Frontend

Use Next.js App Router files only as route entrypoints. Product code belongs in feature folders.

```text
apps/frontend/app/                 routes and layouts only
apps/frontend/components/ui/       shadcn/ui source components
apps/frontend/features/<feature>/  feature-owned api, components, hooks, data, types
apps/frontend/shared/              frontend-only layout, navigation, and shared app components
apps/frontend/lib/                 generic app utilities only
```

Feature-specific API clients live under the owning feature, for example `features/auth/api/auth-api.ts`. Avoid feature barrels for route imports; import the concrete route component directly.

## Backend

The backend is feature-first with a thin composition root.

```text
apps/backend/Program.cs                 host setup and cross-cutting services
apps/backend/Features/<Feature>/        controller, commands, queries, domain, services, persistence
apps/backend/Shared/                    backend primitives used by multiple features
apps/backend/Database/Migrations/       runtime SQL migrations
apps/backend/tests/                     unit and integration tests
```

Each feature owns its registration through a small feature extension such as `AddAuthFeature`. Keep dependencies explicit; do not add mediator, ORM, mapping, or helper packages until the codebase has real pressure for them.

Data access uses Dapper directly. Global Dapper conventions, such as snake_case column mapping, are configured once in `Program.cs`; repositories should stay focused on SQL and mapping results.

Authentication cookies are owned by the auth feature through `AuthCookieService`. Controllers and handlers should depend on that service instead of reading protected cookies through extension methods or service-location.
