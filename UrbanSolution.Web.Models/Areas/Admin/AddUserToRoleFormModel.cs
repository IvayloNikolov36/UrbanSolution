namespace UrbanSolution.Web.Models.Areas.Admin
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
