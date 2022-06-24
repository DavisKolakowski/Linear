// <copyright file="MarketsController.cs" company="Comcast">
// Copyright (c) Comcast. All Rights Reserved.
// </copyright>

namespace LinearUpdateDashboard.Data
{
    using LinearUpdateDashboard.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    ///   <br />
    /// </summary>
    public class LinearDbContext : DbContext
    {
        /// <summary>Initializes a new instance of the <see cref="LinearDbContext" /> class.</summary>
        /// <remarks>See <a href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</a>
        /// for more information.</remarks>
        public LinearDbContext()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LinearDbContext" /> class.</summary>
        /// <param name="options">The options.</param>
        public LinearDbContext(DbContextOptions<LinearDbContext> options)
            : base(options)
        {
        }

        /// <summary>Gets or sets the distribution servers.</summary>
        /// <value>The distribution servers.</value>
        public DbSet<DistributionServer> DistributionServers { get; set; } = null!;

        /// <summary>Gets or sets the distribution server spots.</summary>
        /// <value>The distribution server spots.</value>
        public DbSet<DistributionServerSpot> DistributionServerSpots { get; set; } = null!;

        /// <summary>Gets or sets the headquarters.</summary>
        /// <value>The headquarters.</value>
        public DbSet<Headquarters> Headquarters { get; set; } = null!;

        /// <summary>Gets or sets the markets.</summary>
        /// <value>The markets.</value>
        public DbSet<Market> Markets { get; set; } = null!;

        /// <summary>Gets or sets the spots.</summary>
        /// <value>The spots.</value>
        public DbSet<Spot> Spots { get; set; } = null!;
    }
}
