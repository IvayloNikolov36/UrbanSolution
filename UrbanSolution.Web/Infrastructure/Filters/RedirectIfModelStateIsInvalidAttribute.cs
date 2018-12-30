namespace UrbanSolution.Web.Infrastructure.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public class RedirectIfModelStateIsInvalidAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var controller = context.Controller as Controller;

                if (controller == null)
                {
                    return;
                }

                context.Result = controller
                    .RedirectToAction(nameof(Index), "Users", new {area = "Admin"});
            }
        }
    }
}
