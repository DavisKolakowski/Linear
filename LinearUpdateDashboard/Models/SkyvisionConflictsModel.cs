#nullable disable

namespace LinearUpdateDashboard.Models
{
    public class Market
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Headquarters> Headquarters { get; } = new List<Headquarters>();
        public DateTime? LastUpdated { get; set; }
    }

    public class Headquarters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Market> Markets { get; } = new List<Market>();
        public List<DistributionServer> DistributionServers { get; } = new List<DistributionServer>();
        public DateTime? LastUpdated { get; set; }
    }

    public class DistributionServer
    {
        public int Id { get; set; }
        public string ServerIdentity { get; set; }
        public string ServerFolder { get; set; }
        public Headquarters Headquarters { get; set; }
        public List<DistributionServerSpot> DistributionServerSpots { get; } = new List<DistributionServerSpot>();
        public DateTime? LastUpdated { get; set; }
    }

    public class DistributionServerSpot
    {
        public int Id { get; set; }
        public DistributionServer DistributionServer { get; set; }
        public Spot Spot { get; set; }
        public DateTime? FirstAirDate { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class Spot
    {
        public int Id { get; set; }
        public string SpotCode { get; set; }
        public string Name { set; get; }
        public List<DistributionServerSpot> DistributionServerSpots { get; } = new List<DistributionServerSpot>();
        public DateTime? LastUpdated { get; set; }
    }
}