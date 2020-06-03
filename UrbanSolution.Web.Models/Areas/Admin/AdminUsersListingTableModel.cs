namespace UrbanSolution.Web.Models.Areas.Admin
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    public class AdminUsersListingTableModel
    {
        public IEnumerable<AdminUserListingModel> Users { get; set; }

        public IEnumerable<SelectListItem> LockDays { get; set; }
    }
}
