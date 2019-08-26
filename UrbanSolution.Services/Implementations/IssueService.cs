using System;
using System.Linq.Expressions;

namespace UrbanSolution.Services.Implementations
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
        private readonly UrbanSolutionDbContext db;

        public IssueService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, int rowsCount, int page, string regionFilter, string typeFilter)
        {
            bool isRegionParsed = Enum.TryParse(regionFilter, true, out RegionType regionType);
            bool filterByRegion = isRegionParsed && regionType != RegionType.All;

            bool filterByType = Enum.TryParse(typeFilter, true, out IssueType issueType);

            Expression<Func<UrbanIssue, bool>> predicate = i => i.IsApproved == isApproved;

            if (filterByRegion && filterByType)
            {
                predicate = i => i.IsApproved == isApproved && i.Region == regionType && i.Type == issueType;
            }

            if (filterByRegion && !filterByType)
            {
                predicate = i => i.IsApproved == isApproved && i.Region == regionType;
            }

            if (filterByType && !filterByRegion)
            {
                predicate = i => i.IsApproved == isApproved && i.Type == issueType;
            }

            var issues = await this.db.UrbanIssues
                .Where(predicate)
                .OrderByDescending(i => i.PublishedOn)
                .Skip((page - 1) * IssuesPageSize * rowsCount)
                .Take(IssuesPageSize * rowsCount)
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
