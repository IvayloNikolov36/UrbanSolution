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
    using UrbanSolution.Services.Models;

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
            var issue = await this.db
                .FindAsync<UrbanIssue>(id);

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
            
            await this.db.SaveChangesAsync();

            await this.activity.WriteManagerLogInfoAsync(manager.Id, ManagerActivityType.EditedIssue);

            return true;
        }

        public async Task<bool> DeleteAsync(User manager, int issueId)
        {
            var issue = await this.db.FindAsync<UrbanIssue>(issueId);

            var canDelete =  issue.Region == manager.ManagedRegion || manager.ManagedRegion == RegionType.All; 

            if (!canDelete)
            {
                return false;
            }

            var pictureId = issue.CloudinaryImageId;

            //First delete urbanIssue, than the image
            this.db.UrbanIssues.Remove(issue);

            await this.db.SaveChangesAsync();

            await this.activity.WriteManagerLogInfoAsync(manager.Id, ManagerActivityType.DeletedIssue);

            await this.pictureService.DeleteImageAsync(pictureId);

            return true;
        }

        public async Task<bool> ApproveAsync(User manager, int issueId)
        {
            var issue = await this.db.FindAsync<UrbanIssue>(issueId);

            var canApprove = issue.Region == manager.ManagedRegion || manager.ManagedRegion == RegionType.All;

            if (!canApprove)
            {
                return false;
            }
            
            issue.IsApproved = true;

            await this.db.SaveChangesAsync();

            await this.activity.WriteManagerLogInfoAsync(manager.Id, ManagerActivityType.ApprovedIssue);

            return true;
        }

        public async Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(
            bool isApproved, RegionType? region)
        {
            bool takeAllRegions = region == RegionType.All;

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

        public async Task RemoveResolvedReferenceAsync(int issueId)
        {
            var issueToUpdate = await this.db.FindAsync<UrbanIssue>(issueId);

            issueToUpdate.ResolvedIssue = null;

            await this.db.SaveChangesAsync();
        }

    }
}
