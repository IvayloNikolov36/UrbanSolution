namespace UrbanSolution.Web.Models.Issues
{
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;

    public class IssuePictureModel : IMapFrom<UrbanIssue>
    {
        public string IssuePictureUrl { get; set; }
    }
}
