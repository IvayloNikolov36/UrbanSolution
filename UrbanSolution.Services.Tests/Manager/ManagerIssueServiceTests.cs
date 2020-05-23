namespace UrbanSolution.Services.Tests.Manager
{
    using FluentAssertions;
    using FluentAssertions.Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Mocks;
    using Moq;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager.Implementations;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Manager;
    using Xunit;
    using Seed;

    public class ManagerIssueServiceTests : BaseServiceTest
    {
        private const int DefaultImageId = 999;
        private const int DefaultPicId = 5879;

        [Fact]
        public async Task UpdateAsyncShould_ReturnsFalseIfManagerRegionIsNotEqualToIssueRegion()
        {
            //Arrange
            var picService = IPictureServiceMock.New(DefaultPicId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService( 
                Db, picService.Object, activityService.Object);

            var manager = UserCreator.Create(RegionType.Western);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.North);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, null);

            //Assert
            result.Should().BeFalse();

            picService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Never);

            picService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);

        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerForAllRegionsUpdatesIssue_WithNoPictureFile()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                Db, pictureService.Object, activityService.Object);

            var manager = UserCreator.Create(RegionType.All);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, pictureFile: null);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Never);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);

        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerForAllRegionsUpdatesIssue_WithPictureFile()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                Db, pictureService.Object, activityService.Object);

            var manager = UserCreator.Create(RegionType.All);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            var pictureFile = new Mock<IFormFile>();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, pictureFile: pictureFile.Object);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a => a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegionAnd_NoInvocationOfPictureServiceWhen_NoPictureFileIsPassed()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                Db, pictureService.Object, activityService.Object);

            var manager = UserCreator.Create(RegionType.Western);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, null);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Never);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegionAnd_InvocationOfPictureServiceWhen_PictureFileIsPassed()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(
                Db, pictureService.Object, activityService.Object);

            var manager = UserCreator.Create(RegionType.Western);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            var formFileMock = new Mock<IFormFile>();

            //Act
            var result = await service.UpdateAsync(
                manager, issue.Id, null, null, issue.Region, IssueType.DamagedRoads, null, formFileMock.Object);

            //Assert
            result.Should().BeTrue();

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a => a
                .WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalse_IfManagerRegionIsNotEqualToIssueRegion()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(Db, pictureService.Object, activityService.Object);

            var manager = UserCreator.Create(RegionType.Central);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            //Act
            var result = await service.DeleteAsync(manager.Id, manager.ManagedRegion, issue.Id);

            //Assert
            result.Should().BeFalse();

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => 
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegion_OrManagerIsForAllRegions()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(Db, pictureService.Object, activityService.Object);

            var manager = UserCreator.Create(RegionType.Western);
            var secondManager = UserCreator.Create(RegionType.All);
            await Db.AddRangeAsync(manager, secondManager);

            var pic = ImageInfoCreator.Create();
            var secondPic = ImageInfoCreator.Create();
            await Db.AddRangeAsync(pic, secondPic);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western, pic.Id);
            var secondIssue = UrbanIssueCreator.CreateIssue(RegionType.South, secondPic.Id);

            await Db.AddRangeAsync(issue, secondIssue);

            await Db.SaveChangesAsync();

            //Act
            var result = await service.DeleteAsync(manager.Id, manager.ManagedRegion, issue.Id);

            var result2 = await service.DeleteAsync(secondManager.Id, secondManager.ManagedRegion, secondIssue.Id);

            //Assert
            result.Should().BeTrue();

            result2.Should().BeTrue();

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Exactly(2));

            activityService.Verify(a => 
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ApproveAsyncShould_ReturnsFalse_IfManagerRegionIsNotEqualToIssueRegion()
        {
            //Arrange
            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(Db, null, activityService.Object);

            var manager = UserCreator.Create(RegionType.Central);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            //Act
            var result = await service.ApproveAsync(manager, issue.Id);

            //Assert
            result.Should().BeFalse();

            activityService.Verify(a => a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);
        }

        [Fact]
        public async Task ApproveAsyncShould_ReturnsTrue_IfManagerRegionIsEqualToIssueRegion_OrManagerIsForAllRegionsAnd_SetsIssuePropertyIsApprovedToTrue()
        {
            //Arrange            
            var manager = UserCreator.Create(RegionType.Western);
            await Db.AddAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.Western, DefaultImageId);
            await Db.AddAsync(issue);

            await Db.SaveChangesAsync();

            var activityService = new Mock<IManagerActivityService>();

            var service = new ManagerIssueService(Db, null, activityService.Object);

            //Act
            var result = await service.ApproveAsync(manager, issue.Id);

            var dbEntryIsApproved = this.Db.UrbanIssues.FirstOrDefault(i => i.Id == issue.Id)?.IsApproved;

            //Assert
            result.Should().BeTrue();

            dbEntryIsApproved.Should().BeTrue();

            activityService.Verify(a => 
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Theory]
        [InlineData(true, RegionType.Western)]
        [InlineData(true, RegionType.Eastern)]
        [InlineData(true, RegionType.North)]
        [InlineData(true, RegionType.South)]
        [InlineData(true, RegionType.Central)]
        [InlineData(true, RegionType.Thracia)]
        [InlineData(false, RegionType.Western)]
        [InlineData(false, RegionType.Eastern)]
        [InlineData(false, RegionType.North)]
        [InlineData(false, RegionType.South)]
        [InlineData(false, RegionType.Central)]
        [InlineData(false, RegionType.Thracia)]
        public async Task AllAsyncShould_ReturnsApprovedAndNotApprovedIssuesWith_CorrectModel(bool isApproved, RegionType concreteRegion)
        {
            //Arrange
            var service = new ManagerIssueService(Db, null, null);

            var user = UserCreator.Create(null);
            await Db.AddAsync(user);

            var pic = ImageInfoCreator.Create();        
            await Db.AddAsync(pic);

            var issue = UrbanIssueCreator.CreateIssue(true, RegionType.Western, user.Id, pic.Id);
            var secondIssue = UrbanIssueCreator.CreateIssue(true, RegionType.Thracia, user.Id, pic.Id);
            var thirdIssue = UrbanIssueCreator.CreateIssue(false, RegionType.Western, user.Id, pic.Id);
            var fourthIssue = UrbanIssueCreator.CreateIssue(false, RegionType.North, user.Id, pic.Id);
            var fifthIssue = UrbanIssueCreator.CreateIssue(false, RegionType.Thracia, user.Id, pic.Id);
            var sixthIssue = UrbanIssueCreator.CreateIssue(false, RegionType.Western, user.Id, pic.Id);
            var seventhIssue = UrbanIssueCreator.CreateIssue(true, RegionType.South, user.Id, pic.Id);
            await Db.UrbanIssues.AddRangeAsync(issue, secondIssue, thirdIssue, fourthIssue, fifthIssue, sixthIssue, seventhIssue);

            await Db.SaveChangesAsync();

            //Act
            //TODO: change test - no has new parameters - page and takeCount
            (int count, var resultAllRegions) = (await service.AllAsync<UrbanIssuesListingServiceModel>(isApproved, RegionType.All, 1, 5));

            var expectedAllRegions = await this.Db.UrbanIssues.Where(i => i.IsApproved == isApproved).ToListAsync();

            (int countConcreteRegion, var resultConcreteRegion) = (await service.AllAsync<UrbanIssuesListingServiceModel>(isApproved, concreteRegion, 1, 5));

            var expectedConcreteRegion = await this.Db.UrbanIssues.Where(i => i.IsApproved == isApproved)
                .Where(i => i.Region == concreteRegion).ToListAsync();

            //Assert
            resultAllRegions.Should().AllBeOfType<UrbanIssuesListingServiceModel>();
            resultAllRegions.Should().HaveCount(expectedAllRegions.Count);

            resultConcreteRegion.Should().HaveCount(expectedConcreteRegion.Count);
            resultConcreteRegion.Should().HaveCount(expectedConcreteRegion.Count);

        }

        [Fact]
        public async Task RemoveResolvedReferenceAsyncShould_SettsIssuePropertyResolvedIssueToNull()
        {
            //Arrange
            var service = new ManagerIssueService(Db, null, null);

            var user = UserCreator.Create(null);
            await Db.AddAsync(user);

            var pic = ImageInfoCreator.Create();
            await Db.AddAsync(pic);

            var issue = UrbanIssueCreator.CreateIssue(true, RegionType.Western, user.Id, pic.Id);
            await Db.AddAsync(issue);

            var resolved = ResolvedCreator.Create(user.Id, issue.Id, pic.Id);

            await Db.AddAsync(resolved);

            var hasResolvedBefore = issue.ResolvedIssue != null; //true

            //Act
            await service.RemoveResolvedReferenceAsync(issue.Id);

            var hasResolvedAfter = issue.ResolvedIssue != null; //false

            //Assert
            hasResolvedAfter.Should().BeFalse();

            hasResolvedAfter.Should().IsSameOrEqualTo(!hasResolvedBefore);
        }

    }
}
