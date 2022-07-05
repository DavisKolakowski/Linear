using Microsoft.Data.SqlClient;
using LinearConflictService.Logic;
using System.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Logging;
using Serilog.Sinks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace LinearConflictService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json")
                            .AddUserSecrets<Program>()
                            .Build();

            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .ReadFrom.Configuration(config)
                            .CreateLogger();

            Log.Information("Application Starting");

            var conString = config.GetConnectionString("LinearTestSQLDatabase");

            await UpdateDbService.UpdateDatabaseAsync(conString);
            Log.Information("Task Complete!");
        }
    }
}
