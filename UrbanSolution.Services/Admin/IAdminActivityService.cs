using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Models.Enums;
using UrbanSolution.Services.Admin.Models;

namespace UrbanSolution.Services.Admin
{
    public interface IAdminActivityService
    {
        Task WriteAdminLogInfoAsync(string adminId, string userId, string role, AdminActivityType activity);

        Task<IEnumerable<AdminActivitiesListingServiceModel>> AllAsync(string adminId);
    }
}
