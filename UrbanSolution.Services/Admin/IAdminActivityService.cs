using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Models.Enums;
using UrbanSolution.Services.Admin.Models;

namespace UrbanSolution.Services.Admin
{
    public interface IAdminActivityService
    {
        Task<IEnumerable<AdminActivitiesListingServiceModel>> AllAsync(string adminId);

        Task<int> WriteInfoAsync(string adminId, string userId, string role, AdminActivityType activity);
    }
}
