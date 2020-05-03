namespace UrbanSolution.Web.Areas.Admin.Models
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using UrbanSolution.Services.Admin.Models;

    public class AdminUserTableRowModel
    {
        public AdminUserListingServiceModel User { get; set; }

        public IEnumerable<SelectListItem> LockDays { get; set; }
    }
}
