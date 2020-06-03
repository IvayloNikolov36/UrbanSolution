namespace UrbanSolution.Web.Models.Areas.Admin
{
    using System;
    using System.Collections.Generic;
    using UrbanSolution.Web.Models.Common;
    using UrbanSolutionUtilities.Enums;
    using UrbanSolutionUtilities.Extensions;
    using static UrbanSolutionUtilities.WebConstants;

    public class AdminUsersListingViewModel
    {
        public AdminUsersListingTableModel TableDataModel { get; set; }

        public PagesModel PagesModel { get; set; }

        public IDictionary<string, string> SearchFiltersOptions
        {
            get
            {
                var filters = new Dictionary<string, string>();

                foreach (string enumName in Enum.GetNames(typeof(UsersFilters)))
                {
                    var filterName = enumName.SeparateStringByCapitals();
                    if (!filters.ContainsKey(filterName))
                    {
                        filters.Add(filterName, enumName);
                    }
                }

                return filters;
            }
        }

        public IDictionary<string, string> FilterByOptions { get; set; }

        public IDictionary<string, string> SortByOptions => new Dictionary<string, string>
        {
            { Username, UserNameProp},
            { EmailProp, EmailProp}
        };

        public IList<string> SortingType => new List<string> { SortAsc, SortDesc };

    }
}
