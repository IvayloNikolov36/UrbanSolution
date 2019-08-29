namespace UrbanSolution.Web.Components
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    [ViewComponent(Name = ViewComponentDynamicMenuName)]
    public class DynamicMenuViewComponent : ViewComponent
    {
        private readonly UserManager<User> userManager;

        public DynamicMenuViewComponent(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await this.userManager.GetUserAsync(this.UserClaimsPrincipal);

            if (user != null)
            {
                this.ViewData[ViewDataUsernameKey] = user.UserName;
            }

            return this.View();
        }
    }
}
