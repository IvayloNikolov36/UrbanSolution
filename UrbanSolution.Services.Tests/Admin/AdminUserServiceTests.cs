﻿namespace UrbanSolution.Services.Tests.Admin
{
    using FluentAssertions;
    using Mocks;
    using Moq;
    using Seed;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Admin;
    using UrbanSolution.Services.Admin.Models;
    using Xunit;

    public class AdminUserServiceTests : BaseServiceTest
    {
        [Fact]
        public async Task AllAsyncShould_ReturnsCorrectModelWith_CorrectData()
        {
            const string ManagerRole = "Manager";
            const string AdminRole = "Administrator";
            const string BlogAuthorRole = "Blog Author";

            var admin = UserCreator.Create();
            var manager = UserCreator.Create();
            var blogger = UserCreator.Create();
            await Db.Users.AddRangeAsync(admin, manager, blogger);
            await Db.SaveChangesAsync();

            //userManager Mock and Setups
            var userManagerMock = UserManagerMock.New;

            IList<string> listAdminRole = new List<string> { AdminRole };
            userManagerMock.Setup(u => u.GetRolesAsync(admin)).Returns(Task.FromResult(listAdminRole));

            IList<string> listManagerRole = new List<string> { ManagerRole };
            userManagerMock.Setup(u => u.GetRolesAsync(manager)).Returns(Task.FromResult(listManagerRole));

            IList<string> listBloggerRole = new List<string> { BlogAuthorRole };
            userManagerMock.Setup(u => u.GetRolesAsync(blogger)).Returns(Task.FromResult(listBloggerRole));

            //add users to roles
            await userManagerMock.Object.AddToRoleAsync(admin, AdminRole);
            await userManagerMock.Object.AddToRoleAsync(manager, ManagerRole);
            await userManagerMock.Object.AddToRoleAsync(blogger, BlogAuthorRole);

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            var result = (await service.AllAsync()).ToList();
            var expectedCount = this.Db.Users.Count();

            var allRolesForUsers = new List<IList<string>>();
            foreach (var user in this.Db.Users.ToList())
            {
                var userRoles = await userManagerMock.Object.GetRolesAsync(user);
                allRolesForUsers.Add(userRoles);
            }

            //Assert
            result.Should().BeOfType<List<AdminUserListingServiceModel>>();

            result.Should().HaveCount(expectedCount);

            for (var index = 0; index < result.Count; index++)
            {
                var model = result[index];
                model.UserRoles.Count.Should().Be(allRolesForUsers[index].Count);
            }
        }

        [Fact]
        public async Task AddToRoleAsyncShould_ReturnsFalseWhen_UserIsAlreadyInRole()
        {
            const string Role = "Manager";

            //Arrange
            var admin = UserCreator.Create();
            var user = UserCreator.Create();

            await Db.Users.AddRangeAsync(admin, user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));
            userManagerMock.Setup(u => u.IsInRoleAsync(user, Role)).Returns(Task.FromResult(true));

            await userManagerMock.Object.AddToRoleAsync(user, Role);

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Asct
            var result = await service.AddToRoleAsync(admin.Id, user.Id, Role);

            //Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task AddToRoleAsyncShould_ReturnsTrueWhen_UserIsNotInRole()
        {
            const string Role = "Manager";

            //Arrange
            var admin = UserCreator.Create();
            var user = UserCreator.Create();

            await Db.Users.AddRangeAsync(admin, user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));
            userManagerMock.Setup(u => u.IsInRoleAsync(user, Role)).Returns(Task.FromResult(false));

            var activityService = new Mock<IAdminActivityService>();

            var service = new AdminUserService(Db, userManagerMock.Object, activityService.Object);

            //Act
            var result = await service.AddToRoleAsync(admin.Id, user.Id, Role);

            //Assert
            result.Should().BeTrue();

            activityService.Verify(a => 
                a.WriteInfoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), AdminActivityType.AddedToRole), Times.Once);
        }

        [Fact]
        public async Task RemoveFromRoleAsyncShould_ReturnsFalseWhen_UserIsNotInRole()
        {
            const string Role = "Manager";

            //Arrange
            var admin = UserCreator.Create();
            var user = UserCreator.Create();

            await Db.Users.AddRangeAsync(admin, user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));
            userManagerMock.Setup(u => u.IsInRoleAsync(user, Role)).Returns(Task.FromResult(false));

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            var result = await service.RemoveFromRoleAsync(admin.Id, user.Id, Role);

            //Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task RemoveFromRoleAsyncShould_ReturnsTrueWhen_UserIsInRole()
        {
            const string Role = "Manager";

            //Arrange
            var admin = UserCreator.Create();
            var user = UserCreator.Create();

            await Db.Users.AddRangeAsync(admin, user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));
            userManagerMock.Setup(u => u.IsInRoleAsync(user, Role)).Returns(Task.FromResult(true));

            await userManagerMock.Object.AddToRoleAsync(user, Role);

            var activityService = new Mock<IAdminActivityService>();

            var service = new AdminUserService(Db, userManagerMock.Object, activityService.Object);

            //Act
            var result = await service.RemoveFromRoleAsync(admin.Id, user.Id, Role);

            //Assert
            result.Should().BeTrue();

            activityService.Verify(a =>
                a.WriteInfoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), AdminActivityType.RemovedFromRole), 
                Times.Once);
        }

    }
}