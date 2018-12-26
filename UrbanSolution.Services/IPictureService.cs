
namespace UrbanSolution.Services
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface IPictureService
    {        
        Task<int> UploadImageAsync(string userId, IFormFile pictureFile);

        Task DeleteImageAsync(int pictureId);
    }
}
