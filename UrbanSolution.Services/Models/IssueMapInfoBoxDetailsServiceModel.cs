
namespace UrbanSolution.Services.Models
{
    using AutoMapper;
    using Mapping;
    using UrbanSolution.Models;
   
    public class IssueMapInfoBoxDetailsServiceModel : IMapFrom<UrbanIssue>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, IssueMapInfoBoxDetailsServiceModel>()
                .ForMember(x => x.Latitude,m => m.MapFrom(u => u.Latitude.ToString().Replace(",", ".")))
                .ForMember(x => x.Longitude, m => m.MapFrom(u => u.Longitude.ToString().Replace(",", ".")));
        }
    }
}
