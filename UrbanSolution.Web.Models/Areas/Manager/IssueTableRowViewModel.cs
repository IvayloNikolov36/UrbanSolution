namespace UrbanSolution.Web.Models.Areas.Manager
{
    using AutoMapper;
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssueTableRowViewModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string PictureThumbnail { get; set; }

        public string Title { get; set; }

        public string AddressStreet { get; set; }

        public DateTime PublishedOn { get; set; }

        public string Region { get; set; }

        public string Description { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, IssueTableRowViewModel>()
                .ForMember(x => x.PictureThumbnail, m => m
                    .MapFrom(u => CloudPicUrlPrefix + u.CloudinaryImage.PictureThumbnailUrl))
                .ForMember(x => x.Region, m => m.MapFrom(u => u.Region.ToString()));
        }
    }
}
