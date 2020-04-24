namespace UrbanSolution.Services.Admin
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Services.Admin.Models;

    public interface IAdminUserService
    {
        Task<int> AllCountAsync();

        Task<(int, IEnumerable<AdminUserListingServiceModel>)> AllAsync(int page, string sortBy, string sortType, string searchType,
            string searchText, string filter);

        Task<bool> UnlockAsync(string userId);

        Task<bool> LockAsync(string userId, int lockDays);

        Task<bool> AddToRoleAsync(string adminId, string userId, string role);

        Task<bool> RemoveFromRoleAsync(string adminId, string userId, string role);
    }
}
