namespace UrbanSolution.Services.Blog.Models
{
    using AutoMapper;
    using Mapping;
    using System;
    using UrbanSolution.Models;

    public class BlogArticleDetailsServiceModel : IMapFrom<Article>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string PictureUrl { get; set; }

        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Article, BlogArticleDetailsServiceModel>()
                .ForMember(x => x.Author, m => m.MapFrom(a => a.Author.UserName))
                .ForMember(x => x.PictureUrl, m => m.MapFrom(a => a.CloudinaryImage.PictureUrl));
        }
    }
}
