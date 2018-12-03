using System;
using AutoMapper;
using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Models
{
    public class UrbanIssueDetailsServiceModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string IssuePictureUrl { get; set; }

        public string Publisher { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Region { get; set; }

        public string Type { get; set; }

        public string AddressStreet { get; set; }

        public string StreetNumber { get; set; }

        public bool IsApproved { get; set; }

        public bool HasResolved { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, UrbanIssueDetailsServiceModel>()
                .ForMember(x => x.IsApproved, m => m.MapFrom(u => u.ResolvedIssueId != null))
                .ForMember(x => x.Latitude, m => m.MapFrom(u => u.Latitude.ToString().Replace(",", ".")))
                .ForMember(x => x.Longitude, m => m.MapFrom(u => u.Longitude.ToString().Replace(",", ".")));
        }
    }
}
