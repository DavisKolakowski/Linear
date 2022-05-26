using LinearUpdateDashboard.Models;

namespace LinearUpdateDashboard.ViewModels
{
    public class MarketListViewModel
    {
        public List<Market> Markets { get; set; } = new List<Market>();
        public Dictionary<int, int> MarketSpotCount = new Dictionary<int, int>();
    }
}
