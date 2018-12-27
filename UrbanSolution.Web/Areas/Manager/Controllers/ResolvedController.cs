using UrbanSolution.Services;


namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    using Infrastructure;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Manager;
    using UrbanSolution.Services.Manager.Models;
    using static Infrastructure.WebConstants;

    public class ResolvedController : BaseController
    {
        private readonly IResolvedService resolvedService;
        private readonly IPictureService pictureService;

        public ResolvedController(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,            
            IResolvedService resolvedService,
            IPictureService pictureService) 
            : base(userManager, roleManager)
        {
            this.resolvedService = resolvedService;
            this.pictureService = pictureService;
        }

        public IActionResult Upload(int id)
        {
            this.ViewData[WebConstants.ViewDataIssueId] = id; //TODO: ???

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ResolvedIssueUploadModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }
           
            var managerId = this.UserManager.GetUserId(User);

            var resolvedId = await this.resolvedService
                .UploadAsync(managerId, model.UrbanIssueId, model.PictureFile, model.Description);

            return this.RedirectToAction("Details", "Resolved", new { id = resolvedId, area = "" })
                .WithSuccess("", ResolvedUploaded);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var resolvedToEdit = await this.resolvedService.GetAsync(id);

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
                return this.RedirectToAction("Details", "Resolved", new { id, area = "" })
                    .WithDanger(NotAuthorized, CantEditResolved);
            }

            return this.RedirectToAction("Details", "Resolved", new { id, area= "" })
                .WithSuccess("", ResolvedUpdated);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var managerId = this.UserManager.GetUserId(User);

            var canDelete = await this.resolvedService.DeleteAsync(managerId, id);

            if (!canDelete)
            {
                return this.RedirectToAction("Index", "UrbanIssue", new { area = "Manager" })
                    .WithDanger(NotAuthorized, CantDeleteResolved);
            }
                
            return this.RedirectToAction("Index", "UrbanIssue", new { area = "Manager" })
                    .WithSuccess("", ResolvedDeleted);
        }

    }
}