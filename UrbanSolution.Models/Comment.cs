using System;
using System.ComponentModel.DataAnnotations;
using static UrbanSolution.Models.Utilities.DataConstants;

namespace UrbanSolution.Models
{
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

        public int? TargetId { get; set; }
        public ResolvedIssue Target { get; set; }

    }
}
