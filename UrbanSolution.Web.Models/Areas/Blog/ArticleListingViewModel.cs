namespace UrbanSolution.Web.Models.Areas.Blog
{
    using System.Collections.Generic;
    using UrbanSolution.Web.Models.Common;

    public class ArticleListingViewModel
    {
        public IEnumerable<BlogArticleListingModel> Articles { get; set; }

        public PagesModel PagesModel { get; set; }

    }
}