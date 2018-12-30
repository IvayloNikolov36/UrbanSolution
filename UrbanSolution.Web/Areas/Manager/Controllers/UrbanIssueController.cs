namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;
    using UrbanSolution.Services.Manager.Models;
    using UrbanSolution.Web.Models;
    using static Infrastructure.WebConstants;

    public class UrbanIssueController : BaseController
    {
        private readonly IManagerIssueService managerIssues;
        private readonly IIssueService issues;

        public UrbanIssueController(
            UserManager<User> userManager, 
            IManagerIssueService managerIssues,
            IIssueService issues) 
            : base(userManager)
        {
            this.managerIssues = managerIssues;
            this.issues = issues;
        }

        public async Task<IActionResult> Index()
        {
            var manager = await this.UserManager.GetUserAsync(this.User);

            RegionType? region = manager.ManagedRegion;

            var issuesForRegion = await this.managerIssues.AllAsync(isApproved: false, region: region);

            var model = new IssuesListingViewModel
            {
                Issues = issuesForRegion,
                Region = region,
                UseCarousel = true
            };

            return View(model);
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        public async Task<IActionResult> Edit(int id)
        {
            var issue = await this.issues.GetAsync<UrbanIssueEditServiceViewModel>(id);

            this.SetModelSelectListItems(issue);

            return this.View(issue);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UrbanIssueEditServiceViewModel model)
        {
            if (!this.ModelState.IsValid)                 //TODO: make a filter
            {
                this.SetModelSelectListItems(model);

                return this.View(model);
            }

            var manager = await this.UserManager.GetUserAsync(User);

            var isUpdated = await this.managerIssues.UpdateAsync(
                manager, model.Id, model.Title, model.Description, 
                model.Region, model.Type, model.AddressStreet, model.PictureFile);

            if (!isUpdated)
            {
                return this.RedirectToAction("Details", "Issue", new { id, Area = "" })
                    .WithDanger("", CantEditIssueForAnotherRegion);
            }

            return this.RedirectToAction("Details", "Issue", new {id, Area = ""})
                .WithSuccess("", IssueUpdateSuccess);
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        public async Task<IActionResult> Delete(int id)
        {
            var manager = await this.UserManager.GetUserAsync(this.User);

            var isDeleted = await this.managerIssues.DeleteAsync(manager, id);

            if (!isDeleted)
            {
                return this.RedirectToAction("Index", "UrbanIssue", new { Area = "Manager" })
                    .WithDanger("", CantDeleteIssueForAnotherRegion);
            }

            return this.RedirectToAction("Index", "UrbanIssue", new {Area = "Manager"})
                .WithSuccess("", IssueDeleteSuccess);
        }

        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        public async Task<IActionResult> Approve(int id)
        {
            var manager = await this.UserManager.GetUserAsync(User);

            var isApproved = await this.managerIssues.ApproveAsync(manager, id);
            if (!isApproved)
            {
                return this.RedirectToAction(nameof(Index)).WithDanger("", CantApproveIssueForAnotherRegion);
            }

            return this.RedirectToAction(nameof(Index)).WithSuccess("", IssueApprovedSuccess);
        }

        private void SetModelSelectListItems(UrbanIssueEditServiceViewModel model)
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