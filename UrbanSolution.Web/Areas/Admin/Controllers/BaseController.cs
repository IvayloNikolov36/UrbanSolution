namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    [Area(AdminArea)]
    [Authorize(Roles = AdminRole)]
    public abstract class BaseController : Controller
    {
        protected BaseController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
        }

        protected  UserManager<User> UserManager { get; }

        protected  RoleManager<IdentityRole> RoleManager { get; }
    }
}