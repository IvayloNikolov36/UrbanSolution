
namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;

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

            if (username != WebConstants.ManagerUserName)
            {
                return this.BadRequest();
            }
            
            var managerActivity = await this.activity.AllAsync();

            return this.View(managerActivity);
        }
    }
}
