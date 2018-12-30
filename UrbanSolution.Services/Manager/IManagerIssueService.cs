namespace UrbanSolution.Services.Manager
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    public interface IManagerIssueService
    {
        Task<bool> UpdateAsync(User manager, int id, string title, string description, 
            RegionType region, IssueType type, string street, IFormFile pictureFile);

        Task<bool> DeleteAsync(User manager, int issueId);

        Task<bool> ApproveAsync(User manager, int issueId);

        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, RegionType? region);

        Task<TModel> GetAsync<TModel>(int issueId);

        Task<int> TotalAsync(bool isApproved);        

        Task<bool> ExistsAsync(int issueId);        

        Task RemoveResolvedReferenceAsync(int issueId);
    }
}
