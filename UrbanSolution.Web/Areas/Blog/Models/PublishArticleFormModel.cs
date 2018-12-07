namespace UrbanSolution.Web.Areas.Blog.Models
{
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models.Utilities;

    public class PublishArticleFormModel
    {
        [Required]
        [MinLength(DataConstants.ArticleTitleMinLenght)]
        [MaxLength(DataConstants.ArticleTitleMaxLength)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
