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
        public string Name { get; set; }

        [Required]
        public string  PublisherId { get; set; }
        public User Publisher { get; set; }

        [Required]
        [Url]
        public string IssuePictureUrl { get; set; }

        public DateTime PublishedOn { get; set; } 

        [Required]
        [StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        [StringLength(StreetNameMaxLength, MinimumLength = StreetNameMinLength)]
        public string AddressStreet { get; set; }

        [Required]
        public string StreetNumber { get; set; }

        public RegionType Region { get; set; }

        public IssueType Type { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsApproved { get; set; }

        public ResolvedIssue ResolvedIssue { get; set; }

    }
}
