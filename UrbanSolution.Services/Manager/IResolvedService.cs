using System.Threading.Tasks;

namespace UrbanSolution.Services.Manager
{
    public interface IResolvedService
    {
        Task<int> UploadAsync(string publisherId, int issueId, string pictureUrl, string description);

        
    }
}
