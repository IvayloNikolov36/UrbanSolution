namespace UrbanSolution.Web.Areas.Manager.Models
{
    using System.Collections.Generic;
    using UrbanSolution.Models;
    using UrbanSolution.Web.Models;

    public class ManagerIssuesListingViewModel
    {
        public IEnumerable<IssueTableRowViewModel> Issues { get; set; }

        public RegionType? Region { get; set; }

        public PagesModel PaginationModel { get; set; }
    }
}
