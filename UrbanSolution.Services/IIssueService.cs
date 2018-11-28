using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services
{
    public interface IIssueService
    {
        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved);

        Task<UrbanIssueDetailsServiceModel> DetailsAsync(int id);

        IEnumerable<IssueMapInfoBoxDetailsServiceModel> AllMapInfoDetails(bool areApproved);
    }
}
