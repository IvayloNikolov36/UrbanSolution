namespace UrbanSolution.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class IssuesApiController : ControllerBase
    {
        private IIssueService issues;

        public IssuesApiController(IIssueService issueService)
        {
            this.issues = issueService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetIssuesInfoBoxDetails()
        {
            var data = await this.issues.AllMapInfoDetailsAsync(areApproved: false);

            return Ok(data);
        }
    }
}