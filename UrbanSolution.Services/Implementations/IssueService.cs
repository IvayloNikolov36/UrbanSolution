namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public class IssueService : IIssueService
    {
        private readonly UrbanSolutionDbContext db;

        public IssueService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(
            bool isApproved, int rowsCount, int page, string regionFilter, string typeFilter, string sortType)
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

            var query = this.db.UrbanIssues.Where(predicate);

            if (sortType == null)
            {
                sortType = SortDesc;
            }

            query = sortType == SortAsc
                ? query.OrderBy(i => i.PublishedOn)
                : query.OrderByDescending(i => i.PublishedOn);

            var issues = await query.Skip((page - 1) * IssuesOnRow * rowsCount)
                .Take(IssuesOnRow * rowsCount)
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
