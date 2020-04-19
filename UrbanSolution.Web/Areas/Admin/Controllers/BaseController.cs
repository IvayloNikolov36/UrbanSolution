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
        protected BaseController(UserManager<User> userManager)
        {
            this.UserManager = userManager;
        }

        protected  UserManager<User> UserManager { get; }

    }
}