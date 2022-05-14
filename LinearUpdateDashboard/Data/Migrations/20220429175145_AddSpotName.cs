using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinearUpdateDashboard.Data.Migrations
{
    public partial class AddSpotName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Spots",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Spots");
        }
    }
}
