namespace UrbanSolution.Services.Manager
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    public interface IManagerIssueService
    {
        Task UpdateAsync(string managerId, int id, string title, string description, RegionType region, IssueType type, string street);

        Task DeleteAsync(string managerId, int issueId);

        Task ApproveAsync(string managerId, int issueId);

        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, RegionType? region);

        Task<TModel> GetAsync<TModel>(int issueId);

        Task<int> TotalAsync(bool isApproved);        

        Task<bool> ExistsAsync(int issueId);        

        Task<bool> IsIssueInSameRegionAsync(int issueId, RegionType? managerRegion);
        
    }
}
