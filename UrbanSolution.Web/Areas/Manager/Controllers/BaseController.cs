using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Models;
using UrbanSolution.Services.Manager;
using UrbanSolution.Web.Infrastructure;

namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    [Area(WebConstants.ManagerArea)]
    [Authorize(Roles = WebConstants.ManagerRole)]
    public class BaseController : Controller
    {
        protected BaseController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IManagerActivityService managerActivity)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            this.ManagerActivity = managerActivity;
        }

        protected UserManager<User> UserManager { get; }

        protected RoleManager<IdentityRole> RoleManager { get; }

        protected IManagerActivityService ManagerActivity { get; }
    }
}