namespace UrbanSolution.Web.Models
{
    using static UrbanSolutionUtilities.WebConstants;

    public class IssuesSortAndFilterModel
    {
        //TODO: make validation
        public string RegionFilter { get; set; } = OptionAllRegions;

        public string TypeFilter { get; set; } = OptionAllIssuesTypes;

        public string SortType { get; set; }

        public int RowsCount { get; set; } = 1;

        public int ToPage { get; set; } = InitalPage;
    }
}
