namespace UrbanSolution.Services.Tests.Manager
{
    using Data;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Mocks;
    using Moq;
    using Seed;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Manager.Models;
    using UrbanSolution.Services.Manager.Implementations;
    using UrbanSolution.Services.Manager;
    using Xunit;

    public class ManagerResolvedServiceTests : BaseServiceTest
    {
        private const int DefaultPicId = 8965129;
        private const string DefaultDescription = "Description";
        private const string ChangedDescription = "ChangedDescription";

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalse_IfManagerId_IsNotEqualTo_ResolvedPublisherId()
        {
            //Arrange
            var issueService = new Mock<IManagerIssueService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ResolvedService(Db, issueService.Object, pictureService.Object, activityService.Object);

            var manager = UserCreator.Create();
            var publisher = UserCreator.Create();
            await Db.AddRangeAsync(manager, publisher);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.All);
            await Db.AddAsync(issue);

            var resolved = ResolvedCreator.Create(publisher.Id, issue.Id, 0);
            await Db.AddAsync(resolved);

            await Db.SaveChangesAsync();

            //Act

            var result = await service.DeleteAsync(manager.Id, resolved.Id);

            //Assert

            result.Should().BeFalse();

            issueService.Verify(i => i.RemoveResolvedReferenceAsync(It.IsAny<int>()), Times.Never);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => 
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrueAnd_DeletesCorrectResolvedIssue()
        {
            //Arrange
            var activityService = new Mock<IManagerActivityService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var issueService = new Mock<IManagerIssueService>();

            var service = new ResolvedService(Db, issueService.Object, pictureService.Object, activityService.Object);

            var (managerId, secondIssue, secondResolved, resolved) = 
                await ResolvedCreator.CreateResolvedManagerAndIssue(Db);

            //Act
            var result = await service.DeleteAsync(managerId, secondResolved.Id);

            var resolvedIssuesAfter = await this.Db.ResolvedIssues.ToListAsync();

            var urbanIssueAfter = await this.Db.UrbanIssues.Where(i => i.Id == secondIssue.Id).FirstOrDefaultAsync();

            //Assert
            result.Should().BeTrue();

            Db.ResolvedIssues.Should().HaveCount(resolvedIssuesAfter.Count);

            Db.ResolvedIssues.Should().BeEquivalentTo(resolvedIssuesAfter);

            urbanIssueAfter.ResolvedIssue.Should().BeNull();

            issueService.Verify(i => i.RemoveResolvedReferenceAsync(It.IsAny<int>()), Times.Once);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a =>
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectModel()
        {
            //Arrange
            var service = new ResolvedService(Db, null, null, null);

            var resolved = await ResolvedCreator.Create(Db);

            //Act
            var result = await service.GetAsync<ResolvedIssueEditServiceModel>(resolved.Id);

            //Assert
            result.Should().BeOfType<ResolvedIssueEditServiceModel>();

            result.Id.Should().Be(resolved.Id);
            result.CloudinaryImageId.Should().Be(resolved.CloudinaryImageId);
            result.Description.Should().Be(resolved.Description);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrueAnd_ChangesDescriptionProperty()
        {
            //Arrange
            var issueService = new Mock<IManagerIssueService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ResolvedService(Db, issueService.Object, pictureService.Object, activityService.Object);

            var (manager, resolved) = await ResolvedCreator.CreateResolvedAndManager(Db);

            //Act
            var result = await service.UpdateAsync(manager.Id, resolved.Id, ChangedDescription, pictureFile: null);

            //Assert
            result.Should().BeTrue();

            resolved.Description.Should().NotMatch(DefaultDescription);
            resolved.Description.Should().Match(ChangedDescription);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a =>
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrueAnd_ChangesResolvedPictureFileHasPassed()
        {
            //Arrange
            var issueService = new Mock<IManagerIssueService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ResolvedService(Db, issueService.Object, pictureService.Object, activityService.Object);

            var (manager, resolved) = await ResolvedCreator.CreateResolvedAndManager(Db);

            var file = new Mock<IFormFile>();           

            var oldImage = resolved.CloudinaryImage;

            //Act
            var result = await service.UpdateAsync(manager.Id, resolved.Id, ChangedDescription, file.Object);

            //Assert
            result.Should().BeTrue();

            resolved.CloudinaryImage.Should().NotBe(oldImage);
            resolved.CloudinaryImageId.Should().Be(DefaultPicId);

            resolved.Description.Should().NotMatch(DefaultDescription);
            resolved.Description.Should().Match(ChangedDescription);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a =>
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsFalseIf_UpdaterIsNotTheSameAsUploader()
        {
            //Arrange
            var publisher = UserCreator.Create();
            var updater = UserCreator.Create();
            await this.Db.AddRangeAsync(publisher, updater);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.All);
            await this.Db.AddAsync(issue);

            var resolved = ResolvedCreator.Create(publisher.Id, issue.Id, DefaultPicId);
            await this.Db.AddAsync(resolved);

            await this.Db.SaveChangesAsync();

            var issueService = new Mock<IManagerIssueService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ResolvedService(Db, issueService.Object, pictureService.Object, activityService.Object);

            //Act
            var result = await service.UpdateAsync(updater.Id, resolved.Id, ChangedDescription, null);

            var resolvedAfterAct = await this.Db.FindAsync<ResolvedIssue>(resolved.Id);

            //Assert
            result.Should().Be(false);

            resolved.Should().BeEquivalentTo(resolvedAfterAct);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a =>
                a.WriteLogAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);
        }
       
    }
}
