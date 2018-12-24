namespace UrbanSolution.Services.Manager
{
    using System.Threading.Tasks;

    public interface IResolvedService
    {
        Task<int> UploadAsync(string publisherId, int issueId, int pictureId, string description);       
    }
}
