namespace UrbanSolution.Services.Manager
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models.Enums;
    
    public interface IManagerActivityService
    {
        Task<IEnumerable<TModel>> GetAsync<TModel>(string managerId);

        Task<IEnumerable<TModel>> AllAsync<TModel>();

        Task<int> WriteLogAsync(string managerId, ManagerActivityType activity);
    }
}
