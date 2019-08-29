using static UrbanSolution.Services.Utilities.ServiceConstants;

namespace UrbanSolution.Web.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;
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

        //public int PreviousPage => this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;

        //public int NextPage => this.CurrentPage == this.TotalRows ? this.TotalRows : this.CurrentPage + 1;

        public bool UseCarousel { get; set; } = false;

        public RegionType? Region { get; set; }

        public IEnumerable<SelectListItem> SortingType =>
            new List<SelectListItem>
            {
                new SelectListItem(SortingDateDesc, SortDesc),
                new SelectListItem(SortingDateAsc, SortAsc)
            };

        //Drop Downs for Filtering

        public IEnumerable<SelectListItem> RegionFilter
        {
            get
            {
                var regions = new List<SelectListItem> { new SelectListItem(OptionAll, null) };

                foreach (string region in Enum.GetNames(typeof(RegionType)))
                {
                    if (region == OptionAll)
                        continue;

                    regions.Add(new SelectListItem(region, region));
                }

                return regions;
            }
        }

        public IEnumerable<SelectListItem> TypeFilter
        {
            get
            {
                var regions = new List<SelectListItem> { new SelectListItem(OptionTextAllTypes, null) }; //OptionAll

                foreach (string type in Enum.GetNames(typeof(IssueType)))
                    regions.Add(new SelectListItem(type, type));

                return regions;
            }
        }

    }
}
