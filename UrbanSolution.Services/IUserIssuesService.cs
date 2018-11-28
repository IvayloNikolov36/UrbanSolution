using System.Threading.Tasks;

namespace UrbanSolution.Services
{
    public interface IUserIssuesService
    {
        Task UploadAsync(string userId, string name, string description, string pictureUrl, string issueType, string region, string addressStreet, string streetNumber, double latitude, double longitude);
    }
}
