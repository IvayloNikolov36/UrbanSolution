namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using UrbanSolution.Services.Admin;
    using static Infrastructure.WebConstants;

    public class UsersController : BaseController
    {
        private readonly IAdminUserService users;
        private readonly IAdminActivityService activities;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IAdminUserService users,
            IAdminActivityService activities)
            : base(userManager, roleManager)
        {
            this.users = users;
            this.activities = activities;
        }

        public async Task<IActionResult> Index()
        {
            var usersFromDb = await this.users.AllAsync();

            var roles = this.RoleManager
                .Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
                .ToList();

            var viewModel = new AdminUsersListingViewModel
            {
                Users = usersFromDb,
                AllRoles = roles
            };

            return this.View(viewModel);
        }

        [HttpPost]
        [RedirectIfModelStateIsInvalid]
        [ServiceFilter(typeof(ValidateUserAndRoleExistsAttribute))]
        public async Task<IActionResult> AddToRole(UserToRoleFormModel model)
        {
            var user = await this.UserManager.FindByIdAsync(model.UserId);

            bool userAlreadyInRole = await this.UserManager.IsInRoleAsync(user, model.Role);
            if (userAlreadyInRole)
            {
                return this.RedirectToAction(nameof(Index))
                    .WithWarning("", string.Format(UserAlreadyInRole, user.UserName, model.Role));
            }

            await this.UserManager.AddToRoleAsync(user, model.Role);

            await this.WriteAdminLogInfoAsync(AdminActivityType.AddToRole, user.Id, model.Role);

            return this.RedirectToAction(nameof(Index)).WithSuccess("", string.Format(UserAddedToRoleSuccess, user.UserName, model.Role));
        }

        [HttpPost]
        [RedirectIfModelStateIsInvalid]
        [ServiceFilter(typeof(ValidateUserAndRoleExistsAttribute))]
        public async Task<IActionResult> RemoveFromRole(UserToRoleFormModel model)
        {
            var user = await this.UserManager.FindByIdAsync(model.UserId);

            bool userInRole = await this.UserManager.IsInRoleAsync(user, model.Role);
            if (!userInRole)
            {
                return this.RedirectToAction(nameof(Index))
                    .WithWarning("", string.Format(UserIsNotSetToRole, user.UserName, model.Role));
            }

            await this.UserManager.RemoveFromRoleAsync(user, model.Role);

            await this.WriteAdminLogInfoAsync(AdminActivityType.RemoveFromRole, user.Id, model.Role);

            return this.RedirectToAction(nameof(Index)).WithSuccess("", string.Format(UserRemovedFromRoleSuccess, user.UserName, model.Role));
        }

        private async Task WriteAdminLogInfoAsync(AdminActivityType activity, string userId, string role)
        {
            var admin = await this.UserManager.GetUserAsync(this.User);

            await this.activities.WriteAdminLogInfoAsync(admin.Id, userId, role, activity);
        }

    }
}
