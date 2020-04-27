namespace UrbanSolution.Services.Admin
{
    using Data;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;
    using static UrbanSolutionUtilities.WebConstants;

    public class AdminActivityService : IAdminActivityService
    {
        private readonly UrbanSolutionDbContext db;

        public AdminActivityService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<(int, IEnumerable<TModel>)> AllAsync<TModel>(string adminId, int page)
        {
            var query = this.db.AdminLogs.AsNoTracking()
                .Where(a => a.AdminId == adminId)
                .OrderByDescending(a => a.CreatedOn);

            int count = await query.CountAsync();

            var activityModel = await query
                .Skip((page - 1) * ActivityRowsOnPage)
                .Take(ActivityRowsOnPage)
                .To<TModel>()
                .ToListAsync();

            return (count, activityModel);
        }

        public async Task<int> WriteInfoAsync(string adminId, string userId, string role, AdminActivityType activity)
        {
            var logInfo = new AdminLog
            {
                Activity = activity,
                AdminId = adminId,
                CreatedOn = DateTime.UtcNow,
                EditedUserId = userId,
                ForRole = role
            };

            await this.db.AdminLogs.AddAsync(logInfo);
            await this.db.SaveChangesAsync();

            return logInfo.Id;
        }
   
    }
}
