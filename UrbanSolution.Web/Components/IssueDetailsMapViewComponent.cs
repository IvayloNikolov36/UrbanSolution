namespace UrbanSolution.Web.Components
{
    using Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using UrbanSolution.Services.Models;
    using static Infrastructure.WebConstants;

    [ViewComponent(Name = ViewComponentIssueDetailsMapName)]
    public class IssueDetailsMapViewComponent : ViewComponent
    {
        private readonly IConfiguration configuration;

        public IssueDetailsMapViewComponent(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IViewComponentResult Invoke(UrbanIssueDetailsServiceModel model)
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
