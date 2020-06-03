namespace UrbanSolution.Web.Models.Areas.Admin
{
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;

    public class AdminUserListingModel : IMapFrom<UsersWithRoles>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public string UserRoles { get; set; }

        public string UserNotInRoles { get; set; }
    }
}
