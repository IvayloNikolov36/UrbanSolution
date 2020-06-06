namespace UrbanSolution.Web.Models.Areas.Manager
{
    using AutoMapper;
    using UrbanSolution.Services.Mapping;
    using System;
    using System.Collections.Generic;    
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class ResolvedDetailsModel : IMapFrom<ResolvedIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string ResolvedPictureUrl { get; set; }

        public string IssuePictureUrl { get; set; }

        public string PublisherId { get; set;  }

        public string PublisherUserName { get; set; }

        public DateTime ResolvedOn { get; set; }

        public double Rating { get; set; }

        public ICollection<Comment> Comments { get; set; } 

        public ICollection<Rating> Ratings { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ResolvedIssue, ResolvedDetailsModel>()
                .ForMember(x => x.IssuePictureUrl, m => m
                    .MapFrom(r => CloudPicUrlPrefix + r.UrbanIssue.CloudinaryImage.PictureUrl))
                .ForMember(x => x.ResolvedPictureUrl, m => m
                    .MapFrom(r => CloudPicUrlPrefix + r.CloudinaryImage.PictureUrl))
                .ForMember(x => x.PublisherUserName, m => m.MapFrom(r => r.Publisher.UserName));
        }
    }
}
