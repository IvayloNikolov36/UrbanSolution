namespace UrbanSolution.Web.Models.Issues
{
    using AutoMapper;
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;

    public class IssueDetailsViewModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Region { get; set; }

        public string Publisher { get; set; }

        public DateTime PublishedOn { get; set; }

        public string AddressStreet { get; set; }

        public string PictureUrl { get; set; }

        public string PictureThumbnailUrl { get; set; }

        public IssueType Type { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, IssueDetailsViewModel>()
                .ForMember(x => x.PictureUrl, m => m.MapFrom(u => u.CloudinaryImage.PictureUrl))
                .ForMember(x => x.PictureThumbnailUrl, m => m.MapFrom(u => u.CloudinaryImage.PictureThumbnailUrl))
                .ForMember(x => x.Publisher, m => m.MapFrom(u => u.Publisher.UserName));
        }
    }
}
