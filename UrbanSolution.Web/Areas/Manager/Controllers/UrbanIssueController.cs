namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;
    using UrbanSolution.Services.Manager.Models;

    using UrbanSolution.Web.Models;

    public class UrbanIssueController : BaseController
    {
        private readonly IManagerIssueService issues;

        public UrbanIssueController(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            IManagerIssueService issues) 
            : base(userManager, roleManager)
        {
            this.issues = issues;
        }

        public async Task<IActionResult> Index()
        {
            var user = await this.UserManager.GetUserAsync(this.User);
            RegionType? region = user.ManagedRegion;

            var model = new IssuesListingViewModel
            {
                Issues = await this.issues.AllAsync(isApproved: false, region: region),
                UseCarousel = true
            };

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var issue = await this.issues.GetAsync(id);

            if (issue == null)
            {
                return this.BadRequest();
            }

            this.SetModelSelectListItems(issue);
            return this.View(issue);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UrbanIssueEditServiceViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                this.SetModelSelectListItems(model);
                return this.RedirectToAction(nameof(Edit), "UrbanIssue", new {id});
            }

            await this.issues.Update(model.Id, model.Name, model.IssuePictureUrl, model.Description, model.Region, model.Type,
                model.AddressStreet, model.StreetNumber);

            this.TempData.AddSuccessMessage(WebConstants.IssueUpdateSuccess);

            return this.RedirectToAction("Details", "Issue", new {id, Area = ""});
        }

        public async Task<IActionResult> Delete(int id)
        { 
            bool exists = await this.issues.ExistsAsync(id);             //TODO: make a filter
            if (!exists)
            {
                this.TempData.AddErrorMessage(WebConstants.IssueNotFound);
                return this.NotFound();
            }

            await this.issues.Delete(id);

            this.TempData.AddSuccessMessage(WebConstants.IssueDeleteSuccess);

            return this.RedirectToAction("Index", "UrbanIssue", new {Area = "Manager"});
        }

        public async Task<IActionResult> Approve(int id)
        {
            bool exists = await this.issues.ExistsAsync(id);
            if (!exists)
            {
                this.TempData.AddErrorMessage(WebConstants.IssueNotFound);
                return this.NotFound();
            }

            await this.issues.ApproveAsync(id);
            this.TempData.AddSuccessMessage(WebConstants.IssueApprovedSuccess);

            return this.RedirectToAction(nameof(Index));
        }

        private void SetModelSelectListItems(UrbanIssueEditServiceViewModel model)  //TODO: make it Generic
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