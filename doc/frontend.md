# Frontend

The frontend is a Next.js 16 app inside the monorepo workspace.

## Stack

- Next.js App Router
- React 19
- TypeScript
- Tailwind CSS 4
- shadcn/ui

## Common Commands

From the repository root:

```bash
bun run dev:frontend
bun run build:frontend
bun run lint:frontend
bun run typecheck:frontend
```

From `apps/frontend` directly:

```bash
bun run dev
bun run build
bun run lint
bun run typecheck
```

## Structure

```text
apps/frontend/
|-- app/           # App Router entrypoints
|-- components/    # UI and shared components
|-- features/      # Feature-first product code
|-- shared/        # Frontend-only layout and navigation
`-- lib/           # Utilities
```

## UI Notes

- Path aliases use `@/*`.
- Frontend-only API shapes live with the owning feature.
- Generic API request behavior lives in `lib/api-client.ts`; feature-specific endpoints live under the owning feature.
- Feature endpoint paths live beside feature API functions. Keep them in the API file until multiple files need them.
- Demo-only frontend content should be named as demo data, for example `home-demo-data.ts`, until it is replaced by API data.
- Global styles live in `app/globals.css`.
- Shared utility helpers live in `lib/utils.ts`.
