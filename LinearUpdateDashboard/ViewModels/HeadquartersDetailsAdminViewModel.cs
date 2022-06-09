using LinearUpdateDashboard.Models;

namespace LinearUpdateDashboard.ViewModels
{
    public class HeadquartersDetailsAdminViewModel
    {
        public Headquarters? Headquarters { get; set; } = new Headquarters();
        public List<DistributionServer> DistributionServers { get; set; } = new List<DistributionServer>();
    }
}
