using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Models;
using UrbanSolution.Services.Manager.Models;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services.Manager
{
    public interface IIssueService
    {
        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved);

        Task<UrbanIssueEditServiceViewModel> GetAsync(int issueId);

        Task Update(int id, string name, string issuePictureUrl, string description, RegionType region, IssueType type, string addressStreet, string streetNumber);

        Task Delete(int issueId);

        Task<bool> ExistsAsync(int issueId);

        Task ApproveAsync(int issueId);
    }
}
