using UrbanSolution.Services.Models;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;

namespace UrbanSolution.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using UrbanSolution.Models;
    using IIssueService = Services.IIssueService;
    using IResolvedService = Services.IResolvedService;

    public class ResolvedController : Controller
    {
        private readonly IResolvedService resolvedService;
        private readonly IIssueService issueService;
        private readonly UserManager<User> userManager;

        public ResolvedController(IResolvedService resolvedService, IIssueService issueService, UserManager<User> userManager)
        {
            this.resolvedService = resolvedService;
            this.issueService = issueService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Details(int id)
        {            
            var detailsModel = await this.resolvedService.GetAsync(id);

            if (detailsModel == null)
            {
                return this.RedirectToAction("Index", "UrbanIssue").WithDanger("", WebConstants.NoResolvedFound);
            }

            return this.View(detailsModel);
        }
    }
}
