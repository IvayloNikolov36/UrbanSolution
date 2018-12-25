namespace UrbanSolution.Services
{
    using System.Threading.Tasks;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;

    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadFormFileAsync(IFormFile file);

        string GetImageUrl(string imagePublicId);

        string GetImageThumbnailUrl(string imageThumbnailPublicId);
    }
}
