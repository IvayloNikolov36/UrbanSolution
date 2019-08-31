using UrbanSolutionUtilities;
using UrbanSolutionUtilities.Enums;
using UrbanSolutionUtilities.Extensions;

namespace UrbanSolution.Services.Tests.Admin
{
    using FluentAssertions;
    using Mocks;
    using Moq;
    using Seed;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Admin;
    using UrbanSolution.Services.Admin.Models;
    using Xunit;
    using System.Linq.Expressions;
    using static UrbanSolutionUtilities.WebConstants;

    public class AdminUserServiceTests : BaseServiceTest
    {
        private const int LockDays = 10;

        private const string BlogAuthorRole = "Blog Author";
        const string ManagerRole = "Manager";
        const string AdminRole = "Administrator";

        private const string UserUserName = "Stamat123";
        private const string UserEmail = "stamat.stamatov@abv.bg";
        private const string SearchUserForNoResult = "Ztamat";
        private const string SearchUser = "Stamat";
        private const string SearchEmail = "@gmail.com";


        [Fact]
        public async Task AllFilterAsyncShould_ReturnsCorrectModelWith_FilteredData()
        {
            const string BlogAuthorRole = "Blog Author";

            const string userName = "Stamat123";
            const string email = "stamat.stamatov@abv.bg";

            const string searchUser = "stamat";
            const string searchEmail = "@abv.bg";

            var createdUser = UserCreator.Create(userName, email);

            await Db.Users.AddAsync(createdUser);
            await Db.SaveChangesAsync();

            //userManager Mock and Setups
            var userManagerMock = UserManagerMock.New;

            IList<string> listUserRoles = new List<string> { BlogAuthorRole };
            userManagerMock.Setup(u => u.GetRolesAsync(createdUser)).Returns(Task.FromResult(listUserRoles));

            //add users to roles
            await userManagerMock.Object.AddToRoleAsync(createdUser, BlogAuthorRole);


            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            StringComparison stringComp = StringComparison.InvariantCultureIgnoreCase;

            //for username
            Expression<Func<User, bool>> filterUsername = u => u.UserName.Contains(searchUser, stringComp);

            var resultFilterUsername = (await service.AllFilterAsync(filterUsername)).ToList();

            var expectedCountFilterUsername = this.Db.Users.Count(u => u.UserName.Contains(searchUser, stringComp));
            List<string> expectedUsernames = this.Db.Users.Where(filterUsername).Select(u => u.UserName).ToList();

            //for email
            Expression<Func<User, bool>> filterEmail = u => u.Email.Contains(searchEmail, stringComp);

            var resultFilterEmail = (await service.AllFilterAsync(filterEmail)).ToList();

            var expectedCountFilterEmail = this.Db.Users.Count(u => u.Email.Contains(searchEmail, stringComp));
            List<string> expectedEmails = this.Db.Users.Where(filterEmail).Select(u => u.Email).ToList();

            //Assert
            //for username
            resultFilterUsername.Should().BeOfType<List<AdminUserListingServiceModel>>();
            resultFilterUsername.Should().HaveCount(expectedCountFilterUsername);
            Assert.Contains(resultFilterUsername.SelectMany(u => u.UserName), expectedUsernames);

            //for email
            resultFilterEmail.Should().BeOfType<List<AdminUserListingServiceModel>>();
            resultFilterEmail.Should().HaveCount(expectedCountFilterEmail);
            Assert.Contains(resultFilterEmail.SelectMany(u => u.Email), expectedEmails);
        }

        [Fact]
        public async Task AllFilterAsyncShould_ReturnsEmptyCollectionWhen_NoSearchMatching()
        {
            var createdUser = UserCreator.Create(UserUserName, UserEmail);

            await Db.Users.AddAsync(createdUser);
            await Db.SaveChangesAsync();

            //userManager Mock and Setups
            var userManagerMock = UserManagerMock.New;

            IList<string> listUserRoles = new List<string> { BlogAuthorRole };
            userManagerMock.Setup(u => u.GetRolesAsync(createdUser)).Returns(Task.FromResult(listUserRoles));

            //add users to roles
            await userManagerMock.Object.AddToRoleAsync(createdUser, BlogAuthorRole);

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            StringComparison stringComp = StringComparison.InvariantCultureIgnoreCase;

            //for username
            Expression<Func<User, bool>> filterUsername = u => u.UserName.Contains(SearchUserForNoResult, stringComp);

            var resultFilterUsername = (await service.AllFilterAsync(filterUsername)).ToList();
            List<string> allUserNames = this.Db.Users.Select(u => u.UserName).ToList();

            //for email
            Expression<Func<User, bool>> filterEmail = u => u.Email.Contains(SearchEmail, stringComp);

            var resultFilterEmail = (await service.AllFilterAsync(filterEmail)).ToList();
            List<string> allEmails = this.Db.Users.Select(u => u.Email).ToList();

            //Assert
            //for username
            resultFilterUsername.Should().BeOfType<List<AdminUserListingServiceModel>>();
            resultFilterUsername.Should().HaveCount(0);
            Assert.DoesNotContain(resultFilterUsername.SelectMany(u => u.UserName), allUserNames);

            //for email
            resultFilterEmail.Should().BeOfType<List<AdminUserListingServiceModel>>();
            resultFilterEmail.Should().HaveCount(0);
            Assert.DoesNotContain(resultFilterEmail.SelectMany(u => u.Email), allEmails);
        }

        [Theory]
        [InlineData(true, UserNameProp, SortAsc)]
        [InlineData(true, UserNameProp, SortDesc)]
        [InlineData(true, EmailProp, SortAsc)]
        [InlineData(true, EmailProp, SortDesc)]
        [InlineData(false, null, null)]
        public async Task AllWhereAsyncShould_SortInCorrectOrder(bool hasSorting, string orderBy, string orderType)
        {
            const string SecondUserName = "Ani";
            const string SecondEmail = "ani@gmail.com";
            
            var firstUser = UserCreator.Create(UserUserName, UserEmail);
            var secondUser = UserCreator.Create(SecondUserName, SecondEmail);

            await Db.Users.AddRangeAsync(firstUser, secondUser);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;

            IList<string> listUserRoles = new List<string> { BlogAuthorRole };
            userManagerMock.Setup(u => u.GetRolesAsync(firstUser)).Returns(Task.FromResult(listUserRoles));

            IList<string> secondUserRoles = new List<string> {ManagerRole, EventCreatorRole};
            userManagerMock.Setup(u => u.GetRolesAsync(secondUser)).Returns(Task.FromResult(secondUserRoles));

            //add users to roles
            await userManagerMock.Object.AddToRoleAsync(firstUser, BlogAuthorRole);
            await userManagerMock.Object.AddToRoleAsync(secondUser, BlogAuthorRole);

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            var result = (await service.AllWhereAsync(hasSorting, orderBy, orderType)).ToList();

            //Assert

            if (orderBy != null && orderBy == UserNameProp)
            {
                if (orderType == SortAsc)
                    result.Should().BeInAscendingOrder(u => u.UserName);

                if (orderType == SortDesc)
                    result.Should().BeInDescendingOrder(u => u.UserName);

            }

            if (orderBy != null && orderBy == EmailProp)
            {
                if (orderType == SortAsc)
                    result.Should().BeInAscendingOrder(u => u.Email);

                if (orderType == SortDesc)
                    result.Should().BeInDescendingOrder(u => u.Email);
            }

            if (!hasSorting)
            {
                result.Should().BeInAscendingOrder(u => u.UserName);
            }
        }

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

            var service = new AdminUserService(Db, userManagerMock.Object, null);

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

        [Fact]
        public async Task UnlockAsyncShould_SetLockEndPropToNullWhen_ThereIsUserWithNotNullLockEndProp()
        {
            string userId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(userId, LockDays);
            await Db.Users.AddAsync(user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            bool expected = true;
            bool actual = await service.UnlockAsync(userId);

            //Assert
            Assert.Equal(expected, actual); //expected, actual
            Assert.True(user.LockoutEnd == null);
        }

        [Fact]
        public async Task UnlockAsyncShould_NotMakeChangesWhen_UserIsNotLocked()
        {
            string userId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(userId, lockDaysFor: null);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            bool expected = false;
            bool actual = await service.UnlockAsync(userId);

            //Assert
            Assert.Equal(expected, actual); //expected, actual
            Assert.True(user.LockoutEnd == null);
        }

        [Fact]
        public async Task UnlockAsyncShould_NotMakeChangesWhen_UserIdIsNotCorrect() 
        {
            string createdUserId = Guid.NewGuid().ToString();
            string unlockingUserId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(createdUserId, lockDaysFor: LockDays);

            await Db.Users.AddAsync(user);
            await Db.SaveChangesAsync();

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            bool expected = false;
            bool actual = await service.UnlockAsync(unlockingUserId);

            //Assert
            Assert.Equal(expected, actual); //expected, actual
        }

        //LockAsync

        [Fact]
        public async Task LockAsyncShould_SetLockEndPropWhen_UserIsNotLockedAnd_CorrectLockEndValueIsPassed()
        {
            string userId = Guid.NewGuid().ToString();

            //Arrange
            var user = this.CreateUser(userId, lockDaysFor: null);

            await Db.Users.AddAsync(user);
            await Db.SaveChangesAsync();

            DateTime timeThatLockEnds = DateTime.UtcNow.AddDays(LockDays);

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            bool expected = true;
            bool actual = await service.LockAsync(userId, LockDays);

            var expectedTime = timeThatLockEnds.Date;
            var actualTime = user.LockoutEnd.Value.UtcDateTime.Date;

            //Assert
            Assert.Equal(expected, actual); //expected, actual
            Assert.Equal(expectedTime, actualTime);
        }

        [Fact]
        public async Task LockAsyncShould_ReturnsFalseWhen_UserIdIsNotCorrect()
        {

            //Arrange
            string createdUserId = Guid.NewGuid().ToString();
            string lockingUserId = Guid.NewGuid().ToString();

            var user = this.CreateUser(createdUserId, lockDaysFor: null);

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            bool expected = false;
            bool actual = await service.LockAsync(lockingUserId, LockDays);

            //Assert
            Assert.Equal(expected, actual); //expected, actual
            Assert.True(user.LockoutEnd == null);
        }

        [Fact]
        public async Task LockAsyncShould_NotLockUserWhen_UserIsAlreadyLocked()
        {
            //Arrange
            string createdUserId = Guid.NewGuid().ToString();

            var user = this.CreateUser(createdUserId, lockDaysFor: LockDays);

            var userManagerMock = UserManagerMock.New;
            userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).Returns(Task.FromResult(user));

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            bool expected = false;
            bool actual = await service.LockAsync(createdUserId, LockDays);

            //Assert
            Assert.Equal(expected, actual); //expected, actual
        }


        [Theory]
        [InlineData(UserNameProp, SortDesc, UserNameProp, SearchUser, UserIsNotLocked)]
        [InlineData(UserNameProp, SortAsc, UserNameProp, SearchUser, UserLocked)]
        [InlineData(UserNameProp, SortDesc, EmailProp, "@gmail.com", UserIsNotLocked)]
        [InlineData(UserNameProp, SortAsc, EmailProp, "@abv.bg", UserLocked)]
        [InlineData(EmailProp, SortDesc, EmailProp, SearchUser, UserIsNotLocked)]
        [InlineData(EmailProp, SortDesc, EmailProp, SearchUser, UserLocked)]
        public async Task AllAsyncShould_ReturnsCorrectModelWith_CorrectData(string sortBy, string sortType, string searchType, string searchText, string filter)
        {
            bool hasSearching = !string.IsNullOrEmpty(searchText);
            bool hasFiltering = filter != null && !string.IsNullOrEmpty(filter) && !filter.Equals(NoFilter);
            bool hasSorting = sortBy != null && sortBy != SortBy;

            var manager = UserCreator.Create(UserUserName, UserEmail); //Stamat
            var blogger = UserCreator.Create("Gosho", "gosho@abv.bg");
            await Db.Users.AddRangeAsync(manager, blogger);
            await Db.SaveChangesAsync();


            //userManager Mock and Setups
            var userManagerMock = UserManagerMock.New;

            IList<string> listManagerRole = new List<string> { ManagerRole };
            userManagerMock.Setup(u => u.GetRolesAsync(manager)).Returns(Task.FromResult(listManagerRole));

            IList<string> listBloggerRole = new List<string> { BlogAuthorRole };
            userManagerMock.Setup(u => u.GetRolesAsync(blogger)).Returns(Task.FromResult(listBloggerRole));

            //add users to roles

            await userManagerMock.Object.AddToRoleAsync(manager, ManagerRole);
            await userManagerMock.Object.AddToRoleAsync(blogger, BlogAuthorRole);

            var service = new AdminUserService(Db, userManagerMock.Object, null);

            //Act
            List<AdminUserListingServiceModel> result = null;

            if (hasSorting)
            {
                result = (await service.AllWhereAsync(hasSorting: true, sortBy, sortType)).ToList();
            }

            if (!hasSearching && !hasFiltering)
            {
                result = (await service.AllWhereAsync()).ToList();
            }

            Expression<Func<User, bool>> exprFiltering = null;

            if (hasSearching)
            {
                if (searchType == UsersFilters.UserName.ToString())
                    exprFiltering = u => u.UserName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase);
                else if (searchType == UsersFilters.Email.ToString())
                    exprFiltering = u => u.Email.Contains(searchText, StringComparison.InvariantCultureIgnoreCase);
            }

            if (hasFiltering)
            {
                if (filter == FilterUsersBy.Locked.ToString())
                    exprFiltering = u => u.LockoutEnd != null;

                if (filter == FilterUsersBy.NotLocked.ToString().SeparateStringByCapitals())
                    exprFiltering = u => u.LockoutEnd == null;
            }

            var resultFiltering = (await service.AllFilterAsync(exprFiltering)).ToList();

            var expectedCountWitNoSearch = this.Db.Users.Count();

            var allRolesForUsers = new List<IList<string>>();
            foreach (var user in this.Db.Users.ToList())
            {
                var userRoles = await userManagerMock.Object.GetRolesAsync(user);
                allRolesForUsers.Add(userRoles);
            }

            //Assert
            result.Should().BeOfType<List<AdminUserListingServiceModel>>();

            if (hasSorting)
            {
                AssertResultFormAllWhereAsyncWithSorting(result, sortBy, sortType);
            }

            if (!hasSearching && !hasFiltering)
            {
                result.Should().BeInAscendingOrder(u => u.UserName);
                result.Should().HaveCount(expectedCountWitNoSearch);
            }

            //if (hasSearching)
            //{
            //    if (searchType == UserNameProp)
            //    {
            //        result.Should().OnlyContain(um => um.UserName.Contains(searchText));
            //    }

            //    if (searchType == EmailProp)
            //    {
            //        result.Should().OnlyContain(um => um.Email.Contains(searchText));
            //    }
            //}

            //if (hasFiltering)
            //{
            //    if (filter == UserLocked)
            //    {
            //        result.Should().OnlyContain(um => um.LockoutEnd != null);
            //    }

            //    if (filter == UserIsNotLocked)
            //    {
            //        result.Should().OnlyContain(um => um.LockoutEnd == null);
            //    }
            //}


            for (var index = 0; index < result?.Count; index++)
            {
                var model = result[index];
                model.UserRoles.Count.Should().Be(allRolesForUsers[index].Count);
            }
        }

        private void AssertResultFormAllWhereAsyncWithSorting(List<AdminUserListingServiceModel> result, string sortBy, string sortType)
        {
            if (sortBy == UserNameProp)
            {
                if (sortType == SortAsc)
                    result.Should().BeInAscendingOrder(u => u.UserName);

                if (sortType == SortDesc)
                    result.Should().BeInDescendingOrder(u => u.UserName);

            }

            if (sortBy == EmailProp)
            {
                if (sortType == SortAsc)
                    result.Should().BeInAscendingOrder(u => u.Email);

                if (sortType == SortDesc)
                    result.Should().BeInDescendingOrder(u => u.Email);
            }
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
