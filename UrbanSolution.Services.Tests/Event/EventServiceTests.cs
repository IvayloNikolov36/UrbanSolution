namespace UrbanSolution.Services.Tests.Event
{
    using Data;
    using FluentAssertions;
    using Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Mocks;
    using Moq;
    using Seed;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Events.Implementations;
    using UrbanSolution.Services.Events.Models;
    using Utilities;
    using Xunit;

    public class EventServiceTests : BaseServiceTest
    {
        private const int DefaultImageId = 5896324;
        private const string TitleToSet = "EventTitle";
        private const string DescriptionToSet = "ContentForEvent";
        private const string AddressToSet = "AddressForEvent";
        private const string LatitudeToSet = "45.368";
        private const string LongitudeToSet = "89.256";
        private DateTime startDateToSet = DateTime.UtcNow.AddDays(2);
        private DateTime endDateToSet = DateTime.UtcNow.AddDays(3);

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task AllAsyncShould_ReturnsCorrectEventModelAndCountWith_DefaultPageEqualsToOne(int page)
        {
            //Arrange
            var picService = IPictureServiceMock.New(DefaultImageId);

            var service = new EventService(Db, picService.Object);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.Create();
            await this.Db.AddAsync(image);

            var firstEvent = EventCreator.Create(user.Id, null);
            var secondEvent = EventCreator.Create(user.Id, null);
            var thirdEvent = EventCreator.Create(user.Id, null);
            await this.Db.AddRangeAsync(firstEvent, secondEvent, thirdEvent);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.AllAsync<EventsListingServiceModel>(page: page);  

            var expected = await this.Db
                .Events
                .OrderByDescending(e => e.Id)
                .Skip((page - 1) * ServiceConstants.EventsPageSize)
                .Take(ServiceConstants.EventsPageSize)
                .To<EventsListingServiceModel>()
                .ToListAsync();

            //Assert
            result.Should().HaveCount(expected.Count);

            result.Should().AllBeOfType<EventsListingServiceModel>();

            result.Should().BeInDescendingOrder(x => x.Id);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateAsyncShould_ReturnsEventId_AndShould_SetsThePropertiesOfEventCorrectly()
        {           
            //Arrange
            var picService = IPictureServiceMock.New(DefaultImageId);
            var service = new EventService(Db, picService.Object);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            await this.Db.SaveChangesAsync();

            var formFile = new Mock<IFormFile>();

            //Act
            int resultId = await service.CreateAsync(TitleToSet, DescriptionToSet, startDateToSet, endDateToSet, 
                formFile.Object, AddressToSet, LatitudeToSet, LongitudeToSet, user.Id);

            //Assert
            resultId.Should().BeOfType(typeof(int));

            picService.Verify(p => p.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);

            var savedEvent = Db.Find<Event>(resultId);

            savedEvent.Id.Should().Be(resultId);
            savedEvent.Title.Should().Match(TitleToSet);
            savedEvent.Address.Should().Match(AddressToSet);
            savedEvent.CreatorId.Should().Match(user.Id);
            savedEvent.EndDate.Should().Be(endDateToSet);
            savedEvent.StartDate.Should().Be(startDateToSet);
            savedEvent.Description.Should().Match(DescriptionToSet);
            savedEvent.Latitude.Should().BeOfType(typeof(double));
            savedEvent.Longitude.Should().BeOfType(typeof(double));
        }

        [Fact]
        public async Task EditAsyncShould_ReturnsFalse_IfEventEditorIsNotEventCreator()
        {
            //Arrange
            var service = new EventService(Db, null);

            var creator = UserCreator.Create();
            var user = UserCreator.Create();
            await this.Db.AddRangeAsync(creator, user);

            var eventObj = EventCreator.Create(creator.Id, null);
            await this.Db.AddAsync(eventObj);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.EditAsync(eventObj.Id, user.Id, null, null, DateTime.Now,
                DateTime.Now, null, null, null);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task EditAsyncShould_ReturnsTrue_AndShould_SetsThePropertiesOfEventCorrectly()
        {
                      
            //Arrange
            var creator = UserCreator.Create();
            await this.Db.AddAsync(creator);

            var img = ImageInfoCreator.CreateWithFullData(creator.Id);
            await this.Db.AddAsync(img);

            var eventObj = EventCreator.Create(creator.Id, img.Id);
            await this.Db.AddAsync(eventObj);

            await this.Db.SaveChangesAsync();

            var service = new EventService(Db, null);

            //Act
            var result = await service.EditAsync(eventObj.Id, creator.Id, TitleToSet, DescriptionToSet, startDateToSet,
                endDateToSet, AddressToSet, LatitudeToSet, LongitudeToSet);

            //Assert
            result.Should().BeTrue();

            eventObj.Title.Should().Match(TitleToSet);
            eventObj.Address.Should().Match(AddressToSet);
            eventObj.CreatorId.Should().Match(creator.Id);
            eventObj.EndDate.Should().Be(endDateToSet);
            eventObj.StartDate.Should().Be(startDateToSet);
            eventObj.Description.Should().Match(DescriptionToSet);
            eventObj.Latitude.Should().Be(double.Parse(LatitudeToSet, CultureInfo.InvariantCulture));
            eventObj.Longitude.Should().Be(double.Parse(LongitudeToSet, CultureInfo.InvariantCulture));
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsCorrectModel()
        {
            //Arrange
            var service = new EventService(Db, null);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.Create();
            await this.Db.AddAsync(image);

            var firstEvent = EventCreator.Create(user.Id, null);
            var secondEvent = EventCreator.Create(user.Id, null);
            await this.Db.AddRangeAsync(firstEvent, secondEvent);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.GetAsync<EventEditServiceModel>(secondEvent.Id);
            var expected = await this.Db.Events
                .Where(e => e.Id == secondEvent.Id)
                .To<EventEditServiceModel>()
                .FirstOrDefaultAsync();

            var secondResult = await service.GetAsync<EventDetailsServiceModel>(firstEvent.Id);
            var secondExpected = await this.Db.Events
                .Where(e => e.Id == firstEvent.Id)
                .To<EventDetailsServiceModel>()
                .FirstOrDefaultAsync();

            //Assert
            result.Should().BeOfType<EventEditServiceModel>();
            result.Should().BeEquivalentTo(expected);

            secondResult.Should().BeOfType<EventDetailsServiceModel>();
            secondResult.Should().BeEquivalentTo(secondExpected);
        }

        [Fact]
        public async Task ExistsAsyncShould_ReturnsTrueIf_EventExistsInDbAnd_FalseInOtherCase()
        {
            const int RandomEventId = 32147893;

            //Arrange
            var service = new EventService(Db, null);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.Create();
            await this.Db.AddAsync(image);

            var firstEvent = EventCreator.Create(user.Id, null);
            var secondEvent = EventCreator.Create(user.Id, null);
            await this.Db.AddRangeAsync(firstEvent, secondEvent);

            await this.Db.SaveChangesAsync();

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
            var service = new EventService(Db, null);

            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.Create();
            await this.Db.AddAsync(image);

            var firstEvent = EventCreator.Create(user.Id, null);
            var secondEvent = EventCreator.Create(user.Id, null);
            await this.Db.AddRangeAsync(firstEvent, secondEvent);

            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.TotalCountAsync();

            var expectedCount = await this.Db.Events.CountAsync();

            //Assert
            result.Should().Be(expectedCount);
        }

    }
}
