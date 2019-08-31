namespace UrbanSolution.Services.Tests.Seed
{
    using System;
    using UrbanSolution.Models.MappingTables;

    public class EventCreator
    {
        private static int EventId;

        private const int DefaultImageId = 983739;

        public static UrbanSolution.Models.Event Create(string userId = null, int? imageId = null)
        {
            var eventObj = new UrbanSolution.Models.Event
            {
                Id = ++EventId,
                CloudinaryImageId = imageId ?? DefaultImageId,
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(4),
                CreatorId = userId ?? Guid.NewGuid().ToString(),
                Address = Guid.NewGuid().ToString(),
            };

            return eventObj;
        }

        public static UrbanSolution.Models.Event CreateEvent(int id, string description)
        {
            var eventObj = new UrbanSolution.Models.Event
            {
                Id = id,
                Title = Guid.NewGuid().ToString(),
                Description = description,
                Address = Guid.NewGuid().ToString(),
            };

            return eventObj;
        }

        public static EventUser CreateEventUser(int eventId, string userId)
        {
            return new EventUser
            {
                EventId = eventId,
                ParticipantId = userId
            };
        }
    }
}
