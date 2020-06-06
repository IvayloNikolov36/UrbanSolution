namespace UrbanSolution.Web.Models.Issues
{
    using AutoMapper;
    using UrbanSolution.Services.Mapping;
    using System;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssueDetailsModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string IssuePictureUrl { get; set; }

        public string Publisher { get; set; }

        public DateTime PublishedOn { get; set; }

        public RegionType Region { get; set; }

        public IssueType Type { get; set; }

        public string AddressStreet { get; set; }

        public bool IsApproved { get; set; }

        public bool HasResolved { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public int? ResolvedId { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, IssueDetailsModel>()
                .ForMember(x => x.ResolvedId, m => m.MapFrom(u => u.ResolvedIssue.Id))
                .ForMember(x => x.HasResolved, m => m.MapFrom(u => u.ResolvedIssue != null))
                .ForMember(x => x.Latitude, m => m.MapFrom(u => u.Latitude.ToString().Replace(",", ".")))
                .ForMember(x => x.Longitude, m => m.MapFrom(u => u.Longitude.ToString().Replace(",", ".")))
                .ForMember(x => x.IssuePictureUrl, m => m.MapFrom(u => CloudPicUrlPrefix + u.CloudinaryImage.PictureUrl));
        }
    }
}
