using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels.Admin;
using System.Xml.Linq;
using NuGet.Packaging.Signing;

namespace LinearUpdateDashboard.Controllers
{
    public class HeadquartersController : Controller
    {
        private readonly ILogger<HeadquartersController> _logger;

        private readonly LinearDbContext _context;

        public HeadquartersController(LinearDbContext context, ILogger<HeadquartersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Headquarters
        [HttpGet("Headquarters/Admin")]
        public async Task<IActionResult> Admin()
        {
            
            var markets = await this.GetMarketsListAsync();

            var marketHqs = new MarketHeadquarters();
            foreach (var market in markets)
            {
                marketHqs.MarketId = market.Id;
                marketHqs.Markets.Add(market);
            }
            var model = new HeadquartersListViewModel()
            {
                MarketHeadquarters = marketHqs,
            };
            
            return _context.Headquarters != null ?
                            View(model) :
                            Problem("Entity set 'LinearDbContext.Headquarters'  is null.");
        }

        public async Task<List<Market>> GetMarketsListAsync()
        {
            var markets = await _context.Markets
                    .Include(m => m.Headquarters)
                .Distinct()
                .ToListAsync();
            return markets;
        }

        // GET: Headquarters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Headquarters == null)
            {
                return NotFound();
            }

            var model = new HeadquartersDetailsViewModel()
            {
                Headquarters = await GetHeadquartersDetailsAsync(id)
            };

            if (model.Headquarters == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public async Task<Headquarters> GetHeadquartersDetailsAsync(int? id)
        {
            var headquarters = await _context.Headquarters
                .Where(hq => hq.Id == id)
                    .Include(hq => hq.DistributionServers)
                    .Include(hq => hq.Markets)  
                .FirstOrDefaultAsync();
            return headquarters;
        }

        // GET: Headquarters/Create
        [HttpGet("Headquarters/Create")]
        public IActionResult Create()
        {
            var viewModel = new HeadquartersCreateViewModel()
            {
                MarketSelectListItems = _context.Markets
                    .Select(m => new SelectListItem
                    {
                        Text = m.Name,
                        Value = m.Id.ToString(),
                    }).AsEnumerable(),
            };

            return View(viewModel);
        }

        // POST: Headquarters/Create
        [HttpPost("Headquarters/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HeadquartersCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var hq = new Headquarters();

                hq.Name = viewModel.HeadquartersName;
                hq.LastUpdated = DateTime.Now;
                for (var i = 0; i < viewModel.SelectedMarketIds.Length; i++)
                {
                    var market = _context.Markets.First(m => m.Id == viewModel.SelectedMarketIds[i]);
                    hq.Markets.Add(market);
                }

                _context.Add(hq);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Admin));
            }
            return View(viewModel);
        }

        // GET: Headquarters/Edit/5
        [HttpGet("Headquarters/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Headquarters == null)
            {
                return NotFound();
            }

            var headquarters = await _context.Headquarters
                .Include(hq => hq.Markets)
                .FirstAsync(hq => hq.Id == id);

            var viewModel = new HeadquartersEditViewModel();

            viewModel.Headquarters = headquarters;
            viewModel.SelectedMarketIds = _context.Headquarters
                .Where(hq => hq.Id == id)
                .Include(hq => hq.Markets)
                .SelectMany(hq => hq.Markets)
                .Select(m => m.Id)
                .ToArray();
            viewModel.MarketSelectListItems = _context.Markets
                .Include(m => m.Headquarters)
                .ToList().Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString(),
                }).AsEnumerable();
            
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: Headquarters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Headquarters/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HeadquartersCreateViewModel viewModel)
        {           
            if (ModelState.IsValid)
            {
                var hq = await _context.Headquarters                    
                        .Include(hq => hq.Markets)
                        .FirstAsync(hq => hq.Id == id);

                hq.Markets.Clear();

                hq.Name = viewModel.HeadquartersName;
                hq.LastUpdated = DateTime.Now;               

                for (var i = 0; i < viewModel.SelectedMarketIds.Length; i++)
                {
                    var market = _context.Markets.SingleOrDefault(m => m.Id == viewModel.SelectedMarketIds[i]);                    
                    hq.Markets.Add(market);                 
                }
                
                _context.Update(hq);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Admin));
            }
            return View(viewModel);
        }

        // GET: Headquarters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Headquarters == null)
            {
                return NotFound();
            }

            var headquarters = await _context.Headquarters
                .FirstOrDefaultAsync(hq => hq.Id == id);
                
            if (headquarters == null)
            {
                return NotFound();
            }

            return View(headquarters);
        }

        // POST: Headquarters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Headquarters == null)
            {
                return Problem("Entity set 'LinearDbContext.Headquarters'  is null.");
            }
            var headquarters = await _context.Headquarters.FindAsync(id);
            if (headquarters != null)
            {
                _context.Headquarters.Remove(headquarters);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }

        private bool HeadquartersMarketExists(int id)
        {
            return (_context.Markets?.Any(m => m.Id == id)).GetValueOrDefault();
        }

        public async Task<List<Headquarters>> GetHeadquartersForMarketsListAsync(int marketId)
        {
            var headquarters = await _context.Markets
                .Where(m => m.Id == marketId)
                    .Include(m => m.Headquarters)
                    .SelectMany(m => m.Headquarters)
                .ToListAsync();
            return headquarters;
        }

        public async Task<List<DistributionServer>> GetDistributionServersByHeadquartersIdAsync(int id)
        {
            return await this._context.Markets
                .Where(m => m.Id == id)
                    .Include(m => m.Headquarters)
                        .ThenInclude(hq => hq.DistributionServers)
                            .SelectMany(m => m.Headquarters)
                            .SelectMany(hq => hq.DistributionServers)
                            .ToListAsync();
        }

        public async Task<List<int>> GetHeadquartersMarketIdAsync(int id)
        {
            var headquarters = await this._context.Headquarters
                    .Where(hq => hq.Id == id)
                    .Include(hq => hq.Markets)
                        .SelectMany(hq => hq.Markets)
                        .Select(m => m.Id)
                        .ToListAsync();
            return headquarters;
        }
    }
}
