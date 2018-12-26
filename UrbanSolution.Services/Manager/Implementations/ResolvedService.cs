
namespace UrbanSolution.Services.Manager.Implementations
{
    using Data;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class ResolvedService : IResolvedService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IPictureService pictureService;
        private readonly IManagerActivityService activity;

        public ResolvedService(UrbanSolutionDbContext db, IPictureService pictureService, IManagerActivityService activity)
        {
            this.db = db;
            this.pictureService = pictureService;
            this.activity = activity;
        }

        public async Task<int> UploadAsync(string managerId, int issueId, IFormFile pictureFile, string description)
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
    }
}
