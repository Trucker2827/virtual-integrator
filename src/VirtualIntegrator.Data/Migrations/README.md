# EF Core migrations

The initial schema is defined by the entity classes in `../Entities/` and the
model configuration in `../VirtualIntegratorDbContext.cs`. Migrations are
generated via the `dotnet ef` tooling — they are **not** hand-authored.

## First-time setup

1. Start Postgres:
   ```
   docker compose up -d postgres
   ```
2. From the repo root, install the EF Core tool if needed:
   ```
   dotnet tool install --global dotnet-ef --version 8.*
   ```
3. Generate the initial migration:
   ```
   dotnet ef migrations add Initial --project src/VirtualIntegrator.Data
   ```
4. Apply it:
   ```
   dotnet ef database update --project src/VirtualIntegrator.Data
   ```

The design-time `DbContext` factory in `../DesignTimeDbContextFactory.cs` reads
the connection string from the `VI_DB_CONNECTION` environment variable, falling
back to `Host=localhost;...Password=vi` which matches `compose.yml`.

## Adding a migration

After changing entity classes:

```
dotnet ef migrations add <Name> --project src/VirtualIntegrator.Data
dotnet ef database update --project src/VirtualIntegrator.Data
```

Migrations and the model snapshot (`VirtualIntegratorDbContextModelSnapshot.cs`)
are committed alongside the entity changes that produced them.
