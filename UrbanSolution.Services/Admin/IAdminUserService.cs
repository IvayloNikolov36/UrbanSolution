using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Services.Admin.Models;

namespace UrbanSolution.Services.Admin
{
    public interface IAdminUserService
    {
        Task<IEnumerable<AdminUserListingServiceModel>> AllAsync();
    }
}
