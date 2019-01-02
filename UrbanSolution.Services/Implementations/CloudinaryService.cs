
namespace UrbanSolution.Services.Implementations
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Utilities;

    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration configuration;
        private Cloudinary cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.InitializeCloudinary();
        }

        public async Task<ImageUploadResult> UploadFormFileAsync(IFormFile file)
        {
            using (var memoryStream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), memoryStream),
                    PublicId = $"urban{Guid.NewGuid()}",
                    Transformation = new Transformation().Crop("limit").Width(800).Height(600),
                    EagerTransforms = new List<Transformation>()
                    {
                        new Transformation().Width(200).Height(200).Crop("thumb")
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
                .BuildUrl(string.Format(ServiceConstants.CloudinaryGetImageUrl, imagePublicId));
            
            return pictureUrl;
        }

        public string GetImageThumbnailUrl(string imagePublicId)
        {
            var pictureUrl = cloudinary.Api.UrlImgUp
                .Transform(new Transformation().Height(200).Width(200).Crop("thumb"))
                .BuildUrl(string.Format(ServiceConstants.CloudinaryGetImageUrl, imagePublicId));

            return pictureUrl;
        }

        private void InitializeCloudinary()
        {

            var key = configuration.GetSection("Cloudinary:CloudName").Value;
            this.cloudinary = new Cloudinary(
                new Account(
                    configuration.GetSection("Cloudinary:CloudName").Value,
                    configuration.GetSection("Cloudinary:ApiKey").Value,
                    configuration.GetSection("Cloudinary:ApiSecret").Value));
        }
    }
}
