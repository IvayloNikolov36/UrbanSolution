namespace UrbanSolution.Services.Tests.Seed
{
    using System;
    using UrbanSolution.Models;

    public class UrbanIssueCreator
    {
        private static int issueId;
        private static string issueTitle = "Title";

        public static UrbanIssue CreateIssue(RegionType region)
        {
            return new UrbanIssue
            {
                Id = ++issueId,
                Region = region
            };
        }

        public static UrbanIssue CreateIssue(RegionType region, int imageId)
        {
            return new UrbanIssue
            {
                Id = ++issueId,
                Region = region,
                CloudinaryImageId = imageId
            };
        }

        public static UrbanIssue CreateIssue(bool isApproved, RegionType region, string publisherId, int picId)
        {
            return new UrbanIssue
            {
                Id = ++issueId,
                Title = string.Format(issueTitle, issueId),
                PublishedOn = DateTime.UtcNow,
                Latitude = 42D,
                Longitude = 42D,
                Region = region,
                IsApproved = isApproved,
                PublisherId = publisherId,
                CloudinaryImageId = picId
            };
        }
    }
}
