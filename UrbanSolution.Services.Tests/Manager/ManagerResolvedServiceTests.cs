namespace UrbanSolution.Services.Tests.Manager
{
    using Data;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Mocks;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Manager.Models;
    using UrbanSolution.Services.Manager.Implementations;
    using UrbanSolution.Services.Manager;
    using Xunit;

    public class ManagerResolvedServiceTests
    {
        private int issueId;
        private int imageId;

        private const int DefaultPicId = 8965129;
        private const string DefaultDescription = "Description";
        private const string ChangedDescription = "ChangedDescription";
        private const string DefaultUserName = "DefaultManagerUserName";

        private readonly UrbanSolutionDbContext db;

        public ManagerResolvedServiceTests()
        {
            AutomapperInitializer.Initialize();
            this.db = InMemoryDatabase.Get();
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalse_IfManagerId_IsNotEqualTo_ResolvedPublisherId()
        {
            //Arrange
            var issueService = new Mock<IManagerIssueService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ResolvedService(db, issueService.Object, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(null);
            var publisher = this.CreateUser(null);
            await db.AddRangeAsync(manager, publisher);

            var issue = this.CreateIssue();
            await db.AddAsync(issue);

            var resolved = this.CreateResolved(publisher.Id, issue.Id, 0);
            await db.AddAsync(resolved);

            await db.SaveChangesAsync();

            //Act

            var result = await service.DeleteAsync(manager.Id, resolved.Id);

            //Assert

            result.Should().BeFalse();

            issueService.Verify(i => i.RemoveResolvedReferenceAsync(It.IsAny<int>()), Times.Never);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a => 
                a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Never);

        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrueAnd_DeletesCorrectResolvedIssue()
        {
            //Arrange

            var activityService = new Mock<IManagerActivityService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var issueService = new Mock<IManagerIssueService>();

            var service = new ResolvedService(db, issueService.Object, pictureService.Object, activityService.Object);

            var (managerId, secondIssueId, secondResolvedId, resolved) = 
                await this.CreateEntities(db);

            //Act
            var result = await service.DeleteAsync(managerId, secondResolvedId);

            //Assert
            result.Should().BeTrue();

            db.ResolvedIssues.Should().HaveCount(1);
            db.ResolvedIssues.Should().BeEquivalentTo(resolved);

            db.UrbanIssues.First(i => i.Id == secondIssueId).ResolvedIssue.Should().BeNull();

            issueService.Verify(i => i.RemoveResolvedReferenceAsync(It.IsAny<int>()), Times.Once);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a =>
                a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectModel()
        {
            //Arrange
            var service = new ResolvedService(db, null, null, null);

            var (managerId, secondIssueId, secondResolvedId, resolved) =
                await this.CreateEntities(db);

            //Act
            var result = await service.GetAsync<ResolvedIssueEditServiceModel>(resolved.Id);

            //Assert

            result.Should().BeOfType<ResolvedIssueEditServiceModel>();

            //check properties are mapped correctly
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

            var service = new ResolvedService(db, issueService.Object, pictureService.Object, activityService.Object);

            var manager = this.CreateUser(null);
            await db.AddRangeAsync(manager);

            var issue = this.CreateIssue();
            await db.AddAsync(issue);

            var resolved = this.CreateResolved(manager.Id, issue.Id, DefaultPicId);
            await db.AddAsync(resolved);

            await db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(manager.Id, resolved.Id, ChangedDescription, pictureFile: null);

            //Assert
            result.Should().BeTrue();

            resolved.Description.Should().NotMatch(DefaultDescription);
            resolved.Description.Should().Match(ChangedDescription);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Never);

            activityService.Verify(a =>
                a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrueAnd_ChangesResolvedPictureFileHasPassed()
        {
            //Arrange
            var issueService = new Mock<IManagerIssueService>();
            var pictureService = IPictureServiceMock.New(DefaultPicId);
            var activityService = new Mock<IManagerActivityService>();

            var service = new ResolvedService(db, issueService.Object, pictureService.Object, activityService.Object);

            var (managerId, secondIssueId, secondResolvedId, resolved) = await this.CreateEntities(db);

            var file = new Mock<IFormFile>();           

            var oldImage = resolved.CloudinaryImage;

            //Act
            var result = await service.UpdateAsync(managerId, resolved.Id, ChangedDescription, file.Object);

            //Assert
            result.Should().BeTrue();

            resolved.CloudinaryImage.Should().NotBe(oldImage);
            resolved.CloudinaryImageId.Should().Be(DefaultPicId);

            resolved.Description.Should().NotMatch(DefaultDescription);
            resolved.Description.Should().Match(ChangedDescription);

            pictureService.Verify(p => p.DeleteImageAsync(It.IsAny<int>()), Times.Once);

            activityService.Verify(a =>
                a.WriteManagerLogInfoAsync(It.IsAny<string>(), It.IsAny<ManagerActivityType>()), Times.Once);
        }

        private async Task<(string, int, int, ResolvedIssue)> CreateEntities(
            UrbanSolutionDbContext db)
        {
            var manager = this.CreateUser(null);
            await db.AddRangeAsync(manager);

            var issue = this.CreateIssue();
            var secondIssue = this.CreateIssue();
            await db.AddRangeAsync(issue, secondIssue);

            var pic = this.GetImage();
            var secondPic = this.GetImage();
            await db.AddRangeAsync(pic, secondPic);

            var resolved = this.CreateResolved(manager.Id, issue.Id, pic.Id);
            var secondResolved = this.CreateResolved(manager.Id, secondIssue.Id, secondPic.Id);
            await db.AddRangeAsync(resolved, secondResolved);

            issue.ResolvedIssue = resolved;
            secondIssue.ResolvedIssue = secondResolved;

            await db.SaveChangesAsync();

            return (manager.Id, secondIssue.Id, secondResolved.Id, resolved);
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

        private CloudinaryImage GetImage()
        {
            var image = new CloudinaryImage
            {
                Id = ++imageId,
                PicturePublicId = Guid.NewGuid().ToString()
            };

            return image;
        }

        private UrbanIssue CreateIssue()
        {
            var issue = new UrbanIssue
            {
                Id = ++issueId,
            };

            return issue;
        }

        private ResolvedIssue CreateResolved(string publisherId, int issueId, int picId)
        {
            var resolved = new ResolvedIssue
            {
                Id = ++issueId,
                PublisherId = publisherId,
                UrbanIssueId = issueId,
                CloudinaryImageId = picId,
                Description = DefaultDescription
            };

            return resolved;
        }
    }
}
