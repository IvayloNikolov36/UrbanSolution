namespace UrbanSolution.Services
{
    using System.Threading.Tasks;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;

    public interface ICloudImageService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile imageFile);

        Task DeleteImagesAsync(params string[] publicIds);

        string GetImageUrl(string imagePublicId);

        string GetImageThumbnailUrl(string imageThumbnailPublicId);
    }
}
