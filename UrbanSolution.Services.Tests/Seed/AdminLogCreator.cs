namespace UrbanSolution.Services.Tests.Seed
{
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class AdminLogCreator
    {
        public static AdminLog Create(string adminId, string userId, string forRole)
        {
            return new AdminLog
            {
                Activity = AdminActivityType.AddedToRole,
                AdminId = adminId,
                CreatedOn = DateTime.UtcNow,
                EditedUserId = userId,
                ForRole = forRole
            };
        }
    }
}
