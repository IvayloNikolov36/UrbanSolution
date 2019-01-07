namespace UrbanSolution.Services.Admin
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserListingServiceModel>> AllAsync();

        Task<bool> AddToRole(string adminId, string userId, string role);

        Task<bool> RemoveFromRole(string adminId, string userId, string role);
    }
}
