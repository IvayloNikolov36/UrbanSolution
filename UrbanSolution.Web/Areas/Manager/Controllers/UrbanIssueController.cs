using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrbanSolution.Models;
using UrbanSolution.Services.Manager;
using UrbanSolution.Services.Manager.Models;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;

namespace UrbanSolution.Web.Areas.Manager.Controllers
{    
    public class UrbanIssueController : BaseController
    {
        private readonly IIssueService issues;

        public UrbanIssueController(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            IIssueService issues) 
            : base(userManager, roleManager)
        {
            this.issues = issues;
        }

        public async Task<IActionResult> Index()
        {
            var notApproved = await this.issues.AllAsync(isApproved: false);

            return View(notApproved);
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