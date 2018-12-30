
namespace UrbanSolution.Web.Areas.Admin.Models
{
    using System.Collections.Generic;
    using UrbanSolution.Services.Admin.Models;

    public class AdminActivityIndexModel
    {
        public IEnumerable<AdminActivitiesListingServiceModel> Activities { get; set; }

        public string AdminUserName { get; set; }
    }
}
