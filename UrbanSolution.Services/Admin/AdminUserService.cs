namespace UrbanSolution.Services.Admin
{
    using Data;
    using Mapping;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class AdminUserService : IAdminUserService
    {
        private readonly UrbanSolutionDbContext db;

        public AdminUserService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<AdminUserListingServiceModel>> AllAsync()
        {
            var users = await this.db
                .Users
                .To<AdminUserListingServiceModel>()
                .ToListAsync();

            return users;
        }

    }
}
