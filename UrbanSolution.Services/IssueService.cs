using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanSolution.Data;
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
            var issues = await this.db.UrbanIssues.Where(i => i.IsApproved == isApproved).Select(i => new UrbanIssuesListingServiceModel
            {
                Id = i.Id,
                Name = i.Name,
                IssuePictureUrl = i.IssuePictureUrl,
                Publisher = i.Publisher.UserName,
                PublishedOn = i.PublishedOn,
                HasResolved = i.ResolvedIssueId != null,
                IsApproved = i.IsApproved
            }).ToListAsync();

            return issues;
        }

        public async Task<UrbanIssueDetailsServiceModel> DetailsAsync(int id)
        {
            return await this.db
                .UrbanIssues.Where(i => i.Id == id)
                .Select(i => new UrbanIssueDetailsServiceModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    IssuePictureUrl = i.IssuePictureUrl,
                    Publisher = i.Publisher.UserName,
                    PublishedOn = i.PublishedOn,
                    AddressStreet = i.AddressStreet,
                    StreetNumber = i.StreetNumber,
                    HasResolved = i.ResolvedIssue != null,
                    IsApproved = i.IsApproved,
                    Region = i.Region.ToString(),
                    Type = i.Type.ToString(),
                    Latitude = i.Latitude.ToString().Replace(",", "."),
                    Longitude = i.Longitude.ToString().Replace(",", ".")
                })
                .FirstOrDefaultAsync();
        }

        public  IEnumerable<IssueMapInfoBoxDetailsServiceModel> AllMapInfoDetails(bool areApproved)
        {
            return this.db.UrbanIssues.Where(i => i.IsApproved == areApproved).Select(i =>
                new IssueMapInfoBoxDetailsServiceModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Latitude = i.Latitude.ToString().Replace(",", "."),
                    Longitude = i.Longitude.ToString().Replace(",", ".")
                }).ToList();
        }
    }
}
