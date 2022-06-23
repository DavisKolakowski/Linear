using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;
using System.Xml.Linq;

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
            _logger.LogTrace("Retrieving market list");
            var markets = await this._context.Markets.ToListAsync();
            var countDict = new Dictionary<int,int>();
    
            foreach(var market in markets) {
                _logger.LogTrace("Getting spot count for {0}", market.Name);
                var spots = await this.GetSpotsByMarketName(market.Name);
                _logger.LogTrace("Adding {0} spots to {1}'s dictionary", spots.Count, market.Name);
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
            _logger.LogTrace("Attempting to find {0} in {1}", name, _context.Markets);
            if (name == null || this._context.Markets == null)
            {
                _logger.LogInformation("{0} cannot be found in {1}", name, _context.Markets);
                return this.Error();
            }          
            var model = new MarketDetailsViewModel()
            {
                SpotsInMarket = await this.GetSpotsByMarketName(name)
            };
            if (model == null)
            {
                _logger.LogError("{0} failed to retrieve data for {1}", model, name);
                return this.Error();
            }
            return View(model);
        }

        public async Task<List<Spot>> GetSpotsByMarketName(string name) 
        {
            _logger.LogTrace("Retrieving spots for {0}", name);
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
