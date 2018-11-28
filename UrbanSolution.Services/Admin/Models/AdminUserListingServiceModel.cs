using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UrbanSolution.Services.Admin.Models
{
    public class AdminUserListingServiceModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> UserRoles { get; set; }

    }
}
