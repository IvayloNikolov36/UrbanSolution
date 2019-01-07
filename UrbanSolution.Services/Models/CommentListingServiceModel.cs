using System;
using AutoMapper;
using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Models
{
    public class CommentListingServiceModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string Publisher { get; set; }

        public DateTime PostedOn { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Comment, CommentListingServiceModel>()
                .ForMember(x => x.Publisher, m => m.MapFrom(u => u.Publisher.UserName));
        }
    }
}
