using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class RemovedUrbanIssueStreetNumberColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreetNumber",
                table: "UrbanIssues");

            migrationBuilder.AlterColumn<string>(
                name: "AddressStreet",
                table: "UrbanIssues",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 30);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AddressStreet",
                table: "UrbanIssues",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 60);

            migrationBuilder.AddColumn<string>(
                name: "StreetNumber",
                table: "UrbanIssues",
                nullable: false,
                defaultValue: "");
        }
    }
}
