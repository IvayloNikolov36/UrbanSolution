namespace UrbanSolution.Web.Models.Comments
{
    using System;
    using AutoMapper;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;

    public class CommentListingModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public string Publisher { get; set; }

        public DateTime PostedOn { get; set; }

        public int ArticleId { get; set; }

        public string ArticleAuthor { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Comment, CommentListingModel>()
                .ForMember(x => x.Publisher, m => m.MapFrom(u => u.Publisher.UserName))
                .ForMember(x => x.ArticleAuthor, m => m.MapFrom(c => c.Article.Author.UserName));
        }
    }
}
