﻿using UrbanSolution.Services;

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
            IResolvedService resolvedService,
            IPictureService pictureService,
            ICloudinaryService cloudinary) 
            : base(userManager, roleManager)
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
           
            var managerId = this.UserManager.GetUserId(User);

            var resolvedId = await this.resolvedService
                .UploadAsync(managerId, model.UrbanIssueId, model.PictureFile, model.Description);

            this.TempData.AddSuccessMessage(WebConstants.ResolvedUploaded);

            return this.RedirectToAction("Details", "Resolved", new { resolvedId, area="" });
        }
 
    }
}