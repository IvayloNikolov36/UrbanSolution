namespace UrbanSolution.Web.Models.Areas.Manager
{
    using UrbanSolution.Services.Mapping;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;
    using UrbanSolution.Models;
    using static UrbanSolution.Models.Utilities.DataConstants;

    public class ResolvedIssueEditModel : IMapFrom<ResolvedIssue>
    {
        public int Id { get; set; }

        public int CloudinaryImageId { get; set; }

        [Required, StringLength(IssueDescriptionMaxLength, MinimumLength = IssueDescriptionMinLength)]
        public string Description { get; set; }

        public IFormFile PictureFile { get; set; }

    }
}
