namespace UrbanSolution.Services.Tests.Event
{
    using Data;
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Mocks;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Events.Implementations;
    using UrbanSolution.Services.Events.Models;
    using Utilities;
    using Xunit;

    public class EventServiceTests
    {
        private int eventId = 0;
        private const string DefaultUserName = "Username";
        private const int DefaultImageId = 5896324;

        private readonly UrbanSolutionDbContext db;

        public EventServiceTests()
        {
            AutomapperInitializer.Initialize();
            this.db = InMemoryDatabase.Get();
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectEventModelAndCountWith_DefaultPageEqualsToOne()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var service = new EventService(db, pictureService.Object);

            var user = this.CreateEventCreator();
            await this.db.AddAsync(user);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var firstEvent = this.CreateEvent(user.Id, null);
            var secondEvent = this.CreateEvent(user.Id, null);
            var thirdEvent = this.CreateEvent(user.Id, null);
            await this.db.AddRangeAsync(firstEvent, secondEvent, thirdEvent);

            await this.db.SaveChangesAsync();

            //Act
            IEnumerable<EventsListingServiceModel> result = await service
                .AllAsync<EventsListingServiceModel>();  //Skip(page-1 * BlogArticlesPageSize ).Take(BlogArticlesPageSize)

            //Assert
            result.Should().HaveCount(ServiceConstants.EventsPageSize); //2
            result.Should().AllBeOfType<EventsListingServiceModel>();

            result.Should().BeInDescendingOrder(x => x.Id);
            result.First().Id.Should().Be(thirdEvent.Id);
            result.Last().Id.Should().Be(secondEvent.Id);
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectEventModelAndCountWith_PageTwo()
        {
            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);

            var service = new EventService(db, pictureService.Object);

            var user = this.CreateEventCreator();
            await this.db.AddAsync(user);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var firstEvent = this.CreateEvent(user.Id, null);
            var secondEvent = this.CreateEvent(user.Id, null);
            var thirdEvent = this.CreateEvent(user.Id, null);
            await this.db.AddRangeAsync(firstEvent, secondEvent, thirdEvent);

            await this.db.SaveChangesAsync();

            //Act
            IEnumerable<EventsListingServiceModel> result = await service
                .AllAsync<EventsListingServiceModel>(page: 2);  //Skip(page-1 * BlogArticlesPageSize ).Take(BlogArticlesPageSize)

            //Assert
            result.Should().HaveCount(1); //1
            result.Should().AllBeOfType<EventsListingServiceModel>();

            result.Should().BeInDescendingOrder(x => x.Id);
            result.First().Id.Should().Be(firstEvent.Id);
        }

        [Fact]
        public async Task CreateAsyncShould_ReturnsEventId_AndShould_SetsThePropertiesOfEventCorrectly()
        {
            const string TitleToSet = "EventTitle";
            const string DescriptionToSet = "ContentForEvent";
            const string AddressToSet = "AddressForEvent";
            const string LatitudeToSet = "45.368";
            const string LongitudeToSet = "89.256";
            DateTime startsToSet = DateTime.UtcNow.AddDays(2);
            DateTime endsToSet = DateTime.UtcNow.AddDays(3);

            //Arrange
            var pictureService = IPictureServiceMock.New(DefaultImageId);
            var service = new EventService(db, pictureService.Object);

            var user = this.CreateEventCreator();
            await this.db.AddAsync(user);

            await this.db.SaveChangesAsync();

            var formFile = new Mock<IFormFile>();
            //Act
            var result = await service.CreateAsync(TitleToSet, DescriptionToSet, startsToSet, endsToSet, 
                formFile.Object, AddressToSet, LatitudeToSet, LongitudeToSet, user.Id);

            //Assert
            result.Should().BeOfType(typeof(int));

            pictureService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            var savedEvent = db.Find<UrbanSolution.Models.Event>(result);

            savedEvent.Id.Should().Be(result);
            savedEvent.Title.Should().Match(TitleToSet);
            savedEvent.Address.Should().Match(AddressToSet);
            savedEvent.CreatorId.Should().Match(user.Id);
            savedEvent.EndDate.Should().Be(endsToSet);
            savedEvent.StartDate.Should().Be(startsToSet);
            savedEvent.Description.Should().Match(DescriptionToSet);
            savedEvent.Latitude.Should().BeOfType(typeof(double));
            savedEvent.Longitude.Should().BeOfType(typeof(double));
        }

        [Fact]
        public async Task EditAsyncShould_ReturnsFalse_IfEventEditorIsNotEventCreator()
        {
            //Arrange
            var service = new EventService(db, null);

            var creator = this.CreateEventCreator();
            var user = this.CreateEventCreator();
            await this.db.AddRangeAsync(creator, user);

            var eventObj = this.CreateEvent(creator.Id, null);
            await this.db.AddAsync(eventObj);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.EditAsync(eventObj.Id, user.Id, null, null, DateTime.Now, DateTime.Now, 
                null, null, null);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task EditAsyncShould_ReturnsTrue_AndShould_SetsThePropertiesOfEventCorrectly()
        {
            const string TitleToSet = "EventTitle";
            const string DescriptionToSet = "ContentForEvent";
            const string AddressToSet = "AddressForEvent";
            const string LatitudeToSet = "45.368";
            const string LongitudeToSet = "89.256";
            DateTime startsToSet = DateTime.UtcNow.AddDays(2);
            DateTime endsToSet = DateTime.UtcNow.AddDays(3);
            
            //Arrange
            var service = new EventService(db, null);

            var creator = this.CreateEventCreator();
            await this.db.AddAsync(creator);

            var img = this.CreateImage(DefaultImageId, creator.Id);
            await this.db.AddAsync(img);

            var eventObj = this.CreateEvent(creator.Id, img.Id);
            await this.db.AddAsync(eventObj);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.EditAsync(eventObj.Id, creator.Id, TitleToSet, DescriptionToSet, startsToSet,
                endsToSet, AddressToSet, LatitudeToSet, LongitudeToSet);

            //Assert
            result.Should().BeTrue();

            eventObj.Title.Should().Match(TitleToSet);
            eventObj.Address.Should().Match(AddressToSet);
            eventObj.CreatorId.Should().Match(creator.Id);
            eventObj.EndDate.Should().Be(endsToSet);
            eventObj.StartDate.Should().Be(startsToSet);
            eventObj.Description.Should().Match(DescriptionToSet);
            eventObj.Latitude.Should().Be(double.Parse(LatitudeToSet, CultureInfo.InvariantCulture));
            eventObj.Longitude.Should().Be(double.Parse(LongitudeToSet, CultureInfo.InvariantCulture));
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectModel()
        {
            //Arrange
            var service = new EventService(db, null);

            var user = this.CreateEventCreator();
            await this.db.AddAsync(user);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var firstEvent = this.CreateEvent(user.Id, null);
            var secondEvent = this.CreateEvent(user.Id, null);
            await this.db.AddRangeAsync(firstEvent, secondEvent);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.GetAsync<EventEditServiceModel>(secondEvent.Id);

            var secondResult = await service.GetAsync<EventDetailsServiceModel>(firstEvent.Id);

            //Assert
            result.Should().BeOfType<EventEditServiceModel>();
            result.Address.Should().Match(secondEvent.Address);
            result.Latitude.Should().Match(secondEvent.Latitude.ToString(CultureInfo.InvariantCulture));
            result.Longitude.Should().Match(secondEvent.Longitude.ToString(CultureInfo.InvariantCulture));

            secondResult.Should().BeOfType<EventDetailsServiceModel>();
            secondResult.Id.Should().Be(firstEvent.Id);
            secondResult.CreatorUserName.Should().Match(user.UserName);
            secondResult.Latitude.Should().Match(firstEvent.Latitude.ToString(CultureInfo.InvariantCulture));
            secondResult.Longitude.Should().Match(firstEvent.Longitude.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public async Task ExistsAsyncShould_ReturnsTrueIf_EventExistsInDbAnd_FalseInOtherCase()
        {
            const int RandomEventId = 32147893;

            //Arrange
            var service = new EventService(db, null);

            var user = this.CreateEventCreator();
            await this.db.AddAsync(user);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var firstEvent = this.CreateEvent(user.Id, null);
            var secondEvent = this.CreateEvent(user.Id, null);
            await this.db.AddRangeAsync(firstEvent, secondEvent);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.ExistsAsync(secondEvent.Id);

            var secondResult = service.ExistsAsync(RandomEventId);

            //Assert
            result.Should().BeTrue();

            secondResult.Should().NotBe(true);
        }

        [Fact]
        public async Task TotalCountAsyncShould_ReturnsCorrectCountOfAllEventsInDb()
        {
            //Arrange
            var service = new EventService(db, null);

            var user = this.CreateEventCreator();
            await this.db.AddAsync(user);

            var image = this.CreateImage(null, string.Empty);
            await this.db.AddAsync(image);

            var firstEvent = this.CreateEvent(user.Id, null);
            var secondEvent = this.CreateEvent(user.Id, null);
            await this.db.AddRangeAsync(firstEvent, secondEvent);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.TotalCountAsync();

            //Assert
            result.Should().Be(this.db.Events.Count());
        }

        private CloudinaryImage CreateImage(int? imageId, string uploaderId)
        {
            var image = new CloudinaryImage
            {
                Id = imageId ?? DefaultImageId,
                PictureUrl = Guid.NewGuid().ToString(),
                Length = long.MaxValue,
                PicturePublicId = Guid.NewGuid().ToString(),
                PictureThumbnailUrl = Guid.NewGuid().ToString(),
                UploadedOn = DateTime.UtcNow,
                UploaderId = uploaderId
            };

            return image;
        }

        private User CreateEventCreator()
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = string.Format(DefaultUserName)
            };

            return user;
        }

        private UrbanSolution.Models.Event CreateEvent(string userId, int? cloudinaryImageId)
        {
            var eventObj = new UrbanSolution.Models.Event
            {
                Id = ++this.eventId,
                CloudinaryImageId = cloudinaryImageId ?? DefaultImageId,
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(4),
                CreatorId = userId,
                Address = Guid.NewGuid().ToString(),
            };

            return eventObj;
        }
    }
}
