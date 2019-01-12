namespace UrbanSolution.Services.Tests.Seed
{
    using System;

    public class EventCreator
    {
        private static int EventId;

        private const int DefaultImageId = 983739;

        public static UrbanSolution.Models.Event Create(string userId, int? imageId)
        {
            var eventObj = new UrbanSolution.Models.Event
            {
                Id = ++EventId,
                CloudinaryImageId = imageId ?? DefaultImageId,
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(4),
                CreatorId = userId,
                Address = Guid.NewGuid().ToString(),
            };

            return eventObj;
        }
    }
}
