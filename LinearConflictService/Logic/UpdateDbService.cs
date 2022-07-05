using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

using Dapper;

using LinearConflictService.Models;

using Microsoft.Data.SqlClient;

using Serilog;

using static System.Net.Mime.MediaTypeNames;

namespace LinearConflictService.Logic
{
    internal class UpdateDbService
    {
        public static async Task UpdateDatabaseAsync(string conString)
        {
            SqlConnection conn = new SqlConnection(conString);
            await conn.OpenAsync();

            var distServers = await FileScrapeManager.GetDistributionServerAsync(conString);
    
            foreach (var distServer in distServers)
            {
                if (!Directory.Exists(distServer.ServerFolder))
                {
                    Log.Fatal("Distribution path {0} is invalid!", distServer.ServerFolder);
                    continue;
                }

                var items = await FileScraperService.GetServerDirectoryAsync(distServer.ServerFolder);

                Log.Verbose($"Deleting spots for {distServer.ServerFolder}");
                await conn.ExecuteAsync($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity=@DistributionServerIdentity", new { DistributionServerIdentity = distServer.ServerIdentity });
                Log.Verbose("Starting Database Upload...");
                items.ToList().ForEach(async x =>
                    await conn.ExecuteAsync("exec dbo.InsertSpot @DistributionServerIdentity = @DistributionServerIdentity, @SpotCode = @SpotCode, @Name = @SpotTitle, @FirstAirDate = @FirstAirDate",
                    new
                    {
                        DistributionServerIdentity = distServer.ServerIdentity,
                        SpotCode = x.SpotCode,
                        SpotTitle = x.SpotTitle,
                        FirstAirDate = x.FirstAirDate
                    }));               
                Log.Information("Distribution Server {0} has been successfully updated with {1} spots!", distServer.ServerFolder, items.Count());
            }
        }
    }
}
