using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels;
using LinearUpdateDashboard.ViewModels.Admin;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinearUpdateDashboard.Controllers
{
    public class DataIntegrityController : Controller
    {
        private readonly ILogger<DataIntegrityController> _logger;

        private readonly LinearDbContext _context;

        public DataIntegrityController(LinearDbContext context, ILogger<DataIntegrityController> logger)
        {
            _context = context;

            _logger = logger;
        }

        // GET: DataIntegrity
        [HttpGet("DataIntegrity/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var distributionServers = await this.GetDistributionServersListAsync();

            var hqsDss = new HeadquartersDistributionServersModel();
            var countDict = new Dictionary<int, int>();

            foreach (var distributionServer in distributionServers)
            {
                hqsDss.HeadquartersId = distributionServer.HeadquartersId;
                hqsDss.DistributionServers.Add(distributionServer);
                var spots = await this.GetSpotsByDistributionServerIdentity(distributionServer.ServerIdentity);
                countDict.Add(distributionServer.Id, spots.Count());
            }

            var model = new DataIntegrityViewModel()
            {
                HeadquartersDistributionServers = hqsDss,
                DistributionServerSpotCount = countDict,
            };

            return _context.Headquarters != null ?
                            View(model) :
                            Problem("Entity set 'LinearDbContext.Headquarters'  is null.");
        }

        public async Task<List<DistributionServer>> GetDistributionServersListAsync()
        {
            var distributionServers = await _context.DistributionServers
                .Include(ds => ds.Headquarters)
                    .ThenInclude(hq => hq.Markets)
                .ToListAsync();
            return distributionServers;
        }

        public async Task<List<Spot>> GetSpotsByDistributionServerIdentity(string serverIdentity)
        {
            this._logger.LogInformation("Retrieving spots for {0}", serverIdentity);
            return await this._context.DistributionServers
                .Where(ds => ds.ServerIdentity == serverIdentity)
                        .Include(ds => ds.DistributionServerSpots)
                            .ThenInclude(dss => dss.Spot)
                    .SelectMany(ds => ds.DistributionServerSpots)
                    .Select(dss => dss.Spot)
                    .ToListAsync();
        }
    }
}
