namespace UrbanSolution.Services.Tests
{
    using Implementations;
    using FluentAssertions;
    using Seed;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using Xunit;

    public class PictureInfoWriterServiceTests : BaseServiceTest
    {
        [Fact]
        public async Task WriteToDbAsyncShould_WriteInDbCorrectInfoForPicture()
        {
            const string PictureUrl = "PicUrl";
            const string PictureThumbnailUrl = "picThumbnailUrl";
            const string PicturePublicId = "PublicId";
            long picLength = long.MaxValue;
            DateTime uploadedOn = DateTime.UtcNow;

            //Arrange
            var service = new PictureInfoWriterService(this.Db);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.WriteToDbAsync(user.Id, PictureUrl, PictureThumbnailUrl, PicturePublicId, uploadedOn, picLength);

            //Assert
            result.Should().BeOfType(typeof(int));

            var entry = this.Db.Find<CloudinaryImage>(result);

            entry.Id.Should().Be(result);
            entry.Length.Should().Be(picLength);
            entry.PicturePublicId.Should().Match(PicturePublicId);
            entry.PictureUrl.Should().Match(PictureUrl);
            entry.PictureThumbnailUrl.Should().Match(PictureThumbnailUrl);
            entry.UploadedOn.Should().Be(uploadedOn);
            entry.UploaderId.Should().Match(user.Id);
        }

    }
}
