using System;

namespace UrbanSolution.Services.Tests.Seed
{
    using UrbanSolution.Models;

    public class ImageInfoCreator
    {
        private static int picId;
        private const string DefaultPictureUrl = "https://www.cloudinary.com";

        public static CloudinaryImage Create()
        {
            return new CloudinaryImage
            {
                Id = ++picId,
                PictureThumbnailUrl = DefaultPictureUrl
            };
        }

        public static CloudinaryImage CreateWithFullData(string userId)
        {
            return new CloudinaryImage
            {
                Id = ++picId,
                Length = long.MaxValue,
                UploaderId = userId,
                PictureThumbnailUrl = Guid.NewGuid().ToString(),
                PicturePublicId = Guid.NewGuid().ToString(),
                PictureUrl = Guid.NewGuid().ToString()
            };
        }
    }
}
