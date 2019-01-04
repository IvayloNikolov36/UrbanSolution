namespace UrbanSolution.Web.Infrastructure.Filters
{
    using Data;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Linq;
    using UrbanSolution.Models;
    using static WebConstants;

    public class ValidateIssueIdExistsAttribute : ActionFilterAttribute
    {
        private readonly UrbanSolutionDbContext db;

        public ValidateIssueIdExistsAttribute(UrbanSolutionDbContext db)
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
                    .WithDanger("", IssueNotFound);
            }
            else
            {
                var exists = this.db.Find<UrbanIssue>(id) != null;
                if (!exists)
                {
                    context.Result = controller.RedirectToAction("Index")
                        .WithDanger("", IssueNotFound);
                }
            }   

        }

    }
}
