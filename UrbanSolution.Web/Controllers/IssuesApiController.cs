using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Services;

namespace UrbanSolution.Web.Controllers
{
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