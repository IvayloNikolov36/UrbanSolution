using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Manager.Models
{
    public class IssueRegionServiceModel : IMapFrom<UrbanIssue>
    {
        public RegionType Region { get; set; }
    }
}
