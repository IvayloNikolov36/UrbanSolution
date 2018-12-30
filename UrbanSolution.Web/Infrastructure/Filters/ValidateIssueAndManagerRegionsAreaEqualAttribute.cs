namespace UrbanSolution.Web.Infrastructure.Filters
{
    using Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Linq;
    using Services.Manager;
    using UrbanSolution.Models;
    using static WebConstants;

    public class ValidateIssueAndManagerRegionsAreaEqualAttribute : ActionFilterAttribute
    {
        private IManagerIssueService issues;
        private UserManager<User> userManager;

        public ValidateIssueAndManagerRegionsAreaEqualAttribute(IManagerIssueService issues, UserManager<User> userManager)
        {
            this.issues = issues;
            this.userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller == null)
            {
                return;
            }

            var id = context.ActionArguments.FirstOrDefault(a => a.Key.ToLower().Contains("id")).Value;

            if (id == null)
            {
                context.Result = controller.RedirectToAction("Index")
                    .WithDanger("", IssueNotFound);
            }

            else
            {
                var exists = this.issues.ExistsAsync((int)id).GetAwaiter().GetResult();
                if (!exists)
                {
                    context.Result = controller.RedirectToAction("Index")
                        .WithWarning("", IssueNotFound);
                }
                else
                {
                    var manager = this.userManager.GetUserAsync(context.HttpContext.User).GetAwaiter().GetResult();

                    var areEqual = this.issues.IsIssueInSameRegionAsync((int)id, manager.ManagedRegion).GetAwaiter().GetResult();
                    if (!areEqual)
                    {
                        context.Result = controller.RedirectToAction("Index")
                            .WithDanger("", CantChangeIssue);
                    }
                }
                
            }
                     
        }
    }
}
