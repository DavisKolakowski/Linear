using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace LinearUpdateDashboard.DataModels
{
    public class MarketDetailsDataModel
    {
        public int Id { get; set; }
        public string SpotIdentifier { get; set; } = string.Empty;
        public string SpotTitle { get; set; } = string.Empty;
        public DateTime? SpotFirstAir { get; set; } = DateTime.MinValue;
    }
}
