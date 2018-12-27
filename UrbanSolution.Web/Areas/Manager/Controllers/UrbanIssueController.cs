
namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure;
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

    public class UrbanIssueController : BaseController
    {
        private readonly IManagerIssueService managerIssues;
        private readonly IIssueService issues;

        public UrbanIssueController(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            IManagerIssueService managerIssues,
            IIssueService issues) 
            : base(userManager, roleManager)
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
        [ServiceFilter(typeof(ValidateIssueAndManagerRegionsAreaEqualAttribute))]
        public async Task<IActionResult> Edit(int id)
        {
            var issue = await this.issues.GetAsync<UrbanIssueEditServiceViewModel>(id);

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

            var managerId = this.UserManager.GetUserId(this.User);

            await this.managerIssues.UpdateAsync(managerId, model.Id, model.Title, model.Description, model.Region, model.Type, model.AddressStreet);

            this.TempData.AddSuccessMessage(WebConstants.IssueUpdateSuccess);

            return this.RedirectToAction("Details", "Issue", new {id, Area = ""});
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        [ServiceFilter(typeof(ValidateIssueAndManagerRegionsAreaEqualAttribute))]
        public async Task<IActionResult> Delete(int id)
        {
            var managerId = this.UserManager.GetUserId(this.User);

            await this.managerIssues.DeleteAsync(managerId, id);

            this.TempData.AddSuccessMessage(WebConstants.IssueDeleteSuccess);

            return this.RedirectToAction("Index", "UrbanIssue", new {Area = "Manager"});
        }

        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        [ServiceFilter(typeof(ValidateIssueAndManagerRegionsAreaEqualAttribute))]
        public async Task<IActionResult> Approve(int id)
        {
            var managerId = this.UserManager.GetUserId(this.User);

            await this.managerIssues.ApproveAsync(managerId, id);

            this.TempData.AddSuccessMessage(WebConstants.IssueApprovedSuccess);

            return this.RedirectToAction(nameof(Index));
        }

        private void SetModelSelectListItems(UrbanIssueEditServiceViewModel model)
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