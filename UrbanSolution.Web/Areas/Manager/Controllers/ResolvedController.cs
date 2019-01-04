namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;
    using UrbanSolution.Services.Manager.Models;
    using Microsoft.AspNetCore.Authorization;
    using System.Threading.Tasks;
    using UrbanSolution.Services.Models;
    using static Infrastructure.WebConstants;
    
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
                .WithSuccess("", ResolvedUploaded);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var resolvedToEdit = await this.resolvedService.GetAsync<ResolvedIssueEditServiceModel>(id);

            return this.View(resolvedToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ResolvedIssueEditServiceModel model)
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
                .WithSuccess("", ResolvedUpdated);
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
                    .WithSuccess("", ResolvedDeleted);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var detailsModel = await this.resolvedService
                .GetAsync<ResolvedDetailsServiceModel>(id);           

            if (detailsModel == null)
            {
                return this.RedirectToAction("Index", "UrbanIssue").WithDanger("", NoResolvedFound);
            }

            this.ViewData[ViewDataManagerId] = this.UserManager.GetUserId(this.User);

            return this.View(detailsModel);
        }
    }
}