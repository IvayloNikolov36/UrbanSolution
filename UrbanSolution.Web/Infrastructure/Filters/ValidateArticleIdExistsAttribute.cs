namespace UrbanSolution.Web.Infrastructure.Filters
{
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Data;
    using System.Linq;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class ValidateArticleIdExistsAttribute : ActionFilterAttribute
    {
        private readonly UrbanSolutionDbContext db;

        public ValidateArticleIdExistsAttribute(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller == null)
            {
                return;
            }

            var id = context.ActionArguments
                .FirstOrDefault(a => a.Key.ToLower().Contains("id")).Value;
            if (id == null)
            {
                context.Result = controller.RedirectToAction("Index")
                    .WithDanger(string.Empty, ArticleNotFound);
            }
            else
            {
                var exists = this.db.Find<Article>(id) != null;

                if (!exists)
                {
                    context.Result = controller.RedirectToAction("Index")
                        .WithDanger(string.Empty, ArticleNotFound);
                }
            }

        }
    }
}
