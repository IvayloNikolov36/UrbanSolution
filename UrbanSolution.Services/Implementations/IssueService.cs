using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
using UrbanSolution.Services.Mapping;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services
{
    public class IssueService : IIssueService
    {
        private UrbanSolutionDbContext db;

        public IssueService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved)
        {
            var issues = await this.db
                .UrbanIssues.Where(i => i.IsApproved == isApproved)
                .To<UrbanIssuesListingServiceModel>().ToListAsync();

            return issues;
        }

        public async Task<UrbanIssueDetailsServiceModel> DetailsAsync(int id)
        {
            return await this.db
                .UrbanIssues.Where(i => i.Id == id)
                .To<UrbanIssueDetailsServiceModel>()               
                .FirstOrDefaultAsync();
        }

        public  IEnumerable<IssueMapInfoBoxDetailsServiceModel> AllMapInfoDetails(bool areApproved)
        {
            return this.db.UrbanIssues
                          .Where(i => i.IsApproved == areApproved)
                          .To<IssueMapInfoBoxDetailsServiceModel>().ToList();
        }
    }
}
