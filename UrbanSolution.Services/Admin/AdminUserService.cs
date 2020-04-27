namespace UrbanSolution.Services.Admin
{
    using Data;
    using Mapping;
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

        public AdminUserService(UrbanSolutionDbContext db,
            UserManager<User> userManager,
            IAdminActivityService activity)
        {
            this.db = db;
            this.userManager = userManager;
            this.activity = activity;
        }

        public async Task<(int, IEnumerable<T>)> AllAsync<T>(int page, string sortBy, 
            string sortType, string searchType, string searchText, string filter)
        {
            string search = searchText;
            bool hasSearching = !string.IsNullOrEmpty(search);
            bool hasFiltering = filter != null && !string.IsNullOrEmpty(filter) && !filter.Equals(NoFilter);
            bool hasSorting = sortBy != null;

            IQueryable<UsersWithRoles> users = this.db.UsersWithRoles
                .AsNoTracking()
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

            var filteredUsersCount = await users.CountAsync();

            var usersForPage = users.Skip((page - 1) * UsersOnPage).Take(UsersOnPage);

            var usersModels = await usersForPage.To<T>().ToListAsync();

            return (filteredUsersCount, usersModels);  
        }

        private IQueryable<UsersWithRoles> AllFilteredBySearch(IQueryable<UsersWithRoles> users, string searchType, string searchText)
        {
            Expression<Func<UsersWithRoles, bool>> expression = null;

            if (searchType == UsersFilters.UserName.ToString())
                expression = u => u.UserName.ToUpper().Contains(searchText.ToUpper());
            else if (searchType == UsersFilters.Email.ToString())
                expression = u => u.Email.ToUpper().Contains(searchText.ToUpper());

            return users.Where(expression);
        }

        private IQueryable<UsersWithRoles> AllFilteredByLockedStatus(IQueryable<UsersWithRoles> users, string filter)
        {
            Expression<Func<UsersWithRoles, bool>> expression = null;

            if (filter == FilterUsersBy.Locked.ToString())
                expression = u => u.LockoutEnd != null;

            if (filter == FilterUsersBy.NotLocked.ToString().SeparateStringByCapitals())
                expression = u => u.LockoutEnd == null;

            return users.Where(expression);
        }

        private IQueryable<UsersWithRoles> AllSortedBy(IQueryable<UsersWithRoles> users, string sortBy, string sortType)
        {
            Expression<Func<UsersWithRoles, string>> expression = null;

            if (sortBy == UserNameProp)
                expression = u => u.UserName;

            if (sortBy == EmailProp)
                expression = u => u.Email;

            return sortType == SortAsc
                ? users.OrderBy(expression)
                : users.OrderByDescending(expression);
        }

        public async Task<bool> UnlockAsync(string adminId, User user)
        {
            if (user.LockoutEnd == null)
            {
                return false;
            }

            user.LockoutEnd = null;
            await this.db.SaveChangesAsync();

            await this.activity.WriteInfoAsync(adminId, user.Id, string.Empty, AdminActivityType.UnlockedUser);

            return true;
        }

        public async Task<bool> LockAsync(string adminId, User user, int lockDays)
        {
            if (user.LockoutEnd != null)
            {
                return false;
            }

            user.LockoutEnd = new DateTimeOffset(DateTime.UtcNow.AddDays(lockDays));
            await this.db.SaveChangesAsync();

            await this.activity.WriteInfoAsync(adminId, user.Id, string.Empty, AdminActivityType.LockedUser);

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
