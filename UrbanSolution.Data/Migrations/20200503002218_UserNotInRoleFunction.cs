namespace UrbanSolution.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;
    using UrbanSolution.Data.MigrationHelpers;

    public partial class UserNotInRoleFunction : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            FunctionsHelpers.CreateGetUserNotInRolesFunction(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            FunctionsHelpers.DropGetUserNotInRolesFunction(migrationBuilder);
        }
    }
}
