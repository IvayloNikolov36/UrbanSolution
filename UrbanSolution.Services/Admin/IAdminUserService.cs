using System;
using System.Linq.Expressions;
using UrbanSolution.Models;

namespace UrbanSolution.Services.Admin
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserListingServiceModel>> AllAsync(bool hasSorting = false, string orderBy = null, string orderType = null);

        Task<IEnumerable<AdminUserListingServiceModel>> AllAsyncWhere(Expression<Func<User, bool>> expression);

        Task<bool> UnlockAsync(string userId);

        Task<bool> LockAsync(string userId, int lockDays);

        Task<bool> AddToRoleAsync(string adminId, string userId, string role);

        Task<bool> RemoveFromRoleAsync(string adminId, string userId, string role);
    }
}
