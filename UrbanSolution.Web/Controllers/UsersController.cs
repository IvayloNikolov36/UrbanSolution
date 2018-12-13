using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrbanSolution.Models;
using UrbanSolution.Services;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;
using UrbanSolution.Web.Models;


namespace UrbanSolution.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserIssuesService issues;
        private readonly UserManager<User> userManager;

        public UsersController(IUserIssuesService issues, UserManager<User> userManager)
        {
            this.issues = issues;
            this.userManager = userManager;
        }

        public IActionResult PublishIssue()
        {
            var model = new PublishIssueViewModel();
            this.SetModelSelectListItems(model);    

            return this.View(model);
        }


        [HttpPost]
        public async Task<IActionResult> PublishIssue(PublishIssueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.SetModelSelectListItems(model);
                return this.View(model);
            }

            var userId = this.userManager.GetUserId(User);


            var latitude = double.Parse(model.Latitude.Trim(), CultureInfo.InvariantCulture);
            var longitude = double.Parse(model.Longitude.Trim(), CultureInfo.InvariantCulture);
            
            await this.issues.UploadAsync(userId, model.Name, model.Description, model.PictureUrl, 
                model.IssueType, model.Region, model.Address, latitude, longitude);

            this.TempData.AddSuccessMessage(WebConstants.IssueUploaded);

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void SetModelSelectListItems(PublishIssueViewModel model)
        {
            model.Regions = Enum.
                GetNames(typeof(RegionType))
                .Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r
                }).ToList();

            model.IssueTypes = Enum
                .GetNames(typeof(IssueType))
                .Select(i => new SelectListItem
                {
                    Text = (Enum.Parse<IssueType>(i)).ToFriendlyName(),
                    Value = i
                });
        }
    }

}
