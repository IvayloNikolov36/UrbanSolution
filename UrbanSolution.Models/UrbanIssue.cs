using System;
using System.ComponentModel.DataAnnotations;
using UrbanSolution.Models.Utilities;
using static UrbanSolution.Models.Utilities.DataConstants;

namespace UrbanSolution.Models
{
    public class UrbanIssue
    {
        public int Id { get; set; }

        [Required]
        [StringLength(DataConstants.IssueTitleMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Title { get; set; }

        [Required]
        public string  PublisherId { get; set; }
        public User Publisher { get; set; }

        public int CloudinaryImageId { get; set; }

        public CloudinaryImage CloudinaryImage { get; set; }

        public DateTime PublishedOn { get; set; } 

        [Required]
        [StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        [StringLength(AddressMaxLength, MinimumLength = AddressMinLength)]
        public string AddressStreet { get; set; }

        public RegionType Region { get; set; }

        public IssueType Type { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsApproved { get; set; }

        public ResolvedIssue ResolvedIssue { get; set; }

    }
}
