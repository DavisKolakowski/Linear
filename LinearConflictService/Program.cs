using Microsoft.Data.SqlClient;
using LinearConflictService.Logic;
using System.Data;
using LinearConflictService.Models;
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

            string distServer = "vaacdnyc01";
            string conflLog = "042522";
            var items = await FileService.ReadSpotsListAsync(distServer, conflLog);
                SqlConnection conn = new SqlConnection(conString);
                await conn.OpenAsync();
                Console.WriteLine($"Deleting spots for {distServer}");
                await conn.ExecuteAsync($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity='{distServer}'");
                Console.WriteLine("Starting Database Upload...");
                foreach (var item in items)
                    {
                        await conn.ExecuteAsync("exec dbo.InsertSpot @DistributionServerIdentity = @DistributionServerIdentity, @SpotCode = @SpotCode, @Name = @SpotTitle, @FirstAirDate = @FirstAirDate",
                            new { DistributionServerIdentity = distServer,
                                SpotCode = item.SpotCode,
                                SpotTitle = item.SpotTitle,
                                FirstAirDate = item.FirstAirDate
                            });
                        Console.WriteLine($"{item.SpotCode}, {item.SpotTitle}, {item.FirstAirDate}");
                    }
            Console.WriteLine($"Upload Complete! {items.Count()} Spots have been uploaded");
        }
    }
}
