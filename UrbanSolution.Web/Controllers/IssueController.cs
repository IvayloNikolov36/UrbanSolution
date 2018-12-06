using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Services;
using UrbanSolution.Services.Models;
using UrbanSolution.Web.Models;

namespace UrbanSolution.Web.Controllers
{
    public class IssueController : Controller
    {
        private readonly IIssueService issues;

        public IssueController(IIssueService issues)
        {
            this.issues = issues;
        }

        public async Task<IActionResult> Index(int page = 1)
        {

            var model = new IssuesListingViewModel
            {
                Issues = await this.issues.AllAsync(isApproved: true, page: page),
                TotalIssues = await this.issues.TotalAsync(isApproved: true),
                CurrentPage = page
            };

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
    }
}
