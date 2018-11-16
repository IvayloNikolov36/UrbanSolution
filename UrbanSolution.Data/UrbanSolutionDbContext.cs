using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Models;

namespace UrbanSolution.Data
{
    public class UrbanSolutionDbContext : IdentityDbContext<User>
    {
        public UrbanSolutionDbContext(DbContextOptions<UrbanSolutionDbContext> options)
            : base(options)
        {
        }
    }
}
