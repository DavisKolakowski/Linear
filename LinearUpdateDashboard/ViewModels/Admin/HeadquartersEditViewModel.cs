namespace LinearUpdateDashboard.ViewModels.Admin
{
    using LinearUpdateDashboard.Models;

    using Microsoft.AspNetCore.Mvc.Rendering;

    public class HeadquartersEditViewModel
    {
        public Headquarters Headquarters { get; set; } = new Headquarters();

        public IEnumerable<SelectListItem> MarketSelectListItems { get; set; } = Enumerable.Empty<SelectListItem>();

        public int[] SelectedMarketIds { get; set; }
    }
}
