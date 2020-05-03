namespace UrbanSolution.Data.MigrationHelpers
{
	using Microsoft.EntityFrameworkCore.Migrations;

	public static class FunctionsHelpers
    {
		public static void CreateGetUserNotInRolesFunction(MigrationBuilder builder)
		{
			builder.Sql(
				@"CREATE FUNCTION dbo.GetUserNotInRoles (@userId NVARCHAR(450))
				RETURNS NVARCHAR(700)
				AS
				BEGIN	
					DECLARE @notInRoles NVARCHAR(700) = (SELECT STRING_AGG(r.[Name], ',') AS Roles
														 FROM dbo.AspNetRoles AS r
														 WHERE r.Id NOT IN 
															(SELECT RoleId FROM dbo.AspNetUserRoles 
														     WHERE UserId = @userId));
					RETURN @notInRoles;	
				END");
		}

		public static void DropGetUserNotInRolesFunction(MigrationBuilder builder)
		{
			builder.Sql("DROP FUNCTION [dbo].GetUserNotInRoles");
		}
	}
}
