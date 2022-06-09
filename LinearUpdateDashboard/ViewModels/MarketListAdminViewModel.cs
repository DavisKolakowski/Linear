using LinearUpdateDashboard.Models;

namespace LinearUpdateDashboard.ViewModels
{
    public class MarketListAdminViewModel
    {
        public List<Market> Markets { get; set; } = new List<Market>();
        public Dictionary<int, int> MarketHeadquartersCount = new Dictionary<int, int>();
    }
}
