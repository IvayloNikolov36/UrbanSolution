namespace UrbanSolution.Services
{
    using System.Threading.Tasks;

    public interface IUserIssuesService
    {
        Task UploadAsync(string userId, string name, 
            string description,string pictureUrl, string issueType, 
            string region, string address,
            double latitude, double longitude);
    }
}
