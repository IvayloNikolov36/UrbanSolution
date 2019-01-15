namespace UrbanSolution.Services.Events
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEventService
    {
        Task<IEnumerable<TModel>> AllAsync<TModel>(int page = 1);

        Task<int> CreateAsync(string title, string description, DateTime starts, DateTime ends, 
            IFormFile pictureFile, string address, string latitude, string longitude, string creatorId);

        Task<bool> EditAsync(int id, string userId, string title, string description,
            DateTime starts, DateTime ends, string address, string latitude, string longitude);

        Task<TModel> GetAsync<TModel>(int id);

        Task<int> TotalCountAsync();      

        Task<bool> Participate(int id, string userId);

    }
}
