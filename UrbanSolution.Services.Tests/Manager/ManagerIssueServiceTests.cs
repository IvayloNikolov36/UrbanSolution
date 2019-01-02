namespace UrbanSolution.Services.Tests.Manager
{
    using FluentAssertions;
    using FluentAssertions.Common;
    using Mocks;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using Implementations;
    using UrbanSolution.Services.Manager.Implementations;
    using Xunit;

    public class ManagerIssueServiceTests
    {
        private int issueId;
        private int picId;
        private string issueTitle = "issue{0}";

        private const int DefaultImageId = 999;
        private const string DefaultUserName = "DefaultUserName";
        const string DefaultPictureUrl = "Pictureurl";

        public ManagerIssueServiceTests()
        {
            AutomapperInitializer.Initialize();
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsFalseIfManagerRegionIsNotEqualToIssueRegion()
        {
            var db = InMemoryDatabase.Get();

            var service = new ManagerIssueService( db, null, null);

            var manager = this.CreateUser(RegionType.Western);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.North);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            var result = await service.UpdateAsync(manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, null);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegion()
        {
            var db = InMemoryDatabase.Get();

            var pictureService = new Mock<PictureService>(db, null);

            var managerActivityMock = new Mock<ManagerActivityService>(db);

            var service = new ManagerIssueService(
                db, pictureService.Object, managerActivityMock.Object);

            var manager = this.CreateUser(RegionType.Western);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, null);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerForAllRegionsUpdatesIssue()
        {
            var db = InMemoryDatabase.Get();

            var pictureService = new Mock<PictureService>(db, null);

            var managerActivityMock = new Mock<ManagerActivityService>(db);

            var service = new ManagerIssueService(
                db, pictureService.Object, managerActivityMock.Object);

            var manager = this.CreateUser(RegionType.All);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, null);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalse_IfManagerRegionIsNotEqualToIssueRegion()
        {
            var db = InMemoryDatabase.Get();

            var service = new ManagerIssueService(db, null, null);

            var manager = this.CreateUser(RegionType.Central);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            var result = await service.DeleteAsync(manager, issue.Id);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegion_OrManagerIsForAllRegions()
        {
            var db = InMemoryDatabase.Get();


            var cloudinaryService = new Mock<CloudinaryService>(ConfigurationMock.New());

            var pictureService = new Mock<PictureService>(db, cloudinaryService.Object);

            var managerActivityMock = new Mock<ManagerActivityService>(db);

            var service = new ManagerIssueService(
                db, pictureService.Object, managerActivityMock.Object);

            var manager = this.CreateUser(RegionType.Western);
            var secondManager = this.CreateUser(RegionType.All);
            await db.AddRangeAsync(manager, secondManager);

            var pic = this.CreateImage();
            var secondPic = this.CreateImage();
            await db.AddRangeAsync(pic, secondPic);

            var issue = this.CreateIssue(RegionType.Western, pic.Id);
            var secondIssue = this.CreateIssue(RegionType.South, secondPic.Id);

            await db.AddRangeAsync(issue, secondIssue);

            await db.SaveChangesAsync();

            var result = await service.DeleteAsync(manager, issue.Id);
            var result2 = await service.DeleteAsync(secondManager, secondIssue.Id);

            result.Should().BeTrue();
            result2.Should().BeTrue();
        }

        [Fact]
        public async Task ApproveAsyncShould_ReturnsFalse_IfManagerRegionIsNotEqualToIssueRegion()
        {
            var db = InMemoryDatabase.Get();

            var service = new ManagerIssueService(db, null, null);

            var manager = this.CreateUser(RegionType.Central);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            var result = await service.ApproveAsync(manager, issue.Id);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task ApproveAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegion_OrManagerIsForAllRegions()
        {
            var db = InMemoryDatabase.Get();

            var managerActivity = new Mock<ManagerActivityService>(db);

            var service = new ManagerIssueService(db, null, managerActivity.Object);

            var manager = this.CreateUser(RegionType.Western);
            var secondManager = this.CreateUser(RegionType.All);
            await db.AddRangeAsync(manager, secondManager);

            var issue = this.CreateIssue(RegionType.Western, DefaultImageId);
            var secondIssue = this.CreateIssue(RegionType.Thracia, DefaultImageId);
            await db.AddRangeAsync(issue, secondIssue);

            await db.SaveChangesAsync();

            var result = await service.ApproveAsync(manager, issue.Id);
            var secondResult = await service.ApproveAsync(secondManager, secondIssue.Id);

            result.Should().BeTrue();
            secondResult.Should().BeTrue();
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsApprovedIssuesForAllRegions()
        {
            var db = InMemoryDatabase.Get();

            var service = new ManagerIssueService(db, null, null);

            var user = this.CreateUser(null);
            await db.AddAsync(user);

            var pic = this.CreateImage();        
            await db.AddAsync(pic);

            var issue = this.CreateIssueWithAllProperties(true, RegionType.Western, user.Id, pic.Id);
            var secondIssue = this.CreateIssueWithAllProperties(true, RegionType.Thracia, user.Id, pic.Id);
            var thirdIssue = this.CreateIssueWithAllProperties(false, RegionType.Western, user.Id, pic.Id);
            var fourthIssue = this.CreateIssueWithAllProperties(false, RegionType.North, user.Id, pic.Id);        
            await db.UrbanIssues.AddRangeAsync(issue, secondIssue, thirdIssue, fourthIssue);

            await db.SaveChangesAsync();

            //IsApproved = True
            var result = await service.AllAsync(true, RegionType.All); 
            Assert.True(result.Count() == 2);
            Assert.True(result.Last().Id == secondIssue.Id);

            var result2 = await service.AllAsync(true, RegionType.Thracia);
            Assert.True(result2.Count() == 1);
            Assert.True(result2.Last().Id == secondIssue.Id);

            var result3 = await service.AllAsync(true, RegionType.North);
            Assert.True(!result3.Any());

            // IsApproved = False
            var result4 = await service.AllAsync(false, RegionType.All);
            Assert.True(result4.Count() == 2);
            Assert.True(result4.First().Id == thirdIssue.Id);
            Assert.True(result4.Last().Id == fourthIssue.Id);

            var result5 = await service.AllAsync(false, RegionType.North);
            Assert.True(result5.Count() == 1);
            Assert.True(result5.First().Id == fourthIssue.Id);
          
            var result6 = await service.AllAsync(false, RegionType.Eastern);
            Assert.True(!result6.Any());
        }

        [Fact]
        public async Task RemoveResolvedReferenceAsyncShould_SettsIssuePropertyResolvedIssueToNull()
        {
            var db = InMemoryDatabase.Get();

            var service = new ManagerIssueService(db, null, null);

            var user = this.CreateUser(null);
            await db.AddAsync(user);

            var pic = this.CreateImage();
            await db.AddAsync(pic);

            var issue = this.CreateIssueWithAllProperties(true, RegionType.Western, user.Id, pic.Id);
            await db.AddAsync(issue);

            var resolved = new ResolvedIssue
            {
                Id = ++issueId,
                UrbanIssueId = issue.Id
            };
            await db.AddAsync(resolved);

            var hasResolvedBefore = issue.ResolvedIssue != null; //true

            await service.RemoveResolvedReferenceAsync(issue.Id);

            var hasResolvedAfter = issue.ResolvedIssue != null; //false

            hasResolvedAfter.Should().BeFalse();
            hasResolvedAfter.Should().IsSameOrEqualTo(!hasResolvedBefore);
        }
        

        private User CreateUser(RegionType? region)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = DefaultUserName,
                ManagedRegion = region ?? RegionType.All
            };

            return user;
        }

        private UrbanIssue CreateIssue(RegionType region)
        {
            var issue = new UrbanIssue
            {
                Id = ++issueId,
                Region = region
            };

            return issue;
        }

        private UrbanIssue CreateIssue(RegionType region, int imageId)
        {
            var issue = new UrbanIssue
            {
                Id = ++issueId,
                Region = region,
                CloudinaryImageId = imageId
            };

            return issue;
        }

        private UrbanIssue CreateIssueWithAllProperties(bool isApproved, RegionType region, string publisherId, int picId)
        {
            var issue = new UrbanIssue
            {
                Id = ++issueId,
                Title = string.Format(issueTitle, issueId),
                PublishedOn = DateTime.UtcNow,
                Latitude = 42D,
                Longitude = 42D,
                Region = region,
                IsApproved = isApproved,
                PublisherId = publisherId,
                CloudinaryImageId = picId
            };

            return issue;
        }

        private CloudinaryImage CreateImage()
        {
            var image = new CloudinaryImage
            {
                Id = ++picId,
                PictureThumbnailUrl = DefaultPictureUrl
            };

            return image;
        }

    }
}
