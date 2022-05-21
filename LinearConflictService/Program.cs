using Microsoft.Data.SqlClient;
using LinearConflictService.Logic;
using System.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
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

            var conString = config.GetConnectionString("LinearSQLDatabase");

            await UpdateDbService.UpdateDatabaseAsync(conString);
        }
    }
}
