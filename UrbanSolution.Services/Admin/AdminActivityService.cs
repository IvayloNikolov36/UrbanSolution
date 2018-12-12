using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Services.Admin.Models;
using UrbanSolution.Services.Mapping;

namespace UrbanSolution.Services.Admin
{
    using Data;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class AdminActivityService : IAdminActivityService
    {
        private readonly UrbanSolutionDbContext db;

        public AdminActivityService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task WriteAdminLogInfoAsync(string adminId, string userId, string role, ActivityType activity)
        {
            var logInfo = new AdminLog
            {
                Activity = activity,
                AdminId = adminId,
                DateTime = DateTime.UtcNow,
                EditedUserId = userId
            };

            await this.db.AdminLogs.AddAsync(logInfo);
            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AdminActivitiesListingServiceModel>> AllAsync(string adminId)
        {
            var activity = await this.db.AdminLogs
                .Where(a => a.AdminId == adminId)
                .OrderByDescending(a => a.DateTime)
                .To<AdminActivitiesListingServiceModel>()
                .ToListAsync();

            return activity;
        }
    }
}
