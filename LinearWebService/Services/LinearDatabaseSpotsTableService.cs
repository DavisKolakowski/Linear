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
        public static void RunUpdateLinearDatabase(string connString)
        {
            var conn = new SqlConnection(connString);

            var distServers = conn.Query<DistributionServerModel>("SELECT * FROM dbo.DistributionServers WHERE ServerFolder IS NOT NULL");

            foreach (var distServer in distServers)
            {
                if (!Directory.Exists(distServer.ServerFolder))
                {
                    Log.Fatal("Distribution path {0} is invalid!", distServer.ServerFolder);
                    continue;
                }
                else
                {
                    try
                    {
                        BackgroundJob.Enqueue<LinearDatabaseSpotsTableService>(m => RunSendUpdateLinearDatabaseSpotsTable(distServer, connString));
                    }
                    catch (BackgroundJobClientException jobEx)
                    {
                        throw new Exception(jobEx.Source);
                    }
                }
            }
        }
        public static void RunSendUpdateLinearDatabaseSpotsTable(DistributionServerModel distServer, string connString)
        {          
            var conn = new SqlConnection(connString);

            Log.Information("Running update for {0}", distServer.ServerFolder);
            var items = DistributionServerEncodeListFileService.GetServerDirectorySpotsList(distServer.ServerFolder);
            Log.Verbose($"Deleting spots for {distServer.ServerFolder}");
            conn.Execute($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity=@DistributionServerIdentity", new { DistributionServerIdentity = distServer.ServerIdentity });
            Log.Verbose("Starting Database Upload...");
            items.ToList().ForEach(x =>
                conn.Execute("exec dbo.InsertSpot @DistributionServerIdentity = @DistributionServerIdentity, @SpotCode = @SpotCode, @Name = @SpotTitle, @FirstAirDate = @FirstAirDate",
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
