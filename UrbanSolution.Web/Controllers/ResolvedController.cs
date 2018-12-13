using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IIssueService = UrbanSolution.Services.IIssueService;
using IResolvedService = UrbanSolution.Services.IResolvedService;

namespace UrbanSolution.Web.Controllers
{
    public class ResolvedController : Controller
    {
        private readonly IResolvedService resolvedService;
        private readonly IIssueService issueService;

        public ResolvedController(IResolvedService resolvedService, IIssueService issueService)
        {
            this.resolvedService = resolvedService;
            this.issueService = issueService;
        }

        public async Task<IActionResult> Details(int id)
        {            
            var detailsModel = await this.resolvedService.GetAsync(id);

            return this.View(detailsModel);
        }
    }
}
