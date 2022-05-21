using LinearConflictService.Models;

using Microsoft.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;

namespace LinearConflictService.Logic
{
    internal class FileScrapeManager
    {
        public static async Task<List<DistributionServerModel>> GetDistributionServerAsync(string conString)
{
            SqlConnection conn = new SqlConnection(conString);
            await conn.OpenAsync();

            var servers = new List<DistributionServerModel>();
            foreach(var server in servers)
            {
                await conn.ExecuteAsync("SELECT * FROM dbo.DistributionServers WHERE ServerFolder=@ServerFolder",
                    new
                    {
                        ServerFolder = server.ServerFolder
                    });
                servers.Add(server);
            }
            return servers;
        }
    }
}
