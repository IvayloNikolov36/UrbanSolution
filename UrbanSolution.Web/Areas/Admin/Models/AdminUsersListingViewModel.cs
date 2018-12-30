
namespace UrbanSolution.Web.Areas.Admin.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UrbanSolution.Services.Admin.Models;

    public class AdminUsersListingViewModel
    {
        public IEnumerable<AdminUserListingServiceModel> Users { get; set; }

        public IEnumerable<SelectListItem> AllRoles { get; set; }
        
    }
}
