namespace UrbanSolution.Services
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface IUserIssuesService
    {
        Task<int> UploadAsync(string userId, string title, string description, IFormFile pictureFile,
            string issueType, string region, string address, string latitude, string longitude);

    }
}
