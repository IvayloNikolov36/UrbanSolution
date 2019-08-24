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
    using static Infrastructure.WebConstants;

    public class UsersController : BaseController
    {
        private readonly IAdminUserService users;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IAdminUserService users)
            : base(userManager, roleManager)
        {
            this.users = users;
        }

        public async Task<IActionResult> Index(string searchType, string searchText)
        {
            IEnumerable<AdminUserListingServiceModel> modelUsers;

            string search = searchText;
            if (string.IsNullOrEmpty(searchText))
            {
                modelUsers = await this.users.AllAsync();
            }
            else
            {
                Expression<Func<User, bool>> expression = null;

                if (searchType == UsersFilters.UserName.ToString())
                    expression = u => u.UserName.Contains(search, StringComparison.InvariantCultureIgnoreCase);
                else if(searchType == UsersFilters.Email.ToString())
                    expression = u => u.Email.Contains(search, StringComparison.InvariantCultureIgnoreCase);

                modelUsers = await this.users.AllAsyncWhere(expression);
            }

            var allRoles = this.RoleManager.Roles
                .Select(r => new SelectListItem(r.Name, r.Name))
                .ToList();

            var searchFilters = new List<SelectListItem>
            {
                new SelectListItem(UsersFilters.UserName.ToString(), UsersFilters.UserName.ToString()),
                new SelectListItem(UsersFilters.Email.ToString(), UsersFilters.Email.ToString())
            };

            var lockDays = new List<SelectListItem>();
            foreach (var ld in (int[])Enum.GetValues(typeof(LockDays)))
            {
                lockDays.Add(new SelectListItem(ld.ToString(), ld.ToString()));
            }

            var viewModel = new AdminUsersListingViewModel
            {
                Users = modelUsers,
                AllRoles = allRoles,
                SearchFilters = searchFilters,
                LockDays = lockDays
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
                    .WithDanger("", string.Format(UserIsNotUnlocked, user.UserName));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess("", string.Format(UserUnlocked, user.UserName));
        }

        [HttpPost]
        public async Task<IActionResult> Lock(string userId, string LockDays)
        {
            bool isParsed = int.TryParse(LockDays, out int daysToLock);
            if (!isParsed)
            {
                this.RedirectToAction(nameof(Index))
                    .WithDanger("", LockDaysNotValid);
            }

            var user = await this.UserManager.FindByIdAsync(userId);

            bool islocked = await this.users.LockAsync(userId, daysToLock);

            if (!islocked)
            {
                return this.RedirectToAction(nameof(Index))
                    .WithDanger("", string.Format(UserIsNotLocked, user.UserName));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess("", string.Format(UserLocked, user.UserName, $"{LockDays} days."));
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
                    .WithWarning("", string.Format(UserAlreadyInRole, user.UserName, model.Role));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess("", string.Format(UserAddedToRoleSuccess, user.UserName, model.Role));
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
                    .WithWarning("", string.Format(UserIsNotSetToRole, user.UserName, model.Role));
            }

            return this.RedirectToAction(nameof(Index))
                .WithSuccess("", string.Format(UserRemovedFromRoleSuccess, user.UserName, model.Role));
        }

    }
}
