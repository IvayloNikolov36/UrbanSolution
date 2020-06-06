namespace UrbanSolution.Web.Models.Areas.Manager
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models.Utilities;
    using static UrbanSolutionUtilities.WebConstants;

    public class ResolvedIssueUploadModel : IValidatableObject
    {
        [Range(1, int.MaxValue)]
        public int UrbanIssueId { get; set; }

        [Required]
        public IFormFile PictureFile { get; set; }

        [Required]
        [StringLength(DataConstants.IssueDescriptionMaxLength, MinimumLength = DataConstants.IssueDescriptionMinLength)]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {            
            //TOO: extend validation to pass other picture files too
            if (!this.PictureFile.FileName.EndsWith(".jpg")
                || PictureFile.Length > PictureUploadFileLength)
            {
                yield return new ValidationResult("Your file submission should be a '.jpg' file with no more than 5.5mb size");
            }
          
        }
    }
}
