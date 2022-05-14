using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using LinearUpdateDashboard.Data;

namespace LinearUpdateDashboard.Data
{
    public class LinearDbContext : DbContext
    {
        public LinearDbContext()
        {
        }

        public LinearDbContext(DbContextOptions<LinearDbContext> options)
            : base(options)
        {
        }

        public DbSet<DistributionServer> DistributionServers { get; set; } = null!;
        public DbSet<DistributionServerSpot> DistributionServerSpots { get; set; } = null!;
        public DbSet<Headquarters> Headquarters { get; set; } = null!;
        public DbSet<Market> Markets { get; set; } = null!;
        public DbSet<Spot> Spots { get; set; } = null!;
    }
}
