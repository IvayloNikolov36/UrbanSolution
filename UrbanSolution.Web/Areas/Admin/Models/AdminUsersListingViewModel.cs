namespace UrbanSolution.Web.Areas.Admin.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UrbanSolution.Services.Admin.Models;

    public class AdminUsersListingViewModel
    {
        public IEnumerable<AdminUserListingServiceModel> Users { get; set; }

        public IEnumerable<SelectListItem> AllRoles { get; set; }
        
        public IEnumerable<SelectListItem> SearchFilters { get; set; }

        public IEnumerable<SelectListItem> LockDays { get; set; }

        public IDictionary<string, string> FilterBy { get; set; }

        public IEnumerable<SelectListItem> SortBy  => new List<SelectListItem>
        {
            new SelectListItem { Value = null, Text = "Sort by" },
            new SelectListItem { Value = "UserName", Text = "Username" },
            new SelectListItem { Value = "Email", Text = "Email"  }
        };

        public IEnumerable<SelectListItem> SortingType => new List<SelectListItem>
        {
            new SelectListItem { Value = "ASC", Text = "ASC" },
            new SelectListItem { Value = "DESC", Text = "DESC" }
        };
    }
}
