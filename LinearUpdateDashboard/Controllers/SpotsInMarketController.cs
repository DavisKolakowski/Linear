using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.ViewModels;
using LinearUpdateDashboard.Components.DataTables;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using LinearUpdateDashboard.DataModels;

namespace LinearUpdateDashboard.Controllers
{
    public class SpotsInMarketController : Controller
    {
        private readonly ILogger<SpotsInMarketController> _logger;
        private readonly LinearDbContext _context;
        private readonly MarketDetailsViewModel _model;

        public SpotsInMarketController(ILogger<SpotsInMarketController> logger, LinearDbContext context, MarketDetailsViewModel model)
        {
            _logger = logger;
            _context = context;
            _model = model;
        }

        // GET: SpotsInMarketController/Details/name
        [HttpGet("Home/Details/{name}")]
        public async Task<IActionResult> Details(String name, JsonResult jsonResult)
        {
            if (string.IsNullOrEmpty(name))
            {
                NotFound();
            }
            if (!_context.Markets.Any(m => m.Name == name))
            {
                this.NotFound();
            }
            _model.SpotsInMarket = await _context.Markets
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
            for (int i = 0; i < _model.SpotsInMarket.Count(); i++)
            {
                var items = new MarketDetailsDataModel();
                foreach (var s in _model.SpotsInMarket)
                {
                    items.Id = i;
                    items.SpotIdentifier = s.SpotCode;
                    items.SpotTitle = s.Name;
                    foreach (var dss in s.DistributionServerSpots.DistinctBy(s => new { s.FirstAirDate, s.Spot.Id }))
                    {
                        items.SpotFirstAir = dss.FirstAirDate;
                    }
                }
               // _model.DataModel.Add(items);
            }
            return View();
        }
        [BindProperty]
        public TableBase Table { get; set; } = new TableBase();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(JsonResult jsonResult)
        {
            
            var recordsTotal = _model.DataModel.Count();
            var sQuery = _model.DataModel.AsQueryable();

            var searchText = Table.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                sQuery = sQuery.Where(s =>
                    s.SpotIdentifier.ToUpper().Contains(searchText) ||
                    s.SpotTitle.ToUpper().Contains(searchText) ||
                    s.SpotFirstAir.ToString().ToUpper().Contains(searchText)
                );
            }
            var recordsFiltered = sQuery.Count();

            var sortColumnName = Table.Columns.ElementAt(Table.Order.ElementAt(0).Column).Name;
            var sortDirection = Table.Order.ElementAt(0).Dir.ToLower();

            sQuery = sQuery.OrderBy($"{sortColumnName} {sortDirection}");

            var skip = Table.Start;
            var take = Table.Length;
            var data = await sQuery
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            jsonResult = new JsonResult(new
            {
                Draw = Table.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            });
            return View(jsonResult);
        }
    }
}
