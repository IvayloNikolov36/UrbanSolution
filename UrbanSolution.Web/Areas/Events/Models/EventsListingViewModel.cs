namespace UrbanSolution.Web.Areas.Events.Models
{
    using Services.Utilities;
    using System;
    using System.Collections.Generic;

    public class EventsListingViewModel
    {
        public IEnumerable<EventsListingServiceModel> Events { get; set; }

        public int TotalEvents { get; set; }

        public int TotalPages => (int) Math.Ceiling((double) this.TotalEvents / ServiceConstants.EventsPageSize);

        public int CurrentPage { get; set; }

        public int PreviousPage => this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;

        public int NextPage => this.CurrentPage == this.TotalPages ? this.TotalPages : this.CurrentPage + 1;


    }
}
