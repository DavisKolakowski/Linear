using LinearUpdateDashboard.Models;

namespace LinearUpdateDashboard.ViewModels
{
    public class DataIntegrityViewModel
    {
        public HeadquartersDistributionServersModel HeadquartersDistributionServers { get; set; } = new HeadquartersDistributionServersModel();

        public Dictionary<int, int> DistributionServerSpotCount = new Dictionary<int, int>();
    }
}
