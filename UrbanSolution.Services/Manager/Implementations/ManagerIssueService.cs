namespace UrbanSolution.Services.Manager.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    public class ManagerIssueService : IManagerIssueService
    {
        private readonly UrbanSolutionDbContext db;

        public ManagerIssueService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, RegionType? region)
        {
            bool takeAllRegions = region == null;

            var issues = this.db
                .UrbanIssues
                .Where(i => i.IsApproved == isApproved);

            if (!takeAllRegions)
            {
                issues = issues.Where(i => i.Region == region);
            }

            var result = await issues
                .To<UrbanIssuesListingServiceModel>()
                .ToListAsync();

            return result;
        }

        public async Task<TModel> GetAsync<TModel>(int issueId)
        {
            var issueModel = await this.db.UrbanIssues
                .Where(i => i.Id == issueId)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return issueModel;
        }

        public async Task<int> TotalAsync(bool isApproved)
        {
            var total = await this.db
                .UrbanIssues
                .Where(i => i.IsApproved == isApproved)
                .CountAsync();

            return total;
        }

        public async Task UpdateAsync(int id, string title, string description, RegionType region, IssueType type, string street)
        {
            var issue = await this.db
                .FindAsync<UrbanIssue>(id);

            issue.Title = title;           
            issue.Description = description;
            issue.Region = region;
            issue.Type = type;

            await this.db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int issueId)
        {
            var exists = await this.db
                .UrbanIssues
                .AnyAsync(i => i.Id == issueId);

            return exists;
        }

        public async Task ApproveAsync(int issueId)
        {
            var issueFromDb = await this.db.FindAsync<UrbanIssue>(issueId);

            issueFromDb.IsApproved = true;

            await this.db.SaveChangesAsync();
        }

        public async Task<bool> IsIssueInSameRegionAsync(int issueId, RegionType? managerRegion)
        {
            var issue = await this.GetAsync<IssueRegionServiceModel>(issueId);
            var issueRegion = issue.Region;
            if (managerRegion == null)
            {
                return true;
            }

            if (issueRegion != managerRegion)
            {
                return false;
            }

            return true;
        }

        public async Task DeleteAsync(int issueId)
        {
            var issueToDelete = await this.db.FindAsync<UrbanIssue>(issueId);

            this.db.UrbanIssues.Remove(issueToDelete);

            await this.db.SaveChangesAsync();
        }
    }
}
