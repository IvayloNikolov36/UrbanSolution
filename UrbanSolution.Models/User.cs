using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static UrbanSolution.Models.Utilities.DataConstants;

namespace UrbanSolution.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(UserFullNameMaxLength, MinimumLength = UserFullNameMinLength)]                          
        public string FullName { get; set; }      

        [Range(UserMinAge, UserMaxAge)]
        public int Age { get; set; }

        public RegionType? ManagedRegion { get; set; }

        public ICollection<UrbanIssue> UrbanIssues { get; set; } = new List<UrbanIssue>();

        public ICollection<ResolvedIssue> ResolvedIssues { get; set; } = new List<ResolvedIssue>();

        public ICollection<Article> PublishedArticles { get; set; } = new List<Article>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
