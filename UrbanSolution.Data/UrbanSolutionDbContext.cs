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

        public DbSet<Article> Articles { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UrbanIssue>()
                .HasOne(u => u.ResolvedIssue)
                .WithOne(r => r.UrbanIssue)
                .HasForeignKey<ResolvedIssue>(r => r.UrbanIssueId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<User>()
                .HasMany(u => u.UrbanIssues)
                .WithOne(ui => ui.Publisher)
                .HasForeignKey(u => u.PublisherId);

            builder.Entity<User>()
                .HasMany(u => u.ResolvedIssues)
                .WithOne(u => u.Publisher)
                .HasForeignKey(u => u.PublisherId);

            builder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.Publisher)
                .HasForeignKey(u => u.PublisherId);

            builder.Entity<User>()
                .HasMany(u => u.Ratings)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            builder.Entity<User>()
                .HasMany(u => u.PublishedArticles)
                .WithOne(a => a.Author)
                .HasForeignKey(e => e.AuthorId);

            builder.Entity<ResolvedIssue>()
                .HasMany(e => e.Comments)
                .WithOne(c => c.Target)
                .HasForeignKey(e => e.TargetId);

            base.OnModelCreating(builder);
        }
    }
}
