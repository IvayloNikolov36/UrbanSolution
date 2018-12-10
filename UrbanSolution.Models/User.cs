namespace UrbanSolution.Models
{
    using MappingTables;
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using static Utilities.DataConstants;

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

        public ICollection<Event> EventsCreated { get; set; } = new List<Event>();

        public ICollection<EventUser> EventsParticipations { get; set; } = new List<EventUser>();

    }
}
