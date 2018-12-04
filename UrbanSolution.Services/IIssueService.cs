using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services
{
    public interface IIssueService
    {
        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved);

        Task<TModel> GetAsync<TModel>(int id);

        Task<IEnumerable<IssueMapInfoBoxDetailsServiceModel>> AllMapInfoDetailsAsync(bool areApproved);
    }
}
