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
|-- hooks/         # React hooks
`-- lib/           # Utilities
```

## UI Notes

- Path aliases use `@/*`.
- Global styles live in `app/globals.css`.
- Shared utility helpers live in `lib/utils.ts`.
