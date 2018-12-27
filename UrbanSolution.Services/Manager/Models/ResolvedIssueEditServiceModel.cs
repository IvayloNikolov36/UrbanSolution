
namespace UrbanSolution.Services.Manager.Models
{
    using Mapping;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models;
    using static UrbanSolution.Models.Utilities.DataConstants;

    public class ResolvedIssueEditServiceModel : IMapFrom<ResolvedIssue>
    {
        public int Id { get; set; }

        public int CloudinaryImageId { get; set; }

        [Required, StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        public IFormFile PictureFile { get; set; }

    }
}
