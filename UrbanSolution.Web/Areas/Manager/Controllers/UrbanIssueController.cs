namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;
    using UrbanSolution.Web.Models.Areas.Manager;
    using static UrbanSolutionUtilities.WebConstants;

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

            var model = new NewIssuesIndexModel
            {
                RegionType = manager.ManagedRegion
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> New(int page = 1)
        {
            var manager = await this.UserManager.GetUserAsync(this.User);

            RegionType? region = manager.ManagedRegion;

            (int totalItems, var issues) = await this.managerIssues
                .AllAsync<IssueTableRowViewModel>(isApproved: false, region, page, takeCount: IssuesOnTablePage);

            var model = new NewIssuesTablePartialViewModel
            {
                Isssues = issues,
                Page = page,
                PagesCount = (int)Math.Ceiling((double)totalItems / IssuesOnTablePage)
            };

            return this.PartialView("_NewIssuesTablePartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var issue = await this.issues.GetAsync<IssueEditViewModel>(id);

            this.SetModelSelectListItems(issue);

            return this.ViewOrNotFound(issue);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, IssueEditViewModel model)
        {
            if (!this.ModelState.IsValid)
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
                    .WithDanger(string.Empty, CantEditIssueForAnotherRegion);
            }

            return this.RedirectToAction("Details", "Issue", new { id, Area = "" })
                .WithSuccess(string.Empty, IssueUpdateSuccess);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var manager = await this.UserManager.GetUserAsync(this.User);

            var isDeleted = await this.managerIssues.DeleteAsync(manager.Id, manager.ManagedRegion, id);
            if (!isDeleted)
            {
                return this.RedirectToAction("Index", "UrbanIssue", new { Area = "Manager" })
                    .WithDanger(string.Empty, CantDeleteIssueForAnotherRegion);
            }

            return this.RedirectToAction("Index", "UrbanIssue", new { Area = "Manager" })
                .WithSuccess(string.Empty, IssueDeleteSuccess);
        }

        public async Task<IActionResult> Approve(int id)
        {
            var manager = await this.UserManager.GetUserAsync(User);

            var isApproved = await this.managerIssues.ApproveAsync(manager, id);
            if (!isApproved)
            {
                return this.RedirectToAction(nameof(Index)).WithDanger(string.Empty, CantApproveIssueForAnotherRegion);
            }

            return this.RedirectToAction(nameof(Index)).WithSuccess(string.Empty, IssueApprovedSuccess);
        }

        private void SetModelSelectListItems(IssueEditViewModel model)
        {
            model.Regions = Enum.GetNames(typeof(RegionType))
                .Select(r => new SelectListItem
                { Text = r, Value = r })
                .ToList();

            model.IssueTypes = Enum.GetNames(typeof(IssueType))
                .Select(i => new SelectListItem
                {
                    Text = (Enum.Parse<IssueType>(i)).ToFriendlyName(),
                    Value = i
                });
        }
    }
}