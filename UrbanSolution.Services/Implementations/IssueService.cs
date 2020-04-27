namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System;
    using System.Linq;
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

        public async Task<(int, IEnumerable<TModel>)> AllAsync<TModel>(
            bool isApproved, int rowsCount, int page, string regionFilter, string typeFilter, string sortType)
        {
            bool isRegionParsed = Enum.TryParse(regionFilter, true, out RegionType regionType);
            bool filterByRegion = isRegionParsed && regionType != RegionType.All;
            bool filterByType = Enum.TryParse(typeFilter, true, out IssueType issueType);

            var issues = this.db.UrbanIssues.AsNoTracking().Where(i => i.IsApproved);

            if (filterByRegion)
            {
                issues = issues.Where(i => i.Region == regionType);
            }

            if (filterByType)
            {
                issues = issues.Where(i => i.Type == issueType);
            }

            if (sortType == null)
            {
                sortType = SortDesc;
            }

            issues = sortType == SortAsc
                ? issues.OrderBy(i => i.PublishedOn)
                : issues.OrderByDescending(i => i.PublishedOn);

            int filteredIssuesCount = await issues.CountAsync();
            int pagesCount = (int)Math.Ceiling((double)filteredIssuesCount / (IssuesOnRow * rowsCount));

            var issuesModel = await issues
                .Skip((page - 1) * IssuesOnRow * rowsCount)
                .Take(IssuesOnRow * rowsCount)
                .To<TModel>()
                .ToListAsync();

            return (pagesCount, issuesModel);
        }

        public async Task<int> TotalAsync(bool isApproved)
        {
            var countOfIssues = await this.db
                .UrbanIssues.AsNoTracking()
                .Where(i => i.IsApproved == isApproved)
                .CountAsync();

            return countOfIssues;
        }

        public async Task<TModel> GetAsync<TModel>(int id)
        {
            var model = await this.db
                .UrbanIssues.AsNoTracking()
                .Where(i => i.Id == id)
                .To<TModel>()
                .FirstOrDefaultAsync();

            return model;
        }

        public async Task<IEnumerable<TModel>> AllMapInfoDetailsAsync<TModel>(
            bool areApproved, RegionType? region)
        {
            bool takeAllRegions = region == RegionType.All;

            var issues = this.db.UrbanIssues.AsNoTracking()
                .Where(i => i.IsApproved == areApproved);

            if (!takeAllRegions)
            {
                issues = issues.Where(i => i.Region == region);
            }

            var result = await issues
                .To<TModel>()
                .ToListAsync();

            return result;
        }
    }
}
