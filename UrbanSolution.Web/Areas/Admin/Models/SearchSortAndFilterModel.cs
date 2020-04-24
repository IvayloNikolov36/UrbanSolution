namespace UrbanSolution.Web.Areas.Admin.Models
{
    using static UrbanSolutionUtilities.WebConstants;

    public class SearchSortAndFilterModel
    {
        public string SearchType { get; set; }

        public string SearchText { get; set; }

        public string Filter { get; set; }

        public string SortBy { get; set; }

        public string SortType { get; set; }

        public int Page { get; set; } = InitalPage;
    }
}
