namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    using UrbanSolution.Web.Infrastructure.Enums;
    using UrbanSolution.Web.Infrastructure.Extensions;
    using UrbanSolution.Web.Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UrbanSolution.Web.Areas.Admin.Models;
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Admin;
    using UrbanSolutionUtilities.Enums;
    using UrbanSolutionUtilities.Extensions;
    using UrbanSolution.Web.Models;
    using static UrbanSolutionUtilities.WebConstants;
    using UrbanSolution.Services.Admin.Models;

    public class UsersController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IAdminUserService users;

        public UsersController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IAdminUserService users)
            : base(userManager)
        {
            this.roleManager = roleManager;
            this.users = users;
        }

        public async Task<IActionResult> Index(SearchSortAndFilterModel model)
        {
            (int filteredUsersCount, var modelUsers) = await this.users
                .AllAsync<AdminUserListingServiceModel>(model.Page, model.SortBy, 
                    model.SortType, model.SearchType, model.SearchText, model.Filter);

            var tableDataModel = new AdminUsersListingTableModel
            {
                Users = modelUsers,
                AllRoles = this.roleManager.Roles
                .Select(r => new SelectListItem(r.Name, r.Name))
                .ToList(),
                LockDays = GetDropDownLockedDaysOptions()
            };

            var pagesModel = new PagesModel
            {
                ItemsOnPage = UsersOnPage,
                CurrentPage = model.Page,
                TotalItems = filteredUsersCount,
                SortBy = model.SortBy,
                SortType = model.SortType,
                SearchType = model.SearchType,
                SearchText = model.SearchText,
                Filter = model.Filter
            };

            var viewModel = new AdminUsersListingViewModel
            {
                TableDataModel = tableDataModel,               
                PagesModel = pagesModel,
                FilterByOptions = GetDropDownFilterUsersOptions(),
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(string userId)
        {
            User admin = await this.UserManager.GetUserAsync(this.User);
            User user = await this.UserManager.FindByIdAsync(userId);

            bool isUnlocked = await this.users.UnlockAsync(admin.Id, user);

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
            User admin = await this.UserManager.GetUserAsync(this.User);
            var user = await this.UserManager.FindByIdAsync(userId);

            bool isLocked = await this.users.LockAsync(admin.Id, user, daysToLock);

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
            User admin = await this.UserManager.GetUserAsync(this.User);
            User user = await this.UserManager.FindByIdAsync(model.UserId);

            bool isSetRole = await this.users.AddToRoleAsync(admin.Id, user.Id, model.Role);

            if (!isSetRole)
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
            User admin = await this.UserManager.GetUserAsync(this.User);
            User user = await this.UserManager.FindByIdAsync(model.UserId);

            bool isDone = await this.users.RemoveFromRoleAsync(admin.Id, user.Id, model.Role);

            if (!isDone)
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
