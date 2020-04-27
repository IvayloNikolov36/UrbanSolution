namespace UrbanSolution.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Admin;
    using UrbanSolution.Services.Admin.Models;
    using UrbanSolution.Web.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class ActivityController : BaseController
    {
        private readonly IAdminActivityService activities;

        public ActivityController(UserManager<User> userManager,
            IAdminActivityService activities) 
            : base(userManager)
        {
            this.activities = activities;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var admin = await this.UserManager.GetUserAsync(this.User);

            (int count, var activities) = await this.activities
                    .AllAsync<AdminActivitiesListingServiceModel>(admin.Id, page);

            var model = new AdminActivityIndexModel
            {
                Activities = activities,
                AdminUserName = admin.UserName,
                PagesModel = new PagesModel 
                { 
                    CurrentPage = page,
                    TotalItems = count,
                    ItemsOnPage = ActivityRowsOnPage
                }
            };

            return this.View(model);
        }

    }
}
