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

        Task<IEnumerable<TModel>> AllAsync<TModel>(bool isApproved, RegionType? region);      

        Task RemoveResolvedReferenceAsync(int issueId);
 
    }
}
