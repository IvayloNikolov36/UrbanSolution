using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UrbanSolution.Models;
using UrbanSolution.Services.Manager;
using UrbanSolution.Web.Infrastructure.Extensions;

namespace UrbanSolution.Web.Infrastructure.Filters
{
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
                controller.TempData.AddErrorMessage(WebConstants.IssueNotFound);
                context.Result = controller.RedirectToAction("Index");
            }
            else
            {
                var exists = this.issues.ExistsAsync((int)id).GetAwaiter().GetResult();
                if (!exists)
                {
                    controller.TempData.AddErrorMessage(WebConstants.IssueNotFound);
                    context.Result = controller.RedirectToAction("Index");
                }
                else
                {
                    var manager = this.userManager.GetUserAsync(context.HttpContext.User).GetAwaiter().GetResult();

                    var areEqual = this.issues.IsIssueInSameRegionAsync((int)id, manager.ManagedRegion).GetAwaiter().GetResult();
                    if (!areEqual)
                    {
                        controller.TempData.AddErrorMessage(WebConstants.CantChangeIssue);
                        context.Result = controller.RedirectToAction("Index");
                    }
                }
                
            }
                     
        }
    }
}
