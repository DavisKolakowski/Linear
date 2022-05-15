using LinearUpdateDashboard.Models;
using LinearUpdateDashboard.DataModels;
using LinearUpdateDashboard.Components.DataTables;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.AspNetCore.Mvc;

namespace LinearUpdateDashboard.ViewModels
{
    public class MarketDetailsViewModel
    {
        int Id { get; set; }
        public Market? Market { get; set; } = null;
        public List<Spot> SpotsInMarket { get; set; } = new List<Spot>();
        public List<MarketDetailsDataModel> DataModel { get; set; } = new List<MarketDetailsDataModel>();
    }
}
