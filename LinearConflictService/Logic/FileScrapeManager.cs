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
        public static async Task<IEnumerable<DistributionServerModel>> GetDistributionServerAsync(string conString)
        {
            SqlConnection conn = new SqlConnection(conString);
            var ds = await conn.QueryAsync<DistributionServerModel>("SELECT * FROM dbo.DistributionServers WHERE ServerFolder IS NOT NULL");
            return ds;
        }
    }
}
