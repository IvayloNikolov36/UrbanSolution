namespace UrbanSolution.Web.Models.Areas.Events
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static UrbanSolution.Models.Utilities.DataConstants;
    using static UrbanSolutionUtilities.WebConstants;

    public class EventCreateInputModel : IValidatableObject
    {        
        [Required]
        [StringLength(EventTitleMaxLength, MinimumLength = EventTitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(EventDescriptionMaxLength, MinimumLength = EventDescriptionMinLength)]
        public string Description { get; set; }

        [Display(Name = "Starting")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ending")]
        public DateTime EndDate { get; set; }

        [Required]
        public IFormFile PictureFile { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.StartDate <= DateTime.UtcNow.AddDays(1))
            {
                yield return new ValidationResult(EventStartDateRestriction);
            }

            if (this.EndDate < this.StartDate.AddHours(1))
            {
                yield return new ValidationResult(EventEndDateRestriction);
            }

            if (string.IsNullOrEmpty(this.Latitude))
            {
                yield return new ValidationResult(NoCoordinatesValidationError);
            }
        }
    }
}
