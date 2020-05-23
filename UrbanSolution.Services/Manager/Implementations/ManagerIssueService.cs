namespace UrbanSolution.Services.Manager.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class ManagerIssueService : IManagerIssueService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IPictureService pictureService;
        private readonly IManagerActivityService activity;

        public ManagerIssueService(
            UrbanSolutionDbContext db, 
            IPictureService pictureService, 
            IManagerActivityService activity)
        {
            this.db = db;
            this.pictureService = pictureService;
            this.activity = activity;
        }

        public async Task<bool> UpdateAsync(
            User manager, int id, string title, string description, 
            RegionType region, IssueType type, string street, IFormFile pictureFile)
        {
            var issue = await this.db.FindAsync<UrbanIssue>(id);

            var canUpdate = issue.Region == manager.ManagedRegion 
                            || manager.ManagedRegion == RegionType.All;

            if (!canUpdate)
            {
                return false;
            }

            var oldPictureId = issue.CloudinaryImageId;

            if (pictureFile != null)
            {
                var pictureId = await this.pictureService.UploadImageAsync(manager.Id, pictureFile);
                issue.CloudinaryImageId = pictureId;

                await this.pictureService.DeleteImageAsync(oldPictureId);
            }

            issue.Title = title;
            issue.Description = description;
            issue.Region = region;
            issue.Type = type;
            issue.AddressStreet = street;
            
            await this.db.SaveChangesAsync();

            await this.activity.WriteLogAsync(manager.Id, ManagerActivityType.EditedIssue);

            return true;
        }

        public async Task<bool> DeleteAsync(string managerId, RegionType? managerRegion, int issueId)
        {
            var issue = await this.db.FindAsync<UrbanIssue>(issueId);
            if (issue == null)
            {
                return false;
            }
            
            var canDelete =  issue.Region == managerRegion
                || managerRegion == RegionType.All; 
            if (!canDelete)
            {
                return false;
            }

            var pictureId = issue.CloudinaryImageId;

            //First delete urbanIssue, than the image
            this.db.UrbanIssues.Remove(issue);

            await this.db.SaveChangesAsync();

            await this.activity.WriteLogAsync(managerId, ManagerActivityType.DeletedIssue);

            await this.pictureService.DeleteImageAsync(pictureId);

            return true;
        }

        public async Task<bool> ApproveAsync(User manager, int issueId)
        {
            var issue = await this.db.FindAsync<UrbanIssue>(issueId);
            if (issue == null)
            {
                return false;
            }

            var canApprove = issue.Region == manager.ManagedRegion 
                || manager.ManagedRegion == RegionType.All;
            if (!canApprove)
            {
                return false;
            }
            
            issue.IsApproved = true;
            await this.db.SaveChangesAsync();

            await this.activity.WriteLogAsync(manager.Id, ManagerActivityType.ApprovedIssue);

            return true;
        }

        public async Task<(int, IEnumerable<TModel>)> AllAsync<TModel>(
            bool isApproved, RegionType? region, int page, int takeCount)
        {
            bool takeAllRegions = region == RegionType.All;

            var issues = this.db
                .UrbanIssues.AsNoTracking()
                .Where(i => i.IsApproved == isApproved);

            if (!takeAllRegions)
            {
                issues = issues.Where(i => i.Region == region);
            }

            var filteredCount = await issues.CountAsync();

            var issuesModel = await issues
                .OrderByDescending(i => i.PublishedOn)
                .Skip((page - 1) * takeCount)
                .Take(takeCount)
                .To<TModel>()
                .ToListAsync();

            return (filteredCount, issuesModel);
        }

        public async Task RemoveResolvedReferenceAsync(int issueId)
        {
            var issueToUpdate = await this.db.FindAsync<UrbanIssue>(issueId);

            issueToUpdate.ResolvedIssue = null;

            await this.db.SaveChangesAsync();
        }

    }
}
