using System.Net.Http;
using Application.Api.GraphQL;
using Application.Common.Logging;
using Application.Database;
using Application.Import.ConcordiumNode;
using Application.Persistence;
using Concordium.NodeApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var performDatabaseMigration = args.FirstOrDefault()?.ToLower() == "migrate-db";

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.With<SourceClassNameEnricher>()    
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddGraphQLServer().AddQueryType<Query>();
    
builder.Services.AddHostedService<ImportController>();
builder.Services.AddSingleton<GrpcClient>();
builder.Services.AddSingleton<DatabaseMigrator>();
builder.Services.AddSingleton<BlockRepository>();
builder.Services.AddSingleton(new HttpClient());
builder.Services.AddSingleton(builder.Configuration.GetSection("PostgresDatabase").Get<DatabaseSettings>());
builder.Services.AddSingleton(builder.Configuration.GetSection("ConcordiumNodeGrpc").Get<GrpcClientSettings>());
builder.Host.UseSystemd();
var app = builder.Build();

var logger = Log.ForContext<Program>();

try
{
    if (performDatabaseMigration)
    {
        logger.Information("Application started in database migration mode. Starting database migration...");
        app.Services.GetRequiredService<DatabaseMigrator>().MigrateDatabase();
        logger.Information("Database migration finished successfully");
    }
    else
    {
        logger.Information("Application starting...");
        app.Services.GetRequiredService<DatabaseMigrator>().EnsureDatabaseMigrationNotNeeded();
        
        app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
            });
        
        app.Run();    
    }
}
catch (Exception e)
{
    logger.Fatal(e, "Unhandled exception caught. Terminating application.");

    // TODO: Do we need to signal to the process host that we are terminating due to an exception? throw?
}

logger.Information("Exiting application!");