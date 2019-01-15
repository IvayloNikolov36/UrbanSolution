namespace UrbanSolution.Services.Events.Models
{
    using System;
    using AutoMapper;
    using Mapping;
    using UrbanSolution.Models;

    public class EventsListingServiceModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PictureUrl { get; set; }

        public DateTime Starts { get; set; }

        public DateTime Ends { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Event, EventsListingServiceModel>()
                .ForMember(x => x.Starts, m => m.MapFrom(e => e.StartDate))
                .ForMember(x => x.Ends, m => m.MapFrom(e => e.EndDate));
        }
    }
}
