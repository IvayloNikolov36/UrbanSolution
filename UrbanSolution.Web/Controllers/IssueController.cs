
namespace UrbanSolution.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Services.Models;

    public class IssueController : Controller
    {
        private readonly IIssueService issues;

        public IssueController(IIssueService issues)
        {
            this.issues = issues;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var modelIssues = await this.issues.AllAsync(isApproved: true, page: page);

            var model = await this.GetModelForListingIssuesAsync(modelIssues, page);

            return this.View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var issueModel = await this.issues.GetAsync<UrbanIssueDetailsServiceModel>(id);

            if (issueModel == null)
            {
                return this.BadRequest();
            }

            return this.View(issueModel);
        }

        private async Task<IssuesListingViewModel> GetModelForListingIssuesAsync(IEnumerable<UrbanIssuesListingServiceModel> modelIssues, int page)
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
