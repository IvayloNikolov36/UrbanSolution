using System;
using System.Collections.Generic;
using AutoMapper;
using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Models
{
    public class ResolvedDetailsServiceModel : IMapFrom<ResolvedIssue>, IHaveCustomMappings
    {
        public string Description { get; set; }

        public string PictureUrl { get; set; }

        public string IssuePictureUrl { get; set; }

        public string PublishedOn { get; set; }

        public string PublisherUserName { get; set; }

        public DateTime ResolvedOn { get; set; }

        public double Rating { get; set; }

        public ICollection<Comment> Comments { get; set; } 

        public ICollection<Rating> Ratings { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ResolvedIssue, ResolvedDetailsServiceModel>()
                .ForMember(x => x.IssuePictureUrl, m => m.MapFrom(r => r.UrbanIssue.IssuePictureUrl));
        }
    }
}
