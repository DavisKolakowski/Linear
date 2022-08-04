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
            var markets = await this.GetMarketsListAsync();

            var marketHqsDss = new MarketHeadquartersDistributionServersModel();

            foreach (var market in markets)
            {
                marketHqsDss.MarketId = market.Id;
                marketHqsDss.Markets.Add(market);

                foreach (var headquarters in market.Headquarters)
                {
                    marketHqsDss.MarketHeadquartersId = headquarters.Id;
                    marketHqsDss.DistributionServers = headquarters.DistributionServers;
                }
            }

            var model = new DataIntegrityViewModel()
            {
                MarketHeadquartersDistributionServers = marketHqsDss,
            };

            return _context.Headquarters != null ?
                            View(model) :
                            Problem("Entity set 'LinearDbContext.Headquarters'  is null.");
        }

        public async Task<List<Market>> GetMarketsListAsync()
        {
            var markets = await _context.Markets
                    .Include(m => m.Headquarters)
                        .ThenInclude(hq => hq.DistributionServers)
                .Distinct()
                .ToListAsync();
            return markets;
        }
    }
}
