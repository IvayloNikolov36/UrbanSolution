namespace UrbanSolution.Web.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UrbanSolution.Services.Admin.Models;
    using UrbanSolutionUtilities.Enums;
    using UrbanSolutionUtilities.Extensions;
    using static UrbanSolutionUtilities.WebConstants;

    public class AdminUsersListingViewModel
    {
        public IEnumerable<AdminUserListingServiceModel> Users { get; set; }

        public IEnumerable<SelectListItem> AllRoles { get; set; }
        
        public IDictionary<string, string> SearchFilters
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

        public IEnumerable<SelectListItem> LockDays { get; set; }

        public IDictionary<string, string> FilterBy { get; set; }

        public IDictionary<string, string> SortBy => new Dictionary<string, string>
        {
            { Username, UserNameProp},
            { EmailProp, EmailProp}
        };

        public IList<string> SortingType => new List<string> { SortAsc, SortDesc };
    }
}
