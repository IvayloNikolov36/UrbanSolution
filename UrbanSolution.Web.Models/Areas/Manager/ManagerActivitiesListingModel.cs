namespace UrbanSolution.Web.Models.Areas.Manager
{
    using AutoMapper;
    using UrbanSolution.Services.Mapping;
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class ManagerActivitiesListingModel : IMapFrom<ManagerLog>, IHaveCustomMappings
    {
        public string UserName { get; set; }

        public DateTime DateTime { get; set; }

        public ManagerActivityType Activity { get; set; }


        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ManagerLog, ManagerActivitiesListingModel>()
                .ForMember(x => x.UserName, m => m.MapFrom(ml => ml.Manager.UserName));
        }
    }
}
