namespace LinearUpdateDashboard.Models
{
    public class MarketHeadquartersModel
    {
        public int MarketId { get; set; }

        public List<Market> Markets { get; set; } = new List<Market>();
    }
}
