namespace UrbanSolution.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static Utilities.DataConstants;

    public class Article
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ArticleTitleMaxLength, MinimumLength = ArticleTitleMinLenght)]
        public string Title { get; set; }

        public int CloudinaryImageId { get; set; }

        public CloudinaryImage CloudinaryImage { get; set; }

        [Required]
        [MinLength(ArticleContentMinLength)]
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }

    }
}
