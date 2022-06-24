// <copyright file="MarketsController.cs" company="Comcast">
// Copyright (c) Comcast. All Rights Reserved.
// </copyright>

namespace LinearUpdateDashboard.Controllers
{
    using System.Diagnostics;
    using System.Xml.Linq;
    using LinearUpdateDashboard.Data;
    using LinearUpdateDashboard.Models;
    using LinearUpdateDashboard.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    ///   <br />
    /// </summary>
    public class MarketsController : Controller
    {
        private readonly ILogger<MarketsController> _logger;

        private readonly LinearDbContext _context;

        /// <summary>Initializes a new instance of the <see cref="MarketsController" /> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public MarketsController(LinearDbContext context, ILogger<MarketsController> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        // GET: Markets

        /// <summary>
        ///   <br />
        /// </summary>
        /// <returns>A <see cref="Task{TResult}" /> representing the result of the asynchronous operation.</returns>
        public async Task<IActionResult> Index()
        {
            this._logger.LogInformation("Retrieving market list");
            var markets = await this._context.Markets.ToListAsync();
            var countDict = new Dictionary<int,int>();

            foreach (var market in markets)
            {
                this._logger.LogInformation("Getting spot count for {0}", market.Name);
                var spots = await this.GetSpotsByMarketName(market.Name);
                this._logger.LogTrace("Adding {0} spots to {1}'s dictionary", spots.Count, market.Name);
                countDict.Add(market.Id, spots.Count());
            }

            var model = new MarketListViewModel()
            {
                Markets = markets,
                MarketSpotCount = countDict,
            };
            return this._context.Markets != null ?
                            this.View(model) :
                            this.Problem("Entity set 'LinearDbContext.Markets'  is null.");
        }

        // GET: Market/Details/liberty

        /// <summary>Details the specified name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   Market Spots List.
        /// </returns>
        [HttpGet("{name}")]
        public async Task<IActionResult> Details(string name)
        {
            this._logger.LogTrace("Attempting to find {0} in {1}", name, this._context.Markets);
            if (name == null || this._context.Markets == null)
            {
                this._logger.LogInformation("{0} cannot be found in {1}", name, this._context.Markets);
                return this.Error();
            }

            var model = new MarketDetailsViewModel()
            {
                SpotsInMarket = await this.GetSpotsByMarketName(name),
            };
            if (model == null)
            {
                this._logger.LogError("{0} failed to retrieve data for {1}", model, name);
                return this.Error();
            }

            return this.View(model);
        }

        /// <summary>Gets the name of the spots by market.</summary>
        /// <param name="name">The name.</param>
        /// <returns>Spots by Market Name.</returns>
        public async Task<List<Spot>> GetSpotsByMarketName(string name)
        {
            this._logger.LogInformation("Retrieving spots for {0}", name);
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

        /// <summary>Errors this instance.</summary>
        /// <returns>Error View.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
