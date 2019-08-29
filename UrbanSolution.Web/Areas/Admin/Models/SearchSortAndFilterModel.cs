

using System;
using System.Collections.Generic;
using UrbanSolution.Web.Infrastructure.Enums;
using UrbanSolution.Web.Infrastructure.Extensions;

namespace UrbanSolution.Web.Areas.Admin.Models
{
    public class SearchSortAndFilterModel
    {

        public string SearchType { get; set; }

        public string SearchText { get; set; }

        public string Filter { get; set; }

        public string SortBy { get; set; }

        public string SortType { get; set; }



    }
}
