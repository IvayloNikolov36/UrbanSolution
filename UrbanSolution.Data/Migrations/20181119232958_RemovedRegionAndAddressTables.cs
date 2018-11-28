using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class RemovedRegionAndAddressTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UrbanIssues_Address_AddressId",
                table: "UrbanIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_UrbanIssues_Regions_RegionId",
                table: "UrbanIssues");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_UrbanIssues_AddressId",
                table: "UrbanIssues");

            migrationBuilder.DropIndex(
                name: "IX_UrbanIssues_RegionId",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "UrbanIssues");

            migrationBuilder.AddColumn<string>(
                name: "AddressStreet",
                table: "UrbanIssues",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Region",
                table: "UrbanIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "UrbanIssues",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressStreet",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "UrbanIssues");

            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "UrbanIssues");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "UrbanIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "UrbanIssues",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StreetName = table.Column<string>(maxLength: 30, nullable: false),
                    StreetNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegionName = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrbanIssues_AddressId",
                table: "UrbanIssues",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_UrbanIssues_RegionId",
                table: "UrbanIssues",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UrbanIssues_Address_AddressId",
                table: "UrbanIssues",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UrbanIssues_Regions_RegionId",
                table: "UrbanIssues",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
