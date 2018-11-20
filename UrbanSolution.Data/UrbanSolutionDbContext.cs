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

        public DbSet<UrbanIssue> UrbanIssues { get; set; }

        public DbSet<ResolvedIssue> ResolvedIssues { get; set; }

        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.Publisher)
                .HasForeignKey(u => u.PublisherId);

            builder.Entity<User>()
                .HasMany(u => u.UrbanIssues)
                .WithOne(ui => ui.Publisher)
                .HasForeignKey(u => u.PublisherId);

            builder.Entity<User>()
                .HasMany(u => u.ResolvedIssues)
                .WithOne(u => u.Publisher)
                .HasForeignKey(u => u.PublisherId);

            builder.Entity<ResolvedIssue>()
                .HasMany(e => e.Comments)
                .WithOne(c => c.Target)
                .HasForeignKey(e => e.TargetId);

            base.OnModelCreating(builder);
        }
    }
}
