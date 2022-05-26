using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace LinearUpdateDashboard.Controllers
{

    public class MarketsController : Controller
    {
        private readonly ILogger<MarketsController> _logger;

        private readonly LinearDbContext _context;

        public MarketsController(LinearDbContext context, ILogger<MarketsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Markets
        public async Task<IActionResult> Index()
        {
            var markets = await this._context.Markets.ToListAsync();
            var countDict = new Dictionary<int,int>();
    
            foreach(var market in markets) {
                var spots = await this.GetSpotsByMarketName(market.Name);
                countDict.Add(market.Id, spots.Count());
            }

            var model = new MarketListViewModel()
            {
                Markets = markets,
                MarketSpotCount = countDict
            };
            return _context.Markets != null ?
                            View(model) :
                            Problem("Entity set 'LinearDbContext.Markets'  is null.");
        }

        // GET: Market/Details/liberty
        [HttpGet("{name}")]
        public async Task<IActionResult> Details(string name)
        {
            if (name == null || this._context.Markets == null)
            {
                return this.NotFound();
            }
            var model = new MarketDetailsViewModel()
            {
                SpotsInMarket = await this.GetSpotsByMarketName(name)
            };
            if (model == null)
            {
                return this.NotFound();
            }
            return View(model);
        }

        public async Task<List<Spot>> GetSpotsByMarketName(string name) 
        {
            return await this._context.Markets
                    .Where(m => m.Name == name)
                    .Include(hq => hq.Headquarters)
                            .ThenInclude(hqds => hqds.DistributionServers)
                                .ThenInclude(ds => ds.DistributionServerSpots)
                                    .ThenInclude(dss => dss.Spot)
                                        .ThenInclude(spot => spot.DistributionServerSpots)
                                            .ThenInclude(dss => dss.DistributionServer)
                        .SelectMany(m => m.Headquarters)
                        .SelectMany(hq => hq.DistributionServers)
                        .SelectMany(ds => ds.DistributionServerSpots)
                        .Select(dss => dss.Spot)
                        .Distinct()
                        .ToListAsync();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
