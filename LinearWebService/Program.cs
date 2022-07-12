using Hangfire;
using Hangfire.SqlServer;

using LinearWebService.Data;
using LinearWebService.Services;

using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Dapper;
using Serilog;

using System.Data;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connString = builder.Configuration.GetConnectionString("LinearTestSQLDatabase");
var conn = new SqlConnection(connString);
await conn.OpenAsync();

builder.Services.AddScoped<DapperContext>();

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

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

RecurringJob.AddOrUpdate<LinearDatabaseSpotsTableService>(m => LinearDatabaseSpotsTableService.RunUpdateLinearDatabase(connString), "*/1 * * * *");

app.Run();