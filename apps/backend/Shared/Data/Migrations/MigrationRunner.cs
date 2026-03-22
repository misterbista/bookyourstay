using DbUp;
using DbUp.Engine;

namespace backend.Shared.Data.Migrations;

public sealed class MigrationRunner(IConfiguration configuration, IWebHostEnvironment environment)
{
    public void RunAll()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        var scriptsPath = Path.Combine(environment.ContentRootPath, "Database", "Migrations");
        if (!Directory.Exists(scriptsPath))
        {
            return;
        }

        var scriptFiles = Directory.GetFiles(scriptsPath, "*.sql", SearchOption.AllDirectories)
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (scriptFiles.Length == 0)
        {
            return;
        }

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScripts(scriptFiles.Select(path => new SqlScript(
                Path.GetRelativePath(environment.ContentRootPath, path).Replace('\\', '/'),
                File.ReadAllText(path))).ToArray())
            .JournalToPostgresqlTable("public", "schema_versions")
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            throw new InvalidOperationException("Failed to apply startup migrations.", result.Error);
        }
    }
}
