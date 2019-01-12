namespace UrbanSolution.Services.Tests.Manager
{
    using Data;
    using FluentAssertions;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Seed;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Services.Manager.Implementations;
    using UrbanSolution.Services.Manager.Models;
    using Xunit;

    public class ManagerActivityServiceTests : BaseServiceTest
    {

        [Fact]
        public async Task GetAsyncShould_ReturnsManagerActivitySorted()
        {
            //Arrange
            var manager = UserCreator.Create();
            var secondManager = UserCreator.Create();
            await this.Db.AddRangeAsync(manager, secondManager);

            var managerLogs = ManagerLogCreator.Create(manager.Id);
            var secondManagerLogs = ManagerLogCreator.Create(secondManager.Id);
            await this.Db.AddRangeAsync(managerLogs);
            await this.Db.AddRangeAsync(secondManagerLogs);

            await this.Db.SaveChangesAsync();

            var service = new ManagerActivityService(Db);

            //Act
            var result = await service.GetAsync(manager.Id);

            var expected = await this.Db.ManagerLogs
                .Where(m => m.ManagerId == manager.Id)
                .OrderByDescending(a => a.DateTime)
                .To<ManagerActivitiesListingServiceModel>()
                .ToListAsync();

            //Assert
            result.Should().NotBeNull();

            result.Should().BeOfType<List<ManagerActivitiesListingServiceModel>>();

            result.Should().BeInDescendingOrder(x => x.DateTime);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsAllManagersActivitySorted()
        {
            //Arrange
            var manager = UserCreator.Create();
            var secondManager = UserCreator.Create();
            await this.Db.AddRangeAsync(manager, secondManager);

            var managerLogs = ManagerLogCreator.Create(manager.Id);
            var secondManagerLogs = ManagerLogCreator.Create(secondManager.Id);
            await this.Db.AddRangeAsync(managerLogs);
            await this.Db.AddRangeAsync(secondManagerLogs);

            await this.Db.SaveChangesAsync();

            var service = new ManagerActivityService(Db);

            //Act
            var result = await service.AllAsync();

            var expected = await this.Db.ManagerLogs
                .OrderByDescending(a => a.DateTime)
                .To<ManagerActivitiesListingServiceModel>()
                .ToListAsync();

            //Assert
            result.Should().NotBeNull();

            result.Should().BeOfType<List<ManagerActivitiesListingServiceModel>>();

            result.Should().BeInDescendingOrder(x => x.DateTime);

            result.Should().HaveCount(expected.Count);

            result.Should().BeEquivalentTo(expected);          
        }

        //TODO: WriteManagerLogInfoAsync method tests
    }
}
