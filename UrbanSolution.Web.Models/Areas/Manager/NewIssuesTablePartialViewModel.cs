namespace UrbanSolution.Web.Models.Areas.Manager
{
    using System.Collections.Generic;

    public class NewIssuesTablePartialViewModel
    {
        public IEnumerable<IssueTableRowViewModel> Isssues { get; set; }

        public int PagesCount { get; set; }

        public int Page { get; set; }
    }
}
