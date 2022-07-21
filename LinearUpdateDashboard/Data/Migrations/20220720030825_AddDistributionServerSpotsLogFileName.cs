using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinearUpdateDashboard.Data.Migrations
{
    public partial class AddDistributionServerSpotsLogFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpotsLogFileName",
                table: "DistributionServers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotsLogFileName",
                table: "DistributionServers");
        }
    }
}
