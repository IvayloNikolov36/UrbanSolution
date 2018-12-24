namespace UrbanSolution.Services.Implementations
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
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

        public async Task<ImageUploadResult> UploadImageAsync(string filePath)
        { 
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(filePath),
                PublicId = $"urban{Guid.NewGuid()}",
                Transformation = new Transformation().Crop("limit").Width(800).Height(600),
                EagerTransforms = new List<Transformation>()
                {
                    new Transformation().Width(200).Height(200).Crop("thumb"),
                    //new Transformation().Width(100).Height(150).Crop("fit").FetchFormat("png"),
                },
                //Tags = "special, for_homepage"
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            return uploadResult;
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
            this.cloudinary = new Cloudinary(
                new Account(
                    configuration.GetSection("Cloudinary:CloudName").Value,
                    configuration.GetSection("Cloudinary:ApiKey").Value,
                    configuration.GetSection("Cloudinary:ApiSecret").Value));
        }
    }
}
