using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinearUpdateDashboard.Data.Migrations
{
    public partial class AddDistributionServerSpotsLogLastWriteTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SpotsLogLastWriteTime",
                table: "DistributionServers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotsLogLastWriteTime",
                table: "DistributionServers");
        }
    }
}
