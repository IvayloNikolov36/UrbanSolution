namespace UrbanSolution.Data.MigrationHelpers
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public static class ViewsHelper
    {
        public static void CreateUsersWithRolesView(MigrationBuilder builder)
        {
            builder.Sql(@"CREATE VIEW [dbo].[UsersWithRoles] AS
                            SELECT u.[ID], u.[UserName], u.[Email], u.[LockoutEnd], 
                            	STRING_AGG(r.[Name],  ',') AS [UserRoles] 
                            FROM [dbo].AspNetUsers AS u
                            LEFT JOIN [dbo].AspNetUserRoles AS ur 
                            	ON u.Id = ur.UserId
                            JOIN [dbo].AspNetRoles AS r 
                            	ON ur.RoleId = r.Id 
                            GROUP BY u.[Id], u.[UserName], u.[Email], u.[LockoutEnd]");
        }

        public static void DropUsersWithRolesView(MigrationBuilder builder)
        {
            builder.Sql("DROP VIEW [dbo].[UsersWithRoles]");
        }
    }
}
