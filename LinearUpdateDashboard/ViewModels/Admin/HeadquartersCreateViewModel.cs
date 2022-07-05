namespace LinearUpdateDashboard.ViewModels.Admin
{
    using LinearUpdateDashboard.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class HeadquartersCreateViewModel
    {
        public Headquarters Headquarters { get; set; } = new Headquarters();

        public string HeadquartersName { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> MarketSelectListItems { get; set; } = Enumerable.Empty<SelectListItem>();

        public int[] SelectedMarketIds { get; set; }
    }
}
