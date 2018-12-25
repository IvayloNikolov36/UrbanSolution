﻿
namespace UrbanSolution.Services.Manager.Implementations
{
    using Data;
    using Mapping;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Models.Enums;

    public class ManagerActivityService : IManagerActivityService
    {
        private readonly UrbanSolutionDbContext db;

        public ManagerActivityService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task WriteManagerLogInfoAsync(string managerId, ManagerActivityType activity)
        {
            var logInfo = new ManagerLog
            {
                ManagerId = managerId,
                Activity = activity,
                DateTime = DateTime.UtcNow
            };

            await this.db.ManagerLogs.AddAsync(logInfo);

            await this.db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ManagerActivitiesListingServiceModel>> AllAsync(string managerId)
        {
            var activity = await this.db.ManagerLogs
                .Where(m => m.ManagerId == managerId)
                .OrderByDescending(a => a.DateTime)
                .To<ManagerActivitiesListingServiceModel>()
                .ToListAsync();

            return activity;
        }

        public async Task<IEnumerable<ManagerActivitiesListingServiceModel>> AllAsync()
        {
            var activity = await this.db.ManagerLogs
                .OrderByDescending(a => a.DateTime)
                .To<ManagerActivitiesListingServiceModel>()
                .ToListAsync();

            return activity;
        }
    }
}