
namespace UrbanSolution.Services.Manager
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models.Enums;
    
    public interface IManagerActivityService
    {
        Task WriteManagerLogInfoAsync(string managerId, ManagerActivityType activity);

        Task<IEnumerable<ManagerActivitiesListingServiceModel>> AllAsync(string managerId);

        Task<IEnumerable<ManagerActivitiesListingServiceModel>> AllAsync();
    }
}
