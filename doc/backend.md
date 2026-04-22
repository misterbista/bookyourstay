# Backend

The backend is an ASP.NET Core 10 API organized by feature.

## Structure

```text
apps/backend/
|-- Database/      # SQL migrations
|-- Features/      # Feature-first application code
|-- Shared/        # Shared backend primitives
`-- tests/         # Unit and integration tests
```

## Common Commands

From the repository root:

```bash
bun run dev:backend
bun run build:backend
bun run test:backend
```

Direct .NET equivalents:

```bash
dotnet watch --project apps/backend
dotnet build apps/backend/backend.csproj
dotnet test apps/backend/tests/backend.UnitTests/backend.UnitTests.csproj
dotnet test apps/backend/tests/backend.IntegrationTests/backend.IntegrationTests.csproj
```

## Notes

- Startup migrations run automatically in normal environments.
- Integration tests disable startup migrations with `Database:RunMigrationsOnStartup=false`.
- The current architecture is feature-first: controllers, domain objects, services, registration, and persistence live inside the owning feature.
- Shared backend primitives stay in `Shared/`; feature-specific helpers stay inside the feature.
- The backend intentionally uses Dapper and direct feature services. Avoid adding mediator or ORM packages unless the codebase has a concrete need.
- Auth cookie names, protection, and expiry behavior are centralized in `AuthCookieService`.
- Auth endpoints that issue cookies use the shared auth result mapper instead of controller-local response helpers.
- Feature route strings live on the controller until multiple controllers need to share them.
- Protected API routes use standard ASP.NET Core `[Authorize]` with the auth feature's cookie access-token scheme.
