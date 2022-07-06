namespace LinearUpdateDashboard.ViewModels.Admin
{
    using LinearUpdateDashboard.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DistributionServerCreateViewModel
    {
        public DistributionServer DistributionServer { get; set; } = new DistributionServer();

        public string ServerIdentity { get; set; } = string.Empty;

        public string ServerFolder { get; set; } = string.Empty;

        public int SelectedHeadquartersId { get; set; }

        public IEnumerable<SelectListItem> HeadquartersSelectedListItems { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
