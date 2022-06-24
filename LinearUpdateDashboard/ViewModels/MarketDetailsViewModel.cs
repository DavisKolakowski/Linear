namespace LinearUpdateDashboard.ViewModels
{
    using LinearUpdateDashboard.Models;

    public class MarketDetailsViewModel
    {
        /// <summary>Gets or sets the market.</summary>
        /// <value>The market.</value>
        public Market? Market { get; set; } = null;

        /// <summary>Gets or sets the spots in market.</summary>
        /// <value>The spots in market.</value>
        public List<Spot> SpotsInMarket { get; set; } = new List<Spot>();
    }
}
