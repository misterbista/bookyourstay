# Architecture

This repo is a small polyglot monorepo. Keep it boring, explicit, and feature-first.

## Repository Boundaries

```text
apps/backend      ASP.NET Core API and backend tests
apps/frontend     Next.js app
```

Do not add root packages until there is real same-language reuse across workspaces. The C# backend cannot consume TypeScript contracts directly, so frontend API shapes belong in the frontend unless generated contracts are introduced.

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

Shared frontend infrastructure, such as the generic HTTP client, belongs in `apps/frontend/lib`. Feature endpoints should wrap that infrastructure from inside the owning feature.

Each frontend feature should keep endpoint path constants next to its API functions. Each backend feature should keep route strings on the controller until multiple controllers need to share them. That keeps URLs discoverable without introducing generated clients before the project needs them.

## Backend

The backend is feature-first with a thin composition root.

```text
apps/backend/Program.cs                 host setup and cross-cutting services
apps/backend/Features/<Feature>/        controller, domain, services, persistence
apps/backend/Shared/                    backend primitives used by multiple features
apps/backend/Database/Migrations/       runtime SQL migrations
apps/backend/tests/                     unit and integration tests
```

Each feature owns its registration through a small feature extension such as `AddAuthFeature`. Keep dependencies explicit; do not add mediator, ORM, mapping, or helper packages until the codebase has real pressure for them.

Data access uses Dapper directly. Global Dapper conventions, such as snake_case column mapping, are configured once in `Program.cs`; repositories should stay focused on SQL and mapping results.

Authentication cookies are owned by the auth feature through `AuthCookieService`. Controllers read HTTP cookies and claims at the boundary, then pass plain values into feature services.

Auth endpoints that issue cookies should use the auth feature's `ToAuthActionResult` extension so token-cookie writes and public session responses stay consistent.

Protect backend feature routes with standard ASP.NET Core `[Authorize]`. The auth feature registers a cookie access-token authentication scheme that reads the protected access-token cookie, validates the JWT, checks that the session is still active, and then builds `HttpContext.User`.
