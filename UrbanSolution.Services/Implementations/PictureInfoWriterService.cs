namespace UrbanSolution.Services.Implementations
{
    using Data;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public class PictureInfoWriterService : IPictureInfoWriterService
    {
        private readonly UrbanSolutionDbContext db;

        public PictureInfoWriterService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<int> WriteToDbAsync(string uploaderId, string pictureUrl, string pictureThumbnailUrl, 
            string picPublicId, DateTime uploadedOn, long picLength)
        {
            var image = new CloudinaryImage
            {
                UploaderId = uploaderId,
                PictureUrl = pictureUrl,
                PictureThumbnailUrl = pictureThumbnailUrl,
                PicturePublicId = picPublicId,
                UploadedOn = uploadedOn,
                Length = picLength
            };

            await this.db.CloudinaryImages.AddAsync(image);
            await this.db.SaveChangesAsync();

            return image.Id;
        }
    }
}
