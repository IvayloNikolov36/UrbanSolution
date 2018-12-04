using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
using UrbanSolution.Services.Mapping;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services.Implementations
{
    public class ResolvedService : IResolvedService
    {
        private readonly UrbanSolutionDbContext db;

        public ResolvedService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<ResolvedDetailsServiceModel> GetAsync(int id)
        {
            var resolvedIssue = await this.db
                .ResolvedIssues.Where(ri => ri.Id == id)
                .To<ResolvedDetailsServiceModel>()
                .FirstOrDefaultAsync();

            return resolvedIssue;
        }
    }
}
