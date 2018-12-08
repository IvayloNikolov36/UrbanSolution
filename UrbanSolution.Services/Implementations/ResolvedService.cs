namespace UrbanSolution.Services.Implementations
{
    using Data;
    using Mapping;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Linq;
    using System.Threading.Tasks;

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
