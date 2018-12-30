namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;

    public class ActivityController : BaseController
    {
        private readonly IManagerActivityService activity;

        public ActivityController(UserManager<User> userManager, IManagerActivityService activity) 
            : base(userManager)
        {
            this.activity = activity;
        }

        public async Task<IActionResult> Index()
        {
            var managerId = this.UserManager.GetUserId(this.User);

            var managerActivity = await this.activity.AllAsync(managerId);

            return this.View(managerActivity);
        }

        [Route("All")]
        [ServiceFilter(typeof(ValidateManagerIsMainManagerAttribute))]
        public async Task<IActionResult> AllManagersActivity()
        {                   
            var managerActivity = await this.activity.AllAsync();

            return this.View(managerActivity);
        }
    }
}
