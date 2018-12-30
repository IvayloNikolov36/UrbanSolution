namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using UrbanSolution.Models;

    [Area(WebConstants.ManagerArea)]
    [Authorize(Roles = WebConstants.ManagerRole)]
    public class BaseController : Controller
    {
        protected BaseController(UserManager<User> userManager)
        {
            this.UserManager = userManager;
        }

        protected UserManager<User> UserManager { get; }

    }
}