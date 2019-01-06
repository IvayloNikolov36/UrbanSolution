﻿namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using static Utilities.ServiceConstants;

    public class IssueService : IIssueService
    {
        private UrbanSolutionDbContext db;

        public IssueService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, int page = 1)
        {
            var issues = await this.db
                .UrbanIssues.Where(i => i.IsApproved == isApproved)
                .OrderByDescending(i => i.PublishedOn)
                .Skip((page - 1) * IssuesPageSize)
                .Take(IssuesPageSize)
                .To<UrbanIssuesListingServiceModel>()
                .ToListAsync();

            return issues;
        }

        public async Task<int> TotalAsync(bool isApproved)
        {
            var countOfIssues = await this.db
                .UrbanIssues.Where(i => i.IsApproved == isApproved)
                .CountAsync();

            return countOfIssues;
        }

        public async Task<TModel> GetAsync<TModel>(int id)
        {
            var model = await this.db
                .UrbanIssues.Where(i => i.Id == id)
                .To<TModel>()               
                .FirstOrDefaultAsync();

            return model;
        }

        public async Task<IEnumerable<IssueMapInfoBoxDetailsServiceModel>> AllMapInfoDetailsAsync(
            bool areApproved, RegionType? region)                                   
        {
            bool takeAllRegions = region == RegionType.All;

            var issues = this.db.UrbanIssues
                .Where(i => i.IsApproved == areApproved);

            if (!takeAllRegions)
            {
                issues = issues.Where(i => i.Region == region);
            }

            var result = await issues
                .To<IssueMapInfoBoxDetailsServiceModel>()
                .ToListAsync();

            return result;
        }
    }
}
