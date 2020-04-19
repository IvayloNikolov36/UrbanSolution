namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Admin;
    using UrbanSolution.Services.Admin.Models;

    public class ActivityController : BaseController
    {
        private readonly IAdminActivityService activities;

        public ActivityController(UserManager<User> userManager,
            IAdminActivityService activities) 
            : base(userManager)
        {
            this.activities = activities;
        }

        public async Task<IActionResult> Index()
        {
            var admin = await this.UserManager.GetUserAsync(this.User);

            var model = new AdminActivityIndexModel
            {
                Activities =  await this.activities
                    .AllAsync<AdminActivitiesListingServiceModel>(admin.Id),
                AdminUserName = (await this.UserManager.GetUserAsync(this.User)).UserName
            };

            return this.View(model);
        }

    }
}
