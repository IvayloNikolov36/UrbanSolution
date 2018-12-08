namespace UrbanSolution.Services.Admin.Models
{
    using Mapping;
    using System.Collections.Generic;
    using UrbanSolution.Models;

    public class AdminUserListingServiceModel : IMapFrom<User>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> UserRoles { get; set; }

    }
}
