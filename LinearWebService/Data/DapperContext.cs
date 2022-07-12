using Microsoft.Data.SqlClient;
using System.Data;

namespace LinearWebService.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IDbConnection Connection()
            => new SqlConnection(_configuration.GetConnectionString("LinearTestSQLDatabase"));
    }
}
