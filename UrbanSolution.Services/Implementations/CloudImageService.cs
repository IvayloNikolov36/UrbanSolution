namespace UrbanSolution.Services.Implementations
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static UrbanSolutionUtilities.WebConstants;

    public class CloudImageService : ICloudImageService
    {
        private readonly Cloudinary cloudinary;

        public CloudImageService(Cloudinary cloudinary)
        {
            this.cloudinary = cloudinary;
        }

        public async Task<ImageUploadResult> UploadFormFileAsync(IFormFile file)
        {
            using (var memoryStream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), memoryStream),
                    PublicId = $"{PublicPicIdPrefix}{Guid.NewGuid()}",
                    Transformation = new Transformation().Crop(CropLimit).Width(800).Height(600),
                    EagerTransforms = new List<Transformation>
                    {
                        new Transformation().Width(ThumbnailWidth).Height(ThumbnailHeight).Crop(CropThumb)
                    }
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                return uploadResult;
            }           
        }

        public async Task DeleteImages(params string[] publicIds)
        {
            var delParams = new DelResParams
            {
                PublicIds = publicIds.ToList(),
                Invalidate = true
            };

            await cloudinary.DeleteResourcesAsync(delParams);
        }

        public string GetImageUrl(string imagePublicId)
        {
            var pictureUrl = this.cloudinary.Api.UrlImgUp
                .BuildUrl(string.Format(CloudinaryGetImageUrl, imagePublicId));
            
            return pictureUrl;
        }

        public string GetImageThumbnailUrl(string imagePublicId)
        {
            var pictureUrl = cloudinary.Api.UrlImgUp
                .Transform(new Transformation().Height(200).Width(200).Crop("thumb"))
                .BuildUrl(string.Format(CloudinaryGetImageUrl, imagePublicId));

            return pictureUrl;
        }

    }
}
