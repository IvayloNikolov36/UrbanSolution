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
    using UrbanSolution.Services.Admin;
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

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminUsersListingViewModel
            {
                Users = await this.users.AllAsync(),
                AllRoles = this.RoleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                    .ToList()
            };

            return this.View(viewModel);
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
