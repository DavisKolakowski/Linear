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
                var items = await FileScraperService.ReadSpotsListAsync(@$"C:\{distServer.ServerFolder}\c$\encodelist");
                Console.WriteLine($"Deleting spots for {distServer.ServerFolder}");
                await conn.ExecuteAsync($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity='{distServer.ServerFolder}'");
                Console.WriteLine("Starting Database Upload...");
                foreach (var item in items)
                {
                    await conn.ExecuteAsync("exec dbo.InsertSpot DistributionServerIdentity = @DistributionServerIdentity, SpotCode = @SpotCode, Name = @SpotTitle, FirstAirDate = @FirstAirDate",
                        new
                        {
                            DistributionServerIdentity = distServer.ServerFolder,
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
}
