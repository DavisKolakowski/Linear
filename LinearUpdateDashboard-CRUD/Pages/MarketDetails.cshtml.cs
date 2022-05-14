#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Components.DataTables;
using System.Xml.Linq;
using System.Data;
using System.Linq.Dynamic.Core;

namespace LinearUpdateDashboard.Pages
{
    public class MarketDetailsModel : PageModel
    {
        private readonly LinearDbContext _context;

        public MarketDetailsModel(LinearDbContext context)
        {
            _context = context;
        }       
        public async Task<JsonResult> OnGetAsync()
        {
            return await OnPostAsync();
        }
        [BindProperty]
        public MarketDetailsDataModel DataModel { get; set; }
        public TableBase TableBase { get; set; }
        public async Task<JsonResult> OnPostAsync()
        {
            var recordsTotal = DataModel.SpotsInMarket.Count();

            var sQuery = _context.Markets
                 .Where(m => m.Name == DataModel.Market.Name)
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
                 .AsQueryable();

            var searchText = TableBase.Search.Value?.ToUpper();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                sQuery = sQuery.Where(s =>
                    s.SpotCode
                        .ToUpper()
                        .Contains(searchText) ||
                    s.Name
                        .ToUpper()
                        .Contains(searchText)
                //s.FirstAirDate
                //    .ToString()
                //    .Contains(searchText)
                );
            }

            var recordsFiltered = sQuery.Count();

            var sortColumnName = TableBase.Columns.ElementAt(TableBase.Order.ElementAt(0).Column).Name;
            var sortDirection = TableBase.Order.ElementAt(0).Dir.ToLower();

            sQuery = sQuery.OrderBy($"{sortColumnName} {sortDirection}");

            var skip = TableBase.Start;
            var take = TableBase.Length;
            var data = await sQuery
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            new JsonResult(new
            {
                Draw = TableBase.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            });
            return OnGetAsync().Result;
        }
        //// POST: Markets/Details/liberty
        //[BindProperty]
        //public TableBase DataTablesRequest { get; set; }
        //public async Task<JsonResult> Details(string name)
        //{
        //    var model = new MarketDetailsViewModel()
        //    {
        //        SpotsInMarket = await _context.Markets
        //                .Where(m => m.Name == name)
        //                .Include(hq => hq.Headquarters)
        //                    .ThenInclude(hqds => hqds.DistributionServers)
        //                        .ThenInclude(ds => ds.DistributionServerSpots)
        //                            .ThenInclude(dss => dss.Spot)
        //                                .ThenInclude(spot => spot.DistributionServerSpots)
        //                                    .ThenInclude(dss => dss.DistributionServer)
        //                .SelectMany(m => m.Headquarters)
        //                .SelectMany(hq => hq.DistributionServers)
        //                .SelectMany(ds => ds.DistributionServerSpots)
        //                .Select(dss => dss.Spot)
        //                .Distinct()
        //                .ToListAsync()
        //    };
        //    var items = new SpotMapperDataModel();
        //    foreach (var item in model.SpotsInMarket)
        //    {
        //        item.SpotCode = items.SpotIdentifier;
        //        item.Name = items.SpotTitle;
        //        foreach (var dss in item.DistributionServerSpots.DistinctBy(s => new { s.FirstAirDate, s.Spot.Id }))
        //        {
        //            dss.FirstAirDate = items.SpotFirstAir;
        //        }
        //    }
        //    var recordsTotal = model.SpotsInMarket.Count();
        //    var sQuery = model.SpotsInMarket.AsQueryable();
        //    var searchText = DataTablesRequest.Search.Value?.ToUpper();
        //    if (!string.IsNullOrWhiteSpace(searchText))
        //    {
        //        sQuery = sQuery.Where(s =>
        //            s.SpotCode
        //                .ToUpper()
        //                .Contains(searchText) ||
        //            s.Name
        //                .ToUpper()
        //                .Contains(searchText));
        //    }
        //    var recordsFiltered = sQuery.Count();

        //    var sortColumnName = DataTablesRequest.Columns.ElementAt(DataTablesRequest.Order.ElementAt(0).Column).Name;
        //    var sortDirection = DataTablesRequest.Order.ElementAt(0).Dir.ToLower();

        //    // using System.Linq.Dynamic.Core
        //    sQuery = sQuery.OrderBy($"{sortColumnName} {sortDirection}");

        //    var skip = DataTablesRequest.Start;
        //    var take = DataTablesRequest.Length;
        //    var data = await sQuery
        //        .Skip(skip)
        //        .Take(take)
        //        .ToListAsync();
        //    var view = new JsonResult(new
        //    {
        //        Draw = DataTablesRequest.Draw,
        //        RecordsTotal = recordsTotal,
        //        RecordsFiltered = recordsFiltered,
        //        Data = data
        //    });
        //    return Json(view);
        //}
    }
    public class MarketDetailsDataModel
    {
        public Market Market { get; set; }
        public IList<Spot> SpotsInMarket { get; set; } = new List<Spot>();
    }
}
