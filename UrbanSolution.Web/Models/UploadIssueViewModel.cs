using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using static UrbanSolution.Models.Utilities.DataConstants;

namespace UrbanSolution.Web.Models
{
    public class UploadIssueViewModel
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

        [Required]
        [StringLength(StreetNameMaxLength, MinimumLength = StreetNameMinLength)]
        [Display(Name = "Street name")]
        public string AddressStreet { get; set; }

        [Required]
        [Display(Name = "Street number")]
        public string StreetNumber { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }
        
    }
}
