namespace LinearUpdateDashboard.ViewModels.Admin
{
    using LinearUpdateDashboard.Models;
    //public class DistributionServersListViewModel
    //{
    //    public List<DistributionServer> DistributionServers { get; set; } = new List<DistributionServer>();
    //}
    public class DistributionServersListViewModel
    {
        public HeadquartersDistributionServers HeadquartersDistributionServers { get; set; } = new HeadquartersDistributionServers();
    }
    public class HeadquartersDistributionServers
    {
        public int? HeadquartersId { get; set; }

        public List<DistributionServer> DistributionServers { get; set; } = new List<DistributionServer>();

    }
}
