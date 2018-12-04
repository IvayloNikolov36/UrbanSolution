using System;
using AutoMapper;
using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Models
{
    public class UrbanIssuesListingServiceModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string IssuePictureUrl { get; set; }

        public bool HasResolved { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Publisher { get; set; }

        public bool IsApproved { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, UrbanIssuesListingServiceModel>()
                .ForMember(x => x.HasResolved, m => m.MapFrom(u => u.ResolvedIssue != null));
        }
    }
}
