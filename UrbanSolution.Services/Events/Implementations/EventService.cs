namespace UrbanSolution.Services.Events.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.MappingTables;
    using static UrbanSolutionUtilities.WebConstants;

    public class EventService : IEventService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IPictureService pictureService;

        public EventService(UrbanSolutionDbContext db, IPictureService pictureService)
        {
            this.db = db;
            this.pictureService = pictureService;
        }

        public async Task<IEnumerable<TModel>> AllAsync<TModel>(int page = 1)
        {
            var eventsModel = await this.db
                .Events
                .OrderByDescending(e => e.Id)
                .Skip((page - 1) * EventsPageSize)
                .Take(EventsPageSize)
                .To<TModel>()
                .ToListAsync();

            return eventsModel;
        }

        public async Task<int> CreateAsync(
            string title, string description, DateTime starts, 
            DateTime ends, IFormFile pictureFile, string address, 
            string latitude, string longitude, string creatorId)
        {
            var picId = await this.pictureService.UploadImageAsync(creatorId, pictureFile);

            var eventObj = new Event
            {
                Address = address,
                CreatorId = creatorId,
                Description = description,
                EndDate = ends,
                StartDate = starts,
                Latitude = double.Parse(latitude, CultureInfo.InvariantCulture),
                Longitude = double.Parse(longitude, CultureInfo.InvariantCulture),
                Title = title,
                CloudinaryImageId = picId
            };

            await this.db.Events.AddAsync(eventObj);

            await this.db.SaveChangesAsync();

            return eventObj.Id;
        }

        public async Task<bool> EditAsync(int id, string userId, string title, string description, 
            DateTime starts, DateTime ends, string address, string latitude, string longitude)
        {
            var eventToUpdate = await this.db.FindAsync<Event>(id);

            var creatorId = eventToUpdate.CreatorId;

            if (userId != creatorId)
            {
                return false;
            }

            eventToUpdate.Title = title;
            eventToUpdate.Description = description;
            eventToUpdate.StartDate = starts;
            eventToUpdate.EndDate = ends;
            eventToUpdate.Address = address;
            eventToUpdate.Latitude = double.Parse(latitude, CultureInfo.InvariantCulture);
            eventToUpdate.Longitude = double.Parse(longitude, CultureInfo.InvariantCulture);

            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<TModel> GetAsync<TModel>(int id)
        {
            var eventModel = await this.db.Events
                .Where(e => e.Id == id)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return eventModel;
        }

        public async Task<bool> Participate(int id, string userId)
        {
            var eventFromDb = await this.db.Events.Include(e => e.Participants).FirstOrDefaultAsync(e => e.Id == id);

            bool isUserParticipant = eventFromDb.Participants.Any(eu => eu.ParticipantId == userId);

            if (isUserParticipant)
            {
                return false;
            }

            var eventUser = new EventUser
            {
                EventId = id,
                ParticipantId = userId
            };

            await this.db.AddAsync(eventUser);

            await this.db.SaveChangesAsync();

            return true;
        }

        public async Task<int> TotalCountAsync()
        {
            int count = await this.db.Events.CountAsync();

            return count;
        }
    }
}
