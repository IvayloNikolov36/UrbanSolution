namespace UrbanSolution.Services.Tests
{
    using FluentAssertions;
    using Implementations;
    using Microsoft.AspNetCore.Http;
    using Mocks;
    using Moq;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using Xunit;

    public class UserIssuesServiceTests : BaseServiceTest
    {
        private const int DefaultImageId = 458962;

        [Fact]
        public async Task UploadAsyncShould()
        {
            const string UserId = "rt8856dse45e12ds2";
            const string Title = "Title";
            const string Description = "Description";
            const string Address = "Address";
            const string Latitude = "42.36";
            const string Longitude = "41.458";
            string issueType = IssueType.DangerousTrees.ToString();
            string region = RegionType.Western.ToString();

            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var service = new UserIssuesService(this.Db, pictureService.Object);

            var picFile = new Mock<IFormFile>();

            //Act
            var result = await service.UploadAsync(UserId, Title, Description, picFile.Object, issueType, region,
                Address, Latitude, Longitude);

            //Assert
            result.Should().BeOfType(typeof(int));

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            var savedIssue = this.Db.Find<UrbanIssue>(result);

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

    }
}
