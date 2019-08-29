namespace UrbanSolution.Services.Admin
{
    using Data;
    using Mapping;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using UrbanSolutionUtilities.Enums;
    using UrbanSolutionUtilities.Extensions;
    using static UrbanSolution.Services.Utilities.ServiceConstants;

    public class AdminUserService : IAdminUserService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly UserManager<User> userManager;
        private readonly IAdminActivityService activity;

        public AdminUserService(
            UrbanSolutionDbContext db, 
            UserManager<User> userManager, 
            IAdminActivityService activity)
        {
            this.db = db;
            this.userManager = userManager;
            this.activity = activity;

        }
        public async Task<IEnumerable<AdminUserListingServiceModel>> AllAsync(string sortBy, string sortType, string searchType, string searchText, string filter)
        {
            string search = searchText;

            bool hasSearching = !string.IsNullOrEmpty(search);
            bool hasFiltering = filter != null && !string.IsNullOrEmpty(filter) && !filter.Equals(NoFilter);
            bool hasSorting = sortBy != null && sortBy != SortBy;

            if (hasSorting)
            {
                return await this.AllAsync(true, sortBy, sortType);
            }

            if (!hasSearching && !hasFiltering)
            {
                return await this.AllAsync();
            }

            Expression<Func<User, bool>> expression = null;

            if (hasSearching)
            {
                if (searchType == UsersFilters.UserName.ToString())
                    expression = u => u.UserName.Contains(search, StringComparison.InvariantCultureIgnoreCase);
                else if (searchType == UsersFilters.Email.ToString())
                    expression = u => u.Email.Contains(search, StringComparison.InvariantCultureIgnoreCase);
            }

            if (hasFiltering)
            {
                if (filter == FilterUsersBy.Locked.ToString())
                    expression = u => u.LockoutEnd != null;

                if (filter == FilterUsersBy.NotLocked.ToString().SeparateStringByCapitals())
                    expression = u => u.LockoutEnd == null;
  
            }

            return await this.AllAsyncWhere(expression);
        }

        public async Task<bool> UnlockAsync(string userId)
        {
            User userFromDb = await this.userManager.FindByIdAsync(userId);

            if (userFromDb == null)
            {
                return false;
            }

            userFromDb.LockoutEnd = null;

            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LockAsync(string userId, int lockDays)
        {
            User userFromDb = await this.userManager.FindByIdAsync(userId);

            if (userFromDb == null)
            {
                return false;
            }

            userFromDb.LockoutEnd = new DateTimeOffset(DateTime.UtcNow.AddDays(lockDays));

            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddToRoleAsync(string adminId, string userId, string role)
        {
            User user = await this.userManager.FindByIdAsync(userId);

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
            User user = await this.userManager.FindByIdAsync(userId);

            bool userInRole = await this.userManager.IsInRoleAsync(user, role);

            if (!userInRole)
            {
                return false;
            }

            await this.userManager.RemoveFromRoleAsync(user, role);

            await this.activity.WriteInfoAsync(adminId, userId, role, AdminActivityType.RemovedFromRole);

            return true;
        }

        private async Task<IEnumerable<AdminUserListingServiceModel>> AllAsync(bool hasSorting = false, string orderBy = null, string orderType = null)
        {
            IOrderedQueryable<User> query = this.db.Users.OrderBy(u => u.UserName);

            if (hasSorting)
            {
                if (orderBy == "UserName")
                {
                    query = orderType == "ASC"
                        ? this.db.Users.OrderBy(u => u.UserName)
                        : this.db.Users.OrderByDescending(u => u.UserName);
                }

                if (orderBy == "Email")
                {
                    query = orderType == "ASC"
                        ? this.db.Users.OrderBy(u => u.Email)
                        : this.db.Users.OrderByDescending(u => u.Email);
                }
            }

            var usersRoles = new List<List<string>>();

            foreach (var user in query.ToList())
            {
                var userAllRoles = await this.userManager.GetRolesAsync(user);
                usersRoles.Add(userAllRoles.ToList());
            }

            var usersModels = await query
                .To<AdminUserListingServiceModel>()
                .ToListAsync();

            for (var i = 0; i < usersModels.Count; i++)
                usersModels[i].UserRoles = usersRoles[i];

            return usersModels;
        }

        private async Task<IEnumerable<AdminUserListingServiceModel>> AllAsyncWhere(
            Expression<Func<User, bool>> expression)
        {
            var usersRoles = new List<List<string>>();

            IQueryable<User> filteredUsers = this.db.Users.Where(expression);
            foreach (var user in filteredUsers.ToList())
            {
                var userAllRoles = await this.userManager.GetRolesAsync(user);
                usersRoles.Add(userAllRoles.ToList());
            }

            var usersModels = await filteredUsers
                .To<AdminUserListingServiceModel>()
                .ToListAsync();

            for (var i = 0; i < usersModels.Count; i++)
                usersModels[i].UserRoles = usersRoles[i];

            return usersModels;
        }
    }
}
