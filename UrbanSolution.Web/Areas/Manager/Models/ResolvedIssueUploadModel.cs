using System.ComponentModel.DataAnnotations;
using UrbanSolution.Models.Utilities;

namespace UrbanSolution.Web.Areas.Manager.Models
{
    public class ResolvedIssueUploadModel
    {
        [Range(1, int.MaxValue)]
        public int UrbanIssueId { get; set; } //TODO: check if it is real urbanIssueId

        [Required, Url]
        public string PictureUrl { get; set; }

        [Required]
        [StringLength(DataConstants.IssueDescriptionMaxLength, MinimumLength = DataConstants.IssueDescriptionMinLength)]
        public string Description { get; set; }
    }
}
