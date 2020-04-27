namespace UrbanSolution.Data.MigrationHelpers
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public static class ProcedureHelpers
    {
        public static void CreateAssignUserToRoleProcedure(MigrationBuilder builder)
        {
            builder
				.Sql(@"CREATE PROC [dbo].AssignUserToRole @userId NVARCHAR(450), @role NVARCHAR(256)
						AS
						BEGIN
							DECLARE @roleId NVARCHAR(450) = (SELECT Id FROM [dbo].[AspNetRoles] WHERE [Name] = @role);
							IF @roleId IS NULL
							BEGIN
								RAISERROR('Invalid role name!', 16, 1);
								RETURN
							END						
							DECLARE @usersCount INT = (SELECT COUNT(*) FROM [dbo].[AspNetUsers] WHERE [Id] = @userId);
							IF @usersCount <> 1
							BEGIN
								RAISERROR('Invalid user id', 16, 2);
								RETURN
							END						
							DECLARE @userRoleCount INT = (SELECT COUNT(*) FROM [dbo].[AspNetUserRoles] AS ur 
														  WHERE ur.UserId = @userId AND ur.RoleId = @roleId);
							IF @userRoleCount > 0
							BEGIN
								RAISERROR('User is already assigned to this role', 16, 3);
								RETURN
							END						
							INSERT INTO [dbo].[AspNetUserRoles] (UserId, RoleId)
								VALUES (@userId, @roleId)
						END");
        }

        public static void DropAssignUserToRoleProcedure(MigrationBuilder builder)
        {
            builder.Sql("DROP PROC [dbo].AssignUserToRole");
        }

		public static void CreateRemoveUserRoleProcedure(MigrationBuilder builder)
		{
			builder
				.Sql(@"CREATE PROC [dbo].RemoveUserRole @userId NVARCHAR(450), @role NVARCHAR(256)
						AS
						BEGIN
							DECLARE @roleId NVARCHAR(450) = (SELECT Id FROM [dbo].[AspNetRoles] WHERE [Name] = @role);
							IF @roleId IS NULL
							BEGIN
								RAISERROR('Invalid role name!', 16, 1);
								RETURN
							END
							DECLARE @usersCount INT = (SELECT COUNT(*) FROM [dbo].[AspNetUsers] WHERE [Id] = @userId);
							IF @usersCount <> 1
							BEGIN
								RAISERROR('Invalid user id', 16, 2);
								RETURN
							END
							DECLARE @userRoleCount INT = (SELECT COUNT(*) FROM [dbo].[AspNetUserRoles] AS ur 
														  WHERE ur.UserId = @userId AND ur.RoleId = @roleId);
							IF @userRoleCount = 0 --user is not in this role
							BEGIN
								RAISERROR('Can not remove user from role!', 16, 3);
								RETURN
							END
							DELETE [dbo].[AspNetUserRoles] WHERE UserId = @userId AND RoleId = @roleId; 
						END");
		}

		public static void DropRemoveUserRoleProcedure(MigrationBuilder builder)
		{
			builder.Sql("DROP PROC [dbo].RemoveUserRole");
		}
	}
}
