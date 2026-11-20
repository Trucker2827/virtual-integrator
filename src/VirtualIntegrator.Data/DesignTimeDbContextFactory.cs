using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtualIntegrator.Data;

// Used by `dotnet ef` tooling (migrations add, database update) when it can't
// construct the DbContext via DI. Reads the connection string from the
// VI_DB_CONNECTION environment variable, falling back to the local docker-compose
// defaults so `dotnet ef migrations add` just works after `docker compose up -d`.
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<VirtualIntegratorDbContext>
{
    public VirtualIntegratorDbContext CreateDbContext(string[] args)
    {
        var connString = Environment.GetEnvironmentVariable("VI_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=virtualintegrator;Username=vi;Password=vi";

        var options = new DbContextOptionsBuilder<VirtualIntegratorDbContext>()
            .UseNpgsql(connString)
            .Options;

        return new VirtualIntegratorDbContext(options);
    }
}
