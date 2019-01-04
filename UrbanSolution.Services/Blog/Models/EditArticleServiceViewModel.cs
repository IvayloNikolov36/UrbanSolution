namespace UrbanSolution.Services.Blog.Models
{
    using Mapping;
    using UrbanSolution.Models;
    using System.ComponentModel.DataAnnotations;
    using static UrbanSolution.Models.Utilities.DataConstants;

    public class EditArticleServiceViewModel : IMapFrom<Article>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ArticleTitleMaxLength), MinLength(ArticleTitleMinLenght)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

    }
}
