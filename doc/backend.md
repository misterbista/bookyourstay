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
- The current architecture is feature-first, with the auth slice implemented end to end.
