using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
using UrbanSolution.Models;
using UrbanSolution.Services.Manager.Models;
using UrbanSolution.Services.Models;

namespace UrbanSolution.Services.Manager.Implementations
{
    public class IssueService : IIssueService
    {
        private readonly UrbanSolutionDbContext db;

        public IssueService(UrbanSolutionDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved)
        {
            var notApproved = await this.db.UrbanIssues.Where(i => i.IsApproved == isApproved).Select(i => new UrbanIssuesListingServiceModel
            {
                Id = i.Id,
                Name = i.Name,
                IssuePictureUrl = i.IssuePictureUrl,
                PublishedOn = i.PublishedOn,
                Publisher = i.Publisher.UserName,
                Latitude = i.Latitude.ToString().Replace(",", "."),
                Longitude = i.Longitude.ToString().Replace(",",".")
            }).ToListAsync();

            return notApproved;
        }

        public async Task<UrbanIssueEditServiceViewModel> GetAsync(int issueId)
        {
            var issueModel = await this.db.UrbanIssues.Where(i => i.Id == issueId).Select(i => new UrbanIssueEditServiceViewModel
            {
                Id = i.Id,
                Description = i.Description,
                Name = i.Name,
                AddressStreet = i.AddressStreet,
                StreetNumber = i.StreetNumber,
                IssuePictureUrl = i.IssuePictureUrl,
                PublishedOn = i.PublishedOn,
                Publisher = i.Publisher.UserName,
                Region = i.Region,
                Type = i.Type,
                IsApproved = i.IsApproved
            })
            .FirstOrDefaultAsync();

            return issueModel;
        }

        public async Task<int> TotalAsync(bool isApproved)
            => await this.db.UrbanIssues.Where(i => i.IsApproved == isApproved).CountAsync();

        public async Task Update(int id, string name, string issuePictureUrl, string description, RegionType region, IssueType type,
            string addressStreet, string streetNumber)
        {
            var issueFromDb = await this.db.FindAsync<UrbanIssue>(id);

            issueFromDb.Name = name;
            issueFromDb.IssuePictureUrl = issuePictureUrl;
            issueFromDb.Description = description;
            issueFromDb.Region = region;
            issueFromDb.Type = type;
            issueFromDb.AddressStreet = addressStreet;
            issueFromDb.StreetNumber = streetNumber;

            await this.db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int issueId)
        {
            var exists = await this.db.UrbanIssues.AnyAsync(i => i.Id == issueId);

            return exists;
        }

        public async Task ApproveAsync(int issueId)
        {
            var issueFromDb = await this.db.FindAsync<UrbanIssue>(issueId);

            issueFromDb.IsApproved = true;

            await this.db.SaveChangesAsync();
        }

        public async Task Delete(int issueId)
        {
            var issueToDelete = await this.db.FindAsync<UrbanIssue>(issueId);

            this.db.UrbanIssues.Remove(issueToDelete);

            await this.db.SaveChangesAsync();
        }
    }
}
