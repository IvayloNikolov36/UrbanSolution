namespace UrbanSolution.Services.Events.Models
{
    using AutoMapper;
    using Mapping;
    using UrbanSolution.Models;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class EventDetailsServiceModel : IMapFrom<Event>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PictureUrl { get; set; }

        public string Address { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string CreatorUserName { get; set; }

        public List<string> Participants { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Event, EventDetailsServiceModel>()
                .ForMember(x => x.Participants,
                    m => m.MapFrom(e => e.Participants.Select(p => p.Participant.UserName)))
                .ForMember(x => x.CreatorUserName, m => m.MapFrom(e => e.Creator.UserName))
                .ForMember(x => x.Latitude, m => m.MapFrom(e => e.Latitude.ToString().Replace(",", ".")))
                .ForMember(x => x.Longitude, m => m.MapFrom(e => e.Longitude.ToString().Replace(",", ".")));

        }
    }
}
