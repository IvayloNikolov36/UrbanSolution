namespace UrbanSolution.Services.Tests.Admin
{
    using Data;
    using FluentAssertions;
    using Seed;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Admin;
    using UrbanSolution.Services.Admin.Models;
    using Xunit;

    public class AdminActivityServiceTests : BaseServiceTest
    {
        private const string ManagerRole = "Manager";
        private const string BloggerRole = "Blog Author";

        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectModelWith_CorrectData()
        {
            var admin = UserCreator.Create();
            var user = UserCreator.Create();
            await this.Db.AddRangeAsync(admin, user);

            var adminLog = AdminLogCreator.Create(admin.Id, user.Id, ManagerRole);
            var secondAdminLog = AdminLogCreator.Create(admin.Id, user.Id, BloggerRole);
            await this.Db.AddRangeAsync(adminLog, secondAdminLog);

            await this.Db.SaveChangesAsync();

            var service = new AdminActivityService(this.Db);

            //Act
            var result = await service.AllAsync(admin.Id);

            var expectedCount = this.Db.AdminLogs.Count();

            var editedUsersUserNames = this.Db.AdminLogs
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
            var service = new AdminActivityService(this.Db);

            //Act
            var result = await service.WriteInfoAsync(adminId, userId, Role, activityType);
            var savedLog = this.Db.AdminLogs.First(al => al.Id == result);

            //Assert
            result.Should().BeOfType(typeof(int));

            savedLog.Id.Should().Be(result);
            savedLog.AdminId.Should().Be(adminId);
            savedLog.EditedUserId.Should().Be(userId);
            savedLog.Activity.Should().Be(activityType);
            savedLog.ForRole.Should().Match(Role);
        }

    }
}
