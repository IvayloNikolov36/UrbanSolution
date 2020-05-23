namespace UrbanSolution.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    [Route("api/[controller]")]
    [ApiController]
    public class IssuesApiController : ControllerBase
    {
        //TODO: move all actions to IssueController
        private readonly IIssueService issues;
        private readonly UserManager<User> userManager;
        private readonly IUserIssuesService userIssuesService;

        public IssuesApiController(IIssueService issueService, 
            UserManager<User> userManager, 
            IUserIssuesService userIssuesService)
        {
            //TODO: leave only one service (from userIssuesService move methods to issueService)
            this.userManager = userManager;
            this.userIssuesService = userIssuesService;
            this.issues = issueService;
        }

        //[HttpGet("")]
        //[Authorize]
        //public async Task<IActionResult> GetIssuesInfoBoxDetails()
        //{
        //    var user = await this.userManager.GetUserAsync(this.User);
        //    RegionType? region = user.ManagedRegion;

        //    var data = await this.issues
        //        .AllMapInfoDetailsAsync<IssueMapInfoBoxDetailsServiceModel>(areApproved: false, region: region);

        //    return this.Ok(data);
        //}

        //[HttpPost("uploadImage")]
        //[Authorize]
        //public async Task<ActionResult<int>> UploadImage(IFormFile file)
        //{
        //    var user = await this.userManager.GetUserAsync(this.User);

        //    var picId = await this.userIssuesService.UploadIssueImageAsync(user.Id, file);

        //    return this.Ok(picId);
        //}

    }
}