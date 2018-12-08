﻿namespace UrbanSolution.Services.Manager
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using UrbanSolution.Services.Models;

    public interface IManagerIssueService
    {
        Task<IEnumerable<UrbanIssuesListingServiceModel>> AllAsync(bool isApproved, RegionType? region);

        Task<int> TotalAsync(bool isApproved);

        Task Update(int id, string name, string issuePictureUrl, string description, RegionType region, IssueType type, string addressStreet, string streetNumber);

        Task Delete(int issueId);

        Task<bool> ExistsAsync(int issueId);

        Task ApproveAsync(int issueId);
    }
}
