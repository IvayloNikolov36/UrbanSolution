using UrbanSolution.Models.Utilities;

namespace UrbanSolution.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Article
    {
        public int Id { get; set; }

        [Required]
        [StringLength(DataConstants.ArticleTitleMaxLength, MinimumLength = DataConstants.ArticleTitleMinLenght)]
        public string Title { get; set; }

        [Required]
        [MinLength(DataConstants.ArticleContentMinLength)]
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }

        [Url]
        public string PictureUrl { get; set; }
    }
}
