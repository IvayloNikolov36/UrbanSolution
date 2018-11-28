
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
using UrbanSolution.Services.Admin.Models;

namespace UrbanSolution.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UrbanSolutionDbContext db;

        public AdminUserService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<AdminUserListingServiceModel>> AllAsync()
        {
            var users = await this.db.Users.Select(u => new AdminUserListingServiceModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            }).ToListAsync();

            return users;
        }

    }
}
