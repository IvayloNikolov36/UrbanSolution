namespace UrbanSolution.Services.Implementations
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Threading.Tasks;
    using Utilities;

    public class FileService : IFileService
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public FileService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> UploadFileToServerAsync(IFormFile pictureFile)
        {
            var fileName = Path.Combine(string.Format(ServiceConstants.ImageUploadPath, hostingEnvironment.WebRootPath), pictureFile.FileName);

            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                await pictureFile.CopyToAsync(fileStream);
            }

            return fileName;
        }

        public void DeleteFileFromServer(string fileName)
        {
            System.IO.File.Delete(fileName);
        }
    }
}
