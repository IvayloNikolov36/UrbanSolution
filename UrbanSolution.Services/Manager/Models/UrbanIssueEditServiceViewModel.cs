using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using UrbanSolution.Models;

namespace UrbanSolution.Services.Manager.Models
{
    public class UrbanIssueEditServiceViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Publisher { get; set; }

        [Display(Name = "Picture Url")]
        public string IssuePictureUrl { get; set; }

        [Display(Name = "Published on")]
        public DateTime PublishedOn { get; set; }

        public string Description { get; set; }

        [Display(Name = "Street")]
        public string AddressStreet { get; set; }
        
        [Display(Name = "Address number")]
        [RegularExpression(@"\b(\d{1,3}[A-Za-z]?-\d{1,3}[A-Za-z]?\b)|(\b\d{1,3}[A-Za-z]?)\b")]
        public string StreetNumber { get; set; }

        public RegionType Region { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }

        public IEnumerable<SelectListItem> IssueTypes { get; set; }

        public IssueType Type { get; set; }

        public bool IsApproved { get; set; }

    }
}
