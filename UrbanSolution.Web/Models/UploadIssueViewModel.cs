namespace UrbanSolution.Web.Models
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static UrbanSolution.Models.Utilities.DataConstants;
    using static UrbanSolutionUtilities.WebConstants;
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

        [Required(ErrorMessage = NoAddressSet)]
        [StringLength(AddressMaxLength, MinimumLength = AddressMinLength)]
        public string Address { get; set; }

        [Required(ErrorMessage = MarkerNotPlaced)]
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Town { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!this.Town.Equals(CurrentTownEn) && !this.Town.Equals(CurrentTownBg))
            {
                yield return new ValidationResult($"The place should be in {CurrentTownEn}");
            }

            if (!this.PictureFile.FileName.EndsWith(PictureExtension)
                || PictureFile.Length > PictureUploadFileLength)
            {
                yield return new ValidationResult(MessageForImageUploadingRestrictions);
            }
        }
    }
}
