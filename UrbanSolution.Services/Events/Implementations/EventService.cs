using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;
using UrbanSolution.Services.Utilities;


namespace UrbanSolution.Services.Events.Implementations
{
    public class EventService : IEventService
    {
        private UrbanSolutionDbContext db;

        public EventService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<TModel>> AllAsync<TModel>(int page)
        {
            var eventsModel = await this.db
                .Events
                .OrderByDescending(e => e.Id)
                .Skip((page - 1) * ServiceConstants.EventsPageSize)
                .Take(ServiceConstants.EventsPageSize)
                .To<TModel>()
                .ToListAsync();

            return eventsModel;
        }

        public async Task<int> CreateAsync(string title, string description, DateTime starts, DateTime ends,
            string pictureUrl,
            string address, double latitude, double longitude, string creatorId)
        {
            var eventObj = new Event
            {
                Address = address,
                CreatorId = creatorId,
                Description = description,
                EndDate = ends,
                StartDate = starts,
                Latitude = latitude,
                Longitude = longitude,
                Title = title,
                PictureUrl = pictureUrl
            };

            await this.db.Events.AddAsync(eventObj);

            int eventId = await this.db.SaveChangesAsync();

            return eventId;
        }

        public async Task<TModel> GetAsync<TModel>(int id)
        {
            var eventModel = await this.db.Events
                .Where(e => e.Id == id)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return eventModel;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            bool exists = await this.db.Events.AnyAsync(e => e.Id == id);

            return exists;
        }

        public async Task<int> TotalCountAsync()
        {
            int count = await this.db.Events.CountAsync();

            return count;
        }
    }
}
