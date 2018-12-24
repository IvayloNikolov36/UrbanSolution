using System;
using System.Threading.Tasks;
using UrbanSolution.Data;
using UrbanSolution.Models;

namespace UrbanSolution.Services.Implementations
{
    public class PictureService : IPictureService
    {
        private readonly UrbanSolutionDbContext db;

        public PictureService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<int> WritePictureInfo(string uploaderId, string pictureUrl, string pictureThumbnailUrl, string picturePublicId, DateTime uploadedOn, long pictureLength)
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
