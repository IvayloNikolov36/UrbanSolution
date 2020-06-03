namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;
    using Microsoft.AspNetCore.Authorization;
    using System.Threading.Tasks;
    using static UrbanSolutionUtilities.WebConstants;
    using UrbanSolution.Web.Models.Areas.Manager;

    public class ResolvedController : BaseController
    {
        private readonly IResolvedService resolvedService;

        public ResolvedController(UserManager<User> userManager, IResolvedService resolvedService) 
            : base(userManager)
        {
            this.resolvedService = resolvedService;
        }

        public IActionResult Upload(int id)
        {
            this.ViewData[ViewDataIssueId] = id; // resolved issue needs urbanIssueId for reference

            return View();
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Upload(ResolvedIssueUploadModel model)
        {
            var managerId = this.UserManager.GetUserId(User);

            var resolvedId = await this.resolvedService
                .UploadAsync(managerId, model.UrbanIssueId, model.PictureFile, model.Description);

            return this.RedirectToAction("Details", "Resolved", new { id = resolvedId, area = "Manager" })
                .WithSuccess(string.Empty, ResolvedUploaded);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var resolvedToEdit = await this.resolvedService.GetAsync<ResolvedIssueEditModel>(id);

            return this.View(resolvedToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ResolvedIssueEditModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var managerId = this.UserManager.GetUserId(this.User);

            bool isCompleted = await this.resolvedService.UpdateAsync(managerId, id, model.Description, model.PictureFile);

            if (!isCompleted) // the manager is not the same manager who is published the resolved issue.
            {
                return this.RedirectToAction("Details", "Resolved", new { id, area = "Manager" })
                    .WithDanger(NotAuthorized, CantEditResolved);
            }

            return this.RedirectToAction("Details", "Resolved", new { id, area= "Manager" })
                .WithSuccess(string.Empty, ResolvedUpdated);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var managerId = this.UserManager.GetUserId(User);

            var canDelete = await this.resolvedService.DeleteAsync(managerId, id);

            if (!canDelete)
            {
                return this.RedirectToAction("Index", "Home")
                    .WithDanger(NotAuthorized, CantDeleteResolved);
            }
                
            return this.RedirectToAction("Index", "UrbanIssue", new { area = "Manager" })
                    .WithSuccess(string.Empty, ResolvedDeleted);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var detailsModel = await this.resolvedService
                .GetAsync<ResolvedDetailsModel>(id);           

            if (detailsModel == null)
            {
                return this.RedirectToAction("Index", "UrbanIssue").WithDanger(string.Empty, NoResolvedFound);
            }

            this.ViewData[ViewDataManagerId] = this.UserManager.GetUserId(this.User);

            return this.View(detailsModel);
        }
    }
}