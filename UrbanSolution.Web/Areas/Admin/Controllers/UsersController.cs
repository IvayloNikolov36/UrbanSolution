using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Services.Admin;
using UrbanSolution.Web.Areas.Admin.Models;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;
using static UrbanSolution.Web.Infrastructure.WebConstants;

namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    
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
            var usersFromDb = await this.users.AllAsync();
            
            var roles =  this.RoleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();

            var viewModel = new AdminUsersListingViewModel
            {
                Users = usersFromDb,
                AllRoles = roles
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(UserToRoleFormModel model)
        {
            var user = await this.UserManager.FindByIdAsync(model.UserId);

            if (!(await this.UserAndRoleExists(model, user)))
            {
                this.ModelState.AddModelError(String.Empty, "Invalid identity details");
            }

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            bool userAlreadyInRole = await this.UserManager.IsInRoleAsync(user, model.Role);

            if (userAlreadyInRole)
            {
                this.TempData.AddInfoMessage(string.Format(WebConstants.UserAlreadyInRole, user.UserName, model.Role));
            }
            else
            {
                await this.UserManager.AddToRoleAsync(user, model.Role);

                this.TempData.AddSuccessMessage(string.Format(WebConstants.UserAddedToRoleSuccess, user.UserName, model.Role));
            }
        
            return this.RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(UserToRoleFormModel model)
        {
            var user = await this.UserManager.FindByIdAsync(model.UserId);

            if (!(await this.UserAndRoleExists(model, user)))
            {
                this.ModelState.AddModelError(String.Empty, "Invalid identity details");
            }

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction(nameof(Index));
            }

            bool userInRole = await this.UserManager.IsInRoleAsync(user, model.Role);
            if (!userInRole)
            {
                this.TempData.AddInfoMessage(string.Format(WebConstants.UserIsNotSetToRole, user.UserName, model.Role));
            }
            else
            {
                await this.UserManager.RemoveFromRoleAsync(user, model.Role);

                this.TempData.AddSuccessMessage(string.Format(WebConstants.UserRemovedFromRoleSuccess, user.UserName, model.Role));
            }

            return this.RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserAndRoleExists(UserToRoleFormModel model, User user)
        {
            var roleExists = await this.RoleManager.RoleExistsAsync(model.Role);

            if (!roleExists)
            {
                return false;
            }
           
            var userExists = user != null;

            return userExists;
        }
    }
}
