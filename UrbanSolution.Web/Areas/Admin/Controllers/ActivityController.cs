using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Models;
using UrbanSolution.Services.Admin;
using UrbanSolution.Services.Admin.Models;
using UrbanSolution.Web.Areas.Admin.Models;

namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    public class ActivityController : BaseController
    {
        private readonly IAdminActivityService activities;

        public ActivityController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IAdminActivityService activities) 
            : base(userManager, roleManager)
        {
            this.activities = activities;
        }

        public async Task<IActionResult> Index()
        {
            var admin = await this.UserManager.GetUserAsync(this.User);

            var model = new AdminActivityIndexModel
            {
                Activities =  await this.activities.AllAsync(adminId: admin.Id),
                AdminUserName = (await this.UserManager.GetUserAsync(this.User)).UserName
            };

            return this.View(model);
        }

    }
}
