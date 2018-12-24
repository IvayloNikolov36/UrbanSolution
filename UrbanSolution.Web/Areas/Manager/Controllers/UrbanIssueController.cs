
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
    using UrbanSolution.Models.Enums;
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
            IManagerActivityService managerActivity,
            IManagerIssueService managerIssues,
            IIssueService issues) 
            : base(userManager, roleManager, managerActivity)
        {
            this.managerIssues = managerIssues;
            this.issues = issues;
        }

        public async Task<IActionResult> Index()
        {
            var user = await this.UserManager.GetUserAsync(this.User);
            RegionType? region = user.ManagedRegion;

            var model = new IssuesListingViewModel
            {
                Issues = await this.managerIssues.AllAsync(isApproved: false, region: region),
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

            await this.managerIssues.UpdateAsync(model.Id, model.Title, model.Description, model.Region, model.Type,
                model.AddressStreet);

            var managerId = this.UserManager.GetUserId(this.User);

            await this.ManagerActivity.WriteManagerLogInfoAsync(managerId, ManagerActivityType.EditedIssue);

            this.TempData.AddSuccessMessage(WebConstants.IssueUpdateSuccess);

            return this.RedirectToAction("Details", "Issue", new {id, Area = ""});
        }

        [HttpGet]
        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        [ServiceFilter(typeof(ValidateIssueAndManagerRegionsAreaEqualAttribute))]
        public async Task<IActionResult> Delete(int id)
        {           
            await this.managerIssues.DeleteAsync(id);

            var managerId = this.UserManager.GetUserId(this.User);

            await this.ManagerActivity.WriteManagerLogInfoAsync(managerId, ManagerActivityType.DeletedIssue);

            this.TempData.AddSuccessMessage(WebConstants.IssueDeleteSuccess);

            return this.RedirectToAction("Index", "UrbanIssue", new {Area = "Manager"});
        }

        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        [ServiceFilter(typeof(ValidateIssueAndManagerRegionsAreaEqualAttribute))]
        public async Task<IActionResult> Approve(int id)
        {
            await this.managerIssues.ApproveAsync(id);

            var managerId = this.UserManager.GetUserId(this.User);

            await this.ManagerActivity.WriteManagerLogInfoAsync(managerId, ManagerActivityType.ApprovedIssue);

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