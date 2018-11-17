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

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<UrbanIssue> UrbanIssues { get; set; } = new List<UrbanIssue>();

        public ICollection<ResolvedIssue> ResolvedIssues { get; set; } = new List<ResolvedIssue>();

    }
}
