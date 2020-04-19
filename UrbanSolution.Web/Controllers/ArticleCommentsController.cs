namespace UrbanSolution.Web.Controllers
{
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;
    using UrbanSolution.Web.Models;
    using static UrbanSolutionUtilities.WebConstants;

    [ApiController]
    [Route("api/[controller]")]
    public class ArticleCommentsController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IArticleCommentService comments;

        public ArticleCommentsController(UserManager<User> userManager, IArticleCommentService comments)
        {
            this.userManager = userManager;
            this.comments = comments;
        }

        [ServiceFilter(typeof(ValidateArticleIdExistsAttribute))]
        [HttpGet("all/{id}")]
        public async Task<ActionResult<CommentListingServiceModel>> All(int id)
        {
            return this.Ok(await this.comments.AllAsync<CommentListingServiceModel>(id));
        }

        [HttpGet("details/{id}")]
        public async Task<ActionResult<CommentListingServiceModel>> Details(int id)
        {
            return this.Ok(await this.comments.GetAsync<CommentListingServiceModel>(id));
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<ActionResult<CommentListingServiceModel>> Submit(CommentSubmitModel model)
        {
            //TODO: check if content is empty, length... 
            var user = await this.userManager.GetUserAsync(this.User);

            var comment = await this.comments
                .SubmitAsync<CommentListingServiceModel>(model.ArticleId, user.Id, model.Content);

            if (comment == null)
            {
                return this.BadRequest();
            }

            return this.Ok(comment);
        }

        [Authorize(Roles = AdminRole)]
        [HttpGet("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var isDeleted = await this.comments.DeleteAsync(id);

            if (!isDeleted)
            {
                return this.BadRequest();
            }

            return NoContent();
        }
    }
}