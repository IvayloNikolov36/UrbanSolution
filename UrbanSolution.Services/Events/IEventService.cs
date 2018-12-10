using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UrbanSolution.Services.Events
{
    public interface IEventService
    {
        Task<IEnumerable<TModel>> AllAsync<TModel>(int page);

        Task<int> CreateAsync(string title, string description, DateTime starts, DateTime ends,
            string pictureUrl,
            string address, double latitude, double longitude, string creatorId);

        Task<TModel> GetAsync<TModel>(int id);

        Task<bool> ExistsAsync(int id);

        Task<int> TotalCountAsync();
    }
}
