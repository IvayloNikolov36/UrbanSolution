namespace UrbanSolution.Web.Areas.Events.Models
{
    using System.Collections.Generic;
    using UrbanSolution.Services.Events.Models;
    using UrbanSolution.Web.Models;

    public class EventsListingViewModel
    {
        public IEnumerable<EventsListingServiceModel> Events { get; set; }

        public PagesModel PagesModel { get; set; }

    }
}
