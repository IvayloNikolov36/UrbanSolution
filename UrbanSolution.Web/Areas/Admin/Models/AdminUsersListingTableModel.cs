namespace UrbanSolution.Web.Areas.Admin.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using UrbanSolution.Services.Admin.Models;

    public class AdminUsersListingTableModel
    {
        public IEnumerable<AdminUserListingServiceModel> Users { get; set; }

        public IEnumerable<SelectListItem> AllRoles { get; set; }

        public IEnumerable<SelectListItem> LockDays { get; set; }
    }
}
