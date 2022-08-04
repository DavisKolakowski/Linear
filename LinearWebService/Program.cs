using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.SqlServer;

using LinearWebService.Data;
using LinearWebService.Services;

using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Dapper;
using Serilog;

using System.Data;
using System.Diagnostics;
using LinearWebService.Models;
using Hangfire.Client;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Common;
using Microsoft.EntityFrameworkCore;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Configuration.AddJsonFile("appsettings.json");

    builder.Services.AddScoped<LogEventsFormatModel>();
    builder.Services.AddScoped<DapperContext>();

    var connString = builder.Configuration.GetConnectionString("LinearTestSQLDatabase");

    builder.Services.AddLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddSerilog();
    });

    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration.MinimumLevel.Debug();
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    });

    builder.Services.AddHangfireServer();
    builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            })
            .UseSerilogLogProvider()
    );
    
    builder.Services.AddControllers();

    var app = builder.Build();
    // Configure the HTTP request pipeline.

    app.UseHttpsRedirection();

    app.UseHangfireDashboard();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHangfireDashboard();
        endpoints.MapControllers();
    });

    RecurringJob.AddOrUpdate<LinearDatabaseSpotsTableService>("UpdateLinearDatabase", x => x.RunUpdateLinearDatabase(), "*/2 * * * *");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception has occurred");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}