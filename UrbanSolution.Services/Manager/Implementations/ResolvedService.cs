namespace UrbanSolution.Services.Manager.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class ResolvedService : IResolvedService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IManagerIssueService issues;
        private readonly IPictureService pictureService;
        private readonly IManagerActivityService activity;

        public ResolvedService(
            UrbanSolutionDbContext db, 
            IManagerIssueService issues, 
            IPictureService pictureService, 
            IManagerActivityService activity)
        {
            this.db = db;
            this.issues = issues;
            this.pictureService = pictureService;
            this.activity = activity;
        }

        public async Task<int> UploadAsync(
            string managerId, int issueId, IFormFile pictureFile, string description)
        {
            var picId = await this.pictureService.UploadImageAsync(managerId, pictureFile);

            var resolvedIssue = new ResolvedIssue
            {
                PublisherId = managerId,
                UrbanIssueId = issueId,
                CloudinaryImageId = picId,
                Description = description,                
                Rating = 0,
                ResolvedOn = DateTime.UtcNow               
            };

            this.db.ResolvedIssues.Add(resolvedIssue);

            await this.db.SaveChangesAsync();

            await this.activity.WriteManagerLogInfoAsync(managerId, ManagerActivityType.UploadedResolved);

            return resolvedIssue.Id;
        }

        public async Task<bool> DeleteAsync(string managerId, int resolvedId)
        {
            var resolvedIssue = await this.db
                .ResolvedIssues
                .Include(r => r.UrbanIssue)
                .FirstOrDefaultAsync(r => r.Id == resolvedId);

            var publisherId = resolvedIssue.PublisherId;

            //manager can't delete others managers published resolved issues
            if (managerId != publisherId)
            {
                return false;
            }

            var issue = resolvedIssue.UrbanIssue; 
            
            await this.issues.RemoveResolvedReferenceAsync(issue.Id); //sets UrbanIssue Field "ResolvedIssue" to null

            var pictureId = resolvedIssue.CloudinaryImageId;          

            this.db.ResolvedIssues.Remove(resolvedIssue);

            await this.db.SaveChangesAsync();

            await this.pictureService.DeleteImageAsync(pictureId); //removes the picture from cloudinary and pictureInfo from DB 

            await this.activity.WriteManagerLogInfoAsync(managerId, ManagerActivityType.RemovedResolved);

            return true;
        }

        public async Task<TModel> GetAsync<TModel>(int id)
        {
            var resolvedModel = await this.db.
                ResolvedIssues
                .Where(r => r.Id == id)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return resolvedModel;
        }

        public async Task<bool> UpdateAsync(string managerId, int id, string description, IFormFile pictureFile)
        {
            var resolvedFromDb = await this.db.FindAsync<ResolvedIssue>(id);

            var publisherId = resolvedFromDb.PublisherId;
            
            if (managerId != publisherId) //manager can't delete others managers resolved issues
            {
                return false;
            }
                       
            if (pictureFile != null) //have to change the cloudinaryImage
            {
                var imageToDelete = resolvedFromDb.CloudinaryImageId;

                var pictureId = await this.pictureService.UploadImageAsync(managerId, pictureFile);

                resolvedFromDb.CloudinaryImageId = pictureId;

                await this.pictureService.DeleteImageAsync(imageToDelete);
            }

            resolvedFromDb.Description = description;

            await this.db.SaveChangesAsync();

            await this.activity.WriteManagerLogInfoAsync(managerId, ManagerActivityType.UpdatedResolved);

            return true;
        }
        
    }
}
