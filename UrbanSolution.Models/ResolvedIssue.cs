using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static UrbanSolution.Models.Utilities.DataConstants;

namespace UrbanSolution.Models
{
    public class ResolvedIssue
    {
        public int Id { get; set; }

        [Required]
        public string PublisherId { get; set; }
        public User Publisher { get; set; }

        public DateTime ResolvedOn { get; set; }

        [Required]
        [Url]
        public string PictureUrl { get; set; }

        [Required]
        [StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        public double? Evaluation { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
