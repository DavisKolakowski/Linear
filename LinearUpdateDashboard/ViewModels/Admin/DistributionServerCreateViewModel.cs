namespace LinearUpdateDashboard.ViewModels.Admin
{
    using LinearUpdateDashboard.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DistributionServerCreateViewModel
    {
        public DistributionServer DistributionServer { get; set; } = new DistributionServer();

        public string? ServerIdentity { get; set; }

        public string? ServerFolder { get; set; }

        public int SelectedHeadquartersId { get; set; }

        public IEnumerable<SelectListItem> HeadquartersSelectedListItems { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
