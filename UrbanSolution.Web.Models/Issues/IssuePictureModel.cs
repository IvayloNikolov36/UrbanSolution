namespace UrbanSolution.Web.Models.Issues
{
    using AutoMapper;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssuePictureModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public string IssuePictureUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, IssuePictureModel>()
               .ForMember(x => x.IssuePictureUrl, m => m.MapFrom(u => CloudPicUrlPrefix + u.CloudinaryImage.PictureUrl));
        }
    }
}
