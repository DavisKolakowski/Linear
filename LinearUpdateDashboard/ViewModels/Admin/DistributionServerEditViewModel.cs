namespace LinearUpdateDashboard.ViewModels.Admin
{
    using LinearUpdateDashboard.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    using System.ComponentModel.DataAnnotations;
    using System.Xml.Linq;

    public class DistributionServersEditViewModel
    {
        public DistributionServer DistributionServer { get; set; } = new DistributionServer();

        public int HeadquartersId { get; set; }

        public int? SelectedHeadquartersId { get; set; }

        public IEnumerable<SelectListItem> HeadquartersSelectedListItems { get; set; } = Enumerable.Empty<SelectListItem>();      
    }
}
