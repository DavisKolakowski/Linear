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
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using LinearWebService.Services;

namespace LinearWebService.Services
{
    public class LinearDatabaseSpotsTableService
    {
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly ILogger<RecurringJobScheduler> _logger;
        private readonly DapperContext _dapper;

        public LinearDatabaseSpotsTableService(IBackgroundJobClient backgroundJobs, ILogger<RecurringJobScheduler> logger, DapperContext dapper)
        {
            _backgroundJobs = backgroundJobs;
            _logger = logger;
            _dapper = dapper;
        }
        public void RunUpdateLinearDatabase()
        {
            var connString = _dapper.Connection().ConnectionString;

            var distServers = DistributionServerEncodeListFileService.GetDistributionServerDirectoryPaths(connString);

            foreach (var distServer in distServers)
            {
                var UpdateLinearDatabaseSpotsTable = _backgroundJobs.Enqueue<LinearDatabaseSpotsTableService>(m => m.RunDeleteDistributionServerSpots(distServer));
                _backgroundJobs.ContinueJobWith<LinearDatabaseSpotsTableService>(UpdateLinearDatabaseSpotsTable, m => m.RunImportDistributionServerSpots(distServer));
            }
        }
        [JobDisplayName("DeleteDistributionServerSpots")]
        public void RunDeleteDistributionServerSpots(DistributionServerModel distServer)
        {
            var conn = _dapper.Connection();
            try
            {
                _logger.LogTrace($"Deleting spots for {distServer.ServerIdentity}");
                Console.WriteLine($"Deleting spots for {distServer.ServerIdentity}");
                conn.Execute($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity = @DistributionServerIdentity", new { DistributionServerIdentity = distServer.ServerIdentity });
            }
            catch (BackgroundJobClientException jobEx)
            {
                throw new Exception(jobEx.Source);
            }
        }

        [JobDisplayName("ImportDistributionServerSpots")]
        public void RunImportDistributionServerSpots(DistributionServerModel distServer)
        {
            var conn = _dapper.Connection();
            var connString = conn.ConnectionString;
            try
            {
                _logger.LogInformation("Running update for {0}", distServer.ServerIdentity);
                Console.WriteLine("Running update for {0}", distServer.ServerIdentity);
                var items = DistributionServerEncodeListFileService.GetServerDirectorySpotsList(distServer.ServerFolder, connString);

                _logger.LogTrace("Starting Database Upload...");
                Console.WriteLine("Starting Database Upload...");
                items.ToList().ForEach(x =>
                    conn.Execute("exec dbo.InsertSpot @DistributionServerIdentity = @DistributionServerIdentity, @SpotCode = @SpotCode, @Name = @SpotTitle, @FirstAirDate = @FirstAirDate",
                    new
                    {
                        DistributionServerIdentity = distServer.ServerIdentity,
                        SpotCode = x.SpotCode,
                        SpotTitle = x.SpotTitle,
                        FirstAirDate = x.FirstAirDate
                    }));

                _logger.LogInformation("Distribution Server {0} has been successfully updated with {1} spots!", distServer.ServerFolder, items.Count());

                conn.Execute($"UPDATE dbo.DistributionServers SET LastSuccessfulDatabaseJob = @LastSuccessfulDatabaseJob WHERE ServerIdentity = @ServerIdentity",
                    new
                    {
                        ServerIdentity = distServer.ServerIdentity,
                        LastSuccessfulDatabaseJob = DateTime.Now
                    });
            }
            catch (DirectoryNotFoundException dirEx)
            {
                _logger.LogCritical("Distribution path {0} is invalid!", distServer.ServerFolder);
                throw new Exception($"Distribution path {distServer.ServerFolder} is invalid!", dirEx);
            }
            catch (BackgroundJobClientException jobEx)
            {
                throw new Exception(jobEx.Source);
            }
        }

        //[JobDisplayName("UpdatingDistributionServers")]
        //public void RunSendUpdateLinearDatabaseSpotsTable(DistributionServerModel distServer)
        //{
        //    var conn = _dapper.Connection();
        //    var connString = conn.ConnectionString;
        //    try
        //    {
        //        _logger.LogInformation("Running update for {0}", distServer.ServerFolder);
        //        Console.WriteLine("Running update for {0}", distServer.ServerFolder);
        //        var items = DistributionServerEncodeListFileService.GetServerDirectorySpotsList(distServer.ServerFolder, connString);

        //        _logger.LogTrace($"Deleting spots for {distServer.ServerFolder}");
        //        Console.WriteLine($"Deleting spots for {distServer.ServerFolder}");
        //        conn.Execute($"exec DeleteSpotsByDistributionServer @DistributionServerIdentity=@DistributionServerIdentity", new { DistributionServerIdentity = distServer.ServerIdentity });

        //        _logger.LogTrace("Starting Database Upload...");
        //        Console.WriteLine("Starting Database Upload...");
        //        items.ToList().ForEach(x =>
        //            conn.Execute("exec dbo.InsertSpot @DistributionServerIdentity = @DistributionServerIdentity, @SpotCode = @SpotCode, @Name = @SpotTitle, @FirstAirDate = @FirstAirDate",
        //            new
        //            {
        //                DistributionServerIdentity = distServer.ServerIdentity,
        //                SpotCode = x.SpotCode,
        //                SpotTitle = x.SpotTitle,
        //                FirstAirDate = x.FirstAirDate
        //            }));

        //        _logger.LogInformation("Distribution Server {0} has been successfully updated with {1} spots!", distServer.ServerFolder, items.Count());

        //        conn.Execute($"UPDATE dbo.DistributionServers SET LastSuccessfulDatabaseJob = @LastSuccessfulDatabaseJob WHERE ServerIdentity = @ServerIdentity",
        //            new {
        //                ServerIdentity = distServer.ServerIdentity,
        //                LastSuccessfulDatabaseJob = DateTime.Now
        //            });
        //    }
        //    catch (DirectoryNotFoundException dirEx)
        //    {
        //        _logger.LogCritical("Distribution path {0} is invalid!", distServer.ServerFolder);
        //        throw new Exception($"Distribution path {distServer.ServerFolder} is invalid!", dirEx);
        //    }
        //    catch (BackgroundJobClientException jobEx)
        //    {
        //        throw new Exception(jobEx.Source);
        //    }                    
        //}
    }
}
