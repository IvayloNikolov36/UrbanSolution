namespace UrbanSolution.Web.Areas.Blog.Models
{
    using System.Collections.Generic;
    using UrbanSolution.Services.Blog.Models;
    using UrbanSolution.Web.Models;

    public class ArticleListingViewModel
    {
        public IEnumerable<BlogArticleListingServiceModel> Articles { get; set; }

        public PagesModel PagesModel { get; set; }

    }
}