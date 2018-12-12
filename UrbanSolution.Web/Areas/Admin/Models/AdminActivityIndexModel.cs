using System.Collections.Generic;
using UrbanSolution.Services.Admin.Models;

namespace UrbanSolution.Web.Areas.Admin.Models
{
    public class AdminActivityIndexModel
    {
        public IEnumerable<AdminActivitiesListingServiceModel> Activities { get; set; }

        public string AdminUserName { get; set; }
    }
}
