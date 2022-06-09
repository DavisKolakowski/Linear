using LinearUpdateDashboard.Models;

namespace LinearUpdateDashboard.ViewModels
{
    public class MarketDetailsAdminViewModel
    {
        public Market? Market { get; set; } = null;
        public List<Headquarters> HeadquartersInMarket { get; set; } = new List<Headquarters>();

        public Dictionary<int, int> MarketHeadquartersDistributionServersCount = new Dictionary<int, int>();
    }
}
