using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Moq;
using UrbanSolution.Data;
using UrbanSolution.Data.Migrations;
using UrbanSolution.Models;
using UrbanSolution.Services.Implementations;
using UrbanSolution.Services.Manager.Models;
using UrbanSolution.Services.Mapping;
using UrbanSolution.Services.Models;
using UrbanSolution.Services.Utilities;
using Xunit;

namespace UrbanSolution.Services.Tests
{
    public class IssueServiceTests
    {
        private int issueId;
        private int resolvedId;
        private const double DefaultLocation = 45.289;
        private const int DefaultImageId = 458962;

        private readonly UrbanSolutionDbContext db;

        public IssueServiceTests()
        {
            this.db = InMemoryDatabase.Get();
            AutomapperInitializer.Initialize();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task AllAsyncShould_ReturnsIssueWithCorrectModel(int pageToGet)
        {
            //Arrange
            var service = new IssueService(this.db);

            var user = this.CreateUser();
            await this.db.AddAsync(user);

            var image = this.CreateImage(user.Id);
            await this.db.AddRangeAsync(image);

            var issue = this.CreateIssue(true, user.Id, null);
            var secondIssue = this.CreateIssue(true, user.Id, null);
            var thirdIssue = this.CreateIssue(true, user.Id, null);
            var fourthIssue = this.CreateIssue(true, user.Id, null);
            var fifthIssue = this.CreateIssue(true, user.Id, null);
            var sixthIssue = this.CreateIssue(true, user.Id, null);
            var seventhIssue = this.CreateIssue(true, user.Id, null);
            var eightIssue = this.CreateIssue(true, user.Id, null);
            await this.db.AddRangeAsync(issue, secondIssue, thirdIssue, fourthIssue, fifthIssue, sixthIssue,
                seventhIssue, eightIssue);

            var notApprovedIssue = this.CreateIssue(false, user.Id, null);
            var secondNotApprovedIssue = this.CreateIssue(true, user.Id, null);
            var thirdNotApprovedIssue = this.CreateIssue(true, user.Id, null);
            var fourthNotApprovedIssue = this.CreateIssue(true, user.Id, null);
            var fifthNotApprovedIssue = this.CreateIssue(true, user.Id, null);
            var sixthNotApprovedIssue = this.CreateIssue(true, user.Id, null);
            var sevenNotApprovedIssue = this.CreateIssue(true, user.Id, null);
            var eightNotApprovedIssue = this.CreateIssue(true, user.Id, null);

            await this.db.AddRangeAsync(notApprovedIssue, secondNotApprovedIssue, thirdNotApprovedIssue, 
                fourthNotApprovedIssue,fifthNotApprovedIssue, sixthNotApprovedIssue, sevenNotApprovedIssue, eightNotApprovedIssue);

            await this.db.SaveChangesAsync();

            //Act
            var resultApproved = await service.AllAsync(isApproved: true, page: pageToGet); //page = 1
            var resultNotApproved = await service.AllAsync(isApproved: false, page: pageToGet); //page = 1

            var expectedApproved = await this.db.UrbanIssues.Where(i => i.IsApproved)
                .OrderByDescending(i => i.PublishedOn)
                .Skip( (pageToGet - 1) * ServiceConstants.IssuesPageSize)
                .Take(ServiceConstants.IssuesPageSize)
                .To<UrbanIssuesListingServiceModel>()
                .ToListAsync();

            var expectedNotApproved = await this.db.UrbanIssues.Where(i => !i.IsApproved)
                .OrderByDescending(i => i.PublishedOn)
                .Skip( (pageToGet - 1) * ServiceConstants.IssuesPageSize)
                .Take(ServiceConstants.IssuesPageSize)
                .To<UrbanIssuesListingServiceModel>()
                .ToListAsync();

            //Assert
            resultApproved.Should().AllBeOfType<UrbanIssuesListingServiceModel>();
            resultApproved.Should().HaveCountLessOrEqualTo(ServiceConstants.IssuesPageSize); //8
            resultApproved.Should().NotContain(i => !i.IsApproved);
            resultApproved.Should().BeInDescendingOrder(i => i.PublishedOn);
            resultApproved.Should().BeEquivalentTo(expectedApproved);

            resultNotApproved.Should().AllBeOfType<UrbanIssuesListingServiceModel>();
            resultNotApproved.Should().HaveCountLessOrEqualTo(ServiceConstants.IssuesPageSize); //8
            resultNotApproved.Should().NotContain(i => i.IsApproved);
            resultNotApproved.Should().BeInDescendingOrder(i => i.PublishedOn);
            resultNotApproved.Should().BeEquivalentTo(expectedNotApproved);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TotalAsyncShould_ReturnsCorrectCountOf_AllApprovedAnd_CountOfAllNotApprovedIssues(bool takeApproved)
        {
            //Arrange
            var service = new IssueService(this.db);

            var user = this.CreateUser();
            await this.db.AddAsync(user);

            var image = this.CreateImage(user.Id);
            await this.db.AddRangeAsync(image);

            var issue = this.CreateIssue(true, user.Id, null);
            var secondIssue = this.CreateIssue(true, user.Id, null);
            await this.db.AddRangeAsync(issue, secondIssue);

            var notApprovedIssue = this.CreateIssue(false, user.Id, null);
            var secondNotApprovedIssue = this.CreateIssue(true, user.Id, null);
            await this.db.AddRangeAsync(notApprovedIssue, secondNotApprovedIssue);

            await this.db.SaveChangesAsync();

            //Act
            var resultCount = await service.TotalAsync(isApproved: takeApproved);
            var expectedCount = await this.db.UrbanIssues.Where(i => i.IsApproved == takeApproved).CountAsync();

            //Assert
            resultCount.Should().Be(expectedCount);
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectModel()
        {
            //Arrange
            var service = new IssueService(this.db);

            var user = this.CreateUser();
            await this.db.AddAsync(user);

            var image = this.CreateImage(user.Id);
            await this.db.AddRangeAsync(image);

            var issue = this.CreateIssue(true, user.Id, null);
            var secondIssue = this.CreateIssue(true, user.Id, null);
            await this.db.AddRangeAsync(issue, secondIssue);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.GetAsync<UrbanIssueEditServiceViewModel>(issue.Id);

            var secondResult = await service.GetAsync<UrbanIssueDetailsServiceModel>(secondIssue.Id);

            //Assert
            result.Should().BeOfType<UrbanIssueEditServiceViewModel>();
            result.Id.Should().Be(issue.Id);
            result.Publisher.Should().Match(user.UserName);

            secondResult.Should().BeOfType<UrbanIssueDetailsServiceModel>();
            secondResult.Id.Should().Be(secondIssue.Id);
            secondResult.HasResolved.Should().Be(secondIssue.ResolvedIssue != null);
            secondResult.Latitude.Should().Be(secondIssue.Latitude.ToString().Replace(",", "."));
            secondResult.Longitude.Should().Be(secondIssue.Longitude.ToString().Replace(",", "."));
            secondResult.IssuePictureUrl.Should().Be(secondIssue.CloudinaryImage.PictureUrl);
        }

        [Theory]
        [InlineData(true, RegionType.All)]
        [InlineData(true, RegionType.Western)]
        [InlineData(false, RegionType.All)]
        [InlineData(false, RegionType.Western)]
        public async Task AllMapInfoDetailsAsync(bool isApprovedParam, RegionType regionParam)
        {
            //Arrange
            var service = new IssueService(this.db);

            var user = this.CreateUser();
            await this.db.AddAsync(user);

            var image = this.CreateImage(user.Id);
            await this.db.AddRangeAsync(image);

            var issue = this.CreateIssue(true, user.Id, regionParam);
            var notApprovedIssue = this.CreateIssue(false, user.Id, regionParam);
            await this.db.AddRangeAsync(issue, notApprovedIssue);

            await this.db.SaveChangesAsync();

            //Act
            var resultModel = await service.AllMapInfoDetailsAsync(isApprovedParam, regionParam);

            var expected = this.db.UrbanIssues
                .Where(i => i.IsApproved == isApprovedParam && i.Region == regionParam)
                .To<IssueMapInfoBoxDetailsServiceModel>();

            //Assert
            resultModel.Should().AllBeOfType<IssueMapInfoBoxDetailsServiceModel>();
            resultModel.Should().BeEquivalentTo(expected);
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
