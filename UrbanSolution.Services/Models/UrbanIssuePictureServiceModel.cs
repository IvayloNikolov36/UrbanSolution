using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Models
{
    public class UrbanIssuePictureServiceModel : IMapFrom<UrbanIssue>
    {
        public string IssuePictureUrl { get; set; }
    }
}
