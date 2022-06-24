namespace LinearUpdateDashboard.ViewModels
{
    using LinearUpdateDashboard.Models;

    public class MarketListViewModel
    {
        /// <summary>Gets or sets the markets.</summary>
        /// <value>The markets.</value>
        public List<Market> Markets { get; set; } = new List<Market>();

        /// <summary>The market spot count</summary>
        public Dictionary<int, int> MarketSpotCount = new Dictionary<int, int>();
    }
}
