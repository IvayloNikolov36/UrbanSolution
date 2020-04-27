namespace UrbanSolution.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;
    using UrbanSolution.Data.MigrationHelpers;

    public partial class RemoveUserRoleProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ProcedureHelpers.CreateRemoveUserRoleProcedure(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ProcedureHelpers.DropRemoveUserRoleProcedure(migrationBuilder);
        }
    }
}
