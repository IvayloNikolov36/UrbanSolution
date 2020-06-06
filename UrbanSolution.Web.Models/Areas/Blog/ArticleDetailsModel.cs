namespace UrbanSolution.Web.Models.Areas.Blog
{
    using AutoMapper;
    using UrbanSolution.Services.Mapping;
    using System;
    using System.Linq;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class ArticleDetailsModel : IMapFrom<Article>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string PictureUrl { get; set; }

        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        public bool HasComments { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Article, ArticleDetailsModel>()
                .ForMember(x => x.Author, m => m.MapFrom(a => a.Author.UserName))
                .ForMember(x => x.PictureUrl, m => m
                    .MapFrom(a => CloudPicUrlPrefix + a.CloudinaryImage.PictureUrl))
                .ForMember(x => x.HasComments, m => m.MapFrom(a => a.Comments.Any()));
        }
    }
}
