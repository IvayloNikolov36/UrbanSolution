namespace UrbanSolution.Services.Admin.Models
{
    using Mapping;
    using System;
    using UrbanSolution.Models;

    public class AdminUserListingServiceModel : IMapFrom<UsersWithRoles>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public string UserRoles { get; set; }

        public string UserNotInRoles { get; set; }
    }
}
