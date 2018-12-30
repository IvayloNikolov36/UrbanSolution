namespace UrbanSolution.Web.Components
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Models;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    using static Infrastructure.WebConstants;
    
    [ViewComponent(Name = ViewComponentUrbanIssuesMapWithMarkersName)]
    public class UrbanIssuesMapWithMarkersViewComponent : ViewComponent
    {
        private readonly IConfiguration configuration;

        public UrbanIssuesMapWithMarkersViewComponent(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(RegionType? region)
        {
            (double mapRegLat, double mapRegLong, double mapRegZoom) = region.GetMapPositions();

            var apiKey = this.configuration.GetSection("Google:MapsApiKey").Value;

            var model = new MapRegionInfoComponentViewModel
            {
                ApiKey = apiKey,
                Latitude = mapRegLat,
                Longitude = mapRegLong,
                Zoom = mapRegZoom
            };

            return this.View(model);
        }
    }
}
