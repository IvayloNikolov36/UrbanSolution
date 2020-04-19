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
    using static UrbanSolutionUtilities.WebConstants;

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
            //TODO: Remove userManeger!!!!!
            this.db = db;
            this.userManager = userManager;
            this.activity = activity;
        }

        public async Task<IEnumerable<AdminUserListingServiceModel>> AllAsync(
            string sortBy, string sortType, string searchType, string searchText, string filter)
        {
            string search = searchText;
            bool hasSearching = !string.IsNullOrEmpty(search);
            bool hasFiltering = filter != null && !string.IsNullOrEmpty(filter) && !filter.Equals(NoFilter);
            bool hasSorting = sortBy != null; 

            IQueryable<User> users = this.db.Users.AsNoTracking()
                .OrderBy(u => u.UserName);

            if (hasFiltering)
            {
                users = this.AllFilteredByLockedStatus(users, filter);
            }

            if (hasSearching)
            {
                users = this.AllFilteredBySearch(users, searchType, searchText);
            }

            if (hasSorting)
            {
                users = this.AllSortedBy(users, sortBy, sortType);
            }

            var usersRoles = new List<List<string>>();

            foreach (var user in users.ToList())
            {
                var userAllRoles = await this.userManager.GetRolesAsync(user);
                usersRoles.Add(userAllRoles.ToList());
            }

            var usersModels = await users.To<AdminUserListingServiceModel>()
                .ToListAsync();

            for (var i = 0; i < usersModels.Count; i++)
            {
                usersModels[i].UserRoles = usersRoles[i];
            }

            return usersModels;
        }

        private IQueryable<User> AllFilteredBySearch(IQueryable<User> users, string searchType, string searchText)
        {
            Expression<Func<User, bool>> expression = null;

            if (searchType == UsersFilters.UserName.ToString())
                expression = u => u.UserName.ToUpper().Contains(searchText.ToUpper());
            else if (searchType == UsersFilters.Email.ToString())
                expression = u => u.Email.ToUpper().Contains(searchText.ToUpper());

            return users.Where(expression);
        }

        private IQueryable<User> AllFilteredByLockedStatus(IQueryable<User> users, string filter)
        {
            Expression<Func<User, bool>> expression = null;

            if (filter == FilterUsersBy.Locked.ToString())
                expression = u => u.LockoutEnd != null;

            if (filter == FilterUsersBy.NotLocked.ToString().SeparateStringByCapitals())
                expression = u => u.LockoutEnd == null;

            return users.Where(expression);
        }

        private IQueryable<User> AllSortedBy(IQueryable<User> users, string sortBy, string sortType)
        {
            Expression<Func<User, string>> expression = null;

            if (sortBy == UserNameProp)
                expression = u => u.UserName;

            if (sortBy == EmailProp)
                expression = u => u.Email;

            return sortType == SortAsc
                ? users.OrderBy(expression)
                : users.OrderByDescending(expression);
        }

        public async Task<bool> UnlockAsync(string userId)
        {
            User userFromDb = await this.userManager.FindByIdAsync(userId);

            if (userFromDb?.LockoutEnd == null)
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

            if (userFromDb == null || userFromDb.LockoutEnd != null)
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

    }
}
