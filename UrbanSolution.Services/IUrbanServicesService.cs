using System.Collections.Generic;
using System.Threading.Tasks;

namespace UrbanSolution.Services
{
    public interface IUrbanServicesService
    {
        Task<IEnumerable<TModel>> AllAsync<TModel>();

        Task<TModel> GetAsync<TModel>(int serviceId);

        Task CreateAsync(string name, string description, decimal price);
    }
}
