namespace UrbanSolution.Services.Blog.Models
{
    using AutoMapper;
    using Mapping;
    using System;
    using UrbanSolution.Models;

    public class BlogArticleListingServiceModel : IMapFrom<Article>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration
                .CreateMap<Article, BlogArticleListingServiceModel>()
                .ForMember(a => a.Author, m => m.MapFrom(a => a.Author.UserName));
        }
    }
}
