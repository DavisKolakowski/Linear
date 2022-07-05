namespace LinearUpdateDashboard.ViewModels.Admin
{
    using LinearUpdateDashboard.Models;
    public class HeadquartersListViewModel
    {       
        public MarketHeadquarters MarketHeadquarters { get; set; } = new MarketHeadquarters();

    }
    public class MarketHeadquarters
    {
        public int MarketId { get; set; }

        public List<Market> Markets { get; set; } = new List<Market>();

    }
}
