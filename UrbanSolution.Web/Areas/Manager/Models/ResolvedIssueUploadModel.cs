namespace UrbanSolution.Web.Areas.Manager.Models
{
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models.Utilities;

    public class ResolvedIssueUploadModel : IValidatableObject
    {
        [Range(1, int.MaxValue)]
        public int UrbanIssueId { get; set; } //TODO: check if it is real urbanIssueId

        [Required]
        public IFormFile PictureFile { get; set; }

        [Required]
        [StringLength(DataConstants.IssueDescriptionMaxLength, MinimumLength = DataConstants.IssueDescriptionMinLength)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {            
            if (!this.PictureFile.FileName.EndsWith(".jpg")
                || PictureFile.Length > WebConstants.PictureUploadFileLength)
            {
                yield return new ValidationResult("Your file submission should be a '.jpg' file with no more than 5.5mb size");
            }
          
        }
    }
}
