namespace UrbanSolution.Services.Models
{
    using AutoMapper;
    using Mapping;
    using System;
    using System.Collections.Generic;    
    using UrbanSolution.Models;  

    public class ResolvedDetailsServiceModel : IMapFrom<ResolvedIssue>, IHaveCustomMappings
    {
        public string Description { get; set; }

        public string ResolvedPictureUrl { get; set; }

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
                .ForMember(x => x.IssuePictureUrl, m => m.MapFrom(r => r.UrbanIssue.CloudinaryImage.PictureUrl))
                .ForMember(x => x.ResolvedPictureUrl, m => m.MapFrom(r => r.CloudinaryImage.PictureUrl));
        }
    }
}
