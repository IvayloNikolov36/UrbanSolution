using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Services.Implementations;
using UrbanSolution.Services.Tests.Mocks;
using Xunit;

namespace UrbanSolution.Services.Tests
{
    public class UserIssuesServiceTests
    {
        private int issueId;
        private int resolvedId;
        private const double DefaultLocation = 45.289;
        private const int DefaultImageId = 458962;

        private readonly UrbanSolutionDbContext db;

        public UserIssuesServiceTests()
        {
            this.db = InMemoryDatabase.Get();
            AutomapperInitializer.Initialize();
        }

        [Fact]
        public async Task UploadAsyncShould()
        {
            const string UserId = "rt8856dse45e12ds2";
            const string Title = "dssfrfsfsd";
            const string Description = "Description";
            const string Address = "Address";
            const string Latitude = "42.36";
            const string Longitude = "41.458";
            string issueType = UrbanSolution.Models.IssueType.DangerousTrees.ToString();
            string region = RegionType.Western.ToString();

            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var service = new UserIssuesService(this.db, pictureService.Object);

            var picFile = new Mock<IFormFile>();

            //Act
            var result = await service.UploadAsync(UserId, Title, Description, picFile.Object, issueType, region,
                Address, Latitude, Longitude);

            //Assert
            result.Should().BeOfType(typeof(int));

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            var savedIssue = this.db.Find<UrbanIssue>(result);

            savedIssue.Id.Should().Be(result);
            savedIssue.Title.Should().Match(Title);
            savedIssue.Description.Should().Match(Description);
            savedIssue.Latitude.Should().Be(double.Parse(Latitude.Trim(), CultureInfo.InvariantCulture));
            savedIssue.Longitude.Should().Be(double.Parse(Longitude.Trim(), CultureInfo.InvariantCulture));
            savedIssue.AddressStreet.Should().Match(Address);
            savedIssue.PublisherId.Should().Match(UserId);
            savedIssue.Type.Should().Be(Enum.Parse<IssueType>(issueType));
            savedIssue.Region.Should().Be(Enum.Parse<RegionType>(region));
            savedIssue.CloudinaryImageId.Should().Be(DefaultImageId);
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

        private ResolvedIssue CreateResolvedIssue(string publisherId)
        {
            return new ResolvedIssue
            {
                Id = ++resolvedId,
                CloudinaryImageId = DefaultImageId,
                Description = Guid.NewGuid().ToString(),
                PublisherId = publisherId,
                ResolvedOn = DateTime.UtcNow
            };
        }

        private User CreateUser()
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString()
            };
        }

        private UrbanIssue CreateIssue(bool isApproved, string publisherId, RegionType? region)
        {
            return new UrbanIssue
            {
                Id = ++issueId,
                AddressStreet = Guid.NewGuid().ToString(),
                CloudinaryImageId = DefaultImageId,
                Description = Guid.NewGuid().ToString(),
                IsApproved = isApproved,
                Latitude = DefaultLocation,
                Longitude = DefaultLocation,
                PublishedOn = DateTime.UtcNow,
                PublisherId = publisherId,
                Region = region ?? RegionType.All
            };

        }
    }
}
