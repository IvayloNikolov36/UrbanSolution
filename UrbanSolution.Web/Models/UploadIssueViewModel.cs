using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using static UrbanSolution.Models.Utilities.DataConstants;

namespace UrbanSolution.Web.Models
{
    public class UploadIssueViewModel
    {
        [Required]
        [StringLength(IssueNameMaxLength, MinimumLength = IssueNameMinLength)]
        [Display(Name = "Issue Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        [Required, Url, Display(Name = "Picture Url")]
        public string PictureUrl { get; set; }

        [Required]
        public string IssueType { get; set; }

        public IEnumerable<SelectListItem> IssueTypes { get; set; }

        [Required]
        public string Region { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }

        [Required, Display(Name = "Street Name")]
        [StringLength(StreetNameMaxLength, MinimumLength = StreetNameMinLength)]
        public string AddressStreet { get; set; }

        [Required, Display(Name = "Street Number")]
        [RegularExpression(@"\b(\d{1,3}[A-Za-z]?-\d{1,3}[A-Za-z]?\b)|(\b\d{1,3}[A-Za-z]?)\b")]
        public string StreetNumber { get; set; }

        [Required(ErrorMessage = "Please move and place the marker to take coordinates.")]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }
        
    }
}
