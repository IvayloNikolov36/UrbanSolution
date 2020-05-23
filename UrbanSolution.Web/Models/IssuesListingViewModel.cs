namespace UrbanSolution.Web.Models
{
    using System.Collections.Generic;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    public class IssuesListingViewModel
    {
        public IEnumerable<UrbanIssuesListingServiceModel> Issues { get; set; }

        public int PagesCount { get; set; }

        public int Page { get; set; }

        public RegionType? Region { get; set; }

    }
}
