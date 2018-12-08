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

        public async Task UploadAsync(string userId, string name, string description, string pictureUrl, string issueType, string region,
            string addressStreet, string streetNumber, double latitude, double longitude)
        {
            var issue = new UrbanIssue
            {
                Name = name,
                Description = description,
                IssuePictureUrl = pictureUrl,
                Type = Enum.Parse<IssueType>(issueType),
                Region = Enum.Parse<RegionType>(region),
                PublishedOn = DateTime.UtcNow,
                PublisherId = userId,
                AddressStreet = addressStreet,
                StreetNumber = streetNumber,
                Latitude = latitude,
                Longitude = longitude
            };

            this.db.Add(issue);

            await this.db.SaveChangesAsync();
        }
    }
}
