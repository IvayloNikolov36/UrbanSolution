namespace UrbanSolution.Web.Models.Areas.Blog
{
    using AutoMapper;
    using System;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;
    using static UrbanSolutionUtilities.WebConstants;

    public class BlogArticleListingModel : IMapFrom<Article>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string PictureUrl { get; set; }

        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration
                .CreateMap<Article, BlogArticleListingModel>()
                .ForMember(a => a.Author, m => m.MapFrom(a => a.Author.UserName))
                .ForMember(a => a.PictureUrl, m => m
                    .MapFrom(a => CloudPicUrlPrefix + a.CloudinaryImage.PictureUrl));
        }
    }
}
