using System.ComponentModel.DataAnnotations;

namespace UrbanSolution.Web.Areas.Admin.Models
{
    public class UserToRoleFormModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }

    }
}
