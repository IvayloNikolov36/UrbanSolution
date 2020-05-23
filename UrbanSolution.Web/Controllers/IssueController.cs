namespace UrbanSolution.Web.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssueController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IIssueService issues;

        public IssueController(IIssueService issues, UserManager<User> userManager)
        {
            this.issues = issues;
            this.userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var model = new IssuesIndexModel();

            return this.View(model);
        }

        [Authorize]
        public async Task<IActionResult> Get([FromQuery] IssuesSortAndFilterModel model)
        {
            (int pagesCount, var modelIssues) = await this.issues
                .AllAsync<UrbanIssuesListingServiceModel>(
                isApproved: true, model.RowsCount, model.ToPage, model.RegionFilter, model.TypeFilter, model.SortType);

            var partialModel = new IssuesListingViewModel
            {
                Issues = modelIssues,
                PagesCount = pagesCount,
                Page = model.ToPage
            };

            return this.PartialView("_IssuesListingPartial", partialModel);
        }

        [Authorize]
        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        public async Task<IActionResult> Details(int id)
        {            
            if(this.User.IsInRole(ManagerRole))
            {
                var user = await this.userManager.GetUserAsync(this.User);

                this.ViewData[ViewDataManagerRegionKey] = user.ManagedRegion?.ToString();
            }

            var issueModel = await this.issues.GetAsync<UrbanIssueDetailsServiceModel>(id);

            if (issueModel == null)
            {
                return this.RedirectToAction("Index").WithDanger(string.Empty, NoIssueInDb);
            }

            return this.View(issueModel);
        }

        [Authorize]
        public async Task<ActionResult> GetDetails(int id)
        {
            var issueDetails = await this.issues.GetAsync<IssueDetailsViewModel>(id);

            return this.PartialView("_IssueDetailsPartial", issueDetails);
        }
    }
}
