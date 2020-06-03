namespace UrbanSolution.Web.Models.Areas.Blog
{
    using UrbanSolution.Models;
    using UrbanSolution.Services.Mapping;
    using System.ComponentModel.DataAnnotations;
    using static UrbanSolution.Models.Utilities.DataConstants;

    public class EditArticleViewModel : IMapFrom<Article>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ArticleTitleMaxLength), MinLength(ArticleTitleMinLenght)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

    }
}
