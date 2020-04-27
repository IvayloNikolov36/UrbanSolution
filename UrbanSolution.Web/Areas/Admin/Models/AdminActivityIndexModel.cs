namespace UrbanSolution.Web.Areas.Admin.Models
{
    using System.Collections.Generic;
    using UrbanSolution.Services.Admin.Models;
    using UrbanSolution.Web.Models;

    public class AdminActivityIndexModel
    {
        public IEnumerable<AdminActivitiesListingServiceModel> Activities { get; set; }

        public string AdminUserName { get; set; }

        public PagesModel PagesModel { get; set; }
    }
}
