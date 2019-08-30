namespace UrbanSolution.Web.Models
{
    using static UrbanSolutionUtilities.WebConstants;

    public class IssuesSortAndFilterModel
    {
        //TODO: make validation
        public string SortType { get; set; }

        public string RegionFilter { get; set; }

        public string TypeFilter { get; set; }

        public int RowsCount { get; set; }

        public int Page { get; set; } = InitalPage;
    }
}
