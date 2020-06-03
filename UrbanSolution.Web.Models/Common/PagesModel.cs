namespace UrbanSolution.Web.Models.Common
{
    using System;

    public class PagesModel
    {
        public int ItemsOnPage { get; set; }

        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)this.TotalItems / this.ItemsOnPage);

        public int NextPage => this.CurrentPage == this.TotalPages ? this.TotalPages : this.CurrentPage + 1;

        public int PreviousPage => this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;

        public string SortBy { get; set; }

        public string SortType { get; set; }

        public string SearchType { get; set; }

        public string SearchText { get; set; }

        public string Filter { get; set; }
    }
}
