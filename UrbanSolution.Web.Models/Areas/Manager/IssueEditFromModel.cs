namespace UrbanSolution.Web.Models.Areas.Manager
{
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Utilities;
    using static UrbanSolution.Models.Utilities.DataConstants;

    public class IssueEditFromModel
    {
        [Required]
        [StringLength(DataConstants.IssueTitleMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        public RegionType Region { get; set; }

        public IssueType Type { get; set; }

        [Required]
        [Url]
        public string IssuePictureUrl { get; set; }

        public string AddressStreet { get; set; }

        public string StreetNumber { get; set; }

    }
}
