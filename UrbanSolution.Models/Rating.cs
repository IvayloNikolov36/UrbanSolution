using System.ComponentModel.DataAnnotations;

namespace UrbanSolution.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int ResolvedIssueId { get; set; }
        public ResolvedIssue ResolvedIssue { get; set; }

        [Range(0, 6)]
        public int Value { get; set; }

    }
}
