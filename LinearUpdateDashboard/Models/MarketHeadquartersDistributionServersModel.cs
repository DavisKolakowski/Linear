namespace LinearUpdateDashboard.Models
{
    public class MarketHeadquartersDistributionServersModel
    {
        public int MarketId { get; set; }

        public List<Market> Markets { get; set; } = new List<Market>();

        public int? MarketHeadquartersId { get; set; }

        public List<DistributionServer> DistributionServers { get; set; } = new List<DistributionServer>();
    }
}
