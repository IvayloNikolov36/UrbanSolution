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
    using Utilities;

    public class EventService : IEventService
    {
        private readonly UrbanSolutionDbContext db;
        private readonly IPictureService pictureService;

        public EventService(UrbanSolutionDbContext db, IPictureService pictureService)
        {
            this.db = db;
            this.pictureService = pictureService;
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

        public async Task<bool> EditAsync(int id, string creatorId, string userId, string title, string description, 
            DateTime starts, DateTime ends, string address, string latitude, string longitude)
        {
            if (userId != creatorId)
            {
                return false;
            }

            var eventToUpdate = await this.db.FindAsync<Event>(id);

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
