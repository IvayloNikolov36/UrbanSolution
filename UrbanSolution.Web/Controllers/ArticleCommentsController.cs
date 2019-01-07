using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Models;
using UrbanSolution.Services;

namespace UrbanSolution.Web.Controllers
{
    public class ArticleCommentsController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IArticleCommentService comments;

        public ArticleCommentsController(UserManager<User> userManager, IArticleCommentService comments)
        {
            this.userManager = userManager;
            this.comments = comments;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, string content)
        {
            //TODO: complete 
            var user = await this.userManager.GetUserAsync(this.User);

            var comment = await this.comments.SubmitAsync(id, user.Id, content);

            if (comment == null)
            {
                return this.BadRequest();
            }

            return new JsonResult(comment);
        }

        public async Task<IActionResult> All(int id)
        {
            var allComments = await this.comments.AllAsync(id);

            return new JsonResult(allComments);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var comment = this.comments.GetAsync(id);

            return new JsonResult(comment);
        }
    }
}