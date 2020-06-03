namespace UrbanSolution.Web.Components
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using static UrbanSolutionUtilities.WebConstants;
    using UrbanSolution.Web.Models.Issues;

    [ViewComponent(Name = ViewComponentIssueDetailsMapName)]
    public class IssueDetailsMapViewComponent : ViewComponent
    {
        private readonly IConfiguration configuration;

        public IssueDetailsMapViewComponent(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IViewComponentResult Invoke(IssueDetailsModel model)
        {
            var viewModel = new MapIssueMarkerComponentViewModel
            {
                ApiKey = this.configuration.GetSection("Google:MapsApiKey").Value,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Zoom = ZoomMapIssueDetails.ToString().Replace(",", ".")
            };

            return this.View(viewModel);
        }
    }
}
