
namespace UrbanSolution.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Threading.Tasks;

    using UrbanSolution.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class IssuesApiController : ControllerBase
    {
        private IIssueService issues;
        private UserManager<User> userManager;

        public IssuesApiController(IIssueService issueService, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.issues = issueService;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetIssuesInfoBoxDetails()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            RegionType? region = user.ManagedRegion;

            var data = await this.issues.AllMapInfoDetailsAsync(areApproved: false, region: region);

            return Ok(data);
        }
    }
}