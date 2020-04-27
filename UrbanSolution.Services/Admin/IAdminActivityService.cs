namespace UrbanSolution.Services.Admin
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models.Enums;

    public interface IAdminActivityService
    {
        Task<(int, IEnumerable<TModel>)> AllAsync<TModel>(string adminId, int page);

        Task<int> WriteInfoAsync(string adminId, string userId, string role, AdminActivityType activity);
    }
}
