# Packages

There are no root packages right now.

Add a package only when at least two same-language workspaces consume it. A TypeScript package does not add value for sharing contracts with the C# backend unless the project introduces generated clients or generated schemas.

Keep frontend API shapes in the owning frontend feature. Keep backend response contracts in the owning backend feature.
