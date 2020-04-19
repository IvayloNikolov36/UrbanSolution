namespace UrbanSolution.Services
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface IUserIssuesService
    {
        Task<int> UploadAsync(string userId, string title, string description, IFormFile pictureFile,
            string issueType, string region, string address, string latitude, string longitude);

        //

        Task<int> UploadIssueImageAsync(string userId, IFormFile pictureFile);

        Task<int> UploadIssueAsync(string userId, string title, string description,
            int pictureId, string issueType, string region, string address, string latitude, string longitude);
    }
}
