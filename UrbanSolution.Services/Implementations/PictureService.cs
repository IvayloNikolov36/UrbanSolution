namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public class PictureService : IPictureService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly ICloudinaryService cloudinary;
        private readonly IPictureInfoWriterService pictureInfoWriter;

        public PictureService(
            UrbanSolutionDbContext db, ICloudinaryService cloudinary, IPictureInfoWriterService pictureInfoWriter)
        {
            this.db = db;
            this.cloudinary = cloudinary;
            this.pictureInfoWriter = pictureInfoWriter;
        }

        public async Task<int> UploadImageAsync(string userId, IFormFile pictureFile)
        {
            var uploadResult = await this.cloudinary.UploadFormFileAsync(pictureFile);

            var cloudinaryPictureUrl = this.cloudinary.GetImageUrl(uploadResult.PublicId);

            var cloudinaryThumbnailPictureUrl = this.cloudinary.GetImageThumbnailUrl(uploadResult.PublicId);

            var pictureId = await this.pictureInfoWriter.WriteToDbAsync(userId, cloudinaryPictureUrl, cloudinaryThumbnailPictureUrl, uploadResult.PublicId, uploadResult.CreatedAt, uploadResult.Length);

            return pictureId;
        }

        public async Task DeleteImageAsync(int pictureId)
        {
            var pictureFromDb = await this.db.FindAsync<CloudinaryImage>(pictureId);

            var picturePublicId = pictureFromDb.PicturePublicId;

            this.db.CloudinaryImages.Remove(pictureFromDb);

            await this.db.SaveChangesAsync();

            await this.cloudinary.DeleteImages(picturePublicId);
        }

    }
}
