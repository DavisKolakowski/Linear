using LinearUpdateDashboard.Data;
using LinearUpdateDashboard.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinearUpdateDashboard.Controllers
{
    public class SpotController : Controller
    {
        private readonly ILogger<SpotController> _logger;
        private readonly LinearDbContext _context;

        public SpotController(ILogger<SpotController> logger, LinearDbContext context) 
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
    }
}
