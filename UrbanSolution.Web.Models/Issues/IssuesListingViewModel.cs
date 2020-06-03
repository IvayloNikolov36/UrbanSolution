namespace UrbanSolution.Web.Models.Issues
{
    using System.Collections.Generic;
    using UrbanSolution.Models;

    public class IssuesListingViewModel
    {
        public IEnumerable<IssuesListingModel> Issues { get; set; }

        public int PagesCount { get; set; }

        public int Page { get; set; }

        public RegionType? Region { get; set; }

    }
}
