namespace UrbanSolution.Web.Controllers
{
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using System.Collections.Generic;
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

        public async Task<IActionResult> Index(int pagination, IssuesSortAndFilterModel model)
        {
            var rowsCount = model.RowsCount == 0 ? DefaultRowsCount : model.RowsCount;
            var goToPage = pagination > model.Page ? pagination : model.Page;

            var modelIssues = await this.issues.AllAsync(isApproved: true, rowsCount, goToPage, model.RegionFilter, model.TypeFilter, model.SortType);
            var issueModel = await this.GetModelForListingIssuesAsync(modelIssues, model.Page);

            this.ViewData[RowsCountKey] = rowsCount;
            this.ViewData[PageKey] = model.Page;
            this.ViewData[SortTypeKey] = model.SortType ?? SortDesc;
            this.ViewData[RegionFilterKey] = model.RegionFilter;
            this.ViewData[TypeFilterKey] = model.TypeFilter;

            return this.View(issueModel);
        }

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

        private async Task<IssuesListingViewModel> GetModelForListingIssuesAsync(
            IEnumerable<UrbanIssuesListingServiceModel> modelIssues, int page)
        {
            var totalIssues = await this.issues.TotalAsync(isApproved: true);

            var model = new IssuesListingViewModel
            {
                Issues = modelIssues,
                TotalIssues = totalIssues,
                CurrentPage = page
            };

            return model;
        }

    }
}
