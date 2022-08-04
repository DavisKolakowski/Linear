namespace LinearUpdateDashboard.Models
{
    public class HeadquartersDistributionServersModel
    {
        public int? HeadquartersId { get; set; }

        public List<DistributionServer> DistributionServers { get; set; } = new List<DistributionServer>();
    }
}
