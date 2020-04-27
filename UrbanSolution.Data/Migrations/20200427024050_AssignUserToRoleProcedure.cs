using Microsoft.EntityFrameworkCore.Migrations;
using UrbanSolution.Data.MigrationHelpers;

namespace UrbanSolution.Data.Migrations
{
    public partial class AssignUserToRoleProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ProcedureHelpers.CreateAssignUserToRoleProcedure(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ProcedureHelpers.DropAssignUserToRoleProcedure(migrationBuilder);
        }
    }
}
