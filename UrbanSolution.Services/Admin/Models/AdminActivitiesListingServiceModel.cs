﻿namespace UrbanSolution.Services.Admin.Models
{
    using AutoMapper;
    using Mapping;
    using System;
    using UrbanSolution.Models;

    public class AdminActivitiesListingServiceModel : IMapFrom<AdminLog>, IHaveCustomMappings
    {
        public string EditedUserUserName { get; set; }

        public string Activity { get; set; }

        public DateTime DateTime { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<AdminLog, AdminActivitiesListingServiceModel>()
                .ForMember(x => x.EditedUserUserName, m => m.MapFrom(e => e.EditedUser.UserName))
                .ForMember(x => x.Activity, m => m.MapFrom(e => e.Activity.ToString()));
        }
    }
}
