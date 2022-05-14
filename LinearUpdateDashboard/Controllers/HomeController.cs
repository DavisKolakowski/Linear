using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels;
using LinearUpdateDashboard.Components.DataTables;

using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using System.Diagnostics;
using System.Data;
using X.PagedList;
using LinearUpdateDashboard.DataModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Linq;
using System.Security.Claims;

namespace LinearUpdateDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LinearDbContext _context;

        public HomeController(ILogger<HomeController> logger, LinearDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new MarketListViewModel()
            {
                Markets = await _context.Markets.ToListAsync()
            };
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [BindProperty]
        public TableBase Table { get; set; } = new TableBase();
        [HttpPost("Home/Details/{name}")]
        public async Task<MarketDetailsViewModel> GetMarketAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                NotFound();
            }
            if (!_context.Markets.Any(m => m.Name == name))
            {
                this.NotFound();
            }
            var model = new MarketDetailsViewModel()
            {
                SpotsInMarket = await _context.Markets
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
                        .ToListAsync()
            };
            return model;
        }
        public async Task<List<MarketDetailsDataModel>> SetSpotsListAsync(string name)
        {
            var model = await GetMarketAsync(name);
            var data = new List<MarketDetailsDataModel>();
            foreach (var item in model.SpotsInMarket)
            {
                foreach (var dss in item.DistributionServerSpots.DistinctBy(s => new { s.FirstAirDate, s.Spot.Id }))
                {
                    var list = new MarketDetailsDataModel()
                    {
                        SpotIdentifier = item.SpotCode,
                        SpotTitle = item.Name,
                        SpotFirstAir = dss.FirstAirDate
                    };
                    data.Add(list);
                }
            }
            return data;
        }
        public async Task<IActionResult> Details(string name)
        {
            var spotsList = await SetSpotsListAsync(name);
            var recordsTotal = spotsList.Count();

            var sQuery = spotsList.AsQueryable();

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

            var jsonResult = new JsonResult(new
            {
                Draw = Table.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            });
            return View(jsonResult);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}