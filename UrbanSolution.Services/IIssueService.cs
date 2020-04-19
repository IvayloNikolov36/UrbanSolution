namespace UrbanSolution.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public interface IIssueService
    {
        Task<IEnumerable<TModel>> AllAsync<TModel>(bool isApproved, int rowsCount, int page, string regionFilter, string typeFilter, string sortType);

        Task<int> TotalAsync(bool isApproved);

        Task<TModel> GetAsync<TModel>(int id);

        Task<IEnumerable<TModel>> AllMapInfoDetailsAsync<TModel>(bool areApproved, RegionType? region);
    }
}
