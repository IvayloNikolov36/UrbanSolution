namespace UrbanSolution.Services.Implementations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Data;
    using Mapping;
    using Models;
    using Utilities;

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
                .Skip((page - 1) * ServiceConstants.IssuesPageSize)
                .Take(count: ServiceConstants.IssuesPageSize)
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

        public async Task<IEnumerable<IssueMapInfoBoxDetailsServiceModel>> AllMapInfoDetailsAsync(bool areApproved)
        {
            return await this.db.UrbanIssues
                          .Where(i => i.IsApproved == areApproved)
                          .To<IssueMapInfoBoxDetailsServiceModel>().ToListAsync();
        }
    }
}
