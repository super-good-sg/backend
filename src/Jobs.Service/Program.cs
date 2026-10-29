using Microsoft.EntityFrameworkCore;
using SuperGood.Jobs.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Runtime connection uses this service's own database user (ADR 0005). Migrations are
// applied by the deploy pipeline with the migration role, never at startup.
builder.Services.AddDbContext<JobsDbContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("Jobs"),
    npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", JobsDbContext.SchemaName)));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok());

app.Run();
