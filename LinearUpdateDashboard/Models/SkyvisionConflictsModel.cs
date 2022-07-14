#nullable disable

namespace LinearUpdateDashboard.Models
{
    public class Market
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the headquarters.</summary>
        /// <value>The headquarters.</value>
        public List<Headquarters> Headquarters { get; set; } = new List<Headquarters>();

        /// <summary>Gets or sets the last updated.</summary>
        /// <value>The last updated.</value>
        public DateTime? LastUpdated { get; set; }
    }

    public class Headquarters
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the markets.</summary>
        /// <value>The markets.</value>
        public List<Market> Markets { get; set; } = new List<Market>();

        /// <summary>Gets the distribution servers.</summary>
        /// <value>The distribution servers.</value>
        public List<DistributionServer> DistributionServers { get; } = new List<DistributionServer>();

        /// <summary>Gets or sets the last updated.</summary>
        /// <value>The last updated.</value>
        public DateTime? LastUpdated { get; set; }
    }

    public class DistributionServer
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the server identity.</summary>
        /// <value>The server identity.</value>
        public string ServerIdentity { get; set; }

        /// <summary>Gets or sets the server folder.</summary>
        /// <value>The server folder.</value>
        public string ServerFolder { get; set; }

        /// <summary>Gets or sets the headquarters identifier.</summary>
        /// <value>The headquarters identifier.</value>
        public int? HeadquartersId { get; set; }

        /// <summary>Gets or sets the headquarters.</summary>
        /// <value>The headquarters.</value>
        public Headquarters Headquarters { get; set; }

        /// <summary>Gets the distribution server spots.</summary>
        /// <value>The distribution server spots.</value>
        public List<DistributionServerSpot> DistributionServerSpots { get; } = new List<DistributionServerSpot>();

        /// <summary>Gets or sets the last updated.</summary>
        /// <value>The last updated.</value>
        public DateTime? LastUpdated { get; set; }

        /// <summary>Gets or sets the last successful spots update job</summary>
        /// <value>The last import.</value>
        public DateTime? LastSuccessfulDatabaseJob { get; set; }
    }

    public class DistributionServerSpot
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the distribution server.</summary>
        /// <value>The distribution server.</value>
        public DistributionServer DistributionServer { get; set; }

        /// <summary>Gets or sets the spot.</summary>
        /// <value>The spot.</value>
        public Spot Spot { get; set; }

        /// <summary>Gets or sets the first air date.</summary>
        /// <value>The first air date.</value>
        public DateTime? FirstAirDate { get; set; }

        /// <summary>Gets or sets the last updated.</summary>
        /// <value>The last updated.</value>
        public DateTime? LastUpdated { get; set; }
    }

    public class Spot
    {
        /// <summary>Gets or sets the identifier.</summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>Gets or sets the spot code.</summary>
        /// <value>The spot code.</value>
        public string SpotCode { get; set; }

        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets the distribution server spots.</summary>
        /// <value>The distribution server spots.</value>
        public List<DistributionServerSpot> DistributionServerSpots { get; } = new List<DistributionServerSpot>();

        /// <summary>Gets or sets the last updated.</summary>
        /// <value>The last updated.</value>
        public DateTime? LastUpdated { get; set; }
    }
}