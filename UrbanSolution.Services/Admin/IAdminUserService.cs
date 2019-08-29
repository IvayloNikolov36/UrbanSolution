namespace UrbanSolution.Services.Admin
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserListingServiceModel>> AllAsync(string sortBy, string sortType, string searchType,
            string searchText, string filter);

        Task<bool> UnlockAsync(string userId);

        Task<bool> LockAsync(string userId, int lockDays);

        Task<bool> AddToRoleAsync(string adminId, string userId, string role);

        Task<bool> RemoveFromRoleAsync(string adminId, string userId, string role);
    }
}
