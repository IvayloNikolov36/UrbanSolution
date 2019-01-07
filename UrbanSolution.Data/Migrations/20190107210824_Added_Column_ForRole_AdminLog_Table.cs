using Microsoft.EntityFrameworkCore.Migrations;

namespace UrbanSolution.Data.Migrations
{
    public partial class Added_Column_ForRole_AdminLog_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "AdminLogs",
                newName: "CreatedOn");

            migrationBuilder.AddColumn<string>(
                name: "ForRole",
                table: "AdminLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForRole",
                table: "AdminLogs");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "AdminLogs",
                newName: "DateTime");
        }
    }
}
