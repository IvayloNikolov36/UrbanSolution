namespace UrbanSolution.Web.Models
{
    using Services.Utilities;
    using System;
    using System.Collections.Generic;
    using UrbanSolution.Services.Models;

    public class IssuesListingViewModel
    {
        public IEnumerable<UrbanIssuesListingServiceModel> Issues { get; set; }

        public int TotalIssues { get; set; }

        public int TotalPages =>
            (int) Math.Ceiling((double) this.TotalIssues / ServiceConstants.IssuesPageSize);

        public int CurrentPage { get; set; }

        public int PreviousPage => this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;

        public int NextPage => this.CurrentPage == this.TotalPages ? this.TotalPages : this.CurrentPage + 1;

        public bool UseCarousel { get; set; } = false;

    }
}
