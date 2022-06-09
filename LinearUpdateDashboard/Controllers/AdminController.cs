using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels;
using System.Xml.Linq;

namespace LinearUpdateDashboard.Controllers
{
    public class AdminController : Controller
    {
        private readonly LinearDbContext _context;

        public AdminController(LinearDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Markets
        public async Task<IActionResult> Markets()
        {
            var markets = await this._context.Markets.ToListAsync();
            var countDict = new Dictionary<int, int>();

            foreach (var market in markets)
            {
                var headquarters = await this.GetHeadquartersByMarketName(market.Name);
                countDict.Add(market.Id, headquarters.Count());
            }

            var model = new MarketListAdminViewModel()
            {
                Markets = markets,
                MarketHeadquartersCount = countDict
            };

            return _context.Markets != null ?
                            View(model) :
                            Problem("Entity set 'LinearDbContext.Markets'  is null.");
        }

        // GET: Admin/MarketDetails/liberty
        [HttpGet("Admin/MarketDetails/{name}")]
        public async Task<IActionResult> MarketDetails(string name)
        {
            var headquarters = await this.GetHeadquartersByMarketName(name);
            var countDict = new Dictionary<int, int>();

            if (name == null || this._context.Markets == null)
            {
                return this.NotFound();
            }

            foreach (var hq in headquarters)
            {
                var distributionServers = await this.GetDistributionServersByHeadquartersName(hq.Name);
                countDict.Add(hq.Id, distributionServers.Count());
            }

            var model = new MarketDetailsAdminViewModel()
{
                HeadquartersInMarket = headquarters,
                MarketHeadquartersDistributionServersCount = countDict
            };

            if (model == null)
            {
                return this.NotFound();
            }
            return View(model);
        }

        // GET: Admin/HeadquartersDetails/hqphl
        [HttpGet("Admin/HeadquartersDetails/{name}")]
        public async Task<IActionResult> HeadquartersDetails(string name)
        {
            if (name == null || this._context.Headquarters == null)
            {
                return this.NotFound();
            }
            var model = new HeadquartersDetailsAdminViewModel()
            {
                DistributionServers = await this.GetDistributionServersByHeadquartersName(name)
            };
            if (model == null)
            {
                return this.NotFound();
            }
            return View(model);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,LastUpdated")] Market market)
        {
            if (ModelState.IsValid)
            {
                _context.Add(market);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(market);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Markets == null)
            {
                return NotFound();
            }

            var market = await _context.Markets.FindAsync(id);
            if (market == null)
            {
                return NotFound();
            }
            return View(market);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,LastUpdated")] Market market)
        {
            if (id != market.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(market);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketExists(market.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(market);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Markets == null)
            {
                return NotFound();
            }

            var market = await _context.Markets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (market == null)
            {
                return NotFound();
            }

            return View(market);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Markets == null)
            {
                return Problem("Entity set 'LinearDbContext.Markets'  is null.");
            }
            var market = await _context.Markets.FindAsync(id);
            if (market != null)
            {
                _context.Markets.Remove(market);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarketExists(int id)
        {
          return (_context.Markets?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<List<Headquarters>> GetHeadquartersByMarketName(string name)
        {
            return await this._context.Markets
                .Where(m => m.Name == name)
                    .Include(hq => hq.Headquarters)
                        .ThenInclude(hq => hq.DistributionServers)
                            .SelectMany(m => m.Headquarters)
                            .ToListAsync();
        }

        public async Task<List<DistributionServer>> GetDistributionServersByHeadquartersName(string name)
        {
            return await this._context.Headquarters
                .Where(m => m.Name == name)
                    .Include(hq => hq.DistributionServers)
                        .SelectMany(m => m.DistributionServers)
                        .ToListAsync();
        }
    }
}
