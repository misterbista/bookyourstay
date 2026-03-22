using EzyMediatr.DependencyInjection;
using backend.Features.Auth.Domain;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Security;
using backend.Shared.Data.Migrations;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
const string FrontendDevCorsPolicy = "FrontendDev";

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddEzyMediatr();
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
            .AllowAnyMethod();
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.")));
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddSingleton<IPasswordHasher<AuthIdentity>, PasswordHasher<AuthIdentity>>();
builder.Services.AddSingleton<JwtTokenService>();

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
