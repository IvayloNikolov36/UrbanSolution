namespace UrbanSolution.Services.Implementations
{
    using Data;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public class UserIssuesService : IUserIssuesService
    {
        private readonly UrbanSolutionDbContext db;

        public UserIssuesService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task UploadAsync(string userId, string title, string description, int cloudinaryImageId, string issueType, string region, string address, double latitude, double longitude)
        {
            var issue = new UrbanIssue
            {
                Title = title,
                Description = description,
                CloudinaryImageId = cloudinaryImageId,
                Type = Enum.Parse<IssueType>(issueType),
                Region = Enum.Parse<RegionType>(region),
                PublishedOn = DateTime.UtcNow,
                PublisherId = userId,
                AddressStreet = address,
                Latitude = latitude,
                Longitude = longitude
            };

            this.db.Add(issue);

            await this.db.SaveChangesAsync();
        }

    }
}
