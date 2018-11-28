using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Services;

namespace UrbanSolution.Web.Controllers
{
    public class IssueController : Controller
    {
        private readonly IIssueService issues;

        public IssueController(IIssueService issues)
        {
            this.issues = issues;
        }

        public async Task<IActionResult> Index()
        {
            var model = await this.issues.AllAsync(isApproved: true);

            return this.View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var issueModel = await this.issues.DetailsAsync(id);
            if (issueModel == null)
            {
                return this.BadRequest();
            }

            return this.View(issueModel);
        }
    }
}
