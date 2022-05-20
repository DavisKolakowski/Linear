using LinearUpdateDashboard.Models;

namespace LinearUpdateDashboard.ViewModels
{
    public class MarketDetailsViewModel
    {
        public Market? Market { get; set; } = null;
        public List<Spot> SpotsInMarket { get; set; } = new List<Spot>();
    }
}
