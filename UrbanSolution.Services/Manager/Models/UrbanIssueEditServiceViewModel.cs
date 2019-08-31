namespace UrbanSolution.Services.Manager.Models
{
    using Microsoft.AspNetCore.Http;
    using AutoMapper;
    using Mapping;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models;
    using static UrbanSolution.Models.Utilities.DataConstants;
    using static UrbanSolutionUtilities.WebConstants;

    public class UrbanIssueEditServiceViewModel : IMapFrom<UrbanIssue>, IHaveCustomMappings, IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        [StringLength(IssueTitleMaxLength, MinimumLength = IssueTitleMinLength)]
        public string Title { get; set; }

        public IFormFile PictureFile { get; set; }

        public string Publisher { get; set; }

        [Display(Name = "Published on")]
        public DateTime PublishedOn { get; set; }

        [Required]
        [StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        [Display(Name = "Street")]
        [Required]
        [StringLength(AddressMaxLength, MinimumLength = AddressMinLength)]
        public string AddressStreet { get; set; }

        public RegionType Region { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }

        public IEnumerable<SelectListItem> IssueTypes { get; set; }

        public IssueType Type { get; set; }

        public bool IsApproved { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<UrbanIssue, UrbanIssueEditServiceViewModel>()
                .ForMember(x => x.Publisher, m => m.MapFrom(u => u.Publisher.UserName));
        }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            if (this.PictureFile != null)
            {
                //TODO: complete the validation for extension type
                if (!PictureFile.FileName.EndsWith(".jpg")
                    || PictureFile.Length > PictureUploadFileLength)
                {
                    yield return new ValidationResult(MessageForImageUploadingRestrictions);
                }
            }
            
        }
    }
}
