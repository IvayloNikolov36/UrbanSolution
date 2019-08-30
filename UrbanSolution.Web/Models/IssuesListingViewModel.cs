namespace UrbanSolution.Web.Models
{
    using System;
    using System.Collections.Generic;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssuesListingViewModel
    {
        public IEnumerable<UrbanIssuesListingServiceModel> Issues { get; set; }

        public int TotalIssues { get; set; }

        public int TotalRows =>
            (int)Math.Ceiling((double)this.TotalIssues / IssuesOnRow);

        public int CurrentPage { get; set; }

        public bool UseCarousel { get; set; } = false;

        public RegionType? Region { get; set; }

        public Dictionary<string, string> SortingType =>
            new Dictionary<string, string>
            {
                { SortingDateDesc, SortDesc},
                { SortingDateAsc, SortAsc}
            };

        //Drop Downs for Filtering

        public Dictionary<string, string> RegionFilter
        {
            get
            {
                var regions = new Dictionary<string, string>();    //{ {OptionAll, null} };

                foreach (string region in Enum.GetNames(typeof(RegionType)))
                {
                    if (region == OptionAll || regions.ContainsKey(region))
                        continue;

                    regions.Add(region, region);
                }

                return regions;
            }
        }

        public Dictionary<string, string> TypeFilter
        {
            get
            {
                var regions = new Dictionary<string, string>(); //{ {OptionTextAllTypes, null} };

                foreach (string type in Enum.GetNames(typeof(IssueType)))
                    if(!regions.ContainsKey(type)) regions.Add(type, type);

                return regions;
            }
        }

    }
}
