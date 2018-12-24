
namespace UrbanSolution.Services.Manager.Models
{
    using AutoMapper;
    using Mapping;
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class ManagerActivitiesListingServiceModel : IMapFrom<ManagerLog>, IHaveCustomMappings
    {
        public string UserName { get; set; }

        public DateTime DateTime { get; set; }

        public ManagerActivityType Activity { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ManagerLog, ManagerActivitiesListingServiceModel>()
                .ForMember(x => x.UserName, m => m.MapFrom(ml => ml.Manager.UserName));
        }
    }
}
