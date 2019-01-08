using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Models.Enums;
using UrbanSolution.Services.Admin;
using UrbanSolution.Services.Admin.Models;
using Xunit;

namespace UrbanSolution.Services.Tests.Admin
{
    public class AdminActivityServiceTests
    {
        private const string DefaultUsername = "Username";
        private const string ManagerRole = "Manager";
        private const string BloggerRole = "Blog Author";

        private readonly UrbanSolutionDbContext db;

        public AdminActivityServiceTests()
        {
            this.db = InMemoryDatabase.Get();
            AutomapperInitializer.Initialize();
        }

        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectModelWith_CorrectData()
        {
            var admin = this.CreateUser();
            var user = this.CreateUser();

            await this.db.AddRangeAsync(admin, user);

            var adminLog = this.CreateAdminLog(admin.Id, user.Id, ManagerRole);
            var secondAdminLog = this.CreateAdminLog(admin.Id, user.Id, BloggerRole);

            await this.db.AddRangeAsync(adminLog, secondAdminLog);

            await this.db.SaveChangesAsync();

            var service = new AdminActivityService(this.db);

            //Act
            var result = await service.AllAsync(admin.Id);

            var expectedCount = this.db.AdminLogs.Count();

            var editedUsersUserNames = this.db.AdminLogs
                .Where(al => al.AdminId == admin.Id)
                .OrderByDescending(al => al.CreatedOn)
                .Select(al => al.EditedUser.UserName).ToList();

            //Assert
            result.Should().BeOfType<List<AdminActivitiesListingServiceModel>>();

            result.Should().HaveCount(expectedCount);

            result.Should().BeInDescendingOrder(a => a.CreatedOn);

            var logsModel = result.ToList();

            for (int i = 0; i < logsModel.Count; i++)
            {
                var log = logsModel[i];
                editedUsersUserNames[i].Should().Be(log.EditedUserUserName);

            }
        }

        [Fact]
        public async Task WriteInfoAsyncShould_WriteTheCorrectDataInDatabase()
        {
            string adminId = Guid.NewGuid().ToString();
            string userId = Guid.NewGuid().ToString();
            const string Role = "Manager";
            AdminActivityType activityType = AdminActivityType.AddedToRole;

            //Arrange
            var service = new AdminActivityService(this.db);

            //Act
            var result = await service.WriteInfoAsync(adminId, userId, Role, activityType);
            var savedLog = this.db.AdminLogs.First(al => al.Id == result);

            //Assert
            result.Should().BeOfType(typeof(int));

            savedLog.Id.Should().Be(result);
            savedLog.AdminId.Should().Be(adminId);
            savedLog.EditedUserId.Should().Be(userId);
            savedLog.Activity.Should().Be(activityType);
            savedLog.ForRole.Should().Match(Role);
        }

        private User CreateUser()
        {
            return new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = DefaultUsername
            };
        }

        private AdminLog CreateAdminLog(string adminId, string userId, string forRole)
        {
            return new AdminLog
            {
                Activity = AdminActivityType.AddedToRole,
                AdminId = adminId,
                CreatedOn = DateTime.UtcNow,
                EditedUserId = userId,
                ForRole = forRole
            };
        }
    }
}
