namespace UrbanSolution.Web.Models.Areas.Blog
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models.Utilities;

    public class PublishArticleInputModel
    {
        [Required]
        [MinLength(DataConstants.ArticleTitleMinLenght)]
        [MaxLength(DataConstants.ArticleTitleMaxLength)]
        public string Title { get; set; }

        [Required]
        public IFormFile PictureFile { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
