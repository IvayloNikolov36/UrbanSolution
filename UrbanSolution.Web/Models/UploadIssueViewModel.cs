using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrbanSolution.Web.Infrastructure;
using static UrbanSolution.Models.Utilities.DataConstants;

namespace UrbanSolution.Web.Models
{
    public class PublishIssueViewModel : IValidatableObject
    {
        [Required, StringLength(IssueTitleMaxLength, MinimumLength = IssueTitleMinLength)]
        [Display(Name = "Title")]
        public string Name { get; set; }

        [Required, StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        [Required, Url, Display(Name = "Picture Url")]
        public string PictureUrl { get; set; }

        [Required]
        [Display(Name = "Issue type")]
        public string IssueType { get; set; }

        public IEnumerable<SelectListItem> IssueTypes { get; set; }

        [Required]
        public string Region { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }

        [Required(ErrorMessage = WebConstants.NoAddressSet)]
        [StringLength(AddressMaxLength, MinimumLength = AddressMinLength)]
        public string Address { get; set; }

        [Required(ErrorMessage = WebConstants.MarkerNotPlaced)]
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Town { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!this.Town.Equals(WebConstants.CurrentTownEn) && !this.Town.Equals(WebConstants.CurrentTownBg))
            {
                yield return new ValidationResult($"The place should be in {WebConstants.CurrentTownEn}");
            }
        }
    }
}
