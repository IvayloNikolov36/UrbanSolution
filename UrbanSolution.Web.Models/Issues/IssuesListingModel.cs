namespace UrbanSolution.Web.Models.Issues
{
    using AutoMapper;
    using UrbanSolution.Services.Mapping;
    using System;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssuesListingModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string IssuePictureThumbnailUrl { get; set; }

        public bool HasResolved { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Publisher { get; set; }

        public bool IsApproved { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Region { get; set; } 

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, IssuesListingModel>()
                .ForMember(x => x.HasResolved, m => m.MapFrom(u => u.ResolvedIssue != null))
                .ForMember(x => x.Publisher, m => m.MapFrom(u => u.Publisher.UserName))
                .ForMember(x => x.Latitude, m => m.MapFrom(u => u.Latitude.ToString().Replace(",", ".")))
                .ForMember(x => x.Longitude, m => m.MapFrom(u => u.Longitude.ToString().Replace(",", ".")))
                .ForMember(x => x.IssuePictureThumbnailUrl, m => m
                    .MapFrom(u => CloudPicUrlPrefix + u.CloudinaryImage.PictureThumbnailUrl))
                .ForMember(x => x.Region, m => m.MapFrom(u => u.Region.ToString()));
        }
    }
}
