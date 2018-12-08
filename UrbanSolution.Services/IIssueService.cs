namespace UrbanSolution.Services
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public interface IIssueService
    {
        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, int page = 1);

        Task<int> TotalAsync(bool isApproved);

        Task<TModel> GetAsync<TModel>(int id);

        Task<IEnumerable<IssueMapInfoBoxDetailsServiceModel>> AllMapInfoDetailsAsync(bool areApproved, RegionType? region);
    }
}
