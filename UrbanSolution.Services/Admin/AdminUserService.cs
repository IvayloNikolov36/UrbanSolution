namespace UrbanSolution.Services.Admin
{
    using Data;
    using Mapping;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class AdminUserService : IAdminUserService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly UserManager<User> userManager;
        private readonly IAdminActivityService activity;

        public AdminUserService(UrbanSolutionDbContext db, UserManager<User> userManager, IAdminActivityService activity)
        {
            this.db = db;
            this.userManager = userManager;
            this.activity = activity;

        }

        public async Task<IEnumerable<AdminUserListingServiceModel>> AllAsync()
        {
            var usersRoles = new List<List<string>>();

            foreach (var user in this.db.Users.ToList())
            {
                var userAllRoles = await this.userManager.GetRolesAsync(user);
                usersRoles.Add(userAllRoles.ToList());
            }

            var usersModels = await this.db
                .Users
                .To<AdminUserListingServiceModel>()
                .ToListAsync();

            for (var i = 0; i < usersModels.Count; i++)
                usersModels[i].UserRoles = usersRoles[i];

            return usersModels;
        }


        public async Task<bool> AddToRoleAsync(string adminId, string userId, string role)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            bool userAlreadyInRole = await this.userManager.IsInRoleAsync(user, role);

            if (userAlreadyInRole)
            {
                return false;
            }

            await this.userManager.AddToRoleAsync(user, role);

            await this.activity.WriteInfoAsync(adminId, userId, role, AdminActivityType.AddedToRole);

            return true;
        }

        public async Task<bool> RemoveFromRoleAsync(string adminId, string userId, string role)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            bool userInRole = await this.userManager.IsInRoleAsync(user, role);

            if (!userInRole)
            {
                return false;
            }

            await this.userManager.RemoveFromRoleAsync(user, role);

            await this.activity.WriteInfoAsync(adminId, userId, role, AdminActivityType.RemovedFromRole);

            return true;
        }
    }
}
