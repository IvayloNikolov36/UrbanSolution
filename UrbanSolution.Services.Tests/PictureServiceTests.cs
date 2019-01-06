namespace UrbanSolution.Services.Tests
{
    using CloudinaryDotNet.Actions;
    using Data;
    using Implementations;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using Xunit;

    public class PictureServiceTests
    {

        private const int DefaultImageId = 458962;

        private readonly UrbanSolutionDbContext db;

        public PictureServiceTests()
        {
            this.db = InMemoryDatabase.Get();
            AutomapperInitializer.Initialize();
        }

        [Fact]
        public async Task UploadImageAsyncShould_UploadImageToCloudinaryAnd_WritheInDbInfoForPicture()
        {
            //Arrange
            var imageUploadResultMock = new Mock<ImageUploadResult>();

            var cloudinaryService = new Mock<ICloudinaryService>();

            cloudinaryService.Setup(cs => cs.UploadFormFileAsync(It.IsAny<IFormFile>()))
                .Returns(It.IsAny<Task<ImageUploadResult>>());
            
            var picInfoWriter = new Mock<IPictureInfoWriterService>();

            var service = new PictureService(this.db, cloudinaryService.Object, picInfoWriter.Object);

            var user = this.CreateUser();
            await this.db.AddAsync(user);

            await this.db.SaveChangesAsync();

            var picFile = new Mock<IFormFile>();

            //Act
            var result = await service.UploadImageAsync(user.Id, picFile.Object);

            //Assert
            result.Should().BeOfType(typeof(int));

            cloudinaryService.Verify(c => c.UploadFormFileAsync(It.IsAny<IFormFile>()), Times.Once);

            cloudinaryService.Verify(c => c.GetImageUrl(It.IsAny<string>()), Times.Once);

            cloudinaryService.Verify(c => c.GetImageThumbnailUrl(It.IsAny<string>()), Times.Once);

            picInfoWriter.Verify(p => p.WriteToDbAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<long>()), 
                Times.Once);
        }

        [Fact]
        public async Task DeleteImageAsyncShould_DeleteTheImageFromCloudAnd_ImageInfoFromDB()
        {
            //Arrange
            var cloudinaryService = new Mock<ICloudinaryService>();
            cloudinaryService.Setup(cs => cs.UploadFormFileAsync(It.IsAny<IFormFile>()))
                .Returns(It.IsAny<Task<ImageUploadResult>>());

            var picInfoWriter = new Mock<IPictureInfoWriterService>();

            var service = new PictureService(this.db, cloudinaryService.Object, picInfoWriter.Object);

            var user = this.CreateUser();
            await this.db.AddAsync(user);

            var image = this.CreateImage(user.Id);
            await this.db.AddAsync(image);

            await this.db.SaveChangesAsync();

            //Act
            await service.DeleteImageAsync(image.Id);

            //Assert
            cloudinaryService.Verify(c => c.DeleteImages(It.IsAny<string>()), Times.Once);

            this.db.CloudinaryImages
                .FirstOrDefault(i => i.Id == DefaultImageId).Should().BeNull();
        }

        private User CreateUser()
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString()
            };
        }

        private CloudinaryImage CreateImage(string userId)
        {
            return new CloudinaryImage
            {
                Id = DefaultImageId,
                Length = long.MaxValue,
                UploaderId = userId,
                PictureThumbnailUrl = Guid.NewGuid().ToString(),
                PicturePublicId = Guid.NewGuid().ToString(),
                PictureUrl = Guid.NewGuid().ToString()
            };
        }
    }
}
