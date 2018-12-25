using UrbanSolution.Models.Enums;
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

    public class ResolvedController : BaseController
    {
        private readonly IResolvedService resolvedService;
        private readonly IPictureService pictureService;
        private readonly ICloudinaryService cloudinary;

        public ResolvedController(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,            
            IManagerActivityService managerActivity,
            IResolvedService resolvedService,
            IPictureService pictureService,
            ICloudinaryService cloudinary) 
            : base(userManager, roleManager, managerActivity)
        {
            this.resolvedService = resolvedService;
            this.pictureService = pictureService;
            this.cloudinary = cloudinary;
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

            var uploadResult = await this.cloudinary.UploadFormFileAsync(model.PictureFile);

            var cloudinaryPictureUrl = this.cloudinary.GetImageUrl(uploadResult.PublicId);

            var cloudinaryThumbnailPictureUrl = this.cloudinary.GetImageThumbnailUrl(uploadResult.PublicId);
           
            var managerId = this.UserManager.GetUserId(User);

            var pictureId = await this.pictureService.WritePictureInfo(managerId, cloudinaryPictureUrl, cloudinaryThumbnailPictureUrl, uploadResult.PublicId, uploadResult.CreatedAt, uploadResult.Length);

            var resolvedId = await this.resolvedService
                .UploadAsync(managerId, model.UrbanIssueId, pictureId, model.Description);

            await this.ManagerActivity.WriteManagerLogInfoAsync(managerId, ManagerActivityType.UploadedResolved);

            this.TempData.AddSuccessMessage(WebConstants.ResolvedUploaded);

            return this.RedirectToAction("Details", "Resolved", new { resolvedId, area="" });
        }
 
    }
}