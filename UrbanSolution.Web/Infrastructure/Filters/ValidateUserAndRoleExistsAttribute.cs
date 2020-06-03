namespace UrbanSolution.Web.Infrastructure.Filters
{
    using Extensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Linq;
    using UrbanSolution.Models;
    using UrbanSolution.Web.Models.Areas.Admin;
    using static UrbanSolutionUtilities.WebConstants;

    public class ValidateUserAndRoleExistsAttribute : ActionFilterAttribute
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ValidateUserAndRoleExistsAttribute(
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;

            if (controller == null)
            {
                return;
            }

            var model = context
                .ActionArguments
                .FirstOrDefault(a => a.Key.ToLower()
                .Contains("model"))
                .Value as UserToRoleFormModel;

            if (model == null)
            {
                return;
            }

            var userExists = this.userManager
                                 .FindByIdAsync(model.UserId)
                                 .GetAwaiter().GetResult() != null;

            var roleExists = this.roleManager.RoleExistsAsync(model.Role).GetAwaiter().GetResult();

            if (!roleExists || !userExists)
            {
                context.Result =  controller
                    .RedirectToAction("Index", "Users", new { area = "Admin" })
                    .WithDanger(string.Empty, InvalidIdentityDetails);
            }

        }
    }
}
