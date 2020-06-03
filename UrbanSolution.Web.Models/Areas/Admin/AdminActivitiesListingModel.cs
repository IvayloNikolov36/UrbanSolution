namespace UrbanSolution.Web.Models.Areas.Admin
{
    using AutoMapper;
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;

    public class AdminActivitiesListingModel : IMapFrom<AdminLog>, IHaveCustomMappings
    {
        public string EditedUserUserName { get; set; }

        public string Activity { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ForRole { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<AdminLog, AdminActivitiesListingModel>()
                .ForMember(x => x.EditedUserUserName, m => m.MapFrom(e => e.EditedUser.UserName))
                .ForMember(x => x.Activity, m => m.MapFrom(e => e.Activity));
        }

    }
}
