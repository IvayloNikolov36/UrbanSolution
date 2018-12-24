namespace UrbanSolution.Services.Manager
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    public interface IManagerIssueService
    {
        Task<TModel> GetAsync<TModel>(int issueId);

        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, RegionType? region);

        Task<int> TotalAsync(bool isApproved);

        Task DeleteAsync(int issueId);

        Task<bool> ExistsAsync(int issueId);

        Task ApproveAsync(int issueId);

        Task<bool> IsIssueInSameRegionAsync(int issueId, RegionType? managerRegion);

        Task UpdateAsync(int id, string title, string description, RegionType region, IssueType type, string street);
    }
}
