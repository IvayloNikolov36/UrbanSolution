namespace UrbanSolution.Services.Tests
{
    using Data;
    using Implementations;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using Xunit;

    public class PictureInfoWriterServiceTests
    {
        private const int DefaultImageId = 458962;

        private readonly UrbanSolutionDbContext db;

        public PictureInfoWriterServiceTests()
        {
            this.db = InMemoryDatabase.Get();
        }

        [Fact]
        public async Task WriteToDbAsyncShould_WriteInDbCorrectInfoForPicture()
        {
            const string PictureUrl = "PicUrl";
            const string PictureThumbnailUrl = "picThumbnailUrl";
            const string PicturePublicId = "PublicId";
            long picLength = long.MaxValue;
            DateTime uploadedOn = DateTime.UtcNow;

            //Arrange
            var service = new PictureInfoWriterService(this.db);

            var user = this.CreateUser();
            await this.db.AddAsync(user);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.WriteToDbAsync(user.Id, PictureUrl, PictureThumbnailUrl, PicturePublicId, uploadedOn, picLength);

            //Assert
            result.Should().BeOfType(typeof(int));

            var entry = this.db.Find<CloudinaryImage>(result);

            entry.Id.Should().Be(result);
            entry.Length.Should().Be(picLength);
            entry.PicturePublicId.Should().Match(PicturePublicId);
            entry.PictureUrl.Should().Match(PictureUrl);
            entry.PictureThumbnailUrl.Should().Match(PictureThumbnailUrl);
            entry.UploadedOn.Should().Be(uploadedOn);
            entry.UploaderId.Should().Match(user.Id);
        }

        private User CreateUser()
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString()
            };
        }

    }
}
