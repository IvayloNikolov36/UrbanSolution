namespace UrbanSolution.Web.Areas.Admin.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserToRoleFormModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }

    }
}
