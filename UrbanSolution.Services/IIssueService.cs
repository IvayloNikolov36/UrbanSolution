using System.Collections.Generic;
using System.Threading.Tasks;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services
{
    public interface IIssueService
    {
        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, int page = 1);

        Task<int> TotalAsync(bool isApproved);

        Task<TModel> GetAsync<TModel>(int id); //TODO: the same is in another service

        Task<IEnumerable<IssueMapInfoBoxDetailsServiceModel>> AllMapInfoDetailsAsync(bool areApproved);
    }
}
