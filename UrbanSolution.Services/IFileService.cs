namespace UrbanSolution.Services
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public interface IFileService
    {
        Task<string> UploadFileToServerAsync(IFormFile pictureFile);

        void DeleteFileFromServer(string fileName);
    }
}
