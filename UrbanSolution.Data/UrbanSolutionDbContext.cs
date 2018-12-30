namespace UrbanSolution.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Models.MappingTables;

    public class UrbanSolutionDbContext : IdentityDbContext<User>
    {
        public UrbanSolutionDbContext(DbContextOptions<UrbanSolutionDbContext> options)
            : base(options)
        {
        }

        public DbSet<UrbanIssue> UrbanIssues { get; set; }

        public DbSet<ResolvedIssue> ResolvedIssues { get; set; }

        public DbSet<AdminLog> AdminLogs { get; set; }

        public DbSet<ManagerLog> ManagerLogs { get; set; }

        public DbSet<CloudinaryImage> CloudinaryImages { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventUser> EventUsers { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Mappings

            builder.Entity<EventUser>()
                .HasKey(eu => new {eu.ParticipantId, eu.EventId});

            builder.Entity<EventUser>()
                .HasOne(eu => eu.Participant)
                .WithMany(u => u.EventsParticipations)
                .HasForeignKey(eu => eu.ParticipantId);

            builder.Entity<EventUser>()
                .HasOne(eu => eu.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(eu => eu.EventId);

            //one to zero or one
            builder.Entity<UrbanIssue>()
                .HasOne(u => u.ResolvedIssue)
                .WithOne(r => r.UrbanIssue)
                .HasForeignKey<ResolvedIssue>(r => r.UrbanIssueId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //one to many relations

            builder.Entity<Event>()
                .HasOne(e => e.Creator)
                .WithMany(u => u.EventsCreated)
                .HasForeignKey(e => e.CreatorId);           

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

            base.OnModelCreating(builder);
        }
    }
}
