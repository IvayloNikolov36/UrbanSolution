namespace UrbanSolution.Web.Areas.Events.Models
{
    using System;
    using System.Collections.Generic;
    using UrbanSolution.Services.Events.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class EventsListingViewModel
    {

        public IEnumerable<EventsListingServiceModel> Events { get; set; }

        public int TotalEvents { get; set; }

        public int TotalPages => (int) Math.Ceiling((double) this.TotalEvents / EventsPageSize);

        public int CurrentPage { get; set; }

        public int PreviousPage => this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;

        public int NextPage => this.CurrentPage == this.TotalPages ? this.TotalPages : this.CurrentPage + 1;

    }
}
