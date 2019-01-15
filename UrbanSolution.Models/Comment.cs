namespace UrbanSolution.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static Utilities.DataConstants;

    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CommentContentMaxLength, MinimumLength = CommentContentMinLength)]
        public string Content { get; set; }

        public DateTime PostedOn { get; set; }

        [Required]
        public string PublisherId { get; set; }

        public User Publisher { get; set; }

        public int ArticleId { get; set; }

        public Article Article { get; set; }
    }
}
