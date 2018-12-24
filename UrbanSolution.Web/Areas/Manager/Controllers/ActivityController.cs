
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
        public ActivityController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IManagerActivityService managerActivity) 
            : base(userManager, roleManager, managerActivity)
        {            
        }

        public async Task<IActionResult> Index()
        {
            var managerId = this.UserManager.GetUserId(this.User);

            var managerActivity = await this.ManagerActivity.AllAsync(managerId);

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
            
            var managerActivity = await this.ManagerActivity.AllAsync();

            return this.View(managerActivity);
        }
    }
}
