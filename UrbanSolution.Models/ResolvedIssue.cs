namespace UrbanSolution.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static Utilities.DataConstants;

    public class ResolvedIssue
    {
        public int Id { get; set; }

        public int? UrbanIssueId { get; set; }
        public UrbanIssue UrbanIssue { get; set; }

        [Required]
        public string PublisherId { get; set; }
        public User Publisher { get; set; }

        public DateTime ResolvedOn { get; set; }

        public int CloudinaryImageId { get; set; }

        public CloudinaryImage CloudinaryImage { get; set; }

        [Required, StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        public double Rating { get; set; }

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    }
}
