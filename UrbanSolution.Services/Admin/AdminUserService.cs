namespace UrbanSolution.Services.Admin
{
    using UrbanSolution.Data;
    using UrbanSolution.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
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
    using Microsoft.Data.SqlClient;
    using System.Data;

    public class AdminUserService : IAdminUserService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IAdminActivityService activity;

        public AdminUserService(UrbanSolutionDbContext db,
            IAdminActivityService activity)
        {
            this.db = db;
            this.activity = activity;
        }


        public async Task<T> SingleAsync<T>(string userId)
        {
            return await this.db.UsersWithRoles.AsNoTracking()
                .Where(u => u.Id == userId)
                .To<T>()
                .FirstOrDefaultAsync();
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
            //await this.db.SaveChangesAsync(); it will be invoked in WriteInfoAsync

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
            //await this.db.SaveChangesAsync();

            await this.activity.WriteInfoAsync(adminId, user.Id, string.Empty, AdminActivityType.LockedUser);

            return true;
        }

        public async Task<bool> AddToRoleAsync(string adminId, string userId, string role)
        {
            bool isDone = await ExecuteProcedureAsync("[dbo].AssignUserToRole", userId, role);

            if (isDone)
            {
                await this.activity.WriteInfoAsync(adminId, userId, role, AdminActivityType.AddedToRole);
            }

            return isDone;
        }

        public async Task<bool> RemoveFromRoleAsync(string adminId, string userId, string role)
        {
            bool isDone = await ExecuteProcedureAsync("[dbo].RemoveUserRole", userId, role);

            if (isDone)
            {
                await this.activity.WriteInfoAsync(adminId, userId, role, AdminActivityType.RemovedFromRole);
            }

            return isDone;
        }

        private async Task<bool> ExecuteProcedureAsync(string procedureName, string userId, string role)
        {
            var userIdParam = new SqlParameter("@userId", SqlDbType.NVarChar)
            {
                Direction = ParameterDirection.Input,
                Value = userId
            };

            var roleParam = new SqlParameter("@role", SqlDbType.NVarChar)
            {
                Direction = ParameterDirection.Input,
                Value = role
            };

            try
            {
                await this.db.Database.ExecuteSqlInterpolatedAsync(
                     $"EXEC {procedureName} {userIdParam}, {roleParam}");
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

    }
}
