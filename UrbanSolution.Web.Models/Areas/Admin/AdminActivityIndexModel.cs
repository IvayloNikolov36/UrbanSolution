namespace UrbanSolution.Web.Models.Areas.Admin
{
    using System.Collections.Generic;
    using UrbanSolution.Web.Models.Common;

    public class AdminActivityIndexModel
    {
        public IEnumerable<AdminActivitiesListingModel> Activities { get; set; }

        public string AdminUserName { get; set; }

        public PagesModel PagesModel { get; set; }
    }
}
