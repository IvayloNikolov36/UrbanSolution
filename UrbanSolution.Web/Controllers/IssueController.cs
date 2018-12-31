using UrbanSolution.Web.Infrastructure.Filters;

namespace UrbanSolution.Web.Controllers
{
    using Infrastructure;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    public class IssueController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IIssueService issues;

        public IssueController(IIssueService issues, UserManager<User> userManager)
        {
            this.issues = issues;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var modelIssues = await this.issues.AllAsync(isApproved: true, page: page);

            var model = await this.GetModelForListingIssuesAsync(modelIssues, page);

            return this.View(model);
        }

        [ServiceFilter(typeof(ValidateIssueIdExistsAttribute))]
        public async Task<IActionResult> Details(int id)
        {            
            if(this.User.IsInRole(WebConstants.ManagerRole))
            {
                var user = await this.userManager.GetUserAsync(this.User);

                this.ViewData[WebConstants.ViewDataManagerRegionKey] = user.ManagedRegion?.ToString();
            }

            var issueModel = await this.issues.GetAsync<UrbanIssueDetailsServiceModel>(id);

            if (issueModel == null)
            {
                return this.RedirectToAction("Index").WithDanger("", WebConstants.NoIssueInDb);
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
