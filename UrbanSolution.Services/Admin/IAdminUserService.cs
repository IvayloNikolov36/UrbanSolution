namespace UrbanSolution.Services.Admin
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public interface IAdminUserService
    {
        Task<T> SingleAsync<T>(string userId);

        Task<(int, IEnumerable<T>)> AllAsync<T>(int page, string sortBy, string sortType, string searchType,
            string searchText, string filter);

        Task<bool> UnlockAsync(string adminId, User user);

        Task<bool> LockAsync(string adminId, User user, int lockDays);

        Task<bool> AddToRoleAsync(string adminId, string userId, string role);

        Task<bool> RemoveFromRoleAsync(string adminId, string userId, string role);
    }
}
