namespace UrbanSolution.Services.Tests
{
    using CloudinaryDotNet.Actions;
    using Implementations;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Seed;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class PictureServiceTests : BaseServiceTest
    {
        private const int DefaultImageId = 458962;

        [Fact]
        public async Task UploadImageAsyncShould_UploadImageToCloudAnd_WritheInDbInfoForPicture()
        {
            //Arrange
            var cloudService = new Mock<ICloudImageService>();

            cloudService.Setup(cs => cs.UploadFormFileAsync(It.IsAny<IFormFile>()))
                .Returns(It.IsAny<Task<ImageUploadResult>>());
            
            var picInfoWriter = new Mock<IPictureInfoWriterService>();

            var service = new PictureService(this.Db, cloudService.Object, picInfoWriter.Object);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            await this.Db.SaveChangesAsync();

            var picFile = new Mock<IFormFile>();

            //Act
            var result = await service.UploadImageAsync(user.Id, picFile.Object);

            //Assert
            result.Should().BeOfType(typeof(int));

            cloudService.Verify(c => c.UploadFormFileAsync(It.IsAny<IFormFile>()), Times.Once);

            cloudService.Verify(c => c.GetImageUrl(It.IsAny<string>()), Times.Once);

            cloudService.Verify(c => c.GetImageThumbnailUrl(It.IsAny<string>()), Times.Once);

            picInfoWriter.Verify(p => p.WriteToDbAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<long>()), 
                Times.Once);
        }

        [Fact]
        public async Task DeleteImageAsyncShould_DeleteTheImageFromCloudAnd_ImageInfoFromDB()
        {
            //Arrange
            var cloudService = new Mock<ICloudImageService>();
            cloudService.Setup(cs => cs.UploadFormFileAsync(It.IsAny<IFormFile>()))
                .Returns(It.IsAny<Task<ImageUploadResult>>());

            var picInfoWriter = new Mock<IPictureInfoWriterService>();

            var service = new PictureService(this.Db, cloudService.Object, picInfoWriter.Object);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddAsync(image);

            await this.Db.SaveChangesAsync();

            //Act
            await service.DeleteImageAsync(image.Id);

            //Assert
            cloudService.Verify(c => c.DeleteImages(It.IsAny<string>()), Times.Once);

            this.Db.CloudinaryImages
                .FirstOrDefault(i => i.Id == DefaultImageId).Should().BeNull();
        }
      
    }
}
