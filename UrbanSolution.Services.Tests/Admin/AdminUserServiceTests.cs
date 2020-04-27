namespace UrbanSolution.Services.Tests.Admin
{
    using FluentAssertions;
    using Mocks;
    using Moq;
    using Seed;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Admin;
    using Xunit;

    public class AdminUserServiceTests : BaseServiceTest
    {
        private const int LockDays = 10;
        const string ManagerRole = "Manager";

        [Fact]
        public async Task AddToRoleAsyncShould_ReturnsFalseWhen_UserIsAlreadyInRole()
        {
            //Arrange
            var admin = UserCreator.Create();
            var user = UserCreator.Create();

            await Db.Users.AddRangeAsync(admin, user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));
            userManagerMock.Setup(u => u.IsInRoleAsync(user, ManagerRole)).Returns(Task.FromResult(true));

            await userManagerMock.Object.AddToRoleAsync(user, ManagerRole);

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

            //Act
            var result = await service.AddToRoleAsync(admin.Id, user.Id, ManagerRole);

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

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, activityService.Object);

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

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

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

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, activityService.Object);

            //Act
            var result = await service.RemoveFromRoleAsync(admin.Id, user.Id, Role);

            //Assert
            result.Should().BeTrue();

            activityService.Verify(a =>
                a.WriteInfoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), AdminActivityType.RemovedFromRole), 
                Times.Once);
        }

        [Fact]
        public async Task UnlockAsyncShould_SetLockEndPropToNullWhen_ThereIsUserWithNotNullLockEndProp()
        {
            string userId = Guid.NewGuid().ToString();
            string adminId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(userId, LockDays);
            await Db.Users.AddAsync(user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

            //Act
            bool expected = true;
            //bool actual = await service.UnlockAsync(adminId, userId);/////

            //Assert
            //Assert.Equal(expected, actual); //expected, actual////////
            Assert.True(user.LockoutEnd == null);
        }

        [Fact]
        public async Task UnlockAsyncShould_NotMakeChangesWhen_UserIsNotLocked()
        {
            string userId = Guid.NewGuid().ToString(); 
            string adminId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(userId, lockDaysFor: null);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

            //Act
            bool expected = false;
            //bool actual = await service.UnlockAsync(adminId, userId);/////

            //Assert
            ///Assert.Equal(expected, actual); //expected, actual/////
            Assert.True(user.LockoutEnd == null);
        }

        [Fact]
        public async Task UnlockAsyncShould_NotMakeChangesWhen_UserIdIsNotCorrect() 
        {
            string createdUserId = Guid.NewGuid().ToString();
            string unlockingUserId = Guid.NewGuid().ToString();
            string adminId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(createdUserId, lockDaysFor: LockDays);

            await Db.Users.AddAsync(user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

            //Act
            bool expected = false;
            //bool actual = await service.UnlockAsync(adminId, unlockingUserId);///

            //Assert
            //Assert.Equal(expected, actual); //expected, actual///
        }

        //LockAsync

        [Fact]
        public async Task LockAsyncShould_SetLockEndPropWhen_UserIsNotLockedAnd_CorrectLockEndValueIsPassed()
        {
            string userId = Guid.NewGuid().ToString();
            string adminId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(userId, lockDaysFor: null);

            await Db.Users.AddAsync(user);
            await Db.SaveChangesAsync();

            DateTime timeThatLockEnds = DateTime.UtcNow.AddDays(LockDays);

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

            //Act
            bool expected = true;
            //bool actual = await service.LockAsync(adminId, userId, LockDays);//

            var expectedTime = timeThatLockEnds.Date;
            var actualTime = user.LockoutEnd.Value.UtcDateTime.Date;

            //Assert
            // Assert.Equal(expected, actual); //expected, actual//
            Assert.Equal(expectedTime, actualTime);
        }

        [Fact]
        public async Task LockAsyncShould_ReturnsFalseWhen_UserIdIsNotCorrect()
        {

            //Arrange
            string createdUserId = Guid.NewGuid().ToString();
            string lockingUserId = Guid.NewGuid().ToString();
            string adminId = Guid.NewGuid().ToString();

            var user = this.CreateUser(createdUserId, lockDaysFor: null);

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

            //Act
            bool expected = false;
            //bool actual = await service.LockAsync(adminId, lockingUserId, LockDays);//

            //Assert
            //Assert.Equal(expected, actual); //expected, actual
            Assert.True(user.LockoutEnd == null);
        }

        [Fact]
        public async Task LockAsyncShould_NotLockUserWhen_UserIsAlreadyLocked()
        {
            //Arrange
            string createdUserId = Guid.NewGuid().ToString();
            string adminId = Guid.NewGuid().ToString();

            var user = this.CreateUser(createdUserId, lockDaysFor: LockDays);

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            //TODO: rewrite this test (now there is no need for userManager)
            var service = new AdminUserService(Db, null);

            //Act
            bool expected = false;
            //bool actual = await service.LockAsync(adminId, createdUserId, LockDays);

            //Assert
            //Assert.Equal(expected, actual); //expected, actual//
        }


        private User CreateUser(string userId, int? lockDaysFor = null)
        {
            return new User
            {
                Id = userId,
                LockoutEnd = lockDaysFor == null ? null : new DateTimeOffset?(DateTime.UtcNow.AddDays((int)lockDaysFor))
            };

        }

    }
}
