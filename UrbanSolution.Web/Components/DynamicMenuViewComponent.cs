using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrbanSolution.Models;

namespace UrbanSolution.Web.Components
{
    [ViewComponent(Name = "DynamicMenu")]
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
                this.ViewData["username"] = user.UserName;
            }

            return this.View();
        }
    }
}
