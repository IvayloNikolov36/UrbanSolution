namespace UrbanSolution.Web.Infrastructure.Filters
{
    using Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using UrbanSolution.Models;
    using static WebConstants;

    public class ValidateManagerIsMainManagerAttribute : ActionFilterAttribute
    {
        private UserManager<User> userManager;

        public ValidateManagerIsMainManagerAttribute(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller == null)
            {
                return;
            }

            var username = this.userManager.GetUserName(controller.HttpContext.User);

            if (username != ManagerUserName)
            {
                context.Result = controller.RedirectToAction("Index", "Home", new { area = "" })
                    .WithDanger(NotAuthorized, CantViewManagersActivity);
            }

        }

    }
}
