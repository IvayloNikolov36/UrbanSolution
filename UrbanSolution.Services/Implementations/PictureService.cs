
namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public class PictureService : IPictureService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly ICloudinaryService cloudinary;

        public PictureService(UrbanSolutionDbContext db, ICloudinaryService cloudinary)
        {
            this.db = db;
            this.cloudinary = cloudinary;
        }

        public async Task<int> UploadImageAsync(string userId, IFormFile pictureFile)
        {
            var uploadResult = await this.cloudinary.UploadFormFileAsync(pictureFile);

            var cloudinaryPictureUrl = this.cloudinary.GetImageUrl(uploadResult.PublicId);

            var cloudinaryThumbnailPictureUrl = this.cloudinary.GetImageThumbnailUrl(uploadResult.PublicId);

            var pictureId = await this.WritePictureInfoAsync(userId, cloudinaryPictureUrl, cloudinaryThumbnailPictureUrl, uploadResult.PublicId, uploadResult.CreatedAt, uploadResult.Length);

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

        private async Task<int> WritePictureInfoAsync(string uploaderId, string pictureUrl, string pictureThumbnailUrl, string picturePublicId, DateTime uploadedOn, long pictureLength)
        {
            var cloudinaryImage = new CloudinaryImage
            {
                UploaderId = uploaderId,
                PictureUrl = pictureUrl,
                PictureThumbnailUrl = pictureThumbnailUrl,
                PicturePublicId = picturePublicId,
                UploadedOn = uploadedOn,
                Length = pictureLength
            };

            await this.db.CloudinaryImages.AddAsync(cloudinaryImage);

            await this.db.SaveChangesAsync();

            return cloudinaryImage.Id;
        }
    }
}
