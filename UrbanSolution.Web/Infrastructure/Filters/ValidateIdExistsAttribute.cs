namespace UrbanSolution.Web.Infrastructure.Filters
{
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Linq;
    using Services.Manager;

    public class ValidateIssueIdExistsAttribute : ActionFilterAttribute
    {
        private IManagerIssueService issues;

        public ValidateIssueIdExistsAttribute(IManagerIssueService issues)
        {
            this.issues = issues;
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
            }   

        }

    }
}
