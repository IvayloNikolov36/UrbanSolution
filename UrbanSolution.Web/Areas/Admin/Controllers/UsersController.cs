namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    using Infrastructure.Enums;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Admin;
    using UrbanSolution.Services.Admin.Models;
    using UrbanSolutionUtilities.Enums;
    using UrbanSolutionUtilities.Extensions;
    using static UrbanSolutionUtilities.WebConstants;

    public class UsersController : BaseController
    {
        private readonly IAdminUserService users;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IAdminUserService users)
            : base(userManager, roleManager)
        {
            this.users = users;
        }

        public async Task<IActionResult> Index(SearchSortAndFilterModel model)
        {
            var modelUsers = await this.users.AllAsync(
                model.SortBy, model.SortType, model.SearchType, model.SearchText, model.Filter);

            this.ViewData[FilterKey] = model.Filter;
            this.ViewData[UsersSortByKey] = model.SortBy;
            this.ViewData[UsersSortTypeKey] = model.SortType;
            this.ViewData[UsersSearchTypeKey] = model.SearchType;
            this.ViewData[UsersSearchTextKey] = model.SearchText;

            var viewModel = new AdminUsersListingViewModel
            {
                Users = modelUsers,
                AllRoles = this.RoleManager.Roles.Select(r => new SelectListItem(r.Name, r.Name)).ToList(),
                LockDays = GetDropDownLockedDaysOptions(),
                FilterBy = GetDropDownFilterUsersOptions()
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(string userId)
        {
            var user = await this.UserManager.FindByIdAsync(userId);

            bool isUnlocked = await this.users.UnlockAsync(userId);

            if (!isUnlocked)
            {
                return this.RedirectToAction(nameof(Index))
                    .WithDanger(string.Empty, string.Format(UserIsNotUnlocked, user.UserName));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess(string.Empty, string.Format(UserUnlocked, user.UserName));
        }

        [HttpPost]
        public async Task<IActionResult> Lock(string userId, string LockDays)
        {
            bool isParsed = int.TryParse(LockDays, out int daysToLock);
            if (!isParsed)
            {
                this.RedirectToAction(nameof(Index))
                    .WithDanger(string.Empty, LockDaysNotValid);
            }

            var user = await this.UserManager.FindByIdAsync(userId);

            bool isLocked = await this.users.LockAsync(userId, daysToLock);

            if (!isLocked)
            {
                return this.RedirectToAction(nameof(Index))
                    .WithDanger(string.Empty, string.Format(UserIsNotLocked, user.UserName));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess(string.Empty, string.Format(UserLocked, user.UserName, $"{LockDays} days."));
        }

        [HttpPost]
        [RedirectIfModelStateIsInvalid]
        [ServiceFilter(typeof(ValidateUserAndRoleExistsAttribute))]
        public async Task<IActionResult> AddToRole(UserToRoleFormModel model)
        {
            var admin = await this.UserManager.GetUserAsync(this.User);

            var user = await this.UserManager.FindByIdAsync(model.UserId);

            var isAdded = await this.users.AddToRoleAsync(admin.Id, user.Id, model.Role);

            if (!isAdded)
            {
                return this.RedirectToAction(nameof(Index))
                    .WithWarning(string.Empty, string.Format(UserAlreadyInRole, user.UserName, model.Role));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess(string.Empty, string.Format(UserAddedToRoleSuccess, user.UserName, model.Role));
        }

        [HttpPost]
        [RedirectIfModelStateIsInvalid]
        [ServiceFilter(typeof(ValidateUserAndRoleExistsAttribute))]
        public async Task<IActionResult> RemoveFromRole(UserToRoleFormModel model)
        {
            var admin = await this.UserManager.GetUserAsync(this.User);

            var user = await this.UserManager.FindByIdAsync(model.UserId);

            var isDeleted = await this.users.RemoveFromRoleAsync(admin.Id, user.Id, model.Role);

            if (!isDeleted)
            {
                return this.RedirectToAction(nameof(Index))
                    .WithWarning(string.Empty, string.Format(UserIsNotSetToRole, user.UserName, model.Role));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess(string.Empty, string.Format(UserRemovedFromRoleSuccess, user.UserName, model.Role));
        }

        private IEnumerable<SelectListItem> GetDropDownLockedDaysOptions()
        {
            var lockDays = new List<SelectListItem>();

            foreach (var ld in (int[])Enum.GetValues(typeof(LockDays)))
                lockDays.Add(new SelectListItem(ld.ToString(), ld.ToString()));

            return lockDays;
        }

        private IDictionary<string, string> GetDropDownFilterUsersOptions()
        {
            var filterBy = new Dictionary<string, string>();

            foreach (string filter in Enum.GetNames(typeof(FilterUsersBy)))
            {
                if (!filterBy.ContainsKey(filter))
                {
                    filterBy.Add(filter, filter.SeparateStringByCapitals());
                }
            }

            return filterBy;
        }
    }
}
