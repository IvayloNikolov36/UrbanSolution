namespace UrbanSolution.Services.Tests
{
    using Data;
    using FluentAssertions;
    using Implementations;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Seed;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager.Models;
    using Utilities;
    using Xunit;

    public class IssueServiceTests : BaseServiceTest
    {
        [Theory]
        [InlineData(true, 1)]
        [InlineData(true, 2)]
        [InlineData(true, 3)]
        [InlineData(false, 1)]
        [InlineData(false, 2)]
        [InlineData(false, 3)]
        public async Task AllAsyncShould_ReturnsIssueWithCorrectModel(bool isApproved, int pageToGet)
        {
            //Arrange
            var service = new IssueService(this.Db);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddRangeAsync(image);

            var issue = UrbanIssueCreator.CreateIssue(true, RegionType.Central, user.Id, image.Id);
            var secondIssue = UrbanIssueCreator.CreateIssue(true, RegionType.Central, user.Id, image.Id); 
            var thirdIssue = UrbanIssueCreator.CreateIssue(true, RegionType.Central, user.Id, image.Id); 
            await this.Db.AddRangeAsync(issue, secondIssue, thirdIssue);

            var notApprovedIssue = UrbanIssueCreator.CreateIssue(false, RegionType.Central, user.Id, image.Id); 
            var secondNotApprovedIssue = UrbanIssueCreator.CreateIssue(false, RegionType.Central, user.Id, image.Id); 
            var thirdNotApprovedIssue = UrbanIssueCreator.CreateIssue(false, RegionType.Central, user.Id, image.Id);           
            await this.Db.AddRangeAsync(notApprovedIssue, secondNotApprovedIssue, thirdNotApprovedIssue);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.AllAsync(isApproved: isApproved, rowsCount: 1, page: pageToGet, RegionType.All.ToString(), IssueType.Other.ToString(), "ASC"); //TODO: remake this 

            var expected = await this.Db.UrbanIssues.Where(i => i.IsApproved == isApproved)
                .OrderByDescending(i => i.PublishedOn)
                .Skip( (pageToGet - 1) * ServiceConstants.IssuesPageSize)
                .Take(ServiceConstants.IssuesPageSize)
                .To<UrbanIssuesListingServiceModel>()
                .ToListAsync();

            //Assert
            result.Should().BeOfType<List<UrbanIssuesListingServiceModel>>();

            result.Should().HaveCount(expected.Count);

            result.Should().NotContain(i => i.IsApproved != isApproved);

            result.Should().BeInDescendingOrder(i => i.PublishedOn);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TotalAsyncShould_ReturnsCorrectCountOf_AllApprovedAnd_CountOfAllNotApprovedIssues(bool takeApproved)
        {
            //Arrange
            var service = new IssueService(this.Db);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddRangeAsync(image);

            var issue = UrbanIssueCreator.CreateIssue(true, RegionType.Central,  user.Id, image.Id);
            var secondIssue = UrbanIssueCreator.CreateIssue(true, RegionType.Eastern, user.Id, image.Id);
            await this.Db.AddRangeAsync(issue, secondIssue);

            var notApprovedIssue = UrbanIssueCreator.CreateIssue(false, RegionType.North, user.Id, image.Id);
            var secondNotApprovedIssue = UrbanIssueCreator.CreateIssue(true, RegionType.North, user.Id, image.Id);
            await this.Db.AddRangeAsync(notApprovedIssue, secondNotApprovedIssue);

            await this.Db.SaveChangesAsync();

            //Act
            var resultCount = await service.TotalAsync(isApproved: takeApproved);
            var expectedCount = await this.Db.UrbanIssues.Where(i => i.IsApproved == takeApproved).CountAsync();

            //Assert
            resultCount.Should().Be(expectedCount);
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectModel()
        {
            //Arrange
            var service = new IssueService(this.Db);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddRangeAsync(image);

            var issue = UrbanIssueCreator.CreateIssue(true, RegionType.Western, user.Id, image.Id);
            var secondIssue = UrbanIssueCreator.CreateIssue(true, RegionType.Central, user.Id, image.Id);
            await this.Db.AddRangeAsync(issue, secondIssue);

            await this.Db.SaveChangesAsync();

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
            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddRangeAsync(image);

            var issue = UrbanIssueCreator.CreateIssue(true, regionParam, user.Id, image.Id);
            var notApprovedIssue = UrbanIssueCreator.CreateIssue(false, regionParam, user.Id, image.Id);
            await this.Db.AddRangeAsync(issue, notApprovedIssue);

            await this.Db.SaveChangesAsync();

            var service = new IssueService(this.Db);

            //Act
            var resultModel = await service.AllMapInfoDetailsAsync(isApprovedParam, regionParam);

            var expected = this.Db.UrbanIssues
                .Where(i => i.IsApproved == isApprovedParam && i.Region == regionParam)
                .To<IssueMapInfoBoxDetailsServiceModel>();

            //Assert
            resultModel.Should().AllBeOfType<IssueMapInfoBoxDetailsServiceModel>();

            resultModel.Should().BeEquivalentTo(expected);
        }

    }
}
