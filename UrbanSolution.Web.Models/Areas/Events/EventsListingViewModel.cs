namespace UrbanSolution.Web.Models.Areas.Events
{
    using System.Collections.Generic;
    using UrbanSolution.Web.Models.Common;

    public class EventsListingViewModel
    {
        public IEnumerable<EventsListingModel> Events { get; set; }

        public PagesModel PagesModel { get; set; }

    }
}
