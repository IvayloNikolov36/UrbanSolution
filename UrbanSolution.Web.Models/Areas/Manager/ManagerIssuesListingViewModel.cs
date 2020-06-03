namespace UrbanSolution.Web.Models.Areas.Manager
{
    using System.Collections.Generic;
    using UrbanSolution.Models;
    using UrbanSolution.Web.Models.Common;

    public class ManagerIssuesListingViewModel
    {
        public IEnumerable<IssueTableRowViewModel> Issues { get; set; }

        public RegionType? Region { get; set; }

        public PagesModel PaginationModel { get; set; }
    }
}
