namespace UrbanSolution.Web.Models.Issues
{
    using System;
    using System.Collections.Generic;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssuesIndexModel
    {
        public Dictionary<string, string> SortingType =>
            new Dictionary<string, string>
            {
                { SortingDateDesc, SortDesc},
                { SortingDateAsc, SortAsc}
            };

        public Dictionary<string, string> RegionFilter
        {
            get
            {
                var regions = new Dictionary<string, string>();    //{ {OptionAll, null} };

                foreach (string region in Enum.GetNames(typeof(RegionType)))
                {
                    if (region == OptionAllRegions || regions.ContainsKey(region))
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
                    if (!regions.ContainsKey(type)) regions.Add(type, type);

                return regions;
            }
        }

    }
}
