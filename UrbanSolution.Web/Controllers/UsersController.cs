using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using UrbanSolution.Models;
using UrbanSolution.Services;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;
using UrbanSolution.Web.Models;


namespace UrbanSolution.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserIssuesService issues;
        private readonly UserManager<User> userManager;
        private readonly ICloudinaryService cloudinary;
        private readonly IFileService fileService;
        private readonly IPictureService pictureService;

        public UsersController(IUserIssuesService issues, UserManager<User> userManager, ICloudinaryService cloudinary, IFileService fileService, IPictureService pictureService)
        {
            this.issues = issues;
            this.userManager = userManager;
            this.cloudinary = cloudinary;
            this.fileService = fileService;
            this.pictureService = pictureService;
        }

        public IActionResult PublishIssue()
        {
            var model = new PublishIssueViewModel();

            this.SetModelSelectListItems(model);    

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PublishIssue(PublishIssueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.SetModelSelectListItems(model);
                return this.View(model);
            }

            var fileName = await this.fileService.UploadFileToServerAsync(model.PictureFile);

            var uploadResult = await this.cloudinary.UploadImageAsync(fileName);

            var cloudinaryPictureUrl = this.cloudinary.GetImageUrl(uploadResult.PublicId);

            var cloudinaryThumbnailPictureUrl = this.cloudinary.GetImageThumbnailUrl(uploadResult.PublicId);

            this.fileService.DeleteFileFromServer(fileName);

            var userId = this.userManager.GetUserId(User);

            var pictureId = await this.pictureService.WritePictureInfo(userId, cloudinaryPictureUrl, cloudinaryThumbnailPictureUrl,uploadResult.PublicId, uploadResult.CreatedAt, uploadResult.Length);

            await this.issues.UploadAsync(
                userId, 
                model.Name, 
                model.Description,
                pictureId, 
                model.IssueType, 
                model.Region, 
                model.Address,
                double.Parse(model.Latitude.Trim(), CultureInfo.InvariantCulture),
                double.Parse(model.Longitude.Trim(), CultureInfo.InvariantCulture));

            this.TempData.AddSuccessMessage(WebConstants.IssueUploaded);

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void SetModelSelectListItems(PublishIssueViewModel model)
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
