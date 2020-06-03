namespace UrbanSolution.Web.Models.Areas.Admin
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    public class AdminUserTableRowModel
    {
        public AdminUserListingModel User { get; set; }

        public IEnumerable<SelectListItem> LockDays { get; set; }
    }
}
