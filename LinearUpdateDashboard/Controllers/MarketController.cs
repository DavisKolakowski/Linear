using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinearUpdateDashboard.Controllers
{
    public class MarketController : Controller
    {
        private readonly ILogger<MarketController> _logger;
        private readonly LinearDbContext _context;

        public MarketController(ILogger<MarketController> logger, LinearDbContext context) 
        {
            this._logger = logger;
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = new MarketListViewModel()
            {
                Markets = await _context.Markets.ToListAsync()
            };
          
            return this.View(model);
        }

        [HttpGet("Market/{name}/export")] 
        public async Task<IActionResult> ExportMarket(string name)
        {
            var currentDate = DateTime.Now;

            // TODO: Return the list of spots and put it in a csv file here.  Use CSVHelper or something and return the byte[] to the user in the this.File() response
            return this.File(new byte[0], "application/octet-stream", $"{currentDate.ToString("yyyyddMM")}-{name}.csv");
        }

        [HttpGet("Market/{name}")]
        public async Task<IActionResult> Details(string name, int currentPage = 1, int numItems = 3)
        {
            var market = await this.GetMarketByName(name);
            var model = new MarketDetailsViewModelTwo();
            var spotsInMarket = await this.GetTotalSpotsByMarket(market);
            model.Market = market;
            model.SpotsInMarket = await this.GetSpotsByMarket(market, currentPage, numItems);
            model.PagingModel = new SpotPageModel()
            {
                MarketName = name,
                CurrentPage = currentPage,
                ItemsPerPage = numItems,
                TotalItems = spotsInMarket,
                NumPages = (int)Math.Ceiling((double)spotsInMarket / numItems)
            };

            return this.View(model);
        }

        private async Task<Market?> GetMarketByName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return null;
            }

            return await this._context.Markets
                .Include(hq => hq.Headquarters)
                        .ThenInclude(hqds => hqds.DistributionServers)
                            .ThenInclude(ds => ds.DistributionServerSpots)
                                .ThenInclude(dss => dss.Spot)
                                    .ThenInclude(spot => spot.DistributionServerSpots)
                                        .ThenInclude(dss => dss.DistributionServer)
                .FirstOrDefaultAsync(m => m.Name == name);
        }

        private async Task<List<Spot>> GetAllSpotsByMarketAsync(Market? market)
        {
            if (market == null)
            {
                return null;
            }
            var spots = await this._context.Markets
                .Where(m => m.Id == market.Id)
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

            return spots;
        }

        private async Task<int> GetTotalSpotsByMarket(Market? market)
        {
            if (market == null)
            {
                return -1;
            }
            var spotCount = await this._context.Markets
                .Where(m => m.Id == market.Id)
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
                    .CountAsync();

            return spotCount;
        }

        private async Task<List<Spot>> GetSpotsByMarket(Market? market, int page=1, int pageSize=10)
        {
            if(market == null)
            {
                return null;
            }
            var spots = await this._context.Markets
                .Where(m=>m.Id == market.Id)
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
                    .OrderBy(s => s.SpotCode)
                    .Skip((page - 1) * pageSize).Take(pageSize)
                    .ToListAsync();

            return spots; 
        }
    }
}
