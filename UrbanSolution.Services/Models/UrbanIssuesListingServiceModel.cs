namespace UrbanSolution.Services.Models
{
    using AutoMapper;
    using Mapping;
    using System;
    using UrbanSolution.Models;

    public class UrbanIssuesListingServiceModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
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

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, UrbanIssuesListingServiceModel>()
                .ForMember(x => x.HasResolved, m => m.MapFrom(u => u.ResolvedIssue != null))
                .ForMember(x => x.Publisher, m => m.MapFrom(u => u.Publisher.UserName))
                .ForMember(x => x.Latitude, m => m.MapFrom(u => u.Latitude.ToString().Replace(",", ".")))
                .ForMember(x => x.Longitude, m => m.MapFrom(u => u.Longitude.ToString().Replace(",", ".")))
                .ForMember(x => x.IssuePictureThumbnailUrl, m => m.MapFrom(u => u.CloudinaryImage.PictureThumbnailUrl));
        }
    }
}
