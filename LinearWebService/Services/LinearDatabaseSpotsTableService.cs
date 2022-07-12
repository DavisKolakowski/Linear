using Dapper;

using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.Processing;
using Hangfire.Annotations;

using LinearWebService.Data;

using LinearWebService.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

using Serilog;

using System.Data;

using System.Text.RegularExpressions;
using System.Text;

namespace LinearWebService.Services
{
    public class LinearDatabaseSpotsTableService
    {
        public static Task RunUpdateLinearDatabase(string connString)
        {
            var conn = new SqlConnection(connString);

            var distServers = Task.Run(() => DistributionServerEncodeListFileService.GetDistributionServerDirectoryPathsAsync(conn)).Result;
            foreach (var distServer in distServers)
            {
                if (!Directory.Exists(distServer.ServerFolder))
                {
                    Log.Fatal("Distribution path {0} is invalid!", distServer.ServerFolder);
                    continue;
                }
                BackgroundJob.Enqueue(() => RunSendUpdateLinearDatabaseSpotsTable(distServer, conn));              
            }
            return Task.CompletedTask;
        }

        public static Task RunSendUpdateLinearDatabaseSpotsTable(DistributionServerModel distServer, SqlConnection conn)
        {
            var items = Task.Run(() => DistributionServerEncodeListFileService.GetServerDirectorySpotsListAsync(distServer.ServerFolder)).Result;
            Log.Verbose($"Deleting spots for {distServer.ServerFolder}");
            conn.Execute($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity=@DistributionServerIdentity", new { DistributionServerIdentity = distServer.ServerIdentity });
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

            return Task.CompletedTask;
        }
    }
}
