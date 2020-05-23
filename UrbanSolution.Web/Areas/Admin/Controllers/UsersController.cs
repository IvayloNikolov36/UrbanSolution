namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    using UrbanSolution.Web.Infrastructure.Enums;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UrbanSolution.Web.Areas.Admin.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Admin;
    using UrbanSolutionUtilities.Enums;
    using UrbanSolutionUtilities.Extensions;
    using UrbanSolution.Web.Models;
    using UrbanSolution.Services.Admin.Models;
    using UrbanSolution.Web.Infrastructure.Extensions;
    using static UrbanSolutionUtilities.WebConstants;

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
        public async Task<IActionResult> Unlock([FromBody] UnlockUserInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new
                {
                    errMessage = this.ModelState.ErrorsAsString()
                });
            }

            User user = await this.UserManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return this.Json(new { errMessage = NoUserFound });
            }

            User admin = await this.UserManager.GetUserAsync(this.User);

            bool isUnlocked = await this.users.UnlockAsync(admin.Id, user);
            if (!isUnlocked)
            {
                return this.Json(new
                {
                    errMessage = string.Format(UserIsNotUnlocked, user.UserName)
                });
            }

            var partialViewModel = await CreatePartialViewModelAsync(model.UserId);

            return this.PartialView("_AdminUserTableRow", partialViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Lock([FromBody] LockUserInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new
                {
                    errMessage = this.ModelState.ErrorsAsString()
                });
            }

            User admin = await this.UserManager.GetUserAsync(this.User);
            var user = await this.UserManager.FindByIdAsync(model.UserId);

            bool isLocked = await this.users.LockAsync(admin.Id, user, model.LockDays);
            if (!isLocked)
            {
                return this.Json(new
                {
                    errMessage = string.Format(UserIsNotLocked, user.UserName)
                });
            }

            var partialViewModel = await CreatePartialViewModelAsync(model.UserId);

            return this.PartialView("_AdminUserTableRow", partialViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole([FromBody] UserToRoleFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new
                {
                    errMessage = this.ModelState.ErrorsAsString()
                });
            }

            User admin = await this.UserManager.GetUserAsync(this.User);
            User user = await this.UserManager.FindByIdAsync(model.UserId);

            bool isSetRole = await this.users.AddToRoleAsync(admin.Id, user.Id, model.Role);
            if (!isSetRole)
            {
                return this.Json(new
                {
                    errMessage = string.Format(InvalidUserOrRole, user.UserName, model.Role)
                });
            }

            var partialViewModel = await CreatePartialViewModelAsync(model.UserId);

            return this.PartialView("_AdminUserTableRow", partialViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromRole([FromBody] UserToRoleFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new
                {
                    errMessage = this.ModelState.ErrorsAsString()
                });
            }

            User admin = await this.UserManager.GetUserAsync(this.User);
            User user = await this.UserManager.FindByIdAsync(model.UserId);

            bool isDone = await this.users.RemoveFromRoleAsync(admin.Id, user.Id, model.Role);
            if (!isDone)
            {
                return this.Json(new
                {
                    errMessage = string.Format(UserNotRemovedFromRole, user.UserName, model.Role)
                });
            }

            var partialViewModel = await CreatePartialViewModelAsync(model.UserId);

            return this.PartialView("_AdminUserTableRow", partialViewModel);
        }

        private async Task<AdminUserTableRowModel> CreatePartialViewModelAsync(string userId)
        {
            return new AdminUserTableRowModel
            {
                User = await this.users
                    .SingleAsync<AdminUserListingServiceModel>(userId),
                LockDays = this.GetDropDownLockedDaysOptions()
            };
        }

        private IEnumerable<SelectListItem> GetDropDownLockedDaysOptions()
        {
            var lockDays = new List<SelectListItem>();

            foreach (int ld in Enum.GetValues(typeof(LockDays)))
                lockDays.Add(new SelectListItem(ld.ToString(), ld.ToString()));

            return lockDays;
        }

        private IDictionary<string, string> GetDropDownFilterUsersOptions()
        {
            var filterBy = new Dictionary<string, string>();

            foreach (string filter in Enum.GetNames(typeof(FilterUsersBy)))
            {
                if (!filterBy.ContainsKey(filter))
                    filterBy.Add(filter, filter.SeparateStringByCapitals());
            }

            return filterBy;
        }
    }
}
