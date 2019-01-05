namespace UrbanSolution.Services.Tests.Manager
{
    using Data;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Microsoft.AspNetCore.Http;
    using Models;
    using Mocks;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager.Implementations;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Manager;
    using Xunit;

    public class ManagerIssueServiceTests
    {
        private int issueId;
        private int picId;
        private string issueTitle = "issue{0}";

        private const int DefaultImageId = 999;
        private const string DefaultUserName = "DefaultUserName";
        private const string DefaultPictureUrl = "Pictureurl";
        private const int DefaultPicId = 5879;

        private readonly UrbanSolutionDbContext db;

        public ManagerIssueServiceTests()
        {
            AutomapperInitializer.Initialize();
            this.db = InMemoryDatabase.Get();
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsFalseIfManagerRegionIsNotEqualToIssueRegion()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService( 
                db, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(RegionType.Western);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.North);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, null);

            //Assert
            result.Should().BeFalse();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Never);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);

        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerForAllRegionsUpdatesIssue_WithNoPictureFile()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                db, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(RegionType.All);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, pictureFile: null);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Never);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);

        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerForAllRegionsUpdatesIssue_WithPictureFile()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                db, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(RegionType.All);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            var pictureFile = new Mock<IFormFile>();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, pictureFile: pictureFile.Object);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            //for deleting older image
            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a => a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegionAnd_NoInvocationOfPictureServiceWhen_NoPictureFileIsPassed()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                db, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(RegionType.Western);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, null);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Never);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegionAnd_InvocationOfPictureServiceWhen_PictureFileIsPassed()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                db, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(RegionType.Western);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            var formFileMock = new Mock<IFormFile>();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, formFileMock.Object);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a => a
                .WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalse_IfManagerRegionIsNotEqualToIssueRegion()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(db, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(RegionType.Central);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            //Act
            var result = await service.DeleteAsync(manager, issue.Id);

            //Assert
            result.Should().BeFalse();

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => 
                a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegion_OrManagerIsForAllRegions()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(db, pictureService.Object, activityService.Object);

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

            //Act
            var result = await service.DeleteAsync(manager, issue.Id);

            var result2 = await service.DeleteAsync(secondManager, secondIssue.Id);

            //Assert
            result.Should().BeTrue();

            result2.Should().BeTrue();

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Exactly(2));

            activityService.Verify(a => 
                a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ApproveAsyncShould_ReturnsFalse_IfManagerRegionIsNotEqualToIssueRegion()
        {
            //Arrange
            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(db, null, activityService.Object);

            var manager = this.CreateUser(RegionType.Central);
            await db.AddAsync(manager);

            var issue = this.CreateIssue(RegionType.Western);
            await db.AddAsync(issue);

            await db.SaveChangesAsync();

            //Act
            var result = await service.ApproveAsync(manager, issue.Id);

            //Assert
            result.Should().BeFalse();

            activityService.Verify(a => a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);
        }

        [Fact]
        public async Task ApproveAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegion_OrManagerIsForAllRegionsAnd_SetsIssuePropertyIsApprovedToTrue()
        {
            //Arrange
            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(db, null, activityService.Object);

            var manager = this.CreateUser(RegionType.Western);
            var secondManager = this.CreateUser(RegionType.All);
            await db.AddRangeAsync(manager, secondManager);

            var issue = this.CreateIssue(RegionType.Western, DefaultImageId);
            var secondIssue = this.CreateIssue(RegionType.Thracia, DefaultImageId);
            await db.AddRangeAsync(issue, secondIssue);

            await db.SaveChangesAsync();

            //Act
            var result = await service.ApproveAsync(manager, issue.Id);

            var secondResult = await service.ApproveAsync(secondManager, secondIssue.Id);

            //Assert
            result.Should().BeTrue();

            secondResult.Should().BeTrue();

            this.db.UrbanIssues.FirstOrDefault(i => i.Id == issue.Id)?.IsApproved.Should().BeTrue();

            this.db.UrbanIssues.FirstOrDefault(i => i.Id == secondIssue.Id)?.IsApproved.Should().BeTrue();

            activityService.Verify(a => 
                a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsApprovedIssuesWith_CorrectModel()
        {
            //Arrange
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

            //Act
            var result = await service.AllAsync(isApproved: true, region: RegionType.All); 

            var result2 = await service.AllAsync(true, RegionType.Thracia);

            var result3 = await service.AllAsync(true, RegionType.North);

            //Assert
            result.Should().AllBeOfType<UrbanIssuesListingServiceModel>();

            result.Should().HaveCount(2);
            result.Where(i => i.IsApproved).Should().HaveCount(2);
            result.Where(i => !i.IsApproved).Should().BeEmpty();

            result2.Should().HaveCount(1);
            result2.Where(i => i.IsApproved).Should().HaveCount(1);
            result2.Where(i => !i.IsApproved).Should().BeEmpty();

            result3.Should().BeEmpty();
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsNotApprovedIssuesWith_CorrectModel()
        {
            //Arrange
            var service = new ManagerIssueService(db, null, null);

            var user = this.CreateUser(null);
            await db.AddAsync(user);

            var pic = this.CreateImage();
            await db.AddAsync(pic);

            var issue = this.CreateIssueWithAllProperties(false, RegionType.Western, user.Id, pic.Id);
            var secondIssue = this.CreateIssueWithAllProperties(true, RegionType.Thracia, user.Id, pic.Id);
            var thirdIssue = this.CreateIssueWithAllProperties(false, RegionType.Western, user.Id, pic.Id);
            var fourthIssue = this.CreateIssueWithAllProperties(false, RegionType.North, user.Id, pic.Id);
            await db.UrbanIssues.AddRangeAsync(issue, secondIssue, thirdIssue, fourthIssue);

            await db.SaveChangesAsync();

            //Act
            var result = await service.AllAsync(isApproved: false, region: RegionType.All);

            var result2 = await service.AllAsync(isApproved: false, region: RegionType.North);

            var result3 = await service.AllAsync(isApproved: false, region: RegionType.Eastern);

            //Assert
            result.Should().AllBeOfType<UrbanIssuesListingServiceModel>();

            result.Should().HaveCount(3);
            result.Where(i => !i.IsApproved).Should().HaveCount(3);
            result.Where(i => i.IsApproved).Should().BeEmpty();

            result2.Should().HaveCount(1);
            result2.Where(i => !i.IsApproved).Should().HaveCount(1);
            result2.Where(i => i.IsApproved).Should().BeEmpty();

            result3.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveResolvedReferenceAsyncShould_SettsIssuePropertyResolvedIssueToNull()
        {
            //Arrange
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

            //Act
            await service.RemoveResolvedReferenceAsync(issue.Id);

            var hasResolvedAfter = issue.ResolvedIssue != null; //false

            //Assert
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
