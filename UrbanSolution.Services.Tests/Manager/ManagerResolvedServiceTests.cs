namespace UrbanSolution.Services.Tests.Manager
{
    using Data;
    using FluentAssertions;
    using Implementations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Mocks;
    using Moq;
    using UrbanSolution.Models;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Services.Manager.Models;
    using UrbanSolution.Services.Manager.Implementations;
    using Xunit;

    public class ManagerResolvedServiceTests
    {
        private int issueId;
        private int imageId;

        private const string DefaultDescription = "Description";
        private const string ChangedDescription = "ChangedDescription";
        private const string DefaultUserName = "DefaultManagerUserName";

        private readonly UrbanSolutionDbContext db;
        private readonly Mock<ManagerActivityService> managerActivityMock;

        public ManagerResolvedServiceTests()
        {
            AutomapperInitializer.Initialize();
            this.db = InMemoryDatabase.Get();
            this.managerActivityMock = new Mock<ManagerActivityService>(this.db);
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsFalse_IfManagerId_IsNotEqualTo_ResolvedPublisherId()
        {
            //Arrange
            var service = new ResolvedService(db, null, null, null);

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
        }

        [Fact]
        public async Task DeleteAsyncShould_ReturnsTrueAnd_DeletesCorrectResolvedIssue()
        {
            //Arrange
            var cloudinaryService = new Mock<CloudinaryService>(ConfigurationMock.New());
            var pictureService = new Mock<PictureService>(db, cloudinaryService.Object);

            var managerIssueService = new Mock<ManagerIssueService>(db, pictureService.Object, this.managerActivityMock.Object);

            var service = new ResolvedService(db, managerIssueService.Object, pictureService.Object, this.managerActivityMock.Object);

            var (managerId, secondIssueId, secondResolvedId, resolved) = 
                await this.CreateEntities(db);

            //Act
            var result = await service.DeleteAsync(managerId, secondResolvedId);

            //Assert
            result.Should().BeTrue();

            db.ResolvedIssues.Should().HaveCount(1);
            db.ResolvedIssues.Should().BeEquivalentTo(resolved);

            (await db.UrbanIssues.FirstAsync(i => i.Id == secondIssueId))
                .ResolvedIssue.Should().BeNull();
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
            result.Id.Should().Be(resolved.Id);
            result.CloudinaryImageId.Should().Be(resolved.CloudinaryImageId);
            result.Description.Should().Be(resolved.Description);
        }


        [Fact]
        public async Task UpdateAsyncShould_ReturnsFalse_IfManagerId_IsNotEqualTo_ResolvedPublisherId()
        {
            //Arrange
            var service = new ResolvedService(db, null, null, this.managerActivityMock.Object);

            var (managerId, secondIssueId, secondResolvedId, resolved) =
                await this.CreateEntities(db);

            //Act
            var result = 
                await service.UpdateAsync(managerId, resolved.Id, resolved.Description, pictureFile: null);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrueAnd_ChangesDescriptionProperty()
        {
            //Arrange
            var service = new ResolvedService(db, null, null, this.managerActivityMock.Object);

            var manager = this.CreateUser(null);
            await db.AddRangeAsync(manager);

            var issue = this.CreateIssue();
            await db.AddAsync(issue);

            var resolved = this.CreateResolved(manager.Id, issue.Id, int.MaxValue);
            await db.AddAsync(resolved);

            await db.SaveChangesAsync();

            //Act
            var result = await service.UpdateAsync(manager.Id, resolved.Id, ChangedDescription, null);

            //Assert
            result.Should().BeTrue();
            resolved.Description.Should().NotMatch(DefaultDescription);
            resolved.Description.Should().Match(ChangedDescription);
        }

        [Fact]
        public async Task UpdateAsyncShould_ReturnsTrueAnd_ChangesResolvedPictureIfIformFileIsGiven()
        {
            const int NewImageId = 25;

            //Arrange
            var pictureService = new Mock<IPictureService>();

            pictureService.Setup(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>())).Returns(Task.FromResult(NewImageId));

            var service = new ResolvedService(db, null, pictureService.Object, this.managerActivityMock.Object);

            var (managerId, secondIssueId, secondResolvedId, resolved) =
                await this.CreateEntities(db);

            var file = new Mock<IFormFile>();           

            var oldImage = resolved.CloudinaryImage;

            //Act
            var result = await service.UpdateAsync(managerId, resolved.Id, ChangedDescription, file.Object);

            //Assert
            result.Should().BeTrue();
            resolved.CloudinaryImage.Should().NotBe(oldImage);
            resolved.CloudinaryImageId.Should().Be(NewImageId);
            resolved.Description.Should().NotMatch(DefaultDescription);
            resolved.Description.Should().Match(ChangedDescription);
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
