namespace UrbanSolution.Web.Models.Areas.Events
{
    using System;
    using AutoMapper;
    using UrbanSolution.Services.Mapping;
    using UrbanSolution.Models;

    public class EventsListingModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PictureUrl { get; set; }

        public DateTime Starts { get; set; }

        public DateTime Ends { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventsListingModel>()
                .ForMember(x => x.Starts, m => m.MapFrom(e => e.StartDate))
                .ForMember(x => x.Ends, m => m.MapFrom(e => e.EndDate));
        }
    }
}
