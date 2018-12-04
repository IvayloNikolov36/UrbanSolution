using System;
using System.Threading.Tasks;
using UrbanSolution.Data;
using UrbanSolution.Models;

namespace UrbanSolution.Services.Manager.Implementations
{
    public class ResolvedService : IResolvedService
    {
        private readonly UrbanSolutionDbContext db;

        public ResolvedService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<int> UploadAsync(string publisherId, int issueId, string pictureUrl, string description)
        {
            var resolvedIssue = new ResolvedIssue
            {
                PictureUrl = pictureUrl,
                Description = description,
                PublisherId = publisherId,
                Rating = 0,
                ResolvedOn = DateTime.UtcNow,
                UrbanIssueId = issueId
            };

            this.db.ResolvedIssues.Add(resolvedIssue);

            await this.db.SaveChangesAsync();

            return resolvedIssue.Id;
        }
    }
}
