namespace UrbanSolution.Web.Areas.Manager.Models
{
    using System.Collections.Generic;

    public class NewIssuesTablePartialViewModel
    {
        public IEnumerable<IssueTableRowViewModel> Isssues { get; set; }

        public int PagesCount { get; set; }

        public int Page { get; set; }
    }
}
