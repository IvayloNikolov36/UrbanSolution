namespace UrbanSolution.Web.Areas.Events.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static UrbanSolution.Models.Utilities.DataConstants;

    public class EventCreateFormModel : IValidatableObject
    {        
        [Required]
        [StringLength(EventTitleMaxLength, MinimumLength = EventTitleMinLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(EventDescriptionMaxLength, MinimumLength = EventTitleMinLength)]
        public string Description { get; set; }

        [Display(Name = "Starting")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ending")]
        public DateTime EndDate { get; set; }

        [Url]
        public string PictureUrl { get; set; }

        [Required]
        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.StartDate <= DateTime.UtcNow.AddDays(1))
            {
                yield return new ValidationResult("Start date of the event should be at least one day after creation!");
            }

            if (this.EndDate <= this.StartDate.AddHours(1))
            {
                yield return new ValidationResult("End date of the event should be at least one hour after start time!");
            }
        }
    }
}
