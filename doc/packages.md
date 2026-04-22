# Packages

Shared TypeScript packages live under `packages/*` and are consumed by apps through workspace package names.

## Current Packages

```text
packages/shared/
|-- package.json
`-- src/
    |-- constants.ts
    |-- types.ts
    `-- utils.ts
```

Use `@bookyourstay/shared` for cross-app contracts and tiny framework-agnostic utilities. Keep app-specific UI, hooks, API clients, and feature code inside the owning app.
