namespace UrbanSolution.Web.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Services;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Web.Models.Issues;
    using static UrbanSolutionUtilities.WebConstants;

    [Authorize]
    public class IssueController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IIssueService issues;

        public IssueController(IIssueService issues, UserManager<User> userManager)
        {
            this.issues = issues;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new IssuesIndexModel();

            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] IssuesSortAndFilterModel model)
        {
            (int pagesCount, var modelIssues) = await this.issues
                .AllAsync<IssuesListingModel>(
                isApproved: true, model.RowsCount, model.ToPage, model.RegionFilter, model.TypeFilter, model.SortType);

            var partialModel = new IssuesListingViewModel
            {
                Issues = modelIssues,
                PagesCount = pagesCount,
                Page = model.ToPage
            };

            return this.PartialView("_IssuesListingPartial", partialModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {            
            if(this.User.IsInRole(ManagerRole))
            {
                var user = await this.userManager.GetUserAsync(this.User);
                this.ViewData[ViewDataManagerRegionKey] = user.ManagedRegion?.ToString();
            }

            var issueModel = await this.issues.GetAsync<IssueDetailsModel>(id);
            if (issueModel == null)
            {
                return this.RedirectToAction("Index").WithDanger(string.Empty, NoIssueInDb);
            }

            return this.View(issueModel);
        }

        [HttpGet]
        public async Task<ActionResult> GetDetails(int id)
        {
            var issueDetails = await this.issues.GetAsync<IssueDetailsViewModel>(id);

            return this.PartialView("_IssueDetailsPartial", issueDetails);
        }

        [HttpGet]
        public IActionResult Publish()
        {
            var model = new PublishIssueViewModel();

            this.SetModelSelectListItems(model);

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Publish(PublishIssueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.SetModelSelectListItems(model);
                return this.View(model);
            }

            var userId = this.userManager.GetUserId(User);

            var issueId = await this.issues.UploadAsync(userId, model.Title, model.Description, model.PictureFile, model.IssueType, model.Region, model.Address, model.Latitude, model.Longitude);

            return this.RedirectToAction("Details", "Issue", new { area = "", id = issueId })  //TODO: remove area
                .WithSuccess(string.Empty, IssueUploaded);
        }

        [HttpGet]
        public async Task<IActionResult> InfoBoxDetails()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            RegionType? region = user.ManagedRegion;

            var data = await this.issues
                .AllMapInfoDetailsAsync<IssueMapInfoBoxModel>(areApproved: false, region: region);

            return this.Ok(data);
        }
       
        [HttpPost]
        public async Task<ActionResult<int>> UploadImage(IFormFile file)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            var picId = await this.issues.UploadIssueImageAsync(user.Id, file);

            return this.Ok(picId);
        }

        private void SetModelSelectListItems(PublishIssueViewModel model)
        {
            model.Regions = Enum.GetNames(typeof(RegionType)).Where(regionName => !regionName.EndsWith("All"))
                .Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r
                }).ToList();

            model.IssueTypes = Enum.GetNames(typeof(IssueType))
                .Select(i => new SelectListItem
                {
                    Text = (Enum.Parse<IssueType>(i)).ToFriendlyName(),
                    Value = i
                });
        }
    }
}
