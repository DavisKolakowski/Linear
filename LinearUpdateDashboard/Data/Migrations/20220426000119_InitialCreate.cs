using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinearUpdateDashboard.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Headquarters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Headquarters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpotCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DistributionServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerIdentity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerFolder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadquartersId = table.Column<int>(type: "int", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionServers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributionServers_Headquarters_HeadquartersId",
                        column: x => x.HeadquartersId,
                        principalTable: "Headquarters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeadquartersMarket",
                columns: table => new
                {
                    HeadquartersId = table.Column<int>(type: "int", nullable: false),
                    MarketsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadquartersMarket", x => new { x.HeadquartersId, x.MarketsId });
                    table.ForeignKey(
                        name: "FK_HeadquartersMarket_Headquarters_HeadquartersId",
                        column: x => x.HeadquartersId,
                        principalTable: "Headquarters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeadquartersMarket_Markets_MarketsId",
                        column: x => x.MarketsId,
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DistributionServerSpots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistributionServerId = table.Column<int>(type: "int", nullable: true),
                    SpotId = table.Column<int>(type: "int", nullable: true),
                    FirstAirDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionServerSpots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistributionServerSpots_DistributionServers_DistributionServerId",
                        column: x => x.DistributionServerId,
                        principalTable: "DistributionServers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DistributionServerSpots_Spots_SpotId",
                        column: x => x.SpotId,
                        principalTable: "Spots",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistributionServers_HeadquartersId",
                table: "DistributionServers",
                column: "HeadquartersId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributionServerSpots_DistributionServerId",
                table: "DistributionServerSpots",
                column: "DistributionServerId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributionServerSpots_SpotId",
                table: "DistributionServerSpots",
                column: "SpotId");

            migrationBuilder.CreateIndex(
                name: "IX_HeadquartersMarket_MarketsId",
                table: "HeadquartersMarket",
                column: "MarketsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributionServerSpots");

            migrationBuilder.DropTable(
                name: "HeadquartersMarket");

            migrationBuilder.DropTable(
                name: "DistributionServers");

            migrationBuilder.DropTable(
                name: "Spots");

            migrationBuilder.DropTable(
                name: "Markets");

            migrationBuilder.DropTable(
                name: "Headquarters");
        }
    }
}
