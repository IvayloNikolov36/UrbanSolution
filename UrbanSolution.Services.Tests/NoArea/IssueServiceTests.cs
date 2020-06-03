namespace UrbanSolution.Services.Tests
{
    using FluentAssertions;
    using Implementations;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Seed;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Web.Models.Issues;
    using UrbanSolution.Web.Models.Areas.Manager;
    using Xunit;
    using static Seed.UrbanIssueCreator;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssueServiceTests : BaseServiceTest
    {
        [Theory]
        [InlineData(true, 1, 1, RegionType.All, IssueType.DangerousTrees, null)]
        [InlineData(true, 1, 1, RegionType.All, IssueType.DangerousTrees, SortAsc)]
        [InlineData(true, 1, 1, RegionType.Central, IssueType.DangerousBuildings, SortDesc)]
        [InlineData(false, 1, 1, RegionType.All, IssueType.DangerousBuildings, SortDesc)]
        [InlineData(false, 1, 1, RegionType.Central, IssueType.Sidewalks, SortAsc)]
        [InlineData(true, 2, 1, RegionType.All, IssueType.DangerousTrees, SortAsc)]
        [InlineData(true, 2, 1, RegionType.Central, IssueType.DangerousBuildings, SortDesc)]
        [InlineData(false, 2, 1, RegionType.All, IssueType.DangerousBuildings, SortDesc)]
        [InlineData(false, 2, 1, RegionType.Central, IssueType.Sidewalks, SortAsc)]
        [InlineData(true, 1, 2, RegionType.All, IssueType.DangerousTrees, SortAsc)]
        [InlineData(true, 1, 2, RegionType.Central, IssueType.DangerousBuildings, SortDesc)]
        [InlineData(false, 1, 2, RegionType.All, IssueType.DangerousBuildings, SortDesc)]
        [InlineData(false, 1, 2, RegionType.Central, IssueType.Sidewalks, SortAsc)]
        public async Task AllAsyncShould_ReturnsIssueWithCorrectModel(bool isApproved, int rowsCount, int page, RegionType region, IssueType issueType, string sortType) 
        {
            //Arrange
            var issue = CreateIssue(true, RegionType.Central, IssueType.DangerousTrees);
            var secondIssue = CreateIssue(true, RegionType.Central, IssueType.Sidewalks); 
            var thirdIssue = CreateIssue(true, RegionType.Western, IssueType.DangerousBuildings);
            await this.Db.AddRangeAsync(issue, secondIssue, thirdIssue);

            var notApprovedIssue = CreateIssue(false, RegionType.Central, IssueType.DangerousBuildings); 
            var secondNotApprovedIssue = CreateIssue(false, RegionType.Central, IssueType.DangerousBuildings); 
            var thirdNotApprovedIssue = CreateIssue(false, RegionType.South, IssueType.DangerousBuildings);           
            await this.Db.AddRangeAsync(notApprovedIssue, secondNotApprovedIssue, thirdNotApprovedIssue);

            await this.Db.SaveChangesAsync();

            //TODO: rearrange test - now issueService has more parameters
            var service = new IssueService(this.Db, null);
            //Act
            (var pagesCount, var issues) = await service
                .AllAsync<IssuesListingModel>(isApproved, rowsCount, page, region.ToString(), issueType.ToString(), sortType);
            var result = issues.ToList();

            var expectedCount = await this.Db.UrbanIssues.Where(i => i.IsApproved == isApproved && i.Region == region && i.Type == issueType)
                .Skip((page - 1) * IssuesOnRow * rowsCount)
                .Take(IssuesOnRow).CountAsync();

            var dbIssues = this.Db.UrbanIssues.ToList();

            //Assert
            result.Should().BeOfType<List<IssuesListingModel>>();

            result.Should().HaveCount(expectedCount);

            result.Should().NotContain(i => i.IsApproved != isApproved);

            if (region != RegionType.All)
            {
                var issuesToNotContain = dbIssues.Where(x => x.Region != region);
                result.Should().NotContain(issuesToNotContain);
            }

            var issuesWithAnotherTypes = dbIssues.Where(x => x.Type != issueType).ToList();
            result.Should().NotContain(issuesWithAnotherTypes);

            if (sortType == null || sortType == SortDesc)
            {
                result.Should().BeInDescendingOrder(i => i.PublishedOn);
            }
            else
            {
                result.Should().BeInAscendingOrder(i => i.PublishedOn);
            }

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task TotalAsyncShould_ReturnsCorrectCountOf_AllApprovedAnd_CountOfAllNotApprovedIssues(bool takeApproved)
        {
            //Arrange
            //TODO: rearrange test - now issueService has more parameters
            var service = new IssueService(this.Db, null);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddRangeAsync(image);

            var issue = CreateIssue(true, RegionType.Central,  user.Id, image.Id);
            var secondIssue = CreateIssue(true, RegionType.Eastern, user.Id, image.Id);
            await this.Db.AddRangeAsync(issue, secondIssue);

            var notApprovedIssue = CreateIssue(false, RegionType.North, user.Id, image.Id);
            var secondNotApprovedIssue = CreateIssue(true, RegionType.North, user.Id, image.Id);
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
            //TODO: rearrange test - now issueService has more parameters
            var service = new IssueService(this.Db, null);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.CreateWithFullData(user.Id);
            await this.Db.AddRangeAsync(image);

            var issue = CreateIssue(true, RegionType.Western, user.Id, image.Id);
            var secondIssue = CreateIssue(true, RegionType.Central, user.Id, image.Id);
            await this.Db.AddRangeAsync(issue, secondIssue);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.GetAsync<IssueEditViewModel>(issue.Id);

            var secondResult = await service.GetAsync<IssueDetailsModel>(secondIssue.Id);

            //Assert
            result.Should().BeOfType<IssueEditViewModel>();
            result.Id.Should().Be(issue.Id);
            result.Publisher.Should().Match(user.UserName);

            secondResult.Should().BeOfType<IssueDetailsModel>();
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

            var issue = CreateIssue(true, RegionType.All, user.Id, image.Id);
            var secondIssue = CreateIssue(true, RegionType.Thracia, user.Id, image.Id);
            var thirdIssue = CreateIssue(true, RegionType.Western, user.Id, image.Id);
            var notApprovedIssue = CreateIssue(false, RegionType.Western, user.Id, image.Id);
            var secondNotApproved = CreateIssue(false, RegionType.Eastern, user.Id, image.Id);
            await this.Db.AddRangeAsync(issue, secondIssue, thirdIssue, notApprovedIssue, secondNotApproved);

            await this.Db.SaveChangesAsync();

            //TODO: rearrange test - now issueService has more parameters
            var service = new IssueService(this.Db, null);

            //Act
            var resultModel = (await service.AllMapInfoDetailsAsync<IssueMapInfoBoxModel>(isApprovedParam, regionParam)).ToList();

            var issuesToNotContain = this.Db.UrbanIssues.Where(i => i.IsApproved != isApprovedParam);

            var notContainFromAnotherRegions = this.Db.UrbanIssues.Where(i => i.Region != regionParam);

            //Assert
            resultModel.Should().AllBeOfType<IssueMapInfoBoxModel>();

            resultModel.Should().NotContain(issuesToNotContain);

            if (regionParam != RegionType.All)
            {
                resultModel.Should().NotContain(notContainFromAnotherRegions);
            }
            
        }

    }
}
