namespace UrbanSolution.Web.Models.Areas.Manager
{
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;

    public class IssueRegionModel : IMapFrom<UrbanIssue>
    {
        public RegionType Region { get; set; }
    }
}
