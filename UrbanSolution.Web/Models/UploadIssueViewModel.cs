using UrbanSolution.Services.Utilities;

namespace UrbanSolution.Web.Models
{
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static UrbanSolution.Models.Utilities.DataConstants;

    public class PublishIssueViewModel : IValidatableObject
    {
        [Required, StringLength(IssueTitleMaxLength, MinimumLength = IssueTitleMinLength)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required, StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Picture file")]
        public IFormFile PictureFile { get; set; }

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

            if (!this.PictureFile.FileName.EndsWith(ServiceConstants.PictureExtension)
                || PictureFile.Length > ServiceConstants.PictureUploadFileLength)
            {
                yield return new ValidationResult(ServiceConstants.MessageForImageUploadingRestrictions);
            }
        }
    }
}
