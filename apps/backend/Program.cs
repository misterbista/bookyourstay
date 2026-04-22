using backend.Features.Auth;
using backend.Shared.Data.Migrations;
using Dapper;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
const string FrontendDevCorsPolicy = "FrontendDev";

DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendDevCorsPolicy, policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    return false;

                return uri.Host is "localhost" or "127.0.0.1";
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.")));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAuthFeature(builder.Configuration);

var shouldRunMigrations = builder.Configuration.GetValue("Database:RunMigrationsOnStartup", !builder.Environment.IsEnvironment("Testing"));
if (shouldRunMigrations)
{
    var migrationRunner = new MigrationRunner(builder.Configuration, builder.Environment);
    migrationRunner.RunAll();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(FrontendDevCorsPolicy);
}

app.UseExceptionHandler();
app.UseStatusCodePages();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () =>
        Results.Ok(new
        {
            name = "BookYourStay API",
            version = "v1",
            architecture = "micro-feature"
        }))
    .WithTags("Root");

app.MapControllers();

app.Run();

public partial class Program;
