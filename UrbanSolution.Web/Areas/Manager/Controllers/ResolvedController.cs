using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Models;
using UrbanSolution.Services.Manager;
using UrbanSolution.Web.Areas.Manager.Models;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;

namespace UrbanSolution.Web.Areas.Manager.Controllers
{
    public class ResolvedController : BaseController
    {
        private readonly IResolvedService resolvedService;

        public ResolvedController(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            IResolvedService resolvedService) 
            : base(userManager, roleManager)
        {
            this.resolvedService = resolvedService;
        }

        public IActionResult Upload(int id)
        {
            this.ViewData[WebConstants.ViewDataIssueId] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ResolvedIssueUploadModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }

            var userId = this.UserManager.GetUserId(this.User);

            var resolvedId = await this.resolvedService
                .UploadAsync(userId, model.UrbanIssueId, model.PictureUrl, model.Description);

            this.TempData.AddSuccessMessage(WebConstants.ResolvedUploaded);

            return this.RedirectToAction("Details", "Resolved", new { resolvedId, area="" });
        }
 
    }
}