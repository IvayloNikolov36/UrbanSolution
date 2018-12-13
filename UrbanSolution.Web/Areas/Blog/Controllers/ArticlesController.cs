using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UrbanSolution.Models;
using UrbanSolution.Services.Blog;
using UrbanSolution.Services.Blog.Models;
using UrbanSolution.Services.Html;
using UrbanSolution.Services.Utilities;
using UrbanSolution.Web.Areas.Blog.Models;
using UrbanSolution.Web.Infrastructure;
using UrbanSolution.Web.Infrastructure.Extensions;
using UrbanSolution.Web.Infrastructure.Filters;
using UrbanSolution.Web.Models;

namespace UrbanSolution.Web.Areas.Blog.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Area(WebConstants.BlogArea)]
    [Authorize(Roles = WebConstants.BlogAuthorRole)]
    public class ArticlesController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly IBlogArticleService articles;
        private readonly IHtmlService htmlService;

        public ArticlesController(UserManager<User> userManager, IBlogArticleService articles, IHtmlService htmlService)
        {

            this.userManager = userManager;
            this.articles = articles;
            this.htmlService = htmlService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            var model = new ArticleListingViewModel
            {
                Articles = await this.articles.AllAsync(page),
                TotalArticles = await this.articles.TotalAsync(),
                CurrentPage = page
            };

            return this.View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.articles.GetAsync(id);

            return this.ViewOrNotFound(model);
        }

        public IActionResult Create()
        { 
            return this.View();
        }

        [HttpPost]
        [ValidateModelState]
        public async Task<IActionResult> Create(PublishArticleFormModel model)
        {
            model.Content = this.htmlService.Sanitize(model.Content);

            var userId = this.userManager.GetUserId(User);

            await this.articles.CreateAsync(model.Title, model.Content, userId);

            return RedirectToAction(nameof(Index));
        }
    }
}
