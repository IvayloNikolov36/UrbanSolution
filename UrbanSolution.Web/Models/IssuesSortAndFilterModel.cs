namespace UrbanSolution.Web.Models
{
    using Infrastructure;

    public class IssuesSortAndFilterModel
    {
        public string SortType { get; set; }

        public string RegionFilter { get; set; }

        public string TypeFilter { get; set; }

        public int RowsCount { get; set; } = WebConstants.IssuesRows;

        public int Page { get; set; } = WebConstants.InitalPage;
    }
}
