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
using System.Globalization;

namespace LinearUpdateDashboard.Controllers
{
    public class DistributionServersController : Controller
    {
        private readonly ILogger<DistributionServersController> _logger;

        private readonly LinearDbContext _context;

        public DistributionServersController(LinearDbContext context, ILogger<DistributionServersController> logger)
        {
            _context = context;

            _logger = logger;
        }

        // GET: DistributionServers
        [HttpGet("DistributionServers/Admin")]
        public async Task<IActionResult> Admin()
        {
            DistributionServersListViewModel model = new DistributionServersListViewModel()
            {
                DistributionServers = await GetDistributionServersListAsync()
            };

            return model != null ? 
                        View(model) :
                        Problem("Entity set 'LinearDbContext.DistributionServers'  is null.");
        }

        public async Task<List<DistributionServer>> GetDistributionServersListAsync()
        {
            var distributionServers = await _context.DistributionServers
                .Include(ds => ds.Headquarters)
                .ToListAsync();
            return distributionServers;
        }

        // GET: DistributionServers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DistributionServers == null)
            {
                return NotFound();
            }
            
            DistributionServersDetailsViewModel model = new DistributionServersDetailsViewModel()
            {
                DistributionServer = await GetDistributionServersDetailsAsync(id)
            };

            if (model.DistributionServer == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public async Task<DistributionServer> GetDistributionServersDetailsAsync(int? id)
        {
            var distributionServer = await _context.DistributionServers
                .Where(m => m.Id == id)
                .Include(ds => ds.Headquarters)
                .FirstOrDefaultAsync();
            return distributionServer;
        }

        // GET: DistributionServers/Create
        [HttpGet("DistributionServers/Create")]
        public IActionResult Create()
        {
            var model = new DistributionServerCreateViewModel()
            {
                HeadquartersSelectedListItems = _context.Headquarters
                    .Select(hq => new SelectListItem
                    {
                        Value = hq.Id.ToString(),
                        Text = hq.Name,
                    }).AsEnumerable(),
            };            

            return View(model);
        }

        // POST: DistributionServers/Create
        [HttpPost("DistributionServers/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DistributionServerCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ds = new DistributionServer
                {
                    ServerIdentity = model.ServerIdentity,
                    ServerFolder = model.ServerFolder,
                    HeadquartersId = model.SelectedHeadquartersId,
                    LastUpdated = DateTime.Now,
                };
                _context.Add(ds);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Admin));
            }

            return View(model);
        }

        // GET: DistributionServers/Edit/5
        [HttpGet("DistributionServers/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DistributionServers == null)
            {
                return NotFound();
            }

            var result = await EditDistributionServerAsync(id);

            var model = new DistributionServersEditViewModel()
            {
                DistributionServer = result,
                SelectedHeadquartersId = result.HeadquartersId,
                HeadquartersSelectedListItems = _context.Headquarters
                    .Select(hq => new SelectListItem
                    {
                        Value = hq.Id.ToString(),
                        Text = hq.Name,
                        Selected = hq.Id == result.HeadquartersId,
                    }).AsEnumerable(),
            };

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        public async Task<DistributionServer> EditDistributionServerAsync(int? id)
        {
            var distributionServer = await _context.DistributionServers
                .Include(ds => ds.Headquarters)
                .FirstAsync(ds => ds.Id == id);
            return distributionServer;
        }

        // POST: DistributionServers/Edit/5
        [HttpPost("DistributionServers/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DistributionServersEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dsFromDb = await _context.DistributionServers
                        .SingleAsync(ds => ds.Id == id);
                dsFromDb.ServerIdentity = model.DistributionServer.ServerIdentity;
                dsFromDb.ServerFolder = model.DistributionServer.ServerFolder;
                dsFromDb.HeadquartersId = model.SelectedHeadquartersId;
                dsFromDb.LastUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Admin));
            }

            return View(model);
        }

        // GET: DistributionServers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DistributionServers == null)
            {
                return NotFound();
            }

            var distributionServer = await _context.DistributionServers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (distributionServer == null)
            {
                return NotFound();
            }

            return View(distributionServer);
        }

        // POST: DistributionServers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DistributionServers == null)
            {
                return Problem("Entity set 'LinearDbContext.DistributionServers'  is null.");
            }
            var distributionServer = await _context.DistributionServers.FindAsync(id);
            if (distributionServer != null)
            {
                _context.DistributionServers.Remove(distributionServer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Admin));
        }

        private bool DistributionServerExists(int id)
        {
          return (_context.DistributionServers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
