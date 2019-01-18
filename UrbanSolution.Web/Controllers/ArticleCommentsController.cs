namespace UrbanSolution.Web.Controllers
{
    using Infrastructure.Filters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using Infrastructure;

    public class ArticleCommentsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IArticleCommentService comments;

        public ArticleCommentsController(UserManager<User> userManager, IArticleCommentService comments)
        {
            this.userManager = userManager;
            this.comments = comments;
        }

        [ServiceFilter(typeof(ValidateArticleIdExistsAttribute))]
        public async Task<IActionResult> All(int id)
        {
            var allComments = await this.comments.AllAsync(id);

            return new JsonResult(allComments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var comment = await this.comments.GetAsync(id);

            return new JsonResult(comment);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Submit(int id, string content)
        {
            //TODO: check if content is empty, length... 
            var user = await this.userManager.GetUserAsync(this.User);

            var comment = await this.comments.SubmitAsync(id, user.Id, content);

            if (comment == null)
            {
                return this.BadRequest();
            }

            return new JsonResult(comment);
        }

        [Authorize(Roles = WebConstants.AdminRole)]
        public async Task<IActionResult> Delete(int id)
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