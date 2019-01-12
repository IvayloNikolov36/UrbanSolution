namespace UrbanSolution.Services.Tests.Seed
{
    using Data;
    using System.Threading.Tasks;
    using UrbanSolution.Models;

    public class ResolvedCreator
    {
        private const string DefaultDescription = "DefaultDescription"; 

        public static ResolvedIssue Create(string publisherId, int issueId, int picId)
        {
            return new ResolvedIssue
            {
                Id = ++issueId,
                PublisherId = publisherId,
                UrbanIssueId = issueId,
                CloudinaryImageId = picId,
                Description = DefaultDescription
            };

        }

        public static async Task<ResolvedIssue> Create(UrbanSolutionDbContext db)
        {
            var manager = UserCreator.Create(null);
            await db.AddRangeAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.All);
            await db.AddAsync(issue);

            var pic = ImageInfoCreator.Create();
            await db.AddAsync(pic);

            var resolved = ResolvedCreator.Create(manager.Id, issue.Id, pic.Id);
            await db.AddAsync(resolved);

            issue.ResolvedIssue = resolved;

            await db.SaveChangesAsync();

            return resolved;
        }

        public static async Task<(User, ResolvedIssue)> CreateResolvedAndManager(UrbanSolutionDbContext db)
        {
            var manager = UserCreator.Create(null);
            await db.AddRangeAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.All);
            await db.AddAsync(issue);

            var pic = ImageInfoCreator.Create();
            await db.AddAsync(pic);

            var resolved = ResolvedCreator.Create(manager.Id, issue.Id, pic.Id);
            await db.AddAsync(resolved);

            issue.ResolvedIssue = resolved;

            await db.SaveChangesAsync();

            return (manager, resolved);
        }


        public static async Task<(string, UrbanIssue, ResolvedIssue, ResolvedIssue)> CreateResolvedManagerAndIssue(UrbanSolutionDbContext db)
        {
            var manager = UserCreator.Create(null);
            await db.AddRangeAsync(manager);

            var issue = UrbanIssueCreator.CreateIssue(RegionType.All);
            var secondIssue = UrbanIssueCreator.CreateIssue(RegionType.All);
            await db.AddRangeAsync(issue, secondIssue);

            var pic = ImageInfoCreator.Create();
            var secondPic = ImageInfoCreator.Create();
            await db.AddRangeAsync(pic, secondPic);

            var resolved = ResolvedCreator.Create(manager.Id, issue.Id, pic.Id);
            var secondResolved = ResolvedCreator.Create(manager.Id, secondIssue.Id, secondPic.Id);
            await db.AddRangeAsync(resolved, secondResolved);

            issue.ResolvedIssue = resolved;
            secondIssue.ResolvedIssue = secondResolved;

            await db.SaveChangesAsync();

            return (manager.Id, secondIssue, secondResolved, resolved);
        }

    }
}
