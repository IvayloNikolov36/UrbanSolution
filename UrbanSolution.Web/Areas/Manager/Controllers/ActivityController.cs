

namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;
    using static Infrastructure.WebConstants;

    public class ActivityController : BaseController
    {
        private readonly IManagerActivityService activity;

        public ActivityController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IManagerActivityService activity) 
            : base(userManager, roleManager)
        {
            this.activity = activity;
        }

        public async Task<IActionResult> Index()
        {
            var managerId = this.UserManager.GetUserId(this.User);

            var managerActivity = await this.activity.AllAsync(managerId);

            return this.View(managerActivity);
        }

        public async Task<IActionResult> AllManagersActivity()
        {       
            //TODO: make it filter
            var username = this.UserManager.GetUserName(this.User);

            if (username != ManagerUserName)
            {
                return this.RedirectToAction("Index", "Home").WithDanger(NotAuthorized, CantViewManagersActivity);
            }
            
            var managerActivity = await this.activity.AllAsync();

            return this.View(managerActivity);
        }
    }
}
