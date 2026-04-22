# BookYourStay

Feature-first monorepo:

- `apps/frontend` - Next.js App Router frontend
- `apps/backend` - ASP.NET Core API

Local development:

```bash
cp .env.example .env
bun run bootstrap
bun run infra:up
bun run dev:backend
bun run dev:frontend
```

Repository documentation lives under [doc/](doc/README.md).
