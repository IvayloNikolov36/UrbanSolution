namespace UrbanSolution.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;
    using UrbanSolution.Data.MigrationHelpers;

    public partial class UsersWithRolesView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ViewsHelper.CreateUsersWithRolesView(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ViewsHelper.DropUsersWithRolesView(migrationBuilder);
        }
    }
}
