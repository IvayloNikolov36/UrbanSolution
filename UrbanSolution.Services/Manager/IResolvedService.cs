
namespace UrbanSolution.Services.Manager
{
    using Microsoft.AspNetCore.Http;
    using Models;
    using System.Threading.Tasks;

    public interface IResolvedService
    {
        Task<int> UploadAsync(string publisherId, int issueId, IFormFile pictureFile, string description);

        Task<bool> DeleteAsync(string managerId, int resolvedId);

        Task<ResolvedIssueEditServiceModel> GetAsync(int id);

        Task<bool> UpdateAsync(string managerId, int id, string description, IFormFile pictureFile);
    }
}
