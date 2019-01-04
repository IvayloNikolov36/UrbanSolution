namespace UrbanSolution.Services.Tests.Manager
{
    using Data;
    using FluentAssertions;
    using Mapping;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Manager.Implementations;
    using UrbanSolution.Services.Manager.Models;
    using Xunit;

    public class ManagerActivityServiceTests
    {
        private int managerId;

        private const string DefaultUserName = "Default{0}";

        private readonly UrbanSolutionDbContext db;

        public ManagerActivityServiceTests()
        {
            AutomapperInitializer.Initialize();
            this.db = InMemoryDatabase.Get();
        }

        [Fact]
        public async Task GetAsyncShould_ReturnsManagerActivitySorted()
        {
            //Arrange
            var service = new ManagerActivityService(db);

            var manager = this.CreateManager();
            var secondManager = this.CreateManager();
            await this.db.AddRangeAsync(manager, secondManager);

            var managerLogs = this.CreateManagerLogs(manager.Id);
            var secondManagerLogs = this.CreateManagerLogs(secondManager.Id);
            var allLogs = managerLogs.Concat(secondManagerLogs);
            await this.db.AddRangeAsync(allLogs);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.GetAsync(manager.Id);

            var expected = managerLogs.OrderByDescending(x => x.DateTime)
                .AsQueryable().To<ManagerActivitiesListingServiceModel>().ToList();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<ManagerActivitiesListingServiceModel>>();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expected);
            
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsAllManagersActivitySorted()
        {
            //Arrange
            var service = new ManagerActivityService(db);

            var manager = this.CreateManager();
            var secondManager = this.CreateManager();
            await this.db.AddRangeAsync(manager, secondManager);

            var managerLogs = this.CreateManagerLogs(manager.Id);
            var secondManagerLogs = this.CreateManagerLogs(secondManager.Id);
            var allLogs = managerLogs.Concat(secondManagerLogs);
            await this.db.AddRangeAsync(allLogs);

            await this.db.SaveChangesAsync();

            //Act
            var result = await service.AllAsync();

            var expected = allLogs.OrderByDescending(x => x.DateTime)
                .AsQueryable().To<ManagerActivitiesListingServiceModel>().ToList();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<ManagerActivitiesListingServiceModel>>();
            result.Should().HaveCount(4);
            result.Should().BeEquivalentTo(expected);
            
        }

        private List<ManagerLog> CreateManagerLogs(string managerId)
        {
            var log = new ManagerLog
            {
                DateTime = new DateTime(2018, 12, 4),
                ManagerId = managerId
            };

            var secondLog = new ManagerLog
            {
                DateTime = new DateTime(2018, 11, 6),
                ManagerId = managerId,
                Activity = ManagerActivityType.EditedIssue
            };

            return new List<ManagerLog>{ log, secondLog };
        }

        private User CreateManager()
        {
            var manager = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = string.Format(DefaultUserName, ++managerId)
            };

            return manager;
        }
    }
}
