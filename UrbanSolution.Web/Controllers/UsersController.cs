namespace UrbanSolution.Web.Controllers
{
    using Infrastructure;
    using Infrastructure.Extensions;
    using Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UrbanSolution.Models;
    using Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

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

            var issueId = await this.issues.UploadAsync(userId, model.Title, model.Description, model.PictureFile, model.IssueType,model.Region, model.Address, model.Latitude, model.Longitude);

            return this.RedirectToAction("Details", "Issue", new { area = "", id = issueId })
                .WithSuccess("", WebConstants.IssueUploaded);
        }

        private void SetModelSelectListItems(PublishIssueViewModel model)
        {
            model.Regions = Enum.GetNames(typeof(RegionType))
                .Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r
                }).ToList();

            model.IssueTypes = Enum.GetNames(typeof(IssueType))
                .Select(i => new SelectListItem
                {
                    Text = (Enum.Parse<IssueType>(i)).ToFriendlyName(),
                    Value = i
                });
        }
    }

}
