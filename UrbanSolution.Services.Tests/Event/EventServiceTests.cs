namespace UrbanSolution.Services.Tests.Event
{
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
    using Xunit;
    using static UrbanSolutionUtilities.WebConstants;

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

            var service = new EventService(Db, null);

            var firstEvent = EventCreator.Create();
            var secondEvent = EventCreator.Create();
            var thirdEvent = EventCreator.Create();
            await this.Db.AddRangeAsync(firstEvent, secondEvent, thirdEvent);

            await this.Db.SaveChangesAsync();

            //Act
            var result = (await service.AllAsync<EventsListingServiceModel>(page)).ToList();  

            var expectedCount = await this.Db.Events.Skip((page - 1) * EventsPageSize)
                .Take(EventsPageSize).CountAsync();

            //Assert
            result.Should().HaveCount(expectedCount);
            result.Should().AllBeOfType<EventsListingServiceModel>();
            result.Should().BeInDescendingOrder(x => x.Id);
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
            const int eventId = 589;
            const string description = "Description";

            //Arrange
            var service = new EventService(Db, null);
            var firstEvent = EventCreator.CreateEvent(eventId, description);

            await this.Db.AddAsync(firstEvent);
            await this.Db.SaveChangesAsync();

            //Act
            var result = await service.GetAsync<EventDetailsServiceModel>(firstEvent.Id);
            
            //Assert
            result.Should().BeOfType<EventDetailsServiceModel>();
            result.Id.Should().Be(eventId);
            result.Description.Should().Be(description);
        }

        [Fact]
        public async Task TotalCountAsyncShould_ReturnsCorrectCountOfAllEventsInDb()
        {
            //Arrange

            var firstEvent = EventCreator.Create();
            var secondEvent = EventCreator.Create();
            await this.Db.AddRangeAsync(firstEvent, secondEvent);

            await this.Db.SaveChangesAsync();

            var service = new EventService(Db, null);

            //Act
            var result = await service.TotalCountAsync();

            var expectedCount = await this.Db.Events.CountAsync();

            //Assert
            result.Should().Be(expectedCount);
        }

        [Fact]
        public async Task TotalCountAsyncShould_ReturnsCountZeroIf_NoEventsInDB()
        {
            //Arrange

            var service = new EventService(Db, null);

            //Act
            var result = await service.TotalCountAsync();

            var expectedCount = 0;

            //Assert
            result.Should().Be(expectedCount);
        }

        [Fact]
        public async Task ParticipateShould_ReturnFalseIf_UserIsAlreadyParticipatingEvent()
        {
            //Arrange
            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.Create();
            await this.Db.AddAsync(image);

            var firstEvent = EventCreator.Create(user.Id, null);
            await this.Db.AddAsync(firstEvent);

            var eventUser = EventCreator.CreateEventUser(firstEvent.Id, user.Id);
            await this.Db.AddAsync(eventUser);

            await this.Db.SaveChangesAsync();

            var service = new EventService(Db, null);

            //Act
            var result = await service.Participate(firstEvent.Id, user.Id);

            //Assert

            result.Should().Be(false);
        }

        [Fact]
        public async Task ParticipateShould_ReturnsTrueIf_UserIsNotParticipatingEvent()
        {
            //Arrange
            var user = UserCreator.Create();
            await this.Db.AddAsync(user);

            var image = ImageInfoCreator.Create();
            await this.Db.AddAsync(image);

            var firstEvent = EventCreator.Create(user.Id, null);
            await this.Db.AddAsync(firstEvent);

            await this.Db.SaveChangesAsync();

            var service = new EventService(Db, null);

            //Act
            var result = await service.Participate(firstEvent.Id, user.Id);

            //Assert

            result.Should().BeTrue();

            this.Db.EventUsers.Should().Contain(eu => eu.EventId == firstEvent.Id && eu.ParticipantId == user.Id);
        }
    }
}
