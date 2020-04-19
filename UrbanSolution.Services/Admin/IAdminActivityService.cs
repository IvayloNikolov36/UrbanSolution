using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Models.Enums;

namespace UrbanSolution.Services.Admin
{
    public interface IAdminActivityService
    {
        Task<IEnumerable<TModel>> AllAsync<TModel>(string adminId);

        Task<int> WriteInfoAsync(string adminId, string userId, string role, AdminActivityType activity);
    }
}
