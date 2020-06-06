namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class PictureService : IPictureService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly ICloudImageService cloudService;
        private readonly IPictureInfoWriterService pictureInfoWriter;

        public PictureService(
            UrbanSolutionDbContext db, ICloudImageService cloudinary, IPictureInfoWriterService pictureInfoWriter)
        {
            this.db = db;
            this.cloudService = cloudinary;
            this.pictureInfoWriter = pictureInfoWriter;
        }

        public async Task<int> UploadImageAsync(string userId, IFormFile pictureFile)
        {
            var uploadResult = await this.cloudService.UploadImageAsync(pictureFile);

            var cloudinaryPictureUrl = this.cloudService.GetImageUrl(uploadResult.PublicId)
                .Replace(PicUrlPrefix, string.Empty);

            var cloudinaryThumbnailPictureUrl = this.cloudService.GetImageThumbnailUrl(uploadResult.PublicId)
                .Replace(PicUrlPrefix, string.Empty);

            var pictureId = await this.pictureInfoWriter
                .WriteToDbAsync(
                userId, cloudinaryPictureUrl, cloudinaryThumbnailPictureUrl, uploadResult.PublicId, uploadResult.CreatedAt, uploadResult.Length);

            return pictureId;
        }

        public async Task DeleteImageAsync(int pictureId)
        {
            var pictureFromDb = await this.db.FindAsync<CloudinaryImage>(pictureId);

            var picturePublicId = pictureFromDb.PicturePublicId;

            this.db.CloudinaryImages.Remove(pictureFromDb);
            await this.db.SaveChangesAsync();

            await this.cloudService.DeleteImagesAsync(picturePublicId);
        }

    }
}
