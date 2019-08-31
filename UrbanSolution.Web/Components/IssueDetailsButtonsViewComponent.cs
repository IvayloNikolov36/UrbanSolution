namespace UrbanSolution.Web.Components
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    [ViewComponent(Name = ViewComponentIssueDetailsButtonsName)]
    public class IssueDetailsButtonsViewComponent : ViewComponent
    {
        private readonly UserManager<User> userManager;

        public IssueDetailsButtonsViewComponent(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(RegionType region)
        {
            var user = await this.userManager.GetUserAsync(this.UserClaimsPrincipal);

            if (user != null)
            {
                this.ViewData[ViewDataManagerRegionKey] = user.ManagedRegion.ToString();
                this.ViewData[IssueRegionKey] = region;
            }

            return this.View();
        }
    }
}
