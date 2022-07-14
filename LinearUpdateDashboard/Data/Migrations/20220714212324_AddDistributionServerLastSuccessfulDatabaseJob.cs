using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinearUpdateDashboard.Data.Migrations
{
    public partial class AddDistributionServerLastSuccessfulDatabaseJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSuccessfulDatabaseJob",
                table: "DistributionServers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSuccessfulDatabaseJob",
                table: "DistributionServers");
        }
    }
}
