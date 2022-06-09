using Microsoft.Data.SqlClient;
using LinearConflictService.Logic;
using System.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Logging;
using Serilog.Sinks.SystemConsole;
using Dapper;
using Microsoft.Extensions.Configuration;

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
                            .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
                            .CreateLogger();

            Log.Logger.Information("Application Starting");

            var conString = config.GetConnectionString("LinearTestSQLDatabase");

            await UpdateDbService.UpdateDatabaseAsync(conString);
            Console.WriteLine("Task Complete!");
        }
    }
}
