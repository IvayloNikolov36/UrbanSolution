namespace UrbanSolution.Services
{
    using System.Threading.Tasks;
    using CloudinaryDotNet.Actions;

    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(string filePath);

        string GetImageUrl(string imagePublicId);

        string GetImageThumbnailUrl(string imageThumbnailPublicId);
    }
}
