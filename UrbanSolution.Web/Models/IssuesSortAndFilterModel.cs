namespace UrbanSolution.Web.Models
{
    using static UrbanSolutionUtilities.WebConstants;

    public class IssuesSortAndFilterModel
    {
        public string SortType { get; set; }

        public string RegionFilter { get; set; }

        public string TypeFilter { get; set; }

        public int RowsCount { get; set; } = IssuesRows;

        public int Page { get; set; } = InitalPage;
    }
}
