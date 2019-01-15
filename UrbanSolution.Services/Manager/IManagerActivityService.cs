namespace UrbanSolution.Services.Manager
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models.Enums;
    
    public interface IManagerActivityService
    {
        Task<IEnumerable<ManagerActivitiesListingServiceModel>> GetAsync(string managerId);

        Task<IEnumerable<ManagerActivitiesListingServiceModel>> AllAsync();

        Task<int> WriteLogAsync(string managerId, ManagerActivityType activity);
    }
}
