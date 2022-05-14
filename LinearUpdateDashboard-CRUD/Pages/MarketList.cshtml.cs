#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LinearUpdateDashboard.Data;

namespace LinearUpdateDashboard.Pages
{
    public class MarketListModel : PageModel
    {
        private readonly LinearDbContext _context;

        public MarketListModel(LinearDbContext context)
        {
            _context = context;
        }

        public IList<Market> Markets { get; set; } = new List<Market>();
        public async Task<IActionResult> OnGetAsync()
        {
            Markets = await _context.Markets.ToListAsync();

            if (Markets == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
