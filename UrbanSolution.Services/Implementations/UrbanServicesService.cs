using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Implementations
{
    public class UrbanServicesService : IUrbanServicesService
    {
        private readonly UrbanSolutionDbContext db;

        public UrbanServicesService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<TModel>> AllAsync<TModel>()
        {
            var allServices = await this.db.UrbanServices
                .OrderByDescending(us => us.Id)
                .To<TModel>()
                .ToListAsync();

            return allServices;
        }

        public async Task<TModel> GetAsync<TModel>(int serviceId)
        {
            var urbanService = await this.db.UrbanServices
                .Where(us => us.Id == serviceId)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return urbanService;
        }

        public async Task CreateAsync(string name, string description, decimal price)
        {
            var service = new UrbanService
            {
                Name = name,
                Description = description,
                Price = price
            };

            await this.db.UrbanServices.AddAsync(service);
            await this.db.SaveChangesAsync();
        }
    }
}
