namespace UrbanSolution.Services.Admin.Models
{
    using AutoMapper;
    using Mapping;
    using System;
    using System.Collections.Generic;
    using UrbanSolution.Models;

    public class AdminUserListingServiceModel : IMapFrom<User>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime? LockoutEnd { get; set; }      

        public List<string> UserRoles { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<User, AdminUserListingServiceModel>()
                .ForMember(x => x.LockoutEnd, m => m.MapFrom(e => e.LockoutEnd.Value.DateTime));
        }
    }
}
